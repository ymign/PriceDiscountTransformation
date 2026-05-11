using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IBankTrans
{
    public class BankTrans : Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBankTrans
    {
        #region IBankTrans 成员

        public bool Do()
        {
            return true;
        }

        private List<object> inputListInfo = new List<object>();

        public List<object> InputListInfo
        {
            get
            {
                return inputListInfo;
            }
            set
            {
                inputListInfo = value;
            }
        }

        private List<object> outputListInfo = new List<object>();

        public List<object> OutputListInfo
        {
            get
            {
                return outputListInfo;
            }
            set
            {
                outputListInfo = value;
            }
        }

        #endregion
    }
}
