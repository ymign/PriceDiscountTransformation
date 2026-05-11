using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    public class FIN_OPB_ACCOUNTCARD
    {
        public System.String CARD_NO { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public System.String MARKNO { get; set; }
        public System.String TYPE { get; set; }
        public System.String STATE { get; set; }
        public System.String REFLAG { get; set; }
        public System.String CREATEOPER { get; set; }
        public System.DateTime CREATEDATE { get; set; }
        public System.String STOPOPER { get; set; }
        public System.DateTime STOPDATE { get; set; }
        public System.String BACKOPER { get; set; }
        public System.DateTime BACKDATE { get; set; }
        public System.String SECURITYCODE { get; set; }
    }
}
