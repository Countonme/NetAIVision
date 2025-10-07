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
            this.uiSplitContainer1 = new Sunny.UI.UISplitContainer();
            this.uiTreeView1 = new Sunny.UI.UITreeView();
            this.uiListBox1 = new Sunny.UI.UIListBox();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).BeginInit();
            this.uiSplitContainer1.Panel1.SuspendLayout();
            this.uiSplitContainer1.Panel2.SuspendLayout();
            this.uiSplitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiSplitContainer1
            // 
            this.uiSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.uiSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSplitContainer1.Location = new System.Drawing.Point(0, 0);
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
            this.uiSplitContainer1.Size = new System.Drawing.Size(800, 450);
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
            this.uiTreeView1.ScrollBarStyleInherited = false;
            this.uiTreeView1.ShowText = false;
            this.uiTreeView1.Size = new System.Drawing.Size(266, 450);
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
            this.uiListBox1.Size = new System.Drawing.Size(523, 450);
            this.uiListBox1.TabIndex = 0;
            this.uiListBox1.Text = "uiListBox1";
            // 
            // FrmScript
            // 
            this.AllowShowTitle = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.uiSplitContainer1);
            this.MaximizeBox = false;
            this.Name = "FrmScript";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.ShowTitle = false;
            this.Text = "FrmScript";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.uiSplitContainer1.Panel1.ResumeLayout(false);
            this.uiSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).EndInit();
            this.uiSplitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UISplitContainer uiSplitContainer1;
        private Sunny.UI.UITreeView uiTreeView1;
        private Sunny.UI.UIListBox uiListBox1;
    }
}