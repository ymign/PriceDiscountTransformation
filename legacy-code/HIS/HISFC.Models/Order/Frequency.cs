using System;
using System.Collections;
using Neusoft.HISFC;
namespace Neusoft.HISFC.Models.Order
{
    /// <summary>
    /// Neusoft.HISFC.Models.Order.Frequency<br></br>
    /// [功能描述: 医嘱频次实体]<br></br>
    /// [创 建 者: 李云凡]<br></br>
    /// [创建时间: 2006-09-10]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [Serializable]
    public class Frequency : Neusoft.HISFC.Models.Base.Spell, Neusoft.HISFC.Models.Base.ISort
    {

        public Frequency()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 变量

        /// <summary>
        /// 是否用默认的频次名
        /// </summary>
        private bool isUseDefaultName = true;

        /// <summary>
        /// 科室
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject dept;
        /// <summary>
        /// 用法
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject usage;

        /// <summary>
        /// 排序
        /// </summary>
        private int sortID;

        /// <summary>
        /// 频次执行时间
        /// 8:00,12:00,16:00
        /// </summary>
        protected string[] strTimes;
        /// <summary>
        /// 频次执行星期or间隔天
        /// 1
        /// </summary>
        protected string[] strDays;

        /// <summary>
        /// 获得的时间
        /// </summary>
        protected string strTime;

        /// <summary>
        /// 是否适用于门诊处方开立
        /// </summary>
        private bool isUseForOutPatient = true;

        /// <summary>
        /// 是否适用于门诊处方开立
        /// </summary>
        public bool IsUseForOutPatient
        {
            get { return isUseForOutPatient; }
            set { isUseForOutPatient = value; }
        }

        /// <summary>
        /// 是否适用于住院临嘱开立
        /// </summary>
        private bool isUserForInpatientLZ = true;

        /// <summary>
        /// 是否适用于住院临嘱开立
        /// </summary>
        public bool IsUserForInpatientLZ
        {
            get { return isUserForInpatientLZ; }
            set { isUserForInpatientLZ = value; }
        }

        /// <summary>
        /// 是否适用于住院长嘱开立
        /// </summary>
        private bool isUseForInpatientCZ = true;

        /// <summary>
        /// 是否适用于住院长嘱开立
        /// </summary>
        public bool IsUseForInpatientCZ
        {
            get { return isUseForInpatientCZ; }
            set { isUseForInpatientCZ = value; }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 是否用默认的频次名
        /// </summary>
        public bool IsUseDefaultName
        {
            get
            {
                return this.isUseDefaultName;
            }
            set
            {
                this.isUseDefaultName = value;
            }
        }

        /// <summary>
        /// 科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Dept
        {
            get
            {
                if (dept == null)
                {
                    dept = new Neusoft.FrameWork.Models.NeuObject();
                }
                return this.dept;
            }
            set
            {
                this.dept = value;
            }
        }

        /// <summary>
        /// 频次ID
        /// </summary>
        public new string ID
        {
            get
            {
                if (base.ID == null)
                {
                    base.ID = string.Empty;
                }
                return base.ID;
            }
            set
            {
                string str = "";
                //value = value.TrimEnd();
                //value = value.TrimStart();
                value = value.ToUpper();
                if (this.isUseDefaultName)
                {
                    string[] s = value.Split(' ');
                    str = this.f_getName(s[0]);
                    try
                    {
                        if (s.Length > 1) str = str + " " + this.f_getName(s[1]);
                    }
                    catch { }
                    if (str != "")
                    {
                        base.Name = str;
                    }
                }
                base.ID = value;
            }
        }

        /// <summary>
        /// 频次执行时间
        /// 8:00,12:00,16:00
        /// </summary>
        public string[] Times
        {
            get
            {
                strTimes = Time.Split('-');
                return strTimes;
            }
        }

        /// <summary>
        /// 频次执行星期or间隔天
        /// 1
        /// </summary>
        public string[] Days
        {
            get
            {
                if (strDays == null)
                {
                    this.strDays = new string[1];
                    strDays[0] = "1";
                }
                return strDays;
            }
        }

        /// <summary>
        /// 获得的时间
        /// </summary>
        public string Time
        {
            get
            {
                if (strTime == null || strTime == "") strTime = "8:00";//时间点为空，默认为8点
                return strTime;
            }
            set
            {
                strTime = value;
            }
        }

        /// <summary>
        /// 用法
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Usage
        {
            get
            {
                if (usage == null)
                {
                    usage = new Neusoft.FrameWork.Models.NeuObject();
                }
                return this.usage;
            }
            set
            {
                this.usage = value;
            }
        }

        #region 作废的

        /// <summary>
        /// 获得全部列表
        /// </summary>
        /// <returns>ArrayList(SysClass)</returns>
        [Obsolete("暂时不用了", true)]
        public static ArrayList List()
        {
            Frequency o;
            EnumFrequency e = new EnumFrequency();
            ArrayList alReturn = new ArrayList();
            int i;
            for (i = 0; i <= System.Enum.GetValues(e.GetType()).GetUpperBound(0); i++)
            {
                o = new Frequency();
                o.ID = ((EnumFrequency)i).ToString();
                o.Memo = i.ToString();
                alReturn.Add(o);
            }
            return alReturn;
        }
        #endregion

        #region 方法

        /// <summary>
        /// 重载ToString
        /// </summary>
        public override string ToString()
        {
            return base.Name;
        }
        #endregion

        protected bool f_IsNumber(string s)
        {
            try
            {
                if (s.ToUpper().IndexOf("A") > 0) return false;
                if (s.ToUpper().IndexOf("B") > 0) return false;
                if (s.ToUpper().IndexOf("C") > 0) return false;
                if (s.ToUpper().IndexOf("D") > 0) return false;
                if (s.ToUpper().IndexOf("E") > 0) return false;
                if (s.ToUpper().IndexOf("F") > 0) return false;
                if (s.ToUpper().IndexOf("G") > 0) return false;
                if (s.ToUpper().IndexOf("H") > 0) return false;
                if (s.ToUpper().IndexOf("I") > 0) return false;
                if (s.ToUpper().IndexOf("J") > 0) return false;
                if (s.ToUpper().IndexOf("K") > 0) return false;
                if (s.ToUpper().IndexOf("L") > 0) return false;
                if (s.ToUpper().IndexOf("M") > 0) return false;
                if (s.ToUpper().IndexOf("N") > 0) return false;
                if (s.ToUpper().IndexOf("O") > 0) return false;
                if (s.ToUpper().IndexOf("P") > 0) return false;
                if (s.ToUpper().IndexOf("Q") > 0) return false;
                if (s.ToUpper().IndexOf("R") > 0) return false;
                if (s.ToUpper().IndexOf("S") > 0) return false;
                if (s.ToUpper().IndexOf("T") > 0) return false;
                if (s.ToUpper().IndexOf("U") > 0) return false;
                if (s.ToUpper().IndexOf("V") > 0) return false;
                if (s.ToUpper().IndexOf("W") > 0) return false;
                if (s.ToUpper().IndexOf("X") > 0) return false;
                if (s.ToUpper().IndexOf("Y") > 0) return false;
                if (s.ToUpper().IndexOf("Z") > 0) return false;
                int i = System.Convert.ToInt16(s, 10);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获得名称
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string f_getName(string s)
        {
            s = s.Replace(".", "");
            string str = "";
            string sInterval = "", sDay = "1";
            int myDays = 0;
            if (s.Length <= 0) return "";
            try
            {
                try
                {
                    if (s.Substring(0, 2) == "QJ")
                    {
                        s = "Q1J" + s.Substring(2);
                    }
                }
                catch { }
                if (s.Substring(0, 1) == "Q")
                {
                    int i = 0;
                    for (i = s.Length - 1; i >= 1; i--)
                    {
                        if (this.f_IsNumber(s.Substring(1, i)))
                        {
                            sInterval = (s.Substring(1, i));
                            i = s.Length - 2 - i;
                            break;
                        }
                        else
                        {

                        }
                    }

                    if (this.f_IsNumber(s.Substring(s.Length - i)))
                    {
                        sDay = (s.Substring(s.Length - i));
                    }
                }
            }
            catch { }
            try
            {
                if (sInterval.Length > sDay.Length)//比较谁包含谁
                {
                    if (sInterval != "")
                    {
                        s = s.Replace(sInterval, "_integer_");
                    }
                    if (sDay != "")
                    {
                        s = s.Replace(sDay, "_day_");
                    }
                }
                else
                {
                    if (sDay != "")
                        s = s.Replace(sDay, "_day_");
                    if (sInterval != "")
                        s = s.Replace(sInterval, "_integer_");
                }
            }
            catch { }

            try
            {
                if (s.Length >= 2 && s.Substring(s.Length - 2, 2) == "ID")
                {
                    if (this.f_IsNumber(s.Substring(0, s.Length - 2)))
                    {
                        sInterval = (s.Substring(0, s.Length - 2));
                        s = s.Replace(sInterval, "_integer_");
                    }
                }
            }
            catch { }

            try
            {
                switch (s)
                {
                    // 一次
                    case "Once":
                        str = "一次";
                        this.strDays = new string[1];
                        strDays[0] = "1";
                        break;

                    // 间隔几秒
                    case "Q_integer_S":
                        str = "间隔" + sInterval + "秒";
                        break;

                    // 间隔分钟
                    case "Q_integer_M":
                        str = "间隔" + sInterval + "分钟";
                        break;

                    // 间隔小时
                    case "Q_integer_H":
                        // {87A79B85-B2FF-402e-B967-3EE882327E48} 间隔12小时
                        if (sInterval == "12")
                        {
                            str = "每十二小时一次";
                        }
                        else
                        {
                            str = "间隔" + sInterval + "小时";
                        }
                        break;

                    // 间隔天
                    // {DA246D16-D7B0-433d-9C37-3760671ED218}
                    case "Q_integer_D":
                        if (sInterval == "3")
                        {
                            str = "三日一次";
                        }
                        else
                        {
                            str = "间隔" + sInterval + "天";
                        }
                        this.strDays = new string[1];
                        strDays[0] = sInterval;
                        break;

                    // 间隔周
                    case "Q_integer_W": str = "间隔" + sInterval + "周";
                        this.strDays = new string[1];
                        myDays = int.Parse(sInterval) * 7;
                        strDays[0] = myDays.ToString();
                        break;

                    // 间隔日期
                    case "Q_integer_J_day_":
                        str = "间隔" + sInterval + "周 星期";
                        this.strDays = new string[sDay.Length + 1];
                        myDays = int.Parse(sInterval) * 7;
                        strDays[0] = myDays.ToString();
                        string d = "";
                        for (int i = 0; i < sDay.Length; i++)
                        {
                            d = d + sDay.Substring(i, 1) + ",";
                            strDays[i + 1] = sDay.Substring(i, 1);
                        }
                        str = str + d.Substring(0, d.Length - 1);
                        break;

                    case "Q_day_J_day_":

                        str = "间隔" + sInterval + "周 星期";
                        this.strDays = new string[sDay.Length + 1];
                        myDays = int.Parse(sInterval) * 7;
                        strDays[0] = myDays.ToString();
                        string d1 = "";
                        for (int i = 0; i < sDay.Length; i++)
                        {
                            d1 = d1 + sDay.Substring(i, 1) + ",";
                            strDays[i + 1] = sDay.Substring(i, 1);
                        }
                        str = str + d1.Substring(0, d1.Length - 1);
                        break;

                    //每周
                    case "QW":
                        str = "间隔一周";
                        this.strDays = new string[1];
                        strDays[0] = "7";
                        break;

                    // 每天
                    case "QD":
                        str = "每天";
                        this.strDays = new string[1];
                        strDays[0] = "1";
                        break;

                    // 隔天=Q1D
                    case "QOD":
                        str = "隔天一次";
                        this.strDays = new string[1];
                        strDays[0] = "2";
                        break;

                    // 早上定时
                    case "QAM":
                        str = "早上定时";
                        break;

                    // 晚上定时
                    case "QPM":
                        str = "晚上定时";
                        break;

                    // 临睡时
                    case "QHS":
                        str = "临睡时";
                        break;

                    // 早餐前
                    case "ACM":
                        str = "早餐前";
                        break;

                    // 早餐后
                    case "PCM":
                        str = "早餐后";
                        break;

                    // 早午餐之间
                    case "ICM":
                        str = "早午餐之间";
                        break;

                    // 午餐前
                    case "ACD":
                        str = "午餐前";
                        break;

                    // 午餐后
                    case "PCD":
                        str = "午餐后";
                        break;

                    // 午餐和晚餐之间
                    case "ICD":
                        str = "午餐和晚餐之间";
                        break;

                    // 晚餐前
                    case "ACV":
                        str = "晚餐前";
                        break;

                    // 晚餐后
                    case "PCV":
                        str = "晚餐后";
                        break;

                    // 晚餐和临睡之间
                    case "ICV":
                        str = "晚餐和临睡之间";
                        break;

                    // 日两次
                    case "BID":
                        str = "每日两次";
                        break;

                    // 日三次
                    case "TID":
                        str = "每日三次";
                        break;


                    // 日四次
                    case "QID":
                        str = "每日四次";
                        break;

                    // 日n次
                    case "_integer_ID":
                        str = "日" + sInterval + "次";
                        break;

                    // 随时服用
                    case "PRN":
                        str = "必要时服用";
                        break;

                    // 立即服用
                    case "ST":
                        str = "立即使用";
                        break;
                    //{D4A128C2-9112-402a-A610-DB1937C53A5F}feng.ch一些不在规则之内的频次2010-12-21
                    //每周五次周1,2,3,4,5
                    case "5/W":
                        str = "每周五次(1,2,3,4,5)";
                        this.strDays = new string[6];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "2";
                        strDays[3] = "3";
                        strDays[4] = "4";
                        strDays[5] = "5";
                        break;
                    //每周两次，周一，周三
                    case "BIW_day_":
                        str = "每周两次(1,4)";
                        this.strDays = new string[3];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "4";
                        break;
                    // {EB2DF05A-F3DB-4723-B4C9-839E97E6BCE8} BIW每天都会分解
                    case "BIW":
                        str = "每周两次(1,4)";
                        this.strDays = new string[3];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "4";
                        break;
                    
                    //每周两次，周一，周三
                    case "BIW1":
                        str = "每周两次(1,4)";
                        this.strDays = new string[3];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "4";
                        break;
                    //每周两次，周二，周四
                    case "BIW2":
                        str = "每周两次(2,5)";
                        this.strDays = new string[3];
                        strDays[0] = "7";
                        strDays[1] = "2";
                        strDays[2] = "5";
                        break;
                    //每周两次，周二，周五
                    case "BIW3":
                        str = "每周两次(3,6)";
                        this.strDays = new string[3];
                        strDays[0] = "7";
                        strDays[1] = "3";
                        strDays[2] = "6";
                        break;
                    //每周两次，周一，周四
                    case "BIW4":
                        str = "每周两次(1,4)";
                        this.strDays = new string[3];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "4";
                        break;
                    //每周两次，周三，周六
                    case "BIW5":
                        str = "每周两次(3,6)";
                        this.strDays = new string[3];
                        strDays[0] = "7";
                        strDays[1] = "3";
                        strDays[2] = "6";
                        break;
                    //每周四次1,2,3,4
                    case "QIW_day_":
                        str = "每周四次(1,2,3,4)";
                        this.strDays = new string[5];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "2";
                        strDays[3] = "3";
                        strDays[4] = "4";
                        break;
                    //每周四次1,3,5,7
                    case "QIW2":
                        str = "每周四次(1,3,5,7)";
                        this.strDays = new string[5];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "3";
                        strDays[3] = "5";
                        strDays[4] = "7";
                        break;
                    //每周一次周1
                    case "QW_day_":
                        str = "每周一次(1)";
                        this.strDays = new string[2];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        break;
                    //每周一次周2
                    case "QW2":
                        str = "每周一次(2)";
                        this.strDays = new string[2];
                        strDays[0] = "7";
                        strDays[1] = "2";
                        break;
                    //每周一次周3
                    case "QW3":
                        str = "每周一次(3)";
                        this.strDays = new string[2];
                        strDays[0] = "7";
                        strDays[1] = "3";
                        break;
                    //每周一次周4
                    case "QW4":
                        str = "每周一次(4)";
                        this.strDays = new string[2];
                        strDays[0] = "7";
                        strDays[1] = "4";
                        break;
                    //每周一次周5
                    case "QW5":
                        str = "每周一次(5)";
                        this.strDays = new string[2];
                        strDays[0] = "7";
                        strDays[1] = "5";
                        break;
                    //每周一次周6
                    case "QW6":
                        str = "每周一次(6)";
                        this.strDays = new string[2];
                        strDays[0] = "7";
                        strDays[1] = "6";
                        break;
                    //每周一次周4
                    case "QW7":
                        str = "每周一次(7)";
                        this.strDays = new string[2];
                        strDays[0] = "7";
                        strDays[1] = "7";
                        break;
                    case "TIW_day_":
                        str = "每周三次(1,3,5)";
                        this.strDays = new string[4];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "3";
                        strDays[3] = "5";
                        break;
                    //每周三次周1,3,5
                    case "TIW1":
                        str = "每周三次(1,3,5)";
                        this.strDays = new string[4];
                        strDays[0] = "7";
                        strDays[1] = "1";
                        strDays[2] = "3";
                        strDays[3] = "5";
                        break;
                    //每周三次周2,4,6
                    case "TIW2":
                        str = "每周三次(2,4,6)";
                        this.strDays = new string[4];
                        strDays[0] = "7";
                        strDays[1] = "2";
                        strDays[2] = "4";
                        strDays[3] = "6";
                        break;
                    //每周三次周2,5,7
                    case "TIW3":
                        str = "每周三次(2,5,7)";
                        this.strDays = new string[4];
                        strDays[0] = "7";
                        strDays[1] = "2";
                        strDays[2] = "5";
                        strDays[3] = "7";
                        break;
                    //日七次
                    case "SID": str = "日七次"; break;
                }
            }
            catch
            {
            }
            str = str.Replace("间隔1周", "每周");
            return str;
        }

        #region 克隆

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Frequency Clone()
        {
            Frequency obj = base.Clone() as Frequency;
            obj.dept = this.Dept.Clone();
            return obj;
        }

        #endregion

        #endregion

        #region 接口实现

        #region ISort 成员

        /// <summary>
        /// 排序
        /// </summary>
        public int SortID
        {
            get
            {
                // TODO:  添加 Frequency.SortID getter 实现
                return this.sortID;
            }
            set
            {
                // TODO:  添加 Frequency.SortID setter 实现
                this.sortID = value;
            }
        }

        #endregion

        #endregion
    }


    public enum EnumFrequency
    {
        /// <summary>
        /// 一次
        /// </summary>
        Once,
        /// <summary>
        /// 间隔几秒
        /// </summary>
        Q_integer_S,
        /// <summary>
        /// 间隔分钟
        /// </summary>
        Q_integer_M,
        /// <summary>
        /// 间隔小时
        /// </summary>
        Q_integer_H,
        /// <summary>
        /// 间隔天
        /// </summary>
        Q_integer_D,
        /// <summary>
        /// 间隔周
        /// </summary>
        Q_integer_W,
        /// <summary>
        /// 间隔日期
        /// </summary>
        Q_integer_J_day_,
        /// <summary>
        /// 每天
        /// </summary>
        QD,
        /// <summary>
        /// 隔天=Q1D
        /// </summary>
        QOD,
        /// <summary>
        /// 早上定时
        /// </summary>
        QAM,
        /// <summary>
        /// 晚上定时
        /// </summary>
        QPM,
        /// <summary>
        /// 临睡时
        /// </summary>
        QHS,
        /// <summary>
        /// 早餐前
        /// </summary>
        ACM,
        /// <summary>
        /// 早餐后
        /// </summary>
        PCM,
        /// <summary>
        /// 早午餐之间
        /// </summary>
        ICM,
        /// <summary>
        /// 午餐前
        /// </summary>
        ACD,
        /// <summary>
        /// 午餐后
        /// </summary>
        PCD,
        /// <summary>
        /// 午餐和晚餐之间
        /// </summary>
        ICD,
        /// <summary>
        /// 晚餐前
        /// </summary>
        ACV,
        /// <summary>
        /// 晚餐后
        /// </summary>
        PCV,
        /// <summary>
        /// 晚餐和临睡之间
        /// </summary>
        ICV,
        /// <summary>
        /// 日两次
        /// </summary>
        BID,
        /// <summary>
        /// 日三次
        /// </summary>
        TID,
        /// <summary>
        /// 日四次
        /// </summary>
        QID,
        /// <summary>
        /// 日n次
        /// </summary>
        _integer_ID,
        /// <summary>
        /// 必要时服用
        /// </summary>
        PRN
    }
}
