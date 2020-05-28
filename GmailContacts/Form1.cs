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
            using (var ctx = new ContactContext())
            {
                var contact = new Contact()
                {
                    ContactId = int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()),
                    FirstName = dataGridView1.SelectedRows[0].Cells[1].Value.ToString(),
                    LastName = dataGridView1.SelectedRows[0].Cells[2].Value.ToString(),
                    CompanyName = dataGridView1.SelectedRows[0].Cells[3].Value.ToString(),
                    JobTitle = dataGridView1.SelectedRows[0].Cells[4].Value.ToString(),
                    PhoneNumber = dataGridView1.SelectedRows[0].Cells[5].Value.ToString()
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
                        break;
                    }
                }
                ctx.SaveChanges();
                UpdateTable();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Update
            List<Contact> list;
            
            GoogleSync gs = new GoogleSync();
            gs.Login();
            gs.GetContactsFromGoogle();
            using (var ctx = new ContactContext())
            {
                list = ctx.Contacts.ToList();
                foreach (var item in list)
                {
                    if (gs.Contacts == null) return;
                    bool found = false;
                    for (int i = 0; i < gs.Contacts.Count; i++)
                    {
                        if (item.IsMatch(gs.Contacts[i]))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        gs.CreateContact(item);

                    }
                }
            }
            gs.GetContactsFromGoogle();
            gs.WriteContactsToDatabase();
            UpdateTable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Modify
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to Modify.");
                return;
            }
            var contact = new Contact()
            {
                ContactId = int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()),
                FirstName = dataGridView1.SelectedRows[0].Cells[1].Value.ToString(),
                LastName = dataGridView1.SelectedRows[0].Cells[2].Value.ToString(),
                CompanyName = dataGridView1.SelectedRows[0].Cells[3].Value.ToString(),
                JobTitle = dataGridView1.SelectedRows[0].Cells[4].Value.ToString(),
                PhoneNumber = dataGridView1.SelectedRows[0].Cells[5].Value.ToString()
            };
            using (modifyContact form = new modifyContact(contact))
            {
                DialogResult dr = form.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    UpdateTable();
                }
            }
        }
    }
}
