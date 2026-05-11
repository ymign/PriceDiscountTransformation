using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    /// <summary>
    /// 预约主表
    /// </summary>
    public class FIN_OPR_BOOKING
    {
        /// <summary>
        /// 预约号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String CLINIC_CODE { get; set; }

        /// <summary>
        /// 就诊卡号
        /// </summary>
        public System.String CARD_NO { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        public System.DateTime BOOKING_DATE { get; set; }

        /// <summary>
        /// 午别
        /// </summary>
        public System.String NOON_CODE { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public System.String NAME { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String IDENNO { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public System.String SEX_CODE { get; set; }

        /// <summary>
        /// 出生日
        /// </summary>
        public System.DateTime BIRTHDAY { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public System.String RELA_PHONE { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public System.String ADDRESS { get; set; }

        /// <summary>
        /// 所用排班的序号
        /// </summary>
        public System.String SCHEMA_NO { get; set; }

        /// <summary>
        /// 科室号
        /// </summary>
        public System.String DEPT_CODE { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        public System.String DEPT_NAME { get; set; }

        /// <summary>
        /// 看诊开始时间
        /// </summary>
        public System.DateTime? BEGIN_TIME { get; set; }

        /// <summary>
        /// 看诊结束时间
        /// </summary>
        public System.DateTime? END_TIME { get; set; }

        /// <summary>
        /// 医师代号
        /// </summary>
        public System.String DOCT_CODE { get; set; }

        /// <summary>
        /// 医师姓名
        /// </summary>
        public System.String DOCT_NAME { get; set; }

        /// <summary>
        /// 1已经看诊/0未看诊
        /// </summary>
        public System.String SEE_FLAG { get; set; }

        /// <summary>
        /// 1加号/0正常
        /// </summary>
        public System.String APP_FLAG { get; set; }

        /// <summary>
        /// 操作员代码
        /// </summary>
        public System.String OPER_CODE { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public System.DateTime? OPER_DATE { get; set; }

        /// <summary>
        /// confirm person
        /// </summary>
        public System.String CONFIRM_OPCD { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.DateTime? CONFIRM_DATE { get; set; }

        /// <summary>
        /// 挂号级别代码
        /// </summary>
        public System.String REGLEVL_CODE { get; set; }

        /// <summary>
        /// 1 有效 0 作废
        /// </summary>
        public System.String VALID_FLAG { get; set; }

        /// <summary>
        /// 挂号流水号
        /// </summary>
        public System.String REG_ID { get; set; }

        /// <summary>
        /// 来源（0移动1本地2大象就医3微信4自助设备9其他）
        /// </summary>
        public System.String SOURCE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String APP_SENDFLAG { get; set; }
    }
}
