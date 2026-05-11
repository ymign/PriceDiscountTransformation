using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew
{
    public class Upload : IUpload
    {


        public SQLServerManager server = null;//new SQLServerManager();//数据链接实体
        public Function function = null;//new Function();//功能实体
        
       

        Neusoft.HISFC.BizLogic.Manager.Constant constMana = new Neusoft.HISFC.BizLogic.Manager.Constant();

        //单位列表 ADD BY ZHY
        Neusoft.FrameWork.Public.ObjectHelper UnitListHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        public void Commit()
        {
            server.Commit();
        }

        public void Rollback()
        {
            server.Rollback();
        }

        private string err = "";

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

        public void SetServer()
        {
            if (server == null)
            {
                this.server = new SQLServerManager();
            }
            if (function.Server == null)
            {
                function.Server = this.server;
            }
        }

        public Upload()
        {
            server = new SQLServerManager();//数据链接实体
            function = new Function();
            SetServer();
        }

                /// <summary>
        /// 根据住院流水号判断是否已经录入
        /// 返回值： 0 需要上传 1 已经上传
        /// </summary>
        /// <param name="fprn">病案号</param>
        /// <param name="fzyid">住院流水号</param>
        /// <returns></returns>
        public int GetIsNeedUpload(string fprn, string fzyid)
        {
            int iReturn = 0;
            int NotNeed = 0;
            int Need = 0;
            string strSQLNotNeed = @"select count(1) from tPatientVisit where FPRN ='{0}' AND fzyid = '{1}'";
            string strSQLNeed = @"select count(1) from tPatientVisit where FPRN ='{0}' AND fzyid = '{1}'";
            try
            {
                strSQLNotNeed = string.Format(strSQLNotNeed, fprn, fzyid);

                strSQLNeed = string.Format(strSQLNeed, fprn, fzyid);
            }
            catch
            {
                return -1;
            }

            string notn = "0";
            if (server.ExecuteQueryReturnOne(strSQLNotNeed, ref notn)<0)
            {
                return -1;
            }
          
            NotNeed = Neusoft.FrameWork.Function.NConvert.ToInt32(notn);

            string yneed = "0";
            if (server.ExecuteQueryReturnOne(strSQLNeed, ref yneed) < 0)
            {
                return -1;
            }
            Need = Neusoft.FrameWork.Function.NConvert.ToInt32(yneed);

            if (NotNeed == 1)
            {
                iReturn = 3;
            }
            else
            {
                if (Need == 1)
                {
                    iReturn = 1;
                }
                else
                {
                    iReturn = 2;
                }
            }
            return iReturn;
        }

        /// <summary>
        /// 获得住院次数
        /// ref inTimes =0 不需要更改；否则更改
        /// </summary>
        /// <param name="prn">住院号</param>
        /// <param name="fzyid">住院流水号</param>
        /// <param name="itype">类型 1已经上传未录入  2取中间表和录入中最大住院次数 </param>
        /// <param name="inTimes"></param>
        /// <returns></returns>
        public int GetInTimes(string prn, string fzyid, int itype, ref string inTimes)
        {
            string strSQLinTimes = string.Empty;
            if (itype == 1)
            {
                strSQLinTimes = @"SELECT FTIMES FROM tPatientVisit WHERE FZYID='{0}'";
                try
                {
                    strSQLinTimes = string.Format(strSQLinTimes, fzyid);
                }
                catch
                {
                    return -1;
                }
            }
            else
            {
                strSQLinTimes = @"select case when  max(a.ftimes) is null then 0 else  max(a.ftimes) end   ftimes from 
                                    (select max(ftimes) ftimes from tPatientVisit        where fprn='{0}'
                                    union 
                                    select max(ftimes)  ftimes from  tpatientvisit  where fprn='{0}') a";

                try
                {
                    strSQLinTimes = string.Format(strSQLinTimes, prn);
                }
                catch
                {
                    return -1;
                }
            }
            
            if (server.ExecuteQueryReturnOne(strSQLinTimes, ref inTimes) < 0)
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 根据病历号查询
        /// </summary>
        /// <param name="cardNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.PatientInfo GetPatientFromBA(string cardNO)
        {
            #region sql
            string strSQL = @"SELECT tPatientVisit.FPRN,
       FNAME,
       FTIMES,
       FFB,
       FAGE,
       FJOB,
       FSTATUS,
       FDWNAME,
       FDWADDR,
       FDWTELE,
       FDWPOST,
       FHKADDR,
       FHKPOST,
       FLXNAME,
       FRELATE,
       FLXADDR,
       FLXTELE,
       FRYDATE,
       FRYTIME,
       FRYINFO,
       FSOURCE,
       FRYTYKH,
       FRYDEPT,
       FZKTYKH,
       FZKDEPT,
       FCYTYKH,
       FCYDEPT,
       FCYDATE,
       FCYTIME,
       FDAYS,
       FMZZD,
       FRYZD,
       FQZDATE,
       FIFSS,
       FIFFYK,
       FBFZ,
       FYNGR,
       FPHZD,
       FGMYW,
       FBLOOD,
       FQJTIMES,
       FQJSUCTIMES,
       FISSZ,
       FSZQX,
       FBODY,
       FSUM1,
       FCWF,
       FXYF,
       FZYF,
       FZCYF,
       FZCHYF,
       FJCF,
       FZLF,
       FFSF,
       FSSF,
       FHYF,
       FSXF,
       FSYF,
       FJSF,
       FQTF,
       FSAMPLE,
       FQUALITY,
       FZRDOCTOR,
       FZZDOCT,
       FZYDOCT,
       FSXDOCT,
       FBMY,
       FMZCYACCO,
       FRYCYACCO,
       FOPACCO,
       FISZLFIRST,
       FISJCFIRST,
       FISZDFIRST,
       FTWILL,
       FQJBR,
       FQJSUC,
       FTHREQZ,
       FBABYNUM,
       FZLFZY, 
       FIFDBZ,
       FBACK,
       FSXFY,
       FSYFY,
       FWORKRQ,
       FIFZDSS,
       FMZDOCT,
       FJBFX
       FSEX,
        FBIRTHDAY,
        FBIRTHPLACE,
        FIDCard,
        fcountry,
        fnationality
  FROM tPatientVisit where tPatientVisit.fprn = '{0}' order by ftimes desc";

            #endregion

            //this.cmd.CommandText = string.Empty;

            try
            {
                strSQL = string.Format(strSQL, cardNO);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;

                return null;
            }

            //ReadSQL(strSQL);
            DataSet ds=new DataSet();
            if (server.Execute(strSQL, ref ds) < 0)
            {
                return null;
            }
            if (ds.Tables[0].Rows.Count <= 0)
            {
                this.Err = "没读到！";
                return null;
            }
            Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

            try
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    PatientInfo.PID.CardNO = dr[0].ToString();
                    //PatientInfo.Birthday.ToString(); //出生日期
                    //PatientInfo.Sex.ID.ToString(); //性别
                    //PatientInfo.IDCard; //身份证号
                    PatientInfo.Name = dr[1].ToString();

                    string sex = dr[88].ToString();
                    if (sex.Trim().Equals("1"))
                    {
                        sex = "M";
                    }
                    else if (sex.Trim().Equals("2"))
                    {
                        sex = "F";
                    }
                    PatientInfo.Sex.ID = sex;

                    PatientInfo.IDCard = dr[91].ToString();

                    PatientInfo.Profession.ID = dr[5].ToString(); //职业

                    PatientInfo.CompanyName = dr[7].ToString(); //工作单位
                    PatientInfo.PhoneBusiness = dr[9].ToString(); //单位电话
                    PatientInfo.BusinessZip = dr[10].ToString(); //单位邮编
                    PatientInfo.AddressHome = dr[11].ToString(); //户口或家庭所在
                    //PatientInfo.PhoneHome = dr[10].ToString(); //家庭电话
                    PatientInfo.HomeZip = dr[12].ToString(); //户口或家庭邮政编码
                    //PatientInfo.DIST = dr[10].ToString(); //籍贯
                    //PatientInfo.Nationality.ID = dr[10].ToString(); //民族
                    PatientInfo.Kin.Name = dr[13].ToString(); //联系人姓名
                    PatientInfo.Kin.RelationPhone = dr[16].ToString(); //联系人电话
                    PatientInfo.Kin.RelationAddress = dr[15].ToString(); //联系人住址
                    PatientInfo.Kin.Relation.Name = dr[14].ToString(); //联系人关系

                    PatientInfo.MaritalStatus.ID = dr[6].ToString(); //婚姻状况
                    //PatientInfo.Country.ID = dr[10].ToString(); //国籍

                    //PatientInfo.AreaCode = this.reader[10].ToString(); //出生地
                    try
                    {
                        PatientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(dr[89].ToString().Replace('/', '-') + " 00:00:00");
                    }
                    catch
                    {
                        PatientInfo.Birthday = DateTime.MinValue;
                    }

                    PatientInfo.AreaCode = dr[90].ToString();

                    PatientInfo.Country.Name = dr[92].ToString();
                    PatientInfo.Nationality.Name = dr[93].ToString();
                    //来源
                    PatientInfo.PVisit.InSource.ID = dr[20].ToString();
                    //医保号

                    PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(dr[2].ToString());

                    try
                    {
                        PatientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(dr[27].ToString().Replace('/', '-') + " 00:00:00");
                    }
                    catch
                    {
                        PatientInfo.PVisit.OutTime = DateTime.MinValue;

                        PatientInfo.User01 = dr[27].ToString();
                    }

                    PatientInfo.User02 = dr[38].ToString();//过敏药物
                    break;
                    
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return PatientInfo;
        }

        /// <summary>
        /// 根据住院流水号判断是否已经录入
        /// 返回值： 0 需要上传 1 已经上传
        /// </summary>
        /// <param name="fprn">病案号</param>
        /// <param name="in_date">入院时间</param>
        /// <returns></returns>
        public int GetIsHavedNoUpload(string fprn, DateTime in_date)
        {
            int iReturn = 0;
            int NotNeed = 0;
            string strSQLNotNeed = @"SELECT count(1) FROM TPATIENTVISIT t WHERE t.FPRN='{0}' AND t.FRYDATE='{1}'";
            try
            {
                strSQLNotNeed = string.Format(strSQLNotNeed, function.PatientNoChang(fprn.PadLeft(10, '0').Substring(function.PatientNoSubstr())), in_date.ToShortDateString());
            }
            catch
            {
                return -1;
            }

            //ReadSQL(strSQLNotNeed);
            string notn="0";
            if (server.ExecuteQueryReturnOne(strSQLNotNeed, ref notn) < 0)
            {
                return -1;
            }

            NotNeed = Neusoft.FrameWork.Function.NConvert.ToInt32(notn);

            if (NotNeed == 1)
            {
                iReturn = 1;
            }
            else
            {
                iReturn = 0;
            }
            return iReturn;
        }

        #region TPATIENTvisit(HIS_BA1)
        /// <summary>
        /// 删除中间表数据 by 住院流水号
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns></returns>
        public int DeleteHISBA1ByFzyid(string inpatientNO)
        {
            //string strSQL = @"delete from TPATIENTvisit WHERE FZYID='{0}' ";

            //strSQL = string.Format(strSQL, inpatientNO);

            return -1;
        }

        /// <summary>
        /// 删除主表信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int DeleteHISBA1ByFzyid(string inpatientNO, int times)
        {
            string strSQL = @"delete from TPATIENTvisit WHERE FPRN='{0}'  AND FTIMES={1} ";

            strSQL = string.Format(strSQL, inpatientNO, times);

            return server.ExecuteNonQuery(strSQL);

        }

        /// <summary>
        ///  HIS_BA1 --病人信息
        /// </summary>
        /// <param name="b">病案首页实体</param>
        /// <param name="Feeds">费用信息</param>
        /// <param name="alChangeDepe">转科信息</param>
        /// <param name="alDose"> 诊断</param>
        ///<param name="isMetCasBase">是否病案主表数据</param> 
        /// <returns></returns>
        public int InsertPatientInfoBA1Drgs(Neusoft.HISFC.Models.HealthRecord.Base b, DataSet Feeds,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            
            string strSQL = function.GetInsertHISBA1SQLDrgs(b, Feeds, alChangeDepe, alDose, isMetCasBase);
            if (strSQL == null || strSQL == "")
            {
                return -1;
            }
            //ReadSQL(strSQL);
            return server.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// 更改fzkdate  fzkrq 为空
        /// </summary>
        /// <param name="fprn"></param>
        /// <returns></returns>
        public int UpdateHISBA1Fzkdate(string fprn)
        {
            string strSQL = @"update TPATIENTvisit set  fzkdate=null  where FPRN='{0}' and fzkdate<'2000-1-1 00:00:00'";
            try
            {
                strSQL = string.Format(strSQL, fprn.Substring(function.PatientNoSubstr()));
            }
            catch
            {
            }
            return server.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        ///  HIS_BA1 --病人信息
        /// </summary>
        /// <param name="b">病案信息</param>
        /// <param name="alFee">费用信息</param>
        /// <param name="alChangeDepe">转科信息</param>
        /// <param name="alDose"> 诊断</param>
        ///<param name="isMetCasBase">是否病案主表数据</param> 
        /// <returns></returns>
        //public int InsertPatientInfoBA1(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
        //    System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        //{
        //    string strSQL = function.GetInsertHISBA1SQL(b, alFee, alChangeDepe, alDose, isMetCasBase);

        //    if (strSQL == null || strSQL == "")
        //    {
        //        return -1;
        //    }
        //    //ReadSQL(strSQL);

        //    return server.ExecuteNonQuery(strSQL);
        //}

        #endregion

        #region tSwitchKs(HISBA2)
        /// <summary>
        /// 删除HISBA2
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <param name="times">住院次数</param>
        /// <returns></returns>
        public int DeleteHISBA2(string inpatientNO, int times)
        {
            string strSQL = @"DELETE FROM tSwitchKs WHERE FPRN='{0}' AND FTIMES={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            return server.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// HIS_BA2  --病人诊断信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertHISBA2(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.RADT.Location obj)
        {
            string sql = @"INSERT INTO tSwitchKs
(FPRN,FTIMES,FZKTYKH,FZKDEPT,FZKDATE,FZKTIME)
VALUES
('{0}',{1},'{2}','{3}','{4}','{5}') ";

            DateTime dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Dept.Memo);
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');

            try
            {
                sql = string.Format(sql,
                                                 function.PatientNoChang(patientNO.Substring(function.PatientNoSubstr())),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 function.ConverDept(obj.Dept.ID).ID,//转科统一科号，HIS接收时存储HIS科号
                                                 function.ConverDept(obj.Dept.ID).Name,//转科科别
                                                 function.ChangeDateTime(obj.Dept.Memo),//转科日期
                                                 dt.ToShortTimeString());
            }
            catch(Exception e)
            {
                this.Err = "赋值失败! 请联系信息科! " + e.ToString();
                return -1;
            }

            return server.ExecuteNonQuery(sql);

        }
        #endregion

        #region tDiagnose(HIS_BA3)

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int DeleteHISBA3(string inpatientNO, int times)
        {
            string strSQL = @"delete from tDiagnose where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            return server.ExecuteNonQuery(strSQL);

        }

        /// <summary>
        /// HIS_BA3  --病人诊断信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertHISBA3Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Diagnose obj)
        {
            if (obj.DiagInfo.DiagType.ID == "16")
            {
                obj.DiagInfo.DiagType.ID = "f";
            }
            if (obj.DiagInfo.DiagType.ID == "14")
            {
                obj.DiagInfo.DiagType.ID = "2";
            }
            string sql = @"insert into tDiagnose (fprn,ftimes,FZDLX, FICDVersion, FICDM, FJBNAME,FRYBQBH,FRYBQ) values ('{0}',{1},'{2}',{3},'{4}','{5}','{6}','{7}')";
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql,
                                                 function.PatientNoChang(patientNO.Substring(function.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),
                    //patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 obj.DiagInfo.DiagType.ID,//对照
                                                 "11",//ICD版本号
                                                 obj.DiagInfo.ICD10.ID,
                    //obj.DiagInfo.Name,//医生描述的诊断名称
                                                 obj.DiagInfo.ICD10.Name,//标准ICD名称
                                                 obj.DiagOutState,
                                                 obj.Memo);
            }
            catch(Exception e)
            {
                this.Err = "赋值失败! 请联系信息科! " + e.ToString();
                return -1;
            }

            return server.ExecuteNonQuery(sql);

        }
        #endregion

        #region tOperation(HIS_BA4)

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public int DeleteHISBA4(string inpatientNO, int time)
        {
            string strSQL = @"delete from tOperation where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO, time.ToString());

            //ReadSQL(strSQL);
            return server.ExecuteNonQuery(strSQL);

        }

        /// <summary>
        /// insert HIS_BA4  --手术信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">手术信息实体</param>
        /// <returns></returns>
        public int insertHisBa4Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.OperationDetail obj)
        {

            #region  sql
            string sql = @"INSERT INTO tOperation
(
FPRN ,--病案号 0
FTIMES ,--	次数
FNAME	,--病人姓名
FOPTIMES   ,--手术次数
FOPCODE	,--	手术码
FOP	,--	手术码对应名称
FOPDATE	,--	手术日期
FQIEKOUBH	,--	切口编号
FQIEKOU	,--切口
FYUHEBH	,--愈合编号
FYUHE	,--	愈合--10
FDOCBH	,--	手术医生编号
FDOCNAME	,--	手术医生
FMAZUIBH   ,--	麻醉方式编号
FMAZUI	,--麻醉方式
FIFFSOP	,--	是否附加手术
FOPDOCT1BH	,--I助编号
FOPDOCT1	,--I助姓名
FOPDOCT2BH	,--	II助编号
FOPDOCT2	,--II助姓名
FMZDOCTBH	,--	麻醉医生编号--20
FMZDOCT,	--麻醉医生
FZQSSBH,--择期手术编号1是，0否
FZQSS,--择期手术
FSSJBBH,--手术级别编号
FSSJB,--手术级别
FOPKSNAME,--手术医生所在科室名称
FOPTYKH --手术医生所在科室编号   不能为空
)
VALUES
(
'{0}' ,
{1},
'{2}' ,
{3},
'{4}' ,
'{5}',
'{6}' ,
'{7}',
'{8}' ,
'{9}',
'{10}' ,
'{11}',
'{12}' ,
'{13}',
'{14}' ,
{15},
'{16}' ,
'{17}',
'{18}' ,
'{19}',
'{20}' ,
'{21}',
'{22}' ,
'{23}',
'{24}' ,
'{25}',
'{26}' ,
'{27}'
)";
            #endregion
            string MarcKind_Code = string.Empty;
            string MarcKind_Name = string.Empty;
            //Neusoft.FrameWork.Models.NeuObject info = this.constMana.GetConstant("CASEANESTYPE", obj.MarcKind);
            Neusoft.FrameWork.Models.NeuObject info = this.constMana.GetConstant("ANESTYPE", obj.MarcKind);
            if (info != null && info.ID != "")
            {
                if (info.Memo != "" && info.Memo.ToUpper() != "TRUE")
                {
                    MarcKind_Code = info.Memo;
                    MarcKind_Name = info.Name;
                }
                else
                {
                    MarcKind_Code = obj.MarcKind;
                    MarcKind_Name = info.Name;
                }
            }
            else
            {
                MarcKind_Code = obj.MarcKind;
                MarcKind_Name = info.Name;
            }

            //新增切口，0类分为01，02,03
            string NickKind_Code = string.Empty;
            string NickKind_Name = string.Empty;
            //Neusoft.FrameWork.Models.NeuObject info = this.constMana.GetConstant("INCITYPE", obj.NickKind);
            //if(info!=null&&info.ID!="")
            //{
            //    if(info.Memo!=""&&info.Memo!="true")
            //    {
            //        NickKind_Code=info.Memo;
            //        NickKind_Name = info.Name;
            //    }
            //    else
            //    {
            //        NickKind_Code=obj.NickKind;
            //        NickKind_Name=this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCITYPE, obj.NickKind).Name;
            //    }
            if (obj.NickKind==null ||obj.NickKind.Trim().ToString()=="" || obj.NickKind.ToString() == "01" || obj.NickKind.ToString() == "02" || obj.NickKind.ToString() == "03")
            {
                NickKind_Code = "0";
                NickKind_Name = "0类";
            }
            else
            {
                NickKind_Code = obj.NickKind;
                NickKind_Name = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCITYPE, obj.NickKind).Name;
            }

            // {35CE6F51-8F7F-4ea5-89A7-EFA71A1A1309}
            //首页手术操作项中，若医生填写切口为“0类”愈合等级填“其他”导入省病案为：切口“0类”愈合为空不填。
            if (NickKind_Code == "0" && obj.CicaKind.ToString()=="4")
            {
                obj.CicaKind = "";
            }


            string checkDateTypeCode = string.Empty;
            string checkDateTypeName = string.Empty;

            if (obj.OperationKind == "1")
            {
                checkDateTypeCode = "1";
                checkDateTypeName = "是";
            }
            else
            {
                checkDateTypeCode = "0";
                checkDateTypeName = "否";
            }

            // {89C4E8A4-DA2B-401f-A0A9-524D7B2D25DF}  附加手术标记
            string addFlag = string.Empty;
            if (obj.AddFlag == "1")
            {
                addFlag = "1";
            }
            else
            {
                addFlag = "0";
            }
            //根据术者编码获取术者科室 若获取不到 再按一助获取一助科室
            Neusoft.HISFC.BizLogic.Manager.Person perMana = new Neusoft.HISFC.BizLogic.Manager.Person();
            Neusoft.HISFC.Models.Base.Employee empl = perMana.GetPersonByID(obj.FirDoctInfo.ID.PadLeft(6, '0'));
            if (empl == null || empl.Dept.ID == "")
            {
                ArrayList peral = new ArrayList();
                peral = perMana.GetPersonByName(obj.FirDoctInfo.Name);
                if (peral != null && peral.Count > 0)
                {
                    empl = peral[0] as Neusoft.HISFC.Models.Base.Employee;
                }
                else
                {
                    empl = new Neusoft.HISFC.Models.Base.Employee();
                    empl = perMana.GetPersonByID(obj.SecDoctInfo.ID.PadLeft(6, '0'));
                    if (empl == null || empl.Dept.ID == "")
                    {
                        ArrayList peral1 = new ArrayList();
                        peral1 = perMana.GetPersonByName(obj.SecDoctInfo.Name);
                        if (peral != null && peral1.Count > 0)
                        {
                            empl = peral1[0] as Neusoft.HISFC.Models.Base.Employee;
                        }
                        else
                        {
                            empl = new Neusoft.HISFC.Models.Base.Employee();
                        }
                    }
                }
            }
            else
            {
                if (obj.FirDoctInfo.Name == null || obj.FirDoctInfo.Name == "")
                {
                    obj.FirDoctInfo.Name = empl.Name;
                }
            }
            //如果这个字段为空 在省厅病案处保存时提示：null，导致保存失败，现在是在为空先做下面的处理。
            if (obj.FirDoctInfo.Name == null || obj.FirDoctInfo.Name.Trim() == "")//
            {
                obj.FirDoctInfo.Name = "1";
            }
            if (obj.FirDoctInfo.ID == null || obj.FirDoctInfo.ID.Trim() == "")//
            {
                obj.FirDoctInfo.ID = "1";
            }
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            string opLevelId = string.Empty;
            string opLevelName = string.Empty;
            Neusoft.HISFC.Models.Base.Const levelInfo = new Neusoft.HISFC.Models.Base.Const();
            levelInfo = function.ConverOperationL(obj.OperationInfo.ID);//从省厅病案 tOperate 表跟进手术编码查询级别 查询不到默认1级
            if (levelInfo.ID != null && levelInfo.ID.ToString().Trim() != "")
            {
                opLevelId = levelInfo.ID;
                opLevelName = levelInfo.Name;
            }
            else
            {
                //opLevelId = "1";
                //opLevelName = "一级";
                opLevelId = obj.FourDoctInfo.Name;
                opLevelName = function.GetOperationLevelNameByCode(obj.FourDoctInfo.Name);
            }
            
            try
            {
                sql = string.Format(sql, function.PatientNoChang(patientNO.Substring(function.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.HappenNO.ToString(),//手术次数
                                                 obj.OperationInfo.ID.ToString(),//手术码
                                                 function.ChangeCharacter(obj.OperationInfo.Memo.ToString()),//手术码对应名称
                    //this.ChangeCharacter(obj.OperationInfo.Name.ToString()),//手术码对应名称
                                                 function.ChangeDateTime(obj.OperationDate.ToShortDateString()),//手术日期
                                                 NickKind_Code,//obj.NickKind.ToString(), //切口编号
                                                 NickKind_Name,// this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCITYPE, obj.NickKind).Name.ToString(),  //切口
                                                 obj.CicaKind.ToString(),//愈合编号
                                                 this.constMana.GetConstant("CICATYPE", obj.CicaKind).Name.ToString(),  //愈合
                                                 function.ConverDoc(obj.FirDoctInfo.ID).ID,//手术医生编号
                                                 obj.FirDoctInfo.Name,//手术医生
                                                 MarcKind_Code, // obj.MarcKind.ToString(),//麻醉方式编号
                                                 MarcKind_Name, //this.constMana.GetConstant("CASEANESTYPE",obj.MarcKind).Name.ToString(),//麻醉方式
                                                 addFlag, //是否附加手术  "0",
                                                 function.ConverDoc(obj.SecDoctInfo.ID).ID,//I助编号
                                                 //function.ConverDoc(obj.SecDoctInfo.ID).Name, // I助姓名
                                                 obj.SecDoctInfo.Name,//II助姓名
                                                 function.ConverDoc(obj.ThrDoctInfo.ID).ID,//II助编号
                                                 //function.ConverDoc(obj.ThrDoctInfo.ID).Name,//II助姓名
                                                 obj.ThrDoctInfo.Name,//II助姓名
                                                 function.ConverDoc(obj.NarcDoctInfo.ID).ID,//麻醉医生编号
                                                 //function.ConverDoc(obj.NarcDoctInfo.ID).Name, //麻醉医生
                                                 obj.NarcDoctInfo.Name,//之前没有编号时传不过去
                                                 checkDateTypeCode,//择期手术编号1是，0否
                                                 checkDateTypeName,//择期手术名称1是，0否
                                                 //obj.FourDoctInfo.Name,//手术级别编号
                                                 //this.constMana.GetConstant("CASELEVEL", obj.FourDoctInfo.Name).Name.ToString(),//手术级别
                                                 //function.ConverOperationL(obj.OperationInfo.ID).ID,//手术级别编号
                                                 //function.ConverOperationL(obj.OperationInfo.ID).Name,//手术级别
                                                 opLevelId,//手术级别编号
                                                 opLevelName,//手术级别
                                                 empl.Dept.Name,//手术医生所在科室名称
                                                 empl.Dept.ID //手术医生所在科室编码
                                                 );
            }
            catch(Exception e)
            {
                this.Err = "赋值失败! 请联系信息科! " + e.ToString();
                return -1;
            }
            //ReadSQL(sql);
            return server.ExecuteNonQuery(sql);

        }

        #endregion

        #region tBabyCard(HIS_BA5)

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int DeleteHISBA5(string inpatientNO, int times)
        {
            string strSQL = @"delete from tBabyCard where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO, times.ToString());

            //ReadSQL(strSQL);

            return server.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// insert HIS_BA5 --妇婴信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">妇婴卡信息</param>
        /// <returns></returns>
        public int insertHisBa5Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Baby obj)
        {


            string sql = @"insert into tBabyCard (FPRN,FTIMES ,FBABYNUM ,FNAME ,FBABYSEXBH ,FBABYSEX ,FTZ ,FRESULTBH ,FRESULT ,
                  FZGBH ,FZG ,FBABYQJ,FBABYSUC ,FHXBH ,FHX) 
                                         values ('{0}',{1},{2},'{3}','{4}','{5}',{6},'{7}','{8}',
                  '{9}','{10}','{11}','{12}','{13}','{14}')";



            if (obj.SexCode.ToString() == "M" || obj.SexCode.ToString() == "1")
            {
                obj.SexCode = "1";
                obj.Infect.Memo = "男";
            }
            else if (obj.SexCode.ToString() == "F" || obj.SexCode.ToString() == "2")
            {
                obj.SexCode = "2";
                obj.Infect.Memo = "女";
            }
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql, function.PatientNoChang(patientNO.Substring(function.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 obj.HappenNum.ToString(),//婴儿序号
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.SexCode.ToString(),//婴儿性别编号
                                                 obj.Infect.Memo,//婴儿性别
                                                 obj.Weight.ToString(),//婴儿体重
                                                 obj.BirthEnd.ToString(), //分娩结果编号
                                                 this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CHILDBEARINGRESULT, obj.BirthEnd).Name.ToString(),  //分娩结果
                                                 obj.BabyState,//转归编号
                                                 this.constMana.GetConstant("BABYZG", obj.BabyState).Name.ToString(),  //转归
                                                 obj.SalvNum.ToString(),//抢救次数
                                                 obj.SuccNum.ToString(),//婴儿抢救成功次数
                                                 obj.Breath,//呼吸编号
                                                 this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.BREATHSTATE, obj.BirthEnd).Name.ToString() //呼吸
                                                 );
            }
            catch(Exception e)
            {
                this.Err = "赋值失败! 请联系信息科! " + e.ToString();
                return -1;
            }
            //ReadSQL(sql);
            return server.ExecuteNonQuery(sql);
        }
        #endregion

        #region tKnubCard(HIS_BA6)

        /// <summary>
        /// 删除HISBA6
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <param name="times">住院次数</param>
        /// <returns></returns>
        public int DeleteHISBA6(string inpatientNO, int times)
        {
            string strSQL = @"DELETE FROM tKnubCard WHERE FPRN='{0}' AND FTIMES={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            //ReadSQL(strSQL);
            return server.ExecuteNonQuery(strSQL);

        }

        /// <summary>
        /// HIS_BA6  --病人肿瘤信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">肿瘤卡信息</param>
        /// <returns></returns>
        public int InsertHISBA6Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Tumour obj)
        {
            #region sql
            string sql = @"INSERT INTO tKnubCard
(
FPRN,--0
FTIMES,
FFLFSBH,
FFLFS,
FFLCXBH,
FFLCX,--5
FFLZZBH,
FFLZZ,
FYJY,
FYCS,
FYTS,--10
FYRQ1,
FYRQ2,
FQJY,
FQCS,
FQTS,--15
FQRQ1,
FQRQ2,
FZNAME,
FZJY,
FZCS,--20
FZTS,
FZRQ1,
FZRQ2,
FHLFSBH,
FHLFS,--25
FHLFFBH,
FHLFF,
FQTYPE,
FQT,
FQN,--30
FQM,
FQALL,
FQALLBH--33
)
VALUES
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}',
'{28}',
'{29}',
'{30}',
'{31}',
'{32}',
'{33}'
)";
            #endregion
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql,
                                                 function.PatientNoChang(patientNO.Substring(function.PatientNoSubstr())),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 obj.Rmodeid,//放疗方式编号
                                                 this.constMana.GetConstant("RADIATETYPE", obj.Rmodeid).Name,//放疗方式
                                                 obj.Rprocessid,//放疗程序编号
                                                 this.constMana.GetConstant("RADIATEPERIOD", obj.Rprocessid).Name,//放疗程序
                                                 obj.Rdeviceid,//放疗装置编号
                                                 this.constMana.GetConstant("RADIATEDEVICE", obj.Rdeviceid).Name,//放疗装置
                                                 obj.Gy1,//1.原发灶剂量
                                                 obj.Time1,//原发灶次数
                                                 obj.Day1,//原发灶天数
                                                 function.ChangeDateTime(obj.BeginDate1.ToString()),//原发灶开始日期
                                                 function.ChangeDateTime(obj.EndDate1.ToString()),//原发灶结束时间
                                                 obj.Gy2,//2.区域淋巴结剂量
                                                 obj.Time2,//区域淋巴结次数
                                                 obj.Day2,//区域淋巴结天数
                                                 function.ChangeDateTime(obj.BeginDate2.ToString()),//区域淋巴结开始时间
                                                 function.ChangeDateTime(obj.EndDate2.ToString()),//区域淋巴结结束时间
                                                 obj.Position,//3.转移灶名称
                                                 obj.Gy3,//3.转移灶剂量
                                                 obj.Time3,//转移灶次数
                                                 obj.Day3,//转移灶天数
                                                 function.ChangeDateTime(obj.BeginDate3.ToString()),//转移灶开始时间
                                                 function.ChangeDateTime(obj.EndDate3.ToString()),//转移灶结束时间
                                                 obj.Cmodeid,//化疗方式编号
                                                 this.constMana.GetConstant("CHEMOTHERAPY", obj.Cmodeid).Name,//化疗方式
                                                 obj.Cmethod,//化疗方法编号
                                                 this.constMana.GetConstant("CHEMOTHERAPYWAY", obj.Cmethod).Name,//化疗方法
                                                 obj.Tumour_Type,//肿瘤病例分类
                                                 obj.Tumour_T,//原发肿瘤T
                                                 obj.Tumour_N,//淋巴转移N
                                                 obj.Tumour_M,//远程转移M
                                                 this.constMana.GetConstant("CASETUMOURSTAGE", obj.Tumour_Stage).Name,//分期
                                                 obj.Tumour_Stage//分期编码
                                                 );
            }
            catch(Exception e)
            {
                this.Err = "赋值失败! 请联系信息科! " + e.ToString();
                return -1;
            }

            sql = sql.Replace("'NULL'", "NULL");
            //ReadSQL(sql);
            return server.ExecuteNonQuery(sql);

        }

        /// <summary>
        /// 更改HIS_BA6  FYRQ1 FYRQ2为空
        /// </summary>
        /// <returns></returns>
        public int UpdateHISBA6FYRQ(string FPRN)
        {
            // t.FQRQ1 t.FQRQ2  t.FZRQ1 t.FZRQ2  
            string strSQL = @"update tKnubCard set FYJY=null, FYCS=null, FYTS=null ,FYRQ1=null ,FYRQ2=null where FPRN='{0}'";
            try
            {
                strSQL = string.Format(strSQL, function.PatientNoChang(FPRN.PadLeft(10, '0').Substring(function.PatientNoSubstr())));
            }
            catch
            {
                return -1;
            }
            //ReadSQL(strSQL);

            return server.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// 更改HIS_BA6  t.FQRQ1 t.FQRQ2  为空
        /// </summary>
        /// <returns></returns>
        public int UpdateHISBA6FQRQ(string FPRN)
        {
            //t.FZRQ1 t.FZRQ2  
            string strSQL = @"update tKnubCard set FQJY=null,FQCS=null ,FQTS=null,FQRQ1=null ,FQRQ2=null where FPRN='{0}'";
            try
            {
                // strSQL = string.Format(strSQL, this.PatientNoChang(FPRN.Substring(this.PatientNoSubstr())));
                strSQL = string.Format(strSQL, function.PatientNoChang(FPRN.PadLeft(10, '0').Substring(function.PatientNoSubstr())));



            }
            catch
            {
                return -1;
            }
            //ReadSQL(strSQL);

            return server.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// 更改HIS_BA6  t.FZRQ1 t.FZRQ2为空
        /// </summary>
        /// <returns></returns>
        public int UpdateHISBA6FZRQ(string FPRN)
        {
            string strSQL = @"update tKnubCard set FZJY=null, FZCS=null, FZTS=null ,FZRQ1=null ,FZRQ2=null where FPRN='{0}'";
            try
            {
                strSQL = string.Format(strSQL, function.PatientNoChang(FPRN.PadLeft(10, '0').Substring(function.PatientNoSubstr())));
            }
            catch
            {
                return -1;
            }
            //ReadSQL(strSQL);
            return server.ExecuteNonQuery(strSQL);

        }

        /// <summary>
        /// 更改HIS_BA1  Ftimes+1 住院次数+1  用于处理住院次数错误提示
        /// </summary>
        /// <returns></returns>
        public int DelectHISBA1Ftimes(string FPRN,string FZYID)
        {
            string strSQL =
                            @"delete from HIS_BA1 
                               where fprn='{0}'
                               and fzyid='{1}';";
            try
            {
                strSQL = string.Format(strSQL, function.PatientNoChang(FPRN.PadLeft(10, '0').Substring(function.PatientNoSubstr())), FZYID);
            }
            catch
            {
                return -1;
            }
            //ReadSQL(strSQL);
            return server.ExecuteNonQuery(strSQL);

        }
        #endregion

        #region tKnubHl(HIS_BA7)

        /// <summary>
        /// 删除HISBA7
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <param name="times">住院次数</param>
        /// <returns></returns>
        public int DeleteHISBA7(string inpatientNO, int times)
        {
            string strSQL = @"DELETE FROM tKnubHl WHERE FPRN='{0}' AND FTIMES={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            //ReadSQL(strSQL);
            return server.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// HIS_BA7  --肿瘤化疗记录
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">肿瘤卡肿瘤化疗信息</param>
        /// <returns></returns>
        public int InsertHISBA7Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.TumourDetail obj)
        {
            #region sql
            string sql = @"INSERT INTO tKnubHl
(
FPRN,
FTIMES,
FHLRQ1,
FHLRQ2,
FHLDRUG,
FHLPROC,
FHLLXBH,
FHLLX
)
VALUES
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}'
)";
            #endregion
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                Neusoft.HISFC.BizLogic.Manager.Constant con = new Neusoft.HISFC.BizLogic.Manager.Constant();
                ArrayList UnitList = con.GetList(Neusoft.HISFC.Models.Base.EnumConstant.DOSEUNIT);
                UnitListHelper.ArrayObject = UnitList;
                sql = string.Format(sql,
                                                 function.PatientNoChang(patientNO.Substring(function.PatientNoSubstr())),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 function.ChangeDateTime(obj.CureDate.ToString()),//化疗起始日期
                                                 function.ChangeDateTime(obj.OperInfo.OperTime.ToString()),//化疗终止日期
                    //  obj.DrugInfo.Name + "(" + obj.Qty.ToString() + obj.Unit.ToString() + ")",//化疗药物名称及剂量
                                                 obj.DrugInfo.Name + "(" + obj.Qty.ToString() + UnitListHelper.GetName(obj.Unit) + ")",
                                                 obj.Period.ToString(),//化疗疗程
                                                 obj.Result.ToString(),//疗效编号
                                                 "" //疗效
                                                 );
            }
            catch(Exception e)
            {
                this.Err = "赋值失败! 请联系信息科! " + e.ToString();
                return -1;
            }
            sql = sql.Replace("'NULL'", "NULL");
            //ReadSQL(sql);
            return server.ExecuteNonQuery(sql);

        }

        #endregion

        #region 插入病人诊断附属码
        /// <summary>
        /// 插入病人诊断附属码
        /// </summary>
        /// <returns></returns>
        public int InsertTDiagnoseAdd(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Diagnose obj)
        {
            if (obj.DiagInfo.DiagType.ID == "16")
            {
                obj.DiagInfo.DiagType.ID = "f";
            }
            string sql = @"insert into TDiagnoseAdd (fprn,ftimes,fzdlx,ficdm,ffjicdm,ffjjbname,ffrybqbh,ffrybq) 
values ('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}')";
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql,
                                                 function.PatientNoChang(patientNO.Substring(function.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),
                                                 obj.DiagInfo.DiagType.ID,//对照
                                                 obj.DiagInfo.ICD10.ID,
                                                 obj.DiagInfo.ICDF10.ID,//附属编码
                                                 obj.DiagInfo.ICDF10.Name,//附属编码名称
                                                 obj.DiagOutState,
                                                 obj.Memo);
            }
            catch (Exception e)
            {
                this.Err = "赋值失败! 请联系信息科! " + e.ToString();
                return -1;
            }

            return server.ExecuteNonQuery(sql);
        }

        #endregion

        #region  删除非主要诊断 附属码

        /// <summary>
        /// 删除非主要诊断 附属码
        /// </summary>
        /// <returns></returns>
        public int DeleteTDiagnoseAdd(string inpatientNO, int times)
        {
            string strSQL = @"delete from TDiagnoseAdd where fprn='{0}'and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            //ReadSQL(strSQL);
            return server.ExecuteNonQuery(strSQL);
        }

        #endregion 

        #region 错误数据处理
        /// <summary>
        /// 查询错误数据
        /// </summary>
        /// <param name="Inpatientno"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int QueryUploadInfo(string Inpatientno,string patientno, ref DataSet ds)
        {
            string sql = "";
            sql = @"select 
                    p.fprn as 住院号,
                    p.ftimes as 住院次数,
                    p.frydate as 入院日期,
                    p.fcydate as 出院日期,
                    (select count(s.ficdm) from tDiagnose s where s.fprn=p.fprn and s.ftimes=p.ftimes and s.fzdlx not in ('f','s')) as 诊断信息数,
                    (select count(o.fopcode) from tOperation o where o.fprn=p.fprn and o.ftimes=p.ftimes) as 手术信息数
                    from tpatientvisit p
                    where p.fzyid='{0}'
                    and p.fprn='{1}'
                    ";
            sql = string.Format(sql, Inpatientno, patientno);
            string ss = "";
            //server.ExecuteQueryReturnOne(sql, ref ss);
            return server.QueryDataSet(sql, ref ds);
        }

        public int GetPatientByInpatientNo(string Inpatientno, ref string Patientno, ref int Times)
        {
            string sql = "";
            sql = @"select  
                    p.fprn as 住院号,
                    p.ftimes as 住院次数
                    from tpatientvisit p
                    where p.fzyid='{0}'";
            sql = string.Format(sql, Inpatientno);
            DataSet ds = new DataSet();
            if (server.QueryDataSet(sql, ref ds) < 0)
            {
                return -1;
            }
            if (ds.Tables[0].Rows.Count <= 0)
            {
                this.Err = "没读到！";
                return -1;
            }
            try
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Patientno = dr[0].ToString();
                    Times = Neusoft.FrameWork.Function.NConvert.ToInt32(dr[1].ToString());
                    break;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 删除错误数据
        /// </summary>
        /// <param name="Patientno">住院号</param>
        /// <param name="Times">Times</param>
        /// <returns></returns>
        public int DeletePatientByInpatientNo(string inpatientNO,string Patientno, int Times)
        {
            int rest = -1;
            rest = DelectHISBA1Ftimes(Patientno, Times.ToString());
            if (rest <0)
            {
                this.Err = "删除表HIS_BA1失败！";
                return -1;
            }
            rest = DeleteHISBA1ByFzyid(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表TPATIENTvisit失败！";
                return -1;
            }

            //rest = DeleteHISBA1ByFzyid(inpatientNO);
            //if (rest < 0)
            //{
            //    this.Err = "删除表TPATIENTvisit失败！";
            //    return -1;
            //}
            rest = DeleteHISBA2(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表tSwitchKs失败！";
                return -1;
            }
            rest = DeleteHISBA3(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表tDiagnose失败！";
                return -1;
            }
            rest = DeleteHISBA4(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表tOperation失败！";
                return -1;
            }
            rest = DeleteHISBA5(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表tBabyCard失败！";
                return -1;
            }
            rest = DeleteHISBA6(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表tKnubCard失败！";
                return -1;
            }
            rest = DeleteHISBA7(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表tKnubHl失败！";
                return -1;
            }

            rest = DeleteTDiagnoseAdd(Patientno, Times);
            if (rest < 0)
            {
                this.Err = "删除表TDiagnoseAdd失败！";
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 获取上传错误信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int GetUploadErro(string inpatientNO, int times)
        {
            return -1;
        }

        #endregion
    }
}
