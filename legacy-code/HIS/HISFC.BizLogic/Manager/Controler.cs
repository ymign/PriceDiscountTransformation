using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Manager
{
	/// <summary>
	/// Controler 的摘要说明。
	/// </summary>
	public class Controler:Neusoft.FrameWork.Management.Database
	{
		public Controler()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

        /// <summary>
        /// 控制参数缓存 Key ControlCode  Value ControlValue
        /// </summary>
        private static Dictionary<string, string> controlDictionary = new Dictionary<string, string>();

		/// <summary>
		/// 添加控制信息
		/// </summary>
		/// <param name="Controler">控制参数信息</param>
		/// <returns>成功返回1失败返回-1</returns>
		public int AddControlerInfo(Neusoft.HISFC.Models.Base.ControlParam Controler)
		{
			string strSql = "";
            if (this.Sql.GetSql( "AddControlerInfo.1", ref strSql ) == -1)
            {
                return -1;
            }

			try
			{
				//0控制参数代码1控制参数名称2控制参数值3显示标记4操作员5操作时间
				strSql = string.Format(strSql,Controler.ID,Controler.Name,Controler.ControlerValue,Neusoft.FrameWork.Function.NConvert.ToInt32(Controler.IsVisible).ToString(),
					this.Operator.ID,this.GetSysDateTime());
			}
			catch(Exception ex)
			{
				this.Err=ex.Message;
				this.ErrCode=ex.Message;
				return -1;
			}
			return this.ExecNoQuery(strSql);
		}

		/// <summary>
		/// 更新控制信息
		/// </summary>
		/// <param name="Controler">控制参数信息</param>
		/// <returns>成功返回1 失败返回-1</returns>
		public int UpdateControlerInfo(Neusoft.HISFC.Models.Base.ControlParam Controler)
		{
			string strSql = "";
            if (this.Sql.GetSql( "UpdateControlerInfo.1", ref strSql ) == -1)
            {
                return -1;
            }

			try
			{
				//0控制参数代码1控制参数名称2控制参数值3显示标记4操作员5操作时间
                strSql = string.Format( strSql, Controler.ID, Controler.Name, Controler.ControlerValue, Neusoft.FrameWork.Function.NConvert.ToInt32( Controler.IsVisible ).ToString(),
					this.Operator.ID);
			}
			catch(Exception ex)
			{
				this.Err=ex.Message;
				this.ErrCode=ex.Message;
				return -1;
			}
			return this.ExecNoQuery(strSql);
		}

		/// <summary>
		/// 检索控制信息 只显示让客户可以看见的信息
		/// </summary>
		/// <returns></returns>
		public ArrayList QueryControlerInfo()
		{
			string strSql = "";
			ArrayList al = new ArrayList();
            if (this.Sql.GetSql( "QueryControlerInfo.1", ref strSql ) == -1)
            {
                return null;
            }

            if (this.ExecQuery( strSql ) == -1)
            {
                return null;
            }

			//0控制参数代码1控制参数名称2控制参数值3显示标记
			while (this.Reader.Read())
			{
				Neusoft.HISFC.Models.Base.ControlParam Controler = new Neusoft.HISFC.Models.Base.ControlParam();
                try
                {
                    Controler.ID = this.Reader[0].ToString();
                    Controler.Name = this.Reader[1].ToString();
                    Controler.ControlerValue = this.Reader[2].ToString();
                    Controler.IsVisible = Neusoft.FrameWork.Function.NConvert.ToBoolean( this.Reader[3].ToString() );
                    Controler.User01 = this.Reader[4].ToString();
                    Controler.User02 = this.Reader[5].ToString();
                }
                catch (Exception ex)
                {
                    this.Err = "查询控制信息赋值错误!" + ex.Message;
                    return null;
                }
                finally
                {
                    this.Reader.Close();
                }
				al.Add(Controler);
			}

			return al;
		}

        /// <summary>
        /// 根据控制类代码检索控制类型的值
        /// 不重新从数据库中取
        /// </summary>
        /// <param name="controlCode"></param>
        /// <returns></returns>
        public string QueryControlerInfo(string controlCode)
        {
            return this.QueryControlerInfo( controlCode, false );
        }

        /// <summary>
        /// 根据控制类代码检索控制类型的值
        /// </summary>
        /// <param name="controlCode"></param>
        /// <param name="isRefresh"></param>
        /// <returns></returns>
        public string QueryControlerInfo(string controlCode, bool isRefresh)
        {
            //不重新取
            if (isRefresh == false)
            {
                if (controlDictionary.ContainsKey( controlCode ) == true)         //已包含
                {
                    return controlDictionary[controlCode];
                }
            }

            string strSql = "";
            if (this.Sql.GetSql( "QueryControlerInfo.2", ref strSql ) == -1)
            {
                return "";
            }
            try
            {
                //0控制参数代码
                strSql = string.Format( strSql, controlCode );
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return "";
            }
            string strValue = this.ExecSqlReturnOne( strSql );

            if (controlDictionary.ContainsKey( controlCode ) == false)         //不包含该对应 添加到缓存内
            {
                controlDictionary.Add( controlCode, strValue );
            }

            return strValue;
        }

		/// <summary>
		/// 获得整个控制类信息
		/// </summary>
		/// <param name="ctrlCode"></param>
		/// <returns></returns>
        public Neusoft.HISFC.Models.Base.ControlParam QueryControlInfoByCode(string ctrlCode)
        {
            string strSql = "";

            if (this.Sql.GetSql( "QueryControlInfoByCode", ref strSql ) == -1)
            {
                return null;
            }

            strSql = string.Format( strSql, ctrlCode );
            if (this.ExecQuery( strSql ) == -1)
            {
                return null;
            }
            Neusoft.HISFC.Models.Base.ControlParam Controler = null;
            //0控制参数代码1控制参数名称2控制参数值3显示标记
            while (this.Reader.Read())
            {
                Controler = new Neusoft.HISFC.Models.Base.ControlParam();
                try
                {
                    Controler.ID = this.Reader[0].ToString();
                    Controler.Name = this.Reader[1].ToString();
                    Controler.ControlerValue = this.Reader[2].ToString();
                    Controler.IsVisible = Neusoft.FrameWork.Function.NConvert.ToBoolean( this.Reader[3].ToString() );
                    Controler.User01 = this.Reader[4].ToString();
                    Controler.User02 = this.Reader[5].ToString();
                }
                catch (Exception ex)
                {
                    this.Err = "查询控制信息赋值错误!" + ex.Message;

                    return null;
                }
                finally
                {
                    this.Reader.Close();
                }
            }

            return Controler;
        }

		/// <summary>
		/// 获得整个控制类信息
		/// </summary>
		/// <param name="ctrlName"></param>
		/// <returns></returns>
		public Neusoft.HISFC.Models.Base.ControlParam QueryControlInfoByName(string ctrlName)
		{
			string strSql = "";

            if (this.Sql.GetSql( "QueryControlInfoByName", ref strSql ) == -1)
            {
                return null;
            }

			strSql = string.Format(strSql,ctrlName);
            if (this.ExecQuery( strSql ) == -1)
            {
                return null;
            }
			Neusoft.HISFC.Models.Base.ControlParam Controler = null;
			//0控制参数代码1控制参数名称2控制参数值3显示标记
			while (this.Reader.Read())
			{
				Controler = new Neusoft.HISFC.Models.Base.ControlParam();
                try
                {
                    Controler.ID = this.Reader[0].ToString();
                    Controler.Name = this.Reader[1].ToString();
                    Controler.ControlerValue = this.Reader[2].ToString();
                    Controler.IsVisible = Neusoft.FrameWork.Function.NConvert.ToBoolean( this.Reader[3].ToString() );
                    Controler.User01 = this.Reader[4].ToString();
                    Controler.User02 = this.Reader[5].ToString();
                }
                catch (Exception ex)
                {
                    this.Err = "查询控制信息赋值错误!" + ex.Message;
                    return null;
                }
                finally
                {
                    this.Reader.Close();
                }
			}

			return Controler;
		}

		public ArrayList QueryControlInfoByKind(string Kind)
		{
			string strSql = "";
			ArrayList al = new ArrayList();
            if (this.Sql.GetSql( "QueryControlInfoByKind", ref strSql ) == -1)
            {
                return null;
            }

			strSql = string.Format(strSql,Kind);
            if (this.ExecQuery( strSql ) == -1)
            {
                return null;
            }

			//0控制参数代码1控制参数名称2控制参数值3显示标记
			while (this.Reader.Read())
			{
				Neusoft.HISFC.Models.Base.ControlParam Controler = new Neusoft.HISFC.Models.Base.ControlParam();
                try
                {
                    Controler.ID = this.Reader[0].ToString();
                    Controler.Name = this.Reader[1].ToString();
                    Controler.ControlerValue = this.Reader[2].ToString();
                    Controler.IsVisible = Neusoft.FrameWork.Function.NConvert.ToBoolean( this.Reader[3].ToString() );
                    Controler.User01 = this.Reader[4].ToString();
                    Controler.User02 = this.Reader[5].ToString();
                }
                catch (Exception ex)
                {
                    this.Err = "查询控制信息赋值错误!" + ex.Message;
                    this.ErrCode = ex.Message;
                    return null;
                }
                finally
                {
                    this.Reader.Close();
                }
				al.Add(Controler);
			}

			return al;
		}

        public DataSet selFINPatientHide(string CODE)
        {
            DataSet dataSet = new DataSet();
            string text = string.Format("select * from FIN_Patient_Hide  where EMPL_CODE='{0}'", CODE);
            DataSet result;
            if (base.ExecQuery(text) == -1)
            {
                result = null;
            }
            else
            {
                base.ExecQuery(text, ref dataSet);
                result = dataSet;
            }
            return result;
        }

        public int SelHide(string CODE)
        {
            DataSet dataSet = new DataSet();
            string text = string.Format("select * from FIN_Patient_Hide  where EMPL_CODE='{0}'", CODE);
            int result;
            if (base.ExecQuery(text) == -1)
            {
                result = 0;
            }
            else
            {
                base.ExecQuery(text, ref dataSet);
                IEnumerator enumerator = dataSet.Tables[0].Rows.GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        DataRow dataRow = (DataRow)enumerator.Current;
                        result = 1;
                        return result;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                result = 0;
            }
            return result;
        }

        public DataSet selCOM_EMPLOYEE(string CODE)
        {
            DataSet dataSet = new DataSet();
            string text = string.Format("select * from COM_EMPLOYEE where EMPL_NAME='{0}' or EMPL_CODE='{0}'", CODE);
            DataSet result;
            if (base.ExecQuery(text) == -1)
            {
                result = null;
            }
            else
            {
                base.ExecQuery(text, ref dataSet);
                result = dataSet;
            }
            return result;
        }

        public int AddCOM_EMPLOYEE(string Empl_Code, string name, string TEXT1, string TEXT2, string TEXT3, string TEXT4, string TEXT5, string TEXT6, string TEXT7, string TEXT8, string TEXT9, string TEXT10, string VALID_STATE, string CONFIRMDATE)
        {
            string text = @"insert into FIN_Patient_Hide
  (id,
  EMPL_CODE,
   EMPL_NAME,
   TEXT1,
   TEXT2,
   TEXT3,
   TEXT4,
   TEXT5,
   TEXT6,
   TEXT7,
   TEXT8,
   TEXT9,
   TEXT10,
   VALID_STATE,
   CONFIRMDATE)  values
  (SEQ_PATINET_HIDE.Nextval,
  '{0}',
   '{1}',
   '{2}',
   '{3}',
   '{4}',
   '{5}',
   '{6}',
   '{7}',
   '{8}',
   '{9}',
   '{10}',
   '{11}',
   '{12}',
   to_date('{13}', 'yyyy-mm-dd hh24:mi:ss'))
";
            text = string.Format(text, new object[]
			{
				Empl_Code,
				name,
				TEXT1,
				TEXT2,
				TEXT3,
				TEXT4,
				TEXT5,
				TEXT6,
				TEXT7,
				TEXT8,
				TEXT9,
				TEXT10,
				VALID_STATE,
				CONFIRMDATE
			});
            return base.ExecQuery(text);
        }

        public int UpadteHide(string Empl_Code, string TEXT1, string TEXT2, string TEXT3, string TEXT4, string TEXT5, string TEXT6, string TEXT7, string TEXT8, string CONFIRMDATE)
        {
            string text = @"update FIN_Patient_Hide
   set TEXT1       = '{0}',
       TEXT2       = '{1}',
       text3       = '{2}',
       text4       = '{3}',
       text5       = '{4}',
       text6       = '{5}',
       text7       = '{6}',
       text8       = '{7}',
       confirmdate = to_date('{8}', 'yyyy-mm-dd hh24:mi:ss')
 where EMPL_CODE = '{9}'
";
            text = string.Format(text, new object[]
			{
				TEXT1,
				TEXT2,
				TEXT3,
				TEXT4,
				TEXT5,
				TEXT6,
				TEXT7,
				TEXT8,
				CONFIRMDATE,
				Empl_Code
			});
            return base.ExecQuery(text);
        }
	
	}
}
