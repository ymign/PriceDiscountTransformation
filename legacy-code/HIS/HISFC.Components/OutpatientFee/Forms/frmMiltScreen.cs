using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.FrameWork.Management;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    public partial class frmMiltScreen : Form, Neusoft.HISFC.BizProcess.Interface.FeeInterface.IMultiScreen
    {
        public frmMiltScreen()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 项目信息
        /// </summary>
        private DataSet dsItem = null;

        /// <summary>
        /// 费用待遇接口
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareInterfaceProxy = null;

        /// <summary>
        /// 属于药品的最小费用列表
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper drugFeeCodeHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 管理业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        #endregion

        #region 属性

        /// <summary>
        /// 属于药品的最小费用列表
        /// </summary>
        public Neusoft.FrameWork.Public.ObjectHelper DrugFeeCodeHelper 
        {
            set 
            {
                this.drugFeeCodeHelper = value;
            }
        }

        /// <summary>
        /// 设置待遇接口变量
        /// </summary>
        public Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy MedcareInterfaceProxy 
        {
            set 
            {
                this.medcareInterfaceProxy = value;
            }
        }
        private bool isPreeFee = false;
        ///
        /// <summary>
        /// 医保患者是否预结算
        /// </summary>
        public bool IsPreFee
        {
            set
            {
                this.isPreeFee = value;
            }
            get
            {
                return this.isPreeFee;
            }
        }


        #endregion

        #region 方法

        /// <summary>
        /// 设置属于药品的最小费用列表
        /// </summary>
        /// <param name="drugFeeCodeHelper">属于药品的最小费用列表</param>
        public void SetFeeCodeIsDrugArrayListObj(Neusoft.FrameWork.Public.ObjectHelper drugFeeCodeHelper) 
        {
            this.drugFeeCodeHelper = drugFeeCodeHelper;
        }

        /// <summary>
        /// 设置待遇接口变量
        /// </summary>
        /// <param name="medcareInterfaceProxy">接口变量</param>
        public void SetMedcareInterfaceProxy(Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareInterfaceProxy) 
        {
            this.medcareInterfaceProxy = medcareInterfaceProxy;
        }

        /// <summary>
        /// 设置显示信息
        /// </summary>
        /// <param name="patient">挂号基本信息</param>
        /// <param name="ft">计算后的费用分项信息</param>
        /// <param name="feeItemLists">费用明细基本信息</param>
        /// <param name="diagLists">诊断信息</param>
        /// <param name="otherInfomations">其他信息</param>
        public void SetInfomation(Neusoft.HISFC.Models.Registration.Register patient, Neusoft.HISFC.Models.Base.FT ft, ArrayList feeItemLists, ArrayList diagLists,
            params string[] otherInfomations)
        {            
            if (this.medcareInterfaceProxy == null)
            {
                this.medcareInterfaceProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();
            }
            
            if (this.medcareInterfaceProxy == null) 
            {
                return;
            }

            if (patient == null) 
            {
                this.lblPaientInfo.Text = "";
                return;
            }
            
            this.medcareInterfaceProxy.SetPactCode(patient.Pact.ID);
            // {293FDD11-FC10-4ceb-8E4C-1A4304F22592}
            this.medcareInterfaceProxy.IsLocalProcess = false;

            if (feeItemLists == null)// || feeItemLists.Count <= 0)
            {
                if (ft != null)
                {
                    if (patient.Pact.PayKind.ID == "01" || patient.Pact.PayKind.ID == "03")
                    {
                        this.tbDrugSendInfo.Text = ft.User01;
                        if (string.IsNullOrEmpty(tbDrugSendInfo.Text))
                        {
                            tbDrugSendInfo.Text = "取药药房";
                        }
                        else
                        {
                            tbDrugSendInfo.Text = "请到  " + tbDrugSendInfo + "  取药";
                        }
                    }
                    this.tbRealOwnCost.Text = ft.RealCost.ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbReturnCost.Text = ft.ReturnCost.ToString("F4").TrimEnd('0').TrimEnd('.');
                }
                //this.Clear();
                this.lblPaientInfo.Text = patient.Name + "  正在办理划价交费，请稍候！";
            }

            #region 作废
            //{CC93C88A-9DD0-49fe-9DC0-B6DA445A7F30}根据参数判断、不区分是否是医保患者了，
            //要不该参数控制不了自费患者。--修改新乡分币时发现的问题
            if (false)
            {
                long returnValue = this.medcareInterfaceProxy.DeleteUploadedFeeDetailsOutpatient(patient, ref feeItemLists);
                if (returnValue == -1)
                {
                    MessageBox.Show(Language.Msg("待遇接口上传明细失败!") + this.medcareInterfaceProxy.ErrMsg);

                    return;
                }
                returnValue = this.medcareInterfaceProxy.UploadFeeDetailsOutpatient(patient, ref feeItemLists);
                if (returnValue == -1)
                {
                    MessageBox.Show(Language.Msg("待遇接口上传明细失败!") + this.medcareInterfaceProxy.ErrMsg);

                    return;
                }

                returnValue = this.medcareInterfaceProxy.PreBalanceOutpatient(patient, ref feeItemLists);
                if (returnValue == -1)
                {
                    MessageBox.Show(Language.Msg("待遇接口预结算计算失败!") + this.medcareInterfaceProxy.ErrMsg);

                    return;
                }
                //清除上传明细,结算再传
                this.medcareInterfaceProxy.Rollback();
            }
            #endregion

            decimal sumTotCost = 0, sumPayCost = 0, sumPubCost = 0, sumOwnCost = 0; decimal sumDrugCost = 0;

            if(feeItemLists != null)
            {
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in feeItemLists)
                {
                    if (f.IsAccounted)
                    {
                        if (f.FT.OwnCost > 0)
                        {
                            sumPayCost += f.FT.OwnCost;
                            sumOwnCost += 0;
                        }
                        else 
                        {
                            sumPayCost += f.FT.PayCost;
                        }
                        
                    }
                    else
                    {
                        sumTotCost += f.FT.TotCost;
                        sumPayCost += f.FT.PayCost;
                        //{C623A693-19A7-4378-859D-5C07CFF9BEB1}
                        sumPubCost += f.FT.PubCost + f.FT.RebateCost ;
                        sumOwnCost += f.FT.OwnCost - f.FT.RebateCost;
                    }
                    if (this.drugFeeCodeHelper.ArrayObject != null && this.drugFeeCodeHelper.ArrayObject.Count > 0 && this.drugFeeCodeHelper.GetObjectFromID(f.Item.MinFee.ID) != null)
                    {
                        sumDrugCost += f.FT.OwnCost + f.FT.PayCost + f.FT.PubCost;
                    }
                }
            
                if (patient.Pact.PayKind.ID == "01")
                {
                    this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumOwnCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbPayCost.Text = sumPayCost.ToString("F4").TrimEnd('0').TrimEnd('.');

                    this.tbPubCost.Text = sumPubCost.ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbDrugCost.Text = sumDrugCost.ToString("F4").TrimEnd('0').TrimEnd('.');

                    this.lblPayTitle.Visible = false;
                    this.tbPayCost.Visible = false;
                    this.lblPubTitle.Visible = false;
                    this.tbPubCost.Visible = false;
                }
                else if (patient.Pact.PayKind.ID == "03")
                {
                    this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumOwnCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumPayCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbPubCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumPubCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbDrugCost.Text = sumDrugCost.ToString("F4").TrimEnd('0').TrimEnd('.');

                    this.lblPayTitle.Visible = false;
                    this.tbPayCost.Visible = false;
                    this.lblPubTitle.Visible = true;
                    this.tbPubCost.Visible = true;
                    this.lblPubTitle.Text = "公费：";
                }
                else if (patient.Pact.PayKind.ID == "02")
                {
                    this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.OwnCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.TotCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbPayCost.Text = "0.00";
                    this.tbPubCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.PubCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                    this.tbDrugCost.Text = sumDrugCost.ToString("F4").TrimEnd('0').TrimEnd('.');

                    this.lblPayTitle.Visible = true;
                    this.tbPayCost.Visible = true;
                    this.lblPubTitle.Visible = true;
                    this.tbPubCost.Visible = true;
                    this.lblPubTitle.Text = "医保：";
                }
            }

            if (ft != null)
            {
                this.tbDrugSendInfo.Text = ft.User01;

                if (string.IsNullOrEmpty(tbDrugSendInfo.Text))
                {
                    tbDrugSendInfo.Text = "取药药房";
                }
                else
                {
                    tbDrugSendInfo.Text = "请到 " + tbDrugSendInfo.Text + " 取药";
                }

                if (ft.RealCost == 0)
                {
                    ft.RealCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.tbOwnCost.Text);
                }

                this.tbRealOwnCost.Text = ft.RealCost.ToString("F4").TrimEnd('0').TrimEnd('.');
                this.tbReturnCost.Text = ft.ReturnCost.ToString("F4").TrimEnd('0').TrimEnd('.');
                Decimal ownpay = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.tbOwnCost.Text) + Neusoft.FrameWork.Function.NConvert.ToDecimal(this.tbPayCost.Text);
                this.tbPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.PayCost, 2).ToString("F4").TrimEnd('0').TrimEnd('.');
                this.lblPaientInfo.Text = patient.Name + "  应缴金额为 " + ownpay.ToString("F4").TrimEnd('0').TrimEnd('.') + " 元";
            }

            #endregion
        }

        /// <summary>
        /// 设置项目信息
        /// </summary>
        /// <param name="dsItem">项目信息集合</param>
        public void SetDataSet(DataSet dsItem) 
        {
            this.dsItem = dsItem;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public int Init() 
        {
            if (this.drugFeeCodeHelper != null && (this.drugFeeCodeHelper.ArrayObject == null || this.drugFeeCodeHelper.ArrayObject.Count == 0))
            {
                ArrayList drugFeeCodeList = this.managerIntegrate.GetConstantList("DrugMinFee");
                if (drugFeeCodeList == null)
                {
                    MessageBox.Show(Language.Msg("获得药品最小费用列表出错!") + this.managerIntegrate.Err);

                    return -1;
                }
                
                this.drugFeeCodeHelper.ArrayObject = drugFeeCodeList;
            }

            return 1;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() 
        {
            this.tbDrugCost.Text = "0.00";
            this.tbDrugSendInfo.Text = "取药药房";
            this.tbRealOwnCost.Text = "0.00";
            this.tbReturnCost.Text = "0.00";
            this.tbOwnCost.Text = "0.00";
            this.tbTotCost.Text = "0.00";
            this.tbPayCost.Text = "0.00";
            this.tbPubCost.Text = "0.00";
            this.tbDrugCost.Text = "0.00";
            //this.lblPaientInfo.Text = patient.Name + "  正在办理划价交费，请稍后";
        }

        #region IMultiScreen 成员

        public System.Collections.Generic.List<Object> ListInfo
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.Clear();
                 //this.SetInfomation(
                //Neusoft.HISFC.Models.Registration.Register register
                //    ,Neusoft.HISFC.Models.Base.FT ft
                //        , ArrayList feeItemLists
                //,ArrayList diagLists
                //, params string[] otherInfomations);
                if (value != null)
                {
                    this.SetInfomation(
                   value[0] as Neusoft.HISFC.Models.Registration.Register
                        , value[1] as Neusoft.HISFC.Models.Base.FT
                            , value[2] as ArrayList
                    , value[3] as ArrayList
                    );
                }
                else
                {
                    this.lblPaientInfo.Text = "";
                }
            }
        }

        public int ShowScreen()
        {
          //  this.Clear();

            if (Screen.AllScreens.Length > 1)
            {
                this.Show();

                //this.DesktopBounds = Screen.AllScreens[1].Bounds;
                //this.DesktopBounds = Screen.AllScreens[0].Bounds;
                if (Screen.AllScreens[0].Primary)
                {
                    this.DesktopBounds = Screen.AllScreens[1].Bounds;
                }
                else
                {
                    this.DesktopBounds = Screen.AllScreens[0].Bounds;
                }
            }
            return 0;
        }

        #endregion

        #region IMultiScreen 成员

        public int CloseScreen()
        {
            //this.Close();
            this.Hide();
            return 0;
        }

        #endregion
    }
}
