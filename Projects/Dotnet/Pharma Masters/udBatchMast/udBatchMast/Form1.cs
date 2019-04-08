using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udBatchMast
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        string SqlStr, BatchID;
        int itcode, Inc_gst, BatchOnHold, MRPIncGST;
        DataTable dtBatchDet, dtBatchGrid;
        String cAppPId, cAppName;
        int It_code, count = 0;
        string BatchNo, storenm;
        DateTime Mfgdt, Expdt;
        double BatchRate, prate, srate, ptr, pts, Qty;
        CheckBox checkboxHeader;

        public Form1()
        {
            InitializeComponent();
        }
        public Form1(string[] args)
        {
            this.pDisableCloseBtn = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            InitializeComponent();
            this.pPApplPID = 0;
            this.pPara = args;
            this.pFrmCaption = "Batch Master";
            this.pCompId = Convert.ToInt16(args[0]);
            this.pComDbnm = args[1];
            this.pServerName = args[2];
            this.pUserId = args[3];
            this.pPassword = args[4];
            this.pPApplRange = args[5];
            this.pAppUerName = args[6];
            Icon MainIcon = new System.Drawing.Icon(args[7].Replace("<*#*>", " "));
            this.pFrmIcon = MainIcon;
            this.pPApplText = args[8].Replace("<*#*>", " ");
            this.pPApplName = args[9];
            this.pPApplPID = Convert.ToInt16(args[10]);
            this.pPApplCode = args[11];

            DataAccess_Net.clsDataAccess._databaseName = this.pComDbnm;
            DataAccess_Net.clsDataAccess._serverName = this.pServerName;
            DataAccess_Net.clsDataAccess._userID = this.pUserId;
            DataAccess_Net.clsDataAccess._password = this.pPassword;
            oDataAccess = new DataAccess_Net.clsDataAccess();

            CultureInfo ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getBatchCode();
            bindControls(0);
            bindGrid(0);
            setEnableDisable(false);
            setNavigation();
            this.mInsertProcessIdRecord();
            lblProgress.Visible = false;
            progressBar1.Visible = false;

        }

        private void mInsertProcessIdRecord()
        {
            DataSet dsData = new DataSet();
            string sqlstr;
            int pi;
            pi = Process.GetCurrentProcess().Id;
            cAppName = "udBatchMast.exe";
            cAppPId = Convert.ToString(Process.GetCurrentProcess().Id);
            sqlstr = "Set DateFormat dmy insert into vudyog..ExtApplLog (pApplCode,CallDate,pApplNm,pApplId,pApplDesc,cApplNm,cApplId,cApplDesc) Values('" + this.pPApplCode + "','" + DateTime.Now.Date.ToString() + "','" + this.pPApplName + "'," + this.pPApplPID + ",'" + this.pPApplText + "','" + cAppName + "'," + cAppPId + ",'" + this.Text.Trim() + "')";
            oDataAccess.ExecuteSQLStatement(sqlstr, null, 20, true);
        }

        private void mDeleteProcessIdRecord()
        {
            if (string.IsNullOrEmpty(this.pPApplName) || this.pPApplPID == 0 || string.IsNullOrEmpty(this.cAppName) || string.IsNullOrEmpty(this.cAppPId))
            {
                return;
            }
            DataSet dsData = new DataSet();
            string sqlstr;
            sqlstr = " Delete from vudyog..ExtApplLog where pApplNm='" + this.pPApplName + "' and pApplId=" + this.pPApplPID + " and cApplNm= '" + cAppName + "' and cApplId= " + cAppPId;
            oDataAccess.ExecuteSQLStatement(sqlstr, null, 20, true);
        }


        public void getBatchCode()
        {
            SqlStr = "select top 1 id from pRetBatchDetails order by id desc";
            DataTable dtBatchCode = new DataTable();
            dtBatchCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtBatchCode.Rows.Count > 0)
            {
                BatchID = dtBatchCode.Rows[0][0].ToString();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            count = 0;
            this.pAddMode = true;
            setEnableDisable(true);
            setNavigation();
            //    dataGridView1.Enabled = false;
        }

        private void bindControls(int val)
        {
            if (val == 0)
            {
                SqlStr = "select * from pRetBatchDetails where id='" + BatchID + "'";
            }
            else if (val == 1)
            {
                SqlStr = "select * from pRetBatchDetails where it_code=" + itcode + " and batchno='" + txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "'";
            }
            else if (val == 2)
            {
                SqlStr = "select top 1 * from pRetBatchDetails where batchno='" + txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' order by id desc";
            }

            dtBatchDet = new DataTable();
            dtBatchDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtBatchDet.Rows.Count > 0)
            {
                itcode = Int32.Parse(dtBatchDet.Rows[0]["it_code"].ToString());
                bool GST, Hold, MRPIncGST;

                SqlStr = "select it_name from IT_MAST where It_code=" + itcode;
                DataTable dtItemName = new DataTable();
                dtItemName = oDataAccess.GetDataTable(SqlStr, null, 20);

                txtItemName.Text = dtItemName.Rows[0]["it_name"].ToString().Trim();
                txtBatchNumber.Text = dtBatchDet.Rows[0]["BatchNo"].ToString();
                txtBatchRate.Text = dtBatchDet.Rows[0]["BatchRate"].ToString();
                dtMfg.Text = dtBatchDet.Rows[0]["Mfgdt"].ToString();
                dtExp.Text = dtBatchDet.Rows[0]["Expdt"].ToString();

                //Add Rupesh G. on 05122018 --start
                txtPrate.Text = dtBatchDet.Rows[0]["prate"].ToString();
                txtSrate.Text = dtBatchDet.Rows[0]["srate"].ToString();
                txtPtr.Text = dtBatchDet.Rows[0]["ptr"].ToString();
                txtPts.Text = dtBatchDet.Rows[0]["pts"].ToString();
                txtStoreName.Text = dtBatchDet.Rows[0]["storenm"].ToString();
                txtQty.Text = dtBatchDet.Rows[0]["Qty"].ToString();
                //Add Rupesh G. on 05122018 --end

                GST = bool.Parse(dtBatchDet.Rows[0]["Inc_gst"].ToString());
                Hold = bool.Parse(dtBatchDet.Rows[0]["BatchOnHold"].ToString());
                MRPIncGST = bool.Parse(dtBatchDet.Rows[0]["MRPINC_gst"].ToString());

                if (GST)
                {
                    chkGST.Checked = true;
                }
                else
                {
                    chkGST.Checked = false;
                }

                if (Hold)
                {
                    chkHold.Checked = true;
                }
                else
                {
                    chkHold.Checked = false;
                }

                if (MRPIncGST)
                {
                    chkMRPInc.Checked = true;
                }
                else
                {
                    chkMRPInc.Checked = false;
                }

            }
            else
            {
                txtItemName.Text = "";
                txtBatchNumber.Text = "";
                txtBatchRate.Text = "";
                dtMfg.Text = DateTime.Now.ToString("dd-MM-yyyy");
                dtExp.Text = DateTime.Now.ToString("dd-MM-yyyy");
                chkGST.Checked = false;
                chkHold.Checked = false;
                chkMRPInc.Checked = false;

                //Add Rupesh G. on 05122018 --start
                txtPrate.Text = "";
                txtSrate.Text = "";
                txtPtr.Text = "";
                txtPts.Text = "";
                txtStoreName.Text = "";
                txtQty.Text = "";
                //Add Rupesh G. on 05122018 --End

            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            count = 0;
            this.pEditMode = true;
            this.pAddMode = false;
            setEnableDisable(true);
            setNavigation();
            // dataGridView1.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            count = 1;

            this.pAddMode = false;
            this.pEditMode = false;
            getBatchCode();
            bindControls(0);
            bindGrid(0);
            setEnableDisable(false);
            setNavigation();
            //  dataGridView1.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //**** Added by Sachin N. S. on 21/11/2018 for Bug-31943 --Start
            //Comment by Rupesh G. on 07122018--Start
            //string csql = "";
            //csql = "Execute Usp_Ent_Check_Data_Used 'PBatchM','BATCHNO','" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "'";
            //Comment by Rupesh G. on 07122018--End

            //Add by Rupesh G. on 07122018--Start
            string csql = "";
            csql = "select BatchNo,It_code,ware_nm,'Pharma Excess Stock' as'tran_name' from ESITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            csql = csql + "union all select distinct BatchNo,It_code,ware_nm,'Pharma Opening Stock' as'tran_name' from OSITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            csql = csql + "union all select distinct BatchNo,It_code,ware_nm,'Pharma POINT OF SALES' as'tran_name' from DCITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            csql = csql + "union all select distinct BatchNo,It_code,ware_nm,'Pharma Purchase' as'tran_name' from PTITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            csql = csql + "union all select distinct BatchNo,It_code,ware_nm,'Pharma Purchase Return' as'tran_name' from PRITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            csql = csql + "union all select distinct BatchNo,It_code,ware_nm,'Pharma Sales' as'tran_name' from STITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            csql = csql + "union all select distinct BatchNo,It_code,ware_nm,'Pharma Sales Return' as'tran_name' from SRITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            csql = csql + "union all select distinct BatchNo,It_code,ware_nm,'Pharma Shortage Stock' as'tran_name' from SSITEM where BatchNo='" + this.txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and It_code='" + itcode + "' and ware_nm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";

            //Add by Rupesh G. on 07122018--End

            DataTable _dt = oDataAccess.GetDataTable(csql, null, 25);
            if (_dt != null)
            {
                if (_dt.Rows.Count > 0)
                {
                    //Add by Rupesh G. on 07122018--Start
                    string msg = "Batch is used in \n";
                    foreach (DataRow dr in _dt.Rows)
                    {
                        msg = msg + dr["tran_name"].ToString() + "\n";
                    }
                    msg = msg + "Can not delete!!!";
                    MessageBox.Show(msg, this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                    //Add by Rupesh G. on 07122018--End

                    //Comment by Rupesh G. on 07122018--Start
                    //if (_dt.Rows[0]["msg"].ToString() != "")
                    //{
                    //    MessageBox.Show(_dt.Rows[0]["msg"].ToString(), this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //    return;
                    //}
                    //Comment by Rupesh G. on 07122018--End
                }
            }
            //**** Added by Sachin N. S. on 21/11/2018 for Bug-31943 --End
            this.dataGridView1.Controls.Remove(checkboxHeader);

            SqlStr = "select It_Code from IT_MAST where it_name='" + txtItemName.Text.ToString().Trim().Replace("'", "''") + "'";
            DataTable dtCode = new DataTable();
            dtCode = oDataAccess.GetDataTable(SqlStr, null, 20);

            int itcode1 = Int32.Parse(dtCode.Rows[0][0].ToString());


            if (MessageBox.Show("Are you sure you wish to delete this Record ?", this.pPApplText,
             MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "Delete from pRetBatchDetails where it_code='" + itcode1 + "' and BatchNo='" + txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "' and storenm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "' ";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                catch (Exception ee)
                {


                }

                this.pAddMode = false;
                this.pEditMode = false;
                getBatchCode();
                bindControls(0);
                bindGrid(0);
                setEnableDisable(false);
                setNavigation();

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            count = 0;
            // dataGridView1.Enabled = true;

            string batchNo = txtBatchNumber.Text.ToString().Trim().Trim().Replace("'", "''");
            SqlStr = "select * from pRetBatchDetails where It_code=" + itcode + " and BatchNo='" + batchNo + "' and  storenm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "'";
            DataTable dtBatch = new DataTable();
            dtBatch = oDataAccess.GetDataTable(SqlStr, null, 20);

            double n1;


            if (txtItemName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Item Name cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                btnName.Focus();
            }
            else if (txtBatchNumber.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Batch Number cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtBatchNumber.Focus();
            }
            else if ((dtBatch.Rows.Count > 0) && this.pAddMode == true)
            {
                MessageBox.Show("Batch already exist.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtBatchNumber.Focus();
            }
            else if (!double.TryParse(txtBatchRate.Text, out n1) && txtBatchRate.Text != "")
            {
                MessageBox.Show("MRP Rate sholud be numeric.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtBatchRate.Text = "";
                txtBatchRate.Focus();

            }
            else if (txtStoreName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Store Name cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                btnStoreName.Focus();
            }
            else if (!double.TryParse(txtPrate.Text, out n1) && txtPrate.Text != "")
            {
                MessageBox.Show("Purchase Rate sholud be numeric.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtPrate.Text = "";
                txtPrate.Focus();
            }

            else if (!double.TryParse(txtSrate.Text, out n1) && txtSrate.Text != "")
            {
                MessageBox.Show("Sales Rate sholud be numeric.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtSrate.Text = "";
                txtSrate.Focus();
            }

            else if (!double.TryParse(txtPtr.Text, out n1) && txtPtr.Text != "")
            {
                MessageBox.Show("PTR sholud be numeric.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtPtr.Text = "";
                txtPtr.Focus();
            }
            else if (!double.TryParse(txtPts.Text, out n1) && txtPts.Text != "")
            {
                MessageBox.Show("PTS sholud be numeric.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtPts.Text = "";
                txtPts.Focus();
            }
            else if (!double.TryParse(txtQty.Text, out n1) && txtQty.Text != "")
            {
                MessageBox.Show("Quantity sholud be numeric.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtQty.Text = "";
                txtQty.Focus();
            }
            else
            {
                setval();
                InsertUpdate();

                this.pAddMode = false;
                this.pEditMode = false;


                bindGrid(0);
                setEnableDisable(false);
                setNavigation();
                //  dataGridView1.Enabled = true;

            }


        }

        private void btnName_Click(object sender, EventArgs e)
        {

            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();

            if (pAddMode == true || pEditMode == true)
            {
                SqlStr = " select It_Code, it_name from IT_MAST";
            }
            else
            {
                SqlStr = "select distinct b.It_Code,Ltrim(Rtrim(I.it_name)) as it_name  from pRetBatchDetails b inner join IT_MAST i  ON b.It_code = i.It_code";
                SqlStr = SqlStr + " union  select It_Code='',it_name='' from pRetBatchDetails";//Add Rupesh G on 02012019
            }

            //SqlStr = "select distinct b.It_Code,Ltrim(Rtrim(I.it_name)) as it_name  from pRetBatchDetails b inner join IT_MAST i  ON b.It_code = i.It_code";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Item Name ";
            vSearchCol = "it_name";
            vDisplayColumnList = "it_name:Item Name";
            vReturnCol = "it_name,It_Code";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.Icon = pFrmIcon;
            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();

            if (oSelectPop.pReturnArray != null)
            {
                txtItemName.Text = oSelectPop.pReturnArray[0];
                //Add by rupesh g 02012019---Start
                if (txtItemName.Text.ToString().Trim() == "")
                {
                    BatchID = "0";
                }
                //Add by rupesh g 02012019---End
                itcode = Int32.Parse(oSelectPop.pReturnArray[1]);
                SqlStr = "select top 1 id from pRetBatchDetails where it_code=" + itcode + " order by id desc";
                DataTable dtBatchCode = new DataTable();
                dtBatchCode = oDataAccess.GetDataTable(SqlStr, null, 20);
                if (dtBatchCode.Rows.Count > 0)
                {
                    BatchID = dtBatchCode.Rows[0][0].ToString();
                }
            }

            setNavigation();
            if ((BatchID == "0") && (pAddMode == false && pEditMode == false))
            {
                bindControls(0);
                bindGrid(4);
            }
            else if (pAddMode == false && pEditMode == false)
            {
                bindControls(0);
                bindGrid(1);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();

            //Comment by Rupesh G. on 02012018 
            //SqlStr = "select distinct batchno from pRetBatchDetails where it_code=" + itcode;

            //Add by Rupesh G. on 02012018---Start
            if (txtItemName.Text.ToString().Trim() == "" && (pAddMode == false && pEditMode == false))
            {
                SqlStr = "select distinct batchno from pRetBatchDetails ";

            }
            else
            {
                SqlStr = "select distinct batchno from pRetBatchDetails where it_code=" + itcode;
            }
            //Add by Rupesh G. on 02012018----End

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Batch Number ";
            vSearchCol = "batchno";
            vDisplayColumnList = "batchno:Batch Number";
            vReturnCol = "batchno";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.Icon = pFrmIcon;
            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();

            if (oSelectPop.pReturnArray != null)
            {
                txtBatchNumber.Text = oSelectPop.pReturnArray[0];

                // ManufacturerID = oSelectPop.pReturnArray[1];
            }

            //Comment By Rupesh G on 02012018----Start
            //setNavigation();
            //bindControls(1);
            //bindGrid(2);
            //Comment By Rupesh G on 02012018----End

            //Add By Rupesh G on 02012018----Start
            if (txtItemName.Text.ToString().Trim() == "" && (pAddMode == false && pEditMode == false))
            {
                setNavigation();
                bindControls(2);
                bindGrid(3);
            }
            else
            {
                setNavigation();
                bindControls(1);
                bindGrid(2);
            }

            //Add By Rupesh G on 02012018----End

        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dtBatchGrid.Rows.Count > 0)
            {
                DataRow[] result = dtBatchGrid.Select("Sel=1");
                if (result.Length != 0)
                {
                    string file_path = getFileName("\\BatchMaster_");

                    if (file_path != "")
                    {
                        progressBar1.Value = 20;

                        DataTable dt = result.CopyToDataTable();
                        DataTable dt1 = new DataView(dt).ToTable(false, "it_name", "BatchNo", "BatchRate", "Mfgdt", "Expdt", "prate", "srate", "ptr", "pts", "storenm", "Qty", "Inc_gst", "BatchOnHold", "MRPInc_gst");
                        //DataView view = new DataView(dtBatchGrid);
                        //DataTable dt = view.ToTable(false, "it_name", "BatchNo", "BatchRate", "Mfgdt", "Expdt", "Inc_gst", "BatchOnHold");

                        dt1.Columns["it_name"].ColumnName = "Item Name";
                        dt1.Columns["BatchNo"].ColumnName = "Batch Number";
                        dt1.Columns["BatchRate"].ColumnName = "MRP Rate";
                        dt1.Columns["Mfgdt"].ColumnName = "Manufacture Date";
                        dt1.Columns["Expdt"].ColumnName = "Expiry Date";

                        //Add Rupesh G. on 05122018---Start
                        dt1.Columns["prate"].ColumnName = "Purchase Rate";
                        dt1.Columns["srate"].ColumnName = "Sales Rate";
                        dt1.Columns["ptr"].ColumnName = "PTR";
                        dt1.Columns["pts"].ColumnName = "PTS";
                        dt1.Columns["storenm"].ColumnName = "Store Name";
                        dt1.Columns["Qty"].ColumnName = "Quantity";
                        //Add Rupesh G. on 05122018---End

                        dt1.Columns["Inc_gst"].ColumnName = "Include GST";
                        dt1.Columns["BatchOnHold"].ColumnName = "Batch On Hold";
                        dt1.Columns["MRPInc_gst"].ColumnName = "MRP Include GST";
                        ExportToExcel(dt1, file_path);
                    }
                }
                else
                {
                    MessageBox.Show("Select atleast one record...! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No Record found..!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                MessageBox.Show("Your excel file exported successfully at " + excelFilename + ".xls", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtItemName.Text = row.Cells[1].Value.ToString();
                txtBatchNumber.Text = row.Cells[2].Value.ToString();
                txtBatchRate.Text = row.Cells[3].Value.ToString();
                dtMfg.Text = row.Cells[4].Value.ToString();
                dtExp.Text = row.Cells[5].Value.ToString();

                //Add Rupesh G. on 05122018 --start
                txtPrate.Text = row.Cells[6].Value.ToString();
                txtSrate.Text = row.Cells[7].Value.ToString();
                txtPtr.Text = row.Cells[8].Value.ToString();
                txtPts.Text = row.Cells[9].Value.ToString();
                txtStoreName.Text = row.Cells[10].Value.ToString();
                txtQty.Text = row.Cells[11].Value.ToString();
                //Add Rupesh G. on 05122018 --start

                bool includeGst = bool.Parse(row.Cells[12].Value.ToString());
                if (bool.Parse(row.Cells[12].Value.ToString()))
                {
                    chkGST.Checked = true;
                }
                else
                {
                    chkGST.Checked = false;
                }
                if (bool.Parse(row.Cells[13].Value.ToString()))
                {
                    chkHold.Checked = true;
                }
                else
                {
                    chkHold.Checked = false;
                }
                if (bool.Parse(row.Cells[15].Value.ToString()))
                {
                    chkMRPInc.Checked = true;
                }
                else
                {
                    chkMRPInc.Checked = false;
                }

                itcode = Int32.Parse(row.Cells[14].Value.ToString());
            }
            catch (Exception ee)
            {

            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            count = 1;
            Form_upload fu = new Form_upload();
            fu.pPApplPID = this.pPApplPID;
            fu.pPara = this.pPara;
            fu.pFrmCaption = "Batch Details";
            fu.pCompId = this.pCompId;
            fu.pComDbnm = this.pComDbnm;
            fu.pServerName = this.pServerName;
            fu.pUserId = this.pUserId;
            fu.pPassword = this.pPassword;
            fu.pPApplRange = this.pPApplRange;
            fu.pAppUerName = this.pAppUerName;
            fu.pFrmIcon = this.pFrmIcon;
            fu.pPApplText = this.pPApplText;
            fu.pPApplName = this.pPApplName;
            fu.pPApplPID = this.pPApplPID;
            fu.pPApplCode = this.pPApplCode;
            fu.pEditMode = this.pEditMode;
            fu.pAddMode = this.pAddMode;


            fu.ShowDialog();


            this.pAddMode = false;
            this.pEditMode = false;
            getBatchCode();
            bindControls(0);
            bindGrid(0);
            setEnableDisable(false);
            setNavigation();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.btnNew.Enabled)
                btnNew_Click(this.btnNew, e);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (this.btnEdit.Enabled)
                btnEdit_Click(this.btnEdit, e);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled)
                btnSave_Click(this.btnSave, e);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (this.btnCancel.Enabled)
                btnCancel_Click(this.btnCancel, e);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (this.btnDelete.Enabled)
                btnDelete_Click(this.btnDelete, e);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (this.btnLogout.Enabled)
                btnLogout_Click(this.btnLogout, e);
        }

        private void txtBatchNumber_Leave(object sender, EventArgs e)
        {
            if (count == 0)
            {
                if (txtBatchNumber.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("Batch Number cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtBatchNumber.Focus();
                }
            }
        }

        private void txtBatchRate_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void txtPrate_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void txtSrate_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void txtPtr_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void txtPts_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void btnStoreName_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select Ware_nm from warehouse where IsStoreWare='s'";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Store Name";
            vSearchCol = "Ware_nm";
            vDisplayColumnList = "Ware_nm:Store Name";
            vReturnCol = "Ware_nm";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.Icon = pFrmIcon;
            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();

            if (oSelectPop.pReturnArray != null)
            {
                txtStoreName.Text = oSelectPop.pReturnArray[0];

            }
        }

        private void dtMfg_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtExp_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnUpload_MouseEnter(object sender, EventArgs e)
        {
            count = 1;
        }

        private void btnUpload_MouseLeave(object sender, EventArgs e)
        {
            count = 0;
        }

        private void dtMfg_Validating(object sender, CancelEventArgs e)
        {
            DateTime fromdate = Convert.ToDateTime(dtMfg.Text);
            DateTime todate = Convert.ToDateTime(dtExp.Text);
            if (fromdate > todate)
            {
                MessageBox.Show("Manufacture Date cannot be greater than Expiry Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtMfg.Text = "";
                dtMfg.Focus();
            }
        }

        private void dtExp_Validating(object sender, CancelEventArgs e)
        {
            DateTime fromdate = Convert.ToDateTime(dtMfg.Text);
            DateTime todate = Convert.ToDateTime(dtExp.Text);
            if (fromdate > todate)
            {
                MessageBox.Show("Expiry Date cannot be less than Manufacture Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtExp.Text = "";
                dtExp.Focus();
            }
        }

        private void btnName_Leave(object sender, EventArgs e)
        {
            if (count == 0 && (pAddMode == true || pEditMode == true))
            {
                if (txtItemName.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("Item Name cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    btnName.Focus();
                }
            }
        }

        private void bindGrid(int val)
        {

            if (val == 0)
            {
                SqlStr = "select  Ltrim(Rtrim(I.it_name)) as it_name,b.BatchNo,BatchRate,Inc_gst,BatchOnHold,convert (CHAR(10),Mfgdt,105) as Mfgdt,convert (CHAR(10),Expdt,105) as Expdt,b.prate,srate,ptr,pts,storenm,Qty,MRPINC_gst,b.It_code from pRetBatchDetails b inner join IT_MAST i  ON b.It_code = i.It_code";
            }
            else if (val == 1)
            {
                SqlStr = "select  Ltrim(Rtrim(I.it_name)) as it_name,b.BatchNo,BatchRate,Inc_gst,BatchOnHold,convert (CHAR(10),Mfgdt,105) as Mfgdt,convert (CHAR(10),Expdt,105) as Expdt,b.prate,srate,ptr,pts,storenm,Qty,MRPINC_gst,b.It_code from pRetBatchDetails b inner join IT_MAST i  ON b.It_code = i.It_code where b.it_code=" + itcode;
            }
            else if (val == 2)
            {
                SqlStr = "select  Ltrim(Rtrim(I.it_name)) as it_name,b.BatchNo,BatchRate,Inc_gst,BatchOnHold,convert (CHAR(10),Mfgdt,105) as Mfgdt,convert (CHAR(10),Expdt,105) as Expdt,b.prate,srate,ptr,pts,storenm,Qty,MRPINC_gst,b.It_code from pRetBatchDetails b inner join IT_MAST i  ON b.It_code = i.It_code where b.it_code=" + itcode + " and b.BatchNo='" + txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "'";
            }
            else if (val == 3)
            {
                SqlStr = "select  Ltrim(Rtrim(I.it_name)) as it_name,b.BatchNo,BatchRate,Inc_gst,BatchOnHold,convert (CHAR(10),Mfgdt,105) as Mfgdt,convert (CHAR(10),Expdt,105) as Expdt,b.prate,srate,ptr,pts,storenm,Qty,MRPINC_gst,b.It_code from pRetBatchDetails b inner join IT_MAST i  ON b.It_code = i.It_code where  b.BatchNo='" + txtBatchNumber.Text.ToString().Trim().Replace("'", "''") + "'";
            }
            else if (val == 4)
            {
                SqlStr = "select  Ltrim(Rtrim(I.it_name)) as it_name,b.BatchNo,BatchRate,Inc_gst,BatchOnHold,convert (CHAR(10),Mfgdt,105) as Mfgdt,convert (CHAR(10),Expdt,105) as Expdt,b.prate,srate,ptr,pts,storenm,Qty,MRPINC_gst,b.It_code from pRetBatchDetails b inner join IT_MAST i  ON b.It_code = i.It_code ";
            }

            dataGridView1.AutoGenerateColumns = false;
            dtBatchGrid = new DataTable();
            dtBatchGrid = oDataAccess.GetDataTable(SqlStr, null, 20);

            dtBatchGrid.Columns.Add("sel", typeof(System.Boolean));

            foreach (DataRow row in dtBatchGrid.Rows)
            {
                //need to set value to NewColumn column
                row["sel"] = false;   // or set it to some other value
            }
            dataGridView1.DataSource = dtBatchGrid;
            dataGridView1.Columns[0].DataPropertyName = "sel";
            dataGridView1.Columns[1].DataPropertyName = "it_name";
            dataGridView1.Columns[2].DataPropertyName = "BatchNo";
            dataGridView1.Columns[3].DataPropertyName = "BatchRate";
            dataGridView1.Columns[4].DataPropertyName = "Mfgdt";
            dataGridView1.Columns[5].DataPropertyName = "Expdt";

            //Add Rupesh G. on 05122018---Start
            dataGridView1.Columns[6].DataPropertyName = "prate";
            dataGridView1.Columns[7].DataPropertyName = "srate";
            dataGridView1.Columns[8].DataPropertyName = "ptr";
            dataGridView1.Columns[9].DataPropertyName = "pts";
            dataGridView1.Columns[10].DataPropertyName = "storenm";
            dataGridView1.Columns[11].DataPropertyName = "Qty";
            //Add Rupesh G. on 05122018---End

            dataGridView1.Columns[12].DataPropertyName = "Inc_gst";
            dataGridView1.Columns[13].DataPropertyName = "BatchOnHold";
            dataGridView1.Columns[14].DataPropertyName = "It_code";
            dataGridView1.Columns[15].DataPropertyName = "MRPINC_gst";



            dataGridView1.AutoGenerateColumns = false;


            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

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

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {

                var chk = row1.Cells["chk"].Value.ToString().Trim();
                if (chk == "False")
                {

                    row1.Cells["chk"].Value = true;

                }
                else if (chk == "True")
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
        public void setEnableDisable(bool val)
        {
            txtItemName.Enabled = val;
            txtBatchNumber.Enabled = val;
            txtBatchRate.Enabled = val;
            dtMfg.Enabled = val;
            dtExp.Enabled = val;
            //Add Rupesh G.on 05122018---Start
            txtPrate.Enabled = val;
            txtSrate.Enabled = val;
            txtPtr.Enabled = val;
            txtPts.Enabled = val;
            txtStoreName.Enabled = val;
            txtQty.Enabled = val;
            //Add Rupesh G.on 05122018---End

            chkGST.Enabled = val;
            chkHold.Enabled = val;
            chkMRPInc.Enabled = val;

            btnName.Enabled = true;
            btnBatchNo.Enabled = true;
            btnUpload.Enabled = false;
            btnStoreName.Enabled = false;


            if (val == true && pAddMode == true)
            {
                btnStoreName.Enabled = true;
                btnBatchNo.Enabled = false;
                txtItemName.Enabled = false;
                txtStoreName.Enabled = false;
                btnName.Focus();

                txtItemName.Text = "";
                txtBatchNumber.Text = "";
                txtBatchRate.Text = "";
                dtMfg.Text = DateTime.Now.ToString("dd-MM-yyyy");
                dtExp.Text = DateTime.Now.ToString("dd-MM-yyyy");

                //Add Rupesh G.on 05122018---Start
                txtPrate.Text = "";
                txtSrate.Text = "";
                txtPtr.Text = "";
                txtPts.Text = "";
                txtStoreName.Text = "";
                txtQty.Text = "";
                //Add Rupesh G.on 05122018---End

                chkGST.Checked = false;
                chkHold.Checked = false;
                chkMRPInc.Checked = false;
                btnUpload.Enabled = true;

            }
            else if (val == true && pEditMode == true)
            {
                btnStoreName.Enabled = true;
                btnName.Enabled = false;
                btnBatchNo.Enabled = false;
                txtItemName.Enabled = false;
                txtStoreName.Enabled = false;
                txtBatchNumber.Enabled = false;
                txtItemName.Focus();
                btnStoreName.Enabled = false;
            }
        }

        private void setNavigation()
        {
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (txtItemName.Text == "")
                {

                    btnNew.Enabled = true;
                    btnSave.Enabled = false;
                    btnEdit.Enabled = false;
                    btnCancel.Enabled = false;
                    btnDelete.Enabled = false;
                }
                else
                {

                    btnNew.Enabled = true;
                    btnSave.Enabled = false;
                    btnEdit.Enabled = true;
                    btnCancel.Enabled = false;
                    btnDelete.Enabled = true;
                }
            }
            else if (this.pAddMode == true)
            {

                btnNew.Enabled = false;
                btnSave.Enabled = true;
                btnEdit.Enabled = false;
                btnCancel.Enabled = true;
                btnDelete.Enabled = false;
            }
            else if (this.pEditMode == true)
            {

                btnNew.Enabled = false;
                btnSave.Enabled = true;
                btnEdit.Enabled = false;
                btnCancel.Enabled = true;
                btnDelete.Enabled = false;
            }
        }

        private void setval()
        {

            It_code = itcode;
            BatchNo = txtBatchNumber.Text.ToString().Trim().Replace("'", "''");
            if (txtBatchRate.Text == "")
            {
                BatchRate = 0.00;
            }
            else
            {
                BatchRate = double.Parse(txtBatchRate.Text.ToString());
            }

            if (chkGST.Checked)
            {
                Inc_gst = 1;
            }
            else
            {
                Inc_gst = 0;
            }

            if (chkHold.Checked)
            {
                BatchOnHold = 1;
            }
            else
            {
                BatchOnHold = 0;
            }
            if (chkMRPInc.Checked)
            {
                MRPIncGST = 1;
            }
            else
            {
                MRPIncGST = 0;
            }
            Mfgdt = DateTime.Parse(dtMfg.Text.ToString());
            Expdt = DateTime.Parse(dtExp.Text.ToString());

            //Add Rupesh G. on 05122018---start
            if (txtPrate.Text == "")
            {
                prate = 0.00;
            }
            else
            {
                prate = double.Parse(txtPrate.Text.ToString());
            }

            if (txtSrate.Text == "")
            {
                srate = 0.00;
            }
            else
            {
                srate = double.Parse(txtSrate.Text.ToString());
            }

            if (txtPtr.Text == "")
            {
                ptr = 0.00;
            }
            else
            {
                ptr = double.Parse(txtPtr.Text.ToString());
            }

            if (txtPts.Text == "")
            {
                pts = 0.00;
            }
            else
            {
                pts = double.Parse(txtPts.Text.ToString());
            }

            storenm = txtStoreName.Text.ToString().Trim().Replace("'", "''");

            if (txtQty.Text == "")
            {
                Qty = 0.00;
            }
            else
            {
                Qty = double.Parse(txtQty.Text.ToString());
            }
            //Add Rupesh G. on 05122018---End

        }

        public void InsertUpdate()
        {
            try
            {
                if (this.pAddMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy INSERT INTO pRetBatchDetails(It_code,BatchNo,BatchRate,Inc_gst,BatchOnHold,Mfgdt,Expdt,prate,srate,ptr,pts,storenm,Qty,MRPINC_gst)";
                    SqlStr = SqlStr + " VALUES(" + itcode + ", '" + BatchNo + "', " + BatchRate + " ";
                    SqlStr = SqlStr + ", " + Inc_gst + ", " + BatchOnHold + ", '" + Mfgdt + "', '" + Expdt + "'," + prate + "," + srate + "," + ptr + "," + pts + ",'" + storenm + "'," + Qty + "," + MRPIncGST + ") ";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                else if (this.pEditMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy update pRetBatchDetails set ";
                    SqlStr = SqlStr + "BatchRate = " + BatchRate + ", Inc_gst = " + Inc_gst + ", BatchOnHold =  " + BatchOnHold + ",";
                    SqlStr = SqlStr + "Mfgdt =  '" + Mfgdt + "', Expdt = '" + Expdt + "',";

                    SqlStr = SqlStr + "prate =  " + prate + ", srate = " + srate + ",ptr = " + ptr + ",pts = " + pts + ",storenm = '" + storenm + "',Qty = " + Qty + ",MRPINC_gst=" + MRPIncGST + "";
                    SqlStr = SqlStr + " where It_code = " + itcode + " and BatchNo='" + BatchNo + "' and storenm='" + txtStoreName.Text.ToString().Trim().Replace("'", "''") + "' ";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
            }
            catch (Exception ex)
            {

            }

        }

    }
}
