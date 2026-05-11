using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientInfomation
{
    /// <summary>
    /// 通过姓名查找看诊记录
    /// </summary>
    public partial class frmQueryRegByName : Form
    {
        public frmQueryRegByName()
        {
            InitializeComponent();
        }

        #region 变量属性

        /// <summary>
        /// 门诊收费业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();


        /// <summary>
        /// 患者姓名
        /// </summary>
        private string patientName = string.Empty;

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName
        {
            set { patientName = value; }
        }

        /// <summary>
        /// 患者病历号
        /// </summary>
        private string cardNo = string.Empty;

        /// <summary>
        /// 患者病历号
        /// </summary>
        public string CardNo
        {
            get { return cardNo; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        private string errInfo = string.Empty;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrInfo
        {
            get { return errInfo; }
            set { errInfo = value; }
        }


        #endregion

        #region 方法事件

        /// <summary>
        /// 初始化事件
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            int returnValue = this.QueryRegByName();
            if (returnValue == 1)
            {
                //成功
                return 1;
            }
            else
            {
                //失败
                return -1;
            }
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmQueryRegByName_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

        }

        /// <summary>
        /// 通过姓名查找看诊记录
        /// </summary>
        private int QueryRegByName()
        {
            #region 通过姓名查询当天的有效挂号记录

            this.neuSpread1_Sheet1.Rows.Count = 0;  //清空

            string strSql = @"SELECT t.CARD_NO 病历号,t.NAME 姓名,decode(t.SEX_CODE, 'M','男','F','女','未知') 性别,
                                               FUN_GET_AGE_NEW(t.BIRTHDAY,t.REG_DATE) 年龄,
                                               FUN_GET_EMPLOYEE_NAME(t.SEE_DOCD)看诊医生, FUN_GET_DEPT_NAME(t.SEE_DPCD)看诊科室,
                                               t.PACT_NAME 结算类别
                                      FROM FIN_OPR_REGISTER t
                                      WHERE t.REG_DATE >= TRUNC(SYSDATE)
                                      AND t.VALID_FLAG = '1'
                                      AND t.NAME LIKE '%{0}%'";

            strSql = string.Format(strSql, this.patientName);
            DataSet dsRegister = new DataSet();

            try
            {
                if (this.outpatientManager.ExecQuery(strSql, ref dsRegister) < 0)
                {
                    this.ErrInfo = "没有找到该患者的当天看诊记录!" + this.outpatientManager.Err;
                    return -1;
                }
                if (dsRegister == null || dsRegister.Tables[0].Rows.Count <= 0)
                {
                    this.ErrInfo = "没有找到该患者的当天看诊记录!";
                    return -1;
                }
                DataTable dtRegister = dsRegister.Tables[0];
                System.Windows.Forms.Form frmQueryClinicByName = new Form();
                this.neuSpread1_Sheet1.DataSource = new DataView(dtRegister);
                if (this.neuSpread1_Sheet1.Rows.Count > 0)
                {
                    this.neuSpread1_Sheet1.ActiveRowIndex = 0;
                }
                return 1;

            }
            catch (Exception ex)
            {
                this.ErrInfo = "没有找到该患者的当天看诊记录!" + ex.Message;
                return -1;
            }

            #endregion
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neuSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.RowHeader || e.ColumnHeader)
            {
                return;
            }
            int rowIndex = e.Row;
            if (rowIndex < 0)
            {
                return;
            }

            this.cardNo = this.neuSpread1_Sheet1.Cells[rowIndex, 0].Text;

            //确定
            this.DialogResult = DialogResult.OK;

        }

        /// <summary>
        /// 按键回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neuSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int rowIndex = this.neuSpread1_Sheet1.ActiveRowIndex;
                if (rowIndex < 0)
                {
                    return;
                }
                this.cardNo = this.neuSpread1_Sheet1.Cells[rowIndex, 0].Text;
                //确定
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// 处理按键事件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                //快捷退出
                this.DialogResult = DialogResult.Cancel;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
        


    }
}
