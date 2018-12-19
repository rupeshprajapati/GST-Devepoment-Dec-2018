using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;


using System.Threading;
using System.Diagnostics;
using System.IO;
using uBaseForm;
using System.Collections;
using udSelectPop;
using DataAccess_Net;
using udclsDGVNumericColumn;
using System.Reflection;
using System.Globalization;
using udclsUDF;
using udDataTableQuery;
using udReportList;

namespace uGST_GSTR1_Json_Generation
{
    public partial class frmMain : uBaseForm.FrmBaseForm
    {
        DataSet vDsCommon;
        DataAccess_Net.clsDataAccess oDataAccess;
        udclsUDF.udclsUDF vUdClsUDF = new udclsUDF.udclsUDF();
        udDataTableQuery.cSelectDistinct vUDQ = new udDataTableQuery.cSelectDistinct();

        string vSqlStr;
        short vTimeOut = 50;
        String cAppPId, cAppName;

        StringBuilder vJsonString = new StringBuilder();
        string vFrmDate = "", vToDate = "", vSecCode = "B2B";
        int vMnth = 0, vYear = 0;
        DataTable tblGSTR1;



        public frmMain(string[] args)
        {
            InitializeComponent();
            this.pPApplPID = 0;
            this.pPara = args;
            this.pFrmCaption = "GSTR -1 JSON Generation";
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
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            this.pAppPath = Application.ExecutablePath;
            this.pAppPath = Path.GetDirectoryName(this.pAppPath);
            this.mthControlSet();


            DataAccess_Net.clsDataAccess._databaseName = this.pComDbnm;
            DataAccess_Net.clsDataAccess._serverName = this.pServerName;
            DataAccess_Net.clsDataAccess._userID = this.pUserId;
            DataAccess_Net.clsDataAccess._password = this.pPassword;
            oDataAccess = new DataAccess_Net.clsDataAccess();
            this.mthDsCommon();
            this.SetFormColor();
            this.mInsertProcessIdRecord();
            this.mthView();
        }
        private void mthView()
        {

            int vYear = DateTime.Now.Year;
            int vMonth = DateTime.Now.Month;
            if (vMonth <= 3)
            {
                this.txtFYear.Text = (vYear - 1).ToString().Trim() + "-" + (vYear).ToString().Trim();
            }
            else
            {
                this.txtFYear.Text = (vYear).ToString().Trim() + "-" + (vYear + 1).ToString().Trim();
            }
            this.txtPeriod.Text = vUdClsUDF.MonthName((short)vMonth);
            this.txtSection.Text = "B2B Invoice - 4A,4B, 4C, 6B, 6C";
            vSecCode = "B2B";


        }
        private void btnFYear_Click(object sender, EventArgs e)
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataTable tblPop;
            this.txtFYear.Text = "";
            vSqlStr = "Select [Fin_Year]='2017-2018'  union  Select '2018-2019' ";
            tblPop = oDataAccess.GetDataTable(vSqlStr, null, 20);


            DataView dvw = tblPop.DefaultView;

            VForText = "Select Financial Year";
            vSearchCol = "Fin_Year";
            vDisplayColumnList = "Fin_Year:Finanacial Year";
            vReturnCol = "Fin_Year";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();
            if (oSelectPop.pReturnArray != null)
            {
                this.txtFYear.Text = oSelectPop.pReturnArray[0];
            }
        }

        private void btnPeriod_Click(object sender, EventArgs e)
        {
            txtJsonFile.Text = "";
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataTable tblPop;
            this.txtPeriod.Text = "";
            vSqlStr = "Select [TPeriod]='January' ,[order]=10 union  Select 'February' ,11 union  Select 'March' ,12 union  Select 'April' ,1 union  Select 'May',2 union  Select 'June' ,3 union  Select 'July' ,4 union  Select 'August',5 union  Select 'September' ,6 union  Select 'October',7 union  Select 'November',8 union  Select 'December',9 union  Select 'Quarter-1',13 union  Select 'Quarter-2',14 union  Select 'Quarter-3',15  union  Select 'Quarter-4',16 order by [order] ";
            tblPop = oDataAccess.GetDataTable(vSqlStr, null, 20);


            DataView dvw = tblPop.DefaultView;

            VForText = "Select Tax Period";
            vSearchCol = "TPeriod";
            vDisplayColumnList = "TPeriod:Tax Period";
            vReturnCol = "TPeriod";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();
            if (oSelectPop.pReturnArray != null)
            {
                this.txtPeriod.Text = oSelectPop.pReturnArray[0];
            }
        }

        private void btnSection_Click(object sender, EventArgs e)
        {
            vSecCode = "B2B";
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataTable tblPop;
            vSqlStr = "Select [SecNm]='All',SecCode='All' union Select [SecNm]='B2B Invoice - 4A,4B, 4C, 6B, 6C',SecCode='B2B'  union  Select 'B2C (Large) Invoices 5A, 5B','B2CL' union  Select 'B2C (Small) Invoices 7','B2CS' union Select 'Credit/Debit Notes(Registered) - 9B','CDNR' union Select 'Credit/Debit Notes(Unregistered) - 9B','CDNUR' union Select 'Export Invoices - 6A','EXP'";
            vSqlStr = vSqlStr + " union Select 'Tax Liability (Advances Received) - 11A(1),11A(2)','AT' union Select 'Adjustment of Advances - 11B(1),11B(2)','TXPD'";
            vSqlStr = vSqlStr + " union Select 'Amended B2B Invoices - 9','B2BA' union Select 'Amended B2C (Large) Invoices - 9','B2CLA'  union Select 'Amended B2C (Small) Invoices - 9','B2CSA' union Select 'Amended Credit/Debit Notes(Registered) - 9C','CDNRA' union Select 'Amended Credit/Debit Notes(unregistered) - 10','CDNURA' union Select 'Amended Tax Liability(Advances Received) - 11','ATA' union Select 'Amended Adjustment of Advances - 11','TXPDA'";
            vSqlStr = vSqlStr + " union Select 'Nil Rated / Exempted Non GST Invoice - 8','NIL' union Select 'HSN-Wise Summary of Outward Supplies - 12','HSN'  union Select 'Document Issued - 13','DOC_ISSUE' union Select 'Ammended  Exports','EXPA'";
            tblPop = oDataAccess.GetDataTable(vSqlStr, null, 20);


            DataView dvw = tblPop.DefaultView;

            VForText = "Select Section";
            vSearchCol = "SecNm";
            vDisplayColumnList = "SecNm:Section Name";
            vReturnCol = "SecNm,SecCode";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;

            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();
            if (oSelectPop.pReturnArray != null)
            {
                this.txtSection.Text = oSelectPop.pReturnArray[0];
                vSecCode = oSelectPop.pReturnArray[1];
            }

        }
        private void btnJson_Click(object sender, EventArgs e)
        {
            mcheckCallingApplication();


            if (this.txtJsonFile.Text == "")
            {

                MessageBox.Show("Please select the json File Path", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.btnJsonFile_Click(sender, e);
                this.btnJson_Click(sender, e);
            }
            else
            {
                this.mthGetDateRange(ref vFrmDate, ref vToDate);
                // vJsonString = vJsonString.Clear();RRG
                vJsonString = new StringBuilder();

                vJsonString.Append("{ \n  \"gstin\":" + "\"" + this.vDsCommon.Tables["company"].Rows[0]["gstin"].ToString().Trim() + "\"");
                string vTxPrd = (vMnth == 1 || vMnth == 2 || vMnth == 3 || vMnth == 4 || vMnth == 5 || vMnth == 6 || vMnth == 7 || vMnth == 8 || vMnth == 9 ? "0" : "") + vMnth.ToString().Trim() + vYear.ToString().Trim();
                vJsonString.Append(", \n  \"fp\":" + "\"" + vTxPrd.ToString().Trim() + "\"");
                vJsonString.Append(", \n  \"gt\":" + this.vDsCommon.Tables["company"].Rows[0]["grturnover"].ToString().Trim());
                vJsonString.Append(", \n  \"cur_gt\":" + this.vDsCommon.Tables["company"].Rows[0]["grstrnover"].ToString().Trim());
                vJsonString.Append(", \n  \"version\":" + "\"GST2.2.5\"");
                vJsonString.Append(", \n  \"hash\":\"hash\"");
                //this.vDsCommon.Tables["company"].Rows[0][]
                switch (vSecCode)
                {
                    case "All":
                        this.mthJsonGen_B2B(false);
                        this.mthJsonGen_B2CL(false);
                        this.mthJsonGen_B2CS(false);
                        this.mthJsonGen_CDNR(false);
                        this.mthJsonGen_CDNUR(false);
                        this.mthJsonGen_EXP();
                        this.mthJsonGen_AT(false);
                        this.mthJsonGen_TXPD(false);
                        this.mthJsonGen_NIL();
                        this.mthJsonGen_HSN();
                        this.mthJsonGen_DocIssue();

                        this.mthJsonGen_B2B(true);
                        this.mthJsonGen_B2CL(true);
                        this.mthJsonGen_B2CS(true);
                        this.mthJsonGen_CDNR(true);
                        this.mthJsonGen_CDNUR(true);
                        this.mthJsonGen_AT(true);
                        this.mthJsonGen_TXPD(true);


                        break;
                    case "B2B":
                        this.mthJsonGen_B2B(false);
                        break;
                    case "B2CL":
                        this.mthJsonGen_B2CL(false);
                        break;
                    case "B2CS":
                        this.mthJsonGen_B2CS(false);
                        break;
                    case "CDNR":
                        this.mthJsonGen_CDNR(false);
                        break;
                    case "CDNUR":
                        this.mthJsonGen_CDNUR(false);
                        break;
                    case "EXP":
                        this.mthJsonGen_EXP();
                        break;
                    case "EXPA":
                        this.mthJsonGen_EXPA();
                        break;
                    case "AT":
                        this.mthJsonGen_AT(false);
                        break;
                    case "TXPD":
                        this.mthJsonGen_TXPD(false);
                        break;
                    case "NIL":
                        this.mthJsonGen_NIL();
                        break;
                    case "HSN":
                        this.mthJsonGen_HSN();
                        break;
                    case "DOC_ISSUE":
                        this.mthJsonGen_DocIssue();
                        break;
                    case "B2BA":
                        this.mthJsonGen_B2B(true);
                        break;
                    case "B2CLA":
                        this.mthJsonGen_B2CL(true);
                        break;
                    case "B2CSA":
                        this.mthJsonGen_B2CS(true);
                        break;
                    case "CDNRA":
                        this.mthJsonGen_CDNR(true);
                        break;
                    case "CDNURA":
                        this.mthJsonGen_CDNUR(true);
                        break;
                    case "ATA":
                        this.mthJsonGen_AT(true);
                        break;
                    case "TXPDA":
                        this.mthJsonGen_TXPD(true);
                        break;
                }
                vJsonString.Append("\n }");
                string v;
                v = vJsonString.ToString().Trim();
                v = v.Replace(".000,", ",");
                v = v.Replace(".00,", ",");

                string fName = this.txtJsonFile.Text;


                System.IO.File.WriteAllText(fName, v);

                MessageBox.Show("JSON file has been generated at the below location : \n" + fName.ToString().Trim(), this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
        }
        private void mthJsonGen_B2B(Boolean vAmended)
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'B2B'" : "'B2BA'") + ",0";
            DataTable tblB2B;
            tblB2B = oDataAccess.GetDataTable(vSqlStr, null, 160);
            //  MessageBox.Show(tblB2B.Rows.Count.ToString().Trim());
            DataTable tblCTIN;
            tblCTIN = vUDQ.SelectDistinct(tblB2B, "", "ctin");

            if (vAmended == false)
            { vJsonString.Append(",\n   \"b2b\":["); }
            else
            { vJsonString.Append(",\n   \"b2ba\":["); }

            for (int i = 0; i <= tblCTIN.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*Ctin Breaket Open*/
                vJsonString.Append(" \n" + "           \"ctin\":" + "\"" + tblCTIN.Rows[i]["ctin"].ToString().Trim() + "\"");
                this.mthJsonGenB2B_inv(vAmended, tblCTIN.Rows[i]["ctin"].ToString().Trim(), tblB2B);
                vJsonString.Append("\n         }");/*Ctin Breaket Close*/
            }
            vJsonString.Append("\n   ]");
        }
        private void mthJsonGenB2B_inv(Boolean vAmended, string vCTIN, DataTable tblB2B)
        {
            CultureInfo ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            DataTable tblInv;

            string[] vFldList = new string[10];
            vFldList[0] = "inum"; vFldList[1] = "idt"; vFldList[2] = "val"; vFldList[3] = "pos"; vFldList[4] = "rchrg"; vFldList[5] = "inv_typ"; vFldList[6] = "ctin"; vFldList[7] = "etin"; vFldList[8] = "oinum"; vFldList[9] = "oidt";
            tblInv = vUDQ.SelectDistinct(tblB2B, "ctin = '" + vCTIN + "'", vFldList);

            vJsonString.Append(",\n          \"inv\":[");
            for (int i = 0; i <= tblInv.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n               {"); } else { vJsonString.Append(", \n               {"); } /*inv Breaket Open*/
                if (vAmended == true)
                {
                    vJsonString.Append(" \n" + "                   \"oinum\":" + "\"" + tblInv.Rows[i]["oinum"].ToString().Trim() + "\",");
                    vJsonString.Append(" \n" + "                   \"oidt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["oidt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                }
                vJsonString.Append(" \n" + "                   \"inum\":" + "\"" + tblInv.Rows[i]["inum"].ToString().Trim() + "\",");

                //DateTime enteredDate = DateTime.Parse(tblInv.Rows[i]["idt"].ToString().Trim());
                //   string d = enteredDate.ToString("dd-MM-yyyy");
                //  string date1 = DateTime.Parse(tblInv.Rows[i]["idt"].ToString().Trim()).ToString("dd-MM-yyyy");
                vJsonString.Append(" \n" + "                   \"idt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["idt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                //vJsonString.Append(" \n" + "                   \"idt\":" + "\"" + tblInv.Rows[i]["idt"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"val\":" + tblInv.Rows[i]["val"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                   \"pos\":" + "\"" + tblInv.Rows[i]["pos"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"rchrg\":" + "\"" + tblInv.Rows[i]["rchrg"].ToString().Trim() + "\",");

                //RRG Comment Start
                //if (tblInv.Rows[i]["etin"].ToString().Trim() != "")
                //{
                //    vJsonString.Append(" \n" + "                   \"etin\":" + "\"" + tblInv.Rows[i]["etin"].ToString().Trim() + "\",");
                //}
                //RRG Comment End

                vJsonString.Append(" \n" + "                   \"inv_typ\":" + "\"" + tblInv.Rows[i]["inv_typ"].ToString().Trim() + "\",");
                this.mthJsonGenB2B_itms(tblInv.Rows[i]["inum"].ToString().Trim(), tblInv, tblB2B);
                vJsonString.Append("\n               }");/*inv Breaket Close*/
            }
            vJsonString.Append("\n   ]");

        }
        private void mthJsonGenB2B_itms(string vINum, DataTable tblInv, DataTable tblB2B)
        {

            DataTable tblItms;
            string[] vFldList = new string[7];
            vFldList[0] = "txval"; vFldList[1] = "rt"; vFldList[2] = "camt"; vFldList[3] = "samt"; vFldList[4] = "iamt"; vFldList[5] = "csamt"; vFldList[6] = "inum";
            tblItms = vUDQ.SelectDistinct(tblB2B, "inum = '" + vINum + "'", vFldList);

            vJsonString.Append("\n                   \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n                            {"); } else { vJsonString.Append(", \n                         {"); } /*itms Breaket Open*/
                //vJsonString.Append(" \n" + "                             \"num\":" + (i + 1).ToString().Trim());
                vJsonString.Append(" \n" + "                             \"num\":" + tblItms.Rows[i]["rt"].ToString().Trim().Replace(".000", "") + "01");
                vJsonString.Append(",\n                                   \"itm_det\":{");  /*itm_det Breaket Open*/
                vJsonString.Append(" \n" + "                                  \"txval\":" + tblItms.Rows[i]["txval"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                                  \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");
                if (((Decimal)tblItms.Rows[i]["iamt"]) > 0)
                {
                    vJsonString.Append(" \n" + "                                  \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");
                }
                else
                {
                    vJsonString.Append(" \n" + "                                  \"camt\":" + tblItms.Rows[i]["camt"].ToString().Trim() + ",");
                    vJsonString.Append(" \n" + "                                  \"samt\":" + tblItms.Rows[i]["samt"].ToString().Trim() + ",");
                }
                string cessamt = string.Empty;
                if (tblItms.Rows[i]["csamt"].ToString().Trim() == "0.00")
                {
                    cessamt = "0";

                }
                else
                {
                    cessamt = tblItms.Rows[i]["csamt"].ToString().Trim();
                }
                vJsonString.Append(" \n" + "                                  \"csamt\":" + cessamt);
                vJsonString.Append("\n                             }");/*itm_det Breaket Close*/
                vJsonString.Append("\n                          }");/*itms Breaket Close*/
            }
            vJsonString.Append("\n                           ]");

        }
        private void mthGetDateRange(ref string vFrmDate, ref string vToDate)
        {
            vMnth = vUdClsUDF.MonthNumber(this.txtPeriod.Text);
            vYear = (vMnth == 1 || vMnth == 2 || vMnth == 3 ? Convert.ToInt16(txtFYear.Text.Substring(5, 4)) : Convert.ToInt16(txtFYear.Text.Substring(0, 4)));

            if (this.txtPeriod.Text == "Quarter-1")
            {
                vFrmDate = "01/04/" + this.txtFYear.Text.Substring(0, 4);
                vToDate = "30/06/" + this.txtFYear.Text.Substring(0, 4);
            }
            else if (this.txtPeriod.Text == "Quarter-2")
            {
                vFrmDate = "01/07/" + this.txtFYear.Text.Substring(0, 4);
                vToDate = "30/09/" + this.txtFYear.Text.Substring(0, 4);
            }
            else if (this.txtPeriod.Text == "Quarter-3")
            {
                vFrmDate = "01/10/" + this.txtFYear.Text.Substring(0, 4);
                vToDate = "31/12/" + this.txtFYear.Text.Substring(0, 4);
            }
            else if (this.txtPeriod.Text == "Quarter-4")
            {
                vFrmDate = "01/01/"+this.txtFYear.Text.Substring(5,4);
                vToDate = "31/03/" + this.txtFYear.Text.Substring(5, 4);
            }
            else
            {

                if (vMnth == 5 || vMnth == 7 || vMnth == 8 || vMnth == 10 || vMnth == 12)
                {
                    vFrmDate = "01/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(0, 4);
                    vToDate = "31/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(0, 4);
                }
                else if (vMnth == 4 || vMnth == 6 || vMnth == 9 || vMnth == 11)
                {
                    vFrmDate = "01/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(0, 4);
                    vToDate = "30/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(0, 4);
                }
                else if (vMnth == 1 || vMnth == 3)
                {
                    vFrmDate = "01/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(5, 4);
                    vToDate = "31/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(5, 4);
                }
                else
                {
                    vFrmDate = "01/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(5, 4);
                    vToDate = "28/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(5, 4);
                    int vYear = 0;
                    vYear = Convert.ToUInt16(txtFYear.Text.Substring(5, 4));
                    if (DateTime.IsLeapYear(vYear))
                    {
                        vToDate = "29/" + vMnth.ToString().Trim() + "/" + this.txtFYear.Text.Substring(5, 4);
                    }
                }
            }


        }
        private void mthJsonGen_B2CL(Boolean vAmended)
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'B2CL'" : "'B2CLA'") + ",0";
            DataTable tblB2CL;
            tblB2CL = oDataAccess.GetDataTable(vSqlStr, null, 25);
            DataTable tblPOS;
            tblPOS = vUDQ.SelectDistinct(tblB2CL, "", "pos");
            //if(vSecCode=="All")
            //{
            //    vJsonString.Append(",");
            //}
            if (vAmended == false)
            { vJsonString.Append(",\n   \"b2cl\":["); }
            else
            { vJsonString.Append(",\n   \"b2cla\":["); }

            for (int i = 0; i <= tblPOS.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*POS Breaket Open*/
                vJsonString.Append(" \n" + "           \"pos\":" + "\"" + tblPOS.Rows[i]["pos"].ToString().Trim() + "\",");
                this.mthJsonGen_B2CL_inv(vAmended, tblPOS.Rows[i]["pos"].ToString().Trim(), tblB2CL);
                vJsonString.Append("\n         }");/*POS Breaket Close*/
            }
            vJsonString.Append("\n   ]");
        }
        private void mthJsonGen_B2CL_inv(Boolean vAmended, string vPos, DataTable tblB2CL)
        {

            DataTable tblInv;
            string[] vFldList = new string[6];
            vFldList[0] = "inum"; vFldList[1] = "idt"; vFldList[2] = "val"; vFldList[3] = "etin"; vFldList[4] = "oinum"; vFldList[5] = "oidt";
            tblInv = vUDQ.SelectDistinct(tblB2CL, "pos = '" + vPos + "'", vFldList);

            vJsonString.Append("\n          \"inv\":[");
            for (int i = 0; i <= tblInv.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n               {"); } else { vJsonString.Append(", \n               {"); } /*inv Breaket Open*/
                if (vAmended == true)
                {
                    vJsonString.Append(" \n" + "                   \"oinum\":" + "\"" + tblInv.Rows[i]["oinum"].ToString().Trim() + "\",");
                    vJsonString.Append(" \n" + "                   \"oidt\":" + "\"" + tblInv.Rows[i]["oidt"].ToString().Trim() + "\",");
                }
                vJsonString.Append(" \n" + "                   \"inum\":" + "\"" + tblInv.Rows[i]["inum"].ToString().Trim() + "\",");

                vJsonString.Append(" \n" + "                   \"idt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["idt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                //vJsonString.Append(" \n" + "                   \"idt\":" + "\"" + tblInv.Rows[i]["idt"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"val\":" + tblInv.Rows[i]["val"].ToString().Trim() + ",");
                if (tblInv.Rows[i]["etin"].ToString().Trim() != "")
                {
                    vJsonString.Append(" \n" + "                   \"etin\":" + "\"" + tblInv.Rows[i]["etin"].ToString().Trim() + "\",");
                }
                this.mthJsonGen_B2CL_itms(tblInv.Rows[i]["inum"].ToString().Trim(), tblB2CL);
                vJsonString.Append("\n               }");/*inv Breaket Close*/
            }
            vJsonString.Append("\n   ]");

        }
        private void mthJsonGen_B2CL_itms(string vINum, DataTable tblB2CL)
        {

            DataTable tblItms;
            string[] vFldList = new string[4];
            vFldList[0] = "txval"; vFldList[1] = "rt"; vFldList[2] = "iamt"; vFldList[3] = "csamt";
            tblItms = vUDQ.SelectDistinct(tblB2CL, "inum = '" + vINum + "'", vFldList);

            vJsonString.Append("\n                   \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n                            {"); } else { vJsonString.Append(", \n                         {"); } /*itms Breaket Open*/
                                                                                                                                                      // vJsonString.Append(" \n" + "                             \"num\":" + (i + 1).ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                             \"num\":" + tblItms.Rows[i]["rt"].ToString().Trim().Replace(".000", "") + "01");
                vJsonString.Append("\n                              \"itm_det\":{");  /*itm_det Breaket Open*/
                vJsonString.Append(" \n" + "                               \"txval\":" + tblItms.Rows[i]["txval"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                               \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                               \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");

                string cessamt = string.Empty;
                if (tblItms.Rows[i]["csamt"].ToString().Trim() == "0.00")
                {
                    cessamt = "0";

                }
                else
                {
                    cessamt = tblItms.Rows[i]["csamt"].ToString().Trim();
                }
                vJsonString.Append(" \n" + "                               \"csamt\":" + cessamt);
                // vJsonString.Append(" \n" + "                               \"csamt\":" + tblItms.Rows[i]["csamt"].ToString().Trim() );
                vJsonString.Append("\n                             }");/*itm_det Breaket Close*/
                vJsonString.Append("\n                            }");/*itms Breaket Close*/
            }
            vJsonString.Append("\n                           ]");

        }
        private void mthJsonGen_B2CS(Boolean vAmended)
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'B2CS'" : "'B2CSA'") + ",0";
            DataTable tblB2CS;
            tblB2CS = oDataAccess.GetDataTable(vSqlStr, null, 25);

            //if (vSecCode == "All")
            //{
            //    vJsonString.Append(",");
            //}

            //if (vAmended == true) comment RRG
            if (vAmended == false)// add RRG
            { vJsonString.Append(",\n   \"b2cs\":["); }
            else
            { vJsonString.Append(",\n   \"b2csa\":["); }

            if (vAmended == false)
            {
                for (int i = 0; i <= tblB2CS.Rows.Count - 1; i++)
                {
                    if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); }
                    vJsonString.Append(" \n" + "                   \"sply_ty\":" + "\"" + tblB2CS.Rows[i]["sply_ty"].ToString().Trim() + "\",");

                    //RRG Comment Start
                    //if (vAmended == true)
                    //{
                    //    vJsonString.Append(" \n" + "                   \"omon\":" + "\"" + tblB2CS.Rows[i]["omon"].ToString().Trim() + "\",");
                    //}
                    //RRG Comment end

                    vJsonString.Append(" \n" + "                   \"pos\":" + "\"" + tblB2CS.Rows[i]["pos"].ToString().Trim() + "\",");//ADD RRG
                    vJsonString.Append(" \n" + "                   \"typ\":" + "\"" + tblB2CS.Rows[i]["typ"].ToString().Trim() + "\",");
                    vJsonString.Append(" \n" + "                   \"txval\":" + tblB2CS.Rows[i]["txval"].ToString().Trim() + ",");
                    vJsonString.Append(" \n" + "                   \"rt\":" + tblB2CS.Rows[i]["rt"].ToString().Trim() + ",");//Add RRG

                    //  vJsonString.Append(" \n" + "                   \"etin\":" + "\"" + tblB2CS.Rows[i]["etin"].ToString().Trim() + "\",");RRG Comment


                    // vJsonString.Append(" \n" + "                   \"typ\":" + "\"" + tblB2CS.Rows[i]["typ"].ToString().Trim() + "\",");RRG

                    //vJsonString.Append(" \n" + "                   \"pos\":" + "\"" + tblB2CS.Rows[i]["pos"].ToString().Trim() + "\",");RRG
                    //vJsonString.Append(" \n" + "                   \"rt\":" + tblB2CS.Rows[i]["rt"].ToString().Trim() + ",");

                    //ADD Comment RRG
                    //if (((Decimal)tblB2CS.Rows[i]["iamt"]) > 0)
                    //{
                    //    vJsonString.Append(" \n" + "                   \"iamt\":" + tblB2CS.Rows[i]["iamt"].ToString().Trim() + ",");
                    //}
                    //else
                    //{
                    //    vJsonString.Append(" \n" + "                   \"camt\":" + tblB2CS.Rows[i]["camt"].ToString().Trim() + ",");
                    //    vJsonString.Append(" \n" + "                   \"samt\":" + tblB2CS.Rows[i]["samt"].ToString().Trim() + ",");
                    //}
                    //End Comment RRG

                    //Add RRG 

                    //if (((Decimal)tblB2CS.Rows[i]["iamt"]) > 0)
                    //{
                        vJsonString.Append(" \n" + "                   \"iamt\":" + tblB2CS.Rows[i]["iamt"].ToString().Trim() + ",");
                    //}
                    //else
                    //{
                        vJsonString.Append(" \n" + "                   \"camt\":" + tblB2CS.Rows[i]["camt"].ToString().Trim() + ",");
                        vJsonString.Append(" \n" + "                   \"samt\":" + tblB2CS.Rows[i]["samt"].ToString().Trim() + ",");
                    //}
                    //Add RRg
                    string cessamt = string.Empty;
                    if (tblB2CS.Rows[i]["csamt"].ToString().Trim() == "0.00")
                    {
                        cessamt = "0";
                    }
                    else
                    {
                        cessamt = tblB2CS.Rows[i]["csamt"].ToString().Trim();

                    }
                    vJsonString.Append(" \n" + "                   \"csamt\":" + cessamt);
                    //End RRG
                    // vJsonString.Append(" \n" + "                   \"csamt\":" + tblB2CS.Rows[i]["csamt"].ToString().Trim() );
                    vJsonString.Append("\n         }");
                }
            }
            else
            {
                DataTable tblItms;
                string[] vFldList = new string[4];
                vFldList[0] = "sply_ty"; vFldList[1] = "pos"; vFldList[2] = "typ"; vFldList[3] = "omon";
                tblItms = vUDQ.SelectDistinct(tblB2CS, "", vFldList);

                int ii = 0;

                foreach (DataRow dr in tblItms.Rows)
                {
                    if (ii == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); }
                    vJsonString.Append(" \n" + "                   \"sply_ty\":" + "\"" + dr["sply_ty"].ToString().Trim() + "\",");
                    vJsonString.Append(" \n" + "                   \"pos\":" + "\"" + dr["pos"].ToString().Trim() + "\",");
                    vJsonString.Append(" \n" + "                   \"typ\":" + "\"" + dr["typ"].ToString().Trim() + "\",");

                    vJsonString.Append("\n                   \"itms\":[");

                    //MessageBox.Show(dr["sply_ty"].ToString()+""+ dr["pos"].ToString()+""+ dr["typ"].ToString());
                    DataTable tblItms1;
                    string[] vFldList1 = new string[6];
                    vFldList1[0] = "txval"; vFldList1[1] = "rt"; vFldList1[2] = "iamt"; vFldList1[3] = "camt"; vFldList1[4] = "samt"; vFldList1[5] = "csamt";
                    tblItms1 = vUDQ.SelectDistinct(tblB2CS, "sply_ty='" + dr["sply_ty"].ToString() + "' and pos='" + dr["pos"].ToString() + "' and typ='" + dr["typ"].ToString() + "'", vFldList1);

                    for (int i = 0; i <= tblItms1.Rows.Count - 1; i++)
                    {
                        if (i == 0) { vJsonString.Append("\n                  {"); } else { vJsonString.Append(", \n                  {"); }
                        vJsonString.Append(" \n" + "                   \"rt\":" + tblItms1.Rows[i]["rt"].ToString().Trim() + ",");//Add RRG 
                        vJsonString.Append(" \n" + "                   \"txval\":" + tblItms1.Rows[i]["txval"].ToString().Trim() + ",");


                        if (((Decimal)tblItms1.Rows[i]["iamt"]) > 0)
                        {
                            vJsonString.Append(" \n" + "                   \"iamt\":" + tblItms1.Rows[i]["iamt"].ToString().Trim() + ",");
                        }
                        else
                        {
                            vJsonString.Append(" \n" + "                   \"camt\":" + tblItms1.Rows[i]["camt"].ToString().Trim() + ",");
                            vJsonString.Append(" \n" + "                   \"samt\":" + tblItms1.Rows[i]["samt"].ToString().Trim() + ",");
                        }
                        string cessamt = string.Empty;
                        if (tblItms1.Rows[i]["csamt"].ToString().Trim() == "0.00")
                        {
                            cessamt = "0";
                        }
                        else
                        {
                            cessamt = tblItms1.Rows[i]["csamt"].ToString().Trim();

                        }
                        vJsonString.Append(" \n" + "                   \"csamt\":" + cessamt);
                        vJsonString.Append("\n                  }");


                    }
                    vJsonString.Append("\n                   ],");
                    vJsonString.Append(" \n" + "                   \"omon\":" + "\"" + dr["omon"].ToString().Trim() + "\"");
                    vJsonString.Append("\n         }");
                    ii++;
                }

            }


            vJsonString.Append("\n   ]");
        }
        private void mthJsonGen_CDNR(Boolean vAmended)
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'CDNR'" : "'CDNRA'") + ",0";
            DataTable tblCDNR;
            tblCDNR = oDataAccess.GetDataTable(vSqlStr, null, 25);
            //MessageBox.Show(tblCDNR.Rows.Count.ToString().Trim());
            DataTable tblCTIN;
            tblCTIN = vUDQ.SelectDistinct(tblCDNR, "", "ctin");
            if (vAmended == false)
            { vJsonString.Append(",\n   \"cdnr\":["); }
            else
            { vJsonString.Append(",\n   \"cdnra\":["); }

            for (int i = 0; i <= tblCTIN.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*Ctin Breaket Open*/
                vJsonString.Append(" \n" + "           \"ctin\":" + "\"" + tblCTIN.Rows[i]["ctin"].ToString().Trim() + "\"");
                this.mthJsonGen_CDNR_inv(vAmended, tblCTIN.Rows[i]["ctin"].ToString().Trim(), tblCDNR);
                vJsonString.Append("\n         }");/*Ctin Breaket Close*/
            }
            vJsonString.Append("\n   ]");
        }
        private void mthJsonGen_CDNR_inv(Boolean vAmended, string vCTIN, DataTable tblCDNR)
        {

            DataTable tblInv;
            string[] vFldList = new string[11];
            vFldList[0] = "ctin"; vFldList[1] = "ntty"; vFldList[2] = "nt_num"; vFldList[3] = "nt_dt"; vFldList[4] = "rsn"; vFldList[5] = "p_gst"; vFldList[6] = "inum"; vFldList[7] = "idt"; vFldList[8] = "val"; vFldList[9] = "ont_num"; vFldList[10] = "ont_dt";
            tblInv = vUDQ.SelectDistinct(tblCDNR, "ctin = '" + vCTIN + "'", vFldList);

            vJsonString.Append(",\n          \"nt\":[");
            for (int i = 0; i <= tblInv.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n               {"); } else { vJsonString.Append(", \n               {"); } /*nt Breaket Open*/
                vJsonString.Append(" \n" + "                   \"nt_num\":" + "\"" + tblInv.Rows[i]["nt_num"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"nt_dt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["nt_dt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                if (vAmended == true)
                {
                    vJsonString.Append(" \n" + "                   \"ont_num\":" + "\"" + tblInv.Rows[i]["ont_num"].ToString().Trim() + "\",");
                    vJsonString.Append(" \n" + "                   \"ont_dt\":" + "\"" + tblInv.Rows[i]["ont_dt"].ToString().Trim() + "\",");
                }
                vJsonString.Append(" \n" + "                   \"ntty\":" + "\"" + tblInv.Rows[i]["ntty"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"val\":" + tblInv.Rows[i]["val"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                   \"inum\":" + "\"" + tblInv.Rows[i]["inum"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"idt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["idt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                vJsonString.Append(" \n" + "                   \"p_gst\":" + "\"" + tblInv.Rows[i]["p_gst"].ToString().Trim() + "\",");
                //Dis with RP
                //vJsonString.Append(" \n" + "                   \"rsn\":" + "\"" + tblInv.Rows[i]["rsn"].ToString().Trim() + "\",");//Comment RRG

                this.mthJsonGen_CDNR_itms(tblInv.Rows[i]["nt_num"].ToString().Trim(), tblInv, tblCDNR);
                vJsonString.Append("\n               }");/*nt Breaket Close*/
            }
            vJsonString.Append("\n   ]");

        }
        private void mthJsonGen_CDNR_itms(string vNT_Num, DataTable tblInv, DataTable tblCDNR)
        {

            DataTable tblItms;
            string[] vFldList = new string[7];
            vFldList[0] = "txval"; vFldList[1] = "rt"; vFldList[2] = "camt"; vFldList[3] = "samt"; vFldList[4] = "iamt"; vFldList[5] = "csamt"; vFldList[6] = "inum";
            tblItms = vUDQ.SelectDistinct(tblCDNR, "nt_num = '" + vNT_Num + "'", vFldList);

            vJsonString.Append("\n                   \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n                            {"); } else { vJsonString.Append(", \n                         {"); } /*itms Breaket Open*/
                                                                                                                                                      // vJsonString.Append(" \n" + "                             \"num\":" + (i + 1).ToString().Trim());
                vJsonString.Append(" \n" + "                             \"num\":" + tblItms.Rows[i]["rt"].ToString().Trim().Replace(".000", "") + "01");
                vJsonString.Append(",\n                                   \"itm_det\":{");  /*itm_det Breaket Open*/
                vJsonString.Append(" \n" + "                                  \"txval\":" + tblItms.Rows[i]["txval"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                                  \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");

                if (((Decimal)tblItms.Rows[i]["iamt"]) > 0)
                {
                    vJsonString.Append(" \n" + "                                  \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");
                }
                else
                {
                    vJsonString.Append(" \n" + "                                  \"camt\":" + tblItms.Rows[i]["camt"].ToString().Trim() + ",");
                    vJsonString.Append(" \n" + "                                  \"samt\":" + tblItms.Rows[i]["samt"].ToString().Trim() + ",");
                }

                //Add RRG
                string cessamt = string.Empty;
                if (tblItms.Rows[i]["csamt"].ToString().Trim() == "0.00")
                {
                    cessamt = "0";
                }
                else
                {
                    cessamt = tblItms.Rows[i]["csamt"].ToString().Trim();

                }
                vJsonString.Append(" \n" + "                                  \"csamt\":" + cessamt);
                //END RRG
                // vJsonString.Append(" \n" + "                                  \"csamt\":" + tblItms.Rows[i]["csamt"].ToString().Trim()); comment RRG

                vJsonString.Append("\n                             }");/*itm_det Breaket Close*/
                vJsonString.Append("\n                          }");/*itms Breaket Close*/
            }
            vJsonString.Append("\n                           ]");

        }

        private void mthJsonGen_CDNUR(Boolean vAmended)
        {

            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'CDNUR'" : "'CDNURA'") + ",0";
            DataTable tblCDNUR;
            tblCDNUR = oDataAccess.GetDataTable(vSqlStr, null, 25);
            //MessageBox.Show(tblCDNUR.Rows.Count.ToString().Trim());

            if (vAmended == false)
            { vJsonString.Append(",\n   \"cdnur\":["); }
            else
            { vJsonString.Append(",\n   \"cdnura\":["); }

            DataTable dtCDNUR = new DataTable();
            dtCDNUR= tblCDNUR.DefaultView.ToTable(true, "nt_num", "nt_dt", "ont_num", "ont_dt", "ntty", "val", "typ", "inum", "idt", "p_gst");

            for (int i = 0; i <= dtCDNUR.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n               {"); } else { vJsonString.Append(", \n               {"); } /*nt Breaket Open*/
                                                                                                                               //dis rp
                                                                                                                               // vJsonString.Append(" \n" + "                   \"pos\":" + "\"" + tblCDNUR.Rows[i]["pos"].ToString().Trim() + "\",");//RRG Comment

                vJsonString.Append(" \n" + "                   \"nt_num\":" + "\"" + dtCDNUR.Rows[i]["nt_num"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"nt_dt\":" + "\"" + DateTime.Parse(dtCDNUR.Rows[i]["nt_dt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                if (vAmended == true)
                {
                    vJsonString.Append(" \n" + "                   \"ont_num\":" + "\"" + dtCDNUR.Rows[i]["ont_num"].ToString().Trim() + "\",");
                    vJsonString.Append(" \n" + "                   \"ont_dt\":" + "\"" + DateTime.Parse(dtCDNUR.Rows[i]["ont_dt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                }
                vJsonString.Append(" \n" + "                   \"ntty\":" + "\"" + dtCDNUR.Rows[i]["ntty"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"val\":" + dtCDNUR.Rows[i]["val"].ToString().Trim() + ",");
                if (vAmended == false)
                {
                    vJsonString.Append(" \n" + "                   \"typ\":" + "\"" + dtCDNUR.Rows[i]["typ"].ToString().Trim() + "\",");
                }
                vJsonString.Append(" \n" + "                   \"inum\":" + "\"" + dtCDNUR.Rows[i]["inum"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"idt\":" + "\"" + DateTime.Parse(dtCDNUR.Rows[i]["idt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                vJsonString.Append(" \n" + "                   \"p_gst\":" + "\"" + dtCDNUR.Rows[i]["p_gst"].ToString().Trim() + "\",");

                //dis RP
                //   vJsonString.Append(" \n" + "                   \"rsn\":" + "\"" + tblCDNUR.Rows[i]["rsn"].ToString().Trim() + "\",");//RRG Comment




                this.mthJsonGen_CDNUR_itms(dtCDNUR.Rows[i]["nt_num"].ToString().Trim(), tblCDNUR);
                if (vAmended == true)
                {
                    vJsonString.Append(", \n" + "                   \"typ\":" + "\"" + dtCDNUR.Rows[i]["typ"].ToString().Trim() + "\"");
                }
                vJsonString.Append("\n              }");
            }


            vJsonString.Append("\n   ]");//CDNUR Close
        }
        private void mthJsonGen_CDNUR_itms(string vNT_Num, DataTable tblCDNUR)
        {

            DataTable tblItms;
            string[] vFldList = new string[5];
            vFldList[0] = "txval"; vFldList[1] = "rt"; vFldList[2] = "iamt"; vFldList[3] = "csamt"; vFldList[4] = "inum";
            tblItms = vUDQ.SelectDistinct(tblCDNUR, "nt_num = '" + vNT_Num + "'", vFldList);

            vJsonString.Append("\n                   \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n                            {"); } else { vJsonString.Append(", \n                         {"); } /*itms Breaket Open*/
                                                                                                                                                      //   vJsonString.Append(" \n" + "                             \"num\":" + (i + 1).ToString().Trim());
                vJsonString.Append(" \n" + "                             \"num\":" + tblItms.Rows[i]["rt"].ToString().Trim().Replace(".000", "") + "01");
                vJsonString.Append(",\n                                   \"itm_det\":{");  /*itm_det Breaket Open*/
                vJsonString.Append(" \n" + "                                  \"txval\":" + tblItms.Rows[i]["txval"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                                  \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                                  \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");

                //Add RRG
                string cessamt = string.Empty;
                if (tblItms.Rows[i]["csamt"].ToString().Trim() == "0.00")
                {
                    cessamt = "0";
                }
                else
                {
                    cessamt = tblItms.Rows[i]["csamt"].ToString().Trim();

                }

                vJsonString.Append(" \n" + "                                  \"csamt\":" + cessamt);
                //End RRG

                //vJsonString.Append(" \n" + "                                  \"csamt\":" + tblItms.Rows[i]["csamt"].ToString().Trim());Comment RRG
                vJsonString.Append("\n                             }");/*itm_det Breaket Close*/
                vJsonString.Append("\n                          }");/*itms Breaket Close*/
            }
            vJsonString.Append("\n                           ]");

        }

        private void mthJsonGen_EXP()
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "','EXP','0'";
            DataTable tblEXP;
            tblEXP = oDataAccess.GetDataTable(vSqlStr, null, 25);
            //MessageBox.Show(tblEXP.Rows.Count.ToString().Trim());
            DataTable tblExp_Typ;
            tblExp_Typ = vUDQ.SelectDistinct(tblEXP, "", "exp_typ");

            vJsonString.Append(",\n   \"exp\":[");
            for (int i = 0; i <= tblExp_Typ.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*exp_typ Breaket Open*/
                vJsonString.Append(" \n" + "           \"exp_typ\":" + "\"" + tblExp_Typ.Rows[i]["exp_typ"].ToString().Trim() + "\"");
                this.mthJsonGen_EXP_inv(tblExp_Typ.Rows[i]["exp_typ"].ToString().Trim(), tblEXP);
                vJsonString.Append("\n         }");/*exp_typ Breaket Close*/
            }
            vJsonString.Append("\n   ]");
        }
        private void mthJsonGen_EXP_inv(string vExp_Typ, DataTable tblExp_Typ)
        {

            DataTable tblInv;
            string[] vFldList = new string[7];
            vFldList[0] = "exp_typ"; vFldList[1] = "inum"; vFldList[2] = "idt"; vFldList[3] = "val"; vFldList[4] = "sbpcode"; vFldList[5] = "sbnum"; vFldList[6] = "sbdt";
            tblInv = vUDQ.SelectDistinct(tblExp_Typ, "exp_typ = '" + vExp_Typ + "'", vFldList);

            vJsonString.Append(",\n          \"inv\":[");
            for (int i = 0; i <= tblInv.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n               {"); } else { vJsonString.Append(", \n               {"); } /*inv Breaket Open*/
                //RRG Comment 2062018
                //vJsonString.Append(" \n" + "                \"exp_typ\":" + "\"" + tblInv.Rows[i]["exp_typ"].ToString().Trim() + "\",");

                vJsonString.Append(" \n" + "                \"inum\":" + "\"" + tblInv.Rows[i]["inum"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                \"idt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["idt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");

                //RRG Add Start 19/07/2018
                vJsonString.Append(" \n" + "                \"sbpcode\":" + "\"" + tblInv.Rows[i]["sbpcode"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                \"sbnum\":" + "\"" + tblInv.Rows[i]["sbnum"].ToString().Trim() + "\",");
                string SBdate = "";
                if (DateTime.Parse(tblInv.Rows[i]["sbdt"].ToString().Trim()).ToString("dd-MM-yyyy")!= "01-01-1900")
                {
                    SBdate= DateTime.Parse(tblInv.Rows[i]["sbdt"].ToString().Trim()).ToString("dd-MM-yyyy");
                    //vJsonString.Append(" \n" + "                \"sbdt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["sbdt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                }
                else
                {
                    SBdate = "";
                }
                vJsonString.Append(" \n" + "                \"sbdt\":" + "\"" + SBdate + "\",");
                // RRG Add End 19/07/2018

                vJsonString.Append(" \n" + "                \"val\":" + tblInv.Rows[i]["val"].ToString().Trim() + ",");

               

                this.mthJsonGen_EXP_itms(tblInv.Rows[i]["inum"].ToString().Trim(), tblExp_Typ);
                vJsonString.Append("\n               }");/*inv Breaket Close*/
            }
            vJsonString.Append("\n                ]");

        }
        private void mthJsonGen_EXP_itms(string vINum, DataTable tblExp_Typ)
        {

            DataTable tblItms;
            string[] vFldList = new string[7];
            vFldList[0] = "txval"; vFldList[1] = "rt"; vFldList[2] = "camt"; vFldList[3] = "samt"; vFldList[4] = "iamt"; vFldList[5] = "csamt"; vFldList[6] = "inum";
            tblItms = vUDQ.SelectDistinct(tblExp_Typ, "inum = '" + vINum + "'", vFldList);

            vJsonString.Append("\n                \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n                        {"); } else { vJsonString.Append(", \n                     {"); } /*itms Breaket Open*/
                vJsonString.Append(" \n" + "                         \"txval\":" + tblItms.Rows[i]["txval"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                         \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                         \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");

                //RRG Start
                string cessamt = string.Empty;
                if (tblItms.Rows[i]["csamt"].ToString().Trim() == "0.00")
                {
                    cessamt = "0";
                }
                else
                {
                    cessamt = tblItms.Rows[i]["csamt"].ToString().Trim();

                }
                vJsonString.Append(" \n" + "                         \"csamt\":" + cessamt);
                //RRG END


                //vJsonString.Append(" \n" + "                         \"csamt\":" + tblItms.Rows[i]["csamt"].ToString().Trim());


                vJsonString.Append("\n                        }");/*itms Breaket Close*/
            }
            vJsonString.Append("\n                       ]");

        }

        //RRG START

        private void mthJsonGen_EXPA()
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "','EXPA','0'";
            DataTable tblEXP;
            tblEXP = oDataAccess.GetDataTable(vSqlStr, null, 25);
            //MessageBox.Show(tblEXP.Rows.Count.ToString().Trim());
            DataTable tblExp_Typ;
            tblExp_Typ = vUDQ.SelectDistinct(tblEXP, "", "exp_typ");

            vJsonString.Append(",\n   \"exp\":[");
            for (int i = 0; i <= tblExp_Typ.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*exp_typ Breaket Open*/
                vJsonString.Append(" \n" + "           \"exp_typ\":" + "\"" + tblExp_Typ.Rows[i]["exp_typ"].ToString().Trim() + "\"");
                this.mthJsonGen_EXPA_inv(tblExp_Typ.Rows[i]["exp_typ"].ToString().Trim(), tblEXP);
                vJsonString.Append("\n         }");/*exp_typ Breaket Close*/
            }
            vJsonString.Append("\n   ]");
        }
        private void mthJsonGen_EXPA_inv(string vExp_Typ, DataTable tblExp_Typ)
        {

            DataTable tblInv;
            string[] vFldList = new string[9];
            vFldList[0] = "exp_typ"; vFldList[1] = "inum"; vFldList[2] = "idt"; vFldList[3] = "val"; vFldList[4] = "sbpcode"; vFldList[5] = "sbnum"; vFldList[6] = "sbdt"; vFldList[7] = "oinum"; vFldList[8] = "oidt";
            tblInv = vUDQ.SelectDistinct(tblExp_Typ, "exp_typ = '" + vExp_Typ + "'", vFldList);

            vJsonString.Append(",\n          \"inv\":[");
            for (int i = 0; i <= tblInv.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n               {"); } else { vJsonString.Append(", \n               {"); } /*inv Breaket Open*/
                vJsonString.Append(" \n" + "                \"exp_typ\":" + "\"" + tblInv.Rows[i]["exp_typ"].ToString().Trim() + "\",");

                vJsonString.Append(" \n" + "                   \"oinum\":" + "\"" + tblInv.Rows[i]["oinum"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                   \"oidt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["oidt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");

                vJsonString.Append(" \n" + "                \"inum\":" + "\"" + tblInv.Rows[i]["inum"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                \"idt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["idt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                vJsonString.Append(" \n" + "                \"val\":" + tblInv.Rows[i]["val"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                \"sbpcode\":" + "\"" + tblInv.Rows[i]["sbpcode"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                \"sbnum\":" + "\"" + tblInv.Rows[i]["sbnum"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "                \"sbdt\":" + "\"" + DateTime.Parse(tblInv.Rows[i]["sbdt"].ToString().Trim()).ToString("dd-MM-yyyy") + "\",");
                this.mthJsonGen_EXPA_itms(tblInv.Rows[i]["inum"].ToString().Trim(), tblExp_Typ);
                vJsonString.Append("\n               }");/*inv Breaket Close*/
            }
            vJsonString.Append("\n                ]");

        }
        private void mthJsonGen_EXPA_itms(string vINum, DataTable tblExp_Typ)
        {

            DataTable tblItms;
            string[] vFldList = new string[7];
            vFldList[0] = "txval"; vFldList[1] = "rt"; vFldList[2] = "camt"; vFldList[3] = "samt"; vFldList[4] = "iamt"; vFldList[5] = "csamt"; vFldList[6] = "inum";
            tblItms = vUDQ.SelectDistinct(tblExp_Typ, "inum = '" + vINum + "'", vFldList);

            vJsonString.Append("\n                \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n                        {"); } else { vJsonString.Append(", \n                     {"); } /*itms Breaket Open*/
                vJsonString.Append(" \n" + "                         \"txval\":" + tblItms.Rows[i]["txval"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                         \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                         \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                         \"csamt\":" + tblItms.Rows[i]["csamt"].ToString().Trim());
                vJsonString.Append("\n                        }");/*itms Breaket Close*/
            }
            vJsonString.Append("\n                       ]");

        }
        //RRG END

        private void mthJsonGen_AT(Boolean vAmended)
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'AT'" : "'ATA'") + ",0";
            DataTable tblAT;
            tblAT = oDataAccess.GetDataTable(vSqlStr, null, 25);
            // MessageBox.Show(tblAT.Rows.Count.ToString().Trim());
            DataTable tbl_POS_SplyTy;
            string[] vFldList = new string[3];
            vFldList[0] = "pos"; vFldList[1] = "sply_ty"; vFldList[2] = "omon";
            tbl_POS_SplyTy = vUDQ.SelectDistinct(tblAT, "", vFldList);

            if (vAmended == false)
            { vJsonString.Append(",\n   \"at\":["); }
            else
            { vJsonString.Append(",\n   \"ata\":["); }


            for (int i = 0; i <= tbl_POS_SplyTy.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*POS,SplyTy Breaket Open*/

                if (vAmended == true)
                {
                    vJsonString.Append(" \n" + "           \"omon\":" + "\"" + tbl_POS_SplyTy.Rows[i]["omon"].ToString().Trim() + "\"");//RRG ADD
                }
                vJsonString.Append(", \n" + "           \"pos\":" + "\"" + tbl_POS_SplyTy.Rows[i]["pos"].ToString().Trim() + "\"");

                this.mthJsonGen_AT_itms(tbl_POS_SplyTy.Rows[i]["pos"].ToString().Trim(), tbl_POS_SplyTy.Rows[i]["sply_ty"].ToString().Trim(), tblAT);

                vJsonString.Append(", \n" + "           \"sply_ty\":" + "\"" + tbl_POS_SplyTy.Rows[i]["sply_ty"].ToString().Trim() + "\"");

                vJsonString.Append("\n         }");/*POS,SplyTy Breaket Close*/
            }
            vJsonString.Append("\n   ]");
        }
        private void mthJsonGen_AT_itms(string vPos, string vSplyTy, DataTable tblAT)
        {

            DataTable tblItms;
            string[] vFldList = new string[6];
            vFldList[0] = "rt"; vFldList[1] = "ad_amt"; vFldList[2] = "iamt"; vFldList[3] = "camt"; vFldList[4] = "samt"; vFldList[5] = "csamt";
            tblItms = vUDQ.SelectDistinct(tblAT, "pos = '" + vPos + "' and sply_ty='" + vSplyTy + "'", vFldList);

            vJsonString.Append(",\n           \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n              {"); } else { vJsonString.Append(", \n              {"); } /*itms Breaket Open*/
                vJsonString.Append(" \n" + "                 \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                 \"ad_amt\":" + tblItms.Rows[i]["ad_amt"].ToString().Trim() + ",");
                if (((Decimal)tblItms.Rows[i]["iamt"]) > 0)
                {
                    vJsonString.Append(" \n" + "                 \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");
                }
                else
                {
                    vJsonString.Append(" \n" + "                 \"camt\":" + tblItms.Rows[i]["camt"].ToString().Trim() + ",");
                    vJsonString.Append(" \n" + "                 \"samt\":" + tblItms.Rows[i]["samt"].ToString().Trim() + ",");
                }

                //RRG Start
                string cessamt = string.Empty;
                if (tblItms.Rows[i]["csamt"].ToString().Trim() == "0.00")
                {
                    cessamt = "0";
                }
                else
                {
                    cessamt = tblItms.Rows[i]["csamt"].ToString().Trim();

                }
                vJsonString.Append(" \n" + "                         \"csamt\":" + cessamt);
                //RRG END

                //     vJsonString.Append(" \n" + "                 \"csamt\":" + tblItms.Rows[i]["csamt"].ToString().Trim());
                vJsonString.Append("\n              }");/*itms Breaket Close*/
            }

            vJsonString.Append("\n                  ]");

        }
        private void mthJsonGen_TXPD(Boolean vAmended)
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'TXPD'" : "'TXPA'") + ",0";
            DataTable tblTXPD;
            tblTXPD = oDataAccess.GetDataTable(vSqlStr, null, 25);
            // MessageBox.Show(tblTXPD.Rows.Count.ToString().Trim());
            DataTable tbl_POS_SplyTy;
            string[] vFldList = new string[2];
            vFldList[0] = "pos"; vFldList[1] = "sply_ty";
            tbl_POS_SplyTy = vUDQ.SelectDistinct(tblTXPD, "", vFldList);
            if (vAmended == true)
            { vJsonString.Append(",\n   \"txp\":["); }
            else
            { vJsonString.Append(",\n   \"txpd\":["); }


            for (int i = 0; i <= tbl_POS_SplyTy.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*POS,SplyTy Breaket Open*/
                vJsonString.Append(" \n" + "           \"pos\":" + "\"" + tbl_POS_SplyTy.Rows[i]["pos"].ToString().Trim() + "\"");

                this.mthJsonGen_AT_itms(tbl_POS_SplyTy.Rows[i]["pos"].ToString().Trim(), tbl_POS_SplyTy.Rows[i]["sply_ty"].ToString().Trim(), tblTXPD);
                vJsonString.Append(", \n" + "           \"sply_ty\":" + "\"" + tbl_POS_SplyTy.Rows[i]["sply_ty"].ToString().Trim() + "\"");
                vJsonString.Append("\n         }");/*POS,SplyTy Breaket Close*/
            }
            vJsonString.Append("\n   ]");
        }
        private void mthJsonGen_TXPD_itms(string vPos, string vSplyTy, DataTable tblTXPD)
        {

            DataTable tblItms;
            string[] vFldList = new string[6];
            vFldList[0] = "rt"; vFldList[1] = "ad_amt"; vFldList[2] = "iamt"; vFldList[3] = "camt"; vFldList[4] = "samt"; vFldList[5] = "csamt";
            tblItms = vUDQ.SelectDistinct(tblTXPD, "pos = '" + vPos + "' and sply_ty='" + vSplyTy + "'", vFldList);

            vJsonString.Append(",\n           \"itms\":[");
            for (int i = 0; i <= tblItms.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n              {"); } else { vJsonString.Append(", \n              {"); } /*itms Breaket Open*/
                vJsonString.Append(" \n" + "                 \"rt\":" + tblItms.Rows[i]["rt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                 \"ad_amt\":" + tblItms.Rows[i]["ad_amt"].ToString().Trim() + ",");
                if (((Decimal)tblItms.Rows[i]["iamt"]) > 0)
                {
                    vJsonString.Append(" \n" + "                 \"iamt\":" + tblItms.Rows[i]["iamt"].ToString().Trim() + ",");
                }
                else
                {
                    vJsonString.Append(" \n" + "                 \"camt\":" + tblItms.Rows[i]["camt"].ToString().Trim() + ",");
                    vJsonString.Append(" \n" + "                 \"samt\":" + tblItms.Rows[i]["samt"].ToString().Trim() + ",");
                }


                vJsonString.Append(" \n" + "                 \"csamt\":" + tblItms.Rows[i]["csamt"].ToString().Trim());
                vJsonString.Append("\n              }");/*itms Breaket Close*/
            }

            vJsonString.Append("\n                  ]");

        }

        private void mthJsonGen_NIL()
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "','NIL',0";
            DataTable tblNIL;
            tblNIL = oDataAccess.GetDataTable(vSqlStr, null, 25);
            vJsonString.Append(",\n   \"nil\":{"); /*Nill Breaket Open*/
            vJsonString.Append("\n   \"inv\":["); /*Inv Breaket Open*/
            for (int i = 0; i <= tblNIL.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*Inv 2 Breaket Open*/
                vJsonString.Append(" \n" + "           \"sply_ty\":" + "\"" + tblNIL.Rows[i]["sply_ty"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "           \"expt_amt\":" + tblNIL.Rows[i]["expt_amt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "           \"nil_amt\":" + tblNIL.Rows[i]["nil_amt"].ToString().Trim() + ",");

                //RRG Start
                decimal d3 = decimal.Parse(tblNIL.Rows[i]["ngsup_amt"].ToString());
                string ngsup_amt = d3.ToString("0.##");
                vJsonString.Append(" \n" + "           \"ngsup_amt\":" + ngsup_amt);
                //RRG END

                //  vJsonString.Append(" \n" + "           \"ngsup_amt\":" + tblNIL.Rows[i]["ngsup_amt"].ToString().Trim());RRG Comment
                vJsonString.Append("\n         }");/*Inv 2 Breaket Close*/
            }
            vJsonString.Append("\n   ]"); /*Inv Breaket Close*/
            vJsonString.Append("\n   }"); /*Nill Breaket Close*/
        }
        private void mthJsonGen_HSN()
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "','HSN',0";
            DataTable tblNIL;
            tblNIL = oDataAccess.GetDataTable(vSqlStr, null, 25);
            vJsonString.Append(",\n   \"hsn\":{"); /*hsn Breaket Open*/
            vJsonString.Append("\n   \"data\":["); /*data Breaket Open*/
            for (int i = 0; i <= tblNIL.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n         {"); } else { vJsonString.Append(", \n         {"); } /*data 2 Breaket Open*/
                vJsonString.Append(" \n" + "           \"num\":" + (i + 1).ToString().Trim() + ",");

                vJsonString.Append(" \n" + "           \"hsn_sc\":" + "\"" + tblNIL.Rows[i]["hsn_sc"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "           \"desc\":" + "\"" + tblNIL.Rows[i]["desc"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "           \"uqc\":" + "\"" + tblNIL.Rows[i]["uqc"].ToString().Trim() + "\",");
                vJsonString.Append(" \n" + "           \"qty\":" + tblNIL.Rows[i]["qty"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "           \"val\":" + tblNIL.Rows[i]["val"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "           \"txval\":" + tblNIL.Rows[i]["txval"].ToString().Trim() + ",");

                //RRG Add Start
                vJsonString.Append(" \n" + "           \"iamt\":" + tblNIL.Rows[i]["iamt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "           \"camt\":" + tblNIL.Rows[i]["camt"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "           \"samt\":" + tblNIL.Rows[i]["samt"].ToString().Trim() + ",");

                string cessamt = string.Empty;
                if (tblNIL.Rows[i]["csamt"].ToString().Trim() == "0.00")
                {
                    cessamt = "0";
                }
                else
                {
                    cessamt = tblNIL.Rows[i]["csamt"].ToString().Trim();

                }
                vJsonString.Append(" \n" + "                         \"csamt\":" + cessamt);
                //RRG Add End

                //RRG Comment start
                //if (((Decimal)tblNIL.Rows[i]["iamt"]) > 0)
                //{
                //    vJsonString.Append(" \n" + "           \"iamt\":" + tblNIL.Rows[i]["iamt"].ToString().Trim() + ",");
                //}
                //else
                //{
                //    vJsonString.Append(" \n" + "           \"camt\":" + tblNIL.Rows[i]["camt"].ToString().Trim() + ",");
                //    vJsonString.Append(" \n" + "           \"samt\":" + tblNIL.Rows[i]["samt"].ToString().Trim() + ",");
                //}
                //  vJsonString.Append(" \n" + "           \"csamt\":" + tblNIL.Rows[i]["csamt"].ToString().Trim());
                //RRG Comment End


                vJsonString.Append("\n         }");/*data 2 Breaket Close*/
            }
            vJsonString.Append("\n   ]"); /*data Breaket Close*/
            vJsonString.Append("\n   }"); /*hsn Breaket Close*/
        }
        private string getDocnum(string doctype)
        {
            if (doctype == "Invoices for outward supply")
            {
                return "1";
            }
            else if (doctype == "Invoices for inward supply from unregistered person")
            {
                return "2";
            }
            else if (doctype == "Debit Note")
            {
                return "4";
            }
            else if (doctype == "Credit Note")
            {
                return "5";
            }
            else if (doctype == "Receipt voucher")
            {
                return "6";
            }
            else if (doctype == "Payment Voucher")
            {
                return "7";
            }
            else if (doctype == "Refund Voucher")
            {
                return "8";
            }
            else if (doctype == "Delivery Challan for job work")
            {
                return "9";
            }
            else if (doctype == "Delivery Challan for supply on approval")
            {
                return "10";
            }
            else if (doctype == "Delivery Challan in cases other than by way of supply (excluding at S no. 9 to 11)")
            {
                return "12";
            }
            else
            {
                return "1";
            }
        }
        private void mthJsonGen_DocIssue()
        {
            vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "','DOCISSUE',0";
            DataTable tblDocIssue;
            tblDocIssue = oDataAccess.GetDataTable(vSqlStr, null, 25);
            //  MessageBox.Show(tblDocIssue.Rows.Count.ToString().Trim());
            DataTable tblDoc_Typ;
            tblDoc_Typ = vUDQ.SelectDistinct(tblDocIssue, "", "doc_typ");

            vJsonString.Append(",\n  \"doc_issue\":{"); /*DocIssue Breaket Open*/
            vJsonString.Append("\n    \"doc_det\":["); /*doc_det Breaket Open*/
            for (int i = 0; i <= tblDoc_Typ.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n          {"); } else { vJsonString.Append(", \n         {"); } /*doc_det 2 Breaket Open*/
                string docnum = getDocnum(tblDoc_Typ.Rows[i]["doc_typ"].ToString().Trim());
                vJsonString.Append(" \n" + "           \"doc_num\":" + docnum + " ,");//Add Rupesh G.
                //  vJsonString.Append(" \n" + "           \"doc_num\":" + (i + 1).ToString().Trim() + " ,");//Comment Rupesh G.
                vJsonString.Append(" \n" + "           \"doc_typ\":" + "\"" + tblDoc_Typ.Rows[i]["doc_typ"].ToString().Trim() + "\" ,");
                this.mthJsonGen_DocIssue_Docs(tblDoc_Typ.Rows[i]["doc_typ"].ToString().Trim(), tblDocIssue);
                vJsonString.Append("\n         }");/*Ctin Breaket Close*/
            }
            vJsonString.Append("\n       ]"); /*doc_det  2 Breaket Close*/
            vJsonString.Append("\n     }"); /*DocIssue Breaket Close*/

        }
        private void mthJsonGen_DocIssue_Docs(string vDocType, DataTable tblDocIssue)
        {

            DataTable tblDocs;
            string[] vFldList = new string[5];
            vFldList[0] = "from"; vFldList[1] = "to"; vFldList[2] = "totnum"; vFldList[3] = "cancel"; vFldList[4] = "net_issue";
            tblDocs = vUDQ.SelectDistinct(tblDocIssue, "doc_typ = '" + vDocType + "'", vFldList);

            vJsonString.Append("\n           \"docs\":[");/*docs Breaket Open*/
            for (int i = 0; i <= tblDocs.Rows.Count - 1; i++)
            {
                if (i == 0) { vJsonString.Append("\n                    {"); } else { vJsonString.Append(", \n                 {"); } /*docs Breaket 2 Open*/
                vJsonString.Append(" \n" + "                      \"num\":" + (i + 1).ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                      \"to\":" + "\"" + tblDocs.Rows[i]["to"].ToString().Trim() + "\" ,");
                vJsonString.Append(" \n" + "                      \"from\":" + "\"" + tblDocs.Rows[i]["from"].ToString().Trim() + "\" ,");
                vJsonString.Append(" \n" + "                      \"totnum\":" + tblDocs.Rows[i]["totnum"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                      \"cancel\":" + tblDocs.Rows[i]["cancel"].ToString().Trim() + ",");
                vJsonString.Append(" \n" + "                      \"net_issue\":" + tblDocs.Rows[i]["net_issue"].ToString().Trim());
                vJsonString.Append("\n                    }");/*docs Breaket 2 Close*/
            }
            vJsonString.Append("\n                  ]"); /*docs Breaket Open*/

        }
        private void btnJsonFile_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog vFolderBrowserDialog1 = new FolderBrowserDialog();
            vFolderBrowserDialog1.ShowDialog();
            txtJsonFile.Text = @"\" + "GSTR1_" + vDsCommon.Tables["Company"].Rows[0]["GSTIN"].ToString().Trim() + "_" + this.txtFYear.Text.Trim() + "_" + this.txtPeriod.Text.Trim() + "_" + DateTime.Now.Date.ToString("dd/MM/yy") + "_" + DateTime.Now.ToString("hh:mm:ss tt");
            txtJsonFile.Text = txtJsonFile.Text.Replace(".", "").Replace("-", "").Replace("/", "").Replace(":", "").Replace("__", "_").Replace(" ", "") + ".Json";
            txtJsonFile.Text = vFolderBrowserDialog1.SelectedPath + txtJsonFile.Text;
        }
        private void mthControlSet()
        {
            string fName = this.pAppPath + @"\bmp\pickup.gif";
            if (File.Exists(fName) == true)
            {
                this.btnFYear.Image = Image.FromFile(fName);
                this.btnPeriod.Image = Image.FromFile(fName);
                this.btnSection.Image = Image.FromFile(fName);

            }
            fName = this.pAppPath + @"\bmp\loc-on.gif";
            if (File.Exists(fName) == true)
            {
                this.btnJson.Image = Image.FromFile(fName);
            }
            fName = this.pAppPath + @"\bmp\report_rtf.gif";
            if (File.Exists(fName) == true)
            {
                this.btnJsonFile.Image = Image.FromFile(fName);
            }
        }
        private void SetFormColor()
        {
            string colorCode = string.Empty;
            Color myColor = Color.Coral;

            if (vDsCommon.Tables["company"].Rows.Count > 0)
            {
                colorCode = vDsCommon.Tables["company"].Rows[0]["vcolor"].ToString().Trim();
            }

            if (vDsCommon.Tables["company"].Rows.Count > 0)
            {
                this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                myColor = Color.FromArgb(Convert.ToInt32(colorCode.Trim()));
            }
            this.BackColor = Color.FromArgb(myColor.R, myColor.R, myColor.G, myColor.B);
            foreach (Control ctrl in this.Controls)
            {
                //ctrl.BackColor = Color.FromArgb(myColor.R, myColor.R, myColor.G, myColor.B);
            }


        }
        private void mthDsCommon()
        {
            string vL_Yn = "";
            vDsCommon = new DataSet();
            DataTable company = new DataTable();
            company.TableName = "company";
            vSqlStr = "Select * From vudyog..Co_Mast where CompId=" + this.pCompId.ToString().Trim();
            company = oDataAccess.GetDataTable(vSqlStr, null, 25);
            if (vL_Yn == "")
            {
                vL_Yn = ((DateTime)company.Rows[0]["Sta_Dt"]).Year.ToString().Trim() + "-" + ((DateTime)company.Rows[0]["End_Dt"]).Year.ToString().Trim();
            }
            vDsCommon.Tables.Add(company);
            vDsCommon.Tables[0].TableName = "company";
            DataTable tblCoAdditional = new DataTable();
            tblCoAdditional.TableName = "coadditional";
            vSqlStr = "Select Top 1 * From Manufact";
            tblCoAdditional = oDataAccess.GetDataTable(vSqlStr, null, 25);
            vDsCommon.Tables.Add(tblCoAdditional);
            vDsCommon.Tables[1].TableName = "coadditional";

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            short vPrintOption = 2;
            String vPrintPara = "";
            this.mthGetDateRange(ref vFrmDate, ref vToDate);
            //vSqlStr = "SET DATEFormat DMY Execute [USP_REP_GSTR1_JSON]" + " '" + vFrmDate + "','" + vToDate + "'," + (vAmended == false ? "'B2B'" : "'B2BA'");


            vPrintPara = " '" + vFrmDate + "','" + vToDate + "'," + "'" + vSecCode + "',1";
            //vPrintPara = "'','','','','','','','','',0,0,'','','','','','','','',''";
            //MessageBox.Show(vPrintPara);
            string vRepGroup = "GSTR-1 JSON Report";
            //vRepGroup = "Account Master Report";
            udReportList.cReportList oPrint = new udReportList.cReportList();
            oPrint.pDsCommon = this.vDsCommon;
            oPrint.pServerName = this.pServerName;
            oPrint.pComDbnm = this.pComDbnm;
            oPrint.pUserId = this.pUserId;
            oPrint.pPassword = this.pPassword;
            oPrint.pAppPath = this.pAppPath;
            oPrint.pPApplText = this.pPApplText;
            oPrint.pPara = this.pPara;
            oPrint.pRepGroup = vRepGroup;
            oPrint.pSpPara = vPrintPara;
            //   oPrint.pSpPara = vPrintPara + ",'" + this.pAppUerName.Trim() + "'"; /*Ramya 01/11/12*/
            oPrint.pPrintOption = vPrintOption;
            oPrint.Main();


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
                //MessageBox.Show("Can't proceed,Main Application " + this.pPApplText + " is closed", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //Application.Exit();
            }
        }
        private void mInsertProcessIdRecord()/*Added Rup 07/03/2011*/
        {

            DataSet dsData = new DataSet();
            string sqlstr;
            int pi;
            pi = Process.GetCurrentProcess().Id;
            cAppName = "ugstEWB_ItMast_JSON.exe";
            cAppPId = Convert.ToString(Process.GetCurrentProcess().Id);
            sqlstr = " Set DateFormat dmy insert into vudyog..ExtApplLog (pApplCode,CallDate,pApplNm,pApplId,pApplDesc,cApplNm,cApplId,cApplDesc) Values('" + this.pPApplCode + "','" + DateTime.Now.Date.ToString().Trim() + "','" + this.pPApplName + "'," + this.pPApplPID + ",'" + this.pPApplText + "','" + cAppName + "'," + cAppPId + ",'" + this.Text.Trim() + "')";
            oDataAccess.ExecuteSQLStatement(sqlstr, null, vTimeOut, true);
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            mDeleteProcessIdRecord();
        }



        private void mDeleteProcessIdRecord()/*Added Rup 07/03/2011*/
        {
            if (string.IsNullOrEmpty(this.pPApplName) || this.pPApplPID == 0 || string.IsNullOrEmpty(this.cAppName) || string.IsNullOrEmpty(this.cAppPId))
            {
                return;
            }
            DataSet dsData = new DataSet();
            string sqlstr;
            sqlstr = " Delete from vudyog..ExtApplLog where pApplNm='" + this.pPApplName + "' and pApplId=" + this.pPApplPID + " and cApplNm= '" + cAppName + "' and cApplId= " + cAppPId;
            oDataAccess.ExecuteSQLStatement(sqlstr, null, vTimeOut, true);
        }
    }
}
