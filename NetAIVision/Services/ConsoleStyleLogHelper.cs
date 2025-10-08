using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Services
{
    public class ConsoleStyleLogHelper
    {
        private readonly RichTextBox _richTextBox;
        private readonly int _maxLines;
        private readonly Queue<string> _logQueue = new Queue<string>();
        private readonly Dictionary<string, Color> _keywordColors;

        public ConsoleStyleLogHelper(RichTextBox richTextBox, int maxLines = 100)
        {
            _richTextBox = richTextBox;
            _maxLines = maxLines;

            _richTextBox.ReadOnly = true;
            _richTextBox.BackColor = Color.Black;
            _richTextBox.ForeColor = Color.White;
            _richTextBox.Font = new Font("Consolas", 10);
            _richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;

            // 默认关键字和颜色
            _keywordColors = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            { "INFO", Color.White },
            { "DEBUG", Color.LightGray },
            { "WARN", Color.Orange },
            { "ERROR", Color.Red },
            { "FATAL", Color.DarkRed },
            { "SUCCESS", Color.Lime },
            { "FAIL", Color.Pink },
            { "TRACE", Color.Cyan },
            { "NETWORK", Color.LightBlue },
            { "DB", Color.MediumPurple }
        };
        }

        /// <summary>
        /// 添加或修改关键字颜色
        /// </summary>
        public void SetKeywordColor(string keyword, Color color)
        {
            _keywordColors[keyword] = color;
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        public void AppendLog(string message)
        {
            message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}";
            if (_richTextBox.InvokeRequired)
            {
                _richTextBox.Invoke(new Action(() => AppendLogInternal(message)));
            }
            else
            {
                AppendLogInternal(message);
            }
        }

        private void AppendLogInternal(string message)
        {
            _logQueue.Enqueue(message);
            if (_logQueue.Count > _maxLines)
                _logQueue.Dequeue();

            _richTextBox.Clear();

            foreach (var log in _logQueue)
            {
                int start = _richTextBox.TextLength;
                _richTextBox.AppendText(log + Environment.NewLine);

                foreach (var kvp in _keywordColors)
                {
                    string keyword = kvp.Key;
                    Color color = kvp.Value;

                    int index = start;
                    while ((index = _richTextBox.Text.IndexOf(keyword, index, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        _richTextBox.Select(index, keyword.Length);
                        _richTextBox.SelectionColor = color;
                        index += keyword.Length;
                    }
                }
            }

            // 光标滚动到底部
            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            _richTextBox.SelectionColor = _richTextBox.ForeColor;
            _richTextBox.ScrollToCaret();
        }
    }
}