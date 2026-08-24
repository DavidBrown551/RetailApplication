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
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=RetailDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
            //This string allows connection to the database and must be changed if the file name or file path is changed.

            mainview = new ViewConsole();
            connection = new StorageManager(connectionString);
            hash = MD5.Create();
            //The MD5 class does not have a constructor

            //The queries used to get reports from the database
            #region Queries
            //These will be passed to the ReportHandler method in the Controller folder and used to query the database. The '@' is a universal entry point for parameters to be added, and is replaced by whitespace if the query is not parameterised.
            const string qryProductOrder = "SELECT P.ProductName AS Product, S.StoreName AS Store, O.Quantity, P.SupplyPrice*O.Quantity AS Price FROM tblProduct P, tblStore S, tblProductOrder O WHERE P.ProductID=O.ProductID AND S.StoreID=O.StoreID AND DATEDIFF(dy,GETDATE(),O.Date)<30 @ ORDER BY O.Date;";
            const string qryEmployeeShifts = "SELECT (E.FirstName+E.LastName) AS Name, S.StoreName AS Store, I.StartTime AS Start Time, I.EndTime AS End Time, I.Weekday AS Day, J.JobName AS Job FROM tblEmployee E, tblStore S, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND S.StoreID=I.StoreID AND J.JobID=I.JobID @ ORDER BY 2,1,6,5,3,4;";
            const string qryProductSales = "SELECT COALESCE(P.ProductName,Total)AS Product, COALESCE(S.StoreName, Subtotal) AS Store, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblProduct P, tblTransaction T RIGHT JOIN tblStore S ON T.StoreID=S.StoreID WHERE P.ProductID=T.ProductID @ GROUP BY ROLLUP(S.StoreName,P.ProductName) ORDER BY 1,2;";
            const string qryStoreSales = "SELECT COALESCE(S.StoreName,Grand Total)AS Store, COALESCE(P.ProductName, Total Sales) AS Product, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblStore S, tblTransaction T RIGHT JOIN tblProduct P ON T.ProductID=P.ProductID WHERE S.StoreID=T.StoreID @ GROUP BY ROLLUP(P.ProductName,S.StoreName) ORDER BY 1,2;";
            const string qryBestProducts = "SELECT TOP 50 P.ProductName AS Product, SUM(T.Quantity) AS Total Sales, SUM(T.Quantity*P.CustomerPrice) AS Total Revenue, SUM((T.Quantity*P.CustomerPrice)-(T.Quantity*P.SupplyPrice)) AS Profit FROM tblProduct P, tblStore S, tblTransaction T WHERE S.StoreID=T.StoreID AND P.ProductID=T.ProductID @ GROUP BY P.ProductID ORDER BY 2 desc, 4 desc;";
            const string qryEmployeePermissions = "SELECT E.EmpID AS Employee Number, E.FirstName+ ' '+ E.LastName AS Name, l.PermissionName AS Permission Level FROM tblEmployee E, tblPermissionLevel l WHERE E.LevelID=l.LevelID @ ORDER BY l.PermissionLevel desc, 2;";
            const string qryEmployeePay = "SELECT COALESCE(E.FirstName + ' ' + E.LastName, Total) AS Employee Name, SUM(DATEDIFF(hh, I.EndTime, I.StartTime)*J.Pay) FROM tblEmployee E, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND J.JobID=I.JobID @ GROUP BY ROLLUP(E.EmpID) ORDER BY 1;";
            const string qryProductStocks = "SELECT P.ProductName AS Product, S.StoreName AS Store, L.RmnStock AS Quantity FROM tblProduct P, tblStore S, tblStockLevel L WHERE P.ProductID=L.ProductID AND S.StoreID=L.StoreID @ ORDER BY 1,3 desc;";
            const string qryAutoUpdateStocks = "UPDATE sales.tblStockLevel SET ";

            #endregion



            byte userPermissionLevel; //This value is used to determine which pages the user can access


            //The following region contains the arrays that store the data used by the program(coulumn names, datatypes, etc.)
            #region lookupArrays

            //This array stores the names of the tables
            string[] tables = 
                [
                "tblStore",
                "tblPermissionLevel",
                "tblEmployee",
                "tblProvider",
                "tblProduct",
                "tblStockLevel",
                "tblTransaction",
                "tblJob",
                "tblStoreHour",
                "tblEmployeeShift",
                "tblProductOrder"
                ];

            //This array stores the column names of each table. Note that currently these are used in both user-facing display and query parameterisation.
            string[][] tableColumnNames=
                [
                ["StoreID","StoreName","Address","Rent"],//tblStore
                ["LevelID","LevelName","PermissionLevel"],//tblPermissionLevel
                ["EmpID","FirstName","LastName","LevelID"],//tblEmployee
                ["ProviderID","ProviderName","Contact"],//tblProvider
                ["ProductID","ProviderID","ProductName","CustomerPrice","SupplyPrice","Amount"],//tblProduct
                ["StockID","StoreID","ProductID","RmnStock"],//tblStockLevel
                ["TransactionID","StoreID","ProductID","Timestamp","Quantity"],//tblTransaction
                ["JobID","JobName","Pay"],//tblJob
                ["StrHrID","StoreID","Weekday","OpenTime","CloseTime"],//tblStoreHour
                ["ShiftID","EmpID","StoreID","WeekDay","StartTime","EndTime","JobID"],//tblEmployeeShift
                ["OrderID","StoreID","ProductID","Quantity","Date","Filled"]//tblProductOrder
                ];

            //1=int32
            //2=int16
            //3=string
            //4=Time
            //5=Date
            //6=decimal
            //7=Date+Time
            //8=day of week
            //9=bool

            //This array stores a numerical representation of the datatypes in each table column
            byte[][] tableColumnDatatypes=
                [
                [1,3,3,6],//tblStore
                [1,3,1],//tblPermissionLevel
                [1,3,3,2],//tblEmployee
                [1,3,3],//tblProvider
                [1,1,3,6,6,3],//tblProduct
                [1,1,1,1],//tblStockLevel
                [1,1,1,7,1],//tblTransaction
                [1,3,6],//tblJob
                [1,1,3,4,4],//tblStoreHour
                [1,1,1,8,4,4,1],//tblEmployeeShift
                [1,1,1,1,5,9]//tblProductOrder
                ];

            //This array stores the array references to the tables used in the associated query
            byte[][] tablesInQueries=
                [
                [0,4,6],//qryProductSales
                [0,2,9],//qryEmployeeShifts
                [0,8],//qryStoreHour
                [2,7,9],//qryEmployeePay
                [0,4,5],//qryProductStocks
                [0,4,6],//qryBestProducts
                [0,4,10],//qryProductOrders
                [1,2],//qryEmployeePermissions
                [2],//qryEmployee
                [1],//qryPermissions
                [3,4],//qryProductProviders
                [0,4,6],//qryStoreSales
                [],//qry
                ];//could remove qry from beginning of string when displaying

            #endregion

            //The following lists are used to store the records from the corresponding table.
            #region tableLists
            List<Employee> employees = new List<Employee> { };
            List<PermissionLevel> perLevels = new List<PermissionLevel> { };
            List<Store> stores = new List<Store> { };
            List<StoreHour> storeHours = new List<StoreHour> { };
            List<Job> jobs = new List<Job> { };
            List<Provider> providers = new List<Provider> { };
            List<ProductOrder> productOrders = new List<ProductOrder> { };
            List<Product> products = new List<Product> { };
            List<EmployeeShift> employeeShifts = new List<EmployeeShift> { };
            List<Transaction> transactions = new List<Transaction> { };
            List<StockLevel> stockLevels = new List<StockLevel> { };
            #endregion


            //After establishing a connection to the database, the login page is shown
        Login:

            mainview.DisplayLoginPage();
            

            int enteredEmpId=mainview.GetIntegerInput();
            //Gets an error-corrected integer from the user representing their employsee number
            
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

            int userPageChangeValue= mainview.GetUserPageChangeValue(out int genericValue);
            switch (userPageChangeValue)
            {
                case 0://Main Menu
                    mainview.GotoMainMenu(userPermissionLevel);
                    break;
                case 1://Create Menu
                    mainview.GotoInsertMenu(userPermissionLevel);
                    break;
                case 2://Help Page
                    mainview.GotoHelp(userPermissionLevel);
                    break;
                case 3://Update Menu
                    mainview.GotoUpdateMenu(userPermissionLevel);
                    break;
                case 4://Delete Menu
                    mainview.GotoDeleteMenu(userPermissionLevel);
                    break;
                case 5://Logout
                    mainview.DisplayLogoutConfirmation();
                    break;
                case 6://Exit
                    mainview.DisplayExitConfirmation();
                    break;
                case 7://Report Menu
                    mainview.GotoReportMenu(userPermissionLevel);
                    break;
                case 8://A menu option. Further seperation of values necessary.
                    //Send control to a method for determining what this generic option points to
                    switch (mainview.PageNum)
                    {
                        case < 8:
                            //points to tableColumnNames
                            //pass tableColumnNames[userPageChangeValue] to display method
                            //insert pages
                            mainview.DisplayInsertRecordsPage(tableColumnNames[genericValue], tableColumnDatatypes[genericValue]);
                            //method to get records to insert; assign to a List<var> or similar

                            //pass list from previous to insert method
                            break;
                        case < 12:
                            //points to tableColumnNames
                            //pass tableColumnNames[userPageChangeValue] to display method
                            //update pages
                            break;
                        case < 14:
                            //points to tableColumnNames
                            //pass tableColumnNames[userPageChangeValue] to display method
                            //delete pages.
                            break;
                        case < 18:
                            //points to tablesInQueries
                            //pass tablesInQueries[userPageChangeValue] to method
                            //within method, foreach on that array to find potentially multiple arrays of column names, assigning them to a list and printing the list.
                            //covers reports
                            break;

                        default:
                            break;
                    }
                    if (true)//PageNum values not yet defined
                    {
                        //points to lists with records-needs to have a way to get the list to index
                        //covers records

                    }
                    else if (true)//PageNum values not yet defined
                    {
                        //points to parameter pages(pages for query parameterisation, dependent on column datatype)
                    }

                    break;
                default:
                    break;
            }
            //where to from here? use gotos? methods?
            //either page has been printed and is ready for user input or a generic option needs to be deciphered.
            //decipher generic options inside same switch as 





            //The following block will determine the page/table/record/etc the generic option points to. It will use the userPageChangeValue and PageNum to find where to find the relevant data for the following generic method.
            


    



            //The following will be a block to get all the records from different tables in the database.
            //The results may have dependencies in other tables(Foreign Keys). These dependencies will need to be retrieved through the same code block.
            int getTableRecordsValue=0;

            #region getRecordStrings
            switch (getTableRecordsValue)
            {
                case 1://tblEmployee

                    //Gets the Employees and Permission Levels from the database if they have not been gotten already
                    if (employees.Count == 0)
                    {
                        employees = connection.GetAllEmployees();
                    }
                    if (perLevels.Count==0)
                    {
                        perLevels = connection.GetPermissionLevels();//PermissionLevels are a dependency of Employees since PerLevelID is a foreign key in the scheduling.tblEmployee table.
                    }
                    //Format the strings and pass them to a TableRecordDisplayer method.
                    foreach (var employee in employees)
                    {
                        
                       string employeeString= employee.ToDisplayString(out int perLevelId);
                        string perLevelString = perLevels.Find(x=> x.PerLevelID== perLevelId).LevelName;//Finds the record with the specified PerLevelID and assigns the name of that permissionlevel to a string.
                        employeeString.Replace("@perlevel",perLevelString);//Assigns the name of the perlevel to the employee string.
                    }
                    
                    //Strings formatted, now create and pass a list of strings to be displayed.
                    
                    break;
                case 2://tblPermissionLevel
                    if (perLevels.Count==0)
                    {
                        perLevels = connection.GetPermissionLevels();
                    }

                    break;
                case 3://tblStore
                    if (stores.Count==0)
                    {
                        stores = connection.GetAllStores();
                    }
                    break;

                case 4://tblProvider
                    if (providers.Count==0)
                    {
                        providers = connection.GetAllProviders();
                    }
                    break;

                case 5://tblJob
                    if (jobs.Count==0)
                    {
                        jobs = connection.GetAllJobs();
                    }
                    break;

                case 6://tblProduct
                    if (products.Count==0)
                    {
                        products = connection.GetAllProducts();
                    }
                    if (providers.Count==0)
                    {
                        providers = connection.GetAllProviders();
                    }
                    foreach (var product in products)
                    {

                        string productString = product.ToDisplayString(out int perLevelId);
                        string providerString = providers.Find(x => x.ProviderID == perLevelId).ProviderName;//Finds the record with the specified ProviderID and assigns the name of that provider to a string.
                        productString.Replace("@provider", providerString);//Assigns the name of the provider to the product string.
                    }
                    break;

                case 7://tblStockLevel
                    if (stockLevels.Count==0)
                    {
                        stockLevels = connection.GetAllStocks();
                    }
                    if (products.Count==0)
                    {
                        products = connection.GetAllProducts();
                    }
                    if (stores.Count==0)
                    {
                        stores = connection.GetAllStores();
                    }
                    foreach (var stockLevel in stockLevels)
                    {
                        string stockString = stockLevel.ToDisplayString(out int storeId,out int productId);
                        string productString = products.Find(x => x.ProductID == productId).ProductName;//Finds the record with the specified ProductID and assigns the name of that product to a string.
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        stockString.Replace("@product", productString);//Assigns the name of the product to the stockLevel string.
                        stockString.Replace("@store",storeString);//Assigns the name of the store to the stockLevel string.
                    }
                    break;
                case 8://tblTransaction
                    if (transactions.Count==0)
                    {
                        transactions = connection.GetAllTransactions();
                    }
                    if (products.Count == 0)
                    {
                        products = connection.GetAllProducts();
                    }
                    if (stores.Count == 0)
                    {
                        stores = connection.GetAllStores();
                    }
                    foreach (var transaction in transactions)
                    {
                        string transactionString = transaction.ToDisplayString(out int storeId, out int productId);
                        string productString = products.Find(x => x.ProductID == productId).ProductName;//Finds the record with the specified ProductID and assigns the name of that product to a string.
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        transactionString.Replace("@product", productString);//Assigns the name of the product to the transaction string.
                        transactionString.Replace("@store", storeString);//Assigns the name of the store to the transaction string.
                    }
                    break;
                case 9://tblProductOrder
                    if (productOrders.Count == 0)
                    {
                        productOrders = connection.GetAllProductOrders();
                    }
                    if (products.Count == 0)
                    {
                        products = connection.GetAllProducts();
                    }
                    if (stores.Count == 0)
                    {
                        stores = connection.GetAllStores();
                    }
                    foreach (var productOrder in productOrders)
                    {
                        string productOrderString = productOrder.ToDisplayString(out int storeId, out int productId);
                        string productString = products.Find(x => x.ProductID == productId).ProductName;//Finds the record with the specified ProductID and assigns the name of that product to a string.
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        productOrderString.Replace("@product", productString);//Assigns the name of the product to the productOrder string.
                        productOrderString.Replace("@store", storeString);//Assigns the name of the store to the productOrder string.
                    }
                    break;

                case 10://tblStoreHour
                    if (storeHours.Count == 0)
                    {
                        storeHours = connection.GetAllStoreHours();
                    }
                    if (stores.Count == 0)
                    {
                        stores = connection.GetAllStores();//Stores are a dependency of StoreHours since StoreID is a foreign key in the scheduling.tblStoreHour table.
                    }
                    //Format the strings and pass them to a TableRecordDisplayer method.
                    foreach (var storeHour in storeHours)
                    {

                        string storeHourString = storeHour.ToDisplayString(out int storeId);
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        storeHourString.Replace("@perlevel", storeString);//Assigns the name of the store to the storeHour string.
                    }
                    break;
                case 11://tblEmployeeShift
                    if (employeeShifts.Count==0)
                    {
                        employeeShifts = connection.GetAllEmployeeShifts();
                    }
                    if (employees.Count==0)
                    {
                        employees = connection.GetAllEmployees();
                    }
                    if (stores.Count==0)
                    {
                        stores = connection.GetAllStores();
                    }
                    if (jobs.Count==0)
                    {
                        jobs = connection.GetAllJobs();
                    }

                    foreach (var employeeShift in employeeShifts)
                    {
                        string employeeShiftString = employeeShift.ToDisplayString(out int storeId, out int employeeId,out int jobId);
                        string employeeString = employees.Find(x => x.EmployeeID == employeeId).FullName;//Finds the record with the specified EmployeeID and assigns the name of that employee to a string.
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        string jobString = jobs.Find(x => x.JobID == jobId).JobName;//Finds the record with the specified JobID and assigns the name of that job to a string.
                        employeeShiftString.Replace("@employee", employeeString);//Assigns the name of the employee to the employeeShift string.
                        employeeShiftString.Replace("@store", storeString);//Assigns the name of the store to the employeeShift string.
                        employeeShiftString.Replace("@job", jobString);//Assigns the name of the job to the employeeShift string.
                    }

                    break;
                default:
                    break;
            }

            #endregion



            //The following region is the block of code which inserts records into the database.
            #region InsertStatements
            var recordsToInsert;//Assign this list from a method.
            switch (recordsToInsert)
            {
                case List<Employee>:
                    connection.InsertRecords<Employee>(recordsToInsert, "scheduling.tblEmployee");
                    break;

                case List<Store>:
                    connection.InsertRecords<Store>(recordsToInsert, "scheduling.tblStore");
                    break;
                case List<StoreHour>:
                    connection.InsertRecords<StoreHour>(recordsToInsert, "scheduling.tblStoreHours");
                    break;
                default:

                    break;
            }
            //Entire layout of this code in question- since type in <> is defined at compile time, it may be better to switch before assigning types
            #endregion

            //Remaining to do: 
            //-fill out array of valid characters(mostly done)
            //-finish parameterisation methods
            //-connect finished blocks to complete functionality
            //-implement string-length checks on all string values in the Model folder
            //-other?
            //fully organise and comment code(stretch goal)
        }
    }
}
