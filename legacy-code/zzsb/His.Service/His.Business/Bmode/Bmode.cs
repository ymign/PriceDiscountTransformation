using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Models.Endoscope;
using System.Data;
using His.Util.Common;
using System.Xml.Linq;

namespace His.Business.Bmode
{
    public class Bmode
    {

     public DataSource<object> examCheckInNotification(FeeStatusRequestInfo reqInfo)
        
        {
            DataSource<object> source = new DataSource<object>();

            source.Return.Result.ExamApply = null;
            string tabName = string.Empty;
            string msg = string.Empty;
            if (string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM) ||
                string.IsNullOrEmpty(reqInfo.PATIENT_TYPE))
            {
                source.Return.Code = "0";
                source.Return.ErrorMsg = "申请单号,患者来源(类型)不能为空！";
                //return source;
            }
            else
            {
                try
                {
                    if (reqInfo.PATIENT_TYPE == "0")
                        tabName = "fin_opb_feedetail";
                    else
                        tabName = "  fin_ipb_itemlist";

                    string whereSql = string.Empty;

                    if (string.IsNullOrEmpty(reqInfo.ORDER_ID))
                        whereSql += " and a.recipe_no='" + reqInfo.ORDER_ID + "'";

                    string sql = @"update {0} a
                            set  a.noback_num='0'
                            where   a.mo_order='{1}' ";
                    sql = string.Format(sql, tabName, reqInfo.APLY_FLOW_NUM);
                    if (!string.IsNullOrEmpty(whereSql))
                        sql += whereSql;
                    int result=DataBaseHelp.DataExecHelp.ExecuteSql(sql, ref msg);
                    // HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope,
                    if (result>0)
                    {
                        source.Return.Code = "1";
                        // source.Return.ErrorMsg = "接收成功！";
                    }
                    else if(result==0)
                    {
                        source.Return.Code = "0";
                        source.Return.ErrorMsg = "没有相关数据！";
                    }
                    else
                    {
                        source.Return.Code = "0";
                        source.Return.ErrorMsg = msg;
                    }
                }
                catch (Exception ex)
                {
                    source.Return.Code = "0";
                    source.Return.ErrorMsg = ex.Message;
                    HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope, "his.business.CancelCheckIn 发生程序错误：" + ex.Message);
                }
            }
            return source;
        }


     public DataSource<object> CancelExamCheckInNotification(FeeStatusRequestInfo reqInfo)
        {
            DataSource<object> source = new DataSource<object>();

            source.Return.Result.ExamApply = null;
            string tabName = string.Empty;
            string msg = string.Empty;
            if (string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM) ||
                string.IsNullOrEmpty(reqInfo.PATIENT_TYPE))
            {
                source.Return.Code = "0";
                source.Return.ErrorMsg = "申请单号,患者来源(类型)不能为空！";
                //return source;
            }
            else
            {
                try
                {
                    if (reqInfo.PATIENT_TYPE == "0")
                        tabName = "fin_opb_feedetail";
                    else
                        tabName = "  fin_ipb_itemlist";

                    string whereSql = string.Empty;

                    if (string.IsNullOrEmpty(reqInfo.ORDER_ID))
                        whereSql += " and a.recipe_no='" + reqInfo.ORDER_ID + "'";

                    string sql = @"update {0} a
                            set  a.noback_num=a.qty
                            where   a.mo_order='{1}' ";
                    sql = string.Format(sql, tabName, reqInfo.APLY_FLOW_NUM);
                    if (!string.IsNullOrEmpty(whereSql))
                        sql += whereSql;
                    int result=DataBaseHelp.DataExecHelp.ExecuteSql(sql, ref msg);
                    // HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope,
                    if (result>0)
                    {
                        source.Return.Code = "1";
                        // source.Return.ErrorMsg = "接收成功！";
                    }
                    else if(result==0)
                    {
                        source.Return.Code = "0";
                        source.Return.ErrorMsg = "没有相关数据！";
                    }
                    else
                    {
                        source.Return.Code = "0";
                        source.Return.ErrorMsg = msg;
                    }
                }
                catch (Exception ex)
                {
                    source.Return.Code = "0";
                    source.Return.ErrorMsg = ex.Message;
                    HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope, "his.business.CancelCheckIn 发生程序错误：" + ex.Message);
                }
            }
            return source;
        }

    }
}
