using NetAIVision.Services;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Controller
{
    public partial class FrmScript : UIForm
    {
        private Bitmap _bitmap;
        private Bitmap _bitmap_with;

        public FrmScript(Bitmap bitmap)
        {
            InitializeComponent();
            this._bitmap = bitmap;
            this._bitmap_with = bitmap;
            this.Shown += FrmScript_Shown;
            this.baseOToolStripMenuItem.Click += BaseOToolStripMenuItem_Click;
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
    }
}