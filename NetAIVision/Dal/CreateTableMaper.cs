using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Dal
{
    public class CreateTableMaper
    {
        private SqlLiteHelper _db = null;

        public CreateTableMaper(SqlLiteHelper db)
        {
            _db = db;
        }

        /// <summary>
        /// 创建测试记录表
        /// </summary>
        public void CreateTest_RecordTable()
        {
            // 创建一个名为 "test_record" 的表格，仅在不存在时创建
            string createTableQuery = $"CREATE TABLE IF NOT EXISTS main.test_record (" +

                $" \"serial_number\" text(60)," +
                $" \"test_result\" text(60)," +
                $" \"version\" text(60)," +
                $" \"test_datetime\" text(60)" +

                $" );";

            // 设置外键约束
            // string enableForeignKeyQuery = "PRAGMA foreign_keys = true;";

            // 执行查询
            // _db.ExecuteNonQuery(enableForeignKeyQuery);
            _db.ExecuteNonQuery(createTableQuery);
        }
    }
}