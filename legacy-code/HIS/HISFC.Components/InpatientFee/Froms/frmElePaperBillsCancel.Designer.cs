namespace Neusoft.HISFC.Components.InpatientFee.Forms
{
    partial class frmElePaperBillsCancel
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Colcheckbox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.业务流水号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.电子票据代码 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.电子票据号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.纸质票据代码 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.纸质票据号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.住院流水号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.发票号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.状态 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtInvoiceNo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1098, 40);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "发票号:";
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(3, 6);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 1;
            this.btnQuery.Text = "刷新";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(66, 10);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(210, 21);
            this.txtInvoiceNo.TabIndex = 2;
            this.txtInvoiceNo.TextChanged += new System.EventHandler(this.txtInvoiceNo_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnQuery);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 516);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1098, 60);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 40);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1098, 476);
            this.panel3.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Colcheckbox,
            this.业务流水号,
            this.电子票据代码,
            this.电子票据号,
            this.纸质票据代码,
            this.纸质票据号,
            this.住院流水号,
            this.发票号,
            this.状态});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1098, 476);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Location = new System.Drawing.Point(978, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 42);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "作废选中纸质票";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Colcheckbox
            // 
            this.Colcheckbox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Colcheckbox.FillWeight = 20F;
            this.Colcheckbox.HeaderText = "";
            this.Colcheckbox.Name = "Colcheckbox";
            this.Colcheckbox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Colcheckbox.Width = 26;
            // 
            // 业务流水号
            // 
            this.业务流水号.DataPropertyName = "业务流水号";
            this.业务流水号.HeaderText = "业务流水号";
            this.业务流水号.Name = "业务流水号";
            this.业务流水号.ReadOnly = true;
            // 
            // 电子票据代码
            // 
            this.电子票据代码.DataPropertyName = "电子票据代码";
            this.电子票据代码.HeaderText = "电子票据代码";
            this.电子票据代码.Name = "电子票据代码";
            this.电子票据代码.ReadOnly = true;
            // 
            // 电子票据号
            // 
            this.电子票据号.DataPropertyName = "电子票据号";
            this.电子票据号.HeaderText = "电子票据号";
            this.电子票据号.Name = "电子票据号";
            this.电子票据号.ReadOnly = true;
            // 
            // 纸质票据代码
            // 
            this.纸质票据代码.DataPropertyName = "纸质票据代码";
            this.纸质票据代码.HeaderText = "纸质票据代码";
            this.纸质票据代码.Name = "纸质票据代码";
            this.纸质票据代码.ReadOnly = true;
            // 
            // 纸质票据号
            // 
            this.纸质票据号.DataPropertyName = "纸质票据号";
            this.纸质票据号.HeaderText = "纸质票据号";
            this.纸质票据号.Name = "纸质票据号";
            this.纸质票据号.ReadOnly = true;
            // 
            // 住院流水号
            // 
            this.住院流水号.DataPropertyName = "住院流水号";
            this.住院流水号.HeaderText = "住院流水号";
            this.住院流水号.Name = "住院流水号";
            this.住院流水号.ReadOnly = true;
            // 
            // 发票号
            // 
            this.发票号.DataPropertyName = "发票号";
            this.发票号.HeaderText = "发票号";
            this.发票号.Name = "发票号";
            this.发票号.ReadOnly = true;
            // 
            // 状态
            // 
            this.状态.DataPropertyName = "状态";
            this.状态.HeaderText = "状态";
            this.状态.Name = "状态";
            this.状态.ReadOnly = true;
            // 
            // frmElePaperBillsCancel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 576);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmElePaperBillsCancel";
            this.Text = "frmElePaperBillsCancel";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Colcheckbox;
        private System.Windows.Forms.DataGridViewTextBoxColumn 业务流水号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 电子票据代码;
        private System.Windows.Forms.DataGridViewTextBoxColumn 电子票据号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 纸质票据代码;
        private System.Windows.Forms.DataGridViewTextBoxColumn 纸质票据号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 住院流水号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 发票号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 状态;
    }
}