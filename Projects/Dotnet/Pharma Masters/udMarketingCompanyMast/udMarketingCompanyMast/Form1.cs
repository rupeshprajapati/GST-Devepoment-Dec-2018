using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udMarketingCompanyMast
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        string SqlStr, ManufacturerID;
        DataTable dtManufacturerDet;
        String cAppPId, cAppName;

        DataTable dtNavigation = new DataTable();
        int sno, first = 0, last = 0;

        string mName, contact, designation, add1, add2, add3, Area, Zone, CITY, District, state, stateCode, zip, country, office, fax, phoner, cellno, email, prDrugLicNo, prFoodLicNo, marketedby, prDrugExpDate, prFoodExpDate,mcode;
        int City_id, State_id, Country_id;




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
            this.pFrmCaption = "Marketing Company Master";
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

        private void Form1_Load(object sender, EventArgs e)
        {

            getManufacturerCode();
            bindControls();
            setEnableDisable(false);
            setNavigation();
            this.mInsertProcessIdRecord();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["mid"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    chkNavigation(dtNavigation, sno.ToString());

                }
            }

        }



        private void btnNew_Click(object sender, EventArgs e)
        {


            this.pAddMode = true;
            setEnableDisable(true);
            setNavigation();
            txtCountry.Text = "India";
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
            getManufacturerCode();
            bindControls();
            setEnableDisable(false);
            setNavigation();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["mid"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    chkNavigation(dtNavigation, sno.ToString());

                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SqlStr= " select prMarketedBy from it_mast where prMarketedBy = '" + txtName.Text.ToString().Trim().Replace("'", "''") + "'";

           // SqlStr = "select prManufacturer from it_mast where prManufacturer='" + txtName.Text.ToString().Trim().Replace("'", "''") + "'";
            DataTable dtManufacturer = new DataTable();
            dtManufacturer = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtManufacturer.Rows.Count == 0)
            {
                if (MessageBox.Show("Are you sure you wish to delete this Record ?", this.pPApplText,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    try
                    {
                        oDataAccess.BeginTransaction();
                        SqlStr = "Delete from pRetDrugMarketingMast where Name='" + txtName.Text.ToString().Trim().Replace("'", "''") + "'";
                        oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                        oDataAccess.CommitTransaction();

                    }
                    catch (Exception ee)
                    {
                    }
                }
            }
            else
            {
                MessageBox.Show(" You cannot delete this Marketing Company , It is already used in Supply Master.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }


            this.pAddMode = false;
            this.pEditMode = false;
            getManufacturerCode();
            bindControls();
            setEnableDisable(false);
            setNavigation();
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                DataRow[] results = dtNavigation.Select("name = '" + txtName.Text.ToString().Trim() + "'");
                if (results.Length > 0)
                {
                    dtManufacturerDet = new DataTable();
                    dtManufacturerDet = results.CopyToDataTable();
                    if (dtManufacturerDet.Rows.Count > 0)
                    {
                        sno = Int32.Parse(dtManufacturerDet.Rows[0][0].ToString());
                        ManufacturerID = dtManufacturerDet.Rows[0][1].ToString(); ;
                        //  bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }

            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.ToString().Trim().Trim().Replace("'", "''");
            SqlStr = "select * from pRetDrugMarketingMast where Name='" + name + "'";
            DataTable dtManufacturer = new DataTable();
            dtManufacturer = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (txtName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Marketing Company Name cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtName.Focus();
            }
            else if ((dtManufacturer.Rows.Count > 0) && this.pAddMode == true)
            {
                MessageBox.Show("Marketing Company already exist.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtName.Focus();
            }
            else
            {
                setval();
                InsertUpdate();
                this.pAddMode = false;
                this.pEditMode = false;


                setEnableDisable(false);
                setNavigation();

                SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
                dtNavigation = new DataTable();
                dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

                if (dtNavigation.Rows.Count > 0)
                {

                    DataRow[] results = dtNavigation.Select("name = '" + txtName.Text.ToString().Trim().Replace("'", "''") + "'");
                    if (results.Length > 0)
                    {
                        dtManufacturerDet = new DataTable();
                        dtManufacturerDet = results.CopyToDataTable();
                        if (dtManufacturerDet.Rows.Count > 0)
                        {
                            sno = Int32.Parse(dtManufacturerDet.Rows[0][0].ToString());
                            ManufacturerID = dtManufacturerDet.Rows[0][1].ToString(); ;
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



        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (txtEmail.Text != "")
            {
                Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                if (!reg.IsMatch(txtEmail.Text))
                {
                    MessageBox.Show("Email id is not Proper.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtEmail.Focus();
                }
            }
        }

        private void txtOffice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtMarketedBy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F2)
            {
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode.ToString() == "F2")
            {
                string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                DataSet tDs = new DataSet();
                SqlStr = "select Name from pRetDrugMarketingMast order by Name";

                tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
                DataView dvw = tDs.Tables[0].DefaultView;
                VForText = "Select Marketed By";
                vSearchCol = "Name";
                vDisplayColumnList = "Name:Marketed By";
                vReturnCol = "Name";
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
                    txtMarketedBy.Text = oSelectPop.pReturnArray[0];
                   
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select Name from pRetDrugMarketingMast order by Name";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Marketed By";
            vSearchCol = "Name";
            vDisplayColumnList = "Name:Marketed By";
            vReturnCol = "Name";
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
                txtMarketedBy.Text = oSelectPop.pReturnArray[0];

            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtName.Text.Length >= 3 && txtCode.Text.Length<=3)
            {
                txtCode.Text = txtName.Text.ToString().Substring(0, 3).ToUpper();
            }           
        }

        private void txtResi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtState_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F2")
            {
                string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                DataSet tDs = new DataSet();
                SqlStr = "Select distinct city from city where city is not null AND City <> '' Order By City";

                tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
                DataView dvw = tDs.Tables[0].DefaultView;
                VForText = "Select city";
                vSearchCol = "city";
                vDisplayColumnList = "city:City";
                vReturnCol = "city";
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
                    txtCity.Text = oSelectPop.pReturnArray[0];
                }
            }
        }

        private void txtState_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F2)
            {
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode.ToString() == "F2")
            {
                string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                DataSet tDs = new DataSet();
                SqlStr = "Select state,gst_state_code as scode from state where state !='' order by state";

                tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
                DataView dvw = tDs.Tables[0].DefaultView;
                VForText = "Select State";
                vSearchCol = "state";
                vDisplayColumnList = "state:State,scode:State Code";
                vReturnCol = "state,scode";
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
                    txtState.Text = oSelectPop.pReturnArray[0];
                    txtStateCode.Text = oSelectPop.pReturnArray[1];
                }
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            Form_upload fu = new Form_upload();
            fu.pPApplPID = this.pPApplPID;
            fu.pPara = this.pPara;
            fu.pFrmCaption = "Upload Marketing Company Master";
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
            getManufacturerCode();
            bindControls();

           
            setEnableDisable(false);
            setNavigation();


            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["mid"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    chkNavigation(dtNavigation, sno.ToString());

                }
            }
        }

        private void txtCountry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F2)
            {
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode.ToString() == "F2")
            {
                string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                DataSet tDs = new DataSet();
                SqlStr = "Select Country from Country where Country is not null and Country <> '' order by Country";

                tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
                DataView dvw = tDs.Tables[0].DefaultView;
                VForText = "Select Country";
                vSearchCol = "Country";
                vDisplayColumnList = "Country:Country";
                vReturnCol = "Country";
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
                    txtCountry.Text = oSelectPop.pReturnArray[0];

                }
            }
        }

        private void txtCell_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F2")
            {
                string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                DataSet tDs = new DataSet();
                SqlStr = "Select Distinct Area From pRetDrugMarketingMast Where Area <> ''";

                tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
                DataView dvw = tDs.Tables[0].DefaultView;
                VForText = "Select Area";
                vSearchCol = "Area";
                vDisplayColumnList = "Area:Area";
                vReturnCol = "Area";
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
                    txtArea.Text = oSelectPop.pReturnArray[0];
                    txtZone.Focus();
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
            // SqlStr = "select * from pRetCategory_Master where Category!=''  order by Category";
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                DataTable Temp = dtNavigation.AsEnumerable().Take(1).CopyToDataTable();
                if (Temp.Rows.Count > 0)
                {
                    sno = Int32.Parse(Temp.Rows[0][0].ToString());
                    ManufacturerID = Temp.Rows[0][1].ToString();
                    bindControls();

                    chkNavigation(dtNavigation, sno.ToString());
                }
            }


            //SqlStr = "select top 1 mid from pRetDrugMarketingMast order by Name";
            //dtManufacturerDet = new DataTable();
            //dtManufacturerDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtManufacturerDet.Rows.Count > 0)
            //{
            //    ManufacturerID = dtManufacturerDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                sno = sno - 1;
                DataRow[] results = dtNavigation.Select("number = '" + sno + "'");
                if (results.Length > 0)
                {
                    dtManufacturerDet = new DataTable();
                    dtManufacturerDet = results.CopyToDataTable();
                    if (dtManufacturerDet.Rows.Count > 0)
                    {
                        ManufacturerID = dtManufacturerDet.Rows[0][1].ToString(); ;
                        bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }
                else
                {
                    sno = sno + 1;
                }
            }

            //    SqlStr = "select  mid from pRetDrugMarketingMast where mid=(select top 1 mid from pRetDrugMarketingMast where mid <(select mid from pRetDrugMarketingMast where mid='" + ManufacturerID + "') order by Name desc )";
            //dtManufacturerDet = new DataTable();
            //dtManufacturerDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtManufacturerDet.Rows.Count > 0)
            //{
            //    ManufacturerID = dtManufacturerDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                sno = sno + 1;
                DataRow[] results = dtNavigation.Select("number = '" + sno + "'");
                if (results.Length > 0)
                {
                    dtManufacturerDet = new DataTable();
                    dtManufacturerDet = results.CopyToDataTable();
                    if (dtManufacturerDet.Rows.Count > 0)
                    {
                        ManufacturerID = dtManufacturerDet.Rows[0][1].ToString(); ;
                        bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }
                else
                {
                    sno = sno - 1;
                }

            }

            //SqlStr = "select  mid from pRetDrugMarketingMast where mid=(select top 1 mid from pRetDrugMarketingMast where mid >(select mid from pRetDrugMarketingMast where mid='" + ManufacturerID + "') order by Name  )";
            //dtManufacturerDet = new DataTable();
            //dtManufacturerDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtManufacturerDet.Rows.Count > 0)
            //{
            //    ManufacturerID = dtManufacturerDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            last++;
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["mid"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    ManufacturerID = val;
                    bindControls();
                    chkNavigation(dtNavigation, sno.ToString());
                }
            }

            //SqlStr = "select top 1 mid from pRetDrugMarketingMast order by Name desc";
            //dtManufacturerDet = new DataTable();
            //dtManufacturerDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtManufacturerDet.Rows.Count > 0)
            //{
            //    ManufacturerID = dtManufacturerDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }


        private void btnName_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select mid,Name from pRetDrugMarketingMast order by Name";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Marketing Company Name ";
            vSearchCol = "Name";
            vDisplayColumnList = "Name:Name";
            vReturnCol = "Name,mid";
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
                txtName.Text = oSelectPop.pReturnArray[0];
                ManufacturerID = oSelectPop.pReturnArray[1];
            }

            pAddMode = false;
            pAddMode = false;
            setNavigation();
            bindControls();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY name) number, mid, name  FROM    pRetDrugMarketingMast where name != '' order by name";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                DataRow[] results = dtNavigation.Select("name = '" + txtName.Text.ToString().Trim().Replace("'", "''") + "'");
                if (results.Length > 0)
                {
                    dtManufacturerDet = new DataTable();
                    dtManufacturerDet = results.CopyToDataTable();
                    if (dtManufacturerDet.Rows.Count > 0)
                    {
                        sno = Int32.Parse(dtManufacturerDet.Rows[0][0].ToString());
                        ManufacturerID = dtManufacturerDet.Rows[0][1].ToString(); ;
                        //  bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }

            }


        }

        private void btnArea_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "Select Distinct Area From pRetDrugMarketingMast Where Area <> ''";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Area";
            vSearchCol = "Area";
            vDisplayColumnList = "Area:Area";
            vReturnCol = "Area";
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
                txtArea.Text = oSelectPop.pReturnArray[0];
            }
        }

        private void btnCity_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "Select distinct city from city where city is not null AND City <> '' Order By City";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select city";
            vSearchCol = "city";
            vDisplayColumnList = "city:City";
            vReturnCol = "city";
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
                txtCity.Text = oSelectPop.pReturnArray[0];
            }
        }

        private void btnState_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "Select state,gst_state_code as scode from state where state !='' order by state";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select State";
            vSearchCol = "state";
            vDisplayColumnList = "state:State,scode:State Code";
            vReturnCol = "state,scode";
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
                txtState.Text = oSelectPop.pReturnArray[0];
                txtStateCode.Text = oSelectPop.pReturnArray[1];
            }
        }

        private void btnCountry_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "Select Country from Country where Country is not null and Country <> '' order by Country";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Country";
            vSearchCol = "Country";
            vDisplayColumnList = "Country:Country";
            vReturnCol = "Country";
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
                txtCountry.Text = oSelectPop.pReturnArray[0];

            }
        }



        private void mInsertProcessIdRecord()
        {
            DataSet dsData = new DataSet();
            string sqlstr;
            int pi;
            pi = Process.GetCurrentProcess().Id;
            cAppName = "udMarketingCompanyMast.exe";
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



        public void getManufacturerCode()
        {
            SqlStr = "select top 1 mid from pRetDrugMarketingMast order by Name desc";
            DataTable dtManufacturerCode = new DataTable();
            dtManufacturerCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtManufacturerCode.Rows.Count > 0)
            {
                ManufacturerID = dtManufacturerCode.Rows[0][0].ToString();
            }
        }

        private void bindControls()
        {
            SqlStr = "select * from pRetDrugMarketingMast where mid='" + ManufacturerID + "'";
            dtManufacturerDet = new DataTable();
            dtManufacturerDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtManufacturerDet.Rows.Count > 0)
            {
                txtName.Text = dtManufacturerDet.Rows[0]["Name"].ToString();
                txtCode.Text = dtManufacturerDet.Rows[0]["mcode"].ToString();
                txtContactPerson.Text = dtManufacturerDet.Rows[0]["contact"].ToString();
                txtDesignation.Text = dtManufacturerDet.Rows[0]["designatio"].ToString();
                txtAdd1.Text = dtManufacturerDet.Rows[0]["add1"].ToString();
                txtAdd2.Text = dtManufacturerDet.Rows[0]["add2"].ToString();
                txtAdd3.Text = dtManufacturerDet.Rows[0]["add3"].ToString();
                txtArea.Text = dtManufacturerDet.Rows[0]["Area"].ToString();
                txtZone.Text = dtManufacturerDet.Rows[0]["Zone"].ToString();
                txtCity.Text = dtManufacturerDet.Rows[0]["CITY"].ToString();
                txtDist.Text = dtManufacturerDet.Rows[0]["District"].ToString();
                txtState.Text = dtManufacturerDet.Rows[0]["state"].ToString();
                txtZip.Text = dtManufacturerDet.Rows[0]["zip"].ToString();
                txtStateCode.Text = dtManufacturerDet.Rows[0]["stateCode"].ToString();
                txtCountry.Text = dtManufacturerDet.Rows[0]["country"].ToString();
                txtOffice.Text = dtManufacturerDet.Rows[0]["phone"].ToString();
                txtFax.Text = dtManufacturerDet.Rows[0]["fax"].ToString();
                txtResi.Text = dtManufacturerDet.Rows[0]["phoner"].ToString();
                txtCell.Text = dtManufacturerDet.Rows[0]["mobile"].ToString();
                txtEmail.Text = dtManufacturerDet.Rows[0]["email"].ToString();


                txtDrugLicNo.Text = dtManufacturerDet.Rows[0]["prDrugLicNo"].ToString();
                txtFSSAINo.Text = dtManufacturerDet.Rows[0]["prFoodLicNo"].ToString();
                dtDrugLicExp.Text = dtManufacturerDet.Rows[0]["prDrugExpDate"].ToString();
                dtFSSAIExp.Text = dtManufacturerDet.Rows[0]["prFoodExpDate"].ToString();
               // txtMarketedBy.Text = dtManufacturerDet.Rows[0]["marketedby"].ToString();


            }
            else
            {
                txtName.Text = "";
                txtCode.Text = "";
                txtContactPerson.Text = "";
                txtDesignation.Text = "";
                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";
                txtArea.Text = "";
                txtZone.Text = "";
                txtCity.Text = "";
                txtDist.Text = "";
                txtState.Text = "";
                txtZip.Text = "";
                txtStateCode.Text = "";
                txtCountry.Text = "";
                txtOffice.Text = "";
                txtFax.Text = "";
                txtResi.Text = "";
                txtCell.Text = "";
                txtEmail.Text = "";

                txtDrugLicNo.Text = "";
                txtFSSAINo.Text = "";
                dtDrugLicExp.Text = "";
                dtFSSAIExp.Text = "";
                txtMarketedBy.Text = "";


            }
        }

        public void setEnableDisable(bool val)
        {
            txtName.Enabled = val;
            txtCode.Enabled = val;
            txtContactPerson.Enabled = val;
            txtDesignation.Enabled = val;
            txtAdd1.Enabled = val;
            txtAdd2.Enabled = val;
            txtAdd3.Enabled = val;
            txtArea.Enabled = val;
            txtZone.Enabled = val;
            txtCity.Enabled = val;
            txtDist.Enabled = val;
            txtState.Enabled = val;
            txtZip.Enabled = val;
            txtStateCode.Enabled = false;
            txtCountry.Enabled = val;
            txtOffice.Enabled = val;
            txtFax.Enabled = val;
            txtResi.Enabled = val;
            txtCell.Enabled = val;
            txtEmail.Enabled = val;

            txtDrugLicNo.Enabled = val;
            txtFSSAINo.Enabled = val;
            dtDrugLicExp.Enabled = val;
            dtFSSAIExp.Enabled = val;
            txtMarketedBy.Enabled = val;

            btnName.Enabled = true;
            btnArea.Enabled = val;
            btnCity.Enabled = val;
            btnState.Enabled = val;
            btnCountry.Enabled = val;
            button1.Enabled = val;
            btnUpload.Enabled = false;


            if (val == true && pAddMode == true)
            {
                btnName.Enabled = false;
                txtName.Focus();

                txtName.Text = "";
                txtCode.Text = "";
                txtContactPerson.Text = "";
                txtDesignation.Text = "";
                txtAdd1.Text = "";
                txtAdd2.Text = "";
                txtAdd3.Text = "";
                txtArea.Text = "";
                txtZone.Text = "";
                txtCity.Text = "";
                txtDist.Text = "";
                txtState.Text = "";
                txtZip.Text = "";
                txtStateCode.Text = "";
                txtCountry.Text = "";
                txtOffice.Text = "";
                txtFax.Text = "";
                txtResi.Text = "";
                txtCell.Text = "";
                txtEmail.Text = "";

                txtDrugLicNo.Text = "";
                txtFSSAINo.Text = "";
                dtDrugLicExp.Text = "";
                dtFSSAIExp.Text = "";
                txtMarketedBy.Text = "";

                btnUpload.Enabled = true;

            }
            else if (val == true && pEditMode == true)
            {
                btnName.Enabled = false;
                txtName.Enabled = false;
                txtName.Focus();
            }
        }

        private void setNavigation()
        {
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (txtName.Text == "")
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



        private void setval()
        {

            mName = txtName.Text.ToString().Trim().Replace("'", "''");
            mcode=txtCode.Text.ToString().Trim().Replace("'", "''");
            contact = txtContactPerson.Text.ToString().Trim();
            designation = txtDesignation.Text.ToString().Trim();
            add1 = txtAdd1.Text.ToString().Trim().Replace("'", "''");
            add2 = txtAdd2.Text.ToString().Trim().Replace("'", "''");
            add3 = txtAdd3.Text.ToString().Trim().Replace("'", "''");
            Area = txtArea.Text.ToString().Trim().Replace("'", "''");
            Zone = txtZone.Text.ToString().Trim().Replace("'", "''");
            CITY = txtCity.Text.ToString().Trim().Replace("'", "''");
            City_id = getCityDetails(CITY);
            District = txtDist.Text.ToString().Trim().Replace("'", "''");
            state = txtState.Text.ToString().Trim().Replace("'", "''");
            stateCode = txtStateCode.Text.ToString().Trim().Replace("'", "''");
            State_id = getStateDetails(stateCode);
            zip = txtZip.Text.ToString().Trim().Replace("'", "''");
            country = txtCountry.Text.ToString().Trim().Replace("'", "''");
            Country_id = getCountryDetails(country);
            office = txtOffice.Text.ToString().Trim();
            fax = txtFax.Text.ToString().Trim().Replace("'", "''");
            phoner = txtResi.Text.ToString().Trim();
            cellno = txtCell.Text.ToString().Trim();
            email = txtEmail.Text.ToString().Trim().Replace("'", "''");

            prDrugLicNo = txtDrugLicNo.Text.Trim().Replace("'", "''");
            prDrugExpDate = DateTime.Parse(dtDrugLicExp.Text).ToString("dd/MM/yyyy");
            prFoodLicNo = txtFSSAINo.Text.Trim().Replace("'", "''");
            prFoodExpDate = DateTime.Parse(dtFSSAIExp.Text).ToString("dd/MM/yyyy");
            marketedby = txtMarketedBy.Text.Trim().Replace("'", "''");

        }

        public void InsertUpdate()
        {
            try
            {
                if (this.pAddMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy INSERT INTO pRetDrugMarketingMast(Name,contact,designatio,add1,add2,add3";
                    SqlStr = SqlStr + ", Area, Zone, city, City_id, District,[state], StateCode, State_id, zip, country, Country_id, phone, fax, phoner, mobile, email,prDrugLicNo,prDrugExpDate,prFoodLicNo,prFoodExpDate,mcode)";
                    SqlStr = SqlStr + " VALUES('" + mName + "', '" + contact + "', '" + designation + "' ";
                    SqlStr = SqlStr + ", '" + add1 + "', '" + add2 + "', '" + add3 + "', '" + Area + "', '" + Zone + "', '" + CITY + "', " + City_id + ", '" + District + "', '" + state + "' ";
                    SqlStr = SqlStr + ", '" + stateCode + "', " + State_id + ", '" + zip + "', '" + country + "', " + Country_id + ", '" + office + "', '" + fax + "', '" + phoner + "', '" + cellno + "', '" + email + "','" + prDrugLicNo + "','" + prDrugExpDate + "','" + prFoodLicNo + "','" + prFoodExpDate + "','"+mcode+"')";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                else if (this.pEditMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy update pRetDrugMarketingMast set ";
                    SqlStr = SqlStr + "Name = '" + mName + "',";
                    SqlStr = SqlStr + "contact = '" + contact + "', designatio = '" + designation + "', add1 = '" + add1 + "', add2 = '" + add2 + "', add3 = '" + add3 + "',";
                    SqlStr = SqlStr + "Area = '" + Area + "', Zone = '" + Zone + "', city = '" + CITY + "', City_id = " + City_id + ", District = '" + District + "',[state] = '" + state + "',";
                    SqlStr = SqlStr + "StateCode = '" + stateCode + "', State_id = " + State_id + ", zip = '" + zip + "', country = '" + country + "', Country_id = " + Country_id + ",";
                    SqlStr = SqlStr + "phone = '" + office + "', fax = '" + fax + "', phoner = '" + phoner + "', mobile = '" + cellno + "', email = '" + email + "',prDrugLicNo='" + prDrugLicNo + "',prDrugExpDate='" + prDrugExpDate + "',prFoodLicNo='" + prFoodLicNo + "',prFoodExpDate='" + prFoodExpDate + "',mcode='"+mcode+"'";
                    SqlStr = SqlStr + "where mid = '" + ManufacturerID + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
            }
            catch (Exception ex)
            {

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

    }
}
