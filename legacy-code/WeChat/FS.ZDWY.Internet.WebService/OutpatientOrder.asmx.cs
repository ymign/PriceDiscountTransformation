using FS.ZDWY.Internet.Models.Views.OutPatient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// outpatient 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class OutpatientOrder : System.Web.Services.WebService
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

        [WebMethod]
        public string HelloWorld()
        {
            FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager mgr = new BP.OutPatient.RegisterInfoManager();
            return mgr.Test();
        }

        [WebMethod(Description = "自助核酸开单")]
        public string preOrder(string req)
        {
            #region 入参模板
            /*
            <Request>
            <data>
            <parSequenceNo></parSequenceNo>
            <parCardNo></parCardNo>
            <parDoctcode></parDoctcode>
            <parDeptcode></parDeptcode>
            <parItemcode></parItemcode>
            <parUnitPrice></parUnitPrice>
            <parQty></parQty>
            <parOwnCost></parOwnCost>
            <parExecdeptcode></parExecdeptcode>
            <parExecdeptname></parExecdeptname>
            </data>
            </Request> 
            */

            #endregion

            #region 出参模板
            /*
            <Response>
            <ok>true</ok>
            <errorMsg></errorMsg><data>
            <discountsAmount></discountsAmount>
            <remark></remark>
            </data></Response>
            */

            #endregion

            ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<parClinicCode>{0}</parClinicCode>";
            #endregion

            #region 获取入参值并验证

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(req);
            List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parSequenceNo", NodeInstruction = "处方内流水号" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parCardNo", NodeInstruction = "门诊号" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parDoctcode", NodeInstruction = "开方医师工号" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parDeptcode", NodeInstruction = "开方医师所在科室" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parItemcode", NodeInstruction = "项目代码" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parUnitPrice", NodeInstruction = "单价" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parQty", NodeInstruction = "数量" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parOwnCost", NodeInstruction = "金额" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parExecdeptcode", NodeInstruction = "执行科室代码" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "parExecdeptname", NodeInstruction = "执行科室名称" });

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


            string parSequenceNo = nodesVales["parSequenceNo"];
            string parCardNo = nodesVales["parCardNo"];
            string parDoctcode = nodesVales["parDoctcode"];
            string parDeptcode = nodesVales["parDeptcode"];
            string parItemcode = nodesVales["parItemcode"];
            string parUnitPrice = (Function.ToDecimal(nodesVales["parUnitPrice"]) / 100).ToString();
            string parQty = nodesVales["parQty"];
            string parOwnCost = (Function.ToDecimal(nodesVales["parOwnCost"]) / 100).ToString();
            string parExecdeptcode = nodesVales["parExecdeptcode"];
            string parExecdeptname = nodesVales["parExecdeptname"];
            string parClinicCode = string.Empty;
            string parAppCode = string.Empty;
            string parErrMsg = string.Empty;
            #endregion

            #region 挂号减免
            string resXml = string.Empty;
            BP.OutPatient.OrderManager orderMgr = new BP.OutPatient.OrderManager();
            Models.Views.ComResult<Models.Views.OutPatient.AddOrderResult> result = orderMgr.AddNewOrder(parSequenceNo, parCardNo, parDoctcode, parDeptcode,
             parItemcode, parUnitPrice, parQty, parOwnCost,
             parExecdeptcode, parExecdeptname, ref parClinicCode, ref  parAppCode, ref  parErrMsg);

            if (!result.IsSuccessful)
            {
                resXml = Function.GetResponseXML(false, "操作失败！" + result.Message, "");
            }
            else
            {
                resXml = Function.GetResponseXML(true, "操作成功",
                    string.Format(dataXml, result.ReturnData.clinicCode)
                    );
            }
            ServiceLogManager.Write("传出报文：" + resXml);
            return resXml;

            #endregion
        }

        [WebMethod(Description = "自助开单")]
        public string SelfServiceAddOrder(string req)
        {
            //自助开单
            #region 入参注释
            //            <Request>
            //<data>
            //<patientName></patientName>
            //<patientCardNo></patientCardNo>
            //<sourceFlag></sourceFlag>
            //<ItemList>
            //<doctCode></doctCode>
            //<doctName></doctName>
            //<deptCode></deptCode>
            //<itemCode></itemCode>
            //<itemName></itemName>
            //<unitPrice></unitPrice>
            //<qty></qty>
            //<ownCost></ownCost>
            //<execDeptCode></execDeptCode>
            //<execDeptName></execDeptName>
            //</ItemList>
            //<ItemList>
            //<doctCode></doctCode>
            //<doctName></doctName>
            //<deptCode></deptCode>
            //.........
            //</ItemList>
            //</data> 
            #endregion
            string guid = Guid.NewGuid().ToString();
            ServiceLogManager.Write("自助开单入参【" + guid + "】" + req);
            string resXml = string.Empty;
            string errMsg = string.Empty;
            string dataXml = @"<clinicCode>{0}</clinicCode>";
            SelfServiceAddOrderRequsetModel reqModel = new SelfServiceAddOrderRequsetModel();
            try
            {
                reqModel = FS.ZDWY.Internet.BP.Common.Xml.XmlDeSerializeToModel<SelfServiceAddOrderRequsetModel>(req, "Request/data", ref errMsg);
                reqModel.itemList = FS.ZDWY.Internet.BP.Common.Xml.XmlDeSerializeToList<item>(req, "Request/data/itemList/item", ref errMsg);
                foreach (var item in reqModel.itemList)
                {
                    item.unitPrice = item.unitPrice / 100;
                    item.ownCost = item.ownCost / 100;
                }
                FS.ZDWY.Internet.BP.OutPatient.OrderManager orderManager = new BP.OutPatient.OrderManager();
                var result = orderManager.SelfServiceAddOrder(reqModel);
                
                if (!result.IsSuccessful)
                {
                    resXml = Function.GetResponseXML(false, "操作失败！" + result.Message, "");
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功",
                        string.Format(dataXml, result.ReturnData.clinicCode)
                        );
                }
            }
            catch (Exception ex)
            {
                resXml = Function.GetResponseXML(false, "操作失败！" + ex.Message, "");
            }
            ServiceLogManager.Write("自助开单出参【" + guid + "】" + resXml);
            return resXml;

        }





    }
}
