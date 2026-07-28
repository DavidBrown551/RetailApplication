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
            //This string allows connection to the database and must be changed if the file name or file path is changed.

            mainview = new ViewConsole();
            connection = new StorageManager(connectionString);
            hash = MD5.Create();

            byte userPermissionLevel; //This value is used to determine which pages the user can access

            //After establishing a connection to the database, the login page is shown
            Login:

            mainview.DisplayLoginPage();
            //Handle the EmployeeID here
            int enteredEmpId=0;//PLACEHOLDER
            //The input will need to be checked for incorrect characters(use int.TryParse)

            string hashedUserPassword = Convert.ToHexStringLower(hash.ComputeHash(mainview.ConcealTypedField()));

            if (hashedUserPassword.Equals(connection.GetUserPassword(enteredEmpId)))
            {
                userPermissionLevel = connection.GetUserPermission(enteredEmpId);
                goto Main_Menu;
            }
            else
            {
                //Send user back to password screen with error message
                goto Login;
            }

        Main_Menu:
            mainview.GotoMainMenu(userPermissionLevel);//Displays the main menu for the user
            goto HandleCharInput;

        HandleCharInput:

            int userPageChangeValue= mainview.GetUserPageChangeValue();
            switch (userPageChangeValue)
            {
                case <=20://A menu option. Further seperation of values necessary.
                    //Send control to a method for determining what this generic option points to
                    break;
                case 21://Main Menu
                    //
                    break;
                case 22://Create Menu
                    break;
                    //Other cases here
                default:
                    break;
            }
        
        }
    }
}
