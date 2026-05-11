using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.HISFC.BizProcess.Integrate.Terminal;
using Neusoft.HISFC.Models.Registration;

namespace Neusoft.HISFC.Components.Terminal.Confirm
{
	/// <summary>
	/// ucPatientTree <br></br>
	/// [功能描述: 终端确认的患者树UC]<br></br>
	/// [创 建 者: 赫一阳]<br></br>
	/// [创建时间: 2006-03-07]<br></br>
	/// <修改记录
	///		修改人=''
	///		修改时间=''
	///		修改目的=''
	///		修改描述=''
	///  />
	/// </summary>
	public partial class ucPatientTree : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
	{
		public ucPatientTree()
		{
			InitializeComponent();
		}

		#region 变量
		
		/// <summary>
		/// 操作员当前科室
		/// </summary>
		string currentDepartment = "";

		/// <summary>
		/// 用于在总UC实现的代理和事件定义：用于实现单击患者结点显示患者基本信息和申请单明细的代理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void delegatePatientTree(object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e);
		
		/// <summary>
		/// 用于在总UC实现的代理和事件定义：用于实现单击患者结点显示患者基本信息和申请单明细的代理
		/// </summary>
		public event delegatePatientTree ClickTreeNode;

		/// <summary>
		/// 按键事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void delegateKeyDown(object sender, KeyEventArgs e);

		public event delegateKeyDown TreeNodeKeyDown;

		/// <summary>
		/// 窗口类型：1-门诊；2-住院；3-门诊预交金；9－通用
        /// {CEA4F0FA-012F-435a-8F38-5D8BC7E79754}
		/// </summary>
		string windowType = "1";

		/// <summary>
		/// 是否处于加载状态
		/// </summary>
		public bool isLoad = false;
		
		/// <summary>
		/// 当前鼠标移动到的树节点
		/// </summary>
		private TreeNode nodeCurrentMove = new TreeNode();
		
		#endregion

		#region 属性

		/// <summary>
		/// 操作员当前科室
		/// </summary>
		public string CurrentDepartment
		{
			get
			{
				return this.currentDepartment;
			}
			set
			{
				this.currentDepartment = value;
			}
		}
		
		/// <summary>
		/// 窗口类型：1-门诊；2-住院
		/// </summary>
		public string WindowType
		{
			get
			{
				return this.windowType;
			}
			set
			{
				this.windowType = value;
			}
		}
		
		#endregion

		#region 事件

		/// <summary>
		/// 鼠标移动事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeView1_MouseMove(object sender, MouseEventArgs e)
		{
			// 结点
			TreeNode treeNode = new TreeNode();
			// 显示的提示信息
			string tips = "";
			// 患者基本信息
			Neusoft.HISFC.Models.Registration.Register tempRegister;
			
			if (this.isLoad)
			{
				return;
			}
			//
			// 根据坐标获取结点
			//
			treeNode = this.treeView1.GetNodeAt(e.X, e.Y);
			if (treeNode == this.nodeCurrentMove)
			{
				return;
			}
			else
			{
				this.nodeCurrentMove = treeNode;
			}
			if (treeNode == null)
			{
				return;
			}
			// 如果获取结点成功
			if (treeNode.Level == 2)
			{
				// 转换结点为患者基本信息

				tempRegister = (Neusoft.HISFC.Models.Registration.Register)treeNode.Tag;
				string patientType = "";
				if (tempRegister.Memo == "1")
				{
					patientType = "门诊患者";
				}
				else if (tempRegister.Memo == "2")
				{
					patientType = "住院患者";
				}
				else if (tempRegister.Memo == "3")
				{
					patientType = "急诊患者";
				}
                else if (tempRegister.Memo == "4" || tempRegister.Memo == "5")
				{
					patientType = "体检患者";
				}
				// 构造提示信息

				tips +=   "患者姓名: " + tempRegister.Name;
				tips += "\n性    别: " + tempRegister.Sex.Name+" 年龄:"+tempRegister.Age;
				tips += "\n就诊科室: " + tempRegister.DoctorInfo.Templet.Dept.Name;
				tips += "\n病 历 号: " + tempRegister.PID.CardNO;
				tips += "\n患者类别: " + patientType;

				// 设置提示信息
				this.toolTip1.SetToolTip(this.treeView1, tips);
				// 显示提示信息
				this.toolTip1.Active = true;
			}
			else
			{
				// 关闭提示信息
				this.toolTip1.Active = false;
			}
		}

		/// <summary>
		/// 单击树节点
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			// 如果处于加载状态，那么返回
			if (this.isLoad)
			{
				return;
			}
            // 根据不通的窗口用途，执行不通的操作 {FDE4AA62-5FD7-4246-A130-FAC4E878583F}
			if (e.Node.Level == 2 && windowType != "3")
			{
				this.ClickTreeNode(sender, e);
			}
            else if (windowType == "3")
            {
                // {FDE4AA62-5FD7-4246-A130-FAC4E878583F}
                if (this.ClickTreeNode != null)
                {
                    this.ClickTreeNode(sender, e);
                }
            }
            else
            {
                return;
            }
		}

		/// <summary>
		/// 按键事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
            if (this.TreeNodeKeyDown != null)
            {
                this.TreeNodeKeyDown(sender, e);
            }
		}

		#endregion

		#region 函数

		/// <summary>
		/// 初始化患者树
		/// </summary>
		public void Init()
		{
			Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载患者信息...");
			Application.DoEvents();
            this.treeView1.ImageList = treeView1.deptImageList;
			// 初始化患者树
			if (windowType == "1")
			{
				// 检索门诊患者
				this.LoadOutpatient();
			}
			else if (windowType == "2")
			{
				// 检索住院患者基本信息
				this.LoadInpatient();
			}
            else if (windowType == "3")
            {
                // {FDE4AA62-5FD7-4246-A130-FAC4E878583F}
                // 不检索病人基本信息
                this.isLoad = false;
            }
            else
            {
                this.LoadOutpatient();
            }
		}

		/// <summary>
		/// 初始化/刷新门诊患者列表树
		/// </summary>
		public void LoadOutpatient()
		{
			Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("正在加载患者信息...");
			Application.DoEvents();
			// 业务层
			Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm confirmIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm ();
			// 患者树结点
			TreeNode treeNode = new TreeNode();
            treeNode.ImageIndex = 2;
            treeNode.SelectedImageIndex = 2;
			// 结果
			Neusoft.HISFC.BizProcess.Integrate.Terminal.Result result = new Result();

			this.isLoad = true;
			
			// 获取树结点
			result = confirmIntegrate.GetOutpatientTreeNode(ref treeNode);
			if (result == Neusoft.HISFC.BizProcess.Integrate.Terminal.Result.Failure)
			{
				MessageBox.Show(confirmIntegrate.Err);
				this.isLoad = false;
				return;
			}
			this.treeView1.Nodes.Clear();
			// 添加结点数组到树
			this.treeView1.Nodes.Add(treeNode);
			// 展开所有树结点
			this.treeView1.ExpandAll();
			
			this.isLoad = false;
			Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
		}

		/// <summary>
		/// 初始化/刷新住院患者列表数
		/// </summary>
		void LoadInpatient()
		{
			// 业务层
			Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm confirmIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm();
			// 患者树结点
			TreeNode treeNode = new TreeNode();
            treeNode.ImageIndex = 0;
            treeNode.SelectedImageIndex = 1;
			// 结果
			Neusoft.HISFC.BizProcess.Integrate.Terminal.Result result = new Result();

			this.isLoad = true;

			// 获取树结点
			result = confirmIntegrate.GetOutpatientTreeNode(ref treeNode);
			if (result == Neusoft.HISFC.BizProcess.Integrate.Terminal.Result.Failure)
			{
				MessageBox.Show(confirmIntegrate.Err);
				this.isLoad = false;
				return;
			}
			// 添加结点数组到树
			this.treeView1.Nodes.Add(treeNode);
			// 展开所有树结点
			this.treeView1.ExpandAll();

			this.isLoad = false;
		}

		/// <summary>
		/// 按键切换焦点到患者树
		/// </summary>
		public void SetFocus()
		{
			this.treeView1.Focus();
		}
		
		/// <summary>
		/// 获取当前节点
		/// </summary>
		/// <returns></returns>
		public System.Windows.Forms.TreeNode GetCurrentNode()
		{
			TreeNode currentNode = this.treeView1.SelectedNode;

			return currentNode;
		}
		
		#endregion

        #region 扩展--外部调用方法 {CEA4F0FA-012F-435a-8F38-5D8BC7E79754}
        /// <summary>
        /// 清空节点
        /// {CEA4F0FA-012F-435a-8F38-5D8BC7E79754}
        /// </summary>
        public void ClearPatient()
        {
            this.treeView1.Nodes.Clear();
        }
        /// <summary>
        /// 增加病人节点
        /// {CEA4F0FA-012F-435a-8F38-5D8BC7E79754}
        /// </summary>
        /// <param name="regPatient"></param>
        /// <returns></returns>
        public int AddNodes(Neusoft.HISFC.Models.Registration.Register regPatient)
        {
            if (regPatient == null || string.IsNullOrEmpty(regPatient.ID))
                return 0;

            TreeNode tnTemp = null;
            TreeNode tnTemp2 = null;
            TreeNode[] tnFindArr = null;

            // 查找挂号科室节点
            tnFindArr = treeView1.Nodes.Find("DEPT" + regPatient.DoctorInfo.Templet.Dept.ID, true);
            if (tnFindArr == null || tnFindArr.Length <= 0)
            {
                // 没找到相同科室节点 -- 添加科室节点
                tnTemp = new TreeNode();
                tnTemp.ImageIndex = 1;
                tnTemp.SelectedImageIndex = 0;

                tnTemp.Name = "DEPT" + regPatient.DoctorInfo.Templet.Dept.ID;
                tnTemp.Text = regPatient.DoctorInfo.Templet.Dept.Name;
                tnTemp.Tag = regPatient.DoctorInfo.Templet.Dept;

                treeView1.Nodes.Add(tnTemp);

                // 添加病人节点
                tnTemp2 = new TreeNode();
                tnTemp2.ImageIndex = 6;
                tnTemp2.SelectedImageIndex = 7;

                tnTemp2.Name = regPatient.ID;
                tnTemp2.Text = regPatient.Name + " [" + regPatient.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm") + "]";
                tnTemp2.Tag = regPatient;

                tnTemp.Nodes.Add(tnTemp2);

                treeView1.SelectedNode = tnTemp2;
            }
            else
            {
                tnTemp = tnFindArr[0];

                tnFindArr = treeView1.Nodes.Find(regPatient.ID, true);
                if (tnFindArr != null && tnFindArr.Length > 0)
                {
                    // 已存在节点
                    tnTemp2 = tnFindArr[0];
                    tnTemp2.Name = regPatient.ID;
                    tnTemp2.Text = regPatient.Name + " [" + regPatient.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm") + "]";
                    tnTemp2.Tag = regPatient;

                    treeView1.SelectedNode = tnTemp2;
                }
                else
                {
                    // 不存在，增加一个节点
                    tnTemp2 = new TreeNode();
                    tnTemp2.ImageIndex = 6;
                    tnTemp2.SelectedImageIndex = 7;
                    tnTemp2.Name = regPatient.ID;
                    tnTemp2.Text = regPatient.Name + " [" + regPatient.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm") + "]";
                    tnTemp2.Tag = regPatient;

                    tnTemp.Nodes.Add(tnTemp2);
                    treeView1.SelectedNode = tnTemp2;
                }
            }

            treeView1.ExpandAll();

            return 1;

        }

        /// <summary>
        /// 增加病人节点
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="arlRegister"></param>
        /// <returns></returns>
        public int AddNodes(ArrayList arlRegister)
        {
            if (arlRegister == null || arlRegister.Count <= 0)
                return 0;

            foreach (Neusoft.HISFC.Models.Registration.Register regPatient in arlRegister)
            {
                this.AddNodes(regPatient);
            }

            return 1;
        }

        public int AddNoDeptNodes(Neusoft.HISFC.Models.Registration.Register reg,ArrayList arlRegister)
        {

            if (reg == null || string.IsNullOrEmpty(reg.ID))
                return 0;

            bool isMutiNum =arlRegister!=null&&arlRegister.Count > 1 ? true : false;

            TreeNode tnTemp = null;
            TreeNode[] tnFindArr = null;
            // 查找挂号科室节点
            tnFindArr = treeView1.Nodes.Find(isMutiNum?reg.PID.CardNO:reg.ID, true);

            if (tnFindArr == null || tnFindArr.Length <= 0)
            {
                // 没找到相同科室节点 -- 添加科室节点
                tnTemp = new TreeNode();
                tnTemp.ImageIndex = 1;
                tnTemp.SelectedImageIndex = 0;
                tnTemp.Name = isMutiNum ? reg.PID.CardNO : reg.ID;
                tnTemp.Text = isMutiNum ? reg.Name + "[" + reg.DoctorInfo.SeeDate.ToString("yyyy年MM月") + "]" : reg.Name + "[" + reg.DoctorInfo.SeeDate.ToString("yyyy年MM月dd号HH点mm分") + "]";
                tnTemp.Tag = reg;
                treeView1.Nodes.Add(tnTemp);
            }
            else
            {
                tnTemp = tnFindArr[0];
            }

            TreeNode tnTemp2 = null;
            if (isMutiNum)
            {
                for (int i = arlRegister.Count - 1; i >= 0; i--)
                {
                    Neusoft.HISFC.Models.Registration.Register regPatient = arlRegister[i] as Neusoft.HISFC.Models.Registration.Register;
                    //判断是否加子节点
                    tnFindArr = tnTemp.Nodes.Find(regPatient.ID, true);
                    if (tnFindArr == null || tnFindArr.Length <= 0)
                    {
                        tnTemp2 = new TreeNode();
                        tnTemp2.ImageIndex = 1;
                        tnTemp2.SelectedImageIndex = 0;
                        tnTemp2.Name = isMutiNum ? regPatient.PID.CardNO : regPatient.ID;
                        tnTemp2.Text = regPatient.Name + "[" + regPatient.DoctorInfo.SeeDate.ToString("dd号HH点mm分") + "]" + regPatient.DoctorInfo.Templet.Doct.Name;
                        tnTemp2.Tag = regPatient;
                        tnTemp.Nodes.Add(tnTemp2);
                    }
                    else
                    {
                        tnTemp2 = tnFindArr[0];
                    }

                    if (regPatient.ID == reg.ID)
                    {
                        treeView1.SelectedNode = tnTemp2;
                    }

                }
            }
            else
            {
                treeView1.SelectedNode = tnTemp;
            }

            return 1;
        }
        #endregion
    }
}
