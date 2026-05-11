using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    public class PatientAndApplyInfo
    {
        /// <summary>
        /// 申请流水号
        /// </summary>
        public string ApplyNumber { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 门诊号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string PatientNo { get; set; }

        /// <summary>
        /// 处方号
        /// </summary>
        public string RecipeNo { get; set; }

        /// <summary>
        /// 发药部门名称
        /// </summary>
        public string DrugDeptCode { get; set; }

        /// <summary>
        /// 发药部门名称
        /// </summary>
        public string DrugDeptName { get; set; }

        /// <summary>
        /// 申请部门编码
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 申请部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 开单科室编码
        /// </summary>
        public string RecipeDeptCode { get; set; }

        /// <summary>
        /// 开单科室名称
        /// </summary>
        public string RecipeDeptName { get; set; }

        /// <summary>
        /// 开单人员编码
        /// </summary>
        public string RecipeOperCode { get; set; }

        /// <summary>
        /// 开单人员名称
        /// </summary>
        public string RecipeOperName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        

    }
}
