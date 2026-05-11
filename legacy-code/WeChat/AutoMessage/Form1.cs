using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AutoMessage
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }

        LogHelper.ServiceLog serviceLogManager;
        /// <summary>
        /// 服务日志管理
        /// </summary>
        LogHelper.ServiceLog ServiceLogManager
        {
            get
            {
                if (serviceLogManager == null)
                {
                    serviceLogManager = new LogHelper.ServiceLog();
                }
                return serviceLogManager;
            }
        }

        FS.ZDWY.Internet.BP.OutPatient.PatientInfoManager patientInfoMgr = null;

        FS.ZDWY.Internet.BP.OutPatient.PatientInfoManager PatientInfoMgr
        {
            get
            {
                if (patientInfoMgr == null)
                {
                    patientInfoMgr = new FS.ZDWY.Internet.BP.OutPatient.PatientInfoManager();
                }
                return patientInfoMgr;
            }
            set
            {
                patientInfoMgr = value;
            }
        }

        /// <summary>
        /// 缴费提醒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCharge_Click(object sender, EventArgs e)
        {
            #region 入参
            // < Request >
            //< data >
            //< patientId ></ patientId >
            //< medicalNo ></ medicalNo >
            //< cardType ></ cardType >
            //< cardNo ></ cardNo >
            //< certifcateType ></ certifcateType >
            //< certifcateNo ></ certifcateNo >
            //< name ></ name >
            //< birth ></ birth >
            //< sex ></ sex >
            //< departmentName ></ departmentName >
            //< doctorName ></ doctorName >
            //< amount ></ amount >
            //< billId ></ billId >
            //< content ></ content >
            //</ data >
            //</ Request >

            #endregion

            DataTable dsRes = null;
            dsRes = PatientInfoMgr.QueryOutpatientFeeMsgList();
            if (dsRes == null || dsRes.Rows.Count == 0)
            {
                return;
            }

            string errInfo = string.Empty;

            for (int i = 0; i < dsRes.Rows.Count; i++)
            {
                StringBuilder xmlData = new StringBuilder();
                StringBuilder xmlDataInner = new StringBuilder();
                for (int j = 0; j < dsRes.Columns.Count; j++)
                {
                    if (dsRes.Columns[j].ToString() == "CLINIC_CODE"
                       || dsRes.Columns[j].ToString() == "CREATE_TIME"
                        )
                    {
                        continue;
                    }
                    xmlDataInner.AppendFormat("<{0}>{1}</{0}>", dsRes.Columns[j], dsRes.Rows[i][j]);
                }
                xmlData.AppendFormat("<{0}><{1}>{2}</{1}></{0}>", "Request", "data", xmlDataInner.ToString());
                //入参
                ServiceLogManager.Write(xmlData);

                object[] param = new object[] { xmlData.ToString() };

                string returnValue = WebServiceClient.InvokeWebService(string.Empty, string.Empty, param, ref errInfo);
                //出参
                ServiceLogManager.Write(returnValue);

                try
                {
                    if (string.IsNullOrEmpty(returnValue))
                    {
                        throw new Exception("入参不正确");
                    }
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(returnValue); //院内用户id
                    string successFlag = FS.ZDWY.Internet.WebService.Function.GetNoteValue(xmlDoc, "Response/ok");
                    if (successFlag == "true")
                    {
                        string clinicCode = dsRes.Rows[i]["CLINIC_CODE"].ToString();
                        DateTime createTime = FS.ZDWY.Internet.WebService.Function.ToDateTime(dsRes.Rows[i]["CREATE_TIME"].ToString());
                        DateTime execTime = PatientInfoMgr.GetSysTime();
                        int rev = PatientInfoMgr.UpdateOutPatientFeeMsg(clinicCode, createTime, execTime, returnValue);
                        if (rev <= 0)
                        {
                            System.Windows.Forms.MessageBox.Show("更新数据失败");
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 排队推送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWaiting_Click(object sender, EventArgs e)
        {
            #region 入参

            //< Request >
            //< data >
            //< patientId ></ patientId >
            //< medicalNo ></ medicalNo >
            //< cardType ></ cardType >
            //< cardNo ></ cardNo >
            //< certifcateType ></ certifcateType >
            //< certifcateNo ></ certifcateNo >
            //< name ></ name >
            //< birth ></ birth >
            //< sex ></ sex >
            //< departmentName ></ departmentName >
            //< doctorName ></ doctorName >
            //< lineType ></ lineType >
            //< type ></ type >
            //< sequence ></ sequence >
            //< remainNo ></ remainNo >
            //< lineId ></ lineId >
            //< lineTime ></ lineTime >
            //< note ></ note >
            //< content ></ content >
            //< data >
            //</ Request >

            #endregion

            DataTable dsRegRes = null;
            DataTable dsPhaRes = null;
            dsRegRes = PatientInfoMgr.QueryRegWaitingALL();
            dsRegRes = PatientInfoMgr.QueryPhaWaitingALL();
            if ((dsRegRes == null || dsRegRes.Rows.Count == 0)&& (dsPhaRes == null || dsPhaRes.Rows.Count == 0))
            {
                return;
            }

            string errInfo = string.Empty;

            #region 挂号排队

            for (int i = 0; i < dsRegRes.Rows.Count; i++)
            {
                StringBuilder xmlData = new StringBuilder();
                StringBuilder xmlDataInner = new StringBuilder();
                for (int j = 0; j < dsRegRes.Columns.Count; j++)
                {
                    xmlDataInner.AppendFormat("<{0}>{1}</{0}>", dsRegRes.Columns[j], dsRegRes.Rows[i][j]);
                }
                xmlData.AppendFormat("<{0}><{1}>{2}</{1}></{0}>", "Request", "data", xmlDataInner.ToString());
                //入参
                ServiceLogManager.Write(xmlData);

                object[] param = new object[] { xmlData.ToString() };

                string returnValue = WebServiceClient.InvokeWebService(string.Empty, string.Empty, param, ref errInfo);
                //出参
                ServiceLogManager.Write(returnValue);

            }

            #endregion

            #region 取药排队

            for (int i = 0; i < dsPhaRes.Rows.Count; i++)
            {
                StringBuilder xmlData = new StringBuilder();
                StringBuilder xmlDataInner = new StringBuilder();
                for (int j = 0; j < dsPhaRes.Columns.Count; j++)
                {
                    xmlDataInner.AppendFormat("<{0}>{1}</{0}>", dsPhaRes.Columns[j], dsPhaRes.Rows[i][j]);
                }
                xmlData.AppendFormat("<{0}><{1}>{2}</{1}></{0}>", "Request", "data", xmlDataInner.ToString());
                //入参
                ServiceLogManager.Write(xmlData);

                object[] param = new object[] { xmlData.ToString() };

                string returnValue = WebServiceClient.InvokeWebService(string.Empty, string.Empty, param, ref errInfo);
                //出参
                ServiceLogManager.Write(returnValue);
            }

            #endregion
        }

        /// <summary>
        /// 取消排班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopSchema_Click(object sender, EventArgs e)
        {
            #region 入参

            #endregion
        }

        /// <summary>
        /// 挂号接诊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAcceptReg_Click(object sender, EventArgs e)
        {
            #region 入参

            //< Request >
            //< data >
            //< orderId ></ orderId >
            //< hospOrderId ></ hospOrderId >
            //< getTime ></ getTime >
            //< data >
            //</ Request >

            #endregion


            DataTable dsRes = null;
            dsRes = PatientInfoMgr.QueryQueryRegAcceptList();
            if (dsRes == null || dsRes.Rows.Count == 0)
            {
                return;
            }

            string errInfo = string.Empty;

            for (int i = 0; i < dsRes.Rows.Count; i++)
            {
                StringBuilder xmlData = new StringBuilder();
                StringBuilder xmlDataInner = new StringBuilder();
                for (int j = 0; j < dsRes.Columns.Count; j++)
                {
                    if (dsRes.Columns[j].ToString() == "CLINIC_CODE"
                       || dsRes.Columns[j].ToString() == "CREATE_TIME"
                        )
                    {
                        continue;
                    }
                    xmlDataInner.AppendFormat("<{0}>{1}</{0}>", dsRes.Columns[j], dsRes.Rows[i][j]);
                }
                xmlData.AppendFormat("<{0}><{1}>{2}</{1}></{0}>", "Request", "data", xmlDataInner.ToString());
                //入参
                ServiceLogManager.Write(xmlData);

                object[] param = new object[] { xmlData.ToString() };

                string returnValue = WebServiceClient.InvokeWebService(string.Empty, string.Empty, param, ref errInfo);
                //出参
                ServiceLogManager.Write(returnValue);

                try
                {
                    if (string.IsNullOrEmpty(returnValue))
                    {
                        throw new Exception("入参不正确");
                    }
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(returnValue); //院内用户id
                    string successFlag = FS.ZDWY.Internet.WebService.Function.GetNoteValue(xmlDoc, "Response/ok");
                    if (successFlag == "true")
                    {
                        string clinicCode = dsRes.Rows[i]["CLINIC_CODE"].ToString();
                        DateTime createTime = FS.ZDWY.Internet.WebService.Function.ToDateTime(dsRes.Rows[i]["CREATE_TIME"].ToString());
                        DateTime execTime = PatientInfoMgr.GetSysTime();
                        int rev = PatientInfoMgr.UpdateOutPatientFeeMsg(clinicCode, createTime, execTime, returnValue);
                        if (rev <= 0)
                        {
                            System.Windows.Forms.MessageBox.Show("更新数据失败");
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 取消挂号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelReg_Click(object sender, EventArgs e)
        {
            #region 入参

            // <Request>
            //<data>
            //<orderId></orderId>
            //<hospOrderId></hospOrderId >
            //<cancelTime ></cancelTime>
            //<reason></reason>
            //<isStop></isStop>
            //<data>
            //</Request>

            #endregion

            DataTable dsRes = null;
            dsRes = PatientInfoMgr.QueryQueryCancelRegList();
            if (dsRes == null || dsRes.Rows.Count == 0)
            {
                return;
            }

            string errInfo = string.Empty;

            for (int i = 0; i < dsRes.Rows.Count; i++)
            {
                StringBuilder xmlData = new StringBuilder();
                StringBuilder xmlDataInner = new StringBuilder();
                for (int j = 0; j < dsRes.Columns.Count; j++)
                {
                    if (dsRes.Columns[j].ToString() == "CLINIC_CODE"
                       || dsRes.Columns[j].ToString() == "CREATE_TIME"
                        )
                    {
                        continue;
                    }
                    xmlDataInner.AppendFormat("<{0}>{1}</{0}>", dsRes.Columns[j], dsRes.Rows[i][j]);
                }
                xmlData.AppendFormat("<{0}><{1}>{2}</{1}></{0}>", "Request", "data", xmlDataInner.ToString());
                //入参
                ServiceLogManager.Write(xmlData);

                object[] param = new object[] { xmlData.ToString() };

                string returnValue = WebServiceClient.InvokeWebService(string.Empty, string.Empty, param, ref errInfo);
                //出参
                ServiceLogManager.Write(returnValue);

                try
                {
                    if (string.IsNullOrEmpty(returnValue))
                    {
                        throw new Exception("入参不正确");
                    }
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(returnValue); //院内用户id
                    string successFlag = FS.ZDWY.Internet.WebService.Function.GetNoteValue(xmlDoc, "Response/ok");
                    if (successFlag == "true")
                    {
                        string clinicCode = dsRes.Rows[i]["CLINIC_CODE"].ToString();
                        DateTime createTime = FS.ZDWY.Internet.WebService.Function.ToDateTime(dsRes.Rows[i]["CREATE_TIME"].ToString());
                        DateTime execTime = PatientInfoMgr.GetSysTime();
                        int rev = PatientInfoMgr.UpdateOutPatientFeeMsg(clinicCode, createTime, execTime, returnValue);
                        if (rev <= 0)
                        {
                            System.Windows.Forms.MessageBox.Show("更新数据失败");
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }
    }
}
