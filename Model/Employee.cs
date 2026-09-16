using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailApplication.Model
{
    public class Employee
    {
        public int EmployeeID { get; }
        [StringLength(10)]
        public string FirstName { get; set;}//FirstName has a maximum length of 10 characters
        [StringLength(20)]
        public string LastName { get; set; }//LastName has a maximum length of 20 characters
        public short PerLevelID { get; set; }
        [StringLength(32,MinimumLength=32)]
        private string Password { get; set; }//Password has a maximum length of 32 characters, however the field will be handled completely internally so does not need validation.
        public string FullName { get; }

        //A generic constructor for the Employee class. Initialises the object instance.
        public Employee(int employeeId,string firstName,string lastName, short perLevelId, string password)
        {
            this.EmployeeID = employeeId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PerLevelID = perLevelId;
            this.Password = password;
            FullName = FirstName + " " + LastName;
        }


        //This method returns the Employee object formatted as in an SQL insert statement
        public override string ToString()
        {
            return $"{FirstName},{LastName},{PerLevelID},{Password}";
        }

        //This method returns the parts of the employee object that can be shown to the user. Since the PerLevelID field is a Foreign Key, the ID is passed as an out parameter so the name of the PerLevel can be added.
        public string ToDisplayString(out int perLevelId)
        {
            perLevelId = PerLevelID;
            return $"{EmployeeID}\t{FirstName}\t{LastName}\t@perLevel";
        }
    }
}
