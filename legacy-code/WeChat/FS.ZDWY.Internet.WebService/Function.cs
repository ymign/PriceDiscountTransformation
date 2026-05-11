using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    public class Function
    {
        /// <summary>
        /// 出参模板
        /// </summary>
        private static readonly string xmlResponse = @"
<Response>
    <ok>{0}</ok>
    <errorMsg>{1}</errorMsg>
    <data>
        {2}
    </data>
</Response>";

        /// <summary>
        /// 组织出参XML
        /// </summary>
        /// <param name="xml">XML</param>
        /// <param name="funName">函数名</param>
        /// <returns></returns>
        public static string GetResponseXML(bool isOk, string message, string dataXml)
        {
            return string.Format(xmlResponse, isOk.ToString(), message, dataXml);
        }

        /// <summary>
        /// 获取标签对的值
        /// </summary>
        /// <param name="xmlDoc">XML文档</param>
        /// <param name="xpath">xml表达式</param>
        /// <returns></returns>
        public static string GetNoteValue(XmlDocument xmlDoc, string xpath)
        {
            //var node = xmlDoc.SelectSingleNode(xpath);
            //if (node == null)
            //{
            //    throw new Exception("没有找到xml节点。" + xpath);
            //}
            //return node.InnerText;
            var node = xmlDoc.SelectSingleNode(xpath);
            if (node == null)
            {
                throw new Exception("没有找到xml节点。" + xpath);
            }

            // 获取节点文本内容
            string nodeValue = node.InnerText;

            // SQL注入检测和过滤
            if (IsSqlInjectionDetected(nodeValue))
            {
                throw new Exception($"检测到异常参数，节点值: {nodeValue}");
            }

            return nodeValue;
        }


        private static bool IsSqlInjectionDetected(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            string pattern = "";

            // 1. 检测SQL注释块
            pattern += @"/\*.*?\*/|";          // /*注释*/ 形式的SQL注释块


            // 3. 检测SQL关键字（使用单词边界\b确保匹配完整单词）
            pattern += @"\b(select|insert|update|delete|";
            pattern += @"drop|truncate|exec|xp_|DBMS_\w+)\b|";

            // 4. 检测逻辑操作注入
            pattern += @"'\s*and\s*'|";        // ' and ' 形式的逻辑操作
            pattern += @"'\s*or\s*'|";         // ' or ' 形式的逻辑操作

            // 5. 检测时间延迟函数
            pattern += @"waitfor\s+delay|";     // SQL Server的waitfor delay
            pattern += @"sleep$|";             // MySQL的sleep函数
            pattern += @"benchmark\(|";         // MySQL的benchmark函数
            pattern += @"pg_sleep\(|";          // PostgreSQL的pg_sleep函数

            // 6. 检测特定数据库函数
            pattern += @"DBMS_PIPE\.RECEIVE_MESSAGE|";  // Oracle管道函数
            pattern += @"UTL_HTTP\.REQUEST|";           // Oracle HTTP请求函数
            pattern += @"UTL_INADDR\.GET_HOST_ADDRESS|";// Oracle网络函数

            // 7. 检测联合查询和子查询
            pattern += @"\bunion\s+all\b|";     // union all注入
            pattern += @"\bunion\s+select\b|";  // union select注入
            pattern += @"\bhaving\s+1=1\b|";    // having子句注入

            // 8. 检测编码混淆尝试
            pattern += @"char\(\d+$|";         // char(65)形式编码
            pattern += @"0x[0-9a-f]+|";         // 十六进制编码

            // 9. 检测条件语句
            pattern += @"case\s+when\s+1=1|";  // case when条件注入
            pattern += @"if\s*\(1=1\s*,";       // if函数注入

            // 移除最后一个多余的"|"字符
            pattern = pattern.TrimEnd('|');

            // 检查特殊字符
            if (input.Contains("'") || input.Contains("\"") || input.Contains("/*"))
            {
                return true;
            }

            // 检查函数调用
            if (Regex.IsMatch(input, @"\b(?:length|substr|ascii|cast)\s*\(", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return Regex.IsMatch(input, pattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// 验证入参是否为空
        /// </summary>
        /// <param name="value"></param>
        public static void ValidateParameter(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception(string.Format("入参不正确，{0}必填", parameterName));
            }
        }

        /// <summary>
        /// 特殊符号转换
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        public static string XmlString(string text, bool isAttribute = false)
        {
            if (string.IsNullOrEmpty(text.Trim()))
            {
                return string.Empty;
            }

            var sb = new System.Text.StringBuilder(text.Length);

            foreach (var chr in text)
            {
                if (chr == '<')
                    sb.Append("&lt;");
                else if (chr == '>')
                    sb.Append("&gt;");
                else if (chr == '&')
                    sb.Append("&amp;");

              // special handling for quotes
                else if (isAttribute && chr == '\"')
                    sb.Append("&quot;");
                else if (isAttribute && chr == '\'')
                    sb.Append("&apos;");
                // Legal sub-chr32 characters
                else if (chr == '\n')
                    sb.Append(isAttribute ? "&#xA;" : "\n");
                else if (chr == '\r')
                    sb.Append(isAttribute ? "&#xD;" : "\r");
                else if (chr == '\t')
                    sb.Append(isAttribute ? "&#x9;" : "\t");
                else
                {
                    if (chr < 32)
                        throw new InvalidOperationException("Invalid character in Xml String. Chr " +
                                                            Convert.ToInt16(chr) + " is illegal.");
                    sb.Append(chr);
                }
            }
     
           return sb.ToString();
       }



        #region HIS和平台的字典转换


        /// <summary>
        /// 平台的性别代码转成HIS的
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ConvertHISSexCode(string code)
        {
            switch (code)
            {
                case "1": return "M";
                case "2": return "F";
                case "9": return "U";
                default: return "U";
            }
        }

        /// <summary>
        /// HIS的性别代码转成平台的
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ConvertPlatformSexCode(string code)
        {
            switch (code)
            {
                case "M": return "1";
                case "F": return "2";
                case "U": return "9";
                default: return "9";
            }
        }

        public static string ConvertHisCardTypeCode(string code)
        {
            /*HIS的字典：
             01	身份证
             02	驾驶证
             03	军官证
             04	市民卡
             05	学生卡
             06	户口簿
             07	护照
             08	其他身份证
             09	健康卡
             */
            /* 平台的字典：
            1	身份证
            2	港澳居民来往内地通行证
            3	台湾居民来往大陆通行证/台胞证
            4	护照
            9	其它 
            */
            switch (code)
            {
                case "101": return "01";
                case "102": return "10";
                case "103": return "12";
                case "104": return "11";
                case "105": return "13";
                case "106": return "07";
                default: return "08";
            }
        }
        public static string ConvertPlatformCardTypeCode(string code)
        {
            /*HIS的字典：
             01	身份证
             02	驾驶证
             03	军官证
             04	市民卡
             05	学生卡
             06	户口簿
             07	护照
             08	其他身份证
             09	健康卡
             */
            /* 平台的字典：
            1	身份证
            2	港澳居民来往内地通行证
            3	台湾居民来往大陆通行证/台胞证
            4	护照
            9	其它 
            */
            switch (code)
            {
                case "01": return "1";
                case "02": return "9";
                case "03": return "9";
                case "04": return "9";
                case "05": return "9";
                case "06": return "1";
                case "07": return "4";
                case "08": return "9";
                case "09": return "9";
                default: return "9";
            }
        }

        /// <summary>
        /// 医生职称对应
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ConvertPlatformDoctLevelCode(string code)
        {
            /*HIS的字典：
            01	教授
            09	主任医师
            10	副主任医师
            11	主治医师
            12	主管医师
            13	医师
            14	主任护师
            18	护士
            21	主管技师
            22	技师
            57	见习医师
            60	其他

             */
            /* 平台的字典：
            1	主任医师
            2	副主任医师
            3	主治医师
            4	医师
            5	住院医师
            6	医士
            9	其他
            */
            switch (code)
            {
                case "01": return "9";
                case "09": return "1";
                case "10": return "3";
                case "11": return "9";
                case "12": return "9";
                case "13": return "4";
                case "14": return "9";
                case "18": return "9";
                case "21": return "9";
                case "22": return "9";
                default: return "9";
            }
        }

        #endregion

        #region 格式转换

        public static Int64 ToInt64(bool val)
        {
            return val ? 1 : 0;
        }

        public static Int64 ToInt64(object val)
        {
            if (val == null || val.ToString().Length == 0)
            {
                return -1;
            }

            return System.Convert.ToInt64(val);
        }

        public static Int64 ToInt64(string val)
        {
            if (val == null || val.Length == 0)
            {
                return -1;
            }
            else
            {
                if (val.ToLower() == "false")
                {
                    return 0;
                }

                if (val.ToLower() == "true")
                {
                    return 1;
                }
            }

            return System.Convert.ToInt64(val);
        }

        public static int ToInt32(bool val)
        {
            return val ? 1 : 0;
        }

        public static int ToInt32(string val)
        {
            if (val == null || val.Length == 0)
            {
                return -1;
            }
            else
            {
                if (val.ToLower() == "false")
                {
                    return 0;
                }

                if (val.ToLower() == "true")
                {
                    return 1;
                }
            }

            return System.Convert.ToInt32(val);
        }

        public static int ToInt32(object val)
        {
            if (val == null || val.ToString().Length == 0)
            {
                return -1;
            }

            return System.Convert.ToInt32(val);
        }

        public static DateTime ToDateTime(object val)
        {
            if (val == null || val.ToString() == string.Empty)
            {
                return DateTime.MinValue;
            }
            DateTime dt;
            if (DateTime.TryParse(val.ToString(), out dt))
            {
                return dt;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// 日期转换成年龄
        /// </summary>
        /// <param name="birthDate"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static string ToAge(DateTime birthDate, DateTime currentDate)
        {
            int years = 0, months = 0, days = 0, hour = 0, minute = 0;
            if (ToAge(birthDate, currentDate, ref years, ref months, ref days, ref hour, ref minute) > 0)
            {
                string age = "";
                if (years > 0)
                {
                    age = years.ToString() + "岁";
                }
                if (months > 0)
                {
                    age += months.ToString() + "月";
                }
                if (years <= 0 && days > 0)
                {
                    age += days.ToString() + "天";
                }
                if (years == 0 && months == 0 && days == 0 && hour >= 0 && minute > 0)
                {
                    age += string.Format("{0}小时{1}分", hour, minute);
                }
                return age;

            }
            else
            {
                return "0岁"; ;
            }
        }

        public static int ToAge(DateTime birthDate, DateTime currentDate, ref int year, ref int month, ref int day, ref int hour, ref int minute)
        {
            if ((birthDate - currentDate).Ticks >= 0)
            {
                return 0;
            }

            if (birthDate.Year == currentDate.Year && birthDate.Month == currentDate.Month && birthDate.Day == currentDate.Day)
            {
                TimeSpan ts = currentDate.Subtract(birthDate);
                hour = ts.Hours;
                minute = ts.Minutes;
                day = 0;
                month = 0;
                year = 0;

                return 1;
            }

            //减法，天相减
            if (currentDate.Day >= birthDate.Day)
            {
                day = currentDate.Day - birthDate.Day;
            }
            else
            {
                //借位，天数不够，借一个月
                month = month - 1;
                DateTime tmpTime = currentDate.AddMonths(-1);

                //被借位的那个月总天数比出生日期大，天数=当前日期+被借位月份天数-初始日期               
                if (DateTime.DaysInMonth(tmpTime.Year, tmpTime.Month) > birthDate.Day)
                {
                    day = currentDate.Day + DateTime.DaysInMonth(tmpTime.Year, tmpTime.Month) - birthDate.Day;
                }
                else
                {
                    //否则，天数=当前日期，为什么呢？ 

                    /* 简单点说：
                     * 在1.31号出生的人，2.28或者2.29都不算满月，而在3.1时要算年龄应该是1月1天，期间不存在刚好满一月的情况
                     */
                    /* 复杂情况：
                     * 如果在大月31号出生，而当前日期小于31号并且被借位那个月刚好又是小月，只有30号，甚至是闰月只有28天时就会产生问题；
                     * 同样如果在29、30号出生，而当前日期小于29、30，被借位那个月刚好是闰月也会产生问题；
                     * 到被借位的那个月最后一天不能算整月，再过一天（就是当前月）又是整月多一天，所以不存在刚好整月的那一天
                     */
                    /*
                     * 那么，在1.30、31号出生的人到3.1号时是不是一样大呢？
                     */
                    day = currentDate.Day;
                }
            }

            //减法，月相减
            if (currentDate.AddMonths(month).Month >= birthDate.Month)
            {
                //借月引起年变化
                if (currentDate.AddMonths(month).Year < currentDate.Year)
                {
                    year = year - 1;
                }
                month = currentDate.AddMonths(month).Month - birthDate.Month;
            }
            else
            {
                //借位，月份不够，借一年
                year = year - 1;
                month = currentDate.AddMonths(month).Month - birthDate.Month + 12;
            }
            year = currentDate.Year + year - birthDate.Year;

            return 1;
        }

        /// <summary>
        /// 日期转换成年龄
        /// </summary>
        /// <param name="birthDate"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static int ToAge(DateTime birthDate, DateTime currentDate, ref int year, ref int month, ref int day)
        {
            if ((birthDate - currentDate).Ticks >= 0)
            {
                return 0;
            }

            if (birthDate.Year == currentDate.Year && birthDate.Month == currentDate.Month && birthDate.Day == currentDate.Day)
            {
                day = 1;
                month = 0;
                year = 0;

                return 1;
            }

            //减法，天相减
            if (currentDate.Day >= birthDate.Day)
            {
                day = currentDate.Day - birthDate.Day;
            }
            else
            {
                //借位，天数不够，借一个月
                month = month - 1;
                DateTime tmpTime = currentDate.AddMonths(-1);

                //被借位的那个月总天数比出生日期大，天数=当前日期+被借位月份天数-初始日期               
                if (DateTime.DaysInMonth(tmpTime.Year, tmpTime.Month) > birthDate.Day)
                {
                    day = currentDate.Day + DateTime.DaysInMonth(tmpTime.Year, tmpTime.Month) - birthDate.Day;
                }
                else
                {
                    //否则，天数=当前日期，为什么呢？ 

                    /* 简单点说：
                     * 在1.31号出生的人，2.28或者2.29都不算满月，而在3.1时要算年龄应该是1月1天，期间不存在刚好满一月的情况
                     */
                    /* 复杂情况：
                     * 如果在大月31号出生，而当前日期小于31号并且被借位那个月刚好又是小月，只有30号，甚至是闰月只有28天时就会产生问题；
                     * 同样如果在29、30号出生，而当前日期小于29、30，被借位那个月刚好是闰月也会产生问题；
                     * 到被借位的那个月最后一天不能算整月，再过一天（就是当前月）又是整月多一天，所以不存在刚好整月的那一天
                     */
                    /*
                     * 那么，在1.30、31号出生的人到3.1号时是不是一样大呢？
                     */
                    day = currentDate.Day;
                }
            }

            //减法，月相减
            if (currentDate.AddMonths(month).Month >= birthDate.Month)
            {
                //借月引起年变化
                if (currentDate.AddMonths(month).Year < currentDate.Year)
                {
                    year = year - 1;
                }
                month = currentDate.AddMonths(month).Month - birthDate.Month;
            }
            else
            {
                //借位，月份不够，借一年
                year = year - 1;
                month = currentDate.AddMonths(month).Month - birthDate.Month + 12;
            }
            year = currentDate.Year + year - birthDate.Year;

            return 1;
        }

        /// <summary>
        /// 根据年龄、当前时间获取出生日期
        /// </summary>
        /// <param name="sysdate"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(DateTime sysdate, int iYear, int iMonth, int iDay)
        {
            DateTime temp = sysdate.AddDays(-sysdate.Day);
            while (iDay > temp.Day)
            {
                //换算成月
                iDay = iDay - temp.Day;
                temp = temp.AddDays(-temp.Day);
                iMonth = iMonth + 1;
            }

            //先进行换算
            if (iMonth > 12)
            {
                iYear += iMonth / 12;
                iMonth = iMonth % 12;
            }

            if (sysdate.Year < iYear)
            {
                return sysdate;
            }

            try
            {
                int year = sysdate.Year - iYear;
                int m = sysdate.Month - iMonth;
                if (m <= 0)
                {
                    if (year > 0)
                    {
                        year = year - 1;
                        DateTime dt = new DateTime(year, 1, 1);
                        m = dt.AddYears(1).AddDays(-1).Month + m;
                    }
                }

                int day = sysdate.Day - iDay;
                if (day <= 0)
                {
                    if (m > 0)
                    {
                        m = m - 1;
                        DateTime dt = new DateTime(year, m + 1, 1).AddMonths(-1);
                        day = dt.AddMonths(1).AddDays(-1).Day + day;
                    }
                    else if (year > 0)
                    {
                        year = year - 1;
                        DateTime dt = new DateTime(year, 1, 1);
                        m = dt.AddYears(1).AddDays(-1).Month - 1;
                        dt = new DateTime(year, m + 1, 1).AddMonths(-1);
                        day = dt.AddMonths(1).AddDays(-1).Day + day;
                    }

                    if (m <= 0)
                    {
                        if (year > 0)
                        {
                            year = year - 1;
                            DateTime dt = new DateTime(year, 1, 1);
                            m = dt.AddYears(1).AddDays(-1).Month + m;
                        }
                    }
                }
                else
                {
                    DateTime dt = new DateTime(year, m, 1);
                    if (day > dt.AddMonths(1).AddDays(-1).Day)
                    {
                        day = dt.AddMonths(1).AddDays(-1).Day;
                    }
                }

                return new DateTime(year, m, day);
            }
            catch
            {
                return sysdate;
            }
        }

        public static bool ToBoolean(object val)
        {
            if (val == null)
            {
                return false;
            }

            if (val.ToString() == "1")
            {
                return true;
            }

            if (val.ToString().ToLower() == "true")
            {
                return true;
            }

            return false;
        }

        public static decimal ToDecimal(object val)
        {
            if (val == null || val.ToString() == string.Empty)
                return 0;
            return Decimal.Parse(val.ToString());
        }

        public static string ToString(object val)
        {
            if (val == null)
            {
                return null;
            }

            if (val == DBNull.Value)
            {
                return null;
            }

            return val.ToString();
        }

        /// <summary>
        /// 保留两位小数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal ToDecimal(decimal val, int pos)
        {
            decimal d;
            if (decimal.TryParse(val.ToString("F2"), out d))
            {
                return d;
            }
            else
            {
                return val;
            }
        }
        #endregion

        #region DataTable数据转xml字符串
        /// <summary>
        /// DataTable数据转xml字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="i"></param>
        /// <param name="sb"></param>
        public static void SetStringBuilder(System.Data.DataTable dt, int i, System.Text.StringBuilder sb)
        {
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (dt.Columns[j].DataType.Name == "DateTime")
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", dt.Columns[j], dt.Rows[i][j].ToString().Replace('/', '-'));
                }
                else
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", dt.Columns[j], dt.Rows[i][j]);
                }
            }
        }
        #endregion

        #region 处理特殊符号转义
        static System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("[&<>'\"]");
        /// <summary>
        /// 处理特殊符号转义
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetEscapingContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "";
            }
            if (re.IsMatch(content))
            {
                content = content.Replace("&", "&amp;");
                content = content.Replace("<", "&lt;");
                content = content.Replace(">", "&gt;");
                content = content.Replace("'", "&apos;");
                content = content.Replace("\"", "&quot;");
            }
            return content;
        }
        #endregion

        #region 默认操作人信息
        static Models.OperInfo oper;
        public static Models.OperInfo DefaultOper
        {
            get
            {
                oper = new Models.OperInfo()
                {
                    Code = "00A105",// Platfo
                    Name = "新微信"
                };
                return oper;
            }
        }

        public static Models.OperInfo EmergencyOper
        {
            get
            {
                oper = new Models.OperInfo()
                {
                    Code = "00E001",// Platfo
                    Name = "急诊系统"
                };
                return oper;
            }
        }


        /// <summary>
        /// 支付宝操作人员信息
        /// </summary>
        public static Models.OperInfo ZFBOper
        {
            get
            {
                oper = new Models.OperInfo()
                {
                    Code = "00A106",// Platfo
                    Name = "平台支付宝"
                };
                return oper;
            }
        }

        /// <summary>
        /// 手机APP操作人员信息
        /// </summary>
        public static Models.OperInfo APPOper
        {
            get
            {
                oper = new Models.OperInfo()
                {
                    Code = "00A107",// Platfo
                    Name = "平台手机APP"
                };
                return oper;
            }
        }
        #endregion
    }
}