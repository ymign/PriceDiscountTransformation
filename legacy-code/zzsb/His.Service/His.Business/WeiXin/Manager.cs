using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Shadow.Util.Data.Func;

namespace His.Business.WeiXin
{
    internal class Manager : Shadow.Util.Data.Management.OracleBase
    {

        public static string OPERID = "00A102";

        #region 看诊序号

        /// <summary>
        /// 查找当前看诊序号
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="seeNo"></param>
        /// <returns></returns>
        public int GetCurrentSeeNo(string schemaNo, ref int seeNo)
        {
            string str = "Registration.Register.SeeNo.Current";
            string sql = string.Empty, No = string.Empty;
            if (this.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                seeNo = Shadow.Util.Data.Func.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找当前看诊序号出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 查找最小看诊序号
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="minNo"></param>
        /// <returns></returns>
        public int GetMinSeeNo(string schemaNo, ref int minNo)
        {
            //Registration.Register.SeeNo.Begin.1
            string str = "Registration.Register.SeeNo.Begin.1";
            string sql = string.Empty, No = string.Empty;
            if (this.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                minNo = Shadow.Util.Data.Func.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找最小看诊序号出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }


        /// <summary>
        /// 取排班的号源总额数
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public int GetSourceCount(string schemaNo, ref int cnt)
        {
            string sql = @"select nvl(sum(a.tel_lmt+a.reg_lmt+a.spe_lmt ),-1) cnt
                        from fin_opr_schema a
                        where a.id='{0}' ";
            string No = string.Empty;

            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                cnt = Shadow.Util.Data.Func.NConvert.ToInt32(No);
                if (cnt == -1)
                {
                    this.Err = "没有找到相关行数！";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = "查找号源数量出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }
        #endregion
        /// <summary>
        /// 查找有效挂号数
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="Residue"></param>
        /// <returns></returns>
        public int GetResidue(string schemaNo, ref int Residue)
        {
            //Registration.Register.Residue
            string str = "Registration.Register.Residue";
            string sql = string.Empty, No = string.Empty;
            if (this.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                Residue = Neusoft.FrameWork.Function.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找有效挂号数出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 查询 返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            DataSet ds = new DataSet();
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds.Tables[0];

        }


        /// <summary>
        /// 执行单条sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, ref string errMsg)
        {
            int i = this.ExecNoQuery(sql);
            errMsg = Err;
            return i;
        }


        public Neusoft.FrameWork.Models.NeuObject GetConstant(string type, string ID)
        {
            string sql = @"select * from com_dictionary a
                      where a.type='{0}'
                      and a.code='{1}'";
            sql = string.Format(sql, type, ID);
            Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();

            try
            {
                if (this.ExecQuery(sql) == -1)
                    return null;
                while (this.Reader.Read())
                {
                    obj.ID = Reader[1].ToString();
                    obj.Name = Reader[2].ToString();
                    obj.Memo = Reader[3].ToString();
                    obj.User01 = Reader[0].ToString();
                    obj.User02 = Reader[6].ToString();
                    obj.User03 = Reader[8].ToString();

                }
                this.Reader.Close();
                return obj;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return null;
            }


        }

        /// <summary>
        /// 生成预约流水号
        /// </summary>
        /// <returns></returns>
        public int GetBookSerialNo()
        {
            string sql = "select SEQ_FIN_BOOKING.Nextval from dual ";
            return Shadow.Util.Data.Func.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }


        public bool VaildRegSource(string SourceID)
        {
            int regRemainCount = 0;
            string sql = @"select (t.reg_lmt - t.reged) regRemain
                                                          from fin_opr_schema t
                                                         where t.id = '{0}'";
            sql = string.Format(sql, SourceID);
            //排班表
            regRemainCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
            if (regRemainCount <= 0)
            {
                this.Err = "没有足够号源，请选择其他排班！";
                return false;
            }
            return true;
            //mgr.ExecQuery("select '" + SourceID + "-" + regRemainCount.ToString() + "' from dual ");
        }


        /// <summary>
        /// 获取合同单位
        /// </summary>
        /// <param name="opr"></param>
        /// <param name="patient"></param>
        /// <returns></returns>
        public int GetPactInfo(His.Models.ZZSB.OutPatientReg opr, His.Models.ZZSB.ComPatient patient)
        {
            string pactSql = Sql.Sql.GetPactInfo;

            #region 获取合同单位
            if (!string.IsNullOrEmpty(opr.Payinsufeestr))
            {
                List<string> infos = opr.Payinsufeestr.Split('^').ToList();
                if (infos.Count >= 2)
                {
                    if (!string.IsNullOrEmpty(infos[1]))
                    {
                        opr.FeeType = "107";
                    }
                    if (infos.Count >= 10)
                    {
                        if (!string.IsNullOrEmpty(infos[8]) && string.IsNullOrEmpty(patient.IDCard))
                        {
                            patient.IDCard = infos[8];
                        }
                        if (!string.IsNullOrEmpty(infos[9]) && string.IsNullOrEmpty(patient.McardNo))
                        {
                            patient.McardNo = infos[9];
                        }
                    }
                }
            }
            His.Models.ZZSB.PactInfo pactUnit = null;
            pactSql = string.Format(pactSql, opr.FeeType);
            if (this.ExecQuery(pactSql) != -1)
            {
                while (this.Reader.Read())
                {
                    #region 赋值

                    pactUnit = new His.Models.ZZSB.PactInfo();

                    pactUnit.ID = this.Reader[0].ToString();//合同代码          
                    pactUnit.Name = this.Reader[1].ToString();//合同单位名称                    
                    pactUnit.PayKind.ID = this.Reader[2].ToString();//结算类别                    
                    pactUnit.Rate.PubRate = NConvert.ToDecimal(this.Reader[3].ToString().Trim());//公费比例                    
                    pactUnit.Rate.PayRate = NConvert.ToDecimal(this.Reader[4].ToString().Trim());//自付比例                   
                    pactUnit.Rate.OwnRate = NConvert.ToDecimal(this.Reader[5].ToString().Trim()); //自费比例                   
                    pactUnit.Rate.RebateRate = NConvert.ToDecimal(this.Reader[6].ToString().Trim()); //优惠比例                    
                    pactUnit.Rate.ArrearageRate = NConvert.ToDecimal(this.Reader[7].ToString().Trim());//欠费比例                    
                    pactUnit.Rate.IsBabyShared = NConvert.ToBoolean(this.Reader[8].ToString());//婴儿标志 0 无关 1 有关                                
                    pactUnit.IsNeedMCard = NConvert.ToBoolean(this.Reader[9].ToString().Trim()); //是否要求必须有医疗证号 0 否 1 是                      
                    pactUnit.IsInControl = NConvert.ToBoolean(this.Reader[10].ToString().Trim());//是否受监控 1受监控0不受监控                   
                    pactUnit.ItemType = this.Reader[11].ToString().Trim(); //标志  0 全部 1 药品 2 非药品   
                    pactUnit.DayQuota = NConvert.ToDecimal(this.Reader[12].ToString().Trim());//日限额                     
                    pactUnit.MonthQuota = NConvert.ToDecimal(this.Reader[13].ToString().Trim()); //月限额                    
                    pactUnit.YearQuota = NConvert.ToDecimal(this.Reader[14].ToString().Trim());//年限额
                    pactUnit.OnceQuota = NConvert.ToDecimal(this.Reader[15].ToString().Trim());//一次限
                    string PriceForm = this.Reader[16].ToString();
                    if (PriceForm == "0")
                    {
                        pactUnit.PriceForm = "默认价";
                    }
                    else if (PriceForm == "1")
                    {
                        pactUnit.PriceForm = "特诊价";
                    }
                    else if (PriceForm == "2")
                    {
                        pactUnit.PriceForm = "儿童价";
                    }
                    //{B9303CFE-755D-4585-B5EE-8C1901F79450}maokb增加购入价
                    else if (PriceForm == "3")
                    {
                        pactUnit.PriceForm = "购入价";
                    }
                    else
                    {
                        pactUnit.PriceForm = "默认价";
                    }

                    pactUnit.BedQuota = NConvert.ToDecimal(this.Reader[17].ToString());//床位限额
                    pactUnit.AirConditionQuota = NConvert.ToDecimal(this.Reader[18].ToString());//空调限额
                    pactUnit.SortID = NConvert.ToInt32(this.Reader[19]);//序号             
                    pactUnit.ShortName = this.Reader[20].ToString();//合同单位简称
                    pactUnit.PactDllName = this.Reader[21].ToString(); //待遇dll名称
                    pactUnit.PactDllDescription = this.Reader[22].ToString();//待遇dll说明
                    pactUnit.PactSystemType = this.Reader[23].ToString().Trim();

                    switch (pactUnit.PactSystemType)
                    {
                        case "1":
                            pactUnit.PactSystemType = "门诊";
                            break;
                        case "2":
                            pactUnit.PactSystemType = "住院";
                            break;
                        case "3":
                            pactUnit.PactSystemType = "系统";
                            break;
                        default:
                            pactUnit.PactSystemType = "全院";
                            break;
                    }
                    pactUnit.SpellCode = this.Reader[24].ToString();//拼音码
                    pactUnit.WBCode = this.Reader[25].ToString();//五笔码
                    pactUnit.PatientType.ID = this.Reader[26].ToString();//人员类型编码
                    pactUnit.PatientType.Name = this.Reader[27].ToString();//人员类型名称
                    pactUnit.IsUseInOutPatientFee = NConvert.ToBoolean(this.Reader[28].ToString().Trim());
                    #endregion
                }
                this.Reader.Close();
            }
            if (pactUnit == null || string.IsNullOrEmpty(pactUnit.ID))
            {
                this.Err = "没有找到相应的合同单位";
                return -1;
            }

            patient.Pact = pactUnit;
            return 1;
            #endregion
        }

        /// <summary>
        /// 获取挂号等级费用
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public  int GetRegLevelFee(His.Models.ZZSB.ComPatient patient)
        {
            string regfeeSql = Sql.Sql.GetRegFee;
            regfeeSql = string.Format(regfeeSql, "1", patient.RegLevel.ID);

            if (this.ExecQuery(regfeeSql) == -1)
            {
                this.Err = "获取挂号等级费用出错！";
                return -1;
            }
            else
            {
                while (this.Reader.Read())
                {
                    patient.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[4]);//挂号费
                    patient.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[6]);//诊查费
                    break;
                }
                this.Reader.Close();
            }
            if (patient.RegFee+patient.OwnDigFee<=0)
            {
                this.Err = "没有找到相关等级费用！";
                return -1;
            }
            return 1;
        }

    }
}
