namespace Neusoft.HISFC.Components.InpatientFee.Balance
{
    partial class frmBalancePay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.ComboBoxCellType comboBoxCellType1 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.ComboBoxCellType comboBoxCellType2 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.gbBalanceInfo = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.txtPay = new Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox();
            this.lbReturnCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtCharge = new Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox();
            this.lbTransPrepayCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtShouldPay = new Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox();
            this.lblShouldPay = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.fpPayType = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.fpPayType_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.btnOK = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnCancel = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnReCompute = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.gbBalanceInfo.SuspendLayout();
            this.neuGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpPayType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpPayType_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // gbBalanceInfo
            // 
            this.gbBalanceInfo.Controls.Add(this.txtPay);
            this.gbBalanceInfo.Controls.Add(this.lbReturnCost);
            this.gbBalanceInfo.Controls.Add(this.txtCharge);
            this.gbBalanceInfo.Controls.Add(this.lbTransPrepayCost);
            this.gbBalanceInfo.Controls.Add(this.txtShouldPay);
            this.gbBalanceInfo.Controls.Add(this.lblShouldPay);
            this.gbBalanceInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbBalanceInfo.Font = new System.Drawing.Font("宋体", 1F);
            this.gbBalanceInfo.Location = new System.Drawing.Point(0, 0);
            this.gbBalanceInfo.Name = "gbBalanceInfo";
            this.gbBalanceInfo.Size = new System.Drawing.Size(622, 39);
            this.gbBalanceInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gbBalanceInfo.TabIndex = 3;
            this.gbBalanceInfo.TabStop = false;
            // 
            // txtPay
            // 
            this.txtPay.AllowNegative = false;
            this.txtPay.BackColor = System.Drawing.Color.White;
            this.txtPay.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPay.IsAutoRemoveDecimalZero = false;
            this.txtPay.IsEnter2Tab = false;
            this.txtPay.Location = new System.Drawing.Point(262, 8);
            this.txtPay.Name = "txtPay";
            this.txtPay.NumericPrecision = 10;
            this.txtPay.NumericScaleOnFocus = 2;
            this.txtPay.NumericScaleOnLostFocus = 2;
            this.txtPay.NumericValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtPay.SetRange = new System.Drawing.Size(-1, -1);
            this.txtPay.Size = new System.Drawing.Size(119, 23);
            this.txtPay.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtPay.TabIndex = 1;
            this.txtPay.Text = "0.00";
            this.txtPay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPay.UseGroupSeperator = true;
            this.txtPay.ZeroIsValid = true;
            // 
            // lbReturnCost
            // 
            this.lbReturnCost.AutoSize = true;
            this.lbReturnCost.Font = new System.Drawing.Font("宋体", 9F);
            this.lbReturnCost.Location = new System.Drawing.Point(229, 13);
            this.lbReturnCost.Name = "lbReturnCost";
            this.lbReturnCost.Size = new System.Drawing.Size(29, 12);
            this.lbReturnCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbReturnCost.TabIndex = 101;
            this.lbReturnCost.Text = "实收";
            // 
            // txtCharge
            // 
            this.txtCharge.AllowNegative = false;
            this.txtCharge.BackColor = System.Drawing.Color.White;
            this.txtCharge.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCharge.IsAutoRemoveDecimalZero = false;
            this.txtCharge.IsEnter2Tab = false;
            this.txtCharge.Location = new System.Drawing.Point(486, 8);
            this.txtCharge.Name = "txtCharge";
            this.txtCharge.NumericPrecision = 10;
            this.txtCharge.NumericScaleOnFocus = 2;
            this.txtCharge.NumericScaleOnLostFocus = 2;
            this.txtCharge.NumericValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtCharge.ReadOnly = true;
            this.txtCharge.SetRange = new System.Drawing.Size(-1, -1);
            this.txtCharge.Size = new System.Drawing.Size(90, 23);
            this.txtCharge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtCharge.TabIndex = 2;
            this.txtCharge.Text = "0.00";
            this.txtCharge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCharge.UseGroupSeperator = true;
            this.txtCharge.ZeroIsValid = true;
            // 
            // lbTransPrepayCost
            // 
            this.lbTransPrepayCost.AutoSize = true;
            this.lbTransPrepayCost.Font = new System.Drawing.Font("宋体", 9F);
            this.lbTransPrepayCost.Location = new System.Drawing.Point(427, 13);
            this.lbTransPrepayCost.Name = "lbTransPrepayCost";
            this.lbTransPrepayCost.Size = new System.Drawing.Size(53, 12);
            this.lbTransPrepayCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbTransPrepayCost.TabIndex = 16;
            this.lbTransPrepayCost.Text = "找零金额";
            // 
            // txtShouldPay
            // 
            this.txtShouldPay.AllowNegative = false;
            this.txtShouldPay.BackColor = System.Drawing.Color.White;
            this.txtShouldPay.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtShouldPay.ForeColor = System.Drawing.Color.Red;
            this.txtShouldPay.IsAutoRemoveDecimalZero = false;
            this.txtShouldPay.IsEnter2Tab = false;
            this.txtShouldPay.Location = new System.Drawing.Point(73, 8);
            this.txtShouldPay.Name = "txtShouldPay";
            this.txtShouldPay.NumericPrecision = 10;
            this.txtShouldPay.NumericScaleOnFocus = 2;
            this.txtShouldPay.NumericScaleOnLostFocus = 2;
            this.txtShouldPay.NumericValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtShouldPay.ReadOnly = true;
            this.txtShouldPay.SetRange = new System.Drawing.Size(-1, -1);
            this.txtShouldPay.Size = new System.Drawing.Size(119, 23);
            this.txtShouldPay.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtShouldPay.TabIndex = 0;
            this.txtShouldPay.TabStop = false;
            this.txtShouldPay.Text = "0.00";
            this.txtShouldPay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtShouldPay.UseGroupSeperator = true;
            this.txtShouldPay.ZeroIsValid = true;
            // 
            // lblShouldPay
            // 
            this.lblShouldPay.AutoSize = true;
            this.lblShouldPay.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.lblShouldPay.ForeColor = System.Drawing.Color.Red;
            this.lblShouldPay.Location = new System.Drawing.Point(4, 13);
            this.lblShouldPay.Name = "lblShouldPay";
            this.lblShouldPay.Size = new System.Drawing.Size(71, 12);
            this.lblShouldPay.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblShouldPay.TabIndex = 14;
            this.lblShouldPay.Text = "应收(自费)";
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.fpPayType);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox1.Font = new System.Drawing.Font("宋体", 1F);
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 39);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(622, 186);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 4;
            this.neuGroupBox1.TabStop = false;
            // 
            // fpPayType
            // 
            this.fpPayType.About = "3.0.2004.2005";
            this.fpPayType.AccessibleDescription = "fpCost, Sheet1, Row 0, Column 0, ";
            this.fpPayType.BackColor = System.Drawing.Color.White;
            this.fpPayType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpPayType.EditModeReplace = true;
            this.fpPayType.FileName = "";
            this.fpPayType.Font = new System.Drawing.Font("宋体", 9F);
            this.fpPayType.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpPayType.IsAutoSaveGridStatus = false;
            this.fpPayType.IsCanCustomConfigColumn = false;
            this.fpPayType.Location = new System.Drawing.Point(3, 5);
            this.fpPayType.Name = "fpPayType";
            this.fpPayType.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpPayType.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpPayType_Sheet1});
            this.fpPayType.Size = new System.Drawing.Size(616, 178);
            this.fpPayType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpPayType.TabIndex = 99;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpPayType.TextTipAppearance = tipAppearance1;
            this.fpPayType.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpPayType_Sheet1
            // 
            this.fpPayType_Sheet1.Reset();
            this.fpPayType_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpPayType_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpPayType_Sheet1.ColumnCount = 6;
            this.fpPayType_Sheet1.RowCount = 5;
            this.fpPayType_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpPayType_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "支付方式";
            this.fpPayType_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "支付金额";
            this.fpPayType_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "开户银行";
            this.fpPayType_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "账号";
            this.fpPayType_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "开据单位";
            this.fpPayType_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "支票/交易流水号";
            this.fpPayType_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpPayType_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpPayType_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            comboBoxCellType1.ButtonAlign = FarPoint.Win.ButtonAlign.Right;
            this.fpPayType_Sheet1.Columns.Get(0).CellType = comboBoxCellType1;
            this.fpPayType_Sheet1.Columns.Get(0).Label = "支付方式";
            this.fpPayType_Sheet1.Columns.Get(0).Width = 88F;
            this.fpPayType_Sheet1.Columns.Get(1).CellType = numberCellType1;
            this.fpPayType_Sheet1.Columns.Get(1).Label = "支付金额";
            this.fpPayType_Sheet1.Columns.Get(1).Locked = false;
            this.fpPayType_Sheet1.Columns.Get(1).Width = 73F;
            comboBoxCellType2.ButtonAlign = FarPoint.Win.ButtonAlign.Right;
            this.fpPayType_Sheet1.Columns.Get(2).CellType = comboBoxCellType2;
            this.fpPayType_Sheet1.Columns.Get(2).Label = "开户银行";
            this.fpPayType_Sheet1.Columns.Get(2).Locked = false;
            this.fpPayType_Sheet1.Columns.Get(2).Width = 101F;
            this.fpPayType_Sheet1.Columns.Get(3).Label = "账号";
            this.fpPayType_Sheet1.Columns.Get(3).Locked = false;
            this.fpPayType_Sheet1.Columns.Get(3).Width = 99F;
            this.fpPayType_Sheet1.Columns.Get(4).Label = "开据单位";
            this.fpPayType_Sheet1.Columns.Get(4).Locked = false;
            this.fpPayType_Sheet1.Columns.Get(4).Width = 93F;
            this.fpPayType_Sheet1.Columns.Get(5).CellType = textCellType1;
            this.fpPayType_Sheet1.Columns.Get(5).Label = "支票/交易流水号";
            this.fpPayType_Sheet1.Columns.Get(5).Locked = false;
            this.fpPayType_Sheet1.Columns.Get(5).Width = 114F;
            this.fpPayType_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
            this.fpPayType_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.fpPayType_Sheet1.RowHeader.Columns.Get(0).Width = 37F;
            this.fpPayType_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpPayType_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpPayType_Sheet1.Rows.Default.Height = 25F;
            this.fpPayType_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpPayType_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.fpPayType_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpPayType_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("宋体", 11F);
            this.btnOK.Location = new System.Drawing.Point(418, 227);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 32);
            this.btnOK.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.OK;
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("宋体", 11F);
            this.btnCancel.Location = new System.Drawing.Point(521, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 32);
            this.btnCancel.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.Cancel;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnReCompute
            // 
            this.btnReCompute.Font = new System.Drawing.Font("宋体", 11F);
            this.btnReCompute.Location = new System.Drawing.Point(16, 227);
            this.btnReCompute.Name = "btnReCompute";
            this.btnReCompute.Size = new System.Drawing.Size(87, 32);
            this.btnReCompute.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnReCompute.TabIndex = 7;
            this.btnReCompute.Text = "重新计算";
            this.btnReCompute.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnReCompute.UseVisualStyleBackColor = true;
            // 
            // frmBalancePay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(247)))), ((int)(((byte)(213)))));
            this.ClientSize = new System.Drawing.Size(622, 262);
            this.Controls.Add(this.btnReCompute);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.neuGroupBox1);
            this.Controls.Add(this.gbBalanceInfo);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBalancePay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "结算支付方式";
            this.gbBalanceInfo.ResumeLayout(false);
            this.gbBalanceInfo.PerformLayout();
            this.neuGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpPayType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpPayType_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gbBalanceInfo;
        protected Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox txtPay;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbReturnCost;
        protected Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox txtCharge;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbTransPrepayCost;
        protected Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox txtShouldPay;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblShouldPay;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        protected FarPoint.Win.Spread.SheetView fpPayType_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnOK;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnCancel;
        protected Neusoft.FrameWork.WinForms.Controls.NeuSpread fpPayType;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnReCompute;
    }
}