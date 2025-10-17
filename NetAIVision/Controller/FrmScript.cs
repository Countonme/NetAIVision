using NetAIVision.Model.ROI;
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
using System.Web.UI.WebControls;
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
        public ROI _roi;

        // 初始化日志控件
        private ConsoleStyleLogHelper logHelper;

        public FrmScript(Bitmap bitmap, ROI roi)
        {
            InitializeComponent();
            logHelper = new ConsoleStyleLogHelper(richboxLogs, 50);
            this._roi = roi;
            this._bitmap = bitmap;
            this._bitmap_with = bitmap;
            this.runToolStripMenuItem.Click += RunToolStripMenuItem_Click;
            //脚本保存
            this.saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;

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

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (uiListBox1.Items.Count > 0)
            {
                var selectedItems = uiListBox1.Items.Cast<object>()
                                          .Select(x => x.ToString())
                                          .ToArray();

                // 转成数组：["11", "22", "33"]
                //string result = string.Join(", ", selectedItems);
                _roi.step_script = selectedItems.ToList();
            }
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
            System.Windows.Forms.TreeNode clickedNode = e.Node;
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
            var frm = new FrmGaussianBlur();
            frm.ShowDialog();
            if (frm.IsOK)
            {
                _bitmap_with = BitmapProcessorServices.GaussianBlur(_bitmap_with, frm.Param.SigmaX, frm.Param.KernelSize);
                pictureBox1.Image = _bitmap_with;
            }
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
            if (!(_roi.step_script is null) && _roi.step_script.Count > 0)
            {
                uiListBox1.Items.AddRange(_roi.step_script.ToArray());
            }
        }

        private void addProcessStep(string stepFn)
        {
            switch (stepFn)
            {
                //高斯模糊
                case "YS001":
                    {
                        var frm = new FrmGaussianBlur();
                        frm.ShowDialog();
                        if (frm.IsOK)
                        {
                            uiListBox1.Items.Add($"YS001->{uiListBox1.Items.Count}->图像高斯模糊->{frm.Param.SigmaX}->{frm.Param.KernelSize}");
                            logHelper.AppendLog("INFO: 添加图像高斯模糊处理步骤");
                        }
                        else
                        {
                            logHelper.AppendLog("WARN: 图像高斯模糊处理步骤取消");
                        }
                        break;
                    }
                //OCR
                case "YS101":
                    {
                        uiListBox1.Items.Add($"YS101->{uiListBox1.Items.Count}->OCR 文字提取");
                        logHelper.AppendLog("INFO: 添加图像OCR 文字提取处理步骤");
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
                        var strFrm = new FrmCharacterComparison(uiListBox1.Items.Count - 1);
                        strFrm.ShowDialog();
                        if (strFrm.IsOK)
                        {
                            uiListBox1.Items.Add($"YS102->{uiListBox1.Items.Count}->文字比对->{strFrm.Person.step_number}->{strFrm.Person.base_string}");
                        }
                        else
                        {
                            logHelper.AppendLog("WARN: 处理步骤取消");
                        }
                        break;
                    }
                case "YS103":
                    {
                        uiListBox1.Items.Add($"YS103->{uiListBox1.Items.Count}->图像反色");
                        logHelper.AppendLog("INFO: 添加图像  图像反色 处理步骤");
                        break;
                    }
                case "YS104"://图像二值化
                    {
                        uiListBox1.Items.Add($"YS104->{uiListBox1.Items.Count}->图像二值化");
                        logHelper.AppendLog("INFO: 添加图像  二值化 处理步骤");
                        break;
                    }
                case "YS105"://图像灰色边缘检测
                    {
                        uiListBox1.Items.Add($"YS105->{uiListBox1.Items.Count}->图像灰色边缘检测");
                        logHelper.AppendLog("INFO: 添加图像  灰色边缘检测 处理步骤");
                        break;
                    }
                case "YS106"://图像彩色边缘检测
                    {
                        uiListBox1.Items.Add($"YS106->{uiListBox1.Items.Count}->图像彩色边缘检测");
                        logHelper.AppendLog("INFO: 添加图像  彩色边缘检测 处理步骤");
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
                        uiListBox1.Items.Add($"YS110->{uiListBox1.Items.Count}->图像锐化处理");
                        logHelper.AppendLog("INFO: 添加图像  锐化 处理步骤");
                        break;
                    }
                case "YS111"://QRCODE 识别
                    {
                        uiListBox1.Items.Add($"YS111->{uiListBox1.Items.Count}->图像QR Code 识别");
                        logHelper.AppendLog("INFO: 添加图像  QR Code 识别 处理步骤");
                        break;
                    }
                case "YS112": //图片相似度比较
                    {
                        var frm = new FrmSimilarity(_bitmap);
                        frm.ShowDialog();
                        if (frm.IsOK)
                        {
                            uiListBox1.Items.Add($"YS112->{uiListBox1.Items.Count}->图片相似度比较->{frm.Param.threshold}->{frm.Param.path}");
                            logHelper.AppendLog("INFO: 添加图像 图片相似度比较 处理步骤");
                        }
                        break;
                    }
                case "YS113"://拼詞檢查
                    {
                        // 初始化（假設詞典文件在執行目錄下）
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
                            this.ShowErrorNotifier("必须有OCR 步骤才能使用 拼寫檢查");
                            return;
                        }
                        var strFrm = new FrmSettingSpellCheck(uiListBox1.Items.Count - 1);
                        strFrm.ShowDialog();
                        if (strFrm.IsOK)
                        {
                            uiListBox1.Items.Add($"YS113->{uiListBox1.Items.Count}->拼寫檢查->{strFrm.Param.step_number}->{strFrm.Param.suggest}");
                        }
                        else
                        {
                            logHelper.AppendLog("WARN: 处理步骤取消");
                        }
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
                    var fn = itemString.Split("->")[0];
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
                                var step = int.Parse(itemString.Split("->")[3].ToString());
                                var base_string = itemString.Split("->")[4].ToString();
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
                        case "YS112":////图片相似度比较
                            {
                                var threshold = double.Parse(itemString.Split("->")[3].ToString());
                                var path = itemString.Split("->")[4].ToString();
                                if (string.IsNullOrEmpty(path) && !File.Exists(path))
                                {
                                    logHelper.AppendLog($"ERROR :Step{i} 图片相似度比较 参考图片路径无效");
                                    return;
                                }
                                string _basePath = System.Windows.Forms.Application.StartupPath + @"\" + Guid.NewGuid().ToString() + ".jpg";
                                _bitmap_with.Save(_basePath);
                                var value = BitmapProcessorServices.CompareWithSIFT(_basePath, path);
                                if (value >= threshold)
                                {
                                    logHelper.AppendLog($"SUCCESS :Step{i} 图片相似度比较 结果:{value.ToString("F3")},阈值:{threshold}");
                                }
                                else
                                {
                                    logHelper.AppendLog($"WARN :Step{i} 图片相似度比较 结果:{value.ToString("F3")},阈值:{threshold}");
                                }
                                break;
                            }
                        case "YS113":////Spell 拼詞檢查
                            {
                                var step = int.Parse(itemString.Split("->")[3].ToString());
                                var suggestion = bool.Parse(itemString.Split("->")[4].ToString());
                                var ocr_string = list.Where(x => x.step == step).FirstOrDefault().text;
                                logHelper.AppendLog($"Info: '{ocr_string}' 拼寫檢查。");
                                if (SpellChecker.Check(ocr_string))
                                {
                                    logHelper.AppendLog($"SUCCESS:✅ '{ocr_string}' 拼寫正確。");
                                }
                                else
                                {
                                    logHelper.AppendLog($"❌ '{ocr_string}' 拼寫錯誤。");
                                    var suggestions = SpellChecker.Suggest(ocr_string);
                                    if (suggestion)
                                    {
                                        logHelper.AppendLog($"WARN: 建議: {string.Join(", ", suggestions)}");
                                    }
                                }
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