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

        //The following lists are used to store the records from the corresponding table.
        #region tableLists
        static List<Employee> employees = new List<Employee> { };
        static List<PermissionLevel> perLevels = new List<PermissionLevel> { };
        static List<Store> stores = new List<Store> { };
        static List<StoreHour> storeHours = new List<StoreHour> { };
        static List<Job> jobs = new List<Job> { };
        static List<Provider> providers = new List<Provider> { };
        static List<ProductOrder> productOrders = new List<ProductOrder> { };
        static List<Product> products = new List<Product> { };
        static List<EmployeeShift> employeeShifts = new List<EmployeeShift> { };
        static List<Transaction> transactions = new List<Transaction> { };
        static List<StockLevel> stockLevels = new List<StockLevel> { };
        static List<object[]> recordsToInsert;

        #endregion

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
            /*const string qryProductOrder = "SELECT P.ProductName AS Product, S.StoreName AS Store, O.Quantity, P.SupplyPrice*O.Quantity AS Price FROM tblProduct P, tblStore S, tblProductOrder O WHERE P.ProductID=O.ProductID AND S.StoreID=O.StoreID AND DATEDIFF(dy,GETDATE(),O.Date)<30 @ ORDER BY O.Date;";
            const string qryEmployeeShifts = "SELECT (E.FirstName+E.LastName) AS Name, S.StoreName AS Store, I.StartTime AS Start Time, I.EndTime AS End Time, I.Weekday AS Day, J.JobName AS Job FROM tblEmployee E, tblStore S, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND S.StoreID=I.StoreID AND J.JobID=I.JobID @ ORDER BY 2,1,6,5,3,4;";
            const string qryProductSales = "SELECT COALESCE(P.ProductName,Total)AS Product, COALESCE(S.StoreName, Subtotal) AS Store, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblProduct P, tblTransaction T RIGHT JOIN tblStore S ON T.StoreID=S.StoreID WHERE P.ProductID=T.ProductID @ GROUP BY ROLLUP(S.StoreName,P.ProductName) ORDER BY 1,2;";
            const string qryStoreSales = "SELECT COALESCE(S.StoreName,Grand Total)AS Store, COALESCE(P.ProductName, Total Sales) AS Product, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblStore S, tblTransaction T RIGHT JOIN tblProduct P ON T.ProductID=P.ProductID WHERE S.StoreID=T.StoreID @ GROUP BY ROLLUP(P.ProductName,S.StoreName) ORDER BY 1,2;";
            const string qryBestProducts = "SELECT TOP 50 P.ProductName AS Product, SUM(T.Quantity) AS Total Sales, SUM(T.Quantity*P.CustomerPrice) AS Total Revenue, SUM((T.Quantity*P.CustomerPrice)-(T.Quantity*P.SupplyPrice)) AS Profit FROM tblProduct P, tblStore S, tblTransaction T WHERE S.StoreID=T.StoreID AND P.ProductID=T.ProductID @ GROUP BY P.ProductID ORDER BY 2 desc, 4 desc;";
            const string qryEmployeePermissions = "SELECT E.EmpID AS Employee Number, E.FirstName+ ' '+ E.LastName AS Name, l.PermissionName AS Permission Level FROM tblEmployee E, tblPermissionLevel l WHERE E.LevelID=l.LevelID @ ORDER BY l.PermissionLevel desc, 2;";
            const string qryEmployeePay = "SELECT COALESCE(E.FirstName + ' ' + E.LastName, Total) AS Employee Name, SUM(DATEDIFF(hh, I.EndTime, I.StartTime)*J.Pay) FROM tblEmployee E, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND J.JobID=I.JobID @ GROUP BY ROLLUP(E.EmpID) ORDER BY 1;";
            const string qryProductStocks = "SELECT P.ProductName AS Product, S.StoreName AS Store, L.RmnStock AS Quantity FROM tblProduct P, tblStore S, tblStockLevel L WHERE P.ProductID=L.ProductID AND S.StoreID=L.StoreID @ ORDER BY 1,3 desc;";
            const string qryAutoUpdateStocks = "UPDATE sales.tblStockLevel SET ";*/
            string[,] queries = 
                {
                {"qryProductOrder","SELECT P.ProductName AS Product, S.StoreName AS Store, O.Quantity, P.SupplyPrice*O.Quantity AS Price FROM tblProduct P, tblStore S, tblProductOrder O WHERE P.ProductID=O.ProductID AND S.StoreID=O.StoreID AND DATEDIFF(dy,GETDATE(),O.Date)<30 @ ORDER BY O.Date;" },
                {"qryEmployeeShifts" ,"SELECT (E.FirstName+E.LastName) AS Name, S.StoreName AS Store, I.StartTime AS Start Time, I.EndTime AS End Time, I.Weekday AS Day, J.JobName AS Job FROM tblEmployee E, tblStore S, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND S.StoreID=I.StoreID AND J.JobID=I.JobID @ ORDER BY 2,1,6,5,3,4;"},
                {"qryProductSales" ,"SELECT COALESCE(P.ProductName,Total)AS Product, COALESCE(S.StoreName, Subtotal) AS Store, SUM(T.Quantity)AS Quantity, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblProduct P, tblTransaction T RIGHT JOIN tblStore S ON T.StoreID=S.StoreID WHERE P.ProductID=T.ProductID @ GROUP BY ROLLUP(S.StoreName,P.ProductName) ORDER BY 1,2;"},
                {"qryStoreSales" ,"SELECT COALESCE(S.StoreName,Grand Total)AS Store, COALESCE(P.ProductName, Total Sales) AS Product, SUM(T.Quantity)AS Quantity, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblStore S, tblTransaction T RIGHT JOIN tblProduct P ON T.ProductID=P.ProductID WHERE S.StoreID=T.StoreID @ GROUP BY ROLLUP(P.ProductName,S.StoreName) ORDER BY 1,2;"},
                { "qryBestProducts","SELECT TOP 50 P.ProductName AS Product, SUM(T.Quantity) AS Total Sales, SUM(T.Quantity*P.CustomerPrice) AS Total Revenue, SUM((T.Quantity*P.CustomerPrice)-(T.Quantity*P.SupplyPrice)) AS Profit FROM tblProduct P, tblStore S, tblTransaction T WHERE S.StoreID=T.StoreID AND P.ProductID=T.ProductID @ GROUP BY P.ProductID ORDER BY 2 desc, 4 desc;"},
                {"qryEmployeePermissions" ,"SELECT E.EmpID AS Employee Number, E.FirstName+ ' '+ E.LastName AS Name, l.PermissionName AS Permission Level FROM tblEmployee E, tblPermissionLevel l WHERE E.LevelID=l.LevelID @ ORDER BY l.PermissionLevel desc, 2;"},
                {"qryEmployeePay" ,"SELECT COALESCE(E.FirstName + ' ' + E.LastName, Total) AS Employee Name, SUM(DATEDIFF(hh, I.EndTime, I.StartTime)*J.Pay) AS Shift Pay FROM tblEmployee E, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND J.JobID=I.JobID @ GROUP BY ROLLUP(E.EmpID) ORDER BY 1;"},
                {"qryProductStocks" ,"SELECT P.ProductName AS Product, S.StoreName AS Store, L.RmnStock AS Quantity FROM tblProduct P, tblStore S, tblStockLevel L WHERE P.ProductID=L.ProductID AND S.StoreID=L.StoreID @ ORDER BY 1,3 desc;"}
                };
            #endregion



            byte userPermissionLevel; //This value is used to determine which pages the user can access
            bool stayLoggedIn=true; //This value indicates whether the user has chosen to logout. It will remain true until changed by the logout system.
            bool stayInProgram = true;//This value indicates whether the user has chosen to close the program. It will remain true until changed by the exit system.

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
            //Note: the primary key column should always be the first one in the array. This is because the InsertRecords Page assumes that the primary key is the first column when excluding the key column.
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

            //0=int32
            //1=int16
            //2=string
            //3=Time
            //4=Date
            //5=decimal
            //6=Date+Time
            //7=day of week
            //8=bool
            //could add another datatype representing a foreign key integer. this would need a way to find the primary key this corresponds to(could do by searching the first column name in tableColumnNames, but this would still need a way to find/get the corresponding string field etc from the other list.(could just find the index in the list and assign name? but identifying column name difficult.))
            //can pattern match for the string with{Name} to get column(inefficient?)
            //or define lookup array(foreignKeyLookUp?)

            //This array stores a numerical representation of the datatypes in each table column
            byte[][] tableColumnDatatypes=
                [
                [0,2,2,5],//tblStore
                [0,2,0],//tblPermissionLevel
                [0,2,2,1],//tblEmployee
                [0,2,2],//tblProvider
                [0,0,2,5,5,2],//tblProduct
                [0,0,0,0],//tblStockLevel
                [0,0,0,6,1],//tblTransaction
                [0,2,5],//tblJob
                [0,0,2,3,3],//tblStoreHour
                [0,0,0,7,3,3,0],//tblEmployeeShift
                [0,0,0,0,4,8]//tblProductOrder
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
            //could just name query columns directly(fixes naming of aggregate columns)
            //also define column datatypes
            string[][] queryColumns =
                [
                ["Product","Store","Quantity","Price"],
                ["Name","Store","Start Time","End Time","Day","Job"],
                ["Product","Store","Quantity","Revenue"],
                ["Store","Product","Quantity","Revenue"],
                ["Product","Total Sales","Total Revenue","Profit"],
                ["Employee Number","Name","Permission Level"],
                ["Employee Name","Shift Pay"],
                ["Product","Store","Quantity"],
                ];



            #endregion

            


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

            do//Beginning of main program loop
            {
                int userPageChangeValue= mainview.GetUserPageChangeValue(null,out int genericValue);
                switch (userPageChangeValue)
                {
                    //Most of these cases return control directly to character input after displaying the page.
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
                        stayLoggedIn = mainview.GetYesNoInput("");
                        if (!stayLoggedIn)
                        {
                            mainview.DisplayMessage("Press M to return to the main menu");
                        }
                        break;
                    case 6://Exit
                        mainview.DisplayExitConfirmation();
                        stayInProgram = mainview.GetYesNoInput("");
                        if (!stayInProgram)
                        {
                            mainview.DisplayMessage("Press M to return to the main menu");
                        }
                        break;
                    case 7://Report Menu
                        mainview.GotoReportMenu(userPermissionLevel);
                        break;
                    case 8://A menu option. Further seperation of values necessary.
                        //Determine what this generic option points to
                        //The following block will determine the page/table/record/etc the generic option points to. It will use the userPageChangeValue and PageNum to find where to find the relevant data for the following  generic method.
                        //could make a method for readability?
                        switch (mainview.PageNum)
                        {
                            case < 8:

                                //insert pages
                                recordsToInsert= mainview.DisplayInsertRecordsPage(tableColumnNames[genericValue], tableColumnDatatypes[genericValue]);
                                //method to get records to insert; assigns to a List<object[]>

                                //pass list from previous to insert method
                                InsertRecords(recordsToInsert, genericValue);
                                break;
                            case < 12:
                                //points to tableColumnNames
                                //pass tableColumnNames[genericValue] to display method
                                //update pages

                                HandleRecordUpdating(tableColumnNames[genericValue],genericValue);//NOT DONE
                                break;
                            case < 14:
                                //points to tableColumnNames
                                //pass tableColumnNames[genericValue] to display method
                                //delete pages.

                                HandleRecordDeletion(tableColumnNames[genericValue],genericValue);
                                break;
                            case < 18:
                                //points to tablesInQueries
                                //pass tablesInQueries[genericValue] to method
                                //within method, foreach on that array to find potentially multiple arrays of column names, assigning them to a list and printing the list.
                                //needs parameters before getting records
                                //ask for parameterisation-parameterise-get column names-get records-print page
                                //covers reports

                                //get column names here
                                var recordsToDisplay = connection.ReportHandler(queries[genericValue, 1], "");//(does not work pending testing)
                                mainview.DisplayReport(queryColumns[genericValue],recordsToDisplay,queries[genericValue,0].TrimStart(['q','r','y']));

                                break;

                            default:
                                break;
                        }
                        if (true)//PageNum values not yet defined
                        {
                            //points to parameter pages(pages for query parameterisation, dependent on column datatype)
                        }

                        break;
                    default:
                        break;
                        //where to from here? use gotos? methods?
                        //page has been printed and is ready for user input.
                }
            } while (stayLoggedIn&&stayInProgram);//End of main program loop

            if (stayInProgram)
            {
                goto Login;
            }
            else
            {
                goto End;
                //The end of the program; All code execution will stop.
            }


            
            
            //Remaining to do: 
            //-fill out array of valid characters(mostly done)
            //-finish parameterisation methods(finish after everything else)
            //-connect finished blocks to complete functionality
            //-implement string-length checks on all string values in the Model folder
            //-make all CRUD blocks functional
            //-Update/delete blocks need the code to get the records to update/delete
            //update/delete blocks need to get the records from the database
            //-other?
            //fully organise and comment code(stretch goal)

            //-Delete functional
            //-Insert functional but missing user prompts
            //-Update and report unfinished


        End:;
        }
        public static void InsertRecords(List<object[]>recordsToInsert,int tableValue)
        {

            //The following region is the block of code which inserts records into the database.
            #region InsertStatement:
            switch (tableValue)
            {
                case 0:
                    List<Store> storeRecords = new List<Store> { };
                    foreach (var record in recordsToInsert)
                    {
                        storeRecords.Add(new Store(-1, (string)record[0], (string)record[1], (short)record[2]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<Store>(storeRecords, "scheduling.tblStore");
                    break;
                case 1:
                    List<PermissionLevel> permissionLevelRecords = new List<PermissionLevel> { };
                    foreach (var record in recordsToInsert)
                    {
                        permissionLevelRecords.Add(new PermissionLevel(-1, (string)record[0], (short)record[1]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<PermissionLevel>(permissionLevelRecords, "scheduling.tblPermissionLevel");
                    break;
                case 2:
                    List<Employee> empRecords = new List<Employee> { };
                    foreach (var record in recordsToInsert)
                    {
                        empRecords.Add(new Employee(-1, (string)record[0], (string)record[1], (short)record[2], Convert.ToHexStringLower(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes((string)record[3])))));
                    }
                    connection.InsertRecords<Employee>(empRecords, "scheduling.tblEmployee");
                    break;
                case 3:
                    List<Provider> providerRecords = new List<Provider> { };
                    foreach (var record in recordsToInsert)
                    {
                        providerRecords.Add(new Provider(-1, (string)record[0], (string)record[1]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<Provider>(providerRecords, "sales.tblProvider");
                    break;
                case 4:
                    List<Product> productRecords = new List<Product> { };
                    foreach (var record in recordsToInsert)
                    {
                        productRecords.Add(new Product(-1, (int)record[0], (string)record[1], (decimal)record[2], (decimal)record[3], (string)record[4]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<Product>(productRecords, "sales.tblProduct");
                    break;
                case 5:
                    List<StockLevel> stockLevelRecords = new List<StockLevel> { };
                    foreach (var record in recordsToInsert)
                    {
                        stockLevelRecords.Add(new StockLevel(-1, (int)record[0], (int)record[1], (int)record[2]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<StockLevel>(stockLevelRecords, "sales.tblStockLevel");
                    break;
                case 6:
                    List<Transaction> transactionRecords = new List<Transaction> { };
                    foreach (var record in recordsToInsert)
                    {
                        transactionRecords.Add(new Transaction(-1, (int)record[0], (int)record[1], (DateTime)record[2], (short)record[3]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<Transaction>(transactionRecords, "sales.tblTransaction");
                    break;
                case 7:
                    List<Job> jobRecords = new List<Job> { };
                    foreach (var record in recordsToInsert)
                    {
                        jobRecords.Add(new Job(-1, (string)record[0], (decimal)record[1]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<Job>(jobRecords, "scheduling.tblJob");
                    break;
                case 8:
                    List<StoreHour> storeHourRecords = new List<StoreHour> { };
                    foreach (var record in recordsToInsert)
                    {
                        storeHourRecords.Add(new StoreHour(-1, (int)record[0], (string)record[1], (TimeOnly)record[2], (TimeOnly)record[3]));
                    }
                    connection.InsertRecords<StoreHour>(storeHourRecords, "scheduling.tblStoreHours");
                    break;
                case 9:
                    List<EmployeeShift> employeeShiftRecords = new List<EmployeeShift> { };
                    foreach (var record in recordsToInsert)
                    {
                        employeeShiftRecords.Add(new EmployeeShift(-1, (int)record[0], (int)record[1], (DayOfWeek)record[2], (TimeOnly)record[3], (TimeOnly)record[4], (int)record[5]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<EmployeeShift>(employeeShiftRecords, "scheduling.tblEmpShift");
                    break;
                case 10:
                    List<ProductOrder> productOrderRecords = new List<ProductOrder> { };
                    foreach (var record in recordsToInsert)
                    {
                        productOrderRecords.Add(new ProductOrder(-1, (int)record[0], (int)record[1], (short)record[2], (DateTime)record[3], (bool)record[4]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    connection.InsertRecords<ProductOrder>(productOrderRecords, "scheduling.tblProductOrder");
                    break;
                default:

                    break;
            }
            #endregion
        }

        //Handles the deletion of records; includes prompting the user for records to delete and executing the resultant query.
        public static int HandleRecordDeletion(string[] tableColumnNames,int tableValue)
        {
            List<int> idsToDelete;

            switch (tableValue)
            {
                case 0://tblStore
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Store>(tableColumnNames, stores);
                    return connection.DeleteRecordsByID("scheduling.tblStore", tableColumnNames[0], idsToDelete);
                     
                case 1://tblPermissionLevel
                    idsToDelete = mainview.DisplayDeleteRecordsPage<PermissionLevel>(tableColumnNames, perLevels);
                    return connection.DeleteRecordsByID("scheduling.tblPermissionLevel", tableColumnNames[0], idsToDelete);
                     
                case 2://tblEmployee
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Employee>(tableColumnNames, employees);
                    return connection.DeleteRecordsByID("scheduling.tblEmployee", tableColumnNames[0], idsToDelete);
                     
                case 3://tblProvider
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Provider>(tableColumnNames, providers);
                    return connection.DeleteRecordsByID("sales.tblProvider", tableColumnNames[0], idsToDelete);
                     
                case 4://tblProduct
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Product>(tableColumnNames, products);
                    return connection.DeleteRecordsByID("sales.tblProduct", tableColumnNames[0], idsToDelete);
                     
                case 5://tblStockLevel
                    idsToDelete = mainview.DisplayDeleteRecordsPage<StockLevel>(tableColumnNames, stockLevels);
                    return connection.DeleteRecordsByID("sales.tblStocklevel", tableColumnNames[0], idsToDelete);
                     
                case 6://tblTransaction
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Transaction>(tableColumnNames, transactions);
                    return connection.DeleteRecordsByID("sales.tblTransaction", tableColumnNames[0], idsToDelete);
                     
                case 7://tblJob
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Job>(tableColumnNames, jobs);
                    return connection.DeleteRecordsByID("scheduling.tblJob", tableColumnNames[0],idsToDelete);
                     
                case 8://tblStoreHour
                    idsToDelete = mainview.DisplayDeleteRecordsPage<StoreHour>(tableColumnNames, storeHours);
                    return connection.DeleteRecordsByID("scheduling.tblStoreHour", tableColumnNames[0], idsToDelete);
                     
                case 9://tblEmpShift
                   idsToDelete= mainview.DisplayDeleteRecordsPage<EmployeeShift>(tableColumnNames,employeeShifts);
                    return connection.DeleteRecordsByID("scheduling.tblEmpShift", tableColumnNames[0], idsToDelete);
                     
                case 10://tblProductOrder
                    idsToDelete = mainview.DisplayDeleteRecordsPage<ProductOrder>(tableColumnNames, productOrders);
                    return connection.DeleteRecordsByID("sales.tblProductOrder", tableColumnNames[0], idsToDelete);
                     
                default:
                    return 0;
                     
                    //returns the number of rows affected
            }
           
        }
        public static int HandleRecordUpdating(string[] tableColumnNames, int tableValue)
        {
            List<int> idsToUpdate;
            List<string> colNames;
            List<string> newValues;
            int totalAffectedRows=0;
            int i = 0;
            switch (tableValue)
            {
                case 0://tblStore
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Store>(tableColumnNames,stores,out colNames,out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblStore", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 1://tblPermissionLevel
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<PermissionLevel>(tableColumnNames, perLevels, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblPermissionLevel", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 2://tblEmployee
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Employee>(tableColumnNames, employees, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblEmployee", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 3://tblProvider
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Provider>(tableColumnNames, providers, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblProvider", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 4://tblProduct
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Product>(tableColumnNames, products, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblProduct", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 5://tblStockLevel
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<StockLevel>(tableColumnNames, stockLevels, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblStockLevel", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 6://tblTransaction
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Transaction>(tableColumnNames, transactions, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblTransaction", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 7://tblJob
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Job>(tableColumnNames, jobs, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblJob", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 8://tblStoreHour
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<StoreHour>(tableColumnNames, storeHours, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblStoreHour", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 9://tblEmpShift
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<EmployeeShift>(tableColumnNames, employeeShifts, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblEmpShift", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 10://tblProductOrder
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<ProductOrder>(tableColumnNames, productOrders, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblProductOrder", tableColumnNames[0], recordId.ToString(), colNames[i], newValues[i]);
                        i++;
                    }
                    //add method call here to auto-update tblStockLevel using reflection
                    break;
                default:
                    break;
            }


            return 0;
        }

        public static void GetRelevantRecords(int getTableRecordsValue)
        {

            //The following is a block to get all the records from different tables in the database.
            //The results may have dependencies in other tables(Foreign Keys). These dependencies will need to be retrieved through the same code block.
            #region getRecordStrings
            switch (getTableRecordsValue)
            {
                case 1://tblEmployee

                    //Gets the Employees and Permission Levels from the database if they have not been gotten already
                    if (employees.Count == 0)
                    {
                        employees = connection.GetAllEmployees();
                    }
                    if (perLevels.Count == 0)
                    {
                        perLevels = connection.GetPermissionLevels();//PermissionLevels are a dependency of Employees since PerLevelID is a foreign key in the scheduling.tblEmployee table.
                    }
                    //Format the strings and pass them to a TableRecordDisplayer method.
                    foreach (var employee in employees)
                    {

                        string employeeString = employee.ToDisplayString(out int perLevelId);
                        string perLevelString = perLevels.Find(x => x.PerLevelID == perLevelId).LevelName;//Finds the record with the specified PerLevelID and assigns the name of that permissionlevel to a string.
                        employeeString = employeeString.Replace("@perlevel", perLevelString);//Assigns the name of the perlevel to the employee string.
                    }

                    //Strings formatted, now create and pass a list of strings to be displayed.

                    break;
                case 2://tblPermissionLevel
                    if (perLevels.Count == 0)
                    {
                        perLevels = connection.GetPermissionLevels();
                    }

                    break;
                case 3://tblStore
                    if (stores.Count == 0)
                    {
                        stores = connection.GetAllStores();
                    }
                    break;

                case 4://tblProvider
                    if (providers.Count == 0)
                    {
                        providers = connection.GetAllProviders();
                    }
                    break;

                case 5://tblJob
                    if (jobs.Count == 0)
                    {
                        jobs = connection.GetAllJobs();
                    }
                    break;

                case 6://tblProduct
                    if (products.Count == 0)
                    {
                        products = connection.GetAllProducts();
                    }
                    if (providers.Count == 0)
                    {
                        providers = connection.GetAllProviders();
                    }
                    foreach (var product in products)
                    {

                        string productString = product.ToDisplayString(out int perLevelId);
                        string providerString = providers.Find(x => x.ProviderID == perLevelId).ProviderName;//Finds the record with the specified ProviderID and assigns the name of that provider to a string.
                        productString = productString.Replace("@provider", providerString);//Assigns the name of the provider to the product string.
                    }
                    break;

                case 7://tblStockLevel
                    if (stockLevels.Count == 0)
                    {
                        stockLevels = connection.GetAllStocks();
                    }
                    if (products.Count == 0)
                    {
                        products = connection.GetAllProducts();
                    }
                    if (stores.Count == 0)
                    {
                        stores = connection.GetAllStores();
                    }
                    foreach (var stockLevel in stockLevels)
                    {
                        string stockString = stockLevel.ToDisplayString(out int storeId, out int productId);
                        string productString = products.Find(x => x.ProductID == productId).ProductName;//Finds the record with the specified ProductID and assigns the name of that product to a string.
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        stockString = stockString.Replace("@product", productString);//Assigns the name of the product to the stockLevel string.
                        stockString = stockString.Replace("@store", storeString);//Assigns the name of the store to the stockLevel string.
                    }
                    break;
                case 8://tblTransaction
                    if (transactions.Count == 0)
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
                        transactionString = transactionString.Replace("@product", productString);//Assigns the name of the product to the transaction string.
                        transactionString = transactionString.Replace("@store", storeString);//Assigns the name of the store to the transaction string.
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
                        productOrderString = productOrderString.Replace("@product", productString);//Assigns the name of the product to the productOrder string.
                        productOrderString = productOrderString.Replace("@store", storeString);//Assigns the name of the store to the productOrder string.
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
                        storeHourString = storeHourString.Replace("@perlevel", storeString);//Assigns the name of the store to the storeHour string.
                    }
                    break;
                case 11://tblEmployeeShift
                    if (employeeShifts.Count == 0)
                    {
                        employeeShifts = connection.GetAllEmployeeShifts();
                    }
                    if (employees.Count == 0)
                    {
                        employees = connection.GetAllEmployees();
                    }
                    if (stores.Count == 0)
                    {
                        stores = connection.GetAllStores();
                    }
                    if (jobs.Count == 0)
                    {
                        jobs = connection.GetAllJobs();
                    }

                    foreach (var employeeShift in employeeShifts)
                    {
                        string employeeShiftString = employeeShift.ToDisplayString(out int storeId, out int employeeId, out int jobId);
                        string employeeString = employees.Find(x => x.EmployeeID == employeeId).FullName;//Finds the record with the specified EmployeeID and assigns the name of that employee to a string.
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        string jobString = jobs.Find(x => x.JobID == jobId).JobName;//Finds the record with the specified JobID and assigns the name of that job to a string.
                        employeeShiftString= employeeShiftString.Replace("@employee", employeeString);//Assigns the name of the employee to the employeeShift string.
                        employeeShiftString = employeeShiftString.Replace("@store", storeString);//Assigns the name of the store to the employeeShift string.
                        employeeShiftString = employeeShiftString.Replace("@job", jobString);//Assigns the name of the job to the employeeShift string.
                    }

                    break;
                default:
                    break;
            }

            #endregion
        }
    }
}
