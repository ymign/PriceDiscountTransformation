using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views.OutPatient
{
    public class SelfServiceAddOrderRequsetModel
    {
        public string patientName { get; set; }
        public string patientCardNo { get; set; }
        public string sourceFlag { get; set; }
        public List<item> itemList { get; set; }
    }

    public class SelfServiceAddOrderResponseModel 
    {
        public string clinicCode { get; set; }
    }


    public class item
    {
        public string doctCode { get; set; }
        public string doctName { get; set; }
        public string deptCode { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public decimal unitPrice { get; set; }
        public decimal qty { get; set; }
        public decimal ownCost { get; set; }
        public string execDeptCode { get; set; }
        public string execDeptName { get; set; }
    }
}
