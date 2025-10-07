namespace NetAIVision
{
    partial class FrmMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMaster));
            this.StepMenuStrip = new Sunny.UI.UIContextMenuStrip();
            this.addROIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeROIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oCRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbDeviceList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbFrameRate = new System.Windows.Forms.TextBox();
            this.tbGain = new System.Windows.Forms.TextBox();
            this.tbExposure = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.原圖OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.灰度GToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.二值化BToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.反色IToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.高斯模糊UToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.边缘检测EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.水平翻转FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.垂直翻转VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.亮度LToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.亮度MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.对比度CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.对比度TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oCRRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.二维码读取QToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.参考线NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.目标ROI显示DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ℹ关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StepMenuStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // StepMenuStrip
            // 
            this.StepMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.StepMenuStrip.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.StepMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addROIToolStripMenuItem,
            this.removeROIToolStripMenuItem,
            this.oCRToolStripMenuItem,
            this.pToolStripMenuItem});
            this.StepMenuStrip.Name = "StepMenuStrip";
            this.StepMenuStrip.Size = new System.Drawing.Size(157, 92);
            // 
            // addROIToolStripMenuItem
            // 
            this.addROIToolStripMenuItem.Name = "addROIToolStripMenuItem";
            this.addROIToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.addROIToolStripMenuItem.Text = "Add ROI";
            // 
            // removeROIToolStripMenuItem
            // 
            this.removeROIToolStripMenuItem.Name = "removeROIToolStripMenuItem";
            this.removeROIToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.removeROIToolStripMenuItem.Text = "Remove ROI";
            // 
            // oCRToolStripMenuItem
            // 
            this.oCRToolStripMenuItem.Name = "oCRToolStripMenuItem";
            this.oCRToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.oCRToolStripMenuItem.Text = "OCR";
            // 
            // pToolStripMenuItem
            // 
            this.pToolStripMenuItem.Name = "pToolStripMenuItem";
            this.pToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pToolStripMenuItem.Text = "P";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI Emoji", 10.25F);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.功能ToolStripMenuItem,
            this.ℹ关于AToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 35);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.menuStrip1.Size = new System.Drawing.Size(1404, 25);
            this.menuStrip1.TabIndex = 28;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openScriptToolStripMenuItem,
            this.openLogsToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(64, 23);
            this.fileToolStripMenuItem.Text = "📁 File";
            // 
            // openScriptToolStripMenuItem
            // 
            this.openScriptToolStripMenuItem.Name = "openScriptToolStripMenuItem";
            this.openScriptToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.openScriptToolStripMenuItem.Text = "Open Script";
            // 
            // openLogsToolStripMenuItem
            // 
            this.openLogsToolStripMenuItem.Name = "openLogsToolStripMenuItem";
            this.openLogsToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.openLogsToolStripMenuItem.Text = "Open Logs";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(73, 23);
            this.viewToolStripMenuItem.Text = "👁️ View";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.logToolStripMenuItem.Text = "Log";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(114, 23);
            this.openToolStripMenuItem.Text = "✅ Connection";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(145, 23);
            this.closeToolStripMenuItem.Text = "🔌❌Disconnection";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 10.25F);
            this.refreshToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(89, 23);
            this.refreshToolStripMenuItem.Text = "🔄 Refresh";
            // 
            // cbDeviceList
            // 
            this.cbDeviceList.FormattingEnabled = true;
            this.cbDeviceList.Location = new System.Drawing.Point(3, 88);
            this.cbDeviceList.Name = "cbDeviceList";
            this.cbDeviceList.Size = new System.Drawing.Size(523, 24);
            this.cbDeviceList.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(1036, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 36;
            this.label3.Text = "帧率";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(1036, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 35;
            this.label2.Text = "增益";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(1036, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 34;
            this.label1.Text = "曝光";
            // 
            // tbFrameRate
            // 
            this.tbFrameRate.Enabled = false;
            this.tbFrameRate.Location = new System.Drawing.Point(1081, 185);
            this.tbFrameRate.Name = "tbFrameRate";
            this.tbFrameRate.Size = new System.Drawing.Size(93, 26);
            this.tbFrameRate.TabIndex = 33;
            // 
            // tbGain
            // 
            this.tbGain.Enabled = false;
            this.tbGain.Location = new System.Drawing.Point(1081, 154);
            this.tbGain.Name = "tbGain";
            this.tbGain.Size = new System.Drawing.Size(93, 26);
            this.tbGain.TabIndex = 32;
            // 
            // tbExposure
            // 
            this.tbExposure.Enabled = false;
            this.tbExposure.Location = new System.Drawing.Point(1081, 123);
            this.tbExposure.Name = "tbExposure";
            this.tbExposure.Size = new System.Drawing.Size(93, 26);
            this.tbExposure.TabIndex = 31;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(3, 128);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1027, 768);
            this.pictureBox1.TabIndex = 29;
            this.pictureBox1.TabStop = false;
            // 
            // 功能ToolStripMenuItem
            // 
            this.功能ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.原圖OToolStripMenuItem,
            this.灰度GToolStripMenuItem,
            this.二值化BToolStripMenuItem,
            this.反色IToolStripMenuItem,
            this.高斯模糊UToolStripMenuItem,
            this.边缘检测EToolStripMenuItem,
            this.水平翻转FToolStripMenuItem,
            this.垂直翻转VToolStripMenuItem,
            this.亮度LToolStripMenuItem,
            this.亮度MToolStripMenuItem,
            this.对比度CToolStripMenuItem,
            this.对比度TToolStripMenuItem,
            this.oCRRToolStripMenuItem,
            this.二维码读取QToolStripMenuItem,
            this.参考线NToolStripMenuItem,
            this.目标ROI显示DToolStripMenuItem});
            this.功能ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.功能ToolStripMenuItem.Name = "功能ToolStripMenuItem";
            this.功能ToolStripMenuItem.Size = new System.Drawing.Size(72, 23);
            this.功能ToolStripMenuItem.Text = "⚙️ 功能";
            // 
            // 原圖OToolStripMenuItem
            // 
            this.原圖OToolStripMenuItem.Name = "原圖OToolStripMenuItem";
            this.原圖OToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.原圖OToolStripMenuItem.Text = "🖼️ 原圖(&O)";
            // 
            // 灰度GToolStripMenuItem
            // 
            this.灰度GToolStripMenuItem.Name = "灰度GToolStripMenuItem";
            this.灰度GToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.灰度GToolStripMenuItem.Text = "🔳 灰度(&G)";
            // 
            // 二值化BToolStripMenuItem
            // 
            this.二值化BToolStripMenuItem.Name = "二值化BToolStripMenuItem";
            this.二值化BToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.二值化BToolStripMenuItem.Text = "⚖️ 二值化(&B)";
            // 
            // 反色IToolStripMenuItem
            // 
            this.反色IToolStripMenuItem.Name = "反色IToolStripMenuItem";
            this.反色IToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.反色IToolStripMenuItem.Text = "🔄 反色(&I)";
            // 
            // 高斯模糊UToolStripMenuItem
            // 
            this.高斯模糊UToolStripMenuItem.Name = "高斯模糊UToolStripMenuItem";
            this.高斯模糊UToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.高斯模糊UToolStripMenuItem.Text = "🌀 高斯模糊(&U)";
            // 
            // 边缘检测EToolStripMenuItem
            // 
            this.边缘检测EToolStripMenuItem.Name = "边缘检测EToolStripMenuItem";
            this.边缘检测EToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.边缘检测EToolStripMenuItem.Text = "🔍 边缘检测(&E)";
            // 
            // 水平翻转FToolStripMenuItem
            // 
            this.水平翻转FToolStripMenuItem.Name = "水平翻转FToolStripMenuItem";
            this.水平翻转FToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.水平翻转FToolStripMenuItem.Text = "↔️ 水平翻转(&F)";
            // 
            // 垂直翻转VToolStripMenuItem
            // 
            this.垂直翻转VToolStripMenuItem.Name = "垂直翻转VToolStripMenuItem";
            this.垂直翻转VToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.垂直翻转VToolStripMenuItem.Text = "↕️ 垂直翻转(&V)";
            // 
            // 亮度LToolStripMenuItem
            // 
            this.亮度LToolStripMenuItem.Name = "亮度LToolStripMenuItem";
            this.亮度LToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.亮度LToolStripMenuItem.Text = "🔆 亮度 + (&L)";
            // 
            // 亮度MToolStripMenuItem
            // 
            this.亮度MToolStripMenuItem.Name = "亮度MToolStripMenuItem";
            this.亮度MToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.亮度MToolStripMenuItem.Text = "🔅 亮度 -(&M)";
            // 
            // 对比度CToolStripMenuItem
            // 
            this.对比度CToolStripMenuItem.Name = "对比度CToolStripMenuItem";
            this.对比度CToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.对比度CToolStripMenuItem.Text = "📈 对比度 +(&C)";
            // 
            // 对比度TToolStripMenuItem
            // 
            this.对比度TToolStripMenuItem.Name = "对比度TToolStripMenuItem";
            this.对比度TToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.对比度TToolStripMenuItem.Text = "📉 对比度 -(&T)";
            // 
            // oCRRToolStripMenuItem
            // 
            this.oCRRToolStripMenuItem.Name = "oCRRToolStripMenuItem";
            this.oCRRToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.oCRRToolStripMenuItem.Text = "🔤 OCR(&R)";
            // 
            // 二维码读取QToolStripMenuItem
            // 
            this.二维码读取QToolStripMenuItem.Name = "二维码读取QToolStripMenuItem";
            this.二维码读取QToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.二维码读取QToolStripMenuItem.Text = "🧩 二维码读取(&Q)";
            // 
            // 参考线NToolStripMenuItem
            // 
            this.参考线NToolStripMenuItem.Name = "参考线NToolStripMenuItem";
            this.参考线NToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.参考线NToolStripMenuItem.Text = "➕ 参考线(&N)";
            // 
            // 目标ROI显示DToolStripMenuItem
            // 
            this.目标ROI显示DToolStripMenuItem.Name = "目标ROI显示DToolStripMenuItem";
            this.目标ROI显示DToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.目标ROI显示DToolStripMenuItem.Text = "🔲 目标ROI显示(&D)";
            // 
            // ℹ关于AToolStripMenuItem
            // 
            this.ℹ关于AToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.ℹ关于AToolStripMenuItem.Name = "ℹ关于AToolStripMenuItem";
            this.ℹ关于AToolStripMenuItem.Size = new System.Drawing.Size(98, 23);
            this.ℹ关于AToolStripMenuItem.Text = "ℹ️ 关于(&A)...";
            // 
            // FrmMaster
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1404, 709);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbFrameRate);
            this.Controls.Add(this.tbGain);
            this.Controls.Add(this.tbExposure);
            this.Controls.Add(this.cbDeviceList);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMaster";
            this.Text = "AI Vision";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 840, 590);
            this.StepMenuStrip.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UIContextMenuStrip StepMenuStrip;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem addROIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeROIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oCRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogsToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbDeviceList;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbFrameRate;
        private System.Windows.Forms.TextBox tbGain;
        private System.Windows.Forms.TextBox tbExposure;
        private System.Windows.Forms.ToolStripMenuItem 功能ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 原圖OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 灰度GToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 二值化BToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 反色IToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 高斯模糊UToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 边缘检测EToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 水平翻转FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 垂直翻转VToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 亮度LToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 亮度MToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 对比度CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 对比度TToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oCRRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 二维码读取QToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 参考线NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 目标ROI显示DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ℹ关于AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}

