using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Store
    {
        public int StoreID { get; }
        public string StoreName { get; set; }
        public string Address { get; set; }//The address should be checked for formatting here
        public  decimal Rent { get; set; }    

        public Store(int storeID,string storeName, string address, decimal rent)
        {
            //This is a standard constructor
            this.StoreID = storeID;
            this.StoreName = storeName;
            this.Address = address;
            this.Rent = rent;

        }
        public override string ToString()
        {
            //Returns a string in the formatting used for inserting an sql record
            return $"(\"{StoreName}\",\"{Address}\",{Rent})";
        }

        public string ToDisplayString()
        {
            return $"{StoreID}\t{StoreName}\t{Address}\t{Rent}" ;
        }

    }
}
