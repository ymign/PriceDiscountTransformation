using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.FrameWork.Management;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucCostDisplay : UserControl, Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IOutpatientOtherInfomationRight
    {
        public ucCostDisplay()
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

        private Neusoft.FrameWork.Public.ObjectHelper diagnoseTypeHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        private Neusoft.HISFC.BizLogic.Fee.UndrugPackAge managerUndrugPackAge = new Neusoft.HISFC.BizLogic.Fee.UndrugPackAge();

        /// <summary>
        /// 控制参数业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

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
        ///
        /// <summary>
        /// 是否显示患者诊断
        /// </summary>
        protected bool isSetDiag = false;

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
        /// flag  为是否每条项目都显示一次
        public void SetInfomation(Neusoft.HISFC.Models.Registration.Register patient, Neusoft.HISFC.Models.Base.FT ft, ArrayList feeItemLists, ArrayList diagLists,
            params string[] otherInfomations)
        {
            if (iMultiScreen != null)
            {
                this.iMultiScreen.ListInfo = null;
            }

            System.Collections.Generic.List<Object> lo = new System.Collections.Generic.List<object>();
            lo.Add(patient);
            lo.Add(ft);
            lo.Add(feeItemLists);
            lo.Add(diagLists);
            lo.Add(otherInfomations);
            //不实时更新。 xingz 以后好好整合。
            // otherInfomations[0] = "1" 看诊医生科室变更
            // otherInfomations[0] = "2" 合同单位变更
            // otherInfomations[0] = "3" 收费前显示
            // otherInfomations[0] = "4" 收费后清空
            string strTemp = otherInfomations[0];
            if (this.iMultiScreen !=null && (strTemp == "1" || strTemp == "2" || strTemp == "3" || strTemp == "4"))
            {
                this.iMultiScreen.ListInfo = lo; 
            }
         
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
                return;
            }
            
            this.medcareInterfaceProxy.SetPactCode(patient.Pact.ID);
            // {293FDD11-FC10-4ceb-8E4C-1A4304F22592}
            this.medcareInterfaceProxy.IsLocalProcess = false;

            if (feeItemLists == null)// || feeItemLists.Count <= 0)
            {
                if (ft != null)
                {
                    this.tbDrugSendInfo.Text = ft.User01;
                    this.tbRealOwnCost.Text = ft.RealCost.ToString();
                    this.tbReturnCost.Text = ft.ReturnCost.ToString();
                }

                return;
            }

            //if (feeItemLists == null || feeItemLists.Count <= 0)
            //{
            //    return;
            //}


            //if (patient.Pact.PayKind.ID == "02" && isPreeFee == false)
            //{
            //    //医保划价按自费来
            //}
            //else
            //{

             //{CC93C88A-9DD0-49fe-9DC0-B6DA445A7F30}根据参数判断、不区分是否是医保患者了，要不该参数控制不了自费患者。--修改新乡分币时发现的问题
            if (isPreeFee)
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
                    this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumOwnCost, 2).ToString();
                    this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString();
                    this.tbPayCost.Text = sumPayCost.ToString();
                    //{C623A693-19A7-4378-859D-5C07CFF9BEB1}
                    //this.tbPubCost.Text = "0.00";
                    this.tbPubCost.Text = sumPubCost.ToString();
                    this.tbDrugCost.Text = sumDrugCost.ToString();
                }
                else if (patient.Pact.PayKind.ID == "03")
                {
                    this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString();
                    this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumOwnCost, 2).ToString();
                    this.tbPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumPayCost, 2).ToString();
                    this.tbPubCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumPubCost, 2).ToString();
                    this.tbDrugCost.Text = sumDrugCost.ToString();
                }
                else
                {
                    if (patient.Pact.PayKind.ID == "02" && isPreeFee == false)
                    {
                        if (patient.SIMainInfo.PubCost != 0)
                        {
                            this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString();
                            this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.OwnCost, 2).ToString();
                            this.tbPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.PayCost, 2).ToString();
                            this.tbPubCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.PubCost, 2).ToString();
                            this.tbDrugCost.Text = sumDrugCost.ToString();
                        }
                        else
                        {
                            //医保划价按自费来
                            this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumOwnCost, 2).ToString();
                            this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString();
                            this.tbPayCost.Text = sumPayCost.ToString();
                            this.tbPubCost.Text = "0.00";
                            this.tbDrugCost.Text = sumDrugCost.ToString();
                        }
                    }
                    else
                    {
                        this.tbTotCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString();
                        //this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(sumTotCost, 2).ToString();
                        //this.tbPayCost.Text = "0.00";
                        //this.tbPubCost.Text = "0.00";
                        this.tbOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.OwnCost, 2).ToString();
                        this.tbPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.PayCost, 2).ToString();
                        this.tbPubCost.Text = Neusoft.FrameWork.Public.String.FormatNumber(patient.SIMainInfo.PubCost, 2).ToString();
                        this.tbDrugCost.Text = sumDrugCost.ToString();
                    }
                }
            }
            this.isSetDiag = this.controlParamIntegrate.GetControlParam<bool>("MZ9204", true, false);
            if (this.isSetDiag)
            {
                this.neuSpread1.Visible = true;
                this.SetDiay(patient.ID);
            }
            if (ft != null)
            {
                this.tbDrugSendInfo.Text = ft.User01;
                this.tbRealOwnCost.Text = ft.RealCost.ToString();
                this.tbReturnCost.Text = ft.ReturnCost.ToString();
            }
        }

        /// <summary>
        /// 设置单条项目信息
        /// </summary>
        /// <param name="f">项目信息</param>
        public void SetSingleFeeItemInfomation(Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f) 
        {
            string siType = string.Empty;
            decimal siRate = 0;

            if (f.Compare == null)
            {
                siType = "自费";
                siRate = 100;
            }
            else
            {
                if (f.Compare.CenterItem.ItemGrade == "1")
                {
                    siType = "甲类";
                    siRate = 0;
                }
                if (f.Compare.CenterItem.ItemGrade == "2")
                {
                    siType = "乙类";
                    siRate = f.Compare.CenterItem.Rate * 100;
                }
                if (f.Compare.CenterItem.ItemGrade == "3")
                {
                    siType = "自费";
                    siRate = 100;
                }
                if (f.Compare.CenterItem.ID.Length <= 0)
                {
                    siType = "自费";
                    siRate = 100;
                }
            }
            //if (f.Item.IsPharmacy)
            if(f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
            {
                string itemCode = f.Item.ID;
                DataRow findRow;

                DataRow[] rowFinds = this.dsItem.Tables[0].Select("ITEM_CODE = " + "'" + itemCode + "'");

                if (rowFinds == null || rowFinds.Length == 0)
                {
                    MessageBox.Show(Language.Msg("编码为: [") + itemCode + Language.Msg(" ] 的项目查找失败!"));

                    return;
                }
                findRow = rowFinds[0];

                this.rtbItemInfo.Text = "医保类别: " + siType + " 比例:" + siRate.ToString() + "%" + "\n"
                    + "通用名:" + findRow["cus_name"].ToString() + " 英文名:" + findRow["en_name"].ToString().ToLower() + "\n" +
                    "别名:" + findRow["OTHER_NAME"].ToString() + "\n" +
                    "规格:" + f.Item.Specs;
            }
            else
            {
                if (!f.IsGroup)
                {
                    this.rtbItemInfo.Text = "医保类别: " + siType + " 比例:" + siRate.ToString() + "%";
                }
                else
                {
                    this.rtbItemInfo.Text = "";

                    ArrayList alDetails = this.managerUndrugPackAge.QueryUndrugPackagesBypackageCode(f.Item.ID);

                    foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb info in alDetails)
                    {
                        this.rtbItemInfo.Text += info.Name + "\n";
                    }
                }
            }
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
            //获取诊断类别
            ArrayList diagnoseType = Neusoft.HISFC.Models.HealthRecord.DiagnoseType.SpellList();
            diagnoseTypeHelper.ArrayObject = diagnoseType;

            return 1;
        }
        /// <summary>
        ///获得患者闷着信息
        /// </summary>
        /// <param name="patientId">患者门诊号</param>
        public void SetDiay(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return;
            }
            ArrayList alDiay = null;
            Neusoft.HISFC.BizLogic.HealthRecord.Diagnose diagManager = new Neusoft.HISFC.BizLogic.HealthRecord.Diagnose();
            try
            {
                alDiay = diagManager.QueryCaseDiagnoseForClinicByState(patientId,
                    Neusoft.HISFC.Models.HealthRecord.EnumServer.frmTypes.DOC,
                    true);//默认不显示作废诊断
            }
            catch (Exception ex)
            {
                MessageBox.Show("获得患者的诊断信息出错！" + ex.Message, "提示");
                return;
            }
            if (alDiay == null)
            {
                return;
            }
            if (this.diagnoseTypeHelper.ArrayObject.Count <= 0)
            {
                this.diagnoseTypeHelper.ArrayObject = Neusoft.HISFC.Models.HealthRecord.DiagnoseType.SpellList();
            }
            //填充
            this.neuSpread1_Sheet1.RowCount = 0;
            foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose diag in alDiay)
            {
                this.neuSpread1_Sheet1.Rows.Add(0, 1);
                this.neuSpread1_Sheet1.Cells[0, 0].Text = this.diagnoseTypeHelper.GetObjectFromID(diag.DiagInfo.DiagType.ID).Name;
                this.neuSpread1_Sheet1.Cells[0, 1].Value = diag.DiagInfo.IsMain;//是否描述
                this.neuSpread1_Sheet1.Cells[0, 2].Text = diag.DiagInfo.ICD10.ID;//icd码
                this.neuSpread1_Sheet1.Cells[0, 3].Text = diag.DiagInfo.ICD10.Name;//icd名称
                this.neuSpread1_Sheet1.Cells[0, 4].Value = Neusoft.FrameWork.Function.NConvert.ToBoolean(diag.DubDiagFlag);//是否疑诊
                this.neuSpread1_Sheet1.Cells[0, 5].Text = diag.DiagInfo.DiagDate.Date.ToShortDateString();//日期
                //this.neuSpread1_Sheet1.Cells[0, 7].CellType = new FarPoint.Win.Spread.CellType.TextCellType();
                //this.neuSpread1_Sheet1.Cells[0, 7].Text = diag.DiagInfo.Doctor.ID;//代码
                this.neuSpread1_Sheet1.Cells[0, 6].Text = diag.DiagInfo.Doctor.Name;//诊断医生
            }
        }
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() 
        {
            this.tbDrugCost.Text = "0.00";
            this.rtbItemInfo.Text = string.Empty;
            this.neuSpread1_Sheet1.RowCount = 0;
            this.neuSpread1_Sheet1.RowCount = 2;
            if (iMultiScreen != null)
            {
                this.iMultiScreen.ListInfo = null;
            }

            // 不需要清空
            this.tbOwnCost.Text = "0.00";
            this.tbPayCost.Text = "0.00";
            this.tbPubCost.Text = "0.00";

            this.tbRealOwnCost.Text = "0.00";
            this.tbReturnCost.Text = "0.00";
            this.tbDrugCost.Text = "0.00";
            this.tbDrugSendInfo.Text = "";
            this.tbTotCost.Text = "0.00";

        }

        #endregion

        #region IOutpatientOtherInfomationRight 成员

        private Neusoft.HISFC.BizProcess.Interface.FeeInterface.IMultiScreen iMultiScreen;
        public Neusoft.HISFC.BizProcess.Interface.FeeInterface.IMultiScreen MultiScreen
        {
            set { iMultiScreen = value; }
        }

        #endregion
    }
}
