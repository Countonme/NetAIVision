using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Services.MES
{
    public class MES_Service
    {
        public MES_Service()
        {
        }

        /// <summary>
        /// 检查MES Connect .dll lib 是否存在
        /// </summary>
        /// <returns></returns>
        public static string CheckLib()
        {
            var path = Path.Combine(Application.StartupPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = Path.Combine(path, "SajetConnect.dll");
            if (File.Exists(fileName))
            {
                return string.Empty;
            }
            return $"No Found {fileName} \r\n lib 不全,请下载文件至Saject";
        }

        /// <summary>
        /// 打开MES 连接
        /// </summary>
        /// <returns></returns>
        public static bool MesConnect()
        {
            return _ = MesCommand.SajetTransStart();
        }

        /// <summary>
        /// 关闭MES 连接
        /// </summary>
        /// <returns></returns>
        public static bool MesDisConnect()
        {
            return _ = MesCommand.SajetTransClose();
        }

        /// <summary>
        /// mallocIntptr 轉指針 字符串转指针
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private static IntPtr mallocIntptr(string strData)
        {
            Byte[] btData = System.Text.Encoding.Default.GetBytes(strData);
            IntPtr m_ptr = Marshal.AllocHGlobal(1000);
            Byte[] btZero = new Byte[btData.Length + 1];
            Marshal.Copy(btData, 0, m_ptr, btData.Length);
            return m_ptr;
        }

        /// <summary>
        /// 将字符串转换为指针
        /// </summary>
        /// <param name="strData">要转换的字符串</param>
        /// <returns>指向字符串的指针</returns>
        //private static IntPtr mallocIntptr(string strData)
        //{
        //    if (strData == null) return IntPtr.Zero;

        //    // 获取字符串的字节数组（ANSI编码）
        //    byte[] btData = System.Text.Encoding.Default.GetBytes(strData);

        //    // 分配足够的内存：字节长度加1（包含结尾的空字符）
        //    IntPtr m_ptr = Marshal.AllocHGlobal(btData.Length + 1);

        //    // 将字节数组复制到分配的非托管内存
        //    Marshal.Copy(btData, 0, m_ptr, btData.Length);

        //    // 手动添加字符串结尾的空字符（0）
        //    Marshal.WriteByte(m_ptr, btData.Length, 0);

        //    return m_ptr;
        //}

        /// <summary>
        /// mallocIntptr 轉指針 int 转指针
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static IntPtr mallocIntptr(int length)
        {
            IntPtr m_ptr = Marshal.AllocHGlobal(4);
            int[] btZero = new int[1];
            btZero[0] = length;
            Marshal.Copy(btZero, 0, m_ptr, 1);
            return m_ptr;
        }

        ///// <summary>
        ///// 将整数转换为指针
        ///// </summary>
        ///// <param name="length">要转换的整数</param>
        ///// <returns>指向整数的指针</returns>
        //private static IntPtr mallocIntptr(int length)
        //{
        //    // 分配 4 字节内存
        //    IntPtr m_ptr = Marshal.AllocHGlobal(sizeof(int));

        //    // 将整数值直接写入分配的非托管内存
        //    Marshal.WriteInt32(m_ptr, length);

        //    return m_ptr;
        //}

        /// <summary>
        /// CallMesCommand
        /// </summary>
        /// <param name="Command">Command</param>
        /// <param name="sp_data">sp_data</param>
        /// <returns></returns>
        public static (bool flag, string receive) CallMesCommand(int Command, string sp_data)
        {
            try
            {
                // 轉指針
                IntPtr initData = mallocIntptr(sp_data);
                IntPtr initLength = mallocIntptr(sp_data.Length);
                // transData
                var flag = MesCommand.SajetTransData(Command, initData, initLength);
                // 獲取返回指針
                string resData = Marshal.PtrToStringAnsi(initData);
                int myi = Marshal.ReadInt32(initLength);
                if (myi > resData.Length)
                {
                    myi = resData.Length;
                }
                string receive = resData.Substring(0, myi);

                // 釋放指針
                Marshal.FreeHGlobal(initData);
                Marshal.FreeHGlobal(initLength);
                return (flag, receive);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// 工号检查
        /// </summary>
        /// <param name="Empyno">Empyno</param>
        /// <param name="message">ref message</param>
        /// <returns></returns>
        public static bool CommandCheckUser(string Empyno, ref string message)
        {
            ////工号检查
            var sp_data = Empyno + ";";

            var res = CallMesCommand(1, sp_data);

            if (res.receive.IndexOf("OK") > -1)
            {
                message = "MES 工号检查 Pass " + res.receive;
                return true;
            }
            else
            {
                message = "MES 工号检查 Fail " + res.receive;
                return false;
            }
            //工号检查
        }

        /// <summary>
        /// 检查箱号
        /// </summary>
        /// <param name="Box_Number">Box_Number 箱号内容</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool CommandCheckBox(string Box_Number, ref string message)
        {
            //箱号检查
            Box_Number += ";REEL;CHECK;";
            var sp_data = Box_Number;
            var flag = CallMesCommand(2, sp_data);

            if (flag.receive.IndexOf("OK") > -1)
            {
                message = "MES 箱号检查 Pass " + flag.receive;
                return true;
            }
            else
            {
                message = "MES 箱号检查 Fail " + flag.receive;
                return false;
            }
        }

        /// <summary>
        ///  上传MES MES 测试记录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ReelNo"></param>
        /// <param name="listData"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool UploadTestRecords(string username, string ReelNo, List<string> listData, ref string message)
        {
            try
            {
                //上传测试记录
                string data = $"{username};{ReelNo};";
                for (int i = 0; i < listData.Count; i++)
                {
                    data += listData[i] + ";";
                }
                var sp_data = data;
                var res = CallMesCommand(6, sp_data);

                if (res.receive.IndexOf("OK") > -1)
                {
                    message = "MES 上传测试记录成功" + res.receive;
                    return true;
                }
                else
                {
                    message = "MES 上传测试记录失败 " + res.receive;
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取BOM對應的内部机种名称清單
        /// </summary>
        /// <param name="ReelNo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static List<string> Get_Mes_Model(string ReelNo, ref string message)
        {
            string sp_data = $"{ReelNo};REEL;LIST;FINPART;";
            var res = CallMesCommand(2, sp_data);
            if (!(res.receive.IndexOf("NG") > -1))
            {
                var listdata = res.receive.Split(',');
                var result = new List<string>();
                for (int i = 0; i < listdata.Length; i++)
                {
                    result.Add(listdata[i].Trim().Replace(";", "").Replace("OK", ""));
                }
                if (result.Count == 0)
                {
                    message = $"{ReelNo} 没有获取到MES 内部机种名称";
                }
                return result;
            }
            else
            {
                message = "MES 获取生成机种失败 " + res.receive;
                return new List<string>();
            }
        }

        /// <summary>
        /// 獲取前一站位的參數
        /// </summary>
        /// <param name="username"></param>
        /// <param name="serial_Number"></param>
        /// <param name="test_item"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Get_Test_ItemValue(string username, string serial_Number, string test_item, ref string message)
        {
            string sp_data = $"{username};{serial_Number};{test_item};";
            var res = CallMesCommand(602, sp_data);
            if (!(res.receive.IndexOf("NG") > -1))
            {
                var listdata = res.receive.Split('；');
                if (listdata.Length == 2)
                {
                    return listdata[2];
                }
                message = $"{serial_Number} 没有获取到MES {test_item} 的值";
                return message;
            }
            else
            {
                message = "MES 获取值 " + res.receive;
                return message;
            }
        }

        /// <summary>
        /// 获取BOM對應的内部品名清單
        /// </summary>
        /// <param name="ReelNo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static List<string> Get_Mes_Model_FINPARTEAN(string ReelNo, ref string message)
        {
            string sp_data = $"{ReelNo};REEL;LIST;FINPARTEAN;";

            var res = CallMesCommand(2, sp_data);
            if (!(res.receive.IndexOf("NG") > -1))
            {
                var listdata = res.receive.Split(',');
                var result = new List<string>();
                for (int i = 0; i < listdata.Length; i++)
                {
                    result.Add(listdata[i].Trim().Replace(";", "").Replace("OK", "").Replace("/ADP", ""));
                }
                if (result.Count == 0)
                {
                    message = $"{ReelNo} 没有获取到MES 内部机种品名";
                }
                return result;
            }
            else
            {
                message = "MES 获取生成机种品名失败 " + res.receive;
                return new List<string>();
            }
        }

        /// <summary>
        /// 檢查 生产机种
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetProductionModelName(string SerialNumber, ref string message)
        {
            try
            {
                SerialNumber += ";";
                var res = CallMesCommand(12, SerialNumber);
                if (res.receive.IndexOf("OK") > -1)
                {
                    message = $"{res.receive.Split(';')[1]}";
                    return true;
                }
                else
                {
                    message = $"MES  Checked Fail {SerialNumber}:{res.receive}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = "MES  Checked Fail " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 返回SMT 主子板绑定关系
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static (bool flag, string error_message, List<string> data) GetMasterSnChildrenBinding(string SerialNumber, ref string message)
        {
            try
            {
                var res = CallMesCommand(11, SerialNumber);
                if (res.receive.IndexOf("OK") > -1)
                {
                    message = $"{res.receive.Split(';')[1]}";

                    return (true, string.Empty, message.Split(',').Where(e => e != "OK").ToList());
                }
                else
                {
                    message = $"MES  Checked Fail {SerialNumber}:{res.receive}";
                    return (false, message, null);
                }
            }
            catch (Exception ex)
            {
                message = "MES  Checked Fail " + ex.Message;
                return (false, message, null);
            }
        }

        /// <summary>
        /// 檢查條碼是否是本站
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool CheckSerialNumber(string SerialNumber, ref string message)
        {
            try
            {
                SerialNumber += ";";
                var res = CallMesCommand(2, SerialNumber);
                if (res.receive.IndexOf("OK") > -1)
                {
                    message = $"MES  Checked Pass {SerialNumber}:{res.receive}";
                    return true;
                }
                else
                {
                    message = $"MES  Checked Fail {SerialNumber}:{res.receive}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = "MES  Checked Fail " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Pass 過站條碼
        /// </summary>
        /// <param name="username"></param>
        /// <param name=""></param>
        /// <param name="SerialNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SerialNumberCorssingStationPass(string username, string SerialNumber, ref string message)
        {
            SerialNumber = $"{username};{SerialNumber};OK;";
            var res = CallMesCommand(3, SerialNumber);
            if (res.receive.IndexOf("OK") > -1)
            {
                message = "MES CorssingStation Pass " + res.receive;
                return true;
            }
            else
            {
                message = "MES CorssingStation Fail " + res.receive;
                return false;
            }
        }

        /// <summary>
        /// Fail 過站條碼
        /// </summary>
        /// <param name="username"></param>
        /// <param name="SerialNumber"></param>
        /// <param name="NG_Code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SerialNumberCorssingStationFail(string username, string SerialNumber, string NG_Code, ref string message)
        {
            SerialNumber = $"{username};{SerialNumber};NG;{NG_Code};";
            var res = CallMesCommand(3, SerialNumber);
            if (res.receive.IndexOf("OK") > -1)
            {
                message = "MES CorssingStation Repair Pass " + res.receive;
                return true;
            }
            else
            {
                message = "MES CorssingStation Repair Fail " + res.receive;
                return false;
            }
        }
    }

    public class MesCommand
    {
        private const string saject = @"SajetConnect.dll";

        static MesCommand()
        {
        }

        [DllImport(saject, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "SajetTransStart")]
        public static extern bool SajetTransStart();

        [DllImport(saject, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "SajetTransData")]
        public static extern Boolean SajetTransData(int f_iCommandNo, IntPtr f_pData, IntPtr f_pLen);

        [DllImport(saject, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "SajetTransData")]
        public static extern bool SajetTransData(int f_iCommandNo, StringBuilder f_pData, ref int f_pLen);

        [DllImport(saject, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "SajetTransClose")]
        public static extern bool SajetTransClose();
    }
}