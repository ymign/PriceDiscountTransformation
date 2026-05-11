namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    partial class ucPanelStatus
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
            this.panelBig = new System.Windows.Forms.Panel();
            this.panelSmall = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblText = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.panelBig.SuspendLayout();
            this.panelSmall.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBig
            // 
            this.panelBig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelBig.Controls.Add(this.panelSmall);
            this.panelBig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBig.Location = new System.Drawing.Point(0, 0);
            this.panelBig.Name = "panelBig";
            this.panelBig.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.panelBig.Size = new System.Drawing.Size(242, 70);
            this.panelBig.TabIndex = 0;
            // 
            // panelSmall
            // 
            this.panelSmall.BackColor = System.Drawing.Color.White;
            this.panelSmall.Controls.Add(this.tableLayoutPanel1);
            this.panelSmall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSmall.Location = new System.Drawing.Point(0, 3);
            this.panelSmall.Margin = new System.Windows.Forms.Padding(6);
            this.panelSmall.Name = "panelSmall";
            this.panelSmall.Size = new System.Drawing.Size(242, 67);
            this.panelSmall.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lblText, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblValue, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(242, 67);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblText
            // 
            this.lblText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblText.Font = new System.Drawing.Font("宋体", 12F);
            this.lblText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblText.Location = new System.Drawing.Point(3, 33);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(236, 34);
            this.lblText.TabIndex = 4;
            this.lblText.Text = "总包装数";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblValue
            // 
            this.lblValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblValue.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.lblValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(164)))), ((int)(((byte)(164)))));
            this.lblValue.Location = new System.Drawing.Point(3, 0);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(236, 33);
            this.lblValue.TabIndex = 3;
            this.lblValue.Text = "4";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucPanelStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelBig);
            this.Name = "ucPanelStatus";
            this.Size = new System.Drawing.Size(242, 70);
            this.panelBig.ResumeLayout(false);
            this.panelSmall.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBig;
        private System.Windows.Forms.Panel panelSmall;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label lblText;
    }
}
