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

namespace udBatchMast
{
    public partial class Form_upload : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        string SqlStr;
        DataTable dtBatch;
        DataTable dtExcel = new DataTable();
        DataTable dtTemp = new DataTable();
        CheckBox checkboxHeader;
        int count = 0;
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
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DataTable dtRecord = new DataTable();
            dtRecord.Columns.Add("ItemName");
            dtRecord.Columns.Add("BatchNumber");
            dtRecord.Columns.Add("MRPRate");
            dtRecord.Columns.Add("ManufactureDate");
            dtRecord.Columns.Add("ExpiryDate");

            //Add Rupesh G on 05122018---start
            dtRecord.Columns.Add("PurchaseRate");
            dtRecord.Columns.Add("SalesRate");
            dtRecord.Columns.Add("PTR");
            dtRecord.Columns.Add("PTS");
            dtRecord.Columns.Add("StoreName");
            dtRecord.Columns.Add("Quantity");
            //Add Rupesh G on 05122018---End

            dtRecord.Columns.Add("IncludeGST(Y/N)");
            dtRecord.Columns.Add("BatchOnHold(Y/N)");
            dtRecord.Columns.Add("MRPIncludeGST(Y/N)");
            if (dtRecord != null)
            {
                string file_path = getFileName("\\BatchMaster_Format_");
                if (file_path != "")
                {
                    progressBar1.Value = 20;
                    ExportToExcel(dtRecord, file_path);
                    MessageBox.Show("Default format download successfully at " + file_path + ".xls", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }

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

                //MessageBox.Show("Your excel file exported successfully at " + excelFilename + ".xls", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
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
                string itname, batchcode,sname;
                int itcode;
               
                itname = row1["ItemName"].ToString().Trim().Replace("'", "''");
                batchcode = row1["BatchNumber"].ToString().Trim().Replace("'", "''");
                sname = row1["StoreName"].ToString().Trim().Replace("'", "''");

                SqlStr = "select It_code from IT_mast where lower(it_name)='" + itname.ToLower().ToString() + "' ";
                DataTable dtItcode = new DataTable();
                dtItcode = oDataAccess.GetDataTable(SqlStr, null, 20);
                if (dtItcode.Rows.Count > 0)
                {
                    itcode = Int32.Parse(dtItcode.Rows[0][0].ToString());
                }
                else
                {
                    itcode = 0;
                }

                string d = row1["Status"].ToString();

                if (row1["Status"].ToString() != "Deleted" && row1["Status"].ToString() != "Inserted" && row1["Status"].ToString() != "Not Inserted")
                {
                    SqlStr = "select * from pRetBatchDetails where It_code=" + itcode + " and BatchNo='" + batchcode + "' and storenm='"+sname+"'";
                    dtBatch = new DataTable();
                    dtBatch = oDataAccess.GetDataTable(SqlStr, null, 20);

                    if (dtBatch.Rows.Count > 0)
                    {
                        row1["Status"] = "Exists";

                    }
                    else if (dtBatch.Rows.Count == 0)
                    {
                        row1["Status"] = "New Record";



                    }


                }
            }
            foreach (DataRow row1 in dtExcel.Rows)
            {
                string itname, batchcode;
                itname = row1["ItemName"].ToString().Trim().Replace("'", "''");
                batchcode = row1["BatchNumber"].ToString().Trim().Replace("'", "''");
                DataRow[] foundRec = dtExcel.Select("ItemName = '" + itname.ToString() + "' and BatchNumber='" + batchcode + "' and Status <> 'Deleted' and  Status <> 'Exists'");
                if (foundRec.Length > 1)
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
            dataGridView1.Columns[1].DataPropertyName = "ItemName";
            dataGridView1.Columns[2].DataPropertyName = "BatchNumber";
            dataGridView1.Columns[3].DataPropertyName = "MRPRate";
            dataGridView1.Columns[4].DataPropertyName = "ManufactureDate";
            dataGridView1.Columns[5].DataPropertyName = "ExpiryDate";

            //Add Rupesh G. on 05122018--Start
            dataGridView1.Columns[6].DataPropertyName = "PurchaseRate";
            dataGridView1.Columns[7].DataPropertyName = "SalesRate";
            dataGridView1.Columns[8].DataPropertyName = "PTR";
            dataGridView1.Columns[9].DataPropertyName = "PTS";
            dataGridView1.Columns[10].DataPropertyName = "StoreName";
            dataGridView1.Columns[11].DataPropertyName = "Quantity";
            //Add Rupesh G. on 05122018--End

            dataGridView1.Columns[12].DataPropertyName = "IncludeGST(Y/N)";
            dataGridView1.Columns[13].DataPropertyName = "BatchOnHold(Y/N)";
            dataGridView1.Columns[14].DataPropertyName = "MRPIncludeGST(Y/N)";
            dataGridView1.Columns[15].DataPropertyName = "Status";
            dataGridView1.Columns[16].DataPropertyName = "Error";
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

            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {

                var chk = row1.Cells["chk"].Value.ToString().Trim();
                if ((/*remark != "Exists" && remark != "Duplicate" && */chk == "False"))
                {

                    row1.Cells["chk"].Value = true;

                }
                else if ((/*remark != "Exists" && remark != "Duplicate" &&*/ chk == "True"))
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
                    string itname, batchcode,sname;
                    int itcode;

                    itname = row1["ItemName"].ToString().Trim().Replace("'", "''");
                    batchcode = row1["BatchNumber"].ToString();
                    sname= row1["StoreName"].ToString().Trim().Replace("'", "''");

                    SqlStr = "select It_code from IT_mast where lower(it_name)='" + itname.ToLower().ToString().Trim() + "' ";
                    DataTable dtItcode = new DataTable();
                    dtItcode = oDataAccess.GetDataTable(SqlStr, null, 20);
                    itcode = Int32.Parse(dtItcode.Rows[0][0].ToString());

                    if (row1["Status"].ToString() != "Deleted" && row1["Status"].ToString() != "Inserted" && row1["Status"].ToString() != "Not Inserted")
                    {


                        SqlStr = "select * from pRetBatchDetails where It_code=" + itcode + " and BatchNo='" + batchcode + "' and storenm='"+sname+"'";
                        dtBatch = new DataTable();
                        dtBatch = oDataAccess.GetDataTable(SqlStr, null, 20);

                        if (dtBatch.Rows.Count > 0)
                        {
                            row1["Status"] = "Exists";

                        }
                        else if (dtBatch.Rows.Count == 0)
                        {
                            row1["Status"] = "New Record";



                        }


                    }
                }
                foreach (DataRow row1 in dtTemp.Rows)
                {
                    string itname, batchcode;
                    itname = row1["ItemName"].ToString();
                    batchcode = row1["BatchNumber"].ToString();
                    DataRow[] foundRec = dtTemp.Select("ItemName = '" + itname + "' and BatchNumber='" + batchcode + "' and Status <> 'Deleted' and  Status <> 'Exists'");
                    if (foundRec.Length > 1)
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (dtExcel.Rows.Count > 0)
            {
                string file_path = getFileName("\\BatchMaster_Log_");
                if (file_path != "")
                {
                    progressBar1.Value = 20;
                    DataView view = new DataView(dtExcel);
                    DataTable dtLogRecord = view.ToTable(false, "ItemName", "BatchNumber", "MRPRate", "ManufactureDate", "ExpiryDate", "PurchaseRate", "SalesRate", "PTR", "PTS", "StoreName", "Quantity", "IncludeGST(Y/N)", "BatchOnHold(Y/N)","MRPIncludeGST(Y/N)", "Status", "Error");
                    ExportToExcel(dtLogRecord, file_path);
                    MessageBox.Show("Log download successfully at " + file_path + ".xls", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("No Record found..!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string validate(string itemName, string It_code, string BatchNo, string BatchRate, string Inc_gst, string BatchOnHold, DateTime Mfgdt, DateTime Expdt, string prate, string srate, string ptr, string pts, string storenm, string Qty,string MRPIncludeGST)
        {
            error_message = "";
            DateTime Date = DateTime.MinValue;

            double n;
            bool isNumeric = double.TryParse(BatchRate, out n);

            DataTable dtstorenm = new DataTable();
            SqlStr = "select Ware_nm from warehouse where IsStoreWare='s' and Ware_nm='"+ storenm + "'";

            dtstorenm = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (itemName == "")
            {
                error_message = "Item Name Can not be blank.";
            }
            else if (It_code == "0")
            {

                error_message = "Item Name not found in Supply master.";
            }
            else if (BatchNo == "")
            {
                error_message = "Batch Number Can not be blank.";
            }
            else if (!isNumeric)
            {
                error_message = "MRP Rate sholud be numeric.";
            }
           

            else if (!double.TryParse(prate, out n))
            {
                error_message = "Purchase Rate sholud be numeric.";
            }
            else if (!double.TryParse(srate, out n))
            {
                error_message = "Sales Rate sholud be numeric.";
            }
            else if (!double.TryParse(ptr, out n))
            {
                error_message = "PTR sholud be numeric.";
            }
            else if (!double.TryParse(pts, out n))
            {
                error_message = "PTS sholud be numeric.";
            }
            else if (!double.TryParse(Qty, out n))
            {
                error_message = "Quantity sholud be numeric.";
            }
            else if (dtstorenm.Rows.Count==0)
            {
                error_message = "Store Name not found in Store Room master.";
            }

            else if (Mfgdt == Date)
            {
                error_message = "Manufacture Date invalid or blank.";
            }
            else if (Expdt == Date)
            {
                error_message = "Expiry Date invalid or blank.";
            }
            else if (Inc_gst.ToString().ToLower() != "y" && Inc_gst.ToString().ToLower() != "n")
            {
                error_message = "Include GST value does not match with Y/N.";
            }
            else if (BatchOnHold.ToString().ToLower() != "y" && BatchOnHold.ToString().ToLower() != "n")
            {
                error_message = "Batch On Hold value does not match with Y/N.";
            }
            else if (MRPIncludeGST.ToString().ToLower() != "y" && MRPIncludeGST.ToString().ToLower() != "n")
            {
                error_message = "MRP Include GST value does not match with Y/N.";
            }

            return error_message;

        }
        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (dtTemp.Rows.Count > 0)
            {
                progressBar1.Visible = true;
                progressBar1.Value = 10;
                foreach (DataRow dr in dtTemp.Rows)
                {
                    error_message = "";
                    progressBar1.Value = 30;
                    if (dr["Status"].ToString() == "New Record" && dr["select"].ToString() == "True")
                    {
                        count++;
                        int It_code, Inc_gst = 0, BatchOnHold = 0, MRPINC_gst=0;
                        string BatchNo,storenm;
                        DateTime Mfgdt = DateTime.MinValue, Expdt = DateTime.MinValue;
                        double BatchRate = 0;
                        double prate = 0, srate = 0, ptr = 0, pts = 0,  Qty = 0;


                        try
                        {
                            oDataAccess.BeginTransaction();

                            SqlStr = "select It_code from IT_mast where lower(it_name)='" + dr["ItemName"].ToString().ToLower().Trim().Replace("'", "''") + "' ";
                            DataTable dtItcode = new DataTable();
                            dtItcode = oDataAccess.GetDataTable(SqlStr, null, 20);
                            if (dtItcode.Rows.Count > 0)
                            {
                                It_code = Int32.Parse(dtItcode.Rows[0][0].ToString());
                            }
                            else
                            {
                                It_code = 0;
                            }

                            BatchNo = dr["BatchNumber"].ToString().Trim().Replace("'", "''");
                          
                            if (dr["MRPRate"].ToString() == "")
                            {
                                BatchRate = 0;
                            }
                            else
                            {
                                try
                                {
                                    BatchRate = double.Parse(dr["MRPRate"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            if (dr["IncludeGST(Y/N)"].ToString().Trim().Replace("'", "''") == "y")
                            {
                                Inc_gst = 1;
                            }
                            else if (dr["IncludeGST(Y/N)"].ToString().Trim().Replace("'", "''") == "n")
                            {
                                Inc_gst = 0;
                            }

                            if (dr["BatchOnHold(Y/N)"].ToString().Trim().Replace("'", "''") == "y")
                            {
                                BatchOnHold = 1;
                            }
                            else if (dr["BatchOnHold(Y/N)"].ToString().Trim().Replace("'", "''") == "n")
                            {
                                BatchOnHold = 0;
                            }
                            if (dr["MRPIncludeGST(Y/N)"].ToString().Trim().Replace("'", "''") == "y")
                            {
                                MRPINC_gst = 1;
                            }
                            else if (dr["MRPIncludeGST(Y/N)"].ToString().Trim().Replace("'", "''") == "n")
                            {
                                MRPINC_gst = 0;
                            }
                            try
                            {
                                Mfgdt = DateTime.Parse(dr["ManufactureDate"].ToString().Trim());
                                Expdt = DateTime.Parse(dr["ExpiryDate"].ToString().Trim());

                            }
                            catch (Exception ee)
                            {

                            }

                            //Add Rupesh G. on 05122018--Start
                            if (dr["PurchaseRate"].ToString() == "")
                            {
                                prate = 0;
                            }
                            else
                            {
                                try
                                {
                                    prate = double.Parse(dr["PurchaseRate"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            if (dr["SalesRate"].ToString() == "")
                            {
                                srate = 0;
                            }
                            else
                            {
                                try
                                {
                                    srate = double.Parse(dr["SalesRate"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            if (dr["PTR"].ToString() == "")
                            {
                                ptr = 0;
                            }
                            else
                            {
                                try
                                {
                                    ptr = double.Parse(dr["PTR"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            if (dr["PTS"].ToString() == "")
                            {
                                pts = 0;
                            }
                            else
                            {
                                try
                                {
                                    pts = double.Parse(dr["PTS"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }
                            storenm = dr["StoreName"].ToString().Trim().Replace("'", "''");

                            if (dr["Quantity"].ToString() == "")
                            {
                                Qty = 0;
                            }
                            else
                            {
                                try
                                {
                                    Qty = double.Parse(dr["Quantity"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            //Add Rupesh G. on 05122018--End

                            error_message = validate(dr["ItemName"].ToString().ToLower().Trim().Replace("'", "''"), It_code.ToString(), BatchNo, dr["BatchRate"].ToString(), dr["IncludeGST(Y/N)"].ToString(), dr["BatchOnHold(Y/N)"].ToString(), Mfgdt, Expdt, dr["PurchaseRate"].ToString(), dr["SalesRate"].ToString(), dr["PTR"].ToString(), dr["PTS"].ToString(), storenm, dr["Quantity"].ToString(), dr["MRPIncludeGST(Y/N)"].ToString().Trim());

                            if (It_code != 0 && error_message == "")
                            {

                                oDataAccess.BeginTransaction();
                                SqlStr = "set dateformat dmy INSERT INTO pRetBatchDetails(It_code,BatchNo,BatchRate,Inc_gst,BatchOnHold,Mfgdt,Expdt,prate,srate,ptr,pts,storenm,Qty,MRPINC_gst)";
                                SqlStr = SqlStr + " VALUES(" + It_code + ", '" + BatchNo + "', " + BatchRate + " ";
                                SqlStr = SqlStr + ", " + Inc_gst + ", " + BatchOnHold + ", '" + Mfgdt + "', '" + Expdt + "',"+ prate + ","+ srate + ","+ ptr + ","+pts+",'"+ storenm + "',"+Qty+","+ MRPINC_gst + ") ";
                                oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                                oDataAccess.CommitTransaction();

                                dr["Status"] = "Inserted";
                            }
                            else
                            {

                                dr["Error"] = error_message;



                                dr["Status"] = "Not Inserted1";
                            }
                        }
                        catch (Exception ee)
                        {
                            dr["Error"] = error_message;
                            dr["Status"] = "Not Inserted ";
                          //  oDataAccess.CommitTransaction();
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
                        count++;
                        int It_code, Inc_gst = 0, BatchOnHold = 0, MRPINC_gst=0;
                        string BatchNo, storenm;
                        DateTime Mfgdt = DateTime.MinValue, Expdt = DateTime.MinValue;
                        double BatchRate = 0;
                        double prate = 0, srate = 0, ptr = 0, pts = 0, Qty = 0;

                        try
                        {
                            oDataAccess.BeginTransaction();

                            SqlStr = "select It_code from IT_mast where lower(it_name)='" + dr["ItemName"].ToString().ToLower().Trim().Replace("'", "''") + "' ";
                            DataTable dtItcode = new DataTable();
                            dtItcode = oDataAccess.GetDataTable(SqlStr, null, 20);
                            oDataAccess.CommitTransaction();
                            if (dtItcode.Rows.Count > 0)
                            {
                                It_code = Int32.Parse(dtItcode.Rows[0][0].ToString());
                            }
                            else
                            {
                                It_code = 0;
                            }


                            BatchNo = dr["BatchNumber"].ToString().Trim().Replace("'", "''");

                            if (dr["MRPRate"].ToString() == "")
                            {
                                BatchRate = 0;
                            }
                            else
                            {
                                try
                                {
                                    BatchRate = double.Parse(dr["MRPRate"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }


                            if (dr["IncludeGST(Y/N)"].ToString().ToLower().Trim().Replace("'", "''") == "y")
                            {
                                Inc_gst = 1;
                            }
                            else if (dr["IncludeGST(Y/N)"].ToString().ToLower().Trim().Replace("'", "''") == "n")
                            {
                                Inc_gst = 0;
                            }

                            if (dr["BatchOnHold(Y/N)"].ToString().Trim().ToLower().Replace("'", "''") == "y")
                            {
                                BatchOnHold = 1;
                            }
                            else if (dr["BatchOnHold(Y/N)"].ToString().ToLower().Trim().Replace("'", "''") == "n")
                            {
                                BatchOnHold = 0;
                            }
                            if (dr["MRPIncludeGST(Y/N)"].ToString().Trim().Replace("'", "''") == "y")
                            {
                                MRPINC_gst = 1;
                            }
                            else if (dr["MRPIncludeGST(Y/N)"].ToString().Trim().Replace("'", "''") == "n")
                            {
                                MRPINC_gst = 0;
                            }
                            try
                            {
                                Mfgdt = DateTime.Parse(dr["ManufactureDate"].ToString().Trim());
                                Expdt = DateTime.Parse(dr["ExpiryDate"].ToString().Trim());
                            }
                            catch (Exception ee)
                            {

                            }

                            //Add Rupesh G. on 05122018--Start
                            if (dr["PurchaseRate"].ToString() == "")
                            {
                                prate = 0;
                            }
                            else
                            {
                                try
                                {
                                    prate = double.Parse(dr["PurchaseRate"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            if (dr["SalesRate"].ToString() == "")
                            {
                                srate = 0;
                            }
                            else
                            {
                                try
                                {
                                    srate = double.Parse(dr["SalesRate"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            if (dr["PTR"].ToString() == "")
                            {
                                ptr = 0;
                            }
                            else
                            {
                                try
                                {
                                    ptr = double.Parse(dr["PTR"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            if (dr["PTS"].ToString() == "")
                            {
                                pts = 0;
                            }
                            else
                            {
                                try
                                {
                                    pts = double.Parse(dr["PTS"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }
                            storenm = dr["StoreName"].ToString().Trim().Replace("'", "''");

                            if (dr["Quantity"].ToString() == "")
                            {
                                Qty = 0;
                            }
                            else
                            {
                                try
                                {
                                    Qty = double.Parse(dr["Quantity"].ToString());
                                }
                                catch (Exception ee)
                                {

                                }
                            }

                            //Add Rupesh G. on 05122018--End

                            //error_message = validate(dr["ItemName"].ToString().ToLower().Trim().Replace("'", "''"), It_code.ToString(), BatchNo, dr["BatchRate"].ToString(), dr["IncludeGST(Y/N)"].ToString(), dr["BatchOnHold(Y/N)"].ToString(), Mfgdt, Expdt, prate.ToString(), srate.ToString(), ptr.ToString(), pts.ToString(), storenm, Qty.ToString());
                            error_message = validate(dr["ItemName"].ToString().ToLower().Trim().Replace("'", "''"), It_code.ToString(), BatchNo, dr["MRPRate"].ToString(), dr["IncludeGST(Y/N)"].ToString(), dr["BatchOnHold(Y/N)"].ToString(), Mfgdt, Expdt, dr["PurchaseRate"].ToString(), dr["SalesRate"].ToString(), dr["PTR"].ToString(), dr["PTS"].ToString(), storenm, dr["Quantity"].ToString(), dr["MRPIncludeGST(Y/N)"].ToString().Trim());
                            //if (It_code == "" || Schedule == "H1" || Schedule == "Narcotic" || Schedule == "")
                            //{
                            if (It_code != 0 && error_message == "")
                            {
                                oDataAccess.BeginTransaction();
                                SqlStr = "set dateformat dmy INSERT INTO pRetBatchDetails(It_code,BatchNo,BatchRate,Inc_gst,BatchOnHold,Mfgdt,Expdt,prate,srate,ptr,pts,storenm,Qty,MRPINC_gst)";
                                SqlStr = SqlStr + " VALUES(" + It_code + ", '" + BatchNo + "', " + BatchRate + " ";
                                SqlStr = SqlStr + ", " + Inc_gst + ", " + BatchOnHold + ", '" + Mfgdt + "', '" + Expdt + "'," + prate + "," + srate + "," + ptr + "," + pts + ",'" + storenm + "'," + Qty + ","+ MRPINC_gst + ") ";
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
                            dr["Status"] = "Not Inserted ";
                          
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
            if (count == 0)
            {
                MessageBox.Show("Select atleast one record...! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (dtExcel.Rows.Count > 0)
            {
                ddlRemark.Enabled = true;
            }
            count = 0;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (this.btnLogout.Enabled)
                btnLogout_Click(this.btnLogout, e);
        }
    }
}
