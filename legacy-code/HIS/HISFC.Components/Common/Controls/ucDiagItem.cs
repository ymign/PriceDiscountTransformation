using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.Common.Controls
{
    public partial class ucDiagItem : UserControl
    {
        public ucDiagItem()
        {
            InitializeComponent();
        }

        public delegate void ItemSelectedDelegate(Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag);
        public event ItemSelectedDelegate ItemSelected;

        private string diagType;
        /// <summary>
        /// 诊断类型
        /// </summary>
        public string DiagType
        {
            get { return this.diagType;}
            set{this.diagType=value;}
        }

        public Neusoft.HISFC.Models.RADT.Patient Patient
        {
            get { return this.curPatient; }
            set { this.curPatient = value; }
        }
        private Neusoft.HISFC.Models.RADT.Patient curPatient;

        public Neusoft.HISFC.Models.HealthRecord.InhosDiagnose SelectedDiag
        {
            get {
                if (this.neuFpEnter1_Sheet1.Rows.Count>0&&this.neuFpEnter1_Sheet1.ActiveRowIndex>=0)
                {
                    Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag = this.neuFpEnter1_Sheet1.ActiveRow.Tag as Neusoft.HISFC.Models.HealthRecord.InhosDiagnose;
                    if (diag!=null)
                    {
                        return diag;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
        }


        public Neusoft.HISFC.Models.HealthRecord.InhosDiagnose PreDiag
        {
            get
            {
                if (this.neuFpEnter1_Sheet1.Rows.Count > 0 && this.neuFpEnter1_Sheet1.ActiveRowIndex >= 0)
                {
                    if (this.neuFpEnter1_Sheet1.ActiveRowIndex==0)
                    {
                        return null;
                    }

                    Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag =this.neuFpEnter1_Sheet1.Rows[this.neuFpEnter1_Sheet1.ActiveRowIndex-1].Tag as Neusoft.HISFC.Models.HealthRecord.InhosDiagnose;
                    if (diag != null)
                    {
                        return diag;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public Neusoft.HISFC.Models.HealthRecord.InhosDiagnose NextDiag
        {
            get
            {
                if (this.neuFpEnter1_Sheet1.Rows.Count > 0 && this.neuFpEnter1_Sheet1.ActiveRowIndex >= 0)
                {
                    if (this.neuFpEnter1_Sheet1.ActiveRowIndex == this.neuFpEnter1_Sheet1.Rows.Count-1)
                    {
                        return null;
                    }
                    Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag = this.neuFpEnter1_Sheet1.Rows[this.neuFpEnter1_Sheet1.ActiveRowIndex + 1].Tag as Neusoft.HISFC.Models.HealthRecord.InhosDiagnose;
                    if (diag != null)
                    {
                        return diag;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }



        public void InitData()
        {
            if (this.curPatient == null || string.IsNullOrEmpty(this.curPatient.ID))
            {
                return;
            }

            Neusoft.HISFC.BizLogic.HealthRecord.Diagnose diagMgr = new Neusoft.HISFC.BizLogic.HealthRecord.Diagnose();
            ArrayList alDiag = diagMgr.QueryInhosDiag(this.curPatient.ID, this.diagType);
            if (alDiag==null)
            {
                MessageBox.Show("初始化诊断信息出错，错误信息：" + diagMgr.Err);
                return;
            }
            else
            {
                this.setData(alDiag);
            }

        }

        private void setData(ArrayList alDiag)
        {
            this.neuFpEnter1_Sheet1.Rows.Count = 0;
            int rowIndex=0;
            foreach (Neusoft.HISFC.Models.HealthRecord.InhosDiagnose item in alDiag)
            {
                this.neuFpEnter1_Sheet1.Rows.Add(rowIndex, 1);
                this.neuFpEnter1_Sheet1.Cells[rowIndex, 0].Text = item.InhosDiag.ID;
                string diagName = string.Empty;
                if (!string.IsNullOrEmpty(item.Prefix))
                {
                    diagName += "(" + item.Prefix + ")";
                }
                diagName += item.InhosDiag.Name;
                if (!string.IsNullOrEmpty(item.Suffix))
                {
                    diagName += "(" + item.Suffix + ")";
                }
                this.neuFpEnter1_Sheet1.Cells[rowIndex, 1].Text = diagName;
                this.neuFpEnter1_Sheet1.Cells[rowIndex, 2].Text = item.ICD10.ID;
                this.neuFpEnter1_Sheet1.Cells[rowIndex, 3].Text = item.ICD10.Name;
                this.neuFpEnter1_Sheet1.Cells[rowIndex, 4].Text = item.Dept.Name;
                this.neuFpEnter1_Sheet1.Cells[rowIndex, 5].Text = item.Doctor.Name;
                this.neuFpEnter1_Sheet1.Cells[rowIndex, 6].Text = item.DiagDate.ToString();
                if (item.IsValid)
                {
                    this.neuFpEnter1_Sheet1.Cells[rowIndex, 7].Text ="有效";
                }
                else
                {
                    this.neuFpEnter1_Sheet1.Cells[rowIndex, 7].Text = "无效";
                }
                this.neuFpEnter1_Sheet1.Rows[rowIndex].Tag = item;
                
                rowIndex++;
            }
        }

        public void AddDiag(Neusoft.HISFC.Models.HealthRecord.InhosDiagnose item)
        {
            int rowIndex = this.neuFpEnter1_Sheet1.Rows.Count;
            this.neuFpEnter1_Sheet1.Rows.Add(rowIndex, 1);
            this.neuFpEnter1_Sheet1.Cells[rowIndex, 0].Text = item.InhosDiag.ID;
            string diagName = string.Empty;
            if (!string.IsNullOrEmpty(item.Prefix))
            {
                diagName += "(" + item.Prefix + ")";
            }
            diagName += item.InhosDiag.Name;
            if (!string.IsNullOrEmpty(item.Suffix))
            {
                diagName += "(" + item.Suffix + ")";
            }
            this.neuFpEnter1_Sheet1.Cells[rowIndex, 1].Text = diagName;
            this.neuFpEnter1_Sheet1.Cells[rowIndex, 2].Text = item.ICD10.ID;
            this.neuFpEnter1_Sheet1.Cells[rowIndex, 3].Text = item.ICD10.Name;
            this.neuFpEnter1_Sheet1.Cells[rowIndex, 4].Text = item.Dept.Name;
            this.neuFpEnter1_Sheet1.Cells[rowIndex, 5].Text = item.Doctor.Name;
            this.neuFpEnter1_Sheet1.Cells[rowIndex, 6].Text = item.DiagDate.ToString();
            this.neuFpEnter1_Sheet1.Rows[rowIndex].Tag = item;
        }

        public void DeleteDiag()
        { 
            
        }

        private void neuFpEnter1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Row >= 0)
            {
                Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag = this.neuFpEnter1_Sheet1.Rows[e.Row].Tag as Neusoft.HISFC.Models.HealthRecord.InhosDiagnose;
                if (diag != null && !string.IsNullOrEmpty(diag.ID))
                {
                    this.ItemSelected(diag);
                }
            }
            
        }

    }
}
