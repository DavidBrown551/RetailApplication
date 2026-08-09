using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class EmployeeShift
    {
        public int EmpShiftID { get; }
        public int EmpID { get; }
        public int StoreID { get; set; }
        public int JobID { get; set; }
        public TimeOnly StartTime{ get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek WeekDay { get; set; }

        public EmployeeShift(int empShiftId,int empId,int storeId,DayOfWeek weekDay ,TimeOnly startTime, TimeOnly endTime, int jobId)
        {
            EmpShiftID = empShiftId;
            EmpID = empId;
            StoreID = storeId;
            JobID = jobId;
            StartTime = startTime;
            EndTime = endTime;
            WeekDay = weekDay;

        }

        public override string ToString()
        {
            return $"({EmpID},{StoreID},{WeekDay},{StartTime},{EndTime},{JobID})";
        }

        public string ToDisplayString(out int storeId,out int employeeId,out int jobId)
        {
            storeId = StoreID;
            employeeId = EmpID;
            jobId = JobID;
            return $"{EmpShiftID}\t@store\t@employee\t@job\t{StartTime}\t{EndTime}\t{WeekDay}" ;
        }
    }
}
