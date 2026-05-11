using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// OperApply <br></br>
    /// [功能描述: 手术申请管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2010-02-22]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-13' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class OperApplyManage : Neusoft.FrameWork.Management.Database
    {

        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 将手术申请单实体转换为参数列表
        /// </summary>
        /// <param name="operApply">手术申请单实体</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.OperApply operApply)
        {
            string[] str = new string[]
                        {
                            operApply.OperApplyId.ToString(),
                            operApply.OperId,
                            operApply.OperName,
                            operApply.OperDeptName,
                            operApply.OperDeptId,
                            operApply.InHosNum,
                            operApply.PatientName, 
                            operApply.MediDoc.MainDoc.ID,
                            operApply.MediDoc.MainDoc.Name,
                            operApply.MediDoc.MainDoc1.ID,
                            operApply.MediDoc.MainDoc1.Name,
                            operApply.MediDoc.MainDoc2.Name,  
                            operApply.MediDoc.MainDoc2.ID,
                            operApply.MediDoc.MainDoc3.ID,                                                      
                            operApply.MediDoc.MainDoc3.Name,
                            operApply.OperTime.ToString(),
                            operApply.HadCollect,
                            operApply.NoColReason,
                            operApply.ColTime.ToString(),
                            operApply.HadOperInfo,
                            operApply.GetOperInfoTime.ToString(),
                            operApply.GetPeriod,                           
                            operApply.MainDiaICD,
                            operApply.MainDiaName,
                            operApply.MainDiaICD1,
                            operApply.MainDiaName1,
                            operApply.MainDiaICD2, 
                            operApply.MainDiaName2,                          
                            operApply.Comment,
                            operApply.OperLocation,
                            operApply.TumorPor,
                            operApply.OperPosId,
                            operApply.OperPosName,
                            operApply.OrgOrBlood,
                            operApply.OrderId
                        };
            return str;
        }

        /// <summary>
        /// 读取OperApply的完整实体信息
        /// </summary>
        /// <returns>手术申请单</returns>
        private OperApply SetOperApply()
        {
            OperApply operApply = new OperApply();
            try
            {
                operApply.OperApplyId = Convert.ToInt32(this.Reader["APPID"].ToString());
                operApply.OperId = this.Reader["OPERID"].ToString();
                operApply.OperName = this.Reader["OPERNAME"].ToString();
                operApply.OperDeptName = this.Reader["OPERDEPTNAME"].ToString();
                operApply.OperDeptId = this.Reader["OPERDEPTID"].ToString();
                operApply.InHosNum = this.Reader["INHOSNUM"].ToString();
                operApply.PatientName = this.Reader["PATIENTNAME"].ToString();
                operApply.MediDoc.MainDoc.ID = this.Reader["MAIN_DOCTOR"].ToString();
                operApply.MediDoc.MainDoc.Name = this.Reader["MAIN_DOCNAME"].ToString();
                operApply.MediDoc.MainDoc1.ID = this.Reader["ASS_DOC1"].ToString();
                operApply.MediDoc.MainDoc1.Name = this.Reader["ASS_DOCNAME1"].ToString();
                operApply.MediDoc.MainDoc2.Name = this.Reader["ASS_DOCNAME2"].ToString();
                operApply.MediDoc.MainDoc2.ID = this.Reader["ASS_DOC2"].ToString();
                operApply.MediDoc.MainDoc3.ID = this.Reader["ASS_DOC3"].ToString();
                operApply.MediDoc.MainDoc3.Name = this.Reader["ASS_DOCNAME3"].ToString();
                operApply.OperTime = Convert.ToDateTime(this.Reader["OPERTIME"].ToString());
                operApply.HadCollect = this.Reader["HADCOLLECT"].ToString();
                operApply.NoColReason = this.Reader["NOCOLREASON"].ToString();
                operApply.ColTime = Convert.ToDateTime(this.Reader["COLTIME"].ToString());
                operApply.HadOperInfo = this.Reader["HADOPERINFO"].ToString();
                operApply.GetOperInfoTime = Convert.ToDateTime(this.Reader["GETOPERINFOTIME"].ToString());
                operApply.GetPeriod = this.Reader["GETPEORID"].ToString();               
                operApply.MainDiaICD = this.Reader["MAIN_DIACODE"].ToString();
                operApply.MainDiaName = this.Reader["MAIN_DIANAME"].ToString();
                operApply.MainDiaICD1 = this.Reader["MAIN_DIACODE1"].ToString();
                operApply.MainDiaName1 = this.Reader["MAIN_DIANAME1"].ToString();
                operApply.MainDiaICD2 = this.Reader["MAIN_DIACODE2"].ToString();
                operApply.MainDiaName2 = this.Reader["MAIN_DIANAME2"].ToString();               
                operApply.Comment = this.Reader["MARK"].ToString();
                operApply.OperLocation = this.Reader["OPERLOCATION"].ToString();
                operApply.TumorPor = this.Reader["TUMORPOR"].ToString();
                operApply.OrgOrBlood = this.Reader["ORGORBLOOD"].ToString();
                operApply.OrderId = this.Reader["ORDERID"].ToString();
                 
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return operApply;
        }

        /// <summary>
        /// 根据sql索引取出OperApply
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList SelectOperApply(string sqlIndex, string[] parm)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            OperApply operApply;
            ArrayList arrOperApply = new ArrayList();
            while (this.Reader.Read())
            {
                operApply = SetOperApply();
                arrOperApply.Add(operApply);
            }
            this.Reader.Close();
            return arrOperApply;
        }

        #region 设置错误信息
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="errorCode">错误代码发生行数</param>
        /// <param name="errorText">错误信息</param>
        private void SetError(string errorCode, string errorText)
        {
            this.ErrCode = errorCode;
            this.Err = errorText + "[" + this.Err + "]"; // + "在ShelfSpecManage.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新操作
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateOperApply(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;//Update语句

            //获得Where语句
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return -1;
            }

            return this.ExecNoQuery(sql, args);
        }
        #endregion

        #endregion


        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        public int GetSequence()
        {
            //
            // 执行SQL语句
            //
            string sequence = this.GetSequence("Speciment.BizLogic.OperApplyManage.GetNextSequence");
            //
            // 如果返回NULL，则获取失败
            //
            if (sequence == null)
            {
                this.SetError("", "获取Sequence失败");
                return -1;
            }
            //
            // 成功返回
            //
            return Convert.ToInt32(sequence);
        }

        /// <summary>
        /// 手术申请单插入
        /// </summary>
        /// <param name="specSource"></param>
        /// <returns></returns>
        public int InsertOperApply(Neusoft.HISFC.Models.Speciment.OperApply operApply)
        {
            try
            {
                return this.UpdateOperApply("Speciment.BizLogic.OperApplyManage.Insert", this.GetParam(operApply));
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }


        /// <summary>
        /// 更新手术申请单
        /// </summary>
        /// <param name="operApply"></param>
        /// <returns></returns>
        public int UpdateOperApply(OperApply operApply)
        {
            try
            {
                return this.UpdateOperApply("Speciment.BizLogic.OperApplyManage.Update", this.GetParam(operApply));
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新全部信息
        /// </summary>
        /// <param name="operApply"></param>
        /// <returns></returns>
        public int UpdateOperApply1(OperApply operApply)
        {
            try
            {
                return this.UpdateOperApply("Speciment.BizLogic.OperApplyManage.Update1", this.GetParam(operApply));
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新收集标志
        /// </summary>
        /// <param name="operId">ID</param>
        /// <param name="flag">标志0，已收集，1 未收集，2 护士站 已发送 3 取消标本</param>
        /// <returns></returns>
        public int UpdateColFlag(string operApplyId, string flag)
        {
            try
            {
                return this.UpdateOperApply("Speciment.BizLogic.OperApplyManage.UpdateColFlag", new string[] { operApplyId, flag });
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 删除手术申请单
        /// </summary>
        /// <param name="operId">HIS手术申请单号</param>
        /// <returns></returns>
        public int DeleteOperApply(string operId)
        {
            try
            {
                return this.UpdateOperApply("Speciment.BizLogic.OperApplyManage.DeleteByOperId", new string[] { operId });
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 根据医嘱号删除记录
        /// </summary>
        /// <param name="orderId">医嘱流水号</param>
        /// <returns></returns>
        public int DeleteByOrderId(string orderId)
        {
            try
            {
                return this.UpdateOperApply("Speciment.BizLogic.OperApplyManage.DeleteByOrderId", new string[] { orderId });
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 根据sql获取手术申请单
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetOperApplyBySql(string sql)
        {
            this.ExecQuery(sql);
            OperApply operApply;
            ArrayList arrList = new ArrayList();
            while (this.Reader.Read())
            {
                operApply = new OperApply();
                operApply = SetOperApply();
                arrList.Add(operApply);
            }
            this.Reader.Close();
            return arrList;
        }

        /// <summary>
        /// 根据医嘱流水号获取
        /// </summary>
        /// <param name="orderId">医嘱流水号</param>
        /// <returns></returns>
        public ArrayList GetOperApplyByOrderId(string orderId)
        {
            return this.GetOperApplyBySql("select * from spec_operapply where orderid = '" + orderId + "'");
        }

        /// <summary>
        /// 取出没有录入手术信息的所有记录
        /// </summary>
        /// <returns></returns>
        public System.Data.DataSet GetOperInfoAll()
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            this.ExecQuery("Speciment.BizLogic.OperApplyManage.OperInfoAll", ref ds, new string[] { });
            return ds;
        }

        /// <summary>
        /// 根据住院流水号取出没有录入手术信息的记录
        /// </summary>
        /// <param name="inHosNum"></param>
        /// <returns></returns>
        public System.Data.DataSet GetOperInfoByInPatientNo(string inHosNum)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            this.ExecQuery("Speciment.BizLogic.OperApplyManage.OperInfoByInpatientno", ref ds, new string[] { inHosNum });
            return ds;
        }

        /// <summary>
        /// 根据住院流水号查出是否录入标本
        /// </summary>
        /// <param name="inHosNum"></param>
        /// <param name="type">B:血标本，O组织标本</param>
        /// <returns></returns>
        public ArrayList GetByInPatientNo(string inHosNum,string type)
        {
            return this.SelectOperApply("Speciment.BizLogic.OperApplyManage.OperApplyInfoByInpatientno", new string[] { inHosNum, type });
        }

        /// <summary>
        /// 查询出当天病人的血标本执行单
        /// </summary>
        /// <param name="inHosNum">住院流水号</param>
        /// <param name="dateTime">时间段</param>
        /// <param name="hadCol">是否收集</param>
        /// <returns></returns>
        public OperApply GetByInPatientNoAndOperTime(string inHosNum, string startTime,string endTime,string hadCol,string type)
        {

            ArrayList arr = new ArrayList();
            if (hadCol != "")
            {
                arr = SelectOperApply("Speciment.BizLogic.OperApplyManage.GetByInPatientNoAndOperTime", new string[] { inHosNum, startTime, endTime, hadCol, type });

            }
            else
            {
                arr = SelectOperApply("Speciment.BizLogic.OperApplyManage.GetByInPatientNoAndOperTimeAll", new string[] { inHosNum, startTime, endTime, type });
            }
            if (arr != null && arr.Count > 0)
            {
                return arr[0] as OperApply;
            }
            return null; 
        }

        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OperApply GetById(string Id, string type)
        {
            ArrayList arr = SelectOperApply("Speciment.BizLogic.OperApplyManage.GetById", new string[] { Id, type });
            if (arr != null && arr.Count > 0)
            {
                return arr[0] as OperApply;
            }
            return null;
        }

        /// <summary>
        /// 根据手术申请表ID更新 未取标本理由
        /// </summary>
        /// <param name="reason">理由</param>
        /// <param name="operApplyId">Id</param>
        /// <returns></returns>
        public int UpdateReason(string reason,string operApplyId)
        {
            try
            {
                return this.UpdateOperApply("Speciment.BizLogic.OperApplyManage.UpdateColReason", new string[] { reason, operApplyId });
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 查询出某一医生给定时间内没有实验室收到标本的记录
        /// </summary>
        /// <param name="docId">医生Id</param>
        /// <param name="operTime">给定时间</param>
        /// <param name="noColReason">没有收到的缘由</param>
        /// <param name="colFlag">是否收到的标记</param>
        /// <returns></returns>
        public ArrayList GetNoReasonApply(string docId, string operTime, string noColReason,string colFlag)
        {
            try
            {
                return this.SelectOperApply("Speciment.BizLogic.OperApplyManage.NoColSpec", new string[] { docId, operTime, noColReason, colFlag });
            }
            catch(Exception ex)
            {
                this.ErrCode = ex.Message;
                return null;
            } 
        }

        /// <summary>
        /// 根据手术申请单Id获取记录
        /// </summary>
        /// <param name="operId">手术申请单</param>
        /// <returns></returns>
        public ArrayList GetByOperId(string operId)
        {
            try
            {
                return this.SelectOperApply("Speciment.BizLogic.OperApplyManage.GetByOperId", new string[] { operId });
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return null;
            } 
        }

        /// <summary>
        /// 根据住院流水号取入院诊断
        /// </summary>
        /// <param name="inhosNum"></param>
        /// <returns></returns>
        public string GetDiaFromInMain(string inhosNum)
        {
            string sql = "select DIAG_NAME from FIN_IPR_INMAININFO where INPATIENT_NO='" + inhosNum + "'";
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 根据住院流水号更新诊断
        /// </summary>
        /// <param name="inHosNum"></param>
        /// <param name="diagNose"></param>
        /// <returns></returns>
        public int UpdateDiagInMain(string inHosNum,string diagNose)
        {
            string sql = "update FIN_IPR_INMAININFO set DIAG_NAME = '" + diagNose + "' where INPATIENT_NO='" + inHosNum + "'";
            return this.ExecNoQuery(sql);
        }
    }
}
