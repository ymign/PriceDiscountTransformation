using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Data;
using System.Xml;

namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDong
{
    /// <summary>
    /// 重写广东省病案接口DRGS；去除不需要的方法
    /// 
    /// </summary>
    public class UpLoadCaseInterfaceDrgs : Neusoft.FrameWork.Management.Database
    {
        private SqlConnection conn = new SqlConnection();
        private SqlCommand cmd = new SqlCommand();
        private SqlTransaction transaction = null;
        private SqlDataReader reader;

        /// <summary>
        /// 广东省病案服务数据串保存读取路径
        /// 
        /// </summary>
        private string profileName = System.Windows.Forms.Application.StartupPath + @".\Profile\CaseDataBase.xml";
        /// <summary>
        /// 常数业务类
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Constant constBizLogic = new Neusoft.HISFC.BizLogic.Manager.Constant();

        Neusoft.HISFC.BizLogic.Manager.Constant constMana = new Neusoft.HISFC.BizLogic.Manager.Constant();
        /// <summary>
        /// 科室业务类
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Department deptBizLogic = new Neusoft.HISFC.BizLogic.Manager.Department();
        /// <summary>
        /// 病案首页业务类
        /// </summary>
        Neusoft.HISFC.BizLogic.HealthRecord.Base baseBizLogic = new Neusoft.HISFC.BizLogic.HealthRecord.Base();

        string err = "";
        /// <summary>
        /// 错误信息
        /// </summary>
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
        public UpLoadCaseInterfaceDrgs()
        {
            this.conn.ConnectionString = this.GetConnectString();
            this.conn.Open();
            this.transaction = this.conn.BeginTransaction();
            this.cmd.Connection = this.conn;
            this.cmd.Transaction = transaction;

            CreatFile();
        }
        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            this.transaction.Commit();
        }
        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback()
        {
            this.transaction.Rollback();
        }
        /// <summary>
        /// 连接
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                this.IsOpen();
                return this.conn;
            }
        }
        /// <summary>
        /// 是否打开连接
        /// </summary>
        /// <returns></returns>
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
        /// 根据住院流水号判断是否已经录入
        /// 返回值： 0 需要上传 1 已经上传
        /// </summary>
        /// <param name="fzyid">住院流水号</param>
        /// <returns></returns>
        public int GetIsNeedUpload(string fzyid)
        {
            int iReturn = 0;
            int NotNeed = 0;
            int Need = 0;
            string strSQLNotNeed = @"select count(1) from tPatientVisit where fzyid = '{0}'";
            string strSQLNeed = @"select count(1) from HIS_BA1 where fzyid = '{0}'";
            try
            {
                strSQLNotNeed = string.Format(strSQLNotNeed, fzyid);

                strSQLNeed = string.Format(strSQLNeed, fzyid);
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

        #region HIS_BA1
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
        ///  HIS_BA1 --病人信息
        /// </summary>
        /// <param name="b">病案首页实体</param>
        /// <param name="alFee">费用信息</param>
        /// <param name="alChangeDepe">转科信息</param>
        /// <param name="alDose"> 诊断</param>
        ///<param name="isMetCasBase">是否病案主表数据</param> 
        /// <returns></returns>
        public int InsertPatientInfoBA1(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            string strSQL = this.GetInsertHISBA1SQL(b, alFee, alChangeDepe, alDose, isMetCasBase);
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
        /// <param name="alFee">费用信息数组</param>
        /// <param name="alChangeDepe">转科信息数组</param>
        /// <param name="alDose">诊断信息数组</param>
        /// <param name="isMetCasBase">true病案首页信息 false 住院主表信息</param>
        /// <returns>失败返回null</returns>
        private string[] GetBaseInfoBA1(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            string[] s = new string[203];
            try
            {
                s[0] = "0";
                s[1] = b.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());//病案号
                s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
                s[3] = "11";
                s[4] = b.PatientInfo.ID;
                if (b.PatientInfo.Age == "0")
                {
                    if (b.AgeUnit == "不存")
                    {
                        s[5] = this.baseBizLogic.GetAgeByFun(b.PatientInfo.Birthday.Date, b.PatientInfo.PVisit.InTime.Date);
                    }
                    else
                    {
                        s[5] = b.AgeUnit.ToString();//年龄
                    }
                }
                else
                {
                    s[5] = b.PatientInfo.Age.ToString();//年龄
                }
                s[6] = b.PatientInfo.Name;//姓名
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
                s[11] = b.PatientInfo.IDCard.ToString();//身份证号
                if (b.PatientInfo.Country.ID.ToString() == "1")//中国  需要转换其他
                {
                    s[12] = "A156";
                    s[13] = "中国";
                }
                else
                {
                    Neusoft.FrameWork.Models.NeuObject countryObj = this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                    if (countryObj != null && countryObj.ID != "")
                    {
                        if (countryObj.Memo != "")
                        {
                            s[12] = countryObj.Memo.ToString(); //国籍编号
                            s[13] = countryObj.Name.ToString(); //国籍
                        }
                        else
                        {
                            s[12] = countryObj.ID.ToString(); //国籍编号
                            s[13] = countryObj.Name.ToString(); //国籍
                        }
                    }
                    else
                    {
                        s[12] = b.PatientInfo.Country.ID.ToString(); //国籍编号
                        s[13] = ""; //国籍
                    }
                }

                Neusoft.FrameWork.Models.NeuObject NationObj = this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                if (NationObj != null && NationObj.ID != "")
                {
                    if (NationObj.Memo != "")
                    {
                        s[14] = NationObj.Memo; //民族编号
                        s[15] = NationObj.Name; //民族
                    }
                    else
                    {
                        s[14] = NationObj.ID; //民族编号
                        s[15] = NationObj.Name; //民族
                    }
                }
                else
                {
                    s[14] = b.PatientInfo.Nationality.ID; //民族编号
                    s[15] = ""; //民族
                }
                //add by chengym 2011-6-15  字典表的名称字段varchar（50） 有些执业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                Neusoft.FrameWork.Models.NeuObject JobObj = this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.PROFESSION, b.PatientInfo.Profession.ID.ToString());
                if (JobObj != null && JobObj.ID != "")
                {
                    if (JobObj.Memo != "")
                    {
                        if (JobObj.Memo.Length <= 100)
                        {
                            s[16] = JobObj.Memo;
                        }
                        else
                        {
                            s[16] = JobObj.Memo.Substring(0, 100);
                        }
                    }
                    else
                    {
                        if (JobObj.Name.Length <= 100)
                        {
                            s[16] = JobObj.Name;
                        }
                        else
                        {
                            s[16] = JobObj.Name.Substring(0, 100);
                        }
                    }
                }
                else
                {
                    s[16] = b.PatientInfo.Profession.ID.ToString(); //职业 没有传中文不知道是否可以
                }
                if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1")
                {
                    s[17] = "1"; //婚姻状况编号
                    s[18] = "未婚"; //婚姻状况
                }
                else if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2")
                {
                    s[17] = "2";
                    s[18] = "已婚";
                }
                else if (b.PatientInfo.MaritalStatus.ID.ToString() == "3")
                {
                    s[17] = "3";
                    s[18] = "离婚";
                }
                else if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4")
                {
                    s[17] = "5";
                    s[18] = "其他";
                }
                else if (b.PatientInfo.MaritalStatus.ID.ToString() == "R" || b.PatientInfo.MaritalStatus.ID.ToString() == "5")
                {
                    s[17] = "5";
                    s[18] = "其他";
                }
                else if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                {
                    s[17] = "5";
                    s[18] = "其他";
                }
                else if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "6")
                {
                    s[17] = "4";
                    s[18] = "丧偶";
                }

                s[19] = b.PatientInfo.AddressBusiness.ToString();  //工作单位及地址
                s[20] = b.PatientInfo.AddressBusiness.ToString();//单位地址
                s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                s[22] = b.PatientInfo.BusinessZip;//单位邮编      
                s[23] = b.PatientInfo.AddressHome;//家庭住址
                s[24] = b.PatientInfo.HomeZip;//住址邮编
                s[25] = b.PatientInfo.Kin.Name;//联系人
                Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constBizLogic.GetConstant("RELATIVE", b.PatientInfo.Kin.RelationLink);
                if (RelativeObj != null && RelativeObj.ID != "")
                {
                    if (RelativeObj.Memo != "")
                    {
                        if (RelativeObj.Memo.Length <= 20)
                        {
                            s[26] = RelativeObj.Memo;//与患者关系
                        }
                        else
                        {
                            s[26] = RelativeObj.Memo.Substring(0, 20);//与患者关系
                        }
                    }
                    else
                    {
                        if (RelativeObj.Name.Length <= 20)
                        {
                            s[26] = RelativeObj.Name;//与患者关系
                        }
                        else
                        {
                            s[26] = RelativeObj.Name.Substring(0, 20);//与患者关系
                        }
                    }
                }
                else
                {
                    s[26] = b.PatientInfo.Kin.RelationLink;//与患者关系
                }
                s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话
                s[29] = b.PatientInfo.SSN; //其他医疗保险卡号
                s[30] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                s[31] = b.PatientInfo.PVisit.InTime.Hour.ToString().PadLeft(2, '0'); //入院时间
                s[32] = this.ConverDept(b.InDept.ID);//入院科室代码
                s[33] = this.ConverDeptName(b.InDept.ID, b.InDept.Name);//出院科室名称2011-6-8
                s[34] = b.InRoom;//入院病室    
                s[35] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                s[36] = b.PatientInfo.PVisit.OutTime.Hour.ToString().PadLeft(2, '0'); //出院时间
                s[37] = this.ConverDept(b.OutDept.ID);//出院科室代码
                s[38] = this.ConverDeptName(b.OutDept.ID, b.OutDept.Name);//出院科室名称2011-6-8
                s[39] = b.OutRoom; //出院病室
                s[40] = b.InHospitalDays.ToString();//住院天数
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
                if (b.PathologicalDiagCode == null)
                {
                    s[45] = b.PathologicalDiagName;
                }
                else
                {
                    s[45] = b.PathologicalDiagCode;
                }
                string anaphyPh = b.FirstAnaphyPharmacy.ID;
                if (anaphyPh.Length > 100)
                {
                    s[46] = this.ChangeCharacter(anaphyPh.Substring(0, 100));
                }
                else
                {
                    s[46] = this.ChangeCharacter(anaphyPh);//药物过敏  
                }
                if (b.PiPo == null || b.PiPo == "")
                {
                    s[47] = "1";
                    s[48] = "符合";
                }
                else
                {
                    s[47] = b.PiPo;//入院与出院诊断符合情况编号
                    s[48] = this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.PiPo).Name;//入院与出院诊断符合情况
                }
                if (b.ClPa == null || b.ClPa == "")
                {
                    s[49] = "1";
                    s[50] = "符合";
                }
                else
                {
                    s[49] = b.ClPa;//临床与病理诊断符合情况编号
                    s[50] = this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.ClPa).Name;//临床与病理诊断符合情况
                }
                s[51] = b.SalvTimes.ToString();//抢救次数
                s[52] = b.SuccTimes.ToString();//成功次数

                s[53] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID);
                s[54] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
                s[55] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID);
                s[56] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
                s[57] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID);//主治医师姓名
                s[58] = b.PatientInfo.PVisit.AttendingDoctor.Name;
                s[59] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID);//住院医师姓名
                s[60] = b.PatientInfo.PVisit.AdmittingDoctor.Name;
                s[61] = this.ConverDoc(b.RefresherDoc.ID);//进修医生
                s[62] = b.RefresherDoc.Name;
                s[63] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID);
                s[64] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                s[65] = this.ConverDoc(b.CodingOper.ID);//编码员名称
                s[66] = b.CodingOper.Name;
                s[67] = this.ConverDoc(b.OperInfo.ID);
                s[68] = b.OperInfo.Name;//操作员名称（病案整理者）
                s[69] = b.MrQuality.ToString();//病案质量 
                s[70] = this.constBizLogic.GetConstant("CASEQUALITY", b.MrQuality).Name;
                s[71] = this.ConverDoc(b.QcDoc.ID);//质控医师名称
                s[72] = b.QcDoc.Name;
                s[73] = this.ConverDoc(b.QcNurse.ID);
                s[74] = b.QcNurse.Name;//质控护士名称
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

                s[76] = "0.00";//总费用
                s[77] = "0.00";//西药费
                s[78] = "0.00";//中药费
                s[79] = "0.00";//中成药费
                s[80] = "0.00";//中草药费
                s[81] = "0.00";//其他费
                if (b.CadaverCheck.ToString() == "0")
                {
                    s[82] = "2";//尸检
                }
                else
                {
                    s[82] = b.CadaverCheck;//尸检
                }
                s[83] = this.constBizLogic.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
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
                    s[84] = "3";
                    s[85] = b.PatientInfo.BloodType.ID.ToString();
                }
                else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                {
                    s[84] = "4";
                    s[85] = b.PatientInfo.BloodType.ID.ToString();
                }
                else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                {
                    s[84] = "9";
                    s[85] = "未查";
                }
                else
                {
                    s[84] = "5";
                    s[85] = "其他";
                }
                s[86] = b.RhBlood;
                if (b.RhBlood.ToString() == "1")
                {
                    s[87] = "阴";
                }
                else if (b.RhBlood.ToString() == "2")
                {
                    s[87] = "阳";
                }
                else
                {
                    s[87] = "未查";
                }

                s[88] = "0";//婴儿数chengym
                s[89] = "0";//是否部分病种，1是 0否
                s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                s[91] = "";//首次转科科别
                s[92] = "";//首次转科日期
                s[93] = "";//首次转科时间
                s[94] = "";//输入员编号
                s[95] = "";//输入员
                s[96] = "";//输入日期Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.HISFC.Models.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');
                s[97] = "";//疾病分型编号
                s[98] = "";//疾病分型
                s[99] = "";//复合归档编号
                s[100] = "";//复合归档
                s[101] = "";//病人来源编号
                s[102] = "";//病人来源
                s[103] = "";//是否手术 chengym
                s[104] = "";//是否输入妇婴卡
                s[105] = "0";//医院感染次数，不允许为空，否则影响报表统计结果 chengym
                s[106] = "";//扩展1
                s[107] = "";
                s[108] = "";
                s[109] = "";
                s[110] = "";
                s[111] = "";
                s[112] = "";
                s[113] = "";
                s[114] = "";
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
                s[126] = b.BabyBirthWeight;//新生儿出生体重
                s[127] = b.BabyInWeight;//新生儿入院体重
                s[128] = b.InPath;//入院途径编号
                s[129] = this.constBizLogic.GetConstant("CASEINAVENUE", b.InPath).Name;//入院途径
                s[130] = b.ClinicPath;//临床路径病例编号
                if (b.ClinicPath == "1")
                {
                    s[131] = "是";//临床路径病例
                }
                else
                {
                    s[131] = "否";//临床路径病例
                }
                s[132] = b.PathologicalDiagCode;//病理疾病编码
                s[133] = b.PathNum;//病理号
                s[134] = b.AnaphyFlag;//是否药物过敏编号
                if (b.AnaphyFlag == "1")
                {
                    s[135] = "无";//是否药物过敏
                }
                else
                {
                    s[135] = "有";//是否药物过敏
                }
                s[136] = b.DutyNurse.ID;//责任护士编号
                s[137] = b.DutyNurse.Name;//责任护士
                s[138] = b.Out_Type;//离院方式编号
                s[139] = "";//离院方式
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
                s[153] = b.PatientInfo.Pact.ID;//付款方式编号
                s[154] = this.constBizLogic.GetConstant("CASEPACT", b.PatientInfo.Pact.ID).Name;//付款方式

                s[155] = "";//住院总费用：自费金额
                s[156] = "";//综合医疗服务类：（1）一般医疗服务费
                s[157] = "";//综合医疗服务类：（2）一般治疗操作费
                s[158] = "";//综合医疗服务类：（3）护理费
                s[159] = "";//综合医疗服务类：（4）其他费用
                s[160] = "";//诊断类：(5) 病理诊断费
                s[161] = "";//诊断类：(6) 实验室诊断费
                s[162] = "";//诊断类：(7) 影像学诊断费
                s[163] = "";//诊断类：(8) 临床诊断项目费
                s[164] = "";//治疗类：(9) 非手术治疗项目费
                s[165] = "";//治疗类：非手术治疗项目费 其中临床物理治疗费
                s[166] = "";//治疗类：(10) 手术治疗费
                s[167] = "";//治疗类：手术治疗费 其中麻醉费
                s[168] = "";//治疗类：手术治疗费 其中手术费
                s[169] = "";//康复类：(11) 康复费
                s[170] = "";//中医类：中医治疗类
                s[171] = "";//西药类： 西药费 其中抗菌药物费用
                s[172] = "";//血液和血液制品类： 血费
                s[173] = "";//血液和血液制品类： 白蛋白类制品费
                s[174] = "";//血液和血液制品类： 球蛋白制品费
                s[175] = "";//血液和血液制品类：凝血因子类制品费
                s[176] = "";//血液和血液制品类： 细胞因子类费
                s[177] = "";//耗材类：检查用一次性医用材料费
                s[178] = "";//耗材类：治疗用一次性医用材料费
                s[179] = "";//耗材类：手术用一次性医用材料费
                s[180] = "";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                s[181] = "";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                s[182] = "";//中医类：诊断（中医）
                s[183] = "";//中医类：治疗（中医）
                s[184] = "";//中医类：治疗 其中外治（中医）
                s[185] = "";//中医类：治疗 其中骨伤（中医）
                s[186] = "";//中医类：治疗 其中针刺与灸法（中医）
                s[187] = "";//中医类：治疗推拿治疗（中医）
                s[188] = "";//中医类：治疗 其中肛肠治疗（中医）
                s[189] = "";//中医类：治疗 其中特殊治疗（中医）
                s[190] = "";//中医类：其他（中医）
                s[191] = "";//中医类：其他 其中中药特殊调配加工（中医）
                s[192] = "";//中医类：其他 其中辨证施膳（中医）
                s[193] = "";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                s[194] = "";//中医类：治疗 其中骨伤（中医）
                s[195] = "";//中医类：治疗 其中针刺与灸法（中医）
                s[196] = "";//中医类：治疗推拿治疗（中医）
                s[197] = "";//中医类：治疗 其中肛肠治疗（中医）
                s[198] = "";//中医类：治疗 其中特殊治疗（中医）
                s[199] = "";//中医类：其他（中医）
                s[200] = "";//中医类：其他 其中中药特殊调配加工（中医）
                s[201] = "";//中医类：其他 其中辨证施膳（中医）
                s[202] = "";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                return s;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获得接口HIS_BA1 INSERT SQL
        /// </summary>
        /// <param name="b">病案首页的实体类</param>
        /// <param name="alFee">费用信息数组</param>
        /// <param name="alChangeDepe">转科信息数组</param>
        /// <param name="alDose">诊断信息数组</param>
        /// <param name="isMetCasBase">true病案首页信息 false 住院主表信息</param>
        /// <returns></returns>
        private string GetInsertHISBA1SQL(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
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
   FRYCYACCOBH,
   FRYCYACCO,
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
FZCLJGZJF,
FZYLZLF02,
FZYLZLF03,
FZYLZLF04,
FZYLZLF05,
FZYLZLF06,
FZYLQTF,
FZYLQTF01,--200
FZYLQTF02,
FZCLJGZJF  --202
)
  VALUES
  (
'{0}'
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
 {43},
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
{193},
{194},
{195},
{196},
{197},
{198},
{199},
{200},
{201},
{202}
)";

            try
            {

                strReturn = string.Format(strReturn, this.GetBaseInfoBA1(b, alFee, alChangeDepe, alDose, isMetCasBase));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        /// <summary>
        /// 更改fzkdate  fzkrq 为空
        /// </summary>
        /// <returns></returns>
        public int UpdateHISBA1Fzkdate()
        {
            string strSQL = @"update HIS_BA1 set fzkdate=null  where fzkdate<'2000-1-1 00:00:00'";
            ReadSQL(strSQL);

            int intReturn = this.cmd.ExecuteNonQuery();

            if (intReturn < 0)
            {
                return -1;
            }

            return intReturn;
        }
        #endregion
        #region HIS_BA2
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
        /// <param name="patientInfo">病案首页实体类</param>
        /// <param name="obj">转科信息</param>
        /// <returns></returns>
        public int InsertHISBA2(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.RADT.Location obj)
        {
            string sql = @"INSERT INTO HIS_BA2
(FPRN,FTIMES,FZKTYKH,FZKDEPT,FZKDATE,FZKTIME)
VALUES
('{0}','{1}','{2}','{3}','{4}','{5}') ";

            DateTime dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Dept.Memo);

            try
            {
                sql = string.Format(sql,
                                                 patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr()),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 this.ConverDept(obj.Dept.ID),//转科统一科号，HIS接收时存储HIS科号
                                                 obj.Dept.Name,//转科科别
                                                 this.ChangeDateTime(obj.Dept.Memo),//转科日期
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
        #endregion
        #region HIS_BA3
        /// <summary>
        /// 删除 HIS_BA3
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <param name="times">住院次数</param>
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
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">出院诊断信息</param>
        /// <returns></returns>
        public int InsertHISBA3(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Diagnose obj)
        {
            string sql = @"insert into HIS_BA3 (FPRN,FTIMES,FZDLX, FICDVersion, FICDM, FJBNAME,FRYBQBH,FRYBQ) values ('{0}',{1},'{2}',{3},'{4}','{5}','{6}','{7}')";

            try
            {
                sql = string.Format(sql,
                                                 patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr()),
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),
                                                 obj.DiagInfo.DiagType.ID,//对照
                                                 "10",//ICD版本号
                                                 obj.DiagInfo.ICD10.ID,
                                                 obj.DiagInfo.Name,
                                                 obj.DiagOutState,
                                                 obj.Memo);
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
        /// 删除HIS_BA4
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <returns></returns>
        public int DeleteHISBA4(string inpatientNO)
        {
            string strSQL = @"delete from HIS_BA4 where fprn = '{0}' ";

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
        /// insert HIS_BA4  --手术信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">手术信息实体</param>
        /// <returns></returns>
        public int insertHisBa4(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.OperationDetail obj)
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
FOPTYKH --手术医生所在科室编号
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
                if (info.Memo != "" && info.Memo != "true")
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
            try
            {
                sql = string.Format(sql, patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr()),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.HappenNO.ToString(),//手术次数
                                                 obj.OperationInfo.ID.ToString(),//手术码
                                                 this.ChangeCharacter(obj.OperationInfo.Name.ToString()),//手术码对应名称
                                                 this.ChangeDateTime(obj.OperationDate.ToShortDateString()),//手术日期
                                                 obj.NickKind.ToString(), //切口编号
                                                 this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCITYPE, obj.NickKind).Name.ToString(),  //切口
                                                 obj.CicaKind.ToString(),//愈合编号
                                                 this.constBizLogic.GetConstant("CICATYPE", obj.CicaKind).Name.ToString(),  //愈合
                                                 this.ConverDoc(obj.FirDoctInfo.ID),//手术医生编号
                                                 obj.FirDoctInfo.Name,//手术医生
                                                 MarcKind_Code, // obj.MarcKind.ToString(),//麻醉方式编号
                                                 MarcKind_Name, //this.constMana.GetConstant("CASEANESTYPE",obj.MarcKind).Name.ToString(),//麻醉方式
                                                 "0",//是否附加手术
                                                 this.ConverDoc(obj.SecDoctInfo.ID),//I助编号
                                                 obj.SecDoctInfo.Name.ToString(), // I助姓名
                                                 this.ConverDoc(obj.ThrDoctInfo.ID),//II助编号
                                                 obj.ThrDoctInfo.Name.ToString(),//II助姓名
                                                 this.ConverDoc(obj.NarcDoctInfo.ID),//麻醉医生编号
                                                 obj.NarcDoctInfo.Name.ToString(), //麻醉医生
                                                 obj.OperationKind,//择期手术编号1是，0否
                                                 obj.OperationKind,//择期手术编号1是，0否
                                                 "",//手术级别编号
                                                 "",//手术级别
                                                 "",//手术医生所在科室名称
                                                 "" //手术医生所在科室编码
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
        /// 删除HIS_BA5
        /// </summary>
        /// <param name="inpatientNO">病案号</param>
        /// <returns></returns>
        public int DeleteHISBA5(string inpatientNO)
        {
            string strSQL = @"delete from HIS_BA5 where fprn = '{0}' ";

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
        /// insert HIS_BA5 --妇婴信息
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">妇婴卡信息</param>
        /// <returns></returns>
        public int insertHisBa5(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Baby obj)
        {


            string sql = @"insert into HIS_BA5 (FPRN,FTIMES ,FBABYNUM ,FNAME ,FBABYSEXBH ,FBABYSEX ,FTZ ,FRESULTBH ,FRESULT ,
                  FZGBH ,FZG ,FBABYSUC ,FHXBH ,FHX) 
                                         values ('{0}',{1},{2},'{3}','{4}','{5}',{6},'{7}','{8}',
                  '{9}','{10}','{11}','{12}','{13}')";



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
            try
            {
                sql = string.Format(sql, patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr()),
                                                 patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0'),
                                                 obj.HappenNum.ToString(),//婴儿序号
                                                 patientInfo.PatientInfo.Name.ToString(),//病人姓名
                                                 obj.SexCode.ToString(),//婴儿性别编号
                                                 obj.Infect.Memo,//婴儿性别
                                                 obj.Weight.ToString(),//婴儿体重
                                                 obj.BirthEnd.ToString(), //分娩结果编号
                                                 this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CHILDBEARINGRESULT, obj.BirthEnd).Name.ToString(),  //分娩结果
                                                 obj.BabyState,//转归编号
                                                 this.constBizLogic.GetConstant("babyZG", obj.BabyState).Name.ToString(),  //转归
                                                 obj.SuccNum,//婴儿抢救成功次数
                                                 obj.Breath,//呼吸编号
                                                 this.constBizLogic.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.BREATHSTATE, obj.BirthEnd).Name.ToString() //呼吸
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
        /// 删除HIS_BA6
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
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">肿瘤卡信息</param>
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

            try
            {
                sql = string.Format(sql,
                                                 patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr()),//病案号
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
        /// HIS_BA7  --肿瘤化疗记录
        /// </summary>
        /// <param name="patientInfo">病案首页实体</param>
        /// <param name="obj">肿瘤卡肿瘤化疗信息</param>
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
            try
            {
                sql = string.Format(sql,
                                                 patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr()),//病案号
                                                 patientInfo.PatientInfo.InTimes.ToString().TrimStart('0'),//次数
                                                 this.ChangeDateTime(obj.CureDate.ToString()),//化疗起始日期
                                                 this.ChangeDateTime(obj.CureDate.ToString()),//化疗终止日期
                                                 obj.DrugInfo.Name + "(" + obj.Qty.ToString() + obj.Unit.ToString() + ")",//化疗药物名称及剂量
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

        #region  日志报表
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
                sql = this.GetInserttWorkLogSQL(b);

                if (sql == null || sql == "")
                {
                    this.err = this.Err;
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
        /// insert into tWorklog
        /// </summary>
        /// <returns></returns>
        public string GetInserttWorkLogSQL(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            if (b == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            strReturn = @"INSERT INTO tWorklog (FRq,
FTykh,
FKsName,
FTygh,
FDoctorName,
FOccup,
FTyzkcode,
FzkName,
FGs,
FZlrc,
FZkmz,
FRc,
FJkjc,
FSsls,
FJtbc,
FQtrc 
--,FWorkrq
)
VALUES  ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},{10},{11},{12},{13},{14},{15}
--,'{16}'
)";

            try
            {
                strReturn = string.Format(strReturn, this.GetBaseInfotWorkLog(b));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }
        /// <summary>
        /// 将病案Mworklog2实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案Mworklog2的实体类</param>
        /// <returns>失败返回null</returns>
        private string[] GetBaseInfotWorkLog(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string[] s = new string[17];
            try
            {
                s[0] = b.PatientInfo.PVisit.InTime.ToString();
                s[1] = b.InDept.ID;//科室编号
                s[2] = b.InDept.Name;
                s[3] = b.PatientInfo.DoctorReceiver.ID;// 医生工号
                s[4] = b.PatientInfo.DoctorReceiver.Name;//医生名称
                s[5] = b.PatientInfo.DoctorReceiver.Memo;
                s[6] = b.OutDept.ID;
                s[7] = b.OutDept.Name;
                s[8] = b.PatientInfo.Memo;//工时 5
                s[9] = b.PatientInfo.User01;//诊疗人次
                s[10] = b.PatientInfo.User02;//其中：专科门诊
                s[11] = b.PatientInfo.User03;//其中：专家门诊
                s[12] = b.PatientInfo.UserCode;
                s[13] = b.PatientInfo.WBCode;
                s[14] = b.PatientInfo.PVisit.User01;
                s[15] = b.PatientInfo.PVisit.User02;
               //s[16] = b.PatientInfo.PVisit.OutTime.ToString();

                return s;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

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
                sql = string.Format(sql, b.PatientInfo.PVisit.InTime, b.InDept.ID, b.InDept.Name, b.PatientInfo.ID, b.PatientInfo.Memo, b.PatientInfo.User01, b.PatientInfo.User02,
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
                sql = string.Format(sql, b.PatientInfo.PVisit.InTime, b.InDept.ID, b.InDept.Name, b.OutDept.ID, b.OutDept.Name,
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


        /// <summary>
        /// 插入TZyWardWorklog -- 病房动态日志
        /// </summary>
        /// <param name="alPatientMove">动态日报集合</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isExist">是否有数据被覆盖</param>
        /// <returns></returns>
        public int InsertZZyWardWorklog(System.Collections.ArrayList alPatientMove, string beginDate, string endDate, ref int intDelete)
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
                if (j > 0) intDelete += j;


                this.cmd.CommandText = string.Empty;
                temp = strsql;
                this.cmd.CommandText = string.Format(temp,
                                                 patientMove.OperDate.ToShortDateString().Replace('-', '/'),
                                                 "TZY" + oldDeptCode + "*1",
                                                 this.deptBizLogic.GetDeptmentById(patientMove.DeptCode).Name,
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
            return intReturn;
        }


        #endregion

        #region 转换处理函数
        /// <summary>
        /// 根据常数“DEPTUPLOAD”mark字段转换科室编码
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        public string ConverDept(string deptCode)
        {
            Neusoft.FrameWork.Models.NeuObject obj = this.constBizLogic.GetConstant("DEPTUPLOAD", deptCode);
            if (obj == null)
            {
                return deptCode;
            }
            string strReturn = obj.Memo;

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
        /// 根据常数“DEPTUPLOAD1”mark字段转换科室名称
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="deptName">科室名称</param>
        private string ConverDeptName(string deptCode, string deptName)
        {
            Neusoft.FrameWork.Models.NeuObject obj = this.constBizLogic.GetConstant("DEPTUPLOAD1", deptCode);
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
        public string ChangeCharacter(string Character)
        {
            Character = Character.Replace("'", "’");
            return Character;
        }


        /// <summary>
        /// 根据常数“DOCTORUPLOAD”mark字段转换医生工号
        /// </summary>
        /// <param name="DocCode">医生工号</param>
        public string ConverDoc(string DocCode)
        {
            Neusoft.FrameWork.Models.NeuObject obj = this.constBizLogic.GetConstant("DOCTORUPLOAD", DocCode);
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
        public string ChangeDateTime(string dtStr)
        {
            string retStr = string.Empty;
            DateTime dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(dtStr);

            if (dt.Date.Year < 1990)
            {
                retStr = "1990-01-01 00:00:00";
            }
            else
            {
                retStr = dtStr;
            }
            return retStr;
        }

        /// <summary>
        ///  上传病案号位数
        ///  没有设置常数：返回6位 否则按照实际返回
        /// </summary>
        /// <returns></returns>
        private int PatientNoSubstr()
        {
            int ret = 4;//六位 
            Neusoft.FrameWork.Models.NeuObject obj = this.constBizLogic.GetConstant("CASEPNOSUBSTR", "1");
            //无维护情况上传六位
            if (obj == null)
            {
                ret = 4;
                return ret;
            }
            if (obj.Memo == "")
            {
                ret = 4;
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
                    ret = 4;
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

    }
}
