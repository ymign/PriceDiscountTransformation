using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 查询的患者信息多于一个选择患者UC
    /// </summary>
    public partial class frmQueryPatientByName : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public frmQueryPatientByName()
        {
            InitializeComponent();

            this.fpSpread1.KeyDown += new KeyEventHandler(fpSpread1_KeyDown);
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(fpSpread1_CellDoubleClick);
        }

        #region 变量
        /// <summary>
        /// 如初转业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 人员业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Person personManager = new Neusoft.HISFC.BizLogic.Manager.Person();
        /// <summary>
        /// 当选择单条患者信息
        /// </summary>
        public delegate void GetPatient(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo);

        private Neusoft.HISFC.BizLogic.Fee.Account accountMgr = new Neusoft.HISFC.BizLogic.Fee.Account();

        /// <summary>
        /// 当选择单条患者信息后触发
        /// </summary>
        public event GetPatient SelectedPatient;

        /// <summary>
        /// 患者信息实体
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

        /// <summary>
        /// 患者身份证号
        /// </summary>
        string idenNO = string.Empty;

        /// <summary>
        /// 符合条件的患者信息条目
        /// </summary>
        private int personCount;
        #endregion

        #region 属性
        /// <summary>
        /// 患者身份证号
        /// </summary>
        public string IdenNO
        {
            get
            {
                return this.idenNO;
            }
            set
            {
                try
                {
                    this.idenNO = value;
                    if (this.idenNO == string.Empty || this.idenNO == null)
                    {
                        return;
                    }
                    //根据idenNO 获得符合条件的患者信息
                    FillPatientInfoByIdenNO();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString() + this.Name.ToString());
                }
            }
        }

        /// <summary>
        /// 符合条件的患者信息条目
        /// </summary>
        public int PersonCount
        {
            get
            {
                return this.personCount;
            }
        }

        /// <summary>
        /// 选定的患者信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo
        {
            get
            {
                return this.patientInfo;
            }
            set
            {
                this.patientInfo = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 将符合条件的患者信息显示在列表中，如果只有一条，则不显示该控件，直接赋值患者信息实体
        /// </summary>
        private void FillPatientInfoByIdenNO()
        {
            ArrayList patients = this.radtIntegrate.QueryComPatientInfoListByIDNO(this.idenNO);

            DisplayPatients(patients);
        }

        /// <summary>
        /// 显示患者基本信息
        /// </summary>
        /// <param name="patients">查询出来的患者列表</param>
        private void DisplayPatients(ArrayList patients)
        {
            //获得患者基本信息出错
            if (patients == null)
            {
                this.personCount = 0;
                this.patientInfo = null;
                this.SelectedPatient(null);

                return;
            }
            //没有找到符合条件的患者信息
            if (patients.Count == 0)
            {
                this.personCount = 0;
                this.patientInfo = null;
                this.SelectedPatient(null);

                return;
            }
            //如果只找到一个符合条件的患者信息，那么不显示控件，直接返回患者的挂号实体
            if (patients.Count == 1)
            {
                this.personCount = 1;
                this.patientInfo = patients[0] as Neusoft.HISFC.Models.RADT.PatientInfo;
                this.SelectedPatient(this.patientInfo);

                return;
            }
            //如果有多个符合条件的患者信息，在控件的列表中显示基本信息，患者信息实体邦定在改行的tag属性中
            this.fpSpread1_Sheet1.RowCount = 1; //默认只有一行
            this.personCount = patients.Count;
            this.fpSpread1_Sheet1.RowCount = personCount;
            Neusoft.HISFC.Models.RADT.PatientInfo patient = null;

            int index = 0;
            for (int i = personCount - 1; i >= 0; i--)
            {
                patient = patients[i] as Neusoft.HISFC.Models.RADT.PatientInfo;


                this.fpSpread1_Sheet1.Cells[index, 0].Text = patient.PID.CardNO;
                this.fpSpread1_Sheet1.Cells[index, 1].Text = patient.Name;
                this.fpSpread1_Sheet1.Cells[index, 2].Text = patient.Sex.Name;
                this.fpSpread1_Sheet1.Cells[index, 3].Text = patient.Birthday.ToShortDateString();
                this.fpSpread1_Sheet1.Cells[index, 4].Text = personManager.GetAge(patient.Birthday);
                this.fpSpread1_Sheet1.Cells[index, 5].Text = patient.IDCard;
                this.fpSpread1_Sheet1.Cells[index, 6].Text = patient.Pact.Name;
                this.fpSpread1_Sheet1.Cells[index, 7].Text = patient.PhoneHome;
                this.fpSpread1_Sheet1.Cells[index, 8].Text = patient.Kin.RelationPhone;
                this.fpSpread1_Sheet1.Cells[index, 9].Text = patient.AddressHome;

                this.fpSpread1_Sheet1.Rows[index].Tag = patient;
                this.fpSpread1_Sheet1.Rows[index].Height = 30F;
                this.fpSpread1_Sheet1.Rows[index].Font = new System.Drawing.Font("宋体", 14F);

                index++;
            }
            this.ShowDialog();
        }

        /// <summary>
        /// 按患者姓名查询患者挂号信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int QueryByName(string Name)
        {
            ArrayList al = radtIntegrate.QueryPatientByName(Name);

            if (al == null)
            {
                MessageBox.Show("查询患者挂号信息时出错!" + radtIntegrate.Err, "提示");
                return -1;
            }

            return query(al);
        }

        /// <summary>
        /// 按患者手机号查询患者挂号信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int QueryByPhone(string phone)
        {
            ArrayList al = radtIntegrate.QueryPatientByPhone(phone);

            if (al == null)
            {
                MessageBox.Show("查询患者挂号信息时出错!" + radtIntegrate.Err, "提示");
                return -1;
            }

            return query(al);
        }

        public int QueryComPatientInfoListByIDNO(string txtIdenNo)
        {
            ArrayList al = radtIntegrate.QueryComPatientInfoListByIDNO(txtIdenNo);

            if (al == null)
            {
                MessageBox.Show("查询患者挂号信息时出错!" + radtIntegrate.Err, "提示");
                return -1;
            }

            return query(al);
        }



        /// <summary>
        /// 按患者医疗证号查询患者挂号信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int QueryByMCardNO(string mCardNO)
        {
            ArrayList al = new ArrayList();

            Neusoft.HISFC.Models.RADT.PatientInfo p = radtIntegrate.QueryComPatientInfoByMcardNO(mCardNO);

            if (p == null)
            {
                MessageBox.Show("查询患者挂号信息时出错!" + radtIntegrate.Err, "提示");
                return -1;
            }

            if (string.IsNullOrEmpty(p.PID.CardNO) == false)
            {
                al.Add(p);
            }

            return query(al);
        }

        private int query(ArrayList al)
        {
            string deptid = ((accountMgr.Operator) as Neusoft.HISFC.Models.Base.Employee).Dept.ID;

            bool isyk = false;

            ArrayList ykal = managerIntegrate.GetConstantList("YkDept");

            if (ykal != null)
            {
                foreach (Neusoft.FrameWork.Models.NeuObject obj in ykal)
                {
                    if (deptid == obj.ID)
                    {
                        isyk = true;
                        break;
                    }
                }
            }


            if (this.fpSpread1_Sheet1.RowCount > 0)
                this.fpSpread1_Sheet1.Rows.Remove(0, this.fpSpread1_Sheet1.RowCount);

            foreach (Neusoft.HISFC.Models.RADT.PatientInfo obj in al)
            {
                this.fpSpread1_Sheet1.Rows.Add(this.fpSpread1_Sheet1.RowCount, 1);

                int row = this.fpSpread1_Sheet1.RowCount - 1;
                if (isyk)
                {
                    List<Neusoft.HISFC.Models.Account.AccountCard> list = accountMgr.GetMarkList(obj.PID.CardNO, "2", "1");
                    string markNo = string.Empty;
                    if (list.Count == 0)
                    {
                        //markNo = register.PID.CardNO;
                        this.fpSpread1_Sheet1.SetValue(row, 0, obj.PID.CardNO, false);
                    }
                    else
                    {
                        //markNo = list[0].MarkNO;
                        this.fpSpread1_Sheet1.SetValue(row, 0, list[0].MarkNO, false);
                    }
                }
                else
                {
                    this.fpSpread1_Sheet1.SetValue(row, 0, obj.PID.CardNO, false);
                }
                // this.fpSpread1_Sheet1.SetValue(row, 0, obj.PID.CardNO, false);
                this.fpSpread1_Sheet1.Cells[row, 1].Text = obj.Name;
                this.fpSpread1_Sheet1.Cells[row, 2].Text = obj.Sex.Name;
                this.fpSpread1_Sheet1.Cells[row, 3].Text = obj.Birthday.ToShortDateString();
                this.fpSpread1_Sheet1.Cells[row, 4].Text = personManager.GetAge(obj.Birthday);
                this.fpSpread1_Sheet1.Cells[row, 5].Text = obj.IDCard;
                this.fpSpread1_Sheet1.Cells[row, 6].Text = obj.Pact.Name;
                this.fpSpread1_Sheet1.Cells[row, 7].Text = obj.PhoneHome;
                this.fpSpread1_Sheet1.Cells[row, 8].Text = obj.Kin.RelationPhone;
                this.fpSpread1_Sheet1.Cells[row, 9].Text = obj.AddressHome;

                fpSpread1_Sheet1.Rows[row].Tag = obj;
            }

            return al.Count;
        }
        /// <summary>
        /// 获取选择的患者病历号
        /// </summary>
        public string SelectedCardNo
        {
            get
            {
                if (this.fpSpread1_Sheet1.RowCount == 0) return "";

                int Row = this.fpSpread1_Sheet1.ActiveRowIndex;

                return this.fpSpread1_Sheet1.GetText(Row, 0);
            }
        }
        #endregion

        #region 事件
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.fpSpread1_Sheet1.Rows.Count > 0)
                {
                    if (this.fpSpread1_Sheet1.Rows[this.fpSpread1_Sheet1.ActiveRowIndex].Tag != null)
                    {
                        this.SelectPatient(this.fpSpread1_Sheet1.ActiveRowIndex);
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 双击FP事件,选择当前患者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.ColumnHeader || this.fpSpread1_Sheet1.RowCount == 0) return;
            int row = e.Row;
            if (this.fpSpread1_Sheet1.RowCount > 0)
            {
                if (this.fpSpread1_Sheet1.Rows[row].Tag != null)
                {
                    this.SelectPatient(row);
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            //this.DialogResult = DialogResult.OK;
            //this.Close();
        }

        /// <summary>
        /// 双击，回车等选择患者
        /// </summary>
        /// <param name="row">当前行</param>
        private void SelectPatient(int row)
        {
            if (this.SelectedPatient != null)
            {
                this.SelectedPatient((Neusoft.HISFC.Models.RADT.PatientInfo)this.fpSpread1_Sheet1.Rows[row].Tag);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 按键事件
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                if (this.SelectedPatient != null)
                {
                    this.SelectedPatient(null);
                }
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 当控件获得焦点的时候,让FP获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmQueryPatientByName_Enter(object sender, EventArgs e)
        {
            this.fpSpread1.Focus();
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.fpSpread1_Sheet1.RowCount == 0
                || this.fpSpread1_Sheet1.SelectionCount == 0)
            {
                return;
            }
            int row = this.fpSpread1_Sheet1.ActiveRowIndex;
            if (this.fpSpread1_Sheet1.RowCount > 0)
            {
                if (this.fpSpread1_Sheet1.Rows[row].Tag != null)
                {
                    this.SelectPatient(row);
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }
}