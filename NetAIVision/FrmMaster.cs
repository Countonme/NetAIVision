using Microsoft.VisualBasic;
using MvCamCtrl.NET;
using NetAIVision.Controller;
using NetAIVision.Model;
using NetAIVision.Model.FrmResult;
using NetAIVision.Model.ROI;
using NetAIVision.Model.Scripts;
using NetAIVision.Services;
using NetAIVision.Services.MES;
using NetAIVision.Services.OnnxServices;
using Newtonsoft.Json;
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using Tesseract;
using ZXing;
using ZXing.Common;
using static MvCamCtrl.NET.MyCamera;

namespace NetAIVision
{
    public partial class FrmMaster : UIForm
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        // ch:判断用户自定义像素格式 | en:Determine custom pixel format
        public const Int32 CUSTOMER_PIXEL_FORMAT = unchecked((Int32)0x80000000);

        private MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        private MyCamera m_MyCamera = new MyCamera();
        private bool m_bGrabbing = false;
        private Thread m_hReceiveThread = null;
        private MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();

        // ch:用于从驱动获取图像的缓存 | en:Buffer for getting image from driver
        private UInt32 m_nBufSizeForDriver = 0;

        private IntPtr m_BufForDriver = IntPtr.Zero;
        private static Object BufForDriverLock = new Object();

        // ch:Bitmap及其像素格式 | en:Bitmap and Pixel Format
        private Bitmap m_bitmap = null;

        private Bitmap templateBitmap = null;
        private PixelFormat m_bitmapPixelFormat = PixelFormat.DontCare;
        private IntPtr m_ConvertDstBuf = IntPtr.Zero;
        private UInt32 m_nConvertDstBufLen = 0;

        private IntPtr displayHandle = IntPtr.Zero;
        private bool NewScriptFlag = false;

        // 初始化日志控件
        private ConsoleStyleLogHelper logHelper;

        // 拼詞模型Status
        private bool spellReadyFlag = true;

        private int _zoomFactor = 1; // 根据你的代码，放大倍数为1

        #region roi

        private List<ScriptROI> rois = new List<ScriptROI>(); // 保存ROI的列表
        private ScriptROI currentROI = null;            // 当前正在绘制的ROI
        private bool isDrawing = false;           // 是否正在绘制
        private System.Drawing.Point startPoint;                 // 起点
        private int roiCounter = 1;               // ROI编号计数
        private ScriptROI selectedRoi;
        private bool isDraggingRoi = false;
        private System.Drawing.Point dragOffset; // 鼠标相对于 ROI 左上角的偏移
        private ScriptROI draggingRoi = null;
        private bool isResizingRoi = false;
        private ScriptROI resizingRoi = null;
        private ResizeHandle activeHandle = ResizeHandle.None;
        private System.Drawing.Point lastMousePos; // 用于计算增量
        private bool RunFlag = false;

        // 缩放手柄类型（四个角）
        public enum ResizeHandle
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        #endregion roi

        //OCR
        public readonly string _lang = "chi_sim"; // 可改为 eng, chi_tra 等

        public string _tessDataPath;
        public bool OCRReadyFlag = false;
        private string QrcodeString = string.Empty;
        private float Goal_rotationAngle = 0;
        private int Goal_MoveCount = 0;
        private string saveImgPath = Path.Combine(Application.StartupPath, "VideoCollection");
        private FrameSaver save;
        private bool Collection = false;
        private bool RoiMoveFlag = false;

        //AI Model
        private OnnxDetector detector;

        public FrmMaster()
        {
            InitializeComponent();
            //拼詞檢查Spell 初始化
            InitSpellChecker();
            //OCR 初始化
            InitOCR();
            //Init Model
            InitDetector();
            this.Load += FrmMaster_Load;
            //About
            this.aboutAToolStripMenuItem.Click += AboutAToolStripMenuItem_Click;
            this.editScriptsToolStripMenuItem.Click += EditScriptsToolStripMenuItem_Click;
            // Open
            this.RunningScriptToolStripMenuItem.Click += RunningScriptToolStripMenuItem_Click;
            this.openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            this.openImagesToolStripMenuItem.Click += OpenImagesToolStripMenuItem_Click;
            this.openLogsToolStripMenuItem.Click += OpenLogsToolStripMenuItem_Click;
            this.importImageToolStripMenuItem.Click += ImportImageToolStripMenuItem_Click;
            this.closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            this.stopToolStripMenuItem.Click += StopToolStripMenuItem_Click;
            this.pictureBox1.MouseDown += PictureBox1_MouseDown;
            this.pictureBox1.MouseMove += PictureBox1_MouseMove;
            this.pictureBox1.MouseUp += PictureBox1_MouseUp;
            this.pictureBox1.MouseClick += PictureBox1_MouseClick;
            this.pictureBox1.Paint += PictureBox1_Paint;
            this.refreshToolStripMenuItem.Click += RefreshToolStripMenuItem_Click;
            this.Text += $" Version:{Application.ProductVersion}";
            this.Shown += FrmMaster_Shown;
            this.switchMES.Click += SwitchMES_Click;

            this.RadioDebugMode.Click += RadioDebugMode_Click;
            this.RadioBtnProductionMode.Click += RadioBtnProductionMode_Click;

            //--Save Image——————————————————————————————————————————————————
            this.SavebmpToolStripMenuItem.Click += SaveBmpToolStripMenuItem_Click;
            this.SavejPGToolStripMenuItem.Click += SaveJpgToolStripMenuItem_Click;
            this.SavetIFFToolStripMenuItem.Click += SaveTiffToolStripMenuItem_Click;
            this.SavepNGToolStripMenuItem.Click += SavePNGToolStripMenuItem_Click;
            this.saveTempToolStripMenuItem.Click += SaveTempToolStripMenuItem_Click;
            logHelper = new ConsoleStyleLogHelper(richboxLogs, 20);

            //Remove ROI
            this.removeROIToolStripMenuItem.Click += RemoveROIToolStripMenuItem_Click;
            this.QRCodeToolStripMenuItem.Click += QRCodeToolStripMenuItem_Click;
            this.RenameROIToolStripMenuItem.Click += RenameROIToolStripMenuItem_Click;
            this.clearROIToolStripMenuItem.Click += ClearROIToolStripMenuItem_Click;
            this.oCRToolStripMenuItem.Click += OCRToolStripMenuItem_Click;
            //Exit
            this.exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            this.FormClosing += FrmMaster_FormClosing;
            //污点检查
            this.taintAnalysisToolStripMenuItem.Click += TaintAnalysisToolStripMenuItem_Click;
            //ROI 脚本按钮
            this.viewRoiScriptToolStripMenuItem.Click += ViewRoIScriptToolStripMenuItem_Click;
            this.chearLogToolStripMenuItem.Click += ClearLogToolStripMenuItem_Click;
            //新建脚本
            this.newScriptsToolStripMenuItem.Click += NewScriptsToolStripMenuItem_Click;
            //保存脚本
            this.saveingScriptsToolStripMenuItem.Click += SaveingScriptsToolStripMenuItem_Click;
            //加载脚本
            this.openScriptToolStripMenuItem.Click += OpenScriptToolStripMenuItem_Click;
            //单步运行脚本
            this.runScriptToolStripMenuItem.Click += RunScriptToolStripMenuItem_Click;
            if (!Directory.Exists(saveImgPath))
            {
                Directory.CreateDirectory(saveImgPath);
            }
            save = new FrameSaver(saveImgPath);
        }

        public void InitDetector()
        {
            var modelPath = Path.Combine(Application.StartupPath, "Lib", "Model", "UK5W.onnx");

            detector = new OnnxDetector(modelPath, confidenceThreshold: 0.35f);
        }

        private void ClearROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rois?.Count > 0)
            {
                rois.Clear();
                logHelper.AppendLog("INFO: Clear scripts ROI Completed...");
            }
        }

        private void EditScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewScriptFlag = true;
            logHelper.AppendLog("WARN: 启用脚本编辑功能");
        }

        private void RunningScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logHelper.AppendLog($"Info: 自動校準狀態 {RunFlag}");
            if (!(rois is null) && rois.Count > 0)
            {   ///每次檢查前 初始化檢查結果
                initRoIAndCheckResult();
                pictureBox1.Invalidate();
                pictureBox2.Image = Properties.Resources.Inprogress;
                Task.Run(() =>
               {
                   // 耗时操作放后台线程
                   var flag = true;
                   RunFlag = true;
                   Stopwatch timer = new Stopwatch();
                   timer.Start();
                   foreach (var item in rois)
                   {
                       Rectangle roiRect = item.Rect;
                       if (pictureBox1.Image is null)
                       {
                           RunFlag = false;
                           return;
                       }
                       // 确保 ROI 在图像范围内
                       if (roiRect.X < 0 || roiRect.Y < 0 ||
                           roiRect.Right > pictureBox1.Image.Width ||
                           roiRect.Bottom > pictureBox1.Image.Height)
                       {
                           MessageBox.Show("ROI区域超出图像范围，无法分析。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                           RunFlag = false;
                           return;
                       }

                       // 1. 创建 ROI 区域的位图
                       Bitmap templateImage = new Bitmap(roiRect.Width, roiRect.Height);
                       using (Graphics g = Graphics.FromImage(templateImage))
                       {
                           g.DrawImage(pictureBox1.Image,
                                       new Rectangle(0, 0, roiRect.Width, roiRect.Height),
                                       roiRect,
                                       GraphicsUnit.Pixel);
                       }
                       if (runProcessStep(templateImage, item))
                       {
                           item.pen_color = Color.LimeGreen; // 绿色表示通过
                           item.Brushes_color = Brushes.LimeGreen;
                       }
                       else
                       {
                           item.pen_color = Color.Red; // 绿色表示通过
                           item.Brushes_color = Brushes.Red;
                           flag = false;
                           break;
                       }
                       pictureBox1.Invalidate();  // 重绘图像区域以显示 ROI
                   }
                   RunFlag = false;
                   timer.Stop();
                   TimeSpan elapsed = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
                   txtLengthy.BeginInvoke(new Action(() =>
                   {
                       txtLengthy.Text = elapsed.TotalSeconds.ToString("F2") + " S";
                   }));
                   this.Invoke(new Action(() =>
                   {
                       if (flag)
                       {
                           StyleManager.Style = UIStyle.LayuiGreen;
                           pictureBox2.Image = Properties.Resources.pass;
                           var frm = new FrmResult(ResultEnum.Pass, 3);
                           frm.Show();
                           // 把阻塞操作放到后台线程
                           Task.Run(() =>
                           {
                               Thread.Sleep(3000); // 等待3秒
                               frm.Invoke(new Action(() =>
                               {
                                   frm.Close();
                                   StyleManager.Style = UIStyle.Blue;
                               })); // 回到UI线程关闭
                           });

                           this.ShowSuccessNotifier("所有检测通过！");
                       }
                       else
                       {
                           StyleManager.Style = UIStyle.Red;
                           pictureBox2.Image = Properties.Resources.fail;
                           var frm = new FrmResult(ResultEnum.Fail, 3);
                           frm.Show();
                           // 把阻塞操作放到后台线程
                           Task.Run(() =>
                            {
                                Thread.Sleep(3000); // 等待3秒
                                frm.Invoke(new Action(() =>
                                {
                                    frm.Close();
                                    StyleManager.Style = UIStyle.Blue;
                                })); // 回到UI线程关闭
                            });
                           this.ShowErrorNotifier("检测失败，请检查！");
                       }
                   }));
               });
            }
        }

        /// <summary>
        /// 单步运行脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RunScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi == null)
            {
                MessageBox.Show("请先选择一个ROI区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Rectangle roiRect = selectedRoi.Rect;

            // 确保 ROI 在图像范围内
            if (roiRect.X < 0 || roiRect.Y < 0 ||
                roiRect.Right > pictureBox1.Image.Width ||
                roiRect.Bottom > pictureBox1.Image.Height)
            {
                MessageBox.Show("ROI区域超出图像范围，无法分析。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. 创建 ROI 区域的位图
            Bitmap templateImage = new Bitmap(roiRect.Width, roiRect.Height);
            using (Graphics g = Graphics.FromImage(templateImage))
            {
                g.DrawImage(pictureBox1.Image,
                            new Rectangle(0, 0, roiRect.Width, roiRect.Height),
                            roiRect,
                            GraphicsUnit.Pixel);
            }
            if (runProcessStep(templateImage, selectedRoi))
            {
                selectedRoi.pen_color = Color.Lime; // 绿色表示通过
                selectedRoi.Brushes_color = Brushes.LimeGreen;
            }
            else
            {
                selectedRoi.pen_color = Color.Red; // 绿色表示通过
                selectedRoi.Brushes_color = Brushes.Red;
            }
            pictureBox1.Invalidate(); ; // 重绘图像区域以显示 ROI
        }

        /// <summary>
        /// 加载脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OpenScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    // 设置文件过滤器
                    ofd.Filter = "脚本文件|*.ysc|所有文件|*.*";
                    ofd.Title = "请选择要加载的脚本文件";

                    // 获取程序根目录下的 Scripts 文件夹路径
                    string appPath = Application.StartupPath;
                    string defaultFolder = Path.Combine(appPath, "Scripts");

                    // 如果 Scripts 文件夹存在，则设为初始目录
                    if (Directory.Exists(defaultFolder))
                    {
                        ofd.InitialDirectory = defaultFolder;
                    }

                    // 设置默认文件扩展名
                    ofd.DefaultExt = "ysc";
                    ofd.CheckFileExists = true; // 确保文件存在

                    // 显示对话框
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = ofd.FileName;
                        txtscriptName.Text = filePath;
                        try
                        {
                            // 读取文件内容
                            string scriptContent = System.IO.File.ReadAllText(filePath);

                            // 反序列化 JSON 到 rois 列表
                            var loadedRois = JsonConvert.DeserializeObject<OBAScripts>(scriptContent);

                            if (loadedRois == null || loadedRois.scripts.Count == 0)
                            {
                                logHelper.AppendLog("ERROR: 脚本文件为空或无效");
                                this.ShowWarningNotifier("脚本文件为空或无效。");
                                return;
                            }
                            if (!File.Exists(loadedRois.modelTemplate))
                            {
                                logHelper.AppendLog("ERROR: 没有参考模板");
                                this.ShowWarningNotifier("脚本文件没有参考模板或无效。");
                                return;
                            }
                            pictureBox3.Image?.Dispose();
                            var bim = new Bitmap(loadedRois.modelTemplate);
                            pictureBox3.Image = (bim);
                            // 成功加载，替换当前 rois
                            rois.Clear();
                            rois.AddRange(loadedRois.scripts);
                            //初始化
                            initRoIAndCheckResult();
                            // 重置为已加载状态（非新建）
                            NewScriptFlag = false;

                            // 初始化模板，只需调用一次
                            ImageAlignment.InitTemplate(bim);
                            // 可选：刷新 UI（如 ROI 列表、图像显示等）
                            pictureBox1.Invalidate();  // 重绘图像区域以显示 ROI

                            // 提示成功
                            logHelper.AppendLog("脚本加载成功！");
                            this.ShowSuccessNotifier("脚本加载成功！");
                        }
                        catch (JsonException jsonEx)
                        {
                            logHelper.AppendLog("ERROR: 脚本格式错误，无法解析：" + jsonEx.Message);
                            this.ShowErrorNotifier("脚本格式错误，无法解析：" + jsonEx.Message);
                        }
                        catch (Exception ex)
                        {
                            logHelper.AppendLog("ERROR: 加载失败：" + ex.Message);
                            this.ShowErrorNotifier("加载失败：" + ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SaveingScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewScriptFlag = false;
            if (rois.Count == 0)
            {
                this.ShowErrorNotifier("没有可用的脚本信息");
                return;
            }
            if (pictureBox1.Image is null)
            {
                this.ShowErrorNotifier("没有可用的参考图信息");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                // 设置默认扩展名和过滤器
                sfd.Filter = " 脚本文件|*.ysc|所有文件|*.ysc";
                sfd.Title = "请选择保存路径";

                // 获取程序根目录下的 BNXScripts 文件夹路径
                string appPath = Application.StartupPath;
                string defaultFolder = Path.Combine(appPath, "Scripts");
                string defaultImgFolder = Path.Combine(appPath, "Scripts", "ScriptTempleteImages");

                // 如果文件夹不存在，则创建
                if (!Directory.Exists(defaultFolder))
                {
                    Directory.CreateDirectory(defaultFolder);
                }
                if (!Directory.Exists(defaultImgFolder))
                {
                    Directory.CreateDirectory(defaultImgFolder);
                }

                // 设置初始目录为 BNXScripts 文件夹
                sfd.InitialDirectory = defaultFolder;

                // 默认文件名（可选）
                sfd.FileName = "NewScript.ysc";

                // 显示对话框并处理用户选择
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;

                    try
                    {
                        var imgpath = $"{defaultImgFolder}\\{Guid.NewGuid().ToString()}.png";
                        pictureBox1.Image.Save(imgpath);
                        OBAScripts scripts = new OBAScripts();
                        scripts.modelTemplate = imgpath;
                        scripts.scripts = rois;
                        var scriptContent = JsonConvert.SerializeObject(scripts, Newtonsoft.Json.Formatting.Indented);
                        // 写入内容到文件
                        System.IO.File.WriteAllText(filePath, scriptContent);

                        // 自定义提示方法（假设你已实现 ShowSuccessTip）
                        this.ShowSuccessTip("保存成功！");
                    }
                    catch (Exception ex)
                    {
                        this.ShowSuccessTip("保存失败：" + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 脚本新建功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewScriptFlag = true;
            rois.Clear();
            logHelper.AppendLog("开启脚本新建功能");
        }

        /// <summary>
        /// 擦除日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                richboxLogs.Clear();
            }));
        }

        /// <summary>
        /// 查看ROI 的脚本信息（ROI 内容处理步骤）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewRoIScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi == null)
            {
                this.ShowErrorNotifier("请先选择一个ROI区域。");
                return;
            }
            if (pictureBox1.Image is null)
            {
                this.ShowErrorNotifier("沒有可以處理的圖像,請連接 相機或手動導入圖片");
                return;
            }
            Rectangle roiRect = selectedRoi.Rect;

            //roiRect.X = (roiRect.X * _zoomFactor);
            //roiRect.Y = (roiRect.Y * _zoomFactor);
            //roiRect.Width = (roiRect.Width * _zoomFactor);
            //roiRect.Height = (roiRect.Height * _zoomFactor);

            // 确保 ROI 在图像范围内
            if (roiRect.X < 0 || roiRect.Y < 0 ||
                roiRect.Right > pictureBox1.Image.Width ||
                roiRect.Bottom > pictureBox1.Image.Height)
            {
                this.ShowErrorNotifier("ROI区域超出图像范围，无法分析。");
                return;
            }

            // 1. 创建 ROI 区域的位图
            Bitmap templateImage = new Bitmap(roiRect.Width, roiRect.Height);
            using (Graphics g = Graphics.FromImage(templateImage))
            {
                g.DrawImage(pictureBox1.Image,
                            new Rectangle(0, 0, roiRect.Width, roiRect.Height),
                            roiRect,
                            GraphicsUnit.Pixel);
            }
            var frmScrip = new FrmScript(templateImage, selectedRoi);
            frmScrip.ShowDialog();
            selectedRoi = frmScrip._roi;
        }

        /// <summary>
        /// 关于我们的窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FrmAbout().ShowDialog();
        }

        /// <summary>
        /// ROI 污点检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaintAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi == null)
            {
                MessageBox.Show("请先选择一个ROI区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Rectangle roiRect = selectedRoi.Rect;

            // 确保 ROI 在图像范围内
            if (roiRect.X < 0 || roiRect.Y < 0 ||
                roiRect.Right > pictureBox1.Image.Width ||
                roiRect.Bottom > pictureBox1.Image.Height)
            {
                MessageBox.Show("ROI区域超出图像范围，无法分析。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. 创建 ROI 区域的位图
            Bitmap templateImage = new Bitmap(roiRect.Width, roiRect.Height);
            using (Graphics g = Graphics.FromImage(templateImage))
            {
                g.DrawImage(pictureBox1.Image,
                            new Rectangle(0, 0, roiRect.Width, roiRect.Height),
                            roiRect,
                            GraphicsUnit.Pixel);
            }

            try
            {
                // 2. 将 Bitmap 转为 Mat（使用 OpenCvSharp.Extensions）
                using (Mat src = BitmapToMat(templateImage))
                using (Mat resultMat = src.Clone())
                {
                    // 3. 执行脏污检测
                    var dirtAreas = DetectDirt(src, thresholdValue: 50, minArea: 50);

                    if (dirtAreas.Count > 0)
                    {
                        // 对每一个检测到的脏污区域画矩形框
                        foreach (var area in dirtAreas)
                        {
                            // area 是 System.Drawing.Rectangle
                            // 需要转换为 OpenCvSharp.Rect
                            OpenCvSharp.Rect roiRects = new OpenCvSharp.Rect(area.X, area.Y, area.Width, area.Height);
                            Cv2.Rectangle(resultMat, roiRects, Scalar.Red, 2);
                        }
                    }

                    // 5. 将结果 Mat 转回 Bitmap
                    Bitmap resultBitmap = MatToBitmap(resultMat);

                    // 6. 显示在 pictureBox2 或新窗体中
                    //pictureBox1.Image?.Dispose(); // 释放旧图像
                    //pictureBox1.Image = resultBitmap;

                    // 7. 提示结果
                    if (dirtAreas.Count > 0)
                    {
                        MessageBox.Show($"在ROI区域内发现 {dirtAreas.Count} 处脏污。", "检测结果",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("ROI区域干净，未发现明显脏污。", "检测结果",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"脏污分析失败：{ex.Message}", "错误",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 释放临时图像
                templateImage.Dispose();
            }
        }

        public static Bitmap MatToBitmap(Mat mat)
        {
            if (mat == null)
                throw new ArgumentException("Mat 为空或无效");

            // ✅ 正确调用旧版 ImEncode
            bool success = Cv2.ImEncode(".png", mat, out byte[] encodedBytes);

            if (!success)
                throw new Exception("图像编码失败：ImEncode 返回 false");

            using (var ms = new MemoryStream(encodedBytes))
            {
                return new Bitmap(ms);
            }
        }

        public static Mat BitmapToMat(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
            }
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMaster_FormClosing(object sender, FormClosingEventArgs e)
        {   //保存参数
            SaveControllerSetting();
            if (this.ShowAskDialog("您確定要退出系統嗎?"))
            {
                MES_Service.MesDisConnect();
                // DisConnectionHardwareDevice();
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //保存参数
            SaveControllerSetting();
            if (this.ShowAskDialog("您確定要退出系統嗎?"))
            {
                MES_Service.MesDisConnect();
                // DisConnectionHardwareDevice();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// OCR 识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OCRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi == null)
            {
                MessageBox.Show("请先选择一个ROI区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Rectangle roiRect = selectedRoi.Rect;

            // 确保 ROI 在图像范围内
            if (roiRect.X < 0 || roiRect.Y < 0 ||
                roiRect.Right > pictureBox1.Image.Width ||
                roiRect.Bottom > pictureBox1.Image.Height)
            {
                MessageBox.Show("ROI区域超出图像范围，无法保存。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. 创建 ROI 区域的位图
            Bitmap templateImage = new Bitmap(roiRect.Width, roiRect.Height);
            using (Graphics g = Graphics.FromImage(templateImage))
            {
                g.DrawImage(pictureBox1.Image,
                            new Rectangle(0, 0, roiRect.Width, roiRect.Height),
                            roiRect,
                            GraphicsUnit.Pixel);
            }
            //// 使用 Tesseract 进行 OCR 识别
            //var engine = new TesseractEngine(_tessDataPath, _lang, EngineMode.Default);
            //// 将 Bitmap 转为 Pix（内存中完成，不保存文件）
            //var ms = new MemoryStream();
            //templateImage = BitmapProcessorServices.PreprocessForOCR(templateImage);
            //templateImage.Save(ms, System.Drawing.Imaging.ImageFormat.Tiff); // 推荐 TIFF，支持灰度/二值化
            //ms.Position = 0;

            //var img = Pix.LoadFromMemory(ms.ToArray());
            //var page = engine.Process(img, PageSegMode.SingleLine);
            //string text = page.GetText();
            var text = BitmapProcessorServices.OCRFn(templateImage);
            pictureBox2.Invoke(new Action(() =>
            {
                if (!(pictureBox2.Image is null))
                {
                    pictureBox2.Image.Dispose();
                }
                pictureBox2.Image = (templateImage);
                pictureBox2.Refresh();
            }));
            logHelper.AppendLog($"Tessreact OCR Data:{text}");
            //text = PaddleOCRHelper.Recognize(templateImage);
            //logHelper.AppendLog($"Paddle OCR Data:{text}");
        }

        /// <summary>
        /// 保存ROI 图片Temp 模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveTempToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi == null)
            {
                MessageBox.Show("请先选择一个ROI区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Rectangle roiRect = selectedRoi.Rect;

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("当前没有加载图像，无法保存模板。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 确保 ROI 在图像范围内
            if (roiRect.X < 0 || roiRect.Y < 0 ||
                roiRect.Right > pictureBox1.Image.Width ||
                roiRect.Bottom > pictureBox1.Image.Height)
            {
                MessageBox.Show("ROI区域超出图像范围，无法保存。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. 创建 ROI 区域的位图
            Bitmap templateImage = new Bitmap(roiRect.Width, roiRect.Height);
            using (Graphics g = Graphics.FromImage(templateImage))
            {
                g.DrawImage(pictureBox1.Image,
                            new Rectangle(0, 0, roiRect.Width, roiRect.Height),
                            roiRect,
                            GraphicsUnit.Pixel);
            }

            // 2. 定义保存路径：程序根目录
            string rootPath = AppDomain.CurrentDomain.BaseDirectory; // 程序运行目录
            string fileName = $"模板_{selectedRoi.Name}.png";
            string path = Path.Combine(rootPath, "Images", "Temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(rootPath, "Images", "Temp", fileName);

            try
            {
                // 3. 保存图像
                templateImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                // 4. 释放资源
                templateImage.Dispose();

                // 5. 提示用户
                MessageBox.Show($"模板已保存：\n{filePath}", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 可选：打开文件所在目录
                // System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 重命名ROI 区域名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi == null)
            {
                MessageBox.Show("请先选择一个ROI区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 弹出输入框
            string newName = Interaction.InputBox(
                "请输入新的名称：",
                "重命名ROI",
                selectedRoi.Name,  // 默认显示当前名称
                -1, -1);           // 居中显示

            // 如果用户点击取消或输入为空，则不操作
            if (string.IsNullOrWhiteSpace(newName))
                return;

            // 更新名称
            selectedRoi.Name = newName;

            // 刷新图像显示（确保 Name 被重绘）
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// 二维码解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QRCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi == null)
            {
                MessageBox.Show("请先选择一个ROI区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 获取 ROI 矩形
            Rectangle roiRect = selectedRoi.Rect;

            // 确保 ROI 在图像范围内
            if (pictureBox1.Image == null ||
                roiRect.X < 0 || roiRect.Y < 0 ||
                roiRect.Right > pictureBox1.Image.Width ||
                roiRect.Bottom > pictureBox1.Image.Height)
            {
                MessageBox.Show("ROI区域无效或超出图像范围。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. 创建一个位图来保存截取的区域
            Bitmap roiImage = new Bitmap(roiRect.Width, roiRect.Height);

            // 2. 使用 Graphics 从 pictureBox1.Image 中拷贝 ROI 区域
            using (Graphics g = Graphics.FromImage(roiImage))
            {
                g.DrawImage(pictureBox1.Image,
                            new Rectangle(0, 0, roiRect.Width, roiRect.Height),
                            roiRect,
                            GraphicsUnit.Pixel);
            }
            // 3. 使用 ZXing 识别二维码
            var barcodeReader = new ZXing.BarcodeReader()
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> {
                        BarcodeFormat.QR_CODE,
                        ZXing.BarcodeFormat.DATA_MATRIX,
                        ZXing.BarcodeFormat.AZTEC,
                        ZXing.BarcodeFormat.PDF_417
                    }
                }
            };
            var result = barcodeReader.Decode(roiImage);

            // 4. 显示结果
            if (result != null)
            {
                logHelper.AppendLog($"INFO:{selectedRoi.Name} 识别成功：{result.Text}");
                //MessageBox.Show($"识别成功：\n{result.Text}", "二维码内容", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                logHelper.AppendLog($"ERROR:{selectedRoi.Name} 未识别到二维码，请确保该区域包含清晰的二维码");
                //MessageBox.Show("未识别到二维码，请确保该区域包含清晰的二维码。", "识别失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // 可选：释放资源
            roiImage.Dispose();
        }

        /// <summary>
        /// 手动导入图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ImportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmps = new Bitmap(dlg.FileName);
                    Bitmap bmp = new Bitmap(bmps);
                    ImageAlignment.RecalcEveryNFrames = 1;
                    bmp = ImageAlignment.AlignToTemplate(bmp);
                    Detect(bmp);
                    // 绘制结果
                    //Bitmap resultImage = detector.DrawDetections(bmp, result);
                    pictureBox1.Image = bmp; // 显示在 UI
                                             //Console.WriteLine("✅ 检测完成，结果已保存到 " + outputPath);
                                             //pictureBox1.Image = result;

                    //var (text, points) = QRCodeHelper.ReturnBarcodeCoordinates(bmp);

                    //if (!string.IsNullOrEmpty(text))
                    //{
                    //    // 绘制二维码框
                    //    var imgWithBox = QRCodeHelper.DrawQRCodeBox(new Bitmap(bmp), points);
                    //    pictureBox1.Image = imgWithBox;
                    //    MessageBox.Show($"识别成功：{text}");
                    //}
                    //else
                    //{
                    //    MessageBox.Show("未识别到二维码");
                    //}
                }
            }
        }

        /// <summary>
        /// 移除ROI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRoi != null)
            {
                rois.Remove(selectedRoi);
                selectedRoi = null; // 清空引用
                pictureBox1.Invalidate(); // 触发重绘
            }
        }

        #region 图片保存

        /// <summary>
        /// PNG 保存
        /// </summary>
        private void SavePNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage(MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Png, "PNG", "png", 8);
        }

        /// <summary>
        /// TIFF 保存
        /// </summary>
        private void SaveTiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage(MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Tif, "TIFF", "tif");
        }

        /// <summary>
        /// JPG 保存
        /// </summary>
        private void SaveJpgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage(MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg, "JPG", "jpg", 80);
        }

        /// <summary>
        /// BMP 保存
        /// </summary>
        private void SaveBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage(MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp, "Bmp", "bmp");
        }

        /// <summary>
        /// Math.Clamp 是 .NET Core / .NET 5+ 才有， .NET Framework（WinForms 很多项目还是 4.x）， Math.Clamp 。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private uint Clamp(int value, int min, int max)
        {
            if (value < min) return (uint)min;
            if (value > max) return (uint)max;
            return (uint)value;
        }

        /// <summary>
        /// 通用图像保存方法
        /// </summary>
        /// <param name="imageType">图片类型</param>
        /// <param name="folderName">保存文件夹名称</param>
        /// <param name="extension">文件扩展名</param>
        /// <param name="quality">保存质量，默认 -1 不设置</param>
        private void SaveImage(MyCamera.MV_SAVE_IAMGE_TYPE imageType, string folderName, string extension, int quality = 0)
        {
            if (!m_bGrabbing)
            {
                logHelper.AppendLog("WARN: Not Start Grabbing");
                ShowErrorMsg("Not Start Grabbing", 0);
                return;
            }

            lock (BufForDriverLock)
            {
                if (m_stFrameInfo.nFrameLen == 0)
                {
                    logHelper.AppendLog("WARN: Save {extension.ToUpper()} Fail!");
                    ShowErrorMsg($"Save {extension.ToUpper()} Fail!", 0);
                    return;
                }

                MyCamera.MV_SAVE_IMG_TO_FILE_PARAM stSaveFileParam = new MyCamera.MV_SAVE_IMG_TO_FILE_PARAM
                {
                    enImageType = imageType,
                    enPixelType = m_stFrameInfo.enPixelType,
                    pData = m_BufForDriver,
                    nDataLen = m_stFrameInfo.nFrameLen,
                    nHeight = m_stFrameInfo.nHeight,
                    nWidth = m_stFrameInfo.nWidth,
                    iMethodValue = 2
                };
                switch (imageType)
                {
                    case MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg:
                        stSaveFileParam.nQuality = Clamp(quality > 0 ? quality : 80, 1, 100);
                        break;

                    case MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Png:
                        stSaveFileParam.nQuality = Clamp(quality >= 0 ? quality : 8, 0, 9);
                        break;

                    default:
                        stSaveFileParam.nQuality = 0; // BMP、TIFF 不设置
                        break;
                }

                string folderPath = Path.Combine(Application.StartupPath, "Images", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                stSaveFileParam.pImagePath = Path.Combine(folderPath,
                    $"{stSaveFileParam.nWidth}_h{stSaveFileParam.nHeight}_fn{m_stFrameInfo.nFrameNum}.{extension}");

                int nRet = m_MyCamera.MV_CC_SaveImageToFile_NET(ref stSaveFileParam);
                if (MyCamera.MV_OK != nRet)
                {
                    logHelper.AppendLog($"WARN: Save {extension.ToUpper()} Fail!");
                    ShowErrorMsg($"Save {extension.ToUpper()} Fail!", nRet);
                    return;
                }
            }

            this.ShowSuccessNotifier("Save Succeed!");
        }

        #endregion 图片保存

        /// <summary>
        /// 打开系统日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "Logs");
            OpenSystemFolder(path);
        }

        /// <summary>
        /// 打开图像日志文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "Images");
            OpenSystemFolder(path);
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ch:标志位设为false | en:Set flag bit false
            m_bGrabbing = false;
            m_hReceiveThread.Join();

            // ch:停止采集 | en:Stop Grabbing
            int nRet = m_MyCamera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                logHelper.AppendLog($"WARN: Stop Grabbing Fail! ");
                ShowErrorMsg("Stop Grabbing Fail!", nRet);
            }
            logHelper.AppendLog($"INFO: Stop Grabbing Collection... ");
        }

        /// <summary>
        /// Handles the click event for a radio button that toggles production mode.
        /// </summary>
        /// <param name="sender">Represents the source of the click event.</param>
        /// <param name="e">Contains the event data associated with the click action.</param>
        private void RadioBtnProductionMode_Click(object sender, EventArgs e)
        {
            if (RadioBtnProductionMode.Checked)
            {
                //if (string.IsNullOrEmpty(txtModelName.Text))
                //{
                //    RadioDebugMode.Checked = true;
                //    this.ShowErrorDialog("没有选择配置文件");
                //    return;
                //}

                if (string.IsNullOrEmpty(mlabEmp.Text))
                {
                    RadioDebugMode.Checked = true;
                    logHelper.AppendLog($"WARN: 没有输入人员工号,非法登录 无法连接MES ");
                    this.ShowErrorDialog("没有输入人员工号");

                    return;
                }
                string message = string.Empty;
                bool checkFlag = MES_Service.CommandCheckUser(mlabEmp.Text, ref message);
                if (!checkFlag)
                {
                    RadioDebugMode.Checked = true;
                    logHelper.AppendLog($"WARN: {message} ");
                    this.ShowErrorDialog($"{message}");
                    return;
                }
                logHelper.AppendLog($"SUCCESS: 登录成功 {mlabEmp.Text} ");
                this.ShowSuccessNotifier($"登录成功 {mlabEmp.Text}");
            }
        }

        /// <summary>
        /// Handles the click event for enabling or disabling radio debug mode.
        /// </summary>
        /// <param name="sender">Represents the source of the click event.</param>
        /// <param name="e">Contains the event data associated with the click action.</param>
        private void RadioDebugMode_Click(object sender, EventArgs e)
        {
            // flagProductionModel = false;
        }

        /// <summary>
        /// MES 开关按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchMES_Click(object sender, EventArgs e)
        {
            if (switchMES.Active)
            {
                var message = MES_Service.CheckLib();
                if (string.IsNullOrEmpty(message))
                {
                    //ShowSystemLogs("MES", "MesConnect");
                    this.ShowSuccessNotifier("MES Connection ...");
                    logHelper.AppendLog($"info: MES Connection ... Start ");
                    MES_Service.MesConnect();
                }
                else
                {
                    //ShowSystemLogs("MES", $"MES连接失败 {message}");
                    switchMES.Active = false;
                    this.ShowErrorNotifier(message);
                    logHelper.AppendLog($"ERROR: {message} ");
                    return;
                }
            }
            else
            {
                // ShowSystemLogs("MES", "MesDisConnect");
                MES_Service.MesDisConnect();
                //切换Debug模式
                RadioDebugMode.Checked = true;
            }
        }

        /// <summary>
        /// 控件加载完成后 调整布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMaster_Shown(object sender, EventArgs e)
        {
            pictureBox1.Size = new System.Drawing.Size(1024, 768);
            pictureBox3.Size = new System.Drawing.Size(640, 480);
            //pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.Location = new System.Drawing.Point(this.Width - 1028, 100);
            cbDeviceList.Width = 1024;
            cbDeviceList.Location = new System.Drawing.Point(this.Width - 1028, 70);
            grouplogs.Height = this.Height - pictureBox1.Height - 100;
            groupSetting.Height = this.Height - groupSetting.Height - 335;
            groupSetting.Width = this.Width - pictureBox1.Width - 10;
            uiLine2.Width = groupSetting.Width - 10;
            logHelper.AppendLog("INFO: 初始化完成 程序启动");
            LoadControllerSetting();
            UIStyles.CultureInfo = CultureInfos.en_US;
            //if (pageIndex < UIStyle.Colorful.Value())
        }

        /// <summary>
        /// 刷新相机设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowWaitForm("正在刷新设备列表...");
            DeviceListAcq();
            this.HideWaitForm();
        }

        /// <summary>
        /// 断开相机连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ch:取流标志位清零 | en:Reset flow flag bit
            if (m_bGrabbing == true)
            {
                m_bGrabbing = false;
                m_hReceiveThread.Join();
            }

            if (m_BufForDriver != IntPtr.Zero)
            {
                Marshal.Release(m_BufForDriver);
            }

            if (m_MyCamera != null)
            {
                // ch:关闭设备 | en:Close Device
                m_MyCamera.MV_CC_CloseDevice_NET();
                m_MyCamera.MV_CC_DestroyDevice_NET();
            }
        }

        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_stDeviceList.nDeviceNum == 0 || cbDeviceList.SelectedIndex == -1)
            {
                ShowErrorMsg("No device, please select", 0);
                return;
            }

            // ch:获取选择的设备信息 | en:Get selected device information
            MyCamera.MV_CC_DEVICE_INFO device =
                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[cbDeviceList.SelectedIndex],
                                                              typeof(MyCamera.MV_CC_DEVICE_INFO));

            // ch:打开设备 | en:Open device
            if (null == m_MyCamera)
            {
                m_MyCamera = new MyCamera();
                if (null == m_MyCamera)
                {
                    ShowErrorMsg("Applying resource fail!", MyCamera.MV_E_RESOURCE);
                    return;
                }
            }

            int nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Create device fail!", nRet);
                return;
            }

            nRet = m_MyCamera.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_MyCamera.MV_CC_DestroyDevice_NET();
                ShowErrorMsg("Device open fail!", nRet);
                return;
            }
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                tbExposure.Text = stParam.fCurValue.ToString("F1");
            }

            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                tbGain.Text = stParam.fCurValue.ToString("F1");
            }

            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                tbFrameRate.Text = stParam.fCurValue.ToString("F1");
            }
            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                if (nPacketSize > 0)
                {
                    nRet = m_MyCamera.MV_CC_SetIntValueEx_NET("GevSCPSPacketSize", nPacketSize);
                    if (nRet != MyCamera.MV_OK)
                    {
                        logHelper.AppendLog($"WARN: Set Packet Size failed! ");
                        ShowErrorMsg("Set Packet Size failed!", nRet);
                    }
                }
                else
                {
                    logHelper.AppendLog($"WARN: Get Packet Size failed! ");
                    ShowErrorMsg("Get Packet Size failed!", nPacketSize);
                }
            }

            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            // ch:前置配置 | en:pre-operation
            nRet = NecessaryOperBeforeGrab();
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }
            displayHandle = pictureBox1.Handle;

            // ch:标志位置true | en:Set position bit true
            m_bGrabbing = true;

            m_stFrameInfo.nFrameLen = 0;//取流之前先清除帧长度
            m_stFrameInfo.enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;

            m_hReceiveThread = new Thread(HikCaremaReceiveThreadProcess);
            m_hReceiveThread.Start();

            // ch:开始采集 | en:Start Grabbing
            nRet = m_MyCamera.MV_CC_StartGrabbing_NET();

            if (MyCamera.MV_OK != nRet)
            {
                m_bGrabbing = false;
                m_hReceiveThread.Join();
                logHelper.AppendLog($"WARN: Start Grabbing Fail! ");
                ShowErrorMsg("Start Grabbing Fail!", nRet);
                return;
            }
            logHelper.AppendLog($"SUCCESS: 相机连接成功,采集开始 ");
        }

        // ch:取图前的必要操作步骤 | en:Necessary operation before grab
        private Int32 NecessaryOperBeforeGrab()
        {
            // ch:取图像宽 | en:Get Iamge Width
            MyCamera.MVCC_INTVALUE_EX stWidth = new MyCamera.MVCC_INTVALUE_EX();
            int nRet = m_MyCamera.MV_CC_GetIntValueEx_NET("Width", ref stWidth);
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Get Width Info Fail!", nRet);
                return nRet;
            }
            // ch:取图像高 | en:Get Iamge Height
            MyCamera.MVCC_INTVALUE_EX stHeight = new MyCamera.MVCC_INTVALUE_EX();
            nRet = m_MyCamera.MV_CC_GetIntValueEx_NET("Height", ref stHeight);
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Get Height Info Fail!", nRet);
                return nRet;
            }
            // ch:取像素格式 | en:Get Pixel Format
            MyCamera.MVCC_ENUMVALUE stPixelFormat = new MyCamera.MVCC_ENUMVALUE();
            nRet = m_MyCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref stPixelFormat);
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Get Pixel Format Fail!", nRet);
                return nRet;
            }

            // ch:设置bitmap像素格式，申请相应大小内存 | en:Set Bitmap Pixel Format, alloc memory
            if ((Int32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined == (Int32)stPixelFormat.nCurValue)
            {
                ShowErrorMsg("Unknown Pixel Format!", MyCamera.MV_E_UNKNOW);
                return MyCamera.MV_E_UNKNOW;
            }
            else if (IsMono(stPixelFormat.nCurValue))
            {
                m_bitmapPixelFormat = PixelFormat.Format8bppIndexed;

                if (IntPtr.Zero != m_ConvertDstBuf)
                {
                    Marshal.Release(m_ConvertDstBuf);
                    m_ConvertDstBuf = IntPtr.Zero;
                }

                // Mono8为单通道
                m_nConvertDstBufLen = (UInt32)(stWidth.nCurValue * stHeight.nCurValue);
                m_ConvertDstBuf = Marshal.AllocHGlobal((Int32)m_nConvertDstBufLen);
                if (IntPtr.Zero == m_ConvertDstBuf)
                {
                    ShowErrorMsg("Malloc Memory Fail!", MyCamera.MV_E_RESOURCE);
                    return MyCamera.MV_E_RESOURCE;
                }
            }
            else
            {
                m_bitmapPixelFormat = PixelFormat.Format24bppRgb;

                if (IntPtr.Zero != m_ConvertDstBuf)
                {
                    Marshal.FreeHGlobal(m_ConvertDstBuf);
                    m_ConvertDstBuf = IntPtr.Zero;
                }

                // RGB为三通道
                m_nConvertDstBufLen = (UInt32)(3 * stWidth.nCurValue * stHeight.nCurValue);
                m_ConvertDstBuf = Marshal.AllocHGlobal((Int32)m_nConvertDstBufLen);
                if (IntPtr.Zero == m_ConvertDstBuf)
                {
                    ShowErrorMsg("Malloc Memory Fail!", MyCamera.MV_E_RESOURCE);
                    return MyCamera.MV_E_RESOURCE;
                }
            }

            // 确保释放保存了旧图像数据的bitmap实例，用新图像宽高等信息new一个新的bitmap实例
            if (null != m_bitmap)
            {
                m_bitmap.Dispose();
                m_bitmap = null;
            }
            m_bitmap = new Bitmap((Int32)stWidth.nCurValue, (Int32)stHeight.nCurValue, m_bitmapPixelFormat);

            // ch:Mono8格式，设置为标准调色板 | en:Set Standard Palette in Mono8 Format
            if (PixelFormat.Format8bppIndexed == m_bitmapPixelFormat)
            {
                ColorPalette palette = m_bitmap.Palette;
                for (int i = 0; i < palette.Entries.Length; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                m_bitmap.Palette = palette;
            }

            return MyCamera.MV_OK;
        }

        /// <summary>
        /// ch:像素类型是否为Mono格式 | en:If Pixel Type is Mono
        /// </summary>
        /// <param name="enPixelType"></param>
        /// <returns></returns>
        private Boolean IsMono(UInt32 enPixelType)
        {
            switch (enPixelType)
            {
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono1p:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono2p:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono4p:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8_Signed:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono14:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono16:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 相机像素格式转换及显示线程
        /// </summary>
        public void HikCaremaReceiveThreadProcess()
        {
            MyCamera.MV_FRAME_OUT stFrameInfo = new MyCamera.MV_FRAME_OUT();
            MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();
            MyCamera.MV_PIXEL_CONVERT_PARAM stConvertInfo = new MyCamera.MV_PIXEL_CONVERT_PARAM();
            int nRet = MyCamera.MV_OK;

            while (m_bGrabbing)
            {
                nRet = m_MyCamera.MV_CC_GetImageBuffer_NET(ref stFrameInfo, 1000);
                if (nRet == MyCamera.MV_OK)
                {
                    lock (BufForDriverLock)
                    {
                        if (m_BufForDriver == IntPtr.Zero || stFrameInfo.stFrameInfo.nFrameLen > m_nBufSizeForDriver)
                        {
                            if (m_BufForDriver != IntPtr.Zero)
                            {
                                Marshal.Release(m_BufForDriver);
                                m_BufForDriver = IntPtr.Zero;
                            }

                            m_BufForDriver = Marshal.AllocHGlobal((Int32)stFrameInfo.stFrameInfo.nFrameLen);
                            if (m_BufForDriver == IntPtr.Zero)
                            {
                                return;
                            }
                            m_nBufSizeForDriver = stFrameInfo.stFrameInfo.nFrameLen;
                        }

                        m_stFrameInfo = stFrameInfo.stFrameInfo;
                        CopyMemory(m_BufForDriver, stFrameInfo.pBufAddr, stFrameInfo.stFrameInfo.nFrameLen);

                        // ch:转换像素格式 | en:Convert Pixel Format
                        stConvertInfo.nWidth = stFrameInfo.stFrameInfo.nWidth;
                        stConvertInfo.nHeight = stFrameInfo.stFrameInfo.nHeight;
                        stConvertInfo.enSrcPixelType = stFrameInfo.stFrameInfo.enPixelType;
                        stConvertInfo.pSrcData = stFrameInfo.pBufAddr;
                        stConvertInfo.nSrcDataLen = stFrameInfo.stFrameInfo.nFrameLen;
                        stConvertInfo.pDstBuffer = m_ConvertDstBuf;
                        stConvertInfo.nDstBufferSize = m_nConvertDstBufLen;
                        if (PixelFormat.Format8bppIndexed == m_bitmap.PixelFormat)
                        {
                            stConvertInfo.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                            m_MyCamera.MV_CC_ConvertPixelType_NET(ref stConvertInfo);
                        }
                        else
                        {
                            stConvertInfo.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                            m_MyCamera.MV_CC_ConvertPixelType_NET(ref stConvertInfo);
                        }

                        // ch:保存Bitmap数据 | en:Save Bitmap Data
                        BitmapData bitmapData = m_bitmap.LockBits(new Rectangle(0, 0, stConvertInfo.nWidth, stConvertInfo.nHeight), ImageLockMode.ReadWrite, m_bitmap.PixelFormat);
                        CopyMemory(bitmapData.Scan0, stConvertInfo.pDstBuffer, (UInt32)(bitmapData.Stride * m_bitmap.Height));
                        m_bitmap.UnlockBits(bitmapData);
                        //绘制参考线
                        if (guidelineToolStripMenuItem.Checked)
                        {
                            DrawCrossLine();
                        }
                        // === 如需显示处理结果，可通过 Invoke 设置 UI ===
                        this.Invoke(new Action(() =>
                        {
                            if (m_bitmap != null)
                            {
                                // ✅ 关键：生成安全副本（与 SDK 内存分离）  适应 PictureBox 大小
                                Bitmap safeBitmap = new Bitmap(m_bitmap, pictureBox1.Size);
                                // Bitmap safeBitmap1 = new Bitmap(safeBitmap, new System.Drawing.Size(1024 * _zoomFactor, 768 * _zoomFactor));

                                //int newWidth = (int)(_originalBitmap.Width * _zoomFactor);
                                //int newHeight = (int)(_originalBitmap.Height * _zoomFactor);
                                // 清理旧图像，防止内存泄漏

                                if (!RunFlag)
                                {
                                    if (pictureBox1.Image != null)
                                        pictureBox1.Image.Dispose();
                                    //safeBitmap = RotateImage(safeBitmap, Goal_rotationAngle);
                                    //safeBitmap = TranslateImage(safeBitmap, _zoomFactor);
                                    //safeBitmap = TranslateImageVertically(safeBitmap, Goal_MoveCount);
                                    safeBitmap = ImageAlignment.AlignToTemplate(safeBitmap);
                                    pictureBox1.Image = safeBitmap;
                                }
                                else
                                {
                                    //if (pictureBox1.Image != null)
                                    //    pictureBox1.Image.Dispose();
                                    //safeBitmap = ImageAlignment.AlignToTemplate(safeBitmap);
                                    // pictureBox1.Image = safeBitmap;
                                    // pictureBox1.Refresh();

                                    logHelper.AppendLog("INFO: 程序開始處理步驟啓動 原幀鎖定 ");
                                    //bmp = ImageAlignment.AlignToTemplate(bmp);
                                    Detect(safeBitmap);
                                }
                                ///樣本采集
                                if (Collection)
                                {
                                    save.SaveFrame(safeBitmap);
                                }

                                //pictureBox1.Image = m_bitmap;
                            }
                        }));
                    }

                    //stDisplayInfo.hWnd = displayHandle;
                    //stDisplayInfo.pData = stFrameInfo.pBufAddr;
                    //stDisplayInfo.nDataLen = stFrameInfo.stFrameInfo.nFrameLen;
                    //stDisplayInfo.nWidth = stFrameInfo.stFrameInfo.nWidth;
                    //stDisplayInfo.nHeight = stFrameInfo.stFrameInfo.nHeight;
                    //stDisplayInfo.enPixelType = stFrameInfo.stFrameInfo.enPixelType;
                    //m_MyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);

                    m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameInfo);
                    //if (rois.Count > 0)
                    //{
                    //    this.BeginInvoke(new Action(() => ShowAllROIs()));
                    //}
                }
                else
                {
                    logHelper.AppendLog("Error: 相机连接失败");
                }
            }
        }
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="safeBitmap"></param>
        private void Detect(Bitmap safeBitmap)
        {
            var result = detector.Detect(safeBitmap);
            rois?.Clear();
            foreach (var item in result)
            {
                var roi = new ScriptROI();
                roi.Rect = item.Box;
                roi.msg = $"{item.ClassId}->{item.Confidence}";
                rois.Add(roi);
            }
        }

        /// <summary>
        /// 圖片選擇角度
        /// </summary>
        /// <param name="img"></param>
        /// <param name="rotationAngle"></param>
        /// <returns></returns>

        public Bitmap RotateImage(Bitmap img, float rotationAngle)
        {
            if (rotationAngle == 0)
            {
                return img;
            }
            // 创建一个新的空白位图以放置旋转后的图像
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // 创建一个绘图对象
            Graphics gfx = Graphics.FromImage(bmp);

            // 设置旋转点为中心
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            // 旋转指定的角度
            gfx.RotateTransform(rotationAngle);

            // 将坐标系平移到中心
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            // 绘制原始图像到新的位置
            gfx.DrawImage(img, new System.Drawing.Point(0, 0));

            return bmp;
        }

        /// <summary>
        /// 左右移動
        /// </summary>
        /// <param name="img"></param>
        /// <param name="offsetX"></param>
        /// <returns></returns>
        public Bitmap TranslateImage(Bitmap img, int offsetX)
        {
            if (offsetX == 0)
            {
                return img;
            }
            // 创建一个新的空白位图以放置平移后的图像
            Bitmap bmp = new Bitmap(img.Width + Math.Abs(offsetX), img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // 创建一个绘图对象
            Graphics gfx = Graphics.FromImage(bmp);

            // 平移指定的X轴距离
            gfx.TranslateTransform(offsetX, 0);

            // 绘制原始图像到新的位置
            gfx.DrawImage(img, new System.Drawing.Point(0, 0));

            // 释放绘图资源
            gfx.Dispose();

            return bmp;
        }

        /// <summary>
        /// 上下移動
        /// </summary>
        /// <param name="img"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public Bitmap TranslateImageVertically(Bitmap img, int offsetY)
        {
            if (offsetY == 0)
            {
                return img;
            }
            // 创建一个新的空白位图以放置平移后的图像
            Bitmap bmp = new Bitmap(img.Width, img.Height + Math.Abs(offsetY));
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // 创建一个绘图对象
            Graphics gfx = Graphics.FromImage(bmp);

            // 平移指定的Y轴距离
            gfx.TranslateTransform(0, offsetY);

            // 绘制原始图像到新的位置
            gfx.DrawImage(img, new System.Drawing.Point(0, 0));

            // 释放绘图资源
            gfx.Dispose();

            return bmp;
        }

        // 在缓冲上画十字参考线，直接用整数判断像素类型
        // 修改绘制函数，直接接受枚举类型
        private void DrawCrossLine()
        {
            // 在 Bitmap 上绘制参考线
            using (Graphics g = Graphics.FromImage(m_bitmap))
            {
                int w = m_bitmap.Width;
                int h = m_bitmap.Height;

                // 设置抗锯齿绘图参数
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 定义画笔
                using (Pen pen = new Pen(Color.Lime, 2))
                {
                    // 绘制中心十字线
                    g.DrawLine(pen, w / 2, 0, w / 2, h);
                    g.DrawLine(pen, 0, h / 2, w, h / 2);

                    // 例如可以再画外边框
                    g.DrawRectangle(pen, 0, 0, w - 1, h - 1);
                }
            }
        }

        /// windows 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FrmMaster_Load(object sender, EventArgs e)
        {
            //var frm = new FrmResult(ResultEnum.Fail, 3); // 3 秒后关闭
            //frm.Show(); // 或 ShowDialog()
            //await frm.CloseMeAsync(); // 异步等待自动关闭
            // ch: 初始化 SDK | en: Initialize SDK
            MyCamera.MV_CC_Initialize_NET();

            // ch: 枚举设备 | en: Enum Device List
            DeviceListAcq();
        }

        /// <summary>
        /// 相机设备列表查询
        /// </summary>
        private void DeviceListAcq()
        {
            // ch:创建设备列表 | en:Create Device List
            System.GC.Collect();
            cbDeviceList.Items.Clear();
            m_stDeviceList.nDeviceNum = 0;
            //这里枚举了所有类型，根据实际情况，选择合适的枚举类型即可
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE | MyCamera.MV_GENTL_GIGE_DEVICE
                | MyCamera.MV_GENTL_CAMERALINK_DEVICE | MyCamera.MV_GENTL_CXP_DEVICE | MyCamera.MV_GENTL_XOF_DEVICE, ref m_stDeviceList);
            if (0 != nRet)
            {
                ShowErrorMsg("Enumerate devices fail!", 0);
                return;
            }

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                string strUserDefinedName = "";
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO_EX gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO_EX)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO_EX));

                    if ((gigeInfo.chUserDefinedName.Length > 0) && (gigeInfo.chUserDefinedName[0] != '\0'))
                    {
                        if (MyCamera.IsTextUTF8(gigeInfo.chUserDefinedName))
                        {
                            strUserDefinedName = Encoding.UTF8.GetString(gigeInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        else
                        {
                            strUserDefinedName = Encoding.Default.GetString(gigeInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        cbDeviceList.Items.Add("GEV: " + DeleteTail(strUserDefinedName) + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    MyCamera.MV_USB3_DEVICE_INFO_EX usbInfo = (MyCamera.MV_USB3_DEVICE_INFO_EX)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO_EX));

                    if ((usbInfo.chUserDefinedName.Length > 0) && (usbInfo.chUserDefinedName[0] != '\0'))
                    {
                        if (MyCamera.IsTextUTF8(usbInfo.chUserDefinedName))
                        {
                            strUserDefinedName = Encoding.UTF8.GetString(usbInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        else
                        {
                            strUserDefinedName = Encoding.Default.GetString(usbInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        cbDeviceList.Items.Add("U3V: " + DeleteTail(strUserDefinedName) + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_GENTL_CAMERALINK_DEVICE)
                {
                    MyCamera.MV_CML_DEVICE_INFO CMLInfo = (MyCamera.MV_CML_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stCMLInfo, typeof(MyCamera.MV_CML_DEVICE_INFO));

                    if ((CMLInfo.chUserDefinedName.Length > 0) && (CMLInfo.chUserDefinedName[0] != '\0'))
                    {
                        if (MyCamera.IsTextUTF8(CMLInfo.chUserDefinedName))
                        {
                            strUserDefinedName = Encoding.UTF8.GetString(CMLInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        else
                        {
                            strUserDefinedName = Encoding.Default.GetString(CMLInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        cbDeviceList.Items.Add("CML: " + DeleteTail(strUserDefinedName) + " (" + CMLInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("CML: " + CMLInfo.chManufacturerInfo + " " + CMLInfo.chModelName + " (" + CMLInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_GENTL_CXP_DEVICE)
                {
                    MyCamera.MV_CXP_DEVICE_INFO CXPInfo = (MyCamera.MV_CXP_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stCXPInfo, typeof(MyCamera.MV_CXP_DEVICE_INFO));

                    if ((CXPInfo.chUserDefinedName.Length > 0) && (CXPInfo.chUserDefinedName[0] != '\0'))
                    {
                        if (MyCamera.IsTextUTF8(CXPInfo.chUserDefinedName))
                        {
                            strUserDefinedName = Encoding.UTF8.GetString(CXPInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        else
                        {
                            strUserDefinedName = Encoding.Default.GetString(CXPInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        cbDeviceList.Items.Add("CXP: " + DeleteTail(strUserDefinedName) + " (" + CXPInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("CXP: " + CXPInfo.chManufacturerInfo + " " + CXPInfo.chModelName + " (" + CXPInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_GENTL_XOF_DEVICE)
                {
                    MyCamera.MV_XOF_DEVICE_INFO XOFInfo = (MyCamera.MV_XOF_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stXoFInfo, typeof(MyCamera.MV_XOF_DEVICE_INFO));

                    if ((XOFInfo.chUserDefinedName.Length > 0) && (XOFInfo.chUserDefinedName[0] != '\0'))
                    {
                        if (MyCamera.IsTextUTF8(XOFInfo.chUserDefinedName))
                        {
                            strUserDefinedName = Encoding.UTF8.GetString(XOFInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        else
                        {
                            strUserDefinedName = Encoding.Default.GetString(XOFInfo.chUserDefinedName).TrimEnd('\0');
                        }
                        cbDeviceList.Items.Add("XOF: " + DeleteTail(strUserDefinedName) + " (" + XOFInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("XOF: " + XOFInfo.chManufacturerInfo + " " + XOFInfo.chModelName + " (" + XOFInfo.chSerialNumber + ")");
                    }
                }
            }

            // ch:选择第一项 | en:Select the first item
            if (m_stDeviceList.nDeviceNum != 0)
            {
                cbDeviceList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 删除字符串尾部的无效字符
        /// </summary>
        /// <param name="strUserDefinedName"></param>
        /// <returns></returns>
        private string DeleteTail(string strUserDefinedName)
        {
            strUserDefinedName = Regex.Unescape(strUserDefinedName);
            int nIndex = strUserDefinedName.IndexOf("\0");
            if (nIndex >= 0)
            {
                strUserDefinedName = strUserDefinedName.Remove(nIndex);
            }

            return strUserDefinedName;
        }

        /// <summary>
        /// ch:显示错误信息 | en:Show error message
        /// </summary>
        /// <param name="csMessage"></param>
        /// <param name="nErrorNum"></param>
        private void ShowErrorMsg(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " No data "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case MyCamera.MV_E_NETER: errorMsg += " Network error "; break;
            }

            this.ShowErrorDialog("PROMPT", errorMsg);
        }

        //##
        //        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        //        {
        //            if (e.Button == MouseButtons.Left)
        //            {
        //                if (NewScriptFlag)
        //                {
        //                    isDrawing = true;
        //                    startPoint = e.Location;
        //                    currentROI = new ScriptROI { Name = $"ROI_{roiCounter++}" };
        //                }
        //            }
        //        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 1️⃣ 先检查是否点击了某个 ROI 的缩放手柄
                foreach (var roi in rois)
                {
                    var handle = GetHandleAtPoint(roi, e.Location);
                    if (handle != ResizeHandle.None)
                    {
                        isResizingRoi = true;
                        resizingRoi = roi;
                        activeHandle = handle;
                        lastMousePos = e.Location;
                        return;
                    }
                }

                // 2️⃣ 再检查是否点击了 ROI 主体（用于拖动）
                var clickedRoi = rois.FirstOrDefault(r => r.Rect.Contains(e.Location));
                if (clickedRoi != null)
                {
                    isDraggingRoi = true;
                    draggingRoi = clickedRoi;
                    dragOffset = new System.Drawing.Point(e.X - draggingRoi.Rect.X, e.Y - draggingRoi.Rect.Y);
                    return;
                }

                // 3️⃣ 否则尝试绘制新 ROI
                if (NewScriptFlag)
                {
                    isDrawing = true;
                    startPoint = e.Location;
                    currentROI = new ScriptROI { Name = $"ROI_{roiCounter++}" };
                }
            }
        }

        //private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isDrawing && currentROI != null)
        //    {
        //        int x = Math.Min(startPoint.X, e.X);
        //        int y = Math.Min(startPoint.Y, e.Y);
        //        int w = Math.Abs(e.X - startPoint.X);
        //        int h = Math.Abs(e.Y - startPoint.Y);
        //        currentROI.Rect = new Rectangle(x, y, w, h);
        //        pictureBox1.Invalidate();
        //    }
        //}
        //private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isDraggingRoi && draggingRoi != null)
        //    {
        //        // 计算新的 ROI 位置（保持 dragOffset 不变）
        //        int newX = e.X - dragOffset.X;
        //        int newY = e.Y - dragOffset.Y;

        //        // 可选：限制 ROI 不超出图片边界
        //        // newX = Math.Max(0, Math.Min(newX, pictureBox1.ClientSize.Width - draggingRoi.Rect.Width));
        //        // newY = Math.Max(0, Math.Min(newY, pictureBox1.ClientSize.Height - draggingRoi.Rect.Height));

        //        draggingRoi.Rect = new Rectangle(newX, newY, draggingRoi.Rect.Width, draggingRoi.Rect.Height);
        //        pictureBox1.Invalidate();
        //    }
        //    else if (isDrawing && currentROI != null)
        //    {
        //        int x = Math.Min(startPoint.X, e.X);
        //        int y = Math.Min(startPoint.Y, e.Y);
        //        int w = Math.Abs(e.X - startPoint.X);
        //        int h = Math.Abs(e.Y - startPoint.Y);
        //        currentROI.Rect = new Rectangle(x, y, w, h);
        //        pictureBox1.Invalidate();
        //    }
        //}
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // 缩放模式
            if (isResizingRoi && resizingRoi != null)
            {
                var rect = resizingRoi.Rect;
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;

                int newX = rect.X, newY = rect.Y, newWidth = rect.Width, newHeight = rect.Height;

                switch (activeHandle)
                {
                    case ResizeHandle.TopLeft:
                        newX += dx; newWidth -= dx;
                        newY += dy; newHeight -= dy;
                        break;

                    case ResizeHandle.TopRight:
                        newWidth += dx;
                        newY += dy; newHeight -= dy;
                        break;

                    case ResizeHandle.BottomLeft:
                        newX += dx; newWidth -= dx;
                        newHeight += dy;
                        break;

                    case ResizeHandle.BottomRight:
                        newWidth += dx;
                        newHeight += dy;
                        break;
                }

                // 确保宽度/高度 >= 最小值（比如 10）
                if (newWidth < 10) { newWidth = 10; if (activeHandle == ResizeHandle.TopLeft || activeHandle == ResizeHandle.BottomLeft) newX = rect.Right - newWidth; }
                if (newHeight < 10) { newHeight = 10; if (activeHandle == ResizeHandle.TopLeft || activeHandle == ResizeHandle.TopRight) newY = rect.Bottom - newHeight; }

                // 可选：限制不能拖出 PictureBox 边界
                newX = Math.Max(0, newX);
                newY = Math.Max(0, newY);
                newWidth = Math.Min(newWidth, pictureBox1.ClientSize.Width - newX);
                newHeight = Math.Min(newHeight, pictureBox1.ClientSize.Height - newY);

                resizingRoi.Rect = new Rectangle(newX, newY, newWidth, newHeight);
                lastMousePos = e.Location;
                pictureBox1.Invalidate();
                return;
            }

            // 拖动模式
            if (isDraggingRoi && draggingRoi != null)
            {
                int newX = e.X - dragOffset.X;
                int newY = e.Y - dragOffset.Y;
                draggingRoi.Rect = new Rectangle(newX, newY, draggingRoi.Rect.Width, draggingRoi.Rect.Height);
                pictureBox1.Invalidate();
                return;
            }

            // 绘制模式
            if (isDrawing && currentROI != null)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int w = Math.Abs(e.X - startPoint.X);
                int h = Math.Abs(e.Y - startPoint.Y);
                currentROI.Rect = new Rectangle(x, y, w, h);
                pictureBox1.Invalidate();
            }

            // 👆 可选：改变鼠标样式（悬停在手柄上时显示 SizeNWSE 等）
        }

        //private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (isDraggingRoi)
        //    {
        //        isDraggingRoi = false;
        //        draggingRoi = null;
        //        pictureBox1.Invalidate();
        //        return;
        //    }

        //    if (isDrawing && currentROI != null)
        //    {
        //        isDrawing = false;
        //        if (currentROI.Rect.Width > 10 && currentROI.Rect.Height > 10)
        //        {
        //            rois.Add(currentROI);
        //        }
        //        currentROI = null;
        //        pictureBox1.Invalidate();
        //    }
        //}
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isResizingRoi)
            {
                isResizingRoi = false;
                resizingRoi = null;
                activeHandle = ResizeHandle.None;
                pictureBox1.Invalidate();
                return;
            }

            if (isDraggingRoi)
            {
                isDraggingRoi = false;
                draggingRoi = null;
                pictureBox1.Invalidate();
                return;
            }

            if (isDrawing && currentROI != null)
            {
                isDrawing = false;
                if (currentROI.Rect.Width > 10 && currentROI.Rect.Height > 10)
                {
                    rois.Add(currentROI);
                }
                currentROI = null;
                pictureBox1.Invalidate();
            }
        }

        //private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (isDrawing && currentROI != null)
        //    {
        //        isDrawing = false;
        //        if (currentROI.Rect.Width > 10 && currentROI.Rect.Height > 10)
        //        {
        //            rois.Add(currentROI);
        //        }
        //        currentROI = null;
        //        pictureBox1.Invalidate();
        //    }
        //}

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 查找是否点击到了某个ROI
                var roi = rois.FirstOrDefault(r => r.Rect.Contains(e.Location));
                if (roi != null)
                {
                    // 将 StepMenuStrip 作为上下文菜单，在鼠标位置显示
                    selectedRoi = roi;
                    StepMenuStrip.Show(pictureBox1, e.Location);
                }
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var pen = new Pen(Color.Blue, 2))

            using (var semiTransBrush = new SolidBrush(Color.FromArgb(60, Color.Lime)))
            using (var font = new Font("Arial", 10, FontStyle.Bold))
            {
                // 绘制所有已存在的ROI
                // 绘制所有已存在的 ROI
                foreach (var roi in rois)
                {
                    // 设置画笔颜色（注意：你这里用了两个 pen，建议优化）
                    using (var pen1 = new Pen(roi.pen_color, 2))
                    {
                        g.DrawRectangle(pen1, roi.Rect);

                        // 计算文字位置：在矩形上方，垂直居中对齐
                        var textSizes = g.MeasureString($"{roi.Name} {roi.msg}", font);  // 测量文字大小
                        float textX = roi.Rect.X;                          // 文字从矩形左侧开始
                        float textY = roi.Rect.Y - textSizes.Height - 2;    // 在矩形上方，留 2 像素间距

                        // 可选：绘制一个浅色背景，提高文字可读性
                        using (var bgBrush = new SolidBrush(Color.FromArgb(180, Color.White)))
                        {
                            g.FillRectangle(bgBrush, textX, textY, textSizes.Width, textSizes.Height);
                        }

                        // 绘制文字
                        g.DrawString($"{roi.Name} {roi.msg}", font, roi.Brushes_color, new PointF(textX, textY));
                        // ... 已有繪製矩形和文字的代碼 ...

                        // 畫中心 "+"
                        var center = new System.Drawing.Point(roi.Rect.X + roi.Rect.Width / 2, roi.Rect.Y + roi.Rect.Height / 2);
                        int crossSize = 5; // 十字長度

                        g.DrawLine(Pens.Red, center.X - crossSize, center.Y, center.X + crossSize, center.Y); // 橫線
                        g.DrawLine(Pens.Red, center.X, center.Y - crossSize, center.X, center.Y + crossSize); // 直線
                    }
                }

                // 绘制当前正在绘制的ROI
                if (isDrawing && currentROI != null)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(60, Color.Red)), currentROI.Rect);
                    g.DrawRectangle(Pens.Red, currentROI.Rect);
                }
            }
            // 初始化字体和画刷
            var font1 = new Font("Arial", 12, FontStyle.Bold);
            var brush = new SolidBrush(Color.White);
            // 获取当前时间字符串
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 计算文本的尺寸
            SizeF textSize = e.Graphics.MeasureString(currentTime, font1);

            // 计算文本的位置（右下角）
            float x = pictureBox1.ClientSize.Width - textSize.Width - 5; // 5是边距
            float y = pictureBox1.ClientSize.Height - textSize.Height - 5; // 5是边距

            // 绘制文本
            e.Graphics.DrawString(currentTime, font1, brush, x, y);
            // 绘制所有 ROI 的缩放手柄（仅当不是正在绘制新 ROI 时）
            if (!isDrawing)
            {
                //var g = e.Graphics;
                foreach (var roi in rois)
                {
                    // 四个角的手柄
                    var handles = new[] { ResizeHandle.TopLeft, ResizeHandle.TopRight, ResizeHandle.BottomLeft, ResizeHandle.BottomRight };
                    foreach (var handle in handles)
                    {
                        var handleRect = GetHandleRect(roi.Rect, handle);
                        g.FillRectangle(Brushes.White, handleRect);      // 白色填充
                        g.DrawRectangle(Pens.Black, handleRect);         // 黑边框
                    }
                }
            }
        }

        /// <summary>
        /// 打开系统文件夹
        /// </summary>
        /// <param name="path"></param>
        public void OpenSystemFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 使用系统关联的程序打开路径
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 图片污点检查
        /// </summary>
        /// <param name="image"></param>
        /// <param name="thresholdValue"></param>
        /// <param name="minArea"></param>
        /// <returns></returns>
        public List<OpenCvSharp.Rect> DetectDirt(Mat image, int thresholdValue = 60, int minArea = 100)
        {
            // 转换为灰度图像
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            // 使用高斯模糊降噪
            Mat blurred = new Mat();
            Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 0);

            // 应用阈值处理
            Mat thresh = new Mat();
            Cv2.Threshold(blurred, thresh, thresholdValue, 255, ThresholdTypes.BinaryInv);

            // 查找轮廓
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            var dirtRegions = new List<OpenCvSharp.Rect>();

            for (int i = 0; i < contours.Length; i++)
            {
                double area = Cv2.ContourArea(contours[i]);
                if (area > minArea) // 过滤掉太小的区域
                {
                    OpenCvSharp.Rect rect = Cv2.BoundingRect(contours[i]);
                    dirtRegions.Add(rect);
                }
            }

            return dirtRegions;
        }

        /// <summary>
        /// 加载窗体配置
        /// </summary>
        private void LoadControllerSetting()
        {
            //参考线
            this.guidelineToolStripMenuItem.Checked = Properties.Settings.Default.guide_line;
        }

        /// <summary>
        /// 保存窗体配置
        /// </summary>
        private void SaveControllerSetting()
        {
            //参考线
            Properties.Settings.Default.guide_line = this.guidelineToolStripMenuItem.Checked;
            Properties.Settings.Default.Save(); // 保存到用户配置文件
        }

        /// <summary>
        /// 执行处理步骤
        /// </summary>
        private bool runProcessStep(Bitmap _bitmap, ScriptROI _withRoi)
        {
            var _bitmap_with = _bitmap;
            if (!(_withRoi.step_script is null) && _withRoi.step_script.Count > 0)
            {
                List<(int step, string text)> list = new List<(int step, string text)>();
                for (int i = 0; i < _withRoi.step_script.Count; i++)
                {
                    var itemString = _withRoi.step_script[i].ToString();
                    var fn = itemString.Split("->")[0];
                    switch (fn)
                    {
                        //OCR Function
                        case "YS101":
                            {
                                var result = BitmapProcessorServices.OCRFn(_bitmap_with);
                                //var result = PaddleOCRHelper.Recognize(_bitmap_with);
                                //logHelper.AppendLog($"INFO :OCR Data:{result}");
                                list.Add((i, result));
                                _withRoi.msg = result;
                                // cv2.Show(_bitmap_with);
                                pictureBox2.Invoke(new Action(() =>
                                {
                                    if (!(pictureBox2.Image is null))
                                    {
                                        pictureBox2.Image.Dispose();
                                    }
                                    pictureBox2.Image = (_bitmap_with);
                                    pictureBox2.Refresh();
                                }));
                                logHelper.AppendLog($"INFO :Step{i} OCR 文字提取 OCR Data:{result}");
                                break;
                            }
                        //文字比对
                        case "YS102":
                            {
                                var step = int.Parse(itemString.Split("->")[3].ToString());
                                var base_string = itemString.Split("->")[4].ToString().Trim();
                                var ocr_string = list.Where(x => x.step == step).FirstOrDefault().text;
                                logHelper.AppendLog($"INFO :Step{i} OCR 文字提取 OCR Data:{ocr_string}");
                                logHelper.AppendLog($"INFO :Step{i} OCR 文字比对 OCR Data:{base_string}");
                                logHelper.AppendLog($"INFO :Step{i} BaseLen:{base_string.Length} OCRLen:{ocr_string?.Length}");

                                if (base_string == ocr_string)
                                {
                                    _withRoi.msg = "内容一致";
                                    logHelper.AppendLog($"SUCCESS:{base_string}");
                                }
                                else
                                {
                                    _withRoi.msg = "内容不一致";
                                    logHelper.AppendLog($"ERROR:文字比对失败，内容不一致:{ocr_string}");
                                    return false;
                                }

                                break;
                            }
                        case "YS103":  //图像反色
                            {
                                _bitmap_with = BitmapProcessorServices.Invert(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像反色处理完成");
                                //PictureRefresh();
                                break;
                            }
                        case "YS104":  //二值化
                            {
                                _bitmap_with = BitmapProcessorServices.Threshold(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像二值化处理完成");
                                //PictureRefresh();
                                break;
                            }
                        case "YS105":  //灰色边缘处理
                            {
                                _bitmap_with = BitmapProcessorServices.DetectEdges(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像灰色边缘处理完成");
                                //PictureRefresh();
                                break;
                            }
                        case "YS106":  //彩色边缘处理
                            {
                                _bitmap_with = BitmapProcessorServices.DetectEdgesColored(_bitmap_with);
                                logHelper.AppendLog($"INFO :Step{i} 图像彩色边缘处理完成");
                                // PictureRefresh();
                                break;
                            }

                        case "YS110":  //锐化处理
                            {
                                _bitmap_with = BitmapProcessorServices.EnhanceSharpness(_bitmap_with, 5);
                                logHelper.AppendLog($"INFO :Step{i} 图像理锐化处理完成");
                                break;
                            }
                        case "YS111": //图像二维码识别
                            {
                                bool result = (bool)txtSerialNumber.Invoke(new Func<bool>(() =>

                                     {
                                         QrcodeString = string.Empty;
                                         txtSerialNumber.Text = string.Empty;
                                         var text = BitmapProcessorServices.QR_Code(_bitmap_with);
                                         logHelper.AppendLog($"INFO :Step{i} 图像QR Code:Data{text}");
                                         if (text.flag)
                                         {
                                             _withRoi.msg = $"{text.txt}";
                                             txtSerialNumber.Text = text.txt;
                                             //MES 检测
                                             //
                                             QrcodeString = text.txt;
                                             return true;
                                         }
                                         else
                                         {
                                             logHelper.AppendLog($"ERROR: 无法识别二维码,{text.message}");
                                             return false;
                                         }
                                     }
                                     ));
                                return result;
                            }
                        case "YS112":////图片相似度比较
                            {
                                var threshold = double.Parse(itemString.Split("->")[3].ToString());
                                var path = itemString.Split("->")[4].ToString();
                                if (string.IsNullOrEmpty(path) && !File.Exists(path))
                                {
                                    logHelper.AppendLog($"ERROR :Step{i} 图片相似度比较 参考图片路径无效");
                                    return false;
                                }
                                string _basePath = System.Windows.Forms.Application.StartupPath + @"\" + Guid.NewGuid().ToString() + ".jpg";
                                _bitmap_with.Save(_basePath);
                                //var value = BitmapProcessorServices.CompareWithSIFT(_basePath, path);
                                var value = ImageSimilarityHelper.CompareImageSimilarityHybrid(_basePath, path);
                                if (File.Exists(_basePath))
                                {
                                    File.Delete(_basePath);
                                }
                                _withRoi.msg = $"相似度:{(value * 100).ToString("F2")}%";
                                if (value >= threshold)
                                {
                                    logHelper.AppendLog($"SUCCESS :Step{i} 图片相似度比较 结果:{value.ToString("F3")},阈值:{threshold}");
                                    return true;
                                }
                                else
                                {
                                    logHelper.AppendLog($"WARN :Step{i} 图片相似度比较 结果:{value.ToString("F3")},阈值:{threshold}");
                                    return false;
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
                                    _withRoi.msg = "✅拼寫正確";
                                    return true;
                                }
                                else
                                {
                                    logHelper.AppendLog($"❌ '{ocr_string}' 拼寫錯誤。");
                                    _withRoi.msg = "❌ 拼寫錯誤";
                                    var suggestions = SpellChecker.Suggest(ocr_string);
                                    if (suggestion)
                                    {
                                        _withRoi.msg = $"❌ 拼寫錯誤 建議: {string.Join(", ", suggestions)}";
                                        logHelper.AppendLog($"WARN: 建議: {string.Join(", ", suggestions)}");
                                    }
                                    return false;
                                }
                                break;
                            }
                        case "YS114":
                            {
                                var step = int.Parse(itemString.Split("->")[3].ToString());
                                var ocr_string = list.Where(x => x.step == step).FirstOrDefault().text;
                                if (string.IsNullOrEmpty(QrcodeString))
                                {
                                    logHelper.AppendLog("ERROR: QRCode 未识别");
                                    return false;
                                }
                                if (string.IsNullOrEmpty(ocr_string))
                                {
                                    logHelper.AppendLog("ERROR: 文字提取失败");
                                    return false;
                                }
                                if (ocr_string == QrcodeString)
                                {
                                    logHelper.AppendLog($"SUECCES: 同QR CODE 一致 {ocr_string}");
                                    _withRoi.msg = $"同QRCode 一致 {ocr_string}";
                                    return true;
                                }
                                logHelper.AppendLog("ERROR: 同QR Code 不一致");
                                _withRoi.msg = $"同QRCode 不一致 {ocr_string}";
                                return false;
                            }
                    }
                }
            }
            else
            {
                logHelper.AppendLog($"ERROR: {_withRoi.Name} 未设定处理脚本");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化ROI 檢查結果
        /// </summary>
        private void initRoIAndCheckResult()
        {
            foreach (var item in rois)
            {
                item.Brushes_color = Brushes.Blue;
                item.pen_color = Color.Blue;
                item.msg = string.Empty;
            }
            //pictureBox2.Image.Dispose();
            pictureBox2.Image = null;
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// 拼詞檢查模型初始化
        /// </summary>
        private void InitSpellChecker()
        {
            // 初始化（假設詞典文件在執行目錄下）
            string path = Path.Combine(Application.StartupPath, "Lib", "libreoffice");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string affFile = Path.Combine(path, "en_US.aff");
            string dicFile = Path.Combine(path, "en_US.dic");
            if (!File.Exists(affFile))
            {
                logHelper.AppendLog($"ERROR:No found file {affFile}");
                spellReadyFlag = false;
            }
            if (!File.Exists(dicFile))
            {
                logHelper.AppendLog($"ERROR:No found file {dicFile}");
                spellReadyFlag = false;
            }
            if (spellReadyFlag)
            {
                SpellChecker.Initialize(affFile, dicFile);
                uiLedBulb1.Color = Color.LimeGreen;
            }
        }

        /// <summary>
        /// OCR 初始化
        /// </summary>
        private void InitOCR()
        {
            //OCR
            // 假设 tessdata 文件夹在项目根目录下\Lib\tessdata
            _tessDataPath = Path.Combine(Application.StartupPath, "Lib", "tessdata");

            // 检查 tessdata 文件夹是否存在
            if (!Directory.Exists(this._tessDataPath))
            {
                Directory.CreateDirectory(this._tessDataPath);
            }
            var file_path = Path.Combine(this._tessDataPath, "chi_sim.traineddata");
            if (File.Exists(file_path))
            {
                OCRReadyFlag = true;
                uiLedBulb2.Color = Color.LimeGreen;
                BitmapProcessorServices._tessDataPath = _tessDataPath;
            }
        }

        /// <summary>
        /// 重载多语翻译
        /// </summary>
        public override void Translate()
        {
            //必须保留
            base.Translate();
            //读取翻译代码中的多语资源
            CodeTranslator.Load(this);
        }

        private void lauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _zoomFactor += 1;
        }

        private void 縮小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _zoomFactor -= 1;
        }

        private void 原圖ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _zoomFactor = 1;
            Goal_rotationAngle = 0;
            Goal_MoveCount = 0;
        }

        private void 角度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Goal_rotationAngle >= 180)
            {
                Goal_rotationAngle = 180;
            }
            else
            {
                Goal_rotationAngle += 0.5f;
            }
        }

        private void 角度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Goal_rotationAngle <= 0)
            {
                Goal_rotationAngle = 0;
            }
            else
            {
                Goal_rotationAngle -= 0.5f;
            }
        }

        private void 上移動ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Goal_MoveCount += 1;
        }

        private void 下移動ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Goal_MoveCount -= 1;
        }

        private void 目标ROI显示DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Collection = 目标ROI显示DToolStripMenuItem.Checked;
        }

        // 手柄大小（像素）
        private const int HANDLE_SIZE = 8;

        // 根据 ROI 和手柄类型，返回手柄的绘制区域
        private Rectangle GetHandleRect(Rectangle roiRect, ResizeHandle handle)
        {
            int x = 0, y = 0;
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    x = roiRect.X - HANDLE_SIZE / 2;
                    y = roiRect.Y - HANDLE_SIZE / 2;
                    break;

                case ResizeHandle.TopRight:
                    x = roiRect.Right - HANDLE_SIZE / 2;
                    y = roiRect.Y - HANDLE_SIZE / 2;
                    break;

                case ResizeHandle.BottomLeft:
                    x = roiRect.X - HANDLE_SIZE / 2;
                    y = roiRect.Bottom - HANDLE_SIZE / 2;
                    break;

                case ResizeHandle.BottomRight:
                    x = roiRect.Right - HANDLE_SIZE / 2;
                    y = roiRect.Bottom - HANDLE_SIZE / 2;
                    break;
            }
            return new Rectangle(x, y, HANDLE_SIZE, HANDLE_SIZE);
        }

        // 检查鼠标位置是否落在某个手柄上
        private ResizeHandle GetHandleAtPoint(ScriptROI roi, System.Drawing.Point point)
        {
            var rect = roi.Rect;
            if (GetHandleRect(rect, ResizeHandle.TopLeft).Contains(point)) return ResizeHandle.TopLeft;
            if (GetHandleRect(rect, ResizeHandle.TopRight).Contains(point)) return ResizeHandle.TopRight;
            if (GetHandleRect(rect, ResizeHandle.BottomLeft).Contains(point)) return ResizeHandle.BottomLeft;
            if (GetHandleRect(rect, ResizeHandle.BottomRight).Contains(point)) return ResizeHandle.BottomRight;
            return ResizeHandle.None;
        }
    }

    public class CodeTranslator : IniCodeTranslator<CodeTranslator>
    {
        public string CloseAskString { get; set; } = "您确认要退出程序吗？";
        public string Controls { get; set; } = "控件";
        public string Forms { get; set; } = "窗体";
        public string Charts { get; set; } = "图表";
        public string Industrial { get; set; } = "工控";
        public string Theme { get; set; } = "主题";
        public string Symbols { get; set; } = "字体图标";
        public string Colorful { get; set; } = "多彩主题";
    }
}