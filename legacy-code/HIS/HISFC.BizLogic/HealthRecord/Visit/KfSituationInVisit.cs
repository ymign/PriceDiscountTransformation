using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.HealthRecord.Visit.KfModel;

namespace Neusoft.HISFC.BizLogic.HealthRecord.Visit
{
    /// <summary>
    /// 康复情况业务层
    /// </summary>
    public class KfSituationInVisit : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="addtionXml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int InsertAddtionInfo(string recordId, string addtionXml, string type)
        {
            string strSql = @"insert into met_cas_visitKf (RECORD_ID, TYPE) values ('{0}', '{1}')";
            try
            {
                strSql = string.Format(strSql, recordId, type);
            }
            catch
            {
                this.Err = "格式化sql出错";
                return -1;
            }
            return this.ExecQuery(strSql) + this.SetReportXML(addtionXml, recordId);
        }
        
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public SpecialTag QueryKfSituation(string recordId)
        {
            string sql = @"select RECORD_ID, XML, TYPE from met_cas_visitKf t where t.RECORD_ID = '{0}'";
            sql = string.Format(sql, recordId);
            if (this.ExecQuery(sql) == -1)
            {
                this.Err = "执行SQL语句出错！" + this.Err;
                this.ErrCode = "-1";
                return null;
            }
            SpecialTag obj = new SpecialTag();
            try
            {
                while (this.Reader.Read())
                {
                    obj.RecordId = recordId;
                    obj.Xml = this.Reader[1].ToString();
                    obj.Type = this.Reader[2].ToString();
                }
            }
            catch (System.Exception ex)
            {
                this.Err = "获得随访信息出错！" + ex.Message;
                this.ErrCode = "-1";
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return obj;
        }

        /// <summary>
        /// 针对>4000以上的XML进行存储
        /// </summary>
        /// <returns></returns>
        public int SetReportXML(string xml, string recordID)
        {
            string strSql = "update met_cas_visitKf set Xml=:r where Record_Id='{0}'";
            try
            {
                strSql = string.Format(strSql, recordID);
                if (this.InputLong(strSql, xml) == -1)
                {
                    this.Err = "转换XML数据错误";
                    return -1;
                }
                return 1;
            }
            catch (Exception e)
            {
                this.Err = "初始化SQL语句出错" + e.Message;
                this.WriteErr();
                return -1;
            }
        }
    }
}
