using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.OutPatient
{
    public class ModelConverter
    {
        public static Models.FIN_OPR_BOOKING ToFinOprBooking(Models.PLATFORM_REGISTER_ORDER order, Models.OperInfo oper)
        {
            BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
            Models.FIN_OPR_SCHEMA schema = schedulingLogic.Get(order.SCHEDULEID);
            BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
            Models.COM_PATIENTINFO patient = patientInfoLogic.Get(order.PATIENTID);
            Models.FIN_OPR_BOOKING booking = new Models.FIN_OPR_BOOKING();
            booking.ADDRESS = order.ADDRESS;
            booking.APP_FLAG = "0";
            //booking.APP_SENDFLAG = "1";  //这个暂时不知道用来是干什么的
            booking.BEGIN_TIME = SqlFunc.ToDate(order.BEGINTIME);
            booking.BIRTHDAY = order.BIRTH;
            booking.BOOKING_DATE = SqlFunc.ToDate(order.ORDERTIME);
            booking.CARD_NO = order.CARDNO;
            //booking.CLINIC_CODE = order.CLINIC_CODE;  //这个需要到外面去赋值流水号
            //booking.CONFIRM_DATE  //此时也还没有确认时间
            //booking.CONFIRM_OPCD  //此时也还没有确认操作人
            booking.DEPT_CODE = order.DEPTCODE;
            booking.DEPT_NAME = schema.DEPT_NAME;
            booking.DOCT_CODE = schema.DOCT_CODE;
            booking.DOCT_NAME = schema.DOCT_NAME;
            booking.END_TIME = SqlFunc.ToDate(order.ENDTIME);
            booking.IDENNO = patient.IDENNO;
            booking.NAME = patient.NAME;
            booking.NOON_CODE = schema.NOON_CODE;
            booking.OPER_CODE = oper.Code;
            booking.OPER_DATE = oper.Time;
            booking.REGLEVL_CODE = schema.REGLEVL_CODE;
            //booking.REG_ID  //此时还没有挂号流水号
            booking.RELA_PHONE = order.MOBILE;
            booking.SCHEMA_NO = schema.ID;
            booking.SEE_FLAG = "0";
            booking.SEX_CODE = patient.SEX_CODE;
            booking.SOURCE = order.FRONTPROVIDERID;
            booking.VALID_FLAG = "1";
            return booking;
        }


        public static Neusoft.HISFC.Models.Registration.Schema ToNeuSoftSchema(Models.FIN_OPR_SCHEMA schema, Models.OperInfo oper)
        {
            Neusoft.HISFC.Models.Registration.Schema res = new Neusoft.HISFC.Models.Registration.Schema();
            


            return res;
        }
    }
}
