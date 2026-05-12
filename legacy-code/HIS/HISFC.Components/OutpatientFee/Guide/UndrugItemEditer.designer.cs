namespace Neusoft.HISFC.Components.OutpatientFee.Guide
{
    partial class UndrugItemEditer
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
            this.components = new System.ComponentModel.Container();
            this.btSave = new System.Windows.Forms.Button();
            this.btExit = new System.Windows.Forms.Button();
            this.cmbDept = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbAddr = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbValid = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbItem = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cburgency = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.SuspendLayout();
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(51, 299);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 0;
            this.btSave.Text = "保存";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btExit
            // 
            this.btExit.Location = new System.Drawing.Point(183, 299);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(75, 23);
            this.btExit.TabIndex = 1;
            this.btExit.Text = "退出";
            this.btExit.UseVisualStyleBackColor = true;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // cmbDept
            // 
            this.cmbDept.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbDept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbDept.FormattingEnabled = true;
            this.cmbDept.IsEnter2Tab = false;
            this.cmbDept.IsFlat = false;
            this.cmbDept.IsLike = true;
            this.cmbDept.IsListOnly = false;
            this.cmbDept.IsPopForm = true;
            this.cmbDept.IsShowCustomerList = false;
            this.cmbDept.IsShowID = false;
            this.cmbDept.IsShowIDAndName = false;
            this.cmbDept.Location = new System.Drawing.Point(83, 76);
            this.cmbDept.Name = "cmbDept";
            this.cmbDept.ShowCustomerList = false;
            this.cmbDept.ShowID = false;
            this.cmbDept.Size = new System.Drawing.Size(247, 20);
            this.cmbDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDept.TabIndex = 12;
            this.cmbDept.Tag = "";
            this.cmbDept.ToolBarUse = false;
            this.cmbDept.Visible = false;
            this.cmbDept.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSort_KeyPress);
            // 
            // cmbAddr
            // 
            this.cmbAddr.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbAddr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbAddr.FormattingEnabled = true;
            this.cmbAddr.IsEnter2Tab = false;
            this.cmbAddr.IsFlat = false;
            this.cmbAddr.IsLike = true;
            this.cmbAddr.IsListOnly = false;
            this.cmbAddr.IsPopForm = true;
            this.cmbAddr.IsShowCustomerList = false;
            this.cmbAddr.IsShowID = false;
            this.cmbAddr.IsShowIDAndName = false;
            this.cmbAddr.Location = new System.Drawing.Point(83, 102);
            this.cmbAddr.Name = "cmbAddr";
            this.cmbAddr.ShowCustomerList = false;
            this.cmbAddr.ShowID = false;
            this.cmbAddr.Size = new System.Drawing.Size(248, 20);
            this.cmbAddr.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbAddr.TabIndex = 11;
            this.cmbAddr.Tag = "";
            this.cmbAddr.ToolBarUse = false;
            this.cmbAddr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSort_KeyPress);
            // 
            // cmbValid
            // 
            this.cmbValid.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbValid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbValid.FormattingEnabled = true;
            this.cmbValid.IsEnter2Tab = false;
            this.cmbValid.IsFlat = false;
            this.cmbValid.IsLike = true;
            this.cmbValid.IsListOnly = false;
            this.cmbValid.IsPopForm = true;
            this.cmbValid.IsShowCustomerList = false;
            this.cmbValid.IsShowID = false;
            this.cmbValid.IsShowIDAndName = false;
            this.cmbValid.Location = new System.Drawing.Point(84, 151);
            this.cmbValid.Name = "cmbValid";
            this.cmbValid.ShowCustomerList = false;
            this.cmbValid.ShowID = false;
            this.cmbValid.Size = new System.Drawing.Size(247, 20);
            this.cmbValid.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbValid.TabIndex = 10;
            this.cmbValid.Tag = "";
            this.cmbValid.ToolBarUse = false;
            this.cmbValid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSort_KeyPress);
            // 
            // cmbItem
            // 
            this.cmbItem.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.IsEnter2Tab = false;
            this.cmbItem.IsFlat = false;
            this.cmbItem.IsLike = true;
            this.cmbItem.IsListOnly = false;
            this.cmbItem.IsPopForm = true;
            this.cmbItem.IsShowCustomerList = false;
            this.cmbItem.IsShowID = false;
            this.cmbItem.IsShowIDAndName = false;
            this.cmbItem.Location = new System.Drawing.Point(83, 45);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.ShowCustomerList = false;
            this.cmbItem.ShowID = false;
            this.cmbItem.Size = new System.Drawing.Size(247, 20);
            this.cmbItem.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbItem.TabIndex = 9;
            this.cmbItem.Tag = "";
            this.cmbItem.ToolBarUse = false;
            this.cmbItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSort_KeyPress);
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Location = new System.Drawing.Point(13, 159);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(65, 12);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 8;
            this.neuLabel4.Text = "是否有效：";
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Location = new System.Drawing.Point(12, 79);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(65, 12);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 6;
            this.neuLabel3.Text = "开单科室：";
            this.neuLabel3.Visible = false;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Location = new System.Drawing.Point(12, 110);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(65, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 4;
            this.neuLabel2.Text = "执行地点：";
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(12, 45);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(65, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 2;
            this.neuLabel1.Text = "项    目：";
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Location = new System.Drawing.Point(12, 204);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(65, 12);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 13;
            this.neuLabel5.Text = "能否加急：";
            // 
            // cburgency
            // 
            this.cburgency.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cburgency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cburgency.FormattingEnabled = true;
            this.cburgency.IsEnter2Tab = false;
            this.cburgency.IsFlat = false;
            this.cburgency.IsLike = true;
            this.cburgency.IsListOnly = false;
            this.cburgency.IsPopForm = true;
            this.cburgency.IsShowCustomerList = false;
            this.cburgency.IsShowID = false;
            this.cburgency.IsShowIDAndName = false;
            this.cburgency.Location = new System.Drawing.Point(83, 199);
            this.cburgency.Name = "cburgency";
            this.cburgency.ShowCustomerList = false;
            this.cburgency.ShowID = false;
            this.cburgency.Size = new System.Drawing.Size(247, 20);
            this.cburgency.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cburgency.TabIndex = 10;
            this.cburgency.Tag = "";
            this.cburgency.ToolBarUse = false;
            this.cburgency.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSort_KeyPress);
            // 
            // UndrugItemEditer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 382);
            this.Controls.Add(this.neuLabel5);
            this.Controls.Add(this.cmbDept);
            this.Controls.Add(this.cmbAddr);
            this.Controls.Add(this.cburgency);
            this.Controls.Add(this.cmbValid);
            this.Controls.Add(this.cmbItem);
            this.Controls.Add(this.neuLabel4);
            this.Controls.Add(this.neuLabel3);
            this.Controls.Add(this.neuLabel2);
            this.Controls.Add(this.neuLabel1);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.btSave);
            this.Name = "UndrugItemEditer";
            this.Text = "执行地点维护";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btExit;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbItem;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbValid;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbAddr;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDept;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cburgency;

        //private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        //private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbItem;
             
    }
}