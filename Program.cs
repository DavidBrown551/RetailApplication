using Microsoft.Data.SqlClient;
using RetailApplication.Controller;
using RetailApplication.View;
namespace RetailApplication
{
    internal class Program
    {
        private static StorageManager connection;
        //This is the object that will manage the connection to the Sql Database

        private static ViewConsole consoleview;
        //This is the object that will be called to display a page
        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=RetailApplication;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            consoleview = new ViewConsole();
            connection = new StorageManager(connectionString);
            consoleview.DisplayLoginPage();
        }
    }
}
