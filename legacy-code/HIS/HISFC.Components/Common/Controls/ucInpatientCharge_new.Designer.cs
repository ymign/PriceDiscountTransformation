namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucInpatientCharge_new
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
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType3 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType2 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType3 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType4 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType4 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType5 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType6 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType7 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.fpDetail = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.fpDetail_Sheet = new FarPoint.Win.Spread.SheetView();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.tvGroup = new Neusoft.HISFC.Components.Common.Controls.tvDoctorGroup(this.components);
            this.neuSplitter1 = new Neusoft.FrameWork.WinForms.Controls.NeuSplitter();
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail_Sheet)).BeginInit();
            this.neuPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fpDetail
            // 
            this.fpDetail.About = "3.0.2004.2005";
            this.fpDetail.AccessibleDescription = "fpDetail, Sheet, Row 0, Column 0, ";
            this.fpDetail.BackColor = System.Drawing.SystemColors.Control;
            this.fpDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpDetail.EditModePermanent = true;
            this.fpDetail.EditModeReplace = true;
            this.fpDetail.FileName = "";
            this.fpDetail.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpDetail.IsAutoSaveGridStatus = false;
            this.fpDetail.IsCanCustomConfigColumn = false;
            this.fpDetail.Location = new System.Drawing.Point(248, 0);
            this.fpDetail.Margin = new System.Windows.Forms.Padding(4);
            this.fpDetail.Name = "fpDetail";
            this.fpDetail.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpDetail.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpDetail_Sheet});
            this.fpDetail.Size = new System.Drawing.Size(1137, 744);
            this.fpDetail.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpDetail.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpDetail.TextTipAppearance = tipAppearance1;
            this.fpDetail.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpDetail.EditModeOn += new System.EventHandler(this.fpDetail_EditModeOn);
            this.fpDetail.EditChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpDetail_EditChange);
            this.fpDetail.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpDetail_CellDoubleClick);
            this.fpDetail.DragDrop += new System.Windows.Forms.DragEventHandler(this.fpDetail_DragDrop);
            this.fpDetail.ComboSelChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpDetail_ComboSelChange);
            this.fpDetail.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpDetail_Change);
            // 
            // fpDetail_Sheet
            // 
            this.fpDetail_Sheet.Reset();
            this.fpDetail_Sheet.SheetName = "Sheet";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpDetail_Sheet.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpDetail_Sheet.ColumnCount = 15;
            this.fpDetail_Sheet.RowCount = 2;
            this.fpDetail_Sheet.Cells.Get(1, 6).Value = "合计:";
            this.fpDetail_Sheet.Cells.Get(1, 7).Value = 0;
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 0).Value = "住院号";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 1).Value = "姓名";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 2).Value = "项目名称";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 3).Value = "价格";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 4).Value = "数量";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 5).Value = "付数";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 6).Value = "单位";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 7).Value = "合计金额";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 8).Value = "执行科室";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 9).Value = "ItemObject";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 10).Value = "IsNew";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 11).Value = "IsDeptChange";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 12).Value = "IsDrug";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 13).Value = "条码";
            this.fpDetail_Sheet.ColumnHeader.Cells.Get(0, 14).Value = "SPD条码";
            this.fpDetail_Sheet.Columns.Get(0).CellType = textCellType1;
            this.fpDetail_Sheet.Columns.Get(0).Label = "住院号";
            this.fpDetail_Sheet.Columns.Get(0).Locked = true;
            this.fpDetail_Sheet.Columns.Get(0).Width = 126F;
            this.fpDetail_Sheet.Columns.Get(1).CellType = textCellType2;
            this.fpDetail_Sheet.Columns.Get(1).Label = "姓名";
            this.fpDetail_Sheet.Columns.Get(1).Locked = true;
            this.fpDetail_Sheet.Columns.Get(1).Width = 77F;
            this.fpDetail_Sheet.Columns.Get(2).CellType = textCellType3;
            this.fpDetail_Sheet.Columns.Get(2).Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpDetail_Sheet.Columns.Get(2).Label = "项目名称";
            this.fpDetail_Sheet.Columns.Get(2).Width = 280F;
            numberCellType1.DecimalPlaces = 4;
            this.fpDetail_Sheet.Columns.Get(3).CellType = numberCellType1;
            this.fpDetail_Sheet.Columns.Get(3).Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpDetail_Sheet.Columns.Get(3).Label = "价格";
            this.fpDetail_Sheet.Columns.Get(3).Width = 77F;
            numberCellType2.MaximumValue = 99999.99;
            numberCellType2.MinimumValue = -9999.99;
            this.fpDetail_Sheet.Columns.Get(4).CellType = numberCellType2;
            this.fpDetail_Sheet.Columns.Get(4).Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpDetail_Sheet.Columns.Get(4).Label = "数量";
            this.fpDetail_Sheet.Columns.Get(4).Width = 63F;
            this.fpDetail_Sheet.Columns.Get(5).CellType = numberCellType3;
            this.fpDetail_Sheet.Columns.Get(5).Label = "付数";
            this.fpDetail_Sheet.Columns.Get(5).Width = 33F;
            this.fpDetail_Sheet.Columns.Get(6).CellType = textCellType4;
            this.fpDetail_Sheet.Columns.Get(6).Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpDetail_Sheet.Columns.Get(6).Label = "单位";
            this.fpDetail_Sheet.Columns.Get(6).Width = 50F;
            numberCellType4.ReadOnly = true;
            this.fpDetail_Sheet.Columns.Get(7).CellType = numberCellType4;
            this.fpDetail_Sheet.Columns.Get(7).Font = new System.Drawing.Font("宋体", 12F);
            this.fpDetail_Sheet.Columns.Get(7).Label = "合计金额";
            this.fpDetail_Sheet.Columns.Get(7).Locked = true;
            this.fpDetail_Sheet.Columns.Get(7).Width = 83F;
            this.fpDetail_Sheet.Columns.Get(8).CellType = textCellType5;
            this.fpDetail_Sheet.Columns.Get(8).Label = "执行科室";
            this.fpDetail_Sheet.Columns.Get(8).Width = 110F;
            this.fpDetail_Sheet.Columns.Get(9).Label = "ItemObject";
            this.fpDetail_Sheet.Columns.Get(9).Width = 87F;
            this.fpDetail_Sheet.Columns.Get(11).Label = "IsDeptChange";
            this.fpDetail_Sheet.Columns.Get(11).Width = 85F;
            this.fpDetail_Sheet.Columns.Get(13).CellType = textCellType6;
            this.fpDetail_Sheet.Columns.Get(13).Label = "条码";
            this.fpDetail_Sheet.Columns.Get(13).Width = 100F;
            this.fpDetail_Sheet.Columns.Get(14).CellType = textCellType7;
            this.fpDetail_Sheet.Columns.Get(14).Label = "SPD条码";
            this.fpDetail_Sheet.Columns.Get(14).Width = 150F;
            this.fpDetail_Sheet.OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
            this.fpDetail_Sheet.RowHeader.Columns.Default.Resizable = false;
            this.fpDetail_Sheet.Rows.Get(0).Height = 23F;
            this.fpDetail_Sheet.Rows.Get(1).Height = 23F;
            this.fpDetail_Sheet.Rows.Get(1).Locked = true;
            this.fpDetail_Sheet.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.tvGroup);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.neuPanel1.Location = new System.Drawing.Point(0, 0);
            this.neuPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(244, 744);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 1;
            // 
            // tvGroup
            // 
            this.tvGroup.AllowDrop = true;
            this.tvGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvGroup.HideSelection = false;
            this.tvGroup.ImageIndex = 0;
            this.tvGroup.IsEditGroup = false;
            this.tvGroup.IsShowGroupDetailing = false;
            this.tvGroup.Location = new System.Drawing.Point(0, 0);
            this.tvGroup.Margin = new System.Windows.Forms.Padding(4);
            this.tvGroup.Name = "tvGroup";
            this.tvGroup.SelectedImageIndex = 0;
            this.tvGroup.Size = new System.Drawing.Size(244, 744);
            this.tvGroup.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tvGroup.TabIndex = 0;
            this.tvGroup.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvGroup_NodeMouseDoubleClick);
            // 
            // neuSplitter1
            // 
            this.neuSplitter1.Location = new System.Drawing.Point(244, 0);
            this.neuSplitter1.Margin = new System.Windows.Forms.Padding(4);
            this.neuSplitter1.Name = "neuSplitter1";
            this.neuSplitter1.Size = new System.Drawing.Size(4, 744);
            this.neuSplitter1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSplitter1.TabIndex = 2;
            this.neuSplitter1.TabStop = false;
            // 
            // ucInpatientCharge_new
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fpDetail);
            this.Controls.Add(this.neuSplitter1);
            this.Controls.Add(this.neuPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ucInpatientCharge_new";
            this.Size = new System.Drawing.Size(1385, 744);
            this.Load += new System.EventHandler(this.ucInpatientChargeNew_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail_Sheet)).EndInit();
            this.neuPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuSpread fpDetail;
        private FarPoint.Win.Spread.SheetView fpDetail_Sheet;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuSplitter neuSplitter1;
        private Neusoft.HISFC.Components.Common.Controls.tvDoctorGroup tvGroup;
    }
}
