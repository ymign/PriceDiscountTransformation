using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace His.Service
{
    public partial class TestServceForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //  xmlns=\"http://www.google.com\"  xmlns=\"http://www.baidu.com\"
    //       Pathologic p = new Pathologic();
//            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?> "+
//"<message>"+
//         "<Request>"+
//           @"  <APLY_FLOW_NUM>11687757</APLY_FLOW_NUM>
//	         <BILL_NO></BILL_NO>
//	         <PATIENT_TYPE></PATIENT_TYPE>
//	         <PATIENT_ID>552413</PATIENT_ID>
//		 <SAMPLE_BARNUM></SAMPLE_BARNUM>
//	         <CARDNO></CARDNO>
//			 <EMPI></EMPI>
//			 <START_TIME></START_TIME>
//			 <END_TIME></END_TIME>
//			 <PATIENT_NAME></PATIENT_NAME>
//	         <EXAM_TYPE></EXAM_TYPE>
//         </Request>
//	 </message>";
//        //    p.getOutPathologicApply(xml);

//            xml = @"<message>
//         <Request>
//             <APLY_FLOW_NUM>11692437</APLY_FLOW_NUM>
//	         <BILL_NO></BILL_NO>
//	         <PATIENT_TYPE></PATIENT_TYPE>0
//	         <PATIENT_ID>0000157494</PATIENT_ID>
//		 <SAMPLE_BARNUM></SAMPLE_BARNUM>
//	         <CARDNO></CARDNO>
//			 <EMPI></EMPI>
//			 <START_TIME></START_TIME>
//			 <END_TIME></END_TIME>
//			 <PATIENT_NAME></PATIENT_NAME>
//	         <EXAM_TYPE></EXAM_TYPE>
//         </Request>
//	 </message>";
//           // p.getInpPathologicApply(xml);

//            xml = "<message xmlns=\"http://www.baidu.com\">"+
//     @"<Request>
//         <PATIENTINFO>
//			 <ORDER_ID></ORDER_ID>
//			 <PATIENT_TYPE>0</PATIENT_TYPE>
//			 <PATIENT_ID>552413</PATIENT_ID>
//			 <EMPI></EMPI>
//			 <CARDNO></CARDNO>
//			 <PATIENT_NAME></PATIENT_NAME>
//			 <PATIENT_SEX></PATIENT_SEX>
//			 <PATIENT_BIRTH></PATIENT_BIRTH>
//	     </PATIENTINFO>
//	     <OPERATORINFO>
//			 <OPER_CODE></OPER_CODE>
//			 <OPER_NAME></OPER_NAME>
//	     </OPERATORINFO>
//	     <CHECKININFO>
//			 <APLY_FLOW_NUM>11687757</APLY_FLOW_NUM>
//			 <EXAM_SYSTEM_CODE></EXAM_SYSTEM_CODE>
//			 <APLY_ITM_CODE></APLY_ITM_CODE>
//			 <APLY_ITM_NAME></APLY_ITM_NAME>
//			 <CHECK_REG_NUM></CHECK_REG_NUM>
//			 <CHECK_REG_TIME></CHECK_REG_TIME>
//	     </CHECKININFO>
//     </Request>
//</message>";
//            Endoscope ee = new Endoscope();
//            ee.QuseryOutEndoscopeFeeStatus(xml);

            //<DataSource><message><UserID></UserID><PassWord></PassWord><DeviceID>111</DeviceID><ServiceCode>111</ServiceCode><FunCode>1</FunCode><HospCode></HospCode><CardTypeCode>2</CardTypeCode><clinic_code>712148</clinic_code><card_no>9500000042</card_no><PAR_SEX_CODE>F</PAR_SEX_CODE><PAR_IDENNO>20160426</PAR_IDENNO><PAR_RELA_PHONE>20160803</PAR_RELA_PHONE><PAR_ADDRESS>777</PAR_ADDRESS><PAR_DEPT_CODE>7021</PAR_DEPT_CODE><PAR_DEPT_NAME>体检中心</PAR_DEPT_NAME><PAR_BIRTHDAY>2016-08-03</PAR_BIRTHDAY><PAR_OPER_CODE>009999</PAR_OPER_CODE><InvoiceNo></InvoiceNo></message></DataSource>
            string xml =
                @"<DataSource><message><UserID></UserID><PassWord></PassWord><DeviceID>111</DeviceID><ServiceCode>111</ServiceCode><FunCode>1</FunCode><HospCode></HospCode><PAR_SEQUENCE_NO>9</PAR_SEQUENCE_NO><clinic_code>713644</clinic_code><card_no>9588881111</card_no><PAR_DEPTCODE>7021</PAR_DEPTCODE><PAR_ITEMCODE>F00000010813</PAR_ITEMCODE><PAR_UNIT_PRICE>2</PAR_UNIT_PRICE><PAR_QTY>0</PAR_QTY><PAR_OWN_COST>7</PAR_OWN_COST><PAR_EXECDEPTCODE>7021</PAR_EXECDEPTCODE><PAR_EXECDEPTNAME>体检中心</PAR_EXECDEPTNAME><PAR_OPER_CODE>009999</PAR_OPER_CODE><PAR_RECIPE_NUM>2822073</PAR_RECIPE_NUM><InvoiceNo></InvoiceNo></message></DataSource>";
           //His.Business.BUltrasonic.BUltrasonic tnf = new His.Business.BUltrasonic.BUltrasonic();
            His.Business.ZWTJ.PEPatInfoChange ap = new His.Business.ZWTJ.PEPatInfoChange();
            ap.TjPEPatInfoChange(xml);

           
            //His.Service.ZWTJ tnf = new ZWTJ();
            //His.Business.ZWTJ.PERegisterInfo jj = new His.Business.ZWTJ.PERegisterInfo();
            //int PAR_DEPTCODE = 7021;
            //jj.GetPEPatFee("1", "9588881111", 009999,ref PAR_DEPTCODE, "F00000021927", "11", "1", "11", "7021", "体检中心");
            //tnf.GetPEPatFee(xml);

        }
    }
}
