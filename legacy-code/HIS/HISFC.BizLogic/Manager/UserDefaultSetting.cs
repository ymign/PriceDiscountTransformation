using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Neusoft.FrameWork.Function;

namespace Neusoft.HISFC.BizLogic.Manager
{
    /// <summary>
    /// 类名称<br>ICDMedicare</br>
    /// <Font color='#FF1111'>[功能描述: 用户默认设置信息类]</Font><br></br>
    /// [创 建 者: ]<br>喻赟</br>
    /// [创建时间: ]<br>2009-04-17</br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///		/>
    /// </summary>
    public class UserDefaultSetting : DataBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserDefaultSetting()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 方法

        #region 私有

        /// <summary>
        /// 获得update或者insert的传入参数数组
        /// </summary>
        /// <param name="undrug">非药品实体</param>
        /// <returns>参数数组</returns>
        private string[] GetParams(Neusoft.HISFC.Models.Base.UserDefaultSetting setting)
        {
            string[] args = 
			{	
				setting.Empl.ID,
                setting.Setting1,
                setting.Setting2,
                setting.Setting3,
                setting.Setting4,
                setting.Setting5,
                setting.Setting6,
                setting.Setting7,
                setting.Setting8,
                setting.Setting9,
                setting.Setting10,
                setting.Oper.ID,
                setting.Oper.OperTime.ToString()
			};

            return args;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private ArrayList GetListBySql(string sql)
        {
            ArrayList alSettings = new ArrayList(); //用于返回非药品信息的数组

            //执行当前Sql语句
            if (this.ExecQuery(sql) == -1)
            {
                this.Err = this.Sql.Err;

                return null;
            }

            try
            {
                //循环读取数据
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Base.UserDefaultSetting setting = new Neusoft.HISFC.Models.Base.UserDefaultSetting();

                    setting.Empl.ID = this.Reader[0].ToString();
                    setting.Setting1 = this.Reader[1].ToString();
                    setting.Setting2 = this.Reader[2].ToString();
                    setting.Setting3 = this.Reader[3].ToString();
                    setting.Setting4 = this.Reader[4].ToString();
                    setting.Setting5 = this.Reader[5].ToString();
                    setting.Setting6 = this.Reader[6].ToString();
                    setting.Setting7 = this.Reader[7].ToString();
                    setting.Setting8 = this.Reader[8].ToString();
                    setting.Setting9 = this.Reader[9].ToString();
                    setting.Setting10 = this.Reader[10].ToString();
                    setting.Oper.ID = this.Reader[11].ToString();
                    setting.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[12].ToString());

                    alSettings.Add(setting);
                }//循环结束
            }
            catch (Exception e)
            {
                this.Err = "获得用户设置信息出错！" + e.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return alSettings;
        }

        private int SetUserSettings(Neusoft.HISFC.Models.Base.UserDefaultSetting setting, string sqlIndex)
        {
            string sql = "";

            try
            {
                //获取查询SQL语句
                if (this.GetSQL(sqlIndex, ref sql) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:" + sqlIndex + "";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1; // 出现错误返回null
            }

            string[] strParam = this.GetParams(setting);

            sql = string.Format(sql, strParam);
            return this.ExecNoQuery(sql);
        }

        #endregion

        #region 公开

        /// <summary>
        /// 查询用户设置信息
        /// </summary>
        public Neusoft.HISFC.Models.Base.UserDefaultSetting Query(string emplCode)
        {
            //定义字符变量 ,存储查询主体SQL语句
            string strSelect = "";
            //定义字符变量 ,存储WHERE 条件SQL语句
            string strWhere = "";
            //定义动态数组 ,存储查询出的信息
            ArrayList arryList = new ArrayList();
            try
            {
                //获取查询SQL语句
                if (this.GetSQL("Manager.UserDefaultSetting.Query", ref strSelect) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:Manager.UserDefaultSetting.Query";
                    return null;
                }
                //获取查询where语句
                if (this.GetSQL("Manager.UserDefaultSetting.Where", ref strWhere) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:Manager.UserDefaultSetting.Where";
                    return null;
                }
                try
                {
                    //格式化SQL语句 --------------------------------icd_code-valid_state-type-owner
                    strSelect = string.Format(strSelect + strWhere, emplCode);
                }
                catch (Exception ex)
                {
                    this.Err = "SQL语句赋值出错!" + ex.Message;
                }
                //读取数据
                arryList = this.GetListBySql(strSelect);
                if (arryList == null || arryList.Count == 0)
                {
                    this.Err = "没有找到该操作的设置信息!";
                    return null;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null; // 出现错误返回null
            }
            finally
            {
                this.Reader.Close();
            }

            return arryList[0] as Neusoft.HISFC.Models.Base.UserDefaultSetting;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Base.UserDefaultSetting setting)
        {
            return SetUserSettings(setting, "Manager.UserDefaultSetting.Insert");
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.Base.UserDefaultSetting setting)
        {
            return SetUserSettings(setting, "Manager.UserDefaultSetting.Update");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int Delete(Neusoft.HISFC.Models.Base.UserDefaultSetting setting)
        {
            return SetUserSettings(setting, "Manager.UserDefaultSetting.Delete");
        }

        #endregion

        #endregion
    }
}
