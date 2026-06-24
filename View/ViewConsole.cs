using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RetailApplication.View
{
    internal class ViewConsole
    {
        public int PageNum { get; set; }
        //This value represents the page the user is currently seeing.
        //it is changed whenever a new page is output
        //The PageNum value will be used by the main program to determine what pages the user can visit, in conjunction with the employee permission level

        public void DisplayLoginPage()
        {
            Console.Clear();
            Console.WriteLine("Please Enter your Employee Number:");
            Console.WriteLine();
            Console.WriteLine("Please Enter your Password:");
            Console.SetCursorPosition(0,1);
            PageNum = 1;
            return;
        }


        public void DisplayLogoutConfirmation()
        {
            Console.Clear();
            Console.WriteLine("Log out of the program? User State will not be preserved.\nLogout:Y\t Return to the program:N");

            //The PageNum value is not changed to allow the program to maintain user state if the user stays logged in. If the user logs out, they will be taken to the login screen, otherwise they will return to their previous page
            //Logout will only be allowed from menus, to avoid the difficulties of maintaining complex user states
        }

        public void DisplayExitConfirmation()
        {
            Console.Clear();
            Console.WriteLine("Exit the program? User state will not be preserved and the program will have to reconnect on startup\nExit the program:Y \t Return to the menu:N");
            //This method is similar in function to DisplayLogoutConfirmation, but will ask if the user wishes to close the program instead of if they wish to log out.
        }


        //This method is implemented here instead of using Console.WriteLine in Program.cs to comply with MVC architecture
        public void DisplayMessage(String message)
        {
            Console.WriteLine(message);
        }


        public char GetCharacterInput()
        {
            
            return Console.ReadKey().KeyChar;
        }

        public void DisplayMainMenu(int perlvl)
        {
            //Add text to display here, conditional on what perlvl the user has
            PageNum = 2;
        }

        //Shows the user a list of tables to insert values into based on their employee permission level
        public void DisplayInsertMenu(int perlvl)
        {
            //Add text to display here, conditional on what perlvl the user has
            PageNum = 3;
        }

        public void DisplayUpdateMenu(int perlvl)
        {
            //Add text to display here, conditional on what perlvl the user has
            PageNum = 5;
        }

        public void DisplayDeleteMenu(int perlvl)
        {
            //Add text to display here, conditional on what perlvl the user has
            PageNum = 7;
        }
        
        public void DisplayReportMenu(int perlvl)
        {
            //Add text to display here, conditional on what perlvl the user has
            PageNum = 9;
        }

        public void ConcealTypedField()
        {
            //This method will obscure the password field when the employee logs in
            //Will get and store each character, replace it with a *, allow mid-string replacement of characters
            //Get characters using Console.ReadKey().KeyChar
            //This preserves capitalisation but responds to enters
            //Check with .Equals
        }

        public void DisplayReport(string[] columnHeadings, string[] records)
        {
            //Add conditional logic for diplaying passed arrays here
        }
    }
}
