using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizLogic.HealthRecord.Visit
{
    public class SMS : Neusoft.FrameWork.Management.Database
    {
        #region send
        /// <summary>
        /// 根据病历号查询所有短信记录
        /// </summary>
        /// <param name="carno"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS> QuerySendRecordByPationNo(string patientno)
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.QuerySendRecordByPationNo", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.QuerySendRecordByCarno字段！";
                return null;
            }

            try
            {
                strSQL = string.Format(strSQL, patientno);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return null;
            }

            this.ExecQuery(strSQL);

            List<Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS> list = new List<Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS>();

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS sendsms = new Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS();
                    sendsms.Mobiles = this.Reader[0].ToString();//m.MOBILES       ,
                    sendsms.Content = this.Reader[1].ToString();//m.CONTENT       ,
                    sendsms.Smid = FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());//m.SMID          ,
                    sendsms.Srcid = FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString());//m.SRCID         ,
                    sendsms.Url = this.Reader[4].ToString();//m.URL           ,
                    sendsms.Sendtime = this.Reader[5].ToString();//m.SENDTIME      ,
                    sendsms.Patientno = this.Reader[6].ToString();//m.PATIENTNO     ,
                    sendsms.Cardno = this.Reader[7].ToString();//m.CARDNO        ,
                    sendsms.Inpatientno = this.Reader[8].ToString();//m.INPATIENTNO   ,
                    sendsms.Linkwayid = FrameWork.Function.NConvert.ToInt32(this.Reader[9].ToString());//m.LINKWAYID     ,
                    sendsms.Models = this.Reader[10].ToString();//m.MODELS        ,
                    sendsms.Expend1 = this.Reader[11].ToString();//m.EXPEND1       ,
                    sendsms.Expend2 = this.Reader[12].ToString();//m.EXPEND2       ,
                    sendsms.Expend3 = this.Reader[13].ToString();//m.EXPEND3       ,
                    sendsms.Opercode = this.Reader[14].ToString();//m.OPERCODE      ,
                    sendsms.Operdate = FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString());//m.OPERDATE 
                    sendsms.Name = this.Reader[16].ToString();//name      ,
                    list.Add(sendsms);
                }
            }
            catch (Exception ex)
            {
                this.Err = "读取联系方式出错！" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            //返回数组
            return list;
        }

        /// <summary>
        /// 根据病历号查询一条短信记录
        /// </summary>
        /// <param name="carno"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS QuerySendRecordBySmid(long smid)
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.QuerySendRecordBySmid", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.QuerySendRecordBySmid字段！";
                return null;
            }

            try
            {
                strSQL = string.Format(strSQL, smid);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return null;
            }

            this.ExecQuery(strSQL);

            Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS sendsms = new Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS();
            try
            {
                while (this.Reader.Read())
                {
                    sendsms.Mobiles = this.Reader[0].ToString();//m.MOBILES       ,
                    sendsms.Content = this.Reader[1].ToString();//m.CONTENT       ,
                    sendsms.Smid = FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());//m.SMID          ,
                    sendsms.Srcid = FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString());//m.SRCID         ,
                    sendsms.Url = this.Reader[4].ToString();//m.URL           ,
                    sendsms.Sendtime = this.Reader[5].ToString();//m.SENDTIME      ,
                    sendsms.Patientno = this.Reader[6].ToString();//m.PATIENTNO     ,
                    sendsms.Cardno = this.Reader[7].ToString();//m.CARDNO        ,
                    sendsms.Inpatientno = this.Reader[8].ToString();//m.INPATIENTNO   ,
                    sendsms.Linkwayid = FrameWork.Function.NConvert.ToInt32(this.Reader[9].ToString());//m.LINKWAYID     ,
                    sendsms.Models = this.Reader[10].ToString();//m.MODELS        ,
                    sendsms.Expend1 = this.Reader[11].ToString();//m.EXPEND1       ,
                    sendsms.Expend2 = this.Reader[12].ToString();//m.EXPEND2       ,
                    sendsms.Expend3 = this.Reader[13].ToString();//m.EXPEND3       ,
                    sendsms.Opercode = this.Reader[14].ToString();//m.OPERCODE      ,
                    sendsms.Operdate = FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString());//m.OPERDATE 
                    sendsms.Name = this.Reader[16].ToString();//name      ,
                }
            }
            catch (Exception ex)
            {
                this.Err = "读取联系方式出错！" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            //返回数组
            return sendsms;
        }

        /// <summary>
        /// 插入发送短信记录
        /// </summary>
        /// <returns></returns>
        public int InsertSendSMS(Neusoft.HISFC.Models.HealthRecord.Visit.SendSMS sms)
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.InsertSendRecord", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.InsertSendRecord字段！";
                return -1;
            }

            try
            {
                strSQL = string.Format(strSQL, 
                sms.Mobiles,//MOBILES,         
                sms.Content,//CONTENT,         
                sms.Smid,//SMID,            
                sms.Srcid,//SRCID,           
                sms.Url,//URL,             
                sms.Sendtime,//SENDTIME,        
                sms.Patientno,//PATIENTNO,       
                sms.Cardno,//CARDNO,          
                sms.Patientno,//INPATIENTNO,     
                sms.Linkwayid,//LINKWAYID,       
                sms.Models,//MODELS,          
                sms.Expend1,//EXPEND1,         
                sms.Expend2,//EXPEND2,         
                sms.Expend3,//EXPEND3,         
                sms.Opercode,//OPERCODE,        
                sms.Operdate//OPERDATE 
                );
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return -1;
            }

            return this.ExecNoQuery(strSQL);
        }

        #endregion


        #region recrive
        /// <summary>
        /// 插入接收短信记录
        /// </summary>
        /// <returns></returns>
        public int InsertReceiveSMS(Neusoft.HISFC.Models.HealthRecord.Visit.ReceiveSMS sms)
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.InsertReceiveRecord", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.InsertReceiveRecord字段！";
                return -1;
            }

            try
            {
                strSQL = string.Format(strSQL,
                sms.Type,//TYPE,          
                sms.Mobile,//MOBILE,        
                sms.Smid,//SMID,          
                sms.Srcid,//SRCID,         
                sms.Code,//CODE,          
                sms.Content,//CONTENT,       
                sms.Receivetime,//RECELVETIME,   
                sms.MsgFmt,//MSGFMT,        
                sms.Expend1,//EXPEND1,       
                sms.Expend2,//EXPEND2,       
                sms.Expend3,//EXPEND3,       
                sms.Opercode,//OPERCODE,      
                sms.Operdate.ToString()//OPERDATE  
                );
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return -1;
            }

            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 查询接收短信记录
        /// </summary>
        /// <param name="smid"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.HealthRecord.Visit.ReceiveSMS> QueryReceiveRecode(long smid)
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.QueryReceiveRecord", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.QueryReceiveRecord字段！";
                return null;
            }

            try
            {
                strSQL = string.Format(strSQL, smid);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return null;
            }

            this.ExecQuery(strSQL);

            List<Neusoft.HISFC.Models.HealthRecord.Visit.ReceiveSMS> list = new List<Neusoft.HISFC.Models.HealthRecord.Visit.ReceiveSMS>();

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.Visit.ReceiveSMS receivesms = new Neusoft.HISFC.Models.HealthRecord.Visit.ReceiveSMS();
                    receivesms.Type = this.Reader[0].ToString();//TYPE        ,
                    receivesms.Mobile = this.Reader[1].ToString();//MOBILE      ,
                    receivesms.Smid = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());//SMID        ,
                    receivesms.Srcid = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString());//SRCID       ,
                    receivesms.Code = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[4].ToString());//CODE        ,
                    receivesms.Content = this.Reader[5].ToString();//CONTENT     ,
                    receivesms.Receivetime = this.Reader[6].ToString();//RECELVETIME ,
                    receivesms.MsgFmt = this.Reader[7].ToString();//MSGFMT      ,
                    receivesms.Expend1 = this.Reader[8].ToString();//EXPEND1     ,
                    receivesms.Expend2 = this.Reader[9].ToString();//EXPEND2     ,
                    receivesms.Expend3 = this.Reader[10].ToString();//EXPEND3     ,
                    receivesms.Opercode = this.Reader[11].ToString();//OPERCODE    ,
                    receivesms.Operdate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[12].ToString());//OPERDATE      
                    list.Add(receivesms);
                }
            }
            catch (Exception ex)
            {
                this.Err = "读取联系方式出错！" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            //返回数组
            return list;
        }
        
        #endregion

        /// <summary>
        /// 更新smid
        /// </summary>
        /// <returns></returns>
        public int UpdateSmID()
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.UpdateSmid", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.UpdateSmid字段！";
                return -1;
            }
            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 更新srcid
        /// </summary>
        /// <returns></returns>
        public int UpdateSrcid()
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.UpdateSrcid", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.UpdateSrcid字段！";
                return -1;
            }
            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 获取smid
        /// </summary>
        /// <returns></returns>
        public long GetSmid()
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.GetSmid", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.GetSmid字段！";
                return -1;
            }
            string id = "";
            id = this.ExecSqlReturnOne(strSQL,"");
            long smid = -1;
            try
            {
                smid = Neusoft.FrameWork.Function.NConvert.ToInt32(id);
            }
            catch(Exception e)
            {
                this.Err = "获取smid失败!";
                return -1;
            }
            return smid;
        }

        /// <summary>
        /// 获取srcid
        /// </summary>
        /// <returns></returns>
        public long GetSrcid()
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.MSM.GetSrcid", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.MSM.GetSrcid字段！";
                return -1;
            }
            string id = "";
            id = this.ExecSqlReturnOne(strSQL, "");
            long srcid = -1;
            try
            {
                srcid = Neusoft.FrameWork.Function.NConvert.ToInt32(id);
            }
            catch (Exception e)
            {
                this.Err = "获取Srcid失败!";
                return -1;
            }
            return srcid;
        }
    }
}
