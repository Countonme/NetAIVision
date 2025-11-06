using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetAIVision.Services
{
    public class FrameSaver
    {
        private readonly string _saveDirectory;
        private int _frameIndex = 0;
        private readonly object _lock = new object();

        public FrameSaver(string saveDirectory)
        {
            _saveDirectory = saveDirectory;

            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
        }

        /// <summary>
        /// 保存相機每一幀圖像
        /// </summary>
        /// <param name="frame">當前幀的 Bitmap 物件</param>
        public void SaveFrame(Bitmap frame)
        {
            if (frame == null)
                return;

            // 使用時間戳+索引生成唯一檔名
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string fileName = $"frame_{timestamp}_{Interlocked.Increment(ref _frameIndex)}.jpg";
            string filePath = Path.Combine(_saveDirectory, fileName);

            try
            {
                // 鎖住避免多執行緒同時寫入
                lock (_lock)
                {
                    frame.Save(filePath, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存影像失敗: {ex.Message}");
            }
        }
    }
}