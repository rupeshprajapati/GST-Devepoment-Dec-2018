﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace CreditAvailable
{
    public partial class Form1 : uBaseForm.FrmBaseForm
    {
        DataAccess_Net.clsDataAccess oDataAccess;

        // DataTable Define
        DataTable dtCredit = new DataTable();/* DataTable For dgvCredit Gridview Data*/
        DataTable dtPartyName;/* DataTable For Party Textbox search Data*/
        DataTable dtGroupDetail;/* DataTable For accountname and group  Data*/
        DataTable dtGroup;/* DataTable For group  Data*/
        DataTable dtCredit1 = new DataTable();/* Copy dtCredit Table*/
        DataTable dtmainall_vw = new DataTable();
        DataTable dtTbl = new DataTable();
        String cAppPId, cAppName, lblcode = string.Empty;
        int edit = 0;
        string mode = string.Empty; //Rupesh G. Add 11-05-2018 Bug No.31401
        decimal icgst, isgst, iigst, icess;
        public Form1(string[] args)
        {
            InitializeComponent();
            this.pDisableCloseBtn = true;  /* close disable  */
            this.MaximizeBox = false; /* Maximize disable  */
            this.WindowState = FormWindowState.Maximized;/* Full Screen */
            this.dgvCredit.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystrokeOrF2;

            this.pPApplPID = 0;
            this.pPara = args;
            //this.pFrmCaption = "Eligible ITC (Goods/Services)";  //Commented By Prajakta B. on 14052018 for ERP-Installer
            this.pFrmCaption = "In-Eligible ITC (Goods/Services)"; //Modified By Prajakta B. on 14052018 for ERP-Installer
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
            timer1.Enabled = true;

            CultureInfo ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        private void mInsertProcessIdRecord()
        {

            DataSet dsData = new DataSet();
            string sqlstr;
            int pi;
            pi = Process.GetCurrentProcess().Id;
            //cAppName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            cAppName = "CreditAvailable.exe";
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

        public void lbltxt()
        {
            if (comboBox1.SelectedIndex == 0)
            {
                lblcode = "HSN Code";
                dgvCredit.Columns[15].HeaderText = "HSN Code";  //Added By Prajakta 04052018  

            }
            else if (comboBox1.SelectedIndex == 1)
            {
                //Rupesh G. Add 1-Nov-2017 Bug No.30750
                //lblcode = "HSN Code";  //Commented By Prajakta 04052018
                lblcode = "Service Tarrif Code";  //Modified By Prajakta 04052018
                // lblcode = "SAC Code";//Rupesh G. comment 1-Nov-2017 Bug No.30750
                dgvCredit.Columns[15].HeaderText = "Service Tarrif Code";  //Added By Prajakta 04052018  
            }
            label9.Text = lblcode;
        }

        public void set_dtCredit()
        {
            string from, to, supply;

            from = dateTimePicker1.Value.Date.AddDays(-1).ToString();
            to = dateTimePicker2.Value.Date.ToString();
            supply = comboBox1.SelectedIndex.ToString();

            dtCredit.Columns.Clear();
            dtCredit.Clear();

            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1)
            {
                string SqlStr = "Set dateformat dmy EXEC SP_CREDIT_AVAILABILITY @from = '" + from + "',@to = '" + to + "',@supply = '" + supply + "';";
                dtCredit = oDataAccess.GetDataSet(SqlStr, null, 20).Tables[0];

            }
            set_Grid();
            dtCredit1 = dtCredit;
        }

        public void set_Grid()
        {
            dgvCredit.AutoGenerateColumns = false;
            dgvCredit.DataSource = dtCredit;
            dgvCredit.Columns[0].DataPropertyName = "tran_cd";
            dgvCredit.Columns[1].DataPropertyName = "inv_no";
            dgvCredit.Columns[2].DataPropertyName = "item";
            dgvCredit.Columns[3].DataPropertyName = "date";
            dgvCredit.Columns[4].DataPropertyName = "pinvno";
            dgvCredit.Columns[5].DataPropertyName = "pinvdt";
            dgvCredit.Columns[6].DataPropertyName = "net_amt";
            dgvCredit.Columns[7].DataPropertyName = "cgst_amt";
            dgvCredit.Columns[8].DataPropertyName = "sgst_amt";
            dgvCredit.Columns[9].DataPropertyName = "igst_amt";
            dgvCredit.Columns[10].DataPropertyName = "Credit_Availability";
            dgvCredit.Columns[14].DataPropertyName = "IN_ITC_SEC";
            dgvCredit.Columns[15].DataPropertyName = "HSNCODE";
            dgvCredit.Columns[16].DataPropertyName = "party_nm";
            dgvCredit.Columns[17].DataPropertyName = "linerule";
            dgvCredit.Columns[18].DataPropertyName = "COMPCESS";
            dgvCredit.Columns[20].DataPropertyName = "entry_ty";
            dgvCredit.Columns[21].DataPropertyName = "itserial";
        }

        public void editGrid()
        {
            if (edit == 1)
            {


                foreach (DataGridViewRow dr in dgvCredit.Rows)
                {
                    if (dr.Cells[10].Value.ToString().ToLower() == "false")
                    {

                        dr.Cells[11].ReadOnly = false;
                        dr.Cells[12].ReadOnly = false;
                        dr.Cells[13].ReadOnly = false;
                        dr.Cells[19].ReadOnly = false;
                    }
                    else
                    {
                        dr.Cells[11].ReadOnly = true;
                        dr.Cells[12].ReadOnly = true;
                        dr.Cells[13].ReadOnly = true;
                        dr.Cells[19].ReadOnly = true;
                    }


                }
            }
        }
        public void gridColumn_Display()
        {
            dgvCredit.Columns[1].DisplayIndex = 1;
            dgvCredit.Columns[2].DisplayIndex = 5;
            dgvCredit.Columns[3].DisplayIndex = 2;
            dgvCredit.Columns[4].DisplayIndex = 3;
            dgvCredit.Columns[5].DisplayIndex = 4;
            dgvCredit.Columns[15].DisplayIndex = 6;
            dgvCredit.Columns[6].DisplayIndex = 7;
            dgvCredit.Columns[7].DisplayIndex = 9;
            dgvCredit.Columns[8].DisplayIndex = 10;
            dgvCredit.Columns[9].DisplayIndex = 11;
            dgvCredit.Columns[10].DisplayIndex = 8;

            dgvCredit.Columns[18].DisplayIndex = 12;

            dgvCredit.Columns[11].DisplayIndex = 13;
            dgvCredit.Columns[12].DisplayIndex = 14;
            dgvCredit.Columns[13].DisplayIndex = 15;
            dgvCredit.Columns[19].DisplayIndex = 16;
            dgvCredit.Columns[14].DisplayIndex = 17;


        }

        public void FormSetting()
        {
            gridColumn_Display();

            //Form dgvCredit Resize settings
            dgvCredit.Width = this.Width - 40;
            dgvCredit.Height = this.Height - 260;
            dgvCredit.Columns[10].Frozen = true;

            //Form panel1 Resize settings
            panel1.Location = new Point(12, this.Height - 152);
            panel1.Width = dgvCredit.Width;

            //Form panel1 Resize settings
            panel2.Location = new Point(this.Width - 700, -1);
            panel2.Width = this.Width - 40;

            //Form lblNote Resize settings
            lblNote.Location = new Point(12, this.Height - 100);
            dgvCredit.Columns[1].Width = 120;
            foreach (DataGridViewColumn column in dgvCredit.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        public void set_Unavailable()
        {
            int i = 0;
            string e_ty, t_cd, isr, icgst, isgst, iigst, icess;
            foreach (DataGridViewRow dr1 in dgvCredit.Rows)
            {
                //if (dr1.Cells["Column11"].Value.ToString() == "False")
                //{

                dgvCredit.Rows[i].Cells[11].Value = dgvCredit.Rows[i].Cells[7].Value;
                dgvCredit.Rows[i].Cells[12].Value = dgvCredit.Rows[i].Cells[8].Value;
                dgvCredit.Rows[i].Cells[13].Value = dgvCredit.Rows[i].Cells[9].Value;
                dgvCredit.Rows[i].Cells[19].Value = dgvCredit.Rows[i].Cells[18].Value;

                e_ty = dr1.Cells[20].Value.ToString();
                t_cd = dr1.Cells[0].Value.ToString();
                isr = dr1.Cells[21].Value.ToString();
                icgst = dr1.Cells[11].Value.ToString();
                isgst = dr1.Cells[12].Value.ToString();
                iigst = dr1.Cells[13].Value.ToString();
                icess = dr1.Cells[19].Value.ToString();

                //   MessageBox.Show(e_ty + " " + t_cd + " " + isr+" "+icgst+" "+isgst+" "+iigst+" "+icess);
                DataTable epayment = new DataTable();
                string SqlStr1 = "select * from Epayment where itserial='" + isr + "' and entry_ty='" + e_ty + "' and tran_cd=" + t_cd + " ";
                epayment = oDataAccess.GetDataSet(SqlStr1, null, 20).Tables[0];

                //    MessageBox.Show(""+epayment.Rows[0]["ICGST_AMT"].ToString());
                if (epayment.Rows.Count > 0)
                {
                    dgvCredit.Rows[i].Cells[11].Value = epayment.Rows[0]["ICGST_AMT"].ToString();
                    dgvCredit.Rows[i].Cells[12].Value = epayment.Rows[0]["ISGST_AMT"].ToString(); ;
                    dgvCredit.Rows[i].Cells[13].Value = epayment.Rows[0]["IIGST_AMT"].ToString(); ;
                    dgvCredit.Rows[i].Cells[19].Value = epayment.Rows[0]["ICESS_AMT"].ToString(); ;
                }
                else
                {
                    if (dr1.Cells["Column11"].Value.ToString() == "True")
                    {
                        dgvCredit.Rows[i].Cells[11].Value = "0.00";
                        dgvCredit.Rows[i].Cells[12].Value = "0.00";
                        dgvCredit.Rows[i].Cells[13].Value = "0.00";
                        dgvCredit.Rows[i].Cells[14].Value = "";
                        dgvCredit.Rows[i].Cells[19].Value = "0.00";
                    }
                }
                //}
                //else
                //{
                //    dgvCredit.Rows[i].Cells[11].Value = "0.00";
                //    dgvCredit.Rows[i].Cells[12].Value = "0.00";
                //    dgvCredit.Rows[i].Cells[13].Value = "0.00";
                //    dgvCredit.Rows[i].Cells[19].Value = "0.00";
                //}
                i++;
            }
        }

        public void set_PartyName()
        {
            dtPartyName = new DataTable();/* DataTable For Party Textbox search Data*/
            dtGroupDetail = new DataTable();/* DataTable For accountname and group  Data*/
            dtGroup = new DataTable();/* DataTable For group  Data*/
            dtPartyName.Clear();
            dtPartyName.Columns.Clear();
            dtPartyName.Columns.Add("partyname");
            dtGroupDetail.Clear();
            dtGroupDetail.Columns.Clear();
            dtGroup.Clear();
            dtGroup.Columns.Clear();
            string strSQL = "select ac_name ,[group] from AC_MAST where ac_name in (select party_nm from epitem)";
            dtGroupDetail = oDataAccess.GetDataSet(strSQL, null, 20).Tables[0];
            foreach (DataRow dr in dtGroupDetail.Rows)
            {
                string strSQL1 = "EXECUTE [USP_ENT_GET_PARENT_ACGROUP] '" + dr[1].ToString() + "'";
                dtGroup = oDataAccess.GetDataSet(strSQL1, null, 20).Tables[0];
                foreach (DataRow dr1 in dtGroup.Rows)
                {
                    if (dr1[2].ToString().Trim() == "SUNDRY CREDITORS")
                    {
                        dtPartyName.Rows.Add(dr[0].ToString());
                    }
                }

            }
        }

        public void set_amount()
        {
            int i = 0;
            double cgst = 0.00, sgst = 0.00, igst = 0.00, cess = 0.00, g_cgst, g_sgst, g_igst, g_cess;
            foreach (DataGridViewRow dr in dgvCredit.Rows)
            {
                g_cgst = Convert.ToDouble(dgvCredit.Rows[i].Cells[11].Value.ToString());
                g_sgst = Convert.ToDouble(dgvCredit.Rows[i].Cells[12].Value.ToString());
                g_igst = Convert.ToDouble(dgvCredit.Rows[i].Cells[13].Value.ToString());
                g_cess = Convert.ToDouble(dgvCredit.Rows[i].Cells[19].Value.ToString());
                cgst = cgst + g_cgst;
                sgst = sgst + g_sgst;
                igst = igst + g_igst;
                cess = cess + g_cess;
                i++;
            }
            textBox2.Text = Convert.ToDecimal(cgst).ToString("0.00");
            textBox3.Text = Convert.ToDecimal(sgst).ToString("0.00");
            textBox4.Text = Convert.ToDecimal(igst).ToString("0.00");
            textBox6.Text = Convert.ToDecimal(cess).ToString("0.00");
        }

        public void search()
        {
            if (textBox5.Text == "" && textBox1.Text != "")
            {
                pageLoad(1);
                DataRow[] result = dtCredit1.Select("party_nm='" + textBox1.Text + "' ");
                if (result.Length > 0)
                {
                    dtCredit = new DataTable();
                    dtCredit.Clear();
                    dtCredit.Columns.Clear();
                    dtCredit = result.CopyToDataTable();
                    set_Grid();
                }
            }
            else if (textBox5.Text != "" && textBox1.Text == "")
            {
                pageLoad(1);
                DataRow[] result = dtCredit1.Select("HSNCODE='" + textBox5.Text + "' ");
                if (result.Length > 0)
                {
                    dtCredit = new DataTable();
                    dtCredit.Clear();
                    dtCredit.Columns.Clear();
                    dtCredit = result.CopyToDataTable();
                    set_Grid();
                }
            }
            else if (textBox5.Text != "" && textBox1.Text != "")
            {
                pageLoad(1);
                DataRow[] result = dtCredit1.Select(" HSNCODE='" + textBox5.Text + "' and party_nm='" + textBox1.Text + "' ");
                if (result.Length > 0)
                {
                    dtCredit = new DataTable();
                    dtCredit.Clear();
                    dtCredit.Columns.Clear();
                    dtCredit = result.CopyToDataTable();
                    set_Grid();
                }
                else
                {
                    dgvCredit.DataSource = null;
                    MessageBox.Show("Records not found", this.pPApplName);
                }
            }
            set_Unavailable();
        }

        public void pageLoad(int i)
        {
            if (i == 0)
            {
                textBox1.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = DateTime.Now;
                set_dtCredit();
                set_Unavailable();
                set_amount();
            }
            else
            {
                set_dtCredit();
                set_Unavailable();
                set_amount();
            }
            if (dtCredit.Rows.Count > 0)
            {

                btnEdit.Enabled = true;
                btnCancel.Enabled = true;

            }
            else
            {
                btnCancel.Enabled = false;
                btnEdit.Enabled = false;
            }

            if (edit == 1)
            {
                btnEdit.Enabled = false;
                btnCancel.Enabled = true;

            }
            dgvCredit.AutoResizeColumns();
            dgvCredit.Columns[3].Width = 74;
            dgvCredit.Columns[15].Width = 91;


        }

        public void party_Notification()
        {
            try
            {
                // set_PartyName();
                string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                DataSet tDs = new DataSet();
                DataView view = new DataView(dtCredit1);
                DataTable distinctValues = new DataTable();
                distinctValues = view.ToTable(true, "party_nm");
                DataView dvw = distinctValues.DefaultView;
                VForText = "Select Party Name";
                vSearchCol = "party_nm";
                vDisplayColumnList = "party_nm:Party Name";
                vReturnCol = "party_nm";
                udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
                oSelectPop.pdataview = dvw;
                oSelectPop.pformtext = VForText;
                oSelectPop.psearchcol = vSearchCol;
                oSelectPop.Width = 30;
                oSelectPop.Icon = pFrmIcon;
                oSelectPop.pDisplayColumnList = vDisplayColumnList;
                oSelectPop.pRetcolList = vReturnCol;

                oSelectPop.ShowDialog();

                if (oSelectPop.pReturnArray != null)
                {
                    textBox1.Text = oSelectPop.pReturnArray[0];
                }

                pageLoad(1);
            }
            catch (Exception)
            {
            }
        }

        public void hsn_Notification()
        {
            try
            {
                string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                DataSet tDs = new DataSet();
                DataView view = new DataView(dtCredit1);
                DataTable distinctValues = new DataTable();
                distinctValues = view.ToTable(true, "HSNCODE");
                DataView dvw = distinctValues.DefaultView;
                if (comboBox1.SelectedIndex == 0)
                {
                    VForText = "Select HSN Code";
                    //vSearchCol = "dtCredit";  //Commented by Priyanka B on 29032019 for Bug-32366
                    vSearchCol = "HSNCODE";  //Modified by Priyanka B on 29032019 for Bug-32366
                    vDisplayColumnList = "HSNCODE:" + lblcode + "";
                    vReturnCol = "HSNCODE";
                }
                else
                {
                    VForText = "Select Service Tarrif Code";
                    //vSearchCol = "dtCredit";  //Commented by Priyanka B on 29032019 for Bug-32366
                    vSearchCol = "HSNCODE";  //Modified by Priyanka B on 29032019 for Bug-32366
                    //vDisplayColumnList = "Service Tarrif Code:" + lblcode + "";  //Commented by Priyanka B on 29032019 for Bug-32366
                    vDisplayColumnList = "HSNCODE:" + lblcode + "";  //Modified by Priyanka B on 29032019 for Bug-32366
                    //vReturnCol = "Service Tarrif Code";  //Commented by Priyanka B on 29032019 for Bug-32366
                    vReturnCol = "HSNCODE";  //Modified by Priyanka B on 29032019 for Bug-32366
                }
                udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
                oSelectPop.pdataview = dvw;
                oSelectPop.pformtext = VForText;
                oSelectPop.psearchcol = vSearchCol;
                oSelectPop.Width = 30;
                oSelectPop.Icon = pFrmIcon;
                oSelectPop.pDisplayColumnList = vDisplayColumnList;
                oSelectPop.pRetcolList = vReturnCol;
                oSelectPop.ShowDialog();
                if (oSelectPop.pReturnArray != null)
                {
                    textBox5.Text = oSelectPop.pReturnArray[0];
                }
            }
            catch (Exception)
            {

            }

        }

        public bool section_Notification()
        {
            string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
            DataSet tDs = new DataSet();
            string SqlStr1 = "select descr as 'Ineligible ITC Section' from Reversalmast where Category = 'ITC Ineligible' and isactive = 0";
            tDs = oDataAccess.GetDataSet(SqlStr1, null, 20);
            DataView dvw = tDs.Tables[0].DefaultView;
            VForText = "Select Ineligible ITC Section";
            vSearchCol = "Ineligible ITC Section";
            vDisplayColumnList = "Ineligible ITC Section:Ineligible ITC Section";
            vReturnCol = "Ineligible ITC Section";
            udSelectPop.SELECTPOPUP oSelectPop = new udSelectPop.SELECTPOPUP();
            oSelectPop.pdataview = dvw;
            oSelectPop.pformtext = VForText;
            oSelectPop.psearchcol = vSearchCol;
            oSelectPop.Width = 30;
            oSelectPop.Icon = pFrmIcon;
            oSelectPop.pDisplayColumnList = vDisplayColumnList;
            oSelectPop.pRetcolList = vReturnCol;
            oSelectPop.ShowDialog();
            if (oSelectPop.pReturnArray != null)
            {
                dgvCredit.CurrentRow.Cells[14].Value = oSelectPop.pReturnArray[0].ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
        //Add Rupesh G. 26 Sep 2017 
        public string getTableName(string entry_ty)
        {
            dtTbl.Clear();
            string SqlStr1 = "Select entry_ty,mEntry=case when Bcode_nm<>'' then Bcode_nm else (case when ext_vou=1 then '' else entry_ty end) end From Lcode where Entry_ty='" + entry_ty + "'";
            dtTbl = oDataAccess.GetDataSet(SqlStr1, null, 20).Tables[0];
            string tblname = dtTbl.Rows[0][1].ToString() + "ITEM";
            return tblname.ToString().Trim();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormSetting();
            // pageLoad(0);
            textBox1.Visible = true;
            panel1.Visible = true;
            dgvCredit.Columns[10].ReadOnly = true;
            //rrg
            dgvCredit.Columns[11].ReadOnly = true;
            dgvCredit.Columns[12].ReadOnly = true;
            dgvCredit.Columns[13].ReadOnly = true;
            dgvCredit.Columns[19].ReadOnly = true;

            this.mInsertProcessIdRecord();
            dgvCredit.AutoResizeColumns();
            dgvCredit.Columns[3].Width = 74;
            dgvCredit.Columns[15].Width = 91;
            lblNote.ForeColor = Color.Red;
            dgvCredit.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            dgvCredit.AllowUserToResizeRows = false;
            dgvCredit.AllowUserToOrderColumns = true;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            edit = 0;
            this.Close();
            mDeleteProcessIdRecord();
        }

        private void dgvCredit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 10 && dgvCredit.Columns[10].ReadOnly == false)
            {
                bool itc = (bool)dgvCredit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                string val = dgvCredit.Rows[e.RowIndex].Cells[20].Value.ToString() + dgvCredit.Rows[e.RowIndex].Cells[0].Value.ToString();
                string entry_ty_mainall_vw = string.Empty;
                string SqlStr = "select distinct m.entry_ty,l.code_nm from mainall_vw m inner join LCODE l on(m.ENTRY_TY=l.Entry_ty)Where m.Entry_ty+Convert(varchar(10),m.Tran_cd)='" + val + "' or m.Entry_all+Convert(varchar(10),m.Main_tran)='" + val + "'";
                dtmainall_vw = oDataAccess.GetDataSet(SqlStr, null, 20).Tables[0];
                if (dtmainall_vw.Rows.Count > 1)
                {
                    foreach (DataRow dr in dtmainall_vw.Rows)
                    {
                        entry_ty_mainall_vw = dr[1].ToString().Trim() + "(" + dr[0].ToString().Trim() + ")" + ",";
                    }
                    entry_ty_mainall_vw = entry_ty_mainall_vw.TrimEnd(entry_ty_mainall_vw[entry_ty_mainall_vw.Length - 1]);
                }
                else if (dtmainall_vw.Rows.Count == 1)
                {
                    entry_ty_mainall_vw = dtmainall_vw.Rows[0][1].ToString().Trim() + "(" + dtmainall_vw.Rows[0][0].ToString().Trim() + ")";
                }

                //if (dtmainall_vw.Rows.Count == 0)
                //{
                if (dgvCredit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "True" && dgvCredit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                {
                    dgvCredit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                    this.dgvCredit.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
                else
                {
                    dgvCredit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
                    this.dgvCredit.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
                if (dgvCredit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "False")
                {
                    dgvCredit.Columns[11].ReadOnly = false;
                    dgvCredit.Columns[12].ReadOnly = false;
                    dgvCredit.Columns[13].ReadOnly = false;
                    dgvCredit.Columns[19].ReadOnly = false;

                    bool result = section_Notification();
                    dgvCredit.Rows[e.RowIndex].Cells[11].Value = dgvCredit.Rows[e.RowIndex].Cells[7].Value;
                    dgvCredit.Rows[e.RowIndex].Cells[12].Value = dgvCredit.Rows[e.RowIndex].Cells[8].Value;
                    dgvCredit.Rows[e.RowIndex].Cells[13].Value = dgvCredit.Rows[e.RowIndex].Cells[9].Value;
                    dgvCredit.Rows[e.RowIndex].Cells[19].Value = dgvCredit.Rows[e.RowIndex].Cells[18].Value;
                    if (result == false)
                    {
                        dgvCredit.Columns[11].ReadOnly = true;
                        dgvCredit.Columns[12].ReadOnly = true;
                        dgvCredit.Columns[13].ReadOnly = true;
                        dgvCredit.Columns[19].ReadOnly = true;

                        dgvCredit.Rows[e.RowIndex].Cells[10].Value = true;
                        dgvCredit.Rows[e.RowIndex].Cells[11].Value = "0.00";
                        dgvCredit.Rows[e.RowIndex].Cells[12].Value = "0.00";
                        dgvCredit.Rows[e.RowIndex].Cells[13].Value = "0.00";
                        dgvCredit.Rows[e.RowIndex].Cells[19].Value = "0.00";
                        this.dgvCredit.RefreshEdit();
                    }
                    this.dgvCredit.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
                else
                {
                    dgvCredit.Columns[11].ReadOnly = true;
                    dgvCredit.Columns[12].ReadOnly = true;
                    dgvCredit.Columns[13].ReadOnly = true;
                    dgvCredit.Columns[19].ReadOnly = true;

                    dgvCredit.Rows[e.RowIndex].Cells[11].Value = "0.00";
                    dgvCredit.Rows[e.RowIndex].Cells[12].Value = "0.00";
                    dgvCredit.Rows[e.RowIndex].Cells[13].Value = "0.00";
                    dgvCredit.Rows[e.RowIndex].Cells[19].Value = "0.00";
                    dgvCredit.Rows[e.RowIndex].Cells[14].Value = "";
                }
                set_amount();
                dgvCredit.EndEdit();
                //}
                //else
                //{
                //    dgvCredit.Rows[e.RowIndex].Cells[10].Value = itc;
                //    this.dgvCredit.RefreshEdit();
                //    this.dgvCredit.CommitEdit(DataGridViewDataErrorContexts.Commit);
                //    if (itc == false)
                //    {
                //        MessageBox.Show("Entry has been allocated against the " + entry_ty_mainall_vw + "" + Environment.NewLine + "Entry / Transaction can not be checked.", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    }
                //    else
                //    {
                //        MessageBox.Show("Entry has been allocated against the " + entry_ty_mainall_vw + "" + Environment.NewLine + "Entry / Transaction can not be unchecked.", this.pPApplName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    }

                //    dgvCredit.Rows[e.RowIndex].Cells[10].Value = itc;
                //    this.dgvCredit.RefreshEdit();
                //    this.dgvCredit.CommitEdit(DataGridViewDataErrorContexts.Commit);
                //}
            }
            //   set_Unavailable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dtCredit.Rows.Count > 0)
            {
                party_Notification();
                search();
            }
            else
            {
                MessageBox.Show("Party Name not found.", this.pPApplName);
            }
            set_amount();
            editGrid();
        }

        public void epayment()
        {
            string e_ty, t_cd, isr, icgst, isgst, iigst, icess;
            foreach (DataGridViewRow dr in dgvCredit.Rows)
            {
                e_ty = dr.Cells[20].Value.ToString();
                t_cd = dr.Cells[0].Value.ToString();
                isr = dr.Cells[21].Value.ToString();
                icgst = dr.Cells[11].Value.ToString();
                isgst = dr.Cells[12].Value.ToString();
                iigst = dr.Cells[13].Value.ToString();
                icess = dr.Cells[19].Value.ToString();

                //   MessageBox.Show(e_ty + " " + t_cd + " " + isr+" "+icgst+" "+isgst+" "+iigst+" "+icess);
                DataTable epayment = new DataTable();
                string SqlStr1 = "select * from Epayment where itserial='" + isr + "' and entry_ty='" + e_ty + "' and tran_cd=" + t_cd + " ";
                epayment = oDataAccess.GetDataSet(SqlStr1, null, 20).Tables[0];
                if (epayment.Rows.Count > 0)
                {
                    oDataAccess.BeginTransaction();
                    string SqlStr = "update Epayment set ICGST_AMT=" + icgst + ",ISGST_AMT=" + isgst + ",IIGST_AMT=" + iigst + ",ICESS_AMT=" + icess + " where   itserial='" + isr + "' and entry_ty='" + e_ty + "' and tran_cd=" + t_cd + "";
                    int result = oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }
                else
                {
                    oDataAccess.BeginTransaction();
                    string SqlStr = "insert into Epayment values('" + e_ty + "'," + t_cd + ",'" + isr + "'," + icgst + "," + isgst + "," + iigst + "," + icess + ");";
                    int result = oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    oDataAccess.CommitTransaction();
                }

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mode = "add";
            set_enable();
            edit = 0;
            int updated = 0;
            string SqlStr = string.Empty;
            epayment();
            dtCredit.AcceptChanges();
            foreach (DataRow dr in dtCredit.Rows)

            {
                oDataAccess.BeginTransaction();
                string Credit_Availability, itserial, entry_ty, tran_cd, itc_sec;
                Credit_Availability = dr["Credit_Availability"].ToString();
                itc_sec = dr["IN_ITC_SEC"].ToString();
                itserial = dr["itserial"].ToString();
                entry_ty = dr["entry_ty"].ToString();
                tran_cd = dr["tran_cd"].ToString();
                //Add Rupesh G. 26 Sep 2017 
                string tblname = getTableName(entry_ty);
                SqlStr = "update " + tblname + " set ECredit='" + Credit_Availability + "',ITCSEC='" + itc_sec + "' where itserial='" + itserial + "' and entry_ty='" + entry_ty + "' and tran_cd=" + tran_cd + " ";

                //Comment start Rupesh G. 26 Sep 2017
                //if (comboBox1.SelectedItem.ToString() == "Goods")
                //{
                //    SqlStr = "update PTITEM set ECredit='" + Credit_Availability + "',ITCSEC='" + itc_sec + "' where itserial='" + itserial + "' and entry_ty='" + entry_ty + "' and tran_cd=" + tran_cd + " ";
                //}
                //else if (comboBox1.SelectedItem.ToString() == "Services")
                //{
                //    SqlStr = "update EPITEM set ECredit='" + Credit_Availability + "',ITCSEC='" + itc_sec + "'  where itserial='" + itserial + "' and entry_ty='" + entry_ty + "' and tran_cd=" + tran_cd + " ";
                //}
                //Comment end Rupesh G. 26 Sep 2017
                try
                {
                    updated = oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                }
                catch (Exception)
                {

                }
                oDataAccess.CommitTransaction();



            }


            if (updated != 0)
            {
                btnSave.Enabled = false;
                btnEdit.Enabled = true;

            }
            set_Unavailable();
            dgvCredit.Columns[10].ReadOnly = true;
            dgvCredit.Columns[11].ReadOnly = true;
            dgvCredit.Columns[12].ReadOnly = true;
            dgvCredit.Columns[13].ReadOnly = true;
            dgvCredit.Columns[19].ReadOnly = true;


            foreach (DataRow dr in dtCredit.Rows)
            {
                oDataAccess.BeginTransaction();
                string Credit_Availability, itserial, entry_ty, tran_cd, itc_sec;
                Credit_Availability = dr["Credit_Availability"].ToString();
                itc_sec = dr["IN_ITC_SEC"].ToString();
                itserial = dr["itserial"].ToString();
                entry_ty = dr["entry_ty"].ToString();
                tran_cd = dr["tran_cd"].ToString();
                string tblname = getTableName(entry_ty);

                foreach (DataGridViewRow dr1 in dgvCredit.Rows)
                {
                    string e_ty = dr1.Cells[20].Value.ToString();
                    string t_cd = dr1.Cells[0].Value.ToString();
                    string isr = dr1.Cells[21].Value.ToString();

                    string cgst = dr1.Cells[7].Value.ToString();
                    string sgst = dr1.Cells[8].Value.ToString();
                    string igst = dr1.Cells[9].Value.ToString();
                    string cess = dr1.Cells[18].Value.ToString();

                    string icgst = dr1.Cells[11].Value.ToString();
                    string isgst = dr1.Cells[12].Value.ToString();
                    string iigst = dr1.Cells[13].Value.ToString();
                    string icess = dr1.Cells[19].Value.ToString();

                    if (e_ty == entry_ty && t_cd == tran_cd && isr == itserial)
                    {
                        if (cgst == icgst && sgst == isgst && igst == iigst && cess == icess)
                        {
                            Credit_Availability = "False";
                            dr1.Cells[10].Value = false;
                        }
                        else if (icgst == "0.00" && isgst == "0.00" && iigst == "0.00" && icess == "0.00")
                        {
                            //MessageBox.Show("ok");
                            Credit_Availability = "True";
                            itc_sec = "";
                            dr1.Cells[14].Value = "";
                            dr1.Cells[10].Value = true;
                        }
                        else
                        {
                            Credit_Availability = "True";
                            dr1.Cells[10].Value = true;
                        }

                        SqlStr = "update " + tblname + " set ECredit='" + Credit_Availability + "',ITCSEC='" + itc_sec + "' where itserial='" + itserial + "' and entry_ty='" + entry_ty + "' and tran_cd=" + tran_cd + " ";
                        try
                        {
                            updated = oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                        }
                        catch (Exception)
                        {

                        }
                        oDataAccess.CommitTransaction();

                    }
                }
            }
            set_Unavailable();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mode = "add";
            set_enable();
            edit = 0;
            if (dgvCredit.Rows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you wish to cancel this Process ?", this.pPApplText, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    btnSave.Enabled = false;
                    pageLoad(0);
                    dgvCredit.Columns[10].ReadOnly = true;
                    dgvCredit.Columns[11].ReadOnly = true;
                    dgvCredit.Columns[12].ReadOnly = true;
                    dgvCredit.Columns[13].ReadOnly = true;
                    dgvCredit.Columns[19].ReadOnly = true;

                    textBox1.Text = "";
                    textBox5.Text = "";
                    comboBox1.SelectedIndex = -1;
                    //Rupesh G. Add 1-Nov-2017 Bug No.30750
                    label9.Text = "HSN Code";
                    // label9.Text = "HSN / SAC Code"; //Rupesh G. Comment 1-Nov-2017 Bug No.30750
                    lblcode = string.Empty;
                }
            }
            else
            {
                textBox1.Text = "";
                textBox5.Text = "";
                comboBox1.SelectedIndex = -1;
                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = DateTime.Now;
            }

        }

        public void set_enable()
        {
            if(mode=="add")
            {
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;
                comboBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                label1.Focus();

            }
            else if (mode == "edit")
            {
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
                comboBox1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            mode = "edit";
            set_enable();
            edit = 1;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            dgvCredit.Columns[10].ReadOnly = false;
            //foreach (DataGridViewRow dr in dgvCredit.Rows)
            //{
            //    if (dr.Cells[10].Value.ToString().ToLower() == "false")
            //    {

            //        dr.Cells[11].ReadOnly = false;
            //        dr.Cells[12].ReadOnly = false;
            //        dr.Cells[13].ReadOnly = false;
            //        dr.Cells[19].ReadOnly = false;
            //    }

            //}
            editGrid();

        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox5.Text = "";
            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
            {
                MessageBox.Show("From date should less than To date.", this.pPApplName);
                dateTimePicker1.Value = DateTime.Now;
                pageLoad(1);
            }
            else
            {
                pageLoad(1);
            }
            editGrid();
        }

        private void dateTimePicker2_CloseUp(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox5.Text = "";
            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
            {
                MessageBox.Show("To date should greater than From date.", this.pPApplName);
                dateTimePicker2.Value = DateTime.Now;
                pageLoad(1);
            }
            else
            {
                pageLoad(1);
            }
            editGrid();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pageLoad(1);
            textBox1.Text = "";
            textBox5.Text = "";
            lbltxt();
            editGrid(); set_Unavailable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dtCredit.Rows.Count > 0)
            {
                hsn_Notification();
                search();
            }
            else
            {
                if (lblcode == string.Empty)
                {
                    //Rupesh G. Add 1-Nov-2017 Bug No.30750
                    MessageBox.Show("HSN Code Not Found.", this.pPApplName);
                    //  MessageBox.Show("HSN/SAC Code Not Found.", this.pPApplName);//Rupesh G. Comment 1-Nov-2017 Bug No.30750
                }
                else
                {
                    MessageBox.Show(lblcode + " Not Found.", this.pPApplName);
                }
            }
            set_amount();
            editGrid();
        }

        private void dateTimePicker1_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1.Text = "";
            textBox5.Text = "";
            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
            {
                MessageBox.Show("From date should less than To date.", this.pPApplName);
                dateTimePicker1.Value = DateTime.Now;
                pageLoad(1);
            }
            else
            {
                pageLoad(1);
            }
            editGrid();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            //pageLoad(1);

            //textBox1.Text = "";
            //textBox5.Text = "";
        }

        private void dateTimePicker2_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1.Text = "";
            textBox5.Text = "";
            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
            {
                MessageBox.Show("To date should greater than From date.", this.pPApplName);
                dateTimePicker2.Value = DateTime.Now;
                pageLoad(1);

            }
            else
            {
                pageLoad(1);
            }
            editGrid();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnCancel.Enabled)
                btnCancel_Click(this.btnCancel, e);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnLogout.Enabled)
                btnLogout_Click(this.btnLogout, e);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.btnEdit.Enabled)
                btnEdit_Click(this.btnEdit, e);
        }

        private void lblNote_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblNote.Visible = !lblNote.Visible;
        }

        private void dgvCredit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 11)
            {
                decimal ic = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[7].Value.ToString());
                if (decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[11].Value.ToString()) <= ic)
                {

                }
                else
                {
                    MessageBox.Show("Ineligible CGST Amount is greater than Tax Amount. ", this.pPApplName);
                    dgvCredit.Rows[e.RowIndex].Cells[11].Value = icgst;

                }

            }
            if (e.ColumnIndex == 12)
            {
                decimal ic = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[8].Value.ToString());
                if (decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[12].Value.ToString()) <= ic)
                {

                }
                else
                {
                    MessageBox.Show("Ineligible SGST Amount is greater than Tax Amount. ", this.pPApplName);
                    dgvCredit.Rows[e.RowIndex].Cells[12].Value = isgst;
                }

            }
            if (e.ColumnIndex == 13)
            {
                decimal ic = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[9].Value.ToString());
                if (decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[13].Value.ToString()) <= ic)
                {

                }
                else
                {
                    MessageBox.Show("Ineligible IGST Amount is greater than Tax Amount. ", this.pPApplName);
                    dgvCredit.Rows[e.RowIndex].Cells[13].Value = iigst;
                }

            }
            if (e.ColumnIndex == 19)
            {
                decimal ic = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[18].Value.ToString());
                if (decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[19].Value.ToString()) <= ic)
                {

                }
                else
                {

                    MessageBox.Show("Ineligible Cess Amount is greater than Tax Amount. ", this.pPApplName);
                    dgvCredit.Rows[e.RowIndex].Cells[19].Value = icess;
                }

            }
            set_amount();
        }

        private void dgvCredit_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 11)
            {
                icgst = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[11].Value.ToString());

            }
            if (e.ColumnIndex == 12)
            {
                isgst = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[12].Value.ToString());

            }
            if (e.ColumnIndex == 13)
            {
                iigst = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[13].Value.ToString());

            }
            if (e.ColumnIndex == 19)
            {
                icess = decimal.Parse(dgvCredit.Rows[e.RowIndex].Cells[19].Value.ToString());

            }
        }

        private void dgvCredit_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.Value != null && (e.ColumnIndex == 14 || e.ColumnIndex == 16 || e.ColumnIndex == 2))
                {
                    e.Value = e.Value.ToString().Trim();
                }

            }
        }



        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled)
                btnSave_Click(this.btnSave, e);
        }
    }
}
