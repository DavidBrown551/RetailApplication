using Microsoft.Data.SqlClient;
using RetailApplication.Controller;
using RetailApplication.Model;
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
            //This string allows connection to the database and must be changed if the file name or file path is changed.

            mainview = new ViewConsole();
            connection = new StorageManager(connectionString);
            hash = MD5.Create();

            //The queries used by the program. These will be passed to the ReportHandler method in the Controller folder and used to query the database. The '@' is a universal entry point for parameters to be added, and is replaced by whitespace if the query is not parameterised.
            const string qryProductOrder = "SELECT P.ProductName AS Product, S.StoreName AS Store, O.Quantity, P.SupplyPrice*O.Quantity AS Price FROM tblProduct P, tblStore S, tblProductOrder O WHERE P.ProductID=O.ProductID AND S.StoreID=O.StoreID AND DATEDIFF(dy,GETDATE(),O.Date)<30 @ ORDER BY O.Date;";
            const string qryEmployeeShifts = "SELECT (E.FirstName+E.LastName) AS Name, S.StoreName AS Store, I.StartTime AS Start Time, I.EndTime AS End Time, I.Weekday AS Day, J.JobName AS Job FROM tblEmployee E, tblStore S, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND S.StoreID=I.StoreID AND J.JobID=I.JobID @ ORDER BY 2,1,6,5,3,4;";
            const string qryProductSales = "SELECT COALESCE(P.ProductName,Total)AS Product, COALESCE(S.StoreName, Subtotal) AS Store, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblProduct P, tblTransaction T RIGHT JOIN tblStore S ON T.StoreID=S.StoreID WHERE P.ProductID=T.ProductID @ GROUP BY ROLLUP(S.StoreName,P.ProductName) ORDER BY 1,2;";
            const string qryStoreSales = "SELECT COALESCE(S.StoreName,Grand Total)AS Store, COALESCE(P.ProductName, Total Sales) AS Product, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblStore S, tblTransaction T RIGHT JOIN tblProduct P ON T.ProductID=P.ProductID WHERE S.StoreID=T.StoreID @ GROUP BY ROLLUP(P.ProductName,S.StoreName) ORDER BY 1,2;";
            const string qryBestProducts = "SELECT TOP 50 P.ProductName AS Product, SUM(T.Quantity) AS Total Sales, SUM(T.Quantity*P.CustomerPrice) AS Total Revenue, SUM((T.Quantity*P.CustomerPrice)-(T.Quantity*P.SupplyPrice)) AS Profit FROM tblProduct P, tblStore S, tblTransaction T WHERE S.StoreID=T.StoreID AND P.ProductID=T.ProductID @ GROUP BY P.ProductID ORDER BY 2 desc, 4 desc;";
            const string qryEmployeePermissions = "SELECT E.EmpID AS Employee Number, E.FirstName+ ' '+ E.LastName AS Name, l.PermissionName AS Permission Level FROM tblEmployee E, tblPermissionLevel l WHERE E.LevelID=l.LevelID @ ORDER BY l.PermissionLevel desc, 2;";
            const string qryEmployeePay = "SELECT COALESCE(E.FirstName + ' ' + E.LastName, Total) AS Employee Name, SUM(DATEDIFF(hh, I.EndTime, I.StartTime)*J.Pay) FROM tblEmployee E, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND J.JobID=I.JobID @ GROUP BY ROLLUP(E.EmpID) ORDER BY 1;";
            const string qryProductStocks = "SELECT P.ProductName AS Product, S.StoreName AS Store, L.RmnStock AS Quantity FROM tblProduct P, tblStore S, tblStockLevel L WHERE P.ProductID=L.ProductID AND S.StoreID=L.StoreID @ ORDER BY 1,3 desc;";

            const string qryGetRecords = "SELECT * FROM @;";//The @ in this query is replaced with the name of the table that the user needs records from. This could cause problems with larger tables, particularly with a bigger company due to the length of the read operation.




            byte userPermissionLevel; //This value is used to determine which pages the user can access

            string[][] tableColumnNames=
                [
                ["StoreID","StoreName","Address","Rent"],
                ["LevelID","LevelName","PermissionLevel"],
                ["EmpID","FirstName","LastName","LevelID"],
                ["ProviderID","ProviderName","Contact"],
                ["ProductID","ProviderID","ProductName","CustomerPrice","SupplyPrice","Amount"],
                ["StockID","StoreID","ProductID","RmnStock"],
                ["TransactionID","StoreID","ProductID","Timestamp","Quantity"],
                [],
                ];
            byte[][] tableColumnDatatypes;
            byte[][] tablesInQueries;//Populate these later

            //After establishing a connection to the database, the login page is shown
        Login:

            mainview.DisplayLoginPage();
            

            int enteredEmpId=mainview.GetIntegerInput();
            //Gets an error-corrected integer from the user representing their employee number

            string hashedUserPassword = Convert.ToHexStringLower(hash.ComputeHash(mainview.ConcealTypedField()));

            if (hashedUserPassword.Equals(connection.GetUserPassword(enteredEmpId)))
            {
                userPermissionLevel = connection.GetUserPermission(enteredEmpId);
                goto Main_Menu;
            }
            else
            {
                //Send user back to password screen with error message
                goto Login;
            }

        Main_Menu:
            mainview.GotoMainMenu(userPermissionLevel);//Displays the main menu for the user
            goto HandleCharInput;

        HandleCharInput:

            int userPageChangeValue= mainview.GetUserPageChangeValue();
            switch (userPageChangeValue)
            {
                case <=20://A menu option. Further seperation of values necessary.
                    //Send control to a method for determining what this generic option points to
                    break;
                case 21://Main Menu
                    mainview.GotoMainMenu(userPermissionLevel);
                    break;
                case 22://Create Menu
                    mainview.GotoInsertMenu(userPermissionLevel);
                    break;
                case 23:

                    break;
                    //Other cases here
                default:
                    break;
            }


            //The following will be a block to get all the records from different tables in the database.
            switch (some_intvalue)
            {
                case 1://tblEmployees
                    List<object[]> rawEmployees= connection.GetRecordsHandler("scheduling.tblEmployee",qryGetRecords);
                    foreach (var rawEmployee in rawEmployees)
                    {
                        new Employee((int)rawEmployee[0], (string)rawEmployee[1], (string)rawEmployee[2], (short)rawEmployee[3], (string)rawEmployee[4]);
                        //IMPORTANT: This is computationally expensive due to the large amount of boxing/unboxing required. Test this section for slowdowns.
                    }
                    break;
                case 2:

                    break;
                case 3:

                    break;
                default:
                    break;
            }



        }
    }
}
