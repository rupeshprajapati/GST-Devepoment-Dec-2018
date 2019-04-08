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

namespace udSaltMast
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        string SqlStr, SaltId;
        string Salt, Indications, Dosage, SideEffects, SpePrec, DrugInte,  Schedule, MaxRate, Continued, Prohibited;
        DataTable dtSaltDet;
        String cAppPId, cAppName;
        DataTable dtNavigation = new DataTable();
        int sno, first = 0, last = 0;
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
            this.pFrmCaption = "Generic Drug Master";
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

        private void setval()
        {
            Salt = txtSaltName.Text.ToString().Trim().Replace("'", "''"); 
            Indications = txtIndication.Text.ToString().Trim().Replace("'", "''"); 
            Dosage = txtDosage.Text.ToString().Trim().Replace("'", "''"); 
            SideEffects = txtSideEffect.Text.ToString().Trim().Replace("'", "''"); 
            SpePrec = txtSpPrecaution.Text.ToString().Trim().Replace("'", "''"); 
            DrugInte = txtDInteraction.Text.ToString().Trim().Replace("'", "''"); 
            MaxRate = txtMaxRate.Text.ToString();

            if (MaxRate.Trim() == "")
            {
                MaxRate = "0.00";
            }
            //if (chkNarco.Checked == true)
            //{
            //    Narcotic = "1";
            //}
            //else
            //{
            //    Narcotic = "0";
            //}

            if (ddlSchedule.SelectedIndex == 0)
            {
                Schedule = "H";
            }
            else if (ddlSchedule.SelectedIndex == 1)
            {
                Schedule = "H1";
            }
            else if(ddlSchedule.SelectedIndex == 2)
            {
                Schedule = "Narcotic";
            }
            //Added by Priyanka B on 22112018 for Bug-31930 Start
            else if (ddlSchedule.SelectedIndex == 3)
            {
                Schedule = "TB";
            }
            //Added by Priyanka B on 22112018 for Bug-31930 End
            else
            {
                Schedule = "";
            }
            
            if (chkContinued.Checked == true)
            {
                Continued = "1";
            }
            else
            {
                Continued = "0";
            }

            if (chkProhibited.Checked == true)
            {
                Prohibited = "1";
            }
            else
            {
                Prohibited = "0";
            }


        }

        private void InsertUpdate()
        {
            try
            {
                if (this.pAddMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy INSERT INTO pRetSalt_Master(Salt,Indications,Dosage,SideEffects,SpePrec,DrugInte,Schedule,MaxRate,Continued,Prohibited)";
                    SqlStr = SqlStr + " VALUES('" + Salt + "','" + Indications + "','" + Dosage + "','" + SideEffects + "','" + SpePrec + "','" + DrugInte + "','" + Schedule + "'," + MaxRate + "," + Continued + "," + Prohibited + ")";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                else if (this.pEditMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy update pRetSalt_Master set ";
                    SqlStr = SqlStr + "Salt='" + Salt + "',Indications='" + Indications + "',Dosage='" + Dosage + "',SideEffects='" + SideEffects + "',SpePrec='" + SpePrec + "',DrugInte='" + DrugInte + "',Schedule='" + Schedule + "',MaxRate=" + MaxRate + ",Continued=" + Continued + ",Prohibited=" + Prohibited + " where Salt_id='" + SaltId + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.ToString());
            }
        }

        public void getSaltId()
        {
            SqlStr = "select top 1 Salt_id from pRetSalt_Master order by Salt desc";
            DataTable dtPatCode = new DataTable();
            dtPatCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtPatCode.Rows.Count > 0)
            {
                SaltId = dtPatCode.Rows[0][0].ToString();
            }
        }

        private void bindControls()
        {
            ddlSchedule.SelectedIndex = 2;

            SqlStr = "select * from pRetSalt_Master where Salt_id='" + SaltId + "'";
            dtSaltDet = new DataTable();
            dtSaltDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtSaltDet.Rows.Count > 0)
            {
                txtSaltName.Text = dtSaltDet.Rows[0]["Salt"].ToString();
                txtIndication.Text = dtSaltDet.Rows[0]["Indications"].ToString();
                txtDosage.Text = dtSaltDet.Rows[0]["Dosage"].ToString();
                txtSideEffect.Text = dtSaltDet.Rows[0]["SideEffects"].ToString();
                txtSpPrecaution.Text = dtSaltDet.Rows[0]["SpePrec"].ToString();
                txtDInteraction.Text = dtSaltDet.Rows[0]["DrugInte"].ToString();
                txtMaxRate.Text = dtSaltDet.Rows[0]["MaxRate"].ToString();

                string Narcotic, Schedule, Continued, Prohibited;

               // Narcotic = dtSaltDet.Rows[0]["Narcotic"].ToString();
                Schedule = dtSaltDet.Rows[0]["Schedule"].ToString();
                Continued = dtSaltDet.Rows[0]["Continued"].ToString();
                Prohibited = dtSaltDet.Rows[0]["Prohibited"].ToString();

                //if (Narcotic == "False")
                //{
                //    chkNarco.Checked = false;
                //}
                //else
                //{
                //    chkNarco.Checked = true;
                //}

                if (Schedule == "H")
                {
                    ddlSchedule.SelectedIndex = 0;
                }
                else if (Schedule == "H1")
                {
                    ddlSchedule.SelectedIndex = 1;
                }
                else if (Schedule == "Narcotic")
                {
                    ddlSchedule.SelectedIndex = 2;
                }
                //Added by Priyanka B on 22112018 for Bug-31930 Start
                else if (Schedule == "TB")
                {
                    ddlSchedule.SelectedIndex = 3;
                }
                //Added by Priyanka B on 22112018 for Bug-31930 End
                else
                {
                    //ddlSchedule.SelectedIndex = 3;  //Commented by Priyanka B on 22112018 for Bug-31930
                    ddlSchedule.SelectedIndex = 4;  //Added by Priyanka B on 22112018 for Bug-31930
                }


                if (Continued == "False")
                {
                    chkContinued.Checked = false;
                }
                else
                {
                    chkContinued.Checked = true;
                }

                if (Prohibited == "False")
                {
                    chkProhibited.Checked = false;

                }
                else
                {
                    chkProhibited.Checked = true;

                }

            }
            else
            {
                txtSaltName.Text = "";
                txtIndication.Text = "";
                txtDosage.Text = "";
                txtSideEffect.Text = "";
                txtSpPrecaution.Text = "";
                txtDInteraction.Text = "";
                txtMaxRate.Text = "";

               // chkNarco.Checked = true;
                ddlSchedule.SelectedIndex = 2;
                chkContinued.Checked = true;
                chkProhibited.Checked = true;


            }
        }

        public void setEnableDisable(bool val)
        {
            txtSaltName.Enabled = val;
            txtIndication.Enabled = val;
            txtDosage.Enabled = val;
            txtSideEffect.Enabled = val;
            txtSpPrecaution.Enabled = val;
            txtDInteraction.Enabled = val;
            txtMaxRate.Enabled = val;
            btnSaltSearch.Enabled = true;
           // chkNarco.Enabled = val;
            ddlSchedule.Enabled = val;

            chkContinued.Enabled = val;
            chkProhibited.Enabled = val;
            btnUpload.Enabled = true;
            if (val == true && pAddMode == true)
            {
                btnUpload.Enabled = false;
                txtSaltName.Focus();
                btnSaltSearch.Enabled = false;

                txtSaltName.Text = "";
                txtIndication.Text = "";
                txtDosage.Text = "";
                txtSideEffect.Text = "";
                txtSpPrecaution.Text = "";
                txtDInteraction.Text = "";
                txtMaxRate.Text = "";

              //  chkNarco.Checked = false;
                ddlSchedule.SelectedIndex = 3;

                chkContinued.Checked = true;
                chkProhibited.Checked = false;
            }
            else if (val == true && pEditMode == true)
            {
                txtSaltName.Enabled = false;
                btnSaltSearch.Enabled = false;
                btnUpload.Enabled = false;
                txtIndication.Focus();
            }
        }

        private void setNavigation()
        {
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (txtSaltName.Text == "")
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getSaltId();

            bindControls();

            setEnableDisable(false);

            setNavigation();

            this.mInsertProcessIdRecord();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["Salt_id"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    SaltId = val;
                    bindControls();
                    chkNavigation(dtNavigation, sno.ToString());
                }
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.pAddMode = true;
            setEnableDisable(true);
            setNavigation();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            Form_upload fu = new Form_upload();
            fu.pPApplPID = this.pPApplPID;
            fu.pPara = this.pPara;
            fu.pFrmCaption = "Generic Drug Master";
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
            getSaltId();
            bindControls();
            //try
            //{
            //    Process p = new Process();
            //    p.StartInfo.FileName = Application.StartupPath+"\\udUploadSalt.exe";
            //    p.Start();
            //    btnLogout_Click(this.btnLogout, e);

            //}
            //catch(Exception ee)
            //{
            //    MessageBox.Show(""+ee.ToString());
            //}


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
            getSaltId();
            bindControls();
            setEnableDisable(false);
            setNavigation();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["Salt_id"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    SaltId = val;
                    bindControls();
                    chkNavigation(dtNavigation, sno.ToString());
                }
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
                    SqlStr = "Delete from pRetSalt_Master where Salt='" + txtSaltName.Text.ToString().Trim().Trim().Replace("'", "''") + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                catch (Exception ee)
                {
                    MessageBox.Show(" Cannot Delete !!!Transaction has generated against this Account.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }

                this.pAddMode = false;
                this.pEditMode = false;
                getSaltId();
                bindControls();
                setEnableDisable(false);
                setNavigation();


                SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
                dtNavigation = new DataTable();
                dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

                if (dtNavigation.Rows.Count > 0)
                {

                    DataRow[] results = dtNavigation.Select("Salt = '" + txtSaltName.Text.ToString().Trim() + "'");
                    if (results.Length > 0)
                    {
                        dtSaltDet = new DataTable();
                        dtSaltDet = results.CopyToDataTable();
                        if (dtSaltDet.Rows.Count > 0)
                        {
                            sno = Int32.Parse(dtSaltDet.Rows[0][0].ToString());
                            SaltId = dtSaltDet.Rows[0][1].ToString(); ;
                            //  bindControls();
                            chkNavigation(dtNavigation, sno.ToString());
                        }
                    }

                }


            }
        }

        private void chkNavigation(DataTable dt, string val)
        {
            this.btnForward.Enabled = false;
            this.btnLast.Enabled = false;
            this.btnFirst.Enabled = false;
            this.btnBack.Enabled = false;



            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (dt.Rows.Count > 0)
                {
                    string vMainFldVal = val;

                    DataRow[] result = dt.Select("number >" + val);
                    if (result.Length > 0)
                    {
                        this.btnForward.Enabled = true;
                        this.btnLast.Enabled = true;
                    }
                    DataRow[] result1 = dt.Select("number <" + val);
                    if (result1.Length > 0)
                    {
                        this.btnBack.Enabled = true;
                        this.btnFirst.Enabled = true;
                    }
                }
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            first++;
            // SqlStr = "select * from pRetSalt_Master where Category!=''  order by Category";
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                DataTable Temp = dtNavigation.AsEnumerable().Take(1).CopyToDataTable();
                if (Temp.Rows.Count > 0)
                {
                    sno = Int32.Parse(Temp.Rows[0][0].ToString());
                    SaltId = Temp.Rows[0][1].ToString();
                    bindControls();

                    chkNavigation(dtNavigation, sno.ToString());
                }
            }

            //SqlStr = "select top 1 Salt_id from pRetSalt_Master where Salt!='' order by Salt ";
            //dtSaltDet = new DataTable();
            //dtSaltDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtSaltDet.Rows.Count > 0)
            //{
            //    SaltId = dtSaltDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            last++;
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["Salt_id"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    SaltId = val;
                    bindControls();
                    chkNavigation(dtNavigation, sno.ToString());
                }
            }

            //SqlStr = "select top 1 Salt_id from pRetSalt_Master  where Salt!='' order by Salt desc";
            //dtSaltDet = new DataTable();
            //dtSaltDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtSaltDet.Rows.Count > 0)
            //{
            //    SaltId = dtSaltDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                sno = sno - 1;
                DataRow[] results = dtNavigation.Select("number = '" + sno + "'");
                if (results.Length > 0)
                {
                    dtSaltDet = new DataTable();
                    dtSaltDet = results.CopyToDataTable();
                    if (dtSaltDet.Rows.Count > 0)
                    {
                        SaltId = dtSaltDet.Rows[0][1].ToString(); ;
                        bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }
                else
                {
                    sno = sno + 1;
                }
            }
            //    SqlStr = "select  Salt_id from pRetSalt_Master where Salt!='' and Salt_id=(select top 1 Salt_id from pRetSalt_Master where Salt_id <(select Salt_id from pRetSalt_Master where Salt_id='" + SaltId + "') order by Salt desc )";
            //dtSaltDet = new DataTable();
            //dtSaltDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtSaltDet.Rows.Count > 0)
            //{
            //    SaltId = dtSaltDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                sno = sno + 1;
                DataRow[] results = dtNavigation.Select("number = '" + sno + "'");
                if (results.Length > 0)
                {
                    dtSaltDet = new DataTable();
                    dtSaltDet = results.CopyToDataTable();
                    if (dtSaltDet.Rows.Count > 0)
                    {
                        SaltId = dtSaltDet.Rows[0][1].ToString(); ;
                        bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }
                else
                {
                    sno = sno - 1;
                }

            }

            //SqlStr = "select  Salt_id from pRetSalt_Master where  Salt!='' and Salt_id=(select top 1 Salt_id from pRetSalt_Master where Salt_id >(select Salt_id from pRetSalt_Master where Salt_id='" + SaltId + "') order by Salt  )";
            //dtSaltDet = new DataTable();
            //dtSaltDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtSaltDet.Rows.Count > 0)
            //{
            //    SaltId = dtSaltDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnSaltSearch_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select Salt_id,Salt from pRetSalt_Master where Salt!='' order by Salt";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Generic Name ";
            vSearchCol = "Salt";
            vDisplayColumnList = "Salt:Generic Name";
            vReturnCol = "Salt,Salt_id";
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
                txtSaltName.Text = oSelectPop.pReturnArray[0];
                SaltId = oSelectPop.pReturnArray[1];
            }

            pAddMode = false;
            pAddMode = false;
            setNavigation();
            bindControls();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                DataRow[] results = dtNavigation.Select("Salt = '" + txtSaltName.Text.ToString().Trim().Trim().Replace("'", "''") + "'");
                if (results.Length > 0)
                {
                    dtSaltDet = new DataTable();
                    dtSaltDet = results.CopyToDataTable();
                    if (dtSaltDet.Rows.Count > 0)
                    {
                        sno = Int32.Parse(dtSaltDet.Rows[0][0].ToString());
                        SaltId = dtSaltDet.Rows[0][1].ToString(); ;
                        //  bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }

            }


        }

        private void txtMaxRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
                e.Handled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string salt = txtSaltName.Text.ToString().Trim().Trim().Replace("'", "''");
            SqlStr = "select * from pRetSalt_Master where Salt='" + salt + "'";
            DataTable dtSalt = new DataTable();
            dtSalt = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (txtSaltName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Generic Name cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtSaltName.Focus();
            }
            else if ((dtSalt.Rows.Count > 0) && this.pAddMode == true)
            {
                MessageBox.Show("Generic Name already exist.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtSaltName.Focus();
            }
            else
            {
                setval();
                InsertUpdate();
                this.pAddMode = false;
                this.pEditMode = false;
                //getSaltId();
               // bindControls();
                setEnableDisable(false);
                setNavigation();


                SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Salt) number, Salt_id, Salt  FROM    pRetSalt_Master where Salt != '' order by Salt";
                dtNavigation = new DataTable();
                dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

                if (dtNavigation.Rows.Count > 0)
                {

                    DataRow[] results = dtNavigation.Select("Salt = '" + txtSaltName.Text.ToString().Trim().Trim().Replace("'", "''") + "'");
                    if (results.Length > 0)
                    {
                        dtSaltDet = new DataTable();
                        dtSaltDet = results.CopyToDataTable();
                        if (dtSaltDet.Rows.Count > 0)
                        {
                            sno = Int32.Parse(dtSaltDet.Rows[0][0].ToString());
                            SaltId = dtSaltDet.Rows[0][1].ToString(); ;
                            //  bindControls();
                            chkNavigation(dtNavigation, sno.ToString());
                        }
                    }

                }

            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            mDeleteProcessIdRecord();
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
            cAppName = "udSaltMast.exe";
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
