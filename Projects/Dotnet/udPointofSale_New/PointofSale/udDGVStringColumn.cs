using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PointofSale
{
    [ToolboxBitmap(typeof(System.Windows.Forms.TextBox))] //Show Texbox icon
    public class uTextbox : TextBox
    {
        private System.ComponentModel.Container components = null;
        public uTextbox()
            : base()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
    class TextEditingControl : uTextbox, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;
        public TextEditingControl()
        {

        }
        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property.
        public object EditingControlFormattedValue
        {
            get
            {
                return this.Text;
            }
            set
            {
                if (value is String)
                {
                    this.Text = (string)value;
                }
            }
        }

        // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        // Implements the IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey method.
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            //// Let the DateTimePicker handle the keys listed.
            //switch (key & Keys.KeyCode)
            //{
            //    case Keys.Left:
            //    case Keys.Up:
            //    case Keys.Down:
            //    case Keys.Right:
            //    case Keys.Home:
            //    case Keys.End:
            //    case Keys.PageDown:
            //    case Keys.PageUp:
            //        return true;
            //    default:
            //        return false;
            //}
            return false;
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingPanelCursor property.
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        //protected override void OnValueChanged(EventArgs eventargs)
        protected override void OnTextChanged(EventArgs eventargs)
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            valueChanged = true;
            this.dataGridView.NotifyCurrentCellDirty(true);
            base.OnTextChanged(eventargs);
        }
    }

    public class udDGVStringCell : DataGridViewTextBoxCell
    {
        public udDGVStringCell()
            : base()
        {

        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            try
            {
                // Set the value of the editing control to the current cell value.
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                TextEditingControl ctl = DataGridView.EditingControl as TextEditingControl;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                // do nothing
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing contol that CalendarCell uses.
                return typeof(TextEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(uTextbox);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return string.Empty;
            }
        }
        public override object Clone()
        {
            udDGVStringCell dataGridViewCell = base.Clone() as udDGVStringCell;
            return dataGridViewCell;
        }
    }

    public class udDGVStringColumn : DataGridViewColumn
    {
        public udDGVStringColumn()
            : base(new udDGVStringCell())
        {
        }

        private string _whn_con;
        public string Whn_con
        {
            get { return _whn_con; }
            set { _whn_con = value; }
        }

        private string val_con;
        public string Val_con
        {
            get { return val_con; }
            set { val_con = value; }
        }
        private string val_err;
        public string Val_err
        {
            get { return val_err; }
            set { val_err = value; }
        }
        private string defa_val;
        public string Defa_val
        {
            get { return defa_val; }
            set { defa_val = value; }
        }
        private string filtcond;
        public string Filtcond
        {
            get { return filtcond; }
            set { filtcond = value; }
        }
        private string inter_use;
        public string Inter_use
        {
            get { return inter_use; }
            set { inter_use = value; }
        }
        private string mandatory;
        public string Mandatory
        {
            get { return mandatory; }
            set { mandatory = value; }
        }
        private string inptmask;
        public string Inptmask
        {
            get { return inptmask; }
            set { inptmask = value; }
        }
        private string fldnm;
        public string Fldnm
        {
            get { return fldnm; }
            set { fldnm = value; }
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(udDGVStringCell)))
                {
                    throw new InvalidCastException("Must be a udDGVStringCell");
                }
                base.CellTemplate = value;
            }
        }
    }
}
