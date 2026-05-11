using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace His.Models.ZZSB
{
    /// <summary>
    /// 停诊退号接口
    /// </summary>
    [XmlRoot("message")]
    public class ReturnTheStoppedRegRequestModel
    {
       
        /// <summary>
        /// 门诊流水号
        /// </summary>
        [XmlElement("ClincCode")]
        public string ClincCode { get; set; }

        /// <summary>
        /// 预约流水号
        /// </summary>
        [XmlElement("AppointNO")]
        public string AppointNO { get; set; }

       
        /// <summary>
        /// 排班ID
        /// </summary>
        [XmlElement("RegSourceID")]
        public string RegSourceID { get; set; }

        
        /// <summary>
        /// 门诊卡号
        /// </summary>
        [XmlElement("CardNO")]
        public string CardNO { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 挂号科室
        /// </summary>
        [XmlElement("DeptCode")]
        public string DeptCode { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        [XmlElement("ToTCost")]
        public string ToTCost { get; set; }

        /// <summary>
        /// 自费金额
        /// </summary>
        [XmlElement("OwnCost")]
        public string OwnCost { get; set; }

        /// <summary>
        /// 报销金额
        /// </summary>
        [XmlElement("PubCost")]
        public string PubCost { get; set; }

    }
}
