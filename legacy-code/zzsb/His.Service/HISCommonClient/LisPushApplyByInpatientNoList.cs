using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HISCommonClient
{
    public class LisPushApplyByInpatientNoList
    {
        WebReference1.CommonServiceForHIS commonServiceForHIS = new HISCommonClient.WebReference1.CommonServiceForHIS();

        public int PushLisApplyByInpatientNoList(List<string> inpatientList)
        {
            int i = -1;
            string[] inpatientStrList = new string[inpatientList.Count];
            inpatientStrList=inpatientList.ToArray();
            i = commonServiceForHIS.PushLisInpatientApplyByInpatientNoList(inpatientStrList);
            return i;
        }
    }
}
