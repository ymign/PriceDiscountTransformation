using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 原标本管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-21]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-19' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecSourceManage : Neusoft.FrameWork.Management.Database
    {       
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specSource">原标本对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecSource specSource)
        {
            specSource.OperPosCode = "";
            string[] str = new string[]{
							specSource.SpecId.ToString(), 
						    specSource.OrgOrBoold,
                            specSource.IsHis.ToString(),
                            specSource.AnticolBldCapcacity.ToString(),
                            specSource.AnticolBldCount.ToString(),
                            specSource.NonantiBldCount.ToString(),
                            specSource.NonAntiBldCapcacity.ToString(),
                            specSource.HisBarCode,
                            specSource.DiseaseType.DisTypeID.ToString(),
                            specSource.SendDoctor.ID,
                            specSource.ColDoctor.ID,
                            specSource.OperTime.ToString(),
                            specSource.OperEmp.Name,
                            specSource.SendTime.ToString(),
                            specSource.InPatientNo,//住院流水号
                            specSource.DeptNo,//来源科室
                            specSource.GetSpecPeriod,
                            specSource.OperPosCode,
                            specSource.OperPosName,
                            specSource.InDeptNo,//住院科室
                            specSource.RadScheme,
                            specSource.MedScheme,
                            specSource.MedComment,
                            specSource.MedGrp,
                            specSource.MediDoc.MainDoc.ID,
                            specSource.MediDoc.MainDoc.Name,
                            specSource.MediDoc.MainDoc1.ID,
                            specSource.MediDoc.MainDoc1.Name,
                            specSource.MediDoc.MainDoc2.Name,  
                            specSource.MediDoc.MainDoc2.ID,
                            specSource.MediDoc.MainDoc3.ID,                                                      
                            specSource.MediDoc.MainDoc3.Name,
                            specSource.Patient.PatientID.ToString(),
                            specSource.Commet,
                            specSource.TumorPor,
                            specSource.OperApplyId,
                            specSource.IsInBase.ToString(),
                            specSource.MatchFlag,
                            specSource.SpecNo,
                            specSource.IsCancer,
                            specSource.Ext1,
                            specSource.Ext2,
                            specSource.Ext3
						};
            return str;
        }

        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        public int GetNextSequence(ref string sequence)
        {
            //
            // 执行SQL语句
            //
            sequence = this.GetSequence("Speciment.BizLogic.SpecSourceManage.GetNextSequence");
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
            return 1;
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
            this.Err = errorText + "[" + this.Err + "]"; // + "在TerminalConfirm.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新原标本操作

        private SpecSource SetSpecSource()
        {
            SpecSource specSource = new SpecSource();
            try
            {
                #region 读取原标本数据
                specSource.SpecId = Convert.ToInt32(this.Reader["SPECID"].ToString());//0
                specSource.OrgOrBoold = this.Reader["ORGORBLOOD"].ToString();//1
                specSource.IsHis = Convert.ToChar(this.Reader["ISHIS"].ToString());//2
                if (!Reader.IsDBNull(3)) specSource.AnticolBldCapcacity = Convert.ToDecimal(this.Reader["ANTICOLBLDCAPACITY"].ToString());
                else specSource.AnticolBldCapcacity = 0;//3
                if (!Reader.IsDBNull(4)) specSource.AnticolBldCount = Convert.ToInt32(this.Reader["ANTICOLBLDCOUNT"].ToString());//4
                else specSource.AnticolBldCount = 0;
                if (!Reader.IsDBNull(5)) specSource.NonantiBldCount = Convert.ToInt32(this.Reader["NONANTIBLDCAPCOUNT"].ToString());//5
                else specSource.NonantiBldCount = 0;
                if (!Reader.IsDBNull(6)) specSource.NonAntiBldCapcacity = Convert.ToDecimal(this.Reader["NONANTIBLDCAP"].ToString());//6
                else specSource.NonAntiBldCapcacity = 0;
                if (!Reader.IsDBNull(7)) specSource.HisBarCode = this.Reader["HISBARCODE"].ToString();//7
                else specSource.HisBarCode = "";
                if (!Reader.IsDBNull(8)) specSource.DiseaseType.DisTypeID = Convert.ToInt32(this.Reader["DISEASETYPEID"].ToString());//8
                else specSource.DiseaseType.DisTypeID = 0;
                if (!Reader.IsDBNull(9)) specSource.SendDoctor.ID = this.Reader["SENDDOCID"].ToString();//9
                else specSource.SendDoctor.ID = "";
                if (!Reader.IsDBNull(10)) specSource.ColDoctor.ID = this.Reader["COLLECTDOCID"].ToString();//10
                else specSource.ColDoctor.ID = "";
                if (!Reader.IsDBNull(11)) specSource.OperTime = Convert.ToDateTime(this.Reader["OPERTIME"].ToString());//11
                else specSource.OperTime = DateTime.MinValue;
                if (!Reader.IsDBNull(12)) specSource.OperEmp.Name = this.Reader["OPEREMP"].ToString();//12
                else specSource.OperEmp.ID = "";
                if (!Reader.IsDBNull(13)) specSource.SendTime = Convert.ToDateTime(this.Reader["SENDDATE"].ToString());//13
                else specSource.AnticolBldCount = 0;
                if (!Reader.IsDBNull(14)) specSource.InPatientNo = this.Reader["INPATIENT_NO"].ToString();//住院流水号14
                else specSource.SendTime = DateTime.MinValue;
                if (!Reader.IsDBNull(15)) specSource.DeptNo = this.Reader["DEPTNO"].ToString();//来源科室15
                else specSource.DeptNo = "";
                if (!Reader.IsDBNull(16)) specSource.GetSpecPeriod = this.Reader["GETPEORID"].ToString();//16
                else specSource.GetSpecPeriod = "";
                if (!Reader.IsDBNull(17)) specSource.OperPosCode = this.Reader["OPERPOSCOD"].ToString();//17
                else specSource.OperPosCode = "";
                if (!Reader.IsDBNull(18)) specSource.OperPosName = this.Reader["OPERPOSNAME"].ToString();//18
                else specSource.OperPosName = "";
                if (!Reader.IsDBNull(19)) specSource.InDeptNo = this.Reader["INDEPTNO"].ToString();//住院科室19
                else specSource.InDeptNo = "";
                if (!Reader.IsDBNull(20)) specSource.RadScheme = this.Reader["RADSCHEME"].ToString();//20
                else specSource.RadScheme = "";
                if (!Reader.IsDBNull(21)) specSource.MedScheme = this.Reader["MEDSCHEME"].ToString();//21
                else specSource.MedScheme = "";
                if (!Reader.IsDBNull(22)) specSource.MedComment = this.Reader["MEDICOMMENT"].ToString();//22
                else specSource.MedComment = "";
                if (!Reader.IsDBNull(23)) specSource.MedGrp = this.Reader["MEDICALGROUP"].ToString();//23
                else specSource.MedGrp = "";
                if (!Reader.IsDBNull(24)) specSource.MediDoc.MainDoc.ID = this.Reader["MAIN_DOCTOR"].ToString();//24
                else specSource.MediDoc.MainDoc.ID = "";
                if (!Reader.IsDBNull(25)) specSource.MediDoc.MainDoc.Name = this.Reader["MAIN_DOCNAME"].ToString();//25
                else specSource.MediDoc.MainDoc.Name = "";
                if (!Reader.IsDBNull(26)) specSource.MediDoc.MainDoc1.ID = this.Reader["ASS_DOC1"].ToString();//26
                else specSource.MediDoc.MainDoc1.ID = "";
                if (!Reader.IsDBNull(27)) specSource.MediDoc.MainDoc1.Name = this.Reader["ASS_DOCNAME1"].ToString();//27
                else specSource.MediDoc.MainDoc1.Name = "";
                if (!Reader.IsDBNull(28)) specSource.MediDoc.MainDoc2.ID = this.Reader["ASS_DOC2"].ToString();//28
                else specSource.MediDoc.MainDoc2.ID = "";
                if (!Reader.IsDBNull(29)) specSource.MediDoc.MainDoc2.Name = this.Reader["ASS_DOCNAME2"].ToString();//29
                else specSource.MediDoc.MainDoc2.Name = "";
                if (!Reader.IsDBNull(30)) specSource.MediDoc.MainDoc3.ID = this.Reader["ASS_DOC3"].ToString();//30
                else specSource.MediDoc.MainDoc3.ID = "";
                if (!Reader.IsDBNull(31)) specSource.MediDoc.MainDoc3.Name = this.Reader["ASS_DOC3NAME3"].ToString();//31
                else specSource.MediDoc.MainDoc3.Name = "";
                if (!Reader.IsDBNull(32)) specSource.Patient.PatientID = Convert.ToInt32(this.Reader["PATIENTID"].ToString());//32
                else specSource.Patient.PatientID = 0;
                if (!Reader.IsDBNull(33)) specSource.Commet = this.Reader["MARK"].ToString();//33
                else specSource.Commet = "";
                if (!Reader.IsDBNull(34)) specSource.TumorPor = this.Reader["TUMORPOR"].ToString();//34
                else specSource.TumorPor = "";
                if (!Reader.IsDBNull(35)) specSource.OperApplyId = this.Reader["OPERAPPLYID"].ToString();//35
                else specSource.OperApplyId = "";
                if (!Reader.IsDBNull(36)) specSource.IsInBase = Convert.ToChar(this.Reader["ISINBASE"].ToString());//36
                else specSource.IsInBase = '0';
                if (!Reader.IsDBNull(37)) specSource.MatchFlag = this.Reader["MATCHFLAG"].ToString();//36
                else specSource.MatchFlag = "0";
                if (!Reader.IsDBNull(41))
                specSource.Ext2 = this.Reader["EXT_2"].ToString();//41
                try
                {
                    specSource.SpecNo = this.Reader["SPEC_NO"].ToString();//                   
                }
                catch
                { }
                #endregion
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return specSource;

        }

        /// <summary>
        /// 更新原标本
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSpecSource(string sqlIndex, params string[] args)
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

        /// <summary>
        /// 根据sql索引查找符合条件的原标本
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns>原标本列表</returns>
        private ArrayList GetSpecSource(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrSpecSource = new ArrayList();
            while (this.Reader.Read())
            {
                SpecSource specTmp = SetSpecSource();
                arrSpecSource.Add(specTmp);
            }
            Reader.Close();
            return arrSpecSource;
        }
        #endregion

        #endregion

        #region 公共方法

        /// <summary>
        /// 根据sql查询标本源
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetSpecSource(string sql)
        {
            if (this.ExecQuery(sql) == -1)
            {
                return null;
            }
            ArrayList arrResult = new ArrayList();
            while (this.Reader.Read())
            {
                SpecSource specTmp = SetSpecSource();
                arrResult.Add(specTmp);
            }
            this.Reader.Close();
            return arrResult;
        }

        /// <summary>
        /// 原标本插入
        /// </summary>
        /// <param name="specBox">即将插入的标本</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertSpecSource(Neusoft.HISFC.Models.Speciment.SpecSource specSource)
        {
            try
            {
                return this.UpdateSpecSource("Speciment.BizLogic.SpecSourceManage.Insert", this.GetParam(specSource));
            }
            catch(Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新标本源
        /// </summary>
        /// <param name="specSource"></param>
        /// <returns></returns>
        public int UpdateSpecSource(SpecSource specSource)
        {
            try
            {
                return this.UpdateSpecSource("Speciment.BizLogic.SpecSourceManage.Update", this.GetParam(specSource));
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新标本录入诊断标志
        /// </summary>
        /// <param name="specID"></param>
        /// <returns></returns>
        public int UpdateInBase(string specID)
        {
            try
            {
                return this.UpdateSpecSource("Speciment.BizLogic.SpecSourceManage.UpdateInBase", new string[] { specID });
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 根据标本源ID或者条形码获取原标本
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public SpecSource GetSource(string specId, string barCode)
        {
            ArrayList arr=new ArrayList();
            if (specId != "")
            {
                arr = this.GetSpecSource("Speciment.BizLogic.SpecSourceManage.GetSourceById", new string[] { specId });                
            }
            if (barCode != "")
            {
                arr = this.GetSpecSource("Speciment.BizLogic.SpecSourceManage.GetSourceByBarCode", new string[] { barCode });
            }
            if (arr == null || arr.Count <= 0)
            {
                return null;
            }
            return arr[0] as SpecSource;

        }

        public bool ChkSpecExsit(string specID)
        {
            ArrayList arrResult = new ArrayList();
            arrResult = this.GetSpecSource("Speciment.BizLogic.SpecSourceManage.ChkSpecSoureExsit", new string[] { specID });
            if (arrResult.Count >= 1)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 在HIS中获取手术医生的信息
        /// </summary>
        /// <param name="operationNo">手术申请单号</param>
        /// <returns></returns>
        public SpecSource GetSendDocInfoInHis(string operationNo)
        {
            string sql = string.Empty;

            if (this.ExecQueryByIndex("Speciment.BizLogic.SpecSourceManage.GetSendDocInfoInHis1", new string[] { operationNo }) == -1)
                return null;
            SpecSource specSource = new SpecSource();
            int count = 0;
            while (this.Reader.Read())
            {
                if (!Reader.IsDBNull(2)) specSource.MediDoc.MainDoc.Name = this.Reader["MAINNAME"].ToString();//12
                else specSource.MediDoc.MainDoc.Name = "";
                if (!Reader.IsDBNull(3)) specSource.MediDoc.MainDoc.ID =this.Reader["MAINDOCID"].ToString();
                else specSource.AnticolBldCapcacity = 0;//3
                if (!Reader.IsDBNull(4)) specSource.DeptNo = this.Reader["DEPT_CODE"].ToString();//4
                else specSource.DeptNo = "";
                if (!Reader.IsDBNull(5)) specSource.MediDoc.MainDoc1.ID = this.Reader["H1ID"].ToString();//5
                else specSource.MediDoc.MainDoc1.ID = "";
                if (!Reader.IsDBNull(6)) specSource.MediDoc.MainDoc1.Name = this.Reader["H1NAME"].ToString();//6
                else specSource.MediDoc.MainDoc1.Name = "";
                if (!Reader.IsDBNull(7)) specSource.MediDoc.MainDoc2.Name = this.Reader["H2NAME"].ToString();//7
                else specSource.MediDoc.MainDoc2.Name = "";
                if (!Reader.IsDBNull(8)) specSource.MediDoc.MainDoc2.ID = this.Reader["H2ID"].ToString();//8
                else specSource.MediDoc.MainDoc2.ID = "";
                if (!Reader.IsDBNull(9)) specSource.MediDoc.MainDoc3.Name = this.Reader["H3NAME"].ToString();//9
                else specSource.MediDoc.MainDoc3.Name = "";
                if (!Reader.IsDBNull(10)) specSource.MediDoc.MainDoc3.ID = this.Reader["H3ID"].ToString();//10
                else specSource.MediDoc.MainDoc3.ID = "";
                count++;
            }
            Reader.Close();
            if (count > 0)
                return specSource;            
            return null;
           
        }

        /// <summary>
        /// 获取组织和血都有的标本
        /// </summary>
        /// <returns></returns>
        public ArrayList GetMatch()
        {
            return this.GetSpecSource("Speciment.BizLogic.SpecSourceManage.MatchFlagTriger", new string[] { });
        }

        /// <summary>
        /// 根据标本号更新配对标志
        /// </summary>
        /// <param name="matchFlag">配对标志</param>
        /// <param name="specId">标本号</param>
        /// <returns></returns>
        public int UpdateMatchFlag(string matchFlag,string specId)
        {
            return this.UpdateSpecSource("Speciment.BizLogic.SpecSourceManage.MatchFlagUpdate", new string[] { matchFlag, specId });
        }

        public SpecSource GetBySubBarCode(string subBarCode, string subSpecId)
        {
            ArrayList arr = this.GetSpecSource("Speciment.BizLogic.SpecSourceManage.GetSourceBySub", new string[] { subBarCode, subSpecId });
            try
            {
                return arr[0] as SpecSource;
            }
            catch
            {
                return null;
            } 
        }

        #endregion
    }
}
