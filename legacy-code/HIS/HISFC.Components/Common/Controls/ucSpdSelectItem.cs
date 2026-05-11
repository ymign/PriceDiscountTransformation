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
    public partial class ucSpdSelectItem : UserControl
    {
        public ucSpdSelectItem()
        {
            InitializeComponent();
            isUserICD9 = controlParamMgr.GetControlParam("USEICD", true, false);
        }
        private Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamMgr = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        private HisCallExternalServiceProject.FunctionModule.SPDModule.SPDService spdService = new HisCallExternalServiceProject.FunctionModule.SPDModule.SPDService();
        List<HisCallExternalServiceProject.FunctionModule.SPDModule.Model.SpdStockUseModel> spdStockUseList = new List<HisCallExternalServiceProject.FunctionModule.SPDModule.Model.SpdStockUseModel>();
        private DataSet ds = null;
        public delegate int MyDelegate(Keys key);
        /// <summary>
        /// 双击、回车项目列表时执行的事件
        /// </summary>
        public event MyDelegate SelectItem;

        private bool isUserICD9 = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            
            //if (spdService.GetSpdStock(ref spdStockUseList) < 0)
            //{
            //    MessageBox.Show("获取Spd系统库存视图错误：" + this.spdService.ErrMsg);
            //    return -1;
            //}
            //if (spdStockUseList.Count < 0)
            //{
            //    MessageBox.Show("SPD视图获取库存数据为空！");
            //    return -1;
            //}

            #region 生成DataSet
            ds = new DataSet();
            ds.Tables.Add("items");
            ds.Tables[0].Columns.AddRange(new DataColumn[]
				{
					new DataColumn("MaterialCode",Type.GetType("System.String")),
					new DataColumn("RFID",Type.GetType("System.String")),
					new DataColumn("Lot",Type.GetType("System.Decimal")),
					new DataColumn("Exp",Type.GetType("System.String")),
					new DataColumn("StockQty",Type.GetType("System.String")),
					new DataColumn("deptCode",Type.GetType("System.String")),
					new DataColumn("Type",Type.GetType("System.String")),
					new DataColumn("MaterialName",Type.GetType("System.String")),
					new DataColumn("MaterialSpec",Type.GetType("System.String")),
                    new DataColumn("Unit",Type.GetType("System.String")),
                    new DataColumn("UnitSubCount",Type.GetType("System.String"))
				});
            ds.CaseSensitive = false;
            #endregion

            foreach (HisCallExternalServiceProject.FunctionModule.SPDModule.Model.SpdStockUseModel item in spdStockUseList)
            {
                ds.Tables[0].Rows.Add(new object[]
						{
						    item.MaterialCode,
	                        item.RFID,
                            item.Lot,
                            item.Exp,
                            item.StockQty,
                            item.DeptCode,
                            item.Type,
                            item.MaterialName,
                            item.MaterialSpec,
                            item.Unit,
                            item.UnitSubCount
                        });
            }

            fpSpread1.DataSource = ds;
            fpSpread1_Sheet1.Columns[0].Width = 66F;
            fpSpread1_Sheet1.Columns[1].Width = 216F;
            fpSpread1_Sheet1.Columns[2].Width = 57F;
            fpSpread1_Sheet1.Columns[3].Width = 0F;
            fpSpread1_Sheet1.Columns[4].Width = 88F;
            fpSpread1_Sheet1.Columns[5].Width = 59F;
            fpSpread1_Sheet1.Columns[6].Width = 57F;
            fpSpread1_Sheet1.Columns[7].Width = 50F;
            fpSpread1_Sheet1.Columns[8].Width = 50F;
            fpSpread1_Sheet1.Columns[9].Width = 50F;
            fpSpread1_Sheet1.Columns[10].Width = 50F;
            return 0;
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int Filter(string text)
        {
            text = "MaterialName like '%" + text.Trim() + "%' or " +
                 "MaterialCode like '%" + text.Trim() + "%'";
            DataView dv = new DataView(ds.Tables[0]);
            try
            {
                dv.RowFilter = text;
            }
            catch { }

            fpSpread1.DataSource = dv;
            fpSpread1_Sheet1.Columns[0].Width = 66F;
            fpSpread1_Sheet1.Columns[1].Width = 216F;
            fpSpread1_Sheet1.Columns[2].Width = 57F;
            fpSpread1_Sheet1.Columns[3].Width = 0F;
            fpSpread1_Sheet1.Columns[4].Width = 88F;
            fpSpread1_Sheet1.Columns[5].Width = 59F;
            fpSpread1_Sheet1.Columns[6].Width = 57F;
            fpSpread1_Sheet1.Columns[7].Width = 50F;
            fpSpread1_Sheet1.Columns[8].Width = 50F;
            fpSpread1_Sheet1.Columns[9].Width = 50F;
            fpSpread1_Sheet1.Columns[10].Width = 50F;

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
                //{0CD66D53-785C-4ba5-840B-885F01A31A42}
                //fpSpread1_Sheet1.AddSelection(row + 1, 0, 1, 0);
                fpSpread1_Sheet1.AddSelection(row + 1, 1, 1, 1);
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
                //{0CD66D53-785C-4ba5-840B-885F01A31A42}
                //fpSpread1_Sheet1.AddSelection(row - 1, 0, 1, 0);
                fpSpread1_Sheet1.AddSelection(row - 1, 1, 1, 1);
            }
            return 0;
        }

        /// <summary>
        /// 返回选择项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItem(ref HisCallExternalServiceProject.FunctionModule.SPDModule.Model.SpdStockUseModel spdItem)
        {
            int row = fpSpread1_Sheet1.ActiveRowIndex;

            if (row < 0 || fpSpread1_Sheet1.RowCount == 0)
            {
                spdItem = null;
                return -1;
            }
            string itemCode = fpSpread1_Sheet1.GetText(row, 0);//项目代码

            foreach (HisCallExternalServiceProject.FunctionModule.SPDModule.Model.SpdStockUseModel m in spdStockUseList)
            {
                if (m.MaterialCode == itemCode)
                {
                    spdItem = m;
                    return 0;
                }
            }

            spdItem = null;
            return -1;
        }


        private void fpSpread1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (SelectItem != null)
            {
                this.SelectItem(Keys.Enter);
            }
        }

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (SelectItem != null)
            {
                this.SelectItem(Keys.Enter);
            }
        }
    }
}
