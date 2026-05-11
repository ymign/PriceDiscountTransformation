namespace Neusoft.HISFC.Components.Terminal.Confirm
{
	partial class ucPatientInformation
	{
		/// <summary> 
		/// 必需的设计器变量。

		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 清理所有正在使用的资源。

		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码

		/// <summary> 
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。

		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.labelDisplayType = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.textBoxCode = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabelPatientName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabelSexAndAge = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabelFreeCount = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabelPatientType = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabelPactType = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabelSeeDept = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ucQueryInpatientNo1 = new Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo();
            this.textBoxPatientName = new System.Windows.Forms.Label();
            this.textBoxPatientType = new System.Windows.Forms.Label();
            this.textBoxSex = new System.Windows.Forms.Label();
            this.textBoxAge = new System.Windows.Forms.Label();
            this.textBoxPactCode = new System.Windows.Forms.Label();
            this.textBoxFreeCount = new System.Windows.Forms.Label();
            this.textBoxSeeDepartment = new System.Windows.Forms.Label();
            this.lblRegDate = new System.Windows.Forms.Label();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblSeeDate = new System.Windows.Forms.Label();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.SuspendLayout();
            // 
            // labelDisplayType
            // 
            this.labelDisplayType.AutoSize = true;
            this.labelDisplayType.Location = new System.Drawing.Point(11, 10);
            this.labelDisplayType.Name = "labelDisplayType";
            this.labelDisplayType.Size = new System.Drawing.Size(47, 12);
            this.labelDisplayType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.labelDisplayType.TabIndex = 0;
            this.labelDisplayType.Text = "门诊号:";
            this.labelDisplayType.Click += new System.EventHandler(this.labelDisplayType_Click);
            // 
            // textBoxCode
            // 
            this.textBoxCode.IsEnter2Tab = false;
            this.textBoxCode.Location = new System.Drawing.Point(68, 7);
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.Size = new System.Drawing.Size(116, 21);
            this.textBoxCode.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.textBoxCode.TabIndex = 1;
            this.textBoxCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCode_KeyDown);
            // 
            // neuLabelPatientName
            // 
            this.neuLabelPatientName.AutoSize = true;
            this.neuLabelPatientName.Location = new System.Drawing.Point(188, 12);
            this.neuLabelPatientName.Name = "neuLabelPatientName";
            this.neuLabelPatientName.Size = new System.Drawing.Size(53, 12);
            this.neuLabelPatientName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabelPatientName.TabIndex = 2;
            this.neuLabelPatientName.Text = "患者姓名";
            // 
            // neuLabelSexAndAge
            // 
            this.neuLabelSexAndAge.AutoSize = true;
            this.neuLabelSexAndAge.Location = new System.Drawing.Point(363, 12);
            this.neuLabelSexAndAge.Name = "neuLabelSexAndAge";
            this.neuLabelSexAndAge.Size = new System.Drawing.Size(53, 12);
            this.neuLabelSexAndAge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabelSexAndAge.TabIndex = 4;
            this.neuLabelSexAndAge.Text = "性别年龄";
            // 
            // neuLabelFreeCount
            // 
            this.neuLabelFreeCount.AutoSize = true;
            this.neuLabelFreeCount.Location = new System.Drawing.Point(529, 12);
            this.neuLabelFreeCount.Name = "neuLabelFreeCount";
            this.neuLabelFreeCount.Size = new System.Drawing.Size(53, 12);
            this.neuLabelFreeCount.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabelFreeCount.TabIndex = 7;
            this.neuLabelFreeCount.Text = "帐户余额";
            // 
            // neuLabelPatientType
            // 
            this.neuLabelPatientType.AutoSize = true;
            this.neuLabelPatientType.Location = new System.Drawing.Point(188, 37);
            this.neuLabelPatientType.Name = "neuLabelPatientType";
            this.neuLabelPatientType.Size = new System.Drawing.Size(53, 12);
            this.neuLabelPatientType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabelPatientType.TabIndex = 10;
            this.neuLabelPatientType.Text = "患者类别";
            // 
            // neuLabelPactType
            // 
            this.neuLabelPactType.AutoSize = true;
            this.neuLabelPactType.Location = new System.Drawing.Point(363, 37);
            this.neuLabelPactType.Name = "neuLabelPactType";
            this.neuLabelPactType.Size = new System.Drawing.Size(53, 12);
            this.neuLabelPactType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabelPactType.TabIndex = 12;
            this.neuLabelPactType.Text = "收费类别";
            // 
            // neuLabelSeeDept
            // 
            this.neuLabelSeeDept.AutoSize = true;
            this.neuLabelSeeDept.Location = new System.Drawing.Point(529, 37);
            this.neuLabelSeeDept.Name = "neuLabelSeeDept";
            this.neuLabelSeeDept.Size = new System.Drawing.Size(53, 12);
            this.neuLabelSeeDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabelSeeDept.TabIndex = 14;
            this.neuLabelSeeDept.Text = "挂号科室";
            // 
            // ucQueryInpatientNo1
            // 
            this.ucQueryInpatientNo1.DefaultInputType = 0;
            this.ucQueryInpatientNo1.InputType = 0;
            //this.ucQueryInpatientNo1.IsDeptOnly = false;
            this.ucQueryInpatientNo1.Location = new System.Drawing.Point(8, 31);
            this.ucQueryInpatientNo1.Name = "ucQueryInpatientNo1";
            this.ucQueryInpatientNo1.PatientInState = "ALL";
            this.ucQueryInpatientNo1.ShowState = Neusoft.HISFC.Components.Common.Controls.enuShowState.All;
            this.ucQueryInpatientNo1.Size = new System.Drawing.Size(176, 27);
            this.ucQueryInpatientNo1.TabIndex = 16;
            this.ucQueryInpatientNo1.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(this.ucQueryInpatientNo1_myEvent);
            // 
            // textBoxPatientName
            // 
            this.textBoxPatientName.AutoSize = true;
            this.textBoxPatientName.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxPatientName.ForeColor = System.Drawing.Color.Red;
            this.textBoxPatientName.Location = new System.Drawing.Point(247, 12);
            this.textBoxPatientName.Name = "textBoxPatientName";
            this.textBoxPatientName.Size = new System.Drawing.Size(35, 14);
            this.textBoxPatientName.TabIndex = 17;
            this.textBoxPatientName.Text = "张三";
            // 
            // textBoxPatientType
            // 
            this.textBoxPatientType.AutoSize = true;
            this.textBoxPatientType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxPatientType.ForeColor = System.Drawing.Color.Red;
            this.textBoxPatientType.Location = new System.Drawing.Point(247, 37);
            this.textBoxPatientType.Name = "textBoxPatientType";
            this.textBoxPatientType.Size = new System.Drawing.Size(35, 14);
            this.textBoxPatientType.TabIndex = 18;
            this.textBoxPatientType.Text = "普通";
            // 
            // textBoxSex
            // 
            this.textBoxSex.AutoSize = true;
            this.textBoxSex.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxSex.ForeColor = System.Drawing.Color.Red;
            this.textBoxSex.Location = new System.Drawing.Point(416, 12);
            this.textBoxSex.Name = "textBoxSex";
            this.textBoxSex.Size = new System.Drawing.Size(21, 14);
            this.textBoxSex.TabIndex = 19;
            this.textBoxSex.Text = "男";
            // 
            // textBoxAge
            // 
            this.textBoxAge.AutoSize = true;
            this.textBoxAge.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxAge.ForeColor = System.Drawing.Color.Red;
            this.textBoxAge.Location = new System.Drawing.Point(439, 12);
            this.textBoxAge.Name = "textBoxAge";
            this.textBoxAge.Size = new System.Drawing.Size(35, 14);
            this.textBoxAge.TabIndex = 20;
            this.textBoxAge.Text = "18岁";
            // 
            // textBoxPactCode
            // 
            this.textBoxPactCode.AutoSize = true;
            this.textBoxPactCode.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxPactCode.ForeColor = System.Drawing.Color.Red;
            this.textBoxPactCode.Location = new System.Drawing.Point(416, 37);
            this.textBoxPactCode.Name = "textBoxPactCode";
            this.textBoxPactCode.Size = new System.Drawing.Size(63, 14);
            this.textBoxPactCode.TabIndex = 21;
            this.textBoxPactCode.Text = "居民医保";
            // 
            // textBoxFreeCount
            // 
            this.textBoxFreeCount.AutoSize = true;
            this.textBoxFreeCount.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxFreeCount.ForeColor = System.Drawing.Color.Red;
            this.textBoxFreeCount.Location = new System.Drawing.Point(588, 12);
            this.textBoxFreeCount.Name = "textBoxFreeCount";
            this.textBoxFreeCount.Size = new System.Drawing.Size(42, 14);
            this.textBoxFreeCount.TabIndex = 22;
            this.textBoxFreeCount.Text = "28.00";
            // 
            // textBoxSeeDepartment
            // 
            this.textBoxSeeDepartment.AutoSize = true;
            this.textBoxSeeDepartment.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxSeeDepartment.ForeColor = System.Drawing.Color.Red;
            this.textBoxSeeDepartment.Location = new System.Drawing.Point(588, 37);
            this.textBoxSeeDepartment.Name = "textBoxSeeDepartment";
            this.textBoxSeeDepartment.Size = new System.Drawing.Size(49, 14);
            this.textBoxSeeDepartment.TabIndex = 23;
            this.textBoxSeeDepartment.Text = "急诊科";
            // 
            // lblRegDate
            // 
            this.lblRegDate.AutoSize = true;
            this.lblRegDate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblRegDate.ForeColor = System.Drawing.Color.Red;
            this.lblRegDate.Location = new System.Drawing.Point(724, 12);
            this.lblRegDate.Name = "lblRegDate";
            this.lblRegDate.Size = new System.Drawing.Size(119, 14);
            this.lblRegDate.TabIndex = 25;
            this.lblRegDate.Text = "1900-01-01 01:01";
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(665, 12);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(65, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 24;
            this.neuLabel1.Text = "挂号日期：";
            // 
            // lblSeeDate
            // 
            this.lblSeeDate.AutoSize = true;
            this.lblSeeDate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSeeDate.ForeColor = System.Drawing.Color.Red;
            this.lblSeeDate.Location = new System.Drawing.Point(724, 37);
            this.lblSeeDate.Name = "lblSeeDate";
            this.lblSeeDate.Size = new System.Drawing.Size(119, 14);
            this.lblSeeDate.TabIndex = 27;
            this.lblSeeDate.Text = "1900-01-01 01:01";
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Location = new System.Drawing.Point(665, 37);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(65, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 26;
            this.neuLabel2.Text = "看诊日期：";
            // 
            // ucPatientInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.lblSeeDate);
            this.Controls.Add(this.neuLabel2);
            this.Controls.Add(this.lblRegDate);
            this.Controls.Add(this.neuLabel1);
            this.Controls.Add(this.textBoxSeeDepartment);
            this.Controls.Add(this.textBoxFreeCount);
            this.Controls.Add(this.textBoxPactCode);
            this.Controls.Add(this.textBoxAge);
            this.Controls.Add(this.textBoxSex);
            this.Controls.Add(this.textBoxPatientType);
            this.Controls.Add(this.textBoxPatientName);
            this.Controls.Add(this.neuLabelSeeDept);
            this.Controls.Add(this.neuLabelPactType);
            this.Controls.Add(this.neuLabelPatientType);
            this.Controls.Add(this.neuLabelFreeCount);
            this.Controls.Add(this.neuLabelSexAndAge);
            this.Controls.Add(this.neuLabelPatientName);
            this.Controls.Add(this.textBoxCode);
            this.Controls.Add(this.labelDisplayType);
            this.Controls.Add(this.ucQueryInpatientNo1);
            this.Name = "ucPatientInformation";
            this.Size = new System.Drawing.Size(885, 61);
            this.Load += new System.EventHandler(this.ucPatientInformation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		/// <summary>
		/// 病历号(&A)
		/// </summary>
		public Neusoft.FrameWork.WinForms.Controls.NeuLabel labelDisplayType;
		/// <summary>
		/// 病历号(&A)
		/// </summary>
		public Neusoft.FrameWork.WinForms.Controls.NeuTextBox textBoxCode;
		/// <summary>
		/// 患者姓名

		/// </summary>
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabelPatientName;
		/// <summary>
		/// 性别年龄
		/// </summary>
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabelSexAndAge;
		/// <summary>
		/// 帐户余额
		/// </summary>
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabelFreeCount;
		/// <summary>
		/// 患者类别

		/// </summary>
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabelPatientType;
		/// <summary>
		/// 收费类别
		/// </summary>
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabelPactType;
		/// <summary>
		/// 就诊科室
		/// </summary>
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabelSeeDept;
		private System.Windows.Forms.ToolTip toolTip1;
		/// <summary>
		/// 住院号控件

		/// </summary>
		public Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo ucQueryInpatientNo1;
        private System.Windows.Forms.Label textBoxPatientName;
        private System.Windows.Forms.Label textBoxPatientType;
        private System.Windows.Forms.Label textBoxSex;
        private System.Windows.Forms.Label textBoxAge;
        private System.Windows.Forms.Label textBoxPactCode;
        private System.Windows.Forms.Label textBoxFreeCount;
        private System.Windows.Forms.Label textBoxSeeDepartment;
        private System.Windows.Forms.Label lblRegDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private System.Windows.Forms.Label lblSeeDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
	}
}
