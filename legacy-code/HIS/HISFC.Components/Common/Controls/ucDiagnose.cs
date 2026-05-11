using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using Neusoft.HISFC.Models.HealthRecord.EnumServer;
namespace Neusoft.HISFC.Components.Common.Controls
{
    /// <summary>
    /// 诊断项目列表
    /// </summary>
    public partial class ucDiagnose : UserControl
    {
        public ucDiagnose()
        {
            InitializeComponent();
        }

        private ArrayList almc = new ArrayList();

        /// <summary>
        /// 诊断列表dataset
        /// </summary>
        private DataSet dsDiag = null;

        /// <summary>
        /// 列宽配置文件
        /// </summary>
        string xmlPath = Application.StartupPath + "\\Profile\\outdiagnose.xml";

        private Neusoft.HISFC.BizLogic.HealthRecord.ICD icdManager = new Neusoft.HISFC.BizLogic.HealthRecord.ICD();

        public delegate int MyDelegate(Keys key);

        /// <summary>
        /// 双击、回车项目列表时执行的事件
        /// </summary>
        public event MyDelegate SelectItem;

        public bool isDrag = false;

        /// <summary>
        /// 存放诊断列表
        /// </summary>
        public Hashtable hsDiags;

        #region 属性

        /// <summary>
        /// 传入的诊断列表
        /// </summary>
        ArrayList alDiag = new ArrayList();

        /// <summary>
        /// 传入的诊断列表
        /// </summary>
        public ArrayList AlDiag
        {
            get
            {
                return alDiag;
            }
            set
            {
                alDiag = value;
                this.Retrieve();
            }
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            this.alDiag = this.icdManager.QueryNew(ICDTypes.ICD10, QueryTypes.Valid, ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID);

            Retrieve();

            return 0;
        }

        /// <summary>
        /// 初始化诊断分类
        /// </summary>
        /// <param name="alDiags"></param>
        public void InitICDCategory(ArrayList alDiags)
        {
            try
            {
                //诊断分类树
                this.tvICDCategory1.Init();
                this.tvICDCategory1.alIcd = alDiags;
                this.tvICDCategory1.InitHsICD(Neusoft.HISFC.Models.HealthRecord.EnumServer.ICDTypes.ICD10, Neusoft.HISFC.Models.HealthRecord.EnumServer.QueryTypes.Valid);
                this.tvICDCategory1.SelectedNode = this.tvICDCategory1.Nodes[0];
                if (tvICDCategory1.Nodes.Count == 0)
                {
                    tvICDCategory1.Visible = false;
                }
                this.tvICDCategory1.NodeSelected += new tvICDCategory.NodeSelectedHandler(tvICDCategory1_NodeSelected); 
            }
            catch { }
        }

        /// <summary>
        /// 是否显示诊断分类
        /// </summary>
        public bool IsShowICDCategory
        {
            set
            {
                tvICDCategory1.Visible = value;
            }
            get
            {
                return tvICDCategory1.Visible;
            }
        }

        /// <summary>
        /// 按照类别过滤
        /// </summary>
        /// <param name="node"></param>
        void tvICDCategory1_NodeSelected(TreeNode node)
        {
            this.category = "";

            if (node == this.tvICDCategory1.Nodes[0])
            {
                this.category = "";
            }
            else
            {
                this.category += "'" + (node.Tag as Neusoft.FrameWork.Models.NeuObject).ID + "',";

                foreach (TreeNode nod in node.Nodes)
                {
                    this.category += "'" + (nod.Tag as Neusoft.FrameWork.Models.NeuObject).ID + "',";

                    foreach (TreeNode nod1 in nod.Nodes)
                    {
                        this.category += "'" + (nod1.Tag as Neusoft.FrameWork.Models.NeuObject).ID + "',";

                        foreach (TreeNode nod2 in nod1.Nodes)
                        {
                            this.category += "'" + (nod2.Tag as Neusoft.FrameWork.Models.NeuObject).ID + "',";
                        }
                    }
                }

                this.category = this.category.Substring(0, this.category.Length - 1);
            }

            this.Filter(filterText, false);
        }

        /// <summary>
        /// 显示诊断
        /// </summary>
        private void Retrieve()
        {
            if (dsDiag == null)
            {
                dsDiag = new DataSet();

                dsDiag.Tables.Add("items");
                dsDiag.Tables[0].Columns.AddRange(new DataColumn[]
				{
					new DataColumn("icd_code",Type.GetType("System.String")),
					new DataColumn("icd_name",Type.GetType("System.String")),
					new DataColumn("spell_code",Type.GetType("System.String")),
					new DataColumn("wb_code",Type.GetType("System.String")),
					new DataColumn("category",Type.GetType("System.String"))
				});
                dsDiag.CaseSensitive = false;
            }

            hsDiags = new Hashtable();

            if (alDiag != null)
            {
                if (dsDiag != null && dsDiag.Tables.Count > 0)
                {
                    this.dsDiag.Tables[0].Clear();
                }
                //绑定到farPoint的DataSource时，赋值速度会很慢....
                this.fpSpread1.DataSource = null;
                this.fpSpread1_Sheet1.DataAutoSizeColumns = false;
                foreach (Neusoft.HISFC.Models.HealthRecord.ICD item in alDiag)
                {
                    dsDiag.Tables[0].Rows.Add(new object[] { item.ID, item.Name, item.SpellCode, item.WBCode,item.Category.ID });

                    if (!hsDiags.Contains(item.ID))
                    {
                        hsDiags.Add(item.ID, item);
                    }
                }

                if (dsDiag != null && dsDiag.Tables.Count > 0)
                {
                    fpSpread1.DataSource = dsDiag.Tables[0].DefaultView;
                }

                //fpSpread1_Sheet1.Columns[0].Width = 66F;
                //fpSpread1_Sheet1.Columns[1].Width = 251F;
                //fpSpread1_Sheet1.Columns[2].Width = 90F;
                //fpSpread1_Sheet1.Columns[2].Visible = false;
            }
        }

        string category = "";
        string filterText = "";

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isReal">是否完全比配，通过组套调用时就是按照编码完全匹配</param>
        /// <returns></returns>
        public int Filter(string text)
        {
            return this.Filter(text, false);
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isReal">是否完全比配，通过组套调用时就是按照编码完全匹配</param>
        /// <returns></returns>
        public int Filter(string text, bool isReal)
        {
            string oldText = text;

            this.filterText = text.Trim();
            if (string.IsNullOrEmpty(filterText))
            {
                dsDiag.Tables[0].DefaultView.RowFilter = filterText;
                return 1;
            }

            if (isReal)
            {
                text = "(icd_code = '" + filterText + "') ";
            }
            else
            {
                string strFilter = string.Empty;
                string strSplit = string.Empty;

                for (int i = 0; i < filterText.Length; i++)
                {
                    strSplit = filterText[i].ToString();
                    if (!string.IsNullOrEmpty(strSplit))
                    {
                        if (!string.IsNullOrEmpty(strFilter))
                        {
                            strFilter += " and ";
                        }
                        strFilter += "((icd_code like '%" + strSplit + "%') or " +
                         "(spell_code like '%" + strSplit + "%') or " +
                         "(wb_code like '%" + strSplit + "%') or " +
                         "(icd_name like '%" + strSplit + "%'))";
                    }
                }

                string splitFilter = "((icd_code like '%" + text.Trim() + "%') or " +
                     "(spell_code like '%" + text.Trim() + "%') or " +
                     "(wb_code like '%" + text.Trim() + "%') or " +
                     "(icd_name like '%" + text.Trim() + "%'))";

                //中文的时候 再查分过滤，英文拆分会产生笛卡尔积
                if (System.Text.RegularExpressions.Regex.IsMatch(oldText, @"^[\u4e00-\u9fa5]+$"))
                {
                    text = strFilter;
                }
                else
                {
                    text = splitFilter;
                }
            }

            if (this.category != "")
            {
                string categoryFilter = " and (category in({0})) ";

                categoryFilter = string.Format(categoryFilter, this.category);

                text = text + categoryFilter;
            }

            try
            {
                dsDiag.Tables[0].DefaultView.RowFilter = text;

                //按照ICD编码排序，保证按照编码过滤时优先精确查找
                //不要随意修改，否则调用模板时，如果存在类似编码的 可能会显示错误！！

                //dsDiag.Tables[0].DefaultView.Sort = "icd_code asc";

                if (fpSpread1_Sheet1.Rows.Count == 1 && this.isDrag)
                {
                    if (SelectItem != null)
                    {
                        this.SelectItem(Keys.Enter);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 0;
        }

        /// <summary>
        /// 下一行
        /// </summary>
        /// <returns></returns>
        public int NextRow()
        {
            int row = fpSpread1_Sheet1.ActiveRowIndex;
            if (row < fpSpread1_Sheet1.RowCount - 1)
            {
                fpSpread1_Sheet1.ActiveRowIndex = row + 1;
                fpSpread1_Sheet1.AddSelection(row + 1, 0, 1, 1);
                this.fpSpread1.ShowRow(0, this.fpSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Nearest);
            }
            return 0;
        }

        /// <summary>
        /// 上一行
        /// </summary>
        /// <returns></returns>
        public int PriorRow()
        {
            int row = fpSpread1_Sheet1.ActiveRowIndex;
            if (row > 0)
            {
                fpSpread1_Sheet1.ActiveRowIndex = row - 1;
                fpSpread1_Sheet1.AddSelection(row - 1, 0, 1, 1);
                this.fpSpread1.ShowRow(0, this.fpSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Nearest);
            }
            return 0;
        }


        /// <summary>
        /// 返回选择项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItem(ref Neusoft.HISFC.Models.HealthRecord.ICD item)
        {
            int row = fpSpread1_Sheet1.ActiveRowIndex;

            if (row < 0 || fpSpread1_Sheet1.RowCount == 0)
            {
                item = null;
                return -1;
            }
            string icdCode = fpSpread1_Sheet1.GetText(row, 0);//项目代码
            string icdName = fpSpread1_Sheet1.GetText(row, 1);

            foreach (Neusoft.HISFC.Models.HealthRecord.ICD icd in alDiag)
            {
                if (icd.ID == icdCode && icd.Name == icdName)
                {
                    item = icd;
                    return 0;
                }
            }

            item = null;
            return -1;
        }

        /// <summary>
        /// 添加焦点
        /// </summary>
        /// <returns></returns>
        public void SetFocus()
        {
            this.fpSpread1.Focus();
        }

        /// <summary>
        /// 回车选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData.GetHashCode() == Keys.Enter.GetHashCode())
            {
                if (SelectItem != null)
                {
                    this.SelectItem(Keys.Enter);
                }
            }
        }

        /// <summary>
        /// 双击选择项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.ColumnHeader)
            {
                return;
            }

            if (SelectItem != null)
            {
                this.SelectItem(Keys.Enter);
            }
        }

        #region 无用

        /// <summary>
        /// 返回选择项(医保)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemMc(ref Neusoft.HISFC.Models.HealthRecord.ICDMedicare item)
        {
            int row = fpSpread1_Sheet1.ActiveRowIndex;

            if (row < 0 || fpSpread1_Sheet1.RowCount == 0)
            {
                item = null;
                return -1;
            }
            string IcdCode = fpSpread1_Sheet1.GetText(row, 0);//项目代码
            string IcdName = fpSpread1_Sheet1.GetText(row, 1);

            foreach (Neusoft.HISFC.Models.HealthRecord.ICDMedicare icd in almc)
            {
                if (icd.ID == IcdCode && icd.Name == IcdName)
                {
                    item = icd;
                    return 0;
                }
            }

            item = null;
            return -1;
        }

        /// <summary>
        /// 初始化医保ICD
        /// </summary>
        /// <returns></returns>
        public int InitICDMedicare(String dType)
        {
            dsDiag = new DataSet();
            dsDiag.Tables.Add("items");
            dsDiag.Tables[0].Columns.AddRange(new DataColumn[]
				{
					new DataColumn("icd_code",Type.GetType("System.String")),
					new DataColumn("icd_name",Type.GetType("System.String")),
					new DataColumn("spell_code",Type.GetType("System.String")),
					new DataColumn("wb_code",Type.GetType("System.String")),
					new DataColumn("icd_type",Type.GetType("System.String"))
				});
            dsDiag.CaseSensitive = false;
            RetrieveICDMedicare(dType);
            return 0;
        }

        public void RetrieveICDMedicare(String dType)
        {
            dsDiag.CaseSensitive = false;
            Neusoft.HISFC.BizProcess.Integrate.HealthRecord.HealthRecordBaseMC icdMgrMc = new Neusoft.HISFC.BizProcess.Integrate.HealthRecord.HealthRecordBaseMC();
            almc = icdMgrMc.ICDQueryMc(dType);
            String icdTypeName = "";
            if (almc != null)
            {
                foreach (Neusoft.HISFC.Models.HealthRecord.ICDMedicare item in almc)
                {
                    switch (item.IcdType)
                    {
                        case "1":
                            icdTypeName = "ICD10";
                            break;
                        case "2":
                            icdTypeName = "市医保";
                            break;
                        case "3":
                            icdTypeName = "省医保";
                            break;
                        default:
                            icdTypeName = "";
                            break;
                    }
                    dsDiag.Tables[0].Rows.Add(new object[] { item.ID, item.Name, item.SpellCode, item.WBCode, icdTypeName });
                }

                fpSpread1.DataSource = dsDiag;
                fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Text = "ICD类别";
                fpSpread1_Sheet1.Columns.Get(3).Label = "ICD类别";
                fpSpread1_Sheet1.Columns.Get(3).Visible = true;
                //fpSpread1_Sheet1.Columns[0].Width = 66F;
                //fpSpread1_Sheet1.Columns[1].Width = 251F;
                //fpSpread1_Sheet1.Columns[2].Width = 90F;
                //fpSpread1_Sheet1.Columns[3].Width = 66F;
            }
        }
        #endregion

        private void fpSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnFormatProperty(this.fpSpread1_Sheet1, xmlPath);
        }

        private void ucDiagnose_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(xmlPath))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnFormatProperty(this.fpSpread1_Sheet1, xmlPath);
            }
        }
    }
}
