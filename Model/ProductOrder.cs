using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class ProductOrder
    {
       public int OrderID { get; }
        public int StoreID { get; }
        public int ProductID { get; }
        public short Quantity { get; }
        public DateOnly Date { get; }
        public bool Filled { get; set; }
        public ProductOrder(int orderId,int storeId,int productId,short quantity, DateOnly date,bool filled)
        {
            OrderID = orderId;
            StoreID = storeId;
            ProductID = productId;
            Quantity = quantity;
            Date = date;
            Filled = filled;

        }

        public override string ToString()
        {
            return $"({StoreID},{ProductID},{Quantity},{Date.ToString("yyyy-MM-dd")},{Filled})";
        }

        public string ToDisplayString(out int storeId,out int productId)
        {
            storeId = StoreID;
            productId = ProductID;
            return $"{OrderID}\t@store\t@product\t{Quantity}\t{Date}\t{Filled}";
        }
    }
}
