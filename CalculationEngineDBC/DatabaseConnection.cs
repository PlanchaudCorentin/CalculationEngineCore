using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CalculationEngineDBC
{
    public class DatabaseConnection
    {
        private DatabaseConnection()
        {
            
        }

        private MySqlConnection connection = null;
        
        private MySqlCommand cmd = new MySqlCommand();

        private static DatabaseConnection _instance = null;

        public static DatabaseConnection Instance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseConnection();
            }

            return _instance;
        }

        public bool connect(string hostname, string port, string database, string user, string password)
        {
            if (connection == null)
            {
                string cString = string.Format("Server={0}; database={1}; port={2}; UID={3}; password={4}",
                    hostname, database, port, user, password);
                connection = new MySqlConnection(cString);
                connection.Open();
            }
            return true;
        }

        public void executeStoredProcedure(string name, string[] args, string[] values)
        {
            cmd.Connection = connection;
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;
            
            for (int i = 0; i < args.Length; i++)
            {
                cmd.Parameters.AddWithValue(String.Concat("@", args[i]), values[i]);
                cmd.Parameters[String.Concat("@", args[i])].Direction = ParameterDirection.Input;
            }

            try
            {
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Close()
        {
            connection.Close();
        }
        
        public MySqlConnection Connection => connection;
    }
}