using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;
using Neusoft.HISFC.Models.Base;
using System.Collections;
using Neusoft.FrameWork.Function;
namespace Neusoft.HISFC.Components.InpatientFee.Register.Classes
{
    public class Function :Neusoft.FrameWork.Management.Database
    {

    
        #region 私有函数

        /// <summary>
        /// 查询患者信息主SQL(ZL)
        /// </summary>
        /// <returns>FIN_IPR_INMAININFO SQL 语句</returns>
        private string PatientQuerySelectALL()
        {
            string sql = string.Empty;
            if (Sql.GetSql("RADT.Inpatient.OldPatientQuery.Select.All", ref sql) == -1)
            {
                this.Err = Sql.Err;
                return null;
            }
            return sql;
        }

        /// <summary>
        /// 根据SQL语句查询旧系统患者基本信息
        /// </summary>
        /// <param name="SQLPatient"></param>
        /// <returns></returns>
        private ArrayList myPatientQuery(string SQLPatient)
        {
             
            ArrayList al = new ArrayList();
            Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo;
            ProgressBarText = "正在查询患者...";
            ProgressBarValue = 0;
            //取系统时间,用来得到年龄字符串
            DateTime sysDate = GetDateTimeFromSysDateTime();
            if (ExecQuery(SQLPatient) == -1)
            {
                return null;
            }

            try
            {
                while (Reader.Read())
                {
                    PatientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

     

                    try
                    {
                        if (!Reader.IsDBNull(0)) PatientInfo.PID.CardNO = Reader[0].ToString(); // 住院号
                        if (!Reader.IsDBNull(0)) PatientInfo.PID.PatientNO = Reader[0].ToString(); // 住院号
                        if (!Reader.IsDBNull(1)) PatientInfo.Card.ICCard.ID = Reader[1].ToString(); //IC卡号
                        if (!Reader.IsDBNull(2)) PatientInfo.Name = Reader[2].ToString(); //  姓名						
                        if (!Reader.IsDBNull(3)) PatientInfo.SpellCode = Reader[3].ToString(); //拼音码
                        if (!Reader.IsDBNull(4)) PatientInfo.WBCode = Reader[4].ToString(); // 五笔码
                        if (!Reader.IsDBNull(5)) PatientInfo.Birthday = NConvert.ToDateTime(Reader[5]); // 生日
                        if (!Reader.IsDBNull(6)) PatientInfo.Sex.ID = Reader[6].ToString(); //   性别
                        if (!Reader.IsDBNull(7)) PatientInfo.IDCard = Reader[7].ToString(); //  身份证号
                        if (!Reader.IsDBNull(8)) PatientInfo.BloodType.ID = Reader[8].ToString(); //  血型
                        if (!Reader.IsDBNull(9)) PatientInfo.Profession.ID = Reader[9].ToString(); //  职业
                        if (!Reader.IsDBNull(10)) PatientInfo.CompanyName = Reader[10].ToString(); // 单位名称
                        if (!Reader.IsDBNull(11)) PatientInfo.PhoneBusiness = Reader[11].ToString(); //  单位电话
                        if (!Reader.IsDBNull(12)) PatientInfo.User01 = Reader[12].ToString(); //单位邮编
                        if (!Reader.IsDBNull(13)) PatientInfo.AddressHome = Reader[13].ToString(); //  家庭地址
                        if (!Reader.IsDBNull(14)) PatientInfo.PhoneHome = Reader[14].ToString(); //  家庭电话
                        if (!Reader.IsDBNull(15)) PatientInfo.User02 = Reader[15].ToString(); //  家庭邮编
                        if (!Reader.IsDBNull(16)) PatientInfo.DIST = Reader[16].ToString(); //  籍贯
                        if (!Reader.IsDBNull(17)) PatientInfo.Nationality.ID = Reader[17].ToString(); //  民族
                        if (!Reader.IsDBNull(18)) PatientInfo.Kin.Name = Reader[18].ToString(); //  联系人姓名
                        if (!Reader.IsDBNull(19)) PatientInfo.Kin.RelationPhone = Reader[19].ToString(); // 联系人电话
                        if (!Reader.IsDBNull(20)) PatientInfo.Kin.RelationAddress = Reader[20].ToString(); //  联系人地址
                        if (!Reader.IsDBNull(21)) PatientInfo.Kin.Relation.ID = Reader[21].ToString(); //  与患者关系
                        if (!Reader.IsDBNull(22)) PatientInfo.MaritalStatus.ID = Reader[22].ToString(); // 婚姻状况
                        if (!Reader.IsDBNull(23)) PatientInfo.Country.ID = Reader[23].ToString(); //  国籍             
                        if (!Reader.IsDBNull(24)) PatientInfo.Pact.PayKind.ID = Reader[24].ToString(); //  合同类别
                        if (!Reader.IsDBNull(25)) PatientInfo.Pact.PayKind.Name = Reader[25].ToString(); //  合同类别名称
                        if (!Reader.IsDBNull(26)) PatientInfo.Pact.ID = Reader[26].ToString(); //  合同单位
                        if (!Reader.IsDBNull(27)) PatientInfo.Pact.Name = Reader[27].ToString(); //  合同单位名称

                        if (!Reader.IsDBNull(28)) PatientInfo.SSN = Reader[28].ToString(); //  医疗证号
                        if (!Reader.IsDBNull(29)) PatientInfo.AreaCode = Reader[29].ToString(); //  出生地
                        if (!Reader.IsDBNull(30)) PatientInfo.FT.TotCost = NConvert.ToDecimal(Reader[30]); //  医疗费用
                        if (!Reader.IsDBNull(31)) PatientInfo.Disease.IsAlleray = NConvert.ToBoolean(Reader[31]); //  药物过敏
                       
                        if (!Reader.IsDBNull(41)) PatientInfo.InTimes = NConvert.ToInt32(Reader[41]); //入院次数
                        
                    }
                    catch (Exception ex)
                    {
                        Err = ex.Message;
                        WriteErr();
                        if (!Reader.IsClosed)
                        {
                            Reader.Close();
                        }
                        return null;
                    }

                    ProgressBarValue++;
                    al.Add(PatientInfo);
                }
            } //抛出错误
            catch (Exception ex)
            {
                Err = "获得患者基本信息出错！" + ex.Message;
                ErrCode = "-1";
                WriteErr();
                if (!Reader.IsClosed)
                {
                    Reader.Close();
                }
                return al;
            }
            Reader.Close();

            ProgressBarValue = -1;
            return al;
        }

        #endregion

        /// <summary>
        /// 查询旧系统患者
        /// </summary>
        /// <param name="patientName">姓名</param>
        /// <param name="sexcode">性别：1 男；2 女</param>
        /// <returns></returns>
        public ArrayList GetOldSysPatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string sqlWhere)
        {
             
            ArrayList al = new ArrayList();
            string sqlSelectALL_ZL = string.Empty;
            string sql2 = string.Empty;

            sqlSelectALL_ZL = this.PatientQuerySelectALL();

            if (sqlSelectALL_ZL == null)
            {
                return null;
            }
            sql2 += sqlSelectALL_ZL + " " + sqlWhere;

            ArrayList oldPatientList = this.myPatientQuery(sql2);
            if (oldPatientList == null)
            {
                oldPatientList = new ArrayList();
            }

            return oldPatientList;
        }

        /// <summary>
        /// 根据身份证号获取生日
        /// </summary>
        /// <param name="idNO">身份证号</param>
        /// <returns></returns>
        public static string GetBirthDayFromIdNO(string idNO, ref string err)
        {

            if (Neusoft.FrameWork.WinForms.Classes.Function.CheckIDInfo(idNO, ref err) < 0)
            {
                return "-1";
            }
            if (idNO.Length == 15)
            {
                idNO = Neusoft.FrameWork.WinForms.Classes.Function.TransIDFrom15To18(idNO);
            }
            string datestr = idNO.Substring(6, 8);
            string year = datestr.Substring(0, 4);
            string month = datestr.Substring(4, 2);
            string day = datestr.Substring(6, 2);
            datestr = year + "-" + month + "-" + day;
            return datestr;
        }

        /// <summary>
        /// 根据身份证号获取性别
        /// </summary>
        /// <param name="idNO">身份证号</param>
        /// <returns></returns>
        public static Neusoft.FrameWork.Models.NeuObject GetSexFromIdNO(string idNO, ref string err)
        {
            if (Neusoft.FrameWork.WinForms.Classes.Function.CheckIDInfo(idNO, ref err) < 0)
            {
                return null;
            }

            if (idNO.Length == 15)
            {
                idNO = Neusoft.FrameWork.WinForms.Classes.Function.TransIDFrom15To18(idNO);
            }

            int flag = Neusoft.FrameWork.Function.NConvert.ToInt32((idNO.Substring(16, 1)));
            Neusoft.FrameWork.Models.NeuObject sexobj = new Neusoft.FrameWork.Models.NeuObject();
            Neusoft.HISFC.Models.Base.SexEnumService sexlist = new Neusoft.HISFC.Models.Base.SexEnumService();
            if (flag % 2 == 0)
            {
                sexobj.ID = Neusoft.HISFC.Models.Base.EnumSex.F.ToString();
                sexobj.Name = sexlist.GetName(Neusoft.HISFC.Models.Base.EnumSex.F);
            }
            else
            {
                sexobj.ID = Neusoft.HISFC.Models.Base.EnumSex.M.ToString();
                sexobj.Name = sexlist.GetName(Neusoft.HISFC.Models.Base.EnumSex.M);
            }
            return sexobj;
        }

        public string TransMariStr(int mari)
        {
            if (((int)EnumMaritalStatus.D) == mari)
                return "D";
            else if (((int)EnumMaritalStatus.M) == mari)
                return "M";
            else if (((int)EnumMaritalStatus.S) == mari)
                return "S";
            else if (((int)EnumMaritalStatus.W) == mari)
                return "W";
            else
                return "A";
        }
    }

    /// <summary>
    /// 清单查询
    /// </summary>
    public class QueryDayFee : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 查询患者清单
        /// </summary>
        /// <param name="beginTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="inPatientNo">住院号</param>
        /// <returns></returns>
        public int QueryDataSetBySql(string SQL, DateTime beginTime, DateTime endTime, string inPatientNo, string deptCode, string inState, ref DataSet ds)
        {
            if (ds == null || ds.Tables.Count > 0)
            {
                ds = new DataSet();
            }

            if (SQL.Trim() == "")
            {
                this.Err = "获得查询SQL失败，请维护SQL！";
                return -1;
            }

            string querySql = string.Empty;

            if (this.Sql.GetSql(SQL, ref querySql) == -1)
            {
                this.Err = "获取Sql语句出错，索引号：" + SQL;
                WriteErr();
                return -1;
            }

            try
            {
                querySql = string.Format(querySql, inPatientNo, beginTime.ToString(), endTime.ToString(), deptCode, inState);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return -1;
            }

            return this.ExecQuery(querySql, ref ds);
        }
    }
}
