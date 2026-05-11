using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Order.Medical;

namespace Neusoft.HISFC.BizLogic.Order.Medical
{
    /// <summary>
    /// [功能描述: 过敏管理业务类]
    /// [创 建 者: wangw]
    /// [创建时间: 2008-3-20]
    /// </summary>
    public class AllergyManager : Neusoft.FrameWork.Management.Database
    {
        #region 构造方法

        public AllergyManager()
        {
        }

        #endregion

        /// <summary>
        /// 获取过敏实体的信息
        /// </summary>
        /// <param name="allergyInfo">过敏实体</param>
        /// <returns>成功返回参数数组/失败返回null值</returns>
        protected string[] myGetParmAllergy(Neusoft.HISFC.Models.Order.Medical.AllergyInfo allergyInfo)
        {
            try
            {
                string[] parm = {
                                            allergyInfo.PatientNO,//病历号或者住院号
                                            NConvert.ToInt32(allergyInfo.PatientType).ToString(),//1门诊患者/2住院患者
                                            allergyInfo.Allergen.ID,//药品院内代码
                                            allergyInfo.Allergen.Name,//过敏药物
                                            allergyInfo.Symptom.ID,//1：皮试阳性 2：休克 3：药疹
                                            NConvert.ToInt32(allergyInfo.ValidState).ToString(),//1有效/0无效
                                            allergyInfo.Remark,//备注
                                            allergyInfo.Oper.ID,//操作员代码
                                            allergyInfo.Oper.OperTime.ToString(),//操作时间(最新)
                                            allergyInfo.CancelOper.ID,//作废人
                                            allergyInfo.CancelOper.OperTime.ToString(),//作废时间
                                            allergyInfo.Type.ToString(),//过敏类型
                                            allergyInfo.ID,//门诊号或者住院流水号
                                            allergyInfo.HappenNo.ToString(),//发生序号
                                            //{D1B1616C-3863-40f6-AAD5-11D9161C6B14}
                                            allergyInfo.Allergen.Memo//过敏药物类别
                                       };
                return parm;
            }
            catch (System.Exception ex)
            {
                this.Err = "由实体获取参数信息时出错! \n" + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取过敏信息数组
        /// </summary>
        /// <param name="strSql">查询所用SQL语句</param>
        /// <returns>返回查询数组</returns>
        protected ArrayList myGetAllergy(string strSql)
        {
            ArrayList al = new ArrayList();
            AllergyInfo allergyInfo = null;
            if (this.ExecQuery(strSql) == -1)
            {
                this.Err = "获取患者过敏信息失败!" + this.Err;
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    allergyInfo = new AllergyInfo();
                    allergyInfo.PatientNO = this.Reader[0].ToString();
                    allergyInfo.PatientType = (Neusoft.HISFC.Models.Base.ServiceTypes)NConvert.ToInt32(this.Reader[1].ToString());
                    Neusoft.FrameWork.Models.NeuObject allergenObj = new Neusoft.FrameWork.Models.NeuObject();
                    allergenObj.ID = this.Reader[2].ToString();
                    allergenObj.Name = this.Reader[3].ToString();
                    //{D1B1616C-3863-40f6-AAD5-11D9161C6B14}
                    //allergenObj.Memo = this.Reader[14].ToString();
                    allergyInfo.Allergen = allergenObj;
                    Neusoft.FrameWork.Models.NeuObject symptomObj = new Neusoft.FrameWork.Models.NeuObject();
                    symptomObj.ID = this.Reader[4].ToString();
                    allergyInfo.Symptom = symptomObj;
                    allergyInfo.ValidState = NConvert.ToBoolean(this.Reader[5].ToString());
                    allergyInfo.Remark = this.Reader[6].ToString();
                    allergyInfo.Oper.ID = this.Reader[7].ToString();
                    allergyInfo.Oper.OperTime = NConvert.ToDateTime(this.Reader[8].ToString());
                    allergyInfo.CancelOper.ID = this.Reader[9].ToString();
                    allergyInfo.CancelOper.OperTime = NConvert.ToDateTime(this.Reader[10].ToString());
                    allergyInfo.Type = (Neusoft.HISFC.Models.Order.Medical.AllergyType)Enum.Parse(typeof(Neusoft.HISFC.Models.Order.Medical.AllergyType), this.Reader[11].ToString());
                    allergyInfo.ID = this.Reader[12].ToString();
                    allergyInfo.HappenNo = NConvert.ToInt32(this.Reader[13].ToString());


                    al.Add(allergyInfo);
                }
                return al;
            }
            catch (System.Exception ex)
            {
                this.Err = "获取过敏信息时 由READER内读取信息出错!" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        /// <summary>
        /// 获取最大发生序号
        /// </summary>
        /// <param name="inpatientNo">门诊号或者住院流水号</param>
        /// <param name="patientKind">1门诊患者/2住院患者</param>
        /// <returns>成功返回最大HappenNo/失败返回-1</returns>
        public int GetMaxHappenNo(string inpatientNo, string patientKind)
        {
            string strSql = string.Empty;
            //取SQL语句
            if (this.Sql.GetCommonSql("Order.Allergy.GetMaxHappenNo", ref strSql) == -1)
            {
                this.Err = "没有找到Order.Allergy.GetMaxHappenNo字段!";
                return -1;
            }

            strSql = string.Format(strSql, inpatientNo, patientKind);
            //执行sql语句
            this.ExecQuery(strSql);
            int maxHappenNo = 1;

            try
            {
                while (this.Reader.Read())
                {
                    string temp = this.Reader[0].ToString();
                    if (temp != "")
                    {
                        maxHappenNo = NConvert.ToInt32(temp) + 1;
                        return maxHappenNo;
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.Err = "获取最大发生序号错误!" + ex.Message;
                return -1;
            }
            finally
            {
                this.Reader.Close();
            }
            return maxHappenNo;
        }

        /// <summary>
        /// 根据SQL获取过敏信息
        /// </summary>
        /// <param name="whereIndex"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        private ArrayList MyQueryAllergyInfo(string whereIndex,params object[] parms)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Order.Allergy.QueryAllAllergyInfo", ref strSql) == -1)
            {
                this.Err = "查找Sql失败：[Order.Allergy.QueryAllAllergyInfo]";
                return null;
            }

            if (!string.IsNullOrEmpty(whereIndex))
            {
                string whereSql = "";
                if (this.Sql.GetCommonSql(whereIndex, ref whereSql) == -1)
                {
                    this.Err = "查找Sql失败：[" + whereIndex + "]";
                    return null;
                }

                whereSql = string.Format(whereSql, parms);

                strSql = strSql + "\r\n" + whereSql;
            }
            return this.myGetAllergy(strSql);
        }

        /// <summary>
        /// 获取所有过敏信息
        /// </summary>
        /// <returns>过敏信息数组</returns>
        public ArrayList QueryAllergyInfo()
        {
            return MyQueryAllergyInfo("");
        }

        /// <summary>
        /// 查询患者过敏信息
        /// </summary>
        /// <param name="inPatientNo">流水号</param>
        /// <returns>过敏信息</returns>
        public ArrayList QueryAllergyInfo(string inPatientNo)
        {
            return MyQueryAllergyInfo("Order.Allergy.QueryAllergyByiPatientNo");

            //string strSql = string.Empty;
            //string strWhere = string.Empty;
            ////取SELECT语句
            //if (this.Sql.GetCommonSql("Order.Allergy.QueryAllAllergyInfo", ref strSql) == -1)
            //{
            //    this.Err = "查找Order.Allergy.QueryAllAllergyInfo字段失败";
            //    return null;
            //}
            ////取WHERE语句
            //if (this.Sql.GetCommonSql("Order.Allergy.QueryAllergyByiPatientNo", ref strWhere) == -1)
            //{
            //    this.Err = "查找Order.Allergy.QueryAllergyByiPatientNo字段失败";
            //    return null;
            //}

            //strSql = strSql + strWhere;

            //strSql = string.Format(strSql, inPatientNo);

            //return this.myGetAllergy(strSql);
        }

        /// <summary>
        /// 查询有效的患者过敏信息
        /// </summary>
        /// <param name="patientNO"></param>
        /// <param name="patientType">1 门诊 2 住院</param>
        /// <returns></returns>
        public ArrayList QueryValidAllergyInfo(string patientNO, string patientType)
        {
            return this.MyQueryAllergyInfo("Order.Allergy.QueryValidAllergyByPatient", patientNO, patientType);
        }

        /// <summary>
        /// 查询患者过敏信息
        /// {D1B1616C-3863-40f6-AAD5-11D9161C6B14}
        /// </summary>
        /// <param name="patientNo">门诊卡号/住院号</param>
        /// <returns>过敏信息</returns>
        public ArrayList QueryAllergyInfo(string patientNO, string patientType)
        {
            return this.MyQueryAllergyInfo("Order.Allergy.QueryAllergyByPatient", patientNO, patientType);
            //string strSql = string.Empty;
            //string strWhere = string.Empty;
            ////取SELECT语句
            //if (this.Sql.GetCommonSql("Order.Allergy.QueryAllAllergyInfo", ref strSql) == -1)
            //{
            //    this.Err = "查找Order.Allergy.QueryAllAllergyInfo字段失败";
            //    return null;
            //}
            ////取WHERE语句
            //if (this.Sql.GetCommonSql("Order.Allergy.QueryAllergyByPatient", ref strWhere) == -1)
            //{
            //    this.Err = "查找Order.Allergy.QueryAllergyByPatient字段失败";
            //    return null;
            //}

            //strSql = strSql +" " + strWhere;

            //strSql = string.Format(strSql, patientNO, patientType);

            //return this.myGetAllergy(strSql);
        }

        /// <summary>
        /// 查询患者过敏信息
        /// </summary>
        /// <param name="patient">住院号</param>
        /// <param name="inpatientNO">流水号</param>
        /// <param name="happenNO">发生序号</param>
        /// <returns>过敏信息</returns>
        public ArrayList GetAllergyInfo(string patient, string inpatientNO, int happenNO)
        {
            return MyQueryAllergyInfo("Order.Allergy.QueryAllergyInfoWhere", inpatientNO, happenNO);
            //string strSql = string.Empty;
            //string strWhere = string.Empty;
            ////取SELECT语句
            //if (this.Sql.GetCommonSql("Order.Allergy.QueryAllAllergyInfo", ref strSql) == -1)
            //{
            //    this.Err = "查找Order.Allergy.QueryAllAllergyInfo字段失败";
            //    return null;
            //}
            ////取WHERE语句
            //if (this.Sql.GetCommonSql("Order.Allergy.QueryAllergyInfoWhere", ref strWhere) == -1)
            //{
            //    this.Err = "查找Order.Allergy.QueryAllergyByiPatientNo字段失败";
            //    return null;
            //}

            //strSql = strSql + strWhere;

            //strSql = string.Format(strSql, patient, inpatientNO, happenNO);

            //return this.myGetAllergy(strSql);
        }

        /// <summary>
        /// 插入一条过敏信息
        /// </summary>
        /// <param name="allergyInfo">过敏实体</param>
        public int InsertAllergyInfo(Neusoft.HISFC.Models.Order.Medical.AllergyInfo allergyInfo)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Order.Allergy.InsertAllergyInfo", ref strSql) == -1)
            {
                this.Err = "查找Order.Allergy.InsertAllergyInfo字段发生错误!";
            }
            
            try
            {
                string[] parm = this.myGetParmAllergy(allergyInfo);
                strSql = string.Format(strSql, parm);
            }
            catch (System.Exception ex)
            {
                this.Err = "格式化SQL语句Order.Allergy.InsertAllergyInfo出错!" + ex.Message;
            }

            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新一条过敏信息
        /// </summary>
        /// <param name="allergyInfo">过敏实体</param>
        public int UpdateAllergyInfo(Neusoft.HISFC.Models.Order.Medical.AllergyInfo allergyInfo)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Order.Allergy.UpdateAllergy", ref strSql) == -1)
            {
                this.Err = "查找Order.Allergy.UpdateAllergy字段失败" + this.Err;
            }

            try
            {
                string[] parm = this.myGetParmAllergy(allergyInfo);
                strSql = string.Format(strSql, parm);
            }
            catch (System.Exception ex)
            {
                this.Err = "格式化SQL语句Order.Allergy.UpdateAllergy发生错误!" + ex.Message;
            }

            return this.ExecNoQuery(strSql);
        }

        public int SetAllergyInfo(Neusoft.HISFC.Models.Order.Medical.AllergyInfo allergyInfo)
        {
            int parm = this.UpdateAllergyInfo(allergyInfo);
            if (parm == 0)
            {
                parm = this.InsertAllergyInfo(allergyInfo);
            }
            return parm;
        }
    }
}
