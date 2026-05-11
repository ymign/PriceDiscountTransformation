using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Models.ZZSB;
using System.Data;

namespace His.Business.ZZSB
{
    public class QueryData
    {
        #region xml转实体

        public static int GetItemDictionariesFromXml(string xml ,ref His.Models.ZZSB.ItemDictionaries obj)
        {
            //His.Models.ZZSB.ItemDictionaries obj = new His.Models.ZZSB.ItemDictionaries();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
                System.Xml.XmlNodeList UserID = doc.GetElementsByTagName("UserID");
                System.Xml.XmlNodeList PassWord = doc.GetElementsByTagName("PassWord");
                System.Xml.XmlNodeList DeviceID = doc.GetElementsByTagName("DeviceID");
                System.Xml.XmlNodeList ServiceCode = doc.GetElementsByTagName("ServiceCode");
                System.Xml.XmlNodeList BankCode = doc.GetElementsByTagName("BankCode");
                System.Xml.XmlNodeList HospCode = doc.GetElementsByTagName("HospCode");
                System.Xml.XmlNodeList AppCode = doc.GetElementsByTagName("AppCode");
                System.Xml.XmlNodeList AppTypeCode = doc.GetElementsByTagName("AppTypeCode");
                System.Xml.XmlNodeList FunCode = doc.GetElementsByTagName("FunCode");
                System.Xml.XmlNodeList ReqTime = doc.GetElementsByTagName("ReqTime");
                System.Xml.XmlNodeList ReqTraceNo = doc.GetElementsByTagName("ReqTraceNo");

                obj.UserID = UserID[0].InnerText;
                obj.PassWord = PassWord[0].InnerText;
                obj.DeviceID = DeviceID[0].InnerText;
                obj.ServiceCode = ServiceCode[0].InnerText;
                obj.BankCode = BankCode[0].InnerText;
                obj.HospCode = HospCode[0].InnerText;
                obj.AppCode = AppCode[0].InnerText;
                obj.AppTypeCode = AppTypeCode[0].InnerText;
                obj.FunCode = FunCode[0].InnerText;
                obj.ReqTime = Shadow.Util.Data.Func.NConvert.ToDateTime(ReqTime[0].InnerText);
                obj.ReqTraceNo = ReqTraceNo[0].InnerText;
            }
            catch (Exception e)
            {
                return -1;
            }
            return 1;
        }

        public static int GetItemDictionaryFromXml(string xml, ref His.Models.ZZSB.ItemDictionary obj)
        {
            //His.Models.ZZSB.ItemDictionaries obj = new His.Models.ZZSB.ItemDictionaries();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
                System.Xml.XmlNodeList UserID = doc.GetElementsByTagName("UserID");
                System.Xml.XmlNodeList PassWord = doc.GetElementsByTagName("PassWord");
                System.Xml.XmlNodeList DeviceID = doc.GetElementsByTagName("DeviceID");
                System.Xml.XmlNodeList ServiceCode = doc.GetElementsByTagName("ServiceCode");
                System.Xml.XmlNodeList BankCode = doc.GetElementsByTagName("BankCode");
                System.Xml.XmlNodeList HospCode = doc.GetElementsByTagName("HospCode");
                System.Xml.XmlNodeList AppCode = doc.GetElementsByTagName("AppCode");
                System.Xml.XmlNodeList AppTypeCode = doc.GetElementsByTagName("AppTypeCode");
                System.Xml.XmlNodeList FunCode = doc.GetElementsByTagName("FunCode");
                System.Xml.XmlNodeList ReqTime = doc.GetElementsByTagName("ReqTime");
                System.Xml.XmlNodeList ReqTraceNo = doc.GetElementsByTagName("ReqTraceNo");
                System.Xml.XmlNodeList TypeId = doc.GetElementsByTagName("TypeId");

                obj.UserID = UserID[0].InnerText;
                obj.PassWord = PassWord[0].InnerText;
                obj.DeviceID = DeviceID[0].InnerText;
                obj.ServiceCode = ServiceCode[0].InnerText;
                obj.BankCode = BankCode[0].InnerText;
                obj.HospCode = HospCode[0].InnerText;
                obj.AppCode = AppCode[0].InnerText;
                obj.AppTypeCode = AppTypeCode[0].InnerText;
                obj.FunCode = FunCode[0].InnerText;
                obj.ReqTime = Shadow.Util.Data.Func.NConvert.ToDateTime( ReqTime[0].InnerText);
                obj.ReqTraceNo = ReqTraceNo[0].InnerText;
                obj.TypeId = TypeId[0].InnerText;
            }
            catch (Exception e)
            {
                return -1;
            }
            return 1;
        }

        #endregion

        #region 逻辑
        /// <summary>
        /// 获取收费类别信息
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public His.Models.ZZSB.ItemTypeList GetDictionaries(ItemDictionaries reqInfo)
        {
            His.Models.ZZSB.ItemTypeList itemtypeList = new His.Models.ZZSB.ItemTypeList();

            if (reqInfo != null)
            {
                if (string.IsNullOrEmpty(reqInfo.DeviceID) || string.IsNullOrEmpty(reqInfo.ServiceCode) || string.IsNullOrEmpty(reqInfo.ReqTraceNo) || string.IsNullOrEmpty(reqInfo.FunCode))
                {
                    itemtypeList.ErrorMsg = "服务编码,设备编号,业务编号,请求流水号不能为空！";
                    itemtypeList.Code = "0";
                    return itemtypeList;
                }
            }
            else
            {
                itemtypeList.ErrorMsg = "请输入有效请求参数！";
                itemtypeList.Code = "0";
                return itemtypeList;
            }

           
            string SqlStr = @"
                        SELECT distinct y.fee_stat_cate, y.fee_stat_name
                           FROM fin_com_feecodestat y
                          where y.report_code in ( 'ZY01', 'MZ01')
                            and y.VALID_STATE = fun_get_valid
                        ";
            DataTable dt = new DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(SqlStr);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    His.Models.ZZSB.TypeInfo typeinfo = new His.Models.ZZSB.TypeInfo();
                    int i = 0;
                    typeinfo.TypeId = row[i].ToString();
                    i++;
                    typeinfo.TypeName = row[i].ToString();

                    itemtypeList.TypeInfoList.Add(typeinfo); 
                }
                itemtypeList.Code = "1";
                itemtypeList.FunCode = reqInfo.FunCode;
                itemtypeList.OpTime = Function.GetSysDate().ToString("yyyy-MM-dd HH:mm:ss");
                return itemtypeList;
            }
            else
            {
                itemtypeList.Code = "0";
                itemtypeList.ErrorMsg = "没有找到相关记录！";
                return itemtypeList;
            }
        }

        /// <summary>
        /// 获取收费类别信息
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public His.Models.ZZSB.ItemInfoList GetDictionary(ItemDictionary reqInfo)
        {
            His.Models.ZZSB.ItemInfoList iteminfolist = new His.Models.ZZSB.ItemInfoList();

            if (reqInfo != null)
            {
                if (string.IsNullOrEmpty(reqInfo.DeviceID) || string.IsNullOrEmpty(reqInfo.ServiceCode) || string.IsNullOrEmpty(reqInfo.ReqTraceNo) || string.IsNullOrEmpty(reqInfo.FunCode) || string.IsNullOrEmpty(reqInfo.TypeId))
                {
                    iteminfolist.ErrorMsg = "服务编码,设备编号,业务编号,收费类别,请求流水号不能为空！";
                    iteminfolist.Code = "0";
                    return iteminfolist;
                }
            }
            else
            {
                iteminfolist.ErrorMsg = "请输入有效请求参数！";
                iteminfolist.Code = "0";
                return iteminfolist;
            }


            string SqlStr = @"
                           select fee_stat_name,
       item_name,
       stock_unit,
       unit_price,
       mark,
       spell_code,
       mdt_price,
       UNIT_PRICE1,
       TX_Flag,
       sortord
  from (select ta.fee_stat_name,
               p.item_name,
               p.stock_unit,
               p.unit_price,
               p.mark,
               p.spell_code,
               p.mdt_price,
               p.UNIT_PRICE1,
               case
                 when (p.gb_code like '%T%' and p.item_name like '%T%') then
                  '1'
                 else
                  '0'
               end TX_Flag,
               nvl((case
                     when p.item_code in
                          (select cc.code
                             from com_dictionary cc
                            where cc.type = 'SCTJJXM'
                              and cc.valid_state = '1') THEN
                      '4'
                     when p.gb_code like '%N%' THEN
                      '2'
                     WHEN p.gb_code like '%F%' THEN
                      '3'
                     when p.gb_code like '%T%' THEN
                      '5'
                     ELSE
                      '1'
                   END),
                   '1') sortord
          from fin_com_undruginfo p
         inner join (SELECT distinct y.fee_code, y.fee_stat_name
                      FROM fin_com_feecodestat y
                     where y.report_code in ('ZY01', 'MZ01')
                       and y.VALID_STATE = fun_get_valid
                       and y.fee_stat_cate = '{0}') ta
            on (ta.fee_code = p.fee_code and p.unitflag = '0' and
               p.valid_state = fun_get_valid and p.unit_price <> 0)
        union
        select tb.fee_stat_name,
               s.trade_name,
               s.pack_unit,
               case
                 when s.class_code = 'PCZ' or s.class_code = 'P' then
                  s.retail_price2
                 when s.class_code = 'PCC' then
                  s.retail_price
                 else
                  s.purchase_price
               end purchase_price,
               s.mark,
               s.spell_code,
               null,
               null,
               '0',
               '1' sortord
          from pha_com_baseinfo s
         inner join (SELECT distinct y.fee_code, y.fee_stat_name
                      FROM fin_com_feecodestat y
                     where y.report_code in ('ZY01', 'MZ01')
                       and y.VALID_STATE = fun_get_valid
                       and y.fee_stat_cate = '{0}') tb
            on (tb.fee_code = s.fee_code and s.valid_state = fun_get_valid)
         where s.special_flag3 = 1
            or s.special_flag3 = 6)
 order by sortord asc

                            ";
            SqlStr = string.Format(SqlStr, reqInfo.TypeId);
            DataTable dt = new DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(SqlStr);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    His.Models.ZZSB.ItemInfo iteminfo = new His.Models.ZZSB.ItemInfo();
                    int i = 0;
                    iteminfo.TypeName = row[i].ToString();
                    i++;
                    iteminfo.CostName = row[i].ToString();
                    i++;
                    iteminfo.Unit = row[i].ToString();
                    i++;
                    iteminfo.Price = row[i].ToString();
                    i++;
                    iteminfo.Remark = row[i].ToString();
                    i++;
                    iteminfo.Alias = row[i].ToString();
                    i++;
                    iteminfo.Mdt_price = row[i].ToString();
                    i++;
                    iteminfo.ChildrenPrice = row[i].ToString();
                    i++;
                    iteminfo.TX_Flag = row[i].ToString();
                    i++;
                    iteminfolist.ItemList.Add(iteminfo);
                }
                iteminfolist.Code = "1";
                iteminfolist.FunCode = reqInfo.FunCode;
                iteminfolist.OpTime = Function.GetSysDate().ToString("yyyy-MM-dd HH:mm:ss");
                return iteminfolist;
            }
            else
            {
                iteminfolist.Code = "0";
                iteminfolist.ErrorMsg = "没有找到相关记录！";
                return iteminfolist;
            }
        }

        /// <summary>
        /// 查询交易记录
        /// </summary>
        /// <param name="TradeInfo"></param>
        /// <returns></returns>
        public string QueryTradeRecords(His.Models.ZZSB.TradeRecords TradeInfo)
        {
            if (string.IsNullOrEmpty(TradeInfo.TranserNo))
            {
                return Function.DataSource("0", "交易流水号不能为空！","0").ToString();
            }
            if (string.IsNullOrEmpty(TradeInfo.TYPE))
            {
                return Function.DataSource("0", "交易类型不能为空！", "0").ToString();
            }
            if (string.IsNullOrEmpty(TradeInfo.TOT_COST))
            {
                return Function.DataSource("0", "交易金额不能为空！", "0").ToString();
            }

            His.Models.ZZSB.TradeRecords TradeRecordInfo = new TradeRecords();
            string returnStr = "";
            string SqlStr = @"select TRANSERNO,--交易流水号
                                      INVOICE_NO,--发票号
                                      CLINIC_NO,--流水号
                                      CARDNO,--卡号
                                      NAME,--姓名
                                      ORDERID,--订单号
                                      PAY_TYPE,--支付方式
                                      TYPE,--交易类型
                                      TOT_COST,--交易金额
                                      DEVICEID,--设备号
                                      OPER_DATE,--操作日期
                                      REMARK,--备注
                                      PACTCODE--合同单位
                                       from FIN_OPB_TRADERECORDSZZSB 
                                       where TRANSERNO = '{0}'
                                       and TYPE = '{1}'
                                       and TOT_COST = '{2}'";

            SqlStr = string.Format(SqlStr, TradeInfo.TranserNo, TradeInfo.TYPE, TradeInfo.TOT_COST);
            DataTable dt = new DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(SqlStr);
            if (dt!=null&&dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TradeRecordInfo.TranserNo = dt.Rows[i][0].ToString();
                    TradeRecordInfo.INVOICE_NO = dt.Rows[i][1].ToString();
                    TradeRecordInfo.CLINIC_NO = dt.Rows[i][2].ToString();
                    TradeRecordInfo.CARDNO = dt.Rows[i][3].ToString();
                    TradeRecordInfo.NAME = dt.Rows[i][4].ToString();
                    TradeRecordInfo.ORDERID = dt.Rows[i][5].ToString();
                    TradeRecordInfo.PAY_TYPE = dt.Rows[i][6].ToString();
                    TradeRecordInfo.TYPE = dt.Rows[i][7].ToString();
                    TradeRecordInfo.TOT_COST = dt.Rows[i][8].ToString();
                    TradeRecordInfo.DEVICEID = dt.Rows[i][9].ToString();
                    TradeRecordInfo.OPER_DATE = dt.Rows[i][10].ToString();
                    TradeRecordInfo.REMARK = dt.Rows[i][11].ToString();
                    TradeRecordInfo.PACTCODE = dt.Rows[i][12].ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(TradeRecordInfo.TYPE))
            {
                return Function.DataSource("0", "查询不到交易记录！", "0").ToString();
            }
            else
            {
                switch (TradeRecordInfo.TYPE)
                {
                    case "1":
                        #region 返回串
                        try
                        {
                            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
                            System.Xml.XmlElement root = xml.CreateElement("DataSource");
                            xml.AppendChild(root);

                            System.Xml.XmlElement root1 = xml.CreateElement("return");
                            root.AppendChild(root1);

                            System.Xml.XmlElement Code = xml.CreateElement("Code");
                            Code.InnerText = "1";
                            root1.AppendChild(Code);

                            System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                            ErrorMsg.InnerText = "";
                            root1.AppendChild(ErrorMsg);

                            System.Xml.XmlElement Result = xml.CreateElement("Result");
                            root1.AppendChild(Result);

                            System.Xml.XmlElement TranSerNo = xml.CreateElement("TranSerNo");
                            TranSerNo.InnerText = TradeRecordInfo.TranserNo;
                            Result.AppendChild(TranSerNo);

                            System.Xml.XmlElement TotalRegFee = xml.CreateElement("TotalRegFee");
                            TotalRegFee.InnerText = "";
                            Result.AppendChild(TotalRegFee);

                            System.Xml.XmlElement RegFee = xml.CreateElement("RegFee");
                            RegFee.InnerText = "";
                            Result.AppendChild(RegFee);

                            System.Xml.XmlElement TreatFee = xml.CreateElement("TreatFee");
                            TreatFee.InnerText = "";
                            Result.AppendChild(TreatFee);

                            System.Xml.XmlElement PatientBookFee = xml.CreateElement("PatientBookFee");
                            PatientBookFee.InnerText = "";
                            Result.AppendChild(PatientBookFee);

                            System.Xml.XmlElement ServicesFee = xml.CreateElement("ServicesFee");
                            ServicesFee.InnerText = "0.00";
                            Result.AppendChild(ServicesFee);

                            System.Xml.XmlElement MetaFee = xml.CreateElement("MetaFee");
                            MetaFee.InnerText = "0.00";
                            Result.AppendChild(MetaFee);

                            System.Xml.XmlElement OtherFee = xml.CreateElement("OtherFee");
                            OtherFee.InnerText = "0.00";
                            Result.AppendChild(OtherFee);

                            System.Xml.XmlElement MedInsureFee = xml.CreateElement("MedInsureFee");
                            MedInsureFee.InnerText = "0.00";
                            Result.AppendChild(MedInsureFee);

                            System.Xml.XmlElement PersonalFee = xml.CreateElement("PersonalFee");
                            PersonalFee.InnerText = "0.00";
                            Result.AppendChild(PersonalFee);

                            System.Xml.XmlElement TreatLocation = xml.CreateElement("TreatLocation");
                            TreatLocation.InnerText = "";
                            Result.AppendChild(TreatLocation);

                            System.Xml.XmlElement WaitTreatNo = xml.CreateElement("WaitTreatNo");
                            WaitTreatNo.InnerText = "";
                            Result.AppendChild(WaitTreatNo);

                            System.Xml.XmlElement ReceiptNo = xml.CreateElement("ReceiptNo");
                            ReceiptNo.InnerText = TradeRecordInfo.INVOICE_NO;
                            Result.AppendChild(ReceiptNo);

                            System.Xml.XmlElement SortNo = xml.CreateElement("SortNo");
                            SortNo.InnerText = TradeRecordInfo.REMARK;
                            Result.AppendChild(SortNo);

                            System.Xml.XmlElement Note = xml.CreateElement("Note");
                            Note.InnerText = TradeRecordInfo.CLINIC_NO;
                            Result.AppendChild(Note);
                            returnStr = xml.InnerXml.ToString();
                        }
                        catch (Exception ex)
                        {
                            returnStr = Function.DataSource("0", ex.Message, "0").ToString();
                        }
                        #endregion
                        break;
                    case "2":
                        #region 返回串
                        try
                        {
                            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
                            System.Xml.XmlElement root = xml.CreateElement("DataSource");
                            xml.AppendChild(root);

                            System.Xml.XmlElement root1 = xml.CreateElement("return");
                            root.AppendChild(root1);

                            System.Xml.XmlElement Code = xml.CreateElement("Code");
                            Code.InnerText = "1";
                            root1.AppendChild(Code);

                            System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                            ErrorMsg.InnerText = "";
                            root1.AppendChild(ErrorMsg);

                            System.Xml.XmlElement Result = xml.CreateElement("Result");
                            root1.AppendChild(Result);

                            System.Xml.XmlElement TranSerNo = xml.CreateElement("TranSerNo");
                            TranSerNo.InnerText = TradeRecordInfo.TranserNo;
                            Result.AppendChild(TranSerNo);

                            System.Xml.XmlElement TotalRegFee = xml.CreateElement("TotalRegFee");
                            TotalRegFee.InnerText = "";
                            Result.AppendChild(TotalRegFee);

                            System.Xml.XmlElement RegFee = xml.CreateElement("RegFee");
                            RegFee.InnerText = "";
                            Result.AppendChild(RegFee);

                            System.Xml.XmlElement TreatFee = xml.CreateElement("TreatFee");
                            TreatFee.InnerText = "";
                            Result.AppendChild(TreatFee);

                            System.Xml.XmlElement PatientBookFee = xml.CreateElement("PatientBookFee");
                            PatientBookFee.InnerText = "";
                            Result.AppendChild(PatientBookFee);

                            System.Xml.XmlElement ServicesFee = xml.CreateElement("ServicesFee");
                            ServicesFee.InnerText = "0.00";
                            Result.AppendChild(ServicesFee);

                            System.Xml.XmlElement MetaFee = xml.CreateElement("MetaFee");
                            MetaFee.InnerText = "0.00";
                            Result.AppendChild(MetaFee);

                            System.Xml.XmlElement OtherFee = xml.CreateElement("OtherFee");
                            OtherFee.InnerText = "0.00";
                            Result.AppendChild(OtherFee);

                            System.Xml.XmlElement MedInsureFee = xml.CreateElement("MedInsureFee");
                            MedInsureFee.InnerText = "0.00";
                            Result.AppendChild(MedInsureFee);

                            System.Xml.XmlElement PersonalFee = xml.CreateElement("PersonalFee");
                            PersonalFee.InnerText = "0.00";
                            Result.AppendChild(PersonalFee);

                            System.Xml.XmlElement TreatLocation = xml.CreateElement("TreatLocation");
                            TreatLocation.InnerText = "";
                            Result.AppendChild(TreatLocation);

                            System.Xml.XmlElement WaitTreatNo = xml.CreateElement("WaitTreatNo");
                            WaitTreatNo.InnerText = "";
                            Result.AppendChild(WaitTreatNo);

                            System.Xml.XmlElement ReceiptNo = xml.CreateElement("ReceiptNo");
                            ReceiptNo.InnerText = TradeRecordInfo.INVOICE_NO;
                            Result.AppendChild(ReceiptNo);

                            System.Xml.XmlElement SortNo = xml.CreateElement("SortNo");
                            SortNo.InnerText = TradeRecordInfo.REMARK;
                            Result.AppendChild(SortNo);

                            System.Xml.XmlElement Note = xml.CreateElement("Note");
                            Note.InnerText = TradeRecordInfo.CLINIC_NO;
                            Result.AppendChild(Note);
                            returnStr = xml.InnerXml.ToString();
                        }
                        catch (Exception ex)
                        {
                            returnStr = Function.DataSource("0", ex.Message, "0").ToString();
                        }
                        #endregion
                        break;
                    case "3":
                        #region 返回串
                        try
                        {
                            DataSource source = new DataSource();
                            source.Return.ErrorMsg = string.Empty;
                            source.Return.Code = "1";
                            source.Return.FunCode = TradeRecordInfo.CLINIC_NO;
                            source.Return.OpTime = Function.GetSysDate().ToString("yyyy-MM-dd HH:mm:ss");
                            returnStr = His.Util.Common.XmlUtil.Serializer(source.GetType(), source);
                        }
                        catch (Exception ex)
                        {
                            returnStr = Function.DataSource("0", ex.Message, "0").ToString();
                        }
                        #endregion
                        break;
                    case "4":
                        #region 返回串
                        returnStr = @"
                                <DataSource>
                                     <return>
	                                     <Code>1</Code>
	                                     <ErrorMsg></ErrorMsg>
		                                 <OpTime>{0}</OpTime>
		                                 <FunCode>1804</FunCode>
	                                     <Result> 
			                                  <TranSerNo>{2}</TranSerNo>
                                          <InvoiceNo>{3}</InvoiceNo>
                                          <ExecAdress>{4}</ExecAdress>
                                          <Message>{5}</Message>
                                          <Note>{6}</Note>
	                                     </Result>
                                     </return>
                                </DataSource>
                                ";
                        returnStr = string.Format(returnStr, System.DateTime.Now.ToString(),
                                                "1", TradeRecordInfo.INVOICE_NO, TradeRecordInfo.INVOICE_NO, "", "", "");
                        #endregion
                        break;
                    case "5":
                        #region 返回串
                        try
                        {
                            string msg = "尊敬的：" + TradeRecordInfo.NAME + "\n" +
               "您已成功补交住院押金 " + TradeRecordInfo.TOT_COST +
               " 元，请前往住院收费处打印住院押金收据，谢谢！";

                            System.Xml.Linq.XElement result = new System.Xml.Linq.XElement("Result",
                                 new System.Xml.Linq.XElement("Balance", TradeRecordInfo.REMARK),
                                 new System.Xml.Linq.XElement("ReceptNo", TradeRecordInfo.INVOICE_NO),
                                 new System.Xml.Linq.XElement("Note"));
                            System.Xml.Linq.XElement root = Function.DataSource("1", msg, "0");
                            root.Element("return").Add(result);
                            returnStr = root.ToString();
                        }
                        catch (Exception ex)
                        {
                            returnStr = Function.DataSource("0", ex.Message, "0").ToString();
                        }
                        #endregion
                        break;
                    default:
                        returnStr = Function.DataSource("0", "查询不到" + TradeRecordInfo.TYPE + "的交易类型！", "0").ToString();
                        break;
                }
            }
            return returnStr;
        }
        #endregion

    }
}
