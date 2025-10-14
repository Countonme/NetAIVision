namespace NetAIVision.Controller
{
    partial class FrmCharacterComparison
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
            this.uiLine1 = new Sunny.UI.UILine();
            this.txtBaseString = new Sunny.UI.UITextBox();
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
            this.uiLine4.Location = new System.Drawing.Point(3, 55);
            this.uiLine4.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine4.Name = "uiLine4";
            this.uiLine4.Size = new System.Drawing.Size(476, 20);
            this.uiLine4.TabIndex = 51;
            this.uiLine4.Text = "获取的目标步骤";
            this.uiLine4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stepNumber
            // 
            this.stepNumber.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.stepNumber.Font = new System.Drawing.Font("SimSun", 12F);
            this.stepNumber.Location = new System.Drawing.Point(4, 83);
            this.stepNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stepNumber.Maximum = 100D;
            this.stepNumber.Minimum = -100D;
            this.stepNumber.MinimumSize = new System.Drawing.Size(100, 0);
            this.stepNumber.Name = "stepNumber";
            this.stepNumber.Padding = new System.Windows.Forms.Padding(5);
            this.stepNumber.ShowText = false;
            this.stepNumber.Size = new System.Drawing.Size(150, 29);
            this.stepNumber.TabIndex = 50;
            this.stepNumber.Text = "10";
            this.stepNumber.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.stepNumber.Value = 10;
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("SimSun", 12F);
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(3, 135);
            this.uiLine1.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(476, 20);
            this.uiLine1.TabIndex = 52;
            this.uiLine1.Text = "比对实体字符串";
            this.uiLine1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBaseString
            // 
            this.txtBaseString.ButtonWidth = 100;
            this.txtBaseString.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBaseString.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBaseString.Location = new System.Drawing.Point(4, 163);
            this.txtBaseString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBaseString.MinimumSize = new System.Drawing.Size(1, 1);
            this.txtBaseString.Name = "txtBaseString";
            this.txtBaseString.Padding = new System.Windows.Forms.Padding(5);
            this.txtBaseString.ShowText = false;
            this.txtBaseString.Size = new System.Drawing.Size(475, 29);
            this.txtBaseString.Symbol = 61461;
            this.txtBaseString.TabIndex = 53;
            this.txtBaseString.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtBaseString.Watermark = "字符串";
            // 
            // FrmCharacterComparison
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(499, 280);
            this.Controls.Add(this.txtBaseString);
            this.Controls.Add(this.uiLine1);
            this.Controls.Add(this.uiLine4);
            this.Controls.Add(this.stepNumber);
            this.Name = "FrmCharacterComparison";
            this.Text = "Character Comparison (字符比较)";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.Controls.SetChildIndex(this.stepNumber, 0);
            this.Controls.SetChildIndex(this.uiLine4, 0);
            this.Controls.SetChildIndex(this.uiLine1, 0);
            this.Controls.SetChildIndex(this.txtBaseString, 0);
            this.Controls.SetChildIndex(this.pnlBtm, 0);
            this.pnlBtm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILine uiLine4;
        private Sunny.UI.UIIntegerUpDown stepNumber;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UITextBox txtBaseString;
    }
}