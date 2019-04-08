using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using DataAccess_Net;
using udSelectPop;

namespace PointofSale
{
    public partial class udFrmPOSLoginSettings : Form
    {
        //******* Form Properties - Start *******\\
        clsDataAccess _oDataAccess;
        string cAppName = "", cAppPId = "";
        //***** Added by Sachin N. S. on 14/11/2018 for Pharma Retailer Changes -- Start
        DataRow _drPosSettings;
        //***** Added by Sachin N. S. on 14/11/2018 for Pharma Retailer Changes -- End
        //******* Form Properties - End *******\\

        public udFrmPOSLoginSettings(string[] _args)
        {
            CultureInfo ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            clsDataAccess._serverName = _args[2].ToString();
            clsDataAccess._databaseName = _args[1].ToString();
            clsDataAccess._userID = _args[3].ToString();
            clsDataAccess._password = _args[4].ToString();

            CommonInfo.Server = _args[2].ToString();
            CommonInfo.DbName = _args[1].ToString();
            CommonInfo.Uid = _args[3].ToString();
            CommonInfo.Pwd = _args[4].ToString();
            CommonInfo.UserName = _args[6].ToString();
            CommonInfo.IconPath = _args[7].ToString();
            CommonInfo.AppCaption = _args[8].ToString().Replace("<*#*>", " ").ToString();
            CommonInfo.StartDate = Convert.ToDateTime(_args[13].ToString().Replace("<*#*>", " ")).ToString("dd/MM/yyyy hh:mm:ss");
            CommonInfo.EndDate = Convert.ToDateTime(_args[14].ToString().Replace("<*#*>", " ")).ToString("dd/MM/yyyy hh:mm:ss");
            CommonInfo.L_yn = Convert.ToDateTime(CommonInfo.StartDate).Year.ToString() + "-" + Convert.ToDateTime(CommonInfo.EndDate).Year.ToString();
            CommonInfo.CompId = Convert.ToInt64(_args[0].ToString());
            CommonInfo.ApplName = _args[9];
            CommonInfo.ApplPId = Convert.ToInt64(_args[10]);
            CommonInfo.ApplCode = _args[11].ToString();
            CommonInfo.Neg_itBal = Convert.ToInt16((_args[15].ToString() == ".T." ? 1 : 0));       // Added by Sachin N. S. on 14/01/2016 for Bug-27503
            //**** Added by Sachin N. S. on 26/09/2018 for New Pharma Retailer Changes -- Start
            CommonInfo.EntryTy = _args[16].ToString();
            CommonInfo.ScreenCaption = _args[17].ToString().Replace("<*#*>", " ");

            _oDataAccess = new clsDataAccess();
            string cSql = "";
            DataRow _dr1;

            cSql = "select Top 1 State, Dir_nm, Logo from Vudyog..Co_mast where CompID = " + CommonInfo.CompId.ToString() + " ";
            _dr1 = _oDataAccess.GetDataRow(cSql, null, 50);
            CommonInfo.StateName = _dr1["State"].ToString();
            CommonInfo.Dir_nm = _dr1["Dir_Nm"].ToString();
            CommonInfo.CompImg = _dr1["Logo"].ToString();

            cSql = "select Top 1 gst_state_code from [state] where [state] = '" + CommonInfo.StateName.ToString() + "' ";
            _dr1 = _oDataAccess.GetDataRow(cSql, null, 50);
            CommonInfo.StateCode = _dr1["gst_state_code"].ToString();
            
            //**** Added by Sachin N. S. on 26/09/2018 for New Pharma Retailer Changes -- End

            InitializeComponent();
        }

        private void udFrmPOSLoginSettings_Load(object sender, EventArgs e)
        {
            this.txtLoggedUser.Text = CommonInfo.UserName;
            this.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));

            this.mInsertProcessIdRecord();

            //***** Added by Sachin N. S. on 14/11/2018 for Pharma Retailer Changes -- Start
            this._InitialSettings();
            this.enableDisableControls();
            //***** Added by Sachin N. S. on 14/11/2018 for Pharma Retailer Changes -- End
        }

        private void udFrmPOSLoginSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.mDeleteProcessIdRecord();
        }

        private void cmdContinue_Click(object sender, EventArgs e)
        {
            if (this.txtParty.Text == string.Empty)
            {
                MessageBox.Show("Party cannot be empty.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //****** Added by Sachin N. S. on 04/02/2016 for Bug-27503 -- Start
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            DataRow _dr;

            cSql = "select Inv_sr from series where validity like '%" + CommonInfo.EntryTy + "%' ";  // Changed by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes
            _dr = _oDataAccess.GetDataRow(cSql, null, 50);
            if (_dr != null && this.txtInvSeries.Text == "")
            {
                MessageBox.Show("Please select a valid Invoice Series.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            cSql = "select Dept from Department where validity like '%" + CommonInfo.EntryTy + "%' ";    // Changed by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes
            _dr = _oDataAccess.GetDataRow(cSql, null, 50);
            if (_dr != null && this.txtDepartment.Text == "")
            {
                MessageBox.Show("Please select a valid Department.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            cSql = "select Cate from Category where validity like '%" + CommonInfo.EntryTy + "%' ";     // Changed by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes
            _dr = _oDataAccess.GetDataRow(cSql, null, 50);
            if (_dr != null && this.txtCategory.Text == "")
            {
                MessageBox.Show("Please select a valid Category.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //cSql = "Select lCinMndtry from lcode where entry_ty='PS' ";
            if (Convert.ToBoolean(_drPosSettings["lCInMndtry"]) == false)
            {
                if (this.txtCounter.Text == string.Empty)
                {
                    MessageBox.Show("Please select valid Counter.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (CommonInfo.CashInNo == 0)
                {
                    MessageBox.Show("Cash-In entry not done against the '" + CommonInfo.UserName + "' user and '" + CommonInfo.CounterNm + "' counter.\nCannot continue...!!!", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            //****** Added by Sachin N. S. on 04/02/2016 for Bug-27503 -- End
            this.Hide();
            //udFrmPointofSale _frmPos = new udFrmPointofSale();
            udFrmPointofSaleNew _frmPos = new udFrmPointofSaleNew();
            _frmPos.ShowDialog();
            this.Close();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdInvSeries_Click(object sender, EventArgs e)
        {
            getInvSr_Dept_Cate("SERIES");
        }

        private void btnDepartment_Click(object sender, EventArgs e)
        {
            getInvSr_Dept_Cate("DEPARTMENT");
        }

        private void cmdCategory_Click(object sender, EventArgs e)
        {
            getInvSr_Dept_Cate("CATEGORY");
        }

        //****** Added by Sachin N. S. on 22/05/2013 for Bug-14538 -- Start ******//
        private void cmdParty_Click(object sender, EventArgs e)
        {
            getInvSr_Dept_Cate("PARTY");
        }

        private void cmdWarehouse_Click(object sender, EventArgs e)
        {
            getInvSr_Dept_Cate("WAREHOUSE");
        }

        private void txtParty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                this.cmdParty.PerformClick();
            }
        }

        private void txtWareHouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                this.cmdWarehouse.PerformClick();
            }
        }

        //****** Added by Sachin N. S. on 22/05/2013 for Bug-14538 -- End ******//

        //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- Start ******//
        private void cmdCounter_Click(object sender, EventArgs e)
        {
            getInvSr_Dept_Cate("COUNTER");
        }

        private void txtCounter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                this.cmdCounter.PerformClick();
            }
        }
        //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- End ******//

        //******* Calling Select Popup for Invoice Series, Department & Category
        private void getInvSr_Dept_Cate(string _popupType)
        {
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            string cFrmCap = "", cSrchCol = "", cDispCol = "", cRetCol = "";
            DataTable _dt = new DataTable();
            udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
            switch (_popupType)
            {
                case "SERIES":
                    cSql = "select Inv_sr from series where validity like '%" + CommonInfo.EntryTy + "%' ";     // Changed by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes
                    cFrmCap = "Select Invoice Series";
                    cSrchCol = "Inv_Sr";
                    cDispCol = "Inv_Sr:Invoice Series";
                    cRetCol = "Inv_Sr";
                    break;
                case "DEPARTMENT":
                    cSql = "select Dept from Department where validity like '%" + CommonInfo.EntryTy + "%' ";     // Changed by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes
                    cFrmCap = "Select Department";
                    cSrchCol = "Dept";
                    cDispCol = "Dept:Department";
                    cRetCol = "Dept";
                    break;
                case "CATEGORY":
                    cSql = "select Cate from Category where validity like '%" + CommonInfo.EntryTy + "%' ";     // Changed by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes
                    cFrmCap = "Select Category";
                    cSrchCol = "Cate";
                    cDispCol = "Cate:Category";
                    cRetCol = "Cate";
                    break;
                //***** Added by Sachin N. S. on 23/05/2013 for Bug-14538 -- Start *****//
                case "PARTY":
                    string xcFldList = "";
                    string xcDispCol = "";
                    string xcRetCol = "", xcSrchCol = "";
                    string cTbl = " Ac_Mast ";
                    string cCond = " Ac_Id!=0 ";

                    //DataRow _dr = _oDataAccess.GetDataRow("Select Ac_fields from lcode where entry_ty='PS'", null, 25);
                    DataRow _dr = _oDataAccess.GetDataRow("Select Ac_fields from lcode where entry_ty='" + CommonInfo.EntryTy + "'", null, 25);      // Changed by Sachin N. S. on 27/09/2018 for Pharma Retailer Changes

                    string cFldList = _dr[0].ToString();
                    string[][] cFldLst = cFldList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(':')).ToArray();
                    var cAcId =
                        from _cFldLst in cFldLst
                        where _cFldLst[0].ToUpper().Contains("AC_ID")
                        select _cFldLst[0];
                    if (cAcId.Count<string>() == 0)
                    {
                        xcFldList += ",Ac_mast.Ac_Id ";
                    }
                    xcFldList += "";
                    xcRetCol += "";

                    cCond += " And (Ac_mast.ldeactive = 0 Or (Ac_mast.ldeactive = 1 And Ac_mast.deactfrom > getdate() )) ";
                    cSql = "set dateformat dmy select " + cFldLst[0][0] + xcFldList + " from " + cTbl + " where " + cCond + " order by Ac_Name ";
                    cFrmCap = "Select Party";
                    cSrchCol = "Ac_Name" + xcSrchCol;
                    cDispCol = "Ac_Name:Party" + xcDispCol;
                    cRetCol = "Ac_Id,Ac_Name" + xcRetCol;
                    break;

                case "WAREHOUSE":
                    xcFldList = "";
                    xcDispCol = "";
                    xcRetCol = "";
                    cTbl = " Warehouse ";
                    cCond = " Ware_nm!='' ";

                    cFldList = "Ware_nm";

                    cCond += " And Validity like '%" + CommonInfo.EntryTy + "%' ";     // Changed by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes
                    cSql = "set dateformat dmy select Ware_nm from Warehouse where " + cCond + " order by Ware_nm ";
                    cFrmCap = "Select "+CommonInfo.EntryTy=="HS" ? "Store Room" : "Warehouse";  // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes

                    cSrchCol = "Ware_nm";
                    cDispCol = "Ware_nm:" + (CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse") + xcDispCol;
                    cRetCol = "Ware_nm" + xcRetCol;
                    break;

                //***** Added by Sachin N. S. on 23/05/2013 for Bug-14538 -- End *****//
                //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- Start ******//
                case "COUNTER":
                    cSql = "SELECT CNTRCODE,CNTRDESC FROM COUNTERMAST ";
                    cFrmCap = "Select Counter";
                    cSrchCol = "CNTRCODE+CNTRDESC";
                    cDispCol = "CNTRCODE:Counter Code,CNTRDESC:Counter Desc";
                    cRetCol = "CNTRCODE";
                    break;
                    //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- End ******//
            }

            _dt = _oDataAccess.GetDataTable(cSql, null, 50);
            DataView _dvw = _dt.DefaultView;

            _oSelPop.pdataview = _dvw;
            _oSelPop.pformtext = cFrmCap;
            _oSelPop.psearchcol = cSrchCol;
            _oSelPop.pDisplayColumnList = cDispCol;
            _oSelPop.pRetcolList = cRetCol;
            _oSelPop.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
            _oSelPop.ShowDialog();
            if (_oSelPop.pReturnArray != null)
            {
                switch (_popupType)
                {
                    case "SERIES":
                        this.txtInvSeries.Text = _oSelPop.pReturnArray[0].ToString();
                        CommonInfo.InvSeries = _oSelPop.pReturnArray[0].ToString();
                        //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- Start ******//
                        if (CommonInfo.InvSeries != "")
                        {
                            //if (clsInsUpdDelPrint.CheckInvNoLength("PS", CommonInfo.InvSeries) == false)
                            if (clsInsUpdDelPrint.CheckInvNoLength(CommonInfo.EntryTy, CommonInfo.InvSeries) == false)      // Changed by Sachin N. S. on 27/09/2018 for Pharma Retailer Changes
                            {
                                MessageBox.Show("Invoice Number size should not be more than 15.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                CommonInfo.InvSeries = "";
                                this.txtInvSeries.Text = "";
                            }
                        }
                        //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- End ******//
                        break;
                    case "DEPARTMENT":
                        this.txtDepartment.Text = _oSelPop.pReturnArray[0].ToString();
                        CommonInfo.Department = _oSelPop.pReturnArray[0].ToString();
                        break;
                    case "CATEGORY":
                        this.txtCategory.Text = _oSelPop.pReturnArray[0].ToString();
                        CommonInfo.Category = _oSelPop.pReturnArray[0].ToString();
                        break;
                    // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- Start ****** //
                    case "PARTY":
                        this.txtParty.Text = _oSelPop.pReturnArray[1].ToString();
                        CommonInfo.PartyId = Convert.ToInt64(_oSelPop.pReturnArray[0]);
                        CommonInfo.PartyNm = _oSelPop.pReturnArray[1].ToString();
                        break;
                    case "WAREHOUSE":
                        this.txtWarehouse.Text = _oSelPop.pReturnArray[0].ToString();
                        CommonInfo.WareHouse = _oSelPop.pReturnArray[0].ToString();
                        break;
                    // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- End ****** //
                    //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- Start ******//
                    case "COUNTER":
                        DataRow _dr;
                        cSql = "set dateformat dmy select Tran_cd,cashinamt from poscashin where convert(varchar,date,103)=convert(varchar,getdate(),103) and username='" + CommonInfo.UserName + "' and CashOTran=0 and cntrcode= '" + _oSelPop.pReturnArray[0].ToString() + "'";
                        _dr = _oDataAccess.GetDataRow(cSql, null, 50);

                        if (_dr != null)
                        {
                            CommonInfo.CashInNo = Convert.ToInt64(_dr[0]);
                            this.txtCash.Text = _dr[1].ToString();
                        }
                        else
                        {
                            CommonInfo.CashInNo = 0;
                            this.txtCash.Text = "";
                        }
                        this.txtCounter.Text = _oSelPop.pReturnArray[0].ToString();
                        CommonInfo.CounterNm = _oSelPop.pReturnArray[0].ToString();
                        break;
                        //****** Added by Sachin N. S. on 22/05/2013 for Bug-27503 -- Start ******//
                }
            }
        }

        #region Generate Process Id's
        private void mInsertProcessIdRecord()/*Added Rup 07/03/2011*/
        {
            _oDataAccess = new clsDataAccess();
            DataSet dsData = new DataSet();
            string sqlstr;
            int pi;
            pi = Process.GetCurrentProcess().Id;
            cAppName = "udPointofSale.exe";
            cAppPId = Convert.ToString(Process.GetCurrentProcess().Id);
            sqlstr = "Set DateFormat dmy insert into vudyog..ExtApplLog (pApplCode,CallDate,pApplNm,pApplId,pApplDesc,cApplNm,cApplId,cApplDesc) Values('" + CommonInfo.ApplCode + "','" + DateTime.Now.Date.ToString() + "','" + CommonInfo.ApplName + "'," + CommonInfo.ApplPId + ",'" + CommonInfo.AppCaption + "','" + cAppName + "'," + cAppPId + ",'" + this.Text.Trim() + "')";
            _oDataAccess.ExecuteSQLStatement(sqlstr, null, 20, true);
        }

        private void mDeleteProcessIdRecord()/*Added Rup 07/03/2011*/
        {
            _oDataAccess = new clsDataAccess();
            if (string.IsNullOrEmpty(CommonInfo.ApplName) || CommonInfo.ApplPId == 0 || string.IsNullOrEmpty(this.cAppName) || string.IsNullOrEmpty(this.cAppPId))
            {
                return;
            }
            DataSet dsData = new DataSet();
            string sqlstr;
            sqlstr = " Delete from vudyog..ExtApplLog where pApplNm='" + CommonInfo.ApplName + "' and pApplId=" + CommonInfo.ApplPId + " and cApplNm= '" + cAppName + "' and cApplId= " + cAppPId;
            _oDataAccess.ExecuteSQLStatement(sqlstr, null, 20, true);
        }
        #endregion Generate Process Id's

        private void _InitialSettings()
        {
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            DataRow _dr1;

            cSql = "select Top 1 * from UDPOSSETTINGS where Entry_ty = '" + CommonInfo.EntryTy + "' ";
            _drPosSettings = _oDataAccess.GetDataRow(cSql, null, 50);
            if (_drPosSettings != null)
            {
                if ((bool)_drPosSettings["LSHWLGSCRN"] == false)
                {
                    if (_drPosSettings["CDEFAPARTY"].ToString() == string.Empty)
                    {
                        MessageBox.Show("Party cannot be empty.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    CommonInfo.InvSeries = _drPosSettings["CDEFAINVSR"].ToString();
                    CommonInfo.Department = _drPosSettings["CDEFADEPT"].ToString();
                    CommonInfo.Category = _drPosSettings["CDEFACATE"].ToString();
                    CommonInfo.PartyNm = _drPosSettings["CDEFAPARTY"].ToString();
                    CommonInfo.WareHouse = _drPosSettings["CDEFAWARE"].ToString();

                    cSql = "select Top 1 Ac_id from ac_mast where ac_name = '" + _drPosSettings["CDEFAPARTY"].ToString().Trim() + "' ";
                    _dr1 = _oDataAccess.GetDataRow(cSql, null, 50);
                    if (_dr1 != null)
                    {
                        CommonInfo.PartyId = (int)_dr1["Ac_id"];
                    }
                    else
                    {
                        MessageBox.Show("Party not found in Party Master.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    this.Hide();
                    //udFrmPointofSale _frmPos = new udFrmPointofSale();
                    udFrmPointofSaleNew _frmPos = new udFrmPointofSaleNew();
                    //udtesting _frmPos = new udtesting();
                    _frmPos.ShowDialog();
                    this.Close();
                }
                else
                {
                    CommonInfo.InvSeries = _drPosSettings["CDEFAINVSR"].ToString();
                    CommonInfo.Department = _drPosSettings["CDEFADEPT"].ToString();
                    CommonInfo.Category = _drPosSettings["CDEFACATE"].ToString();
                    CommonInfo.PartyNm = _drPosSettings["CDEFAPARTY"].ToString();
                    CommonInfo.WareHouse = _drPosSettings["CDEFAWARE"].ToString();

                    cSql = "select Top 1 Ac_id from ac_mast where ac_name = '" + _drPosSettings["CDEFAPARTY"].ToString().Trim() + "' ";
                    _dr1 = _oDataAccess.GetDataRow(cSql, null, 50);
                    if (_dr1 != null)
                    {
                        CommonInfo.PartyId = (int)_dr1["Ac_id"];
                    }
                    else
                        CommonInfo.PartyId = 0;

                    this.txtCategory.Text = CommonInfo.Category;
                    this.txtDepartment.Text = CommonInfo.Department;
                    this.txtInvSeries.Text = CommonInfo.InvSeries;
                    this.txtParty.Text = CommonInfo.PartyNm;
                    this.txtWarehouse.Text = CommonInfo.WareHouse;
                }
            }

            this.lblWarehouse.Text = CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse";   // Added by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
        }

        private void enableDisableControls()
        {
            this.cmdInvSeries.Enabled = (bool)_drPosSettings["LSHWINVSR"];
            this.cmdCategory.Enabled = (bool)_drPosSettings["LSHWCATE"];
            this.btnDepartment.Enabled = (bool)_drPosSettings["LSHWDEPT"];
            this.cmdParty.Enabled = (bool)_drPosSettings["LSHWPARTY"];
            this.cmdWarehouse.Enabled = (bool)_drPosSettings["LSHWWARENM"];
            this.cmdCounter.Enabled = !(bool)_drPosSettings["LCINMNDTRY"];
        }
    }
}
