using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailContacts
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }

        public Contact()
        {

        }
        public Contact(String FirstName, String LastName, String CompanyName, String JobTitle, String PhoneNumber)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.CompanyName = CompanyName;
            this.JobTitle = JobTitle;
            this.PhoneNumber = PhoneNumber;
        }
        public Contact(String ContactId,String FirstName, String LastName, String CompanyName, String JobTitle, String PhoneNumber)
        {
            this.ContactId = int.Parse(ContactId);
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.CompanyName = CompanyName;
            this.JobTitle = JobTitle;
            this.PhoneNumber = PhoneNumber;
        }

        public bool IsMatch(Contact contact)
        {
            if(this.FirstName == contact.FirstName &&
            this.LastName == contact.LastName &&
            this.CompanyName == contact.CompanyName &&
            this.JobTitle == contact.JobTitle &&
            this.PhoneNumber == contact.PhoneNumber)
            {
                return true;
            }
            return false;
        }
        public bool IsMatch(Google.Contacts.Contact contact)
        {
            if (this.FirstName == contact.Name.GivenName &&
            (contact.Name.FamilyName == null || this.LastName == contact.Name.FamilyName) &&
            (contact.Organizations.FirstOrDefault() == null || this.CompanyName == contact.Organizations.FirstOrDefault().Name ) &&
            (contact.Organizations.FirstOrDefault() == null || this.JobTitle == contact.Organizations.FirstOrDefault().JobDescription) &&
            (contact.Phonenumbers.FirstOrDefault() == null || this.PhoneNumber == contact.Phonenumbers.FirstOrDefault().Value))
            {
                return true;
            }
            return false;
        }
    }

    
}
