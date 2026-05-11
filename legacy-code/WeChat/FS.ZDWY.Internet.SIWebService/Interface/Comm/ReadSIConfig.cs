using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.SIWebService.Interface.Comm
{
    public static class ReadSIConfig
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public static string ServiceUrl = string.Empty;

        /// <summary>
        /// 用户
        /// </summary>
        public static string UserID = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public static string Password = string.Empty;

        /// <summary>
        /// 医院编码
        /// </summary>
        public static string HospitalCode = string.Empty;

        /// <summary>
        /// 省内医院编码
        /// </summary>
        public static string ProvHospitalCode = string.Empty;

        /// <summary>
        /// 跨省医院编码
        /// </summary>
        public static string TProvHospitalCode = string.Empty;

        /// <summary>
        /// 医院名称
        /// </summary>
        public static string HospitalName = string.Empty;

        /// <summary>
        /// 超时限制
        /// </summary>
        public static int TimeOut = 1000;

        public static string SessionId = string.Empty;

        static ReadSIConfig()
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + @"server\Insurance.Config";
            System.Xml.XmlDocument doc = null;
            if (System.IO.File.Exists(fileName))
            {
                doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(fileName);
                    HospitalCode = GetConfigValueSI(doc, "HospitalCode");
                    HospitalName = GetConfigValueSI(doc, "Hospitalname");
                    ServiceUrl = GetConfigValueSI(doc, "ServiceUrl");
                    UserID = GetConfigValueSI(doc, "UserID");
                    Password = GetConfigValueSI(doc, "PassWord");
                    TimeOut = System.Convert.ToInt32(GetConfigValueSI(doc, "TimeOut"));
                    ProvHospitalCode = GetConfigValueSI(doc, "ProvHospitalCode");
                    TProvHospitalCode = GetConfigValueSI(doc, "TProvHospitalCode");
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("加载Insurance.Config配置文件失败，原因：" + ex.Message);
                }
            }
            else
            {
                throw new System.Exception("缺少配置文件Insurance.Config");
            }
        }

        private static string GetConfigValueSI(System.Xml.XmlDocument doc, string nodeName)
        {
            System.Xml.XmlNode node = doc.SelectSingleNode(string.Format("/configuration/GDSI/{0}", nodeName));
            if (node == null)
            {
                throw new System.Exception(string.Format("加载Insurance.Config配置文件失败，原因：缺少/configuration/GDSI/{0}的配置", nodeName));
            }
            return node.InnerText.Trim();
        }

    }
}