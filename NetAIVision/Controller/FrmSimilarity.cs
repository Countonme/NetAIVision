using NetAIVision.Model;
using NetAIVision.Services;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Controller
{
    public partial class FrmSimilarity : UIEditForm
    {
        private Bitmap _baseBitmap;
        private Bitmap _selectBitmap;

        public FrmSimilarity(Bitmap bitmap)
        {
            InitializeComponent();
            pictureBox1.Image = bitmap;
            this._baseBitmap = bitmap;
            this.btnSelect.Click += BtnSelect_Click;
            this.btnSimilarity.Click += BtnSimilarity_Click;
        }

        private void BtnSimilarity_Click(object sender, EventArgs e)
        {
            string _basePath = Application.StartupPath + @"\" + Guid.NewGuid().ToString() + ".jpg";
            _baseBitmap.Save(_basePath);
            //var value = BitmapProcessorServices.CompareWithSIFT(_basePath, txtPath.Text);
            var value = ImageSimilarityHelper.CompareImageSimilarityHybrid(_basePath, txtPath.Text);
            uiRoundProcess3.Text = value.ToString("F3");
            if (File.Exists(_basePath))
            {
                File.Delete(_basePath);
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmps = new Bitmap(dlg.FileName);
                    //Bitmap bmp = new Bitmap(bmps, pictureBox2.Size);

                    pictureBox2.Image = bmps;

                    _selectBitmap = bmps;
                    txtPath.Text = dlg.FileName;
                }
            }
        }

        protected override bool CheckData()
        {
            //图片路径
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                this.ShowErrorNotifier("没有参考路径");
                return false;
            }
            return true;
        }

        private PicSimilarity param;

        public PicSimilarity Param
        {
            get
            {
                if (param == null)
                {
                    param = new PicSimilarity();
                }
                //阈值
                param.threshold = threshold.Value;
                param.path = txtPath.Text;
                return param;
            }

            set
            {
                param = value;
            }
        }
    }
}