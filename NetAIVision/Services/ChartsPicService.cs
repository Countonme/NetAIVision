using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Services
{
    public class ChartsPicService
    {
        /// <summary>
        /// 設計狀況
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="failCount"></param>
        /// <param name="passCount"></param>
        /// <param name="PieChart"></param>
        public static void Set_TestData(int totalCount, int failCount, int passCount, ref UIPieChart PieChart)
        {
            var option = new UIPieOption();
            //设置Title
            option.Title = new UITitle();
            option.Title.Text = $"總數{totalCount}";
            option.Title.SubText = "";
            option.Title.Left = UILeftAlignment.Center;

            //设置ToolTip
            option.ToolTip.Visible = true;
            //设置Legend
            option.Legend = new UILegend();
            option.Legend.Orient = UIOrient.Vertical;
            option.Legend.Top = UITopAlignment.Top;
            option.Legend.Left = UILeftAlignment.Left;
            option.Legend.AddData($"待測", Color.Blue);
            //option.Legend.AddData("Now數量", Color.SlateBlue);
            option.Legend.AddData($"不良品", Color.Red);
            option.Legend.AddData($"良品", Color.SpringGreen);

            //设置Series
            var series = new UIPieSeries();
            series.Name = "StarCount";
            series.Center = new UICenter(50, 55);
            series.Radius = 70;
            series.Label.Show = true;

            //增加数据
            series.AddData("待測", totalCount - passCount - failCount, Color.Blue);
            //series.AddData("Now數量", 21, Color.SlateBlue);
            series.AddData("不良品", failCount, Color.Red);
            series.AddData("良品", passCount, Color.SpringGreen);
            //增加Series
            option.Series.Clear();
            option.Series.Add(series);

            //显示数据小数位数
            // option.DecimalPlaces = 1;

            //设置Option
            PieChart.SetOption(option);
        }

        public static void Set_PieTestData(ref UIPieChart PieChart)
        {
            var option = new UIPieOption();
            //设置Title
            option.Title = new UITitle();
            option.Title.Text = $"總數 0";
            option.Title.SubText = "";
            option.Title.Left = UILeftAlignment.Center;

            //设置ToolTip
            option.ToolTip.Visible = true;
            //设置Legend
            option.Legend = new UILegend();
            option.Legend.Orient = UIOrient.Vertical;
            option.Legend.Top = UITopAlignment.Top;
            option.Legend.Left = UILeftAlignment.Left;
            option.Legend.AddData($"不良品", Color.Red);
            option.Legend.AddData($"良品", Color.SpringGreen);
            //设置Series
            var series = new UIPieSeries();
            series.Name = "StarCount";
            series.Center = new UICenter(50, 55);
            series.Radius = 70;
            series.Label.Show = true;

            series.AddData("不良品", 0, Color.Red);
            series.AddData("良品", 0, Color.SpringGreen);
            //增加Series
            option.Series.Clear();
            option.Series.Add(series);

            //显示数据小数位数
            // option.DecimalPlaces = 1;

            //设置Option
            PieChart.SetOption(option);
        }

        public static void Set_PieTestData(int failCount, int passCount, ref UIPieChart PieChart)
        {
            var option = new UIPieOption();
            //设置Title
            option.Title = new UITitle();
            option.Title.Text = $"總數 {failCount + passCount}";
            option.Title.SubText = "";
            option.Title.Left = UILeftAlignment.Center;

            //设置ToolTip
            option.ToolTip.Visible = true;
            //设置Legend
            option.Legend = new UILegend();
            option.Legend.Orient = UIOrient.Vertical;
            option.Legend.Top = UITopAlignment.Top;
            option.Legend.Left = UILeftAlignment.Left;
            option.Legend.AddData($"不良品", Color.Red);
            option.Legend.AddData($"良品", Color.SpringGreen);
            //设置Series
            var series = new UIPieSeries();
            series.Name = "StarCount";
            series.Center = new UICenter(50, 55);
            series.Radius = 70;
            series.Label.Show = true;

            series.AddData("不良品", failCount, Color.Red);
            series.AddData("良品", passCount, Color.SpringGreen);
            //增加Series
            option.Series.Clear();
            option.Series.Add(series);

            //显示数据小数位数
            // option.DecimalPlaces = 1;

            //设置Option
            PieChart.SetOption(option);
        }

        public static void Set_Production_BarChar(ref UIBarChart BarChart)
        {
            UIBarOption option = new UIBarOption();
            option.Title = new UITitle();
            option.Title.Text = $"{DateTime.Now.ToString("yyyy-MM-dd")}";
            option.Title.SubText = "";
            option.XAxis.ShowGridLine = true;

            //设置Legend
            option.Legend = new UILegend();
            option.Legend.Orient = UIOrient.Horizontal;
            option.Legend.Top = UITopAlignment.Top;
            option.Legend.Left = UILeftAlignment.Left;
            option.Legend.AddData("良品", Color.SpringGreen);
            option.Legend.AddData("不良品", Color.Red);

            var series = new UIBarSeries();
            series.Name = "良品";
            for (int i = 0; i < 24; i++)
            {
                series.AddData(0, Color.SpringGreen);
            }

            //数据显示小数位数
            //series.DecimalPlaces = 1;
            option.Series.Add(series);

            series = new UIBarSeries();
            series.Name = "不良品";
            for (int i = 0; i < 24; i++)
            {
                series.AddData(0, Color.Red);
            }
            option.Series.Add(series);

            //轉換時間
            for (int i = 0; i < 24; i++)
            {
                if (i < 10)
                {
                    option.XAxis.Data.Add($"0{i}:00");
                }
                else
                {
                    option.XAxis.Data.Add($"{i}:00");
                }
            }

            option.ToolTip.Visible = true;
            option.YAxis.Scale = true;

            option.XAxis.Name = "日期";
            option.XAxis.AxisLabel.Angle = 60;//(0° ~ 90°)

            option.YAxis.Name = "数值";

            //坐标轴显示小数位数
            //option.YAxis.AxisLabel.DecimalPlaces = 1;

            //option.YAxisScaleLines.Add(new UIScaleLine() { Color = Color.Red, Name = "上限", Value = 12 });
            //option.YAxisScaleLines.Add(new UIScaleLine() { Color = Color.Gold, Name = "下限", Value = -20 });

            //option.ToolTip.AxisPointer.Type = UIAxisPointerType.Shadow;

            option.ShowValue = true;
            // option.YAxis.ShowGridLine = false; 网格线
            BarChart.SetOption(option);
        }

        public static void Set_Production_BarChar(List<string> x, List<int> y1_pass, List<int> y2_Fail, ref UIBarChart BarChart)
        {
            UIBarOption option = new UIBarOption();
            option.Title = new UITitle();
            option.Title.Text = $"{DateTime.Now.ToString("yyyy-MM-dd")}";
            option.Title.SubText = "";

            //设置Legend
            option.Legend = new UILegend();
            option.Legend.Orient = UIOrient.Horizontal;
            option.Legend.Top = UITopAlignment.Top;
            option.Legend.Left = UILeftAlignment.Left;
            option.Legend.AddData("良品", Color.SpringGreen);
            option.Legend.AddData("不良品", Color.Red);

            var series = new UIBarSeries();
            series.Name = "良品";
            for (int i = 0; i < y1_pass.Count; i++)
            {
                series.AddData(y1_pass[i], Color.SpringGreen);
            }

            //数据显示小数位数
            //series.DecimalPlaces = 1;
            option.Series.Add(series);

            series = new UIBarSeries();
            series.Name = "不良品";
            for (int i = 0; i < y2_Fail.Count; i++)
            {
                series.AddData(y2_Fail[i], Color.Red);
            }
            option.Series.Add(series);

            //轉換時間
            for (int i = 0; i < x.Count; i++)
            {
                option.XAxis.Data.Add($"{x[i]}");
            }

            option.ToolTip.Visible = true;
            option.YAxis.Scale = true;

            option.XAxis.Name = "日期";
            option.XAxis.AxisLabel.Angle = 60;//(0° ~ 90°)
            option.YAxis.Name = "数值";

            //坐标轴显示小数位数
            //option.YAxis.AxisLabel.DecimalPlaces = 1;
            //option.YAxisScaleLines.Add(new UIScaleLine() { Color = Color.Red, Name = "上限", Value = 12 });
            //option.YAxisScaleLines.Add(new UIScaleLine() { Color = Color.Gold, Name = "下限", Value = -20 });
            //option.ToolTip.AxisPointer.Type = UIAxisPointerType.Shadow;
            option.ShowValue = true;
            BarChart.SetOption(option);
        }
    }
}