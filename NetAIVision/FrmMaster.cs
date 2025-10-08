using MvCamCtrl.NET;
using NetAIVision.Controller;
using NetAIVision.Model.FrmResult;
using NetAIVision.Model.ROI;
using NetAIVision.Services;
using NetAIVision.Services.MES;
using Sunny.UI;
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
using System.Windows.Forms;

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

        private PixelFormat m_bitmapPixelFormat = PixelFormat.DontCare;
        private IntPtr m_ConvertDstBuf = IntPtr.Zero;
        private UInt32 m_nConvertDstBufLen = 0;

        private IntPtr displayHandle = IntPtr.Zero;

        // 初始化日志控件
        private ConsoleStyleLogHelper logHelper;

        #region roi

        private List<ROI> rois = new List<ROI>(); // 保存ROI的列表
        private ROI currentROI = null;            // 当前正在绘制的ROI
        private bool isDrawing = false;           // 是否正在绘制
        private Point startPoint;                 // 起点
        private int roiCounter = 1;               // ROI编号计数

        #endregion roi

        public FrmMaster()
        {
            InitializeComponent();

            this.Load += FrmMaster_Load;
            // Open
            this.openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            this.openImagesToolStripMenuItem.Click += OpenImagesToolStripMenuItem_Click;
            this.openLogsToolStripMenuItem.Click += OpenLogsToolStripMenuItem_Click;

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
            logHelper = new ConsoleStyleLogHelper(richboxLogs, 100);
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
                ShowErrorMsg("Not Start Grabbing", 0);
                logHelper.AppendLog("WARN: Not Start Grabbing");
                return;
            }

            lock (BufForDriverLock)
            {
                if (m_stFrameInfo.nFrameLen == 0)
                {
                    ShowErrorMsg($"Save {extension.ToUpper()} Fail!", 0);
                    logHelper.AppendLog("WARN: Save {extension.ToUpper()} Fail!");
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
                    ShowErrorMsg($"Save {extension.ToUpper()} Fail!", nRet);
                    logHelper.AppendLog($"WARN: Save {extension.ToUpper()} Fail!");
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
                ShowErrorMsg("Stop Grabbing Fail!", nRet);
            }
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
                    this.ShowErrorDialog("没有输入人员工号");

                    return;
                }
                string message = string.Empty;
                bool checkFlag = MES_Service.CommandCheckUser(mlabEmp.Text, ref message);
                if (!checkFlag)
                {
                    RadioDebugMode.Checked = true;
                    this.ShowErrorDialog($"{message}");
                    return;
                }
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
            pictureBox1.Size = new Size(1024, 768);
            pictureBox1.Location = new Point(this.Width - 1028, 100);
            cbDeviceList.Width = 1024;
            cbDeviceList.Location = new Point(this.Width - 1028, 70);
            grouplogs.Height = this.Height - pictureBox1.Height - 100;
            groupSetting.Height = this.Height - groupSetting.Height - 16;
            groupSetting.Width = this.Width - pictureBox1.Width - 10;
            uiLine2.Width = groupSetting.Width - 10;
            logHelper.AppendLog("INFO: 程序启动");
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
                        ShowErrorMsg("Set Packet Size failed!", nRet);
                    }
                }
                else
                {
                    ShowErrorMsg("Get Packet Size failed!", nPacketSize);
                }
            }

            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            // ch:前置配置 | en:pre-operation

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
                ShowErrorMsg("Start Grabbing Fail!", nRet);
                return;
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
                    }

                    stDisplayInfo.hWnd = displayHandle;
                    stDisplayInfo.pData = stFrameInfo.pBufAddr;
                    stDisplayInfo.nDataLen = stFrameInfo.stFrameInfo.nFrameLen;
                    stDisplayInfo.nWidth = stFrameInfo.stFrameInfo.nWidth;
                    stDisplayInfo.nHeight = stFrameInfo.stFrameInfo.nHeight;
                    stDisplayInfo.enPixelType = stFrameInfo.stFrameInfo.enPixelType;
                    m_MyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);
                    m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameInfo);
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

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                startPoint = e.Location;
                currentROI = new ROI { Name = $"ROI_{roiCounter++}" };
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && currentROI != null)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int w = Math.Abs(e.X - startPoint.X);
                int h = Math.Abs(e.Y - startPoint.Y);
                currentROI.Rect = new Rectangle(x, y, w, h);
                pictureBox1.Invalidate();
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
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

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 查找是否点击到了某个ROI
                var roi = rois.FirstOrDefault(r => r.Rect.Contains(e.Location));
                if (roi != null)
                {
                    if (MessageBox.Show($"删除 {roi.Name} ?", "删除ROI", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        rois.Remove(roi);
                        pictureBox1.Invalidate();
                    }
                }
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var pen = new Pen(Color.Lime, 2))
            using (var semiTransBrush = new SolidBrush(Color.FromArgb(60, Color.Lime)))
            using (var font = new Font("Arial", 10, FontStyle.Bold))
            {
                // 绘制所有已存在的ROI
                foreach (var roi in rois)
                {
                    g.FillRectangle(semiTransBrush, roi.Rect);
                    g.DrawRectangle(pen, roi.Rect);
                    g.DrawString(roi.Name, font, Brushes.Yellow, roi.Rect.Location);
                }

                // 绘制当前正在绘制的ROI
                if (isDrawing && currentROI != null)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(60, Color.Red)), currentROI.Rect);
                    g.DrawRectangle(Pens.Red, currentROI.Rect);
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
    }
}