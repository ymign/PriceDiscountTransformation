using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Neusoft.HISFC.Models.Fee;
using Neusoft.HISFC.Models.RADT;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;

namespace Neusoft.HISFC.Components.InpatientFee
{
    public class Function
    {
        private static Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();


        #region 住院取部分支付方式



        /// <summary>
        /// 通过结算方式找id
        /// </summary>
        /// <param name="payTypeName"></param>
        /// <returns></returns>
        public static string GetPayTypeIdByName(string payTypeName)
        {
            ArrayList al = new ArrayList();
            //{93E6443C-1FB5-45a7-B89D-F21A92200CF6}
            //al = Neusoft.HISFC.Models.Fee.EnumPayTypeService.List();
            al = managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.PAYMODES);
            for (int i = 0; i < al.Count; i++)
            {
                //Neusoft.HISFC.Models.Fee.EnumPayTypeService payType = new Neusoft.HISFC.Models.Fee.EnumPayTypeService();
                //payType = (EnumPayTypeService)al[i];
                if (((Neusoft.FrameWork.Models.NeuObject)al[i]).Name == payTypeName) return ((Neusoft.FrameWork.Models.NeuObject)al[i]).ID;
            }
            return null;
        }
        /// <summary>
        /// 通过银行名称获取银行id
        /// </summary>
        /// <param name="bankName">银行名称</param>
        /// <returns>银行ID</returns>
        public static string GetBankIdByName(string bankName)
        {
            ArrayList al = new ArrayList();
            al = managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.BANK);
            if (al == null || al.Count <= 0)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("获取银行列表失败!", 211);
                return null;
            }

            for (int i = 0; i < al.Count; i++)
            {
                Neusoft.FrameWork.Models.NeuObject obj;
                obj = (Neusoft.FrameWork.Models.NeuObject)al[i];
                if (obj.Name == bankName) return obj.ID;
            }
            return null;


        }

        #endregion

        #region "大写"
        /// <summary>
        /// 
        /// </summary>
        private static readonly string cnNumber = "零壹贰叁肆伍陆柒捌玖";
        private static readonly string cnUnit = "分角元拾佰仟万拾佰仟亿拾佰仟兆拾佰仟";
        private static readonly string[] enSmallNumber = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
        private static readonly string[] enLargeNumber = { "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };
        private static readonly string[] enUnit = { "", "THOUSAND", "MILLION", "BILLION", "TRILLION" };
        // 最大转化金额         9999999999999999.99  （16位整数位，2位小数位）



        // 方法返回值           转换成功则返回非零长字串,否则返回零长字串

        // 静态英文转换方法     string GetEnString(string MoneyString)
        // 最大转化金额         999999999999999.99   （15位整数位，2位小数位）



        // 方法返回值           转换成功则返回非零长字串,否则返回零长字串


        // 以下是货币金额中文大写转换方法



        public static string ConvertNumberToChineseMoneyString(string MoneyString)
        {
            string[] tmpString = MoneyString.Split('.');
            string intString = MoneyString;   // 默认为整数



            string decString = "";            // 保存小数部分字串
            string rmbCapital = "";            // 保存中文大写字串
            int k;
            int j;
            int n;

            if (tmpString.Length > 1)
            {
                intString = tmpString[0];             // 取整数部分



                decString = tmpString[1];             // 取小数部分



            }
            decString += "00";
            decString = decString.Substring(0, 2);   // 保留两位小数位



            intString += decString;

            try
            {
                k = intString.Length - 1;
                if (k > 0 && k < 18)
                {
                    for (int i = 0; i <= k; i++)
                    {
                        j = (int)intString[i] - 48;
                        // rmbCapital = rmbCapital + cnNumber[j] + cnUnit[k-i];     // 供调试用的直接转换



                        n = i + 1 >= k ? (int)intString[k] - 48 : (int)intString[i + 1] - 48; // 等效于 if( ){ }else{ }
                        if (j == 0)
                        {
                            if (k - i == 2 || k - i == 6 || k - i == 10 || k - i == 14)
                            {
                                rmbCapital += cnUnit[k - i];
                            }
                            else
                            {
                                if (n != 0)
                                {
                                    rmbCapital += cnNumber[j];
                                }
                            }
                        }
                        else
                        {
                            rmbCapital = rmbCapital + cnNumber[j] + cnUnit[k - i];
                        }
                    }

                    rmbCapital = rmbCapital.Replace("兆亿万", "兆");
                    rmbCapital = rmbCapital.Replace("兆亿", "兆");
                    rmbCapital = rmbCapital.Replace("亿万", "亿");
                    rmbCapital = rmbCapital.TrimStart('元');
                    rmbCapital = rmbCapital.TrimStart('零');

                    return rmbCapital;
                }
                else
                {
                    return "";   // 超出转换范围时，返回零长字串
                }
            }
            catch
            {
                return "";   // 含有非数值字符时，返回零长字串



            }
        }

        #endregion

        #region 判断就诊卡号第一个字符是否含有字符
        /// <summary>
        /// 判断就诊卡号第一个字符是否含有字符



        /// </summary>
        /// <param name="CarNO">得到就诊卡号</param>
        /// <returns></returns>
        public static string GetCarNO(string CarNO)
        {
            if (!Char.IsNumber(CarNO, 0))
            {
                string FirtrLeter = CarNO.Substring(0, 1).ToUpper();
                string TempStr = CarNO.Substring(1, CarNO.Length - 1);
                TempStr = TempStr.PadLeft(9, '0');
                CarNO = FirtrLeter + TempStr;
            }
            else
            {
                CarNO = CarNO.PadLeft(10, '0');
            }
            return CarNO;
        }
        #endregion

        #region 快捷键
        /// <summary>
        /// 快捷键设置路径
        /// </summary>
        private static string filePath = Application.StartupPath + @".\" + Neusoft.FrameWork.WinForms.Classes.Function.SettingPath + @".\InpatientShotcut.xml";

        /// <summary>
        /// 重新初始化工具栏
        /// </summary>
        /// <param name="hsToolBar">工具栏哈希表</param>
        /// <param name="toolBarService">当前窗口的toolBarService</param>
        /// <param name="windowName">窗口名称</param>
        public static void RefreshToolBar(Hashtable hsToolBar, Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService, string windowName)
        {
            XmlDocument doc = new XmlDocument();
            if (filePath == "")
            {
                return;
            }
            try
            {
                StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default);
                string cleandown = sr.ReadToEnd();
                doc.LoadXml(cleandown);
                sr.Close();
            }
            catch
            {
                return;
            }
            XmlNode winNode = doc.SelectSingleNode("//" + windowName);
            XmlNodeList nodes = winNode.SelectNodes("Column");
            foreach (XmlNode node in nodes)
            {
                string opKey = node.Attributes["opKey"].Value;
                string cuKey = node.Attributes["cuKey"].Value;
                string opName = node.Attributes["opName"].Value;
                if (opKey != "")
                {
                    opKey = "Ctrl+";
                }
                if (cuKey == "")
                {
                    cuKey = "";
                }
                else
                {
                    cuKey = "(" + opKey + cuKey + ")";
                }
                if (opName != "")
                {
                    ToolStripButton tempButton = new ToolStripButton();
                    tempButton = toolBarService.GetToolButton(opName);
                    if (tempButton != null)
                    {
                        tempButton.Text = opName + cuKey;
                        hsToolBar.Add(tempButton.Text, opName);
                    }
                }
            }
        }
        
        /// <summary>
        /// 重新初始化工具栏
        /// </summary>
        /// <param name="hsToolBar">工具栏哈希表</param>
        /// <param name="toolStrip">当前窗口的toolstrip</param>
        /// <param name="windowName">窗口名称</param>
        public static void RefreshToolBar(Hashtable hsToolBar, System.Windows.Forms.ToolStrip toolStrip, string windowName)
        {
            XmlDocument doc = new XmlDocument();
            if (filePath == "")
            {
                return;
            }
            try
            {
                StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default);
                string cleandown = sr.ReadToEnd();
                doc.LoadXml(cleandown);
                sr.Close();
            }
            catch
            {
                return;
            }
            XmlNode winNode = doc.SelectSingleNode("//" + windowName);
            XmlNodeList nodes = winNode.SelectNodes("Column");
            foreach (XmlNode node in nodes)
            {
                string opKey = node.Attributes["opKey"].Value;
                string cuKey = node.Attributes["cuKey"].Value;
                string opName = node.Attributes["opName"].Value;
                if (opKey != "")
                {
                    opKey = "Ctrl+";
                }
                if (cuKey == "")
                {
                    cuKey = "";
                }
                else
                {
                    cuKey = "(" + opKey + cuKey + ")";
                }
                if (opName != "")
                {
                    foreach (ToolStripItem ts in toolStrip.Items)
                    {
                        try
                        {
                            if (ts.GetType() == typeof(ToolStripButton))
                            {
                                if (ts.Text == opName)
                                {
                                  ts.Text = opName + cuKey;
                                  hsToolBar.Add(ts.Text, opName);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    
                }
            }
        }
        
        /// <summary>
        /// 获取操作功能名称
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="hashCode">当前按键的HashCode</param>
        /// <returns>成功当前值,失败 string.Empty</returns>
        public static string GetOperationName(string windowName, string hashCode)
        {
            XmlDocument doc = new XmlDocument();
            if (filePath == "") return "";
            try
            {
                StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default);
                string cleandown = sr.ReadToEnd();
                doc.LoadXml(cleandown);
                sr.Close();
            }
            catch
            {
                return "";
            }

            XmlNode winNode = doc.SelectSingleNode("//" + windowName);
            XmlNodeList nodes = winNode.SelectNodes("Column");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["hash"].Value == hashCode)
                {
                    return node.Attributes["opName"].Value;
                }
            }


            return "";
        }

        #endregion

        #region 宜康科室

        private static Dictionary<string, string> dictionaryYKDept = new Dictionary<string, string>();
        /// <summary>
        ///  判断是否是宜康科室
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public static bool IsContainYKDept()
        {

            string dept = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID;

            return IsContainYKDept(dept);
        }

        /// <summary>
        ///  判断是否是宜康科室
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public static bool IsContainYKDept(string dept)
        {
            if (dictionaryYKDept == null || dictionaryYKDept.Count == 0)
            {
                ArrayList al = CommonController.Instance.QueryConstant("YkDept");
                if (al != null)
                {
                    foreach (Neusoft.FrameWork.Models.NeuObject obj in al)
                    {
                        dictionaryYKDept[obj.ID] = obj.Name;
                    }
                }
            }

            return dictionaryYKDept.ContainsKey(dept);
        }

        #endregion

        #region 画组合号
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="iColumnCombFlag">画组合号列号</param>
        /// <param name="iColumnCombNo">组合号列号</param>
        /// <param name="iColumnDtExec">执行时间列号</param>
        public static void DrawCombFlag(FarPoint.Win.Spread.SheetView sv, int iColumnCombFlag, int iColumnCombNo, int iColumnDtExec)
        {
            //画组合号


            string tmp = "", curComboNo = "";
            string strDt = "", strDtEexc = "";
            for (int i = 0; i < sv.RowCount; ++i)
            {
                #region "画"
                if (sv.Cells[i, iColumnCombNo].Text == "0") sv.Cells[i, iColumnCombNo].Text = "";
                tmp = sv.Cells[i, iColumnCombNo].Text + "";
                strDtEexc = sv.Cells[i, iColumnDtExec].Text + "";
                sv.Cells[i, iColumnCombNo].Tag = tmp;
                if ((curComboNo != tmp && tmp != "") || (curComboNo == tmp && tmp != "" && strDt.Trim() != strDtEexc.Trim())) //是头
                {
                    curComboNo = tmp;
                    sv.Cells[i, iColumnCombFlag].Text = "┓";


                    strDt = strDtEexc.Trim();
                    try
                    {
                        if (sv.Cells[i - 1, iColumnCombFlag].Text == "┃")
                        {
                            sv.Cells[i - 1, iColumnCombFlag].Text = "┛";
                        }
                        else if (sv.Cells[i - 1, iColumnCombFlag].Text == "┓")
                        {
                            sv.Cells[i - 1, iColumnCombFlag].Text = "";
                        }
                    }
                    catch { }
                }
                else if (curComboNo == tmp && tmp != "" && strDt.Trim() == strDtEexc.Trim())
                {
                    sv.Cells[i, iColumnCombFlag].Text = "┃";
                }
                else if ((curComboNo != tmp && tmp == ""))//||strDt.Trim()!=strDtEexc.Trim()
                {
                    try
                    {
                        if (sv.Cells[i - 1, iColumnCombFlag].Text == "┃")
                        {
                            sv.Cells[i - 1, iColumnCombFlag].Text = "┛";
                        }
                        else if (sv.Cells[i - 1, iColumnCombFlag].Text == "┓")
                        {
                            sv.Cells[i - 1, iColumnCombFlag].Text = "";
                        }
                    }
                    catch { }
                    sv.Cells[i, iColumnCombFlag].Text = "";
                    curComboNo = "";
                    strDt = "";

                }
                else if (curComboNo == tmp && tmp != "" && strDt.Trim() != strDtEexc.Trim())
                {
                    try
                    {
                        if (sv.Cells[i - 1, iColumnCombFlag].Text == "┃")
                        {
                            sv.Cells[i - 1, iColumnCombFlag].Text = "┛";
                        }
                        else if (sv.Cells[i - 1, iColumnCombFlag].Text == "┓")
                        {
                            sv.Cells[i - 1, iColumnCombFlag].Text = "";
                        }
                    }
                    catch { }
                    sv.Cells[i, iColumnCombFlag].Text = "┓";
                    curComboNo = "";
                    strDt = "";

                }
                if (i == sv.RowCount - 1 && sv.Cells[i, iColumnCombFlag].Text == "┃") sv.Cells[i, iColumnCombFlag].Text = "┛";
                if (i == sv.RowCount - 1 && sv.Cells[i, iColumnCombFlag].Text == "┓") sv.Cells[i, iColumnCombFlag].Text = "";
                sv.Cells[i, iColumnCombFlag].ForeColor = System.Drawing.Color.Red;
                #endregion
            }
        }
        #endregion 

        /// <summary>
        /// 住院收费二级权限
        /// </summary>
        public const string PrivQuit = "0830";
        /// <summary>
        /// 住院退其他收费员的费用权限
        /// </summary>
        public const string PrivQuitOtherOperFee = "24";
        /// <summary>
        /// 住院隔日退费权限
        /// </summary>
        public const string PrivQuitLastDayFee = "25";
        /// <summary>
        /// 住院退其他科室费用权限
        /// </summary>
        public const string PrivQuitOtherDeptFee = "26";

    }
}
