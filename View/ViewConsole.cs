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
using System.Text.RegularExpressions;
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
        private const string INVALIDCHARINPUTMESSAGE = "Error: That is not a character recognised by the system in the current state. Please try again.";
        private const string BADDECIMALMESSAGE = "You must input a decimal with at most two decimal digits that is less than ";


        //The following are the page titles and headers
        private const string HELP_TITLE = "Help Page";
        private const string MAINMENU_TITLE = "Main Menu";
        private const string CREATEMENU_TITLE = "Add Records";
        private const string UPDATEMENU_TITLE = "Update Records";
        private const string REPORTMENU_TITLE = "Report Menu";
        private const string OPTIONSMESSAGE = "Enter an option(O)";
        private const string PER1MENUOPTIONS = "Register a Transaction(C)   View Details(R)   Help With the Interface(H)   Main Menu(M)   Log Out(L)   Exit Program(Esc)";
        private const string PER2MENUOPTIONS = "Create a record(C)   View Details(R)   Help With the Interface(H)   Main Menu(M)   Log Out(L)   Exit Program(Esc)\nUpdate Order Status(U)";
        private const string PER3MENUOPTIONS = "Create Menu(C)   Report Menu(R)   Update Menu(U)   Delete Menu(D)\nHelp(H)   Main Menu(M)   Log Out(L)   Exit(Esc)";
        private const string PER4MENUOPTIONS = "Create Menu(C)   Report Menu(R)   Update Menu(U)   Delete Menu(D)\nHelp(H)   Main Menu(M)   Log Out(L)   Exit(Esc)";

        static int optionsOnPage;

        //The arrays of valid characters used by the program to check user input.
        #region validCharArrays

        //The current length of the following array is 25 pages
        //This is the array of all the valid characters for each page in the application. The array reference of the valid characters for a page is equal to its PageNum value.
        //O allows the user to choose a generic option
        static readonly char[][] AllValidChars =
            [
            [],//PageNum=0,Page=login
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
            ['m','M'],//PageNum=24, Page=Logout/Exit confirmation

            ];
        //The AllValidChars array has characters in both cases to avoid having to set the character value to always be uppercase/lowercase


        //This array stores how the generic input options of each menu map to the tables/queries they reference.
        static int[][] genericInputMapping =
            [
            [6],//Insertper1
            [6,10],//Insertper2
            [6,10,2,9,8,4,3,0,7],//Insertper3
            [6,10,2,9,8,4,3,0,1,7],//Insertper4
            [10,2,9],//Updateper2
            [10,2,9,3,0,7,8,4],//Updateper3
            [10,2,9,3,0,7,1,6,8,5,4],//Updateper4
            [0,2,3,4,7,8,9],//Deleteper3
            [0,1,2,3,4,5,6,7,8,9,10],//Deleteper4
            [1,5,6,11],//Reportper1
            [1,5,6,0,7,11],//Reportper2
            [0,1,2,3,4,5,6,7,8,9,10,11],//Reportper3
            [0, 1, 2, 3, 4, 5, 6, 7,8,9,10,11]//Reportper4
            ];

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
            ClearScreen();
            Console.WriteLine("Please Enter your Employee Number:");
            Console.WriteLine();
            Console.WriteLine("Please Enter your Password:");
            Console.SetCursorPosition(0,1);//Sets the cursor to be in the field below the first line of text
            PageNum = 0;
            return;
        }

        
        public void DisplayLogoutConfirmation()
        {
            PageNum = 24;
            Console.Title = "Confirm Logout";
            ClearScreen();
            Console.WriteLine("Log out of the program? User State will not be preserved.\nLogout:Y\t Return to the program:N");

            //The PageNum value is not changed to allow the program to maintain user state if the user stays logged in. If the user logs out, they will be taken to the login screen.
            //Logout is only be allowed from menus.
        }

        public void DisplayExitConfirmation()
        {
            PageNum = 24;
            Console.Title = "Confirm Exit";
            ClearScreen();
            Console.WriteLine("Exit the program? User state will not be preserved and the program will have to reconnect on startup\nExit the program:Y \t Return to the menu:N");
            //This method is similar in function to DisplayLogoutConfirmation, but will ask if the user wishes to close the program instead of if they wish to log out.
        }
        #endregion

        //This method is implemented here instead of using Console.WriteLine in Program.cs to comply with MVC architecture
        public static void DisplayMessage(String message)
        {
            Console.WriteLine(message);
        }

        //This method clears the entire console buffer, including strings not currently visible
        public static void ClearScreen()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.Clear();
        }


        //This region contains all if the methods for getting user input of a specific type.
        #region Get[Type]_Methods
        public static char GetCharacterInput()
        {
            
            return Console.ReadKey(true).KeyChar;
        }

        //This method gets and returns a (error-corrected) int32, displaying a default error message if the user enters an invalid string
        public static int GetIntegerInput()
        {
            bool exitLoop = false;
            int returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue)||returnValue<1)//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
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
        public static int GetIntegerInput(string errorMessage)
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
        public static int GetIntegerInput(int maxsize,int minsize)
        {
            bool exitLoop = false;
            int returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue)|| maxsize < returnValue||returnValue<minsize)//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADINTMESSAGE+maxsize);//Needs to handle inputs below minsize
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return returnValue;
        }

        public static int GetIntegerInput(int maxsize,string errorMessage)
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
        public static short GetShortIntegerInput(short maxsize, short minsize)
        {
            bool exitLoop = false;
            short returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!short.TryParse(inputString, out returnValue) || maxsize < returnValue || returnValue < minsize)//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADINTMESSAGE + maxsize);//Needs to handle inputs below minsize
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return returnValue;
        }

        public static byte GetByteInput() 
        {
            bool exitLoop = false;
            byte returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!byte.TryParse(inputString, out returnValue))//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADINTMESSAGE + "255");
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return returnValue;
        }


        //This method gets an error-corrected decimal of a specified maximum size and precision from the user.
        public static decimal GetDecimalInput(int maxintsize,int maxPlaces)
        {
            bool exitLoop = false;
            decimal returnValue;
            do
            {
                string inputString = Console.ReadLine();
                if (!decimal.TryParse(inputString, out returnValue) || maxintsize+1 <= returnValue || returnValue < 0)//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADDECIMALMESSAGE + (maxintsize+1));
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);

            return decimal.Round(returnValue,maxPlaces);
        }

        //This method gets the user's answer to a yes/no question, e.g. confirming the user's exit from the program 
        public static bool GetYesNoInput(string userQuery)
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

        public static string GetStringInput(char[] inValidChars,int maxLength)
        {
            bool validString;
            string userString;
            do
            {
                validString = true;
                userString = Console.ReadLine();
                if (string.IsNullOrEmpty(userString)||userString.Length>maxLength)
                {
                    validString = false;
                    Console.WriteLine("Please enter a string with length greater than zero and less than or equal to "+maxLength);
                    continue;
                }
                foreach (var item in inValidChars)
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

        //This method returns a TimeOnly value. The user is prompted for a time in 24-hour format. The input string is checked with a regular expression, before being converted to a TimeOnly.
        public static TimeOnly GetTimeInput()
        {
            string userString;
            bool validString;
            Console.WriteLine("Please enter a time in the format hh:mm:ss (24-hour time)");
            do
            {
                validString = true;
                userString = Console.ReadLine();
                if (string.IsNullOrEmpty(userString))
                {
                    validString = false;
                    Console.WriteLine("Error: No string was input. Try again.");
                }
                else if (!Regex.IsMatch(userString, "([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]") || userString.Length > 8)//If the user input does not match the specified pattern or has a length greater than 8, it is not valid.
                {
                    validString = false;
                    Console.WriteLine("Error: The input string is not in the correct format. Please enter a time in the format hh:mm:ss (24-hour time)");
                }

            } while (!validString);


            return TimeOnly.FromDateTime(Convert.ToDateTime(userString));
        }

        //This method prompts the user to choose one of the days of the week by inputting a number. The metho then returns the corresponding Day of Week.
        public static DayOfWeek GetDayOfWeekInput()
        {
            DayOfWeek returnDay;
            Console.WriteLine("Please select a day of week. This is one of:");
            Console.WriteLine("Monday[0]");
            Console.WriteLine("Tuesday[1]");
            Console.WriteLine("Wednesday[2]");
            Console.WriteLine("Thursday[3]");
            Console.WriteLine("Friday[4]");
            Console.WriteLine("Saturday[5]");
            Console.WriteLine("Sunday[6]");
                
                switch (GetIntegerInput(6, 0))
                {
                    case 0:
                        returnDay = DayOfWeek.Monday;
                        break;
                    case 1:
                        returnDay = DayOfWeek.Tuesday;
                        break;
                    case 2:
                        returnDay = DayOfWeek.Wednesday;
                        break;
                    case 3:
                        returnDay = DayOfWeek.Thursday;
                        break;
                    case 4:
                        returnDay = DayOfWeek.Friday;
                        break;
                    case 5:
                        returnDay = DayOfWeek.Saturday;
                        break;
                    default:
                        returnDay = DayOfWeek.Sunday;
                        break;
                }

            return returnDay;
        }
        #endregion

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

        //This method handles input that changes the page the user is on. The extInput parameter allows characters the user entered when a different method had control to be processed, but is currently unused.
        public int GetUserPageChangeValue(char? extInput, out int genericValue)
        {
            int outInt = 0;
            char? a;
            if (extInput.HasValue)
            {
                a = extInput;//This block cannot currently be triggered. It is designed to handle cases where user input is collected outside this method.
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

                    genericValue = GetIntegerInput(optionsOnPage - 1, BADOPTIONINTMESSAGE);//Needs to check pagenum for table indexing
                    if (PageNum<18&&PageNum>4)
                    {
                        genericValue = genericInputMapping[PageNum-5][genericValue];
                    }
                    break;
                default:
                    //The default(which should not occur due to input validation in HandleCharInput) sends the user to the main menu
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
            ClearScreen();
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
                    DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        public void DisplayPer1MainMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 1
            Console.WriteLine(PER1MENUOPTIONS);
            PageNum = 1;
        }
        public void DisplayPer2MainMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 2
            Console.WriteLine(PER2MENUOPTIONS);
            PageNum = 2;
        }
        public void DisplayPer3MainMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine(PER3MENUOPTIONS);
            PageNum = 3;
        }
        public void DisplayPer4MainMenu()
        {
            Console.Title = "Admin Main Menu";
            //This is the text to be displayed here, showing all available options
            Console.WriteLine(PER4MENUOPTIONS);
            PageNum = 4;
        }


        public void GotoInsertMenu(int userPerLevel)
        {
            optionsOnPage = 0;
            Console.Title = CREATEMENU_TITLE;
            ClearScreen();
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
                    DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        //Shows the user a list of tables to insert values into based on their employee permission level
        public void DisplayPer1InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 1
            Console.WriteLine(PER1MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"Log a transaction({optionsOnPage})"); optionsOnPage++;
            PageNum = 5;
        }
        public void DisplayPer2InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 2
            Console.WriteLine(PER2MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"Log a transaction({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Order a product({optionsOnPage})"); optionsOnPage++;
            PageNum = 6;
        }
        public void DisplayPer3InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine(PER3MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
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
            Console.WriteLine(PER4MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
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
            ClearScreen();
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
                    DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        public void DisplayPer2UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 2
            Console.WriteLine(PER2MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"Update Order Status({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee shift({optionsOnPage})"); optionsOnPage++;
            PageNum = 9;
        }
        public void DisplayPer3UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine(PER3MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"Update Order Status({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update employee shift({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update provider details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update store details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update job details({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update store hours({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Update product details({optionsOnPage})"); optionsOnPage++;
            PageNum = 10;
        }
        public void DisplayPer4UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 4
            Console.WriteLine(PER4MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
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
            ClearScreen();
            switch (userPerLevel)
            {
                
                case 3:
                    this.DisplayPer3DeleteMenu();
                    break;
                case 4:
                    this.DisplayPer4DeleteMenu();
                    break;
                default:
                    DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }
        public void DisplayPer3DeleteMenu()
        {
            //Add text to display here, specific to users with a per level of 3
            Console.WriteLine(PER3MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
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
            Console.WriteLine(PER4MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
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
            ClearScreen();
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
                    DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }


        public void DisplayPer1ReportMenu()
        {
            //The text to display here, specific to users with a per level of 1
            Console.WriteLine(PER1MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"All employee shifts({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All employee permission levels({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Employee Pay({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All Permission Levels({optionsOnPage})");optionsOnPage++;
            PageNum = 14;
        }
        public void DisplayPer2ReportMenu()
        {
            //The text to display here, specific to users with a per level of 2
            Console.WriteLine(PER1MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"All employee shifts({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All employee permission levels({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Employee Pay({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All product orders({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All product stocks({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All Permission Levels({optionsOnPage})"); optionsOnPage++;
            PageNum = 15;
        }
        public void DisplayPer3ReportMenu()
        {
            //The text to display, specific to users with a per level of 3
            Console.WriteLine(PER3MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"All product orders({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All employee shifts({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Sales by Product({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Sales by Store({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Best Products({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All employee permission levels({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Employee Pay({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All product stocks({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Product Providers({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All Store Hours({optionsOnPage})");optionsOnPage++;
            Console.WriteLine($"All Employees({optionsOnPage})");optionsOnPage++;
            Console.WriteLine($"All Permission Levels({optionsOnPage})"); optionsOnPage++;
            PageNum = 16;
        }
        public void DisplayPer4ReportMenu()
        {
            //The text to display, specific to users with a per level of 4(currently the same as for a PerLevel of 3)
            Console.WriteLine(PER4MENUOPTIONS);
            Console.WriteLine(OPTIONSMESSAGE);
            Console.WriteLine($"All product orders({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All employee shifts({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Sales by Product({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Sales by Store({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Best Products({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All employee permission levels({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Employee Pay({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All product stocks({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"Product Providers({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All Store Hours({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All Employees({optionsOnPage})"); optionsOnPage++;
            Console.WriteLine($"All Permission Levels({optionsOnPage})"); optionsOnPage++;
            PageNum = 17;
        }



        public void GotoHelp(int userPerLevel)
        {

            Console.Title = HELP_TITLE;
            ClearScreen();
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
                    DisplayMessage(INVALIDPERLEVELMESSAGE);
                    //Current handling of this error insufficient
                    break;
            }
            return;
        }

        public void DisplayPer1Help() 
        {
            Console.WriteLine(PER1MENUOPTIONS);
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            Console.WriteLine("Remember to press O before trying to select a numbered option");
            Console.WriteLine("Remember that unless stated otherwise, no numeric field accepts negative input.");
            PageNum = 18;
        }
        public void DisplayPer2Help()
        {
            Console.WriteLine(PER2MENUOPTIONS);
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            Console.WriteLine("Remember to press O before trying to select a numbered option");
            Console.WriteLine("Remember that unless stated otherwise, no numeric field accepts negative input.");
            PageNum = 19;
        }
        public void DisplayPer3Help()
        {
            Console.WriteLine(PER3MENUOPTIONS);
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            Console.WriteLine("Remember to press O before trying to select a numbered option");
            Console.WriteLine("Remember that unless stated otherwise, no numeric field accepts negative input.");
            PageNum = 20;
        }
        public void DisplayPer4Help()
        {
            Console.WriteLine(PER4MENUOPTIONS);
            Console.WriteLine("RULE ONE OF THIS SYSTEM: This system is unforgiving. Double-check all of your inputs and choices before entering.");
            Console.WriteLine("Remember to press O before trying to select a numbered option");
            Console.WriteLine("Remember that unless stated otherwise, no numeric field accepts negative input.");
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
            List<string[]> stringRecords=[];
            Type listType = typeof(T);
            PropertyInfo[] fieldInfo= listType.GetProperties();//Gets information about all the public properties of the specified type
           
            foreach (var record in recordList)
            {
                string[] stringRecord=new string[fieldInfo.Length];
                int i = 0;
                foreach (var field in fieldInfo)
                {
                   stringRecord[i]= field.GetValue(record).ToString();//Converts records to string representation
                    i++;
                }
                stringRecords.Add(stringRecord);
            }

            do
            {
               listIndex= DisplayRecordOptions(stringRecords, tableColumns);//Displays the records and returns the index of the record to modify. 
                idsToUpdate.Add((int)fieldInfo[0].GetValue(recordList[listIndex]));//Gets the database identifier of the record, using the specified listIndex and the fieldInfo previously specified for the type.
                string[] raworiginalRecord = stringRecords.Find(x => x[0].Equals(idsToUpdate.Last().ToString()));//Finds the stringRecord with the specified idToUpdate(guaranteed to be the last idToUpdate)
                string originalRecord =String.Join(" | ",raworiginalRecord);//Joins the stringRecord to be in the format: (field1) | (field2) | (field3) | ...(fieldn)
                Console.WriteLine("Original record: "+originalRecord);
                //here get column and new value
                Console.WriteLine("Please pick a field to modify:");
                string[] fieldNames = new string[fieldInfo.Length - 1];
                for (int i = 1; i < fieldInfo.Length; i++)//Gets the field names of the record being updated using fieldInfo. Note that the first(ID) column cannot be updated and therefore is not obtained or shown.
                {
                    fieldNames[i - 1] = fieldInfo[i].Name;
                }
                DisplayTableFieldOptions(fieldNames);//Displays the options for which column to update
                int optionInt= GetIntegerInput(optionsOnPage-1,0);//Gets the user input for which column to update
                colNamesToUpdate.Add(fieldNames[optionInt]);
                newValues.Add(GetTypeSpecificInputAsString(fieldInfo[optionInt+1]));//Gets user input specific to the type of the column.

                continueUpdating = GetYesNoInput("Select more records to update?[Y/N]");
            } while (continueUpdating);

            colNames = colNamesToUpdate;//Assigns out parameters and returns to main program.
            newVals = newValues;
            return idsToUpdate;
        }

        
        private string GetTypeSpecificInputAsString(PropertyInfo fieldInfo)
        {
            Dictionary<Type, int> typeTable = new Dictionary<Type, int> { { typeof(Int32), 0 }, {typeof(Int16),1 }, { typeof(String), 2 } ,{typeof(TimeOnly),3 },{ typeof(DateOnly),4},{typeof(decimal),5 },{typeof(DateTime),6 },{typeof(DayOfWeek),7 },{typeof(bool),8 },{typeof(byte),9 } };
            Type fieldtype = fieldInfo.GetType();
            switch (typeTable[fieldInfo.PropertyType])
            {
                case 0:
                    Console.WriteLine("Enter an integer greater than 0 and less than 2147483648");
                    return GetIntegerInput(Int32.MaxValue,1).ToString();//Int32
                case 1:
                    Console.WriteLine("Enter an integer greater than 0 and less than 32768");
                    return GetShortIntegerInput(Int16.MaxValue,1).ToString();//Int16
                case 2:
                    Console.WriteLine($"Enter text with a maximum length of {fieldInfo.GetCustomAttribute<StringLengthAttribute>().MaximumLength} characters");
                    return GetStringInput([], fieldInfo.GetCustomAttribute<StringLengthAttribute>().MaximumLength);//String
                case 3:
                    //Prompting handled within method
                    return GetTimeInput().ToString("hh:mm:ss");//Time
                case 4:
                    Console.WriteLine("Field Autofilled");
                    return DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");//Date(does not need to be updated due to being used only for record-keeping purposes)
                case 5:
                    Console.WriteLine("Please enter a decimal no larger than 99999. Any decimal places beyond two will be rounded.");
                    return GetDecimalInput(99999,2).ToString();//decimal
                case 6:
                    Console.WriteLine("Field Autofilled");
                    return DateTime.Now.ToString("O");//DateTime(should not be updated due to only being used in timestamps)
                case 7:
                    //Prompting handled within method
                    return GetDayOfWeekInput().ToString();//DayofWeek

                case 8:
                    bool boolInput= GetYesNoInput("Please enter a boolean value[Y=true,N=false]");
                    if (boolInput)
                    {
                        return "1";//boolean true
                    }
                    else
                    {
                        return "0";//boolean false
                    }
                case 9:
                    Console.WriteLine("Please enter a number no greater than 255");
                    return GetByteInput().ToString();
                default:
                    return "";
            }
        }
       


        //This method displays the page for record deletion(for records of type T), prompts the user for which records to delete, and returns a list of the corresponding Ids.
        public List<int> DisplayDeleteRecordsPage<T>(string[] tableColumns, List<T> recordList) where T:class
        {
            Console.Title="Delete Records";
            bool continueDeleting;
            List<int> idsToDelete= [] ;
            List<string[]> stringRecords = [];
            Type listType = typeof(T);//The type of recordList
            PropertyInfo[] fieldInfo = listType.GetProperties();//Gets the properties of the type that recordList is a list of.

            foreach (var record in recordList)
            {
                string[] stringRecord = new string[fieldInfo.Length];//Length of string array=number of fields
                int i = 0;
                foreach (var field in fieldInfo)
                {
                    stringRecord[i] = field.GetValue(record).ToString();//Converts records to string representation so they can be displayed
                    i++;
                }
                stringRecords.Add(stringRecord);
            }
            Console.WriteLine();//Adds a line of whitespace to the console
            do
            {
                int listIndex = DisplayRecordOptions(stringRecords, tableColumns);
                idsToDelete.Add((int)fieldInfo[0].GetValue(recordList[listIndex]));
                //this uses reflection to get the actual ID from the record using the list index.
                continueDeleting = GetYesNoInput("Select more records to delete?[Y/N]");
            } while (continueDeleting);

            return idsToDelete;  //This returns the IDs of the records to delete
        }


        //This method displays the page for and gets records to insert from the user, returning a List<object[]> of records to insert. Each object in an object[] represents a field in a record.
        //This method has no awareness of the actual fields of the database. It should be modified in a future update to allow this
        public List<object[]> DisplayInsertRecordsPage(string[] columnNames, byte[] columnDatatypes)
        {
            ClearScreen();
            Console.Title = "Insert Records";
            List<object[]> recordsToInsert = [];
            bool addMoreRecords;

            Console.WriteLine($"The columns are:");
            for (int j = 1; j < columnNames.Length; j++)//loops through and prints all of the non-Primary Key columns(PK column is always first)
            {
                Console.Write(columnNames[j]+"  ");
            }
            Console.WriteLine();
            do//Main loop for getting records to insert
            {
                object[] fields = new object[columnDatatypes.Length-1];//Creates an array representing a record
                int i = -1;
                foreach (var column in columnDatatypes)
                {

                    if (i==-1)//This skips the first (Primary Key) column, since the primary key is added automatically by the database.
                    {
                        goto skipInput;
                    }
                    switch (column)//columnDatatypes uses numbers to represent different datatypes.
                    {
                        case 0:
                            Console.WriteLine("Please enter an integer(Int32):");
                            fields[i] = GetIntegerInput(int.MaxValue,1);//Minvalue of one prevents the user from setting a foreign key balue as zero(invalid), but also prevents other int32 values such as the RmnStock column in tblStockLevel from being created with value of zero, which is not ideal.
                            break;
                        case 1:
                            Console.WriteLine("Please enter an integer(Int16):");
                            fields[i] = GetShortIntegerInput(Int16.MaxValue,0);
                            break;
                        case 2:
                            Console.WriteLine("Please enter a string:");
                            fields[i] = GetStringInput([], 75);//needs a way to get the max length and invalid characters. Address in future update.
                            break;
                        case 3:
                            //time
                            fields[i] = GetTimeInput();//Gets an error-corrected time value
                            break;
                        case 4:
                            //date
                            Console.WriteLine("Field autofilled");
                            fields[i] = DateOnly.FromDateTime(DateTime.Now);
                            break;
                        case 5:
                            //decimal
                            Console.WriteLine("Please enter a decimal smaller than 100000");
                            fields[i] = GetDecimalInput(99999, 2);//A better way to restrict this would be ideal
                            break;
                        case 6:
                            //Date+time
                            Console.WriteLine("Field autofilled");
                            fields[i] = DateTime.Now;//Autofills timestamp values with the current system date and time.
                            break;
                        case 7:
                            //Day of week
                            fields[i] =GetDayOfWeekInput();

                            break;
                        case 8:
                            //bool
                            //currently, this can be auto-assigned to false since the only bit/boolean value in the database defaults to zero.
                            Console.WriteLine("Field autofilled");
                            fields[i] = false;
                            break;
                        case 9:
                            Console.WriteLine("Please enter a number no greater than 255");
                            fields[i] =GetByteInput();
                            break;
                        default:
                            break;
                    }
                skipInput:
                    i++;
                }  
                recordsToInsert.Add(fields);
                addMoreRecords = GetYesNoInput("Do you want to add more records?[Y/N]");
            } while (addMoreRecords);

            return recordsToInsert;
        }


        public byte[] ConcealTypedField()
        {
            //This method obscures the password field when the employee logs in
            //Gets and stores each character pressed, replacing it with a * on the console.
            //Allows editing of password as it is being written.
            //Checked for matching string with .Equals outside this method
            List<char> password = [];//A list is used due to its variable length
            char buffer;
            Console.SetCursorPosition(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top + 1);//Moves the cursor down one row. This is done to put the cursor in the correct part of the login screen.
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
                            Console.Write('\b');//Moves the cursor one space to the left
                        }
                        break;
                    default:
                        password.Add(buffer);//This stores the character the user pressed
                        Console.Write("*");
                        break;
                }


            } while (!(buffer == '\r') || password.Count == 0);//When the user presses enter, their password will be verified(as long as the password is of non-zero length)

            byte[] bytepassword = new byte[password.Count];
            int i = 0;
            foreach (var item in bytepassword)
            {
                bytepassword[i] = Convert.ToByte(password[i]);
                i++;
                //The length-unfixed List<char> is converted to a byte[] to format it for hashing
            }
            return bytepassword;

        }


        //This method displays the fields in a user-chosen table, labelled with generic option numbers.
        public static void DisplayTableFieldOptions(string[] columnNames)
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
                //
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
                ClearScreen();
                for (int j = 0; j < columnHeadings.Length; j++)
                {
                    Console.Write(columnHeadings[j].PadRight(maxColumnLengths[j] + 1));//Prints and left-aligns the column headings.
                }
                Console.WriteLine("\n");//This block clears the console, prints the column names, and moves two lines down.

                try
                {
                    do
                    {
                        for (int j = 0; j < records[i].Length; j++)//Loops through all the fields in a record
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
                Console.WriteLine($"Page {outputPageNum} of {Math.Ceiling((double)records.Count / maxRecordsOnPage)}\t\t<-Backspace   Enter->  Exit Report:Esc");

                //Gets user input for what to do next.
            RetryInput:
                char userkey = Console.ReadKey(true).KeyChar;
                if (userkey == '\r' && (i < records.Count))//Cannot evaluate to true if the report is on the last page
                {
                    outputPageNum++;
                    exitReport = false;
                    //Goes to next page if Enter key is pressed
                }
                else if (userkey == '\b' && outputPageNum > 1)//Cannot evaluate to true if the report is on the first page
                {
                    i -= (recordsOnPage + maxRecordsOnPage);
                    outputPageNum--;
                    exitReport = false;
                    //Goes to previous page if Backspace is pressed
                }
                else if (userkey == '\e')//Exits the report if Esc is pressed
                {
                    exitReport = true;
                }
                else//This statement includes the cases where the user presses either Backspace or Enter, but the report is already on the minimum or maximum page respectively
                {
                    goto RetryInput;//Instead of reprinting the page, input is retried.
                }
            } while (!exitReport);

        }

        //This method gets the maximum column lengths from a List<string[]>. The column lengths are returned as a List<int>, and are used to align the columns when displayed to the console.
        public static List<int> GetMaxColumnLengths(List<string[]> table, string[] headings)
        {
            table.Add(headings);//Adds the column headings to the list.
            // Source - https://stackoverflow.com/a/32716864
            // Posted by Tim Schmelter, modified by community. See post 'Timeline' for change history
            // Retrieved 2026-08-27, License - CC BY-SA 3.0
            List<int> Result = Enumerable.Range(0, table.First().Length)
                .Select(i => table.Max(arr => arr[i].Length))
                .ToList();
            //selects and assigns to a List<int> the greatest string length of each column.
            table.RemoveAt(table.Count - 1);//Removes the column headings from the list, so they do not show up as a record.
            return Result;
        }


        //this method is used for both update and delete functions, displaying records in left-aligned columns. Pagination is supported. This method returns the list index of the record the user selects.
        public static int DisplayRecordOptions(List<string[]> records, string[]columnHeadings)
        {
            int i = 0;//This number represents the index of the next record to display.
            int outputPageNum = 1;//This number is used as the page number
            bool makeInput;
            int maxRecordsOnPage = 0;//Used to calculate operations involving pages
            List<int> maxColumnLengths = GetMaxColumnLengths(records, columnHeadings);//Gets the maximum width of columns and their headings

            //Main pagination loop
            do
            {
                //This block clears the console, prints the column names, and moves two lines down.
                ClearScreen();
                optionsOnPage = 0;//This is true, since the console has just been cleared.
                for (int j = 0; j < columnHeadings.Length; j++)
                {
                    Console.Write(columnHeadings[j].PadRight(maxColumnLengths[j] + 1));//Prints and left-aligns the column headings.
                }
                Console.WriteLine("\n");

                try
                {
                    do
                    {
                        for (int j = 0; j < records[i].Length; j++)//Cycles through every field in the record
                        {
                            Console.Write(records[i][j].PadRight(maxColumnLengths[j] + 1));
                            //Prints one field in the record, followed by a space. If the field is shorter than the corresponding column heading, the space to the right of the text will be padded with spaces to make up the difference.
                            //Note: This method of printing records does not currently allow using the foreign key connections
                        }
                        Console.WriteLine($"[{optionsOnPage}]");//Prints the number the user will use to select records and moves to the next line.
                        i++;
                        optionsOnPage++;
                    } while (Console.CursorTop < (Console.BufferHeight - 2) && i < records.Count);//Stops printing records one line above the bottom of the buffer.

                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine($"Error:{e.Message}");
                }
                if (outputPageNum == 1)
                {
                    maxRecordsOnPage = i;//Since the first page is printed, the maximum possible number of records for this table is now on screen
                }
                Console.WriteLine($"Page {outputPageNum} of {Math.Ceiling((double)records.Count / maxRecordsOnPage)}\t\t<-Backspace   Enter->  Chose record:O");
            RetryInput:
                char userkey = Console.ReadKey(true).KeyChar;//Gets a character from the user
                if (userkey == '\r' && (i < records.Count))//Enter increments the page if there are still more records to display
                {
                    outputPageNum++;
                    makeInput = false;
                }
                else if (userkey == '\b' && outputPageNum > 1)//Backspace decrements the page if the page is not the first page
                {
                    i -= (optionsOnPage + maxRecordsOnPage);
                    outputPageNum--;
                    makeInput = false;
                }
                else if (userkey == 'o' || userkey == 'O')//O allows the user to choose a record on the current page
                {
                    makeInput = true;
                }
                else//This case can occur when the user presses a page increment/decrement key but they are already at the maximum/minimum page number.
                {
                    goto RetryInput;//Instead of reprinting the page, input is retried.
                }
            } while (!makeInput);//End main pagination loop

            int userValue= (GetIntegerInput(optionsOnPage-1,0));//Gets the option for the record to update from the user
            return ((outputPageNum - 1) * maxRecordsOnPage) + userValue;//This returns the list index of the record the user selected
        }


        //This region contains currently-unused code for allowing the user to parameterise what reports return.
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
            //This block prints each column in the tables that the query uses, along with a number allowing the choice of that column.
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
