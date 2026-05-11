using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.Models.SIInterface;

namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    public partial class ucModifyCompare : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucModifyCompare()
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
        protected Neusoft.HISFC.BizLogic.Fee.Interface myInterface = new Neusoft.HISFC.BizLogic.Fee.Interface();
        private CompareTypes compareType;
        private CompareUndrugTypes compareUndrugType;
        DataTable dtCompareItem = new DataTable();
        DataView dvCompareItem = new DataView();
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
        #endregion

        #region 方法

        /// <summary>
        /// 初始化显示数据
        /// </summary>
        public void Init()
        {
          
            InitColumn();

            InitData();

            InitColumnProCompare();
        }

        /// <summary>
        /// 设置显示列信息;
        /// </summary>
        private void InitColumn()
        {
            Type str = typeof(System.String);
            Type dec = typeof(System.Decimal);
            Type date = typeof(System.DateTime);
            DataColumn[] colCompareItem = new DataColumn[]{ 
                                                            new DataColumn("医保目录等级", str),
                                                            new DataColumn("医院自定义码", str),
                                                            new DataColumn("本地项目编码", str),
                                                            new DataColumn("中心编码", str),
                                                            new DataColumn("项目类别", str),
                                                            new DataColumn("医保收费项目中文名称", str),
                                                            new DataColumn("本地项目名称", str),
                                                            new DataColumn("本地项目别名", str),
                                                            new DataColumn("药监局编码",str),
                                                            new DataColumn("医保收费项目英文名称", str),
                                                            new DataColumn("医保剂型", str),
                                                            new DataColumn("医保规格",str),
                                                            new DataColumn("医保拼音代码", str),
                                                            new DataColumn("医保费用分类代码", str),
                                                            new DataColumn("医保目录级别", str),
                                                            new DataColumn("自负比例", dec),
                                                            new DataColumn("基准价格", dec),
                                                            new DataColumn("限制使用说明", str),
                                                            new DataColumn("医院拼音", str),
                                                            new DataColumn("医院五笔码", str),
                                                            new DataColumn("医院规格", str),
                                                            new DataColumn("医院基本价格", dec),
                                                            new DataColumn("医院剂型", str),
                                                            new DataColumn("操作员", str),
                                                            new DataColumn("操作时间", date),
                                                            new DataColumn("修改标识", str)};
            dtCompareItem.Columns.AddRange(colCompareItem);
            DataColumn[] keyCompare = new DataColumn[2];
            keyCompare[0] = dtCompareItem.Columns[2];
            dtCompareItem.CaseSensitive = true;
            dtCompareItem.PrimaryKey = keyCompare;
            dvCompareItem = new DataView(dtCompareItem);
            dvCompareItem.Sort = "操作时间 DESC";
            fpCompareItem_Sheet1.DataSource = dvCompareItem;
        }


        /// <summary>
        /// 属性列设置
        /// </summary>
        private void InitColumnProCompare()
        {
            int width = 20;

            FarPoint.Win.Spread.CellType.DateTimeCellType dtType = new FarPoint.Win.Spread.CellType.DateTimeCellType();
            dtType.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.ShortDateWithTime;
            
            fpCompareItem_Sheet1.Columns[0].Visible = true;
            fpCompareItem_Sheet1.Columns[0].Width = width*4;
            fpCompareItem_Sheet1.Columns[1].Visible = true;
            fpCompareItem_Sheet1.Columns[1].Width = 0;
            fpCompareItem_Sheet1.Columns[2].Visible = true;
            fpCompareItem_Sheet1.Columns[3].Width = width * 8;
            fpCompareItem_Sheet1.Columns[4].Width = 0;
            fpCompareItem_Sheet1.Columns[5].Width = width * 8;
            fpCompareItem_Sheet1.Columns[6].Visible = true;
            fpCompareItem_Sheet1.Columns[6].Width = width * 8;
            fpCompareItem_Sheet1.Columns[7].Visible = true;
            fpCompareItem_Sheet1.Columns[7].Width = width * 4;
            fpCompareItem_Sheet1.Columns[8].Visible = false;
            fpCompareItem_Sheet1.Columns[9].Visible = false;
            fpCompareItem_Sheet1.Columns[10].Visible = false;
            fpCompareItem_Sheet1.Columns[11].Width = width * 4;
            fpCompareItem_Sheet1.Columns[12].Visible = true;
            fpCompareItem_Sheet1.Columns[13].Width = width * 4;
            fpCompareItem_Sheet1.Columns[14].Width = width * 4;
            fpCompareItem_Sheet1.Columns[15].Width = width * 4;
            fpCompareItem_Sheet1.Columns[16].Width = width * 6;
            fpCompareItem_Sheet1.Columns[17].Visible = false;
            fpCompareItem_Sheet1.Columns[18].Visible = false;
            fpCompareItem_Sheet1.Columns[19].Visible = false;
            fpCompareItem_Sheet1.Columns[20].Width = width * 8;
            fpCompareItem_Sheet1.Columns[21].Width = width * 4;
            fpCompareItem_Sheet1.Columns[22].Width = width * 4;
            fpCompareItem_Sheet1.Columns[23].Width = width * 4;
            fpCompareItem_Sheet1.Columns[24].Width = width * 6;
            fpCompareItem_Sheet1.Columns[24].CellType = dtType;
            fpCompareItem_Sheet1.Columns[25].Width = 0;

            fpCompareItem_Sheet1.Columns[0].Locked = false;
            fpCompareItem_Sheet1.Columns[1].Locked = true;
            fpCompareItem_Sheet1.Columns[2].Locked = true;
            fpCompareItem_Sheet1.Columns[3].Locked = true;
            fpCompareItem_Sheet1.Columns[4].Locked = true;
            fpCompareItem_Sheet1.Columns[5].Locked = true;
            fpCompareItem_Sheet1.Columns[6].Locked = true;
            fpCompareItem_Sheet1.Columns[7].Locked = true;
            fpCompareItem_Sheet1.Columns[8].Locked = true;
            fpCompareItem_Sheet1.Columns[9].Locked = true;
            fpCompareItem_Sheet1.Columns[10].Locked = true;
            fpCompareItem_Sheet1.Columns[11].Locked = true;
            fpCompareItem_Sheet1.Columns[12].Locked = true;
            fpCompareItem_Sheet1.Columns[13].Locked = true;
            fpCompareItem_Sheet1.Columns[14].Locked = true;
            fpCompareItem_Sheet1.Columns[15].Locked = true;
            fpCompareItem_Sheet1.Columns[16].Locked = true;
            fpCompareItem_Sheet1.Columns[17].Locked = true;
            fpCompareItem_Sheet1.Columns[18].Locked = true;
            fpCompareItem_Sheet1.Columns[19].Locked = true;
            fpCompareItem_Sheet1.Columns[20].Locked = true;
            fpCompareItem_Sheet1.Columns[21].Locked = true;
            fpCompareItem_Sheet1.Columns[22].Locked = true;
            fpCompareItem_Sheet1.Columns[23].Locked = true;
            fpCompareItem_Sheet1.Columns[24].Locked = true;
            fpCompareItem_Sheet1.Columns[25].Locked = true;
        }

         /// <summary>
        /// 初始化显示数据
        /// </summary>
        public void InitData()
        {
            System.Collections.ArrayList alCompareItem = new System.Collections.ArrayList();
            alCompareItem = this.myInterface.GetCompareItem("14", compareType.ToString());

            if (alCompareItem != null)
            {
                foreach (Neusoft.HISFC.Models.SIInterface.Compare obj in alCompareItem)
                {
                    DataRow row = dtCompareItem.NewRow();

                    row["医保目录等级"] = obj.CenterItem.ItemGrade;
                    row["本地项目编码"] = obj.HisCode;
                    row["中心编码"] = obj.CenterItem.ID;
                    row["项目类别"] = obj.CenterItem.SysClass;
                    row["医保收费项目中文名称"] = obj.CenterItem.Name;
                    row["医保收费项目英文名称"] = obj.CenterItem.EnglishName;
                    row["本地项目名称"] = obj.Name;
                    row["本地项目别名"] = obj.RegularName;
                    row["医保剂型"] = obj.CenterItem.DoseCode;
                    row["医保拼音代码"] = obj.CenterItem.SpellCode;
                    row["医保费用分类代码"] = obj.CenterItem.FeeCode;
                    row["医保目录级别"] = obj.CenterItem.ItemType;
                    row["自负比例"] = obj.CenterItem.Rate;
                    row["基准价格"] = obj.CenterItem.Price;
                    row["限制使用说明"] = obj.CenterItem.Memo;
                    row["医院拼音"] = obj.SpellCode.SpellCode;
                    row["医院五笔码"] = obj.SpellCode.WBCode;
                    row["医院自定义码"] = obj.SpellCode.UserCode;
                    row["医院规格"] = obj.Specs;
                    row["医院基本价格"] = obj.Price;
                    row["医院剂型"] = obj.DoseCode;
                    row["操作员"] = obj.CenterItem.OperCode;
                    row["操作时间"] = obj.CenterItem.OperDate;
                    row["修改标识"] = "0";
                    dtCompareItem.Rows.Add(row);
                }
            }
        }
       
        private void ucModifyCompare_Load(object sender, EventArgs e)
        {
            Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载数据，请稍后^^");
            Application.DoEvents();
            
            this.Init();
            Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
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
                case "COMPARE":
                    filterString = "医院拼音" + " like '%" + input + "%'" + " or " + "医院自定义码" + " like '%" + input + "%'" + " or " + "本地项目名称" + " like '%" + input + "%'";
                    this.dvCompareItem.RowFilter = filterString;
                    break;
            }
        }

        /// <summary>
        /// 过滤框内容变更时出发过滤方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbCompareQuery_TextChanged(object sender, EventArgs e)
        {
            FilterItem("COMPARE", this.tbCompareQuery.Text);
        }

        /// <summary>
        /// 获得新增项目
        /// </summary>
        /// <returns></returns>
        private System.Collections.ArrayList GetModifyCompareItem()
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            for(int i = 0;i<fpCompareItem_Sheet1.Rows.Count;i++)
            {
                if (fpCompareItem_Sheet1.Cells[i,25].Text.ToString() == "1")
                {
                    Compare obj = new Compare();

                    obj.CenterItem.PactCode = "14";
                    obj.CenterItem.ItemGrade = fpCompareItem_Sheet1.Cells[i, 0].Text.ToString();
                    obj.HisCode = fpCompareItem_Sheet1.Cells[i, 2].Text.ToString();
                    obj.CenterItem.ID = fpCompareItem_Sheet1.Cells[i, 3].Text.ToString();
                    obj.CenterItem.SysClass = fpCompareItem_Sheet1.Cells[i, 4].Text.ToString();
                    obj.CenterItem.Name = fpCompareItem_Sheet1.Cells[i,5].Text.ToString();
                    obj.CenterItem.EnglishName = fpCompareItem_Sheet1.Cells[i, 9].Text.ToString();
                    obj.Name = fpCompareItem_Sheet1.Cells[i, 6].Text.ToString();//fpCompareItem_Sheet1.Cells[i, 9].Text.ToString()不知道之前维护要取拼音码
                    obj.RegularName = fpCompareItem_Sheet1.Cells[i, 7].Text.ToString();
                    obj.CenterItem.DoseCode = fpCompareItem_Sheet1.Cells[i, 10].Text.ToString();
                    obj.CenterItem.SpellCode = fpCompareItem_Sheet1.Cells[i, 12].Text.ToString();
                    obj.CenterItem.FeeCode = fpCompareItem_Sheet1.Cells[i, 13].Text.ToString();
                    obj.CenterItem.ItemType = fpCompareItem_Sheet1.Cells[i, 14].Text.ToString();
                    obj.CenterItem.Rate = Neusoft.FrameWork.Function.NConvert.ToDecimal(fpCompareItem_Sheet1.Cells[i, 15].Text.ToString());
                    obj.CenterItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(fpCompareItem_Sheet1.Cells[i, 16].Text.ToString());
                    obj.CenterItem.Memo = fpCompareItem_Sheet1.Cells[i, 17].Text.ToString();
                    obj.SpellCode.SpellCode = fpCompareItem_Sheet1.Cells[i, 18].Text.ToString();
                    obj.SpellCode.WBCode = fpCompareItem_Sheet1.Cells[i, 19].Text.ToString();
                    obj.SpellCode.UserCode = fpCompareItem_Sheet1.Cells[i, 1].Text.ToString();
                    obj.Specs = fpCompareItem_Sheet1.Cells[i, 20].Text.ToString();
                    obj.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(fpCompareItem_Sheet1.Cells[i, 21].Text.ToString());
                    obj.DoseCode = fpCompareItem_Sheet1.Cells[i, 22].Text.ToString();
                    obj.CenterItem.OperCode = myInterface.Operator.ID;
                    obj.CenterItem.OperDate = System.DateTime.Now;

                    al.Add(obj);
                }
            }

            return al;
        }

        protected override int OnSave(object sender, object neuObject)
        {
            this.ModifyCompare();
            return 1;
        }
        private void fpCompareItem_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpCompareItem_Sheet1.Cells[fpCompareItem_Sheet1.ActiveRowIndex, 25].Text = "1";
        }

        /// <summary>
        /// 修改保存
        /// </summary>
        private void ModifyCompare()
        {
            int returnValue = 0;
           System.Collections.ArrayList al = GetModifyCompareItem();

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            myInterface.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            if (al != null)
            {
                foreach (Compare obj in al)
                {
                    returnValue = myInterface.DeleteCompareItem("14", obj.HisCode);
                    if (returnValue == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("删除对照信息出错!" + myInterface.Err);
                        return;
                    }
                }
            }

            if (al != null)
            {
                foreach (Compare obj in al)
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

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            MessageBox.Show("保存成功!");
        }
        #endregion

    }
}
