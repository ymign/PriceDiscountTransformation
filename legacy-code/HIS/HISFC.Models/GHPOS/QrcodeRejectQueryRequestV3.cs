using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sdk_cop
{
        class QrcodeRejectQueryRequestV3 : AbstractIcbcRequest<QrcodeRejectQueryResponseV3>
        {
            /// <summary>
            /// 获取响应类
            /// </summary>
            /// <returns>返回QrcodeRejectQueryRequestV3类对应的响应类类型</returns>
            public override Type getResponseClass()
            {
                return Type.GetType("sdk_cop.QrcodeRejectQueryResponseV3");
            }
            /// <summary>
            /// QrcodeRejectQueryRequestV3，设置请求URL
            /// </summary>
            public QrcodeRejectQueryRequestV3()
            {
                this.setServiceUrl("https://gw.open.icbc.com.cn/api/qrcode/reject/query/V3");
            }
            /// <summary>
            /// 是否支持加密（目前sdk只支持对字段进行AES加解密）
            /// </summary>
            /// <returns>一般返回false</returns>
            public override Boolean isNeedEncrypt()
            {
                return false;
            }
            /// <summary>
            /// http发起请求方式（支持GET、POST两种方式）
            /// </summary>
            /// <returns>一般返回POST</returns>
            public override String getMethod()
            {
                return "POST";
            }
            /// <summary>
            /// 获取业务类类型
            /// </summary>
            /// <returns>返回request类内部定义的BizContent类的类型</returns>
            public override Type getBizContentClass()
            {
                return Type.GetType("sdk_cop" + ".QrcodeRejectQueryRequestV3+QrcodeRejectQueryRequestV3Biz", true, true);
            }
            /// <summary>
            /// 内部业务类，封装需要发送给服务端的业务字段
            /// </summary>
            [DataContract]
            public class QrcodeRejectQueryRequestV3Biz : BizContent
            {
                [DataMember]
                private String mer_id;
                [DataMember]
                private String out_trade_no;
                [DataMember]
                private String order_id;
                [DataMember]
                private String cust_id;
                [DataMember]
                private String reject_no;

                public String getMerId(){
                    return mer_id;
                }
                public void setMerId(String value){
                    mer_id = value;
                }

                public String getOutTradeNo(){
                    return out_trade_no;
                }
                public void setOutTradeNo(String value){
                    out_trade_no = value;
                }

                public String getOrderId(){
                    return order_id;
                }
                public void setOrderId(String value){
                    order_id = value;
                }

                public String getCustId(){
                    return cust_id;
                }
                public void setCustId(String value){
                    cust_id = value;
                }

                public String getRejectNo(){
                    return reject_no;
                }
                public void setRejectNo(String value){
                    reject_no = value;
                }
        }
        }
}
