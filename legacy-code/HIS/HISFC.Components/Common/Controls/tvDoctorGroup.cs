using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
namespace Neusoft.HISFC.Components.Common.Controls
{
    public delegate void SelectOrderHandler(ArrayList alOrders);

    /// <summary>
    /// 医生站组套列表
    /// </summary>
    public partial class tvDoctorGroup : Neusoft.FrameWork.WinForms.Controls.NeuTreeView
    {
        public tvDoctorGroup()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                this.ImageList = imageList1;
            }
        }

        public tvDoctorGroup(IContainer container)
        {
            if (!DesignMode)
            {
                container.Add(this);
            }

            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 组套选择事件
        /// </summary>
        public event SelectOrderHandler SelectOrder;

        /// <summary>
        /// 科室组套
        /// </summary>
        protected Neusoft.FrameWork.Public.ObjectHelper deptHelper = null;

        Neusoft.HISFC.BizProcess.Integrate.Manager manager = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 个人组套
        /// </summary>
        protected Neusoft.FrameWork.Public.ObjectHelper personHelper = null;

        /// <summary>
        /// 是否有科室组套修改权限
        /// </summary>
        private bool isHaveDeptEditPower = false;

        /// <summary>
        /// 是否有全院组套修改权限
        /// </summary>
        private bool isHaveAllEditPower = false;

        /// <summary>
        /// 是否双击显示组套明细，0 为单击显示
        /// </summary>
        private int isDoubleClickShowSelect = -1;

        /// <summary>
        /// 是否模式对话框显示组套明细 0不是模式对话框
        /// </summary>
        private int isShowDetailDialog = -1;

        /// <summary>
        /// 是否允许左键修改诊断名称
        /// </summary>
        private int isLeftMouseEditGroupName = -1;

        /// <summary>
        /// 是否允许修改组套名称
        /// </summary>
        private bool isAllowEditGroupName = false;

        /// <summary>
        /// 修改前组套名称
        /// </summary>
        string groupOldName = "";

        /// <summary>
        /// 是否编辑组套模式
        /// </summary>
        private bool isEditGroup = false;

        /// <summary>
        /// 是否编辑组套模式
        /// </summary>
        public bool IsEditGroup
        {
            get
            {
                return isEditGroup;
            }
            set
            {
                isEditGroup = value;
            }
        }

        #region 属性

        /// <summary>
        /// 当前组套类型，医嘱
        /// </summary>
        protected enuType myType = enuType.Order;

        /// <summary>
        /// 当前组套类型，医嘱
        /// </summary>
        [DefaultValue(enuType.Order)]
        public enuType Type
        {
            get
            {
                return this.myType;
            }
            set
            {
                this.myType = value;
            }
        }

        /// <summary>
        /// 显示组套类型
        /// </summary>
        protected enuGroupShowType myShowType = enuGroupShowType.Dept;

        /// <summary>
        /// 显示组套类型
        /// </summary>
        [DefaultValue(enuGroupShowType.Dept)]
        public enuGroupShowType ShowType
        {
            get
            {
                return this.myShowType;
            }
            set
            {
                this.myShowType = value;
            }
        }

        /// <summary>
        /// 组套类别：门诊、住院
        /// </summary>
        protected Neusoft.HISFC.Models.Base.ServiceTypes inpatientType = Neusoft.HISFC.Models.Base.ServiceTypes.I;

        Neusoft.HISFC.BizLogic.Manager.Group groupManager = new Neusoft.HISFC.BizLogic.Manager.Group();

        /// <summary>
        /// 组套类别：门诊、住院
        /// </summary>
        [DefaultValue(Neusoft.HISFC.Models.Base.ServiceTypes.I)]
        public Neusoft.HISFC.Models.Base.ServiceTypes InpatientType
        {
            get
            {
                return inpatientType;
            }
            set
            {
                inpatientType = value;
            }
        }

        /// <summary>
        /// 合同单位信息
        /// </summary>
        private Neusoft.HISFC.Models.Base.PactInfo pact;

        /// <summary>
        /// 合同单位信息
        /// </summary>
        public Neusoft.HISFC.Models.Base.PactInfo Pact
        {
            set
            {
                pact = value;
            }
        }

        #endregion

        #endregion

        #region 函数

        /// <summary>
        /// 初始化函数
        /// </summary>
        public void Init()
        {
            try
            {
                this.personHelper = new Neusoft.FrameWork.Public.ObjectHelper(manager.QueryEmployeeAll());
                this.deptHelper = new Neusoft.FrameWork.Public.ObjectHelper(manager.QueryDeptmentsInHos(false));

                this.RefrshGroup();

                #region 组套修改权限判断

                Neusoft.HISFC.BizLogic.Order.Medical.Ability docAbility = new Neusoft.HISFC.BizLogic.Order.Medical.Ability();

                string errInfo = "";
                int ret = docAbility.CheckPopedom(docAbility.Operator.ID, "ALL", "groupManager", false, ref errInfo);

                if (ret > 0)
                {
                    this.isHaveAllEditPower = true;
                }
                else if (ret < 0)
                {
                    this.isHaveAllEditPower = false;
                    ShowErr(errInfo);
                    return;
                }
                else
                {
                    this.isHaveAllEditPower = false;
                }

                ret = docAbility.CheckPopedom(docAbility.Operator.ID, "DEPT", "groupManager", false, ref errInfo);

                if (ret > 0)
                {
                    this.isHaveDeptEditPower = true;
                }
                else if (ret < 0)
                {
                    this.isHaveDeptEditPower = false;
                    ShowErr(errInfo);
                    return;
                }
                else
                {
                    this.isHaveDeptEditPower = false;
                }
                #endregion

                if (this.isLeftMouseEditGroupName == -1)
                {
                    Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                    this.isLeftMouseEditGroupName = contrIntegrate.GetControlParam<int>("HNMZ38", true, 1);
                }
            }
            catch (Exception ex)
            {
                ShowErr(ex.Message);
            }
        }

        #region 初始化列表

        /// <summary>
        /// 刷新组套
        /// </summary>
        public void RefrshGroup()
        {
            if (this.myShowType == enuGroupShowType.Dept)
            {
                this.RefreshGroupMe();
            }
            else
            {
                this.RefreshGroupAll();
            }
        }

        /// <summary>
        /// 添加单个科室组套（全院、个人、科室）
        /// </summary>
        protected void RefreshGroupMe()
        {
            this.Nodes.Clear();
            TreeNode rootNode = new TreeNode(Neusoft.FrameWork.Management.Language.Msg("组套"));
            rootNode.ImageIndex = 0;
            rootNode.SelectedImageIndex = 1;
            rootNode.Tag = null;
            this.Nodes.Add(rootNode);

            #region 添加科室节点、个人、全院节点

            Neusoft.HISFC.Models.Base.Employee person = (Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator;

            //添加科室组套节点
            TreeNode deptNode = new TreeNode(Neusoft.FrameWork.Management.Language.Msg("科室组套"));
            deptNode.ImageIndex = 4;
            deptNode.SelectedImageIndex = 5;
            deptNode.Tag = null;
            rootNode.Nodes.Add(deptNode);

            //添加个人组套节点
            TreeNode perNode = new TreeNode(Neusoft.FrameWork.Management.Language.Msg("个人组套"));
            perNode.ImageIndex = 4;
            perNode.SelectedImageIndex = 5;
            perNode.Tag = null;
            rootNode.Nodes.Add(perNode);

            //添加全院组套节点
            TreeNode allNode = new TreeNode(Neusoft.FrameWork.Management.Language.Msg("全院组套"));
            allNode.ImageIndex = 4;
            allNode.SelectedImageIndex = 5;
            allNode.Tag = null;
            rootNode.Nodes.Add(allNode);


            #endregion

            #region "获得组套 获取当前科室的科室组套 当前操作员的个人组套  全院组套"

            //查询所有组套文件夹
            ArrayList alFolder = this.groupManager.GetAllFirstLVFolder(InpatientType, person.Dept.ID, person.ID);

            if (alFolder == null)
            {
                this.ShowErr(groupManager.Err);
                return;
            }

            //查询不在文件夹内的组套
            ArrayList al = this.groupManager.GetDeptOrderGroup(InpatientType, person.Dept.ID, person.ID);
            if (al == null)
            {
                this.ShowErr(groupManager.Err);
                return;
            }

            #endregion

            TreeNode node;
            Neusoft.HISFC.Models.Base.Group info;

            #region 添加组套文件夹及文件夹下的组套

            //对于组套文件夹  初始化时只加载2级目录，单击单个文件夹时，再加载多级目录

            try
            {
                for (int i = 0; i < alFolder.Count; i++)
                {
                    info = alFolder[i] as Neusoft.HISFC.Models.Base.Group;
                    if (info == null)
                    {
                        continue;
                    }

                    node = new TreeNode(info.Name);
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 3;
                    node.Tag = info;

                    switch (info.Kind)
                    {
                        case Neusoft.HISFC.Models.Base.GroupKinds.Dept:					//科室组套						
                            deptNode.Nodes.Add(node);
                            break;
                        case Neusoft.HISFC.Models.Base.GroupKinds.Doctor:					//个人组套
                            perNode.Nodes.Add(node);
                            break;
                        case Neusoft.HISFC.Models.Base.GroupKinds.All:					//全院组套
                            allNode.Nodes.Add(node);
                            break;
                    }

                    //查询单个文件夹下的组套
                    ArrayList alGroup = this.groupManager.GetGroupByFolderID(info.ID);
                    if (alGroup == null)
                    {
                        this.ShowErr(groupManager.Err);
                        continue;
                    }
                    else if (alGroup.Count == 0)
                    {
                        continue;
                    }

                    for (int j = 0; j < alGroup.Count; j++)
                    {
                        Neusoft.HISFC.Models.Base.Group group = alGroup[j] as Neusoft.HISFC.Models.Base.Group;
                        if (group == null)
                        {
                            continue;
                        }
                        TreeNode temNode = new TreeNode(group.Name);
                        temNode.ImageIndex = 10;
                        temNode.SelectedImageIndex = 11;
                        temNode.Tag = group;
                        node.Nodes.Add(temNode);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErr(ex.Message);
                return;
            }
            #endregion

            #region 添加文件夹外的单个组套
            try
            {
                for (int i = 0; i < al.Count; i++)
                {
                    info = al[i] as Neusoft.HISFC.Models.Base.Group;
                    if (info == null)
                    {
                        continue;
                    }

                    node = new TreeNode(info.Name);
                    node.ImageIndex = 10;
                    node.SelectedImageIndex = 11;
                    node.Tag = info;

                    switch (info.Kind)
                    {
                        case Neusoft.HISFC.Models.Base.GroupKinds.Dept:					//科室组套						
                            deptNode.Nodes.Add(node);
                            break;
                        case Neusoft.HISFC.Models.Base.GroupKinds.Doctor:					//个人组套
                            perNode.Nodes.Add(node);
                            break;
                        case Neusoft.HISFC.Models.Base.GroupKinds.All:					//全院组套
                            allNode.Nodes.Add(node);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErr(ex.Message);
                return;
            }
            #endregion

            this.Nodes[0].Expand();

            //科室组套
            this.Nodes[0].Nodes[0].Expand();
            //个人组套
            this.Nodes[0].Nodes[1].Expand();
            //全院组套
            this.Nodes[0].Nodes[2].Expand();
        }

        /// <summary>
        /// 所有组套信息
        /// </summary>
        public ArrayList AllGroup = null;

        /// <summary>
        /// 显示全院所有科室组套列表  这个基本没有，就不管了先
        /// </summary>
        protected void RefreshGroupAll()
        {
            this.Nodes.Clear();
            TreeNode rootNode = new TreeNode("全院组套");
            rootNode.ImageIndex = 0;
            rootNode.SelectedImageIndex = 1;
            this.Nodes.Add(rootNode);

            ArrayList al = groupManager.GetAllOrderGroup(inpatientType);
            AllGroup = al;
            if (al == null)
                return;

            ArrayList alDepts = new ArrayList();

            ArrayList alDept = manager.GetDepartment();

            Neusoft.HISFC.Models.Base.Department obj = null;
            for (int i = 0; i < alDept.Count; i++)
            {
                obj = alDept[i] as Neusoft.HISFC.Models.Base.Department;

                if (this.myType == enuType.Order)
                {
                    if (obj.DeptType.ID.ToString() == "I")
                    {
                        alDepts.Add(obj);
                    }
                }
                else if (this.myType == enuType.Fee)
                {
                    if (obj.DeptType.ID.ToString() == "N" || obj.DeptType.ToString() == "F")
                    {
                        alDepts.Add(obj);
                    }
                }
                else if (this.myType == enuType.Terminal)
                {
                    if (obj.DeptType.ID.ToString() == "T")
                    {
                        alDepts.Add(obj);
                    }
                }
            }

            AddDeptGroup(rootNode, alDepts);
            addPerson();
            addNodes(al);
            this.Nodes[0].Expand();
        }

        /// <summary>
        /// 添加科室组套
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="alDepts"></param>
        private static void AddDeptGroup(TreeNode rootNode, ArrayList alDepts)
        {
            for (int i = 0; i < alDepts.Count; i++)
            {
                Neusoft.FrameWork.Models.NeuObject obj = alDepts[i] as Neusoft.FrameWork.Models.NeuObject;
                TreeNode node = new TreeNode(obj.Name);
                node.ImageIndex = 2;
                node.SelectedImageIndex = 3;
                node.Tag = obj.ID;

                TreeNode cnode = new TreeNode("科室组套");
                cnode.ImageIndex = 4;
                cnode.SelectedImageIndex = 5;
                node.Nodes.Add(cnode);

                TreeNode cnode1 = new TreeNode("医生组套");
                cnode1.ImageIndex = 4;
                cnode1.SelectedImageIndex = 5;
                node.Nodes.Add(cnode1);

                rootNode.Nodes.Add(node);
            }
        }

        /// <summary>
        /// 添加所有人员
        /// </summary>
        private void addPerson()
        {
            for (int i = 0; i < personHelper.ArrayObject.Count; i++)
            {
                Neusoft.HISFC.Models.Base.Employee obj = personHelper.ArrayObject[i] as Neusoft.HISFC.Models.Base.Employee;
                TreeNode deptNode = this.GetDeptNodeByTag(obj.Dept.ID);
                if (deptNode == null)
                {

                }
                else
                {
                    TreeNode node = new TreeNode(obj.Name);
                    node.ImageIndex = 6;
                    node.SelectedImageIndex = 7;
                    node.Tag = obj.ID;
                    deptNode.Nodes[1].Nodes.Add(node);
                }
            }
        }

        private void addNodes(ArrayList al)
        {
            for (int i = 0; i < al.Count; i++)
            {
                Neusoft.HISFC.Models.Base.Group info = al[i] as Neusoft.HISFC.Models.Base.Group;
                TreeNode myRoot = null;
                myRoot = this.GetDeptNodeByTag(info.Dept.ID);
                if (info.Kind == Neusoft.HISFC.Models.Base.GroupKinds.Dept)//科室属性
                {
                    if (myRoot != null)
                    {
                        TreeNode node = new TreeNode(info.Name);
                        node.ImageIndex = 10;
                        node.SelectedImageIndex = 11;
                        node.Tag = info;
                        myRoot.Nodes[0].Nodes.Add(node);
                    }
                }
                else
                {
                    if (myRoot != null)
                    {
                        myRoot = this.GetDocNodeByDeptNode(myRoot, info.Doctor.ID);
                        if (myRoot != null)
                        {
                            TreeNode node = new TreeNode(info.Name);
                            node.Tag = info;
                            node.ImageIndex = 10;
                            node.SelectedImageIndex = 11;
                            myRoot.Nodes.Add(node);
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 获得科室结点
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private TreeNode GetDeptNodeByTag(string tag)
        {
            foreach (TreeNode node in this.Nodes[0].Nodes)
            {
                if (node.Tag != null && node.Tag.ToString() == tag)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得人员结点
        /// </summary>
        /// <param name="deptNode"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private TreeNode GetDocNodeByDeptNode(TreeNode deptNode, string tag)
        {
            foreach (TreeNode node in deptNode.Nodes[1].Nodes)
            {
                if (node.Tag != null && node.Tag.ToString() == tag)
                {
                    return node;
                }
            }
            return null;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 是否正在显示组套明细
        /// </summary>
        bool isShowGroupDetailing = false;

        /// <summary>
        /// 是否正在显示组套明细
        /// </summary>
        public bool IsShowGroupDetailing
        {
            get
            {
                return isShowGroupDetailing;
            }
            set
            {
                isShowGroupDetailing = value;

                if (isShowGroupDetailing)
                {
                    this.Enabled = false;
                }
                else
                {
                    this.Enabled = true;
                }
            }
        }

        /// <summary>
        /// 鼠标右键事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.SelectedNode = this.GetNodeAt(e.X, e.Y);

            if (this.isLeftMouseEditGroupName == 1)
            {
                if (this.SelectedNode == null || this.SelectedNode.Tag == null)
                    this.LabelEdit = false;
                else
                    this.LabelEdit = true;
            }

            if (this.SelectedNode == this.Nodes[0])
            {
                return;
            }

            try
            {
                ContextMenu m = null;

                #region 左键事件

                if (e.Button == MouseButtons.Left)
                {
                    if (isDragDropEvent)
                    {
                        return;
                    }
                    if (this.isDoubleClickShowSelect == -1)
                    {
                        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                        this.isDoubleClickShowSelect = contrIntegrate.GetControlParam<int>("HNMZ36", true, 1);
                    }

                    if (isDoubleClickShowSelect == 1)
                    {
                        return;
                    }

                    if (this.SelectedNode.Tag != null
                        && this.SelectedNode.Tag.GetType() == typeof(Neusoft.HISFC.Models.Base.Group))
                    {
                        Neusoft.HISFC.Models.Base.Group groupSelected = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;
                        //文件夹
                        if (groupSelected.UserCode != "F")
                        {
                            this.ShowGroupDetail();
                        }
                    }
                }
                #endregion

                #region 右键事件

                if (e.Button == MouseButtons.Right)
                {
                    if (this.fSelect != null && fSelect.Visible)
                    {
                        this.fSelect.Close();
                    }

                    MenuItem AddItem = new MenuItem("增加文件夹");
                    AddItem.Click += new EventHandler(AddItem_Click);

                    MenuItem deleteItem = new MenuItem("删除");
                    deleteItem.Click += new EventHandler(deleteItem_Click);

                    MenuItem ChangeGroupName = new MenuItem("修改组套名称");
                    ChangeGroupName.Click += new EventHandler(ChangeGroupName_Click);

                    //最上级节点
                    if (this.SelectedNode.Tag == null)
                    {
                        m = new ContextMenu();
                        m.MenuItems.Add(AddItem);
                        m.MenuItems.Add(deleteItem);

                        this.ContextMenu = m;
                        this.ContextMenu.Show(this, new Point(e.X, e.Y));
                    }
                    else
                    {
                        if (this.SelectedNode.Tag.GetType() == typeof(Neusoft.HISFC.Models.Base.Group))
                        {
                            Neusoft.HISFC.Models.Base.Group groupSelected = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;
                            //文件夹
                            if (groupSelected.UserCode == "F")
                            {
                                if (this.SelectedNode.Nodes.Count > 0)
                                {
                                    m = new ContextMenu();
                                    m.MenuItems.Add(AddItem);
                                    m.MenuItems.Add(ChangeGroupName);

                                    this.ContextMenu = m;
                                    this.ContextMenu.Show(this, new Point(e.X, e.Y));
                                }
                                else
                                {
                                    m = new ContextMenu();
                                    m.MenuItems.Add(AddItem);
                                    m.MenuItems.Add(deleteItem);
                                    m.MenuItems.Add(ChangeGroupName);

                                    this.ContextMenu = m;
                                    this.ContextMenu.Show(this, new Point(e.X, e.Y));
                                }
                            }
                            else
                            {
                                m = new ContextMenu();
                                m.MenuItems.Add(deleteItem);

                                if (groupSelected.Kind == Neusoft.HISFC.Models.Base.GroupKinds.All)
                                {
                                    if (this.isHaveDeptEditPower)
                                    {
                                        MenuItem copyToDeptGroup = new MenuItem("复制为科室组套");
                                        copyToDeptGroup.Click += new EventHandler(copyAllToDeptGroup_Click);
                                        m.MenuItems.Add(copyToDeptGroup);
                                    }

                                    MenuItem copyAllToPersonGroup = new MenuItem("复制为个人组套");
                                    copyAllToPersonGroup.Click += new EventHandler(copyAllToPersonGroup_Click);
                                    m.MenuItems.Add(copyAllToPersonGroup);

                                    m.MenuItems.Add(ChangeGroupName);
                                }
                                else if (groupSelected.Kind == Neusoft.HISFC.Models.Base.GroupKinds.Dept)
                                {
                                    if (this.isHaveAllEditPower)
                                    {
                                        MenuItem copyDeptToAllGroup = new MenuItem("复制为全院组套");
                                        copyDeptToAllGroup.Click += new EventHandler(copyDeptToAllGroup_Click);
                                        m.MenuItems.Add(copyDeptToAllGroup);
                                    }
                                    MenuItem copyDeptToPersonGroup = new MenuItem("复制为个人组套");
                                    copyDeptToPersonGroup.Click += new EventHandler(copyDeptToPersonGroup_Click);
                                    m.MenuItems.Add(copyDeptToPersonGroup);

                                    m.MenuItems.Add(ChangeGroupName);
                                }
                                else
                                {
                                    if (this.isHaveAllEditPower)
                                    {
                                        MenuItem copyPersonToAllGroup = new MenuItem("复制为全院组套");
                                        copyPersonToAllGroup.Click += new EventHandler(copyPersonToAllGroup_Click);
                                        m.MenuItems.Add(copyPersonToAllGroup);
                                    }

                                    if (this.isHaveDeptEditPower)
                                    {
                                        MenuItem copyPersonToDeptGroup = new MenuItem("复制为科室组套");
                                        copyPersonToDeptGroup.Click += new EventHandler(copyPersonToDeptGroup_Click);
                                        m.MenuItems.Add(copyPersonToDeptGroup);
                                    }

                                    m.MenuItems.Add(ChangeGroupName);
                                }

                                this.ContextMenu = m;
                                this.ContextMenu.Show(this, new Point(e.X, e.Y));
                            }
                        }
                        else
                        {
                            this.ContextMenu = null;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                this.ShowErr(ex.Message);
            }
        }

        #region 复制组套

        /// <summary>
        /// 科室组套复制为全院组套
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyDeptToAllGroup_Click(object sender, EventArgs e)
        {
            this.SaveGroup(Neusoft.HISFC.Models.Base.GroupKinds.All);
        }

        /// <summary>
        /// 科室组套复制为个人组套
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyDeptToPersonGroup_Click(object sender, EventArgs e)
        {
            this.SaveGroup(Neusoft.HISFC.Models.Base.GroupKinds.Doctor);
        }

        /// <summary>
        /// 全院组套复制为个人组套
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyAllToPersonGroup_Click(object sender, EventArgs e)
        {
            this.SaveGroup(Neusoft.HISFC.Models.Base.GroupKinds.Doctor);
        }

        /// <summary>
        /// 全院组套复制为科室组套
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyAllToDeptGroup_Click(object sender, EventArgs e)
        {
            this.SaveGroup(Neusoft.HISFC.Models.Base.GroupKinds.Dept);
        }

        /// <summary>
        /// 个人组套复制为全院组套
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyPersonToAllGroup_Click(object sender, EventArgs e)
        {
            this.SaveGroup(Neusoft.HISFC.Models.Base.GroupKinds.All);
        }

        /// <summary>
        /// 个人组套复制为科室组套
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyPersonToDeptGroup_Click(object sender, EventArgs e)
        {
            this.SaveGroup(Neusoft.HISFC.Models.Base.GroupKinds.Dept);
        }


        /// <summary>
        /// 保存组套
        /// </summary>
        /// <param name="groupKind"></param>
        private void SaveGroup(Neusoft.HISFC.Models.Base.GroupKinds groupKind)
        {
            //新组套
            Neusoft.HISFC.Models.Base.Group newGroup = (this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group).Clone();
            ArrayList alDetail = groupManager.GetAllItem(newGroup);
            if (alDetail == null)
            {
                this.ShowErr("查找组套明细失败:" + groupManager.Err);
                return;
            }

            //1 门诊 2 住院
            string outOrIn = newGroup.UserType == Neusoft.HISFC.Models.Base.ServiceTypes.C ? "1" : "2";

            newGroup.Kind = groupKind;
            newGroup.ID = groupManager.GetNewGroupID();
            newGroup.Dept = ((Neusoft.HISFC.Models.Base.Employee)groupManager.Operator).Dept;
            newGroup.Doctor = groupManager.Operator;
            newGroup.ParentID = null;

            //组套备注，用于排序 {2E2FE2A6-3C9C-431e-908F-77B5B941E5F9} houwb
            if (string.IsNullOrEmpty(newGroup.Memo))
            {
                newGroup.Memo = this.groupManager.GetMaxGroupSortID();
            }

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            if (groupManager.UpdateGroup(newGroup) == -1)
            {
                FrameWork.Management.PublicTrans.RollBack();
                this.ShowErr("保存组套失败:" + groupManager.Err);
                return;
            }

            //门诊
            if (outOrIn == "1")
            {
                foreach (Neusoft.HISFC.Models.Order.OutPatient.Order order in alDetail)
                {
                    order.Item.ID = order.ID;
                    if (groupManager.UpdateGroupItem(newGroup, order) == -1)
                    {
                        FrameWork.Management.PublicTrans.RollBack();
                        this.ShowErr("保存组套明细失败:" + groupManager.Err);
                        return;
                    }
                }
            }
            //住院
            else
            {
                foreach (Neusoft.HISFC.Models.Order.Inpatient.Order order in alDetail)
                {
                    order.Item.ID = order.ID;

                    if (groupManager.UpdateGroupItem(newGroup, order) == -1)
                    {
                        FrameWork.Management.PublicTrans.RollBack();
                        this.ShowErr("保存组套明细失败:" + groupManager.Err);
                        return;
                    }
                }
            }

            FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("保存成功");
            this.RefrshGroup();
        }

        #endregion

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.ContextMenu != null)
            {
                if (this.ContextMenu.MenuItems.Count > 0)
                {
                    this.ContextMenu.MenuItems.Clear();
                }
            }

            //this.SelectedNode = this.GetNodeAt(e.X, e.Y);

            //if (this.isLeftMouseEditGroupName == 1)
            //{
            //    if (this.SelectedNode == null || this.SelectedNode.Tag == null)
            //        this.LabelEdit = false;
            //    else
            //        this.LabelEdit = true;
            //}

            //if (this.SelectedNode == this.Nodes[0])
            //{
            //    return;
            //}

            //try
            //{
            //    ContextMenu m = null;

            //    #region 左键事件

            //    if (e.Button == MouseButtons.Left)
            //    {
            //        if (this.isDoubleClickShowSelect == -1)
            //        {
            //            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
            //            this.isDoubleClickShowSelect = contrIntegrate.GetControlParam<int>("HNMZ36", true, 1);
            //        }

            //        if (isDoubleClickShowSelect == 1)
            //        {
            //            return;
            //        }

            //        if (this.SelectedNode.Tag.GetType() == typeof(Neusoft.HISFC.Models.Base.Group))
            //        {
            //            Neusoft.HISFC.Models.Base.Group groupSelected = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;
            //            //文件夹
            //            if (groupSelected.UserCode != "F")
            //            {
            //                this.ShowGroupDetail();
            //            }
            //        }
            //    }
            //    #endregion
            //}
            //catch (Exception ex)
            //{
            //    this.ShowErr(ex.Message);
            //}
        }

        /// <summary>
        /// 修改组套名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangeGroupName_Click(object sender, EventArgs e)
        {
            isAllowEditGroupName = true;

            if (this.SelectedNode.Tag != null)
            {
                this.LabelEdit = true;
                groupOldName = this.SelectedNode.Text;

                this.SelectedNode.BeginEdit();
            }
        }

        /// <summary>
        /// 增加文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddItem_Click(object sender, EventArgs e)
        {
            #region 组套修改判断

            Neusoft.HISFC.Models.Base.Group info = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;

            if (info != null && info.Kind == Neusoft.HISFC.Models.Base.GroupKinds.Dept && !this.isHaveDeptEditPower)
            {
                this.ShowErr("你没有修改科室组套的权限!");
                return;
            }

            if (info != null && info.Kind == Neusoft.HISFC.Models.Base.GroupKinds.All && !this.isHaveAllEditPower)
            {
                this.ShowErr("你没有修改全院组套的权限!");
                return;
            }

            #endregion

            TreeNode node = new TreeNode();
            node.ImageIndex = 2;
            node.SelectedImageIndex = 3;
            Neusoft.HISFC.Models.Base.Group group = new Neusoft.HISFC.Models.Base.Group();
            group.ID = this.groupManager.GetNewFolderID();
            group.Name = "新建文件夹";
            group.UserType = this.inpatientType;

            if (this.SelectedNode == this.Nodes[0].Nodes[0])
            {
                group.Kind = Neusoft.HISFC.Models.Base.GroupKinds.Dept;
                group.Dept.ID = (this.groupManager.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID;
                group.Doctor = this.groupManager.Operator;
                group.ParentID = "ROOT";
            }
            else if (this.SelectedNode == this.Nodes[0].Nodes[1])
            {
                group.Kind = Neusoft.HISFC.Models.Base.GroupKinds.Doctor;
                group.Dept.ID = (this.groupManager.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID;
                group.Doctor = this.groupManager.Operator;
                group.ParentID = "ROOT";
            }
            else if (this.SelectedNode == this.Nodes[0].Nodes[2])
            {
                group.Kind = Neusoft.HISFC.Models.Base.GroupKinds.All;
                group.Dept.ID = "ALL";
                group.Doctor = this.groupManager.Operator;
                group.ParentID = "ROOT";
            }
            else
            {
                Neusoft.HISFC.Models.Base.Group groupSelected = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;

                group.Kind = groupSelected.Kind;
                group.Dept = groupSelected.Dept;

                group.Doctor = this.groupManager.Operator;
                group.ParentID = groupSelected.ID;
            }

            group.UserCode = "F";

            if (this.groupManager.SetNewFolder(group) < 0)
            {
                ShowErr("增加文件夹失败！");
                return;
            }
            node.Text = group.Name;
            node.Tag = group;
            this.SelectedNode.Nodes.Add(node);
        }

        /// <summary>
        /// 提示错误信息
        /// </summary>
        /// <param name="errInfo"></param>
        private void ShowErr(string errInfo)
        {
            MessageBox.Show(errInfo, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                #region 组套修改判断

                Neusoft.HISFC.Models.Base.Group info = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;

                if (info != null && info.Kind == Neusoft.HISFC.Models.Base.GroupKinds.All && !this.isHaveAllEditPower)
                {
                    this.ShowErr("你没有修改全院组套的权限!");
                    return;
                }
                if (info != null && info.Kind == Neusoft.HISFC.Models.Base.GroupKinds.Dept && !this.isHaveDeptEditPower)
                {
                    this.ShowErr("你没有修改科室组套的权限!");
                    return;
                }

                #endregion

                if (info.UserCode == "F")//文件夹
                {
                    if (MessageBox.Show("是否删除文件夹" + info.Name, "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (this.groupManager.deleteFolder(info) < 0)
                        {
                            ShowErr(this.groupManager.Err);
                        }
                        this.RefrshGroup();
                    }
                }
                else
                {
                    if (MessageBox.Show("是否删除组套" + info.Name, "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        /*
                         * 以单独的权限来维护
                         * 
                        if (!(this.groupManager.Operator as Neusoft.HISFC.Models.Base.Employee).IsManager &&
                            info.Kind == Neusoft.HISFC.Models.Base.GroupKinds.All)
                        {
                            MessageBox.Show("您不是管理员,没有权限删除组套", "提示");
                            return;
                        }
                        */
                        if (this.groupManager.DeleteGroup(info) == -1)
                        {
                            ShowErr(this.groupManager.Err);
                        }
                        this.RefrshGroup();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 选中后刷新下面子节点内容
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (isDragDropEvent)
            {
                return;
            }
            object o = this.SelectedNode.Tag;
            if (o != null)
            {
                if (o.GetType() == typeof(Neusoft.HISFC.Models.Base.Group))
                {
                    //if (this.SelectedNode.Nodes.Count > 0)
                    //{
                    //    return;
                    //}
                    this.SelectedNode.Nodes.Clear();
                    Neusoft.HISFC.Models.Base.Group info = o as Neusoft.HISFC.Models.Base.Group;
                    if (info.UserCode == "F")
                    {
                        ArrayList alFolder = this.groupManager.GetAllFolderByFolderID(info.ID);

                        if (alFolder == null)
                        {
                            return;
                        }

                        try
                        {
                            TreeNode node;

                            Neusoft.HISFC.Models.Base.Group myGroup;
                            for (int i = 0; i < alFolder.Count; i++)
                            {
                                myGroup = alFolder[i] as Neusoft.HISFC.Models.Base.Group;
                                if (info == null)
                                {
                                    continue;
                                }
                                node = new TreeNode(myGroup.Name);
                                node.ImageIndex = 2;
                                node.SelectedImageIndex = 3;
                                node.Tag = myGroup;


                                switch (myGroup.Kind)
                                {
                                    case Neusoft.HISFC.Models.Base.GroupKinds.Dept:					//科室组套						
                                        this.SelectedNode.Nodes.Add(node);
                                        break;
                                    case Neusoft.HISFC.Models.Base.GroupKinds.Doctor:					//个人组套
                                        this.SelectedNode.Nodes.Add(node);
                                        break;
                                    case Neusoft.HISFC.Models.Base.GroupKinds.All:					//全院组套
                                        this.SelectedNode.Nodes.Add(node);
                                        break;
                                }
                            }
                            ArrayList alGroup = this.groupManager.GetGroupByFolderID(info.ID);
                            if (alGroup != null && alGroup.Count > 0)
                            {
                                for (int j = 0; j < alGroup.Count; j++)
                                {
                                    Neusoft.HISFC.Models.Base.Group group = alGroup[j] as Neusoft.HISFC.Models.Base.Group;
                                    if (group == null)
                                    {
                                        continue;
                                    }
                                    TreeNode temNode = new TreeNode(group.Name);
                                    temNode.ImageIndex = 10;
                                    temNode.SelectedImageIndex = 11;
                                    temNode.Tag = group;
                                    this.SelectedNode.Nodes.Add(temNode);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            ShowErr(ex.Message);
                            return;
                        }
                    }
                }
            }
            base.OnAfterSelect(e);
        }

        /// <summary>
        /// 名字被编辑之前
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            if (this.isLeftMouseEditGroupName != 1 && !isAllowEditGroupName)
            {
                return;
            }

            if (this.SelectedNode == null)
            {
                this.groupOldName = "";
            }

            this.groupOldName = this.SelectedNode.Text;

            base.OnBeforeLabelEdit(e);
        }

        /// <summary>
        /// 名称被编辑
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (this.isLeftMouseEditGroupName != 1 && !isAllowEditGroupName)
            {
                return;
            }

            if (e.Label == null)
            {
                this.LabelEdit = false;
                return;
            }

            Neusoft.HISFC.Models.Base.Group group = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;

            if (group.Kind == Neusoft.HISFC.Models.Base.GroupKinds.Dept && !this.isHaveDeptEditPower)
            {
                this.ShowErr("你没有修改科室组套的权限!");
                this.SelectedNode.Text = this.groupOldName;
                return;
            }
            if (group.Kind == Neusoft.HISFC.Models.Base.GroupKinds.All && !this.isHaveAllEditPower)
            {
                this.ShowErr("你没有修改全院组套的权限!");
                this.SelectedNode.Text = this.groupOldName;
                return;
            }

            DialogResult r = MessageBox.Show("节点名称已经修改，是否保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (r == DialogResult.Cancel)
            {
                this.LabelEdit = false;
                this.SelectedNode.Text = this.groupOldName;
                this.RefrshGroup();
                return;
            }

            if ((this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group).UserCode == "F")
            {
                Neusoft.HISFC.Models.Base.Group tem = this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group;
                tem.Name = e.Label;
                if (this.groupManager.updateFolder(tem) <= 0)
                {
                    MessageBox.Show("文件夹名称更新失败·", "提示");
                }
                else
                {
                    MessageBox.Show("文件夹名称更新成功！", "提示");
                }
            }
            else
            {
                string GroupId = (this.SelectedNode.Tag as Neusoft.HISFC.Models.Base.Group).ID;
                if (groupManager.UpdateGroupName(GroupId, e.Label) > 0)
                    MessageBox.Show("组套名称更新成功", "提示");
                else
                {
                    MessageBox.Show("更新失败", "提示");
                }
            }

            this.LabelEdit = false;
        }

        /// <summary>
        /// 选中节点后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvDoctorGroup_AfterSelect(object sender, TreeViewEventArgs e)
        {
            return;

            if (this.isDoubleClickShowSelect == -1)
            {
                Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                this.isDoubleClickShowSelect = contrIntegrate.GetControlParam<int>("HNMZ36", true, 1);
            }

            if (isDoubleClickShowSelect == 1)
            {
                return;
            }

            this.ShowGroupDetail();
        }

        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.isDoubleClickShowSelect == -1)
            {
                Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                this.isDoubleClickShowSelect = contrIntegrate.GetControlParam<int>("HNMZ36", true, 0);
            }

            if (isDoubleClickShowSelect != 1)
            {
                return;
            }

            this.ShowGroupDetail();
        }

        Forms.frmSelectGroup fSelect = null;

        /// <summary>
        /// 显示组套明细
        /// </summary>
        private void ShowGroupDetail()
        {
            object o = this.SelectedNode.Tag;
            if (o != null)
            {
                if (o.GetType() == typeof(Neusoft.HISFC.Models.Base.Group))
                {
                    Neusoft.HISFC.Models.Base.Group info = o as Neusoft.HISFC.Models.Base.Group;
                    //文件夹
                    if (info.UserCode == "F")
                    {
                        this.SelectedNode.Expand();
                        return;
                    }

                    if (this.isShowDetailDialog == -1)
                    {
                        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                        this.isShowDetailDialog = contrIntegrate.GetControlParam<int>("HNMZ37", true, 1);
                    }

                    if (fSelect == null || fSelect.IsDisposed)
                    {
                        fSelect = new Neusoft.HISFC.Components.Common.Forms.frmSelectGroup();
                        fSelect.IsShowDetailDialog = isShowDetailDialog;

                        fSelect.IsEditGroup = this.isEditGroup;
                        fSelect.Pact = this.pact;

                        fSelect.Init(info);

                        if (this.isShowDetailDialog == 0)
                        {
                            fSelect.GroupConfirm -= this.AfterSelectOrder;
                            fSelect.GroupConfirm += this.AfterSelectOrder;
                        }
                    }
                    else
                    {

                        fSelect.IsEditGroup = this.isEditGroup;
                        fSelect.Pact = this.pact;
                        fSelect.Init(info);
                    }

                    fSelect.InpatientType = this.inpatientType;
                    fSelect.IsHaveAllEditPower = this.isHaveAllEditPower;
                    fSelect.IsHaveDeptEditPower = this.isHaveDeptEditPower;

                    // {57D49CC0-3BEE-4168-8E71-4FAE394DF6A6}  内镜中心流程改造
                    fSelect.Text = "组套选择  【" + info.Name + "】" + info.ID;

                    if (this.isShowDetailDialog == 1)
                    {
                        fSelect.ShowDialog();

                        if (fSelect.DialogResult == DialogResult.OK)
                        {
                            try
                            {
                                if (SelectOrder != null)
                                {
                                    SelectOrder(fSelect.Orders);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.ShowErr(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        fSelect.Visible = false;
                        fSelect.Show();
                        fSelect.TopMost = true;
                    }
                }
            }
        }

        private void AfterSelectOrder()
        {
            try
            {
                if (SelectOrder != null)
                {
                    SelectOrder(fSelect.Orders);
                }
            }
            catch (Exception ex)
            {
                this.ShowErr(ex.Message);
            }
        }

        private void tvDoctorGroup_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = System.Windows.Forms.DragDropEffects.Move;
        }

        private void tvDoctorGroup_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            System.Drawing.Point position = new Point(0, 0);
            position.X = e.X;
            position.Y = e.Y;
            position = this.PointToClient(position);
            TreeNode dropNode = this.GetNodeAt(position);
            this.isDragDropEvent = true;
            this.SelectedNode = dropNode;
            isDragDropEvent = false;
            this.Focus();
        }

        private void tvDoctorGroup_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //开始进行"Drag"操作
                DoDragDrop((TreeNode)e.Item, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// 是否是拖动事件，拖动时不重新加载文件夹下的节点，否则从文件夹内拖动到最外层节点有问题
        /// </summary>
        bool isDragDropEvent = false;

        /// <summary>
        /// 移动节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvDoctorGroup_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            TreeNode temp = new TreeNode();

            //得到要移动的节点
            TreeNode moveNode = (TreeNode)e.Data.GetData(temp.GetType());
            //转换坐标为控件treeview的坐标
            Point position = new Point(0, 0);
            position.X = e.X;
            position.Y = e.Y;
            position = this.PointToClient(position);

            //得到移动的目的地的节点
            TreeNode aimNode = this.GetNodeAt(position);
            if (aimNode == null)//超出区域 返回
            {
                return;
            }

            Neusoft.HISFC.Models.Base.Group fromGroup = moveNode.Tag as Neusoft.HISFC.Models.Base.Group;
            Neusoft.HISFC.Models.Base.Group toGroup = aimNode.Tag as Neusoft.HISFC.Models.Base.Group;

            if (fromGroup == null) //是组套父节点 返回
            {
                return;
            }
            if (fromGroup.UserCode == "F")//是文件夹节点 返回
            {
                MessageBox.Show("文件夹不允许移动！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return;
            }
            if (toGroup != null && fromGroup.ParentID == toGroup.ID)
            {
                MessageBox.Show("目标节点已经是父级节点，不需要移动！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (toGroup == null)//目标节点 是父节点
            {
                if (aimNode.Text == "科室组套")
                {
                    toGroup = new Neusoft.HISFC.Models.Base.Group();
                    toGroup.ID = "";
                    toGroup.Name = "科室组套";
                    toGroup.UserCode = "F";
                    toGroup.Kind = Neusoft.HISFC.Models.Base.GroupKinds.Dept;
                }
                else if (aimNode.Text == "个人组套")
                {
                    toGroup = new Neusoft.HISFC.Models.Base.Group();
                    toGroup.ID = "";
                    toGroup.Name = "个人组套";
                    toGroup.UserCode = "F";
                    toGroup.Kind = Neusoft.HISFC.Models.Base.GroupKinds.Doctor;
                }
                else if (aimNode.Text == "全院组套")
                {
                    toGroup = new Neusoft.HISFC.Models.Base.Group();
                    toGroup.ID = "";
                    toGroup.Name = "全院组套";
                    toGroup.UserCode = "F";
                    toGroup.Kind = Neusoft.HISFC.Models.Base.GroupKinds.All;
                }
                //return;
            }

            #region 组套修改判断

            if (fromGroup.Kind == Neusoft.HISFC.Models.Base.GroupKinds.Dept && !this.isHaveDeptEditPower)
            {
                this.ShowErr("你没有修改科室组套的权限!");
                return;
            }
            if (fromGroup.Kind == Neusoft.HISFC.Models.Base.GroupKinds.All && !this.isHaveAllEditPower)
            {
                this.ShowErr("你没有修改全院组套的权限!");
                return;
            }

            if (toGroup != null &&
                toGroup.Kind == Neusoft.HISFC.Models.Base.GroupKinds.Dept && !this.isHaveDeptEditPower)
            {
                this.ShowErr("你没有修改科室组套的权限!");
                return;
            }
            if (toGroup != null
                && toGroup.Kind == Neusoft.HISFC.Models.Base.GroupKinds.All && !this.isHaveAllEditPower)
            {
                this.ShowErr("你没有修改全院组套的权限!");
                return;
            }

            #endregion

            if (toGroup != null && fromGroup.Kind != toGroup.Kind)//科室和个人之间不允许拖
            {
                this.ShowErr("不同类别间不允许拖动！");
                return;
            }

            //目标节点也是组套，置换位置
            if (toGroup.UserCode != "F")//目标节点不是文件夹
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                //组套排序功能 {2E2FE2A6-3C9C-431e-908F-77B5B941E5F9} houwb
                if (this.groupManager.UpdateGroupSortID(fromGroup.ID, toGroup.Memo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新组套序号失败：" + groupManager.Err);
                    return;
                }

                //组套排序功能 {2E2FE2A6-3C9C-431e-908F-77B5B941E5F9} houwb
                if (this.groupManager.UpdateGroupSortID(toGroup.ID, fromGroup.Memo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新组套序号失败：" + groupManager.Err);
                    return;
                }
                string memoTemp = toGroup.Memo;
                //置换备注
                toGroup.Memo = fromGroup.Memo;
                fromGroup.Memo = memoTemp;
                //置换父节点
                memoTemp = toGroup.ParentID;
                toGroup.ParentID = fromGroup.ParentID;
                fromGroup.ParentID = memoTemp;

                Neusoft.FrameWork.Models.NeuObject groupObjTemp = new Neusoft.FrameWork.Models.NeuObject();
                groupObjTemp = fromGroup.Clone();
                moveNode.Tag = toGroup;
                moveNode.Text = toGroup.Name;
                aimNode.Tag = groupObjTemp;
                aimNode.Text = groupObjTemp.Name;

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                return;
            }

            if (IsDragEnable(aimNode, moveNode) == true)
            {
                if (aimNode != moveNode)
                {
                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                    //Neusoft.HISFC.Models.Base.Group temGroup = aimNode.Tag as Neusoft.HISFC.Models.Base.Group;
                    //Neusoft.HISFC.Models.Base.Group tempGroup = moveNode.Tag as Neusoft.HISFC.Models.Base.Group;

                    if (fromGroup == null || toGroup == null)
                    {
                        return;
                    }
                    try
                    {
                        if (this.groupManager.UpdateGroupFolderID(fromGroup.ID, toGroup.ID) < 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            this.ShowErr("拖动组套到文件夹失败:" + groupManager.Err);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        this.ShowErr("拖动组套到文件夹失败:" + groupManager.Err);
                        return;
                    }
                    Neusoft.FrameWork.Management.PublicTrans.Commit();

                    this.Nodes.Remove(moveNode);
                    aimNode.Nodes.Add(moveNode);
                    fromGroup.ParentID = toGroup.ID;
                    moveNode.Tag = fromGroup;

                    //this.RefrshGroup();
                }
            }
        }

        /// <summary>
        /// 判断是否可以拖动动目标节点，如果可以则返回true，否则为false;
        /// 判断根据是：目标节点不能是被拖动的节点的父亲节点！
        /// </summary>
        private bool IsDragEnable(TreeNode aimNode, TreeNode oriNode)
        {
            while (aimNode != null)
            {
                if (aimNode.Parent != oriNode)
                {
                    aimNode = aimNode.Parent;
                    IsDragEnable(aimNode, oriNode);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

    }

    /// <summary>
    /// 显示类型
    /// </summary>
    public enum enuGroupShowType
    {
        /// <summary>
        /// 全院组套
        /// </summary>
        All = 0,

        /// <summary>
        /// 科室组套
        /// </summary>
        Dept = 1,

        /// <summary>
        /// 个人组套
        /// </summary>
        Person = 1
    }

    /// <summary>
    /// 门诊or住院
    /// </summary>
    public enum enuInpatientType
    {
        /// <summary>
        /// 门诊
        /// </summary>
        C = 0,

        /// <summary>
        /// 住院
        /// </summary>
        I = 1
    }

    /// <summary>
    /// 组套
    /// </summary>
    public enum enuType
    {
        /// <summary>
        /// 医嘱
        /// </summary>
        Order = 0,

        /// <summary>
        /// 费用
        /// </summary>
        Fee = 1,

        /// <summary>
        /// 终端？？
        /// </summary>
        Terminal = 2
    }
}
