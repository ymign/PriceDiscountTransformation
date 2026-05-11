using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Models.ZZSB;
using System.Xml.Linq;
using System.Data;
using System.Collections;

namespace His.Business.WeiXin
{
    public class Register
    {

        private string errMsg = string.Empty;
        private string resultCode = "190302";
        Manager mgr = new Manager();

        /// <summary>
        /// 提交挂号
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        public string SubmitRegister(His.Models.ZZSB.OutPatientReg opr)
        {
            Manager mgr = new Manager();
            this.resultCode = "190302";
            string returnStr = string.Empty;           
            DateTime now = Shadow.Util.Data.Func.NConvert.ToDateTime(mgr.GetSysDateTime());
            DataSource source = new DataSource();
            source.Return.FunCode = opr.FunCode;
            source.Return.OpTime = now.ToString("yyyy-MM-dd HH:mm:ss") ;

            try
            {
                //这里开始增加事务控制 20161117 alter by  y_ming
                Shadow.Util.Data.Management.Trans.BeginTransaction();

                #region 这里只做数据锁行作用

                //                string lockBook = @"update fin_opr_schema a
                //                    set a.reged=a.reged
                //                    where a.id='{0}'
                //                    and a.valid_flag='1'
                //                    and a.stop<>'1' ";
                //              string lockBook = @"update com_dictionary a
                //                                           set a.name = '0'
                //                                         where upper(a.type) = 'REGLOCK'
                //                                           and a.code = 'ZZSB0001'";
                //              //lockBook = string.Format(lockBook, opr.RegSourceID);

                //              if (mgr.ExecNoQuery(lockBook) != 1)
                //              {
                //                  this.errMsg = "更新号源锁号发生错误！" + mgr.Err;
                //                  return ReturnFailure();
                //              }

                #endregion

                #region 验证挂号患者信息和挂号级别

                //returnStr = this.ValidData(opr);
                //if (!string.IsNullOrEmpty(returnStr))
                //{
                //    return returnStr;
                //}

                #endregion

                #region 获取患者信息，排班信息，挂号登记费用等

                string schemaSql = Sql.Sql.GetSchema;
                string compatientSql = Sql.Sql.GetPatientInfo;
                string regfeeSql = Sql.Sql.GetRegFee;
                string nurQueueSql1 = Sql.Sql.GetNurQueueByDept;
                string nurQueueSql2 = Sql.Sql.GetNurQueueByDoct;
                string invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
                string invoicenoSql2 = Sql.Sql.GetInvoiceR;
                string invoicenoSql3 = Sql.Sql.GetInvoiceUserCode;
                string seenoSql = Sql.Sql.GetSeeNo;
                string clinicCodeSql = Sql.Sql.GetClinicCode;
                string noonSql = Sql.Sql.GetNoonName;
                string intimesSql = Sql.Sql.GetOutPatientInTimes;
                string pactSql = Sql.Sql.GetPactInfo;

                string getnewseeno = Sql.Sql.GetNewSeeNo;
                string updateseeno = Sql.Sql.UpdateSeeNo;

                DataTable dt = new DataTable();

                #region 判断是否有足够号源

                if (!mgr.VaildRegSource(opr.RegSourceID))
                    return Root(ErrSource(source, mgr.Err)).ToString();

                #endregion

                His.Models.ZZSB.ComPatient patient = null;

                #region 获取患者基本信息

                if (Function.GetPatientInfo(opr.CardNo, ref patient, ref errMsg) == 0)
                {
                    return Root(ErrSource(source, errMsg)).ToString();
                }
                if (patient == null)
                {
                    return Root(ErrSource(source, mgr.Err)).ToString();
                }

                #endregion

                #region 获取合同单位

                if (mgr.GetPactInfo(opr, patient) == -1)
                {
                    return Root(ErrSource(source, mgr.Err)).ToString();
                }

                #endregion

                #region 支付方式

                if (Function.SetPayType(opr.PayType, ref  patient, ref errMsg) == 0)
                    return Root(ErrSource(source, errMsg)).ToString();

                #endregion

                #region 获取排班信息

                if (Function.GetSchema(opr.RegSourceID, ref patient, ref errMsg) == -1)
                    return Root(ErrSource(source, errMsg)).ToString();

                #endregion

                #region 14岁以下不能挂急诊内科

                if (patient.Dept.ID == "1026")
                {
                    if (string.IsNullOrEmpty(patient.Birthday))
                    {
                        DateTime dd = DateTime.MinValue;
                        if (DateTime.TryParse(patient.Birthday, out dd))
                            if (dd.AddDays(14 * 365) < DateTime.Now)
                            {
                                errMsg = "14周岁以下不能挂急诊内科！";
                                Root(ErrSource(source, mgr.Err)).ToString();
                            }
                    }
                }

                #endregion

                #region 获取挂号等级费用

                if (mgr.GetRegLevelFee(patient) == -1)
                {
                    return Root(ErrSource(source, mgr.Err)).ToString();
                }

                #endregion

                #region 获取护士分诊队列信息

                dt = new System.Data.DataTable();
                if (patient.SchemaType == "0")
                {

                    nurQueueSql1 = string.Format(nurQueueSql1, patient.SchemaID);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(nurQueueSql1);
                }
                else if (patient.SchemaType == "1")
                {
                    //为医生排班
                    nurQueueSql2 = string.Format(nurQueueSql2, now.ToString("yyyy-MM-dd HH:mm:ss"), patient.Doct.ID, patient.Noon.ID);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(nurQueueSql2);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.NurseCell.ID = dt.Rows[i][0].ToString();
                            patient.Queue.ID = dt.Rows[i][1].ToString();
                            patient.Queue.Name = dt.Rows[i][2].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.Queue.ID) || string.IsNullOrEmpty(patient.NurseCell.ID))
                        {
                            errMsg = "获取分诊队列信息出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        this.errMsg = "没有找到分诊队列信息！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    this.errMsg = "没有找到分诊队列信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }
                #endregion

                #region 获取发票信息

                string realInvoice = string.Empty;
                string invoiceStr = string.Empty;
                dt = new System.Data.DataTable();
                invoicenoSql1 = string.Format(invoicenoSql1, Manager.OPERID, "1");
                dt = DataBaseHelp.DataExecHelp.GetDataTable(invoicenoSql1);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.BeginInvoice = dt.Rows[i][0].ToString();
                            patient.EndInvoice = dt.Rows[i][1].ToString();
                            break;
                        }

                        if (Function.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr, ref errMsg) == -1)
                            return Root(ErrSource(source, errMsg)).ToString();

                        patient.RealInvoice = realInvoice;
                        patient.InvoiceStr = invoiceStr;
                        patient.IsUseingInvoice = true;
                    }
                    else
                    {
                        invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
                        invoicenoSql1 = string.Format(invoicenoSql1, Manager.OPERID, "0");
                        dt = DataBaseHelp.DataExecHelp.GetDataTable(invoicenoSql1);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (!Convert.IsDBNull(dt.Rows[0][0]))
                                {
                                    patient.RealInvoice = dt.Rows[0][0].ToString();
                                    patient.BeginInvoice = dt.Rows[0][0].ToString();
                                    patient.EndInvoice = dt.Rows[0][1].ToString();
                                }
                                if (Function.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr, ref errMsg) == -1)
                                    return Root(ErrSource(source, errMsg)).ToString();
                                patient.InvoiceStr = invoiceStr;
                                patient.IsUseingInvoice = false;
                            }
                            else
                            {
                                this.errMsg = "没有找到发票信息！";
                                return Root(ErrSource(source, errMsg)).ToString();
                            }
                        }
                        else
                        {
                            errMsg = "没有找到发票信息！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    patient.NextRealInvoice = Function.AddNumber(patient.RealInvoice);
                    patient.NextInvoiceStr = Function.AddNumber(patient.InvoiceStr);
                }
                else
                {
                    this.errMsg = "没有找到发票信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 获取门诊流水号

                patient.ClinicCode = mgr.ExecSqlReturnOne(clinicCodeSql);
                if (string.IsNullOrEmpty(patient.ClinicCode))
                {
                    return Root(ErrSource(source, "获取门诊流水号出错！")).ToString();
                    
                }

                #endregion

                #region 获取门诊看诊次数

                dt = new System.Data.DataTable();
                intimesSql = string.Format(intimesSql, patient.CardNo);
                patient.InTimes = Shadow.Util.Data.Func.NConvert.ToInt32(mgr.ExecSqlReturnOne(intimesSql));

                #endregion

                #region 减免费用处理

                if (!string.IsNullOrEmpty(opr.Payinsufeestr))
                {
                    if (Function.DualSIFeeInfo(opr.Payinsufeestr, ref patient, ref errMsg) == 0)
                    {
                        errMsg = "处理诊金减免出错！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }

                #endregion

                #endregion

                #region 更新号源

                string updateLmtSql = string.Format(Sql.Sql.UpdateSchemaReged, opr.RegSourceID, "1");
                int rt = mgr.ExecuteSql(updateLmtSql, ref errMsg);
                if (rt <= 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    if (string.IsNullOrEmpty(errMsg))
                        errMsg = "挂号失败，当前时段号源已被抢完，请选后一时段排班挂号";
                    Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 获取seeNo

                dt = new System.Data.DataTable();
                DataTable dt2 = new DataTable();

                {
                    //为医生排班

                    int minNo = -1, seeNo = 0, cnt = 0, Residue = 0;

                    if (mgr.GetMinSeeNo(patient.SchemaID, ref minNo) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        errMsg = mgr.Err;
                        Root(ErrSource(source, errMsg)).ToString();
                    }


                    if (mgr.GetCurrentSeeNo(patient.SchemaID, ref seeNo) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        errMsg = mgr.Err;
                        return Root(ErrSource(source, errMsg)).ToString();
                    }


                    if (minNo < 1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        errMsg = "取出最小看诊序号不正确，排班ID：" + patient.SchemaID.ToString();
                        Root(ErrSource(source, errMsg)).ToString();
                    }
                    if (mgr.GetSourceCount(patient.SchemaID, ref cnt) != -1)
                    {
                        mgr.GetResidue(patient.SchemaID, ref Residue);
                        if (Residue >= cnt)
                        {
                            Shadow.Util.Data.Management.Trans.RollBack();
                            errMsg = "已经没有足够号源可以，请选择其他时段排班";
                            Root(ErrSource(source, errMsg)).ToString();
                        }
                    }

                    if ((patient.RegLevel.ID != "4") && (seeNo == 0 || seeNo < minNo))
                    {
                        seeNo = minNo;
                    }
                    else
                    {
                        seeNo = seeNo + 1;
                    }
                    patient.SeeNO = seeNo;

                }

                #endregion

                #region 更新排班表，插入号源表
                //插入挂号主表
                string insertReg = Sql.Sql.insertReg;
                //插入挂号费用表 挂号费
                string insertRegFee = Sql.Sql.insertRegFee;
                //插入挂号费用表 诊查费
                string insertDiagFee = Sql.Sql.insertRegFee;
                //插入护士分诊记录表
                string insertAssignRecord = Sql.Sql.insertAssignRecord;
                //更新护士分诊队列表
                string updateNurQueue = Sql.Sql.updateNurQueues;
                //更新com_Dictionary发票信息
                string updatecomDictionarySql = Sql.Sql.updatecomDictionary;
                //更新占用状态
                string updateShemaLockState = Sql.Sql.UpdateRegLockState;
                //跟新看诊序号
                string setseeno = Sql.Sql.SetSeeNo;


                ArrayList sqlList = new ArrayList();
                if (patient.Doct.ID == "None")
                {
                    patient.Doct.ID = string.Empty;
                }

                string[] argm = Function.GetRegInfo(patient);
                string[] regFeeArgm = Function.GetRegFeeInfo(patient);
                string[] diagFeeArgm = Function.GetDiagFeeInfo(patient);// this.GetDiagFeeInfo(patient);
                string[] assignRecordArgm = Function.GetAssignRecordInfo(patient);

                insertReg = string.Format(insertReg, argm);
                insertRegFee = string.Format(insertRegFee, regFeeArgm);
                insertDiagFee = string.Format(insertDiagFee, diagFeeArgm);

                insertAssignRecord = string.Format(insertAssignRecord, assignRecordArgm);
                updateNurQueue = string.Format(updateNurQueue, patient.Queue.ID);
                updateShemaLockState = string.Format(updateShemaLockState, opr.TranSerNo, Manager.OPERID, "3");

                string InsertSISql = string.Empty;//处理诊金减免的sql

                #region 医保减免

                if (opr.Payinsufeestr.Length > 1)
                {
                    if (Function.GetSIRegInfoSql(opr.Payinsufeestr, patient, ref errMsg, ref InsertSISql) == 0)
                        return Root(ErrSource(source, errMsg)).ToString();
                    else
                        sqlList.Add(InsertSISql);
                }

                if (patient.SchemaType == "0")
                {
                    updateseeno = string.Format(updateseeno, now.ToString("yyyy-MM-dd"), patient.Room.ID, "5", patient.Noon.ID);
                    sqlList.Add(updateseeno);
                }
                else if (patient.SchemaType == "1" && patient.RegLevel.ID == "1")
                {
                    updateseeno = string.Format(setseeno, now.ToString("yyyy-MM-dd"), "5", patient.Room.ID, patient.Noon.ID, patient.SeeNO);
                    sqlList.Add(updateseeno);
                }

                #endregion

                sqlList.Add(insertReg);
                sqlList.Add(insertRegFee);
                sqlList.Add(insertDiagFee);
                sqlList.Add(insertAssignRecord);
                //sqlList.Add(updateNurQueue);
                //sqlList.Add(updateShemaLockState);

                #region 处理发票

                if (patient.IsUseingInvoice)
                {
                    //使用在用的发票组
                    if (patient.EndInvoice == patient.NextRealInvoice)
                    {
                        //如果结束发票号=下一张发票号，说明该发票组已经用完了，更新使用标识为-1，并找到下一组发票更新使用标识为1，更新COM_DICTIONARY
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        string updateComInvoiceSql2 = Sql.Sql.updateComInvoiceNew;
                        string starInvoice = string.Empty;
                        string invoiceGetTime = string.Empty;
                        Function.GetUnUseInvoice(ref starInvoice, ref invoiceGetTime);
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Manager.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "-1");
                        //更新新发票组
                        updateComInvoiceSql2 = string.Format(updateComInvoiceSql2, Manager.OPERID, patient.RealInvoice, "1", invoiceGetTime);

                        updatecomDictionarySql = string.Format(updatecomDictionarySql, Manager.OPERID, starInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updateComInvoiceSql2);
                        sqlList.Add(updatecomDictionarySql);
                    }
                    else
                    {
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Manager.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                        updatecomDictionarySql = string.Format(updatecomDictionarySql, Manager.OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updatecomDictionarySql);

                    }
                }
                else
                {
                    string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                    //更新旧发票组
                    updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Manager.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                    updatecomDictionarySql = string.Format(updatecomDictionarySql, Manager.OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                    sqlList.Add(updateComInvoiceSql1);
                    sqlList.Add(updatecomDictionarySql);
                }

                #endregion

                for (int i = 0; i < sqlList.Count; i++)
                {
                    if (mgr.ExecuteSql(sqlList[i].ToString(), ref errMsg) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        His.Util.Common.HisLog.WriteLog("WinXin", "挂号失败，执行sql错误;\n" + sqlList[i].ToString());
                        errMsg = "挂号登记失败！" + errMsg;
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }


                #endregion

                //事务提交
                Shadow.Util.Data.Management.Trans.Commit();

                #region 返回串

                source.Return.Code = "1";
                source.Return.FunCode = opr.FunCode;
                source.Return.OpTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                source.Return.ErrorMsg = "挂号成功!";
                XElement root = this.Root(source);

                XElement result = new XElement("Result",
                    new XElement("TranSerNo", opr.TranSerNo),
                     new XElement("TotalRegFee", (patient.RegFee + patient.PubDigFee + patient.OwnDigFee).ToString("0.00")),
                      new XElement("RegFee", patient.RegFee.ToString("0.00")),
                       new XElement("TreatFee", patient.OwnDigFee.ToString("0.00")),
                          new XElement("TreatFee", "0.00"),
                        new XElement("ServicesFee", "0.00"),
                         new XElement("MetaFee", "0.00"),
                          new XElement("OtherFee", "0.00"),
                          new XElement("MedInsureFee", "0.00"),
                new XElement("PersonalFee", "0.00"),
                new XElement("TreatLocation", ""),
                new XElement("WaitTreatNo", ""),
                new XElement("ReceiptNo", patient.RealInvoice),
                new XElement("SortNo", patient.SeeNO.ToString()),
                new XElement("Note", patient.ClinicCode)
                      );

                root.Add(result);
                
                #endregion

                return root.ToString();

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                Shadow.Util.Data.Management.Trans.RollBack();
                return Root(ErrSource(source, errMsg)).ToString();
            }

        }


        /// <summary>
        /// 预约挂号（提交）
        /// </summary>
        /// <param name="regPaymentRequest"></param>
        /// <returns></returns>
        public string SubmitAppointment(His.Models.ZZSB.OutPatientReg opr)
        {

            string returnStr = string.Empty;
            DateTime now = mgr.GetDateTimeFromSysDateTime();
            DataSource source = new DataSource();

            try
            {
                //这里开始增加事务控制 20161117 alter by  y_ming
                Shadow.Util.Data.Management.Trans.BeginTransaction();

                #region 验证挂号患者信息和挂号级别

             
                #endregion

                #region 锁号已经扣掉号源，这里只做数据锁行作用

                string lockBook = @"update com_dictionary a
                                       set a.name = '0'
                                     where upper(a.type) = 'REGLOCK'
                                       and a.code = 'ZZSB0001' ";

                if (mgr.ExecNoQuery(lockBook) != 1)
                {
                    return Root(ErrSource(source, mgr.Err)).ToString();
                }

                #endregion

                #region 获取患者信息，排班信息，挂号登记费用等

                string schemaSql = Sql.Sql.GetSchema;
                string compatientSql = Sql.Sql.GetPatientInfo;
                string regfeeSql = Sql.Sql.GetRegFee;
                string nurQueueSql1 = Sql.Sql.GetNurQueueByDept;
                string nurQueueSql2 = Sql.Sql.GetNurQueueByDoct;
                string invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
                string invoicenoSql2 = Sql.Sql.GetInvoiceR;
                string invoicenoSql3 = Sql.Sql.GetInvoiceUserCode;
                string seenoSql = Sql.Sql.GetSeeNo;
                string clinicCodeSql = Sql.Sql.GetClinicCode;
                string noonSql = Sql.Sql.GetNoonName;
                string intimesSql = Sql.Sql.GetOutPatientInTimes;
                string pactSql = Sql.Sql.GetPactInfo;

                string getnewseeno = Sql.Sql.GetNewSeeNo;
                string updateseeno = Sql.Sql.UpdateSeeNo;


                #region 获取患者基本信息

                compatientSql = string.Format(compatientSql, opr.CardNo);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(compatientSql);
                His.Models.ZZSB.ComPatient patient = null;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient = new His.Models.ZZSB.ComPatient();
                            patient.CardNo = dt.Rows[i][0].ToString();
                            patient.Name = dt.Rows[i][1].ToString();
                            patient.Birthday = dt.Rows[i][2].ToString();
                            patient.SexCode = dt.Rows[i][3].ToString();
                            patient.IDCard = dt.Rows[i][4].ToString();
                            patient.McardNo = dt.Rows[i][5].ToString();
                            patient.HomePhone = dt.Rows[i][6].ToString();
                            patient.Address = dt.Rows[i][7].ToString();
                            patient.RegDate = now;
                            break;
                        }
                        if (patient == null || string.IsNullOrEmpty(patient.CardNo))
                        {
                            // resultCode = "0";
                            errMsg = "获取患者信息出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.errMsg = "没有找到患者信息！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.errMsg = "没有找到患者信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

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
                pactSql = string.Format(pactSql, opr.FeeType);
                dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(pactSql);
                His.Models.ZZSB.PactInfo pactUnit = null;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            pactUnit = new His.Models.ZZSB.PactInfo();

                            pactUnit.ID = dt.Rows[i][0].ToString();//合同代码          
                            pactUnit.Name = dt.Rows[i][1].ToString();//合同单位名称                    
                            pactUnit.PayKind.ID = dt.Rows[i][2].ToString();//结算类别                    
                            pactUnit.Rate.PubRate = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][3].ToString().Trim());//公费比例                    
                            pactUnit.Rate.PayRate = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][4].ToString().Trim());//自付比例                   
                            pactUnit.Rate.OwnRate = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][5].ToString().Trim()); //自费比例                   
                            pactUnit.Rate.RebateRate = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][6].ToString().Trim()); //优惠比例                    
                            pactUnit.Rate.ArrearageRate = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][7].ToString().Trim());//欠费比例                    
                            pactUnit.Rate.IsBabyShared = Shadow.Util.Data.Func.NConvert.ToBoolean(dt.Rows[i][8].ToString());//婴儿标志 0 无关 1 有关                                
                            pactUnit.IsNeedMCard = Shadow.Util.Data.Func.NConvert.ToBoolean(dt.Rows[i][9].ToString().Trim()); //是否要求必须有医疗证号 0 否 1 是                      
                            pactUnit.IsInControl = Shadow.Util.Data.Func.NConvert.ToBoolean(dt.Rows[i][10].ToString().Trim());//是否受监控 1受监控0不受监控                   
                            pactUnit.ItemType = dt.Rows[i][11].ToString().Trim(); //标志  0 全部 1 药品 2 非药品   
                            pactUnit.DayQuota = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][12].ToString().Trim());//日限额                     
                            pactUnit.MonthQuota = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][13].ToString().Trim()); //月限额                    
                            pactUnit.YearQuota = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][14].ToString().Trim());//年限额
                            pactUnit.OnceQuota = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][15].ToString().Trim());//一次限
                            string PriceForm = dt.Rows[i][16].ToString();
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

                            pactUnit.BedQuota = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][17].ToString());//床位限额
                            pactUnit.AirConditionQuota = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][18].ToString());//空调限额
                            pactUnit.SortID = Shadow.Util.Data.Func.NConvert.ToInt32(dt.Rows[i][19]);//序号             
                            pactUnit.ShortName = dt.Rows[i][20].ToString();//合同单位简称
                            pactUnit.PactDllName = dt.Rows[i][21].ToString(); //待遇dll名称
                            pactUnit.PactDllDescription = dt.Rows[i][22].ToString();//待遇dll说明
                            pactUnit.PactSystemType = dt.Rows[i][23].ToString().Trim();

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
                            pactUnit.SpellCode = dt.Rows[i][24].ToString();//拼音码
                            pactUnit.WBCode = dt.Rows[i][25].ToString();//五笔码
                            pactUnit.PatientType.ID = dt.Rows[i][26].ToString();//人员类型编码
                            pactUnit.PatientType.Name = dt.Rows[i][27].ToString();//人员类型名称
                            pactUnit.IsUseInOutPatientFee = Shadow.Util.Data.Func.NConvert.ToBoolean(dt.Rows[i][28].ToString().Trim());

                            break;
                        }
                        if (pactUnit == null || string.IsNullOrEmpty(pactUnit.ID))
                        {
                            // resultCode = "0";
                            errMsg = "获取合同单位信息出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.errMsg = "没有找到合同单位信息！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.errMsg = "没有找到合同单位信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }
                patient.Pact = pactUnit;
                #endregion

                #region 支付方式

                if (Function.SetPayType(opr.PayType, ref  patient, ref errMsg) == 0)
                {
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 获取排班信息

                schemaSql = string.Format(schemaSql, opr.RegSourceID);
                dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(schemaSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.SchemaID = dt.Rows[i][0].ToString();
                            patient.SchemaType = dt.Rows[i][1].ToString();//排班类型，0科室/1医生
                            patient.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][2].ToString());
                            patient.Noon.ID = dt.Rows[i][4].ToString();
                            patient.Dept.ID = dt.Rows[i][5].ToString();
                            patient.Dept.Name = dt.Rows[i][6].ToString();
                            patient.Doct.ID = dt.Rows[i][7].ToString();
                            patient.Doct.Name = dt.Rows[i][8].ToString();
                            patient.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][20].ToString());
                            patient.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][21].ToString());
                            patient.RegLevel.ID = dt.Rows[i][29].ToString();
                            patient.RegLevel.Name = dt.Rows[i][30].ToString();
                            patient.Room.ID = dt.Rows[i][31].ToString();
                            patient.Room.Name = dt.Rows[i][32].ToString();
                            patient.Console.ID = dt.Rows[i][33].ToString();
                            patient.Console.Name = dt.Rows[i][34].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.SchemaID))
                        {
                            // resultCode = "0";
                            errMsg = "获取排班信息出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.errMsg = "没有找到排班信息！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.errMsg = "没有找到排班信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 获取挂号等级费用

                regfeeSql = string.Format(regfeeSql, "1", patient.RegLevel.ID);
                dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(regfeeSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4]);//挂号费
                            patient.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6]);//诊查费
                            break;
                        }
                        if (patient.RegFee<=0)
                        {
                            errMsg = "获取费用信息出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        this.errMsg = "没有找到费用信息！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    this.errMsg = "没有找到费用信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 获取护士分诊队列信息
                dt = new System.Data.DataTable();
                if (patient.SchemaType == "0")
                {
                    //为科室排班
                    nurQueueSql1 = string.Format(nurQueueSql1, patient.Begin.ToShortDateString(), patient.Dept.ID, patient.Noon.ID, patient.Room.ID);
                    nurQueueSql1 = string.Format(nurQueueSql1, patient.SchemaID);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(nurQueueSql1);
                }
                else if (patient.SchemaType == "1")
                {
                    //为医生排班
                    nurQueueSql2 = string.Format(nurQueueSql2, patient.Begin.ToShortDateString(), patient.Doct.ID, patient.Noon.ID);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(nurQueueSql2);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.NurseCell.ID = dt.Rows[i][0].ToString();
                            patient.Queue.ID = dt.Rows[i][1].ToString();
                            patient.Queue.Name = dt.Rows[i][2].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.Queue.ID) || string.IsNullOrEmpty(patient.NurseCell.ID))
                        {
                            //resultCode = "0";
                            errMsg = "获取分诊队列信息出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        //this.resultCode = "0";
                        this.errMsg = "没有找到分诊队列信息！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.errMsg = "没有找到分诊队列信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }
                #endregion

                #region 获取发票信息
                string realInvoice = string.Empty;
                string invoiceStr = string.Empty;
                dt = new System.Data.DataTable();
                invoicenoSql1 = string.Format(invoicenoSql1, Manager.OPERID, "1");
                dt = DataBaseHelp.DataExecHelp.GetDataTable(invoicenoSql1);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.BeginInvoice = dt.Rows[i][0].ToString();
                            patient.EndInvoice = dt.Rows[i][1].ToString();
                            break;
                        }
                        if (Function.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr, ref errMsg) == -1)
                            return Root(ErrSource(source, errMsg)).ToString();

                        patient.RealInvoice = realInvoice;
                        patient.InvoiceStr = invoiceStr;
                        patient.IsUseingInvoice = true;
                    }
                    else
                    {
                        invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
                        invoicenoSql1 = string.Format(invoicenoSql1, Manager.OPERID, "0");
                        dt = DataBaseHelp.DataExecHelp.GetDataTable(invoicenoSql1);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (!Convert.IsDBNull(dt.Rows[0][0]))
                                {
                                    patient.RealInvoice = dt.Rows[0][0].ToString();
                                    patient.BeginInvoice = dt.Rows[0][0].ToString();
                                    patient.EndInvoice = dt.Rows[0][1].ToString();
                                }
                                if (Function.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr, ref errMsg) == -1)
                                    return Root(ErrSource(source, errMsg)).ToString();

                                patient.InvoiceStr = invoiceStr;
                                patient.IsUseingInvoice = false;
                            }
                            else
                            {
                                //this.resultCode = "0";
                                this.errMsg = "没有找到发票信息！";
                                return Root(ErrSource(source, errMsg)).ToString();
                            }
                        }
                        else
                        {
                            // this.resultCode = "0";
                            this.errMsg = "没有找到发票信息！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    patient.NextRealInvoice = Function.AddNumber(patient.RealInvoice);
                    patient.NextInvoiceStr = Function.AddNumber(patient.InvoiceStr);
                }
                else
                {
                    //this.resultCode = "0";
                    this.errMsg = "没有找到发票信息！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 获取门诊流水号

                dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(clinicCodeSql);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (!Convert.IsDBNull(dt.Rows[0][0]))
                        {
                            patient.ClinicCode = dt.Rows[0][0].ToString();
                        }
                        else
                        {
                            //this.resultCode = "0";
                            this.errMsg = "获取门诊流水号出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.errMsg = "没有找到门诊流水号！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.errMsg = "没有找到门诊流水号！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 获取门诊看诊次数

                dt = new System.Data.DataTable();
                intimesSql = string.Format(intimesSql, patient.CardNo);
                dt = DataBaseHelp.DataExecHelp.GetDataTable(intimesSql);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (!Convert.IsDBNull(dt.Rows[0][0]))
                        {
                            patient.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                        }
                        else
                        {
                            //  this.resultCode = "0";
                            this.errMsg = "获取门诊看诊次数出错！";
                            return Root(ErrSource(source, errMsg)).ToString();
                        }
                    }
                    else
                    {
                        //  this.resultCode = "0";
                        this.errMsg = "没有找到门诊看诊次数！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.errMsg = "没有找到门诊看诊次数！";
                    return Root(ErrSource(source, errMsg)).ToString();
                }

                #endregion

                #region 减免费用处理

                if (!string.IsNullOrEmpty(opr.Payinsufeestr))
                {
                    if (Function.DualSIFeeInfo(opr.Payinsufeestr, ref patient, ref errMsg) == 0)
                    {
                        this.errMsg = "处理诊金减免出错！";
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }


                #endregion

                #region 判断是否有足够号源
                int regRemainCount = 0;


                string sql = @"select t.id,(t.reg_lmt - t.tel_reging) regRemain
                               from fin_opr_schema t
                               where t.id = '{0}'";
                sql = string.Format(sql, opr.RegSourceID);

                dt = new System.Data.DataTable();
                //排班表
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            regRemainCount = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[i][1].ToString());
                        }
                    }
                    else
                    {
                        regRemainCount = -1;
                    }
                }
                else
                {
                    regRemainCount = -1;
                }
                #endregion

                #endregion

                #region 更新号源

                //string updateLmtSql = Sql.Sql.UpdateSchemaReged;
                //if (regRemainCount != -1)
                //{
                //    if (regRemainCount > 0)
                //    {
                //        updateLmtSql = string.Format(updateLmtSql, opr.RegSourceID, "1");
                //        if (mgr.ExecuteSql(updateLmtSql, ref errMsg) == -1)
                //        {
                //            //Shadow.Util.Data.Management.Trans.RollBack();
                //            return Root(ErrSource(source, errMsg)).ToString();
                //        }
                //    }

                //    else
                //    {
                //        //Shadow.Util.Data.Management.Trans.RollBack();
                //        this.errMsg = "没有可用的号源，请选择其他时段的排班！";
                //        return Root(ErrSource(source, errMsg)).ToString();

                //    }
                //}

                #endregion

                #region 更新排班表，插入号源表

                //插入挂号主表
                string insertReg = Sql.Sql.insertReg;
                //插入挂号费用表 挂号费
                string insertRegFee = Sql.Sql.insertRegFee;
                //插入挂号费用表 诊查费
                string insertDiagFee = Sql.Sql.insertRegFee;
                //插入护士分诊记录表
                string insertAssignRecord = Sql.Sql.insertAssignRecord;
                //更新护士分诊队列表
                string updateNurQueue = Sql.Sql.updateNurQueues;
                //更新com_Dictionary发票信息
                string updatecomDictionarySql = Sql.Sql.updatecomDictionary;
                //更新占用状态
                string updateShemaLockState = Sql.Sql.UpdateRegLockState;
                //跟新看诊序号
                string setseeno = Sql.Sql.SetSeeNo;



                ArrayList sqlList = new ArrayList();

                string[] argm = Function.GetRegInfo(patient);
                string[] regFeeArgm = Function.GetRegFeeInfo(patient);
                string[] diagFeeArgm = Function.GetDiagFeeInfo(patient);// this.GetDiagFeeInfo(patient);
                string[] assignRecordArgm = Function.GetAssignRecordInfo(patient);

                insertReg = string.Format(insertReg, argm);
                insertRegFee = string.Format(insertRegFee, regFeeArgm);
                insertDiagFee = string.Format(insertDiagFee, diagFeeArgm);


                insertAssignRecord = string.Format(insertAssignRecord, assignRecordArgm);
                updateNurQueue = string.Format(updateNurQueue, patient.Queue.ID);
                updateShemaLockState = string.Format(updateShemaLockState, opr.TranSerNo, Manager.OPERID, "3");

                string InsertSISql = string.Empty;//处理诊金减免的sql

                #region 更新预约主表
                //            string updatBooking = string.Format(@"
                //                update fin_opr_booking t
                //                set t.confirm_opcd='{0}',
                //                t.oper_code='{0}',
                //                t.confirm_date=sysdate,
                //                t.card_no=(select r.patientid from fin_opr_bookingreg r where r.regno='{1}' and rownum=1),
                //                t.see_flag='1',
                //                t.source=decode('{3}','2','2','21','2','32','3','2')
                //                where t.reg_id='{2}'
                //                and t.valid_flag='1'",
                //            operCode,
                //            clinic_code,
                //            regPaymentRequest.RegNo,
                //            regPaymentRequest.PaymentWay);

                //            sqlList.Add(updatBooking);
                #endregion

                #region 插入预约主表

                string insertFinOprBooking = string.Format(Sql.Sql.BookInsertSql,
                opr.RegSourceID,//0
                opr.CardNo,//1
                mgr.GetBookSerialNo().ToString(),//2
                patient.Birthday,//3
                Manager.OPERID,//4
                patient.ClinicCode, //5
                "4", // 预约类别 4 ：自助机。
                Manager.OPERID,
                "sysdate"
                );

                sqlList.Add(insertFinOprBooking);

                #endregion

                #region 医保减免

                if (opr.Payinsufeestr.Length > 1)
                {
                    if (Function.GetSIRegInfoSql(opr.Payinsufeestr, patient, ref errMsg, ref InsertSISql) == 0)
                    {
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                    else
                        sqlList.Add(InsertSISql);
                }

                if (patient.SchemaType == "0")
                {
                    updateseeno = string.Format(updateseeno, now.ToString("yyyy-MM-dd"), patient.Room.ID, "5", patient.Noon.ID);
                    sqlList.Add(updateseeno);
                }
                else if (patient.SchemaType == "1" && patient.RegLevel.ID == "1")
                {
                    updateseeno = string.Format(setseeno, now.ToString("yyyy-MM-dd"), "5", patient.Room.ID, patient.Noon.ID, patient.SeeNO);
                    sqlList.Add(updateseeno);
                }

                #endregion

                sqlList.Add(insertReg);
                sqlList.Add(insertRegFee);
                sqlList.Add(insertDiagFee);
                //sqlList.Add(insertAssignRecord);
                // sqlList.Add(updateNurQueue);
                //sqlList.Add(updateShemaLockState);

                #region 处理发票

                if (patient.IsUseingInvoice)
                {
                    //使用在用的发票组
                    if (patient.EndInvoice == patient.NextRealInvoice)
                    {
                        //如果结束发票号=下一张发票号，说明该发票组已经用完了，更新使用标识为-1，并找到下一组发票更新使用标识为1，更新COM_DICTIONARY
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        string updateComInvoiceSql2 = Sql.Sql.updateComInvoiceNew;
                        string starInvoice = string.Empty;
                        string invoiceGetTime = string.Empty;
                        Function.GetUnUseInvoice(ref starInvoice, ref invoiceGetTime);
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Manager.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "-1");
                        //更新新发票组
                        updateComInvoiceSql2 = string.Format(updateComInvoiceSql2, Manager.OPERID, patient.RealInvoice, "1", invoiceGetTime);

                        updatecomDictionarySql = string.Format(updatecomDictionarySql, Manager.OPERID, starInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updateComInvoiceSql2);
                        sqlList.Add(updatecomDictionarySql);
                    }
                    else
                    {
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Manager.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                        updatecomDictionarySql = string.Format(updatecomDictionarySql, Manager.OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updatecomDictionarySql);

                    }
                }
                else
                {
                    string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                    //更新旧发票组
                    updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Manager.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                    updatecomDictionarySql = string.Format(updatecomDictionarySql, Manager.OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                    sqlList.Add(updateComInvoiceSql1);
                    sqlList.Add(updatecomDictionarySql);
                }

                #endregion

                for (int i = 0; i < sqlList.Count; i++)
                {
                    if (mgr.ExecuteSql(sqlList[i].ToString(), ref errMsg) == -1)
                    {
                        His.Util.Common.HisLog.WriteLog("ZZSB", "挂号失败，执行sql错误;\n" +
                            sqlList[i].ToString());
                        errMsg = "挂号登记失败！" + errMsg;
                        return Root(ErrSource(source, errMsg)).ToString();
                    }
                }


                His.Util.Common.HisLog.WriteLog("ZZSB", insertDiagFee + "**********" + insertRegFee);


                #region 返回串

                source.Return.Code = "1";
                source.Return.FunCode = opr.FunCode;
                source.Return.OpTime = DateTime.Now.ToString("YYYY-MM-DD HH:mm:ss");
                source.Return.ErrorMsg = "挂号成功!";
                XElement root = this.Root(source);

                XElement result = new XElement("Result",
                    new XElement("TranSerNo", opr.TranSerNo),
                     new XElement("TotalRegFee", (patient.RegFee + patient.PubDigFee + patient.OwnDigFee).ToString("0.00")),
                      new XElement("RegFee", patient.RegFee.ToString("0.00")),
                       new XElement("TreatFee", patient.OwnDigFee.ToString("0.00")),
                          new XElement("TreatFee", "0.00"),
                        new XElement("ServicesFee", "0.00"),
                         new XElement("MetaFee", "0.00"),
                          new XElement("OtherFee", "0.00"),
                          new XElement("MedInsureFee", "0.00"),
                new XElement("PersonalFee", "0.00"),
                new XElement("TreatLocation", ""),
                new XElement("WaitTreatNo", ""),
                new XElement("ReceiptNo", patient.RealInvoice),
                new XElement("SortNo", patient.SeeNO.ToString()),
                new XElement("Note", patient.ClinicCode)
                      );

                root.Add(result);

                #endregion

                returnStr= root.ToString();

                #endregion
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return Root(ErrSource(source, errMsg)).ToString();
            }

            Shadow.Util.Data.Management.Trans.Commit();
            return returnStr;

        }

        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="source"></param>
        /// <param name="errerrMsg"></param>
        /// <returns></returns>
        private DataSource ErrSource(DataSource source, string errerrMsg)
        {
            Shadow.Util.Data.Management.Trans.RollBack();
            source.Return.Code = "190302";
            source.Return.ErrorMsg = " ErrMsg: " + errerrMsg;
            Shadow.Util.Data.Func.Log.WriteLog("Err", source.Return.ErrorMsg);
            return source;
        }

        /// <summary>
        /// 生成xml根
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private XElement Root(DataSource source)
        {
            XElement root = new XElement("DataSource",
                new XElement("Return",
                    new XElement("Code", source.Return.Code),
                    new XElement("ErrorMsg", source.Return.ErrorMsg),
                    new XElement("FunCode", source.Return.FunCode),
                    new XElement("OpTime", source.Return.OpTime)));
            return root;
        }

    }
}
