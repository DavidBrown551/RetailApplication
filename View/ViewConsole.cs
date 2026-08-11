using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private const string BADINTMESSAGE = "You must input a number-only reply with a maximum size of 2,147,483,648.";
        private const string INVALIDPERLEVELMESSAGE = "Error: the user does not have a valid permission level";
        private const string READONLYFIELDMESSAGE = "Error: This field is read-only. Please try updating a different field.";
        private const string INVALIDCHARINPUTMESSAGE = "Error: That is not a character recognised by the system. Please try again.";


        //The following are the page titles
        private const string HELP_TITLE = "Help Page";
        private const string MAINMENU_TITLE = "Main Menu";
        private const string CREATEMENU_TITLE = "Add Records";
        private const string UPDATEMENU_TITLE = "Update Records";
        private const string REPORTMENU_TITLE = "Report Menu";

        //The arrays of valid characters used by the program to check user input.
        #region validCharArrays

        //This is the set of generic characters which are used for multi-choice options, e.g. fields in a table. Please Note: The length of this array is the maximum number of options the user can be presented with. The current length is 21 characters.
        static char[] genericOptionChars = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'q', 'w', 'e', 't', 'y', 'i', 'o', 'p', 'a', 's', 'f'];


        //The current length of the following array is 23 pages
        //This is the array of all the valid characters for each page in the application. The array reference of the valid characters for a page is equal to its PageNum value.
        static char[][] AllValidChars =
            [
            ['a'],//PageNum=0,Page=login
            ['c','C','h','H','r','R','l','L','\e'],//PageNum=1, Page=Per1MainMenu
            ['c','C','h','H','r','R','l','L','\e','u','U'],//PageNum=2, Page=Per2MainMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','d','D'],//PageNum=3, Page=Per3MainMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','d','D'],//PageNum=4, Page=Per4MainMenu
            ['m','M','h','H','r','R','l','L','\e'],//PageNum=5, Page=Per1InsertMenu
            ['m','M','h','H','r','R','l','L','\e','u','U'],//PageNum=6, Page=Per2InsertMenu
            ['m','M','h','H','r','R','l','L','\e','u','U','d','D'],//PageNum=7, Page=Per3InsertMenu
            ['m','M','h','H','r','R','l','L','\e','u','U','d','D'],//PageNum=8, Page=Per4InsertMenu
            ['c','C','h','H','r','R','l','L','\e','m','M'],//PageNum=9, Page=Per2UpdateMenu
            ['c','C','h','H','r','R','l','L','\e','m','M','d','D'],//PageNum=10, Page=Per3UpdateMenu
            ['c','C','h','H','r','R','l','L','\e','m','M','d','D'],//PageNum=11, Page=Per4UpdateMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','m','M'],//PageNum=12, Page=Per3DeleteMenu
            ['c','C','h','H','r','R','l','L','\e','u','U','m','M'],//PageNum=13, Page=Per4DeleteMenu
            ['c','C','h','H','m','M','l','L','\e'],//PageNum=14, Page=Per1ReportMenu
            ['c','C','h','H','m','M','l','L','\e','u','U'],//PageNum=15, Page=Per2ReportMenu
            ['c','C','h','H','m','M','l','L','\e','u','U','d','D'],//PageNum=16, Page=Per3ReportMenu
            ['c','C','h','H','m','M','l','L','\e','u','U','d','D'],//PageNum=17, Page=Per4ReportMenu
            ['c','C','m','M','r','R','l','L','\e'],//PageNum=18, Page=Per1Help
            ['c','C','m','M','r','R','l','L','\e','u','U'],//PageNum=19, Page=Per2Help
            ['c','C','m','M','r','R','l','L','\e','u','U','d','D'],//PageNum=20, Page=Per3Help
            ['c','C','m','M','r','R','l','L','\e','u','U','d','D'],//PageNum=21, Page=Per4Help
            [],//PageNum=22, Page=DisplayReport
            ['\e','l','L'],//PageNum=23, Page=Update/Insert/Delete page
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


        //This method is implemented here instead of using Console.WriteLine in Program.cs to comply with MVC architecture
        public void DisplayMessage(String message)
        {
            Console.WriteLine(message);
        }


        public char GetCharacterInput()
        {
            
            return Console.ReadKey(true).KeyChar;
        }


        //This method gets and returns a (error-corrected) integer
        public int GetIntegerInput()
        {
            bool exitLoop=false;
            int returnValue=0;
            do
            {
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out returnValue))//If the input string fails to parse(due to containing incorrect characters), this will be caught and a error message will be output.
                {
                    Console.WriteLine(BADINTMESSAGE);
                }
                else
                {
                    exitLoop = true;
                }
            } while (!exitLoop);
            
            return returnValue;
        }


        public int GetUserPageChangeValue()
        {
            char? a;
            int outInt=0;
            while (!HandleCharInput(out a))
            {
                Console.WriteLine(INVALIDCHARINPUTMESSAGE);
            };

            switch (a)
            {
                case 'm'://M=Main menu
                case 'M':
                    outInt = 21;
                    break;
                case 'c'://C=create records
                case 'C':
                    outInt = 22;//numbers need to be defined that correspond to destinations e.g. 2 for main menu
                    break;
                case 'H'://H=Help
                case 'h':
                    outInt = 23;
                    break;
                case 'u'://U=update menu
                case 'U':
                    outInt = 24;
                    break;
                case 'd'://D=delete menu
                case 'D':
                    outInt = 25;
                    break;
                case 'l'://L=Logout
                case'L':
                    outInt = 26;
                    break;
                case '\e':
                    outInt = 27;
                break;
                case 'r':
                case 'R':
                    outInt = 28;
                    break;

                default:
                    for (int i = 0; i < genericOptionChars.Length; i++)
                    {
                        if (genericOptionChars[i]==a)
                        {
                            outInt = i;
                            break;
                        }
                    }
                    break;
            }
            

            return outInt;
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

        public void DecipherGenericMenuOption(int userOption)
        {
           //Use PageNum to figure out which array to search for the option selected
           //Then send the string and any relevant values to a method to display etc.
           //Generic options lead to generic display methods





        }





        //The following region contains the methods used to display the various menus of the application.
        #region DisplayMenuMethods
        public void GotoMainMenu(int userPerLevel)
        {
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
            Console.WriteLine($"Log a transaction({genericOptionChars[0]})");
            PageNum = 5;
        }
        public void DisplayPer2InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 2
            Console.WriteLine($"Log a transaction({genericOptionChars[0]})");
            Console.WriteLine($"Order a product({genericOptionChars[1]})");
            PageNum = 6;
        }
        public void DisplayPer3InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine($"Log a transaction({genericOptionChars[0]})");
            Console.WriteLine($"Order a product({genericOptionChars[1]})");
            Console.WriteLine($"Add an employee({genericOptionChars[2]})");
            Console.WriteLine($"Add shifts for an employee({genericOptionChars[3]})");
            Console.WriteLine($"Add hours for a store({genericOptionChars[4]})");
            Console.WriteLine($"Add a product({genericOptionChars[5]})");
            Console.WriteLine($"Add a provider({genericOptionChars[6]})");
            Console.WriteLine($"Add a new store({genericOptionChars[7]})");
            Console.WriteLine($"Add a new job({genericOptionChars[8]})");
            PageNum = 7;
        }
        public void DisplayPer4InsertMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 4
            Console.WriteLine($"Log a transaction({genericOptionChars[0]})");
            Console.WriteLine($"Order a product({genericOptionChars[1]})");
            Console.WriteLine($"Add an employee({genericOptionChars[2]})");
            Console.WriteLine($"Add shifts for an employee({genericOptionChars[3]})");
            Console.WriteLine($"Add hours for a store({genericOptionChars[4]})");
            Console.WriteLine($"Add a product({genericOptionChars[5]})");
            Console.WriteLine($"Add a provider({genericOptionChars[6]})");
            Console.WriteLine($"Add a new store({genericOptionChars[7]})");
            Console.WriteLine($"Add a new permission level({genericOptionChars[8]})");
            Console.WriteLine($"Add a new job({genericOptionChars[9]})");
            PageNum = 8;
        }


        public void GotoUpdateMenu(int userPerLevel)
        {

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
            Console.WriteLine($"Update Order Status({genericOptionChars[0]})");
            Console.WriteLine($"Update employee details({genericOptionChars[1]})");
            Console.WriteLine($"Update employee shift({genericOptionChars[2]})");
            PageNum = 9;
        }
        public void DisplayPer3UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 3
            Console.WriteLine($"Update Order Status({genericOptionChars[0]})");
            Console.WriteLine($"Update employee details({genericOptionChars[1]})");
            Console.WriteLine($"Update employee shift({genericOptionChars[2]})");
            Console.WriteLine($"Update provider contact details({genericOptionChars[3]})");
            Console.WriteLine($"Update store details({genericOptionChars[4]})");
            Console.WriteLine($"Update job details({genericOptionChars[5]})");
            Console.WriteLine($"Update store hours({genericOptionChars[6]})");
            Console.WriteLine($"Update product details({genericOptionChars[7]})");
            PageNum = 10;
        }
        public void DisplayPer4UpdateMenu()
        {
            //This is the text to be displayed here, specific to users with a per level of 4
            Console.WriteLine($"Update Order Status({genericOptionChars[0]})");
            Console.WriteLine($"Update employee details({genericOptionChars[1]})");
            Console.WriteLine($"Update employee shifts({genericOptionChars[2]})");
            Console.WriteLine($"Update provider details({genericOptionChars[3]})");
            Console.WriteLine($"Update store details({genericOptionChars[4]})");
            Console.WriteLine($"Update job details({genericOptionChars[5]})");
            Console.WriteLine($"Update permission levels({genericOptionChars[6]})");
            Console.WriteLine($"Update transaction({genericOptionChars[7]})");
            Console.WriteLine($"Update store hours({genericOptionChars[8]})");
            Console.WriteLine($"Update store stocks({genericOptionChars[9]})");
            Console.WriteLine($"Update product details({genericOptionChars[10]})");
            PageNum = 11;
        }


        public void GotoDeleteMenu(int userPerLevel)
        {

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
            Console.WriteLine($"Note: some records may not be deleteable due to being a dependency for other records");
            PageNum = 12;
        }
        public void DisplayPer4DeleteMenu()
        {
            //Add text to display here, specific to users with a per level of 4
            Console.WriteLine($"Note: some records may not be deleteable due to being a dependency for other records");
            PageNum = 13;
        }



        public void GotoReportMenu(int userPerLevel)
        {
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
            //Add text to display here, specific to users with a per level of 1. This text should help explain how the website works and what they can and cannot do.
            PageNum = 18;
        }
        public void DisplayPer2Help()
        {
            //Add text to display here, specific to users with a per level of 2. This text should help explain how the website works and what they can and cannot do.
            PageNum = 19;
        }
        public void DisplayPer3Help()
        {
            //Add text to display here, specific to users with a per level of 3. This text should help explain how the website works and what they can and cannot do.
            PageNum = 20;
        }
        public void DisplayPer4Help()
        {
            //Add text to display here, specific to users with a per level of 4. This text should help explain how the website works and what they can and cannot do.
            PageNum = 21;
        }

        #endregion


        public byte[] ConcealTypedField()
        {
            //This method will obscure the password field when the employee logs in
            //Will get and store each character, replace it with a *, allow mid-string replacement of characters(FIRST PART)
            //Checked for matching string with .Equals outside this method
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

            try
            {
                int charOptionNumber = 0;
                foreach (var column in columnNames)
                {
                    Console.WriteLine(column + ": " + genericOptionChars[charOptionNumber]);
                    charOptionNumber++;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(TOOMANYOPTIONSMESSAGE);
                //This exception will occur if there are more columns in the table than generic option characters.
                throw;
            }

            
        }






        public void DisplayReport(string[] columnHeadings, string[] records)
        {
            PageNum = 22;
            //Try avoid using windows-specific functions to help pagination
            //If impossible to do this ensure it is mentioned in documentation
            //(mostly) FIXED:core functions like Console.ReadKey are unsupported on several non-windows architechtures
            int i = 0;
            int outputPageNum = 1;
            bool exitReport;
            do
            {
                Console.Clear();
                foreach (var heading in columnHeadings)
                {
                    Console.Write(heading+"\t");
                }
                Console.WriteLine("\n");//Clears the console, prints the column names, and moves two lines down.
                
                try
                {
                    do
                    {
                        Console.WriteLine(records[i]);
                        i++;
                    } while (Console.CursorTop < (Console.BufferHeight - 2)&&i<records.Length);//Stops printing records one line above the bottom of the buffer.

                }
                catch (IndexOutOfRangeException)
                {

                    throw;
                }
                
                Console.WriteLine("Page:"+outputPageNum);
                char userkey= Console.ReadKey().KeyChar;
                if (userkey=='\n')
                {
                    outputPageNum++;
                    exitReport = false;
                }
                else if (userkey=='\b')
                {
                    i -= (i + 1) / outputPageNum;//Reduces the index by the amount of records in a page
                    outputPageNum--;
                    exitReport = false;
                }
                else
                {
                    exitReport = true;
                }
            } while (exitReport);
            
        }


        public bool AskIfParametersWanted()
        {
            Console.WriteLine("Do you want to filter what the report returns?[Y/N]");
            do
            {

                char buffer = Console.ReadKey(true).KeyChar;
                if (buffer=='y'||buffer=='Y')
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

       public string AskParameters(int querynum, byte[][]queryTables, string[][] columnNames, byte[][]columnDatatypes)
       {
            Console.Title = "Parameterisation";
            //Only users with a Permission Level of 3 or 4 will be using this menu. Users with a lower Permission Level will have any parameterisation they require done automatically.

            char userKey;
            int charOptionNumber=0;
            string finalParameters;
            //This block prints each column in the tables that the query uses, along with a character allowing the choice of that column.
            try
            {
                foreach (var table in queryTables[querynum])
                {
                    DisplayTableFieldOptions(columnNames[table]);
                    //Try using DisplayTableFieldOptions here.

                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(TOOMANYOPTIONSMESSAGE);
                throw;
            }
            
            
            userKey=Console.ReadKey(true).KeyChar;

            //Now: 
            //1. determine which column the user wants to parameterise using a switch statement with array references.
            //2. Get the position of the column in the tableColumnNames array and input the same indexes to the tableColumnDatatypes array. This will return an integer corresponding to the datatype of the column. Use this integer as the variable for a switch case pointing to blocks of code that will allow datatype-specific parameterisation.

            //Datatypes:strings,integers, booleans, date/time
            //Aggregate columns: sum, average, etc
            //Are generic options always stored in arrays? if so, generic option input can be simplified to an array reference(YES)
            //How to find which array to reference?

            





            finalParameters = "Placeholder";
            return finalParameters;
       }

        //This method handles assigning parameters for all fields. 
        public string GetParameters<T>()
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
            string[] predicateBeginnings = ["@col_name LIKE \"", "@col_name LIKE \"%", "@col_name LIKE \"%","@col_name LIKE \""];
            string[] predicateEndings = ["%\"","\"","%\"","\""];

            //Display heading
            //Display options
            Console.WriteLine("");

            //Get input for which parameter

            do
            {
                string rawParameter = Console.ReadLine();
                if (!string.IsNullOrEmpty(rawParameter))
                {
                    return"placeholder";//
                }
            } while (true);
            

        }


    }
}
