using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;

namespace His.Service
{
    /// <summary>
    /// Recipe 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Recipe : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取门诊西药房处方")]
        public string GetRecipeInfo()
        {
            string xml = string.Empty, msg = string.Empty;
            Business.Recipe.RecipeInfo mgr = new His.Business.Recipe.RecipeInfo();
            if (mgr.QueryRecipeInfo(ref xml, ref msg) != 1)
            {
                xml = (new XElement("ROOT",
                    new XElement("MSG", msg))).ToString();
            }
            return xml;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "更新处方接收状态")]
        public string UpdateRecipeInfo(string recipeNo)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(recipeNo))
            {
                msg = (new XElement("ROOT",
                    new XElement("MSG", "处方号不能为空"))).ToString();
            }
            Business.Recipe.RecipeInfo mgr = new His.Business.Recipe.RecipeInfo();


            if (mgr.UpdateRecipeFlag(recipeNo.Split(',').ToList(), ref msg) == -1)
            {
                msg = (new XElement("ROOT",
                   new XElement("MSG", msg))).ToString();
            }

            else
            {
                msg = (new XElement("ROOT",
                 new XElement("MSG", "更新成功"))).ToString();
            }

            return msg;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "摆药完成接收通知")]
        public string UpdateDrugedState(string xml)
        {
            try
            {
                //日志
                His.Util.Common.HisLog.WriteLog("recipe", xml);
                string err = string.Empty,result=string.Empty;
                if (string.IsNullOrEmpty(xml))
                {
                     result= new XElement("ROOT",
                        new XElement("RETVAL", "0"),
                        new XElement("RETMSG", "入参不能为空！"),
                        new XElement("RETCODE", "0")
                        ).ToString();
                     His.Util.Common.HisLog.WriteLog("recipe", result);
                     return result;
                }
                XElement Req = XElement.Parse(xml);
                IEnumerable<XElement> list = from e in Req.Elements("CONSIS_PRESC_MSTVW") select e;
                string emplCode=Req.Element("OPMANNO").Value;

                List<XElement> x = list.ToList();
                Dictionary<string, string> noList = new Dictionary<string, string>();
                x.ForEach(f => { noList.Add(f.Element("PRESC_NO").Value, emplCode); });
                string no = string.Empty;
               // noList.ForEach(f => { no += f.ToString() + ","; });
               // no = no.Substring(0, no.Length - 1);
                Business.Recipe.RecipeInfo mgr = new His.Business.Recipe.RecipeInfo();
                if (noList.Count<1)
                {
                    err = "没有找到相关处方！";
                     result= new XElement("ROOT",
                       new XElement("RETVAL", "0"),
                       new XElement("RETMSG", err),
                       new XElement("RETCODE", "0")
                       ).ToString();
                    His.Util.Common.HisLog.WriteLog("recipe", result);
                    return result;
                }

                if (mgr.UpdateDrugedState(noList, ref err) == -1)
                {
                    result= new XElement("ROOT",
                       new XElement("RETVAL", "0"),
                       new XElement("RETMSG", err),
                       new XElement("RETCODE", "0")
                       ).ToString();
                    His.Util.Common.HisLog.WriteLog("recipe", result);
                    return result;
                }
                else
                {
                    result= new XElement("ROOT",
                                          new XElement("RETVAL", "0"),
                                          new XElement("RETMSG", no + "处方配药状态更新成功!"),
                                          new XElement("RETCODE", "1")
                                          ).ToString();
                    His.Util.Common.HisLog.WriteLog("recipe", result);
                    return result;
                }

            }

            catch (Exception ex)
            {
                string result= new XElement("ROOT",
                        new XElement("RETVAL", "0"),
                        new XElement("RETMSG", "更新配药完成出错！错误信息：" + ex.Message),
                        new XElement("RETCODE", "0")
                        ).ToString();
                His.Util.Common.HisLog.WriteLog("recipe", result);
                return result;
            }
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "住院发药机配药状态更新接口")]
        public string UpdateDrugedStatus(string xml)
        {
            /*
             <OPWINID></OPWINID>
<OPTYPE>301</OPTYPE>
<OPIP>192.168.1.11</OPIP>
<OPMANNO>12345</OPMANNO>
<OPMANNAME>马丽丽</OPMANNAME>
<IPACK_BASIC_PRESCRIPTION_STATE>
<RecipeNo>处方号</RecipeNo>[新增处方号]
<PresDetailID>1242424</PresDetailID>
<PrescriptionID>54545457</PrescriptionID>
<SendStatus>0</SendStatus>
</IPACK_BASIC_PRESCRIPTION_STATE>
<IPACK_BASIC_PRESCRIPTION_STATE>
<RecipeNo>处方号</RecipeNo>
<PresDetailID>1242424</PresDetailID>
<PrescriptionID>54545457</PrescriptionID>
<SendStatus>0</SendStatus>
</IPACK_BASIC_PRESCRIPTION_STATE>
</ROOT>
             */
            //日志
            His.Util.Common.HisLog.WriteLog("inRecipe", xml);
            string err = string.Empty, result = string.Empty;
            if (string.IsNullOrEmpty(xml))
            {
                result = new XElement("ROOT",
                   new XElement("RETVAL", "0"),
                   new XElement("RETMSG", "入参不能为空！"),
                   new XElement("RETCODE", "0")
                   ).ToString();
                His.Util.Common.HisLog.WriteLog("inRecipe", result);
                return result;
            }

            string parm = string.Empty;
            XElement Req = XElement.Parse(xml);
            IEnumerable<XElement> list = from e in Req.Elements("CONSIS_PRESC_MSTVW") select e;
            string emplCode = Req.Element("OPMANNO").Value;
            string emplName = Req.Element("OPMANNAME").Value;
            His.Models.Pha.RecipeBase recipe = new His.Models.Pha.RecipeBase();
            List<XElement> items = Req.Elements("IPACK_BASIC_PRESCRIPTION_STATE").ToList();
            foreach (var item in items)
            {
                His.Models.Pha.RecipeStatusInfo info = new His.Models.Pha.RecipeStatusInfo();
                info.PrescriptionID = item.Element("PrescriptionID").Value;
                info.RecipeNo = item.Element("RecipeNo").Value;
                info.PresDetailID = item.Element("PresDetailID").Value;
                info.SendStatus = item.Element("SendStatus").Value;
                recipe.Details.Add(info);
            }

            if (recipe!=null &&recipe.Details!=null&&recipe.Details.Count>0)
            {
               parm= new His.Business.Recipe.RecipeInfo().UpdateDrugedInStatus(recipe.Details);
            }

            His.Util.Common.HisLog.WriteLog("inRecipe", parm);
            return parm;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "Test")]
        public string TestDB()
        {
            return new Shadow.Util.Data.Management.OracleBase().GetDateTimeFromSysDateTime().ToString();
        }
    }
}
