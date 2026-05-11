using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.HealthRecord.Visit.KfModel
{
    public class SpecialTag
    {
        private string xml = string.Empty;

        private string type = string.Empty;

        private string recordId = string.Empty;

        public string Xml
        {
            get { return xml; }
            set { xml = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string RecordId
        {
            get { return recordId; }
            set { recordId = value; }
        }

    }
}
