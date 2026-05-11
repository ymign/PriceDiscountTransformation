namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    partial class ucPatientFeeQueryAll
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
            this.gbQuery = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.neuGroupBox3 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.cmbDoct = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel27 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuButton1 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.dtpEndTime = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.neuLabel6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.dtpBeginTime = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbPact = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbDept = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbPatientState = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuGroupBox4 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.btnQuery = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.txtName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel7 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.ucQueryInpatientNo2 = new Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo();
            this.neuSplitter2 = new Neusoft.FrameWork.WinForms.Controls.NeuSplitter();
            this.gbQuery.SuspendLayout();
            this.neuGroupBox3.SuspendLayout();
            this.neuGroupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuPanel2
            // 
            this.neuPanel2.Location = new System.Drawing.Point(214, 2);
            this.neuPanel2.Size = new System.Drawing.Size(784, 756);
            // 
            // gbQuery
            // 
            this.gbQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.gbQuery.Controls.Add(this.neuGroupBox3);
            this.gbQuery.Controls.Add(this.neuGroupBox4);
            this.gbQuery.Dock = System.Windows.Forms.DockStyle.Left;
            this.gbQuery.Font = new System.Drawing.Font("宋体", 1F);
            this.gbQuery.Location = new System.Drawing.Point(2, 2);
            this.gbQuery.Name = "gbQuery";
            this.gbQuery.Size = new System.Drawing.Size(212, 756);
            this.gbQuery.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gbQuery.TabIndex = 3;
            this.gbQuery.TabStop = false;
            // 
            // neuGroupBox3
            // 
            this.neuGroupBox3.Controls.Add(this.cmbDoct);
            this.neuGroupBox3.Controls.Add(this.neuLabel27);
            this.neuGroupBox3.Controls.Add(this.neuButton1);
            this.neuGroupBox3.Controls.Add(this.dtpEndTime);
            this.neuGroupBox3.Controls.Add(this.neuLabel6);
            this.neuGroupBox3.Controls.Add(this.dtpBeginTime);
            this.neuGroupBox3.Controls.Add(this.neuLabel5);
            this.neuGroupBox3.Controls.Add(this.cmbPact);
            this.neuGroupBox3.Controls.Add(this.neuLabel4);
            this.neuGroupBox3.Controls.Add(this.cmbDept);
            this.neuGroupBox3.Controls.Add(this.neuLabel3);
            this.neuGroupBox3.Controls.Add(this.cmbPatientState);
            this.neuGroupBox3.Controls.Add(this.neuLabel2);
            this.neuGroupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuGroupBox3.Font = new System.Drawing.Font("宋体", 9F);
            this.neuGroupBox3.Location = new System.Drawing.Point(3, 145);
            this.neuGroupBox3.Name = "neuGroupBox3";
            this.neuGroupBox3.Size = new System.Drawing.Size(206, 608);
            this.neuGroupBox3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox3.TabIndex = 3;
            this.neuGroupBox3.TabStop = false;
            this.neuGroupBox3.Text = "批量查询";
            // 
            // cmbDoct
            // 
            this.cmbDoct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDoct.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbDoct.FormattingEnabled = true;
            this.cmbDoct.IsEnter2Tab = false;
            this.cmbDoct.IsFlat = false;
            this.cmbDoct.IsLike = true;
            this.cmbDoct.IsListOnly = false;
            this.cmbDoct.IsPopForm = true;
            this.cmbDoct.IsShowCustomerList = false;
            this.cmbDoct.IsShowID = false;
            this.cmbDoct.Location = new System.Drawing.Point(14, 131);
            this.cmbDoct.Name = "cmbDoct";
            this.cmbDoct.PopForm = null;
            this.cmbDoct.ShowCustomerList = false;
            this.cmbDoct.ShowID = false;
            this.cmbDoct.Size = new System.Drawing.Size(186, 20);
            this.cmbDoct.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDoct.TabIndex = 14;
            this.cmbDoct.Tag = "";
            this.cmbDoct.ToolBarUse = false;
            // 
            // neuLabel27
            // 
            this.neuLabel27.AutoSize = true;
            this.neuLabel27.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel27.Location = new System.Drawing.Point(11, 112);
            this.neuLabel27.Name = "neuLabel27";
            this.neuLabel27.Size = new System.Drawing.Size(77, 14);
            this.neuLabel27.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel27.TabIndex = 13;
            this.neuLabel27.Text = "管床医生：";
            // 
            // neuButton1
            // 
            this.neuButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.neuButton1.Location = new System.Drawing.Point(85, 321);
            this.neuButton1.Name = "neuButton1";
            this.neuButton1.Size = new System.Drawing.Size(75, 23);
            this.neuButton1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuButton1.TabIndex = 4;
            this.neuButton1.Text = "查询(&F)";
            this.neuButton1.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.neuButton1.UseVisualStyleBackColor = true;
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpEndTime.CalendarFont = new System.Drawing.Font("宋体", 10F);
            this.dtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.IsEnter2Tab = false;
            this.dtpEndTime.Location = new System.Drawing.Point(14, 268);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(186, 21);
            this.dtpEndTime.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpEndTime.TabIndex = 12;
            // 
            // neuLabel6
            // 
            this.neuLabel6.AutoSize = true;
            this.neuLabel6.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel6.Location = new System.Drawing.Point(11, 245);
            this.neuLabel6.Name = "neuLabel6";
            this.neuLabel6.Size = new System.Drawing.Size(28, 14);
            this.neuLabel6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel6.TabIndex = 10;
            this.neuLabel6.Text = "至:";
            // 
            // dtpBeginTime
            // 
            this.dtpBeginTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpBeginTime.CalendarFont = new System.Drawing.Font("宋体", 10F);
            this.dtpBeginTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpBeginTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBeginTime.IsEnter2Tab = false;
            this.dtpBeginTime.Location = new System.Drawing.Point(14, 220);
            this.dtpBeginTime.Name = "dtpBeginTime";
            this.dtpBeginTime.Size = new System.Drawing.Size(186, 21);
            this.dtpBeginTime.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpBeginTime.TabIndex = 9;
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel5.Location = new System.Drawing.Point(11, 199);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(112, 14);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 8;
            this.neuLabel5.Text = "入院时间范围从:";
            // 
            // cmbPact
            // 
            this.cmbPact.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPact.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbPact.FormattingEnabled = true;
            this.cmbPact.IsEnter2Tab = false;
            this.cmbPact.IsFlat = false;
            this.cmbPact.IsLike = true;
            this.cmbPact.IsListOnly = false;
            this.cmbPact.IsPopForm = true;
            this.cmbPact.IsShowCustomerList = false;
            this.cmbPact.IsShowID = false;
            this.cmbPact.Location = new System.Drawing.Point(14, 173);
            this.cmbPact.Name = "cmbPact";
            this.cmbPact.PopForm = null;
            this.cmbPact.ShowCustomerList = false;
            this.cmbPact.ShowID = false;
            this.cmbPact.Size = new System.Drawing.Size(186, 20);
            this.cmbPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbPact.TabIndex = 7;
            this.cmbPact.Tag = "";
            this.cmbPact.ToolBarUse = false;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel4.Location = new System.Drawing.Point(11, 153);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(70, 14);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 6;
            this.neuLabel4.Text = "合同单位:";
            // 
            // cmbDept
            // 
            this.cmbDept.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDept.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbDept.FormattingEnabled = true;
            this.cmbDept.IsEnter2Tab = false;
            this.cmbDept.IsFlat = false;
            this.cmbDept.IsLike = true;
            this.cmbDept.IsListOnly = false;
            this.cmbDept.IsPopForm = true;
            this.cmbDept.IsShowCustomerList = false;
            this.cmbDept.IsShowID = false;
            this.cmbDept.Location = new System.Drawing.Point(14, 89);
            this.cmbDept.Name = "cmbDept";
            this.cmbDept.PopForm = null;
            this.cmbDept.ShowCustomerList = false;
            this.cmbDept.ShowID = false;
            this.cmbDept.Size = new System.Drawing.Size(186, 20);
            this.cmbDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDept.TabIndex = 5;
            this.cmbDept.Tag = "";
            this.cmbDept.ToolBarUse = false;
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel3.Location = new System.Drawing.Point(11, 70);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(70, 14);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 4;
            this.neuLabel3.Text = "科室范围:";
            // 
            // cmbPatientState
            // 
            this.cmbPatientState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPatientState.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbPatientState.FormattingEnabled = true;
            this.cmbPatientState.IsEnter2Tab = false;
            this.cmbPatientState.IsFlat = false;
            this.cmbPatientState.IsLike = true;
            this.cmbPatientState.IsListOnly = false;
            this.cmbPatientState.IsPopForm = true;
            this.cmbPatientState.IsShowCustomerList = false;
            this.cmbPatientState.IsShowID = false;
            this.cmbPatientState.Location = new System.Drawing.Point(14, 44);
            this.cmbPatientState.Name = "cmbPatientState";
            this.cmbPatientState.PopForm = null;
            this.cmbPatientState.ShowCustomerList = false;
            this.cmbPatientState.ShowID = false;
            this.cmbPatientState.Size = new System.Drawing.Size(186, 20);
            this.cmbPatientState.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbPatientState.TabIndex = 3;
            this.cmbPatientState.Tag = "";
            this.cmbPatientState.ToolBarUse = false;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel2.Location = new System.Drawing.Point(11, 25);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(70, 14);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 2;
            this.neuLabel2.Text = "患者范围:";
            // 
            // neuGroupBox4
            // 
            this.neuGroupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.neuGroupBox4.Controls.Add(this.btnQuery);
            this.neuGroupBox4.Controls.Add(this.txtName);
            this.neuGroupBox4.Controls.Add(this.neuLabel7);
            this.neuGroupBox4.Controls.Add(this.ucQueryInpatientNo2);
            this.neuGroupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox4.Font = new System.Drawing.Font("宋体", 9F);
            this.neuGroupBox4.Location = new System.Drawing.Point(3, 5);
            this.neuGroupBox4.Name = "neuGroupBox4";
            this.neuGroupBox4.Size = new System.Drawing.Size(206, 140);
            this.neuGroupBox4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox4.TabIndex = 2;
            this.neuGroupBox4.TabStop = false;
            this.neuGroupBox4.Text = "单人查询";
            // 
            // btnQuery
            // 
            this.btnQuery.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQuery.Location = new System.Drawing.Point(89, 91);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnQuery.TabIndex = 3;
            this.btnQuery.Text = "查询(&Q)";
            this.btnQuery.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnQuery.UseVisualStyleBackColor = true;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("宋体", 10F);
            this.txtName.IsEnter2Tab = false;
            this.txtName.Location = new System.Drawing.Point(67, 53);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(137, 23);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 2;
            // 
            // neuLabel7
            // 
            this.neuLabel7.AutoSize = true;
            this.neuLabel7.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel7.Location = new System.Drawing.Point(15, 60);
            this.neuLabel7.Name = "neuLabel7";
            this.neuLabel7.Size = new System.Drawing.Size(42, 14);
            this.neuLabel7.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel7.TabIndex = 1;
            this.neuLabel7.Text = "姓名:";
            // 
            // ucQueryInpatientNo2
            // 
            this.ucQueryInpatientNo2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucQueryInpatientNo2.DefaultInputType = 0;
            this.ucQueryInpatientNo2.Font = new System.Drawing.Font("宋体", 10F);
            this.ucQueryInpatientNo2.InputType = 0;
            this.ucQueryInpatientNo2.Location = new System.Drawing.Point(6, 20);
            this.ucQueryInpatientNo2.Name = "ucQueryInpatientNo2";
            this.ucQueryInpatientNo2.ShowState = Neusoft.HISFC.Components.Common.Controls.enuShowState.All;
            this.ucQueryInpatientNo2.Size = new System.Drawing.Size(198, 27);
            this.ucQueryInpatientNo2.TabIndex = 0;
            // 
            // neuSplitter2
            // 
            this.neuSplitter2.Location = new System.Drawing.Point(214, 2);
            this.neuSplitter2.Name = "neuSplitter2";
            this.neuSplitter2.Size = new System.Drawing.Size(5, 756);
            this.neuSplitter2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSplitter2.TabIndex = 4;
            this.neuSplitter2.TabStop = false;
            // 
            // ucPatientFeeQueryAll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuSplitter2);
            this.Controls.Add(this.gbQuery);
            this.Name = "ucPatientFeeQueryAll";
            this.Controls.SetChildIndex(this.gbQuery, 0);
            this.Controls.SetChildIndex(this.neuPanel2, 0);
            this.Controls.SetChildIndex(this.neuSplitter2, 0);
            this.gbQuery.ResumeLayout(false);
            this.neuGroupBox3.ResumeLayout(false);
            this.neuGroupBox3.PerformLayout();
            this.neuGroupBox4.ResumeLayout(false);
            this.neuGroupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gbQuery;
        private Neusoft.FrameWork.WinForms.Controls.NeuSplitter neuSplitter2;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox3;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDoct;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel27;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton neuButton1;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpEndTime;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel6;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpBeginTime;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbPact;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDept;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbPatientState;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox4;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnQuery;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtName;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel7;
        private Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo ucQueryInpatientNo2;
    }
}
