using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace His.Util.Common
{
    /// <summary>
    /// DESC:记录日志,按日期滚动,程序根目录\log
    /// Creater;杨明
    /// Version：1.0.0.1
    /// Date:2015-05-15
    /// Alter:2015-06-14
    /// </summary>
    public class HisLog
    {

        public readonly static string baseUrl = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 记录程序日志
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="logMsg">日志消息体</param>
        /// <returns></returns>
        public static bool WriteLog(string type, string logMsg)
        {
            bool bo = true;
            string date = DateTime.Now.ToString("yyyyMMdd");
            string fileName = date + "_" + type + "_log.txt";
            string filePath = baseUrl + "Log\\" + fileName;
            string path = baseUrl + "Log\\";
            
            StreamWriter sw = null;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        using (sw = new StreamWriter(fs))
                        {
                            sw.WriteLine("*********************************[ "
                                + date + " ]**********************************");
                            sw.Close();
                        }
                        fs.Close();
                    }                  
                }
                if (!string.IsNullOrEmpty(logMsg))
                {
                    using (sw = new StreamWriter(filePath, true))
                    {
                        sw.WriteLine("写入时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.WriteLine(logMsg);
                        sw.WriteLine(@"***************************************************************");
                        sw.WriteLine("\r");
                    }
                    if (sw != null)
                        sw.Close();
                }
            }
            catch (Exception ex)
            {
                bo = false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
            return bo;
        }
    }
}
