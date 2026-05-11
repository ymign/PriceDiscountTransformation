using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.Common.Controls
{
    /// <summary>
    /// 报表基类查询控件
    /// 修改说明
    /// 1、屏蔽配置文件功能
    /// </summary>
    public partial class ucReportBase : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IReport
    {
        public ucReportBase()
        {
            InitializeComponent();
        }

        #region 域变量参数

        /// <summary>
        /// 报表统计类型
        /// </summary>
        public string reportParm = "";

        /// <summary>
        /// 报表统计类型
        /// </summary>
        public string reportParmDetail = "";

        private string filePath = @".\temp\";

        private string parm = "";

        private string reportID = "";

        public DataView myDataView;

        public DataSet myDataSet = new DataSet();

        public DataView myDataViewDetail;

        public DataSet myDataSetDetail = new DataSet();

        private int iTextIndex = 0;

        private int[] iIndex = null;

        /// <summary>
        /// 页边距 默认 50
        /// </summary>
        private int top = 10;

        /// <summary>
        /// 页边距 默认 50
        /// </summary>
        private int left = 10;

        /// <summary>
        /// 分组列索引
        /// </summary>
        private int groupIndex = -1;

        /// <summary>
        /// 分组合计行索引
        /// </summary>
        private string groupSumIndex = "";

        /// <summary>
        /// 存储分组后的每组累计额
        /// </summary>
        private System.Collections.Hashtable hsGroupSum = new System.Collections.Hashtable();

        /// <summary>
        /// 是否显示明细Tab
        /// </summary>
        private bool isShowDetailTab = true;

        #endregion

        #region 属性

        /// <summary>
        /// Sql配置参数
        /// </summary>
        public virtual string Parm
        {
            get
            {
                // TODO:  添加 ucReportBase.Parm getter 实现
                return this.parm;
            }
            set
            {
                // TODO:  添加 ucReportBase.Parm setter 实现
                if (value.IndexOf("|") == -1 || value.IndexOf("|") == value.Length - 1)
                {
                    if (value.IndexOf("@") == -1 || value.IndexOf("@") == value.Length - 1)
                    {
                        this.reportParm = value;
                    }
                    else
                    {
                        this.reportParm = value.Substring(0, value.IndexOf("|"));
                        this.reportParmDetail = value.Substring(value.IndexOf("|") + 1);
                    }
                }
                else
                {
                    this.iIndex = null;
                    if (value.IndexOf("+") == -1 || value.IndexOf("+") == value.Length - 1)
                    {
                        if (value.IndexOf("@") == -1 || value.IndexOf("@") == value.Length - 1)
                        {
                            //aaaaaaa|bbbbbb

                            this.reportParm = value.Substring(0, value.IndexOf("|"));
                            this.lbTitle.Text = value.Substring(value.IndexOf("|") + 1, value.Length - value.IndexOf("|") - 1);
                        }
                        else
                        {
                            //aaaaaa|bbbbbb@sdfsf+1,2,3

                            this.reportParm = (value.Substring(0, value.IndexOf("@"))).Substring(0, value.IndexOf("|"));
                            this.lbTitle.Text = value.Substring(value.IndexOf("|") + 1, value.Length - value.IndexOf("@") +1);
                            this.lblTitleDetail.Text = this.lbTitle.Text + "明细";
                            this.reportParmDetail = value.Substring(value.IndexOf("@") + 1);
                        }
                    }
                    else
                    {
                        if ((value.IndexOf("@") == -1 || value.IndexOf("@") == value.Length - 1))
                        {
                            string str1 = value.Substring(0, value.IndexOf("+"));
                            this.reportParm = str1.Substring(0, str1.IndexOf("|"));
                            this.lbTitle.Text = str1.Substring(str1.IndexOf("|") + 1, str1.Length - str1.IndexOf("|") - 1);

                        }
                        else
                        {
                            //aaaaaa|bbbbbb@sdfsf+1,2,3

                            string str1 = value.Substring(0, value.IndexOf("+"));
                            this.reportParm = str1.Substring(0, str1.IndexOf("|"));
                            this.reportParmDetail = str1.Substring(value.IndexOf("@") + 1);
                            this.lbTitle.Text = str1.Substring(str1.IndexOf("|") + 1, str1.Length - str1.IndexOf("@")+1);
                            this.lblTitleDetail.Text = this.lbTitle.Text + "明细";
                        }

                        string str2 = value.Substring(value.IndexOf("+") + 1);
                        str2 = str2.Trim(',');
                        string[] strIndex = str2.Split(',');
                        iIndex = new int[strIndex.Length - 1];
                        iTextIndex = Neusoft.FrameWork.Function.NConvert.ToInt32(strIndex[0]);
                        int j = 1;
                        for (int i = 0; i < strIndex.Length - 1; i++)
                        {
                            if (j >= strIndex.Length)
                                break;
                            iIndex[i] = Neusoft.FrameWork.Function.NConvert.ToInt32(strIndex[j]);
                            j++;
                        }

                    }
                }

                this.parm = value;

                if (this.reportParmDetail.Length <= 0)
                {
                    this.InitTabPage();
                }

                //查询
                this.Query();
            }
        }
      
        /// <summary>
        /// 页边距 默认 50
        /// </summary>
        public int TableTop
        {
            get
            {
                return this.top;
            }
            set
            {
                this.top = value;
            }
        }

        /// <summary>
        /// 页边距 默认 50
        /// </summary>
        public int TableLeft
        {
            get
            {
                return this.left;
            }
            set
            {
                this.left = value;
            }
        }

        /// <summary>
        /// 分组列索引
        /// </summary>
        public int GroupValueIndex
        {
            get
            {
                return this.groupIndex;
            }
            set
            {
                this.groupIndex = value;
            }
        }

        /// <summary>
        /// 分组合计行索引
        /// </summary>
        [Description("分组合计行索引"), Category("设置"), DefaultValue(false)]
        public string GroupSumIndex
        {
            get
            {
                return this.groupSumIndex;
            }
            set
            {
                this.groupSumIndex = value;
            }
        }

        /// <summary>
        /// 是否显示明细Tab
        /// </summary>
        [Description("是否显示明细Tab"),Category("设置"),DefaultValue(false)]
        public bool IsShowDetailTab
        {
            get
            {
                return this.isShowDetailTab;
            }
            set
            {
                this.isShowDetailTab = value;
            }
        }

        #endregion

        /// <summary>
        /// tabPage初始化
        /// </summary>
        public virtual void InitTabPage()
        {
            //默认不显示第二个tab页.如果需要可以overridate此方法.
            this.neuTabControl1.TabPages.Remove(this.tabPage2);
        }

        /// <summary>
        /// 设置数据第一个tab页的显示格式显示格式
        /// </summary>
        public virtual void SetFormat()
        {
            //如果存在配置文件，则调用配置文件中的样式。否则为DataSet的默认样式

            if (System.IO.File.Exists(this.filePath + this.reportParm + ".xml"))
               Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(this.neuSpread1_Sheet1, this.filePath+this.reportParm+".xml");
        }

        /// <summary>
        /// 添加合计项
        /// </summary>
        /// <param name="iTextIndex">"合计："项所在行</param>
        /// <param name="iIndex">需计算合计的行索引</param>
        public void SetSum(int iTextIndex, params int[] iIndex)
        {
            if(iIndex.Length <= 0)
                return;

            DataRow rowSum = (this.neuSpread1_Sheet1.DataSource as DataView).Table.NewRow();

            rowSum[iTextIndex] = "合计：";

            (this.neuSpread1_Sheet1.DataSource as DataView).Table.Rows.Add(rowSum);

            int iRowIndex = this.neuSpread1_Sheet1.Rows.Count - 1;

            Hashtable hsSum = new Hashtable();

            for (int i = 0; i < iIndex.Length; i++)
            {
                if(iIndex[i] > this.neuSpread1_Sheet1.Columns.Count -1 )
                    continue;

                this.neuSpread1_Sheet1.Columns[iIndex[i]].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;

                decimal sumDec = 0m;

                for (int j = 0; j < this.neuSpread1_Sheet1.Rows.Count - 1; j++)
                {
                    sumDec += Neusoft.FrameWork.Function.NConvert.ToDecimal(this.neuSpread1_Sheet1.Cells[j, iIndex[i]].Text);
                }

                string sumStr = "";

                string[] donite = sumDec.ToString().Split('.');

                if (donite.Length >= 2)
                {
                    int don = Neusoft.FrameWork.Function.NConvert.ToInt32(donite[1]);

                    if (don > 0)
                    {
                        sumStr = donite[0] + "." + don.ToString();
                    }
                    else
                    {
                        sumStr = donite[0];
                    }
                }
                else
                {
                    sumStr = sumDec.ToString();
                }


               this.neuSpread1_Sheet1.Cells[iRowIndex, iIndex[i]].Text = sumStr;
            }

        }

        /// <summary>
        /// 列分组处理
        /// </summary>
        public void Group()
        {
            string groupValue = "";

            decimal totCost = 0;
            for (int i = 0; i < this.neuSpread1_Sheet1.Rows.Count; i++)
            {
                if (groupValue != this.neuSpread1_Sheet1.Cells[i, this.groupIndex].Text && groupValue != "")
                {
                    
                }
            }
        }

        Neusoft.HISFC.BizLogic.Manager.Report report = new Neusoft.HISFC.BizLogic.Manager.Report();


        #region IReport 成员

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public int Query()
        {
            DateTime myBeginDate = this.dtpBeginDate.Value;	//取开始时间
            DateTime myEndDate = this.dtpEndDate.Value;	//取结束时间
            this.filterStr = "";

            //判断时间的有效性
            if (myEndDate.CompareTo(myBeginDate) < 0)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("终止时间必须大于起始时间"));
                return -1;
            }

            this.lblTop.Text = Neusoft.FrameWork.Management.Language.Msg("统计时间:") + myBeginDate.ToString() + " － " + myEndDate.ToString() +
                "     统计人:"+report.Operator.Name;

            Neusoft.HISFC.BizLogic.Manager.Person ps = new Neusoft.HISFC.BizLogic.Manager.Person();
            Neusoft.HISFC.Models.Base.Employee var = new Neusoft.HISFC.Models.Base.Employee();
            var = ps.GetPersonByID(ps.Operator.ID);
            this.myDataSet = new DataSet();

            string emplCode = "";
            if(this.cmbEmpl.Tag != null)
                emplCode = this.cmbEmpl.Tag.ToString();

            string deptCode = "";
            if(this.cmbDept.Tag != null)
                deptCode = this.cmbDept.Tag.ToString();

            int parm = report.ExecQuery(reportParm, ref this.myDataSet, myBeginDate.ToString(), myEndDate.ToString(), var.ID,var.Dept.ID,this.reportID,emplCode,deptCode);
            if (parm == -1)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg(report.Err));
                return -1;
            }

            //对farpoint绑定数据源
            this.myDataView = new DataView(myDataSet.Tables[0]);
            this.neuSpread1_Sheet1.DataSource = this.myDataView;

            foreach (DataColumn col in myDataSet.Tables[0].Columns)
            {
                if (col.DataType.ToString().IndexOf("String") >= 0)
                    this.filterStr += col.ColumnName + " like '%{0}%' or ";
            }

            if (this.filterStr.Length > 0)
                this.filterStr = this.filterStr.Substring(0, this.filterStr.Length - 3);

            //格式化 该函数不在起作用
            this.SetFormat();
            
            if (this.iIndex != null)
            {
                this.SetSum(this.iTextIndex, this.iIndex);
            }

            float ColWidth = 0;

            for (int i = 0; i < this.neuSpread1_Sheet1.Columns.Count; i++)
            {
                if (this.neuSpread1_Sheet1.Columns[i].Visible)
                {
                    ColWidth += this.neuSpread1_Sheet1.Columns[i].Width;
                }
            }

            this.lbTitle.Location = new Point((int)(ColWidth / 2 - this.lbTitle.Width / 2), this.lbTitle.Location.Y);

            return 1;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public int QueryDetail()
        {
            if (this.reportParmDetail.Length <= 0)
            {
                return -1 ;
            }

            DateTime myBeginDate = this.dtpBeginDate.Value;	//取开始时间

            DateTime myEndDate = this.dtpEndDate.Value;	//取结束时间

            this.filterStr = "";

            //判断时间的有效性

            if (myEndDate.CompareTo(myBeginDate) < 0)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("终止时间必须大于起始时间"));
                return -1;
            }

            this.lblTopDetail.Text = Neusoft.FrameWork.Management.Language.Msg("统计时间:") + myBeginDate.ToString() + " － " + myEndDate.ToString() +
                "     统计人:" + report.Operator.Name;

            Neusoft.HISFC.BizLogic.Manager.Person ps = new Neusoft.HISFC.BizLogic.Manager.Person();
            Neusoft.HISFC.Models.Base.Employee var = new Neusoft.HISFC.Models.Base.Employee();
            var = ps.GetPersonByID(ps.Operator.ID);
            this.myDataSetDetail = new DataSet();

            string emplCode = "";
            if (this.cmbEmpl.Tag != null)
                emplCode = this.cmbEmpl.Tag.ToString();

            string deptCode = "";
            if (this.cmbDept.Tag != null)
                deptCode = this.cmbDept.Tag.ToString();

            List<string> parms = new List<string>();

            parms.Add(myBeginDate.ToString());
            parms.Add(myEndDate.ToString());
            parms.Add(var.ID);
            parms.Add(var.Dept.ID);
            parms.Add(this.reportID);

            for (int i = 0; i < this.neuSpread1_Sheet1.Columns.Count; i++)
            {
                parms.Add(this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, i].Text);
            }

            int parm = report.ExecQuery(reportParmDetail, ref this.myDataSetDetail,parms.ToArray());

            if (parm == -1)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg(report.Err));
                return -1;
            }

            //对farpoint绑定数据源

            this.myDataViewDetail = new DataView(myDataSetDetail.Tables[0]);
            this.neuSpread2_Sheet1.DataSource = this.myDataViewDetail;

            foreach (DataColumn col in myDataSetDetail.Tables[0].Columns)
            {
                if (col.DataType.ToString().IndexOf("String") >= 0)
                    this.filterStrDetail += col.ColumnName + " like '%{0}%' or ";
            }

            if (this.filterStrDetail.Length > 0)
                this.filterStrDetail = this.filterStrDetail.Substring(0, this.filterStrDetail.Length - 3);

            //格式化 该函数不在起作用
            this.SetFormat();

            float ColWidthDetail = 0;

            for (int i = 0; i < this.neuSpread2_Sheet1.Columns.Count; i++)
            {
                if (this.neuSpread2_Sheet1.Columns[i].Visible)
                {
                    ColWidthDetail += this.neuSpread2_Sheet1.Columns[i].Width;
                }
            }

            this.lblTitleDetail.Location = new Point((int)(ColWidthDetail / 2 - this.lblTitleDetail.Width / 2), this.lblTitleDetail.Location.Y);

            this.neuTabControl1.SelectedIndex = 1;

            return 1;
        }

        #endregion

        #region IReportPrinter 成员

        public int Export()
        {
            try
            {
                string fileName = "";
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".xls";
                dlg.Filter = "Microsoft Excel 工作薄 (*.xls)|*.*";
                DialogResult result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fileName = dlg.FileName;

                    if (this.neuTabControl1.SelectedIndex == 0)
                    {
                        this.neuSpread1.SaveExcel(fileName, FarPoint.Win.Spread.Model.IncludeHeaders.ColumnHeadersCustomOnly);
                    }
                    else
                    {
                        this.neuSpread2.SaveExcel(fileName, FarPoint.Win.Spread.Model.IncludeHeaders.ColumnHeadersCustomOnly);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
            return 1;
        }

        public int Print()
        {
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.White;

            Neusoft.FrameWork.WinForms.Classes.Print print = new Neusoft.FrameWork.WinForms.Classes.Print();
            return print.PrintPage(this.left, this.top, this.panelPrint);

        }

        public int PrintPreview()
        {
            Neusoft.FrameWork.WinForms.Classes.Print p = new Neusoft.FrameWork.WinForms.Classes.Print();
            //纸张配置
            p.ShowPageSetup();
            p.IsHaveGrid = true;
            //打印起始终止页面配置
            p.ShowPrintPageDialog();
            System.Drawing.Printing.PaperSize size = new System.Drawing.Printing.PaperSize("Letter", 813, 1064);
            p.SetPageSize(size);
            //打印预览
            return p.PrintPreview(10, 60, this.panelPrint);
        }

        #endregion

        private void ucReportBase_Load(object sender, EventArgs e)
        {
            try
            {
                this.neuSpread1_Sheet1.SetColumnAllowAutoSort(-1, true);
                Neusoft.FrameWork.Management.DataBaseManger data = new Neusoft.FrameWork.Management.DataBaseManger();
                this.dtpBeginDate.Value = DateTime.Parse(data.GetSysDate() + " 00:00:00");	//起始时间
                this.dtpEndDate.Value = DateTime.Parse(data.GetSysDate() + " 23:59:59");	//结束时间

                if (!this.isShowDetailTab)
                {
                    this.InitTabPage();
                }
            }
            catch { }

        }

        private void neuSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            try
            {
                System.IO.File.Delete(this.filePath + this.reportParm + ".xml");
            }
            catch { }

            Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.neuSpread1_Sheet1, this.filePath + this.reportParm + ".xml");

        }

        #region IReport 成员

        /// <summary>
        /// 設置參數
        /// </summary>
        /// <param name="repparm"></param>
        /// <returns></returns>
        public int SetParm(string repparm,string repID)
        {
            this.Parm = repparm;
            this.reportID = repID;

            return 1;
        }

        #endregion

        string filterStr = "";

        string filterStrDetail = "";

        string emplSql = "";

        /// <summary>
        /// 人员SQL
        /// </summary>
        public string EmplSql
        {
            set
            {
                string[] condi = value.Split('|');

                if (condi.Length == 2)
                {
                    this.emplSql = condi[1];
                }

                this.cmbEmpl.ClearItems();
                this.label2.Text = "条件";

                if (this.emplSql.Length > 0)
                {
                    DataSet ds = new DataSet();
                    
                    this.report.ExecQuery(this.emplSql, ref ds);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        this.label2.Visible = true;
                        this.label2.Text = condi[0];
                        this.cmbEmpl.Visible = true;

                        ArrayList al = new ArrayList();

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();

                            if (row.ItemArray.Length > 0)
                                obj.ID = row[0].ToString();

                            if (row.ItemArray.Length > 1)
                                obj.Name = row[1].ToString();

                            if (row.ItemArray.Length > 2)
                                obj.Memo = row[2].ToString();

                            al.Add(obj);
                        }

                        this.cmbEmpl.AddItems(al);
                    }
                    else
                    {
                        this.label2.Visible = false;
                        this.cmbEmpl.Visible = false;
                    }
                }
                else
                {
                    this.label2.Visible = false;
                    this.cmbEmpl.Visible = false;
                }
            }
        }

        string deptSql = "";

        /// <summary>
        /// 科室SQL
        /// </summary>
        public string DeptSql
        {
            set
            {
                string[] condi = value.Split('|');

                if (condi.Length == 2)
                {
                    this.deptSql = condi[1];
                }

                this.cmbDept.ClearItems();
                this.label3.Text = "条件";

                if (this.deptSql.Length > 0)
                {
                    DataSet ds = new DataSet();

                    this.report.ExecQuery(this.deptSql, ref ds);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        this.label3.Visible = true;
                        this.label3.Text = condi[0];

                        this.cmbDept.Visible = true;

                        ArrayList al = new ArrayList();

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();

                            if (row.ItemArray.Length > 0)
                                obj.ID = row[0].ToString();

                            if (row.ItemArray.Length > 1)
                                obj.Name = row[1].ToString();

                            if (row.ItemArray.Length > 2)
                                obj.Memo = row[2].ToString();

                            al.Add(obj);
                        }

                        this.cmbDept.ClearItems();
                        this.cmbDept.AddItems(al);
                    }
                    else
                    {
                        this.label3.Visible = false;
                        this.cmbDept.Visible = false;
                    }
                }
                else
                {
                    this.label3.Visible = false;
                    this.cmbDept.Visible = false;
                }
            }
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {

            if (this.neuTabControl1.SelectedIndex == 0)
            {
                string filter = string.Format(this.filterStr, this.txtFilter.Text.Trim());

                this.myDataView.RowFilter = filter;
            }
            else
            {
                string filter = string.Format(this.filterStrDetail, this.txtFilter.Text.Trim());

                this.myDataViewDetail.RowFilter = filter;
            }

        }

        #region IReport 成员


        public int SetParm(string parm, string reportID, string emplSql, string deptSql)
        {
            this.Parm = parm;
            this.reportID = reportID;
            this.EmplSql = emplSql;
            this.DeptSql = deptSql;

            return 1;
        }

        #endregion

        /// <summary>
        /// 双击显示明细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neuSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            this.QueryDetail();
        }
    }
}
