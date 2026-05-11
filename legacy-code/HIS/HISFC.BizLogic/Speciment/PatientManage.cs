using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;
using Neusoft.HISFC.Models.HealthRecord;
using Neusoft.HISFC.Models.RADT;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 病人基本信息管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-22]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class PatientManage : Neusoft.FrameWork.Management.Database
    {
        private PatientInfo patientInfo = new PatientInfo();
        private SpecPatient specPatient = new SpecPatient();

        #region HIS patient 转换 成 Spec Patient
        /// <summary>
        /// HIS patient 转换 成 Spec Patient
        /// </summary>
        /// <returns></returns>
        private void PatientInfoToPa()
        {        
            //家庭住址
            specPatient.Address = patientInfo.AddressHome;
            //籍贯
            specPatient.Home = patientInfo.DIST;
            //联系电话
            specPatient.ContactNum = patientInfo.Kin.RelationPhone;
            //性别
            specPatient.Gender = Convert.ToChar(patientInfo.Sex.ID);
            //家庭电话
            specPatient.HomePhoneNum = patientInfo.PhoneHome;
            //身份证号
            specPatient.IdCardNo = patientInfo.IDCard;
            specPatient.IcCardNo = patientInfo.PID.CardNO;
            specPatient.CardNo = patientInfo.PID.CaseNO;
            //民族
            specPatient.Nation = patientInfo.Nationality.Name;
            //国籍
            specPatient.Nationality = patientInfo.Country.Name;
            //年龄
            specPatient.Birthday = Convert.ToDateTime(patientInfo.Birthday);
            //名称
            specPatient.PatientName = patientInfo.Name;
            //血型
            specPatient.BloodType = patientInfo.BloodType.ID.ToString();
            specPatient.IsMarried = patientInfo.MaritalStatus.ID.ToString();

        }
        #endregion

        /// <summary>
        ///  从His中提取的病人信息
        /// </summary>
        /// <returns></returns>
        private SpecPatient SetSpecPatient()
        {
            SpecPatient patient = new SpecPatient();
            patient.IcCardNo = Reader.IsDBNull(0)? "" : Reader["IC_CARDNO"].ToString();
            patient.CardNo = Reader.IsDBNull(1)? "" : Reader["CARD_NO"].ToString();
            patient.InHosNum = Reader.IsDBNull(2)? "" : Reader["INPATIENT_NO"].ToString();
            patient.PatientName = Reader.IsDBNull(3) ? "" : Reader["NAME"].ToString();
            patient.IdCardNo = Reader.IsDBNull(4)  ? "" : Reader["IDENNO"].ToString();
            patient.Gender = Reader.IsDBNull(5)  ? ' ' : Convert.ToChar(Reader["SEX_CODE"].ToString());
            patient.Birthday = Reader.IsDBNull(6) ? DateTime.MinValue : Convert.ToDateTime(Reader["BIRTHDAY"].ToString());// ("yyyy-MM-dd");
            patient.Address = Reader.IsDBNull(7)  ? "" : Reader["HOME"].ToString();
            patient.Home = Reader.IsDBNull(8)  ? "" : Reader["DISTRICT"].ToString();
            patient.Nation= Reader.IsDBNull(9)  ? "" : Reader["NATION_CODE"].ToString();
            patient.Nationality = Reader.IsDBNull(10)  ? "" : Reader["COUN_CODE"].ToString();
            patient.ContactNum = Reader.IsDBNull(11)  ? "" : Reader["LINKMAN_TEL"].ToString();
            patient.HomePhoneNum = Reader.IsDBNull(12)  ? "" : Reader["HOME_TEL"].ToString();
            patient.BloodType = Reader.IsDBNull(13)  ? "" : Reader["BLOOD_CODE"].ToString();
            patient.IsMarried = Reader.IsDBNull(14)  ? "" : Reader["MARI"].ToString();
            return patient;
        }

        #region 设置参数数组
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="patient">标本库病人实体</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecPatient patient)
        {
            string[] str = new string[]
						{
							patient.PatientID.ToString(),
                            patient.PatientName,
                            patient.IcCardNo,
                            patient.CardNo,
                            patient.ContactNum,
                            patient.HomePhoneNum,
                            patient.Nationality,
                            patient.Nation,
                            patient.IdCardNo,
                            patient.Gender.ToString(),
                            patient.BloodType,
                            patient.Home,
                            patient.Address,
                            patient.IsMarried,
                            patient.Comment,
                            patient.Birthday.ToString()
                        };
            return str;
        }
        #endregion

        /// <summary>
        /// 从标本库中提取的病人信息
        /// </summary>
        /// <returns></returns>
        private SpecPatient SetPatient()
        {
            SpecPatient specPatient = new SpecPatient();
            specPatient.PatientID = Convert.ToInt32(this.Reader["PATIENTID"].ToString());
            specPatient.PatientName = this.Reader["PNAME"].ToString();
            specPatient.IcCardNo = this.Reader["IC_CARDNO"].ToString();
            specPatient.CardNo = this.Reader["CARD_NO"].ToString();
            specPatient.ContactNum = this.Reader["CONTACTNUM"].ToString();
            specPatient.HomePhoneNum = this.Reader["HOMEPHONENUM"].ToString();
            specPatient.Nationality = this.Reader["NATIONALITY"].ToString();
            specPatient.Nation = this.Reader["NATION"].ToString();
            specPatient.IdCardNo = this.Reader["IDCARDNO"].ToString();
            specPatient.Gender = Convert.ToChar(this.Reader["GENDER"].ToString());
            specPatient.BloodType = this.Reader["BLOOD_CODE"].ToString();
            specPatient.Home = this.Reader["HOME"].ToString();
            specPatient.Address = this.Reader["ADDRESS"].ToString();
            specPatient.IsMarried = this.Reader["ISMARR"].ToString();
            specPatient.Comment = this.Reader["MARK"].ToString();
            specPatient.Birthday = Convert.ToDateTime(Reader["BIRTHDAY"].ToString());
            return specPatient;
        }


        #region 获取参数
        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        public int GetNextSequence(ref string sequence)
        {
            //
            // 执行SQL语句
            //
            sequence = this.GetSequence("Speciment.BizLogic.PatientManage.GetNextSequence");
            //
            // 如果返回NULL，则获取失败
            //
            if (sequence == null)
            {
                this.SetError("", "获取Sequence失败");
                return -1;
            }
            //
            // 成功返回
            //
            return 1;
        }
        #endregion

        #region 更新病人
        /// <summary>
        /// 更新病人
        /// </summary>
        /// <param name="sqlIndex">sql索引</param>
        /// <param name="args"></param>
        /// <returns></returns>
        private int UpdatePatient(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;//Update语句

            //获得Where语句
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return -1;
            }

            return this.ExecNoQuery(sql, args);
        }
        #endregion

        /// <summary>
        /// 根据索引获取病人列表
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList GetPatientInfo(string sqlIndex, string[] parm,string patientFrom)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            SpecPatient patient;
            ArrayList arrList = new ArrayList();
            while (this.Reader.Read())
            {
                patient = new SpecPatient();
                if (patientFrom == "H")
                {
                    patient = SetSpecPatient();
                }
                if (patientFrom == "S")
                {
                    patient = SetPatient();
                }
                arrList.Add(patient);
            }
            this.Reader.Close();
            return arrList;
        }

        #region  根据住院流水号获取住院科室
        /// <summary>
        /// 根据住院流水号获取住院科室
        /// </summary>
        /// <param name="inPatientNo"></param>
        /// <returns>住院科室</returns>
        public string GetIndeptName(string inPatientNo)
        {
            string deptName="";
            string[] parm = new string[] { inPatientNo};
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.PatientManage.GetDeptByPatientNo", ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;           
            while (this.Reader.Read())
            {
                if (!Reader.IsDBNull(0))
                    deptName =  Reader[0].ToString();
                break;
            }
            this.Reader.Close();
            return deptName;
        }
        #endregion

        #region 设置错误信息
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="errorCode">错误代码发生行数</param>
        /// <param name="errorText">错误信息</param>
        private void SetError(string errorCode, string errorText)
        {
            this.ErrCode = errorCode;
            this.Err = errorText + "[" + this.Err + "]"; // + "在ShelfSpecManage.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion

        #region 标本库病人信息插入
       /// <summary>
        /// 插入病人信息
        /// </summary>
        /// <param name="specPatient">标本库病人信息</param>
        /// <returns></returns>
        public int InsertPatient(SpecPatient specPatient)
        {
            try
            {
                return UpdatePatient("Speciment.BizLogic.PatientManage.Insert", GetParam(specPatient));
            }
            catch(Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }
       #endregion

        /// <summary>
        /// 更新病人信息
        /// </summary>
        /// <param name="specPatient"></param>
        /// <returns></returns>
        public int UpdatePatient(SpecPatient specPatient)
        {
            try
            {
                return UpdatePatient("Speciment.BizLogic.PatientManage.Update", GetParam(specPatient));
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 根据sql查询病人信息
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetPatientInfo(string sql)
        {
            if(this.ExecQuery(sql)==-1)
            {
                return null;
            }
            ArrayList arrSpecPatient = new ArrayList();
            while (this.Reader.Read())
            {
                SpecPatient patient = SetPatient();
                arrSpecPatient.Add(patient);
            }
            Reader.Close();
            return arrSpecPatient;
        }

        /// <summary>
        /// 根据CardNo获取病人信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public SpecPatient GetPatientInfoCardNo(string cardNo)
        {
            ArrayList arr = new ArrayList();
            arr = this.GetPatientInfo("Speciment.BizLogic.PatientManage.SelectPatientByCardNo", new string[] { cardNo }, "H");
            if (arr.Count > 0)
            {
                return arr[0] as SpecPatient;
            }
            return null;
        }

        /// <summary>
        /// 根据IcCardNO获取病人信息
        /// </summary>
        /// <param name="icCardNo"></param>
        /// <returns></returns>
        public SpecPatient GetPatientInfoIcCardNo(string icCardNo)
        {
            ArrayList arr = new ArrayList();
            arr = this.GetPatientInfo("Speciment.BizLogic.PatientManage.SelectPatientByICCardNo", new string[] { icCardNo }, "H");
            if (arr.Count > 0)
            {
                return arr[0] as SpecPatient;
            }
            return null;
        }

        /// <summary>
        /// 根据住院流水号获取病人信息
        /// </summary>
        /// <param name="inHosNum"></param>
        /// <returns></returns>
        public SpecPatient GetPatientInfoInNum(string inHosNum)
        {
            ArrayList arr = new ArrayList();
            arr = this.GetPatientInfo("Speciment.BizLogic.PatientManage.SelectPatientByInpatientNo", new string[] { inHosNum }, "H");
            if (arr.Count > 0)
            {
                return arr[0] as SpecPatient;
            }
            return null;
        }

        /// <summary>
        /// 根据病历号从索引表中获取病人信息
        /// </summary>
        /// <param name="patNo"></param> 
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetPatientForOldData(string patNo, string name ,string sql , ref string val)
        {  
            if (this.ExecQuery(sql)== -1)
                return null;
            Neusoft.FrameWork.Models.NeuObject neuo;
            ArrayList arrList = new ArrayList();
            while (this.Reader.Read())
            {
                neuo = new Neusoft.FrameWork.Models.NeuObject();
                neuo.ID = Reader["CARD_NO"] == null ? "" : Reader["CARD_NO"].ToString();
                neuo.Name = Reader["PNAME"] == null ? "" : Reader["PNAME"].ToString();
                neuo.Memo = this.Reader["SEX_CODE"].ToString() == "M" ? "男" : "女";
                if (neuo.ID == patNo && neuo.Name == name)
                {
                    val = "3";
                    ArrayList tmp = new ArrayList();
                    tmp.Add(neuo);
                    return tmp;
                }
                arrList.Add(neuo);
            }
            this.Reader.Close();
            return arrList;
        }
    }
}
