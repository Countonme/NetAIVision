namespace NetAIVision.Controller
{
    partial class FrmSettingSpellCheck
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
            this.stepNumber = new Sunny.UI.UIIntegerUpDown();
            this.uiCheckBox1 = new Sunny.UI.UICheckBox();
            this.pnlBtm.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBtm
            // 
            this.pnlBtm.Location = new System.Drawing.Point(1, 222);
            this.pnlBtm.Size = new System.Drawing.Size(497, 55);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(369, 12);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(254, 12);
            // 
            // uiLine4
            // 
            this.uiLine4.BackColor = System.Drawing.Color.Transparent;
            this.uiLine4.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine4.Location = new System.Drawing.Point(3, 49);
            this.uiLine4.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine4.Name = "uiLine4";
            this.uiLine4.Size = new System.Drawing.Size(493, 20);
            this.uiLine4.TabIndex = 53;
            this.uiLine4.Text = "获取的目标步骤";
            this.uiLine4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stepNumber
            // 
            this.stepNumber.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.stepNumber.Font = new System.Drawing.Font("SimSun", 12F);
            this.stepNumber.Location = new System.Drawing.Point(4, 77);
            this.stepNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stepNumber.Maximum = 100D;
            this.stepNumber.Minimum = -100D;
            this.stepNumber.MinimumSize = new System.Drawing.Size(100, 0);
            this.stepNumber.Name = "stepNumber";
            this.stepNumber.Padding = new System.Windows.Forms.Padding(5);
            this.stepNumber.ShowText = false;
            this.stepNumber.Size = new System.Drawing.Size(150, 29);
            this.stepNumber.TabIndex = 52;
            this.stepNumber.Text = "10";
            this.stepNumber.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.stepNumber.Value = 10;
            // 
            // uiCheckBox1
            // 
            this.uiCheckBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiCheckBox1.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiCheckBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiCheckBox1.Location = new System.Drawing.Point(4, 137);
            this.uiCheckBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBox1.Name = "uiCheckBox1";
            this.uiCheckBox1.Size = new System.Drawing.Size(150, 29);
            this.uiCheckBox1.TabIndex = 54;
            this.uiCheckBox1.Text = "錯詞 提示";
            // 
            // FrmSettingSpellCheck
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(499, 280);
            this.Controls.Add(this.uiCheckBox1);
            this.Controls.Add(this.uiLine4);
            this.Controls.Add(this.stepNumber);
            this.Name = "FrmSettingSpellCheck";
            this.Text = "拼詞檢查設定";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.Controls.SetChildIndex(this.stepNumber, 0);
            this.Controls.SetChildIndex(this.uiLine4, 0);
            this.Controls.SetChildIndex(this.uiCheckBox1, 0);
            this.Controls.SetChildIndex(this.pnlBtm, 0);
            this.pnlBtm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILine uiLine4;
        private Sunny.UI.UIIntegerUpDown stepNumber;
        private Sunny.UI.UICheckBox uiCheckBox1;
    }
}