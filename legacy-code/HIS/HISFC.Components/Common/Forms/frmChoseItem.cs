using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 复合项目选择明细
    /// </summary>
    public partial class frmChoseItemCommon : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public frmChoseItemCommon()
        {
            InitializeComponent();
            this.cbxCheckAll.CheckedChanged += new EventHandler(cbxCheckAll_CheckedChanged);
        }

        /// <summary>
        /// 附材医嘱列表
        /// </summary>
        ArrayList alSubItems = new ArrayList();

        /// <summary>
        /// 复合项目明细
        /// </summary>
        public ArrayList AlSubItems
        {
            get
            {
                return alSubItems;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    this.AddItem(value);

                    //设置焦点控件
                    this.btCancel.Select();
                    this.btCancel.Focus();
                }
            }
        }

        /// <summary>
        /// 类别：门诊、住院
        /// </summary>
        private Neusoft.HISFC.Models.Base.ServiceTypes serverType = Neusoft.HISFC.Models.Base.ServiceTypes.C;

        /// <summary>
        /// 类别：门诊、住院
        /// </summary>
        public Neusoft.HISFC.Models.Base.ServiceTypes ServerType
        {
            get
            {
                return serverType;
            }
            set
            {
                serverType = value;
            }
        }

        private void AddItem(ArrayList al)
        {
            this.fpSublItem_Sheet1.RowCount = 0;

            if (serverType == Neusoft.HISFC.Models.Base.ServiceTypes.C)
            {
                foreach (Neusoft.HISFC.Models.Base.Item item in al)
                {
                    this.fpSublItem_Sheet1.Rows.Add(0, 1);
                    this.fpSublItem_Sheet1.Columns[3].CellType = new FarPoint.Win.Spread.CellType.TextCellType();
                    this.fpSublItem_Sheet1.Cells[0, 0].Value = 0;
                    this.fpSublItem_Sheet1.Cells[0, 0].Locked = false;
                    this.fpSublItem_Sheet1.Cells[0, 1].Text = item.Name;
                    this.fpSublItem_Sheet1.Cells[0, 1].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 2].Text = item.UserCode;
                    this.fpSublItem_Sheet1.Cells[0, 2].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 3].Text = item.Qty.ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.fpSublItem_Sheet1.Cells[0, 3].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 4].Text = item.Specs;
                    this.fpSublItem_Sheet1.Cells[0, 4].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 5].Text = item.SysClass.Name;
                    this.fpSublItem_Sheet1.Cells[0, 5].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 6].Text = item.PriceUnit;
                    this.fpSublItem_Sheet1.Cells[0, 6].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 7].Text = item.Price.ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.fpSublItem_Sheet1.Cells[0, 7].Locked = true;

                    this.fpSublItem_Sheet1.Rows[0].Tag = item;
                }
            }
            else
            {
                foreach (Neusoft.HISFC.Models.Base.Item item in al)
                {
                    this.fpSublItem_Sheet1.Rows.Add(0, 1);
                    this.fpSublItem_Sheet1.Columns[3].CellType = new FarPoint.Win.Spread.CellType.TextCellType();
                    this.fpSublItem_Sheet1.Cells[0, 0].Value = 0;
                    this.fpSublItem_Sheet1.Cells[0, 0].Locked = false;
                    this.fpSublItem_Sheet1.Cells[0, 1].Text = item.Name;
                    this.fpSublItem_Sheet1.Cells[0, 1].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 2].Text = item.UserCode;
                    this.fpSublItem_Sheet1.Cells[0, 2].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 3].Text = item.Qty.ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.fpSublItem_Sheet1.Cells[0, 3].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 4].Text = item.Specs;
                    this.fpSublItem_Sheet1.Cells[0, 4].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 5].Text = item.SysClass.Name;
                    this.fpSublItem_Sheet1.Cells[0, 5].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 6].Text = item.PriceUnit;
                    this.fpSublItem_Sheet1.Cells[0, 6].Locked = true;
                    this.fpSublItem_Sheet1.Cells[0, 7].Text = item.Price.ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.fpSublItem_Sheet1.Cells[0, 7].Locked = true;

                    this.fpSublItem_Sheet1.Rows[0].Tag = item;
                }
            }

            this.cbxCheckAll.Checked = true;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            this.alSubItems = new ArrayList();
            for (int i = 0; i < fpSublItem_Sheet1.RowCount; i++)
            {
                if (fpSublItem_Sheet1.Cells[i, 0].Text == "True")
                {
                    if (fpSublItem_Sheet1.Rows[i].Tag != null)
                    {
                        alSubItems.Add(fpSublItem_Sheet1.Rows[i].Tag);
                    }
                }
            }
            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmChoseItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                this.alSubItems = new ArrayList();
                for (int i = 0; i < fpSublItem_Sheet1.RowCount; i++)
                {
                    if (fpSublItem_Sheet1.Cells[i, 0].Text == "True")
                    {
                        if (fpSublItem_Sheet1.Rows[i].Tag != null)
                        {
                            alSubItems.Add(fpSublItem_Sheet1.Rows[i].Tag);
                        }
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }


        void cbxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSublItem_Sheet1.RowCount; i++)
            {
                this.fpSublItem_Sheet1.Cells[i, 0].Value = cbxCheckAll.Checked;
            }
        }

        private void fpSublItem_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
        }
    }
}
