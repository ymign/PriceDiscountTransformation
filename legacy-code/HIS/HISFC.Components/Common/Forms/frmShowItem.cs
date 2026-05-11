using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 项目选择
    /// </summary>
    public partial class frmShowItem : Form
    {
        /// <summary>
        /// 构造
        /// </summary>
        public frmShowItem()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                lnkMore.Click += new EventHandler(lnkMore_Click);
                this.primitiveSize = this.Size;
                this.titlePanelSize = this.pnMain.Size;


                this.ckbSpell.CheckedChanged += new EventHandler(QueryTypeChanged);
                this.chbWB.CheckedChanged += new EventHandler(QueryTypeChanged);
                this.chkDeptItem.CheckedChanged += new EventHandler(QueryTypeChanged);
                this.cbxIsReal.CheckedChanged += new EventHandler(QueryTypeChanged);
            }
        }

        #region 变量

        //输入法
        public delegate void SelectWriteWay(int i);

        /// <summary>
        /// 配置设置
        /// </summary>
        Neusoft.HISFC.Components.Common.Controls.ucSetColumnForOrder ucSetXML = new Neusoft.HISFC.Components.Common.Controls.ucSetColumnForOrder();

        /// <summary>
        /// 控制参数
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam contrIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        /// <summary>
        /// 输入法
        /// </summary>
        public event SelectWriteWay writeWay = null;

        ///
        public bool isShowIsReal = true;

        //是否合作医疗
        public delegate void IsCompanyRang(bool flag);

        /// <summary>
        /// 是否合作医疗
        /// </summary>
        public event IsCompanyRang companyRang;

        //是否科常用
        public delegate void IsDeptUsedItem(bool flag);

        /// <summary>
        /// 是否科常用
        /// </summary>
        public event IsDeptUsedItem isDeptUsedItem = null;

        /// <summary>
        /// 门诊医生站刷新列表标记（社保标记）的方式：0 不刷新；1 使用多线程；2 单线程过滤刷新；3 初始化加载（根据合同单位调整社保标记列）
        /// </summary>
        public int IsUserThread = 0;

        private const int WM_NCLBUTTONDOWN = 0xA1;

        private const int HTCSPTION = 2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int wMsg, int wParm, int lParm);

        [DllImport("user32.dll")]
        private static extern int ReleaseCapture();

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage((int)this.Handle, WM_NCLBUTTONDOWN, HTCSPTION, 0);
        }
        FarPoint.Win.Spread.FpSpread sheetView = null;

        /// <summary>
        /// 是否按照五笔码过滤
        /// </summary>
        public bool IsFilterWBCode
        {
            get
            {
                return chbWB.Checked;
            }
        }

        /// <summary>
        /// 是否按照拼音码过滤
        /// </summary>
        public bool IsFilterSpellCode
        {
            get
            {
                return ckbSpell.Checked;
            }
        }

        /// <summary>
        /// 注意事项
        /// </summary>
        private string memo;
        public string Memo
        {
            get
            {
                return memo;
            }
            set
            {
                memo = value;
            }
        }

        #endregion

        public void Init()
        {
            if (DesignMode)
            {
                return;
            }
            if (this.sheetView != null)
            {
                this.sheetView.ColumnWidthChanged += new FarPoint.Win.Spread.ColumnWidthChangedEventHandler(sheetView_ColumnWidthChanged);
            }
            if (this.sheeViewDetail != null)
            {
                this.sheeViewDetail.ColumnWidthChanged += new FarPoint.Win.Spread.ColumnWidthChangedEventHandler(sheeViewDetail_ColumnWidthChanged);
            }

            SetDeptItem(false);

            isShowIsReal = contrIntegrate.GetControlParam<bool>("201026", false, true);
        }

        void frmShowItem_MouseWheel(object sender, MouseEventArgs e)
        {
            if (IsUserThread == 2)
            {
                Neusoft.HISFC.Models.SIInterface.Compare compareItem = null;

                if (this.ActiveControl.GetType().IsSubclassOf(typeof(FarPoint.Win.Spread.FpSpread)))
                {
                    Font font_Bold = new Font(((FarPoint.Win.Spread.FpSpread)ActiveControl).Font.FontFamily, ((FarPoint.Win.Spread.FpSpread)ActiveControl).Font.Size, FontStyle.Bold);
                    Font font_Regular = new Font(((FarPoint.Win.Spread.FpSpread)ActiveControl).Font.FontFamily, ((FarPoint.Win.Spread.FpSpread)ActiveControl).Font.Size, FontStyle.Regular);

                    //只刷新当前显示的行数
                    for (int i = ((FarPoint.Win.Spread.FpSpread)ActiveControl).GetViewportTopRow(0);
                        i < ((FarPoint.Win.Spread.FpSpread)ActiveControl).GetViewportBottomRow(0) + 1;
                        i++)
                    {
                        if (i >= ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowCount)
                        {
                            return;
                        }
                        if (((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Cells[i, (int)Neusoft.HISFC.Components.Common.Controls.EnumMainColumnSet.LackFlag].Text == "是")
                        {
                            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Rows[i].BackColor = Color.LightCoral;
                        }
                        else
                        {
                            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Rows[i].BackColor = Color.Transparent;
                        }

                        #region 显示医保等级信息

                        //if (Classes.Function.GetCompareItemInfo(((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Cells[i, (int)Neusoft.HISFC.Components.Common.Controls.EnumMainColumnSet.ItemCode].Text.Trim(), ref compareItem) == -1)
                        //{
                        //    ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Rows[i].Label = i.ToString();
                        //    ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].Font = font_Regular;
                        //    ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].ForeColor = ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].ColumnHeader.Columns[0].ForeColor;
                        //}
                        //else
                        //{
                        //    //医保标记
                        //    switch (compareItem.CenterItem.ItemGrade)
                        //    {
                        //        case "1":
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Rows[i].Label = "甲";
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].Font = font_Bold;
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].ForeColor = Color.Red;
                        //            break;
                        //        case "2":
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Rows[i].Label = "乙";
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].Font = font_Bold;
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].ForeColor = Color.Red;
                        //            break;
                        //        default:
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].Rows[i].Label = i.ToString();
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].Font = font_Regular;
                        //            ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].RowHeader.Rows[i].ForeColor = ((FarPoint.Win.Spread.FpSpread)ActiveControl).Sheets[0].ColumnHeader.Columns[0].ForeColor;
                        //            break;
                        //    }
                        //}
                        #endregion
                    }
                }
            }
        }

        void sheeViewDetail_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnFormatProperty(this.sheeViewDetail.Sheets[0], this.sheetDetailXMLFileName);
        }

        void sheetView_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnFormatProperty(this.sheetView.Sheets[0], this.sheetXMLFileName);
        }

        /// <summary>
        /// 项目明细列表配置文件
        /// </summary>
        protected string sheetDetailXMLFileName = Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath +
            Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "OrderItemDetail.xml";

        /// <summary>
        /// 项目列表配置文件
        /// </summary>
        protected string sheetXMLFileName = Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath +
            Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "OrderItem.xml";

        /// <summary>
        /// 
        /// </summary>
        private FarPoint.Win.Spread.FpSpread sheeViewDetail = null;

        
        /// <summary>
        /// 添加FarPoint
        /// </summary>
        /// <param name="c"></param>
        public void AddControl(Control c)
        {
            if (c.GetType().IsSubclassOf(typeof(FarPoint.Win.Spread.FpSpread)))
                sheetView = c as FarPoint.Win.Spread.FpSpread;

            c.Dock = DockStyle.Fill;
            //c.Height = Neusoft.FrameWork.Function.NConvert.ToInt32(((FarPoint.Win.Spread.FpSpread)c).Sheets[0].RowCount * ((FarPoint.Win.Spread.FpSpread)c).Sheets[0].Rows[0].Height) + 100;
            //c.Width = PanelMain.Width * 2;
            c.Visible = true;

            this.PanelMain.Controls.Add(c);
        }

        #region 设置用户默认配置

        /// <summary>
        /// 设置用户默认配置
        /// </summary>
        private void SaveUserDefaultSetting()
        {
            Neusoft.HISFC.BizLogic.Manager.UserDefaultSetting settingManager = new Neusoft.HISFC.BizLogic.Manager.UserDefaultSetting();

            string setting1 = "";
            string setting2 = "";
            string setting3 = "";
            string errInfo = "";
            if (this.GetUserDefaultSetting(ref setting1, ref setting2, ref setting3, ref errInfo) == -1)
            {
                MessageBox.Show(errInfo);
                return;
            }

            Neusoft.HISFC.Models.Base.UserDefaultSetting settingObj = settingManager.Query(settingManager.Operator.ID);

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            if (settingObj == null)
            {
                settingObj = new Neusoft.HISFC.Models.Base.UserDefaultSetting();
                settingObj.Empl.ID = settingManager.Operator.ID;

                settingObj.Setting1 = setting1;
                settingObj.Setting2 = setting2;
                settingObj.Setting3 = setting3;

                settingObj.Oper.ID = settingManager.Operator.ID;
                settingObj.Oper.OperTime = settingManager.GetDateTimeFromSysDateTime();

                if (settingManager.Insert(settingObj) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入用户设置信息出错" + settingManager.Err);
                    return;
                }
            }
            else
            {
                settingObj.Setting1 = setting1;
                settingObj.Setting2 = setting2;
                settingObj.Setting3 = setting3;

                settingObj.Oper.ID = settingManager.Operator.ID;
                settingObj.Oper.OperTime = settingManager.GetDateTimeFromSysDateTime();

                if (settingManager.Update(settingObj) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入用户设置信息出错" + settingManager.Err);
                    return;
                }
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
        }

        /// <summary>
        /// 设置是否使用科室常用项目
        /// </summary>
        /// <param name="isUseDeptItem">true 使用</param>
        public void SetDeptItem(bool isUseDeptItem)
        {
            this.chkDeptItem.CheckedChanged -= new EventHandler(QueryTypeChanged);
            this.chkDeptItem.Checked = isUseDeptItem;
            this.chkDeptItem.CheckedChanged += new EventHandler(QueryTypeChanged);
        }

        /// <summary>
        /// 设置查询输入法类别
        /// </summary>
        /// <param name="queryType"></param>
        public void SetQueryType(string queryType)
        {
            switch (queryType)
            {
                case "00":
                    this.ckbSpell.Checked = false;
                    this.chbWB.Checked = false;
                    break;
                case "01":
                    this.ckbSpell.Checked = false;
                    this.chbWB.Checked = true;
                    break;
                case "10":
                    this.ckbSpell.Checked = true;
                    this.chbWB.Checked = false;
                    break;
                case "11":
                    this.ckbSpell.Checked = true;
                    this.chbWB.Checked = true;
                    break;
                default:
                    this.ckbSpell.Checked = true;
                    this.chbWB.Checked = true;
                    break;
            }
        }

        /// <summary>
        /// 获取当前用户默认配置
        /// </summary>
        /// <param name="setting1">拼音码、五笔码</param>
        /// <param name="setting2">科室常用项目</param>
        /// <param name="setting3">是否精确查找</param>
        /// <param name="errInfo">错误信息</param>
        /// <returns></returns>
        private int GetUserDefaultSetting(ref string setting1, ref string setting2, ref string setting3, ref string errInfo)
        {
            try
            {
                //setting1 拼音码、五笔码
                setting1 = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ckbSpell.Checked).ToString() + Neusoft.FrameWork.Function.NConvert.ToInt32(this.chbWB.Checked).ToString();

                //科常用项目
                setting2 = Neusoft.FrameWork.Function.NConvert.ToInt32(this.chkDeptItem.Checked).ToString();

                //是否精确查找
                setting3 = Neusoft.FrameWork.Function.NConvert.ToInt32(this.cbxIsReal.Checked).ToString();
            }
            catch (Exception ex)
            {
                errInfo = ex.Message;
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 设置当前用户默认配置
        /// </summary>
        public void SetUserDefaultSetting()
        {
            Neusoft.HISFC.BizLogic.Manager.UserDefaultSetting settingManager = new Neusoft.HISFC.BizLogic.Manager.UserDefaultSetting();

            Neusoft.HISFC.Models.Base.UserDefaultSetting settingObj = settingManager.Query(settingManager.Operator.ID);

            if (settingObj != null)
            {
                this.SetQueryType(settingObj.Setting1);
                this.chkDeptItem.Checked = Neusoft.FrameWork.Function.NConvert.ToBoolean(string.IsNullOrEmpty(settingObj.Setting2) ? "0" : settingObj.Setting2);

                if (isShowIsReal)
                {
                    this.cbxIsReal.Checked = Neusoft.FrameWork.Function.NConvert.ToBoolean(string.IsNullOrEmpty(settingObj.Setting3) ? "0" : settingObj.Setting3);
                }
            }
        }
        #endregion

        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshFP()
        {
            if (sheeViewDetail == null && sheetView != null)//程序某些地方调用的时候sheeViewDetail可能为null，so加入此判断add by sunm
            {
                sheeViewDetail = new FarPoint.Win.Spread.FpSpread();
                sheeViewDetail.Sheets.Add(new FarPoint.Win.Spread.SheetView());
                sheeViewDetail.Sheets[0].ColumnCount = sheetView.Sheets[0].ColumnCount;
            }

            if (sheeViewDetail != null)
            {
                if (System.IO.File.Exists(this.sheetDetailXMLFileName))
                {
                    Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnFormatProperty(this.sheeViewDetail.Sheets[0], this.sheetDetailXMLFileName);
                }
                else
                {
                    Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnFormatProperty(this.sheeViewDetail.Sheets[0], this.sheetDetailXMLFileName);
                }
            }

            if (this.sheetView != null)
            {
                if (System.IO.File.Exists(this.sheetXMLFileName))
                {
                    Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnFormatProperty(this.sheetView.Sheets[0], this.sheetXMLFileName);
                }
                else
                {
                    Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnFormatProperty(this.sheetView.Sheets[0], this.sheetXMLFileName);
                }
            }

        }

        /// <summary>
        /// toolTip
        /// </summary>
        public string TipText
        {
            set
            {
                this.lblTip.Text = value;
            }
        }
        DataView dv = null;

        /// <summary>
        /// 当前视图
        /// </summary>
        public DataView DataView
        {
            set
            {
                this.dv = value;
            }
        }

        /// <summary>
        /// 是否精确查找
        /// </summary>
        public bool IsReal
        {
            get
            {
                return !this.cbxIsReal.Checked;
            }
            set
            {
                this.cbxIsReal.Checked = value;
            }
        }

        /// <summary>
        /// 是否外延药品
        /// </summary>
        public bool IsOUT
        {
            get
            {
                return this.outflag.Checked;
            }
            set
            {
                this.outflag.Checked = value;
            }
        }

        private void lblLis_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void lblSet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sheetView == null)
                return;
            ucSetXML = new Neusoft.HISFC.Components.Common.Controls.ucSetColumnForOrder();
            ucSetXML.SetColVisible(true, true, false, false);
            ucSetXML.SetDataTable(this.sheetDetailXMLFileName, sheetView.ActiveSheet);
            Neusoft.FrameWork.WinForms.Classes.Function.PopForm.TopMost = true;

            if (Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(ucSetXML) == DialogResult.OK)
            {
                Neusoft.HISFC.Components.Common.Controls.ucSetColumnForOrder.ReadXml(this.sheetDetailXMLFileName, sheetView.ActiveSheet);
            }
            Neusoft.FrameWork.WinForms.Classes.Function.PopForm.TopMost = false;
        }


        #region {9A40A1FE-C527-4f86-B6F5-E7F52FDD28C9}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lnkMore_Click(object sender, EventArgs e)
        {
            if (lnkMore.Text == "显示5条")
            {
                this.ResizeBottom();
                this.lnkMore.Text = "更多...";
            }
            else
            {
                //sheeViewDetail.Sheets[0].RowCount
                for (int i = 5; i < sheeViewDetail.Sheets[0].RowCount; i++)
                    sheeViewDetail.Sheets[0].SetRowVisible(i, true);
                this.resizebottomFP();
                this.lnkMore.Text = "显示5条";
            }
        }

        /// <summary>
        /// 全部控件的大小
        /// </summary>
        private Size primitiveSize;

        /// <summary>
        /// 不包含明细信息的控件大小
        /// </summary>
        private Size titlePanelSize;

        public void ResizeBottom()
        {
            sheeViewDetail.ActiveSheet.ColumnHeader.Visible = true;
            sheeViewDetail.ActiveSheet.Rows.Default.Height = 25F;
            sheeViewDetail.ActiveSheet.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.lnkMore.Visible = false;

            if (sheeViewDetail.Sheets[0].Rows.Count == 0 && string.IsNullOrEmpty(Memo))
            {
                this.pnlBottom.Visible = false;
                this.statusStrip1.Visible = false;

                this.Size = titlePanelSize;
            }
            else if (sheeViewDetail.Sheets[0].RowCount > 0 || !string.IsNullOrEmpty(Memo))
            {
                this.pnlBottom.Visible = true;
                this.statusStrip1.Visible = false;

                this.Size = this.primitiveSize;
            }
            else
            {
                this.pnlBottom.Visible = false;
                this.statusStrip1.Visible = false;

                this.Size = this.titlePanelSize;
            }
        }

        private void resizebottomFP()
        {
            //this.pnlBottom.Height = 140 + sheeViewDetail.Sheets[0].RowCount <= 10 ? sheeViewDetail.Sheets[0].RowCount : 10 * (20);
            sheeViewDetail.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Always;

        }

        /// <summary>
        /// 添加详细列表
        /// </summary>
        /// <param name="c"></param>
        public void AddBottomControl(Control c)
        {
            if (c.GetType().IsSubclassOf(typeof(FarPoint.Win.Spread.FpSpread)))
            {
                sheeViewDetail = c as FarPoint.Win.Spread.FpSpread;
                //sheeViewDetail.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Always;
                //sheeViewDetail.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Always;
                sheeViewDetail.Sheets[0].GrayAreaBackColor = pnlBottom.BackColor;
                sheeViewDetail.Sheets[0].DefaultStyle.BackColor = pnlBottom.BackColor;
                sheeViewDetail.Sheets[0].RowHeader.DefaultStyle.BackColor = pnlBottom.BackColor;
                this.statusbarText.Text = "显示的扩展信息供医生参考";
            }
            else
            {
                this.statusbarText.Text = "显示的扩展信息供医生参考";
            }
            c.Dock = DockStyle.Fill;
            c.Visible = true;
            this.pnlBottom.Controls.Add(c);

            //Neusoft.FrameWork.WinForms.Controls.NeuPanel cPanel = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            //cPanel.Dock = DockStyle.Fill;
            //cPanel.Controls.Add(c);

            //this.pnlBottom.Controls.Add(cPanel);
        }

        /// <summary>
        /// 添加注意事项
        /// </summary>
        /// <param name="c"></param>
        public void AddBottomMemoControl(Control c)
        {
            Neusoft.FrameWork.WinForms.Controls.NeuPanel memoPanel = null;
            if (c.GetType().IsSubclassOf(typeof(Neusoft.FrameWork.WinForms.Controls.NeuPanel)))
            {
                memoPanel = c as Neusoft.FrameWork.WinForms.Controls.NeuPanel;
                memoPanel.BackColor = pnlBottom.BackColor;
            }
            c.Size = new Size(700, 30);
            c.Dock = DockStyle.Top;
            c.Visible = true;

            this.pnlBottom.Controls.Add(c);
        }

        #endregion

        private void lklb_exit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
        }
        #region {E68EC2D3-2E6B-4062-A194-9E3C88B1AA98}
        private void ckbSpell_Click(object sender, EventArgs e)
        {
            if (this.ckbSpell.CheckState == CheckState.Checked)
            {
                this.ckbSpell.CheckState = CheckState.Checked;
                this.chbWB.CheckState = CheckState.Unchecked;
            }
            else
            {
                this.ckbSpell.CheckState = CheckState.Unchecked;
                this.chbWB.CheckState = CheckState.Checked;
            }
            // if (this.chbWB.CheckState == CheckState.Checked && this.ckbSpell.CheckState == CheckState.Checked)
            //{
            //    this.writeWay(2);
            //    return;
            //}
            this.writeWay(this.ckbSpell.CheckState == CheckState.Checked ? 0 : 1);
        }

        private void chbWB_Click(object sender, EventArgs e)
        {
            if (this.chbWB.CheckState == CheckState.Checked)
            {
                this.chbWB.CheckState = CheckState.Checked;
                this.ckbSpell.CheckState = CheckState.Unchecked;
            }
            else
            {
                this.chbWB.CheckState = CheckState.Unchecked;
                this.ckbSpell.CheckState = CheckState.Checked;
            }
            //if (this.chbWB.CheckState == CheckState.Checked && this.ckbSpell.CheckState == CheckState.Checked)
            //{
            //    this.writeWay(2);
            //    return;
            //}
            this.writeWay(this.ckbSpell.CheckState == CheckState.Checked ? 0 : 1);

        }
        #endregion

        private void chkCompantDrug_CheckedChanged(object sender, EventArgs e)
        {
            //if (this.chkCompantDrug.CheckState == CheckState.Checked)
            //{
            //    this.companyRang(true);
            //}
            //else
            //{
            //    this.companyRang(false);
            //}

        }

        private void chkDeptItem_Click(object sender, EventArgs e)
        {
        }
        public void hideDeptUsedCheckBox()
        {
            this.chkDeptItem.CheckState = CheckState.Unchecked;
        }
        public void showDeptUsedCheckBox()
        {
            this.chkDeptItem.CheckState = CheckState.Checked;
        }

        private void cbxIsReal_CheckedChanged(object sender, EventArgs e)
        {
            //this.IsReal = !this.cbxIsReal.Checked;
        }

        private void QueryTypeChanged(object sender, EventArgs e)
        {
            this.SaveUserDefaultSetting();
        }

        private void frmShowItem_Load(object sender, EventArgs e)
        {
            //this.SetDeptItem(false);
        }

        private void chkDeptItem_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkDeptItem.CheckState == CheckState.Checked)
            {
                if (this.isDeptUsedItem != null)
                {
                    this.isDeptUsedItem(true);
                }
            }
            if (this.chkDeptItem.CheckState == CheckState.Unchecked)
            {
                if (this.isDeptUsedItem != null)
                {
                    this.isDeptUsedItem(false);
                }
            }
        }

        private void outflag_CheckedChanged(object sender, EventArgs e)
        {
            if (this.outflag.CheckState == CheckState.Checked)
            {
                
                    this.IsOUT = true;
                    this.RefreshFP();
                
            }
            if (this.outflag.CheckState == CheckState.Unchecked)
            {
                
                    this.IsOUT = false;
                    this.RefreshFP();
                
            }
        }


    }
}