using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Registration
{
    public class AppointmentOrder
    {
        public AppointmentOrder()
        {

        }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID = "";

        /// <summary>
        /// 身份证件号码
        /// </summary>
        public string UserIdCard = "";

        /// <summary>
        /// 健康卡号码
        /// </summary>
        public string UserJKK = "";

        /// <summary>
        /// 市民卡号码
        /// </summary>
        public string UserSMK = "";

        /// <summary>
        /// 医保卡号码
        /// </summary>
        public string UserYBK = "";

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName = "";

        /// <summary>
        /// 用户性别：M-男性 F-女性
        /// </summary>
        public string UserGender = "";

        /// <summary>
        /// 用户电话
        /// </summary>
        public string UserMobile = "";

        /// <summary>
        /// 用户出生日期
        /// </summary>
        public DateTime UserBirthday = DateTime.MinValue;

        /// <summary>
        /// 座席工号
        /// </summary>
        public string AgentId = "";

        /// <summary>
        /// 用户选择 1-换医生 2-退费
        /// </summary>
        public string UserChoice = "";

        /// <summary>
        /// 预约方式 0-网络 1-电话（12580） 2-电话（114） 3-自助终端
        /// </summary>
        public string OrderType = "";

        /// <summary>
        /// 使用排班序号
        /// </summary>
        public string SchemaID = "";
        /// <summary>
        /// 预约挂号科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Dept = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 预约看诊专家
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Doct = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 出诊日期
        /// </summary>
        public DateTime RegDate = DateTime.MinValue;
        /// <summary>
        /// 午别
        /// </summary>
        public string TimeFlag = "";
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime = "";
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime = "";

        /// <summary>
        /// 挂号费(单位“分”)
        /// </summary>
        public int RegFee = 0;

        /// <summary>
        /// 诊疗费(单位“分”)
        /// </summary>
        public int TreatFee = 0;

        /// <summary>
        /// 订单状态 0-下定成功 1-下定失败 2-已取消 3-已支付 4-已取号 5-已退费
        /// </summary>
        public string OrderState = "";

        /// <summary>
        /// 就诊流水号
        /// </summary>
        public string VisitingNum = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Note = "";

        /// <summary>
        /// 预约流水号
        /// </summary>
        public string SerialNO = "";

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo = "";

        /// <summary>
        /// 支付方式 1-银联 2-手机支付 3-羊城通
        /// </summary>
        public string PayMode = "";

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate = DateTime.MinValue;
        public AppointmentOrder Clone()
        {
            return base.MemberwiseClone() as Neusoft.HISFC.Models.Registration.AppointmentOrder;
        }
    }
}
