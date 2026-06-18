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


        public void DisplayLoginPage()
        {
            //Add text to display here
            PageNum = 1;
            return;
        }


        //DisplayLogoutConfirmation returns a boolean value indicating whether the user has chosen to log out
        public bool DisplayLogoutConfirmation()
        {
            //Add text to display here

            return true;//temp value
            //The PageNum value is not changed to allow the program to maintain user state if the user stays logged in.
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

        public void DisplayMainMenu()
        {
            //Add menu text to display here

        }

        //Shows the user a list of tables to insert values into based on their employee permission level
        public void DisplayInsertMenu(string menutext)
        {
            //Display menutext here
        }

        
        public void ConcealTypedField()
        {
            //This method will obscure the password field when the employee logs in
            //Will get each character, replace it with a *, allow mid-string replacement of characters

        }

    }
}
