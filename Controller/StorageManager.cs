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
            string query = "SELECT * FROM tblPermissionLevel ORDER BY PermissionLevel desc;";
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
                Console.WriteLine($"Error retrieving brands: {ex.Message}");
            }
            return perLevelList;
        }

        public string GetUserPassword(int empId)
        {
            string password="null";
            string query = "SELECT Password FROM tblEmployee WHERE EmpID=@empid";
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
            string query = "SELECT PermissionLevel FROM tblPermissionLevel P, tblEmployee E WHERE EmpID=@empid AND P.LevelID=E.LevelID";
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
