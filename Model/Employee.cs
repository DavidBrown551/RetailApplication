using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Employee
    {
        public int EmployeeID { get; }
        public string FirstName { get; set; }//FirstName has a maximum length of 10 characters
        public string LastName { get; set; }//LastName has a maximum length of 20 characters
        public short PerLevelID { get; set; }
        private string Password { get; set; }//Password has a maximum length of 32 characters, however the field will be handled completely internally


        //A generic constructor for the Employee class. Initialises the object instance.
        public Employee(int employeeId,string firstName,string lastName, short perLevelId, string password)
        {
            this.EmployeeID = employeeId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PerLevelID = perLevelId;
            this.Password = password;
        }

        public override string ToString()
        {
            return $"({FirstName},{LastName},{PerLevelID},{Password})";
        }

    }
}
