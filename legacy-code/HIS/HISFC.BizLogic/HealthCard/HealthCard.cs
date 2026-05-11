using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace Neusoft.HISFC.BizLogic.HealthCard
{
    public class HealthCard
    {
        public HealthCard()
		{
            this.GetCustomizeInfo();
		}

        private string unitCode = string.Empty;
        private string departmentCode = string.Empty;
        private string userName = string.Empty;
		private string password = string.Empty;
        private string cardType = "0";
        private string cardNumber = string.Empty;
        private string cardPassword = string.Empty;
		private string name = string.Empty;
        private string sex = string.Empty;
        private string idType = string.Empty;
        private string id = string.Empty;
        private string birthday = string.Empty;
        private string phone = string.Empty;
        private string mphone = string.Empty;
        private string province = string.Empty;
        private string city = string.Empty;
        private string section = string.Empty;
        private string address = string.Empty;
        private string idAddress = string.Empty;
        private string profession = string.Empty;
        private string createcardUnit = string.Empty;
        private string createcardTime = DateTime.Now.ToString("yyyyMMdd");
        private string secretLevel = "0";
        private string agreeAccess = "1";
        private string agreeWeb = "1";
        private string lostType = string.Empty;
        private string lockReason = string.Empty;
        private string lockByName = string.Empty;
        private string unlockReason = string.Empty;
        private string unlockByName = string.Empty;
        private string oldPassword = string.Empty;
        private string newPassword = string.Empty;
        private string newCardType = "0";
        private string newCardNumber = string.Empty;
        private string logoutReason = string.Empty;
        private string logoutTime = string.Empty;
        private string logoutByName = string.Empty;
        private byte[] photo = null;

		/// <summary>
        /// 机构编码
		/// </summary>
        public string UnitCode
		{
            get { return unitCode; }
            set { unitCode = value; }
		}

        /// <summary>
        /// 科室编码
		/// </summary>
        public string DepartmentCode
		{
            get { return departmentCode; }
            set { departmentCode = value; }
		}

		/// <summary>
        /// 用户名/登录名
		/// </summary>
        public string UserName
		{
            get { return userName; }
            set { userName = value; }
		}

		/// <summary>
        /// 登录密码
		/// </summary>
        public string Password
		{
            get { return password; }
            set { password = value; }
		}

		/// <summary>
        /// 卡类型
		/// </summary>
        public string CardType
		{
            get { return cardType; }
            set { cardType = value; }
		}

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string CardPassword
        {
            get { return cardPassword; }
            set { cardPassword = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get { return sex; }
            set { sex = value; }
        }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string IdType
        {
            get { return idType; }
            set { idType = value; }
        }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday
        {
            get { return birthday; }
            set { birthday = value; }
        }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mphone
        {
            get { return mphone; }
            set { mphone = value; }
        }

        /// <summary>
        /// 居住地所在省
        /// </summary>
        public string Province
        {
            get { return province; }
            set { province = value; }
        }

        /// <summary>
        /// 居住地所在市
        /// </summary>
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        /// <summary>
        /// 居住地区县
        /// </summary>
        public string Section
        {
            get { return section; }
            set { section = value; }
        }

        /// <summary>
        /// 居住详细地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// 身份证地址
        /// </summary>
        public string IdAddress
        {
            get { return idAddress; }
            set { idAddress = value; }
        }

        /// <summary>
        /// 职业类别
        /// </summary>
        public string Profession
        {
            get { return profession; }
            set { profession = value; }
        }

        /// <summary>
        /// 发卡机构
        /// </summary>
        public string CreatecardUnit
        {
            get { return createcardUnit; }
            set { createcardUnit = value; }
        }

        /// <summary>
        /// 发卡时间
        /// </summary>
        public string CreatecardTime
        {
            get { return createcardTime; }
            set { createcardTime = value; }
        }

        /// <summary>
        /// 密级标识
        /// </summary>
        public string SecretLevel
        {
            get { return secretLevel; }
            set { secretLevel = value; }
        }

        /// <summary>
        /// 同意查阅病史
        /// </summary>
        public string AgreeAccess
        {
            get { return agreeAccess; }
            set { agreeAccess = value; }
        }

        /// <summary>
        /// 开通网上查询
        /// </summary>
        public string AgreeWeb
        {
            get { return agreeWeb; }
            set { agreeWeb = value; }
        }

        /// <summary>
        /// 挂失方式
        /// </summary>
        public string LostType
        {
            get { return lostType; }
            set { lostType = value; }
        }

        /// <summary>
        /// 锁卡原因
        /// </summary>
        public string LockReason
        { 
            get { return lockReason; }
            set { lockReason = value; }
        }

        /// <summary>
        /// 锁卡人姓名
        /// </summary>
        public string LockByName
        {
            get { return lockByName; }
            set { lockByName = value; }
        }

        /// <summary>
        /// 解锁原因
        /// </summary>
        public string UnlockReason
        {
            get { return unlockReason; }
            set { unlockReason = value; }
        }

        /// <summary>
        /// 解锁原因
        /// </summary>
        public string UnlockByName
        {
            get { return unlockByName; }
            set { unlockByName = value; }
        }

        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword
        {
            get { return oldPassword; }
            set { oldPassword = value; }
        }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value; }
        }

        /// <summary>
        /// 新卡类型
        /// </summary>
        public string NewCardType
        {
            get { return newCardType; }
            set { newCardType = value; }
        }

        /// <summary>
        /// 新卡号码
        /// </summary>
        public string NewCardNumber
        {
            get { return newCardNumber; }
            set { newCardNumber = value; }
        }

        /// <summary>
        /// 注销原因
        /// </summary>
        public string LogoutReason
        {
            get { return logoutReason; }
            set { logoutReason = value; }
        }

        /// <summary>
        /// 注销日期
        /// </summary>
        public string LogoutTime
        {
            get { return logoutTime; }
            set { logoutTime = value; }
        }

        /// <summary>
        /// 注销人姓名
        /// </summary>
        public string LogoutByName
        {
            get { return logoutByName; }
            set { logoutByName = value; }
        }

        private void GetCustomizeInfo()
        {
            /**
             * 这里没有try——catch就这样读文件,如果文件未找到会抛出异常的!
             * 很多地方初始化时调用new Neusoft.HISFC.BizLogic.HealthCard(),建议在此处理异常并抛出
             * 例如门诊医生站患者列表树Neusoft.HISFC.Components.Order.OutPatient.Controls.ucOutPatientTree
             */
            //string fileName = Application.StartupPath + "\\HealthCardService.xml";
            string fileName = null;
            fileName = System.AppDomain.CurrentDomain.BaseDirectory + "\\HealthCardService.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNode node = doc.SelectSingleNode(@"/设置/医院设置");
            this.unitCode = node.Attributes["UnitCode"].Value;
            this.userName = node.Attributes["UserName"].Value;
            this.password = node.Attributes["Password"].Value;
            this.createcardUnit = node.Attributes["UnitCode"].Value;
        }

        public byte[] Photo
        {
            get
            {
                return this.photo;
            }
            set
            {
                this.photo = value;
            }
        }
    }
}