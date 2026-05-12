namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucChaoYangDataDeal
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
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbRebate;
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbMCardNO;
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbPact;
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbDoct;
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            this.trvPatient = new System.Windows.Forms.TreeView();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlTime = new System.Windows.Forms.Panel();
            this.pnlPatient = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.txtDoct = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbMCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtMarkNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtRegDept = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtSex = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbAge = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtRebate = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtPact = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lbCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbSex = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblMarkNo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbAge = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbRegDept = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.pnlItem = new System.Windows.Forms.Panel();
            this.fpSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.label2 = new System.Windows.Forms.Label();
            this.neuTextBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.label1 = new System.Windows.Forms.Label();
            lbRebate = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            lbMCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            lbPact = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            lbDoct = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.pnlLeft.SuspendLayout();
            this.pnlTime.SuspendLayout();
            this.pnlPatient.SuspendLayout();
            this.pnlItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbRebate
            // 
            lbRebate.AutoSize = true;
            lbRebate.Font = new System.Drawing.Font("宋体", 10F);
            lbRebate.Location = new System.Drawing.Point(386, 73);
            lbRebate.Name = "lbRebate";
            lbRebate.Size = new System.Drawing.Size(70, 14);
            lbRebate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbRebate.TabIndex = 18;
            lbRebate.Text = "优惠编码:";
            lbRebate.Visible = false;
            // 
            // lbMCardNO
            // 
            lbMCardNO.AutoSize = true;
            lbMCardNO.Font = new System.Drawing.Font("宋体", 10F);
            lbMCardNO.Location = new System.Drawing.Point(193, 73);
            lbMCardNO.Name = "lbMCardNO";
            lbMCardNO.Size = new System.Drawing.Size(70, 14);
            lbMCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbMCardNO.TabIndex = 16;
            lbMCardNO.Text = "医疗证号:";
            // 
            // lbPact
            // 
            lbPact.AutoSize = true;
            lbPact.Font = new System.Drawing.Font("宋体", 10F);
            lbPact.Location = new System.Drawing.Point(386, 44);
            lbPact.Name = "lbPact";
            lbPact.Size = new System.Drawing.Size(70, 14);
            lbPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbPact.TabIndex = 12;
            lbPact.Text = "合同单位:";
            // 
            // lbDoct
            // 
            lbDoct.AutoSize = true;
            lbDoct.Font = new System.Drawing.Font("宋体", 10F);
            lbDoct.Location = new System.Drawing.Point(193, 44);
            lbDoct.Name = "lbDoct";
            lbDoct.Size = new System.Drawing.Size(70, 14);
            lbDoct.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbDoct.TabIndex = 10;
            lbDoct.Text = "开立医生:";
            // 
            // trvPatient
            // 
            this.trvPatient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvPatient.Location = new System.Drawing.Point(0, 70);
            this.trvPatient.Name = "trvPatient";
            this.trvPatient.Size = new System.Drawing.Size(267, 428);
            this.trvPatient.TabIndex = 0;
            this.trvPatient.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvPatient_AfterSelect);
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.trvPatient);
            this.pnlLeft.Controls.Add(this.pnlTime);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(267, 498);
            this.pnlLeft.TabIndex = 1;
            // 
            // pnlTime
            // 
            this.pnlTime.Controls.Add(this.label2);
            this.pnlTime.Controls.Add(this.neuTextBox1);
            this.pnlTime.Controls.Add(this.label1);
            this.pnlTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTime.Location = new System.Drawing.Point(0, 0);
            this.pnlTime.Name = "pnlTime";
            this.pnlTime.Size = new System.Drawing.Size(267, 70);
            this.pnlTime.TabIndex = 3;
            // 
            // pnlPatient
            // 
            this.pnlPatient.Controls.Add(this.txtDoct);
            this.pnlPatient.Controls.Add(this.tbName);
            this.pnlPatient.Controls.Add(this.tbMCardNO);
            this.pnlPatient.Controls.Add(this.txtMarkNo);
            this.pnlPatient.Controls.Add(this.txtRegDept);
            this.pnlPatient.Controls.Add(this.tbCardNO);
            this.pnlPatient.Controls.Add(this.txtSex);
            this.pnlPatient.Controls.Add(this.tbAge);
            this.pnlPatient.Controls.Add(this.txtRebate);
            this.pnlPatient.Controls.Add(this.txtPact);
            this.pnlPatient.Controls.Add(this.lbCardNO);
            this.pnlPatient.Controls.Add(lbRebate);
            this.pnlPatient.Controls.Add(this.lbName);
            this.pnlPatient.Controls.Add(lbMCardNO);
            this.pnlPatient.Controls.Add(this.lbSex);
            this.pnlPatient.Controls.Add(this.lblMarkNo);
            this.pnlPatient.Controls.Add(this.lbAge);
            this.pnlPatient.Controls.Add(lbPact);
            this.pnlPatient.Controls.Add(this.lbRegDept);
            this.pnlPatient.Controls.Add(lbDoct);
            this.pnlPatient.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPatient.Location = new System.Drawing.Point(267, 0);
            this.pnlPatient.Name = "pnlPatient";
            this.pnlPatient.Size = new System.Drawing.Size(614, 102);
            this.pnlPatient.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.pnlPatient.TabIndex = 2;
            // 
            // txtDoct
            // 
            this.txtDoct.Enabled = false;
            this.txtDoct.Font = new System.Drawing.Font("宋体", 10F);
            this.txtDoct.IsEnter2Tab = false;
            this.txtDoct.Location = new System.Drawing.Point(263, 39);
            this.txtDoct.Name = "txtDoct";
            this.txtDoct.Size = new System.Drawing.Size(111, 23);
            this.txtDoct.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtDoct.TabIndex = 3;
            // 
            // tbName
            // 
            this.tbName.Enabled = false;
            this.tbName.Font = new System.Drawing.Font("宋体", 10F);
            this.tbName.IsEnter2Tab = false;
            this.tbName.Location = new System.Drawing.Point(263, 10);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(111, 23);
            this.tbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbName.TabIndex = 3;
            // 
            // tbMCardNO
            // 
            this.tbMCardNO.Enabled = false;
            this.tbMCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.tbMCardNO.IsEnter2Tab = false;
            this.tbMCardNO.Location = new System.Drawing.Point(263, 68);
            this.tbMCardNO.Name = "tbMCardNO";
            this.tbMCardNO.Size = new System.Drawing.Size(111, 23);
            this.tbMCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbMCardNO.TabIndex = 17;
            // 
            // txtMarkNo
            // 
            this.txtMarkNo.Font = new System.Drawing.Font("宋体", 10F);
            this.txtMarkNo.IsEnter2Tab = false;
            this.txtMarkNo.Location = new System.Drawing.Point(74, 68);
            this.txtMarkNo.Name = "txtMarkNo";
            this.txtMarkNo.Size = new System.Drawing.Size(111, 23);
            this.txtMarkNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtMarkNo.TabIndex = 1;
            // 
            // txtRegDept
            // 
            this.txtRegDept.Font = new System.Drawing.Font("宋体", 10F);
            this.txtRegDept.IsEnter2Tab = false;
            this.txtRegDept.Location = new System.Drawing.Point(74, 39);
            this.txtRegDept.Name = "txtRegDept";
            this.txtRegDept.Size = new System.Drawing.Size(111, 23);
            this.txtRegDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtRegDept.TabIndex = 1;
            // 
            // tbCardNO
            // 
            this.tbCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.tbCardNO.IsEnter2Tab = false;
            this.tbCardNO.Location = new System.Drawing.Point(74, 10);
            this.tbCardNO.Name = "tbCardNO";
            this.tbCardNO.Size = new System.Drawing.Size(111, 23);
            this.tbCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCardNO.TabIndex = 1;
            // 
            // txtSex
            // 
            this.txtSex.Enabled = false;
            this.txtSex.Font = new System.Drawing.Font("宋体", 10F);
            this.txtSex.IsEnter2Tab = false;
            this.txtSex.Location = new System.Drawing.Point(458, 10);
            this.txtSex.Name = "txtSex";
            this.txtSex.Size = new System.Drawing.Size(40, 23);
            this.txtSex.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtSex.TabIndex = 7;
            // 
            // tbAge
            // 
            this.tbAge.Enabled = false;
            this.tbAge.Font = new System.Drawing.Font("宋体", 10F);
            this.tbAge.IsEnter2Tab = false;
            this.tbAge.Location = new System.Drawing.Point(536, 10);
            this.tbAge.Name = "tbAge";
            this.tbAge.Size = new System.Drawing.Size(31, 23);
            this.tbAge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbAge.TabIndex = 7;
            // 
            // txtRebate
            // 
            this.txtRebate.Enabled = false;
            this.txtRebate.Font = new System.Drawing.Font("宋体", 10F);
            this.txtRebate.IsEnter2Tab = false;
            this.txtRebate.Location = new System.Drawing.Point(458, 68);
            this.txtRebate.Name = "txtRebate";
            this.txtRebate.Size = new System.Drawing.Size(111, 23);
            this.txtRebate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtRebate.TabIndex = 3;
            // 
            // txtPact
            // 
            this.txtPact.Enabled = false;
            this.txtPact.Font = new System.Drawing.Font("宋体", 10F);
            this.txtPact.IsEnter2Tab = false;
            this.txtPact.Location = new System.Drawing.Point(458, 39);
            this.txtPact.Name = "txtPact";
            this.txtPact.Size = new System.Drawing.Size(111, 23);
            this.txtPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtPact.TabIndex = 3;
            // 
            // lbCardNO
            // 
            this.lbCardNO.AutoSize = true;
            this.lbCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.lbCardNO.Location = new System.Drawing.Point(7, 14);
            this.lbCardNO.Name = "lbCardNO";
            this.lbCardNO.Size = new System.Drawing.Size(70, 14);
            this.lbCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbCardNO.TabIndex = 0;
            this.lbCardNO.Text = "病 例 号:";
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("宋体", 10F);
            this.lbName.Location = new System.Drawing.Point(193, 14);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(70, 14);
            this.lbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbName.TabIndex = 2;
            this.lbName.Text = "患者姓名:";
            // 
            // lbSex
            // 
            this.lbSex.AutoSize = true;
            this.lbSex.Font = new System.Drawing.Font("宋体", 10F);
            this.lbSex.Location = new System.Drawing.Point(414, 14);
            this.lbSex.Name = "lbSex";
            this.lbSex.Size = new System.Drawing.Size(42, 14);
            this.lbSex.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbSex.TabIndex = 4;
            this.lbSex.Text = "性别:";
            // 
            // lblMarkNo
            // 
            this.lblMarkNo.AutoSize = true;
            this.lblMarkNo.Font = new System.Drawing.Font("宋体", 10F);
            this.lblMarkNo.Location = new System.Drawing.Point(7, 73);
            this.lblMarkNo.Name = "lblMarkNo";
            this.lblMarkNo.Size = new System.Drawing.Size(70, 14);
            this.lblMarkNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblMarkNo.TabIndex = 14;
            this.lblMarkNo.Text = "就诊卡号:";
            // 
            // lbAge
            // 
            this.lbAge.AutoSize = true;
            this.lbAge.Font = new System.Drawing.Font("宋体", 10F);
            this.lbAge.Location = new System.Drawing.Point(498, 14);
            this.lbAge.Name = "lbAge";
            this.lbAge.Size = new System.Drawing.Size(42, 14);
            this.lbAge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbAge.TabIndex = 6;
            this.lbAge.Text = "年龄:";
            // 
            // lbRegDept
            // 
            this.lbRegDept.AutoSize = true;
            this.lbRegDept.Font = new System.Drawing.Font("宋体", 10F);
            this.lbRegDept.Location = new System.Drawing.Point(7, 44);
            this.lbRegDept.Name = "lbRegDept";
            this.lbRegDept.Size = new System.Drawing.Size(70, 14);
            this.lbRegDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbRegDept.TabIndex = 8;
            this.lbRegDept.Text = "看诊科室:";
            // 
            // pnlItem
            // 
            this.pnlItem.Controls.Add(this.fpSpread1);
            this.pnlItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlItem.Location = new System.Drawing.Point(267, 102);
            this.pnlItem.Name = "pnlItem";
            this.pnlItem.Size = new System.Drawing.Size(614, 396);
            this.pnlItem.TabIndex = 3;
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1, Row 0, Column 0, ";
            this.fpSpread1.BackColor = System.Drawing.Color.White;
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.FileName = "";
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.IsAutoSaveGridStatus = false;
            this.fpSpread1.IsCanCustomConfigColumn = false;
            this.fpSpread1.Location = new System.Drawing.Point(0, 0);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(614, 396);
            this.fpSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpSpread1.TabIndex = 5;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread1.TextTipAppearance = tipAppearance1;
            this.fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 7;
            this.fpSpread1_Sheet1.RowCount = 5;
            this.fpSpread1_Sheet1.Cells.Get(0, 0).Font = new System.Drawing.Font("宋体", 10.5F);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "项目名称";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "规格";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "总量";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "总金额";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "执行科室";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "状态";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "上传";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.fpSpread1_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "项目名称";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 250F;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "规格";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 150F;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "总量";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 80F;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "总金额";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 80F;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "执行科室";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 130F;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "状态";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 80F;
            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.Font = new System.Drawing.Font("宋体", 15.75F);
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "RowHeaderDefault";
            this.fpSpread1_Sheet1.Rows.Default.Height = 28F;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(15, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(239, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "(注意：可支持多个，流水号中间以\'|\'隔开)";
            // 
            // neuTextBox1
            // 
            this.neuTextBox1.Font = new System.Drawing.Font("宋体", 10F);
            this.neuTextBox1.IsEnter2Tab = false;
            this.neuTextBox1.Location = new System.Drawing.Point(85, 15);
            this.neuTextBox1.Name = "neuTextBox1";
            this.neuTextBox1.Size = new System.Drawing.Size(163, 23);
            this.neuTextBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuTextBox1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "门诊流水号：";
            // 
            // ucChaoYangDataDeal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlItem);
            this.Controls.Add(this.pnlPatient);
            this.Controls.Add(this.pnlLeft);
            this.Name = "ucChaoYangDataDeal";
            this.Size = new System.Drawing.Size(881, 498);
            this.Load += new System.EventHandler(this.ucPactUpdateLoad_Load);
            this.pnlLeft.ResumeLayout(false);
            this.pnlTime.ResumeLayout(false);
            this.pnlTime.PerformLayout();
            this.pnlPatient.ResumeLayout(false);
            this.pnlPatient.PerformLayout();
            this.pnlItem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView trvPatient;
        private System.Windows.Forms.Panel pnlLeft;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel pnlPatient;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtSex;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbAge;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbName;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbCardNO;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtMarkNo;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCardNO;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbMCardNO;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbName;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbSex;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblMarkNo;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbAge;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbRegDept;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtRegDept;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtRebate;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtPact;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtDoct;
        private System.Windows.Forms.Panel pnlItem;
        public Neusoft.FrameWork.WinForms.Controls.NeuSpread fpSpread1;
        public FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Panel pnlTime;
        private System.Windows.Forms.Label label2;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox neuTextBox1;
        private System.Windows.Forms.Label label1;
    }
}
