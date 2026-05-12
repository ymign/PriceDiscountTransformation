namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucUpdateUsedInvoiceNo
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
            this.tbRealInvoiceNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.btnUpdate = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel8 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtBeginNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtEndNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtUserNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.btnCancel = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbUserNumbers = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbInvoiceType = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.btnClear = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.SuspendLayout();
            // 
            // tbRealInvoiceNO
            // 
            this.tbRealInvoiceNO.BackColor = System.Drawing.Color.White;
            this.tbRealInvoiceNO.Enabled = false;
            this.tbRealInvoiceNO.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRealInvoiceNO.IsEnter2Tab = false;
            this.tbRealInvoiceNO.Location = new System.Drawing.Point(348, 114);
            this.tbRealInvoiceNO.MaxLength = 12;
            this.tbRealInvoiceNO.Name = "tbRealInvoiceNO";
            this.tbRealInvoiceNO.Size = new System.Drawing.Size(122, 22);
            this.tbRealInvoiceNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbRealInvoiceNO.TabIndex = 8;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("宋体", 10F);
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(285, 148);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(81, 33);
            this.btnUpdate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnUpdate.TabIndex = 7;
            this.btnUpdate.Text = "更新(&U)";
            this.btnUpdate.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel1.Location = new System.Drawing.Point(258, 119);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(84, 14);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 6;
            this.neuLabel1.Text = "下一发票号:";
            // 
            // neuLabel8
            // 
            this.neuLabel8.AutoSize = true;
            this.neuLabel8.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel8.Location = new System.Drawing.Point(32, 119);
            this.neuLabel8.Name = "neuLabel8";
            this.neuLabel8.Size = new System.Drawing.Size(77, 14);
            this.neuLabel8.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel8.TabIndex = 22;
            this.neuLabel8.Text = "已 用 号：";
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel4.Location = new System.Drawing.Point(264, 85);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(77, 14);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 19;
            this.neuLabel4.Text = "截 止 号：";
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel2.Location = new System.Drawing.Point(32, 85);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(77, 14);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 18;
            this.neuLabel2.Text = "起 始 号：";
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel3.Location = new System.Drawing.Point(29, 14);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(77, 14);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 16;
            this.neuLabel3.Text = "收据类型：";
            // 
            // txtBeginNO
            // 
            this.txtBeginNO.BackColor = System.Drawing.Color.White;
            this.txtBeginNO.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBeginNO.IsEnter2Tab = false;
            this.txtBeginNO.Location = new System.Drawing.Point(115, 80);
            this.txtBeginNO.MaxLength = 12;
            this.txtBeginNO.Name = "txtBeginNO";
            this.txtBeginNO.Size = new System.Drawing.Size(122, 22);
            this.txtBeginNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtBeginNO.TabIndex = 23;
            // 
            // txtEndNO
            // 
            this.txtEndNO.BackColor = System.Drawing.Color.White;
            this.txtEndNO.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEndNO.IsEnter2Tab = false;
            this.txtEndNO.Location = new System.Drawing.Point(348, 80);
            this.txtEndNO.MaxLength = 12;
            this.txtEndNO.Name = "txtEndNO";
            this.txtEndNO.Size = new System.Drawing.Size(122, 22);
            this.txtEndNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtEndNO.TabIndex = 24;
            // 
            // txtUserNO
            // 
            this.txtUserNO.BackColor = System.Drawing.Color.White;
            this.txtUserNO.Enabled = false;
            this.txtUserNO.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserNO.IsEnter2Tab = false;
            this.txtUserNO.Location = new System.Drawing.Point(115, 114);
            this.txtUserNO.MaxLength = 12;
            this.txtUserNO.Name = "txtUserNO";
            this.txtUserNO.ReadOnly = true;
            this.txtUserNO.Size = new System.Drawing.Size(122, 22);
            this.txtUserNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtUserNO.TabIndex = 25;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("宋体", 10F);
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(388, 148);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 33);
            this.btnCancel.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel5.Location = new System.Drawing.Point(32, 47);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(77, 14);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 28;
            this.neuLabel5.Text = "使用号段：";
            // 
            // cmbUserNumbers
            // 
            this.cmbUserNumbers.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbUserNumbers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserNumbers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.cmbUserNumbers.FormattingEnabled = true;
            this.cmbUserNumbers.IsEnter2Tab = false;
            this.cmbUserNumbers.IsFlat = false;
            this.cmbUserNumbers.IsLike = true;
            this.cmbUserNumbers.IsListOnly = false;
            this.cmbUserNumbers.IsPopForm = true;
            this.cmbUserNumbers.IsShowCustomerList = false;
            this.cmbUserNumbers.IsShowID = false;
            this.cmbUserNumbers.Location = new System.Drawing.Point(115, 43);
            this.cmbUserNumbers.Name = "cmbUserNumbers";
            this.cmbUserNumbers.PopForm = null;
            this.cmbUserNumbers.ShowCustomerList = false;
            this.cmbUserNumbers.ShowID = false;
            this.cmbUserNumbers.Size = new System.Drawing.Size(354, 24);
            this.cmbUserNumbers.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbUserNumbers.TabIndex = 29;
            this.cmbUserNumbers.Tag = "";
            this.cmbUserNumbers.ToolBarUse = false;
            // 
            // cmbInvoiceType
            // 
            this.cmbInvoiceType.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbInvoiceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInvoiceType.Enabled = false;
            this.cmbInvoiceType.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.cmbInvoiceType.FormattingEnabled = true;
            this.cmbInvoiceType.IsEnter2Tab = false;
            this.cmbInvoiceType.IsFlat = false;
            this.cmbInvoiceType.IsLike = true;
            this.cmbInvoiceType.IsListOnly = false;
            this.cmbInvoiceType.IsPopForm = true;
            this.cmbInvoiceType.IsShowCustomerList = false;
            this.cmbInvoiceType.IsShowID = false;
            this.cmbInvoiceType.Location = new System.Drawing.Point(116, 10);
            this.cmbInvoiceType.Name = "cmbInvoiceType";
            this.cmbInvoiceType.PopForm = null;
            this.cmbInvoiceType.ShowCustomerList = false;
            this.cmbInvoiceType.ShowID = false;
            this.cmbInvoiceType.Size = new System.Drawing.Size(121, 24);
            this.cmbInvoiceType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbInvoiceType.TabIndex = 31;
            this.cmbInvoiceType.Tag = "";
            this.cmbInvoiceType.ToolBarUse = false;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("宋体", 10F);
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.Location = new System.Drawing.Point(188, 148);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(81, 33);
            this.btnClear.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnClear.TabIndex = 32;
            this.btnClear.Text = "清空(&U)";
            this.btnClear.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // ucUpdateUsedInvoiceNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.cmbInvoiceType);
            this.Controls.Add(this.cmbUserNumbers);
            this.Controls.Add(this.neuLabel5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtUserNO);
            this.Controls.Add(this.txtEndNO);
            this.Controls.Add(this.txtBeginNO);
            this.Controls.Add(this.neuLabel8);
            this.Controls.Add(this.neuLabel4);
            this.Controls.Add(this.neuLabel2);
            this.Controls.Add(this.neuLabel3);
            this.Controls.Add(this.tbRealInvoiceNO);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.neuLabel1);
            this.Name = "ucUpdateUsedInvoiceNo";
            this.Size = new System.Drawing.Size(488, 190);
            this.Load += new System.EventHandler(this.ctlUpdateUsedInvoiceNo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbRealInvoiceNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnUpdate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel8;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtBeginNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtEndNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtUserNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnCancel;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbUserNumbers;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbInvoiceType;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnClear;
    }
}
