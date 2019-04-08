namespace udMarketingCompanyMast
{
    partial class Form_upload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_upload));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLogout = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnUpload = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlRemark = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column25 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLogout});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(959, 36);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLogout
            // 
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnLogout.ForeColor = System.Drawing.Color.Red;
            this.btnLogout.Image = ((System.Drawing.Image)(resources.GetObject("btnLogout.Image")));
            this.btnLogout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(29, 33);
            this.btnLogout.Text = "Exit";
            this.btnLogout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLogout.ToolTipText = "Exit(Ctrl+F4)";
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.btnDownload);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.lblProgress);
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.btnUpload);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.lblCount);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(3, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1079, 381);
            this.panel2.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Image = global::udMarketingCompanyMast.Properties.Resources.d1;
            this.button1.Location = new System.Drawing.Point(796, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 27);
            this.button1.TabIndex = 138;
            this.button1.Text = "Download Log";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Image = global::udMarketingCompanyMast.Properties.Resources.d1;
            this.btnDownload.Location = new System.Drawing.Point(641, 32);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(156, 27);
            this.btnDownload.TabIndex = 137;
            this.btnDownload.Text = "Download DefaultFormat";
            this.btnDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::udMarketingCompanyMast.Properties.Resources.delete;
            this.btnDelete.Location = new System.Drawing.Point(425, 32);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 136;
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Location = new System.Drawing.Point(960, 20);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(47, 16);
            this.lblProgress.TabIndex = 135;
            this.lblProgress.Text = "label5";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(957, 39);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(116, 16);
            this.progressBar1.TabIndex = 134;
            // 
            // btnUpload
            // 
            this.btnUpload.Image = global::udMarketingCompanyMast.Properties.Resources.u1;
            this.btnUpload.Location = new System.Drawing.Point(500, 32);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(140, 27);
            this.btnUpload.TabIndex = 7;
            this.btnUpload.Text = "Upload into Database";
            this.btnUpload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(430, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Total Record :";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Location = new System.Drawing.Point(534, 17);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 13);
            this.lblCount.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label16);
            this.panel3.Controls.Add(this.txtFileName);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.btnSelectFile);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.ddlRemark);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(422, 62);
            this.panel3.TabIndex = 3;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.Red;
            this.label16.Location = new System.Drawing.Point(80, 14);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(15, 19);
            this.label16.TabIndex = 50;
            this.label16.Text = "*";
            // 
            // txtFileName
            // 
            this.txtFileName.Enabled = false;
            this.txtFileName.Location = new System.Drawing.Point(101, 14);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(278, 20);
            this.txtFileName.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Select Status";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Image = global::udMarketingCompanyMast.Properties.Resources.open;
            this.btnSelectFile.Location = new System.Drawing.Point(381, 14);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(37, 21);
            this.btnSelectFile.TabIndex = 0;
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Name";
            // 
            // ddlRemark
            // 
            this.ddlRemark.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlRemark.Enabled = false;
            this.ddlRemark.FormattingEnabled = true;
            this.ddlRemark.Items.AddRange(new object[] {
            "Exists",
            "New Record",
            "Duplicate",
            "Deleted",
            "All"});
            this.ddlRemark.Location = new System.Drawing.Point(101, 38);
            this.ddlRemark.Name = "ddlRemark";
            this.ddlRemark.Size = new System.Drawing.Size(317, 21);
            this.ddlRemark.TabIndex = 4;
            this.ddlRemark.SelectedIndexChanged += new System.EventHandler(this.ddlRemark_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chk,
            this.Column1,
            this.Column25,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Column14,
            this.Column15,
            this.Column16,
            this.Column17,
            this.Column18,
            this.Column19,
            this.Column20,
            this.Column13,
            this.Column22,
            this.Column21,
            this.Column23,
            this.Status,
            this.Column7});
            this.dataGridView1.Location = new System.Drawing.Point(-1, 61);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(913, 311);
            this.dataGridView1.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 36);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(959, 24);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // chk
            // 
            this.chk.Frozen = true;
            this.chk.HeaderText = "";
            this.chk.Name = "chk";
            this.chk.Width = 20;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Name";
            this.Column1.Name = "Column1";
            // 
            // Column25
            // 
            this.Column25.HeaderText = "Code";
            this.Column25.Name = "Column25";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Contact Person";
            this.Column2.Name = "Column2";
            this.Column2.Width = 150;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Designation";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Address 1";
            this.Column4.Name = "Column4";
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Address 2";
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Address 3";
            this.Column6.Name = "Column6";
            // 
            // Column8
            // 
            this.Column8.HeaderText = "Area";
            this.Column8.Name = "Column8";
            // 
            // Column9
            // 
            this.Column9.HeaderText = "Zone";
            this.Column9.Name = "Column9";
            // 
            // Column10
            // 
            this.Column10.HeaderText = "CITY";
            this.Column10.Name = "Column10";
            // 
            // Column11
            // 
            this.Column11.HeaderText = "District";
            this.Column11.Name = "Column11";
            // 
            // Column12
            // 
            this.Column12.HeaderText = "State";
            this.Column12.Name = "Column12";
            // 
            // Column14
            // 
            this.Column14.HeaderText = "Zip";
            this.Column14.Name = "Column14";
            // 
            // Column15
            // 
            this.Column15.HeaderText = "Country";
            this.Column15.Name = "Column15";
            // 
            // Column16
            // 
            this.Column16.HeaderText = "Office Tel";
            this.Column16.Name = "Column16";
            // 
            // Column17
            // 
            this.Column17.HeaderText = "Fax";
            this.Column17.Name = "Column17";
            // 
            // Column18
            // 
            this.Column18.HeaderText = "Resi.Tel";
            this.Column18.Name = "Column18";
            // 
            // Column19
            // 
            this.Column19.HeaderText = "Cell No";
            this.Column19.Name = "Column19";
            // 
            // Column20
            // 
            this.Column20.HeaderText = "Email Id";
            this.Column20.Name = "Column20";
            // 
            // Column13
            // 
            this.Column13.HeaderText = "Drug Lic. No.";
            this.Column13.Name = "Column13";
            // 
            // Column22
            // 
            this.Column22.HeaderText = "Drug Lic. Exp.";
            this.Column22.Name = "Column22";
            // 
            // Column21
            // 
            this.Column21.HeaderText = "FSSAI No.";
            this.Column21.Name = "Column21";
            // 
            // Column23
            // 
            this.Column23.HeaderText = "FSSAI Exp.";
            this.Column23.Name = "Column23";
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Error";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column7.Width = 250;
            // 
            // Form_upload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 417);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form_upload";
            this.Text = "Form_upload";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form_upload_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLogout;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlRemark;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chk;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column25;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column19;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column20;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column22;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column21;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column23;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
    }
}