using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GmailContacts
{
    public class GoogleSync
    {
        static string[] Scopes = new string[] { "https://www.google.com/m8/feeds/" };
        private OAuth2Parameters parameters = new OAuth2Parameters();
        private RequestSettings settings; 
        private ContactsRequest cr;
        private Feed<Google.Contacts.Contact> f;
        private UserCredential credential;
        public List<Google.Contacts.Contact> Contacts { get; private set; }

        public Feed<Google.Contacts.Contact> Feed { get => f;}

        public GoogleSync()
        {
            settings = new RequestSettings("Google contacts", parameters);
            
            cr = new ContactsRequest(settings);
           

        }

        public void Login()
        { 
            try
            {
                using (var stream =
               new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                parameters.AccessToken = credential.Token.AccessToken;
                parameters.RefreshToken = credential.Token.RefreshToken;
                GetContactsFromGoogle();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void GetFeed()
        {
            f = cr.GetContacts();
          
        }
        public void GetContactsFromGoogle()
        {
            try
            {

                GetFeed();
                Contacts = f.Entries.ToList();
                
            }
            catch (Exception)
            {
                try
                {
                    GoogleWebAuthorizationBroker.ReauthorizeAsync(credential, CancellationToken.None);
                }
                catch (Exception)
                {
                    MessageBox.Show("Authorization failed.");
                }
                
            }
        }

        public void WriteContactsToDatabase()
        {
            foreach (Google.Contacts.Contact c in Contacts)
            {
                bool isPresent = false;
                Contact contact = new Contact();
                if (c.Name.GivenName == null) { continue; }
                else contact.FirstName = c.Name.GivenName;

                if (c.Name.FamilyName == null) contact.LastName = "none";
                else contact.LastName = c.Name.FamilyName;

                if (c.Organizations.FirstOrDefault() == null)
                {
                    contact.CompanyName = "none";
                    contact.JobTitle = "none";

                }
                else
                {
                    contact.CompanyName = c.Organizations.FirstOrDefault().Name;
                    contact.JobTitle = c.Organizations.FirstOrDefault().JobDescription;
                }

                if (c.Phonenumbers.FirstOrDefault() == null) contact.PhoneNumber = "none";
                else contact.PhoneNumber = c.Phonenumbers.FirstOrDefault().Value;

                using (var ctx = new ContactContext())
                {
                    try
                    {
                        var saved = ctx.Contacts.ToList();
                        for (int i = 0; i < saved.Count; i++)
                        {
                            if (saved[i].IsMatch(contact))
                            {
                                //MessageBox.Show(contact.FirstName + " is already in the database.");
                                isPresent = true;
                                break;
                            }
                        }
                        if (isPresent) continue;
                        ctx.Contacts.Add(contact);
                        ctx.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public Google.Contacts.Contact CreateContact(Contact contact)
        {
            Google.Contacts.Contact newEntry = new Google.Contacts.Contact();
            // Set the contact's name.
            newEntry.Name = new Name()
            {
                FullName = contact.FirstName+" "+contact.LastName,
                GivenName = contact.FirstName,
                FamilyName = contact.LastName,
            };
            newEntry.Content = "Notes";
            // Set the contact's phone numbers.
            newEntry.Phonenumbers.Add(new PhoneNumber()
            {
                Primary = true,
                Rel = ContactsRelationships.IsHome,
                Value = contact.PhoneNumber,
            });
            // Set the contact's IM information.
            newEntry.IMs.Add(new IMAddress()
            {
                Primary = true,
                Rel = ContactsRelationships.IsHome,
                Protocol = ContactsProtocols.IsGoogleTalk,
            });
            newEntry.Organizations.Add(new Organization()
            {
                Name = contact.CompanyName,
                JobDescription = contact.JobTitle
            });
            // Insert the contact.
            Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
            Google.Contacts.Contact createdEntry = cr.Insert(feedUri, newEntry);
            return createdEntry;
        }

        public void DeleteContact(Google.Contacts.Contact contact)
        {
            try
            {
                cr.Delete(contact); 
                
              
            }
            catch (GDataVersionConflictException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
