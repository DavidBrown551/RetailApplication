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
        public ushort Quantity { get; }
        public DateTime Date { get; }
        public bool Filled { get; set; }
        ProductOrder(int orderId,int storeId,int productId,ushort quantity, DateTime date,bool filled)
        {
            OrderID = orderId;
            StoreID = storeId;
            ProductID = productId;
            Quantity = quantity;
            Date = date;
            Filled = filled;

        }
    }
}
