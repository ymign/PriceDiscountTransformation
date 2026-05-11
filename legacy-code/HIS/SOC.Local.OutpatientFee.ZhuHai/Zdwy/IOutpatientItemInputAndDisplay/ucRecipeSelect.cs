using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientItemInputAndDisplay
{
    /// <summary>
    /// ucRecipeSelect<br></br>
    /// [功能描述: 处方选择UC]<br></br>
    /// [创 建 者: cube]<br></br>
    /// [创建时间: 2010.09.10]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public partial class ucRecipeSelect : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucRecipeSelect()
        {
            InitializeComponent();
            this.Tag = DialogResult.Cancel;
            this.Load += new EventHandler(ucRecipeSelect_Load);
        }

        System.Collections.ArrayList alFeeItemList = new System.Collections.ArrayList();


        /// <summary>
        /// 非药品组合项目业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.UndrugPackAge undrugPackAgeManager = new Neusoft.HISFC.BizLogic.Fee.UndrugPackAge();


        /// <summary>
        /// 费用实体数组
        /// </summary>
        public System.Collections.ArrayList FeeItemList
        {
            get
            {
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int rowIndex = 0; rowIndex < this.alFeeItemList.Count; rowIndex++)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f = this.sheetView1.Rows[rowIndex].Tag as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                    if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.sheetView1.Cells[rowIndex, 0].Value))
                    {
                        al.Add(f);
                    }

                }
                return al;
            }
        }
        /// <summary>
        /// 显示处方列表
        /// </summary>
        /// <param name="cardNO">病例号</param>
        /// <param name="dtBegin">开始时间</param>
        /// <param name="dtEnd">结束时间</param>
        /// <param name="isShowDetail">是否需要显示明细数据</param>
        /// <returns>1成功</returns>
        public int ShowFeeItemList(string cardNO, DateTime dtBegin, DateTime dtEnd, bool isShowDetail)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outPatientMgr = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            System.Data.DataSet ds = new DataSet();
            //sql 的第3个字段必须是处方号
            if (outPatientMgr.ExecQuery("Fee.OutPatient.RecipeInfo.Query.1", ref ds, cardNO, dtBegin.ToString(), dtEnd.ToString()) == -1)
            {
                MessageBox.Show("获取历史处方信息出错："+outPatientMgr.Err);
                return -1;
            }
            if (ds == null || ds.Tables.Count == 0)
            {
                MessageBox.Show("获取历史处方信息出错：" + outPatientMgr.Err);
                return -1;
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("没有找到处方信息");
                return 0;
            }
            this.fpSpread1_Sheet1.DataSource = ds;
            this.SetFormat();
            this.fpSpread1_Sheet1.ActiveRowIndex = 0;
            this.npanelDetail.Visible = isShowDetail;
            if (!isShowDetail)
            {
                this.ncbCheckAll.Checked = true;
                this.ncbCheckAll.Visible = false;
            }
            return 1;
        }

        /// <summary>
        /// 明细把组套
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ArrayList ConvertDetailToGroup(ArrayList f)
        {
            Hashtable FeeItemHt = new Hashtable();

            ArrayList feeListAl = new ArrayList();

            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList al in f)
            {

                if (FeeItemHt.Contains(al.UndrugComb.ID))
                {
                    continue;
                }
                else
                {
                    if (al.UndrugComb.ID != "")
                    {
                        FeeItemHt.Add(al.UndrugComb.ID, al);
                        al.Item.Price = this.undrugPackAgeManager.GetUndrugCombPrice(al.UndrugComb.ID);
                        al.Item.ID = al.UndrugComb.ID;
                        al.Item.Name = al.UndrugComb.Name;
                        al.FT.OwnCost = al.Item.Qty * al.Item.Price;
                        al.FT.TotCost = al.Item.Qty * al.Item.Price;
                        feeListAl.Add(al);

                    }
                    else
                    {

                        feeListAl.Add(al);
                    }

                }

            }

            return feeListAl;


        }


        /// <summary>
        /// 根据处方号获取处方明细
        /// </summary>
        /// <param name="recipeNO"></param>
        private void QueryRecipeDetail(string recipeNO)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outPatientMgr = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            this.alFeeItemList = outPatientMgr.QueryFeeDetailFromRecipeNOForHistoryRecipe(recipeNO);
            if (this.alFeeItemList == null)
            {
                MessageBox.Show("没有找到处方" + recipeNO + "的信息");
            } if (alFeeItemList != null)
            {
                alFeeItemList = ConvertDetailToGroup(alFeeItemList);
                this.sheetView1.RowCount = alFeeItemList.Count;
                for (int rowIndex = 0; rowIndex < this.alFeeItemList.Count; rowIndex++)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f = alFeeItemList[rowIndex] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                    this.sheetView1.Cells[rowIndex, 0].Value = this.ncbCheckAll.Checked;
                    this.sheetView1.Cells[rowIndex, 1].Value = f.Item.Name + "[" + f.Item.Specs + "]";
                    this.sheetView1.Cells[rowIndex, 2].Value = f.Item.Price;
                    if (f.FeePack == "1")
                    {
                        this.sheetView1.Cells[rowIndex, 3].Value = f.Item.Qty / f.Item.PackQty;
                    }
                    else
                    {
                        this.sheetView1.Cells[rowIndex, 3].Value = f.Item.Qty;
                    }
                    this.sheetView1.Cells[rowIndex, 4].Value = f.Item.PriceUnit;
                    this.sheetView1.Rows[rowIndex].Tag = f;

                }
            }

            this.Tag = DialogResult.OK;
        }
      

        /// <summary>
        /// 格式化FarPoint
        /// </summary>
        private void SetFormat()
        {
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 60F;
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 77F;
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 68F;
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 105F;
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 123F;
         
        }

        void ucRecipeSelect_Load(object sender, EventArgs e)
        {
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(fpSpread1_CellDoubleClick);
            this.fpSpread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(fpSpread1_CellClick);
            this.neuSpread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(neuSpread1_CellClick);
            this.nbtOK.Click += new EventHandler(nbtOK_Click);
            this.nbtCancel.Click += new EventHandler(nbtCancel_Click);
            this.ncbCheckAll.CheckedChanged += new EventHandler(ncbCheckAll_CheckedChanged);
        }

        void neuSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            
            this.sheetView1.Cells[e.Row, 0].Value = !Neusoft.FrameWork.Function.NConvert.ToBoolean(this.sheetView1.Cells[e.Row, 0].Value);
        }

        void ncbCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int rowIndex = 0; rowIndex < this.alFeeItemList.Count; rowIndex++)
            {
                this.sheetView1.Cells[rowIndex, 0].Value = this.ncbCheckAll.Checked;
            }
        }

        void nbtCancel_Click(object sender, EventArgs e)
        {
            if (this.alFeeItemList != null)
            {
                this.alFeeItemList.Clear();
            }
            this.Tag = DialogResult.Cancel;
            if (this.FindForm() != null)
            {
                this.FindForm().Close();
            }
        }

        void nbtOK_Click(object sender, EventArgs e)
        {
            if (this.fpSpread1_Sheet1.ActiveRowIndex != -1 && !this.npanelDetail.Visible)
            {
                string recipeNO = this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.ActiveRowIndex, 2].Text;
                this.QueryRecipeDetail(recipeNO);
            }
            this.Tag = DialogResult.OK;
            if (this.FindForm() != null)
            {
                this.FindForm().Close();
            }
        }

        void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                string recipeNO = this.fpSpread1_Sheet1.Cells[e.Row, 2].Text;
                this.QueryRecipeDetail(recipeNO);

    
            }
            catch
            { }
        }

        void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {

                this.FindForm().Close();
                //string recipeNO = this.fpSpread1_Sheet1.Cells[e.Row, 2].Text;
                //this.QueryRecipeDetail(recipeNO);

                //if (!this.npanelDetail.Visible && this.FindForm() != null)
                //{
                //    this.FindForm().Close();
                //}
            }
            catch
            { }
        }

    }
}
