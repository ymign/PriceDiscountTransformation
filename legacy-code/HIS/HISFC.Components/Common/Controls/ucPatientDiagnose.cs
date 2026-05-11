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
    public partial class ucPatientDiagnose : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucPatientDiagnose()
        {
            InitializeComponent();
        }


        //Hashtable ht
        Neusoft.HISFC.BizLogic.Manager.Constant constMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();

        Neusoft.HISFC.BizLogic.HealthRecord.Diagnose diagMgr = new Neusoft.HISFC.BizLogic.HealthRecord.Diagnose();

        private ArrayList alDiagType = new ArrayList();

        public Neusoft.HISFC.Models.RADT.Patient Patient
        {
            get { return this.curPatient; }
            set { this.curPatient = value; }
        }
        private Neusoft.HISFC.Models.RADT.Patient curPatient;


        private ArrayList alDiagList=new ArrayList();

        public ArrayList ALDiagList
        {
            get { return this.alDiagList; }
            set { this.alDiagList = value; }
        }

        public void InitData()
        {
            if (this.curPatient==null||string.IsNullOrEmpty(this.curPatient.ID))
            {
                return;
            }
            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载诊断信息...");
            Application.DoEvents();
            try
            {
                this.initButtonImg();
                this.initDiagType();
                this.setPatientinfo();
                this.initDiagControl();
                this.initDiagList();
                this.initPrefixList();
                this.initDoctDiagList();
                this.initDeptDiagList();

            }
            catch (Exception ex)
            {
                MessageBox.Show("加载诊断数据出错，错误信息：" + ex.Message);
            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }
        }

        private void initButtonImg()
        {
            ImageList imgList = new ImageList();
            imgList.Images.Add("save",Neusoft.FrameWork.WinForms.Classes.Function.GetImage(FrameWork.WinForms.Classes.EnumImageList.B保存));
            imgList.Images.Add("delete",Neusoft.FrameWork.WinForms.Classes.Function.GetImage(FrameWork.WinForms.Classes.EnumImageList.S删除));
            imgList.Images.Add("abondon",Neusoft.FrameWork.WinForms.Classes.Function.GetImage(FrameWork.WinForms.Classes.EnumImageList.Z作废));
            imgList.Images.Add("up",Neusoft.FrameWork.WinForms.Classes.Function.GetImage(FrameWork.WinForms.Classes.EnumImageList.S上一个));
            imgList.Images.Add("down",Neusoft.FrameWork.WinForms.Classes.Function.GetImage(FrameWork.WinForms.Classes.EnumImageList.X下一个));
            imgList.Images.Add("exit", Neusoft.FrameWork.WinForms.Classes.Function.GetImage(FrameWork.WinForms.Classes.EnumImageList.T退出));
            
            this.neuToolBar1.ImageList=imgList;
            this.tbSave.ImageKey="save";
            this.tbDelete.ImageKey="delete";
            this.tbAbandon.ImageKey="abondon";
            this.tbUp.ImageKey="up";
            this.tbDown.ImageKey="down";
            this.tbExit.ImageKey = "exit";
        }

        private void initDoctDiagList()
        {
            //TODO:初始化医生个人常用诊断
        }
       private void initDeptDiagList()
       {
           //TODO:初始化科室常用诊断
       }

        private void initPrefixList()
        {
            ArrayList alPrefix= constMgr.GetList("diagPrefix");
            this.cmbPrefix.Items.Clear();
            this.cmbPrefix.AddItems(alPrefix);
        }

        private void initSuffixList()
        {
            ArrayList alSuffix = constMgr.GetList("diagSuffix");
            this.cmbSuffix.Items.Clear();
            this.cmbSuffix.AddItems(alSuffix);
        }

        private void initDiagList()
        {
            if (this.alDiagList.Count == 0)
            {
                this.alDiagList = constMgr.GetList("InhosDiagList");
            }
            //this.cmbDiagnose.Items.Clear();
            this.cmbDiagnose.AddItems(this.alDiagList);
            this.cmbDiagList.AddItems(this.alDiagList);
        }

        private void initDiagType()
        {
            if (this.alDiagType.Count==0)
            {
                alDiagType = constMgr.GetList("InhosDiagType");
            }

            this.cmbDiagType.AddItems(alDiagType);
        }


        private void initDiagControl()
        {
            if (this.alDiagType != null && this.alDiagType.Count > 0)
            {
                this.tbcDiagList.TabPages.Clear();
                string defaultDiagType = string.Empty;
                for (int i = 0; i < this.alDiagType.Count; i++)
                {
                    Neusoft.FrameWork.Models.NeuObject diagType = this.alDiagType[i] as Neusoft.FrameWork.Models.NeuObject;
                    TabPage tp = new TabPage();
                    tp.Text = diagType.Name;
                    ucDiagItem uc = new ucDiagItem();
                    uc.DiagType = diagType.ID;
                    uc.Patient = this.curPatient;
                    uc.InitData();
                    tp.Controls.Add(uc);
                    uc.Dock = DockStyle.Fill;
                    uc.ItemSelected += new ucDiagItem.ItemSelectedDelegate(uc_ItemSelected);
                    tp.Tag = diagType.ID;
                    if (string.IsNullOrEmpty(defaultDiagType))
                    {
                        defaultDiagType = diagType.ID;
                    }
                    this.tbcDiagList.TabPages.Add(tp);
                }
                if (this.tbcDiagList.TabPages.Count > 0)
                {
                    this.tbcDiagList.SelectedIndex = 0;
                    this.cmbDiagType.Tag = defaultDiagType;
                }
            }
        }

        private ucDiagItem GetCurrentUC()
        {
            if (this.tbcDiagList.TabPages.Count > 0 && this.tbcDiagList.SelectedTab != null)
            {
                return this.tbcDiagList.SelectedTab.Controls[0] as ucDiagItem;
            }
            return null;
        }

        void uc_ItemSelected(Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag)
        {
            this.cmbDiagType.Tag = diag.DiagType.ID;
            this.cmbPrefix.Text = diag.Prefix;
            this.cmbDiagList.Tag = diag.InhosDiag.ID;
            this.cmbSuffix.Text = diag.Suffix;
            this.plDiagInput.Tag = diag;
        }

        private void setPatientinfo()
        {
            this.lblCardNO.Text = "住院号：" + this.Patient.PID.CardNO;
            this.lblName.Text="姓名："+this.Patient.Name;
            
        }

        private void ucPatientDiagnose_Load(object sender, EventArgs e)
        {
            this.InitData();
        }

        private void tbcDiagList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (plDiagInput.Tag==null&&tbcDiagList.SelectedTab!=null&&tbcDiagList.SelectedTab.Tag!=null)
            {
                this.cmbDiagType.Tag = tbcDiagList.SelectedTab.Tag;
            }
        }

        private void neuToolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button== this.tbSave)
            {
                this.SaveDiag();
            }
            else if (e.Button==this.tbDelete)
            {
                this.DeleteDiag();
            }
            else if (e.Button==this.tbAbandon)
            {
                this.SetDiagEnvalid();
            }
            else if (e.Button== this.tbDown)
            {
                this.SetDiagDown();
            }
            else if (e.Button==this.tbUp)
            {
                this.SetDiagUp();
            }
            else if (e.Button == this.tbExit)
            {
                this.Exit();
            }

        }

        private void Exit()
        {
            this.FindForm().Close();
        }

        private void DeleteDiag()
        {
            ucDiagItem uc = this.GetCurrentUC();
            if (uc!=null)
            {
                Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag = uc.SelectedDiag; 
                if (diag!=null&&!string.IsNullOrEmpty(diag.ID))
                {
                    string curOper = this.diagMgr.Operator.ID;

                    if (curOper != diag.Doctor.ID)
                    {
                        MessageBox.Show("不允许删除他人开立的诊断，如确实需要删除，请联系此诊断的开立医生：" + diag.Doctor.Name, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show("确认删除该诊断？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                        {

                            if (diagMgr.DeleteInhosDiagnose(diag.ID) == -1)
                            {
                                MessageBox.Show("删除诊断出错，错误信息：" + diagMgr.Err);
                            }
                            else
                            {
                                uc.InitData();
                            }
                        }

                    }
                }
            }
        }


        private void SetDiagEnvalid()
        {
            ucDiagItem uc = this.GetCurrentUC();
            if (uc != null)
            {
                Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag = uc.SelectedDiag;
                if (diag != null && !string.IsNullOrEmpty(diag.ID))
                {string curOper = this.diagMgr.Operator.ID;

                if (curOper != diag.Doctor.ID)
                {
                    MessageBox.Show("不允许作废他人开立的诊断，如确实需要作废，请联系此诊断的开立医生：" + diag.Doctor.Name,"提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {

                    if (MessageBox.Show("确认作废该诊断？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                    {

                        diag.IsValid = false;
                        diag.CancelDate = diagMgr.GetDateTimeFromSysDateTime();

                        if (diagMgr.UpdateInhosDiagnose(diag) == -1)
                        {
                            MessageBox.Show("作废诊断出错，错误信息：" + diagMgr.Err);
                        }
                        else
                        {
                            uc.InitData();
                        }
                    }
                }
                }
            }
        }



        private void SetDiagDown()
        {
            ucDiagItem uc = this.GetCurrentUC();
            if (uc != null)
            {
                Neusoft.HISFC.Models.HealthRecord.InhosDiagnose curDiag = uc.SelectedDiag;
                Neusoft.HISFC.Models.HealthRecord.InhosDiagnose nextDiag = uc.NextDiag;
                if (curDiag != null && !string.IsNullOrEmpty(curDiag.ID) && nextDiag != null && !string.IsNullOrEmpty(nextDiag.ID))
                {

                    this.ExchangeOrderNO(curDiag, nextDiag,uc);

                }
            }
        }



        private void SetDiagUp()
        {
            ucDiagItem uc = this.GetCurrentUC();
            if (uc != null)
            {
                Neusoft.HISFC.Models.HealthRecord.InhosDiagnose curDiag = uc.SelectedDiag;
                Neusoft.HISFC.Models.HealthRecord.InhosDiagnose PreDiag = uc.PreDiag;
                if (curDiag != null && !string.IsNullOrEmpty(curDiag.ID) && PreDiag != null && !string.IsNullOrEmpty(PreDiag.ID))
                {
                    this.ExchangeOrderNO(curDiag, PreDiag,uc);
                }
            }
        }


        private void ExchangeOrderNO(Neusoft.HISFC.Models.HealthRecord.InhosDiagnose curDiag, Neusoft.HISFC.Models.HealthRecord.InhosDiagnose nextDiag, ucDiagItem uc)
        {
            DateTime dtNow = diagMgr.GetDateTimeFromSysDateTime();

            string orderNO = curDiag.OrderNO;
            curDiag.OrderNO = nextDiag.OrderNO;
            nextDiag.OrderNO = orderNO;
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            diagMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            try
            {
                if (diagMgr.UpdateInhosDiagnose(curDiag) == -1)
                {
                    MessageBox.Show("调整诊断顺序出错，错误信息：" + diagMgr.Err);
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                }
                else if (diagMgr.UpdateInhosDiagnose(nextDiag) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("调整诊断顺序出错，错误信息：" + diagMgr.Err);
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                uc.InitData();
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("调整诊断顺序出错，错误信息：" + ex.Message);
            }
        }

        private void SaveDiag()
        {
            string err=string.Empty;
            Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag = this.getDiagFromControl(ref err);
            if (diag!=null)
            {
                if (!string.IsNullOrEmpty(diag.ID))
                {
                    if (diagMgr.UpdateInhosDiagnose(diag) == -1)
                    {
                        MessageBox.Show("保存诊断失败，错误信息：" + diagMgr.Err);
                    }
                    else
                    {
                        this.RefreshTabpage(diag.DiagType.ID);
                    }
                }
                else
                {
                    diag.ID = diagMgr.GetSequence("");
                    if (diagMgr.InsertInhosDiagnose(diag) == -1)
                    {
                        MessageBox.Show("保存诊断失败，错误信息：" + diagMgr.Err);
                    }
                    else
                    {
                        this.RefreshTabpage(diag.DiagType.ID);
                    }
                }
            }
            else
            {
                MessageBox.Show("获取诊断信息失败，错误信息" + err);
            }
        }

        private Neusoft.HISFC.Models.HealthRecord.InhosDiagnose getDiagFromControl(ref string err)
        {
            if (this.cmbDiagList.SelectedIndex < 0 || string.IsNullOrEmpty(this.cmbDiagList.Text))
            {
                err = "诊断信息为空";
                return null;
            }
            Neusoft.HISFC.Models.Base.Const selectedDiag = this.cmbDiagList.SelectedItem as Neusoft.HISFC.Models.Base.Const;
            if (selectedDiag==null)
            {
                err = "诊断信息为空";
		 return null;
	}

            Neusoft.HISFC.Models.HealthRecord.InhosDiagnose diag = new Neusoft.HISFC.Models.HealthRecord.InhosDiagnose();
            if (this.plDiagInput.Tag!=null)
            {
                diag = this.plDiagInput.Tag as Neusoft.HISFC.Models.HealthRecord.InhosDiagnose;
            }

            if (string.IsNullOrEmpty(diag.DiagType.ID))
            {
                diag.DiagType.ID = this.cmbDiagType.Tag.ToString();
            }

            diag.Patient.ID = this.Patient.ID;
            diag.Patient.PID.CardNO = this.Patient.PID.CardNO;
            diag.Patient.Name = this.Patient.Name;
            diag.InhosDiag.ID = this.cmbDiagList.Tag.ToString();
            diag.InhosDiag.Name = this.cmbDiagList.Text;
            diag.ICD10.ID = selectedDiag.Memo;
            diag.ICD10.Name = selectedDiag.UserCode;
            DateTime dtNow = diagMgr.GetDateTimeFromSysDateTime();
            if (string.IsNullOrEmpty(diag.ID))
            {
                diag.DiagDate = dtNow;
            }
            diag.OperDate = dtNow;
            diag.Doctor.ID = this.diagMgr.Operator.ID;
            diag.Doctor.Name = this.diagMgr.Operator.Name;
            diag.Dept.ID=((Neusoft.HISFC.Models.Base.Employee)diagMgr.Operator).Dept.ID;
            diag.Dept.Name = ((Neusoft.HISFC.Models.Base.Employee)diagMgr.Operator).Dept.Name;
            if (string.IsNullOrEmpty(diag.OrderNO))
            {
                diag.OrderNO = diagMgr.GetMaxOrderNO(this.Patient.ID, diag.DiagType.ID);
                if (string.IsNullOrEmpty(diag.OrderNO))
                {
                    err = "获取诊断序号失败，"+diagMgr.Err;
                    return null;
                }
            }
            diag.Prefix = this.cmbPrefix.Text;
            diag.Suffix = this.cmbSuffix.Text;// this.cmbSuffix.Text;
            diag.IsValid = true;
            diag.PatientType = "1";
            return diag;
            
        }

        private void RefreshTabpage(string diagType)
        {
            foreach (TabPage tp in this.tbcDiagList.TabPages)
            {
                if (tp.Tag!=null&&tp.Tag.ToString()==diagType)
                {
                    this.tbcDiagList.SelectedTab = tp;
                    ucDiagItem ucItem = tp.Controls[0] as ucDiagItem;
                    ucItem.InitData();
                    break;
                }
            }
        }
    }
}
