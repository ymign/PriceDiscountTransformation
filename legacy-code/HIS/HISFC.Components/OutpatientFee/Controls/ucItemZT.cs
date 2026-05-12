using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// DR或者CT组合项目明细合并维护
    /// </summary>
    public partial class ucItemZT : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IMaintenanceControlable
    {
        public ucItemZT()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 常数管理业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Constant consManager = new Neusoft.HISFC.BizLogic.Manager.Constant();

        /// <summary>
        /// 交叉管理业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Manager interManager = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        private Neusoft.HISFC.BizLogic.Manager.UndrugztManager ztManager = new Neusoft.HISFC.BizLogic.Manager.UndrugztManager();

        /// <summary>
        /// DR或者CT组合项目明细合并
        /// </summary>
        private string constPhaDoseOnce = "ItemZT";

        /// <summary>
        /// 组套列表
        /// </summary>
        private ArrayList alZTItem = null;

        /// <summary>
        /// 组套列表
        /// </summary>
        private ArrayList alItem = null;

        /// <summary>
        /// 非药品组合基本信息帮助类
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper itemHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 非药品基本信息帮助类
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper undrugHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 配置文件
        /// </summary>
        private string xmlProfile = Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + "\\Profiles\\ItemZTSet.xml";

        private int sysCode = 0;

        #endregion

        #region 方法

        /// <summary>
        /// 初始化列表
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            this.neuSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(neuSpread1_CellDoubleClick);

            this.neuSpread1.ColumnWidthChanged += new FarPoint.Win.Spread.ColumnWidthChangedEventHandler(neuSpread1_ColumnWidthChanged);

            this.neuSpread1.KeyPress += new KeyPressEventHandler(neuSpread1_KeyPress);

            #region 初始化列表

            //组套
            alZTItem = feeIntegrate.QueryValidZTItems();
            if (alZTItem == null)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("查询非药品组合列表发生错误！" + feeIntegrate.Err));
                return -1;
            }
            this.itemHelper = new Neusoft.FrameWork.Public.ObjectHelper(alZTItem);
            this.ucInputItem1.Init();
            
            //非药品
            alItem = Neusoft.SOC.HISFC.BizProcess.Cache.Fee.GetValidItem();//Neusoft.HISFC.Components.Common.Controls.ucInputItem.alItem;//feeIntegrate.QueryValidItems();
            if (alItem == null)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("查询非药品组合列表发生错误！" + feeIntegrate.Err));
                return -1;
            }
            this.undrugHelper = new Neusoft.FrameWork.Public.ObjectHelper(alItem);

            
            #endregion

            //this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.SysCode].Visible = false;

            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.ItemCode].Locked = true;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.ItemName].Locked = true;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.A1].Locked = true;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Index].Locked = true;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.SysCode].Locked = true;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Oper].Locked = true;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.OperTime].Locked = true;

            if (System.IO.File.Exists(this.xmlProfile))
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.ReadColumnProperty(neuSpread1_Sheet1, this.xmlProfile);
            }
            else
            {
                Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.neuSpread1_Sheet1, xmlProfile);
            }

            #region 下拉列表
            //组范围：0 DR 1 CT
            string[] arrayTemp = new string[2] { "DR","CT" };
            FarPoint.Win.Spread.CellType.ComboBoxCellType comCellType1 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            comCellType1.Items = arrayTemp;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Type].CellType = comCellType1;

            //组范围：0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
            arrayTemp = new string[4] { "每个项目收取", "第一个项目收取", "第二个项目起加收", "只收取一次" };
            FarPoint.Win.Spread.CellType.ComboBoxCellType comCellType2 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            comCellType2.Items = arrayTemp;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Math1].CellType = comCellType2;

            //组范围：0 总量取整、1 单个取整 2固定数量
            arrayTemp = new string[3] { "总量取整","单个取整","固定数量" };
            FarPoint.Win.Spread.CellType.ComboBoxCellType comCellType3 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            comCellType3.Items = arrayTemp;
            this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Rule].CellType = comCellType3;

            #endregion

            sysCode = this.feeIntegrate.QueryMaxCode();
            if (sysCode < 0 || sysCode == null)
            {
                sysCode = 0;
            }

            return this.Query();
        }

        private void neuSpread1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                if (this.neuSpread1_Sheet1.ActiveColumnIndex == (int)SubtblColumns.A1)
                {
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, this.neuSpread1_Sheet1.ActiveColumnIndex].Tag = null;
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, this.neuSpread1_Sheet1.ActiveColumnIndex].Text = "";
                }
            }
        }

        void neuSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            Neusoft.FrameWork.WinForms.Classes.CustomerFp.SaveColumnProperty(this.neuSpread1_Sheet1, xmlProfile);
        }

        /// <summary>
        /// 双击弹出选择项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neuSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.ColumnHeader || e.RowHeader)
            {
                return;
            }
            if (e.Column == (int)SubtblColumns.SysCode || e.Column == (int)SubtblColumns.ItemCode || e.Column == (int)SubtblColumns.ItemName)
            {
                this.PopItem(this.alZTItem, "1","");
            }
            else if (e.Column == (int)SubtblColumns.A1)
            {
                string undrugID = string.Empty;
                if (this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.SysCode].Value == null)
                {
                    return;
                }
                undrugID = this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.SysCode].Value.ToString();
                if (string.IsNullOrEmpty(undrugID.Trim()))
                {
                    return;
                }
                ArrayList alTemp = null;
                List<Neusoft.HISFC.Models.Fee.Item.UndrugComb> itemList = new List<Neusoft.HISFC.Models.Fee.Item.UndrugComb>();
                this.ztManager.QueryUnDrugztDetail(undrugID, ref itemList);
                alTemp = new ArrayList(itemList.ToArray());
                if (alTemp != null && alTemp.Count > 0)
                {
                    this.PopItem(alTemp, "2", e.Column.ToString());
                }
            }
        }

        /// <summary>
        /// 弹出常数选择
        /// </summary>
        private void PopItem(ArrayList al, string type, string column)
        {
            Neusoft.FrameWork.Models.NeuObject info = new Neusoft.FrameWork.Models.NeuObject();
            if (Neusoft.FrameWork.WinForms.Classes.Function.ChooseItem(al, ref info) == 0)
            {
                return;
            }
            else
            {
                if (type == "1")
                {
                    Neusoft.HISFC.Models.Fee.Item.Undrug itemObj = itemHelper.GetObjectFromID(info.ID) as Neusoft.HISFC.Models.Fee.Item.Undrug;
                    if (itemObj != null)
                    {
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.SysCode].Value = itemObj.ID;
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.ItemCode].CellType = new FarPoint.Win.Spread.CellType.TextCellType();
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.ItemCode].Value = itemObj.UserCode;
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.ItemName].Value = itemObj.Name;

                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.A1].Tag = null;
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.A1].Text = "";

                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.Qty1].Tag = null;
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.Qty1].Text = "";

                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.Math1].Tag = null;
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.Math1].Text = "";
                    }
                }
                else if (type == "2")
                {
                    if (info != null)
                    {
                        if (this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, Neusoft.FrameWork.Function.NConvert.ToInt32(column)].Tag == null)
                        {
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, Neusoft.FrameWork.Function.NConvert.ToInt32(column)].Tag = info.ID;
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, Neusoft.FrameWork.Function.NConvert.ToInt32(column)].Text = info.Name;

                        }
                        else
                        {
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, Neusoft.FrameWork.Function.NConvert.ToInt32(column)].Tag += "|" + info.ID;
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, Neusoft.FrameWork.Function.NConvert.ToInt32(column)].Text += "|" + info.Name;

                        }
                    }
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.neuSpread1.ContainsFocus)
            {
                if (keyData == Keys.Space)
                {
                    if (this.neuSpread1_Sheet1.ActiveColumnIndex == (int)SubtblColumns.SysCode || this.neuSpread1_Sheet1.ActiveColumnIndex == (int)SubtblColumns.ItemCode || this.neuSpread1_Sheet1.ActiveColumnIndex == (int)SubtblColumns.ItemName)
                    {
                        this.PopItem(this.alZTItem, "1", "");
                    }
                    else if (this.neuSpread1_Sheet1.ActiveColumnIndex == (int)SubtblColumns.A1)
                    {
                        string undrugID = string.Empty;
                        if (this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.SysCode].Value == null)
                        {
                            return true;
                        }
                        undrugID = this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.SysCode].Value.ToString();
                        if (string.IsNullOrEmpty(undrugID.Trim()))
                        {
                            return true;
                        }
                        ArrayList alTemp = null;
                        List<Neusoft.HISFC.Models.Fee.Item.UndrugComb> itemList = new List<Neusoft.HISFC.Models.Fee.Item.UndrugComb>();
                        this.ztManager.QueryUnDrugztDetail(undrugID, ref itemList);
                        alTemp = new ArrayList(itemList.ToArray());
                        if (alTemp != null && alTemp.Count > 0)
                        {
                            this.PopItem(alTemp, "2", this.neuSpread1_Sheet1.ActiveColumnIndex.ToString());
                        }
                    }
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public int Query()
        {
            this.neuSpread1_Sheet1.RowCount = 0;

            ArrayList alUndrug = this.consManager.GetAllList(this.constPhaDoseOnce);

            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = null;
            myCompare compare = new myCompare();
            alUndrug.Sort(compare);

            int i = 0;
            foreach (Neusoft.HISFC.Models.Base.Const conObj in alUndrug)
            {

                undrug = itemHelper.GetObjectFromID(conObj.Name) as Neusoft.HISFC.Models.Fee.Item.Undrug;

                if (undrug == null)
                {
                    continue;
                }
                i++;
                this.neuSpread1_Sheet1.Rows.Add(0, 1);
                //序号
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Index].Text = conObj.ID;
                //系统编码
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.SysCode].Text = conObj.Name;
                //类型
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Type].Text = (this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Type].CellType as FarPoint.Win.Spread.CellType.ComboBoxCellType).Items[Neusoft.FrameWork.Function.NConvert.ToInt32(conObj.UserCode)];
                //取整规则
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Rule].Text = (this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Rule].CellType as FarPoint.Win.Spread.CellType.ComboBoxCellType).Items[Neusoft.FrameWork.Function.NConvert.ToInt32(conObj.SpellCode)];
                     
                //组合编码
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.ItemCode].CellType = new FarPoint.Win.Spread.CellType.TextCellType();
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.ItemCode].Text = undrug.UserCode;
                //组合名称
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.ItemName].Text = undrug.Name;
                //A1
                if (!string.IsNullOrEmpty(conObj.Memo))
                {
                    //string[] strRoot = conObj.Memo.Split('&');
                    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Qty1].Text = conObj.WBCode;
                    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Math1].Text = (this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Math1].CellType as FarPoint.Win.Spread.CellType.ComboBoxCellType).Items[Neusoft.FrameWork.Function.NConvert.ToInt32(conObj.SortID.ToString())];

                    string[] strTemp = conObj.Memo.Split('|');
                    string A1 = string.Empty;
                    string A1Tag = string.Empty;
                    foreach (string temp in strTemp)
                    {
                        if (undrugHelper.GetObjectFromID(temp) != null)
                        {
                            if (string.IsNullOrEmpty(A1))
                            {
                                A1 = (undrugHelper.GetObjectFromID(temp) as Neusoft.HISFC.Models.Base.Item).Name;
                                A1Tag = temp;
                            }
                            else
                            {
                                A1 += "|" + (undrugHelper.GetObjectFromID(temp) as Neusoft.HISFC.Models.Base.Item).Name;
                                A1Tag = "|" + temp;
                            }
                        }
                    }
                    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.A1].Text = A1;
                    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.A1].Tag = A1Tag;
                }
                #region 屏蔽
                ////A2
                //if (!string.IsNullOrEmpty(conObj.Memo))
                //{
                //    string[] strRoot = conObj.Memo.Split('&');
                //    if (strRoot.Length > 1)
                //    {
                //        this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Qty2].Text = strRoot[1];
                //    }
                //    if (strRoot.Length > 2)
                //    {
                //        this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Math2].Text = (this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Math2].CellType as FarPoint.Win.Spread.CellType.ComboBoxCellType).Items[Neusoft.FrameWork.Function.NConvert.ToInt32(strRoot[2])];
                //    }
                //    string[] strTemp = strRoot[0].Split('|');
                //    string A2 = string.Empty;
                //    string A2Tag = string.Empty;
                //    foreach (string temp in strTemp)
                //    {
                //        if (undrugHelper.GetObjectFromID(temp) != null)
                //        {
                //            if (string.IsNullOrEmpty(A2))
                //            {
                //                A2 = (undrugHelper.GetObjectFromID(temp) as Neusoft.HISFC.Models.Base.Item).Name;
                //                A2Tag = temp;
                //            }
                //            else
                //            {
                //                A2 += "|" + (undrugHelper.GetObjectFromID(temp) as Neusoft.HISFC.Models.Base.Item).Name;
                //                A2Tag = "|" + temp;
                //            }
                //        }
                //    }
                //    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.A2].Text = A2;
                //    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.A2].Tag = A2Tag;
                //}
                ////A3
                //if (!string.IsNullOrEmpty(conObj.SpellCode))
                //{
                //    string[] strRoot = conObj.SpellCode.Split('&');
                //    if (strRoot.Length > 1)
                //    {
                //        this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Qty3].Text = strRoot[1];
                //    }
                //    if (strRoot.Length > 2)
                //    {
                //        this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Math3].Text = (this.neuSpread1_Sheet1.Columns[(int)SubtblColumns.Math3].CellType as FarPoint.Win.Spread.CellType.ComboBoxCellType).Items[Neusoft.FrameWork.Function.NConvert.ToInt32(strRoot[2])];
                //    }
                //    string[] strTemp = strRoot[0].Split('|');
                //    string A3 = string.Empty;
                //    string A3Tag = string.Empty;
                //    foreach (string temp in strTemp)
                //    {
                //        if (undrugHelper.GetObjectFromID(temp) != null)
                //        {
                //            if (string.IsNullOrEmpty(A3))
                //            {
                //                A3 = (undrugHelper.GetObjectFromID(temp) as Neusoft.HISFC.Models.Base.Item).Name;
                //                A3Tag = temp;
                //            }
                //            else
                //            {
                //                A3 += "|" + (undrugHelper.GetObjectFromID(temp) as Neusoft.HISFC.Models.Base.Item).Name;
                //                A3Tag = "|" + temp;
                //            }
                //        }
                //    }
                //    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.A3].Text = A3;
                //    this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.A3].Tag = A3Tag;
                //}
                ////部位数量
                //this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Qty].Text = conObj.WBCode;
                #endregion
                //是否有效
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.ValidFlag].Value = conObj.IsValid;
                //操作员
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.Oper].Text = conObj.OperEnvironment.ID;
                //操作日期
                this.neuSpread1_Sheet1.Cells[0, (int)SubtblColumns.OperTime].Text = conObj.OperEnvironment.OperTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return 1;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            if (!this.Valid())
            {
                return -1;
            }
            this.txtFileter.Text = "";

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            consManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            if (consManager.DelConstant(this.constPhaDoseOnce) == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("删除常数失败：" + consManager.Err));
                return -1;
            }

            Neusoft.HISFC.Models.Base.Const myConst = null;
            for (int i = 0; i < this.neuSpread1_Sheet1.Rows.Count; i++)
            {
                myConst = new Neusoft.HISFC.Models.Base.Const();
                string A1 = string.Empty;
                string qty1 = string.Empty;
                string math1 = string.Empty;

                //string A2 = string.Empty;
                //string qty2 = string.Empty;
                //string math2 = string.Empty;

                //string A3 = string.Empty;
                //string qty3 = string.Empty;
                //string math3 = string.Empty;

                myConst.ID = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Index].Text.Trim();
                
                myConst.Name = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.SysCode].Text.Trim();

                A1 = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.A1].Tag != null ? this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.A1].Tag.ToString().Trim() : "";
                qty1 = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Qty1].Text;
                math1 = this.GetSelectData(i, (int)SubtblColumns.Math1); 
                myConst.Memo = A1;
                myConst.WBCode = qty1;
                myConst.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(math1);

                //A2 = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.A2].Tag != null ? this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.A2].Tag.ToString().Trim() : "";
                //qty2 = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Qty2].Text;
                //math2 = this.GetSelectData(i, (int)SubtblColumns.Math2); 
                //myConst.Memo = A2 + "&" + qty2 + "&" + math2;

                //A3 = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.A3].Tag != null ? this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.A3].Tag.ToString().Trim() : "";
                //qty3 = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Qty3].Text;
                //math3 = this.GetSelectData(i, (int)SubtblColumns.Math3); 
                //myConst.SpellCode = A3 + "&" + qty3 + "&" + math3;


                //myConst.WBCode = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Qty].Text.Trim();
                myConst.UserCode = this.GetSelectData(i, (int)SubtblColumns.Type);
                myConst.SpellCode = this.GetSelectData(i, (int)SubtblColumns.Rule);

                myConst.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.ValidFlag].Value);
                myConst.OperEnvironment.ID = this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Oper].Text.Trim();
                myConst.OperEnvironment.OperTime = consManager.GetDateTimeFromSysDateTime();

                if (consManager.SetConstant(this.constPhaDoseOnce, myConst) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("保存失败：" + consManager.Err));
                    return -1;
                }
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("保存成功"));

            return 0;
        }

        /// <summary>
        /// 获取现在的ID
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetSelectData(int row, int column)
        {
            for (int j = 0; j < (this.neuSpread1_Sheet1.Columns[column].CellType as FarPoint.Win.Spread.CellType.ComboBoxCellType).Items.Length; j++)
            {
                string item = (this.neuSpread1_Sheet1.Columns[column].CellType as FarPoint.Win.Spread.CellType.ComboBoxCellType).Items[j];

                if (item == this.neuSpread1_Sheet1.Cells[row, column].Text)
                {
                    return j.ToString();
                }
            }
            return "0";
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private bool Valid()
        {
            Hashtable hsCode = new Hashtable();
            for (int i = 0; i < this.neuSpread1_Sheet1.Rows.Count; i++)
            {
                if (hsCode.Contains(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Index].Text.Trim()))
                {
                    MessageBox.Show("第" + (i + 1).ToString() + "行编码重复，已存在相同编码项目！");
                    this.neuSpread1_Sheet1.ActiveRowIndex = i;
                    this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);
                    return false;
                }
                hsCode.Add(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Index].Text.Trim(), null);

                for (int index = 0; index < this.neuSpread1_Sheet1.Columns.Count; index++)
                {
                    if (string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.SysCode].Text))
                    {
                        MessageBox.Show("第" + (i + 1).ToString() + "行编码不能为空！");
                        this.neuSpread1_Sheet1.ActiveRowIndex = i;
                        this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);
                        return false;
                    }
                    else if (string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Type].Text))
                    {
                        MessageBox.Show("第" + (i + 1).ToString() + "行类型不能为空！");
                        this.neuSpread1_Sheet1.ActiveRowIndex = i;
                        this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);
                        return false;
                    }
                    else if (string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Qty1].Text))
                    {
                        MessageBox.Show("第" + (i + 1).ToString() + "行数量不能为空！");
                        this.neuSpread1_Sheet1.ActiveRowIndex = i;
                        this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);
                        return false;
                    }
                    else if (string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Math1].Text))
                    {
                        MessageBox.Show("第" + (i + 1).ToString() + "行公式不能为空！");
                        this.neuSpread1_Sheet1.ActiveRowIndex = i;
                        this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);
                        return false;
                    }
                    else if (string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.Rule].Text))
                    {
                        MessageBox.Show("第" + (i + 1).ToString() + "行取整规则不能为空！");
                        this.neuSpread1_Sheet1.ActiveRowIndex = i;
                        this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);
                        return false;
                    }
                    else if (string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.ItemName].Text))
                    {
                        MessageBox.Show("第" + (i + 1).ToString() + "行名称不能为空！");
                        this.neuSpread1_Sheet1.ActiveRowIndex = i;
                        this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region IMaintenanceControlable 成员 无用

        public int Add()
        {
            this.neuSpread1.Focus();
            this.neuSpread1_Sheet1.AddRows(0, 1);
            this.neuSpread1_Sheet1.ActiveRowIndex = 0;
            this.neuSpread1_Sheet1.ActiveColumnIndex = 0;
            sysCode++;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.Index].Value = sysCode;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.ValidFlag].Value = true;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.Oper].Value = consManager.Operator.ID;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, (int)SubtblColumns.OperTime].Value = this.consManager.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss");
            this.neuSpread1.ShowRow(0, this.neuSpread1_Sheet1.ActiveRowIndex, FarPoint.Win.Spread.VerticalPosition.Center);

            return 0;
        }

        public int Copy()
        {
            return 1;
        }

        public int Cut()
        {
            return 1;
        }

        public int Delete()
        {
            this.neuSpread1_Sheet1.Rows.Remove(this.neuSpread1_Sheet1.ActiveRowIndex, 1);

            return 1;
        }

        public int Export()
        {
            //>>导出{72DA7F3E-3446-4025-B21D-1C2465C69D84}
            if (this.neuSpread1.Export() == 1)
            {
                MessageBox.Show("导出成功");
            }
            //<<

            return 1;
        }

        public int Import()
        {
            return 1;
        }

        public bool IsDirty
        {
            get
            {
                return true;
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Modify()
        {
            return 1;
        }

        public int NextRow()
        {
            return 1;
        }

        public int Paste()
        {
            return 1;
        }

        public int PreRow()
        {
            return 1;
        }

        public int Print()
        {
            return 1;
        }

        public int PrintConfig()
        {
            return 1;
        }

        public int PrintPreview()
        {
            return 1;
        }
        private Neusoft.FrameWork.WinForms.Forms.IMaintenanceForm queryForm;
        public Neusoft.FrameWork.WinForms.Forms.IMaintenanceForm QueryForm
        {
            get
            {
               return this.queryForm;
            }
            set
            {
                this.queryForm = value;
            }
        }

        #endregion

        private void txtFileter_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.neuSpread1_Sheet1.RowCount; i++)
            {
                if (string.IsNullOrEmpty(txtFileter.Text))
                {
                    this.neuSpread1_Sheet1.Rows[i].Visible = true;
                }
                else
                {
                    if (this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.SysCode].Text.Contains(txtFileter.Text.Trim())
                        || this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.ItemCode].Text.Contains(txtFileter.Text.Trim())
                        || this.neuSpread1_Sheet1.Cells[i, (int)SubtblColumns.ItemName].Text.Contains(txtFileter.Text.Trim()))
                    {
                        this.neuSpread1_Sheet1.Rows[i].Visible = true;
                        //this.neuSpread1_Sheet1.RowFilter.AllString=
                    }
                    else
                    {
                        this.neuSpread1_Sheet1.Rows[i].Visible = false;
                    }
                }
            }
        }

        private void ucInputItem1_SelectedItem(Neusoft.FrameWork.Models.NeuObject sender)
        {
            if (this.ucInputItem1.FeeItem == null)
            {
                return;
            }

            if (this.neuSpread1_Sheet1.ActiveColumnIndex != (int)SubtblColumns.A1)
            {
                return;
            }
            //if (this.neuSpread1_Sheet1.Tag == null)
            //{
            //    MessageBox.Show("请先选择用法!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    return;
            //}

            if (this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, this.neuSpread1_Sheet1.ActiveColumnIndex].Tag == null)
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, this.neuSpread1_Sheet1.ActiveColumnIndex].Tag = this.ucInputItem1.FeeItem.ID;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, this.neuSpread1_Sheet1.ActiveColumnIndex].Text = this.ucInputItem1.FeeItem.Name;

            }
            else
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, this.neuSpread1_Sheet1.ActiveColumnIndex].Tag += "|" + this.ucInputItem1.FeeItem.ID;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.ActiveRowIndex, this.neuSpread1_Sheet1.ActiveColumnIndex].Text += "|" + this.ucInputItem1.FeeItem.Name;

            }
        }

        /// <summary>
        /// 列设置
        /// </summary>
        public enum SubtblColumns
        {
            /// <summary>
            /// 序号
            /// </summary>
            Index = 0,

            /// <summary>
            /// 类型
            /// </summary>
            Type,

            /// <summary>
            /// 系统编码
            /// </summary>
            SysCode,
            
            /// <summary>
            /// 组套编码
            /// </summary>
            ItemCode,

            /// <summary>
            /// 组套名称
            /// </summary>
            ItemName,

            /// <summary>
            /// 明细
            /// </summary>
            A1,

            /// <summary>
            /// 数量
            /// </summary>
            Qty1,

            /// <summary>
            /// 取整规则
            /// </summary>
            Rule,

            /// <summary>
            /// 公式
            /// </summary>
            Math1,

            ///// <summary>
            ///// A2
            ///// </summary>
            //A2,

            ///// <summary>
            ///// 数量2
            ///// </summary>
            //Qty2,

            ///// <summary>
            ///// 公式2
            ///// </summary>
            //Math2,

            ///// <summary>
            ///// A3
            ///// </summary>
            //A3,

            ///// <summary>
            ///// 数量3
            ///// </summary>
            //Qty3,

            ///// <summary>
            ///// 公式3
            ///// </summary>
            //Math3,

            ///// <summary>
            ///// 数量
            ///// </summary>
            //Qty,

            /// <summary>
            /// 是否有效
            /// </summary>
            ValidFlag,

            /// <summary>
            /// 操作员
            /// </summary>
            Oper,

            /// <summary>
            /// 操作时间
            /// </summary>
            OperTime,

            /// <summary>
            /// 扩展1 妇幼用于药袋打印
            /// </summary>
            Extend1,

            /// <summary>
            /// 扩展2
            /// </summary>
            Extend2,

            /// <summary>
            /// 扩展3
            /// </summary>
            Extend3
        }
    }

    class myCompare : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return  Neusoft.FrameWork.Function.NConvert.ToInt32(((Neusoft.HISFC.Models.Base.Const)y).ID) - Neusoft.FrameWork.Function.NConvert.ToInt32(((Neusoft.HISFC.Models.Base.Const)x).ID);
        }

    } 
}
