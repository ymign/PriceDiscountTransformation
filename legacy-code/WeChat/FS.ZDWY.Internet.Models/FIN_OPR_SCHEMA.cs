using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    /// <summary>
    /// 医师出诊表
    /// </summary>
    public class FIN_OPR_SCHEMA
    {
        /// <summary>
        /// 医师出诊表
        /// </summary>
        public FIN_OPR_SCHEMA()
        {
        }

        /// <summary>
        /// 序号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String ID { get; set; }

        /// <summary>
        /// 排班类型，0科室/1医生
        /// </summary>
        public System.String SCHEMA_TYPE { get; set; }

        /// <summary>
        /// 看诊日期
        /// </summary>
        public System.DateTime SEE_DATE { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        public System.String WEEK { get; set; }

        /// <summary>
        /// 午别
        /// </summary>
        public System.String NOON_CODE { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime BEGIN_TIME { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime END_TIME { get; set; }

        /// <summary>
        /// 科室代号
        /// </summary>
        public System.String DEPT_CODE { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        public System.String DEPT_NAME { get; set; }

        /// <summary>
        /// 医师代号,当为科室排班时,值为None
        /// </summary>
        public System.String DOCT_CODE { get; set; }

        /// <summary>
        /// 医生姓名
        /// </summary>
        public System.String DOCT_NAME { get; set; }

        /// <summary>
        /// 1在职/2返聘
        /// </summary>
        public System.String DOCT_TYPE { get; set; }

        /// <summary>
        /// 来人挂号限额
        /// </summary>
        public System.Int32? REG_LMT { get; set; }

        /// <summary>
        /// 挂号已挂
        /// </summary>
        public System.Int32? REGED { get; set; }

        /// <summary>
        /// 来电挂号限额
        /// </summary>
        public System.Int32? TEL_LMT { get; set; }

        /// <summary>
        /// 来电已挂
        /// </summary>
        public System.Int32? TEL_REGED { get; set; }

        /// <summary>
        /// 来电已预约
        /// </summary>
        public System.Int32? TEL_REGING { get; set; }

        /// <summary>
        /// 特诊挂号限额
        /// </summary>
        public System.Int32? SPE_LMT { get; set; }

        /// <summary>
        /// 特诊已挂
        /// </summary>
        public System.Int32? SPE_REGED { get; set; }

        /// <summary>
        /// 1正常/0停诊
        /// </summary>
        public System.String VALID_FLAG { get; set; }

        /// <summary>
        /// 1加号/0否
        /// </summary>
        public System.String APPEND_FLAG { get; set; }

        /// <summary>
        /// 停诊原因
        /// </summary>
        public System.String REASON_NO { get; set; }

        /// <summary>
        /// 停诊原因名称
        /// </summary>
        public System.String REASON_NAME { get; set; }

        /// <summary>
        /// 停止人
        /// </summary>
        public System.String STOP_OPCD { get; set; }

        /// <summary>
        /// 停止时间
        /// </summary>
        public System.DateTime? STOP_DATE { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public System.String REMARK { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public System.String OPER_CODE { get; set; }

        /// <summary>
        /// 最近改动日期
        /// </summary>
        public System.DateTime? OPER_DATE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32? ORDER_NO { get; set; }

        /// <summary>
        /// 挂号级别代码
        /// </summary>
        public System.String REGLEVL_CODE { get; set; }

        /// <summary>
        /// 挂号级别name
        /// </summary>
        public System.String REGLEVL_NAME { get; set; }

        /// <summary>
        /// 诊室代码
        /// </summary>
        public System.String ROOM_ID { get; set; }

        /// <summary>
        /// 诊室名称
        /// </summary>
        public System.String ROOM_NAME { get; set; }

        /// <summary>
        /// 诊台代码
        /// </summary>
        public System.String CONSOLE_CODE { get; set; }

        /// <summary>
        /// 诊台名称
        /// </summary>
        public System.String CONSOLE_NAME { get; set; }

        /// <summary>
        /// 是否停诊
        /// </summary>
        public System.String STOP { get; set; }

        /// <summary>
        /// 推送标识 0-为推送 1-新增 3-删除 10-同步
        /// </summary>
        public System.String SEND_FLAG { get; set; }

        public string SCHEMA_DEPT_CODE { get; set; }
    }
}
