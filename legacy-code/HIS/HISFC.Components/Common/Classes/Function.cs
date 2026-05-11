using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Components.Common.Controls;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;
using Neusoft.HISFC.BizProcess.Integrate;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neusoft.HISFC.Components.Common.Classes
{
    /// <summary>
    /// [功能描述: 常用函数]<br></br>
    /// [创 建 者: wolf]<br></br>
    /// [创建时间: 2004-10-12]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class Function
    {
        public Function()
        {

        }

        private static Neusoft.HISFC.BizLogic.Fee.Interface managerInterface = null;

        /// <summary>
        /// 判断是否在编译状态
        /// </summary>
        public static bool DesignMode
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
            }
        }

        /// <summary>
        /// 显示标志
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ShowItemFlag(Neusoft.HISFC.Models.Base.Item item)
        {
            if (managerInterface == null)
                managerInterface = new Neusoft.HISFC.BizLogic.Fee.Interface();
            return managerInterface.ShowItemFlag(item);
        }

        /// <summary>
        /// 多科室选择
        /// </summary>
        /// <returns>被选中的科室数组</returns>
        public static List<Neusoft.HISFC.Models.Base.Department> ChooseMultiDept()
        {
            ucChooseMultiDept uc = new ucChooseMultiDept();
            Neusoft.FrameWork.WinForms.Classes.Function.PopForm.Text = "科室选择";
            Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(uc);
            return uc.SelectedDeptList;
        }

        /// <summary>
        /// 自费药品列表
        /// </summary>
        private static List<Neusoft.FrameWork.Models.NeuObject> ownFeeDrug;//= new List<Neusoft.FrameWork.Models.NeuObject>();
        public static List<Neusoft.FrameWork.Models.NeuObject> OwnFeeDrugList
        {
            get
            {
                if (ownFeeDrug==null)
                {
                    ownFeeDrug = new HISFC.BizLogic.Order.Order().GetOwnFeeDrugs();
                }
                return ownFeeDrug;
            }
           
        }

        #region 权限控制

        /// <summary>
        /// 取当前操作员是否有某一权限。
        /// </summary>
        /// <param name="class2Code">二级权限编码</param>
        /// <returns>True 有权限, False 无权限</returns>
        public static bool ChoosePiv(string class2Code)
        {
            List<Neusoft.FrameWork.Models.NeuObject> al = null;
            //权限管理类
            Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager privManager = new Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager();
            //取操作员拥有权限的科室
            al = privManager.QueryUserPriv(privManager.Operator.ID, class2Code);

            if (al == null || al.Count == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取当前所有权限科室集合
        /// </summary>
        /// <param name="class2Code">二级权限编码</param>
        /// <param name="class3Code">三级权限编码</param>
        /// <param name="isShowErrMsg">是否弹出错误信息</param>
        /// <returns>成功返回拥有权限科室列表 失败返回null</returns>
        public static List<Neusoft.FrameWork.Models.NeuObject> QueryPrivList(string class2Code, string class3Code, bool isShowErrMsg)
        {
            List<Neusoft.FrameWork.Models.NeuObject> al = null;
            //权限管理类
            Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager privManager = new Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager();
            //取操作员拥有权限的科室
            if (class3Code == null || class3Code == "")
                al = privManager.QueryUserPriv(privManager.Operator.ID, class2Code);
            else
                al = privManager.QueryUserPriv(privManager.Operator.ID, class2Code, class3Code);

            if (al == null)
            {
                if (isShowErrMsg)
                {
                    System.Windows.Forms.MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg(privManager.Err));
                }
                return null;
            }
            if (al.Count == 0)
            {
                if (isShowErrMsg)
                {
                    System.Windows.Forms.MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("您没有此窗口的操作权限"));
                }
                return al;
            }

            //成功则权限科室数组
            return al;
        }

        /// <summary>
        /// 获取当前所有权限科室集合
        /// </summary>
        /// <param name="class2Code">二级权限编码</param>
        /// <param name="isShowErrMsg">是否弹出错误信息</param>
        /// <returns>成功返回拥有权限科室列表 失败返回null</returns>
        public static List<Neusoft.FrameWork.Models.NeuObject> QueryPrivList(string class2Code, bool isShowErrMsg)
        {
            return Function.QueryPrivList(class2Code, null, isShowErrMsg);
        }

        /// <summary>
        /// 弹出窗口，选择权限科室，如果用户只有一个科室的权限，则可以直接登陆
        /// </summary>
        /// <param name="class2Code">二级权限编码</param>
        /// <param name="privDept">权限科室</param>
        /// <returns>1 返回正常权限科室 0 用户选择取消 －1 用户无权限</returns>
        public static int ChoosePivDept(string class2Code, ref Neusoft.FrameWork.Models.NeuObject privDept)
        {
            return ChoosePrivDept(class2Code, null, ref privDept);
        }

        /// <summary>
        /// 弹出窗口，选择权限科室，如果用户只有一个科室的权限，则可以直接登陆
        /// </summary>
        /// <param name="class2Code">二级权限编码</param>
        /// <param name="class3Code">三级权限编码</param>
        /// <param name="privDept">权限科室</param>
        /// <returns>1 返回正常权限科室 0 用户选择取消 －1 用户无权限</returns>
        public static int ChoosePrivDept(string class2Code, string class3Code, ref Neusoft.FrameWork.Models.NeuObject privDept)
        {
            List<Neusoft.FrameWork.Models.NeuObject> al = new List<Neusoft.FrameWork.Models.NeuObject>();
            if (class3Code == null || class3Code == "")
                al = QueryPrivList(class2Code, true);
            else
                al = QueryPrivList(class2Code, class3Code, true);

            if (al == null || al.Count == 0)
            {
                return -1;
            }

            //如果用户只有一个科室的权限，则返回此科室
            if (al.Count == 1)
            {
                privDept = al[0] as Neusoft.FrameWork.Models.NeuObject;
                return 1;
            }

            //弹出窗口，取权限科室
            Neusoft.HISFC.Components.Common.Forms.frmChoosePrivDept formPrivDept = new Neusoft.HISFC.Components.Common.Forms.frmChoosePrivDept();
            formPrivDept.SetPriv(al, true);
            System.Windows.Forms.DialogResult Result = formPrivDept.ShowDialog();

            //取窗口返回权限科室
            if (Result == System.Windows.Forms.DialogResult.OK)
            {
                privDept = formPrivDept.SelectData;
                return 1;
            }

            return 0;
        }

        #endregion

        #region 显示医嘱

        static Neusoft.HISFC.BizLogic.Pharmacy.Item pManager = new Neusoft.HISFC.BizLogic.Pharmacy.Item();
        static Neusoft.HISFC.BizLogic.Fee.Item fManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        static Neusoft.HISFC.BizLogic.Manager.Department deptMgr = new Neusoft.HISFC.BizLogic.Manager.Department();
        static Neusoft.FrameWork.Public.ObjectHelper helper = new Neusoft.FrameWork.Public.ObjectHelper();
        static Neusoft.HISFC.BizProcess.Integrate.Pharmacy phaIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();

        //private static Neusoft.SOC.HISFC.BizProcess.OrderIntegrate.OrderBase socOrderIntegrate = new Neusoft.SOC.HISFC.BizProcess.OrderIntegrate.OrderBase();

        private static Hashtable hsPhaStorage = new Hashtable();

        /// <summary>
        /// 非药品基本信息列表
        /// </summary>
        private static List<Neusoft.HISFC.Models.Fee.Item.Undrug> undrugList = null;

        private static Hashtable hsUndrugItem = null;
        static Neusoft.HISFC.Models.Pharmacy.Storage storage = null;

        /// <summary>
        /// 医保项目：合同单位+项目编码
        /// </summary>
        public static Hashtable HsItemPactInfo = null;

        /// <summary>
        /// 住院是否使用预扣库存 P00200
        /// </summary>
        //private static int isUseInDrugPreOut = -1;

        ///// <summary>
        ///// 门诊是否使用预扣库存 P00320
        ///// </summary>
        //private static int isUseOutDrugPreOut = -1;
        //static Neusoft.HISFC.Models.Base.Employee oper = pManager.Operator as Neusoft.HISFC.Models.Base.Employee;

        /// <summary>
        /// 检查缺药、停用
        /// </summary>
        /// <param name="drugDept">扣库科室</param>
        /// <param name="item">项目</param>
        /// <param name="IsOutPatient">是否门诊开立</param>
        /// <param name="drugItem">返回的项目信息</param>
        /// <param name="errInfo">错误信息</param>
        /// <returns></returns>
        //public static int CheckDrugState(Neusoft.FrameWork.Models.NeuObject drugDept, string itemCode, Neusoft.HISFC.Models.Base.Item item, bool IsOutPatient, ref string errInfo)
        //{
        //    if (item.ID == "999")
        //    {
        //        return 1;
        //    }
        //    storage = pManager.GetStockInfoByDrugCode(drugDept.ID, item.ID);
        //    if (storage == null)
        //    {
        //        errInfo = "错误：" + pManager.Err;
        //        return -1;
        //    }
        //    else if (storage.Item.ID == "")
        //    {
        //        storage.Item = pManager.GetItem(itemCode);
        //        if (IsOutPatient)
        //        {
        //            errInfo = "【" + item.Name + "】在本科室没有找到对应门诊系统的取药药房!";
        //        }
        //        else
        //        {
        //            errInfo = "【" + item.Name + "】在本科室没有找到对应住院系统的取药药房!";
        //        }
        //        item = null;
        //        return -1;
        //    }
        //    else if (storage.ValidState != Neusoft.HISFC.Models.Base.EnumValidState.Valid)
        //    {
        //        //在药房已停用
        //        errInfo = "【" + item.Name + "】在" + drugDept.Name + "已停用!";
        //        return -1;
        //    }
        //    if (IsOutPatient)
        //    {
        //        if (!storage.IsUseForOutpatient)
        //        {
        //            //不能用于门诊
        //            errInfo = "【" + item.Name + "】在" + drugDept.Name + "设置为不能用于门诊用药!";
        //            return -1;
        //        }
        //        //门诊缺药
        //        else if (storage.IsLack)
        //        {
        //            //药房缺药
        //            errInfo = "【" + item.Name + "】" + drugDept.Name + "已缺药!";
        //            return -1;
        //        }

        //        if (isUseOutDrugPreOut == -1)
        //        {
        //            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctrlIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        //            isUseOutDrugPreOut = ctrlIntegrate.GetControlParam<int>(PharmacyConstant.OutDrug_Pre_Out, true);
        //        }

        //        if (isUseOutDrugPreOut == 1)
        //        {
        //            if (storage.StoreQty - storage.PreOutQty <= 0)
        //            {
        //                errInfo = "【" + item.Name + "】在" + drugDept.Name + "库存为0，不允许开立!";
        //                return -1;
        //            }
        //        }
        //        else
        //        {
        //            if (storage.StoreQty <= 0)
        //            {
        //                errInfo = "【" + item.Name + "】在" + drugDept.Name + "库存为0，不允许开立!";
        //                return -1;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (!storage.IsUseForInpatient)
        //        {
        //            //不能用于门诊
        //            errInfo = "【" + item.Name + "】在" + drugDept.Name + "设置为不能用于住院用药!";
        //            return -1;
        //        }
        //        //住院缺药
        //        else if (storage.IsLackForInpatient)
        //        {
        //            //药房缺药
        //            errInfo = "【" + item.Name + "】" + drugDept.Name + "已缺药!";
        //            return -1;
        //        }

        //        if (isUseInDrugPreOut == -1)
        //        {
        //            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctrlIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        //            isUseInDrugPreOut = ctrlIntegrate.GetControlParam<int>(PharmacyConstant.InDrug_Pre_Out, true);
        //        }

        //        if (isUseInDrugPreOut == 1)
        //        {
        //            if (storage.StoreQty - storage.PreOutQty <= 0)
        //            {
        //                errInfo = "【" + item.Name + "】在" + drugDept.Name + "库存为0，不允许开立!";
        //                return -1;
        //            }
        //        }
        //        else
        //        {
        //            if (storage.StoreQty <= 0)
        //            {
        //                errInfo = "【" + item.Name + "】在" + drugDept.Name + "库存为0，不允许开立!";
        //                return -1;
        //            }
        //        }
        //    }
        //    return 1;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alOrder"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private static void CheckOrderState(ArrayList alOrder, Neusoft.HISFC.Models.Base.ServiceTypes serviceType)
        {
            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = null;
            string errInfo = "";
            string undrugCodes = "''";

            if (helper.ArrayObject.Count == 0)
            {
                Neusoft.HISFC.BizLogic.Manager.OrderType orderType = new Neusoft.HISFC.BizLogic.Manager.OrderType();
                helper.ArrayObject = orderType.GetList();
            }

            if (serviceType == Neusoft.HISFC.Models.Base.ServiceTypes.I)
            {
                hsUndrugItem = new Hashtable();
                foreach (Neusoft.HISFC.Models.Order.Inpatient.Order inOrder in alOrder)
                {
                    if (inOrder.Item.ID != "999" && inOrder.Item.ItemType != Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        undrugCodes += ",'" + inOrder.Item.ID + "'";
                    }
                }

                if (!string.IsNullOrEmpty(undrugCodes))
                {
                    undrugList = fManager.GetItemByCodeBatch("(" + undrugCodes + ")");
                    if (undrugList != null)
                    {
                        for (int i = 0; i < undrugList.Count; i++)
                        {
                            if (!hsUndrugItem.Contains(undrugList[i].ID))
                            {
                                hsUndrugItem.Add(undrugList[i].ID, undrugList[i]);
                            }
                        }
                    }
                }

                foreach (Neusoft.HISFC.Models.Order.Inpatient.Order inOrder in alOrder)
                {
                    inOrder.OrderType = helper.GetObjectFromID(inOrder.OrderType.ID) as Neusoft.HISFC.Models.Order.OrderType;

                    //storage = new Neusoft.HISFC.Models.Pharmacy.Storage();
                    undrug = new Neusoft.HISFC.Models.Fee.Item.Undrug();

                    #region 更新项目信息
                    if (inOrder.ID == "999")//自备项目
                    {
                        undrug = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        undrug.ID = inOrder.ID;
                        undrug.Name = inOrder.Name;
                        undrug.Qty = inOrder.Item.Qty;

                        undrug.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
                        undrug.SysClass.ID = "M";
                        undrug.PriceUnit = inOrder.Unit;
                        inOrder.Item = undrug;
                    }
                    else if (inOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)//药品
                    {
                        if (hsPhaStorage.Contains(inOrder.Item.ID))
                        {
                            storage = hsPhaStorage[inOrder.Item.ID] as Neusoft.HISFC.Models.Pharmacy.Storage;
                        }

                        //if (storage == null || string.IsNullOrEmpty(storage.StockDept.ID))
                        //{
                        //    storage = pManager.GetItemForInpatient(((Neusoft.HISFC.Models.Base.Employee)pManager.Operator).Dept.ID, inOrder.Item.ID);
                        //}
                        Neusoft.HISFC.Models.Pharmacy.Item drugItem = null;
                        Neusoft.HISFC.Models.Pharmacy.Item baseDrug = Function.GetItem(inOrder.Item.ID);
                        if (baseDrug.ExtendData3 == "1")
                        {
                            if (SOC.HISFC.BizProcess.OrderIntegrate.OrderBase.CheckDrugStateFHY(inOrder.Patient, null, ((Neusoft.HISFC.Models.Base.Employee)pManager.Operator).Dept, inOrder.Item, false, ref drugItem, ref storage, ref errInfo) <= 0)
                            {
                                inOrder.ExtendFlag2 = "N";
                            }
                        }
                        else
                        {
                            if (SOC.HISFC.BizProcess.OrderIntegrate.OrderBase.CheckDrugState(inOrder.Patient, null, ((Neusoft.HISFC.Models.Base.Employee)pManager.Operator).Dept, inOrder.Item, false, ref drugItem, ref storage, ref errInfo) <= 0)
                            {
                                inOrder.ExtendFlag2 = "N";
                            }
                        }
                        
                        storage.Item = drugItem;

                        storage.Item.Qty = inOrder.Qty;
                        try
                        {
                            //storage.Item.DoseUnit = ((Neusoft.HISFC.Models.Pharmacy.Item)inOrder.Item).DoseUnit;
                        }
                        catch
                        {
                        }

                        if (storage.Item != null && !string.IsNullOrEmpty(storage.Item.ID))
                        {
                            inOrder.Item = storage.Item;
                        }

                        //药品执行科室为空
                        inOrder.ExeDept = new Neusoft.FrameWork.Models.NeuObject();
                    }
                    else//复合项目
                    {
                        //if (hsUndrugItem.Contains(inOrder.Item.ID))
                        //{
                        //    undrug = hsUndrugItem[inOrder.Item.ID] as Neusoft.HISFC.Models.Fee.Item.Undrug;
                        //}
                        //else
                        //{
                            if (SOC.HISFC.BizProcess.OrderIntegrate.OrderBase.CheckUnDrugState(inOrder, ref undrug, ref errInfo) == -1)
                            {
                                inOrder.ExtendFlag2 = "N";
                            }
                        //}
                        if (undrug == null || !undrug.IsValid)
                        {
                            inOrder.ExtendFlag2 = "N";
                        }
                        else
                        {
                            //undrug.ID = inOrder.ID;
                            //undrug.Name = inOrder.Name;
                            undrug.Qty = inOrder.Item.Qty;
                            undrug.PriceUnit = inOrder.Unit;
                            inOrder.Item = undrug;
                            inOrder.Item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                hsUndrugItem = new Hashtable();
                foreach (Neusoft.HISFC.Models.Order.OutPatient.Order outOrder in alOrder)
                {
                    if (outOrder.Item.ID != "999" && outOrder.Item.ItemType != Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        undrugCodes += ",'" + outOrder.Item.ID + "'";
                    }
                }

                if (!string.IsNullOrEmpty(undrugCodes))
                {
                    undrugList = fManager.GetItemByCodeBatch("(" + undrugCodes + ")");
                    if (undrugList != null)
                    {
                        for (int i = 0; i < undrugList.Count; i++)
                        {
                            if (!hsUndrugItem.Contains(undrugList[i].ID))
                            {
                                hsUndrugItem.Add(undrugList[i].ID, undrugList[i]);
                            }
                        }
                    }
                }

                foreach (Neusoft.HISFC.Models.Order.OutPatient.Order outOrder in alOrder)
                {
                    //storage = new Neusoft.HISFC.Models.Pharmacy.Storage();
                    undrug = new Neusoft.HISFC.Models.Fee.Item.Undrug();

                    #region 更新项目信息
                    if (outOrder.Item.ID == "999")//自备项目
                    {
                        //undrug = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        //undrug.ID = outOrder.ID;
                        //undrug.Name = outOrder.Name;
                        //undrug.Qty = outOrder.Item.Qty;

                        //undrug.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
                        //undrug.SysClass.ID = "M";
                        //undrug.PriceUnit = outOrder.Unit;
                        //outOrder.Item = undrug;
                    }
                    else if (outOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)//药品
                    {
                        //if (hsPhaStorage.Contains(outOrder.Item.ID))
                        //{
                        //    storage = hsPhaStorage[outOrder.Item.ID] as Neusoft.HISFC.Models.Pharmacy.Storage;
                        //}

                        //if (storage == null || string.IsNullOrEmpty(storage.StockDept.ID))
                        //{
                        //    storage = pManager.GetItemForInpatient(((Neusoft.HISFC.Models.Base.Employee)pManager.Operator).Dept.ID, outOrder.Item.ID);
                        //}

                        storage = pManager.GetItemForInpatient(((Neusoft.HISFC.Models.Base.Employee)pManager.Operator).Dept.ID, outOrder.Item.ID);

                        Neusoft.HISFC.Models.Pharmacy.Item drugItem = null;
                        Neusoft.HISFC.Models.Pharmacy.Item baseDrug = GetItem(outOrder.Item.ID);
                        if (baseDrug.ExtendData3 == "1")
                        {
                            if (SOC.HISFC.BizProcess.OrderIntegrate.OrderBase.CheckDrugStateFHY(outOrder.Patient, null, ((Neusoft.HISFC.Models.Base.Employee)pManager.Operator).Dept, outOrder.Item, true, ref drugItem, ref storage, ref errInfo) == -1)
                            {
                                outOrder.ExtendFlag2 = "N";
                            }
                        }
                        else
                        {
                            if (SOC.HISFC.BizProcess.OrderIntegrate.OrderBase.CheckDrugState(outOrder.Patient, null, ((Neusoft.HISFC.Models.Base.Employee)pManager.Operator).Dept, outOrder.Item, true, ref drugItem, ref storage, ref errInfo) == -1)
                            {
                                outOrder.ExtendFlag2 = "N";
                            }
                        }
                        storage.Item = drugItem;

                        try
                        {
                            storage.Item.DoseUnit = ((Neusoft.HISFC.Models.Pharmacy.Item)outOrder.Item).DoseUnit;
                        }
                        catch
                        {
                        }
                        if (storage.Item != null && !string.IsNullOrEmpty(storage.Item.ID))
                        {
                            decimal qty = outOrder.Item.Qty;
                            outOrder.Item = storage.Item.Clone();//需要用克隆方法
                            outOrder.Item.Qty = qty;
                        }

                        //药品执行科室为空
                        outOrder.ExeDept = new Neusoft.FrameWork.Models.NeuObject();
                    }
                    else
                    {
                        //if (hsUndrugItem.Contains(outOrder.Item.ID))
                        //{
                        //    undrug = hsUndrugItem[outOrder.Item.ID] as Neusoft.HISFC.Models.Fee.Item.Undrug;
                        //}
                        //else
                        //{
                            if (SOC.HISFC.BizProcess.OrderIntegrate.OrderBase.CheckUnDrugState(outOrder, ref undrug, ref errInfo) == -1)
                            {
                                outOrder.ExtendFlag2 = "N";
                            }
                        //}
                        if (undrug == null || !undrug.IsValid)
                        {
                            outOrder.ExtendFlag2 = "N";
                        }
                        else
                        {
                            //undrug.ID = outOrder.ID;
                            //undrug.Name = outOrder.Name;
                            undrug.Qty = outOrder.Item.Qty;
                            undrug.PriceUnit = outOrder.Unit;
                            outOrder.Item = undrug;
                            outOrder.Item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 显示医嘱信息
        /// </summary>
        /// <param name="sender"></param>
        public static void ShowOrder(object sender, ArrayList alOrder, Neusoft.HISFC.Models.Base.ServiceTypes serviceType)
        {
            ShowOrder(sender, alOrder, 0, serviceType);
        }

        /// <summary>
        /// 显示医嘱信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="alOrder"></param>
        /// <param name="type"></param>
        public static void ShowOrder(object sender, ArrayList alOrder, int type, Neusoft.HISFC.Models.Base.ServiceTypes serviceType)
        {
            try
            {
                #region 设置dataSet

                #region 变量声明及初始化
                //定义传出DataSet
                DataSet myDataSet = new DataSet();
                myDataSet.EnforceConstraints = false;//是否遵循约束规则
                //定义类型
                System.Type dtStr = System.Type.GetType("System.String");
                System.Type dtBool = System.Type.GetType("System.Boolean");
                System.Type dtInt = System.Type.GetType("System.Int32");
                //定义表********************************************************
                //Main Table
                DataTable dtMain = new DataTable();
                dtMain = myDataSet.Tables.Add("TableMain");

                dtMain.Columns.AddRange(new DataColumn[]{
                    new DataColumn("ID", dtStr),
                    new DataColumn("组合号", dtStr), 
                    new DataColumn("医嘱名称", dtStr),
                    new DataColumn("规格", dtStr),
                    new DataColumn("组合", dtStr),
                    new DataColumn("间隔时间", dtStr),
                    new DataColumn("每次剂量", dtStr),
                    new DataColumn("频次", dtStr),
                    new DataColumn("数量", dtStr),
                    new DataColumn("付数", dtStr),
                    new DataColumn("用法", dtStr),
                    new DataColumn("医嘱类型", dtStr),
                    new DataColumn("加急", dtBool),
                    new DataColumn("开始时间", dtStr),
                    new DataColumn("开立时间", dtStr),
                    new DataColumn("开立医生", dtStr),
                    new DataColumn("执行科室", dtStr),
                    new DataColumn("停止时间", dtStr),
                    new DataColumn("停止医生", dtStr),
                    new DataColumn("备注", dtStr),
                    new DataColumn("顺序号", dtStr)
                });
                #endregion

                string beginDate = "", endDate = "", moDate = "";

                Neusoft.HISFC.Models.Order.Inpatient.Order inOrder = null;
                Neusoft.HISFC.Models.Order.OutPatient.Order outOrder = null;
                Neusoft.HISFC.Models.Pharmacy.Item item = null;

                CheckOrderState(alOrder, serviceType);

                for (int i = 0; i < alOrder.Count; i++)
                {
                    #region 住院组套

                    if (serviceType == Neusoft.HISFC.Models.Base.ServiceTypes.I)
                    {
                        inOrder = alOrder[i] as Neusoft.HISFC.Models.Order.Inpatient.Order;

                        #region 显示医嘱
                        if (inOrder.Item != null)
                        {
                            if (inOrder.BeginTime == DateTime.MinValue)
                                beginDate = "";
                            else
                                beginDate = inOrder.BeginTime.ToString();

                            if (inOrder.EndTime == DateTime.MinValue)
                                endDate = "";
                            else
                                endDate = inOrder.EndTime.ToString();

                            if (inOrder.MOTime == DateTime.MinValue)
                                moDate = "";
                            else
                                moDate = inOrder.MOTime.ToString();

                            if (inOrder.Item.GetType() == typeof(Neusoft.HISFC.Models.Pharmacy.Item))
                            {
                                item = inOrder.Item as Neusoft.HISFC.Models.Pharmacy.Item;
                                if (string.IsNullOrEmpty(inOrder.DoseUnit))
                                {
                                    inOrder.DoseUnit = item.DoseUnit;
                                }
                                dtMain.Rows.Add(new Object[] 
                                {  inOrder.ID,
                                    inOrder.Combo.ID,
                                    inOrder.Item.Name,
                                    inOrder.Item.Specs,															 
                                    "",
                                    inOrder.User03,
                                    //o.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.')+item.DoseUnit ,	
                                    //存在用最小单位作为每次用量单位的
                                    inOrder.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.')+inOrder.DoseUnit ,																 
                                    inOrder.Frequency.ID,
                                    inOrder.Qty.ToString()+inOrder.Unit,inOrder.HerbalQty,
                                    inOrder.Usage.Name,
                                    inOrder.OrderType.Name,
                                    inOrder.IsEmergency,
                                    beginDate,
                                    moDate,
                                    inOrder.ReciptDoctor.Name,
                                    inOrder.ExeDept.Name,
                                    endDate,
                                    inOrder.DCOper.Name,
                                    inOrder.Memo,inOrder.SortID
                                });
                            }
                            else if (inOrder.Item.GetType() == typeof(Neusoft.HISFC.Models.Fee.Item.Undrug))
                            {
                                dtMain.Rows.Add(new Object[] 
                                { 
                                    inOrder.ID,
                                    inOrder.Combo.ID,
                                    inOrder.Item.Name,
                                    inOrder.Item.Specs,
                                    "",
                                    inOrder.User03,
                                    "" ,
                                    inOrder.Frequency.ID,
                                    inOrder.Qty.ToString()+inOrder.Unit,
                                    "","",
                                    inOrder.OrderType.Name,
                                    inOrder.IsEmergency,
                                    beginDate,
                                    moDate,
                                    inOrder.ReciptDoctor.Name,
                                    inOrder.ExeDept.Name,
                                    endDate,
                                    inOrder.DCOper.Name,
                                    inOrder.Memo,
                                    inOrder.SortID});
                            }
                            else
                            {
                                dtMain.Rows.Add(new Object[] 
                                { 
                                    inOrder.ID,
                                    inOrder.Combo.ID,
                                    inOrder.Item.Name,
                                    inOrder.Item.Specs,
                                    "",
                                    inOrder.User03,
                                    inOrder.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.')+inOrder.DoseUnit,
                                    inOrder.Frequency.ID,
                                    inOrder.Qty.ToString()+inOrder.Unit,
                                    inOrder.HerbalQty,
                                    inOrder.Usage.Name,
                                    inOrder.OrderType.Name,
                                    inOrder.IsEmergency,
                                    beginDate,
                                    moDate,
                                    inOrder.ReciptDoctor.Name,
                                    inOrder.ExeDept.Name,
                                    endDate,
                                    inOrder.DCOper.Name,
                                    inOrder.Memo,
                                    inOrder.SortID});
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region 门诊组套
                    else
                    {
                        outOrder = alOrder[i] as Neusoft.HISFC.Models.Order.OutPatient.Order;

                        #region 显示医嘱
                        if (outOrder.Item != null)
                        {
                            if (outOrder.BeginTime == DateTime.MinValue)
                                beginDate = "";
                            else
                                beginDate = outOrder.BeginTime.ToString();

                            if (outOrder.EndTime == DateTime.MinValue)
                                endDate = "";
                            else
                                endDate = outOrder.EndTime.ToString();

                            if (outOrder.MOTime == DateTime.MinValue)
                                moDate = "";
                            else
                                moDate = outOrder.MOTime.ToString();

                            if (outOrder.Item.GetType() == typeof(Neusoft.HISFC.Models.Pharmacy.Item))
                            {
                                item = outOrder.Item as Neusoft.HISFC.Models.Pharmacy.Item;

                                dtMain.Rows.Add(new Object[] 
                                {  
                                    outOrder.ID,
                                    outOrder.Combo.ID,
                                    outOrder.Item.Name,
                                    outOrder.Item.Specs,
                                    "",
                                    outOrder.User03,
                                    //o.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.')+item.DoseUnit ,
                                    //存在用最小单位作为每次用量单位的
                                    outOrder.DoseUnit == item.MinUnit ?outOrder.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.')+outOrder.DoseUnit :outOrder.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.')+item.DoseUnit ,
                                    outOrder.Frequency.ID,
                                    outOrder.Qty.ToString()+outOrder.Unit,outOrder.HerbalQty,outOrder.Usage.Name,
                                    "",
                                    outOrder.IsEmergency,
                                    beginDate,
                                    moDate,
                                    outOrder.ReciptDoctor.Name,
                                    outOrder.ExeDept.Name,endDate,
                                    outOrder.DCOper.Name,
                                    outOrder.Memo,
                                    outOrder.SortID
                                });

                            }
                            else if (outOrder.Item.GetType() == typeof(Neusoft.HISFC.Models.Fee.Item.Undrug))
                            {
                                dtMain.Rows.Add(new Object[] 
                                { 
                                    outOrder.ID,
                                    outOrder.Combo.ID,
                                    outOrder.Item.Name,
                                    outOrder.Item.Specs,
                                    "",
                                    outOrder.User03,
                                    "" ,
                                    outOrder.Frequency.ID,
                                    outOrder.Qty.ToString()+outOrder.Unit,
                                    "",
                                    "",
                                    "",
                                    outOrder.IsEmergency,
                                    beginDate,moDate,
                                    outOrder.ReciptDoctor.Name,
                                    outOrder.ExeDept.Name,
                                    endDate,
                                    outOrder.DCOper.Name,
                                    outOrder.Memo,
                                    outOrder.SortID
                                });
                            }
                            else
                            {
                                dtMain.Rows.Add(new Object[] 
                                { 
                                    outOrder.ID,
                                    outOrder.Combo.ID, 
                                    outOrder.Item.Name,
                                    outOrder.Item.Specs,
                                    "",
                                    outOrder.User03,
                                    outOrder.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.')+outOrder.DoseUnit,
                                    outOrder.Frequency.ID,
                                    outOrder.Qty.ToString()+outOrder.Unit,outOrder.HerbalQty,outOrder.Usage.Name,
                                    "",
                                    outOrder.IsEmergency,
                                    beginDate,
                                    moDate,
                                    outOrder.ReciptDoctor.Name,
                                    outOrder.ExeDept.Name,
                                    endDate,
                                    outOrder.DCOper.Name,
                                    outOrder.Memo,
                                    outOrder.SortID
                                });
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                switch (sender.GetType().ToString().Substring(sender.GetType().ToString().LastIndexOf(".") + 1))
                {
                    case "SheetView":
                        FarPoint.Win.Spread.SheetView o = sender as FarPoint.Win.Spread.SheetView;
                        o.RowCount = 0;
                        o.DataSource = myDataSet.Tables[0];
                        for (int i = 0; i < alOrder.Count; i++)
                        {
                            o.Rows[i].Tag = alOrder[i];

                            //根据医嘱状态设置颜色
                            Neusoft.HISFC.Models.Order.Order tmpOrder = alOrder[i] as Neusoft.HISFC.Models.Order.Order;

                            switch (tmpOrder.Status)
                            {
                                case 0:
                                    o.RowHeader.Rows[i].BackColor = Color.FromArgb(128, 255, 128);
                                    break;
                                case 1:
                                    o.RowHeader.Rows[i].BackColor = Color.FromArgb(106, 174, 242);
                                    break;
                                case 2:
                                    o.RowHeader.Rows[i].BackColor = Color.FromArgb(243, 230, 105);
                                    break;
                                case 3:
                                    o.RowHeader.Rows[i].BackColor = Color.FromArgb(248, 120, 222);
                                    break;
                                default:
                                    o.RowHeader.Rows[i].BackColor = Color.Black;
                                    break;
                            }
                        }
                        #region 设置列
                        o.Columns[0].Visible = false;
                        o.Columns[1].Visible = false;
                        //2 ("医嘱名称", dtStr),3("规格", dtStr),4 组合,5间隔时间,6("每次剂量", dtStr),
                        //7("频次", dtStr),8("数量", dtStr),9("付数", dtStr),
                        //10("用法", dtStr),11("医嘱类型", dtStr),12("加急", dtBool),
                        //13("开始时间", dtStr),14("开立时间", dtStr),15("开立医生", dtStr),
                        //16("执行科室", dtStr),17("停止时间", dtStr),18("停止医生", dtStr),
                        //19("备注", dtStr),20("顺序号", dtStr)});
                        o.Columns[2].Width = 300;
                        o.Columns[3].Width = 50;
                        o.Columns[4].Width = 40;
                        o.Columns[5].Width = 80;
                        o.Columns[5].CellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                        o.Columns[6].Width = 100;
                        o.Columns[7].Width = 80;
                        o.Columns[8].Width = 80;
                        o.Columns[9].Width = 60;
                        o.Columns[10].Width = 80;
                        o.Columns[11].Width = 60;
                        o.Columns[12].Width = 40;
                        o.Columns[13].Width = 80;
                        o.Columns[14].Width = 80;
                        o.Columns[15].Width = 80;
                        o.Columns[16].Width = 80;
                        o.Columns[17].Width = 80;
                        o.Columns[18].Width = 80;
                        o.Columns[19].Width = 80;
                        o.Columns[20].Width = 30;
                        if (type == 1)//组套
                        {
                            o.Columns[5].Visible = true;
                        }
                        else
                        {
                            o.Columns[5].Visible = false;
                        }
                        #endregion

                        Function.DrawCombo(o, 1, 4);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ShowOrder" + ex.Message);
                return;
            }
        }

        public static void DrawCombo(object sender, int column, int DrawColumn, int ChildViewLevel)
        {
            switch (sender.GetType().ToString().Substring(sender.GetType().ToString().LastIndexOf(".") + 1))
            {
                case "SheetView":
                    FarPoint.Win.Spread.SheetView o = sender as FarPoint.Win.Spread.SheetView;
                    int i = 0;
                    string tmp = "", curComboNo = "";
                    if (ChildViewLevel == 0)
                    {
                        for (i = 0; i < o.RowCount; i++)
                        {
                            #region "画"
                            if (o.Cells[i, column].Text == "0") o.Cells[i, column].Text = "";
                            tmp = o.Cells[i, column].Text + "";
                            o.Cells[i, column].Tag = tmp;
                            if (curComboNo != tmp && tmp != "") //是头
                            {
                                curComboNo = tmp;
                                o.Cells[i, DrawColumn].Text = "┓";
                                try
                                {
                                    if (o.Cells[i - 1, DrawColumn].Text == "┃")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "┛";
                                    }
                                    else if (o.Cells[i - 1, DrawColumn].Text == "┓")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "";
                                    }
                                }
                                catch { }
                            }
                            else if (curComboNo == tmp && tmp != "")
                            {
                                o.Cells[i, DrawColumn].Text = "┃";
                            }
                            else if (curComboNo != tmp && tmp == "")
                            {
                                try
                                {
                                    if (o.Cells[i - 1, DrawColumn].Text == "┃")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "┛";
                                    }
                                    else if (o.Cells[i - 1, DrawColumn].Text == "┓")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "";
                                    }
                                }
                                catch { }
                                o.Cells[i, DrawColumn].Text = "";
                                curComboNo = "";
                            }
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┃") o.Cells[i, DrawColumn].Text = "┛";
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┓") o.Cells[i, DrawColumn].Text = "";
                            //o.Cells[i, DrawColumn].ForeColor = System.Drawing.Color.Red;
                            #endregion
                        }
                    }
                    else if (ChildViewLevel == 1)
                    {
                        for (int m = 0; m < o.RowCount; m++)
                        {
                            FarPoint.Win.Spread.SheetView c = o.GetChildView(m, 0);
                            for (int j = 0; j < c.RowCount; j++)
                            {
                                #region "画"
                                if (c.Cells[j, column].Text == "0") c.Cells[j, column].Text = "";
                                tmp = c.Cells[j, column].Text + "";

                                c.Cells[j, column].Tag = tmp;
                                if (curComboNo != tmp && tmp != "") //是头
                                {
                                    curComboNo = tmp;
                                    c.Cells[j, DrawColumn].Text = "┓";
                                    try
                                    {
                                        if (c.Cells[j - 1, DrawColumn].Text == "┃")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "┛";
                                        }
                                        else if (c.Cells[j - 1, DrawColumn].Text == "┓")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "";
                                        }
                                    }
                                    catch { }
                                }
                                else if (curComboNo == tmp && tmp != "")
                                {
                                    c.Cells[j, DrawColumn].Text = "┃";
                                }
                                else if (curComboNo != tmp && tmp == "")
                                {
                                    try
                                    {
                                        if (c.Cells[j - 1, DrawColumn].Text == "┃")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "┛";
                                        }
                                        else if (c.Cells[j - 1, DrawColumn].Text == "┓")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "";
                                        }
                                    }
                                    catch { }
                                    c.Cells[j, DrawColumn].Text = "";
                                    curComboNo = "";
                                }
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┃") c.Cells[j, DrawColumn].Text = "┛";
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┓") c.Cells[j, DrawColumn].Text = "";
                                //c.Cells[j, DrawColumn].ForeColor = System.Drawing.Color.Red;
                                #endregion

                            }
                        }
                    }
                    break;
            }

        }


        public static void DrawComboLeft(object sender, int column, int DrawColumn, int ChildViewLevel)
        {
            switch (sender.GetType().ToString().Substring(sender.GetType().ToString().LastIndexOf(".") + 1))
            {
                case "SheetView":
                    FarPoint.Win.Spread.SheetView o = sender as FarPoint.Win.Spread.SheetView;
                    int i = 0;
                    string tmp = "", curComboNo = "";
                    if (ChildViewLevel == 0)
                    {
                        for (i = 0; i < o.RowCount; i++)
                        {
                            #region "画"
                            if (o.Cells[i, column].Text == "0") o.Cells[i, column].Text = "";
                            tmp = o.Cells[i, column].Text + "";
                            o.Cells[i, column].Tag = tmp;
                            if (curComboNo != tmp && tmp != "") //是头
                            {
                                curComboNo = tmp;
                                o.Cells[i, DrawColumn].Text = "┏";
                                try
                                {
                                    if (o.Cells[i - 1, DrawColumn].Text == "┃")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "┗";
                                    }
                                    else if (o.Cells[i - 1, DrawColumn].Text == "┏")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "";
                                    }
                                }
                                catch { }
                            }
                            else if (curComboNo == tmp && tmp != "")
                            {
                                o.Cells[i, DrawColumn].Text = "┃";
                            }
                            else if (curComboNo != tmp && tmp == "")
                            {
                                try
                                {
                                    if (o.Cells[i - 1, DrawColumn].Text == "┃")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "┗";
                                    }
                                    else if (o.Cells[i - 1, DrawColumn].Text == "┏")
                                    {
                                        o.Cells[i - 1, DrawColumn].Text = "";
                                    }
                                }
                                catch { }
                                o.Cells[i, DrawColumn].Text = "";
                                curComboNo = "";
                            }
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┃") o.Cells[i, DrawColumn].Text = "┗";
                            if (i == o.RowCount - 1 && o.Cells[i, DrawColumn].Text == "┏") o.Cells[i, DrawColumn].Text = "";
                            //o.Cells[i, DrawColumn].ForeColor = System.Drawing.Color.Red;
                            #endregion
                        }
                    }
                    else if (ChildViewLevel == 1)
                    {
                        for (int m = 0; m < o.RowCount; m++)
                        {
                            FarPoint.Win.Spread.SheetView c = o.GetChildView(m, 0);
                            for (int j = 0; j < c.RowCount; j++)
                            {
                                #region "画"
                                if (c.Cells[j, column].Text == "0") c.Cells[j, column].Text = "";
                                tmp = c.Cells[j, column].Text + "";

                                c.Cells[j, column].Tag = tmp;
                                if (curComboNo != tmp && tmp != "") //是头
                                {
                                    curComboNo = tmp;
                                    c.Cells[j, DrawColumn].Text = "┏";
                                    try
                                    {
                                        if (c.Cells[j - 1, DrawColumn].Text == "┃")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "┗";
                                        }
                                        else if (c.Cells[j - 1, DrawColumn].Text == "┏")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "";
                                        }
                                    }
                                    catch { }
                                }
                                else if (curComboNo == tmp && tmp != "")
                                {
                                    c.Cells[j, DrawColumn].Text = "┃";
                                }
                                else if (curComboNo != tmp && tmp == "")
                                {
                                    try
                                    {
                                        if (c.Cells[j - 1, DrawColumn].Text == "┃")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "┗";
                                        }
                                        else if (c.Cells[j - 1, DrawColumn].Text == "┏")
                                        {
                                            c.Cells[j - 1, DrawColumn].Text = "";
                                        }
                                    }
                                    catch { }
                                    c.Cells[j, DrawColumn].Text = "";
                                    curComboNo = "";
                                }
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┃") c.Cells[j, DrawColumn].Text = "┗";
                                if (j == c.RowCount - 1 && c.Cells[j, DrawColumn].Text == "┏") c.Cells[j, DrawColumn].Text = "";
                                //c.Cells[j, DrawColumn].ForeColor = System.Drawing.Color.Red;
                                #endregion

                            }
                        }
                    }
                    break;
            }

        }

        /// <summary>
        /// 画组合号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="column"></param>
        /// 		/// <param name="DrawColumn"></param>
        public static void DrawCombo(object sender, int column, int DrawColumn)
        {
            DrawCombo(sender, column, DrawColumn, 0);
        }

        /// <summary>
        /// 画组合号(左侧）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="column"></param>
        /// 		/// <param name="DrawColumn"></param>
        public static void DrawComboLeft(object sender, int column, int DrawColumn)
        {
            DrawComboLeft(sender, column, DrawColumn, 0);
        }

        #endregion

        #region 获得纸张大小
        protected static Neusoft.HISFC.BizLogic.Manager.PageSize manager = new Neusoft.HISFC.BizLogic.Manager.PageSize();

        /// <summary>
        /// 设置打印纸张
        /// 只对非默认纸张A4需要进行设置
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="print"></param>
        public static void GetPageSize(Neusoft.HISFC.Models.Base.PageSize p, ref Neusoft.FrameWork.WinForms.Classes.Print print)
        {
            print.SetPageSize(p);
        }

        /// <summary>
        /// 设置打印纸张
        /// 只对非默认纸张A4需要进行设置
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="print"></param>
        public static void GetPageSize(string ID, ref Neusoft.FrameWork.WinForms.Classes.Print print)
        {

            Neusoft.HISFC.Models.Base.PageSize p = manager.GetPageSize(ID);
            if (p == null || p.Name.Trim() == "") return;
            print.SetPageSize(p);
            //manager = null;
        }
        /// <summary>
        /// 设置打印纸张
        /// 需要传Transaction
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="print"></param>
        /// <param name="t"></param>
        public static void GetPageSize(string ID, ref Neusoft.FrameWork.WinForms.Classes.Print print, ref Neusoft.FrameWork.Management.Transaction t)
        {
            try
            {
                manager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            }
            catch { }
            Neusoft.HISFC.Models.Base.PageSize p = manager.GetPageSize(ID);
            if (p == null || p.Name.Trim() == "") return;
            print.SetPageSize(p);
            //manager = null;
        }
        /// <summary>
        /// 获得打印纸张
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Neusoft.HISFC.Models.Base.PageSize GetPageSize(string ID)
        {
            return manager.GetPageSize(ID);
        }
        #endregion

        #region "ISql interface访问"
        //private static Neusoft.FrameWork.Management.Interface sql = null;
        ///// <summary>
        ///// sql 住院访问类
        ///// </summary>
        //public static Neusoft.FrameWork.Management.Interface ISql
        //{
        //    get
        //    {
        //        if (sql == null)
        //        {
        //            sql = new Neusoft.FrameWork.Management.Interface();
        //            string fileName = TemplateDesignerHost.Function.SystemPath + "PATIENTINFO.xml";
        //            sql.ReadXML(fileName);
        //            try
        //            {
        //                sql.SetParam(Neusoft.FrameWork.Management.Connection.Operator.ID, "");
        //                sql.RefreshVariant();
        //            }
        //            catch { }
        //            return sql;
        //        }
        //        else
        //        {
        //            return sql;
        //        }
        //    }
        //}
        //private static Neusoft.FrameWork.Management.Interface sqlOutPatient = null;
        ///// <summary>
        ///// sql 门诊访问类
        ///// </summary>
        //public static Neusoft.FrameWork.Management.Interface ISqlOutPatient
        //{
        //    get
        //    {
        //        if (sqlOutPatient == null)
        //        {
        //            sqlOutPatient = new Neusoft.FrameWork.Management.Interface();
        //            string fileName = TemplateDesignerHost.Function.SystemPath + "OUTPATIENTINFO.xml";
        //            sqlOutPatient.ReadXML(fileName);
        //            try
        //            {
        //                sqlOutPatient.SetParam(Neusoft.FrameWork.Management.Connection.Operator.ID, "");
        //                sqlOutPatient.RefreshVariant();
        //            }
        //            catch { }
        //            return sqlOutPatient;
        //        }
        //        else
        //        {
        //            return sqlOutPatient;
        //        }
        //    }
        //}
        //private static Neusoft.FrameWork.Management.Interface sqlCheck = null;
        ///// <summary>
        ///// sql 体检访问类
        ///// </summary>
        //public static Neusoft.FrameWork.Management.Interface ISqlCheck
        //{
        //    get
        //    {
        //        if (sqlCheck == null)
        //        {
        //            sqlCheck = new Neusoft.FrameWork.Management.Interface();
        //            string fileName = TemplateDesignerHost.Function.SystemPath + "CHECKPATIENTINFO.xml";
        //            sqlCheck.ReadXML(fileName);
        //            try
        //            {
        //                sqlCheck.SetParam(Neusoft.FrameWork.Management.Connection.Operator.ID, "");
        //                sqlCheck.RefreshVariant();
        //            }
        //            catch { }
        //            return sqlCheck;
        //        }
        //        else
        //        {
        //            return sqlCheck;
        //        }
        //    }
        //}

        //private static Neusoft.FrameWork.Management.Interface sqlOther = null;
        ///// <summary>
        ///// sql 体检访问类
        ///// </summary>
        //public static Neusoft.FrameWork.Management.Interface ISqlOther
        //{
        //    get
        //    {
        //        if (sqlOther == null)
        //        {
        //            sqlOther = new Neusoft.FrameWork.Management.Interface();
        //            string fileName = Neusoft.FrameWork.Management.Connection.SystemPath + "\\OtherPATIENTINFO.xml";
        //            sqlOther.ReadXML(fileName);
        //            try
        //            {
        //                sqlOther.SetParam(Neusoft.FrameWork.Management.Connection.Operator.ID, "");
        //                sqlOther.RefreshVariant();
        //            }
        //            catch { }
        //            return sqlOther;
        //        }
        //        else
        //        {
        //            return sqlOther;
        //        }
        //    }
        //}

        #endregion


        #region 电子病历使用的单据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="patientInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //public static int EMRPrint(Control parentControl, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string type)
        //{
        //    TemplateDesignerApplication.ucDataFileLoader ucDataFileLoader1 = new TemplateDesignerApplication.ucDataFileLoader();
        //    string[] param = { Neusoft.FrameWork.Management.Connection.Operator.ID, patientInfo.ID };
        //    ucDataFileLoader1.Location = new Point(0, 2000);
        //    ucDataFileLoader1.ISql = Neusoft.HISFC.Components.Common.Classes.Function.ISql;
        //    ucDataFileLoader1.InitSql("", param);
        //    ucDataFileLoader1.Init(type, patientInfo.ID);//3 出院通知单
        //    ucDataFileLoader1.index1 = patientInfo.ID;
        //    ucDataFileLoader1.index2 = patientInfo.Name;
        //    ucDataFileLoader1.IsShowInterface = false;
        //    ucDataFileLoader1.RefreshForm();
        //    ucDataFileLoader1.Visible = true;
        //    parentControl.Controls.Add(ucDataFileLoader1);

        //    Neusoft.FrameWork.WinForms.Classes.Print p = new Neusoft.FrameWork.WinForms.Classes.Print();
        //    int i = p.PrintPage(0, 0, ucDataFileLoader1.CurrntPanel);
        //    ucDataFileLoader1.Visible = false;
        //    return i;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="parentControl"></param>
        ///// <param name="patientInfo"></param>
        ///// <param name="type"></param>
        ///// <param name="printer"></param>
        ///// <returns></returns>
        //public static int EMRPrint(Control parentControl, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string type, Neusoft.FrameWork.WinForms.Classes.Print printer)
        //{
        //    TemplateDesignerApplication.ucDataFileLoader ucDataFileLoader1 = new TemplateDesignerApplication.ucDataFileLoader();
        //    string[] param = { Neusoft.FrameWork.Management.Connection.Operator.ID, patientInfo.ID };
        //    ucDataFileLoader1.Location = new Point(0, 2000);
        //    ucDataFileLoader1.ISql = Neusoft.HISFC.Components.Common.Classes.Function.ISql;
        //    ucDataFileLoader1.InitSql("", param);
        //    ucDataFileLoader1.Init(type, patientInfo.ID);//3 出院通知单
        //    ucDataFileLoader1.index1 = patientInfo.ID;
        //    ucDataFileLoader1.index2 = patientInfo.Name;
        //    ucDataFileLoader1.IsShowInterface = false;
        //    ucDataFileLoader1.RefreshForm();
        //    ucDataFileLoader1.Visible = true;
        //    parentControl.Controls.Add(ucDataFileLoader1);
        //    int i = printer.PrintPage(0, 0, ucDataFileLoader1.CurrntPanel);
        //    ucDataFileLoader1.Visible = false;
        //    return i;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="parentControl"></param>
        ///// <param name="patientInfo"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static int EMRPrintPreview(Control parentControl, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string type)
        //{
        //    TemplateDesignerApplication.ucDataFileLoader ucDataFileLoader1 = new TemplateDesignerApplication.ucDataFileLoader();
        //    string[] param = { Neusoft.FrameWork.Management.Connection.Operator.ID, patientInfo.ID };
        //    ucDataFileLoader1.Location = new Point(0, 2000);
        //    ucDataFileLoader1.ISql = Neusoft.HISFC.Components.Common.Classes.Function.ISql;
        //    ucDataFileLoader1.InitSql("", param);
        //    ucDataFileLoader1.Init(type, patientInfo.ID);//3 出院通知单
        //    ucDataFileLoader1.index1 = patientInfo.ID;
        //    ucDataFileLoader1.index2 = patientInfo.Name;
        //    ucDataFileLoader1.IsShowInterface = false;
        //    ucDataFileLoader1.RefreshForm();
        //    ucDataFileLoader1.Visible = true;
        //    parentControl.Controls.Add(ucDataFileLoader1);
        //    Neusoft.FrameWork.WinForms.Classes.Print p = new Neusoft.FrameWork.WinForms.Classes.Print();
        //    int i = p.PrintPreview(ucDataFileLoader1.CurrntPanel);
        //    ucDataFileLoader1.Visible = false;
        //    return i;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="parentControl"></param>
        ///// <param name="patientInfo"></param>
        ///// <param name="type"></param>
        ///// <param name="printer"></param>
        ///// <returns></returns>
        //public static int EMRPrintPreview(Control parentControl, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string type, Neusoft.FrameWork.WinForms.Classes.Print printer)
        //{
        //    TemplateDesignerApplication.ucDataFileLoader ucDataFileLoader1 = new TemplateDesignerApplication.ucDataFileLoader();
        //    string[] param = { Neusoft.FrameWork.Management.Connection.Operator.ID, patientInfo.ID };
        //    ucDataFileLoader1.Location = new Point(0, 2000);
        //    ucDataFileLoader1.ISql = Neusoft.HISFC.Components.Common.Classes.Function.ISql;
        //    ucDataFileLoader1.InitSql("", param);
        //    ucDataFileLoader1.Init(type, patientInfo.ID);//3 出院通知单
        //    ucDataFileLoader1.index1 = patientInfo.ID;
        //    ucDataFileLoader1.index2 = patientInfo.Name;
        //    ucDataFileLoader1.IsShowInterface = false;
        //    ucDataFileLoader1.RefreshForm();
        //    ucDataFileLoader1.Visible = true;
        //    parentControl.Controls.Add(ucDataFileLoader1);
        //    int i = printer.PrintPreview(ucDataFileLoader1.CurrntPanel);
        //    ucDataFileLoader1.Visible = false;
        //    return i;
        //}
        ///// <summary>
        ///// /
        ///// </summary>
        ///// <param name="parentControl"></param>
        ///// <param name="patientInfo"></param>
        ///// <param name="type"></param>
        //public static void EMRShow(Control parentControl, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string type)
        //{
        //    TemplateDesignerApplication.ucDataFileLoader ucDataFileLoader1 = new TemplateDesignerApplication.ucDataFileLoader();
        //    string[] param = { Neusoft.FrameWork.Management.Connection.Operator.ID, patientInfo.ID };
        //    ucDataFileLoader1.Location = new Point(0, 0);
        //    ucDataFileLoader1.Dock = DockStyle.Fill;
        //    ucDataFileLoader1.ISql = Neusoft.HISFC.Components.Common.Classes.Function.ISql;
        //    ucDataFileLoader1.InitSql("", param);
        //    ucDataFileLoader1.Init(type, patientInfo.ID);
        //    ucDataFileLoader1.index1 = patientInfo.ID;
        //    ucDataFileLoader1.index2 = patientInfo.Name;
        //    ucDataFileLoader1.IsShowInterface = false;
        //    ucDataFileLoader1.RefreshForm();
        //    ucDataFileLoader1.Visible = true;
        //    parentControl.Controls.Add(ucDataFileLoader1);
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="parentControl"></param>
        ///// <param name="patientInfo"></param>
        ///// <param name="type"></param>
        ///// <param name="bShowInterface"></param>
        //public static void EMRShow(Control parentControl, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string type, bool bShowInterface)
        //{
        //    TemplateDesignerApplication.ucDataFileLoader ucDataFileLoader1 = new TemplateDesignerApplication.ucDataFileLoader();
        //    string[] param = { Neusoft.FrameWork.Management.Connection.Operator.ID, patientInfo.ID };
        //    ucDataFileLoader1.Location = new Point(0, 0);
        //    ucDataFileLoader1.Dock = DockStyle.Fill;
        //    ucDataFileLoader1.ISql = Neusoft.HISFC.Components.Common.Classes.Function.ISql;
        //    ucDataFileLoader1.InitSql("", param);
        //    ucDataFileLoader1.Init(type, patientInfo.ID);
        //    ucDataFileLoader1.index1 = patientInfo.ID;
        //    ucDataFileLoader1.index2 = patientInfo.Name;
        //    ucDataFileLoader1.IsShowInterface = bShowInterface;
        //    ucDataFileLoader1.RefreshForm();
        //    ucDataFileLoader1.Visible = true;
        //    parentControl.Controls.Add(ucDataFileLoader1);
        //}
        #endregion

        /// <summary>
        /// 创建配置文件 feeSetting.xml 提供保存输入法
        /// </summary>
        /// <returns></returns>
        public static int CreateFeeSetting()
        {
            try
            {
                XmlDocument docXml = new XmlDocument();
                if (System.IO.File.Exists(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "/feeSetting.xml"))
                {
                    System.IO.File.Delete(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "/feeSetting.xml");
                }
                else
                {
                    System.IO.Directory.CreateDirectory(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath);
                }
                docXml.LoadXml("<setting>  </setting>");
                XmlNode root = docXml.DocumentElement;

                XmlElement elem1 = docXml.CreateElement("输入法");
                System.Xml.XmlComment xmlcomment;
                xmlcomment = docXml.CreateComment("查询方式0:拼音码 1:五笔码 2:自定义码 3:国标码 4:英文");
                elem1.SetAttribute("currentmodel", "0");
                root.AppendChild(xmlcomment);
                root.AppendChild(elem1);

                XmlElement elem2 = docXml.CreateElement("IME");
                System.Xml.XmlComment xmlcomment2;
                xmlcomment2 = docXml.CreateComment("当前默认输入法");
                elem2.SetAttribute("currentmodel", "紫光拼音输入法");
                root.AppendChild(xmlcomment2);
                root.AppendChild(elem2);

                docXml.Save(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "/feeSetting.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show("CreateFeeSetting" + ex.Message);
                return -1;
            }
            return 1;
        }

        static Hashtable hsUCULItem = null;

        /// <summary>
        /// 是否允许开立长嘱
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool isUCUCForLong(Neusoft.HISFC.Models.Base.Item item)
        {
            if ("UC、UL".Contains(item.SysClass.ID.ToString()))
            {
                if (hsUCULItem == null)
                {
                    hsUCULItem = new Hashtable();
                    Neusoft.HISFC.BizProcess.Integrate.Manager interMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();
                    ArrayList alUCUL = interMgr.GetConstantList("LongUCUL");
                    foreach (Neusoft.HISFC.Models.Base.Const con in alUCUL)
                    {
                        if (con.IsValid && !hsUCULItem.Contains(con.ID))
                        {
                            hsUCULItem.Add(con.ID, null);
                        }
                    }
                }
                if (hsUCULItem != null
                    && hsUCULItem.Contains(item.ID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 创建配置文件 feeSettingOutpatientFee.xml 提供保存输入法
        /// </summary>
        /// <returns></returns>
        public static int CreateFeeSettingOutpatient()
        {
            try
            {
                XmlDocument docXml = new XmlDocument();
                if (System.IO.File.Exists(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "/feeSettingOutpatientFee.xml"))
                {
                    System.IO.File.Delete(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "/feeSettingOutpatientFee.xml");
                }
                else
                {
                    System.IO.Directory.CreateDirectory(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath);
                }
                docXml.LoadXml("<setting>  </setting>");
                XmlNode root = docXml.DocumentElement;

                XmlElement elem1 = docXml.CreateElement("排序");
                System.Xml.XmlComment xmlcomment;
                xmlcomment = docXml.CreateComment("查询方式0:拼音码 1:五笔码 2:自定义码");
                elem1.SetAttribute("currentmodel", "0");
                root.AppendChild(xmlcomment);
                root.AppendChild(elem1);

                docXml.Save(Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + "/feeSettingOutpatientFee.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show("CreateFeeSetting" + ex.Message);
                return -1;
            }
            return 1;
        }


        #region 数据库操作

        /// <summary>
        /// 查询痕迹保留Sql
        /// </summary>
        private static string insertSql = @"INSERT INTO COM_QUERY_LOG (
	REPORT_ID ,                             --报表ID
	REPORT_NAME ,                           --报表名称
	REPORT_CONDITION ,                      --查询条件
	QUERY_OPER ,                            --查询人
	LOGIN_FUN ,                             --登陆功能组
	LOGIN_DEPT ,                            --登陆科室
	MACHINE_NAME ,                          --机器名称
	MACHINE_IP ,                            --登陆IP
	QUERY_DATE                              --查询时间
)  VALUES(
	'{0}' ,       --报表ID
	'{1}' ,       --报表名称
	'{2}' ,       --查询条件
	'{3}' ,       --查询人
	'{4}' ,       --登陆功能组
	'{5}' ,       --登陆科室
	'{6}' ,       --机器名称
	'{7}' ,       --登陆IP
	to_date('{8}','yyyy-mm-dd HH24:mi:ss')         --查询时间
) 
";

        #endregion



        /// <summary>
        /// 图标通知
        /// </summary>
        static System.Windows.Forms.NotifyIcon notify = null;

        /// <summary>
        /// 图标通知事件
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="tipTitle"></param>
        /// <param name="tipText"></param>
        /// <param name="tipIcon"></param>
        public static void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            if (notify == null)
            {
                notify = new System.Windows.Forms.NotifyIcon();
                notify.Icon = Properties.Resources.HIS;
            }
            notify.Visible = true;
            notify.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        /// <summary>
        /// 敏感信息查询痕迹保留函数
        /// </summary>
        /// <param name="reportID">报表ID</param>
        /// <param name="reportName">报表名称</param>
        /// <param name="reportCondition">查询条件</param>
        public static void SaveQueryLog(string reportID, string reportName, string reportCondition)
        {
            Neusoft.HISFC.Models.Base.Employee person = new Neusoft.HISFC.Models.Base.Employee();
            Neusoft.FrameWork.Management.DataBaseManger dataManager = new Neusoft.FrameWork.Management.DataBaseManger();

            string hosName = System.Net.Dns.GetHostName();
            string ip = System.Net.Dns.GetHostEntry(hosName).AddressList[0].ToString();

            person = dataManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            if (person == null)
            {
                return;
            }

            //person.ID = person.ID;							//操作员ID
            //person.Name = person.Name;						//操作员姓名
            //person.Memo = person.Dept.ID;					//登陆科室名
            //person.User01 = person.Dept.Name;				//登陆科室ID
            //person.User02 = hosName;						//主机名称
            //person.User03 = ip;								//IP
            //person.CurrentGroup.Name = person.CurrentGroup.Name;	//登陆组名
            //person.CurrentGroup.ID = person.CurrentGroup.ID;		//登记组ID

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(dataManager.Connection);
            //t.BeginTransaction();
            dataManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            string sql = string.Format(Function.insertSql,
                                        reportID,
                                        reportName,
                                        reportCondition,
                                        person.ID,
                                        person.CurrentGroup.Name,
                                        person.Dept.ID,
                                        hosName,
                                        ip,
                                        dataManager.GetDateTimeFromSysDateTime().ToString());
            int parm = dataManager.ExecNoQuery(sql);
            if (parm == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return;
            }
            if (parm > 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.Commit();
                return;
            }
        }

        #region 输入法全角切换成半角

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hwnd);
        [DllImport("imm32.dll")]
        public static extern bool ImmGetOpenStatus(IntPtr himc);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetOpenStatus(IntPtr himc, bool b);
        [DllImport("imm32.dll")]
        public static extern bool ImmGetConversionStatus(IntPtr himc, ref int lpdw, ref int lpdw2);
        [DllImport("imm32.dll")]
        public static extern int ImmSimulateHotKey(IntPtr hwnd, int lngHotkey);
        public const int IME_CMODE_FULLSHAPE = 0x8;
        public const int IME_CHOTKEY_SHAPE_TOGGLE = 0x11;

        /// <summary>
        /// 设置窗体控件中的输入法状态为半角
        /// </summary>
        /// <param name="frm">当前窗体</param>
        public static void SetIme(Form frm)
        {
            frm.Paint += new PaintEventHandler(frm_Paint);
            ChangeAllControl(frm);
        }
        /// <summary>
        /// 设置控件的输入法状态为半角
        /// </summary>
        /// <param name="ctl">控件</param>
        public static void SetIme(Control ctl)
        {
            ChangeAllControl(ctl);
        }
        /// <summary>
        /// 设置对象的输入法状态为半角
        /// </summary>
        /// <param name="Handel">对象句柄</param>
        public static void SetIme(IntPtr Handel)
        {
            ChangeControlIme(Handel);
        }
        private static void ChangeAllControl(Control ctl)
        {
            //在控件的的Enter事件中触发来调整输入法状态


            ctl.Enter += new EventHandler(ctl_Enter);
            //遍历子控件，使每个控件都用上Enter的委托处理


            foreach (Control ctlChild in ctl.Controls)
            {
                ChangeAllControl(ctlChild);
            }
        }

        static void frm_Paint(object sender, PaintEventArgs e)
        {
            ChangeControlIme(sender);
        }
        //控件的Enter处理程序
        static void ctl_Enter(object sender, EventArgs e)
        {
            ChangeControlIme(sender);
        }
        private static void ChangeControlIme(object sender)
        {
            Control ctl = (Control)sender;
            ChangeControlIme(ctl.Handle);
        }
        /// <summary>
        /// 下面这个函数才是真正检查输入法的全角半角状态


        /// </summary>
        /// <param name="h"></param>
        private static void ChangeControlIme(IntPtr h)
        {
            IntPtr HIme = ImmGetContext(h);
            if (ImmGetOpenStatus(HIme)) //如果输入法处于打开状态
            {
                int iMode = 0;
                int iSentence = 0;
                bool bSuccess = ImmGetConversionStatus(HIme, ref iMode, ref iSentence); //检索输入法信息
                if (bSuccess)
                {
                    if ((iMode & IME_CMODE_FULLSHAPE) > 0) //如果是全角
                    {
                        ImmSimulateHotKey(h, IME_CHOTKEY_SHAPE_TOGGLE); //转换成半角


                    }
                }
            }
        }

        #endregion

        //{ED51E97B-B752-4c32-BD93-F80209A24879}增加输入次数排序

        #region 输入项目使用频率排序

        /// <summary>
        /// 排序xml 文件地址
        /// </summary>
        public static string SORT_FILE_PATH = Application.StartupPath + "\\Setting\\Profiles\\itemSort.xml";

        /// <summary>
        /// 设置XML 文件位置
        /// </summary>
        public static string FEE_SET_PATH = Application.StartupPath + "\\Setting\\Profiles\\feeSetting.xml";

        /// <summary>
        /// 当前xml的实例
        /// </summary>
        public static XmlDocument xmlDoc = null;

        /// <summary>
        /// 当前设置xml实例
        /// </summary>
        public static XmlDocument xmlSetDoc = null;

        /// <summary>
        /// 是否存在过滤xml
        /// </summary>
        /// <returns>成功 true 失败 false</returns>
        private static bool IsExistSettingFeeXML()
        {
            try
            {
                return System.IO.File.Exists(FEE_SET_PATH);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获得当前项目的排序次数,如果没有找到,则默认为0
        /// </summary>
        /// <param name="xDoc">当前xml</param>
        /// <param name="itemCode">项目编码</param>
        /// <returns>成功 当前项目的排序次数 没有找到返回 0</returns>
        public static string GetDeptValue(XmlDocument xDoc)
        {
            XmlNode root = xDoc.SelectSingleNode("setting//defaultDept");

            if (root != null)
            {
                return root.InnerText;
            }

            return null;
        }

        /// <summary>
        /// 设置当前默认执行药房
        /// </summary>
        /// <param name="deptCode">科室代码</param>
        public static void SetDefaultDeptXML(string deptCode)
        {
            if (xmlSetDoc == null)
            {
                xmlSetDoc = GetSetXML();
            }

            if (xmlDoc == null)
            {
                return;
            }

            XmlNode root = xmlSetDoc.SelectSingleNode("setting");

            XmlNode xFind = root.SelectSingleNode("defaultDept");
            if (xFind == null)//没有找到,要新增一个节点
            {
                InsertSettingNewItem(deptCode, xmlSetDoc);
            }
            else//更改当前项目的使用频率 + 1 
            {
                ModifySettingItem(deptCode, xmlSetDoc, xFind);
            }
        }

        /// <summary>
        /// 添加一个新的节点,担心第一个为数字,项目编码前面添加一个大写字母"A"
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="xDoc">当前xml</param>
        private static void InsertSettingNewItem(string deptCode, XmlDocument xDoc)
        {
            if (xDoc == null)
            {
                return;
            }

            XmlElement xe = xDoc.CreateElement("defaultDept");
            xe.InnerText = deptCode;

            XmlNode root = xDoc.SelectSingleNode("Setting");

            root.AppendChild(xe);

            xDoc.Save(FEE_SET_PATH);
        }

        /// <summary>
        /// 修改一个节点
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="xDoc">当前xml</param>
        /// <param name="xe">当前节点</param>
        private static void ModifySettingItem(string deptCode, XmlDocument xDoc, XmlNode xe)
        {
            if (xDoc == null)
            {
                return;
            }

            xe.InnerText = deptCode;

            xDoc.Save(FEE_SET_PATH);
        }

        /// <summary>
        /// 是否存在过滤xml
        /// </summary>
        /// <returns>成功 true 失败 false</returns>
        private static bool IsExistSettingXML()
        {
            try
            {
                return System.IO.File.Exists(SORT_FILE_PATH);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获得当前sortXML
        /// </summary>
        /// <returns>成功 过滤xml 失败 null</returns>
        public static XmlDocument GetSortXML()
        {
            if (!IsExistSettingXML())
            {
                return null;
            }

            XmlDocument xdoc = new XmlDocument();

            xdoc.Load(SORT_FILE_PATH);

            return xdoc;
        }

        /// <summary>
        /// 获得当前sortXML
        /// </summary>
        /// <returns>成功 过滤xml 失败 null</returns>
        public static XmlDocument GetSetXML()
        {
            if (!IsExistSettingXML())
            {
                return null;
            }

            XmlDocument xdoc = new XmlDocument();

            xdoc.Load(FEE_SET_PATH);

            return xdoc;
        }

        /// <summary>
        /// 添加一个新的节点,担心第一个为数字,项目编码前面添加一个大写字母"A"
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="xDoc">当前xml</param>
        private static void InsertNewItem(string itemCode, XmlDocument xDoc)
        {
            if (xDoc == null)
            {
                return;
            }

            XmlElement xe = xDoc.CreateElement("A" + itemCode);
            xe.InnerText = "1";

            XmlNode root = xDoc.SelectSingleNode("Column");

            root.AppendChild(xe);

            xDoc.Save(SORT_FILE_PATH);
        }

        /// <summary>
        /// 修改一个节点
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="xDoc">当前xml</param>
        /// <param name="xe">当前节点</param>
        private static void ModifyItem(string itemCode, XmlDocument xDoc, XmlNode xe)
        {
            if (xDoc == null)
            {
                return;
            }

            xe.InnerText = (Neusoft.FrameWork.Function.NConvert.ToInt32(xe.InnerText) + 1).ToString();

            xDoc.Save(SORT_FILE_PATH);
        }

        /// <summary>
        /// 设置当前输入的项目是新项目加入一条排序节点,如果已经存在,更新排序次数+1
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        public static void SetSortItemXML(string itemCode)
        {
            if (xmlDoc == null)
            {
                xmlDoc = GetSortXML();
            }

            if (xmlDoc == null)
            {
                return;
            }

            XmlNode root = xmlDoc.SelectSingleNode("Column");

            XmlNode xFind = root.SelectSingleNode("A" + itemCode);
            if (xFind == null)//没有找到,要新增一个节点
            {
                InsertNewItem(itemCode, xmlDoc);
            }
            else//更改当前项目的使用频率 + 1 
            {
                ModifyItem(itemCode, xmlDoc, xFind);
            }
        }

        /// <summary>
        /// 获得当前项目的排序次数,如果没有找到,则默认为0
        /// </summary>
        /// <param name="xDoc">当前xml</param>
        /// <param name="itemCode">项目编码</param>
        /// <returns>成功 当前项目的排序次数 没有找到返回 0</returns>
        public static int GetSortValue(XmlDocument xDoc, string itemCode)
        {
            if (xDoc == null)
            {
                return 0;
            }

            XmlNode root = xDoc.SelectSingleNode("Column");

            XmlNode xFind = root.SelectSingleNode("A" + itemCode);
            if (xFind != null)
            {
                return Neusoft.FrameWork.Function.NConvert.ToInt32(xFind.InnerText);
            }

            return 0;
        }

        #endregion

        //{ED51E97B-B752-4c32-BD93-F80209A24879}增加输入次数排序完毕

        #region MQ嵌入到护士站
        //{839D3A8A-49FA-4d47-A022-6196EB1A5715} 将MQ嵌入系统，医生站保存时能自动通知护士站。护士站医嘱界面能自动响应，类似QQ的头像晃动，并可以给出声音提示

        public static string currentPath = ".";
        public static string strLabel = "";

        /// <summary>
        /// 记录删除后的MQ消息
        /// </summary>
        /// <param name="sRecord"></param>
        public static string Label(string sRecord)
        {
            FileStream fs;
            string path = currentPath + "\\Label.txt";
            if (!File.Exists(path))
            {
                fs = File.Create(path);
                fs.Close();
            }
            StreamReader sr = File.OpenText(path);
            strLabel = sr.ReadToEnd();
            strLabel = strLabel.Trim('\0') + '\0';
            sr.Close();

            if (!string.IsNullOrEmpty(sRecord))
            {
                strLabel += sRecord + "\0";

                fs = File.OpenWrite(path);
                Byte[] info = new UTF8Encoding(true).GetBytes(strLabel);
                fs.Write(info, 0, info.Length);
                fs.Close();
            }

            return strLabel;
        }

        /// <summary>
        /// 删除跑马灯提示
        /// </summary>
        /// <param name="inpatientNO"></param>
        public static void DelLabel(string inpatientNO)
        {
            Label("");

            string sRecord = "";
            string[] messageArr = strLabel.Split('\0');
            ArrayList alInpatienNO = new ArrayList();

            int start = 0;
            int end = 0;
            string inpatientNOTemp = "";

            foreach (string messageInfo in messageArr)
            {
                if (messageInfo != "")
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();

                    start = messageInfo.IndexOf("住院号：");
                    end = messageInfo.IndexOf("\n床号：");
                    if (start > -1 && end > -1 && end - start - "住院号：".Length > 0 && start - "住院号：".Length > -1)
                    {
                        inpatientNOTemp = messageInfo.Substring(start + "住院号：".Length, end - start - "住院号：".Length);
                    }

                    //if (messageInfo.Substring(messageInfo.IndexOf("ZY"), 14) == inpatientNO)
                    if (inpatientNOTemp == inpatientNO)
                    {
                        sRecord += messageInfo + '\0';
                    }
                }
            }

            if (strLabel.IndexOf(sRecord) < 0)
            {
                return;
            }
            strLabel = strLabel.Remove(strLabel.LastIndexOf(sRecord), sRecord.Length);

            if (File.Exists(currentPath + "\\Label.txt"))
            {
                File.Delete(currentPath + "\\Label.txt");
            }
            Label(strLabel);
        }

        #endregion

        /// <summary>
        /// 查询患者信息
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public static int QueryComPatientInfo(ref Neusoft.HISFC.Models.RADT.PatientInfo patient)
        {
            ucQueryPatientInfo uc = new ucQueryPatientInfo();
            DialogResult diaResult = Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(uc);
            if (diaResult == DialogResult.OK)
            {
                patient = uc.Patient;
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 项目扩展信息接口
        /// </summary>
        private static Neusoft.HISFC.BizProcess.Interface.Common.IItemCompareInfo iItemCompareInfo = null;

        /// <summary>
        /// 项目扩展信息接口
        /// </summary>
        public static Neusoft.HISFC.BizProcess.Interface.Common.IItemCompareInfo IItemCompareInfo
        {
            get
            {
                if (iItemCompareInfo == null)
                {
                    iItemCompareInfo = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(typeof(Neusoft.HISFC.Components.Common.Controls.ucInputItem), typeof(Neusoft.HISFC.BizProcess.Interface.Common.IItemCompareInfo)) as Neusoft.HISFC.BizProcess.Interface.Common.IItemCompareInfo;
                }
                return Function.iItemCompareInfo;
            }
        }

        /// <summary>
        /// 判断是否自费药品
        /// </summary>
        /// <param name="drugCode"></param>
        /// <returns></returns>
        public static bool IsOwnFeeItem(string drugCode)
        {
            if (OwnFeeDrugList==null || OwnFeeDrugList.Count==0)
            {
                return false;
            }
            foreach (Neusoft.FrameWork.Models.NeuObject item in OwnFeeDrugList)
            {
                if (item.ID==drugCode)
                {
                    return true;
                }
            }
            return false;
        }

        ///// <summary>
        ///// 深拷贝
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static object CloneObject(object obj)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    object CloneObject;
        //    BinaryFormatter bf = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
        //    bf.Serialize(ms, obj);
        //    ms.Seek(0, SeekOrigin.Begin);
        //    CloneObject = bf.Deserialize(ms);
        //    ms.Close();
        //    return CloneObject;
        //}

        #region 获取药品项目基本信息

        /// <summary>
        /// 获取药品项目基本信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Neusoft.HISFC.Models.Pharmacy.Item GetItem(Neusoft.HISFC.Models.Pharmacy.Item item)
        {
            Neusoft.HISFC.Models.Pharmacy.Item baseDrug = null;
            baseDrug = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(item.ID);
            if (baseDrug == null || string.IsNullOrEmpty(baseDrug.ID))
            {
                baseDrug = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItemFHY(item.ID);
            }
            return baseDrug;
        }

        /// <summary>
        /// 获取药品项目基本信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Neusoft.HISFC.Models.Pharmacy.Item GetItem(string itemID)
        {
            Neusoft.HISFC.Models.Pharmacy.Item baseDrug = null;
            baseDrug = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemID);
            if (baseDrug == null || string.IsNullOrEmpty(baseDrug.ID))
            {
                baseDrug = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItemFHY(itemID);
            }
            return baseDrug;
        }

        #endregion
    }


    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogManager
    {
        public static void Write(string logInfo)
        {
            //检查目录是否存在
            if (System.IO.Directory.Exists("./Log/Order/InOrder") == false)
            {
                System.IO.Directory.CreateDirectory("./Log/Order/InOrder");
            }
            Neusoft.HISFC.BizLogic.Manager.DataBase dbMgr = new Neusoft.HISFC.BizLogic.Manager.DataBase();
            DateTime dtNow = dbMgr.GetDateTimeFromSysDateTime();

            //保存一周的日志
            System.IO.File.Delete("./Log/Order/OutOrder/" + dtNow.AddDays(-7).ToString("yyyyMMdd") + ".LOG");
            string name = dtNow.ToString("yyyyMMdd");
            System.IO.StreamWriter w = new System.IO.StreamWriter("./Log/Order/OutOrder/" + name + ".LOG", true);
            w.WriteLine(dtNow.ToString("yyyy-MM-dd HH:mm:ss") + logInfo);
            w.Flush();
            w.Close();
        }
    }
}
