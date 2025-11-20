using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Services
{
    public class FilesService
    {
        /// <summary>
        /// 检查路径是否存在没有就创建
        /// </summary>
        /// <param name="newPath"></param>
        public void CheckPath(string newPath)
        {
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
        }

        /// <summary>
        /// 打開路徑
        /// </summary>
        /// <param name="path"></param>
        public void OpenPath(string path)
        {
            try
            {
                CheckPath(path);
                // 使用系统关联的程序打开路径
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法打开路径: {ex.Message}");
            }
        }
    }
}