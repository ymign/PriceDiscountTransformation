using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace His.Business.ZZSB
{
    public class Bussiness
    {
        public ArrayList GetFeeItemList(string clinicCode, ref string errMsg)
        {
            ArrayList comFeeItemLists = new ArrayList();
            //获取挂号的未收费项目信息
            ArrayList al = this.outpatientManager.QueryChargedFeeItemListsByClinicNO(clinicCode);
            if (al == null || al.Count == 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                errMsg = "您暂时无缴费信息!";
                return -1;
            }
            //开方医生工号
            string doctid = "";
            foreach (FeeItemList f in al)
            {
                if (f.Item.IsMaterial)
                {
                    continue;
                }
                doctid = f.RecipeOper.ID;
            }
            if (string.IsNullOrEmpty(doctid))
            {
                errMsg = "开方医生为空!";
                return -1;
            }

            //清空费用信息
            comFeeItemLists.Clear();
            comFeeItemLists = this.GetFeeItemList(al, reg, ref errMsg);
            return comFeeItemLists;
        }
        
    }
}
