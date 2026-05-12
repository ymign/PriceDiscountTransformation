using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
//using Neusoft.NFC.Interface.Forms;
using Neusoft.FrameWork.WinForms.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
	/// <summary>
	/// frmChooseDate 的摘要说明。
	/// </summary>
	public class frmChoosePatientInfo : BaseForm {
        //private Neusoft.FrameWork.WinForms.Controls.NeuPictureBox pictureBox1;
        //private Neusoft.FrameWork.WinForms.Controls.NeuLabel label3;
        //private Neusoft.FrameWork.WinForms.Controls.NeuButton btnOK;
        //private Neusoft.FrameWork.WinForms.Controls.NeuButton btnExit;
        //private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpBeginDate;
        //private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpEndDate;

        private Neusoft.FrameWork.WinForms.Controls.NeuPictureBox pictureBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label3;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnOK;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnExit;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpBeginDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpEndDate;

		private bool myIsOneDate = false;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblBeginDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblEndDate;
        private Label label1;
        private TextBox tbcardno;
        private CheckBox ckquery;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		//是否只可以选择一个日期
		public bool IsOneDate {
			set{
				this.myIsOneDate = value;
				//this.Init();
			}
		}

		//起始时间
		public DateTime DateBegin {
			get{return this.dtpBeginDate.Value;}
			set{this.dtpBeginDate.Value = value;}
		}

		//终止时间
		public DateTime DateEnd {
			get{return this.dtpEndDate.Value;}
			set{this.dtpEndDate.Value = value;}
		}
        /// <summary>
        /// 卡号
        /// </summary>
        public String CardNo
        {
            get { return this.tbcardno.Text; }
            set { this.tbcardno.Text = value; }
        
        }

        public bool CkQuery
        {
            get { return this.ckquery.Checked; }
            set { this.ckquery.Checked = value; }
        
        }
 

        public frmChoosePatientInfo()
        {
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			// 初始化
			this.Init();
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		public void Init() {
			try {
				//取系统日期
				Neusoft.FrameWork.Management.DataBaseManger dataBase = new Neusoft.FrameWork.Management.DataBaseManger();
				string sysDate = dataBase.GetSysDate();
				this.dtpBeginDate.Value = DateTime.Parse(sysDate + " 00:00:00");	//起始日期
				this.dtpEndDate.Value   = DateTime.Parse(sysDate + " 23:59:59");	//结束日期

				if(this.myIsOneDate) {
					//用户只可以选择一个日期的时候
					this.lblBeginDate.Text  = "日期:";
					//不显示终止日期
					this.lblEndDate.Visible = false; 
					this.dtpEndDate.Visible = false;
				}
				else {
					//用户只可以选择起始日期和终止日期的时候
					this.lblBeginDate.Text  = "起始日期:";
					//不显示终止日期
					this.lblEndDate.Visible = true; 
					this.dtpEndDate.Visible = true;
				}
			}
			catch{}
		}


		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChoosePatientInfo));
            this.dtpBeginDate = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.dtpEndDate = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.lblBeginDate = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblEndDate = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.pictureBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuPictureBox();
            this.label3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.btnOK = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnExit = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tbcardno = new System.Windows.Forms.TextBox();
            this.ckquery = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpBeginDate
            // 
            this.dtpBeginDate.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpBeginDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBeginDate.IsEnter2Tab = false;
            this.dtpBeginDate.Location = new System.Drawing.Point(117, 50);
            this.dtpBeginDate.Name = "dtpBeginDate";
            this.dtpBeginDate.Size = new System.Drawing.Size(151, 21);
            this.dtpBeginDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpBeginDate.TabIndex = 0;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.IsEnter2Tab = false;
            this.dtpEndDate.Location = new System.Drawing.Point(117, 78);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(151, 21);
            this.dtpEndDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpEndDate.TabIndex = 0;
            // 
            // lblBeginDate
            // 
            this.lblBeginDate.Location = new System.Drawing.Point(54, 52);
            this.lblBeginDate.Name = "lblBeginDate";
            this.lblBeginDate.Size = new System.Drawing.Size(60, 17);
            this.lblBeginDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblBeginDate.TabIndex = 2;
            this.lblBeginDate.Text = "起始时间:";
            this.lblBeginDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblEndDate
            // 
            this.lblEndDate.Location = new System.Drawing.Point(54, 80);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(60, 17);
            this.lblEndDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblEndDate.TabIndex = 2;
            this.lblEndDate.Text = "终止时间:";
            this.lblEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(54, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 17);
            this.label3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.label3.TabIndex = 2;
            this.label3.Text = "请选择日期:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(50, 148);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(79, 23);
            this.btnOK.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(169, 147);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(79, 23);
            this.btnExit.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "退出(&X)";
            this.btnExit.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "病历号:";
            // 
            // tbcardno
            // 
            this.tbcardno.Location = new System.Drawing.Point(117, 110);
            this.tbcardno.Name = "tbcardno";
            this.tbcardno.Size = new System.Drawing.Size(151, 21);
            this.tbcardno.TabIndex = 6;
            // 
            // ckquery
            // 
            this.ckquery.AutoSize = true;
            this.ckquery.Location = new System.Drawing.Point(208, 21);
            this.ckquery.Name = "ckquery";
            this.ckquery.Size = new System.Drawing.Size(60, 16);
            this.ckquery.TabIndex = 7;
            this.ckquery.Text = "发票号";
            this.ckquery.UseVisualStyleBackColor = true;
            // 
            // frmChoosePatientInfo
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(333, 197);
            this.Controls.Add(this.ckquery);
            this.Controls.Add(this.tbcardno);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblBeginDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpBeginDate);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.btnExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "frmChoosePatientInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "处方查询";
            this.Load += new System.EventHandler(this.frmChoosePatientInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btnExit_Click(object sender, System.EventArgs e) {
			this.FindForm().Close();
		}

		private void btnOK_Click(object sender, System.EventArgs e) {
			DateTime dateBegin = this.dtpBeginDate.Value;
			DateTime dateEnd   = this.dtpEndDate.Value;
            string cardno = this.tbcardno.Text;

			if(dateEnd.CompareTo(dateBegin) < 0) {
				MessageBox.Show("终止时间必须大于起始时间！","提示");
				return;
			}

			this.DialogResult = DialogResult.OK;

		}

	
        private void frmChoosePatientInfo_Load(object sender, EventArgs e)
        {

        }

	}
}
