using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Endoscope
{
    public class ApplyChargeInfo
    {

        private string feecode;
        public string ITEM_FEE_CODE
        {
            get
            {
                return feecode;
            }
            set
            {
                feecode = value;
            }


        }

        private string feename;
        public string ITEM_FEE_NAME
        {
            get
            {
                return feename;
            }
            set
            {
                feename = value;
            }
        }

        private string fee_count;
        public string FEE_COUNT
        {
            get
            {
                return fee_count;
            }
            set
            {
                fee_count = value;
            }
        }

        private string price;
        public string ITEM_PRICE
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        private string status;
        public string FEE_STATUS
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
    }

   
}
