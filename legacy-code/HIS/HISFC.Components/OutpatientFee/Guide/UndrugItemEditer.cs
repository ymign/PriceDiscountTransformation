using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;
using System.Collections;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Fee.Outpatient;


namespace Neusoft.HISFC.Components.OutpatientFee.Guide
{
    /// <summary>
    /// 
    /// </summary>
    public partial class UndrugItemEditer : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public UndrugItemEditer(MZGuideContrast item)
        {
            InitializeComponent();
            GuideManager = new Neusoft.HISFC.BizLogic.Fee.MZGuide();
            this.Init();
            if (item != null)
            {
                this.EditItem = item;
                this.editType = UndrugEditType.Edited;
                this.SetData();
                this.cmbItem.Enabled = false;
            }
            else
            {
                this.editType = UndrugEditType.Added;
                this.EditItem = new MZGuideContrast();
            }
        }

        #region 属性
       
        /// <summary>
        /// 
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.MZGuide GuideManager;

        /// <summary>
        /// 科室字典
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Department deptManager = new Neusoft.HISFC.BizLogic.Manager.Department();
        private UndrugEditType editType;

        private string Msg;

        private Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item;

        private Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast EditItem
        {
            get
            {
                return item;
            }

            set
            {
                item = value;
            }
        }

        #endregion

        private void Init()
        {
            //Const cons = new Const();
            ////cons.Type = (Const.enuConstant)(Reader[0].ToString());
            //cons.ID = "All";
            //cons.Name = "全部";
            //cons.Memo = "";
            //cons.SpellCode = "QB";
            //cons.WBCode = "";
            //cons.UserCode = "";

            //cons.SortID = 0;
            //cons.IsValid = true;
            //cons.OperEnvironment.ID = "009999";
            Neusoft.HISFC.Models.Base.Department dept = new Neusoft.HISFC.Models.Base.Department();
            dept.ID="ALL";
            dept.Name = "全部";
            ArrayList deptList = new ArrayList();
            deptList = this.deptManager.GetRegDepartment();
            deptList.Add(dept);
            this.cmbDept.AddItems(deptList);
            this.cmbDept.Tag = "ALL";
            this.cmbValid.AddItems(this.GetListOfValid());
            this.cmbValid.Tag = "1";
            this.cburgency.AddItems(this.GetListOfValid());
            this.cburgency.Tag = "0";
            this.cmbItem.AddItems(this.GuideManager.QueryUndrugNotULList());
            this.cmbAddr.AddItems(CommonController.CreateInstance().QueryConstant("GuideNotULAddress"));
        }

        private void SetData()
        {
            
            this.cmbItem.Tag = this.EditItem.ItemCode;
            this.cmbDept.Tag = this.EditItem.LabCode;
            this.cmbValid.Tag = this.EditItem.ValidState=="是"?"1":"0";
            this.cmbAddr.Tag = this.EditItem.Addr_Code;
            this.cburgency.Tag = this.EditItem.Urgency == "是" ? "1" : "0";
        }

        private void txtSort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                System.Windows.Forms.SendKeys.Send("{tab}");
            }
        }

        private void txtSort_KeyPress(object sender, KeyEventArgs e)
        {

        }

        private void btExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, EventArgs e)
        {
           
            this.EditItem.ItemCode = this.cmbItem.SelectedItem.ID;
            this.EditItem.ItemName=this.cmbItem.SelectedItem.Name;
            this.EditItem.LabCode = "ALL";//this.cmbDept.SelectedItem.ID;
            this.EditItem.LabName = "全部";//this.cmbDept.SelectedItem.Name;
            this.EditItem.Addr_Code=this.cmbAddr.SelectedItem.ID;
            this.EditItem.Addresses=this.cmbAddr.SelectedItem.Name;
            this.EditItem.ValidState=this.cmbValid.SelectedItem.ID;
            this.EditItem.OperCode = Neusoft.FrameWork.Management.Connection.Operator.ID;
          //  Neusoft.SOC.HISFC.Fee.Models.Undrug
            this.EditItem.SpellCode=(this.cmbItem.SelectedItem as Neusoft.SOC.HISFC.Fee.Models.Undrug).SpellCode;
            this.EditItem.FineCode=(this.cmbItem.SelectedItem as Neusoft.SOC.HISFC.Fee.Models.Undrug).WBCode;
            this.EditItem.Urgency = this.cburgency.SelectedItem.ID;
            if (!Valid())
            {
                MessageBox.Show(Msg);
                return;
            }

            if (this.editType == UndrugEditType.Added)
                this.Add();
            else
                this.Modify();
        }

        private void Modify()
        {
            //throw new NotImplementedException();

            if (this.GuideManager.ModefyNotULContrast(this.EditItem, ref Msg) == -1)
            {
                MessageBox.Show(Msg);
                return;
            }
            else
            {
                MessageBox.Show("更新成功！");
                DialogResult = DialogResult.OK;
                this.Close();
                this.Dispose();
            }
        }

        private void Add()
        {
            // throw new NotImplementedException();
            if (this.GuideManager.AddNotULContrast(this.EditItem, ref Msg) == -1)
            {
                MessageBox.Show(Msg);
                return;
            }
            else
            {
                MessageBox.Show("添加成功！");
                DialogResult = DialogResult.OK;
                this.Close();
                this.Dispose();
            }
        }

        private bool Valid()
        {
            bool bo = true;
            if (string.IsNullOrEmpty(this.EditItem.ItemCode))
            {
                if (bo) bo = !bo;
                this.Msg = "请选择项目!";
            }
            else if (string.IsNullOrEmpty(this.EditItem.LabCode))
            {
                if (bo) bo = !bo;
                this.Msg = "请选择开单科室！";
            }
            else if (string.IsNullOrEmpty(this.EditItem.Addr_Code))
            {
                if (bo) bo = !bo;
                this.Msg = "请填写地址！";
            }
            return bo;
        }

        private ArrayList GetListOfValid()
        {
            ArrayList li = new ArrayList();
            Const cons = new Const();
            //cons.Type = (Const.enuConstant)(Reader[0].ToString());
            cons.ID = "1";
            cons.Name = "是";
            cons.Memo = "";
            cons.SpellCode = "S";
            cons.WBCode = "";
            cons.UserCode = "";
            cons.SortID = 0;
            cons.IsValid = true;
            cons.OperEnvironment.ID = "009999";
            
            li.Add(cons);

            Const cons2 = new Const();
            cons2.ID = "0";
            cons2.Name = "否";
            cons2.Memo = "";
            cons2.SpellCode = "F";
            cons2.WBCode = "";
            cons2.UserCode = "";
            cons2.SortID = 0;
            cons2.IsValid = true;
            cons2.OperEnvironment.ID = "009999";
            li.Add(cons2);
            return li;
        }
    }

    public enum UndrugEditType
    {
        Added,
        Edited
    }
}
