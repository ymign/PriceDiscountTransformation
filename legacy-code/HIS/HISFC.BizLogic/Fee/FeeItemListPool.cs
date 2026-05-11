using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Fee.Inpatient;

namespace Neusoft.HISFC.BizLogic.Fee
{
    public class FeeItemListPool
    {
        private readonly Stack<FeeItemList> _pool = new Stack<FeeItemList>();

        public FeeItemList GetObject()
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop();
            }
            else
            {
                return new FeeItemList();
            }
        }

        public void ReleaseObject(FeeItemList obj)
        {
            _pool.Push(obj);
        }
    }
}
