namespace NetAIVision.Controller
{
    partial class FrmScript
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("🌀 高斯模糊");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("反色");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("二值化");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("🔍 边缘检测");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("图像处理", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("文字提取（OCR）");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("文字处理", new System.Windows.Forms.TreeNode[] {
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("条码识别");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("条码处理", new System.Windows.Forms.TreeNode[] {
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("运算处理");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmScript));
            this.uiSplitContainer1 = new Sunny.UI.UISplitContainer();
            this.uiTreeView1 = new Sunny.UI.UITreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uiListBox1 = new Sunny.UI.UIListBox();
            this.uiLine3 = new Sunny.UI.UILine();
            this.uiLine1 = new Sunny.UI.UILine();
            this.功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.baseOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrayscaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InvertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GaussianBlurUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DetectEdgesColoredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FlipHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FlipVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.亮度LToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.亮度MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.对比度CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.对比度TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oCRRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.二维码读取QToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.目标ROI显示DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.DetectEdgestoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).BeginInit();
            this.uiSplitContainer1.Panel1.SuspendLayout();
            this.uiSplitContainer1.Panel2.SuspendLayout();
            this.uiSplitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // uiSplitContainer1
            // 
            this.uiSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.uiSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSplitContainer1.Location = new System.Drawing.Point(0, 59);
            this.uiSplitContainer1.MinimumSize = new System.Drawing.Size(20, 20);
            this.uiSplitContainer1.Name = "uiSplitContainer1";
            // 
            // uiSplitContainer1.Panel1
            // 
            this.uiSplitContainer1.Panel1.Controls.Add(this.uiTreeView1);
            // 
            // uiSplitContainer1.Panel2
            // 
            this.uiSplitContainer1.Panel2.Controls.Add(this.uiLine1);
            this.uiSplitContainer1.Panel2.Controls.Add(this.uiLine3);
            this.uiSplitContainer1.Panel2.Controls.Add(this.uiListBox1);
            this.uiSplitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.uiSplitContainer1.Size = new System.Drawing.Size(1214, 552);
            this.uiSplitContainer1.SplitterDistance = 203;
            this.uiSplitContainer1.SplitterWidth = 11;
            this.uiSplitContainer1.TabIndex = 0;
            // 
            // uiTreeView1
            // 
            this.uiTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTreeView1.FillColor = System.Drawing.Color.White;
            this.uiTreeView1.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiTreeView1.Location = new System.Drawing.Point(0, 0);
            this.uiTreeView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTreeView1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTreeView1.Name = "uiTreeView1";
            treeNode1.Name = "Node1";
            treeNode1.Text = "🌀 高斯模糊";
            treeNode2.Name = "Node2";
            treeNode2.Text = "反色";
            treeNode3.Name = "Node3";
            treeNode3.Text = "二值化";
            treeNode4.Name = "Node1";
            treeNode4.Text = "🔍 边缘检测";
            treeNode5.Name = "Node0";
            treeNode5.Text = "图像处理";
            treeNode6.Name = "Node7";
            treeNode6.Text = "文字提取（OCR）";
            treeNode7.Name = "Node4";
            treeNode7.Text = "文字处理";
            treeNode8.Name = "Node8";
            treeNode8.Text = "条码识别";
            treeNode9.Name = "Node5";
            treeNode9.Text = "条码处理";
            treeNode10.Name = "Node6";
            treeNode10.Text = "运算处理";
            this.uiTreeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode7,
            treeNode9,
            treeNode10});
            this.uiTreeView1.ScrollBarStyleInherited = false;
            this.uiTreeView1.ShowText = false;
            this.uiTreeView1.Size = new System.Drawing.Size(203, 552);
            this.uiTreeView1.TabIndex = 0;
            this.uiTreeView1.Text = "uiTreeView1";
            this.uiTreeView1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.功能ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 35);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1214, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.runToolStripMenuItem.Text = "Run";
            // 
            // uiListBox1
            // 
            this.uiListBox1.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiListBox1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiListBox1.ItemSelectForeColor = System.Drawing.Color.White;
            this.uiListBox1.Location = new System.Drawing.Point(589, 55);
            this.uiListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiListBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiListBox1.Name = "uiListBox1";
            this.uiListBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiListBox1.ShowText = false;
            this.uiListBox1.Size = new System.Drawing.Size(396, 492);
            this.uiListBox1.TabIndex = 1;
            this.uiListBox1.Text = "uiListBox1";
            // 
            // uiLine3
            // 
            this.uiLine3.BackColor = System.Drawing.Color.Transparent;
            this.uiLine3.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine3.Location = new System.Drawing.Point(589, 16);
            this.uiLine3.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine3.Name = "uiLine3";
            this.uiLine3.Size = new System.Drawing.Size(396, 20);
            this.uiLine3.TabIndex = 70;
            this.uiLine3.Text = "处理过程";
            this.uiLine3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(5, 16);
            this.uiLine1.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(553, 20);
            this.uiLine1.TabIndex = 71;
            this.uiLine1.Text = "图像对象";
            this.uiLine1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // 功能ToolStripMenuItem
            // 
            this.功能ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.baseOToolStripMenuItem,
            this.GrayscaleToolStripMenuItem,
            this.ThresholdToolStripMenuItem,
            this.InvertToolStripMenuItem,
            this.GaussianBlurUToolStripMenuItem,
            this.DetectEdgestoolStripMenuItem1,
            this.DetectEdgesColoredToolStripMenuItem,
            this.FlipHorizontalToolStripMenuItem,
            this.FlipVerticalToolStripMenuItem,
            this.亮度LToolStripMenuItem,
            this.亮度MToolStripMenuItem,
            this.对比度CToolStripMenuItem,
            this.对比度TToolStripMenuItem,
            this.oCRRToolStripMenuItem,
            this.二维码读取QToolStripMenuItem,
            this.guidelineToolStripMenuItem,
            this.目标ROI显示DToolStripMenuItem});
            this.功能ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.功能ToolStripMenuItem.Name = "功能ToolStripMenuItem";
            this.功能ToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.功能ToolStripMenuItem.Text = "⚙️ 功能";
            // 
            // baseOToolStripMenuItem
            // 
            this.baseOToolStripMenuItem.Name = "baseOToolStripMenuItem";
            this.baseOToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.baseOToolStripMenuItem.Text = "🖼️ 原圖(&O)";
            // 
            // GrayscaleToolStripMenuItem
            // 
            this.GrayscaleToolStripMenuItem.Name = "GrayscaleToolStripMenuItem";
            this.GrayscaleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.GrayscaleToolStripMenuItem.Text = "🔳 灰度(&G)";
            // 
            // ThresholdToolStripMenuItem
            // 
            this.ThresholdToolStripMenuItem.Name = "ThresholdToolStripMenuItem";
            this.ThresholdToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ThresholdToolStripMenuItem.Text = "⚖️ 二值化(&B)";
            // 
            // InvertToolStripMenuItem
            // 
            this.InvertToolStripMenuItem.Name = "InvertToolStripMenuItem";
            this.InvertToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.InvertToolStripMenuItem.Text = "🔄 反色(&I)";
            // 
            // GaussianBlurUToolStripMenuItem
            // 
            this.GaussianBlurUToolStripMenuItem.Name = "GaussianBlurUToolStripMenuItem";
            this.GaussianBlurUToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.GaussianBlurUToolStripMenuItem.Text = "🌀 高斯模糊(&U)";
            // 
            // DetectEdgesColoredToolStripMenuItem
            // 
            this.DetectEdgesColoredToolStripMenuItem.Name = "DetectEdgesColoredToolStripMenuItem";
            this.DetectEdgesColoredToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.DetectEdgesColoredToolStripMenuItem.Text = "🔍 边缘检测(彩色)";
            // 
            // FlipHorizontalToolStripMenuItem
            // 
            this.FlipHorizontalToolStripMenuItem.Checked = true;
            this.FlipHorizontalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FlipHorizontalToolStripMenuItem.Name = "FlipHorizontalToolStripMenuItem";
            this.FlipHorizontalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.FlipHorizontalToolStripMenuItem.Text = "↔️ 水平翻转(&F)";
            // 
            // FlipVerticalToolStripMenuItem
            // 
            this.FlipVerticalToolStripMenuItem.Checked = true;
            this.FlipVerticalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FlipVerticalToolStripMenuItem.Name = "FlipVerticalToolStripMenuItem";
            this.FlipVerticalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.FlipVerticalToolStripMenuItem.Text = "↕️ 垂直翻转(&V)";
            // 
            // 亮度LToolStripMenuItem
            // 
            this.亮度LToolStripMenuItem.Name = "亮度LToolStripMenuItem";
            this.亮度LToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.亮度LToolStripMenuItem.Text = "🔆 亮度 + (&L)";
            // 
            // 亮度MToolStripMenuItem
            // 
            this.亮度MToolStripMenuItem.Name = "亮度MToolStripMenuItem";
            this.亮度MToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.亮度MToolStripMenuItem.Text = "🔅 亮度 -(&M)";
            // 
            // 对比度CToolStripMenuItem
            // 
            this.对比度CToolStripMenuItem.Name = "对比度CToolStripMenuItem";
            this.对比度CToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.对比度CToolStripMenuItem.Text = "📈 对比度 +(&C)";
            // 
            // 对比度TToolStripMenuItem
            // 
            this.对比度TToolStripMenuItem.Name = "对比度TToolStripMenuItem";
            this.对比度TToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.对比度TToolStripMenuItem.Text = "📉 对比度 -(&T)";
            // 
            // oCRRToolStripMenuItem
            // 
            this.oCRRToolStripMenuItem.Name = "oCRRToolStripMenuItem";
            this.oCRRToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.oCRRToolStripMenuItem.Text = "🔤 OCR(&R)";
            // 
            // 二维码读取QToolStripMenuItem
            // 
            this.二维码读取QToolStripMenuItem.Name = "二维码读取QToolStripMenuItem";
            this.二维码读取QToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.二维码读取QToolStripMenuItem.Text = "🧩 二维码读取(&Q)";
            // 
            // guidelineToolStripMenuItem
            // 
            this.guidelineToolStripMenuItem.CheckOnClick = true;
            this.guidelineToolStripMenuItem.Name = "guidelineToolStripMenuItem";
            this.guidelineToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.guidelineToolStripMenuItem.Text = "➕ 参考线(&N)";
            // 
            // 目标ROI显示DToolStripMenuItem
            // 
            this.目标ROI显示DToolStripMenuItem.Name = "目标ROI显示DToolStripMenuItem";
            this.目标ROI显示DToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.目标ROI显示DToolStripMenuItem.Text = "🔲 目标ROI显示(&D)";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(5, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(553, 505);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // DetectEdgestoolStripMenuItem1
            // 
            this.DetectEdgestoolStripMenuItem1.Name = "DetectEdgestoolStripMenuItem1";
            this.DetectEdgestoolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.DetectEdgestoolStripMenuItem1.Text = "🔍 边缘检测(灰色)";
            // 
            // FrmScript
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1214, 611);
            this.Controls.Add(this.uiSplitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FrmScript";
            this.Text = "图像处理步骤";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.uiSplitContainer1.Panel1.ResumeLayout(false);
            this.uiSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).EndInit();
            this.uiSplitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UISplitContainer uiSplitContainer1;
        private Sunny.UI.UITreeView uiTreeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UILine uiLine3;
        private Sunny.UI.UIListBox uiListBox1;
        private System.Windows.Forms.ToolStripMenuItem 功能ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem baseOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GrayscaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ThresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InvertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GaussianBlurUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DetectEdgesColoredToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FlipHorizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FlipVerticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 亮度LToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 亮度MToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 对比度CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 对比度TToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oCRRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 二维码读取QToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guidelineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 目标ROI显示DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DetectEdgestoolStripMenuItem1;
    }
}