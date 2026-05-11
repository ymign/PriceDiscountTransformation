namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucSetRealInvoiceNo
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
            this.plMain = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.txtReal = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblMessage = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbInvoiceType = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.btCancel = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btOK = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.txtInvoiceNoComp = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.plMain.SuspendLayout();
            this.neuGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // plMain
            // 
            this.plMain.Controls.Add(this.neuGroupBox1);
            this.plMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plMain.Location = new System.Drawing.Point(0, 0);
            this.plMain.Name = "plMain";
            this.plMain.Size = new System.Drawing.Size(251, 220);
            this.plMain.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.plMain.TabIndex = 0;
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.txtInvoiceNoComp);
            this.neuGroupBox1.Controls.Add(this.neuLabel2);
            this.neuGroupBox1.Controls.Add(this.txtReal);
            this.neuGroupBox1.Controls.Add(this.neuLabel3);
            this.neuGroupBox1.Controls.Add(this.lblMessage);
            this.neuGroupBox1.Controls.Add(this.neuLabel1);
            this.neuGroupBox1.Controls.Add(this.cmbInvoiceType);
            this.neuGroupBox1.Controls.Add(this.btCancel);
            this.neuGroupBox1.Controls.Add(this.btOK);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(251, 220);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 0;
            this.neuGroupBox1.TabStop = false;
            // 
            // txtReal
            // 
            this.txtReal.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtReal.IsEnter2Tab = false;
            this.txtReal.Location = new System.Drawing.Point(100, 93);
            this.txtReal.MaxLength = 12;
            this.txtReal.Name = "txtReal";
            this.txtReal.Size = new System.Drawing.Size(121, 21);
            this.txtReal.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtReal.TabIndex = 8;
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Location = new System.Drawing.Point(17, 96);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(77, 12);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 7;
            this.neuLabel3.Text = "实际发票号：";
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.ForeColor = System.Drawing.Color.Blue;
            this.lblMessage.Location = new System.Drawing.Point(29, 19);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(192, 28);
            this.lblMessage.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblMessage.TabIndex = 6;
            this.lblMessage.Text = "收款员：";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(29, 65);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(65, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 3;
            this.neuLabel1.Text = "发票类别：";
            // 
            // cmbInvoiceType
            // 
            this.cmbInvoiceType.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbInvoiceType.FormattingEnabled = true;
            this.cmbInvoiceType.IsEnter2Tab = false;
            this.cmbInvoiceType.IsFlat = true;
            this.cmbInvoiceType.IsLike = true;
            this.cmbInvoiceType.Location = new System.Drawing.Point(100, 58);
            this.cmbInvoiceType.Name = "cmbInvoiceType";
            this.cmbInvoiceType.PopForm = null;
            this.cmbInvoiceType.ShowCustomerList = false;
            this.cmbInvoiceType.ShowID = false;
            this.cmbInvoiceType.Size = new System.Drawing.Size(121, 20);
            this.cmbInvoiceType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.cmbInvoiceType.TabIndex = 2;
            this.cmbInvoiceType.Tag = "";
            this.cmbInvoiceType.ToolBarUse = false;
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(134, 166);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "退出";
            this.btCancel.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(32, 166);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btOK.TabIndex = 0;
            this.btOK.Text = "确定";
            this.btOK.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // txtInvoiceNoComp
            // 
            this.txtInvoiceNoComp.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtInvoiceNoComp.IsEnter2Tab = false;
            this.txtInvoiceNoComp.Location = new System.Drawing.Point(100, 129);
            this.txtInvoiceNoComp.MaxLength = 12;
            this.txtInvoiceNoComp.Name = "txtInvoiceNoComp";
            this.txtInvoiceNoComp.Size = new System.Drawing.Size(121, 21);
            this.txtInvoiceNoComp.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtInvoiceNoComp.TabIndex = 10;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Location = new System.Drawing.Point(17, 132);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(77, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 9;
            this.neuLabel2.Text = "电脑发票号：";
            // 
            // ucSetRealInvoiceNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.plMain);
            this.Name = "ucSetRealInvoiceNo";
            this.Size = new System.Drawing.Size(251, 220);
            this.plMain.ResumeLayout(false);
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuPanel plMain;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblMessage;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbInvoiceType;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btCancel;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btOK;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtReal;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtInvoiceNoComp;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
    }
}
