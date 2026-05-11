using Neusoft.FrameWork.Function;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.OutPatient.Register
{
    public class Manager : Neusoft.FrameWork.Management.Database
    {
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
            if (this.Sql.GetCommonSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                seeNo = Neusoft.FrameWork.Function.NConvert.ToInt32(No);
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
            if (this.Sql.GetCommonSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                minNo = Neusoft.FrameWork.Function.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找最小看诊序号出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }
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
            if (this.Sql.GetCommonSql(str, ref sql) == -1)
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
                cnt = Neusoft.FrameWork.Function.NConvert.ToInt32(No);
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
                return obj;
            }
            catch (Exception ex)
            {
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
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }

        /// <summary>
        /// 获取处方号
        /// </summary>
        /// <returns></returns>
        public string GetOpbRecipeNoSequece()
        {
            string sql = "select SEQ_OPB_RECIPE_NO.NEXTVAL from dual ";
            return this.ExecSqlReturnOne(sql);
        }
        /// <summary>
        /// 获取医嘱流水
        /// </summary>
        /// <returns></returns>
        public string GetMetMOOrderIDSequece()
        {
            string sql = "SELECT SEQ_MET_ORDER_ID.NEXTVAL FROM dual ";
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 根据工号查询人员所属科室
        /// </summary>
        /// <returns></returns>
        public string GetBelongDeptCodeForEmplCode(string emplCode)
        {
            string sql = "SELECT dept_code FROM com_employee WHERE empl_code = '{0}' ";
            sql = string.Format(sql,emplCode);
            return this.ExecSqlReturnOne(sql);
        }

        public int GetInvoiceR(string sql, string oper_id, DateTime now, ref string realInvoice, ref string invoiceStr, ref string erro)
        {
            erro = "";
            sql = string.Format(sql, oper_id);
            System.Data.DataTable dt = this.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        realInvoice = dt.Rows[i][0].ToString();
                        invoiceStr = dt.Rows[i][1].ToString();
                        break;
                    }
                    if (invoiceStr.Substring(0, 6) != now.ToString("yyMMdd"))
                    {
                        string userCode = this.GetInvoiceCode(oper_id);
                        invoiceStr = now.ToString("yyMMdd") + userCode + "0001";
                    }
                }
                else
                {
                    realInvoice = "";
                    invoiceStr = "";
                    erro = "没有找到发票信息！";
                    return -1;
                }
            }
            else
            {
                realInvoice = "";
                invoiceStr = "";
                erro = "没有找到发票信息！";
                return -1;
            }

            return 1;
        }

        private string GetInvoiceCode(string operID)
        {
            string sql = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.GetInvoiceUserCode;
            sql = string.Format(sql, operID);
            System.Data.DataTable dt = new System.Data.DataTable();
            string userCode = string.Empty;
            dt = this.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        userCode = dt.Rows[0][0].ToString();
                    }
                }
            }
            return userCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oper_id"></param>
        /// <param name="starInvoice"></param>
        /// <param name="invoiceGetTime"></param>
        public void GetUnUseInvoice(string oper_id, ref string starInvoice, ref string invoiceGetTime)
        {
            string returnNumber = string.Empty;
            string sql = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.GetUnUseInvoce;
            sql = string.Format(sql, oper_id);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = this.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        invoiceGetTime = dt.Rows[0][0].ToString();
                        starInvoice = dt.Rows[0][1].ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string AddNumber(string number)
        {
            string returnNumber = string.Empty;
            string sql = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.addnumber;
            sql = string.Format(sql, number);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = this.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        returnNumber = dt.Rows[0][0].ToString();
                    }
                }
            }
            return returnNumber;
        }

        /// <summary>
        /// 挂号实体
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public string[] GetRegInfo(Models.Views.OutPatient.ComPatient patient)
        {
            string[] argm = {
                               patient.ClinicCode, //门诊号/发票号
                               patient.CardNo, //就诊卡号
                               patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"), //挂号日期RegDate
                               patient.Noon.ID, //午别
                               patient.Name, //姓名
                               patient.IDCard, //身份证号
                               patient.SexCode, //性别
                               patient.Birthday, //出生日
                               patient.Pact.PayKind.ID, //结算类别号
                               patient.Pact.PayKind.Name, //结算类别名称
                               patient.Pact.ID, //合同号
                               patient.Pact.Name, //合同单位名称
                               patient.McardNo, //医疗证号
                               patient.RegLevel.ID, //挂号级别
                               patient.RegLevel.Name, //挂号级别名称
                               patient.Dept.ID, //科室号
                               patient.Dept.Name, //科室名称
                               patient.SeeNO.ToString(), //看诊序号
                               patient.Doct.ID, //医师代号
                               patient.Doct.Name, //医师姓名
                               //"", //看诊日期
                               "1", //挂号收费标志
                               //"0", //是否预约
                               patient.Isbooking,
                               "0", //1初诊/2复诊
                               patient.RegFee.ToString(), //挂号费
                               "0", //检查费
                               (patient.OwnDigFee+patient.PubDigFee+ patient.Ecost-patient.RegFee).ToString(), //诊察费
                               "0", //附加费
                               (patient.RegFee + patient.OwnDigFee + patient.Ecost).ToString(), //自费金额
                               patient.PubDigFee.ToString(), //报销金额
                               "0", //自付金额
                               "1", //退号标志
                               patient.Oper.ID, //操作员代码
                               "0", //是否看诊
                               "0", //1未核查/2已核查
                               patient.HomePhone, //联系电话
                               patient.Address, //地址
                               "1", //交易类型
                               "", //证件类型
                               patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"), //开始时间段
                               patient.End.ToString("yyyy-MM-dd HH:mm:ss"), //结束时间段
                               "", //作废人
                               "", //作废时间
                               patient.InvoiceStr,//发票号
                               "",//处方号
                               "0",//是否加号
                               "",//每日顺序号
                               patient.SchemaID,//排班序号
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"), //操作时间
                               "",//患者来源
                               "0",//1：需要提取病案0：不需要提取病案
                               "0",//是否加密姓名
                               "",//密文
                               "",//优惠金额
                               "0",//账户流程标识1 账户挂号 0普通
                               "0",//是否急诊号
                               "",//扩展字段1
                               "",//56当前使用卡号
                               "",//57当前使用卡类型
                               patient.InTimes.ToString(),//58登记次数
                               "1",//患者类别（普通、VIP、特诊等） 常数PersonType
                               patient.RegNo,//诊金登记单号
                               (patient.OwnDigFee + patient.PubDigFee).ToString(),//诊金金额
                               patient.RegDiagCode, //诊金代码
                               "1",//分诊标志,0未分/1已分
                               patient.Oper.ID,//分诊护士代码
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//分诊时间
                               "CORE_HIS50",
                               patient.InformedConsentResult
                            };

            return argm;
        }

        public string[] GetRegFeeInfo(Models.Views.OutPatient.ComPatient patient)
        {
            string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               patient.RegFee.ToString(),//总额
                               patient.Oper.ID,//收费人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
                               patient.Oper.ID,//操作人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//操作时间
                               "0",//0未日结/1已日结
                               "",//日结标识号
                               "",//日结人
                               "",//日结时间
                               "1",//‘0’ 无效 ‘1’ 有效,2退费
                               patient.RealInvoice,//实际发票打印号码
                               "3",//1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
                               patient.ClinicCode,//病历号/门诊号
                               "",//备注
                               patient.RegFee.ToString(),//自费金额
                               "0",//报销金额
                               "0",//自付金额
                               "COMM"//支付方式
                            };

            return argm;
        }

        public string[] GetDiagFeeInfo(Models.Views.OutPatient.ComPatient patient)
        {
            if (patient.Pact.ID == "99"|| patient.Ecost!= 0 )//本院职工挂号
            {
                string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               (patient.OwnDigFee+patient.PubDigFee+patient.Ecost).ToString(),//总额
                               patient.Oper.ID,//收费人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
                               patient.Oper.ID,//操作人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//操作时间
                               "0",//0未日结/1已日结
                               "",//日结标识号
                               "",//日结人
                               "",//日结时间
                               "1",//‘0’ 无效 ‘1’ 有效,2退费
                               patient.RealInvoice,//实际发票打印号码
                               "4",//1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
                               patient.ClinicCode,//病历号/门诊号
                               "",//备注
                               patient.OwnDigFee.ToString(),//自费金额
                               patient.PubDigFee.ToString(),//报销金额
                               patient.Ecost.ToString(),//自付金额
                               patient.PayType//支付方式
                            };
                return argm;
            }
            else
            {
                string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               (patient.OwnDigFee+patient.PubDigFee).ToString(),//总额
                               patient.Oper.ID,//收费人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
                               patient.Oper.ID,//操作人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//操作时间
                               "0",//0未日结/1已日结
                               "",//日结标识号
                               "",//日结人
                               "",//日结时间
                               "1",//‘0’ 无效 ‘1’ 有效,2退费
                               patient.RealInvoice,//实际发票打印号码
                               "4",//1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
                               patient.ClinicCode,//病历号/门诊号
                               "",//备注
                               patient.OwnDigFee.ToString(),//自费金额
                               patient.PubDigFee.ToString(),//报销金额
                               "0",//自付金额
                               patient.PayType//支付方式
                            };
                return argm;
            }
        }

        //public string[] GetEcostFeeInfo(Models.Views.OutPatient.ComPatient patient)
        //{
        //    if (patient.Pact.ID == "99")//本院职工挂号
        //    {
        //        string[] argm = {
        //                       patient.InvoiceStr,//发票
        //                       "1",//交易类型
        //                       patient.CardNo,//门诊卡号
        //                       patient.McardNo,//医疗证号
        //                       "",//身份标识卡类别 0无卡1磁卡 2IC卡
        //                       (patient.Ecost).ToString(),//总额
        //                       patient.Oper.ID,//收费人
        //                       patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
        //                       patient.Oper.ID,//操作人
        //                       patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//操作时间
        //                       "0",//0未日结/1已日结
        //                       "",//日结标识号
        //                       "",//日结人
        //                       "",//日结时间
        //                       "1",//‘0’ 无效 ‘1’ 有效,2退费
        //                       patient.RealInvoice,//实际发票打印号码
        //                       "8",//1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
        //                       patient.ClinicCode,//病历号/门诊号
        //                       "",//备注
        //                       (patient.Ecost).ToString(),//自费金额
        //                       "0",//报销金额
        //                       "0",//自付金额
        //                       "PTECOST"//支付方式
        //                    };
        //        return argm;
        //    }
        //    else
        //    {
        //        string[] argm = {
        //                       patient.InvoiceStr,//发票
        //                       "1",//交易类型
        //                       patient.CardNo,//门诊卡号
        //                       patient.McardNo,//医疗证号
        //                       "",//身份标识卡类别 0无卡1磁卡 2IC卡
        //                       (patient.Ecost).ToString(),//总额
        //                       patient.Oper.ID,//收费人
        //                       patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
        //                       patient.Oper.ID,//操作人
        //                       patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//操作时间
        //                       "0",//0未日结/1已日结
        //                       "",//日结标识号
        //                       "",//日结人
        //                       "",//日结时间
        //                       "1",//‘0’ 无效 ‘1’ 有效,2退费
        //                       patient.RealInvoice,//实际发票打印号码
        //                       "8",//1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
        //                       patient.ClinicCode,//病历号/门诊号
        //                       "",//备注
        //                       (patient.Ecost).ToString(),//自费金额
        //                       "0",//报销金额
        //                       "0",//自付金额
        //                        "PTECOST"//支付方式
        //                    };
        //        return argm;
        //    }
        //}

        public string[] GetAssignRecordInfo(Models.Views.OutPatient.ComPatient patient)
        {
            string[] argm = {
                                patient.ClinicCode,   //门诊号
                                patient.SeeNO.ToString(),   //看诊序号
                                patient.CardNo,   //病历号
                                //patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),   //挂号日期
                                patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"),   //挂号日期
                                patient.Name,   //患者姓名
                                patient.SexCode,   //性别
                                "01",   //结算类别
                                "0",   //1急诊/0普通
                                "0",//patient.Book==null?"0":"1",   //1预约/0普通
                                patient.Dept.ID,   //看诊科室
                                patient.Dept.Name,   //科室名称
                                patient.Queue.Name,   //队列名称
                                patient.Room.ID,   //出诊诊室
                                patient.Queue.ID,   //队列代码
                                patient.Room.Name,   //诊室名称
                                patient.Doct.ID,   //看诊医生
                                patient.RegDate.ToString("yyyy-MM-dd"),   //看诊时间
                                "1",   //1分诊/2进诊/3诊出
                                patient.NurseCell.ID,   //分诊科室
                                patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),   //分诊时间
                                "",   //进诊时间
                                "",   //出诊时间
                                patient.Oper.ID,   //操作员
                                patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),  //操作时间
                                patient.Console.ID,//诊台代码
                                patient.Console.Name,//诊台名称
                                patient.RegLevel.ID,// 挂号级别代码
                                patient.RegLevel.Name,//挂号级别
                                "" //每日顺序号
                            };

            return argm;
        }


        public string[] GetRegFeeDetailInfo(Models.Views.OutPatient.ComPatient patient,string itemCode,string itemName,string itemPrice)
        {
            string[] argm = {
                               GetOpbRecipeNoSequece(),
                               "1",
                               "1",
                               patient.ClinicCode,
                               patient.CardNo,
                               patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"),
                               patient.Dept.ID,
                               patient.Doct.ID,
                               patient.Dept.ID,
                               itemCode,
                               itemName,
                               "0",
                               "次",
                               "015",
                               "U",
                               itemPrice,
                               "1",
                               "1",
                               "0",
                               "0",
                               "0",
                               "1",
                               "次",
                               "0",
                               "0",
                               itemPrice,
                               patient.Dept.ID,
                               patient.Dept.Name,
                               "0",
                               "00A105",
                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                               "0",
                               "1",
                               "0",
                               "0",
                               "1",
                               "1",
                               "0",
                               GetMetMOOrderIDSequece(),
                               itemPrice,
                               "0",
                               "0",
                               "0",
                               "0",
                               "0",
                               GetBelongDeptCodeForEmplCode(patient.Doct.ID),//医生所属科室
                               "01",
                               patient.Pact.ID,
                               itemPrice,
                               "0",
                               GetBelongDeptCodeForEmplCode(patient.Doct.ID),//开立医生所属科室
                               "CORE_HIS50",
                               "NULL"
                            };

            return argm;
        }

        /// <summary>
        /// 通过发票号查询费用信息
        /// </summary>
        /// <param name="invoiceNO"></param>
        /// <param name="lstCardFee"></param>
        /// <returns></returns>
        public int QueryAccountCardFeeByInvoiceNO(string invoiceNO, out List<Neusoft.HISFC.Models.Account.AccountCardFee> lstCardFee)
        {
            lstCardFee = null;
            if (string.IsNullOrEmpty(invoiceNO) || string.IsNullOrEmpty(invoiceNO))
            {
                this.Err = "参数不对！";
                return -1;
            }

            string strWhere = string.Empty;

            if (this.Sql.GetCommonSql("Fee.Account.CardFee.Where.4", ref strWhere) == -1)
            {
                this.Err = this.Err = "查找索引为 Fee.Account.CardFee.Where.4 的Sql语句失败！";
                return -1;
            }

            int iRes = 0;
            try
            {
                strWhere = string.Format(strWhere, invoiceNO);

                iRes = this.QueryAccountCardFeeSQL(strWhere, out lstCardFee);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }

            return iRes;
        }

        /// <summary>
        /// 查询卡费用信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="lstCardFee"></param>
        /// <returns></returns>
        private int QueryAccountCardFeeSQL(string sql, out List<Neusoft.HISFC.Models.Account.AccountCardFee> lstCardFee)
        {
            lstCardFee = null;

            string strSql = string.Empty;

            if (this.Sql.GetCommonSql("Fee.Account.CardFee.Select", ref strSql) == -1)
            {
                this.Err = this.Err = "查找索引为 Fee.Account.CardFee.Select 的Sql语句失败！";
                return -1;
            }

            try
            {
                strSql = strSql + sql;

                if (this.ExecQuery(strSql) == -1)
                {
                    return -1;
                }

                lstCardFee = new List<Neusoft.HISFC.Models.Account.AccountCardFee>();
                Neusoft.HISFC.Models.Account.AccountCardFee cardFee = null;
                while (this.Reader.Read())
                {
                    cardFee = new Neusoft.HISFC.Models.Account.AccountCardFee();
                    cardFee.InvoiceNo = this.Reader[0].ToString().Trim();
                    cardFee.TransType = this.Reader[1].ToString().Trim() == "1" ? Neusoft.HISFC.Models.Base.TransTypes.Positive : Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    cardFee.MarkNO = this.Reader[2].ToString().Trim();
                    cardFee.MarkType.ID = this.Reader[3].ToString().Trim();
                    cardFee.Tot_cost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[4].ToString());
                    cardFee.ID = this.Reader[5].ToString().Trim();
                    cardFee.FeeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6]);
                    cardFee.ID = this.Reader[7].ToString().Trim();
                    cardFee.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[8]);
                    cardFee.IsBalance = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[9].ToString());
                    cardFee.BalanceNo = this.Reader[10].ToString().Trim();
                    cardFee.ID = this.Reader[11].ToString().Trim();
                    cardFee.BalnaceOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[12]);
                    cardFee.IStatus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[13]);
                    cardFee.CardNo = this.Reader[14].ToString().Trim();

                    cardFee.Print_InvoiceNo = this.Reader[15].ToString().Trim();
                    switch (this.Reader[16].ToString().Trim())
                    {
                        case "1":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.CardFee;
                            break;
                        case "2":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.CaseFee;
                            break;
                        case "3":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.RegFee;
                            break;
                        case "4":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.DiaFee;
                            break;
                        case "5":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.ChkFee;
                            break;
                        case "6":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.AirConFee;
                            break;
                        case "7":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.OthFee;
                            break;
                        case "8":
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.PBFee;
                            break;
                        default:
                            cardFee.FeeType = Neusoft.HISFC.Models.Account.AccCardFeeType.OthFee;
                            break;
                    }
                    cardFee.ClinicNO = this.Reader[17].ToString().Trim();
                    cardFee.Remark = this.Reader[18].ToString().Trim();
                    cardFee.PayType.ID = this.Reader[19].ToString();
                    cardFee.Own_cost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20].ToString());
                    cardFee.Pub_cost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[21].ToString());
                    cardFee.Pay_cost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[22].ToString());

                    cardFee.Name = this.Reader[23].ToString().Trim();
                    cardFee.MarkType.Name = this.Reader[24].ToString().Trim();

                    lstCardFee.Add(cardFee);
                }

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 保存卡费用表
        /// </summary>
        /// <param name="cardFee"></param>
        /// <returns></returns>
        public int InsertAccountCardFee(Neusoft.HISFC.Models.Account.AccountCardFee cardFee)
        {
            if (cardFee == null)
                return -1;

            if (string.IsNullOrEmpty(cardFee.InvoiceNo))
            {
                this.Err = "发票流水号为空！";

                return -1;
            }
            //默认支付方式
            if (string.IsNullOrEmpty(cardFee.PayType.ID)) cardFee.PayType.ID = "CA";

            string Sql = string.Empty;
            if (this.Sql.GetCommonSql("Fee.Account.CardFee.Insert3", ref Sql) == -1)
            {
                this.Err = this.Err = "查找索引为 Fee.Account.CardFee.Insert 的Sql语句失败！";
                return -1;
            }
            try
            {
                Sql = string.Format(Sql,
                    cardFee.InvoiceNo,
                    ((int)cardFee.TransType).ToString(),
                    cardFee.CardNo,
                    cardFee.MarkNO,
                    cardFee.MarkType.ID,
                    cardFee.Tot_cost,
                    cardFee.ID,
                    cardFee.FeeOper.OperTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    cardFee.ID,
                    cardFee.Oper.OperTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    0,
                    "",
                    "",
                    "",
                    cardFee.IStatus,
                    cardFee.Print_InvoiceNo,
                    ((int)cardFee.FeeType).ToString(),
                    cardFee.ClinicNO,
                    cardFee.Remark,
                    cardFee.Own_cost,
                    cardFee.Pub_cost,
                    cardFee.Pay_cost,
                    cardFee.PayType.ID,
                    "",
                    ""
                    );
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(Sql);
        }

        /// <summary>
        /// 退费卡费用信息
        /// </summary>
        /// <param name="?"></param>
        /// <param name="flag">0：无效 1：有效 2:退费 3：作废</param>
        /// <returns></returns>
        public int CancelAccountCardFeeByInvoice(string invoice, int flag)
        {
            if (string.IsNullOrEmpty(invoice))
            {
                this.Err = "发票流水号为空！";

                return -1;
            }

            string Sql = string.Empty;
            if (this.Sql.GetCommonSql("Fee.Account.CardFee.Cancel.ByInvoice.1", ref Sql) == -1)
            {
                #region 默认sql
                Sql = @"update fin_opb_accountcardfee a
   set a.cancel_flag ={1}
 where a.invoice_no = '{0}'";
                #endregion
            }
            try
            {
                Sql = string.Format(Sql, invoice, flag);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(Sql);
        }

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
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 插入门诊挂号信息
        /// </summary>
        /// <param name="registerPatientInfo"></param>
        /// <returns></returns>
        public int InsertOutPatientReg(Neusoft.HISFC.Models.Registration.Register registerPatientInfo)
        {
            string balanceNo = this.GetBalanceNo(registerPatientInfo.ID);
            if (string.IsNullOrEmpty(balanceNo))
            {
                balanceNo = "0";
            }
            balanceNo = (int.Parse(balanceNo) + 1).ToString();

            #region sql
            string strSql = @"INSERT INTO fin_ipr_siinmaininfo_gd f
                                (
                                INPATIENT_NO,
                                REG_NO,
                                BALANCE_NO,
                                INVOICE_NO,
                                CARD_NO,
                                MCARD_NO,
                                NAME,
                                IDENNO,
                                CLINIC_DIAGNOSE,
                                PAYKIND_CODE,
                                PACT_CODE,
                                PACT_NAME,
                                OPER_CODE,
                                OPER_DATE,
                                TOT_COST,
                                PUB_COST,
                                OWN_COST,
                                VALID_FLAG,
                                FEE_TIMES,
                                SEX_CODE,
                                DEPT_CODE,
                                IN_DATE,
                                BALANCE_DATE,
                                TYPE_CODE, --22
                                BKA825,
                                BKA826,
                                AKA151,
                                BKA838,
                                AKB067, --27
                                AKB066,
                                BKA821,
                                BKA839,
                                AKE039,
                                AKE035,
                                AKE026,
                                AKE029,
                                BKA841,
                                BKA842,
                                BKA840,   --37 
                                PATIENT_NO,
                                AAA027,
                                AAZ267,
                                bka438,
                                aab301,
                                bka006,
                                aae140,
                                aka130,
                                DEPT_NAME
                                )
                                Values
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
                                to_date('{13}','YYYY-MM-DD hh24:mi:ss'),
                                '{14}',
                                '{15}',
                                '{16}',
                                '{17}',
                                0,
                                '{18}',
                                '{19}',
                                to_date('{20}','YYYY-MM-DD hh24:mi:ss'),
                                to_date('{21}','YYYY-MM-DD hh24:mi:ss'),
                                '{22}',
                                {23},
                                {24},
                                {25},
                                {26},
                                {27},
                                {28},
                                {29},
                                {30},
                                {31},
                                {32},
                                {33},
                                {34},
                                {35},
                                {36},
                                {37},
                                '{38}',
                                '{39}',
                                '{40}',
                                '{41}',
                                '{42}',
                                '{43}',
                                '{44}',
                                 '{45}',
                                 '{46}'
                                )";
            strSql = string.Format(strSql,
                registerPatientInfo.ID,
                registerPatientInfo.SIMainInfo.RegNo,
                balanceNo,
                registerPatientInfo.InvoiceNO,
                registerPatientInfo.PID.CardNO,
                registerPatientInfo.SSN,//personInfo.MCardNo,
                registerPatientInfo.Name,//personInfo.Name,
                registerPatientInfo.IDCard,//personInfo.IdenNo,
                registerPatientInfo.ClinicDiagnose,
                registerPatientInfo.Pact.PayKind.ID,
                registerPatientInfo.Pact.ID,
                registerPatientInfo.Pact.Name,
                "平台",
                this.GetSysDate("yyyy-MM-dd HH:mm:ss"),
                //registerPatientInfo.PVisit.InTime.ToString("yyyy-MM-dd HH:mm:ss"),
                registerPatientInfo.SIMainInfo.TotCost,
                registerPatientInfo.SIMainInfo.PubCost,
                registerPatientInfo.SIMainInfo.OwnCost,
                "1",
                registerPatientInfo.Sex.ID,
                registerPatientInfo.DoctorInfo.Templet.Dept.ID,//PVisit.PatientLocation.Dept.ID,
                registerPatientInfo.PVisit.InTime.ToString("yyyy-MM-dd HH:mm:ss"),
                registerPatientInfo.SIMainInfo.BalanceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                "0",
                registerPatientInfo.SIMainInfo.Bka825,
                registerPatientInfo.SIMainInfo.Bka826,
                registerPatientInfo.SIMainInfo.Aka151,
                registerPatientInfo.SIMainInfo.Bka838,
                registerPatientInfo.SIMainInfo.Akb067,
                registerPatientInfo.SIMainInfo.Akb066,
                registerPatientInfo.SIMainInfo.Bka821,
                registerPatientInfo.SIMainInfo.Bka839,
                registerPatientInfo.SIMainInfo.Ake039,
                registerPatientInfo.SIMainInfo.Ake035,
                registerPatientInfo.SIMainInfo.Ake026,
                registerPatientInfo.SIMainInfo.Ake029,
                registerPatientInfo.SIMainInfo.Bka841,
                registerPatientInfo.SIMainInfo.Bka842,
                registerPatientInfo.SIMainInfo.Bka840,
                registerPatientInfo.PID.PatientNO,
                registerPatientInfo.SIMainInfo.Aaa027,
                registerPatientInfo.SIMainInfo.Aaz267,
                registerPatientInfo.SIMainInfo.Bka438,
                registerPatientInfo.SIMainInfo.Aab301,
                registerPatientInfo.SIMainInfo.Bka006,
                registerPatientInfo.SIMainInfo.Aae140,
                registerPatientInfo.SIMainInfo.Aka130,
                registerPatientInfo.SIMainInfo.Bka020
                );

            #endregion

            try
            {
                this.Logo.WriteLog(strSql);
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

        public int UpdateOutPatientRegNo(string regno, string clincode)
        {
            string updatereg = @"UPDATE fin_opr_register reg
 SET reg.reg_no='{0}'
WHERE reg.clinic_code='{1}'";
            updatereg = string.Format(updatereg, regno, clincode);
            this.ExecNoQuery(updatereg);
            return 1;
        }

        //获取最大结算序号
        private string GetBalanceNo(string inpatientNo)
        {
            string strSql = @"select max(to_number(BALANCE_NO))
                                from fin_ipr_siinmaininfo_gd
                               where inpatient_no = '{0}'";
            strSql = string.Format(strSql, inpatientNo);

            try
            {
                return this.ExecSqlReturnOne(strSql);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取移动支付挂号订单号
        /// </summary>
        /// <param name="AppOrderId"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public int GetGHYDZFOrderID(string AppOrderId, ref string orderid)
        {
            string strSql = @"select Order_id from PF_YDZF_ORDER o where o.app_order_id ='{0}' and order_type = '0'";
            try
            {
                strSql = string.Format(strSql, AppOrderId);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            this.ExecQuery(strSql);
            while (Reader.Read())
            {
                orderid = Reader[0].ToString();
            }
            return 1;
        }

        /// <summary>
        /// 得到门诊挂号医保主表信息
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <returns></returns>
        public int GetRegSIPersonInfo(string inpatientNo, ref Neusoft.HISFC.Models.Registration.Register obj)
        {
            #region MyRegion
            //Neusoft.HISFC.Models.Registration.Register obj = new Neusoft.HISFC.Models.Registration.Register();
            string strSql = @"SELECT inpatient_no,   --住院流水号
                           reg_no,       --就医登记号
                           balance_no,   --结算序号
                           invoice_no,   --发票号
                           medical_type,   --医疗类别
                           patient_no,   --住院号
                           card_no,   --就诊卡号
                           mcard_no,   --医疗证号
                           name,   --姓名
                           sex_code,   --性别
                           idenno,   --身份证号
                           birthday,   --生日
                           empl_type,   --人员类别 1 在职 2 退休
                           work_name,   --工作单位
                           clinic_diagnose,   --门诊诊断
                           dept_code,   --科室代码
                           dept_name,   --科室名称
                           paykind_code,   --结算类别 1-自费  2-保险 3-公费在职 4-公费退休 5-公费高干
                           pact_code,   --合同代码
                           pact_name,   --合同单位名称
                           bed_no,   --床号
                           in_date,   --入院日期
                           in_diagnosedate,--入院诊断日期
                           in_diagnose,   --入院诊断代码
                           in_diagnosename,   --入院诊断名称
                           out_date,   --出院日期
                           out_diagnose,   --出院诊断代码
                           out_diagnosename,   --出院诊断名称
                           balance_date,   --结算日期(上次)
                           tot_cost,   --费用金额(未结)(住院总金额)
                           pay_cost,   --帐户支付
                           pub_cost,   --公费金额(未结)(社保支付金额)
                           item_paycost,   --部分项目自付金额
                           base_cost,   --个人起付金额
                           item_paycost2,   --个人自费项目金额
                           item_ylcost,   --个人自付金额（乙类自付部分）
                           own_cost,   --个人自负金额
                           overtake_owncost,   --超统筹支付限额个人自付金额
                           own_cause,   --自费原因
                           oper_code,   --操作员
                           oper_date,    --操作日期
                           fee_times,
                           hos_cost,
                           year_cost,
                           VALID_FLAG,
                           BALANCE_STATE,
                           remark,
                           type_code,
                           over_cost,
                           person_type,
                           bka911,
                           bka912,
                            bka913,
                            bka914,
                            bka915,
                            bka916,
                            bka917,
                            bka042,
                            aaz267,
                            bka825,
                            bka826,
                            aka151,
                            bka838,
                            akb067,
                            akb066,
                            bka821,
                            bka839,
                            ake039,
                            ake035,
                            ake026,
                            ake029,
                            bka841,
                            bka842,
                            bka840,
                            aaa027,
                            bka006,
                            aab301,
                            aae140,
                            aka130
                      FROM fin_ipr_siinmaininfo_gd   --广东省统一医保信息住院主表
                     WHERE   inpatient_no = '{0}'
                             and valid_flag = '1'                             
                             and type_code = '0'";

            try
            {
                strSql = string.Format(strSql, inpatientNo);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            this.ExecQuery(strSql);
            try
            {
                while (Reader.Read())
                {
                    obj.ID = Reader[0].ToString();
                    obj.SIMainInfo.RegNo = Reader[1].ToString();
                    obj.SIMainInfo.BalNo = Reader[2].ToString();
                    obj.SIMainInfo.InvoiceNo = Reader[3].ToString();
                    obj.SIMainInfo.MedicalType.ID = Reader[4].ToString();
                    obj.PID.PatientNO = Reader[5].ToString();
                    obj.PID.CardNO = Reader[6].ToString();
                    obj.SSN = Reader[7].ToString();
                    obj.Name = Reader[8].ToString();
                    obj.Sex.ID = Reader[9].ToString();
                    obj.IDCard = Reader[10].ToString();
                    obj.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[11].ToString());
                    obj.SIMainInfo.EmplType = Reader[12].ToString();
                    obj.CompanyName = Reader[13].ToString();
                    obj.ClinicDiagnose = Reader[14].ToString();
                    obj.PVisit.PatientLocation.Dept.ID = Reader[15].ToString();
                    obj.PVisit.PatientLocation.Dept.Name = Reader[16].ToString();
                    obj.Pact.PayKind.ID = Reader[17].ToString();
                    obj.Pact.ID = Reader[18].ToString();
                    obj.Pact.Name = Reader[19].ToString();
                    obj.PVisit.PatientLocation.Bed.ID = Reader[20].ToString();
                    obj.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[21].ToString());
                    obj.SIMainInfo.InDiagnoseDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[22].ToString());
                    obj.SIMainInfo.InDiagnose.ID = Reader[23].ToString();
                    obj.SIMainInfo.InDiagnose.Name = Reader[24].ToString();
                    if (!Reader.IsDBNull(25))
                        obj.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[25].ToString());
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
                    obj.SIMainInfo.Memo = Reader[38].ToString();//
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
                    if (!Reader.IsDBNull(50))//--
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
                    obj.SIMainInfo.Bka006 = Reader[75].ToString();
                    obj.SIMainInfo.Aab301 = Reader[76].ToString();
                    obj.SIMainInfo.Aae140 = Reader[77].ToString();
                    obj.SIMainInfo.Aka130 = Reader[78].ToString();
                }
                Reader.Close();
                return 1;
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                Reader.Close();
                return -1;
            }
            #endregion
        }

        public int UpdateSIPersonInfo(string clincode)
        {
            string sql = @"UPDATE fin_ipr_siinmaininfo_gd p
SET p.valid_flag='0'
WHERE p.inpatient_no='{0}'";
            sql = string.Format(sql, clincode);
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 14周岁限制科室
        /// </summary>
        /// <param name="deptcode"></param>
        /// <returns></returns>
        public string GetAge14LimitDept(string deptcode)
        {
            string sql = @"SELECT p.dept_code FROM com_department p WHERE p.bro_name='内科' AND p.dept_type='C' AND dept_code='{0}'
union all
SELECT d.code FROM com_dictionary d WHERE d.type ='Age14LimitDept' AND d.code='{0}'
";
            sql = string.Format(sql, deptcode);
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 儿科科室，14周岁以上限制
        /// </summary>
        /// <param name="deptcode"></param>
        /// <returns></returns>
        public string GetPediatricsDept(string deptcode)
        {
            string sql = @"SELECT d.code FROM com_dictionary d WHERE d.type ='PediatricsDeptCodeList' AND d.code='{0}'";
            sql = string.Format(sql, deptcode);
            return this.ExecSqlReturnOne(sql);
        }
        /// <summary>
        /// 获取医嘱扩展表的限制药集合
        /// </summary>
        /// <param name="ID"></param>
        public Hashtable GetItemLimitList(string ID)
        {
            //根据流水号获取限制药列表
            Hashtable hsLimit = new Hashtable();

            Neusoft.HISFC.BizLogic.Order.OrderExtend orderExtendMgr = new Neusoft.HISFC.BizLogic.Order.OrderExtend();
            ArrayList al = orderExtendMgr.QueryByInpatine(ID);
            if (al != null && al.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Order.Inpatient.OrderExtend ordExt in al)
                {
                    hsLimit.Add(ordExt.MoOrder, ordExt);
                }
            }
            return hsLimit;
        }

        /// <summary>
        /// 插入门诊收费信息
        /// </summary>
        /// <param name="registerPatientInfo"></param>
        /// <returns></returns>
        public int InsertOutPatientBalance(Neusoft.HISFC.Models.Registration.Register patientInfo)
        {
            string balanceNo = this.GetBalanceNo(patientInfo.ID);
            if (string.IsNullOrEmpty(balanceNo))
            {
                balanceNo = "0";
            }
            balanceNo = (int.Parse(balanceNo) + 1).ToString();
            patientInfo.SIMainInfo.BalNo = balanceNo;
            string TacCode = "0";
            if (string.IsNullOrEmpty(patientInfo.SIMainInfo.TacCode))
            {
                TacCode = "0";
            }
            else
            {
                TacCode = patientInfo.SIMainInfo.TacCode;
            }
            #region sql
            string strSql = @"INSERT INTO fin_ipr_siinmaininfo_gd f
                                (
                                INPATIENT_NO,
                                REG_NO,
                                BALANCE_NO,
                                INVOICE_NO,
                                CARD_NO,
                                MCARD_NO, --5
                                NAME,
                                IDENNO,
                                CLINIC_DIAGNOSE,
                                PAYKIND_CODE,
                                PACT_CODE,--10
                                PACT_NAME,
                                OPER_CODE,
                                OPER_DATE,
                                TOT_COST,
                                PUB_COST,--15
                                OWN_COST,
                                VALID_FLAG,--17
                                FEE_TIMES,
                                SEX_CODE,--18
                                DEPT_CODE,
                                IN_DATE,--20
                                BALANCE_DATE,
                                TYPE_CODE,
                                BKA825,
                                BKA826,
                                AKA151,
                                BKA838,
                                AKB067,--27
                                AKB066,
                                BKA821,
                                BKA839,
                                AKE039,
                                AKE035,--32
                                AKE026,
                                AKE029,
                                BKA841,
                                BKA842,
                                BKA840,    --37        
                                PATIENT_NO,
                                AAA027,
                                AAZ267,
                                AAB301,
                                AAE140,
                                BKA006,
                                AKA130,
                                ic_reg_permit,
                                empl_type          
                                 )
                                Values
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
                                to_date('{13}','YYYY-MM-DD hh24:mi:ss'),
                                '{14}',
                                '{15}',
                                '{16}',
                                '{17}',
                                0,
                                '{18}',
                                '{19}',
                                to_date('{20}','YYYY-MM-DD hh24:mi:ss'),
                                to_date('{21}','YYYY-MM-DD hh24:mi:ss'),
                                '{22}',
                                {23},
                                {24},
                                {25},
                                {26},
                                {27},
                                {28},
                                {29},
                                {30},
                                {31},
                                {32},
                                {33},
                                {34},
                                {35},
                                {36},
                                {37},
                                '{38}',
                                '{39}',
                                '{40}',
                                '{41}',
                                '{42}',
                                '{43}',
                                 {44},
                                 '{45}',
                                 '{46}'
                                )";
            strSql = string.Format(strSql,
                patientInfo.ID,
                patientInfo.SIMainInfo.RegNo,
                balanceNo,
                patientInfo.SIMainInfo.InvoiceNo,
                patientInfo.PID.CardNO,
                patientInfo.SSN,//personInfo.MCardNo,
                patientInfo.Name,//personInfo.Name,
                patientInfo.IDCard,//personInfo.IdenNo,
                patientInfo.ClinicDiagnose,
                patientInfo.Pact.PayKind.ID,
                patientInfo.Pact.ID,
                patientInfo.Pact.Name,
                //this.Operator.ID,
                BP.Common.Function.DefaultOper.Code,
                this.GetDateTimeFromSysDateTime().ToString(),
                patientInfo.SIMainInfo.TotCost,
                patientInfo.SIMainInfo.PubCost,
                patientInfo.SIMainInfo.OwnCost,
                "1",
                patientInfo.Sex.ID,
                patientInfo.DoctorInfo.Templet.Dept.ID,//SeeDoct.Dept.ID,//PVisit.PatientLocation.Dept.ID,
                patientInfo.PVisit.InTime.ToString("yyyy-MM-dd HH:mm:ss"),
                patientInfo.SIMainInfo.BalanceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                "1",
                patientInfo.SIMainInfo.Bka825,
                patientInfo.SIMainInfo.Bka826,
                patientInfo.SIMainInfo.Aka151,
                patientInfo.SIMainInfo.Bka838,
                patientInfo.SIMainInfo.Akb067,
                patientInfo.SIMainInfo.Akb066,
                patientInfo.SIMainInfo.Bka821,
                patientInfo.SIMainInfo.Bka839,
                patientInfo.SIMainInfo.Ake039,
                patientInfo.SIMainInfo.Ake035,
                patientInfo.SIMainInfo.Ake026,
                patientInfo.SIMainInfo.Ake029,
                patientInfo.SIMainInfo.Bka841,
                patientInfo.SIMainInfo.Bka842,
                patientInfo.SIMainInfo.Bka840,
                patientInfo.PID.PatientNO,
                patientInfo.SIMainInfo.Aaa027,
                patientInfo.SIMainInfo.Aaz267,
                patientInfo.SIMainInfo.Aab301,
                patientInfo.SIMainInfo.Aae140,
                patientInfo.SIMainInfo.Bka006,
                patientInfo.SIMainInfo.Aka130,
                TacCode,
                patientInfo.SIMainInfo.Bka004
                );

            #endregion

            try
            {
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

        #region 退费相关

        public List<Neusoft.HISFC.Models.Fee.Outpatient.BalancePay> QueryPayTypeByInvoiceNO(string invoiceNO)
        {
            #region
            string sql = @"		     SELECT 
                 INVOICE_NO,	--VARCHAR2(12)	N			发票号
                 TRANS_TYPE,	--VARCHAR2(1)	N			交易类型,1正，2反
                 SEQUENCE_NO, --	NUMBER(2)	N			交易流水号
                 MODE_CODE,	--VARCHAR2(1)	Y			支付方式
                 TOT_COST,	--NUMBER(8,2)	Y			应付金额
                 REAL_COST,	--NUMBER(8,2)	Y			实付金额
                 BANK_CODE,	--VARCHAR2(3)	Y			开户银行代码
                 BANK_NAME,	--VARCHAR2(50)	Y			开户银行名称
                 ACCOUNT,	--VARCHAR2(20)  Y      账号
                 POS_NO,  --VARCHAR2(20)  Y      pos机号
                 CHECK_NO,  --VARCHAR2(20)  Y      支票号
                 OPER_CODE,  --VARCHAR2(6)  Y      结算人
                 OPER_DATE,  --DATE  Y      结算时间
                 CHECK_FLAG,  --VARCHAR2(1)  Y      1未核查/2已核查
                 CHECK_OPCD,  --VARCHAR2(6)  Y      核查人
                 CHECK_DATE,  --DATE  Y      核查时间
                 BALANCE_FLAG,  --VARCHAR2(1)  Y      1已日结/2未日结
                 BALANCE_NO,  --VARCHAR2(10)  Y      日结标识号
                 BALANCE_OPCD,  --VARCHAR2(6)  Y      日结人
                 CORRECT_FLAG,  --VARCHAR2(1)  Y      1未对帐/2已对帐
                 CORRECT_OPCD,  --VARCHAR2(6)  Y      对帐人
                 CORRECT_DATE,  --DATE  Y      对帐时间
                 BALANCE_DATE,  --DATE  Y      日结时间
     INVOICE_SEQ,
                 CANCEL_FLAG 
             From fin_opb_paymode m 
             WHERE EXISTS(SELECT 1 FROM fin_opb_invoiceinfo a WHERE a.invoice_seq=m.invoice_seq AND a.invoice_no='{0}' AND a.trans_type='2')";
            #endregion
            sql = string.Format(sql, invoiceNO);

            if (this.ExecQuery(sql) == -1)
            {
                return null;
            }

            List<Neusoft.HISFC.Models.Fee.Outpatient.BalancePay> balancePays = new List<Neusoft.HISFC.Models.Fee.Outpatient.BalancePay>();//支付方式信息
            Neusoft.HISFC.Models.Fee.Outpatient.BalancePay balancePay;//支付方式实体

            try
            {
                //循环读取数据
                while (this.Reader.Read())
                {
                    balancePay = new Neusoft.HISFC.Models.Fee.Outpatient.BalancePay();

                    balancePay.Invoice.ID = this.Reader[0].ToString();//,	--		发票号
                    if (this.Reader[1].ToString() == "2")//交易类型
                    {
                        balancePay.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    }
                    else
                    {
                        balancePay.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                    }
                    balancePay.Squence = this.Reader[2].ToString();//交易流水号
                    balancePay.PayType.ID = this.Reader[3].ToString();//支付方式
                    balancePay.FT.TotCost = NConvert.ToDecimal(this.Reader[4].ToString());//应付金额
                    balancePay.FT.RealCost = NConvert.ToDecimal(this.Reader[5].ToString());//实付金额
                    balancePay.Bank.ID = this.Reader[6].ToString();//银行号
                    balancePay.Bank.Name = this.Reader[7].ToString();//名
                    balancePay.Bank.Account = this.Reader[8].ToString();//帐号
                    balancePay.POSNO = this.Reader[9].ToString();//pos号
                    balancePay.Bank.InvoiceNO = this.Reader[10].ToString();//支票号
                    balancePay.InputOper.ID = this.Reader[11].ToString();//结算人
                    balancePay.InputOper.OperTime = NConvert.ToDateTime(this.Reader[12].ToString());//结算时间
                    //是否核查
                    if (this.Reader[13].ToString() == "1")
                    {
                        balancePay.IsAuditing = true;
                    }
                    else
                    {
                        balancePay.IsAuditing = false;
                    }
                    balancePay.AuditingOper.ID = this.Reader[14].ToString();
                    balancePay.AuditingOper.OperTime = NConvert.ToDateTime(this.Reader[15].ToString());//检查时间
                    balancePay.IsDayBalanced = NConvert.ToBoolean(this.Reader[16].ToString());//是否日结
                    balancePay.BalanceOper.ID = this.Reader[18].ToString();//日结人
                    //是否对帐
                    if (this.Reader[19].ToString() == "1")
                    {
                        balancePay.IsChecked = true;
                    }
                    else
                    {
                        balancePay.IsChecked = false;
                    }
                    balancePay.CheckOper.ID = this.Reader[20].ToString();//对帐人
                    balancePay.CheckOper.OperTime = NConvert.ToDateTime(this.Reader[21].ToString());//对帐时间
                    balancePay.BalanceOper.OperTime = NConvert.ToDateTime(this.Reader[22].ToString());//日结时间
                    balancePay.InvoiceCombNO = this.Reader[23].ToString();//发票序号
                    balancePay.CancelType = (Neusoft.HISFC.Models.Base.CancelTypes)NConvert.ToInt32(this.Reader[24].ToString());

                    balancePays.Add(balancePay);
                }//循环结束

                this.Reader.Close();

                return balancePays;
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }

                return null;
            }
        }

        public decimal QuitRegFee(string invoiceno)
        {
            try
            {
                string sql = @"SELECT  SUM(fee.own_cost)
FROM fin_opb_accountcardfee fee 
WHERE  fee.pay_type in ('PTWX','PTYBK','PTYL','PTZFB')
AND fee.trans_type='2'
AND fee.invoice_no='{0}'";
                sql = string.Format(sql, invoiceno);
                decimal tot = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.ExecSqlReturnOne(sql));
                tot = Math.Abs(tot) * 100;
                return tot;
            }
            catch
            {
                return 0;
            }
        }


        public string GetReglevlName(string ReglevlCode)
        {
            try
            {
                string sql = @"SELECT
       reglevl_name --挂号级别名称
  FROM fin_opr_reglevel  --挂号级别维护
WHERE valid_state = '1'
and reglevl_code = '{0}'";
                sql = string.Format(sql, ReglevlCode);
                string reglevlName = this.ExecSqlReturnOne(sql);
                return reglevlName;
            }
            catch
            {
                return "";
            }
        }

        public string GetItemNameForItemCode(string itemCode)
        {
            try
            {
                string sql = @"select item_name from fin_com_undruginfo  where item_code='{0}'";
                sql = string.Format(sql, itemCode);
                string itemName = this.ExecSqlReturnOne(sql);
                return itemName;
            }
            catch
            {
                return "";
            }
        }
        public string GetPriceForItemCode(string itemCode)
        {
            try
            {
                string sql = @"select  decode(unitflag, '1', fun_get_packageprice(item_code), unit_price) unit_price from fin_com_undruginfo  where item_code='{0}'";
                sql = string.Format(sql, itemCode);
                string itemPrice = this.ExecSqlReturnOne(sql);
                return itemPrice;
            }
            catch
            {
                return "";
            }
        }

        public int UpdatePlaStatus()
        {
            string updatestatu = @"UPDATE  platform_register_order regord
SET regord.status='6'
WHERE EXISTS(SELECT 1 FROM fin_opr_register reg WHERE reg.clinic_code=regord.registerid AND reg.valid_flag='1' 
AND reg.ynsee='1' AND reg.reg_date>=trunc(SYSDATE))";
            try
            {
                if (this.ExecNoQuery(updatestatu) < 0)
                {
                    return -1;
                }
                return 1;
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                return -1;
            }
        }

        #endregion

        #region 医保相关
        /// <summary>
        /// 获取医保挂号最大时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetMaxSiRegTime(string idno)
        {
            return DateTime.MinValue;
        }
        #endregion
    }
}
