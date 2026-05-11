using System;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using Neusoft.HISFC.Models.RADT;
using System.Collections.Generic;
using Neusoft.HISFC.Models.Registration;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Base;
using System.IO;

namespace Neusoft.HISFC.BizProcess.Integrate.Pass
{
    /// <summary>
    /// 美康Pass 的摘要说明。
    /// </summary>
    public class MKPass4 : Neusoft.HISFC.BizProcess.Interface.Order.IPASS
    {
        public MKPass4()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region  美康2018-04-24
        //*******美康嵌入代码开始（DLL函数声明）*****************************
        //1、PASS初始化
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_Init", CharSet = CharSet.Ansi)]
        public static extern int MDC_Init(string pcCheckMode, string pcHisCode, string pcDoctorCode);

        //2、获取PASS系统最后一次错误信息函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_GetLastError", CharSet = CharSet.Ansi)]
        public static extern string MDC_GetLastError();
        //3、审查函数
        //3-1 传入审查对象信息类函数
        //3-1-1 传病人基本记录信息
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_SetPatient", CharSet = CharSet.Ansi)]
        public static extern int MDC_SetPatient(string pcPatCode,
        string pcInHospNo,
        string pcVisitCode,
        string pcName,
        string pcSex,
        string pcBirthday,
        string pcHeightCM,
        string pcWeighKG,
        string pcDeptCode,
        string pcDeptName,
        string pcDoctorCode,
        string pcDoctorName,
        int piPatStatus,
        int piIsLactation,
        int piIsPregnancy,
        string pcPregStartDate,
        int piHepDamageDegree,
        int piRenDamageDegree);
        //3-1-2 传病人药品记录信息
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_AddScreenDrug", CharSet = CharSet.Ansi)]
        public static extern int MDC_AddScreenDrug(string pcIndex,
        int piOrderNo,
        string pcDrugUniqueCode,
        string pcDrugName,
        string pcDosePerTime,
        string pcDoseUnit,
        string pcFrequency,
        string pcRouteCode,
        string pcRouteName,
        string pcStartTime,
        string pcEndTime,
        string pcExecuteTime,
        string pcGroupTag,
        string pcIsTempDrug,
        string pcOrderType,
        string pcDeptCode,
        string pcDeptName,
        string pcDoctorCode,
        string pcDoctorName,
        string pcRecipNo,
        string pcNum,
        string pcNumUnit,
        string pcPurpose,
        string pcOprCode,
        string pcMediTime,
        string pcRemark);
        //3-1-3 传入病人过敏史记录信息
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_AddAller", CharSet = CharSet.Ansi)]
        public static extern int MDC_AddAller(string pcIndex,
        string pcAllerCode,
        string pcAllerName,
        string pcAllerSymptom);
        //3-1-4 传入病人诊断记录信息
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_AddMedCond", CharSet = CharSet.Ansi)]
        public static extern int MDC_AddMedCond(string pcIndex,
        string pcDiseaseCode,
        string pcDiseaseName,
        string pcRecipNo);
        //3-1-5 传入病人手术记录信息
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_AddOperation", CharSet = CharSet.Ansi)]
        public static extern int MDC_AddOperation(string pcIndex,
        string pcOprCode,
        string pcOprName,
        string pcIncisionType,
        string pcOprStartDateTime,
        string pcOprEndDateTime);
        //3-2审查函数
        //
        /// <summary>
        /// 3-2-1合理用药审查函数
        /// 0-正常监测，无监测结果，蓝灯。
        /// 1-正常监测，结果为禁忌或严重，黑灯。
        /// 2-正常监测，结果为不推荐，红灯。
        /// 3-正常监测，结果为慎用，橙灯。
        /// 4-正常监测，结果为关注，黄灯。
        /// </summary>
        /// <param name="piShowMode"></param>
        /// <param name="piIsSave"></param>
        /// <returns></returns>
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_DoCheck", CharSet = CharSet.Ansi)]
        public static extern int MDC_DoCheck(int piShowMode,
        int piIsSave);

        //3-3 获取审查结果函数
        /// <summary>
        /// 3-3-1 获取药品医嘱警示级别
        /// 0-正常监测，无监测结果，蓝灯。
        /// 1-正常监测，结果为禁忌或严重，黑灯。
        /// 2-正常监测，结果为不推荐，红灯。
        /// 3-正常监测，结果为慎用，橙灯。
        /// 4-正常监测，结果为关注，黄灯。
        /// </summary>
        /// <param name="pcIndex"></param>
        /// <returns></returns>
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_GetWarningCode", CharSet = CharSet.Ansi)]
        public static extern int MDC_GetWarningCode(string pcIndex);

        //3-3-2获取一条药品医嘱的审查结果提示窗口函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_ShowWarningHint", CharSet = CharSet.Ansi)]
        public static extern int MDC_ShowWarningHint(string pcIndex);
        //3-3-3关闭一条药品医嘱的审查结果提示窗口函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_CloseWarningHint", CharSet = CharSet.Ansi)]
        public static extern int MDC_CloseWarningHint();
        //3-3-4获取药品审查结果条数函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_GetResultItemCount", CharSet = CharSet.Ansi)]
        public static extern string MDC_GetResultItemCount(string pcIndex);
        //3-3-5 获取审查结果详细信息函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_GetResultDetail", CharSet = CharSet.Ansi)]
        public static extern int MDC_GetResultDetail(string pcIndex);

        //4、信息查询类函数
        //4-1传入查询药品信息函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_DoSetDrug", CharSet = CharSet.Ansi)]
        public static extern int MDC_DoSetDrug(string pcDrugUniqueCode,
        string pcDrugName);
        //4-2获取查询项目的内容是否存在函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_DoRefDrugEnable", CharSet = CharSet.Ansi)]
        public static extern int MDC_DoRefDrugEnable(int piQueryType);
        //4-3执行药品信息查询函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_DoRefDrug", CharSet = CharSet.Ansi)]
        public static extern int MDC_DoRefDrug(int piQueryType);
        //4-4关闭药品重要信息浮动窗口函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_CloseDrugHint", CharSet = CharSet.Ansi)]
        public static extern int MDC_CloseDrugHint();
        //4-5获取查询项目有效性函数[暂不用]
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_GetDrugRefEnabled", CharSet = CharSet.Ansi)]
        public static extern string MDC_GetDrugRefEnabled(string pcDrugUniqueCode,
        int piQueryType);
        //4-6 查询药品信息函数[暂不用]
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_GetDrugQueryInfo", CharSet = CharSet.Ansi)]
        public static extern int MDC_GetDrugQueryInfo(string pcDrugUniqueCode,
        string pcDrugName,
        int piQueryType,
        int x,
        int y);
        //5、调用药研究窗口函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_DoMediStudy", CharSet = CharSet.Ansi)]
        public static extern int MDC_DoMediStudy(string pcUseTime);
        //6、本地参数设置窗口函数
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_Settings", CharSet = CharSet.Ansi)]
        public static extern int iMDC_Settings();
        //7、PASS退出
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_Quit", CharSet = CharSet.Ansi)]
        public static extern int iMDC_Quit();


        /// <summary>
        /// 显示用药指导单
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_DoPASSCommand", CharSet = CharSet.Ansi)]
        public static extern int MDC_DoPASSCommand(string command);


        /// <summary>
        /// 获取药品合理用药审核状态
        /// </summary>
        /// <param name="patcode">病人id</param>
        /// <param name="pcInHospNo">病人号</param>
        /// <param name="pcVisitCode">
        /// </param>
        /// <param name="pcRecipNo">空值</param>
        /// <param name="piTaskType">住院传 1 ，门诊传 2</param>
        /// <returns></returns>
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_GetTaskStatus", CharSet = CharSet.Ansi)]
        public static extern int MDC_GetTaskStatus(string patcode, string pcInHospNo, string pcVisitCode, string pcRecipNo, int piTaskType);

        // {62795612-7CF2-41da-B8DC-8DEC9A2F48E5}
        /// <summary>
        /// 调用病人补充信息
        /// </summary>
        /// <param name="pcJson">病人id</param>
        /// <returns></returns>
        [DllImport("PASS4Invoke.dll", EntryPoint = "MDC_AddJsonInfo", CharSet = CharSet.Ansi)]
        public static extern int MDC_AddJsonInfo(string pcJson);



        //*******PASS4.0**1-美康嵌入代码结束（DLL函数声明）*****************************
        #endregion


        /// <summary>
        /// 错误信息
        /// </summary>
        private string err = "";

        /// <summary>
        /// 错误信息
        /// </summary>
        public string PassErr
        {
            get
            {
                return this.err;
            }
            set
            {
                this.err = value;
            }
        }

        /// <summary>
        /// 工作站类别 C 门诊 I 住院
        /// </summary>
        Neusoft.HISFC.Models.Base.ServiceTypes stationType = Neusoft.HISFC.Models.Base.ServiceTypes.C;

        /// <summary>
        /// 工作站类别 C 门诊 I 住院
        /// </summary>
        public Neusoft.HISFC.Models.Base.ServiceTypes PassStationType
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
        /// PASS4.0初始化函数
        /// </summary>
        /// <param name="pcCheckMode"></param>
        /// <param name="pcHisCode"></param>
        /// <param name="pcDoctorCode"></param>
        /// <returns></returns>
        public int Pass4_Init(int w, string pcCheckMode, string pcHisCode, string pcDoctorCode)
        {
            #region 判断DIFPassDll.dll是否存在
            bool passFileIsExists = File.Exists(@".\PASS4Invoke.dll");
            if (!passFileIsExists)
            {

                MessageBox.Show("美康相关Dll文件：PASS4Invoke.dll未找到,不能进行监测！");
                return -1;
            }
            #endregion

            #region 进行初始化
            int passService;
            if (w == 1)
            {
                passService = MDC_Init("mz", "0", pcDoctorCode);
            }
            else
            {
                //pcHisCode参数默认传0
                passService = MDC_Init("zy", "1", pcDoctorCode);
            }


            if (passService == 1)
            {
                return 1;
            }
            else if (passService == 0)
            {
                MessageBox.Show("美康初始化失败：初始化失败！");
                return -1;
            }
            else if (passService == -1)
            {
                MessageBox.Show("美康初始化失败：执行命令超时！");
                return -1;
            }
            else if (passService == -2)
            {
                MessageBox.Show("美康初始化失败：连接PASS服务器失败！");
                return -1;
            }
            else if (passService == -3)
            {
                MessageBox.Show("美康初始化失败：获取审查、查询列表出错！");
                return -1;
            }
            else if (passService == -4)
            {
                MessageBox.Show("美康初始化失败：初始化工具条出错！");
                return -1;
            }
            else if (passService == -5)
            {
                MessageBox.Show("美康初始化失败：更新资源文件出错！");
                return -1;
            }
            else
            {
                MessageBox.Show("美康初始化失败！");
                return -1;
            }
            #endregion
        }

        /// <summary>
        /// 退出PASS
        /// </summary>
        /// <returns></returns>
        public int PassClose()
        {

            return iMDC_Quit();

        }
        /// <summary>
        /// 合理用药功能初始化刷新
        /// </summary>
        /// <returns></returns>
        public int PassRefresh()
        {
            return 1;
        }

        /// <summary>
        /// 当前审查的患者信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.Patient passPatient = null;

        /// <summary>
        /// 传病人基本记录信息函数
        /// </summary>
        /// <param name="patientObj"></param>
        /// <param name="recipeDoct"></param>
        /// <param name="hh"></param>
        /// <returns></returns>
        public int PassSetPatientInfo(Neusoft.HISFC.Models.RADT.Patient patientObj, Neusoft.FrameWork.Models.NeuObject recipeDoc)
        {
            this.passPatient = patientObj;

            bool isOutPatient = true;
            if (patientObj is Neusoft.HISFC.Models.RADT.PatientInfo)
            {
                isOutPatient = false;
            }

            Neusoft.HISFC.Models.Base.Employee recipeDoctxx = recipeDoc as Neusoft.HISFC.Models.Base.Employee;

            string pcPatCode = patientObj.ID;
            int piPatStatus = 1;
            string pcInHospNo = patientObj.PID.CardNO;
            string pcVisitCode = "1";//入院次数

            if (isOutPatient)
            {
                pcPatCode = patientObj.PID.CardNO;
                pcInHospNo = patientObj.ID;
                pcVisitCode = "1";//入院次数
                if (patientObj.IsTreatment)
                {
                    piPatStatus = 3;
                }
                else
                {
                    piPatStatus = 2;
                }
            }
            else
            {
                pcPatCode = patientObj.PID.PatientNO;
                pcInHospNo = patientObj.ID;
                pcVisitCode = (patientObj as Neusoft.HISFC.Models.RADT.PatientInfo).InTimes.ToString();
            }
            string pcName = patientObj.Name;
            string pcSex = patientObj.Sex.Name;
            string pcBirthday = patientObj.Birthday.ToString("yyyy-MM-dd");
            //string pcHeightCM = "";//patient.Height;
            //string pcWeighKG = "";//patient.Weight;
            // {25D08AFA-177F-4a1a-B6F8-C56FF0A18BCB} 加入身高体重
            string pcHeightCM = patientObj.Height;
            string pcWeighKG = patientObj.Weight;

            string pcDeptCode = recipeDoctxx.Dept.ID;
            string pcDeptName = SOC.HISFC.BizProcess.Cache.Common.GetDeptName(recipeDoctxx.Dept.ID);
            string pcDoctorCode = recipeDoctxx.ID;
            string pcDoctorName = recipeDoctxx.Name;


            int piIsLactation = -1;
            int piIsPregnancy = -1;
            string pcPregStartDate = "";
            int piHepDamageDegree = -1;
            int piRenDamageDegree = -1;


            return MDC_SetPatient(pcPatCode, pcInHospNo, pcVisitCode, pcName, pcSex, pcBirthday, pcHeightCM, pcWeighKG, pcDeptCode, pcDeptName, pcDoctorCode, pcDoctorName,
piPatStatus, piIsLactation, piIsPregnancy, pcPregStartDate, piHepDamageDegree, piRenDamageDegree);

        }


        /// <summary>
        /// 传入病人过敏史信息
        /// </summary>
        /// <param name="diagnoseList"></param>
        /// <returns></returns>
        public int PassAllergyInfo(ArrayList AllergyList)
        {
            for (int i = 0; i < AllergyList.Count; i++)
            {
                string pcIndex = (i + 1).ToString();
                Neusoft.HISFC.Models.Order.Medical.AllergyInfo diag = AllergyList[i] as Neusoft.HISFC.Models.Order.Medical.AllergyInfo;

                string pcAllerCode = diag.Allergen.ID;
                string pcAllerName = diag.Allergen.Name;
                string pcAllerSymptom = "";
                MDC_AddAller(pcIndex, pcAllerCode, pcAllerName, pcAllerSymptom);
            }

            return 1;
        }

        /// <summary>
        /// 传入病人诊断记录信息
        /// </summary>
        /// <param name="diagnoseList"></param>
        /// <returns></returns>
        public int PassSetDiagnoses(ArrayList diagnoseList)
        {
            for (int i = 0; i < diagnoseList.Count; i++)
            {
                string pcIndex = (i + 1).ToString();
                Neusoft.HISFC.Models.HealthRecord.Diagnose diag = diagnoseList[i] as Neusoft.HISFC.Models.HealthRecord.Diagnose;
                if (!string.IsNullOrEmpty(diag.DiagInfo.ICD10.Name))
                {
                    if (diag.DiagInfo.ICD10.ID == "MS999")
                    {
                        string pcDiseaseCode = "";
                        string pcDiseaseName = diag.DiagInfo.ICD10.Name;
                        string pcRecipNo = "";
                        MDC_AddMedCond(pcIndex, pcDiseaseCode, pcDiseaseName, pcRecipNo);
                    }
                    else
                    {
                        string pcDiseaseCode = diag.DiagInfo.ICD10.ID;
                        string pcDiseaseName = diag.DiagInfo.ICD10.Name;
                        string pcRecipNo = "";
                        MDC_AddMedCond(pcIndex, pcDiseaseCode, pcDiseaseName, pcRecipNo);
                    }
                    //string pcDiseaseCode = diag.DiagInfo.ICD10.ID;
                    //string pcDiseaseName = diag.DiagInfo.ICD10.Name;
                    //string pcRecipNo = "";
                    //MDC_AddMedCond(pcIndex, pcDiseaseCode, pcDiseaseName, pcRecipNo);
                }

            }

            return 1;
        }
        /// 设置浮动窗口是否显示
        /// </summary>
        /// <param name="isShow"></param>
        public int PassShowFloatWindow(int rownum)
        {
            string pcIndex = rownum.ToString();
            return MDC_ShowWarningHint(pcIndex);
        }

        /// <summary>
        /// 传病人药品记录信息
        /// </summary>
        /// <param name="alOrder"></param>
        public void PassSetRecipeInfo(int w, ArrayList alOrder)
        {

            if (w == 1)  //门诊
            {
                int piOrderNo = 0;
                foreach (Neusoft.HISFC.Models.Order.OutPatient.Order order in alOrder)
                {
                    //Unit al = this.getDoseUnit(order.ID);

                    piOrderNo++;

                    string pcIndex = order.ID;// .SequenceNO.ToString();// .SeqNo.ToString();

                    string pcDrugUniqueCode = order.Item.ID;
                    string pcDrugName = order.Item.Name;

                    string pcDosePerTime = "";
                    string pcDoseUnit = "";
                    // {62795612-7CF2-41da-B8DC-8DEC9A2F48E5}
                    string jsonStr = "";
                    string pharmacycode = "";
                    string pharmacyname = "";

                    if (order.Item.ItemType == EnumItemType.Drug)
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item phaItem = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(order.Item.ID);
                        if (phaItem != null)
                        {
                            if (phaItem.DoseUnit == order.DoseUnit)
                            {
                                pcDosePerTime = order.DoseOnce.ToString();
                            }
                            else
                            {
                                //by20190923取开立药品单位为片等单位时，转换药品单片剂量
                                //pcDosePerTime = (phaItem.OnceDose * order.DoseOnce).ToString("F4").TrimEnd('0').TrimEnd('.');
                                pcDosePerTime = (phaItem.BaseDose * order.DoseOnce).ToString("F4").TrimEnd('0').TrimEnd('.');
                            }
                            pcDoseUnit = phaItem.DoseUnit;
                        }
                    }

                    string pcFrequency = order.Frequency.ID;
                    string pcRouteCode = order.Usage.Name;//order.Usage.ID;
                    string pcRouteName = order.Usage.Name;
                    string pcStartTime = order.MOTime.ToString();
                    string pcEndTime = "";
                    string pcExecuteTime = "";
                    string pcGroupTag = order.Combo.ID;
                    string pcIsTempDrug = "0";

                    string pcOrderType = "0";
                    //if (order.Status == 0)
                    //{
                    //    pcOrderType = "9";
                    //}
                    string pcDeptCode = order.InDept.ID;
                    string pcDeptName = SOC.HISFC.BizProcess.Cache.Common.GetDeptName(order.InDept.ID);
                    string pcDoctorCode = order.Oper.ID;
                    string pcDoctorName = order.Oper.Name;
                    // {1A2B8C63-F737-44eb-9870-21FE2A294ED5}住院pcRecipNo传空
                    string pcRecipNo = order.ReciptNO;
                    //string pcRecipNo = "";

                    string pcNum = order.Qty.ToString();
                    string pcNumUnit = order.Unit;
                    string pcPurpose = "0";
                    string pcOprCode = "";
                    string pcMediTime = "";
                    string pcRemark = order.Memo;


                    MDC_AddScreenDrug(pcIndex,
                                         piOrderNo,
                                         pcDrugUniqueCode,
                                         pcDrugName,
                                         pcDosePerTime,
                                         pcDoseUnit,
                                         pcFrequency,
                                         pcRouteCode,
                                         pcRouteName,
                                         pcStartTime,
                                         pcEndTime,
                                         pcExecuteTime,
                                         pcGroupTag,
                                         pcIsTempDrug,
                                         pcOrderType,
                                         pcDeptCode,
                                         pcDeptName,
                                         pcDoctorCode,
                                         pcDoctorName,
                                         pcRecipNo,
                                         pcNum,
                                         pcNumUnit,
                                         pcPurpose,
                                         pcOprCode,
                                         pcMediTime,
                                         pcRemark);

                    // {62795612-7CF2-41da-B8DC-8DEC9A2F48E5}
                    pharmacycode = order.StockDept.ID;
                    pharmacyname = order.StockDept.Name;
                    jsonStr = "{\"type\":\"druginfo\",\"index\":\"" + pcIndex + "\",\"driprate\":\"\",\"driprange\":\"\",\"driptime\":\"\",\"duration\":\"\",\"reciptypecode\": \"\",\"reciptypename\": \"\",\"moredaydesc\": \"\",\"doseday\": \"\",\"skintest\":\"\",\"executivedept\": \"\",\"pharmacycode\": \"" + pharmacycode + "\",\"pharmacyname\":\"" + pharmacyname + "\",\"doctorpriv\":\"-1\" }";
                    MDC_AddJsonInfo(jsonStr);
                    #region 55C58DD4-5509-041A-1303-A270769E22E0 2021-11-22 合理用药接口加上外延处方标记 by yhm
                    string recipetypecode = string.Empty;
                    string recipetypename = string.Empty;
                    if (order.IsExtendRecipe)
                    {
                        recipetypecode = "9";
                        recipetypename = "外延处方";
                    }
                    else
                    {
                        recipetypecode = "1";
                        recipetypename = "普通";
                    }

                    string jsonStr2 = "{\"type\":\"recipeinfo\",\"recipeno\":\"" + order.ReciptNO + "\",\"recipetypecode\":\"" + recipetypecode + "\",\"recipetypename\":\"" + recipetypename + "\",\"reciperoutecode\":\"\",\"reciperoutename\":\"\",\"recipefreq\": \"\",\"recipedosage\": \"\",\"reciperemark\": \"\",\"ischronicdisease\": \"\",\"chmndaypereach\":\"\",\"cost\": \"\",\"starttime\":\"" + order.MOTime.ToString("yyyy-mm-dd hh:mm:ss") + "\"}";
                    MDC_AddJsonInfo(jsonStr2); 
                    #endregion
                    
                }

            }
            else
            {
                #region 住院处方

                int piOrderNo = 0;
                Neusoft.HISFC.BizLogic.Admin.FunSetting funMgr = new Neusoft.HISFC.BizLogic.Admin.FunSetting();
                bool isContinue = false;
                //若是新开立的医嘱立没有含有药品的，则不传数据给合理用药接口(判断放这里是怕有其他入口调用该接口)
                foreach (Neusoft.HISFC.Models.Order.Inpatient.Order item in alOrder)
                {
                    if (item.Item.ItemType == EnumItemType.Drug)
                    {
                        if (string.IsNullOrEmpty(item.ID) || item.Status == 0)
                        {
                            isContinue = true;
                            break;
                        }
                    }
                }
                if (!isContinue)
                {
                    return;
                }
                foreach (Neusoft.HISFC.Models.Order.Inpatient.Order order in alOrder)
                {
                    //Unit al = this.getDoseUnit(order.ID);

                    piOrderNo++;

                    //string pcIndex = order.ID;// .SequenceNO.ToString();// .SeqNo.ToString();
                    //pcInde重复问题改为取序号
                    string pcIndex = piOrderNo.ToString();

                    string pcDrugUniqueCode = order.Item.ID;
                    string pcDrugName = order.Item.Name;

                    string pcDosePerTime = "";
                    string pcDoseUnit = "";

                    // {62795612-7CF2-41da-B8DC-8DEC9A2F48E5}
                    string jsonStr = "";
                    string pharmacycode = "";
                    string pharmacyname = "";

                    if (order.Item.ItemType == EnumItemType.Drug)
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item phaItem = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(order.Item.ID);
                        if (phaItem != null)
                        {
                            if (phaItem.DoseUnit == order.DoseUnit)
                            {
                                //pcDosePerTime = order.DoseOnce.ToString();
                                // {4CE543AA-E110-41b3-8F09-E24FD442A63F}    去掉小数点
                                pcDosePerTime = order.DoseOnce.ToString("F4").TrimEnd('0').TrimEnd('.');
                            }
                            else
                            {
                                pcDosePerTime = (phaItem.BaseDose * order.DoseOnce).ToString("F4").TrimEnd('0').TrimEnd('.');
                                //pcDosePerTime = (phaItem.OnceDose * order.DoseOnce).ToString("F4").TrimEnd('0').TrimEnd('.');
                            }
                            pcDoseUnit = phaItem.DoseUnit;
                        }
                    }

                    //if (al.doseUnit == al.onceUnit)
                    //{
                    //    pcDosePerTime = order.DoseOnce.ToString();
                    //    pcDoseUnit = order.DoseUnit;
                    //}
                    //else
                    //{
                    //    pcDosePerTime = (Convert.ToDecimal(al.onceDose) * Convert.ToDecimal(al.baseDose)).ToString();
                    //    pcDoseUnit = al.doseUnit;
                    //}
                    string pcIsTempDrug = "";
                    //if (order.OrderType.ID == "CZ")
                    //{
                    //    pcIsTempDrug = "0";
                    //}
                    //if (order.OrderType.ID == "LZ")
                    //{
                    //    pcIsTempDrug = "1";
                    //}
                    // {3FB58EB6-2932-4eca-B179-4C4A631F3C42} 长临嘱问题
                    if (order.OrderType.ID == "CZ" || order.OrderType.ID == "ZC")
                    {
                        pcIsTempDrug = "0";
                    }
                    else
                    {
                        //有个未知bug 先这么试试 不是当天开立的临嘱那就过滤掉
                        string nowtime = funMgr.GetDateTimeFromSysDateTime().ToString("yyyyMMdd");
                        string modertime = order.MOTime.ToString("yyyyMMdd");
                        if (modertime != nowtime)
                        {
                            continue;
                        }
                        pcIsTempDrug = "1";
                    }


                    string pcFrequency = order.Frequency.ID;
                    string pcRouteCode = order.Usage.Name;//order.Usage.ID;
                    string pcRouteName = order.Usage.Name;
                    string pcStartTime = order.MOTime.ToString();
                    string pcEndTime = "";
                    string pcExecuteTime = "";
                    string pcGroupTag = order.Combo.ID;


                    // {08DA6BA1-1C5B-4c45-BE53-0C0912F66D82}  
                    //当医生点击提交医嘱时，传入的医嘱应该包括：新开医嘱、当天新开的临时医嘱、未停的长期医嘱。
                    //如果是新开医嘱 MDC_AddScreenDrug函数的ordertype参数传9 
                    //如果医嘱是出院带药医嘱，MDC_AddScreenDrug函数的ordertype参数传3。
                    //不是出院带药医嘱情况下：                   
                    //当天新开的临时医嘱和未停的长期医嘱ordertype传 0 。
                    string pcOrderType = "0";
                    if (order.OrderType.ID == "CD")//出院带药 传3优先级最高
                    {
                        pcOrderType = "3";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(order.ID)) //代表为新开立的医嘱  
                        {
                            pcOrderType = "9";
                        }

                    }



                    #region 原有注释
                    //string pcOrderType = "0";
                    //if (order.OrderType.ID == "CD")
                    //{
                    //    pcOrderType = "3";
                    //}
                    //else
                    //{
                    //    if (order.Status == 0)
                    //    {
                    //        pcOrderType = "9";
                    //    }
                    //    else
                    //    {
                    //        pcOrderType = "0";
                    //    }
                    //}

                    #endregion


                    string pcDeptCode = order.InDept.ID;
                    string pcDeptName = SOC.HISFC.BizProcess.Cache.Common.GetDeptName(order.InDept.ID);
                    string pcDoctorCode = order.Oper.ID;
                    string pcDoctorName = order.Oper.Name;

                    // {1A2B8C63-F737-44eb-9870-21FE2A294ED5}住院pcRecipNo传空
                    //string pcRecipNo = order.ID;
                    string pcRecipNo = "";
                    string pcNum = "";/// order.Qty.ToString();
                    string pcNumUnit = "";///order.Unit;
                    string pcPurpose = "0";
                    string pcOprCode = "";
                    string pcMediTime = "";
                    string pcRemark = order.Memo;


                    MDC_AddScreenDrug(pcIndex,
                                         piOrderNo,
                                         pcDrugUniqueCode,
                                         pcDrugName,
                                         pcDosePerTime,
                                         pcDoseUnit,
                                         pcFrequency,
                                         pcRouteCode,
                                         pcRouteName,
                                         pcStartTime,
                                         pcEndTime,
                                         pcExecuteTime,
                                         pcGroupTag,
                                         pcIsTempDrug,
                                         pcOrderType,
                                         pcDeptCode,
                                         pcDeptName,
                                         pcDoctorCode,
                                         pcDoctorName,
                                         pcRecipNo,
                                         pcNum,
                                         pcNumUnit,
                                         pcPurpose,
                                         pcOprCode,
                                         pcMediTime,
                                         pcRemark);
                    // {62795612-7CF2-41da-B8DC-8DEC9A2F48E5}
                    pharmacycode = order.StockDept.ID;
                    pharmacyname = order.StockDept.Name;
                    jsonStr = "{\"type\":\"druginfo\",\"index\":\"" + pcIndex + "\",\"driprate\":\"\",\"driprange\":\"\",\"driptime\":\"\",\"duration\":\"\",\"reciptypecode\": \"\",\"reciptypename\": \"\",\"moredaydesc\": \"\",\"doseday\": \"\",\"skintest\":\"\",\"executivedept\": \"\",\"pharmacycode\": \"" + pharmacycode + "\",\"pharmacyname\":\"" + pharmacyname + "\",\"doctorpriv\":\"-1\" }";
                    MDC_AddJsonInfo(jsonStr);
                }
                #endregion
            }

        }


        /// <summary>
        /// 合理用药审查
        /// </summary>
        /// <param name="checkType">1 用药审查，0 用药指导单</param>
        /// <param name="alOrder"></param>
        /// <returns></returns>
        public int PASSCheck(int checkType, ArrayList alOrder)
        {
            if (checkType == 1)
            {
                int piShowMode = 1;
                int piIsSave = 1;

                int rev = MDC_DoCheck(piShowMode, piIsSave);
                if (rev < 1)
                {
                    this.err = "状态码:" + rev + "";
                    return -1;
                }
                string pcInHospNo = string.Empty;
                string pcPatCode = passPatient.ID;
                string pcVisitCode = "1";
                int piTaskType = 1;
                if (passPatient is Neusoft.HISFC.Models.RADT.PatientInfo)
                {
                    pcInHospNo = passPatient.ID;
                    pcPatCode = passPatient.PID.PatientNO;
                    pcVisitCode = (passPatient as Neusoft.HISFC.Models.RADT.PatientInfo).InTimes.ToString();
                }
                else
                {
                    pcInHospNo = passPatient.ID;
                    pcPatCode = passPatient.PID.CardNO;
                    piTaskType = 2;
                }

                //A.门诊 1-通过，0-不能通过
                //住院 1-通过，0-不能通过，-1-待定
                rev = MDC_GetTaskStatus(pcPatCode, pcInHospNo, pcVisitCode, "", piTaskType);
                if (rev == 0)
                {
                    this.err = "药物审查未通过，不允许继续保存医嘱！";
                    return -1;
                }
                else if (rev == -1)
                {
                    this.err = "药物审查为待定状态，请继续等待！";
                    return -1;
                }

                string errMsg = "";
                foreach (Neusoft.HISFC.Models.Order.Order order in alOrder)
                {
                    string orderId = order.ID; //.Id.Value.ToString();
                    if (MDC_GetWarningCode(orderId) == 1)
                    {
                        errMsg = errMsg + orderId.ToString() + ",";
                    }
                }

                if (!string.IsNullOrEmpty(errMsg))
                {
                    errMsg = errMsg.Substring(0, errMsg.Length - 1);
                }

                //int x = MDC_GetWarningCode("");
            }
            else
            {
                return MDC_DoPASSCommand("34");
            }
            return 1;
        }



        /// <summary>
        /// 传入一个查询药品函数(显示工具条)
        /// </summary>
        /// <param name="drugCode"></param>
        /// <param name="drugName"></param>
        /// <returns></returns>
        public int Pass4Toolbar(string drugCode, string drugName)
        {
            string pcDrugUniqueCode = drugCode;
            string pcDrugName = drugName;

            MDC_DoSetDrug(pcDrugUniqueCode, pcDrugName);


            return 1;
        }

        /// <summary>
        /// 右键查询药品简要信息
        /// </summary>
        /// <param name="drugCode"></param>
        /// <param name="drugName"></param>
        /// <returns></returns>
        public int Pass4DrugInfo(int w, string drugCode, string drugName)
        {
            string pcDrugUniqueCode = drugCode;
            string pcDrugName = drugName;

            if (MDC_DoSetDrug(pcDrugUniqueCode, pcDrugName) == 1 && MDC_DoRefDrugEnable(51) != 0)
            {
                if (w == 1)
                {
                    return MDC_DoRefDrug(51);
                }
                else
                {
                    return MDC_DoRefDrug(11);
                }

            }

            return 0;
        }


        /// <summary>
        /// 用药指导单
        /// </summary>
        /// <param name="drugCode"></param>
        /// <param name="drugName"></param>
        /// <returns></returns>
        public int Pass4DrugGuide(int w, string drugCode, string drugName)
        {
            string pcDrugUniqueCode = drugCode;
            string pcDrugName = drugName;

            if (MDC_DoSetDrug(pcDrugUniqueCode, pcDrugName) == 1 && MDC_DoRefDrugEnable(51) != 0)
            {
                if (w == 1)
                {
                    return MDC_DoRefDrug(51);
                }
                else
                {
                    return MDC_DoRefDrug(11);
                }

            }

            return 0;
        }



        class Unit
        {
            public string baseDose { get; set; }
            public string doseUnit { get; set; }
            public string onceDose { get; set; }
            public string onceUnit { get; set; }
        }


        //        private Unit getDoseUnit(string moOrder)
        //        {
        //            HISFC.BizLogic.Pharmacy.Item logic = new Neusoft.HISFC.BizLogic.Pharmacy.Item();
        //            ArrayList al = new ArrayList();

        //            string sql = string.Format(@"SELECT r.base_dose,b.dose_unit,r.once_dose,r.once_unit  /*,r.**/ 
        //                                           FROM met_ord_recipedetail r,pha_com_baseinfo b 
        //                                          WHERE r.item_code = b.drug_code 
        //                                            and r.drug_flag='1'
        //                                            and r.sequence_no = '{0}'", moOrder);

        //            if (logic.ExecQuery(sql) == -1)
        //            {
        //                logic.Err = "剂量单位转换出错：" + logic.Err;
        //                return null;
        //            }
        //            try
        //            {
        //                Unit Unit = new Unit(); ;

        //                while (logic.Reader.Read())
        //                {

        //                    Unit.baseDose = logic.Reader[0].ToString();
        //                    Unit.doseUnit = logic.Reader[1].ToString();
        //                    Unit.onceDose = logic.Reader[2].ToString();
        //                    Unit.onceUnit = logic.Reader[3].ToString();

        //                    //al.Add(Unit);
        //                }
        //                return Unit;
        //            }
        //            catch (Exception ex)
        //            {
        //                logic.Err = "剂量单位转换出错！" + ex.Message;
        //                logic.ErrCode = "-1";
        //                logic.WriteErr();
        //                return null;
        //            }
        //            finally
        //            {
        //                logic.Reader.Close();
        //            }
        //        }

    }
}
