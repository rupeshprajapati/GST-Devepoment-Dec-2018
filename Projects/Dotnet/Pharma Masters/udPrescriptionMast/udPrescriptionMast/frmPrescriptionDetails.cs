using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udPrescriptionMast
{
    public partial class frmPrescriptionDetails : uBaseForm.FrmBaseForm
    {

        string SqlStr, presc_NO, for_Day;
        DataAccess_Net.clsDataAccess oDataAccess;
        static public DataTable dtPrescDet;
        DataTable childTbl = new DataTable();
        static public int count;
        public frmPrescriptionDetails()
        {
            InitializeComponent();
        }


        public string prescNO
        {
            get
            {
                return presc_NO;
            }
            set
            {
                presc_NO = value;
            }
        }

        public string forDay
        {
            get
            {
                return for_Day;
            }
            set
            {
                for_Day = value;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                string gval = "";
                int count = 0;
                if (e.KeyCode == Keys.F2)
                {
                    var selectedCell = dataGridView1.SelectedCells[0];
                    // do something with selectedCell...
                    if (selectedCell.ColumnIndex == 0 && (this.pEditMode == true || this.pAddMode == true))
                    {
                        string VForText = string.Empty, vSearchCol = string.Empty, strSQL = string.Empty, Vstr = string.Empty, vColExclude = string.Empty, vDisplayColumnList = string.Empty, vReturnCol = string.Empty;
                        DataSet tDs = new DataSet();
                        SqlStr = "select it_name,rateunit from it_mast where isService='0'";

                        tDs = oDataAccess.GetDataSet(SqlStr, null, 20);
                        DataView dvw = tDs.Tables[0].DefaultView;
                        VForText = "Select Drug/Medicine Name";
                        vSearchCol = "it_name";
                        vDisplayColumnList = "it_name:Drug/Medicine Name";
                        vReturnCol = "it_name,rateunit";
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
                            gval = oSelectPop.pReturnArray[0];
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (gval == row.Cells[0].Value.ToString())
                                {
                                    MessageBox.Show("Duplicate Drug/Medicine Name Not Allowed..", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    count++;
                                }


                            }
                            if (count == 0)
                            {
                                dataGridView1.CurrentCell.Value = oSelectPop.pReturnArray[0];
                                dataGridView1.CurrentRow.Cells[1].Value = oSelectPop.pReturnArray[1];
                            }




                        }



                    }
                }
                else if ((e.Modifiers == Keys.Control && e.KeyCode == Keys.I) && (this.pEditMode == true || this.pAddMode == true))
                {
                    DataRow drchild = childTbl.NewRow();
                    drchild["GName"] = "";
                    drchild["unit"] = "";

                    drchild["QtyPerDay"] = "0.00";//Change RRG
                    drchild["FDay"] = forDay;//Change RRG

                    drchild["qty"] = "0.00";

                    childTbl.Rows.Add(drchild);
                    this.dataGridView1.Refresh();

                    dataGridView1.ClearSelection();
                    int nRowIndex = dataGridView1.Rows.Count - 1;
                    int nColIndex = dataGridView1.Columns.Count - 1;
                    dataGridView1.Rows[nRowIndex].Cells[0].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[nRowIndex].Cells[0];
                }
                else if ((e.Modifiers == Keys.Control && e.KeyCode == Keys.T) && (this.pEditMode == true || this.pAddMode == true))
                {
                    var selectedCell = dataGridView1.SelectedCells[0];
                    MenuItemClick(new MenuItem(string.Format("Remove")), new EventArgs());
                }

            }
            catch (Exception ee)
            {

            }
        }

        private void frmPrescriptionDetails_Load(object sender, EventArgs e)
        {
            if (count < 1)
            {
                dtPrescDet = new DataTable();
            }

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            DataAccess_Net.clsDataAccess._databaseName = this.pComDbnm;
            DataAccess_Net.clsDataAccess._serverName = this.pServerName;
            DataAccess_Net.clsDataAccess._userID = this.pUserId;
            DataAccess_Net.clsDataAccess._password = this.pPassword;
            oDataAccess = new DataAccess_Net.clsDataAccess();

            CultureInfo ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            dataGridView1.AutoGenerateColumns = false;

            if (dtPrescDet.Rows.Count == 0)
            {
                SqlStr = "select GName,unit,QtyPerDay,FDay,qty from pRetPrescDet where PrescNo='" + prescNO + "'";
                childTbl = new DataTable();
                childTbl = oDataAccess.GetDataTable(SqlStr, null, 20);


                dataGridView1.DataSource = childTbl;
                dataGridView1.Columns[0].DataPropertyName = "GName";
                dataGridView1.Columns[1].DataPropertyName = "unit";
                dataGridView1.Columns[2].DataPropertyName = "QtyPerDay";//Change RRG
                dataGridView1.Columns[3].DataPropertyName = "FDay";//Change RRG
                dataGridView1.Columns[4].DataPropertyName = "qty";
            }
            else
            {

                childTbl = dtPrescDet;
                dataGridView1.DataSource = childTbl;
                dataGridView1.Columns[0].DataPropertyName = "GName";
                dataGridView1.Columns[1].DataPropertyName = "unit";
                dataGridView1.Columns[2].DataPropertyName = "QtyPerDay";//Change RRG
                dataGridView1.Columns[3].DataPropertyName = "FDay";//Change RRG
                dataGridView1.Columns[4].DataPropertyName = "qty";
            }

            if (this.pEditMode == true || this.pAddMode == true)
            {
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Columns[1].ReadOnly = false;
                dataGridView1.Columns[2].ReadOnly = false;
                dataGridView1.Columns[3].ReadOnly = false;
                dataGridView1.Columns[4].ReadOnly = false;
            }
            else
            {

                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Columns[1].ReadOnly = true;
                dataGridView1.Columns[2].ReadOnly = true;
                dataGridView1.Columns[3].ReadOnly = true;
                dataGridView1.Columns[4].ReadOnly = true;

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("GName", typeof(string));
            dt.Columns.Add("unit", typeof(string));
            dt.Columns.Add("QtyPerDay", typeof(decimal));//Change RRG
            dt.Columns.Add("FDay", typeof(decimal));//Change RRG
            dt.Columns.Add("qty", typeof(decimal));


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    dr[j] = row.Cells[j].Value;
                }

                dt.Rows.Add(dr);
                dtPrescDet = dt;

            }

            if (dtPrescDet.Rows.Count > 0)
            {
                DataRow[] result = dtPrescDet.Select("GName=''");
                DataRow[] result1 = dtPrescDet.Select("unit=''");
                DataRow[] result2 = dtPrescDet.Select("qty=0");

                if (result.Length > 0 && (this.pEditMode == true || this.pAddMode == true))
                {
                    MessageBox.Show("Empty Drug/Medicine Name Not Allowed..", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else if (result1.Length > 0 && (this.pEditMode == true || this.pAddMode == true))
                {
                    MessageBox.Show("Empty Unit Not Allowed..", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else if (result2.Length > 0 && (this.pEditMode == true || this.pAddMode == true))
                {
                    MessageBox.Show("Zero Qty Not Allowed..", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    if (this.pAddMode == true || this.pEditMode == true)
                    { count++; }

                    //if(dtPrescDet.Rows.Count>0)
                    //{
                    //    oDataAccess.BeginTransaction();
                    //    SqlStr = "Delete from pRetPrescDet where PrescNo='" + presc_NO + "'";
                    //    oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    //    oDataAccess.CommitTransaction();

                    //    foreach (DataRow dr in dtPrescDet.Rows)
                    //    {
                    //        oDataAccess.BeginTransaction();
                    //        SqlStr = "Set Dateformat DMY  insert into pRetPrescDet values('" + presc_NO + "','" + dr[0] + "','" + dr[1] + "','" + dr[2] + "')";
                    //        oDataAccess.ExecuteSQLStatement(SqlStr, null, 20, true);
                    //        oDataAccess.CommitTransaction();
                    //    }
                    //}
                    // dttemp = dtPrescDet;
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }

        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && (this.pEditMode == true || this.pAddMode == true))
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Add", new System.EventHandler(this.MenuItemClick)));
                //  m.MenuItems.Add(new MenuItem("",new System.EventHandler(this.MenuItemClick),Shortcut.))
                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    m.MenuItems.Add(new MenuItem(string.Format("Remove", currentMouseOverRow.ToString()), new System.EventHandler(this.MenuItemClick)));
                }

                m.Show(dataGridView1, new Point(e.X, e.Y));


            }



        }

        private void MenuItemClick(Object sender, System.EventArgs e)
        {
            var m = (MenuItem)sender;

            if (this.pEditMode == true || this.pAddMode == true)
            {
                if (m.Text.ToString() == "Add")
                {

                    DataRow drchild = childTbl.NewRow();
                    drchild["GName"] = "";
                    drchild["unit"] = "";
                    drchild["QtyPerDay"] = "0.00";//Change RRG
                    drchild["FDay"] = forDay;//Change RRG
                    drchild["qty"] = "0.00";

                    childTbl.Rows.Add(drchild);
                    this.dataGridView1.Refresh();
                }
                else if (m.Text.ToString() == "Remove")
                {
                    if (MessageBox.Show("Are you sure you wish to delete this Record ?", this.pPApplText,
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        int selectedIndex = dataGridView1.CurrentCell.RowIndex;
                        dataGridView1.Rows.RemoveAt(selectedIndex);
                    }

                }
            }
            else
            {

            }
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {


            //if (e.ColumnIndex == 1 || e.ColumnIndex == 2) // 1 should be your column index
            //{
            //    int i;

            //    if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
            //    {
            //        e.Cancel = true;
            //        MessageBox.Show("Only numeric value Allowed..", this.pPApplText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    }

            //}
        }

        private void frmPrescriptionDetails_MouseLeave(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double qtyPerDay=0, NoOfDay=0, Qty=0;
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4)
            {
                qtyPerDay = double.Parse(dataGridView1[2, e.RowIndex].Value.ToString());
                NoOfDay = double.Parse(dataGridView1[3, e.RowIndex].Value.ToString());
                Qty = double.Parse(dataGridView1[4, e.RowIndex].Value.ToString());
            }

            if(e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
               if(qtyPerDay>0 && NoOfDay>0)
                {                   
                    dataGridView1[4, e.RowIndex].Value = (qtyPerDay * NoOfDay).ToString();
                }

            }            
            else if(e.ColumnIndex == 4)
            {
                if (Qty>0)
                {
                    dataGridView1[2, e.RowIndex].Value = "0.00";
                    dataGridView1[3, e.RowIndex].Value = "0.00";
                 
                }
                else if(Qty==0)
                {
                    dataGridView1[4, e.RowIndex].Value = (qtyPerDay * NoOfDay).ToString();
                  
                }
            }

        }

        private void frmPrescriptionDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.D)
            {
                button1_Click(this.button1, e);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
