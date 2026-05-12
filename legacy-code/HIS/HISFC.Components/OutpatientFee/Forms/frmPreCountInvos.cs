using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Neusoft.FrameWork.Models;
using Neusoft.FrameWork.Function;

namespace Neusoft.UFC.OutpatientFee.Forms
{
    /// <summary>
    /// frmPreCountInvos 的摘要说明。
    /// </summary>
    public class frmPreCountInvos : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbOwn;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbPub;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbTot;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbReal;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbPay;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbLittle;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbRealRecived;
        private Label label9;
        private Label label10;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtReCost;
        private Button button4;
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.Container components = null;

        public frmPreCountInvos()
        {
            //
            // Windows 窗体设计器支持所必需的
            //
            InitializeComponent();
            iMultiScreen = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject<
                        Neusoft.HISFC.BizProcess.Interface.FeeInterface.IMultiScreen>(this.GetType());
            //
            // TODO: 在 InitializeComponent 调用后添加任何构造函数代码
            //
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbLittle = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbRealRecived = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbReal = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbTot = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtReCost = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbPub = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbPay = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbOwn = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbCount = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbLittle);
            this.groupBox1.Controls.Add(this.tbRealRecived);
            this.groupBox1.Controls.Add(this.tbReal);
            this.groupBox1.Controls.Add(this.tbTot);
            this.groupBox1.Controls.Add(this.txtReCost);
            this.groupBox1.Controls.Add(this.tbPub);
            this.groupBox1.Controls.Add(this.tbPay);
            this.groupBox1.Controls.Add(this.tbOwn);
            this.groupBox1.Controls.Add(this.tbCount);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(837, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(504, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "找零:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(352, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 15);
            this.label8.TabIndex = 12;
            this.label8.Text = "实收:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(200, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "现金:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(45, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "合计:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 11F);
            this.label10.Location = new System.Drawing.Point(659, 52);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "减免:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 11F);
            this.label4.Location = new System.Drawing.Point(505, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "记账:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 11F);
            this.label3.Location = new System.Drawing.Point(353, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "自付:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 11F);
            this.label2.Location = new System.Drawing.Point(201, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "自费:";
            // 
            // tbLittle
            // 
            this.tbLittle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbLittle.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbLittle.IsEnter2Tab = false;
            this.tbLittle.Location = new System.Drawing.Point(552, 16);
            this.tbLittle.Name = "tbLittle";
            this.tbLittle.ReadOnly = true;
            this.tbLittle.Size = new System.Drawing.Size(100, 25);
            this.tbLittle.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbLittle.TabIndex = 15;
            // 
            // tbRealRecived
            // 
            this.tbRealRecived.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbRealRecived.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbRealRecived.IsEnter2Tab = false;
            this.tbRealRecived.Location = new System.Drawing.Point(400, 16);
            this.tbRealRecived.Name = "tbRealRecived";
            this.tbRealRecived.Size = new System.Drawing.Size(100, 25);
            this.tbRealRecived.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbRealRecived.TabIndex = 13;
            this.tbRealRecived.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbRealRecived_KeyDown);
            // 
            // tbReal
            // 
            this.tbReal.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbReal.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbReal.IsEnter2Tab = false;
            this.tbReal.Location = new System.Drawing.Point(248, 16);
            this.tbReal.Name = "tbReal";
            this.tbReal.ReadOnly = true;
            this.tbReal.Size = new System.Drawing.Size(100, 25);
            this.tbReal.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbReal.TabIndex = 11;
            // 
            // tbTot
            // 
            this.tbTot.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbTot.Font = new System.Drawing.Font("宋体", 11F);
            this.tbTot.IsEnter2Tab = false;
            this.tbTot.Location = new System.Drawing.Point(96, 49);
            this.tbTot.Name = "tbTot";
            this.tbTot.ReadOnly = true;
            this.tbTot.Size = new System.Drawing.Size(100, 24);
            this.tbTot.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbTot.TabIndex = 9;
            // 
            // txtReCost
            // 
            this.txtReCost.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtReCost.Font = new System.Drawing.Font("宋体", 11F);
            this.txtReCost.IsEnter2Tab = false;
            this.txtReCost.Location = new System.Drawing.Point(706, 49);
            this.txtReCost.Name = "txtReCost";
            this.txtReCost.ReadOnly = true;
            this.txtReCost.Size = new System.Drawing.Size(100, 24);
            this.txtReCost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtReCost.TabIndex = 7;
            // 
            // tbPub
            // 
            this.tbPub.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbPub.Font = new System.Drawing.Font("宋体", 11F);
            this.tbPub.IsEnter2Tab = false;
            this.tbPub.Location = new System.Drawing.Point(553, 49);
            this.tbPub.Name = "tbPub";
            this.tbPub.ReadOnly = true;
            this.tbPub.Size = new System.Drawing.Size(100, 24);
            this.tbPub.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbPub.TabIndex = 7;
            // 
            // tbPay
            // 
            this.tbPay.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbPay.Font = new System.Drawing.Font("宋体", 11F);
            this.tbPay.IsEnter2Tab = false;
            this.tbPay.Location = new System.Drawing.Point(401, 49);
            this.tbPay.Name = "tbPay";
            this.tbPay.ReadOnly = true;
            this.tbPay.Size = new System.Drawing.Size(100, 24);
            this.tbPay.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbPay.TabIndex = 5;
            // 
            // tbOwn
            // 
            this.tbOwn.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbOwn.Font = new System.Drawing.Font("宋体", 11F);
            this.tbOwn.IsEnter2Tab = false;
            this.tbOwn.Location = new System.Drawing.Point(249, 49);
            this.tbOwn.Name = "tbOwn";
            this.tbOwn.ReadOnly = true;
            this.tbOwn.Size = new System.Drawing.Size(100, 24);
            this.tbOwn.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbOwn.TabIndex = 3;
            // 
            // tbCount
            // 
            this.tbCount.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbCount.Font = new System.Drawing.Font("宋体", 11F);
            this.tbCount.IsEnter2Tab = false;
            this.tbCount.Location = new System.Drawing.Point(96, 16);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(100, 24);
            this.tbCount.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCount.TabIndex = 1;
            this.tbCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCount_KeyDown);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 11F);
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "发票张数:";
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1, Row 0, Column 0, ";
            this.fpSpread1.BackColor = System.Drawing.SystemColors.Control;
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.Location = new System.Drawing.Point(0, 80);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(837, 249);
            this.fpSpread1.TabIndex = 1;
            tipAppearance2.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread1.TextTipAppearance = tipAppearance2;
            this.fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 9;
            this.fpSpread1_Sheet1.RowCount = 1;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "发票号";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "患者姓名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "收费时间";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "合计金额";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "自费";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "自付";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "记账";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "减免";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "结算方式";
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = textCellType2;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "发票号";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 105F;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "收费时间";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 147F;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "合计金额";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 67F;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "自费";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 72F;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "自付";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 62F;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "记账";
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 61F;
            this.fpSpread1_Sheet1.Columns.Get(8).Label = "结算方式";
            this.fpSpread1_Sheet1.Columns.Get(8).Width = 146F;
            this.fpSpread1_Sheet1.GrayAreaBackColor = System.Drawing.Color.White;
            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.fpSpread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 329);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(837, 40);
            this.panel1.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(17, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(175, 14);
            this.label9.TabIndex = 3;
            this.label9.Text = "输入发票张数累计发票金额";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 10F);
            this.button3.Location = new System.Drawing.Point(507, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 24);
            this.button3.TabIndex = 2;
            this.button3.Text = "查询(&Q)";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 10F);
            this.button2.Location = new System.Drawing.Point(699, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 24);
            this.button2.TabIndex = 1;
            this.button2.Text = "退出(&X)";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 10F);
            this.button1.Location = new System.Drawing.Point(603, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 24);
            this.button1.TabIndex = 0;
            this.button1.Text = "清屏(&C)";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 10F);
            this.button4.Location = new System.Drawing.Point(405, 10);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(88, 24);
            this.button4.TabIndex = 4;
            this.button4.Text = "叫号(&J)";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // frmPreCountInvos
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(837, 369);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmPreCountInvos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "输入发票张数累计发票金额";
            this.Load += new System.EventHandler(this.frmPreCountInvos_Load);
            this.Closed += new System.EventHandler(this.frmPreCountInvos_Closed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        Neusoft.HISFC.BizLogic.Fee.Outpatient myOutPatient = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        Neusoft.FrameWork.Public.ObjectHelper deptHelper = new Neusoft.FrameWork.Public.ObjectHelper();
        ArrayList alInvos = new ArrayList();
        Hashtable hcInvoiceSeq = new Hashtable();

        string name = "";
        decimal siCost = 0m;
        decimal totCost = 0m;
        decimal cashCost = 0m;

        /// <summary>
        /// 查询
        /// </summary>
        private void Query()
        {
            if (tbCount.Text == "")
            {
                MessageBox.Show("请输入发票张数!");
                this.tbCount.Focus();
                return;
            }

            if (this.hcInvoiceSeq.Count > 0)
            {
                this.hcInvoiceSeq.Clear();
            }

            int count = 0;
            decimal sumTotCost = 0;
            decimal sumOwnCost = 0;
            decimal sumPayCost = 0;
            decimal sumPubCost = 0;
            decimal sumCashCost = 0;
            decimal sumReCost = 0;
            decimal sumShouldCost = 0;

            try
            {
                count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.tbCount.Text);

                if (count <= 0)
                {
                    MessageBox.Show("发票张数应大于0" + "~r");
                    tbCount.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("发票张数输入不合法" + "~r" + ex.Message);
                tbCount.Focus();
                return;
            }
            alInvos = myOutPatient.QueryBalancesByCount(this.myOutPatient.Operator.ID, count);
            if (alInvos == null)
            {
                MessageBox.Show("查询患者发票信息出错!" + myOutPatient.Err);
                return;
            }
            if (alInvos.Count == 0)
            {
                MessageBox.Show("今天该操作员没有发票发生!");
                return;
            }
            Neusoft.HISFC.Models.Fee.Outpatient.Balance invo = null;
            this.fpSpread1_Sheet1.RowCount = 0;
            this.fpSpread1_Sheet1.Rows.Add(0, 1);
            for (int i = 0; i < alInvos.Count; i++)
            {
                this.fpSpread1_Sheet1.Rows.Add(this.fpSpread1_Sheet1.RowCount, 1);
                invo = alInvos[i] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;
                this.fpSpread1_Sheet1.Cells[i, 0].Text = invo.Invoice.ID;
                this.fpSpread1_Sheet1.Cells[i, 1].Text = invo.Patient.Name;
                this.fpSpread1_Sheet1.Cells[i, 2].Text = invo.BalanceOper.OperTime.ToString();
                this.fpSpread1_Sheet1.Cells[i, 3].Text = (invo.FT.OwnCost + invo.FT.PayCost + invo.FT.PubCost).ToString();
                this.fpSpread1_Sheet1.Cells[i, 4].Text = invo.FT.OwnCost.ToString();
                this.fpSpread1_Sheet1.Cells[i, 5].Text = invo.FT.PayCost.ToString();
                this.fpSpread1_Sheet1.Cells[i, 6].Text = invo.FT.PubCost.ToString();
                this.fpSpread1_Sheet1.Cells[i, 7].Text = invo.FT.DerateCost.ToString();
                #region 增加支付方式
                ArrayList alPayMode = this.myOutPatient.QueryBalancePaysByInvoiceNO(invo.Invoice.ID);//.GetPayModeByInvo(invo.ID);
                string payModeText = "";

                if (alPayMode != null && alPayMode.Count > 0)
                {
                    payModeText = "";
                    NeuObject payType = null;
                    for (int k = 0; k < alPayMode.Count; k++)
                    {
                        Neusoft.HISFC.Models.Fee.Outpatient.BalancePay payMode = alPayMode[k] as Neusoft.HISFC.Models.Fee.Outpatient.BalancePay;

                        if (payMode.PayType.ID == "RC")
                        {
                            sumReCost += payMode.FT.TotCost;
                        }

                        payType = deptHelper.GetObjectFromID(payMode.PayType.ID);
                        if (payType == null)
                        {
                            switch (payMode.PayType.ID)
                            {
                                case "YS":
                                    payModeText += "帐户支付";
                                    break;

                                case "RC":
                                    payModeText += "减免";
                                    break;

                                default:
                                    payModeText += "其他";
                                    break;
                            }
                        }
                        else
                        {
                            payModeText += payType.Name;
                        }

                        payModeText += " " + Neusoft.FrameWork.Public.String.FormatNumber(payMode.FT.TotCost, 2);



                        //if (payMode.PayType.ID.ToString() == "CD" || payMode.PayType.ID.ToString() == "DB")
                        //{
                        //    cdCost += payMode.FT.TotCost;
                        //}
                        //else
                        //{
                        //    payModeText += " ";
                        //    if (payMode.PayType.ID.ToString() == "PS")
                        //    {
                        //        payModeText +="医保卡";
                        //    }
                        //    else
                        //    {
                        //        payModeText += deptHelper.GetObjectFromID(payMode.PayType.ID.ToString()).Name;
                        //    }
                        //    payModeText +=" "+Neusoft.FrameWork.Public.String.FormatNumber(payMode.FT.TotCost,2);//结算操作员
                        //}

                        //if (deptHelper.GetObjectFromID(payMode.PayType.ID.ToString()).Name == "现金" || deptHelper.GetObjectFromID(payMode.PayType.ID.ToString()).Name == "支票")
                        //{
                        //    cashCost += payMode.FT.TotCost;
                        //}
                        //else
                        //{
                        //    siCost += payMode.FT.TotCost;
                        //}

                        //totCost += payMode.FT.TotCost;

                        //if (name.IndexOf(invo.Patient.Name) < 0)
                        //{
                        //    name += invo.Patient.Name+",";
                        //}

                    }
                }
                this.fpSpread1_Sheet1.Cells[i, 8].Text = payModeText;
                #endregion
                sumOwnCost += invo.FT.OwnCost;
                sumPayCost += invo.FT.PayCost;
                sumPubCost += invo.FT.PubCost;

                if ("03".Equals(invo.Patient.Pact.PayKind.ID))
                {
                    //公费
                    sumShouldCost += (invo.FT.OwnCost + invo.FT.PayCost);
                }
                else
                {
                    //其他
                    sumShouldCost += invo.FT.OwnCost;
                }

                if (!hcInvoiceSeq.Contains(invo.Invoice.ID))
                {
                    hcInvoiceSeq.Add(invo.Invoice.ID, null);

                    #region 根据发票号码获得支付方式
                    ArrayList alPayModes = this.myOutPatient.QueryBalancePaysByInvoiceNO(invo.Invoice.ID);//.GetPayModeByInvo(invo.ID);

                    if (alPayModes == null)
                    {
                        MessageBox.Show("根据发票号获得支付方式出错！发票号:" + invo.ID);
                        continue;
                    }
                    else
                    {
                        foreach (Neusoft.HISFC.Models.Fee.Outpatient.BalancePay paymode in alPayModes)
                        {
                            if (paymode.PayType.ID.ToString() == "CA")
                            {
                                sumCashCost += paymode.FT.TotCost;
                            }
                        }
                    }
                    #endregion
                }
            }
            sumTotCost = sumOwnCost + sumPayCost + sumPubCost - sumReCost;

            tbTot.Text = sumTotCost.ToString();
            tbOwn.Text = sumOwnCost.ToString();
            tbPay.Text = sumPayCost.ToString();
            tbPub.Text = sumPubCost.ToString();
            //公费
            tbReal.Text = sumShouldCost.ToString();

            txtReCost.Text = sumReCost.ToString();

            this.tbRealRecived.Focus();
            this.tbRealRecived.SelectAll();
        }
        /// <summary>
        /// 清屏
        /// </summary>
        private void Clear()
        {
            tbTot.Text = "";
            tbOwn.Text = "";
            tbPay.Text = "";
            tbPub.Text = "";
            tbReal.Text = "";
            this.tbRealRecived.Text = "";
            this.tbLittle.Text = "";
            this.fpSpread1_Sheet1.RowCount = 0;
            this.fpSpread1_Sheet1.Rows.Add(0, 1);
            this.tbCount.Text = "";
            this.tbCount.Focus();

            name = "";
            siCost = 0m;
            totCost = 0m;
            cashCost = 0m;
        }

        private void tbCount_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Query();
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            Query();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Clear();
        }

        private void frmPreCountInvos_Load(object sender, System.EventArgs e)
        {
            Clear();

            deptHelper.ArrayObject = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.PAYMODES);
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
            if (keyData == Keys.J)
            {
                this.Call();
            }
            return base.ProcessDialogKey(keyData);
        }

        private void frmPreCountInvos_Closed(object sender, System.EventArgs e)
        {
            this.tbCount.Focus();
        }

        private void tbRealRecived_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                decimal realReceive = 0m;
                decimal shouldPay = 0m;
                decimal ownCost = 0m;
                try
                {
                    shouldPay = System.Convert.ToDecimal(this.tbReal.Text.Trim());
                    realReceive = System.Convert.ToDecimal(this.tbRealRecived.Text.Trim());
                    ownCost = System.Convert.ToDecimal(this.tbOwn.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("您输入的实收金额格式不正确,请重新输入!");
                    this.tbRealRecived.Focus();
                    this.tbRealRecived.SelectAll();
                    return;
                }

                if (realReceive < shouldPay)
                {
                    MessageBox.Show("您输入的实收金额小于应收金额!");
                }

                this.tbLittle.Text = System.Convert.ToString(realReceive - shouldPay);

                this.button2.Focus();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Call();
        }

        /// <summary>
        /// 叫号
        /// </summary>
        private void Call()
        {
            if (iMultiScreen != null)
            {
                System.Collections.Generic.List<Object> lo = new System.Collections.Generic.List<object>();
                Neusoft.HISFC.Models.Registration.Register patient = new Neusoft.HISFC.Models.Registration.Register();
                lo.Add(patient);
                Neusoft.HISFC.Models.Base.FT ft = new Neusoft.HISFC.Models.Base.FT();
                try
                {
                    if (tbRealRecived.Text == "")
                    {
                        ft.RealCost = NConvert.ToDecimal(tbReal.Text);
                        ft.TotCost = NConvert.ToDecimal(tbRealRecived.Text);
                        ft.ReturnCost = NConvert.ToDecimal(tbRealRecived.Text);
                    }
                    else
                    {
                        ft.RealCost = NConvert.ToDecimal(tbRealRecived.Text);
                        ft.TotCost = NConvert.ToDecimal(tbRealRecived.Text);
                        ft.ReturnCost = NConvert.ToDecimal(tbLittle.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("录入数据出错!" + ex.Message);
                    return;
                }

                ft.Memo = "Call";
                lo.Add(ft);
                lo.Add(null);
                lo.Add(null);
                string[] otherInfomations = new string[] { "4" };
                lo.Add(otherInfomations);
                this.iMultiScreen.ListInfo = lo;
            }
        }
        #region IOutpatientOtherInfomationRight 成员

        private Neusoft.HISFC.BizProcess.Interface.FeeInterface.IMultiScreen iMultiScreen = null;

        /// <summary>
        /// IOutpatientOtherInfomationRight
        /// </summary>
        public Neusoft.HISFC.BizProcess.Interface.FeeInterface.IMultiScreen MultiScreen
        {
            set { iMultiScreen = value; }
        }

        #endregion
    }
}
