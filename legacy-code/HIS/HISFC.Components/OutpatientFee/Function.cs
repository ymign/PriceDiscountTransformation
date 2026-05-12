using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Neusoft.HISFC.Components.OutpatientFee
{
    public class Function
    {
        /// <summary>
        /// 返回执行科室
        /// </summary>
        /// <param name="recipeDept"></param>
        /// <param name="item"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public static ArrayList GetExecDept(Neusoft.FrameWork.Models.NeuObject recipeDept, Neusoft.HISFC.Models.Fee.Item.Undrug item, ref string errorInfo)
        {
            Neusoft.HISFC.BizProcess.Interface.Fee.IExecDept IExecDept=InterfaceManager.GetIExecDept();

            if (IExecDept != null)
            {
                return IExecDept.GetExecDept(recipeDept, item,ref errorInfo);
            }

            return null;
        }

        /// <summary>
        /// 门诊收费二级权限
        /// </summary>
        public const string PrivQuit = "0820";
        /// <summary>
        /// 门诊退其他操作员的三级权限
        /// </summary>
        public const string PrivQuitOtherOperFee = "24";
        /// <summary>
        /// 门诊隔日退费权限
        /// </summary>
        public const string PrivQuitLastDayFee = "25";
        /// <summary>
        /// 门诊未看诊是否能继续收费的三级权限
        /// </summary>
        public const string PrivFeeWhenNoSeeDoc = "26";

        private static Dictionary<string, string> dictionaryYKDept = new Dictionary<string, string>();

        /// <summary>
        ///  判断是否是宜康科室
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public static bool IsContainYKDept(string dept)
        {
            if (dictionaryYKDept == null || dictionaryYKDept.Count == 0)
            {
                ArrayList al = Neusoft.SOC.HISFC.BizProcess.CommonInterface.CommonController.Instance.QueryConstant("YkDept");
                if (al != null)
                {
                    foreach (Neusoft.FrameWork.Models.NeuObject obj in al)
                    {
                        dictionaryYKDept[obj.ID] = obj.Name;
                    }
                }
            }

            return dictionaryYKDept.ContainsKey(dept);
        }


        /// <summary>
        /// 读卡接口
        /// </summary>
        /// <param name="cardNO"></param>
        /// <param name="errInfo"></param>
        /// <returns></returns>
        public static int OperCard(ref string cardNO, ref string errInfo)
        {
            if (InterfaceManager.GetIOperCard() == null)
            {
                errInfo = "没有维护读卡接口！";
                return -1;
            }

            int result = InterfaceManager.GetIOperCard().ReadMCardNO(ref cardNO, ref  errInfo);
            if (result == -1)
            {
                errInfo = "读卡失败，请确认是否正确放置诊疗卡！";
                return -1;
            }

            return 1;
        }


        public static string GetCardNoByIdNo(string idno)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string sql = string.Format(@"SELECT card_no FROM (SELECT * FROM fin_opr_register f WHERE f.idenno = '{0}' AND f.card_no NOT LIKE '99%'  ORDER BY f.reg_date DESC) WHERE  ROWNUM =1", idno);
            string result = outpatientManager.ExecSqlReturnOne(sql);
            if (result == "-1")
            {
                return "";
            }
            return result;
        }

        public static string GETHKElderlyroll(string clinicno)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string sql = string.Format(@"SELECT fun_get_dictionary_name('HKELDERLYROLL', m.hkelderlyroll)
  FROM met_cas_diagnose m
 WHERE m.inpatient_no = '{0}'
   AND m.happen_no = (SELECT MAX(happen_no)
                        FROM met_cas_diagnose
                       WHERE inpatient_no = m.inpatient_no)
   AND m.hkelderlyroll IS NOT NULL
   AND m.hkelderlyroll <> '0'", clinicno);
            string result = outpatientManager.ExecSqlReturnOne(sql);
            if (result == "-1")
            {
                return "";
            }
            return result;
        }

        public static bool CheckAtmFee(string invoiceno)
        {
            string sql = @"
                            select * from (
                            select * from fin_opb_invoiceinfo  i 
                            start with  i.invoice_no='{0}' and i.trans_type='1'
                            connect  by prior cancel_invoice=invoice_no
                            )
                            where invoice_no like 'T%' and trans_type='1'

                   ";
            sql = string.Format(sql, invoiceno);

            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            System.Data.DataSet ds=new System.Data.DataSet();
            outpatientManager.ExecQuery(sql, ref ds);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


 
        }
        //查询处方类型
        public static string GetRecipeTypeid(string CombNo)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string sql = string.Format(@"SELECT m.recipe_flag FROM met_ord_recipedetail m WHERE m.comb_no = '{0}' AND ROWNUM = 1", CombNo);
            string RecipeType = outpatientManager.ExecSqlReturnOne(sql);
            return RecipeType;

        }
        //查询提示开关
        public static string GetGastroscopyType()
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string sql = string.Format(@"select code from com_dictionary p where  type='GastroscopyType'");
            return outpatientManager.ExecSqlReturnOne(sql);
        }
        //记录多发伤日志
        public static int insertMultipleInjuryLog(string clinicno, string InvoiceNO ,string oper_code )
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string sql = string.Format(@"  insert into com_MultipleInjuryLOG
                                      (CLINIC_CODE, INVOICE_NO, OPER_CODE, OPER_DATE)
                                      VALUES
                                      ('{0}', '{1}','{2}', sysdate) ", clinicno, InvoiceNO, oper_code);
            return outpatientManager.ExecQuery(sql);
        }
        //查询是否之后有开诊查费
        public static string GetGastroscopy(string clinicno)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string sql = string.Format(@"WITH gastroscopy_codes AS
                                         (SELECT code FROM com_dictionary WHERE type = 'Gastroscopy')
                                          SELECT 1
                                          FROM fin_opb_feedetail FEE
                                          WHERE FEE.clinic_code = '{0}'
                                          AND FEE.CANCEL_FLAG <> '0'
                                          AND FEE.item_CODE IN (SELECT item_CODE
                                          FROM fin_com_undruginfo p
                                          WHERE p.item_name LIKE '%诊查费%'
                                          AND p.VALID_STATE = '1')
                                          AND EXISTS
                                         (SELECT 1
                                          FROM fin_opb_feedetail FEEA
                                          WHERE FEEA.Clinic_Code = FEE.Clinic_Code
                                          AND (FEEA.package_code IN (SELECT code FROM gastroscopy_codes) OR
                                          FEEA.item_code IN (SELECT code FROM gastroscopy_codes))
                                          AND (FEE.oper_date >= FEEA.oper_date OR
                                          FEE.fee_date >= FEEA.oper_date))", clinicno);
            string Gastrtype = outpatientManager.ExecSqlReturnOne(sql);
            if (Gastrtype == "-1")
            {
                return "0";
            }
            return Gastrtype;
        }
        public readonly static string baseUrl = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 记录程序日志
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="logMsg">日志消息体</param>
        /// <returns></returns>
        public static bool WriteLog(string type, string logMsg)
        {
            bool bo = true;
            string date = DateTime.Now.ToString("yyyyMMdd");
            string fileName = date + "_" + type + "_log.txt";
            string filePath = baseUrl + "Log\\" + fileName;
            string path = baseUrl + "Log\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            StreamWriter sw = null;
            try
            {
                if (!File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        using (sw = new StreamWriter(fs))
                        {
                            sw.WriteLine("======================================================= "
                                + date + " ==============================================================");
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
                if (!string.IsNullOrEmpty(logMsg))
                {
                    using (sw = new StreamWriter(filePath, true))
                    {
                        sw.WriteLine("写入时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.WriteLine(logMsg);
                        sw.WriteLine(@"==============================================================
                            =================================================================");
                        sw.WriteLine("\r");
                    }
                    if (sw != null)
                        sw.Close();
                }
            }
            catch (Exception ex)
            {
                bo = false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
            return bo;
        }
    }
}
