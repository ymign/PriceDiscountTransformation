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
    /// <summary>
    /// [功能描述: 住院患者选择列表]<br></br>
    /// [创 建 者: ]<br></br>
    /// [创建时间: ]<br></br>
    /// <修改记录>
    ///     <修改人></修改人>
    ///     <修改时间></修改时间>
    ///     <修改内容>
    ///     </修改内容>
    /// </修改记录>
    /// </summary>
    public partial class ucPatientList : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        #region 构造函数

        public ucPatientList()
        {
            InitializeComponent();
        }
        #endregion

        #region 变量

        public delegate void SetDataEventHandler(string inPatientNO);

        public event SetDataEventHandler SetDataEvent;

        private enum EnumColumns
        {
            /// <summary>
            /// 当前科室
            /// </summary>
            Dept,

            /// <summary>
            /// 病人号
            /// </summary>
            UniqueNO,

            /// <summary>
            /// 姓名
            /// </summary>
            PatientName,

            /// <summary>
            /// 性别
            /// </summary>
            Sex,

            /// <summary>
            /// 年龄
            /// </summary>
            Age,

            /// <summary>
            /// 入院日期
            /// </summary>
            InTime,

            /// <summary>
            /// 出院日期
            /// </summary>
            OutTime,

            /// <summary>
            /// 住院流水号
            /// </summary>
            InPatientNO,

            /// <summary>
            /// 住院次数
            /// </summary>
            InTimes,

            /// <summary>
            /// 在院状态
            /// </summary>
            InState
        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        public void ShowPatient(ArrayList patientList)
        {
            this.neuSpread1_Sheet1.RowCount = 0;

            if (patientList != null && patientList.Count > 0)
            {
                Neusoft.HISFC.BizLogic.Manager.DataBase dbMgr = new Neusoft.HISFC.BizLogic.Manager.DataBase();
                for (int i = 0; i < patientList.Count; i++)
                {
                    Neusoft.HISFC.Models.RADT.PatientInfo pInfo = patientList[i] as Neusoft.HISFC.Models.RADT.PatientInfo;

                    this.neuSpread1_Sheet1.Rows.Add(i, 1);

                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.Dept].Text = SOC.HISFC.BizProcess.Cache.Common.GetDeptName(pInfo.PVisit.PatientLocation.Dept.ID);
                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.UniqueNO].Text = pInfo.PID.PatientNO;
                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.PatientName].Text = pInfo.Name;
                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.Sex].Text = pInfo.Sex.Name;
                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.Age].Text = dbMgr.GetAge(pInfo.Birthday);

                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.InTime].Text = pInfo.PVisit.InTime.ToString();

                    if (pInfo.PVisit.OutTime > new DateTime(2000, 1, 1))
                    {
                        this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.OutTime].Text = pInfo.PVisit.OutTime.ToString();
                    }
                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.InPatientNO].Text = pInfo.ID;
                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.InTimes].Text = pInfo.InTimes.ToString();
                    this.neuSpread1_Sheet1.Cells[i, (int)EnumColumns.InState].Text = pInfo.PVisit.InState.Name;

                    this.neuSpread1_Sheet1.Rows.Get(i).Tag = pInfo;
                }
            }
        }

        #endregion

        #region 事件

        private void neuSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.ColumnHeader || this.neuSpread1_Sheet1.RowCount == 0)
            {
                return;
            }

            if (this.SetDataEvent != null)
            {
                this.SetDataEvent(this.neuSpread1_Sheet1.Cells[e.Row, (int)EnumColumns.InPatientNO].Text);
            }
        }

        private void neuSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.neuSpread1_Sheet1.RowCount == 0)
            {
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (this.SetDataEvent != null)
                {
                    this.SetDataEvent(this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)EnumColumns.InPatientNO].Text);
                }
            }
        }

        #endregion
    }
}
