namespace IISGeoIP2blockModule
{
    partial class AddExceptionRuleForm
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
            this.BOk = new System.Windows.Forms.Button();
            this.BCancel = new System.Windows.Forms.Button();
            this.RBSpecificIpAddress = new System.Windows.Forms.RadioButton();
            this.LCaption = new System.Windows.Forms.Label();
            this.TBSpecificIpAddress = new System.Windows.Forms.TextBox();
            this.RBAddressRange = new System.Windows.Forms.RadioButton();
            this.TBAddressRange = new System.Windows.Forms.TextBox();
            this.LMask = new System.Windows.Forms.Label();
            this.TBMask = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BOk
            // 
            this.BOk.Enabled = false;
            this.BOk.Location = new System.Drawing.Point(225, 202);
            this.BOk.Name = "BOk";
            this.BOk.Size = new System.Drawing.Size(75, 23);
            this.BOk.TabIndex = 5;
            this.BOk.Text = "OK";
            this.BOk.UseVisualStyleBackColor = true;
            this.BOk.Click += new System.EventHandler(this.BOk_Click);
            // 
            // BCancel
            // 
            this.BCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BCancel.Location = new System.Drawing.Point(306, 202);
            this.BCancel.Name = "BCancel";
            this.BCancel.Size = new System.Drawing.Size(75, 23);
            this.BCancel.TabIndex = 6;
            this.BCancel.Text = "Cancel";
            this.BCancel.UseVisualStyleBackColor = true;
            // 
            // RBSpecificIpAddress
            // 
            this.RBSpecificIpAddress.AutoSize = true;
            this.RBSpecificIpAddress.Checked = true;
            this.RBSpecificIpAddress.Location = new System.Drawing.Point(21, 34);
            this.RBSpecificIpAddress.Name = "RBSpecificIpAddress";
            this.RBSpecificIpAddress.Size = new System.Drawing.Size(131, 17);
            this.RBSpecificIpAddress.TabIndex = 2;
            this.RBSpecificIpAddress.TabStop = true;
            this.RBSpecificIpAddress.Text = "Specific IPv4 address:";
            this.RBSpecificIpAddress.UseVisualStyleBackColor = true;
            this.RBSpecificIpAddress.CheckedChanged += new System.EventHandler(this.RBSpecificIpAddress_CheckedChanged);
            // 
            // LCaption
            // 
            this.LCaption.AutoSize = true;
            this.LCaption.Location = new System.Drawing.Point(18, 13);
            this.LCaption.Name = "LCaption";
            this.LCaption.Size = new System.Drawing.Size(286, 13);
            this.LCaption.TabIndex = 3;
            this.LCaption.Text = "Allow/Deny access for the following IPv4 address or range:";
            // 
            // TBSpecificIpAddress
            // 
            this.TBSpecificIpAddress.Location = new System.Drawing.Point(40, 58);
            this.TBSpecificIpAddress.Name = "TBSpecificIpAddress";
            this.TBSpecificIpAddress.Size = new System.Drawing.Size(260, 20);
            this.TBSpecificIpAddress.TabIndex = 1;
            this.TBSpecificIpAddress.TextChanged += new System.EventHandler(this.SetOkButtonStatus);
            // 
            // RBAddressRange
            // 
            this.RBAddressRange.AutoSize = true;
            this.RBAddressRange.Location = new System.Drawing.Point(21, 89);
            this.RBAddressRange.Name = "RBAddressRange";
            this.RBAddressRange.Size = new System.Drawing.Size(120, 17);
            this.RBAddressRange.TabIndex = 2;
            this.RBAddressRange.Text = "IPv4 address range:";
            this.RBAddressRange.UseVisualStyleBackColor = true;
            this.RBAddressRange.CheckedChanged += new System.EventHandler(this.RBAddressRange_CheckedChanged);
            // 
            // TBAddressRange
            // 
            this.TBAddressRange.Enabled = false;
            this.TBAddressRange.Location = new System.Drawing.Point(40, 112);
            this.TBAddressRange.Name = "TBAddressRange";
            this.TBAddressRange.Size = new System.Drawing.Size(260, 20);
            this.TBAddressRange.TabIndex = 3;
            this.TBAddressRange.TextChanged += new System.EventHandler(this.SetOkButtonStatus);
            // 
            // LMask
            // 
            this.LMask.AutoSize = true;
            this.LMask.Location = new System.Drawing.Point(37, 137);
            this.LMask.Name = "LMask";
            this.LMask.Size = new System.Drawing.Size(36, 13);
            this.LMask.TabIndex = 7;
            this.LMask.Text = "IPv4 subnet mask:";
            // 
            // TBMask
            // 
            this.TBMask.Enabled = false;
            this.TBMask.Location = new System.Drawing.Point(40, 153);
            this.TBMask.Name = "TBMask";
            this.TBMask.Size = new System.Drawing.Size(260, 20);
            this.TBMask.TabIndex = 4;
            this.TBMask.TextChanged += new System.EventHandler(this.SetOkButtonStatus);
            // 
            // AddExceptionRuleForm
            // 
            this.AcceptButton = this.BOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BCancel;
            this.ClientSize = new System.Drawing.Size(400, 241);
            this.Controls.Add(this.TBMask);
            this.Controls.Add(this.LMask);
            this.Controls.Add(this.TBAddressRange);
            this.Controls.Add(this.RBAddressRange);
            this.Controls.Add(this.TBSpecificIpAddress);
            this.Controls.Add(this.LCaption);
            this.Controls.Add(this.RBSpecificIpAddress);
            this.Controls.Add(this.BCancel);
            this.Controls.Add(this.BOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddExceptionRuleForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Exception Rule";
            this.Load += new System.EventHandler(this.AddExceptionRuleForm_Load);
            this.Shown += new System.EventHandler(this.AddExceptionRuleForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BOk;
        private System.Windows.Forms.Button BCancel;
        private System.Windows.Forms.RadioButton RBSpecificIpAddress;
        private System.Windows.Forms.Label LCaption;
        private System.Windows.Forms.TextBox TBSpecificIpAddress;
        private System.Windows.Forms.RadioButton RBAddressRange;
        private System.Windows.Forms.TextBox TBAddressRange;
        private System.Windows.Forms.Label LMask;
        private System.Windows.Forms.TextBox TBMask;
    }
}