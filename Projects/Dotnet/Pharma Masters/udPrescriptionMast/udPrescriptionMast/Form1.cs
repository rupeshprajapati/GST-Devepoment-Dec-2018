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

namespace udPrescriptionMast
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        DataTable dtPrescDet;
        string SqlStr, Presc_No = "";
        string PrescNo = "", FDay = "0", DrCode = "", PentCode = "", DtAddm = "", DtDischarge = "", OldHis = "", PrescDate = "", DrPrescNo = "", DrPrescDate = "";
        static int count, dcount = 0;
        String cAppPId, cAppName;
        string drprescno;
        public Form1(string[] args)
        {
            this.pDisableCloseBtn = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            InitializeComponent();
            this.pPApplPID = 0;
            this.pPara = args;
            this.pFrmCaption = "Prescription Master";
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

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            mDeleteProcessIdRecord();
        }

        private void btnPrescription_Click(object sender, EventArgs e)
        {

            if (txtPrescriptionNo.Text.ToString() != "")
            {
                dcount++;
                DataTable dtPrescDet = new DataTable();
                frmPrescriptionDetails fpd = new frmPrescriptionDetails();

                fpd.prescNO = txtPrescriptionNo.Text;
                if (txtForDays.Text.ToString() == "")
                {
                    fpd.forDay = "0.00";
                }
                else
                {
                    fpd.forDay = txtForDays.Text;
                }

                fpd.pPApplPID = this.pPApplPID;
                fpd.pPara = this.pPara;
                fpd.pFrmCaption = "Drug/Medicine Details";
                fpd.pCompId = this.pCompId;
                fpd.pComDbnm = this.pComDbnm;
                fpd.pServerName = this.pServerName;
                fpd.pUserId = this.pUserId;
                fpd.pPassword = this.pPassword;
                fpd.pPApplRange = this.pPApplRange;
                fpd.pAppUerName = this.pAppUerName;
                fpd.pFrmIcon = this.pFrmIcon;
                fpd.pPApplText = this.pPApplText;
                fpd.pPApplName = this.pPApplName;
                fpd.pPApplPID = this.pPApplPID;
                fpd.pPApplCode = this.pPApplCode;
                fpd.pEditMode = this.pEditMode;
                fpd.pAddMode = this.pAddMode;
                fpd.pDisableCloseBtn = true;
                fpd.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please enter Prescription number...", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        public void setEnableDisable(bool val)
        {
            txtPrescriptionNo.Enabled = val;
            txtForDays.Enabled = val;

            txtDoctorCode.Enabled = val;
            txtDoctorName.Enabled = val;
            txtDoctorRegNo.Enabled = val;

            txtPatientCode.Enabled = val;
            txtPatientName.Enabled = val;
            txtPatientContact.Enabled = val;
            txtAdd1.Enabled = val;
            txtAdd2.Enabled = val;
            txtAdd3.Enabled = val;
            txtDiagnosis.Enabled = val;
            dtAdmission.Enabled = val;
            dtDischarge.Enabled = val;
            txtOldHistory.Enabled = val;

            btnDoctorSearch.Enabled = val;
            btnDoctorNew.Enabled = val;
            btnPatientSearch.Enabled = val;
            btnPatientNew.Enabled = val;
            btnPresSearch.Enabled = true;

            dtPresc.Enabled = false;
            txtDrPrescNo.Enabled = val;
            dtDrPresc.Enabled = val;

            if (val == true && pAddMode == true)
            {
                txtDoctorCode.Enabled = false;
                txtDoctorName.Enabled = false;
                txtDoctorRegNo.Enabled = false;
                txtPatientCode.Enabled = false;
                txtPatientName.Enabled = false;
                txtPatientContact.Enabled = false;
                txtAdd1.Enabled = false;
                txtAdd2.Enabled = false;
                txtAdd3.Enabled = false;
                btnPresSearch.Enabled = false;
                txtDiagnosis.Enabled = false;
                txtPrescriptionNo.Focus();

                txtPrescriptionNo.Text = "";
                txtForDays.Text = "";

                txtDoctorCode.Text = "";
                txtDoctorName.Text = "";
                txtDoctorRegNo.Text = "";

                txtPatientCode.Text = "";
                txtPatientName.Text = "";
                txtPatientContact.Text = "";
                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";
                txtDiagnosis.Text = "";
                dtAdmission.Text = "";
                dtDischarge.Text = "";
                txtOldHistory.Text = "";

                dtPresc.Text = "";
                txtDrPrescNo.Text = "";
                dtDrPresc.Text = "";

            }
            else if (val == true && pEditMode == true)
            {
                txtForDays.Focus();
                txtPrescriptionNo.Enabled = false;
                txtDoctorCode.Enabled = false;
                txtDoctorName.Enabled = false;
                txtDoctorRegNo.Enabled = false;
                txtPatientCode.Enabled = false;
                txtPatientName.Enabled = false;
                txtPatientContact.Enabled = false;
                txtAdd1.Enabled = false;
                txtAdd2.Enabled = false;
                txtAdd3.Enabled = false;
                txtDiagnosis.Enabled = false;
                btnPresSearch.Enabled = false;

            }

        }

        private void setNavigation()
        {
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (txtPrescriptionNo.Text == "")
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

        private void getPresc()
        {
            SqlStr = "select top 1 PrescNo from pRetPrescMast order by prid desc";
            DataTable dtPresc = new DataTable();
            dtPresc = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtPresc.Rows.Count > 0)
            {
                Presc_No = dtPresc.Rows[0][0].ToString();

            }
        }

        private void bindControls()
        {
            SqlStr = "Execute USP_DET_PRESC '" + Presc_No + "'";
            dtPrescDet = new DataTable();
            dtPrescDet = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtPrescDet.Rows.Count > 0)
            {
                txtPrescriptionNo.Text = dtPrescDet.Rows[0]["PrescNo"].ToString();
                txtForDays.Text = dtPrescDet.Rows[0]["FDay"].ToString();

                txtDoctorCode.Text = dtPrescDet.Rows[0]["DrCode"].ToString();
                txtDoctorName.Text = dtPrescDet.Rows[0]["drName"].ToString();
                txtDoctorRegNo.Text = dtPrescDet.Rows[0]["RgNo"].ToString();

                txtPatientCode.Text = dtPrescDet.Rows[0]["PentCode"].ToString();
                txtPatientName.Text = dtPrescDet.Rows[0]["ptName"].ToString();
                txtPatientContact.Text = dtPrescDet.Rows[0]["ResMobile"].ToString();
                txtAdd1.Text = dtPrescDet.Rows[0]["Add1"].ToString();
                txtAdd2.Text = dtPrescDet.Rows[0]["Add2"].ToString();
                txtAdd3.Text = dtPrescDet.Rows[0]["Add3"].ToString();
                txtDiagnosis.Text = dtPrescDet.Rows[0]["DtDiagno"].ToString();
                dtAdmission.Text = dtPrescDet.Rows[0]["DtAddm"].ToString();
                dtDischarge.Text = dtPrescDet.Rows[0]["DtDischarge"].ToString();
                txtOldHistory.Text = dtPrescDet.Rows[0]["OldHis"].ToString();

                dtPresc.Text = dtPrescDet.Rows[0]["PrescDate"].ToString();
                txtDrPrescNo.Text = dtPrescDet.Rows[0]["DrPrescNo"].ToString();
                dtDrPresc.Text = dtPrescDet.Rows[0]["DrPrescDate"].ToString();

            }
            else
            {
                txtPrescriptionNo.Text = "";
                txtForDays.Text = "";

                txtDoctorCode.Text = "";
                txtDoctorName.Text = "";
                txtDoctorRegNo.Text = "";

                txtPatientCode.Text = "";
                txtPatientName.Text = "";
                txtPatientContact.Text = "";
                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";
                txtDiagnosis.Text = "";
                dtAdmission.Text = "";
                dtDischarge.Text = "";
                txtOldHistory.Text = "";

                dtPresc.Text = "";
                txtDrPrescNo.Text = "";
                dtDrPresc.Text = "";
            }
        }

        private void setval()
        {
            PrescNo = txtPrescriptionNo.Text.Trim().Replace("'", "''");
            FDay = txtForDays.Text.Trim().Replace("'", "''");
            DrCode = txtDoctorCode.Text;
            PentCode = txtPatientCode.Text;
            DtAddm = dtAdmission.Text;
            DtDischarge = dtDischarge.Text;
            OldHis = txtOldHistory.Text.Trim().Replace("'", "''");

            PrescDate = dtPresc.Text;
            DrPrescNo = txtDrPrescNo.Text.Trim().Replace("'", "''");
            DrPrescDate = dtDrPresc.Text;


        }

        private void InsertUpdate()
        {
            try
            {
                if (this.pAddMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy INSERT INTO pRetPrescMast(PrescNo,FDay,DrCode,PentCode,DtAddm,DtDischarge,OldHis,PrescDate,DrPrescNo,DrPrescDate)";
                    SqlStr = SqlStr + " VALUES('" + PrescNo + "', " + FDay + ", '" + DrCode + "', '" + PentCode + "', '" + DtAddm + "', '" + DtDischarge + "', '" + OldHis + "','" + PrescDate + "','" + DrPrescNo + "','" + DrPrescDate + "' )";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                else if (this.pEditMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy update pRetPrescMast set ";
                    SqlStr = SqlStr + "FDay = " + FDay + ", DrCode = '" + DrCode + "',PentCode = '" + PentCode + "', DtAddm = '" + DtAddm + "',DtDischarge='" + DtDischarge + "',OldHis='" + OldHis + "',PrescDate='" + PrescDate + "',DrPrescNo='" + DrPrescNo + "',DrPrescDate='" + DrPrescDate + "' where PrescNo='" + txtPrescriptionNo.Text + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
            }
            catch (Exception eee)
            {
                // MessageBox.Show(eee.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getPresc();
            bindControls();
            setEnableDisable(false);
            setNavigation();
            this.mInsertProcessIdRecord();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            frmPrescriptionDetails.dtPrescDet = new DataTable();
            frmPrescriptionDetails.count = 0;

            this.pAddMode = true;
            setEnableDisable(true);
            setNavigation();

            SqlStr = "SELECT top 1 RIGHT('00000000' + CAST(ISNULL(case PrescNo WHEN '' THEN '000001' ELSE CAST(PrescNo AS int) + 1 End, 0) AS VARCHAR), 8) as PrescNo  FROM pRetPrescMast order by PrescNo desc";
            DataTable dtPresNo = new DataTable();
            dtPresNo = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtPresNo.Rows.Count > 0)
            {
                txtPrescriptionNo.Text = dtPresNo.Rows[0][0].ToString();
            }
            else
            {
                txtPrescriptionNo.Text = "00000001";
            }
            txtPrescriptionNo.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.pAddMode = false;
            this.pEditMode = false;
            getPresc();
            bindControls();
            setEnableDisable(false);
            setNavigation();
            frmPrescriptionDetails.dtPrescDet = new DataTable();
            frmPrescriptionDetails.count = 0;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            drprescno = txtDrPrescNo.Text.ToString().Trim();
            frmPrescriptionDetails.count = 0;

            SqlStr = "select GName,unit,QtyPerDay,FDay,qty from pRetPrescDet where PrescNo='" + Presc_No + "'";
            frmPrescriptionDetails.dtPrescDet = new DataTable();
            frmPrescriptionDetails.dtPrescDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            this.pEditMode = true;
            this.pAddMode = false;
            setEnableDisable(true);
            setNavigation();
        }

        private void btnDoctorSearch_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select  DrCode,Name,RgNo from pRetDrMast";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Doctor Name";
            vSearchCol = "Name";
            vDisplayColumnList = "DrCode:Doctor Code,Name:Doctor Name,RgNo:Doctor Reg.No.";
            vReturnCol = "DrCode,Name,RgNo";
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
                txtDoctorName.Text = oSelectPop.pReturnArray[1];
                txtDoctorRegNo.Text = oSelectPop.pReturnArray[2];
            }

        }

        private void btnPatientSearch_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select PentCode,Name,Add1,Add2,Add3,DtDiagno,ResMobile from pRetPatientMast";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Patient  Name";
            vSearchCol = "Name";
            vDisplayColumnList = "PentCode:Patient Code,Name:Patient Name,ResMobile:Patient Contact No.";
            vReturnCol = "PentCode,Name,Add1,Add2,Add3,DtDiagno,ResMobile";
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
                txtPatientCode.Text = oSelectPop.pReturnArray[0];
                txtPatientName.Text = oSelectPop.pReturnArray[1];
                txtAdd1.Text = oSelectPop.pReturnArray[2];
                txtAdd2.Text = oSelectPop.pReturnArray[3];
                txtAdd3.Text = oSelectPop.pReturnArray[4];
                txtDiagnosis.Text = oSelectPop.pReturnArray[5];
                txtPatientContact.Text = oSelectPop.pReturnArray[6];
            }
        }

        private void btnPrescription_MouseHover(object sender, EventArgs e)
        {
            //if(this.pEditMode==false && this.pAddMode==false && txtPrescriptionNo.Text!="")
            //{
            //    if (txtPrescriptionNo.Text.ToString() != "")
            //    {
            //        DataTable dtPrescDet = new DataTable();
            //        frmPrescriptionDetails fpd = new frmPrescriptionDetails();

            //        fpd.prescNO = txtPrescriptionNo.Text;

            //        fpd.pPApplPID = this.pPApplPID;
            //        fpd.pPara = this.pPara;
            //        fpd.pFrmCaption = "InterestRate Details";
            //        fpd.pCompId = this.pCompId;
            //        fpd.pComDbnm = this.pComDbnm;
            //        fpd.pServerName = this.pServerName;
            //        fpd.pUserId = this.pUserId;
            //        fpd.pPassword = this.pPassword;
            //        fpd.pPApplRange = this.pPApplRange;
            //        fpd.pAppUerName = this.pAppUerName;
            //        fpd.pFrmIcon = this.pFrmIcon;
            //        fpd.pPApplText = this.pPApplText;
            //        fpd.pPApplName = this.pPApplName;
            //        fpd.pPApplPID = this.pPApplPID;
            //        fpd.pPApplCode = this.pPApplCode;
            //        fpd.pEditMode = this.pEditMode;
            //        fpd.pAddMode = this.pAddMode;

            //        fpd.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please enter Prescription number...");
            //    }
            //}

        }

        private void btnDoctorNew_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "udDoctorMast.exe";
            p.Start();

        }

        private void txtForDays_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtForDays.Text, "[^0-9]"))
            {
                MessageBox.Show("Allow only numeric value", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtForDays.Text = txtForDays.Text.Remove(txtForDays.Text.Length - 1);
                txtForDays.Focus();
                txtForDays.SelectionStart = txtForDays.Text.Length;
            }
        }

        private void txtPatientContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void chkNavigation()
        {
            this.btnForward.Enabled = false;
            this.btnLast.Enabled = false;
            this.btnFirst.Enabled = false;
            this.btnBack.Enabled = false;

            DataSet dsMain = new DataSet();
            SqlStr = "select * from pRetPrescMast";
            dsMain = oDataAccess.GetDataSet(SqlStr, null, 20);

            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (dsMain.Tables[0].Rows.Count > 0)
                {
                    string vMainFldVal = txtPrescriptionNo.Text.ToString();

                   
                    SqlStr = "select * from pRetPrescMast Where PrescNo >'" + vMainFldVal + "' ";

                    DataSet dsTemp = new DataSet();
                    dsTemp = oDataAccess.GetDataSet(SqlStr, null, 20);

                    if (dsTemp.Tables[0].Rows.Count > 0)
                    {
                        this.btnForward.Enabled = true;
                        this.btnLast.Enabled = true;
                    }
                   
                    SqlStr = "select * from pRetPrescMast Where PrescNo <'" + vMainFldVal + "' ";

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
            SqlStr = "select top 1 PrescNo from pRetPrescMast";
            dtPrescDet = new DataTable();
            dtPrescDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPrescDet.Rows.Count > 0)
            {
                Presc_No = dtPrescDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }



        private void btnLast_Click(object sender, EventArgs e)
        {
            SqlStr = "select top 1 PrescNo from pRetPrescMast order by prid desc";
            dtPrescDet = new DataTable();
            dtPrescDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPrescDet.Rows.Count > 0)
            {
                Presc_No = dtPrescDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SqlStr = "select  PrescNo from pRetPrescMast where prid=(select top 1 prid from pRetPrescMast where prid <(select prid from pRetPrescMast where PrescNo='" + txtPrescriptionNo.Text + "') order by prid desc )";
            dtPrescDet = new DataTable();
            dtPrescDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPrescDet.Rows.Count > 0)
            {
                Presc_No = dtPrescDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }

        private void dtAdmission_Validating(object sender, CancelEventArgs e)
        {
            DateTime fromdate = Convert.ToDateTime(txtDiagnosis.Text);
            DateTime todate = Convert.ToDateTime(dtAdmission.Text);

            DateTime todate1 = Convert.ToDateTime(dtDischarge.Text);

            if (fromdate > todate)
            {
                MessageBox.Show(" Admission Date cannot be less than Diagnosis Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtAdmission.Text = "";
                dtAdmission.Focus();
            }
            else if (todate > todate1)
            {
                MessageBox.Show(" Admission Date cannot be greater than Discharge Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtAdmission.Text = "";
                dtAdmission.Focus();
            }

        }

        private void dtDischarge_Validating(object sender, CancelEventArgs e)
        {
            DateTime fromdate = Convert.ToDateTime(txtDiagnosis.Text);
            DateTime todate = Convert.ToDateTime(dtAdmission.Text);

            DateTime todate1 = Convert.ToDateTime(dtDischarge.Text);

            if (fromdate > todate1)
            {
                MessageBox.Show(" Discharge Date cannot be less than Diagnosis Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtDischarge.Text = "";
                dtDischarge.Focus();
            }
            else if (todate > todate1)
            {
                MessageBox.Show(" Discharge Date cannot be less than Admission Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtDischarge.Text = "";
                dtDischarge.Focus();
            }
        }

        private void dtDrPresc_Validating(object sender, CancelEventArgs e)
        {
            DateTime fromdate = Convert.ToDateTime(txtDiagnosis.Text);
            DateTime todate = Convert.ToDateTime(dtDrPresc.Text);

            if (fromdate > todate)
            {
                MessageBox.Show(" Prescription Date cannot be less than Diagnosis Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtDrPresc.Text = "";
                dtDrPresc.Focus();
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            SqlStr = "select  PrescNo from pRetPrescMast where prid=(select top 1 prid from pRetPrescMast where prid >(select prid from pRetPrescMast where PrescNo='" + txtPrescriptionNo.Text + "') order by prid)";
            dtPrescDet = new DataTable();
            dtPrescDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPrescDet.Rows.Count > 0)
            {
                Presc_No = dtPrescDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to delete this Record ?", this.pPApplText,
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "Delete from pRetPrescMast where PrescNo='" + txtPrescriptionNo.Text.ToString() + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                    oDataAccess.BeginTransaction();
                    SqlStr = "Delete from pRetPrescDet where PrescNo='" + txtPrescriptionNo.Text.ToString() + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                catch (Exception ee)
                {
                    MessageBox.Show(" Cannot Delete !!!Transaction has generated against this Account.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }

                this.pAddMode = false;
                this.pEditMode = false;
                getPresc();
                bindControls();
                setEnableDisable(false);
                setNavigation();

            }

            frmPrescriptionDetails.dtPrescDet = new DataTable();
            frmPrescriptionDetails.count = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool drprescmsg;
                drprescmsg = false;
                SqlStr = "select * from pRetPrescMast where DrCode='" + txtDoctorCode.Text.ToString() + "' and DrPrescNo='" + txtDrPrescNo.Text.ToString().Trim().Replace("'", "''") + "'";
                DataTable dt = new DataTable();
                dt = oDataAccess.GetDataTable(SqlStr, null, 20);
                if (dt.Rows.Count > 0)
                {
                    if (this.pEditMode == true)
                    {
                        if (txtDrPrescNo.Text.ToString().Trim().ToLower() != drprescno.Trim().ToLower())
                        {
                            drprescmsg = true;

                        }
                    }
                    else
                    {
                        drprescmsg = true;


                    }
                }


                if (txtPrescriptionNo.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("Prescription Number cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPrescriptionNo.Focus();
                }
                else if (txtForDays.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("For Days cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtForDays.Focus();
                }
                else if (txtDoctorCode.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("Doctor Code cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    btnDoctorSearch.Focus();
                }
                else if (txtPatientCode.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("Patient Code cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    btnPatientSearch.Focus();
                }
                else if (drprescmsg)
                {
                    MessageBox.Show("Doctor Prescription Number already exist.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtDrPrescNo.Focus();

                }
                else if (txtDrPrescNo.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("Doctor Prescription Number  cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtDrPrescNo.Focus();
                }
                else
                {
                    DataTable dd = new DataTable();
                    try
                    {
                        dd = frmPrescriptionDetails.dtPrescDet;
                    }
                    catch (Exception ee)
                    {
                        dd = new DataTable();
                    }


                    if (dd.Rows.Count > 0 || (this.pEditMode == true && dcount == 0))
                    {
                        setval();
                        InsertUpdate();

                        if (dd != null)
                        {
                            if (dd.Rows.Count > 0)
                            {
                                oDataAccess.BeginTransaction();
                                SqlStr = "Delete from pRetPrescDet where PrescNo='" + txtPrescriptionNo.Text + "'";
                                oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                                oDataAccess.CommitTransaction();

                                foreach (DataRow dr in dd.Rows)
                                {
                                    oDataAccess.BeginTransaction();
                                    SqlStr = "Set Dateformat DMY  insert into pRetPrescDet values('" + txtPrescriptionNo.Text + "','" + dr["GName"] + "'," + dr["QtyPerDay"] + "," + dr["FDay"] + ",'" + dr["Unit"] + "'," + dr["qty"] + ")";
                                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                                    oDataAccess.CommitTransaction();
                                }
                            }
                            frmPrescriptionDetails.dtPrescDet = new DataTable();

                        }


                        this.pAddMode = false;
                        this.pEditMode = false;
                        //  getPresc();
                        //   bindControls();
                        setEnableDisable(false);
                        setNavigation();
                    }
                    else
                    {
                        //oDataAccess.BeginTransaction();
                        //SqlStr = "Delete from pRetPrescDet where PrescNo='" + txtPrescriptionNo.Text + "'";
                        //oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                        //oDataAccess.CommitTransaction();

                        MessageBox.Show("Drug / Medicine Details cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        if (txtPrescriptionNo.Text.ToString() != "")
                        {
                            DataTable dtPrescDet = new DataTable();
                            frmPrescriptionDetails fpd = new frmPrescriptionDetails();

                            fpd.prescNO = txtPrescriptionNo.Text;
                            fpd.forDay = txtForDays.Text;
                            fpd.pPApplPID = this.pPApplPID;
                            fpd.pPara = this.pPara;
                            fpd.pFrmCaption = "Drug/Medicine Details";
                            fpd.pCompId = this.pCompId;
                            fpd.pComDbnm = this.pComDbnm;
                            fpd.pServerName = this.pServerName;
                            fpd.pUserId = this.pUserId;
                            fpd.pPassword = this.pPassword;
                            fpd.pPApplRange = this.pPApplRange;
                            fpd.pAppUerName = this.pAppUerName;
                            fpd.pFrmIcon = this.pFrmIcon;
                            fpd.pPApplText = this.pPApplText;
                            fpd.pPApplName = this.pPApplName;
                            fpd.pPApplPID = this.pPApplPID;
                            fpd.pPApplCode = this.pPApplCode;
                            fpd.pEditMode = this.pEditMode;
                            fpd.pAddMode = this.pAddMode;

                            fpd.Show();
                        }
                        else
                        {
                            MessageBox.Show("Please enter Prescription number...");
                        }
                    }
                    dcount = 0;
                }
                frmPrescriptionDetails.count = 0;
            }
            catch (Exception ee)
            {

            }
        }

        private void btnPresSearch_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select  PrescNo,pt.Name as Name,pt.ResMobile as ResMobile,DrPrescNo  from pRetPrescMast pr inner join pRetPatientMast pt ON pr.PentCode = pt.PentCode";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Prescription ";
            vSearchCol = "Name";
            vDisplayColumnList = "DrPrescNo:Doctor Prescription Number,Name:Patient Name,ResMobile:Patient Contact No.";
            vReturnCol = "PrescNo";
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
                txtPrescriptionNo.Text = oSelectPop.pReturnArray[0];
            }
            pAddMode = false;
            pAddMode = false;
            setNavigation();

            Presc_No = txtPrescriptionNo.Text.ToString();
            bindControls();
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
            cAppName = "udPrescriptionMast.exe";
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
