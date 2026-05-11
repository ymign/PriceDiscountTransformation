using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace His.Models.ZZSB
{
    public class BooKInFoList
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
        /// 预约患者基本信息集合
        /// </summary>
        private ArrayList baseInfoList = new ArrayList();

        /// <summary>
        /// 预约患者基本信息集合
        /// </summary>
        public ArrayList BaseInfoList
        {
            get { return baseInfoList; }
            set { baseInfoList = value; }
        }
    }

    public class BooKBaseInFo
    {
        /// <summary>
        /// String	交易流水号	Y(必填)
        /// </summary>
        private string tranSerNo = string.Empty;
        /// <summary>
        /// String	交易流水号	Y(必填)
        /// </summary>
        public string TranSerNo
        {
            get { return tranSerNo; }
            set { tranSerNo = value; }
        }

        /// <summary>
        /// string	预约的日期	Y(必填)
        /// </summary>
        private string orderDate = string.Empty;
        /// <summary>
        /// string	预约的日期	Y(必填)
        /// </summary>
        public string OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; }
        }

        /// <summary>
        /// String	医生姓名	Y(必填)
        /// </summary>
        private string doctorName = string.Empty;
        /// <summary>
        /// String	医生姓名	Y(必填)
        /// </summary>
        public string DoctorName
        {
            get { return doctorName; }
            set { doctorName = value; }
        }

        /// <summary>
        /// DoctorCode	String	医生编号	Y(必填)
        /// </summary>
        private string doctorCode = string.Empty;
        /// <summary>
        /// DoctorCode	String	医生编号	Y(必填)
        /// </summary>
        public string DoctorCode
        {
            get { return doctorCode; }
            set { doctorCode = value; }
        }

        /// <summary>
        /// DeptCode	String	科室编号	Y(必填)
        /// </summary>
        private string deptCode = string.Empty;
        /// <summary>
        /// DeptCode	String	科室编号	Y(必填)
        /// </summary>
        public string DeptCode
        {
            get { return deptCode; }
            set { deptCode = value; }
        }

        /// <summary>
        /// DeptName	string	科室名称	Y(必填)
        /// </summary>
        private string deptName = string.Empty;
        /// <summary>
        /// DeptName	string	科室名称	Y(必填)
        /// </summary>
        public string DeptName
        {
            get { return deptName; }
            set { deptName = value; }
        }

        /// <summary>
        /// TotalRegFee	Decimal	总挂号费	Y(必填)
        /// </summary>
        private string totalRegFee = string.Empty;
        /// <summary>
        /// TotalRegFee	Decimal	总挂号费	Y(必填)
        /// </summary>
        public string TotalRegFee
        {
            get { return totalRegFee; }
            set { totalRegFee = value; }
        }

        /// <summary>
        /// OrderType	string	预约类型	
        /// </summary>
        private string orderType = string.Empty;
        /// <summary>
        /// OrderType	string	预约类型	
        /// </summary>
        public string OrderType
        {
            get { return orderType; }
            set { orderType = value; }
        }

        /// <summary>
        /// OrderStatus	String	预约状态	
        /// </summary>
        private string orderStatus = string.Empty;
        /// <summary>
        /// OrderStatus	String	预约状态	
        /// </summary>
        public string OrderStatus
        {
            get { return orderStatus; }
            set { orderStatus = value; }
        }

        /// <summary>
        /// PatientName	String	预约人姓名	Y(必填)
        /// </summary>
        private string patientName = string.Empty;
        /// <summary>
        /// PatientName	String	预约人姓名	Y(必填)
        /// </summary>
        public string PatientName
        {
            get { return patientName; }
            set { patientName = value; }
        }


        /// <summary>
        /// TranNum	string	预约流水号	Y(必填)
        /// </summary>
        private string tranNum = string.Empty;
        /// <summary>
        /// TranNum	string	预约流水号	Y(必填)
        /// </summary>
        public string TranNum
        {
            get { return tranNum; }
            set { tranNum = value; }
        }

        /// <summary>
        /// String	业务编号	
        /// </summary>
        private string funCode = string.Empty;
        /// <summary>
        /// String	业务编号	
        /// </summary>
        public string FunCode
        {
            get { return funCode; }
            set { funCode = value; }
        }
    }
}
