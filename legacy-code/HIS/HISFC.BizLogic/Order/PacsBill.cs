using System;
using System.Collections;
using Neusoft.FrameWork.Function;

namespace Neusoft.HISFC.BizLogic.Order
{
	/// <summary>
	/// 检查管理类
	/// written by zuowy 
	/// 2005-8-20
	/// </summary>
	public class PacsBill:Neusoft.FrameWork.Management.Database 
	{
        public PacsBill()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 保存时的判断
        /// </summary>
        /// <param name="pacsbill"></param>
        public int SetPacsBill(Neusoft.HISFC.Models.Order.PacsBill pacsbill)
        {
            int Parm;
            Parm = this.UpdatePacsBill(pacsbill);
            if (Parm == 0)
                Parm = this.SavePacsBill(pacsbill);
            return Parm;
        }
        /// <summary>
        /// 查询检查单信息
        /// </summary>
        /// <param name="combNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.PacsBill QueryPacsBill(string combNo)
        {
            # region 查询检查单信息
            // 查询检查单信息
            // Management.Order.SelectPacsBill
            // 传入 1 传出 11
            # endregion
            string strSql = "";
            ArrayList al = null;
            if (this.Sql.GetCommonSql("Management.Order.QueryResourceByPacsBillNo", ref strSql) == -1)
            {
                this.Err = "没有找到Management.Order.QueryResourceByPacsBillNo字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            strSql = string.Format(strSql, combNo);
            al = this.myPacsBillQuery(strSql);
            if (al == null || al.Count == 0) return null;
            return al[0] as Neusoft.HISFC.Models.Order.PacsBill;
        }

        public Neusoft.HISFC.Models.Order.PacsBill QueryPacsApply(string interfaceCode)
        {
            # region 查询检查单信息
            // 查询检查单信息
            // Management.Order.SelectPacsBill
            // 传入 1 传出 11
            # endregion
            string strSql = "";
            ArrayList al = null;
            if (this.Sql.GetCommonSql("Management.Order.interfaceCode", ref strSql) == -1)
            {
                this.Err = "没有找到Management.Order.interfaceCode字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            strSql = string.Format(strSql, interfaceCode);
            al = this.myPacsBillQuery(strSql);
            if (al == null || al.Count == 0) return null;
            return al[0] as Neusoft.HISFC.Models.Order.PacsBill;
        }
        /// <summary>
        /// 根据科室代码查申请信息
        /// </summary>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public ArrayList QueryPacsBillByDept(string deptCode, DateTime dtBegin, DateTime dtEnd)
        {
            # region 查询检查单信息
            // 查询检查单信息
            // Management.Order.SelectPacsBill
            // 传入 1 传出 11
            # endregion
            string strSql = "";
            ArrayList al = null;
            if (this.Sql.GetCommonSql("Management.Order.QueryResourceByDeptCode", ref strSql) == -1)
            {
                this.Err = "没有找到Management.Order.QueryResourceByDeptCode字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            strSql = string.Format(strSql, deptCode, dtBegin, dtEnd);
            al = this.myPacsBillQuery(strSql);

            return al;
        }
        /// <summary>
        /// 根据卡号查申请信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public ArrayList QueryPacsBillByCardNo(string cardNo, DateTime dtBegin, DateTime dtEnd)
        {
            # region 查询检查单信息
            // 查询检查单信息
            // Management.Order.SelectPacsBill
            // 传入 1 传出 11
            # endregion
            string strSql = "";
            ArrayList al = null;
            if (this.Sql.GetCommonSql("Management.Order.QueryPacsBillByCardNo", ref strSql) == -1)
            {
                this.Err = "没有找到Management.Order.QueryPacsBillByCardNo字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            strSql = string.Format(strSql, cardNo, dtBegin, dtEnd);
            al = this.myPacsBillQuery(strSql);

            return al;
        }
        /// <summary>
        /// 开立新的检查单
        /// </summary>
        /// <param name="PacsBill"></param>
        /// <returns></returns>
        public int SavePacsBill(Neusoft.HISFC.Models.Order.PacsBill PacsBill)
        {
            // 开立新的检查单
            // Management.Order.InsertPacsBill
            // 传入 12 传出 0
            string strSql = "";
            if (this.Sql.GetCommonSql("Management.Order.InsertPacsBill", ref strSql) == -1) return -1;
            strSql = this.getPacsBillInfo(strSql, PacsBill);

            if (strSql == null)
            {
                this.Err = "格式化Sql语句时出错";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 更新检查单信息
        /// </summary>
        /// <param name="pacsbill"></param>
        /// <returns></returns>
        public int UpdatePacsBill(Neusoft.HISFC.Models.Order.PacsBill pacsbill)
        {
            // 更新检查单
            // Management.Order.UpdatePacsBill
            // 传入 12 传出 0
            string strSql = "";
            if (this.Sql.GetCommonSql("Management.Order.UpdatePacsBill", ref strSql) == -1) return -1;
            strSql = this.getPacsBillInfo(strSql, pacsbill);

            if (strSql == null)
            {
                this.Err = "格式化Sql语句时出错";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 更新检查单信息
        /// </summary>
        /// <param name="pacsbill"></param>
        /// <returns></returns>
        public int UpdatePacsBillState(Neusoft.HISFC.Models.Order.PacsBill pacsbill)
        {
            // 更新检查单
            // Management.Order.UpdatePacsBill
            // 传入 12 传出 0
            string strSql = "";
            if (this.Sql.GetCommonSql("Management.Order.UpdatePacsBillState", ref strSql) == -1) return -1;
            strSql = System.String.Format(strSql, pacsbill.ComboNO, this.Operator.ID);

            if (strSql == null)
            {
                this.Err = "格式化Sql语句时出错";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="PacsId"></param>
        /// <returns></returns>
        public int DeletePacsBill(string PacsId)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Management.Order.deletePacsBill", ref strSql) == -1)
                return -1;
            strSql = string.Format(strSql, PacsId);
            if (strSql == null)
                return -1;
            return this.ExecNoQuery(strSql);
        }
      
        public int DeletePacsApply(string interfaceCode)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Management.Order.deletePacsBill.1", ref strSql) == -1)
                return -1;
            strSql = string.Format(strSql, interfaceCode);
            if (strSql == null)
                return -1;
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 获得检查单信息
        /// </summary>
        /// <param name="pacsbill"></param>
        /// <returns></returns>
        public string getPacsBillInfo(string strSql, Neusoft.HISFC.Models.Order.PacsBill pacsbill)
        {
            # region "接口说明"
            // 0 检查单号       1 检查单名称      2 住院流水号 3 科室编码 
            // 4 科室名称       5 检查部位/目的   6 病史及特征
            // 7 实验室检查结果 8 注意事项        9 诊断 10 备注
            // 11 操作员        12 操作日期
            # endregion
            int patientType = 0;
            int his_PatientType = 1;
            if (pacsbill.PatientType == Neusoft.HISFC.Models.Order.PatientType.OutPatient)
            {
                patientType = 1;
                his_PatientType = 1;
            }
            else if (pacsbill.PatientType == Neusoft.HISFC.Models.Order.PatientType.InPatient)
            {
                his_PatientType = 2;
            }
            try
            {
                System.Object[] s = {pacsbill.ComboNO,//检查单号
										pacsbill.BillName,//检查单名称
										pacsbill.PatientNO,//住院流水号
										pacsbill.Dept.ID,//科室代码
										pacsbill.Dept.Name,//科室名称
										pacsbill.CheckOrder,//检查部位/目的
										pacsbill.IllHistory,//病史检查及特征
										pacsbill.LisResult,//检查结果
										pacsbill.Caution,//注意事项
										pacsbill.Diagnose1,//诊断1
										pacsbill.Memo,//备注
										pacsbill.Oper.ID,//操作员
										pacsbill.ApplyDate,//操作日期
					                    pacsbill.Doctor.ID,//申请医师代码
					                    pacsbill.Doctor.Name,//申请医师姓名
					                    pacsbill.TotCost.ToString(),//项目金额
					                    //pacsbill.PatientNo.Length>=7?pacsbill.PatientNo.Substring(pacsbill.PatientNo.Length - 7):pacsbill.PatientNo,//接口
										pacsbill.User01,
					                    patientType.ToString(),//患者类别
					                    pacsbill.Diagnose2,//诊断2
					                    pacsbill.Diagnose3,//诊断3
					                    pacsbill.MachineType,//设备类型
					                    pacsbill.CheckBody,//检查部位
					                    his_PatientType.ToString(),
					                    pacsbill.ItemCode,
										pacsbill.ExeDept,
					                    pacsbill.ClinicCode,
										pacsbill.PacsItem,
										pacsbill.SampleDate,
										pacsbill.LastMensesDate,
										pacsbill.IsMenopause == true?"1":"0",
										pacsbill.Exec_sqn,//执行单流水号
					                    pacsbill.Antiviotic1,//抗生素1
					                    pacsbill.Antiviotic2,//抗生素2
					                    pacsbill.Temperature,//体温
					                    pacsbill.SpecimenType//样本类型
									};
                string myErr = "";
                if (Neusoft.FrameWork.Public.String.CheckObject(out myErr, s) == -1)
                {
                    this.Err = myErr;
                    this.WriteErr();
                    return null;
                }
                strSql = string.Format(strSql, s);
            }
            catch (Exception ex)
            {
                this.Err = "付数值时候出错！" + ex.Message;
                this.WriteErr();
                return null;
            }
            return strSql;
        }
        /// <summary>
        /// 获得检查单信息实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public ArrayList myPacsBillQuery(string strSql)
        {
            ArrayList al = new ArrayList();

            if (this.ExecQuery(strSql) == -1) return null;
            Neusoft.HISFC.Models.Order.PacsBill pacsbill;// = new neusoft.HISFC.Object.Order.PacsBill();
            try
            {
                while (this.Reader.Read())
                {
                    try
                    {
                        pacsbill = new Neusoft.HISFC.Models.Order.PacsBill();
                        pacsbill.ComboNO = this.Reader[0].ToString();//检查单号
                        pacsbill.BillName = this.Reader[1].ToString();//检查单名称
                        pacsbill.PatientNO = this.Reader[2].ToString();//住院流水号
                        pacsbill.Dept.Name = this.Reader[3].ToString();//科室名称
                        pacsbill.CheckOrder = this.Reader[4].ToString();//检查部位/目的
                        pacsbill.IllHistory = this.Reader[5].ToString();//病史检查及特征
                        pacsbill.LisResult = this.Reader[6].ToString();//检查结果
                        pacsbill.Caution = this.Reader[7].ToString();//注意事项
                        pacsbill.Diagnose1 = this.Reader[8].ToString();//诊断1
                        pacsbill.Memo = this.Reader[9].ToString();//备注
                        pacsbill.Doctor.Name = this.Reader[10].ToString();//操作员
                        pacsbill.ApplyDate = this.Reader[11].ToString();//申请日期
                        pacsbill.User02 = this.Reader[12].ToString();//是否确认
                        pacsbill.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[13].ToString());//有效状态
                        pacsbill.TotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[14].ToString());//金额
                        pacsbill.Diagnose2 = this.Reader[15].ToString();//诊断2
                        pacsbill.Diagnose3 = this.Reader[16].ToString();//诊断3
                        pacsbill.MachineType = this.Reader[17].ToString();//设备类型
                        pacsbill.CheckBody = this.Reader[18].ToString();//检查部位
                        pacsbill.ItemCode = this.Reader[19].ToString();//项目编码
                        pacsbill.Dept.ID = this.Reader[20].ToString();//科室代码
                        pacsbill.ExeDept = this.Reader[21].ToString();//执行科室
                        pacsbill.ClinicCode = this.Reader[22].ToString();//流水号
                        pacsbill.PacsItem = this.Reader[23].ToString();//医技项目
                        pacsbill.SampleDate = this.Reader[24].ToString();
                        pacsbill.LastMensesDate = this.Reader[25].ToString();
                        pacsbill.IsMenopause = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[26].ToString());
                        pacsbill.Exec_sqn = this.Reader[27].ToString();//执行单流水号
                        pacsbill.Antiviotic1 = this.Reader[28].ToString();//抗生素1
                        pacsbill.Antiviotic2 = this.Reader[29].ToString();//抗生素2
                        pacsbill.Temperature = this.Reader[30].ToString();//体温
                        pacsbill.SpecimenType = this.Reader[31].ToString();//样本类型
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得检查单信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    al.Add(pacsbill);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得检查单信息出错！" + ex.Message;
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return al;
        }



	}
}
