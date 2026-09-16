using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Transaction
    {
        public int TransactionID { get; }
        public int StoreID { get; }
        public int ProductID { get; }
        public DateTime Timestamp { get; }
        public short Quantity { get; }

        public Transaction(int transactionId,int storeId,int productId,DateTime timestamp,short quantity)
        {
            TransactionID = transactionId;
            StoreID = storeId;
            ProductID = productId;
            Timestamp = timestamp;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{StoreID},{ProductID},{Timestamp.ToString("yyyy-MM-dd hh:mm:ss")},{Quantity}";
        }

        public string ToDisplayString(out int storeId,out int productId)
        {
            storeId = StoreID;
            productId = ProductID;
            return$"{TransactionID}\t@store\t@product\t{Timestamp}\t{Quantity}" ; 
        }
    }
}
