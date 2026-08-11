using Microsoft.Data.SqlClient;
using RetailApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Console.WriteLine("Connection Successful");
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
                            PermissionLevel perlevel = new PermissionLevel(reader.GetInt16(0), reader.GetString(1), reader.GetInt16(2));
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
                            Store store = new Store(reader.GetInt16(0), reader.GetString(1), reader.GetString(2),reader.GetDecimal(3));
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
            List<Product> allProducts = new List<Product> { };

            string query = "SELECT * FROM sales.tblProduct;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product(reader.GetInt32(0), reader.GetInt16(1), reader.GetString(2), reader.GetDecimal(3), reader.GetDecimal(4), reader.GetString(5));
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
            List<Provider> allProviders = new List<Provider> { };

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
            List<StockLevel> allStocks = new List<StockLevel> { };

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
            List<Transaction> allTransactions = new List<Transaction> { };

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
            List<EmployeeShift> allEmpShifts = new List<EmployeeShift> { };

            string query = "SELECT * FROM sscheduling.tblEmpShift;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeShift empShift = new EmployeeShift(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2),reader.GetDateTime(3).DayOfWeek,TimeOnly.FromDateTime(reader.GetDateTime(4)),TimeOnly.FromDateTime(reader.GetDateTime(5)),reader.GetInt32(6));
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
            List<ProductOrder> allProductOrders = new List<ProductOrder> { };

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
            List<StoreHour> allStoreHours = new List<StoreHour> { };

            string query = "SELECT * FROM scheduling.tblStoreHour;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StoreHour storeHour = new StoreHour(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), TimeOnly.FromDateTime(reader.GetDateTime(3)), TimeOnly.FromDateTime(reader.GetDateTime(4)));
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
            List<string[]> output=new List<string[]>();
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (parameters.Length==0)
                    {
                        cmd.Parameters.AddWithValue("@"," ");
                    }
                    else
                    {
                        
                        cmd.Parameters.AddWithValue("@", parameters);
                    }
                      
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] record = new string[reader.FieldCount];
                            for(int i=0;i<reader.FieldCount;i++)
                            {
                                record[i]=reader[i].ToString();
                            }
                            output.Add(record);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("");
                
                throw;
            }

            return output;
        }


        //This method deletes the record with the provided ID from the provided table. It is used for all the tables in the database.
        public int DeleteRecordByID(string table, string IDColName, string ID)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM @table WHERE @IDColName = @ID", connection))
                {
                    cmd.Parameters.AddWithValue("@table", table);
                    cmd.Parameters.AddWithValue("@IDColName", IDColName);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Error deleting record: {e.Message}");
                throw;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error deleting record: {e.Message}");
                throw;
            }

           
        }

        //This method
        public int UpdateRecordByID(string table, string IDColName, string ID,string colName,string value)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand($"UPDATE @table SET @colName = @value WHERE @IDColName = @ID", connection))
                {
                    cmd.Parameters.AddWithValue("@table", table);
                    cmd.Parameters.AddWithValue("@colName", colName);
                    cmd.Parameters.AddWithValue("@value", value);
                    cmd.Parameters.AddWithValue("@IDColName", IDColName);
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
        public int InsertRecords<T>(List<T> records, string table)
        {
            try
            {
                using (SqlCommand cmd=new SqlCommand($"INSERT INTO @table VALUES @values;",connection))
                {
                    foreach (var record in records)
                    {
                        cmd.Parameters.AddWithValue("@values", record + ",\n@values");//This calls the override ToString in the class, formatting the values like an SQL insert statement.
                        //The above line replaces @values with the string representation of the object, followed by a comma, new line, and an @values for the next parameter.
                    }
                    cmd.Parameters.Remove(",\n@values");//Removes the parameter and statement terminator from the end of the query.
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
