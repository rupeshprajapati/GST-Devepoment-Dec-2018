using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DataAccess_Net;
using System.Globalization;
using System.Diagnostics;
using uBaseForm;
using System.Collections;

namespace udPosSettings
{
    public partial class udPosSettings : uBaseForm.FrmBaseForm
    {
        //******* Form Properties - Start *******\\
        clsDataAccess _oDataAccess;
        string cAppName = "", cAppPId = "", ApplCode = "", ApplName = "", AppCaption = "";
        int ApplPId = 0;
        string _Iconpath;
        DataTable _dtPosSetting;
        string ServiceType = string.Empty;

        //******* Form Properties - End *******\\

        public udPosSettings(string[] _args)
        {
            CultureInfo ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            clsDataAccess._serverName = _args[2].ToString();
            clsDataAccess._databaseName = _args[1].ToString();
            clsDataAccess._userID = _args[3].ToString();
            clsDataAccess._password = _args[4].ToString();

            this.pPApplRange = _args[5].ToString();
            this.pAppUerName = _args[6].ToString();
            _Iconpath = _args[7].ToString().Replace("<*#*>", " ");
            this.pPApplName = _args[9];
            this.pPApplPID = Convert.ToInt16(_args[10]);
            this.pPApplCode = _args[11];
            this.pPApplText = _args[8].Replace("<*#*>", " ");

            InitializeComponent();
        }

        private void udPosSettings_Load(object sender, EventArgs e)
        {
            this.Text = "Point of Sale Settings";

            this.Icon = new Icon(_Iconpath);

            this.getDataCursor();

            this.MainDataBinding();

            this.setEnabledDisableControls(false);

            this.SetMenuRights();

            this.pEditButton = true;

            this.mthChkNavigationButton();

            this.mInsertProcessIdRecord();
        }

        private void udPosSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.mDeleteProcessIdRecord();
        }

        #region Generate Process Id's
        private void mInsertProcessIdRecord()
        {
            _oDataAccess = new clsDataAccess();
            DataSet dsData = new DataSet();
            string sqlstr;
            int pi;
            pi = Process.GetCurrentProcess().Id;
            cAppName = "udPosSettings.exe";
            cAppPId = Convert.ToString(Process.GetCurrentProcess().Id);
            //sqlstr = "Set DateFormat dmy insert into vudyog..ExtApplLog (pApplCode,CallDate,pApplNm,pApplId,pApplDesc,cApplNm,cApplId,cApplDesc) Values('" + ApplCode + "','" + DateTime.Now.Date.ToString() + "','" + ApplName + "'," + ApplPId + ",'" + AppCaption + "','" + cAppName + "'," + cAppPId + ",'" + this.Text.Trim() + "')";
            sqlstr = "Set DateFormat dmy insert into vudyog..ExtApplLog (pApplCode,CallDate,pApplNm,pApplId,pApplDesc,cApplNm,cApplId,cApplDesc) Values('" + this.pPApplCode + "','" + DateTime.Now.Date.ToString() + "','" + this.pPApplName + "'," + this.pPApplPID + ",'" + this.pPApplText + "','" + cAppName + "'," + cAppPId + ",'" + this.Text.Trim() + "')";
            _oDataAccess.ExecuteSQLStatement(sqlstr, null, 20, true);
        }

        private void mDeleteProcessIdRecord()
        {
            _oDataAccess = new clsDataAccess();
            if (string.IsNullOrEmpty(ApplName) || ApplPId == 0 || string.IsNullOrEmpty(this.cAppName) || string.IsNullOrEmpty(this.cAppPId))
            {
                return;
            }
            DataSet dsData = new DataSet();
            string sqlstr;
            sqlstr = " Delete from vudyog..ExtApplLog where pApplNm='" + this.pPApplName + "' and pApplId=" + this.pPApplPID + " and cApplNm= '" + cAppName + "' and cApplId= " + cAppPId;
            _oDataAccess.ExecuteSQLStatement(sqlstr, null, 20, true);
        }
        #endregion Generate Process Id's

        private void getDataCursor()
        {
            _oDataAccess = new clsDataAccess();

            _dtPosSetting = _oDataAccess.GetDataTable("SELECT top 1 A.* FROM UDPOSSETTINGS A INNER JOIN LCODE B ON A.ENTRY_TY=B.ENTRY_TY ORDER BY A.ENTRY_TY DESC", null, 25);
            _dtPosSetting.TableName = "UDPOSSETTINGS";

        }

        private void MainDataBinding()
        {
            this.chkCINMNDTRY.DataBindings.Add("Checked", _dtPosSetting, "LCINMNDTRY");
            this.chkSHWCATE.DataBindings.Add("Checked", _dtPosSetting, "LSHWCATE");
            this.chkSHWDEPT.DataBindings.Add("Checked", _dtPosSetting, "LSHWDEPT");
            this.chkSHWINVSR.DataBindings.Add("Checked", _dtPosSetting, "LSHWINVSR");
            this.chkshwlgnscrn.DataBindings.Add("Checked", _dtPosSetting, "LSHWLGSCRN");
            this.chkSHWWARENM.DataBindings.Add("Checked", _dtPosSetting, "LSHWWARENM");
            this.chkLSHWPARTY.DataBindings.Add("Checked", _dtPosSetting, "LSHWPARTY");
            this.chkSHUFFLEUOM.DataBindings.Add("Checked", _dtPosSetting, "SHUFFLEUOM");
            this.chkGstIncl.DataBindings.Add("Checked", _dtPosSetting, "GSTINCL");
            this.chkit_rate.DataBindings.Add("Checked", _dtPosSetting, "IT_RATE");
            this.chki_disc.DataBindings.Add("Checked", _dtPosSetting, "I_DISC");
            this.chkv_disc.DataBindings.Add("Checked", _dtPosSetting, "V_DISC");
            this.chkLCECsv.DataBindings.Add("Checked", _dtPosSetting, "LCECSV");
            this.chkLCCsvEmail.DataBindings.Add("Checked", _dtPosSetting, "LCCSVEMAIL");

            this.chkAskFroPrint.DataBindings.Add("Checked", _dtPosSetting, "LASKPRINT");//Added by Rupesh G. on 04122018

            this.chki_showdisc.DataBindings.Add("Checked", _dtPosSetting , "LSHWITMDISC");  //Added by Prajakta B. on 30112018

            this.txtBCode_nm.DataBindings.Add("text", _dtPosSetting, "BCODE_NM");
            this.txtCDEFACATE.DataBindings.Add("text", _dtPosSetting, "CDEFACATE");
            this.txtCDEFADEPT.DataBindings.Add("text", _dtPosSetting, "CDEFADEPT");
            this.txtCDEFAINVSR.DataBindings.Add("text", _dtPosSetting, "CDEFAINVSR");
            this.txtCDEFAWARE.DataBindings.Add("text", _dtPosSetting, "CDEFAWARE");
            this.txtCDEFAPARTY.DataBindings.Add("text", _dtPosSetting, "CDEFAPARTY");
            this.txtCode_nm.DataBindings.Add("text", _dtPosSetting, "CODE_NM");
            this.txtEntry_ty.DataBindings.Add("text", _dtPosSetting, "ENTRY_TY");
            this.txtPOSBarcode.DataBindings.Add("text", _dtPosSetting, "POSBARCODE");
            if (_dtPosSetting.Rows.Count > 0)
            {
                this.lblSHWWARENM.Text = (_dtPosSetting.Rows[0]["Entry_ty"].ToString() == "HS" ? "Store Room" : "Warehouse");
                this.lblCDEFAWARE.Text = (_dtPosSetting.Rows[0]["Entry_ty"].ToString() == "HS" ? "Default Store Room" : "Default Warehouse");
            }
        }

        private void setEnabledDisableControls(bool lEnbl)
        {
            this.btnEntry_ty.Enabled = !lEnbl;

            this.btnCDEFACATE.Enabled = lEnbl;
            this.btnCDEFADEPT.Enabled = lEnbl;
            this.btnCDEFAPARTY.Enabled = lEnbl;
            this.btnCDEFAWARE.Enabled = lEnbl;
            this.btncSHWINVSR.Enabled = lEnbl;

            this.chkSHWWARENM.Enabled = lEnbl;
            this.chkLSHWPARTY.Enabled = lEnbl;
            this.chkSHWCATE.Enabled = lEnbl;
            //this.chkCINMNDTRY.Enabled = lEnbl;   //Commented by Prajakta B. on 12/12/2018
            this.chkSHWDEPT.Enabled = lEnbl;
            this.chkSHWINVSR.Enabled = lEnbl;
            this.chkshwlgnscrn.Enabled = lEnbl;
            this.chkSHWWARENM.Enabled = lEnbl;
            this.chkSHUFFLEUOM.Enabled = lEnbl;
            this.chkv_disc.Enabled = lEnbl;
            this.chki_disc.Enabled = lEnbl;
            this.chkit_rate.Enabled = lEnbl;
            this.chkGstIncl.Enabled = lEnbl;
            this.chkLCECsv.Enabled = lEnbl;
            this.chkLCCsvEmail.Enabled = lEnbl;

            this.chkAskFroPrint.Enabled = lEnbl;//Added by Rupesh G. on 04122018

            //Added by Prajakta B. on 30112018  --Start
            if (this.chki_disc.Checked==true && this.chki_disc.Enabled == true)
            {
                this.chki_showdisc.Enabled = true;  
            }
            else
            {
                this.chki_showdisc.Enabled = false; 
            }
            //Added by Prajakta B. on 30112018  --End
            //Added by Prajakta B. on 12/12/2018  --Start
            if (this.pEditMode==true && this.chkshwlgnscrn.Checked== lEnbl)
            {
                 this.chkCINMNDTRY.Enabled = lEnbl;
            }
            else
            {
                 this.chkCINMNDTRY.Enabled = false;
            }
            //Added by Prajakta B. on 12/12/2018  --End
            this.txtPOSBarcode.Enabled = lEnbl;
            this.txtPOSBarcode.ReadOnly = !lEnbl;
            this.lblSHUFFLEUOM.Visible = false;
            this.chkSHUFFLEUOM.Visible = false;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            string cSqlStr = "", vMainFldVal = "";
            this.mcheckCallingApplication();
            vMainFldVal = "Entry_ty";
            cSqlStr = "Select top 1 * from udPOSSettings order by " + vMainFldVal;
            _dtPosSetting = _oDataAccess.GetDataTable(cSqlStr, null, 20);
            _dtPosSetting.TableName = "UDPOSSETTINGS";
            this.mthView();
            this.mthChkNavigationButton();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            string cSqlStr = "";
            this.mcheckCallingApplication();
            string vMainFldVal = _dtPosSetting.Rows[0]["Entry_ty"].ToString().Trim();
            cSqlStr = "Select top 1 * from udPosSettings Where Entry_ty <'" + vMainFldVal + "' order by Entry_ty desc";
            _dtPosSetting = _oDataAccess.GetDataTable(cSqlStr, null, 20);
            _dtPosSetting.TableName = "UDPOSSETTINGS";
            this.mthView();
            this.mthChkNavigationButton();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            this.mcheckCallingApplication();
            string vMainFldVal = _dtPosSetting.Rows[0]["Entry_ty"].ToString().Trim();
            string cSqlStr = "Select top 1 * from udPosSettings Where Entry_ty >'" + vMainFldVal + "' order by Entry_ty ";
            _dtPosSetting = _oDataAccess.GetDataTable(cSqlStr, null, 20);
            _dtPosSetting.TableName = "UDPOSSETTINGS";
            this.mthView();

            this.mthChkNavigationButton();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            this.mcheckCallingApplication();
            this.pAddMode = false;
            this.pEditMode = false;
            DataSet dsTemp = new DataSet();
            string cSqlStr = "select top 1 * from udPOSSettings order by Entry_ty desc";
            _dtPosSetting = _oDataAccess.GetDataTable(cSqlStr, null, 20);
            _dtPosSetting.TableName = "UDPOSSETTINGS";
            this.mthView();
            this.mthChkNavigationButton();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.mcheckCallingApplication();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.mcheckCallingApplication();
            this.mthEdit();
        }

        private void mthEdit()
        {
            if (this._dtPosSetting.Rows.Count <= 0)
            {
                return;
            }
            this.pAddMode = false;
            this.pEditMode = true;
            this.setEnabledDisableControls(true);
            _dtPosSetting.Rows[0].BeginEdit();
            this.mthChkNavigationButton();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool cValid = true;
            if (cValid == false)
                return;

            this.mcheckCallingApplication();

            this.Refresh();
            this.mthSave();

            this.mthChkNavigationButton();
        }

        private void mthSave()
        {

            string vSaveString = string.Empty;
            this.BindingContext[_dtPosSetting].EndCurrentEdit();
            _dtPosSetting.Rows[0].EndEdit();
            _dtPosSetting.Rows[0].AcceptChanges();

            this.mSaveCommandString(ref vSaveString, "#ID#");
            this.pAddMode = false;
            this.pEditMode = false;
            this.setEnabledDisableControls(false);
            _oDataAccess.ExecuteSQLStatement(vSaveString, null, 20, true);
            string cSqlStr = "Select top 1 * from udPosSettings where Entry_ty='" + _dtPosSetting.Rows[0]["Entry_ty"].ToString() + "' order by Entry_ty desc";
            _dtPosSetting = _oDataAccess.GetDataTable(cSqlStr, null, 20);
            _dtPosSetting.TableName = "UDPOSSETTINGS";
            this.mthView();
        }

        private void mSaveCommandString(ref string vSaveString, string vkeyField)
        {
            string vfldList = string.Empty;
            string vfldValList = string.Empty;
            string vIdentityFields = string.Empty, vfldVal = string.Empty, vDataType = string.Empty;
            string vMainTblNm = "udPosSettings";
            DataSet ds = new DataSet();
            DataSet dsVal = new DataSet();
            string sysdate = "";

            /*Identity Columns--->*/
            DataSet dsData = new DataSet();
            string sqlstr = "select c.name as ColName from sys.objects o inner join sys.columns c on o.object_id = c.object_id where c.is_identity = 1 ";
            sqlstr = sqlstr + " and o.name='" + vMainTblNm + " ' ";
            dsData = _oDataAccess.GetDataSet(sqlstr, null, 20);
            foreach (DataRow dr1 in dsData.Tables[0].Rows)
            {
                if (string.IsNullOrEmpty(vIdentityFields) == false) { vIdentityFields = vIdentityFields + ","; }
                vIdentityFields = vIdentityFields + "#" + dr1["ColName"].ToString().Trim() + "#";
            }
            /*<---Identity Columns--->*/

            String a = "select getdate() as sysdate";
            ds = _oDataAccess.GetDataSet(a, null, 20);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sysdate = ds.Tables[0].Rows[0]["sysdate"].ToString();
                }
            }
            dsVal = _oDataAccess.GetDataSet(a, null, 20);

            if (this.pEditMode == true)
            {
                vSaveString = "Set DateFormat dmy ";
                vSaveString += "UPDATE UDPOSSETTINGS SET LSHWLGSCRN = " + (Convert.ToBoolean(chkshwlgnscrn.Checked) ? "1" : "0");
                vSaveString += ", LSHWINVSR = " + (chkSHWINVSR.Checked ? "1" : "0");
                vSaveString += ", CDEFAINVSR = '" + txtCDEFAINVSR.Text.Trim() + "'";
                vSaveString += ", LSHWDEPT = " + (chkSHWDEPT.Checked ? "1" : "0");
                vSaveString += ", CDEFADEPT = '" + txtCDEFADEPT.Text.Trim() + "'";
                vSaveString += ", LSHWCATE = " + (chkSHWCATE.Checked ? "1" : "0");
                vSaveString += ", CDEFACATE = '" + txtCDEFACATE.Text.Trim() + "'";
                vSaveString += ", LCINMNDTRY = " + (chkCINMNDTRY.Checked ? "1" : "0");
                vSaveString += ", LSHWPARTY = " + (chkLSHWPARTY.Checked ? "1" : "0");
                vSaveString += ", CDEFAPARTY = '" + txtCDEFAPARTY.Text.Trim() + "'";
                vSaveString += ", LSHWWARENM = " + (chkSHWWARENM.Checked ? "1" : "0");
                vSaveString += ", CDEFAWARE = '" + txtCDEFAWARE.Text.Trim() + "'";
                vSaveString += ", SHUFFLEUOM = " + (chkSHUFFLEUOM.Checked ? "1" : "0");
                vSaveString += ", V_DISC = " + (chkv_disc.Checked ? "1" : "0");
                vSaveString += ", I_DISC = " + (chki_disc.Checked ? "1" : "0");
                vSaveString += ", IT_RATE = " + (chkit_rate.Checked ? "1" : "0");
                vSaveString += ", GSTINCL = " + (chkGstIncl.Checked ? "1" : "0");
                vSaveString += ", POSBARCODE = '" + txtPOSBarcode.Text.Trim() + "'";
                vSaveString += ", LCECSV = " + (chkLCECsv.Checked ? "1" : "0");
                vSaveString += ", LCCSVEMAIL = " + (chkLCCsvEmail.Checked ? "1" : "0");
                vSaveString += ", LASKPRINT = " + (chkAskFroPrint.Checked ? "1" : "0");//Added by Rupesh G. on 04122018
                vSaveString += ", LSHWITMDISC = " + (chki_showdisc.Checked ? "1" : "0");  //Added by Prajakta B. on 30112018
                vSaveString += " WHERE ID = " + _dtPosSetting.Rows[0]["ID"].ToString();

                vSaveString += "UPDATE LCODE SET ";
                vSaveString += " LCINMNDTRY = " + (chkCINMNDTRY.Checked ? "1" : "0");
                vSaveString += ", SHUFFLEUOM = " + (chkSHUFFLEUOM.Checked ? "1" : "0");
                vSaveString += ", V_DISC = " + (chkv_disc.Checked ? "1" : "0");
                vSaveString += ", I_DISC = " + (chki_disc.Checked ? "1" : "0");
                vSaveString += ", IT_RATE = " + (chkit_rate.Checked ? "1" : "0");
                vSaveString += ", GSTINCL = " + (chkGstIncl.Checked ? "1" : "0");
                vSaveString += ", POSBARCODE = '" + txtPOSBarcode.Text.Trim() + "'";
                vSaveString += ", LCECSV = " + (chkLCECsv.Checked ? "1" : "0");
                vSaveString += ", LCCSVEMAIL = " + (chkLCCsvEmail.Checked ? "1" : "0");
                vSaveString += " WHERE ENTRY_TY = '" + _dtPosSetting.Rows[0]["ENTRY_TY"].ToString() + "'";
            }
            this.mthChkNavigationButton();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.mthCancel(sender, e);
            this.mthView();
            this.setEnabledDisableControls(false);
            this.mthChkNavigationButton();
        }

        private void mthCancel(object sender, EventArgs e)
        {
            if (this._dtPosSetting.Rows.Count != 0)
            {
                _dtPosSetting.Rows[0].CancelEdit();
            }

            this.setEnabledDisableControls(false);
            if (this._dtPosSetting.Rows.Count == 1)
            {
                _dtPosSetting.Rows[0].CancelEdit();
                if (this.pAddMode)
                {
                    _dtPosSetting.Rows[0].Delete();
                    this.btnLast_Click(sender, e);
                }
                if (this.pEditMode)
                {
                    string vMainFldVal = _dtPosSetting.Rows[0]["Entry_ty"].ToString().Trim();
                    string cSqlStr = "Select top 1 * from udPosSettings Where Entry_ty='" + vMainFldVal + "'";
                    _dtPosSetting = _oDataAccess.GetDataTable(cSqlStr, null, 20);
                    _dtPosSetting.TableName = "UDPOSSETTINGS";
                }
            }
            this.pAddMode = false;
            this.pEditMode = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {

        }

        private void SetFormColor()
        {
            DataSet dsColor = new DataSet();
            Color myColor = Color.Coral;
            string strSQL;
            string colorCode = string.Empty;
            strSQL = "select vcolor from Vudyog..co_mast where compid =" + this.pCompId;
            dsColor = _oDataAccess.GetDataSet(strSQL, null, 20);
            if (dsColor != null)
            {
                if (dsColor.Tables.Count > 0)
                {
                    dsColor.Tables[0].TableName = "ColorInfo";
                    colorCode = dsColor.Tables["ColorInfo"].Rows[0]["vcolor"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(colorCode))
            {
                this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                myColor = Color.FromArgb(Convert.ToInt32(colorCode.Trim()));
            }
            this.BackColor = Color.FromArgb(myColor.R, myColor.R, myColor.G, myColor.B);
        }

        private void mcheckCallingApplication()
        {
            Process pProc;
            Boolean procExists = true;
            try
            {
                pProc = Process.GetProcessById(Convert.ToInt16(this.pPApplPID));
                String pName = pProc.ProcessName;
                string pName1 = this.pPApplName.Substring(0, this.pPApplName.IndexOf("."));
                if (pName.ToUpper() != pName1.ToUpper())
                {
                    procExists = false;
                }
            }
            catch (Exception)
            {
                procExists = false;
            }
            if (procExists == false)
            {
                MessageBox.Show("Can't proceed,Main Application " + this.pPApplText + " is closed", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
        }

        private void mthChkNavigationButton()
        {
            DataSet dsTemp = new DataSet();
            this.btnForward.Enabled = false;
            this.btnLast.Enabled = false;
            this.btnFirst.Enabled = false;
            this.btnBack.Enabled = false;
            btnEdit.Enabled = false;
            Boolean vBtnAdd, vBtnEdit, vBtnDelete, vBtnPrint;
            if (ServiceType.ToUpper() != "VIEWER VERSION")
            {
                if (_dtPosSetting.Rows.Count == 0)
                {
                    if (this.pAddMode == false && this.pEditMode == false)
                    {
                        this.btnCancel.Enabled = false;
                        this.btnSave.Enabled = false;

                        vBtnAdd = false;
                        vBtnDelete = false;
                        vBtnEdit = false;
                        vBtnPrint = false;
                        this.mthChkAEDPButton(vBtnAdd, vBtnDelete, vBtnEdit, vBtnPrint);
                    }
                    return;
                }
            }
            if (this.pAddMode == false && this.pEditMode == false)
            {
                if (_dtPosSetting.Rows.Count > 0)
                {
                    string vMainTblNm = "udPosSettings", vMainField = "Entry_ty";
                    string vMainFldVal = _dtPosSetting.Rows[0]["Entry_ty"].ToString().Trim();
                    string SqlStr = "select Id from " + vMainTblNm + " Where " + vMainField + " >'" + vMainFldVal + "'";
                    dsTemp = _oDataAccess.GetDataSet(SqlStr, null, 20);
                    if (dsTemp.Tables[0].Rows.Count > 0)
                    {
                        this.btnForward.Enabled = true;
                        this.btnLast.Enabled = true;
                    }
                    SqlStr = "select Id from " + vMainTblNm + " Where " + vMainField + "<'" + vMainFldVal + "'";

                    dsTemp = _oDataAccess.GetDataSet(SqlStr, null, 20);
                    if (dsTemp.Tables[0].Rows.Count > 0)
                    {
                        this.btnBack.Enabled = true;
                        this.btnFirst.Enabled = true;
                    }
                }
            }

            vBtnAdd = false;
            vBtnDelete = false;
            vBtnEdit = false;
            vBtnPrint = false;
            if (ServiceType.ToUpper() != "VIEWER VERSION")
            {
                if (this.btnForward.Enabled == true || this.btnBack.Enabled == true || (this.pAddMode == false && this.pEditMode == false))
                {
                    vBtnAdd = false;
                    if (_dtPosSetting.Rows.Count > 0)
                    {
                        vBtnDelete = true;
                        vBtnEdit = true;
                        vBtnPrint = true;
                    }
                }
                this.mthChkAEDPButton(vBtnAdd, vBtnDelete, vBtnEdit, vBtnPrint);
            }
        }

        private void btnEntry_ty_Click(object sender, EventArgs e)
        {
            mcheckCallingApplication();
            string vMainTblNm = "udPosSettings";
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            string SqlStr = "Select Entry_ty,Code_nm from " + vMainTblNm + " order by Entry_ty ";
            tDs = _oDataAccess.GetDataSet(SqlStr, null, 20);

            DataView dvw = tDs.Tables[0].DefaultView;

            VForText = "Select Transaction";
            vSearchCol = "ENTRY_TY+CODE_NM";
            vDisplayColumnList = "ENTRY_TY:Transaction Type,CODE_NM:Transaction Name";
            vReturnCol = "ENTRY_TY";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();

            if (oSelectPop.pReturnArray != null)
            {
                this.txtEntry_ty.Text = oSelectPop.pReturnArray[0];
                string cEntTy = this.txtEntry_ty.Text.Trim();
                SqlStr = "Select top 1 * from " + vMainTblNm + " Where Entry_ty='" + cEntTy + "' order by Entry_ty ";
                _dtPosSetting = _oDataAccess.GetDataTable(SqlStr, null, 20);
            }
            else if (oSelectPop.pReturnArray == null)
            {
                string cEntTy = this.txtEntry_ty.Text.Trim();
                if (String.IsNullOrEmpty(cEntTy))
                {
                    return;
                }
                _dtPosSetting = _oDataAccess.GetDataTable(SqlStr, null, 20);
            }

            this.mthView();
            this.mthChkNavigationButton();
        }

        private void btncSHWINVSR_Click(object sender, EventArgs e)
        {
            this.getpopdata("INV_SR");
        }

        private void btnCDEFADEPT_Click(object sender, EventArgs e)
        {
            this.getpopdata("DEPT");
        }

        private void btnCDEFACATE_Click(object sender, EventArgs e)
        {
            this.getpopdata("CATE");
        }

        private void btnCDEFAPARTY_Click(object sender, EventArgs e)
        {
            this.getpopdata("PARTY");
        }

        private void btnCDEFAWARE_Click(object sender, EventArgs e)
        {
            this.getpopdata("WAREHOUSE");
        }

        private void mthChkAEDPButton(Boolean vBtnAdd, Boolean vBtnEdit, Boolean vBtnDelete, Boolean vBtnPrint)
        {
            this.btnNew.Enabled = false;
            this.btnEdit.Enabled = false;
            if (vBtnAdd && this.pAddButton)
            {
                this.btnNew.Enabled = true;
            }
            if (vBtnEdit && this.pEditButton)
            {
                this.btnEdit.Enabled = true;
            }
            if (vBtnDelete && this.pDeleteButton)
            {
            }
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            if (this.btnNew.Enabled == false && this.btnEdit.Enabled == false)
            {
                this.btnSave.Enabled = true;
                this.btnCancel.Enabled = true;
            }
            if (this.btnSave.Enabled == false)
            {
                this.btnLogout.Enabled = true;
            }
        }

        private void chkshwlgnscrn_CheckedChanged(object sender, EventArgs e)
        {
            if (this.pEditMode == true)
            {
                if (this.chkshwlgnscrn.Checked == true)
                {
                    this.chkSHWINVSR.Enabled = true;
                    this.chkSHWDEPT.Enabled = true;
                    this.chkSHWCATE.Enabled = true;
                    this.chkCINMNDTRY.Enabled = true;
                    this.chkLSHWPARTY.Enabled = true;
                    this.chkSHWWARENM.Enabled = true;

                    this.chkSHWINVSR.Checked = true;
                    this.chkSHWDEPT.Checked = true;
                    this.chkSHWCATE.Checked = true;
                    //this.chkCINMNDTRY.Checked = true; //Commented by Prajakta B. on 12/12/2018
                    this.chkLSHWPARTY.Checked = true;
                    this.chkSHWWARENM.Checked = true;
                }
                else
                {
                    this.chkSHWINVSR.Enabled = false;
                    this.chkSHWDEPT.Enabled = false;
                    this.chkSHWCATE.Enabled = false;
                    this.chkCINMNDTRY.Enabled = false;
                    this.chkLSHWPARTY.Enabled = false;
                    this.chkSHWWARENM.Enabled = false;

                    this.chkSHWINVSR.Checked = false;
                    this.chkSHWDEPT.Checked = false;
                    this.chkSHWCATE.Checked = false;
                    //this.chkCINMNDTRY.Checked = true;  //Commented by Prajakta B. on 12/12/2018
                    this.chkCINMNDTRY.Checked = false;  //Modified by Prajakta B. on 12/12/2018
                    this.chkLSHWPARTY.Checked = false;
                    this.chkSHWWARENM.Checked = false;
   
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
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnLogout.Enabled)
                btnLogout_Click(this.btnLogout, e);
        }

        private void mthView()
        {
            this.mthBindClear();
            this.MainDataBinding();
        }

        //Added by Prajakta B. on 30112018 --Start
        private void chki_disc_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chki_disc.Checked == true)
            {
                this.chki_showdisc.Enabled = true;
            }
            else
            {
                this.chki_showdisc.Checked = false;
                this.chki_showdisc.Enabled = false;
            }
        }
        //Added by Prajakta B. on 30112018 --Start

        private void mthBindClear()
        {
            this.chkCINMNDTRY.DataBindings.Clear();
            this.chkLSHWPARTY.DataBindings.Clear();
            this.chkSHWCATE.DataBindings.Clear();
            this.chkSHWDEPT.DataBindings.Clear();
            this.chkSHWINVSR.DataBindings.Clear();
            this.chkshwlgnscrn.DataBindings.Clear();
            this.chkSHWWARENM.DataBindings.Clear();
            this.chkSHUFFLEUOM.DataBindings.Clear();
            this.chkGstIncl.DataBindings.Clear();
            this.chkit_rate.DataBindings.Clear();
            this.chki_disc.DataBindings.Clear();
            this.chkv_disc.DataBindings.Clear();
            this.chkLCECsv.DataBindings.Clear();
            this.chkLCCsvEmail.DataBindings.Clear();
            this.chkAskFroPrint.DataBindings.Clear();//Added by Rupesh G. on 04122018 

            this.chki_showdisc.DataBindings.Clear(); //Added by Prajakta B. on 30112018 --Start

            this.txtCDEFAPARTY.DataBindings.Clear();
            this.txtCDEFACATE.DataBindings.Clear();
            this.txtCDEFADEPT.DataBindings.Clear();
            this.txtCDEFAINVSR.DataBindings.Clear();
            this.txtCDEFAWARE.DataBindings.Clear();
            this.txtCode_nm.DataBindings.Clear();
            this.txtEntry_ty.DataBindings.Clear();
            this.txtBCode_nm.DataBindings.Clear();
            this.txtPOSBarcode.DataBindings.Clear();
        }

        private void getpopdata(string cType)
        {
            string SqlStr = "";
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;

            DataSet tDs = new DataSet();
            switch (cType)
            {
                case "INV_SR":
                    SqlStr = "Select '' as INV_SR union all Select INV_SR from Series where validity like '%" + this.txtEntry_ty.Text + "%' order by inv_sr ";
                    VForText = "Select Invoice Series";
                    vSearchCol = "INV_SR";
                    vDisplayColumnList = "INV_SR:Invoice Series";
                    vReturnCol = "INV_SR";
                    break;
                case "DEPT":
                    SqlStr = "Select '' as dept union all Select dept from department where validity like '%" + this.txtEntry_ty.Text + "%' order by dept";
                    VForText = "Select Department";
                    vSearchCol = "DEPT";
                    vDisplayColumnList = "DEPT:Department";
                    vReturnCol = "DEPT";
                    break;
                case "CATE":
                    SqlStr = "Select '' as cate union all Select cate from category where validity like '%" + this.txtEntry_ty.Text + "%' order by cate";
                    VForText = "Select Category";
                    vSearchCol = "CATE";
                    vDisplayColumnList = "CATE:Category";
                    vReturnCol = "CATE";
                    break;
                case "PARTY":
                    SqlStr = "Select ac_name from ac_mast order by ac_name ";
                    VForText = "Select Party";
                    vSearchCol = "AC_NAME";
                    vDisplayColumnList = "AC_NAME:Party";
                    vReturnCol = "AC_NAME";
                    break;
                case "WAREHOUSE":
                    SqlStr = "Select '' as ware_nm union all Select ware_nm from warehouse where validity like '%" + this.txtEntry_ty.Text + "%' order by ware_nm ";
                    VForText = "Select " + (this.txtEntry_ty.Text == "HS" ? "Store Room" : "Warehouse");
                    vSearchCol = "WARE_NM";
                    vDisplayColumnList = "WARE_NM:" + (this.txtEntry_ty.Text == "HS" ? "Store Room" : "Warehouse");
                    vReturnCol = "WARE_NM";
                    break;
            }
            tDs = _oDataAccess.GetDataSet(SqlStr, null, 20);

            DataView dvw = tDs.Tables[0].DefaultView;

            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();

            if (oSelectPop.pReturnArray != null)
            {
                switch (cType)
                {
                    case "INV_SR":
                        this.txtCDEFAINVSR.Text = oSelectPop.pReturnArray[0].ToString().Trim();
                        _dtPosSetting.Rows[0]["CDEFAINVSR"] = oSelectPop.pReturnArray[0].ToString().Trim();
                        break;
                    case "DEPT":
                        this.txtCDEFADEPT.Text = oSelectPop.pReturnArray[0].ToString().Trim();
                        _dtPosSetting.Rows[0]["CDEFADEPT"] = oSelectPop.pReturnArray[0].ToString().Trim();
                        break;
                    case "CATE":
                        this.txtCDEFACATE.Text = oSelectPop.pReturnArray[0].ToString().Trim();
                        _dtPosSetting.Rows[0]["CDEFACATE"] = oSelectPop.pReturnArray[0].ToString().Trim();
                        break;
                    case "PARTY":
                        this.txtCDEFAPARTY.Text = oSelectPop.pReturnArray[0].ToString().Trim();
                        _dtPosSetting.Rows[0]["CDEFAPARTY"] = oSelectPop.pReturnArray[0].ToString().Trim();
                        break;
                    case "WAREHOUSE":
                        this.txtCDEFAWARE.Text = oSelectPop.pReturnArray[0].ToString().Trim();
                        _dtPosSetting.Rows[0]["CDEFAWARE"] = oSelectPop.pReturnArray[0].ToString().Trim();
                        break;
                }
            }
        }

        private void SetMenuRights()
        {
            DataSet dsMenu = new DataSet();
            DataSet dsRights = new DataSet();
            this.pPApplRange = this.pPApplRange.Replace("^", "");
            string strSQL = "select padname,barname,range from com_menu where range =" + this.pPApplRange;
            dsMenu = _oDataAccess.GetDataSet(strSQL, null, 20);
            if (dsMenu != null)
            {
                if (dsMenu.Tables[0].Rows.Count > 0)
                {
                    string padName = "";
                    string barName = "";
                    padName = dsMenu.Tables[0].Rows[0]["padname"].ToString();
                    barName = dsMenu.Tables[0].Rows[0]["barname"].ToString();
                    strSQL = "select padname,barname,dbo.func_decoder(rights,'F') as rights from ";
                    strSQL += " userrights where padname ='" + padName.Trim() + "' and barname ='" + barName + "' and range = " + this.pPApplRange;
                    strSQL += " and dbo.func_decoder([user],'T') ='" + this.pAppUerName.Trim() + "'";
                }
            }
            dsRights = _oDataAccess.GetDataSet(strSQL, null, 20);

            if (dsRights != null)
            {
                string rights = dsRights.Tables[0].Rows[0]["rights"].ToString();
                int len = rights.Length;
                string newString = "";
                ArrayList rArray = new ArrayList();

                while (len > 2)
                {
                    newString = rights.Substring(0, 2);
                    rights = rights.Substring(2);
                    rArray.Add(newString);
                    len = rights.Length;
                }
                rArray.Add(rights);

                this.pAddButton = (rArray[0].ToString().Trim() == "IY" ? true : false);
                this.pEditButton = (rArray[1].ToString().Trim() == "CY" ? true : false);
                this.pDeleteButton = (rArray[2].ToString().Trim() == "DY" ? true : false);
                this.pPrintButton = (rArray[3].ToString().Trim() == "PY" ? true : false);
                this.pEditButton = true;        // To be Removed
            }
        }

        //***** Added by Sachin N. S. on 22/11/2018 for Pharma Retailer Changes -- Start
        private bool ValidateSettings(string cType)
        {
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            DataRow _dr;

            if (cType == "" || cType == "S")
            {
                cSql = "select Inv_sr from series where validity like '%" + this.txtEntry_ty.Text + "%' ";
                _dr = _oDataAccess.GetDataRow(cSql, null, 50);
                if (_dr != null && (this.chkSHWINVSR.Checked == false || this.txtCDEFAINVSR.Text==""))
                {
                    MessageBox.Show("Please select a valid Invoice Series.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.chkSHWINVSR.Checked = true;
                    return false;
                }
            }
            if (cType == "" || cType == "D")
            {

                cSql = "select Dept from Department where validity like '%" + this.txtEntry_ty.Text + "%' ";
                _dr = _oDataAccess.GetDataRow(cSql, null, 50);
                if (_dr != null && (this.chkSHWDEPT.Checked == false || this.txtCDEFADEPT.Text==""))
                {
                    MessageBox.Show("Please select a valid Department.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.chkSHWDEPT.Checked = true;
                    return false;
                }
            }
            if (cType == "" || cType == "C")
            {
                cSql = "select Cate from Category where validity like '%" + this.txtEntry_ty.Text + "%' ";
                _dr = _oDataAccess.GetDataRow(cSql, null, 50);
                if (_dr != null && (this.chkSHWCATE.Checked == false || this.txtCDEFACATE.Text==""))
                {
                    MessageBox.Show("Please select a valid Category.", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.chkSHWCATE.Checked = true;
                    return false;
                }
            }
            return true;
        }
        //***** Added by Sachin N. S. on 22/11/2018 for Pharma Retailer Changes -- End
    }
}
