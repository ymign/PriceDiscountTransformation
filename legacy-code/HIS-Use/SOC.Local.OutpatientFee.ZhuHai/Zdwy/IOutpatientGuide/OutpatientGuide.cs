using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;
using Neusoft.FrameWork.WinForms.Forms;
using System.Data;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientGuide
{
    public class OutpatientGuide : Neusoft.HISFC.BizProcess.Interface.Fee.IOutpatientGuide
    {
        ucFeeDetailGuide ucFeeDetail = null;

        ucFeeDetail ucFeePrint = null;

        /// <summary>
        /// 打印控制
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.PageSize pageSizeManager = new Neusoft.HISFC.BizLogic.Manager.PageSize();

        /// <summary>
        /// 控制参数
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        public Neusoft.HISFC.Models.Base.PageSize GetPageSize()
        {
            Neusoft.HISFC.Models.Base.PageSize pSize = pageSizeManager.GetPageSize("MZGuide");
            if (pSize == null)
            {
                pSize = new Neusoft.HISFC.Models.Base.PageSize("MZGuide", 260, 260);
            }
            pSize.Height = ucFeeDetail.GetHeigth();
            return pSize;
        }

        #region IOutpatientGuide 成员

        public void Print()
        {
            if (ucFeeDetail != null)
            {
                Neusoft.HISFC.Models.Base.PageSize pageSize = this.GetPageSize();
                //使用Neusoft默认打印方式
                if (pageSize == null)
                {
                    pageSize = new Neusoft.HISFC.Models.Base.PageSize();//使用默认的A4纸张
                }
                Neusoft.FrameWork.WinForms.Classes.Print print = new Neusoft.FrameWork.WinForms.Classes.Print();
                print.SetPageSize(pageSize);

                string printer = this.controlIntegrate.GetControlParam<string>("MZGuide", true, "");
                if (!string.IsNullOrEmpty(printer))
                {
                    print.PrintDocument.PrinterSettings.PrinterName = printer;
                }

                print.ControlBorder = Neusoft.FrameWork.WinForms.Classes.enuControlBorder.None;
                print.PrintPage(pageSize.Left, pageSize.Top, ucFeeDetail);
            }
        }

        public void SetValue(Neusoft.HISFC.Models.Registration.Register rInfo, System.Collections.ArrayList invoices, System.Collections.ArrayList feeDetails)
        {
            ucFeeDetail = null;
            if (rInfo.PrintInvoiceCnt == 2)//补打时不提示
            {
                //ucFeeDetailGuide = new ucFeeDetailGuide();
                //string errorInfo = string.Empty;
                //ucFeeDetail.SetValue(rInfo, invoices, feeDetails);

                ucFeePrint = new ucFeeDetail();
                ucFeePrint.SetValue(rInfo, invoices, feeDetails);
                ucFeePrint.PrintPage();
            }
            else if (rInfo.PrintInvoiceCnt == 3)//记账单查看
            {
                //收费时，如果是公费患者，则进行公费记账单的提示
                if (rInfo.Pact.PayKind.ID == "03")
                {
                    ucPubCostBill ucPubCostBill = new ucPubCostBill();
                    ucPubCostBill.SetValue(rInfo, invoices, feeDetails);


                    frmPopPubCostBill form = new frmPopPubCostBill(ucPubCostBill);
                    form.WindowState = FormWindowState.Normal;
                    form.Text = "公费记账单";

                    form.ShowDialog();
                }
            }
            else//收费时全部都提示
            {
                if (CommonController.Instance.MessageBox("是否打印费用清单？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //ucFeeDetail = new ucFeeDetailGuide();
                    //string errorInfo = string.Empty;
                    //ucFeeDetail.SetValue(rInfo, invoices, feeDetails);


                    ucFeePrint = new ucFeeDetail();
                    ucFeePrint.SetValue(rInfo, invoices, feeDetails);
                    ucFeePrint.PrintPage();
                }

                //收费时，如果是公费患者，则进行公费记账单的提示
                if (rInfo.Pact.PayKind.ID == "03" && rInfo.SIMainInfo.PubCost + rInfo.SIMainInfo.PayCost > 0)
                {
                    ucPubCostBill ucPubCostBill = new ucPubCostBill();
                    ucPubCostBill.SetValue(rInfo, invoices, feeDetails);

                    frmPopPubCostBill form = new frmPopPubCostBill(ucPubCostBill);
                    form.WindowState = FormWindowState.Normal;
                    form.Text = "公费记账单";

                    form.ShowDialog();
                }
            }
        }

        public void SetValue(DataTable dt) 
        {
            ucGhDetail ucGhDetail = new ucGhDetail();
            ucGhDetail.SetValue(dt);
            ucGhDetail.PrintPage();
        
        }

        #endregion
    }
}
