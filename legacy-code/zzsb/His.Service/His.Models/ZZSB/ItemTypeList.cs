using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace His.Models.ZZSB
{
    public class ItemTypeList
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
        /// 项目类别信息集合
        /// </summary>
        private ArrayList typeInfoList = new ArrayList();

        /// <summary>
        /// 项目类别信息集合
        /// </summary>
        public ArrayList TypeInfoList
        {
            get { return typeInfoList; }
            set { typeInfoList = value; }
        }
    }

    public class TypeInfo
    {
        /// <summary>
        /// TypeId	string	项目编码	Y(必填)
        /// </summary>
        private string typeId = string.Empty;
        /// <summary>
        /// TypeId	string	项目编码	Y(必填)
        /// </summary>
        public string TypeId
        {
            get { return typeId; }
            set { typeId = value; }
        }

        /// <summary>
        /// TypeName	string	项目名称	Y(必填)
        /// </summary>
        private string typeName = string.Empty;
        /// <summary>
        /// TypeName	string	项目名称	Y(必填)
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }
    }
}
