using Microsoft.Data.SqlClient;
using RetailApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace RetailApplication.Controller
{
    public class StorageManager
    {
        private SqlConnection connection;
        public StorageManager(string connectionString)
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Invalid connection string or connection already open");
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
                if (e.Message.Contains("attach an auto-named database"))
                {
                    Console.WriteLine("Fix: Database is already attached OR file is in use.");
                    Console.WriteLine("Try this:");
                    Console.WriteLine("1. Remove AttachDbFilename from connection string");
                    Console.WriteLine("2. Use Initial Catalog instead");
                    Console.WriteLine("3. Or delete/rename duplicate DB in SQL Server");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
            }
        }


        public string GetUserPassword(int empId)
        {
            string password="null";
            string query = "SELECT Password FROM scheduling.tblEmployee WHERE EmpID=@empid";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@empid",empId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            password = reader.GetString(0);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine();
                password = "Error";
                throw;
            }
            return password;
        }

        public byte GetUserPermission(int empId)
        {
            byte perLevel=0;
            string query = "SELECT PermissionLevel FROM scheduling.tblPermissionLevel P, scheduling.tblEmployee E WHERE EmpID=@empid AND P.LevelID=E.LevelID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@empid", empId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            perLevel = reader.GetByte(0);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine();
                perLevel = 0;
                throw;
            }

            return perLevel;
        }



        //The following methods get all the records from a specific table and return them as list of the corresponding Object.
        //These methods use hard-coded string literals to query the database.
        #region GetRecordMethods
        public List<Store> GetAllStores() 
        {
            List<Store> allStores = new List<Store> { };

            string query = "SELECT * FROM scheduling.tblStore;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Store store = new Store(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),reader.GetDecimal(3));
                            allStores.Add(store);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Stores: {ex.Message}");
            }



            return allStores;
        }


        public List<PermissionLevel> GetPermissionLevels()
        {
            List<PermissionLevel> perLevelList = new List<PermissionLevel> { };
            string query = "SELECT * FROM scheduling.tblPermissionLevel ORDER BY PermissionLevel desc;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PermissionLevel perlevel = new PermissionLevel(reader.GetInt32(0), reader.GetString(1), reader.GetByte(2));
                            perLevelList.Add(perlevel);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Permission Levels: {ex.Message}");
            }
            return perLevelList;
        }


        public List<Job> GetAllJobs()
        {
            List<Job> allJobs = new List<Job> { };

            string query = "SELECT * FROM scheduling.tblJob;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Job job = new Job(reader.GetInt16(0), reader.GetString(1), reader.GetDecimal(2));
                            allJobs.Add(job);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Jobs: {ex.Message}");
            }



            return allJobs;
        }

        public List<Employee> GetAllEmployees()
        {
            List<Employee> allEmployees = new List<Employee> { };

            string query = "SELECT * FROM scheduling.tblEmployee;";//This string is hardcoded since it is not used anywhere else.
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee employee = new Employee(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt16(3), reader.GetString(4));
                            allEmployees.Add(employee);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Employees: {ex.Message}");
            }



            return allEmployees;
        }

        public List<Product> GetAllProducts()
        {
            List<Product> allProducts = [];

            string query = "SELECT * FROM sales.tblProduct;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetDecimal(3), reader.GetDecimal(4), reader.GetString(5));
                            allProducts.Add(product);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Products: {ex.Message}");
            }



            return allProducts;
        }
        public List<Provider> GetAllProviders()
        {
            List<Provider> allProviders = [];

            string query = "SELECT * FROM sales.tblProvider;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Provider provider = new Provider(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                            allProviders.Add(provider);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Providers: {ex.Message}");
            }



            return allProviders;
        }

        public List<StockLevel> GetAllStocks()
        {
            List<StockLevel> allStocks = [];

            string query = "SELECT * FROM sales.tblStockLevel;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StockLevel stock = new StockLevel(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2),reader.GetInt32(3));
                            allStocks.Add(stock);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Stocks: {ex.Message}");
            }


            return allStocks;
        }

        public List<Transaction> GetAllTransactions()
        {
            List<Transaction> allTransactions = [];

            string query = "SELECT * FROM sales.tblTransaction;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Transaction transaction = new Transaction(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2),reader.GetDateTime(3),reader.GetInt16(4));
                            allTransactions.Add(transaction);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Transactions: {ex.Message}");
            }


            return allTransactions;
        }

        public List<EmployeeShift> GetAllEmployeeShifts()
        {
            List<EmployeeShift> allEmpShifts = [];

            string query = "SELECT * FROM sscheduling.tblEmpShift;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeShift empShift = new EmployeeShift(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader.GetString(3)),TimeOnly.FromDateTime(reader.GetDateTime(4)),TimeOnly.FromDateTime(reader.GetDateTime(5)),reader.GetInt32(6));
                            allEmpShifts.Add(empShift);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Employee Shifts: {ex.Message}");
            }


            return allEmpShifts;
        }

        public List<ProductOrder> GetAllProductOrders()
        {
            List<ProductOrder> allProductOrders = [];

            string query = "SELECT * FROM sales.tblProductOrder;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductOrder productOrder = new ProductOrder(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2),  reader.GetInt16(3),reader.GetDateTime(4),reader.GetBoolean(5));
                            allProductOrders.Add(productOrder);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Product Orders: {ex.Message}");
            }


            return allProductOrders;
        }

        public List<StoreHour> GetAllStoreHours()
        {
            List<StoreHour> allStoreHours=[];

            string query = "SELECT * FROM scheduling.tblStoreHour;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StoreHour storeHour = new StoreHour(reader.GetInt32(0), reader.GetInt32(1), (DayOfWeek)Enum.Parse(typeof(DayOfWeek),reader.GetString(3)), TimeOnly.FromDateTime(reader.GetDateTime(4)), TimeOnly.FromDateTime(reader.GetDateTime(5)));
                            allStoreHours.Add(storeHour);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Store Hours: {ex.Message}");
            }


            return allStoreHours;
        }

        #endregion
        ///End of GetRecords methods
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //This method gets a report using the provided query and parameters, returning it as a list of strings representing the records.
        public List<string[]> ReportHandler(string query, string parameters)
        {
            List<string[]> output=[];
        TryExecute:
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (parameters.Length == 0)
                    {
                        cmd.CommandText = cmd.CommandText.Replace("@", " ");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@", parameters);//Needs fixing when parameterisation added
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] record = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                record[i] = reader[i].ToString();
                            }
                            output.Add(record);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"There was a database error while retrieving the report: {e.Message}");
                if (e.Message.Contains("Execution Timeout Expired"))
                {
                    Console.WriteLine("Retrying Execution..."); 
                    goto TryExecute;
                }
                throw;
            }

            return output;
        }


        //This method deletes the record with the provided ID from the provided table. It is used for all the tables in the database.
        public int DeleteRecordsByID(string table, string IDColName, List<int> IDs)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand($"DELETE FROM {table} WHERE {IDColName} IN (", connection))//DOES NOT WORK DUE TO INCORRECT COMMAND TEXT/PARAMETERS.
                {
                    int i = 0;
                    foreach (var Id in IDs)
                    {
                        cmd.Parameters.AddWithValue("@ID"+i,Id);
                        cmd.CommandText = cmd.CommandText.Insert(cmd.CommandText.Length,"@ID"+i);
                        if (i<IDs.Count-1)
                        {
                            cmd.CommandText = cmd.CommandText.Insert(cmd.CommandText.Length,",");
                        }
                    }
                    cmd.CommandText = cmd.CommandText.Insert(cmd.CommandText.Length, ");");
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Database error deleting record: {e.Message}");
                throw;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error deleting record: {e.Message}");
                throw;
            }

           
        }

        //This method
        public int UpdateRecordByID(string table, string IDColName, int ID,string colName,string value)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand($"UPDATE {table} SET {colName} = @value WHERE {IDColName} = @ID", connection))//There is no risk of sql injection here since the table and column name strings are not obtained from the user.
                {
                    cmd.Parameters.AddWithValue("@value", value);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Database Error updating record: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error updating record: {e.Message}");
                throw;
            }
            
        }

        //This method handles the insertion of new records to the database. It takes a list of objects and inserts them into the supplied table.
        public int InsertRecords<T>(List<T> records, string table, string[]columnNames)
        {
            string columns="";
            Type insertClass = typeof(T);
            PropertyInfo[] classInfo = insertClass.GetProperties();
            SqlParameter[,] recordParameters= new SqlParameter[records.Count, classInfo.Length-1]; 
            //needs to format each record correctly, using the values of the fields.
            //organisation of parameters?
            for (int i=1; i< columnNames.Length; i++)//Formats the column names for the insert statements.
            {
               columns = columns.Insert(columns.Length, columnNames[i]);
                if (i<columnNames.Length-1)
                { 
                    columns = columns.Insert(columns.Length, ",");
                }
            }
            for (int i = 0; i < records.Count; i++)//Gets each field of the record and assigns it to an SqlParameter.
            {
                for(int j=1;j<classInfo.Length;j++)
                {
                   recordParameters[i,j-1]= new SqlParameter($"@{i}i{j}",classInfo[j].GetValue(records[i]));
                }

            }
            StringBuilder inputString = new StringBuilder($"INSERT INTO {table} ({columns}) VALUES ");
            for (int i = 0; i < records.Count; i++)//Gets each field of the record and assigns it to an SqlParameter.
            {
                inputString.Append('(');
                for (int j = 1; j < classInfo.Length; j++)
                {
                   inputString.Append($"@{i}i{j}");
                    if (j<classInfo.Length-1)
                    {
                        inputString.Append(',');
                    }
                }
                inputString.Append(')');
                if (i<records.Count-1)
                {
                    inputString.Append(',');
                }

            }
            inputString.Append(';');
            //create a new list or multidimensional array of record fields, then use the below foreach to insert them.
            try
            {
                using (SqlCommand cmd=new SqlCommand(inputString.ToString(),connection))//Not vunerable to SQL injection since the input string is not obtained from the user.
                {
                        for (int i = 0; i < records.Count; i++)
                        {
                            for (int j = 0; j < classInfo.Length-1; j++)
                            {
                                cmd.Parameters.Add(recordParameters[i,j]);//Adds a parameter to the list
                            }
                        }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                Console.WriteLine($"Database Error inserting record/s: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error inserting record/s: {e.Message}");
                throw;
            }
        }

    }
}
