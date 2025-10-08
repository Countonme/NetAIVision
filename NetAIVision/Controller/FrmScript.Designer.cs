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
            this.uiSplitContainer1 = new Sunny.UI.UISplitContainer();
            this.uiTreeView1 = new Sunny.UI.UITreeView();
            this.uiListBox1 = new Sunny.UI.UIListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).BeginInit();
            this.uiSplitContainer1.Panel1.SuspendLayout();
            this.uiSplitContainer1.Panel2.SuspendLayout();
            this.uiSplitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.uiSplitContainer1.Panel2.Controls.Add(this.uiListBox1);
            this.uiSplitContainer1.Size = new System.Drawing.Size(800, 391);
            this.uiSplitContainer1.SplitterDistance = 266;
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
            this.uiTreeView1.Size = new System.Drawing.Size(266, 391);
            this.uiTreeView1.TabIndex = 0;
            this.uiTreeView1.Text = "uiTreeView1";
            this.uiTreeView1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiListBox1
            // 
            this.uiListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiListBox1.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiListBox1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiListBox1.ItemSelectForeColor = System.Drawing.Color.White;
            this.uiListBox1.Location = new System.Drawing.Point(0, 0);
            this.uiListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiListBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiListBox1.Name = "uiListBox1";
            this.uiListBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiListBox1.ShowText = false;
            this.uiListBox1.Size = new System.Drawing.Size(523, 391);
            this.uiListBox1.TabIndex = 0;
            this.uiListBox1.Text = "uiListBox1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 35);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.runToolStripMenuItem.Text = "Run";
            // 
            // FrmScript
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.uiSplitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FrmScript";
            this.Text = "FrmScript";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.uiSplitContainer1.Panel1.ResumeLayout(false);
            this.uiSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).EndInit();
            this.uiSplitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UISplitContainer uiSplitContainer1;
        private Sunny.UI.UITreeView uiTreeView1;
        private Sunny.UI.UIListBox uiListBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
    }
}