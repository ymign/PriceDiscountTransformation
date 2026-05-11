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
    /// 香港大学深圳医院 病人号查询
    /// </summary>
    public partial class ucQueryPatientInfoBase : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucQueryPatientInfoBase()
        {
            InitializeComponent();

            this.neuSpread1_Sheet1.RowCount = 0;
            this.cmbQueryType.Text = "姓名";
            neuSpread1_Sheet1.Columns[0].Visible = false;
        }

        Function funMgr = new Function();

        protected override int OnQuery(object sender, object neuObject)
        {
            Query();

            return base.OnQuery(sender, neuObject);
        }

        private void txtQueryInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Query();
            }
        }

        /// <summary>
        /// 最多显示的数据行数
        /// </summary>
        private int maxShowCount = 500;

        /// <summary>
        /// 最多显示的数据行数
        /// </summary>
        [Description("最多显示的数据行数")]
        public int MaxShowCount
        {
            get
            {
                return maxShowCount;
            }
            set
            {
                maxShowCount = value;
            }
        }

        private void Query()
        {
            //病历号（门诊卡号）
            //住院号
            //姓名
            string whereSQL = "";
            if (!string.IsNullOrEmpty(this.txtQueryInfo.Text.Trim()))
            {
                if (cmbQueryType.Text == "姓名")
                {
                    if (cbxAccurate.Checked)
                    {
                        whereSQL = "where f.name = '" + txtQueryInfo.Text + "' order by f.oper_date desc";
                    }
                    else
                    {
                        whereSQL = "where f.name like '%" + txtQueryInfo.Text + "%' and rowNum<" + maxShowCount.ToString() + " order by f.oper_date desc";
                    }
                }
                else if (cmbQueryType.Text == "住院号")
                {
                    if (cbxAccurate.Checked)
                    {
                        whereSQL = @"where f.card_no in (select i.card_no from fin_ipr_inmaininfo i
                            where i.patient_no = '" + txtQueryInfo.Text.PadLeft(10, '0') + @"') 
                            order by f.oper_date desc";
                    }
                    else
                    {
                        whereSQL = @"where f.card_no in (select i.card_no from fin_ipr_inmaininfo i
                            where i.patient_no like '%" + txtQueryInfo.Text + @"%') 
                            and rowNum<" + maxShowCount.ToString() + @"
                            order by f.oper_date desc";
                    }
                }
                else if (cmbQueryType.Text == "门诊号")
                {
                    if (cbxAccurate.Checked)
                    {
                        whereSQL = "where f.card_no = '" + txtQueryInfo.Text.PadLeft(10, '0') + "' order by f.oper_date desc";
                    }
                    else
                    {
                        whereSQL = "where f.card_no like '%" + txtQueryInfo.Text + "%' and rowNum<" + maxShowCount.ToString() + @" order by f.oper_date desc";
                    }
                }
                else
                {
                    MessageBox.Show("查询条件选择错误，请联系管理员！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtQueryInfo.Select();
                    this.txtQueryInfo.SelectAll();
                    this.txtQueryInfo.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("查询内容为空，请重新输入！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtQueryInfo.Select();
                this.txtQueryInfo.SelectAll();
                this.txtQueryInfo.Focus();
                return;
            }

            ArrayList alPatient = funMgr.GetPatientInfoBySQL(whereSQL);
            if (alPatient == null)
            {
                MessageBox.Show("查询出错！\r\n" + funMgr.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FarPoint.Win.Spread.CellType.TextCellType text = new FarPoint.Win.Spread.CellType.TextCellType();

            for (int i = 0; i < neuSpread1_Sheet1.Columns.Count; i++)
            {
                neuSpread1_Sheet1.Columns[i].CellType = text;
            }
            this.neuSpread1_Sheet1.RowCount = 0;


            foreach (Neusoft.HISFC.Models.RADT.Patient pInfo in alPatient)
            {
                neuSpread1_Sheet1.Rows.Add(0, 1);

                //neuSpread1_Sheet1.Cells[0, 0].Text = pInfo.PID.UniqueNO;
                neuSpread1_Sheet1.Cells[0, 1].Text = pInfo.PID.CardNO;
                neuSpread1_Sheet1.Cells[0, 2].Text = pInfo.PID.PatientNO;//住院号
                neuSpread1_Sheet1.Cells[0, 3].Text = pInfo.Name;

                if (pInfo.Sex.ID.ToString() == "F")
                {
                    neuSpread1_Sheet1.Cells[0, 4].Text = "女";
                }
                else if (pInfo.Sex.ID.ToString() == "M")
                {
                    neuSpread1_Sheet1.Cells[0, 4].Text = "男";
                }
                else
                {
                    neuSpread1_Sheet1.Cells[0, 4].Text = "未知";
                }
                neuSpread1_Sheet1.Cells[0, 5].Text = funMgr.GetAge(pInfo.Birthday);//.ToString("yyyy-MM-dd HH:mm");
                neuSpread1_Sheet1.Cells[0, 6].Text = pInfo.IDCard;
                neuSpread1_Sheet1.Cells[0, 7].Text = pInfo.AddressHome;
                neuSpread1_Sheet1.Cells[0, 8].Text = pInfo.PhoneHome;
                neuSpread1_Sheet1.Cells[0, 9].Text = pInfo.AddressBusiness;
                neuSpread1_Sheet1.Cells[0, 10].Text = pInfo.PhoneBusiness;
            }
        }

        private void cbxAccurate_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbxAccurate.Checked)
            {
                MessageBox.Show("模糊查找时，数据可能会比较多，请耐心等待！>>\r\n\r\n数据较多时，系统默认只显示" + maxShowCount.ToString() + "条数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public class Function : Neusoft.FrameWork.Management.Database
    {
        string strMainSQL = @"select '',--f.empi_id,
                               f.card_no,
                               f.name,
                               f.sex_code,
                               f.birthday,
                               f.idenno,
                               f.home,
                               f.home_tel,
                               f.work_home,
                               f.work_tel,
                                (select A.PATIENT_NO from FIN_IPR_INMAININFO A 
                                where A.CARD_NO=F.CARD_NO AND ROWNUM=1) PATIENT_NO
                        from com_patientinfo f";

        public ArrayList GetPatientInfoBySQL(string whereSQL)
        {
            try
            {
                string sql = strMainSQL + "\r\n" + whereSQL;


                ArrayList al = new ArrayList();

                if (ExecQuery(sql) == -1)
                {
                    return null;
                }

                while (Reader.Read())
                {
                    Neusoft.HISFC.Models.RADT.Patient pInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    //pInfo.PID.UniqueNO = Reader[0].ToString();
                    pInfo.PID.CardNO = Reader[1].ToString();
                    pInfo.Name = Reader[2].ToString();
                    pInfo.Sex.ID = Reader[3].ToString();
                    pInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[4]);
                    pInfo.IDCard = Reader[5].ToString();
                    pInfo.AddressHome = Reader[6].ToString();
                    pInfo.PhoneHome = Reader[7].ToString();
                    pInfo.AddressBusiness = Reader[8].ToString();
                    pInfo.PhoneBusiness = Reader[9].ToString();
                    pInfo.PID.PatientNO = Reader[10].ToString();
                    al.Add(pInfo);
                }

                return al;
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return null;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
            }
        }
    }
}
