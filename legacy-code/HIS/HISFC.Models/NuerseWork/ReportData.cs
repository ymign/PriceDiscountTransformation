using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.NuerseWork
{
    public class ReportData : Neusoft.FrameWork.Models.NeuObject
    {
        private string dept = "";
        /// <summary>
        /// 科室
        /// </summary>
        public string Dept
        {
            get { return dept; }
            set { dept = value; }
        }
        private string datatypename = "";
        /// <summary>
        /// 科室报表名称
        /// </summary>
        public string Datatypename
        {
            get { return datatypename; }
            set { datatypename = value; }
        }
        private string item_name = "";
        /// <summary>
        /// 科室报表名称列
        /// </summary>
        public string Item_name
        {
            get { return item_name; }
            set { item_name = value; }
        }
        private int sort_id = 0;
        /// <summary>
        /// 科室报表行号
        /// </summary>
        public int Sort_id
        {
            get { return sort_id; }
            set { sort_id = value; }
        }
        private string data_data = "";
        /// <summary>
        /// 填写内容
        /// </summary>
        public string Data_data
        {
            get { return data_data; }
            set { data_data = value; }
        }

        private DateTime data_date = System.DateTime.Now;
        /// <summary>
        /// 填写日期
        /// </summary>
        public DateTime Data_date
        {
            get { return data_date; }
            set { data_date = value; }
        }
        private string oper_code = "";
        /// <summary>
        /// 操作员
        /// </summary>
        public string Oper_code
        {
            get { return oper_code; }
            set { oper_code = value; }
        }
        private DateTime oper_date = System.DateTime.Now;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Oper_date
        {
            get { return oper_date; }
            set { oper_date = value; }
        }
        private string valid = "1";
        /// <summary>
        /// 有效性
        /// </summary>
        public string Valid
        {
            get { return valid; }
            set { valid = value; }
        }
        private string check = "0";
        /// <summary>
        /// 审核状态
        /// </summary>
        public string Check
        {
            get { return check; }
            set { check = value; }
        }
        private DateTime check_date = System.DateTime.Now;
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime Check_date
        {
            get { return check_date; }
            set { check_date = value; }
        }
        private string check_opercode = "";
        /// <summary>
        /// 审核员
        /// </summary>

        public string Check_opercode
        {
            get { return check_opercode; }
            set { check_opercode = value; }
        }
    }
}
