using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    public class COM_DEPARTMENT
    {
        /// <summary>
        /// 科室表
        /// </summary>
        private System.String _DEPT_CODE;
        /// <summary>
        /// 科室编码
        /// </summary>
        public System.String DEPT_CODE { get { return this._DEPT_CODE; } set { this._DEPT_CODE = value; } }

        private System.String _DEPT_NAME;
        /// <summary>
        /// 科室名称
        /// </summary>
        public System.String DEPT_NAME { get { return this._DEPT_NAME; } set { this._DEPT_NAME = value; } }

        private System.String _SPELL_CODE;
        /// <summary>
        /// 拼音
        /// </summary>
        public System.String SPELL_CODE { get { return this._SPELL_CODE; } set { this._SPELL_CODE = value; } }

        private System.String _WB_CODE;
        /// <summary>
        /// 五笔
        /// </summary>
        public System.String WB_CODE { get { return this._WB_CODE; } set { this._WB_CODE = value; } }

        private System.String _DEPT_ENAME;
        /// <summary>
        /// 科室英文
        /// </summary>
        public System.String DEPT_ENAME { get { return this._DEPT_ENAME; } set { this._DEPT_ENAME = value; } }

        private System.String _DEPT_TYPE;
        /// <summary>
        /// C 门诊  I  住院  F  财务 L  后勤(logistics)  PI  药库 T 医技(terminal)  O  其它 D  机关(department)P  药房   N 护士站
        /// </summary>
        public System.String DEPT_TYPE { get { return this._DEPT_TYPE; } set { this._DEPT_TYPE = value; } }

        private System.Decimal? _MEDI_TIME;
        /// <summary>
        /// 发药时间
        /// </summary>
        public System.Decimal? MEDI_TIME { get { return this._MEDI_TIME; } set { this._MEDI_TIME = value; } }

        private System.Decimal? _CYCLE_BEGIN;
        /// <summary>
        /// 周期开始
        /// </summary>
        public System.Decimal? CYCLE_BEGIN { get { return this._CYCLE_BEGIN; } set { this._CYCLE_BEGIN = value; } }

        private System.Decimal? _CYCLE_END;
        /// <summary>
        /// 周期结束
        /// </summary>
        public System.Decimal? CYCLE_END { get { return this._CYCLE_END; } set { this._CYCLE_END = value; } }

        private System.String _REGDEPT_FLAG;
        /// <summary>
        /// 是否挂号科室 0 假 1 真
        /// </summary>
        public System.String REGDEPT_FLAG { get { return this._REGDEPT_FLAG; } set { this._REGDEPT_FLAG = value; } }

        private System.String _TATDEPT_FLAG;
        /// <summary>
        /// 是否核算科室 0 假 1 真
        /// </summary>
        public System.String TATDEPT_FLAG { get { return this._TATDEPT_FLAG; } set { this._TATDEPT_FLAG = value; } }

        private System.String _DEPT_PRO;
        /// <summary>
        /// 特殊科室属性 0 普通, 1 手术,  2 麻醉, 3 ICU,  4 CCU, C 产科(中山一需求),E急诊留观,T特诊
        /// </summary>
        public System.String DEPT_PRO { get { return this._DEPT_PRO; } set { this._DEPT_PRO = value; } }

        private System.Single? _ALTER_MONEY;
        /// <summary>
        /// 警戒线
        /// </summary>
        public System.Single? ALTER_MONEY { get { return this._ALTER_MONEY; } set { this._ALTER_MONEY = value; } }

        private System.String _EXT_FLAG;
        /// <summary>
        /// 扩展标志 －是否已经集中发送 0 未,1 已
        /// </summary>
        public System.String EXT_FLAG { get { return this._EXT_FLAG; } set { this._EXT_FLAG = value; } }

        private System.String _EXT1_FLAG;
        /// <summary>
        /// 扩展标志1
        /// </summary>
        public System.String EXT1_FLAG { get { return this._EXT1_FLAG; } set { this._EXT1_FLAG = value; } }

        private System.String _VALID_STATE;
        /// <summary>
        /// 有效性标志 1在用 0 停用 2 废弃
        /// </summary>
        public System.String VALID_STATE { get { return this._VALID_STATE; } set { this._VALID_STATE = value; } }

        private System.Decimal? _SORT_ID;
        /// <summary>
        /// 顺序号
        /// </summary>
        public System.Decimal? SORT_ID { get { return this._SORT_ID; } set { this._SORT_ID = value; } }

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

        private System.String _SIMPLE_NAME;
        /// <summary>
        /// 科室简称
        /// </summary>
        public System.String SIMPLE_NAME { get { return this._SIMPLE_NAME; } set { this._SIMPLE_NAME = value; } }

        private System.String _HOS_CODE;
        /// <summary>
        /// 医院编码
        /// </summary>
        public System.String HOS_CODE { get { return this._HOS_CODE; } set { this._HOS_CODE = value; } }

        private System.Decimal? _SORT_ID1;
        /// <summary>
        /// 自助设备顺序号
        /// </summary>
        public System.Decimal? SORT_ID1 { get { return this._SORT_ID1; } set { this._SORT_ID1 = value; } }

        private System.String _BRO_NAME;
        /// <summary>
        /// 自助设备科室大类
        /// </summary>
        public System.String BRO_NAME { get { return this._BRO_NAME; } set { this._BRO_NAME = value; } }

        private System.Decimal? _BRO_ID;
        /// <summary>
        /// 大类id
        /// </summary>
        public System.Decimal? BRO_ID { get { return this._BRO_ID; } set { this._BRO_ID = value; } }

        private System.String _SEND_FLAG;
        /// <summary>
        /// 推送标识 0-未推送 1-新增 3-删除
        /// </summary>
        public System.String SEND_FLAG { get { return this._SEND_FLAG; } set { this._SEND_FLAG = value; } }
    }
}

