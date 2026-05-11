using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    /// <summary>
    /// 来源于平台的挂号订单信息
    /// </summary>
    public class PLATFORM_REGISTER_ORDER
    {
        /// <summary>
        /// 平台订单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String ORDERID { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public System.DateTime? ORDERTIME { get; set; }

        /// <summary>
        /// 科室代码
        /// </summary>
        public System.String DEPTCODE { get; set; }

        /// <summary>
        /// 医生代码
        /// </summary>
        public System.String DOCTORCODE { get; set; }

        /// <summary>
        /// 号源日期
        /// </summary>
        public System.DateTime? SCHEDULEDATE { get; set; }

        /// <summary>
        /// 班次ID
        /// </summary>
        public System.String SCHEDULEID { get; set; }

        /// <summary>
        /// 分时号源ID
        /// </summary>
        public System.String NUMBERINFOID { get; set; }

        /// <summary>
        /// 分时开始时间
        /// </summary>
        public System.String BEGINTIME { get; set; }

        /// <summary>
        /// 分时结束时间
        /// </summary>
        public System.String ENDTIME { get; set; }

        /// <summary>
        /// 挂号费
        /// </summary>
        public System.String REGFEE { get; set; }

        /// <summary>
        /// 诊疗卡类型
        /// </summary>
        public System.String CARDTYPE { get; set; }

        /// <summary>
        /// 诊疗卡号码
        /// </summary>
        public System.String CARDNO { get; set; }

        /// <summary>
        /// 患者类型
        /// </summary>
        public System.String TYPE { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public System.String NAME { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public System.String SEX { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public System.String AGE { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public System.DateTime BIRTH { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public System.String ADDRESS { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public System.String MOBILE { get; set; }

        /// <summary>
        /// 挂号类型
        /// </summary>
        public System.String REGTYPE { get; set; }

        /// <summary>
        /// 第三方服务商ID
        /// </summary>
        public System.String FRONTPROVIDERID { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public System.String CERTIFCATETYPE { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public System.String CERTIFCATENO { get; set; }

        /// <summary>
        /// 院内用户Id
        /// </summary>
        public System.String PATIENTID { get; set; }

        /// <summary>
        /// 监护人姓名
        /// </summary>
        public System.String GUARDNAME { get; set; }

        /// <summary>
        /// 监护人证件类型
        /// </summary>
        public System.String GUARDIDTYPE { get; set; }

        /// <summary>
        /// 监护人证件号码
        /// </summary>
        public System.String GUARDIDNO { get; set; }

        /// <summary>
        /// 数据来源
        /// </summary>
        public System.String SOURCE { get; set; }

        /// <summary>
        /// 状态,1有效，0无效
        /// </summary>
        public System.String STATUS { get; set; }

        /// <summary>
        /// 对应的门诊流水号
        /// </summary>
        public System.String CLINIC_CODE { get; set; }

        /// <summary>
        /// 支付方式	1在线支付 0到院支付
        /// </summary>
        public string PAYMETHOD { get; set; }

        /// <summary>
        /// 挂号流水号
        /// </summary>
        public System.String REGISTERID { get; set; }

        public System.String OPERCODE { get; set; }

        public System.String OPERNAME { get; set; }

        /// <summary>
        /// 是否进行优惠
        /// </summary>
        public System.String ISECOST { get; set; }

    }
}
