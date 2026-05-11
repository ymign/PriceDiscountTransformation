using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Xml;



namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDong
{
    /// <summary>
    /// 肿瘤医院病历首页上传业务层,
    /// 独立连接SQLSERVER数据库,连接字符串都是写死的
    /// [2008/04/08]
    /// /// <修改记录
    ///		修改人='新病案系统接口'
    ///		修改时间='2009-5-30'
    ///		修改目的='新系统接口'
    ///		修改描述='何荣'
    ///  />
    /// </summary>
    /// </summary>
    public class UploadBAInterface : Neusoft.FrameWork.Management.Database
    {
        
        //private SqlConnection conn = new SqlConnection(@"Data Source=<SQL_SERVER_HOST>;Initial Catalog=<DATABASE>;User ID=<USER>;Password=<PASSWORD>;");
        //private SqlConnection conn = new SqlConnection(@"Data Source=<SQL_SERVER_HOST>;Initial Catalog=<DATABASE>;User ID=<USER>;Password=<PASSWORD>;");
       //private SqlConnection conn = new SqlConnection(@"Data Source=localhost;Initial Catalog=ba_java;Integrated Security=True;");
        //private SqlConnection conn = new SqlConnection(@"Data Source=<SQL_SERVER_HOST>;Initial Catalog=<DATABASE>;User ID=<USER>;Password=<PASSWORD>");
        //private SqlConnection conn = new SqlConnection(@"Data Source=<SQL_SERVER_HOST>;Initial Catalog=<DATABASE>;User ID=<USER>;Password=<PASSWORD>");
        private SqlConnection conn = new SqlConnection();

        public Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew.SQLServerManager server = null;//new SQLServerManager();//数据链接实体
        public Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew.Function function = null;
            
        private SqlCommand cmd = new SqlCommand();
        private SqlTransaction transaction = null;
        private SqlDataReader reader;

        private InterfaceFunction fun = new InterfaceFunction();

        private string profileName = System.Windows.Forms.Application.StartupPath + @".\Profile\CaseDataBase.xml";//病案数据库连接设置;



        Neusoft.HISFC.BizLogic.Manager.Constant constMana = new Neusoft.HISFC.BizLogic.Manager.Constant();
        Neusoft.HISFC.BizLogic.Manager.Department deptMana = new Neusoft.HISFC.BizLogic.Manager.Department();
        Neusoft.HISFC.BizLogic.HealthRecord.Base baseDml = new Neusoft.HISFC.BizLogic.HealthRecord.Base();
        string err = "";
        //单位列表 ADD BY ZHY
        Neusoft.FrameWork.Public.ObjectHelper UnitListHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        public string Err1
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
        /// 病案首页上传接口（广东病案3.0系统）
        /// </summary>
        public UploadBAInterface()
        {
            //if (!this.IsOpen())
            //{
            //    throw new Exception("连接数据库失败");
            //}
            this.conn.ConnectionString = this.GetConnectString();
            this.conn.Open();
            this.transaction = this.conn.BeginTransaction();
            this.cmd.Connection = this.conn;
            this.cmd.Transaction = transaction;
            function = new Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew.Function();
            SetServer();
            CreatFile();
        }

        public void SetServer()
        {
            if (server == null)
            {
                this.server = new Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew.SQLServerManager();
            }
            if (function.Server == null)
            {
                function.Server = this.server;
            }
        }


        public void Commit()
        {
            this.transaction.Commit();
        }

        public void Rollback()
        {
            this.transaction.Rollback();
        }

        public SqlConnection Connection
        {
            get
            {
                this.IsOpen();
                return this.conn;
            }
        }

        public bool IsOpen()
        {
            try
            {
                if (this.conn != null && this.conn.State == ConnectionState.Closed)
                {
                    this.conn.Open();
                    this.transaction = this.conn.BeginTransaction();
                    this.cmd.Connection = this.conn;
                    this.cmd.Transaction = this.transaction;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        /// <summary>
        /// 获得连接串
        /// </summary>
        /// <returns></returns>
        public string GetConnectString()
        {
            string dbInstance = "";
            string DataBaseName = "";
            string userName = "";
            string password = "";
            string connString = "";

            if (!System.IO.File.Exists(profileName))
            {
                Neusoft.FrameWork.Xml.XML myXml = new Neusoft.FrameWork.Xml.XML();
                XmlDocument doc = new XmlDocument();
                XmlElement root;
                root = myXml.CreateRootElement(doc, "SqlServerConnectForHis5.0", "1.0");

                XmlElement dbName = myXml.AddXmlNode(doc, root, "设置", "");

                myXml.AddNodeAttibute(dbName, "数据库实例名", "");
                myXml.AddNodeAttibute(dbName, "数据库名", "");
                myXml.AddNodeAttibute(dbName, "用户名", "");
                myXml.AddNodeAttibute(dbName, "密码", "");

                try
                {
                    StreamWriter sr = new StreamWriter(profileName, false, System.Text.Encoding.Default);
                    string cleandown = doc.OuterXml;
                    sr.Write(cleandown);
                    sr.Close();
                }
                catch (Exception ex)
                {
                    this.Err = "创建医保连接服务配置出错!" + ex.Message;
                    this.ErrCode = "-1";
                    this.WriteErr();
                    return "";
                }

                return "";
            }
            else
            {
                XmlDocument doc = new XmlDocument();

                try
                {
                    StreamReader sr = new StreamReader(profileName, System.Text.Encoding.Default);
                    string cleandown = sr.ReadToEnd();
                    doc.LoadXml(cleandown);
                    sr.Close();
                }
                catch { return ""; }

                XmlNode node = doc.SelectSingleNode("//设置");

                try
                {

                    dbInstance = node.Attributes["数据库实例名"].Value.ToString();
                    DataBaseName = node.Attributes["数据库名"].Value.ToString();
                    userName = node.Attributes["用户名"].Value.ToString();
                    password = node.Attributes["密码"].Value.ToString();
                }
                catch { return ""; }

                connString = "packet size=4096;user id=" + userName + ";data source=" + dbInstance + ";pers" +
                    "ist security info=True;initial catalog=" + DataBaseName + ";password=" + password;
            }

            return connString;
        }

        #region 上传

        #region HIS_BA1

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int DeleteHISBA1(string inpatientNO,int times)
        {
            string strSQL = @"delete from HIS_BA1 where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// 删除中间表数据 by 住院流水号
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns></returns>
        public int DeleteHISBA1ByFzyid(string inpatientNO)
        {
            string strSQL = @"delete from HIS_BA1 WHERE FZYID='{0}' ";

            strSQL = string.Format(strSQL, inpatientNO);

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// 更改fzkdate  fzkrq 为空
        /// </summary>
        /// <param name="fprn"></param>
        /// <returns></returns>
        public int UpdateHISBA1Fzkdate(string fprn)
        {
            string strSQL = @"update HIS_BA1 set  fzkdate=null  where FPRN='{0}' and fzkdate<'2000-1-1 00:00:00'";
            try
            {
                strSQL = string.Format(strSQL, fprn.Substring(this.PatientNoSubstr()));
            }
            catch
            {
            }
            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        #endregion

        /// <summary>
        /// 诊断上传对照
        /// </summary>
        /// <param name="alICD"></param>
        private void ConverICDType(System.Collections.ArrayList alICD)
        {

            for (int i = alICD.Count - 1; i >= 0; i--)
            {
                Neusoft.HISFC.Models.HealthRecord.Diagnose obj = alICD[i] as Neusoft.HISFC.Models.HealthRecord.Diagnose;
                if (obj.DiagInfo.DiagType.ID == "10" || obj.DiagInfo.DiagType.ID == "11" || obj.DiagInfo.DiagType.ID == "6")
                {
                    alICD.Remove(obj);
                }
                else
                {
                    obj.DiagInfo.DiagType.ID = this.constMana.GetConstant("CaseDiagnose", obj.DiagInfo.DiagType.ID).Name;

                    if (obj.DiagInfo.DiagType.ID.Trim().Equals(string.Empty))
                    {
                        obj.DiagInfo.DiagType.ID = "2";
                    }
                }
            }
        }

        /// <summary>
        /// 取对照科室
        /// </summary>
        /// <param name="deptCode"></param>
        private string ConverDept(string deptCode)
        {
            string strReturn = this.constMana.GetConstant("DEPTUPLOAD", deptCode).Memo;

            if (strReturn == "")
            {
                return deptCode;
            }
            else
            {
                return strReturn;
            }
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
        public int InsertPatientInfoBA1(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose,bool isMetCasBase)
        {
            //this.IsOpen();

            //string sql = @"insert into ba1 (prn,name,sex,birthday,birthpl, idcard, native, nation) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
            //this.cmd.CommandText = string.Format(sql,
            //                                     patientInfo.PatientInfo.PID.PatientNO.Substring(3),
            //                                     patientInfo.PatientInfo.Name,
            //                                     patientInfo.PatientInfo.Sex.Name.Equals("男") ? "1": "2",//对照
            //                                     patientInfo.PatientInfo.Birthday.ToShortDateString().Replace('-', '/'),
            //                                     patientInfo.PatientInfo.AreaCode,
            //                                     patientInfo.PatientInfo.IDCard,
            //                                     patientInfo.PatientInfo.Country.Name,
            //                                     patientInfo.PatientInfo.Nationality.Name);

            //if (this.cmd.ExecuteNonQuery() <= 0)
            //{
            //    return -1;
            //}

            string strSQL = this.fun.GetInsertHISBA1SQL(b, alFee, alChangeDepe, alDose,isMetCasBase);

            if (strSQL == null || strSQL == "")
            {
                return -1;
            }
            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int DeleteHISBA3(string inpatientNO, int times)
        {
            string strSQL = @"delete from HIS_BA3 where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// HIS_BA3  --病人诊断信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertHISBA3(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Diagnose obj)
        {
            if (obj.DiagInfo.DiagType.ID == "16")
            {
                obj.DiagInfo.DiagType.ID = "f";
            }
            string sql = @"insert into HIS_BA3 (fprn,ftimes,FZDLX, FICDVersion, FICDM, FJBNAME,FZLJGBH,FZLJG) values ('{0}',{1},'{2}',{3},'{4}','{5}','{6}','{7}')";
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql,
                                                 this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),
                                                 //patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 obj.DiagInfo.DiagType.ID,//对照
                                                 "11",//ICD版本号
                                                 obj.DiagInfo.ICD10.ID,
                                                 obj.DiagInfo.Name,
                                                 obj.DiagOutState,
                                                 "");
            }
            catch
            {
            }

            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// HIS_BA4  病人手术信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="alOperations"></param>
        /// <returns></returns>
        public int InsertPatientOperationInfo(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, System.Collections.ArrayList alOperations)
        {
            this.IsOpen();

            this.cmd.CommandText = string.Empty;
            string strsql = @"insert into tOperation (FPRN,FTIMES,FNAME, FOPTIMES,FOPCODE,FOP,FOPDATE,FQIEKOUBH,FQIEKOU,FYUHEBH,FYUHE,FDOCBH,FDOCNAME,FMAZUIBH,FMAZUI,FIFFSOP,FOPDOCT1BH,FOPDOCT1,FOPDOCT2BH,FOPDOCT2,FMZDOCTBH,FMZDOCT) 
values ('{0}',{1},'{2}',{3},'{4}','{5}','{6}', '{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},'{16}','{17}','{18}','{19}','{20}','{21}')";
            string temp = string.Empty;

            int times = 0;

            for (int i = 0; i < alOperations.Count; i++)
            //foreach (Neusoft.HISFC.Models.HealthRecord.OperationDetail obj in alOperations)
            {
                Neusoft.HISFC.Models.HealthRecord.OperationDetail obj = alOperations[i] as Neusoft.HISFC.Models.HealthRecord.OperationDetail;
                this.cmd.CommandText = string.Empty;
                temp = strsql;
                string StatFlag = "";
                if (obj.StatFlag == "1")
                {
                    StatFlag = "1";
                }
                else
                {
                    StatFlag = "0";
                }

                if (StatFlag == "0")
                {
                    times++;
                }
                if ( obj.OperationInfo.ID.Substring ( 0, 2 ) != "01" || obj.OperationInfo.ID.Substring ( 0, 2 ) != "02" || obj.OperationInfo.ID.Substring ( 0, 2 ) != "03" || obj.OperationInfo.ID.Substring ( 0, 2 ) != "04" )
                {
                    obj.OperationInfo.ID = obj.OperationInfo.ID.PadRight ( obj.OperationInfo.ID.Length + 1, ' ' );
                }
                //**************
                //由于手术编码有只有“32”这个的格式，但新病案系统中不知道要补多少空格，这边不处理Fuck
                //**************

                if ( obj.FirDoctInfo.ID != "" )
                {
                    obj.FirDoctInfo.ID = "T" + obj.FirDoctInfo.ID.Substring ( obj.FirDoctInfo.ID.Length - 4 ) + "*1";
                }
                if ( obj.FourDoctInfo.ID != "" )
                {
                    obj.FourDoctInfo.ID = "T" + obj.FourDoctInfo.ID.Substring ( obj.FourDoctInfo.ID.Length - 4 ) + "*1";
                }
                if ( obj.SecDoctInfo.ID != "" )
                {
                    obj.SecDoctInfo.ID = "T" + obj.SecDoctInfo.ID.Substring ( obj.SecDoctInfo.ID.Length - 4 ) + "*1";
                }
                

                string str = string.Format(temp,
                                                     patientInfo.PatientInfo.PID.PatientNO.Substring(3),
                                                     patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                     patientInfo.PatientInfo.Name,
                                                     times.ToString()/*手术次数*/,
                                                     obj.OperationInfo.ID,
                                                     obj.OperationInfo.Name,//手术码名称
                                                     obj.OperationDate.ToShortDateString().Replace('-', '/'),
                                                     obj.NickKind/*切口*/,
                                                     this.constMana.GetConstant ( "INCITYPE", obj.NickKind ).Name,//切口名称
                                                     obj.CicaKind/*愈合*/,
                                                     this.constMana.GetConstant ( "CICATYPE", obj.CicaKind ).Name,//愈合名称
                                                     obj.FirDoctInfo.ID,
                                                     obj.FirDoctInfo.Name,
                                                     obj.MarcKind,//麻醉方式
                                                     this.constMana.GetConstant ( "ANESTYPE", obj.MarcKind ).Name,//麻醉方式名称
                                                     StatFlag,/*是否附加手术*/
                                                     obj.FourDoctInfo.ID,
                                                     obj.FourDoctInfo.Name,
                                                     obj.SecDoctInfo.ID,
                                                     obj.SecDoctInfo.Name,
                                                     obj.NarcDoctInfo.ID,
                                                     obj.NarcDoctInfo.Name);

                this.ReadSQL(str);

                if (this.cmd.ExecuteNonQuery() <= 0)
                {
                    return -1;
                }
            }

            return 1;
        }

        /// <summary>
        /// HIS_BA6 肿瘤卡（放化疗信息）
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="Tumour"></param>
        /// <returns></returns>
        public int InsertPatientTumourInfo(Neusoft.HISFC.Models.HealthRecord.Base patientInfo,Neusoft.HISFC.Models.HealthRecord.Tumour Tumour)
        {
            this.IsOpen();
           // ArrayList tempchemoty = this.constMana.GetList ( "CHEMOTHERAPYWAY" );
           // for ( int i = 0; i <= tempchemoty.Count; i++ )
           // {
           //     Neusoft.FrameWork.Models.NeuObject obj = tempchemoty [i] as Neusoft.FrameWork.Models.NeuObject;
            if ( Tumour.Rdeviceid.Contains ( "钴" ) )
                {
                    Tumour.User03 = "1";
                }
                else if ( Tumour.Rdeviceid.Contains ( "直加" ) )
                {
                    Tumour.User03 = "2";
                }
                else if ( Tumour.Rdeviceid.Contains ( "X线" ) )
                {
                    Tumour.User03 = "3";
                }
                else if ( Tumour.Rdeviceid.Contains ( "后装" ) )
                {
                    Tumour.User03 = "4";
                }
          //  }

          //  ArrayList radivce = this.constMana.GetList ( "RADIATEDEVICE" );
          //  for ( int i = 0; i <= radivce.Count; i++ )
          //  {
          //      Neusoft.FrameWork.Models.NeuObject obj = radivce [i] as Neusoft.FrameWork.Models.NeuObject;
                if ( Tumour.Cmethod.Contains ( "全化" ) )
                {
                    Tumour.User02 = "1";
                }
                else if ( Tumour.Cmethod.Contains ( "半化" ) )
                {
                    Tumour.User02 = "2";
                }
                else if ( Tumour.Cmethod.Contains ( "A插管" ) )
                {
                    Tumour.User02 = "3";
                }
                else if ( Tumour.Cmethod.Contains ( "胸腔注" ) )
                {
                    Tumour.User02 = "4";
                }
                else if ( Tumour.Cmethod.Contains ( "髓注" ) )
                {
                    Tumour.User02 = "5";
                }
                else if ( Tumour.Cmethod.Contains ( "其他" ) )
                {
                    Tumour.User02 = "6";
                }
                else if ( Tumour.Cmethod.Contains ( "腹腔注" ) )
                {
                    Tumour.User02 = "7";
                }
        //    }


                string sql = @"insert into tKnubCard (fprn,ftimes,fflfsbh,fflfs,fflcxbh,fflcx,fflzzbh,fflzz,fyjy,fycs,fyts,fyrq1,fyrq2,fqjy,fqcs,fqts,fqrq1,fqrq2,fzname,fzjy,fzcs,fzts,fzrq1,fzrq2,fhlfsbh,fhlfs,fhlffbh,fhlff) 
values ('{0}',{1},'{2}','{3}','{4}','{5}','{6}', '{7}',{8},{9},{10},'{11}','{12}',{13},{14}, {15},'{16}','{17}','{18}',{19},{20},{21},'{22}','{23}','{24}','{25}','{26}','{27}')";
            this.cmd.CommandText = string.Format(sql,
                                                 patientInfo.PatientInfo.PID.PatientNO.Substring(3),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 Tumour.Rmodeid.TrimStart('0'),//
                                                 this.constMana.GetConstant ( "RADIATETYPE", Tumour.Rmodeid ).Name,
                                                 Tumour.Rprocessid.TrimStart ( '0' ),//
                                                 this.constMana.GetConstant ( "RADIATEPERIOD", Tumour.Rprocessid ).Name,
                                                 Tumour.User03,
                                                 Tumour.Rdeviceid,//
                                                 Tumour.Gy1.ToString(),
                                                 Tumour.Time1.ToString(),
                                                 Tumour.Day1.ToString(),
                                                 Tumour.BeginDate1.ToShortDateString(),                                                 
                                                 Tumour.EndDate1.ToShortDateString(),
                                                 Tumour.Gy2.ToString(),
                                                 Tumour.Time2.ToString(),
                                                 Tumour.Day2.ToString(),
                                                 Tumour.BeginDate2.ToShortDateString(),
                                                 Tumour.EndDate2.ToShortDateString ( ),
                                                 "",//转移灶名称，HIS系统中没有对应字段
                                                 Tumour.Gy3.ToString(),
                                                 Tumour.Time3.ToString(),
                                                 Tumour.Day3.ToString(),
                                                 Tumour.BeginDate3.ToShortDateString ( ),
                                                 Tumour.EndDate3.ToShortDateString ( ),
                                                 Tumour.Cmodeid.TrimStart ( '0' ),//
                                                 this.constMana.GetConstant ( "CHEMOTHERAPY", Tumour.Cmodeid ).Name,
                                                 Tumour.User02,
                                                 Tumour.Cmethod
                                                 );
            this.cmd.CommandText = this.cmd.CommandText.Replace ( "'0001-1-1'", "null" );
            this.cmd.CommandText = this.cmd.CommandText.Replace ( "'0001-01-01'", "null" );


            if (this.cmd.ExecuteNonQuery() <= 0)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 删除HISBA2
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <param name="times">住院次数</param>
        /// <returns></returns>
        public int DeleteHISBA2(string inpatientNO, int times)
        {
            string strSQL = @"DELETE FROM HIS_BA2 WHERE FPRN='{0}' AND FTIMES='{1}'";

            strSQL = string.Format(strSQL, inpatientNO, times);

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// HIS_BA2  --病人诊断信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertHISBA2(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.RADT.Location obj)
        {
            string sql = @"INSERT INTO HIS_BA2
(FPRN,FTIMES,FZKTYKH,FZKDEPT,FZKDATE,FZKTIME)
VALUES
('{0}','{1}','{2}','{3}','{4}','{5}') ";

            DateTime dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Dept.Memo);
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');

            try
            {
                sql = string.Format(sql,
                                                 this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 fun.ConverDept(obj.Dept.ID),//转科统一科号，HIS接收时存储HIS科号
                                                 obj.Dept.Name,//转科科别
                                                 fun.ChangeDateTime(obj.Dept.Memo),//转科日期
                                                 dt.ToShortTimeString());
            }
            catch
            {
            }

            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int DeleteHISBA5(string inpatientNO, int times)
        {
            string strSQL = @"delete from HIS_BA5 where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO,times.ToString());

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// HisBa5  --妇婴信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int insertHisBa5(Neusoft.HISFC.Models.HealthRecord.Base patientInfo,Neusoft.HISFC.Models.HealthRecord.Baby  obj)
        {


            string sql = @"insert into HIS_BA5 (FPRN,FTIMES ,FBABYNUM ,FNAME ,FBABYSEXBH ,FBABYSEX ,FTZ ,FRESULTBH ,FRESULT ,
                  FZGBH ,FZG ,FGRICD10 ,FGRNAME ,FBABYGR ,FBABYQJ ,FBABYSUC ,FHXBH ,FHX) 
                                         values ('{0}',{1},{2},'{3}','{4}','{5}',{6},'{7}','{8}',
                  '{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')";



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
                sql = string.Format(sql, this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 obj.HappenNum.ToString(),//婴儿序号
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.SexCode.ToString(),//婴儿性别编号
                                                 obj.Infect.Memo,//婴儿性别
                                                 obj.Weight.ToString(),//婴儿体重
                                                 obj.BirthEnd.ToString(), //分娩结果编号
                                                 this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CHILDBEARINGRESULT, obj.BirthEnd).Name.ToString(),  //分娩结果
                                                 obj.BabyState,//转归编号
                                                 this.constMana.GetConstant("babyZG", obj.BabyState).Name.ToString(),  //转归
                                                 obj.Infect.ID,//感染名称ICD10码
                                                 obj.Infect.Name,//
                                                 obj.InfectNum,//婴儿感染次数
                                                 obj.SalvNum,//婴儿抢救次数,
                                                 obj.SuccNum,//婴儿抢救成功次数
                                                 obj.Breath,//呼吸编号
                                                 this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.BREATHSTATE, obj.BirthEnd).Name.ToString() //呼吸
                                                 );
            }
            catch
            {
            }
            ReadSQL(sql);
            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }


            return 1;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public int DeleteHISBA4(string inpatientNO, int times)
        {
            string strSQL = @"delete from HIS_BA4 where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO,times.ToString());

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }

        /// <summary>
        ///  HisBa4  --手术信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int insertHisBa4(Neusoft.HISFC.Models.HealthRecord.Base patientInfo,Neusoft.HISFC.Models.HealthRecord.OperationDetail  obj)
        {

            #region  sql
            string sql = @"INSERT INTO HIS_BA4
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
FMZDOCT	--麻醉医生
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
'{21}'
)";
            #endregion
            string MarcKind_Code = string.Empty;
            string MarcKind_Name = string.Empty;

            Neusoft.FrameWork.Models.NeuObject info= this.constMana.GetConstant("CASEANESTYPE", obj.MarcKind);
            if (info != null && info.ID!="")
            {
                if(info.Memo!="" && info.Memo!="true")
                {
                    MarcKind_Code = info.Memo;
                    MarcKind_Name = info.Name;
                }
            }
            else
            {
                MarcKind_Code = obj.MarcKind;
                MarcKind_Name = info.Name;
            }
            //新增切口，0类分为01，02,03
            //string NickKind_Code = string.Empty;
            //string NickKind_Name = string.Empty;
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

            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql, this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.HappenNO.ToString(),//手术次数
                                                 obj.OperationInfo.ID.ToString(),//手术码
                                                 this.fun.ChangeCharacter(obj.OperationInfo.Name.ToString()),//手术码对应名称
                                                 fun.ChangeDateTime(obj.OperationDate.ToShortDateString()),//手术日期
                                                 obj.NickKind.ToString(), //切口编号
                                                 this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCITYPE, obj.NickKind).Name.ToString(),  //切口
                                                 obj.CicaKind.ToString(),//愈合编号
                                                 this.constMana.GetConstant("CICATYPE", obj.CicaKind).Name.ToString(),  //愈合
                                                 this.fun.ConverDoc(obj.FirDoctInfo.ID),//手术医生编号
                                                 obj.FirDoctInfo.Name,//手术医生
                                                 MarcKind_Code, // obj.MarcKind.ToString(),//麻醉方式编号
                                                 MarcKind_Name, //this.constMana.GetConstant("CASEANESTYPE",obj.MarcKind).Name.ToString(),//麻醉方式
                                                 addFlag, //"0"是否附加手术
                                                 this.fun.ConverDoc(obj.SecDoctInfo.ID),//I助编号
                                                 obj.SecDoctInfo.Name.ToString(), // I助姓名
                                                 this.fun.ConverDoc(obj.ThrDoctInfo.ID),//II助编号
                                                 obj.ThrDoctInfo.Name.ToString(),//II助姓名
                                                 this.fun.ConverDoc(obj.NarcDoctInfo.ID),//麻醉医生编号
                                                 obj.NarcDoctInfo.Name.ToString() //麻醉医生
                                                 );
            }
            catch
            {
            }
            sql = sql.Replace("'NULL'", "NULL");
            ReadSQL(sql);
            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 删除HISBA6
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <param name="times">住院次数</param>
        /// <returns></returns>
        public int DeleteHISBA6(string inpatientNO, int times)
        {
            string strSQL = @"DELETE FROM HIS_BA6 WHERE FPRN='{0}' AND FTIMES='{1}'";

            strSQL = string.Format(strSQL, inpatientNO, times);

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// HIS_BA6  --病人肿瘤信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertHISBA6(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Tumour obj)
        {
            #region sql
            string sql = @"INSERT INTO HIS_BA6
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
                                                 this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 obj.Rmodeid,//放疗方式编号
                                                 "",//放疗方式
                                                 obj.Rprocessid,//放疗程序编号
                                                 "",//放疗程序
                                                 obj.Rdeviceid,//放疗装置编号
                                                 "",//放疗装置
                                                 obj.Gy1,//1.原发灶剂量
                                                 obj.Time1,//原发灶次数
                                                 obj.Day1,//原发灶天数
                                                 fun.ChangeDateTime(obj.BeginDate1.ToString()),//原发灶开始日期
                                                 fun.ChangeDateTime(obj.EndDate1.ToString()),//原发灶结束时间
                                                 obj.Gy2,//2.区域淋巴结剂量
                                                 obj.Time2,//区域淋巴结次数
                                                 obj.Day2,//区域淋巴结天数
                                                 fun.ChangeDateTime(obj.BeginDate2.ToString()),//区域淋巴结开始时间
                                                 fun.ChangeDateTime(obj.EndDate2.ToString()),//区域淋巴结结束时间
                                                 obj.Position,//3.转移灶名称
                                                 obj.Gy3,//3.转移灶剂量
                                                 obj.Time3,//转移灶次数
                                                 obj.Day3,//转移灶天数
                                                 fun.ChangeDateTime(obj.BeginDate3.ToString()),//转移灶开始时间
                                                 fun.ChangeDateTime(obj.EndDate3.ToString()),//转移灶结束时间
                                                 obj.Cmodeid,//化疗方式编号
                                                 "",//化疗方式
                                                 obj.Cmethod,//化疗方法编号
                                                 "",//化疗方法
                                                 obj.Tumour_Type,//肿瘤病例分类
                                                 obj.Tumour_T,//原发肿瘤T
                                                 obj.Tumour_N,//淋巴转移N
                                                 obj.Tumour_M,//远程转移M
                                                 obj.Tumour_Stage,//分期
                                                 obj.Tumour_Stage//分期编码
                                                 );
            }
            catch
            {
            }

            sql = sql.Replace("'NULL'", "NULL");

            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 删除HISBA7
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <param name="times">住院次数</param>
        /// <returns></returns>
        public int DeleteHISBA7(string inpatientNO, int times)
        {
            string strSQL = @"DELETE FROM HIS_BA7 WHERE FPRN='{0}' AND FTIMES='{1}'";

            strSQL = string.Format(strSQL, inpatientNO, times);

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// HIS_BA7  --病人肿瘤信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertHISBA7(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.TumourDetail obj)
        {
            #region sql
            string sql = @"INSERT INTO HIS_BA7
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
                sql = string.Format(sql,
                                                 this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 fun.ChangeDateTime(obj.CureDate.ToString()),//化疗起始日期
                                                 fun.ChangeDateTime(obj.CureDate.ToString()),//化疗终止日期
                                                 obj.DrugInfo.Name+"("+obj.Qty.ToString()+obj.Unit.ToString()+")",//化疗药物名称及剂量
                                                 obj.Period.ToString(),//化疗疗程
                                                 obj.Result.ToString(),//疗效编号
                                                 "" //疗效
                                                 );
            }
            catch
            {
            }

            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// ba8  基本信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <returns></returns>
        public int InsertBa8(Neusoft.HISFC.Models.HealthRecord.Base patientInfo)
        {
            string strSQL = this.fun.GetInsertba8SQL(patientInfo);

            if (strSQL == null || strSQL == "")
            {
                return -1;
            }

            ReadSQL(strSQL);

            if (this.cmd.ExecuteNonQuery() <= 0)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// HIS_BA7 --肿瘤化疗药物
        /// </summary>
        /// <param name="alTumourDetail"></param>
        /// <param name="patientInfo"></param>
        /// <returns></returns>
        public int InsertBa10(System.Collections.ArrayList alTumourDetail, Neusoft.HISFC.Models.HealthRecord.Base patientInfo)
        {
            foreach (Neusoft.HISFC.Models.HealthRecord.TumourDetail info in alTumourDetail)
            {
                string strSQL = this.fun.GetInsertba10SQL(info, patientInfo);

                if (strSQL == null || strSQL == "")
                {
                    return -1;
                }

                ReadSQL(strSQL);

                if (this.cmd.ExecuteNonQuery() <= 0)
                {
                    return -1;
                }
            }

            return 1;
        }

        /// <summary>
        /// 原来的ba3_f导入到新的HIS_BA3中
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="alBA3_F"></param>
        /// <returns></returns>
        public int InsertBa3_f(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, System.Collections.ArrayList alBA3_F)
        {
            this.IsOpen();

            this.ConverICDType(alBA3_F);

            this.cmd.CommandText = string.Empty;
            string strsql = @"insert into tDiagnose (fprn,ftimes,FZDLX, FICDVersion, FICDM, FJBNAME,FZLJGBH,FZLJG) values ('{0}',{1},'{2}',{3},'{4}','{5}','{6}','{7}')";
            string temp = string.Empty;

            foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose obj in alBA3_F)
            {
                if (obj.SecondICD.Trim() == "")
                {
                    continue;
                }

                if (obj.DiagInfo.DiagType.ID == "1")
                {
                    continue;
                }

                this.cmd.CommandText = string.Empty;
                temp = strsql;
                this.cmd.CommandText = string.Format(temp,
                                                     patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr()),
                                                     patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                     obj.DiagInfo.DiagType.ID,//.HappenNo,//
                                                     "10",
                                                     obj.SecondICD,
                                                     "",//还没写
                                                     obj.DiagOutState,
                                                     this.constMana.GetConstant ( "ZG", obj.DiagOutState ).Name );//没确定

                if (this.cmd.ExecuteNonQuery() <= 0)
                {
                    return -1;
                }
            }

            return 1;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="inpatientNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int Delete(string tableName, string inpatientNo, string times)
        {
            string strSQL = this.fun.GetDeleteSQL(tableName, inpatientNo, times);

            if (strSQL == null || strSQL == "")
            {
                return -1;
            }

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }

        #region 门诊观察室和急诊日志
        /// <summary>
        /// tEmergeLogNoKs -- 门诊急诊日志(不分科)
        /// </summary>
        /// <param name="alUpload">急诊日报集合</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isExist">是否有数据被覆盖</param>
        /// <returns></returns>
        public int InserttEmergeLog(System.Collections.ArrayList alUpload, string beginDate, string endDate, ref bool isExist)
        {


            int i = 0;
            string strSQL = "delete tEmergeLog where FRQ  between '{0}' and '{1}'";
            ReadSQL(string.Format(strSQL, beginDate, endDate));
            try
            {
                i = this.cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            if (i > 0) isExist = true;
            i = 0;

          
            foreach (Neusoft.HISFC.Models.HealthRecord.Base b in alUpload)
            {
                string sql = @"INSERT INTO tEmergeLog
                              (frq,
                            ftykh,
                            fksname,
                            FdoctorNum,
                            Fhour,
                            Fnum,
                            FdeadNum,
                            FqjNum,
                            FqjsucNum,
                            Fcars,
                            FoperNum,
                            FWorkrq
                            )
                            VALUES
                              (
                               '{0}',
                               '{1}',
                               '{2}',
                               {3},
                               {4},
                               {5},
                               {6},
                               {7},
                               {8},
                               {9},
                               {10},
                               '{11}'
                            )";
                sql = string.Format(sql, b.PatientInfo.PVisit.InTime, b.InDept.ID,b.InDept.Name,b.PatientInfo.ID,b.PatientInfo.Memo, b.PatientInfo.User01, b.PatientInfo.User02,
                                         b.PatientInfo.User03, b.PatientInfo.UserCode, b.PatientInfo.WBCode, b.PatientInfo.PVisit.User01,
                                         b.PatientInfo.PVisit.OutTime);

                ReadSQL(sql);
                if (this.cmd.ExecuteNonQuery() < 0)
                {
                    //return -1;
                }
                i++;
            }
            //if (intDelete > 0) isExist = true;
            return i;
        }

        /// <summary>
        /// HIS_MZLOG5 -- 门诊观察室日志(不分科)
        /// </summary>
        /// <param name="alPatientMove">观察室日报集合</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isExist">是否有数据被覆盖</param>
        /// <returns></returns>
//        public int InsertHisMzlog5 ( System.Collections.ArrayList alObserver, string beginDate, string endDate, ref int intDelete )
//        {
//            this.IsOpen ( );
//            //int intDelete = 0;
//            string strSQL = "delete tObservelogNoKs where FRQ = '{0}'";
//            string strsql = @"INSERT INTO tObservelogNoKs
//  (FRq,
//Fbedsum,
//Foldbrrs,
//Finbrrs,
//Foutbrrs,
//Foutdeadbrrs,
//Fqjbrrsum,
//Fqjsuc,
//Fnowbrrs,
//Foutbrbedts,
//Fallbeddays,
//Fywcwwryrs,
//FWorkrq
//)
//VALUES
//  (
//   '{0}',
//   {1},
//   {2},
//   {3},
//   {4},
//   {5},
//   {6},
//   {7},
//   {8},
//   {9},
//   {10},
//   {11},
//   '{12}'
//)";
//            string temp = string.Empty;
//            int intReturn = 0;
//            foreach ( Neusoft.FrameWork.Models.Nurse.ObserveReport clinic in alObserver )
//            {
//                int j = 0;
//                // string oldDeptCode = this.ConverDept ( patientMove.DeptCode );
//                this.cmd.CommandText = string.Empty;
//                this.cmd.CommandText = string.Format ( strSQL, clinic.ReportDate.Replace ( '-', '/' ) );
//                try
//                {
//                    j = this.cmd.ExecuteNonQuery ( );
//                }
//                catch ( Exception ex )
//                {
//                    this.Err = ex.Message;
//                }
//                if ( j > 0 )
//                    intDelete += j;


//                this.cmd.CommandText = string.Empty;
//                temp = strsql;
//                this.cmd.CommandText = string.Format ( temp,
//                                                 clinic.ReportDate.Replace ( '-', '/' ),
//                                                 clinic.BedCount,
//                                                 clinic.Count1,
//                                                 clinic.Count2,
//                                                 clinic.OutCount,
//                                                 clinic.OutCount3,
//                                                 clinic.OutCount1,
//                                                 clinic.OutCount2,
//                                                 clinic.TodayCount,
//                                                 clinic.TodayCount1,
//                                                 clinic.TodayCount2,
//                                                 "0",//因无床位未住院人数
//                                                 clinic.Memo.Replace ( '-', '/' )
//                                                 );

//                if ( this.cmd.ExecuteNonQuery ( ) <= 0 )
//                {
//                    return -1;
//                }
//                intReturn++;
//            }
//            //if (intDelete > 0) isExist = true;
//            return intReturn;
//        }

        /// <summary>
        /// tSpecialLog --专科门诊病人数
        /// </summary>
        /// <param name="alUpload">专科门诊病人日报集合</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isExist">是否有数据被覆盖</param>
        /// <returns></returns>
        public int InserttSpecialLog(System.Collections.ArrayList alUpload, string beginDate, string endDate, ref bool isExist)
        {


            int i = 0;
            string strSQL = "delete tSpecialLog where FRQ  between '{0}' and '{1}'";
            ReadSQL(string.Format(strSQL, beginDate, endDate));
            try
            {
                i = this.cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            if (i > 0) isExist = true;
            i = 0;

        
            foreach (Neusoft.HISFC.Models.HealthRecord.Base b in alUpload)
            {
                string sql = @"INSERT INTO tSpecialLog
                              (frq,
                            FTykh,
                            FKsName,
                            FTyzkcode,
                            FzkName,
                            FZlrs,
                            FWorkrq
                            )
                            VALUES
                              (
                               '{0}',
                               '{1}',
                               '{2}',
                               '{3}',
                               '{4}',
                                {5}, 
                               '{6}'
                            )";
                sql = string.Format(sql, b.PatientInfo.PVisit.InTime, b.InDept.ID, b.InDept.Name, b.OutDept.ID,b.OutDept.Name,
                                          b.PatientInfo.ID, b.PatientInfo.PVisit.OutTime);

                ReadSQL(sql);
                if (this.cmd.ExecuteNonQuery() < 0)
                {
                    //return -1;
                }
                i++;
            }
            //if (intDelete > 0) isExist = true;
            return i;
        }

        #endregion

        #region 住院动态日报
        /// <summary>
        /// HIS_ZYLOG -- 病房动态日志
        /// 不插入这个HIS临时表，改为插入正式的TZyWardWorkDayReport
        /// </summary>
        /// <param name="alPatientMove">动态日报集合</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isExist">是否有数据被覆盖</param>
        /// <returns></returns>
        public int InsertZbfworklog(System.Collections.ArrayList alPatientMove, string beginDate, string endDate, ref int intDelete)
        {
            this.IsOpen();
            //int intDelete = 0;
            string strSQL = "delete TZyWardWorklog where FRQ = '{0}' and FTykh= '{1}'";

            string strsql = @"INSERT INTO TZyWardWorklog
  (FRQ,
FTykh,
FKsName,
FBEDSUM,
FStatFlag,
FYRS,
FRYRS,
FTKZR,
FTQZR,
FCYRS,
FDEADRS,
FZRTK,
FZRTQ,
FXYRS,
FPRS,
FBCZYL,
FWorkrq
)
VALUES
  (
   '{0}',
   '{1}',
   '{2}',
   {3},
   '{4}',
   {5},
   {6},
   {7},
   {8},
   {9},
   {10},
   {11},
   {12},
   {13},
   {14},
   {15},
   '{16}'
)";
            string temp = string.Empty;
            int intReturn = 0;
            foreach (Neusoft.HISFC.Models.HealthRecord.Case.PatientMove patientMove in alPatientMove)
            {
                int j = 0;
                string oldDeptCode = this.ConverDept(patientMove.DeptCode);
                this.cmd.CommandText = string.Empty;
                this.cmd.CommandText = string.Format(strSQL, patientMove.OperDate.ToShortDateString().Replace('-', '/'), oldDeptCode);
                try
                {
                    j = this.cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                }
                if (j > 0) intDelete+=j;

                
                this.cmd.CommandText = string.Empty;
                temp = strsql;
                this.cmd.CommandText = string.Format(temp,
                                                 patientMove.OperDate.ToShortDateString ( ).Replace ( '-', '/' ),
                                                 "TZY" + oldDeptCode +"*1",
                                                 this.deptMana.GetDeptmentById(patientMove.DeptCode).Name,
                                                 //GetOldInDeptCode(patientMove.DeptCode),
                                                 //patientMove.Name,
                                                 patientMove.BedNum.ToString(),
                                                 "***",
                                                 patientMove.OriginalNum.ToString(),
                                                 patientMove.InNum.ToString(),
                                                 patientMove.OtherDeptIn.ToString(),
                                                 patientMove.OtherRegionIn.ToString(),
                                                 patientMove.OutNum.ToString(),
                                                 patientMove.DeadNum.ToString(),
                                                 patientMove.ToOtherDept.ToString(),
                                                 patientMove.ToOtherRegion.ToString(),
                                                 patientMove.PatientNum.ToString(),
                                                 patientMove.AccompanyNum.ToString(),
                                                 patientMove.BeduseNum.ToString(),//**床位占用率,没写算法，要在表现层赋值**
                                                 patientMove.Memo.Replace('-', '/')
                                                 );

                if (this.cmd.ExecuteNonQuery() <= 0)
                {
                    return -1;
                }
                intReturn++;
            }
            //if (intDelete > 0) isExist = true;
            return intReturn;
        }

        public string GetOldInDeptCode(string newDeptCode)
        {
            string oldDeptname = "";
            try
            {
                oldDeptname = this.constMana.GetConstant("DEPTUPLOAD", newDeptCode).Memo;
            }
            catch
            {
            }
           
            return oldDeptname;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientMove"></param>
        /// <returns></returns>
        public int InsertZbfworklog(Neusoft.HISFC.Models.HealthRecord.Case.PatientMove  patientMove)
        {
            this.IsOpen();
        

            this.cmd.CommandText = string.Empty;
            string strsql = @"INSERT INTO TZyWardWorklog
  (FRQ,
FTykh,
FKsName,
FBEDSUM,
FStatFlag,
FYRS,
FRYRS,
FTKZR,
FTQZR,
FCYRS,
FDEADRS,
FZRTK,
FZRTQ,
FXYRS,
FPRS,
FBCZYL,
FWorkrq
)
VALUES
  (
   '{0}',
   '{1}',
   '{2}',
   {3},
   '{4}',
   {5},
   {6},
   {7},
   {8},
   {9},
   {10},
   {11},
   {12},
   {13},
   {14},
   {15},
   '{16}'
)";

            this.cmd.CommandText = string.Format(strsql,
                                                 patientMove.OperDate.ToShortDateString ( ).Replace ( '-', '/' ),
                                                 "TZY" + this.ConverDept ( patientMove.DeptCode ) + "*1",
                                                 this.deptMana.GetDeptmentById(patientMove.DeptCode).Name,
                                                 //patientMove.Name,
                                                 patientMove.BedNum.ToString(),
                                                 "***",
                                                 patientMove.OriginalNum.ToString(),
                                                 patientMove.InNum.ToString(),
                                                 patientMove.OtherDeptIn.ToString(),
                                                 patientMove.OtherRegionIn.ToString(),
                                                 patientMove.OutNum.ToString(),
                                                 patientMove.DeadNum.ToString(),
                                                 patientMove.ToOtherDept.ToString(),
                                                 patientMove.ToOtherRegion.ToString(),
                                                 patientMove.PatientNum.ToString(),
                                                 patientMove.AccompanyNum.ToString(),
                                                 patientMove.BeduseNum.ToString ( ),//**床位占用率,没写算法，要在表现层赋值**
                                                 patientMove.Memo.Replace('-', '/')
                                                 );

            if (this.cmd.ExecuteNonQuery() <= 0)
            {
                return -1;
            }

            return 1;
        }


        public int DeleteZbfworklog(Neusoft.HISFC.Models.HealthRecord.Case.PatientMove patientMove)
        {
            string strSQL = @"delete from TZyWardWorklog where FTykh = '{0}' and FRQ = '{1}'";

            strSQL = string.Format(strSQL, this.ConverDept(patientMove.DeptCode), patientMove.OperDate.ToShortDateString().Replace('-', '/'));

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }

        #endregion

        /// <summary>
        /// 上传费用信息(已修改)
        /// </summary>
        /// <returns></returns>
        public int UpdateFeeinfo(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee)
        {
            string strSQL = @"UPDATE  tPatientVisit
   SET 
       FSUM1 =  {0},
       FCWF = {1},   
       FXYF = {2},
       FZYF = {3},
       FZCYF = {4},
       FZCHYF = {5},
       FJCF = {6},
       FZLF = {7},
       FFSF = {8},
       FSSF = {9},
       FHYF = {10},
       FSXF = {11},
       FSYF = {12},
       FJSF = {13},
       FQTF = {14}
 WHERE fprn = '{15}' and frydate = '{16}' and fsum1 = 0";



            strSQL = string.Format(strSQL, this.GetFeeinfo(b, alFee));

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }

        /// <summary>
        /// 取费用信息(不需要修改)
        /// </summary>
        /// <param name="b"></param>
        /// <param name="alFee"></param>
        /// <returns></returns>
        private string[] GetFeeinfo(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee)
        {
            string[] s = new string[17];

            for (int j = 0; j < s.Length; j++)
            {
                s[j] = "0.00";
            }

            decimal feeTot = 0.0M;
            decimal feeOther = 0.0M;
            foreach (Neusoft.HISFC.Models.RADT.Patient feeInfo in alFee)
            {
                decimal fee1 = 0.0M;
                fee1 = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeInfo.IDCard), 2);

                feeTot += fee1;

                string fee = fee1.ToString();

                if (feeInfo.DIST == "1")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[1]) + fee1;
                    s[1] = temp.ToString();
                }
                else if (feeInfo.DIST == "3")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[2]) + fee1;
                    s[2] = temp.ToString();
                }
                else if (feeInfo.DIST == "5")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[4]) + fee1;
                    s[4] = temp.ToString();
                }
                else if (feeInfo.DIST == "4")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[5]) + fee1;
                    s[5] = temp.ToString();
                }
                else if (feeInfo.DIST == "13")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[6]) + fee1;
                    s[6] = temp.ToString();
                }
                else if (feeInfo.DIST == "10")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[7]) + fee1;
                    s[7] = temp.ToString();
                }
                else if (feeInfo.DIST == "11")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[9]) + fee1;
                    s[9] = temp.ToString();
                }
                else if (feeInfo.DIST == "7")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[10]) + fee1;
                    s[10] = temp.ToString();
                }
                else if (feeInfo.DIST == "9")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[11]) + fee1;
                    s[11] = temp.ToString();
                }
                else if (feeInfo.DIST == "8")
                {
                    decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[12]) + fee1;
                    s[12] = temp.ToString();
                }
                else
                {
                   
                    feeOther += fee1;
                }
            }
            s[0] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeTot), 2).ToString();
            s[3] = "0.00";
            s[8] = "0.00";
            s[13] = "0.00";
            s[14] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeOther), 2).ToString();

            s[15] = b.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());
            s[16] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');

            return s;
            
        }

        /// <summary>
        /// 从病案取数据
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetPatientInfoFromBA(string begin, string end)
        {
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
       FCYTYKHM,
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
       IFZDSS,
       FIFZDSS,
       FMZDOCT,
       FJBFX
  FROM HIS_BA1, temp1 where HIS_BA1.fprn = temp1.prn and HIS_BA1.ftimes = temp1.times and HIS_BA1.fprn >= '{0}' and HIS_BA1.fprn <= '{1}'";

            this.cmd.CommandText = string.Empty;

            try
            {
                strSQL = string.Format(strSQL, begin, end);
            }
            catch (Exception ex)
            {
                return null;
            }

            ReadSQL(strSQL);

            System.Collections.ArrayList al = new System.Collections.ArrayList();

            this.reader = this.cmd.ExecuteReader();

            try
            {
                while (this.reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.Base b = new Neusoft.HISFC.Models.HealthRecord.Base();

                    b.PatientInfo.PID.PatientNO = this.reader[0].ToString();
                    b.PatientInfo.Name = this.reader[1].ToString();

                    al.Add(b);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.reader.Close();
            }


            return al;
        }

        /// <summary>
        /// 根据病历号查询
        /// </summary>
        /// <param name="cardNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.PatientInfo GetPatientFromBA(string cardNO)
        {
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

            this.cmd.CommandText = string.Empty;

            try
            {
                strSQL = string.Format(strSQL, cardNO);
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;

                return null;
            }

            ReadSQL(strSQL);

            this.reader = this.cmd.ExecuteReader();

            try
            {
                if (this.reader.Read())
                {
                    try
                    {
                        Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

                        PatientInfo.PID.CardNO = this.reader[0].ToString();
                        //PatientInfo.Birthday.ToString(); //出生日期
                        //PatientInfo.Sex.ID.ToString(); //性别
                        //PatientInfo.IDCard; //身份证号
                        PatientInfo.Name = this.reader[1].ToString();

                        string sex = this.reader[88].ToString();
                        if (sex.Trim().Equals("1"))
                        {
                            sex = "M";
                        }
                        else if (sex.Trim().Equals("2"))
                        {
                            sex = "F";
                        }
                        PatientInfo.Sex.ID = sex;

                        PatientInfo.IDCard = this.reader[91].ToString();

                        PatientInfo.Profession.ID = this.reader[5].ToString(); //职业

                        PatientInfo.CompanyName = this.reader[7].ToString(); //工作单位
                        PatientInfo.PhoneBusiness = this.reader[9].ToString(); //单位电话
                        PatientInfo.BusinessZip = this.reader[10].ToString(); //单位邮编
                        PatientInfo.AddressHome = this.reader[11].ToString(); //户口或家庭所在
                        //PatientInfo.PhoneHome = this.reader[10].ToString(); //家庭电话
                        PatientInfo.HomeZip = this.reader[12].ToString(); //户口或家庭邮政编码
                        //PatientInfo.DIST = this.reader[10].ToString(); //籍贯
                        //PatientInfo.Nationality.ID = this.reader[10].ToString(); //民族
                        PatientInfo.Kin.Name = this.reader[13].ToString(); //联系人姓名
                        PatientInfo.Kin.RelationPhone = this.reader[16].ToString(); //联系人电话
                        PatientInfo.Kin.RelationAddress = this.reader[15].ToString(); //联系人住址
                        PatientInfo.Kin.Relation.Name = this.reader[14].ToString(); //联系人关系

                        PatientInfo.MaritalStatus.ID = this.reader[6].ToString(); //婚姻状况
                        //PatientInfo.Country.ID = this.reader[10].ToString(); //国籍

                        //PatientInfo.AreaCode = this.reader[10].ToString(); //出生地
                        try
                        {
                            PatientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[89].ToString().Replace('/', '-') + " 00:00:00");
                        }
                        catch
                        {
                            PatientInfo.Birthday = DateTime.MinValue;
                        }

                        PatientInfo.AreaCode = this.reader[90].ToString();

                        PatientInfo.Country.Name = this.reader[92].ToString();
                        PatientInfo.Nationality.Name = this.reader[93].ToString();
                        //来源
                        PatientInfo.PVisit.InSource.ID = this.reader[20].ToString();
                        //医保号

                        PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[2].ToString());

                        try
                        {
                            PatientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[27].ToString().Replace('/', '-') + " 00:00:00");
                        }
                        catch
                        {
                            PatientInfo.PVisit.OutTime = DateTime.MinValue;

                            PatientInfo.User01 = this.reader[27].ToString();
                        }

                        PatientInfo.User02 = this.reader[38].ToString();//过敏药物

                        return PatientInfo;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    this.Err1 = "没读到！";

                    return null;
                }
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }

        }

        /// <summary>
        /// HIS_MZLOG9 -- 专科门诊病人数
        /// </summary>
        /// <param name="alFee"></param>
        /// <returns></returns>
        public int InsertMspecial(System.Collections.ArrayList alFee)
        {
            int i = 0;
            foreach (Neusoft.HISFC.Models.Base.Employee empl in alFee)
            {
                string strSQL = this.fun.GetInsertMspecialSQL(empl);

                if (strSQL == null || strSQL == "")
                {
                    //return -1;
                }

                ReadSQL(strSQL);

                if (this.cmd.ExecuteNonQuery() <= 0)
                {

                    //return -1;
                }
                //return 1;
                i++;
            }
            return i;
        }

        /// <summary>
        /// HIS_MZLOG1 -- 科室门诊工作日志
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <returns></returns>
        public int InsertMworklog1(System.Collections.ArrayList alFee)
        {
            int i = 0;
            foreach (Neusoft.HISFC.Models.Base.Employee empl in alFee)
            {
                string strSQL = this.fun.GetInsertMworklog1SQL(empl);

                if (strSQL == null || strSQL == "")
                {
                    //return -1;
                }

                ReadSQL(strSQL);

                if (this.cmd.ExecuteNonQuery() <= 0)
                {

                    //return -1;
                }
                //return 1;
                i++;
            }
            return i;
        }

        /// <summary>
        /// HIS_MZLOG2 -- 医生门诊工作日志
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <returns></returns>
        public int InserttWorkLog(System.Collections.ArrayList alUpload, string beginDate, string endDate, ref bool isExist)
        {
            int i = 0;
            //删除同一天的记录
            string strSQL = "delete tWorklog where frq between '{0}' and '{1}'";
            ReadSQL(string.Format(strSQL, beginDate, endDate));
            try
            {
                i = this.cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            if (i > 0) isExist = true;
            i = 0;

            string sql = string.Empty;
            foreach (Neusoft.HISFC.Models.HealthRecord.Base b in alUpload)
            {
                sql = this.fun.GetInserttWorkLogSQL(b);

                if (sql == null || sql == "")
                {
                    this.err = fun.Err;
                    return -1;
                }

                this.cmd.CommandText = sql;

                if (this.cmd.ExecuteNonQuery() <= 0)
                {

                    //return -1;
                }
                //return 1;
                i++;
            }
            return i;
        }

        /// <summary>
        /// 是否已经有了
        /// 返回：-1 失败  1 都有  2 已录入或已上传  3 需要上传
        /// 1、病案表tPatientVisit是否已录入
        /// 2、接口表his_ba1是否已上传
        /// </summary>
        /// <param name="prn">病案号</param>
        /// <param name="outTime">出院日期</param>
        /// <returns></returns>
        public int GetIsHave(string prn,  DateTime outTime)
        {
            string strSQLHisba1 = @"select count(*) from tPatientVisit where fprn = '{0}' and fcydate = '{1}'";

            string strSQLTpatientVisit = @"select count(*) from his_ba1 where fprn = '{0}' and fcydate = '{1}'";

            try
            {
                strSQLHisba1 = string.Format(strSQLHisba1, prn, outTime.ToShortDateString().Replace('-', '/'));

                strSQLTpatientVisit = string.Format(strSQLTpatientVisit, prn, outTime.ToShortDateString().Replace('-', '/'));
            }
            catch
            {
                return -1;
            }

            ReadSQL(strSQLHisba1);

            this.reader = this.cmd.ExecuteReader();

            bool haveHisba1 = false;
            bool haveTpatientVisit = false;


            try
            {
                if (this.reader.Read())
                {
                    if (this.reader[0].ToString().Trim().Equals("0"))
                    {
                        haveHisba1 = false;
                    }
                    else
                    {
                        haveHisba1 = true;
                    }
                }
                else
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                    return -1;
                }
            }
            catch
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }

                return -1;
            }

            if (!this.reader.IsClosed)
            {
                this.reader.Close();
            }

            this.cmd.CommandText = strSQLTpatientVisit;

            this.reader = this.cmd.ExecuteReader();

            try
            {
                if (this.reader.Read())
                {
                    if (this.reader[0].ToString().Trim().Equals("0"))
                    {
                        haveTpatientVisit = false;
                    }
                    else
                    {
                        haveTpatientVisit = true;
                    }
                }
                else
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                    return -1;
                }
            }
            catch
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }

                return -1;
            }

            if (!this.reader.IsClosed)
            {
                this.reader.Close();
            }

            if (haveHisba1)
            {
                if (haveTpatientVisit)
                {
                    //都有了
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                if (haveTpatientVisit)
                {
                    return 2;
                }
                else
                {
                    //都没有
                    return 3;
                }
            }

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
        public int GetInTimes(string prn,string fzyid,int itype ,ref string inTimes)
        {
            string strSQLinTimes=string.Empty;
            if (itype == 1)
            {
                strSQLinTimes = @"SELECT FTIMES FROM HIS_BA1 WHERE FZYID='{0}'";
                try
                {
                    strSQLinTimes = string.Format(strSQLinTimes,fzyid);
                }
                catch
                {
                    return -1;
                }
            }
            else
            {
                strSQLinTimes = @"select case when  max(a.ftimes) is null then 0 else  max(a.ftimes) end   ftimes from 
                                    (select max(ftimes) ftimes from his_ba1        where fprn='{0}'
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
            ReadSQL(strSQLinTimes);

            this.reader = this.cmd.ExecuteReader();
            try
            {
                if (this.reader.Read())
                {
                    inTimes = this.reader[0].ToString().Trim();
                }
                else
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                    return -1;
                }
            }
            catch
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }

                return -1;
            }
            finally
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
            }
            return 1;
        }

        /// <summary>
        /// 首先判断是否有，如果有，则判断是否一样
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="patientNO"></param>
        /// <param name="name"></param>
        /// <param name="times"></param>
        /// <param name="inTime"></param>
        /// <returns></returns>
        public int GetIsHaveNew(string prn,string patientNO,string name, ref string times,DateTime inTime)
        {
            string strSql1= "select FNAME from tPatientVisit where FPRN='{0}'";

            string strSql2 = "select count(*) from tPatientVisit where FPRN='{0}' and FTIMES={1}";

            string strSql3 = @"select max(c),max(ftimes) from 
                                        (select max(FCYDATE) c,ftimes ftimes from his_ba1
                                        where ftimes=(select max(ftimes) from his_ba1 where fprn='{0}')
                                        and fprn='{0}' 
                                        group by ftimes
                                        union 
                                        select max(FCYDATE) c,ftimes from tpatientvisit
                                        where ftimes=(select max(ftimes) from tpatientvisit
                                        where fprn='{0}')
                                        and fprn='{0}'
                                        group by ftimes) a";

            try
            {
                strSql1 = string.Format(strSql1, prn);
                strSql2 = string.Format(strSql2, prn, times);
                strSql3 = string.Format(strSql3, prn, times);
            }
            catch(Exception e)
            {
                this.Err ="格式化SQL语句出错！"+ e.Message;
                return -1;
            }

            int retValue = -1;
            try
            {
                //如果已经传过


                this.ReadSQL(strSql1);
                this.reader=this.cmd.ExecuteReader();

                if (this.reader.Read())
                {
                    if (this.reader[0].ToString().Trim() == name.Trim())
                    {
                        retValue = 2;
                    }
                    else
                    {
                        retValue = 0;
                    }
                }
                else
                {
                    retValue = 1;
                }
                this.reader.Close();

                //判断是否存在
                if (retValue == 2)
                {
                    this.ReadSQL(strSql2);
                    this.reader=this.cmd.ExecuteReader();
                    if (this.reader.Read())
                    {
                        if (Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[0].ToString()) > 0)
                        {
                            retValue = 2;                            
                        }
                        else
                        {
                            retValue = 1;
                        }
                    }
                    else
                    {
                        retValue = 1;
                    }

                    this.reader.Close();
                }

                //判断出院日期
                if (retValue == 2)
                {
                    this.ReadSQL(strSql3);
                    this.reader = this.cmd.ExecuteReader();
                    if (this.reader.Read())
                    {
                        //上次出院时间小于本次入院时间  修改次数继续上传
                        if (Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[0].ToString()).Date < inTime.Date)
                        {
                            retValue = 2;
                            //取最大次数
                            times = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[1].ToString()).ToString();
                        }
                        else
                        {
                            retValue = 3;
                        }
                    }
                    else
                    {
                        retValue = 1;
                    }

                    this.reader.Close();
                }


                return retValue;
            }
            catch (Exception e)
            {
                this.Err = "执行SQL语句出错！" + e.Message;
                return -1;
            }
            finally
            {
                if (this.reader != null && this.reader.IsClosed == false)
                {
                    this.reader.Close();
                }
            }

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
            string strSQLNeed = @"select count(1) from HIS_BA1 where FPRN ='{0}' AND fzyid = '{1}'";
            try
            {
                strSQLNotNeed = string.Format(strSQLNotNeed, fprn,fzyid);

                strSQLNeed = string.Format(strSQLNeed,fprn, fzyid);
            }
            catch
            {
                return -1;
            }

            ReadSQL(strSQLNotNeed);

            this.reader = this.cmd.ExecuteReader();
            try
            {
                if (this.reader.Read())
                {
                    NotNeed = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[0].ToString());
                }
                else
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                    return -1;
                }
            }
            catch
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
                return -1;
            }
            finally
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
            }

            this.cmd.CommandText = strSQLNeed;

            this.reader = this.cmd.ExecuteReader();

            try
            {
                if (this.reader.Read())
                {
                    Need = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[0].ToString());
                }
                else
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                    return -1;
                }
            }
            catch
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
                return -1;
            }
            finally
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
            }


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
                strSQLNotNeed = string.Format(strSQLNotNeed, this.PatientNoChang(fprn.PadLeft(10,'0').Substring(this.PatientNoSubstr())), in_date.ToShortDateString());
            }
            catch
            {
                return -1;
            }

            ReadSQL(strSQLNotNeed);

            this.reader = this.cmd.ExecuteReader();
            try
            {
                if (this.reader.Read())
                {
                    NotNeed = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[0].ToString());
                }
                else
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                    return -1;
                }
            }
            catch
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
                return -1;
            }
            finally
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
            }
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
        #endregion

        #region  查询  新接口不需要使用这些，这个只用于旧系统导入到新HIS系统时使用

        /// <summary>
        /// 根据病历号加次数取基本信息
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Base GetPatientByCardnoAndTimes
            ( string cardno, string times )
        {
            #region SQL

            //            string strSQL = @"SELECT PRN,
            //       NAME,
            //       TIMES,
            //       FB,
            //       AGE,
            //       JOB,
            //       STATU,
            //       DWNAME,
            //       DWADDR,
            //       DWTELE,
            //       DWPOST,
            //       HKADDR,
            //       HKPOST,
            //       LXNAME,
            //       RELATE,
            //       LXADDR,
            //       LXTELE,
            //       RYDATE,
            //       RYTIME,
            //       RYINFO,
            //       SOURCE,
            //       RYNUM,
            //       RYDEPT,
            //       ZKNUM,
            //       ZKDEPT,
            //       CYNUM,
            //       CYDEPT,
            //       CYDATE,
            //       CYTIME,
            //       DAYS,
            //       MZZD,
            //       RYZD,
            //       QZDATE,
            //       IFSS,
            //       FYK,
            //       BFZ,
            //       YNGR,
            //       PHZD,
            //       GMYW,
            //       BLOOD,
            //       QJTIMES,
            //       SUCTIMES,
            //       SZ,
            //       SZQX,
            //       BODY,
            //       SUM1,
            //       CWF,
            //       XYF,
            //       ZYF,
            //       ZCYF,
            //       ZCHF,
            //       JCF,
            //       ZLF,
            //       FSF,
            //       SSF,
            //       HYF,
            //       SXF,
            //       SYF,
            //       JSF,
            //       QTF,
            //       SAMPLE,
            //       QUALITY,
            //       ZRDOCT,
            //       ZZDOCT,
            //       ZYDOCT,
            //       SXDOCT,
            //       BMY,
            //       MZACCO,
            //       RYACCO,
            //       OPACCO,
            //       OPACCO2,
            //       OPACCO3,
            //       OPACCO4,
            //       TWILL,
            //       QJBR,
            //       QJSUC,
            //       THREQZ,
            //       BABYNUM,
            //       ZLLB,
            //       QJFF,
            //       BACK,
            //       SXFY,
            //       SYFY,
            //       WORKDATE,
            //       IFZDSS,
            //       ifbzzl,
            //       mzdoct,
            //       zlf_zy,
            //       (select ascard1 from ba8 where ba2.prn = ba8.prn and ba2.times = ba8.times) as ascard1,
            //       (select birthday from ba1 where ba1.prn=ba2.prn) as birthday,
            //       (select native from ba1 where ba1.prn=ba2.prn) as native,
            //       (select nation from ba1 where ba1.prn=ba2.prn) as nation,
            //       (select sex from ba1 where ba1.prn=ba2.prn) as sex,
            //       (select idcard from ba1 where ba1.prn=ba2.prn) as idcard,
            //       (select birthpl from ba1 where ba1.prn=ba2.prn) as birthpl,
            //       (select hbsag from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as hbsag,   
            //       (select hcv_ab from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as hcv_ab,
            //       (select hiv_ab from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as hiv_ab,
            //       (select kzr from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as kzr,  
            //       (select jxdoct from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as jxdoct,
            //       (select ysxdoct from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as ysxdoct,
            //       (select zkdoct from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as zkdoct,
            //       (select zknurse from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as zknurse,
            //       (select zkrq from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as zkrq, 
            //       (select rh from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as rh,
            //       (select ISOPFIRST from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as ISOPFIRST,
            //       (select redcell from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as redcell,
            //       (select plaque from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as plaque,
            //       (select serous from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as serous,
            //       (select ALLBLOOD from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as ALLBLOOD,
            //       (select OTHERBLOOD from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as OTHERBLOOD,
            //       (select HZ_YJ from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HZ_YJ,  
            //       (select HZ_YC from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HZ_YC,
            //       (select HL_TJ from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HL_TJ,
            //       (select HL_I from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HL_I,
            //       (select HL_II from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HL_II,
            //       (select HL_III from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HL_III,
            //       (select HL_ZZ from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HL_ZZ,
            //       (select HL_TS from ba6 where ba6.prn = ba2.prn and ba6.times = ba2.times) as HL_TS
            //
            //
            //  FROM ba2
            //  where prn = '{0}' and times = '{1}'
            //";
            #endregion

            #region SQL

            string strSQL = @"SELECT PRN,
       NAME,
       TIMES,
       FB,
       AGE,
       JOB,
       STATU,
       DWNAME,
       DWADDR,
       DWTELE,
       DWPOST,
       HKADDR,
       HKPOST,
       LXNAME,
       RELATE,
       LXADDR,
       LXTELE,
       RYDATE,
       RYTIME,
       RYINFO,
       SOURCE,
       RYNUM,
       RYDEPT,
       ZKNUM,
       ZKDEPT,
       CYNUM,
       CYDEPT,
       CYDATE,
       CYTIME,
       DAYS,
       MZZD,
       RYZD,
       QZDATE,
       IFSS,
       FYK,
       BFZ,
       YNGR,
       PHZD,
       GMYW,
       BLOOD,
       QJTIMES,
       SUCTIMES,
       SZ,
       SZQX,
       BODY,
       SUM1,
       CWF,
       XYF,
       ZYF,
       ZCYF,
       ZCHF,
       JCF,
       ZLF,
       FSF,
       SSF,
       HYF,
       SXF,
       SYF,
       JSF,
       QTF,
       SAMPLE,
       QUALITY,
       ZRDOCT,
       ZZDOCT,
       ZYDOCT,
       SXDOCT,
       BMY,
       MZACCO,
       RYACCO,
       OPACCO,
       OPACCO2,
       OPACCO3,
       OPACCO4,
       TWILL,
       QJBR,
       QJSUC,
       THREQZ,
       BABYNUM,
       ZLLB,
       QJFF,
       BACK,
       SXFY,
       SYFY,
       WORKDATE,
       IFZDSS,
       ifbzzl,
       mzdoct,
       zlf_zy,
       (select top 1 ascard1 from ba8 where ba2.prn = ba8.prn and ba2.times = ba8.times) as ascard1
       

  FROM ba2
  where prn = '{0}' and times = '{1}'
";
            #endregion

            this.cmd.CommandText = string.Empty;

            try
            {
                strSQL = string.Format ( strSQL, cardno, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;

                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                if ( this.reader.Read ( ) )
                {
                    try
                    {
                        Neusoft.HISFC.Models.HealthRecord.Base b = new Neusoft.HISFC.Models.HealthRecord.Base ( );

                        //b.PatientInfo.ID = "BA" + times.PadLeft(2, '0') + cardno.PadLeft(10, '0');//住院流水号

                        b.PatientInfo.PID.PatientNO = this.reader [0].ToString ( ).PadLeft ( 10, '0' );//住院病历号

                        b.PatientInfo.PID.CardNO = this.reader [0].ToString ( ).PadLeft ( 10, '0' );//卡号

                        b.PatientInfo.Name = this.reader [1].ToString ( );//姓名

                        //s[4] = b.Nomen;//曾用名



                        b.PatientInfo.Profession.ID = this.reader [5].ToString ( );//职业

                        b.PatientInfo.BloodType.ID = this.reader [39].ToString ( );//血型编码

                        string mari = this.reader [6].ToString ( );

                        if ( mari.Trim ( ) == "1" )
                        {
                            b.PatientInfo.MaritalStatus.ID = "S";//婚否
                        }
                        else if ( mari.Trim ( ) == "2" )
                        {
                            b.PatientInfo.MaritalStatus.ID = "M";//婚否
                        }
                        else if ( mari.Trim ( ) == "3" )
                        {
                            b.PatientInfo.MaritalStatus.ID = "D";//婚否
                        }
                        else if ( mari.Trim ( ) == "4" )
                        {
                            b.PatientInfo.MaritalStatus.ID = "W";//婚否
                        }

                        string age = this.reader [4].ToString ( );
                        if ( age.Trim ( ) == "" )
                        {
                            b.PatientInfo.Age = "0";//年龄
                        }
                        else
                        {
                            try
                            {
                                b.AgeUnit = age.Substring ( 0, 1 );
                            }
                            catch
                            {
                            }

                            try
                            {
                                if ( Neusoft.FrameWork.Public.String.IsNumeric ( age.Substring ( 1 ) ) )
                                {
                                    b.PatientInfo.Age = age.Substring ( 1 );
                                }
                                else
                                {
                                    b.PatientInfo.Age = "0";
                                }
                            }
                            catch
                            {
                            }
                        }

                        //s[13] = b.AgeUnit;//年龄单位


                        b.PatientInfo.PVisit.InSource.ID = this.reader [20].ToString ( );//地区来源

                        //s[16] = b.PatientInfo.Pact.ID;//结算类别号
                        b.PatientInfo.Pact.ID = this.reader [3].ToString ( );

                        //s[17] = b.PatientInfo.Pact.ID;//合同代码

                        b.PatientInfo.SSN = this.reader [88].ToString ( );//医保公费号



                        b.PatientInfo.AddressHome = this.reader [11].ToString ( );//家庭住址

                        //s[22] = b.PatientInfo.PhoneHome;//家庭电话

                        b.PatientInfo.HomeZip = this.reader [12].ToString ( );//住址邮编

                        b.PatientInfo.AddressBusiness = this.reader [7].ToString ( );//单位地址

                        b.PatientInfo.PhoneBusiness = this.reader [9].ToString ( );//单位电话

                        b.PatientInfo.BusinessZip = this.reader [10].ToString ( );//单位邮编

                        b.PatientInfo.Kin.Name = this.reader [13].ToString ( );//联系人

                        b.PatientInfo.Kin.RelationLink = this.reader [14].ToString ( );//与患者关系

                        b.PatientInfo.Kin.RelationPhone = this.reader [16].ToString ( );//联系电话

                        b.PatientInfo.Kin.RelationAddress = this.reader [15].ToString ( );//联系地址

                        //s[31] = b.ClinicDoc.ID;//门诊诊断医生

                        b.ClinicDoc.Name = this.reader [86].ToString ( );//门诊诊断医生姓名

                        //s[33] = b.ComeFrom;//转来医院

                        b.PatientInfo.PVisit.InTime = this.GetDateTime ( this.reader [17].ToString ( ) );//入院日期

                        b.PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [2].ToString ( ) );//住院次数

                        b.InDept.ID = this.reader [21].ToString ( );//入院科室代码

                        b.InDept.Name = this.reader [22].ToString ( );//入院科室名称

                        b.PatientInfo.PVisit.InSource.ID = this.reader [20].ToString ( );//入院来源

                        b.PatientInfo.PVisit.Circs.ID = this.reader [19].ToString ( );//入院状态

                        b.DiagDate = this.GetDateTime ( this.reader [32].ToString ( ) );//确诊日期

                        //s[41] = b.OperationDate.ToString();//手术日期

                        b.PatientInfo.PVisit.OutTime = this.GetDateTime ( this.reader [27].ToString ( ) );//出院日期

                        b.OutDept.ID = this.reader [25].ToString ( );//出院科室代码

                        b.OutDept.Name = this.reader [26].ToString ( );//出院科室名称

                        //s[45] = b.PatientInfo.PVisit.ZG.ID;//转归代码

                        //b.DiagDays.ToString() = this.reader[32].ToString();//确诊天数

                        b.InHospitalDays = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [29].ToString ( ) );//住院天数

                        //s[48] = b.DeadDate.ToString();//死亡日期

                        //s[49] = b.DeadReason;//死亡原因

                        b.CadaverCheck = this.reader [44].ToString ( );//尸检

                        //s[51] = b.DeadKind;//死亡种类

                        //s[52] = b.BodyAnotomize;//尸体解剖号

                        //b.Hbsag = this.reader[96].ToString();//乙肝表面抗原

                        //b.HcvAb = this.reader[97].ToString();//丙肝病毒抗体

                        //b.HivAb = this.reader[98].ToString();//获得性人类免疫缺陷病毒抗体

                        b.CePi = this.reader [67].ToString ( );//门急_入院符合

                        b.PiPo = this.reader [68].ToString ( );//入出_院符合

                        b.OpbOpa = this.reader [69].ToString ( );//术前_后符合

                        //s[59] = b.ClX;//临床_X光符合

                        //s[60] = b.ClCt;//临床_CT符合

                        //s[61] = b.ClMri;//临床_MRI符合

                        b.ClPa = this.reader [70].ToString ( );//临床_病理符合

                        b.FsBl = this.reader [71].ToString ( );//放射_病理符合

                        b.SalvTimes = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [40].ToString ( ) );//抢救次数

                        b.SuccTimes = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [41].ToString ( ) );//成功次数

                        b.TechSerc = this.reader [60].ToString ( );//示教科研

                        b.VisiStat = this.reader [42].ToString ( );//是否随诊

                        b.VisiPeriodWeek = this.reader [43].ToString ( );//随访期限

                        //b.InconNum = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[112].ToString());//院际会诊次数 70 远程会诊次数

                        //b.OutconNum = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[113].ToString());//院际会诊次数 70 远程会诊次数

                        //s[71] = b.AnaphyFlag;//药物过敏

                        b.FirstAnaphyPharmacy.Name = this.reader [38].ToString ( );//过敏药物名称

                        //s[73] = b.SecondAnaphyPharmacy.ID;//过敏药物名称

                        //s[74] = b.CoutDate.ToString();//更改后出院日期

                        //s[75] = b.PatientInfo.PVisit.AdmittingDoctor.ID;//住院医师代码

                        b.PatientInfo.PVisit.AdmittingDoctor.Name = this.reader [64].ToString ( );//住院医师姓名

                        //s[77] = b.PatientInfo.PVisit.AttendingDoctor.ID;//主治医师代码

                        b.PatientInfo.PVisit.AttendingDoctor.Name = this.reader [63].ToString ( );//主治医师姓名

                        //s[79] = b.PatientInfo.PVisit.ConsultingDoctor.ID;//主任医师代码

                        b.PatientInfo.PVisit.ConsultingDoctor.Name = this.reader [62].ToString ( );//主任医师姓名

                        //s[81] = b.PatientInfo.PVisit.ReferringDoctor.ID;//科主任代码

                        //b.PatientInfo.PVisit.ReferringDoctor.Name = this.reader[99].ToString();//科主任名称

                        //s[83] = b.RefresherDoc.ID;//进修医师代码

                        //b.RefresherDoc.Name = this.reader[100].ToString();//进修医生名称

                        //s[85] = b.GraduateDoc.ID;//研究生实习医师代码

                        //b.GraduateDoc.Name = this.reader[101].ToString();//研究生实习医师名称

                        //s[87] = b.PatientInfo.PVisit.TempDoctor.ID;//实习医师代码

                        b.PatientInfo.PVisit.TempDoctor.Name = this.reader [65].ToString ( );//实习医师名称

                        //s[89] = b.CodingOper.ID;//编码员代码

                        b.CodingOper.Name = this.reader [66].ToString ( );//编码员名称
                        //b.OperationCoding.Name = b.codingCode.Name;

                        b.MrQuality = this.reader [61].ToString ( );//病案质量

                        //s[92] = b.MrEligible;//合格病案

                        //s[93] = b.QcDoc.ID;//质控医师代码

                        //b.QcDoc.Name = this.reader[102].ToString();//质控医师名称

                        //s[95] = b.QcNurse.ID;//质控护士代码

                        //b.QcNurse.Name = this.reader[103].ToString();//质控护士名称

                        //b.CheckDate = this.GetDateTime(this.reader[104].ToString());//检查时间

                        //b.YnFirst = this.reader[106].ToString();//手术操作治疗检查诊断为本院第一例项目

                        // b.RhBlood = this.reader[105].ToString();//Rh血型(阴阳)

                        b.ReactionBlood = this.reader [81].ToString ( );//输血反应（有无）

                        //b.BloodRed = this.reader[107].ToString();//红细胞数

                        //b.BloodPlatelet = this.reader[108].ToString();//血小板数

                        //b.BodyAnotomize = this.reader[109].ToString();//血浆数

                        //b.BloodWhole = this.reader[110].ToString();//全血数

                        //b.BloodOther = this.reader[111].ToString();//其他输血数

                        //s[106] = b.XNum;//X光号

                        //s[107] = b.CtNum;//CT号

                        //s[108] = b.MriNum;//MRI号

                        //s[109] = b.PathNum;//病理号

                        //s[110] = b.DsaNum;//DSA号

                        //s[111] = b.PetNum;//PET号

                        //s[112] = b.EctNum;//ECT号

                        //s[113] = b.XQty.ToString();//X线次数

                        //s[114] = b.CTQty.ToString();//CT次数

                        //s[115] = b.MRQty.ToString();//MR次数

                        //s[116] = b.DSAQty.ToString();//DSA次数

                        //s[117] = b.PetQty.ToString();//PET次数

                        //s[118] = b.EctQty.ToString();//ECT次数

                        //s[119] = b.PatientInfo.Memo;//说明

                        //s[120] = b.BarCode;//归档条码号

                        //s[121] = b.LendStat;//病案借阅状态(O借出 I在架)

                        b.PatientInfo.CaseState = "3";//病案状态1科室质检2登记保存3整理4病案室质检5无效

                        //s[123] = b.OperInfo.ID;//操作员

                        b.OperInfo.OperTime = this.GetDateTime ( this.reader [83].ToString ( ) );//操作时间
                        //s[124] = b.VisiPeriodWeek; //随访期限 周
                        //s[125] = b.VisiPeriodMonth; //随访期限 月
                        //s[126] = b.VisiPeriodYear;//随访期限 年
                        //try
                        //{
                        //    b.SpecalNus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[114].ToString());  // 特殊护理(日)                                        
                        //}
                        //catch
                        //{
                        //}
                        //try
                        //{
                        //    b.INus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[115].ToString()); //I级护理时间(日)  
                        //}
                        //catch
                        //{
                        //}
                        //try
                        //{
                        //    b.IINus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[116].ToString()); //II级护理时间(日)  
                        //}
                        //catch
                        //{
                        //}
                        //try
                        //{
                        //    b.IIINus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[117].ToString()); //III级护理时间(日)   
                        //}
                        //catch
                        //{
                        //}
                        //try
                        //{
                        //    b.StrictNuss = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[118].ToString()); //重症监护时间( 小时) 
                        //}
                        //catch
                        //{
                        //}
                        //try
                        //{
                        //    b.SuperNus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[119].ToString()); //特级护理时间(小时) 
                        //}
                        //catch
                        //{
                        //}
                        //s[133] = b.PackupMan.ID; //整理员
                        //s[134] = b.Disease30; //单病种 
                        //s[135] = b.IsHandCraft;//手工录入病案 标志
                        //s[136] = b.SyndromeFlag; //是否有并犯症
                        //s[137] = b.InfectionNum.ToString();//院内感染次数 
                        //s[138] = b.OperationCoding.ID;//手术编码员 
                        b.CaseNO = b.PatientInfo.PID.CardNO.PadLeft ( 10, '0' );//病案号
                        //s[140] = b.InfectionPosition.ID; //院内感染部位编码
                        //s[141] = b.InfectionPosition.Name; //院内感染部位名称*/

                        b.SyndromeFlag = this.reader [35].ToString ( );//是否有并犯症
                        b.InfectionNum = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [36].ToString ( ) ); //院内感染次数
                        b.Disease30 = this.reader [85].ToString ( );
                        b.LendStat = this.reader [73].ToString ( );//部分病种

                        this.GetpatientFromba1 ( b, cardno );
                        this.GetpatientFromba6 ( b, cardno, times );

                        return b;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    this.Err1 = "没读到！";

                    return null;
                }
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }


            /*
             * s[0] = b.PatientInfo.ID;//住院流水号

                s[1] = b.PatientInfo.PID.PatientNO;//住院病历号

                s[2] = b.PatientInfo.PID.CardNO;//卡号

                s[3] = b.PatientInfo.Name;//姓名

                s[4] = b.Nomen;//曾用名

                s[5] = b.PatientInfo.Sex.ID.ToString();//性别

                s[6] = b.PatientInfo.Birthday.ToString();//出生日期

                s[7] = b.PatientInfo.Country.ID;//国家

                s[8] = b.PatientInfo.Nationality.ID;//民族

                s[9] = b.PatientInfo.Profession.ID;//职业

                s[10] = b.PatientInfo.BloodType.ID.ToString();//血型编码

                s[11] = b.PatientInfo.MaritalStatus.ID.ToString();//婚否

                s[12] = b.PatientInfo.Age.ToString();//年龄

                s[13] = b.AgeUnit;//年龄单位

                s[14] = b.PatientInfo.IDCard;//身份证号

                s[15] = b.PatientInfo.PVisit.InSource.ID;//地区来源

                s[16] = b.PatientInfo.Pact.ID;//结算类别号

                s[17] = b.PatientInfo.Pact.ID;//合同代码

                s[18] = b.PatientInfo.SSN;//医保公费号

                s[19] = b.PatientInfo.DIST;//籍贯

                s[20] = b.PatientInfo.AreaCode;//出生地

                s[21] = b.PatientInfo.AddressHome;//家庭住址

                s[22] = b.PatientInfo.PhoneHome;//家庭电话

                s[23] = b.PatientInfo.HomeZip;//住址邮编

                s[24] = b.PatientInfo.AddressBusiness;//单位地址

                s[25] = b.PatientInfo.PhoneBusiness;//单位电话

                s[26] = b.PatientInfo.BusinessZip;//单位邮编

                s[27] = b.PatientInfo.Kin.Name;//联系人

                s[28] = b.PatientInfo.Kin.RelationLink;//与患者关系

                s[29] = b.PatientInfo.Kin.RelationPhone;//联系电话

                s[30] = b.PatientInfo.Kin.RelationAddress;//联系地址

                s[31] = b.ClinicDoc.ID;//门诊诊断医生

                s[32] = b.ClinicDoc.Name;//门诊诊断医生姓名

                s[33] = b.ComeFrom;//转来医院

                s[34] = b.PatientInfo.PVisit.InTime.ToString();//入院日期

                s[35] = b.PatientInfo.InTimes.ToString();//住院次数

                s[36] = b.InDept.ID;//入院科室代码

                s[37] = b.InDept.Name;//入院科室名称

                s[38] = b.PatientInfo.PVisit.InSource.ID;//入院来源

                s[39] = b.PatientInfo.PVisit.Circs.ID;//入院状态

                s[40] = b.DiagDate.ToString();//确诊日期

                s[41] = b.OperationDate.ToString();//手术日期

                s[42] = b.PatientInfo.PVisit.OutTime.ToString();//出院日期

                s[43] = b.OutDept.ID;//出院科室代码

                s[44] = b.OutDept.Name;//出院科室名称

                s[45] = b.PatientInfo.PVisit.ZG.ID;//转归代码

                s[46] = b.DiagDays.ToString();//确诊天数

                s[47] = b.InHospitalDays.ToString();//住院天数

                s[48] = b.DeadDate.ToString();//死亡日期

                s[49] = b.DeadReason;//死亡原因

                s[50] = b.CadaverCheck;//尸检

                s[51] = b.DeadKind;//死亡种类

                s[52] = b.BodyAnotomize;//尸体解剖号

                s[53] = b.Hbsag;//乙肝表面抗原

                s[54] = b.HcvAb;//丙肝病毒抗体

                s[55] = b.HivAb;//获得性人类免疫缺陷病毒抗体

                s[56] = b.CePi;//门急_入院符合

                s[57] = b.PiPo;//入出_院符合

                s[58] = b.OpbOpa;//术前_后符合

                s[59] = b.ClX;//临床_X光符合

                s[60] = b.ClCt;//临床_CT符合

                s[61] = b.ClMri;//临床_MRI符合

                s[62] = b.ClPa;//临床_病理符合

                s[63] = b.FsBl;//放射_病理符合

                s[64] = b.SalvTimes.ToString();//抢救次数

                s[65] = b.SuccTimes.ToString();//成功次数

                s[66] = b.TechSerc;//示教科研

                s[67] = b.VisiStat;//是否随诊

                s[68] = b.VisiPeriod.ToString();//随访期限

                s[69] = b.InconNum.ToString();//院际会诊次数 70 远程会诊次数

                s[70] = b.OutconNum.ToString();//院际会诊次数 70 远程会诊次数

                s[71] = b.AnaphyFlag;//药物过敏

                s[72] = b.FirstAnaphyPharmacy.Name;//过敏药物名称

                s[73] = b.SecondAnaphyPharmacy.ID;//过敏药物名称

                s[74] = b.CoutDate.ToString();//更改后出院日期

                s[75] = b.PatientInfo.PVisit.AdmittingDoctor.ID;//住院医师代码

                s[76] = b.PatientInfo.PVisit.AdmittingDoctor.Name;//住院医师姓名

                s[77] = b.PatientInfo.PVisit.AttendingDoctor.ID;//主治医师代码

                s[78] = b.PatientInfo.PVisit.AttendingDoctor.Name;//主治医师姓名

                s[79] = b.PatientInfo.PVisit.ConsultingDoctor.ID;//主任医师代码

                s[80] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名

                s[81] = b.PatientInfo.PVisit.ReferringDoctor.ID;//科主任代码

                s[82] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称

                s[83] = b.RefresherDoc.ID;//进修医师代码

                s[84] = b.RefresherDoc.Name;//进修医生名称

                s[85] = b.GraduateDoc.ID;//研究生实习医师代码

                s[86] = b.GraduateDoc.Name;//研究生实习医师名称

                s[87] = b.PatientInfo.PVisit.TempDoctor.ID;//实习医师代码

                s[88] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称

                s[89] = b.CodingOper.ID;//编码员代码

                s[90] = b.CodingOper.Name;//编码员名称

                s[91] = b.MrQuality;//病案质量

                s[92] = b.MrEligible;//合格病案

                s[93] = b.QcDoc.ID;//质控医师代码

                s[94] = b.QcDoc.Name;//质控医师名称

                s[95] = b.QcNurse.ID;//质控护士代码

                s[96] = b.QcNurse.Name;//质控护士名称

                s[97] = b.CheckDate.ToString();//检查时间

                s[98] = b.YnFirst;//手术操作治疗检查诊断为本院第一例项目

                s[99] = b.RhBlood;//Rh血型(阴阳)

                s[100] = b.ReactionBlood;//输血反应（有无）

                s[101] = b.BloodRed;//红细胞数

                s[102] = b.BloodPlatelet;//血小板数

                s[103] = b.BodyAnotomize;//血浆数

                s[104] = b.BloodWhole;//全血数

                s[105] = b.BloodOther;//其他输血数

                s[106] = b.XNum;//X光号

                s[107] = b.CtNum;//CT号

                s[108] = b.MriNum;//MRI号

                s[109] = b.PathNum;//病理号

                s[110] = b.DsaNum;//DSA号

                s[111] = b.PetNum;//PET号

                s[112] = b.EctNum;//ECT号

                s[113] = b.XQty.ToString();//X线次数

                s[114] = b.CTQty.ToString();//CT次数

                s[115] = b.MRQty.ToString();//MR次数

                s[116] = b.DSAQty.ToString();//DSA次数

                s[117] = b.PetQty.ToString();//PET次数

                s[118] = b.EctQty.ToString();//ECT次数

                s[119] = b.PatientInfo.Memo;//说明

                s[120] = b.BarCode;//归档条码号

                s[121] = b.LendStat;//病案借阅状态(O借出 I在架)

                s[122] = b.PatientInfo.CaseState;//病案状态1科室质检2登记保存3整理4病案室质检5无效

                s[123] = b.OperInfo.ID;//操作员

                //				s[124]=b.OperDate.ToString() ;//操作时间
                s[124] = b.VisiPeriodWeek; //随访期限 周
                s[125] = b.VisiPeriodMonth; //随访期限 月
                s[126] = b.VisiPeriodYear;//随访期限 年
                s[127] = b.SpecalNus.ToString();  // 特殊护理(日)                                        
                s[128] = b.INus.ToString(); //I级护理时间(日)                                     
                s[129] = b.IINus.ToString(); //II级护理时间(日)                                    
                s[130] = b.IIINus.ToString(); //III级护理时间(日)                                   
                s[131] = b.StrictNuss.ToString(); //重症监护时间( 小时)                                 
                s[132] = b.SuperNus.ToString(); //特级护理时间(小时)     
                s[133] = b.PackupMan.ID; //整理员
                s[134] = b.Disease30; //单病种 
                s[135] = b.IsHandCraft;//手工录入病案 标志
                s[136] = b.SyndromeFlag; //是否有并犯症
                s[137] = b.InfectionNum.ToString();//院内感染次数 
                s[138] = b.OperationCoding.ID;//手术编码员 
                s[139] = b.CaseNO;//病案号
                s[140] = b.InfectionPosition.ID; //院内感染部位编码
                s[141] = b.InfectionPosition.Name; //院内感染部位名称
             */

        }

        public int GetpatientFromba6 ( Neusoft.HISFC.Models.HealthRecord.Base b,
            string cardno, string times )
        {
            #region SQL

            string strSQL = @"select hbsag,   
	hcv_ab,
	hiv_ab,
	kzr,  
   	jxdoct,
        ysxdoct,
        zkdoct,
        zknurse,
        zkrq, 
        rh,
        ISOPFIRST,
        redcell,
        plaque,
        serous,
        ALLBLOOD,
        OTHERBLOOD,
        HZ_YJ,  
        HZ_YC,
        HL_TJ,
        HL_I,
        HL_II,
        HL_III,
        HL_ZZ,
        HL_TS
from ba6
where prn = '{0}' and times = '{1}'
";
            #endregion

            this.cmd.CommandText = string.Empty;

            try
            {
                strSQL = string.Format ( strSQL, cardno, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;

                return -1;
            }
            if ( !this.reader.IsClosed )
            {
                this.reader.Close ( );
            }
            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                if ( this.reader.Read ( ) )
                {
                    try
                    {


                        b.Hbsag = this.reader [0].ToString ( );//乙肝表面抗原

                        b.HcvAb = this.reader [1].ToString ( );//丙肝病毒抗体

                        b.HivAb = this.reader [2].ToString ( );//获得性人类免疫缺陷病毒抗体

                        b.InconNum = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [16].ToString ( ) );//院际会诊次数 70 远程会诊次数

                        b.OutconNum = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [17].ToString ( ) );//院际会诊次数 70 远程会诊次数

                        b.PatientInfo.PVisit.ReferringDoctor.Name = this.reader [3].ToString ( );//科主任名称

                        //s[83] = b.RefresherDoc.ID;//进修医师代码

                        b.RefresherDoc.Name = this.reader [4].ToString ( );//进修医生名称

                        //s[85] = b.GraduateDoc.ID;//研究生实习医师代码

                        b.GraduateDoc.Name = this.reader [5].ToString ( );//研究生实习医师名称

                        b.QcDoc.Name = this.reader [6].ToString ( );//质控医师名称

                        //s[95] = b.QcNurse.ID;//质控护士代码

                        b.QcNurse.Name = this.reader [7].ToString ( );//质控护士名称

                        b.CheckDate = this.GetDateTime ( this.reader [8].ToString ( ) );//检查时间

                        b.YnFirst = this.reader [10].ToString ( );//手术操作治疗检查诊断为本院第一例项目

                        b.RhBlood = this.reader [9].ToString ( );//Rh血型(阴阳)

                        b.BloodRed = this.reader [11].ToString ( );//红细胞数

                        b.BloodPlatelet = this.reader [12].ToString ( );//血小板数

                        b.BodyAnotomize = this.reader [13].ToString ( );//血浆数

                        b.BloodWhole = this.reader [14].ToString ( );//全血数

                        b.BloodOther = this.reader [15].ToString ( );//其他输血数

                        try
                        {
                            b.SpecalNus = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [23].ToString ( ) );  // 特殊护理(日)                                        
                        }
                        catch
                        {
                        }
                        try
                        {
                            b.INus = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [19].ToString ( ) ); //I级护理时间(日)  
                        }
                        catch
                        {
                        }
                        try
                        {
                            b.IINus = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [20].ToString ( ) ); //II级护理时间(日)  
                        }
                        catch
                        {
                        }
                        try
                        {
                            b.IIINus = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [21].ToString ( ) ); //III级护理时间(日)   
                        }
                        catch
                        {
                        }
                        try
                        {
                            b.StrictNuss = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [22].ToString ( ) ); //重症监护时间( 小时) 
                        }
                        catch
                        {
                        }
                        try
                        {
                            b.SuperNus = Neusoft.FrameWork.Function.NConvert.ToInt32 ( this.reader [18].ToString ( ) ); //特级护理时间(小时) 
                        }
                        catch
                        {
                        }

                        //return b;
                    }
                    catch ( Exception ex )
                    {
                        this.Err1 = ex.Message;
                        return -1;
                    }
                }

                //return b;
                return 1;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return -1;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        public int GetpatientFromba1 ( Neusoft.HISFC.Models.HealthRecord.Base b,
            string cardno )
        {
            #region SQL

            string strSQL = @"select birthday,
        native,
        nation,
        sex,
        idcard,
        birthpl
from ba1 where prn = '{0}'
";
            #endregion

            this.cmd.CommandText = string.Empty;

            try
            {
                strSQL = string.Format ( strSQL, cardno );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;

                return -1;
            }

            if ( !this.reader.IsClosed )
            {
                this.reader.Close ( );
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                if ( this.reader.Read ( ) )
                {
                    try
                    {
                        b.PatientInfo.Birthday = this.GetDateTime ( this.reader [0].ToString ( ) );//出生日期

                        //
                        b.PatientInfo.Country.Name = this.reader [1].ToString ( ).Trim ( );//国家

                        //
                        b.PatientInfo.Nationality.Name = this.reader [2].ToString ( ).Trim ( );//民族


                        string sex = this.reader [3].ToString ( ).Trim ( );
                        if ( sex.Trim ( ) == "1" )
                        {
                            b.PatientInfo.Sex.ID = "M";//性别
                        }
                        else if ( sex.Trim ( ) == "2" )
                        {
                            b.PatientInfo.Sex.ID = "F";//性别
                        }
                        else
                        {
                            b.PatientInfo.Sex.ID = "U";//性别
                        }

                        b.PatientInfo.IDCard = this.reader [4].ToString ( );//身份证号
                        b.PatientInfo.DIST = this.reader [5].ToString ( );//籍贯

                        b.PatientInfo.AreaCode = b.PatientInfo.DIST;//出生地
                        //return b;


                    }
                    catch ( Exception ex )
                    {
                        this.Err1 = ex.Message;
                        return -1;
                    }
                }
                return 1;
                //return b;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return -1;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// ba3
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public ArrayList GetICD ( string cardNo, string times )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"SELECT PRN, NAME, TIMES, ZDXH, ICDM, ZLJG, ICDM10 FROM ba3
 where prn = '{0}'
   and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.HealthRecord.Diagnose obj;

                while ( this.reader.Read ( ) )
                {
                    obj = new Neusoft.HISFC.Models.HealthRecord.Diagnose ( );
                    /*手术次数*/
                    obj.DiagInfo.DiagType.ID = this.reader [3].ToString ( );//对照

                    obj.DiagInfo.ICD10.ID = this.reader [6].ToString ( );
                    obj.DiagOutState = this.reader [5].ToString ( );//""/*疗效*/,
                    obj.OperType = "2";
                    al.Add ( obj );
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// ba3
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public ArrayList GetICDf ( string cardNo, string times )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"SELECT ZDXH, ICDM10, ICDM10_F, ZLJG
  FROM  ba3_f
 where prn = '{0}'
   and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.HealthRecord.Diagnose obj;

                while ( this.reader.Read ( ) )
                {
                    obj = new Neusoft.HISFC.Models.HealthRecord.Diagnose ( );

                    //obj.DiagInfo.DiagType.ID = this.reader[0].ToString();//对照

                    obj.DiagInfo.ICD10.ID = this.reader [1].ToString ( );
                    obj.SecondICD = this.reader [2].ToString ( );
                    //obj.DiagOutState = this.reader[3].ToString();//""/*疗效*/,
                    //obj.OperType = "2";
                    al.Add ( obj );
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        public ArrayList GetICDFromba6 ( string cardNo, string times )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"SELECT MZZD10,
       RYZD10
  FROM  ba6 
  where prn = '{0}' and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.HealthRecord.Diagnose obj;

                while ( this.reader.Read ( ) )
                {
                    string ryzd = this.reader [0].ToString ( ).Trim ( );
                    if ( ryzd != "" )
                    {
                        obj = new Neusoft.HISFC.Models.HealthRecord.Diagnose ( );
                        obj.DiagInfo.DiagType.ID = "10";
                        obj.DiagInfo.ICD10.ID = ryzd;
                        obj.OperType = "2";
                        al.Add ( obj );
                    }

                    string mzzd = this.reader [1].ToString ( ).Trim ( );
                    if ( mzzd != "" )
                    {
                        obj = new Neusoft.HISFC.Models.HealthRecord.Diagnose ( );
                        obj.DiagInfo.DiagType.ID = "11";
                        obj.DiagInfo.ICD10.ID = mzzd;
                        obj.OperType = "2";
                        al.Add ( obj );
                    }
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        public ArrayList GetICDFromba2 ( string cardNo, string times )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"select phzd , (select top 1 icd10 from icd10 where jbname = phzd) as a  from ba2 
  where prn = '{0}' and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.HealthRecord.Diagnose obj;

                if ( this.reader.Read ( ) )
                {
                    string blzdID = this.reader [1].ToString ( ).Trim ( );
                    string blzdName = this.reader [0].ToString ( ).Trim ( );
                    if ( blzdName != "" && blzdName != "无" )
                    {
                        obj = new Neusoft.HISFC.Models.HealthRecord.Diagnose ( );
                        obj.DiagInfo.DiagType.ID = "6";
                        obj.DiagInfo.ICD10.ID = blzdID;
                        if ( obj.DiagInfo.ICD10.ID.Trim ( ) == "" )
                        {
                            obj.DiagInfo.ICD10.ID = "BA";
                        }
                        obj.DiagInfo.ICD10.Name = blzdName;
                        obj.OperType = "2";
                        al.Add ( obj );
                    }
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// ba4
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public ArrayList GetOper ( string cardNo, string times )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"SELECT PRN,
       TIMES,
       NAME,
       OPTIMES,
       OPDATE,
       OPCODE,
       QIEKOU,
       YUHE,
       DOCNAME,
       MAZUI,
       IFFSOP,
       OPDOCTI,
       OPDOCTII,
       MZDOCT
  FROM  ba4 
  where prn = '{0}' and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.HealthRecord.OperationDetail obj;

                while ( this.reader.Read ( ) )
                {
                    obj = new Neusoft.HISFC.Models.HealthRecord.OperationDetail ( );
                    /*手术次数*/
                    obj.OperationDate = GetDateTime ( this.reader [4].ToString ( ) );
                    obj.OperationInfo.ID = this.reader [5].ToString ( );
                    obj.NickKind/*切口*/= this.reader [6].ToString ( );
                    obj.CicaKind/*愈合*/= this.reader [7].ToString ( );
                    obj.FirDoctInfo.Name = this.reader [8].ToString ( ).Trim ( );
                    obj.MarcKind = this.reader [9].ToString ( );//麻醉方式
                    /*是否附加手术*/
                    string temp = this.reader [10].ToString ( );
                    if ( temp.Trim ( ) == "y" || temp.Trim ( ) == "Y" )
                    {
                        obj.StatFlag = "1";
                    }
                    else
                    {
                        obj.StatFlag = "0";
                    }

                    obj.FourDoctInfo.Name = this.reader [11].ToString ( ).Trim ( );
                    obj.SecDoctInfo.Name = this.reader [12].ToString ( ).Trim ( );
                    //obj.ThrDoctInfo.Name,
                    obj.NarcDoctInfo.Name = this.reader [13].ToString ( ).Trim ( );
                    al.Add ( obj );
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// ba7
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public ArrayList GetChangeDept ( string cardNo, string times )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"SELECT  ZKNUM, ZKDEPT,  ZKDATE, ZKTIME
  FROM  ba7
 where prn = '{0}'
   and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.RADT.Location obj;

                while ( this.reader.Read ( ) )
                {
                    obj = new Neusoft.HISFC.Models.RADT.Location ( );

                    obj.Dept.ID = this.reader [0].ToString ( );//对照
                    obj.Dept.Name = this.reader [1].ToString ( );//""/*疗效*/,
                    obj.User01 = this.GetDateTime ( this.reader [2].ToString ( ) ).ToString ( );

                    al.Add ( obj );
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// 第一次转科
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.Location GetFristDept ( string cardNo, string times )
        {
            string strSQL = @"select ZKNUM,
       ZKDEPT,
       (select top 1 ZKDATE
          from ba6
         where prn = '{0}'
           and times = '{1}') as zkdate
  from ba2
 where prn = '{0}'
   and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.RADT.Location obj;

                if ( this.reader.Read ( ) )
                {
                    string deptID = this.reader [0].ToString ( ).Trim ( );

                    if ( deptID != "" )
                    {
                        obj = new Neusoft.HISFC.Models.RADT.Location ( );

                        obj.Dept.ID = deptID;//对照
                        obj.Dept.Name = this.reader [1].ToString ( );//""/*疗效*/,
                        obj.User01 = this.GetDateTime ( this.reader [2].ToString ( ) ).ToString ( );

                        return obj;
                    }
                }
                else
                {
                }

                return null;

            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// ba9
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Tumour GetTumout
            ( string cardNo, string times )
        {
            string strSQL = @"SELECT prn,
       name,
       times,
       flfs,
       flcs,
       flzz,
       ygy,
       ycs,
       yts,
       yrq1,
       yrq2,
       qgy,
       qcs,
       qts,
       qrq1,
       qrq2,
       zgy,
       zcs,
       zts,
       zrq1,
       zrq2,
       hlfs,
       hlff
  FROM ba9
  where prn = '{0}' and times = '{1}'
";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.HealthRecord.Tumour info = new Neusoft.HISFC.Models.HealthRecord.Tumour ( );

                if ( this.reader.Read ( ) )
                {
                    info.Rmodeid = this.reader [3].ToString ( );
                    info.Rprocessid = this.reader [4].ToString ( );
                    info.Rdeviceid = this.reader [5].ToString ( );
                    try
                    {
                        info.Gy1 = Convert.ToDecimal ( this.reader [6].ToString ( ) );
                    }
                    catch
                    {
                    }
                    try
                    {
                        info.Time1 = Convert.ToDecimal ( this.reader [7].ToString ( ) );
                    }
                    catch
                    {
                    }
                    try
                    {
                        info.Day1 = Convert.ToDecimal ( this.reader [8].ToString ( ) );
                    }
                    catch
                    {
                    }
                    info.BeginDate1 = GetDateTime ( this.reader [9].ToString ( ) );
                    info.EndDate1 = GetDateTime ( this.reader [10].ToString ( ) );
                    try
                    {
                        info.Gy2 = Convert.ToDecimal ( this.reader [11].ToString ( ) );
                    }
                    catch
                    {
                    }
                    try
                    {
                        info.Time2 = Convert.ToDecimal ( this.reader [12].ToString ( ) );
                    }
                    catch
                    {
                    }
                    try
                    {
                        info.Day2 = Convert.ToDecimal ( this.reader [13].ToString ( ) );
                    }
                    catch
                    {
                    }
                    info.BeginDate2 = GetDateTime ( this.reader [14].ToString ( ) );
                    info.EndDate2 = GetDateTime ( this.reader [15].ToString ( ) );
                    try
                    {
                        info.Gy3 = Convert.ToDecimal ( this.reader [16].ToString ( ) );
                    }
                    catch
                    {
                    }
                    try
                    {
                        info.Time3 = Convert.ToDecimal ( this.reader [17].ToString ( ) );
                    }
                    catch
                    {
                    }
                    try
                    {
                        info.Day3 = Convert.ToDecimal ( this.reader [18].ToString ( ) );
                    }
                    catch
                    {
                    }
                    info.BeginDate3 = GetDateTime ( this.reader [19].ToString ( ) );
                    info.EndDate3 = GetDateTime ( this.reader [20].ToString ( ) );
                    info.Cmodeid = this.reader [21].ToString ( );
                    info.Cmethod = this.reader [22].ToString ( );
                }
                else
                {
                    info.User01 = "0";
                }

                return info;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// ba10
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public ArrayList GetTumoutDetail ( string cardNo, string times )
        {
            string strSQL = @"SELECT prn, name, times, hldate1, hldate2, hldrug, hlproc, hljg
  FROM ba10 where prn = '{0}' and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            ArrayList al = new ArrayList ( );

            try
            {
                Neusoft.HISFC.Models.HealthRecord.TumourDetail info = new Neusoft.HISFC.Models.HealthRecord.TumourDetail ( );
                while ( this.reader.Read ( ) )
                {
                    info = new Neusoft.HISFC.Models.HealthRecord.TumourDetail ( );
                    try
                    {
                        info.CureDate = Neusoft.FrameWork.Function.NConvert.ToDateTime ( this.reader [3].ToString ( ).Replace ( '/', '-' ) + " 00:00:00" );
                    }
                    catch
                    {
                    }
                    try
                    {
                        info.OperInfo.OperTime = GetDateTime ( this.reader [4].ToString ( ) );
                    }
                    catch
                    {
                    }
                    info.DrugInfo.Name = this.reader [5].ToString ( );
                    info.Period = this.reader [6].ToString ( ).Trim ( );
                    info.Result = this.reader [7].ToString ( ).Trim ( );
                    al.Add ( info );
                }

                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// fees
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public ArrayList GetFeesFromba2 ( string cardNo, string times, ref decimal otherfee )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"SELECT SUM1,
       CWF,
       XYF,
       ZYF,
       ZCYF,
       ZCHF,
       JCF,
       ZLF,
       FSF,
       SSF,
       HYF,
       SXF,
       SYF,
       JSF,
       QTF
  FROM ba2 where prn = '{0}' and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.RADT.Patient info;

                if ( this.reader.Read ( ) )
                {
                    info = new Neusoft.HISFC.Models.RADT.Patient ( );

                    decimal cost = 0;

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [1].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "1";
                        info.AreaCode = "床位";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [2].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "3";
                        info.AreaCode = "西药";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [4].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "5";
                        info.AreaCode = "中草药";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [5].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "4";
                        info.AreaCode = "中成药";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [6].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "13";
                        info.AreaCode = "检查";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }


                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [7].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "10";
                        info.AreaCode = "诊疗";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [8].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "6";
                        info.AreaCode = "放射";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [9].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "11";
                        info.AreaCode = "手术";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [10].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "7";
                        info.AreaCode = "化验";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }


                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [11].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "2";
                        info.AreaCode = "护理费";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [12].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "8";
                        info.AreaCode = "输氧";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [14].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        //info.DIST = "17";
                        //info.AreaCode = "其他";
                        //info.IDCard = cost.ToString();

                        //al.Add(info);
                        otherfee = cost;
                    }


                    //info.DIST = row["统计编码"].ToString();//统计大类编码
                    //if (info.DIST == "" || info.DIST == null)
                    //{
                    //    continue;
                    //}
                    //info.AreaCode = row["费用名称"].ToString(); //统计名称 
                    //if (row["费用金额"] != DBNull.Value)
                    //{
                    //    info.IDCard = row["费用金额"].ToString();//统计费用 
                    //}
                    //feeInfoList.Add(info);

                    //al.Add(info);
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// fees
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public ArrayList GetFeesFromba6 ( string cardNo, string times, ref decimal otherfee )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"select HLF, MZF, YEF, PCF
  from ba6
 where prn = '{0}'
   and times = '{1}'";

            try
            {
                strSQL = string.Format ( strSQL, cardNo, times );
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL ( strSQL );

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.HISFC.Models.RADT.Patient info;

                if ( this.reader.Read ( ) )
                {
                    info = new Neusoft.HISFC.Models.RADT.Patient ( );

                    decimal cost = 0;

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [0].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "9";
                        info.AreaCode = "输血";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [1].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        info = new Neusoft.HISFC.Models.RADT.Patient ( );
                        info.DIST = "14";
                        info.AreaCode = "麻醉";
                        info.IDCard = cost.ToString ( );

                        al.Add ( info );
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [2].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        //info.DIST = "";
                        //info.AreaCode = "";
                        //info.IDCard = cost.ToString();

                        //al.Add(info);

                        otherfee += cost;
                    }

                    try
                    {
                        cost = Neusoft.FrameWork.Function.NConvert.ToDecimal ( this.reader [3].ToString ( ) );
                    }
                    catch
                    {
                        cost = 0;
                    }
                    if ( cost != 0 )
                    {
                        //info.DIST = "";
                        //info.AreaCode = "";
                        //info.IDCard = cost.ToString();

                        //al.Add(info);
                        otherfee += cost;
                    }


                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// 取需要导的数据
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAllCardAndTimes ( )
        {
            ArrayList al = new ArrayList ( );
            string strSQL = @"select prn,times from ba2 where prn = '0000001'";


            this.ReadSQL ( strSQL );
            //this.cmd.CommandText = strSQL;

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.FrameWork.Models.NeuObject obj;

                while ( this.reader.Read ( ) )
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject ( );
                    obj.ID = this.reader [0].ToString ( );
                    obj.Name = this.reader [1].ToString ( );
                    al.Add ( obj );
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        public ArrayList GetAllCardAndTimes ( string strSQL )
        {
            ArrayList al = new ArrayList ( );
            //string strSQL = @"select prn,times from ba2 where prn = '0000001'";


            this.ReadSQL ( strSQL );
            //this.cmd.CommandText = strSQL;

            this.reader = this.cmd.ExecuteReader ( );

            try
            {
                Neusoft.FrameWork.Models.NeuObject obj;

                while ( this.reader.Read ( ) )
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject ( );
                    obj.ID = this.reader [0].ToString ( );
                    obj.Name = this.reader [1].ToString ( );
                    al.Add ( obj );
                }
                return al;
            }
            catch ( Exception ex )
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close ( );
            }
        }

        /// <summary>
        /// 取时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private DateTime GetDateTime ( string str )
        {
            if ( str.Trim ( ) == "" )
            {
                return DateTime.MinValue;
            }
            try
            {
                return Neusoft.FrameWork.Function.NConvert.ToDateTime ( str.Replace ( '/', '-' ) + " 00:00:00" );
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        #endregion

        #region 写日志

        private string fileName = "./SQLServer.log";

        private void CreatFile()
        {
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.CreateText(fileName);
            }
        }

        private System.IO.TextWriter output;

        private void WriteLog(string log)
        {
            try
            {
                output = System.IO.File.AppendText(fileName);
                output.WriteLine(System.DateTime.Now + "\n" + log);
                output.Close();
            }
            catch
            {
                //System.w
            }
        }

        private void ReadSQL(string sql)
        {
            this.WriteLog(sql);
            this.cmd.CommandText = sql;          
        }

        #endregion

        /// <summary>
        /// 随访用的函数
        /// </summary>
        /// <param name="IcdStr">icd字符串</param>
        /// <returns></returns>
        public ArrayList GetPatientByICD(string IcdStr)
        {
            ArrayList al = new ArrayList();
            string strSQL = @"select distinct t.fprn from dbo.tpatientvisit  t ,dbo.tdiagnose  z
where t.fprn=z.fprn and t.ftimes=z.ftimes 
and t. fcydate>'2009-1-1 00:00:00' 
and z.ficdm in ({0})";

            try
            {
                strSQL = string.Format(strSQL, IcdStr);
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL(strSQL);

            this.reader = this.cmd.ExecuteReader();

            try
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();

                while (this.reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.reader[0].ToString().PadLeft(10, '0');
                    al.Add(obj);
                }
                return al;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }

        public Neusoft.HISFC.Models.RADT.PatientInfo GetPatientByIdAndTimes(string patient_NO ,int inTimes)
        {
            //string Sql = "select t.fprn,t.ftimes,t.fname from tPatientVisit t where  t. fprn='{0}' and t.ftimes={1} ";
            string Sql = "select t.fprn,t.ftimes,t.fname,t.FSEX,t.FRYTYKH,t.FRYDEPT,t.FRYDATE,t.FCYTYKH,t.FCYDEPT,t.FCYDATE,t.FBIRTHDAY from tPatientVisit t where   t. fprn='{0}' and t.ftimes={1} ";
            try
            {
                Sql = string.Format(Sql, patient_NO,inTimes);
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }

            ReadSQL(Sql);

            this.reader = this.cmd.ExecuteReader();

            try
            {
                Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

                while (this.reader.Read())
                {
                    patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patientInfo.ID = this.reader[0].ToString().PadLeft(10, '0');
                    patientInfo.PID.PatientNO = this.reader[0].ToString().PadLeft(10, '0');
                    patientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32( this.reader[1].ToString());
                    patientInfo.Name = this.reader[2].ToString();
                    patientInfo.Sex.Name = this.reader[3].ToString();
                    patientInfo.PVisit.PatientLocation.Dept.ID = this.reader[4].ToString();//入院科室统一号 广东省3.0系统
                    patientInfo.PVisit.PatientLocation.Dept.Name = this.reader[5].ToString();
                    patientInfo.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[6].ToString());
                    patientInfo.PVisit.PatientLocation.ID = this.reader[7].ToString();//出院统一号
                    patientInfo.PVisit.PatientLocation.Name = this.reader[8].ToString();
                    patientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[9].ToString());
                    patientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[10].ToString());
                }
                return patientInfo;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }

        public ArrayList GetPatientByName(string PatientName, int inTimes)
        {
            string Sql = "select t.fprn,t.ftimes,t.fname,t.FSEX,t.FRYTYKH,t.FRYDEPT,t.FRYDATE,t.FCYTYKH,t.FCYDEPT,t.FCYDATE,t.FBIRTHDAY,t.FLXNAME from tPatientVisit t where   t.fname='{0}' and (t.ftimes={1} or 'ALL'='{1}') ";
            try
            {
                Sql = string.Format(Sql, PatientName, inTimes);
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            ArrayList al = new ArrayList();
            ReadSQL(Sql);

            this.reader = this.cmd.ExecuteReader();

            try
            {
                Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

                while (this.reader.Read())
                {
                    patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patientInfo.ID = this.reader[0].ToString().PadLeft(10, '0');
                    patientInfo.PID.PatientNO = this.reader[0].ToString().PadLeft(10, '0');
                    patientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[1].ToString());
                    patientInfo.Name = this.reader[2].ToString();
                    patientInfo.Sex.Name = this.reader[3].ToString();
                    patientInfo.PVisit.PatientLocation.Dept.ID = this.reader[4].ToString();//入院科室统一号 广东省3.0系统
                    patientInfo.PVisit.PatientLocation.Dept.Name = this.reader[5].ToString();
                    patientInfo.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[6].ToString());
                    patientInfo.PVisit.PatientLocation.ID = this.reader[7].ToString();//出院统一号
                    patientInfo.PVisit.PatientLocation.Name = this.reader[8].ToString();
                    patientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[9].ToString());
                    patientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[10].ToString());
                    patientInfo.Kin.Relation.Name = this.reader[11].ToString();
                    al.Add(patientInfo);
                }
                return al;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }

        public ArrayList GetPatientByOutDate(DateTime dtBegin, DateTime dtEnd)
        {
            string Sql = "select t.fprn,t.ftimes,t.fname,t.FSEX,t.FRYTYKH,t.FRYDEPT,t.FRYDATE,t.FCYTYKH,t.FCYDEPT,t.FCYDATE,t.FBIRTHDAY,t.FLXNAME ,t.FWORKRQ from tpatientvisit t where t.fcydate BETWEEN '{0}' and '{1}'";
            try
            {
                Sql = string.Format(Sql, dtBegin, dtEnd);
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            ArrayList al = new ArrayList();
            ReadSQL(Sql);

            this.reader = this.cmd.ExecuteReader();

            try
            {
                Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

                while (this.reader.Read())
                {
                    patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patientInfo.ID = this.reader[0].ToString().PadLeft(10, '0');
                    patientInfo.PID.PatientNO = this.reader[0].ToString().PadLeft(10, '0');
                    patientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[1].ToString());
                    patientInfo.Name = this.reader[2].ToString();
                    patientInfo.Sex.Name = this.reader[3].ToString();
                    patientInfo.PVisit.PatientLocation.Dept.ID = this.reader[4].ToString();//入院科室统一号 广东省3.0系统
                    patientInfo.PVisit.PatientLocation.Dept.Name = this.reader[5].ToString();
                    patientInfo.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[6].ToString());
                    patientInfo.PVisit.PatientLocation.ID = this.reader[7].ToString();//出院统一号
                    patientInfo.PVisit.PatientLocation.Name = this.reader[8].ToString();
                    patientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[9].ToString());
                    patientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[10].ToString());
                    patientInfo.Kin.Relation.Name = this.reader[11].ToString();
                    patientInfo.PVisit.PreOutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.reader[12].ToString());//操作日期
                    al.Add(patientInfo);
                }
                return al;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }

        public List<Neusoft.HISFC.Models.HealthRecord.Case.CaseStore> GetPatientByCaseNo(string strBegin, string strEnd)
        {
            string Sql = "select distinct  fprn,fname,ftimes from dbo.tpatientvisit where fprn>='{0}' and fprn<='{1}'";
            try
            {
                Sql = string.Format(Sql, strBegin, strEnd);
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            List<Neusoft.HISFC.Models.HealthRecord.Case.CaseStore> list = new List<Neusoft.HISFC.Models.HealthRecord.Case.CaseStore>();
            ReadSQL(Sql);

            this.reader = this.cmd.ExecuteReader();

            try
            {
                Neusoft.HISFC.Models.HealthRecord.Case.CaseStore obj = new Neusoft.HISFC.Models.HealthRecord.Case.CaseStore();
                while (this.reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.HealthRecord.Case.CaseStore();
                    obj.PatientInfo.ID = this.reader[0].ToString().PadLeft(10, '0');
                    obj.PatientInfo.PID.PatientNO = this.reader[0].ToString().PadLeft(10, '0');
                    obj.PatientInfo.Name = this.reader[1].ToString();
                    obj.PatientInfo.InTimes =Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[2].ToString());

                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }


        /// <summary>
        /// 获得3.0科室表数组
        /// </summary>
        /// <returns></returns>
        public List<Neusoft.FrameWork.Models.NeuObject> GetDeptCodeByCode()
        {

            string Sql = "select ftykh ,fksname,fkh  from tWorkroom ";
            //try
            //{
            //    Sql = string.Format(Sql, Code);
            //}
            //catch (Exception ex)
            //{
            //    this.Err1 = ex.Message;
            //    return null;
            //}
            List<Neusoft.FrameWork.Models.NeuObject> list = new List<Neusoft.FrameWork.Models.NeuObject>();
            ReadSQL(Sql);
            this.reader = this.cmd.ExecuteReader();
            try
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                while (this.reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.reader[0].ToString();
                    obj.Name = this.reader[1].ToString();
                    obj.Memo = this.reader[2].ToString();
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }
        /// <summary>
        /// 获得3.0专科科室表数组
        /// </summary>
        /// <returns></returns>
        public List<Neusoft.FrameWork.Models.NeuObject> GetZkDeptCodeByCode()
        {

            string Sql = "select  ftyzkcode,fzkname,fzkcode from tSpecialRoom";
            List<Neusoft.FrameWork.Models.NeuObject> list = new List<Neusoft.FrameWork.Models.NeuObject>();
            ReadSQL(Sql);
            this.reader = this.cmd.ExecuteReader();
            try
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                while (this.reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.reader[0].ToString();
                    obj.Name = this.reader[1].ToString();
                    obj.Memo = this.reader[2].ToString();
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }
        /// <summary>
        /// 获得3.0医生表数组
        /// </summary>
        /// <returns></returns>
        public List<Neusoft.FrameWork.Models.NeuObject> GetDocCodeByCode()
        {

            string Sql = "select ftygh,fname,fgh from tdoctor ";
            List<Neusoft.FrameWork.Models.NeuObject> list = new List<Neusoft.FrameWork.Models.NeuObject>();
            ReadSQL(Sql);
            this.reader = this.cmd.ExecuteReader();
            try
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                while (this.reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.reader[0].ToString();
                    obj.Name = this.reader[1].ToString();
                    obj.Memo = this.reader[2].ToString();
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }
        /// <summary>
        /// 获得 3.0顺序录入患者 用于打印条码
        /// </summary>
        /// <param name="dtBegin">开始日期</param>
        /// <param name="dtEnd">结束日期</param>
        /// <param name="operCode">录入操作员号</param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.RADT.PatientInfo> GetPatientBarCode(DateTime dtBegin ,DateTime dtEnd ,string operCode)
        {

            string Sql = @" select  t.fprn,t.ftimes,t.fcydept,t.fname from tpatientvisit t 
                            where t.fworkrq between '{0}'  and  '{1}' 
                            and ( fsry='{2}'  or 'ALL'='{2}')
                            order by t.fid";
            try
            {
                Sql = string.Format(Sql, dtBegin, dtEnd, operCode);
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            List<Neusoft.HISFC.Models.RADT.PatientInfo> list = new List<Neusoft.HISFC.Models.RADT.PatientInfo>();
            ReadSQL(Sql);
            this.reader = this.cmd.ExecuteReader();
            try
            {
                Neusoft.HISFC.Models.RADT.PatientInfo obj = new Neusoft.HISFC.Models.RADT.PatientInfo();
                while (this.reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    obj.PID.PatientNO = this.reader[0].ToString();
                    obj.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.reader[1].ToString());
                    obj.PVisit.PatientLocation.Name = this.reader[2].ToString();
                    obj.Name = this.reader[3].ToString();
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }

        /// <summary>
        ///  获得3.0 ICD10数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.HealthRecord.ICD> GetICDCodeByType(Neusoft.HISFC.Models.HealthRecord.EnumServer.ICDTypes type,string icdVersion)
        {
            string Sql ="";
            if (type == Neusoft.HISFC.Models.HealthRecord.EnumServer.ICDTypes.ICD10)
            {
                if (icdVersion == "10")
                {
                    Sql = "select FId,FICDM,FJBNAME,FTjm from tICD where FICDVersion='10' ";
                }
                else
                {
                    Sql = "select FId,FICDM,FJBNAME,FTjm from tICD where FICDVersion='11' or FICDVersion='12'";
                }
            }
            else if (type == Neusoft.HISFC.Models.HealthRecord.EnumServer.ICDTypes.ICD9)
            {
                Sql = "select FId,FICDM,FJBNAME,FTjm from tICD where FICDVersion='9' ";
            }
            else
            {
                Sql = "select FId,FOpcode,FOpname,FZjc from tOperate  ";
            }
            List<Neusoft.HISFC.Models.HealthRecord.ICD> list = new List<Neusoft.HISFC.Models.HealthRecord.ICD>();
            ReadSQL(Sql);
            this.reader = this.cmd.ExecuteReader();
            try
            {
                Neusoft.HISFC.Models.HealthRecord.ICD obj = new  Neusoft.HISFC.Models.HealthRecord.ICD();
                while (this.reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.HealthRecord.ICD();
                    obj.ID = this.reader[0].ToString();
                    obj.Name = this.reader[1].ToString();
                    obj.Memo = this.reader[2].ToString();
                    obj.DiseaseCode = this.reader[3].ToString();
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return null;
            }
            finally
            {
                this.reader.Close();
            }
        }


        #region drgs接口整理  2012-6-26
        #region HIS_BA1
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
            string strSQL = this.GetInsertHISBA1SQLDrgs(b, Feeds, alChangeDepe, alDose, isMetCasBase);
            if (strSQL == null || strSQL == "")
            {
                return -1;
            }
            ReadSQL(strSQL);
            int intReturn = this.cmd.ExecuteNonQuery();
            if (intReturn < 0)
            {
                return -1;
            }
            return intReturn;
        }

        /// <summary>
        /// 接口HIS_BA1 INSERT SQL 赋值
        /// </summary>
        /// <param name="b">病案首页的实体类</param>
        /// <param name="Feeds">费用信息数组</param>
        /// <param name="alChangeDepe">转科信息数组</param>
        /// <param name="alDose">诊断信息数组</param>
        /// <param name="isMetCasBase">true病案首页信息 false 住院主表信息</param>
        /// <returns>失败返回null</returns>
        private string[] GetBaseInfoBA1Drgs(Neusoft.HISFC.Models.HealthRecord.Base b, DataSet Feeds,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            //ArrayList al = this.baseDml.QueryHealthRecordCaseinfo(b.PatientInfo.ID);
            if (isMetCasBase)
            {
                #region
                string[] s = new string[194];
                try
                {
                    s[0] = "0";//是否输入，0：否，1：是，默认接收为0
                    string patientNO = b.PatientInfo.PID.PatientNO.PadLeft(10, '0');
                    s[1] = this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr()));//病案号
                    s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
                    s[3] = "11";//ICD版本，9：ICD9字典库，10：ICD10字典库，待扩展11：国家ICD库，默认接收为11
                    s[4] = b.PatientInfo.ID;//住院流水号
                    #region //s[5] 年龄
                    if (b.PatientInfo.Age != "" && b.PatientInfo.Age != "0")
                    {
                        if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") < 0) //整岁
                        {
                            s[5] = "Y" + b.AgeUnit.Replace("岁", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//整月
                        {
                            s[5] = "M" + b.AgeUnit.Replace("月", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") < 0 && b.AgeUnit.IndexOf("天") > 0)//整天
                        {
                            s[5] = "D" + b.AgeUnit.Replace("天", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//N岁N月
                        {
                            string[] PAge = b.AgeUnit.Split('岁');
                            s[5] = "Y" + PAge[0] + "M" + PAge[1].Replace("岁", "").Replace("月", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N月N天
                        {
                            string[] PAge = b.AgeUnit.Split('月');
                            s[5] = "M" + PAge[0] + "D" + PAge[1].Replace("月", "").Replace("天", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N岁N月N天
                        {
                            string[] PAge = b.AgeUnit.Split('岁');

                            string[] PAge1 = PAge[1].Split('月');
                            s[5] = "Y" + PAge[0] + "M" + PAge1[0] + "D" + PAge1[1].Replace("月", "").Replace("天", "");
                        }
                    }
                    else
                    {
                        int ts = b.PatientInfo.PVisit.InTime.Year - b.PatientInfo.Birthday.Year;

                        if (ts == 0)
                        {
                            ts = b.PatientInfo.PVisit.InTime.Month - b.PatientInfo.Birthday.Month;

                            if (ts == 0)
                            {
                                ts = b.PatientInfo.PVisit.InTime.Day - b.PatientInfo.Birthday.Day;
                                s[5] = "D" + ts.ToString();//年龄 
                            }
                            else
                            {
                                s[5] = "M" + ts.ToString();//年龄 
                            }
                        }
                        else
                        {
                            s[5] = "Y" + ts.ToString();//年龄 
                        }
                    }
                    #endregion

                    s[6] = b.PatientInfo.Name;//病人姓名
                    //性别编号
                    //性别
                    if (b.PatientInfo.Sex.ID.ToString() == "M" || b.PatientInfo.Sex.ID.ToString() == "1")
                    {
                        s[7] = "1";
                        s[8] = "男";
                    }
                    else
                    {
                        s[7] = "2";
                        s[8] = "女";
                    }
                    s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');//出生日期
                    s[10] = b.PatientInfo.AreaCode; //出生地
                    if (b.PatientInfo.IDCard == "" || b.PatientInfo.IDCard.Trim() == "-")
                    {
                        s[11] = "不详";
                    }
                    else
                    {
                        s[11] = b.PatientInfo.IDCard;//身份证号
                    }
                    #region s[12]国籍编号\ s[13]国籍  中国  需要转换其他
                    if (b.PatientInfo.Country.ID.ToString() == "1")
                    {
                        s[12] = "A156";
                        s[13] = "中国";
                    }
                    else
                    {
                        Neusoft.FrameWork.Models.NeuObject countryObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                        if (countryObj != null && countryObj.ID != "")
                        {
                            if (countryObj.Memo != "" && countryObj.Memo.ToUpper()!="TRUE")
                            {
                                s[12] = countryObj.Memo.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                            else
                            {
                                s[12] = countryObj.ID.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                        }
                        else
                        {
                            s[12] = b.PatientInfo.Country.ID.ToString();
                            s[13] = "";
                        }
                    }
                    #endregion
                    #region s[14]民族编号 s[15]民族
                    Neusoft.FrameWork.Models.NeuObject NationObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                    if (NationObj != null && NationObj.ID != "")
                    {
                        if (NationObj.Memo != "" && NationObj.Memo.ToUpper()!="TRUE")
                        {
                            s[14] = NationObj.Memo;
                            s[15] = NationObj.Name;
                        }
                        else
                        {
                            s[14] = NationObj.ID;
                            s[15] = NationObj.Name;
                        }
                    }
                    else
                    {
                        s[14] = b.PatientInfo.Nationality.ID;
                        s[15] = "";
                    }
                    #endregion
                    #region  s[16] 职业有中文传中文
                    //add by chengym 2011-6-15  字典表的名称字段varchar（100） 有些职业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                    Neusoft.FrameWork.Models.NeuObject JobObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CASEPROFESSION, b.PatientInfo.Profession.ID.ToString());
                    if (JobObj != null && JobObj.ID != "")
                    {
                        if (JobObj.Memo != "" && JobObj.Memo.ToUpper() != "TRUE")
                        {
                            if (JobObj.Memo.Length <= 50)
                            {
                                s[16] = JobObj.Memo;
                            }
                            else
                            {
                                s[16] = JobObj.Memo.Substring(0, 50);
                            }
                        }
                        else
                        {
                            if (JobObj.Name.Length <= 50)
                            {
                                s[16] = JobObj.Name;
                            }
                            else
                            {
                                s[16] = JobObj.Name.Substring(0, 50);
                            }
                        }
                    }
                    else
                    {
                        s[16] = b.PatientInfo.Profession.ID.ToString(); //职业 没有传中文不知道是否可以
                    }
                    #endregion
                    #region s[17] 婚姻状况编号 s[18]婚姻状况
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "10")
                    {
                        s[17] = "10"; //婚姻状况编号
                        s[18] = "未婚"; //婚姻状况
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "20")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "3"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "30")
                    {
                        s[17] = "30";
                        s[18] = "丧偶";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "40")
                    {
                        s[17] = "40";
                        s[18] = "离婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "R")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else
                    {
                        s[17] = "90";
                        s[18] = "未说明的婚姻状况";
                    }
                    #endregion
                    s[19] = b.PatientInfo.AddressBusiness;  //单位名称
                    s[20] = b.PatientInfo.AddressBusiness;//单位地址 
                    s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                    if (b.PatientInfo.BusinessZip != null && b.PatientInfo.BusinessZip.Length == 6)
                    {
                        s[22] = b.PatientInfo.BusinessZip;//单位邮编 
                    }
                    else
                    {
                        s[22] = "";//单位邮编 
                    }
                    s[23] = b.PatientInfo.AddressHome;//户口地址
                    if (b.PatientInfo.HomeZip != null && b.PatientInfo.HomeZip.Length == 6)
                    {
                        s[24] = b.PatientInfo.HomeZip;//户口邮编
                    }
                    else
                    {
                        s[24] = "";
                    }
                    s[25] = b.PatientInfo.Kin.Name;//联系人
                    #region s[26] 与病人关系
                    Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.RelationLink);
                    if (RelativeObj != null && RelativeObj.ID != "")
                    {
                        if (RelativeObj.Memo != "" && RelativeObj.Memo.ToUpper() != "TRUE")
                        {
                            s[26] = RelativeObj.Memo;//与患者关系
                        }
                        else
                        {
                           s[26] = RelativeObj.Name;//与患者关系
                        }
                        //s[26] = RelativeObj.Name;//与患者关系
                    }
                    else
                    {
                        s[26] = b.PatientInfo.Kin.RelationLink;//与患者关系
                    }
                    #endregion
                    s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                    s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话
                    if (b.PatientInfo.SSN.Trim() == "--" || b.PatientInfo.SSN.Trim() == "－" || b.PatientInfo.SSN.Trim() == "-"
                    || b.PatientInfo.SSN.Trim() == "—" || b.PatientInfo.SSN.Trim() == "——"
                    || b.PatientInfo.SSN.Trim().Length < 4)
                    {
                        s[29] = "";
                    }
                    else
                    {
                        s[29] = b.PatientInfo.SSN; // 原3.0的医保卡号
                    }
                    s[30] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                    s[31] = b.PatientInfo.PVisit.InTime.ToShortTimeString(); //Hour.ToString().PadLeft(2, '0'); //入院时间
                    s[32] = this.ConverDept(b.InDept.ID);//入院科室代码 入院统一科号，HIS接收时存储HIS科号
                    s[33] = this.ConverDeptName(b.InDept.ID, b.InDept.Name);//出院科室名称2011-6-8
                    s[34] = b.InRoom;//入院病室    
                    s[35] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                    s[36] = b.PatientInfo.PVisit.OutTime.ToShortTimeString(); //出院时间
                    s[37] = this.ConverDept(b.OutDept.ID);//出院科室代码
                    s[38] = this.ConverDeptName(b.OutDept.ID, b.OutDept.Name);//出院科室名称2011-6-8
                    s[39] = b.OutRoom; //出院病室
                    s[40] = b.InHospitalDays.ToString();//实际住院天数
                    s[41] = b.ClinicDiag.ID;//门（急）诊诊断(ICD10或ICD9)编码

                    if (b.ClinicDiag.Name.Length > 50)//门（急）诊诊断(ICD10或ICD9)对应疾病名
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name.Substring(0, 50).ToString());
                    }
                    else
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name);
                    }
                    s[43] = this.ConverDoc(b.ClinicDoc.ID);//门、急诊医生编号，对应tdoctor 中的ftygh
                    s[44] = b.ClinicDoc.Name;//门、急诊医生
                    //病理诊断
                    //if (b.PathologicalDiagCode == null)
                    //{
                    //    s[45] = b.PathologicalDiagName;
                    //}
                    //else
                    //{
                    //    s[45] = b.PathologicalDiagCode;
                    //}
                    if (b.PathologicalDiagName.Trim() == "-"
                    || b.PathologicalDiagName.Trim() == "－"
                    || b.PathologicalDiagName.Trim() == "--"
                    || b.PathologicalDiagName.Trim() == "——"
                    || b.PathologicalDiagName.Trim() == "—"
                    || b.PathologicalDiagName == "未发现"
                    || b.PathologicalDiagName == "/"
                    || b.PathologicalDiagName.Trim() == "无"
                    //|| b.PathologicalDiagName.Trim().Length < 3
                        )
                    {
                        s[45] = "";
                    }
                    else
                    {
                        s[45] = b.PathologicalDiagName;
                    }

                    //过敏药物
                    string anaphyPh = b.FirstAnaphyPharmacy.ID;
                    if (anaphyPh.Length > 100)
                    {
                        s[46] = this.ChangeCharacter(anaphyPh.Substring(0, 100));
                    }
                    else
                    {
                        s[46] = this.ChangeCharacter(anaphyPh);//药物过敏  
                    }
                    //门诊与出院诊断符合情况编号
                    if (b.CePi == null || b.CePi == "")
                    {
                        s[47] = "1";
                        s[48] = "符合";
                    }
                    else
                    {
                        s[47] = b.CePi;
                        s[48] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.CePi).Name;
                    }
                    //临床与病理诊断符合情况
                    if (b.ClPa == null || b.ClPa == "")
                    {
                        s[49] = "1";
                        s[50] = "符合";
                    }
                    else
                    {
                        s[49] = b.ClPa;
                        s[50] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.ClPa).Name;
                    }
                    s[51] = b.SalvTimes.ToString();//抢救次数
                    s[52] = b.SuccTimes.ToString();//成功次数

                    s[53] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID);//科主任编号，对应tdoctor 中的ftygh
                    s[54] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
                    s[55] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID);//主（副主）任医生编号，对应tdoctor 中的ftygh
                    s[56] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
                    s[57] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID);//主治医生编号，对应tdoctor 中的ftygh
                    s[58] = b.PatientInfo.PVisit.AttendingDoctor.Name;//主治医师姓名
                    s[59] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID);//住院医生编号，对应tdoctor 中的ftygh
                    s[60] = b.PatientInfo.PVisit.AdmittingDoctor.Name;//住院医师姓名
                    s[61] = this.ConverDoc(b.RefresherDoc.ID);//进修医师编号，对应tdoctor 中的ftygh
                    s[62] = b.RefresherDoc.Name;//进修医生
                    s[63] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID);//实习医生编号，对应tdoctor 中的ftygh
                    s[64] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                    s[65] = this.ConverDoc(b.CodingOper.ID);//编码员编号
                    s[66] = b.CodingOper.Name;//编码员名称
                    s[67] = this.ConverDoc(b.OperInfo.ID);//病案整理者编号
                    s[68] = b.OperInfo.Name;//操作员名称（病案整理者）
                    s[69] = b.MrQuality;//病案质量 
                    s[70] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                    s[71] = this.ConverDoc(b.QcDoc.ID);//质控医师名称
                    s[72] = b.QcDoc.Name;//质控医师
                    s[73] = this.ConverDoc(b.QcNurse.ID);
                    s[74] = b.QcNurse.Name;//质控护士名称
                    //质控日期
                    if (b.CheckDate < new DateTime(1900, 1, 1))
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else if (b.CheckDate <= b.PatientInfo.PVisit.OutTime)//质控日期不可能小于出院日期
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else
                    {
                        s[75] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
                    }
                    #region 费用 总费用s[76] 西药费s[77] 中药费s[78] 中成药费s[79] 中草药费s[80] 其他费s[81]
                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[76] = "0.00";//总费用
                        s[77] = "0.00";//西药费
                        s[78] = "0.00";//中药费
                        s[79] = "0.00";//中成药费
                        s[80] = "0.00";//中草药费
                        s[81] = "0.00";//其他费
                    }
                    else
                    {
                        s[76] = Feeds.Tables[0].Rows[0][0].ToString();//总费用
                        s[77] = Feeds.Tables[0].Rows[0][17].ToString();//西药费
                        s[78] = "0.00";//中药费
                        s[79] = Feeds.Tables[0].Rows[0][19].ToString();//中成药费
                        s[80] = Feeds.Tables[0].Rows[0][20].ToString();//中草药费
                        s[81] = Feeds.Tables[0].Rows[0][29].ToString();//其他费
                    }
                    #endregion
                    //是否尸检编号1：是 2：否
                    if (b.CadaverCheck == "1")
                    {
                        s[82] = "1";
                        s[83] = "是";
                    }
                    else if (b.CadaverCheck == "2")
                    {
                        s[82] = "2";
                        s[83] = "否";
                    }
                    else
                    {
                        s[82] = "";
                        s[83] = "-";
                    }
                    //s[83] = this.constMana.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
                    #region s[84]血型编号 s[85]血型
                    //if (b.PatientInfo.BloodType.ID.ToString() == "A")
                    //{
                    //    s[84] = "1";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "B")
                    //{
                    //    s[84] = "2";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "AB")
                    //{
                    //    s[84] = "4";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                    //{
                    //    s[84] = "3";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                    //{
                    //    s[84] = "6";
                    //    s[85] = "未查";
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "6")
                    //{
                    //    s[84] = "6";
                    //    s[85] = "未查";
                    //}
                    //else
                    //{
                    //    s[84] = "5";
                    //    s[85] = "不详";
                    //} 
                    
                    //CHANGE  BY ZHY    2013-08-01                 
                    if (b.PatientInfo.BloodType.ID.ToString() == "1")
                    {
                        s[84] = "1";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString()== "2")
                    {
                        s[84] = "2";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "3")
                    {
                        s[84] = "3";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "4")
                    {
                        s[84] = "4";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "5")
                    {
                        s[84] = "5";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "6")
                    {
                        s[84] = "6";
                       // s[85] = b.PatientInfo.BloodType.ToString();
                        s[85] = "未查";
                    }
                    else
                    {
                        s[84] = "5";
                       // s[85] = b.PatientInfo.BloodType.ToString();
                        s[85] = "不详";
                    }
                    #endregion
                    //s[86] = b.RhBlood;//RH编号
                    //RH
                    if (b.RhBlood == "1")
                    {
                        s[86] = "1";
                        s[87] = "阴";
                    }
                    else if (b.RhBlood == "2")
                    {
                        s[86] = "2";
                        s[87] = "阳";
                    }
                    else if (b.RhBlood == "3")
                    {
                        s[86] = "3";
                        s[87] = "不详";
                    }
                    else
                    {
                        s[86] = "4";
                        s[87] = "未查";
                    }
                    //婴儿数
                    int babyNum = 0;
                    try
                    {
                        babyNum = Neusoft.FrameWork.Function.NConvert.ToInt32(b.PatientInfo.User03);
                        s[88] = babyNum.ToString();
                    }
                    catch
                    {
                        s[88] = "0";
                    }
                    s[89] = "0";//是否部分病种，1是 0否
                    #region  s[90]首次转科统一科号，HIS接收时存储HIS科号 s[91]首次转科科别  s[92]首次转科日期  s[93]首次转科时间
                    if (alChangeDepe != null && alChangeDepe.Count > 0)
                    {
                        Neusoft.HISFC.Models.RADT.Location changeDept = alChangeDepe[0] as Neusoft.HISFC.Models.RADT.Location;
                        if (changeDept.Dept.ID != null && changeDept.Dept.ID != "" && changeDept.Dept.ID != b.InDept.ID)
                        {
                            try
                            {
                                s[90] = this.ConverDept(changeDept.Dept.ID);//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = this.ConverDeptName(changeDept.Dept.ID, changeDept.Dept.Name);//首次转科科别
                                s[92] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).ToShortDateString();//首次转科日期
                                s[93] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).ToShortTimeString();//Hour.ToString().PadLeft(2, '0');//首次转科时间
                            }
                            catch
                            {
                                s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = "";//首次转科科别
                                s[92] = "";//首次转科日期
                                s[93] = "";//首次转科时间
                            }
                        }
                        else
                        {
                            s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                            s[91] = "";//首次转科科别
                            s[92] = "";//首次转科日期
                            s[93] = "";//首次转科时间
                        }
                    }
                    else
                    {
                        s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                        s[91] = "";//首次转科科别
                        s[92] = "";//首次转科日期
                        s[93] = "";//首次转科时间
                    }
                    #endregion
                    s[94] = this.ConverDoc(b.OperInfo.ID);//输入员编号
                    s[95] = b.OperInfo.Name;//输入员
                    s[96] = System.DateTime.Now.ToShortDateString();//输入日期Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.FrameWork.Models.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');
                    Neusoft.FrameWork.Models.NeuObject CaseExaplTypeObj = this.constMana.GetConstant("CASEEXAMPLETYPE", b.ExampleType);
                    if (CaseExaplTypeObj != null && CaseExaplTypeObj.ID != "")
                    {
                        if (CaseExaplTypeObj.Memo != "" && CaseExaplTypeObj.Memo.ToUpper() != "TRUE")
                        {
                            s[97] = CaseExaplTypeObj.Memo;//疾病分型编号
                            s[98] = b.ExampleType;//疾病分型
                        }
                        else
                        {
                            switch (b.ExampleType)//疾病分型编号
                            {
                                case "A":
                                    s[97] = "1";
                                    break;
                                case "B":
                                    s[97] = "2";
                                    break;
                                case "C":
                                    s[97] = "3";
                                    break;
                                case "D":
                                    s[97] = "4";
                                    break;
                            }
                            s[98] = b.ExampleType;//疾病分型
                        }
                    }
                    else
                    {
                        switch (b.ExampleType)//疾病分型编号
                        {
                            case "A":
                                s[97] = "1";
                                break;
                            case "B":
                                s[97] = "2";
                                break;
                            case "C":
                                s[97] = "3";
                                break;
                            case "D":
                                s[97] = "4";
                                break;
                        }
                        s[98] = b.ExampleType;//疾病分型
                    }

                    s[99] = "";//复合归档编号
                    s[100] = "";//复合归档
                    s[101] = b.PatientInfo.PVisit.InSource.ID;//病人来源编号
                    s[102] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;//病人来源
                    s[103] = b.PatientInfo.User02;//是否手术 chengym
                    if (b.PatientInfo.User03 == "0")
                    {
                        s[104] = "0";//是否输入妇婴卡
                    }
                    else
                    {
                        s[104] = "1";//是否输入妇婴卡
                    }
                    //院感次数，来源院感报卡 12-8-28
                    try
                    {
                        int infNum = this.baseDml.QueryInfCount(b.PatientInfo.ID);
                        if (infNum == -1)
                        {
                            s[105] = "0";//医院感染次数，不允许为空，否则影响报表统计结果 chengym
                        }
                        else
                        {
                            s[105] = infNum.ToString();
                        }
                    }
                    catch
                    {
                        s[105] = "0";
                    }
                    s[106] = "";//扩展1 
                    s[107] = "";
                    s[108] = "";
                    s[109] = "";
                    s[110] = "";
                    s[111] = "";
                    s[112] = "";
                    s[113] = "";//扩展8档案号
                    s[114] = "";//扩展9档案次数
                    s[115] = "";
                    s[116] = "";
                    s[117] = "";
                    s[118] = "";
                    s[119] = "";
                    s[120] = "";//扩展15
                    s[121] = b.PatientInfo.DIST;//籍贯
                    string currenadd = string.Empty;
                    try
                    {
                        string[] tt= b.CurrentAddr.Split('@');
                        if (tt.Length > 1)
                        {
                            currenadd = tt[0].ToString();
                        }
                        string code = this.QueryCurrentAdrrCodeByName(currenadd+"@");
                        currenadd = code + b.CurrentAddr;
                    }
                    catch
                    {
                    }
                    if (currenadd != string.Empty)
                    {
                        s[122] = currenadd;
                    }
                    else
                    {
                        s[122] = b.CurrentAddr;//现住址
                    }
                    s[123] = b.CurrentPhone;//现电话
                    if (b.CurrentZip != null && b.CurrentZip.Length == 6)
                    {
                        s[124] = b.CurrentZip;//现邮编
                    }
                    else
                    {
                        s[124] = "";
                    }
                    s[125] = b.PatientInfo.Profession.ID;//职业编号
                    //可能存在医院填写非数字内容
                    try
                    {
                        int bweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyBirthWeight);
                        b.BabyBirthWeight = bweight.ToString();
                    }
                    catch
                    {
                        b.BabyBirthWeight = "0";
                    }
                    s[126] = b.BabyBirthWeight;//新生儿出生体重
                    //可能存在医院填写非数字内容
                    try
                    {
                        int biweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyInWeight);
                        b.BabyInWeight = biweight.ToString();
                    }
                    catch
                    {
                        b.BabyInWeight = "0";
                    }
                    s[127] = b.BabyInWeight;//新生儿入院体重
                    s[128] = b.InPath;//入院途径编号
                    s[129] = this.constMana.GetConstant("CASEINAVENUE", b.InPath).Name;//入院途径
                    s[130] = b.ClinicPath;//临床路径病例编号
                    if (b.ClinicPath == "1")
                    {
                        s[131] = "是";//临床路径病例
                    }
                    else
                    {
                        s[131] = "否";//临床路径病例
                    }
                    if (b.PathologicalDiagName.Trim() == "-"
                   || b.PathologicalDiagName.Trim() == "－"
                   || b.PathologicalDiagName.Trim() == "--"
                   || b.PathologicalDiagName.Trim() == "——"
                   || b.PathologicalDiagName.Trim() == "—"
                   || b.PathologicalDiagName == "未发现"
                   || b.PathologicalDiagName == "/"
                   || b.PathologicalDiagName.Trim() == "无"
                   //|| b.PathologicalDiagName.Trim().Length < 3
                        )
                    {
                        s[49] = "";
                        s[50] = "";
                        s[132] = "";//病理疾病编码
                        s[133] = "";//病理号
                    }
                    else
                    {
                        s[132] = b.PathologicalDiagCode;//病理疾病编码
                        s[133] = b.PathNum;//病理号
                    }
                    s[134] = b.AnaphyFlag;//是否药物过敏编号
                    if (b.AnaphyFlag == "1")
                    {
                        s[135] = "无";//是否药物过敏
                    }
                    else
                    {
                        s[135] = "有";//是否药物过敏
                    }
                    s[136] = this.ConverDoc(b.DutyNurse.ID);//责任护士编号
                    s[137] = b.DutyNurse.Name;//责任护士
                    //s[138] = b.Out_Type;//离院方式编号
                    if (b.Out_Type == "1")//离院方式
                    {
                        s[138] = "1";
                        s[139] = "医嘱离院";
                    }
                    else if (b.Out_Type == "2")//离院方式
                    {
                        s[138] = "2";
                        s[139] = "医嘱转院";
                    }
                    else if (b.Out_Type == "3")//离院方式
                    {
                        s[138] = "3";
                        s[139] = "医嘱转社区乡镇卫生院";
                    }
                    else if (b.Out_Type == "4")//离院方式
                    {
                        s[138] = "4";
                        s[139] = "非医嘱离院";
                    }
                    else if (b.Out_Type == "5")//离院方式
                    {
                        s[138] = "5";
                        s[139] = "死亡";
                    }
                    else
                    {
                        s[138] = "9";
                        s[139] = "其他";
                    }
                    s[140] = b.HighReceiveHopital;//离院方式为医嘱转院，拟接收医疗机构名称
                    s[141] = b.LowerReceiveHopital;//离院方式为转社区卫生服务器机构/乡镇卫生院，拟接收医疗机构名称
                    s[142] = b.ComeBackInMonth;//是否有出院31天内再住院计划编号
                    s[143] = "";//是否有出院31天内再住院计划
                    s[144] = b.ComeBackPurpose;//再住院目的
                    s[145] = b.OutComeDay.ToString();//颅脑损伤患者昏迷时间：入院前 天
                    s[146] = b.OutComeHour.ToString();//颅脑损伤患者昏迷时间：入院前 小时
                    s[147] = b.OutComeMin.ToString();//颅脑损伤患者昏迷时间：入院前 分钟
                    s[148] = (b.OutComeDay * 24 * 60 + b.OutComeHour * 60 + b.OutComeMin).ToString();//入院前昏迷总分钟(天、小时换算成分钟)
                    s[149] = b.InComeDay.ToString();//颅脑损伤患者昏迷时间：入院后 天
                    s[150] = b.InComeHour.ToString();//颅脑损伤患者昏迷时间：入院后 小时
                    s[151] = b.InComeMin.ToString();//颅脑损伤患者昏迷时间：入院后 分钟
                    s[152] = (b.InComeDay * 24 * 60 + b.InComeHour * 60 + b.InComeMin).ToString();//入院后昏迷总分钟
                    if (b.PatientInfo.Pact.ID == "9")
                    {
                        s[153] = "99";//付款方式编号
                    }
                    else
                    {
                        s[153] = b.PatientInfo.Pact.ID.PadLeft(2, '0');//付款方式编号
                    }
                    //s[153] = b.PatientInfo.Pact.ID;//付款方式编号
                    s[154] = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID).Name;//付款方式

                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[155] = "0.00";//住院总费用：自费金额
                        s[156] = "0.00";//综合医疗服务类：（1）一般医疗服务费
                        s[157] = "0.00";//综合医疗服务类：（2）一般治疗操作费
                        s[158] = "0.00";//综合医疗服务类：（3）护理费
                        s[159] = "0.00";//综合医疗服务类：（4）其他费用
                        s[160] = "0.00";//诊断类：(5) 病理诊断费
                        s[161] = "0.00";//诊断类：(6) 实验室诊断费
                        s[162] = "0.00";//诊断类：(7) 影像学诊断费
                        s[163] = "0.00";//诊断类：(8) 临床诊断项目费
                        s[164] = "0.00";//治疗类：(9) 非手术治疗项目费
                        s[165] = "0.00";//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = "0.00";//治疗类：(10) 手术治疗费
                        s[167] = "0.00";//治疗类：手术治疗费 其中麻醉费
                        s[168] = "0.00";//治疗类：手术治疗费 其中手术费
                        s[169] = "0.00";//康复类：(11) 康复费
                        s[170] = "0.00";//中医类：中医治疗类
                        s[171] = "0.00";//西药类： 西药费 其中抗菌药物费用
                        s[172] = "0.00";//血液和血液制品类： 血费
                        s[173] = "0.00";//血液和血液制品类： 白蛋白类制品费
                        s[174] = "0.00";//血液和血液制品类： 球蛋白制品费
                        s[175] = "0.00";//血液和血液制品类：凝血因子类制品费
                        s[176] = "0.00";//血液和血液制品类： 细胞因子类费
                        s[177] = "0.00";//耗材类：检查用一次性医用材料费
                        s[178] = "0.00";//耗材类：治疗用一次性医用材料费
                        s[179] = "0.00";//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    else
                    {
                        s[155] = Feeds.Tables[0].Rows[0][1].ToString();//自负金额
                        s[156] = Feeds.Tables[0].Rows[0][2].ToString();//综合医疗服务类：（1）一般医疗服务费
                        s[157] = Feeds.Tables[0].Rows[0][3].ToString();//综合医疗服务类：（2）一般治疗操作费
                        s[158] = Feeds.Tables[0].Rows[0][4].ToString();//综合医疗服务类：（3）护理费
                        s[159] = Feeds.Tables[0].Rows[0][5].ToString();//综合医疗服务类：（4）其他费用
                        s[160] = Feeds.Tables[0].Rows[0][6].ToString();//诊断类：(5) 病理诊断费
                        s[161] = Feeds.Tables[0].Rows[0][7].ToString();//诊断类：(6) 实验室诊断费
                        s[162] = Feeds.Tables[0].Rows[0][8].ToString();//诊断类：(7) 影像学诊断费
                        s[163] = Feeds.Tables[0].Rows[0][9].ToString();//诊断类：(8) 临床诊断项目费
                        s[164] = Feeds.Tables[0].Rows[0][10].ToString();//治疗类：(9) 非手术治疗项目费
                        s[165] = Feeds.Tables[0].Rows[0][11].ToString();//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = Feeds.Tables[0].Rows[0][12].ToString();//治疗类：(10) 手术治疗费
                        s[167] = Feeds.Tables[0].Rows[0][13].ToString();//治疗类：手术治疗费 其中麻醉费
                        s[168] = Feeds.Tables[0].Rows[0][14].ToString();//治疗类：手术治疗费 其中手术费
                        s[169] = Feeds.Tables[0].Rows[0][15].ToString();//康复类：(11) 康复费
                        s[170] = Feeds.Tables[0].Rows[0][16].ToString();//中医类：中医治疗类
                        s[171] = Feeds.Tables[0].Rows[0][18].ToString();//西药类： 西药费 其中抗菌药物费用
                        s[172] = Feeds.Tables[0].Rows[0][21].ToString();//血液和血液制品类： 血费
                        s[173] = Feeds.Tables[0].Rows[0][22].ToString();//血液和血液制品类： 白蛋白类制品费
                        s[174] = Feeds.Tables[0].Rows[0][23].ToString();//血液和血液制品类： 球蛋白制品费
                        s[175] = Feeds.Tables[0].Rows[0][24].ToString();//血液和血液制品类：凝血因子类制品费
                        s[176] = Feeds.Tables[0].Rows[0][25].ToString();//血液和血液制品类： 细胞因子类费
                        s[177] = Feeds.Tables[0].Rows[0][26].ToString();//耗材类：检查用一次性医用材料费
                        s[178] = Feeds.Tables[0].Rows[0][27].ToString();//耗材类：治疗用一次性医用材料费
                        s[179] = Feeds.Tables[0].Rows[0][28].ToString();//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    return s;
                }
                catch (Exception ex)
                {
                    this.Err = ex.ToString();
                    return null;
                }
                #endregion
            }
            else //顺德妇幼传基本信息 2012-12-27
            {
                #region
                string[] s = new string[194];
                try
                {
                    s[0] = "0";//是否输入，0：否，1：是，默认接收为0
                    //s[1] = this.PatientNoChang(b.PatientInfo.PID.PatientNO.TrimStart(new char[] { '0' }));//病案号
                    string patientNO = b.PatientInfo.PID.PatientNO.PadLeft(10, '0');
                    s[1] = this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr()));//病案号
                    s[2] = b.PatientInfo.InTimes.ToString();//住院次数
                    s[3] = "11";//ICD版本，9：ICD9字典库，10：ICD10字典库，待扩展11：国家ICD库，默认接收为11
                    s[4] = b.PatientInfo.ID;//住院流水号
                    #region //s[5] 年龄
                    if (b.PatientInfo.Age != "" && b.PatientInfo.Age != "0")
                    {
                        if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") < 0) //整岁
                        {
                            s[5] = "Y" + b.AgeUnit.Replace("岁", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//整月
                        {
                            s[5] = "M" + b.AgeUnit.Replace("月", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") < 0 && b.AgeUnit.IndexOf("天") > 0)//整天
                        {
                            s[5] = "D" + b.AgeUnit.Replace("天", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//N岁N月
                        {
                            string[] PAge = b.AgeUnit.Split('岁');
                            s[5] = "Y" + PAge[0] + "M" + PAge[1].Replace("岁", "").Replace("月", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N月N天
                        {
                            string[] PAge = b.AgeUnit.Split('月');
                            s[5] = "M" + PAge[0] + "D" + PAge[1].Replace("月", "").Replace("天", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N岁N月N天
                        {
                            string[] PAge = b.AgeUnit.Split('岁');

                            string[] PAge1 = PAge[1].Split('月');
                            s[5] = "Y" + PAge[0] + "M" + PAge1[0] + "D" + PAge1[1].Replace("月", "").Replace("天", "");
                        }
                    }
                    else
                    {
                        int ts = b.PatientInfo.PVisit.InTime.Year - b.PatientInfo.Birthday.Year;

                        if (ts == 0)
                        {
                            ts = b.PatientInfo.PVisit.InTime.Month - b.PatientInfo.Birthday.Month;

                            if (ts == 0)
                            {
                                ts = b.PatientInfo.PVisit.InTime.Day - b.PatientInfo.Birthday.Day;
                                s[5] = "D" + ts.ToString();//年龄 
                            }
                            else
                            {
                                s[5] = "M" + ts.ToString();//年龄 
                            }
                        }
                        else
                        {
                            s[5] = "Y" + ts.ToString();//年龄 
                        }
                    }
                    #endregion

                    s[6] = b.PatientInfo.Name;//病人姓名
                    //性别编号
                    //性别
                    if (b.PatientInfo.Sex.ID.ToString() == "M" || b.PatientInfo.Sex.ID.ToString() == "1")
                    {
                        s[7] = "1";
                        s[8] = "男";
                    }
                    else
                    {
                        s[7] = "2";
                        s[8] = "女";
                    }
                    s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');//出生日期
                    s[10] = b.PatientInfo.AddressHome; //出生地
                    if (b.PatientInfo.IDCard == "" || b.PatientInfo.IDCard.Trim() == "-")
                    {
                        s[11] = "不详";
                    }
                    else
                    {
                        s[11] = b.PatientInfo.IDCard;//身份证号
                    }
                    #region s[12]国籍编号\ s[13]国籍  中国  需要转换其他
                    if (b.PatientInfo.Country.ID.ToString() == "1")
                    {
                        s[12] = "A156";
                        s[13] = "中国";
                    }
                    else
                    {
                        Neusoft.FrameWork.Models.NeuObject countryObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                        if (countryObj != null && countryObj.ID != "")
                        {
                            if (countryObj.Memo != ""&&countryObj.Memo.ToUpper()!="TRUE")
                            {
                                s[12] = countryObj.Memo.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                            else
                            {
                                s[12] = countryObj.ID.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                        }
                        else
                        {
                            s[12] = b.PatientInfo.Country.ID.ToString();
                            s[13] = "";
                        }
                    }
                    #endregion
                    #region s[14]民族编号 s[15]民族
                    Neusoft.FrameWork.Models.NeuObject NationObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                    if (NationObj != null && NationObj.ID != "")
                    {
                        if (NationObj.Memo != ""&&NationObj.Memo.ToUpper()!="TRUE")
                        {
                            s[14] = NationObj.Memo;
                            s[15] = NationObj.Name;
                        }
                        else
                        {
                            s[14] = NationObj.ID;
                            s[15] = NationObj.Name;
                        }
                    }
                    else
                    {
                        s[14] = b.PatientInfo.Nationality.ID;
                        s[15] = "";
                    }
                    #endregion
                    #region  s[16] 职业有中文传中文
                    //add by chengym 2011-6-15  字典表的名称字段varchar（100） 有些职业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                    Neusoft.FrameWork.Models.NeuObject JobObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CASEPROFESSION, b.PatientInfo.Profession.ID.ToString());
                    if (JobObj != null && JobObj.ID != "")
                    {
                        if (JobObj.Memo != "" && JobObj.Memo.ToUpper() != "TRUE")
                        {
                            if (JobObj.Memo.Length <= 50)
                            {
                                s[16] = JobObj.Memo;
                            }
                            else
                            {
                                s[16] = JobObj.Memo.Substring(0, 50);
                            }
                        }
                        else
                        {
                            if (JobObj.Name.Length <= 50)
                            {
                                s[16] = JobObj.Name;
                            }
                            else
                            {
                                s[16] = JobObj.Name.Substring(0, 50);
                            }
                        }
                    }
                    else
                    {
                        s[16] = b.PatientInfo.Profession.ID.ToString(); //职业 没有传中文不知道是否可以
                    }
                    #endregion
                    #region s[17] 婚姻状况编号 s[18]婚姻状况
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "10")
                    {
                        s[17] = "10"; //婚姻状况编号
                        s[18] = "未婚"; //婚姻状况
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "20")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "3"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "30")
                    {
                        s[17] = "30";
                        s[18] = "丧偶";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "40")
                    {
                        s[17] = "40";
                        s[18] = "离婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "R")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else
                    {
                        s[17] = "90";
                        s[18] = "未说明的婚姻状况";
                    }
                    #endregion
                    if (b.PatientInfo.HomeZip != null && b.PatientInfo.HomeZip != "")
                    {
                        b.PatientInfo.BusinessZip=b.PatientInfo.HomeZip;
                        b.CurrentZip = b.PatientInfo.HomeZip;
                    }
                    else if (b.PatientInfo.BusinessZip != null && b.PatientInfo.BusinessZip != "")
                    {
                        b.PatientInfo.HomeZip = b.PatientInfo.BusinessZip;
                        b.CurrentZip = b.PatientInfo.BusinessZip;
                    }
                    else if (b.CurrentZip != null && b.CurrentZip != "")
                    {
                        b.PatientInfo.HomeZip = b.CurrentZip;
                        b.PatientInfo.BusinessZip = b.CurrentZip;
                    }
                    if (b.PatientInfo.AddressHome != null && b.PatientInfo.AddressHome != "")
                    {
                        b.CurrentAddr = b.PatientInfo.AddressHome;
                    }
                    s[19] = b.PatientInfo.AddressBusiness;  //单位名称
                    s[20] = b.PatientInfo.CompanyName;//单位地址 
                    s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                    s[22] = b.PatientInfo.BusinessZip;//单位邮编      
                    s[23] = b.PatientInfo.AddressHome;//户口地址
                    s[24] = b.PatientInfo.HomeZip;//户口邮编
                    s[25] = b.PatientInfo.Kin.Name;//联系人
                    #region s[26] 与病人关系
                    Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.Relation.ID);
                    if (RelativeObj != null && RelativeObj.ID != "")
                    {
                        if (RelativeObj.Memo != "" &&RelativeObj.Memo.ToUpper()!="TRUE")
                        {
                            if (RelativeObj.Memo.Length <= 10)
                            {
                                s[26] = RelativeObj.Memo;//与患者关系
                            }
                            else
                            {
                                s[26] = RelativeObj.Memo.Substring(0, 10);//与患者关系
                            }
                        }
                        else
                        {
                            if (RelativeObj.Name.Length <= 10)
                            {
                                s[26] = RelativeObj.Name;//与患者关系
                            }
                            else
                            {
                                s[26] = RelativeObj.Name.Substring(0, 10);//与患者关系
                            }
                        }
                    }
                    else
                    {
                        s[26] = b.PatientInfo.Kin.RelationLink;//与患者关系
                    }
                    #endregion
                    s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                    s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话
                    s[29] = b.PatientInfo.SSN; // 原3.0的医保卡号
                    s[30] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                    s[31] = b.PatientInfo.PVisit.InTime.Hour.ToString().PadLeft(2, '0'); //入院时间
                    Neusoft.HISFC.Models.RADT.Location indept = this.baseDml.GetDeptIn(b.PatientInfo.ID);
                    if (indept != null) //入院科室 
                    {
                        s[32] = this.ConverDept(indept.Dept.ID);//入院科室代码
                        s[33] = this.ConverDeptName(indept.Dept.ID, indept.Dept.Name);//入院科室名称
                    }
                    else
                    {
                        s[32] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//入院科室代码
                        s[33] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//入院科室名称
                    }
                    s[34] = b.InRoom;//入院病室    
                    s[35] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                    s[36] = b.PatientInfo.PVisit.OutTime.Hour.ToString().PadLeft(2, '0'); //出院时间
                    s[37] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//出院科室代码
                    s[38] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//出院科室名称2011-6-8
                    s[39] = b.OutRoom; //出院病室
                    System.TimeSpan tt = b.PatientInfo.PVisit.OutTime - b.PatientInfo.PVisit.InTime;
                    s[40] = tt.Days.ToString();//实际住院天数
                    s[41] = b.ClinicDiag.ID;//门（急）诊诊断(ICD10或ICD9)编码

                    if (b.ClinicDiag.Name.Length > 50)//门（急）诊诊断(ICD10或ICD9)对应疾病名
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name.Substring(0, 50).ToString());
                    }
                    else
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name);
                    }
                    s[43] = this.ConverDoc(b.ClinicDoc.ID);//门、急诊医生编号，对应tdoctor 中的ftygh
                    s[44] = b.ClinicDoc.Name;//门、急诊医生
                    //病理诊断
                    if (b.PathologicalDiagCode == null)
                    {
                        s[45] = b.PathologicalDiagName;
                    }
                    else
                    {
                        s[45] = b.PathologicalDiagCode;
                    }
                    //过敏药物
                    string anaphyPh = b.FirstAnaphyPharmacy.ID;
                    if (anaphyPh.Length > 100)
                    {
                        s[46] = this.ChangeCharacter(anaphyPh.Substring(0, 100));
                    }
                    else
                    {
                        s[46] = this.ChangeCharacter(anaphyPh);//药物过敏  
                    }
                    //s[46] = "1";
                    //门诊与出院诊断符合情况编号
                    if (b.CePi == null || b.CePi == "")
                    {
                        s[47] = "1";
                        s[48] = "符合";
                    }
                    else
                    {
                        s[47] = b.CePi;
                        s[48] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.CePi).Name;
                    }
                    //临床与病理诊断符合情况
                    if (b.ClPa == null || b.ClPa == "")
                    {
                        s[49] = "1";
                        s[50] = "符合";
                    }
                    else
                    {
                        s[49] = b.ClPa;
                        s[50] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.ClPa).Name;
                    }
                    s[51] = b.SalvTimes.ToString();//抢救次数
                    s[52] = b.SuccTimes.ToString();//成功次数

                    s[53] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID);//科主任编号，对应tdoctor 中的ftygh
                    s[54] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
                    s[55] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID);//主（副主）任医生编号，对应tdoctor 中的ftygh
                    s[56] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
                    s[57] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID);//主治医生编号，对应tdoctor 中的ftygh
                    s[58] = b.PatientInfo.PVisit.AttendingDoctor.Name;//主治医师姓名
                    s[59] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID);//住院医生编号，对应tdoctor 中的ftygh
                    s[60] = b.PatientInfo.PVisit.AdmittingDoctor.Name;//住院医师姓名
                    s[61] = this.ConverDoc(b.RefresherDoc.ID);//进修医师编号，对应tdoctor 中的ftygh
                    s[62] = b.RefresherDoc.Name;//进修医生
                    s[63] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID);//实习医生编号，对应tdoctor 中的ftygh
                    s[64] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                    s[65] = this.ConverDoc(b.CodingOper.ID);//编码员编号
                    s[66] = b.CodingOper.Name;//编码员名称
                    s[67] = this.ConverDoc(b.OperInfo.ID);//病案整理者编号
                    s[68] = b.OperInfo.Name;//操作员名称（病案整理者）
                    if (b.MrQuality == null || b.MrQuality == "")
                    {
                        s[69] = "1";
                    }
                    else
                    {
                        s[69] = b.MrQuality;//病案质量 
                    }
                    s[70] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                    s[71] = this.ConverDoc(b.QcDoc.ID);//质控医师名称
                    s[72] = b.QcDoc.Name;//质控医师
                    s[73] = this.ConverDoc(b.QcNurse.ID);
                    s[74] = b.QcNurse.Name;//质控护士名称
                    //质控日期
                    if (b.CheckDate < new DateTime(1900, 1, 1))
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else if (b.CheckDate <= b.PatientInfo.PVisit.OutTime)//质控日期不可能小于出院日期
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else
                    {
                        s[75] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
                    }
                    #region 费用 总费用s[76] 西药费s[77] 中药费s[78] 中成药费s[79] 中草药费s[80] 其他费s[81]
                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[76] = "0.00";//总费用
                        s[77] = "0.00";//西药费
                        s[78] = "0.00";//中药费
                        s[79] = "0.00";//中成药费
                        s[80] = "0.00";//中草药费
                        s[81] = "0.00";//其他费
                    }
                    else
                    {
                        s[76] = Feeds.Tables[0].Rows[0][0].ToString();//总费用
                        s[77] = Feeds.Tables[0].Rows[0][17].ToString();//西药费
                        s[78] = "0.00";//中药费
                        s[79] = Feeds.Tables[0].Rows[0][19].ToString();//中成药费
                        s[80] = Feeds.Tables[0].Rows[0][20].ToString();//中草药费
                        s[81] = Feeds.Tables[0].Rows[0][29].ToString();//其他费
                    }
                    #endregion
                    //是否尸检编号1：是 2：否
                    if (b.CadaverCheck == "1")
                    {
                        s[82] = "1";
                        s[83] = "是";
                    }
                    else if (b.CadaverCheck == "2")
                    {
                        s[82] = "2";
                        s[83] = "否";
                    }
                    else
                    {
                        s[82] = "2";
                        s[83] = "否";
                    }
                    //s[83] = this.constMana.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
                    #region s[84]血型编号 s[85]血型
                    if (b.PatientInfo.BloodType.ID.ToString() == "A")
                    {
                        s[84] = "1";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "B")
                    {
                        s[84] = "2";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "AB")
                    {
                        s[84] = "4";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                    {
                        s[84] = "3";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                    {
                        s[84] = "6";
                        s[85] = "未查";
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "6")
                    {
                        s[84] = "6";
                        s[85] = "未查";
                    }
                    else
                    {
                        s[84] = "6";
                        s[85] = "未查";
                    }
                    #endregion
                    //s[86] = b.RhBlood;//RH编号
                    //RH
                    //if (b.RhBlood == "1")
                    //{
                    //    s[86] = "1";
                    //    s[87] = "阴";
                    //}
                    //else if (b.RhBlood == "2")
                    //{
                    //    s[86] = "2";
                    //    s[87] = "阳";
                    //}
                    //else if (b.RhBlood == "3")
                    //{
                    //    s[86] = "3";
                    //    s[87] = "不详";
                    //}
                    //else
                    //{
                    //    //s[86] = "4";
                    //    //s[87] = "未查";
                        s[86] = "2";
                        s[87] = "阳";
                    //}
                    //婴儿数
                    int babyNum = 0;
                    try
                    {
                        babyNum = Neusoft.FrameWork.Function.NConvert.ToInt32(b.PatientInfo.User03);
                        s[88] = babyNum.ToString();
                    }
                    catch
                    {
                        s[88] = "0";
                    }
                    s[89] = "0";//是否部分病种，1是 0否
                    #region  s[90]首次转科统一科号，HIS接收时存储HIS科号 s[91]首次转科科别  s[92]首次转科日期  s[93]首次转科时间
                    if (alChangeDepe != null && alChangeDepe.Count > 0)
                    {
                        Neusoft.HISFC.Models.RADT.Location changeDept = alChangeDepe[0] as Neusoft.HISFC.Models.RADT.Location;
                        if (changeDept.Dept.ID != null && changeDept.Dept.ID != "")
                        {
                            try
                            {
                                s[90] = this.ConverDept(changeDept.Dept.ID);//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = this.ConverDeptName(changeDept.Dept.ID, changeDept.Dept.Name);//首次转科科别
                                s[92] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).ToShortDateString();//首次转科日期
                                s[93] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).Hour.ToString().PadLeft(2, '0');//首次转科时间
                            }
                            catch
                            {
                                s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = "";//首次转科科别
                                s[92] = "";//首次转科日期
                                s[93] = "";//首次转科时间
                            }
                        }
                        else
                        {
                            s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                            s[91] = "";//首次转科科别
                            s[92] = "";//首次转科日期
                            s[93] = "";//首次转科时间
                        }
                    }
                    else
                    {
                        s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                        s[91] = "";//首次转科科别
                        s[92] = "";//首次转科日期
                        s[93] = "";//首次转科时间
                    }
                    #endregion
                    s[94] = this.ConverDoc(b.OperInfo.ID);//输入员编号
                    s[95] = b.OperInfo.Name;//输入员
                    s[96] = System.DateTime.Now.ToShortDateString();//输入日期Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.FrameWork.Models.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');
                    //Neusoft.FrameWork.Models.NeuObject CaseExaplTypeObj = this.constMana.GetConstant("CASEEXAMPLETYPE", b.ExampleType);
                    //if (CaseExaplTypeObj != null && CaseExaplTypeObj.ID != "")
                    //{
                    //    if (CaseExaplTypeObj.Memo != "" && CaseExaplTypeObj.Memo.ToUpper() != "TRUE")
                    //    {
                    //        s[97] = CaseExaplTypeObj.Memo;//疾病分型编号
                    //        s[98] = b.ExampleType;//疾病分型
                    //    }
                    //}
                    //else
                    //{
                    //    switch (b.ExampleType)//疾病分型编号
                    //    {
                    //        case "A":
                    //            s[97] = "1";
                    //            break;
                    //        case "B":
                    //            s[97] = "2";
                    //            break;
                    //        case "C":
                    //            s[97] = "3";
                    //            break;
                    //        case "D":
                    //            s[97] = "4";
                    //            break;
                    //    }
                    //    s[98] = b.ExampleType;//疾病分型
                    //}
                    s[97] = "1";
                    s[98] = "一般";//疾病分型

                    s[99] = "";//复合归档编号
                    s[100] = "";//复合归档
                    s[101] = b.PatientInfo.PVisit.InSource.ID;//病人来源编号
                    s[102] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;//病人来源
                    s[103] = b.PatientInfo.User02;//是否手术 chengym
                    if (b.PatientInfo.User03 == "0")
                    {
                        s[104] = "0";//是否输入妇婴卡
                    }
                    else
                    {
                        s[104] = "1";//是否输入妇婴卡
                    }
                    //院感次数，来源院感报卡 12-8-28
                    try
                    {
                        int infNum = this.baseDml.QueryInfCount(b.PatientInfo.ID);
                        if (infNum == -1)
                        {
                            s[105] = "0";//医院感染次数，不允许为空，否则影响报表统计结果 chengym
                        }
                        else
                        {
                            s[105] = infNum.ToString();
                        }
                    }
                    catch
                    {
                        s[105] = "0";
                    }
                    s[106] = "";//扩展1 
                    s[107] = "";
                    s[108] = "";
                    s[109] = "";
                    s[110] = "";
                    s[111] = "";
                    s[112] = "";
                    s[113] = "";//扩展8档案号
                    s[114] = "";//扩展9档案次数
                    s[115] = "";
                    s[116] = "";
                    s[117] = "";
                    s[118] = "";
                    s[119] = "";
                    s[120] = "";//扩展15
                    s[121] = b.PatientInfo.DIST;//籍贯
                    s[122] = b.CurrentAddr;//现住址
                    s[123] = b.CurrentPhone;//现电话
                    s[124] = b.CurrentZip;//现邮编
                    s[125] = b.PatientInfo.Profession.ID;//职业编号
                    //可能存在医院填写非数字内容
                    try
                    {
                        int bweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyBirthWeight);
                        b.BabyBirthWeight = bweight.ToString();
                    }
                    catch
                    {
                        b.BabyBirthWeight = "0";
                    }
                    s[126] = b.BabyBirthWeight;//新生儿出生体重
                    //可能存在医院填写非数字内容
                    try
                    {
                        int biweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyInWeight);
                        b.BabyInWeight = biweight.ToString();
                    }
                    catch
                    {
                        b.BabyInWeight = "0";
                    }
                    s[127] = b.BabyInWeight;//新生儿入院体重
                    s[128] = b.InPath;//入院途径编号
                    s[129] = this.constMana.GetConstant("CASEINAVENUE", b.InPath).Name;//入院途径
                    s[130] = b.ClinicPath;//临床路径病例编号
                    if (b.ClinicPath == "1")
                    {
                        s[131] = "是";//临床路径病例
                    }
                    else
                    {
                        s[131] = "否";//临床路径病例
                    }
                    s[132] = "";//病理疾病编码
                    s[133] = b.PathNum;//病理号
                    //s[134] = b.AnaphyFlag;//是否药物过敏编号
                    //if (b.AnaphyFlag == "1")
                    //{
                    //    s[135] = "无";//是否药物过敏
                    //}
                    //else
                    //{
                    //    s[135] = "有";//是否药物过敏
                    //}
                    s[134] = "1";
                    s[135] = "无";
                    s[136] = this.ConverDoc(b.DutyNurse.ID);//责任护士编号
                    s[137] = b.DutyNurse.Name;//责任护士
                    //s[138] = b.Out_Type;//离院方式编号
                    //if (b.Out_Type == "1")//离院方式
                    //{
                    //    s[138] = "1";
                    //    s[139] = "医嘱离院";
                    //}
                    //else if (b.Out_Type == "2")//离院方式
                    //{
                    //    s[138] = "2";
                    //    s[139] = "医嘱转院";
                    //}
                    //else if (b.Out_Type == "3")//离院方式
                    //{
                    //    s[138] = "3";
                    //    s[139] = "医嘱转社区乡镇卫生院";
                    //}
                    //else if (b.Out_Type == "4")//离院方式
                    //{
                    //    s[138] = "4";
                    //    s[139] = "非医嘱离院";
                    //}
                    //else if (b.Out_Type == "5")//离院方式
                    //{
                    //    s[138] = "5";
                    //    s[139] = "死亡";
                    //}
                    //else
                    //{
                    //    s[138] = "1";
                    //    s[139] = "医嘱离院";
                    //}
                    s[138] = "1";
                    s[139] = "医嘱离院";
                    s[140] = b.HighReceiveHopital;//离院方式为医嘱转院，拟接收医疗机构名称
                    s[141] = b.LowerReceiveHopital;//离院方式为转社区卫生服务器机构/乡镇卫生院，拟接收医疗机构名称
                    //s[142] = b.ComeBackInMonth;//是否有出院31天内再住院计划编号
                    //s[143] = "";//是否有出院31天内再住院计划
                    s[142] = "1";//是否有出院31天内再住院计划编号
                    s[143] = "无";//是否有出院31天内再住院计划
                    s[144] = b.ComeBackPurpose;//再住院目的
                    s[145] = b.OutComeDay.ToString();//颅脑损伤患者昏迷时间：入院前 天
                    s[146] = b.OutComeHour.ToString();//颅脑损伤患者昏迷时间：入院前 小时
                    s[147] = b.OutComeMin.ToString();//颅脑损伤患者昏迷时间：入院前 分钟
                    s[148] = (b.OutComeDay * 24 * 60 + b.OutComeHour * 60 + b.OutComeMin).ToString();//入院前昏迷总分钟(天、小时换算成分钟)
                    s[149] = b.InComeDay.ToString();//颅脑损伤患者昏迷时间：入院后 天
                    s[150] = b.InComeHour.ToString();//颅脑损伤患者昏迷时间：入院后 小时
                    s[151] = b.InComeMin.ToString();//颅脑损伤患者昏迷时间：入院后 分钟
                    s[152] = (b.InComeDay * 24 * 60 + b.InComeHour * 60 + b.InComeMin).ToString();//入院后昏迷总分钟
                    Neusoft.FrameWork.Models.NeuObject pactObj = this.constMana.GetConstant("CASEPACTCHANGE", b.PatientInfo.Pact.ID);
                    if (pactObj != null)
                    {
                        if (pactObj.Memo != "" && pactObj.Memo.ToUpper() != "TRUE")
                        {
                            if (pactObj.Memo == "9")
                            {
                                s[153] = "99";//付款方式编号
                            }
                            else
                            {
                                s[153] = pactObj.Memo.PadLeft(2, '0');//付款方式编号
                            }
                            s[154] = this.constMana.GetConstant("CASEPACT", pactObj.Memo).Name;//付款方式
                        }
                        else
                        {
                            s[153] = b.PatientInfo.Pact.ID;
                            s[154] = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID).Name;//付款方式
                        }
                    }
                    else
                    {
                        s[153] = b.PatientInfo.Pact.ID;
                        s[154] = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID).Name;//付款方式
                    }

                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[155] = "0.00";//住院总费用：自费金额
                        s[156] = "0.00";//综合医疗服务类：（1）一般医疗服务费
                        s[157] = "0.00";//综合医疗服务类：（2）一般治疗操作费
                        s[158] = "0.00";//综合医疗服务类：（3）护理费
                        s[159] = "0.00";//综合医疗服务类：（4）其他费用
                        s[160] = "0.00";//诊断类：(5) 病理诊断费
                        s[161] = "0.00";//诊断类：(6) 实验室诊断费
                        s[162] = "0.00";//诊断类：(7) 影像学诊断费
                        s[163] = "0.00";//诊断类：(8) 临床诊断项目费
                        s[164] = "0.00";//治疗类：(9) 非手术治疗项目费
                        s[165] = "0.00";//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = "0.00";//治疗类：(10) 手术治疗费
                        s[167] = "0.00";//治疗类：手术治疗费 其中麻醉费
                        s[168] = "0.00";//治疗类：手术治疗费 其中手术费
                        s[169] = "0.00";//康复类：(11) 康复费
                        s[170] = "0.00";//中医类：中医治疗类
                        s[171] = "0.00";//西药类： 西药费 其中抗菌药物费用
                        s[172] = "0.00";//血液和血液制品类： 血费
                        s[173] = "0.00";//血液和血液制品类： 白蛋白类制品费
                        s[174] = "0.00";//血液和血液制品类： 球蛋白制品费
                        s[175] = "0.00";//血液和血液制品类：凝血因子类制品费
                        s[176] = "0.00";//血液和血液制品类： 细胞因子类费
                        s[177] = "0.00";//耗材类：检查用一次性医用材料费
                        s[178] = "0.00";//耗材类：治疗用一次性医用材料费
                        s[179] = "0.00";//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    else
                    {
                        s[155] = Feeds.Tables[0].Rows[0][1].ToString();//自负金额
                        s[156] = Feeds.Tables[0].Rows[0][2].ToString();//综合医疗服务类：（1）一般医疗服务费
                        s[157] = Feeds.Tables[0].Rows[0][3].ToString();//综合医疗服务类：（2）一般治疗操作费
                        s[158] = Feeds.Tables[0].Rows[0][4].ToString();//综合医疗服务类：（3）护理费
                        s[159] = Feeds.Tables[0].Rows[0][5].ToString();//综合医疗服务类：（4）其他费用
                        s[160] = Feeds.Tables[0].Rows[0][6].ToString();//诊断类：(5) 病理诊断费
                        s[161] = Feeds.Tables[0].Rows[0][7].ToString();//诊断类：(6) 实验室诊断费
                        s[162] = Feeds.Tables[0].Rows[0][8].ToString();//诊断类：(7) 影像学诊断费
                        s[163] = Feeds.Tables[0].Rows[0][9].ToString();//诊断类：(8) 临床诊断项目费
                        s[164] = Feeds.Tables[0].Rows[0][10].ToString();//治疗类：(9) 非手术治疗项目费
                        s[165] = Feeds.Tables[0].Rows[0][11].ToString();//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = Feeds.Tables[0].Rows[0][12].ToString();//治疗类：(10) 手术治疗费
                        s[167] = Feeds.Tables[0].Rows[0][13].ToString();//治疗类：手术治疗费 其中麻醉费
                        s[168] = Feeds.Tables[0].Rows[0][14].ToString();//治疗类：手术治疗费 其中手术费
                        s[169] = Feeds.Tables[0].Rows[0][15].ToString();//康复类：(11) 康复费
                        s[170] = Feeds.Tables[0].Rows[0][16].ToString();//中医类：中医治疗类
                        s[171] = Feeds.Tables[0].Rows[0][18].ToString();//西药类： 西药费 其中抗菌药物费用
                        s[172] = Feeds.Tables[0].Rows[0][21].ToString();//血液和血液制品类： 血费
                        s[173] = Feeds.Tables[0].Rows[0][22].ToString();//血液和血液制品类： 白蛋白类制品费
                        s[174] = Feeds.Tables[0].Rows[0][23].ToString();//血液和血液制品类： 球蛋白制品费
                        s[175] = Feeds.Tables[0].Rows[0][24].ToString();//血液和血液制品类：凝血因子类制品费
                        s[176] = Feeds.Tables[0].Rows[0][25].ToString();//血液和血液制品类： 细胞因子类费
                        s[177] = Feeds.Tables[0].Rows[0][26].ToString();//耗材类：检查用一次性医用材料费
                        s[178] = Feeds.Tables[0].Rows[0][27].ToString();//耗材类：治疗用一次性医用材料费
                        s[179] = Feeds.Tables[0].Rows[0][28].ToString();//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    return s;
                }
                catch (Exception ex)
                {
                    this.Err = ex.ToString();
                    return null;
                }
                #endregion
            }
        }
        /// <summary>
        /// 获取现地址编码
        /// </summary>
        /// <param name="AdrrName">现地址名称</param>
        /// <returns></returns>
        private string QueryCurrentAdrrCodeByName(string AdrrName)
        {
            string strSql = @" select d.code from com_dictionary d where d.type='BZDZK' and d.name='{0}'";

            string iReturn = string.Empty;
            try
            {
                strSql = string.Format(strSql, AdrrName);
                //查询
                if (this.ExecQuery(strSql) < 0)
                {
                    this.Err = "执行sql失败!";
                    return "";
                }
                this.Reader.Read();
                iReturn = Reader[0].ToString();
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            catch (Exception ex)
            {
                this.Err = "" + ex.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                return "";
            }
            return iReturn;
        }
        /// <summary>
        /// 获得接口HIS_BA1 INSERT SQL
        /// </summary>
        /// <param name="b">病案首页的实体类</param>
        /// <param name="Feeds">费用信息数组</param>
        /// <param name="alChangeDepe">转科信息数组</param>
        /// <param name="alDose">诊断信息数组</param>
        /// <param name="isMetCasBase">true病案首页信息 false 住院主表信息</param>
        /// <returns></returns>
        private string GetInsertHISBA1SQLDrgs(Neusoft.HISFC.Models.HealthRecord.Base b, DataSet Feeds,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            if (b == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            strReturn = @"INSERT INTO HIS_BA1
  (
   Fifinput,
   FPRN,
   FTIMES,
   FICDVersion,
   FZYID,
   FAGE,
   FNAME,
   FSEXBH,
   FSEX,
   FBIRTHDAY,
   FBIRTHPLACE, --10
   FIDCard,
   fcountrybh,
   fcountry,
   fnationalitybh,
   fnationality,
   FJOB,
   FSTATUSBH,
   FSTATUS,
   FDWNAME,
   FDWADDR,--20
   FDWTELE,
   FDWPOST,
   FHKADDR,
   FHKPOST,
   FLXNAME,
   FRELATE,
   FLXADDR,
   FLXTELE,
   FASCARD1,
   FRYDATE,--30
   FRYTIME,
   FRYTYKH,
   FRYDEPT,
   FRYBS,
   FCYDATE,
   FCYTIME,
   FCYTYKH,
   FCYDEPT,
   FCYBS,
   FDAYS,--40
   FMZZDBH,
   FMZZD,
   FMZDOCTBH,
   FMZDOCT,
   FPHZD,
   FGMYW,
   FmzCYACCOBH,
   FmzCYACCO,
   FLCBLACCOBH,
   FLCBLACCO,--50
   FQJTIMES,
   FQJSUCTIMES,
   FKZRBH,
   FKZR,
   FZRDOCTBH,
   FZRDOCTOR,
   FZZDOCTBH,
   FZZDOCT,
   FZYDOCTBH,
   FZYDOCT,--60
   FJXDOCTBH,
   FJXDOCT,
   FSXDOCTBH,
   FSXDOCT,
   FBMYBH,
   FBMY,
   FZLRBH,
   FZLR,
   FQUALITYBH,
   FQUALITY,--70
   FZKDOCTBH,
   FZKDOCT,
   FZKNURSEBH,
   FZKNURSE,
   FZKRQ,
FSUM1,
FXYF,
FZYF,
FZCHYF,
FZCYF,--80
FQTF,
   FBODYBH,
   FBODY,
   FBLOODBH,
   FBLOOD,
   FRHBH,
   FRH,
   FBABYNUM,
   FTWILL,
   FZKTYKH,--90
   FZKDEPT,
   FZKDATE,
   FZKTIME,
   FSRYBH,
   FSRY,
   FWORKRQ,
   FJBFXBH,
   FJBFX,
   FFHGDBH,
   FFHGD,--100
   FSOURCEBH,
   FSOURCE,
   FIFSS,
   FIFFYK,
   FYNGR,
   FEXTEND1,
   FEXTEND2,
   FEXTEND3,
   FEXTEND4,
   FEXTEND5,--110
   FEXTEND6,
   FEXTEND7,
   FEXTEND8,
   FEXTEND9,
   FEXTEND10,
   FEXTEND11,
   FEXTEND12,
   FEXTEND13,
   FEXTEND14,
   FEXTEND15,--120
    FNATIVE,
    FCURRADDR,
    FCURRTELE,
    FCURRPOST,
    FJOBBH,
    FCSTZ,
    FRYTZ,
    FRYTJBH,
    FRYTJ,
    FYCLJBH,--130
    FYCLJ,
    FPHZDBH,
    FPHZDNUM,
    FIFGMYWBH,
    FIFGMYW,
    FNURSEBH,
    FNURSE,
    FLYFSBH,
    FLYFS,
    FYZOUTHOSTITAL,--140
    FSQOUTHOSTITAL,
    FISAGAINRYBH,
    FISAGAINRY,
    FISAGAINRYMD,
    FRYQHMDAYS,
    FRYQHMHOURS,
    FRYQHMMINS,
    FRYQHMCOUNTS,
    FRYHMDAYS,
    FRYHMHOURS,--150
    FRYHMMINS,
    FRYHMCOUNTS,
    FFBBHNEW,
    FFBNEW,
FZFJE,
FZHFWLYLF,
FZHFWLCZF,
FZHFWLHLF,
FZHFWLQTF,
FZDLBLF,--160
FZDLSSSF,
FZDLYXF,
FZDLLCF,
FZLLFFSSF,
FZLLFWLZWLF,
FZLLFSSF,
FZLLFMZF,
FZLLFSSZLF,
FKFLKFF,
FZYLZF,--170
FXYLGJF,
FXYLXF,
FXYLBQBF,
FXYLQDBF,
FXYLYXYZF,
FXYLXBYZF,
FHCLCJF,
FHCLZLF,
FHCLSSF,
FZHFWLYLF01,--180
FZHFWLYLF02,
FZYLZDF,
FZYLZLF,
FZYLZLF01,
FZYLZLF02,
FZYLZLF03,
FZYLZLF04,
FZYLZLF05,
FZYLZLF06,
FZYLQTF,--190
FZYLQTF01,
FZYLQTF02,
FZCLJGZJF --193
)
  VALUES
  (
'{0}',
'{1}',
 {2},
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
'{33}',
'{34}',
'{35}',
'{36}',
'{37}',
'{38}',
'{39}',
 {40},
'{41}',
'{42}',
'{43}',
'{44}',
'{45}',
'{46}',
'{47}',
'{48}',
'{49}',
'{50}',
 {51},
 {52},
'{53}',
'{54}',
'{55}',
'{56}',
'{57}',
'{58}',
'{59}',
'{60}',
'{61}',
'{62}',
'{63}',
'{64}',
'{65}',
'{66}',
'{67}',
'{68}',
'{69}',
'{70}',
'{71}',
'{72}',
'{73}',
'{74}',
'{75}',
 {76},
 {77},
 {78},
 {79},
 {80},
 {81},
'{82}',
'{83}',
'{84}',
'{85}',
'{86}',
'{87}',
 {88},
'{89}',
'{90}',
'{91}',
'{92}',
'{93}',
'{94}',
'{95}',
'{96}',
'{97}',
'{98}',
'{99}',
'{100}',
'{101}',
'{102}',
'{103}',
'{104}',
 {105},
'{106}',
'{107}',
'{108}',
'{109}',
'{110}',
'{111}',
'{112}',
'{113}',
'{114}',
'{115}',
'{116}',
'{117}',
'{118}',
'{119}',
'{120}',
'{121}',
'{122}',
'{123}',
'{124}',
'{125}',
 {126},
 {127},
'{128}',
'{129}',
'{130}',
'{131}',
'{132}',
'{133}',
'{134}',
'{135}',
'{136}',
'{137}',
'{138}',
'{139}',
'{140}',
'{141}',
'{142}',
'{143}',
'{144}',
{145},
{146},
{147},
{148},
{149},
{150},
{151},
{152},
'{153}',
'{154}',
{155},
{156},
{157},
{158},
{159},
{160},
{161},
{162},
{163},
{164},
{165},
{166},
{167},
{168},
{169},
{170},
{171},
{172},
{173},
{174},
{175},
{176},
{177},
{178},
{179},
{180},
{181},
{182},
{183},
{184},
{185},
{186},
{187},
{188},
{189},
{190},
{191},
{192},
{193}
)";

            try
            {

                strReturn = string.Format(strReturn, this.GetBaseInfoBA1Drgs(b, Feeds, alChangeDepe, alDose,isMetCasBase));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }
        #endregion
        #region HIS_BA3
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
            string sql = @"insert into HIS_BA3 (fprn,ftimes,FZDLX, FICDVersion, FICDM, FJBNAME,FRYBQBH,FRYBQ) values ('{0}',{1},'{2}',{3},'{4}','{5}','{6}','{7}')";
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql,
                                                 this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),
                                                 //patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 obj.DiagInfo.DiagType.ID,//对照
                                                 "11",//ICD版本号
                                                 obj.DiagInfo.ICD10.ID,
                                                 //obj.DiagInfo.Name,//医生描述的诊断名称
                                                 obj.DiagInfo.ICD10.Name,//标准ICD名称
                                                 obj.DiagOutState,
                                                 "");
            }
            catch
            {
            }

            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        #endregion
        #region HIS_BA4
        /// <summary>
        /// insert HIS_BA4  --手术信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">手术信息实体</param>
        /// <returns></returns>
        public int insertHisBa4Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.OperationDetail obj)
        {

            #region  sql
            string sql = @"INSERT INTO HIS_BA4
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
            Neusoft.FrameWork.Models.NeuObject info = this.constMana.GetConstant("CASEANESTYPE", obj.MarcKind);
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
            if (obj.NickKind.ToString() == "01" || obj.NickKind.ToString() == "02" || obj.NickKind.ToString() == "03")
            {
                NickKind_Code = "0";
                NickKind_Name = "0类";
            }
            else
            {
                NickKind_Code = obj.NickKind;
                NickKind_Name = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCITYPE, obj.NickKind).Name;
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
                sql = string.Format(sql, this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.HappenNO.ToString(),//手术次数
                                                 obj.OperationInfo.ID.ToString(),//手术码
                                                 this.ChangeCharacter(obj.OperationInfo.Memo.ToString()),//手术码对应名称
                                                 //this.ChangeCharacter(obj.OperationInfo.Name.ToString()),//手术码对应名称
                                                 this.ChangeDateTime(obj.OperationDate.ToShortDateString()),//手术日期
                                                 NickKind_Code,//obj.NickKind.ToString(), //切口编号
                                                 NickKind_Name,// this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCITYPE, obj.NickKind).Name.ToString(),  //切口
                                                 obj.CicaKind.ToString(),//愈合编号
                                                 this.constMana.GetConstant("CICATYPE", obj.CicaKind).Name.ToString(),  //愈合
                                                 this.ConverDoc(obj.FirDoctInfo.ID),//手术医生编号
                                                 obj.FirDoctInfo.Name,//手术医生
                                                 MarcKind_Code, // obj.MarcKind.ToString(),//麻醉方式编号
                                                 MarcKind_Name, //this.constMana.GetConstant("CASEANESTYPE",obj.MarcKind).Name.ToString(),//麻醉方式
                                                 addFlag, //是否附加手术"0",
                                                 this.ConverDoc(obj.SecDoctInfo.ID),//I助编号
                                                 obj.SecDoctInfo.Name.ToString(), // I助姓名
                                                 this.ConverDoc(obj.ThrDoctInfo.ID),//II助编号
                                                 obj.ThrDoctInfo.Name.ToString(),//II助姓名
                                                 this.ConverDoc(obj.NarcDoctInfo.ID),//麻醉医生编号
                                                 obj.NarcDoctInfo.Name.ToString(), //麻醉医生
                                                 checkDateTypeCode,//择期手术编号1是，0否
                                                 checkDateTypeName,//择期手术名称1是，0否
                                                 //obj.FourDoctInfo.Name,//手术级别编号
                                                 //this.constMana.GetConstant("CASELEVEL", obj.FourDoctInfo.Name).Name.ToString(),//手术级别
                                                 opLevelId,
                                                 opLevelName,
                                                 empl.Dept.Name,//手术医生所在科室名称
                                                 empl.Dept.ID //手术医生所在科室编码
                                                 );
            }
            catch
            {
            }
            ReadSQL(sql);
            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        #endregion
        #region HIS_BA5
        /// <summary>
        /// insert HIS_BA5 --妇婴信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">妇婴卡信息</param>
        /// <returns></returns>
        public int insertHisBa5Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Baby obj)
        {


            string sql = @"insert into HIS_BA5 (FPRN,FTIMES ,FBABYNUM ,FNAME ,FBABYSEXBH ,FBABYSEX ,FTZ ,FRESULTBH ,FRESULT ,
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
                sql = string.Format(sql, this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 obj.HappenNum.ToString(),//婴儿序号
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.SexCode.ToString(),//婴儿性别编号
                                                 obj.Infect.Memo,//婴儿性别
                                                 obj.Weight.ToString(),//婴儿体重
                                                 obj.BirthEnd.ToString(), //分娩结果编号
                                                 this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CHILDBEARINGRESULT, obj.BirthEnd).Name.ToString(),  //分娩结果
                                                 obj.BabyState,//转归编号
                                                 this.constMana.GetConstant("babyZG", obj.BabyState).Name.ToString(),  //转归
                                                 obj.SalvNum.ToString(),//抢救次数
                                                 obj.SuccNum.ToString(),//婴儿抢救成功次数
                                                 obj.Breath,//呼吸编号
                                                 this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.BREATHSTATE, obj.BirthEnd).Name.ToString() //呼吸
                                                 );
            }
            catch
            {
            }
            ReadSQL(sql);
            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }


            return 1;
        }
        #endregion
        /// <summary>
        /// HIS_BA6  --病人肿瘤信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">肿瘤卡信息</param>
        /// <returns></returns>
        public int InsertHISBA6Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Tumour obj)
        {
            #region sql
            string sql = @"INSERT INTO HIS_BA6
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
                                                 this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr())),//病案号
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
                                                 this.ChangeDateTime(obj.BeginDate1.ToString()),//原发灶开始日期
                                                 this.ChangeDateTime(obj.EndDate1.ToString()),//原发灶结束时间
                                                 obj.Gy2,//2.区域淋巴结剂量
                                                 obj.Time2,//区域淋巴结次数
                                                 obj.Day2,//区域淋巴结天数
                                                 this.ChangeDateTime(obj.BeginDate2.ToString()),//区域淋巴结开始时间
                                                 this.ChangeDateTime(obj.EndDate2.ToString()),//区域淋巴结结束时间
                                                 obj.Position,//3.转移灶名称
                                                 obj.Gy3,//3.转移灶剂量
                                                 obj.Time3,//转移灶次数
                                                 obj.Day3,//转移灶天数
                                                 this.ChangeDateTime(obj.BeginDate3.ToString()),//转移灶开始时间
                                                 this.ChangeDateTime(obj.EndDate3.ToString()),//转移灶结束时间
                                                 obj.Cmodeid,//化疗方式编号
                                                 this.constMana.GetConstant("CHEMOTHERAPY", obj.Cmodeid).Name,//化疗方式
                                                 obj.Cmethod,//化疗方法编号
                                                 this.constMana.GetConstant("CHEMOTHERAPYWAY", obj.Cmethod).Name,//化疗方法
                                                 obj.Tumour_Type,//肿瘤病例分类
                                                 obj.Tumour_T,//原发肿瘤T
                                                 obj.Tumour_N,//淋巴转移N
                                                 obj.Tumour_M,//远程转移M
                                                 this.constMana.GetConstant("CASETUMOURSTAGE",obj.Tumour_Stage).Name,//分期
                                                 obj.Tumour_Stage//分期编码
                                                 );
            }
            catch
            {
            }

            sql = sql.Replace("'NULL'", "NULL");
            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 更改HIS_BA6  FYRQ1 FYRQ2为空
        /// </summary>
        /// <returns></returns>
        public int UpdateHISBA6FYRQ(string FPRN)
        {
            // t.FQRQ1 t.FQRQ2  t.FZRQ1 t.FZRQ2  
            string strSQL = @"update HIS_BA6 set FYJY=null, FYCS=null, FYTS=null ,FYRQ1=null ,FYRQ2=null where FPRN='{0}'";
            try
            {
                strSQL = string.Format(strSQL, this.PatientNoChang(FPRN.PadLeft(10,'0').Substring(this.PatientNoSubstr())));
            }
            catch
            {
                return -1;
            }
            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// 更改HIS_BA6  t.FQRQ1 t.FQRQ2  为空
        /// </summary>
        /// <returns></returns>
        public int UpdateHISBA6FQRQ(string FPRN)
        {
            //t.FZRQ1 t.FZRQ2  
            string strSQL = @"update HIS_BA6 set FQJY=null,FQCS=null ,FQTS=null,FQRQ1=null ,FQRQ2=null where FPRN='{0}'";
            try
            {
               // strSQL = string.Format(strSQL, this.PatientNoChang(FPRN.Substring(this.PatientNoSubstr())));
           strSQL = string.Format(strSQL, this.PatientNoChang(FPRN.PadLeft(10, '0').Substring(this.PatientNoSubstr())));



            }
            catch
            {
                return -1;
            }
            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// 更改HIS_BA6  t.FZRQ1 t.FZRQ2为空
        /// </summary>
        /// <returns></returns>
        public int UpdateHISBA6FZRQ(string FPRN)
        {
            string strSQL = @"update HIS_BA6 set FZJY=null, FZCS=null, FZTS=null ,FZRQ1=null ,FZRQ2=null where FPRN='{0}'";
            try
            {
                strSQL = string.Format(strSQL, this.PatientNoChang(FPRN.PadLeft(10, '0').Substring(this.PatientNoSubstr())));
            }
            catch
            {
                return -1;
            }
            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
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
            string sql = @"INSERT INTO HIS_BA7
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
                                                 this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr())),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 this.ChangeDateTime(obj.CureDate.ToString()),//化疗起始日期
                                                 this.ChangeDateTime(obj.OperInfo.OperTime.ToString()),//化疗终止日期
                                               //  obj.DrugInfo.Name + "(" + obj.Qty.ToString() + obj.Unit.ToString() + ")",//化疗药物名称及剂量
                                                 obj.DrugInfo.Name + "(" + obj.Qty.ToString() + UnitListHelper.GetName(obj.Unit) + ")",
                                                 obj.Period.ToString(),//化疗疗程
                                                 obj.Result.ToString(),//疗效编号
                                                 "" //疗效
                                                 );
            }
            catch
            {
            }
            sql = sql.Replace("'NULL'", "NULL");
            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }

        #region HIS_BA9

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public int DeleteHISBA9(string inpatientNO, int times)
        {
            string strSQL = @"delete from HIS_BA9 where fprn = '{0}' and ftimes={1}";

            strSQL = string.Format(strSQL, inpatientNO, times);

            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }

        /// <summary>
        /// HIS_BA9  --病人诊断信息
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertHISBA9Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Diagnose obj, Neusoft.HISFC.Models.HealthRecord.TumourDetail obj2)
        {
            if (obj.DiagInfo.DiagType.ID == "16")
            {
                obj.DiagInfo.DiagType.ID = "f";
            }
            string sql = @"insert into HIS_BA9 (FID,FPRN,FTIMES,FZDLX,FICDM,FFJICDM,FFJJBNAME,FFJZLJGBH,FFJZLJG,FPX,FFRYBQBH,FFRYBQ) 
                                        values ('{0}',{1},'{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')";
            string patientNO = patientInfo.PatientInfo.PID.PatientNO.PadLeft(10, '0');
            try
            {
                sql = string.Format(sql,patientInfo.PatientInfo.ID+obj.DiagInfo.HappenNo.ToString(),
                                                 this.PatientNoChang(patientNO.Substring(this.fun.PatientNoSubstr())),
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),
                                                 obj.DiagInfo.DiagType.ID,//对照
                                                 obj.DiagInfo.ICD10.ID,
                                                 obj.DiagInfo.ICDF10.ID,
                                                 obj.DiagInfo.ICDF10.Name,
                                                 obj2.Period.ToString(),//化疗疗程
                                                 obj2.Result.ToString(),//疗效编号
                                                 obj.DiagInfo.HappenNo.ToString(),
                                                 obj.DiagOutState,
                                                 "");
            }
            catch
            {
            }

            ReadSQL(sql);

            if (this.cmd.ExecuteNonQuery() < 0)
            {
                return -1;
            }
            return 1;
        }
        #endregion


        #region 转换处理函数
        /// <summary>
        /// 根据常数“DEPTUPLOAD1”mark字段转换科室名称
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="deptName">科室名称</param>
        private string ConverDeptName(string deptCode, string deptName)
        {
            Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("DEPTUPLOAD1", deptCode);
            if (obj == null)
            {
                return deptName;
            }
            string strReturn = obj.Memo;

            if (strReturn == "")
            {
                return deptName;
            }
            else
            {
                return strReturn;
            }
        }

        /// <summary>
        /// 单引号字符 转换 
        /// 将“ '” 转换成 “’”
        /// </summary>
        /// <param name="Character">字符串</param>
        /// <returns></returns>
        private string ChangeCharacter(string Character)
        {
            Character = Character.Replace("'", "’");
            return Character;
        }


        /// <summary>
        /// 根据常数“DOCTORUPLOAD”mark字段转换医生工号
        /// </summary>
        /// <param name="DocCode">医生工号</param>
        private string ConverDoc(string DocCode)
        {
            Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("DOCTORUPLOAD", DocCode);
            if (obj == null)
            {
                return DocCode;
            }
            string strReturn = obj.Memo;

            if (strReturn == "")
            {
                return DocCode;
            }
            else
            {
                return strReturn;
            }
        }
        /// <summary>
        /// 时间转换
        /// sqlserver 不认0001-01-01
        /// </summary>
        /// <param name="dtStr">日期转换</param>
        /// <returns></returns>
        private string ChangeDateTime(string dtStr)
        {
            string retStr = string.Empty;
            DateTime dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(dtStr);

            if (dt.Date.Year < 1990)
            {
                retStr = "NULL";
            }
            else
            {
                retStr = dtStr;
            }
            return retStr;
        }

        /// <summary>
        ///  上传病案号位数
        ///  没有设置常数：返回8位 否则按照实际返回
        /// </summary>
        /// <returns></returns>
        private int PatientNoSubstr()
        {
            int ret = 2;//8位 
            Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("CASEPNOSUBSTR", "1");
            //无维护情况上传8位
            if (obj == null)
            {
                ret = 2;
                return ret;
            }
            if (obj.Memo == "")
            {
                ret = 2;
                return ret;
            }
            else if (obj.Memo.ToUpper() == "TRUE")
            {
                ret = 2;
                return ret;
            }
            else
            {
                int uplaodNum = 0;
                try
                {
                    uplaodNum = Neusoft.FrameWork.Function.NConvert.ToInt32(obj.Memo);
                }
                catch
                {
                    uplaodNum = 0;
                }
                if (uplaodNum == 0)
                {
                    ret = 2;
                    return ret;
                }
                else
                {
                    ret = 10 - uplaodNum;
                }
            }
            return ret;
        }
        #endregion
        #endregion 
        #region 住院日志 2012-6-26 整理
        /// <summary>
        /// HIS_ZYLOG -- 病房动态日志
        /// 不插入这个HIS临时表，改为插入正式的TZyWardWorkDayReport
        /// </summary>
        /// <param name="alPatientMove">动态日报集合</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isExist">是否有数据被覆盖</param>
        /// <returns></returns>
        public int InsertTZyWardWorklog(System.Collections.ArrayList alPatientMove, string beginDate, string endDate, ref int intDelete)
        {
            this.IsOpen();
            string strSQL = "delete TZyWardWorklog where FRQ = '{0}' and FTykh= '{1}'";

            string strsql = @"INSERT INTO TZyWardWorklog
  (FRQ,
FTykh,
FKsName,
FBEDSUM,
FStatFlag,
FYRS,
FRYRS,
FTKZR,
FTQZR,
FCYRS,
FDEADRS,
FZRTK,
FZRTQ,
FXYRS,
FPRS,
FBCZYL,
FWorkrq
)
VALUES
  (
   '{0}',
   '{1}',
   '{2}',
   {3},
   '{4}',
   {5},
   {6},
   {7},
   {8},
   {9},
   {10},
   {11},
   {12},
   {13},
   {14},
   {15},
   '{16}'
)";
            string temp = string.Empty;
            int intReturn = 0;
            foreach (Neusoft.HISFC.Models.HealthRecord.Case.PatientMove patientMove in alPatientMove)
            {
                int j = 0;
                this.cmd.CommandText = string.Empty;
                this.cmd.CommandText = string.Format(strSQL, patientMove.OperDate.ToShortDateString().Replace('-', '/'), patientMove.DeptCode);
                try
                {
                    j = this.cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                }
                if (j > 0) intDelete += j;


                this.cmd.CommandText = string.Empty;
                temp = strsql;
                this.cmd.CommandText = string.Format(temp,
                                                 patientMove.OperDate.ToShortDateString().Replace('-', '/'),
                                                 patientMove.DeptCode,
                                                 patientMove.OperDept,
                                                 patientMove.BedNum.ToString(),
                                                 "***",
                                                 patientMove.OriginalNum.ToString(),
                                                 patientMove.InNum.ToString(),
                                                 patientMove.OtherDeptIn.ToString(),
                                                 patientMove.OtherRegionIn.ToString(),
                                                 patientMove.OutNum.ToString(),
                                                 patientMove.DeadNum.ToString(),
                                                 patientMove.ToOtherDept.ToString(),
                                                 patientMove.ToOtherRegion.ToString(),
                                                 patientMove.PatientNum.ToString(),
                                                 patientMove.AccompanyNum.ToString(),
                                                 patientMove.BeduseNum.ToString(),//**床位占用率,没写算法，要在表现层赋值**
                                                 patientMove.Memo.Replace('-', '/')
                                                 );

                if (this.cmd.ExecuteNonQuery() <= 0)
                {
                    return -1;
                }
                intReturn++;
            }
            //if (intDelete > 0) isExist = true;
            return intReturn;
        }

        #endregion
        private string PatientNoChang(string patientNo)
        {
            string ret = string.Empty;
            //是否需要转换
            ArrayList al = this.constMana.GetList("CasePatientNoChang");
            if (al != null && al.Count>0)//需要转换
            {
                ret = patientNo.Replace("v", "V");
              
                if (ret.IndexOf("V") >= 0)
                {
                    ret = ret.Replace("V", "0");
                    ret = "V" + ret.Substring(1);
                }
                else
                {
                    ret = patientNo;
                }
            }
            else 
            {
                ret= patientNo;
            }
            return ret;
        }

        public int GetBATimes(string inpatientno,string patientno,string date)
        {
            string sql = @"SELECT t.FTIMES  from tpatientvisit t where  t.fzyid='{0}'";
            sql = string.Format(sql, inpatientno);

            ReadSQL(sql);
            this.reader = this.cmd.ExecuteReader();
            try
            {
                while (this.reader.Read())
                {
                    return int.Parse(this.reader[0].ToString());
                }
                return 0;
            }
            catch (Exception ex)
            {
                this.Err1 = ex.Message;
                return 0;
            }
            finally
            {
                this.reader.Close();
            }
        }
    }
}