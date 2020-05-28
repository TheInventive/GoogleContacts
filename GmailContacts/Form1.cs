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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateTable()
        {
            this.contactsTableAdapter.Fill(this.contactsDBDataSet.Contacts);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (addContact form = new addContact())
            {
                DialogResult dr = form.ShowDialog();
            if (dr == DialogResult.OK)
                {
                    UpdateTable();
                }
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }
            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
            {
                using (var ctx = new ContactContext())
                {
                    var contact = new Contact()
                    {
                        ContactId = int.Parse(dataGridView1.SelectedRows[i].Cells[0].Value.ToString()),
                        FirstName = dataGridView1.SelectedRows[i].Cells[1].Value.ToString(),
                        LastName = dataGridView1.SelectedRows[i].Cells[2].Value.ToString(),
                        CompanyName = dataGridView1.SelectedRows[i].Cells[3].Value.ToString(),
                        JobTitle = dataGridView1.SelectedRows[i].Cells[4].Value.ToString(),
                        PhoneNumber = dataGridView1.SelectedRows[i].Cells[5].Value.ToString()
                    };

                    ctx.Contacts.Attach(contact);
                    ctx.Contacts.Remove(contact);

                    GoogleSync gs = new GoogleSync();
                    gs.Login();
                    gs.GetFeed();
                    foreach (Google.Contacts.Contact c in gs.Feed.Entries)
                    {
                        if (contact.IsMatch(c))
                        {
                            gs.DeleteContact(c);
                        }
                    }  
                    ctx.SaveChanges();
                    UpdateTable();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Update
            GoogleSync gs = new GoogleSync();
            gs.Login();
            gs.GetContactsFromGoogle();
            UpdateTable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Modify
        }
    }
}
