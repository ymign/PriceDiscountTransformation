using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    public partial class ucPatientFeeQueryAll : ucPatientFeeQueryNew
    {
        public ucPatientFeeQueryAll()
        {
            InitializeComponent();
            this.btnQuery.Click += new EventHandler(btnQuery_Click);
            this.neuButton1.Click += new EventHandler(neuButton1_Click);
            this.ucQueryInpatientNo2.myEvent+=new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(ucQueryInpatientNo2_myEvent);
        }

        private void neuButton1_Click(object sender, EventArgs e)
        {
            string inState = string.Empty;//入院状态
            string deptCode = string.Empty;//住院科室编码
            string pactCode = string.Empty;//合同单位编码
            string doctCode = string.Empty;//管床医生
            DateTime beginTime = this.dtpBeginTime.Value.Date;
            DateTime endTime = this.dtpEndTime.Value;

            if (this.cmbDept.Text == string.Empty || this.cmbDept.Text == "全部")
            {
                deptCode = "%";

            }
            else
            {
                deptCode = this.cmbDept.Tag.ToString();
            }

            if (this.cmbDoct.Text == string.Empty || this.cmbDoct.Text == "全部")
            {
                doctCode = "%";

            }
            else
            {
                doctCode = this.cmbDoct.Tag.ToString();
            }

            if (this.cmbPact.Text == string.Empty || this.cmbPact.Text == "全部")
            {
                pactCode = "%";
            }
            else
            {
                pactCode = this.cmbPact.Tag.ToString();
            }
            if (this.cmbPatientState.Text == string.Empty || this.cmbPatientState.Text == "全部")
            {
                inState = "%";
            }
            else
            {
                inState = this.cmbPatientState.Tag.ToString();
            }
            Cursor.Current = Cursors.WaitCursor;

            ArrayList patientListTemp = radtManager.QueryPatientByConditons(pactCode, deptCode, inState, beginTime, endTime, doctCode);
            if (patientListTemp == null)
            {
                MessageBox.Show(this, "获取患者基本信息出错!" + this.radtManager.Err, "提示>>", MessageBoxButtons.OK);
                Cursor.Current = Cursors.Arrow;
                return;
            }
            Cursor.Current = Cursors.Arrow;
            this.OnSetValues(patientListTemp, e);
        }

        void btnQuery_Click(object sender, EventArgs e)
        {
            //认为输入住院号查询
            if (this.ucQueryInpatientNo2.Text.Length > 0)
            {
                this.ucQueryInpatientNo2_myEvent();
            }
            else//认为输入姓名查询
            {
                this.QueryPatientByName();
            }
        }

        private void ucQueryInpatientNo2_myEvent()
        {
            this.QueryPatientByInpatientNO();
        }

        protected override void OnLoad(EventArgs e)
        {
            //初始化科室
            if (this.InitDept() == -1)
            {
                return;
            }

            //初始化医生
            if (this.InitDoct() == -1)
            {
                return;
            }

            //初始化合同单位
            if (this.InitPact() == -1)
            {
                return;
            }

            //初始化入院状态
            if (this.InitInState() == -1)
            {
                return;
            }
            base.OnLoad(e);
        }


        /// <summary>
        /// 初始化入院状态
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitInState()
        {
            Neusoft.FrameWork.Models.NeuObject objAll = new Neusoft.FrameWork.Models.NeuObject();

            objAll.ID = "%";
            objAll.Name = "全部";

            Neusoft.HISFC.Models.RADT.InStateEnumService tempEnumState = new Neusoft.HISFC.Models.RADT.InStateEnumService();
            ArrayList inStateList = Neusoft.HISFC.Models.RADT.InStateEnumService.List();

            inStateList.Add(objAll);

            this.cmbPatientState.AddItems(inStateList);

            return 1;
        }

        /// <summary>
        /// 初始化合同单位
        /// </summary>
        /// <returns>成功1 失败 -1</returns>
        private int InitPact()
        {
            int findAll = 0;
            Neusoft.FrameWork.Models.NeuObject objAll = new Neusoft.FrameWork.Models.NeuObject();

            objAll.ID = "%";
            objAll.Name = "全部";
            ArrayList pactList = this.feeIntegrate.QueryPactUnitAll();
            if (pactList == null)
            {
                MessageBox.Show(this,"加载合同单位列表出错!" + this.consManager.Err,"提示>>", MessageBoxButtons.OK);

                return -1;
            }

            pactList.Add(objAll);

            findAll = pactList.IndexOf(objAll);

            this.cmbPact.AddItems(pactList);

            if (findAll >= 0)
            {
                this.cmbPact.SelectedIndex = findAll;
            }

            return 1;
        }

        /// <summary>
        /// 初始化科室
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitDept()
        {
            int findAll = 0;
            Neusoft.FrameWork.Models.NeuObject objAll = new Neusoft.FrameWork.Models.NeuObject();

            objAll.ID = "%";
            objAll.Name = "全部";

            ArrayList deptList = this.deptManager.GetDeptment(Neusoft.HISFC.Models.Base.EnumDepartmentType.I);
            if (deptList == null)
            {
                MessageBox.Show(this,"加载科室列表出错!"+ this.deptManager.Err,"提示>>", MessageBoxButtons.OK);

                return -1;
            }

            deptList.Add(objAll);

            findAll = deptList.IndexOf(objAll);

            this.cmbDept.AddItems(deptList);

            if (findAll >= 0)
            {
                this.cmbDept.SelectedIndex = findAll;
            }

            return 1;
        }

        /// <summary>
        /// 初始化科室
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        private int InitDoct()
        {
            int findAll = 0;
            Neusoft.FrameWork.Models.NeuObject objAll = new Neusoft.FrameWork.Models.NeuObject();

            objAll.ID = "%";
            objAll.Name = "全部";

            ArrayList doctList = this.personManager.GetEmployee(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);
            if (doctList == null)
            {
                MessageBox.Show(this,"加载医生列表出错!" + this.deptManager.Err,"提示>>", MessageBoxButtons.OK);

                return -1;
            }

            doctList.Add(objAll);

            findAll = doctList.IndexOf(objAll);

            this.cmbDoct.AddItems(doctList);

            if (findAll >= 0)
            {
                this.cmbDoct.SelectedIndex = findAll;
            }

            return 1;
        }

        /// <summary>
        /// 根据输入的患者姓名查询患者基本信息
        /// </summary>
        private void QueryPatientByName()
        {
            if (this.txtName.Text.Trim() == string.Empty)
            {
                MessageBox.Show(this,"输入姓名不能为空!","提示>>", MessageBoxButtons.OK);
                this.txtName.Focus();

                return;
            }

            string inputName = "%" + this.txtName.Text.Trim() + "%";
            //去掉特殊字符
            inputName = Neusoft.FrameWork.Public.String.TakeOffSpecialChar(inputName, "'");
            //按照姓名直接查询患者想细信息
            string name = this.txtName.Text;
            ArrayList patientListTemp = this.radtManager.QueryPatientInfoByName(inputName);
            if (patientListTemp == null || patientListTemp.Count == 0)
            {
                MessageBox.Show(this,"无此患者信息!" + this.radtManager.Err,"提示>>", MessageBoxButtons.OK);

                this.Clear();
                this.txtName.Text = name;
                return;
            }

            if (patientListTemp.Count > 0)
            {
                this.Clear();
                this.txtName.Text = name;
                this.OnSetValues(patientListTemp,null);
            }

            return;
        }

        /// <summary>
        /// 根据输入的住院号查询患者基本信息
        /// </summary>
        private void QueryPatientByInpatientNO()
        {
            if (this.ucQueryInpatientNo2.InpatientNo == null || this.ucQueryInpatientNo2.InpatientNo == string.Empty)
            {
                MessageBox.Show(this,"您输入的住院号错误,请重新输入!","提示>>", MessageBoxButtons.OK);
                this.Clear();

                return;
            }

            Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = this.radtManager.QueryPatientInfoByInpatientNO(this.ucQueryInpatientNo2.InpatientNo);
            if (patientInfo == null || patientInfo.ID == null || patientInfo.ID == string.Empty)
            {
                MessageBox.Show(this,"或者患者基本信息出错!" + this.radtManager.Err,"提示>>", MessageBoxButtons.OK);
                this.Clear();
                return;
            }

            this.Clear();

            this.txtName.Text = patientInfo.Name;
            this.btnQuery.Focus();


            this.OnSetValue(patientInfo,null);
        }
    }
}
