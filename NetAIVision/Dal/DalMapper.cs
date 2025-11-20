using NetAIVision.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Dal
{
    public class DalMapper
    {
        private CreateTableMaper CreateTable;
        private FilesService file = new FilesService();
        private SqlLiteHelper db;

        public DalMapper(string dbPath, string DbName)
        {
            file.CheckPath(dbPath);
            db = new SqlLiteHelper(dbPath + @"\" + DbName);
            CreateTable = new CreateTableMaper(db);
            initTable(db);
        }

        /// <summary>
        /// 初始化表
        /// </summary>
        /// <param name="db"></param>
        private void initTable(SqlLiteHelper db)
        {
            //创建db Table
            CreateTable.CreateTest_RecordTable();
        }

        /// <summary>
        /// 插入测试数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InsertTestRecord(object data)
        {
            try
            {
                Type type = data.GetType();
                var properties = type.GetProperties();
                string columns = string.Join(", ", properties.Select(p => p.Name));
                string values = string.Join(", ", properties.Select(p => $"'{p.GetValue(data)}'"));
                string sql = $"INSERT INTO main.test_record ({columns}) VALUES ({values});";

                return db.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取当天的每个测试小时测试数据
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public DataTable GetDayTestData(string result)
        {
            var sql = $"SELECT " +
                $" strftime('%Y-%m-%d %H:00:00', hours_of_day.hour) AS datetime_hour," +
                $" COALESCE(COUNT(main.test_record.test_datetime), 0) AS production_capacity " +
                $" FROM " +
                $" (SELECT " +
                $" datetime(CURRENT_DATE, '+' || (a + b * 10) || ' HOURS') AS hour" +
                $" FROM" +
                $" (SELECT 0 AS a UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9) AS a" +
                $" CROSS JOIN" +
                $" (SELECT 0 AS b UNION SELECT 1 UNION SELECT 2) AS b) AS hours_of_day" +
                $" LEFT JOIN " +
                $" main.test_record ON " +
                $" strftime('%Y-%m-%d',main.test_record.test_datetime)='{DateTime.Now.ToString("yyyy-MM-dd")}'" +
                $" and  main.test_record.test_result='{result}'" +
                $" and  strftime('%Y-%m-%d %H:00:00', hours_of_day.hour) = strftime('%Y-%m-%d %H:00:00', main.test_record.test_datetime)" +
                $" where  strftime('%Y-%m-%d', hours_of_day.hour)='{DateTime.Now.ToString("yyyy-MM-dd")}'" +
                $" GROUP BY " +
                $" hours_of_day.hour" +
                $" ORDER BY " +
                $" hours_of_day.hour;";
            return db.ExecuteQuery(sql);
        }

        /// <summary>
        /// 获取当天测试良品不良品产能
        /// </summary>
        /// <returns></returns>
        public DataTable GetDayTestRate()
        {
            var sql = $"SELECT" +
                $"  COALESCE(SUM(CASE WHEN test_result = 'PASS' THEN 1 ELSE 0 END), 0) AS PassCount," +
                $"  COALESCE(SUM(CASE WHEN test_result = 'FAIL' THEN 1 ELSE 0 END), 0) AS FailCount" +
                $"  FROM" +
                $"  main.test_record" +
                $"  WHERE" +
                $"  strftime('%Y-%m-%d', main.test_record.test_datetime) = '{DateTime.Now.ToString("yyyy-MM-dd")}'";
            return db.ExecuteQuery(sql);
        }
    }
}