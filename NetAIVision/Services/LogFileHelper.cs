using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Services
{
    public static class LogFileHelper
    {
        private static readonly string LogFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly int RetainDays = 90; // 保留天数

        /// <summary>
        /// 保存日志到文件，按年月日时命名，并清理过期日志
        /// </summary>
        public static void SaveLog(string message)
        {
            try
            {
                // 创建日志目录
                if (!Directory.Exists(LogFolder))
                {
                    Directory.CreateDirectory(LogFolder);
                }

                // 文件名按年月日时命名
                string fileName = $"Log_{DateTime.Now:yyyyMMdd_HH}.txt";
                string filePath = Path.Combine(LogFolder, fileName);

                // 日志行内容加时间
                string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}{Environment.NewLine}";

                // 写入文件（追加模式）
                File.AppendAllText(filePath, logLine);

                // 清理过期日志
                DeleteOldLogs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写日志失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除超过保留天数的日志文件
        /// </summary>
        private static void DeleteOldLogs()
        {
            try
            {
                var files = Directory.GetFiles(LogFolder, "Log_*.txt");
                DateTime threshold = DateTime.Now.AddDays(-RetainDays);

                foreach (var file in files)
                {
                    DateTime lastWrite = File.GetLastWriteTime(file);
                    if (lastWrite < threshold)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"清理日志失败: {ex.Message}");
            }
        }
    }
}