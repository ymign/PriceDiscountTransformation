using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.KangMei
{
   public class AddressBase
    {
        private string code;
        /// <summary> 
        /// CODE 
        /// 代码 
        /// </summary> 
        public string CODE { get { return code; } set { code = value; } }

        private string name;
        /// <summary> 
        /// NAME 
        /// 名称 
        /// </summary> 
        public string NAME { get { return name; } set { name = value; } }

        private string eng_name;
        /// <summary>
        ///  ENG_NAME 
        /// 英文名称 
        /// </summary>
        public string ENG_NAME { get { return eng_name; } set { eng_name = value; } }

        private string shorts;
        /// <summary> 
        /// SHORT 
        /// 简写 
        /// </summary> 
        public string SHORT { get { return shorts; } set { shorts = value; } }

        private string spell_code;
        /// <summary> 
        ///  SPELL_CODE 
        /// 拼音码
        /// /// </summary> 
        public string SPELL_CODE { get { return spell_code; } set { spell_code = value; } }

        private string wu_code;
        /// <summary>
        ///  WU_CODE 
        /// 五笔码 
        /// </summary> 
        public string WU_CODE { get { return wu_code; } set { wu_code = value; } }

        private string zip;
        /// <summary> 
        /// ZIP
        /// /// 邮编 
        /// </summary> 
        public string ZIP { get { return zip; } set { zip = value; } }

        private string zone;
        /// <summary> 
        /// ZONE
        /// /// 区域编号
        /// /// </summary>
        public string ZONE { get { return zone; } set { zone = value; } }

        private string tel_lenght;
        /// <summary> 
        /// TEL_LENGHT 
        /// 电话长度 
        /// </summary>
        public string TEL_LENGHT { get { return tel_lenght; } set { tel_lenght = value; } }

        private string zone2;
        /// <summary> 
        /// ZONE2
        ///  电话区号 
        /// </summary> 
        public string ZONE2 { get { return zone2; } set { zone2 = value; } }

        private string parentnode;
        /// <summary> 
        /// PARENTNODE
        /// /// 父节点代码 
        /// </summary> 

        public string PARENTNODE { get { return parentnode; } set { parentnode = value; } }

        private string node;
        /// <summary> 
        /// NODE
        ///  节点代码 
        /// </summary> 
        public string NODE { get { return node; } set { node = value; } }

        private string sortcode;
        /// <summary> 
        /// SORTCODE 
        /// 排序号 
        /// </summary>
        public string SORTCODE { get { return sortcode; } set { sortcode = value; } }

        private string isshow;
        /// <summary> 
        /// ISSHOW
        ///  是否显示 
        /// </summary> 
        public string ISSHOW { get { return isshow; } set { isshow = value; } }

        private string isvalid;
        /// <summary>
        /// /// ISVALID
        /// /// 是否有效 
        /// </summary> 
        public string ISVALID { get { return isvalid; } set { isvalid = value; } }

        private string createcode;
        /// <summary>  
        /// CREATECODE 
        /// 创建人
        /// /// </summary> 
        public string CREATECODE { get { return createcode; } set { createcode = value; } }

        private DateTime createdate;
        /// <summary>
        /// /// CREATEDATE 
        /// 创建时间 
        /// </summary> 
        public DateTime CREATEDATE { get { return createdate; } set { createdate = value; } }

        private string oper_code;
        /// <summary> 
        /// OPER_CODE
        ///  更新人 
        /// </summary> 
        public string OPER_CODE { get { return oper_code; } set { oper_code = value; } }

        private DateTime oper_date;
        /// <summary> 
        /// OPER_DATE 
        /// 更新时间 
        /// </summary> 
        public DateTime OPER_DATE { get { return oper_date; } set { oper_date = value; } }

        private string ishot;
        /// <summary> 
        /// ISHOT 
        /// 是否热门 
        /// </summary> 
        public string ISHOT { get { return ishot; } set { ishot = value; } }

    }
}
