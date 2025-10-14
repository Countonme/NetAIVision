namespace NetAIVision.Controller
{
    partial class FrmSimilarity
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
            this.uiLine9 = new Sunny.UI.UILine();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.uiLine1 = new Sunny.UI.UILine();
            this.uiRoundProcess3 = new Sunny.UI.UIRoundProcess();
            this.btnSimilarity = new Sunny.UI.UISymbolButton();
            this.btnSelect = new Sunny.UI.UISymbolButton();
            this.txtPath = new Sunny.UI.UILine();
            this.threshold = new Sunny.UI.UIDoubleUpDown();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.pnlBtm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBtm
            // 
            this.pnlBtm.Location = new System.Drawing.Point(1, 438);
            this.pnlBtm.Size = new System.Drawing.Size(814, 55);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(686, 12);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(571, 12);
            // 
            // uiLine9
            // 
            this.uiLine9.BackColor = System.Drawing.Color.Transparent;
            this.uiLine9.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine9.Location = new System.Drawing.Point(13, 71);
            this.uiLine9.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine9.Name = "uiLine9";
            this.uiLine9.Size = new System.Drawing.Size(344, 20);
            this.uiLine9.TabIndex = 93;
            this.uiLine9.Text = "原图";
            this.uiLine9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(427, 107);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(344, 191);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 107);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(344, 191);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(427, 71);
            this.uiLine1.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(344, 20);
            this.uiLine1.TabIndex = 94;
            this.uiLine1.Text = "参考图";
            this.uiLine1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiRoundProcess3
            // 
            this.uiRoundProcess3.BackColor = System.Drawing.Color.Transparent;
            this.uiRoundProcess3.DecimalPlaces = 0;
            this.uiRoundProcess3.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiRoundProcess3.ForeColor2 = System.Drawing.Color.Black;
            this.uiRoundProcess3.Inner = 35;
            this.uiRoundProcess3.Location = new System.Drawing.Point(676, 309);
            this.uiRoundProcess3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiRoundProcess3.Name = "uiRoundProcess3";
            this.uiRoundProcess3.Outer = 40;
            this.uiRoundProcess3.ShowProcess = true;
            this.uiRoundProcess3.Size = new System.Drawing.Size(95, 80);
            this.uiRoundProcess3.StartAngle = 220;
            this.uiRoundProcess3.SweepAngle = 280;
            this.uiRoundProcess3.TabIndex = 106;
            this.uiRoundProcess3.Text = "0";
            // 
            // btnSimilarity
            // 
            this.btnSimilarity.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSimilarity.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnSimilarity.Location = new System.Drawing.Point(50, 38);
            this.btnSimilarity.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSimilarity.Name = "btnSimilarity";
            this.btnSimilarity.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.RightTop | Sunny.UI.UICornerRadiusSides.RightBottom)));
            this.btnSimilarity.RectSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)(((System.Windows.Forms.ToolStripStatusLabelBorderSides.Top | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.btnSimilarity.Size = new System.Drawing.Size(46, 35);
            this.btnSimilarity.Symbol = 361473;
            this.btnSimilarity.TabIndex = 110;
            this.btnSimilarity.TipsFont = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // btnSelect
            // 
            this.btnSelect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelect.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnSelect.Location = new System.Drawing.Point(4, 38);
            this.btnSelect.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.LeftTop | Sunny.UI.UICornerRadiusSides.LeftBottom)));
            this.btnSelect.Size = new System.Drawing.Size(46, 35);
            this.btnSelect.Symbol = 361543;
            this.btnSelect.TabIndex = 107;
            this.btnSelect.TipsFont = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // txtPath
            // 
            this.txtPath.BackColor = System.Drawing.Color.Transparent;
            this.txtPath.Font = new System.Drawing.Font("SimSun", 12F);
            this.txtPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.txtPath.Location = new System.Drawing.Point(13, 390);
            this.txtPath.MinimumSize = new System.Drawing.Size(16, 16);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(758, 20);
            this.txtPath.TabIndex = 111;
            this.txtPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // threshold
            // 
            this.threshold.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.threshold.Location = new System.Drawing.Point(61, 360);
            this.threshold.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.threshold.Maximum = 1D;
            this.threshold.Minimum = 0D;
            this.threshold.MinimumSize = new System.Drawing.Size(100, 0);
            this.threshold.Name = "threshold";
            this.threshold.ShowText = false;
            this.threshold.Size = new System.Drawing.Size(150, 29);
            this.threshold.Step = 0.01D;
            this.threshold.TabIndex = 112;
            this.threshold.Text = "1.00";
            this.threshold.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.threshold.Value = 1D;
            // 
            // uiLabel3
            // 
            this.uiLabel3.AutoSize = true;
            this.uiLabel3.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel3.Location = new System.Drawing.Point(15, 371);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(39, 16);
            this.uiLabel3.TabIndex = 113;
            this.uiLabel3.Text = "阈值";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FrmSimilarity
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(816, 496);
            this.Controls.Add(this.threshold);
            this.Controls.Add(this.uiLabel3);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnSimilarity);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.uiRoundProcess3);
            this.Controls.Add(this.uiLine1);
            this.Controls.Add(this.uiLine9);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Name = "FrmSimilarity";
            this.Text = "Pictrue Similarity";
            this.Controls.SetChildIndex(this.pictureBox1, 0);
            this.Controls.SetChildIndex(this.pictureBox2, 0);
            this.Controls.SetChildIndex(this.uiLine9, 0);
            this.Controls.SetChildIndex(this.uiLine1, 0);
            this.Controls.SetChildIndex(this.uiRoundProcess3, 0);
            this.Controls.SetChildIndex(this.pnlBtm, 0);
            this.Controls.SetChildIndex(this.btnSelect, 0);
            this.Controls.SetChildIndex(this.btnSimilarity, 0);
            this.Controls.SetChildIndex(this.txtPath, 0);
            this.Controls.SetChildIndex(this.uiLabel3, 0);
            this.Controls.SetChildIndex(this.threshold, 0);
            this.pnlBtm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Sunny.UI.UILine uiLine9;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UIRoundProcess uiRoundProcess3;
        private Sunny.UI.UISymbolButton btnSimilarity;
        private Sunny.UI.UISymbolButton btnSelect;
        private Sunny.UI.UILine txtPath;
        private Sunny.UI.UIDoubleUpDown threshold;
        private Sunny.UI.UILabel uiLabel3;
    }
}