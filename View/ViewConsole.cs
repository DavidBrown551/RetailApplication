using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RetailApplication.View
{
    internal class ViewConsole
    {
        private const string BADINTMESSAGE = "You must input a number-only reply with a maximum size of 999,999,999.";



        public int PageNum { get; set; }
        //This value represents the page the user is currently seeing.
        //it is changed whenever a new page is output
        //The PageNum value will be used by the main program to determine what pages the user can visit

        public void DisplayLoginPage()
        {
            Console.Title = "Login Page";
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
            Console.Title = "Confirm Logout";
            Console.Clear();
            Console.WriteLine("Log out of the program? User State will not be preserved.\nLogout:Y\t Return to the program:N");

            //The PageNum value is not changed to allow the program to maintain user state if the user stays logged in. If the user logs out, they will be taken to the login screen, otherwise they will return to their previous page
            //Logout will only be allowed from menus, to avoid the difficulties of maintaining complex user states
        }

        public void DisplayExitConfirmation()
        {
            Console.Title = "Confirm Exit";
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
            
            return Console.ReadKey(true).KeyChar;
        }


        //This method gets and returns a (error-corrected) integer of maximum size 999,999,999
        public int GetIntegerInput()
        {
            int returnValue=0;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue))//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADINTMESSAGE);
                }
            } while (returnValue==0);//This is a crude method. It will hopefully work since the user is unlikely to enter zero as their employee number, but this is less than ideal. 
            
            return returnValue;
        }

        public void DisplayPer1MainMenu()
        {
            Console.Title = "Main Menu";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 1
            //Text below is preliminary
            Console.WriteLine($"Register a Transaction(A)   View Details(V)   Help With the Interface(H)   Log Out(X)   Exit Program(Esc)");
            PageNum = 2;
        }
        public void DisplayPer2MainMenu()
        {
            Console.Title = "Main Menu";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 2
            PageNum = 3;
        }
        public void DisplayPer3MainMenu()
        {
            Console.Title = "Main Menu";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 3
            PageNum = 4;
        }
        public void DisplayPer4MainMenu()
        {
            Console.Title = "Admin Main Menu";
            Console.Clear();
            //Add text to display here, showing all availible options
            PageNum = 5;
        }

        //Shows the user a list of tables to insert values into based on their employee permission level
        public void DisplayPer1InsertMenu()
        {
            Console.Title = "Add Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 1
            PageNum = 6;
        }
        public void DisplayPer2InsertMenu()
        {
            Console.Title = "Add Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 2
            PageNum = 7;
        }
        public void DisplayPer3InsertMenu()
        {
            Console.Title = "Add Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 3
            PageNum = 8;
        }
        public void DisplayPer4InsertMenu()
        {
            Console.Title = "Add Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 4
            PageNum = 9;
        }
        public void DisplayPer2UpdateMenu()
        {
            Console.Title = "Update Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 2
            PageNum = 10;
        }
        public void DisplayPer3UpdateMenu()
        {
            Console.Title = "Update Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 3
            PageNum = 11;
        }
        public void DisplayPer4UpdateMenu()
        {
            Console.Title = "Update Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 4
            PageNum = 12;
        }

        public void DisplayPer3DeleteMenu()
        {
            Console.Title = "Delete Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 3
            PageNum = 13;
        }
        public void DisplayPer4DeleteMenu()
        {
            Console.Title = "Delete Records";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 4
            PageNum = 14;
        }

        public void DisplayPer1ReportMenu()
        {
            Console.Title = "Report Menu";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 1
            PageNum = 15;
        }
        public void DisplayPer2ReportMenu()
        {
            Console.Title = "Report Menu";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 2
            PageNum = 16;
        }
        public void DisplayPer3ReportMenu()
        {
            Console.Title = "Report Menu";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 3
            PageNum = 17;
        }
        public void DisplayPer4ReportMenu()
        {
            Console.Title = "Report Menu";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 4
            PageNum = 18;
        }
        public void DisplayPer1Help() 
        {
            Console.Title = "Help Page";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 1. This text should help explain how the website works and what they can and cannot do.
            PageNum = 19;
        }
        public void DisplayPer2Help()
        {
            Console.Title = "Help Page";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 2. This text should help explain how the website works and what they can and cannot do.
            PageNum = 20;
        }
        public void DisplayPer3Help()
        {
            Console.Title = "Help Page";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 3. This text should help explain how the website works and what they can and cannot do.
            PageNum = 21;
        }
        public void DisplayPer4Help()
        {
            Console.Title = "Help Page";
            Console.Clear();
            //Add text to display here, specific to users with a per level of 4. This text should help explain how the website works and what they can and cannot do.
            PageNum = 22;
        }



        public byte[] ConcealTypedField()
        {
            //This method will obscure the password field when the employee logs in
            //Will get and store each character, replace it with a *, allow mid-string replacement of characters(FIRST PART)
            //Get characters using Console.ReadKey().KeyChar(YES)
            //This preserves capitalisation but responds to enters(HANDLED)
            //Check with .Equals(after .MD5)(handled outside this method)
            List<char> password = new List<char>();//A list is used due to its variable length
            char buffer;
            do
            {
                buffer = Console.ReadKey(true).KeyChar;//This gets the inputted character without it being printed to the console
                switch (buffer)
                {
                    case '\r':
                        //This empty case exists so the enter key is not handled as default
                        break;
                    case '\0':
                        //Likewise with this empty case(\0 is an arrow key)
                        break;
                    case '\b':
                        if (password.Count > 0)
                        {
                            password.RemoveAt(password.Count - 1);//Removes the previous character in the list
                            Console.Write('\b');
                            
                        }
                        break;
                    default:
                        password.Add(buffer);//This stores the character the user pressed
                        Console.Write("*");
                        break;
                }
                
                
            } while (!(buffer=='\r')&&password.Count==0);//When the user presses enter, their password will be verified(as long as the password is of non-zero length)

            byte[] bytepassword = new byte[password.Count];
            foreach (char item in password)
            {
                bytepassword[password.IndexOf(item)] = Convert.ToByte(item);
                //The length-unfixed List<char> is converted to a byte[] to format it for hashing
            }
            return bytepassword;
            //This is the current version of the method. There are currently three identified problems:
            //1: Pressing backspace now changes the position of the cursor, but does not remove the *.(This functionality is availible automatically when using Console.ReadLine, but obsfuscating the characters becomes harder.
            //2: Pressing backspace when there are no characters in the password list causes a System.ArgumentOutOfRangeException(fix pending testing)
            //3: The arrow keys do not move the cursor within the console(potentially beyond project scope to fix)

        }

        public void DisplayReport(string[] columnHeadings, string[] records)
        {
            Console.Clear();
            //Add conditional logic for displaying passed arrays here
            //Try avoid using windows-specific functions to help pagination
            //If impossible to do this ensure it is mentioned in documentation
            //(mostly) FIXED-core functions like Console.ReadKey are unsupported on several non-windows architechtures
            PageNum = 23;
        }
    }
}
