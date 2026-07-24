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
            hash = MD5.Create();

            byte userPermissionLevel=0; //This value is used to determine which pages the user can access

            //After establishing a connection to the database, the login page is shown

            Login:;

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
                //send user back to password screen with error message
                goto Login;
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
                    mainview.DisplayMessage("Error: the user does not have a valid permission level");
                    //Current handling of this error insufficient
                    break;
            }
            goto HandleCharInput;

        HandleCharInput:;
            char buffer= mainview.GetCharacterInput();
            switch (mainview.PageNum)
            {
                case 2://Main menu(Perlevel1)
                    switch (buffer)
                    {
                        
                        default:
                            break;
                    }
                    break;
                case 3://Main menu(Perlevel2)
                    switch (buffer)
                    {

                        default:
                            break;
                    }
                    break;
                case 4://Main menu(Perlevel3)
                    switch (buffer)
                    {

                        default:
                            break;
                    }
                    break;
                case 5://Main menu(Perlevel4)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 6://Insert menu(Perlevel1)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 7://Insert menu(Perlevel2)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 8://Insert menu(Perlevel3)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 9://Insert menu(Perlevel4)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 10://Update menu(Perlevel2)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 11://Update menu(Perlevel3)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 12://Update menu(Perlevel4)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 13://Delete menu(Perlevel3)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 14://Delete menu(Perlevel4)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 15://Report menu(perLevel1)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 16://Report menu(perLevel2)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 17://Report menu(perLevel3)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 18://Report menu(perLevel4)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 19://Help(perLevel1)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 20://Help(perLevel2)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 21://Help(perLevel3)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 22://Help(perLevel4)
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;
                case 23://Report view
                    switch (buffer)
                    {
                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
