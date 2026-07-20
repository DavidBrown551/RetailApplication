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

        public void DisplayPer1MainMenu()
        {
            //Add text to display here, specific to users with a per level of 1
            PageNum = 2;
        }
        public void DisplayPer2MainMenu()
        {
            //Add text to display here, specific to users with a per level of 2
            PageNum = 3;
        }
        public void DisplayPer3MainMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            PageNum = 4;
        }
        public void DisplayPer4MainMenu()
        {
            //Add text to display here, showing all availible options
            PageNum = 5;
        }

        //Shows the user a list of tables to insert values into based on their employee permission level
        public void DisplayPer1InsertMenu()
        {
            //Add text to display here, specific to users with a per level of 1
            PageNum = 6;
        }
        public void DisplayPer2InsertMenu()
        {
            //Add text to display here, specific to users with a per level of 2
            PageNum = 7;
        }
        public void DisplayPer3InsertMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            PageNum = 8;
        }
        public void DisplayPer4InsertMenu()
        {
            //Add text to display here, specific to users with a per level of 4
            PageNum = 9;
        }
        public void DisplayPer2UpdateMenu()
        {
            //Add text to display here, specific to users with a per level of 2
            PageNum = 10;
        }
        public void DisplayPer3UpdateMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            PageNum = 11;
        }
        public void DisplayPer4UpdateMenu()
        {
            //Add text to display here, specific to users with a per level of 4
            PageNum = 12;
        }

        public void DisplayPer3DeleteMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            PageNum = 13;
        }
        public void DisplayPer4DeleteMenu()
        {
            //Add text to display here, specific to users with a per level of 4
            PageNum = 14;
        }

        public void DisplayPer1ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 1
            PageNum = 15;
        }
        public void DisplayPer2ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 2
            PageNum = 16;
        }
        public void DisplayPer3ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            PageNum = 17;
        }
        public void DisplayPer4ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 4
            PageNum = 18;
        }
        public void DisplayPer1Help() 
        {
            //Add text to display here, specific to users with a per level of 1. This text should help explain how the website works and what they can and cannot do.
            PageNum = 19;
        }
        public void DisplayPer2Help()
        {
            //Add text to display here, specific to users with a per level of 2. This text should help explain how the website works and what they can and cannot do.
            PageNum = 20;
        }
        public void DisplayPer3Help()
        {
            //Add text to display here, specific to users with a per level of 3. This text should help explain how the website works and what they can and cannot do.
            PageNum = 21;
        }
        public void DisplayPer4Help()
        {
            //Add text to display here, specific to users with a per level of 4. This text should help explain how the website works and what they can and cannot do.
            PageNum = 22;
        }



        public List<char> ConcealTypedField()
        {
            //This method will obscure the password field when the employee logs in
            //Will get and store each character, replace it with a *, allow mid-string replacement of characters
            //Get characters using Console.ReadKey().KeyChar
            //This preserves capitalisation but responds to enters
            //Check with .Equals(after .MD5)
            List<char> password = new List<char>();//A list is used due to its variable length
            char buffer;
            do
            {
                buffer = Console.ReadKey(true).KeyChar;//This gets the inputted character without it being printed to the console
                switch (buffer)
                {
                    case '\r':
                        //This empty case exists so the input is not handled as default
                        break;
                    case '\b':
                        password.RemoveAt(password.Count-1);//Removes the previous character in the list
                        break;
                    default:
                        password.Add(buffer);//This stores the character the user pressed
                        Console.Write("*");
                        break;
                }
                
                
            } while (!(buffer=='\r'));//When the user presses enter, their password will be verified
            return password;

            //This is the current version of the method. One problem remains:
            //1: Pressing backspace removes the character in the list, but does not yet remove the character from the screen
            
            //Numerical characters and the shift key have been tested and function properly
        }

        public void DisplayReport(string[] columnHeadings, string[] records)
        {
            //Add conditional logic for diplaying passed arrays here
        }
    }
}
