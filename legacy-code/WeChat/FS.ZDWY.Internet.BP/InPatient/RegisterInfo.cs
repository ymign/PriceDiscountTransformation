using FS.ZDWY.Internet.Models;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.RADT;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FS.ZDWY.Internet.BP.InPatient
{
    public class RegisterInfo
    {
        bool isCanModifyInTime = false;
        bool isArriveProcess = false;
        /// <summary>
        /// 管理类
        /// </summary>
        private Neusoft.SOC.HISFC.BizProcess.CommonInterface.CommonController commonController = Neusoft.SOC.HISFC.BizProcess.CommonInterface.CommonController.CreateInstance();
        public PatientInfo GetPatientInfo(PatientInfo patient, InPatientRegistInfo Info)
        {
            if (patient == null)
            {
                patient = new PatientInfo();
            }

            patient.Name = Info.Name;//名字
            patient.SSN = SpecialFilte(Info.SSN);//医保号
            patient.IDCard = SpecialFilte(Info.IDCard);//身份证

            patient.PID.PatientNO = Info.PatientNO; //住院号
            patient.PID.CardNO = Info.CardNO;//门诊卡号

            patient.ID = Info.ID;//住院流水号

            patient.ProCreateNO = Info.ProCreateNO;//生育保险电脑号
            if (isCanModifyInTime)
            {
                patient.PVisit.InTime = Convert.ToDateTime(Info.InTime);//入院日期
            }
            else
            {
                patient.PVisit.InTime = commonController.GetSystemTime(); //入院日期
            }
            patient.Pact.ID = Info.PactID;//合同单位编码
            patient.Pact.Name = Info.PactName;//合同单位名称
            patient.Pact.PayKind = this.commonController.GetPayKind(patient.Pact.ID);//结算类别
            if (patient.Pact.PayKind == null)
            {
                throw new Exception("获取结算类别错误！合同单位代码:"+ patient.Pact.ID);
                return null;
            }
            //暂时屏蔽掉 接诊时候给床位
            //接诊
            if (isArriveProcess)
            {
                //Neusoft.HISFC.Models.Base.Bed bedObj = Info.cmbBedNO.SelectedItem as Neusoft.HISFC.Models.Base.Bed;
                //patient.PVisit.PatientLocation.NurseCell = bedObj.NurseStation;
                //patient.PVisit.PatientLocation.Bed = bedObj;
                //patient.PVisit.InState.ID = Neusoft.HISFC.Models.Base.EnumInState.I;
            }
            else
            {
                patient.PVisit.InState.ID = Neusoft.HISFC.Models.Base.EnumInState.R;
            }


            patient.Sex.ID = Info.Sex;//性别
            if (Info.Nationality != null)
            {
                patient.Nationality = GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, Info.Nationality);//民族
            }

            patient.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Info.Birthday);//生日

            if (patient.Birthday < new DateTime(1753, 1, 1))
            {
                throw new Exception("出生日期输入错误，请修改！");
                return null;
            }

            patient.PVisit.PatientLocation.Dept.ID = Info.DeptID;//科室编码
            patient.PVisit.PatientLocation.Dept.Name = commonController.GetDepartmentName(patient.PVisit.PatientLocation.Dept.ID);//科室名称

            //modify by zhy
            //patient.CompanyName = Info.txtWorkAddress;//工作单位
            if (Info.CompanyName == "")
            {
                patient.CompanyName = "-";
            }
            else
            {
                patient.CompanyName = Info.CompanyName;
            }
            //end modify

            patient.MaritalStatus.ID = SpecialFilte(Info.MaritalStatus);//婚姻状况
            patient.DIST = Info.DIST;//籍贯
            patient.AreaCode = Info.AreaCode;//出生地
            patient.Country.ID = Info.Country;//国籍ID
            if (patient.Country.ID != "")
                patient.Country.Name = GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, Info.Country).Name;//国籍
            patient.Profession.ID = Info.Profession;//职位ID
            if (patient.Profession.ID != "")
                patient.Profession.Name = GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.PROFESSION, Info.Profession).Name;//职位名称
            patient.Kin.Name = SpecialFilte(Info.KinName);//联系人姓名
            patient.Kin.RelationPhone = Info.KinRelationPhone;//联系人备注-电话
            patient.Kin.Relation.ID = Info.KinRelation;//与患者关系编码
            if (patient.Kin.Relation.ID != "")
                patient.Kin.Relation.Name = GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.RELATIVE, Info.KinRelation).Name;//与患者关系
            patient.Kin.RelationAddress = Info.KinRelationAddress;//联系人地址
            //patient.AddressHome = Info.txtHomeAddress;//家庭地址
            //modify by zhy
            // patient.HomeZip = Info.txtHomeZip;//家庭地址邮编
            if (Info.HomeZip == "")
            {
                patient.HomeZip = "-";
            }
            else
            {
                patient.HomeZip = Info.HomeZip;
            }
            //end modify
            patient.AddressBusiness = Info.NowAddr + Info.NowAdd;//现地址
            patient.AddressHome = Info.HomeAddr + Info.HomeAdd;// tangyi add by 20220621
            patient.PhoneHome = Info.PhoneHome;//患者电话
            //modify by zhy
            //patient.PhoneBusiness = Info.txtWorkPhone;//单位电话 
            if (Info.PhoneBusiness == "")
            {
                patient.PhoneBusiness = "-";
            }
            else
            {
                patient.PhoneBusiness = Info.PhoneBusiness;
            }
            //end modify
            patient.IDCard = SpecialFilte(Info.IDCard);//身份证
            patient.PVisit.AdmitSource.ID = Info.AdmitSource;//入院途径
            patient.PVisit.AdmitSource.Name = GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INAVENUE, Info.AdmitSource).Name;//入院途径
            patient.PVisit.InSource.ID = Info.InSource;//入院来源
            patient.PVisit.InSource.Name = GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INSOURCE, Info.InSource).Name;//入院来源
            patient.PVisit.Circs.ID = Info.Circs;//入院情况
            patient.PVisit.Circs.Name = GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCIRCS, Info.Circs).Name;//入院情况
            patient.DoctorReceiver.ID = Info.DoctorReceiver;//收住医师

            //patient.PVisit.AdmittingDoctor.ID = Info.cmbDoctor.Tag.ToString();
            //patient.PVisit.AdmittingDoctor.Name = Info.cmbDoctor;
            //patient.PVisit.AttendingDoctor.ID = Info.cmbDoctor.Tag.ToString();
            //patient.PVisit.AttendingDoctor.Name = Info.cmbDoctor;

            patient.ClinicDiagnose = Info.ClinicDiagnose;//门诊诊断
            patient.ClinicDiagnoseNo = Info.ClinicDiagnoseNo;//门诊诊断编码

            patient.FT.BloodLateFeeCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(0);//血滞纳金
            //路志鹏 修改住院次数 目的：本次住院登记的住院次数应该是上一次住院次数加1
            patient.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(1);//住院次数？初次就增加啦
            patient.FT.LeftCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(0);//预交金
            patient.FT.PrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(0);//预交金
            patient.FT.FixFeeInterval = 1;//默认为1

            patient.FT.AirLimitCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(0);  //监护床(空调上限)
            patient.FT.DayLimitCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(0);  //公费日限
            patient.FT.BedLimitCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(0);  //普通标准(床位上限)
            patient.FT.FTRate.PayRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(0);  //自付比例
            //patient.FT.BedOverDeal = Info.cmbBedOverDeal.SelectedIndex.ToString();        //超标处理 
            //patient.FT.OvertopCost = -patient.FT.DayLimitCost;//超标金额
            //patient.ExtendFlag = Info.cmbOverLop.SelectedIndex.ToString();   //日限处理
            //patient.FT.DayLimitTotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Info.txtDayLimit);//日限额累计     

            //加密
            patient.IsEncrypt = false;

            if (patient.IsEncrypt)
            {

                patient.NormalName = Neusoft.FrameWork.WinForms.Classes.Function.Encrypt3DES(patient.Name);
                patient.Name = "******";
            }

            patient.PVisit.PatientLocation.NurseCell.ID = getNurse(Info.DeptID).ID;//病区编码
            patient.PVisit.PatientLocation.NurseCell.Name = getNurse(Info.DeptID).Name;//病区名称
            #region 增加备注
            //patient.Memo = Info.txtMemo.Trim();
            #endregion
            patient.DayOperationFlag = Info.DayOperationFlag;//是否日间手术标记 {7DD8FC90-9857-026E-E9C7-D7558D0054EF}
            return patient;
        }

        //判断输入的是否为"-"，如果输入的是"-"符号则代表为空
        private string SpecialFilte(string controlValue)
        {
            if (controlValue == "-")
            {
                return "";
            }
            else
            {
                return controlValue;
            }
        }

        /// <summary>
        /// 获取常数类实体
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private Neusoft.FrameWork.Models.NeuObject GetConstant(Neusoft.HISFC.Models.Base.EnumConstant type,string code)
        {
            Neusoft.FrameWork.Models.NeuObject constant = commonController.GetConstant(type, code);
            return constant;
        }

        /// <summary>
        /// 获取常数类实体
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<DicObject> QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant type)
        {
            ArrayList al = commonController.QueryConstant(type);
            List<DicObject> dicObjects = new List<DicObject>();
            if (al != null && al.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Base.Const neu in al)
                {
                    DicObject dicObject = new DicObject();
                    dicObject.Code = neu.ID;
                    dicObject.Name = neu.Name;
                    dicObject.Sort = neu.SpellCode;
                    dicObjects.Add(dicObject);
                }
            }
            return dicObjects;
        }
        /// <summary>
        /// 获取婚姻状况
        /// </summary>
        /// <returns></returns>
        public List<DicObject> GetMaritalStatusList()
        {
            List<DicObject> dicObjects = new List<DicObject>();
            dicObjects.Add(new DicObject { Code = "S", Name = "未婚" });
            dicObjects.Add(new DicObject { Code = "M", Name = "已婚" });
            dicObjects.Add(new DicObject { Code = "D", Name = "离婚" });
            dicObjects.Add(new DicObject { Code = "W", Name = "丧偶" });
            dicObjects.Add(new DicObject { Code = "O", Name = "其他" });
            return dicObjects;
        }
        /// <summary>
        /// 获取是否日间手术状态
        /// </summary>
        /// <returns></returns>
        public List<DicObject> GetDayOperFlagList()
        {
            List<DicObject> dicObjects = new List<DicObject>();
            dicObjects.Add(new DicObject { Code = "0", Name = "否" });
            dicObjects.Add(new DicObject { Code = "1", Name = "日间手术" });
            dicObjects.Add(new DicObject { Code = "2", Name = "日间化疗" });
            return dicObjects;
        }

        /// <summary>
        /// 获取是否日间手术状态
        /// </summary>
        /// <returns></returns>
        public List<DicObject> GetICD()
        {
            Neusoft.HISFC.BizLogic.Manager.Constant con = new Neusoft.HISFC.BizLogic.Manager.Constant();
            ArrayList al = new ArrayList();
            al = con.GetICD10();
            List<DicObject> dicObjects = new List<DicObject>();
            if (al != null && al.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Base.Const neu in al)
                {
                    DicObject dicObject = new DicObject();
                    dicObject.Code = neu.ID;
                    dicObject.Name = neu.Name;
                    dicObjects.Add(dicObject);
                }
            }
            return dicObjects;
        }
        /// <summary>
        /// 获取医生
        /// </summary>
        /// <returns></returns>
        public List<DicObject> GetDoct()
        {
            ArrayList al = commonController.QueryEmployee(Neusoft.HISFC.Models.Base.EnumEmployeeType.D);
            List<DicObject> dicObjects = new List<DicObject>();
            if (al != null && al.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Base.Employee neu in al)
                {
                    DicObject dicObject = new DicObject();
                    dicObject.Code = neu.ID;
                    dicObject.Name = neu.Name;
                    dicObjects.Add(dicObject);
                }
            }
            return dicObjects;
        }
        /// <summary>
        /// 获取住院科室
        /// </summary>
        public List<DicObject> GetDept()
        {
            ArrayList alDept = new ArrayList();
            List<DicObject> dicObjects = new List<DicObject>();
            foreach (Neusoft.HISFC.Models.Base.Department s in commonController.QueryDepartment(true))
            {
                
                    DicObject dicObject = new DicObject();
                    dicObject.Code = s.ID;
                    dicObject.Name = s.Name;
                    dicObjects.Add(dicObject);
                
            }
            return dicObjects;
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        public List<DicObject> GetSex()
        {
            List<DicObject> dicObjects = new List<DicObject>();
            dicObjects.Add(new DicObject { Code = "M", Name = "男" });
            dicObjects.Add(new DicObject { Code = "F", Name = "女" });
            return dicObjects;
        }

        /// <summary>
        /// 获取合同单位
        /// </summary>
        public List<DicObject> GetPact()
        {
            ArrayList Pact = commonController.QueryInPatientPactInfo();
            List<DicObject> dicObjects = new List<DicObject>();
            foreach (NeuObject s in Pact)
            {

                DicObject dicObject = new DicObject();
                dicObject.Code = s.ID;
                dicObject.Name = s.Name;
                dicObjects.Add(dicObject);

            }
            return dicObjects;
        }
        
        /// <summary>
        /// 查找对应的护士站
        /// </summary>
        /// <returns></returns>
        private Neusoft.FrameWork.Models.NeuObject getNurse(string deptCode)
        {
            //查找对应的护士站
            Neusoft.FrameWork.Models.NeuObject nurseCell = null;
            ArrayList al = QueryNurseByDept(deptCode);
            if (al != null && al.Count > 0)
            {
                if (al.Count == 1)
                {
                    nurseCell = al[0] as Neusoft.FrameWork.Models.NeuObject;
                }
                else
                {
                    nurseCell = al[al.Count - 1] as Neusoft.FrameWork.Models.NeuObject;
                }
            }
            return nurseCell;
        }

        /// <summary>
        /// 根据科室找对应的病区
        /// </summary>
        /// <param name="deptStatCode"></param>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public static ArrayList QueryNurseByDept(string deptCode)
        {
            Neusoft.HISFC.BizLogic.Manager.DepartmentStatManager deptStatMgr = new Neusoft.HISFC.BizLogic.Manager.DepartmentStatManager();

            ArrayList alNurse = new ArrayList();
            ArrayList al = deptStatMgr.LoadByParent("01", deptCode);
            if (al == null || al.Count == 0)
            {
                al = deptStatMgr.LoadByChildren("01", deptCode);
                if (al != null)
                {
                    foreach (Neusoft.HISFC.Models.Base.DepartmentStat deptStat in al)
                    {
                        alNurse.Add(new Neusoft.FrameWork.Models.NeuObject(deptStat.PardepCode, deptStat.PardepName, ""));
                    }
                }
            }
            else
            {
                foreach (Neusoft.HISFC.Models.Base.DepartmentStat deptStat in al)
                {
                    alNurse.Add(new Neusoft.FrameWork.Models.NeuObject(deptStat.DeptCode, deptStat.DeptName, ""));
                }
            }
            return alNurse;
        }

        public List<DicObject> GetAddr()
        {
            Neusoft.HISFC.BizLogic.Manager.Constant con = new Neusoft.HISFC.BizLogic.Manager.Constant();
            ArrayList addrlist1 = new ArrayList();
            addrlist1 = con.GetList("BZDZK");
            List<DicObject> dicObjects = new List<DicObject>();
            foreach (NeuObject s in addrlist1)
            {

                DicObject dicObject = new DicObject();
                dicObject.Code = s.ID;
                dicObject.Name = s.Name;
                dicObjects.Add(dicObject);

            }
            return dicObjects;
        }


    }
}
