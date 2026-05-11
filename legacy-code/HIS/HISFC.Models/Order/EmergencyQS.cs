using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Order
{
    public class EmergencyQS : Neusoft.FrameWork.Models.NeuObject
    {
        #region
        /// <summary>
        /// 门诊号/发票号
        /// </summary>
        private string clinic_code = string.Empty;

        /// <summary>
        /// 就诊卡号
        /// </summary>
        private string card_no = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        private string sex_code = string.Empty;

        /// <summary>
        /// 出生日
        /// </summary>
        private DateTime birthday = DateTime.MinValue;

        /// <summary>
        /// 挂号日期
        /// </summary>
        private DateTime reg_date = DateTime.MinValue;

        /// <summary>
        /// 分诊护士代码
        /// </summary>
        private string triage_opcd = string.Empty;

        /// <summary>
        /// 诊断名称
        /// </summary>
        private string diag_name = string.Empty;

        /// <summary>
        /// 急诊患者分级
        /// </summary>
        private string level = string.Empty;

        /// <summary>
        /// 患者去向
        /// </summary>
        private string gone = string.Empty;

        /// <summary>
        /// 患者其他去向
        /// </summary>
        private string goother = string.Empty;

        /// <summary>
        /// 入留观室时间
        /// </summary>
        private DateTime inobservation = DateTime.MinValue;

        /// <summary>
        /// 出留观室时间
        /// </summary>
        private DateTime outobservation = DateTime.MinValue;

        /// <summary>
        /// 留观时间
        /// </summary>
        private decimal observationtime = decimal.Zero;

        /// <summary>
        /// 入抢救室
        /// </summary>
        private string rescue = string.Empty;

        /// <summary>
        /// 入抢救室时间
        /// </summary>
        private DateTime inrescue = DateTime.MinValue;

        /// <summary>
        /// 出抢救室时间
        /// </summary>
        private DateTime outrescue = DateTime.MinValue;

        /// <summary>
        /// 抢救时间
        /// </summary>
        private decimal rescuetime = decimal.Zero;

        /// <summary>
        /// 死亡
        /// </summary>
        private string death = string.Empty;

        /// <summary>
        /// 收入院
        /// </summary>
        private string inhospital = string.Empty;

        /// <summary>
        /// 收入科室
        /// </summary>
        private string indept = string.Empty;

        /// <summary>
        /// 心脏复苏
        /// </summary>
        private string heartrescue = string.Empty;

        /// <summary>
        /// 自主呼吸循环恢复超过24小时
        /// </summary>
        private string breath = string.Empty;

        /// <summary>
        /// 急诊手术
        /// </summary>
        private string emoperation = string.Empty;

        /// <summary>
        /// 术后一周内死亡
        /// </summary>
        private string isdeath = string.Empty;

        /// <summary>
        /// 72小时内非计划重返抢救室
        /// </summary>
        private string returnrescue = string.Empty;

        /// <summary>
        /// 绿色通道
        /// </summary>
        private string greenchannel = string.Empty;

        /// <summary>
        /// 绿色通道原因
        /// </summary>
        private string gcreason = string.Empty;

        /// <summary>
        /// 心肌梗死
        /// </summary>
        private string heartdeath = string.Empty;

        /// <summary>
        /// 行急诊pci术时间
        /// </summary>
        private DateTime inpci = DateTime.MinValue;

        /// <summary>
        /// 门球时间
        /// </summary>
        private decimal pcitime = decimal.Zero;

        /// <summary>
        /// 使用溶栓药物时间
        /// </summary>
        private DateTime thrombolysis = DateTime.MinValue;

        /// <summary>
        /// 门药时间
        /// </summary>
        private decimal thrombolysistime = decimal.Zero;


        /// <summary>
        /// 登记护士
        /// </summary>
        private string operneu = string.Empty;

        /// <summary>
        /// 保存医生
        /// </summary>
        private string operdoc = string.Empty;

        /// <summary>
        /// 护士登记时间
        /// </summary>
        private DateTime neudate = DateTime.MinValue;

        /// <summary>
        /// 医生保存时间
        /// </summary>
        private DateTime docdate = DateTime.MinValue;

        /// <summary>
        /// 护士检诊
        /// </summary>
        private string diag_neu = string.Empty;

        /// <summary>
        /// 联系方式
        /// </summary>
        private string contact = string.Empty;

        /// <summary>
        /// 挂号科室
        /// </summary>
        private string dept = string.Empty;

        /// <summary>
        /// 绿色通道的其他原因
        /// </summary>
        private string gcother = string.Empty;

        #endregion

        #region
        /// <summary>
        /// 门诊号/发票号
        /// </summary>
        public string Clinic_code
        {
            get { return clinic_code; }
            set { clinic_code = value; }
        }


        /// <summary>
        /// 就诊卡号
        /// </summary>
        public string Card_no
        {
            get { return card_no; }
            set { card_no = value; }
        }


        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// 性别
        /// </summary>
        public string Sex_code
        {
            get { return sex_code; }
            set { sex_code = value; }
        }


        /// <summary>
        /// 出生日
        /// </summary>
        public DateTime Birthday
        {
            get { return birthday; }
            set { birthday = value; }
        }


        /// <summary>
        /// 挂号日期
        /// </summary>
        public DateTime Reg_date
        {
            get { return reg_date; }
            set { reg_date = value; }
        }


        /// <summary>
        /// 分诊护士代码
        /// </summary>
        public string Triage_opcd
        {
            get { return triage_opcd; }
            set { triage_opcd = value; }
        }

        /// <summary>
        /// 诊断名称
        /// </summary>
        public string Diag_name
        {
            get { return diag_name; }
            set { diag_name = value; }
        }

        /// <summary>
        /// 急诊患者分级
        /// </summary>
        public string Level
        {
            get { return level; }
            set { level = value; }
        }

        /// <summary>
        /// 患者去向
        /// </summary>
        public string Gone
        {
            get { return gone; }
            set { gone = value; }
        }

        /// <summary>
        /// 患者其他去向
        /// </summary>
        public string Goother
        {
            get { return goother; }
            set { goother = value; }
        }

        /// <summary>
        /// 入留观室时间
        /// </summary>
        public DateTime Inobservation
        {
            get { return inobservation; }
            set { inobservation = value; }
        }

        /// <summary>
        /// 出留观室时间
        /// </summary>
        public DateTime Outobservation
        {
            get { return outobservation; }
            set { outobservation = value; }
        }

        /// <summary>
        /// 留观时间
        /// </summary>
        public decimal Observationtime
        {
            get { return observationtime; }
            set { observationtime = value; }
        }

        /// <summary>
        /// 是否入抢救室
        /// </summary>
        public string Rescue
        {
            get { return rescue; }
            set { rescue = value; }
        }

        /// <summary>
        /// 入抢救室时间
        /// </summary>
        public DateTime Inrescue
        {
            get { return inrescue; }
            set { inrescue = value; }
        }

        /// <summary>
        /// 出抢救室时间
        /// </summary>
        public DateTime Outrescue
        {
            get { return outrescue; }
            set { outrescue = value; }
        }

        /// <summary>
        /// 抢救时间
        /// </summary>
        public decimal Rescuetime
        {
            get { return rescuetime; }
            set { rescuetime = value; }
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public string Death
        {
            get { return death; }
            set { death = value; }
        }

        /// <summary>
        /// 收入院
        /// </summary>
        public string Inhospital
        {
            get { return inhospital; }
            set { inhospital = value; }
        }

        /// <summary>
        /// 收入科室
        /// </summary>
        public string Indept
        {
            get { return indept; }
            set { indept = value; }
        }

        /// <summary>
        /// 心脏复苏
        /// </summary>
        public string Heartrescue
        {
            get { return heartrescue; }
            set { heartrescue = value; }
        }

        /// <summary>
        /// 自主呼吸循环恢复超过24小时
        /// </summary>
        public string Breath
        {
            get { return breath; }
            set { breath = value; }
        }

        /// <summary>
        /// 急诊手术
        /// </summary>
        public string Emoperation
        {
            get { return emoperation; }
            set { emoperation = value; }
        }

        /// <summary>
        /// 术后一周内死亡
        /// </summary>
        public string Isdeath
        {
            get { return isdeath; }
            set { isdeath = value; }
        }

        /// <summary>
        /// 72小时内非计划重返抢救室
        /// </summary>
        public string Returnrescue
        {
            get { return returnrescue; }
            set { returnrescue = value; }
        }

        /// <summary>
        /// 绿色通道
        /// </summary>
        public string Greenchannel
        {
            get { return greenchannel; }
            set { greenchannel = value; }
        }

        /// <summary>
        /// 绿色通道原因
        /// </summary>
        public string Gcreason
        {
            get { return gcreason; }
            set { gcreason = value; }
        }

        /// <summary>
        /// 心肌梗死
        /// </summary>
        public string Heartdeath
        {
            get { return heartdeath; }
            set { heartdeath = value; }
        }

        /// <summary>
        /// 行急诊pci术时间
        /// </summary>
        public DateTime Inpci
        {
            get { return inpci; }
            set { inpci = value; }
        }

        /// <summary>
        /// 门球时间
        /// </summary>
        public decimal Pcitime
        {
            get { return pcitime; }
            set { pcitime = value; }
        }

        /// <summary>
        /// 使用溶栓药物时间
        /// </summary>
        public DateTime Thrombolysis
        {
            get { return thrombolysis; }
            set { thrombolysis = value; }
        }


        /// <summary>
        /// 门药时间
        /// </summary>
        public decimal Thrombolysistime
        {
            get { return thrombolysistime; }
            set { thrombolysistime = value; }
        }

        /// <summary>
        /// 登记护士
        /// </summary>
        public string Operneu
        {
            get { return operneu; }
            set { operneu = value; }
        }

        /// <summary>
        /// 保存医生
        /// </summary>
        public string Operdoc
        {
            get { return operdoc; }
            set { operdoc = value; }
        }

        /// <summary>
        /// 护士登记时间
        /// </summary>
        public DateTime Neudate
        {
            get { return neudate; }
            set { neudate = value; }
        }

        /// <summary>
        /// 医生保存时间
        /// </summary>
        public DateTime Docdate
        {
            get { return docdate; }
            set { docdate = value; }
        }

        /// <summary>
        /// 护士检诊
        /// </summary>
        public string Diag_neu
        {
            get { return diag_neu; }
            set { diag_neu = value; }
        }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact
        {
            get { return contact; }
            set { contact = value; }
        }

        /// <summary>
        /// 挂号科室
        /// </summary>
        public string Dept
        {
            get { return dept; }
            set { dept = value; }
        }

        /// <summary>
        /// 绿色通道的其他原因
        /// </summary>
        public string Gcother
        {
            get { return gcother; }
            set { gcother = value; }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new EmergencyQS Clone()
        {
            // TODO:  添加 CurePhase.Clone 实现
            EmergencyQS obj = base.Clone() as EmergencyQS;

            return obj;
        }

        #endregion
    }
}
