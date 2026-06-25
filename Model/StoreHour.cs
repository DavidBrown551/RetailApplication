using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class StoreHour
    {
        public int StoreHourID { get; }
        public int StoreID { get; }//Foreign Key
        public string Weekday { get; set; }//Weekday has a maximum length of 9 characters and must be the name of one of the days of the week
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }//CloseTime must be after OpenTime

        public StoreHour(int storeHourId,int storeId,string weekday, SqlDateTime openTime, SqlDateTime closeTime)
        {
            this.StoreHourID = storeHourId;
            this.StoreID = storeId;
            this.Weekday = weekday;
            this.OpenTime = Convert.ToDateTime(openTime);
            this.CloseTime=
        }
    }
}
