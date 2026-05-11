namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucQueryInpatient
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
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.btBed = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btNuerse = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.txtBed = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtNurse = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtInpatientNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.btOK = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btCancel = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.Location = new System.Drawing.Point(6, 30);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(41, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 0;
            this.neuLabel1.Text = "住院号";
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.btBed);
            this.neuGroupBox1.Controls.Add(this.btNuerse);
            this.neuGroupBox1.Controls.Add(this.txtBed);
            this.neuGroupBox1.Controls.Add(this.txtNurse);
            this.neuGroupBox1.Controls.Add(this.txtInpatientNO);
            this.neuGroupBox1.Controls.Add(this.neuLabel3);
            this.neuGroupBox1.Controls.Add(this.neuLabel2);
            this.neuGroupBox1.Controls.Add(this.neuLabel1);
            this.neuGroupBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuGroupBox1.Location = new System.Drawing.Point(16, 11);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(250, 141);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 1;
            this.neuGroupBox1.TabStop = false;
            // 
            // btBed
            // 
            this.btBed.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btBed.Location = new System.Drawing.Point(195, 97);
            this.btBed.Name = "btBed";
            this.btBed.Size = new System.Drawing.Size(39, 23);
            this.btBed.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btBed.TabIndex = 5;
            this.btBed.Text = "...";
            this.btBed.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btBed.UseVisualStyleBackColor = true;
            this.btBed.Click += new System.EventHandler(this.btBed_Click);
            // 
            // btNuerse
            // 
            this.btNuerse.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btNuerse.Location = new System.Drawing.Point(195, 63);
            this.btNuerse.Name = "btNuerse";
            this.btNuerse.Size = new System.Drawing.Size(39, 23);
            this.btNuerse.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btNuerse.TabIndex = 4;
            this.btNuerse.Text = "...";
            this.btNuerse.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btNuerse.UseVisualStyleBackColor = true;
            this.btNuerse.Click += new System.EventHandler(this.btNuerse_Click);
            // 
            // txtBed
            // 
            this.txtBed.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBed.IsEnter2Tab = false;
            this.txtBed.Location = new System.Drawing.Point(53, 97);
            this.txtBed.Name = "txtBed";
            this.txtBed.Size = new System.Drawing.Size(134, 21);
            this.txtBed.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtBed.TabIndex = 3;
            this.txtBed.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBed_KeyDown);
            // 
            // txtNurse
            // 
            this.txtNurse.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtNurse.IsEnter2Tab = false;
            this.txtNurse.Location = new System.Drawing.Point(53, 63);
            this.txtNurse.Name = "txtNurse";
            this.txtNurse.Size = new System.Drawing.Size(134, 21);
            this.txtNurse.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtNurse.TabIndex = 2;
            this.txtNurse.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNurse_KeyDown);
            // 
            // txtInpatientNO
            // 
            this.txtInpatientNO.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtInpatientNO.IsEnter2Tab = false;
            this.txtInpatientNO.Location = new System.Drawing.Point(53, 25);
            this.txtInpatientNO.Name = "txtInpatientNO";
            this.txtInpatientNO.Size = new System.Drawing.Size(134, 21);
            this.txtInpatientNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtInpatientNO.TabIndex = 1;
            this.txtInpatientNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInpatientNO_KeyDown);
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel3.Location = new System.Drawing.Point(6, 100);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(41, 12);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 0;
            this.neuLabel3.Text = "床位号";
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel2.Location = new System.Drawing.Point(6, 66);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(29, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 0;
            this.neuLabel2.Text = "病区";
            // 
            // btOK
            // 
            this.btOK.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btOK.Location = new System.Drawing.Point(38, 162);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(78, 33);
            this.btOK.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btOK.TabIndex = 6;
            this.btOK.Text = "确认(&O)";
            this.btOK.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // btCancel
            // 
            this.btCancel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btCancel.Location = new System.Drawing.Point(157, 162);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(78, 33);
            this.btCancel.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btCancel.TabIndex = 7;
            this.btCancel.Text = "取消(&C)";
            this.btCancel.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // ucQueryInpatient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.neuGroupBox1);
            this.Name = "ucQueryInpatient";
            this.Size = new System.Drawing.Size(284, 201);
            this.Load += new System.EventHandler(this.ucQueryInpatient_Load);
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtInpatientNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btBed;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btNuerse;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtBed;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtNurse;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btOK;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btCancel;
    }
}
