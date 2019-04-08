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

namespace udItemTypeMast
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;
        string SqlStr, DisID;
        DataTable dtDisDet;
        string Itype, Description;
        String cAppPId, cAppName;
        int sno, first = 0, last = 0;
        DataTable dtNavigation = new DataTable();
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
            this.pFrmCaption = "Item Type Master";
            this.pCompId = Convert.ToInt16(args[0]);
            this.pComDbnm = args[1];
            this.pServerName = args[2];
            this.pUserId = args[3];
            this.pPassword = args[4];
            this.pPApplRange = args[5];
            this.pAppUerName = args[6];
         //   Icon MainIcon = new System.Drawing.Icon(args[7].Replace("<*#*>", " "));
          //  this.pFrmIcon = MainIcon;
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

        public void getDisCode()
        {
            SqlStr = "select top 1 did from pRetItemTypeMast order by Itype desc";
            DataTable dtDisCode = new DataTable();
            dtDisCode = oDataAccess.GetDataTable(SqlStr, null, 20);
            if (dtDisCode.Rows.Count > 0)
            {
                DisID = dtDisCode.Rows[0][0].ToString();
            }
        }

        private void bindControls()
        {
            SqlStr = "select * from pRetItemTypeMast where did='" + DisID + "'";
            dtDisDet = new DataTable();
            dtDisDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtDisDet.Rows.Count > 0)
            {
                txtDisName.Text = dtDisDet.Rows[0]["Itype"].ToString();
                txtDescription.Text = dtDisDet.Rows[0]["Description"].ToString();

            }
            else
            {
                txtDisName.Text = "";
                txtDescription.Text = "";

            }
        }

        public void setEnableDisable(bool val)
        {
            txtDisName.Enabled = val;
            txtDescription.Enabled = val;
            btnDisSearch.Enabled = true;


            if (val == true && pAddMode == true)
            {
                btnDisSearch.Enabled = false;
                txtDisName.Focus();
                txtDisName.Text = "";
                txtDescription.Text = "";

            }
            else if (val == true && pEditMode == true)
            {
                btnDisSearch.Enabled = false;
                txtDisName.Enabled = false;
                txtDescription.Focus();
            }
        }

        private void setNavigation()
        {
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (txtDisName.Text == "")
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

            Itype = txtDisName.Text.ToString().Trim().Replace("'", "''");
            Description = txtDescription.Text.ToString().Trim().Replace("'", "''");
        }

        private void InsertUpdate()
        {
            try
            {
                if (this.pAddMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy INSERT INTO pRetItemTypeMast(Itype,[Description])";
                    SqlStr = SqlStr + " VALUES('" + Itype + "','" + Description + "')";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();

                }
                else if (this.pEditMode == true)
                {
                    oDataAccess.BeginTransaction();
                    SqlStr = "set dateformat dmy update pRetItemTypeMast set ";
                    SqlStr = SqlStr + "Itype='" + Itype + "',[Description]='" + Description + "' where did='" + DisID + "'";
                    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.ToString());
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            mDeleteProcessIdRecord();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.pAddMode = true;
            setEnableDisable(true);
            setNavigation();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.pAddMode = false;
            this.pEditMode = false;
            getDisCode();
            bindControls();
            setEnableDisable(false);
            setNavigation();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                DataRow[] results = dtNavigation.Select("Itype = '" + txtDisName.Text.ToString().Trim() + "'");
                if (results.Length > 0)
                {
                    dtDisDet = new DataTable();
                    dtDisDet = results.CopyToDataTable();
                    if (dtDisDet.Rows.Count > 0)
                    {
                        sno = Int32.Parse(dtDisDet.Rows[0][0].ToString());
                        DisID = dtDisDet.Rows[0][1].ToString(); ;
                        //  bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }

            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.pEditMode = true;
            this.pAddMode = false;
            setEnableDisable(true);
            setNavigation();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataTable dtRec = new DataTable();
            SqlStr = "select prItemType  from it_mast where prItemType='"+txtDisName.Text+"'";
            dtRec = new DataTable();
            dtRec = oDataAccess.GetDataTable(SqlStr, null, 20);

            if(dtRec.Rows.Count>0)
            {
                MessageBox.Show("Item Type is used in Supply Master.\n Can not Delete !!!", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                if (MessageBox.Show("Are you sure you wish to delete this Record ?", this.pPApplText,
               MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        oDataAccess.BeginTransaction();
                        SqlStr = "Delete from pRetItemTypeMast where Itype='" + txtDisName.Text.ToString().Trim().Replace("'", "''") + "'";
                        oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                        oDataAccess.CommitTransaction();

                    }
                    catch (Exception ee)
                    {
                     

                    }

                    this.pAddMode = false;
                    this.pEditMode = false;
                    getDisCode();
                    bindControls();
                    setEnableDisable(false);
                    setNavigation();

                    SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
                    dtNavigation = new DataTable();
                    dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

                    if (dtNavigation.Rows.Count > 0)
                    {

                        DataRow[] results = dtNavigation.Select("Itype = '" + txtDisName.Text.ToString().Trim() + "'");
                        if (results.Length > 0)
                        {
                            dtDisDet = new DataTable();
                            dtDisDet = results.CopyToDataTable();
                            if (dtDisDet.Rows.Count > 0)
                            {
                                sno = Int32.Parse(dtDisDet.Rows[0][0].ToString());
                                DisID = dtDisDet.Rows[0][1].ToString(); ;
                                //  bindControls();
                                chkNavigation(dtNavigation, sno.ToString());
                            }
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
            // SqlStr = "select * from pRetCategory_Master where Category!=''  order by Category";
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                DataTable Temp = dtNavigation.AsEnumerable().Take(1).CopyToDataTable();
                if (Temp.Rows.Count > 0)
                {
                    sno = Int32.Parse(Temp.Rows[0][0].ToString());
                    DisID = Temp.Rows[0][1].ToString();
                    bindControls();

                    chkNavigation(dtNavigation, sno.ToString());
                }
            }

            //SqlStr = "select top 1 id from pRetItemTypeMast order by Itype";
            //dtDisDet = new DataTable();
            //dtDisDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtDisDet.Rows.Count > 0)
            //{
            //    DisID = dtDisDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            last++;
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["did"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    DisID = val;
                    bindControls();
                    chkNavigation(dtNavigation, sno.ToString());
                }
            }

            //    SqlStr = "select top 1 id from pRetItemTypeMast order by Itype desc";
            //dtDisDet = new DataTable();
            //dtDisDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtDisDet.Rows.Count > 0)
            //{
            //    DisID = dtDisDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnBack_Click(object sender, EventArgs e)
        {

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                sno = sno - 1;
                DataRow[] results = dtNavigation.Select("number = '" + sno + "'");
                if (results.Length > 0)
                {
                    dtDisDet = new DataTable();
                    dtDisDet = results.CopyToDataTable();
                    if (dtDisDet.Rows.Count > 0)
                    {
                        DisID = dtDisDet.Rows[0][1].ToString(); ;
                        bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }
                else
                {
                    sno = sno + 1;
                }
            }

            //    SqlStr = "select  id from pRetItemTypeMast where id=(select top 1 id from pRetItemTypeMast where id <(select id from pRetItemTypeMast where id='" + DisID + "') order by Itype desc )";
            //dtDisDet = new DataTable();
            //dtDisDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtDisDet.Rows.Count > 0)
            //{
            //    DisID = dtDisDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {
                sno = sno + 1;
                DataRow[] results = dtNavigation.Select("number = '" + sno + "'");
                if (results.Length > 0)
                {
                    dtDisDet = new DataTable();
                    dtDisDet = results.CopyToDataTable();
                    if (dtDisDet.Rows.Count > 0)
                    {
                        DisID = dtDisDet.Rows[0][1].ToString(); ;
                        bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }
                else
                {
                    sno = sno - 1;
                }

            }

            //SqlStr = "select  id from pRetItemTypeMast where id=(select top 1 id from pRetItemTypeMast where id >(select id from pRetItemTypeMast where id='" + DisID + "') order by Itype  )";
            //dtDisDet = new DataTable();
            //dtDisDet = oDataAccess.GetDataTable(SqlStr, null, 20);

            //if (dtDisDet.Rows.Count > 0)
            //{
            //    DisID = dtDisDet.Rows[0][0].ToString();
            //    bindControls();
            //}
        }

        private void btnDisSearch_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            SqlStr = "select did,Itype from pRetItemTypeMast order by Itype";

            tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Item Type";
            vSearchCol = "Itype";
            vDisplayColumnList = "Itype:Itype";
            vReturnCol = "Itype,did";
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
                txtDisName.Text = oSelectPop.pReturnArray[0];
                DisID = oSelectPop.pReturnArray[1];
            }

            pAddMode = false;
            pAddMode = false;

            setNavigation();
            bindControls();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                DataRow[] results = dtNavigation.Select("Itype = '" + txtDisName.Text.ToString().Trim() + "'");
                if (results.Length > 0)
                {
                    dtDisDet = new DataTable();
                    dtDisDet = results.CopyToDataTable();
                    if (dtDisDet.Rows.Count > 0)
                    {
                        sno = Int32.Parse(dtDisDet.Rows[0][0].ToString());
                        DisID = dtDisDet.Rows[0][1].ToString(); ;
                        //  bindControls();
                        chkNavigation(dtNavigation, sno.ToString());
                    }
                }

            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            string Itype = txtDisName.Text.ToString().Trim().Trim().Replace("'", "''");
            SqlStr = "select * from pRetItemTypeMast where Itype='" + Itype + "'";
            DataTable dtItype = new DataTable();
            dtItype = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (txtDisName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Item Type cannot be empty.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtDisName.Focus();
            }
            else if ((dtItype.Rows.Count > 0) && this.pAddMode == true)
            {
                MessageBox.Show("Item Type already exist.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtDisName.Focus();
            }
            else
            {
                setval();
                InsertUpdate();
                this.pAddMode = false;
                this.pEditMode = false;
                //   getDisCode();
                //  bindControls();
                setEnableDisable(false);
                setNavigation();

                SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
                dtNavigation = new DataTable();
                dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

                if (dtNavigation.Rows.Count > 0)
                {

                    DataRow[] results = dtNavigation.Select("Itype = '" + txtDisName.Text.ToString().Trim() + "'");
                    if (results.Length > 0)
                    {
                        dtDisDet = new DataTable();
                        dtDisDet = results.CopyToDataTable();
                        if (dtDisDet.Rows.Count > 0)
                        {
                            sno = Int32.Parse(dtDisDet.Rows[0][0].ToString());
                            DisID = dtDisDet.Rows[0][1].ToString(); ;
                            //  bindControls();
                            chkNavigation(dtNavigation, sno.ToString());
                        }
                    }

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getDisCode();

            bindControls();

            setEnableDisable(false);

            setNavigation();

            this.mInsertProcessIdRecord();

            SqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY Itype) number, did, Itype  FROM    pRetItemTypeMast where Itype != '' order by Itype";
            dtNavigation = new DataTable();
            dtNavigation = oDataAccess.GetDataTable(SqlStr, null, 20);

            if (dtNavigation.Rows.Count > 0)
            {

                var val = dtNavigation.Rows[dtNavigation.Rows.Count - 1]["did"].ToString();

                if (val != "")
                {
                    sno = Int32.Parse(dtNavigation.Rows[dtNavigation.Rows.Count - 1]["number"].ToString());
                    chkNavigation(dtNavigation, sno.ToString());

                }
            }
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
            cAppName = "udItemTypeMast.exe";
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
