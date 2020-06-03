using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GmailContacts
{
    public partial class modifyContact : Form
    {
        private Contact Contact;
        public modifyContact(Contact contact)
        {
            InitializeComponent();
            label3.Text = contact.FirstName;
            label4.Text = contact.LastName;
            label5.Text = contact.CompanyName;
            label6.Text = contact.JobTitle;
            label7.Text = contact.PhoneNumber;

            textBox1.Text = contact.FirstName;
            textBox2.Text = contact.LastName;
            textBox3.Text = contact.CompanyName;
            textBox4.Text = contact.JobTitle;
            textBox5.Text = contact.PhoneNumber;
            this.Contact = contact;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 ||
               textBox2.Text.Length == 0 ||
               textBox3.Text.Length == 0 ||
               textBox4.Text.Length == 0 ||
               textBox5.Text.Length == 0)
            {
                MessageBox.Show("Please enter data.");
                return;
            }
            using (var ctx = new ContactContext())
            {
                ctx.Contacts.Attach(Contact);
                ctx.Contacts.Remove(Contact);

                GoogleSync gs = new GoogleSync();
                gs.Login();
                foreach (Google.Contacts.Contact c in gs.Contacts)
                {
                    if (Contact.IsMatch(c))
                    {
                        gs.DeleteContact(c);
                        break;
                    }
                }
                ctx.SaveChanges();
            }

            using (var ctx = new ContactContext())
            {
                var contact = new Contact(textBox1.Text,
                    textBox2.Text,
                    textBox3.Text,
                    textBox4.Text,
                    textBox5.Text);
                try
                {
                    ctx.Contacts.Add(contact);
                    GoogleSync gs = new GoogleSync();
                    gs.Login();
                    gs.CreateContact(contact);
                    gs.WriteContactsToDatabase();
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            this.Close();
        }
    }
}

