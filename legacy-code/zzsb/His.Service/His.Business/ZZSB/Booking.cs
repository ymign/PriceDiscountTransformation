using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Models.ZZSB;
using System.Data;
using System.Collections;
using System.Xml.Linq;

namespace His.Business.ZZSB
{
    public class Booking
    {

        /// <summary>
        /// 构造器
        /// </summary>
        public Booking()
        {
            if (regMgr == null)
            {
                regMgr = new RegisterManager();
            }
        }

        #region 变量/属性

        RegisterManager regMgr = new RegisterManager();
        string OPERID = RegisterManager.OPERID;
        DataSource source = new DataSource();
        string msg = string.Empty;

        #endregion



        /// <summary>
        /// 取患者预约信息
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public DataSource GetBookingInfo(BookReq reqInfo)
        {

            DataSource source = new DataSource();
            if (reqInfo != null)
            {
                if (string.IsNullOrEmpty(reqInfo.CardNo))
                {
                    source.Return.ErrorMsg = "卡号不能为空！";
                    source.Return.Code = "0";
                    return source;
                }
                if (string.IsNullOrEmpty(reqInfo.DeviceID) || string.IsNullOrEmpty(reqInfo.ServiceCode))// || string.IsNullOrEmpty(reqInfo.PatientID)
                {
                    source.Return.ErrorMsg = "服务编码，病人ID，设备编号不能为空！";
                    source.Return.Code = "0";
                    return source;
                }
            }
            else
            {
                source.Return.ErrorMsg = "请输入有效请求参数！";
                source.Return.Code = "0";
                return source;
            }


            ComPatient patient = new ComPatient();
            DateTime now = Function.GetSysDate();
            try
            {
                //if (reqInfo.CardTypeCode == "2")
                //{
                //    if (Function.GetPatientInfoByCar(reqInfo.CardNo, ref patient, ref msg) == 0)
                //    {
                //        source.Return.Code = "0";
                //        source.Return.ErrorMsg = msg;
                //        return source;
                //    }
                //}
                //else
                //{
                //    if (Function.GetPatientInfo(reqInfo.CardNo, ref patient, ref msg) == 0)
                //    {
                //        source.Return.Code = "0";
                //        source.Return.ErrorMsg = msg;
                //        return source;
                //    }
                //}20180615  注释 by zhao
                Result obj = null;


                string sql = @"select a.schema_no,
                               a.doct_code,
                               a.doct_name,
                               a.dept_code,
                               a.dept_name,
                               trunc(a.booking_date) booking_date,
                               a.REGLEVL_CODE,
                               a.begin_time,
                               nvl(b.room_name, ''),
                               (select r.invoice_no
                                  from fin_opr_register r
                                 where a.reg_id = r.clinic_code
                                   and r.trans_type = '1'
                                   and rownum = 1) fee
                          from fin_opr_booking a
                          join fin_opr_schema b
                            on a.schema_no = b.id
                         where (a.card_no = '{0}' or a.idenno = '{1}')
                           and trunc(a.booking_date) = trunc(sysdate)
                           and a.see_flag = '0'
                           and a.valid_flag = '1'
                           --and b.stop = '0'
                           and b.valid_flag = '1'
                        ";
                // sql = string.Format(sql, patient.CardNo, patient.IDCard, patient.McardNo);
                sql = string.Format(sql, reqInfo.CardNo, reqInfo.CardNo);

                DataTable dt = new DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                string lelvel = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    string waitSql = @"select a.name from com_dictionary a
                                    where a.type='WaitTime' and code='1'";
                    int waitTime = 0;
                    DataTable dt2 = DataBaseHelp.DataExecHelp.GetDataTable(waitSql);
                    if (dt2.Rows.Count > 0)
                    {
                        int.TryParse(dt2.Rows[0][0].ToString(), out waitTime);
                    }

                    int cnt = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        cnt++;
                        obj = new Result();
                        int i = 0;
                        obj.ordercode = row[i].ToString();
                        i++;
                        obj.DoctorCode = row[i].ToString();
                        i++;
                        obj.doctorName = row[i].ToString();
                        i++;
                        obj.DeptCode = row[i].ToString();
                        i++;

                        obj.DeptName = row[i].ToString();
                        i++;
                        obj.VistDate = row[i].ToString();
                        i++;
                        patient.RegLevel.ID = row[i].ToString();
                        i++;
                        DateTime begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[i].ToString());
                        if (begin != DateTime.MinValue)
                        {
                            if (now > begin.AddMinutes(-waitTime))
                            {
                                if (cnt < dt.Rows.Count)
                                    continue;
                                else
                                {
                                    source.Return.Code = "0";
                                    source.Return.ErrorMsg = "取号时间已过,请前往窗口咨询！";
                                    return source;
                                }
                            }
                        }
                        i++;
                        obj.AdmitAddress = row[i].ToString();
                        i++;
                        string isPay = row[i].ToString();
                        if (!string.IsNullOrEmpty(isPay))
                        {
                            if (cnt < dt.Rows.Count)
                            {
                                continue;
                            }
                            else
                            {
                                msg = "您的预约已缴费，无需取号，可直接前往诊室看诊";
                                return ErrSource(source, msg);
                            }
                        }

                        if (Function.GetRegFee(ref patient, ref msg) == 0)
                        {
                            if (cnt < dt.Rows.Count)
                                continue;
                            else
                                return ErrSource(source, msg);
                        }
                        obj.TotalRegFee = patient.RegFee + patient.OwnDigFee;
                        source.Return.Results.Add(obj);
                    }
                }
                else
                {
                    // return ErrSource(source, "没有相关预约记录！");
                    source.Return.Code = "0";
                    source.Return.ErrorMsg = "没有查到您的预约信息，请到13号预约窗口咨询。预约已交费成功的直接到诊室就诊，无需取号！";
                    return source;
                }
                source.Return.Code = "1";
                source.Return.FunCode = string.Empty;
                source.Return.OpTime = Function.GetSysDate().ToString("yyyy-MM-dd HH:mm:ss");
                return source;
            }
            catch (Exception ex)
            {
                source.Return.ErrorMsg = ex.Message;
                source.Return.Code = "0";
                return source;
            }
        }

        /// <summary>
        /// 取预约信息
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public BooKInFoList GetBookingInfoList(BookReq reqInfo)
        {

            BooKInFoList booklist = new BooKInFoList();
            if (reqInfo != null)
            {
                if (string.IsNullOrEmpty(reqInfo.CardNo))
                {
                    booklist.ErrorMsg = "卡号不能为空！";
                    booklist.Code = "0";
                    return booklist;
                }
                if (string.IsNullOrEmpty(reqInfo.DeviceID) || string.IsNullOrEmpty(reqInfo.ServiceCode))// || string.IsNullOrEmpty(reqInfo.PatientID)
                {
                    booklist.ErrorMsg = "服务编码，病人ID，设备编号不能为空！";
                    booklist.Code = "0";
                    return booklist;
                }
            }
            else
            {
                booklist.ErrorMsg = "请输入有效请求参数！";
                booklist.Code = "0";
                return booklist;
            }


            ComPatient patient = new ComPatient();
            DateTime now = Function.GetSysDate();
            try
            {
                if (reqInfo.CardTypeCode == "2")
                {
                    if (Function.GetPatientInfoByCar(reqInfo.CardNo, ref patient, ref msg) == 0)
                    {
                        booklist.Code = "0";
                        booklist.ErrorMsg = msg;
                        return booklist;
                    }
                }
                else
                {
                    if (Function.GetPatientInfo(reqInfo.CardNo, ref patient, ref msg) == 0)
                    {
                        booklist.Code = "0";
                        booklist.ErrorMsg = msg;
                        return booklist;
                    }
                }

                string sql = @"select a.schema_no, a.doct_code,a.doct_name,a.dept_code,a.dept_name ,trunc(a.booking_date)booking_date 
                            ,a.REGLEVL_CODE,a.begin_time ,nvl(b.room_name,'') 
                           from   fin_opr_booking a join fin_opr_schema b on a.schema_no=b.id
                        where (a.card_no='{0}' or (a.idenno='{1}' or a.idenno='{2}'))
                        and  trunc(a.booking_date)=trunc(to_date('{3}','yyyy-mm-dd'))  
                        and a.see_flag='0'
                        and a.valid_flag='1'
                        and b.stop='0' and b.valid_flag='1'";
                sql = string.Format(sql, patient.CardNo, patient.IDCard, patient.McardNo, reqInfo.RegDate);
                DataTable dt = new DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                string lelvel = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    string waitSql = @"select a.name from com_dictionary a
                                    where a.type='WaitTime' and code='1'";
                    int waitTime = 0;
                    DataTable dt2 = DataBaseHelp.DataExecHelp.GetDataTable(waitSql);
                    if (dt2.Rows.Count > 0)
                    {
                        int.TryParse(dt2.Rows[0][0].ToString(), out waitTime);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        His.Models.ZZSB.BooKBaseInFo bookbase = new BooKBaseInFo();
                        bookbase.TranSerNo = reqInfo.ReqTraceNo;
                        int i = 0;
                        bookbase.TranNum = row[i].ToString();
                        i++;
                        bookbase.DoctorCode = row[i].ToString();
                        i++;
                        bookbase.DoctorName = row[i].ToString();
                        i++;
                        bookbase.DeptCode = row[i].ToString();
                        i++;
                        bookbase.DeptName = row[i].ToString();
                        i++;
                        bookbase.OrderDate = row[i].ToString();
                        i++;
                        patient.RegLevel.ID = row[i].ToString();
                        i++;
                        DateTime begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[i].ToString());
                        if (begin != DateTime.MinValue)
                        {
                            if (now > begin.AddMinutes(-waitTime))
                            {
                                //continue;
                                booklist.Code = "0";
                                booklist.ErrorMsg = "看诊时间已过！";
                                return booklist;
                            }
                        }
                        //i++;
                        //obj.AdmitAddress = row[i].ToString();

                        if (Function.GetRegFee(ref patient, ref msg) == 0)
                        {
                            booklist.Code = "0";
                            booklist.ErrorMsg = msg;
                            //return ErrSource(source, msg);
                            return booklist;
                        }
                        bookbase.TotalRegFee = (patient.RegFee + patient.OwnDigFee).ToString();
                        bookbase.PatientName = patient.Name;
                        booklist.BaseInfoList.Add(bookbase);
                    }
                }
                else
                {
                    // return ErrSource(source, "没有相关预约记录！");
                    booklist.Code = "0";
                    booklist.ErrorMsg = "没有找到相关预约记录，可能医生已经停诊，可前往门诊大厅咨询！";
                    return booklist;
                }
                booklist.Code = "1";
                booklist.FunCode = reqInfo.FunCode;
                booklist.OpTime = Function.GetSysDate().ToString("yyyy-MM-dd HH:mm:ss");
                return booklist;
            }
            catch (Exception ex)
            {
                booklist.ErrorMsg = ex.Message;
                booklist.Code = "0";
                return booklist;
            }
        }

        /// <summary>
        /// 预约取号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DataSource SubmitBooking(His.Models.ZZSB.SubmitBookingReq info)
        {
            DataSource source = new DataSource();
            string returnStr = string.Empty;
            RegisterManager mgr = new RegisterManager();
            His.Models.ZZSB.ComPatient patient = new ComPatient();

            DateTime now = Function.GetSysDate();

            if (!string.IsNullOrEmpty(info.PayType))
            {
                patient.PayType = info.PayType;
            }


            #region 验证挂号患者信息和挂号级别


            //returnStr = this.ValidData(opr);
            //if (!string.IsNullOrEmpty(returnStr))
            //{
            //    return returnStr;
            //}

            #endregion
            //  #region 获取患者信息，排班信息，挂号登记费用等

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


            try
            {
                //这里开始增加事务控制 20161117 alter by  y_ming
                Shadow.Util.Data.Management.Trans.BeginTransaction();

                #region lock

                string lockBook = @"update com_dictionary a
                                       set a.name = '0'
                                     where upper(a.type) = 'REGLOCK'
                                       and a.code = 'ZZSB0001' ";
                //lockBook = string.Format(lockBook, opr.RegSourceID);

                if (mgr.ExecNoQuery(lockBook) != 1)
                {
                    this.msg = "更新号源锁号发生错误！" + mgr.Err;
                    return ErrSource(source, msg);
                }


                #endregion

                #region 获取患者基本信息

                if (Function.GetPatientInfo(info.CardNo, ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);
                }


                #endregion

                #region 获取合同单位信息
                if (Function.GetPactInfo(info.FeeType, ref patient, ref msg) < 0)
                {
                    return ErrSource(source, msg);
                }
                #endregion

                #region 获取排班信息

                if (Function.GetSchema(info.ordercode, ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);
                }
                patient.RegDate = patient.Begin;
                // patient.Book.IsBook = true;
                #endregion

                #region 取预约流水号
                His.Models.ZZSB.BookInfo book = null;
                if (info.IsBook)
                {
                    if (Function.GetBookInfo(patient, ref book, ref msg) == 0)
                        return ErrSource(source, msg);
                    if (book != null)
                        patient.Book = book; book.IsBook = info.IsBook;
                }

                #endregion

                #region 支付方式

                if (Function.SetPayType(info.PayType, ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);
                }

                #endregion

                #region 获取挂号等级费用
                if (Function.GetRegFee(ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);
                }


                #endregion

                #region 获取护士分诊队列信息
                if (Function.GetQueue(ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);
                }
                #endregion

                #region 获取发票信息
                if (Function.GetInvoice(ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);

                }
                #endregion

                #region 获取门诊流水号

                if (Function.GetClinicCode(ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);
                }
                #endregion

                #region 获取门诊看诊次数

                if (Function.GetInTimes(ref patient, ref msg) == 0)
                {
                    return ErrSource(source, msg);
                }

                #endregion

                #region 减免费用处理
                if (!string.IsNullOrEmpty(info.Payinsufeestr))
                {
                    if (Function.DualSIFeeInfo(info.Payinsufeestr, ref patient, ref msg) == 0)
                    {
                        return ErrSource(source, msg);

                    }
                }


                #endregion

                #region 更新排班的已挂人数

                string UpdateSchemaSql = string.Empty;
                if (info.IsBook)
                {
                    UpdateSchemaSql = @"UPDATE fin_opr_schema a
                        set a.tel_reged=a.tel_reged+'{0}',
                        a.spe_reged=a.spe_reged+'{1}'
                        where a.id='{2}' ";
                    int spe = 0, tel = 0;
                    if (book.Source == "0")
                        spe = 1;
                    else
                        tel = 1;
                    UpdateSchemaSql = string.Format(UpdateSchemaSql, tel, spe, patient.SchemaID.ToString());
                    if (mgr.ExecuteSql(UpdateSchemaSql, ref msg) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        return ErrSource(source, msg);
                    }
                }

                #endregion

                #region 获取seeNo

                int minNo = -1, seeNo = -1;

                if (mgr.GetMinSeeNo(patient.SchemaID, ref minNo) == -1)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    msg = mgr.Err;
                    return ErrSource(source, msg); ;
                }
                if (mgr.GetCurrentSeeNo(patient.SchemaID, ref seeNo) == -1)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    msg = "取当前看诊序号不正确，排班ID：" + patient.SchemaID;
                    return ErrSource(source, msg);
                }

                if (minNo < 1 || seeNo < 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    msg = "取看诊序号不正确，排班ID：" + patient.SchemaID;
                    return ErrSource(source, msg);
                }

                if (seeNo == 0 || seeNo < minNo)
                {
                    seeNo = minNo;
                }
                else
                {
                    seeNo = seeNo + 1;
                }
                patient.SeeNO = seeNo;

                //if (Function.GetSeeNo(info.IsBook, ref patient, ref msg) == 0)
                //{
                //    return ErrSource(source, msg);
                //}

                #endregion

                #region 更新排班表，插入号源表

                #region sql

                //插入挂号主表
                string insertReg = Sql.Sql.insertReg;
                //插入挂号费用表 挂号费
                string insertRegFee = Sql.Sql.insertRegFee;
                //插入挂号费用表 诊查费
                string insertDiagFee = Sql.Sql.insertRegFee;
                //插入护士分诊记录表
                string insertAssignRecord = Sql.Sql.insertAssignRecord;
                //插入交易记录表
                string InsertTradeRecords = Sql.Sql.InsertTradeRecords;
                //更新护士分诊队列表
                string updateNurQueue = Sql.Sql.updateNurQueues;
                //更新com_Dictionary发票信息
                string updatecomDictionarySql = Sql.Sql.updatecomDictionary;
                //更新占用状态
                string updateShemaLockState = Sql.Sql.UpdateRegLockState;
                //更新看诊序号
                string setseeno = Sql.Sql.SetSeeNo;
                string updateseeno = Sql.Sql.UpdateSeeNo;
                //
                string updateBookingInfo = string.Empty;
                ArrayList sqlList = new ArrayList();

                #region 获取交易记录信息
                Models.ZZSB.TradeRecords recordsInfo = new His.Models.ZZSB.TradeRecords();
                recordsInfo.TranserNo = info.ReqTraceNo;//交易流水号
                recordsInfo.INVOICE_NO = patient.InvoiceStr;//发票号
                recordsInfo.CLINIC_NO = patient.ClinicCode;//
                recordsInfo.CARDNO = patient.CardNo;//卡号
                recordsInfo.NAME = patient.Name;//姓名
                recordsInfo.ORDERID = info.BankCardNo;//订单号或者银行卡卡号
                recordsInfo.PAY_TYPE = patient.PayType;//支付方式
                recordsInfo.TYPE = "3";//交易类型
                recordsInfo.TOT_COST = info.PayAmt;//交易金额
                recordsInfo.DEVICEID = info.DeviceID;//设备编号
                recordsInfo.REMARK = patient.SeeNO.ToString();//备注,挂号插入的是看诊序号
                recordsInfo.PACTCODE = patient.Pact.ID;//合同单位
                #endregion


                string[] argm = Function.GetRegInfo(patient);
                string[] regFeeArgm = Function.GetRegFeeInfo(patient);
                string[] diagFeeArgm = Function.GetDiagFeeInfo(patient);
                string[] assignRecordArgm = Function.GetAssignRecordInfo(patient);
                string[] tradeRecordsArgm = Function.GetTradeRecordsInfo(recordsInfo);

                insertReg = string.Format(insertReg, argm);
                insertRegFee = string.Format(insertRegFee, regFeeArgm);
                insertDiagFee = string.Format(insertDiagFee, diagFeeArgm);
                insertAssignRecord = string.Format(insertAssignRecord, assignRecordArgm);
                InsertTradeRecords = string.Format(InsertTradeRecords, tradeRecordsArgm);
                updateNurQueue = string.Format(updateNurQueue, patient.Queue.ID);
                // updateShemaLockState = string.Format(updateShemaLockState, opr.TranSerNo, OPERID, "3");
                string InsertSISql = string.Empty;//处理诊金减免的sql
                string InsertGDSIinfo = string.Empty;//省集中平台的sql
                string CheckSql = string.Empty;


                sqlList.Add(insertReg);
                sqlList.Add(insertRegFee);
                sqlList.Add(insertDiagFee);
                sqlList.Add(insertAssignRecord);
                sqlList.Add(InsertTradeRecords);//交易记录表插入数据
                sqlList.Add(updateNurQueue);
                //sqlList.Add(updateShemaLockState);

                #endregion

                #region 更新看诊序号

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

                #region 医保减免

                if (info.Payinsufeestr.Length > 1)
                {
                    //if (Function.GetSIRegInfoSql(info.Payinsufeestr, patient, ref msg, ref InsertSISql) == 0)
                    //{
                    //    Shadow.Util.Data.Management.Trans.RollBack();
                    //    return ErrSource(source, msg);
                    //}
                    //else
                    //{
                    //    sqlList.Add(InsertSISql);
                    //}

                    //省集中平台医保主表插入数据 
                    if (Function.getGDSIinfoSql(info.Payinsufeestr, patient, ref msg, ref InsertGDSIinfo) == 0)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        return ErrSource(source, msg);
                    }
                    else
                    {
                        sqlList.Add(InsertGDSIinfo);
                    }
                }

                #endregion

                #region 银行对账流水号

                if (Function.GetInsrtCheckSql(patient, info.BankCardNo, info.VouchNo, ref CheckSql, ref msg) == -1)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return this.ErrSource(source, msg);
                }
                else
                {
                    sqlList.Add(CheckSql);
                }

                #endregion

                #region 更新预约挂号表

                if (info.IsBook)
                    if (Function.GetUpdateBookSQL(book.ClinicCode, patient.ClinicCode.ToString(), ref updateBookingInfo, ref msg) == 0)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        return ErrSource(source, msg);

                    }
                    else
                        sqlList.Add(updateBookingInfo);


                #endregion

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
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Function.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "-1");
                        //更新新发票组
                        updateComInvoiceSql2 = string.Format(updateComInvoiceSql2, Function.OPERID, patient.RealInvoice, "1", invoiceGetTime);

                        updatecomDictionarySql = string.Format(updatecomDictionarySql, Function.OPERID, starInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updateComInvoiceSql2);
                        sqlList.Add(updatecomDictionarySql);
                    }
                    else
                    {
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Function.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                        updatecomDictionarySql = string.Format(updatecomDictionarySql, Function.OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updatecomDictionarySql);

                    }
                }
                else
                {
                    string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                    //更新旧发票组
                    updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, Function.OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                    updatecomDictionarySql = string.Format(updatecomDictionarySql, Function.OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                    sqlList.Add(updateComInvoiceSql1);
                    sqlList.Add(updatecomDictionarySql);
                }

                #endregion


                for (int i = 0; i < sqlList.Count; i++)
                {
                    if (mgr.ExecuteSql(sqlList[i].ToString(), ref msg) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        His.Util.Common.HisLog.WriteLog("ZZSB", "挂号失败，执行sql错误;\n" + sqlList[i].ToString());
                        return ErrSource(source, msg);
                    }
                }

                //if (!DataBaseHelp.DataExecHelp.ExecArrayList(sqlList))
                //{
                //    return ErrSource(source, "挂号登记失败!");
                //}

                source.Return.ErrorMsg = string.Empty;
                source.Return.Code = "1";
                source.Return.FunCode = patient.ClinicCode;
                source.Return.OpTime = Function.GetSysDate().ToString("yyyy-MM-dd HH:mm:ss");


                #endregion

            }
            catch (Exception ex)
            {
                Shadow.Util.Data.Management.Trans.RollBack();
                return ErrSource(source, "挂号发生未知错误，错误信息：" + ex.Message);
            }

            Shadow.Util.Data.Management.Trans.Commit();
            return source;
        }

        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="source"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private DataSource ErrSource(DataSource source, string errmsg)
        {
            Shadow.Util.Data.Management.Trans.RollBack();
            source.Return.Code = "190302";
            source.Return.ErrorMsg = " ErrMsg: " + errmsg;
            Shadow.Util.Data.Func.Log.WriteLog("Err", source.Return.ErrorMsg);
            return source;
        }


        public string GetBookDeptReqModel(string xml, ref His.Models.ZZSB.BookDeptReq opa)
        {


            string returnStr = "";
            opa = new BookDeptReq();
            DataSource source = new DataSource();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                source = ErrSource(source, "加载请求xml失败！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);

            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DeviceID = DEVICEID.InnerText;
            }
            else
            {
                source = ErrSource(source, "设备编号不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);

            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.ServiceCode = SERVICECODE.InnerText;
            }
            else
            {
                source = ErrSource(source, "服务编号不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FunCode = FUNCODE.InnerText;
            }
            else
            {
                source = ErrSource(source, "业务编号不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.ReqTime = Convert.ToDateTime(REQTIME.InnerText);
            }
            else
            {
                source = ErrSource(source, "请求时间不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.ReqTraceNo = REQTRACENO.InnerText;
            }
            else
            {
                source = ErrSource(source, "请求流水号不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }


            System.Xml.XmlNodeList HOSPCODE1 = doc.GetElementsByTagName("HospCode");
            System.Xml.XmlNode HOSPCODE = HOSPCODE1[0];
            if (!string.IsNullOrEmpty(HOSPCODE.InnerText))
            {
                opa.HospCode = HOSPCODE.InnerText;
            }
            else
            {
                source = ErrSource(source, "院区编号不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }


            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CardTypeCode = CARDTYPECODE.InnerText;
            }
            else
            {
                source = ErrSource(source, "卡类型不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            System.Xml.XmlNodeList REGDATE1 = doc.GetElementsByTagName("RegDate");
            System.Xml.XmlNode REGDATE = REGDATE1[0];
            if (!string.IsNullOrEmpty(REGDATE.InnerText))
            {
                opa.RegDate = REGDATE.InnerText;
            }
            else
            {
                source = ErrSource(source, "RegDate日期不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            System.Xml.XmlNodeList DEPTCODE1 = doc.GetElementsByTagName("DeptCode");
            System.Xml.XmlNode DEPTCODE = DEPTCODE1[0];
            if (!string.IsNullOrEmpty(DEPTCODE.InnerText))
            {
                opa.DeptCode = DEPTCODE.InnerText;
            }
            else
            {
                source = ErrSource(source, "DeptCode科室编号不能为空！");
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            return returnStr;
        }

        public string GetStoppedSchedules()
        {
            string xml = string.Empty;

            DataSource source = new DataSource();
            //return Root(ErrSource(source, msg)).ToString();



            try
            {
                string sql = @" select 
p.reg_id ClincCode,
p.schema_no RegSourceID,
p.card_no CardNO,
p.name Name,
p.dept_code DeptCode,
regexp_replace(p.dept_name,'[＆&]','、') DeptName,
p.doct_code DoctCode,
p.doct_name DoctName,
p.clinic_code AppointNO,
(select f.print_invoiceno from fin_opb_accountcardfee f where f.clinic_no=p.reg_id and rownum=1) print_invoiceno
  from fin_opr_booking p
 inner join fin_opr_schema sch
    on sch.id = p.schema_no
   and trunc(sch.see_date) >= trunc(sysdate)
   and sch.valid_flag = '0'
   --and sch.stop = '1'
 where p.valid_flag = '1'
   --and p.source = '4' ";
                if (regMgr.ExecQuery(sql) == -1)
                {
                    source = ErrSource(source, regMgr.Err);
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }
                source.Return.Code = "1";
                source.Return.ErrorMsg = string.Empty;
                source.Return.FunCode = "HIS_001";
                source.Return.OpTime = DateTime.Now.ToString();
                XElement root = Root(source);
                while (regMgr.Reader.Read())
                {
                    XElement result = new XElement("Result",
                        new XElement("ClincCode", regMgr.Reader[0].ToString()),
                        new XElement("RegSourceID", regMgr.Reader[1].ToString()),
                         new XElement("CardNO", regMgr.Reader[2].ToString()),
                          new XElement("Name", regMgr.Reader[3].ToString()),
                           new XElement("DeptCode", regMgr.Reader[4].ToString()),
                            new XElement("DeptName", regMgr.Reader[5].ToString()),
                             new XElement("DoctorCode", regMgr.Reader[6].ToString()),
                              new XElement("DoctorName", regMgr.Reader[7].ToString()),
                               new XElement("AppointNO", regMgr.Reader[8].ToString()),
                                new XElement("InvoiceNo", regMgr.Reader[9].ToString())
                              );

                    root.Element("Return").Add(result);

                }
                regMgr.Reader.Close();
                xml = root.ToString();
            }
            catch (Exception ex)
            {
                regMgr.Reader.Close();
                source = ErrSource(source, ex.Message);
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            return xml;

        }

        public string GetReturnTheStoppedRegRequsetModel(string requestXml, ref ReturnTheStoppedRegRequestModel requestModel)
        {
            string returnStr = "";


            try
            {
                requestModel = His.Business.ZZSB.Common.XmlHelper.Deserialize<ReturnTheStoppedRegRequestModel>(requestXml);

                if (requestModel == null)
                {
                    source = ErrSource(source, "入参对象不能为空！");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.ClincCode))
                {
                    source = ErrSource(source, "[ClincCode]门诊流水号不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.AppointNO))
                {
                    source = ErrSource(source, "[AppointNO]预约流水号不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.RegSourceID))
                {
                    source = ErrSource(source, "[RegSourceID]排班ID不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.CardNO))
                {
                    source = ErrSource(source, "[CardNO]门诊号不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.Name))
                {
                    source = ErrSource(source, "[Name]患者姓名不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.DeptCode))
                {
                    source = ErrSource(source, "[DeptCode]科室编码不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.ToTCost))
                {
                    source = ErrSource(source, "[ToTCost]总金额不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }

                if (string.IsNullOrEmpty(requestModel.OwnCost))
                {
                    source = ErrSource(source, "[OwnCost]自费金额不能为空!");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }



            }
            catch (Exception ex)
            {
                source = ErrSource(source, "解析请求requestXml出现异常:" + ex.Message);
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            return returnStr;
        }

        /// <summary>
        /// 查询出诊科室（预约）
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public string QueryBookDept(BookDeptReq reqInfo)
        {
            string xml = string.Empty;
            string sql = Sql.Sql.QueryBookDeptSql;
            DataSource source = new DataSource();


            try
            {
                if (string.IsNullOrEmpty(reqInfo.RegDate))
                {
                    source = ErrSource(source, "预约日期不能为空！");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }
                if (string.IsNullOrEmpty(reqInfo.DeptCode))
                {
                    source = ErrSource(source, "科室编号不能为空");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }
                DateTime regDate = Shadow.Util.Data.Func.NConvert.ToDateTime(reqInfo.RegDate);
                if (regDate < DateTime.Now.Date.AddDays(1))
                {
                    source = ErrSource(source, "不能预约当天或更早的排班！");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }


                sql = string.Format(sql, reqInfo.RegDate, reqInfo.DeptCode);
                if (regMgr.ExecQuery(sql) == -1)
                {
                    source = ErrSource(source, regMgr.Err);
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }
                source.Return.Code = "1";
                source.Return.ErrorMsg = string.Empty;
                source.Return.FunCode = reqInfo.FunCode;
                source.Return.OpTime = DateTime.Now.ToString();
                XElement root = Root(source);
                while (regMgr.Reader.Read())
                {
                    XElement result = new XElement("Result",
                        new XElement("RegSourceID", regMgr.Reader[0].ToString()),
                        new XElement("RegSourceName", regMgr.Reader[1].ToString()),
                         new XElement("SchemaType", regMgr.Reader[2].ToString()),
                          new XElement("TypeCode", regMgr.Reader[3].ToString()),
                           new XElement("TypeName", regMgr.Reader[4].ToString()),
                            new XElement("DeptCode", regMgr.Reader[5].ToString()),
                             new XElement("DeptName", regMgr.Reader[6].ToString()),
                              new XElement("DoctorCode", regMgr.Reader[7].ToString()),
                               new XElement("DoctorName", regMgr.Reader[8].ToString()),
                                new XElement("Specify", regMgr.Reader[9].ToString()),
                                 new XElement("RankID", regMgr.Reader[10].ToString()),
                                  new XElement("RankName", regMgr.Reader[11].ToString()),
                                   new XElement("StartTime", regMgr.Reader[12].ToString()),
                                    new XElement("EndTime", regMgr.Reader[13].ToString()),
                                     new XElement("SessionCode", regMgr.Reader[14].ToString()),
                                     new XElement("SessionName", regMgr.Reader[15].ToString()),
                                      new XElement("AllCount", regMgr.Reader[16].ToString()),
                                       new XElement("OutCount", regMgr.Reader[17].ToString()),
                                        new XElement("HaveCount", regMgr.Reader[18].ToString()),
                                         new XElement("TotalRegFee", regMgr.Reader[19].ToString()),
                                          new XElement("RegFee", regMgr.Reader[20].ToString()),
                                           new XElement("TreatFee", regMgr.Reader[21].ToString()),
                                            new XElement("ServicesFee", regMgr.Reader[22].ToString()),
                                             new XElement("MetaFee", regMgr.Reader[23].ToString()),
                                             new XElement("OtherFee", regMgr.Reader[24].ToString()),
                                              new XElement("AdmitAddress", regMgr.Reader[25].ToString()),
                                             new XElement("Note", regMgr.Reader[26].ToString()),
                                             new XElement("ElderlyVoucherDoctorFlag", regMgr.Reader[27].ToString())
                                             );
                    root.Element("Return").Add(result);

                }
                regMgr.Reader.Close();
                xml = root.ToString();
            }
            catch (Exception ex)
            {
                regMgr.Reader.Close();
                source = ErrSource(source, ex.Message);
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            return xml;
        }

        /// <summary>
        /// 查询出诊医生（预约）
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public string QueryBookDoct(BookDoctReq reqInfo)
        {

            string xml = string.Empty;
            string sql = Sql.Sql.QueryDoctTimes;
            DataSource source = new DataSource();




            try
            {
                if (string.IsNullOrEmpty(reqInfo.RegDate))
                {
                    source = ErrSource(source, "预约日期不能为空！");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }
                if (string.IsNullOrEmpty(reqInfo.DeptCode))
                {
                    source = ErrSource(source, "科室编号不能为空");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }
                if (string.IsNullOrEmpty(reqInfo.DoctCode))
                {
                    source = ErrSource(source, "医生编号不能为空");
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }


                sql = string.Format(sql, reqInfo.RegDate, reqInfo.DeptCode, reqInfo.DoctCode);
                if (regMgr.ExecQuery(sql) == -1)
                {
                    source = ErrSource(source, regMgr.Err);
                    return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
                }
                source.Return.Code = "1";
                source.Return.ErrorMsg = string.Empty;
                source.Return.FunCode = reqInfo.FunCode;
                source.Return.OpTime = DateTime.Now.ToString();
                XElement root = Root(source);
                while (regMgr.Reader.Read())
                {
                    XElement result = new XElement("Result",
                        new XElement("SchemaType", regMgr.Reader[0].ToString()),
                        new XElement("StartTime", regMgr.Reader[1].ToString()),
                         new XElement("EndTime", regMgr.Reader[2].ToString()),
                          new XElement("SessionCode", regMgr.Reader[3].ToString()),
                           new XElement("SessionName", regMgr.Reader[4].ToString()),
                            new XElement("AllCount", regMgr.Reader[5].ToString()),
                             new XElement("OutCount", regMgr.Reader[6].ToString()),
                              new XElement("HaveCount", regMgr.Reader[7].ToString()),
                               new XElement("TotalRegFee", regMgr.Reader[8].ToString()),
                                new XElement("Note", regMgr.Reader[9].ToString())
                                );
                    root.Element("Return").Add(result);

                }
                regMgr.Reader.Close();
                xml = root.ToString();
            }
            catch (Exception ex)
            {
                regMgr.Reader.Close();
                source = ErrSource(source, ex.Message);
                return Shadow.Util.Data.Func.XmlUtil.Serializer(source.GetType(), source);
            }

            return xml;
        }

        /// <summary>
        /// 预约挂号锁号源
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        public string BookLock(His.Models.ZZSB.OutPatientReg opr)
        {
            string returnStr = string.Empty;
            string sql = string.Empty, insertSql = string.Empty;
            int execResult = 0;
            DataSource source = new DataSource();

            sql = Sql.Sql.BookLockSql;//fin_opr_schema表号源-1
            insertSql = Sql.Sql.InsertRegLock;//自助锁号表fin_opr_schemalock插入数据

            try
            {
                sql = string.Format(sql, opr.RegSourceID, 1);
                execResult = regMgr.ExecNoQuery(sql);

                if (execResult > 0)
                {
                    insertSql = string.Format(insertSql, opr.ReqTraceNo, opr.UserID, opr.DeviceID,
                        opr.ServiceCode, opr.FunCode, opr.ReqTime, opr.CardNo, opr.DeptCode,
                        opr.SessionCode, opr.DoctorCode, opr.RegSourceID, "0", Function.OPERID, "0");
                    if (regMgr.ExecNoQuery(insertSql) == -1)
                    {
                        source = ErrSource(source, regMgr.Err);
                        return Root(source).ToString();
                    }

                    source.Return.OpTime = DateTime.Now.ToString();
                    source.Return.Code = "1";
                    source.Return.ErrorMsg = "";
                    source.Return.FunCode = opr.FunCode;
                    XElement root = Root(source);
                    root.Element("Return").Add(new XElement("Result",
                        new XElement("TranSerNo", opr.ReqTraceNo),
                        new XElement("Note", string.Empty)));

                    return root.ToString();
                }
                else
                {
                    string errMsg = string.Empty;
                    if (execResult == 0)
                        errMsg = "锁号失败，没有可用的号源！";
                    else
                        errMsg = "锁号失败，错误信息：" + regMgr.Err;

                    return Root(ErrSource(source, errMsg)).ToString();
                }
            }
            catch (Exception ex)
            {
                return Root(ErrSource(source, "锁号失败，错误信息：" + ex.Message)).ToString();
            }
        }

        /// <summary>
        /// 预约挂号释放号源
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        public string BookUnLock(His.Models.ZZSB.OutPatientReg opr)
        {

            string returnStr = string.Empty, resultCode = string.Empty;
            string sql = string.Empty;
            DataSource source = new DataSource();
            string lockState = string.Empty;

            sql = Sql.Sql.SelectRegLock;
            sql = string.Format(sql, opr.TranSerNo);

            System.Data.DataTable dt = new System.Data.DataTable();
            //排班表
            try
            {
                #region 判断锁定号源状态
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (!Convert.IsDBNull(dt.Rows[0][2]))
                        {
                            lockState = dt.Rows[0][2].ToString();
                        }

                        switch (lockState)
                        {
                            case "0":
                                resultCode = "1";
                                msg = "解锁成功";
                                break;
                            case "2":
                                resultCode = "0";
                                msg = "该锁定已解锁";
                                break;
                            case "3":
                                resultCode = "0";
                                msg = "该锁定已占用";
                                break;
                            default:
                                resultCode = "0";
                                msg = "没有找到相应号源";
                                break;
                        }
                        if (resultCode == "0")
                        {
                            return Root(ErrSource(source, msg)).ToString();
                        }

                    }
                    else
                    {
                        this.msg = "没有找到相应号源！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                else
                {
                    this.msg = "没有找到相应号源！";
                    return Root(ErrSource(source, msg)).ToString();
                }

                #endregion

                #region 更新排班表，号源表

                string updateSchema = @"update fin_opr_schema s --医师出诊表
                                           set s.tel_reging = s.tel_reging + {1}, --预约已约
                                               s.tel_reged  = s.tel_reged + {1},-- 预约已挂
                                         where s.id = '{0}'";
                string updateRegLock = Sql.Sql.UpdateRegLockState;

                Shadow.Util.Data.Management.Trans.BeginTransaction();
                updateSchema = string.Format(updateSchema, opr.RegSourceID, "-1");
                int result = regMgr.ExecNoQuery(updateSchema);
                if (result == 1)
                {
                    updateRegLock = string.Format(updateRegLock, opr.TranSerNo, Function.OPERID, "2");

                    if (regMgr.ExecNoQuery(updateRegLock) == -1)
                    {
                        msg = "解锁号源失败！" + regMgr.Err;
                        return Root(ErrSource(source, msg)).ToString();
                    }

                    source.Return.OpTime = DateTime.Now.ToString();
                    source.Return.Code = "1";
                    source.Return.ErrorMsg = "";
                    source.Return.FunCode = opr.FunCode;
                    XElement root = Root(source);
                    root.Element("Return").Add(
                        new XElement("Result",
                        new XElement("TranSerNo", opr.TranSerNo),
                        new XElement("Note", string.Empty)
                        ));
                    returnStr = root.ToString();

                }
                else
                {
                    if (result > 1)
                        msg = "解锁号源失败！";
                    if (result == 0)
                        msg = "解锁号源失败！错误信息：没有找到相应排班！";
                    if (result < 0)
                        msg = "解锁号源失败！错误信息：" + regMgr.Err;
                    return Root(ErrSource(source, msg)).ToString();
                }
                #endregion

            }
            catch (Exception ex)
            {
                Shadow.Util.Data.Func.Log.WriteLog("ZZSB", ex.Message);
                return Root(ErrSource(source, ex.Message)).ToString();
            }

            Shadow.Util.Data.Management.Trans.Commit();
            return returnStr;
        }

        /// <summary>
        /// 预约挂号（提交）
        /// </summary>
        /// <param name="regPaymentRequest"></param>
        /// <returns></returns>
        public string Appointment(His.Models.ZZSB.OutPatientReg opr)
        {

            // RegisterManager mgr = new RegisterManager();
            // string resultCode = "190302";
            string returnStr = string.Empty;
            //DateTime now = new Shadow.Util.Data.Management.OracleBase().GetDateTimeFromSysDateTime();
            DateTime now = this.regMgr.GetDateTimeFromSysDateTime();
            DataSource source = new DataSource();

            try
            {
                //这里开始增加事务控制 20161117 alter by  y_ming
                Shadow.Util.Data.Management.Trans.BeginTransaction();

                #region 验证挂号患者信息和挂号级别

                if (!ValidLock(opr))
                {
                    return Root(ErrSource(source, msg)).ToString();
                }

                #endregion

                #region 锁号已经扣掉号源，这里只做数据锁行作用

                //                string lockBook = @"update fin_opr_schema a
                //                    set a.tel_reging=a.tel_reging
                //                    where a.id='{0}'
                //                    and a.valid_flag='1'
                //                    and a.stop<>'1' ";

                string lockBook = @"update com_dictionary a
                                       set a.name = '0'
                                     where upper(a.type) = 'REGLOCK'
                                       and a.code = 'ZZSB0001' ";
                //lockBook = string.Format(lockBook, opr.RegSourceID);

                if (regMgr.ExecNoQuery(lockBook) != 1)
                {
                    return Root(ErrSource(source, regMgr.Err)).ToString();
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
                            msg = "获取患者信息出错！";
                            return Root(ErrSource(source, msg)).ToString();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.msg = "没有找到患者信息！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到患者信息！";
                    return Root(ErrSource(source, msg)).ToString();
                }

                #endregion


                //代表走医保 需要从医保结算表获取合同单位
                His.Business.ZZSB.Medical.MedicalDB db = new His.Business.ZZSB.Medical.MedicalDB();
                if (!string.IsNullOrEmpty(opr.ClincCode))
                {
                    opr.FeeType = db.GetSiPactCodeForClinCode(opr.ClincCode);
                    if (opr.FeeType == "-1" || string.IsNullOrEmpty(opr.FeeType))
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到门诊流水号对应的医保结算合同单位！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                #region 获取合同单位
                if (!string.IsNullOrEmpty(opr.Payinsufeestr))
                {
                    List<string> infos = opr.Payinsufeestr.Split('^').ToList();
                    if (infos.Count >= 2)
                    {
                        //if (!string.IsNullOrEmpty(infos[1]))
                        //{
                        //    opr.FeeType = "107";
                        //}
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
                            msg = "获取合同单位信息出错！";
                            return Root(ErrSource(source, msg)).ToString();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.msg = "没有找到合同单位信息！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到合同单位信息！";
                    return Root(ErrSource(source, msg)).ToString();
                }
                patient.Pact = pactUnit;
                #endregion

                #region 支付方式

                if (Function.SetPayType(opr.PayType, ref  patient, ref msg) == 0)
                {
                    return Root(ErrSource(source, msg)).ToString();
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
                            msg = "获取排班信息出错！";
                            return Root(ErrSource(source, msg)).ToString();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.msg = "没有找到排班信息！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.msg = "没有找到排班信息！";
                    return Root(ErrSource(source, msg)).ToString();
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
                        if (patient.OwnDigFee == null || string.IsNullOrEmpty(patient.OwnDigFee.ToString()))
                        {
                            //resultCode = "0";
                            msg = "获取费用信息出错！";
                            return Root(ErrSource(source, msg)).ToString();
                        }
                    }
                    else
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到费用信息！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到费用信息！";
                    return Root(ErrSource(source, msg)).ToString();
                }

                #endregion

                #region 获取护士分诊队列信息
                dt = new System.Data.DataTable();
                if (patient.SchemaType == "0")
                {
                    //为科室排班
                    //nurQueueSql1 = string.Format(nurQueueSql1, patient.Begin.ToShortDateString(), patient.Dept.ID, patient.Noon.ID, patient.Room.ID);
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
                            msg = "获取分诊队列信息出错！";
                            return Root(ErrSource(source, msg)).ToString();
                        }
                    }
                    else
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到分诊队列信息！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.msg = "没有找到分诊队列信息！";
                    return Root(ErrSource(source, msg)).ToString();
                }
                #endregion

                #region 获取发票信息
                string realInvoice = string.Empty;
                string invoiceStr = string.Empty;
                dt = new System.Data.DataTable();
                invoicenoSql1 = string.Format(invoicenoSql1, OPERID, "1");
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
                        returnStr = this.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr);
                        if (!string.IsNullOrEmpty(returnStr))
                        {
                            return returnStr;
                        }

                        patient.RealInvoice = realInvoice;
                        patient.InvoiceStr = invoiceStr;
                        patient.IsUseingInvoice = true;
                    }
                    else
                    {
                        invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
                        invoicenoSql1 = string.Format(invoicenoSql1, OPERID, "0");
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
                                returnStr = this.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr);
                                if (!string.IsNullOrEmpty(returnStr))
                                {
                                    return returnStr;
                                }

                                patient.InvoiceStr = invoiceStr;
                                patient.IsUseingInvoice = false;
                            }
                            else
                            {
                                //this.resultCode = "0";
                                this.msg = "没有找到发票信息！";
                                return Root(ErrSource(source, msg)).ToString();
                            }
                        }
                        else
                        {
                            // this.resultCode = "0";
                            this.msg = "没有找到发票信息！";
                            return Root(ErrSource(source, msg)).ToString();
                        }
                    }
                    patient.NextRealInvoice = this.AddNumber(patient.RealInvoice);
                    patient.NextInvoiceStr = this.AddNumber(patient.InvoiceStr);
                }
                else
                {
                    //this.resultCode = "0";
                    this.msg = "没有找到发票信息！";
                    return Root(ErrSource(source, msg)).ToString();
                }

                #endregion

                #region 获取门诊流水号
                if (string.IsNullOrEmpty(opr.ClincCode))
                {
                    opr.ClincCode = db.GetClinicCode();
                    if (opr.ClincCode == "-1" || string.IsNullOrEmpty(opr.ClincCode))
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到门诊流水号！";
                        return Root(ErrSource(source, msg)).ToString();
                    }


                }

                patient.ClinicCode = opr.ClincCode;
                //dt = new System.Data.DataTable();
                //dt = DataBaseHelp.DataExecHelp.GetDataTable(clinicCodeSql);

                //if (dt != null)
                //{
                //    if (dt.Rows.Count > 0)
                //    {
                //        if (!Convert.IsDBNull(dt.Rows[0][0]))
                //        {
                //            patient.ClinicCode = dt.Rows[0][0].ToString();
                //        }
                //        else
                //        {
                //            //this.resultCode = "0";
                //            this.msg = "获取门诊流水号出错！";
                //            return Root(ErrSource(source, msg)).ToString();
                //        }
                //    }
                //    else
                //    {
                //        // this.resultCode = "0";
                //        this.msg = "没有找到门诊流水号！";
                //        return Root(ErrSource(source, msg)).ToString();
                //    }
                //}
                //else
                //{
                //    //this.resultCode = "0";
                //    this.msg = "没有找到门诊流水号！";
                //    return Root(ErrSource(source, msg)).ToString();
                //}

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
                            this.msg = "获取门诊看诊次数出错！";
                            return Root(ErrSource(source, msg)).ToString();
                        }
                    }
                    else
                    {
                        //  this.resultCode = "0";
                        this.msg = "没有找到门诊看诊次数！";
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到门诊看诊次数！";
                    return Root(ErrSource(source, msg)).ToString();
                }

                #endregion

                #region 减免费用处理
                //string msg = string.Empty;
                if (!string.IsNullOrEmpty(opr.Payinsufeestr))
                {
                    if (Function.DualSIFeeInfo(opr.Payinsufeestr, ref patient, ref msg) == 0)
                    {
                        // this.resultCode = "0";
                        this.msg = "处理诊金减免出错！";
                        return Root(ErrSource(source, msg)).ToString();
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
                //        if (regMgr.ExecuteSql(updateLmtSql, ref msg) == -1)
                //        {
                //            //Shadow.Util.Data.Management.Trans.RollBack();
                //            return Root(ErrSource(source, msg)).ToString();
                //        }
                //    }

                //    else
                //    {
                //        //Shadow.Util.Data.Management.Trans.RollBack();
                //        this.msg = "没有可用的号源，请选择其他时段的排班！";
                //        return Root(ErrSource(source, msg)).ToString();

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
                //插入交易记录表
                string InsertTradeRecords = Sql.Sql.InsertTradeRecords;


                #region 获取交易记录信息
                Models.ZZSB.TradeRecords recordsInfo = new His.Models.ZZSB.TradeRecords();
                recordsInfo.TranserNo = opr.ReqTraceNo;//交易流水号
                recordsInfo.INVOICE_NO = patient.InvoiceStr;//发票号
                recordsInfo.CLINIC_NO = patient.ClinicCode;//
                recordsInfo.CARDNO = patient.CardNo;//卡号
                recordsInfo.NAME = patient.Name;//姓名
                recordsInfo.ORDERID = opr.BankCardNo;//订单号或者银行卡卡号
                recordsInfo.PAY_TYPE = patient.PayType;//支付方式
                recordsInfo.TYPE = "2";//交易类型
                recordsInfo.TOT_COST = opr.PayAmt.ToString("0.00");//交易金额
                recordsInfo.DEVICEID = opr.DeviceID;//设备编号
                recordsInfo.REMARK = patient.SeeNO.ToString();//备注,挂号插入的是看诊序号
                recordsInfo.PACTCODE = patient.Pact.ID;//合同单位
                #endregion

                ArrayList sqlList = new ArrayList();

                string[] argm = this.GetRegInfo(patient, opr.Triage_Serialnum, opr.InformedConsentResult);
                string[] regFeeArgm = this.GetRegFeeInfo(patient);
                string[] diagFeeArgm = Function.GetDiagFeeInfo(patient);// this.GetDiagFeeInfo(patient);
                string[] assignRecordArgm = this.GetAssignRecordInfo(patient);
                string[] TradeRecordsArgm = Function.GetTradeRecordsInfo(recordsInfo);


                insertReg = string.Format(insertReg, argm);
                insertRegFee = string.Format(insertRegFee, regFeeArgm);
                insertDiagFee = string.Format(insertDiagFee, diagFeeArgm);
                InsertTradeRecords = string.Format(InsertTradeRecords, TradeRecordsArgm);

                insertAssignRecord = string.Format(insertAssignRecord, assignRecordArgm);
                updateNurQueue = string.Format(updateNurQueue, patient.Queue.ID);
                updateShemaLockState = string.Format(updateShemaLockState, opr.TranSerNo, OPERID, "3");

                string InsertSISql = string.Empty;//处理诊金减免的sql
                string InsertGDSIinfo = string.Empty;//省集中平台的sql

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
                regMgr.GetBookSerialNo().ToString(),//2
                patient.Birthday,//3
                ZZSB.RegisterManager.OPERID,//4
                patient.ClinicCode, //5
                "4", // 预约类别 4 ：自助机。
                ZZSB.RegisterManager.OPERID,
                "sysdate"
                );

                sqlList.Add(insertFinOprBooking);

                #endregion

                #region 医保减免

                //if (opr.Payinsufeestr.Length > 1)
                //{
                //    //if (Function.GetSIRegInfoSql(opr.Payinsufeestr, patient, ref msg, ref InsertSISql) == 0)
                //    //{
                //    //    // this.resultCode = "0";
                //    //    // this.msg = "获取门诊看诊次数出错！";
                //    //    //Shadow.Util.Data.Management.Trans.RollBack();
                //    //    return Root(ErrSource(source, msg)).ToString();
                //    //}
                //    //else
                //    //{ sqlList.Add(InsertSISql); }
                //    //省集中平台医保主表插入数据
                //    if (Function.getGDSIinfoSql(opr.Payinsufeestr, patient, ref msg, ref InsertGDSIinfo) == 0)
                //    {
                //        Shadow.Util.Data.Management.Trans.RollBack();
                //        return Root(ErrSource(source, msg)).ToString();
                //    }
                //    else
                //    {
                //        sqlList.Add(InsertGDSIinfo);
                //    }
                //}

                #region 没用的代码 20180608 By zhaoyiqiang
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

                #endregion

                sqlList.Add(insertReg);//插入挂号主表 fin_opr_register
                sqlList.Add(insertRegFee);//插入挂号费用表fin_opb_accountcardfee
                sqlList.Add(insertDiagFee);//插入挂号费用表fin_opb_accountcardfee
                sqlList.Add(InsertTradeRecords);//插入交易记录表FIN_OPB_TRADERECORDSZZSB
                //sqlList.Add(insertAssignRecord);
                // sqlList.Add(updateNurQueue);
                sqlList.Add(updateShemaLockState);//更新自助锁号表fin_opr_schemalock

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
                        this.GetUnUseInvoice(ref starInvoice, ref invoiceGetTime);
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "-1");
                        //更新新发票组
                        updateComInvoiceSql2 = string.Format(updateComInvoiceSql2, OPERID, patient.RealInvoice, "1", invoiceGetTime);

                        updatecomDictionarySql = string.Format(updatecomDictionarySql, OPERID, starInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updateComInvoiceSql2);
                        sqlList.Add(updatecomDictionarySql);
                    }
                    else
                    {
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                        updatecomDictionarySql = string.Format(updatecomDictionarySql, OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updatecomDictionarySql);

                    }
                }
                else
                {
                    string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                    //更新旧发票组
                    updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                    updatecomDictionarySql = string.Format(updatecomDictionarySql, OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                    sqlList.Add(updateComInvoiceSql1);
                    sqlList.Add(updatecomDictionarySql);
                }

                #endregion

                for (int i = 0; i < sqlList.Count; i++)
                {
                    if (regMgr.ExecuteSql(sqlList[i].ToString(), ref msg) == -1)
                    {
                        His.Util.Common.HisLog.WriteLog("ZZSB", "挂号失败，执行sql错误;\n" +
                            sqlList[i].ToString());
                        msg = "挂号登记失败！" + msg;
                        return Root(ErrSource(source, msg)).ToString();
                    }
                }


                His.Util.Common.HisLog.WriteLog("ZZSB", insertDiagFee + "**********" + insertRegFee);

                #region 返回串

                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
                System.Xml.XmlElement root = xml.CreateElement("DataSource");
                xml.AppendChild(root);

                System.Xml.XmlElement root1 = xml.CreateElement("return");
                root.AppendChild(root1);

                System.Xml.XmlElement Code = xml.CreateElement("Code");
                Code.InnerText = "1";
                root1.AppendChild(Code);

                System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                ErrorMsg.InnerText = "";
                root1.AppendChild(ErrorMsg);

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);

                System.Xml.XmlElement TranSerNo = xml.CreateElement("TranSerNo");
                TranSerNo.InnerText = opr.TranSerNo;
                Result.AppendChild(TranSerNo);

                System.Xml.XmlElement TotalRegFee = xml.CreateElement("TotalRegFee");
                TotalRegFee.InnerText = (patient.RegFee + patient.PubDigFee + patient.OwnDigFee).ToString("0.00");
                Result.AppendChild(TotalRegFee);

                System.Xml.XmlElement RegFee = xml.CreateElement("RegFee");
                RegFee.InnerText = patient.RegFee.ToString("0.00");
                Result.AppendChild(RegFee);

                System.Xml.XmlElement TreatFee = xml.CreateElement("TreatFee");
                TreatFee.InnerText = patient.OwnDigFee.ToString("0.00");
                Result.AppendChild(TreatFee);

                System.Xml.XmlElement PatientBookFee = xml.CreateElement("PatientBookFee");
                PatientBookFee.InnerText = "0.00";
                Result.AppendChild(PatientBookFee);

                System.Xml.XmlElement ServicesFee = xml.CreateElement("ServicesFee");
                ServicesFee.InnerText = "0.00";
                Result.AppendChild(ServicesFee);

                System.Xml.XmlElement MetaFee = xml.CreateElement("MetaFee");
                MetaFee.InnerText = "0.00";
                Result.AppendChild(MetaFee);

                System.Xml.XmlElement OtherFee = xml.CreateElement("OtherFee");
                OtherFee.InnerText = "0.00";
                Result.AppendChild(OtherFee);

                System.Xml.XmlElement MedInsureFee = xml.CreateElement("MedInsureFee");
                MedInsureFee.InnerText = "0.00";
                Result.AppendChild(MedInsureFee);

                System.Xml.XmlElement PersonalFee = xml.CreateElement("PersonalFee");
                PersonalFee.InnerText = "0.00";
                Result.AppendChild(PersonalFee);

                System.Xml.XmlElement TreatLocation = xml.CreateElement("TreatLocation");
                TreatLocation.InnerText = "";
                Result.AppendChild(TreatLocation);

                System.Xml.XmlElement WaitTreatNo = xml.CreateElement("WaitTreatNo");
                WaitTreatNo.InnerText = "";
                Result.AppendChild(WaitTreatNo);

                System.Xml.XmlElement ReceiptNo = xml.CreateElement("ReceiptNo");
                ReceiptNo.InnerText = patient.RealInvoice;
                Result.AppendChild(ReceiptNo);

                System.Xml.XmlElement SortNo = xml.CreateElement("SortNo");
                SortNo.InnerText = patient.SeeNO.ToString();
                Result.AppendChild(SortNo);

                System.Xml.XmlElement Note = xml.CreateElement("Note");
                //Note.InnerText = patient.ClinicCode.ToString();
                Result.AppendChild(Note);

                returnStr = xml.InnerXml.ToString();
                #endregion


                #endregion
            }
            catch (Exception ex)
            {
                LogException(ex);
                msg = ex.Message + ex.StackTrace;
                Shadow.Util.Data.Management.Trans.RollBack();
                return Root(ErrSource(source, msg)).ToString();
            }

            Shadow.Util.Data.Management.Trans.Commit();
            return returnStr;

        }

        private void LogException(Exception ex)
        {

            if (ex == null)
                return;

            string errLog = "异常类型:" + ex.GetType().Name + Environment.NewLine;
            errLog += "异常信息:" + ex.Message + Environment.NewLine;

            // 方法信息
            var method = ex.TargetSite;
            if (method != null)
            {
                string methodName = method.Name != null ? method.Name : "未知方法";
                string className = method.DeclaringType != null ? method.DeclaringType.FullName : "未知类";
                string dllName = method.Module != null ? method.Module.Name : "未知模块";

                errLog += "函数名称:" + methodName + Environment.NewLine;
                errLog += "类名称:" + className + Environment.NewLine;
                errLog += "DLL名称:" + dllName + Environment.NewLine;
            }

            // 栈追踪信息，用来定位行号
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(ex, true); // true 表示尝试获取源文件名和行号
            if (st.FrameCount > 0)
            {
                System.Diagnostics.StackFrame frame = st.GetFrame(0); // 第一个帧是异常抛出的地方
                string fileName = frame.GetFileName() != null ? frame.GetFileName() : "未知文件";
                int lineNumber = frame.GetFileLineNumber();
                int colNumber = frame.GetFileColumnNumber();

                errLog += "源文件:" + fileName + Environment.NewLine;
                errLog += "行号:" + lineNumber + Environment.NewLine;
                errLog += "列号:" + colNumber + Environment.NewLine;
            }

            // 完整堆栈信息
            errLog += "堆栈信息:" + Environment.NewLine + ex.StackTrace + Environment.NewLine;

            // 写入日志
            His.Util.Common.HisLog.WriteLog("ExceptionLog", errLog);
        }

        /// <summary>
        /// 停诊退号
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public string ReturnTheStoppedReg(ReturnTheStoppedRegRequestModel requestModel)
        {
            string returnStr = string.Empty;

            DateTime now = this.regMgr.GetDateTimeFromSysDateTime();
            DataSource source = new DataSource();

            try
            {
                string sql = string.Format(@" select p.dept_code from fin_opr_schema p where p.id='{0}' and (p.valid_flag='0' or p.stop='1' ) ", requestModel.RegSourceID);

                var deptCode = this.regMgr.ExecSqlReturnOne(sql);
                if (string.IsNullOrEmpty(deptCode))
                {
                    return Root(ErrSource(source, "[" + requestModel.RegSourceID + "]排班ID没有找到对应的排班信息！")).ToString();
                }

                if (deptCode != requestModel.DeptCode)
                {
                    return Root(ErrSource(source, "科室信息不一致，无法进行退号处理！")).ToString();
                }

                sql = string.Format(@" select p.clinic_code,p.reg_id,p.name,p.doct_code,p.dept_code,p.card_no,p.schema_no,p.source,p.valid_flag from fin_opr_booking p where p.clinic_code='{0}' and p.source='4' ", requestModel.AppointNO);
                var bookIngDS = new DataSet();
                var bookIngDT = new DataTable();
                var sqlResult = this.regMgr.ExecQuery(sql, ref bookIngDS);
                if (bookIngDS == null || bookIngDS.Tables.Count <= 0)
                {
                    return Root(ErrSource(source, "查询预约挂号信息失败:" + this.regMgr.Err)).ToString();
                }

                bookIngDT = bookIngDS.Tables[0];
                if (bookIngDT.Rows.Count <= 0)
                {
                    return Root(ErrSource(source, "查询预约挂号信息失败:没有找到相关预约信息！")).ToString();
                }
                var bookIngRow = bookIngDT.Rows[0];

                if (requestModel.ClincCode != bookIngRow["reg_id"].ToString())
                {
                    return Root(ErrSource(source, "预约表门诊流水号与传入门诊流水号不一致！")).ToString();
                }

                if (requestModel.Name != bookIngRow["name"].ToString())
                {
                    return Root(ErrSource(source, "预约表姓名与传入姓名不一致！")).ToString();
                }

                if (requestModel.CardNO != bookIngRow["card_no"].ToString())
                {
                    return Root(ErrSource(source, "预约表门诊号与传入门诊号不一致！")).ToString();
                }

                sql = string.Format(@" select p.valid_flag,p.ynsee,p.clinic_code from fin_opr_register p where p.clinic_code='{0}' ", requestModel.ClincCode);

                var regDS = new DataSet();
                var regDT = new DataTable();
                sqlResult = this.regMgr.ExecQuery(sql, ref regDS);
                if (regDS == null || regDS.Tables.Count <= 0)
                {
                    return Root(ErrSource(source, "查询挂号信息失败:" + this.regMgr.Err)).ToString();
                }
                regDT = regDS.Tables[0];
                if (regDT.Rows.Count <= 0)
                {
                    return Root(ErrSource(source, "查询挂号信息失败:没有找到相关挂号信息！")).ToString();
                }
                var regRow = regDT.Rows[0];
                if (regRow["ynsee"].ToString() == "1")
                {
                    return Root(ErrSource(source, "退号失败:当前挂号已经看诊，无法再次退号！")).ToString();
                }
                if (regRow["valid_flag"].ToString() == "0")
                {
                    return Root(ErrSource(source, "退号失败:当前挂号已经被退，无法再次退号！")).ToString();
                }
                //开启事务
                Shadow.Util.Data.Management.Trans.BeginTransaction();

                #region 1.处理预约表信息
                sql = string.Format(@" update fin_opr_booking p set p.valid_flag='0' where p.clinic_code='{0}' and p.valid_flag='1' ", requestModel.AppointNO);
                sqlResult = this.regMgr.ExecNoQuery(sql);
                if (sqlResult <= 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Root(ErrSource(source, "更新预约挂号信息失败:" + this.regMgr.Err)).ToString();
                }

                #endregion

                #region 2.处理挂号表信息
                sql = string.Format(@" insert into fin_opr_register p
(
p.clinic_code,
p.trans_type,
p.card_no,
p.reg_date,
p.noon_code,
p.name,
p.idenno,
p.sex_code,
p.birthday,
p.rela_phone,
p.paykind_code,
p.paykind_name,
p.pact_code,
p.pact_name,
p.mcard_no,
p.reglevl_code,
p.reglevl_name,
p.dept_code,
p.dept_name,
p.schema_no,
p.order_no,
p.seeno,
p.begin_time,
p.end_time,
p.doct_code,
p.doct_name,
p.ynregchrg,
p.invoice_no,
p.ynbook,
p.ynfr,
p.append_flag,
p.reg_fee,
p.chck_fee,
p.diag_fee,
p.oth_fee,
p.own_cost,
p.pub_cost,
p.pay_cost,
p.valid_flag,
p.oper_code,
p.oper_date,
p.cancel_opcd,
p.cancel_date,
p.check_flag,
p.balance_flag,
p.ynsee,
p.see_date,
p.triage_flag,
p.print_invoicecnt,
p.is_sendinhoscase,
p.is_encryptname,
p.in_state,
p.eco_cost,
p.is_account,
p.is_emergency,
p.up_flag,
p.in_times,
p.patient_type,
p.hos_code,
p.greenway,
p.triage_serialnum,
p.isneedautooper,
p.source_flag,
p.informedconsentresult
)
select 
a.clinic_code,
'2',
a.card_no,
a.reg_date,
a.noon_code,
a.name,
a.idenno,
a.sex_code,
a.birthday,
a.rela_phone,
a.paykind_code,
a.paykind_name,
a.pact_code,
a.pact_name,
a.mcard_no,
a.reglevl_code,
a.reglevl_name,
a.dept_code,
a.dept_name,
a.schema_no,
a.order_no,
a.seeno,
a.begin_time,
a.end_time,
a.doct_code,
a.doct_name,
a.ynregchrg,
a.invoice_no,
a.ynbook,
a.ynfr,
a.append_flag,
case when a.reg_fee > 0 then -a.reg_fee else a.reg_fee end as reg_fee,
case when a.chck_fee > 0 then -a.chck_fee else a.chck_fee end as chck_fee,
case when a.diag_fee > 0 then -a.diag_fee else a.diag_fee end as diag_fee,
case when a.oth_fee > 0 then -a.oth_fee else a.oth_fee end as oth_fee,
case when a.own_cost > 0 then -a.own_cost else a.own_cost end as own_cost,
case when a.pub_cost > 0 then -a.pub_cost else a.pub_cost end as pub_cost,
case when a.pay_cost > 0 then -a.pay_cost else a.pay_cost end as pay_cost,
'0',
a.oper_code,
to_date('{1}','YYYY-MM-DD hh24:mi:ss'),--a.oper_date,
'00W999',--a.cancel_opcd,
to_date('{1}','YYYY-MM-DD hh24:mi:ss'),--a.cancel_date,
a.check_flag,
a.balance_flag,
a.ynsee,
a.see_date,
a.triage_flag,
a.print_invoicecnt,
a.is_sendinhoscase,
a.is_encryptname,
a.in_state,
a.eco_cost,
a.is_account,
a.is_emergency,
a.up_flag,
a.in_times,
a.patient_type,
a.hos_code,
a.greenway,
a.triage_serialnum,
a.isneedautooper,
a.source_flag,
a.informedconsentresult
from fin_opr_register a where a.clinic_code='{0}' and a.valid_flag='1' and a.ynsee='0' ", requestModel.ClincCode, now);
                sqlResult = this.regMgr.ExecNoQuery(sql);
                if (sqlResult <= 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Root(ErrSource(source, "新增挂号负交易信息失败:" + this.regMgr.Err)).ToString();
                }
                sql = string.Format(@" update fin_opr_register p
   set p.valid_flag = '0', p.cancel_opcd = '00W999', p.cancel_date = to_date('{1}','YYYY-MM-DD hh24:mi:ss')
 where p.clinic_code = '{0}'
   and p.valid_flag = '1'
   and p.ynsee = '0' ", requestModel.ClincCode, now);
                sqlResult = this.regMgr.ExecNoQuery(sql);
                if (sqlResult <= 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Root(ErrSource(source, "更新挂号信息失败:" + this.regMgr.Err)).ToString();
                }

                #endregion

                #region 3.处理支付表信息
                sql = string.Format(@" insert into fin_opb_accountcardfee p
(
p.invoice_no,
p.trans_type,
p.markno,
p.type,
p.tot_cost,
p.fee_oper,
p.fee_date,
p.oper_code,
p.oper_date,
p.balance_flag,
p.balance_no,
p.balance_opcd,
p.balance_date,
p.cancel_flag,
p.card_no,
p.print_invoiceno,
p.fee_type,
p.clinic_no,
p.remark,
p.pay_type,
p.own_cost,
p.pub_cost,
p.pay_cost
)	 
select
a.invoice_no,
'2',
a.markno,
a.type,
case when a.tot_cost > 0 then -a.tot_cost else a.tot_cost end as tot_cost,
a.fee_oper,
a.fee_date,
a.oper_code,
to_date('{1}','YYYY-MM-DD hh24:mi:ss'),--a.oper_date,
a.balance_flag,
a.balance_no,
a.balance_opcd,
a.balance_date,
'0',
a.card_no,
a.print_invoiceno,
a.fee_type,
a.clinic_no,
a.remark,
a.pay_type,
case when a.own_cost > 0 then -a.own_cost else a.own_cost end as own_cost,
case when a.pub_cost > 0 then -a.pub_cost else a.pub_cost end as pub_cost,
case when a.pay_cost > 0 then -a.pay_cost else a.pay_cost end as pay_cost
from fin_opb_accountcardfee a where a.clinic_no='{0}' and a.cancel_flag='1' ", requestModel.ClincCode, now);
                sqlResult = this.regMgr.ExecNoQuery(sql);
                if (sqlResult <= 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Root(ErrSource(source, "新增费用负交易信息失败:" + this.regMgr.Err)).ToString();
                }

                sql = string.Format(@" update fin_opb_accountcardfee p set p.cancel_flag='0' where p.clinic_no='{0}' and p.cancel_flag='1' ", requestModel.ClincCode);
                if (sqlResult <= 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Root(ErrSource(source, "更新费用信息失败:" + this.regMgr.Err)).ToString();
                }
                #endregion

                #region 4.处理医保信息
                sql = string.Format(@" select p.setlid from fin_ipr_siinmaininfo_gd p where p.inpatient_no='{0}' and p.valid_flag='1' and p.balance_state='1' and p.type_code='0' ", requestModel.ClincCode);
                var setlid = this.regMgr.ExecSqlReturnOne(sql);
                if (!string.IsNullOrEmpty(setlid) && setlid != "-1")
                {
                    GDSI.ZhuHaiSI.Business.Comom.MedicalService medicalService = new GDSI.ZhuHaiSI.Business.Comom.MedicalService();
                    Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
                    if (medicalService.CancelRegSettlement(requestModel.ClincCode, "00W999", "自助设备", "1") < 0)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        return Root(ErrSource(source, "医保处理失败:" + medicalService.ErrorMessage)).ToString();
                    }


                }
                #endregion

            }
            catch (Exception ex)
            {
                Shadow.Util.Data.Management.Trans.RollBack();//记得回滚事务
                LogException(ex);
                return Root(ErrSource(source, "停诊退号出现异常:" + ex.Message)).ToString();
            }
            source.Return.OpTime = DateTime.Now.ToString();
            source.Return.Code = "1";
            XElement root = Root(source);
            root.Element("Return").Add(
                new XElement("Result"
                ));
            returnStr = root.ToString();
            Shadow.Util.Data.Management.Trans.Commit();
            return returnStr;

        }



        #region func

        private string GetInvoiceCode(string operID)
        {
            string sql = Sql.Sql.GetInvoiceUserCode;
            sql = string.Format(sql, operID);
            System.Data.DataTable dt = new System.Data.DataTable();
            string userCode = string.Empty;
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
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

        private string GetInvoiceR(string sql, DateTime now, ref string realInvoice, ref string invoiceStr)
        {
            string returnStr = string.Empty;
            sql = string.Format(sql, OPERID);
            System.Data.DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
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
                        string userCode = this.GetInvoiceCode(OPERID);
                        invoiceStr = now.ToString("yyMMdd") + userCode + "0001";
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.msg = "没有找到发票信息！";
                    return Root(ErrSource(source, msg)).ToString();
                }
            }
            else
            {
                //this.resultCode = "0";
                this.msg = "没有找到发票信息！";
                return Root(ErrSource(source, msg)).ToString();
            }

            return returnStr;
        }

        private string[] GetRegInfo(His.Models.ZZSB.ComPatient patient, string triage_serialnum, string informedConsentResult)
        {
            string[] argm = {
                               patient.ClinicCode, //门诊号/发票号
                               patient.CardNo, //就诊卡号
                               patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"), //挂号日期
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
                               "1", //是否预约
                               "0", //1初诊/2复诊
                               patient.RegFee.ToString(), //挂号费
                               "0", //检查费
                                (patient.OwnDigFee+patient.PubDigFee-patient.RegFee).ToString(), //诊察费//patient.OwnDigFee.ToString(), //诊察费
                               "0", //附加费
                               (patient.RegFee + patient.OwnDigFee).ToString(), //自费金额
                                patient.PubDigFee.ToString(), //报销金额//"0", //报销金额
                               "0", //自付金额
                               "1", //退号标志
                               OPERID, //操作员代码
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
                                DateTime.Now.ToString(), //操作时间
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
                               OPERID,//分诊护士代码
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//分诊时间
                               "CORE_HIS50",
                               triage_serialnum,
                               informedConsentResult
                            };

            return argm;
        }

        private string[] GetRegFeeInfo(His.Models.ZZSB.ComPatient patient)
        {
            string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               patient.RegFee.ToString(),//总额
                               OPERID,//收费人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
                               OPERID,//操作人
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

        private string[] GetDiagFeeInfo(His.Models.ZZSB.ComPatient patient)
        {
            string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               patient.OwnDigFee.ToString(),//总额
                               OPERID,//收费人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
                               OPERID,//操作人
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
                               "0",//报销金额
                               "0",//自付金额
                               "COMM"//支付方式
                            };

            return argm;
        }

        private string[] GetAssignRecordInfo(His.Models.ZZSB.ComPatient patient)
        {
            string[] argm = {
                                patient.ClinicCode,   //门诊号
                                patient.SeeNO.ToString(),   //看诊序号
                                patient.CardNo,   //病历号
                                patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),   //挂号日期
                                patient.Name,   //患者姓名
                                patient.SexCode,   //性别
                                "01",   //结算类别
                                "0",   //1急诊/0普通
                                "0",   //1预约/0普通
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
                                OPERID,   //操作员
                                patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),  //操作时间
                                patient.Console.ID,//诊台代码
                                patient.Console.Name,//诊台名称
                                patient.RegLevel.ID,// 挂号级别代码
                                patient.RegLevel.Name,//挂号级别
                                "" //每日顺序号
                            };

            return argm;
        }

        private string AddNumber(string number)
        {
            string returnNumber = string.Empty;
            string sql = Sql.Sql.addnumber;
            sql = string.Format(sql, number);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
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

        private void GetUnUseInvoice(ref string starInvoice, ref string invoiceGetTime)
        {
            string returnNumber = string.Empty;
            string sql = Sql.Sql.GetUnUseInvoce;
            sql = string.Format(sql, OPERID);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
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

        private bool ValidLock(His.Models.ZZSB.OutPatientReg opr)
        {


            string sql = Sql.Sql.SelectRegLock;
            sql = string.Format(sql, opr.TranSerNo);

            System.Data.DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            string schemaID = string.Empty;
            string lockState = string.Empty;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        schemaID = dt.Rows[i][1].ToString();
                        lockState = dt.Rows[i][2].ToString();
                        break;
                    }
                    if (schemaID != opr.RegSourceID)
                    {
                        this.msg = "挂号排班和锁号排班信息不一致！";
                        return false;
                    }
                    if (lockState != "0")
                    {
                        this.msg = "锁号排班状态无效！";
                        return false;
                    }
                }
                else
                {
                    this.msg = "没有找到号源锁定信息！";
                    return false;
                }
            }
            else
            {
                this.msg = "没有找到号源锁定信息！";
                return false;
            }

            return true;
        }

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

        #endregion

    }
}
