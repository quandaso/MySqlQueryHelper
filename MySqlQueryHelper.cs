using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MySqlHelper
{
    /// <summary>
    /// Base class for MYSQL Query
    /// </summary>
    public class MySqlQueryHelper
    {
        private MySqlConnection Connection { get; set; }
        private string connectionString = "Server=localhost;Port=3306;Database=test;Uid=root;Pwd=;charset=utf8;Convert Zero Datetime=True";

        public MySqlQueryHelper()
        {
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public MySqlQueryHelper(string connectionString)
        {
            this.connectionString = connectionString;
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public MySqlQueryHelper(MySqlConnection conn)
        {
            this.Connection = conn;
        }
        

        /// <summary>
        /// Performs select statements
        /// </summary>
        /// <example>
        /// var db = new MysqlConnection();
        /// db.select("SELECT * FROM `users` WHERE `id`=@id", new {id = 1})
        /// </example>
        /// <returns>
        /// A list which contains select result
        /// </returns>
        private List<Dictionary<string, object>> ExecuteQuery(string sql, object data = null)
        {

            MySqlCommand command = new MySqlCommand(sql, Connection);

            if (data != null)
            {
                foreach (var prop in data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(data, null));
                }
            }

            MySqlDataReader dataReader = command.ExecuteReader();
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            while (dataReader.Read())
            {
                var dict = new Dictionary<string, object>();

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    string field = dataReader.GetName(i);
                    dict.Add(field, dataReader[field]);
                }

                result.Add(dict);
            }

            dataReader.Close();
            return result;
        }

        /// <summary>
        ///  Execute INSERT, UPDATE OR DELETE query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>The number of rows affected</returns>
        private int ExecuteNoneQuery(String sql, object data = null)
        {

            MySqlCommand command = new MySqlCommand(sql, Connection);

            if (data != null)
            {
                foreach (var prop in data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(data, null));
                }
            }

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Short hand for MysqlConnector::ExecuteQuery 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>The number of rows affected</returns>
        public List<Dictionary<string, object>> Select(String sql, object data = null)
        {
            sql = sql.Trim();
            if (!Regex.IsMatch(sql, "^SELECT", RegexOptions.IgnoreCase))
            {
                throw new Exception("SQL query must be SELECT type");
            }

            return this.ExecuteQuery(sql, data);
        }

        /// <summary>
        /// Performs UPDATE statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>The number of rows affected</returns>
        public int Update(String sql, object data = null)
        {
            sql = sql.Trim();
            if (!Regex.IsMatch(sql, "^UPDATE", RegexOptions.IgnoreCase))
            {
                throw new Exception("SQL query must be  UPDATE type");
            }

            return this.ExecuteNoneQuery(sql, data);
        }

        /// <summary>
        /// Performs DELETE statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>The number of rows affected</returns>
        public int Delete(String sql, object data = null)
        {
            sql = sql.Trim();
            if (!Regex.IsMatch(sql, "^DELETE", RegexOptions.IgnoreCase))
            {
                throw new Exception("SQL query must be DELETE type");
            }

            return this.ExecuteNoneQuery(sql, data);
        }

        /// <summary>
        /// Performs INSERT statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>The number of rows affected</returns>
        public int Insert(String sql, object data = null)
        {
            sql = sql.Trim();
            if (!Regex.IsMatch(sql, "^INSERT", RegexOptions.IgnoreCase))
            {
                throw new Exception("SQL query must be INSERT type");
            }

            return this.ExecuteNoneQuery(sql, data);
        }

        /// <summary>
        /// Performs SELECT COUNT statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>Count result</returns>
        public int Count(String sql, object data = null)
        {
            sql = sql.Trim();
            if (!Regex.IsMatch(sql, @"^SELECT(\s+)COUNT\(.+\)", RegexOptions.IgnoreCase))
            {
                throw new Exception("SQL query must be SELECT COUNT... type");
            }

            var CountResult = this.ExecuteQuery(sql, data);
            String FirstKey = CountResult[0].Keys.First();

            return Int32.Parse(CountResult[0][FirstKey].ToString());
        }

        /// <summary>
        /// Performs SELECT|DELETE|INSERT|UPDATE SQL statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>
        /// - A list of records if sql type is SELECT
        /// - Count result if sql type is SELECT COUNT
        /// - Number of rows affected if sql type is INSERT,UPDATE or DELETE
        /// </returns>
        public object Query(string sql, object data = null)
        {
            sql = sql.Trim();

            if (Regex.IsMatch(sql, @"^SELECT(\s+)COUNT\(.+\)", RegexOptions.IgnoreCase))
            {
                var CountResult = this.ExecuteQuery(sql, data);
                String FirstKey = CountResult[0].Keys.First();
                return Int32.Parse(CountResult[0][FirstKey].ToString());

            }
            else if (Regex.IsMatch(sql, "^SELECT", RegexOptions.IgnoreCase))
            {
                return this.ExecuteQuery(sql, data);
            }
            else if (Regex.IsMatch(sql, "^(INSERT|UPDATE|DELETE)", RegexOptions.IgnoreCase))
            {
                return this.ExecuteNoneQuery(sql, data);
            }

            throw new Exception("This method only supports SELECT,UPDATE,INSERT or DELETE SQL type");
        }

        /// <summary>
        /// Closes mysql connection
        /// </summary>
        /// <returns></returns>
        public bool CloseConnection()
        {
            try
            {
                Connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }

}