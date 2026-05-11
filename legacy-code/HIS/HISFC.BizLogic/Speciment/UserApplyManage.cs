using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 用户申请管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-12-17]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class UserApplyManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="userApply">用户申请表对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.UserApply userApply)
        {          
            string[] str = new string[]
						{
							userApply.UserAppId.ToString(), 
							userApply.UserId,
                            userApply.ApplyId.ToString(),
                            userApply.Schedule.ToString(),
                            userApply.CurDate.ToString(),
                            userApply.OperId.ToString(),
                            userApply.OperName.ToString(),
                            userApply.ScheduleId.ToString()
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
            sequence = this.GetSequence("Speciment.BizLogic.UserApplyManage.GetNextSequence");
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
            this.Err = errorText + "[" + this.Err + "]"; // + "在ShelfSpecManage.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region　用户申请记录
        /// <summary>
        /// 更新用户申请记录
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateUserApply(string sqlIndex, params string[] args)
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

        #region　共有方法
        /// <summary>
        /// 更新用户申请记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        public int UpdateUserApply(string sql)
        {
            try
            {
                return this.ExecNoQuery(sql);
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int InsertUserApply(UserApply userApply)
        {
            return this.UpdateUserApply("Speciment.BizLogic.UserApplyManage.Insert", GetParam(userApply));
        }

        /// <summary>
        /// 根据申请ID和进度情况查找记录
        /// </summary>
        /// <param name="appID">申请ID</param>
        /// <param name="schedule">进度情况</param>
        /// <returns></returns>
        public int QueryUserApply(string appID, string schedule)
        {
            return this.UpdateUserApply("Speciment.BizLogic.UserApplyManage.Query", new string[] { appID, schedule });
        }
        #endregion 

    }
}
