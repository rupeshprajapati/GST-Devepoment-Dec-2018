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

namespace udDoctorMast
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        string SqlStr, DocCode;
        string DrCode, Name, Add1, Add2, Add3, RgNo, Phone, Comm, created_on, create_by;
        DataTable dtDocDet;
        String cAppPId, cAppName;
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
            this.pFrmCaption = "Doctor Master";
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

        public void setEnableDisable(bool val)
        {
            txtDoctorCode.Enabled = val;
            txtDoctorName.Enabled = val;
            txtAdd1.Enabled = val;
            txtAdd2.Enabled = val;
            txtAdd3.Enabled = val;
            txtRGNO.Enabled = val;
            txtContactNo.Enabled = val;
            txtComm.Enabled = val;
            btnDoctorSearch.Enabled = true;
            if (val == true && pAddMode == true)
            {
                txtDoctorCode.Focus();
                btnDoctorSearch.Enabled = false;
                txtDoctorCode.Text = "";
                txtDoctorName.Text = "";
                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";
                txtRGNO.Text = "";
                txtContactNo.Text = "";
                txtComm.Text = "";

            }
            else if (val == true && pEditMode == true)
            {

               // txtRGNO.Enabled = false;
                txtDoctorCode.Enabled = false;
                btnDoctorSearch.Enabled = false;
                txtDoctorName.Focus();
            }
        }

        private void setNavigation()
        {
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (txtDoctorCode.Text == "")
                {
                    btnFirst.Enabled = false;
                    btnBack.Enabled = false;
                    btnForward.Enabled = false;
                    btnLast.Enabled = false;
                    btnNew.Enabled = true;
                    btnSave.Enabled = false;
                    btnEdit.Enabled = false;
                    btnCancel.Enabled = false;
                    btnDelete.Enabled = false;
                }
                else
                {
                    btnFirst.Enabled = true;
                    btnBack.Enabled = true;
                    btnForward.Enabled = true;
                    btnLast.Enabled = true;
                    btnNew.Enabled = true;
                    btnSave.Enabled = false;
                    btnEdit.Enabled = true;
                    btnCancel.Enabled = false;
                    btnDelete.Enabled = true;
                }
            }
            else if (this.pAddMode == true)
            {
                btnFirst.Enabled = false;
                btnBack.Enabled = false;
                btnForward.Enabled = false;
                btnLast.Enabled = false;
                btnNew.Enabled = false;
                btnSave.Enabled = true;
                btnEdit.Enabled = false;
                btnCancel.Enabled = true;
                btnDelete.Enabled = false;
            }
            else if (this.pEditMode == true)
            {
                btnFirst.Enabled = false;
                btnBack.Enabled = false;
                btnForward.Enabled = false;
                btnLast.Enabled = false;
                btnNew.Enabled = false;
                btnSave.Enabled = true;
                btnEdit.Enabled = false;
                btnCancel.Enabled = true;
                btnDelete.Enabled = false;
            }

            chkNavigation();
        }

        private void getDocCode()
        {
            SqlStr = "select top 1 DrCode from pRetDrMast order by Drid desc";
            DataTable dtDocCode = new DataTable();
            dtDocCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtDocCode.Rows.Count > 0)
            {
                DocCode = dtDocCode.Rows[0][0].ToString();

            }
        }

        private void bindControls()
        {
            SqlStr = "select * from pRetDrMast where DrCode='" + DocCode + "'";
            dtDocDet = new DataTable();
            dtDocDet = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtDocDet.Rows.Count > 0)
            {
                txtDoctorCode.Text = dtDocDet.Rows[0]["DrCode"].ToString();
                txtDoctorName.Text = dtDocDet.Rows[0]["Name"].ToString();

                txtAdd1.Text = dtDocDet.Rows[0]["Add1"].ToString();
                txtAdd2.Text = dtDocDet.Rows[0]["Add2"].ToString();
                txtAdd3.Text = dtDocDet.Rows[0]["Add3"].ToString();

                txtRGNO.Text = dtDocDet.Rows[0]["RgNo"].ToString();
                txtContactNo.Text = dtDocDet.Rows[0]["Phone"].ToString();
                txtComm.Text = dtDocDet.Rows[0]["Comm"].ToString();

            }
            else
            {
                txtDoctorCode.Text = "";
                txtDoctorName.Text = "";

                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";

                txtRGNO.Text = "";
                txtContactNo.Text = "";
                txtComm.Text = "";
            }
        }

        private void chkNavigation()
        {
            this.btnForward.Enabled = false;
            this.btnLast.Enabled = false;
            this.btnFirst.Enabled = false;
            this.btnBack.Enabled = false;

            DataSet dsMain = new DataSet();
            SqlStr = "select * from pRetDrMast";
            dsMain = oDataAccess.GetDataSet(SqlStr, null, 20);

            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (dsMain.Tables[0].Rows.Count > 0)
                {
                    string vMainFldVal =txtDoctorCode.Text.ToString();

                    //SqlStr = "select SRoom_id from " + vMainTblNm + " Where " + vMainField + ">'" + vMainFldVal + "'";
                    SqlStr = "select * from pRetDrMast Where DrCode >'" + vMainFldVal + "' ";

                    DataSet dsTemp = new DataSet();
                    dsTemp = oDataAccess.GetDataSet(SqlStr, null, 20);

                    if (dsTemp.Tables[0].Rows.Count > 0)
                    {
                        this.btnForward.Enabled = true;
                        this.btnLast.Enabled = true;
                    }
                    //SqlStr = "select SRoom_id from " + vMainTblNm + " Where " + vMainField + "<'" + vMainFldVal + "'";
                    SqlStr = "select * from pRetDrMast Where DrCode <'" + vMainFldVal + "' ";

                    dsTemp = oDataAccess.GetDataSet(SqlStr, null, 20);
                    if (dsTemp.Tables[0].Rows.Count > 0)
                    {
                        this.btnBack.Enabled = true;
                        this.btnFirst.Enabled = true;
                    }
                }
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            SqlStr = "select top 1 DrCode from pRetDrMast";
            dtDocDet = new DataTable();
            dtDocDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtDocDet.Rows.Count > 0)
            {
                DocCode = dtDocDet.Rows[0][0].ToString();
                bindControls();
            }

            chkNavigation();

        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            SqlStr = "select top 1 DrCode from pRetDrMast order by Drid desc";
            dtDocDet = new DataTable();
            dtDocDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtDocDet.Rows.Count > 0)
            {
                DocCode = dtDocDet.Rows[0][0].ToString();
                bindControls();
            }
            chkNavigation();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SqlStr = "select  DrCode from pRetDrMast where Drid=(select top 1 Drid from pRetDrMast where Drid <(select Drid from pRetDrMast where DrCode='" + txtDoctorCode.Text + "') order by Drid desc )";
            dtDocDet = new DataTable();
            dtDocDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtDocDet.Rows.Count > 0)
            {
                DocCode = dtDocDet.Rows[0][0].ToString();
                bindControls();
            }
            chkNavigation();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            SqlStr = "select  DrCode from pRetDrMast where Drid=(select top 1 Drid from pRetDrMast where Drid >(select Drid from pRetDrMast where DrCode='" + txtDoctorCode.Text + "') order by Drid)";
            dtDocDet = new DataTable();
            dtDocDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtDocDet.Rows.Count > 0)
            {
                DocCode = dtDocDet.Rows[0][0].ToString();
                bindControls();
            }
            chkNavigation();
        }

        private void txtComm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void setval()
        {
            DrCode = txtDoctorCode.Text.ToString();
            Name = txtDoctorName.Text.ToString();
            Add1 = txtAdd1.Text.ToString();
            Add2 = txtAdd2.Text.ToString();
            Add3 = txtAdd3.Text.ToString();
            RgNo = txtRGNO.Text.ToString();
            Phone = txtContactNo.Text.ToString();
            Comm = txtComm.Text.ToString();
            if (Comm == "")
            {
                Comm = "0";
            }
            created_on = DateTime.Now.ToString();
            create_by = this.pAppUerName;
        }

        private void InsertUpdate()
        {
            try
            {
                if (this.pAddMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy INSERT INTO pRetDrMast(DrCode,Name,Add1,Add2,Add3,RgNo,Phone,Comm,created_on,create_by)";
                    SqlStr = SqlStr + " VALUES('" + DrCode + "','" + Name + "','" + Add1 + "','" + Add2 + "','" + Add3 + "','" + RgNo + "','" + Phone + "'," + Comm + ",'" + created_on + "','" + create_by + "')";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                else if (this.pEditMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy update pRetDrMast set ";
                    SqlStr = SqlStr + "Name = '" + Name + "',Add1 = '" + Add1 + "', Add2 = '" + Add2 + "',Add3='" + Add3 + "',RgNo='" + RgNo + "',Phone='" + Phone + "',Comm=" + Comm + ",created_on='" + created_on + "',create_by='" + create_by + "' where DrCode='" + txtDoctorCode.Text + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getDocCode();

            bindControls();

            setEnableDisable(false);

            setNavigation();

            this.mInsertProcessIdRecord();
            
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            mDeleteProcessIdRecord();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to delete this Record ?", this.pPApplText,
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "Delete from pRetDrMast where DrCode='" + txtDoctorCode.Text.ToString() + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                catch (Exception ee)
                {
                    MessageBox.Show(" Cannot Delete !!!Transaction has generated against this Account.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }

                this.pAddMode = false;
                this.pEditMode = false;
                getDocCode();
                bindControls();
                setEnableDisable(false);
                setNavigation();

            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.pEditMode = true;
            this.pAddMode = false;
            setEnableDisable(true);
            setNavigation();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.pAddMode = false;
            this.pEditMode = false;
            getDocCode();
            bindControls();
            setEnableDisable(false);
            setNavigation();
        }

        private void btnDoctorSearch_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select DrCode,Name,RgNo from pRetDrMast";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Doctor";
            vSearchCol = "DrCode";
            vDisplayColumnList = "DrCode:Doctor Code,Name:Doctor Name,RgNo:RG.No.";
            vReturnCol = "DrCode";
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
                txtDoctorCode.Text = oSelectPop.pReturnArray[0];
            }
            pAddMode = false;
            pAddMode = false;
            setNavigation();

            DocCode = txtDoctorCode.Text.ToString();
            bindControls();
        }

        private bool validation()
        {
            DataTable dtRGNO = new DataTable();
            SqlStr = "select * from pRetDrMast where RgNo='" + txtRGNO.Text.ToString().Trim() + "' and DrCode<>'"+txtDoctorCode.Text.ToString().Trim()+"'";
            dtRGNO = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtRGNO.Rows.Count > 0 && txtRGNO.Text.ToString().Trim() != "" )
            {
                MessageBox.Show("RG.No. already exist..", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtRGNO.Focus();
                return false;
            }
            else
            {
                return true;
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDoctorName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Doctor Name cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtDoctorName.Focus();
            }
            else if (txtContactNo.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Contact Number cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtContactNo.Focus();
            }
            else if (validation() == true)
            {
                setval();
                InsertUpdate();
                this.pAddMode = false;
                this.pEditMode = false;
              //  getDocCode();
                //bindControls();
                setEnableDisable(false);
                setNavigation();
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.pAddMode = true;
            setEnableDisable(true);
            setNavigation();

            SqlStr = "SELECT top 1 RIGHT('00000000' + CAST(ISNULL(case DrCode WHEN '' THEN '000001' ELSE CAST(DrCode AS int) + 1 End, 0) AS VARCHAR), 8) as DrCode  FROM pRetDrMast order by DrCode desc";
            DataTable dtDrCode = new DataTable();
            dtDrCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtDrCode.Rows.Count > 0)
            {
                txtDoctorCode.Text = dtDrCode.Rows[0][0].ToString();
            }
            else
            {
                txtDoctorCode.Text = "00000001";
            }
            txtDoctorCode.Enabled = false;
        }


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnNew.Enabled)
                btnNew_Click(this.btnNew, e);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnEdit.Enabled)
                btnEdit_Click(this.btnEdit, e);
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled)
                btnSave_Click(this.btnSave, e);
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnCancel.Enabled)
                btnCancel_Click(this.btnCancel, e);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnDelete.Enabled)
                btnDelete_Click(this.btnDelete, e);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnLogout.Enabled)
                btnLogout_Click(this.btnLogout, e);
        }


        private void mInsertProcessIdRecord()
        {
            DataSet dsData = new DataSet();
            string sqlstr;
            int pi;
            pi = Process.GetCurrentProcess().Id;
            cAppName = "udDoctorMast.exe";
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

    }
}
