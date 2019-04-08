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

namespace udPatientMast
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        string SqlStr, PatCode;
        string PentCode, Name, Add1, Add2, Add3, Disease, DtDiagno, DtTreat, Age, Gen, SW, DtBirth, Pin, Id, ResMobile, Disc, created_on, create_by, panno, aadharno;
        string Pstatus;  //Added by Prajakta B. on 04-12-2018 for Bug 32062
        int count = 0;
        DataTable dtPatDet;
        DataAccess_Net.clsDataAccess oDataAccess;
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
            this.pFrmCaption = "Patient Master";
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

        private void getPatCode()
        {
            SqlStr = "select top 1 PentCode from pRetPatientMast order by ptid desc";
            DataTable dtPatCode = new DataTable();
            dtPatCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtPatCode.Rows.Count > 0)
            {
                PatCode = dtPatCode.Rows[0][0].ToString();
            }
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void bindControls()
        {
            SqlStr = "select * from pRetPatientMast where PentCode='" + PatCode + "'";
            dtPatDet = new DataTable();
            dtPatDet = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtPatDet.Rows.Count > 0)
            {
                txtPatientCode.Text = dtPatDet.Rows[0]["PentCode"].ToString();
                txtPatientName.Text = dtPatDet.Rows[0]["Name"].ToString();
                txtPatientId.Text = dtPatDet.Rows[0]["Id"].ToString();
                txtPatientSoWo.Text = dtPatDet.Rows[0]["SW"].ToString();
                string gen = dtPatDet.Rows[0]["Gen"].ToString();
                if (gen == "M")
                {
                    rdbMale.Checked = true;
                }
                else if (gen == "F")
                {
                    rdbFemale.Checked = true;
                }
                txtPatientContact.Text = dtPatDet.Rows[0]["ResMobile"].ToString();
                txtAdd1.Text = dtPatDet.Rows[0]["Add1"].ToString();
                txtAdd2.Text = dtPatDet.Rows[0]["Add2"].ToString();
                txtAdd3.Text = dtPatDet.Rows[0]["Add3"].ToString();
                txtPinNo.Text = dtPatDet.Rows[0]["Pin"].ToString();
                dtBirthDate.Text = dtPatDet.Rows[0]["DtBirth"].ToString();
                txtAge.Text = dtPatDet.Rows[0]["Age"].ToString();
                txtDiease.Text = dtPatDet.Rows[0]["Disease"].ToString();
                dtDiagnoDate.Text = dtPatDet.Rows[0]["DtDiagno"].ToString();
                dtTreatmentDate.Text = dtPatDet.Rows[0]["DtTreat"].ToString();
                txtDiscount.Text = dtPatDet.Rows[0]["Disc"].ToString();

                txtPanCard.Text = dtPatDet.Rows[0]["panno"].ToString();
                txtAadharCard.Text = dtPatDet.Rows[0]["aadharno"].ToString();
                //drpdwn_pstatus.Text= dtPatDet.Rows[0]["Pstatus"].ToString();  //Added by Prajakta B. on 04122018
                drpdwn_pstatus.SelectedItem = dtPatDet.Rows[0]["Pstatus"].ToString();  //Added by Prajakta B. on 04122018
            }
            else
            {
                txtPatientCode.Text = "";
                txtPatientName.Text = "";
                txtPatientId.Text = "";
                txtPatientSoWo.Text = "";

                rdbDefault.Checked = true;

                txtPatientContact.Text = "";
                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";
                txtPinNo.Text = "";
                dtBirthDate.Text = "";
                txtAge.Text = "";
                txtDiease.Text = "";
                dtDiagnoDate.Text = "";
                dtTreatmentDate.Text = "";
                txtDiscount.Text = "";

                txtPanCard.Text = "";
                txtAadharCard.Text = "";
                //drpdwn_pstatus.Text = "";  //Added by Prajakta B. on 04122018
                drpdwn_pstatus.SelectedIndex = 0;  //Added by Prajakta B. on 04122018
            }
        }

        private void txtPatientContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtPinNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        public void setEnableDisable(bool val)
        {
            txtPatientCode.Enabled = val;
            txtPatientName.Enabled = val;
            txtPatientId.Enabled = val;
            txtPatientSoWo.Enabled = val;
            rdbMale.Enabled = val;
            rdbFemale.Enabled = val;
            rdbDefault.Enabled = val;
            txtPatientContact.Enabled = val;
            txtAdd1.Enabled = val;
            txtAdd2.Enabled = val;
            txtAdd3.Enabled = val;
            txtPinNo.Enabled = val;
            dtBirthDate.Enabled = val;
            txtAge.Enabled = false;
            txtDiease.Enabled = val;
            dtDiagnoDate.Enabled = val;
            dtTreatmentDate.Enabled = val;
            txtDiscount.Enabled = val;
            btnPatSearch.Enabled = true;
            btnDisease.Enabled = val;
            drpdwn_pstatus.Enabled = val;  //Added by Prajakta B. on 04122018

            txtPanCard.Enabled = val;
            txtAadharCard.Enabled = val;
            if (val == true && pAddMode == true)
            {
                txtPatientName.Focus();
                txtPatientCode.Enabled = false;
                txtDiease.Enabled = false;
                btnPatSearch.Enabled = false;

                txtPatientName.Text = "";
                txtPatientId.Text = "";
                txtPatientSoWo.Text = "";
                rdbDefault.Checked = true;
                txtPatientContact.Text = "";
                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";
                txtPinNo.Text = "";
                dtBirthDate.Text = "";
                txtAge.Text = "00.00";
                txtDiease.Text = "";
                dtDiagnoDate.Text = "";
                dtTreatmentDate.Text = "";
                txtDiscount.Text = "";
                txtPanCard.Text = "";
                txtAadharCard.Text = "";
               // drpdwn_pstatus.Text = "";  //Added by Prajakta B. on 04122018
                drpdwn_pstatus.SelectedIndex = 0;  //Added by Prajakta B. on 04122018
            }
            else if (val == true && pEditMode == true)
            {
                txtPatientCode.Enabled = false;
                btnPatSearch.Enabled = false;
                txtDiease.Enabled = false;
                txtPatientName.Focus();
            }
        }

        private void setNavigation()
        {
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (txtPatientCode.Text == "")
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

        private void setval()
        {
            PentCode = txtPatientCode.Text;
            Name = txtPatientName.Text.Trim().Replace("'", "''"); ;
            Add1 = txtAdd1.Text.Trim().Replace("'", "''"); ;
            Add2 = txtAdd2.Text.Trim().Replace("'", "''"); ;
            Add3 = txtAdd3.Text.Trim().Replace("'", "''"); ;
            Disease = txtDiease.Text.Trim().Replace("'", "''"); ;
            DtDiagno = DateTime.Parse(dtDiagnoDate.Text).ToString("dd/MM/yyyy");
            DtTreat = DateTime.Parse(dtTreatmentDate.Text).ToString("dd/MM/yyyy");
            Age = txtAge.Text;
            if (rdbMale.Checked == true)
            {
                Gen = "M";
            }
            else if (rdbFemale.Checked == true)
            {
                Gen = "F";
            }
            else
            {
                Gen = "D";
            }

            SW = txtPatientSoWo.Text.Trim().Replace("'", "''"); ;
            DtBirth = DateTime.Parse(dtBirthDate.Text).ToString("dd/MM/yyyy");
            Pin = txtPinNo.Text;
            Id = txtPatientId.Text;
            ResMobile = txtPatientContact.Text;
            if (txtDiscount.Text.ToString().Trim() == "")
            {
                Disc = "00.00";
            }
            else
            {
                Disc = txtDiscount.Text;
            }

            created_on = DateTime.Now.ToString();
            create_by = this.pAppUerName;

            panno = txtPanCard.Text;
            aadharno = txtAadharCard.Text;
            Pstatus = drpdwn_pstatus.SelectedItem.ToString(); //Added By Prajakta B. on 04122018
            if (Pstatus == "")
            {
                Pstatus = "Walk-In";
            }
        }

        private void InsertUpdate()
        {
            try
            {
                if (this.pAddMode == true)
                {
                    oDataAccess.BeginTransaction();
                    //Commented By Prajakta B. on 04122018 for bug 32062 --Start
                    //SqlStr = "set dateformat dmy INSERT INTO pRetPatientMast(PentCode,Name,Add1,Add2,Add3,Disease,DtDiagno,DtTreat,Age,Gen,SW,DtBirth,Pin,Id,ResMobile,Disc,created_on,create_by,panno,aadharno)";
                    //SqlStr = SqlStr + " VALUES('" + PentCode + "','" + Name + "','" + Add1 + "','" + Add2 + "','" + Add3 + "','" + Disease + "','" + DtDiagno + "','" + DtTreat + "'," + Age + ",'" + Gen + "','" + SW + "','" + DtBirth + "','" + Pin + "','" + Id + "','" + ResMobile + "'," + Disc + ",'" + created_on + "','" + create_by + "','" + panno + "','" + aadharno + "')"; 
                    //Commented By Prajakta B. on 04122018 for bug 32062 --End        

                    //Modified By Prajakta B. on 04122018 for bug 32062 --Start                                                                                                                                                                                                                                                                                                                                                                                     
                    SqlStr = "set dateformat dmy INSERT INTO pRetPatientMast(PentCode,Name,Add1,Add2,Add3,Disease,DtDiagno,DtTreat,Age,Gen,SW,DtBirth,Pin,Id,ResMobile,Disc,created_on,create_by,panno,aadharno,PStatus)";
                    SqlStr = SqlStr + " VALUES('" + PentCode + "','" + Name + "','" + Add1 + "','" + Add2 + "','" + Add3 + "','" + Disease + "','" + DtDiagno + "','" + DtTreat + "'," + Age + ",'" + Gen + "','" + SW + "','" + DtBirth + "','" + Pin + "','" + Id + "','" + ResMobile + "'," + Disc + ",'" + created_on + "','" + create_by + "','" + panno + "','" + aadharno + "','" + Pstatus + "')";
                    //Modified By Prajakta B. on 04122018 for bug 32062 --End
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                else if (this.pEditMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy update pRetPatientMast set ";
                    // SqlStr = SqlStr + "Name='" + Name + "',Add1='" + Add1 + "',Add2='" + Add2 + "',Add3='" + Add3 + "',Disease='" + Disease + "',DtDiagno='" + DtDiagno + "',DtTreat='" + DtTreat + "',Age=" + Age + ",Gen='" + Gen + "',SW='" + SW + "',DtBirth='" + DtBirth + "',Pin='" + Pin + "',Id='" + Id + "',ResMobile='" + ResMobile + "',Disc='" + Disc + "',created_on='" + created_on + "',create_by='" + create_by + "',panno='" + panno + "',aadharno='" + aadharno + "'  where PentCode='" + txtPatientCode.Text + "'"; Commented By Prajakta B. on 04122018 for bug 32062 
                    SqlStr = SqlStr + "Name='" + Name + "',Add1='" + Add1 + "',Add2='" + Add2 + "',Add3='" + Add3 + "',Disease='" + Disease + "',DtDiagno='" + DtDiagno + "',DtTreat='" + DtTreat + "',Age=" + Age + ",Gen='" + Gen + "',SW='" + SW + "',DtBirth='" + DtBirth + "',Pin='" + Pin + "',Id='" + Id + "',ResMobile='" + ResMobile + "',Disc='" + Disc + "',created_on='" + created_on + "',create_by='" + create_by + "',panno='" + panno + "',aadharno='" + aadharno + "' ,PStatus='" + Pstatus + "' where PentCode='" + txtPatientCode.Text + "'";  //Modified By Prajakta B. 04122018  for bug 32062 
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.ToString());
            }
        }

        private bool validation()
        {

            System.Text.RegularExpressions.Regex rPan = new System.Text.RegularExpressions.Regex(@"^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$");
            if (txtPanCard.Text != "" && count == 0)
            {
                if (!rPan.IsMatch(txtPanCard.Text))
                {
                    MessageBox.Show("Invalid PAN Card Number", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPanCard.Focus();
                    return false;
                }

            }
            if (txtAadharCard.Text.Length != 12 && txtAadharCard.Text != "" && count == 0)
            {
                MessageBox.Show("Invalid Aadhar Card Number", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtAadharCard.Focus();
                return false;
            }
            return true;

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
                    SqlStr = "Delete from pRetPatientMast where PentCode='" + txtPatientCode.Text.ToString() + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                catch (Exception ee)
                {
                    MessageBox.Show(" Cannot Delete !!!Transaction has generated against this Account.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }

                this.pAddMode = false;
                this.pEditMode = false;
                getPatCode();
                bindControls();
                setEnableDisable(false);
                setNavigation();

            }
        }

        private void txtPatientContact_Leave(object sender, EventArgs e)
        {
            if (rdbMale.Checked == false && rdbFemale.Checked == false)
            {
                rdbMale.Focus();
            }
        }

        private void txtPanCard_Leave(object sender, EventArgs e)
        {
            validation();
            count = 0;
        }

        private void txtPanCard_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAadharCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void txtAadharCard_Leave(object sender, EventArgs e)
        {
            validation();
            count = 0;
        }

        private void txtPanCard_KeyUp(object sender, KeyEventArgs e)
        {
            //txtPanCard.CharacterCasing = CharacterCasing.Upper;
            string a = txtPanCard.Text.ToUpper();
            txtPanCard.Text = "";
            txtPanCard.Text = a;
            txtPanCard.SelectionStart = txtPanCard.Text.Length;
        }

        private void txtAge_Leave(object sender, EventArgs e)
        {
            //DateTime dt = DateTime.Now;

            //DateTime dt1 = dt.AddDays(-(365*1));

        }

        private void dtTreatmentDate_ValueChanged(object sender, EventArgs e)
        {
            //DateTime fromdate = Convert.ToDateTime(dtDiagnoDate.Text);
            //DateTime todate = Convert.ToDateTime(dtTreatmentDate.Text);
            //if (fromdate > todate)
            //{
            //    MessageBox.Show(" Treatment Date cannot be less than Diagnose Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    dtTreatmentDate.Text = "";
            //    dtTreatmentDate.Focus();
            //}
        }

        private void dtTreatmentDate_Leave(object sender, EventArgs e)
        {

            //DateTime DiagnoDate = new DateTime();
            //DiagnoDate = dtDiagnoDate.Value;

            //DateTime Treatment = new DateTime();
            //Treatment = dtTreatmentDate.Value;

            //if (DiagnoDate > Treatment )
            //{
            //    MessageBox.Show("Treatment Date cannot be less than Diagnose Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    dtTreatmentDate.Text = "";
            //    dtTreatmentDate.Focus();

            //}
        }

        private void dtDiagnoDate_Leave(object sender, EventArgs e)
        {
            //DateTime DiagnoDate = new DateTime();
            //DiagnoDate = dtDiagnoDate.Value;

            //DateTime Treatment = new DateTime();
            //Treatment = dtTreatmentDate.Value;

            //if (Treatment > DiagnoDate)
            //{
            //    MessageBox.Show(" Diagnose Date cannot be greater than Treatment Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    dtDiagnoDate.Text = "";
            //    dtDiagnoDate.Focus();
            //}
        }

        private void dtDiagnoDate_ValueChanged(object sender, EventArgs e)
        {
            //DateTime fromdate = Convert.ToDateTime(dtDiagnoDate.Text);
            //DateTime todate = Convert.ToDateTime(dtTreatmentDate.Text);
            //if (fromdate > todate)
            //{
            //    MessageBox.Show(" Diagnose Date cannot be greater than Treatment Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    dtDiagnoDate.Text = "";
            //    dtDiagnoDate.Focus();
            //}

        }

        private void dtBirthDate_Validating(object sender, CancelEventArgs e)
        {
            if (this.pAddMode == true || this.pEditMode == true)
            {
                DateTime date = DateTime.Now.Date;

                var dtBirthDate1 = dtBirthDate.Value.Date;


                var dtDigo = dtDiagnoDate.Value.Date;

                if (dtBirthDate1 > date)
                {
                    MessageBox.Show("Date of birth cannot be greater than current date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    dtBirthDate.Text = "";
                    dtBirthDate.Focus();
                }
                else if (dtBirthDate1 > dtDigo)
                {
                    MessageBox.Show("Date of birth cannot be greater than Diagnose date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    dtBirthDate.Text = "";
                    dtBirthDate.Focus();
                }
                else
                {

                    System.TimeSpan diffResult = date.Subtract(dtBirthDate1);

                    string TotalDays = diffResult.Days.ToString();
                    string Months = ((diffResult.Days) % 365).ToString();
                    string RemainingMonths = (Convert.ToInt32(Months) / 31).ToString("00");
                    string RemainginYears = ((diffResult.Days) / 365).ToString("00");
                    string RemainingDays = (Convert.ToInt32(Months) % 31).ToString();
                    if (Int32.Parse(RemainginYears) < 0 || Int32.Parse(RemainingDays) < 0)
                    {
                        MessageBox.Show("Date of birth cannot be greater than current date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        dtBirthDate.Text = "";
                        dtBirthDate.Focus();
                    }
                    else
                    {
                        txtAge.Text = (RemainginYears + "." + RemainingMonths).ToString();
                    }
                    //txtAge.Text = age.ToString();
                }
            }

        }

        private void dtDiagnoDate_Validating(object sender, CancelEventArgs e)
        {
            DateTime fromdate = Convert.ToDateTime(dtDiagnoDate.Text);
            DateTime todate = Convert.ToDateTime(dtTreatmentDate.Text);

            var dtBirthDate1 = dtBirthDate.Value.Date;


            var dtDigo = dtDiagnoDate.Value.Date;


            if (fromdate > todate)
            {
                MessageBox.Show(" Diagnose Date cannot be greater than Treatment Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtDiagnoDate.Text = "";
                dtDiagnoDate.Focus();
            }
            else if (dtBirthDate1 > dtDigo)
            {
                MessageBox.Show("Date of birth cannot be greater than Diagnose date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtDiagnoDate.Text = "";
                dtDiagnoDate.Focus();
            }
        }

        private void dtTreatmentDate_Validating(object sender, CancelEventArgs e)
        {
            DateTime fromdate = Convert.ToDateTime(dtDiagnoDate.Text);
            DateTime todate = Convert.ToDateTime(dtTreatmentDate.Text);
            if (fromdate > todate)
            {
                MessageBox.Show(" Treatment Date cannot be less than Diagnose Date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtTreatmentDate.Text = "";
                dtTreatmentDate.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.pAddMode = false;
            this.pEditMode = false;
            getPatCode();
            bindControls();
            setEnableDisable(false);
            setNavigation();
        }

        private void drpdwn_pstatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPatientSoWo_Leave(object sender, EventArgs e)
        {

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.pAddMode = true;
            setEnableDisable(true);
            setNavigation();

            SqlStr = "SELECT top 1 RIGHT('00000000' + CAST(ISNULL(case PentCode WHEN '' THEN '000001' ELSE CAST(PentCode AS int) + 1 End, 0) AS VARCHAR), 8) as PentCode  FROM pRetPatientMast order by PentCode desc";
            DataTable dtDrCode = new DataTable();
            dtDrCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtDrCode.Rows.Count > 0)
            {
                txtPatientCode.Text = dtDrCode.Rows[0][0].ToString();
            }
            else
            {
                txtPatientCode.Text = "00000001";
            }
            txtPatientCode.Enabled = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.pEditMode = true;
            this.pAddMode = false;
            setEnableDisable(true);
            setNavigation();
        }

        private void chkNavigation()
        {
            this.btnForward.Enabled = false;
            this.btnLast.Enabled = false;
            this.btnFirst.Enabled = false;
            this.btnBack.Enabled = false;

            DataSet dsMain = new DataSet();
            SqlStr = "select * from pRetPatientMast";
            dsMain = oDataAccess.GetDataSet(SqlStr, null, 20);

            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (dsMain.Tables[0].Rows.Count > 0)
                {
                    string vMainFldVal = txtPatientCode.Text.ToString();

                    //SqlStr = "select SRoom_id from " + vMainTblNm + " Where " + vMainField + ">'" + vMainFldVal + "'";
                    SqlStr = "select * from pRetPatientMast Where PentCode >'" + vMainFldVal + "' ";

                    DataSet dsTemp = new DataSet();
                    dsTemp = oDataAccess.GetDataSet(SqlStr, null, 20);

                    if (dsTemp.Tables[0].Rows.Count > 0)
                    {
                        this.btnForward.Enabled = true;
                        this.btnLast.Enabled = true;
                    }
                    //SqlStr = "select SRoom_id from " + vMainTblNm + " Where " + vMainField + "<'" + vMainFldVal + "'";
                    SqlStr = "select * from pRetPatientMast Where PentCode <'" + vMainFldVal + "' ";

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

            SqlStr = "select top 1 PentCode from pRetPatientMast";
            dtPatDet = new DataTable();
            dtPatDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPatDet.Rows.Count > 0)
            {
                PatCode = dtPatDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SqlStr = "select  PentCode from pRetPatientMast where ptid=(select top 1 ptid from pRetPatientMast where ptid <(select ptid from pRetPatientMast where PentCode='" + txtPatientCode.Text + "') order by ptid desc )";
            dtPatDet = new DataTable();
            dtPatDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPatDet.Rows.Count > 0)
            {
                PatCode = dtPatDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            SqlStr = "select  PentCode from pRetPatientMast where ptid=(select top 1 ptid from pRetPatientMast where ptid >(select ptid from pRetPatientMast where PentCode='" + txtPatientCode.Text + "') order by ptid)";
            dtPatDet = new DataTable();
            dtPatDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPatDet.Rows.Count > 0)
            {
                PatCode = dtPatDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            SqlStr = "select top 1 PentCode from pRetPatientMast order by ptid desc";
            dtPatDet = new DataTable();
            dtPatDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtPatDet.Rows.Count > 0)
            {
                PatCode = dtPatDet.Rows[0][0].ToString();
                bindControls();
                chkNavigation();
            }
        }

        private void btnPatSearch_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select PentCode,Name,ResMobile from pRetPatientMast";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Patient ";
            vSearchCol = "PentCode";
            vDisplayColumnList = "PentCode:Patient Code,Name:Patient Name,ResMobile:Contact No.";
            vReturnCol = "PentCode";
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
            }
            pAddMode = false;
            pAddMode = false;
            setNavigation();

            PatCode = txtPatientCode.Text.ToString();
            bindControls();
        }

        private void btnDisease_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            // SqlStr = "select 'A General' as 'Disease' union select 'B Onco' union select 'C Diabatic' union select 'D TB' union select 'E Cardic' ";
            SqlStr = "select Disease from pRetDiseaseMast order by Disease";
            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Disease ";
            vSearchCol = "Disease";
            vDisplayColumnList = "Disease:Disease";
            vReturnCol = "Disease";
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
                txtDiease.Text = oSelectPop.pReturnArray[0];
            }

        }

        private void dtBirthDate_ValueChanged(object sender, EventArgs e)
        {
            //if(this.pAddMode==true || this.pEditMode==true)
            //{
            //    DateTime date = DateTime.Now.Date;

            //   var dtBirthDate1 = dtBirthDate.Value.Date;


            //   var dtDigo = dtDiagnoDate.Value.Date;

            //    if (dtBirthDate1 > date)
            //    {
            //        MessageBox.Show("Date of birth cannot be greater than current date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        dtBirthDate.Text = "";
            //        dtBirthDate.Focus();
            //    }
            //    else if (dtBirthDate1 > dtDigo)
            //    {
            //        MessageBox.Show("Date of birth cannot be greater than Diagnose date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //       // dtBirthDate.Text = "";
            //        dtBirthDate.Focus();
            //    }
            //    else
            //    {

            //        System.TimeSpan diffResult = date.Subtract(dtBirthDate1);

            //        string TotalDays = diffResult.Days.ToString();
            //        string Months = ((diffResult.Days) % 365).ToString();
            //        string RemainingMonths = (Convert.ToInt32(Months) / 31).ToString("00");
            //        string RemainginYears = ((diffResult.Days) / 365).ToString("00");
            //        string RemainingDays = (Convert.ToInt32(Months) % 31).ToString();
            //        if (Int32.Parse(RemainginYears) < 0 || Int32.Parse(RemainingDays) < 0)
            //        {
            //            MessageBox.Show("Date of birth cannot be greater than current date.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //            dtBirthDate.Text = "";
            //            dtBirthDate.Focus();
            //        }
            //        else
            //        {
            //            txtAge.Text = (RemainginYears + "." + RemainingMonths).ToString();
            //        }
            //        //txtAge.Text = age.ToString();
            //    }
            //}



        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            setval();

            string PatientName = txtPatientName.Text.ToString().Trim().Trim().Replace("'", "''");
            SqlStr = "select * from pRetPatientMast where Name='" + PatientName + "'";
            DataTable dtPatientName = new DataTable();
            dtPatientName = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (txtPatientName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Patient Name cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtPatientName.Focus();
            }
            else if ((dtPatientName.Rows.Count > 0) && this.pAddMode == true)
            {
                MessageBox.Show("Patient Name already exist.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtPatientName.Focus();
            }
            else if (txtPatientContact.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Patient Contact number cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtPatientContact.Focus();
            }
            else if (Gen == "D")
            {
                MessageBox.Show("Gender cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                rdbMale.Focus();
            }
            else if (Age.ToString() == "00.00")
            {
                MessageBox.Show("Age cannot be allowed zero.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                dtBirthDate.Focus();
            }
            else if (validation() == true)
            {
                InsertUpdate();
                this.pAddMode = false;
                this.pEditMode = false;
                //getPatCode();
                // bindControls();
                setEnableDisable(false);
                setNavigation();
                count = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getPatCode();

            bindControls();

            setEnableDisable(false);

            setNavigation();

            this.mInsertProcessIdRecord();
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
            cAppName = "udPatientMast.exe";
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
