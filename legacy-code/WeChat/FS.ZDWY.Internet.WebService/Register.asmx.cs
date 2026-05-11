using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// Register 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Register : System.Web.Services.WebService
    {
        #region 属性

        BP.OutPatient.PatientInfoManager patientManager;
        /// <summary>
        /// 患者基本信息管理
        /// </summary>
        BP.OutPatient.PatientInfoManager PatientManager
        {
            get
            {
                if (patientManager == null)
                {
                    patientManager = new BP.OutPatient.PatientInfoManager();
                }
                return patientManager;
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

        #endregion

        [WebMethod(Description = "预约挂号（锁号占用号源）")]
        public string Order(string req)
        {
            #region 入参模板
            /*
            <Request><data>
<orderId></orderId>
<orderTime></orderTime>
<deptCode></deptCode>
<doctorCode></doctorCode>
<scheduleDate></scheduleDate>
<scheduleId></scheduleId>
<timeFlag></timeFlag>
<timeId></timeId>
<beginTime></beginTime>
<endTime></endTime>
<regFee></regFee>
<visitFlag></visitFlag>
<patientId></patientId>
<cardType></cardType>
<cardNo></cardNo>
<type></type>
<name></name>
<sex></sex>
<age></age>
<birth></birth>
<address></address>
<mobile></mobile>
<regType></ regType>
<frontProviderId></frontProviderId>
<certifcateType></certifcateType>
<certifcateNo></certifcateNo>
<guardName></guardName>
<guardidType></guardidType>
<guardidNo></guardidNo>
<source></source>
<payChannel></payChannel>
</data></Request>
            */
            #endregion

            ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<orderId>{0}</orderId>
                        <hospitalNum>{1}</hospitalNum>
                        <visitNo>{2}</visitNo>
                        <visitAddress>{3}</visitAddress>
                        <remark>{4}</remark>
                        <proof>{5}</proof>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderTime", NodeInstruction = "下单时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "deptCode", NodeInstruction = "科室代码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "doctorCode", NodeInstruction = "医生代码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "scheduleDate", NodeInstruction = "号源日期" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "scheduleId", NodeInstruction = "班次ID" });
                //中大五院的分时号源ID就是排班ID
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "numberinfoId", NodeInstruction = "分时号源ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "beginTime", NodeInstruction = "分时开始时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "endTime", NodeInstruction = "分时结束时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "regFee", NodeInstruction = "挂号费" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "cardType", NodeInstruction = "诊疗卡类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "cardNo", NodeInstruction = "诊疗卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "type", NodeInstruction = "患者类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "name", NodeInstruction = "姓名" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "sex", NodeInstruction = "性别" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "age", NodeInstruction = "年龄" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "birth", NodeInstruction = "出生日期" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "address", NodeInstruction = "地址" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "mobile", NodeInstruction = "电话" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "regType", NodeInstruction = "挂号类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "frontProviderId", NodeInstruction = "第三方服务商ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "院内用户Id" });

                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateType", NodeInstruction = "证件类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateNo", NodeInstruction = "证件号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "guardName", NodeInstruction = "监护人姓名" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "guardidType", NodeInstruction = "监护人证件类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "guardidNo", NodeInstruction = "监护人证件号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "payMethod", NodeInstruction = "支付方式" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "source", NodeInstruction = "数据来源" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "payChannel", NodeInstruction = "支付渠道" });
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                #endregion

                #region 抽取信息

                //订单信息
                //就按这个入参信息存到新的订单表里
                Models.PLATFORM_REGISTER_ORDER order = new Models.PLATFORM_REGISTER_ORDER();
                order.ORDERID = nodesVales["orderId"];
                order.ORDERTIME = Function.ToDateTime(nodesVales["orderTime"]);
                order.DEPTCODE = nodesVales["deptCode"];
                order.DOCTORCODE = nodesVales["doctorCode"];
                order.SCHEDULEDATE = Function.ToDateTime(nodesVales["scheduleDate"]);
                order.SCHEDULEID = nodesVales["scheduleId"];
                order.NUMBERINFOID = nodesVales["numberinfoId"];
                order.BEGINTIME = nodesVales["beginTime"];
                order.ENDTIME = nodesVales["endTime"];
                order.REGFEE = nodesVales["regFee"];
                order.CARDTYPE = nodesVales["cardType"];
                order.CARDNO = nodesVales["cardNo"];
                order.TYPE = nodesVales["type"];
                order.NAME = nodesVales["name"];
                order.SEX = nodesVales["sex"];
                order.AGE = nodesVales["age"];
                order.BIRTH = Function.ToDateTime(nodesVales["birth"]);
                order.ADDRESS = nodesVales["address"];
                order.MOBILE = nodesVales["mobile"];
                order.REGTYPE = nodesVales["regType"];
                order.FRONTPROVIDERID = nodesVales["frontProviderId"];
                order.CERTIFCATETYPE = nodesVales["certifcateType"];
                order.CERTIFCATENO = nodesVales["certifcateNo"];
                order.PATIENTID = nodesVales["patientId"];
                order.GUARDNAME = nodesVales["guardName"];
                order.GUARDIDTYPE = nodesVales["guardidType"];
                order.GUARDIDNO = nodesVales["guardidNo"];
                order.SOURCE = nodesVales["frontProviderId"] + "微信";
                order.OPERCODE = Function.DefaultOper.Code;
                if (nodesVales["payChannel"] == "2")
                {
                    order.SOURCE = nodesVales["frontProviderId"] + "支付宝";
                    order.OPERCODE = Function.ZFBOper.Code;
                }
                else if (nodesVales["payChannel"] == "3")
                {
                    order.SOURCE = nodesVales["frontProviderId"] + "APP";
                    order.OPERCODE = Function.APPOper.Code;
                }
                order.OPERNAME = Function.DefaultOper.Name;
                order.PAYMETHOD = nodesVales["payMethod"];
                #endregion

                string resXml = string.Empty;
                BP.OutPatient.RegisterInfoManager registerManager = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<Models.Views.OrderResult> result = registerManager.Order(order, Function.DefaultOper);
                if (!result.IsSuccessful)
                {
                    throw new Exception("挂号失败。" + result.Message);
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功",
                        string.Format(dataXml, result.ReturnData.OrderId, result.ReturnData.HospitalNum, result.ReturnData.VisitNo, result.ReturnData.VisitAddress, result.ReturnData.Remark, result.ReturnData.Proof)
                        );
                }
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

        [WebMethod(Description = "退号服务（释放号源）")]
        public string Cancel(string req)
        {
            ServiceLogManager.Write("退号服务（释放号源）Cancel传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                string orderId = Function.GetNoteValue(xmlDoc, "Request/data/orderId");  //平台定单号
                Function.ValidateParameter(orderId, "平台定单号");
                string hospitalNum = Function.GetNoteValue(xmlDoc, "Request/data/hospitalNum");  //医院订单号
                Function.ValidateParameter(hospitalNum, "医院订单号");
                string cancelReason = Function.GetNoteValue(xmlDoc, "Request/data/cancelReason");  //取消原因 1、超时，2、用户主动取消
                Function.ValidateParameter(cancelReason, "取消原因");
                string frontProviderId = Function.GetNoteValue(xmlDoc, "Request/data/frontProviderId");  //第三方服务商ID
                Function.ValidateParameter(frontProviderId, "第三方服务商ID");
                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId");  //院内用户ID
                Function.ValidateParameter(patientId, "院内用户ID");
                string cancelTime = Function.GetNoteValue(xmlDoc, "Request/data/cancelTime");  //取消时间
                string clincCode = Function.GetNoteValue(xmlDoc, "Request/data/clincCode");//门诊流水号
                //Function.ValidateParameter(clincCode, "门诊流水号");
                BP.OutPatient.RegisterInfoManager registerManager = new BP.OutPatient.RegisterInfoManager();
                string error = string.Empty;
                int res = registerManager.CancelLock(orderId, hospitalNum, patientId, cancelReason, frontProviderId, Function.ToDateTime(cancelTime), clincCode, ref error);
                if (res <= 0)
                {
                    throw new Exception(error);
                }
                string resXml = Function.GetResponseXML(true, "操作成功", string.Empty);
                ServiceLogManager.Write("退号服务（释放号源）Cancel传出报文：" + resXml);
                return resXml;

            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("退号服务（释放号源）Cancel传出报文：" + resXml);
                return resXml;
            }

        }



        [WebMethod(Description = "保存加号信息")]
        public string SaveAddRegisterInfo(string req)
        {
            #region 入参模板
            /*
           <Request>
<orderId></orderId>
<orderTime></orderTime>
<deptCode></deptCode>
<doctorCode></doctorCode>
<scheduleDate></scheduleDate>
<cardNo></cardNo>
<name></name>
<regType></regType>
<sourceFlag></sourceFlag>
<operCode></operCode>
<regLevel></regLevel>
</Request>
            */
            #endregion

            ServiceLogManager.Write("保存加号传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"
                                <orderId>{0}</orderId>
                                <hospitalNum>{1}</hospitalNum>
                                <visitNo>{2}</visitNo>
                                <visitAddress>{3}</visitAddress>
                                <remark>{4}</remark>
                                <proof>{5}</proof> 
                                <regFee>{6}</regFee>
                                ";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderTime", NodeInstruction = "下单时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "deptCode", NodeInstruction = "科室代码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "doctorCode", NodeInstruction = "医生代码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "scheduleDate", NodeInstruction = "号源日期" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "cardNo", NodeInstruction = "诊疗卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "name", NodeInstruction = "姓名" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "regType", NodeInstruction = "挂号类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "sourceFlag", NodeInstruction = "数据来源" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "operCode", NodeInstruction = "操作员" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "noonCode", NodeInstruction = "午别" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "regLevel", NodeInstruction = "挂号级别" });
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                #endregion

                #region 抽取信息

                //订单信息
                //就按这个入参信息存到新的订单表里
                Models.PLATFORM_REGISTER_ORDER order = new Models.PLATFORM_REGISTER_ORDER();
                order.ORDERID = nodesVales["orderId"];
                order.ORDERTIME = Function.ToDateTime(nodesVales["orderTime"]);
                order.DEPTCODE = nodesVales["deptCode"];
                order.DOCTORCODE = nodesVales["doctorCode"];
                order.SCHEDULEDATE = Function.ToDateTime(nodesVales["scheduleDate"]);
                order.CARDNO = nodesVales["cardNo"];
                order.NAME = nodesVales["name"];
                order.REGTYPE = nodesVales["regType"];
                order.SOURCE = nodesVales["sourceFlag"] + "微信";
                order.OPERCODE = nodesVales["operCode"];
                order.OPERNAME = Function.DefaultOper.Name;
                order.PAYMETHOD = "1";
                DateTime dtBegin = order.SCHEDULEDATE.Value.Date;
                DateTime dtEnd = order.SCHEDULEDATE.Value.Date;
                if (nodesVales["noonCode"] != "1" && nodesVales["noonCode"] != "2")
                {
                    throw new Exception("入参格式不正确。字段：noonCode 只能为1或2");
                }


                if (nodesVales["noonCode"] == "1")
                {

                    dtBegin = order.SCHEDULEDATE.Value.Date;
                    dtEnd = order.SCHEDULEDATE.Value.Date.AddHours(12);
                }
                else
                {
                    dtBegin = order.SCHEDULEDATE.Value.Date.AddHours(12);
                    dtEnd = order.SCHEDULEDATE.Value.AddDays(1);
                }

                #endregion

                string resXml = string.Empty;
                BP.OutPatient.RegisterInfoManager registerManager = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<Models.Views.OrderResult> result = registerManager.AddRegister(order, Function.DefaultOper, nodesVales["noonCode"], nodesVales["regLevel"]);
                if (!result.IsSuccessful)
                {
                    throw new Exception("加号失败。" + result.Message);
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "加号成功",
                        string.Format(dataXml, result.ReturnData.OrderId, result.ReturnData.HospitalNum, result.ReturnData.VisitNo, result.ReturnData.VisitAddress, result.ReturnData.Remark, result.ReturnData.Proof, result.ReturnData.RegFee)
                        );
                }
                ServiceLogManager.Write("保存加号传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("保存加号传出报文：" + resXml);
                return resXml;
            }
        }

    }
}
