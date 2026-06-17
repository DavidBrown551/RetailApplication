using Microsoft.Data.SqlClient;
using RetailApplication.Controller;
using RetailApplication.View;
namespace RetailApplication
{
    internal class Program
    {
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

            //After establishing a connection to the database
            mainview.DisplayLoginPage();
        }
    }
}
