namespace NetAIVision.Controller
{
    partial class FrmGaussianBlur
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiLine4 = new Sunny.UI.UILine();
            this.SigmaX = new Sunny.UI.UIIntegerUpDown();
            this.uiLine1 = new Sunny.UI.UILine();
            this.kernelSize = new Sunny.UI.UIIntegerUpDown();
            this.SuspendLayout();
            // 
            // uiLine4
            // 
            this.uiLine4.BackColor = System.Drawing.Color.Transparent;
            this.uiLine4.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine4.Location = new System.Drawing.Point(13, 49);
            this.uiLine4.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine4.Name = "uiLine4";
            this.uiLine4.Size = new System.Drawing.Size(306, 20);
            this.uiLine4.TabIndex = 49;
            this.uiLine4.Text = "标准差（越大越模糊）";
            this.uiLine4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SigmaX
            // 
            this.SigmaX.Font = new System.Drawing.Font("SimSun", 12F);
            this.SigmaX.Location = new System.Drawing.Point(13, 83);
            this.SigmaX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SigmaX.Maximum = 100D;
            this.SigmaX.Minimum = -100D;
            this.SigmaX.MinimumSize = new System.Drawing.Size(100, 0);
            this.SigmaX.Name = "SigmaX";
            this.SigmaX.ShowText = false;
            this.SigmaX.Size = new System.Drawing.Size(150, 29);
            this.SigmaX.TabIndex = 48;
            this.SigmaX.Text = "10";
            this.SigmaX.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.SigmaX.Value = 10;
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(13, 120);
            this.uiLine1.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(306, 20);
            this.uiLine1.TabIndex = 51;
            this.uiLine1.Text = "卷积核大小（奇数，如 5, 7, 9）";
            this.uiLine1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kernelSize
            // 
            this.kernelSize.Font = new System.Drawing.Font("SimSun", 12F);
            this.kernelSize.Location = new System.Drawing.Point(13, 154);
            this.kernelSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.kernelSize.Maximum = 100D;
            this.kernelSize.Minimum = -100D;
            this.kernelSize.MinimumSize = new System.Drawing.Size(100, 0);
            this.kernelSize.Name = "kernelSize";
            this.kernelSize.ShowText = false;
            this.kernelSize.Size = new System.Drawing.Size(150, 29);
            this.kernelSize.TabIndex = 50;
            this.kernelSize.Text = "10";
            this.kernelSize.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.kernelSize.Value = 10;
            // 
            // FrmGaussianBlur
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(333, 199);
            this.Controls.Add(this.uiLine1);
            this.Controls.Add(this.kernelSize);
            this.Controls.Add(this.uiLine4);
            this.Controls.Add(this.SigmaX);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmGaussianBlur";
            this.ShowIcon = false;
            this.Text = "高斯模糊参数";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILine uiLine4;
        private Sunny.UI.UIIntegerUpDown SigmaX;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UIIntegerUpDown kernelSize;
    }
}