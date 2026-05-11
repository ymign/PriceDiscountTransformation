using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace His.Models.ZZSB
{
    public class ItemInfoList
    {
        /// <summary>
        /// Code	String	结果值	
        /// </summary>
        private string code = string.Empty;
        /// <summary>
        /// Code	String	结果值	
        /// </summary>
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        /// <summary>
        /// ErrorMsg	String	错误码	Y(必填)
        /// </summary>
        private string errorMsg = string.Empty;
        /// <summary>
        /// ErrorMsg	String	错误码	Y(必填)
        /// </summary>
        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }

        /// <summary>
        /// FunCode	String	业务编号	
        /// </summary>
        private string opTime = string.Empty;
        /// <summary>
        /// FunCode	String	业务编号	
        /// </summary>
        public string OpTime
        {
            get { return opTime; }
            set { opTime = value; }
        }

        /// <summary>
        /// OpTime	DateTime	响应时间	Y(必填)
        /// </summary>
        private string funCode = string.Empty;
        /// <summary>
        /// OpTime	DateTime	响应时间	Y(必填)
        /// </summary>
        public string FunCode
        {
            get { return funCode; }
            set { funCode = value; }
        }

        /// <summary>
        /// 项目信息集合
        /// </summary>
        private ArrayList itemList = new ArrayList();

        /// <summary>
        /// 项目信息集合
        /// </summary>
        public ArrayList ItemList
        {
            get { return itemList; }
            set { itemList = value; }
        }
    }

    public class ItemInfo
    {

        public string TX_Flag { get; set; }

        /// <summary>
        /// TypeName	string	项目类别名称	Y(必填)
        /// </summary>
        private string typeName = string.Empty;
        /// <summary>
        /// TypeName	string	项目类别名称	Y(必填)
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        /// <summary>
        /// CostName	String	收费项目名称	Y(必填)
        /// </summary>
        private string costName = string.Empty;
        /// <summary>
        /// CostName	String	收费项目名称	Y(必填)
        /// </summary>
        public string CostName
        {
            get { return costName; }
            set { costName = value; }
        }

        /// <summary>
        /// Unit	String	单位	Y(必填)
        /// </summary>
        private string unit = string.Empty;
        /// <summary>
        /// Unit	String	单位	Y(必填)
        /// </summary>
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        /// <summary>
        /// Price	String 	价格	Y(必填)
        /// </summary>
        private string price = string.Empty;
        /// <summary>
        /// Price	String 	价格	Y(必填)
        /// </summary>
        public string Price
        {
            get { return price; }
            set { price = value; }
        }

        /// <summary>
        /// Remark	String	备注	
        /// </summary>
        private string remark = string.Empty;
        /// <summary>
        /// Remark	String	备注	
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        /// <summary>
        /// Alias	String	收费项目的拼音码	Y(必填)
        /// </summary>
        private string alias = string.Empty;
        /// <summary>
        /// Alias	String	收费项目的拼音码	Y(必填)
        /// </summary>
        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        /// <summary>
        /// Alias	String	mdt价格	Y(必填)
        /// </summary>
        private string mdt_price = string.Empty;
        /// <summary>
        /// Alias	String	mdt价格	Y(必填)
        /// </summary>
        public string Mdt_price
        {
            get { return mdt_price; }
            set { mdt_price = value; }
        }

        public string ChildrenPrice { get; set; }


    }
}
