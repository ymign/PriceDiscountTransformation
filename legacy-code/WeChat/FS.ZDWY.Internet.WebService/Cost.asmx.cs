using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// Cost 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Cost : System.Web.Services.WebService
    {
        FS.ZDWY.Internet.BP.InPatient.CostManager costManager;

        FS.ZDWY.Internet.BP.InPatient.CostManager CostManager
        {
            get
            {
                if (costManager == null)
                {
                    costManager = new BP.InPatient.CostManager();
                }
                return costManager;
            }
        }

        LogHelper.ServiceLog serviceLogManager;
        /// <summary>
        /// 服务日志管理
        /// </summary>
        LogHelper.ServiceLog ServiceLogManager
        {
            get
            {
                if (serviceLogManager == null)
                {
                    serviceLogManager = new LogHelper.ServiceLog();
                }
                return serviceLogManager;
            }
        }

        [WebMethod(Description = "费用日清单查询")]
        public string Daily(string req)
        {
            #region 入参模板

            //<Request><data>
            //<queryType></queryType>
            //<patientId></patientId>
            //<admissionNo></admissionNo>
            //<certifcateType></certifcateType>
            //<certifcateNo></certifcateNo>
            //<cardType></cardType>
            //<cardNo></cardNo>
            //<startDate></startDate>
            //<endDate></endDate>
            //</data></Request>


            #endregion

            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string queryType = Function.GetNoteValue(xmlDoc, "Request/data/queryType");//查询标志0-在院 1-出院
                //Function.ValidateParameter(queryType, "查询标志");
                string patientID = Function.GetNoteValue(xmlDoc, "Request/data/patientId");  //院内用户id
                string admissionNo = Function.GetNoteValue(xmlDoc, "Request/data/inpatId");  //用户住院号
                Function.ValidateParameter(admissionNo, "住院号");
                string inpatientNo = Function.GetNoteValue(xmlDoc, "Request/data/inpatNumber");  //住院流水号
                Function.ValidateParameter(inpatientNo, "住院流水号");
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");//用户证件号码
                DateTime startDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/startDate"));//开始时间
                Function.ValidateParameter(startDate.ToString(), "开始时间");
                DateTime endDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/endDate"));//结束时间
                Function.ValidateParameter(endDate.ToString(), "结束时间");
                System.Data.DataTable dtRes = null;
                if (queryType == "0")
                {
                    dtRes = this.CostManager.QueryInMainDayFeeIn(patientID, admissionNo, inpatientNo,certifcateNo, startDate, endDate);
                }
                else if (queryType == "1")
                {
                    dtRes = this.CostManager.QueryInMainDayFeeOut(patientID, admissionNo, inpatientNo, certifcateNo, startDate, endDate);
                }
                else
                {
                    dtRes = this.CostManager.QueryInMainDayFeeALL(patientID, admissionNo, inpatientNo, certifcateNo, startDate, endDate);
                }

                if (dtRes == null)
                {
                    throw new Exception("查找住院患者一日清单列表失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到住院患者一日清单列表");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    dataXml.Append("<item>");
                    for (int j = 0; j < dtRes.Columns.Count; j++)
                    {
                        if (dtRes.Columns[j].DataType.Name == "DateTime")
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j].ToString().Replace('/', '-'));
                        }
                        else
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j]);
                        }
                    }
                    dataXml.Append("</item>");
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "住院管理")]
        public string Manage(string req)
        {
            #region 入参模板

            // <Request><data>
            //<patientId></patientId>
            //<admissionNo></admissionNo>
            //<certifcateType></certifcateType>
            //<certifcateNo></certifcateNo>
            //<cardType></cardType>
            //<cardNo></cardNo>
            //<startDate></startDate>
            //<endDate></endDate>
            //</data></Request>


            #endregion

            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string patientID = Function.GetNoteValue(xmlDoc, "Request/data/patientId");  //院内用户id
                string admissionNo = Function.GetNoteValue(xmlDoc, "Request/data/inpatId");  //用户住院号
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");//用户证件号码
                string inpatNumber = Function.GetNoteValue(xmlDoc, "Request/data/inpatNumber");//住院流水号
                DateTime startDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/startDate"));//开始时间
                Function.ValidateParameter(startDate.ToString(), "开始时间");
                DateTime endDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/endDate"));//结束时间
                Function.ValidateParameter(endDate.ToString(), "结束时间");
                System.Data.DataTable dtRes = CostManager.QueryInMainfoByPatients(patientID, admissionNo, inpatNumber, certifcateNo, startDate, endDate);
                if (dtRes == null)
                {
                    throw new Exception("查找住院患者列表失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到住院患者列表");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    dataXml.Append("<item>");
                    for (int j = 0; j < dtRes.Columns.Count; j++)
                    {
                        if (dtRes.Columns[j].DataType.Name == "DateTime")
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j].ToString());
                        }
                        else
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j],dtRes.Rows[i][j].ToString());
                        }
                    }
                    dataXml.Append("</item>");
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "住院费用明细查询")]
        public string Detail(string req)
        {
            #region 入参模板

            //<Request><data>
            //<queryType></queryType>
            //<patientId></patientId>
            //<admissionNo></admissionNo>
            //<certifcateType></certifcateType>
            //<certifcateNo></certifcateNo>
            //<cardType></cardType>
            //<cardNo></cardNo>
            //<startDate></startDate>
            //<endDate></endDate>
            //</data></Request>

            #endregion

            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string queryType = Function.GetNoteValue(xmlDoc, "Request/data/queryType");  //查询标志
                //Function.ValidateParameter(queryType, "查询标志");
                string patientID = Function.GetNoteValue(xmlDoc, "Request/data/patientId");  //院内用户id
                string admissionNo = Function.GetNoteValue(xmlDoc, "Request/data/inpatId");  //用户住院号
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");//用户证件号码
                DateTime startDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/startDate"));//开始时间
                Function.ValidateParameter(startDate.ToString(), "开始时间");
                DateTime endDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/endDate"));//结束时间
                Function.ValidateParameter(endDate.ToString(), "结束时间");
                string visitno = Function.GetNoteValue(xmlDoc, "Request/data/inpatNumber");  //用户住院号
                string inState = string.Empty;
                if (queryType == "0")
                {
                    inState = "R,I";
                }
                else if (queryType == "1")
                {
                    inState = "B,O";
                }
                else
                {
                    inState = "R,I,B,O";
                }
                System.Data.DataTable dtRes = CostManager.QueryInMainInfoDetail(inState, patientID, admissionNo, visitno, certifcateNo, startDate, endDate);
                if (dtRes == null)
                {
                    throw new Exception("查找住院患者费用明细列表失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到住院患者费用明细列表");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                string currentDateTime = string.Empty;
                List<InFeeItem> feeList = new List<InFeeItem>();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    InFeeItem item = new InFeeItem();
                    item.InDate = Function.ToDateTime(dtRes.Rows[i]["inDate"].ToString().Replace('/', '-')).ToShortDateString();
                    item.DayAmount = Function.ToDecimal(dtRes.Rows[i]["dayAmount"].ToString().Replace('/', '-'));
                    item.ChargeAmount = Function.ToDecimal(dtRes.Rows[i]["chargeAmount"].ToString().Replace('/', '-'));
                    item.FeeType = dtRes.Rows[i]["feeType"].ToString();
                    item.FeeName = dtRes.Rows[i]["feeName"].ToString();
                    item.Code = dtRes.Rows[i]["code"].ToString();
                    item.Name = dtRes.Rows[i]["name"].ToString();
                    item.Unit = dtRes.Rows[i]["unit"].ToString();
                    item.Price =Function.ToDecimal(dtRes.Rows[i]["price"].ToString());
                    item.Count = Function.ToDecimal(dtRes.Rows[i]["count"].ToString());
                    item.Space = dtRes.Rows[i]["spec"].ToString();
                    item.Amount = Function.ToDecimal(dtRes.Rows[i]["amount"].ToString());
                    feeList.Add(item);
                }

                List<string> dateStringList = feeList.GroupBy(m => m.InDate).Select(g=>g.Key).ToList<string>();

                foreach (string dateString in dateStringList)
                {
                    dataXml.Append("<item>");
                    dataXml.AppendFormat("<{0}>{1}</{0}>", "inDate", dateString);
                    dataXml.AppendFormat("<{0}>{1}</{0}>", "dayAmount", feeList.Find(m => m.InDate == dateString).DayAmount);
                    dataXml.AppendFormat("<{0}>{1}</{0}>", "chargeAmount", feeList.Find(m => m.InDate == dateString).ChargeAmount);
                    dataXml.Append("<costItem>");
                    List<InFeeItem> feeItemListByDate = feeList.FindAll(m => m.InDate == dateString);
                    List<string> feeTypeList = feeItemListByDate.GroupBy(m => m.FeeType).Select(g => g.Key).ToList<string>();
                    foreach (string feeType in feeTypeList)
                    {
                        dataXml.Append("<priceItem>");
                        dataXml.AppendFormat("<{0}>{1}</{0}>", "feeType", feeType);
                        dataXml.AppendFormat("<{0}>{1}</{0}>", "feeName", feeItemListByDate.Find(m=>m.FeeType == feeType).FeeName);
                        dataXml.AppendFormat("<{0}>{1}</{0}>", "amount", feeItemListByDate.FindAll(m => m.FeeType == feeType).Sum(g => g.Amount));
                        List<InFeeItem> feeItemListByFeeType = feeItemListByDate.FindAll(m => m.FeeType == feeType);
                        dataXml.Append("<details>");
                        foreach (InFeeItem item in feeItemListByFeeType)
                        {
                            dataXml.Append("<detail>");
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "code", item.Code);
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "name", item.Name);
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "unit", item.Unit);
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "price", item.Price);
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "count", item.Count);
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "spec", item.Space);
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "amount", item.Amount);
                            dataXml.Append("</detail>");
                        }
                        dataXml.Append("</details>");
                        dataXml.Append("</priceItem>");
                    }
                    dataXml.Append("</costItem>");
                    dataXml.Append("</item>");
                }

                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "出院小结")]
        public string Result(string req)
        {
            #region 入参模板

            //<Request><data>
            //<inpatNumber></inpatNumber>
            //<inpatId></inpatId>
            //</data></Request>

            #endregion
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string inpatNumber = Function.GetNoteValue(xmlDoc, "Request/data/inpatNumber");  //住院流水号
                Function.ValidateParameter(inpatNumber, "住院流水号");

                string inpatId = Function.GetNoteValue(xmlDoc, "Request/data/inpatId");  //住院号
                Function.ValidateParameter(inpatNumber, "住院流水号");


                System.Data.DataTable dtRes = CostManager.QueryOutSummay(inpatNumber, inpatId);

                if (dtRes == null)
                {
                    throw new Exception("出院小结失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有找到出院小结信息");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    for (int j = 0; j < dtRes.Columns.Count; j++)
                    {
                        dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j]);
                    }
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }
    }


    /// <summary>
    /// 住院费用明细
    /// </summary>
    public class InFeeItem
    {
        /// <summary>
        /// 住院日期
        /// </summary>
        private  string inDate;

        /// <summary>
        /// 住院日期
        /// </summary>
        public string InDate { get; set; }

        /// <summary>
        /// 当日产生总费用
        /// </summary>
        private decimal dayAmount;

        /// <summary>
        /// 当日产生总费用
        /// </summary>
        public decimal DayAmount { get; set; }

        /// <summary>
        /// 当日充值费用
        /// </summary>
        private decimal chargeAmount;

        /// <summary>
        /// 当日充值费用
        /// </summary>
        public decimal ChargeAmount { get; set; }

        /// <summary>
        /// 费用分类编码
        /// </summary>
        private string feeType;

        /// <summary>
        /// 费用分类编码
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 费用名称
        /// </summary>
        private string feeName;

        /// <summary>
        /// 费用名称
        /// </summary>
        public string FeeName { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        private string code;

        /// <summary>
        /// 项目编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        private string name;

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        private string unit;

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        private decimal price;

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        private string space;

        /// <summary>
        /// 规格
        /// </summary>
        public string Space { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        private decimal count;

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Count { get; set; }

        /// <summary>
        /// 项目总金额
        /// </summary>
        private decimal amount;

        /// <summary>
        /// 项目总金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}

