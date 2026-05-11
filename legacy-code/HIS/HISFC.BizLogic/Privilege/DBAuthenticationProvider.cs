using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.BizLogic.Privilege.Model;


namespace Neusoft.HISFC.BizLogic.Privilege
{
    /// <summary>
    /// 认证工厂
    /// </summary>
    public class DBAuthenticationProvider
    {

        public static string aa = "9CkI4pLfq2yETWrV1jm1TA==";
        /// <summary>
        /// 认证管理{D515E09B-E299-47e0-BF19-EDFDB6E4C775}
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public NeuIdentity Authenticate(string name, string password, string domain)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(name, "^[a-zA-Z0-9]*$"))
            {
                throw new Exception("账号输入不合法!");
            }


            //根据帐户获得患者信息
            User _user = SecurityService.GetUserByAccount(name);
            if (_user == null)
            {
                throw new Exception("没有该用户注册信息!");
            }

            //if (_user.OperDate > DateTime.Now.AddDays(-30)) 
            //{
            //    throw new Exception("您已经超过一个月未修改密码,请先修改密码!");
            //}

            //{D515E09B-E299-47e0-BF19-EDFDB6E4C775}
            //string pass = Neusoft.HisCrypto.HisDecrypt.Encrypt(password);
            string pass = Neusoft.HisCrypto.DESCryptoService.DESEncrypt(password,Neusoft.FrameWork.Management.Connection.DESKey);


            bool _isMatch;
            //判断密码是否相符
            _isMatch = string.Equals(_user.Password, pass);

            SecurityService ser = new SecurityService();
            string err = "";
            if (!_isMatch)
            {
                ser.UpdateLimitLogin(_user.PersonId, ref err);
                throw new Exception("输入密码不正确!");
            }
            if (ser.LimitLogin(_user.PersonId, ref err) != 1)
            {
                throw new Exception(err);
            }

            NeuIdentity _identity = new NeuIdentity(_user, "DAO", true);

            return _identity;
        }




    }
}
