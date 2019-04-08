using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataAccess_Net;

namespace PointofSale
{
    public partial class frmLastDeal : Form
    {
        private DataTable lastDealDet;
        private DataTable fldmapDet;
        #region Properties

        private clsDataAccess _oDataAccess;
        public clsDataAccess oDataAccess
        {
            get { return _oDataAccess; }
            set { _oDataAccess = value; }
        }
        private int _itemcode;
        public int Itemcode
        {
            get { return _itemcode; }
            set { _itemcode = value; }
        }
        private string _entryType;
        public string EntryType
        {
            get { return _entryType; }
            set { _entryType = value; }
        }
        private string _itemName;

        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        private int _AcId;
        public int AcId
        {
            get { return _AcId; }
            set { _AcId = value; }
        }

        private string _patientNm;
        public string PatientNm
        {
            get { return _patientNm; }
            set { _patientNm = value; }
        }

        #endregion

        public frmLastDeal()
        {
            InitializeComponent();
        }

        private void frmLastDeal_Load(object sender, EventArgs e)
        {
            if (!this.GetLastDeal())
            {
                return;
            }
            if (!this.GetFieldMapSetting())
            {
                return;
            }
            this.SetGridDisplay();
            this.Refresh();
        }
        private bool GetLastDeal()
        {
            try
            {
                lastDealDet = oDataAccess.GetDataTable("Execute Usp_Ent_GetLastDeal '" + this.EntryType + "'," + Convert.ToString(this.Itemcode) + "," + Convert.ToString(this.AcId) + ",'" + Convert.ToString(this.PatientNm)+"'", null, 50);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private bool GetFieldMapSetting()
        {
            try
            {
                fldmapDet = oDataAccess.GetDataTable("Select * From LastDealDetails Where Entry_ty ='" + this.EntryType + "'", null, 50);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void SetGridDisplay()
        {
            this.dgvLastDeal.AutoGenerateColumns = false;
            dgvLastDeal.DataSource = lastDealDet;
            for (int i = 0; i < fldmapDet.Rows.Count; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.HeaderText = Convert.ToString(fldmapDet.Rows[i]["FieldCaption"]).Trim();
                col.DataPropertyName = Convert.ToString(fldmapDet.Rows[i]["FieldName"]).Trim();
                col.Name = "col" + Convert.ToString(fldmapDet.Rows[i]["FieldName"]).Trim();
                this.dgvLastDeal.Columns.Add(col);
            }
            for (int i = 0; i < this.dgvLastDeal.Rows.Count; i++)
            {
                for (int j = 0; j < this.dgvLastDeal.Columns.Count; j++)
                {
                    if (this.dgvLastDeal.Columns[j].ValueType.ToString() == "System.DateTime")
                    {
                        if (Convert.ToDateTime(this.dgvLastDeal.Rows[i].Cells[j].Value).Year <= 1900)
                        {
                            this.dgvLastDeal.Rows[i].Cells[j].Value = string.Empty;
                        }
                    }
                }
            }
            this.Refresh();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
