using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Product
    {
        public int ProductID { get; }
        public int ProviderID { get; }
        [StringLength(75)]
        public string ProductName { get; set; }//ProductName has a maximum length of 75 characters
        public Decimal CustomerPrice { get; set; }
        public Decimal SupplyPrice { get; set; }
        [StringLength(8)]
        public string Amount { get; set; }//Amount has a maximum length of 8 characters

        public Product(int productId, int providerId, string productName, decimal customerPrice, decimal supplyPrice, string amount)
        {
            this.ProductID = productId;
            this.ProviderID = providerId;
            this.ProductName = productName;
            this.CustomerPrice = customerPrice;
            this.SupplyPrice = supplyPrice;
            this.Amount = amount;
        }

        public override string ToString()
        {
            return $"({ProviderID},{ProductName},{CustomerPrice},{SupplyPrice},{Amount})";
        }
        public string ToDisplayString(out int providerId)
        {
            providerId = ProviderID;
            return $"{ProductID}\t{ProductName}\t{Amount}\t{CustomerPrice}\t{SupplyPrice}\t@provider" ;
        }
    }
}
