using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace His.Business.ZZSB
{
    public class ZFPTService
    {
        public ZFPTService()
        {
            if (mgr == null)
            {
                mgr = new RegisterManager();
            }
        }

        RegisterManager mgr = null;

        public bool ZFPTInvoiceBinding(His.Models.ZZSB.PayPlatform.InvoiceBinding invoiceBinding, ref string Msg)
        {
            string url = mgr.ExecSqlReturnOne("select d.NAME from com_dictionary d where d.type='His.ZFPTServiceServiceUrl' and d.code = 'CreditPayInvoiceBinding' and d.valid_state='1'");
            if (string.IsNullOrEmpty(url))
            {
                Msg = "支付平台发票绑定接口没有配置接口地址！请先配置CreditPayInvoiceBinding";
                return false;
            }
            string request = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceBinding);
            ///string rq=string.Format("")
            string response = this.Post(url, request, "application/json;charset=UTF-8");
            His.Models.ZZSB.PayPlatform.Response<string> responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<His.Models.ZZSB.PayPlatform.Response<string>>(response);
            if (responseModel.Code == "1")
            {
                Msg = responseModel.Msg;
                return true;
            }
            else
            {
                Msg = responseModel.Msg;
                return false;
            }
        }
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        /// <summary>
        /// post方式请求数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param">post的数据</param>
        /// <returns></returns>
        private string Post(string url, string param, string contentType)
        {
            HttpWebRequest request;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = contentType;
            request.Accept = "*/*";
            request.Timeout = 120000;
            request.AllowAutoRedirect = false;
            StreamWriter stream = null;
            WebResponse response = null;
            string result = string.Empty;
            try
            {
                using (stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(param);
                    stream.Close();
                }
                using (response = request.GetResponse())
                {
                    if (null != response)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();
                            reader.Close();
                        }
                    }
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                stream = null;
                response = null;
            }
            return result;
        }
    }
}
