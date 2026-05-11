namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmChoseItemCommon
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
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fpSublItem = new FarPoint.Win.Spread.FpSpread();
            this.fpSublItem_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbxCheckAll = new System.Windows.Forms.CheckBox();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOk = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSublItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSublItem_Sheet1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fpSublItem);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(593, 303);
            this.panel1.TabIndex = 0;
            // 
            // fpSublItem
            // 
            this.fpSublItem.About = "3.0.2004.2005";
            this.fpSublItem.AccessibleDescription = "fpSublItem, Sheet1, Row 0, Column 0, ";
            this.fpSublItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(247)))), ((int)(((byte)(213)))));
            this.fpSublItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSublItem.Location = new System.Drawing.Point(0, 0);
            this.fpSublItem.Name = "fpSublItem";
            this.fpSublItem.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSublItem.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSublItem_Sheet1});
            this.fpSublItem.Size = new System.Drawing.Size(593, 303);
            this.fpSublItem.TabIndex = 4;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSublItem.TextTipAppearance = tipAppearance1;
            this.fpSublItem.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSublItem_ButtonClicked);
            // 
            // fpSublItem_Sheet1
            // 
            this.fpSublItem_Sheet1.Reset();
            this.fpSublItem_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSublItem_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSublItem_Sheet1.ColumnCount = 8;
            this.fpSublItem_Sheet1.RowCount = 1;
            this.fpSublItem_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "选择";
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "名称";
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "编码";
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "数量";
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "规格";
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "类别";
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "单位";
            this.fpSublItem_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "单价";
            this.fpSublItem_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSublItem_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSublItem_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpSublItem_Sheet1.Columns.Get(0).CellType = checkBoxCellType1;
            this.fpSublItem_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSublItem_Sheet1.Columns.Get(0).Label = "选择";
            this.fpSublItem_Sheet1.Columns.Get(0).Width = 33F;
            this.fpSublItem_Sheet1.Columns.Get(1).Label = "名称";
            this.fpSublItem_Sheet1.Columns.Get(1).Width = 179F;
            this.fpSublItem_Sheet1.Columns.Get(2).Label = "编码";
            this.fpSublItem_Sheet1.Columns.Get(2).Width = 70F;
            this.fpSublItem_Sheet1.Columns.Get(3).Label = "数量";
            this.fpSublItem_Sheet1.Columns.Get(3).Width = 39F;
            this.fpSublItem_Sheet1.Columns.Get(4).Label = "规格";
            this.fpSublItem_Sheet1.Columns.Get(4).Width = 83F;
            this.fpSublItem_Sheet1.Columns.Get(5).Label = "类别";
            this.fpSublItem_Sheet1.Columns.Get(5).Width = 66F;
            this.fpSublItem_Sheet1.Columns.Get(6).Label = "单位";
            this.fpSublItem_Sheet1.Columns.Get(6).Width = 49F;
            this.fpSublItem_Sheet1.Columns.Get(7).Label = "单价";
            this.fpSublItem_Sheet1.Columns.Get(7).Width = 47F;
            this.fpSublItem_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.fpSublItem_Sheet1.RowHeader.Columns.Get(0).Width = 0F;
            this.fpSublItem_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSublItem_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSublItem_Sheet1.Rows.Default.Height = 25F;
            this.fpSublItem_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSublItem_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.fpSublItem_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpSublItem_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cbxCheckAll);
            this.panel2.Controls.Add(this.btCancel);
            this.panel2.Controls.Add(this.btOk);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 303);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(593, 40);
            this.panel2.TabIndex = 1;
            // 
            // cbxCheckAll
            // 
            this.cbxCheckAll.AutoSize = true;
            this.cbxCheckAll.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxCheckAll.ForeColor = System.Drawing.Color.Black;
            this.cbxCheckAll.Location = new System.Drawing.Point(452, 6);
            this.cbxCheckAll.Name = "cbxCheckAll";
            this.cbxCheckAll.Size = new System.Drawing.Size(61, 20);
            this.cbxCheckAll.TabIndex = 2;
            this.cbxCheckAll.Text = "选择";
            this.cbxCheckAll.UseVisualStyleBackColor = true;
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(256, 6);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "取消(&ESC)";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(76, 6);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 0;
            this.btOk.Text = "确定（&S）";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // frmChoseItemCommon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 343);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.KeyPreview = true;
            this.Name = "frmChoseItemCommon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "项目选择";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmChoseItem_KeyDown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSublItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSublItem_Sheet1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOk;
        private FarPoint.Win.Spread.FpSpread fpSublItem;
        private FarPoint.Win.Spread.SheetView fpSublItem_Sheet1;
        private System.Windows.Forms.CheckBox cbxCheckAll;
    }
}