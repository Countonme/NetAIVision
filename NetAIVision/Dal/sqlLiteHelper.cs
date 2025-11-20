using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Dal
{
    public class SqlLiteHelper
    {
        private string connectionString;

        public SqlLiteHelper(string dbPath)
        {
            connectionString = $"Data Source={dbPath};Version=3;";
            AutoCeratedb();
        }

        /// <summary>
        /// 如果没有db 就自动建立
        /// </summary>
        private void AutoCeratedb()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    if (connection.State != System.Data.ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="sqlString">sqlString</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string sqlString)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sqlString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //FileLog.LogError("DBError", ex.Message);
                return false;
            }
        }
    }
}