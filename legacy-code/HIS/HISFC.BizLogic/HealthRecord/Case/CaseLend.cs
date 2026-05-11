using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.HealthRecord;
using Neusoft.FrameWork.Function;
using System.Data;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.HealthRecord.Case
{
    /// <summary>
    /// Visit<br></br>
    /// [功能描述: 病历查询借阅]<br></br>
    /// [创 建 者: 蒋飞]<br></br>
    /// [创建时间: 2007-08-27]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
        public class CaseLend : Neusoft.FrameWork.Management.Database
        {


        #region 查询

        /// <summary>
        ///　根据病历编码查询病历的信息
        /// </summary>
        /// <param name="">病历流水号</param>
        /// <returns>信息数组元素型: Neusoft.HISFC.Models.HealthRecord.Case.CaseLend</returns>

        
        public ArrayList Query(string billID)
        {
            ArrayList List = null;
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CaseLend.Select", ref strSql) == -1) return null;
            try
            {
                //查询
                strSql = string.Format(strSql, billID);
                this.ExecQuery(strSql);
                Neusoft.HISFC.Models.HealthRecord.Case.CaseLend caseLend = null;
                List = new ArrayList();
                while (this.Reader.Read())
                {
                    caseLend = new Neusoft.HISFC.Models.HealthRecord.Case.CaseLend();
                    caseLend.ID = this.Reader[0].ToString();         //病历号 
                    caseLend.LendEmpl.ID= this.Reader[2].ToString(); //借阅员工号
                    caseLend.StartingTime =NConvert.ToDateTime(this.Reader[3].ToString()); //开始借阅时间
                    caseLend.EndTime = NConvert.ToDateTime(this.Reader[4].ToString());           //归还时间 
                    caseLend.AuditingOper.ID = this.Reader[6].ToString(); //审核员工号
                    caseLend.AuditingOper.OperTime = NConvert.ToDateTime(this.Reader[7].ToString()); //出库审核时间
                    caseLend.IsAuditing = NConvert.ToBoolean(this.Reader[8].ToString()); //是否出库审核 
                    caseLend.IsReturn = NConvert.ToBoolean(this.Reader[9].ToString()); //是否已经归还
                    caseLend.ReturnOper.ID = this.Reader[10].ToString(); //归还员工号
                    caseLend.ReturnOper.OperTime = NConvert.ToDateTime(this.Reader[11].ToString()); //实际归还时间
                    caseLend.ReturnConfirmOper.ID = this.Reader[12].ToString();           //归还确认人工号 
                    caseLend.ReturnConfirmOper.OperTime = NConvert.ToDateTime(this.Reader[13].ToString()); //归还确认时间
                    if (this.Reader[14].ToString().Equals("0"))        //业务类型
                    {
                        caseLend.LendType = Neusoft.HISFC.Models.HealthRecord.Case.EnumLendType.Lend;
                    }
                    else
                    {
                        caseLend.LendType = Neusoft.HISFC.Models.HealthRecord.Case.EnumLendType.Refer;
                    }                  
                               
                    caseLend = null;
                }

                return List;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 患者基本信息查询  com_patientinfo
        /// </summary>
        /// <param name="caseNo">病案号</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.PatientInfo QueryComPatientInfo(string caseNo)
        {
            Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
            string sql = string.Empty;
            if (Sql.GetSql("RADT.Inpatient.PatientInfoQuery", ref sql) == -1)
            #region SQL
            /*SELECT card_no,
						   name,   --姓名
								   spell_code,   --拼音码

								   wb_code,   --五笔
								   birthday,   --出生日期
								   sex_code,   --性别
								   idenno,   --身份证号
								   blood_code,   --血型

								   prof_code,   --职业
								   work_home,   --工作单位
								   work_tel,   --单位电话
								   work_zip,   --单位邮编
								   home,   --户口或家庭所在

								   home_tel,   --家庭电话
								   home_zip,   --户口或家庭邮政编码

								   district,   --籍贯
								   nation_code,   --民族
								   linkman_name,   --联系人姓名

								   linkman_tel,   --联系人电话

								   linkman_add,   --联系人住址
								   rela_code,   --联系人关系

								   mari,   --婚姻状况
								   coun_code,   --国籍
								   paykind_code,   --结算类别
								   paykind_name,   --结算类别名称
								   pact_code,   --合同代码
								   pact_name,   --合同单位名称
								   mcard_no,   --医疗证号
								   area_code,   --出生地

								   framt,   --医疗费用
								   ic_cardno,   --电脑号

								   anaphy_flag,   --药物过敏
								   hepatitis_flag,   --重要疾病
								   act_code,   --帐户密码
								   act_amt,   --帐户总额
								   lact_sum,   --上期帐户余额
								   lbank_sum,   --上期银行余额
								   arrear_times,   --欠费次数
								   arrear_sum,   --欠费金额
								   inhos_source,   --住院来源
								   lihos_date,   --最近住院日期

								   inhos_times,   --住院次数
								   louthos_date,   --最近出院日期

								   fir_see_date,   --初诊日期
								   lreg_date,   --最近挂号日期

								   disoby_cnt,   --违约次数
								   end_date,   --结束日期
								   mark,   --备注
								   oper_code,   --操作员

								   oper_date    --操作日期
							  FROM com_patientinfo   --病人基本信息表
							 WHERE PARENT_CODE='[父级编码]'  and 
								   CURRENT_CODE='[本级编码]' and 
								   card_no='{0}'
								   */
            #endregion
            {
                return null;
            }
            string sqlWhere = @"
                where case_no='{0}'";
            sql = sql + sqlWhere;
            sql = string.Format(sql, caseNo);

            if (ExecQuery(sql) < 0)
            {
                return null;
            }

            if (Reader.Read())
            {
                try
                {
                    if (!Reader.IsDBNull(0)) PatientInfo.PID.CardNO = Reader[0].ToString(); //就诊卡号
                    if (!Reader.IsDBNull(1)) PatientInfo.Name = Reader[1].ToString(); //姓名
                    if (!Reader.IsDBNull(2)) PatientInfo.SpellCode = Reader[2].ToString(); //拼音码
                    if (!Reader.IsDBNull(3)) PatientInfo.WBCode = Reader[3].ToString(); //五笔
                    if (!Reader.IsDBNull(4)) PatientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[4].ToString()); //出生日期
                    if (!Reader.IsDBNull(5)) PatientInfo.Sex.ID = Reader[5].ToString(); //性别
                    if (!Reader.IsDBNull(6)) PatientInfo.IDCard = Reader[6].ToString(); //身份证号
                    if (!Reader.IsDBNull(7)) PatientInfo.BloodType.ID = Reader[7].ToString(); //血型
                    if (!Reader.IsDBNull(8)) PatientInfo.Profession.ID = Reader[8].ToString(); //职业
                    if (!Reader.IsDBNull(9)) PatientInfo.CompanyName = Reader[9].ToString(); //工作单位
                    if (!Reader.IsDBNull(10)) PatientInfo.PhoneBusiness = Reader[10].ToString(); //单位电话
                    if (!Reader.IsDBNull(11)) PatientInfo.BusinessZip = Reader[11].ToString(); //单位邮编
                    if (!Reader.IsDBNull(12)) PatientInfo.AddressHome = Reader[12].ToString(); //户口或家庭所在
                    if (!Reader.IsDBNull(13)) PatientInfo.PhoneHome = Reader[13].ToString(); //家庭电话
                    if (!Reader.IsDBNull(14)) PatientInfo.HomeZip = Reader[14].ToString(); //户口或家庭邮政编码
                    if (!Reader.IsDBNull(15)) PatientInfo.DIST = Reader[15].ToString(); //籍贯
                    if (!Reader.IsDBNull(16)) PatientInfo.Nationality.ID = Reader[16].ToString(); //民族
                    if (!Reader.IsDBNull(17)) PatientInfo.Kin.Name = Reader[17].ToString(); //联系人姓名
                    if (!Reader.IsDBNull(18)) PatientInfo.Kin.RelationPhone = Reader[18].ToString(); //联系人电话
                    if (!Reader.IsDBNull(19)) PatientInfo.Kin.RelationAddress = Reader[19].ToString(); //联系人住址
                    if (!Reader.IsDBNull(20)) PatientInfo.Kin.Relation.ID = Reader[20].ToString(); //联系人关系
                    if (!Reader.IsDBNull(21)) PatientInfo.MaritalStatus.ID = Reader[21].ToString(); //婚姻状况
                    if (!Reader.IsDBNull(22)) PatientInfo.Country.ID = Reader[22].ToString(); //国籍
                    if (!Reader.IsDBNull(23)) PatientInfo.Pact.PayKind.ID = Reader[23].ToString(); //结算类别
                    if (!Reader.IsDBNull(24)) PatientInfo.Pact.PayKind.Name = Reader[24].ToString(); //结算类别名称
                    if (!Reader.IsDBNull(25)) PatientInfo.Pact.ID = Reader[25].ToString(); //合同代码
                    if (!Reader.IsDBNull(26)) PatientInfo.Pact.Name = Reader[26].ToString(); //合同单位名称
                    if (!Reader.IsDBNull(27)) PatientInfo.SSN = Reader[27].ToString(); //医疗证号
                    if (!Reader.IsDBNull(28)) PatientInfo.AreaCode = Reader[28].ToString(); //地区
                    if (!Reader.IsDBNull(29)) PatientInfo.FT.TotCost = NConvert.ToDecimal(Reader[29].ToString()); //医疗费用
                    if (!Reader.IsDBNull(30)) PatientInfo.Card.ICCard.ID = Reader[30].ToString(); //电脑号
                    if (!Reader.IsDBNull(31)) PatientInfo.Disease.IsAlleray = NConvert.ToBoolean(Reader[31].ToString()); //药物过敏
                    if (!Reader.IsDBNull(32)) PatientInfo.Disease.IsMainDisease = NConvert.ToBoolean(Reader[32].ToString()); //重要疾病
                    if (!Reader.IsDBNull(33)) PatientInfo.Card.NewPassword = Reader[33].ToString(); //帐户密码
                    if (!Reader.IsDBNull(34)) PatientInfo.Card.NewAmount = NConvert.ToDecimal(Reader[34].ToString()); //帐户总额
                    if (!Reader.IsDBNull(35)) PatientInfo.Card.OldAmount = NConvert.ToDecimal(Reader[35].ToString()); //上期帐户余额
                    //					if (!this.Reader.IsDBNull(36)) PatientInfo=this.Reader[36].ToString();//上期银行余额
                    //					if (!this.Reader.IsDBNull(37)) PatientInfo=this.Reader[37].ToString();//欠费次数
                    //					if (!this.Reader.IsDBNull(38)) PatientInfo=this.Reader[38].ToString();//欠费金额
                    //					if (!this.Reader.IsDBNull(39)) PatientInfo=this.Reader[39].ToString();//住院来源
                    //					if (!this.Reader.IsDBNull(40)) PatientInfo=this.Reader[40].ToString();//最近住院日期
                    //					if (!this.Reader.IsDBNull(41)) PatientInfo=this.Reader[41].ToString();//住院次数
                    //					if (!this.Reader.IsDBNull(42)) PatientInfo=this.Reader[42].ToString();//最近出院日期
                    //					if (!this.Reader.IsDBNull(43)) PatientInfo=this.Reader[43].ToString();//初诊日期
                    //					if (!this.Reader.IsDBNull(44)) PatientInfo=this.Reader[44].ToString();//最近挂号日期
                    //					if (!this.Reader.IsDBNull(45)) PatientInfo=this.Reader[45].ToString();//违约次数
                    //					if (!this.Reader.IsDBNull(46)) PatientInfo=this.Reader[46].ToString();//结束日期
                    if (!Reader.IsDBNull(47)) PatientInfo.Memo = Reader[47].ToString(); //备注
                    if (!Reader.IsDBNull(48)) PatientInfo.User01 = Reader[48].ToString(); //操作员
                    if (!Reader.IsDBNull(49)) PatientInfo.User02 = Reader[49].ToString(); //操作日期
                    if (!Reader.IsDBNull(50)) PatientInfo.IsEncrypt = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[50].ToString());
                    if (!Reader.IsDBNull(51)) PatientInfo.NormalName = Reader[51].ToString();
                    //{DA67A335-E85E-46e1-A672-4DB409BCC11B}
                    if (!Reader.IsDBNull(52)) PatientInfo.IDCardType.ID = Reader[52].ToString();//证件类型
                    if (!Reader.IsDBNull(53)) PatientInfo.VipFlag = NConvert.ToBoolean(Reader[53]);//vip标识
                    if (!Reader.IsDBNull(54)) PatientInfo.MatherName = Reader[54].ToString();//母亲姓名
                    if (!Reader.IsDBNull(55)) PatientInfo.IsTreatment = NConvert.ToBoolean(Reader[55]);//是否急诊
                    if (!Reader.IsDBNull(56)) PatientInfo.PID.CaseNO = Reader[56].ToString();//病案号
                    if (PatientInfo.IsEncrypt && PatientInfo.NormalName != string.Empty)
                    {
                        PatientInfo.DecryptName = Neusoft.FrameWork.WinForms.Classes.Function.Decrypt3DES(PatientInfo.NormalName);
                    }
                    if (!Reader.IsDBNull(57)) PatientInfo.Insurance.ID = Reader[57].ToString(); //保险公司编码
                    if (!Reader.IsDBNull(58)) PatientInfo.Insurance.Name = Reader[58].ToString(); //保险公司名称
                    if (!Reader.IsDBNull(59)) PatientInfo.Kin.RelationDoorNo = Reader[59].ToString(); //联系人地址门牌号
                    if (!Reader.IsDBNull(60)) PatientInfo.AddressHomeDoorNo = Reader[60].ToString(); //家庭住址门牌号
                    if (!Reader.IsDBNull(61)) PatientInfo.Email = Reader[61].ToString(); //email
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
            }

            Reader.Close();

            return PatientInfo;
        }
   
    


        /// <summary>
        /// 患者基本信息查询  met_cas_base 
        /// </summary>
        /// <param name="caseNo">病案号</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Base QueryMetCasBase(string caseNo)
        {
            Neusoft.HISFC.Models.HealthRecord.Base Info = new Neusoft.HISFC.Models.HealthRecord.Base();
            string sql = string.Empty;
            if (Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.Select.HIS50", ref sql) == -1)
            {
                return null;
            }
            string sqlWhere = @"
                where case_no='{0}'";
            sql = sql + sqlWhere;
            sql = string.Format(sql, caseNo);

            if (ExecQuery(sql) < 0)
            {
                return null;
            }

            if (Reader.Read())
            {
                try
                {
                    if (!Reader.IsDBNull(0)) Info.PatientInfo.PID.CardNO = Reader[0].ToString(); //就诊卡号
                    if (!Reader.IsDBNull(1)) Info.PatientInfo.Name = Reader[1].ToString(); //姓名
                    if (!Reader.IsDBNull(2)) Info.PatientInfo.SpellCode = Reader[2].ToString(); //拼音码
                    if (!Reader.IsDBNull(3)) Info.PatientInfo.WBCode = Reader[3].ToString(); //五笔
                    if (!Reader.IsDBNull(4)) Info.PatientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[4].ToString()); //出生日期
                    if (!Reader.IsDBNull(5)) Info.PatientInfo.Sex.ID = Reader[5].ToString(); //性别
                    if (!Reader.IsDBNull(6)) Info.PatientInfo.IDCard = Reader[6].ToString(); //身份证号
                    if (!Reader.IsDBNull(7)) Info.PatientInfo.BloodType.ID = Reader[7].ToString(); //血型
                    if (!Reader.IsDBNull(8)) Info.PatientInfo.Profession.ID = Reader[8].ToString(); //职业
                    if (!Reader.IsDBNull(9)) Info.PatientInfo.CompanyName = Reader[9].ToString(); //工作单位
                    if (!Reader.IsDBNull(10)) Info.PatientInfo.PhoneBusiness = Reader[10].ToString(); //单位电话
                    if (!Reader.IsDBNull(11)) Info.PatientInfo.BusinessZip = Reader[11].ToString(); //单位邮编
                    if (!Reader.IsDBNull(12)) Info.PatientInfo.AddressHome = Reader[12].ToString(); //户口或家庭所在
                    if (!Reader.IsDBNull(13)) Info.PatientInfo.PhoneHome = Reader[13].ToString(); //家庭电话
                    if (!Reader.IsDBNull(14)) Info.PatientInfo.HomeZip = Reader[14].ToString(); //户口或家庭邮政编码
                    if (!Reader.IsDBNull(15)) Info.PatientInfo.DIST = Reader[15].ToString(); //籍贯
                    if (!Reader.IsDBNull(16)) Info.PatientInfo.Nationality.ID = Reader[16].ToString(); //民族
                    if (!Reader.IsDBNull(17)) Info.PatientInfo.Kin.Name = Reader[17].ToString(); //联系人姓名
                    if (!Reader.IsDBNull(18)) Info.PatientInfo.Kin.RelationPhone = Reader[18].ToString(); //联系人电话
                    if (!Reader.IsDBNull(19)) Info.PatientInfo.Kin.RelationAddress = Reader[19].ToString(); //联系人住址
                    if (!Reader.IsDBNull(20)) Info.PatientInfo.Kin.Relation.ID = Reader[20].ToString(); //联系人关系
                    if (!Reader.IsDBNull(21)) Info.PatientInfo.MaritalStatus.ID = Reader[21].ToString(); //婚姻状况
                    if (!Reader.IsDBNull(22)) Info.PatientInfo.Country.ID = Reader[22].ToString(); //国籍
                    if (!Reader.IsDBNull(23)) Info.PatientInfo.Pact.PayKind.ID = Reader[23].ToString(); //结算类别
                    if (!Reader.IsDBNull(24)) Info.PatientInfo.Pact.PayKind.Name = Reader[24].ToString(); //结算类别名称
                    if (!Reader.IsDBNull(25)) Info.PatientInfo.Pact.ID = Reader[25].ToString(); //合同代码
                    if (!Reader.IsDBNull(26)) Info.PatientInfo.Pact.Name = Reader[26].ToString(); //合同单位名称
                    if (!Reader.IsDBNull(27)) Info.PatientInfo.SSN = Reader[27].ToString(); //医疗证号
                    if (!Reader.IsDBNull(28)) Info.PatientInfo.AreaCode = Reader[28].ToString(); //地区
                    if (!Reader.IsDBNull(29)) Info.PatientInfo.FT.TotCost = NConvert.ToDecimal(Reader[29].ToString()); //医疗费用
                    if (!Reader.IsDBNull(30)) Info.PatientInfo.Card.ICCard.ID = Reader[30].ToString(); //电脑号
                    if (!Reader.IsDBNull(31)) Info.PatientInfo.Disease.IsAlleray = NConvert.ToBoolean(Reader[31].ToString()); //药物过敏
                    if (!Reader.IsDBNull(32)) Info.PatientInfo.Disease.IsMainDisease = NConvert.ToBoolean(Reader[32].ToString()); //重要疾病
                    if (!Reader.IsDBNull(33)) Info.PatientInfo.Card.NewPassword = Reader[33].ToString(); //帐户密码
                    if (!Reader.IsDBNull(34)) Info.PatientInfo.Card.NewAmount = NConvert.ToDecimal(Reader[34].ToString()); //帐户总额
                    if (!Reader.IsDBNull(35)) Info.PatientInfo.Card.OldAmount = NConvert.ToDecimal(Reader[35].ToString()); //上期帐户余额
                    //					if (!this.Reader.IsDBNull(36)) PatientInfo=this.Reader[36].ToString();//上期银行余额
                    //					if (!this.Reader.IsDBNull(37)) PatientInfo=this.Reader[37].ToString();//欠费次数
                    //					if (!this.Reader.IsDBNull(38)) PatientInfo=this.Reader[38].ToString();//欠费金额
                    //					if (!this.Reader.IsDBNull(39)) PatientInfo=this.Reader[39].ToString();//住院来源
                    //					if (!this.Reader.IsDBNull(40)) PatientInfo=this.Reader[40].ToString();//最近住院日期
                    //					if (!this.Reader.IsDBNull(41)) PatientInfo=this.Reader[41].ToString();//住院次数
                    //					if (!this.Reader.IsDBNull(42)) PatientInfo=this.Reader[42].ToString();//最近出院日期
                    //					if (!this.Reader.IsDBNull(43)) PatientInfo=this.Reader[43].ToString();//初诊日期
                    //					if (!this.Reader.IsDBNull(44)) PatientInfo=this.Reader[44].ToString();//最近挂号日期
                    //					if (!this.Reader.IsDBNull(45)) PatientInfo=this.Reader[45].ToString();//违约次数
                    //					if (!this.Reader.IsDBNull(46)) PatientInfo=this.Reader[46].ToString();//结束日期
                    if (!Reader.IsDBNull(47)) Info.PatientInfo.Memo = Reader[47].ToString(); //备注
                    if (!Reader.IsDBNull(48)) Info.PatientInfo.User01 = Reader[48].ToString(); //操作员
                    if (!Reader.IsDBNull(49)) Info.PatientInfo.User02 = Reader[49].ToString(); //操作日期
                    if (!Reader.IsDBNull(50)) Info.PatientInfo.IsEncrypt = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[50].ToString());
                    if (!Reader.IsDBNull(51)) Info.PatientInfo.NormalName = Reader[51].ToString();
                    //{DA67A335-E85E-46e1-A672-4DB409BCC11B}
                    if (!Reader.IsDBNull(52)) Info.PatientInfo.IDCardType.ID = Reader[52].ToString();//证件类型
                    if (!Reader.IsDBNull(53)) Info.PatientInfo.VipFlag = NConvert.ToBoolean(Reader[53]);//vip标识
                    if (!Reader.IsDBNull(54)) Info.PatientInfo.MatherName = Reader[54].ToString();//母亲姓名
                    if (!Reader.IsDBNull(55)) Info.PatientInfo.IsTreatment = NConvert.ToBoolean(Reader[55]);//是否急诊
                    if (!Reader.IsDBNull(56)) Info.PatientInfo.PID.CaseNO = Reader[56].ToString();//病案号
                    if (Info.PatientInfo.IsEncrypt && Info.PatientInfo.NormalName != string.Empty)
                    {
                        Info.PatientInfo.DecryptName = Neusoft.FrameWork.WinForms.Classes.Function.Decrypt3DES(Info.PatientInfo.NormalName);
                    }
                    if (!Reader.IsDBNull(57)) Info.PatientInfo.Insurance.ID = Reader[57].ToString(); //保险公司编码
                    if (!Reader.IsDBNull(58)) Info.PatientInfo.Insurance.Name = Reader[58].ToString(); //保险公司名称
                    if (!Reader.IsDBNull(59)) Info.PatientInfo.Kin.RelationDoorNo = Reader[59].ToString(); //联系人地址门牌号
                    if (!Reader.IsDBNull(60)) Info.PatientInfo.AddressHomeDoorNo = Reader[60].ToString(); //家庭住址门牌号
                    if (!Reader.IsDBNull(61)) Info.PatientInfo.Email = Reader[61].ToString(); //email
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
            }

            Reader.Close();

            return Info;
        }
        #endregion
    
    }
}
