using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HealthRecord.Case
{
    /// <summary>
    /// 住院病人动态日报对象

    /// </summary>
    public class PatientMove:Neusoft.FrameWork.Models.NeuObject
    {
        private string nextVal = "";

        public string NextVal
        {
            get
            {
                return nextVal;
            }
            set
            {
                nextVal = value;
            }
        }
        private string deptCode = "";
        private int bedNum;
        private int originalNum;
        private int inNum;
        private int otherDeptIn;
        private int otherRegionIn;
        private int outNum;
        private int deadNum=0;
        private int toOtherDept;
        private int toOtherRegion;
        private int patientNum;
        private int accompanyNum=0;
        private decimal beduseNum;
        private string operCode = "";
        private DateTime operDate = new DateTime();
        private string operDept = "";
        private int oppatientnum = 0;
        private int sicknum = 0;      
        private int salvenum = 0;       
        private int salvesuccnum = 0;       
        private int infectnum = 0;      
        private int inicudays = 0;
        private int nowinpatientnum = 0;
        
       
        /// <summary>
        /// 科室
        /// </summary>
        public string DeptCode
        {
            get { return deptCode; }
            set { deptCode = value; }
        }
        /// <summary>
        /// 开放床位数
        /// </summary>
        public int BedNum
        {
            get { return bedNum; }
            set { bedNum = value; }
        }
        /// <summary>
        /// 原有人数
        /// </summary>
        public int OriginalNum
        {
            get { return originalNum; }
            set { originalNum = value; }
        }
        /// <summary>
        /// 入院人数
        /// </summary>
        public int InNum
        {
            get { return inNum; }
            set { inNum = value; }
        }
        /// <summary>
        /// 他科转入
        /// </summary>
        public int OtherDeptIn
        {
            get { return otherDeptIn; }
            set { otherDeptIn = value; }
        }
        /// <summary>
        /// 他区转入
        /// </summary>
        public int OtherRegionIn
        {
            get { return otherRegionIn; }
            set { otherRegionIn = value; }
        }
        /// <summary>
        /// 出院总计
        /// </summary>
        public int OutNum
        {
            get { return outNum; }
            set { outNum = value; }
        }
        /// <summary>
        /// 死亡数

        /// </summary>
        public int DeadNum
        {
            get { return deadNum; }
            set { deadNum = value; }
        }
        /// <summary>
        /// 转往他科
        /// </summary>
        public int ToOtherDept
        {
            get { return toOtherDept; }
            set { toOtherDept = value; }
        }
        /// <summary>
        /// 转往他区
        /// </summary>
        public int ToOtherRegion
        {
            get { return toOtherRegion; }
            set { toOtherRegion = value; }
        }
        /// <summary>
        /// 现有人数
        /// </summary>
        public int PatientNum
        {
            get { return patientNum; }
            set { patientNum = value; }
        }
        /// <summary>
        /// 陪人数

        /// </summary>
        public int AccompanyNum
        {
            get { return accompanyNum; }
            set { accompanyNum = value; }
        }
        /// <summary>
        /// 病床占用率

        /// </summary>
        public decimal BeduseNum
        {
            get { return beduseNum; }
            set { beduseNum = value; }
        }
        /// <summary>
        /// 操作员代码

        /// </summary>
        public string OperCode
        {
            get { return operCode; }
            set { operCode = value; }
        }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate
        {
            get { return operDate; }
            set { operDate = value; }
        }
        /// <summary>
        /// 操作员科室

        /// </summary>
        public string OperDept
        {
            get { return operDept; }
            set { operDept = value; }
        }
        /// <summary>
        /// 手术人数
        /// </summary>
        public int Oppatientnum
        {
            get { return oppatientnum; }
            set { oppatientnum = value; }
        }
        /// <summary>
        /// 重病人数
        /// </summary>
        public int Sicknum
        {
            get { return sicknum; }
            set { sicknum = value; }
        }
        /// <summary>
        /// 抢救人数
        /// </summary>
        public int Salvenum
        {
            get { return salvenum; }
            set { salvenum = value; }
        }
        /// <summary>
        /// 抢救成功人数
        /// </summary>
        public int Salvesuccnum
        {
            get { return salvesuccnum; }
            set { salvesuccnum = value; }
        }
        /// <summary>
        /// 感染次数
        /// </summary>
        public int Infectnum
        {
            get { return infectnum; }
            set { infectnum = value; }
        }
        /// <summary>
        /// 转出病人住院天数
        /// </summary>
        public int Inicudays
        {
            get { return inicudays; }
            set { inicudays = value; }
        }
        /// <summary>
        /// 即时现在住院人数
        /// </summary>
        public int Nowinpatientnum
        {
            get { return nowinpatientnum; }
            set { nowinpatientnum = value; }
        }


    }
}
