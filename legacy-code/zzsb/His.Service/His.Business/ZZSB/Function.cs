using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Collections;
using System.Xml.Linq;

namespace His.Business.ZZSB
{

    public class Function
    {
        DataTable dt;
        public static string OPERID = RegisterManager.OPERID;

        /// <summary>
        /// 获取患者基本信息
        /// </summary>
        public static string compatientSql = @"SELECT a.card_no,
                                                       a.name, --姓名
                                                       a.birthday, --出生日期
                                                       a.sex_code, --性别
                                                       a.idenno, --身份证号
                                                       a.mcard_no, --医疗证号
                                                       a.home_tel,--电话
                                                       a.home --地址
                                                      -- a.begin_time
                                                  FROM com_patientinfo a,fin_opb_accountcard b --病人基本信息表

                                                 WHERE a.card_no=b.card_no
                                                  and  (a.card_no = '{0}' or b.markno = '{0}')
                                                ";//静态变量

        public static string compatientSqlnew = @"SELECT a.card_no,
                                          a.name, --姓名
                                          a.birthday, --出生日期
                                          a.sex_code, --性别
                                          a.idenno, --身份证号
                                          a.mcard_no, --医疗证号
                                          a.home_tel,--电话
                                          a.home --地址
                                          -- a.begin_time
                                          FROM com_patientinfo a,fin_opb_accountcard b --病人基本信息表
                                          WHERE a.card_no=b.card_no
                                          and  (a.card_no = '{0}' or b.markno = '{0}')
                                          
                                          union 
                                          
                                          SELECT  
                                          c.card_no,
                                          c.name, --姓名
                                          c.birthday, --出生日期
                                          c.sex_code, --性别
                                          c.idenno, --身份证号
                                          c.mcard_no, --医疗证号
                                          c.home_tel,--电话
                                          c.home --地址
                                          FROM com_patientinfo c
                                          WHERE c.card_no='{0}'
                                                ";

        public static string msg = string.Empty;
        /// <summary>
        /// 取患者信息
        /// </summary>
        /// <param name="cardno"></param>
        /// <returns></returns>
        public static int  GetPatientInfo(string cardno,ref His.Models.ZZSB.ComPatient patient,ref string msg)
        {
          //  His.Models.ZZSB.ComPatient patient = new His.Models.ZZSB.ComPatient();
            compatientSql = string.Format(compatientSqlnew, cardno);
            DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(compatientSql);
            //His.Models.ZZSB.ComPatient patient = null;
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
                        patient.RegDate = GetSysDate();
                        break;
                    }
                    if (patient == null || string.IsNullOrEmpty(patient.CardNo))
                    {
                        //resultCode = "0";
                        msg = "获取患者信息出错！";
                        return 0;
                    }
                }
                else
                {
                    //resultCode = "0";
                    msg = "没有找到患者信息！";
                    return 0;
                }
            }
            else
            {
                //resultCode = "0";
                msg = "没有找到患者信息！";
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// 取患者信息
        /// </summary>
        /// <param name="cardno"></param>
        /// <returns></returns>
        public static int GetPatientInfoByCar(string cardno, ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {
            String GetpationSql = "";
            DataTable dt = null;

            System.Text.RegularExpressions.Regex CarIDRul = new System.Text.RegularExpressions.Regex(@"\d{15}$|^\d{18}$|^\d{17}(\d|X|x)");//身份证
            if (CarIDRul.Matches(cardno).Count > 0)
            {
                GetpationSql = string.Format(Sql.Sql.SelectPationinfoByIDCar, cardno);
                dt = DataBaseHelp.DataExecHelp.GetDataTable(GetpationSql);
            }
            else
            {
                GetpationSql = string.Format(Sql.Sql.SelectPationinfoByMCar, cardno);
                dt = DataBaseHelp.DataExecHelp.GetDataTable(GetpationSql);
            }

            
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
                        patient.RegDate = GetSysDate();
                        break;
                    }
                    if (patient == null || string.IsNullOrEmpty(patient.CardNo))
                    {
                        msg = "获取患者信息出错！";
                        return 0;
                    }
                }
                else
                {
                    msg = "没有找到患者信息！";
                    return 0;
                }
            }
            else
            {
                msg = "没有找到患者信息！";
                return 0;
            }
            return 1;
        }
        

        /// <summary>
        /// 取挂号费
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public static int  GetRegFee(ref His.Models.ZZSB.ComPatient patient,ref string msg)
        {
           // DataTable dt;
             string regfeeSql = Sql.Sql.GetRegFee;
            regfeeSql = string.Format(regfeeSql, "1", patient.RegLevel.ID);

           DataTable  dt = DataBaseHelp.DataExecHelp.GetDataTable(regfeeSql);
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
                        return 0;
                    }
                }
                else
                {
                   // resultCode = "0";
                    //msg = "没有找到费用信息！";
                    return 0;
                }
            }
            else
            {
                //resultCode = "0";
                //msg = "没有找到费用信息！";
                return 0;
            }

            return 1;

        }

        /// <summary>
        /// 取排班信息
        /// </summary>
        /// <param name="RegSourceID"></param>
        /// <param name="patient"></param>
        /// <returns></returns>
        public static int GetSchema(string RegSourceID,ref His.Models.ZZSB.ComPatient patient,ref string msg)
        {
            #region 获取排班信息
            DataTable dt;
            string schemaSql = Sql.Sql.GetSchema;
            schemaSql = string.Format(schemaSql, RegSourceID);
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
                        return 0;
                    }
                }
                else
                {
                   // resultCode = "0";
                    msg = "没有找到排班信息！";
                    return 0;
                }
            }
            else
            {
               // resultCode = "0";
                msg = "没有找到排班信息！";
                return 0;
            }
            return 1;
            #endregion
        }

        /// <summary>
        /// 获取护士分诊队列信息
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetQueue(ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {
            #region 获取护士分诊队列信息
            DateTime now = GetSysDate();
            DataTable dt;
            string nurQueueSql1 = Sql.Sql.GetNurQueueByDept;
            string nurQueueSql2 = Sql.Sql.GetNurQueueByDoct;
            dt = new System.Data.DataTable();
            if (patient.SchemaType == "0")
            {
                //为科室排班
               // nurQueueSql1 = string.Format(nurQueueSql1, now.ToString("yyyy-MM-dd HH:mm:ss"), patient.Dept.ID, patient.Noon.ID);
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
                        //resultCode = "0";
                        msg = "获取分诊队列信息出错！";
                        return 0;
                    }
                }
                else
                {
                    //resultCode = "0";
                    msg = "没有找到分诊队列信息！";
                    return 0;
                }
            }
            else
            {
               // resultCode = "0";
                msg = "没有找到分诊队列信息！";
                return 0;
            }
            return 1;
            #endregion
        }

        /// <summary>
        /// 发票信息
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetInvoice(ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {
            #region 获取发票信息
            DataTable dt;
            DateTime now = GetSysDate();
            string invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
            string invoicenoSql2 = Sql.Sql.GetInvoiceR;
            string invoicenoSql3 = Sql.Sql.GetInvoiceUserCode;
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
                   
                        if (GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr, ref msg) == 0)
                        {
                            return 0;
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
                            if (GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr, ref msg) == 0)
                            {
                                return 0;
                            }
                     

                            patient.InvoiceStr = invoiceStr;
                            patient.IsUseingInvoice = false;
                        }
                        else
                        {
                            //resultCode = "0";
                            msg = "没有找到发票信息！";
                            return 0;
                        }
                    }
                    else
                    {
                        //resultCode = "0";
                        msg = "没有找到发票信息！";
                        return 0;
                    }
                }
                patient.NextRealInvoice = AddNumber(patient.RealInvoice);
                patient.NextInvoiceStr = AddNumber(patient.InvoiceStr);
            }
            else
            {
                //resultCode = "0";
                msg = "没有找到发票信息！";
                return 0;
            }
            return 1;
            #endregion
        }

        /// <summary>
        /// 取看诊排队号
        /// </summary>
        /// <param name="isbook"></param>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetSeeNo(bool isbook, ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {

            #region 获取seeNo
            string seenoSql = string.Empty;
            DateTime now = GetSysDate();
            DataTable dt;
            dt = new System.Data.DataTable();
            if (isbook)
            {
                seenoSql = Sql.Sql.bookSeeNO;//seenoSql = Sql.Sql.bookingSeeNO; 
                if (string.IsNullOrEmpty(patient.Book.OperDate))
                {
                    msg = "生产排队号失败，请联系工作人员！";
                    return -1;
                }
                seenoSql = string.Format(seenoSql, now.ToString("yyyy-MM-dd"), patient.Noon.ID, patient.Doct.ID, string.Empty, patient.SchemaType, patient.End.ToString("yyyy-MM-dd HH:mm:ss"), patient.SchemaID, patient.Book.OperDate);
                His.Util.Common.HisLog.WriteLog("SeeNo",patient.CardNo+":"+ seenoSql);
                dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
            }
            else
            {
                seenoSql = Sql.Sql.GetSeeNo;
                if (patient.SchemaType == "0")
                {
                    //为科室排班
                    //seenoSql = string.Format(seenoSql, now.ToString("yyyy-MM-dd"), patient.Noon.ID, "", patient.Dept.ID, patient.SchemaType, patient.End.ToString());
                    //dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
                    seenoSql = string.Format(Sql.Sql.GetNewSeeNo, now.ToString("yyyy-MM-dd"), patient.Room.ID, "5", patient.Noon.ID);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
                    if (dt == null || dt.Rows.Count <= 0 || Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        seenoSql = string.Format(Sql.Sql.GetSeeNo, now.ToString("yyyy-MM-dd"), patient.Noon.ID, "", patient.Dept.ID, patient.SchemaType, patient.End.ToString());
                        dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
                    }
                }
                else if (patient.SchemaType == "1")
                {
                    //为医生排班
                    seenoSql = string.Format(seenoSql, now.ToString("yyyy-MM-dd"), patient.Noon.ID, patient.Doct.ID, "", patient.SchemaType, patient.End.ToString());
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
                }
            }
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        patient.SeeNO = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                    }
                    else
                    {
                       // resultCode = "0";
                        msg = "获取看诊序号出错！";
                        return 0;
                    }
                }
                else
                {
                   // resultCode = "0";
                    msg = "没有找到看诊序号！";
                    return 0;
                }
            }
            else
            {
                //resultCode = "0";
                msg = "没有找到看诊序号！";
                return 0;
            }
            return 1;
            #endregion
        }


        public static int GetDocSeeNoBySchemaId(string id, ref int seeNo, ref string msg)
        {

            #region 获取seeNo
            string sql = Sql.Sql.GetSeeNoBySchemaNo;
            sql = string.Format(sql, id);
            DataTable dt = new DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        seeNo = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                    }
                    else
                    {
                        // resultCode = "0";
                        msg = "获取看诊序号出错！";
                        return 0;
                    }
                }
                else
                {
                    // resultCode = "0";
                    msg = "没有找到看诊序号，没有返回行！";
                    return 0;
                }
            }
            else
            {
                //resultCode = "0";
                msg = "没有找到看诊序号，返回dt为null！";
                return 0;
            }
            return 1;
            #endregion
        }

        public static int GetSchemaType(string id, ref int type, ref string msg)
        {
            string sql = @"select a.schema_type from fin_opr_schema a
                           where a.id ='{0}' ";
            sql = string.Format(sql, id);
            DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        type = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                    }
                    else
                    {
                        // resultCode = "0";
                        msg = "获取排班类别出错！";
                        return 0;
                    }
                }
                else
                {
                    // resultCode = "0";
                    msg = "获取排班类别出错,没有数据！";
                    return 0;
                }
            }
            else
            {
                //resultCode = "0";
                msg = "获取排班类别出错，dt 为 null！";
                return 0;
            }
            return 1;
        }

        public static int GetDeptSeeNoBySchemaId(string id, ref int seeNo, ref string msg)
        {

            #region 获取seeNo
            string sql = @"select max(a.see_sequence)+1 as seeNo from met_nuo_assignrecord a
where a.queue_code='{0}'";
            sql = string.Format(sql, id);
            DataTable dt = new DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        seeNo = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                    }
                    else
                    {
                        // resultCode = "0";
                        msg = "获取看诊序号出错！";
                        return 0;
                    }
                }
                else
                {
                    // resultCode = "0";
                    msg = "没有找到看诊序号！";
                    return 0;
                }
            }
            else
            {
                //resultCode = "0";
                msg = "没有找到看诊序号！";
                return 0;
            }
            return 1;
            #endregion
        }

        /// <summary>
        /// 获取门诊流水号
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetClinicCode(ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {
            #region 获取门诊流水号

            string clinicCodeSql = Sql.Sql.GetClinicCode;
           DataTable dt = new System.Data.DataTable();
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
                        //resultCode = "0";
                        msg = "获取门诊流水号出错！";
                        //return ReturnFailure();
                        return 0;
                    }
                }
                else
                {
                    //resultCode = "0";
                    msg = "没有找到门诊流水号！";
                   // return ReturnFailure();
                    return 0;
                }
            }
            else
            {
               // resultCode = "0";
                msg = "没有找到门诊流水号！";
                //return ReturnFailure();
                return 0;
            }
            return 1;
            #endregion
        }

        /// <summary>
        /// 取门诊次数
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetInTimes(ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {
            #region 获取门诊看诊次数
            DataTable dt;
            string intimesSql = Sql.Sql.GetOutPatientInTimes;
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
                        //resultCode = "0";
                        msg = "获取门诊看诊次数出错！";
                        return 0;
                    }
                }
                else
                {
                    //resultCode = "0";
                    msg = "没有找到门诊看诊次数！";
                    return 0;
                }
            }
            else
            {
               // resultCode = "0";
                msg = "没有找到门诊看诊次数！";
               // return ReturnFailure();
                return 0;
            }
            return 1;
            #endregion
        }

        /// <summary>
        /// 取服务器当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetSysDate()
        {
            string sql = Sql.Sql.GetSysDate;
            System.Data.DataTable dt = new System.Data.DataTable();
            DateTime now = new DateTime();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        now = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[0][0]);
                    }
                }
            }
            return now;
        }

        /// <summary>
        /// 获取InvoiceUserCode发票信息
        /// </summary>
        public static string GetInvoiceCode(string operID)
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

        /// <summary>
        /// 获取诊金减免登记sql
        /// </summary>
        /// <param name="SIInfo"></param>
        /// <param name="patient"></param>
        /// <returns></returns>
        public static int  GetSIRegInfoSql(string SIInfo, His.Models.ZZSB.ComPatient patient,ref string msg,ref string sql)
        {
            //<Payinsufeestr>Z20180919000126^93009266181^110200002-3^25^6.00^10.00^9.00^3^2000000002091954^13^13C06</Payinsufeestr>
           // <payinsufeestr>Z20151114005461^00000000^110200001^17.00^0.01^10.00^0^3<payinsufeestr>
           //诊金登记单号^门特结算单号^医生级别代码^挂号金额^个人支付金额^医改减免金额^病种报销金额^险种
            try
            {
                DateTime now = GetSysDate();
                List<string> infos = SIInfo.Split('^').ToList();
                 sql = Sql.Sql.InsertSIRegister;
                if (infos.Count > 0)
                {
                    sql = string.Format(sql, patient.IDCard, patient.Name, infos[0], infos[2], infos[3], infos[5], infos[6], infos[4], string.Format("{0:yyyyMMddHHmmssfff}", now), infos[7], infos[1], now.ToString("yyyyMMdd"));
                   
                }
                return 1;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// 获取省集中平台sql
        /// </summary>
        /// <param name="SIInfo"></param>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int getGDSIinfoSql(string SIInfo, His.Models.ZZSB.ComPatient patient, ref string msg, ref string sql) 
        {
            try
            {
                DateTime now = GetSysDate();
                List<string> infos = SIInfo.Split('^').ToList();
                sql = Sql.Sql.InsertGDSIinfo;
                if (infos.Count > 0)
                {
                    sql = string.Format(sql,
                 patient.ClinicCode,//挂号流水号
                 infos[0],//就医登记号
                 "0",//结算序号
                 "",//发票号
                 patient.CardNo,//门诊卡号
                 infos[8],//社会保险号2000000002091954
                 patient.Name,//personInfo.Name,
                 patient.IDCard,//personInfo.IdenNo,
                 "null01",//诊断
                 patient.Pact.PayKind.ID,
                 patient.Pact.ID,
                 patient.Pact.Name,
                 OPERID,//操作员
                 now,//操作时间
                 (patient.PubDigFee + patient.OwnDigFee).ToString("0.00"),//费用总额
                 patient.PubDigFee.ToString("0.00"),//报销金额
                 patient.OwnDigFee.ToString("0.00"),//自费金额
                 "1",
                 patient.SexCode,//性别
                 patient.Dept.ID,//科室
                 "0001-01-01",//住院时间
                 now,//结算时间
                 "0",
                 "",//Bka825
                 "",//Bka826
                 "",//Aka151
                 "",//Bka838
                 patient.OwnDigFee.ToString("0.00"),//个人现金支付
                 "",//Akb066
                 "",//Bka821
                 "",//Bka839
                 Convert.ToDecimal(infos[6]).ToString("0.00"),//医疗保险统筹基金支付
                 "",//Ake035
                 "",//Ake026
                 "",//Ake029
                 "",//Bka841
                 "",//Bka842
                 Convert.ToDecimal( infos[5]).ToString("0.00"),//其他基金支付
                 "",//住院号
                 "",//Aaa027
                 infos[10],//Aaz267门诊选点，门慢选点申请序号
                 "",//Bka438
                 "",//Aab301
                 infos[10],//待遇类型
                 infos[7],//险种
                 infos[9],//业务类型
                 patient.Dept.Name//科室名称
                 ); 
                }
                return 1;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return 0;
            }     
        }

        /// <summary>
        /// 处理诊金减免
        /// </summary>
        /// <param name="feeStr"></param>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int DualSIFeeInfo(string feeStr,ref His.Models.ZZSB.ComPatient patient,ref string msg)
        {
            //就医登记号^^^总费用^个人自付^诊金减免金额^病种报销金额^^医疗证号^业务类型^待遇类型^门慢登记号
            //sc000120181209000020^^^25^3.0^10.0^12.0^^1020000003000255^13^13C04^200064534
            if (string.IsNullOrEmpty(feeStr))
            {
                return 0;
            }

            List<string> infos = feeStr.Split('^').ToList();
            decimal pubDigFee,spPubDigFee,ownFee,totFee;
            if ((!decimal.TryParse(infos[4], out ownFee)) || (!decimal.TryParse(infos[5], out pubDigFee)) || (!decimal.TryParse(infos[6], out spPubDigFee)))
            {
                msg = "挂号减免费用类型有误！";
                return 0;
            }
            if (!decimal.TryParse(infos[3], out totFee))
           {
                msg = "总金额有误！错误信息："+feeStr;
                return 0;
           }
            
            //报销金额 = 减免金额+病种报销金额
            patient.PubDigFee=spPubDigFee + pubDigFee;//报销金额
            patient.OwnDigFee =  totFee- patient.PubDigFee;//自付金额
            
            //减免信息。
            patient.RegNo = infos[0];
            patient.RegDiagCode = infos[2];


            return 1;

        }

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="paytype"></param>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int SetPayType(string paytype, ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {
            try
            {
                //if (string.IsNullOrEmpty(paytype))
                //{
                //    //现金(刷银行卡)
                //    patient.PayType = "COMM";
                //}
                //else 
                if (paytype == "0")
                {
                    //现金(刷银行卡)
                    patient.PayType = "CCB";
                }
                else if (paytype == "1")
                {
                    //珠海医保
                    patient.PayType = "MCZH";
                }
                else if (paytype == "2")
                {
                    //珠海医保
                    patient.PayType = "ZFB";
                }
                else if (paytype == "3")
                {
                    //珠海医保
                    patient.PayType = "WX";
                }
                else if (paytype == "4")
                {
                    //珠海医保
                    patient.PayType = "CA";
                }
                else if (paytype == "5")
                {
                    //医保信用付
                    patient.PayType = "YBXYF";
                }
                else if (paytype == "6")
                {
                    //建行人民币
                    patient.PayType = "JHRMB";
                }
                else
                {
                    patient.PayType = "CCB";//"NH";
                }
                if (patient.Pact.ID == "258")
                {
                    patient.PayType = "XGZZQ";
                }
                return 1;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return 0;
            }
        
        }


        /// <summary>
        /// 取预约ID
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="bookId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetBookInfo(His.Models.ZZSB.ComPatient patient, ref His.Models.ZZSB.BookInfo info,ref string msg)
        {
            try
            {
                string sql = Sql.Sql.GetBookingInfo;
                sql = string.Format(sql, patient.CardNo, patient.SchemaID,patient.IDCard);
                DataTable dt = new DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    info = new His.Models.ZZSB.BookInfo();

                    info.ClinicCode = dt.Rows[0][0].ToString();
                    info.SchemaNo = dt.Rows[0][1].ToString();
                    info.DoctCode = dt.Rows[0][2].ToString();
                    info.DoctName = dt.Rows[0][3].ToString();
                    info.DeptCode = dt.Rows[0][4].ToString();
                    info.DeptName = dt.Rows[0][5].ToString();
                    info.SeeDate = dt.Rows[0][6].ToString();
                    info.LevelCode = dt.Rows[0][7].ToString();
                    info.BeginTime = dt.Rows[0][8].ToString();
                    info.Source = dt.Rows[0][9].ToString();
                    info.SeeFlag = dt.Rows[0][10].ToString();
                    if(dt.Columns.Count>11)
                    info.OperDate = dt.Rows[0][11].ToString();
                    if (dt.Columns.Count > 12)
                        info.EndTime = dt.Rows[0][12].ToString();
                }
                else
                {
                    msg = "查找预约信息出错！";
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return 0;
            }
            
        }

        /// <summary>
        /// 获取合同单位
        /// </summary>
        /// <param name="FeeType">合同单位编码</param>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int GetPactInfo(string FeeType, ref His.Models.ZZSB.ComPatient patient, ref string msg)
        {
            #region MyRegion
            string pactSql = Sql.Sql.GetPactInfo;
            pactSql = string.Format(pactSql, FeeType);
            DataTable dt = new DataTable();
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
                        pactUnit.Rate.PubRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][3].ToString().Trim());//公费比例                    
                        pactUnit.Rate.PayRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4].ToString().Trim());//自付比例                   
                        pactUnit.Rate.OwnRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][5].ToString().Trim()); //自费比例                   
                        pactUnit.Rate.RebateRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6].ToString().Trim()); //优惠比例                    
                        pactUnit.Rate.ArrearageRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][7].ToString().Trim());//欠费比例                    
                        pactUnit.Rate.IsBabyShared = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][8].ToString());//婴儿标志 0 无关 1 有关                                
                        pactUnit.IsNeedMCard = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][9].ToString().Trim()); //是否要求必须有医疗证号 0 否 1 是                      
                        pactUnit.IsInControl = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][10].ToString().Trim());//是否受监控 1受监控0不受监控                   
                        pactUnit.ItemType = dt.Rows[i][11].ToString().Trim(); //标志  0 全部 1 药品 2 非药品   
                        pactUnit.DayQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][12].ToString().Trim());//日限额                     
                        pactUnit.MonthQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][13].ToString().Trim()); //月限额                    
                        pactUnit.YearQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][14].ToString().Trim());//年限额
                        pactUnit.OnceQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][15].ToString().Trim());//一次限
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

                        pactUnit.BedQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][17].ToString());//床位限额
                        pactUnit.AirConditionQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][18].ToString());//空调限额
                        pactUnit.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[i][19]);//序号             
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
                        pactUnit.IsUseInOutPatientFee = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][28].ToString().Trim());

                        break;
                    }
                    if (pactUnit == null || string.IsNullOrEmpty(pactUnit.ID))
                    {
                        msg = "获取合同单位信息出错！";
                        return -1;
                    }
                }
                else
                {
                    msg = "没有找到合同单位信息！";
                    return -1;
                }
            }
            else
            {
                msg = "没有找到合同单位信息！";
                return -1;
            }
            patient.Pact = pactUnit;
            return 1; 
            #endregion
        }

        public static int GetUpdateBookSQL(string id,string schemaId, ref string sql, ref string msg)
        {
            try
            {
                 sql = Sql.Sql.UpdateBookInfo;
                sql = string.Format(sql,schemaId, id);
                return 1;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return 0;
            }
           
        }


        public static int GetInvoiceR(string sql, DateTime now, ref string realInvoice, ref string invoiceStr,ref string msg)
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
                        string userCode = GetInvoiceCode(OPERID);
                        invoiceStr = now.ToString("yyMMdd") + userCode + "0001";
                    }
                }
                else
                {
                    //resultCode = "0";
                    msg = "没有找到发票信息！";
                    return 0;
                }
            }
            else
            {
              //  resultCode = "0";
                msg = "没有找到发票信息！";
                return 0;
            }

            return 1;
        }

        public static string[] GetRegInfo(His.Models.ZZSB.ComPatient patient)
        {
            string[] argm = {
                               patient.ClinicCode, //门诊号/发票号
                               patient.CardNo, //就诊卡号
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"), //挂号日期
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
                               patient.Book==null?"0":"1", //是否预约
                               "0", //1初诊/2复诊
                               patient.RegFee.ToString(), //挂号费
                               "0", //检查费
                               (patient.OwnDigFee+patient.PubDigFee).ToString(), //诊察费
                               "0", //附加费
                               (patient.RegFee + patient.OwnDigFee).ToString(), //自费金额
                               "0", //报销金额
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
                               "",//诊金登记单号
                               patient.OwnDigFee.ToString(),//诊金金额
                               "", //诊金代码
                               "1",//分诊标志,0未分/1已分
                               OPERID,//分诊护士代码
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//分诊时间
                               "CORE_HIS50"
                            };

            return argm;
        }

        public static string[] GetRegFeeInfo(His.Models.ZZSB.ComPatient patient)
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

        /// <summary>
        /// 序列化交易记录信息
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        public static string[] GetTradeRecordsInfo(His.Models.ZZSB.TradeRecords recordsInfo)
        {
            string[] argm = { 
                            recordsInfo.TranserNo,//交易流水号
                            recordsInfo.INVOICE_NO,//发票号
                            recordsInfo.CLINIC_NO, //流水号
                            recordsInfo.CARDNO,//门诊号
                            recordsInfo.NAME,//姓名
                            recordsInfo.ORDERID,//银行卡号或者订单号
                            recordsInfo.PAY_TYPE,//支付方式
                            recordsInfo.TYPE,//交易类型
                            recordsInfo.TOT_COST,//交易金额
                            recordsInfo.DEVICEID,//设备编号
                            GetSysDate().ToString(),//操作时间
                            recordsInfo.REMARK,//备注
                            recordsInfo.PACTCODE//合同单位
                            };
            return argm;
        }

        public static string[] GetDiagFeeInfo(His.Models.ZZSB.ComPatient patient)
        {
            if (patient.Pact.ID == "99")//本院职工挂号
            {
                string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               (patient.OwnDigFee+patient.PubDigFee).ToString(),//总额
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
                               "0",//自费金额
                               patient.PubDigFee.ToString(),//报销金额
                               patient.OwnDigFee.ToString(),//自付金额
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
                               patient.PubDigFee.ToString(),//报销金额
                               "0",//自付金额
                               patient.PayType//支付方式
                            };
                return argm;
            }
        }

        public static string[] GetAssignRecordInfo(His.Models.ZZSB.ComPatient patient)
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
                                patient.Book==null?"0":"1",   //1预约/0普通
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

        public static string AddNumber(string number)
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

        /// <summary>
        /// 可用发票
        /// </summary>
        /// <param name="starInvoice"></param>
        /// <param name="invoiceGetTime"></param>
        public static void GetUnUseInvoice(ref string starInvoice, ref string invoiceGetTime)
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

        public static int GetInsrtCheckSql(His.Models.ZZSB.ComPatient patient, string bankNo, string vouchNo, ref string sql, ref string msg)
        {
            try
            {
                sql = Sql.Sql.InsertCheck;
                if (string.IsNullOrEmpty(vouchNo))
                {
                    msg = "交易流水号不能为空!";
                    return -1;
                }
                sql = string.Format(sql, patient.ClinicCode, "0", "1", patient.InvoiceStr, bankNo, vouchNo, "自助设备缴费对账", patient.PubDigFee + patient.OwnDigFee, ZZSB.RegisterManager.OPERID);
                return 1;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return -1;
            }
          


        }

        public static string GetXml(object t)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement datasource = doc.CreateElement("DataSource");
            XmlElement ret = doc.CreateElement("return");
            doc.AppendChild(datasource);
            datasource.AppendChild(ret);

            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (System.Reflection.PropertyInfo item in properties)
            {

                if (item.GetValue(t, null).GetType().Equals(typeof(ArrayList)))
                {
                    ArrayList items = (ArrayList)item.GetValue(t, null);
                    if (items == null || items.Count == 0)
                    {
                    }
                    else
                    {
                        #region detail/row 节点
                        foreach (object obj in items)
                        {
                            XmlElement res = doc.CreateElement("Result");
                            ret.AppendChild(res);

                            System.Reflection.PropertyInfo[] pro = obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                            foreach (System.Reflection.PropertyInfo proitem in pro)
                            {
                                string provalue = proitem.GetValue(obj, null).ToString();//获取值
                                string proname = proitem.Name.ToString();//获取属性名称
                                AppendChildNode(doc, res, proname, provalue);
                            }
                        }
                        #endregion
                    }

                }
                else
                {
                    string value = item.GetValue(t, null).ToString();//获取值
                    string name = item.Name.ToString();//获取属性名称
                    AppendChildNode(doc, ret, name, value);
                }
            }

            return doc.InnerXml.ToString();
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeValue"></param>
        private static void AppendChildNode(XmlDocument doc, XmlElement parentNode, string nodeName, string nodeValue)
        {
            if (doc == null || parentNode == null || string.IsNullOrEmpty(nodeName))
            {
                return;
            }
            XmlElement node = doc.CreateElement(nodeName);
            node.InnerText = nodeValue;
            parentNode.AppendChild(node);
        }


        public static XElement DataSource(string code,string msg,string funCode)
        {
            
            return new XElement("DataSource",
                new XElement("return",
                new XElement("Code", code),
                new XElement("ErrorMsg", msg),
                new XElement("OpTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement("FunCode", funCode)
                ));
                
        }

    }
}
