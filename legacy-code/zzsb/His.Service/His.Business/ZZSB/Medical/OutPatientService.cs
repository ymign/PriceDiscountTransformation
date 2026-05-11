using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Business.OutpatientWebService;
using Shadow.Util.Data.Func;

namespace His.Business.ZZSB.Medical
{
    public class OutPatientService
    {
        /// <summary>
        /// 医保api接口中间服务
        /// </summary>
        private Outpatient op = new Outpatient();

        #region 获取人员信息接口1101
        /// <summary>
        /// 获取人员信息接口1101
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="responseModel"></param>
        /// <returns></returns>
        public PersonResponseModel QueryPerson(PersonRequestModel requestModel)
        {
            PersonResponseModel responseModel = new PersonResponseModel();
            try
            {
                
                //记录下入参日志
                Shadow.Util.Data.Func.Log.WriteLog("人员信息接口1101入参", requestModel.ToString());
                //调用医保中间服务
                responseModel = op.QueryPerson(requestModel);
                Shadow.Util.Data.Func.Log.WriteLog("人员信息接口1101出参", responseModel.ToString());
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 门诊挂号接口2201
        /// <summary>
        /// 门诊挂号接口2201
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="responseModel"></param>
        /// <returns></returns>
        public ClinicRegisterResponseModel Register(ClinicRegisterRequestModel requestModel)
        {
            ClinicRegisterResponseModel responseModel = new ClinicRegisterResponseModel();
            try
            {   //记录下入参日志
                Shadow.Util.Data.Func.Log.WriteLog("门诊挂号接口2201入参", requestModel.ToString());
                //调用医保中间服务
                responseModel = op.Register(requestModel);
                Shadow.Util.Data.Func.Log.WriteLog("门诊挂号接口2201出参", responseModel.ToString());

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 门诊挂号撤销接口2202
        /// <summary>
        /// 门诊挂号撤销接口2202
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ClinicCancelRegisterResponseModel CancelRegister(ClinicCancelRegisterRequestModel requestModel)
        {
            ClinicCancelRegisterResponseModel responseModel = new ClinicCancelRegisterResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.CancelRegister(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 门诊就诊信息上传接口2203
        /// <summary>
        /// 门诊就诊信息上传接口2203
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ClinicMedicalInfoUploadResponseModel UploadMedInfo(ClinicMedicalInfoUploadRequestModel requestModel)
        {
            ClinicMedicalInfoUploadResponseModel responseModel = new ClinicMedicalInfoUploadResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.UploadMedInfo(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 门诊费用明细上传接口2204
        /// <summary>
        /// 门诊费用明细上传接口2204
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ClinicFeeDetailUploadResponseModel UploadFeeInfo(ClinicFeeDetailUploadRequestModel requestModel)
        {
            ClinicFeeDetailUploadResponseModel responseModel = new ClinicFeeDetailUploadResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.UploadFeeInfo(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 门诊结算接口2207
        /// <summary>
        /// 门诊结算接口2207
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ClinicBalanceResponseModel Balance(ClinicBalanceRequestModel requestModel)
        {
            ClinicBalanceResponseModel responseModel = new ClinicBalanceResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.Balance(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 门诊费用明细信息撤销接口2205
        /// <summary>
        /// 门诊费用明细信息撤销接口2205
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public CancelFeeDetailUploadResponseModel CancelUploadFeeInfo(CancelFeeDetailUploadRequestModel requestModel)
        {
            CancelFeeDetailUploadResponseModel responseModel = new CancelFeeDetailUploadResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.CancelUploadFeeInfo(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 门诊结算撤销接口2208
        /// <summary>
        /// 门诊结算撤销接口2208
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ClinicCancelBalanceResponseModel CancelBalance(ClinicCancelBalanceRequestModel requestModel)
        {
            ClinicCancelBalanceResponseModel responseModel = new ClinicCancelBalanceResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.CancelBalance(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 人员慢特病备案查询接口5301
        /// <summary>
        ///人员慢特病备案查询接口5301
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public PersonDetailResponseModel QueryPersonDetail(PersonDetailRequestModel requestModel)
        {
            PersonDetailResponseModel responseModel = new PersonDetailResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.QueryPersonDetail(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion

        #region 人员待遇享受检查2001
        /// <summary>
        /// 人员待遇享受检查2001
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public TreatmentCheckResponseModel TreatmentCheck(TreatmentCheckRequestModel requestModel)
        {
            TreatmentCheckResponseModel responseModel = new TreatmentCheckResponseModel();
            try
            {   //记录下入参日志
                //log.WriteLog(JsonConvert.SerializeObject(requestModel));
                //调用医保中间服务
                responseModel = op.TreatmentCheck(requestModel);
                //log.WriteLog(JsonConvert.SerializeObject(responseModel));

            }
            catch (Exception ex)
            {
                responseModel.ErrorMsg = ex.Message;
                responseModel.Status = "-1";
                return responseModel;
            }

            return responseModel;
        }
        #endregion
    }
}
