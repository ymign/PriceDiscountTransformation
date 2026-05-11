using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models.Doctor
{
    public class COM_EMPLOYEE
    {
        /// <summary>
        /// 员工代码表
        /// </summary>
        public COM_EMPLOYEE()
        {
        }

        private System.String _EMPL_CODE;
        /// <summary>
        /// 员工代码
        /// </summary>
        public System.String EMPL_CODE { get { return this._EMPL_CODE; } set { this._EMPL_CODE = value; } }

        private System.String _SALARY_ID;
        /// <summary>
        /// 工资号
        /// </summary>
        public System.String SALARY_ID { get { return this._SALARY_ID; } set { this._SALARY_ID = value; } }

        private System.String _EMPL_NAME;
        /// <summary>
        /// 员工姓名
        /// </summary>
        public System.String EMPL_NAME { get { return this._EMPL_NAME; } set { this._EMPL_NAME = value; } }

        private System.String _SPELL_CODE;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String SPELL_CODE { get { return this._SPELL_CODE; } set { this._SPELL_CODE = value; } }

        private System.String _WB_CODE;
        /// <summary>
        /// 五笔
        /// </summary>
        public System.String WB_CODE { get { return this._WB_CODE; } set { this._WB_CODE = value; } }

        private System.String _SEX_CODE;
        /// <summary>
        /// 性别
        /// </summary>
        public System.String SEX_CODE { get { return this._SEX_CODE; } set { this._SEX_CODE = value; } }

        private System.DateTime? _BIRTHDAY;
        /// <summary>
        /// 出生日期
        /// </summary>
        public System.DateTime? BIRTHDAY { get { return this._BIRTHDAY; } set { this._BIRTHDAY = value; } }

        private System.String _POSI_CODE;
        /// <summary>
        /// 职务代号
        /// </summary>
        public System.String POSI_CODE { get { return this._POSI_CODE; } set { this._POSI_CODE = value; } }

        private System.String _LEVL_CODE;
        /// <summary>
        /// 职级代号
        /// </summary>
        public System.String LEVL_CODE { get { return this._LEVL_CODE; } set { this._LEVL_CODE = value; } }

        private System.String _EDUCATION_CODE;
        /// <summary>
        /// 学历
        /// </summary>
        public System.String EDUCATION_CODE { get { return this._EDUCATION_CODE; } set { this._EDUCATION_CODE = value; } }

        private System.String _IDENNO;
        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String IDENNO { get { return this._IDENNO; } set { this._IDENNO = value; } }

        private System.String _DEPT_CODE;
        /// <summary>
        /// 所属科室号
        /// </summary>
        public System.String DEPT_CODE { get { return this._DEPT_CODE; } set { this._DEPT_CODE = value; } }

        private System.String _NURSE_CELL_CODE;
        /// <summary>
        /// 所属护理站
        /// </summary>
        public System.String NURSE_CELL_CODE { get { return this._NURSE_CELL_CODE; } set { this._NURSE_CELL_CODE = value; } }

        private System.String _EMPL_TYPE;
        /// <summary>
        /// 人员类型
        /// </summary>
        public System.String EMPL_TYPE { get { return this._EMPL_TYPE; } set { this._EMPL_TYPE = value; } }

        private System.String _EXPERT_FLAG;
        /// <summary>
        /// 是否专家
        /// </summary>
        public System.String EXPERT_FLAG { get { return this._EXPERT_FLAG; } set { this._EXPERT_FLAG = value; } }

        private System.String _MODIFY_FLAG;
        /// <summary>
        /// 是否有修改票据权限 1允许 0不允许
        /// </summary>
        public System.String MODIFY_FLAG { get { return this._MODIFY_FLAG; } set { this._MODIFY_FLAG = value; } }

        private System.String _NOREGFEE_FLAG;
        /// <summary>
        /// 不挂号就收费权限 0 不允许 1允许
        /// </summary>
        public System.String NOREGFEE_FLAG { get { return this._NOREGFEE_FLAG; } set { this._NOREGFEE_FLAG = value; } }

        private System.String _VALID_STATE;
        /// <summary>
        /// 有效性标志 1 有效 0 停用 2 废弃
        /// </summary>
        public System.String VALID_STATE { get { return this._VALID_STATE; } set { this._VALID_STATE = value; } }

        private System.Decimal? _SORT_ID;
        /// <summary>
        /// 顺序号
        /// </summary>
        public System.Decimal? SORT_ID { get { return this._SORT_ID; } set { this._SORT_ID = value; } }

        private System.String _EXT_FLAG;
        /// <summary>
        /// 扩展标志
        /// </summary>
        public System.String EXT_FLAG { get { return this._EXT_FLAG; } set { this._EXT_FLAG = value; } }

        private System.String _EXT1_FLAG;
        /// <summary>
        /// 扩展标志1
        /// </summary>
        public System.String EXT1_FLAG { get { return this._EXT1_FLAG; } set { this._EXT1_FLAG = value; } }

        private System.String _OPER_CODE;
        /// <summary>
        /// 操作员
        /// </summary>
        public System.String OPER_CODE { get { return this._OPER_CODE; } set { this._OPER_CODE = value; } }

        private System.DateTime _OPER_DATE;
        /// <summary>
        /// 操作时间
        /// </summary>
        public System.DateTime OPER_DATE { get { return this._OPER_DATE; } set { this._OPER_DATE = value; } }

        private System.String _USER_CODE;
        /// <summary>
        /// 自定义码
        /// </summary>
        public System.String USER_CODE { get { return this._USER_CODE; } set { this._USER_CODE = value; } }

        private System.String _REMARK;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String REMARK { get { return this._REMARK; } set { this._REMARK = value; } }

        private System.String _SEND_FLAG;
        /// <summary>
        /// 推送标识 0-未推送 1-新增 3-删除
        /// </summary>
        public System.String SEND_FLAG { get { return this._SEND_FLAG; } set { this._SEND_FLAG = value; } }
    }
}
