using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void DisplayMessage(String message)
        {
            Console.WriteLine(message);
        }
    }
}
