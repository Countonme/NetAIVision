using NetAIVision.Model.FrmResult;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Controller
{
    public partial class FrmResult : UIForm
    {
        private readonly ResultEnum _result;
        private readonly int _closeTime; // 秒数

        public FrmResult(ResultEnum result, int closeTime)
        {
            InitializeComponent();
            _result = result;
            _closeTime = closeTime;

            // 根据结果显示不同背景
            ShowResultImage();
        }

        private void ShowResultImage()
        {
            try
            {
                switch (_result)
                {
                    case ResultEnum.Pass:
                        this.BackgroundImage = Properties.Resources.pass;  // pass.bmp 添加到资源
                        break;

                    case ResultEnum.Fail:
                        this.BackgroundImage = Properties.Resources.fail;  // fail.bmp 添加到资源
                        break;
                }

                this.BackgroundImageLayout = ImageLayout.Stretch; // 让图像铺满
            }
            catch (Exception ex)
            {
                UIMessageBox.ShowError($"加载背景图失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 异步显示并在指定时间后关闭
        /// </summary>
        public async Task CloseMeAsync()
        {
            await Task.Delay(_closeTime * 1000);  // 延迟
            this.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }

        /// <summary>
        /// 同步显示并在指定时间后关闭（会阻塞当前线程）
        /// </summary>
        public void CloseMeSync()
        {
            Thread.Sleep(_closeTime * 1000); // 等待指定秒数（单位：毫秒）

            // 使用 Invoke 确保在 UI 线程关闭窗体
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.Close()));
            }
            else
            {
                this.Close();
            }
        }
    }
}