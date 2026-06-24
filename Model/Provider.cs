using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Provider
    {
        public int ProviderID { get; }
        public string ProviderName { get; set; }//ProviderName has a maximum length of 40 characters
        public string Contact { get; set; }//Contact has a maximum length of 30 characters


        //A generic constructor for the Provider class. Initialises the object
        public Provider(int providerId, string providerName,string contact)
        {
            this.ProviderID = providerId;
            this.ProviderName = providerName;
            this.Contact = contact;
        }

        public override string ToString()
        {
            return $"({ProviderName},{Contact})";
        }
    }
}
