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

            byte userPermissionLevel=0; //This value is used to determine which pages the user can access

            //After establishing a connection to the database, the login page is shown

            Password:;

            mainview.DisplayLoginPage();
            //Handle the EmployeeID here
            int enteredEmpId=0;//PLACEHOLDER
            //The input will need to be checked for incorrect characters

            //Null passwords are not currently handled and will cause a crash
            string hashedUserPassword = Convert.ToString(hash.ComputeHash(mainview.ConcealTypedField()));

            if (hashedUserPassword.Equals(connection.GetUserPassword(enteredEmpId)))
            {
                userPermissionLevel = connection.GetUserPermission(enteredEmpId);
                goto Main_Menu;
            }
            else
            {
                //send user back to password screen with error message
            }

        Main_Menu:;
            switch (userPermissionLevel)
            {
                case 1:
                    mainview.DisplayPer1MainMenu();
                    break;
                case 2:
                    mainview.DisplayPer2MainMenu();
                    break;
                case 3:
                    mainview.DisplayPer3MainMenu();
                    break;
                case 4:
                    mainview.DisplayPer4MainMenu();
                    break;
                default:
                    Console.WriteLine("Error: the user does not have a valid permission level");
                    //Current handling of this error insufficient
                    break;
            }

        }
    }
}
