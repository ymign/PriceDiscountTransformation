using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Neusoft.HISFC.BizLogic.HealthCard
{
    public class HealthCardManager
    {
        //调阅提示接口
        
        //private static extern int patientexist(string unitCode, string departmentCode, string userName,
               //string password, string cardType, string cardNumber);
        [DllImport("getprivateinfo.dll", SetLastError = true)]
        private static extern int patientexist(string unitCode, string departmentCode,string staffNo,string name, string userName,
               string password, string cardType, string cardNumber, string businesType, string businessSerialNO);
        //患者健康档案调阅 add byMajq
        [DllImport("rhin_ehr_browser.dll", SetLastError = true)]
        private static extern int rhin_ehr_browser(string unitCode, string departmentCode, string staffNo, string name, string userName,
               string password, string cardType, string cardNumber, string businesType, string businessSerialNO);

        //患者健康档案调阅
        [DllImport("qyws_dykj.dll", SetLastError = true)]
        private static extern void QUWS_DYHZXX(string unitCode, string departmentCode, string userName,
               string password, string cardType, string cardNumber);

        //重复发卡查询
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int hadcard(string unitCode, string departmentCode, string userName,
               string password, string name, string sex, string idType, string id, string birthday,
               string phone, string mphone, string province, string city, string section, string address,
               string idAddress, string profession, StringBuilder cardType, StringBuilder cardNumber);

        //发卡
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int createcard(string unitCode, string departmentCode, string userName,
               string password, string cardType, string cardNumber, string cardPassword, string name,
               string sex, string idType, string id, string birthday, string phone, string mphone,
               string province, string city, string section, string address, string idAddress,
               string profession, string createcardUnit, string createcardTime, string secretLevel,
               string agreeAccess, string agreeWeb);

        //发卡（带照片）
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int createcardwithphoto(string unitCode, string departmentCode, string userName,
               string password, string cardType, string cardNumber, string cardPassword, string name,
               string sex, string idType, string id, string birthday, string phone, string mphone,
               string province, string city, string section, string address, string idAddress,
               string profession, string createcardUnit, string createcardTime, string secretLevel,
               string agreeAccess, string agreeWeb,int photolength,byte[] photo);

        //挂失
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int lostcard(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string cardPassword, string name,
                string idType, string id, string lostType);

        //解挂失
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int unlostcard(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string cardPassword, string name,
                string idType, string id);

        //锁卡
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int lockcard(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string name, string lockReason,
                string lockByName);

        //解锁
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int unlockcard(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string name, string unlockReason,
                string unlockByName);

        //修改密码
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int changepassword(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string name, string idType, string id,
                string oldPassword, string newPassword);

        //密码重置
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int resetpassword(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string name, string idType, string id,
                string newPassword);

        //换卡(补卡)
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int changecard(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string name, string idType, string id,
                string newCardType, string newCardNumber, string newPassword);

        //卡注销
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int logoutcard(string unitCode, string departmentCode, string userName,
                string password, string cardType, string cardNumber, string name, string idType, string id,
                string logoutReason, string logoutTime, string logoutByName);

        //获取患者基本信息
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int getpatientinfo(string unitCode, string departmentCode, string userName,
               string password, string cardType, string cardNumber, StringBuilder name, StringBuilder sex,
               StringBuilder idType, StringBuilder id, StringBuilder birthday, StringBuilder phone,
               StringBuilder mphone, StringBuilder address, StringBuilder idAddress, StringBuilder profession);

        //获取患者基本信息
        [DllImport("qyws_card.dll", SetLastError = true)]
        private static extern int getpatientinfowithphoto(string unitCode, string departmentCode, string userName,
               string password, string cardType, string cardNumber, StringBuilder name, StringBuilder sex,
               StringBuilder idType, StringBuilder id, StringBuilder birthday, StringBuilder phone,
               StringBuilder mphone, StringBuilder address, StringBuilder idAddress, StringBuilder profession, byte[] photo, ref int photolength);

        //错误信息
        public string Err = string.Empty;

        /// <summary>
        /// 调阅提示接口
        /// </summary>
        /// <param name="healthCard"></param>
        /// <returns></returns>
        public int PatientExist(HealthCard card,string staffNo,string name,string businesType, string businessSerialNO)
        {
            this.Err = string.Empty;
            int existResult;
             
            existResult = patientexist(card.UnitCode, card.DepartmentCode,  staffNo, name,card.UserName, card.Password, card.CardType, card.CardNumber,businesType,businessSerialNO);
           
            switch (existResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有调阅权限";
                    break;
                case 3:
                    this.Err = "没有患者信息";
                    break;
                case 4:
                    this.Err = "没有数据";
                    break;
                case 5:
                    this.Err = "只有本院数据";
                    break;
                case 6:
                    this.Err = "只有外院数据";
                    break;
                case 7:
                    this.Err = "都有数据";
                    break;
                case 8:
                    this.Err = "没有调阅患者权限";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return existResult;
        }

        /// <summary>
        /// 患者健康档案调阅
        /// </summary>
        /// <param name="healthCard"></param>
        /// <returns></returns>
        public void PopHealthRecord(HealthCard card, string staffNo, string name, string businesType, string businessSerialNO)
        {
            this.Err = string.Empty;
            rhin_ehr_browser(card.UnitCode, card.DepartmentCode, staffNo, name, card.UserName, card.Password, card.CardType, card.CardNumber, businesType, businessSerialNO);
           // QUWS_DYHZXX(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber);
        }

        /// <summary>
        /// 患者健康档案调阅
        /// </summary>
        /// <param name="healthCard"></param>
        /// <returns></returns>
        public void PopHealthRecord(Neusoft.HISFC.Models.Registration.Register mnuSelectRegister)
        {
            SOC.Public.Ini.IniFilesUtil iniFile = new Neusoft.SOC.Public.Ini.IniFilesUtil(Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + "\\quws_system.ini");
            string hostaddr = iniFile.ReadString("setup", "hostaddr", "http://127.0.0.1/WebEmr/Default.aspx?inpatientID=21636&recordID=&Out=0");
            System.Uri url = new Uri(string.Format(hostaddr, mnuSelectRegister.ID));
            frmShowInfo frmShowInfo = new frmShowInfo();
            frmShowInfo.webBrowser1.Url = url;
            frmShowInfo.Show();
        }

        /// <summary>
        /// 发卡
        /// </summary>
        /// <param name="healthCard"></param>
        /// <returns></returns>
        public int CreateCard(HealthCard card)
        {
            this.Err = string.Empty;
            int createResult;
            if (card.Photo == null)
            {
                createResult = createcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                                    card.CardPassword, card.Name, card.Sex, card.IdType, card.Id, card.Birthday, card.Phone, card.Mphone,
                                    card.Province, card.City, card.Section, card.Address, card.IdAddress, card.Profession, card.CreatecardUnit,
                                    card.CreatecardTime, card.SecretLevel, card.AgreeAccess, card.AgreeWeb);
            }
            else
            {
                createResult = createcardwithphoto(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                                    card.CardPassword, card.Name, card.Sex, card.IdType, card.Id, card.Birthday, card.Phone, card.Mphone,
                                    card.Province, card.City, card.Section, card.Address, card.IdAddress, card.Profession, card.CreatecardUnit,
                                    card.CreatecardTime, card.SecretLevel, card.AgreeAccess, card.AgreeWeb,card.Photo.Length,card.Photo);
                //如果是=9 重新调用无照片的方法
                if (createResult == 9)
                {
                    createResult = createcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                                    card.CardPassword, card.Name, card.Sex, card.IdType, card.Id, card.Birthday, card.Phone, card.Mphone,
                                    card.Province, card.City, card.Section, card.Address, card.IdAddress, card.Profession, card.CreatecardUnit,
                                    card.CreatecardTime, card.SecretLevel, card.AgreeAccess, card.AgreeWeb);
                }
            }
            switch (createResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "发卡失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return createResult;
        }

        /// <summary>
        /// 重复发卡查询
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int HadCard(HealthCard card)
        {
            this.Err = string.Empty;
            int hadResult;
            StringBuilder cardType = new StringBuilder(10);
            StringBuilder cardNumber = new StringBuilder(30);

            hadResult = hadcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.Name, card.Sex, card.IdType,
                                card.Id, card.Birthday, card.Phone, card.Mphone, card.Province, card.City, card.Section, card.Address,
                                card.IdAddress, card.Profession, cardType, cardNumber);

            switch (hadResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    card.CardType = cardType.ToString();
                    card.CardNumber = cardNumber.ToString();
                    this.Err = "已有健康卡信息";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return hadResult;
        }

        /// <summary>
        /// 换卡(补卡)
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int ChangeCard(HealthCard card)
        {
            this.Err = string.Empty;
            int changeResult;
            changeResult = changecard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                                card.Name, card.IdType, card.Id, card.NewCardType, card.NewCardNumber, card.NewPassword);

            switch (changeResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "无原来卡号";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return changeResult;
        }

        /// <summary>
        /// 挂失
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int LostCard(HealthCard card)
        {
            this.Err = string.Empty;
            int lostResult;
            lostResult = lostcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                             card.CardPassword, card.Name, card.IdType, card.Id, card.LostType);

            switch (lostResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "卡号或密码错误";
                    break;
                case 5:
                    this.Err = "卡状态不正常";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return lostResult;
        }

        /// <summary>
        /// 解挂失
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int UnLostCard(HealthCard card)
        {
            this.Err = string.Empty;
            int unlostResult;
            unlostResult = unlostcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                             card.CardPassword, card.Name, card.IdType, card.Id);

            switch (unlostResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "卡号或密码错误";
                    break;
                case 5:
                    this.Err = "卡状态为正常";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return unlostResult;
        }

        /// <summary>
        /// 锁卡
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int LockCard(HealthCard card)
        {
            this.Err = string.Empty;
            int lockResult;
            lockResult = lockcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                             card.Name, card.LockReason, card.LockByName);

            switch (lockResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "无此卡号";
                    break;
                case 5:
                    this.Err = "卡状态不正常";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return lockResult;
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int UnLockCard(HealthCard card)
        {
            this.Err = string.Empty;
            int unlockResult;
            unlockResult = unlockcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                             card.Name, card.UnlockReason, card.UnlockByName);

            switch (unlockResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "无此卡号";
                    break;
                case 5:
                    this.Err = "卡状态不正常";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return unlockResult;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int ChangePassword(HealthCard card)
        {
            this.Err = string.Empty;
            int changeResult;
            changeResult = changepassword(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                             card.Name, card.IdType, card.Id, card.OldPassword, card.NewPassword);

            switch (changeResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "卡号或密码错误";
                    break;
                case 5:
                    this.Err = "卡状态不正常";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return changeResult;
        }

        /// <summary>
        /// 密码重置
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int ResetPassword(HealthCard card)
        {
            this.Err = string.Empty;
            int resetResult;
            resetResult = resetpassword(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                                card.Name, card.IdType, card.Id, card.NewPassword);

            switch (resetResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "卡号或密码错误";
                    break;
                case 5:
                    this.Err = "卡状态不正常";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return resetResult;
        }

        /// <summary>
        /// 卡注销
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int LogoutCard(HealthCard card)
        {
            this.Err = string.Empty;
            int logoutResult;
            logoutResult = logoutcard(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                                card.Name, card.IdType, card.Id, card.LogoutReason, card.LogoutTime, card.LogoutByName);

            switch (logoutResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    break;
                case 4:
                    this.Err = "无卡号";
                    break;
                case 5:
                    this.Err = "卡状态不正常（该卡已注销）";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return logoutResult;
        }

        /// <summary>
        /// 获取患者基本信息
        /// </summary>
        /// <param name="healthCard"></param>
        /// <returns></returns>
        public int GetPatientInfo(HealthCard card)
        {
            this.Err = string.Empty;
            int getResult;
            StringBuilder name = new StringBuilder(20);
            StringBuilder sex = new StringBuilder(1);
            StringBuilder idType = new StringBuilder(2);
            StringBuilder id = new StringBuilder(30);
            StringBuilder birthday = new StringBuilder(8);
            StringBuilder phone = new StringBuilder(20);
            StringBuilder mphone = new StringBuilder(11);
            StringBuilder address = new StringBuilder(60);
            StringBuilder idAddress = new StringBuilder(60);
            StringBuilder profession = new StringBuilder(4);
            byte[] photo = new byte[1024 * 1000];
            int length=0;

            getResult = getpatientinfowithphoto(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
                               name, sex, idType, id, birthday, phone, mphone, address, idAddress,
                               profession, photo, ref length);

            //getResult = getpatientinfo(card.UnitCode, card.DepartmentCode, card.UserName, card.Password, card.CardType, card.CardNumber,
            //                   name, sex, idType, id, birthday, phone, mphone, address, idAddress,
            //                   profession);

            switch (getResult)
            {
                case 1:
                    this.Err = "无此登录名或密码错误";
                    break;
                case 2:
                    this.Err = "没有权限";
                    break;
                case 3:
                    card.Name = name.ToString();
                    card.Sex = sex.ToString();
                    card.IdType = idType.ToString();
                    card.Id = id.ToString();
                    card.Birthday = birthday.ToString();
                    card.Phone = phone.ToString();
                    card.Mphone = mphone.ToString();
                    card.Address = address.ToString();
                    card.IdAddress = idAddress.ToString();
                    card.Profession = profession.ToString();
                    if (length > 0)
                    {
                        card.Photo = photo;// Encoding.Default.GetBytes(photo.ToString(0, length));
                    }
                    break;
                case 4:
                    this.Err = "无卡号或该卡已停用";
                    break;
                case 6:
                    this.Err = "操作失败";
                    break;
                case 7:
                    this.Err = "无此患者权限";
                    break;
                case 9:
                    this.Err = "网络连接失败或超时";
                    break;
                default:
                    break;
            }
            return getResult;
        }
    }
}
