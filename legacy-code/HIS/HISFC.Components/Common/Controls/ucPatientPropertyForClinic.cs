using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Controls
{
    public partial class ucPatientPropertyForClinic : UserControl
    {
        public ucPatientPropertyForClinic()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 页面属性，接收传过来的患者信息
        /// </summary>
        public Neusoft.HISFC.Models.Registration.Register PatientInfo
        {
            get
            {
                return this.myPatientInfo;
            }
            set
            {
                this.myPatientInfo = value;
                ShowPatientProperty();
            }
        }

        /// <summary>
        /// 患者挂号信息
        /// </summary>
        private Neusoft.HISFC.Models.Registration.Register myPatientInfo = new Neusoft.HISFC.Models.Registration.Register();

        /// <summary>
        /// 显示用的患者信息类
        /// </summary>
        private PatientInfoForClinic patientInfo = new PatientInfoForClinic();

        /// <summary>
        /// 显示属性
        /// </summary>
        private void ShowPatientProperty()
        {
            if (this.PatientInfo != null)
            {
                Neusoft.HISFC.BizLogic.Order.OutPatient.Order orderManager = new Neusoft.HISFC.BizLogic.Order.OutPatient.Order();
                Neusoft.HISFC.BizProcess.Integrate.Manager interMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();
                this.patientInfo.PatientNo = myPatientInfo.PID.CardNO;//门诊病例号
                this.patientInfo.Sex = myPatientInfo.Sex.Name;//性别
                this.patientInfo.Age = orderManager.GetAge(myPatientInfo.Birthday);//年龄
                this.patientInfo.RegDoct = myPatientInfo.DoctorInfo.Templet.Doct.Name;
                this.patientInfo.RegDept = myPatientInfo.DoctorInfo.Templet.Dept.Name;//科室
                this.patientInfo.PatientName = myPatientInfo.Name;//姓名
                this.patientInfo.RegLevel = myPatientInfo.DoctorInfo.Templet.RegLevel.Name;
                this.patientInfo.RegDate = myPatientInfo.DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm");
                this.patientInfo.PactName = myPatientInfo.Pact.Name;

                //家庭地址为空时显示单位地址
                this.patientInfo.Address = !string.IsNullOrEmpty(myPatientInfo.AddressHome) ? myPatientInfo.AddressHome : myPatientInfo.AddressBusiness;
                this.patientInfo.IdenNo = myPatientInfo.IDCard;

                //家庭电话为空时显示单位电话
                this.patientInfo.RelaPhone = !string.IsNullOrEmpty(myPatientInfo.PhoneHome) ? myPatientInfo.PhoneHome : myPatientInfo.PhoneBusiness;
                this.patientInfo.SeeDate = myPatientInfo.SeeDoct.OperTime;

                if (!string.IsNullOrEmpty(myPatientInfo.SeeDoct.Dept.ID))
                {
                    this.patientInfo.SeeDept = interMgr.GetDepartment(myPatientInfo.SeeDoct.Dept.ID).Name;
                }
                if (!string.IsNullOrEmpty(myPatientInfo.SeeDoct.ID))
                {
                    this.patientInfo.SeeDoct = interMgr.GetEmployeeInfo(myPatientInfo.SeeDoct.ID).Name;
                }
            }

            this.propertyGrid1.SelectedObject = patientInfo;
        }
    }


    #region 属性类基类

    #region 所有要放在PropertyGird中的对像的基类.

    /// <summary>
    /// 所有要放在PropertyGird中的对像的基类.
    /// </summary>
    class IBasePropertyForClinic : ICustomTypeDescriptor
    {
        private PropertyDescriptorCollection globalizedProps;

        /// <summary>
        /// 获得组件名称
        /// </summary>
        /// <returns></returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// 获得属性值
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (globalizedProps == null)
            {
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

                globalizedProps = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor oProp in baseProps)
                {
                    globalizedProps.Add(new BasePropertyDescriptorForClinic(oProp));
                }
            }
            return globalizedProps;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            if (globalizedProps == null)
            {
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, true);
                globalizedProps = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor oProp in baseProps)
                {
                    globalizedProps.Add(new BasePropertyDescriptorForClinic(oProp));
                }
            }
            return globalizedProps;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }
    #endregion

    #region 所以要放在PropertyGird中的对像的描绘进行重写

    /// <summary>
    /// 要放在PropertyGird中的对像的描绘进行重写
    /// </summary>
    class BasePropertyDescriptorForClinic : PropertyDescriptor
    {
        private PropertyDescriptor basePropertyDescriptor;

        public BasePropertyDescriptorForClinic(PropertyDescriptor basePropertyDescriptor)
            : base(basePropertyDescriptor)
        {
            this.basePropertyDescriptor = basePropertyDescriptor;
        }

        public override bool CanResetValue(object component)
        {
            return basePropertyDescriptor.CanResetValue(component);
        }

        public override Type ComponentType
        {
            get { return basePropertyDescriptor.ComponentType; }
        }

        public override string DisplayName
        {
            get
            {
                string svalue = "";
                foreach (Attribute attribute in this.basePropertyDescriptor.Attributes)
                {
                    if (attribute is showChineseForClinic)
                    {
                        svalue = attribute.ToString();
                        break;
                    }
                }
                if (svalue == "") return this.basePropertyDescriptor.Name;
                else return svalue;
            }
        }

        public override string Description
        {
            get
            {
                return this.basePropertyDescriptor.Description;
            }
        }

        public override object GetValue(object component)
        {
            return this.basePropertyDescriptor.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get { return this.basePropertyDescriptor.IsReadOnly; }
        }

        public override string Name
        {
            get { return this.basePropertyDescriptor.Name; }
        }

        public override Type PropertyType
        {
            get { return this.basePropertyDescriptor.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            this.basePropertyDescriptor.ResetValue(component);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return this.basePropertyDescriptor.ShouldSerializeValue(component);
        }

        public override void SetValue(object component, object value)
        {
            this.basePropertyDescriptor.SetValue(component, value);
        }
    }
    #endregion


    #region 自定义属性用来显示左的边的汉字

    /// <summary>
    /// 自定义属性用来显示左的边的汉字
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    class showChineseForClinic : System.Attribute
    {
        private string sChineseChar = "";

        public showChineseForClinic(string sChineseChar)
        {
            this.sChineseChar = sChineseChar;
        }

        public string ChineseChar
        {
            get
            {
                return this.sChineseChar;
            }
        }

        public override string ToString()
        {
            return this.sChineseChar;
        }
    }
    #endregion

    #endregion

    #region 患者属性类

    /// <summary>
    /// 用于显示患者属性
    /// </summary> 
    class PatientInfoForClinic : IBasePropertyForClinic
    {
        #region 变量

        /// <summary>
        /// 患者门诊病例号
        /// </summary>
        private string Patientno = null;

        /// <summary>
        /// 患者姓名
        /// </summary>
        private string Patienname = null;

        /// <summary>
        /// 患者性别
        /// </summary>
        private string pSex = null;

        /// <summary>
        /// 患者年龄
        /// </summary>
        private string pAge = null;

        /// <summary>
        /// 挂号科室
        /// </summary>
        private string regDept = null;

        /// <summary>
        /// 挂号医生
        /// </summary>
        private string regDoct = null;

        /// <summary>
        /// 挂号级别
        /// </summary>
        private string regLevel = null;

        /// <summary>
        /// 合同单位
        /// </summary>
        private string pactName = null;

        /// <summary>
        /// 看诊时间
        /// </summary>
        private DateTime seeDate;

        /// <summary>
        /// 看诊医生
        /// </summary>
        private string seeDoct = null;

        /// <summary>
        /// 看诊科室
        /// </summary>
        private string seeDept = null;

        /// <summary>
        /// 挂号时间
        /// </summary>
        private string regDate = null;

        /// <summary>
        /// 身份证号
        /// </summary>
        private string idenNo = null;

        /// <summary>
        /// 联系电话
        /// </summary>
        private string relaPhone = null;

        /// <summary>
        /// 地址
        /// </summary>
        private string address = null;

        #endregion

        #region 基本信息

        /// <summary>
        /// 患者门诊病例号
        /// </summary>
        [DescriptionAttribute("患者门诊病例号。"), showChineseForClinic("A.门诊病例号"), CategoryAttribute("1.患者基本信息"), ReadOnlyAttribute(false)]
        public string PatientNo
        {
            get { return Patientno; }
            set { Patientno = value; }
        }

        /// <summary>
        /// 患者姓名
        /// </summary>
        [DescriptionAttribute("患者姓名。"), showChineseForClinic("B.姓名"), CategoryAttribute("1.患者基本信息"), ReadOnlyAttribute(false)]
        public string PatientName
        {
            get { return Patienname; }
            set { Patienname = value; }
        }

        /// <summary>
        /// 患者性别
        /// </summary>
        [DescriptionAttribute("患者性别。"), showChineseForClinic("C.性别"), CategoryAttribute("1.患者基本信息"), ReadOnlyAttribute(false)]
        public string Sex
        {
            get { return pSex; }
            set { pSex = value; }
        }

        /// <summary>
        /// 患者年龄
        /// </summary>
        [DescriptionAttribute("患者年龄。"), showChineseForClinic("D.年龄"), CategoryAttribute("1.患者基本信息"), ReadOnlyAttribute(false)]
        public string Age
        {
            get
            {
                return pAge;
            }
            set
            {
                pAge = value;
            }
        }

        /// <summary>
        /// 身份证号
        /// </summary>
        [DescriptionAttribute("身份证号。"), showChineseForClinic("E.身份证号"), CategoryAttribute("1.患者基本信息"), ReadOnlyAttribute(false)]
        public string IdenNo
        {
            get { return idenNo; }
            set { idenNo = value; }
        }

        /// <summary>
        /// 地址
        /// </summary>
        [DescriptionAttribute("联系地址。"), showChineseForClinic("F.地址"), CategoryAttribute("1.患者基本信息"), ReadOnlyAttribute(false)]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DescriptionAttribute("联系电话。"), showChineseForClinic("G.联系电话"), CategoryAttribute("1.患者基本信息"), ReadOnlyAttribute(false)]
        public string RelaPhone
        {
            get { return relaPhone; }
            set { relaPhone = value; }
        }

        #endregion

        #region 挂号信息

        /// <summary>
        /// 挂号时间
        /// </summary>
        [DescriptionAttribute("挂号时间。"), showChineseForClinic("H.挂号时间"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public string RegDate
        {
            get
            {
                return regDate;
            }
            set
            {
                regDate = value;
            }
        }

        /// <summary>
        /// 合同单位
        /// </summary>
        [DescriptionAttribute("合同单位。"), showChineseForClinic("I.合同单位"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public string PactName
        {
            get { return pactName; }
            set { pactName = value; }
        }

        /// <summary>
        /// 挂号级别
        /// </summary>
        [DescriptionAttribute("挂号级别。"), showChineseForClinic("J.挂号级别"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public string RegLevel
        {
            get { return regLevel; }
            set { regLevel = value; }
        }

        /// <summary>
        /// 挂号科室
        /// </summary>
        [DescriptionAttribute("挂号科室。"), showChineseForClinic("K.挂号科室"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public string RegDept
        {
            get { return regDept; }
            set { regDept = value; }
        }

        /// <summary>
        /// 挂号医生
        /// </summary>
        [DescriptionAttribute("挂号医生。"), showChineseForClinic("L.挂号医生"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public string RegDoct
        {
            get { return regDoct; }
            set { regDoct = value; }
        }

        /// <summary>
        /// 看诊时间
        /// </summary>
        [DescriptionAttribute("看诊时间。"), showChineseForClinic("M.看诊时间"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public DateTime SeeDate
        {
            get { return seeDate; }
            set { seeDate = value; }
        }

        /// <summary>
        /// 首诊医生
        /// </summary>
        [DescriptionAttribute("首诊医生。"), showChineseForClinic("N.首诊医生"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public string SeeDoct
        {
            get { return seeDoct; }
            set { seeDoct = value; }
        }

        /// <summary>
        /// 看诊科室
        /// </summary>
        [DescriptionAttribute("看诊科室。"), showChineseForClinic("O.看诊科室"), CategoryAttribute("2.挂号信息"), ReadOnlyAttribute(false)]
        public string SeeDept
        {
            get { return seeDept; }
            set { seeDept = value; }
        }
        #endregion
    }
    #endregion
}
