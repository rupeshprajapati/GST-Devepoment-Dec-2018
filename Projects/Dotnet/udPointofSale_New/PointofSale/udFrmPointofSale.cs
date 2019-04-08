using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Reflection;    // Added by Sachin N. S. on 25/07/2013 for Bug-14538
using DataAccess_Net;
using System.Net.Mail;
using System.Net;

namespace PointofSale
{
    public partial class udFrmPointofSale : Form
    {
        clsDataAccess _oDataAccess;
        DataSet _commonDs;
        DataTable _dtSumDiscChrgs;      // Added By Sachin N. S. 27/05/2013 for Bug-14538
        ToolTip _toolTip;
        string pAppPath;
        string _keypress = "", _item = "";
        bool _barcodeRead = false;
        bool ldntValidate = false;
        string cAppName = "", cAppPId = "";
        string _taxExpr = "";           // Added By Sachin N. S. 17/07/2013 for Bug-14538
        string _colValue = "";          // Added By Sachin N. S. 17/07/2013 for Bug-14538
        bool _dgvDiscChrgsdataBind = false;
        decimal OrgHgth = 0M, OrgWdth = 0M;
        bool _setFocus = false;
        int _curRowNo = -1;             // Added by Sachin N. S. on 19/01/2016 for Bug-27503
        bool llgpuom = false;           //**** Added by Sachin N. S. on 28/09/2018 for Pharma Retailer Changes
        int maxRowId = 0;               //Added by Shrikant S. on 09/10/2018 
        string DBfilePath;              //Add by Rupesh G. on 19/10/2018

        public string PayMode;//Added by Rupesh G. on 29/11/2018 for Pharma Retailer Changes

        #region Default Constructor and Screen Load
        public udFrmPointofSale()
        {
            InitializeComponent();
            clsScreenPropEvents.OrgHgth = this.Height;
            clsScreenPropEvents.OrgWdth = this.Width;
            OrgHgth = this.Height;
            OrgWdth = this.Width;
        }

        private void udFrmPointofSale_Load(object sender, EventArgs e)
        {
            CultureInfo ci = new CultureInfo("en-GB");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            _toolTip = new ToolTip();


            this.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
            // Changed by Sachin N. S. on 06/08/2013 -- Start
            //this.txtUserName.Text = CommonInfo.UserName;
            //this.txtInvoiceSr.Text = CommonInfo.InvSeries;
            //this.txtCategory.Text = CommonInfo.Category;
            //this.txtDepartment.Text = CommonInfo.Department;

            this.tsUserName.Text = "User : " + CommonInfo.UserName;
            this.tsInvoiceSeries.Text = "Inv. Series : " + CommonInfo.InvSeries;
            this.tsCategory.Text = "Category : " + CommonInfo.Category;
            this.tsDepartment.Text = "Dept : " + CommonInfo.Department;
            // Changed by Sachin N. S. on 06/08/2013 -- End

            // Changed by Sachin N. S. on 06/08/2013 for Bug-27503 -- Start
            this.tsCounter.Text = "Counter : " + CommonInfo.CounterNm;
            // Changed by Sachin N. S. on 06/08/2013 for Bug-27503 -- End

            // Changed by Sachin N. S. on 03/11/2018 for Pharma Retailer Changes -- Start
            this.tsPoweredBy.Text = "Powered by Udyog Software (I) Ltd." + CommonInfo.CounterNm;
            // Changed by Sachin N. S. on 03/11/2018 for Pharma Retailer Changes -- End


            string appPath;
            appPath = Application.ExecutablePath;

            this.mInsertProcessIdRecord();

            appPath = Path.GetDirectoryName(appPath);
            if (string.IsNullOrEmpty(this.pAppPath))
            {
                this.pAppPath = appPath;
            }

            this.Add_Last_Deal_button();                    //Added by Shrikant S. on 01/11/2018 for Bug-31942  

            //*** Get Data Cursor -- Start ***\\
            getDataCursor();
            //*** Get Data Cursor -- End ***\\

            //*** Binding data to controls -- Start ***\\
            BindControls();
            //*** Binding data to controls -- End ***\\

            clsInsertDefaultValue._DataSet = _commonDs;

            //*** Adding Records to Dataset -- Start ***\\
            addHdrRecords();
            //*** Adding Records to Dataset -- End ***\\

            hideUnhideControls();           //*** Added by Sachin N. S. on 29/08/2013 for Bug-14538

            clsScreenPropEvents.NewHgth = this.Height;
            clsScreenPropEvents.NewWdth = this.Width;
            clsScreenPropEvents.ResizeForm(this);

            //**** Changed by Sachin N. S. on 06/08/2013 for Bug-14538 -- Start
            this.tsUserName.Height = (int)((this.Height * this.tsUserName.Height) / this.OrgHgth);
            this.tsUserName.Width = (int)((this.Width * this.tsUserName.Width) / this.OrgWdth);

            this.tsInvoiceSeries.Height = (int)((this.Height * this.tsInvoiceSeries.Height) / this.OrgHgth);
            this.tsInvoiceSeries.Width = (int)((this.Width * this.tsInvoiceSeries.Width) / this.OrgWdth);

            this.tsDepartment.Height = (int)((this.Height * this.tsDepartment.Height) / this.OrgHgth);
            this.tsDepartment.Width = (int)((this.Width * this.tsDepartment.Width) / this.OrgWdth);

            this.tsCategory.Height = (int)((this.Height * this.tsCategory.Height) / this.OrgHgth);
            this.tsCategory.Width = (int)((this.Width * this.tsCategory.Width) / this.OrgWdth);
            //**** Changed by Sachin N. S. on 06/08/2013 for Bug-14538 -- End

            //**** Changed by Sachin N. S. on 06/08/2013 for Bug-27503 -- End
            this.tsCounter.Height = (int)((this.Height * this.tsCounter.Height) / this.OrgHgth);
            this.tsCounter.Width = (int)((this.Width * this.tsCounter.Width) / this.OrgWdth);
            //**** Changed by Sachin N. S. on 06/08/2013 for Bug-27503 -- End

            //***** Changed By Sachin N. S. on 23/05/2013 for Bug-14538 -- Start *****//
            _dgvDiscChrgsdataBind = true;
            this.Refresh();
            //txtParty.Focus(); 
            //***** Changed by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- Start
            if (CommonInfo.EntryTy == "HS")
            {
                if (this.Controls[0].Controls[1].Controls.ContainsKey("txtPatientNm") == true)
                    this.Controls[0].Controls[1].Controls["txtPatientNm"].Focus();
            }
            else
            {
                //***** Changed by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- End
                dgvItemDetails.Focus();
                SendKeys.Send("{Tab}");
            }
            //***** Changed By Sachin N. S. on 23/05/2013 for Bug-14538 -- Start *****//
        }
        #endregion Default Constructor and Screen Load

        #region Control Events

        //******* Methods to Call Shortcut Keys -- Start *******\\
        private void udFrmPointofSale_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    this.addChldRecords("Item");
                    break;
                case Keys.F8:                           // Changed by Sachin N. S. on 03/12/2018 for Pharma Retailer Changes 
                    this.btnPayment.PerformClick();
                    break;
                case Keys.F5:
                    this.btnPrintInv.PerformClick();
                    break;
                case Keys.F12:
                    this.btnExit.PerformClick();
                    break;
                case Keys.F6:                           // Added by Sachin N. S. on 20/01/2016 for Bug-27503
                    this.CancelEntry();
                    break;
                case Keys.F11:                           // Added by Sachin N. S. on 27/11/2018 for Pharma Retailer Changes
                    System.Diagnostics.Process.Start("Calc");
                    break;
                case Keys.F10:                           // Added by Sachin N. S. on 27/11/2018 for Pharma Retailer Changes
                    this.callLastDeal();
                    break;
                case Keys.F7:
                    PayMode = "DirectCash";   //Added by Rupesh G. on 29/11/2018 for Pharma Retailer Changes
                    // Added by Sachin N. S. on 27/11/2018 for Pharma Retailer Changes
                    this.btnPayment.PerformClick();
                    break;
            }
        }

        private void udFrmPointofSale_FormClosed(object sender, FormClosedEventArgs e)
        {
            mDeleteProcessIdRecord();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (keyData == Keys.F1)
            //{
            //    MessageBox.Show("You pressed the F1 key");
            //    return true;    // indicate that you handled this keystroke
            //}
            //// Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }
        //******* Methods to Call Shortcut Keys -- End *******\\

        //****** Added by Sachin N. S. on 22/05/2013 for Bug-14538 -- Start ******//
        private void txtParty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                this.cmdParty.PerformClick();
            }
        }

        private void cmdParty_Click(object sender, EventArgs e)
        {
            if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count(row => row["Item"].ToString().Trim() != string.Empty) <= 0)
            {
                this.popUpFromTable("PARTY");
            }
            else
            {
                MessageBox.Show("Party cannot be changed as Goods have been selected.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtWareHouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                this.cmdWarehouse.PerformClick();
            }
            RefreshControls();
            dgvItemDetails.Focus();
            SendKeys.Send("{Tab}");
        }

        private void cmdWarehouse_Click(object sender, EventArgs e)
        {
            if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count(row => row["Item"].ToString().Trim() != string.Empty) <= 0)
            {
                this.popUpFromTable("WAREHOUSE");
            }
            else
            {
                MessageBox.Show((CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse") + " cannot be changed as Goods have been selected.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);    // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
            }

        }
        //****** Added by Sachin N. S. on 22/05/2013 for Bug-14538 -- End ******//

        private void dgvItemDetails_KeyDown(object sender, KeyEventArgs e)
        {
            string _orgColKeyDown = dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString();     // Added by Sachin N. S. on 20/11/2018 for Pharma Retailer Changes
            if (dgvItemDetails.RowCount > 0)
            {
                if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colItem")
                {
                    if (e.KeyCode == Keys.F2 && dgvItemDetails.CurrentRow.Cells[dgvItemDetails.CurrentCell.ColumnIndex].Value.ToString() == "")
                    {
                        string _caddItem = "";      // Added by Sachin N. S. on 10/11/2018 for Pharma Retailer Changes
                        popUpFromTable("ITEM");
                        if (this.dgvItemDetails.CurrentRow.Cells[1].Value.ToString() != "")     // ****** Added by Sachin N. S. on 20/08/2015 for Bug-26654
                        {
                            if (Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]) == true)     // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 1;
                            _keypress = "F2";

                            /// Changed by Sachin N. S. on 06/02/2014 for Rupesh Panhale's Query -- Start
                            //SendKeys.Send("{Tab}");       // Commented by Sachin N. S. on 06/02/2014 for Rupesh Requirement
                            if (CommonInfo.EntryTy != "HS")     // Added by Sachin N. S. on 10/11/2018 for Pharma Retailer Changes
                                this.addChldRecords("Item");
                            this.dgvItemDetails.Refresh();
                            _caddItem = "Add";                  // Added by Sachin N. S. on 10/11/2018 for Pharma Retailer Changes
                        }
                        _item = "";
                        /// Changed by Sachin N. S. on 06/02/2014 for Rupesh Panhale's Query -- End

                        //addChldRecords("Item");

                        // ******* Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- Start
                        int _iRwPos = _curRowNo;        // Added by Sachin N. S. on 19/01/2016 for Bug-27503
                        if (_iRwPos >= 0 && Convert.ToInt64(_commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"]) != 0)
                        {
                            //***** Added by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes -- Start
                            if (CommonInfo.EntryTy == "HS" && dgvItemDetails.Columns.Contains("COLBATCHNO") == true && _caddItem == "Add")
                            {
                                this.getBatchNo();
                                this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells["COLBATCHNO"];
                            }
                            else
                            {
                                //***** Added by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes -- End
                                string cSql = "Select AskQty,AskRate from It_Mast where It_Code=" + _commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"].ToString();       // Added by Sachin N. S. on 18/01/2016 for Bug-27503

                                DataTable _dt;
                                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                                if (Convert.ToBoolean(_dt.Rows[0]["AskQty"]) == true)
                                {
                                    this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                    dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells[2];
                                }
                                else
                                {
                                    if (Convert.ToBoolean(_dt.Rows[0]["AskRate"]) == true)
                                    {
                                        this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                        dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells[3];
                                    }
                                }
                            }   //***** Added by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes 
                        }
                        // ******* Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- End
                    }

                    if (e.KeyCode == Keys.Enter || (e.Control == true && e.KeyCode == Keys.V))
                    {
                        //MessageBox.Show("ENTER");
                        _keypress = "ENTER";
                        //if (dgvItemDetails.CurrentRow.Cells[dgvItemDetails.CurrentCell.ColumnIndex].Value.ToString() != "")
                        //{
                        //    bool llAdd = false;
                        //    llAdd = ReadBarcode(dgvItemDetails.CurrentRow.Cells[dgvItemDetails.CurrentCell.ColumnIndex].Value.ToString());
                        //    if (llAdd == true)
                        //    {
                        //        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 1;
                        //        SendKeys.Send("{Tab}");
                        //    }
                        //    else
                        //    {
                        //        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = "";
                        //    }
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Barcode cannot be empty.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //}
                    }
                }
                // ****** Added by Sachin N. S. on 17/07/2013 for Bug-14538 -- Start ******//
                if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colTax_Name")
                {
                    if (e.KeyCode == Keys.F2)
                    {
                        popUpFromTable("SALESTAX");
                    }
                }
                // ****** Added by Sachin N. S. on 17/07/2013 for Bug-14538 -- End ******//

                // ****** Added by Sachin N. S. on 21/08/2015 for Bug-26654 -- Start ******//
                if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colDelete")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        int _rowIndex = dgvItemDetails.CurrentCell.RowIndex;
                        int _srNo = Convert.ToInt16(_commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentCell.RowIndex]["Item_No"]);
                        _commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentCell.RowIndex].Delete();
                        _commonDs.Tables["Item"].AcceptChanges();
                        if (dgvItemDetails.RowCount > 0 && _rowIndex <= dgvItemDetails.CurrentCell.RowIndex)
                        {
                            for (int i = dgvItemDetails.CurrentCell.RowIndex; i < _commonDs.Tables["Item"].Rows.Count; i++)
                            {
                                _commonDs.Tables["Item"].Rows[i]["Item_No"] = _srNo.ToString();
                                _srNo += 1;
                            }
                        }
                    }
                }
                // ****** Added by Sachin N. S. on 21/08/2015 for Bug-26654 -- End ******//

                //Added by Shrikant S. on 01/11/2018 for Bug-31942      //Start

                if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colItemAddInfo")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        int iColumn = dgvItemDetails.CurrentCell.ColumnIndex;
                        int iRow = dgvItemDetails.CurrentCell.RowIndex;

                        _commonDs.Tables["ITEM"].AcceptChanges();

                        udAdditionalInfo.udAdditionalInfo _udAdditionalInfo = new udAdditionalInfo.udAdditionalInfo();
                        _udAdditionalInfo._commonDs = _commonDs;
                        _udAdditionalInfo._EntryTy = CommonInfo.EntryTy;
                        _udAdditionalInfo._HdrDtl = "D";
                        _udAdditionalInfo._TblName = "ITEM";
                        _udAdditionalInfo._FilterRowId = this.dgvItemDetails.Rows[dgvItemDetails.CurrentCell.RowIndex].Cells["ItemRowId"].Value.ToString();
                        _udAdditionalInfo.callAdditionalInfo();

                        dgvItemDetails.CurrentCell = dgvItemDetails[iColumn, iRow];         //Shrikant S. on 05/11/2018 Added

                        //if (iColumn == dgvItemDetails.Columns.Count - 1)
                        //    dgvItemDetails.CurrentCell = dgvItemDetails[0, iRow];
                        //else
                        //    dgvItemDetails.CurrentCell = dgvItemDetails[iColumn, iRow];

                        return;
                    }
                }

                if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colLastDeal")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        frmLastDeal lstDeal = new frmLastDeal();
                        lstDeal.oDataAccess = _oDataAccess;
                        lstDeal.EntryType = _commonDs.Tables["Main"].Rows[0]["Entry_ty"].ToString();
                        lstDeal.Itemcode = Convert.ToInt32(_commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentCell.RowIndex]["It_code"]);
                        lstDeal.AcId = Convert.ToInt32(_commonDs.Tables["Main"].Rows[0]["Ac_id"]);      //02/11/2018 for Bug-31942
                        lstDeal.Width = this.Width - 200;
                        lstDeal.Top = (this.Height - lstDeal.Height) / 2;
                        lstDeal.Left = (this.Width - lstDeal.Width) / 2;
                        lstDeal.Icon = this.Icon;
                        lstDeal.ShowDialog();
                        e.SuppressKeyPress = true;
                        return;
                    }
                }
                //Added by Shrikant S. on 01/11/2018 for Bug-31942      //End

                //Added by Shrikant S. on 03/10/2018        //Start
                if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].ValueType != null && (e.KeyCode == Keys.F2))
                {
                    if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].ValueType.ToString().ToLower() == "system.string")
                    {
                        if (_orgColKeyDown == dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString())       // Added by Sachin N. S. on 10/11/2018 for Bug-31943 
                        {
                            int col = dgvItemDetails.CurrentCell.ColumnIndex;
                            udDGVStringColumn column = dgvItemDetails.Columns[col] as udDGVStringColumn;
                            if (column != null)
                            {
                                if (column.Filtcond.Trim().Length > 0)
                                {
                                    DataTable searchTbl = new DataTable();

                                    udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
                                    string cFrmCap = "Select ", cSrchCol = "", cDispCol = "", cRetCol = "";
                                    if (column.Filtcond.ToLower().Contains("execute ") || column.Filtcond.ToLower().Contains("select ") || column.Filtcond.ToLower().Contains("exec "))
                                    {
                                        cFrmCap = cFrmCap + dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].HeaderText.Trim();
                                        string csql = (column.Filtcond.Trim().IndexOf("{") > 0 ? column.Filtcond.Trim().Substring(0, column.Filtcond.Trim().IndexOf("{") - 1) : column.Filtcond.Trim());

                                        //**** Added by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes -- Start
                                        if (column.Filtcond.ToLower().Contains("execute "))
                                            csql = this.getExecSqlString(csql);
                                        //**** Added by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes -- End

                                        searchTbl = _oDataAccess.GetDataTable(csql, null, 25);

                                        cSrchCol = searchTbl.Columns[0].ColumnName;
                                        cRetCol = searchTbl.Columns[0].ColumnName;
                                        cDispCol = searchTbl.Columns[0].ColumnName;

                                        if (column.Filtcond.Trim().IndexOf("{") > 0)
                                        {
                                            string filtcond = column.Filtcond.Trim().Substring(column.Filtcond.Trim().IndexOf("{") + 1, (column.Filtcond.Trim().Trim().Length - column.Filtcond.Trim().IndexOf("{") - 2));
                                            string[] vals = filtcond.Trim().Split('#');
                                            switch (vals.Length)
                                            {
                                                case 1:
                                                    cSrchCol = vals[0];
                                                    break;
                                                case 2:
                                                    cSrchCol = vals[0];
                                                    cRetCol = vals[1];
                                                    break;
                                                case 4:
                                                    cSrchCol = vals[0];
                                                    cRetCol = vals[1];
                                                    cDispCol = vals[3];
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        _oSelPop.pformtext = cFrmCap;
                                        _oSelPop.psearchcol = cSrchCol;
                                        _oSelPop.pDisplayColumnList = cDispCol;
                                        _oSelPop.pRetcolList = cRetCol;
                                    }
                                    else
                                    {
                                        string[] valList = column.Filtcond.Trim().Split(',');
                                        foreach (var item in valList)
                                        {
                                            DataRow row = searchTbl.NewRow();
                                            row[0] = item;
                                            searchTbl.Rows.Add(row);
                                        }
                                    }
                                    DataView _dvw = searchTbl.DefaultView;
                                    _oSelPop.pdataview = _dvw;
                                    _oSelPop.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
                                    _oSelPop.ShowDialog();

                                    if (_oSelPop.pReturnArray != null)
                                    {
                                        string fldnm = column.Fldnm.Trim();
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm] = _oSelPop.pReturnArray[0].ToString();
                                        //**** Added by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes -- Start
                                        if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString().ToUpper() == "COLBATCHNO")
                                        {
                                            string csql = "SELECT BatchNo, MfgDt, ExpDt, BatchRate, Inc_gst  FROM pRetBatchDetails WHERE it_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString() + " and BatchonHold = 0 and BatchNo='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm].ToString().Trim() + "'";
                                            searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                                            if (searchTbl.Rows.Count > 0)
                                            {
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();
                                                //if (Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]) == true)
                                                //{
                                                //    decimal _npertnm = 0.00M, _OrgRate = 0.00M;
                                                //    _npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["CGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["SGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["IGST_PER"]);
                                                //    _OrgRate = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                                //    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_OrgRate * 100) / (100 + _npertnm);
                                                //    if (this.llgpuom == true)
                                                //    {
                                                //        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = Math.Round((_OrgRate * 100) / (100 + _npertnm), 2);
                                                //    }
                                                //}
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                                                itemwiseTotal(this.BindingContext[_commonDs, "Item"].Position, 0);
                                            }
                                        }
                                        //**** Added by Sachin N. S. on 26/10/2018 for Pharma Retailer Changes -- End
                                    }
                                }
                            }
                        }
                    }
                }
                //Added by Shrikant S. on 03/10/2018        //End

                // ****** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- Start ******//
                if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colgpunit")
                {
                    if (e.KeyCode == Keys.F2 && dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].ReadOnly == false)
                    {
                        popUpFromTable("GROUPUNIT");
                    }
                }
                // ****** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- End ******//
            }

            
            RefreshControls();
            _keypress = "";       // Added by Sachin N. S. on 26/08/2015 for Bug-26654
            //dgvItemDetails.Refresh();
        }

        private void dgvItemDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colQuantity" || (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2") || dgvItemDetails.Columns[e.ColumnIndex].Name == "colRate")   // Changed by Sachin N. S. on 19/01/2016 for Bug-27503
            {
                _commonDs.Tables["Item"].Rows[e.RowIndex].EndEdit();
                this.RefreshControls();

                if (dgvItemDetails.Columns[e.ColumnIndex].Name.ToString() == "colQuantity" || (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2") || dgvItemDetails.Columns[e.ColumnIndex].Name == "colRate")   // Changed by Sachin N. S. on 19/01/2016 for Bug-27503
                {
                    string cSql = "";
                    int _iRwPos = _curRowNo;      // Added by Sachin N. S. on 18/01/2016 for Bug-27503
                    if (_iRwPos >= 0)
                    {
                        cSql = "Select AskQty,AskRate from It_Mast where It_Code=" + _commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"].ToString();       // Added by Sachin N. S. on 18/01/2016 for Bug-27503
                    }

                    // ******* Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- Start
                    if (_iRwPos >= 0 && Convert.ToInt64(_commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"]) != 0)
                    {
                        //***** Added by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes -- Start
                        if (CommonInfo.EntryTy == "HS" && dgvItemDetails.Columns.Contains("COLBATCHNO") == true && e.RowIndex == _curRowNo)
                        {
                            this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                            dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells["COLBATCHNO"];
                        }
                        else
                        {
                            DataTable _dt;
                            _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                            if (Convert.ToBoolean(_dt.Rows[0]["AskQty"]) == true && (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2"))
                            {
                                this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells[2];
                                //this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                            }
                            else
                            {
                                if (Convert.ToBoolean(_dt.Rows[0]["AskRate"]) == true && (dgvItemDetails.Columns[e.ColumnIndex].Name == "colQuantity" || (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2")))
                                {
                                    this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                    dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells[3];
                                    //this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                }
                                else
                                {
                                    if (CommonInfo.EntryTy != "HS")     // Added by Sachin N. S. on 10/11/2018 for Bug-31943 
                                        this.addChldRecords("Item");
                                }
                            }
                        }
                    }
                    // ******* Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- End

                    this.dgvItemDetails.Refresh();
                    _item = "";
                    _barcodeRead = false;
                    _setFocus = true;
                }
            }
            else        //********* Added by Sachin N. S. on 26/06/2013 for Bug-14538 -- Start *********//
            {
                _commonDs.Tables["Item"].Rows[e.RowIndex].EndEdit();
                this.dgvItemDetails.Refresh();
                this.RefreshControls();
            }           //********* Added by Sachin N. S. on 26/06/2013 for Bug-14538 -- End *********//
        }

        private void dgvItemDetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (ldntValidate == true) { return; }
            if (dgvItemDetails.Rows[e.RowIndex].IsNewRow == true) { return; }
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colQuantity")
            {
                if (Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]) == 0)
                { return; }
                // **** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- Start **** //
                //if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]) == 0)
                if (Convert.ToDecimal(e.FormattedValue) == 0)
                { return; }
                // **** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- End **** //

                if (Stock_Checking(Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(e.FormattedValue), e.RowIndex) < 0)
                {
                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                    dgvItemDetails.CancelEdit();
                    MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                }
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2")
            {
                _keypress = "";
                if (_barcodeRead == true)
                    return;

                if (e.FormattedValue.ToString() != "" && _item != e.FormattedValue.ToString())
                {
                    string clAdd = "ADD";
                    //llAdd = ReadBarcode(dgvItemDetails.CurrentRow.Cells[dgvItemDetails.CurrentCell.ColumnIndex].Value.ToString());
                    //clAdd = ReadBarcode(e.FormattedValue.ToString());
                    clAdd = ReadBarcodeGen(e.FormattedValue.ToString());       // Changed by Sachin N. S. on 01/12/2018 for Pharma Retailer Changes
                    if (clAdd == "ADD" && e.RowIndex == this.BindingContext[_commonDs, "Item"].Position)
                    {
                        //MessageBox.Show("Validating : "+this.BindingContext[_commonDs, "Item"].Position.ToString());
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 1;
                        //SendKeys.Send("{Tab}");
                        _barcodeRead = true;
                    }
                    else
                    {
                        if (clAdd == "EXIST")
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = "";
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                            dgvItemDetails.CancelEdit();
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = "";
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                            //int i = dgvItemDetails.CurrentRow.Index;
                            //dgvItemDetails.CurrentCell = dgvItemDetails.Rows[i].Cells[1];
                            dgvItemDetails.CancelEdit();
                            SendKeys.Send("{Esc}");
                            e.Cancel = true;
                        }
                    }
                }
            }

            //***** Added by Sachin N. S. on 24/10/2018 for Pharma Retailer Changes -- Start
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colPhmFreeQty")
            {
                if (Convert.ToDecimal(e.FormattedValue) > Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]))
                {
                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["phmfreeqty"] = _colValue;
                    dgvItemDetails.CancelEdit();
                    MessageBox.Show("Free quantity cannot be more than the Stock quantity.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                }
            }
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colBATCHNO" && _keypress != "F2")
            {
                if (e.FormattedValue.ToString() != "")
                {
                    string cSrchCol = "", cDispCol = "", cRetCol = "", cFrmCap = "";
                    cSrchCol = "BatchNo,MfgDt,ExpDt,BatchRate,Inc_gst";
                    cDispCol = "BatchNo:Batch No.,MfgDt:Mfg Dt.,ExpDt: Exp. Dt.,BatchRate:Batch Rate,Inc_gst: Incl GST";
                    cRetCol = "BatchNo,MfgDt,ExpDt,BatchRate,Inc_gst";

                    string csql = "SELECT BatchNo, MfgDt, ExpDt, BatchRate, Inc_gst  FROM pRetBatchDetails WHERE it_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString() + " and BatchonHold = 0 and BatchNo like '" + e.FormattedValue.ToString().Trim() + "%'";
                    DataTable searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                    if (searchTbl.Rows.Count > 0)
                    {
                        if (searchTbl.Rows.Count > 1)
                        {
                            cFrmCap = "Select Batch";

                            udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
                            DataView _dvw = searchTbl.DefaultView;

                            _oSelPop.pdataview = _dvw;
                            _oSelPop.pformtext = cFrmCap;
                            _oSelPop.psearchcol = cSrchCol;
                            _oSelPop.pDisplayColumnList = cDispCol;
                            _oSelPop.pRetcolList = cRetCol;
                            _oSelPop.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
                            _oSelPop.ShowDialog();

                            if (_oSelPop.pReturnArray != null)
                            {
                                DataTable _dtNew = searchTbl.Select("It_Code=" + _oSelPop.pReturnArray[0].ToString().Trim()).CopyToDataTable();
                                searchTbl = _dtNew;
                            }
                        }
                        // ****** Added by Sachin N. S. on 26/09/2018 for New Pharma Retailer Changes -- End

                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = searchTbl.Rows[0]["BatchNo"].ToString();
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                        itemwiseTotal(this.BindingContext[_commonDs, "Item"].Position, 0);
                    }
                    else
                    {
                        MessageBox.Show("Batch No. not in the Batch Master.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            if (Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]) == false)
            {
                if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colgpqty")
                {
                    if (Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]) == 0)
                    { return; }
                    if (Convert.ToDecimal(e.FormattedValue) == 0)
                    { return; }

                    decimal _stkQty = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]);
                    _commonDs.Tables["Item"].Rows[e.RowIndex]["GpQty"] = Convert.ToDecimal(e.FormattedValue);
                    this.Update_Conv_Qty("GRPQTY", e.RowIndex);

                    if (Stock_Checking(Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["PhmLoseQty"]), e.RowIndex) < 0)
                    {
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpQty"] = Convert.ToDecimal(_colValue);
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = _stkQty;
                        dgvItemDetails.CancelEdit();
                        MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Cancel = true;
                    }
                }
            }
            //***** Added by Sachin N. S. on 24/10/2018 for Pharma Retailer Changes -- End
            _keypress = "";
        }

        private void dgvItemDetails_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Added by Shrikant S. on 01/10/2018            //Start
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItemAddInfo")
            {
                return;
            }

            //Added by Shrikant S. on 01/10/2018            //End
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem")
            {
                _item = dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            //            dgvItemDetails.Columns[e.ColumnIndex].Tag = dgvItemDetails.Columns[e.ColumnIndex].Tag == null || Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag) == string.Empty ? dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value : Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag).Substring(0, 2) + dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();     // Added by Sachin N. S. on 24/07/2013 for Bug-14538
            _colValue = dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();     // Added by Sachin N. S. on 24/07/2013 for Bug-14538
        }

        private void dgvItemDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            //Added by Shrikant S. on 01/10/2018            //Start
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItemAddInfo")
            {
                int iColumn = dgvItemDetails.CurrentCell.ColumnIndex;               //Added by Shrikant S. on 01/11/2018 for Bug-31942
                int iRow = dgvItemDetails.CurrentCell.RowIndex;                     //Added by Shrikant S. on 01/11/2018 for Bug-31942
                //int currentRowIndex = e.RowIndex;
                _commonDs.Tables["ITEM"].AcceptChanges();

                udAdditionalInfo.udAdditionalInfo _udAdditionalInfo = new udAdditionalInfo.udAdditionalInfo();
                _udAdditionalInfo._commonDs = _commonDs;
                _udAdditionalInfo._EntryTy = CommonInfo.EntryTy;
                _udAdditionalInfo._HdrDtl = "D";
                _udAdditionalInfo._TblName = "ITEM";
                _udAdditionalInfo._FilterRowId = this.dgvItemDetails.Rows[e.RowIndex].Cells["ItemRowId"].Value.ToString();
                _udAdditionalInfo.callAdditionalInfo();

                //this.dgvItemDetails.Rows[currentRowIndex].Selected = true;

                dgvItemDetails.CurrentCell = dgvItemDetails[iColumn, iRow];         //Shrikant S. on 05/11/2018 Added

                //Added by Shrikant S. on 01/11/2018 for Bug-31942          //Start
                //if (iColumn == dgvItemDetails.Columns.Count - 1)
                //    dgvItemDetails.CurrentCell = dgvItemDetails[0, iRow + 1];
                //else
                //    dgvItemDetails.CurrentCell = dgvItemDetails[iColumn, iRow];
                //Added by Shrikant S. on 01/11/2018 for Bug-31942          //End
                return;
            }
            //Added by Shrikant S. on 01/10/2018            //End

            //Added by Shrikant S. on 01/11/2018 for Bug-31942           //Start
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colLastDeal")
            {
                if (e.RowIndex >= 0)
                {
                    frmLastDeal lstDeal = new frmLastDeal();
                    lstDeal.oDataAccess = _oDataAccess;
                    lstDeal.EntryType = _commonDs.Tables["Main"].Rows[0]["Entry_ty"].ToString();
                    lstDeal.Itemcode = Convert.ToInt32(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]);
                    lstDeal.AcId = Convert.ToInt32(_commonDs.Tables["Main"].Rows[0]["Ac_id"]);
                    lstDeal.Width = this.Width - 200;
                    lstDeal.Top = (this.Height - lstDeal.Height) / 2;
                    lstDeal.Left = (this.Width - lstDeal.Width) / 2;
                    lstDeal.Icon = this.Icon;
                    lstDeal.ShowDialog();
                    return;
                }
            }
            //Added by Shrikant S. on 01/11/2018 for Bug-31942           //End

            //**** Added by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes -- Start
            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["GSTIncl"]) == true)
            {
                if (e.ColumnIndex == dgvItemDetails.Columns["colGSTIncl"].Index && e.RowIndex >= 0)
                {
                    this.dgvItemDetails.EndEdit();
                    itemwiseTotal(e.RowIndex, 0);
                    dgvItemDetails.CurrentCell = dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
            }
            //**** Added by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes -- End

            this.ldntValidate = true;
            if (e.ColumnIndex == dgvItemDetails.Columns["colDelete"].Index && e.RowIndex >= 0)
            {
                int _srNo = Convert.ToInt16(_commonDs.Tables["Item"].Rows[e.RowIndex]["Item_No"]);
                _commonDs.Tables["Item"].Rows[e.RowIndex].Delete();
                _commonDs.Tables["Item"].AcceptChanges();
                for (int i = e.RowIndex; i < _commonDs.Tables["Item"].Rows.Count; i++)
                {
                    _commonDs.Tables["Item"].Rows[i]["Item_No"] = _srNo.ToString();
                    _srNo += 1;
                }
            }
            this.RefreshControls();
            this.ldntValidate = false;
        }

        private void dgvItemDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex == dgvItemDetails.Columns["colDelete"].Index && e.RowIndex >= 0)
            //{
            //    Control control = dgvItemDetails;

            //    while (control != null)
            //    {
            //        control.CausesValidation = false;
            //        control = control.Parent;
            //    }
            //}
        }

        //****** Added by Sachin N. S. on 24/07/2013 for Bug-14538 -- Start ******//
        private void dgvItemDetails_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (ldntValidate == true) return;

            if (dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Qty" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Rate" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Item" || Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag) == "V#" || Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag) == "%#" || Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag) == "S#" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "phmfreeqty" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "gpqty" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "PhmLoseQty")     // Changed by Sachin N. S. on 24/10/2018 for Pharma Retailer Changes
            {
                if (_colValue != dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                {
                    dgvItemDetails.EndEdit();                        // Changed by Sachin N. S. on 15/05/2017 for GST
                    //**** Added by Sachin N. S. on 17/10/2018 for Pharma Retailer -- Start
                    if (dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Qty")
                    {
                        this.Update_Conv_Qty("QTY", e.RowIndex);
                    }
                    //**** Added by Sachin N. S. on 17/10/2018 for Pharma Retailer -- End
                    itemwiseTotal(e.RowIndex, e.ColumnIndex);        // Changed by Sachin N. S. on 15/01/2016 for Bug-27503
                }
            }
        }

        private void dgvItemDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (Convert.ToString(dgvItemDetails.CurrentCell.OwningColumn.Tag) == "S#")
            {
                ComboBox _cb = e.Control as ComboBox;
                if (_cb != null)
                {
                    _cb.SelectionChangeCommitted -= new
                     EventHandler(cbSalesTax_SelectionChangeCommitted);

                    _cb.SelectionChangeCommitted += new
                      EventHandler(cbSalesTax_SelectionChangeCommitted);
                }
            }
        }

        private void cbSalesTax_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvItemDetails.EndEdit();
            itemwiseTotal(dgvItemDetails.CurrentCell.RowIndex, dgvItemDetails.CurrentCell.ColumnIndex);      // Changed by Sachin N. S. on 15/01/2016 for Bug-27503
        }

        //private void cbSalesTax_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    dgvItemDetails.EndEdit();
        //    itemwiseTotal(dgvItemDetails.CurrentCell.ColumnIndex);
        //}
        //****** Added by Sachin N. S. on 24/07/2013 for Bug-14538 -- End ******//

        //private void dgvItemDetails_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    if (e.ColumnIndex == dgvItemDetails.Columns["colDelete"].Index && e.RowIndex >= 0)
        //    {
        //        Control control = dgvItemDetails;

        //        while (control != null)
        //        {
        //            control.CausesValidation = false;
        //            control = control.Parent;
        //        }
        //        this.AutoValidate = AutoValidate.Disable;
        //    }
        //}

        private void dgvDiscChrgs_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (ldntValidate == true) return;

            if (_dgvDiscChrgsdataBind == true)      // Added by Sachin N. S. on 18/07/2013 for Bug-14538
            {
                if (e.ColumnIndex == 0 && e.RowIndex != -1)
                {
                    if (_commonDs.Tables["HdDiscChrgs"].Rows[e.RowIndex]["Code"].ToString() == "S")
                    {
                        dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;
                    }
                    else
                    {
                        dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
                    }
                }

                if (e.ColumnIndex != 0 && e.RowIndex != -1)
                {
                    if (_commonDs.Tables["HdDiscChrgs"].Rows[e.RowIndex]["Code"].ToString() == "S")
                    {
                        if (_commonDs.Tables["HdDiscChrgs"].Rows[e.RowIndex]["Head_Nm"].ToString() == "NO-TAX")
                        {
                            dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
                        }
                        else
                        {
                            dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;
                        }
                    }
                    else
                    {
                        dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;
                    }
                }
                _colValue = dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();     // Added by Sachin N. S. on 14/01/2016 for Bug-27503
            }
        }

        private void dgvDiscChrgs_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex !=0)
            // Added by Sachin N. S. on 14/01/2016 for Bug-27503 -- Start
            if (_colValue != dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
            {
                if (e.ColumnIndex == 2)
                {
                    _commonDs.Tables["HdDiscChrgs"].Rows[e.RowIndex]["Def_Pert"] = 0;
                }
                // Added by Sachin N. S. on 14/01/2016 for Bug-27503 -- End
                this.CalculateHeaderTax(_commonDs.Tables["HdDiscChrgs"].Rows[e.RowIndex]["Fld_Nm"].ToString());
                this.RefreshControls();
            }
        }

        //******* Added by Sachin N. S. on 20/06/2013 for Bug-14538 ******* Start
        private void dgvDiscChrgs_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvDiscChrgs.RowCount > 0)
            {
                if (dgvDiscChrgs.Columns[dgvDiscChrgs.CurrentCell.ColumnIndex].Name.ToString() == "colHeading")
                {
                    if (e.KeyCode == Keys.F2 && _commonDs.Tables["HdDiscChrgs"].Rows[dgvDiscChrgs.CurrentCell.RowIndex]["Code"].ToString() == "S")
                    {
                        popUpFromTable("SALESTAX");
                    }
                }
            }
        }

        private void btnAddInfo_Click(object sender, EventArgs e)
        {
            _commonDs.Tables["MAIN"].AcceptChanges();
            udAdditionalInfo.udAdditionalInfo _udAdditionalInfo = new udAdditionalInfo.udAdditionalInfo();
            _udAdditionalInfo._commonDs = _commonDs;
            _udAdditionalInfo._EntryTy = CommonInfo.EntryTy;    // Changed by Sachin N. S. on 27/09/2018 for Pharma Retailer Changes
            //_udAdditionalInfo._EntryTy = "PS";
            _udAdditionalInfo._HdrDtl = "H";
            _udAdditionalInfo._TblName = "MAIN";
            _udAdditionalInfo.callAdditionalInfo();
        }
        //******* Added by Sachin N. S. on 20/06/2013 for Bug-14538 ******* End

        private void btnPrintInv_Click(object sender, EventArgs e)
        {
            popUpFromTable("PRINTBILL");
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            //Added by Rupesh G. on 03/12/2018 for Pharma Retailer Changes--Start
            bool chkprintBill;
            chkprintBill = chkPrintBill.Checked;
            //Added by Rupesh G. on 03/12/2018 for Pharma Retailer Changes--End

            if (_commonDs.Tables["Main"].Rows.Count > 0)
            {
                this.BindingContext[_commonDs, "Main"].EndCurrentEdit();

                //**** Added by Sachin N. S. on 14/06/2014 for Bug-14538 -- Start
                //if (Convert.ToDecimal(this.txtNetAmt.Text) <= 0)
                if (Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Net_Amt"]) <= 0)    // Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                {
                    MessageBox.Show("No records found for payment.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //**** Added by Sachin N. S. on 14/06/2014 for Bug-14538 -- End

                this.DeleteChldRecords("Item");
                _commonDs.Tables["Main"].AcceptChanges();
                _commonDs.Tables["Item"].AcceptChanges();

                if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count(row => row["Item"].ToString().Trim() == string.Empty) > 0)
                {
                    MessageBox.Show("Goods name cannot be blank.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SendKeys.Send("{Esc}");
                    return;
                }

                //***** Added by Sachin N. S. on 25/10/2018 for Pharma Retailer Changes -- Start
                //if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["gpuomapp"]) == true)
                //{
                //    if (_commonDs.Tables["Lother"].Select("E_code='" + CommonInfo.EntryTy + "' and att_file=False and ingrid=True").Count() >= 0)
                //    {
                //        if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["BatchNo"] != null).Count(row => row["BatchNo"].ToString().Trim() == string.Empty) > 0)
                //        {
                //            MessageBox.Show("Some Items Batch no are not selected. Cannot Continue...!!!", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //            return;
                //        }
                //    }
                //}
                //***** Added by Sachin N. S. on 25/10/2018 for Pharma Retailer Changes -- End

                //if (Convert.ToDecimal(this.txtNetAmt.Text) > 0)
                if (Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Net_Amt"]) > 0)     // Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                {

                    //Comment by Rupesh G. on 03/12/2018 for Pharma Retailer Changes
                    // udFrmPaymentScreen _frmPayScrn = new udFrmPaymentScreen(_commonDs);

                    //Add  by Rupesh G. on 03/12/2018 for Pharma Retailer Changes
                    udFrmPaymentScreen _frmPayScrn = new udFrmPaymentScreen(_commonDs, PayMode, chkprintBill);

                    _frmPayScrn.ShowDialog();
                    if (_frmPayScrn._RetValue == "SAVED")
                    {
                        //Comment by Rupesh G. on 03/12/2018 for Pharma Retailer Changes
                        //  PrintBill(Convert.ToInt16(_commonDs.Tables["Main"].Rows[0]["Tran_cd"]));

                        //Add  by Rupesh G. on 03/12/2018 for Pharma Retailer Changes--Start
                        bool chkPrintSetting = Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["LASKPRINT"]);

                        if (chkPrintSetting && chkprintBill)
                        {
                            if (MessageBox.Show("Do you want to Print ?", CommonInfo.AppCaption,
               MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                if (chkprintBill && _frmPayScrn._PrintValue == "PRINT")
                                {
                                    PrintBill(Convert.ToInt64(_commonDs.Tables["Main"].Rows[0]["Tran_cd"]));
                                }
                            }
                        }
                        else
                        {
                            if (chkprintBill && _frmPayScrn._PrintValue == "PRINT")
                            {
                                PrintBill(Convert.ToInt64(_commonDs.Tables["Main"].Rows[0]["Tran_cd"]));
                            }
                        }

                        //Add  by Rupesh G. on 03/12/2018 for Pharma Retailer Changes--End

                        Export_CSV_Email();//Add Rupesh G. on 19/10/2018
                        clearTables();
                        addHdrRecords();
                        //**** Added by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- Start
                        if (CommonInfo.EntryTy == "HS")
                        {
                            this.Controls[0].Controls[1].Controls["txtPatientNm"].Focus();
                        }
                        else
                        {
                            //**** Added by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- End
                            dgvItemDetails.Focus();
                            SendKeys.Send("{Tab}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No records found for payment.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.RefreshControls();
            }
            else
            {
                MessageBox.Show("No records found for payment.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            PayMode = "";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.ldntValidate = true;
            if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count(row => row["Item"].ToString().Trim() != string.Empty) > 0)
            {
                if (MessageBox.Show("The data will not be saved if clicked on Exit.\nDo you want still want to Exit?", CommonInfo.AppCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    this.ldntValidate = false;
                    return;
                }
            }
            //**** Added by Sachin N. S. on 03/12/2018 for Pharma Retailer Changes -- Start
            else
            {
                if (MessageBox.Show("You are about to close the POS Screen.\nDo you want still want to Exit?", CommonInfo.AppCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    this.ldntValidate = false;
                    return;
                }
            }
            //**** Added by Sachin N. S. on 03/12/2018 for Pharma Retailer Changes -- End
            this.Close();
        }

        private void btnPrintInv_MouseHover(object sender, EventArgs e)
        {
            _toolTip.Show("Reprint Bill", btnPrintInv);
        }

        private void btnPayment_MouseHover(object sender, EventArgs e)
        {
            _toolTip.Show("Pay Bill", btnPayment);
        }

        private void btnExit_MouseHover(object sender, EventArgs e)
        {
            _toolTip.Show("Exit POS", btnExit);
        }
        #endregion Control Events

        #region Private Methods

        //Added by Shrikant S. on 01/11/2018 for Bug-31942          //Start
        private void Add_Last_Deal_button()
        {
            DataGridViewButtonColumn colLastDeal = new DataGridViewButtonColumn();
            colLastDeal.HeaderText = "History";
            colLastDeal.Name = "colLastDeal";
            colLastDeal.Text = "History";
            colLastDeal.UseColumnTextForButtonValue = true;
            colLastDeal.Width = 90;
            this.dgvItemDetails.Columns.Add(colLastDeal);
        }
        //Added by Shrikant S. on 01/11/2018 for Bug-31942          //End


        #region Build dataset and Refresh controls
        private void getDataCursor()
        {
            _oDataAccess = new clsDataAccess();
            //List<clsParam> _clparam = new List<clsParam>();
            //_clparam.Add(new clsParam { FieldName = "@Entry_ty1", Value="PS"});
            //_clparam.Add(new clsParam { FieldName = "@Tran_cd", Value = 0 });
            _commonDs = new DataSet();

            //_commonDs = _oDataAccess.GetDataSet("Execute USP_GET_PS_DATASET 'PS',0", null, 25);
            _commonDs = _oDataAccess.GetDataSet("Execute USP_GET_PS_DATASET '" + CommonInfo.EntryTy + "',0", null, 25);         // Changed by Sachin N. S. on 27/09/2018 for Pharma Retailer Changes
            _commonDs.Tables[0].TableName = "Lcode";
            _commonDs.Tables[1].TableName = "Main";
            _commonDs.Tables[2].TableName = "Item";
            _commonDs.Tables[3].TableName = "PSPayDetail";
            _commonDs.Tables[4].TableName = "DcMast";
            _commonDs.Tables[5].TableName = "DiscChrgsFldLst";   // DCMast Fields linked in Lother of Price List Master, Account Master and Item Master. // Added by Sachin N. S. on 22/06/2013 for Bug-14538
            _commonDs.Tables[6].TableName = "Stax_Mas";          // Sales Tax Master. // Added by Sachin N. S. on 22/06/2013 for Bug-14538
            _commonDs.Tables[7].TableName = "UdPOSSettings";          // POS Settings // Added by Sachin N. S. on 13/11/2018 for Pharma Retailer Chnages
            //_commonDs.Tables[3].TableName = "Acdet";
            //_commonDs.Tables[4].TableName = "Mall";
            //_commonDs.Tables[5].TableName = "Itref";

            //****** Added by Sachin N. S. on 22/06/2013 for Bug-14538 -- Start ******//
            if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["V_Extra"]) == true)
            {
                DataTable _dt = new DataTable();
                //_dt = _oDataAccess.GetDataTable("Select * from Lother where E_Code='PS' Order by Att_file, Serial, SubSerial", null, 25);
                _dt = _oDataAccess.GetDataTable("Select * from Lother where E_Code='" + CommonInfo.EntryTy + "' Order by Att_file, Serial, SubSerial", null, 25);    // Changed by Sachin N. S. on 27/09/2018 for Pharma Retailer Changes
                _dt.TableName = "Lother";
                _commonDs.Tables.Add(_dt);
            }

            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
            {
                DataRow[] _dr1 = _commonDs.Tables["DiscChrgsFldLst"].Select("Att_File=true");
                DataTable _dt;
                if (_dr1.Count() > 0)
                {
                    _dt = _dr1.CopyToDataTable();
                    string _hdrDiscFlds = string.Join(",", _dt.Rows.OfType<DataRow>().Select(row => row["LothrFldNm"].ToString()).ToArray());
                    CommonInfo.HdrDiscFlds = _hdrDiscFlds;
                }
                _dr1 = _commonDs.Tables["DiscChrgsFldLst"].Select("Att_File=false and E_code='PM'");
                if (_dr1.Count() > 0)
                {
                    _dt = _dr1.CopyToDataTable();
                    string _dtlDiscFlds = string.Join(",", _dt.Rows.OfType<DataRow>().Select(row => row["LothrFldNm"].ToString()).ToArray());
                    CommonInfo.DtlDiscFlds = _dtlDiscFlds;
                }
                _dr1 = _commonDs.Tables["DiscChrgsFldLst"].Select("Att_File=false and E_code='IM'");
                if (_dr1.Count() > 0)
                {
                    _dt = _dr1.CopyToDataTable();
                    string _dtlDiscFlds = string.Join(",", _dt.Rows.OfType<DataRow>().Select(row => row["LothrFldNm"].ToString()).ToArray());
                    CommonInfo.DtlDiscFldsIM = _dtlDiscFlds;
                }
                if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                {
                    DataRow _dr;
                    _dr = _commonDs.Tables["Stax_Mas"].NewRow();
                    _dr["Tax_Name"] = "NO-TAX";
                    _dr["Level1"] = 0;
                    _commonDs.Tables["Stax_Mas"].Rows.Add(_dr);
                }
            }
            //****** Added by Sachin N. S. on 22/06/2013 for Bug-14538 -- End ******//
        }

        private void RefreshControls()
        {
            txtTotalQty.Text = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null).Sum(row => Convert.ToDecimal(row["Qty"])), 2).ToString();
            txtNoOfItems.Text = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count().ToString();
            //txtGrossAmt.Text = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["U_asseamt"] != null).Sum(row => Convert.ToDecimal(row["U_asseamt"])), 2).ToString();
            txtGrossAmt.Text = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Gro_amt"] != null).Sum(row => Convert.ToDecimal(row["Gro_amt"])), 2).ToString();    // Changed by Sachin N. S. on 25/05/2013 for Bug-14538
            //txtTaxAmt.Text = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null).Sum(row => Convert.ToDecimal(row["TaxAmt"])), 2).ToString();
            //txtTaxAmt.Text = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null).Sum(row => Math.Round(Convert.ToDecimal(row["TaxAmt"]),2)).ToString();
            int _roundoff = Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_Op"]) ? 0 : 2;
            //txtNetAmt.Text = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Gro_amt"] != null).Sum(row => Convert.ToDecimal(row["Gro_amt"])), _roundoff).ToString();

            //**** Added by Sachin N. S. on 29/11/2018 for Pharma Retialer Changes -- Start
            decimal gstamt = 0M;
            gstamt = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["SGST_AMT"] != null).Sum(row => Convert.ToDecimal(row["SGST_AMT"])), 2);
            _commonDs.Tables["Main"].Rows[0]["SGST_AMT"] = Convert.ToDecimal(gstamt);
            _commonDs.Tables["Main"].Rows[0]["TOT_EXAMT"] = Convert.ToDecimal(gstamt);

            gstamt = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["CGST_AMT"] != null).Sum(row => Convert.ToDecimal(row["CGST_AMT"])), 2);
            _commonDs.Tables["Main"].Rows[0]["CGST_AMT"] = Convert.ToDecimal(gstamt);
            _commonDs.Tables["Main"].Rows[0]["TOT_EXAMT"] = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["TOT_EXAMT"]) + Convert.ToDecimal(gstamt);

            //**** Added by Sachin N. S. on 29/11/2018 for Pharma Retialer Changes -- End

            //***** Added by Sachin N. S. on 22/05/2013 for Bug-14538 -- Start *****//
            _commonDs.Tables["Main"].Rows[0]["Gro_Amt"] = Convert.ToDecimal(txtGrossAmt.Text);
            this.CalculateHeaderTax("");
            this.RefreshSummaryGrid();
            //***** Added by Sachin N. S. on 22/05/2013 for Bug-14538 -- End *****//
            txtBillAmt.Text = _commonDs.Tables["Main"].Rows[0]["Net_Amt"].ToString();       // Added by Sachin N. S. on 03/12/2018 for Pharma Retailer Changes
            this.Refresh();
        }

        // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- Start ****** //
        private void RefreshSummaryGrid()
        {

            //**** Added by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- Start
            Decimal _Amount = 0;
            var _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                               where currentRw.Field<string>("HeadingNm") == "Line Gross Amount" && currentRw.Field<string>("HdrDet") == "I"
                               select currentRw).FirstOrDefault();
            _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["U_ASSEAMT"] != null).Sum(row => Convert.ToDecimal(row["U_ASSEAMT"]));

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Less : Dedn. Before Tax" && currentRw.Field<string>("HdrDet") == "I"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["TOT_DEDUC"] != null).Sum(row => Convert.ToDecimal(row["TOT_DEDUC"]));
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Taxable Charges" && currentRw.Field<string>("HdrDet") == "I"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["TOT_TAX"] != null).Sum(row => Convert.ToDecimal(row["TOT_TAX"]));
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "GST Charges" && currentRw.Field<string>("HdrDet") == "I"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["TOT_EXAMT"] != null).Sum(row => Convert.ToDecimal(row["TOT_EXAMT"]));
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Add/Less Charges" && currentRw.Field<string>("HdrDet") == "I"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["TOT_ADD"] != null).Sum(row => Convert.ToDecimal(row["TOT_ADD"]));
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Non Taxable Charges" && currentRw.Field<string>("HdrDet") == "I"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["TOT_NONTAX"] != null).Sum(row => Convert.ToDecimal(row["TOT_NONTAX"]));
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Less : Final Discount" && currentRw.Field<string>("HdrDet") == "I"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["TOT_FDISC"] != null).Sum(row => Convert.ToDecimal(row["TOT_FDISC"]));
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Line Total" && currentRw.Field<string>("HdrDet") == "I"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["GRO_AMT"] != null).Sum(row => Convert.ToDecimal(row["GRO_AMT"]));
            }

            //**** Header wise Charges -- Start
            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Less : Dedn. Before Tax" && currentRw.Field<string>("HdrDet") == "H"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_deduc"];
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Taxable Charges" && currentRw.Field<string>("HdrDet") == "H"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_tax"];
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Add/Less Charges" && currentRw.Field<string>("HdrDet") == "H"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_add"];
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Non Taxable Charges" && currentRw.Field<string>("HdrDet") == "H"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_nontax"];
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Less : Final Discount" && currentRw.Field<string>("HdrDet") == "H"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_fdisc"];
            }
            //**** Header wise Charges -- End

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Net Amount"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                _Amount = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_examt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_add"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["taxamt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_nontax"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_fdisc"]);
                int _roundoff = Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_Op"]) ? 0 : 2;
                _currentRow["Amount"] = Math.Round(_Amount, _roundoff).ToString();
                _commonDs.Tables["Main"].Rows[0]["Net_Amt"] = _currentRow["Amount"];
            }

            _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                           where currentRw.Field<string>("HeadingNm") == "Round Off"
                           select currentRw).FirstOrDefault();
            if (_currentRow != null)
            {
                int _roundoff = Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_Op"]) ? 0 : 2;
                _Amount = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Net_Amt"]) - _Amount;
                _commonDs.Tables["Main"].Rows[0]["roundoff"] = _roundoff == 0 ? _Amount : Math.Round(_Amount, 2);
                _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["roundoff"];
            }
            //**** Added by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- End

            //////**** Commented by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- Start
            ////Decimal _Amount = 0;
            ////var _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////                   where currentRw.Field<string>("HeadingNm") == "Gross Amount"
            ////                   select currentRw).FirstOrDefault();
            ////_currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Gro_amt"];

            ////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////               where currentRw.Field<string>("HeadingNm") == "Less : Dedn. Before Tax"
            ////               select currentRw).FirstOrDefault();
            ////if (_currentRow != null)
            ////{
            ////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_deduc"];
            ////}

            ////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////               where currentRw.Field<string>("HeadingNm") == "Taxable Charges"
            ////               select currentRw).FirstOrDefault();
            ////if (_currentRow != null)
            ////{
            ////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_tax"];
            ////}

            //////where currentRw.Field<string>("HeadingNm") == "Excise Duty"       // 
            ////// 
            //////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            //////               where currentRw.Field<string>("HeadingNm") == "GST Charges"      // Changed by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes
            //////               select currentRw).FirstOrDefault();
            //////if (_currentRow != null)
            //////{
            //////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_examt"];
            //////}

            ////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////               where currentRw.Field<string>("HeadingNm") == "Add/Less Charges"
            ////               select currentRw).FirstOrDefault();
            ////if (_currentRow != null)
            ////{
            ////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_add"];
            ////}

            ////// Commneted by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes -- Start
            //////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            //////               where currentRw.Field<string>("HeadingNm") == "GST Tax"
            //////               select currentRw).FirstOrDefault();
            //////if (_currentRow != null)
            //////{
            //////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Taxamt"];
            //////}
            ////// Commneted by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes -- End

            ////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////               where currentRw.Field<string>("HeadingNm") == "Non Taxable Charges"
            ////               select currentRw).FirstOrDefault();
            ////if (_currentRow != null)
            ////{
            ////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_nontax"];
            ////}

            ////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////               where currentRw.Field<string>("HeadingNm") == "Less : Final Discount"
            ////               select currentRw).FirstOrDefault();
            ////if (_currentRow != null)
            ////{
            ////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["Tot_fdisc"];
            ////}

            ////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////               where currentRw.Field<string>("HeadingNm") == "Net Amount"
            ////               select currentRw).FirstOrDefault();
            ////if (_currentRow != null)
            ////{
            ////    _Amount = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_examt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_add"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["taxamt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_nontax"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_fdisc"]);
            ////    int _roundoff = Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_Op"]) ? 0 : 2;
            ////    _currentRow["Amount"] = Math.Round(_Amount, _roundoff).ToString();
            ////    _commonDs.Tables["Main"].Rows[0]["Net_Amt"] = _currentRow["Amount"];
            ////}

            ////_currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
            ////               where currentRw.Field<string>("HeadingNm") == "Round Off"
            ////               select currentRw).FirstOrDefault();
            ////if (_currentRow != null)
            ////{
            ////    int _roundoff = Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_Op"]) ? 0 : 2;
            ////    _Amount = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Net_Amt"]) - _Amount;
            ////    _commonDs.Tables["Main"].Rows[0]["roundoff"] = _roundoff == 0 ? _Amount : Math.Round(_Amount, 2);
            ////    _currentRow["Amount"] = _commonDs.Tables["Main"].Rows[0]["roundoff"];
            ////}
            //////**** Commented by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes -- End

        }

        private void itemwiseTotal(int _grdRowPos, int _grdColPos)
        {
            _grdColPos = dgvItemDetails.Columns[_grdColPos].DisplayIndex;
            string _fldNm, _a_s;
            DataRow _dr, _dr1, _dr2;
            //_dr1 = _commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentRow.Index];
            _commonDs.Tables["Item"].Columns["U_asseamt"].ReadOnly = false; // Added by Sachin N. S. on 27/08/2018 for Bug-`756
            _commonDs.Tables["Item"].Columns["Gro_amt"].ReadOnly = false; // Added by Sachin N. S. on 27/08/2018 for Bug-`756
            _dr1 = _commonDs.Tables["Item"].Rows[_grdRowPos];       // Changed by Scahin N. S. on 15/01/2016 for Bug-27503
            _dr1["Tot_deduc"] = 0;
            _dr1["Tot_tax"] = 0;
            _dr1["Tot_examt"] = 0;
            _dr1["Tot_add"] = 0;
            _dr1["Tot_nontax"] = 0;
            _dr1["Tot_fdisc"] = 0;

            decimal _maValue = 0M;
            _maValue = (Convert.ToDecimal(_dr1["Qty"]) + Convert.ToDecimal(_dr1["PhmLoseQty"]) - Convert.ToDecimal(_dr1["PhmFreeQty"])) * Convert.ToDecimal(_dr1["Rate"]);   // Changed by Sachin N. S. on 24/10/2018 for Pharma Retailer Changes
            string _cpertNm = "";
            decimal _npertnm = 0M, _mAmt = 0M;
            decimal _mCalAmt = 0M;      // Added by Sachin N. S. on 21/08/2018 for GST 
            decimal _maValue2 = 0M;     // Added by Sachin N. S. on 27/08/2018 for Bug-31756
            _maValue2 = Convert.ToDecimal(_maValue);    // Added by Sachin N. S. on 27/08/2018 for Bug-31756
            if (Convert.ToBoolean(_dr1["GSTIncl"]) == true)
            {
                _maValue2 = (Convert.ToDecimal(_maValue) / (1 + (Convert.ToDecimal(_dr1["GstRate"]) / 100)));
            }

            _dr1["u_asseamt"] = _maValue2;      // Added by Sachin N. S. on 27/08/2018 for Bug-31756

            #region Evaluating Column values before the column edited
            DataGridViewColumn _dgvCol, _dgvCol1;
            for (int i = 0; i < _grdColPos; i++)
            {
                _dgvCol = (from _grdVwCol in dgvItemDetails.Columns.Cast<DataGridViewColumn>()
                           where _grdVwCol.DisplayIndex == i
                           select _grdVwCol).SingleOrDefault();
                if (Convert.ToString(_dgvCol.Tag) == "V#")
                {
                    if (_dgvCol.DisplayIndex < _grdColPos)
                    {
                        _fldNm = _dgvCol.DataPropertyName;
                        _dr = _commonDs.Tables["DtDiscChrgs"].Select("Fld_Nm='" + _fldNm.ToString().Trim() + "'").FirstOrDefault();
                        switch (_dr["A_S"].ToString())
                        {
                            case "+":
                                if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                                { _a_s = "-"; }
                                else
                                { _a_s = _dr["a_s"].ToString(); }
                                break;

                            case "-":
                                if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                                { _a_s = "-"; }
                                else
                                { _a_s = _dr["a_s"].ToString(); }
                                break;

                            default:
                                if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                                { _a_s = "-"; }
                                else
                                { _a_s = "+"; }
                                break;
                        }
                        //_maValue = _maValue + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                        _maValue2 = _maValue2 + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));  // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes

                        //Commented by Shrikant S. on 27/12/2018 for Installer 2.1.0        //Start
                        //if (Convert.ToBoolean(_dr["Bef_Aft"]) == true)
                        //{
                        //    //_dr1["u_asseamt"] = _maValue;
                        //    _dr1["u_asseamt"] = _maValue2;      // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
                        //}
                        //Commented by Shrikant S. on 27/12/2018 for Installer 2.1.0        //End

                        //Added by Shrikant S. on 27/12/2018 for Installer 2.1.0        //Start
                        if (Convert.ToInt32(_dr["Bef_Aft"]) == 1)
                        {
                            _dr1["u_asseamt"] = _maValue2;      
                        }
                        //Added by Shrikant S. on 27/12/2018 for Installer 2.1.0        //End
                    }
                    else
                    {
                        break;
                    }
                }
            }
            #endregion Evaluating Column values before the column edited

            #region Evaluating Column values after the column edited
            for (int i = _grdColPos; i < dgvItemDetails.Columns.Count; i++)
            {
                _dgvCol = (from _grdVwCol in dgvItemDetails.Columns.Cast<DataGridViewColumn>()
                           where _grdVwCol.DisplayIndex == i
                           select _grdVwCol).FirstOrDefault();

                if (Convert.ToString(_dgvCol.Tag) == "V#" || Convert.ToString(_dgvCol.Tag) == "S#" || Convert.ToString(_dgvCol.Tag) == "%#")
                {
                    if (Convert.ToString(_dgvCol.Tag) == "V#")
                    {
                        _fldNm = _dgvCol.DataPropertyName;
                    }
                    else
                    {
                        _dgvCol1 = (from _grdVwCol in dgvItemDetails.Columns.Cast<DataGridViewColumn>()
                                    where _grdVwCol.DisplayIndex == i + 1
                                    select _grdVwCol).FirstOrDefault();
                        _fldNm = _dgvCol1.DataPropertyName;
                    }
                    _dr = (_commonDs.Tables["DtDiscChrgs"].Select("Fld_Nm='" + _fldNm + "'").FirstOrDefault());
                    switch (_dr["A_S"].ToString())
                    {
                        case "+":
                            if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                            { _a_s = "-"; }
                            else
                            { _a_s = _dr["a_s"].ToString(); }
                            break;

                        case "-":
                            if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                            { _a_s = "-"; }
                            else
                            { _a_s = _dr["a_s"].ToString(); }
                            break;

                        default:
                            if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                            { _a_s = "-"; }
                            else
                            { _a_s = "+"; }
                            break;
                    }

                    _dgvCol1 = (from _grdVwCol in dgvItemDetails.Columns.Cast<DataGridViewColumn>()
                                where _grdVwCol.DisplayIndex == i - 1
                                select _grdVwCol).FirstOrDefault();
                    if (Convert.ToString(_dgvCol.Tag) == "%#" || Convert.ToString(_dgvCol1.Tag) == "%#")
                    {
                        _mAmt = 0M;
                        _cpertNm = _dr["Pert_Name"].ToString().Trim();
                        //_npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentRow.Index][_cpertNm]);
                        _npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_grdRowPos][_cpertNm]);         // Changed by Sachin N. S. on 15/01/2016 for Bug-27503
                        if (_npertnm > 0)
                        {
                            //***** Added by Sachin N. S. on 08/05/2017 for GST -- Start
                            if (_dr["amtexpr"].ToString().Trim() != "")
                            {
                                string[] _strExpr = _dr["amtexpr"].ToString().Trim().Split(new[] { '.' });
                                string _cTblName = _strExpr[0].ToString().ToUpper() == "ITEM_VW" ? "Item" : "Main";
                                string _cFldName = _strExpr[1].ToString();
                                _mCalAmt = Convert.ToDecimal(_commonDs.Tables[_cTblName].Rows[_grdRowPos][_cFldName]);
                            }
                            //***** Added by Sachin N. S. on 08/05/2017 for GST -- End

                            if (_dr["Disp_Sign"].ToString().Trim() == "%")
                            {
                                //_mAmt = _maValue * (_npertnm / 100);
                                //_mAmt = (_dr["amtexpr"].ToString().Trim() != "" ? _mCalAmt : _maValue) * (_npertnm / 100);        // Changed by Sachin N. S. on 08/05/2017 for GST
                                _mAmt = (_dr["amtexpr"].ToString().Trim() != "" ? _mCalAmt : _maValue2) * (_npertnm / 100);        // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
                            }
                            if (Convert.ToString(_dgvCol1.Tag) == "%#")
                            {
                                if (Convert.ToDecimal(_dr1[_fldNm]) != _mAmt)
                                {
                                    _dr1[_cpertNm] = 0;
                                }
                            }
                        }
                        else
                        {
                            _dr1[_dr["Pert_Name"].ToString().Trim()] = 0;
                            if (_grdColPos != i)
                            {
                                //_mAmt = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentRow.Index][_fldNm]);
                                _mAmt = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_grdRowPos][_fldNm]);      // Changed by Sachin N. S. on 15/01/2016 for Bug-27503
                            }
                        }

                        if (Convert.ToString(_dgvCol.Tag) == "%#")
                        {
                            _dr1[_fldNm] = _mAmt;
                        }
                    }

                    if (Convert.ToString(_dgvCol.Tag) == "S#" || Convert.ToString(_dgvCol1.Tag) == "S#")
                    {
                        _mAmt = 0.00M;
                        _cpertNm = _dr1["Tax_Name"].ToString().Trim();
                        if (_cpertNm != string.Empty)
                        {
                            _dr2 = _commonDs.Tables["Stax_Mas"].Select("Tax_Name='" + _cpertNm + "'").FirstOrDefault();
                            _npertnm = Convert.ToDecimal(_dr2["Level1"]);
                            if (_npertnm > 0)
                            {
                                //_mAmt = _maValue * (_npertnm / 100);
                                _mAmt = _maValue2 * (_npertnm / 100);    // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
                            }

                            if (Convert.ToString(_dgvCol.Tag) == "S#")
                            {
                                _dr1[_fldNm] = _mAmt;
                            }
                        }
                    }

                    if (Convert.ToString(_dgvCol.Tag) != "S#" && Convert.ToString(_dgvCol.Tag) != "%#")
                    {
                        //Commented by Shrikant S. on 27/12/2018 for installer 2.1.0    //Start
                        //if (Convert.ToBoolean(_dr["Bef_Aft"]) == true)
                        //{
                        //    _maValue2 = Convert.ToDecimal(_maValue) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                        //    _dr1["u_asseamt"] = _maValue2;
                        //}
                        //Commented by Shrikant S. on 27/12/2018 for installer 2.1.0    //End

                        //Added by Shrikant S. on 27/12/2018 for installer 2.1.0    //Start
                        if (Convert.ToInt32(_dr["Bef_Aft"]) == 1)
                        {
                            _maValue2 = Convert.ToDecimal(_maValue) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                            _dr1["u_asseamt"] = _maValue2;
                        }
                        //Added by Shrikant S. on 27/12/2018 for installer 2.1.0    //End

                        if (_dr["Excl_Gross"].ToString() != "C" && _dr["Excl_Gross"].ToString() != "A" && _dr["Excl_Gross"].ToString() != "E" && _dr["Excl_Gross"].ToString() != "P")
                        {
                            //_maValue += _a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]);
                            _maValue2 += _a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]);      // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes

                            switch (_dr["Code"].ToString())
                            {
                                case "D":
                                    _dr1["Tot_deduc"] = Convert.ToDecimal(_dr1["Tot_deduc"]) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                                    break;
                                case "T":
                                    _dr1["Tot_tax"] = Convert.ToDecimal(_dr1["Tot_tax"]) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                                    break;
                                case "E":
                                    _dr1["Tot_examt"] = Convert.ToDecimal(_dr1["Tot_examt"]) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                                    break;
                                case "A":
                                    _dr1["Tot_add"] = Convert.ToDecimal(_dr1["Tot_add"]) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                                    break;
                                case "N":
                                    _dr1["Tot_nontax"] = Convert.ToDecimal(_dr1["Tot_nontax"]) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                                    break;
                                case "F":
                                    _dr1["Tot_fdisc"] = Convert.ToDecimal(_dr1["Tot_fdisc"]) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                                    break;
                            }
                        }
                    }
                }
            }
            _dr1["gro_amt"] = Convert.ToDecimal(_maValue2);    // Added by Sachin N. S. on Pharma Retailer Changes
            #endregion Evaluating Column values after the column edited
            this.RefreshControls();     // Added by Sachin N. S. on 28/08/2018 for Bug-31756
        }
        // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- End ****** //
        #endregion Build dataset and Refresh controls

        #region Clear and reset Datatables
        private void clearTables()
        {
            foreach (DataTable _dt in _commonDs.Tables)
            {
                //if (_dt.TableName != "Lcode" && _dt.TableName != "DCMast" && _dt.TableName != "DiscChrgsFldLst" && _dt.TableName != "HdDiscChrgs" && _dt.TableName != "DtDiscChrgs" && _dt.TableName != "Stax_Mas")   // ****** Changed by Sachin N. S. on 28/05/2013 for Bug-14538
                if (_dt.TableName != "Lcode" && _dt.TableName != "DCMast" && _dt.TableName != "DiscChrgsFldLst" && _dt.TableName != "HdDiscChrgs" && _dt.TableName != "DtDiscChrgs" && _dt.TableName != "Stax_Mas" && _dt.TableName != "UdPOSSettings")   // ****** Changed by Sachin N. S. on 20/11/2018 for Pharma Retailer Changes
                {
                    _dt.Clear();
                }
                if (_dt.TableName == "HdDiscChrgs")
                {
                    _dt.Select().ToList<DataRow>().ForEach(r => { r["Def_Pert"] = 0; r["Def_Amt"] = 0; });
                }
            }
        }
        #endregion Clear and reset Datatables

        #region Binding Controls
        private void BindControls()
        {
            txtInvoiceNo.DataBindings.Add("text", _commonDs, "Main.Inv_no");
            txtInvoiceDt.DataBindings.Add("text", _commonDs, "Main.Date");
            // Commented by Sachin N. S. on 06/08/2013 for Bug-14538 -- Start
            //txtCategory.DataBindings.Add("text", _commonDs, "Main.Cate");
            //txtDepartment.DataBindings.Add("text", _commonDs, "Main.dept");
            //txtInvoiceSr.DataBindings.Add("text", _commonDs, "Main.Inv_sr");
            //txtUserName.DataBindings.Add("text", _commonDs, "Main.user_Name");
            // Commented by Sachin N. S. on 06/08/2013 for Bug-14538 -- End
            txtGrossAmt.DataBindings.Add("text", _commonDs, "Main.Gro_amt");
            txtNetAmt.DataBindings.Add("text", _commonDs, "Main.Net_amt");
            txtTaxAmt.DataBindings.Add("text", _commonDs, "Main.TaxAmt");
            txtParty.DataBindings.Add("text", _commonDs, "Main.Party_nm");
            this.lblWarehouse.Text = (CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse");   // Added by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes

            gridBinding();
        }

        private void gridBinding()
        {
            // ***** Binding Item Grid -- Start ***** \\
            dgvItemDetails.AutoGenerateColumns = false;
            dgvItemDetails.ClearSelection();
            dgvItemDetails.DataSource = _commonDs.Tables["Item"];

            //**** Added by Sachin N. S. on 28/09/2018 for Pharma Retailer Changes -- Start
            if (_commonDs.Tables["Lcode"].Columns.Contains("gpuomapp") == true)
            {
                if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["gpuomapp"]) == true)
                {
                    this.llgpuom = true;
                }
            }
            //**** Added by Sachin N. S. on 28/09/2018 for Pharma Retailer Changes -- End

            dgvItemDetails.Columns[0].DataPropertyName = "Item_no";
            dgvItemDetails.Columns[1].DataPropertyName = "Item";
            dgvItemDetails.Columns[2].DataPropertyName = "Qty";
            dgvItemDetails.Columns[3].DataPropertyName = "Rate";

            //***** Added by Sachin N. S. for Pharma Retailer Changes -- Start
            DataGridViewCellStyle _dgcStyl = new DataGridViewCellStyle();
            _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            _dgcStyl.Format = "F3";
            _dgcStyl.BackColor = Color.LightGray;
            dgvItemDetails.Columns[0].DefaultCellStyle = _dgcStyl;

            int _colId = 2;
            if (this.llgpuom == true)
            {
                this.CreateGrpUOMColumns(ref _colId);
            }
            else
            {
                dgvItemDetails.Columns[2].DisplayIndex = _colId;
                _colId += 1;
                dgvItemDetails.Columns[3].DisplayIndex = _colId;
                _colId += 1;
            }
            //***** Added by Sachin N. S. for Pharma Retailer Changes -- End


            //****** Added by Sachin N. S. 25/06/2013 for Bug-14538 -- Start ******//
            //dgvItemDetails.Columns[4].DataPropertyName = "TaxAmt";    // Commented by Sachin N. S. on 25/06/2013 for Bug-14538
            //int _colId = 4;       Commented by Sachin N. S. on 28/09/2018 for Pharma Retailer Changes
            DataRow _dr;
            DataRow[] _drdcmast;
            DataTable _dtdcmast;
            //string _expStr = "U_asseamt";
            string _expStr = "(Qty+PhmLoseQty-phmfreeQty)*Rate";        // Added by Sachin N. S. on 27/08/2018 for Bug-31756   // Changed by Sachin N. S. on 24/10/2018 for Pharma Retailer Changes
            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
            {
                if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
                {
                    _drdcmast = _commonDs.Tables["DCMast"].Select("Att_file=false");
                    if (_drdcmast.Count() > 0)
                    {
                        _dtdcmast = _drdcmast.CopyToDataTable();
                    }
                    else
                    {
                        _dtdcmast = _commonDs.Tables["DCMast"].Clone();
                    }
                }
                else
                {
                    _dtdcmast = _commonDs.Tables["DCMast"].Clone();
                }
                _dtdcmast.TableName = "DtDiscChrgs";
                if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                {
                    _dr = _dtdcmast.NewRow();
                    _dr["Entry_ty"] = _commonDs.Tables["Lcode"].Rows[0]["Entry_ty"];
                    _dr["Code"] = "S";
                    _dr["fld_nm"] = "TAXAMT";
                    _dr["Round_off"] = false;
                    _dr["Att_file"] = false;
                    _dr["Disp_sign"] = "%";
                    _dr["Head_Nm"] = "NO-TAX";
                    _dr["Def_Pert"] = 0;
                    _dr["SortOrder"] = "EA1";
                    _dtdcmast.Rows.Add(_dr);
                }
                _dtdcmast.DefaultView.Sort = "SortOrder ASC";
                _dtdcmast = _dtdcmast.DefaultView.ToTable();
                _commonDs.Tables.Add(_dtdcmast);

                this.CreateItChrgsColDynamically(ref _expStr, ref _colId);
            }
            dgvItemDetails.Columns[4].DataPropertyName = "Gro_amt";
            dgvItemDetails.Columns[4].DisplayIndex = _colId;
            _colId += 1;
            dgvItemDetails.Columns[5].DataPropertyName = "";
            dgvItemDetails.Columns[5].DisplayIndex = _colId;

            //****** Added by Sachin N. S. 25/06/2013 for Bug-14538 -- End ******//

            //dgvItemDetails.Columns[5].DataPropertyName = "Gro_amt";   // Commented by Sachin N. S. 25/06/2013 for Bug-14538
            //dgvItemDetails.Columns[6].DataPropertyName = "";          // Commented by Sachin N. S. 25/06/2013 for Bug-14538

            //_commonDs.Tables["Item"].Columns["U_asseamt"].Expression = "Qty*Rate";        && Commented by Sachin N. S. on 27/08/2018 for Bug-31756
            //_commonDs.Tables["Item"].Columns["Gro_amt"].Expression = _expStr;       // Added by Sachin N. S. 25/06/2013 for Bug-14538     // Commented by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
            //_commonDs.Tables["Item"].Columns["TaxAmt"].Expression = "(U_asseamt*TaxPercent)/100";     // Commented by Sachin N. S. 25/06/2013 for Bug-14538
            //_commonDs.Tables["Item"].Columns["Gro_amt"].Expression = "(Qty*Rate)+TaxAmt";             // Commented by Sachin N. S. 25/06/2013 for Bug-14538

            //Added by Shrikant S. on 01/10/2018        //Start
            if (_commonDs.Tables["Lother"].Rows.Count > 0)
            {
                //int left = this.lblWarehouse.Left;
                //int top = this.lblWarehouse.Top + this.lblWarehouse.Height + 2;
                int left = this.lblParty.Left;
                int top = this.lblParty.Top + this.lblParty.Height + 2;

                int linecount = 0;
                int tabIndex = this.btnAddInfo.TabIndex + 1;        // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                for (int i = 0; i < _commonDs.Tables["Lother"].Rows.Count; i++)
                {
                    if (Convert.ToBoolean(_commonDs.Tables["Lother"].Rows[i]["ingrid"]) == true && Convert.ToBoolean(_commonDs.Tables["Lother"].Rows[i]["att_file"]) == true && _commonDs.Tables["Lother"].Rows[i]["Inter_Use"].ToString().ToUpper() == ".F.")
                    {
                        linecount++;
                        string fldnm = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["fld_nm"]).Trim();
                        string headnm = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["head_nm"]).Trim();
                        Label vlbl = null;
                        switch (Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["data_ty"]).Trim().ToLower())
                        {
                            case "varchar":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.Left = left;
                                vlbl.Top = top;
                                vlbl.AutoSize = false;
                                vlbl.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                this.pnlCenterBox.Controls.Add(vlbl);
                                TextBox txt = new TextBox();
                                txt.Name = "txt" + fldnm;
                                txt.Left = vlbl.Left + 97;
                                txt.Top = top;
                                txt.Width = 150;
                                txt.DataBindings.Add("text", _commonDs, "Main." + fldnm);
                                txt.Enabled = true;
                                txt.ReadOnly = false;
                                txt.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                this.pnlCenterBox.Controls.Add(txt);
                                this.pnlCenterBox.Controls[txt.Name].BringToFront();
                                this.pnlHoriz1.Top = txt.Top + txt.Height - 2;
                                this.dgvItemDetails.Top = this.pnlHoriz1.Top + 4;
                                this.dgvItemDetails.Height = this.dgvItemDetails.Height - txt.Height - 4;
                                string filtcond = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["filtcond"]).Trim();
                                if (filtcond.Length > 0)
                                {
                                    Button btn = new Button();
                                    btn.Name = "btn" + fldnm;
                                    btn.Text = "...";
                                    btn.Left = txt.Left + txt.Width + 2;
                                    btn.Top = top;
                                    //btn.Tag = filtcond;
                                    btn.Tag = "<<" + filtcond + ">><<" + headnm + ">>";     // Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                    btn.Width = 30;
                                    btn.UseVisualStyleBackColor = true;
                                    btn.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                    tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                    this.pnlCenterBox.Controls.Add(btn);
                                    btn.Click += new EventHandler(popupbtn_clicked);
                                }
                                break;
                            case "numeric":
                            case "decimal":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.Left = left;
                                vlbl.Top = top;
                                vlbl.AutoSize = false;
                                vlbl.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                this.pnlCenterBox.Controls.Add(vlbl);
                                uNumericTextBox.cNumericTextBox ntxt = new uNumericTextBox.cNumericTextBox();
                                ntxt.Name = "txt" + fldnm;
                                ntxt.Left = vlbl.Left + 97;
                                ntxt.Top = top;
                                ntxt.Width = 150;
                                ntxt.DataBindings.Add("text", _commonDs, "Main." + fldnm);
                                ntxt.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                this.pnlCenterBox.Controls.Add(ntxt);
                                break;
                            case "bit":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.Left = left;
                                vlbl.Top = top;
                                vlbl.AutoSize = false;
                                vlbl.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                this.pnlCenterBox.Controls.Add(vlbl);
                                CheckBox chk = new CheckBox();
                                chk.Name = "chk" + fldnm;
                                chk.Left = vlbl.Left + 100;
                                chk.Top = top;
                                chk.Width = 120;
                                chk.DataBindings.Add("Checked", _commonDs, "Main." + fldnm);
                                chk.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                this.pnlCenterBox.Controls.Add(chk);
                                break;
                            case "datetime":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.Left = left;
                                vlbl.Top = top;
                                vlbl.AutoSize = false;
                                vlbl.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                this.pnlCenterBox.Controls.Add(vlbl);
                                DateTimePicker dpk = new DateTimePicker();
                                dpk.Name = "dpk" + fldnm;
                                dpk.Left = vlbl.Left + 97;
                                dpk.Top = top;
                                dpk.Width = 100;
                                dpk.CustomFormat = " ";
                                dpk.Format = DateTimePickerFormat.Custom;
                                dpk.ValueChanged += new System.EventHandler(this.dpk_ValueChanged);
                                dpk.TabIndex = tabIndex;       // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                                tabIndex += 1;                  // Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes

                                Binding b = new Binding("Value", _commonDs, "Main." + fldnm, true);
                                b.Format += new ConvertEventHandler(dpk_Format);
                                b.Parse += new ConvertEventHandler(dpk_Parse);
                                dpk.DataBindings.Add(b);

                                this.pnlCenterBox.Controls.Add(dpk);
                                break;
                            case "text":
                                break;
                            default:
                                break;
                        }
                        if (linecount % 2 == 0)
                        {
                            top = vlbl.Top + vlbl.Height;
                            left = left - 325;
                        }
                        else
                        {
                            int prevTop = this.pnlHoriz1.Top;
                            this.pnlHoriz1.Top = vlbl.Top + vlbl.Height - 2;
                            this.dgvItemDetails.Top = this.pnlHoriz1.Top + 4;

                            this.dgvItemDetails.Height = this.pnlHoriz2.Top - this.pnlHoriz1.Top - 4;
                            left = left + 325;
                        }
                    }

                }

                //***** Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- Start

                this.dgvItemDetails.TabIndex = tabIndex;
                tabIndex += 1;
                this.dgvDiscChrgs.TabIndex = tabIndex;
                tabIndex += 1;
                this.dgvSummary.TabIndex = tabIndex;
                tabIndex += 1;
                this.btnPrintInv.TabIndex = tabIndex;
                tabIndex += 1;
                this.btnPayment.TabIndex = tabIndex;
                tabIndex += 1;
                this.btnExit.TabIndex = tabIndex;
                tabIndex += 1;
                //***** Added by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- End
            }

            this.AddLinewiseAdditionalFields();             //Added by Shrikant S. on 01/10/2018 
            //Added by Shrikant S. on 01/10/2018        //End

            dgvItemDetails.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            // ***** Binding Item Grid -- End ***** \\

            // ***** Binding Header-wise Discount and Charges Grid for Bug-14538 -- Start ***** \\
            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true)
            {
                dgvDiscChrgs.AutoGenerateColumns = false;
                dgvDiscChrgs.ClearSelection();
                if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
                {
                    _drdcmast = _commonDs.Tables["DCMast"].Select("Att_file=true");
                    if (_drdcmast.Count() > 0)
                    {
                        _dtdcmast = _drdcmast.CopyToDataTable();
                    }
                    else
                    {
                        _dtdcmast = _commonDs.Tables["DCMast"].Clone();
                    }
                }
                else
                {
                    _dtdcmast = _commonDs.Tables["DCMast"].Clone();
                }
                _dtdcmast.TableName = "HdDiscChrgs";
                if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true)
                {
                    _dr = _dtdcmast.NewRow();
                    _dr["Entry_ty"] = _commonDs.Tables["Lcode"].Rows[0]["Entry_ty"];
                    _dr["Code"] = "S";
                    _dr["fld_nm"] = "TAXAMT";
                    _dr["Round_off"] = false;
                    _dr["Att_file"] = true;
                    _dr["Disp_sign"] = "%";
                    _dr["Head_Nm"] = "NO-TAX";
                    _dr["Def_Pert"] = 0;
                    _dr["SortOrder"] = "EA1";
                    _dtdcmast.Rows.Add(_dr);
                }
                _dtdcmast.DefaultView.Sort = "SortOrder ASC";
                _dtdcmast = _dtdcmast.DefaultView.ToTable();

                _commonDs.Tables.Add(_dtdcmast);
                dgvDiscChrgs.DataSource = _commonDs.Tables["HdDiscChrgs"];

                dgvDiscChrgs.Columns[0].DataPropertyName = "Head_Nm";
                dgvDiscChrgs.Columns[1].DataPropertyName = "Def_Pert";
                dgvDiscChrgs.Columns[2].DataPropertyName = "Def_Amt";
            }
            // ***** Binding Header-wise Discount and Charges Grid for Bug-14538 -- End ***** \\

            // ***** Added by Sachin N. S. on 27/05/2013 for Bug-14538 -- Start ***** //
            // ***** Binding the Summary of the Gross Amount, Tax structure and Net Amount -- Start ***** \\
            dgvSummary.AutoGenerateColumns = false;
            dgvSummary.ClearSelection();

            _dtSumDiscChrgs = new DataTable();
            _dtSumDiscChrgs.TableName = "SumTaxChrgs";
            _dtSumDiscChrgs.Columns.Add(new DataColumn("HeadingNm", typeof(string)));
            _dtSumDiscChrgs.Columns.Add(new DataColumn("Amount", typeof(decimal)));
            _dtSumDiscChrgs.Columns.Add(new DataColumn("HdrDet", typeof(string)));     // Added by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes

            //**** Added by Sachin N. S. on 29/01/2018 for Pharma Retailer Changes -- Start
            _dr = _dtSumDiscChrgs.NewRow();
            _dr["HeadingNm"] = "Line Gross Amount";
            _dr["Amount"] = 0.00;
            _dr["HdrDet"] = "I";
            _dtSumDiscChrgs.Rows.Add(_dr);

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "D").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Less : Dedn. Before Tax";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "T").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["dcmast"].AsEnumerable().Where(row => Convert.ToString(row["code"]) == "E").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["udpossettings"].Rows[0]["I_disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["headingnm"] = "GST Charges";
                _dr["amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "A").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Add/Less Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "N").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Non Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "F").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Less : Final Discount";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            _dr = _dtSumDiscChrgs.NewRow();
            _dr["HeadingNm"] = "Line Total";
            _dr["Amount"] = 0.00;
            _dr["HdrDet"] = "I";
            _dtSumDiscChrgs.Rows.Add(_dr);

            //****** Header wise Charges -- Start
            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "D").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Less : Dedn. Before Tax";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "T").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "A").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Add/Less Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "N").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Non Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "F").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Less : Final Discount";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            //****** Header wise Charges -- End

            if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_op"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Round Off";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            _dr = _dtSumDiscChrgs.NewRow();
            _dr["HeadingNm"] = "Net Amount";
            _dr["Amount"] = 0.00;
            _dr["HdrDet"] = "H";
            _dtSumDiscChrgs.Rows.Add(_dr);

            dgvSummary.DataSource = _dtSumDiscChrgs;

            dgvSummary.Columns[0].DataPropertyName = "HeadingNm";
            dgvSummary.Columns[1].DataPropertyName = "Amount";

            foreach (DataGridViewRow _dgr in dgvSummary.Rows)
            {
                if (_dgr.Cells[0].Value.ToString() == "Line Total" || _dgr.Cells[0].Value.ToString() == "Line Gross Amount")
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();

                    style.Font = new Font(dgvSummary.Font, FontStyle.Bold);
                    style.BackColor = Color.LightGray;
                    style.SelectionBackColor = Color.LightGray;

                    _dgr.DefaultCellStyle = style;

                }
                if (_dgr.Cells[0].Value.ToString() == "Net Amount")
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();

                    style.Font = new Font(dgvSummary.Font, FontStyle.Bold);
                    style.BackColor = Color.Gray;
                    style.SelectionBackColor = Color.Gray;
                    style.ForeColor = Color.White;

                    _dgr.DefaultCellStyle = style;
                }
            }

            // ***** Binding the Summary of the Gross Amount, Tax structure and Net Amount -- End ***** \\
            //**** Added by Sachin N. S. on 29/01/2018 for Pharma Retailer Changes -- End

            ////**** Commented by Sachin N. S. on 29/01/2018 for Pharma Retailer Changes -- Start

            ////DataRow _dr;
            //_dr = _dtSumDiscChrgs.NewRow();
            //_dr["HeadingNm"] = "Gross Amount";
            //_dr["Amount"] = 0.00;
            //_dtSumDiscChrgs.Rows.Add(_dr);

            //if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "D").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            //{
            //    _dr = _dtSumDiscChrgs.NewRow();
            //    _dr["HeadingNm"] = "Less : Dedn. Before Tax";
            //    _dr["Amount"] = 0.00;
            //    _dtSumDiscChrgs.Rows.Add(_dr);
            //}

            //if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "T").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            //{
            //    _dr = _dtSumDiscChrgs.NewRow();
            //    _dr["HeadingNm"] = "Taxable Charges";
            //    _dr["Amount"] = 0.00;
            //    _dtSumDiscChrgs.Rows.Add(_dr);
            //}

            ////**** Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- Start
            ////if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "E").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            ////{
            ////    _dr = _dtSumDiscChrgs.NewRow();
            ////    //_dr["HeadingNm"] = "Excise Duty";
            ////    _dr["HeadingNm"] = "GST Charges";       // Changes done by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes
            ////    _dr["Amount"] = 0.00;
            ////    _dtSumDiscChrgs.Rows.Add(_dr);
            ////}
            ////**** Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- End

            //if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "A").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            //{
            //    _dr = _dtSumDiscChrgs.NewRow();
            //    _dr["HeadingNm"] = "Add/Less Charges";
            //    _dr["Amount"] = 0.00;
            //    _dtSumDiscChrgs.Rows.Add(_dr);
            //}

            //// Commented by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes -- Start
            ////if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["stax_tran"]) == true)
            ////{
            ////    _dr = _dtSumDiscChrgs.NewRow();
            ////    _dr["HeadingNm"] = "GST Tax";
            ////    _dr["Amount"] = 0.00;
            ////    _dtSumDiscChrgs.Rows.Add(_dr);
            ////}
            //// Commented by Sachin N. S. on 19/11/2018 for Pharma Retailer Changes -- End

            //if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "N").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            //{
            //    _dr = _dtSumDiscChrgs.NewRow();
            //    _dr["HeadingNm"] = "Non Taxable Charges";
            //    _dr["Amount"] = 0.00;
            //    _dtSumDiscChrgs.Rows.Add(_dr);
            //}

            //if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "F").Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            //{
            //    _dr = _dtSumDiscChrgs.NewRow();
            //    _dr["HeadingNm"] = "Less : Final Discount";
            //    _dr["Amount"] = 0.00;
            //    _dtSumDiscChrgs.Rows.Add(_dr);
            //}

            //if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_op"]) == true)
            //{
            //    _dr = _dtSumDiscChrgs.NewRow();
            //    _dr["HeadingNm"] = "Round Off";
            //    _dr["Amount"] = 0.00;
            //    _dtSumDiscChrgs.Rows.Add(_dr);
            //}

            //_dr = _dtSumDiscChrgs.NewRow();
            //_dr["HeadingNm"] = "Net Amount";
            //_dr["Amount"] = 0.00;
            //_dtSumDiscChrgs.Rows.Add(_dr);

            //dgvSummary.DataSource = _dtSumDiscChrgs;

            //dgvSummary.Columns[0].DataPropertyName = "HeadingNm";
            //dgvSummary.Columns[1].DataPropertyName = "Amount";

            //// ***** Binding the Summary of the Gross Amount, Tax structure and Net Amount -- End ***** \\
            //// ***** Added by Sachin N. S. on 27/05/2013 for Bug-14538 -- End ***** //

            ////// ***** Binding Payment mode Grid -- Start ***** \\
            ////dgvPayDetails.AutoGenerateColumns = false;
            ////dgvPayDetails.ClearSelection();
            ////dgvPayDetails.DataSource = _commonDs.Tables["PSPayDetail"];

            ////dgvPayDetails.Columns[0].DataPropertyName = "Paymode";
            ////dgvPayDetails.Columns[1].DataPropertyName = "TotalValue";
            ////// ***** Binding Item Grid -- End ***** \\

            ////**** Commented by Sachin N. S. on 29/01/2018 for Pharma Retailer Changes -- End

        }

        //Added by Shrikant S. on 01/10/2018    //Start
        public void dpk_ValueChanged(object sender, EventArgs e)
        {
            if (((DateTimePicker)sender).ShowCheckBox == true)
            {
                if (((DateTimePicker)sender).Checked == false)
                {
                    ((DateTimePicker)sender).CustomFormat = " ";
                    ((DateTimePicker)sender).Format = DateTimePickerFormat.Custom;
                    ((DateTimePicker)sender).Checked = false;
                    ((DateTimePicker)sender).Refresh();
                }
                else
                {
                    ((DateTimePicker)sender).CustomFormat = "dd/MM/yyyy";
                    ((DateTimePicker)sender).Format = DateTimePickerFormat.Custom;
                    ((DateTimePicker)sender).Checked = true;
                }
            }
            else
            {
                ((DateTimePicker)sender).CustomFormat = "dd/MM/yyyy";
                ((DateTimePicker)sender).Format = DateTimePickerFormat.Short;
                ((DateTimePicker)sender).Checked = true;
            }
        }
        void dpk_Format(object sender, ConvertEventArgs e)
        {
            // e.Value is the object value, we format it to be what we want to show up in the control 
            Binding b = sender as Binding;
            if (b != null)
            {
                DateTimePicker dtp = (b.Control as DateTimePicker);
                if (dtp != null)
                {
                    if (e.Value == null || e.Value == System.DBNull.Value || Convert.ToDateTime(e.Value).Year <= 1900)
                    {
                        dtp.ShowCheckBox = true;
                        dtp.Checked = false;
                        e.Value = dtp.Value;
                    }
                    else
                    {
                        dtp.ShowCheckBox = true;
                        if (Convert.ToDateTime(e.Value).Year <= 1900)
                        {
                            dtp.Checked = false;
                            dtp.Format = DateTimePickerFormat.Custom;
                        }
                        else
                            dtp.Checked = true;
                    }
                }
            }
        }

        void dpk_Parse(object sender, ConvertEventArgs e)
        {
            // e.value is the formatted value coming from the control.   
            // we change it to be the value we want to stuff in the object. 
            Binding b = sender as Binding;
            if (b != null)
            {
                DateTimePicker dtp = (b.Control as DateTimePicker);
                if (dtp != null)
                {
                    if (dtp.Checked == false)
                    {
                        dtp.ShowCheckBox = true;
                        dtp.Checked = false;
                        e.Value = System.DBNull.Value;
                    }
                    else
                    {
                        DateTime val = Convert.ToDateTime(e.Value);
                        e.Value = val;
                    }
                }
            }
        }

        private void popupbtn_clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            //**** Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- Start
            //string filtcond = btn.Tag.ToString().Trim();
            string[] btnTag = btn.Tag.ToString().Split(new[] { ">><<" }, StringSplitOptions.None);
            string filtcond = btnTag[0].ToString().Trim().Replace("<<", "");
            string cfrmCapo = btnTag[1].ToString().Trim().Replace(">>", "");
            //**** Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- End
            if (filtcond.ToString().Length > 0)
            {
                DataTable searchTbl = new DataTable();

                udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
                string cFrmCap = "Select ", cSrchCol = "", cDispCol = "", cRetCol = "";
                if (filtcond.ToLower().Contains("execute ") || filtcond.ToLower().Contains("select ") || filtcond.ToLower().Contains("exec "))
                {
                    //cFrmCap = cFrmCap + dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].HeaderText.Trim();
                    cFrmCap = cFrmCap + cfrmCapo;       // // Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                    string csql = (filtcond.Trim().IndexOf("{") > 0 ? filtcond.Trim().Substring(0, filtcond.Trim().IndexOf("{") - 1) : filtcond.Trim());
                    searchTbl = _oDataAccess.GetDataTable(csql, null, 25);

                    cSrchCol = searchTbl.Columns[0].ColumnName;
                    cRetCol = searchTbl.Columns[0].ColumnName;
                    cDispCol = searchTbl.Columns[0].ColumnName;

                    if (filtcond.Trim().IndexOf("{") > 0)
                    {
                        filtcond = filtcond.Trim().Substring(filtcond.Trim().IndexOf("{") + 1, (filtcond.Trim().Trim().Length - filtcond.Trim().IndexOf("{") - 2));     //Added by Shrikant S. on 05/11/2018 

                        string[] vals = filtcond.Trim().Split('#');
                        switch (vals.Length)
                        {
                            case 1:
                                cSrchCol = vals[0];
                                break;
                            case 2:
                                cSrchCol = vals[0];
                                cRetCol = vals[1];
                                break;
                            case 4:
                                cSrchCol = vals[0];
                                cRetCol = vals[1];
                                cDispCol = vals[3];
                                break;
                            default:
                                break;
                        }
                    }
                    _oSelPop.pformtext = cFrmCap;
                    _oSelPop.psearchcol = cSrchCol;
                    _oSelPop.pDisplayColumnList = cDispCol;
                    _oSelPop.pRetcolList = cRetCol;
                }
                else
                {
                    string[] valList = filtcond.Trim().Split(',');
                    foreach (var item in valList)
                    {
                        DataRow row = searchTbl.NewRow();
                        row[0] = item;
                        searchTbl.Rows.Add(row);
                    }
                }
                DataView _dvw = searchTbl.DefaultView;
                _oSelPop.pdataview = _dvw;
                _oSelPop.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
                _oSelPop.ShowDialog();

                if (_oSelPop.pReturnArray != null)
                {
                    string fldnm = btn.Name.Substring(3);
                    _commonDs.Tables["Main"].Rows[this.BindingContext[_commonDs, "Main"].Position][fldnm] = _oSelPop.pReturnArray[0].ToString();
                    string ctlName = "txt" + fldnm;
                    Control[] ctl = this.pnlCenterBox.Controls.Find(ctlName, false);
                    TextBox txt = (TextBox)ctl[0];
                    txt.Text = _oSelPop.pReturnArray[0].ToString();
                }
            }
        }
        private void AddLinewiseAdditionalFields()
        {
            var rowId = new DataGridViewTextBoxColumn();
            rowId.HeaderText = "Rowid";
            rowId.DataPropertyName = "ItemRowId";
            rowId.Name = "ItemRowId";
            rowId.Visible = false;
            this.dgvItemDetails.Columns.Add(rowId);

            if (_commonDs.Tables.Contains("Lother"))
            {
                if (_commonDs.Tables["Lother"].Rows.Count > 0)
                {
                    bool _addInfobutton = (Convert.ToInt32(_commonDs.Tables["Lother"].Compute("Count(Fld_nm)", "Att_file=False And Ingrid=False")) > 0 ? true : false);
                    //,"Ingrid==False and Att_file==False and Inter_use==False")) > 0 ? true : false);
                    if (_addInfobutton)
                    {
                        var addiInfobtn = new DataGridViewButtonColumn();
                        addiInfobtn.HeaderText = "Addi. Info.";
                        addiInfobtn.Text = "Addi. Info.";
                        addiInfobtn.Name = "colItemAddInfo";
                        addiInfobtn.UseColumnTextForButtonValue = true;
                        this.dgvItemDetails.Columns.Add(addiInfobtn);
                    }
                    for (int i = 0; i < _commonDs.Tables["Lother"].Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(_commonDs.Tables["Lother"].Rows[i]["Att_file"]) == false && Convert.ToBoolean(_commonDs.Tables["Lother"].Rows[i]["Ingrid"]) == true)
                        {
                            if (_commonDs.Tables["Lother"].Rows[i]["Fld_nm"].ToString() == "BATCHNO")
                                continue;
                            switch (Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["data_ty"]).ToLower().Trim())
                            {
                                case "varchar":
                                    var txtColV = new PointofSale.udDGVStringColumn();
                                    txtColV.HeaderText = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Head_nm"]).Trim();
                                    txtColV.Name = "col" + Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    txtColV.DataPropertyName = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    txtColV.Width = 150;
                                    txtColV.Fldnm = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    txtColV.Whn_con = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Whn_con"]).Trim();
                                    txtColV.Val_con = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Val_con"]).Trim();
                                    txtColV.Val_err = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Val_err"]).Trim();
                                    txtColV.Defa_val = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["defa_val"]).Trim();
                                    txtColV.Mandatory = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["mandatory"]).Trim();
                                    txtColV.Inter_use = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["inter_use"]).Trim();
                                    txtColV.Filtcond = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Filtcond"]).Trim();
                                    txtColV.Inptmask = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Inptmask"]).Trim();
                                    txtColV.ReadOnly = false;
                                    txtColV.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                                    this.dgvItemDetails.Columns.Add(txtColV);
                                    break;
                                case "numeric":
                                case "decimal":
                                    var txtColN = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                                    txtColN.HeaderText = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Head_nm"]).Trim();
                                    txtColN.Name = "col" + Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    txtColN.DataPropertyName = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    txtColN.Width = 150;
                                    txtColN.DecimalLength = Convert.ToInt32(_commonDs.Tables["Lother"].Rows[i]["Fld_dec"]);
                                    txtColN.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                                    this.dgvItemDetails.Columns.Add(txtColN);
                                    break;
                                case "bit":
                                    var chkCol = new DataGridViewCheckBoxColumn();
                                    chkCol.HeaderText = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Head_nm"]).Trim();
                                    chkCol.Name = "chk" + Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    chkCol.DataPropertyName = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    chkCol.Width = 150;
                                    chkCol.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                                    this.dgvItemDetails.Columns.Add(chkCol);
                                    break;
                                case "datetime":
                                    var udpkCol = new udclsDGVDateTimePicker.MicrosoftDateTimePicker();
                                    udpkCol.HeaderText = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Head_nm"]).Trim();
                                    udpkCol.Name = "dpk" + Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    udpkCol.DataPropertyName = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    udpkCol.Width = 100;
                                    udpkCol.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                                    this.dgvItemDetails.Columns.Add(udpkCol);
                                    break;
                                case "text":
                                    break;
                                default:
                                    break;
                            }
                        }
                        this.dgvItemDetails.Refresh();
                    }
                }
            }
        }
        //Added by Shrikant S. on 01/10/2018    //End

        //***** Added by Sachin N. S. on 25/06/2013 for Bug-14538 -- Start *****//
        private void CreateItChrgsColDynamically(ref string _expStr, ref int _colId)
        {
            udclsDGVNumericColumn.CNumEditDataGridViewColumn _dc;
            //_expStr = (_expStr == "" ? "U_asseamt" : _expStr);
            _expStr = (_expStr == "" ? "(Qty+PhmLoseQty-PhmFreeQty)*Rate" : _expStr);   // Added by Sachin N. S. on 27/08/2018 for Bug-31756
            string _a_s = "";
            foreach (DataRow _dr in _commonDs.Tables["DtDiscChrgs"].Rows)
            {
                if (_dr["a_s"].ToString().Trim() != string.Empty)
                {
                    _a_s = _dr["a_s"].ToString().Trim();
                }
                else if (_dr["Code"].ToString().Trim() == "D" || _dr["Code"].ToString().Trim() == "F")
                {
                    _a_s = "-";
                }
                else
                {
                    _a_s = "+";
                }

                if (_dr["Code"].ToString().Trim() != "S")
                {
                    if (_dr["Pert_Name"].ToString().Trim() != string.Empty)
                    {
                        _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                        _dc.Name = "col" + _dr["Pert_Name"].ToString().Trim();
                        _dc.DataPropertyName = _dr["Pert_Name"].ToString().Trim();
                        _dc.HeaderText = _dr["Head_Nm"].ToString().Trim() + " %";
                        //_dc.HeaderText = " % ";
                        _dc.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        _dc.DecimalLength = 2;
                        _dc.DisplayIndex = _colId;
                        _dc.Tag = "%#";
                        _dc.Visible = ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["LSHWITMDISC"] ? ((bool)_dr["LSHWDISCCG"]) : ((bool)_dr["LSHWDISCCG"]));        // Added by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes
                        _dc.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                        dgvItemDetails.Columns.Add(_dc);
                        _colId += 1;
                    }

                    _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                    _dc.Name = "col" + _dr["Fld_Nm"].ToString().Trim();
                    _dc.DataPropertyName = _dr["Fld_Nm"].ToString().Trim();
                    _dc.HeaderText = _dr["Head_Nm"].ToString().Trim();
                    _dc.DecimalLength = 2;
                    _dc.DisplayIndex = _colId;
                    //_dc.ReadOnly = true;
                    _dc.Tag = "V#";
                    _dc.Visible = ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["LSHWITMDISC"] ? ((bool)_dr["LSHWDISCCG"]) : ((bool)_dr["LSHWDISCCG"]));        // Added by Sachin N. S. on 29/11/2018 for Pharma Retailer Changes
                    _dc.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                    dgvItemDetails.Columns.Add(_dc);

                    //                    _commonDs.Tables["Item"].Columns[_dr["Fld_Nm"].ToString().Trim()].Expression = "((" + _expStr + ")*" + _dr["Pert_Name"].ToString().Trim() + ")/100";
                    _colId += 1;
                    _expStr += _a_s + _dr["Fld_Nm"].ToString().Trim();
                }
                else
                {
                    //DataGridViewTextBoxColumn _tb;
                    DataGridViewComboBoxColumn _tb;
                    _tb = new DataGridViewComboBoxColumn();
                    _tb.Name = "colTax_Name";
                    _tb.DataPropertyName = "Tax_Name";
                    _tb.HeaderText = "Tax Name";
                    _tb.Items.AddRange(_commonDs.Tables["Stax_Mas"].AsEnumerable().Select(row => row["Tax_Name"].ToString()).ToArray());
                    _tb.Tag = "S#";
                    _tb.DisplayIndex = _colId;
                    _tb.Width = 150;
                    _tb.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                    dgvItemDetails.Columns.Add(_tb);
                    _colId += 1;

                    _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                    _dc.Name = "col" + _dr["Fld_Nm"].ToString().Trim();
                    _dc.DataPropertyName = _dr["Fld_Nm"].ToString().Trim();
                    _dc.HeaderText = "Tax Amount";
                    _dc.DecimalLength = 2;
                    _dc.DisplayIndex = _colId;
                    _dc.ReadOnly = true;
                    _dc.Tag = "V#";
                    _dc.SortMode = DataGridViewColumnSortMode.NotSortable;      // Added by Sachin N. S. on 30/11/2018 for Pharma Retailer Changes
                    dgvItemDetails.Columns.Add(_dc);

                    //_commonDs.Tables["Item"].Columns[_dr["Fld_Nm"].ToString().Trim()].Expression = _commonDs.Tables["Item"].Columns[_dr["Fld_Nm"].ToString().Trim()].Expression = "((" + _expStr + ")*TaxPer)/100";
                    _taxExpr = _expStr;
                    _colId += 1;
                    _expStr += _a_s + _dr["Fld_Nm"].ToString().Trim();
                }
            }

            //_commonDs.Tables["Item"].Columns["Gro_amt"].Expression = _expStr;     // Commented by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
        }
        //***** Added by Sachin N. S. on 25/06/2013 for Bug-14538 -- End *****//

        //***** Added by Sachin N. S. on 28/09/2018 for Pharma Retailer Changes -- Start *****//
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_colId"></param>
        private void CreateGrpUOMColumns(ref int _colId)
        {
            udclsDGVNumericColumn.CNumEditDataGridViewColumn _dc;
            DataGridViewTextBoxColumn _dgtxt;
            DataGridViewCellStyle _dgcStyl;

            bool lDispB4 = Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]);
            if (lDispB4 == false)
            {
                if (_commonDs.Tables["Lother"].Select("E_code='" + CommonInfo.EntryTy + "' and att_file=false and fld_nm='BATCHNO'").Count() > 0)
                {
                    DataRow _dr = _commonDs.Tables["Lother"].Select("E_code='" + CommonInfo.EntryTy + "' and att_file=false and fld_nm='batchno'").FirstOrDefault();
                    if (_dr != null)
                    {
                        this.addBatchNoFldInGrid(_dr, _colId);
                        _colId += 1;
                    }
                }
                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";

                _dgtxt = new DataGridViewTextBoxColumn();
                _dgtxt.Name = "colgpunit";
                _dgtxt.DataPropertyName = "gpunit";
                _dgtxt.HeaderText = "UOM";
                _dgtxt.DisplayIndex = _colId;
                _dgtxt.ReadOnly = false;
                _dgtxt.Tag = "";
                _dgtxt.DefaultCellStyle = _dgcStyl;
                _dgtxt.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dgtxt.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dgtxt);
                dgvItemDetails.Columns[_colId].Width = 25;
                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colgpqty";
                _dc.DataPropertyName = "gpqty";
                _dc.HeaderText = "Grp Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 100;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 100;

                _colId += 1;

                // Commented by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- as Discussed with Rupesh P. -- Start
                //_dgcStyl = new DataGridViewCellStyle();
                //_dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                //_dgcStyl.Format = "F3";

                //_dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                //_dc.Name = "colgprate";
                //_dc.DataPropertyName = "gprate";
                //_dc.HeaderText = "Grp Rate";
                //_dc.DecimalLength = 2;
                //_dc.DisplayIndex = _colId;
                //_dc.ReadOnly = false;
                //_dc.Tag = "";
                //_dc.DefaultCellStyle = _dgcStyl;
                //_dc.Width = 100;
                //_dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dgvItemDetails.Columns.Add(_dc);
                //dgvItemDetails.Columns[_colId].Width = 100;

                //_colId += 1;

                //_dgcStyl = new DataGridViewCellStyle();
                //_dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                //_dgcStyl.Format = "F3";
                //_dgcStyl.BackColor = Color.LightGray;

                //_dgtxt = new DataGridViewTextBoxColumn();
                //_dgtxt.Name = "colstkunit";
                //_dgtxt.DataPropertyName = "stkunit";
                //_dgtxt.HeaderText = "Stock UOM";
                //_dgtxt.DisplayIndex = _colId;
                //_dgtxt.ReadOnly = true;
                //_dgtxt.Tag = "";
                //_dgtxt.DefaultCellStyle = _dgcStyl;
                //_dgtxt.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dgvItemDetails.Columns.Add(_dgtxt);
                //dgvItemDetails.Columns[_colId].Width = 50;

                //_colId += 1;
                // Commented by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes -- as Discussed with Rupesh P. -- End

                dgvItemDetails.Columns[2].HeaderText = "Stk Qty";
                dgvItemDetails.Columns[2].DisplayIndex = _colId;
                dgvItemDetails.Columns[2].ReadOnly = true;
                dgvItemDetails.Columns[2].Width = 100;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.LightGray;
                dgvItemDetails.Columns[2].DefaultCellStyle = _dgcStyl;
                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colPhmFreeQty";
                _dc.DataPropertyName = "phmfreeqty";
                _dc.HeaderText = "Free Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 100;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 100;

                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colPhmLoseQty";
                _dc.DataPropertyName = "PhmLoseQty";
                _dc.HeaderText = "Loose Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 100;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 100;

                _colId += 1;

                dgvItemDetails.Columns[3].HeaderText = "Stock Rate";
                dgvItemDetails.Columns[3].DisplayIndex = _colId;
                dgvItemDetails.Columns[3].ReadOnly = true;
                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.LightGray;
                dgvItemDetails.Columns[3].DefaultCellStyle = _dgcStyl;
                dgvItemDetails.Columns[3].Width = 100;
                _colId += 1;
            }
            else
            {
                if (_commonDs.Tables["Lother"].Select("E_code='" + CommonInfo.EntryTy + "' and att_file=false and fld_nm='BATCHNO'").Count() > 0)
                {
                    DataRow _dr = _commonDs.Tables["Lother"].Select("E_code='" + CommonInfo.EntryTy + "' and att_file=false and fld_nm='batchno'").FirstOrDefault();
                    if (_dr != null)
                    {
                        this.addBatchNoFldInGrid(_dr, _colId);
                        _colId += 1;
                    }
                }

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.Aqua;

                _dgtxt = new DataGridViewTextBoxColumn();
                _dgtxt.Name = "colstkunit";
                _dgtxt.DataPropertyName = "stkunit";
                _dgtxt.HeaderText = "Stock UOM";
                _dgtxt.DisplayIndex = _colId;
                _dgtxt.ReadOnly = true;
                _dgtxt.Tag = "";
                _dgtxt.DefaultCellStyle = _dgcStyl;
                _dgtxt.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dgtxt.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dgtxt);
                dgvItemDetails.Columns[_colId].Width = 30;
                _colId += 1;

                dgvItemDetails.Columns[2].HeaderText = "Stock Qty";
                dgvItemDetails.Columns[2].DisplayIndex = _colId;
                dgvItemDetails.Columns[_colId].Width = 50;
                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colPhmFreeQty";
                _dc.DataPropertyName = "phmfreeqty";
                _dc.HeaderText = "Free Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 125;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 50;

                _colId += 1;

                dgvItemDetails.Columns[3].HeaderText = "Stock Rate";
                dgvItemDetails.Columns[3].DisplayIndex = _colId;
                dgvItemDetails.Columns[_colId].Width = 50;
                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.LightGray;

                _dgtxt = new DataGridViewTextBoxColumn();
                _dgtxt.Name = "colgpunit";
                _dgtxt.DataPropertyName = "gpunit";
                _dgtxt.HeaderText = "UOM";
                _dgtxt.DisplayIndex = _colId;
                _dgtxt.ReadOnly = false;
                _dgtxt.Tag = "";
                _dgtxt.DefaultCellStyle = _dgcStyl;
                _dgtxt.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dgtxt.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dgtxt);
                dgvItemDetails.Columns[_colId].Width = 25;
                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.LightGray;

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colgpqty";
                _dc.DataPropertyName = "gpqty";
                _dc.HeaderText = "Grp Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 50;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 50;

                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.LightGray;

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colPhmLoseQty";
                _dc.DataPropertyName = "PhmLoseQty";
                _dc.HeaderText = "Loose Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dgcStyl.BackColor = Color.LightGray;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 50;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 50;

                _colId += 1;

                //_dgcStyl = new DataGridViewCellStyle();
                //_dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                //_dgcStyl.Format = "F3";
                //_dgcStyl.BackColor = Color.Aqua;

                //_dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                //_dc.Name = "colgprate";
                //_dc.DataPropertyName = "gprate";
                //_dc.HeaderText = "Grp Unit Rate";
                //_dc.DecimalLength = 2;
                //_dc.DisplayIndex = _colId;
                //_dc.ReadOnly = false;
                //_dc.Tag = "";
                //_dc.DefaultCellStyle = _dgcStyl;
                //_dc.Width = 125;
                //_dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dgvItemDetails.Columns.Add(_dc);

                //_colId += 1;
            }

            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["GSTIncl"]) == true)
            {
                DataGridViewCheckBoxColumn _dchk;
                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.Aqua;

                _dchk = new DataGridViewCheckBoxColumn();
                _dchk.Name = "colGSTIncl";
                _dchk.DataPropertyName = "GSTIncl";
                _dchk.HeaderText = "Incl. Gst";
                _dchk.DisplayIndex = _colId;
                _dchk.ReadOnly = false;
                _dchk.Tag = "";
                _dchk.DefaultCellStyle = _dgcStyl;
                _dchk.Width = 50;
                _dchk.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dchk.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dchk);

                _colId += 1;
            }
        }
        //***** Added by Sachin N. S. on 28/09/2018 for Pharma Retailer Changes -- End *****//
        #endregion

        #region Calling Popup Selection Screen
        private void popUpFromTable(string _popupType)
        {
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            string cFrmCap = "", cSrchCol = "", cDispCol = "", cRetCol = "";
            string cTbl = "", cCond = "", cFldList = "", xcFldList = "", xcSrchCol = "", xcDispCol = "", xcRetCol = "";
            string[][] cFldLst;     //***** Added by Sachin N. S. on 23/05/2013 for Bug-14538
            int _lRateLevel = -1;    //***** Added by Sachin N. S. on 23/05/2013 for Bug-14538
            DataTable _dt = new DataTable();
            udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
            switch (_popupType)
            {
                //***** Added by Sachin N. S. on 23/05/2013 for Bug-14538 -- Start *****//
                case "PARTY":
                    xcFldList = "";
                    xcDispCol = "";
                    xcRetCol = "";
                    cTbl = " Ac_Mast ";
                    cCond = " Ac_Id!=0 ";

                    cFldList = _commonDs.Tables["Lcode"].Rows[0]["Ac_fields"].ToString();
                    cFldLst = cFldList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(':')).ToArray();
                    var cAcId =
                        from _cFldLst in cFldLst
                        where _cFldLst[0].ToUpper().Contains("AC_ID")
                        select _cFldLst[0];
                    if (cAcId.Count<string>() == 0)
                    {
                        xcFldList += ",Ac_mast.Ac_Id ";
                    }
                    //xcFldList += ",Ac_mast.DISCPERBFT,Ac_mast.DISCPERAFT ";   // Commented by Sachin N. S. on 18/07/2013 for Bug-14538
                    //xcRetCol += ",DISCPERBFT,DISCPERAFT";                     // Commented by Sachin N. S. on 18/07/2013 for Bug-14538

                    cCond += " And (Ac_mast.ldeactive = 0 Or (Ac_mast.ldeactive = 1 And Ac_mast.deactfrom > '" + txtInvoiceDt.Text.ToString() + "' )) ";
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

                    cCond += " And Validity like '%" + Convert.ToString(_commonDs.Tables["Lcode"].Rows[0]["Entry_Ty"]) + "%' ";
                    cSql = "set dateformat dmy select Ware_nm from Warehouse where " + cCond + " order by Ware_nm ";
                    cFrmCap = "Select " + (CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse");      // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
                    cSrchCol = "Ware_nm";
                    cDispCol = "Ware_nm:" + (CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse") + xcDispCol;        // Changed by Sachin N. S. on 15/11/2018 for Pharma Retailer Changes
                    cRetCol = "Ware_nm" + xcRetCol;
                    break;

                //***** Added by Sachin N. S. on 23/05/2013 for Bug-14538 -- End *****//

                case "ITEM":
                    //**** Changed by Sachin N. S. on 21/08/2018 for GST -- Start
                    //xcFldList = ",It_mast.Rate,It_mast.Tax_Name,It_Mast.InclSTTax,It_Mast.AskQty,It_Mast.AskRate ";                // Changed by Sachin N. S. on 18/01/2016 for Bug-27503
                    //xcDispCol = ",Rate:Goods Rate,Tax_Name:Tax Name,InclSTTax:Incl of GST Tax,AskQty: Ask Quantity,AskRate:Ask Rate";    // Changed by Sachin N. S. on 18/01/2016 for Bug-27503
                    //xcRetCol = ",Rate,Tax_Name,InclSTTax,AskQty,AskRate";                                          // Changed by Sachin N. S. on 18/01/2016 for Bug-27503
                    //xcFldList = ",It_mast.Rate,It_Mast.InclSTTax,It_Mast.AskQty,It_Mast.AskRate ";
                    //xcDispCol = ",Rate: Goods Rate, InclSTTax:Incl of GST,AskQty: Ask Quantity, AskRate:Ask Rate";
                    xcFldList = ",It_mast.RateUnit,It_mast.S_Unit,It_mast.Rate,It_Mast.InclSTTax,It_Mast.AskQty,It_Mast.AskRate ";        // Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                    xcDispCol = ",S_Unit:UOM";                                                              // Changed by Sachin N. S. on 28/11/2018 for Pharma Retailer Changes
                    xcRetCol = ",RateUnit,S_Unit,Rate,InclSTTax,AskQty,AskRate";
                    //**** Changed by Sachin N. S. on 21/08/2018 for GST -- End

                    cTbl = " It_Mast ";
                    cCond = " It_Mast.It_code!=0 and It_mast.isService=0 ";

                    //******* Checking for Rate in Item Rate master -- Start *******\\
                    if ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true)
                    {
                        string cSql1 = "Select top 1 Rate_Level From Ac_mast where Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                        _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                        _lRateLevel = Convert.ToInt16(_dt.Rows[0]["Rate_Level"]);           // Added by Sachin N. S. on 29/06/2013 for Bug-14538
                        if (Convert.ToInt16(_dt.Rows[0]["Rate_Level"]) != 0)
                        {
                            cCond += " And (It_mast.It_code = It_rate.It_code And (It_rate.Rlevel = " + _lRateLevel.ToString() + ")) ";
                        }
                        else
                        {
                            cCond += " And (It_mast.It_code = It_rate.It_code And It_rate.Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString() + ") ";
                        }
                        cTbl += ",It_Rate ";
                        xcFldList += ",It_Rate.It_Rate ";
                        xcDispCol += ",It_Rate:Party Rate";
                        xcRetCol += ",It_Rate";
                    }
                    //******* Checking for Rate in Item Rate master -- End *******\\

                    cFldList = _commonDs.Tables["Lcode"].Rows[0]["It_fields"].ToString();
                    cFldLst = cFldList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(':')).ToArray();
                    var cItCode =
                        from _cFldLst in cFldLst
                        where _cFldLst[0].ToUpper().Contains("IT_CODE")
                        select _cFldLst[0];
                    if (cItCode.Count<string>() == 0)
                    {
                        xcFldList += ",It_mast.It_Code ";
                    }

                    string cFldStr = splitText(cFldLst, ref xcSrchCol, ref xcDispCol);  //***** Added by Sachin N. S. on 13/08/2015 for Bug-26654                    

                    cCond += " And (It_mast.ldeactive = 0 Or (It_mast.ldeactive = 1 And It_mast.deactfrom > '" + txtInvoiceDt.Text.ToString() + "' )) ";
                    cSql = "set dateformat dmy select " + cFldStr + xcFldList + " from " + cTbl + " where " + cCond + " order by It_Name ";
                    cFrmCap = "Select Goods";
                    cSrchCol = xcSrchCol;
                    cDispCol = xcDispCol;
                    cRetCol = "It_Code,It_Name" + xcRetCol;
                    break;

                //********* Added by Sachin N. S. on 20/06/2013 for Bug-14538 -- Start ********//
                case "SALESTAX":
                    string _stType = "";
                    cSql = "Select top 1 St_Type from Ac_Mast where ac_id=" + _commonDs.Tables["Main"].Rows[0]["Ac_id"].ToString();
                    _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                    if (_dt.Rows.Count > 0)
                    {
                        _stType = _dt.Rows[0][0].ToString().Trim();
                    }

                    xcFldList = "";
                    xcDispCol = "";
                    xcRetCol = "";
                    cTbl = " STax_Mas ";
                    cCond = " Entry_ty = '" + _commonDs.Tables["Main"].Rows[0]["Entry_ty"].ToString() + "' And (Stax_Mas.ldeactive = 0 Or (Stax_Mas.ldeactive = 1 And Stax_Mas.deactfrom > '" + _commonDs.Tables["Main"].Rows[0]["Date"].ToString() + "'))";
                    cCond += _stType == string.Empty ? "" : " and sTax_Mas.St_Type='" + _stType.ToString() + "'";

                    cSql = "Set dateformat dmy Select Stax_Mas.Tax_Name,Stax_Mas.Level1 From " + cTbl + " where " + cCond + " order by Stax_Mas.Tax_Name ";
                    cFrmCap = "Select GST Charges";
                    cSrchCol = "Tax_Name";
                    cDispCol = "Tax_Name:Tax_Name";
                    cRetCol = "Tax_Name,Level1";
                    break;
                //********* Added by Sachin N. S. on 20/06/2013 for Bug-14538 -- End ********//

                case "PRINTBILL":
                    xcFldList = ",It_mast.Rate,It_mast.Tax_Name ";
                    xcDispCol = ",Rate:Goods Rate,Tax_Name:Tax Name";
                    xcRetCol = ",Rate,Tax_Name";
                    cTbl = " DCMAIN ";
                    cCond = " Entry_ty='" + CommonInfo.EntryTy + "' and DATE='" + DateTime.Now.Date + "' ";     // Changed by Sachin N. S. on 27/09/2018 for Pharma Retailer Changes
                    //cCond = " Entry_ty='PS' and DATE='" + DateTime.Now.Date + "' ";

                    cSql = "set dateformat dmy select Inv_No,Tran_cd from " + cTbl + " where " + cCond + " order by Inv_No ";
                    cFrmCap = "Select Bill No.";
                    cSrchCol = "Inv_No";
                    cDispCol = "Inv_No:Bill No.";
                    cRetCol = "Tran_cd,Inv_No";
                    break;

                // ******* Added by Sachin N. S. on 21/01/2016 for Bug-27503 -- Start
                case "CANCELBILL":
                    xcFldList = "";
                    xcDispCol = "";
                    xcRetCol = "";
                    cTbl = " DCMAIN ";
                    cCond = " Entry_Ty='" + CommonInfo.EntryTy + "' and Party_nm!='CANCELLED.' ";           // Changed by Sachin N. S. on 27/09/2018 for Pharma Retailer Changes
                    //cCond = " Entry_Ty='PS' and Party_nm!='CANCELLED.' ";

                    cFldList = "Inv_No,Date";

                    cCond += " ";
                    cSql = "set dateformat dmy select Tran_cd,Inv_No,Date from DCMAIN where " + cCond + " order by Date ";
                    cFrmCap = "Select Bill to Cancel";
                    cSrchCol = "Inv_No";
                    cDispCol = "Inv_No:Bill No.,Date:Bill Date" + xcDispCol;
                    cRetCol = "Tran_cd" + xcRetCol;
                    break;
                // ******* Added by Sachin N. S. on 21/01/2016 for Bug-27503 -- End

                //********* Added by Sachin N. S. on 20/06/2013 for Bug-14538 -- Start ********//
                case "GROUPUNIT":
                    string _munit = string.Empty, _mgroupuom = string.Empty;
                    cSql = "Select Top 1 RateUnit,s_unit,p_unit From It_mast Where It_code =" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString();
                    _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                    if (_dt.Rows.Count > 0)
                    {
                        _munit = _dt.Rows[0]["RateUnit"].ToString();
                        _mgroupuom = Convert.ToBoolean(_commonDs.Tables["lcode"].Rows[0]["iotrans"]) == true ? _dt.Rows[0]["p_unit"].ToString() : _dt.Rows[0]["s_unit"].ToString();
                    }

                    if (_mgroupuom != string.Empty)
                    {
                        cSql = "if Exists( Select top 1 subuom from GroupUOM Where baseuom='" + _munit.Trim() + "' and groupuom='" + _mgroupuom.Trim() + "') ";
                        cSql += " Select subuom  From GroupUOM Where baseuom='" + _munit.Trim() + "' and groupuom='" + _mgroupuom.Trim() + "' Group by subuom Order by subuom ";
                        cSql += " Else";
                        cSql += " Select u_uom as subuom From uom Group by u_uom Order by u_uom ";

                        xcFldList = "";
                        xcDispCol = "";
                        xcRetCol = "";
                        cFrmCap = "Select Group UOM";
                        cSrchCol = "subuom";
                        cDispCol = "subuom:Group UOM";
                        cRetCol = "subuom";
                    }
                    break;
                //********* Added by Sachin N. S. on 20/06/2013 for Bug-14538 -- End ********//
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
                    // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- Start ****** //
                    case "PARTY":
                        _commonDs.Tables["Main"].Rows[this.BindingContext[_commonDs, "Main"].Position]["Ac_Id"] = _oSelPop.pReturnArray[0].ToString();
                        _commonDs.Tables["Main"].Rows[this.BindingContext[_commonDs, "Main"].Position]["Party_Nm"] = _oSelPop.pReturnArray[1].ToString();

                        //***** Added by Sachin N. S. on 22/06/2013 for Bug-14538 -- Start *****//
                        clsInsertDefaultValue.replNewvalueinChldTable("Item", "Ac_Id", _oSelPop.pReturnArray[0].ToString());
                        clsInsertDefaultValue.replNewvalueinChldTable("Item", "Party_Nm", _oSelPop.pReturnArray[1].ToString());

                        if (Convert.ToInt64(CommonInfo.PartyId) != Convert.ToInt64(_oSelPop.pReturnArray[0]) && (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true || CommonInfo.HdrDiscFlds != ""))
                        {
                            clsInsertDefaultValue.GetPartyDefaDiscChrgsValue(Convert.ToInt64(_oSelPop.pReturnArray[0]));
                            replDefaDiscChrgsValues("PARTY", 0);
                        }
                        //***** Added by Sachin N. S. on 22/06/2013 for Bug-14538 -- End *****//
                        break;

                    case "WAREHOUSE":
                        this.txtWareHouse.Text = _oSelPop.pReturnArray[0].ToString();
                        clsInsertDefaultValue.replNewvalueinChldTable("Item", "Ware_Nm", _oSelPop.pReturnArray[0].ToString());
                        CommonInfo.ChngdWareHouse = _oSelPop.pReturnArray[0].ToString();
                        break;
                    // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- End ****** //

                    case "ITEM":
                        // ****** Added by Sachin N. S. on 20/08/2015 for Bug-26654 -- Start 
                        int _nRowIndex = this.BindingContext[_commonDs, "Item"].Position;       // Added by Sachin N. S. on 15/01/2016 for Bug-27503
                        _curRowNo = _nRowIndex;     // Added by Sachin N. S. on 19/01/2016 for Bug-27503
                        if (Stock_Checking(Convert.ToInt64(_oSelPop.pReturnArray[0].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, this.dgvItemDetails.CurrentRow.Index) < 0)
                        {
                            MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // ****** Added by Sachin N. S. on 20/08/2015 for Bug-26654 -- End

                            string clAdd = "Add";
                            if (_commonDs.Tables["Item"].Rows.Count > 0)
                            {
                                var _currentRow = (from currentRw in _commonDs.Tables["Item"].AsEnumerable()
                                                   where currentRw.Field<decimal>("It_code") == Convert.ToDecimal(_oSelPop.pReturnArray[0].ToString())
                                                   select currentRw).FirstOrDefault();
                                if (_currentRow != null && _currentRow.Table.Rows.Count > 0)
                                {
                                    _currentRow["Qty"] = Convert.ToDecimal(_currentRow["Qty"]) + 1;
                                    clAdd = "EXIST";
                                    _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_oSelPop.pReturnArray[0].ToString()) + "'")[0]);     // Added by Sachin N. S. on 15/01/2016 for Bug-27503
                                    _curRowNo = _nRowIndex;     // Added by Sachin N. S. on 19/01/2016 for Bug-27503
                                    //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- Start
                                    if (this.llgpuom == true)
                                    {
                                        _currentRow["gpQty"] = Convert.ToDecimal(_currentRow["gpQty"]) + 1;
                                        this.Update_Conv_Qty("QTY", _curRowNo);
                                    }
                                    //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- End
                                }
                            }

                            if (clAdd == "Add")
                            {
                                this.BindingContext[_commonDs, "Item"].Position = dgvItemDetails.CurrentCell.RowIndex;      // Added by Sachin N. S. on 19/01/2016 for Bug-27503
                                _curRowNo = this.BindingContext[_commonDs, "Item"].Position;                                // Added by Sachin N. S. on 19/01/2016 for Bug-27503
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _oSelPop.pReturnArray[0].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _oSelPop.pReturnArray[1].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 1;      // Added by Sachin N. S. on 15/01/2016 for Bug-27503
                                //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- Start
                                if (this.llgpuom == true)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpQty"] = 1;
                                    //this.Update_Conv_Qty("QTY", _curRowNo);
                                }
                                //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- End
                                string[] _arr = cRetCol.Split(',');
                                int _iRate = Array.IndexOf(_arr, "Rate");
                                int _iItRate = Array.IndexOf(_arr, "It_Rate");
                                int _iInclTx = Array.IndexOf(_arr, "InclSTTax");        // Added by Sachin N. S. on 18/01/2016 for Bug-27503

                                //***** Commented by Sachin N. S. on 29/06/2013 for Bug-14538 -- Start *****//
                                //int _TaxName = Array.IndexOf(_arr, "Tax_Name");
                                //if (_oSelPop.pReturnArray[_TaxName].ToString() != "")
                                //{
                                //    cSql = "SET DATEFORMAT DMY; SELECT LEVEL1 FROM STAX_MAS WHERE ENTRY_TY='PS' AND TAX_NAME = '" + _oSelPop.pReturnArray[_TaxName].ToString() + "' and '" + this.txtInvoiceDt.Value.ToString() + "' between Wefstkfrom and Wefstkto ";
                                //    _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                                //    if (_dt.Rows.Count > 0)
                                //    {
                                //        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["TAXPERCENT"] = _dt.Rows[0]["Level1"];
                                //    }
                                //}
                                //***** Commented by Sachin N. S. on 29/06/2013 for Bug-14538 -- End *****//

                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_iItRate > 0 ? (Convert.ToDecimal(_oSelPop.pReturnArray[_iItRate]) > 0 ? _oSelPop.pReturnArray[_iItRate] : _oSelPop.pReturnArray[_iRate]) : _oSelPop.pReturnArray[_iRate]);
                                //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- Start
                                if (this.llgpuom == true)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = (_iItRate > 0 ? (Convert.ToDecimal(_oSelPop.pReturnArray[_iItRate]) > 0 ? _oSelPop.pReturnArray[_iItRate] : _oSelPop.pReturnArray[_iRate]) : _oSelPop.pReturnArray[_iRate]);
                                }

                                if (_commonDs.Tables.Contains("Lcode") == true && _commonDs.Tables["Item"].Columns.Contains("stkunit") == true)
                                {
                                    string _mRateUnit = _oSelPop.pReturnArray[2].ToString();
                                    string _mGrpUnit = _oSelPop.pReturnArray[3].ToString();
                                    cSql = "select Top 1 * from GroupUOM where GroupUom ='" + _mGrpUnit + "' and BaseUOM = '" + _mRateUnit + "' and isDefault=1";
                                    _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                                    if (_dt.Rows.Count > 0)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["stkunit"] = _mRateUnit;
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _dt.Rows[0]["SubUom"].ToString();

                                        this.Update_Conv_Qty("GRPUNT", this.BindingContext[_commonDs, "Item"].Position);
                                    }
                                    else
                                    {
                                        cSql = "select Top 1 RateUnit, p_unit, s_unit from it_mast where It_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString();
                                        _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                                        if (_dt.Rows.Count > 0)
                                        {
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["stkunit"] = _dt.Rows[0]["rateunit"].ToString();
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _dt.Rows[0]["rateunit"].ToString();
                                        }
                                    }
                                }

                                cSql = " Select top 1 " + (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Iotrans"]) == true ? "prate" : "sRate") + " as gpuomrate from groupuomrate Where It_code=" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString() + " and subuom='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"].ToString().Trim() + "'";

                                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                                if (_dt != null)
                                {
                                    if (_dt.Rows.Count > 0)                 //Added by Shrikant S. on 19/10/2018 
                                    {                                       //Added by Shrikant S. on 19/10/2018 
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = Convert.ToDecimal(_dt.Rows[0]["gpuomrate"]);
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["rate"] = Convert.ToDecimal(_dt.Rows[0]["gpuomrate"]) / Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]);
                                    }                                       //Added by Shrikant S. on 19/10/2018                                                    
                                    //this.Update_Conv_Qty("QTY", this.BindingContext[_commonDs, "Item"].Position);
                                }

                                //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- End

                                //_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Tax_Name"] = _oSelPop.pReturnArray[_TaxName];     //***** Commented by Sachin N. S. on 29/06/2013 for Bug-14538 

                                //***** Added by Sachin N. S. on 29/06/2013 for Bug-14538 -- Start *****//
                                if (_lRateLevel == -1)
                                {
                                    string cSql1 = "Select top 1 Rate_Level From Ac_Mast where Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                                    _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                                    _lRateLevel = Convert.ToInt16(_dt.Rows[0]["Rate_Level"]);
                                    cTbl += ",It_Rate ";
                                }
                                _lRateLevel = _lRateLevel == -1 ? 0 : _lRateLevel;
                                if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                                {
                                    replDefaDiscChrgsValues("ITEM", _lRateLevel);
                                }
                                //***** Added by Sachin N. S. on 29/06/2013 for Bug-14538 -- End *****//

                                //***** Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- Start *****//
                                if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true && _iInclTx > 0)
                                {
                                    if (Convert.ToBoolean(_oSelPop.pReturnArray[_iInclTx]) == true)
                                    {
                                        decimal _npertnm = 0.00M;
                                        string _cpertNm = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Tax_Name"].ToString().Trim();
                                        if (_cpertNm != string.Empty)
                                        {
                                            DataRow _dr2;
                                            _dr2 = _commonDs.Tables["Stax_Mas"].Select("Tax_Name='" + _cpertNm + "'").FirstOrDefault();
                                            _npertnm = Convert.ToDecimal(_dr2["Level1"]);
                                        }
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]) * 100) / (100 + _npertnm);
                                    }
                                }
                                //***** Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- End *****//

                                //**** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- Start *****//
                                if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true && _iInclTx > 0 && _commonDs.Tables["Item"].Columns.Contains("CGST_PER") == true)
                                {
                                    if (Convert.ToBoolean(_oSelPop.pReturnArray[_iInclTx]) == true)
                                    {
                                        decimal _npertnm = 0.00M, _OrgRate = 0.00M;
                                        _npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["CGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["SGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["IGST_PER"]);
                                        _OrgRate = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_OrgRate * 100) / (100 + _npertnm);
                                        if (this.llgpuom == true)
                                        {
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = Math.Round((_OrgRate * 100) / (100 + _npertnm), 2);
                                        }
                                    }
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(_oSelPop.pReturnArray[_iInclTx]);
                                }
                                //**** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- End *****//
                            }
                        }   // ****** Added by Sachin N. S. on 20/08/2015 for Bug-26654

                        itemwiseTotal(_nRowIndex, 0);      // Added by Sachin N. S. on 14/01/2016 for Bug-27503
                        break;

                    //***** Added by Sachin N. S. on 20/06/2013 for Bug-14538 -- Start *****//
                    case "SALESTAX":
                        if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true)
                        {
                            _commonDs.Tables["Main"].Rows[0]["Tax_Name"] = _oSelPop.pReturnArray[0].ToString();
                            //_commonDs.Tables["Main"].Rows[0]["TaxPercent"] = _oSelPop.pReturnArray[1].ToString();     // Commented by Sachin N. S. on 25/07/2013 for Bug-14538

                            if (Convert.ToDecimal(_oSelPop.pReturnArray[1]) > 0)
                            {
                                var _currentRow = (from currentRw in _commonDs.Tables["HdDiscChrgs"].AsEnumerable()
                                                   where currentRw.Field<string>("Code") == "S"
                                                   select currentRw).FirstOrDefault();
                                _currentRow["Head_nm"] = _oSelPop.pReturnArray[0].ToString();
                                _currentRow["Def_Pert"] = Convert.ToDecimal(_oSelPop.pReturnArray[1]);
                                this.CalculateHeaderTax(_currentRow["Fld_nm"].ToString());
                                this.RefreshControls();
                            }
                        }
                        if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                        {
                            _commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentRow.Index]["Tax_Name"] = _oSelPop.pReturnArray[0].ToString();
                            _commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentRow.Index]["TaxPer"] = Convert.ToDecimal(_oSelPop.pReturnArray[1]);
                        }
                        itemwiseTotal(dgvItemDetails.CurrentCell.RowIndex, dgvItemDetails.CurrentCell.ColumnIndex);      // Changed by Sachin N. S. on 15/01/2016 for Bug-27503
                        break;
                    //***** Added by Sachin N. S. on 20/06/2013 for Bug-14538 -- End *****//

                    case "PRINTBILL":
                        if (_oSelPop.pReturnArray[0].ToString() != "")
                        {
                            PrintBill(Convert.ToInt64(_oSelPop.pReturnArray[0].ToString()));
                            MessageBox.Show("Bill No. " + _oSelPop.pReturnArray[1].ToString() + " printed successfully.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No bills selected to print.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    // ***** Added by Sachin N. S. on 21/01/2016 for Bug-27503 -- Start
                    case "CANCELBILL":
                        this.pnlPrintBill.Visible = true;
                        this.lblPrintBill.Visible = true;
                        this.pgrsBar.Visible = true;
                        this.lblPrintBill.Text = "Cancelling Bill......";

                        if (Convert.ToInt64(_oSelPop.pReturnArray[0]) == 0)
                        {
                            MessageBox.Show("No bill selected to Cancel.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (clsInsUpdDelPrint.CancelRecords(Convert.ToInt64(_oSelPop.pReturnArray[0])) == false)
                            {
                                MessageBox.Show("Could not cancel the Bill. Try again...", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Bill Cancelled Successfully", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        this.pnlPrintBill.Visible = false;
                        this.lblPrintBill.Visible = false;
                        this.pgrsBar.Visible = false;
                        break;
                    // ***** Added by Sachin N. S. on 21/01/2016 for Bug-27503 -- End

                    //***** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- Start *****//
                    case "GROUPUNIT":
                        string _munit = string.Empty;
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _oSelPop.pReturnArray[0].ToString();
                        _munit = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"].ToString();

                        cSql = " Select top 1 " + (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Iotrans"]) == true ? "prate" : "sRate") + " as gpuomrate from groupuomrate Where It_code=" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString() + " and subuom='" + _munit.Trim() + "'";

                        _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                        bool lDispB4 = Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]);
                        if (_dt != null)
                        {
                            if (lDispB4 == false)
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gprate"] = Convert.ToDecimal(_dt.Rows[0]["gpuomrate"]);
                            }
                            this.Update_Conv_Qty("GRPUNT", this.BindingContext[_commonDs, "Item"].Position);
                        }

                        if (lDispB4 == false)
                        {
                            cSql = " Select top 1 InclStTax from It_Mast Where It_code=" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString();
                            _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                            bool _iInclTx1 = Convert.ToBoolean(_dt.Rows[0]["InclStTax"]);
                            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true && _iInclTx1 == true && _commonDs.Tables["Item"].Columns.Contains("CGST_PER") == true)
                            {
                                //decimal _npertnm = 0.00M, _OrgRate = 0.00M;
                                //_npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["CGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["SGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["IGST_PER"]);
                                //_OrgRate = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                //_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_OrgRate * 100) / (100 + _npertnm);
                                //if (this.llgpuom == true)
                                //{
                                //    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = Math.Round(((_OrgRate * 100) / (100 + _npertnm)) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]), 2);
                                //}
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = _iInclTx1;
                            }
                        }
                        itemwiseTotal(dgvItemDetails.CurrentCell.RowIndex, dgvItemDetails.CurrentCell.ColumnIndex);
                        break;
                    //***** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- End *****//
                }
            }
        }

        #region Read Barcode
        private string ReadBarcode(string _barCode)
        {
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            string cSrchCol = "", cDispCol = "", cRetCol = "";
            string cTbl = "", cCond = "", cFldList = "", xcFldList = "", xcSrchCol = "", xcDispCol = "", xcRetCol = "";
            int _lRateLevel = -1;    //***** Added by Sachin N. S. on 18/01/2016 for Bug-27503

            DataTable _dt1 = new DataTable();
            DataTable _dt = new DataTable();
            xcFldList = ",It_mast.Rate,It_mast.Tax_Name,It_Mast.InclSTTax ";                    // Changed by Sachin N. S. on 18/01/2016 for Bug-27503
            xcDispCol = ",Rate:Goods Rate,Tax_Name:Tax Name,InclSTTax:Incl of GST";         // Changed by Sachin N. S. on 18/01/2016 for Bug-27503
            xcRetCol = ",Rate,Tax_Name,InclSTTax";                                              // Changed by Sachin N. S. on 18/01/2016 for Bug-27503
            cTbl = " It_Mast ";
            cCond = " It_Mast.It_code!=0 and It_mast.isService=0 ";         //***** Changed by Sachin N. S. on 26/08/2015 for Bug-26654 //***** Changed by Sachin N. S. on 18/08/2018 for Bug-31756

            //******* Checking for Rate in Item Rate master -- Start *******\\
            if ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true)
            {
                string cSql1 = "Select top 1 Rate_Level From Ac_mast where Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                _lRateLevel = Convert.ToInt16(_dt.Rows[0]["Rate_Level"]);           // Added by Sachin N. S. on 18/01/2016 for Bug-27503
                if (Convert.ToInt16(_dt.Rows[0]["Rate_Level"]) != 0)    //***** Changed by Sachin N. S. on 26/08/2015 for Bug-26654
                {
                    cCond += " And (It_mast.It_code = It_rate.It_code And (It_rate.Rlevel = " + _dt.Rows[0]["Rate_Level"].ToString() + ")) ";
                }
                else
                {
                    cCond += " And (It_mast.It_code = It_rate.It_code And It_rate.Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString() + ") ";
                }

                cTbl += ",It_Rate ";
                xcFldList += ",It_Rate.It_Rate ";
                xcDispCol += ",It_Rate:Party Rate";
                xcRetCol += ",It_Rate";
            }
            //******* Checking for Rate in Item Rat`e master -- End *******\\

            //******* Searching for Barcode value in BarcodeTran Table -- Start *******\\

            //***** Added by Sachin N. S. on 11/09/2018 for Bug-31756 -- Start
            string cCondBrCd = string.Empty, cTblBrCd = string.Empty, xcFldListBrCd = string.Empty, xcDispColBrCd = string.Empty, xcRetColBrCd = string.Empty;
            string cCond1 = string.Empty;
            //***** Added by Sachin N. S. on 11/09/2018 for Bug-31756 -- End

            if (_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() != "")
            {
                //***** Changed by Sachin N. S. on 11/09/2018 for Bug-31756 -- Start
                //cCond += " And ((BarCodeTran.Entry_ty='IM' and BarCodeTran." + _commonDs.Tables["Lcode"].Rows[0]["POSBarcode"].ToString().Trim() + " = '" + _barCode.ToString() + "' and BarCodeTran.Tran_cd = It_Mast.It_Code ) )";
                //cTbl += ",BarcodeTran ";
                //xcFldList += ",BarCodeTran." + _commonDs.Tables["Lcode"].Rows[0]["POSBarcode"].ToString().Trim();
                //xcDispCol += "," + _commonDs.Tables["Lcode"].Rows[0]["POSBarcode"].ToString().Trim() + ":Bar Code";
                //xcRetCol += "," + _commonDs.Tables["Lcode"].Rows[0]["POSBarcode"].ToString().Trim();

                cCondBrCd = " And ((BarCodeTran.Entry_ty='IM' and BarCodeTran." + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() + " = '" + _barCode.ToString() + "' and BarCodeTran.Tran_cd = It_Mast.It_Code ) )";
                cTblBrCd = ",BarcodeTran ";
                xcFldListBrCd = ",BarCodeTran." + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim();
                xcDispColBrCd = "," + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() + ":Bar Code";
                xcRetColBrCd = "," + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim();
                //***** Changed by Sachin N. S. on 11/09/2018 for Bug-31756 -- End
            }
            //else
            //{
            if (_barCode.ToString() != string.Empty)
            {
                //cCond += " And It_Mast.It_Name = '" + _barCode.ToString() + "' ";
                cCond1 = " And It_Mast.It_Name like '%" + _barCode.ToString() + "%' ";        // Changed by Sachin N. S. for New Pharma Retailer Changes
            }
            //}
            //******* Searching for Barcode value in BarcodeTran Table -- End *******\\

            cFldList = _commonDs.Tables["Lcode"].Rows[0]["It_fields"].ToString();
            string[][] cFldLst = cFldList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(':')).ToArray();
            var cItCode =
                from _cFldLst in cFldLst
                where _cFldLst[0].ToUpper().Contains("IT_CODE")
                select _cFldLst[0];
            if (cItCode.Count<string>() == 0)
            {
                xcFldList += ",It_mast.It_Code ";
            }

            ////Added for New field reference in Barcode -- 04/04/2013 -- Start
            //if (_commonDs.Tables["Lcode"].Columns["POSBarcode"]!=null)
            //{
            //    if (_commonDs.Tables["Lcode"].Rows[0]["POSBarcode"] != string.Empty)
            //    {
            //        cFldList = ">>"+_commonDs.Tables["Lcode"].Rows[0]["POSBarcode"].ToString()+"<<";
            //        string[][] cFldLst1 = cFldList.Split(new[] { ">><<" }, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(':')).ToArray();
            //    }
            //}
            ////Added for New field reference in Barcode -- 04/04/2013 -- End

            cCond += " And (It_mast.ldeactive = 0 Or (It_mast.ldeactive = 1 And It_mast.deactfrom > '" + txtInvoiceDt.Text.ToString() + "' )) ";
            cSql = "set dateformat dmy; select " + cFldLst[0][0] + xcFldList + " from " + cTbl + " where " + cCond + cCond1 + " order by It_Name ";     // Changed by Sachin N. S. for New Pharma Retailer Changes
            //cSql = "set dateformat dmy; select " + cFldLst[0][0] + xcFldList + " from " + cTbl + " where " + cCond + " order by It_Name ";
            //cFrmCap = "Select Item";
            cSrchCol = "It_Name" + xcSrchCol;
            cDispCol = "It_Name:Goods" + xcDispCol;
            cRetCol = "It_Code,It_Name" + xcRetCol;

            //***** Changed by Sachin N. S. on 11/09/2018 for Bug-31756 -- Start
            //_dt = _oDataAccess.GetDataTable(cSql, null, 50);      
            DataTable _dtNew;
            _dtNew = _oDataAccess.GetDataTable(cSql, null, 50);
            if (_dtNew != null)
            {
                if (_dtNew.Rows.Count == 0)
                {
                    cDispCol += xcDispColBrCd;
                    cRetCol += xcRetColBrCd;

                    cSql = "set dateformat dmy; select " + cFldLst[0][0] + xcFldList + xcFldListBrCd + " from " + cTbl + cTblBrCd + " where " + cCond + cCondBrCd + " order by It_Name ";
                    _dtNew = _oDataAccess.GetDataTable(cSql, null, 50);
                }
            }
            _dt = _dtNew;
            //***** Changed by Sachin N. S. on 11/09/2018 for Bug-31756 -- End

            string clAdd = "ADD";
            if (_dt != null)
            {
                if (_dt.Rows.Count > 0)
                {
                    // ****** Added by Sachin N. S. on 26/09/2018 for New Pharma Retailer Changes -- Start
                    if (_dt.Rows.Count > 1)
                    {
                        string cFrmCap = "Select Goods";

                        udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
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
                            _dtNew = _dt.Select("It_Code=" + _oSelPop.pReturnArray[0].ToString().Trim()).CopyToDataTable();
                            _dt = _dtNew;
                        }
                    }
                    // ****** Added by Sachin N. S. on 26/09/2018 for New Pharma Retailer Changes -- End

                    int _nRowIndex = this.BindingContext[_commonDs, "Item"].Position;       // Added by Sachin N. S. on 15/01/2016 for Bug-27503
                    _curRowNo = _nRowIndex;     // Added by Sachin N. S. on 19/01/2016 for Bug-27503
                    if (_commonDs.Tables["Item"].Rows.Count > 0)
                    {
                        var _currentRow = (from currentRw in _commonDs.Tables["Item"].AsEnumerable()
                                           where currentRw.Field<decimal>("It_code") == Convert.ToDecimal(_dt.Rows[0]["It_Code"])
                                           select currentRw).FirstOrDefault();
                        if (_currentRow != null && _currentRow.Table.Rows.Count > 0)
                        {
                            //********* Added by Sachin N. S. on 19/02/2016 for Bug-27503 -- Start
                            _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_dt.Rows[0]["It_Code"]) + "'")[0]);
                            if (Stock_Checking(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex) < 0)
                            {
                                MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clAdd = "NOTFOUND";
                            }
                            else
                            {
                                _currentRow["Qty"] = Convert.ToDecimal(_currentRow["Qty"]) + 1;
                                //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- Start
                                if (this.llgpuom == true)
                                {
                                    _currentRow["gpQty"] = Convert.ToDecimal(_currentRow["gpQty"]) + 1;
                                    //this.Update_Conv_Qty("QTY", _curRowNo);
                                }
                                //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- End
                                clAdd = "EXIST";
                            }
                            //********* Added by Sachin N. S. on 19/02/2016 for Bug-27503 -- End
                            _curRowNo = _nRowIndex;     // Added by Sachin N. S. on 19/01/2016 for Bug-27503
                        }
                    }

                    if (clAdd == "ADD")
                    {
                        if (Stock_Checking(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex) < 0)
                        {
                            MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clAdd = "NOTFOUND";
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _dt.Rows[0]["It_Code"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _dt.Rows[0]["It_name"].ToString();

                            string[] _arr = cRetCol.Split(',');
                            int _iRate = Array.IndexOf(_arr, "Rate");
                            int _iItRate = Array.IndexOf(_arr, "It_Rate");
                            int _iInclTx = Array.IndexOf(_arr, "InclSTTax");        // Added by Sachin N. S. on 18/01/2016 for Bug-27503
                            //int _TaxName = Array.IndexOf(_arr, "Tax_Name");           // Commented by Sachin N. S. on 18/01/2016 for Bug-27503

                            // ******* Commented by Sachin N. S. on 25/07/2013 for Bug-14538 -- Start ******* //
                            //if (_dt.Rows[0]["Tax_Name"].ToString() != "")
                            //{
                            //    cSql = "SET DATEFORMAT DMY; SELECT LEVEL1 FROM STAX_MAS WHERE ENTRY_TY='PS' AND TAX_NAME = '" + _dt.Rows[0]["Tax_Name"].ToString() + "' and '" + this.txtInvoiceDt.Value.ToString() + "' between Wefstkfrom and Wefstkto ";
                            //    _dt1 = _oDataAccess.GetDataTable(cSql, null, 50);
                            //    if (_dt1.Rows.Count > 0)
                            //    {
                            //        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["TAXPERCENT"] = _dt1.Rows[0]["Level1"];
                            //    }
                            //}
                            // ******* Commented by Sachin N. S. on 25/07/2013 for Bug-14538 -- End ******* //
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_iItRate > 0 ? (Convert.ToInt16(_dt.Rows[0]["It_Rate"]) > 0 ? _dt.Rows[0]["It_Rate"] : _dt.Rows[0]["Rate"]) : _dt.Rows[0]["Rate"]);
                            //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- Start
                            if (this.llgpuom == true)
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = (_iItRate > 0 ? (Convert.ToInt16(_dt.Rows[0]["It_Rate"]) > 0 ? _dt.Rows[0]["It_Rate"] : _dt.Rows[0]["Rate"]) : _dt.Rows[0]["Rate"]);
                            }
                            DataTable _dtt1;
                            if (_commonDs.Tables.Contains("Lcode") == true && _commonDs.Tables["Item"].Columns.Contains("stkunit") == true)
                            {
                                cSql = "select Top 1 RateUnit, p_unit, s_unit from it_mast where It_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString();
                                _dtt1 = _oDataAccess.GetDataTable(cSql, null, 50);
                                if (_dtt1.Rows.Count > 0)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["stkunit"] = _dtt1.Rows[0]["rateunit"].ToString();

                                    if (Convert.ToBoolean(_commonDs.Tables["lcode"].Rows[0]["gpuomapp"]) == false)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _dtt1.Rows[0]["rateunit"].ToString();
                                    }
                                }
                            }

                            cSql = " Select top 1 " + (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Iotrans"]) == true ? "prate" : "sRate") + " as gpuomrate from groupuomrate Where It_code=" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString() + " and subuom='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"].ToString().Trim() + "'";

                            _dtt1 = _oDataAccess.GetDataTable(cSql, null, 50);
                            if (_dtt1 != null)
                            {
                                if (_dtt1.Rows.Count > 0)
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gprate"] = Convert.ToDecimal(_dtt1.Rows[0]["gpuomrate"]);
                                //this.Update_Conv_Qty("QTY", this.BindingContext[_commonDs, "Item"].Position);
                            }
                            //**** Added by Sachin N. S. on 03/10/2018 for Pharma Retailer Changes -- End

                            bool _lInclStTax = (_iInclTx > 0 ? Convert.ToBoolean(_dt.Rows[0]["InclSTTax"]) : false);
                            //_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Tax_Name"] = _dt.Rows[0]["Tax_Name"].ToString().Trim() == "" ? "NO-TAX" : _dt.Rows[0]["Tax_Name"].ToString().Trim();          // Commented by Sachin N. S. on 18/01/2016 for Bug-27503

                            //***** Added by Sachin N. S. on 29/06/2013 for Bug-14538 -- Start *****//
                            if (_lRateLevel == -1)
                            {
                                string cSql1 = "Select top 1 Rate_Level From Ac_Mast where Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                                _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                                _lRateLevel = Convert.ToInt16(_dt.Rows[0]["Rate_Level"]);
                                cTbl += ",It_Rate ";
                            }
                            _lRateLevel = _lRateLevel == -1 ? 0 : _lRateLevel;
                            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                            {
                                replDefaDiscChrgsValues("ITEM", _lRateLevel);
                            }
                            //***** Added by Sachin N. S. on 29/06/2013 for Bug-14538 -- End *****//

                            //***** Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- Start *****//
                            if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true && _iInclTx > 0)
                            {
                                if (_lInclStTax == true)
                                {
                                    decimal _npertnm = 0.00M;
                                    string _cpertNm = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Tax_Name"].ToString().Trim();
                                    if (_cpertNm != string.Empty)
                                    {
                                        DataRow _dr2;
                                        _dr2 = _commonDs.Tables["Stax_Mas"].Select("Tax_Name='" + _cpertNm + "'").FirstOrDefault();
                                        _npertnm = Convert.ToDecimal(_dr2["Level1"]);
                                    }
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]) * 100) / (100 + _npertnm);
                                }
                            }
                            //***** Added by Sachin N. S. on 18/01/2016 for Bug-27503 -- End *****//

                            //**** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- Start *****//
                            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true && _iInclTx > 0 && _commonDs.Tables["Item"].Columns.Contains("CGST_PER") == true)
                            {
                                //if (_lInclStTax == true)
                                //{
                                //    decimal _npertnm = 0.00M;
                                //    _npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["CGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["SGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["IGST_PER"]);

                                //    decimal _OrgRate = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                //    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_OrgRate * 100) / (100 + _npertnm);
                                //    if (this.llgpuom == true)
                                //    {
                                //        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = Math.Round((_OrgRate * 100) / (100 + _npertnm), 2);
                                //    }
                                //}
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = _lInclStTax;
                            }
                            //**** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- End *****//
                        }
                    }

                    itemwiseTotal(_nRowIndex, 0);       // Added by Sachin N. S. on 15/01/2016 for Bug-27503
                }
                else
                {
                    MessageBox.Show("Validation failed for either of following reasons: \n" +
                        "1. Goods not found in the Supply Master. \n" +
                        (((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true) ? "2. Rate not defined in the Price list master. \n" : "") +
                        ((_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() != "") ? (((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true) ? "3." : "2.") + " Barcode value not matching with the value defined in Goods Master." : ""), CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clAdd = "NOTFOUND";
                }
            }
            //else
            //{
            //    MessageBox.Show("Goods not found in the Supply Master.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = 0;
            //    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = "";
            //    clAdd = "NOTFOUND";
            //}

            return clAdd;
        }

        //***** Added by Sachin N. S. on 01/12/2018 for Pharma Retailer Changes -- Start
        private string ReadBarcodeGen(string _barCode)
        {
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            string cSrchCol = "", cDispCol = "", cRetCol = "";
            string cTbl = "", cCond = "", cFldList = "", xcFldList = "", xcSrchCol = "", xcDispCol = "", xcRetCol = "";
            int _lRateLevel = -1;

            DataTable _dt1 = new DataTable();
            DataTable _dt = new DataTable();
            DataRow _brCodeMast = null;
            DataRow _bcTran = null;
            DataRow _dt12 = null;

            string _brcd = _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim();
            if (_brcd != "")
            {
                cSql = "Select top 1 BC_MT,BC_HD,Entry_Ty,Tbl_Nm1,Fld_Nm2 From BarCodeMast where Fld_Nm2 = '" + _brcd + "'";
                _brCodeMast = _oDataAccess.GetDataRow(cSql, null, 50);

                cSql = "Select top 1 Entry_Ty,Tran_cd,ItSerial,iTran_cd From BarCodeTran where " + _brcd + " = '" + _barCode + "'";
                _bcTran = _oDataAccess.GetDataRow(cSql, null, 50);
                string _cCond = "";

                switch (_brCodeMast["BC_MT"].ToString())
                {
                    case "M":
                        cSql = "Select [NAME] From Sys.Columns Where Object_Id=Object_Id('" + _brCodeMast["Tbl_Nm1"].ToString().Trim() + "') and is_identity=1 ";
                        _dt12 = _oDataAccess.GetDataRow(cSql, null, 50);
                        _cCond = _dt12["Name"].ToString().Trim() + " = " + _bcTran["Tran_cd"].ToString();
                        break;
                    case "T":
                        if (_brCodeMast["BC_HD"].ToString() == "H")
                            _cCond = " Entry_ty = '" + _bcTran["Entry_Ty"].ToString() + "' and Tran_Cd = " + _bcTran["Tran_cd"].ToString();
                        else
                            _cCond = " Entry_ty = '" + _bcTran["Entry_Ty"].ToString() + "' and Tran_Cd = " + _bcTran["Tran_cd"].ToString() + " and ItSerial='" + _bcTran["ItSerial"].ToString() + "' ";
                        break;
                    default:
                        _cCond = "";
                        break;
                }

                cSql = " Select Top 1 * From " + _brCodeMast["Tbl_Nm1"].ToString() + " where " + _cCond;
                _dt12 = _oDataAccess.GetDataRow(cSql, null, 50);

            }

            int _itCode = 0;
            if (_dt12 != null)
            {
                _itCode = Convert.ToUInt16(_dt12["It_Code"]);
            }

            xcFldList = ",It_mast.Rate,It_mast.Tax_Name,It_Mast.InclSTTax ";
            xcDispCol = ",Rate:Goods Rate,Tax_Name:Tax Name,InclSTTax:Incl of GST";
            xcRetCol = ",Rate,Tax_Name,InclSTTax";
            cTbl = " It_Mast ";
            cCond = " It_Mast.It_code!=0 and It_mast.isService=0 ";

            //******* Checking for Rate in Item Rate Master -- Start *******\\
            if ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true)
            {
                string cSql1 = "Select top 1 Rate_Level From Ac_mast where Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                _lRateLevel = Convert.ToInt16(_dt.Rows[0]["Rate_Level"]);
                if (Convert.ToInt16(_dt.Rows[0]["Rate_Level"]) != 0)
                {
                    cCond += " And (It_mast.It_code = It_rate.It_code And (It_rate.Rlevel = " + _dt.Rows[0]["Rate_Level"].ToString() + ")) ";
                }
                else
                {
                    cCond += " And (It_mast.It_code = It_rate.It_code And It_rate.Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString() + ") ";
                }

                cTbl += ",It_Rate ";
                xcFldList += ",It_Rate.It_Rate ";
                xcDispCol += ",It_Rate:Party Rate";
                xcRetCol += ",It_Rate";
            }
            //******* Checking for Rate in Item Rate Master -- End *******\\

            //******* Searching for Barcode value in BarcodeTran Table -- Start *******\\

            string cCondBrCd = string.Empty, cTblBrCd = string.Empty, xcFldListBrCd = string.Empty, xcDispColBrCd = string.Empty, xcRetColBrCd = string.Empty;
            string cCond1 = string.Empty;

            if (_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() != "")
            {
                cCondBrCd = " And It_Mast.It_Code = " + _itCode.ToString();
            }

            if (_barCode.ToString() != string.Empty)
            {
                cCond1 = " And It_Mast.It_Name like '%" + _barCode.ToString() + "%' ";
            }

            //*******Searching for Barcode value in BarcodeTran Table-- End * ******\\


            cFldList = _commonDs.Tables["Lcode"].Rows[0]["It_fields"].ToString();
            string[][] cFldLst = cFldList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(':')).ToArray();
            var cItCode =
                from _cFldLst in cFldLst
                where _cFldLst[0].ToUpper().Contains("IT_CODE")
                select _cFldLst[0];
            if (cItCode.Count<string>() == 0)
            {
                xcFldList += ",It_mast.It_Code ";
            }

            cCond += " And (It_mast.ldeactive = 0 Or (It_mast.ldeactive = 1 And It_mast.deactfrom > '" + txtInvoiceDt.Text.ToString() + "' )) ";
            cSql = "set dateformat dmy; select " + cFldLst[0][0] + xcFldList + " from " + cTbl + " where " + cCond + cCond1 + " order by It_Name ";
            cSrchCol = "It_Name" + xcSrchCol;
            cDispCol = "It_Name:Goods" + xcDispCol;
            cRetCol = "It_Code,It_Name" + xcRetCol;

            DataTable _dtNew;
            _dtNew = _oDataAccess.GetDataTable(cSql, null, 50);
            if (_dtNew != null)
            {
                if (_dtNew.Rows.Count == 0)
                {
                    cDispCol += xcDispColBrCd;
                    cRetCol += xcRetColBrCd;

                    cSql = "set dateformat dmy; select " + cFldLst[0][0] + xcFldList + xcFldListBrCd + " from " + cTbl + cTblBrCd + " where " + cCond + cCondBrCd + " order by It_Name ";
                    _dtNew = _oDataAccess.GetDataTable(cSql, null, 50);
                }
            }
            _dt = _dtNew;

            string clAdd = "ADD";
            if (_dt != null)
            {
                if (_dt.Rows.Count > 0)
                {
                    if (_dt.Rows.Count > 1)
                    {
                        string cFrmCap = "Select Goods";

                        udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
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
                            _dtNew = _dt.Select("It_Code=" + _oSelPop.pReturnArray[0].ToString().Trim()).CopyToDataTable();
                            _dt = _dtNew;
                        }
                    }

                    int _nRowIndex = this.BindingContext[_commonDs, "Item"].Position;
                    _curRowNo = _nRowIndex;
                    if (_commonDs.Tables["Item"].Rows.Count > 0)
                    {
                        var _currentRow = (from currentRw in _commonDs.Tables["Item"].AsEnumerable()
                                           where currentRw.Field<decimal>("It_code") == Convert.ToDecimal(_dt.Rows[0]["It_Code"])
                                           select currentRw).FirstOrDefault();
                        if (_currentRow != null && _currentRow.Table.Rows.Count > 0)
                        {
                            _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_dt.Rows[0]["It_Code"]) + "'")[0]);
                            if (Stock_Checking(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex) < 0)
                            {
                                MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clAdd = "NOTFOUND";
                            }
                            else
                            {
                                _currentRow["Qty"] = Convert.ToDecimal(_currentRow["Qty"]) + 1;
                                if (this.llgpuom == true)
                                {
                                    _currentRow["gpQty"] = Convert.ToDecimal(_currentRow["gpQty"]) + 1;
                                }
                                clAdd = "EXIST";
                            }
                            _curRowNo = _nRowIndex;
                        }
                    }

                    if (clAdd == "ADD")
                    {
                        if (Stock_Checking(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex) < 0)
                        {
                            MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clAdd = "NOTFOUND";
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _dt.Rows[0]["It_Code"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _dt.Rows[0]["It_name"].ToString();

                            //**** Added by Sachin N. S. on 01/12/2018 for Pharma Retailer Changes -- Start
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = _dt12["BatchNo"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = _dt12["MfgDt"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = _dt12["ExpDt"].ToString();

                            //**** Added by Sachin N. S. on 01/12/2018 for Pharma Retailer Changes -- End

                            string[] _arr = cRetCol.Split(',');
                            int _iRate = Array.IndexOf(_arr, "Rate");
                            int _iItRate = Array.IndexOf(_arr, "It_Rate");
                            int _iInclTx = Array.IndexOf(_arr, "InclSTTax");

                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_iItRate > 0 ? (Convert.ToInt16(_dt.Rows[0]["It_Rate"]) > 0 ? _dt.Rows[0]["It_Rate"] : _dt.Rows[0]["Rate"]) : _dt.Rows[0]["Rate"]);
                            if (this.llgpuom == true)
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = (_iItRate > 0 ? (Convert.ToInt16(_dt.Rows[0]["It_Rate"]) > 0 ? _dt.Rows[0]["It_Rate"] : _dt.Rows[0]["Rate"]) : _dt.Rows[0]["Rate"]);
                            }
                            DataTable _dtt1;
                            if (_commonDs.Tables.Contains("Lcode") == true && _commonDs.Tables["Item"].Columns.Contains("stkunit") == true)
                            {
                                cSql = "select Top 1 RateUnit, p_unit, s_unit from it_mast where It_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString();
                                _dtt1 = _oDataAccess.GetDataTable(cSql, null, 50);
                                if (_dtt1.Rows.Count > 0)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["stkunit"] = _dtt1.Rows[0]["rateunit"].ToString();

                                    if (Convert.ToBoolean(_commonDs.Tables["lcode"].Rows[0]["gpuomapp"]) == false)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _dtt1.Rows[0]["rateunit"].ToString();
                                    }
                                }
                            }

                            cSql = " Select top 1 " + (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Iotrans"]) == true ? "prate" : "sRate") + " as gpuomrate from groupuomrate Where It_code=" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString() + " and subuom='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"].ToString().Trim() + "'";

                            _dtt1 = _oDataAccess.GetDataTable(cSql, null, 50);
                            if (_dtt1 != null)
                            {
                                if (_dtt1.Rows.Count > 0)
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gprate"] = Convert.ToDecimal(_dtt1.Rows[0]["gpuomrate"]);
                            }

                            bool _lInclStTax = (_iInclTx > 0 ? Convert.ToBoolean(_dt.Rows[0]["InclSTTax"]) : false);

                            if (_lRateLevel == -1)
                            {
                                string cSql1 = "Select top 1 Rate_Level From Ac_Mast where Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                                _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                                _lRateLevel = Convert.ToInt16(_dt.Rows[0]["Rate_Level"]);
                                cTbl += ",It_Rate ";
                            }
                            _lRateLevel = _lRateLevel == -1 ? 0 : _lRateLevel;
                            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                            {
                                replDefaDiscChrgsValues("ITEM", _lRateLevel);
                            }

                            if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true && _iInclTx > 0)
                            {
                                if (_lInclStTax == true)
                                {
                                    decimal _npertnm = 0.00M;
                                    string _cpertNm = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Tax_Name"].ToString().Trim();
                                    if (_cpertNm != string.Empty)
                                    {
                                        DataRow _dr2;
                                        _dr2 = _commonDs.Tables["Stax_Mas"].Select("Tax_Name='" + _cpertNm + "'").FirstOrDefault();
                                        _npertnm = Convert.ToDecimal(_dr2["Level1"]);
                                    }
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]) * 100) / (100 + _npertnm);
                                }
                            }

                            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true && _iInclTx > 0 && _commonDs.Tables["Item"].Columns.Contains("CGST_PER") == true)
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = _lInclStTax;
                            }
                        }
                    }
                    itemwiseTotal(_nRowIndex, 0);
                }
                else
                {
                    MessageBox.Show("Validation failed for either of following reasons: \n" +
                        "1. Goods not found in the Supply Master. \n" +
                        (((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true) ? "2. Rate not defined in the Price list master. \n" : "") +
                        ((_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() != "") ? (((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true) ? "3." : "2.") + " Barcode value not matching with the value defined in Goods Master." : ""), CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clAdd = "NOTFOUND";
                }
            }
            return clAdd;
        }

        //***** Added by Sachin N. S. on 01/12/2018 for Pharma Retailer Changes -- End
        #endregion Read Barcode

        #endregion Calling Popup Selection Screen

        #region Stock Checking
        private decimal Stock_Checking(long _It_Code, DateTime _InvDt, decimal _CurstkVal, int _rowIndex)
        {
            decimal StkQty;

            if (CommonInfo.Neg_itBal == 0)
            {
                string cSql;
                cSql = "Set dateformat DMY;";
                cSql += " Select SUM(case when b.inv_stk='+' then a.Qty else case when b.Inv_stk='-' then -Qty else 0 end end ) as Qty From It_balw A,Lcode B where ";
                cSql += " a.It_code = " + _It_Code.ToString();
                cSql += " and a.Date <= '" + _InvDt.ToString() + "'";
                cSql += " and a.Entry_ty = b.Entry_ty and b.Inv_stk!='' ";
                cSql += " and [Rule] = '" + _commonDs.Tables["Main"].Rows[0]["Rule"].ToString() + "'";     // Changed by Sachin N. S. on 22/11/2018 for Pharma Retailer Changes
                cSql += " and [Ware_nm] = '" + _commonDs.Tables["Item"].Rows[_rowIndex]["Ware_nm"].ToString() + "'";         // Added by Sachin N. S. on 23/11/2018 for Pharma Retailer Changes

                DataTable _dt;
                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                decimal.TryParse(_dt.Rows[0]["Qty"].ToString(), out StkQty);
                StkQty = StkQty - Convert.ToDecimal(_commonDs.Tables["Item"].AsEnumerable().Where((row, index) => Convert.ToInt64(row["It_Code"]) == _It_Code && row["Qty"] != null && index != _rowIndex).Sum(row => Convert.ToDecimal(row["Qty"]))) - _CurstkVal;
            }
            else
            {
                StkQty = 1;     // Added by Sachin N. S. on 14/01/2016 for Bug-27503 for bypassing Stock checking
            }
            return StkQty;
        }
        #endregion Stock Checking

        // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- Start ****** //
        #region Calculate Headerwise Taxes
        private void CalculateHeaderTax(string _HeadFld)
        {
            bool _calcTax = false;
            decimal mcalamt = 0, mpert = 0;
            string mfld_nm = "", mper_nm = "";

            //****** Added by Sachin N. S. on 25/05/2013 for Bug-14538 -- Start ******//
            _commonDs.Tables["Main"].Rows[0]["Tot_Deduc"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_tax"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_examt"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_add"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_nontax"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_fdisc"] = 0;
            //****** Added by Sachin N. S. on 25/05/2013 for Bug-14538 -- End ******//

            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["LCode"].Rows[0]["Stax_Tran"]) == true)
            {
                foreach (DataRow _dr in _commonDs.Tables["HdDiscChrgs"].Rows)
                {
                    if (Convert.ToBoolean(_dr["Att_File"]) == false)
                    {
                        continue;
                    }

                    if (_dr["Fld_nm"].ToString() == _HeadFld || _HeadFld == string.Empty)
                    {
                        _calcTax = true;
                    }

                    mfld_nm = _dr["fld_nm"].ToString().Trim();      // Changed by Sachin N. S. on 25/05/2013 for Bug-14538
                    if (_calcTax)
                    {
                        mper_nm = _dr["pert_name"].ToString().Trim();
                        mpert = Convert.ToDecimal(_dr["Def_Pert"]);
                        if (mper_nm != "" && _commonDs.Tables["Main"].Rows[0][mper_nm] != null && Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0][mper_nm]) != 0)
                        {
                            mpert = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0][mper_nm]);
                        }

                        decimal mamt = 0;       // Added by Sachin N. S. on 14/01/2016 for Bug-27503
                        mamt = Convert.ToDecimal(_dr["Def_Amt"]);   // Added by Sachin N. S. on 14/01/2016 for Bug-27503
                        if (mpert > 0)
                        {
                            //if (_dr["AmtExpr"].ToString() != string.Empty)
                            //{
                            //    string _amtExpr = _dr["AmtExpr"].ToString().Replace("Main_vw.", "");
                            //    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]);
                            //}
                            //else
                            //{
                            switch (_dr["Code"].ToString())
                            {
                                case "D":
                                    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]);
                                    break;
                                case "T":
                                    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]);
                                    break;
                                case "E":
                                    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]);
                                    break;
                                case "A":
                                    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Tot_Examt"]);
                                    break;
                                case "S":
                                    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Tot_Examt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_add"]);
                                    break;
                                case "N":
                                    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Tot_Examt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_add"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["taxamt"]);
                                    break;
                                case "F":
                                    mcalamt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["gro_amt"]) - Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_deduc"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Tot_Examt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_add"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["taxamt"]) + Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_nontax"]);
                                    break;
                            }
                            //}

                            string mdef_qty = "";
                            decimal mdef_qtyv = 0;
                            switch (_dr["disp_sign"].ToString())
                            {
                                case "@":
                                    mdef_qty = "Item_vw.QTY";
                                    mdef_qtyv = 0;
                                    mdef_qtyv = Convert.ToDecimal(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null).Sum(row => Convert.ToDecimal(row["Qty"])));
                                    mamt = mdef_qtyv * mpert;
                                    break;
                                case "%":
                                    mamt = mcalamt * (mpert / 100);
                                    break;
                            }

                            mamt = Math.Round(mamt, 2);
                            _dr["Def_amt"] = Convert.ToDecimal(mamt);
                            //_commonDs.Tables["Main"].Rows[0][_dr["Fld_Nm"].ToString().Trim()] = Convert.ToDecimal(mamt);      // Commented by Sachin N. S. on 14/01/2016 for Bug-27503
                        }
                        _commonDs.Tables["Main"].Rows[0][mfld_nm] = Convert.ToDecimal(mamt);        // Added by Sachin N. S. on 14/01/2016 for Bug-27503
                    }

                    if (_dr["Excl_Net"].ToString() != "D" || _dr["Excl_Net"].ToString() != "G" || _dr["Excl_Net"].ToString() != "A")
                    {
                        string ma_s = "";
                        switch (_dr["A_S"].ToString())
                        {
                            case "+":
                                if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                                {
                                    ma_s = "-";
                                }
                                else
                                {
                                    ma_s = _dr["a_s"].ToString();
                                }
                                break;

                            case "-":
                                if (_dr["CODE"].ToString() == "D" || _dr["CODE"].ToString() == "F")
                                {
                                    ma_s = "+";
                                }
                                else
                                {
                                    ma_s = _dr["a_s"].ToString();
                                }
                                break;

                            default:
                                ma_s = "+";
                                break;
                        }

                        decimal _fldVal = (ma_s == "+" ? +Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0][mfld_nm]) : -Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0][mfld_nm]));
                        switch (_dr["Code"].ToString())
                        {
                            case "D":
                                _commonDs.Tables["Main"].Rows[0]["Tot_Deduc"] = Math.Round(Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Tot_Deduc"]) + _fldVal, 2);
                                break;
                            case "T":
                                _commonDs.Tables["Main"].Rows[0]["tot_tax"] = Math.Round(Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_tax"]) + _fldVal, 2);
                                break;
                            case "E":
                                _commonDs.Tables["Main"].Rows[0]["tot_examt"] = Math.Round(Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_examt"]) + _fldVal, 2);
                                break;
                            case "A":
                                _commonDs.Tables["Main"].Rows[0]["tot_add"] = Math.Round(Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_add"]) + _fldVal, 2);
                                break;
                            case "N":
                                _commonDs.Tables["Main"].Rows[0]["tot_nontax"] = Math.Round(Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_nontax"]) + _fldVal, 2);
                                break;
                            case "F":
                                _commonDs.Tables["Main"].Rows[0]["tot_fdisc"] = Math.Round(Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["tot_fdisc"]) + _fldVal, 2);
                                break;
                        }
                    }
                }
            }
        }
        #endregion Calculate Headerwise Taxes
        // ****** Added by Sachin N. S. on 28/05/2013 for Bug-14538 -- End ****** //

        #region Add - Edit - Delete Methods
        private void addHdrRecords()
        {
            DataRow _dr;
            this.txtWareHouse.Text = CommonInfo.WareHouse;
            _dr = clsInsertDefaultValue.AddNewRow(_commonDs.Tables["Main"]);
            _commonDs.Tables["Main"].Rows.Add(_dr);
            clsInsertDefaultValue.InsertDefVal_Main(0);
            // ******* Added by Sachin N. S. on 22/06/2013 for Bug-14538 -- Start *******//
            if (_commonDs.Tables["UDPOSSETTINGS"].Rows.Count > 0)
            {
                if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true)
                {
                    replDefaDiscChrgsValues("PARTY", 0);
                }
            }
            // ******* Added by Sachin N. S. on 22/06/2013 for Bug-14538 -- End *******//
            addChldRecords("Item");
        }

        private void addChldRecords(string cTblName)
        {
            bool _add = false;
            if (dgvItemDetails.Rows.Count > 0)
            {
                if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"].ToString() == "").Count() == 0)
                {
                    _add = true;
                }
            }
            else
            {
                _add = true;
            }

            if (_add == true)
            {
                DataRow _dr;
                _dr = clsInsertDefaultValue.AddNewRow(_commonDs.Tables[cTblName]);
                //Added by Shrikant S. on 09/10/2018        //Start
                if (cTblName == "Item")
                {
                    maxRowId = maxRowId - 1;
                    _dr["ItemRowId"] = maxRowId;
                }
                //Added by Shrikant S. on 09/10/2018        //End
                _commonDs.Tables[cTblName].Rows.Add(_dr);
                this.BindingContext[_commonDs, cTblName].Position = _commonDs.Tables["Item"].Rows.Count;
                clsInsertDefaultValue.InsertDefVal_Item(this.BindingContext[_commonDs, cTblName].Position);
                if (dgvItemDetails.CurrentRow.Index < this.dgvItemDetails.Rows.Count && dgvItemDetails.Rows.Count > 1)
                {
                    ldntValidate = true;
                    //this.BindingContext[_commonDs,cTblName].
                    //dgvItemDetails.CurrentRow.Index += 1; 
                    int i = dgvItemDetails.CurrentRow.Index + 1;
                    dgvItemDetails.CurrentCell = dgvItemDetails.Rows[i].Cells[1];
                    ldntValidate = false;
                }
            }
        }

        private void DeleteChldRecords(string cTblName)
        {
            _commonDs.Tables["Item"].Rows.Cast<DataRow>().Where(r => r["Item"].ToString() == "" && Convert.ToDecimal(r["Qty"]) == 0).ToList().ForEach(r => r.Delete());
        }

        //***** Added by Sachin N. S. on 22/06/2013 for Bug-14538 -- Start *****//
        #region Replace Default Discount & Charges or Sales Tax Value
        private void replDefaDiscChrgsValues(string _type, int _RateLevel)
        {
            DataRow _dr1;
            string cSql = "";
            if (_type == "PARTY")
            {
                if (_commonDs.Tables.Contains("HdDiscChrgs") == true)       // Added by Sachin N. S. on 18/08/2018 for Bug-31756
                {
                    foreach (DataRow _dr in _commonDs.Tables["HdDiscChrgs"].Rows)
                    {
                        if (_commonDs.Tables["PartyDiscChrgs"] != null)
                        {
                            if (_dr["Code"].ToString().Trim() == "S")
                            {
                                if (_commonDs.Tables["PartyDiscChrgs"].Columns.Contains("Tax_Name"))
                                {
                                    cSql = "Set DateFormat DMY Select top 1 Level1 from Stax_mas where Tax_Name = '" + _commonDs.Tables["PartyDiscChrgs"].Rows[0]["Tax_Name"].ToString().Trim() + "' and lDeactive=0 and Deactfrom<='" + this.txtInvoiceDt.Text.ToString() + "'";
                                    _dr1 = _oDataAccess.GetDataRow(cSql, null, 25);
                                    if (_dr1 != null)
                                    {
                                        _dr["Head_Nm"] = _commonDs.Tables["PartyDiscChrgs"].Rows[0]["Tax_Name"].ToString();
                                        _dr["Def_Pert"] = Convert.ToDecimal(_dr1["Level1"]);
                                    }
                                }
                            }
                            else
                            {
                                if (_commonDs.Tables["PartyDiscChrgs"].Columns.Contains(_dr["Pert_Name"].ToString().Trim()))
                                {
                                    _dr["Def_Pert"] = Convert.ToDecimal(_commonDs.Tables["PartyDiscChrgs"].Rows[0][_dr["Pert_Name"].ToString()]);
                                }
                                if (_commonDs.Tables["PartyDiscChrgs"].Columns.Contains(_dr["Fld_Nm"].ToString().Trim()) && Convert.ToDecimal(_dr["Def_Pert"]) == 0)
                                {
                                    _dr["Def_Amt"] = Convert.ToDecimal(_commonDs.Tables["PartyDiscChrgs"].Rows[0][_dr["Fld_Nm"].ToString()]);
                                }
                            }
                        }
                    }
                }
            }

            if (_type == "ITEM")
            {
                DataTable _dt;

                //****** Added by Sachin N. S. on 09/05/2017 for GST -- Start
                _dt = clsInsertDefaultValue.GetItemGstRateByState(Convert.ToInt64(_commonDs.Tables["Main"].Rows[0]["Ac_id"]), Convert.ToInt16(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"]), Convert.ToDateTime(_commonDs.Tables["Main"].Rows[0]["Date"]), this.BindingContext[_commonDs, "Item"].Position);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        int _itRowId = this.BindingContext[_commonDs, "Item"].Position;
                        if (Convert.ToInt64(_dt.Rows[0]["HSNID"]) > 0)
                        {
                            if (Convert.ToBoolean(_dt.Rows[0]["Exempted"]) == true)
                            {
                                _commonDs.Tables["Item"].Rows[_itRowId]["SGst_Per"] = 0;
                                _commonDs.Tables["Item"].Rows[_itRowId]["CGst_Per"] = 0;
                                _commonDs.Tables["Item"].Rows[_itRowId]["IGst_Per"] = 0;
                                _commonDs.Tables["Item"].Rows[_itRowId]["LineRule"] = "Exempted";
                            }
                            else
                            {
                                _commonDs.Tables["Item"].Rows[_itRowId]["SGst_Per"] = Convert.ToDecimal(_dt.Rows[0]["SGstPer"]);
                                _commonDs.Tables["Item"].Rows[_itRowId]["CGst_Per"] = Convert.ToDecimal(_dt.Rows[0]["CGstPer"]);
                                if (Convert.ToDecimal(_dt.Rows[0]["SGstPer"]) + Convert.ToDecimal(_dt.Rows[0]["SGstPer"]) <= 0)
                                {
                                    _commonDs.Tables["Item"].Rows[_itRowId]["LineRule"] = "Nill Rated";
                                }
                                else
                                {
                                    _commonDs.Tables["Item"].Rows[_itRowId]["LineRule"] = "Taxable";
                                }
                            }
                            _commonDs.Tables["Item"].Rows[_itRowId]["GSTRate"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_itRowId]["SGst_Per"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_itRowId]["CGst_Per"]);
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[_itRowId]["LineRule"] = "Non GST";
                        }
                    }
                }
                //****** Added by Sachin N. S. on 09/05/2017 for GST -- End

                _dt = clsInsertDefaultValue.GetItemDefaDiscChrgsValue(Convert.ToInt64(_commonDs.Tables["Main"].Rows[0]["Ac_id"]), Convert.ToInt16(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"]), Convert.ToDecimal(_RateLevel));

                DataRow _dr;
                string _cfldnm = "";
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataColumn _dc2 in _dt.Columns)
                    {
                        _cfldnm = _dc2.ColumnName.Trim();
                        _dr = _commonDs.Tables["DiscChrgsFldLst"].Select("Att_File=False and LothrFldNm='" + _cfldnm + "'").FirstOrDefault();
                        if ((_dr["Code"].ToString() == "S" && Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true) || (_dr["Code"].ToString() != "S" && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true))
                        {
                            if (_dr["Code"].ToString() == "S" && Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                            {
                                if (_commonDs.Tables["Stax_Mas"].AsEnumerable().Where(row => row["Tax_Name"] != null).Count(row => row["Tax_Name"].ToString().Trim() == _dt.Rows[0][_cfldnm].ToString().Trim()) > 0)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][_cfldnm] = _dt.Rows[0][_cfldnm];
                                }
                                else
                                {
                                    if (_dt.Rows[0][_cfldnm].ToString().Trim() != "")
                                    {
                                        MessageBox.Show("The GST Name `" + _dt.Rows[0][_cfldnm].ToString().Trim() + "` selected for the Goods is not activated in GST master for the transaction.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                            else
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][_cfldnm] = _dr["Code"].ToString() == "S" ? _dt.Rows[0][_cfldnm].ToString().Trim() == "" ? "NO-TAX" : _dt.Rows[0][_cfldnm] : _dt.Rows[0][_cfldnm];
                            }
                        }
                    }
                }
            }
        }
        #endregion Replace Default Discount & Charges or Sales Tax Value
        //***** Added by Sachin N. S. on 25/06/2013 for Bug-14538 -- End *****//

        #endregion Add - Edit - Delete Methods

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

        #region Printing Bill
        private void PrintBill(long _tran_cd)
        {
            this.pnlPrintBill.Visible = true;    // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.lblPrintBill.Visible = true;   // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.pgrsBar.Visible = true;       // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.lblPrintBill.Text = "Printing Bill......";      // Added by Sachin N. S. on 03/02/2016 for Bug-27503
            this.Refresh();

            string csql = "";
            DataSet vDsCommon = new DataSet();
            DataTable company = new DataTable();
            company.TableName = "company";
            csql = "Select * From vudyog..Co_Mast where CompId=" + CommonInfo.CompId.ToString();
            company = _oDataAccess.GetDataTable(csql, null, 25);
            vDsCommon.Tables.Add(company);
            vDsCommon.Tables[0].TableName = "company";
            DataTable tblCoAdditional = new DataTable();
            tblCoAdditional.TableName = "coadditional";
            csql = "Select Top 1 * From Manufact";
            tblCoAdditional = _oDataAccess.GetDataTable(csql, null, 25);
            vDsCommon.Tables.Add(tblCoAdditional);
            vDsCommon.Tables[1].TableName = "coadditional";

            //string vRepGroup = "POINT OF SALES";
            string vRepGroup = _commonDs.Tables["Lcode"].Rows[0]["code_nm"].ToString();     // Added by Sachin N. S. on 03/11/2018 for Pharma Retailer Changes
            string[] cpara;
            cpara = new string[1];

            udReportList.cReportList oPrint = new udReportList.cReportList();
            oPrint.pDsCommon = vDsCommon;
            oPrint.pServerName = CommonInfo.Server;
            oPrint.pComDbnm = CommonInfo.DbName;
            oPrint.pUserId = CommonInfo.Uid;
            oPrint.pPassword = CommonInfo.Pwd;
            oPrint.pAppPath = this.pAppPath;
            oPrint.pPApplText = "";
            oPrint.pPara = cpara;
            oPrint.pRepGroup = vRepGroup;
            oPrint.pSpPara = _tran_cd.ToString(); // vPrintPara + ",'" + this.pAppUerName.Trim() + "'"; /*Ramya 01/11/12*/
            oPrint.pPrintOption = 3;
            oPrint.Main();

            this.pnlPrintBill.Visible = false;   // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.lblPrintBill.Visible = false;   // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.pgrsBar.Visible = false;        // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.Refresh();
        }
        #endregion

        //***** Added by Sachin N. S. on 29/08/2013 for Bug-14538 -- Start *****//
        #region Method to Hide/Unhide the Controls
        private void hideUnhideControls()
        {
            if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["V_Extra"]) == false)
            {
                this.btnAddInfo.Visible = false;
            }
            this.pnlPrintBill.Visible = false;   // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.lblPrintBill.Visible = false;   // Added by Sachin N. S. on 21/08/2015 for Bug-26654
            this.pgrsBar.Visible = false;        // Added by Sachin N. S. on 21/08/2015 for Bug-26654
        }
        #endregion Method to Hide/Unhide the Controls
        //***** Added by Sachin N. S. on 29/08/2013 for Bug-14538 -- End *****//

        //***** Added by Sachin N. S. on 13/08/2015 for Bug-26654 -- Start *****//
        private string splitText(string[][] cFldLst, ref string xcSrchCol, ref string xcDispCol)
        {
            string cFldStr = "";
            for (int i = 0; i < cFldLst.GetLength(0); i++)
            {
                cFldStr += (cFldStr == "" ? "" : ",") + cFldLst[i][0].ToString();
                xcSrchCol += (xcSrchCol == "" ? "" : "+") + cFldLst[i][0].ToString().Split('.')[1].ToString();
                xcDispCol += (xcDispCol == "" ? "" : ",") + cFldLst[i][0].ToString().Split('.')[1].ToString() + ":" + cFldLst[i][1].ToString();
            }
            return cFldStr;
        }
        //***** Added by Sachin N. S. on 13/08/2015 for Bug-26654 -- End *****//

        //***** Added by Sachin N. S. on 20/01/2016 for Bug-27503 -- Start *****//
        private void CancelEntry()
        {
            popUpFromTable("CANCELBILL");
            maxRowId = 0;           //Added by Shrikant S. on 09/10/2018 
        }

        private void dgvItemDetails_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != dgvItemDetails.BindingContext[_commonDs, "Item"].Position)
                dgvItemDetails.BindingContext[_commonDs, "Item"].Position = e.RowIndex;
        }
        //***** Added by Sachin N. S. on 20/01/2016 for Bug-27503 -- End *****//

        //***** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- Start *****//
        private void Update_Conv_Qty(string _colNm, int _nPosition)
        {
            string _bunit = string.Empty, _gpunit = string.Empty, cSql = string.Empty;
            _bunit = _commonDs.Tables["Item"].Rows[_nPosition]["stkunit"].ToString();
            _gpunit = _commonDs.Tables["Item"].Rows[_nPosition]["gpunit"].ToString();


            if (_colNm == "QTY")
            {
                cSql = "Execute USP_ENT_GETQTYCONVRATIO " + _commonDs.Tables["Item"].Rows[_nPosition]["It_Code"].ToString() + "," + _commonDs.Tables["Item"].Rows[_nPosition]["Qty"].ToString();

                DataTable _dt;
                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                decimal _mratio = 0.00M, _mrate = 0.00M, _mgrpQty = 0.00M, _mBalQty = 0.00M;
                string _mgrpuom = "", _mbaseuom = "";

                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _mgrpQty = Convert.ToDecimal(_dt.Rows[0]["GrpQty"]);
                        _mBalQty = Convert.ToDecimal(_dt.Rows[0]["GrpBal"]);
                        _mgrpuom = _dt.Rows[0]["GrpUom"].ToString();
                        _mbaseuom = _dt.Rows[0]["BaseUom"].ToString();
                        _mrate = 0;
                    }
                }
                _commonDs.Tables["Item"].Rows[_nPosition]["GpQty"] = _mgrpQty;
                _commonDs.Tables["Item"].Rows[_nPosition]["PHMLOSEQTY"] = _mBalQty;
                _commonDs.Tables["Item"].Rows[_nPosition]["gpunit"] = _mgrpuom;

                if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Qty"]) > 0)
                {
                    cSql = " Select top 1 " + (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Iotrans"]) == true ? "prate" : "sRate") + " as gpuomrate from groupuomrate Where It_code=" + _commonDs.Tables["Item"].Rows[_nPosition]["It_code"].ToString() + " and subuom='" + _commonDs.Tables["Item"].Rows[_nPosition]["gpunit"].ToString().Trim() + "'";

                    _commonDs.Tables["Item"].Rows[_nPosition]["GPRate"] = 0;
                    _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                    if (_dt != null)
                    {
                        if (_dt.Rows.Count > 0)
                            _commonDs.Tables["Item"].Rows[_nPosition]["gprate"] = Convert.ToDecimal(_dt.Rows[0]["gpuomrate"]);
                    }
                }
            }
            else
            {
                cSql = "if Exists( Select top 1 convRatio from GroupUOM Where baseuom='" + _bunit.Trim() + "' and subuom='" + _gpunit.Trim() + "') ";
                cSql += " Select top 1 convRatio,0 as gpuomrate from GroupUOM Where baseuom='" + _bunit.Trim() + "' and subuom='" + _gpunit.Trim() + "' ";
                cSql += " Else ";
                cSql += " Select top 1 [Ratio] as convRatio" + (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Iotrans"]) == true ? ",prate" : ",Rate") +
                        " as gpuomrate from It_mast Where It_code= " + _commonDs.Tables["Item"].Rows[_nPosition]["It_code"].ToString();

                DataTable _dt;
                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                decimal _mratio = 0.00M, _mrate = 0.00M;

                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _mratio = Convert.ToDecimal(_dt.Rows[0]["convRatio"]);
                        _mrate = Convert.ToDecimal(_dt.Rows[0]["gpuomrate"]);
                    }
                }
                bool lDispB4 = Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]);
                if (lDispB4 == true)
                {
                    if (_bunit.Trim().ToUpper() == _gpunit.Trim().ToUpper())
                    {
                        _commonDs.Tables["Item"].Rows[_nPosition]["gpqty"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["qty"]);
                        _commonDs.Tables["Item"].Rows[_nPosition]["PHMLOSEQTY"] = 0M;
                    }
                    else
                    {
                        if (_mratio > 0)
                        {
                            int _remd;
                            int _quot = Math.DivRem(Convert.ToInt16(_commonDs.Tables["Item"].Rows[_nPosition]["qty"]), Convert.ToInt16(_mratio), out _remd);
                            _commonDs.Tables["Item"].Rows[_nPosition]["gpQty"] = _quot;
                            _commonDs.Tables["Item"].Rows[_nPosition]["PHMLOSEQTY"] = _remd;
                        }
                    }
                }
                else
                {
                    _commonDs.Tables["Item"].Rows[_nPosition]["Qty"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gpqty"]) * _mratio;
                    if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Qty"]) > 0)
                    {
                        if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Rate"]) == 0)
                            _commonDs.Tables["Item"].Rows[_nPosition]["Rate"] = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gprate"]) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gpqty"])) / Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Qty"]);
                    }
                }
            }
        }
        //***** Added by Sachin N. S. on 01/10/2018 for Pharma Retailer Changes -- End ******//

        //Added by Rupesh G. on 19/10/2018---Start
        private void Export_CSV_Email()
        {
            bool ExportCSV1, CSVEmail;
            ExportCSV1 = (bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["lCECSV"];
            CSVEmail = (bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["lCCSVEMAIL"];

            if (ExportCSV1 || CSVEmail)
            {
                ExportCSV(Convert.ToInt64(_commonDs.Tables["Main"].Rows[0]["Tran_cd"]), CSVEmail);
            }
        }

        private void ExportCSV(long tran_cd, bool CSVEmail)
        {
            string sqlQuery = "";
            string filename = "";
            string username, password, host, to, body, subject, attachment;
            bool enablessl;
            int port;
            // sqlQuery = "select * from CSV_Export "; //Comment by Rupesh G. on 29/11/2018 for Pharma Retailer Changes
            sqlQuery = "select * from CSV_Export where entry_ty='" + CommonInfo.EntryTy + "'";//Add by Rupesh G. on 29/11/2018 for Pharma Retailer Changes
            DataTable dtCSVExport = _oDataAccess.GetDataTable(sqlQuery, null, 25);
            if (dtCSVExport.Rows.Count > 0)
            {
                string fname, Filter_Cond, SP_Name, CSV_Path, file_name, EMailSub, EmailBody, signature, signinit;
                fname = dtCSVExport.Rows[0]["File_Name"].ToString();
                file_name = fname;
                Filter_Cond = dtCSVExport.Rows[0]["Filter_Cond"].ToString();
                SP_Name = dtCSVExport.Rows[0]["SP_Name"].ToString();
                CSV_Path = dtCSVExport.Rows[0]["CSV_Path"].ToString();

                EMailSub = dtCSVExport.Rows[0]["EMailSub"].ToString();
                EmailBody = dtCSVExport.Rows[0]["EmailBody"].ToString();
                signature = dtCSVExport.Rows[0]["signature"].ToString();
                signinit = dtCSVExport.Rows[0]["signinit"].ToString();

                sqlQuery = "select * from DCMAIN where entry_ty='" + CommonInfo.EntryTy + "' and Tran_cd=" + tran_cd;
                DataTable dtDCMAIN = _oDataAccess.GetDataTable(sqlQuery, null, 25);

                sqlQuery = "select * from DCITEM where entry_ty='" + CommonInfo.EntryTy + "' and Tran_cd=" + tran_cd;
                DataTable dtDCITEM = _oDataAccess.GetDataTable(sqlQuery, null, 25);


                var Value1 = fname.Split('/');
                fname = string.Empty;
                foreach (string str in Value1)
                {
                    if (str.Contains("m."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        string type = dtDCITEM.Rows[0][fieldValue].GetType().Name.ToString();
                        if (type == "DateTime")
                        {
                            fname = fname + DateTime.Parse(dtDCMAIN.Rows[0][fieldValue].ToString()).ToString("dd_MM_yyyy");
                            //   fname = DateTime.Parse(fname).ToString("DD_MM_YYYY");
                        }
                        else
                        {
                            fname = fname + dtDCMAIN.Rows[0][fieldValue];
                        }
                    }
                    else if (str.Contains("i."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        fname = fname + dtDCITEM.Rows[0][fieldValue];
                        string type = dtDCITEM.Rows[0][fieldValue].GetType().Name.ToString();
                    }
                    else
                    {
                        fname = fname + str;
                    }
                }

                var Value = Filter_Cond.Split('/');
                Filter_Cond = string.Empty;
                foreach (string str in Value)
                {
                    if (str.Contains("m."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        Filter_Cond = Filter_Cond + dtDCMAIN.Rows[0][fieldValue];
                    }
                    else if (str.Contains("i."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        Filter_Cond = Filter_Cond + dtDCITEM.Rows[0][fieldValue];
                    }
                    else
                    {
                        Filter_Cond = Filter_Cond + str;
                    }
                }

                var Value2 = EMailSub.Split('/');
                EMailSub = string.Empty;
                foreach (string str in Value2)
                {
                    if (str.Contains("m."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        EMailSub = EMailSub + dtDCMAIN.Rows[0][fieldValue];
                    }
                    else if (str.Contains("i."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        EMailSub = EMailSub + dtDCITEM.Rows[0][fieldValue];
                    }
                    else
                    {
                        EMailSub = EMailSub + str;
                    }
                }

                var Value3 = EmailBody.Split('/');
                EmailBody = string.Empty;
                foreach (string str in Value3)
                {
                    if (str.Contains("m."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        EmailBody = EmailBody + dtDCMAIN.Rows[0][fieldValue];
                    }
                    else if (str.Contains("i."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        EmailBody = EmailBody + dtDCITEM.Rows[0][fieldValue];
                    }
                    else if (str.Contains("c."))
                    {
                        string fieldValue = str.Remove(0, 2);
                        EmailBody = EmailBody + dtCSVExport.Rows[0][fieldValue];
                    }
                    else
                    {
                        EmailBody = EmailBody + str;
                    }
                }
                EmailBody = EmailBody.Replace("\\", "/");

                sqlQuery = "execute " + SP_Name + " '" + Filter_Cond + "'";
                DataTable dtExportRec = _oDataAccess.GetDataTable(sqlQuery, null, 25);


                if (MessageBox.Show("Do you want to genrate CSV", CommonInfo.AppCaption,
             MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.pnlPrintBill.Visible = true;
                    this.lblPrintBill.Visible = true;
                    this.pgrsBar.Visible = true;
                    this.lblPrintBill.Text = "Genrate CSV......";
                    this.Refresh();
                    string file_path = CSV_Path.ToString();

                    if (file_path == "" || !Directory.Exists(file_path))
                    {
                        file_path = getFileName("\\" + fname);
                        _oDataAccess.BeginTransaction();

                        //Comment by Rupesh G. on 29/11/2018 for Pharma Retailer Changes
                        // sqlQuery = "update CSV_Export set CSV_Path='" + DBfilePath + "' where [File_Name]='" + file_name + "'";

                        //Add by Rupesh G. on 29/11/2018 for Pharma Retailer Changes--Start
                        sqlQuery = "update CSV_Export set CSV_Path='" + DBfilePath + "' where [File_Name]='" + file_name + "' and entry_ty='" + CommonInfo.EntryTy + "'";
                        //Add by Rupesh G. on 29/11/2018 for Pharma Retailer Changes--End

                        _oDataAccess.ExecuteSQLStatement(sqlQuery, null, 20, true);
                        _oDataAccess.CommitTransaction();
                    }
                    else
                    {
                        fname = fname.Replace(".", "").Replace("-", "").Replace("/", "").Replace(":", "").Replace("__", "_").Replace(" ", "");
                        file_path = file_path + "\\" + fname;
                        file_path = file_path.Replace("\\\\", "\\");
                    }

                    filename = file_path + ".csv";
                    ToCSV(dtExportRec, filename);
                    if (CSVEmail && File.Exists(filename))
                    {

                        sqlQuery = "select username,password,host,port,enablessl from eMailSettings";
                        DataTable dtEmailSetting = _oDataAccess.GetDataTable(sqlQuery, null, 25);
                        if (dtEmailSetting.Rows.Count > 0)
                        {
                            username = dtEmailSetting.Rows[0]["username"].ToString();
                            password = DecryptData(dtEmailSetting.Rows[0]["password"].ToString());
                            host = dtEmailSetting.Rows[0]["host"].ToString();
                            port = Int32.Parse(dtEmailSetting.Rows[0]["port"].ToString());
                            enablessl = bool.Parse(dtEmailSetting.Rows[0]["enablessl"].ToString());

                            sqlQuery = "select email from AC_MAST where Ac_id = (select Ac_id from DCMAIN where Tran_cd = " + tran_cd + ") and email<>''";
                            DataTable dtPartyEmail = _oDataAccess.GetDataTable(sqlQuery, null, 25);
                            if (dtPartyEmail.Rows.Count > 0)
                            {
                                to = dtPartyEmail.Rows[0]["email"].ToString();
                                subject = EMailSub;
                                body = EmailBody;
                                attachment = filename;
                                string result = sendemail(to, subject, body, attachment, username, password, host, port, enablessl);

                            }
                            else
                            {
                                MessageBox.Show("Selected party email id not found....", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                            }
                        }
                    }
                }
            }
        }

        public string DecryptData(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }

        public string sendemail(string to_mail, string subject, string ebody, string attachment, string username, string password, string host, int port, bool enablessl)
        {
            this.pnlPrintBill.Visible = true;
            this.lblPrintBill.Visible = true;
            this.pgrsBar.Visible = true;
            this.lblPrintBill.Text = "Sending Mail......";
            this.Refresh();
            try
            {
                using (MailMessage mm = new MailMessage(username, to_mail))
                {

                    mm.Subject = subject;
                    mm.Body = ebody;
                    mm.IsBodyHtml = true;
                    mm.BodyEncoding = System.Text.Encoding.UTF8;

                    mm.Attachments.Add(new Attachment(attachment));

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = host;
                    smtp.EnableSsl = enablessl;
                    NetworkCredential NetworkCred = new NetworkCredential(username, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = port;
                    smtp.Send(mm);
                }
                this.pnlPrintBill.Visible = false;
                this.lblPrintBill.Visible = false;
                this.pgrsBar.Visible = false;
                this.Refresh();
                return "success";

            }
            catch (Exception ee)
            {
                this.pnlPrintBill.Visible = false;
                this.lblPrintBill.Visible = false;
                this.pgrsBar.Visible = false;
                this.Refresh();
                return "fail";
            }


        }

        public string getFileName(string fname)
        {
            string DfilePath, filename, FilePath = "";

            FolderBrowserDialog vFolderBrowserDialog1 = new FolderBrowserDialog();
            if (vFolderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DfilePath = vFolderBrowserDialog1.SelectedPath;

                filename = fname;
                filename = filename.Replace(".", "").Replace("-", "").Replace("/", "").Replace(":", "").Replace("__", "_").Replace(" ", "");
                //  FilePath = DfilePath + filename+".xls";
                FilePath = DfilePath + filename;
                FilePath = FilePath.Replace("\\\\", "\\");
                DBfilePath = DfilePath;
                DBfilePath = DBfilePath.Replace("\\\\", "\\");

            }

            return FilePath;
        }

        public void ToCSV(DataTable dtDataTable, string strFilePath)
        {

            try
            {
                StreamWriter sw = new StreamWriter(strFilePath, false);
                //headers  
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sw.Write(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write("||");
                    }
                }
                sw.Write(sw.NewLine);
                foreach (DataRow dr in dtDataTable.Rows)
                {
                    for (int i = 0; i < dtDataTable.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains('|'))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }

                        if (i < dtDataTable.Columns.Count - 1)
                        {
                            sw.Write("||");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
                MessageBox.Show("Your CSV file exported successfully at " + strFilePath, CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                this.pnlPrintBill.Visible = false;
                this.lblPrintBill.Visible = false;
                this.pgrsBar.Visible = false;
                this.Refresh();
            }
            catch (Exception e)
            {
                this.pnlPrintBill.Visible = false;
                this.lblPrintBill.Visible = false;
                this.pgrsBar.Visible = false;
                this.Refresh();
            }

        }

        //Added by Rupesh G. on 19/10/2018---End

        //***** Added by Sachin N. S. on 02/11/2018 for Pharma Retailer Changes -- Start
        private void addBatchNoFldInGrid(DataRow _dr, int _ncolId)
        {
            if (_dr != null)
            {
                if (Convert.ToBoolean(_dr["Att_file"]) == false && Convert.ToBoolean(_dr["Ingrid"]) == true)
                {
                    var txtColV = new PointofSale.udDGVStringColumn();
                    txtColV.HeaderText = Convert.ToString(_dr["Head_nm"]).Trim();
                    txtColV.Name = "col" + Convert.ToString(_dr["Fld_nm"]).Trim();
                    txtColV.DataPropertyName = Convert.ToString(_dr["Fld_nm"]).Trim();
                    txtColV.Width = 150;
                    txtColV.Fldnm = Convert.ToString(_dr["Fld_nm"]).Trim();
                    txtColV.Whn_con = Convert.ToString(_dr["Whn_con"]).Trim();
                    txtColV.Val_con = Convert.ToString(_dr["Val_con"]).Trim();
                    txtColV.Val_err = Convert.ToString(_dr["Val_err"]).Trim();
                    txtColV.Defa_val = Convert.ToString(_dr["defa_val"]).Trim();
                    txtColV.Mandatory = Convert.ToString(_dr["mandatory"]).Trim();
                    txtColV.Inter_use = Convert.ToString(_dr["inter_use"]).Trim();
                    txtColV.Filtcond = Convert.ToString(_dr["Filtcond"]).Trim();
                    txtColV.Inptmask = Convert.ToString(_dr["Inptmask"]).Trim();
                    txtColV.ReadOnly = false;
                    txtColV.DisplayIndex = _ncolId;
                    txtColV.SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvItemDetails.Columns.Add(txtColV);
                    this.dgvItemDetails.Columns[_ncolId].Width = 150;
                }
            }
            this.dgvItemDetails.Refresh();
        }

        private string getExecSqlString(string _cSql)
        {
            _cSql = _cSql.ToUpper().Trim();
            string _csql1 = _cSql.Replace("EXECUTE", "").Replace("EXEC", "").Replace("  ", " ");
            _csql1 = _csql1.Trim().Substring(_csql1.Trim().IndexOf(" "), _csql1.Trim().Length - _csql1.Trim().IndexOf(" ")).Trim();
            string[][] _str1 = _csql1.Split(',').Select(p => p.Split('.')).ToArray();

            string[] _str2;
            string _str3, _str4;
            string _tbl = "", _fld = "";
            for (int i = 0; i < _str1.GetLength(0); i++)
            {
                _str2 = _str1[i];
                _str3 = _str2[0].ToString().Trim() + "." + _str2[1].ToString().Trim();
                _tbl = _str2[0].ToString().Replace("?", "").Trim().Replace("ITEM_VW", "ITEM").Replace("MAIN_VW", "MAIN");
                _fld = _str2[1].ToString().Trim();
                string _cType = _commonDs.Tables[_tbl].Rows[this.BindingContext[_commonDs, _tbl].Position][_fld].GetType().Name.ToString();
                switch (_cType)
                {
                    case "DateTime":
                        _str4 = "'" + _commonDs.Tables[_tbl].Rows[this.BindingContext[_commonDs, _tbl].Position][_fld].ToString().Trim() + "'";
                        break;
                    case "String":
                        _str4 = "'" + _commonDs.Tables[_tbl].Rows[this.BindingContext[_commonDs, _tbl].Position][_fld].ToString().Trim() + "'";
                        break;
                    case "Numeric":
                        _str4 = _commonDs.Tables[_tbl].Rows[this.BindingContext[_commonDs, _tbl].Position][_fld].ToString().Trim();
                        break;
                    case "Decimal":
                        _str4 = _commonDs.Tables[_tbl].Rows[this.BindingContext[_commonDs, _tbl].Position][_fld].ToString().Trim();
                        break;
                    case "Integer":
                        _str4 = _commonDs.Tables[_tbl].Rows[this.BindingContext[_commonDs, _tbl].Position][_fld].ToString().Trim();
                        break;
                    default:
                        _str4 = "'" + _commonDs.Tables[_tbl].Rows[this.BindingContext[_commonDs, _tbl].Position][_fld].ToString().Trim() + "'";
                        break;
                }
                _cSql = _cSql.Replace(_str3, _str4);
            }

            return _cSql;
        }

        private void callLastDeal()
        {
            frmLastDeal lstDeal = new frmLastDeal();
            lstDeal.oDataAccess = _oDataAccess;
            lstDeal.EntryType = _commonDs.Tables["Main"].Rows[0]["Entry_ty"].ToString();
            lstDeal.Itemcode = Convert.ToInt32(_commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentCell.RowIndex]["It_code"]);
            lstDeal.AcId = Convert.ToInt32(_commonDs.Tables["Main"].Rows[0]["Ac_id"]);
            lstDeal.Width = this.Width - 200;
            lstDeal.Top = (this.Height - lstDeal.Height) / 2;
            lstDeal.Left = (this.Width - lstDeal.Width) / 2;
            lstDeal.Icon = this.Icon;
            lstDeal.ShowDialog();
        }

        private void getBatchNo()
        {
            string cCol = "colBatchNo";
            udDGVStringColumn column = dgvItemDetails.Columns[cCol] as udDGVStringColumn;
            if (column != null)
            {
                if (column.Filtcond.Trim().Length > 0)
                {
                    DataTable searchTbl = new DataTable();

                    udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
                    string cFrmCap = "Select ", cSrchCol = "", cDispCol = "", cRetCol = "";
                    if (column.Filtcond.ToLower().Contains("execute ") || column.Filtcond.ToLower().Contains("select ") || column.Filtcond.ToLower().Contains("exec "))
                    {
                        cFrmCap = cFrmCap + dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].HeaderText.Trim();
                        string csql = (column.Filtcond.Trim().IndexOf("{") > 0 ? column.Filtcond.Trim().Substring(0, column.Filtcond.Trim().IndexOf("{") - 1) : column.Filtcond.Trim());

                        if (column.Filtcond.ToLower().Contains("execute "))
                            csql = this.getExecSqlString(csql);

                        searchTbl = _oDataAccess.GetDataTable(csql, null, 25);

                        cSrchCol = searchTbl.Columns[0].ColumnName;
                        cRetCol = searchTbl.Columns[0].ColumnName;
                        cDispCol = searchTbl.Columns[0].ColumnName;

                        if (column.Filtcond.Trim().IndexOf("{") > 0)
                        {
                            string filtcond = column.Filtcond.Trim().Substring(column.Filtcond.Trim().IndexOf("{") + 1, (column.Filtcond.Trim().Trim().Length - column.Filtcond.Trim().IndexOf("{") - 2));
                            string[] vals = filtcond.Trim().Split('#');
                            switch (vals.Length)
                            {
                                case 1:
                                    cSrchCol = vals[0];
                                    break;
                                case 2:
                                    cSrchCol = vals[0];
                                    cRetCol = vals[1];
                                    break;
                                case 4:
                                    cSrchCol = vals[0];
                                    cRetCol = vals[1];
                                    cDispCol = vals[3];
                                    break;
                                default:
                                    break;
                            }
                        }


                        string[] returnArray = null;
                        if (searchTbl.Rows.Count > 0)
                        {
                            if (searchTbl.Rows.Count == 1)
                            {
                                string fldnm = column.Fldnm.Trim();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm] = searchTbl.Rows[0][0].ToString();
                                returnArray = new string[] { searchTbl.Rows[0][0].ToString() };
                            }
                            else
                            {
                                _oSelPop.pformtext = cFrmCap;
                                _oSelPop.psearchcol = cSrchCol;
                                _oSelPop.pDisplayColumnList = cDispCol;
                                _oSelPop.pRetcolList = cRetCol;

                                DataView _dvw = searchTbl.DefaultView;
                                _oSelPop.pdataview = _dvw;
                                _oSelPop.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
                                _oSelPop.ShowDialog();
                                if (_oSelPop.pReturnArray != null)
                                    returnArray = new string[] { _oSelPop.pReturnArray[0].ToString() };
                            }
                        }

                        if (returnArray != null)
                        {
                            string fldnm = column.Fldnm.Trim();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm] = returnArray[0].ToString();

                            csql = "SELECT BatchNo, MfgDt, ExpDt, BatchRate, Inc_gst  FROM pRetBatchDetails WHERE it_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString() + " and BatchonHold = 0 and BatchNo='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm].ToString().Trim() + "'";
                            searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                            if (searchTbl.Rows.Count > 0)
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                                itemwiseTotal(this.BindingContext[_commonDs, "Item"].Position, 0);
                            }
                        }
                    }
                }
            }
        }
        //***** Added by Sachin N. S. on 02/11/2018 for Pharma Retailer Changes -- End
        #endregion Private Methods

    }
}
