using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Xml;

namespace AutoMessage
{
    public partial class frmMessageTimeJob : Form
    {
        public frmMessageTimeJob()
        {
            busy = true;
            InitializeComponent();
            this.InitUrl();
            this.InitEvent();
            busy = false;
        }

        BackgroundWorker feeMessage = new BackgroundWorker();

        BackgroundWorker waitMessage = new BackgroundWorker();

        BackgroundWorker schemaMessage = new BackgroundWorker();

        BackgroundWorker acceptReg = new BackgroundWorker();

        BackgroundWorker cancelReg = new BackgroundWorker();

        private bool busy = true;

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
        /// 初始化事件
        /// </summary>
        private void InitEvent()
        {
            this.btnFee.Click += BtnFee_Click;
            this.btnMinSize.Click += BtnMinSize_Click;
            this.btnQuit.Click += BtnQuit_Click;
            this.btnRegAccept.Click += BtnRegAccept_Click;
            this.btnRegCancel.Click += BtnRegCancel_Click;
            this.btnSchema.Click += BtnSchema_Click;
            this.btnWaiting.Click += BtnWaiting_Click;
            this.btnBegin.Click += BtnBegin_Click;
            feeMessage.DoWork += FeeMessage_DoWork;
            //waitMessage.DoWork += WaitMessage_DoWork;
            //schemaMessage.DoWork += SchemaMessage_DoWork;
            //acceptReg.DoWork += AcceptReg_DoWork;
            //cancelReg.DoWork += CancelReg_DoWork;
            this.timer1.Interval = Convert.ToInt32(this.txtFeeInterval.Text) * 1000;
            this.timer2.Interval = Convert.ToInt32(this.txtWaitingInteval.Text) * 1000;
            this.timer3.Interval = Convert.ToInt32(this.txtSchemaInterval.Text) * 1000;
            this.timer4.Interval = Convert.ToInt32(this.txtRegInteval.Text) * 1000;
            this.timer4.Interval = Convert.ToInt32(this.txtRegCencelInteval.Text) * 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            this.timer4.Tick += new System.EventHandler(this.timer4_Tick);
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
        }


        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBegin_Click(object sender, EventArgs e)
        {
            if (this.btnBegin.Text == "结束运行")
            {
                this.btnBegin.Text = "结束运行";
                this.feeMessage.RunWorkerAsync();
            }
            else
            {
                this.btnBegin.Text = "结束运行";
            }
        }

        /// <summary>
        /// 缴费通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeeMessage_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.btnBegin.Text == "结束运行")
            {
                this.FeeMessage();
            }
        }

        /// <summary>
        /// 缴费通知
        /// </summary>
        private void FeeMessage()
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
                string xmlcontent = @"<?xml version='1.0' encoding='utf-8'?>
<soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
   <soap:Header>
      <AuthorizationSoapHeader xmlns='http://datareceive.service.esb.medata.com/'>
         <MDIP_ACCESSTOKEN>7EAC38D1C96B236EAD9A7A148FE1C513</MDIP_ACCESSTOKEN>
      </AuthorizationSoapHeader>
   </soap:Header>
   <soap:Body>
      <ns2:dataReceive xmlns:ns2='http://datareceive.service.esb.medata.com/'>
         <param><![CDATA[
	    {0}
         ]]></param>
      </ns2:dataReceive>
   </soap:Body>
</soap:Envelope>";

                xmlcontent = string.Format(xmlcontent, xmlData);
                string errStr = string.Empty;
                string returnStr = string.Empty;
                int returnValue = WebServiceClient.CallService(this.txtUrlFee.Text, xmlcontent, ref returnStr, ref errStr);
                //出参

                try
                {
                    if (returnValue <= 0)
                    {
                        throw new Exception("入参不正确");
                    }
                    //XmlDocument xmlDoc = new XmlDocument();
                    //xmlDoc.LoadXml(returnStr); //院内用户id
                    //string successFlag = FS.ZDWY.Internet.WebService.Function.GetNoteValue(xmlDoc, "Response/ok");
                    if (returnStr == "true")
                    {
                        string clinicCode = dsRes.Rows[0]["outpatId"].ToString();
                        DateTime createTime = FS.ZDWY.Internet.WebService.Function.ToDateTime(dsRes.Rows[0]["CREATE_TIME"].ToString());
                        DateTime execTime = PatientInfoMgr.GetSysTime();
                        int rev = PatientInfoMgr.UpdateOutPatientFeeMsg(clinicCode, createTime, execTime, returnStr);
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

        private void WaitMessage_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.btnBegin.Text == "结束运行")
            {
                this.WaitMessage();
            }
        }

        /// <summary>
        /// 排队通知
        /// </summary>
        private void WaitMessage()
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
            if ((dsRegRes == null || dsRegRes.Rows.Count == 0) && (dsPhaRes == null || dsPhaRes.Rows.Count == 0))
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

        private void AcceptReg_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.btnBegin.Text == "结束运行")
            {
                this.AcceptRegMessage();
            }
        }

        /// <summary>
        /// 挂号接诊
        /// </summary>
        private void AcceptRegMessage()
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

        private void CancelReg_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.btnBegin.Text == "结束运行")
            {
                this.CancelRegMessage();
            }
        }

        /// <summary>
        /// 取消挂号
        /// </summary>
        private void CancelRegMessage()
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

        /// <summary>
        /// 取消排班
        /// </summary>
        private void SchemaMessage()
        {
            #region 入参

            #endregion
        }

        private void SchemaMessage_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.btnBegin.Text == "结束运行")
            {
                this.SchemaMessage();
            }
        }


        /// <summary>
        /// 加载URL信息
        /// </summary>
        private void InitUrl()
        {
            string appConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + @"App.config";
            if (System.IO.File.Exists(appConfigFile))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(appConfigFile);
                    string innerText = string.Empty;
                    System.Xml.XmlNode node;
                    //缴费提示url
                    node = GetSection(doc, "AutoMessageSetting/FeeUrl");
                    this.txtUrlFee.Text = node.InnerText;
                    //排队提示url
                    node = GetSection(doc, "AutoMessageSetting/WaitUrl");
                    this.txtUrlWaiting.Text = node.InnerText;
                    //排班提示url
                    node = GetSection(doc, "AutoMessageSetting/SchemaUrl");
                    this.txtUrlSchema.Text = node.InnerText;
                    //取消挂号url
                    node = GetSection(doc, "AutoMessageSetting/RegCancelUrl");
                    this.txtRegCancel.Text = node.InnerText;
                    //接诊url
                    node = GetSection(doc, "AutoMessageSetting/RegAcceptUrl"); 
                    this.txtRegAccept.Text = node.InnerText;


                    node = GetSection(doc, "AutoMessageSetting/FeeInterval");
                    this.txtFeeInterval.Text = node.InnerText;
                    node = GetSection(doc, "AutoMessageSetting/WaitInterval");
                    this.txtWaitingInteval.Text = node.InnerText;
                    node = GetSection(doc, "AutoMessageSetting/SchemaInterval");
                    this.txtSchemaInterval.Text = node.InnerText;
                    node = GetSection(doc, "AutoMessageSetting/RegCancelInterval");
                    this.txtRegCencelInteval.Text = node.InnerText;
                    node = GetSection(doc, "AutoMessageSetting/RegAcceptInterval");
                    this.txtRegInteval.Text = node.InnerText;
                }
                catch (Exception e)
                {
                    throw new Exception("加载App.config配置文件失败，原因：" + e.Message);
                }
            }
            else
            {
                throw new Exception("缺少配置文件App.config");
            }
        }

        /// <summary>
        /// 设置XML
        /// </summary>
        /// <param name="v"></param>
        private void SetXml(string type)
        {
            int param =  this.CheckValid();
            if (param <= 0)
            {
                return;
            }
            string appConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + @"App.config";
            if (System.IO.File.Exists(appConfigFile))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(appConfigFile);
                    string innerText = string.Empty;
                    System.Xml.XmlNode node;
                    switch (type)
                    {
                        case "Fee":
                            node = GetSection(doc, "AutoMessageSetting/FeeInterval");
                            node.InnerText = this.txtFeeInterval.Text;
                            break;
                        case "Wait":
                            node = GetSection(doc, "AutoMessageSetting/WaitInterval");
                            node.InnerText = this.txtWaitingInteval.Text;
                            break;
                        case "Schema":
                            node = GetSection(doc, "AutoMessageSetting/SchemaInterval");
                            node.InnerText = this.txtSchemaInterval.Text;
                            break;
                        case "RegCancel":
                            node = GetSection(doc, "AutoMessageSetting/RegCancelInterval");
                            node.InnerText = this.txtFeeInterval.Text;
                            break;
                        case "RegAccept":
                            node = GetSection(doc, "AutoMessageSetting/RegAcceptInterval");
                            node.InnerText = this.txtFeeInterval.Text;
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("加载App.config配置文件失败，原因：" + e.Message);
                }
            }
            else
            {
                throw new Exception("缺少配置文件App.config");
            }
        }

        /// <summary>
        /// 设置数据格式
        /// </summary>
        private int CheckValid()
        {
            if (!IsNumeric(this.txtFeeInterval.Text))
            {
                System.Windows.Forms.MessageBox.Show("费用刷新间隔为非数字");
                return -1;
            }
            if (!IsNumeric(this.txtWaitingInteval.Text))
            {
                System.Windows.Forms.MessageBox.Show("排队间隔为非数字");
                return -1;
            }
            if (!IsNumeric(this.txtSchemaInterval.Text))
            {
                System.Windows.Forms.MessageBox.Show("停诊通知间隔为非数字");
                return -1;
            }
            if (!IsNumeric(this.txtRegInteval.Text))
            {
                System.Windows.Forms.MessageBox.Show("挂号接诊通知间隔为非数字");
                return -1;
            }
            if (!IsNumeric(this.txtRegCencelInteval.Text))
            {
                System.Windows.Forms.MessageBox.Show("挂号取消通知间隔为非数字");
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 判断一个字符串是否为数字型
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>是数字 true 不是数字 false</returns>
        public static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");

            return reg.IsMatch(str);
        }

        public static System.Xml.XmlNode GetSection(System.Xml.XmlDocument doc,string section)
        {
            if (doc == null)
            {
                return null;
            }
            System.Xml.XmlNode getNode = doc.SelectSingleNode(string.Format("/configuration/{0}", section));
            if (getNode == null)
            {
                throw new Exception(string.Format("节点/configuration/{0}不存在", section));
            }
            return getNode;
        }

        private void BtnFee_Click(object sender, EventArgs e)
        {
            #region 设置费用刷新间隔

            this.SetXml("Fee");

            #endregion
        }

        private void BtnWaiting_Click(object sender, EventArgs e)
        {
            #region 设置排队刷新间隔

            this.SetXml("Wait");

            #endregion
        }

        private void BtnSchema_Click(object sender, EventArgs e)
        {
            #region 设置停诊刷新间隔

            this.SetXml("Schema");

            #endregion
        }

        private void BtnRegCancel_Click(object sender, EventArgs e)
        {
            #region 设置取消挂号刷新间隔

            this.SetXml("RegCancel");

            #endregion
        }

        private void BtnRegAccept_Click(object sender, EventArgs e)
        {
            #region 设置挂号接诊刷新间隔

            this.SetXml("RegAccept");

            #endregion
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnMinSize_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string appConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\App.config";
            if (System.IO.File.Exists(appConfigFile))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(appConfigFile);
                    string innerText = string.Empty;
                    System.Xml.XmlNode node;
                    node = GetSection(doc, "AutoMessageSetting/FeeExc");
                    if (!busy && node.InnerText == "true")
                    {
                        if (!feeMessage.IsBusy)
                        {
                            feeMessage.RunWorkerAsync();
                        }
                        
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string appConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\App.config";
            if (System.IO.File.Exists(appConfigFile))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(appConfigFile);
                    string innerText = string.Empty;
                    System.Xml.XmlNode node;
                    node = GetSection(doc, "AutoMessageSetting/WaitExc");
                    if (!busy && node.InnerText == "true")
                    {
                        if (!waitMessage.IsBusy)
                        {
                            waitMessage.RunWorkerAsync();
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            string appConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\App.config";
            if (System.IO.File.Exists(appConfigFile))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(appConfigFile);
                    string innerText = string.Empty;
                    System.Xml.XmlNode node;
                    node = GetSection(doc, "AutoMessageSetting/SchemaExc");
                    if (!busy && node.InnerText == "true")
                    {
                        if (!schemaMessage.IsBusy)
                        {
                            schemaMessage.RunWorkerAsync();
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            string appConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\App.config";
            if (System.IO.File.Exists(appConfigFile))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(appConfigFile);
                    string innerText = string.Empty;
                    System.Xml.XmlNode node;
                    node = GetSection(doc, "AutoMessageSetting/RegAcceptExc");
                    if (!busy && node.InnerText == "true")
                    {
                        if (!acceptReg.IsBusy)
                        {
                            acceptReg.RunWorkerAsync();
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            string appConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\App.config";
            if (System.IO.File.Exists(appConfigFile))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                try
                {
                    doc.Load(appConfigFile);
                    string innerText = string.Empty;
                    System.Xml.XmlNode node;
                    node = GetSection(doc, "AutoMessageSetting/RegCancelExc");
                    if (!busy && node.InnerText == "true")
                    {
                        if (!cancelReg.IsBusy)
                        {
                            cancelReg.RunWorkerAsync();
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
        }
    }
}
