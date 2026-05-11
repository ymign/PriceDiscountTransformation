using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Neusoft.HISFC.BizProcess.Integrate.Pass
{
    /// <summary>
    /// 大通DTPass 的摘要说明。
    /// </summary>
    public class DTPass : Neusoft.HISFC.BizProcess.Interface.Order.IReasonableMedicine
    {
        public DTPass()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }　

        #region 注意

        /*
         * 以下三个符号传入合理用药会导致卡死 <> ◎ &
         * 
         * 注:
         * 1.	<general_name>通用名</general_name>，<medicine_name>商品名</medicine_name>的内容中，发现有些医院带有“<>”符号，例如“<medicine_name><甲>氨茶碱片</medicine_name>”，“<>”符号是XML中的保留字符，因此制作接口的过程中，必须把“<>”替换成“()”。在VB中可以这样替换：
            test = "<甲>氨茶碱片"
            test = Replace(Replace(test, "<", "("), ">", ")")     ‘替换为  (甲)氨茶碱片
         * 2.	<general_name>通用名</general_name>，<medicine_name>商品名</medicine_name>的内容中，发现有些医院带有“◎”符号，例如“<medicine_name>◎二甲双呱片</medicine_name>”，“◎”符号是大通程序中的分隔符，因此制作接口的过程中，必须把“◎”去掉。在VB中可以这样替换：
            test = "◎二甲双呱片"
            test = Replace(test, "◎", "")       ‘替换为二甲双呱片
         * 3.	同第2条中的描述，“&”符号也需要在接口中进行替换。

         * */

        #endregion

        #region 大通接口

        /// <summary>
        /// 传递接口参数，需要HIS程序调用
        /// </summary>
        /// <param name="a">功能参数，控制调用的功能</param>
        /// <param name="b">固定为“0”，作为保留扩展使用，暂时不起作用</param>
        /// <param name="XML">字符串型式，参照的XML格式（节点的方式），具体的内容在后面详细说明</param>
        /// <returns>“0”、“1”、“2”，分别代表“没有问题”、“一般问题”和“严重问题”</returns>
        [System.Runtime.InteropServices.DllImport("dtywzxUI.dll", EntryPoint = "dtywzxUI")]
        public static extern int dtywzxUI(int a, int b, string XML);

        #endregion

        /// <summary>
        /// 保存的xml
        /// </summary>
        private string strXML = "";

        /// <summary>
        /// 取医生以及患者等信息
        /// </summary>
        /// <param name="myOrder"></param>
        /// <param name="dataTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetBaseInfo(Neusoft.HISFC.Models.Order.Order myOrder, string dataTime, string type)
        {
            string strBase = "";
            strBase = "<doctor_information job_number='" + myOrder.Oper.ID + "' date='" + dataTime + "'/>";//工号 处方日期
            strBase = strBase + "<doctor_name>" + myOrder.ReciptDoctor.Name + "</doctor_name>";//医生名 myOrder.Oper.Name
            strBase = strBase + "<doctor_type>" + myOrder.ReciptDoctor.User01 + "</doctor_type>";//医生级别代码 (选填)
            strBase = strBase + "<department_code>" + myOrder.ReciptDept.ID + "</department_code>";//科室代码  myOrder.Oper.Dept.ID
            strBase = strBase + "<department_name>" + myOrder.ReciptDept.Name + "</department_name>";//科室名称  myOrder.Oper.Dept.Name
            if (type == "mz")
            {
                strBase = strBase + "<case_id>" + myOrder.Patient.PID.CardNO + "</case_id>";//病历卡号
            }
            else
            {
                strBase = strBase + "<case_id>" + myOrder.Patient.PID.PatientNO + "</case_id>";//病历卡号
            }
            strBase = strBase + "<inhos_code>" + myOrder.Patient.ID + "</inhos_code>";//门诊就诊号/住院号
            strBase = strBase + "<bed_no>" + ((myOrder.Patient.PVisit.PatientLocation.Bed.ID.Length > 4) ?
                myOrder.Patient.PVisit.PatientLocation.Bed.ID.Substring(4) : myOrder.Patient.PVisit.PatientLocation.Bed.ID) + "</bed_no>";//床号
            //因为合理用药软件会将患者年龄与当前时间进行比对,因此特此减去1天,让合理用药软件不提示患者年龄小于当前时间。
            //strBase = strBase + "<patient_information weight='" + "" + "' height='" + "" + "' birth='" + myOrder.Patient.Birthday.ToString("yyyy-MM-dd") + "'>";//体重 身高 出生年月日(前2选填;

            if (type == "zy")
            {
                strBase = strBase + "<patient_information weight='" + "" + "' height='" + "" + "' birth='" + this.myPatient.Birthday.ToString("yyyy-MM-dd") + "'>";//体重 身高 出生年月日(前2选填;
                strBase = strBase + "<patient_name>" + myPatient.Name + "</patient_name>";//病人名
                strBase = strBase + "<patient_sex>" + myPatient.Sex.Name + "</patient_sex>";//病人性别
            }
            else
            {
                strBase = strBase + "<patient_information weight='" + "" + "' height='" + "" + "' birth='" + this.myPatient.Birthday.ToString("yyyy-MM-dd") + "'>";//体重 身高 出生年月日(前2选填;
                strBase = strBase + "<patient_name>" + this.myPatient.Name + "</patient_name>";//病人名
                strBase = strBase + "<patient_sex>" + this.myPatient.Sex.Name + "</patient_sex>";//病人性别
            }

            strBase = strBase + "<physiological_statms>" + "" + "</physiological_statms>";//生理情况(选填)  
 
            strBase = strBase + "<boacterioscopy_effect>" + "" + "</boacterioscopy_effect>";//菌检结果(选填)
            strBase = strBase + "<bloodpressure>" + "" + "</bloodpressure>";//血压(选填)
            strBase = strBase + "<liver_clean>" + "" + "</liver_clean>";//肌酐清除率 (选填)
            if (type == "zy")
            {
                strBase = strBase + "<pregnant>" + "" + "</pregnant>";//孕妇怀孕时间
                strBase = strBase + "<pdw>" + "" + "</pdw> ";//怀孕时间计量单位
            }

            strBase = strBase + "<allergic_history>";
            strBase = strBase + "<case>";
            //过敏判断
            strBase = strBase + "<case_code>" + "" + "</case_code>";//过敏源代码
            strBase = strBase + "<case_name>" + "" + "</case_name>";//过敏源名称
            strBase = strBase + "</case><case>";
            strBase = strBase + "<case_code>" + "" + "</case_code>";
            strBase = strBase + "<case_name>" + "" + "</case_name>";
            strBase = strBase + "</case><case>";
            strBase = strBase + "<case_code>" + "" + "</case_code>";
            strBase = strBase + "<case_name>" + "" + "</case_name></case></allergic_history>";


            strBase = strBase + "<diagnoses>";
            if (type == "mz")
            {
                //诊断
                //for (int i = 0; i < 3; i++)
                //{
                //    if (myOrder.Patient.Diagnoses != null && myOrder.Patient.Diagnoses.Count > i)
                //    {
                //        strBase = strBase + "<diagnose>" + ((myOrder.Patient.Diagnoses[i] as Neusoft.HISFC.Models.HealthRecord.Diagnose).DiagInfo.IsMain ?
                //            (myOrder.Patient.Diagnoses[i] as Neusoft.HISFC.Models.HealthRecord.Diagnose).DiagInfo.ICD10.Name :
                //            (myOrder.Patient.Diagnoses[i] as Neusoft.HISFC.Models.HealthRecord.Diagnose).DiagInfo.ICD10.ID) + "</diagnose>";
                //    }
                //    else
                //    {
                //        strBase = strBase + "<diagnose>" + "" + "</diagnose>";
                //    }
                //}

                if (diagnoseArrayList.Count > 0)
                {
                    foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose diagnose in diagnoseArrayList)
                    {
                        strBase = strBase + "<diagnose type ='0' name = '" + diagnose.DiagInfo.ICD10.Name + "'>" + diagnose.DiagInfo.ICD10.ID + "</diagnose>";
                    
                    }
                }
                else 
                {
                     strBase = strBase + "<diagnose type = '0'  name = ''>" + "" + "</diagnose>";
                }
                //病生理情况
                strBase = strBase + "<diagnose>" + "" + "</diagnose>";//病生理情况1的中文名称   用于:肝/肾功能不全患者用药审查/妊娠期妇女用药审查/哺乳期妇女用药审查
                strBase = strBase + "<diagnose>" + "" + "</diagnose>";
                strBase = strBase + "<diagnose>" + "" + "</diagnose>";
            }
            else
            {
                //诊断
                //for (int i = 0; i < 3; i++)
                //{
                //    if (myOrder.Patient.Diagnoses != null && myOrder.Patient.Diagnoses.Count > i)
                //    {
                //        strBase = strBase + "<diagnose type ='0' name = '" + (myOrder.Patient.Diagnoses[i] as Neusoft.HISFC.Models.HealthRecord.Diagnose).DiagInfo.ICD10.ID +
                //            "'>" + (myOrder.Patient.Diagnoses[i] as Neusoft.HISFC.Models.HealthRecord.Diagnose).DiagInfo.ICD10.Name + "</diagnose>";
                //    }
                //    else
                //    {
                //        strBase = strBase + "<diagnose type = '0'  name = ''>" + "" + "</diagnose>";
                //    }
                //}

                if (diagnoseArrayList.Count > 0)
                {
                    foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose diagnose in diagnoseArrayList)
                    {
                        strBase = strBase + "<diagnose type ='0' name = '" + diagnose.DiagInfo.ICD10.Name + "'>" + diagnose.DiagInfo.ICD10.ID + "</diagnose>";

                    }
                }
                else
                {
                    strBase = strBase + "<diagnose type = '0'  name = ''>" + "" + "</diagnose>";
                }

                //病生理情况
                strBase = strBase + "<diagnose type ='1' name =''>" + "" + "</diagnose>";//病生理情况1的中文名称   用于:肝/肾功能不全患者用药审查/妊娠期妇女用药审查/哺乳期妇女用药审查
                strBase = strBase + "<diagnose type ='1' name =''>" + "" + "</diagnose>";
                strBase = strBase + "<diagnose type ='1' name =''>" + "" + "</diagnose>";
            }
            strBase = strBase + "</diagnoses></patient_information>";

            return strBase;
        }

        /// <summary>
        /// 根据处方（医嘱）获取住院XML
        /// </summary>
        /// <param name="alOrder"></param>
        /// <param name="cfInfo"></param>
        /// <param name="baseInfo"></param>
        /// <param name="strXml"></param>
        /// <returns></returns>
        private int GetXML(ArrayList alOrder, Neusoft.HISFC.Models.Base.ServiceTypes type,
            string orderInfo, string baseInfo, string dataTime, ref string strXml)
        {
            try
            {
                if (type == Neusoft.HISFC.Models.Base.ServiceTypes.C)
                {
                    foreach (Neusoft.HISFC.Models.Order.OutPatient.Order myOrder in alOrder)
                    {
                        if (myOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            orderInfo = orderInfo + GetOutOrderInfo(myOrder);
                        }
                    }
                    if (orderInfo == "")
                    {
                        return 0;
                    }
                    baseInfo = GetBaseInfo((alOrder[0] as Neusoft.HISFC.Models.Order.Order), dataTime, "mz");
                    orderInfo = "<prescriptions>" + orderInfo + "</prescriptions>";
                    strXml = "<safe>" + baseInfo + orderInfo + "</safe>";
                }
                else
                {
                    foreach (Neusoft.HISFC.Models.Order.Inpatient.Order myOrder in alOrder)
                    {
                        if (myOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            orderInfo = orderInfo + GetInpatientOrderInfo(myOrder);
                        }
                    }
                    if (orderInfo == "")
                    {
                        return 0;
                    }
                    baseInfo = GetBaseInfo((alOrder[0] as Neusoft.HISFC.Models.Order.Order), dataTime, "zy");
                    orderInfo = "<prescriptions>" + orderInfo + "</prescriptions>";
                    strXml = "<safe>" + baseInfo + orderInfo + "</safe>";
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return -1;
            }
            return 1;
        }

        #region 门诊

        /// <summary>
        /// 取门诊医嘱信息
        /// </summary>
        /// <param name="myOrder"></param>
        /// <returns></returns>
        private string GetOutOrderInfo(Neusoft.HISFC.Models.Order.OutPatient.Order myOrder)
        {
            string strCfinfo = "";
            strCfinfo = strCfinfo + "<prescription id='" + myOrder.ReciptNO + "' type='" + "mz" + "' current='1'>"; //处方号
            strCfinfo = strCfinfo + "<medicine suspension='false' judge='true'>";
            strCfinfo = strCfinfo + "<group_number>" + myOrder.Combo.ID + "</group_number>";//组号
            strCfinfo = strCfinfo + "<general_name>" + myOrder.Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</general_name>";//通用名
            strCfinfo = strCfinfo + "<license_number>" + Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItemCustomNO(myOrder.Item.ID).TrimStart('0') + "</license_number>";//医院药品代码
            strCfinfo = strCfinfo + "<medicine_name>" + myOrder.Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</medicine_name>";//商品名
            strCfinfo = strCfinfo + "<single_dose coef='1'>" + myOrder.DoseOnce.ToString() + "</single_dose>";//单次量
            strCfinfo = strCfinfo + "<times>" + myOrder.Frequency.ID + "</times>";//频次代码
            strCfinfo = strCfinfo + "<days>" + "" + "</days>";//天数
            strCfinfo = strCfinfo + "<unit>" + ((Neusoft.HISFC.Models.Pharmacy.Item)myOrder.Item).DoseUnit + "</unit>";//单位（mg,g等）
            strCfinfo = strCfinfo + "<administer_drugs>" + myOrder.Usage.ID + "</administer_drugs></medicine></prescription>";//用药途径
            return strCfinfo;
        }

        /// <summary>
        /// 门诊功能调用
        /// </summary>
        /// <param name="alOrder"></param>
        /// <param name="type">合理用药类型枚举</param>
        /// <param name="str">(传医生工号 -- 传工号)(作废分析/存xml --传时间,)</param>
        /// <returns></returns>
        public int RationalOutPatientDrug(ArrayList alOrder, RationalType type, string str)
        {
            try
            {
                string strXml = "";
                string cfInfo = "";
                string baseInfo = "";
                int returnValue;
                switch (type)
                {
                    case RationalType.Analysis:
                        #region 分析
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            returnValue = GetXML(alOrder, this.stationType, cfInfo, baseInfo, str, ref strXml);
                            if (returnValue == 1)
                            {
                                returnValue = dtywzxUI((int)type, 0, strXml);
                            }
                            return returnValue;
                        }
                        else
                        {
                            return 0;
                        }

                        #endregion
                        break;

                    case RationalType.Close:

                        #region 刷新
                        dtywzxUI(3, 0, "");
                        #endregion

                        #region 退出
                        dtywzxUI((int)type, 0, "");
                        #endregion

                        break;
                    case RationalType.DelOrder:

                        #region 作废分析保存
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            strXml = "<prescription id='" + (alOrder[0] as Neusoft.HISFC.Models.Order.Order).ReciptNO + "'date='" + str + "'/>";
                            dtywzxUI((int)type, 0, strXml);
                        }
                        else
                        {
                            return 0;
                        }
                        #endregion
                        break;

                    case RationalType.Init:

                        #region 初始化
                        dtywzxUI((int)type, 0, "");
                        #endregion
                        break;

                    case RationalType.ReShowTips:

                        #region 重提示
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            foreach (Neusoft.HISFC.Models.Order.OutPatient.Order myOrder in alOrder)
                            {
                                if (myOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                                {
                                    strXml = "<safe><general_name>" + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</general_name><license_number>"
                        + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.ID + "</license_number></safe>";
                                    dtywzxUI((int)type, 0, strXml);
                                }
                            }
                        }
                        else
                        {
                            return 0;
                        }
                        #endregion
                        break;

                    case RationalType.Refresh:

                        #region 刷新
                        dtywzxUI((int)type, 0, "");
                        #endregion
                        break;

                    case RationalType.SaveAnalysis:

                        #region 分析保存
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            returnValue = GetXML(alOrder, this.stationType, cfInfo, baseInfo, str, ref strXml);
                            if (returnValue == 1)
                            {
                                if (dtywzxUI((int)type, 0, strXml) != 0)
                                {
                                    return -1;
                                }
                                strXML = strXml;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            return 0;
                        }

                        #endregion
                        break;

                    case RationalType.SaveXML:

                        #region 存XML
                        if (string.IsNullOrEmpty(strXML))
                        {
                            if (alOrder.Count > 0 && alOrder != null)
                            {
                                if (GetXML(alOrder, stationType, cfInfo, baseInfo, str, ref strXML) == -1)
                                {
                                    return -1;
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        dtywzxUI((int)type, 0, strXML);
                        #endregion
                        break;

                    case RationalType.SendDoctID:

                        #region 传医生工号
                        dtywzxUI((int)type, 0, str);
                        #endregion
                        break;

                    case RationalType.ShowTips:

                        #region 提示
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            foreach (Neusoft.HISFC.Models.Order.OutPatient.Order myOrder in alOrder)
                            {
                                if (myOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                                {
                                    strXml = "<safe><general_name>" + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</general_name><license_number>"
                        + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.ID + "</license_number></safe>";
                                    dtywzxUI((int)type, 0, strXml);
                                }
                            }
                        }
                        else
                        {
                            return 0;
                        }
                        #endregion
                        break;

                    default:
                        return 0;
                        break;
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return -1;
            }
            return 1;
        }

        #endregion

        #region 住院

        /// <summary>
        /// 取住院医嘱信息
        /// </summary>
        /// <param name="myOrder"></param>
        /// <returns></returns>
        private string GetInpatientOrderInfo(Neusoft.HISFC.Models.Order.Inpatient.Order myOrder)
        {
            string strCfinfo = "";
            //strCfinfo = strCfinfo + "<prescription  id='" + myOrder.ID + "' type='" + ((myOrder.OrderType.Type == Neusoft.HISFC.Models.Order.EnumType.LONG) ? "L" : "T") + "'> ";//医嘱类型（L——长期，T——临时）
            strCfinfo = strCfinfo + "<prescription  id='" + myOrder.ID + "' type='" + ((myOrder.OrderType.Type == Neusoft.HISFC.Models.Order.EnumType.LONG) ? "L" : "L") + "'> ";//医嘱类型（L——长期，T——临时）
            strCfinfo = strCfinfo + "<medicine suspension='false'    judge='true'>";
            strCfinfo = strCfinfo + "<group_number>" + myOrder.Combo.ID + "</group_number>";//组号
            strCfinfo = strCfinfo + "<general_name>" + myOrder.Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</general_name>";//通用名
            strCfinfo = strCfinfo + "<license_number>" + Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItemCustomNO(myOrder.Item.ID).TrimStart('0')  + "</license_number>";//医院药品代码
            strCfinfo = strCfinfo + "<medicine_name>" + myOrder.Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</medicine_name>";//商品名
            strCfinfo = strCfinfo + "<single_dose coef='1'>" + myOrder.DoseOnce.ToString() + "</single_dose>";//单次量
            strCfinfo = strCfinfo + "<frequency>" + myOrder.Frequency.ID + "</frequency>";//频次代码
            strCfinfo = strCfinfo + "<times>" + myOrder.InjectCount.ToString() + "</times> ";//次数    --?院注次数
            strCfinfo = strCfinfo + "<unit>" + ((Neusoft.HISFC.Models.Pharmacy.Item)myOrder.Item).DoseUnit + "</unit>";//单位（mg,g等）
            strCfinfo = strCfinfo + "<administer_drugs>" + myOrder.Usage.ID + "</administer_drugs>";//用药途径
            strCfinfo = strCfinfo + "<begin_time>" + myOrder.BeginTime.ToString("yyyy-MM-dd hh:mm:ss") + "</begin_time> ";//用药开始时间(YYYY-MM-DD HH:mm:SS)
            if (myOrder.EndTime == DateTime.MinValue)
            {
                strCfinfo = strCfinfo + "<end_time>" + "</end_time>";
            }
            else
            {
                strCfinfo = strCfinfo + "<end_time>" + myOrder.EndTime.ToString("yyyy-MM-dd hh:mm:ss") + "</end_time>";//用药结束时间(YYYY-MM-DD HH:mm:SS)
            }
            strCfinfo = strCfinfo + "<prescription_time>" + myOrder.MOTime.ToString("yyyy-MM-dd hh:mm:ss") + "</prescription_time> ";//医嘱时间(YYYY-MM-DD HH:mm:SS)
            strCfinfo = strCfinfo + "</medicine>";
            strCfinfo = strCfinfo + "</prescription>";
            return strCfinfo;
        }

        /// <summary>
        /// 住院功能调用
        /// </summary>
        /// <param name="alOrder"></param>
        /// <param name="type"></param>
        /// <param name="str">传时间或医生工号</param>
        /// <returns></returns>
        public int RationalInpatientDrug(ArrayList alOrder, RationalType type, string str)
        {
            try
            {
                string strXml = "";
                string cfInfo = "";
                string baseInfo = "";
                int returnValue;
                switch (type)
                {
                    //{09B34A6E-F6E7-4416-9BE3-FA7A9CBB48E3} 合理用药住院部分接口修改
                    case RationalType.InpatientAnalysis:
                        #region 分析  医嘱配伍分析(住院)
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            returnValue = GetXML(alOrder, stationType, cfInfo, baseInfo, str, ref strXml);
                            if (returnValue == 1)
                            {
                                returnValue = dtywzxUI((int)type, 1, strXml);
                            }
                            return returnValue;
                        }
                        else
                        {
                            return 0;
                        }

                        #endregion
                        break;
                    case RationalType.Close:
                        #region 退出
                        dtywzxUI((int)type, 0, "");
                        #endregion
                        break;
                    case RationalType.DelOrder:
                        #region 作废分析保存
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            strXml = "<prescription id='" + (alOrder[0] as Neusoft.HISFC.Models.Order.Order).ReciptNO + "'date='" + str + "'/>";
                            dtywzxUI((int)type, 0, strXml);
                        }
                        else
                        {
                            return 0;
                        }
                        #endregion
                        break;
                    case RationalType.Init:
                        #region 启动
                        dtywzxUI((int)type, 0, "");
                        #endregion
                        break;
                    case RationalType.ReShowTips:
                        #region 重提示  重新显示要点提示(住院)
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            foreach (Neusoft.HISFC.Models.Order.Order myOrder in alOrder)
                            {
                                if (myOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                                {
                                    strXml = "<safe><general_name>" + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</general_name><license_number>"
                        + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.ID + "</license_number></safe>";
                                    dtywzxUI((int)type, 0, strXml);
                                }
                            }
                        }
                        else
                        {
                            return 0;
                        }
                        #endregion
                        break;
                    case RationalType.Refresh:
                        #region 刷新  新医嘱开始(住院)
                        dtywzxUI((int)type, 0, "");
                        #endregion
                        break;
                    case RationalType.InpatientSaveAnalysis:
                        #region 分析保存  医嘱配伍分析，保存分析结果(住院)
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            returnValue = GetXML(alOrder, stationType, cfInfo, baseInfo, str, ref strXml);
                            if (returnValue == 1)
                            {
                                if (dtywzxUI((int)type, 1, strXml) != 0)
                                {
                                    return -1;
                                }
                                strXML = strXml;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            return 0;
                        }

                        #endregion
                        break;
                    case RationalType.SaveXML:
                        #region 存XML
                        if (string.IsNullOrEmpty(strXML))
                        {
                            if (alOrder.Count > 0 && alOrder != null)
                            {
                                if (GetXML(alOrder, stationType, cfInfo, baseInfo, str, ref strXML) == -1)
                                {
                                    return -1;
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        dtywzxUI((int)type, 0, strXML);

                        #endregion
                        break;
                    case RationalType.SendDoctID:
                        #region 传医生工号
                        dtywzxUI((int)type, 0, str);
                        #endregion
                        break;
                    case RationalType.ShowTips:
                        #region 提示
                        if (alOrder.Count > 0 && alOrder != null)
                        {
                            foreach (Neusoft.HISFC.Models.Order.Order myOrder in alOrder)
                            {
                                if (myOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                                {
                                    strXml = "<safe><general_name>" + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.Name.Replace('<', '(').Replace('>', ')').Replace("◎", "").Replace("&", "") + "</general_name><license_number>"
                        + ((Neusoft.HISFC.Models.Order.Order)myOrder).Item.ID + "</license_number></safe>";
                                    dtywzxUI((int)type, 0, strXml);
                                }
                            }
                        }
                        else
                        {
                            return 0;
                        }
                        #endregion
                        break;
                    default:
                        return 0;
                        break;
                }
            }
            catch
            { }
            return 1;
        }

        #endregion

        #region IReasonableMedicine 成员

        /// <summary>
        /// 合理用药功能是否可用
        /// </summary>
        bool passEnabled = false;

        /// <summary>
        /// 合理用药功能是否可用
        /// </summary>
        public bool PassEnabled
        {
            get
            {
                return passEnabled;
            }
            set
            {
                passEnabled = value;
            }
        }

        /// <summary>
        /// 工作站类别 C 门诊 I 住院
        /// </summary>
        Neusoft.HISFC.Models.Base.ServiceTypes stationType = Neusoft.HISFC.Models.Base.ServiceTypes.C;

        /// <summary>
        /// 工作站类别 C 门诊 I 住院
        /// </summary>
        public Neusoft.HISFC.Models.Base.ServiceTypes StationType
        {
            get
            {
                return stationType;
            }
            set
            {
                stationType = value;
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        string err = "";

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Err
        {
            get
            {
                return err;
            }
            set
            {
                err = value;
            }
        }

        /// <summary>
        /// 药品统一审查
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="alOrder"></param>
        /// <param name="isSave"></param>
        /// <returns>0 人为选择不通过；1 审查通过； -1 审查失败</returns>
        public int PassDrugCheck(ArrayList alOrder, bool isSave)
        {
            Neusoft.HISFC.BizLogic.Order.OutPatient.Order outOrderMgr = new Neusoft.HISFC.BizLogic.Order.OutPatient.Order();

            //“0”、“1”、“2”，分别代表“没有问题”、“一般问题”和“严重问题”
            int returnValue = 0;

            if (this.stationType == Neusoft.HISFC.Models.Base.ServiceTypes.C)
            {
                returnValue = RationalOutPatientDrug(alOrder, RationalType.Analysis, outOrderMgr.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                returnValue = RationalInpatientDrug(alOrder, RationalType.InpatientAnalysis, outOrderMgr.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            if (returnValue == -1)
            {
                return -1;
            }

            if (isSave)
            {
                bool isSaveInfo = true;
                if (returnValue == 2)
                {
                    //MessageBox.Show("合理用药检测出可能存在问题,请根据提示认真考虑！\r\n", "警告", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //return 0;

                    //DialogResult dr = MessageBox.Show("合理用药检测出可能存在严重问题,是否继续?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    //if (dr != DialogResult.Yes)
                    //{
                    //    return 0;
                    //}


                    if (MessageBox.Show("合理用药检测出可能存在问题,请根据提示认真考虑！\r\n是否继续保存？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        isSaveInfo = false;
                        return 0;
                    }
                }
                if (isSaveInfo)
                {
                    #region 分析保存
                    //门诊
                    if (this.stationType == Neusoft.HISFC.Models.Base.ServiceTypes.C)
                    {
                        if (RationalOutPatientDrug(alOrder, RationalType.SaveAnalysis,
                            outOrderMgr.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss")) == -1)
                        {
                            err = "合理用药分析保存出错!\r\n" + err;
                            return -1;
                        }
                    }
                    else
                    {
                        if (RationalInpatientDrug(alOrder, RationalType.InpatientSaveAnalysis,
                          outOrderMgr.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss")) == -1)
                        {
                            err = "合理用药分析保存出错!\r\n" + err;
                            return -1;
                        }
                    }
                    #endregion

                    #region 保存XML参数
                    if (this.stationType == Neusoft.HISFC.Models.Base.ServiceTypes.C)
                    {
                        if (RationalOutPatientDrug(alOrder, RationalType.SaveXML, outOrderMgr.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss")) == -1)
                        {
                            err = "合理用药保存XML出错!\r\n" + err;
                            return -1;
                        }
                    }
                    else
                    {

                        if (RationalInpatientDrug(alOrder, RationalType.SaveXML, outOrderMgr.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss")) == -1)
                        {
                            err = "合理用药保存XML出错!\r\n" + err;
                            return -1;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                if (returnValue == 2)
                {
                    MessageBox.Show("合理用药检测出可能存在问题,请根据提示认真考虑！\r\n", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return 1;
        }

        /// <summary>
        /// 合理用药系统初始化
        /// </summary>
        /// <param name="logEmpl"></param>
        /// <param name="logDept"></param>
        /// <param name="workStationType"></param>
        /// <returns></returns>
        public int PassInit(Neusoft.FrameWork.Models.NeuObject logEmpl, Neusoft.FrameWork.Models.NeuObject logDept, string workStationType)
        {
            try
            {
                this.passEnabled = true;
                return RationalOutPatientDrug(null, RationalType.Init, "");
            }
            catch (Exception ex)
            {
                err = "合理用药系统初始化失败!\r\n" + ex.Message;
                this.passEnabled = false;
                return -1;
            }
        }

        Neusoft.HISFC.Models.RADT.Patient myPatient = new Neusoft.HISFC.Models.RADT.Patient();

        /// <summary>
        /// 设置传入患者基本信息
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public int PassSetPatientInfo(Neusoft.HISFC.Models.RADT.Patient patient, Neusoft.FrameWork.Models.NeuObject recipeDoct)
        {
            this.myPatient = patient;
            return 1;
        }

        ArrayList diagnoseArrayList;

        /// <summary>
        /// 设置诊断
        /// </summary>
        /// <param name="diagnoseList"></param>
        /// <returns></returns>
        public int PassSetDiagnoses(ArrayList diagnoseList)
        {
            diagnoseArrayList = diagnoseList;
            return 1;
        }


        /// <summary>
        /// 合理用药功能初始化刷新
        /// </summary>
        /// <returns></returns>
        public int PassRefresh()
        {
            if (this.passEnabled)
            {
                return RationalOutPatientDrug(null, RationalType.Refresh, "");
            }
            return 1;
        }

        /// <summary>
        /// 合理用药功能关闭
        /// </summary>
        /// <returns></returns>
        public int PassClose()
        {
            if (this.passEnabled)
            {
                return RationalOutPatientDrug(null, RationalType.Close, "");
            }
            return 1;
        }

        /// <summary>
        /// 显示单个药品要点提示
        /// </summary>
        /// <param name="order"></param>
        /// <param name="LeftTop"></param>
        /// <param name="RightButton"></param>
        /// <param name="isFirst">是否首次提示</param>
        /// <returns></returns>
        public int PassShowSingleDrugInfo(Neusoft.HISFC.Models.Order.Order order, System.Drawing.Point LeftTop, System.Drawing.Point RightButton, bool isFirst)
        {
            ArrayList orderList = new ArrayList();
            orderList.Add(order);

            return RationalInpatientDrug(orderList, RationalType.ShowTips, "");
        }

        /// <summary>
        /// 显示要点提示
        /// </summary>
        /// <param name="isShow"></param>
        /// <returns></returns>
        public int PassShowFloatWindow(bool isShow)
        {
            return 1;
        }

        /// <summary>
        /// 显示其他信息（目前仅用于查询菜单）
        /// </summary>
        /// <param name="order"></param>
        /// <param name="queryType"></param>
        /// <param name="alShowMenu"></param>
        /// <returns></returns>
        public int PassShowOtherInfo(Neusoft.HISFC.Models.Order.Order order, Neusoft.FrameWork.Models.NeuObject queryType, ref ArrayList alShowMenu)
        {
            return 1;
        }

        /// <summary>
        /// 获取药品警告信息
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public int PassShowWarnDrug(Neusoft.HISFC.Models.Order.Order order)
        {
            return 1;
        }

        #endregion
    }

    /// <summary>
    /// 合理用药类型枚举
    /// </summary>
    public enum RationalType
    {

        /// <summary>
        /// 初始化
        /// </summary>
        Init = 0,

        /// <summary>
        /// 退出
        /// </summary>
        Close = 1,

        /// <summary>
        /// 刷新
        /// </summary>
        Refresh = 3,

        /// <summary>
        /// 分析
        /// </summary>
        Analysis = 4,

        /// <summary>
        /// 提示
        /// </summary>
        ShowTips = 12,

        /// <summary>
        /// 分析保存
        /// </summary>
        SaveAnalysis = 13,

        /// <summary>
        /// 作废分析保存
        /// </summary>
        DelOrder = 512,

        /// <summary>
        /// 传递医生工号
        /// </summary>
        SendDoctID = 768,

        /// <summary>
        /// 重提示
        /// </summary>
        ReShowTips = 4108,

        /// <summary>
        /// 存XML
        /// </summary>
        SaveXML = 4109,

        /// <summary>
        /// 住院配伍分析
        /// </summary>
        InpatientAnalysis = 28676,

        /// <summary>
        /// 住院配伍分析保存
        /// </summary>
        InpatientSaveAnalysis = 28685,
    }
}
