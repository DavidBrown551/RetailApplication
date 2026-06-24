using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Product
    {
        public int ProductID { get; }
        public int ProviderID { get; }
        public string ProductName { get; set; }
        public Decimal CustomerPrice { get; set; }
        public Decimal SupplyPrice { get; set; }
        public string Amount { get; set; }

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

    }
}
