namespace uGST_GSTR1_Json_Generation
{
    partial class frmMain
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
            this.gbxFilter = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnJson = new System.Windows.Forms.Button();
            this.btnJsonFile = new System.Windows.Forms.Button();
            this.txtJsonFile = new System.Windows.Forms.TextBox();
            this.btnSection = new System.Windows.Forms.Button();
            this.btnPeriod = new System.Windows.Forms.Button();
            this.btnFYear = new System.Windows.Forms.Button();
            this.txtSection = new System.Windows.Forms.TextBox();
            this.txtFYear = new System.Windows.Forms.TextBox();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.gbxFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxFilter
            // 
            this.gbxFilter.Controls.Add(this.label5);
            this.gbxFilter.Controls.Add(this.btnPrint);
            this.gbxFilter.Controls.Add(this.label4);
            this.gbxFilter.Controls.Add(this.label3);
            this.gbxFilter.Controls.Add(this.btnJson);
            this.gbxFilter.Controls.Add(this.btnJsonFile);
            this.gbxFilter.Controls.Add(this.txtJsonFile);
            this.gbxFilter.Controls.Add(this.btnSection);
            this.gbxFilter.Controls.Add(this.btnPeriod);
            this.gbxFilter.Controls.Add(this.btnFYear);
            this.gbxFilter.Controls.Add(this.txtSection);
            this.gbxFilter.Controls.Add(this.txtFYear);
            this.gbxFilter.Controls.Add(this.txtPeriod);
            this.gbxFilter.Controls.Add(this.label2);
            this.gbxFilter.Controls.Add(this.label1);
            this.gbxFilter.Controls.Add(this.label6);
            this.gbxFilter.Location = new System.Drawing.Point(11, 9);
            this.gbxFilter.Name = "gbxFilter";
            this.gbxFilter.Size = new System.Drawing.Size(737, 132);
            this.gbxFilter.TabIndex = 0;
            this.gbxFilter.TabStop = false;
            this.gbxFilter.Text = "Details";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Json File Name";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(646, 103);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 16;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(76, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 25);
            this.label4.TabIndex = 9;
            this.label4.Text = "*";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(76, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "*";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnJson
            // 
            this.btnJson.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnJson.Location = new System.Drawing.Point(520, 101);
            this.btnJson.Name = "btnJson";
            this.btnJson.Size = new System.Drawing.Size(120, 25);
            this.btnJson.TabIndex = 15;
            this.btnJson.Text = "Generate Json File";
            this.btnJson.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJson.UseVisualStyleBackColor = true;
            this.btnJson.Click += new System.EventHandler(this.btnJson_Click);
            // 
            // btnJsonFile
            // 
            this.btnJsonFile.Location = new System.Drawing.Point(697, 66);
            this.btnJsonFile.Name = "btnJsonFile";
            this.btnJsonFile.Size = new System.Drawing.Size(24, 23);
            this.btnJsonFile.TabIndex = 14;
            this.btnJsonFile.UseVisualStyleBackColor = true;
            this.btnJsonFile.Click += new System.EventHandler(this.btnJsonFile_Click);
            // 
            // txtJsonFile
            // 
            this.txtJsonFile.Location = new System.Drawing.Point(110, 68);
            this.txtJsonFile.Name = "txtJsonFile";
            this.txtJsonFile.Size = new System.Drawing.Size(582, 20);
            this.txtJsonFile.TabIndex = 13;
            // 
            // btnSection
            // 
            this.btnSection.Location = new System.Drawing.Point(553, 16);
            this.btnSection.Name = "btnSection";
            this.btnSection.Size = new System.Drawing.Size(24, 23);
            this.btnSection.TabIndex = 7;
            this.btnSection.UseVisualStyleBackColor = true;
            this.btnSection.Click += new System.EventHandler(this.btnSection_Click);
            // 
            // btnPeriod
            // 
            this.btnPeriod.Location = new System.Drawing.Point(289, 44);
            this.btnPeriod.Name = "btnPeriod";
            this.btnPeriod.Size = new System.Drawing.Size(24, 23);
            this.btnPeriod.TabIndex = 11;
            this.btnPeriod.UseVisualStyleBackColor = true;
            this.btnPeriod.Click += new System.EventHandler(this.btnPeriod_Click);
            // 
            // btnFYear
            // 
            this.btnFYear.Location = new System.Drawing.Point(289, 20);
            this.btnFYear.Name = "btnFYear";
            this.btnFYear.Size = new System.Drawing.Size(24, 23);
            this.btnFYear.TabIndex = 4;
            this.btnFYear.UseVisualStyleBackColor = true;
            this.btnFYear.Click += new System.EventHandler(this.btnFYear_Click);
            // 
            // txtSection
            // 
            this.txtSection.Location = new System.Drawing.Point(375, 19);
            this.txtSection.Name = "txtSection";
            this.txtSection.Size = new System.Drawing.Size(172, 20);
            this.txtSection.TabIndex = 6;
            // 
            // txtFYear
            // 
            this.txtFYear.Location = new System.Drawing.Point(111, 20);
            this.txtFYear.Name = "txtFYear";
            this.txtFYear.Size = new System.Drawing.Size(172, 20);
            this.txtFYear.TabIndex = 3;
            // 
            // txtPeriod
            // 
            this.txtPeriod.Location = new System.Drawing.Point(111, 44);
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(172, 20);
            this.txtPeriod.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(333, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Section";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Tax Period";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Financial Year";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 145);
            this.Controls.Add(this.gbxFilter);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbxFilter.ResumeLayout(false);
            this.gbxFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbxFilter;
        private System.Windows.Forms.Button btnSection;
        private System.Windows.Forms.Button btnPeriod;
        private System.Windows.Forms.Button btnFYear;
        private System.Windows.Forms.TextBox txtSection;
        private System.Windows.Forms.TextBox txtFYear;
        private System.Windows.Forms.TextBox txtPeriod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnJson;
        private System.Windows.Forms.Button btnJsonFile;
        private System.Windows.Forms.TextBox txtJsonFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label5;
    }
}

