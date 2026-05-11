using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.FrameWork.Models;
using System.Collections;

using Neusoft.HISFC.Models.SIInterface;

namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    public partial class ucNewUnDrugCompare : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucNewUnDrugCompare()
        {
            InitializeComponent();
        }

        #region 枚举
        public enum CompareTypes
        {
            /// <summary>
            /// 西药
            /// </summary>
            P = 0,
            /// <summary>
            /// 中草药
            /// </summary>
            C = 1,
            /// <summary>
            /// 中成药
            /// </summary>
            Z = 2,
            //{B36F2A99-872C-4659-9035-6D80B5489F50} 同sql语句对应 wbo 2010-08-28
            ///// <summary>
            ///// 全部药品
            ///// </summary>
            //All = 3,
            /// <summary>
            /// 全部药品
            /// </summary>
            ALL = 3,
            /// <summary>
            /// 非药品
            /// </summary>
            Undrug = 4,
           
        };

        public enum CompareUndrugTypes
        {
            /// <summary>
            /// 全部
            /// </summary>
            ALL = 0,
            /// <summary>
            /// 材料
            /// </summary>
            U = 1,
            /// <summary>
            /// 非材料
            /// </summary>
            unU = 2,
        };
        #endregion

        #region 变量
        ArrayList alDrug = new ArrayList();//药品列表
        private NeuObject pactCode = new NeuObject();//合同单位
        private string defaulPactCode = string.Empty;//默认对照的合同单位
        private bool isDrug = false;
        private string code = "PY"; //查询码
        private int circle = 0;
        DataTable dtHisItem = new DataTable();
        DataTable dtCenterItem = new DataTable();
        DataTable dtCompareItem = new DataTable();
        DataView dvHisItem = new DataView();
        DataView dvCenterItem = new DataView();
        DataView dvCompareItem = new DataView();
        private CompareTypes compareType;
        private CompareUndrugTypes compareUndrugType;
        private bool isUseNullItemGrade = false;
        private Neusoft.FrameWork.Public.ObjectHelper itemGradeHelper = new Neusoft.FrameWork.Public.ObjectHelper();
        protected Neusoft.HISFC.BizLogic.Fee.ConnectSI myConnectSI = null;
        /// <summary>
        /// Tab
        /// </summary>
        protected Hashtable hashTableFp = new Hashtable();
        protected Neusoft.HISFC.BizLogic.Fee.Interface myInterface = new Neusoft.HISFC.BizLogic.Fee.Interface();
        Neusoft.HISFC.BizLogic.Manager.Constant consMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        #endregion

        #region 属性
        [Category("设置"), Description("设置项目类型 P:西药；C:中草药；Z:中成药；ALL:全部药品；Undrug:非药品")]
        public CompareTypes CompareType
        {
            get
            {
                return compareType;
            }
            set
            {
                compareType = value;
            }
        }

        [Category("设置"), Description("设置项目类型 ALL:全部；U:材料；unU:非材料")]
        public CompareUndrugTypes CompareUndrugType
        {
            get
            {
                return compareUndrugType;
            }
            set
            {
                compareUndrugType = value;
            }
        }

        [Category("设置"), Description("默认对照的合同单位")]
        public string DefaulPactCode
        {
            get
            {
                return defaulPactCode;
            }
            set
            {
                defaulPactCode = value;
            }
        }

        [Category("设置"), Description("是否设置对照等级传入为空")]
        public bool IsUseNullItemGrade
        {
            get
            {
                return isUseNullItemGrade;
            }
            set
            {
                isUseNullItemGrade = value;
            }
        }

        /// <summary>
        /// 合同单位信息
        /// </summary>
        public NeuObject PactCode
        {
            set
            {
                pactCode = value;
            }
            get
            {
                return pactCode;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化显示数据
        /// </summary>
        public void Init()
        {
            if (CompareType.ToString() == CompareTypes.Undrug.ToString())
            {
                isDrug = false;
            }
            else
            {
                isDrug = true;
            }

            InitColumn();

            InitData();

            InitColumnProHis();

            InitColumnProCenter();

            InitColumnProCompare();

            InitHashTable();
        }
        /// <summary>
        /// 连接医保服务器
        /// </summary>
        /// <returns></returns>
        public int ConnectSIServer()
        {
            try
            {
                myConnectSI = new Neusoft.HISFC.BizLogic.Fee.ConnectSI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接医保服务器失败!,请重新配置连接" + ex.Message);
                return -1;
            }
            return 0;
        }

        private void InitHashTable()
        {
            foreach (TabPage t in this.tabCompare.TabPages)
            {
                foreach (Control c in t.Controls)
                {
                    if (c is FarPoint.Win.Spread.FpSpread)
                    {
                        this.hashTableFp.Add(t, c);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化工具栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            this.toolBarService.AddToolButton("对照", "对照", Neusoft.FrameWork.WinForms.Classes.EnumImageList.H合并, true, false, null);
            this.toolBarService.AddToolButton("取消", "取消", Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q取消, true, false, null);
            this.toolBarService.AddToolButton("清空", "清空", Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q清空, true, false, null);
            this.toolBarService.AddToolButton("修改", "修改", Neusoft.FrameWork.WinForms.Classes.EnumImageList.X修改, true, false, null);

            return this.toolBarService;
        }

        /// <summary>
        /// 获得药品基本信息
        /// </summary>
        public void GetHisDrugItem()
        {
            alDrug = myInterface.GetNoCompareDrugItem(pactCode.ID, compareType.ToString());
        }
        /// <summary>
        /// 设置显示列信息;
        /// </summary>
        private void InitColumn()
        {

            Type str = typeof(System.String);
            Type dec = typeof(System.Decimal);
            Type date = typeof(System.DateTime);




            
                //初始化本地项目:
                DataColumn[] colHisItem = new DataColumn[]{new DataColumn("非药品编码", str),
                                                              new DataColumn("非药品名称", str),
                                                              new DataColumn("拼音码", str),
                                                              new DataColumn("五笔码", str),
                                                              new DataColumn("自定义码", str),
                                                              new DataColumn("规格", str),
                                                              new DataColumn("国际编码", str),
                                                              new DataColumn("国家编码", str),
                                                              new DataColumn("价格", str),
                                                              new DataColumn("单位", str)};

                dtHisItem.Columns.AddRange(colHisItem);

                DataColumn[] keyHis = new DataColumn[1];
                keyHis[0] = dtHisItem.Columns[0];
                dtHisItem.PrimaryKey = keyHis;
                dvHisItem = new DataView(dtHisItem);
                dvHisItem.Sort = "非药品编码 ASC";
                fpHisItem_Sheet1.DataSource = dvHisItem;

                DataColumn[] colCenterItem = new DataColumn[]{new DataColumn("国家码（地方码）", str),
                                                                 new DataColumn("国家医保目录名称", str),
                                                                new DataColumn("地方名称", str)};
                dtCenterItem.Columns.AddRange(colCenterItem);
                DataColumn[] keyCenter = new DataColumn[3];
                keyCenter[0] = dtCenterItem.Columns[0];
                keyCenter[1] = dtCenterItem.Columns[1];
                keyCenter[2] = dtCenterItem.Columns[2];
                dtCenterItem.CaseSensitive=true;
                dtCenterItem.PrimaryKey = keyCenter;
                dvCenterItem = new DataView(dtCenterItem);
                dvCenterItem.Sort = "国家码（地方码） ASC";
                fpCenterItem_Sheet1.DataSource = dvCenterItem;
            

            DataColumn[] colCompareItem = new DataColumn[]{ 
                                                            new DataColumn("非药品编码", str),
                                                            new DataColumn("非药品名称", str),
                                                            new DataColumn("国家码（地方码）", str),
                                                            new DataColumn("国家医保目录名称", str),
                                                            new DataColumn("地方名称", str),
                                                            new DataColumn("操作时间", date)};
            dtCompareItem.Columns.AddRange(colCompareItem);
            DataColumn[] keyCompare = new DataColumn[1];
            keyCompare[0] = dtCompareItem.Columns[0];
            dtCompareItem.CaseSensitive=true;
            dtCompareItem.PrimaryKey = keyCompare;
            dvCompareItem = new DataView(dtCompareItem);
            dvCompareItem.Sort = "操作时间 DESC";
            fpCompareItem_Sheet1.DataSource = dvCompareItem;
            
        }
        /// <summary>
        /// HIS本地项目列属性
        /// </summary>
        private void InitColumnProHis()
        {
            int width = 20;


                this.fpHisItem_Sheet1.Columns[0].Visible = true;
                this.fpHisItem_Sheet1.Columns[0].Width = width * 8;
                this.fpHisItem_Sheet1.Columns[1].Width = width * 8;
                this.fpHisItem_Sheet1.Columns[2].Visible = false;
                this.fpHisItem_Sheet1.Columns[3].Visible = false;
                this.fpHisItem_Sheet1.Columns[4].Visible = true;
                this.fpHisItem_Sheet1.Columns[5].Width = width * 8;
                this.fpHisItem_Sheet1.Columns[6].Visible = false;
                this.fpHisItem_Sheet1.Columns[7].Visible = false;
                this.fpHisItem_Sheet1.Columns[8].Width = width * 3;
                this.fpHisItem_Sheet1.Columns[9].Width = width * 4;
            
        }
        /// <summary>
        /// 初始化中心列属性信息
        /// </summary>
        private void InitColumnProCenter()
        {
            int width = 20;
            this.fpCenterItem_Sheet1.Columns[0].Visible = true;
            this.fpCenterItem_Sheet1.Columns[1].Width = width * 8;
        }
        private void InitColumnProCompare()
        {
            int width = 20;

            FarPoint.Win.Spread.CellType.DateTimeCellType dtType = new FarPoint.Win.Spread.CellType.DateTimeCellType();
            dtType.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.ShortDateWithTime;

            fpCompareItem_Sheet1.Columns[0].Visible = true;
            fpCompareItem_Sheet1.Columns[1].Visible = true;
            fpCompareItem_Sheet1.Columns[2].Visible = true;
            fpCompareItem_Sheet1.Columns[3].Width = width * 8;



        }
        /// <summary>
        /// 初始化显示数据
        /// </summary>
        public void InitData()
        {


            ArrayList alHisItem = new ArrayList();
            ArrayList alCenterItem = new ArrayList();
            ArrayList alCompareItem = new ArrayList();

            if (!string.IsNullOrEmpty(this.DefaulPactCode))
            {
                pactCode = new NeuObject();
                pactCode.ID = this.DefaulPactCode;
            }

            
                #region 加载非药品
                alHisItem = myInterface.GetNoCompareUndrugUnHcItem("16", compareUndrugType.ToString());
                if (alHisItem != null)
                {
                    foreach (Neusoft.HISFC.Models.Fee.Item.Undrug obj in alHisItem)
                    {
                        DataRow row = dtHisItem.NewRow();
                        row["非药品编码"] = obj.ID;
                        row["非药品名称"] = obj.Name;
                        row["拼音码"] = obj.SpellCode;
                        row["五笔码"] = obj.WBCode;
                        row["自定义码"] = obj.UserCode;
                        row["规格"] = obj.Specs;
                        row["国际编码"] = obj.NationCode;
                        row["国家编码"] = obj.GBCode;
                        row["价格"] = obj.Price;
                        row["单位"] = obj.PriceUnit;
                        dtHisItem.Rows.Add(row);
                    }
                }
                this.myInterface.GetGBUNDRUGITEM(ref dtCenterItem);

                this.myInterface.GetUnDrugCompareItem(ref dtCompareItem);
                
                #endregion
            

            this.dtCenterItem.AcceptChanges();
            this.dtCompareItem.AcceptChanges();
            this.dtHisItem.AcceptChanges();
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="input"></param>
        private void FilterItem(string flag, string input)
        {
            string filterString = "";
            input = input.ToUpper();
            switch (flag)
            {
                case "HIS":
                    //switch (code)
                    //{
                    //    case "PY":
                    //        if (!this.checkBox1.Checked)
                    //        {
                    //            filterString = "拼音码" + " like '" + input + "%'";
                    //        }
                    //        else
                    //        {
                    //            filterString = "拼音码" + " like '%" + input + "%'";
                    //        }
                    //        break;
                    //    case "WB":
                    //        if (!this.checkBox1.Checked)
                    //        {
                    //            filterString = "五笔码" + " like '" + input + "%'";
                    //        }
                    //        else
                    //        {
                    //            filterString = "五笔码" + " like '%" + input + "%'";
                    //        }

                    //        break;
                    //    case "US":
                    //        if (!this.checkBox1.Checked)
                    //        {
                    //            filterString = "自定义码" + " like '" + input + "%'";
                    //        }
                    //        else
                    //        {
                    //            filterString = "自定义码" + " like '%" + input + "%'";
                    //        }

                    //        break;
                    //    case "ZW":
                    //        if (!this.checkBox1.Checked)
                    //        {
                    //            filterString = "药品名称" + " like '" + input + "%'";
                    //        }
                    //        else
                    //        {
                    //            filterString = "药品名称" + " like '%" + input + "%'";
                    //        }

                    //        break;
                    //    case "TYPY":
                    //        if (!this.checkBox1.Checked)
                    //        {
                    //            filterString = "通用名拼音" + " like '" + input + "%'";
                    //        }
                    //        else
                    //        {
                    //            filterString = "通用名拼音" + " like '%" + input + "%'";
                    //        }

                    //        break;
                    //    case "TYWB":
                    //        if (!this.checkBox1.Checked)
                    //        {
                    //            filterString = "通用名五笔" + " like '" + input + "%'";
                    //        }
                    //        else
                    //        {
                    //            filterString = "通用名五笔" + " like '%" + input + "%'";
                    //        }

                    //        break;
                    //}
                    filterString = "非药品编码" + " like '%" + input + "%'" + "or" + " 拼音码" + " like '%" + input + "%'" + "or" + " 五笔码" + " like '%" + input + "%'" + "or" + " 自定义码" + " like '" + input + "%'" + "or" + " 非药品名称" + " like '" + input + "%'";                   
                    this.dvHisItem.RowFilter = filterString;
                    InitColumnProHis();
                    break;
                case "CENTER":
                    if (!this.checkBox1.Checked)
                    {
                        filterString = "国家码（地方码）" + " like '" + input + "%'" + " or " + " 国家医保目录名称" + " like '" + input + "%'" + " or " + " 地方名称" + " like '" + input + "%'";
                    }
                    else
                    {
                        filterString = "国家码（地方码）" + " like '%" + input + "%'" + " or " + " 国家医保目录名称" + " like '%" + input + "%'" + " or " + " 地方名称" + " like '" + input + "%'";
                    }
                    this.dvCenterItem.RowFilter = filterString;
                    InitColumnProCenter();
                    break;
                case "COMPARE":
                    if (!this.checkBox1.Checked)
                    {
                        filterString = "非药品编码" + " like '" + input + "%'" + " or " + "非药品名称" + " like '" + input + "%'" + " or " + "国家医保目录名称" + " like '%" + input + "%'";
                    }
                    else
                    {
                        filterString = "非药品编码" + " like '%" + input + "%'" + " or " + "非药品名称" + " like '%" + input + "%'" + " or " + "国家医保目录名称" + " like '%" + input + "%'";
                    }
                    this.dvCompareItem.RowFilter = filterString;
                    break;
            }
        }
        /// <summary>
        /// 显示选择的本地信息
        /// </summary>
        /// <param name="row"></param>
        private void SetHisItemInfo(int row)
        {
            string hisCode = "";

                hisCode = this.fpHisItem_Sheet1.Cells[row, 0].Text.Trim();
                this.tbHisName.Text = this.fpHisItem_Sheet1.Cells[row, 1].Text;
                //this.tbHisPrice.Text = this.fpHisItem_Sheet1.Cells[row, 8].Text;

                Neusoft.HISFC.Models.Fee.Item.Undrug obj = this.GetSelectHisUndrugItem(hisCode);

                if (obj == null)
                {
                    MessageBox.Show("未找到选定本地非药品!");
                }
                else
                {
                    this.tbHisSpell.Tag = obj;
                }

            

            tabCompare.SelectedTab = this.tbCenterItem;
            this.tbCenterSpell.Focus();
        }
        /// <summary>
        /// 显示选择的中心信息
        /// </summary>
        /// <param name="row"></param>
        private void SetCenterItemInfo(int row)
        {
            string ID = "";
            string Name = "";
            string Memo = "";
            ID  = this.fpCenterItem_Sheet1.Cells[row, 0].Text.Trim();
            Name = this.fpCenterItem_Sheet1.Cells[row, 1].Text.Trim();
            Memo = this.fpCenterItem_Sheet1.Cells[row, 2].Text.Trim();
            Item obj = this.GetSelectCenterItem(ID, Name, Memo);

            if (obj == null)
            {
                MessageBox.Show("未找到中心药品");
            }
            else
            {
                tbCenterSpell.Tag = obj;
            }

            this.tbCenterName.Text = this.fpCenterItem_Sheet1.Cells[row, 1].Text;
            //this.tbCenterPrice.Text = this.fpCenterItem_Sheet1.Cells[row, 10].Text;
            this.tabCompare.SelectedTab = this.tbCompare;
        }
        /// <summary>
        /// 获得已选择HIS药品信息
        /// </summary>
        /// <param name="hisCode">医院药品代码</param>
        /// <returns>药品信息实体</returns>
        private Neusoft.HISFC.Models.Pharmacy.Item GetSelectHisItem(string hisCode)
        {
            Neusoft.HISFC.Models.Pharmacy.Item obj = new Neusoft.HISFC.Models.Pharmacy.Item();

            DataRow row = this.dtHisItem.Rows.Find(hisCode);

            obj.ID = row["药品编码"].ToString();
            obj.Name = row["药品名称"].ToString();
            obj.SpellCode = row["拼音码"].ToString();
            obj.WBCode = row["五笔码"].ToString();
            obj.UserCode = row["自定义码"].ToString();
            obj.Specs = row["规格"].ToString();
            obj.NameCollection.RegularName = row["通用名"].ToString();
            //obj.RegularSpellCode.Spell_Code = row["通用名拼音"].ToString();
            obj.NameCollection.SpellCode = row["通用名拼音"].ToString();
            obj.NameCollection.WBCode = row["通用名五笔"].ToString();
            obj.NameCollection.InternationalCode = row["国际编码"].ToString();
            obj.GBCode = row["国家编码"].ToString();
            obj.PriceCollection.RetailPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["价格"].ToString());
            obj.DosageForm.ID = row["剂型编码"].ToString();

            return obj;
        }
        /// <summary>
        /// 获得本地His非药品信息
        /// </summary>
        /// <param name="hisCode"></param>
        /// <returns></returns>
        private Neusoft.HISFC.Models.Fee.Item.Undrug GetSelectHisUndrugItem(string hisCode)
        {
            Neusoft.HISFC.Models.Fee.Item.Undrug obj = new Neusoft.HISFC.Models.Fee.Item.Undrug();

            DataRow row = this.dtHisItem.Rows.Find(hisCode);

            obj.ID = row["非药品编码"].ToString();
            obj.Name = row["非药品名称"].ToString();
            obj.SpellCode = row["拼音码"].ToString();
            obj.WBCode = row["五笔码"].ToString();
            obj.UserCode = row["自定义码"].ToString();
            obj.Specs = row["规格"].ToString();
            obj.NationCode = row["国际编码"].ToString();
            obj.GBCode = row["国家编码"].ToString();
            obj.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["价格"].ToString());
            obj.PriceUnit = row["单位"].ToString();


            return obj;
        }

        /// <summary>
        /// 获得已选中心项目信息
        /// </summary>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        private Neusoft.HISFC.Models.SIInterface.Item GetSelectCenterItem(string ID, string Name, string Memo)
        {
            Item obj = new Item();

            //DataRow row = this.dtCenterItem.Rows.Find(centerCode);

            obj.ID = ID;
            obj.Name = Name;
            obj.Memo = Memo;
            




            return obj;
        }
        /// <summary>
        /// 对照操作
        /// </summary>
        public void Compare()
        {
            Compare objCom = new Compare();

            
                //neusoft.HISFC.Models.Fee.Item objHis = new neusoft.HISFC.Models.Fee.Item();
                Neusoft.HISFC.Models.Fee.Item.Undrug objHis = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                Item objCenter = new Item();

                if (this.tbHisSpell.Tag == null || this.tbHisSpell.Tag.ToString() == "")
                {
                    MessageBox.Show("没有选择本地项目!");
                    return;
                }

                objHis = (Neusoft.HISFC.Models.Fee.Item.Undrug)this.tbHisSpell.Tag;

                if (tbCenterSpell.Tag == null || tbCenterSpell.Tag.ToString() == "")
                {
                    MessageBox.Show("没有选择中心项目");
                    return;
                }

                objCenter = (Item)this.tbCenterSpell.Tag;

                DataRow row = this.dtCompareItem.NewRow();

                row["非药品编码"] = objHis.ID;
                row["非药品名称"] = objHis.Name;
                row["国家码（地方码）"] = objCenter.ID;
                row["国家医保目录名称"] = objCenter.Name;
                row["地方名称"] = objCenter.Memo;
                row["操作时间"] = System.DateTime.Now;

                dtCompareItem.Rows.Add(row);

              

                DataRow rowFind = dtHisItem.Rows.Find(objHis.ID);
                dtHisItem.Rows.Remove(rowFind);
            



            //neusoft.neuFC.Management.Transaction t = new neusoft.neuFC.Management.Transaction(neusoft.neuFC.Management.Connection.Instance);
            //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(Neusoft.FrameWork.Management.Connection.Instance);
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            myInterface.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            int returnValue = 0;
            returnValue = myInterface.InsertCompareItem(objHis.ID,objCenter.ID);
            if (returnValue == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("对照失败!" + myInterface.Err);
                return;
            }
            myInterface.InetCompareItem(objHis.ID, objHis.Name, objCenter.ID, objCenter.Name, myInterface.Operator.ID);//插入珠海医保等级维护里
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            Clear();
            this.tbHisSpell.Focus();
        }
        /// <summary>
        /// 删除已对照信息
        /// </summary>
        public void Delete()
        {
            int rowAct = this.fpCompareItem_Sheet1.ActiveRowIndex;
            if (this.fpCompareItem_Sheet1.RowCount <= 0)
                return;

            string hisCode = "";
            hisCode = this.fpCompareItem_Sheet1.Cells[rowAct, 0].Text;

            if (hisCode == "" || hisCode == null)
                return;

            //neusoft.neuFC.Management.Transaction t = new neusoft.neuFC.Management.Transaction(neusoft.neuFC.Management.Connection.Instance);
            //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(Neusoft.FrameWork.Management.Connection.Instance);
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            myInterface.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            int returnValue = 0;

            returnValue = myInterface.DeleteCompareItem("16", hisCode);

            if (returnValue == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("删除对照失败!" + myInterface.Err);
                return;
            }
            myInterface.DeleteCompareItem("14", hisCode); //删除对照
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            DataRow row = this.dtCompareItem.Rows.Find(hisCode);

            DataRow rowHis = dtHisItem.NewRow();

            rowHis["非药品编码"] = row["非药品编码"].ToString();
            rowHis["非药品名称"] = row["非药品名称"].ToString();
                //				rowHis["国际编码"] = row["国际编码"].ToString();
                //				rowHis["国家编码"] = row["国家编码"].ToString();
                //				rowHis["单位"] = row["单位"].ToString();
            
            //rowHis["拼音码"] = row["医院拼音"].ToString();
            //rowHis["五笔码"] = row["医院五笔码"].ToString();
            //rowHis["自定义码"] = row["医院自定义码"].ToString();
            //rowHis["规格"] = row["医院规格"].ToString();
            //rowHis["价格"] = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["医院基本价格"].ToString());


            dtCompareItem.Rows.Remove(row);
            dtHisItem.Rows.Add(rowHis);


        }
        /// <summary>
        /// 清空信息
        /// </summary>
        public void Clear()
        {
            //this.tbCenterSpell.Text = "";
            this.tbCenterSpell.Tag = "";
            this.tbCenterName.Text = "";
            


            this.tbHisSpell.Tag = "";
            this.tbHisName.Text = "";
        }

        /// <summary>
        /// 保存函数
        /// </summary>
        public void Save()
        {
            //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(Neusoft.FrameWork.Management.Connection.Instance);
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            myInterface.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            int returnValue = 0;

            ArrayList alAdd = GetAddCompareItem();

            if (alAdd != null)
            {
                foreach (Compare obj in alAdd)
                {
                    returnValue = myInterface.InsertCompareItem(obj);
                    if (returnValue == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("插入对照信息出错!" + myInterface.Err);
                        return;
                    }
                }
            }

            ArrayList alDelete = GetDeleteCompareItem();

            if (alDelete != null)
            {
                foreach (Compare obj in alDelete)
                {
                    returnValue = myInterface.DeleteCompareItem(this.pactCode.ID, obj.HisCode);
                    if (returnValue == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("删除对照信息出错!" + myInterface.Err);
                        return;
                    }
                }
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("保存成功!");
        }

        public void Close()
        {

        }

        /// <summary>
        /// 导出当前项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <returns></returns>
        public override int Export(object sender, object neuObject)
        {
            object obj = this.hashTableFp[this.tabCompare.SelectedTab];

            FarPoint.Win.Spread.FpSpread fp = obj as FarPoint.Win.Spread.FpSpread;

            SaveFileDialog op = new SaveFileDialog();

            op.Title = "请选择保存的路径和名称";
            op.CheckFileExists = false;
            op.CheckPathExists = true;
            op.DefaultExt = "*.xls";
            op.Filter = "(*.xls)|*.xls";

            DialogResult result = op.ShowDialog();

            if (result == DialogResult.Cancel || op.FileName == string.Empty)
            {
                return -1;
            }

            string filePath = op.FileName;

            bool returnValue = fp.SaveExcel(filePath, FarPoint.Win.Spread.Model.IncludeHeaders.ColumnHeadersCustomOnly);
                                


            return base.Export(sender, neuObject);
        }
        /// <summary>
        /// 获得新增项目
        /// </summary>
        /// <returns></returns>
        private ArrayList GetAddCompareItem()
        {
            DataTable dt = this.dtCompareItem.GetChanges(DataRowState.Added);
            ArrayList al = new ArrayList();
            if (dt == null)
            {
                return null;
            }
            foreach (DataRow row in dt.Rows)
            {
                Compare obj = new Compare();

                obj.CenterItem.PactCode = pactCode.ID;
                obj.HisCode = row["本地项目编码"].ToString();
                obj.CenterItem.ID = row["中心编码"].ToString();
                obj.CenterItem.SysClass = row["项目类别"].ToString();
                obj.CenterItem.Name = row["医保收费项目中文名称"].ToString();
                obj.CenterItem.EnglishName = row["医保收费项目英文名称"].ToString();
                obj.Name = row["本地项目名称"].ToString();
                obj.RegularName = row["本地项目别名"].ToString();
                obj.CenterItem.DoseCode = row["医保剂型"].ToString();
                obj.CenterItem.Specs = row["医保规格"].ToString();
                obj.CenterItem.SpellCode = row["医保拼音代码"].ToString();
                obj.CenterItem.FeeCode = row["医保费用分类代码"].ToString();
                obj.CenterItem.ItemType = row["医保目录级别"].ToString();
                if (this.IsUseNullItemGrade)
                {
                    obj.CenterItem.ItemGrade = "";
                }
                else
                {
                    obj.CenterItem.ItemGrade = row["医保目录等级"].ToString();
                }
                obj.CenterItem.Rate = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["自负比例"].ToString());
                obj.CenterItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["基准价格"].ToString());
                obj.CenterItem.Memo = row["限制使用说明"].ToString();
                obj.SpellCode.SpellCode = row["医院拼音"].ToString();
                obj.SpellCode.WBCode = row["医院五笔码"].ToString();
                obj.SpellCode.UserCode = row["医院自定义码"].ToString();
                obj.Specs = row["医院规格"].ToString();
                obj.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["医院基本价格"].ToString());
                obj.DoseCode = row["医院剂型"].ToString();
                obj.CenterItem.OperCode = row["操作员"].ToString();
                //obj.CenterItem.OperDate = Convert.ToDateTime(row["操作时间"].ToString());
                //南庄修改 {87ED5A6B-F317-4579-9BC9-660182F49333}
                obj.CenterItem.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(row["操作时间"].ToString());

                al.Add(obj);
            }

            return al;
        }

        private ArrayList GetDeleteCompareItem()
        {
            //dtCompareItem.RejectChanges();

            DataTable dt = this.dtCompareItem.GetChanges(DataRowState.Deleted);

            ArrayList al = new ArrayList();
            if (dt == null)
            {
                return null;
            }
            foreach (DataRow row in dt.Rows)
            {
                Compare obj = new Compare();

                obj.CenterItem.PactCode = pactCode.ID;
                obj.HisCode = row["本地项目编码"].ToString();
                obj.CenterItem.ID = row["中心编码"].ToString();
                obj.CenterItem.SysClass = row["项目类别"].ToString();
                obj.CenterItem.Name = row["医保收费项目中文名称"].ToString();
                obj.CenterItem.EnglishName = row["医保收费项目英文名称"].ToString();
                obj.Name = row["本地项目名称"].ToString();
                obj.RegularName = row["本地项目别名"].ToString();
                obj.CenterItem.DoseCode = row["医保剂型"].ToString();
                obj.CenterItem.Specs = row["医保规格"].ToString();
                obj.CenterItem.SpellCode = row["医保拼音代码"].ToString();
                obj.CenterItem.FeeCode = row["医保费用分类代码"].ToString();
                obj.CenterItem.ItemType = row["医保目录级别"].ToString();
                obj.CenterItem.ItemGrade = row["医保目录等级"].ToString();
                obj.CenterItem.Rate = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["自负比例"].ToString());
                obj.CenterItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["基准价格"].ToString());
                obj.CenterItem.Memo = row["限制使用说明"].ToString();
                obj.SpellCode.SpellCode = row["医院拼音"].ToString();
                obj.SpellCode.WBCode = row["医院五笔码"].ToString();
                obj.SpellCode.UserCode = row["医院自定义码"].ToString();
                obj.Specs = row["医院规格"].ToString();
                obj.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(row["医院基本价格"].ToString());
                obj.DoseCode = row["医院剂型"].ToString();
                obj.CenterItem.OperCode = row["操作员"].ToString();
                //obj.CenterItem.OperDate = Convert.ToDateTime(row["操作时间"].ToString());
                //{87ED5A6B-F317-4579-9BC9-660182F49333} 南庄修改
                obj.CenterItem.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(row["操作时间"].ToString());

                al.Add(obj);
            }

            this.dtCompareItem.AcceptChanges();

            return al;
        }

        #endregion

        #region 事件
        private void tbHisSpell_TextChanged(object sender, EventArgs e)
        {
            this.FilterItem("HIS", this.tbHisSpell.Text);
        }

        private void tbCenterSpell_TextChanged(object sender, EventArgs e)
        {
            this.FilterItem("CENTER", this.tbCenterSpell.Text);
        }



        private void tbHisSpell_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.fpHisItem_Sheet1.RowCount <= 0)
            {
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                this.fpHisItem.SetViewportTopRow(0, this.fpHisItem_Sheet1.ActiveRowIndex - 5);
                this.fpHisItem_Sheet1.ActiveRowIndex--;
                this.fpHisItem_Sheet1.AddSelection(this.fpHisItem_Sheet1.ActiveRowIndex, 0, 1, 0);
            }
            if (e.KeyCode == Keys.Down)
            {
                this.fpHisItem.SetViewportTopRow(0, this.fpHisItem_Sheet1.ActiveRowIndex - 4);
                this.fpHisItem_Sheet1.ActiveRowIndex++;
                this.fpHisItem_Sheet1.AddSelection(this.fpHisItem_Sheet1.ActiveRowIndex, 0, 1, 0);
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (this.fpHisItem_Sheet1.RowCount >= 0)
                {
                    SetHisItemInfo(this.fpHisItem_Sheet1.ActiveRowIndex);
                }
            }
        }

        private void fpHisItem_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (this.fpHisItem_Sheet1.RowCount >= 0)
                SetHisItemInfo(this.fpHisItem_Sheet1.ActiveRowIndex);
        }

        private void fpCenterItem_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (this.fpHisItem_Sheet1.RowCount >= 0)
            {
                SetCenterItemInfo(this.fpCenterItem_Sheet1.ActiveRowIndex);
            }
        }

        private void tbCenterSpell_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.fpCenterItem_Sheet1.RowCount <= 0)
            {
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                this.fpCenterItem.SetViewportTopRow(0, this.fpCenterItem_Sheet1.ActiveRowIndex - 5);
                this.fpCenterItem_Sheet1.ActiveRowIndex--;
                this.fpCenterItem_Sheet1.AddSelection(this.fpCenterItem_Sheet1.ActiveRowIndex, 0, 1, 0);
            }
            if (e.KeyCode == Keys.Down)
            {
                this.fpCenterItem.SetViewportTopRow(0, this.fpCenterItem_Sheet1.ActiveRowIndex - 4);
                this.fpCenterItem_Sheet1.ActiveRowIndex++;
                this.fpCenterItem_Sheet1.AddSelection(this.fpCenterItem_Sheet1.ActiveRowIndex, 0, 1, 0);
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (this.fpHisItem_Sheet1.RowCount >= 0)
                {
                    SetCenterItemInfo(this.fpCenterItem_Sheet1.ActiveRowIndex);
                }
            }
        }

        private void tbHisSpell_Enter(object sender, EventArgs e)
        {
            this.tabCompare.SelectedIndex = 0;
        }

        private void tbCenterSpell_Enter(object sender, EventArgs e)
        {
            this.tabCompare.SelectedIndex = 1;
        }

        private void tbCompareQuery_TextChanged(object sender, EventArgs e)
        {
            FilterItem("COMPARE", this.tbCompareQuery.Text);
        }

        private void ucCompare_Load(object sender, EventArgs e)
        {
            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载数据，请稍后^^");
            Application.DoEvents();
            ////compareType = base.Tag.ToString();// this.FindForm().Tag.ToString();
            //if (this.Tag.ToString() == "DALL")
            //{
            //    drugType.ID = "ALL";
            //    drugType.Name = "全部";
            //}
            //else
            //{
            //    drugType.ID = compareType.Substring(3, 1);
            //    switch (drugType.ID)
            //    {
            //        case "P":
            //            drugType.Name = "西药";
            //            break;
            //        case "Z":
            //            drugType.Name = "中成药";
            //            break;
            //        case "C":
            //            drugType.Name = "草药";
            //            break;
            //        case "U":
            //            drugType.Name = "非药品";
            //            break;
            //    }
            //}
            this.CompareType = this.compareType;
            this.Init();
            //this.pactCode.ID = "2";
            Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
        }


        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "对照":
                    {
                        this.Compare();
                        break;
                    }
                case "取消":
                    {
                        this.Delete();
                        break;
                    }
                //case "修改":
                //    {
                //        this.ModifyCompare();
                //        break;
                //    }
            }
            base.ToolStrip_ItemClicked(sender, e);
        }

        #endregion 



    }
}
