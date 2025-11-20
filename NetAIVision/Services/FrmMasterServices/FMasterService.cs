using NetAIVision.Dal;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Services.FrmMasterServices
{
    public class FMasterService
    {
        private const string PASS = "PASS", FAIL = "FAIL";

        /// <summary>
        /// 注入sql 层
        /// </summary>
        private DalMapper dal;

        private UIForm _frm;

        public FMasterService(UIForm _frm)
        {
            string path = Path.Combine(Application.StartupPath, "dbs");
            dal = new DalMapper(path, "TestData.db");
            this._frm = _frm;
        }

        /// <summary>
        /// 插入测试数据
        /// </summary>
        /// <param name="TestData"></param>
        /// <returns></returns>
        public bool InsertTestRecord(object TestData)
        {
            return dal.InsertTestRecord(TestData);
        }

        /// <summary>
        /// 获取测试产能
        /// </summary>
        /// <param name="uIBarChar"></param>
        /// <param name="PieChart"></param>
        public void getDayHoursTestRecord(ref UIBarChart uIBarChar, ref UIPieChart PieChart)
        {
            //BarChar
            var fail = dal.GetDayTestData(FAIL);
            var pass = dal.GetDayTestData(PASS);
            //表示没有数据
            if (fail.Rows.Count == 24 && pass.Rows.Count == 24)
            {
                ///datetime_hour	production_capacity

                var x = new List<string>();
                var y1_pass = new List<int>();
                var y2_fail = new List<int>();
                for (int i = 0; i < 24; i++)
                {
                    System.DateTime dateTime = System.DateTime.Parse(pass.Rows[i]["datetime_hour"].ToString());
                    // 格式化为只包含小时部分的字符串
                    string formattedHour = dateTime.ToString("HH:mm");
                    x.Add(formattedHour);
                    // x.Add(pass.Rows[i]["datetime_hour"].ToString());
                    y1_pass.Add(int.Parse(pass.Rows[i]["production_capacity"].ToString()));
                    y2_fail.Add(int.Parse(fail.Rows[i]["production_capacity"].ToString()));
                }
                ChartsPicService.Set_Production_BarChar(x, y1_pass, y2_fail, ref uIBarChar);
            }
            else
            {
                ChartsPicService.Set_Production_BarChar(ref uIBarChar);
            }
            //Pie
            var data = dal.GetDayTestRate();
            int failCount = int.Parse(data.Rows[0]["FailCount"].ToString());
            int passCount = int.Parse(data.Rows[0]["PassCount"].ToString());
            ChartsPicService.Set_PieTestData(failCount, passCount, ref PieChart);
        }
    }
}