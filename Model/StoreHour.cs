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
        public DayOfWeek Weekday { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }//CloseTime must be after OpenTime

        public StoreHour(int storeHourId,int storeId,DayOfWeek weekday, TimeOnly openTime, TimeOnly closeTime)
        {
            this.StoreHourID = storeHourId;
            this.StoreID = storeId;
            this.Weekday = weekday;
            this.OpenTime = openTime;
            this.CloseTime = closeTime;
        }

        public override string ToString()
        {
            return $"({StoreID},{Weekday},{OpenTime.ToString("hh:mm:ss")},{CloseTime.ToString("hh:mm:ss")})";
        }

        public string ToDisplayString(out int storeId)
        {
            storeId = StoreID;
            return $"{StoreHourID}\t@store\t{Weekday}\t{OpenTime}\t{CloseTime}";
        }
    }
}
