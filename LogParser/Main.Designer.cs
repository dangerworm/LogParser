namespace IISLogViewer
{
    partial class Main
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
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.ofdFilename = new System.Windows.Forms.OpenFileDialog();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblDateTimeFrom = new System.Windows.Forms.Label();
            this.lblFieldValue = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblDateTimeTo = new System.Windows.Forms.Label();
            this.stsStatus = new System.Windows.Forms.StatusStrip();
            this.tslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.btnProcessFile = new System.Windows.Forms.Button();
            this.stsStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.AutoEllipsis = true;
            this.btnSelectFile.Location = new System.Drawing.Point(12, 12);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(85, 23);
            this.btnSelectFile.TabIndex = 0;
            this.btnSelectFile.Text = "Select File(s)";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // ofdFilename
            // 
            this.ofdFilename.Multiselect = true;
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "dd MMM yyyy  HH:mm:ss";
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(80, 45);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(162, 20);
            this.dtpFrom.TabIndex = 3;
            // 
            // lblDateTimeFrom
            // 
            this.lblDateTimeFrom.AutoSize = true;
            this.lblDateTimeFrom.Location = new System.Drawing.Point(12, 48);
            this.lblDateTimeFrom.Name = "lblDateTimeFrom";
            this.lblDateTimeFrom.Size = new System.Drawing.Size(33, 13);
            this.lblDateTimeFrom.TabIndex = 5;
            this.lblDateTimeFrom.Text = "From:";
            // 
            // lblFieldValue
            // 
            this.lblFieldValue.AutoSize = true;
            this.lblFieldValue.Location = new System.Drawing.Point(12, 74);
            this.lblFieldValue.Name = "lblFieldValue";
            this.lblFieldValue.Size = new System.Drawing.Size(62, 13);
            this.lblFieldValue.TabIndex = 6;
            this.lblFieldValue.Text = "Field Value:";
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "dd MMM yyyy  HH:mm:ss";
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(270, 45);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(162, 20);
            this.dtpTo.TabIndex = 7;
            // 
            // lblDateTimeTo
            // 
            this.lblDateTimeTo.AutoSize = true;
            this.lblDateTimeTo.Location = new System.Drawing.Point(248, 48);
            this.lblDateTimeTo.Name = "lblDateTimeTo";
            this.lblDateTimeTo.Size = new System.Drawing.Size(16, 13);
            this.lblDateTimeTo.TabIndex = 8;
            this.lblDateTimeTo.Text = "to";
            // 
            // stsStatus
            // 
            this.stsStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslStatus,
            this.tspProgress});
            this.stsStatus.Location = new System.Drawing.Point(0, 579);
            this.stsStatus.Name = "stsStatus";
            this.stsStatus.Size = new System.Drawing.Size(624, 22);
            this.stsStatus.TabIndex = 10;
            this.stsStatus.Text = "statusStrip1";
            // 
            // tslStatus
            // 
            this.tslStatus.Name = "tslStatus";
            this.tslStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // tspProgress
            // 
            this.tspProgress.Name = "tspProgress";
            this.tspProgress.Size = new System.Drawing.Size(100, 16);
            this.tspProgress.Visible = false;
            // 
            // btnProcessFile
            // 
            this.btnProcessFile.AutoEllipsis = true;
            this.btnProcessFile.Location = new System.Drawing.Point(103, 12);
            this.btnProcessFile.Name = "btnProcessFile";
            this.btnProcessFile.Size = new System.Drawing.Size(85, 23);
            this.btnProcessFile.TabIndex = 11;
            this.btnProcessFile.Text = "Process File(s)";
            this.btnProcessFile.UseVisualStyleBackColor = true;
            this.btnProcessFile.Click += new System.EventHandler(this.btnProcessFile_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 601);
            this.Controls.Add(this.btnProcessFile);
            this.Controls.Add(this.stsStatus);
            this.Controls.Add(this.lblDateTimeTo);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.lblFieldValue);
            this.Controls.Add(this.lblDateTimeFrom);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.btnSelectFile);
            this.Name = "Main";
            this.Text = "Log Parser";
            this.stsStatus.ResumeLayout(false);
            this.stsStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.OpenFileDialog ofdFilename;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblDateTimeFrom;
        private System.Windows.Forms.Label lblFieldValue;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblDateTimeTo;
        private System.Windows.Forms.StatusStrip stsStatus;
        private System.Windows.Forms.ToolStripStatusLabel tslStatus;
        private System.Windows.Forms.ToolStripProgressBar tspProgress;
        private System.Windows.Forms.Button btnProcessFile;
    }
}

