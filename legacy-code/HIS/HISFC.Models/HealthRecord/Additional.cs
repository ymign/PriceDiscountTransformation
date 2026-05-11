using System;
using Neusoft.FrameWork.Models;


namespace Neusoft.HISFC.Models.HealthRecord
{
    /// <summary>
    /// 中大五院病案首页附页
    /// </summary>
    public class Additional 
    {
        public Additional()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
      
        #region 私有变量
        /// <summary>
        /// 患者流水号 
        /// </summary>
        private string inpatient_no = string.Empty;
        /// <summary>
        /// Ⅰ类手术切口预防性应用抗菌药物 
        /// </summary>
         private string preventivedrug = string.Empty;
         /// <summary>
         /// 首选一、二代头孢
         /// </summary>
       private string generaldrug = string.Empty;
       /// <summary>
       /// 用药时机 
       /// </summary>
       private string medicationtime = string.Empty;
       /// <summary>
       /// 联合用药 
       /// </summary>
       private string drugcombination = string.Empty;
       /// <summary>
       /// 术中追加抗菌药 
       /// </summary>
       private string drugadditional = string.Empty;
       /// <summary>
       /// 术后停用抗菌药物时间 
       /// </summary>
       private string drugstoptime = string.Empty;
       /// <summary>
       /// 临床路径病种 
       /// </summary>
       private string pathway = string.Empty;
       /// <summary>
       /// 进入临床路径 
       /// </summary>
      private string  inpathway = string.Empty;
      /// <summary>
      /// 完成临床路径 
      /// </summary>
       private string isendpathway = string.Empty;
       /// <summary>
       /// 纳入特定（单）病种控制 
       /// </summary>
      private string  isdrgs = string.Empty;
      /// <summary>
      /// 上报特定（单）病种指标 
      /// </summary>
      private string  uploaddrgs = string.Empty;
      /// <summary>
      /// 出院与入院
      /// </summary>
      private string  inandout = string.Empty;
      /// <summary>
      /// 恶性肿瘤术前诊断与术后病理诊断
      /// </summary>
      private string  pathology = string.Empty;
      /// <summary>
      /// 放射与病理
      /// </summary>
      private string  pacsandpat = string.Empty;
      /// <summary>
      /// 进行手术冰冻与石蜡病理检查
      /// </summary>
      private string  isoptical = string.Empty;
      /// <summary>
      /// 手术冰冻与石蜡病理检查符合 
      /// </summary>
      private string  opticaltrue = string.Empty;
      /// <summary>
      /// 同一疾病(主要诊断)出院后非计划再住院
      /// </summary>
      private string  notplanback = string.Empty;
      /// <summary>
      /// 非预期再次手术
      /// </summary>
      private string  notplanops = string.Empty;
      /// <summary>
      /// 非预期再次手术死亡
      /// </summary>
      private string  notplanopsdead = string.Empty;
      /// <summary>
      /// 非预期重返ICU
      /// </summary>
      private string  notplanicuback = string.Empty;
      /// <summary>
      /// 入ICU患者APACHEⅡ评分
      /// </summary>
      private string  apache = string.Empty;
      /// <summary>
      /// 择期术后并发症
      /// </summary>
      private string  opscomplication = string.Empty;
      /// <summary>
      /// 并发症编码1
      /// </summary>
      private string  complicationcode1 = string.Empty;
      /// <summary>
      /// 并发症名称1
      /// </summary>
      private string  complicationname1 = string.Empty;
      /// <summary>
      /// 并发症编码2
      /// </summary>
      private string  complicationcode2 = string.Empty;
      /// <summary>
      /// 并发症名称2
      /// </summary>
      private string  complicationname2 = string.Empty;
      /// <summary>
      /// 并发症编码3
      /// </summary>
      private string  complicationcode3 = string.Empty;
      /// <summary>
      /// 并发症名称3
      /// </summary>
      private string  complicationname3 = string.Empty;
      /// <summary>
      /// 因术后并发症导致死亡
      /// </summary>
     private string   deathofcomp = string.Empty;
     /// <summary>
     /// 术后猝死
     /// </summary>
      private string  suddendeath = string.Empty;
      /// <summary>
      /// 手术过程中发生异物遗留
      /// </summary>
      private string  foreignbody = string.Empty;
      /// <summary>
      /// 择期手术患者围术期死亡
      /// </summary>
      private string  deathpenoperation = string.Empty;
      /// <summary>
      /// 发生医源性意外穿刺或撕裂伤
      /// </summary>
      private string  wound = string.Empty;
      /// <summary>
      /// 发生医源性气胸
      /// </summary>
      private string  pneumothorax = string.Empty;
      /// <summary>
      /// 输液
      /// </summary>
      private string  infusion = string.Empty;
      /// <summary>
      /// 输液反应
      /// </summary>
      private string  infusionreaction = string.Empty;
      /// <summary>
      /// 输血
      /// </summary>
      private string  bloodtrans = string.Empty;
      /// <summary>
      /// 输血反应
      /// </summary>
      private string  bloodtransreaction = string.Empty;
      /// <summary>
      /// 血液透析
      /// </summary>
      private string  hemodialysis = string.Empty;
      /// <summary>
      /// 发生与血液透析相关血液感染
      /// </summary>
      private string  heminfection = string.Empty;
      /// <summary>
      /// 产科新生儿情况：活产数
      /// </summary>
      private string  livebirths = string.Empty;
      /// <summary>
      /// 分娩方式
      /// </summary>
      private string  modeofdelivery = string.Empty;
      /// <summary>
      /// 产伤
      /// </summary>
      private string  birthinjury = string.Empty;
      /// <summary>
      /// 其他补充：（多胎情况）
      /// </summary>
      private string  morebaby = string.Empty;
      /// <summary>
      /// 产妇情况（经阴道分娩）：器械辅助阴道分娩
      /// </summary>
     private string   vaginaldelivery = string.Empty;
     /// <summary>
     /// 产科创伤
     /// </summary>
     private string   obstetrictrauma = string.Empty;
     /// <summary>
     /// 产后出血（500-1000ml）
     /// </summary>
     private string   postpartumbleeding = string.Empty;
     /// <summary>
     /// 产后严重出血（≥1000ml）
     /// </summary>
     private string   beriousbleeding = string.Empty;
     /// <summary>
     /// 备注
     /// </summary>
     private string   mark1 = string.Empty;
     /// <summary>
     /// 备注
     /// </summary>
     private string   mark2 = string.Empty;
     /// <summary>
     /// 备注
     /// </summary>
     private string   mark3 = string.Empty;
     /// <summary>
     /// 备注
     /// </summary>
      private string  mark4 = string.Empty;
      /// <summary>
      /// 备注
      /// </summary>
      private string mark5 = string.Empty;
      private string mark6 = string.Empty;
      private string mark7 = string.Empty;
      private string mark8 = string.Empty;
      private string mark9 = string.Empty;

      private string mark10 = string.Empty;
      private string mark11 = string.Empty;
      private string mark12 = string.Empty;
      private string mark13 = string.Empty;
      private string mark14 = string.Empty;
      private string mark15 = string.Empty;
      private string mark16 = string.Empty;
      private string mark17 = string.Empty;
      
        #endregion

        #region 属性

        /// <summary>
        /// 患者流水号
        /// </summary>
      public string Inpatient_No
      {
          get
          {
              return inpatient_no;
          }
          set
          {
              inpatient_no = value;
          }
      }
      /// <summary>
      /// Ⅰ类手术切口预防性应用抗菌药物 
      /// </summary>
      public string Preventivedrug
      {
          get
          {
              return preventivedrug;
          }
          set
          {
              preventivedrug = value;
          }
      }
      /// <summary>
      /// 首选一、二代头孢
      /// </summary>
      public string Generaldrug
      {
          get
          {
              return generaldrug;
          }
          set
          {
              generaldrug = value;
          }
      }
      /// <summary>
      /// 用药时机 
      /// </summary>
      public string Medicationtime
      {
          get
          {
              return medicationtime;
          }
          set
          {
              medicationtime = value;
          }
      }
      /// <summary>
      /// 联合用药 
      /// </summary>
      public string Drugcombination
      {
          get
          {
              return drugcombination;
          }
          set
          {
              drugcombination = value;
          }
      }
      /// <summary>
      /// 术中追加抗菌药 
      /// </summary>
      public string Drugadditional
      {
          get
          {
              return drugadditional;
          }
          set
          {
              drugadditional = value;
          }
      }
      /// <summary>
      /// 术后停用抗菌药物时间 
      /// </summary>
      public string Drugstoptime
      {
          get
          {
              return drugstoptime;
          }
          set
          {
              drugstoptime = value;
          }
      }
      /// <summary>
      /// 临床路径病种 
      /// </summary>
      public string Pathway
      {
          get
          {
              return pathway;
          }
          set
          {
              pathway = value;
          }
      }
      /// <summary>
      /// 进入临床路径 
      /// </summary>
      public string Inpathway
      {
          get
          {
              return inpathway;
          }
          set
          {
              inpathway = value;
          }
      }
      /// <summary>
      /// 完成临床路径 
      /// </summary>
      public string Isendpathway
      {
          get
          {
              return isendpathway;
          }
          set
          {
              isendpathway = value;
          }
      }
      /// <summary>
      /// 纳入特定（单）病种控制 
      /// </summary>
      public string Isdrgs
      {
          get
          {
              return isdrgs;
          }
          set
          {
              isdrgs = value;
          }
      }
      /// <summary>
      /// 上报特定（单）病种指标 
      /// </summary>
      public string Uploaddrgs
      {
          get
          {
              return uploaddrgs;
          }
          set
          {
              uploaddrgs = value;
          }
      }
      /// <summary>
      /// 出院与入院
      /// </summary>
      public string Inandout
      {
          get
          {
              return inandout;
          }
          set
          {
              inandout = value;
          }
      }
      /// <summary>
      /// 恶性肿瘤术前诊断与术后病理诊断
      /// </summary>
      public string Pathology
      {
          get
          {
              return pathology;
          }
          set
          {
              pathology = value;
          }
      }
      /// <summary>
      /// 放射与病理
      /// </summary>
      public string Pacsandpat
      {
          get
          {
              return pacsandpat;
          }
          set
          {
              pacsandpat = value;
          }
      }
      /// <summary>
      /// 进行手术冰冻与石蜡病理检查
      /// </summary>
      public string Isoptical
      {
          get
          {
              return isoptical;
          }
          set
          {
              isoptical = value;
          }
      }
      /// <summary>
      /// 手术冰冻与石蜡病理检查符合 
      /// </summary>
      public string Opticaltrue
      {
          get
          {
              return opticaltrue;
          }
          set
          {
              opticaltrue = value;
          }
      }
      /// <summary>
      /// 同一疾病(主要诊断)出院后非计划再住院
      /// </summary>
      public string Notplanback
      {
          get
          {
              return notplanback;
          }
          set
          {
              notplanback = value;
          }
      }
      /// <summary>
      /// 非预期再次手术
      /// </summary>
      public string Notplanops
      {
          get
          {
              return notplanops;
          }
          set
          {
              notplanops = value;
          }
      }
      /// <summary>
      /// 非预期再次手术死亡
      /// </summary>
      public string Notplanopsdead
      {
          get
          {
              return notplanopsdead;
          }
          set
          {
              notplanopsdead = value;
          }
      }
      /// <summary>
      /// 非预期重返ICU
      /// </summary>
      public string Notplanicuback
      {
          get
          {
              return notplanicuback;
          }
          set
          {
              notplanicuback = value;
          }
      }
      /// <summary>
      /// 入ICU患者APACHEⅡ评分
      /// </summary>
      public string Apache
      {
          get
          {
              return apache;
          }
          set
          {
              apache = value;
          }
      }
      /// <summary>
      /// 择期术后并发症
      /// </summary>
      public string Opscomplication
      {
          get
          {
              return opscomplication;
          }
          set
          {
              opscomplication = value;
          }
      }
      /// <summary>
      /// 并发症编码1
      /// </summary>
      public string Complicationcode1
      {
          get
          {
              return complicationcode1;
          }
          set
          {
              complicationcode1 = value;
          }
      }
      /// <summary>
      /// 并发症名称1
      /// </summary>
      public string Complicationname1
      {
          get
          {
              return complicationname1;
          }
          set
          {
              complicationname1 = value;
          }
      }
      /// <summary>
      /// 并发症编码2
      /// </summary>
      public string Complicationcode2
      {
          get
          {
              return complicationcode2;
          }
          set
          {
              complicationcode2 = value;
          }
      }
      /// <summary>
      /// 并发症名称2
      /// </summary>
      public string Complicationname2
      {
          get
          {
              return complicationname2;
          }
          set
          {
              complicationname2 = value;
          }
      }
      /// <summary>
      /// 并发症编码3
      /// </summary>
      public string Complicationcode3
      {
          get
          {
              return complicationcode3;
          }
          set
          {
              complicationcode3 = value;
          }
      }
      /// <summary>
      /// 并发症名称3
      /// </summary>
      public string Complicationname3
      {
          get
          {
              return complicationname3;
          }
          set
          {
              complicationname3 = value;
          }
      }
      /// <summary>
      /// 因术后并发症导致死亡
      /// </summary>
      public string Deathofcomp
      {
          get
          {
              return deathofcomp;
          }
          set
          {
              deathofcomp = value;
          }
      }
      /// <summary>
      /// 术后猝死
      /// </summary>
      public string Suddendeath
      {
          get
          {
              return suddendeath;
          }
          set
          {
              suddendeath = value;
          }
      }
      /// <summary>
      /// 手术过程中发生异物遗留
      /// </summary>
      public string Foreignbody
      {
          get
          {
              return foreignbody;
          }
          set
          {
              foreignbody = value;
          }
      }
      /// <summary>
      /// 择期手术患者围术期死亡
      /// </summary>
      public string Deathpenoperation
      {
          get
          {
              return deathpenoperation;
          }
          set
          {
              deathpenoperation = value;
          }
      }
      /// <summary>
      /// 发生医源性意外穿刺或撕裂伤
      /// </summary>
      public string Wound
      {
          get
          {
              return wound;
          }
          set
          {
              wound = value;
          }
      }
      /// <summary>
      /// 发生医源性气胸
      /// </summary>
      public string Pneumothorax
      {
          get
          {
              return pneumothorax;
          }
          set
          {
              pneumothorax = value;
          }
      }
      /// <summary>
      /// 输液
      /// </summary>
      public string Infusion
      {
          get
          {
              return infusion;
          }
          set
          {
              infusion = value;
          }
      }
      /// <summary>
      /// 输液反应
      /// </summary>
      public string Infusionreaction
      {
          get
          {
              return infusionreaction;
          }
          set
          {
              infusionreaction = value;
          }
      }
      /// <summary>
      /// 输血
      /// </summary>
      public string Bloodtrans
      {
          get
          {
              return bloodtrans;
          }
          set
          {
              bloodtrans = value;
          }
      }
      /// <summary>
      /// 输血反应
      /// </summary>
      public string Bloodtransreaction
      {
          get
          {
              return bloodtransreaction;
          }
          set
          {
              bloodtransreaction = value;
          }
      }
      /// <summary>
      /// 血液透析
      /// </summary>
      public string Hemodialysis
      {
          get
          {
              return hemodialysis;
          }
          set
          {
              hemodialysis = value;
          }
      }
      /// <summary>
      /// 发生与血液透析相关血液感染
      /// </summary>
      public string Heminfection
      {
          get
          {
              return heminfection;
          }
          set
          {
              heminfection = value;
          }
      }
      /// <summary>
      /// 产科新生儿情况：活产数
      /// </summary>
      public string Livebirths
      {
          get
          {
              return livebirths;
          }
          set
          {
              livebirths = value;
          }
      }
      /// <summary>
      /// 出生方式1剖宫产  2非器械辅助阴道分娩  3器械辅助阴道分娩
      /// </summary>
      public string Modeofdelivery
      {
          get
          {
              return modeofdelivery;
          }
          set
          {
              modeofdelivery = value;
          }
      }
      /// <summary>
      /// 新生儿产伤1否2是
      /// </summary>
      public string Birthinjury
      {
          get
          {
              return birthinjury;
          }
          set
          {
              birthinjury = value;
          }
      }
      /// <summary>
      /// 新生儿产伤ICD编码
      /// </summary>
      public string Morebaby
      {
          get
          {
              return morebaby;
          }
          set
          {
              morebaby = value;
          }
      }
      /// <summary>
      /// 产妇产妇情况本次住院分娩1否2是
      /// </summary>
      public string Vaginaldelivery
      {
          get
          {
              return vaginaldelivery;
          }
          set
          {
              vaginaldelivery = value;
          }
      }
      /// <summary>
      /// 产科创伤 1否  2是，Ⅲ度  3是，Ⅳ度 
      /// </summary>
      public string Obstetrictrauma
      {
          get
          {
              return obstetrictrauma;
          }
          set
          {
              obstetrictrauma = value;
          }
      }
      /// <summary>
      /// 阴道分娩产后出血1否2是
      /// </summary>
      public string Postpartumbleeding
      {
          get
          {
              return postpartumbleeding;
          }
          set
          {
              postpartumbleeding = value;
          }
      }
      /// <summary>
      /// 剖宫产产后出血1否2是
      /// </summary>
      public string Beriousbleeding
      {
          get
          {
              return beriousbleeding;
          }
          set
          {
              beriousbleeding = value;
          }
      }
      /// <summary>
      /// 出院与门诊1.符合 2.不符合 3.不肯定 4.未做 
      /// </summary>
      public string Mark1
      {
          get
          {
              return mark1;
          }
          set
          {
              mark1 = value;
          }
      }
      /// <summary>
      /// 临床与病理1.符合 2.不符合 3.不肯定 4.未做 
      /// </summary>
      public string Mark2
      {
          get
          {
              return mark2;
          }
          set
          {
              mark2 = value;
          }
      }
      /// <summary>
      /// 异物遗留ICD编码
      /// </summary>
      public string Mark3
      {
          get
          {
              return mark3;
          }
          set
          {
              mark3 = value;
          }
      }
      /// <summary>
      /// 医源性意外伤ICD编码
      /// </summary>
      public string Mark4
      {
          get
          {
              return mark4;
          }
          set
          {
              mark4 = value;
          }
      }
      /// <summary>
      /// 出生方式2 1剖宫产  2非器械辅助阴道分娩  3器械辅助阴道分娩
      /// </summary>
      public string Mark5
      {
          get
          {
              return mark5;
          }
          set
          {
              mark5 = value;
          }
      }
      /// <summary>
      /// 新生儿产伤2 1否2是
      /// </summary>
      public string Mark6
      {
          get
          {
              return mark6;
          }
          set
          {
              mark6 = value;
          }
      }
      /// <summary>
      /// 新生儿产伤ICD编码2
      /// </summary>
      public string Mark7
      {
          get
          {
              return mark7;
          }
          set
          {
              mark7 = value;
          }
      }
      /// <summary>
      /// 分娩方式1剖宫产  2非器械辅助阴道分娩  3器械辅助阴道分娩 4其他
      /// </summary>
      public string Mark8
      {
          get
          {
              return mark8;
          }
          set
          {
              mark8 = value;
          }
      }
      /// <summary>
      /// 备注
      /// </summary>
      public string Mark9
      {
          get
          {
              return mark9;
          }
          set
          {
              mark9 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 是否为初诊：□ 1.是 2.否
      /// </summary>
      public string Mark10
      {
          get
          {
              return mark10;
          }
          set
          {
              mark10 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 恶性肿瘤名称
      /// </summary>
      public string Mark11
      {
          get
          {
              return mark11;
          }
          set
          {
              mark11 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 拟治疗方式1. 手术或介入  2. 化疗   3.放疗    4.其他
      /// </summary>
      public string Mark12
      {
          get
          {
              return mark12;
          }
          set
          {
              mark12 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 拟治疗方式选了其他填写的内容
      /// </summary>
      public string Mark13
      {
          get
          {
              return mark13;
          }
          set
          {
              mark13 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 临床TNM分期评估  T：0/1/2/3/4/X
      /// </summary>
      public string Mark14
      {
          get
          {
              return mark14;
          }
          set
          {
              mark14 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 临床TNM分期评估  N：0/1/2/3/X
      /// </summary>
      public string Mark15
      {
          get
          {
              return mark15;
          }
          set
          {
              mark15 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 临床TNM分期评估  M：0/1/X
      /// </summary>
      public string Mark16
      {
          get
          {
              return mark16;
          }
          set
          {
              mark16 = value;
          }
      }

      /// <summary>
      /// 肿瘤治疗前临床TNM分期评估 分期 0/I/IA/IB/IC/II/IIA/IIB/IIC/III/IIIA/IIIB/IIIC/IV/IVA/IVB/IVC
      /// </summary>
      public string Mark17
      {
          get
          {
              return mark17;
          }
          set
          {
              mark17 = value;
          }
      }
        #endregion
    }
}
