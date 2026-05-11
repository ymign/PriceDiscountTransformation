using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FS.ZDWY.Internet.Models
{
    public class DepartmentEntity
    {
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

        private System.String _HASCHILD;
        ///<summary>
        ///是否有子科室
        ///</summary>
        public System.String HSACHILD { get { return this._HASCHILD; } set { this._HASCHILD = value; } }

        private System.String _PARENTDEPTCODE;
        ///<summary>
        ///父科室代码
        ///</summary>
        public System.String PARENTDEPTCODE { get { return this._PARENTDEPTCODE; } set { this._PARENTDEPTCODE = value; } }

        private System.String _DEPTDESCRIPTION;
        ///<summary>
        ///科室介绍
        ///</summary>
        public System.String DEPTDESCRIPTION { get { return this._DEPTDESCRIPTION; } set { this._DEPTDESCRIPTION = value; } }

        private System.String _DEPTLOCATION;
        ///<summary>
        ///科室位置
        ///</summary>
        public System.String DEPTLOCATION { get { return this._DEPTLOCATION; } set { this._DEPTLOCATION = value; } }

        private System.String _RULE;
        ///<summary>
        ///科室预约规则说明
        ///</summary>
        public System.String RULE { get { return this._RULE; } set { this._RULE = value; } }

        private int _STATUS;
        ///<summary>
        ///是否预约科室
        ///</summary>
        public int STATUS { get { return this._STATUS; } set { this._STATUS = value; } }

        private System.String _EXPERTISE;
        ///<summary>
        ///科室主治
        ///</summary>
        public System.String EXPERTISE { get { return this._EXPERTISE; } set { this._EXPERTISE = value; } }
    }
}
