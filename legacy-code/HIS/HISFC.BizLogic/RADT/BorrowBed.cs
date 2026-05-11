using System;
using System.Collections;
using System.Data;
using System.Globalization;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.RADT;
using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.Management;
using Neusoft.FrameWork.Models;
using System.Collections.Generic;

namespace Neusoft.HISFC.BizLogic.RADT
{
    public class BorrowBed : Database
    {
        /// <summary>
        /// 向借床管理表内插入一条数据
        /// </summary>
        /// <param name="checkInfo">借床管理实体</param>
        /// <returns>0 没有更新 1 成功 －1 失败</returns>
        public int InsertBorrowBedAdd(Neusoft.HISFC.Models.RADT.BorrowBed borrowInfo)
        {
            return this.ExecNoQueryByIndex("RADT.BorrowBed.InsertBorrowBedAdd",
                                    borrowInfo.inpatient_no,			//0 住院流水号
									borrowInfo.patient_no,              //1 住院号
                                    borrowInfo.name,                    //2 姓名
                                    borrowInfo.in_date.ToString(),      //3 入院日期
                                    borrowInfo.dept_code,               //4 所在科室代码
                                    borrowInfo.dept_name,               //5 所在科室名称
                                    borrowInfo.sex_code.ID.ToString(),        //6 性别
                                    borrowInfo.from_dept_code,          //7 病床来源科室代码
                                    borrowInfo.from_dept_name,          //8 病床来源科室名称
                                    borrowInfo.from_bed_no,             //9 病床编号
                                    borrowInfo.borrow_date.ToString(),  //10 借床日期
                                    borrowInfo.return_date.ToString(),  //11 归还日期
                                    borrowInfo.oper_code,               //12 操作人
                                    borrowInfo.oper_date.ToString()     //13 操作时间
                 );
        }
        /// <summary>
        /// 借床管理-按住院流水号查
        /// </summary>
        /// <param name="inPatientNO">患者住院流水号</param>
        /// <returns>返回患者借床信息</returns>
        public ArrayList GetBorrowBedInfoByInPatientNO(string inPatientNO)
        {
            Neusoft.FrameWork.Management.DataBaseManger dbMgr = new Neusoft.FrameWork.Management.DataBaseManger();
            string strSQL = "";
            //取SELECT语句
            if (dbMgr.Sql.GetCommonSql("RADT.BorrowBed.GetBorrowBedInfoByInPatientNO", ref strSQL) == -1)
            {
                strSQL = @"select c.from_dept_code,c.from_bed_no,c.borrow_date,c.return_date from com_borrowbed_view c
                         where c.inpatient_no = '{0}' and (c.return_date = '2002-01-01 00:00:00'or c.return_date = '2001-01-01 00:00:00')";
            }
            try
            {
                strSQL = string.Format(strSQL, inPatientNO);	//替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                dbMgr.Err = "格式化SQL语句时出错RADT.BorrowBed.GetBorrowBedInfoByInPatientNO" + ex.Message;
                dbMgr.WriteErr();
                return null;
            }

            ArrayList al = new ArrayList();

            //执行查询语句
            if (dbMgr.ExecQuery(strSQL) == -1)
            {
                dbMgr.Err = "获得借床还床信息时，执行SQL语句出错！" + dbMgr.Err;
                dbMgr.ErrCode = "-1";
                return null;
            }
            try
            {
                while (dbMgr.Reader.Read())
                {
                    Neusoft.HISFC.Models.RADT.BorrowBed borrowInfo = new Neusoft.HISFC.Models.RADT.BorrowBed();
                    borrowInfo.from_dept_code = dbMgr.Reader[0].ToString();
                    borrowInfo.from_bed_no = dbMgr.Reader[1].ToString();
                    borrowInfo.borrow_date = DateTime.Parse(dbMgr.Reader[2].ToString());
                    borrowInfo.return_date = DateTime.Parse(dbMgr.Reader[3].ToString());

                    al.Add(borrowInfo);
                }
            }//抛出错误
            catch (Exception ex)
            {
                dbMgr.Err = "获得借床还床信息时出错！" + ex.Message;
                dbMgr.ErrCode = "-1";
                return null;
            }
            dbMgr.Reader.Close();

            return al;
        }

        /// <summary>
        /// 更新还床日期
        /// </summary>
        /// <param name="inpatientNO">患者住院流水号</param>
        /// <param name="returnDate">还床日期</param>
        /// <returns>成功返回1 出错返回－1 未找到数据返回0</returns>
        public int UpdateBorrowBedReturnDate(string inpatientNO, DateTime returnDate)
        {
            string strSql = string.Empty;
            if (Sql.GetCommonSql("RADT.BorrowBed.UpdateBorrowBedReturnDate", ref strSql) == -1) return -1;
            #region SQl
            strSql = "update com_borrowbed set return_date = to_date('{1}', 'yyyy-mm-dd hh24:mi:ss') where inpatient_no = '{0}'";
            #endregion
            try
            {
                strSql = string.Format(strSql, inpatientNO, returnDate.ToString());
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                ErrCode = ex.Message;
                WriteErr();
                return -1;
            }
            return ExecNoQuery(strSql);
        }
        
    }
}
