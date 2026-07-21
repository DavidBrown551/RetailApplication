using Microsoft.Data.SqlClient;
using RetailApplication.Controller;
using RetailApplication.View;
using System.Security.Cryptography;
namespace RetailApplication
{
    internal class Program
    {

        private static MD5 hash;

        private static StorageManager connection;
        //This is the object that will manage the connection to the Sql Database

        private static ViewConsole mainview;
        //This is the object that will be called to display a page
        //It is called mainview to leave scope for other views later on.
        
        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=RetailApplication;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            mainview = new ViewConsole();
            connection = new StorageManager(connectionString);

            //After establishing a connection to the database, the login page is shown
            mainview.DisplayLoginPage();
            //Handle the EmployeeID here
            int enteredEmpId=0;//PLACEHOLDER
            //The input will need to be checked for incorrect characters

            //The below block handles getting the user password and sending it to be verified(UNFINISHED)
            //Null passwords are not currently handled and will cause a crash
            string hashedUserPassword = Convert.ToString(hash.ComputeHash(mainview.ConcealTypedField()));//A section needs to be added here to prompt StorageController to retrieve the password corresponding to the entered employee number from the database.

            if (hashedUserPassword.Equals(connection.GetUserPassword(enteredEmpId)))
            {
                //Send User to main menu
            }
            else
            {
                //send user back to password screen with error message
            }
            
        }
    }
}
