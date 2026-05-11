namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucQuerySeeNoByCardNo
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
            this.txtInputCode = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cbxNewReg = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtInputCode
            // 
            this.txtInputCode.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtInputCode.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtInputCode.IsEnter2Tab = false;
            this.txtInputCode.Location = new System.Drawing.Point(38, 3);
            this.txtInputCode.Name = "txtInputCode";
            this.txtInputCode.Size = new System.Drawing.Size(103, 23);
            this.txtInputCode.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtInputCode.TabIndex = 0;
            this.txtInputCode.Text = "输入卡号或姓名";
            this.txtInputCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInputCode_KeyDown);
            this.txtInputCode.Enter += new System.EventHandler(this.txtInputCode_Enter);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.Location = new System.Drawing.Point(-2, 7);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(49, 14);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 1;
            this.neuLabel1.Text = "卡号：";
            // 
            // cbxNewReg
            // 
            this.cbxNewReg.AutoSize = true;
            this.cbxNewReg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxNewReg.ForeColor = System.Drawing.Color.Red;
            this.cbxNewReg.Location = new System.Drawing.Point(11, 29);
            this.cbxNewReg.Name = "cbxNewReg";
            this.cbxNewReg.Size = new System.Drawing.Size(110, 18);
            this.cbxNewReg.TabIndex = 2;
            this.cbxNewReg.Text = "新的挂号记录";
            this.cbxNewReg.UseVisualStyleBackColor = true;
            // 
            // ucQuerySeeNoByCardNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.cbxNewReg);
            this.Controls.Add(this.txtInputCode);
            this.Controls.Add(this.neuLabel1);
            this.Name = "ucQuerySeeNoByCardNo";
            this.Size = new System.Drawing.Size(144, 31);
            this.Load += new System.EventHandler(this.ucQuerySeeNoByCardNo_Load);
            this.Leave += new System.EventHandler(this.ucQuerySeeNoByCardNo_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtInputCode;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private System.Windows.Forms.CheckBox cbxNewReg;
    }
}
