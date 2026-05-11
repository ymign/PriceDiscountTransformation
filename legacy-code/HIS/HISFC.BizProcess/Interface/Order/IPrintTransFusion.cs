using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface
{
    /// <summary>
    /// [功能描述: 输液卡、贴瓶单、瓶签打印接口]<br></br>
    /// [创 建 者: wolf]<br></br>
    /// [创建时间: 2004-10-12]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public interface IPrintTransFusion
    {
        /// <summary>
        /// 打印
        /// </summary>
        void Print();

        /// <summary>
        /// 查询批量患者指定时间点的，指定用法的输液卡
        /// </summary>
        /// <param name="patients"></param>
        /// <param name="usagecode"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <param name="isPrinted">是否补打</param>
        /// <param name="orderType">医嘱类别:长嘱LONG、临嘱SHORT、全部ALL</param>
        /// <param name="isFirst">是否首日量</param>
        void Query(List<Neusoft.HISFC.Models.RADT.PatientInfo> patients, string usagecode, DateTime dtBegin, DateTime dtEnd, bool isRePrint, string orderType, bool isFirst);

        /// <summary>
        /// 打印设置
        /// </summary>
        void PrintSet();

        /// <summary>
        /// 特殊医嘱类型
        /// </summary>
        void SetSpeOrderType(string speStr);

        /// <summary>
        /// 停止后是否打印
        /// </summary>
        bool DCIsPrint
        {
            get;
            set;
        }

        /// <summary>
        /// 未收费知否打印
        /// </summary>
        bool NoFeeIsPrint
        {
            get;
            set;
        }


        /// <summary>
        /// 退费是否打印
        /// </summary>
        bool QuitFeeIsPrint
        {
            get;
            set;
        }

    }
}
