using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml ;

namespace Neusoft.HISFC.Components.Common.Forms
{
	/// <summary>
	/// frmCompareSql 的摘要说明。
	/// </summary>
	public class frmCompareSql : Neusoft.FrameWork.WinForms.Forms.BaseStatusBar
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panel7;
		private FarPoint.Win.Spread.FpSpread fpSpread1;
		private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
		private FarPoint.Win.Spread.FpSpread fpSpread2;
		private FarPoint.Win.Spread.SheetView fpSpread2_Sheet1;
		private System.Windows.Forms.RichTextBox richTextBox2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RichTextBox richTextBox1;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmCompareSql()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			this.Load+=new EventHandler(frmCompareSql_Load);
			this.fpSpread1.LeaveCell += new FarPoint.Win.Spread.LeaveCellEventHandler(fpSpread1_LeaveCell);
			this.fpSpread2.LeaveCell += new FarPoint.Win.Spread.LeaveCellEventHandler(fpSpread2_LeaveCell);			
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType3 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType4 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.fpSpread2 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread2_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel6 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel5 = new System.Windows.Forms.Panel();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).BeginInit();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 650);
            this.statusBar1.Size = new System.Drawing.Size(883, 24);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(883, 64);
            this.panel1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(482, 18);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "[本级编码]";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(396, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "current_code";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(269, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "[父级编码]";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(194, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "Parent_code";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.PaleGreen;
            this.label2.Location = new System.Drawing.Point(86, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Modify";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Bisque;
            this.label1.Location = new System.Drawing.Point(14, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "New";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Location = new System.Drawing.Point(0, 64);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(883, 584);
            this.panel2.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel7);
            this.panel4.Controls.Add(this.splitter2);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Controls.Add(this.splitter1);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(883, 584);
            this.panel4.TabIndex = 3;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.fpSpread2);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(477, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(406, 320);
            this.panel7.TabIndex = 5;
            // 
            // fpSpread2
            // 
            this.fpSpread2.About = "3.0.2004.2005";
            this.fpSpread2.AccessibleDescription = "";
            this.fpSpread2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread2.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread2.Location = new System.Drawing.Point(0, 0);
            this.fpSpread2.Name = "fpSpread2";
            this.fpSpread2.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread2_Sheet1});
            this.fpSpread2.Size = new System.Drawing.Size(406, 320);
            this.fpSpread2.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread2.TextTipAppearance = tipAppearance1;
            this.fpSpread2.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpSpread2_Sheet1
            // 
            this.fpSpread2_Sheet1.Reset();
            this.fpSpread2_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread2_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread2_Sheet1.ColumnCount = 2;
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "ID";
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "Name";
            textCellType1.ReadOnly = true;
            this.fpSpread2_Sheet1.Columns.Get(0).CellType = textCellType1;
            this.fpSpread2_Sheet1.Columns.Get(0).Label = "ID";
            this.fpSpread2_Sheet1.Columns.Get(0).Width = 280F;
            textCellType2.ReadOnly = true;
            this.fpSpread2_Sheet1.Columns.Get(1).CellType = textCellType2;
            this.fpSpread2_Sheet1.Columns.Get(1).Label = "Name";
            this.fpSpread2_Sheet1.Columns.Get(1).Width = 299F;
            this.fpSpread2_Sheet1.DataAutoCellTypes = false;
            this.fpSpread2_Sheet1.DataAutoSizeColumns = false;
            this.fpSpread2_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread2_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(474, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 320);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.button3);
            this.panel6.Controls.Add(this.button2);
            this.panel6.Controls.Add(this.button1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(360, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(114, 320);
            this.panel6.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(20, 156);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 55);
            this.button3.TabIndex = 2;
            this.button3.Text = "<-  Commit one Modify";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(20, 92);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 55);
            this.button2.TabIndex = 1;
            this.button2.Text = "<-  Commit All New";
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 55);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load sql.xml...";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(357, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 320);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.fpSpread1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(357, 320);
            this.panel5.TabIndex = 1;
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "";
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.Location = new System.Drawing.Point(0, 0);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(357, 320);
            this.fpSpread1.TabIndex = 0;
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
            this.fpSpread1_Sheet1.ColumnCount = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "ID";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "Name";
            textCellType3.ReadOnly = true;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = textCellType3;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "ID";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 251F;
            textCellType4.ReadOnly = true;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = textCellType4;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "Name";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 369F;
            this.fpSpread1_Sheet1.DataAutoCellTypes = false;
            this.fpSpread1_Sheet1.DataAutoSizeColumns = false;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.richTextBox2);
            this.panel3.Controls.Add(this.richTextBox1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 320);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(883, 264);
            this.panel3.TabIndex = 0;
            // 
            // richTextBox2
            // 
            this.richTextBox2.AcceptsTab = true;
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(473, 5);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(392, 254);
            this.richTextBox2.TabIndex = 2;
            this.richTextBox2.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.Location = new System.Drawing.Point(4, 7);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(446, 253);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // frmCompareSql
            // 
            this.ClientSize = new System.Drawing.Size(883, 674);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmCompareSql";
            this.Text = "SQL比较";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.statusBar1, 0);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		DataSet ds = new DataSet() ;
		private void frmCompareSql_Load(object sender, EventArgs e)
		{
			string sql = "select id ,name from com_sql order by id " ;

			Neusoft.HISFC.BizLogic.Registration.Register r = new Neusoft.HISFC.BizLogic.Registration.Register() ;
			r.ExecQuery(sql,ref  ds) ;

			
			this.fpSpread1_Sheet1.DataSource = ds ;

            //for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            //{
            //    string name = this.fpSpread1_Sheet1.GetText(i, 1);
            //    //				name = name.Replace("\r"," ") ;
            //    //				name = name.Replace("\t"," ") ;

            //    this.fpSpread1_Sheet1.SetValue(i, 1, name, false);
            //}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlgOpen = new OpenFileDialog();
			dlgOpen.InitialDirectory = Application.StartupPath;
			dlgOpen.Multiselect = false;
			dlgOpen.Title = "导入xml文件";
			dlgOpen.RestoreDirectory = true;
			dlgOpen.AddExtension = true;
			dlgOpen.DefaultExt = ".xml";
			dlgOpen.Filter = "xml files (*.xml)|*.xml";

			if(dlgOpen.ShowDialog() == DialogResult.OK)
			{
				string XmlFileName = dlgOpen.FileName;
				
				if(this.LoadSqlFromXml(XmlFileName) == -1)return ;

				this.Compare() ;
			}
		}

		private ArrayList alSql = new ArrayList() ;

		/// <summary>
		/// 从Xml加载Sql到AlSql
		/// </summary>
		/// <param name="XmlFileName"></param>
		/// <returns></returns>
		private int LoadSqlFromXml(string XmlFileName)
		{
			//ArrayList清空
			this.alSql.Clear();

			//打开文件
			System.Xml.XmlDataDocument doc=new System.Xml.XmlDataDocument();
			try
			{
				doc.Load(XmlFileName);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message) ;
				return -1;
			}

			//将每个Sql节点的数据放到ArrayList
			XmlNodeList nodes;
			nodes=doc.SelectNodes(@"//SQL");
			try
			{
				foreach(XmlNode node in nodes)
				{
					Neusoft.HISFC.Models.Base.Item objValue = new Neusoft.HISFC.Models.Base.Item() ;

					objValue.ID=node.Attributes[0].Value.ToString();
					string strSql = node.InnerText;

                    //strSql = strSql.Replace("\r", " ");
                    //strSql = strSql.Replace("\t", " ");
					objValue.Name=strSql;

					//modual
					if(node.ParentNode == null || node.ParentNode.ParentNode == null)
					{
						objValue.User01 = "" ;
					}
					else
					{
						objValue.User01 = node.ParentNode.ParentNode.Name;
					}

					//type
					if(node.ParentNode == null )
					{
						objValue.User02 = "" ;
					}
					else
					{
						objValue.User02 = node.ParentNode.Name;
					}
					
					//team
					if(node.ParentNode == null || node.ParentNode.ParentNode == null ||node.ParentNode.ParentNode.ParentNode == null)
					{
						objValue.User03 = "" ;
					}
					else
					{
						objValue.User03 = node.ParentNode.ParentNode.ParentNode.Name;
					}
					
					foreach(XmlAttribute att in node.Attributes)
					{
						switch(att.Name.ToLower())
						{
							case "input":
								objValue.UserCode = att.Value;
								break;
							case "output":
								objValue.WBCode = att.Value;
								break;
							case "isvalid":
								objValue.SpellCode = att.Value;
								break;
							case "memo":
								objValue.Memo = att.Value;
								break;
							default:
								break;
						}
					}

					this.alSql.Add(objValue);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message) ;
				return -1;
			}
			return 0;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private int Compare()
		{
            if (this.fpSpread2_Sheet1.RowCount > 0)
            {
                this.fpSpread2_Sheet1.Rows.Remove(0, this.fpSpread2_Sheet1.RowCount);
            }

			for(int i = this.alSql.Count -1; i>=0 ;i--)
			{
				Neusoft.HISFC.Models.Base.Item obj = (Neusoft.HISFC.Models.Base.Item)this.alSql[i] ;

				bool IsFound = false ;

				for(int j = 0; j <this.fpSpread1_Sheet1.RowCount ;j ++)
				{
					try
					{
						//
						if(obj.ID == this.fpSpread1_Sheet1.GetValue(j, 0).ToString())
						{
							if(obj.Name == this.fpSpread1_Sheet1.GetValue(j,1).ToString())
							{
                                this.alSql.Remove(obj);							
							}
							else//修改
							{
								this.fpSpread2_Sheet1.Rows.Add(this.fpSpread2_Sheet1.RowCount,1) ;
								int row = this.fpSpread2_Sheet1.RowCount - 1 ;

								this.fpSpread2_Sheet1.SetValue(row,0,obj.ID) ;
								this.fpSpread2_Sheet1.SetValue(row,1,obj.Name) ;

								this.fpSpread2_Sheet1.Rows[row].BackColor = Color.PaleGreen ;
								this.fpSpread2_Sheet1.Rows[row].Tag = obj ;
							}

							IsFound = true ;
							break;
						}
					}
					catch
					{}
				}

				if( IsFound == false)//新增
				{
					this.fpSpread2_Sheet1.Rows.Add(this.fpSpread2_Sheet1.RowCount,1) ;
					int row = this.fpSpread2_Sheet1.RowCount - 1 ;

					this.fpSpread2_Sheet1.SetValue(row,0,obj.ID) ;
					this.fpSpread2_Sheet1.SetValue(row,1,obj.Name) ;

					this.fpSpread2_Sheet1.Rows[row].BackColor = Color.Bisque ;

					this.fpSpread2_Sheet1.Rows[row].Tag = obj ;
				}
			}

			return 0 ;
		}
		

		private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
		{
			if(e.NewRow<0)return ;

			string sql = ds.Tables[0].Rows[e.NewRow][1].ToString()  ;
			if(sql == null) sql = "" ;
			sql = sql.Replace("\t"," ") ;
			sql = sql.Replace("\r"," ") ;

			this.richTextBox1.Text = sql ;
			
		}

		private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
		{
			if(e.NewRow<0)return ;

			//修改,跳到相应行
			if(this.fpSpread2_Sheet1.Rows[e.NewRow].BackColor == Color.PaleGreen)
			{
				for(int i = 0 ;i<this.fpSpread1_Sheet1.RowCount; i++)
				{
					if(this.fpSpread1_Sheet1.GetValue(i,0).ToString() == this.fpSpread2_Sheet1.GetValue(e.NewRow,0).ToString())
					{
						this.fpSpread1_Sheet1.ActiveRowIndex = i ;
						FarPoint.Win.Spread.LeaveCellEventArgs arg = new FarPoint.Win.Spread.LeaveCellEventArgs(null,0,0,i,0) ;
						this.fpSpread1_LeaveCell(null,arg) ;
						break;
					}
				}
			}
			 string sql = (this.fpSpread2_Sheet1.Rows[e.NewRow].Tag as Neusoft.HISFC.Models.Base.Item).Name;

			sql = sql.Replace("\t"," ") ;
			sql = sql.Replace("\r"," ") ;
			this.richTextBox2.Text = sql ;
		}

		private void button2_Click_1(object sender, System.EventArgs e)
		{		

			Neusoft.HISFC.BizLogic.Registration.Register r = new Neusoft.HISFC.BizLogic.Registration.Register() ;

			for(int i = this.fpSpread2_Sheet1.RowCount -1 ;i>= 0; i--)
			{
				if(this.fpSpread2_Sheet1.Rows[i].BackColor == Color.PaleGreen) continue;

				try
				{
					Neusoft.HISFC.Models.Base.Item obj = (Neusoft.HISFC.Models.Base.Item)this.fpSpread2_Sheet1.Rows[i].Tag ;

					string sql = "insert into com_sql (id,name,memo,type,modual,input,output,isvalid,"+
						"team,oper_code,oper_date) values ('{0}',:a,'{1}','{2}','{3}','{4}','{5}','{6}',"+
						"'{7}','hxw',sysdate)" ;

					//保存新增
					sql = string.Format(sql, obj.ID,obj.Memo,obj.User02,obj.User01,obj.UserCode,obj.WBCode,obj.SpellCode,obj.User03) ;

					if(r.InputLong(sql,obj.Name) == -1)
					{
						MessageBox.Show(r.Err,i.ToString() ) ;
						continue ;
					}

					this.fpSpread2_Sheet1.Rows.Remove(i,1) ;
				}
				catch
				{}
			}
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			//更新一条

			string sql = "insert into com_sql (id,name,memo,type,modual,input,output,isvalid,"+
				"team,oper_code,oper_date) values ('{0}',:a,'{1}','{2}','{3}','{4}','{5}','{6}',"+
				"'{7}','hxw',sysdate)" ;

			int i = this.fpSpread2_Sheet1.ActiveRowIndex ;
			if(i < 0||this.fpSpread2_Sheet1.RowCount == 0 )return ; 

			if(this.fpSpread2_Sheet1.Rows[i].BackColor == Color.Bisque) return ;

			Neusoft.HISFC.BizLogic.Registration.Register r = new Neusoft.HISFC.BizLogic.Registration.Register() ;
            

			Neusoft.HISFC.Models.Base.Item obj = (Neusoft.HISFC.Models.Base.Item)this.fpSpread2_Sheet1.Rows[i].Tag ;
            
			string del = "delete from com_sql where id ='"+obj.ID +"'" ;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            r.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

			try
			{
				if(r.ExecNoQuery(del) == -1)
				{
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
					MessageBox.Show(r.Err,i.ToString() ) ;
					return ;
				}

				sql = string.Format(sql, obj.ID,obj.Memo,obj.User02,obj.User01,obj.UserCode,obj.WBCode,obj.SpellCode,obj.User03) ;

				if(r.InputLong(sql,obj.Name) == -1)
				{
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
					MessageBox.Show(r.Err,i.ToString() ) ;
					return ;
				}

                Neusoft.FrameWork.Management.PublicTrans.Commit();

				this.fpSpread2_Sheet1.Rows.Remove(i,1) ;

				MessageBox.Show("OK" ) ;
			}
			catch(Exception ex)
			{
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
				MessageBox.Show(ex.Message) ;
				return ;
			}
		}
	}
}
