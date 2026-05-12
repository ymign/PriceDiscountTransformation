using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.OutpatientFee.Guide
{
    public partial class UndrugEditerExeAddr : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {

        public UndrugEditerExeAddr()
        {
            InitializeComponent();
            InitSpread();
            InitData();
            this.ntxtFilter.TextChanged += new EventHandler(ntxtFilter_TextChanged);
        }
        DataTable myDataTable;
        #region 代码
        public string strTag = "";

        TreeNode CurrentNode = new TreeNode();//当前选中的节点

		System.Data.DataSet myDataSet = new System.Data.DataSet();

        public Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast GetItemInfo()
		{
            Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item = new Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast();
			int iIndex = fpSpread1.Sheets[0].ActiveRow.Index;
            item.ItemCode = fpSpread1.Sheets[0].Cells[iIndex, 0].Text;
            item.ItemName = fpSpread1.Sheets[0].Cells[iIndex, 1].Text;
            item.LabCode = fpSpread1.Sheets[0].Cells[iIndex, 2].Text;
            item.LabName = fpSpread1.Sheets[0].Cells[iIndex, 3].Text;
            item.Addr_Code = fpSpread1.Sheets[0].Cells[iIndex, 4].Text;
            item.Addresses = fpSpread1.Sheets[0].Cells[iIndex, 5].Text;
            item.SpellCode = fpSpread1.Sheets[0].Cells[iIndex, 6].Text;
            item.FineCode = fpSpread1.Sheets[0].Cells[iIndex, 7].Text;
            item.OperCode = fpSpread1.Sheets[0].Cells[iIndex, 8].Text;
            item.OperDate = fpSpread1.Sheets[0].Cells[iIndex, 9].Text;
            item.Mark = fpSpread1.Sheets[0].Cells[iIndex, 10].Text;
            item.ValidState = fpSpread1.Sheets[0].Cells[iIndex, 11].Text;
            item.Urgency = fpSpread1.Sheets[0].Cells[iIndex, 12].Text;
			//oBedInfo.NurseStation.ID = fpSpread1.Sheets[0].Cells[iIndex,0].Text;//护士站编号
		    
			return item;
        }

		private void EventResultChanged(ArrayList s)
		{
		}

		/// <summary>
		/// 将传入的数组中的数据保存在myDataSet中
		/// </summary>
		/// <param name="arrBed">床位信息</param>
		public void dataSet_Init(List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast> arrUL)
		{
			DataSet dts = new DataSet();
			dts.EnforceConstraints = true;//是否遵循约束规则
			this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
 
			//定义表并增加到myDataSet中
             myDataTable = dts.Tables.Add();			
			//清空当前myDataSet中的所有列
			myDataTable.Columns.Clear();			
			myDataTable.Columns.AddRange
				(new System.Data.DataColumn[] 
					{
						new System.Data.DataColumn("项目编码",Type.GetType("System.String")), //0
						new System.Data.DataColumn("项目名称",Type.GetType("System.String")),   //1
						new System.Data.DataColumn("科室代码", Type.GetType("System.String")),    //2
						new System.Data.DataColumn("科室名称", Type.GetType("System.String")), //3
                        new System.Data.DataColumn("地点代码", Type.GetType("System.String")),
						new System.Data.DataColumn("执行地点", Type.GetType("System.String")), //4
						new System.Data.DataColumn("拼音码", Type.GetType("System.String")), //5
						new System.Data.DataColumn("五笔码", Type.GetType("System.String")), //6
						new System.Data.DataColumn("操作人", Type.GetType("System.String")),     //7
                        new System.Data.DataColumn("修改时间", Type.GetType("System.String")),     //8
                        new System.Data.DataColumn("备注", Type.GetType("System.String")),   //9
                        new System.Data.DataColumn("是否有效", Type.GetType("System.String")),  //10
                        //11
                        //new System.Data.DataColumn("Weaveid", Type.GetType("System.String")) ,  //12
                        //new System.Data.DataColumn("住院号", Type.GetType("System.String")),
                        //new System.Data.DataColumn("特诊价格", Type.GetType("System.String")) //14
                        new System.Data.DataColumn("能否加急", Type.GetType("System.String"))
					}
				);
	
			DataRow dr;
			//Neusoft.HISFC.Models.Base.Bed oEBed = new Neusoft.HISFC.Models.Base.Bed();;
            Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast obj = new Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast();
			if(arrUL!=null)
			{
				//循环插入基本信息
                for (int i = 0; i < arrUL.Count; i++)
				{
                    obj = arrUL[i];
					dr = myDataTable.NewRow();			
					this.SetRow( dr, obj );
					myDataTable.Rows.Add( dr );	
				}
			}

			//将与DataView绑定
			this.fpSpread1_Sheet1.DataSource = dts.Tables[0].DefaultView;
			InitSpread();
		}
        
		private Neusoft.HISFC.BizLogic.Manager.Bed oCBed = new Neusoft.HISFC.BizLogic.Manager.Bed();
		public int DelInfo()
		{
            string msg=string.Empty;
            Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item = this.GetItemInfo();
            return new HISFC.BizLogic.Fee.MZGuide().DelULContrast(item, ref msg);
          
		}

		
		private string Err;
		private void ReBind(string strID,string strTag,string strNurseID)
		{
			ArrayList arr = new ArrayList();
			if(strTag=="0")
			{
				arr = oCBed.GetBedList(strID);
			}
			if(strTag=="1")
			{
				arr = oCBed.GetBedListByRoom(strID,strNurseID);
			}
		
			//this.dataSet_Init(arr);
			InitSpread();
		}


        private DataRow SetRow(DataRow dr, Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item)
        {
            if (item != null)
            {
                dr["项目编码"] = item.ItemCode;
                dr["项目名称"] = item.ItemName;

                dr["科室代码"] = item.LabCode;

                dr["科室名称"] = item.LabName;
                dr["地点代码"] = item.Addr_Code;
                dr["执行地点"] = item.Addresses;
                dr["拼音码"] = item.SpellCode;
                dr["五笔码"] = item.FineCode;
                dr["操作人"] = item.OperCode;
                dr["修改时间"] = item.OperDate;
                dr["备注"] = item.Mark;
                dr["是否有效"] = item.ValidState;
                dr["能否加急"] = item.Urgency;
            }
            return dr;
        }

		
		private void InitSpread()
		{
			this.fpSpread1_Sheet1.Columns[0].Width = 100;
			this.fpSpread1_Sheet1.Columns[1].Width = 160;
			this.fpSpread1_Sheet1.Columns[2].Width = 80;
			this.fpSpread1_Sheet1.Columns[3].Width = 80;
			this.fpSpread1_Sheet1.Columns[4].Width = 100;
			this.fpSpread1_Sheet1.Columns[5].Width = 100;
			this.fpSpread1_Sheet1.Columns[6].Width = 80;
			this.fpSpread1_Sheet1.Columns[7].Width = 120;
			this.fpSpread1_Sheet1.Columns[8].Width = 60;		
			this.fpSpread1_Sheet1.Columns[9].Width =100;
			this.fpSpread1_Sheet1.Columns[10].Width = 0;
			this.fpSpread1_Sheet1.Columns[11].Width = 50;
            //this.fpSpread1_Sheet1.Columns[12].Width = 0;
            //this.fpSpread1_Sheet1.Columns[14].Width = 80;
            if (fpSpread1_Sheet1.Rows.Count > 0)
            {
                fpSpread1.ContextMenuStrip = contextMenuStrip1;
            }
            else
            {
                fpSpread1.ContextMenuStrip = null;

            }
            for (int columnIndex = 0; columnIndex < this.fpSpread1_Sheet1.ColumnCount; columnIndex++)
            {
                fpSpread1_Sheet1.Columns[columnIndex].AllowAutoSort = true;
            }
		}

        public void SetActiveSell(string BedNo)
		{
			for(int i=0;i<fpSpread1_Sheet1.Rows.Count;i++)
			{
				if(fpSpread1_Sheet1.Cells[i,2].Text==BedNo)
				{
					fpSpread1_Sheet1.SetActiveCell(i,2);
					return ;
				}
			}
		}

		
        #endregion

        Neusoft.FrameWork.WinForms.Forms.ToolBarService toolbarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        /// <summary>
        /// 新增加的代码
        /// </summary>
        /// <param name="isEnabled"></param>
        protected override void OnPrintPreviewButtonChanged(bool isEnabled)
        {
            isEnabled = false;
            base.OnPrintPreviewButtonChanged(isEnabled);
        }

        protected override int OnPrint(object sender, object neuObject)
        {
            try
            {
                Neusoft.FrameWork.WinForms.Classes.Print p = new Neusoft.FrameWork.WinForms.Classes.Print();
                p.ControlBorder = Neusoft.FrameWork.WinForms.Classes.enuControlBorder.Border;
                p.PrintPreview(this);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            return base.OnPrint(sender, neuObject);
        }

        public override int Export(object sender, object neuObject)
        {
            if (this.fpSpread1_Sheet1.Rows.Count == 0)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("没有要保存的数据!"), "消息");
                return -1;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "(*.xls)|*.xls";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.fpSpread1.SaveExcel(dlg.FileName);
                this.fpSpread1.SaveExcel(dlg.FileName, FarPoint.Excel.ExcelSaveFlags.SaveBothCustomRowAndColumnHeaders);
                return 1;
            }
            else
                return 0;

           // return base.Export(sender, neuObject);
        }

        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
          //  this.dataSet_Init(new ArrayList());
            toolbarService.AddToolButton("添加", "添加对照", 0, true, false, null);
           // toolbarService.AddToolButton("批量添加", "批量添加床位", 0, true, false, null);
            toolbarService.AddToolButton("编辑", "编辑对照", 0, true, false, null);
            toolbarService.AddToolButton("删除", "删除对照", 0, true, false, null);
            return toolbarService;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void  ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "添加":
                    this.AddInfo();
                    break;
                //case "批量添加":
                //    this.BatchAddInfo();
                //    break;
                case "复制":
                    this.ModifiedInfo();
                    break;
                case "删除":
                    this.Delete();
                    break;
            }
 	        base.ToolStrip_ItemClicked(sender, e);
        }
        private void BatchAddInfo()
        {

        }
        private void AddInfo()
		{
            UndrugItemEditer f = new UndrugItemEditer(null);
            f.StartPosition = FormStartPosition.CenterParent;
            //f.SetBedInfo( this.GetBedInfo());

            if (f.ShowDialog() == DialogResult.OK)
            {
                //应该写刷新代码
                this.Refresh();
            }     
		}
        private void ModifiedInfo()
        {

            UndrugItemEditer f = new UndrugItemEditer(this.GetItemInfo());
            //f.SetBedInfo( this.GetBedInfo());
            f.StartPosition = FormStartPosition.CenterParent;
            if (f.ShowDialog() == DialogResult.OK)
            {
                //应该写刷新代码
                this.Refresh();
            }

        }

        /// <summary>
        /// 修改添加数据后刷新数据
        /// </summary>
        private void Refresh()
        {
            InitData();
            //this.dataSet_Init(arr);
        }


        //private void CopyInfo()
        //{
        //    Forms.frmCopyBed f = new Manager.Forms.frmCopyBed(true);
        //    f.SetBedInfo(this.GetBedInfo());

        //    if (f.ShowDialog() == DialogResult.OK)
        //    {
        //        //应该写刷新代码
        //        this.Refresh();
        //    }

        //}

        private void Delete()
        {
            DialogResult result;
            if (this.fpSpread1.Sheets[0].ActiveRowIndex < 0) return;
            string item_name = fpSpread1.Sheets[0].Cells[this.fpSpread1.Sheets[0].ActiveRowIndex, 1].Text;
            result = MessageBox.Show(string.Format("确认要删除{0}对照？", item_name), "确认", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (this.DelInfo() != -1)
                    {
                        MessageBox.Show("删除成功！");
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
            //if (result == DialogResult.No)
            //{

            //}
        }
        string NurseID;
        string strID = string.Empty;
        protected override int  OnSetValue(object neuObject, TreeNode e)
        {
            //string strID = "";
            ArrayList arr = new ArrayList();
            Neusoft.HISFC.BizLogic.Manager.Bed oCBed = new Neusoft.HISFC.BizLogic.Manager.Bed();
            if (e != null)
            {
                CurrentNode = e;
                if (e.Parent != null && e.Parent.Parent != null)//病房号
                {
                    string strNurse = e.Parent.Tag.ToString();
                    strID = e.Text.Trim();
                    arr = oCBed.GetBedListByRoom(strID, strNurse);
                    this.strTag = "1";
                   // this.dataSet_Init(arr);
                    this.NurseID = strNurse; //护士站    
                }
                else if (e.Parent != null)//护士站号
                {
                    if (e.Tag != null)
                    {
                        strID = e.Tag.ToString();
                        arr = oCBed.GetBedList(strID);
                        this.strTag = "0";
                      //  this.dataSet_Init(arr);
                        strID = "";
                    }
                }
                else
                {
                    strID = "ALL";
                    arr = oCBed.GetBedList(strID);
                   // this.dataSet_Init(arr);

                }
            }
            
            
            return base.OnSetValue(neuObject, e);
        }  
        private void fpSpread1_CellDoubleClick_1(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            this.ModifiedInfo(); 
        }
        void ntxtFilter_TextChanged(object sender, EventArgs e)
        {
            this.Filter();
        }
        #region 过滤
        private void Filter()
        {
            this.myDataTable.DefaultView.RowFilter = "";
            string filter = "";// = Function.GetFilterStr(this.myDataTable.DefaultView, this.ntxtFilter.Text.Trim());
            //filter = "(" + filter + ")";
            //增加系统类别，费用类别，有效性，组套
            if (this.myDataTable.Columns.Contains("拼音码"))
            {
                filter += string.Format("  拼音码 like '%{0}%'", this.ntxtFilter.Text.Trim());
            }

          

            //if (this.dateTimePicker1.Value.ToString() != "" && this.dateTimePicker2.Value.ToString() != "")
            //{
            //    //dateTimePicker2.Value.Date = dateTimePicker2.Value.Date.AddDays(1);
            //    if (ncbMutiQuery.Checked)
            //    {
            //        filter += string.Format(" and 新增时间 >='{0}' and 新增时间 <='{1}'", dateTimePicker1.Value.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), dateTimePicker2.Value.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"));

            //    }
            //}


            this.myDataTable.DefaultView.RowFilter = filter;
            //this.SetFormat();
            //this.InitFarPoint();
        }
        #endregion

        void InitData()
        {
            try
            {
                List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast> list = new List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast>();
                list = new Neusoft.HISFC.BizLogic.Fee.MZGuide().QueryGuideNotULContrast();
                this.dataSet_Init(list);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
