using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HealthRecord.Case
{
    /// <summary>
    /// [功能描述: 病案库房管理实体]<br></br>
    /// [创 建 者: 成郁明]<br></br>
    /// [创建时间: 2011/06/17]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class CaseStore : Neusoft.FrameWork.Models.NeuObject
    {
        /// <summary>
        /// 患者信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo patienInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
        /// <summary>
        /// 病案库房信息
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject store = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 病案柜信息
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject cabinet = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 病案柜格信息
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject grid = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 病案状态信息
        /// </summary>
        private string caseState;
        /// <summary>
        /// 病案是否有效
        /// </summary>
        private bool isVaild;
        /// <summary>
        /// 备注
        /// </summary>
        private string caseMemo;
        /// <summary>
        /// 操作员信息 编号 时间
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment operEnv = new Neusoft.HISFC.Models.Base.OperEnvironment();
        /// <summary>
        /// 病案出入库时间
        /// </summary>
        private DateTime caseInOutDate;
        /// <summary>
        /// 扩展信息1
        /// </summary>
        private string extend1;
        /// <summary>
        /// 扩展信息2
        /// </summary>
        private string extend2;
        /// <summary>
        /// 扩展信息3
        /// </summary>
        private string extend3;
        /// <summary>
        /// 扩展信息4
        /// </summary>
        private string extend4;

        /// <summary>
        /// 患者信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo
        {
            get { return this.patienInfo; }
            set { this.patienInfo = value; }
        }
        /// <summary>
        /// 病案库房信息
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Store
        {
            get { return this.store; }
            set { this.store = value; }
        }
        /// <summary>
        /// 病案柜信息
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Cabinet
        {
            get { return this.cabinet; }
            set { this.cabinet = value; }
        }
        /// <summary>
        /// 病案柜格信息
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Grid
        {
            get { return this.grid; }
            set { this.grid = value; }
        }
        /// <summary>
        /// 病案状态信息
        /// </summary>
        public string CaseState
        {
            get { return this.caseState; }
            set { this.caseState = value; }
        }
        /// <summary>
        /// 病案是否有效
        /// </summary>
        public bool IsVaild
        {
            get { return this.isVaild; }
            set { this.isVaild = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string CaseMemo
        {
            get { return this.caseMemo; }
            set { this.caseMemo = value; }
        }
        /// <summary>
        /// 操作员信息 编号 时间
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment OperEnv
        {
            get { return this.operEnv; }
            set { this.operEnv = value; }
        }

        /// <summary>
        /// 病案出入库时间
        /// </summary>
        public DateTime CaseInOutDate
        {
            get { return this.caseInOutDate; }
            set { this.caseInOutDate = value; }
        }
        /// <summary>
        /// 扩展信息1
        /// </summary>
        public string Extend1
        {
            get { return this.extend1; }
            set { this.extend1 = value; }
        }
        /// <summary>
        /// 扩展信息2
        /// </summary>
        public string Extend2
        {
            get { return this.extend2; }
            set { this.extend2 = value; }
        }
        /// <summary>
        /// 扩展信息3
        /// </summary>
        public string Extend3
        {
            get { return this.extend3; }
            set { this.extend3 = value; }
        }
        /// <summary>
        /// 扩展信息4
        /// </summary>
        public string Extend4
        {
            get { return this.extend4; }
            set { this.extend4 = value; }
        }
        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>病案柜实体</returns>
        public new CaseStore Clone()
        {
            CaseStore casestore = base.Clone() as CaseStore;
            casestore.patienInfo = this.patienInfo.Clone();
            casestore.operEnv = this.operEnv.Clone();
            casestore.store = this.store.Clone();
            casestore.cabinet = this.cabinet.Clone();
            casestore.grid = this.grid.Clone();
            return casestore;
        }

        #endregion

    }

}
