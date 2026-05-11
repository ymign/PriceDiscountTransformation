using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Models.ZZSB.MedicalModel;
using System.Data;

namespace His.Business.ZZSB.Medical
{
    public class MedicalDB : Neusoft.FrameWork.Management.Database
    {
        #region 属性
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
        #endregion

        #region 根据字典类别和code查询对应的字典数据
        /// <summary>
        /// 根据字典类别和code查询对应的字典数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<Neusoft.FrameWork.Models.NeuObject> GetComDictionaryForType(string type, string code)
        {
            List<Neusoft.FrameWork.Models.NeuObject> al = new List<Neusoft.FrameWork.Models.NeuObject>();
            string sql = @"select p.code,p.name from com_dictionary p where p.type='{0}' and p.valid_state=1";
            if (!string.IsNullOrEmpty(code))
            {
                sql = sql + string.Format(@" and p.code='{0}' ", code);
            }
            sql = string.Format(sql, type, code);
            if (this.ExecQuery(sql) == -1)
                return null;
            Neusoft.FrameWork.Models.NeuObject obj;
            while (this.Reader.Read())
            {
                obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();
                obj.Name = this.Reader[1].ToString();
                al.Add(obj);
            }
            this.Reader.Close();
            return al;
        }
        #endregion

       

        #region 获取门诊流水号
        /// <summary>
        /// 获取门诊流水号
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string GetClinicCode()
        {
            string sql = string.Format(@" select seq_fin_clinicno.nextval from dual ");
            string result = string.Empty;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }

        public bool CheckRegFeeDetail(string clinicno,string itemcode)
        {
            int result;
            string sql = @"
                            SELECT COUNT(1) FROM fin_opb_feedetail f WHERE f.clinic_code = '{0}' AND f.item_code = '{1}' AND f.cancel_flag = '1'
                   ";
            sql = string.Format(sql, clinicno, itemcode);

            result = Convert.ToInt32(this.ExecSqlReturnOne(sql));
            if (result > 0)
            {
                //证明有收费的数据了，不用再收
                return true;
            }
            else
            {
                return false;
            }



        }

        /// <summary>
        /// 获取处方号
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string GetOpbRecipeNoSequece()
        {
            string sql = string.Format(@" select SEQ_OPB_RECIPE_NO.NEXTVAL from dual ");
            string result = string.Empty;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }

        /// <summary>
        /// 获取门诊挂号部分信息
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public DataTable GetRegisterInfoByCardNo(string cardno)
        {
            string sql = string.Format(@" SELECT f.clinic_code,f.reg_date,f.dept_code,f.dept_name,f.doct_code,f.pact_code,f.reglevl_code
                           FROM fin_opr_register f
                          WHERE f.clinic_code =
                                (SELECT MAX(clinic_code)
                                   FROM fin_opr_register
                                  WHERE card_no = '{0}') AND f.valid_flag = '1'", cardno);
            string result = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                if (this.ExecQuery(sql, ref ds) < 0)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return null;
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取医嘱流水
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string GetMetMOOrderIDSequece()
        {
            string sql = string.Format(@" SELECT SEQ_MET_ORDER_ID.NEXTVAL FROM dual ");
            string result = string.Empty;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }
        #endregion

        public string GetSiPactCodeForClinCode(string clinCode)
        {
            string sql = string.Format(@" select p.pact_code from fin_ipr_siinmaininfo_gd p where p.inpatient_no='{0}' and p.type_code='0' ", clinCode);
            string result = string.Empty;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }

        public string GetSiPactNameForClinCode(string clinCode)
        {
            string sql = string.Format(@" select p.pact_name from fin_ipr_siinmaininfo_gd p where p.inpatient_no='{0}' and p.type_code='0' ", clinCode);
            string result = string.Empty;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }

        public string GetSiPayKindCodeForClinCode(string clinCode)
        {
            string sql = string.Format(@" select p.paykind_code from fin_ipr_siinmaininfo_gd p where p.inpatient_no='{0}' and p.type_code='0' ", clinCode);
            string result = string.Empty;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }

        #region 根据科室编码获取对应的科室名称
        /// <summary>
        /// 根据科室编码获取对应的科室名称
        /// </summary>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public string NewGetDeptNameForCode(string deptCode)
        {
            string sql = string.Format(@" select p.dept_name from com_department p where p.dept_code='{0}'
 ", deptCode);
            string result = string.Empty; ;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }
        #endregion

        #region 根据项目编码获取对应金额
        /// <summary>
        /// 根据项目编码获取对应金额
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public string GetPriceForItemCode(string itemCode)
        {
            string sql = string.Format(@" select  decode(unitflag, '1', fun_get_packageprice(item_code), unit_price) unit_price from fin_com_undruginfo  where item_code='{0}' ", itemCode);
            string result = string.Empty; ;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }
        #endregion

        #region 根据项目编码获取项目名称
        /// <summary>
        /// 根据项目编码获取项目名称
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public string GetItemNameForItemCode(string itemCode)
        {
            string sql = string.Format(@" select item_name from fin_com_undruginfo  where item_code='{0}' ", itemCode);
            string result = string.Empty; ;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }
        #endregion

        #region 根据工号查询人员所属科室
        /// <summary>
        /// 根据工号查询人员所属科室
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public string GetBelongDeptCodeForEmplCode(string emplCode)
        {
            string sql = string.Format(@" SELECT dept_code FROM com_employee WHERE empl_code = '{0}' ", emplCode);
            string result = string.Empty; ;
            try
            {
                result = this.ExecSqlReturnOne(sql);
                if (result == "-1" || string.IsNullOrEmpty(result))
                {
                    result = string.Empty;
                }
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
            return result;
        }
        #endregion


        #region 根据排班ID获取对应的排班数据
        /// <summary>
        /// 根据排班ID获取对应的排班数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public MedicalSchema GetSchemaForID(string ID)
        {
            MedicalSchema model = new MedicalSchema();
            #region sql脚本
            string sql = @"SELECT id, --序号                       
       dept_code, --科室代号
       dept_name, --科室名称
       doct_code, --医师代号
       doct_name, --医生姓名                                  
       reglevl_code, --挂号级别
       begin_time --开始时间
  FROM fin_opr_schema --医师出诊表
 WHERE id = '{0}'";
            #endregion
            sql = string.Format(sql, ID);
            try
            {
                this.ExecQuery(sql);
                while (Reader.Read())
                {
                    model.ID = Reader[0].ToString();
                    model.DeptCode = Reader[1].ToString();
                    model.DeptName = Reader[2].ToString();
                    model.DoctCode = Reader[3].ToString();
                    model.DoctName = Reader[4].ToString();
                    model.ReglevlCode = Reader[5].ToString();
                    model.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[6].ToString());
                }
                Reader.Close();
                return model;
            }
            catch (Exception e)
            {
                Reader.Close();
                this.ErrMsg = e.Message;
                return null;
                throw;
            }

            return model;
        }
        #endregion


        #region 根据业务类别获取最大结算序号
        public string GetMaxBalanceNo(string inpatientNo)
        {
            string strSql = @"select  nvl(max(to_number(p.balance_no)),0)+1
                                from fin_ipr_siinmaininfo_gd p
                               where p.inpatient_no = '{0}'";
            strSql = string.Format(strSql, inpatientNo);

            try
            {
                return this.ExecSqlReturnOne(strSql);
            }
            catch (Exception e)
            {
                this.ErrMsg = e.Message;
                return string.Empty;
            }
        }
        #endregion


        #region 获取诊疗项目的医保项目编码
        /// <summary>
        /// 获取诊疗项目的医保项目编码
        /// </summary>
        /// <param name="reglevl_code">医生级别代码</param>
        /// <returns></returns>
        public int getRegItemCode(string reglevl_code, ref string diagItemCode)
        {
            string sql = "";
            sql = @"select t.item_code from fin_com_regfeeset t where  t.valid_flag = '1'and t.reglevl_code = '{0}'";
            sql = string.Format(sql, reglevl_code);
            try
            {
                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }
                while (this.Reader.Read())
                {
                    diagItemCode = Reader[0].ToString();
                    break;
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                Err = ex.Message;
                return -1;
            }

            return 1;
        }
        #endregion

        public int NewUpdateSiMainInfoValidFlag(string inpatientNo, string RegNo, string balanceNo, string typeCode)
        {
            string strSql = @"update fin_ipr_siinmaininfo_gd f 
                            set f.valid_flag = '0',f.oper_code = '{2}',f.oper_date = sysdate,f.balance_state='0'
                            where f.inpatient_no = '{0}' and f.balance_no = '{1}' and f.reg_no='{3}' and f.type_code='{4}' ";

            try
            {
                strSql = string.Format(strSql, inpatientNo, balanceNo, this.Operator.ID, RegNo, typeCode);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            return this.ExecNoQuery(strSql);
        }


        public Neusoft.HISFC.Models.Registration.Register NewGetRegPersonInfo(string clinCode, string typeCode, Neusoft.HISFC.Models.Registration.Register obj)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("GDSI.NewSelectRegSIinmaininfoGd", ref strSql) == -1)
            {
                this.Err = "未查询到索引GDSI.NewSelectRegSIinmaininfoGd的sql脚本";
                return null;
            }

            try
            {
                strSql = string.Format(strSql, clinCode, typeCode);
                if (!string.IsNullOrEmpty(obj.SIMainInfo.InvoiceNo))
                {
                    strSql = strSql + string.Format(@" and invoice_no='{0}' ", obj.SIMainInfo.InvoiceNo);
                }


            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return null;
            }
            this.ExecQuery(strSql);
            try
            {
                while (Reader.Read())
                {
                    obj.ID = Reader[0].ToString();
                    obj.SIMainInfo.RegNo = Reader[1].ToString();
                    obj.SIMainInfo.BalNo = Reader[2].ToString();
                    //obj.SIMainInfo.InvoiceNo = Reader[3].ToString();
                    obj.SIMainInfo.MedicalType.ID = Reader[4].ToString();
                    obj.PID.PatientNO = Reader[5].ToString();
                    obj.PID.CardNO = Reader[6].ToString();
                    obj.SSN = Reader[7].ToString();
                    obj.Name = Reader[8].ToString();
                    obj.Sex.ID = Reader[9].ToString();
                    obj.IDCard = Reader[10].ToString();
                    //obj.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[11].ToString());
                    obj.SIMainInfo.EmplType = Reader[12].ToString();
                    obj.CompanyName = Reader[13].ToString();
                    obj.ClinicDiagnose = Reader[14].ToString();
                    obj.PVisit.PatientLocation.Dept.ID = Reader[15].ToString();
                    //obj.PVisit.PatientLocation.Dept.Name = Reader[26].ToString();
                    obj.Pact.PayKind.ID = Reader[17].ToString();
                    if (string.IsNullOrEmpty(obj.Pact.ID))
                    {//预防门诊结算的改了(然后还用的挂号保存的合同单位)
                        obj.Pact.ID = Reader[18].ToString();
                        obj.Pact.Name = Reader[19].ToString();
                    }

                    obj.PVisit.PatientLocation.Bed.ID = Reader[20].ToString();
                    obj.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[21].ToString());
                    obj.SIMainInfo.InDiagnoseDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[22].ToString());
                    obj.SIMainInfo.InDiagnose.ID = Reader[23].ToString();
                    obj.SIMainInfo.InDiagnose.Name = Reader[24].ToString();
                    //if (!Reader.IsDBNull(25))
                    //    obj.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[25].ToString());
                    obj.SIMainInfo.OutDiagnose.ID = Reader[26].ToString();
                    obj.SIMainInfo.OutDiagnose.Name = Reader[27].ToString();
                    if (!Reader.IsDBNull(28))
                        obj.SIMainInfo.BalanceDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[28].ToString());

                    obj.SIMainInfo.TotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[29].ToString());
                    obj.SIMainInfo.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[30].ToString());
                    obj.SIMainInfo.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[31].ToString());
                    obj.SIMainInfo.ItemPayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[32].ToString());
                    obj.SIMainInfo.BaseCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[33].ToString());
                    obj.SIMainInfo.PubOwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[34].ToString());
                    obj.SIMainInfo.ItemYLCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[35].ToString());
                    obj.SIMainInfo.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[36].ToString());
                    obj.SIMainInfo.OverTakeOwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[37].ToString());
                    obj.SIMainInfo.Memo = Reader[38].ToString();
                    obj.SIMainInfo.OperInfo.ID = Reader[39].ToString();
                    obj.SIMainInfo.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[40].ToString());
                    obj.SIMainInfo.FeeTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[41].ToString());
                    obj.SIMainInfo.HosCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[42].ToString());
                    obj.SIMainInfo.YearCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[43].ToString());
                    obj.SIMainInfo.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[44].ToString());
                    obj.SIMainInfo.IsBalanced = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[45].ToString());
                    obj.SIMainInfo.Memo = Reader[46].ToString();
                    obj.SIMainInfo.TypeCode = Reader[47].ToString();
                    obj.SIMainInfo.OverCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[48].ToString());
                    obj.SIMainInfo.PersonType.ID = Reader[49].ToString();
                    if (!Reader.IsDBNull(50))
                        obj.SIMainInfo.Bka911 = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[50].ToString());
                    obj.SIMainInfo.Bka912 = Reader[51].ToString();
                    obj.SIMainInfo.Bka913 = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[52].ToString());
                    obj.SIMainInfo.Bka914 = Reader[53].ToString();
                    if (!Reader.IsDBNull(54))
                        obj.SIMainInfo.Bka915 = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[54].ToString());
                    obj.SIMainInfo.Bka916 = Reader[55].ToString();
                    if (!Reader.IsDBNull(56))
                        obj.SIMainInfo.Bka917 = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[56].ToString());
                    obj.SIMainInfo.Bka042 = Reader[57].ToString();
                    obj.SIMainInfo.Aaz267 = Reader[58].ToString();
                    obj.SIMainInfo.Bka825 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[59].ToString());
                    obj.SIMainInfo.Bka826 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[60].ToString());
                    obj.SIMainInfo.Aka151 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[61].ToString());
                    obj.SIMainInfo.Bka838 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[62].ToString());
                    obj.SIMainInfo.Akb067 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[63].ToString());
                    obj.SIMainInfo.Akb066 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[64].ToString());
                    obj.SIMainInfo.Bka821 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[65].ToString());
                    obj.SIMainInfo.Bka839 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[66].ToString());
                    obj.SIMainInfo.Ake039 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[67].ToString());
                    obj.SIMainInfo.Ake035 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[68].ToString());
                    obj.SIMainInfo.Ake026 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[69].ToString());
                    obj.SIMainInfo.Ake029 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[70].ToString());
                    obj.SIMainInfo.Bka841 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[71].ToString());
                    obj.SIMainInfo.Bka842 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[72].ToString());
                    obj.SIMainInfo.Bka840 = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[73].ToString());
                    obj.SIMainInfo.Aaa027 = Reader[74].ToString();
                    obj.SIMainInfo.Bka438 = Reader[75].ToString();
                    obj.SIMainInfo.Aab301 = Reader[76].ToString();
                    obj.SIMainInfo.Aae140 = Reader[77].ToString();

                    obj.SIMainInfo.PsnNo = Reader[78].ToString();
                    obj.SIMainInfo.MdtrtID = Reader[79].ToString();
                    obj.SIMainInfo.ChargeBatchNumber = Reader[80].ToString();
                    obj.SIMainInfo.MdtrtCertType = Reader[81].ToString();
                    obj.SIMainInfo.MdtrtCertNo = Reader[82].ToString();
                    obj.SIMainInfo.Insutype = Reader[83].ToString();
                    obj.SIMainInfo.SetlId = Reader[84].ToString();
                    obj.SIMainInfo.PsnCertType = Reader[85].ToString();
                    obj.SIMainInfo.Certno = Reader[86].ToString();
                    obj.SIMainInfo.Gend = Reader[87].ToString();
                    obj.SIMainInfo.Naty = Reader[88].ToString();
                    obj.SIMainInfo.Age = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[89].ToString());
                    obj.SIMainInfo.PsnType = Reader[90].ToString();
                    obj.SIMainInfo.CvlservFlag = Reader[91].ToString();
                    obj.SIMainInfo.SetlTime = Reader[92].ToString();
                    obj.SIMainInfo.MedType = Reader[93].ToString();
                    obj.SIMainInfo.MedfeeSumamt = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[94].ToString());
                    obj.SIMainInfo.FulamtOwnpayAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[95].ToString());
                    obj.SIMainInfo.OverlmtSelfpay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[96].ToString());
                    obj.SIMainInfo.PreselfpayAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[97].ToString());
                    obj.SIMainInfo.InscpScpAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[98].ToString());
                    obj.SIMainInfo.ActPayDedc = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[99].ToString());
                    obj.SIMainInfo.HifpPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[100].ToString());
                    obj.SIMainInfo.PoolPropSelfpay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[101].ToString());
                    obj.SIMainInfo.CvlservPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[102].ToString());
                    obj.SIMainInfo.HifesPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[103].ToString());
                    obj.SIMainInfo.HifmiPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[104].ToString());
                    obj.SIMainInfo.HifobPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[105].ToString());
                    obj.SIMainInfo.MafPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[106].ToString());
                    obj.SIMainInfo.OthPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[107].ToString());
                    obj.SIMainInfo.FundPaySumamt = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[108].ToString());
                    obj.SIMainInfo.PsnPartAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[109].ToString());
                    obj.SIMainInfo.AcctPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[110].ToString());
                    obj.SIMainInfo.PsnCashPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[111].ToString());
                    obj.SIMainInfo.HospPartAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[112].ToString());
                    obj.SIMainInfo.Balc = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[113].ToString());
                    obj.SIMainInfo.AcctMulaidPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[114].ToString());
                    obj.SIMainInfo.MedinsSetlId = Reader[115].ToString();
                    obj.SIMainInfo.ClrOptins = Reader[116].ToString();
                    obj.SIMainInfo.ClrWay = Reader[117].ToString();
                    obj.SIMainInfo.ClrType = Reader[118].ToString();
                    obj.SIMainInfo.MdtrtareaAdmvs = Reader[119].ToString();
                    obj.SIMainInfo.InsuplcAdmdvs = Reader[120].ToString();
                    obj.SIMainInfo.OpterType = Reader[121].ToString();
                    obj.SIMainInfo.RecerAdmvs = Reader[122].ToString();
                }
                Reader.Close();
                return obj;
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                Reader.Close();
                return null;
            }
        }

        public int NewInsertOutPatientReg(Neusoft.HISFC.Models.Registration.Register r)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("GDSI.NeWInsertSiinmaininfoGd", ref strSql) == -1)
            {
                this.Err = "未查询到索引GDSI.NeWInsertSiinmaininfoGd的sql脚本";
                return -1;
            }
            try
            {
                strSql = string.Format(strSql,
                r.ID,
                r.SIMainInfo.RegNo,
                r.SIMainInfo.BalNo,
                r.InvoiceNO,
                r.PID.CardNO,
                r.Name,//personInfo.Name,
                r.IDCard,//personInfo.IdenNo,
                "null01",
                r.Pact.PayKind.ID,
                r.Pact.ID,
                r.Pact.Name,
                "00W999",
                this.GetSysDate("yyyy-MM-dd HH:mm:ss"),
                r.SIMainInfo.TotCost,
                r.SIMainInfo.PubCost,
                r.SIMainInfo.OwnCost,
                "1",
                r.DoctorInfo.Templet.Dept.ID,
                this.GetSysDate("yyyy-MM-dd HH:mm:ss"),
                r.SIMainInfo.TypeCode,//业务类型
                "",//科室名称
                "0",//结算标志
                r.SIMainInfo.MdtrtareaAdmvs,
                r.SIMainInfo.InsuplcAdmdvs,
                r.SIMainInfo.MdtrtCertType,
                r.SIMainInfo.MdtrtCertNo,
                r.SIMainInfo.PsnNo,
                r.SIMainInfo.PsnCertType,
                r.SIMainInfo.Certno,
                r.SIMainInfo.Gend,
                r.SIMainInfo.Naty,
                r.SIMainInfo.Age,
                r.SIMainInfo.Insutype,
                r.SIMainInfo.OpterType,
                r.SIMainInfo.MdtrtID,
                r.SIMainInfo.SetlId,
                r.SIMainInfo.SetlTime,
                r.SIMainInfo.MedfeeSumamt,
                r.SIMainInfo.FulamtOwnpayAmt,
                r.SIMainInfo.OverlmtSelfpay,
                r.SIMainInfo.PreselfpayAmt,
                r.SIMainInfo.InscpScpAmt,
                r.SIMainInfo.ActPayDedc,
                r.SIMainInfo.HifpPay,
                r.SIMainInfo.PoolPropSelfpay,
                r.SIMainInfo.CvlservPay,
                r.SIMainInfo.HifesPay,
                r.SIMainInfo.HifmiPay,
                r.SIMainInfo.HifobPay,
                r.SIMainInfo.MafPay,
                r.SIMainInfo.HospPartAmt,
                r.SIMainInfo.OthPay,
                r.SIMainInfo.FundPaySumamt,
                r.SIMainInfo.PsnPartAmt,
                r.SIMainInfo.AcctPay,
                r.SIMainInfo.PsnCashPay,
                r.SIMainInfo.Balc,
                r.SIMainInfo.AcctMulaidPay,
                r.SIMainInfo.MedinsSetlId,
                r.SIMainInfo.ClrOptins,
                r.SIMainInfo.ClrWay,
                r.SIMainInfo.ClrType,
                "1",
                r.Sex.ID,
                this.GetSysDate("yyyy-MM-dd HH:mm:ss"),
                r.SIMainInfo.MedType
                );
                if (this.ExecNoQuery(strSql) < 0)
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                return -1;
            }

            return 1;
        }
    }
}
