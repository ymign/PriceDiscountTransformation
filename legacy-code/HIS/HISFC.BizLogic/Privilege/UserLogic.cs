using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Neusoft.HISFC.BizLogic.Privilege.Model;

namespace Neusoft.HISFC.BizLogic.Privilege
{
    /// <summary>
    /// 用户基础操作类
    /// </summary>
    public class UserLogic : DataBase
    {
        
        #region UserDAL 成员

        /// <summary>
        /// 插入角色信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(User user)
        {
            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.Org.User.Insert");
            //    _sql["userid"] = user.Id;
            //    _sql["username"] = user.Name;
            //    _sql["account"] = user.Account;
            //    _sql["password"] = user.Password;
            //    _sql["appid"] = user.AppId;
            //    _sql["personid"] = user.PersonId;
            //    _sql["description"] = user.Description;
            //    _sql["islock"] = user.IsLock;
            //    _sql["operid"] = user.UserId;
            //    _sql["operdate"] = user.OperDate;
            //    DbCommand _command = _dao.DataConnection.CreateTextCommand();
            //    SqlMapping _mapping = new Neusoft.Framework.DataAccess.SqlMapping.SqlMapping(_dao, _sql);
            //    _mapping.Mapper(_command);
            //    return _command.ExecuteNonQuery();
            //}

            string[] args = new string[] { 
                user.Id,
                user.Name,
                user.Account,
                user.Password,
                user.AppId,
                user.PersonId,
                user.Description,
                FrameWork.Function.NConvert.ToInt32( user.IsLock).ToString(),
                user.operId,
                user.OperDate.ToString(),
                //{46A2B736-8740-405a-8B0A-6DDF1B705B8D}
                Neusoft.FrameWork.Function.NConvert.ToInt32( user.IsManager).ToString()
                };
            string sql = "";
            if (this.GetSQL("SECURITY.ORG.USER.INSERT", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, args);
            }
            catch (Exception ex) { this.Err = ex.Message; return -1; }
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }
               
        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.Org.User.Delete");
            //    _sql["userid"] = userId;
            //    DbCommand _command = _dao.DataConnection.CreateTextCommand();
            //    SqlMapping _mapping = new Neusoft.Framework.DataAccess.SqlMapping.SqlMapping(_dao, _sql);
            //    _mapping.Mapper(_command);
            //    return _command.ExecuteNonQuery();
            //}
            string sql = "";
            if (this.GetSQL("SECURITY.ORG.USER.DELETE", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, userId);
            }
            catch (Exception ex) { this.Err = ex.Message; return -1; }
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;

        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public List<User> QueryUsers(List<String> users)
        {
            List<User> userList = new List<User>();
            foreach (String user in users)
            {
                //排除空用户
                if (Get(user).Id != null)
                {
                    userList.Add(Get(user));
                }
            }
            return userList;
        }

        /// <summary>
        /// 根据用户Id获取用户详细信息（张凯钧）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User Get(string userId)
        {
            User _user = new User();

            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.Org.User.GetByUserId");
            //    _sql["userid"] = userId;
            //    DbDataReader _reader = this.ExecuteReader(_sql);
            //}

            string sql = "";
            if (this.GetSQL("SECURITY.ORG.USER.GETBYUSERID", ref sql) == -1) return null;
            try
            {
                sql = string.Format(sql, userId);
            }
            catch (Exception ex) { this.Err = ex.Message; return null; }
            if (this.ExecQuery(sql) < 0) return null;
            while (this.Reader.Read())
            {
                _user = new User();
                _user.Id = Reader[0].ToString();
                _user.Name = Reader[1].ToString();
                _user.Account = Reader[2].ToString();
                _user.Password = Reader[3].ToString();
                _user.AppId = Reader[4].ToString();
                _user.PersonId = Reader[5].ToString();
                _user.Description = Reader[6].ToString();
                _user.IsLock = FrameWork.Function.NConvert.ToBoolean(Reader[7]);
                _user.operId = Reader[8].ToString();
                if (!Reader.IsDBNull(9))
                    _user.OperDate = FrameWork.Function.NConvert.ToDateTime(Reader[9].ToString());
                //{46A2B736-8740-405a-8B0A-6DDF1B705B8D}
                if (!Reader.IsDBNull(10))
                    _user.IsManager = FrameWork.Function.NConvert.ToBoolean(Reader[10].ToString());
  
            }
                Reader.Close();
            return _user;   
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User Get(string account, string password)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 获得帐号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public User GetByAccount(string account)
        {
            User _user = null;

            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.Org.GetByAccount");
            //    DbCommand _command = _dao.DataConnection.CreateTextCommand();
            //    _sql["Account"] = account;
            //    SqlMapping _map = new Neusoft.Framework.DataAccess.SqlMapping.SqlMapping(_dao, _sql);
            //    _map.Mapper(_command);
            //    DbDataReader _reader = _command.ExecuteReader();
            //}

            string sql = "";
            if (this.GetSQL("SECURITY.ORG.GETBYACCOUNT", ref sql) == -1) return null;
            try
            {
                sql = string.Format(sql, account);
            }
            catch (Exception ex) { this.Err = ex.Message; return null; }
            if (this.ExecQuery(sql) <0) return null;
                while (this.Reader.Read())
                {
                    _user = new User();
                    _user.Id = Reader[0].ToString();
                    _user.Name = Reader[1].ToString();
                    _user.Account = Reader[2].ToString();
                    _user.Password = Reader[3].ToString();
                    _user.AppId = Reader[4].ToString();
                    _user.PersonId = Reader[5].ToString();
                    _user.Description = Reader[6].ToString();
                    _user.IsLock = FrameWork.Function.NConvert.ToBoolean(Reader[7]);
                    _user.operId = Reader[8].ToString();
                    if (!Reader.IsDBNull(9))
                        _user.OperDate = FrameWork.Function.NConvert.ToDateTime(Reader[9].ToString());
                    //{46A2B736-8740-405a-8B0A-6DDF1B705B8D}
                    if (!Reader.IsDBNull(10))
                        _user.IsManager = FrameWork.Function.NConvert.ToBoolean(Reader[10].ToString());
  
                }

                Reader.Close();
            
            return _user;            
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public User GetByPsnID(string personId, string appId)
        {
            User _user = null;

            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.Org.User.GetByPsnID");
            //    DbCommand _command = _dao.DataConnection.CreateTextCommand();
            //    _sql["personid"] = personId;
            //    _sql["appid"] = appId;
            //    SqlMapping _map = new Neusoft.Framework.DataAccess.SqlMapping.SqlMapping(_dao, _sql);
            //    _map.Mapper(_command);
            //    DbDataReader _reader = _command.ExecuteReader();
            //}

            string sql = "";
            if (this.GetSQL("SECURITY.ORG.USER.GETBYPSNID", ref sql) == -1) return null;
            try
            {
                sql = string.Format(sql,personId,appId);
            }
            catch (Exception ex) { this.Err = ex.Message; return null; }
            if (this.ExecQuery(sql) < 0) return null;
                while (this.Reader.Read())
                {
                    _user = new User();
                    _user.Id = Reader[0].ToString();
                    _user.Name = Reader[1].ToString();
                    _user.Account = Reader[2].ToString();
                    _user.Password = Reader[3].ToString();
                    _user.AppId = Reader[4].ToString();
                    _user.PersonId = Reader[5].ToString();
                    _user.Description = Reader[6].ToString();
                    _user.IsLock = FrameWork.Function.NConvert.ToBoolean(Reader[7]);
                    _user.operId = Reader[8].ToString();
                    if (!Reader.IsDBNull(9))
                        _user.OperDate = FrameWork.Function.NConvert.ToDateTime(Reader[9].ToString());
                    //{46A2B736-8740-405a-8B0A-6DDF1B705B8D}
                    if (!Reader.IsDBNull(10))
                        _user.IsManager = FrameWork.Function.NConvert.ToBoolean(Reader[10].ToString());
  
                }

                Reader.Close();

            return _user;   
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(User user)
        {
            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.Org.User.Update");
            //    _sql["userid"] = user.Id;
            //    _sql["username"] = user.Name;
            //    _sql["account"] = user.Account;
            //    _sql["password"] = user.Password;
            //    _sql["appid"] = user.AppId;
            //    _sql["personid"] = user.PersonId;
            //    _sql["description"] = user.Description;
            //    _sql["islock"] = user.IsLock;
            //    _sql["operid"] = user.UserId;
            //    _sql["operdate"] = user.OperDate;
            //    DbCommand _command = _dao.DataConnection.CreateTextCommand();
            //    SqlMapping _mapping = new Neusoft.Framework.DataAccess.SqlMapping.SqlMapping(_dao, _sql);
            //    _mapping.Mapper(_command);
            //    return _command.ExecuteNonQuery();
            //}

            string[] args = new string[] { 
                user.Id,
                user.Name,
                user.Account,
                user.Password,
                user.AppId,
                user.PersonId,
                user.Description,
                FrameWork.Function.NConvert.ToInt32(user.IsLock).ToString(),
                user.operId,
                user.OperDate.ToString(),
                //{46A2B736-8740-405a-8B0A-6DDF1B705B8D}
                Neusoft.FrameWork.Function.NConvert.ToInt32(user.IsManager).ToString()
                };
            string sql = "";
            if (this.GetSQL("SECURITY.ORG.USER.UPDATE", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, args);
            }
            catch (Exception ex) { this.Err = ex.Message; return -1; }
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;

        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IList<User> Query(string roleId)
        {
            User _user = null;
            IList<User> _list = new List<User>();

            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.Org.User.GetByRoleID");
            //    DbCommand _command = _dao.DataConnection.CreateTextCommand();
            //    _sql["roleid"] = roleId;

            //    SqlMapping _map = new Neusoft.Framework.DataAccess.SqlMapping.SqlMapping(_dao, _sql);
            //    _map.Mapper(_command);
            //    DbDataReader _reader = _command.ExecuteReader();
            //}
            string sql = "";
            if (this.GetSQL("SECURITY.ORG.USER.GETBYROLEID", ref sql) == -1) return null;
            try
            {
                sql = string.Format(sql, roleId,Neusoft.FrameWork.Management.Connection.Hospital.ID);
            }
            catch (Exception ex) { this.Err = ex.Message; return null; }
            if (this.ExecQuery(sql) < 0) return null;
                while (this.Reader.Read())
                {
                    _user = new User();
                    _user.Id = Reader[0].ToString();
                    _user.Name = Reader[1].ToString();
                    _user.Account = Reader[2].ToString();
                    _user.Password = Reader[3].ToString();
                    _user.AppId = Reader[4].ToString();
                    _user.PersonId = Reader[5].ToString();
                    _user.Description = Reader[6].ToString();
                    _user.IsLock = FrameWork.Function.NConvert.ToBoolean(Reader[7]);
                    _user.operId = Reader[8].ToString();
                    if (!Reader.IsDBNull(9))
                        _user.OperDate = FrameWork.Function.NConvert.ToDateTime(Reader[9].ToString());
                    //{46A2B736-8740-405a-8B0A-6DDF1B705B8D}
                    if (!Reader.IsDBNull(10))
                        _user.IsManager = FrameWork.Function.NConvert.ToBoolean(Reader[10].ToString());
  

                    _list.Add(_user);
                }

                Reader.Close();


            return _list;            
        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <returns></returns>
        public List<User> Query()
        {
            User _user = null;
            List<User> _list = new List<User>();

            //using (DaoManager _dao = new DaoManager())
            //{
            //    AbstractSqlModel _sql = new SqlModel("Security.User.QueryAll");
            //    DbCommand _command = _dao.DataConnection.CreateTextCommand();
            //    _command.CommandText = _sql.Sql;                
            //    DbDataReader _reader = _command.ExecuteReader();
            //}
            string sql = "";
            if (this.GetSQL("SECURITY.USER.QUERYALL", ref sql) == -1) return null;
            try
            {
                sql = string.Format(sql);
            }
            catch (Exception ex) { this.Err = ex.Message; return null; }
            if (this.ExecQuery(sql) < 0) return null;
                while (this.Reader.Read())
                {
                    _user = new User();
                    _user.Id = Reader[0].ToString();
                    _user.Name = Reader[1].ToString();
                    _user.Account = Reader[2].ToString();
                    _user.Password = Reader[3].ToString();
                    _user.AppId = Reader[4].ToString();
                    _user.PersonId = Reader[5].ToString();
                    _user.Description = Reader[6].ToString();
                    _user.IsLock = FrameWork.Function.NConvert.ToBoolean(Reader[7]);
                    _user.operId = Reader[8].ToString();
                    if (!Reader.IsDBNull(9))
                        _user.OperDate = FrameWork.Function.NConvert.ToDateTime(Reader[9].ToString());
                    //{46A2B736-8740-405a-8B0A-6DDF1B705B8D}
                    if (!Reader.IsDBNull(10))
                        _user.IsManager = FrameWork.Function.NConvert.ToBoolean(Reader[10].ToString());
  

                    _list.Add(_user);
                }

                Reader.Close();
            

            return _list;

        }

        #region 用户登录限制

        /// <summary>
        /// 插入限制登录信息
        /// </summary>
        /// <param name="limitobj"></param>
        /// <returns></returns>
        public int InsertLimitLoginInfo(LimitUserLogin limitobj)
        {
            string sql = "";
            //if (this.GetSQL("SECURITY.ORG.USER.InsertLimitLogin", ref sql) == -1) return -1;
            if (this.Sql.GetSql("SECURITY.ORG.USER.InsertLimitLogin", "COM_SQL", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, limitobj.Empl_code, limitobj.Empl_name, limitobj.Login_time, limitobj.Limitlogin_time, limitobj.Relogin_time, limitobj.Times, limitobj.Lockflag);
            }
            catch (Exception ex) 
            { 
                this.Err = ex.Message; 
                return -1; 
            }
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 插入限制登录信息
        /// </summary>
        /// <param name="limitobj"></param>
        /// <returns></returns>
        public int UpdateLimitLoginInfo(LimitUserLogin limitobj)
        {
            string sql = "";
            //if (this.GetSQL("SECURITY.ORG.USER.UpdateLimitLogin", ref sql) == -1) return -1;
            if (this.Sql.GetSql("SECURITY.ORG.USER.UpdateLimitLogin", "COM_SQL", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, limitobj.Empl_code, limitobj.Empl_name, limitobj.Login_time, limitobj.Limitlogin_time, limitobj.Relogin_time, limitobj.Times, limitobj.Lockflag);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 查询用户登录限制信息
        /// </summary>
        /// <param name="Empl_code"></param>
        /// <returns></returns>
        public LimitUserLogin QueryLimitUserLogin(string Empl_code)
        {
            string sql = "";
            //if (this.GetSQL("SECURITY.ORG.USER.QueryLimitLogin", ref sql) == -1)
            if (this.Sql.GetSql("SECURITY.ORG.USER.QueryLimitLogin", "COM_SQL", ref sql) == -1)
            {
                this.Err = "[SECURITY.ORG.USER.QueryLimitLogin] 不存在!";
                return null;
            }
            try
            {
                sql = string.Format(sql, Empl_code);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            if (this.ExecQuery(sql) < 0) return null;
            LimitUserLogin limit = null;
            while (this.Reader.Read())
            {
                limit = new LimitUserLogin();
                limit.Empl_code = Reader[0].ToString();
                limit.Empl_name = Reader[1].ToString();
                limit.Login_time = Reader[2].ToString();
                limit.Limitlogin_time = Reader[3].ToString();
                limit.Relogin_time = Reader[4].ToString();
                limit.Times = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[5].ToString());
                limit.Lockflag = Reader[6].ToString();
                break;
            }

            Reader.Close();
            return limit;
        }

        /// <summary>
        /// 限制用户登录设置
        /// </summary>
        /// <param name="use">是否启用</param>
        /// <param name="PWErroTimes">时间段内,密码错误次数上限</param>
        /// <param name="LockTime">用户锁定时间</param>
        /// <param name="InTimes">时间段内</param>
        /// <returns></returns>
        public void GetLimitLoginSetting(ref string use, ref int PWErroTimes, ref int LockTime, ref int InTimes)
        {
            use = "0";
            string sql = "";
            //if (this.GetSQL("SECURITY.ORG.USER.QueryLimitLogin", ref sql) == -1)
            if (this.Sql.GetSql("SECURITY.ORG.USER.QueryLimitLoginSetting", "COM_SQL", ref sql) == -1)
            {
                this.Err = "[SECURITY.ORG.USER.QueryLimitLoginSetting] 不存在!";
                return;
            }
            if (this.ExecQuery(sql) < 0) return;

            while (this.Reader.Read())
            {
                if (Reader[0].ToString() == "1")
                {
                    use = Reader[2].ToString();
                }
                if (Reader[0].ToString() == "2")
                {
                    InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[2].ToString());
                }
                if (Reader[0].ToString() == "3")
                {
                    PWErroTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[2].ToString());
                }
                if (Reader[0].ToString() == "4")
                {
                    LockTime = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[2].ToString());
                }
            }

            Reader.Close();
        }

        #endregion
        #endregion
    }
}
