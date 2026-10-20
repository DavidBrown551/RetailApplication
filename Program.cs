using Microsoft.Data.SqlClient;
using RetailApplication.Controller;
using RetailApplication.Model;
using RetailApplication.View;
using System.Security.Cryptography;
namespace RetailApplication
{
    internal class Program
    {
        private static StorageManager connection;
        //This is the object that will manage the connection to the Sql Database

        private static ViewConsole mainview;
        //This is the object that will be called to display a page
        //It is called mainview to leave scope for other views later on.

        //The following lists are used to store the records from the corresponding table.
        #region tableLists
        static List<Employee> employees = [];
        static List<PermissionLevel> perLevels = [];
        static List<Store> stores =[];
        static List<StoreHour> storeHours = [];
        static List<Job> jobs = [];
        static List<Provider> providers = [];
        static List<ProductOrder> productOrders = [];
        static List<Product> products = [];
        static List<EmployeeShift> employeeShifts = [];
        static List<Transaction> transactions = [];
        static List<StockLevel> stockLevels = [];
        static List<object[]> recordsToInsert;

        #endregion

        //The queries used to get reports from the database
        #region Queries
        //These will be passed to the ReportHandler method in the Controller folder and used to query the database. The '@' is a universal entry point for parameters to be added, and is replaced by whitespace if the query is not parameterised.
        readonly static string[,] queries =
            {
                {"qryProductOrder","SELECT P.ProductName AS Product, S.StoreName AS Store, O.Quantity, P.SupplyPrice*O.Quantity AS Price FROM sales.tblProduct P, scheduling.tblStore S, sales.tblProductOrder O WHERE P.ProductID=O.ProductID AND S.StoreID=O.StoreID AND DATEDIFF(dy,GETDATE(),O.Date)<30 @ ORDER BY O.Date,S.StoreName;" },
                {"qryEmployeeShifts" ,"SELECT (E.FirstName+ ' ' +E.LastName) AS Name, S.StoreName AS Store, I.StartTime AS \"Start Time\", I.EndTime AS \"End Time\", I.Weekday AS Day, J.JobName AS Job FROM scheduling.tblEmployee E, scheduling.tblStore S, scheduling.tblEmpShift I, scheduling.tblJob J WHERE E.EmpID=I.EmpID AND S.StoreID=I.StoreID AND J.JobID=I.JobID @ ORDER BY 2,1,6,5,3,4;"},
                {"qryProductSales" ,"SELECT COALESCE(CONCAT_WS( ' ',P.ProductName,P.Amount),'Total')AS Product, COALESCE(S.StoreName, 'Subtotal') AS Store, SUM(T.Quantity)AS Quantity, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM sales.tblProduct P, sales.tblTransaction T RIGHT JOIN scheduling.tblStore S ON T.StoreID=S.StoreID WHERE P.ProductID=T.ProductID @ GROUP BY ROLLUP((P.ProductID,CONCAT_WS(' ',P.ProductName,P.Amount)),S.StoreName) ORDER BY P.ProductID,1;"},
                {"qryStoreSales" ,"SELECT COALESCE(S.StoreName,'Grand Total')AS Store, COALESCE(CONCAT_WS(' ',P.ProductName,P.Amount), 'Subtotal') AS Product, SUM(T.Quantity)AS Quantity, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM scheduling.tblStore S, sales.tblTransaction T RIGHT JOIN sales.tblProduct P ON T.ProductID=P.ProductID WHERE S.StoreID=T.StoreID @ GROUP BY ROLLUP((S.StoreID,S.StoreName),(P.ProductID,CONCAT_WS(' ',P.ProductName,P.Amount))) ORDER BY S.StoreID,1;"},
                { "qryBestProducts","SELECT TOP 50 CONCAT_WS(' ',P.ProductName,P.Amount) AS Product, SUM(T.Quantity) AS \"Total Sales\", SUM(T.Quantity*P.CustomerPrice) AS \"Total Revenue\", SUM((T.Quantity*P.CustomerPrice)-(T.Quantity*P.SupplyPrice)) AS Profit FROM sales.tblProduct P, scheduling.tblStore S, sales.tblTransaction T WHERE S.StoreID=T.StoreID AND P.ProductID=T.ProductID @ GROUP BY P.ProductID,P.ProductName,P.Amount ORDER BY 2 desc, 4 desc;"},
                {"qryEmployeePermissions" ,"SELECT E.EmpID AS \"Employee Number\", E.FirstName+ ' '+ E.LastName AS Name, l.LevelName AS \"Permission Level\" FROM scheduling.tblEmployee E, scheduling.tblPermissionLevel l WHERE E.LevelID=l.LevelID @ ORDER BY l.PermissionLevel desc, 2;"},
                {"qryEmployeePay" ,"SELECT COALESCE(E.FirstName + ' ' + E.LastName, 'Employee Total') AS \"Employee Name\", SUM(DATEDIFF(hh, I.StartTime, I.EndTime)*J.Pay) AS \"Shift Pay\" FROM scheduling.tblEmployee E, scheduling.tblEmpShift I, scheduling.tblJob J WHERE E.EmpID=I.EmpID AND J.JobID=I.JobID @ GROUP BY E.EmpID,E.FirstName,E.LastName ORDER BY 1;"},
                {"qryProductStocks" ,"SELECT (P.ProductName+' '+ P.Amount)AS Product, S.StoreName AS Store, L.RmnStock AS Quantity FROM sales.tblProduct P, scheduling.tblStore S, sales.tblStockLevel L WHERE P.ProductID=L.ProductID AND S.StoreID=L.StoreID @ ORDER BY 1,3 desc;"},
                {"qryProductProviders" ,"SELECT P.ProductName AS Product, D.ProviderName AS Provider, D.Contact FROM sales.tblProvider D, sales.tblProduct P WHERE P.ProviderID=D.ProviderID @ ORDER BY D.ProviderID;"},
                {"qryStoreHours","SELECT S.StoreName AS Store, H.Weekday, H.OpenTime AS \"Opening Time\", H.CloseTime AS \"Closing Time\" FROM scheduling.tblStore S, scheduling.tblStoreHours H WHERE S.StoreID=H.StoreID @ ORDER BY S.StoreID, 2;" },
                {"qryEmployees","SELECT E.EmpID AS \"Employee Number\", E.FirstName+ ' '+ E.LastName AS Name FROM scheduling.tblEmployee E @ ORDER BY 1;" },
                {"qryPermissions","SELECT * FROM scheduling.tblPermissionLevel @ ORDER BY PermissionLevel desc;" }
                };
        #endregion

        //The following region contains the arrays that store the data used by the program(coulumn names, datatypes, etc.)
        #region lookupArrays

        //This array stores the names of the tables
        readonly static string[] tables =
            [
            "tblStore",
                "tblPermissionLevel",
                "tblEmployee",
                "tblProvider",
                "tblProduct",
                "tblStockLevel",
                "tblTransaction",
                "tblJob",
                "tblStoreHours",
                "tblEmployeeShift",
                "tblProductOrder"
            ];

        //This array stores the column names of each table. Note that currently these are used in both user-facing display and query parameterisation.
        //Note: the primary key column should always be the first one in the array. This is because the InsertRecords Page assumes that the primary key is the first column when excluding the key column.
        readonly static string[][] tableColumnNames =
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
        //9=byte

        //This array stores a numerical representation of the datatypes in each table column
        readonly static byte[][] tableColumnDatatypes =
            [
            [0,2,2,5],//tblStore
                [0,2,9],//tblPermissionLevel
                [0,2,2,0],//tblEmployee
                [0,2,2],//tblProvider
                [0,0,2,5,5,2],//tblProduct
                [0,0,0,0],//tblStockLevel
                [0,0,0,6,1],//tblTransaction
                [0,2,5],//tblJob
                [0,0,7,3,3],//tblStoreHour
                [0,0,0,7,3,3,0],//tblEmployeeShift
                [0,0,0,1,4,8]//tblProductOrder
            ];
        //This are the column headings for the reports. They are used by the ViewConsole.DisplayReport method to display the column headings.
        readonly static string[][] queryColumns =
            [
            ["Product","Store","Quantity","Price"],
                ["Name","Store","Start Time","End Time","Day","Job"],
                ["Product","Store","Quantity","Revenue"],
                ["Store","Product","Quantity","Revenue"],
                ["Product","Total Sales","Total Revenue","Profit"],
                ["Employee Number","Name","Permission Level"],
                ["Employee Name","Shift Pay"],
                ["Product","Store","Quantity"],
            ["Product","Provider","Contact"],
            ["Store","Weekday","Opening Time","Closing Time"],
            ["Employee Number","Name"],
            ["PermissionID","Permission Name","Permission Level"]
                ];



        #endregion



        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=RetailDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
            //This string allows connection to the database and must be changed if the file name or file path is changed.
        TryConnection:
            mainview = new ViewConsole();
            connection = new StorageManager(connectionString);//Creates a new connection to the database
          

            byte userPermissionLevel; //This value is used to determine which pages the user can access
            bool stayLoggedIn=true; //This value indicates whether the user has chosen to logout. It will remain true until changed by the logout system.
            bool stayInProgram = true;//This value indicates whether the user has chosen to close the program. It will remain true until changed by the exit system.

            //After establishing a connection to the database, the login page is shown. 
        Login:
            mainview.DisplayLoginPage();
            

            int enteredEmpId=ViewConsole.GetIntegerInput();
            //Gets an error-corrected integer from the user representing their employee number
            
            string hashedUserPassword = Convert.ToHexStringLower(MD5.HashData(mainview.ConcealTypedField()));//Gets the user password as a byte[], allowing the use of MD5.HashData(). The resulting byte[] is converted to a string in the same format as the database(Lowercase Hex).
            try
            {
                if (hashedUserPassword.Equals(connection.GetUserPassword(enteredEmpId)))//Checks whether the hash for the user-submitted password matches the hash stored in the database with the input employee number
                {
                    userPermissionLevel = connection.GetUserPermission(enteredEmpId);//If so the userPermissionLevel is assigned using the input employee number and the user is sent to the main menu
                    goto Main_Menu;
                }
                else
                {
                    //If the hashes do not match, the user is sent back to the password screen
                    goto Login;
                }
            }
            catch (Exception)//Try reproduce error and specify exception type
            {
                ViewConsole.DisplayMessage("Connection timed out. Press any key to try again. if problems persist, contact support.");
                goto TryConnection;
                throw;
            }
            

        Main_Menu:
            mainview.GotoMainMenu(userPermissionLevel);//Displays the main menu for the user


            do//Beginning of main program loop
            {
                stayLoggedIn = true;
                int userPageChangeValue= mainview.GetUserPageChangeValue(null,out int genericValue);//Gets input from the user. userPageChangeValue represents which menu the user wishes to go to.
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
                        stayLoggedIn = !ViewConsole.GetYesNoInput("");
                        if (stayLoggedIn)
                        {
                            ViewConsole.DisplayMessage("Press M to return to the main menu");
                        }
                        break;
                    case 6://Exit
                        mainview.DisplayExitConfirmation();
                        stayInProgram = !ViewConsole.GetYesNoInput("");
                        if (stayInProgram)
                        {
                            ViewConsole.DisplayMessage("Press M to return to the main menu");
                        }
                        break;
                    case 7://Report Menu
                        mainview.GotoReportMenu(userPermissionLevel);
                        break;
                    case 8://A menu option. Further seperation of values necessary.
                        //The following block determines the page/table/record/etc the generic option points to. It uses genericValue and PageNum to find where to find the relevant data for the following generic method.
                        switch (mainview.PageNum)
                        {
                            case < 9:
                                //Insert function
                                //This is a solution for a problem during record insertion that occurs due to the employee password field normally not being needed by the program. This block adds the necessesary column name and datatype number to the usual array.
                                string[] insertColNames;
                                byte[] insertColTypes;
                                if (genericValue==2)//corresponds to tblEmployee
                                {
                                    insertColNames = [tableColumnNames[genericValue][0], tableColumnNames[genericValue][1], tableColumnNames[genericValue][2], tableColumnNames[genericValue][3], "Password"];
                                    insertColTypes = [tableColumnDatatypes[genericValue][0], tableColumnDatatypes[genericValue][1], tableColumnDatatypes[genericValue][2], tableColumnDatatypes[genericValue][3], 2];
                                }
                                else//This block does nothing to all other tables.
                                {
                                    insertColNames = tableColumnNames[genericValue];
                                    insertColTypes = tableColumnDatatypes[genericValue];
                                }
                                recordsToInsert= mainview.DisplayInsertRecordsPage(insertColNames, insertColTypes);
                                //method to get records to insert; assigns to a List<object[]>

                                //passes list from previous to insert method
                                InsertRecords(recordsToInsert, genericValue);
                                break;
                            case < 12:
                                //Update function
                                GetRelevantRecords(genericValue);//Gets the records of the table being updated
                                int rowsUpdated= HandleRecordUpdating(tableColumnNames[genericValue],genericValue);//NOT DONE, needs to prompt user for input.
                                ViewConsole.DisplayMessage($"{rowsUpdated} row(s) updated.\nPress M to return to the main menu.");
                                break;
                            case < 14:
                                //Delete function
                                GetRelevantRecords(genericValue);//Gets the records of the table being updated
                                int rowsDeleted=HandleRecordDeletion(tableColumnNames[genericValue],genericValue);
                                ViewConsole.DisplayMessage($"{rowsDeleted} row(s) deleted.\nPress M to return to the main menu.");
                                break;
                            case < 18:
                                //points to queries
                                var recordsToDisplay = connection.ReportHandler(queries[genericValue, 1], "");//Gets the records to display as a List<string[]>, using the user-specified query.
                                mainview.DisplayReport(queryColumns[genericValue],recordsToDisplay,queries[genericValue,0].TrimStart(['q','r','y']));
                                goto Main_Menu;

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



        End:;
        }
        public static void InsertRecords(List<object[]>recordsToInsert,int tableValue)
        {

            //The following region is the block of code which inserts records into the database.
            int rowsInserted=0;
            #region InsertStatement:
            switch (tableValue)
            {
                case 0:
                    List<Store> storeRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        storeRecords.Add(new Store(-1, (string)record[0], (string)record[1], (decimal)record[2]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted= connection.InsertRecords<Store>(storeRecords, "scheduling.tblStore", tableColumnNames[tableValue]);
                    break;
                case 1:
                    List<PermissionLevel> permissionLevelRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        permissionLevelRecords.Add(new PermissionLevel(-1, (string)record[0], (byte)record[1]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<PermissionLevel>(permissionLevelRecords, "scheduling.tblPermissionLevel", tableColumnNames[tableValue]);
                    break;
                case 2:
                    string[] allEmployeeColumns= [tableColumnNames[tableValue][0], tableColumnNames[tableValue][1], tableColumnNames[tableValue][2], tableColumnNames[tableValue][3], "Password"];
                    List<Employee> empRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        empRecords.Add(new Employee(-1, (string)record[0], (string)record[1], (int)record[2],Convert.ToHexStringLower(MD5.HashData(System.Text.Encoding.UTF8.GetBytes((string)record[3])))));
                    }
                    rowsInserted = connection.InsertRecords<Employee>(empRecords, "scheduling.tblEmployee",allEmployeeColumns);
                    break;
                case 3:
                    List<Provider> providerRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        providerRecords.Add(new Provider(-1, (string)record[0], (string)record[1]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<Provider>(providerRecords, "sales.tblProvider", tableColumnNames[tableValue]);
                    break;
                case 4:
                    List<Product> productRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        productRecords.Add(new Product(-1, (int)record[0], (string)record[1], (decimal)record[2], (decimal)record[3], (string)record[4]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<Product>(productRecords, "sales.tblProduct", tableColumnNames[tableValue]);
                    break;
                case 5:
                    List<StockLevel> stockLevelRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        stockLevelRecords.Add(new StockLevel(-1, (int)record[0], (int)record[1], (int)record[2]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<StockLevel>(stockLevelRecords, "sales.tblStockLevel", tableColumnNames[tableValue]);
                    break;
                case 6:
                    List<Transaction> transactionRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        transactionRecords.Add(new Transaction(-1, (int)record[0], (int)record[1], (DateTime)record[2], (short)record[3]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<Transaction>(transactionRecords, "sales.tblTransaction", tableColumnNames[tableValue]);
                    break;
                case 7:
                    List<Job> jobRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        jobRecords.Add(new Job(-1, (string)record[0], (decimal)record[1]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<Job>(jobRecords, "scheduling.tblJob", tableColumnNames[tableValue]);
                    break;
                case 8:
                    List<StoreHour> storeHourRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        storeHourRecords.Add(new StoreHour(-1, (int)record[0], (DayOfWeek)record[1], (TimeOnly)record[2], (TimeOnly)record[3]));
                    }
                    rowsInserted = connection.InsertRecords<StoreHour>(storeHourRecords, "scheduling.tblStoreHours", tableColumnNames[tableValue]);
                    break;
                case 9:
                    List<EmployeeShift> employeeShiftRecords =[];
                    foreach (var record in recordsToInsert)
                    {
                        employeeShiftRecords.Add(new EmployeeShift(-1, (int)record[0], (int)record[1], (DayOfWeek)record[2], (TimeOnly)record[3], (TimeOnly)record[4], (int)record[5]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<EmployeeShift>(employeeShiftRecords, "scheduling.tblEmpShift", tableColumnNames[tableValue]);
                    break;
                case 10:
                    List<ProductOrder> productOrderRecords = [];
                    foreach (var record in recordsToInsert)
                    {
                        productOrderRecords.Add(new ProductOrder(-1, (int)record[0], (int)record[1], (short)record[2], (DateOnly)record[3], (bool)record[4]));//negative one is used since it will never occur in an actual record and is a blatant marker of an invalid record.
                    }
                    rowsInserted = connection.InsertRecords<ProductOrder>(productOrderRecords, "sales.tblProductOrder", tableColumnNames[tableValue]);
                    break;
                default:

                    break;
            }
            #endregion
            ViewConsole.DisplayMessage($"Inserted {rowsInserted} rows\nPress M to return to the main menu");
            
        }

        //Handles the deletion of records; includes prompting the user for records to delete and executing the resultant query.
        public static int HandleRecordDeletion(string[] tableColumnNames,int tableValue)
        {
            List<int> idsToDelete;
            int rowsaffected;
            switch (tableValue)
            {
                case 0://tblStore
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Store>(tableColumnNames, stores);
                    rowsaffected= connection.DeleteRecordsByID("scheduling.tblStore", tableColumnNames[0], idsToDelete);
                    stores.RemoveAll(x=>idsToDelete.Contains(x.StoreID));
                    break;
                case 1://tblPermissionLevel
                    idsToDelete = mainview.DisplayDeleteRecordsPage<PermissionLevel>(tableColumnNames, perLevels);
                    rowsaffected = connection.DeleteRecordsByID("scheduling.tblPermissionLevel", tableColumnNames[0], idsToDelete);
                    perLevels.RemoveAll(x => idsToDelete.Contains(x.PerLevelID));
                    break;
                case 2://tblEmployee
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Employee>(tableColumnNames, employees);
                    rowsaffected = connection.DeleteRecordsByID("scheduling.tblEmployee", tableColumnNames[0], idsToDelete);
                    employees.RemoveAll(x => idsToDelete.Contains(x.EmployeeID));
                    break;
                case 3://tblProvider
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Provider>(tableColumnNames, providers);
                    rowsaffected = connection.DeleteRecordsByID("sales.tblProvider", tableColumnNames[0], idsToDelete);
                    providers.RemoveAll(x => idsToDelete.Contains(x.ProviderID));
                    break;
                case 4://tblProduct
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Product>(tableColumnNames, products);
                    rowsaffected = connection.DeleteRecordsByID("sales.tblProduct", tableColumnNames[0], idsToDelete);
                    products.RemoveAll(x => idsToDelete.Contains(x.ProductID));
                    break;
                case 5://tblStockLevel
                    idsToDelete = mainview.DisplayDeleteRecordsPage<StockLevel>(tableColumnNames, stockLevels);
                    rowsaffected = connection.DeleteRecordsByID("sales.tblStocklevel", tableColumnNames[0], idsToDelete);
                    stockLevels.RemoveAll(x => idsToDelete.Contains(x.StockLevelID));
                    break;
                case 6://tblTransaction
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Transaction>(tableColumnNames, transactions);
                    rowsaffected = connection.DeleteRecordsByID("sales.tblTransaction", tableColumnNames[0], idsToDelete);
                    transactions.RemoveAll(x => idsToDelete.Contains(x.TransactionID));
                    break;
                case 7://tblJob
                    idsToDelete = mainview.DisplayDeleteRecordsPage<Job>(tableColumnNames, jobs);
                    rowsaffected = connection.DeleteRecordsByID("scheduling.tblJob", tableColumnNames[0],idsToDelete);
                    jobs.RemoveAll(x => idsToDelete.Contains(x.JobID));
                    break;
                case 8://tblStoreHour
                    idsToDelete = mainview.DisplayDeleteRecordsPage<StoreHour>(tableColumnNames, storeHours);
                    rowsaffected = connection.DeleteRecordsByID("scheduling.tblStoreHours", tableColumnNames[0], idsToDelete);
                    storeHours.RemoveAll(x => idsToDelete.Contains(x.StoreHourID));
                    break;
                case 9://tblEmpShift
                   idsToDelete= mainview.DisplayDeleteRecordsPage<EmployeeShift>(tableColumnNames,employeeShifts);
                    rowsaffected = connection.DeleteRecordsByID("scheduling.tblEmpShift", tableColumnNames[0], idsToDelete);
                    employeeShifts.RemoveAll(x => idsToDelete.Contains(x.EmpShiftID));
                    break;
                case 10://tblProductOrder
                    idsToDelete = mainview.DisplayDeleteRecordsPage<ProductOrder>(tableColumnNames, productOrders);
                    rowsaffected = connection.DeleteRecordsByID("sales.tblProductOrder", tableColumnNames[0], idsToDelete);
                    productOrders.RemoveAll(x => idsToDelete.Contains(x.OrderID));
                    break;
                default:
                    return 0;
                     
                    //returns the number of rows affected
            }
            return rowsaffected;
        }

        //This method handles updating records. It has 11 cases, each handling the update functionality for one of the database tables. Each case calls the ViewConsole.DisplayUpdateRecordsPage<T>, which returns lists for the record IDs and column names to update, along with the new values to update to. The cases then update each record using StorageManager.UpdateRecordByID. Finally, this method returns the total number of rows affected.
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
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblStore", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 1://tblPermissionLevel
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<PermissionLevel>(tableColumnNames, perLevels, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblPermissionLevel", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 2://tblEmployee
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Employee>(tableColumnNames, employees, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblEmployee", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 3://tblProvider
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Provider>(tableColumnNames, providers, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblProvider", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 4://tblProduct
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Product>(tableColumnNames, products, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblProduct", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 5://tblStockLevel
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<StockLevel>(tableColumnNames, stockLevels, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblStockLevel", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 6://tblTransaction
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Transaction>(tableColumnNames, transactions, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblTransaction", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 7://tblJob
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<Job>(tableColumnNames, jobs, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblJob", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 8://tblStoreHour
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<StoreHour>(tableColumnNames, storeHours, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblStoreHours", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 9://tblEmpShift
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<EmployeeShift>(tableColumnNames, employeeShifts, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("scheduling.tblEmpShift", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    break;
                case 10://tblProductOrder
                    idsToUpdate = mainview.DisplayUpdateRecordsPage<ProductOrder>(tableColumnNames, productOrders, out colNames, out newValues);
                    foreach (var recordId in idsToUpdate)
                    {
                        totalAffectedRows += connection.UpdateRecordByID("sales.tblProductOrder", tableColumnNames[0], recordId, colNames[i], newValues[i]);
                        i++;
                    }
                    //add method call here to auto-update tblStockLevel using reflection
                    break;
                default:
                    break;
            }
            return totalAffectedRows;
        }

        public static void GetRelevantRecords(int getTableRecordsValue)
        {

            //The following is a block to get all the records from different tables in the database.
            //The results may have dependencies in other tables(Foreign Keys). These dependencies will need to be retrieved through the same code block.
            #region getRecordStrings
            switch (getTableRecordsValue)
            {
                case 2://tblEmployee

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
                case 1://tblPermissionLevel
                    if (perLevels.Count == 0)
                    {
                        perLevels = connection.GetPermissionLevels();
                    }

                    break;
                case 0://tblStore
                    if (stores.Count == 0)
                    {
                        stores = connection.GetAllStores();
                    }
                    break;

                case 3://tblProvider
                    if (providers.Count == 0)
                    {
                        providers = connection.GetAllProviders();
                    }
                    break;

                case 7://tblJob
                    if (jobs.Count == 0)
                    {
                        jobs = connection.GetAllJobs();
                    }
                    break;

                case 4://tblProduct
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

                case 5://tblStockLevel
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
                case 6://tblTransaction
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
                case 10://tblProductOrder
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

                case 8://tblStoreHour
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
                case 9://tblEmployeeShift
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
                        //string employeeString = employees.Find(x => x.EmployeeID == employeeId).FullName;//Finds the record with the specified EmployeeID and assigns the name of that employee to a string.
                        string storeString = stores.Find(x => x.StoreID == storeId).StoreName;//Finds the record with the specified StoreID and assigns the name of that store to a string.
                        string jobString = jobs.Find(x => x.JobID == jobId).JobName;//Finds the record with the specified JobID and assigns the name of that job to a string.
                       // employeeShiftString= employeeShiftString.Replace("@employee", employeeString);//Assigns the name of the employee to the employeeShift string.
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
