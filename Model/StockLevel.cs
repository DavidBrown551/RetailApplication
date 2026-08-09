using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class StockLevel
    {
        public int StockLevelID { get; }
        public int StoreID { get; }
        public int ProductID { get; }
        public int RmnStock { get; set; }

        public StockLevel(int stockLevelId,int storeId, int productId, int rmnStock)
        {
            StockLevelID = stockLevelId;
            StoreID = storeId;
            ProductID = productId;
            RmnStock = rmnStock;

        }
        public override string ToString()
        {
            return $"({StoreID},{ProductID},{RmnStock})";
        }

    }
}
