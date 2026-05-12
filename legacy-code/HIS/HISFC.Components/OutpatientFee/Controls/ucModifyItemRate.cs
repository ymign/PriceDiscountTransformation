using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using FarPoint.Win.Spread;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
	/// <summary>
	/// ucModifyItemRate 的摘要说明。
	/// </summary>
	public class ucModifyItemRate : System.Windows.Forms.UserControl
    {
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblRate;
        private Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox tbCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox tbPayRate;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuContextMenuStrip contextMenu1 = null;
        private Neusoft.FrameWork.Public.ObjectHelper objHelp = new Neusoft.FrameWork.Public.ObjectHelper();
  
        /// <summary>
        /// 是否显示右键菜单
        /// </summary>
        protected bool bIsShowPopMenu = true;
        private Button btnOk;
        private Button tbCancel;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Panel panel1;
        private IContainer components;

        /// <summary>
        /// 是否显示右键菜单
        /// </summary>
        public bool IsShowPopMenu
        {
            get
            {
                return this.bIsShowPopMenu;
            }
            set
            {
                this.bIsShowPopMenu = value;
            }
        }

		public ucModifyItemRate()
		{
			// 该调用是 Windows.Forms 窗体设计器所必需的。
			InitializeComponent();

			// TODO: 在 InitializeComponent 调用后添加任何初始化
            this.tbPayRate.Focus();
            this.tbPayRate.KeyDown += new KeyEventHandler( tbPayRate_KeyDown );
            this.tbCost.KeyDown += new KeyEventHandler( tbCost_KeyDown );

            this.fpSpread1.MouseDown += new MouseEventHandler(fpSpread1_MouseDown);
            this.fpSpread1.MouseUp += new MouseEventHandler(fpSpread1_MouseUp);
            this.contextMenu1 = new Neusoft.FrameWork.WinForms.Controls.NeuContextMenuStrip();
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbCost_KeyDown( object sender, KeyEventArgs e )
        {
            if (e.KeyData == Keys.Enter)
            {
                if (this.fpSpread1_Sheet1.RowCount > 0)
                {
                    for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                    {
                        this.fpSpread1_Sheet1.SetValue( i, 3, this.tbCost.Text );
                    }
                }
                if (this.fpSpread1_Sheet1.RowCount > 0)
                {
                    this.fpSpread1.Focus();
                    int row = this.fpSpread1_Sheet1.RowCount - 1;
                    this.fpSpread1_Sheet1.SetActiveCell( row, 1, false );
                }
            }
        }

        void tbPayRate_KeyDown( object sender, KeyEventArgs e )
        {
            if (e.KeyData == Keys.Enter)
            {
                if (this.fpSpread1_Sheet1.RowCount > 0)
                {
                    for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                    {
                        if(Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[i,5].Value) == true)
                        this.fpSpread1_Sheet1.SetValue( i, 1, this.tbPayRate.Text );
                    }
                }
                this.tbCost.Focus();
                this.tbCost.SelectAll();
            }
        }

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region 组件设计器生成的代码
		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器 
		/// 修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType2 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.lblRate = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbCost = new Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox();
            this.lblCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbPayRate = new Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox();
            this.fpSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.btnOk = new System.Windows.Forms.Button();
            this.tbCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.neuPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.lblRate);
            this.neuPanel1.Controls.Add(this.tbCost);
            this.neuPanel1.Controls.Add(this.lblCost);
            this.neuPanel1.Controls.Add(this.tbPayRate);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuPanel1.Location = new System.Drawing.Point(3, 253);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(638, 46);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 0;
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblRate.Location = new System.Drawing.Point(9, 13);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(172, 16);
            this.lblRate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblRate.TabIndex = 18;
            this.lblRate.Text = "初始化自付比例(F1):";
            // 
            // tbCost
            // 
            this.tbCost.AllowNegative = false;
            this.tbCost.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCost.ForeColor = System.Drawing.Color.Blue;
            this.tbCost.IsAutoRemoveDecimalZero = false;
            this.tbCost.IsEnter2Tab = false;
            this.tbCost.Location = new System.Drawing.Point(469, 13);
            this.tbCost.Name = "tbCost";
            this.tbCost.NumericPrecision = 5;
            this.tbCost.NumericScaleOnFocus = 2;
            this.tbCost.NumericScaleOnLostFocus = 2;
            this.tbCost.NumericValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.tbCost.SetRange = new System.Drawing.Size(-1, -1);
            this.tbCost.Size = new System.Drawing.Size(140, 21);
            this.tbCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCost.TabIndex = 20;
            this.tbCost.Text = "0.00";
            this.tbCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbCost.UseGroupSeperator = true;
            this.tbCost.ZeroIsValid = true;
            // 
            // lblCost
            // 
            this.lblCost.AutoSize = true;
            this.lblCost.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCost.Location = new System.Drawing.Point(329, 14);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(136, 16);
            this.lblCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblCost.TabIndex = 19;
            this.lblCost.Text = "初始化自费金额:";
            // 
            // tbPayRate
            // 
            this.tbPayRate.AllowNegative = false;
            this.tbPayRate.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPayRate.ForeColor = System.Drawing.Color.Blue;
            this.tbPayRate.IsAutoRemoveDecimalZero = false;
            this.tbPayRate.IsEnter2Tab = false;
            this.tbPayRate.Location = new System.Drawing.Point(184, 12);
            this.tbPayRate.Name = "tbPayRate";
            this.tbPayRate.NumericPrecision = 5;
            this.tbPayRate.NumericScaleOnFocus = 2;
            this.tbPayRate.NumericScaleOnLostFocus = 2;
            this.tbPayRate.NumericValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.tbPayRate.SetRange = new System.Drawing.Size(-1, -1);
            this.tbPayRate.Size = new System.Drawing.Size(140, 21);
            this.tbPayRate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbPayRate.TabIndex = 0;
            this.tbPayRate.Text = "0.00";
            this.tbPayRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbPayRate.UseGroupSeperator = true;
            this.tbPayRate.ZeroIsValid = true;
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1, Row 0, Column 0, ";
            this.fpSpread1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Top;
            this.fpSpread1.EditModeReplace = true;
            this.fpSpread1.FileName = "";
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.IsAutoSaveGridStatus = false;
            this.fpSpread1.IsCanCustomConfigColumn = false;
            this.fpSpread1.Location = new System.Drawing.Point(3, 3);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(638, 250);
            this.fpSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpSpread1.TabIndex = 10;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread1.TextTipAppearance = tipAppearance1;
            this.fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
            this.fpSpread1.Leave += new System.EventHandler(this.fpSpread1_Leave);
            this.fpSpread1.EditChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_EditChange);
            this.fpSpread1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread1_KeyDown);
            this.fpSpread1.LeaveCell += new FarPoint.Win.Spread.LeaveCellEventHandler(this.fpSpread1_LeaveCell);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 6;
            this.fpSpread1_Sheet1.RowCount = 10;
            this.fpSpread1_Sheet1.Cells.Get(4, 5).Value = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "收费名称(规格)";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "自付比例";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "特批归类";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "自费金额";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "付款方式";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "特殊";
            this.fpSpread1_Sheet1.Columns.Get(0).Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "收费名称(规格)";
            this.fpSpread1_Sheet1.Columns.Get(0).Locked = true;
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 246F;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = numberCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "自付比例";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 84F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = textCellType1;
            this.fpSpread1_Sheet1.Columns.Get(2).Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "特批归类";
            this.fpSpread1_Sheet1.Columns.Get(2).Locked = true;
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 79F;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = numberCellType2;
            this.fpSpread1_Sheet1.Columns.Get(3).Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "自费金额";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(4).Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "付款方式";
            this.fpSpread1_Sheet1.Columns.Get(4).Locked = true;
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 82F;
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = checkBoxCellType1;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "特殊";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 34F;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.Rows.Default.Height = 22F;
            this.fpSpread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // btnOk
            // 
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOk.Location = new System.Drawing.Point(472, 11);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "确定(&O)";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tbCancel
            // 
            this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.tbCancel.Location = new System.Drawing.Point(553, 11);
            this.tbCancel.Name = "tbCancel";
            this.tbCancel.Size = new System.Drawing.Size(75, 23);
            this.tbCancel.TabIndex = 1;
            this.tbCancel.Text = "取消(&C)";
            this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(18, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "F4";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(50, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "确认修改";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(127, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "ESC";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(166, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "取消修改";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbCancel);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 301);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 48);
            this.panel1.TabIndex = 1;
            // 
            // ucModifyItemRate
            // 
            this.Controls.Add(this.neuPanel1);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.panel1);
            this.Name = "ucModifyItemRate";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(644, 352);
            this.Load += new System.EventHandler(this.ucModifyItemRate_Load);
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region 变量

		private ArrayList feeDetails = new ArrayList();//费用信息
		private bool isConfirm = false;//是否已经修改比例
		private ArrayList relations = new ArrayList();
        private Neusoft.HISFC.Models.Registration.Register register = new Neusoft.HISFC.Models.Registration.Register();
        Neusoft.FrameWork.Public.ObjectHelper apprItemHelper = new Neusoft.FrameWork.Public.ObjectHelper();

		#endregion

		#region 属性
		/// <summary>
		/// feeDetails
		/// </summary>
		public ArrayList FeeDetails
		{
			get
			{
				return feeDetails;
			}
			set
			{
				feeDetails = value;
			}
		}

        public Neusoft.HISFC.Models.Registration.Register Register
        {
            get
            {
                return register;
            }
            set
            {
                register = value;
            }
        }
		/// <summary>
		/// 是否已经修改比例
		/// </summary>
		public bool IsConfirm
		{
			get
			{
				return isConfirm;
			}
		}

		#endregion

		#region 函数
		/// <summary>
		/// 初始化费用列表
		/// </summary>
		/// <param name="feeDetails">费用明细集合</param>
		public void InitFeeDetails(ArrayList feeDetails)
		{
			this.feeDetails = feeDetails;
			this.InitFeeDetails();
		}
		/// <summary>
		/// 初始化费用列表
		/// </summary>
		public void InitFeeDetails()
		{
            Neusoft.HISFC.BizLogic.Manager.Constant myCons = new Neusoft.HISFC.BizLogic.Manager.Constant();

            ArrayList alMenu = myCons.GetAllList("GFSPTYPES");
            if (alMenu == null || alMenu.Count == 0)
            {
                objHelp.ArrayObject = new ArrayList();
                //MessageBox.Show("请维护公费特批归类【TYPE为GFSPTYPES】!");
            }
            else
            {
                objHelp.ArrayObject = alMenu;
            }
			this.fpSpread1_Sheet1.RowCount = this.feeDetails.Count;
			Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeDetail = new Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList();
			for(int i = 0; i < this.feeDetails.Count; i++)
			{
				feeDetail = feeDetails[i] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;

                if (feeDetail.OrgItemRate == 0 && feeDetail.ItemRateFlag != "3")
                {
                    feeDetail.OrgItemRate = 1;
                }
                this.fpSpread1_Sheet1.Rows[i].Tag = feeDetail;
                this.fpSpread1_Sheet1.Cells[i, 0].Text = feeDetail.Item.Name + "[" + feeDetail.Item.Specs + "]";
                this.fpSpread1_Sheet1.Cells[i, 1].Text = (feeDetail.NewItemRate * 100).ToString();

                //这么修改
                if (feeDetail.FT.User03 != string.Empty)
                {
                    this.fpSpread1_Sheet1.Cells[i, 3].Text = feeDetail.FT.User03;
                }
                else
                {
                    //this.fpSpread1_Sheet1.Cells[i, 3].Text = feeDetail.FT.TotCost.ToString();
                    this.fpSpread1_Sheet1.Cells[i, 3].Text = "0";
                }
				string tmpFlag = null;
				switch(feeDetail.ItemRateFlag)
				{
					case "1":
                        tmpFlag = "自费";
						break;
					case "2":
						tmpFlag = "记帐";
						break;
					case "3":
						tmpFlag = "特殊";
						break;
				}

                string tmpGF = objHelp.GetName(feeDetail.FT.FTRate.User03);
               
				this.fpSpread1_Sheet1.Cells[i,4].Text = tmpFlag;
                this.fpSpread1_Sheet1.Cells[i, 2].Text = tmpGF;


				if(feeDetail.ItemRateFlag == "3")
				{
					this.fpSpread1_Sheet1.Cells[i,5].Value = true;
				}
				else
				{
					this.fpSpread1_Sheet1.Cells[i,5].Value = false;
				}

                if (feeDetail.NewItemRate == 0)
                {
                    this.fpSpread1_Sheet1.Cells[i, 4].Text = "自费";
                    this.fpSpread1_Sheet1.Cells[i, 1].Text = "100";
                }
			}

		}
		/// <summary>
		/// 初始化显示列表
		/// </summary>
		private void InitFp()
		{
			InputMap im;
			im = this.fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused);
			im.Put(new Keystroke(Keys.Enter,Keys.None),FarPoint.Win.Spread.SpreadActions.None);
		
			im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused );
			im.Put(new Keystroke(Keys.Down,Keys.None),FarPoint.Win.Spread.SpreadActions.None);

			im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused );
			im.Put(new Keystroke(Keys.Up,Keys.None),FarPoint.Win.Spread.SpreadActions.None);

			im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused );
			im.Put(new Keystroke(Keys.Escape,Keys.None),FarPoint.Win.Spread.SpreadActions.None);

			im = fpSpread1.GetInputMap(InputMapMode.WhenAncestorOfFocused );
			im.Put(new Keystroke(Keys.F4,Keys.None),FarPoint.Win.Spread.SpreadActions.None);
		}
		/// <summary>
		/// 获得修改后的项目信息
		/// </summary>
        private int GetItemList()
        {
            this.feeDetails = new ArrayList();
            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f = null;
            this.fpSpread1.StopCellEditing();

            #region
            /*
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i ++)
			{
				
				decimal newItemRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i,1].Text);
				if(newItemRate > 100 || newItemRate < 0)
				{
					MessageBox.Show("比例输入不合法!");
                    this.fpSpread1.Focus();
					this.fpSpread1_Sheet1.SetActiveCell(i, 1, false);
					return -1;
				}
				if(newItemRate == 100)
				{
					this.fpSpread1_Sheet1.Cells[i,4].Text = "自费";
					((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "1";
				}
                //else if(newItemRate != ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).OrgItemRate * 100)
                //{
                //    this.fpSpread1_Sheet1.Cells[i,4].Text = "特殊";
                //    ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "3";
                //}
                else if (newItemRate != ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).OrgItemRate * 100 ||
                    Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[i, 5].Value) == true)
                {
                    this.fpSpread1_Sheet1.Cells[i, 4].Text = "特殊";
                    ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "3";
                    this.fpSpread1_Sheet1.Cells[i, 5].Value = true;
                }
				else
				{
					this.fpSpread1_Sheet1.Cells[i,4].Text = "记帐";
					((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "2";
				}
				((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).NewItemRate = Neusoft.FrameWork.Public.String.FormatNumber(newItemRate / 100, 2);
				
                //if(((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag != "3" && 
                //  Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[i,5].Value) == true)
                if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[i, 5].Value) == true)
				{
					DialogResult _Reu;

					_Reu = MessageBox.Show("项目:"+((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).Name+"确定为特批项目吗?","提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);

					if(_Reu == DialogResult.Cancel)
					{
						this.fpSpread1_Sheet1.SetActiveCell(i, 5, false);
						return -1;
					}
					else
					{
						this.fpSpread1_Sheet1.Cells[i,4].Text = "特殊";
						((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "3";
					}
				}

				decimal ownPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i,3].Text);

                //这么修改 2008-4-20
                //decimal truePrice = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag ).Item.Price;
                decimal truePrice = ( (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag ).FT.TotCost;

				if(truePrice < ownPrice)
				{
					//MessageBox.Show("输入自费单价不能大于费用单价!");
                    MessageBox.Show( "输入自费金额不能大于费用总价!" );
					this.fpSpread1_Sheet1.SetActiveCell(i, 3, false);
					return -1;
				}
				//这么修改 2008-4-20
                //( (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag ).SpecialPrice = ownPrice;	
                ( (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag ).SpecialPrice = 0m;																			 
                ( (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag ).FT.User03 = ownPrice.ToString();	

				f = this.fpSpread1_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
				feeDetails.Add(f);

            }*/
            #endregion

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {

                decimal newItemRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, 1].Text);
                if (newItemRate > 100 || newItemRate < 0)
                {
                    MessageBox.Show("比例输入不合法!");
                    this.fpSpread1.Focus();
                    this.fpSpread1_Sheet1.SetActiveCell(i, 1, false);
                    return -1;
                }
                decimal ownPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, 3].Text);
                decimal truePrice = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).FT.TotCost;

                if (truePrice < ownPrice)
                {
                    MessageBox.Show("输入自费金额不能大于费用总价!");
                    this.fpSpread1_Sheet1.SetActiveCell(i, 3, false);
                    return -1;
                }

                if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[i, 5].Value) == true)
                {
                    DialogResult _Reu;

                    _Reu = MessageBox.Show("项目:" + ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).Name + "确定为特批项目吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                    if (_Reu == DialogResult.Cancel)
                    {
                        //this.fpSpread1_Sheet1.SetActiveCell(i, 5, false);
                        this.fpSpread1_Sheet1.Cells[i, 5].Value = false;
                        this.fpSpread1_Sheet1.Cells[i, 2].Text = "";
                        this.fpSpread1_Sheet1.Cells[i, 2].Tag = "";
                        this.fpSpread1_Sheet1.Cells[i, 3].Text = "0";

                        this.fpSpread1_Sheet1.Cells[i, 1].Text = (((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).OrgItemRate * 100).ToString();
                        if (((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).OrgItemRate == 1)
                        {
                            this.fpSpread1_Sheet1.Cells[i, 4].Text = "自费";
                            this.fpSpread1_Sheet1.Cells[i, 1].Text = "100";
                        }
                        else
                        {
                            this.fpSpread1_Sheet1.Cells[i, 4].Text = "记账";
                        }
                        return -1;
                    }
                    else
                    {
                        this.fpSpread1_Sheet1.Cells[i, 4].Text = "特殊";
                        //((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "3";
                    }
                }
            }
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                decimal newItemRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, 1].Text);
                decimal ownPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, 3].Text);
                decimal truePrice = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).FT.TotCost;

                if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[i, 5].Value) == true)
                {

                    ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "3";

                }
                else
                {
                    if (((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).OrgItemRate == 1)
                    {
                        ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "1";
                    }
                    else
                    {
                        ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).ItemRateFlag = "2";
                    }
                    ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).FT.FTRate.User03 = "";
                }
                ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).NewItemRate = Neusoft.FrameWork.Public.String.FormatNumber(newItemRate / 100, 2);

                if (this.fpSpread1_Sheet1.Cells[i, 2].Tag != null)
                {
                    ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1.ActiveSheet.Rows[i].Tag).FT.FTRate.User03 = this.fpSpread1_Sheet1.Cells[i, 2].Tag.ToString();
                }
                //这么修改 2008-4-20
                ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).SpecialPrice = 0m;
                ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).FT.User03 = ownPrice.ToString();

                f = this.fpSpread1_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                feeDetails.Add(f);

            }

            return 0;
        }

		#endregion

		#region 事件

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			isConfirm = false;
			this.FindForm().Close();
		}
		private void btnOk_Click(object sender, System.EventArgs e)
		{
			isConfirm = true;
			if(GetItemList() == -1)
			{
				this.isConfirm = false;
				return;
			}
            //if(this.GetRelation() == -1)
            //{
            //    this.isConfirm = false;
            //    return;
            //}
			this.FindForm().Close();
			
		}
		private void ucModifyItemRate_Load(object sender, System.EventArgs e)
		{
			try
			{
				this.InitFp();
                //this.fpSpread1.Focus();
                //if(this.fpSpread1_Sheet1.RowCount > 0)
                //{
                //    int row = this.fpSpread1_Sheet1.RowCount -1;
                //    this.fpSpread1_Sheet1.SetActiveCell(row, 1, false);
                //}

                Neusoft.HISFC.BizLogic.Manager.Constant myCons = new Neusoft.HISFC.BizLogic.Manager.Constant();
				ArrayList alApprItem = myCons.GetAllList("ApprItem");
				if(alApprItem != null)
				{
					this.apprItemHelper.ArrayObject = alApprItem;
				}
                //光标跳转到初始化自付金额
                this.neuPanel1.Focus();
                this.tbPayRate.Focus();
               // SendKeys.Send( "{F1}" );
                this.tbPayRate.SelectAll();
			}
			catch{}
		}
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(this.fpSpread1.ContainsFocus)
			{
				if(keyData == Keys.Enter)
				{
					int row = this.fpSpread1_Sheet1.ActiveRowIndex;
					int col = this.fpSpread1_Sheet1.ActiveColumnIndex;
					if(col == 1)
					{
						this.fpSpread1.StopCellEditing();
						decimal newItemRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[row,1].Text);
						if(newItemRate > 100 || newItemRate < 0)
						{
							MessageBox.Show("比例输入不合法!");
                            this.fpSpread1.Focus();
							this.fpSpread1_Sheet1.SetActiveCell(row, 1, false);
							return false;
						}
                        this.fpSpread1_Sheet1.SetActiveCell(row,3,false);
					}
					if(col == 3)
					{
						col = 1;
						if(row == this.fpSpread1_Sheet1.RowCount -1)
						{
							row = 0;
						}
						else
						{
							row = row + 1;
						}
                        this.fpSpread1_Sheet1.SetActiveCell(row,col,false);
					}
                    //else
                    //{
                    //    col = col + 1;
                    //}
                    //this.fpSpread1_Sheet1.SetActiveCell(row, col, false);
				}
				if(keyData == Keys.Up)
				{
					int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;
					if(currRow > 0)
					{
						this.fpSpread1_Sheet1.ActiveRowIndex = currRow -1;
						this.fpSpread1_Sheet1.SetActiveCell(currRow -1, this.fpSpread1_Sheet1.ActiveColumnIndex);
					}
				}
				if(keyData == Keys.Down)
				{
					int currRow = this.fpSpread1_Sheet1.ActiveRowIndex;
					
					this.fpSpread1_Sheet1.ActiveRowIndex = currRow + 1;
					this.fpSpread1_Sheet1.SetActiveCell(currRow + 1, this.fpSpread1_Sheet1.ActiveColumnIndex);

				}
			}
			if(keyData == Keys.F4)
			{
				this.isConfirm = true;
				if(this.GetItemList() == -1)
				{
					this.isConfirm = false;
					return false;
				}
				
                //if(this.GetRelation() == -1)
                //{
                //    this.isConfirm = false;
                //    return false;
                //}
				this.FindForm().Close();
			}
            if (keyData == Keys.F1)
            {
                this.tbPayRate.Focus();
                this.tbPayRate.SelectAll();
            }
            //if(keyData == Keys.F5)
            //{
            //    foreach(Control c in this.gb1.Controls)
            //    {
            //        if(c.Name.Substring(0,1) == "t" && c.Visible == true)
            //        {
            //            c.Focus();
            //            ((System.Windows.Forms.TextBox)c).SelectAll();
            //            break;
            //        }
            //    }
            //}
			if(keyData == Keys.Escape)
			{
				this.isConfirm = false;
				
				this.FindForm().Close();
				
			}

			
			return base.ProcessDialogKey (keyData);
		}
		private void fpSpread1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			
		}
		private void fpSpread1_Leave(object sender, System.EventArgs e)
		{
			this.fpSpread1.StopCellEditing();
		}
		private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
		{
		
		}
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            int row = this.fpSpread1_Sheet1.ActiveRowIndex;
            int col = this.fpSpread1_Sheet1.ActiveColumnIndex;
            if (col == 1)
            {

                decimal newItemRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[row, 1].Text);
                if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[row, 5].Value) == true)
                {
                    this.fpSpread1_Sheet1.Cells[row, 4].Text = "特殊";
                }
                else
                {
                    if (newItemRate != ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[row].Tag).OrgItemRate * 100)
                    {
                        if (newItemRate == 100)
                        {
                            this.fpSpread1_Sheet1.Cells[row, 4].Text = "自费(特殊)";
                        }
                        else
                        {
                            this.fpSpread1_Sheet1.Cells[row, 4].Text = "特殊";
                        }
                        this.fpSpread1_Sheet1.Cells[row, 5].Value = true;
                    }
                    else
                    {
                        this.fpSpread1_Sheet1.Cells[row, 4].Text = "记帐";
                    }
                }
            }
        }
		private void gb1_Enter(object sender, System.EventArgs e)
		{
		
		}
        /// <summary>
        /// 清除右键菜单内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                this.contextMenu1.Items.Clear();
            }
            catch { }
        }
        /// <summary>
        /// 为右键添加菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.bIsShowPopMenu && e.Button == MouseButtons.Right)
            {
                try
                {
                    this.contextMenu1.Items.Clear();
                }
                catch { }


                #region
                Neusoft.HISFC.BizLogic.Manager.Constant myCons = new Neusoft.HISFC.BizLogic.Manager.Constant();

                ArrayList alMenu = myCons.GetAllList("GFSPTYPES");
                if (alMenu == null || alMenu.Count==0)
                {
                    return;
                    //MessageBox.Show("请维护公费特批归类【TYPE为GFSPTYPES】!");
                }
                ToolStripMenuItem mnuTP = new ToolStripMenuItem("特批归类到:");
                for (int i = 0; i < alMenu.Count; i++)
                {
                    Neusoft.HISFC.Models.Base.Const cs = (Neusoft.HISFC.Models.Base.Const)alMenu[i];
                    ToolStripMenuItem tpType = new ToolStripMenuItem(cs.Name);
                    tpType.Tag = cs.ID;
                    mnuTP.DropDownItems.Add(tpType);
                    tpType.Click += new EventHandler(mnuTP_Click);
                }

                this.contextMenu1.Items.Add(mnuTP);
                this.contextMenu1.Show(this.fpSpread1, new Point(e.X, e.Y));
                #endregion

                if (this.fpSpread1.ActiveSheet.RowCount <= 0)
                {
                    return;
                }

                #region 记录勾选的行

                string rows = "";

                for (int i = 0; i < this.fpSpread1.ActiveSheet.Rows.Count; i++)
                {
                    if (this.fpSpread1.ActiveSheet.IsSelected(i, 0))
                    {
                        rows += "$" + i.ToString() + "|";
                    }
                }
                #endregion

            }
        }
        private void mnuTP_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.fpSpread1.ActiveSheet.Rows.Count; i++)
            {
                if (this.fpSpread1.ActiveSheet.IsSelected(i, 0))
                {
                    //((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1.ActiveSheet.Rows[i].Tag).FT.FTRate.User03 = ((ToolStripMenuItem)sender).Tag.ToString();
                    this.fpSpread1_Sheet1.Cells[i, 2].Tag = ((ToolStripMenuItem)sender).Tag.ToString();
                    this.fpSpread1_Sheet1.Cells[i, 2].Text = ((ToolStripMenuItem)sender).Text;
                    this.fpSpread1_Sheet1.Cells[i, 4].Text = "特殊";
                    this.fpSpread1_Sheet1.Cells[i, 5].Value = true;
                }
            }
        }
        private void fpSpread1_ButtonClicked(object sender, EditorNotifyEventArgs e)
        {
            if (e.Column == 5)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.ActiveRow.Index, 5].Value) == false)
                {
                    int i = 0;
                    i = this.fpSpread1_Sheet1.ActiveRowIndex;
                    this.fpSpread1_Sheet1.Cells[i, 5].Value = false;
                    this.fpSpread1_Sheet1.Cells[i, 3].Text = "0";

                    this.fpSpread1_Sheet1.Cells[i, 1].Text = (((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).OrgItemRate * 100).ToString();
                    if (((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).OrgItemRate == 1)
                    {
                        this.fpSpread1_Sheet1.Cells[i, 4].Text = "自费";
                    }
                    else
                    {
                        this.fpSpread1_Sheet1.Cells[i, 4].Text = "记账";
                    }

                    if (((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)this.fpSpread1_Sheet1.Rows[i].Tag).NewItemRate == 0)
                    {
                        this.fpSpread1_Sheet1.Cells[i, 4].Text = "自费";
                        this.fpSpread1_Sheet1.Cells[i, 1].Text = "100";
                    }

                    this.fpSpread1_Sheet1.Cells[i, 2].Tag = "";
                    this.fpSpread1_Sheet1.Cells[i, 2].Text = "";
                }
            }
        }

        #endregion
		
	}
}
