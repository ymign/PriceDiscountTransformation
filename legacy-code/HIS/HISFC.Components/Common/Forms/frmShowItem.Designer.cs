namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmShowItem
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。

        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Panel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.outflag = new System.Windows.Forms.CheckBox();
            this.cbxIsReal = new System.Windows.Forms.CheckBox();
            this.cmbDrugDept = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.chbWB = new System.Windows.Forms.CheckBox();
            this.ckbSpell = new System.Windows.Forms.CheckBox();
            this.chkDeptItem = new System.Windows.Forms.CheckBox();
            this.lklb_exit = new System.Windows.Forms.LinkLabel();
            this.lblSet = new Neusoft.FrameWork.WinForms.Controls.NeuLinkLabel();
            this.lblTip = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.pnlBottom = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusbarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lnkMore = new Neusoft.FrameWork.WinForms.Controls.NeuLinkLabel();
            this.PanelMain = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.pnMain = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.Panel1.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pnMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.Honeydew;
            this.Panel1.Controls.Add(this.outflag);
            this.Panel1.Controls.Add(this.cbxIsReal);
            this.Panel1.Controls.Add(this.cmbDrugDept);
            this.Panel1.Controls.Add(this.chbWB);
            this.Panel1.Controls.Add(this.ckbSpell);
            this.Panel1.Controls.Add(this.chkDeptItem);
            this.Panel1.Controls.Add(this.lklb_exit);
            this.Panel1.Controls.Add(this.lblSet);
            this.Panel1.Controls.Add(this.lblTip);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(703, 40);
            this.Panel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.Panel1.TabIndex = 0;
            this.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            // 
            // outflag
            // 
            this.outflag.AutoSize = true;
            this.outflag.Enabled = false;
            this.outflag.Location = new System.Drawing.Point(72, 11);
            this.outflag.Name = "outflag";
            this.outflag.Size = new System.Drawing.Size(72, 16);
            this.outflag.TabIndex = 11;
            this.outflag.Text = "外延药品";
            this.outflag.UseVisualStyleBackColor = true;
            this.outflag.CheckedChanged += new System.EventHandler(this.outflag_CheckedChanged);
            // 
            // cbxIsReal
            // 
            this.cbxIsReal.AutoSize = true;
            this.cbxIsReal.Checked = true;
            this.cbxIsReal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxIsReal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.cbxIsReal.Location = new System.Drawing.Point(401, 11);
            this.cbxIsReal.Name = "cbxIsReal";
            this.cbxIsReal.Size = new System.Drawing.Size(72, 16);
            this.cbxIsReal.TabIndex = 10;
            this.cbxIsReal.Text = "模糊查询";
            this.cbxIsReal.UseVisualStyleBackColor = true;
            this.cbxIsReal.CheckedChanged += new System.EventHandler(this.cbxIsReal_CheckedChanged);
            // 
            // cmbDrugDept
            // 
            this.cmbDrugDept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDrugDept.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbDrugDept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbDrugDept.FormattingEnabled = true;
            this.cmbDrugDept.IsEnter2Tab = false;
            this.cmbDrugDept.IsFlat = false;
            this.cmbDrugDept.IsLike = true;
            this.cmbDrugDept.IsListOnly = true;
            this.cmbDrugDept.IsPopForm = true;
            this.cmbDrugDept.IsShowCustomerList = false;
            this.cmbDrugDept.IsShowID = false;
            this.cmbDrugDept.IsShowIDAndName = false;
            this.cmbDrugDept.Location = new System.Drawing.Point(483, 9);
            this.cmbDrugDept.Name = "cmbDrugDept";
            this.cmbDrugDept.ShowCustomerList = false;
            this.cmbDrugDept.ShowID = false;
            this.cmbDrugDept.Size = new System.Drawing.Size(108, 20);
            this.cmbDrugDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDrugDept.TabIndex = 5;
            this.cmbDrugDept.Tag = "";
            this.cmbDrugDept.ToolBarUse = false;
            // 
            // chbWB
            // 
            this.chbWB.AutoSize = true;
            this.chbWB.Location = new System.Drawing.Point(225, 11);
            this.chbWB.Name = "chbWB";
            this.chbWB.Size = new System.Drawing.Size(60, 16);
            this.chbWB.TabIndex = 9;
            this.chbWB.Text = "五笔码";
            this.chbWB.UseVisualStyleBackColor = true;
            // 
            // ckbSpell
            // 
            this.ckbSpell.AutoSize = true;
            this.ckbSpell.Checked = true;
            this.ckbSpell.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbSpell.Location = new System.Drawing.Point(150, 11);
            this.ckbSpell.Name = "ckbSpell";
            this.ckbSpell.Size = new System.Drawing.Size(60, 16);
            this.ckbSpell.TabIndex = 8;
            this.ckbSpell.Text = "拼音码";
            this.ckbSpell.UseVisualStyleBackColor = true;
            // 
            // chkDeptItem
            // 
            this.chkDeptItem.AutoSize = true;
            this.chkDeptItem.Checked = true;
            this.chkDeptItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeptItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDeptItem.ForeColor = System.Drawing.Color.DarkBlue;
            this.chkDeptItem.Location = new System.Drawing.Point(300, 9);
            this.chkDeptItem.Name = "chkDeptItem";
            this.chkDeptItem.Size = new System.Drawing.Size(86, 19);
            this.chkDeptItem.TabIndex = 7;
            this.chkDeptItem.Text = "科常用项目";
            this.chkDeptItem.UseVisualStyleBackColor = true;
            this.chkDeptItem.Click += new System.EventHandler(this.chkDeptItem_Click);
            this.chkDeptItem.CheckedChanged += new System.EventHandler(this.chkDeptItem_CheckedChanged);
            // 
            // lklb_exit
            // 
            this.lklb_exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lklb_exit.AutoSize = true;
            this.lklb_exit.BackColor = System.Drawing.Color.Honeydew;
            this.lklb_exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lklb_exit.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lklb_exit.LinkColor = System.Drawing.Color.Black;
            this.lklb_exit.Location = new System.Drawing.Point(645, 12);
            this.lklb_exit.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lklb_exit.Name = "lklb_exit";
            this.lklb_exit.Size = new System.Drawing.Size(37, 15);
            this.lklb_exit.TabIndex = 4;
            this.lklb_exit.TabStop = true;
            this.lklb_exit.Text = " 退出 ";
            this.lklb_exit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lklb_exit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lklb_exit_LinkClicked);
            // 
            // lblSet
            // 
            this.lblSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSet.AutoSize = true;
            this.lblSet.BackColor = System.Drawing.Color.Honeydew;
            this.lblSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSet.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lblSet.LinkColor = System.Drawing.Color.Black;
            this.lblSet.Location = new System.Drawing.Point(599, 12);
            this.lblSet.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblSet.Name = "lblSet";
            this.lblSet.Size = new System.Drawing.Size(37, 15);
            this.lblSet.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblSet.TabIndex = 3;
            this.lblSet.TabStop = true;
            this.lblSet.Text = " 设置 ";
            this.lblSet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSet.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSet_LinkClicked);
            // 
            // lblTip
            // 
            this.lblTip.AutoSize = true;
            this.lblTip.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblTip.Location = new System.Drawing.Point(3, 5);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(29, 12);
            this.lblTip.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblTip.TabIndex = 0;
            this.lblTip.Text = "提示";
            this.lblTip.Visible = false;
            this.lblTip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.Honeydew;
            this.pnlBottom.Controls.Add(this.statusStrip1);
            this.pnlBottom.Controls.Add(this.lnkMore);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 349);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(703, 120);
            this.pnlBottom.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.pnlBottom.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusbarText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 98);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(703, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Visible = false;
            // 
            // statusbarText
            // 
            this.statusbarText.Name = "statusbarText";
            this.statusbarText.Size = new System.Drawing.Size(152, 17);
            this.statusbarText.Text = "显示的扩展信息供医生参考";
            // 
            // lnkMore
            // 
            this.lnkMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkMore.AutoSize = true;
            this.lnkMore.Location = new System.Drawing.Point(643, 83);
            this.lnkMore.Name = "lnkMore";
            this.lnkMore.Size = new System.Drawing.Size(47, 12);
            this.lnkMore.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lnkMore.TabIndex = 0;
            this.lnkMore.TabStop = true;
            this.lnkMore.Text = "更多...";
            // 
            // PanelMain
            // 
            this.PanelMain.AutoScroll = true;
            this.PanelMain.BackColor = System.Drawing.Color.White;
            this.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelMain.Location = new System.Drawing.Point(0, 40);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(703, 309);
            this.PanelMain.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.PanelMain.TabIndex = 2;
            // 
            // pnMain
            // 
            this.pnMain.Controls.Add(this.PanelMain);
            this.pnMain.Controls.Add(this.Panel1);
            this.pnMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMain.Location = new System.Drawing.Point(0, 0);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(703, 349);
            this.pnMain.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.pnMain.TabIndex = 3;
            // 
            // frmShowItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 469);
            this.ControlBox = false;
            this.Controls.Add(this.pnMain);
            this.Controls.Add(this.pnlBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmShowItem";
            this.Load += new System.EventHandler(this.frmShowItem_Load);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel Panel1;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLinkLabel lblSet;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel pnlBottom;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel PanelMain;
        private Neusoft.FrameWork.WinForms.Controls.NeuLinkLabel lnkMore;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusbarText;
        private System.Windows.Forms.LinkLabel lklb_exit;
        public System.Windows.Forms.CheckBox chkDeptItem;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel pnMain;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblTip;
        public Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDrugDept;
        private System.Windows.Forms.CheckBox chbWB;
        private System.Windows.Forms.CheckBox ckbSpell;
        private System.Windows.Forms.CheckBox cbxIsReal;
        private System.Windows.Forms.CheckBox outflag;
    }
}