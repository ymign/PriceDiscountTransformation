using GDSI.CountryMedical.Model;
using GDSI.MedicalOutpatientService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDSI.CountryMedical.Common;
using Neusoft.HISFC.Models.SIInterface;
using System.Collections;

namespace FS.ZDWY.Internet.BP.OutPatient
{
    public class PerSonMZGJRecordModel
    {
        public string MdtrCertTyp { get; set; }
        public string MdtrtCertNo { get; set; }
        public string PsnCertType { get; set; }
        public string InsuplcAdmdvs { get; set; }
        public string CardSN { get; set; }
        public string CertNo { get; set; }
    }
    public class PerSonMZGJSelectPointModel
    {
        public string MdtrCertTyp { get; set; }
        public string MdtrtCertNo { get; set; }
        public string PsnCertType { get; set; }
        public string InsuplcAdmdvs { get; set; }
        public string CardSN { get; set; }
        public string CertNo { get; set; }
        public string ChgRea { get; set; }
    }



    public class PerSonRecordManager
    {

        private PersonRequestModel GetInModel1101(PerSonMZGJRecordModel req)
        {
            PersonRequestModel inModel1101 = new PersonRequestModel();

            inModel1101.MdtrtCertType = req.MdtrCertTyp;
            inModel1101.MdtrtCertNo = req.MdtrtCertNo;
            inModel1101.CardSN = req.CardSN;
            inModel1101.CertNo = req.CertNo;
            inModel1101.InsuplcAdmdvs = req.InsuplcAdmdvs;
            inModel1101.PsnCertType = req.PsnCertType;
            return inModel1101;
        }
        private PersonRequestModel GetInModel1101(PerSonMZGJSelectPointModel req)
        {
            PersonRequestModel inModel1101 = new PersonRequestModel();

            inModel1101.MdtrtCertType = req.MdtrCertTyp;
            inModel1101.MdtrtCertNo = req.MdtrtCertNo;
            inModel1101.CardSN = req.CardSN;
            inModel1101.CertNo = req.CertNo;
            inModel1101.InsuplcAdmdvs = req.InsuplcAdmdvs;
            inModel1101.PsnCertType = req.PsnCertType;
            return inModel1101;
        }
        GDSI.CountryMedical.Service.MedicalOutService medicalOutService = new GDSI.CountryMedical.Service.MedicalOutService();
        public Models.Views.ComResult<Models.Views.OutPatient.HcareResult> PerSonMZGJRecord(PerSonMZGJRecordModel req)
        {
            Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();

            GDSI.MedicalOutpatientService.PersonRequestModel inModel1101 = this.GetInModel1101(req);
            var outModel1101 = medicalOutService.CallMedicalApi<PersonRequestModel, PersonResponseModel>(inModel1101, EnumCallAPIChannel.ZDWY_WX_GH, EnumMedicalApiInfNo.API1101);
            if (!outModel1101.IsMedicalAPISucess())
            {
                throw new Exception(medicalOutService.errMsg);
            }
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic PatientInfoLogic = new BL.OutPatient.PatientInfoLogic();
            var sysdate = PatientInfoLogic.GetDateTime().ToString("yyyy-MM-dd");
            GDSI.CountryMedical.DAL.QueryDAL queryDB = new GDSI.CountryMedical.DAL.QueryDAL();
            var patient = queryDB.GetPatientInfo(req.CertNo);
            PersonRecordRequestModel InModel2505 = new PersonRecordRequestModel();
            InModel2505.InsuplcAdmdvs = req.InsuplcAdmdvs;
            InModel2505.Data = new PersonRecordRQ();
            InModel2505.Data.PsnNo = outModel1101.BaseInfo.PsnNo;
            if (patient != null)
            {
                InModel2505.Data.Tel = patient.HOME_TEL;
                InModel2505.Data.Addr = patient.HOME_NOW;
            }
            else
            {
                InModel2505.Data.Tel = "-";
                InModel2505.Data.Addr = "-";
            }

            InModel2505.Data.BizAppyType = "99";
            InModel2505.Data.Begndate = sysdate;
            InModel2505.Data.Enddate = "2099-12-31";
            InModel2505.Data.AgnterName = "";
            InModel2505.Data.AgnterCertType = "";
            InModel2505.Data.AgnterCertno = "";
            InModel2505.Data.AgnterTel = "";
            InModel2505.Data.AgnterRlts = "";
            InModel2505.Data.FixSrtNo = "1";
            InModel2505.Data.Insutype = "310";
            InModel2505.Data.FixmedinsCode = "H44040200001";
            InModel2505.Data.FixmedinsName = "中山大学附属第五医院";
            InModel2505.Data.Memo = "";
            var OutModel2505 = medicalOutService.CallMedicalApi<PersonRecordRequestModel, PersonRecordResponseModel>(InModel2505, EnumCallAPIChannel.ZDWY_WX_GH, EnumMedicalApiInfNo.API2505);
            if (!OutModel2505.IsMedicalAPISucess())
            {
                if (medicalOutService.errMsg.Contains("需先登记门诊统筹机构"))
                {
                    medicalOutService.errMsg = "您好，系统提示您尚未办理珠海门诊统筹定点，请先在珠海社保掌上办、粤医保微信小程序上做好门诊统筹定点登记，再选定我院为职工医保门诊共济定点，感谢您的配合！";
                }
                if (medicalOutService.errMsg.Contains("已经成功办理一次此病种业务"))
                {
                    medicalOutService.errMsg = "您好，系统提示您近期已办理过门诊共济定点，若您需变更到我院，请移步到我院门诊大厅任一收费和医保窗口办理！如需查询已定点医疗机构名称可进入微信小程序“粤医保”或“珠海社保掌上办”查询，感谢您的配合！";
                }
                if (medicalOutService.errMsg.Contains("门诊共济只支持职工险种登记"))
                {
                    medicalOutService.errMsg = "门诊共济只支持职工险种登记.";
                }
                throw new Exception(medicalOutService.errMsg);
            }
            GDSI.ZhuHaiSI.Model.PersonRecordModel model = new GDSI.ZhuHaiSI.Model.PersonRecordModel();
            model.PsnNo = outModel1101.BaseInfo.PsnNo;
            model.PsnName = outModel1101.BaseInfo.PsnName;
            model.CertNo = req.CertNo;
            model.Tel = InModel2505.Data.Tel;
            model.Addr = InModel2505.Data.Addr;
            model.BizAppyType = "99";
            model.Begndate = InModel2505.Data.Begndate;
            model.Enddate = InModel2505.Data.Enddate;
            model.AgnterName = "";
            model.AgnterCertType = "";
            model.AgnterCertno = "";
            model.AgnterTel = "";
            //model.AgnterAddr = this.dateTimePickerAgnterAddr.Value.ToString("yyyy-MM-dd"); ;//本身为代办人地址 后续创智改成了预产期
            model.AgnterRlts = "";
            model.FixSrtNo = InModel2505.Data.FixSrtNo;
            model.Valid = "1";
            model.Memo = "";
            model.FixmedinsCode = InModel2505.Data.FixmedinsCode;
            model.FixmedinsName = InModel2505.Data.FixmedinsName;
            model.OpterType = "3";
            model.OpterCode = "00A105";
            model.OpterName = "微信";
            model.TrtDclaDetlSn = OutModel2505.Result.TrtDclaDetlSn;
            GDSI.ZhuHaiSI.DB.DBFunction db = new GDSI.ZhuHaiSI.DB.DBFunction();
            if (db.InsertPersonRecord(model) < 0)
            {
                throw new Exception("插入人员备案信息表失败:" + db.ErrorMessage);

            }
            result.IsSuccessful = true;
            result.Message = "操作成功！";
            return result;
        }
        public Models.Views.ComResult<Models.Views.OutPatient.HcareResult> PerSonMZGJSele(PerSonMZGJRecordModel req, ref string Msg)
        {
            string returnStr = "";
            string LoadTime = DateTime.Now.ToString("yyyy-MM-dd");
            Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
            ArrayList FixedPointLists = new ArrayList();
            string IsInsutype = "0";
            GDSI.MedicalOutpatientService.PersonRequestModel inModel1101 = this.GetInModel1101(req);
            var outModel1101 = medicalOutService.CallMedicalApi<PersonRequestModel, PersonResponseModel>(inModel1101, EnumCallAPIChannel.ZDWY_WX_GH, EnumMedicalApiInfNo.API1101);
            if (outModel1101 == null || !outModel1101.IsMedicalAPISucess())
            {
                throw new Exception(medicalOutService.errMsg);
            }
            if (outModel1101.BaseInfo == null)
            {
                throw new Exception("BaseInfo 为空，无法获取 PsnNo");
            }
            if (outModel1101.Insuinfo != null)
            {
                for (int i = 0; i < outModel1101.Insuinfo.Length; i++)
                {
                    var insuInfo = outModel1101.Insuinfo[i];
                    if (insuInfo.PsnInsuStas == "1" && insuInfo.Insutype == "310")
                    {
                        IsInsutype = "1";
                    }
                }
            }
            FixedPointQueryRequestModel inModel5302 = new FixedPointQueryRequestModel();
            inModel5302.BizAppyType = "99";
            inModel5302.PsnNo = outModel1101.BaseInfo.PsnNo;
            var OutModel5302 = medicalOutService.CallMedicalApi<FixedPointQueryRequestModel, FixedPointQueryResponseModel>(inModel5302, EnumCallAPIChannel.ZDWY_CK_GH, EnumMedicalApiInfNo.API5302);
            if (!OutModel5302.IsMedicalAPISucess())
            {
                throw new Exception(medicalOutService.errMsg);
            }
            else
            {
                int state = 0;
                foreach (var item in OutModel5302.Psnfixmedin)
                {
                    GDSI.ZhuHaiSI.Model.PersonRecordModel model = new GDSI.ZhuHaiSI.Model.PersonRecordModel();
                    if (DateTime.Parse(item.Enddate) >= DateTime.Parse(LoadTime))
                    {
                        if (item.FixmedinsName.Contains("中山大学附属第五医院"))
                        {
                            state = 1;
                        }
                        model.PsnNo = item.PsnNo;
                        model.CertNo = "[" + item.Insutype + "]" + GDSI.ZhuHaiSI.Tool.ComDictionaryFunction.ChangeInsutype(item.Insutype);
                        model.FixmedinsCode = item.FixmedinsCode;
                        model.FixmedinsName = item.FixmedinsName;
                        model.Begndate = item.Begndate;
                        model.Enddate = item.Enddate;
                        model.Memo = item.Memo;
                        FixedPointLists.Add(model);
                    }
                }
                #region 返回串
                returnStr = string.Empty;
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                if (FixedPointLists.Count >= 3 && state != 1)
                {
                    Msg = "您的门诊共济尚未选点我院，请登录“珠海社保学上办”微信小程序或咨询医务人员操作选点我院;如遇操作异常请到我院收费窗口、医保窗口现场办理";
                }
                else if (state == 1)
                {
                    Msg = "您已选点我院为门诊共济选点医院，无需重复办理";
                }
                else if (FixedPointLists.Count <= 0)
                {
                    Msg = "没有查询到您的门诊共济选点信息";
                }     
                System.Xml.XmlElement Response = xml.CreateElement("Response");
                xml.AppendChild(Response);
                System.Xml.XmlElement ok = xml.CreateElement("ok");
                ok.InnerText = "true";
                Response.AppendChild(ok);
                System.Xml.XmlElement errorMsg = xml.CreateElement("errorMsg");
                errorMsg.InnerText = Msg; 
                Response.AppendChild(errorMsg);
                System.Xml.XmlElement data = xml.CreateElement("data");
                Response.AppendChild(data);

                System.Xml.XmlElement isInsutype = xml.CreateElement("IsInsutype");
                isInsutype.InnerText = IsInsutype;
                data.AppendChild(isInsutype);


                foreach (GDSI.ZhuHaiSI.Model.PersonRecordModel Res in FixedPointLists)
                {
                    System.Xml.XmlElement Result = xml.CreateElement("item");
                    data.AppendChild(Result);

                    System.Xml.XmlElement PsnNo = xml.CreateElement("PsnNo");
                    PsnNo.InnerText = Res.PsnNo;
                    Result.AppendChild(PsnNo);

                    System.Xml.XmlElement Insutype = xml.CreateElement("Insutype");
                    Insutype.InnerText = Res.CertNo;
                    Result.AppendChild(Insutype);

                    System.Xml.XmlElement FixmedinsCode = xml.CreateElement("FixmedinsCode");
                    FixmedinsCode.InnerText = Res.FixmedinsCode;
                    Result.AppendChild(FixmedinsCode);

                    System.Xml.XmlElement FixmedinsName = xml.CreateElement("FixmedinsName");
                    FixmedinsName.InnerText = Res.FixmedinsName;
                    Result.AppendChild(FixmedinsName);

                    System.Xml.XmlElement Begndate = xml.CreateElement("Begndate");
                    Begndate.InnerText = Res.Begndate;
                    Result.AppendChild(Begndate);

                    System.Xml.XmlElement Enddate = xml.CreateElement("Enddate");
                    Enddate.InnerText = Res.Enddate;
                    Result.AppendChild(Enddate);


                    System.Xml.XmlElement Memo = xml.CreateElement("Memo");
                    Memo.InnerText = Res.Memo;
                    Result.AppendChild(Memo);
                }
                returnStr = xml.InnerXml.ToString();
                #endregion
            }
            result.IsSuccessful = true;
            result.Message = returnStr;
            return result;
        }
        public Models.Views.ComResult<Models.Views.OutPatient.HcareResult> PerSonMZGJSelectPoint(PerSonMZGJSelectPointModel req)
        {
            Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();

            GDSI.MedicalOutpatientService.PersonRequestModel inModel1101 = this.GetInModel1101(req);
            var outModel1101 = medicalOutService.CallMedicalApi<PersonRequestModel, PersonResponseModel>(inModel1101, EnumCallAPIChannel.ZDWY_WX_GH, EnumMedicalApiInfNo.API1101);
            if (!outModel1101.IsMedicalAPISucess())
            {
                throw new Exception(medicalOutService.errMsg);
            }
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic PatientInfoLogic = new BL.OutPatient.PatientInfoLogic();
            var begndate = PatientInfoLogic.GetDateTime().ToString("yyyy-MM-dd");
            PersonMZRecordChangePointRequestModel inModel2579 = new PersonMZRecordChangePointRequestModel();
            inModel2579.InsuplcAdmdvs = req.InsuplcAdmdvs;
            inModel2579.Data = new PersonMZRecordChangePointRQ();
            inModel2579.Data.Begndate = begndate;
            inModel2579.Data.Enddate = "2099-12-31";
            inModel2579.Data.PsnNo = outModel1101.BaseInfo.PsnNo;
            inModel2579.Data.ChgRea = req.ChgRea;
            inModel2579.Data.BizApplyType = "99";
            List<PsnFixedEvtDetlList> list = new List<PsnFixedEvtDetlList>();
            PsnFixedEvtDetlList model = new PsnFixedEvtDetlList();
            model.Begndate = begndate;
            model.Enddate = "2099-12-31";
            model.FixmedinsCode = "H44040200001";
            model.FixmedinsName = "中山大学附属第五医院";
            model.FixmedinsType = "1";
            model.MedinsLv = "02";
            list.Add(model);
            inModel2579.Data.PsnFixedEvtDetlList = list.ToArray();
            var outModel2579 = medicalOutService.CallMedicalApi<PersonMZRecordChangePointRequestModel, PersonMZRecordChangePointResponseModel>(inModel2579, EnumCallAPIChannel.ZDWY_WX_GH, EnumMedicalApiInfNo.API2579);
            if (!outModel2579.IsMedicalAPISucess())
            {
                throw new Exception(medicalOutService.errMsg);
            }
            result.IsSuccessful = true;
            result.Message = "操作成功！";
            return result;
        }
    }
}
