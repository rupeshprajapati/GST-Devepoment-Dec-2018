using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udSaltMast
{
    public partial class Form_upload : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        string SqlStr;
        DataTable dtSalt;
        DataTable dtExcel = new DataTable();
        DataTable dtTemp = new DataTable();
        CheckBox checkboxHeader;
        string tbl;
        string error_message;
        public Form_upload()
        {

            InitializeComponent();
        }

        public Form_upload(string[] args)
        {


            //this.pDisableCloseBtn = true;
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //this.MaximizeBox = false;
            //InitializeComponent();
            //this.pPApplPID = 0;
            //this.pPara = args;
            //this.pFrmCaption = "Salt Upload Details";
            //this.pCompId = Convert.ToInt16(args[0]);
            //this.pComDbnm = args[1];
            //this.pServerName = args[2];
            //this.pUserId = args[3];
            //this.pPassword = args[4];
            //this.pPApplRange = args[5];
            //this.pAppUerName = args[6];
            //Icon MainIcon = new System.Drawing.Icon(args[7].Replace("<*#*>", " "));
            //this.pFrmIcon = MainIcon;
            //this.pPApplText = args[8].Replace("<*#*>", " ");
            //this.pPApplName = args[9];
            //this.pPApplPID = Convert.ToInt16(args[10]);
            //this.pPApplCode = args[11];





        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.AutoGenerateColumns = false;
                string filePath = string.Empty;
                string fileExt = string.Empty;
                OpenFileDialog file = new OpenFileDialog(); //open dialog to choose file  
                if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK) //if there is a file choosen by the user  
                {
                    filePath = file.FileName; //get the path of the file  
                    fileExt = Path.GetExtension(filePath); //get the file extension  

                    if (fileExt.CompareTo(".xls") == 0 /*|| fileExt.CompareTo(".xlsx") == 0*/)
                    {
                        txtFileName.Text = filePath;
                        //  txtExtension.Text = fileExt;
                        try
                        {
                            dtExcel = new DataTable();
                            dtExcel = ReadExcel(filePath, fileExt); //read excel file  
                            dtExcel.Columns.Add("Status");
                            dtExcel.Columns.Add("Error");//rrg
                            dtExcel.Columns.Add("select", typeof(bool));
                            dtExcel.Columns.Add("id_no");

                            setSelect();
                            setRemark();
                            tbl = "dtExcel";
                            bindGrid(dtExcel);
                            setGridSetting();
                            // gridStatus(dtExcel);
                            if (dtExcel.Rows.Count > 0)
                            {
                                ddlRemark.Enabled = true;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        MessageBox.Show("Please choose .xls  file only.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void setSelect()
        {
            int i = 0;
            foreach (DataRow dr in dtExcel.Rows)
            {
                dr["select"] = false;
                dr["id_no"] = (i + 1).ToString();
                i++;
            }
        }

        private void bindGrid(DataTable dtExcelTemp)
        {
            btnDelete.Enabled = false;
            btnUpload.Enabled = false;
            button1.Enabled = false;
            if (dtExcelTemp.Rows.Count > 0)
            {
                DataRow[] dataNewRecord = dtExcelTemp.Select("Status = 'New Record' ");
                if (dtExcelTemp.Rows.Count > 0)
                {
                    btnDelete.Enabled = true;
                    button1.Enabled = true;
                }
                if (dataNewRecord.Length > 0)
                {
                    btnUpload.Enabled = true;
                }
            }

            dataGridView1.DataSource = dtExcelTemp;
            dataGridView1.Columns[0].DataPropertyName = "select";
            // dataGridView1.Columns[1].DataPropertyName = "Salt";
            dataGridView1.Columns[1].DataPropertyName = "GenericName";
            dataGridView1.Columns[2].DataPropertyName = "Indications";
            dataGridView1.Columns[3].DataPropertyName = "Dosage";
            dataGridView1.Columns[4].DataPropertyName = "SideEffects";
            dataGridView1.Columns[5].DataPropertyName = "SpecialPrecautions";
            dataGridView1.Columns[6].DataPropertyName = "DrugInteractions";
            // dataGridView1.Columns[7].DataPropertyName = "Narcotic(Y/N)";
            dataGridView1.Columns[7].DataPropertyName = "Schedule(H/H1/Narcotic/TB)";

            dataGridView1.Columns[8].DataPropertyName = "MaxRate";
            dataGridView1.Columns[9].DataPropertyName = "Continued(Y/N)";
            dataGridView1.Columns[10].DataPropertyName = "Prohibited(Y/N)";
            dataGridView1.Columns[11].DataPropertyName = "Status";
            dataGridView1.Columns[12].DataPropertyName = "Error";
            dataGridView1.AutoGenerateColumns = false;

            lblCount.Text = dtExcelTemp.Rows.Count.ToString();
            //rrg

            Rectangle rect = dataGridView1.GetCellDisplayRectangle(0, -1, true);
            rect.Y = 4;
            rect.X = 6;
            checkboxHeader = new CheckBox();
            checkboxHeader.Name = "checkboxHeader";

            checkboxHeader.Size = new Size(15, 15);
            checkboxHeader.Location = rect.Location;
            checkboxHeader.CheckedChanged += new EventHandler(checkBox1_CheckedChanged);
            dataGridView1.Controls.Add(checkboxHeader);

            //rrg

            ddlRemark.Items.Clear();
            DataView view = new DataView(dtExcel);
            DataTable distinctValues = view.ToTable(true, "Status");
            if (distinctValues.Rows.Count > 0)
            {
                foreach (DataRow dr in distinctValues.Rows)
                {
                    ddlRemark.Items.Add(dr[0].ToString().Trim());
                }
                ddlRemark.Items.Add("All");
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedCell = null;
            dataGridView1.ClearSelection();

            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {

                var chk = row1.Cells["chk"].Value.ToString().Trim();
                if ((chk == "False"))
                {

                    row1.Cells["chk"].Value = true;

                }
                else if ((chk == "True"))
                {
                    if (chk == "True")
                    {
                        row1.Cells["chk"].Value = false;
                    }
                }
            }
            // dataGridView1.RefreshEdit();
            dataGridView1.EndEdit();
        }

        private void setRemark()
        {
            foreach (DataRow row1 in dtExcel.Rows)
            {
                string Salt;
                Salt = row1["GenericName"].ToString().Trim().Replace("'", "''");
                //   Salt = row1["Salt"].ToString().Trim().Replace("'", "''");
                if (row1["Status"].ToString() != "Deleted" && row1["Status"].ToString() != "Inserted" && row1["Status"].ToString() != "Not Inserted")
                {


                    //SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "' and Indications='" + Indications + "' and Dosage='" + Dosage + "' and SideEffects='" + SideEffects + "' and SpePrec='" + SpePrec + "' and DrugInte='" + DrugInte + "' and Narcotic=" + Narcotic + " and ScheduleH=" + ScheduleH + " and ScheduleH1=" + ScheduleH1 + " and MaxRate=" + MaxRate + " and Continued=" + Continued + " and Prohibited=" + Prohibited + "";
                    SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "' and Salt!=''";
                    dtSalt = new DataTable();
                    dtSalt = oDataAccess.GetDataTable(SqlStr, null, 20);

                    if (dtSalt.Rows.Count > 0)
                    {
                        row1["Status"] = "Exists";

                    }
                    else if (dtSalt.Rows.Count == 0)
                    {
                        row1["Status"] = "New Record";



                    }


                }
            }
            foreach (DataRow row1 in dtExcel.Rows)
            {
                string Salt;
                //  Salt = row1["Salt"].ToString().Trim().Replace("'", "''");
                Salt = row1["GenericName"].ToString().Trim().Replace("'", "''");
                DataRow[] foundAuthors = dtExcel.Select("GenericName = '" + Salt + "' and Status <> 'Deleted' and  Status <> 'Exists'");
                if (foundAuthors.Length > 1)
                {
                    row1["Status"] = "Duplicate";

                }
            }

        }

        private void setGridSetting()
        {

            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {
                var remark = row1.Cells["Status"].Value.ToString().Trim();
                if (remark == "Exists" || remark == "Duplicate")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightSteelBlue;

                    //DataGridViewCell cell = dataGridView1.Rows[row1.Index].Cells[0];
                    //DataGridViewCheckBoxCell chkCell = cell as DataGridViewCheckBoxCell;
                    //chkCell.Value = false;
                    //chkCell.FlatStyle = FlatStyle.Flat;
                    //chkCell.Style.ForeColor = Color.DarkGray;
                    //cell.ReadOnly = true;

                    //dataGridView1.Rows[row1.Index].ReadOnly = true;
                }
                else if (remark == "New Record")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else if (remark == "Inserted")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightGreen;

                    //DataGridViewCell cell = dataGridView1.Rows[row1.Index].Cells[0];
                    //DataGridViewCheckBoxCell chkCell = cell as DataGridViewCheckBoxCell;
                    //chkCell.Value = false;
                    //chkCell.FlatStyle = FlatStyle.Flat;
                    //chkCell.Style.ForeColor = Color.DarkGray;
                    //cell.ReadOnly = true;
                }
                else if (remark == "Deleted" || remark == "Not Inserted")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightPink;
                }

            }

        }

        public DataTable ReadExcel(string fileName, string fileExt)
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable();
            if (fileExt.CompareTo(".xls") == 0)
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
            else

                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Sheet1$]", con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable  
                }
                catch (Exception ee)
                {

                    MessageBox.Show("" + ee.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return dtexcel;
        }

        private void ddlRemark_SelectedIndexChanged(object sender, EventArgs e)
        {

            this.dataGridView1.Controls.Remove(checkboxHeader);
            checkboxHeader.Dispose();

            var remark = ddlRemark.SelectedItem.ToString().Trim();
            setSelect();
            dtTemp = new DataTable();
            if (remark != "All")
            {
                if (dtExcel.Rows.Count > 0)
                {
                    DataRow[] foundAuthors = dtExcel.Select("Status = '" + remark + "'");
                    if (foundAuthors.Length > 0)
                    {
                        dtTemp = foundAuthors.CopyToDataTable();
                    }
                    setSelect();
                    setRemark();
                    tbl = "dtTemp";
                    bindGrid(dtTemp);
                    setGridSetting();
                }

            }
            else
            {
                setSelect();
                setRemark();
                bindGrid(dtExcel);
                setGridSetting();
                tbl = "dtExcel";
            }

        }

        private string validate(string Salt, string Schedule, string MaxRate, string Continued, string Prohibited)
        {
            error_message = "";
            double n;
            bool isNumeric = double.TryParse(MaxRate, out n);

            if (Salt == "" && error_message == "")
            {
                error_message = "Generic Name can not be blank.";
            }
            else if ((Schedule != "H" && Schedule != "H1" && Schedule != "Narcotic" && Schedule != "" && Schedule != "TB"))
            {
                error_message = "Schedule value does not match with H/H1/Narcotic/TB.";
            }
            else if (!isNumeric)
            {
                error_message = "MaxRate sholud be numeric.";
            }
            else if (MaxRate == "")
            {
                error_message = "MaxRate can not be blank.";
            }
            else if (Continued.ToString() != "1" && Continued.ToString() != "0")
            {
                error_message = "Continued value does not match with Y/N.";
            }
            else if (Prohibited.ToString() != "1" && Prohibited.ToString() != "0")
            {
                error_message = "Prohibited value does not match with Y/N.";
            }

            return error_message;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (dtTemp.Rows.Count > 0)
            {
                progressBar1.Visible = true;
                progressBar1.Value = 10;

                // MessageBox.Show("temp");
                foreach (DataRow dr in dtTemp.Rows)
                {
                    error_message = "";
                    progressBar1.Value = 30;
                    if (dr["Status"].ToString() == "New Record" && dr["select"].ToString() == "True")
                    {

                        string Salt, Indications, Dosage, SideEffects, SpePrec, DrugInte, Narcotic, Schedule, MaxRate, Continued, Prohibited;
                        //Salt = dr["Salt"].ToString().Trim().Replace("'", "''");
                        Salt = dr["GenericName"].ToString().Trim().Replace("'", "''");
                        Indications = dr["Indications"].ToString().Trim().Replace("'", "''");
                        Dosage = dr["Dosage"].ToString().Trim().Replace("'", "''");
                        SideEffects = dr["SideEffects"].ToString().Trim().Replace("'", "''");
                        SpePrec = dr["SpecialPrecautions"].ToString().Trim().Replace("'", "''");
                        DrugInte = dr["DrugInteractions"].ToString().Trim().Replace("'", "''");

                        //Narcotic = dr["Narcotic(Y/N)"].ToString();
                        //if (Narcotic.ToString().ToLower() == "y")
                        //{
                        //    Narcotic = "1";
                        //}
                        //else if (Narcotic.ToString().ToLower() == "n")
                        //{
                        //    Narcotic = "0";
                        //}
                        Schedule = dr["Schedule(H/H1/Narcotic/TB)"].ToString();

                        MaxRate = dr["MaxRate"].ToString();
                        Continued = dr["Continued(Y/N)"].ToString();
                        if (Continued.ToString().ToLower() == "y")
                        {
                            Continued = "1";
                        }
                        else if (Continued.ToString().ToLower() == "n")
                        {
                            Continued = "0";
                        }
                        Prohibited = dr["Prohibited(Y/N)"].ToString();
                        if (Prohibited.ToString().ToLower() == "y")
                        {
                            Prohibited = "1";
                        }
                        else if (Prohibited.ToString().ToLower() == "n")
                        {
                            Prohibited = "0";
                        }

                        error_message = validate(Salt, Schedule, MaxRate, Continued, Prohibited);
                        try
                        {
                            if ((Schedule == "H" || Schedule == "H1" || Schedule == "Narcotic" || Schedule == "" || Schedule == "TB") && Salt != "")
                            {


                                oDataAccess.BeginTransaction();
                                SqlStr = "set dateformat dmy INSERT INTO pRetSalt_Master(Salt,Indications,Dosage,SideEffects,SpePrec,DrugInte,Schedule,MaxRate,Continued,Prohibited)";
                                SqlStr = SqlStr + " VALUES('" + Salt + "','" + Indications + "','" + Dosage + "','" + SideEffects + "','" + SpePrec + "','" + DrugInte + "','" + Schedule + "'," + MaxRate + "," + Continued + "," + Prohibited + ")";

                                oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                                oDataAccess.CommitTransaction();

                                dr["Status"] = "Inserted";
                            }
                            else
                            {
                                dr["Error"] = error_message;
                                dr["Status"] = "Not Inserted";
                            }
                        }
                        catch (Exception ee)
                        {
                            //   MessageBox.Show(ee.Message.ToString());

                            dr["Error"] = error_message;
                            dr["Status"] = "Not Inserted";
                            oDataAccess.CommitTransaction();
                        }

                    }
                    progressBar1.Value = 80;
                }
                progressBar1.Value = 90;
                setGridSetting();

                progressBar1.Value = 100;

                foreach (DataRow dr in dtTemp.Rows)
                {
                    foreach (DataRow dr1 in dtExcel.Rows)
                    {
                        if (dr["id_no"].ToString() == dr1["id_no"].ToString())
                        {
                            dr1["Status"] = dr["Status"].ToString();
                        }
                    }

                }

            }
            else
            {
                progressBar1.Visible = true;
                progressBar1.Value = 10;
                //  MessageBox.Show("excel");
                foreach (DataRow dr in dtExcel.Rows)
                {
                    error_message = "";
                    progressBar1.Value = 30;
                    if (dr["Status"].ToString() == "New Record" && dr["select"].ToString() == "True")
                    {
                        string Salt, Indications, Dosage, SideEffects, SpePrec, DrugInte, Narcotic, Schedule, MaxRate, Continued, Prohibited;
                     //   Salt = dr["Salt"].ToString().Trim().Replace("'", "''");
                        Salt = dr["GenericName"].ToString().Trim().Replace("'", "''");
                        Indications = dr["Indications"].ToString().Trim().Replace("'", "''");
                        Dosage = dr["Dosage"].ToString().Trim().Replace("'", "''");
                        SideEffects = dr["SideEffects"].ToString().Trim().Replace("'", "''");
                        SpePrec = dr["SpecialPrecautions"].ToString().Trim().Replace("'", "''");
                        DrugInte = dr["DrugInteractions"].ToString().Trim().Replace("'", "''");

                        //Narcotic = dr["Narcotic(Y/N)"].ToString();
                        //if (Narcotic.ToString().ToLower() == "y")
                        //{
                        //    Narcotic = "1";
                        //}
                        //else if (Narcotic.ToString().ToLower() == "n")
                        //{
                        //    Narcotic = "0";
                        //}
                        Schedule = dr["Schedule(H/H1/Narcotic/TB)"].ToString();

                        MaxRate = dr["MaxRate"].ToString();
                        Continued = dr["Continued(Y/N)"].ToString();
                        if (Continued.ToString().ToLower() == "y")
                        {
                            Continued = "1";
                        }
                        else if (Continued.ToString().ToLower() == "n")
                        {
                            Continued = "0";
                        }
                        Prohibited = dr["Prohibited(Y/N)"].ToString();
                        if (Prohibited.ToString().ToLower() == "y")
                        {
                            Prohibited = "1";
                        }
                        else if (Prohibited.ToString().ToLower() == "n")
                        {
                            Prohibited = "0";
                        }
                        error_message = validate(Salt, Schedule, MaxRate, Continued, Prohibited);
                        try
                        {
                            if ((Schedule == "H" || Schedule == "H1" || Schedule == "Narcotic" || Schedule == "" || Schedule == "TB") && Salt != "")
                            {
                                oDataAccess.BeginTransaction();
                                SqlStr = "set dateformat dmy INSERT INTO pRetSalt_Master(Salt,Indications,Dosage,SideEffects,SpePrec,DrugInte,Schedule,MaxRate,Continued,Prohibited)";
                                SqlStr = SqlStr + " VALUES('" + Salt + "','" + Indications + "','" + Dosage + "','" + SideEffects + "','" + SpePrec + "','" + DrugInte + "','" + Schedule + "'," + MaxRate + "," + Continued + "," + Prohibited + ")";

                                oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                                oDataAccess.CommitTransaction();

                                dr["Status"] = "Inserted";
                            }
                            else
                            {
                                dr["Error"] = error_message;
                                dr["Status"] = "Not Inserted";

                            }
                        }
                        catch (Exception ee)
                        {
                            // MessageBox.Show(ee.Message.ToString());
                            dr["Error"] = error_message;
                            dr["Status"] = "Not Inserted";
                            oDataAccess.CommitTransaction();
                        }
                    }
                    progressBar1.Value = 80;
                }
                progressBar1.Value = 90;
                setGridSetting();
                progressBar1.Value = 100;
            }

            progressBar1.Visible = false;
            progressBar1.Value = 0;

            this.dataGridView1.Controls.Remove(checkboxHeader);
            checkboxHeader.Dispose();

            setSelect();
            setRemark();
            tbl = "dtExcel";
            bindGrid(dtExcel);
            setGridSetting();
            // gridStatus(dtExcel);
            if (dtExcel.Rows.Count > 0)
            {
                ddlRemark.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataAccess_Net.clsDataAccess._databaseName = this.pComDbnm;
            DataAccess_Net.clsDataAccess._serverName = this.pServerName;
            DataAccess_Net.clsDataAccess._userID = this.pUserId;
            DataAccess_Net.clsDataAccess._password = this.pPassword;
            oDataAccess = new DataAccess_Net.clsDataAccess();

            CultureInfo ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            btnDelete.Enabled = false;
            btnUpload.Enabled = false;
            button1.Enabled = false;
            panel2.Size = new Size(this.Width - 25, this.Height - 80);
            dataGridView1.Size = new Size(this.Width - 30, this.Height - 145);
            lblProgress.Visible = false;
            progressBar1.Visible = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            this.dataGridView1.Controls.Remove(checkboxHeader);
            checkboxHeader.Dispose();

            if (tbl == "dtExcel")
            {
                foreach (DataRow dr in dtExcel.Rows)
                {
                    if (dr["select"].ToString() == "True")
                    {
                        dr["Status"] = "Deleted";
                        //  dr.Delete();

                    }
                }
                //  dtExcel.AcceptChanges();

                setRemark();
                bindGrid(dtExcel);
                setGridSetting();

                if (dtExcel.Rows.Count > 0)
                {
                    ddlRemark.Enabled = true;
                }
                setSelect();
            }
            else if (tbl == "dtTemp")
            {
                foreach (DataRow dr in dtTemp.Rows)
                {
                    if (dr["select"].ToString() == "True")
                    {
                        dr["Status"] = "Deleted";
                        //dr.Delete();

                    }
                }
                dtTemp.AcceptChanges();

                // setRemark();

                foreach (DataRow row1 in dtTemp.Rows)
                {
                    string Salt; 
                   //  Salt = row1["Salt"].ToString();
                    Salt = row1["GenericName"].ToString();
                    if (row1["Status"].ToString() != "Deleted" && row1["Status"].ToString() != "Inserted" && row1["Status"].ToString() != "Not Inserted")
                    {


                        //SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "' and Indications='" + Indications + "' and Dosage='" + Dosage + "' and SideEffects='" + SideEffects + "' and SpePrec='" + SpePrec + "' and DrugInte='" + DrugInte + "' and Narcotic=" + Narcotic + " and ScheduleH=" + ScheduleH + " and ScheduleH1=" + ScheduleH1 + " and MaxRate=" + MaxRate + " and Continued=" + Continued + " and Prohibited=" + Prohibited + "";
                        SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "'";
                        dtSalt = new DataTable();
                        dtSalt = oDataAccess.GetDataTable(SqlStr, null, 20);

                        if (dtSalt.Rows.Count > 0)
                        {
                            row1["Status"] = "Exists";

                        }
                        else if (dtSalt.Rows.Count == 0)
                        {
                            row1["Status"] = "New Record";



                        }


                    }
                }
                foreach (DataRow row1 in dtTemp.Rows)
                {
                    string Salt; 
                    // Salt = row1["Salt"].ToString();
                    Salt = row1["GenericName"].ToString();
                    DataRow[] foundAuthors = dtTemp.Select("GenericName = '" + Salt + "' and Status <> 'Deleted' and  Status <> 'Exists'");
                    if (foundAuthors.Length > 1)
                    {
                        row1["Status"] = "Duplicate";

                    }
                }






                bindGrid(dtTemp);
                setGridSetting();

                if (dtExcel.Rows.Count > 0)
                {
                    ddlRemark.Enabled = true;
                }
                foreach (DataRow dr in dtTemp.Rows)
                {
                    foreach (DataRow dr1 in dtExcel.Rows)
                    {
                        if (dr["id_no"].ToString() == dr1["id_no"].ToString())
                        {
                            dr1["Status"] = dr["Status"].ToString();
                        }
                    }

                }
                setSelect();

            }


        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string getFileName(string fname)
        {
            string DfilePath, filename, FilePath = "";
            string date = DateTime.Now.ToString("dd/MM/yy") + "_" + DateTime.Now.ToString("hh:mm:ss tt");

            FolderBrowserDialog vFolderBrowserDialog1 = new FolderBrowserDialog();
            if (vFolderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DfilePath = vFolderBrowserDialog1.SelectedPath;

                progressBar1.Value = 0;
                progressBar1.Value = 10;
                lblProgress.Visible = true;
                progressBar1.Visible = true;

                filename = fname + date;
                filename = filename.Replace(".", "").Replace("-", "").Replace("/", "").Replace(":", "").Replace("__", "_").Replace(" ", "");
                //  FilePath = DfilePath + filename+".xls";
                FilePath = DfilePath + filename;
                FilePath = FilePath.Replace("\\\\", "\\");
            }

            return FilePath;
        }

        public void ExportToExcel(DataTable dt_record, string excelFilename)
        {
            try
            {
                lblProgress.Text = "Opening Excel Application.....";
                progressBar1.Value = 30;
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                progressBar1.Value = 40;
                Microsoft.Office.Interop.Excel.Workbook wBook;

                progressBar1.Value = 50;
                Microsoft.Office.Interop.Excel.Worksheet wSheet;

                progressBar1.Value = 60;
                wBook = excel.Workbooks.Add(System.Reflection.Missing.Value);

                progressBar1.Value = 70;
                wSheet = (Microsoft.Office.Interop.Excel.Worksheet)wBook.ActiveSheet;

                progressBar1.Value = 80;
                System.Data.DataTable dt = dt_record;
                System.Data.DataColumn dc = new DataColumn();

                progressBar1.Value = 90;
                int colIndex = 0;
                float rowIndex = 1;
                progressBar1.Value = 100;

                foreach (DataColumn dcol in dt.Columns)
                {
                    colIndex = colIndex + 1;
                    excel.Cells[rowIndex, colIndex] = dcol.ColumnName;
                }

                progressBar1.Value = 0;
                rowIndex += 1;
                float nrow = 1;
                excel.Columns.NumberFormat = "@";
                foreach (DataRow drow in dt.Rows)
                {
                    colIndex = 0;
                    foreach (DataColumn dcol in dt.Columns)
                    {
                        colIndex = colIndex + 1;
                        excel.Cells[rowIndex, colIndex] = drow[dcol.ColumnName].ToString();

                    }
                    float aa = nrow == dt.Rows.Count ? 100.00f : (nrow / dt.Rows.Count) * 100;

                    progressBar1.Value = (Int32)aa;
                    progressBar1.Refresh();
                    progressBar1.Refresh();
                    lblProgress.Text = "Read Records: " + nrow.ToString() + "/" + dt.Rows.Count;

                    nrow = nrow + 1;
                    rowIndex = rowIndex + 1;
                }
                progressBar1.Value = 100;
                progressBar1.Refresh();



                object misValue = System.Reflection.Missing.Value;
                excel.Columns.AutoFit();
                excel.ActiveWorkbook.SaveAs(excelFilename + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                excel.ActiveWorkbook.Saved = true;

                progressBar1.Visible = false;
                lblProgress.Visible = false;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                MessageBox.Show("Excel Application not available", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                progressBar1.Value = 0;
                progressBar1.Visible = false;
                lblProgress.Visible = false;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DataTable dtRecord = new DataTable();
            //  dtRecord.Columns.Add("Salt");
            dtRecord.Columns.Add("GenericName");
            dtRecord.Columns.Add("Indications");
            dtRecord.Columns.Add("Dosage");
            dtRecord.Columns.Add("SideEffects");
            dtRecord.Columns.Add("SpecialPrecautions");
            dtRecord.Columns.Add("DrugInteractions");
            // dtRecord.Columns.Add("Narcotic(Y/N)");
            dtRecord.Columns.Add("Schedule(H/H1/Narcotic/TB)");
            dtRecord.Columns.Add("MaxRate");
            dtRecord.Columns.Add("Continued(Y/N)");
            dtRecord.Columns.Add("Prohibited(Y/N)");

            if (dtRecord != null)
            {
                string file_path = getFileName("\\GenericDrugMaster_Format_");
                if (file_path != "")
                {
                    progressBar1.Value = 20;
                    ExportToExcel(dtRecord, file_path);
                    MessageBox.Show("Default format download successfully at " + file_path + ".xls", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            if (dtExcel.Rows.Count > 0)
            {
                string file_path = getFileName("\\GenericDrugMaster_Log_");
                if (file_path != "")
                {
                    progressBar1.Value = 20;
                    DataView view = new DataView(dtExcel);
                    DataTable dtLogRecord = view.ToTable(false, "GenericName", "Indications", "Dosage", "SideEffects", "SpecialPrecautions", "DrugInteractions", "Schedule(H/H1/Narcotic/TB)", "MaxRate", "Continued(Y/N)", "Prohibited(Y/N)", "Status", "Error");//Add RRG
                    //DataTable dtLogRecord = view.ToTable(false, "Salt", "Indications", "Dosage", "SideEffects", "SpecialPrecautions", "DrugInteractions", "Schedule(H/H1/Narcotic)", "MaxRate", "Continued(Y/N)", "Prohibited(Y/N)", "Status");//Comment RRG
                    ExportToExcel(dtLogRecord, file_path);
                    MessageBox.Show("Log download successfully at " + file_path + ".xls", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("No Record found..!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnLogout.Enabled)
                btnLogout_Click(this.btnLogout, e);
        }

        //private void gridStatus(DataTable dtExcelTemp)
        //{

        //    foreach (DataGridViewRow row1 in dataGridView1.Rows)
        //    {
        //        string Salt, Indications, Dosage, SideEffects, SpePrec, DrugInte, Narcotic, ScheduleH, ScheduleH1, MaxRate, Continued, Prohibited;
        //        Salt = row1.Cells[1].Value.ToString();
        //        Indications = row1.Cells[2].Value.ToString();
        //        Dosage = row1.Cells[3].Value.ToString();
        //        SideEffects = row1.Cells[4].Value.ToString();
        //        SpePrec = row1.Cells[5].Value.ToString();
        //        DrugInte = row1.Cells[6].Value.ToString();
        //        Narcotic = row1.Cells[7].Value.ToString();
        //        ScheduleH = row1.Cells[8].Value.ToString();
        //        ScheduleH1 = row1.Cells[9].Value.ToString();
        //        MaxRate = row1.Cells[10].Value.ToString();
        //        Continued = row1.Cells[11].Value.ToString();
        //        Prohibited = row1.Cells[12].Value.ToString();

        //        //SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "' and Indications='" + Indications + "' and Dosage='" + Dosage + "' and SideEffects='" + SideEffects + "' and SpePrec='" + SpePrec + "' and DrugInte='" + DrugInte + "' and Narcotic=" + Narcotic + " and ScheduleH=" + ScheduleH + " and ScheduleH1=" + ScheduleH1 + " and MaxRate=" + MaxRate + " and Continued=" + Continued + " and Prohibited=" + Prohibited + "";
        //        SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "'";
        //        dtSalt = new DataTable();
        //        dtSalt = oDataAccess.GetDataTable(SqlStr, null, 20);

        //        if (dtSalt.Rows.Count > 0)
        //        {
        //            row1.Cells[13].Value = "Exists";
        //            row1.DefaultCellStyle.BackColor = Color.LightPink;
        //        }
        //        else if (dtSalt.Rows.Count == 0)
        //        {
        //            row1.Cells[13].Value = "New Record";

        //            //SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "' ";
        //            //dtSalt = new DataTable();
        //            //dtSalt = oDataAccess.GetDataTable(SqlStr, null, 20);
        //            //if (dtSalt.Rows.Count > 0)
        //            //{
        //            //    row1.Cells[13].Value = "New Update";
        //            //    row1.DefaultCellStyle.BackColor = Color.LightYellow;
        //            //}
        //        }

        //        DataRow[] foundAuthors = dtExcelTemp.Select("Salt = '" + Salt + "'");
        //        if (foundAuthors.Length > 1)
        //        {
        //            row1.Cells[13].Value = "Duplicate";
        //            row1.DefaultCellStyle.BackColor = Color.LightPink;
        //        }
        //    }
        //}
    }
}
