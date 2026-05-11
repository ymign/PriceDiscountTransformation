using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDong
{
    /// <summary>
    /// 获取门诊医生工作量业务层数据上传
    /// 首页后续更新主表和婴儿表函数
    /// </summary>
    public class WorklogFuntion : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 获取工作量报表数据
        /// </summary>
        /// <param name="dsM">返回dataset</param>
        /// <param name="Type">工作量类型</param>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">结束时间</param>
        /// <returns></returns>
        public int GetMSpecial(ref DataSet dsM, string Type, DateTime fromTime, DateTime toTime)
        {
            string sql = string.Empty;
            string query = string.Empty;
            switch (Type)
            {
                case "tWorkLog":
                    query = "Report.DoctWordStat.WorkLogUpload";
                    break;
                case "tEmergeLog":
                    query = "Report.DoctWordStat.EmergeLogUpload";
                    break;
                case "tSpecialLog":
                    query = "Report.DoctWordStat.SpecialLogUpload";
                    break;
                case "TZyWardWorklog":
                    query = "Report.DoctWordStat.TZyWardWorklog";
                    break;
            }
            if (this.Sql.GetSql(query, ref sql) == -1)
            {
                this.Err += "GetSQlWrong!";
                return -1;
            }
            sql = string.Format(sql, fromTime, toTime);
            return this.ExecQuery(sql, ref dsM);
        }

        public string GetWorkLogUploadSql(string Type, DateTime fromTime, DateTime toTime)
        {
            #region
            string sql = string.Empty;
            string query = string.Empty;
            switch (Type)
            {
                case "tWorkLog":
                    query = "Report.DoctWordStat.WorkLogUpload";//医生门诊工作日志
                    break;
                case "tEmergeLog":
                    query = "Report.DoctWordStat.EmergeLogUpload";//急诊工作日志
                    break;
                case "tSpecialLog":
                    query = "Report.DoctWordStat.SpecialLogUpload";//专科门诊病人数
                    break;
                case "TZyWardWorklog":
                    query = "Report.DoctWordStat.TZyWardWorklog";//住院病房日志
                    break;
            }
            if (this.Sql.GetSql(query, ref sql) == -1)
            {
                this.Err += "GetSQlWrong!";
                return string.Empty;
            }
            sql = string.Format(sql, fromTime, toTime);
            return sql;
            #endregion
        }

        /// <summary>
        /// 更新主表住院次数
        /// </summary>
        /// <param name="patientNo"></param>
        /// <param name="intimes"></param>
        /// <returns></returns>
        public int UpdatePatientInTimes(string patientNo, int intimes)
        {
            string sqlStr = "";

            if (string.IsNullOrEmpty(patientNo))
            {
                this.Err = "患者住院流水号为空!";
                return -1;
            }
            if (intimes <= 0)
            {
                this.Err = "住院次数输入有误";
                return -1;
            }

            if (this.Sql.GetSql("RADT.Inpatient.UpdatePatientInTimes", ref sqlStr) == -1)
            {
                this.Err = "获取Sql索引出错,索引号:";
                return -1;
            }

            try
            {
                sqlStr = string.Format(sqlStr, patientNo, intimes);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }

            return this.ExecNoQuery(sqlStr);
        }

        /// <summary>
        /// 更新主表
        /// </summary>
        /// <param name="PatientInfo"></param>
        /// <returns></returns>
        public int UpdatePatient(Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo)
        {
            #region "接口说明"

            //			/接口名称 RADT.InPatient.UpdatePatient.1
            //			/max 66
            //					<!-- 0  --住院流水号,1 --姓名 2   --住院号   ,3   --就诊卡号  ,4   --医疗证号
            //			    ,5     --医疗类别,   ,6   --性别   ,7   --身份证号  ,8   --拼音     ,9   --生日
            //			    ,10   --职业代码     ,11   --工作单位    ,12   --工作单位电话      ,13   --单位邮编
            //			    ,14   --户口或家庭地址     ,15   --家庭电话   ,16   --户口或家庭邮编   , 17  --籍贯name
            //			    ,18   --出生地代码        ,19   --民族id    ,20  --民族name    ,21   --联系人姓名
            //			    ,22   --联系人电话       ,23   --联系人地址     ,24   --联系人关系id , 25   --联系人关系id 
            //			    ,26   --婚姻状况id              ,27  --婚姻状况name  ,28   --国籍id     29 --国籍名称
            //			    ,30   --身高           ,31   --体重         ,32   -- 职位id    ,33   --ABO血型
            //			    ,34   --重大疾病标志    ,35   --过敏标志            
            //			    ,36   --入院日期      ,37   --科室代码   , 38  --科室名称  , 39  --结算类别id 1-自费  2-保险 3-公费在职 4-公费退休 5-公费高干
            //			    ,40   --结算类别名称   , 41  --合同代码   , 42  --合同单位名称  , 43  --床号
            //			   , 44 --护理单元代码  , 45  --护理单元名称, 46 --医师代码(住院), 47 --医师姓名(住院)
            //			   , 48 --医师代码(主治) , 49 --医师姓名(主治) , 50 --医师代码(主任) , 51 --医师姓名(主任)
            //			   , 52 --医师代码(实习) , 53 --医师姓名(实习), 54  --护士代码(责任), 55  --护士姓名(责任)
            //			   , 56  --入院情况id  , 57  --入院情况name   , 58  --入院途径id    , 59  --入院途径name      
            //			   , 60  --入院来源id 1 -门诊 2 -急诊 3 -转科 4 -转院    , 61  --入院来源name
            //			   , 62  --住院登记  i-病房接诊 -出院登记 o-出院结算 p-预约出院 n-无费退院
            //			  ,  63  --出院日期(预约)  , 64  --出院日期 , 65  --是否在ICU 0 no 1 yes ,66 操作员 -->

            #endregion

            string strSql = string.Empty;
            if (Sql.GetSql("RADT.InPatient.UpdatePatient.2", ref strSql) == -1)
            {
                return -1;
            }

            try
            {
                string[] s = new string[84];
                try
                {
                    s[0] = PatientInfo.ID; // --住院流水号
                    s[1] = PatientInfo.Name; //--姓名
                    s[2] = PatientInfo.PID.PatientNO; //  --住院号
                    s[3] = PatientInfo.PID.CardNO; //  --就诊卡号
                    s[4] = PatientInfo.SSN; //  --医疗证号
                    s[5] = PatientInfo.PVisit.MedicalType.ID; //    --医疗类别id
                    s[6] = PatientInfo.Sex.ID.ToString(); //  --性别
                    s[7] = PatientInfo.IDCard; //  --身份证号
                    s[8] = PatientInfo.Memo; //  --拼音
                    s[9] = PatientInfo.Birthday.ToString(); //  --生日
                    s[10] = PatientInfo.Profession.ID; //  --职业名称
                    s[11] = PatientInfo.CompanyName; //  --工作单位
                    s[12] = PatientInfo.PhoneBusiness; //  --工作单位电话
                    s[13] = PatientInfo.User01; //  --单位邮编
                    s[14] = PatientInfo.AddressHome; //  --户口或家庭地址
                    s[15] = PatientInfo.PhoneHome; //  --家庭电话
                    s[16] = PatientInfo.User02; //  --户口或家庭邮编
                    s[17] = PatientInfo.DIST; // --籍贯name
                    s[18] = PatientInfo.AreaCode; //  --出生地代码
                    s[19] = PatientInfo.Nationality.ID; //  --民族id
                    s[20] = PatientInfo.Nationality.Name; // --民族name
                    s[21] = PatientInfo.Kin.Name; //  --联系人姓名
                    s[22] = PatientInfo.Kin.RelationPhone; //  --联系人电话
                    s[23] = PatientInfo.Kin.RelationAddress; //  --联系人地址
                    s[24] = PatientInfo.Kin.Relation.ID; //  --联系人关系id
                    s[25] = PatientInfo.Kin.Relation.Name; //  --联系人关系name
                    s[26] = PatientInfo.MaritalStatus.ID.ToString(); //  --婚姻状况id
                    s[27] = PatientInfo.MaritalStatus.Name; // --婚姻状况name
                    s[28] = PatientInfo.Country.ID; //  --国籍id
                    s[29] = PatientInfo.Country.Name; //--国籍名称
                    s[30] = PatientInfo.Height; //  --身高
                    s[31] = PatientInfo.Weight; //  --体重
                    s[32] = PatientInfo.Profession.Name; //  --职业id
                    s[33] = PatientInfo.BloodType.ID.ToString(); //  --ABO血型
                    s[34] = Neusoft.FrameWork.Function.NConvert.ToInt32(PatientInfo.Disease.IsMainDisease).ToString(); //  --重大疾病标志
                    s[35] = Neusoft.FrameWork.Function.NConvert.ToInt32(PatientInfo.Disease.IsAlleray).ToString(); //  --过敏标志
                    s[36] = PatientInfo.PVisit.InTime.ToString(); //  --入院日期
                    s[37] = PatientInfo.PVisit.PatientLocation.Dept.ID; //  --科室代码
                    s[38] = PatientInfo.PVisit.PatientLocation.Dept.Name; // --科室名称
                    s[39] = PatientInfo.Pact.PayKind.ID; // --结算类别id 1-自费  2-保险 3-公费在职 4-公费退休 5-公费高干
                    s[40] = PatientInfo.Pact.PayKind.Name; //  --结算类别名称
                    s[41] = PatientInfo.Pact.ID; // --合同代码
                    s[42] = PatientInfo.Pact.Name; // --合同单位名称
                    s[43] = PatientInfo.PVisit.PatientLocation.Bed.ID; // --床号
                    s[44] = PatientInfo.PVisit.PatientLocation.NurseCell.ID; //--护理单元代码
                    s[45] = PatientInfo.PVisit.PatientLocation.NurseCell.Name; // --护理单元名称
                    s[46] = PatientInfo.PVisit.AdmittingDoctor.ID; //--医师代码(住院)
                    s[47] = PatientInfo.PVisit.AdmittingDoctor.Name; //--医师姓名(住院)
                    s[48] = PatientInfo.PVisit.AttendingDoctor.ID; // --医师代码(主治)
                    s[49] = PatientInfo.PVisit.AttendingDoctor.Name; //--医师姓名(主治)
                    s[50] = PatientInfo.PVisit.ConsultingDoctor.ID; // --医师代码(主任)
                    s[51] = PatientInfo.PVisit.ConsultingDoctor.Name; //--医师姓名(主任)
                    s[52] = PatientInfo.PVisit.TempDoctor.ID; //--医师代码(实习)
                    s[53] = PatientInfo.PVisit.TempDoctor.Name; //--医师姓名(实习)
                    s[54] = PatientInfo.PVisit.AdmittingNurse.ID; // --护士代码(责任)
                    s[55] = PatientInfo.PVisit.AdmittingNurse.Name; // --护士姓名(责任)
                    s[56] = PatientInfo.PVisit.Circs.ID; // --入院情况id
                    s[57] = PatientInfo.PVisit.Circs.Name; // --入院情况name
                    s[58] = PatientInfo.PVisit.AdmitSource.ID; // --入院途径id
                    s[59] = PatientInfo.PVisit.AdmitSource.Name; // --入院途径name
                    s[60] = PatientInfo.PVisit.InSource.ID; // --入院来源id 1 -门诊 2 -急诊 3 -转科 4 -转院
                    s[61] = PatientInfo.PVisit.InSource.Name; // --入院来源name
                    s[62] = PatientInfo.PVisit.InState.ID.ToString(); // --住院登记  i-病房接诊 -出院登记 o-出院结算 p-预约出院 n-无费退院
                    s[63] = PatientInfo.PVisit.PreOutTime.ToString(); // --出院日期(预约)
                    s[64] = PatientInfo.PVisit.OutTime.ToString(); // --出院日期
                    try
                    {
                        if (PatientInfo.PVisit.ICULocation == null)
                        {
                            s[65] = "0";
                        }
                        else
                        {
                            s[65] = "1"; // --是否在ICU
                        }
                        s[66] = Operator.ID;
                    }
                    catch (Exception ex)
                    {
                        Err = ex.Message;
                        WriteErr();
                    }
                    s[67] = PatientInfo.Memo;
                    s[68] = PatientInfo.FT.BloodLateFeeCost.ToString(); //血滞纳金
                    s[69] = PatientInfo.FT.BedLimitCost.ToString(); //床位上限
                    s[70] = PatientInfo.FT.AirLimitCost.ToString(); //空调上限
                    s[71] = PatientInfo.ProCreateNO; //生育保险电脑号
                    s[72] = PatientInfo.FT.FixFeeInterval.ToString(); //床费间隔
                    s[73] = PatientInfo.FT.BedOverDeal.ToString(); //超标处理
                    s[74] = PatientInfo.ExtendFlag; //是否允许日限额超标 0 不同意 1 同意
                    s[75] = PatientInfo.ExtendFlag1;
                    s[76] = PatientInfo.ExtendFlag2;
                    s[77] = PatientInfo.ClinicDiagnose; //门诊诊断
                    s[78] = PatientInfo.MainDiagnose; //住院主诊断
                    s[79] = PatientInfo.DoctorReceiver.ID;//收住医师
                    s[80] = Neusoft.FrameWork.Function.NConvert.ToInt32(PatientInfo.IsEncrypt).ToString();
                    s[81] = PatientInfo.NormalName;
                    s[82] = PatientInfo.IDCardType.ID;//证件类型
                    strSql = string.Format(strSql, s, PatientInfo.InTimes);
                    //    strSql = string.Format(strSql, PatientInfo.InTimes);

                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                    WriteErr();
                }
            }
            catch (Exception ex)
            {
                Err = "赋值时候出错！" + ex.Message;
                WriteErr();
                return -1;
            }

            int parm = ExecNoQuery(strSql);
            if (parm != 1)
            {
                return parm;
            }

            //如果婴儿主表信息修改,则同时更新婴儿表中的信息
            if (PatientInfo.ID.IndexOf("B") > 0)
            {
                return UpdateBabyInfo(PatientInfo);
            }

            return 1;
        }
        /// <summary>
        /// 更新婴儿表
        /// </summary>
        /// <param name="babyInfo"></param>
        /// <returns></returns>
        public int UpdateBabyInfo(Neusoft.HISFC.Models.RADT.PatientInfo babyInfo)
        {
            string strSql = string.Empty;
            if (Sql.GetSql("RADT.Inpatient.UpdateBabyInfo.1", ref strSql) == -1)
            {
                return -1;
            }

            try
            {
                string[] s = new string[13];
                s[0] = babyInfo.ID; //婴儿住院流水号
                s[1] = babyInfo.User01; //发生序号
                s[2] = babyInfo.Name; //姓名
                s[3] = babyInfo.Sex.ID.ToString(); //性别
                s[4] = babyInfo.Birthday.ToString(); //生日
                s[5] = babyInfo.Height; //身高
                s[6] = babyInfo.Weight; //体重
                s[7] = babyInfo.BloodType.ID.ToString(); //血型编码
                s[8] = babyInfo.PVisit.InTime.ToString(); //入院日期
                s[9] = babyInfo.PVisit.PreOutTime.ToString(); //出院日期(预约)
                s[10] = Operator.ID; //操作员
                s[11] = GetSysDateTime().ToString(); //操作日期
                s[12] = babyInfo.PID.MotherInpatientNO; //母亲住院流水号
                strSql = string.Format(strSql, s);
            }
            catch (Exception ex)
            {
                Err = "赋值时候出错！" + ex.Message;
                WriteErr();
                return -1;
            }

            return ExecNoQuery(strSql);
        }

    }
}
