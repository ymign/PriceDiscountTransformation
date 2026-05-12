using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Guide
{
    public partial class MZGuidePrint : Form
    {
        List<HISFC.Models.Fee.Outpatient.MZGuide> ObjList;
        Controls.ucCharge ChargeForm;
        public MZGuidePrint(Controls.ucCharge charge)
        {
            InitializeComponent();
            ChargeForm = charge;
            this.dataSet_Init(ChargeForm.GuideList);
        }

        private void neuSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Column==0)
            {
                this.neuSpread1_Sheet1.Cells[e.Row, 0].Value = !(bool)this.neuSpread1_Sheet1.Cells[e.Row, 0].Value;
                bool isChecked = (bool)this.neuSpread1_Sheet1.Cells[e.Row, 0].Value;
                string recipeNo=this.neuSpread1_Sheet1.Cells[e.Row, 2].Text.ToString();
                for (int i = 0; i < this.neuSpread1_Sheet1.Rows.Count; i++)
                {
                    if (this.neuSpread1_Sheet1.Cells[i, 2].Text.ToString() == recipeNo)
                        this.neuSpread1_Sheet1.Cells[i, 0].Value = isChecked;
                }
            }
        }

        void InitData(List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuide> List)
        {
            this.neuSpread1_Sheet1.RowCount = List.Count;
            for (int i = 0; i < List.Count; i++)
            {
                SetFpRowData(List[i],i);
            }
        }

        void InitFp()
        {
            this.neuSpread1_Sheet1.Columns[0].Width = 40;
            this.neuSpread1_Sheet1.Columns[0].Label = "选择";
            this.neuSpread1_Sheet1.Columns[1].Width = 160;
            this.neuSpread1_Sheet1.Columns[2].Width = 80;
            this.neuSpread1_Sheet1.Columns[3].Width = 80;
            this.neuSpread1_Sheet1.Columns[4].Width = 160;
            this.neuSpread1_Sheet1.Columns[5].Width = 120;
            //this.neuSpread1_Sheet1.Columns[6].Width = 80;
            //this.neuSpread1_Sheet1.Columns[7].Width = 120;
            //this.neuSpread1_Sheet1.Columns[8].Width = 60;
            //this.neuSpread1_Sheet1.Columns[9].Width = 100;
            //this.neuSpread1_Sheet1.Columns[10].Width = 0;
            //this.neuSpread1_Sheet1.Columns[11].Width = 50;
        }

        void SetFpRowData(Neusoft.HISFC.Models.Fee.Outpatient.MZGuide item,int index)
        {
            this.neuSpread1_Sheet1.Cells[index, 0].Text = item.IsChecked.ToString();
            this.neuSpread1_Sheet1.Cells[index, 0].Text = item.ID;
        }
        DataTable myDataTable;

        /// <summary>
        /// 将传入的数组中的数据保存在myDataSet中
        /// </summary>
        /// <param name="arrBed">床位信息</param>
        public void dataSet_Init(List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuide> arrUL)
        {
            DataSet dts = new DataSet();
            dts.EnforceConstraints = true;//是否遵循约束规则
            this.neuSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

            //定义表并增加到myDataSet中
            myDataTable = dts.Tables.Add();
            //清空当前myDataSet中的所有列
            myDataTable.Columns.Clear();
            myDataTable.Columns.AddRange
                (new System.Data.DataColumn[] 
					{
                        new System.Data.DataColumn("选择",Type.GetType("System.Boolean")),
                        new System.Data.DataColumn("患者卡号",Type.GetType("System.String")),
                        new System.Data.DataColumn("处方号",Type.GetType("System.String")),
                        new System.Data.DataColumn("项目代码",Type.GetType("System.String")),
						new System.Data.DataColumn("医嘱名称",Type.GetType("System.String")), //0
						new System.Data.DataColumn("执行科室",Type.GetType("System.String"))   //1
                        //new System.Data.DataColumn("数量", Type.GetType("System.String")),    //2
                        //new System.Data.DataColumn("收费日期", Type.GetType("System.String")), //3
                        //new System.Data.DataColumn("地点代码", Type.GetType("System.String")),
                        //new System.Data.DataColumn("执行地点", Type.GetType("System.String")), //4
                        //new System.Data.DataColumn("拼音码", Type.GetType("System.String")), //5
                        //new System.Data.DataColumn("五笔码", Type.GetType("System.String")), //6
                        //new System.Data.DataColumn("操作人", Type.GetType("System.String")),     //7
                        //new System.Data.DataColumn("修改时间", Type.GetType("System.String")),     //8
                        //new System.Data.DataColumn("备注", Type.GetType("System.String")),   //9
                        //new System.Data.DataColumn("是否有效", Type.GetType("System.String"))  //10
                        //11
                        //new System.Data.DataColumn("Weaveid", Type.GetType("System.String")) ,  //12
                        //new System.Data.DataColumn("住院号", Type.GetType("System.String")),
                        //new System.Data.DataColumn("特诊价格", Type.GetType("System.String")) //14
					}
                );

            DataRow dr;
            //Neusoft.HISFC.Models.Base.Bed oEBed = new Neusoft.HISFC.Models.Base.Bed();;
            Neusoft.HISFC.Models.Fee.Outpatient.MZGuide obj = new Neusoft.HISFC.Models.Fee.Outpatient.MZGuide();
            if (arrUL != null)
            {
                //循环插入基本信息
                for (int i = 0; i < arrUL.Count; i++)
                {
                    obj = arrUL[i];
                    dr = myDataTable.NewRow();
                    this.SetRow(dr, obj);
                    myDataTable.Rows.Add(dr);
                }
            }

            //将与DataView绑定
            this.neuSpread1_Sheet1.DataSource = dts.Tables[0].DefaultView;
            InitFp();
        }


        private DataRow SetRow(DataRow dr, Neusoft.HISFC.Models.Fee.Outpatient.MZGuide item)
        {
            if (item != null)
            {
                dr["选择"] = item.IsChecked;
                dr["患者卡号"] = item.ID;

                dr["处方号"] = item.Recipe_NO;

                dr["项目代码"] = item.Item_Code;
                dr["医嘱名称"] = item.Item_Name;
                dr["执行科室"] = item.Exec_Dpnm;
                //dr["拼音码"] = item.SpellCode;
                //dr["五笔码"] = item.FineCode;
                //dr["操作人"] = item.OperCode;
                //dr["修改时间"] = item.OperDate;
                //dr["备注"] = item.Mark;
                //dr["是否有效"] = item.ValidState;
            }
            return dr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<HISFC.Models.Fee.Outpatient.MZGuide> newList = new List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuide>();
            for (int i = 0; i < this.neuSpread1_Sheet1.Rows.Count; i++)
            {
                bool isCheck = (bool)this.neuSpread1_Sheet1.Cells[i, 0].Value;
                if (isCheck)
                    newList.Add(this.ChargeForm.GuideList[i]);
            }
            //this.ObjList = null;
            this.ChargeForm.GuideList = newList;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
