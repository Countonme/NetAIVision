using NetAIVision.Services;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using ZXing.QrCode.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace NetAIVision.Controller
{
    public partial class FrmScript : UIForm
    {
        private Bitmap _bitmap;
        private Bitmap _bitmap_with;

        // 初始化日志控件
        private ConsoleStyleLogHelper logHelper;

        public FrmScript(Bitmap bitmap)
        {
            InitializeComponent();
            logHelper = new ConsoleStyleLogHelper(richboxLogs, 50);

            this._bitmap = bitmap;
            this._bitmap_with = bitmap;
            this.runToolStripMenuItem.Click += RunToolStripMenuItem_Click;
            this.Shown += FrmScript_Shown;
            this.baseOToolStripMenuItem.Click += BaseOToolStripMenuItem_Click;
            this.clearStepsToolStripMenuItem.Click += ClearStepsToolStripMenuItem_Click;
            //灰度
            this.GrayscaleToolStripMenuItem.Click += GrayscaleToolStripMenuItem_Click;
            //二值化
            this.ThresholdToolStripMenuItem.Click += ThresholdToolStripMenuItem_Click;
            //反色
            this.InvertToolStripMenuItem.Click += InvertToolStripMenuItem_Click;
            //高斯模糊
            this.GaussianBlurUToolStripMenuItem.Click += GaussianBlurUToolStripMenuItem_Click;
            //灰度边缘
            this.DetectEdgestoolStripMenuItem1.Click += DetectEdgestoolStripMenuItem1_Click;
            //彩色边缘
            this.DetectEdgesColoredToolStripMenuItem.Click += DetectEdgesColoredToolStripMenuItem_Click;
            //水平翻转
            this.FlipHorizontalToolStripMenuItem.Click += FlipHorizontalToolStripMenuItem_Click;
            //垂直翻转
            this.FlipVerticalToolStripMenuItem.Click += FlipVerticalToolStripMenuItem_Click;
            //OCR
            this.OCRToolStripMenuItem.Click += OCRToolStripMenuItem_Click;
            //QR CODE
            this.QRCodeToolStripMenuItem.Click += QRCodeToolStripMenuItem_Click;
            //Tree View Node MouseDoubleClick
            this.treeVFn.NodeMouseDoubleClick += TreeVFn_NodeMouseDoubleClick;
            //均值滤波
            this.MeanBlurToolStripMenuItem.Click += MeanBlurToolStripMenuItem_Click;
            //锐化
            this.EnhanceSharpnessToolStripMenuItem.Click += EnhanceSharpnessToolStripMenuItem_Click;
        }

        private void ClearStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uiListBox1.Items.Clear();
        }

        /// <summary>
        /// 锐化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnhanceSharpnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.EnhanceSharpness(_bitmap_with, 5);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeanBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.MeanBlur(_bitmap_with, 5);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 运行单步脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runProcessStep();
        }

        /// <summary>
        /// 添加脚步功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeVFn_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 获取被点击的节点
            TreeNode clickedNode = e.Node;
            addProcessStep(e.Node.Name);
        }

        /// <summary>
        /// QR CODE 识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void QRCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var qrcode = BitmapProcessorServices.QR_Code(_bitmap_with);
            //pictureBox1.Image = _bitmap_with;
            var flag = qrcode.flag ? "SUCCESS" : "Error";
            logHelper.AppendLog($"{flag} QR CODE Data:{qrcode.txt}");
        }

        /// <summary>
        /// OCR 文字处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OCRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = BitmapProcessorServices.OCRFn(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
            logHelper.AppendLog($"OCR Data:{text}");
        }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void FlipVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.FlipVertical(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 水平翻转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void FlipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.FlipHorizontal(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 彩色边缘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetectEdgesColoredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.DetectEdgesColored(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 灰度边缘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetectEdgestoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.DetectEdges(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 高斯模糊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void GaussianBlurUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 反色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void InvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.Invert(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.Threshold(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 转为灰度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = BitmapProcessorServices.ToGrayscale(_bitmap_with);
            pictureBox1.Image = _bitmap_with;
        }

        /// <summary>
        /// 原图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bitmap_with = _bitmap;
            pictureBox1.Image = _bitmap;
        }

        private void FrmScript_Shown(object sender, EventArgs e)
        {
            pictureBox1.Image = _bitmap;
        }

        private void addProcessStep(string stepFn)
        {
            switch (stepFn)
            {
                //OCR
                case "YS101":
                    {
                        uiListBox1.Items.Add($"YS101:{uiListBox1.Items.Count}:OCR 文字提取");
                        break;
                    }
                case "YS102":
                    {
                        if (uiListBox1.Items.Count == 0)
                        {
                            this.ShowErrorNotifier("必须有OCR 步骤才能使用文字比对");
                            return;
                        }
                        bool ocr_flag = false;
                        for (int i1 = 0; i1 < uiListBox1.Items.Count; i1++)
                        {
                            if (uiListBox1.Items[i1].ToString().StartsWith("YS101"))
                            {
                                ocr_flag = true;
                                break;
                            }
                        }
                        if (!ocr_flag)
                        {
                            this.ShowErrorNotifier("必须有OCR 步骤才能使用文字比对");
                            return;
                        }
                        var strFrm = new FrmCharacterComparison(uiListBox1.Items.Count);
                        strFrm.ShowDialog();
                        uiListBox1.Items.Add($"YS102:{uiListBox1.Items.Count}:文字比对:{strFrm.step_number}:{strFrm.base_string}");

                        break;
                    }
                case "YS103":
                    {
                        uiListBox1.Items.Add($"YS103:{uiListBox1.Items.Count}:图像反色");
                        break;
                    }
                case "YS104"://图像二值化
                    {
                        uiListBox1.Items.Add($"YS104:{uiListBox1.Items.Count}:图像二值化");
                        break;
                    }
                case "YS105"://图像灰色边缘检测
                    {
                        uiListBox1.Items.Add($"YS105:{uiListBox1.Items.Count}:图像灰色边缘检测");
                        break;
                    }
                case "YS106"://图像彩色边缘检测
                    {
                        uiListBox1.Items.Add($"YS106:{uiListBox1.Items.Count}:图像彩色边缘检测");
                        break;
                    }
                case "YS107"://均值滤波
                    {
                        //uiListBox1.Items.Add($"YS104:{uiListBox1.Items.Count}:图像彩色边缘检测");
                        break;
                    }
                case "YS108"://双边滤波
                    {
                        //uiListBox1.Items.Add($"YS104:{uiListBox1.Items.Count}:图像彩色边缘检测");
                        break;
                    }
                case "YS109"://中值滤波
                    {
                        //uiListBox1.Items.Add($"YS104:{uiListBox1.Items.Count}:图像彩色边缘检测");
                        break;
                    }

                case "YS110"://锐化处理
                    {
                        uiListBox1.Items.Add($"YS110:{uiListBox1.Items.Count}:图像锐化处理");
                        break;
                    }
                case "YS111"://QRCODE 识别
                    {
                        uiListBox1.Items.Add($"YS111:{uiListBox1.Items.Count}:图像QR Code 识别");
                        break;
                    }
            }
        }

        /// <summary>
        /// 执行处理步骤
        /// </summary>
        private void runProcessStep()
        {
            _bitmap_with = _bitmap;
            if (uiListBox1.Items.Count > 0)
            {
                List<(int step, string text)> list = new List<(int step, string text)>();
                for (int i = 0; i < uiListBox1.Items.Count; i++)
                {
                    var itemString = uiListBox1.Items[i].ToString();
                    var fn = itemString.Split(':')[0];
                    // 先取消之前的选中项
                    uiListBox1.ClearSelected();

                    // 选中当前行
                    uiListBox1.SelectedIndex = i;
                    switch (fn)
                    {
                        //OCR Function
                        case "YS101":
                            {
                                var result = BitmapProcessorServices.OCRFn(_bitmap_with);
                                //logHelper.AppendLog($"INFO :OCR Data:{result}");
                                list.Add((i, result));
                                logHelper.AppendLog($"INFO :Step{i} OCR 文字提取 OCR Data:{result}");
                                break;
                            }
                        //文字比对
                        case "YS102":
                            {
                                var step = int.Parse(itemString.Split(':')[3].ToString());
                                var base_string = itemString.Split(':')[4].ToString();
                                var ocr_string = list.Where(x => x.step == step).FirstOrDefault().text;
                                logHelper.AppendLog($"INFO :Step{i} 文字比对 OCR Data：{ocr_string}");
                                if (base_string == ocr_string)
                                {
                                    logHelper.AppendLog($"SUCCESS:文字比对成功，内容一致");
                                }
                                else
                                {
                                    logHelper.AppendLog($"ERROR:文字比对失败，内容不一致");
                                    return;
                                }

                                break;
                            }
                        case "YS103":  //图像反色
                            {
                                _bitmap_with = BitmapProcessorServices.Invert(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像反色处理完成");
                                PictureRefresh();
                                break;
                            }
                        case "YS104":  //二值化
                            {
                                _bitmap_with = BitmapProcessorServices.Threshold(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像二值化处理完成");
                                PictureRefresh();
                                break;
                            }
                        case "YS105":  //灰色边缘处理
                            {
                                _bitmap_with = BitmapProcessorServices.DetectEdges(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像灰色边缘处理完成");
                                PictureRefresh();
                                break;
                            }
                        case "YS106":  //彩色边缘处理
                            {
                                _bitmap_with = BitmapProcessorServices.DetectEdgesColored(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像彩色边缘处理完成");
                                PictureRefresh();
                                break;
                            }

                        case "YS110":  //锐化处理
                            {
                                _bitmap_with = BitmapProcessorServices.EnhanceSharpness(_bitmap_with, 5);
                                logHelper.AppendLog($"INFO :Step{i} 图像理锐化处理完成");
                                break;
                            }
                        case "YS111":
                            {
                                var text = BitmapProcessorServices.QR_Code(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像QR Code:Data{text}");
                                break;
                            }
                    }
                }
            }
        }

        private void PictureRefresh()
        {
            this.pictureBox1.Image = _bitmap_with;
            this.pictureBox1.Refresh();
        }
    }
}