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

            string query = "SELECT * FROM scheduling.tblEmployee;";
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

        public List<object[]> QryProductSales()
        {
            List<object[]> reportRecords = new List<object[]> { };
            string query = "SELECT COALESCE(P.ProductName,Total)AS Product, COALESCE(S.StoreName, Subtotal) AS Store, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblProduct P, tblTransaction T RIGHT JOIN tblStore S ON T.StoreID=S.StoreID WHERE P.ProductID=T.ProductID GROUP BY ROLLUP(S.StoreName,P.ProductName) ORDER BY 1,2;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] record = { reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetDouble(3) };
                            reportRecords.Add(record);
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
                Console.WriteLine($"Error retrieving Product Sales Report: {ex.Message}");
            }
            return reportRecords;
        }


        public List<object[]> QryEmployeeShifts()
        {
            List<object[]> reportRecords = new List<object[]> { };
            string query = "SELECT (E.FirstName+E.LastName) AS Name, S.StoreName AS Store, I.StartTime AS Start Time, I.EndTime AS End Time, I.Weekday AS Day, J.JobName AS Job FROM tblEmployee E, tblStore S, tblEmpShift I, tblJob J WHERE E.EmpID=I.EmpID AND S.StoreID=I.StoreID AND J.JobID=I.JobID ORDER BY 2,1,6,5,3,4;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] record = { reader.GetString(0), reader.GetString(1), reader.GetDateTime(2), reader.GetDateTime(3), reader.GetString(4),reader.GetString(5)};
                            reportRecords.Add(record);
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
                Console.WriteLine($"Error retrieving Employee Shifts Report: {ex.Message}");
            }


            return reportRecords;
        }

        public List<object[]> QryProductOrders()
        {
            List<object[]> reportRecords = new List<object[]> { };
            string query = "SELECT P.ProductName AS Product, S.StoreName AS Store, O.Quantity, P.SupplyPrice*O.Quantity AS Price FROM tblProduct P, tblStore S, tblProductOrder O WHERE P.ProductID=O.ProductID AND S.StoreID=O.StoreID AND DATEDIFF(dy,GETDATE(),O.Date)<30 ORDER BY O.Date;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] record = { reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(3)};
                            reportRecords.Add(record);
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
                Console.WriteLine($"Error retrieving Product Orders Report: {ex.Message}");
            }


            return reportRecords;
        }

        public List<object[]> QryStoreSales()
        {
            List<object[]> reportRecords = new List<object[]> { };
            string query = "SELECT COALESCE(S.StoreName,Grand Total)AS Store, COALESCE(P.ProductName, Total Sales) AS Product, SUM(T.Quantity)AS QUANTITY, SUM(T.Quantity*P.CustomerPrice)AS Revenue FROM tblStore S, tblTransaction T RIGHT JOIN tblProduct P ON T.ProductID=P.ProductID WHERE S.StoreID=T.StoreID GROUP BY ROLLUP(P.ProductName,S.StoreName) ORDER BY 1,2;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] record = { reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(3) };
                            reportRecords.Add(record);
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
                Console.WriteLine($"Error retrieving Store Sales Report: {ex.Message}");
            }


            return reportRecords;
        }

        public List<object[]> QryBestProducts()
        {
            List<object[]> reportRecords = new List<object[]> { };
            string query = "SELECT TOP 50 P.ProductName AS Product, SUM(T.Quantity) AS Total Sales, SUM(T.Quantity*P.CustomerPrice) AS Total Revenue, SUM((T.Quantity*P.CustomerPrice)-(T.Quantity*P.SupplyPrice)) AS Profit FROM tblProduct P, tblStore S, tblTransaction T WHERE S.StoreID=T.StoreID AND P.ProductID=T.ProductID GROUP BY P.ProductID ORDER BY 2 desc, 4 desc;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] record = { reader.GetString(0), reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(3) };
                            reportRecords.Add(record);
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
                Console.WriteLine($"Error retrieving Best Products Report: {ex.Message}");
            }


            return reportRecords;
        }

        public List<object[]> QryEmployeePermissions()
        {
            List<object[]> reportRecords = new List<object[]> { };
            string query = "SELECT E.EmpID AS Employee Number, E.FirstName+ ' '+ E.LastName AS Name, l.PermissionName AS Permission Level FROM tblEmployee E, tblPermissionLevel l WHERE E.LevelID=l.LevelID ORDER BY l.PermissionLevel desc, 2;";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] record = { reader.GetInt32(0), reader.GetString(1), reader.GetString(2) };
                            reportRecords.Add(record);
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
                Console.WriteLine($"Error retrieving Employee Permissions Report: {ex.Message}");
            }


            return reportRecords;
        }

        /*public List<string[]> ParameterisationTemplate(object[] parameters)
        {
            List<string[]> output=new List<string[]>();
            string query = "sample_query";
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
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@", parameters[i]);
                        }
                    }
                      
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] record = new string[numberofcolumns];
                            record[0]= reader.GetString(0);
                            record[1] = reader.GetString(1);
                            ...
                            record[^1] = reader.GetString(record.Length - 1);


                            output.Add(record);
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine(sampleerrormessage);
                
                throw;
            }

            return output;
        }*/


    }
}
