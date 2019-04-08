using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataAccess_Net;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace PointofSale
{
    public partial class udFrmPointofSaleNew : Form
    {
        clsDataAccess _oDataAccess;
        DataSet _commonDs;
        DataTable _dtSumDiscChrgs;
        ToolTip _toolTip;
        string pAppPath;
        string _keypress = "", _item = "", _keyEnter = "";
        bool _barcodeRead = false;
        bool ldntValidate = false;
        string cAppName = "", cAppPId = "";
        string _taxExpr = "";
        string _colValue = "";
        bool _dgvDiscChrgsdataBind = false;
        decimal OrgHgth = 0M, OrgWdth = 0M;
        bool _setFocus = false;
        int _curRowNo = -1;
        bool llgpuom = false;
        int maxRowId = 0;
        string DBfilePath;
        int _tabOrder = 0;

        public string PayMode;

        #region Default Constructor and Screen Load
        public udFrmPointofSaleNew()
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

            this.Text = CommonInfo.ScreenCaption;
            this.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));

            this.txtUserName.Text = "User : " + CommonInfo.UserName;
            this.txtCounter.Text = "Counter : " + CommonInfo.CounterNm;
            this.txtPoweredBy.Text = "Powered by Udyog Software (I) Ltd.";

            string appPath;
            appPath = Application.ExecutablePath;

            this.mInsertProcessIdRecord();

            appPath = Path.GetDirectoryName(appPath);
            if (string.IsNullOrEmpty(this.pAppPath))
            {
                this.pAppPath = appPath;
            }

            this.Add_Last_Deal_button();

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

            hideUnhideControls();

            clsScreenPropEvents.NewHgth = this.Height;
            clsScreenPropEvents.NewWdth = this.Width;
            ResizeFormNew(this);

            _dgvDiscChrgsdataBind = true;
            this.Refresh();

            if (File.Exists(this.pAppPath + @"\" + CommonInfo.DbName.Trim() + "\\" + CommonInfo.CompImg.ToString()) == true)
            {
                System.Drawing.Bitmap img = new Bitmap(this.pAppPath + @"\" + CommonInfo.DbName.Trim() + "\\" + CommonInfo.CompImg.ToString());
                this.picCompLogo.Image = img;
            }
            dgvItemDetails.Focus();
            //SendKeys.Send("{Tab}");
            if (dgvItemDetails.Rows.Count > 0)
                dgvItemDetails.CurrentCell = dgvItemDetails.Rows[0].Cells[1];
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
                case Keys.F4:
                    this.CancelUnSavedRcrds();
                    break;
                case Keys.F5:
                    popUpFromTable("PRINTBILL");
                    break;
                case Keys.F6:
                    this.CancelEntry();
                    break;
                case Keys.F7:
                    this.btnPayment.PerformClick();
                    break;
                case Keys.F8:
                    this.btnPayOption.PerformClick();
                    break;
                case Keys.F10:
                    this.callLastDeal();
                    break;
                case Keys.F11:
                    System.Diagnostics.Process.Start("Calc");
                    break;
                case Keys.F12:
                    this.ExitForm();
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
                MessageBox.Show((CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse") + " cannot be changed as Goods have been selected.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvItemDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgvItemDetails.CurrentCell == null)
                    return;

                string _orgColKeyDown = dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString();
                if (dgvItemDetails.RowCount > 0)
                {
                    if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colItem")
                    {
                        if (e.KeyCode == Keys.F2 && dgvItemDetails.CurrentRow.Cells[dgvItemDetails.CurrentCell.ColumnIndex].Value.ToString() == "")
                        {
                            string _caddItem = "";
                            popUpFromTable("ITEM");
                            if (this.dgvItemDetails.CurrentRow.Cells[1].Value.ToString() != "")
                            {
                                if (Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]) == true)
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 1;
                                _keypress = "F2";

                                if (CommonInfo.EntryTy != "HS")
                                    this.addChldRecords("Item");
                                this.dgvItemDetails.Refresh();
                                _caddItem = "Add";
                            }
                            _item = "";

                            int _iRwPos = _curRowNo;
                            if (_iRwPos >= 0 && Convert.ToInt64(_commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"]) != 0)
                            {
                                if (CommonInfo.EntryTy == "HS" && dgvItemDetails.Columns.Contains("COLBATCHNO") == true && _caddItem == "Add")
                                {
                                    //this.getBatchNo();
                                    this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                    dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells["COLBATCHNO"];
                                }
                                else
                                {
                                    string cSql = "Select AskQty,AskRate from It_Mast where It_Code=" + _commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"].ToString();

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
                                }
                            }
                        }

                        if (e.KeyCode == Keys.Enter || (e.Control == true && e.KeyCode == Keys.V))
                        {
                            _keypress = "ENTER";
                        }
                    }

                    if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colTax_Name")
                    {
                        if (e.KeyCode == Keys.F2)
                        {
                            popUpFromTable("SALESTAX");
                        }
                    }

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

                            dgvItemDetails.CurrentCell = dgvItemDetails[iColumn, iRow];

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
                            lstDeal.AcId = Convert.ToInt32(_commonDs.Tables["Main"].Rows[0]["Ac_id"]);
                            lstDeal.PatientNm = _commonDs.Tables["Main"].Rows[0]["PatientNm"].ToString();
                            lstDeal.Width = this.Width - 200;
                            lstDeal.Top = (this.Height - lstDeal.Height) / 2;
                            lstDeal.Left = (this.Width - lstDeal.Width) / 2;
                            lstDeal.Icon = this.Icon;
                            lstDeal.ShowDialog();
                            e.SuppressKeyPress = true;
                            return;
                        }
                    }

                    if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].ValueType != null && (e.KeyCode == Keys.F2))
                    {
                        if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].ValueType.ToString().ToLower() == "system.string")
                        {
                            if (_orgColKeyDown == dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString())
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

                                            if (column.Filtcond.ToLower().Contains("execute "))
                                                csql = this.getExecSqlString(csql);
                                            csql = "Set Dateformat DMY; " + csql;
                                            searchTbl = _oDataAccess.GetDataTable(csql, null, 25);

                                            // Added by Sachin N. S. on 07/02/2019 -- Start
                                            if (CommonInfo.EntryTy == "HS" && (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString().ToUpper() == "COLBATCHNO"))
                                            {
                                                long _itcode = 0;
                                                string _bathcNo = "";
                                                decimal _qty = 0;
                                                DataTable _dtBatch = searchTbl;
                                                int _nRowNo = this.BindingContext[_commonDs, "Item"].Position;
                                                foreach (DataRow _dr in _dtBatch.Rows)
                                                {
                                                    _itcode = Convert.ToInt64(_commonDs.Tables["Item"].Rows[_nRowNo]["It_code"]);
                                                    _bathcNo = _dr["BatchNo"].ToString();

                                                    _qty = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null && Convert.ToInt16(row["It_code"]) == _itcode && row["BatchNo"].ToString() == _bathcNo).Sum(row => Convert.ToDecimal(row["Qty"]));
                                                    _dr["Qty"] = Convert.ToDecimal(_dr["Qty"]) - _qty;
                                                    if (Convert.ToDecimal(_dr["Qty"]) > 0)
                                                        searchTbl.Rows[searchTbl.Rows.IndexOf(_dr)]["Qty"] = _dr["Qty"];
                                                    else
                                                        searchTbl.Rows[searchTbl.Rows.IndexOf(_dr)].Delete();
                                                }
                                                searchTbl.AcceptChanges();
                                            }
                                            // Added by Sachin N. S. on 07/02/2019 -- End

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

                                            if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString().ToUpper() == "COLBATCHNO")
                                            {
                                                string csql1 = "SELECT PrAppRate FROM Ac_mast WHERE Ac_Id = " + _commonDs.Tables["Main"].Rows[0]["Ac_id"].ToString();
                                                DataRow _dr = _oDataAccess.GetDataRow(csql1, null, 25);
                                                string cRateFld = "BatchRate";
                                                if (_dr != null)
                                                {
                                                    switch (_dr[0].ToString().Trim())
                                                    {
                                                        case "MRP":
                                                            cRateFld = "BatchRate";
                                                            break;
                                                        case "Purchase Rate":
                                                            cRateFld = "prate";
                                                            break;
                                                        case "Sales Rate":
                                                            cRateFld = "srate";
                                                            break;
                                                        case "PTR":
                                                            cRateFld = "ptr";
                                                            break;
                                                        case "PTS":
                                                            cRateFld = "pts";
                                                            break;
                                                        default:
                                                            cRateFld = "BatchRate";
                                                            break;
                                                    }
                                                }
                                                string csql = "SELECT BatchNo, MfgDt, ExpDt, " + cRateFld + " as BatchRate, " + (_dr[0].ToString().Trim() == "MRP" ? "MRPInc_gst" : "Inc_gst") + " as Inc_gst, Qty  FROM pRetBatchDetails WHERE it_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString() + " and BatchonHold = 0 and BatchNo='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm].ToString().Trim() + "'";      // Changed by Sachin N. S. on 11/02/2019
                                                searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                                                if (searchTbl.Rows.Count > 0)
                                                {
                                                    if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]) > Convert.ToDecimal(searchTbl.Rows[0]["Qty"]))
                                                    {
                                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = _colValue;
                                                        MessageBox.Show("Balance Stock for the Item is : "+ searchTbl.Rows[0]["Qty"].ToString()+" for Batch No : "+ searchTbl.Rows[0]["BatchNo"].ToString().Trim() + ". Cannot Continue...!!!", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    }
                                                    else
                                                    {
                                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();

                                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                                                    }
                                                    itemwiseTotal(this.BindingContext[_commonDs, "Item"].Position, 0);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colgpunit")
                    {
                        if (e.KeyCode == Keys.F2 && dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].ReadOnly == false)
                        {
                            popUpFromTable("GROUPUNIT");
                        }
                    }
                }

                if (e.KeyCode == Keys.Enter)
                {
                    _keyEnter = "ENTERED";
                    List<string> _ctrlList = new List<string> { "colLastDeal", "colItemAddInfo", "colDelete", "colAmount" };
                    if (_ctrlList.Contains(dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString()) == false)
                    {
                        e.Handled = true;

                        //int _dispIdx = ((DataGridViewColumn)sender).DisplayIndex;
                        //var _col = from DataGridViewColumn c in dgvItemDetails.Columns
                        //           where c.DisplayIndex == _dispIdx + 1
                        //           select c;


                        //bool llTrue = true;
                        //  int _colIndx = dgvItemDetails.CurrentCell.ColumnIndex + 1;
                        //  while (llTrue == true)
                        //  {
                        //      if (dgvItemDetails.Rows[dgvItemDetails.CurrentCell.RowIndex].Cells[_colIndx] != null)
                        //      {
                        //          if (dgvItemDetails.Rows[dgvItemDetails.CurrentCell.RowIndex].Cells[_colIndx].Visible == true)
                        //          {
                        //              dgvItemDetails.CurrentCell = dgvItemDetails.Rows[this.BindingContext[_commonDs, "Item"].Position].Cells[_colIndx];
                        //              llTrue = false;
                        //          }
                        //      }
                        //      _colIndx += 1;
                        //      if (dgvItemDetails.ColumnCount < _colIndx || llTrue == false)
                        //          break;
                        //  }
                    }

                    if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colAmount")
                    {
                        if (CommonInfo.EntryTy == "HS")
                            this.addChldRecords("Item");
                    }
                    this.dgvItemDetails.Refresh();
                }

                if (e.KeyCode == Keys.Tab)
                {
                    if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colLastDeal")
                    {
                        if (CommonInfo.EntryTy == "HS")
                        {
                            this.addChldRecords("Item");
                            dgvItemDetails.CurrentCell = dgvItemDetails.Rows[this.BindingContext[_commonDs, "Item"].Position].Cells[0];
                        }
                    }
                    this.dgvItemDetails.Refresh();
                }

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
                RefreshControls();
                //Added by Shrikant S. on 02/01/2019    for Bug-32133    //Start
                if (e.KeyCode == Keys.Enter)
                {
                    DataGridViewColumnCollection columnCollection = dgvItemDetails.Columns;

                    int iColumn = dgvItemDetails.CurrentCell.ColumnIndex;
                    int iRow = dgvItemDetails.CurrentCell.RowIndex;
                    int dColIndex = dgvItemDetails.Columns[iColumn].DisplayIndex;

                    if (iColumn != 1)
                    {
                        DataGridViewColumn nextcol = columnCollection.GetNextColumn(dgvItemDetails.CurrentCell.OwningColumn, DataGridViewElementStates.Displayed, DataGridViewElementStates.None);
                        if (nextcol.Name == "colDelete")
                            dgvItemDetails.CurrentCell = dgvItemDetails[iColumn, iRow];
                        else
                            dgvItemDetails.CurrentCell = dgvItemDetails[nextcol.Index, iRow];
                    }
                }
                //if (e.KeyCode == Keys.Tab)
                //{
                //    DataGridViewColumnCollection columnCollection = dgvItemDetails.Columns;
                //    DataGridViewColumn lastcolumn = columnCollection.GetLastColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.None);
                //    MessageBox.Show("lastcolumn=" + lastcolumn.Name+"=colname="+ dgvItemDetails.CurrentCell.OwningColumn.Name);

                //    if (lastcolumn.Name == dgvItemDetails.CurrentCell.OwningColumn.Name)
                //    {
                //        if (dgvItemDetails.Columns[dgvItemDetails.CurrentCell.ColumnIndex].Name.ToString() == "colLastDeal")
                //        {
                //            if (CommonInfo.EntryTy == "HS")
                //            {
                //                this.addChldRecords("Item");
                //                dgvItemDetails.CurrentCell = dgvItemDetails.Rows[this.BindingContext[_commonDs, "Item"].Position].Cells[0];
                //            }
                //        }
                //        this.dgvItemDetails.Refresh();
                //    }

                //}
                //Added by Shrikant S. on 02/01/2019    for Bug-32133     //End

                _keypress = "";
            }
            catch (Exception ex)
            {

            }
        }

        private void dgvItemDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colQuantity" || (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2") || dgvItemDetails.Columns[e.ColumnIndex].Name == "colRate")
            {
                _commonDs.Tables["Item"].Rows[e.RowIndex].EndEdit();
                this.RefreshControls();

                if (dgvItemDetails.Columns[e.ColumnIndex].Name.ToString() == "colQuantity" || (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2") || dgvItemDetails.Columns[e.ColumnIndex].Name == "colRate")
                {
                    string cSql = "";
                    int _iRwPos = _curRowNo;
                    if (_iRwPos >= 0)
                    {
                        cSql = "Select AskQty,AskRate from It_Mast where It_Code=" + _commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"].ToString();
                    }

                    if (_iRwPos >= 0 && Convert.ToInt64(_commonDs.Tables["Item"].Rows[_iRwPos]["It_Code"]) != 0)
                    {
                        if (CommonInfo.EntryTy == "HS" && dgvItemDetails.Columns.Contains("COLBATCHNO") == true && e.RowIndex == _curRowNo && (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2"))
                        {
                            if (_barcodeRead == false || _commonDs.Tables["Item"].Rows[_iRwPos]["BatchNo"].ToString() == string.Empty)
                            {
                                this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells["COLBATCHNO"];
                            }
                            else
                                this.addChldRecords("Item");
                        }
                        else
                        {
                            DataTable _dt;
                            _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                            if (Convert.ToBoolean(_dt.Rows[0]["AskQty"]) == true && (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2"))
                            {
                                this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells[2];
                            }
                            else
                            {
                                if (Convert.ToBoolean(_dt.Rows[0]["AskRate"]) == true && (dgvItemDetails.Columns[e.ColumnIndex].Name == "colQuantity" || (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2")))
                                {
                                    this.BindingContext[_commonDs, "Item"].Position = _iRwPos;
                                    dgvItemDetails.CurrentCell = dgvItemDetails.Rows[_iRwPos].Cells[3];
                                }
                                else
                                {
                                    if (CommonInfo.EntryTy != "HS")
                                        this.addChldRecords("Item");
                                }
                            }
                        }
                    }

                    this.dgvItemDetails.Refresh();
                    _item = "";
                    _barcodeRead = false;
                    _setFocus = true;
                }
            }
            else
            {
                _commonDs.Tables["Item"].Rows[e.RowIndex].EndEdit();
                this.dgvItemDetails.Refresh();
                this.RefreshControls();
            }
        }

        private void dgvItemDetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (ldntValidate == true) { return; }
            if (dgvItemDetails.Rows[e.RowIndex].IsNewRow == true) { return; }
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colQuantity")
            {
                if (Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]) == 0)
                { return; }
                if (Convert.ToDecimal(e.FormattedValue) == 0)
                {
                    return;
                }

                decimal[] stkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(e.FormattedValue), e.RowIndex);     // Added by Sachin N. S. on 07/02/2019

                //if (Stock_Checking(Convert.ToInt16(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(e.FormattedValue), e.RowIndex) < 0)      // Commented by Sachin N. S. on 07/02/2019
                if (stkQty[0] < 0)        // Changed by Sachin N. S. on 07/02/2019
                {
                    //_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                    string stkUnit = "";
                    if (llgpuom == true)
                        stkUnit = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["StkUnit"].ToString();
                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = _colValue;      // Changed by Sachin N. S. on 07/02/2019 
                    dgvItemDetails.CancelEdit();
                    MessageBox.Show("Balance Stock : " + stkQty[1].ToString() + (stkUnit!="" ? (" " + stkUnit):"") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("Goods out of Stock.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                }
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem" && _keypress != "F2")
            {
                _keypress = "";
                if (_barcodeRead == true)
                    return;

                if (e.FormattedValue.ToString() == "" && _item != "")
                {
                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _item;
                    dgvItemDetails.CancelEdit();
                    MessageBox.Show("Cannot change the Goods/Stock Code. You can delete the Line.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SendKeys.Send("{Esc}");
                    e.Cancel = true;
                }


                if (e.FormattedValue.ToString() != "" && _item != e.FormattedValue.ToString())
                {
                    string clAdd = "ADD";
                    clAdd = ReadBarcodeGen(e.FormattedValue.ToString());
                    if (clAdd == "ADD" && e.RowIndex == this.BindingContext[_commonDs, "Item"].Position)
                    {
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 1;
                        if (llgpuom == true)
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpQty"] = 1;
                            this.Update_Conv_Qty("GPQTY", "GRP", _curRowNo);
                            //**** Added by Sachin N. S. on 07/02/2019 for -- Start
                            decimal[] stkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]), this.BindingContext[_commonDs, "Item"].Position);
                            if (stkQty[0] < 0)
                            {
                                string stkUnit = "";
                                if (llgpuom == true)
                                    stkUnit = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpQty"].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpQty"] = 0;
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                                MessageBox.Show("Balance Stock : " + stkQty[1].ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            //**** Added by Sachin N. S. on 07/02/2019 for -- End
                        }
                        _barcodeRead = true;
                    }
                    else
                    {
                        if (clAdd == "EXIST")
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"] = 0;
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = "";
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = "";
                            dgvItemDetails.CancelEdit();
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"] = 0;
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = "";
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = "";
                            dgvItemDetails.CancelEdit();
                            SendKeys.Send("{Esc}");
                            e.Cancel = true;
                        }
                    }
                }
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colFreeQty")
            {
                if (Convert.ToDecimal(e.FormattedValue) > Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]))
                {
                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["FreeQty"] = _colValue;
                    dgvItemDetails.CancelEdit();
                    MessageBox.Show("Free quantity cannot be more than the Stock quantity.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                }
                else
                {
                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["FREEQTY"] = Convert.ToDecimal(e.FormattedValue);
                }
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colPMGRPFRQTY")
            {
                if (Convert.ToDecimal(e.FormattedValue) > Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpQty"]))
                {
                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["PMGRPFRQTY"] = _colValue;
                    dgvItemDetails.CancelEdit();
                    MessageBox.Show("Free quantity cannot be more than the Stock quantity.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                }
                else
                {
                    if (Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]) == 0)
                    { return; }

                    _commonDs.Tables["Item"].Rows[e.RowIndex]["PMGRPFRQTY"] = Convert.ToDecimal(e.FormattedValue);
                    this.Update_Conv_Qty("GRPQTY", "FREE", e.RowIndex);
                }
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colLooseQty")
            {
                if (Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]) == false)
                {
                    if (Convert.ToDecimal(e.FormattedValue) != Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["LooseQty"]))
                    {
                        decimal _nQty = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]) - Convert.ToDecimal(_colValue) + Convert.ToDecimal(e.FormattedValue);

                        decimal[] stkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), _nQty, e.RowIndex);       // Added by Sachin N. S. on 07/02/2019 
                        //if (Stock_Checking(Convert.ToInt16(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), _nQty, e.RowIndex) < 0)        //Commented by Sachin N. S. on 07/02/2019 
                        if (stkQty[0] < 0)
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["LooseQty"] = _colValue;
                            _nQty = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]);
                            string stkUnit = "";
                            if (llgpuom == true)
                                stkUnit = _commonDs.Tables["Item"].Rows[e.RowIndex]["StkUnit"].ToString();
                            dgvItemDetails.CancelEdit();
                            MessageBox.Show("Balance Stock : " + (stkQty[1] - _nQty).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            e.Cancel = true;
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[e.RowIndex]["LooseQty"] = Convert.ToDecimal(e.FormattedValue);
                            _commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"] = _nQty;
                        }
                    }
                }
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colBATCHNO" && _keypress != "F2")
            {
                if (e.FormattedValue.ToString() != "" && e.FormattedValue.ToString() != _colValue)
                {
                    string csql1 = "SELECT PrAppRate FROM Ac_mast WHERE Ac_Id = " + _commonDs.Tables["Main"].Rows[0]["Ac_id"].ToString();
                    DataRow _dr = _oDataAccess.GetDataRow(csql1, null, 25);
                    string cRateFld = "BatchRate";
                    if (_dr != null)
                    {
                        switch (_dr[0].ToString())
                        {
                            case "MRP":
                                cRateFld = "BatchRate";
                                break;
                            case "Purchase Rate":
                                cRateFld = "prate";
                                break;
                            case "Sales Rate":
                                cRateFld = "srate";
                                break;
                            case "PTR":
                                cRateFld = "ptr";
                                break;
                            case "PTS":
                                cRateFld = "pts";
                                break;
                            default:
                                cRateFld = "BatchRate";
                                break;
                        }
                    }

                    string cSrchCol = "", cDispCol = "", cRetCol = "", cFrmCap = "";
                    cSrchCol = "BatchNo";
                    cDispCol = "BatchNo:Batch No.,MfgDt:Mfg Dt.,ExpDt: Exp. Dt.,BatchRate:Batch Rate,Inc_gst: Incl GST,Qty: Quantity";
                    cRetCol = "BatchNo,MfgDt,ExpDt," + cRateFld + ",Inc_gst,Qty,It_Code";

                    string csql = "SELECT It_Code, BatchNo, MfgDt, ExpDt, " + cRateFld + " as BatchRate, ";
                    csql = csql + (_dr[0].ToString().Trim() == "MRP" ? "MRPInc_gst" : "Inc_gst");
                    csql = csql + " as Inc_gst, Qty  FROM pRetBatchDetails WHERE it_code = ";
                    csql = csql + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString();
                    csql = csql + " and BatchonHold = 0 and '" + _commonDs.Tables["Main"].Rows[0]["Date"].ToString() + "' Between MfgDt and ExpDt and Qty>0 ";
                    csql = csql + " and BatchNo like '" + e.FormattedValue.ToString().Trim() + "%' and StoreNm='" + _commonDs.Tables["Item"].Rows[e.RowIndex]["Ware_nm"].ToString() + "' ";
                    DataTable searchTbl = _oDataAccess.GetDataTable(csql, null, 25);

                    // ***** Added by Sachin N. S. on 07/02/2019 -- Start
                    DataTable _dtBatch = searchTbl;
                    long _itcode = 0;
                    decimal _qty = 0;
                    string _bathcNo = "";
                    int _nRowNo = e.RowIndex;
                    foreach (DataRow _dr1 in _dtBatch.Rows)
                    {
                        _itcode = Convert.ToInt64(_commonDs.Tables["Item"].Rows[_nRowNo]["It_code"]);
                        _bathcNo = _dr1["BatchNo"].ToString();

                        _qty = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null && Convert.ToInt64(row["It_code"]) == _itcode && row["BatchNo"].ToString() == _bathcNo).Sum(row => Convert.ToDecimal(row["Qty"]));
                        _dr1["Qty"] = Convert.ToDecimal(_dr1["Qty"]) - _qty;
                        //if (Convert.ToDecimal(_dr1["Qty"]) > 0)
                        searchTbl.Rows[searchTbl.Rows.IndexOf(_dr1)]["Qty"] = _dr1["Qty"];
                        //else
                        //    searchTbl.Rows[searchTbl.Rows.IndexOf(_dr1)].Delete();
                    }
                    searchTbl.AcceptChanges();
                    // ***** Added by Sachin N. S. on 07/02/2019 -- End

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

                        if (Convert.ToDecimal(searchTbl.Rows[0]["Qty"]) > 0)
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = searchTbl.Rows[0]["BatchNo"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                            _colValue = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"].ToString();
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = _colValue;
                            MessageBox.Show("Goods out of Stock for the Batch No. : " + searchTbl.Rows[0]["BatchNo"].ToString(), CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        itemwiseTotal(this.BindingContext[_commonDs, "Item"].Position, 0);
                    }
                    else
                    {
                        _colValue = e.FormattedValue.ToString();
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
                    //if (Convert.ToDecimal(e.FormattedValue) == 0)
                    //{ return; }
                    if (Convert.ToDecimal(_colValue) == Convert.ToDecimal(e.FormattedValue))
                    { return; }

                    //Added by Shrikant S. on 02/01/2019 for Bug-32133      //Start
                    if (Convert.ToDecimal(e.FormattedValue) == 0)
                    {
                        //MessageBox.Show("Qty should be inclusive of Free Quantity",CommonInfo.AppCaption,MessageBoxButtons.OK,MessageBoxIcon.Information);
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["PMGRPFRQTY"] = 0;
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["FreeQty"] = 0;
                    }
                    //Added by Shrikant S. on 02/01/2019 for Bug-32133      //End

                    decimal _stkQty = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]);
                    _commonDs.Tables["Item"].Rows[e.RowIndex]["GpQty"] = Convert.ToDecimal(e.FormattedValue);
                    this.Update_Conv_Qty("GRPQTY", "GRP", e.RowIndex);

                    //if (Stock_Checking(Convert.ToInt16(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["LooseQty"]), e.RowIndex) < 0)

                    decimal[] stkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]), e.RowIndex);     // Added by Sachin N. S. on 07/02/2019 
                    //if (Stock_Checking(Convert.ToInt16(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]), e.RowIndex) < 0)          // Commented by Sachin N. S. on 07/02/2019 
                    if (stkQty[0] < 0)
                    {
                        string stkUnit = "";
                        if (llgpuom == true)
                            stkUnit = _commonDs.Tables["Item"].Rows[e.RowIndex]["StkUnit"].ToString();
                        _commonDs.Tables["Item"].Rows[e.RowIndex]["GpQty"] = Convert.ToDecimal(_colValue);
                        _commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"] = _stkQty;
                        dgvItemDetails.CancelEdit();
                        MessageBox.Show("Balanace quantity : " + (stkQty[1] - _stkQty).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Cancel = true;
                    }
                }

                if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colRate")
                {
                    if (Convert.ToInt64(_commonDs.Tables["Item"].Rows[e.RowIndex]["It_code"]) == 0)
                    { return; }

                    decimal _stkRate = Convert.ToDecimal(e.FormattedValue);
                    _commonDs.Tables["Item"].Rows[e.RowIndex]["Rate"] = _stkRate;
                    //Added by Shrikant S. on 03/01/2019 for Bug-32133      && Start
                    if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["GpQty"]) <= 0)
                        _commonDs.Tables["Item"].Rows[e.RowIndex]["GPRate"] = 0;
                    else
                        //Added by Shrikant S. on 03/01/2019 for Bug-32133      && End
                        _commonDs.Tables["Item"].Rows[e.RowIndex]["GPRate"] = ((Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["Qty"]) - Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["LooseQty"])) / Convert.ToDecimal(_commonDs.Tables["Item"].Rows[e.RowIndex]["GpQty"])) * _stkRate;
                    _curRowNo = e.RowIndex;
                }
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colgpunit" && _keypress != "F2")
            {
                if (e.FormattedValue.ToString() != "" && e.FormattedValue.ToString() != _colValue)
                {
                    string _munit = string.Empty, _mgroupuom = string.Empty;
                    string xcFldList = "", xcDispCol = "", xcRetCol = "", cFrmCap = "", cSrchCol = "", cDispCol = "", cRetCol = "";
                    string cSql = "Select Top 1 RateUnit,s_unit,p_unit From It_mast Where It_code =" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString();
                    DataTable _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                    if (_dt.Rows.Count > 0)
                    {
                        _munit = _dt.Rows[0]["RateUnit"].ToString();
                        _mgroupuom = Convert.ToBoolean(_commonDs.Tables["lcode"].Rows[0]["iotrans"]) == true ? _dt.Rows[0]["p_unit"].ToString() : _dt.Rows[0]["s_unit"].ToString();
                    }

                    if (_mgroupuom != string.Empty)
                    {
                        cSql = "if Exists( Select top 1 subuom from GroupUOM Where baseuom='" + _munit.Trim() + "' and groupuom='" + _mgroupuom.Trim() + "') ";
                        cSql += " Select subuom  From GroupUOM Where baseuom='" + _munit.Trim() + "' and groupuom='" + _mgroupuom.Trim() + "' and SubUom like '%" + e.FormattedValue.ToString().Trim() + "%' Group by subuom Order by subuom ";
                        cSql += " Else";
                        cSql += " Select u_uom as subuom From uom where u_Uom like '%" + e.FormattedValue.ToString().Trim() + "%' Group by u_uom Order by u_uom ";

                        xcFldList = "";
                        xcDispCol = "";
                        xcRetCol = "";
                        cFrmCap = "Select Group UOM";
                        cSrchCol = "subuom";
                        cDispCol = "subuom:Group UOM";
                        cRetCol = "subuom";

                        _dt = _oDataAccess.GetDataTable(cSql, null, 50);

                        if (_dt != null)
                        {
                            if (_dt.Rows.Count > 0)
                            {
                                if (_dt.Rows.Count > 1)
                                {
                                    DataView _dvw1 = _dt.DefaultView;

                                    udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
                                    _oSelPop.pdataview = _dvw1;
                                    _oSelPop.pformtext = cFrmCap;
                                    _oSelPop.psearchcol = cSrchCol;
                                    _oSelPop.pDisplayColumnList = cDispCol;
                                    _oSelPop.pRetcolList = cRetCol;
                                    _oSelPop.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
                                    _oSelPop.ShowDialog();

                                    if (_oSelPop.pReturnArray != null)
                                    {
                                        DataTable _dtNew = _dt.Select("subuom='" + _oSelPop.pReturnArray[0].ToString().Trim() + "'").CopyToDataTable();
                                        _dt = _dtNew;
                                    }
                                }

                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _dt.Rows[0][0].ToString();
                                _munit = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"].ToString();

                                if (_munit != _colValue)
                                {
                                    cSql = " Select top 1 " + (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Iotrans"]) == true ? "prate" : "sRate") + " as gpuomrate from groupuomrate Where It_code=" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString() + " and subuom='" + _munit.Trim() + "'";

                                    _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                                    bool lDispB4 = Convert.ToBoolean(_commonDs.Tables["UdPOSSettings"].Rows[0]["SHUFFLEUOM"]);
                                    if (_dt != null)
                                    {
                                        if (lDispB4 == false)
                                        {
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gprate"] = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpRate"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                        }
                                        this.Update_Conv_Qty("GRPUNT", "GRP", this.BindingContext[_commonDs, "Item"].Position);
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gprate"] = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpRate"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                    }
                                    itemwiseTotal(dgvItemDetails.CurrentCell.RowIndex, dgvItemDetails.CurrentCell.ColumnIndex);
                                    _colValue = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"].ToString().Trim();
                                }
                            }
                            else
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpUnit"] = _colValue;
                                MessageBox.Show("UOM not found in the UOM Group Master / UOM Master.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpUnit"] = _colValue;
                            MessageBox.Show("UOM not found in the UOM Group Master / UOM Master.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    if (e.FormattedValue.ToString() == "" && _colValue != "")
                    {
                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpUnit"] = _colValue;
                        MessageBox.Show("UOM cannot be blank.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            _keypress = "";
        }

        private void dgvItemDetails_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItemAddInfo")
            {
                return;
            }

            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItem")
            {
                _item = dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            _colValue = dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void dgvItemDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItemDetails.Columns[e.ColumnIndex].Name == "colItemAddInfo")
            {
                int iColumn = dgvItemDetails.CurrentCell.ColumnIndex;
                int iRow = dgvItemDetails.CurrentCell.RowIndex;

                _commonDs.Tables["ITEM"].AcceptChanges();

                udAdditionalInfo.udAdditionalInfo _udAdditionalInfo = new udAdditionalInfo.udAdditionalInfo();
                _udAdditionalInfo._commonDs = _commonDs;
                _udAdditionalInfo._EntryTy = CommonInfo.EntryTy;
                _udAdditionalInfo._HdrDtl = "D";
                _udAdditionalInfo._TblName = "ITEM";
                _udAdditionalInfo._FilterRowId = this.dgvItemDetails.Rows[e.RowIndex].Cells["ItemRowId"].Value.ToString();
                _udAdditionalInfo.callAdditionalInfo();

                dgvItemDetails.CurrentCell = dgvItemDetails[iColumn, iRow];

                return;
            }

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

            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["GSTIncl"]) == true)
            {
                if (e.ColumnIndex == dgvItemDetails.Columns["colGSTIncl"].Index && e.RowIndex >= 0)
                {
                    this.dgvItemDetails.EndEdit();
                    itemwiseTotal(e.RowIndex, 0);
                    dgvItemDetails.CurrentCell = dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
            }

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

        private void dgvItemDetails_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (ldntValidate == true) return;

            if (dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Qty" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Rate" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Item" || Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag) == "V#" || Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag) == "%#" || Convert.ToString(dgvItemDetails.Columns[e.ColumnIndex].Tag) == "S#" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "FreeQty" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "gpqty" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "LooseQty" || dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "PMGRPFRQTY")
            {
                if (_colValue != dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                {
                    dgvItemDetails.EndEdit();
                    if (dgvItemDetails.Columns[e.ColumnIndex].DataPropertyName == "Qty")
                    {
                        this.Update_Conv_Qty("QTY", "", e.RowIndex);
                    }
                    itemwiseTotal(e.RowIndex, e.ColumnIndex);
                    _colValue = dgvItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
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
            itemwiseTotal(dgvItemDetails.CurrentCell.RowIndex, dgvItemDetails.CurrentCell.ColumnIndex);
        }

        private void dgvDiscChrgs_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (ldntValidate == true) return;

            if (_dgvDiscChrgsdataBind == true)
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
                _colValue = dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
        }

        private void dgvDiscChrgs_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (_colValue != dgvDiscChrgs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
            {
                if (e.ColumnIndex == 2)
                {
                    _commonDs.Tables["HdDiscChrgs"].Rows[e.RowIndex]["Def_Pert"] = 0;
                }

                this.CalculateHeaderTax(_commonDs.Tables["HdDiscChrgs"].Rows[e.RowIndex]["Fld_Nm"].ToString(), e.RowIndex, e.ColumnIndex);
                this.RefreshControls();
            }
        }

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
            _udAdditionalInfo._EntryTy = CommonInfo.EntryTy;
            _udAdditionalInfo._HdrDtl = "H";
            _udAdditionalInfo._TblName = "MAIN";
            _udAdditionalInfo.callAdditionalInfo();
        }

        private void btnPrintInv_Click(object sender, EventArgs e)
        {
            popUpFromTable("PRINTBILL");
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            PayMode = "DirectCash";
            this.PaymentOption();
        }

        private void btnPayOption_Click(object sender, EventArgs e)
        {
            this.PaymentOption();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.ExitForm();
        }

        private void btnPrintInv_MouseHover(object sender, EventArgs e)
        {
            //_toolTip.Show("Reprint Bill", btnPrintInv);
        }

        private void btnPayment_MouseHover(object sender, EventArgs e)
        {
            _toolTip.Show("Pay Bill", btnPayment);
        }

        private void btnExit_MouseHover(object sender, EventArgs e)
        {
            _toolTip.Show("Exit POS", btnExit);
        }

        private void dgvItemDetails_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != dgvItemDetails.BindingContext[_commonDs, "Item"].Position && _keyEnter != "ENTERED")
                dgvItemDetails.BindingContext[_commonDs, "Item"].Position = e.RowIndex;
        }

        private void cmdWarehouse_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                this.txtParty.Focus();
        }

        private void cmdParty_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                this.txtMobileNo.Focus();
        }

        private void cmdMobileNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                if (this.tblXtraInfo.Controls.Count > 0)
                    this.tblXtraInfo.Controls[1].Focus();
        }

        #endregion Control Events

        #region Private Methods

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

        #region Build dataset and Refresh controls
        private void getDataCursor()
        {
            _oDataAccess = new clsDataAccess();
            _commonDs = new DataSet();

            _commonDs = _oDataAccess.GetDataSet("Execute USP_GET_PS_DATASET '" + CommonInfo.EntryTy + "',0", null, 25);
            _commonDs.Tables[0].TableName = "Lcode";
            _commonDs.Tables[1].TableName = "Main";
            _commonDs.Tables[2].TableName = "Item";
            _commonDs.Tables[3].TableName = "PSPayDetail";
            _commonDs.Tables[4].TableName = "DcMast";
            _commonDs.Tables[5].TableName = "DiscChrgsFldLst";
            _commonDs.Tables[6].TableName = "Stax_Mas";
            _commonDs.Tables[7].TableName = "UdPOSSettings";
            _commonDs.Tables[8].TableName = "UploadFiles";

            if (CommonInfo.EntryTy == "HS")
            {
                _commonDs.Tables[2].Columns.Add(new DataColumn("SchdDrug", typeof(bool)));
            }

            if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["V_Extra"]) == true)
            {
                DataTable _dt = new DataTable();
                _dt = _oDataAccess.GetDataTable("Select * from Lother where E_Code='" + CommonInfo.EntryTy + "' Order by Att_file, Serial, SubSerial", null, 25);
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
        }

        private void RefreshControls()
        {
            txtTotalQty.Text = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null).Sum(row => Convert.ToDecimal(row["Qty"])), 2).ToString();
            txtNoOfItems.Text = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count().ToString();
            txtGrossAmt.Text = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Gro_amt"] != null).Sum(row => Convert.ToDecimal(row["Gro_amt"])), 2).ToString();
            int _roundoff = Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Net_Op"]) ? 0 : 2;

            decimal gstamt = 0M;
            gstamt = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["SGST_AMT"] != null).Sum(row => Convert.ToDecimal(row["SGST_AMT"])), 2);
            _commonDs.Tables["Main"].Rows[0]["SGST_AMT"] = Convert.ToDecimal(gstamt);
            _commonDs.Tables["Main"].Rows[0]["TOT_EXAMT"] = Convert.ToDecimal(gstamt);

            gstamt = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["CGST_AMT"] != null).Sum(row => Convert.ToDecimal(row["CGST_AMT"])), 2);
            _commonDs.Tables["Main"].Rows[0]["CGST_AMT"] = Convert.ToDecimal(gstamt);
            _commonDs.Tables["Main"].Rows[0]["TOT_EXAMT"] = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["TOT_EXAMT"]) + Convert.ToDecimal(gstamt);

            _commonDs.Tables["Main"].Rows[0]["Gro_Amt"] = Convert.ToDecimal(txtGrossAmt.Text);
            this.CalculateHeaderTax("", 0, 0);
            this.RefreshSummaryGrid();
            txtNetAmt.Text = _commonDs.Tables["Main"].Rows[0]["Net_Amt"].ToString();

            decimal totMrpAmt = 0M, totNetAmt = 0M;
            totNetAmt = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Net_Amt"]);
            totMrpAmt = Math.Round(_commonDs.Tables["Item"].AsEnumerable().Where(row => row["U_ASSEAMT"] != null).Sum(row => Convert.ToDecimal(row["Qty"]) * Convert.ToDecimal(row["Rate"])), 2);
            this.txtMRPSaved.Text = (totNetAmt > totMrpAmt ? 0 : totMrpAmt - totNetAmt).ToString();

            this.Refresh();
        }

        private void RefreshSummaryGrid()
        {

            Decimal _Amount = 0;
            var _currentRow = (from currentRw in _dtSumDiscChrgs.AsEnumerable()
                               where currentRw.Field<string>("HeadingNm") == "Line Gross Amount" && currentRw.Field<string>("HdrDet") == "I"
                               select currentRw).FirstOrDefault();
            //_currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["U_ASSEAMT"] != null).Sum(row => Convert.ToDecimal(row["U_ASSEAMT"]));
            _currentRow["Amount"] = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["U_ASSEAMT"] != null).Sum(row => Convert.ToDecimal(row["U_ASSEAMT"])) - _commonDs.Tables["Item"].AsEnumerable().Where(row => row["TOT_DEDUC"] != null).Sum(row => Convert.ToDecimal(row["TOT_DEDUC"]));

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
        }

        private void itemwiseTotal(int _grdRowPos, int _grdColPos)
        {
            _grdColPos = dgvItemDetails.Columns[_grdColPos].DisplayIndex;
            string _fldNm, _a_s;
            DataRow _dr, _dr1, _dr2;
            _commonDs.Tables["Item"].Columns["U_asseamt"].ReadOnly = false;
            _commonDs.Tables["Item"].Columns["Gro_amt"].ReadOnly = false;
            _dr1 = _commonDs.Tables["Item"].Rows[_grdRowPos];
            _dr1["Tot_deduc"] = 0;
            _dr1["Tot_tax"] = 0;
            _dr1["Tot_examt"] = 0;
            _dr1["Tot_add"] = 0;
            _dr1["Tot_nontax"] = 0;
            _dr1["Tot_fdisc"] = 0;

            decimal _maValue = 0M;
            //_maValue = (Convert.ToDecimal(_dr1["Qty"]) - Convert.ToDecimal(_dr1["FreeQty"])) * Convert.ToDecimal(_dr1["Rate"]);       //Commented by Shrikant S. on 29/12/2018 for Installer 2.1.0    

            //Added by Shrikant S. on 29/12/2018 for Installer 2.1.0            //Start
            decimal nqty = Convert.ToDecimal(_dr1["Qty"]);
            decimal mratio = 1;
            if (Convert.ToDecimal(_dr1["LooseQty"]) != 0)
                nqty = nqty - Convert.ToDecimal(_dr1["LooseQty"]);

            if (Convert.ToDecimal(_dr1["GpQty"]) != 0)
            {
                mratio = nqty / Convert.ToDecimal(_dr1["GpQty"]);
            }
            _maValue = (Convert.ToDecimal(_dr1["Qty"]) - (mratio * Convert.ToDecimal(_dr1["FreeQty"]))) * Convert.ToDecimal(_dr1["Rate"]);
            //Added by Shrikant S. on 29/12/2018 for Installer 2.1.0            //End

            string _cpertNm = "";
            decimal _npertnm = 0M, _mAmt = 0M;
            decimal _mCalAmt = 0M;
            decimal _maValue2 = 0M;
            _maValue2 = Convert.ToDecimal(_maValue);
            if (Convert.ToBoolean(_dr1["GSTIncl"]) == true)
            {
                _maValue2 = (Convert.ToDecimal(_maValue) / (1 + (Convert.ToDecimal(_dr1["GstRate"]) / 100)));
            }

            _dr1["u_asseamt"] = _maValue2;

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
                        _maValue2 = _maValue2 + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));

                        //Commented by Shrikant S. on 27/12/2018        //Start
                        //if (Convert.ToBoolean(_dr["Bef_Aft"]) == true)
                        //{
                        //    _dr1["u_asseamt"] = _maValue2;
                        //}
                        //Commented by Shrikant S. on 27/12/2018        //End

                        //Added by Shrikant S. on 27/12/2018        //Start
                        if (Convert.ToInt32(_dr["Bef_Aft"]) == 1)
                        {
                            _dr1["u_asseamt"] = _maValue2;
                        }
                        //Added by Shrikant S. on 27/12/2018        //End

                        if (_dr["Excl_Gross"].ToString() != "C" && _dr["Excl_Gross"].ToString() != "A" && _dr["Excl_Gross"].ToString() != "E" && _dr["Excl_Gross"].ToString() != "P")
                        {
                            //_maValue2 += _a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]);

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

                        _npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_grdRowPos][_cpertNm]);
                        if (_npertnm > 0)
                        {

                            if (_dr["amtexpr"].ToString().Trim() != "")
                            {
                                string[] _strExpr = _dr["amtexpr"].ToString().Trim().Split(new[] { '.' });
                                string _cTblName = _strExpr[0].ToString().ToUpper() == "ITEM_VW" ? "Item" : "Main";
                                string _cFldName = _strExpr[1].ToString();
                                _mCalAmt = Convert.ToDecimal(_commonDs.Tables[_cTblName].Rows[_grdRowPos][_cFldName]);
                            }


                            if (_dr["Disp_Sign"].ToString().Trim() == "%")
                            {
                                _mAmt = (_dr["amtexpr"].ToString().Trim() != "" ? _mCalAmt : _maValue2) * (_npertnm / 100);
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
                                _mAmt = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_grdRowPos][_fldNm]);
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
                                _mAmt = _maValue2 * (_npertnm / 100);
                            }

                            if (Convert.ToString(_dgvCol.Tag) == "S#")
                            {
                                _dr1[_fldNm] = _mAmt;
                            }
                        }
                    }

                    //if (Convert.ToString(_dgvCol.Tag) != "S#" && Convert.ToString(_dgvCol.Tag) != "%#")       //Commented by Shrikant S. on 27/12/2018 for Installer 2.1.0    
                    if (Convert.ToString(_dgvCol.Tag) != "S#" && Convert.ToString(_dgvCol.Tag) != "%#" && Convert.ToString(_dgvCol.Tag) != "@#")             //Added by Shrikant S. on 27/12/2018 for Installer 2.1.0    
                    {
                        //Commented by Shrikant S. on 27/12/2018 for Installer 2.1.0        //Start
                        //if (Convert.ToBoolean(_dr["Bef_Aft"]) == true)
                        //{
                        //    _maValue2 = Convert.ToDecimal(_maValue2) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                        //    _dr1["u_asseamt"] = _maValue2;
                        //}
                        //Commented by Shrikant S. on 27/12/2018 for Installer 2.1.0        //End

                        //Added by Shrikant S. on 27/12/2018 for Installer 2.1.0        //Start
                        if (Convert.ToInt32(_dr["Bef_Aft"]) == 1)
                        {
                            _maValue2 = Convert.ToDecimal(_maValue2) + (_a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]));
                            _dr1["u_asseamt"] = _maValue2;
                        }
                        //Added by Shrikant S. on 27/12/2018 for Installer 2.1.0        //End

                        if (_dr["Excl_Gross"].ToString() != "C" && _dr["Excl_Gross"].ToString() != "A" && _dr["Excl_Gross"].ToString() != "E" && _dr["Excl_Gross"].ToString() != "P")
                        {
                            //Commented by Shrikant S. on 27/12/2018 for Installer 2.1.0      //Start
                            //if (Convert.ToBoolean(_dr["Bef_Aft"]) == false)
                            //    _maValue2 += _a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]);
                            //Commented by Shrikant S. on 27/12/2018 for Installer 2.1.0       //End

                            //Added by Shrikant S. on 27/12/2018 for Installer 2.1.0        //Start
                            if (Convert.ToInt32(_dr["Bef_Aft"]) != 1)
                                _maValue2 += _a_s == "+" ? +Convert.ToDecimal(_dr1[_fldNm]) : -Convert.ToDecimal(_dr1[_fldNm]);
                            //Added by Shrikant S. on 27/12/2018 for Installer 2.1.0        //End


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
            _dr1["gro_amt"] = Convert.ToDecimal(_maValue2);
            #endregion Evaluating Column values after the column edited
            this.RefreshControls();
        }

        #endregion Build dataset and Refresh controls

        #region Clear and reset Datatables
        private void clearTables()
        {
            foreach (DataTable _dt in _commonDs.Tables)
            {
                //if (_dt.TableName != "Lcode" && _dt.TableName != "DCMast" && _dt.TableName != "DiscChrgsFldLst" && _dt.TableName != "HdDiscChrgs" && _dt.TableName != "DtDiscChrgs" && _dt.TableName != "Stax_Mas" && _dt.TableName != "UdPOSSettings")                   //  Commented by Shrikant S. on 28/12/2018 for Installer 2.1.0  
                if (_dt.TableName.ToLower() != "Lcode".ToLower() && _dt.TableName.ToLower() != "DCMast".ToLower() && _dt.TableName.ToLower() != "DiscChrgsFldLst".ToLower() && _dt.TableName.ToLower() != "HdDiscChrgs".ToLower() && _dt.TableName.ToLower() != "DtDiscChrgs".ToLower() && _dt.TableName.ToLower() != "Stax_Mas".ToLower() && _dt.TableName.ToLower() != "UdPOSSettings".ToLower())     //  Added by Shrikant S. on 28/12/2018 for Installer 2.1.0  
                {
                    _dt.Clear();
                }
                //if (_dt.TableName == "HdDiscChrgs")       //  Commented by Shrikant S. on 28/12/2018 for Installer 2.1.0  
                if (_dt.TableName.ToLower() == "HdDiscChrgs".ToLower())         //  Added by Shrikant S. on 28/12/2018 for Installer 2.1.0  
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
            txtGrossAmt.DataBindings.Add("text", _commonDs, "Main.Gro_amt");
            txtNetAmt.DataBindings.Add("text", _commonDs, "Main.Net_amt");
            txtTaxAmt.DataBindings.Add("text", _commonDs, "Main.TaxAmt");
            txtParty.DataBindings.Add("text", _commonDs, "Main.Party_nm");
            txtMobileNo.DataBindings.Add("text", _commonDs, "Main.NMobileNo");
            this.lblWarehouse.Text = (CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse");
            if (CommonInfo.EntryTy != "HS")
                this.btnUpldPresc.Visible = false;

            _tabOrder = 1;
            this.txtInvoiceNo.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.txtInvoiceDt.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.txtWareHouse.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.cmdWarehouse.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.txtParty.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.cmdParty.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.txtMobileNo.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.cmdMobileNo.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.chkPrintBill.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.btnUpldPresc.TabIndex = _tabOrder;
            _tabOrder += 1;
            this.btnAddInfo.TabIndex = _tabOrder;
            _tabOrder += 1;

            gridBinding();
        }

        private void gridBinding()
        {
            // ***** Binding Item Grid -- Start ***** \\
            dgvItemDetails.TabIndex = _tabOrder;
            _tabOrder += 1;
            dgvItemDetails.AutoGenerateColumns = false;
            dgvItemDetails.ClearSelection();
            dgvItemDetails.DataSource = _commonDs.Tables["Item"];

            if (_commonDs.Tables["Lcode"].Columns.Contains("gpuomapp") == true)
            {
                if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["gpuomapp"]) == true)
                {
                    this.llgpuom = true;
                }
            }

            dgvItemDetails.Columns[0].DataPropertyName = "Item_no";
            dgvItemDetails.Columns[1].DataPropertyName = "Item";
            dgvItemDetails.Columns[2].DataPropertyName = "Qty";
            dgvItemDetails.Columns[3].DataPropertyName = "Rate";

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
                    _dchk.Width = 75;
                    _dchk.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    _dchk.SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvItemDetails.Columns.Add(_dchk);

                    _colId += 1;
                }
            }

            DataRow _dr;
            DataRow[] _drdcmast;
            DataTable _dtdcmast;
            string _expStr = "(Qty+LooseQty-FreeQty)*Rate";
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

            int tabIndex = this.cmdMobileNo.TabIndex + 1;
            if (_commonDs.Tables["Lother"].Rows.Count > 0)
            {
                int left = this.lblWarehouse.Left;
                int top = this.lblWarehouse.Top + this.lblWarehouse.Height + 2;
                int linecount = 0;
                int _xInfoRwNo = 0;
                int _xInfoColNo = 0;
                int _xFldColPos = 0;
                //int tabIndex = this.cmdMobileNo.TabIndex + 1;
                for (int i = 0; i < _commonDs.Tables["Lother"].Rows.Count; i++)
                {
                    if (Convert.ToBoolean(_commonDs.Tables["Lother"].Rows[i]["ingrid"]) == true && Convert.ToBoolean(_commonDs.Tables["Lother"].Rows[i]["att_file"]) == true && _commonDs.Tables["Lother"].Rows[i]["Inter_Use"].ToString().ToUpper() != ".T.")
                    {
                        linecount++;
                        string fldnm = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["fld_nm"]).Trim();
                        string headnm = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["head_nm"]).Trim();
                        Label vlbl = null;
                        Font font = new Font("Arial", 8.0f, FontStyle.Bold);
                        switch (Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["data_ty"]).Trim().ToLower())
                        {
                            case "varchar":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.Font = font;

                                vlbl.AutoSize = true;
                                vlbl.TabIndex = tabIndex;
                                tabIndex += 1;
                                vlbl.Visible = true;
                                vlbl.TextAlign = ContentAlignment.MiddleCenter;
                                vlbl.Margin = new Padding(0);
                                vlbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
                                vlbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                                this.tblXtraInfo.Controls.Add(vlbl, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;

                                TextBox txt = new TextBox();
                                txt.Name = "txt" + fldnm;
                                //txt.Width = 150;
                                txt.BorderStyle = BorderStyle.FixedSingle;
                                txt.DataBindings.Add("text", _commonDs, "Main." + fldnm);
                                txt.Enabled = true;
                                txt.ReadOnly = false;
                                txt.TabIndex = tabIndex;
                                txt.Visible = true;
                                txt.Font = font;
                                //txt.Margin = new Padding(0);
                                txt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
                                if (Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["filtcond"]).Trim().Length > 0)
                                {
                                    txt.Tag = "btn" + fldnm;
                                }
                                //txt.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
                                tabIndex += 1;
                                this.tblXtraInfo.Controls.Add(txt, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;

                                this.dgvItemDetails.Height = this.dgvItemDetails.Height - txt.Height - 4;
                                string filtcond = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["filtcond"]).Trim();
                                if (filtcond.Length > 0)
                                {

                                    Button btn = new Button();
                                    btn.Name = "btn" + fldnm;
                                    btn.Text = "[F2]";
                                    btn.Tag = "<<" + filtcond + ">><<" + headnm + ">>";
                                    //btn.Width = 30;
                                    btn.UseVisualStyleBackColor = true;
                                    btn.TabIndex = tabIndex;
                                    btn.Visible = true;
                                    btn.Margin = new Padding(0);
                                    btn.Font = font;
                                    //btn.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                                    tabIndex += 1;
                                    this.tblXtraInfo.Controls.Add(btn, _xFldColPos, _xInfoRwNo);
                                    _xFldColPos += 1;
                                    btn.Click += new EventHandler(popupbtn_clicked);
                                    txt.KeyDown += new KeyEventHandler(txtF2KeyPress_clicked);
                                }
                                break;
                            case "numeric":
                            case "decimal":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.AutoSize = false;
                                vlbl.TabIndex = tabIndex;
                                vlbl.Visible = true;
                                tabIndex += 1;
                                this.tblXtraInfo.Controls.Add(vlbl, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;
                                uNumericTextBox.cNumericTextBox ntxt = new uNumericTextBox.cNumericTextBox();
                                ntxt.Name = "txt" + fldnm;
                                ntxt.Width = 150;
                                ntxt.DataBindings.Add("text", _commonDs, "Main." + fldnm);
                                ntxt.TabIndex = tabIndex;
                                ntxt.Visible = true;
                                tabIndex += 1;
                                this.tblXtraInfo.Controls.Add(ntxt, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;
                                break;
                            case "bit":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.AutoSize = false;
                                vlbl.TabIndex = tabIndex;
                                vlbl.Visible = true;
                                tabIndex += 1;
                                this.tblXtraInfo.Controls.Add(vlbl, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;
                                CheckBox chk = new CheckBox();
                                chk.Name = "chk" + fldnm;
                                chk.Width = 120;
                                chk.DataBindings.Add("Checked", _commonDs, "Main." + fldnm);
                                chk.TabIndex = tabIndex;
                                chk.Visible = true;
                                tabIndex += 1;
                                this.tblXtraInfo.Controls.Add(chk, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;
                                break;
                            case "datetime":
                                vlbl = new Label();
                                vlbl.Name = "lbl" + fldnm;
                                vlbl.Text = headnm;
                                vlbl.AutoSize = false;
                                vlbl.TabIndex = tabIndex;
                                vlbl.Visible = true;
                                tabIndex += 1;
                                this.tblXtraInfo.Controls.Add(vlbl, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;
                                DateTimePicker dpk = new DateTimePicker();
                                dpk.Name = "dpk" + fldnm;
                                dpk.Width = 100;
                                dpk.CustomFormat = " ";
                                dpk.Format = DateTimePickerFormat.Custom;
                                dpk.ValueChanged += new System.EventHandler(this.dpk_ValueChanged);
                                dpk.TabIndex = tabIndex;
                                dpk.Visible = true;
                                tabIndex += 1;

                                Binding b = new Binding("Value", _commonDs, "Main." + fldnm, true);
                                b.Format += new ConvertEventHandler(dpk_Format);
                                b.Parse += new ConvertEventHandler(dpk_Parse);
                                dpk.DataBindings.Add(b);

                                this.tblXtraInfo.Controls.Add(dpk, _xFldColPos, _xInfoRwNo);
                                _xFldColPos += 1;
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
                            //int prevTop = this.pnlHoriz1.Top;
                            //this.pnlHoriz1.Top = vlbl.Top + vlbl.Height - 2;
                            //this.dgvItemDetails.Top = this.pnlHoriz1.Top + 4;

                            //this.dgvItemDetails.Height = this.pnlHoriz2.Top - this.pnlHoriz1.Top - 4;
                            left = left + 325;
                        }

                        if (_xInfoColNo == 2)
                        {
                            _xInfoColNo = 0;
                            _xInfoRwNo += 1;
                            _xFldColPos = 0;
                        }
                        else
                        {
                            _xInfoColNo += 1;
                        }

                    }
                }
                this.dgvItemDetails.TabIndex = tabIndex;
                tabIndex += 1;
                this.dgvDiscChrgs.TabIndex = tabIndex;
                tabIndex += 1;
                this.dgvSummary.TabIndex = tabIndex;
                tabIndex += 1;
                this.btnPayment.TabIndex = tabIndex;
                tabIndex += 1;
                this.btnPayOption.TabIndex = tabIndex;
                tabIndex += 1;
                //this.btnExit.TabIndex = tabIndex;
                //tabIndex += 1;

            }

            this.AddLinewiseAdditionalFields();

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

            // ***** Binding the Summary of the Gross Amount, Tax structure and Net Amount -- Start ***** \\
            dgvSummary.AutoGenerateColumns = false;
            dgvSummary.ClearSelection();

            _dtSumDiscChrgs = new DataTable();
            _dtSumDiscChrgs.TableName = "SumTaxChrgs";
            _dtSumDiscChrgs.Columns.Add(new DataColumn("HeadingNm", typeof(string)));
            _dtSumDiscChrgs.Columns.Add(new DataColumn("Amount", typeof(decimal)));
            _dtSumDiscChrgs.Columns.Add(new DataColumn("HdrDet", typeof(string)));

            //***** Line wise Charges
            _dr = _dtSumDiscChrgs.NewRow();
            _dr["HeadingNm"] = "Line Gross Amount";
            _dr["Amount"] = 0.00;
            _dr["HdrDet"] = "I";
            _dtSumDiscChrgs.Rows.Add(_dr);

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "D" && Convert.ToBoolean(row["Att_File"]) == false).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Less : Dedn. Before Tax";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "T" && Convert.ToBoolean(row["Att_File"]) == false).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["dcmast"].AsEnumerable().Where(row => Convert.ToString(row["code"]) == "E" && Convert.ToBoolean(row["Att_File"]) == false).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["udpossettings"].Rows[0]["I_disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["headingnm"] = "GST Charges";
                _dr["amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "A" && Convert.ToBoolean(row["Att_File"]) == false).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Add/Less Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "N" && Convert.ToBoolean(row["Att_File"]) == false).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Non Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "I";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "F" && Convert.ToBoolean(row["Att_File"]) == false).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true)
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
            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "D" && Convert.ToBoolean(row["Att_File"]) == true).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Less : Dedn. Before Tax";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "T" && Convert.ToBoolean(row["Att_File"]) == true).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "A" && Convert.ToBoolean(row["Att_File"]) == true).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Add/Less Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "N" && Convert.ToBoolean(row["Att_File"]) == true).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
            {
                _dr = _dtSumDiscChrgs.NewRow();
                _dr["HeadingNm"] = "Non Taxable Charges";
                _dr["Amount"] = 0.00;
                _dr["HdrDet"] = "H";
                _dtSumDiscChrgs.Rows.Add(_dr);
            }

            if (_commonDs.Tables["DCMast"].AsEnumerable().Where(row => Convert.ToString(row["Code"]) == "F" && Convert.ToBoolean(row["Att_File"]) == true).Count() > 0 && Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true)
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
        }

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
            string[] btnTag = btn.Tag.ToString().Split(new[] { ">><<" }, StringSplitOptions.None);
            string filtcond = btnTag[0].ToString().Trim().Replace("<<", "");
            string cfrmCapo = btnTag[1].ToString().Trim().Replace(">>", "");
            if (filtcond.ToString().Length > 0)
            {
                DataTable searchTbl = new DataTable();

                udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
                string cFrmCap = "Select ", cSrchCol = "", cDispCol = "", cRetCol = "";
                if (filtcond.ToLower().Contains("execute ") || filtcond.ToLower().Contains("select ") || filtcond.ToLower().Contains("exec "))
                {
                    cFrmCap = cFrmCap + cfrmCapo;
                    string csql = (filtcond.Trim().IndexOf("{") > 0 ? filtcond.Trim().Substring(0, filtcond.Trim().IndexOf("{") - 1) : filtcond.Trim());
                    searchTbl = _oDataAccess.GetDataTable(csql, null, 25);

                    cSrchCol = searchTbl.Columns[0].ColumnName;
                    cRetCol = searchTbl.Columns[0].ColumnName;
                    cDispCol = searchTbl.Columns[0].ColumnName;

                    if (filtcond.Trim().IndexOf("{") > 0)
                    {
                        filtcond = filtcond.Trim().Substring(filtcond.Trim().IndexOf("{") + 1, (filtcond.Trim().Trim().Length - filtcond.Trim().IndexOf("{") - 2));

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
                    Control[] ctl = this.tblXtraInfo.Controls.Find(ctlName, false);
                    TextBox txt = (TextBox)ctl[0];
                    txt.Text = _oSelPop.pReturnArray[0].ToString();
                }
            }
        }

        private void txtF2KeyPress_clicked(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                TextBox _txtBox = (TextBox)sender;
                string _btnNm = _txtBox.Tag.ToString();

                Button _btn = (Button)this.Controls.Find(_btnNm, true)[0];

                _btn.PerformClick();
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
                    bool _addInfobutton = (Convert.ToInt32(_commonDs.Tables["Lother"].Compute("Count(Fld_nm)", "Att_file=False And Ingrid=False And Inter_Use='.F.' and Fld_nm not in ('BATCHNO','LOOSEQTY','FREEQTY')")) > 0 ? true : false);
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
                            if (_commonDs.Tables["Lother"].Rows[i]["Fld_nm"].ToString().Trim() == "BATCHNO" || _commonDs.Tables["Lother"].Rows[i]["Fld_nm"].ToString().Trim() == "LOOSEQTY" || _commonDs.Tables["Lother"].Rows[i]["Fld_nm"].ToString().Trim() == "FREEQTY")
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
                                    txtColV.SortMode = DataGridViewColumnSortMode.NotSortable;
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
                                    txtColN.SortMode = DataGridViewColumnSortMode.NotSortable;
                                    this.dgvItemDetails.Columns.Add(txtColN);
                                    break;
                                case "bit":
                                    var chkCol = new DataGridViewCheckBoxColumn();
                                    chkCol.HeaderText = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Head_nm"]).Trim();
                                    chkCol.Name = "chk" + Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    chkCol.DataPropertyName = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    chkCol.Width = 150;
                                    chkCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                                    this.dgvItemDetails.Columns.Add(chkCol);
                                    break;
                                case "datetime":
                                    var udpkCol = new udclsDGVDateTimePicker.MicrosoftDateTimePicker();
                                    udpkCol.HeaderText = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Head_nm"]).Trim();
                                    udpkCol.Name = "dpk" + Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    udpkCol.DataPropertyName = Convert.ToString(_commonDs.Tables["Lother"].Rows[i]["Fld_nm"]).Trim();
                                    udpkCol.Width = 100;
                                    udpkCol.SortMode = DataGridViewColumnSortMode.NotSortable;
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

        private void CreateItChrgsColDynamically(ref string _expStr, ref int _colId)
        {
            udclsDGVNumericColumn.CNumEditDataGridViewColumn _dc;
            _expStr = (_expStr == "" ? "(Qty+LooseQty-FreeQty)*Rate" : _expStr);
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
                        _dc.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        _dc.DecimalLength = 2;
                        _dc.DisplayIndex = _colId;
                        _dc.Tag = "%#";
                        _dc.Visible = ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["LSHWITMDISC"] ? !((bool)_dr["LSHWDISCCG"]) : !((bool)_dr["LSHWDISCCG"]));
                        _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvItemDetails.Columns.Add(_dc);
                        _colId += 1;
                    }

                    _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                    _dc.Name = "col" + _dr["Fld_Nm"].ToString().Trim();
                    _dc.DataPropertyName = _dr["Fld_Nm"].ToString().Trim();
                    _dc.HeaderText = _dr["Head_Nm"].ToString().Trim();
                    _dc.DecimalLength = 2;
                    _dc.DisplayIndex = _colId;
                    _dc.Tag = "V#";
                    _dc.Visible = ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["LSHWITMDISC"] ? !((bool)_dr["LSHWDISCCG"]) : !((bool)_dr["LSHWDISCCG"]));
                    _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvItemDetails.Columns.Add(_dc);

                    _colId += 1;
                    _expStr += _a_s + _dr["Fld_Nm"].ToString().Trim();
                }
                else
                {
                    DataGridViewComboBoxColumn _tb;
                    _tb = new DataGridViewComboBoxColumn();
                    _tb.Name = "colTax_Name";
                    _tb.DataPropertyName = "Tax_Name";
                    _tb.HeaderText = "Tax Name";
                    _tb.Items.AddRange(_commonDs.Tables["Stax_Mas"].AsEnumerable().Select(row => row["Tax_Name"].ToString()).ToArray());
                    _tb.Tag = "S#";
                    _tb.DisplayIndex = _colId;
                    _tb.Width = 150;
                    _tb.SortMode = DataGridViewColumnSortMode.NotSortable;
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
                    _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvItemDetails.Columns.Add(_dc);

                    _taxExpr = _expStr;
                    _colId += 1;
                    _expStr += _a_s + _dr["Fld_Nm"].ToString().Trim();
                }
            }

        }

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
                _dgtxt.Width = 80;
                dgvItemDetails.Columns.Add(_dgtxt);
                dgvItemDetails.Columns[_colId].Width = 80;
                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colgpqty";
                _dc.DataPropertyName = "gpqty";
                _dc.HeaderText = "Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 75;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 75;

                _colId += 1;

                dgvItemDetails.Columns[2].HeaderText = "Stk Qty";
                dgvItemDetails.Columns[2].DisplayIndex = _colId;
                dgvItemDetails.Columns[2].ReadOnly = true;
                dgvItemDetails.Columns[2].Width = 75;
                dgvItemDetails.Columns[2].Visible = false;

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
                _dc.Name = "colPMGRPFRQTY";
                _dc.DataPropertyName = "PMGRPFRQTY";
                _dc.HeaderText = "Free Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 75;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 75;

                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colLooseQty";
                _dc.DataPropertyName = "LooseQty";
                _dc.HeaderText = "Loose Qty";
                _dc.DecimalLength = 3;
                _dc.DisplayIndex = _colId;
                _dc.ReadOnly = false;
                _dc.Tag = "";
                _dc.DefaultCellStyle = _dgcStyl;
                _dc.Width = 75;
                _dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dc);
                dgvItemDetails.Columns[_colId].Width = 75;

                _colId += 1;

                dgvItemDetails.Columns[3].HeaderText = "Rate/Unit";
                dgvItemDetails.Columns[3].DisplayIndex = _colId;
                dgvItemDetails.Columns[3].ReadOnly = false;
                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.LightGray;
                dgvItemDetails.Columns[3].DefaultCellStyle = _dgcStyl;
                dgvItemDetails.Columns[3].Width = 75;
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
                _dc.Name = "colFreeQty";
                _dc.DataPropertyName = "FreeQty";
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
                dgvItemDetails.Columns[_colId].Width = 80;
                _colId += 1;

                _dgcStyl = new DataGridViewCellStyle();
                _dgcStyl.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                _dgcStyl.Format = "F3";
                _dgcStyl.BackColor = Color.LightGray;

                _dc = new udclsDGVNumericColumn.CNumEditDataGridViewColumn();
                _dc.Name = "colgpqty";
                _dc.DataPropertyName = "gpqty";
                _dc.HeaderText = "Qty";
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
                _dc.Name = "colLooseQty";
                _dc.DataPropertyName = "LooseQty";
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
                _dchk.Width = 75;
                _dchk.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _dchk.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvItemDetails.Columns.Add(_dchk);

                _colId += 1;
            }
        }
        #endregion

        #region Calling Popup Selection Screen
        private void popUpFromTable(string _popupType)
        {
            _oDataAccess = new clsDataAccess();
            string cSql = "";
            string cFrmCap = "", cSrchCol = "", cDispCol = "", cRetCol = "";
            string cTbl = "", cCond = "", cFldList = "", xcFldList = "", xcSrchCol = "", xcDispCol = "", xcRetCol = "";
            string[][] cFldLst;
            int _lRateLevel = -1;
            DataTable _dt = new DataTable();
            udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
            switch (_popupType)
            {
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
                    cFrmCap = "Select " + (CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse");
                    cSrchCol = "Ware_nm";
                    cDispCol = "Ware_nm:" + (CommonInfo.EntryTy == "HS" ? "Store Room" : "Warehouse") + xcDispCol;
                    cRetCol = "Ware_nm" + xcRetCol;
                    break;

                case "ITEM":
                    _oSelPop.pSearchType = true;           //Added by Shrikant S. on 26/12/2018 
                    _oSelPop.vSearchImg = this.pAppPath;
                    xcFldList = ",It_mast.RateUnit,It_mast.S_Unit,It_mast.Rate,It_Mast.InclSTTax,It_Mast.AskQty,It_Mast.AskRate,It_Mast.prSalt ";
                    xcDispCol = ",S_Unit:UOM";
                    xcRetCol = ",RateUnit,S_Unit,Rate,InclSTTax,AskQty,AskRate,prSalt";

                    cTbl = " It_Mast ";
                    cCond = " It_Mast.It_code!=0 and It_mast.isService=0 ";

                    //******* Checking for Rate in Item Rate master -- Start *******\\
                    if ((bool)_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["It_Rate"] == true)
                    {
                        string cSql1 = "Select top 1 Rate_Level From Ac_mast where Ac_id = " + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                        _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                        _lRateLevel = Convert.ToInt16(_dt.Rows[0]["Rate_Level"]);
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

                    string cFldStr = splitText(cFldLst, ref xcSrchCol, ref xcDispCol);

                    cCond += " And (It_mast.ldeactive = 0 Or (It_mast.ldeactive = 1 And It_mast.deactfrom > '" + txtInvoiceDt.Text.ToString() + "' )) ";
                    cSql = "set dateformat dmy select " + cFldStr + xcFldList + " from " + cTbl + " where " + cCond + " order by It_Name ";
                    cFrmCap = "Select Goods";
                    cSrchCol = xcSrchCol;
                    cDispCol = xcDispCol;
                    cRetCol = "It_Code,It_Name" + xcRetCol;
                    break;

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

                case "PRINTBILL":
                    xcFldList = ",It_mast.Rate,It_mast.Tax_Name ";
                    xcDispCol = ",Rate:Goods Rate,Tax_Name:Tax Name";
                    xcRetCol = ",Rate,Tax_Name";
                    cTbl = " DCMAIN ";
                    cCond = " Entry_ty='" + CommonInfo.EntryTy + "' and DATE='" + DateTime.Now.Date + "' ";

                    cSql = "set dateformat dmy select Inv_No,Tran_cd from " + cTbl + " where " + cCond + " order by Inv_No ";
                    cFrmCap = "Select Bill No.";
                    cSrchCol = "Inv_No";
                    cDispCol = "Inv_No:Bill No.";
                    cRetCol = "Tran_cd,Inv_No";
                    break;

                case "CANCELBILL":
                    xcFldList = "";
                    xcDispCol = "";
                    xcRetCol = "";
                    cTbl = " DCMAIN ";
                    cCond = " Entry_Ty='" + CommonInfo.EntryTy + "' and Party_nm!='CANCELLED.' ";

                    cFldList = "Inv_No,Date";

                    cCond += " ";
                    cSql = "set dateformat dmy select Tran_cd,Inv_No,Date from DCMAIN where " + cCond + " order by Date ";
                    cFrmCap = "Select Bill to Cancel";
                    cSrchCol = "Inv_No";
                    cDispCol = "Inv_No:Bill No.,Date:Bill Date" + xcDispCol;
                    cRetCol = "Tran_cd" + xcRetCol;
                    break;

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

                case "MOBILENO":
                    cSql = "Select Distinct ResMobile, Name from pRetPatientMast union select nmobileno,patientnm from dcmain";

                    xcFldList = "";
                    xcDispCol = "";
                    xcRetCol = "";
                    cFrmCap = "Select Mobile No";
                    cSrchCol = "ResMobile";
                    cDispCol = "ResMobile:Mobile No,Name:Patient";
                    cRetCol = "ResMobile,Name";
                    break;
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
                    case "PARTY":
                        _commonDs.Tables["Main"].Rows[this.BindingContext[_commonDs, "Main"].Position]["Ac_Id"] = _oSelPop.pReturnArray[0].ToString();
                        _commonDs.Tables["Main"].Rows[this.BindingContext[_commonDs, "Main"].Position]["Party_Nm"] = _oSelPop.pReturnArray[1].ToString();

                        _commonDs.Tables["Main"].Rows[this.BindingContext[_commonDs, "Main"].Position]["Party_Nm"] = _oSelPop.pReturnArray[1].ToString();

                        clsInsertDefaultValue.replNewvalueinChldTable("Item", "Ac_Id", _oSelPop.pReturnArray[0].ToString());
                        clsInsertDefaultValue.replNewvalueinChldTable("Item", "Party_Nm", _oSelPop.pReturnArray[1].ToString());

                        if (Convert.ToInt64(CommonInfo.PartyId) != Convert.ToInt64(_oSelPop.pReturnArray[0]) && (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true || CommonInfo.HdrDiscFlds != ""))
                        {
                            clsInsertDefaultValue.GetPartyDefaDiscChrgsValue(Convert.ToInt64(_oSelPop.pReturnArray[0]));
                            replDefaDiscChrgsValues("PARTY", 0);
                        }
                        break;

                    case "WAREHOUSE":
                        this.txtWareHouse.Text = _oSelPop.pReturnArray[0].ToString();
                        clsInsertDefaultValue.replNewvalueinChldTable("Item", "Ware_Nm", _oSelPop.pReturnArray[0].ToString());
                        CommonInfo.ChngdWareHouse = _oSelPop.pReturnArray[0].ToString();
                        break;

                    case "ITEM":
                        int _nRowIndex = this.BindingContext[_commonDs, "Item"].Position;
                        _curRowNo = _nRowIndex;
                        string _clAddExist = "EXIST";
                        DataRow _drr = null;

                        decimal[] stkQty = Stock_Checking_New(Convert.ToInt64(_oSelPop.pReturnArray[0].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, this.dgvItemDetails.CurrentRow.Index);      // Added by Sachin N. S. on 07/02/2019 

                        //if (Stock_Checking(Convert.ToInt16(_oSelPop.pReturnArray[0].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, this.dgvItemDetails.CurrentRow.Index) < 0)
                        if (stkQty[0] < 0)
                        {
                            long _It_Code = Convert.ToInt64(_oSelPop.pReturnArray[0].ToString());        // Added by Sachin N. S. on 07/02/2019
                            decimal _nqty = Convert.ToDecimal(_commonDs.Tables["Item"].AsEnumerable().Where((row, index) => Convert.ToInt64(row["It_Code"]) == _It_Code && row["Qty"] != null && index != _nRowIndex).Sum(row => Convert.ToDecimal(row["Qty"])));       // Added by Sachin N. S. on 07/02/2019
                            MessageBox.Show("Balance Stock : " + (stkQty[1] - _nqty).ToString() + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (CommonInfo.EntryTy == "HS")
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _oSelPop.pReturnArray[0].ToString();
                                _drr = this.getBatchNo(_nRowIndex, ref _clAddExist);
                                if (_drr != null)
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = 0;
                            }
                            string clAdd = "Add";
                            if (_commonDs.Tables["Item"].Rows.Count > 0)
                            {
                                if (CommonInfo.EntryTy == "HS")
                                {
                                    if ((_drr != null && _drr.Table.Rows.Count > 0))
                                    {
                                        decimal _stkQty1 = Convert.ToDecimal(_drr["Qty"]);     //**** Added by Sachin N. S. on 07/02/2019
                                        _drr["Qty"] = Convert.ToDecimal(_drr["Qty"]) + 1;

                                        clAdd = "EXIST";
                                        _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_oSelPop.pReturnArray[0].ToString()) + "' and BatchNo='" + _drr["BatchNo"].ToString() + "'")[0]);
                                        _curRowNo = _nRowIndex;
                                        if (this.llgpuom == true)
                                        {
                                            decimal _colValue1 = Convert.ToDecimal(_drr["gpQty"]);      //**** Added by Sachin N. S. on 07/02/2019
                                            _drr["gpQty"] = Convert.ToDecimal(_drr["gpQty"]) + 1;
                                            this.Update_Conv_Qty("GPQTY", "GRP", _curRowNo);

                                            //**** Added by Sachin N. S. on 07/02/2019 for -- Start
                                            decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[_nRowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nRowIndex]["Qty"]), _nRowIndex);

                                            if (ostkQty[0] < 0)
                                            {
                                                string stkUnit = "";
                                                if (llgpuom == true)
                                                    stkUnit = _commonDs.Tables["Item"].Rows[_curRowNo]["StkUnit"].ToString();
                                                _commonDs.Tables["Item"].Rows[_curRowNo]["GpQty"] = Convert.ToDecimal(_colValue1);
                                                _commonDs.Tables["Item"].Rows[_curRowNo]["Qty"] = _stkQty1;
                                                MessageBox.Show("Balance Stock : " + (ostkQty[1] - _stkQty1).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            //**** Added by Sachin N. S. on 07/02/2019 for -- End
                                        }
                                    }
                                }
                                else
                                {
                                    var _currentRow = (from currentRw in _commonDs.Tables["Item"].AsEnumerable()
                                                       where currentRw.Field<decimal>("It_code") == Convert.ToDecimal(_oSelPop.pReturnArray[0].ToString())
                                                       select currentRw).FirstOrDefault();
                                    if ((_currentRow != null && _currentRow.Table.Rows.Count > 0) && _clAddExist == "EXIST")
                                    {
                                        decimal _stkQty1 = Convert.ToDecimal(_currentRow["Qty"]);     //**** Added by Sachin N. S. on 07/02/2019
                                        _currentRow["Qty"] = Convert.ToDecimal(_currentRow["Qty"]) + 1;
                                        clAdd = "EXIST";
                                        _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_oSelPop.pReturnArray[0].ToString()) + "'")[0]);
                                        _curRowNo = _nRowIndex;
                                        if (this.llgpuom == true)
                                        {
                                            decimal _colValue1 = Convert.ToDecimal(_drr["gpQty"]);      //**** Added by Sachin N. S. on 07/02/2019
                                            _currentRow["gpQty"] = Convert.ToDecimal(_currentRow["gpQty"]) + 1;
                                            this.Update_Conv_Qty("GPQTY", "GRP", _curRowNo);
                                            //**** Added by Sachin N. S. on 07/02/2019 for -- Start
                                            decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[_nRowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nRowIndex]["Qty"]), _nRowIndex);
                                            if (ostkQty[0] < 0)
                                            {
                                                string stkUnit = "";
                                                if (llgpuom == true)
                                                    stkUnit = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["StkUnit"].ToString();
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpQty"] = Convert.ToDecimal(_colValue1);
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = _stkQty1;
                                                MessageBox.Show("Balance Stock : " + (ostkQty[1] - _stkQty1).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            //**** Added by Sachin N. S. on 07/02/2019 for -- End
                                        }
                                    }
                                }
                            }

                            if (clAdd == "Add")
                            {
                                this.BindingContext[_commonDs, "Item"].Position = dgvItemDetails.CurrentCell.RowIndex;
                                _curRowNo = this.BindingContext[_commonDs, "Item"].Position;
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _oSelPop.pReturnArray[0].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _oSelPop.pReturnArray[1].ToString();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 1;

                                if (this.llgpuom == true)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpQty"] = 1;
                                }

                                string _prSalt = "";
                                int _prExpDays = 0;
                                string[] _arr = cRetCol.Split(',');
                                int _iRate = Array.IndexOf(_arr, "Rate");
                                int _iItRate = Array.IndexOf(_arr, "It_Rate");
                                int _iInclTx = Array.IndexOf(_arr, "InclSTTax");
                                int _iprSalt = Array.IndexOf(_arr, "prSalt");
                                int _iExpDays = Array.IndexOf(_arr, "prExpAlertDays");
                                if (_iprSalt >= 0)
                                    _prSalt = _oSelPop.pReturnArray[_iprSalt].ToString();
                                if (_iExpDays >= 0)
                                    _prExpDays = Convert.ToInt16(_oSelPop.pReturnArray[_iExpDays]);

                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_iItRate > 0 ? (Convert.ToDecimal(_oSelPop.pReturnArray[_iItRate]) > 0 ? _oSelPop.pReturnArray[_iItRate] : _oSelPop.pReturnArray[_iRate]) : _oSelPop.pReturnArray[_iRate]);
                                if (this.llgpuom == true)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = (_iItRate > 0 ? (Convert.ToDecimal(_oSelPop.pReturnArray[_iItRate]) > 0 ? _oSelPop.pReturnArray[_iItRate] : _oSelPop.pReturnArray[_iRate]) : _oSelPop.pReturnArray[_iRate]);
                                }

                                if (_commonDs.Tables.Contains("Lcode") == true && _commonDs.Tables["Item"].Columns.Contains("stkunit") == true && this.llgpuom == true)
                                {
                                    string _mRateUnit = _oSelPop.pReturnArray[2].ToString();
                                    string _mGrpUnit = _oSelPop.pReturnArray[3].ToString();
                                    cSql = "select Top 1 * from GroupUOM where GroupUom ='" + _mGrpUnit + "' and BaseUOM = '" + _mRateUnit + "' and isDefault=1";
                                    _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                                    if (_dt.Rows.Count > 0)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["stkunit"] = _mRateUnit;
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _dt.Rows[0]["SubUom"].ToString();

                                        this.Update_Conv_Qty("GRPUNT", "GRP", this.BindingContext[_commonDs, "Item"].Position);

                                        //**** Added by Sachin N. S. on 07/02/2019 for -- Start
                                        decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]), this.BindingContext[_commonDs, "Item"].Position);
                                        if (ostkQty[0] < 0)
                                        {
                                            string stkUnit = "";
                                            if (llgpuom == true)
                                                stkUnit = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["StkUnit"].ToString();
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpQty"] = 0;
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"] = 0;
                                            MessageBox.Show("Balance Stock : " + ostkQty[1].ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        //**** Added by Sachin N. S. on 07/02/2019 for -- End
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
                                    if (_dt.Rows.Count > 0)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = Convert.ToDecimal(_dt.Rows[0]["gpuomrate"]);
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["rate"] = Convert.ToDecimal(_dt.Rows[0]["gpuomrate"]) / Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]);
                                    }
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

                                if (this.llgpuom == true)
                                {
                                    if (CommonInfo.EntryTy == "HS")
                                    {
                                        if (_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"].ToString().Trim() != "")
                                        {
                                            string csql1 = "SELECT PrAppRate FROM Ac_mast WHERE Ac_Id = " + _commonDs.Tables["Main"].Rows[0]["Ac_id"].ToString();
                                            DataRow _dr = _oDataAccess.GetDataRow(csql1, null, 25);
                                            string cRateFld = "BatchRate";
                                            if (_dr != null)
                                            {
                                                switch (_dr[0].ToString().Trim())
                                                {
                                                    case "MRP":
                                                        cRateFld = "BatchRate";
                                                        break;
                                                    case "Purchase Rate":
                                                        cRateFld = "prate";
                                                        break;
                                                    case "Sales Rate":
                                                        cRateFld = "srate";
                                                        break;
                                                    case "PTR":
                                                        cRateFld = "ptr";
                                                        break;
                                                    case "PTS":
                                                        cRateFld = "pts";
                                                        break;
                                                    default:
                                                        cRateFld = "BatchRate";
                                                        break;
                                                }
                                            }

                                            DataTable searchTbl;
                                            string csql = "SELECT BatchNo, MfgDt, ExpDt, " + cRateFld + " as BatchRate, " + (_dr[0].ToString().Trim() == "MRP" ? "MRPInc_gst" : "Inc_gst") + " as Inc_gst  FROM pRetBatchDetails WHERE it_code = " +
                                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString() +
                                                    " and BatchonHold = 0 and BatchNo='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"].ToString().Trim() + "'";
                                            searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                                            if (searchTbl.Rows.Count > 0)
                                            {
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();
                                                if (llgpuom == true)
                                                {
                                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpRate"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                                }

                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                                            }
                                        }
                                    }
                                }

                                if (CommonInfo.EntryTy == "HS")
                                {
                                    DataTable searchTbl;
                                    if (_prSalt != "")
                                    {
                                        string csql = "Select Top 1 Schedule from pRetSalt_Master where [Salt] = '" + _prSalt.Trim() + "'";
                                        searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                                        if (searchTbl.Rows.Count > 0)
                                        {
                                            //Added by Priyanka B on 26122018 for Bug-32062 Start
                                            string cSql1 = "SELECT TOP 1 GSTIN FROM AC_MAST WHERE AC_ID=" + _commonDs.Tables["Main"].Rows[0]["Ac_Id"].ToString();
                                            _dt = _oDataAccess.GetDataTable(cSql1, null, 50);
                                            if (_dt.Rows[0][0].ToString() == "UNREGISTERED")
                                            {
                                                //Added by Priyanka B on 26122018 for Bug-32062 End
                                                if (searchTbl.Rows[0][0].ToString() != "")
                                                {
                                                    MessageBox.Show("You have selected a Scheduled Drug of Schedule : " + searchTbl.Rows[0][0].ToString().Trim(), CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["SchdDrug"] = true;
                                                }
                                            }                                            //Added by Priyanka B on 26122018 for Bug-32062
                                        }
                                    }
                                }

                                //if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true && _iInclTx > 0 && _commonDs.Tables["Item"].Columns.Contains("CGST_PER") == true)
                                //{
                                //    if (Convert.ToBoolean(_oSelPop.pReturnArray[_iInclTx]) == true)
                                //    {
                                //        decimal _npertnm = 0.00M, _OrgRate = 0.00M;
                                //        _npertnm = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["CGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["SGST_PER"]) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["IGST_PER"]);
                                //        _OrgRate = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                //        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_OrgRate * 100) / (100 + _npertnm);
                                //        if (this.llgpuom == true)
                                //        {
                                //            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = Math.Round((_OrgRate * 100) / (100 + _npertnm), 2);
                                //        }
                                //    }
                                //    //_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(_oSelPop.pReturnArray[_iInclTx]);
                                //}
                            }
                        }

                        itemwiseTotal(_nRowIndex, 0);
                        break;

                    case "SALESTAX":
                        if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true)
                        {
                            _commonDs.Tables["Main"].Rows[0]["Tax_Name"] = _oSelPop.pReturnArray[0].ToString();

                            if (Convert.ToDecimal(_oSelPop.pReturnArray[1]) > 0)
                            {
                                var _currentRow = (from currentRw in _commonDs.Tables["HdDiscChrgs"].AsEnumerable()
                                                   where currentRw.Field<string>("Code") == "S"
                                                   select currentRw).FirstOrDefault();
                                _currentRow["Head_nm"] = _oSelPop.pReturnArray[0].ToString();
                                _currentRow["Def_Pert"] = Convert.ToDecimal(_oSelPop.pReturnArray[1]);
                                this.CalculateHeaderTax(_currentRow["Fld_nm"].ToString(), 0, 1);
                                this.RefreshControls();
                            }
                        }
                        if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Item"]) == true)
                        {
                            _commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentRow.Index]["Tax_Name"] = _oSelPop.pReturnArray[0].ToString();
                            _commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentRow.Index]["TaxPer"] = Convert.ToDecimal(_oSelPop.pReturnArray[1]);
                        }
                        itemwiseTotal(dgvItemDetails.CurrentCell.RowIndex, dgvItemDetails.CurrentCell.ColumnIndex);
                        break;

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
                            this.Update_Conv_Qty("GRPUNT", "GRP", this.BindingContext[_commonDs, "Item"].Position);
                        }

                        if (lDispB4 == false)
                        {
                            cSql = " Select top 1 InclStTax from It_Mast Where It_code=" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString();
                            _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                            bool _iInclTx1 = Convert.ToBoolean(_dt.Rows[0]["InclStTax"]);
                            if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["I_Disc"]) == true && _iInclTx1 == true && _commonDs.Tables["Item"].Columns.Contains("CGST_PER") == true)
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = _iInclTx1;
                            }
                        }
                        itemwiseTotal(dgvItemDetails.CurrentCell.RowIndex, dgvItemDetails.CurrentCell.ColumnIndex);
                        break;

                    case "MOBILENO":
                        _commonDs.Tables["Main"].Rows[0]["NMOBILENO"] = _oSelPop.pReturnArray[0].ToString();
                        if (CommonInfo.EntryTy == "HS")
                            _commonDs.Tables["Main"].Rows[0]["PatientNm"] = _oSelPop.pReturnArray[1].ToString();
                        break;
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
            int _lRateLevel = -1;

            DataTable _dt1 = new DataTable();
            DataTable _dt = new DataTable();
            xcFldList = ",It_mast.Rate,It_mast.Tax_Name,It_Mast.InclSTTax ";
            xcDispCol = ",Rate:Goods Rate,Tax_Name:Tax Name,InclSTTax:Incl of GST";
            xcRetCol = ",Rate,Tax_Name,InclSTTax";
            cTbl = " It_Mast ";
            cCond = " It_Mast.It_code!=0 and It_mast.isService=0 ";

            //******* Checking for Rate in Item Rate master -- Start *******\\
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
            //******* Checking for Rate in Item Rat`e master -- End *******\\

            //******* Searching for Barcode value in BarcodeTran Table -- Start *******\\

            string cCondBrCd = string.Empty, cTblBrCd = string.Empty, xcFldListBrCd = string.Empty, xcDispColBrCd = string.Empty, xcRetColBrCd = string.Empty;
            string cCond1 = string.Empty;

            if (_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() != "")
            {
                cCondBrCd = " And ((BarCodeTran.Entry_ty='IM' and BarCodeTran." + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() + " = '" + _barCode.ToString() + "' and BarCodeTran.Tran_cd = It_Mast.It_Code ) )";
                cTblBrCd = ",BarcodeTran ";
                xcFldListBrCd = ",BarCodeTran." + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim();
                xcDispColBrCd = "," + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim() + ":Bar Code";
                xcRetColBrCd = "," + _commonDs.Tables["UDPOSSETTINGS"].Rows[0]["POSBarcode"].ToString().Trim();
            }

            if (_barCode.ToString() != string.Empty)
            {
                cCond1 = " And It_Mast.It_Name like '%" + _barCode.ToString() + "%' ";
            }
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

                            decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex);
                            if (ostkQty[0] < 0)
                            {
                                MessageBox.Show("Balance Stock : " + ostkQty[1].ToString() + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex);     // Added by Sachin N. S. on 07/02/2019 
                        //if (Stock_Checking(Convert.ToInt16(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex) < 0)
                        if (ostkQty[0] < 0)
                        {
                            MessageBox.Show("Balance Stock : " + ostkQty[1].ToString() + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clAdd = "NOTFOUND";
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _dt.Rows[0]["It_Code"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _dt.Rows[0]["It_name"].ToString();

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

                if (_bcTran != null)
                {
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
            }

            int _itCode = 0;
            if (_dt12 != null)
            {
                _itCode = Convert.ToUInt16(_dt12["It_Code"]);
            }

            xcFldList = ",It_mast.RateUnit,It_Mast.S_Unit,It_mast.Rate,It_Mast.InclSTTax,It_mast.AskQty,It_mast.AskRate,It_Mast.prSalt ";
            xcDispCol = ",Rate:Goods Rate,InclSTTax:Incl of GST";
            xcRetCol = ",RateUnit,S_Unit,Rate,InclSTTax,AskQty,AskRate,prSalt";
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
                    bool lSelect = true;
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

                        lSelect = false;
                        clAdd = "";
                        if (_oSelPop.pReturnArray != null)
                        {
                            _dtNew = _dt.Select("It_Code=" + _oSelPop.pReturnArray[0].ToString().Trim()).CopyToDataTable();
                            _dt = _dtNew;
                            lSelect = true;
                            clAdd = "ADD";
                        }
                    }

                    int _nRowIndex = this.BindingContext[_commonDs, "Item"].Position;
                    _curRowNo = _nRowIndex;
                    if (_commonDs.Tables["Item"].Rows.Count > 0 && lSelect)
                    {
                        string _cAddExist = "EXIST";
                        DataRow _drr = null;
                        if (CommonInfo.EntryTy == "HS")
                        {
                            if (_dt12 != null)
                            {
                                if (_dt12.Table.Columns.Contains("BatchNo") == true)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _dt.Rows[0]["It_Code"].ToString();
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _dt.Rows[0]["It_Name"].ToString();
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = _dt12["BatchNo"].ToString();
                                    _drr = this.getBatchNo(_nRowIndex, ref _cAddExist);
                                    if (_drr != null)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = 0;
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = "";
                                    }
                                }
                                else
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _dt.Rows[0]["It_Code"].ToString();
                                    _drr = this.getBatchNo(_nRowIndex, ref _cAddExist);
                                    if (_drr != null)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = 0;
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = "";
                                    }
                                }
                            }
                            else
                            {
                                if (_commonDs.Tables["Item"].Columns.Contains("BatchNo") == true)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _dt.Rows[0]["It_Code"].ToString();
                                    _drr = this.getBatchNo(_nRowIndex, ref _cAddExist);
                                    if (_drr != null)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = 0;
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = "";
                                    }
                                }
                            }

                            if (_drr != null && _drr.Table.Rows.Count > 0 && _cAddExist == "EXIST")
                            {
                                //_nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_dt.Rows[0]["It_Code"]) + "'")[0]);
                                _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_drr);
                                decimal nqty = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nRowIndex]["Qty"]);         // Added by Sachin N. S. on 07/02/2019 
                                decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex);      // Added by Sachin N. S. on 07/02/2019 
                                //if (Stock_Checking(Convert.ToInt16(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _curRowNo) < 0)
                                if (ostkQty[0] < 0)
                                {
                                    string stkUnit = "";
                                    if (llgpuom == true)
                                        stkUnit = _commonDs.Tables["Item"].Rows[_nRowIndex]["StkUnit"].ToString();
                                    MessageBox.Show("Balance Stock : " + (ostkQty[1] - nqty).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    clAdd = "NOTFOUND";
                                }
                                else
                                {
                                    decimal _qty1 = Convert.ToDecimal(_drr["Qty"]);
                                    _drr["Qty"] = Convert.ToDecimal(_drr["Qty"]) + 1;
                                    if (this.llgpuom == true)
                                    {
                                        decimal _gpqty1 = Convert.ToDecimal(_drr["gpQty"]);
                                        _drr["gpQty"] = Convert.ToDecimal(_drr["gpQty"]) + 1;
                                        this.Update_Conv_Qty("GPQTY", "GRP", _nRowIndex);
                                        //**** Added by Sachin N. S. on 07/02/2019 for -- Start
                                        ostkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[_nRowIndex]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nRowIndex]["Qty"]), _nRowIndex);        // Added by Sachin N. S. on 07/02/2019 
                                        if (ostkQty[0] < 0)
                                        {
                                            string stkUnit = "";
                                            if (llgpuom == true)
                                                stkUnit = _commonDs.Tables["Item"].Rows[_nRowIndex]["StkUnit"].ToString();
                                            _commonDs.Tables["Item"].Rows[_nRowIndex]["GpQty"] = _gpqty1;
                                            _commonDs.Tables["Item"].Rows[_nRowIndex]["Qty"] = _qty1;
                                            MessageBox.Show("Balance Stock : " + (ostkQty[1] - _qty1).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        //**** Added by Sachin N. S. on 07/02/2019 for -- End   
                                    }
                                    clAdd = "EXIST";
                                }
                                _curRowNo = _nRowIndex;
                            }
                            else
                            {
                                if (_drr == null && _cAddExist == "NOTFOUND")
                                {
                                    _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_dt.Rows[0]["It_Code"]) + "'")[0]);
                                    MessageBox.Show("Goods are either out of Stock/Expired/On Hold/not in Stores.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    clAdd = "NOTFOUND";
                                    _curRowNo = _nRowIndex;
                                }
                            }
                        }
                        else
                        {
                            var _currentRow = (from currentRw in _commonDs.Tables["Item"].AsEnumerable()
                                               where currentRw.Field<decimal>("It_code") == Convert.ToDecimal(_dt.Rows[0]["It_Code"])
                                               select currentRw).FirstOrDefault();
                            if (_currentRow != null && _currentRow.Table.Rows.Count > 0 && _cAddExist == "EXIST")
                            {
                                _nRowIndex = _commonDs.Tables["Item"].Rows.IndexOf(_commonDs.Tables["Item"].Select("It_Code='" + Convert.ToDecimal(_dt.Rows[0]["It_Code"]) + "'")[0]);
                                decimal nqty = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nRowIndex]["Qty"]);             // Added by Sachin N. S. on 07/02/2019 
                                decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex);     // Added by Sachin N. S. on 07/02/2019 
                                //if (Stock_Checking(Convert.ToInt16(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex) < 0)
                                if (ostkQty[0] < 0)
                                {
                                    string stkUnit = "";
                                    if (llgpuom == true)
                                        stkUnit = _commonDs.Tables["Item"].Rows[_nRowIndex]["StkUnit"].ToString();
                                    MessageBox.Show("Balance Stock : " + (ostkQty[1] - nqty).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    clAdd = "NOTFOUND";
                                }
                                else
                                {
                                    decimal _qty1 = Convert.ToDecimal(_currentRow["Qty"]);
                                    _currentRow["Qty"] = Convert.ToDecimal(_currentRow["Qty"]) + 1;
                                    if (this.llgpuom == true)
                                    {
                                        decimal _gpqty1 = Convert.ToDecimal(_currentRow["gpQty"]);
                                        _currentRow["gpQty"] = Convert.ToDecimal(_currentRow["gpQty"]) + 1;
                                        this.Update_Conv_Qty("GPQTY", "GRP", _curRowNo);
                                        //**** Added by Sachin N. S. on 07/02/2019 for -- Start
                                        ostkQty = Stock_Checking_New(Convert.ToInt64(_commonDs.Tables["Item"].Rows[_curRowNo]["It_code"]), Convert.ToDateTime(this.txtInvoiceDt.Value), Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_curRowNo]["Qty"]), _curRowNo);        // Added by Sachin N. S. on 07/02/2019 
                                        if (ostkQty[0] < 0)
                                        {
                                            string stkUnit = "";
                                            if (llgpuom == true)
                                                stkUnit = _commonDs.Tables["Item"].Rows[_curRowNo]["StkUnit"].ToString();
                                            _commonDs.Tables["Item"].Rows[_curRowNo]["GpQty"] = _gpqty1;
                                            _commonDs.Tables["Item"].Rows[_curRowNo]["Qty"] = _qty1;
                                            MessageBox.Show("Balance Stock : " + (ostkQty[1] - _qty1).ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        //**** Added by Sachin N. S. on 07/02/2019 for -- End   
                                    }
                                    clAdd = "EXIST";
                                }
                                _curRowNo = _nRowIndex;
                            }
                        }
                    }

                    if (clAdd == "ADD" && lSelect)
                    {
                        decimal[] ostkQty = Stock_Checking_New(Convert.ToInt64(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex);     // Added by Sachin N. S. on 07/02/2019 
                        //if (Stock_Checking(Convert.ToInt16(_dt.Rows[0]["It_Code"].ToString()), Convert.ToDateTime(this.txtInvoiceDt.Value), 1, _nRowIndex) < 0)
                        if (ostkQty[0] < 0)
                        {
                            string stkUnit = "";
                            if (llgpuom == true)
                                stkUnit = _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["StkUnit"].ToString();
                            MessageBox.Show("Balance Stock : " + ostkQty[1].ToString() + (stkUnit != "" ? (" " + stkUnit) : "") + ".", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clAdd = "NOTFOUND";
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"] = _dt.Rows[0]["It_Code"].ToString();
                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Item"] = _dt.Rows[0]["It_name"].ToString();

                            if (_dt12 != null)
                            {
                                if (_dt12.ItemArray.Contains("BatchNo") == true)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"] = _dt12["BatchNo"].ToString();
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = _dt12["MfgDt"].ToString();
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = _dt12["ExpDt"].ToString();
                                }
                            }

                            string _prSalt = "";
                            string[] _arr = cRetCol.Split(',');
                            int _iRate = Array.IndexOf(_arr, "Rate");
                            int _iItRate = Array.IndexOf(_arr, "It_Rate");
                            int _iInclTx = Array.IndexOf(_arr, "InclSTTax");
                            int _iprSalt = Array.IndexOf(_arr, "prSalt");
                            if (_iprSalt >= 0)
                                _prSalt = _dt.Rows[0]["prSalt"].ToString().Trim();

                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = (_iItRate > 0 ? (Convert.ToInt16(_dt.Rows[0]["It_Rate"]) > 0 ? _dt.Rows[0]["It_Rate"] : _dt.Rows[0]["Rate"]) : _dt.Rows[0]["Rate"]);
                            if (this.llgpuom == true)
                            {
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpRate"] = (_iItRate > 0 ? (Convert.ToInt16(_dt.Rows[0]["It_Rate"]) > 0 ? _dt.Rows[0]["It_Rate"] : _dt.Rows[0]["Rate"]) : _dt.Rows[0]["Rate"]);
                            }

                            DataTable _dtt1;
                            if (_commonDs.Tables.Contains("Lcode") == true && _commonDs.Tables["Item"].Columns.Contains("stkunit") == true)
                            {
                                //,RateUnit,S_Unit,Rate,InclSTTax,AskQty,AskRate
                                string _mRateUnit = _dt.Rows[0]["RateUnit"].ToString();
                                string _mGrpUnit = _dt.Rows[0]["S_Unit"].ToString();
                                cSql = "select Top 1 * from GroupUOM where GroupUom ='" + _mGrpUnit + "' and BaseUOM = '" + _mRateUnit + "' and isDefault=1";
                                _dtt1 = _oDataAccess.GetDataTable(cSql, null, 50);
                                if (_dtt1.Rows.Count > 0)
                                {
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["stkunit"] = _mRateUnit;
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["gpunit"] = _dtt1.Rows[0]["SubUom"].ToString();

                                    this.Update_Conv_Qty("GRPUNT", "GRP", this.BindingContext[_commonDs, "Item"].Position);
                                }
                                else
                                {
                                    cSql = "select Top 1 RateUnit, p_unit, s_unit from it_mast where It_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_code"].ToString();
                                    _dtt1 = _oDataAccess.GetDataTable(cSql, null, 50);
                                    if (_dtt1.Rows.Count > 0)
                                    {
                                        _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["stkunit"] = _dtt1.Rows[0]["rateunit"].ToString();
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

                            if (this.llgpuom == true)
                            {
                                if (CommonInfo.EntryTy == "HS")
                                {
                                    if (_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"].ToString().Trim() != "")
                                    {
                                        string csql1 = "SELECT PrAppRate FROM Ac_mast WHERE Ac_Id = " + _commonDs.Tables["Main"].Rows[0]["Ac_id"].ToString();
                                        DataRow _dr = _oDataAccess.GetDataRow(csql1, null, 25);
                                        string cRateFld = "BatchRate";
                                        if (_dr != null)
                                        {
                                            switch (_dr[0].ToString().Trim())
                                            {
                                                case "MRP":
                                                    cRateFld = "BatchRate";
                                                    break;
                                                case "Purchase Rate":
                                                    cRateFld = "prate";
                                                    break;
                                                case "Sales Rate":
                                                    cRateFld = "srate";
                                                    break;
                                                case "PTR":
                                                    cRateFld = "ptr";
                                                    break;
                                                case "PTS":
                                                    cRateFld = "pts";
                                                    break;
                                                default:
                                                    cRateFld = "BatchRate";
                                                    break;
                                            }
                                        }

                                        DataTable searchTbl;
                                        string csql = "SELECT BatchNo, MfgDt, ExpDt, " + cRateFld + " as BatchRate, " + (_dr[0].ToString().Trim() == "MRP" ? "MRPInc_gst" : "Inc_gst") + " as Inc_gst  FROM pRetBatchDetails WHERE it_code = " +
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString() +
                                            " and BatchonHold = 0 and BatchNo='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["BatchNo"].ToString().Trim() + "'";
                                        searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                                        if (searchTbl.Rows.Count > 0)
                                        {
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();
                                            if (llgpuom == true)
                                            {
                                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GpRate"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Qty"]) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"]);
                                            }

                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                                        }
                                    }
                                }
                            }

                            if (CommonInfo.EntryTy == "HS")
                            {
                                DataTable searchTbl;
                                if (_prSalt != "")
                                {
                                    string csql = "Select Top 1 Schedule from pRetSalt_Master where [Salt] = '" + _prSalt.Trim() + "'";
                                    searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                                    if (searchTbl.Rows.Count > 0)
                                    {
                                        if (searchTbl.Rows[0][0].ToString() != "")
                                        {
                                            MessageBox.Show("You have selected a Scheduled Drug of Schedule : " + searchTbl.Rows[0][0].ToString().Trim(), CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["SchdDrug"] = true;
                                        }
                                    }
                                }
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
                cSql += " and [Rule] = '" + _commonDs.Tables["Main"].Rows[0]["Rule"].ToString() + "'";
                cSql += " and [Ware_nm] = '" + _commonDs.Tables["Item"].Rows[_rowIndex]["Ware_nm"].ToString().Trim() + "'";

                DataTable _dt;
                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                decimal.TryParse(_dt.Rows[0]["Qty"].ToString(), out StkQty);
                StkQty = StkQty - Convert.ToDecimal(_commonDs.Tables["Item"].AsEnumerable().Where((row, index) => Convert.ToInt64(row["It_Code"]) == _It_Code && row["Qty"] != null && index != _rowIndex).Sum(row => Convert.ToDecimal(row["Qty"]))) - _CurstkVal;

                if (CommonInfo.EntryTy == "HS")
                {
                    if (StkQty > 0)
                    {
                        cSql = "Set DateFormat DMY;";
                        cSql += "Select Qty From pRetBatchDetails where it_code = " + _It_Code.ToString() + " and BatchNo='" + _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim() + "' and BatchOnHold=0 and StoreNm='" + _commonDs.Tables["Item"].Rows[_rowIndex]["Ware_nm"].ToString().Trim() + "'  ";
                        _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                        if (_dt != null && _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim() != "")
                        {
                            if (_dt.Rows.Count > 0)
                            {
                                string _cBatchno = _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim();
                                decimal.TryParse(_dt.Rows[0]["Qty"].ToString(), out StkQty);
                                StkQty = StkQty - Convert.ToDecimal(_commonDs.Tables["Item"].AsEnumerable().Where((row, index) => Convert.ToInt64(row["It_Code"]) == _It_Code && row["BatchNo"].ToString().Trim() == _cBatchno && row["Qty"] != null && index != _rowIndex).Sum(row => Convert.ToDecimal(row["Qty"]))) - _CurstkVal;
                            }
                            else
                                StkQty = 0;
                        }
                    }
                }
            }
            else
            {
                StkQty = 1;
            }
            return StkQty;
        }

        private decimal[] Stock_Checking_New(long _It_Code, DateTime _InvDt, decimal _CurstkVal, int _rowIndex)
        {
            decimal[] StkQty = new decimal[2];

            if (CommonInfo.Neg_itBal == 0)
            {
                string cSql;
                cSql = "Set dateformat DMY;";
                cSql += " Select SUM(case when b.inv_stk='+' then a.Qty else case when b.Inv_stk='-' then -Qty else 0 end end ) as Qty From It_balw A,Lcode B where ";
                cSql += " a.It_code = " + _It_Code.ToString();
                cSql += " and a.Date <= '" + _InvDt.ToString() + "'";
                cSql += " and a.Entry_ty = b.Entry_ty and b.Inv_stk!='' ";
                cSql += " and [Rule] = '" + _commonDs.Tables["Main"].Rows[0]["Rule"].ToString() + "'";
                cSql += " and [Ware_nm] = '" + _commonDs.Tables["Item"].Rows[_rowIndex]["Ware_nm"].ToString().Trim() + "'";

                DataTable _dt;
                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                decimal.TryParse(_dt.Rows[0]["Qty"].ToString(), out StkQty[1]);
                StkQty[0] = StkQty[1] - Convert.ToDecimal(_commonDs.Tables["Item"].AsEnumerable().Where((row, index) => Convert.ToInt64(row["It_Code"]) == _It_Code && row["Qty"] != null && index != _rowIndex).Sum(row => Convert.ToDecimal(row["Qty"]))) - _CurstkVal;

                if (CommonInfo.EntryTy == "HS")
                {
                    if (StkQty[1] > 0)
                    {
                        cSql = "Set DateFormat DMY;";
                        cSql += "Select Qty From pRetBatchDetails where it_code = " + _It_Code.ToString() + " and BatchNo='" + _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim() + "' and BatchOnHold=0 and StoreNm='" + _commonDs.Tables["Item"].Rows[_rowIndex]["Ware_nm"].ToString().Trim() + "'  ";
                        _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                        if (_dt != null && _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim() != "")
                        {
                            if (_dt.Rows.Count > 0)
                            {
                                string _cBatchno = _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim();
                                decimal.TryParse(_dt.Rows[0]["Qty"].ToString(), out StkQty[1]);
                                StkQty[0] = StkQty[1] - Convert.ToDecimal(_commonDs.Tables["Item"].AsEnumerable().Where((row, index) => Convert.ToInt64(row["It_Code"]) == _It_Code && row["BatchNo"].ToString().Trim() == _cBatchno && row["Qty"] != null && index != _rowIndex).Sum(row => Convert.ToDecimal(row["Qty"]))) - _CurstkVal;
                            }
                            else
                            {
                                StkQty[0] = 0;
                                StkQty[1] = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                StkQty[0] = 1;
                StkQty[1] = 1;
            }
            return StkQty;
        }

        #endregion Stock Checking

        #region Calculate Headerwise Taxes
        private void CalculateHeaderTax(string _HeadFld, int _rowIndex, int _colIndex)
        {
            bool _calcTax = false;
            decimal mcalamt = 0, mpert = 0;
            string mfld_nm = "", mper_nm = "";

            _commonDs.Tables["Main"].Rows[0]["Tot_Deduc"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_tax"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_examt"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_add"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_nontax"] = 0;
            _commonDs.Tables["Main"].Rows[0]["tot_fdisc"] = 0;

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

                    mfld_nm = _dr["fld_nm"].ToString().Trim();
                    if (_calcTax)
                    {
                        mper_nm = _dr["pert_name"].ToString().Trim();
                        mpert = Convert.ToDecimal(_dr["Def_Pert"]);
                        if (mper_nm != "" && _commonDs.Tables["Main"].Rows[0][mper_nm] != null && Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0][mper_nm]) != 0)
                        {
                            mpert = Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0][mper_nm]);
                        }

                        decimal mamt = 0;
                        if (_colIndex == 1 && mpert == 0)
                            _dr["Def_Amt"] = 0;

                        mamt = Convert.ToDecimal(_dr["Def_Amt"]);
                        if (mpert > 0)
                        {
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
                        }
                        _commonDs.Tables["Main"].Rows[0][mfld_nm] = Convert.ToDecimal(mamt);
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

        #region Add - Edit - Delete Methods
        private void addHdrRecords()
        {
            DataRow _dr;
            this.txtWareHouse.Text = CommonInfo.WareHouse;
            _dr = clsInsertDefaultValue.AddNewRow(_commonDs.Tables["Main"]);
            _commonDs.Tables["Main"].Rows.Add(_dr);
            clsInsertDefaultValue.InsertDefVal_Main(0);
            if (_commonDs.Tables["UDPOSSETTINGS"].Rows.Count > 0)
            {
                if (Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["V_Disc"]) == true || Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["Stax_Tran"]) == true)
                {
                    //Added by Shrikant S. on 28/12/2018 for Installer 2.1.0        //Start
                    DataRow[] _tmpdcmastRows = _commonDs.Tables["DCMast"].Select("Att_file=true");
                    if (_tmpdcmastRows.Count() > 0)
                    {
                        foreach (var item in _tmpdcmastRows)
                        {
                            DataRow[] _hdrrow = _commonDs.Tables["HdDiscChrgs"].Select("Att_file=true and trim(fld_nm)='" + item["Fld_nm"].ToString().Trim() + "'");
                            if (_hdrrow.Length == 1)
                            {
                                _hdrrow[0]["Def_Pert"] = item["Def_Pert"];
                                _hdrrow[0]["Def_Amt"] = item["Def_Amt"];
                            }
                        }
                    }
                    //Added by Shrikant S. on 28/12/2018 for Installer 2.1.0        //End
                    replDefaDiscChrgsValues("PARTY", 0);
                }
            }
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

                if (cTblName == "Item")
                {
                    maxRowId = maxRowId - 1;
                    _dr["ItemRowId"] = maxRowId;
                }

                _commonDs.Tables[cTblName].Rows.Add(_dr);
                this.BindingContext[_commonDs, cTblName].Position = _commonDs.Tables["Item"].Rows.Count;
                clsInsertDefaultValue.InsertDefVal_Item(this.BindingContext[_commonDs, cTblName].Position);
                if (dgvItemDetails.CurrentRow.Index < this.dgvItemDetails.Rows.Count && dgvItemDetails.Rows.Count > 1)
                {
                    ldntValidate = true;
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

        #region Replace Default Discount & Charges or Sales Tax Value
        private void replDefaDiscChrgsValues(string _type, int _RateLevel)
        {
            DataRow _dr1;
            string cSql = "";
            if (_type == "PARTY")
            {
                if (_commonDs.Tables.Contains("HdDiscChrgs") == true)
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

                _dt = clsInsertDefaultValue.GetItemGstRateByState(Convert.ToInt64(_commonDs.Tables["Main"].Rows[0]["Ac_id"]), Convert.ToInt64(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"]), Convert.ToDateTime(_commonDs.Tables["Main"].Rows[0]["Date"]), this.BindingContext[_commonDs, "Item"].Position);
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

                _dt = clsInsertDefaultValue.GetItemDefaDiscChrgsValue(Convert.ToInt64(_commonDs.Tables["Main"].Rows[0]["Ac_id"]), Convert.ToInt64(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"]), Convert.ToDecimal(_RateLevel));

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

        #endregion Add - Edit - Delete Methods

        #region Generate Process Id's
        private void mInsertProcessIdRecord()
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
        private void mDeleteProcessIdRecord()
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
            this.pnlPrintBill.Visible = true;
            this.lblPrintBill.Visible = true;
            this.pgrsBar.Visible = true;
            this.lblPrintBill.Text = "Printing Bill......";
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

            string vRepGroup = _commonDs.Tables["Lcode"].Rows[0]["code_nm"].ToString();
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
            oPrint.pSpPara = " 'A.Entry_ty=''" + CommonInfo.EntryTy + "'' AND A.TRAN_CD = " + _tran_cd.ToString() + "'";
            //oPrint.pSpPara = _tran_cd.ToString();
            oPrint.pPrintOption = 3;
            oPrint.Main();

            this.pnlPrintBill.Visible = false;
            this.lblPrintBill.Visible = false;
            this.pgrsBar.Visible = false;
            this.Refresh();
        }
        #endregion

        #region Method to Hide/Unhide the Controls
        private void hideUnhideControls()
        {
            if (Convert.ToBoolean(_commonDs.Tables["Lcode"].Rows[0]["V_Extra"]) == false)
            {
                this.btnAddInfo.Visible = false;
            }
            this.pnlPrintBill.Visible = false;
            this.lblPrintBill.Visible = false;
            this.pgrsBar.Visible = false;
        }
        #endregion Method to Hide/Unhide the Controls

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

        private void CancelEntry()
        {
            popUpFromTable("CANCELBILL");
            maxRowId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_colNm"></param>
        /// <param name="_cGrpFree"></param>
        /// <param name="_nPosition"></param>
        private void Update_Conv_Qty(string _colNm, string _cGrpFree, int _nPosition)
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
                _commonDs.Tables["Item"].Rows[_nPosition]["LooseQty"] = _mBalQty;
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
                        if (_cGrpFree == "FREE")
                        {
                            _commonDs.Tables["Item"].Rows[_nPosition]["FreeQty"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["pmgrpfrqty"]);
                        }
                        else
                        {
                            _commonDs.Tables["Item"].Rows[_nPosition]["gpqty"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["qty"]);
                            _commonDs.Tables["Item"].Rows[_nPosition]["LooseQty"] = 0M;
                        }
                    }
                    else
                    {
                        if (_mratio > 0)
                        {
                            int _remd;
                            int _quot = 0;
                            if (_cGrpFree == "FREE")
                            {
                                _quot = Math.DivRem(Convert.ToInt16(_commonDs.Tables["Item"].Rows[_nPosition]["FreeQty"]), Convert.ToInt16(_mratio), out _remd);
                                _commonDs.Tables["Item"].Rows[_nPosition]["pmgrpfrqty"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["FreeQty"]);
                            }
                            else
                            {
                                _quot = Math.DivRem(Convert.ToInt16(_commonDs.Tables["Item"].Rows[_nPosition]["qty"]), Convert.ToInt16(_mratio), out _remd);
                                _commonDs.Tables["Item"].Rows[_nPosition]["gpQty"] = _quot;
                                _commonDs.Tables["Item"].Rows[_nPosition]["LooseQty"] = _remd;
                            }
                        }
                    }
                }
                else
                {
                    if (_cGrpFree == "FREE")
                    {
                        //_commonDs.Tables["Item"].Rows[_nPosition]["FreeQty"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Pmgrpfrqty"]) * _mratio;      //Commented by Shrikant S. on 29/12/2018 for Installer 2.1.0
                        _commonDs.Tables["Item"].Rows[_nPosition]["FreeQty"] = Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Pmgrpfrqty"]);        //Added by Shrikant S. on 29/12/2018 for Installer 2.1.0

                        if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Qty"]) > 0)
                        {
                            if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Rate"]) == 0)
                                _commonDs.Tables["Item"].Rows[_nPosition]["Rate"] = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gprate"]) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gpqty"])) / Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Qty"]);
                        }
                    }
                    else
                    {
                        //decimal _nconvQty = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gpqty"]) * _mratio) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Looseqty"]);
                        //decimal _balStock = this.getBalStock(Convert.ToInt16(_commonDs.Tables["Item"].Rows[_nPosition]["It_code"]), _nPosition);

                        _commonDs.Tables["Item"].Rows[_nPosition]["Qty"] = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gpqty"]) * _mratio) + Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Looseqty"]);
                        if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Qty"]) > 0)
                        {
                            if (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Rate"]) == 0)
                                _commonDs.Tables["Item"].Rows[_nPosition]["Rate"] = (Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gprate"]) * Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["gpqty"])) / Convert.ToDecimal(_commonDs.Tables["Item"].Rows[_nPosition]["Qty"]);
                        }
                    }
                }
            }
        }

        private decimal getBalStock(int _It_Code, int _rowIndex)
        {
            decimal StkQty = 0M;
            if (CommonInfo.EntryTy == "HS")
            {
                string cSql = "";
                DataTable _dt;
                cSql = "Set DateFormat DMY;";
                cSql += "Select Qty From pRetBatchDetails where it_code = " + _It_Code.ToString() + " and BatchNo='" + _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim() + "' and BatchOnHold=0 and StoreNm='" + _commonDs.Tables["Item"].Rows[_rowIndex]["Ware_nm"].ToString().Trim() + "'  ";
                _dt = _oDataAccess.GetDataTable(cSql, null, 50);
                if (_dt != null && _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim() != "")
                {
                    if (_dt.Rows.Count > 0)
                    {
                        string _cBatchno = _commonDs.Tables["Item"].Rows[_rowIndex]["BatchNo"].ToString().Trim();
                        decimal.TryParse(_dt.Rows[0]["Qty"].ToString(), out StkQty);
                    }
                    else
                        StkQty = 0;
                }
            }
            return StkQty;
        }

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

            sqlQuery = "select * from CSV_Export where entry_ty='" + CommonInfo.EntryTy + "'";
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

                        sqlQuery = "update CSV_Export set CSV_Path='" + DBfilePath + "' where [File_Name]='" + file_name + "' and entry_ty='" + CommonInfo.EntryTy + "'";

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
            if (_commonDs.Tables["Item"].Rows != null)
            {
                if (_commonDs.Tables["Item"].Rows.Count > 0)
                {
                    frmLastDeal lstDeal = new frmLastDeal();
                    lstDeal.oDataAccess = _oDataAccess;
                    lstDeal.EntryType = _commonDs.Tables["Main"].Rows[0]["Entry_ty"].ToString();
                    lstDeal.Itemcode = Convert.ToInt32(_commonDs.Tables["Item"].Rows[dgvItemDetails.CurrentCell.RowIndex]["It_code"]);
                    lstDeal.AcId = Convert.ToInt32(_commonDs.Tables["Main"].Rows[0]["Ac_id"]);
                    lstDeal.PatientNm = _commonDs.Tables["Main"].Rows[0]["PatientNm"].ToString();
                    lstDeal.Width = this.Width - 200;
                    lstDeal.Top = (this.Height - lstDeal.Height) / 2;
                    lstDeal.Left = (this.Width - lstDeal.Width) / 2;
                    lstDeal.Icon = this.Icon;
                    lstDeal.ShowDialog();
                }
            }
        }

        private DataRow getBatchNo(int _nRowNo, ref string _caddExist)
        {
            string _cRet = "ADD";
            DataRow _currentRow1 = null;
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
                        csql = "Set DateFormat DMY; " + csql;
                        searchTbl = _oDataAccess.GetDataTable(csql, null, 25);

                        long _itcode = 0;
                        string _bathcNo = "";
                        decimal _qty = 0;
                        DataTable _dtBatch = searchTbl;
                        foreach (DataRow _dr in _dtBatch.Rows)
                        {
                            _itcode = Convert.ToInt64(_commonDs.Tables["Item"].Rows[_nRowNo]["It_code"]);
                            _bathcNo = _dr["BatchNo"].ToString();

                            _qty = _commonDs.Tables["Item"].AsEnumerable().Where(row => row["Qty"] != null && Convert.ToInt64(row["It_code"]) == _itcode && row["BatchNo"].ToString() == _bathcNo).Sum(row => Convert.ToDecimal(row["Qty"]));
                            _dr["Qty"] = Convert.ToDecimal(_dr["Qty"]) - _qty;
                            if (Convert.ToDecimal(_dr["Qty"]) > 0)
                                searchTbl.Rows[searchTbl.Rows.IndexOf(_dr)]["Qty"] = _dr["Qty"];
                            else
                                searchTbl.Rows[searchTbl.Rows.IndexOf(_dr)].Delete();
                        }
                        searchTbl.AcceptChanges();

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
                                //_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm] = searchTbl.Rows[0][0].ToString();
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
                            //var _currentRow = (from currentRw in _commonDs.Tables["Item"].AsEnumerable()
                            //                   where currentRw.Field<decimal>("It_code") == Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"])
                            //                    && currentRw.Field<string>("BatchNo") == returnArray[0].ToString()
                            //                   select currentRw).FirstOrDefault();
                            //int _nidx = _commonDs.Tables["Item"].Rows.IndexOf(_currentRow);

                            var _currentRow = (from currentRw in _commonDs.Tables["Item"].AsEnumerable()
                                               where currentRw.Field<decimal>("It_code") == Convert.ToDecimal(_commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"])
                                                && currentRw.Field<string>("BatchNo") == returnArray[0].ToString()
                                               select currentRw).FirstOrDefault();
                            if (_currentRow != null && _currentRow.Table.Rows.Count > 0)
                            {
                                _cRet = "EXIST";
                                _caddExist = "EXIST";
                                _currentRow1 = _currentRow;
                            }
                            else
                            {
                                string fldnm = column.Fldnm.Trim();
                                _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm] = returnArray[0].ToString();

                                string csql1 = "SELECT PrAppRate FROM Ac_mast WHERE Ac_Id = " + _commonDs.Tables["Main"].Rows[0]["Ac_id"].ToString();
                                DataRow _dr = _oDataAccess.GetDataRow(csql1, null, 25);
                                string cRateFld = "BatchRate";
                                if (_dr != null)
                                {
                                    switch (_dr[0].ToString().Trim())
                                    {
                                        case "MRP":
                                            cRateFld = "BatchRate";
                                            break;
                                        case "Purchase Rate":
                                            cRateFld = "prate";
                                            break;
                                        case "Sales Rate":
                                            cRateFld = "srate";
                                            break;
                                        case "PTR":
                                            cRateFld = "ptr";
                                            break;
                                        case "PTS":
                                            cRateFld = "pts";
                                            break;
                                        default:
                                            cRateFld = "BatchRate";
                                            break;
                                    }
                                }

                                csql = " SELECT b.BatchNo, b.MfgDt, b.ExpDt, b." + cRateFld + " as BatchRate, " + (_dr[0].ToString().Trim() == "MRP" ? "b.MRPInc_gst" : "b.Inc_gst") + " as Inc_gst, " +
                                        " (Case When a.prExpAlertDays>0 Then DATEADD(DAY,a.PREXPALERTDAYS,CAST('" + _commonDs.Tables["Main"].Rows[0]["Date"].ToString() + "' AS SMALLDATETIME)) Else cast('01-01-1900' as smalldatetime) End) as ExpDt1  " +
                                        " FROM It_mast a Inner join pRetBatchDetails b on a.it_code=b.it_code " +
                                        " WHERE a.it_code = " + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["It_Code"].ToString() + " and b.BatchonHold = 0 and b.BatchNo='" + _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position][fldnm].ToString().Trim() + "' ";
                                searchTbl = _oDataAccess.GetDataTable(csql, null, 25);
                                if (searchTbl.Rows.Count > 0)
                                {
                                    double _iExpdays = (Convert.ToDateTime(searchTbl.Rows[0]["ExpDt"]) >= Convert.ToDateTime(searchTbl.Rows[0]["ExpDt1"]) ? 0 : (Convert.ToDateTime(searchTbl.Rows[0]["ExpDt"]) >= Convert.ToDateTime(_commonDs.Tables["Main"].Rows[0]["Date"]) ? (Convert.ToDateTime(searchTbl.Rows[0]["ExpDt"]) - Convert.ToDateTime(_commonDs.Tables["Main"].Rows[0]["Date"])).TotalDays : -1));
                                    if (_iExpdays != 0)
                                    {
                                        MessageBox.Show("The selected Goods is " + (_iExpdays < 0 ? "Expired." : " expiring within " + (_iExpdays == 1 ? " 1 Day" : _iExpdays.ToString() + " Days.")), CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["MfgDt"] = searchTbl.Rows[0]["MfgDt"].ToString();
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["ExpDt"] = searchTbl.Rows[0]["ExpDt"].ToString();
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["Rate"] = searchTbl.Rows[0]["BatchRate"].ToString();
                                    _commonDs.Tables["Item"].Rows[this.BindingContext[_commonDs, "Item"].Position]["GSTIncl"] = Convert.ToBoolean(searchTbl.Rows[0]["Inc_Gst"]);
                                    itemwiseTotal(this.BindingContext[_commonDs, "Item"].Position, 0);
                                }
                                _caddExist = "ADD";
                            }
                        }
                        else
                            _caddExist = "NOTFOUND";
                    }
                }
            }
            return _currentRow1;
        }

        private void cmdMobileNo_Click(object sender, EventArgs e)
        {
            this.getMobileDetail("", "F2");
        }

        private void btnUpldPresc_Click(object sender, EventArgs e)
        {
            string cSql = "";
            cSql = "select Top 1 * from fileuploadmaster where E_Code = '" + CommonInfo.EntryTy + "' and Att_file=1 ";
            DataRow _dr = _oDataAccess.GetDataRow(cSql, null, 25);
            if (_dr == null)
            {
                MessageBox.Show("No File Upload master is created.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OpenFileDialog _fileDialog = new OpenFileDialog();
            _fileDialog.Title = "Upload Prescription";
            _fileDialog.DefaultExt = "docx";
            _fileDialog.Filter = "Doc Files (*.docx)|*.docx|Excel Files (*.xls)|*.xls|BMP Files (*.bmp)|*.bmp|Jpeg Files (*.jpg)|*.jpg|gif Files (*.gif)|*.gif|Png Files (*.png)|*.png|Tiff Files (*.tif)|*.tif|Ppt Files (*.ppt)|*.ppt|Pps Files (*.pps)|*.pps|PDF Files (*.pdf)|*.pdf|Text Files (*.txt)|*.txt";
            _fileDialog.ShowDialog();

            string _fileName = _fileDialog.FileName;

            if (_commonDs.Tables["UploadFiles"].Rows.Count == 0)
                _commonDs.Tables["UploadFiles"].Rows.Add();

            if (_commonDs.Tables["UploadFiles"].Rows.Count > 0)
            {
                _commonDs.Tables["UploadFiles"].Rows[0]["tr_type"] = _commonDs.Tables["Lcode"].Rows[0]["Bcode_nm"].ToString();
                _commonDs.Tables["UploadFiles"].Rows[0]["tr_id"] = _commonDs.Tables["Main"].Rows[0]["Tran_cd"];
                _commonDs.Tables["UploadFiles"].Rows[0]["tr_serial"] = Convert.ToInt16(_dr["Serial"]);
                _commonDs.Tables["UploadFiles"].Rows[0]["tr_itserial"] = "";
                _commonDs.Tables["UploadFiles"].Rows[0]["filename"] = Path.GetFileNameWithoutExtension(_fileName).ToUpper();
                _commonDs.Tables["UploadFiles"].Rows[0]["Extension"] = Path.GetExtension(_fileName).Replace(".", "").ToUpper();
                _commonDs.Tables["UploadFiles"].Rows[0]["ObjPath"] = _fileName.ToUpper();
            }
        }

        private void txtMobileNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
                this.cmdMobileNo.PerformClick();
        }

        #region Form and Controls Resizing and arranging
        private void ResizeFormNew(Control _ctrl)
        {
            //this.pnlWareHouse.Left = this.pnlCompImg.Left - this.pnlWareHouse.Width;
            //this.cmdParty.Left = this.pnlWareHouse.Left - this.cmdParty.Width;
            this.txtParty.Width = this.cmdParty.Left - this.txtParty.Left;
            this.pnlHeader.Height = this.tblXtraInfo.Top + this.tblXtraInfo.Height + 1;
            this.pnlHotKeys.Top = this.pnlHeader.Top + this.pnlHeader.Height + 2;
            this.pnlItemDetails.Height = this.pnlItmtotal.Top - this.pnlHotKeys.Height - this.pnlHotKeys.Top - 4;
            this.pnlItemDetails.Top = this.pnlHotKeys.Top + this.pnlHotKeys.Height + 2;
            this.pnlCompImg.Height = this.pnlHeader.Height;
        }
        #endregion Form and Controls Resizing and arranging

        private void txtMobileNo_Validating(object sender, CancelEventArgs e)
        {
            this.getMobileDetail(((TextBox)sender).Text.ToString().Trim(), "TXT");
        }

        private void ExitForm()
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
            else
            {
                if (MessageBox.Show("You are about to close the POS Screen.\nDo you want still want to Exit?", CommonInfo.AppCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    this.ldntValidate = false;
                    return;
                }
            }
            this.Close();
        }

        private void udFrmPointofSaleNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ldntValidate == false)
            {
                if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count(row => row["Item"].ToString().Trim() != string.Empty) > 0)
                {
                    if (MessageBox.Show("The data will not be saved if clicked on Exit.\nDo you want still want to Exit?", CommonInfo.AppCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    if (MessageBox.Show("You are about to close the POS Screen.\nDo you want still want to Exit?", CommonInfo.AppCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void dgvItemDetails_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (_commonDs.Tables["Item"].Columns.Contains("SchdDrug") == true)
            {
                if (Convert.ToBoolean(_commonDs.Tables["Item"].Rows[e.RowIndex]["SchdDrug"]) == true)
                {
                    this.dgvItemDetails.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(248, 203, 173);
                    this.dgvItemDetails.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        #region Save Record and Payment option
        private void PaymentOption()
        {
            if (this.ValidateB4Saving() == false)
                return;

            bool chkprintBill;
            chkprintBill = chkPrintBill.Checked;

            if (_commonDs.Tables["Main"].Rows.Count > 0)
            {
                this.BindingContext[_commonDs, "Main"].EndCurrentEdit();

                if (Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Net_Amt"]) <= 0)
                {
                    MessageBox.Show("No records found for payment.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                this.DeleteChldRecords("Item");
                _commonDs.Tables["Main"].AcceptChanges();
                _commonDs.Tables["Item"].AcceptChanges();

                if (_commonDs.Tables["Item"].AsEnumerable().Where(row => row["Item"] != null).Count(row => row["Item"].ToString().Trim() == string.Empty) > 0)
                {
                    MessageBox.Show("Goods name cannot be blank.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SendKeys.Send("{Esc}");
                    return;
                }

                if (Convert.ToDecimal(_commonDs.Tables["Main"].Rows[0]["Net_Amt"]) > 0)
                {
                    udFrmPaymentScreen _frmPayScrn = new udFrmPaymentScreen(_commonDs, PayMode, chkprintBill);

                    _frmPayScrn.ShowDialog();
                    if (_frmPayScrn._RetValue == "SAVED")
                    {
                        bool chkPrintSetting = Convert.ToBoolean(_commonDs.Tables["UDPOSSETTINGS"].Rows[0]["LASKPRINT"]);

                        if (chkPrintSetting && chkprintBill)
                        {
                            if (MessageBox.Show("Do you want to Print ?", CommonInfo.AppCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

                        Export_CSV_Email();
                        clearTables();
                        addHdrRecords();

                        dgvItemDetails.Focus();
                        //SendKeys.Send("{Tab}");
                        if (dgvItemDetails.Rows.Count > 0)
                            dgvItemDetails.CurrentCell = dgvItemDetails.Rows[0].Cells[1];
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
        #endregion Save Record and Payment option

        #region Validate Before Saving
        private bool ValidateB4Saving()
        {
            bool _lret = true;

            if (CommonInfo.EntryTy == "HS")
            {
                foreach (DataRow _dr in _commonDs.Tables["Item"].Rows)
                {
                    string _cSql = "Select It_code from It_Mast a Inner join pRetSalt_Master b on a.prSalt=b.Salt where a.It_code = " + _dr["It_Code"].ToString() + " and a.prSalt!='' and b.Schedule!='' ";
                    DataRow _dr1 = _oDataAccess.GetDataRow(_cSql, null, 25);
                    if (_dr1 != null)
                    {
                        if (_commonDs.Tables["Main"].Rows[0]["PatientNm"].ToString() == "" || _commonDs.Tables["Main"].Rows[0]["DRNAME"].ToString() == "")
                        {
                            _lret = false;
                            break;
                        }
                        if (_commonDs.Tables["Main"].Rows[0]["DRPRESCNO"].ToString() == "")
                            if (_commonDs.Tables["UPLOADFILES"].Rows.Count > 0)
                            {
                                if (_commonDs.Tables["UPLOADFILES"].Rows[0]["FileName"].ToString() == "")
                                {
                                    _lret = false;
                                    break;
                                }
                            }
                            else
                            {
                                _lret = false;
                                break;
                            }
                    }
                }
                if (_lret == false)
                    MessageBox.Show("Scheduled Item is defined cannot save without Patient Name, Doctor Name and Prescription.", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (_lret == true)
                {
                    if (_commonDs.Tables["Item"].Select("BatchNo='' and It_code<>0").Count() > 0)
                    {
                        MessageBox.Show("Batch No. of some Goods are not entered. Cannot Continue...!!!", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _lret = false;
                    }
                }
            }
            return _lret;
        }
        #endregion Validate Before Saving

        #region Get Data based on Mobile No
        private void getMobileDetail(string cMobileNo, string _txtF2)
        {
            string cSql = "";
            string xcFldList = "", xcDispCol = "", xcRetCol = "", cFrmCap = "", cSrchCol = "", cDispCol = "", cRetCol = "";

            if ((_txtF2 == "TXT" && cMobileNo == "") || (_commonDs.Tables["Main"].Rows[0]["nMobileNo"].ToString().Trim() == cMobileNo.Trim() && cMobileNo != ""))
                return;
            string cCond1 = (_txtF2 == "TXT" ? " where ResMobile like '%" + cMobileNo + "%' " : "");
            string cCond2 = (_txtF2 == "TXT" ? " where nMobileno like '%" + cMobileNo + "%' " : "");

            cSql = "Select Distinct ResMobile, Name from pRetPatientMast " + cCond1 + " union select nmobileno,patientnm from dcmain " + cCond2;

            xcFldList = "";
            xcDispCol = "";
            xcRetCol = "";
            cFrmCap = "Select Mobile No";
            cSrchCol = "ResMobile";
            cDispCol = "ResMobile:Mobile No,Name:Patient";
            cRetCol = "ResMobile,Name";
            DataTable _dt, _dtFilt;
            _dt = _oDataAccess.GetDataTable(cSql, null, 50);

            if (_dt != null)
            {
                if (_dt.Rows.Count > 0)
                {
                    if (_dt.Rows.Count > 1)
                    {
                        DataView _dvw = _dt.DefaultView;

                        udSelectPop.SELECTPOPUP _oSelPop = new udSelectPop.SELECTPOPUP();
                        _oSelPop.pdataview = _dvw;
                        _oSelPop.pformtext = cFrmCap;
                        _oSelPop.psearchcol = cSrchCol;
                        _oSelPop.pDisplayColumnList = cDispCol;
                        _oSelPop.pRetcolList = cRetCol;
                        _oSelPop.Icon = new Icon(CommonInfo.IconPath.ToString().Replace("<*#*>", " "));
                        _oSelPop.ShowDialog();

                        if (_oSelPop.pReturnArray != null)
                        {
                            _dtFilt = _dt.Select("ResMobile='" + _oSelPop.pReturnArray[0].ToString().Trim() + "' and Name='" + _oSelPop.pReturnArray[1].ToString().Trim() + "'").CopyToDataTable();
                            _dt = _dtFilt;
                        }
                    }
                    _commonDs.Tables["Main"].Rows[0]["nMobileNo"] = _dt.Rows[0][0].ToString();
                    _commonDs.Tables["Main"].Rows[0]["PatientNm"] = _dt.Rows[0][1].ToString();
                    //Added by Shrikant S. on 24/12/2018 for Installer 2.1.0        //Start
                    this.txtMobileNo.Text = _dt.Rows[0][0].ToString();
                    Control[] ctrls = this.Controls.Find("txtPATIENTNM", true);
                    if (ctrls.Length > 0)
                        foreach (var item in ctrls)
                        {
                            item.Text = _dt.Rows[0][1].ToString();
                        }
                    //Added by Shrikant S. on 24/12/2018 for Installer 2.1.0        //End
                }
                else
                    MessageBox.Show("Details of respective Mobile no. is not available", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Details of respective Mobile no. is not available", CommonInfo.AppCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion Get Data based on Mobile No

        #region Method to Cancel UnSaved Bill
        private void CancelUnSavedRcrds()
        {
            if (_commonDs.Tables["Item"].Select("It_Code<>0").Count() > 0)
            {
                if (MessageBox.Show("You are cancelling the Un-Saved Bill. Do you want to continue?", CommonInfo.AppCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.clearTables();
                    this.addHdrRecords();

                    dgvItemDetails.Focus();
                    if (dgvItemDetails.Rows.Count > 0)
                        dgvItemDetails.CurrentCell = dgvItemDetails.Rows[0].Cells[1];
                }
            }
        }
        #endregion Method to Cancel UnSaved Bill

        #endregion Private Methods
    }
}
