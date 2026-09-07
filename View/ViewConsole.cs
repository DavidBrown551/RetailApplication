using Microsoft.IdentityModel.Tokens;
using RetailApplication.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RetailApplication.View
{
    internal class ViewConsole
    {

        //The following are the error messages relating to data input and output that will be displayed to the user
        private const string TOOMANYOPTIONSMESSAGE = "Too many options to display choices for, please exit the page and report this error.";
        private const string BADINTMESSAGE = "You must input a number-only reply with a maximum size of ";
        private const string BADOPTIONINTMESSAGE = "You must input a number-only reply not less or greater than the displayed option numbers.";
        private const string INVALIDPERLEVELMESSAGE = "Error: the user does not have a valid permission level";
        private const string READONLYFIELDMESSAGE = "Error: This field is read-only. Please try updating a different field.";
        private const string INVALIDCHARINPUTMESSAGE = "Error: That is not a character recognised by the system. Please try again.";
        private const string BADDECIMALMESSAGE = "You must input a decimal with at most one decimal point, of maximum size:";


        //The following are the page titles
        private const string HELP_TITLE = "Help Page";
        private const string MAINMENU_TITLE = "Main Menu";
        private const string CREATEMENU_TITLE = "Add Records";
        private const string UPDATEMENU_TITLE = "Update Records";
        private const string REPORTMENU_TITLE = "Report Menu";


        static int optionsOnPage;

        //The arrays of valid characters used by the program to check user input.
        #region validCharArrays

        //The current length of the following array is 23 pages
        //This is the array of all the valid characters for each page in the application. The array reference of the valid characters for a page is equal to its PageNum value.
        //O allows the user to choose a generic option
        static char[][] AllValidChars =
            [
            ['a'],//PageNum=0,Page=login
            ['c','C','h','H','r','R','l','L','\e'],//PageNum=1, Page=Per1MainMenu
            ['c','C','h','H','r','R','l','L','\e','u','U'],//PageNum=2, Page=Per2MainMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','d','D'],//PageNum=3, Page=Per3MainMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','d','D'],//PageNum=4, Page=Per4MainMenu
            ['m','M','h','H','r','R','l','L','\e','o','O'],//PageNum=5, Page=Per1InsertMenu
            ['m','M','h','H','r','R','l','L','\e','u','U','o','O'],//PageNum=6, Page=Per2InsertMenu
            ['m','M','h','H','r','R','l','L','\e','u','U','d','D','o','O'],//PageNum=7, Page=Per3InsertMenu
            ['m','M','h','H','r','R','l','L','\e','u','U','d','D','o','O'],//PageNum=8, Page=Per4InsertMenu
            ['c','C','h','H','r','R','l','L','\e','m','M','o','O'],//PageNum=9, Page=Per2UpdateMenu
            ['c','C','h','H','r','R','l','L','\e','m','M','d','D','o','O'],//PageNum=10, Page=Per3UpdateMenu
            ['c','C','h','H','r','R','l','L','\e','m','M','d','D','o','O'],//PageNum=11, Page=Per4UpdateMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','m','M','o','O'],//PageNum=12, Page=Per3DeleteMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','m','M','o','O'],//PageNum=13, Page=Per4DeleteMenu
            ['c','C','h','H','m','M','l','L','\e','o','O'],//PageNum=14, Page=Per1ReportMenu
            ['c','C','h','H','m','M','l','L','\e','u','U','o','O'],//PageNum=15, Page=Per2ReportMenu
            ['c','C','h','H','m','M','l','L','\e','u','U','d','D','o','O'],//PageNum=16, Page=Per3ReportMenu
            ['c','C','h','H','m','M','l','L','\e','u','U','d','D','o','O'],//PageNum=17, Page=Per4ReportMenu
            ['c','C','m','M','r','R','l','L','\e'],//PageNum=18, Page=Per1Help
            ['c','C','m','M','r','R','l','L','\e','u','U'],//PageNum=19, Page=Per2Help
            ['c','C','m','M','r','R','l','L','\e','u','U','d','D'],//PageNum=20, Page=Per3Help
            ['c','C','m','M','r','R','l','L','\e','u','U','d','D'],//PageNum=21, Page=Per4Help
            [],//PageNum=22, Page=DisplayReport
            ['\e','l','L','o','O'],//PageNum=23, Page=Update/Insert/Delete page
            [],//PageNum=24,
            [],//PageNum=25,

            ];
        //The AllValidChars array has characters in both cases to avoid having to set the character value to always be uppercase/lowercase
        //all of the insert,update,delete,and report menus have the generic option characters as well


        #endregion

        public int PageNum { get; set; }
        //This value represents the page the user is currently seeing.
        //it is changed whenever a new page is output
        //The PageNum value will be used by the main program to determine what pages the user can visit


        #region DisplayMiscPages
        //This method displays the login page to the user. 
        public void DisplayLoginPage()
        {
            Console.Title = "Login Page";
            Console.Clear();
            Console.WriteLine("Please Enter your Employee Number:");
            Console.WriteLine();
            Console.WriteLine("Please Enter your Password:");
            Console.SetCursorPosition(0,1);
            PageNum = 0;
            return;
        }

        
        public void DisplayLogoutConfirmation()
        {
            Console.Title = "Confirm Logout";
            Console.Clear();
            Console.WriteLine("Log out of the program? User State will not be preserved.\nLogout:Y\t Return to the program:N");

            //The PageNum value is not changed to allow the program to maintain user state if the user stays logged in. If the user logs out, they will be taken to the login screen, otherwise they will return to their previous page(realistic with current program?)
            //Logout will only be allowed from menus, to avoid the difficulties of maintaining complex user states
        }

        public void DisplayExitConfirmation()
        {
            Console.Title = "Confirm Exit";
            Console.Clear();
            Console.WriteLine("Exit the program? User state will not be preserved and the program will have to reconnect on startup\nExit the program:Y \t Return to the menu:N");
            //This method is similar in function to DisplayLogoutConfirmation, but will ask if the user wishes to close the program instead of if they wish to log out.
        }
        #endregion

        //This method is implemented here instead of using Console.WriteLine in Program.cs to comply with MVC architecture
        public void DisplayMessage(String message)
        {
            Console.WriteLine(message);
        }


        public char GetCharacterInput()
        {
            
            return Console.ReadKey(true).KeyChar;
        }

        //This method gets and returns a (error-corrected) int32, displaying a default error message if the user enters an invalid string
        public int GetIntegerInput()
        {
            bool exitLoop = false;
            int returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue))//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADINTMESSAGE + Int32.MaxValue);
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return returnValue;
        }

        //This method gets and returns an (error-corrected) integer, displaying the errorMessage string to the user if an invalid input is made
        public int GetIntegerInput(string errorMessage)
        {
            bool exitLoop=false;
            int returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue))//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(errorMessage);
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);
            
            return returnValue;
        }

        //This method gets and returns an (error-corrected) integer with the provided maximum size
        public int GetIntegerInput(int maxsize)
        {
            bool exitLoop = false;
            int returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue)|| maxsize < returnValue||returnValue<0)//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADINTMESSAGE+maxsize);
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return returnValue;
        }


        public int GetIntegerInput(int maxsize,string errorMessage)
        {
            bool exitLoop = false;
            int returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue) || maxsize < returnValue || returnValue < 0)//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(errorMessage + maxsize);
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return returnValue;
        }

        //This method gets an error-corrected decimal of a specified maximum size and precision from the user.
        public decimal GetDecimalInput(int maxsize,int maxPlaces)
        {
            bool exitLoop = false;
            decimal returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!decimal.TryParse(inputString, out returnValue) || maxsize < returnValue || returnValue < 0)//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADDECIMALMESSAGE + maxsize);
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return decimal.Round(returnValue,maxPlaces);
        }

        //This method gets the user's answer to a yes/no question, e.g. confirming the user's exit from the program 
        public bool GetYesNoInput(string userQuery)
        {
            Console.WriteLine(userQuery);
            do
            {

                char buffer = Console.ReadKey(true).KeyChar;
                if (buffer == 'y' || buffer == 'Y')
                {
                    return true;
                }
                else if (buffer == 'n' || buffer == 'N')
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("That is not a valid character. Please try again.");
                }
            } while (true);//Acceptable coding?
        }

        public string GetStringInput(char[] validChars,int maxLength)
        {
            bool validString = true;
            string userString;
            do
            {
                userString = Console.ReadLine();
                if (string.IsNullOrEmpty(userString)||userString.Length>maxLength)
                {
                    validString = false;
                    //error message needed
                    break;
                }
                foreach (var item in validChars)
                {
                    if (userString.Contains(item))
                    {
                        validString = false;
                        break;
                    }
                }
            } while (!validString);

            return userString;
        }

        public TimeOnly GetTimeInput()
        {
            string userString;
            bool validString = true;
            Console.WriteLine("Please enter a time in the format xx:yy:zz A/PM");
            do
            {
                userString = Console.ReadLine();
                if (string.IsNullOrEmpty(userString))
                {
                    validString = false;
                    //errormessage here
                }
                //try get a date/time input here. Format:xx:yy:zz aM where xx<=12>=0,59>=yy>=0, 0<=zz<=59
                //IGNORE FOR NOW
            } while (!validString);


            return TimeOnly.FromDateTime(Convert.ToDateTime(userString));
        }

        public DayOfWeek GetDayOfWeekInput()
        {
            string userString;
            bool validString = true;
            DayOfWeek returnDay=DayOfWeek.Monday;//Only set so the compiler knows this has a value- this is not a default value.
            Console.WriteLine("Please enter a day of week. This is one of:");
            Console.WriteLine("Monday");
            Console.WriteLine("Tuesday");
            Console.WriteLine("Wednesday");
            Console.WriteLine("Thursday");
            Console.WriteLine("Friday");
            Console.WriteLine("Saturday");
            Console.WriteLine("Sunday");
            do
            {
                userString = Console.ReadLine();
                if (string.IsNullOrEmpty(userString))
                {
                    validString = false;
                    //errormessage here
                    continue;
                }
                //try get a dayofweek here
                switch (userString)
                {
                    case "Monday":
                        returnDay = DayOfWeek.Monday;
                        break;
                    case "Tuesday":
                        returnDay = DayOfWeek.Tuesday;
                        break;
                    case "Wednesday":
                        returnDay = DayOfWeek.Wednesday;
                        break;
                    case "Thursday":
                        returnDay = DayOfWeek.Thursday;
                        break;
                    case "Friday":
                        returnDay = DayOfWeek.Friday;
                        break;
                    case "Saturday":
                        returnDay = DayOfWeek.Saturday;
                        break;
                    case "Sunday":
                        returnDay = DayOfWeek.Sunday;
                        break;
                    default:
                        validString = false;
                        //display error message here
                        break;
                }
            } while (!validString);

            return returnDay;
        }

        public bool HandleCharInput(out char? enteredCharacterValid)
        {
            char userKey = Console.ReadKey(true).KeyChar;
            if (AllValidChars[PageNum].Contains(userKey))
            {
                enteredCharacterValid = userKey;//If the user presses a valid key, it will be returned with the out parameter.
                return true;
            }
            enteredCharacterValid = null;
            return false;
        }

        //This method handles input that changes the page the user is on. The extInput parameter allows characters the user entered when not 
        public int GetUserPageChangeValue(char? extInput, out int genericValue)
        {
            int outInt = 0;
            char? a;
            if (extInput.HasValue)
            {
                a = extInput;
                goto userInputSkipped;
            }

            while (!HandleCharInput(out a))
            {
                Console.WriteLine(INVALIDCHARINPUTMESSAGE);
            };

        userInputSkipped:
            switch (a)
            {
                case 'm'://M=Main menu
                case 'M':
                    outInt = 0;
                    genericValue = 0;
                    break;
                case 'c'://C=create records
                case 'C':
                    outInt = 1;
                    genericValue = 0;
                    break;
                case 'H'://H=Help
                case 'h':
                    outInt = 2;
                    genericValue = 0;
                    break;
                case 'u'://U=update menu
                case 'U':
                    outInt = 3;
                    genericValue = 0;
                    break;
                case 'd'://D=delete menu
                case 'D':
                    outInt = 4;
                    genericValue = 0;
                    break;
                case 'l'://L=Logout
                case 'L':
                    outInt = 5;
                    genericValue = 0;
                    break;
                case '\e':
                    outInt = 6;
                    genericValue = 0;
                    break;
                case 'r'://Report Menu
                case 'R':
                    outInt = 7;
                    genericValue = 0;
                    break;

                case 'o'://Generic options
                case 'O':
                    outInt = 8;

                    genericValue = GetIntegerInput(optionsOnPage - 1, BADOPTIONINTMESSAGE);//This is only used where the 
                    break;
                default:
                    genericValue = 0;
                    break;
            }
            
            return outInt;
        }


        //The following region contains the methods used to display the various menus of the application.
        #region DisplayMenuMethods
        public void GotoMainMenu(int userPerLevel)
        {
            optionsOnPage = 0;
            Console.Title = MAINMENU_TITLE;
            Console.Clear();
            switch (userPerLevel)
            {
                case 1:
                    this.DisplayPer1MainMenu();
                    break;
                case 2:
                    this.DisplayPer2MainMenu();
                    break;
                case 3:
                    this.DisplayPer3MainMenu();
                    break;
                case 4:
                    this.DisplayPer4MainMenu();
                    break;
                default:
                    this.DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        public void DisplayPer1MainMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 1
            Console.WriteLine($"Register a Transaction(C)   View Details(R)   Help With the Interface(H)   Log Out(L)   Exit Program(Esc)");
            PageNum = 1;
        }
        public void DisplayPer2MainMenu()
        {

            //This is the text to be displayed here, specific to users with a per level of 2
            Console.WriteLine($"Create a record(C)   View Details(R)   Help With the Interface(H)   Log Out(L)   Exit Program(Esc)");
            Console.WriteLine($"Update Order Status(U)");
            PageNum = 2;
        }
        public void DisplayPer3MainMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine($"Create Menu(C)   Report Menu(R)   Update Menu(U)   Delete Menu(D)");
            Console.WriteLine($"Help(H)   Log Out(L)   Exit(Esc)");

            PageNum = 3;
        }
        public void DisplayPer4MainMenu()
        {
            Console.Title = "Admin Main Menu";
            //This is the text to be displayed here, showing all available options
            Console.WriteLine($"Create Menu(C)   Report Menu(R)   Update Menu(U)   Delete Menu(D)");
            Console.WriteLine($"Help(H)   Log Out(L)   Exit(Esc)");

            PageNum = 4;
        }


        public void GotoInsertMenu(int userPerLevel)
        {
            optionsOnPage = 0;
            Console.Title = CREATEMENU_TITLE;
            Console.Clear();
            switch (userPerLevel)
            {
                case 1:
                    this.DisplayPer1InsertMenu();
                    break;
                case 2:
                    this.DisplayPer2InsertMenu();
                    break;
                case 3:
                    this.DisplayPer3InsertMenu();
                    break;
                case 4:
                    this.DisplayPer4InsertMenu();
                    break;
                default:
                    this.DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        //Shows the user a list of tables to insert values into based on their employee permission level
        public void DisplayPer1InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 1
            Console.WriteLine($"Log a transaction({optionsOnPage})"); optionsOnPage++;
            PageNum = 5;
        }
        public void DisplayPer2InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 2
            Console.WriteLine($"Log a transaction({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Order a product({optionsOnPage})"); optionsOnPage++;
            PageNum = 6;
        }
        public void DisplayPer3InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine($"Log a transaction({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Order a product({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add an employee({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add shifts for an employee({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add hours for a store({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a product({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a provider({optionsOnPage})");optionsOnPage++;
            Console.WriteLine($"Add a new store({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a new job({optionsOnPage})"); optionsOnPage++;
            PageNum = 7;
        }
        public void DisplayPer4InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 4
            Console.WriteLine($"Log a transaction({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Order a product({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add an employee({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add shifts for an employee({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add hours for a store({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a product({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a provider({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a new store({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a new permission level({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Add a new job({optionsOnPage})"); optionsOnPage++;
            PageNum = 8;
        }


        public void GotoUpdateMenu(int userPerLevel)
        {
            optionsOnPage = 0;
            Console.Title = UPDATEMENU_TITLE;
            Console.Clear();
            switch (userPerLevel)
            {
                case 2:
                    this.DisplayPer2UpdateMenu();
                    break;
                case 3:
                    this.DisplayPer3UpdateMenu();
                    break;
                case 4:
                    this.DisplayPer4UpdateMenu();
                    break;
                default:
                    this.DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        public void DisplayPer2UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 2
            Console.WriteLine($"Update Order Status({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee shift({optionsOnPage})"); optionsOnPage++;
            PageNum = 9;
        }
        public void DisplayPer3UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine($"Update Order Status({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee shift({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update provider contact details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update store details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update job details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update store hours({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update product details({optionsOnPage})"); optionsOnPage++;
            PageNum = 10;
        }
        public void DisplayPer4UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 4
            Console.WriteLine($"Update Order Status({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee shifts({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update provider details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update store details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update job details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update permission levels({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update transaction({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update store hours({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update store stocks({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update product details({optionsOnPage})"); optionsOnPage++;
            PageNum = 11;
        }


        public void GotoDeleteMenu(int userPerLevel)
        {
            optionsOnPage = 0;
            Console.Title = "Delete Records";
            Console.Clear();
            switch (userPerLevel)
            {
                
                case 3:
                    this.DisplayPer3DeleteMenu();
                    break;
                case 4:
                    this.DisplayPer4DeleteMenu();
                    break;
                default:
                    this.DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }
        public void DisplayPer3DeleteMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            Console.WriteLine($"Delete Stores({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Employees({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Product Providers({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Products({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Jobs({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Store Hours({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Employee Shifts({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Note: some records may not be deleteable due to being a dependency for other records");
            PageNum = 12;
        }
        public void DisplayPer4DeleteMenu()
        {
            //Add text to display here, specific to users with a per level of 4
            Console.WriteLine($"Delete Stores({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Permission Levels({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Employees({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Product Providers({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Products({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Stock Levels({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Transactions({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Jobs({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Store Hours({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Employee Shifts({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Delete Product Orders({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Note: some records may not be deleteable due to being a dependency for other records");
            PageNum = 13;
        }



        public void GotoReportMenu(int userPerLevel)
        {
            optionsOnPage = 0;
            Console.Title = REPORTMENU_TITLE;
            Console.Clear();
            switch (userPerLevel)
            {
                case 1:
                    this.DisplayPer1ReportMenu();
                    break;
                case 2:
                    this.DisplayPer2ReportMenu();
                    break;
                case 3:
                    this.DisplayPer3ReportMenu();
                    break;
                case 4:
                    this.DisplayPer4ReportMenu();
                    break;
                default:
                    this.DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        public void DisplayPer1ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 1
            PageNum = 14;
        }
        public void DisplayPer2ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 2
            PageNum = 15;
        }
        public void DisplayPer3ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            PageNum = 16;
        }
        public void DisplayPer4ReportMenu()
        {
            //Add text to display here, specific to users with a per level of 4
            PageNum = 17;
        }



        public void GotoHelp(int userPerLevel)
        {

            Console.Title = HELP_TITLE;
            Console.Clear();
            switch (userPerLevel)
            {
                case 1:
                    this.DisplayPer1Help();
                    break;
                case 2:
                    this.DisplayPer2Help();
                    break;
                case 3:
                    this.DisplayPer3Help();
                    break;
                case 4:
                    this.DisplayPer4Help();
                    break;
                default:
                    this.DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }

        public void DisplayPer1Help() 
        {
            //Add header here
            //Add text to display here, specific to users with a per level of 1. This text should help explain how the website works and what they can and cannot do.
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            PageNum = 18;
        }
        public void DisplayPer2Help()
        {
            //Add header here
            //Add text to display here, specific to users with a per level of 2. This text should help explain how the website works and what they can and cannot do.
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            PageNum = 19;
        }
        public void DisplayPer3Help()
        {
            //Add header here
            //Add text to display here, specific to users with a per level of 3. This text should help explain how the website works and what they can and cannot do.
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            PageNum = 20;
        }
        public void DisplayPer4Help()
        {
            //Add header here
            //Add text to display here, specific to users with a per level of 4. This text should help explain how the website works and what they can and cannot do.
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            PageNum = 21;
        }

        #endregion



        public List<int> DisplayUpdateRecordsPage<T>(string[] tableColumns, List<T> recordList, out List<string> colNames,out List<string> newVals) where T:class
        {
            Console.Title = "Update Records";
            List<int> idsToUpdate=[];
            List<string> colNamesToUpdate=[];
            List<string> newValues=[];
            bool continueUpdating;
            int listIndex;

            //DisplayRecords clears the console before displaying
            List<string[]> stringRecords=new List<string[]> { };
            Type listType = typeof(T);
            FieldInfo[] fieldInfo= listType.GetFields();//now needs to return the actual ToString values of those fields
           
            foreach (var record in recordList)
            {
                string[] stringRecord=new string[fieldInfo.Length];
                int i = 0;
                foreach (var field in fieldInfo)
                {
                   stringRecord[i]= field.GetValue(record).ToString();
                    i++;
                }
                stringRecords.Add(stringRecord);
                //converts records to string representation
            }
            //could make this a method e.g. FormatRecordsToStrings

            do
            {
               listIndex= DisplayRecordOptions(stringRecords, tableColumns);
                idsToUpdate.Add((int)fieldInfo[0].GetValue(recordList[listIndex]));

                //here get column and new value
                //get each non-ID field and display with options. get value from user and assign to colNamesToUpdate.(method needed)
                Console.WriteLine("Please pick a field to modify:");
                string[] fieldNames = new string[fieldInfo.Length - 1];
                for (int i = 1; i < fieldInfo.Length; i++)
                {
                    fieldNames[i - 1] = fieldInfo[i].Name;
                }
                DisplayTableFieldOptions(fieldNames);//needs to actually get the input of the column
                //get datatype-dependent value from user and assign to newValues(method needed)
                //use fieldInfo[enteredInt+1].GetType and assign to a type variable, then switch on type to find which method to pass to.
                int optionInt= GetIntegerInput(optionsOnPage);
                colNamesToUpdate.Add(fieldNames[optionInt]);
                newValues.Add(GetTypeSpecificInputAsString(fieldInfo[optionInt+1]));

                continueUpdating = GetYesNoInput("Select more records to update?[Y/N]");
            } while (continueUpdating);
            //could show records and changes, prompting users to confirm.

            colNames = colNamesToUpdate;
            newVals = newValues;
            return idsToUpdate;
            //-re-retrieve record using these values((pagenumber-1)*(records/page)+i where inputval=x for the xth record on the page.

            //Get user input for which record to modify(done)
            //get user input for which column to modify
            //allow user to input literal to modify column in record to
            //perform input validation on literal
            //exit when valid literal aquired

        }

        private string GetTypeSpecificInputAsString(FieldInfo fieldInfo)
        {
            Dictionary<Type, int> typeTable = new Dictionary<Type, int> { { typeof(Int32), 0 }, { typeof(String), 2 } };//NOT ALL DATATYPES ADDED TO THE DICTIONARY
            switch (typeTable[fieldInfo.GetType()])
            {
                case 0:
                    return GetIntegerInput(Int32.MaxValue).ToString();//Int32
                case 1:
                    return GetIntegerInput(32767).ToString();//Int16
                case 2:
                    return GetStringInput([], fieldInfo.GetCustomAttribute<StringLengthAttribute>().MaximumLength);//String
                case 3:
                    return GetTimeInput().ToLongTimeString();//Time
                case 4:
                    return DateTime.Today.ToString();//Date(does not need to be updated due to being used for record-keeping purposes)(is updated automatically to DateTime.Now because I have no time to deal with this)
                case 5:
                    return GetDecimalInput(9999,2).ToString();//decimal
                case 6:
                    return DateTime.Now.ToString();//DateTime(should not be updated due to only being used in timestamps)
                case 7:
                    return GetDayOfWeekInput().ToString();//DayofWeek

                case 8:
                    bool boolInput= GetYesNoInput("Please enter a boolean value[Y=true,N=false]");
                    if (boolInput)
                    {
                        return "1";
                    }
                    else
                    {
                        return "0";
                    }
                default:
                    return "";//bool
                    //NEEDS WAY TO GET VALIDCHARS AND MAX LENGTH
                    //use Attributes property of FieldInfo class to find max length
                    //assume all characters are valid
                    //known to be T by me, but not compiler
            }
        }
       



        public List<int> DisplayDeleteRecordsPage<T>(string[] tableColumns, List<T> recordList) where T:class
        {
            Console.Title="Delete Records";
            bool continueDeleting;
            List<int> idsToDelete= [] ;
            List<string[]> stringRecords = new List<string[]> { };
            Type listType = typeof(T);
            FieldInfo[] fieldInfo = listType.GetFields();//now needs to return the actual string values of those fields

            foreach (var record in recordList)
            {
                string[] stringRecord = new string[fieldInfo.Length];//Length of string array=number of fields
                int i = 0;
                foreach (var field in fieldInfo)
                {
                    stringRecord[i] = field.GetValue(record).ToString();
                    i++;
                }
                stringRecords.Add(stringRecord);
                //converts records to string representation
            }
            Console.WriteLine("");
            do
            {
                int listIndex = DisplayRecordOptions(stringRecords, tableColumns);
                idsToDelete.Add((int)fieldInfo[0].GetValue(recordList[listIndex]));
                //this uses reflection to get the actual ID from the record using the list index.
                continueDeleting = GetYesNoInput("Select more records to delete?[Y/N]");
            } while (continueDeleting);
            //could prompt user for confirmation to delete records

            //This returns the IDs of the records to delete
            return idsToDelete;
            //code block functional but untested
        }



        public List<object[]> DisplayInsertRecordsPage(string[] columnNames, byte[] columnDatatypes)
        {
            Console.Title = "Insert Records";
            List<object[]> recordsToInsert = new List<object[]> { };
            bool addMoreRecords;
            Console.WriteLine($"sample header text");
            for (int j = 1; j <= columnNames.Length; j++)//loops through and prints all of the non-Primary Key columns(PK column should always be first)
            {
                Console.Write(columnNames[j]+"  ");
            }

            do
            {
                object[] fields = new object[columnDatatypes.Length];
                int i = 0;
                foreach (var column in columnDatatypes)
                {

                    //gets input from the user dependent on the datatype; date/time fields are handled automatically
                    //should print text prompting the user to enter data
                    switch (column)
                    {
                        case 0:
                            fields[i] = GetIntegerInput(int.MaxValue);//needs checking for validity
                            break;
                        case 1:
                            fields[i] = GetIntegerInput(Int16.MaxValue);
                            break;
                        case 2:
                            fields[i] = GetStringInput([], 0);//needs a way to get the max length and valid characters
                            break;
                        case 3:
                            //time
                            fields[i] = TimeOnly.FromDateTime(DateTime.Now);//Not ideal, since the time is not the same as when the record is actually saved.
                            break;
                        case 4:
                            //date
                            fields[i] = DateOnly.FromDateTime(DateTime.Now);//Should be fine
                            break;
                        case 5:
                            //decimal
                            fields[i] = GetDecimalInput(99999, 2);//A better way to restrict this would be ideal
                            break;
                        case 6:
                            //Date+time
                            fields[i] = DateTime.Now;//this and other time fields should be the last thing to be done
                            break;
                        case 7:
                            //Day of week
                            //automatically handled by system. Returns an integer from 0-6 inclusive(Sunday-Saturday). Use ToString to make it a day name before assigning.
                            fields[i] = DateTime.Now.DayOfWeek;

                            break;
                        case 8:
                            //bool
                            //currently, this can be auto-assigned to false since the only bit/boolean value in the database defaults to zero.
                            fields[i] = false;
                            break;
                        default:
                            break;
                            //all need to be assigned to variables
                            //autofill current store using current time, day of week, logged-in employee, to find empshift then find storeID of empshift(needs exception handing)
                            //challenge: how to make compiler realise that fields will be valid.
                            //challenge: how to ensure type requirements are met.
                    }
                }  
                recordsToInsert.Add(fields);
                addMoreRecords = GetYesNoInput("Do you want to add more records?[Y/N]");
            } while (addMoreRecords);

            return recordsToInsert;
        }


        public byte[] ConcealTypedField()
        {
            //This method will obscure the password field when the employee logs in
            //Will get and store each character, replace it with a *, allow mid-string replacement of characters(FIRST PART)
            //Checked for matching string with .Equals outside this method
            List<char> password = new List<char>();//A list is used due to its variable length
            char buffer;
            Console.SetCursorPosition(Console.GetCursorPosition().Left,Console.GetCursorPosition().Top+1);//Moves the cursor down one row.
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
                        if (password.Count > 0)//This conditional prevents a System.ArgumentOutOfRangeException that could occur if the user pressed backspace when the password had length=0.
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
                
                
            } while (!(buffer=='\r')||password.Count==0);//When the user presses enter, their password will be verified(as long as the password is of non-zero length)

            byte[] bytepassword = new byte[password.Count];
            foreach (char item in password)
            {
                bytepassword[password.IndexOf(item)] = Convert.ToByte(item);
                //The length-unfixed List<char> is converted to a byte[] to format it for hashing
            }
            return bytepassword;
            //This is the current version of the method. There are currently two identified problems:
            //1: Pressing backspace now changes the position of the cursor, but does not remove the *.(This functionality is availible automatically when using Console.ReadLine, but obsfuscating the characters becomes harder.
            //2: The arrow keys do not move the cursor within the console(potentially beyond project scope to fix)

        }


        //This method displays the fields in a user-chosen table, labelled with the generic option characters.
        public void DisplayTableFieldOptions(string[] columnNames)
        {
            optionsOnPage = 0;
            try
            {
                foreach (string column in columnNames)
                {
                    Console.WriteLine(column + ": " + optionsOnPage);
                    optionsOnPage++;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(TOOMANYOPTIONSMESSAGE);
                //This exception will occur if there are more columns in the table than generic option characters.
                throw;
            }
        }


        //This method prints reports to the console using the provided string arrays. Pressing any key other than enter or backspace will exit the method
        public void DisplayReport(string[] columnHeadings, List<string[]> records, string reportTitle)
        {
            Console.Title = reportTitle;
            PageNum = 22;
            int i = 0;
            int outputPageNum = 1;
            bool exitReport;
            int maxRecordsOnPage = 0;//Ideally a more elegant solution would be used. Having to define and use two variables is not ideal.
            int recordsOnPage;
            List<int> maxColumnLengths = GetMaxColumnLengths(records, columnHeadings);

            //Main pagination loop
            do
            {
                Console.Clear();
                for (int j = 0; j < columnHeadings.Length; j++)
                {
                    Console.Write(columnHeadings[j].PadRight(maxColumnLengths[j] + 1));//Prints and left-aligns the column headings.
                }
                Console.WriteLine("\n");//This block clears the console, prints the column names, and moves two lines down.

                try
                {
                    do
                    {
                        for (int j = 0; j < records[i].Length; j++)
                        {
                            Console.Write(records[i][j].PadRight(maxColumnLengths[j] + 1));
                            //Prints one field in the record, followed by a space. If the field is shorter than the corresponding column heading, the space to the right of the text will be padded with spaces to make up the difference.
                        }
                        Console.WriteLine();
                        i++;
                    } while (Console.CursorTop < (Console.BufferHeight - 2) && i < records.Count);//Stops printing records one line above the bottom of the buffer.

                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine($"Error:{e.Message}");
                    throw;
                }
                if (outputPageNum == 1)
                {
                    maxRecordsOnPage = i;
                }
                recordsOnPage = i - (maxRecordsOnPage * (outputPageNum - 1));
                Console.WriteLine($"Page {outputPageNum} of {Math.Ceiling((double)records.Count / maxRecordsOnPage)}");
            RetryInput:
                char userkey = Console.ReadKey(true).KeyChar;
                if (userkey == '\r' && (i < records.Count))
                {
                    outputPageNum++;
                    exitReport = false;
                }
                else if (userkey == '\b' && outputPageNum > 1)
                {
                    i -= (recordsOnPage + maxRecordsOnPage);
                    outputPageNum--;
                    exitReport = false;
                }
                else if (userkey == '\b' || userkey == '\r')//This case occurs when the user presses a page increment/decrement key but they are already at the maximum/minimum page number.
                {
                    goto RetryInput;//Instead of reprinting the page, input is retried.
                }
                else
                {
                    exitReport = true;
                }
            } while (!exitReport);
            //extract the relevant part of this method and adapt it into a method that handles all of the pagination/multi-printing required.(DisplayRecords)
            //note: this adaptation would have to include support for option characters.

        }

        //This method gets the maximum column lengths from a List<string[]>. The column lengths are returned as a List<int>, and are used to align the columns when displayed to the console.
        public static List<int> GetMaxColumnLengths(List<string[]> table, string[] headings)
        {
            table.Add(headings);
            // Source - https://stackoverflow.com/a/32716864
            // Posted by Tim Schmelter, modified by community. See post 'Timeline' for change history
            // Retrieved 2026-08-27, License - CC BY-SA 3.0
            List<int> Result = Enumerable.Range(0, table.First().Length)
                .Select(i => table.Max(arr => arr[i].Length))
                .ToList();
            //selects and assigns to a List<int> the greatest string length of each column.
            return Result;
        }


        //this method is used for both update and delete functions
        public int DisplayRecordOptions(List<string[]> records, string[]columnHeadings)
        {
            //return an integer representing the list index of the chosen record.
            int i = 0;
            int outputPageNum = 1;
            bool makeInput;
            int maxRecordsOnPage = 0;//Ideally a more elegant solution would be used. Having to define and use two variables is not ideal.
            int recordsOnPage;
            List<int> maxColumnLengths = GetMaxColumnLengths(records, columnHeadings);

            //Main pagination loop
            do
            {
                Console.Clear();
                optionsOnPage = 0;
                for (int j = 0; j < columnHeadings.Length; j++)
                {
                    Console.Write(columnHeadings[j].PadRight(maxColumnLengths[j] + 1));//Prints and left-aligns the column headings.
                }
                Console.WriteLine("\n");//This block clears the console, prints the column names, and moves two lines down.

                try
                {
                    do
                    {
                        for (int j = 0; j < records[i].Length; j++)
                        {
                            Console.Write(records[i][j].PadRight(maxColumnLengths[j] + 1));
                            //Prints one field in the record, followed by a space. If the field is shorter than the corresponding column heading, the space to the right of the text will be padded with spaces to make up the difference.
                            //Note: This method of printing records does not currently allow using the foreign key connections
                        }
                        Console.WriteLine();
                        i++;
                        optionsOnPage++;
                    } while (Console.CursorTop < (Console.BufferHeight - 2) && i < records.Count);//Stops printing records one line above the bottom of the buffer.

                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine($"Error:{e.Message}");
                    throw;
                }
                if (outputPageNum == 1)
                {
                    maxRecordsOnPage = i;
                }
                recordsOnPage = i - (maxRecordsOnPage * (outputPageNum - 1));
                Console.WriteLine($"Page {outputPageNum} of {Math.Ceiling((double)records.Count / maxRecordsOnPage)}\t\t<-Backspace   Enter->  Chose record:O");
            RetryInput:
                char userkey = Console.ReadKey(true).KeyChar;
                if (userkey == '\r' && (i < records.Count))
                {
                    outputPageNum++;
                    makeInput = false;
                }
                else if (userkey == '\b' && outputPageNum > 1)
                {
                    i -= (recordsOnPage + maxRecordsOnPage);
                    outputPageNum--;
                    makeInput = false;
                }
                else if (userkey == 'o' || userkey == 'O')
                {
                    //return GetIntegerInput(recordsOnPage);
                    makeInput = true;
                }
                else//This case can occur when the user presses a page increment/decrement key but they are already at the maximum/minimum page number.
                {
                    goto RetryInput;//Instead of reprinting the page, input is retried.
                }
            } while (!makeInput);//End main pagination loop
            //((pagenumber-1)*(records/page)+i
            int userValue= (GetIntegerInput(recordsOnPage-1));//this returns the list index of the record the user selected
            return ((outputPageNum - 1) * maxRecordsOnPage) + userValue;
            //needs to have a way to simply escape the update/delete page and allow normal character input
        }

        #region Parameterisation
        public bool AskIfParametersWanted()
        {
            return GetYesNoInput("Do you want to filter what the report returns?[Y/N]");
        }


        public string AskParameters(int querynum, byte[][] queryTables, string[][] columnNames, byte[][] columnDatatypes)
        {
            //Only users with a Permission Level of 3 or 4 will be using this menu. Users with a lower Permission Level will have any parameterisation they require done automatically.
            Console.Title = "Parameterisation";
            char userKey;
            int charOptionNumber = 0;
            string finalParameters = "";
            //This block prints each column in the tables that the query uses, along with a character allowing the choice of that column.
            try
            {
                foreach (var table in queryTables[querynum])
                {
                    DisplayTableFieldOptions(columnNames[table]);


                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(TOOMANYOPTIONSMESSAGE);
                throw;
            }


            userKey = Console.ReadKey(true).KeyChar;

            //Now: 
            //1. determine which column the user wants to parameterise using a switch statement with array references.
            //2. Get the position of the column in the tableColumnNames array and input the same indexes to the tableColumnDatatypes array. This will return an integer corresponding to the datatype of the column. Use this integer as the variable for a switch case pointing to blocks of code that will allow datatype-specific parameterisation.

            //Datatypes:strings,integers, booleans, date/time
            //Aggregate columns: sum, average, etc
            //Are generic options always stored in arrays? if so, generic option input can be simplified to an array reference(YES)
            //How to find which array to reference?






            return finalParameters;
        }

        //This method handles assigning parameters for all fields. 
        public string GetParameters<T>(string colName)
        {
            //String Operations supported:
            //-Starts with
            //-Ends with
            //-contains
            //-is exact
            //Int operations supported:
            //-Greater than
            //-less than
            //-equal to
            //-between(two literals)
            string[] stringPredicateBeginnings = ["@col_name LIKE \"", "@col_name LIKE \"%", "@col_name LIKE \"%", "@col_name LIKE \""];
            string[] stringPredicateEndings = ["%\"", "\"", "%\"", "\""];
            string[] intPredicates = ["@col_name =", "@col_name <", "@col_name >"];
            string[] boolPredicates = ["@col_name IS TRUE", "@col_name IS FALSE"];

            //Display heading
            //Display options
            Console.WriteLine("");

            //Get input for which parameter
            if (true)
            {

            }
            do
            {
                string rawParameter = Console.ReadLine();
                if (!string.IsNullOrEmpty(rawParameter))
                {
                    return "placeholder".Replace("@col_name", colName);//
                }
            } while (true);


        }

        #endregion
    }
}
