using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Model
{
    public class ProducitonModelClassName
    {
        // 定义所有类别名称（下标与 ClassId 对应）
        public static string[] PA_1050_36AU = new string[]
           {
                "CE",
                "DOELevelVI",
                "DoubleInsulated",
                "IndoorUseOnly",
                "Power",
                "QRCode",
                "SnLine:",
                "StandardText",
                "UKCA",
                "WEEE",
                "rightArrow"
           };

        public static string[] PA_1150_16VN = new string[]
       {
                            "DOELevelVI",
                            "IndoorUseOnly",
                            "Power",
                            "QRCode",
                            "SnLine:",
                            "StandardText",
                            "NOM",
                            "cTUVus",
                            "rightArrow"
       };

        public static string[] ModelList = new string[] {
           "BIS"//BIS 印度標準
          ,"CE"//CE 歐盟標準
          ,"DOELevelVI"//DOE 六级能效
          ,"DoubleInsulated"//雙重絕緣
          ,"IndoorUseOnly"//室內使用
          ,"NOM"//NOM 墨西哥
          ,"PSE + TUV RH TAIWAN"//PSE + TUV RH TAIWAN 一般出日本
          ,"Power" //功率
          ,"QRCode"//QR碼
          ,"SnLine:" //產品序列號
          ,"StandardText" //標準的文字
          ,"UKCA" //UKCA
          ,"WEEE" //WEEE 國際標準
          ,"cTUVus" //cTUVus
          ,"rightArrow" //右箭頭
        };
    }
}