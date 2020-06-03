using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailContacts
{
    public class ContactContext : DbContext
    {
        public ContactContext() : base("name=ContactsDBConnectionString")
        { 
        }
        public DbSet<Contact> Contacts { get; set; }

        public void AttachContext(List<Contact> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Contacts.Attach(list[i]);
            }
        }
    }
}
