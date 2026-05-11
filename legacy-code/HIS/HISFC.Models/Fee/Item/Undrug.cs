using System;
using System.Data;
using System.Collections;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.Fee.Item
{
	/// <summary>
	/// Undrug<br></br>
	/// [功能描述: 非药品信息类]<br></br>
	/// [创 建 者: 王宇]<br></br>
	/// [创建时间: 2006-08-30]<br></br>
	/// <修改记录
	///		修改人=''
	///		修改时间='yyyy-mm-dd'
	///		修改目的=''
	///		修改描述=''
	///  />
	/// </summary>
    /// 
    [System.Serializable]
	public class Undrug : Base.Item 
	{
		public Undrug() 
		{
			this.IsNeedConfirm = false;
		}
		
		#region 变量
		
		/// <summary>
		/// 执行科室(字符串形式)
		/// </summary>
		private string execDept;
		
		/// <summary>
		/// 执行科室(数组形式)
		/// </summary>
		private ArrayList execDepts =null;// new ArrayList();
		
		/// <summary>
		/// 比例信息
		/// </summary>
		private FTRate ftRate =null;// new FTRate();
		
		/// <summary>
		/// 是否计划生育相关项目
		/// </summary>
		private bool isFamilyPlanning;
		
		/// <summary>
		/// 机器编号(字符串形式)
		/// </summary>
		private string machineNO;
		
		/// <summary>
		/// 机器编号(数组形式)
		/// </summary>
		private ArrayList machineNOs =null;// new ArrayList();
		
		/// <summary>
		/// 默认检查部位
		/// </summary>
		private string checkBody;
		
		/// <summary>
		/// 是否与物资对照
		/// </summary>
		private bool isCompareToMaterial;
		
		/// <summary>
		/// 操作环境(操作员,操作时间,操作科室)
		/// </summary>
		private OperEnvironment oper =null;// new OperEnvironment();
		
		/// <summary>
		/// 手术信息
		/// </summary>
		private NeuObject operationInfo =null;// new NeuObject();
		
		/// <summary>
		/// 手术分类
		/// </summary>
		private NeuObject operationType =null;// new NeuObject();
		
		/// <summary>
		/// 手术规模
		/// </summary>
		private NeuObject operationScale =null;// new NeuObject();
		
		/// <summary>
		/// 疾病类型
		/// </summary>
		private NeuObject diseaseType =null;// new NeuObject();
		
		/// <summary>
		/// 专科信息
		/// </summary>
		private Department specialDept =null;// new Department();
		
		/// <summary>
		/// 是否知情同意
		/// </summary>
		private bool isConsent;

		/// <summary>
		/// 病史及检查
		/// </summary>
		private string medicalRecord;

		/// <summary>
		/// 检查要求  
		/// </summary>
		private string checkRequest;
	
		/// <summary>
		/// 注意事项           
		/// </summary>
		private string notice;

		/// <summary>
		/// 检查申请单名称  
		/// </summary>
		private string checkApplyDept;
		
		/// <summary>
		/// 项目范围
		/// </summary>
		private string itemArea = "";
		
		/// <summary>
		/// 项目例外
		/// </summary>
		private string itemException;

        /// <summary>
        /// 单位标识(0,明细; 1,组套)
        /// </summary>
        private string unitFlag="";

        /// <summary>
        /// 适用范围
        /// </summary>
        private string applicabilityArea = "";

        /// <summary>
        /// 执行状态(0未执行，1已执行)(体检从453版本移植)
        /// </summary>
        private string executeFlag;

        /// <summary>
        /// 注册证编号(体检从453版本移植)
        /// </summary>
        private string registerNo;

        //{2A5608D8-26AD-47d7-82CC-81375722FF72}
        /// <summary>
        /// 允许开立该项目的科室
        /// </summary>
        private string deptList;
        //{55CFCB36-B084-4a56-95AD-2CDED962ADC4}
        /// <summary>
        /// 物价费用类别
        /// </summary>
        private string itemPriceType;
        /// <summary>
        /// 是否打印医嘱单
        /// </summary>
        private string isOrderPrint;
        /// <summary>
        /// 项目名称信息
        /// </summary>
        private Neusoft.HISFC.Models.IMA.NameService nameCollection = null;

        /// <summary>
        /// 显示频次
        /// </summary>
        private bool isShowFre;
		#endregion

		#region 属性
        /// <summary>
        /// 适用范围
        /// </summary>
        public string ApplicabilityArea
        {
            get
            {
                return applicabilityArea;
            }
            set
            {
                applicabilityArea = value;
            }
        }
        /// <summary>
        /// 单位标识(0,明细; 1,组套)
        /// </summary>
        public string UnitFlag
        {
            get
            {
                return this.unitFlag;
            }
            set
            {
                this.unitFlag = value;
            }
        }

		/// <summary>
		/// 执行科室(字符串)
		/// </summary>
		public string ExecDept
		{
			get
			{
				return execDept;
			}
			set
			{
				execDept = value;

				string[] s = value.Split('|');

				this.ExecDepts.Clear();

                this.ExecDepts.CopyTo(s, 0);
			}
		}
		
		/// <summary>
		/// 执行科室(数组形式)
		/// </summary>
		public ArrayList ExecDepts
		{
			get
			{
                if (execDepts == null)
                {
                    execDepts = new ArrayList();
                }
				return this.execDepts;
			}
			set
			{
				this.execDepts = value;
			}
		}
		
		/// <summary>
		/// 比例信息
		/// </summary>
		public FTRate FTRate 
		{
			get
			{
                if (ftRate == null)
                {
                    ftRate = new FTRate();
                }
				return this.ftRate;
			}
			set
			{
				this.ftRate = value;
			}
		}

		/// <summary>
		/// 是否计划生育相关项目
		/// </summary>
		public bool IsFamilyPlanning
		{
			get
			{
				return this.isFamilyPlanning;
			}
			set
			{
				this.isFamilyPlanning = value;
			}
		}
		
		/// <summary>
		/// 设备编号(字符串)
		/// </summary>
		public string MachineNO
		{
			get
			{
				return this.machineNO;
			}
			set
			{
				this.machineNO = value;
				
				string[] s = value.Split('|');

				this.MachineNOs.Clear();

				this.MachineNOs.CopyTo(s, 0);
			}
		}

		///<summary>
		///设备编号(数组)
		///</summary>
		public ArrayList MachineNOs
		{
			get
			{
                if (machineNOs == null)
                {
                    machineNOs = new ArrayList();
                }
				return this.machineNOs;
			}
			set
			{
				this.machineNOs = value;
			}
		}

		/// <summary>
		/// 默认检查部位
		/// </summary>
		public string CheckBody
		{
			get
			{
				return this.checkBody;
			}
			set
			{
				this.checkBody = value;
			}
		}

		/// <summary>
		/// 是否与物资对照
		/// </summary>
		public bool IsCompareToMaterial
		{
			get
			{
				return this.isCompareToMaterial;
			}
			set
			{
				this.isCompareToMaterial = value;
			}
		}

		/// <summary>
		/// 操作环境(操作员,操作时间,操作科室)
		/// </summary>
		public OperEnvironment Oper
		{
			get
			{
                if (oper == null)
                {
                    oper = new OperEnvironment();
                }
				return this.oper;
			}
			set
			{
				this.oper = value;
			}
		}

		/// <summary>
		/// 手术信息
		/// </summary>
		public NeuObject OperationInfo
		{
			get
			{
                if (operationInfo == null)
                {
                    operationInfo = new NeuObject();
                }

				return this.operationInfo;
			}
			set
			{
				this.operationInfo = value;
			}
		}
		
		/// <summary>
		/// 手术分类
		/// </summary>
		public NeuObject OperationType
		{
			get
			{
                if (operationType == null)
                {
                    operationType = new NeuObject();
                }
				return this.operationType;
			}
			set
			{
				this.operationType = value;
			}
		}
		
		/// <summary>
		/// 手术规模
		/// </summary>
		public NeuObject OperationScale
		{
			get
			{
                if (operationScale == null)
                {
                    operationScale = new NeuObject();
                }

				return this.operationScale;
			}
			set
			{
				this.operationScale = value;
			}
		}

		/// <summary>
		/// 疾病类型
		/// </summary>
		public NeuObject DiseaseType
		{
			get
			{
                if (diseaseType == null)
                {
                    diseaseType = new NeuObject();
                }
				return this.diseaseType;
			}
			set
			{
				this.diseaseType = value;
			}
		}
		
		/// <summary>
		/// 专科信息
		/// </summary>
		public Department SpecialDept
		{
			get
			{
                if (specialDept == null)
                {
                    specialDept = new Department();
                }
				return this.specialDept;
			}
			set
			{
				this.specialDept = value;
			}
		}
		
		/// <summary>
		/// 是否知情同意
		/// </summary>
		public bool IsConsent
		{
			get
			{
				return this.isConsent;
			}
			set
			{
				this.isConsent = value;
			}
		}

		/// <summary>
		/// 病史及检查
		/// </summary>
		public string MedicalRecord
		{
			get
			{
				return this.medicalRecord;
			}
			set
			{
				this.medicalRecord = value;
			}
		}

		/// <summary>
		/// 检查要求  
		/// </summary>
		public string CheckRequest
		{
			get
			{
				return this.checkRequest;
			}
			set
			{
				this.checkRequest = value;
			}
		}
	
		/// <summary>
		/// 注意事项           
		/// </summary>
		public string Notice
		{
			get
			{
				return this.notice;
			}
			set
			{
				this.notice = value;
			}
		}

		/// <summary>
		/// 检查申请单名称  
		/// </summary>
		public string CheckApplyDept
		{
			get
			{
				return this.checkApplyDept;
			}
			set
			{
				this.checkApplyDept = value;
			}
		}

		/// <summary>
		/// 项目范围
		/// </summary>
		public string ItemArea
		{
			get
			{
				return this.itemArea;
			}
			set
			{
				this.itemArea = value;
			}
		}
		
		/// <summary>
		/// 项目例外
		/// </summary>
		public string ItemException
		{
			get
			{
				return this.itemException;
			}
			set
			{
				this.itemException = value;
			}
		}

        //{4E5ADF40-F21E-403d-AC37-07795BBC3071}
        /// <summary>
        /// 执行状态(0未执行，1已执行)(体检从453版本移植)
        /// </summary>
        public string ExecuteFlag
        {
            get
            {
                return executeFlag;
            }
            set
            {
                executeFlag = value;
            }
        }

        //{4E5ADF40-F21E-403d-AC37-07795BBC3071}
        /// <summary>
        /// 注册证编号(体检从453版本移植)
        /// </summary>
        public string RegisterNo
        {
            get
            {
                return registerNo;
            }
            set
            {
                registerNo = value;
            }
        }
        //{2A5608D8-26AD-47d7-82CC-81375722FF72}
        /// <summary>
        /// 允许开立该项目的科室
        /// </summary>
        public string DeptList
        {
            get
            {
                return deptList;
            }
            set
            {
                deptList = value;
            }
        }
        /// <summary>
        /// 物价费用类别
        /// </summary>
        public string ItemPriceType
        {
            get
            {
                return this.itemPriceType;
            }
            set
            {
                this.itemPriceType = value;
            }
        }
        /// <summary>
        /// 医嘱单打印属性
        /// </summary>
        public string IsOrderPrint
        {
            get
            {
                return this.isOrderPrint;
            }
            set
            {
                this.isOrderPrint = value;
            }
        }
        /// <summary>
        /// 6岁儿童码
        /// </summary>
        public string SixYearsOldCountryCode { get; set; }

        /// <summary>
        /// 项目名称信息
        /// </summary>
        [System.ComponentModel.DisplayName("名称集合")]
        [System.ComponentModel.Description("包括通用名，别名，英文名，国际编码，国家编码")]
        public Neusoft.HISFC.Models.IMA.NameService NameCollection
        {
            get
            {
                if (this.nameCollection == null)
                {
                    this.nameCollection = new Neusoft.HISFC.Models.IMA.NameService();
                }
                return this.nameCollection;
            }
            set
            {
                this.nameCollection = value;
            }
        }

        /// <summary>
        /// 显示频次
        /// </summary>
        public bool IsShowFre
        {
            get
            {
                return this.isShowFre;
            }
            set
            {
                this.isShowFre = value;
            }
        }

       // private 
		#endregion

		#region 方法
		
		#region 克隆
		
		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns>返回当前对象实例副本</returns>
		public new Undrug Clone()
		{
			Undrug undrug = base.Clone() as Undrug;

			undrug.FTRate = this.FTRate.Clone();
			undrug.Oper = this.Oper.Clone();
			undrug.OperationInfo = this.OperationInfo.Clone();
			undrug.OperationScale = this.OperationScale.Clone();
			undrug.OperationType = this.OperationType.Clone();
			undrug.DiseaseType = this.DiseaseType.Clone();
			undrug.SpecialDept = this.SpecialDept.Clone();
            undrug.nameCollection = this.NameCollection.Clone();
			return undrug;
		}

		#endregion

		#endregion
   }
}