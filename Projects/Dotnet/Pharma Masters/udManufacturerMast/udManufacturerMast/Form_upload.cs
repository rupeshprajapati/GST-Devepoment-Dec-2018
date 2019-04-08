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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udManufacturerMast
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

        private void Form_upload_Load(object sender, EventArgs e)
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
            this.dataGridView1.Columns[1].Frozen = true;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnLogout.Enabled)
                btnLogout_Click(this.btnLogout, e);
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

        private void setRemark()
        {
            foreach (DataRow row1 in dtExcel.Rows)
            {
                string Name;
                Name = row1["Name"].ToString().Trim().Replace("'", "''");
                if (row1["Status"].ToString() != "Deleted" && row1["Status"].ToString() != "Inserted" && row1["Status"].ToString() != "Not Inserted")
                {


                    //SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "' and Indications='" + Indications + "' and Dosage='" + Dosage + "' and SideEffects='" + SideEffects + "' and SpePrec='" + SpePrec + "' and DrugInte='" + DrugInte + "' and Narcotic=" + Narcotic + " and ScheduleH=" + ScheduleH + " and ScheduleH1=" + ScheduleH1 + " and MaxRate=" + MaxRate + " and Continued=" + Continued + " and Prohibited=" + Prohibited + "";
                    SqlStr = "select * from pRetManufacturerMast where Name='" + Name + "' and Name!=''";
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
                string Name;
                Name = row1["Name"].ToString().Trim().Replace("'", "''");
                DataRow[] foundAuthors = dtExcel.Select("Name = '" + Name + "' and Status <> 'Deleted' and  Status <> 'Exists'");
                if (foundAuthors.Length > 1)
                {
                    row1["Status"] = "Duplicate";
                }
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
            dataGridView1.Columns[1].DataPropertyName = "Name";
            dataGridView1.Columns[2].DataPropertyName = "Code";
            dataGridView1.Columns[3].DataPropertyName = "ContactPerson";
            dataGridView1.Columns[4].DataPropertyName = "Designation";
            dataGridView1.Columns[5].DataPropertyName = "Address1";
            dataGridView1.Columns[6].DataPropertyName = "Address2";
            dataGridView1.Columns[7].DataPropertyName = "Address3";
            dataGridView1.Columns[8].DataPropertyName = "Area";
            dataGridView1.Columns[9].DataPropertyName = "Zone";
            dataGridView1.Columns[10].DataPropertyName = "CITY";
            dataGridView1.Columns[11].DataPropertyName = "District";
            dataGridView1.Columns[12].DataPropertyName = "State";
            //  dataGridView1.Columns[12].DataPropertyName = "StateCode";
            dataGridView1.Columns[13].DataPropertyName = "Zip";
            dataGridView1.Columns[14].DataPropertyName = "Country";
            dataGridView1.Columns[15].DataPropertyName = "OfficeTel";
            dataGridView1.Columns[16].DataPropertyName = "Fax";
            dataGridView1.Columns[17].DataPropertyName = "ResiTel";
            dataGridView1.Columns[18].DataPropertyName = "CellNo";
            dataGridView1.Columns[19].DataPropertyName = "EmailId";

            dataGridView1.Columns[20].DataPropertyName = "DrugLicNo";
            dataGridView1.Columns[21].DataPropertyName = "DrugLicExpDate";
            dataGridView1.Columns[22].DataPropertyName = "FSSAINo";
            dataGridView1.Columns[23].DataPropertyName = "FSSAIExpDate";
            dataGridView1.Columns[24].DataPropertyName = "MarketedBy";

            dataGridView1.Columns[25].DataPropertyName = "Status";
            dataGridView1.Columns[26].DataPropertyName = "Error";
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

        private void setGridSetting()
        {

            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {
                var remark = row1.Cells["Status"].Value.ToString().Trim();
                if (remark == "Exists" || remark == "Duplicate")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                }
                else if (remark == "New Record")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else if (remark == "Inserted")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else if (remark == "Deleted" || remark == "Not Inserted")
                {
                    row1.DefaultCellStyle.BackColor = Color.LightPink;
                }

            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DataTable dtRecord = new DataTable();
            dtRecord.Columns.Add("Name");
            dtRecord.Columns.Add("Code");
            dtRecord.Columns.Add("ContactPerson");
            dtRecord.Columns.Add("Designation");
            dtRecord.Columns.Add("Address1");
            dtRecord.Columns.Add("Address2");
            dtRecord.Columns.Add("Address3");
            dtRecord.Columns.Add("Area");
            dtRecord.Columns.Add("Zone");
            dtRecord.Columns.Add("CITY");
            dtRecord.Columns.Add("District");
            dtRecord.Columns.Add("State");
            dtRecord.Columns.Add("Zip");
            dtRecord.Columns.Add("Country");
            dtRecord.Columns.Add("OfficeTel");
            dtRecord.Columns.Add("Fax");
            dtRecord.Columns.Add("ResiTel");
            dtRecord.Columns.Add("CellNo");
            dtRecord.Columns.Add("EmailId");

            dtRecord.Columns.Add("DrugLicNo");
            dtRecord.Columns.Add("DrugLicExpDate");
            dtRecord.Columns.Add("FSSAINo");
            dtRecord.Columns.Add("FSSAIExpDate");
            dtRecord.Columns.Add("MarketedBy");


            if (dtRecord != null)
            {
                string file_path = getFileName("\\Manufacturer_Format_");
                if (file_path != "")
                {
                    progressBar1.Value = 20;
                    ExportToExcel(dtRecord, file_path);
                    MessageBox.Show("Default format download successfully at " + file_path + ".xls", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }

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

        private void button1_Click(object sender, EventArgs e)
        {
            if (dtExcel.Rows.Count > 0)
            {
                string file_path = getFileName("\\Manufacturer_Log_");
                if (file_path != "")
                {
                    progressBar1.Value = 20;
                    DataView view = new DataView(dtExcel);
                    DataTable dtLogRecord = view.ToTable(false, "Name", "Code", "ContactPerson", "Designation", "Address1", "Address2", "Address3", "Area", "Zone", "CITY", "District", "State", "Zip", "Country", "OfficeTel", "Fax", "ResiTel", "CellNo", "EmailId", "DrugLicNo", "DrugLicExpDate", "FSSAINo", "FSSAIExpDate", "MarketedBy", "Status", "Error");//Add RRG
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
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
                        }
                    }

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

                        }
                    }
                    dtTemp.AcceptChanges();


                    foreach (DataRow row1 in dtTemp.Rows)
                    {
                        string Name;
                        Name = row1["Name"].ToString();
                        if (row1["Status"].ToString() != "Deleted" && row1["Status"].ToString() != "Inserted" && row1["Status"].ToString() != "Not Inserted")
                        {


                            //SqlStr = "select * from pRetSalt_Master where Salt='" + Salt + "' and Indications='" + Indications + "' and Dosage='" + Dosage + "' and SideEffects='" + SideEffects + "' and SpePrec='" + SpePrec + "' and DrugInte='" + DrugInte + "' and Narcotic=" + Narcotic + " and ScheduleH=" + ScheduleH + " and ScheduleH1=" + ScheduleH1 + " and MaxRate=" + MaxRate + " and Continued=" + Continued + " and Prohibited=" + Prohibited + "";
                            SqlStr = "select * from pRetManufacturerMast where Name='" + Name + "'";
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
                        string Name;
                        Name = row1["Name"].ToString();
                        DataRow[] foundAuthors = dtTemp.Select("Name = '" + Name + "' and Status <> 'Deleted' and  Status <> 'Exists'");
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

                    int i = 0;
                    foreach (DataRow dr in dtTemp.Rows)
                    {
                        dr["select"] = false;
                        dr["id_no"] = (i + 1).ToString();
                        i++;
                    }

                }
            }
            catch (Exception ee)
            {

            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            DataTable dtUpData = new DataTable();
            int count = 0;
            if (dtTemp.Rows.Count > 0)
            {
                dtUpData = dtTemp;
                count++;
            }
            else
            {
                dtUpData = dtExcel;
            }

            //insert Code Start---
            progressBar1.Visible = true;
            progressBar1.Value = 10;
            foreach (DataRow dr in dtUpData.Rows)
            {


                error_message = "";
                progressBar1.Value = 30;
                if (dr["Status"].ToString() == "New Record" && dr["select"].ToString() == "True")
                {
                    string mName, contact, designation, add1, add2, add3, Area, Zone, CITY, District, state, stateCode, zip, country, office, fax, phoner, cellno, email, prDrugLicNo, prFoodLicNo, marketedby, mcode;
                    int City_id, State_id, Country_id;

                    DateTime prDrugExpDate = DateTime.MinValue, prFoodExpDate = DateTime.MinValue;

                    mName = dr["Name"].ToString().Trim().Replace("'", "''");
                    mcode = dr["Code"].ToString().Trim().Replace("'", "''");
                    contact = dr["ContactPerson"].ToString().Trim().Trim().Replace("'", "''");
                    designation = dr["Designation"].ToString().Trim().Trim().Replace("'", "''");
                    add1 = dr["Address1"].ToString().Trim().Replace("'", "''");
                    add2 = dr["Address2"].ToString().Trim().Replace("'", "''");
                    add3 = dr["Address3"].ToString().Trim().Replace("'", "''");
                    Area = dr["Area"].ToString().Trim().Trim().Replace("'", "''");
                    Zone = dr["Zone"].ToString().Trim().Trim().Replace("'", "''");
                    CITY = dr["CITY"].ToString().Trim().Trim().Replace("'", "''");
                    City_id = getCityDetails(CITY);
                    District = dr["District"].ToString().Trim().Trim().Replace("'", "''");
                    state = dr["State"].ToString().Trim().Trim().Replace("'", "''");
                    stateCode = getStateCode(state);
                    State_id = getStateDetails(stateCode);
                    zip = dr["Zip"].ToString().Trim().Trim().Replace("'", "''");
                    country = dr["Country"].ToString().Trim().Trim().Replace("'", "''");
                    Country_id = getCountryDetails(country);
                    if (Country_id == 251)
                    {
                        country = "India";
                        Country_id = getCountryDetails(country);
                    }
                    office = dr["OfficeTel"].ToString().Trim();
                    fax = dr["Fax"].ToString().Trim().Trim().Replace("'", "''");
                    phoner = dr["ResiTel"].ToString().Trim();
                    cellno = dr["CellNo"].ToString().Trim();
                    email = dr["EmailId"].ToString().Trim().Replace("'", "''"); ;

                    prDrugLicNo = dr["DrugLicNo"].ToString().Trim().Replace("'", "''");

                    prFoodLicNo = dr["FSSAINo"].ToString().Trim().Replace("'", "''");

                    marketedby = dr["MarketedBy"].ToString().Trim().Replace("'", "''");

                    try
                    {
                        prDrugExpDate = DateTime.Parse(dr["DrugLicExpDate"].ToString().Trim());
                        prFoodExpDate = DateTime.Parse(dr["FSSAIExpDate"].ToString().Trim());
                    }
                    catch (Exception ex)
                    {

                    }

                    error_message = validate(mName, stateCode, Country_id, zip, office, phoner, cellno, email, mcode, marketedby, prDrugExpDate, prFoodExpDate);
                    if (stateCode == "0")
                    {
                        stateCode = "";
                    }
                    try
                    {
                        if (mName != "" && error_message == "")
                        {


                            oDataAccess.BeginTransaction();

                            SqlStr = "set dateformat dmy INSERT INTO pRetManufacturerMast(Name,contact,designatio,add1,add2,add3";
                            SqlStr = SqlStr + ", Area, Zone, city, City_id, District,[state], StateCode, State_id, zip, country, Country_id, phone, fax, phoner, mobile, email,prDrugLicNo,prDrugExpDate,prFoodLicNo,prFoodExpDate,marketedby,mcode)";
                            SqlStr = SqlStr + " VALUES('" + mName + "', '" + contact + "', '" + designation + "' ";
                            SqlStr = SqlStr + ", '" + add1 + "', '" + add2 + "', '" + add3 + "', '" + Area + "', '" + Zone + "', '" + CITY + "', " + City_id + ", '" + District + "', '" + state + "' ";
                            SqlStr = SqlStr + ", '" + stateCode + "', " + State_id + ", '" + zip + "', '" + country + "', " + Country_id + ", '" + office + "', '" + fax + "', '" + phoner + "', '" + cellno + "', '" + email + "','" + prDrugLicNo + "','" + prDrugExpDate + "','" + prFoodLicNo + "','" + prFoodExpDate + "','" + marketedby + "','" + mcode + "')";

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
            //insert code End----

            if (count != 0)
            {
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

        public int getCityDetails(string city)
        {
            SqlStr = "Select City_id from city where city='" + city + "'";
            DataTable dtCity = new DataTable();
            dtCity = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtCity.Rows.Count > 0)
            {
                return Int32.Parse(dtCity.Rows[0][0].ToString());
            }
            else
            {
                SqlStr = "INSERT INTO city (city)VALUES('" + city + "')";
                oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                return getCityDetails(city);
            }
        }

        public int getStateDetails(string stateCode)
        {
            SqlStr = "Select state_id from state where Gst_State_Code='" + stateCode + "'";
            DataTable dtState = new DataTable();
            dtState = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtState.Rows.Count > 0)
            {
                return Int32.Parse(dtState.Rows[0][0].ToString());
            }
            else
            {
                return 37;
            }
        }
        public string getStateCode(string state)
        {
            SqlStr = "Select Gst_State_Code from state where state='" + state + "'";
            DataTable dtState = new DataTable();
            dtState = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtState.Rows.Count > 0)
            {
                return dtState.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }
        public int getCountryDetails(string country)
        {
            SqlStr = "Select country_id from Country where Country='" + country + "'";
            DataTable dtCountry = new DataTable();
            dtCountry = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtCountry.Rows.Count > 0)
            {
                return Int32.Parse(dtCountry.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public string validate(string name, string stateCode, int countryid, string zip, string office, string phoner, string cellno, string email, string mcode, string marketedby, DateTime prDrugExpDate, DateTime prFoodExpDate)
        {
            error_message = "";
            int n;
            bool isNumeric = int.TryParse(zip, out n);

            double n1;
            bool isNumericoffice = double.TryParse(office, out n1);

            double n2;
            bool isNumericphoner = double.TryParse(phoner, out n2);

            double n3;
            bool isNumericcellno = double.TryParse(cellno, out n3);

            Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

            DateTime Date = DateTime.MinValue;

            SqlStr = "select * from pRetManufacturerMast where Name='" + name + "'";
            DataTable dtManufacturer = new DataTable();
            dtManufacturer = oDataAccess.GetDataTable(SqlStr, null, 20);

            SqlStr = "select Name from pRetDrugMarketingMast where Name='" + marketedby + "'";
            DataTable dtMarketed = new DataTable();
            dtMarketed = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (name.ToString().Trim() == "")
            {
                error_message = "Manufacturer Name cannot be empty.";

            }
            else if (mcode.Length > 10)
            {
                error_message = "Manufacturer Code sholud be less than or equal 10 Character.";
            }
            else if ((dtManufacturer.Rows.Count > 0) && this.pAddMode == true)
            {
                error_message = "Manufacturer already exist.";
            }
            else if (stateCode == "")
            {
                error_message = "State not found in default list.";
            }
            else if (countryid == 0)
            {
                error_message = "Country not found in default list.";
            }
            else if (!isNumeric && zip != "")
            {
                error_message = "Zip sholud be numeric.";
            }
            else if (zip.Length != 6 && zip != "")
            {
                error_message = "Enter valid Zip.";
            }
            else if (zip.Length != 6 && zip != "")
            {
                error_message = "Enter valid Zip.";
            }
            else if (!isNumericoffice && office != "")
            {
                error_message = "Office Tel sholud be numeric.";
            }
            else if (!isNumericphoner && phoner != "")
            {
                error_message = "Resi Tel sholud be numeric.";
            }
            else if (!isNumericcellno && cellno != "")
            {
                error_message = "Cell No sholud be numeric.";
            }
            else if (!reg.IsMatch(email) && email != "")
            {
                error_message = "Email id is not Proper.";
            }
            else if (dtMarketed.Rows.Count == 0 && marketedby != "")
            {
                error_message = "Marketed by not found in default list.";
            }
            else if (prDrugExpDate == Date)
            {
                error_message = "Drug Lic.Exp. Date invalid or blank.";
            }

            else if (prFoodExpDate == Date)
            {
                error_message = "FSSAI Exp. Date invalid or blank.";
            }

            return error_message;
        }
    }
}
