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

            //The below block handles getting the user password and sending it to be verified(UNFINISHED)
            byte[] bytes = new byte[200];
            int a=0;
            foreach (char character in mainview.ConcealTypedField())
            {
                bytes[a]=  Convert.ToByte(character);
                a++;
            }
            hash.ComputeHash(bytes);

        }
    }
}
