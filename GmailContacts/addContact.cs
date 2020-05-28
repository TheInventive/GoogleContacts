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
    public partial class addContact : Form
    {
        public addContact()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Length == 0||
               textBox2.Text.Length == 0||
               textBox3.Text.Length == 0||
               textBox4.Text.Length == 0||
               textBox5.Text.Length == 0)
            {
                MessageBox.Show("Please enter data.");
                return;
            }
            using(var ctx = new ContactContext())
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
                    gs.GetContactsFromGoogle();
                    gs.CreateContact(contact);
                    gs.WriteContactsToDatabase();
                    
                    
                    ctx.SaveChanges();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            this.Close();
        }

    }
}
