namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IMultiScreen
{
    partial class frmMiltScreen
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

            if (disposing)
            {
                if (this.dsItem != null)
                {
                    this.dsItem.Clear();
                    this.dsItem.Dispose();
                    this.dsItem = null;

                    System.GC.Collect();
                }
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
            this.neuPanel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuPanel4 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbDrugCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbTotCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbReturnCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbRealOwnCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbPubCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbPayCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbOwnCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbDrugSendInfo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblPaient = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblPaientInfo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel8 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbDrugCost = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel7 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblPubTitle = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblPayTitle = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuPanel3.SuspendLayout();
            this.neuPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuPanel3
            // 
            this.neuPanel3.Controls.Add(this.neuPanel4);
            this.neuPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel3.Location = new System.Drawing.Point(0, 0);
            this.neuPanel3.Name = "neuPanel3";
            this.neuPanel3.Size = new System.Drawing.Size(1024, 768);
            this.neuPanel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel3.TabIndex = 1;
            // 
            // neuPanel4
            // 
            this.neuPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.neuPanel4.Controls.Add(this.neuLabel1);
            this.neuPanel4.Controls.Add(this.tbDrugCost);
            this.neuPanel4.Controls.Add(this.tbTotCost);
            this.neuPanel4.Controls.Add(this.tbReturnCost);
            this.neuPanel4.Controls.Add(this.tbRealOwnCost);
            this.neuPanel4.Controls.Add(this.tbPubCost);
            this.neuPanel4.Controls.Add(this.tbPayCost);
            this.neuPanel4.Controls.Add(this.tbOwnCost);
            this.neuPanel4.Controls.Add(this.tbDrugSendInfo);
            this.neuPanel4.Controls.Add(this.lblPaient);
            this.neuPanel4.Controls.Add(this.lblPaientInfo);
            this.neuPanel4.Controls.Add(this.neuLabel8);
            this.neuPanel4.Controls.Add(this.lbDrugCost);
            this.neuPanel4.Controls.Add(this.neuLabel6);
            this.neuPanel4.Controls.Add(this.neuLabel7);
            this.neuPanel4.Controls.Add(this.lblPubTitle);
            this.neuPanel4.Controls.Add(this.lblPayTitle);
            this.neuPanel4.Controls.Add(this.neuLabel2);
            this.neuPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel4.Location = new System.Drawing.Point(0, 0);
            this.neuPanel4.Name = "neuPanel4";
            this.neuPanel4.Size = new System.Drawing.Size(1024, 768);
            this.neuPanel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel4.TabIndex = 1;
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.BackColor = System.Drawing.Color.Transparent;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.ForeColor = System.Drawing.Color.Red;
            this.neuLabel1.Location = new System.Drawing.Point(589, 665);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(423, 56);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 25;
            this.neuLabel1.Text = "祝您身体健康！";
            // 
            // tbDrugCost
            // 
            this.tbDrugCost.AutoSize = true;
            this.tbDrugCost.BackColor = System.Drawing.Color.Transparent;
            this.tbDrugCost.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbDrugCost.Location = new System.Drawing.Point(217, 675);
            this.tbDrugCost.Name = "tbDrugCost";
            this.tbDrugCost.Size = new System.Drawing.Size(91, 35);
            this.tbDrugCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbDrugCost.TabIndex = 24;
            this.tbDrugCost.Text = "0.00";
            this.tbDrugCost.Visible = false;
            // 
            // tbTotCost
            // 
            this.tbTotCost.AutoSize = true;
            this.tbTotCost.BackColor = System.Drawing.Color.Transparent;
            this.tbTotCost.Font = new System.Drawing.Font("宋体", 44F, System.Drawing.FontStyle.Bold);
            this.tbTotCost.Location = new System.Drawing.Point(270, 366);
            this.tbTotCost.Name = "tbTotCost";
            this.tbTotCost.Size = new System.Drawing.Size(149, 59);
            this.tbTotCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbTotCost.TabIndex = 23;
            this.tbTotCost.Text = "0.00";
            // 
            // tbReturnCost
            // 
            this.tbReturnCost.AutoSize = true;
            this.tbReturnCost.BackColor = System.Drawing.Color.Transparent;
            this.tbReturnCost.Font = new System.Drawing.Font("宋体", 52F, System.Drawing.FontStyle.Bold);
            this.tbReturnCost.ForeColor = System.Drawing.Color.Red;
            this.tbReturnCost.Location = new System.Drawing.Point(739, 235);
            this.tbReturnCost.Name = "tbReturnCost";
            this.tbReturnCost.Size = new System.Drawing.Size(174, 70);
            this.tbReturnCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbReturnCost.TabIndex = 22;
            this.tbReturnCost.Text = "0.00";
            // 
            // tbRealOwnCost
            // 
            this.tbRealOwnCost.AutoSize = true;
            this.tbRealOwnCost.BackColor = System.Drawing.Color.Transparent;
            this.tbRealOwnCost.Font = new System.Drawing.Font("宋体", 52F, System.Drawing.FontStyle.Bold);
            this.tbRealOwnCost.ForeColor = System.Drawing.Color.Red;
            this.tbRealOwnCost.Location = new System.Drawing.Point(256, 235);
            this.tbRealOwnCost.Name = "tbRealOwnCost";
            this.tbRealOwnCost.Size = new System.Drawing.Size(174, 70);
            this.tbRealOwnCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbRealOwnCost.TabIndex = 21;
            this.tbRealOwnCost.Text = "0.00";
            // 
            // tbPubCost
            // 
            this.tbPubCost.AutoSize = true;
            this.tbPubCost.BackColor = System.Drawing.Color.Transparent;
            this.tbPubCost.Font = new System.Drawing.Font("宋体", 44F, System.Drawing.FontStyle.Bold);
            this.tbPubCost.Location = new System.Drawing.Point(270, 490);
            this.tbPubCost.Name = "tbPubCost";
            this.tbPubCost.Size = new System.Drawing.Size(149, 59);
            this.tbPubCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbPubCost.TabIndex = 20;
            this.tbPubCost.Text = "0.00";
            this.tbPubCost.Visible = false;
            // 
            // tbPayCost
            // 
            this.tbPayCost.AutoSize = true;
            this.tbPayCost.BackColor = System.Drawing.Color.Transparent;
            this.tbPayCost.Font = new System.Drawing.Font("宋体", 44F, System.Drawing.FontStyle.Bold);
            this.tbPayCost.Location = new System.Drawing.Point(739, 489);
            this.tbPayCost.Name = "tbPayCost";
            this.tbPayCost.Size = new System.Drawing.Size(149, 59);
            this.tbPayCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbPayCost.TabIndex = 19;
            this.tbPayCost.Text = "0.00";
            this.tbPayCost.Visible = false;
            // 
            // tbOwnCost
            // 
            this.tbOwnCost.AutoSize = true;
            this.tbOwnCost.BackColor = System.Drawing.Color.Transparent;
            this.tbOwnCost.Font = new System.Drawing.Font("宋体", 52F, System.Drawing.FontStyle.Bold);
            this.tbOwnCost.ForeColor = System.Drawing.Color.Red;
            this.tbOwnCost.Location = new System.Drawing.Point(739, 359);
            this.tbOwnCost.Name = "tbOwnCost";
            this.tbOwnCost.Size = new System.Drawing.Size(282, 70);
            this.tbOwnCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbOwnCost.TabIndex = 18;
            this.tbOwnCost.Text = "9999.99";
            // 
            // tbDrugSendInfo
            // 
            this.tbDrugSendInfo.BackColor = System.Drawing.Color.Transparent;
            this.tbDrugSendInfo.Font = new System.Drawing.Font("宋体", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbDrugSendInfo.ForeColor = System.Drawing.Color.Black;
            this.tbDrugSendInfo.Location = new System.Drawing.Point(26, 580);
            this.tbDrugSendInfo.Name = "tbDrugSendInfo";
            this.tbDrugSendInfo.Size = new System.Drawing.Size(986, 72);
            this.tbDrugSendInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbDrugSendInfo.TabIndex = 17;
            this.tbDrugSendInfo.Text = "取药药房";
            this.tbDrugSendInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPaient
            // 
            this.lblPaient.BackColor = System.Drawing.Color.Transparent;
            this.lblPaient.Font = new System.Drawing.Font("微软雅黑", 46F, System.Drawing.FontStyle.Bold);
            this.lblPaient.ForeColor = System.Drawing.Color.Red;
            this.lblPaient.Location = new System.Drawing.Point(65, 105);
            this.lblPaient.Name = "lblPaient";
            this.lblPaient.Size = new System.Drawing.Size(270, 108);
            this.lblPaient.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPaient.TabIndex = 16;
            this.lblPaient.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPaientInfo
            // 
            this.lblPaientInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblPaientInfo.Font = new System.Drawing.Font("微软雅黑", 42F, System.Drawing.FontStyle.Bold);
            this.lblPaientInfo.ForeColor = System.Drawing.Color.Red;
            this.lblPaientInfo.Location = new System.Drawing.Point(341, 105);
            this.lblPaientInfo.Name = "lblPaientInfo";
            this.lblPaientInfo.Size = new System.Drawing.Size(656, 108);
            this.lblPaientInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPaientInfo.TabIndex = 16;
            this.lblPaientInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // neuLabel8
            // 
            this.neuLabel8.AutoSize = true;
            this.neuLabel8.BackColor = System.Drawing.Color.Transparent;
            this.neuLabel8.Font = new System.Drawing.Font("宋体", 46F, System.Drawing.FontStyle.Bold);
            this.neuLabel8.Location = new System.Drawing.Point(42, 363);
            this.neuLabel8.Name = "neuLabel8";
            this.neuLabel8.Size = new System.Drawing.Size(248, 62);
            this.neuLabel8.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel8.TabIndex = 14;
            this.neuLabel8.Text = "总费用:";
            // 
            // lbDrugCost
            // 
            this.lbDrugCost.AutoSize = true;
            this.lbDrugCost.BackColor = System.Drawing.Color.Transparent;
            this.lbDrugCost.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbDrugCost.Location = new System.Drawing.Point(47, 675);
            this.lbDrugCost.Name = "lbDrugCost";
            this.lbDrugCost.Size = new System.Drawing.Size(178, 35);
            this.lbDrugCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbDrugCost.TabIndex = 10;
            this.lbDrugCost.Text = "药品总额:";
            this.lbDrugCost.Visible = false;
            // 
            // neuLabel6
            // 
            this.neuLabel6.AutoSize = true;
            this.neuLabel6.BackColor = System.Drawing.Color.Transparent;
            this.neuLabel6.Font = new System.Drawing.Font("宋体", 46F, System.Drawing.FontStyle.Bold);
            this.neuLabel6.Location = new System.Drawing.Point(573, 237);
            this.neuLabel6.Name = "neuLabel6";
            this.neuLabel6.Size = new System.Drawing.Size(185, 62);
            this.neuLabel6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel6.TabIndex = 8;
            this.neuLabel6.Text = "找零:";
            // 
            // neuLabel7
            // 
            this.neuLabel7.AutoSize = true;
            this.neuLabel7.BackColor = System.Drawing.Color.Transparent;
            this.neuLabel7.Font = new System.Drawing.Font("宋体", 46F, System.Drawing.FontStyle.Bold);
            this.neuLabel7.Location = new System.Drawing.Point(99, 237);
            this.neuLabel7.Name = "neuLabel7";
            this.neuLabel7.Size = new System.Drawing.Size(185, 62);
            this.neuLabel7.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel7.TabIndex = 6;
            this.neuLabel7.Text = "实收:";
            // 
            // lblPubTitle
            // 
            this.lblPubTitle.AutoSize = true;
            this.lblPubTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblPubTitle.Font = new System.Drawing.Font("宋体", 46F, System.Drawing.FontStyle.Bold);
            this.lblPubTitle.Location = new System.Drawing.Point(99, 489);
            this.lblPubTitle.Name = "lblPubTitle";
            this.lblPubTitle.Size = new System.Drawing.Size(185, 62);
            this.lblPubTitle.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPubTitle.TabIndex = 4;
            this.lblPubTitle.Text = "医保:";
            this.lblPubTitle.Visible = false;
            // 
            // lblPayTitle
            // 
            this.lblPayTitle.AutoSize = true;
            this.lblPayTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblPayTitle.Font = new System.Drawing.Font("宋体", 46F, System.Drawing.FontStyle.Bold);
            this.lblPayTitle.Location = new System.Drawing.Point(572, 489);
            this.lblPayTitle.Name = "lblPayTitle";
            this.lblPayTitle.Size = new System.Drawing.Size(186, 62);
            this.lblPayTitle.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPayTitle.TabIndex = 2;
            this.lblPayTitle.Text = "IC卡:";
            this.lblPayTitle.Visible = false;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.BackColor = System.Drawing.Color.Transparent;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 46F, System.Drawing.FontStyle.Bold);
            this.neuLabel2.Location = new System.Drawing.Point(573, 363);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(185, 62);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 0;
            this.neuLabel2.Text = "应缴:";
            // 
            // frmMiltScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.neuPanel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMiltScreen";
            this.Text = "门诊外屏";
            this.neuPanel3.ResumeLayout(false);
            this.neuPanel4.ResumeLayout(false);
            this.neuPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbDrugCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel6;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel7;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPubTitle;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPayTitle;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel8;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPaientInfo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbDrugSendInfo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbOwnCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbDrugCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbTotCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbReturnCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbRealOwnCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbPubCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel tbPayCost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPaient;
    }
}
