using System;

namespace Neusoft.HISFC.Models.Registration
{
    /// <summary>
    /// <br>RegLevel</br>
    /// <br>[功能描述: 预约挂号实体]</br>
    /// <br>[创 建 者: 黄小卫]</br>
    /// <br>[创建时间: 2007-2-1]</br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [Serializable]
    public class Booking:Neusoft.HISFC.Models.RADT.Patient
	{
		/// <summary>
		/// 
		/// </summary>
		public Booking()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//          
        }

        #region 变量

        #region 看诊信息
        /// <summary>
        /// 看诊信息
        /// </summary>
        private Schema doctor;
		#endregion 		
		
        /// <summary>
        /// 操作环境
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper;

        /// <summary>
        /// 是否已看诊
        /// </summary>
        private bool isSee;

        /// <summary>
        /// 每日流水号
        /// </summary>
        private int orderNO = 0;

        /// <summary>
        /// 确认人
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment confirmOper;

        /// <summary>
        /// 预约类型ID
        /// </summary>
        private string bookingTypeId = string.Empty;

        /// <summary>
        /// 预约类型名称
        /// </summary>
        private string bookingTypeName = string.Empty;

        #endregion

        #region 属性
        /// <summary>
        /// 预约类型ID
        /// </summary>
        public string BookingTypeId
        {
            get
            {
                return bookingTypeId;
            }
            set
            {
                bookingTypeId = value;
            }
        }

        /// <summary>
        /// 预约类型名称
        /// </summary>
        public string BookingTypeName
        {
            get
            {
                return bookingTypeName;
            }
            set
            {
                bookingTypeName = value;
            }
        }

        /// <summary>
        /// 看诊信息
        /// </summary>
        public Schema DoctorInfo
        {
            get {
                if (this.doctor == null)
                {
                    this.doctor = new Schema();
                }

                return this.doctor; }
            set { this.doctor = value; }
        }

        /// <summary>
        /// 每日流水号
        /// </summary>
        public int OrderNO
        {
            get { return orderNO; }
            set { orderNO = value; }
        }

        /// <summary>
        /// 是否已看诊
        /// </summary>
        public bool IsSee
        {
            get { return this.isSee; }
            set { this.isSee = value; }
        }

        /// <summary>
        /// 操作环境
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get {
                if (this.oper == null)
                {
                    this.oper = new Neusoft.HISFC.Models.Base.OperEnvironment();
                }
                return this.oper; }
            set { this.oper = value; }
        }

        /// <summary>
        /// 确认人
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment ConfirmOper
        {
            get {
                if (this.confirmOper == null)
                {
                    this.confirmOper = new Neusoft.HISFC.Models.Base.OperEnvironment();
                }
                return this.confirmOper; }
            set { this.confirmOper = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Booking Clone()
		{
            Booking obj = base.Clone() as Booking;

            obj.DoctorInfo = this.DoctorInfo.Clone();
            obj.Oper = this.Oper.Clone();

            return obj;
        }
        #endregion

        #region 废弃
        /// <summary>
        /// 病历号
        /// </summary>
        [Obsolete("更改为：PID.CardNO", true)]
        public string CardNo = "";

        /// <summary>
        /// 身份证
        /// </summary>
        [Obsolete("更改为:IDCard", true)]
        public string IdenNo = "";

        /// <summary>
        /// 性别
        /// </summary>
        [Obsolete("更改为：Sex.ID", true)]
        public string SexID = "";

        /// <summary>
        /// 联系电话
        /// </summary>
        [Obsolete("更改为：PhoneHome", true)]
        public string Phone = "";

        /// <summary>
        /// 地址
        /// </summary>
        [Obsolete("更改为：AddressHome", true)]
        public string Address = "";
        #endregion
	}
}
