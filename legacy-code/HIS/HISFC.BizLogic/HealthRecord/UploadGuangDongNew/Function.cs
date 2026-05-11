using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew
{
    public class Function : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 字典业务实体
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Constant constMana = new Neusoft.HISFC.BizLogic.Manager.Constant();

        /// <summary>
        /// 科室业务实体
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Department deptMana = new Neusoft.HISFC.BizLogic.Manager.Department();

        /// <summary>
        /// 医生业务
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Person docMana = new Neusoft.HISFC.BizLogic.Manager.Person();

        Neusoft.HISFC.BizLogic.HealthRecord.Base baseDml = new Neusoft.HISFC.BizLogic.HealthRecord.Base();

        private static Neusoft.HISFC.BizLogic.Manager.Person personMgr = new Neusoft.HISFC.BizLogic.Manager.Person();

        private SQLServerManager server = null;

        public SQLServerManager Server
        {
            get { return server; }
            set { server = value; }
        }

        int fid = 0;

        #region  字典
        /// <summary>
        /// 科室字典
        /// </summary>
        Dictionary<string, Neusoft.HISFC.Models.Base.Const> deptdic = new Dictionary<string, Neusoft.HISFC.Models.Base.Const>();

        /// <summary>
        /// 医生字典
        /// </summary>
        Dictionary<string, Neusoft.HISFC.Models.Base.Const> doctordic = new Dictionary<string, Neusoft.HISFC.Models.Base.Const>();

        /// <summary>
        /// 入院病情
        /// </summary>
        Dictionary<string, Neusoft.HISFC.Models.Base.Const> diagrybq = new Dictionary<string, Neusoft.HISFC.Models.Base.Const>();
        #endregion

        private string erro = "";

        public string Erro
        {
            get { return erro; }
            set { erro = value; }
        }


        #region 获取字典
        //FTYPE 性质，1：门诊，2：住院，3：急诊，4：观察室，5：医技科室
        //FTYKH 统一科号，内编码，关联使用，用户不可见，一般情况下不能修改，只有涉及到院区合并时由DBA统一改动。
       //FType=1时默认值为'TMZ' + Fkh的值
       //FType=2时默认值为'TZY' + Fkh的值
       //FType=3时默认值为'TJZ' + Fkh的值

        //FKH 科号，用户录入
        //FKSNAME  科室名称

        /// <summary>
        /// 下载科室字典(参数:CASEDEPT)
        /// </summary>
        /// <returns></returns>
        public int DownDeptDiction()
        {
            if (this.server == null)
            {
                server = new SQLServerManager();
            }
            DataSet ds = new DataSet();
            string sql = @"
                        select 
                        s.fksname,
                        s.fhiskh,
                        s.ftykh
                        from tworkroom t,thisksset s
                        where t.fnoused='0'
                        and t.ftykh=s.ftykh
                        and s.fkstype='0'
                        order by s.fkstype, s.fhiskh
                        ";
            int result = server.Execute(sql, ref ds);
            if (result < 0)
            {
                //server.Rollback();
                this.Erro = "下载科室字典失败!";
                return -1;
            }
            //server.Commit();
            List<string> deptcoloectstr = new List<string>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string deptcollect = dr[1].ToString();
                if (string.IsNullOrEmpty(deptcollect))
                {
                    continue;
                }
                string[] dept = deptcollect.Split(',');
                string deptnamecollect = "";
                string deptidcollect = "";
                for (int i = 0; i < dept.Length; i++)
                {
                    Neusoft.HISFC.Models.Base.Department locoldept = deptMana.GetDeptmentById(dept[i]);
                    if (locoldept==null || string.IsNullOrEmpty(locoldept.ID))
                    {
                        continue;
                    }
                    if (deptcoloectstr.Contains(locoldept.ID))
                    {
                        continue;
                    }
                    deptnamecollect += locoldept.Name + ",";
                    deptidcollect += locoldept.ID + ",";
                    deptcoloectstr.Add(locoldept.ID);
                    
                }
                if (string.IsNullOrEmpty(deptnamecollect))
                {
                    continue;
                }
                //deptMana.GetDeptmentById();
                Neusoft.HISFC.Models.Base.Const newcon = new Neusoft.HISFC.Models.Base.Const();
                newcon.ID = dr[2].ToString();
                newcon.Name = dr[0].ToString();
                newcon.Memo = deptnamecollect.Substring(0,deptnamecollect.Length - 1);
                newcon.UserCode = deptidcollect.Substring(0, deptidcollect.Length - 1);
                newcon.IsValid = true;
                int update = constMana.UpdateItem("CASEDEPT", newcon);
                if (update <= 0)
                {
                    int insert = constMana.InsertItem("CASEDEPT", newcon);//只管插入
                    if (insert <= 0)
                    {
                        return -1;
                    }

                }

            }
            return 1;

        }

        /// <summary>
        /// 保存医生信息
        /// </summary>
        /// <returns></returns>
        public int SaveDoctorInfo()
        {
            //医生对照信息不用事务,数据都来自服务器172.16.60.84  tdoctor表
            if (doctordic == null || doctordic.Count == 0)
            {
                return 1;
            }
            foreach (KeyValuePair<string, Neusoft.HISFC.Models.Base.Const> doc in doctordic)
            {
                Neusoft.HISFC.Models.Base.Const docobj = doc.Value as Neusoft.HISFC.Models.Base.Const;
                docobj.IsValid = true;
                int update = constMana.UpdateItem("CASEDOCTOR", docobj);
                if (update <= 0)
                {
                    int insert = constMana.InsertItem("CASEDOCTOR", docobj);
                    if (insert <= 0)
                    {
                        return -1;
                    }

                }
                
            }
            return 1;
        }

        #endregion

        #region HIS_BA1 INSERT SQL
        /// <summary>
        /// 获得接口HIS_BA1 INSERT SQL
        /// </summary>
        /// <param name="b">病案首页的实体类</param>
        /// <param name="Feeds">费用信息数组</param>
        /// <param name="alChangeDepe">转科信息数组</param>
        /// <param name="alDose">诊断信息数组</param>
        /// <param name="isMetCasBase">true病案首页信息 false 住院主表信息</param>
        /// <returns></returns>
        public string GetInsertHISBA1SQLDrgs(Neusoft.HISFC.Models.HealthRecord.Base b, DataSet Feeds,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            if (b == null)
            {
                Err += "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            #region sql
            //sql 太长直接使用原来的
            strReturn = @"INSERT INTO TPATIENTvisit
  (
   --Fifinput,
   FPRN,
   FTIMES,
   FICDVersion,
   FZYID,
   FAGE,
   FNAME,
   FSEXBH,
   FSEX,
   FBIRTHDAY,
   FBIRTHPLACE, --10
   FIDCard,
   fcountrybh,
   fcountry,
   fnationalitybh,
   fnationality,
   FJOB,
   FSTATUSBH,
   FSTATUS,
   FDWNAME,
   FDWADDR,--20
   FDWTELE,
   FDWPOST,
   FHKADDR,
   FHKPOST,
   FLXNAME,
   FRELATE,
   FLXADDR,
   FLXTELE,
   FASCARD1,
   FRYDATE,--30
   FRYTIME,
   FRYTYKH,
   FRYDEPT,
   FRYBS,
   FCYDATE,
   FCYTIME,
   FCYTYKH,
   FCYDEPT,
   FCYBS,
   FDAYS,--40
   FMZZDBH,
   FMZZD,
   FMZDOCTBH,
   FMZDOCT,
   FPHZD,
   FGMYW,
   FmzCYACCOBH,
   FmzCYACCO,
   FLCBLACCOBH,
   FLCBLACCO,--50
   FQJTIMES,
   FQJSUCTIMES,
   FKZRBH,
   FKZR,
   FZRDOCTBH,
   FZRDOCTOR,
   FZZDOCTBH,
   FZZDOCT,
   FZYDOCTBH,
   FZYDOCT,--60
   FJXDOCTBH,
   FJXDOCT,
   FSXDOCTBH,
   FSXDOCT,
   FBMYBH,
   FBMY,
   FZLRBH,
   FZLR,
   FQUALITYBH,
   FQUALITY,--70
   FZKDOCTBH,
   FZKDOCT,
   FZKNURSEBH,
   FZKNURSE,
   FZKRQ,
FSUM1,
FXYF,
FZYF,
FZCHYF,
FZCYF,--80
FQTF,
   FBODYBH,
   FBODY,
   FBLOODBH,
   FBLOOD,
   FRHBH,
   FRH,
   FBABYNUM,
   FTWILL,
   FZKTYKH,--90
   FZKDEPT,
   FZKDATE,
   FZKTIME,
   FSRYBH,
   FSRY,
   FWORKRQ,
   FJBFXBH,
   FJBFX,
   FFHGDBH,
   FFHGD,--100
   FSOURCEBH,
   FSOURCE,
   FIFSS,
   FIFFYK,
   FYNGR,
   FEXTEND1,
   FEXTEND2,
   FEXTEND3,
   FEXTEND4,
   FEXTEND5,--110
   FEXTEND6,
   FEXTEND7,
   FEXTEND8,
   FEXTEND9,
   FEXTEND10,
   FEXTEND11,
   FEXTEND12,
   FEXTEND13,
   FEXTEND14,
   FEXTEND15,--120
    FNATIVE,
    FCURRADDR,
    FCURRTELE,
    FCURRPOST,
    FJOBBH,
    FCSTZ,
    FRYTZ,
    FRYTJBH,
    FRYTJ,
    FYCLJBH,--130
    FYCLJ,
    FPHZDBH,
    FPHZDNUM,
    FIFGMYWBH,
    FIFGMYW,
    FNURSEBH,
    FNURSE,
    FLYFSBH,
    FLYFS,
    FYZOUTHOSTITAL,--140
    FSQOUTHOSTITAL,
    FISAGAINRYBH,
    FISAGAINRY,
    FISAGAINRYMD,
    FRYQHMDAYS,
    FRYQHMHOURS,
    FRYQHMMINS,
    FRYQHMCOUNTS,
    FRYHMDAYS,
    FRYHMHOURS,--150
    FRYHMMINS,
    FRYHMCOUNTS,
    FFBBHNEW,
    FFBNEW,
FZFJE,
FZHFWLYLF,
FZHFWLCZF,
FZHFWLHLF,
FZHFWLQTF,
FZDLBLF,--160
FZDLSSSF,
FZDLYXF,
FZDLLCF,
FZLLFFSSF,
FZLLFWLZWLF,
FZLLFSSF,
FZLLFMZF,
FZLLFSSZLF,
FKFLKFF,
FZYLZF,--170
FXYLGJF,
FXYLXF,
FXYLBQBF,
FXYLQDBF,
FXYLYXYZF,
FXYLXBYZF,
FHCLCJF,
FHCLZLF,
FHCLSSF,
FZHFWLYLF01,--180
FZHFWLYLF02,
FZYLZDF,
FZYLZLF,
FZYLZLF01,
FZYLZLF02,
FZYLZLF03,
FZYLZLF04,
FZYLZLF05,
FZYLZLF06,
FZYLQTF,--190
FZYLQTF01,
FZYLQTF02,
FZCLJGZJF --193
)
  VALUES
  (
'{1}',
 {2},
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}',
'{28}',
'{29}',
'{30}',
'{31}',
'{32}',
'{33}',
'{34}',
'{35}',
'{36}',
'{37}',
'{38}',
'{39}',
 {40},
'{41}',
'{42}',
'{43}',
'{44}',
'{45}',
'{46}',
'{47}',
'{48}',
'{49}',
'{50}',
 {51},
 {52},
'{53}',
'{54}',
'{55}',
'{56}',
'{57}',
'{58}',
'{59}',
'{60}',
'{61}',
'{62}',
'{63}',
'{64}',
'{65}',
'{66}',
'{67}',
'{68}',
'{69}',
'{70}',
'{71}',
'{72}',
'{73}',
'{74}',
'{75}',
 {76},
 {77},
 {78},
 {79},
 {80},
 {81},
'{82}',
'{83}',
'{84}',
'{85}',
'{86}',
'{87}',
 {88},
'{89}',
'{90}',
'{91}',
'{92}',
'{93}',
'{94}',
'{95}',
'{96}',
'{97}',
'{98}',
'{99}',
'{100}',
'{101}',
'{102}',
'{103}',
'{104}',
 {105},
'{106}',
'{107}',
'{108}',
'{109}',
'{110}',
'{111}',
'{112}',
'{113}',
'{114}',
'{115}',
'{116}',
'{117}',
'{118}',
'{119}',
'{120}',
'{121}',
'{122}',
'{123}',
'{124}',
'{125}',
 {126},
 {127},
'{128}',
'{129}',
'{130}',
'{131}',
'{132}',
'{133}',
'{134}',
'{135}',
'{136}',
'{137}',
'{138}',
'{139}',
'{140}',
'{141}',
'{142}',
'{143}',
'{144}',
{145},
{146},
{147},
{148},
{149},
{150},
{151},
{152},
'{153}',
'{154}',
{155},
{156},
{157},
{158},
{159},
{160},
{161},
{162},
{163},
{164},
{165},
{166},
{167},
{168},
{169},
{170},
{171},
{172},
{173},
{174},
{175},
{176},
{177},
{178},
{179},
{180},
{181},
{182},
{183},
{184},
{185},
{186},
{187},
{188},
{189},
{190},
{191},
{192},
{193}
)";
            #endregion

            try
            {

                strReturn = string.Format(strReturn, this.GetBaseInfoBA1Drgs(b, Feeds, alChangeDepe, alDose, isMetCasBase));
            }
            catch (Exception ex)
            {
                this.Err += "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        /// <summary>
        /// 接口HIS_BA1 INSERT SQL 赋值
        /// </summary>
        /// <param name="b">病案首页的实体类</param>
        /// <param name="Feeds">费用信息数组</param>
        /// <param name="alChangeDepe">转科信息数组</param>
        /// <param name="alDose">诊断信息数组</param>
        /// <param name="isMetCasBase">true病案首页信息 false 住院主表信息</param>
        /// <returns>失败返回null</returns>
        private string[] GetBaseInfoBA1Drgs(Neusoft.HISFC.Models.HealthRecord.Base b, DataSet Feeds,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            //ArrayList al = this.baseDml.QueryHealthRecordCaseinfo(b.PatientInfo.ID);
            if (isMetCasBase)
            {
                #region
                string[] s = new string[194];
                try
                {
                    s[0] = "0";//是否输入，0：否，1：是，默认接收为0
                    string patientNO = b.PatientInfo.PID.PatientNO.PadLeft(10, '0');
                    s[1] = this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr()));//病案号
                    s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
                    s[3] = "11";//ICD版本，9：ICD9字典库，10：ICD10字典库，待扩展11：国家ICD库，默认接收为11
                    s[4] = b.PatientInfo.ID;//住院流水号
                    #region //s[5] 年龄
                    //if (b.PatientInfo.Age != "" && b.PatientInfo.Age != "0")
                    //{
                    //    if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") < 0) //整岁
                    //    {
                    //        s[5] = "Y" + b.AgeUnit.Replace("岁", "");
                    //    }
                    //    else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//整月
                    //    {
                    //        s[5] = "M" + b.AgeUnit.Replace("月", "");
                    //    }
                    //    else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") < 0 && b.AgeUnit.IndexOf("天") > 0)//整天
                    //    {
                    //        s[5] = "D" + b.AgeUnit.Replace("天", "");
                    //    }
                    //    else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//N岁N月
                    //    {
                    //        string[] PAge = b.AgeUnit.Split('岁');
                    //        s[5] = "Y" + PAge[0] + "M" + PAge[1].Replace("岁", "").Replace("月", "");
                    //    }
                    //    else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N月N天
                    //    {
                    //        string[] PAge = b.AgeUnit.Split('月');
                    //        s[5] = "M" + PAge[0] + "D" + PAge[1].Replace("月", "").Replace("天", "");
                    //    }
                    //    else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N岁N月N天
                    //    {
                    //        string[] PAge = b.AgeUnit.Split('岁');

                    //        string[] PAge1 = PAge[1].Split('月');
                    //        s[5] = "Y" + PAge[0] + "M" + PAge1[0] + "D" + PAge1[1].Replace("月", "").Replace("天", "");
                    //    }
                    //}
                    //else
                    //{
                    //    //int ts = b.PatientInfo.PVisit.InTime.Year - b.PatientInfo.Birthday.Year;
                    //    //跨年的不足一年也算成了一年
                    //    int ty = b.PatientInfo.PVisit.InTime.Year - b.PatientInfo.Birthday.Year;
                    //    int tm = (b.PatientInfo.PVisit.InTime- b.PatientInfo.Birthday).Days;
                    //    int ts = tm / 365;

                    //    if (ts == 0)
                    //    {
                    //        ts = b.PatientInfo.PVisit.InTime.Month - b.PatientInfo.Birthday.Month;

                    //        if (ts == 0)
                    //        {
                    //            ts = b.PatientInfo.PVisit.InTime.Day - b.PatientInfo.Birthday.Day;
                    //            s[5] = "D" + ts.ToString();//年龄 
                    //        }
                    //        else
                    //        {
                    //            s[5] = "M" + ts.ToString();//年龄 
                    //        }
                    //    }
                    //    else
                    //    {
                    //        s[5] = "Y" + ty.ToString();//年龄 
                    //    }
                    //}
                    #endregion


                    #region //s[5] 年龄  
                    // {92350868-ECF3-4750-B8CB-D19798A7E796} 改为取AgeUnit字段
                    //s[5] = b.AgeUnit;
                    // {F4163568-BD1F-475e-BB0F-94DADC5C7402}  省病案年龄中存在空格符号报错
                    s[5] = b.AgeUnit.Trim();
                    #endregion

                    s[6] = b.PatientInfo.Name;//病人姓名
                    //性别编号
                    //性别
                    if (b.PatientInfo.Sex.ID.ToString() == "M" || b.PatientInfo.Sex.ID.ToString() == "1")
                    {
                        s[7] = "1";
                        s[8] = "男";
                    }
                    else
                    {
                        s[7] = "2";
                        s[8] = "女";
                    }
                    s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');//出生日期
                    s[10] = b.PatientInfo.AreaCode; //出生地
                    if (b.PatientInfo.IDCard == "" || b.PatientInfo.IDCard.Trim() == "-")
                    {
                        s[11] = "不详";
                    }
                    else
                    {
                        s[11] = b.PatientInfo.IDCard;//身份证号
                    }
                    #region s[12]国籍编号\ s[13]国籍  中国  需要转换其他
                    if (b.PatientInfo.Country.ID.ToString() == "1")
                    {
                        //s[12] = "A156";
                        s[12] = "CHN";
                        s[13] = "中国";
                    }
                    else
                    {
                        Neusoft.FrameWork.Models.NeuObject countryObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                        if (countryObj != null && countryObj.ID != "")
                        {
                            if (countryObj.Memo != "" && countryObj.Memo.ToUpper() != "TRUE")
                            {
                                s[12] = countryObj.Memo.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                            else
                            {
                                s[12] = countryObj.ID.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                        }
                        else
                        {
                            s[12] = b.PatientInfo.Country.ID.ToString();
                            s[13] = "";
                        }
                    }
                    #endregion
                    #region s[14]民族编号 s[15]民族
                    Neusoft.FrameWork.Models.NeuObject NationObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                    if (NationObj != null && NationObj.ID != "")
                    {
                        if (NationObj.Memo != "" && NationObj.Memo.ToUpper() != "TRUE")
                        {
                            s[14] = NationObj.Memo;
                            s[15] = NationObj.Name;
                        }
                        else
                        {
                            s[14] = NationObj.ID;
                            s[15] = NationObj.Name;
                        }
                    }
                    else
                    {
                        s[14] = b.PatientInfo.Nationality.ID;
                        s[15] = "";
                    }
                    #endregion
                    #region  s[16] 职业有中文传中文
                    //add by chengym 2011-6-15  字典表的名称字段varchar（100） 有些职业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                    Neusoft.FrameWork.Models.NeuObject JobObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CASEPROFESSION, b.PatientInfo.Profession.ID.ToString());
                    if (JobObj != null && JobObj.ID != "")
                    {
                        if (JobObj.Memo != "" && JobObj.Memo.ToUpper() != "TRUE")
                        {
                            if (JobObj.Memo.Length <= 50)
                            {
                                s[16] = JobObj.Memo;
                            }
                            else
                            {
                                s[16] = JobObj.Memo.Substring(0, 50);
                            }
                        }
                        else
                        {
                            if (JobObj.Name.Length <= 50)
                            {
                                s[16] = JobObj.Name;
                            }
                            else
                            {
                                s[16] = JobObj.Name.Substring(0, 50);
                            }
                        }
                    }
                    else
                    {
                        s[16] = "其他";//b.PatientInfo.Profession.ID.ToString(); //职业 没有传中文不知道是否可以
                    }
                    #endregion
                    #region s[17] 婚姻状况编号 s[18]婚姻状况
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "10")
                    {
                        s[17] = "10"; //婚姻状况编号
                        s[18] = "未婚"; //婚姻状况
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "20")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "3"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "30")
                    {
                        s[17] = "30";
                        s[18] = "丧偶";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "40")
                    {
                        s[17] = "40";
                        s[18] = "离婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "R")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else
                    {
                        s[17] = "90";
                        s[18] = "未说明的婚姻状况";
                    }
                    #endregion
                    s[19] = b.PatientInfo.AddressBusiness;  //单位名称
                    s[20] = b.PatientInfo.AddressBusiness;//单位地址 
                    s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                    if (b.PatientInfo.BusinessZip != null && b.PatientInfo.BusinessZip.Length == 6)
                    {
                        s[22] = b.PatientInfo.BusinessZip;//单位邮编 
                    }
                    else
                    {
                        s[22] = "";//单位邮编 
                    }
                    s[23] = b.PatientInfo.AddressHome;//户口地址
                    if (b.PatientInfo.HomeZip != null && b.PatientInfo.HomeZip.Length == 6)
                    {
                        s[24] = b.PatientInfo.HomeZip;//户口邮编
                    }
                    else
                    {
                        s[24] = "";
                    }
                    s[25] = b.PatientInfo.Kin.Name;//联系人
                    #region s[26] 与病人关系
                    Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.RelationLink);
                    if (RelativeObj != null && RelativeObj.ID != "")
                    {
                        if (RelativeObj.ID == "0" || RelativeObj.ID == "1" || RelativeObj.ID == "2" || RelativeObj.ID == "3" || RelativeObj.ID == "4" || RelativeObj.ID == "5" || RelativeObj.ID == "6" || RelativeObj.ID == "7")
                        {
                            
                            if (RelativeObj.ID == "0")
                            {
                                s[26] = "本人或户主";
                            }
                            else
                            {
                                s[26] = RelativeObj.Name;//与患者关系
                            }
                        }
                        else
                        {
                            s[26] = "其他";
                        }
                        //if (RelativeObj.Memo != "" && RelativeObj.Memo.ToUpper() != "TRUE")
                        //{
                        //    if (RelativeObj.Memo.Length <= 10)
                        //    {
                        //        s[26] = RelativeObj.Memo;//与患者关系
                        //    }
                        //    else
                        //    {
                        //        s[26] = RelativeObj.Memo.Substring(0, 10);//与患者关系
                        //    }
                        //}
                        //else
                        //{
                        //    if (RelativeObj.Name.Length <= 10)
                        //    {
                        //        s[26] = RelativeObj.Name;//与患者关系
                        //    }
                        //    else
                        //    {
                        //        s[26] = RelativeObj.Name.Substring(0, 10);//与患者关系
                        //    }
                        //}
                        //s[26] = RelativeObj.Name;//与患者关系
                    }
                    else
                    {
                        s[26] = b.PatientInfo.Kin.RelationLink;//与患者关系
                    }
                    #endregion
                    s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                    s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话
                    if (b.PatientInfo.SSN.Trim() == "--" || b.PatientInfo.SSN.Trim() == "－" || b.PatientInfo.SSN.Trim() == "-"
                    || b.PatientInfo.SSN.Trim() == "—" || b.PatientInfo.SSN.Trim() == "——"
                    || b.PatientInfo.SSN.Trim().Length < 4)
                    {
                        s[29] = "";
                    }
                    else
                    {
                        s[29] = b.PatientInfo.SSN; // 原3.0的医保卡号
                    }
                    s[30] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                    s[31] = b.PatientInfo.PVisit.InTime.ToShortTimeString(); //Hour.ToString().PadLeft(2, '0'); //入院时间
                    s[32] = this.ConverDept(b.InDept.ID).ID;//入院科室代码 入院统一科号，HIS接收时存储HIS科号
                    s[33] = this.ConverDept(b.InDept.ID).Name;//出院科室名称2011-6-8
                    s[34] = b.InRoom;//入院病室    
                    s[35] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                    s[36] = b.PatientInfo.PVisit.OutTime.ToShortTimeString(); //出院时间
                    s[37] = this.ConverDept(b.OutDept.ID).ID;//出院科室代码
                    s[38] = this.ConverDept(b.OutDept.ID).Name;//出院科室名称2011-6-8
                    s[39] = b.OutRoom; //出院病室
                    s[40] = b.InHospitalDays.ToString();//实际住院天数
                    if (b.ClinicDiag == null || string.IsNullOrEmpty(b.ClinicDiag.ID))
                    {
                        throw new Exception("门（急）诊诊断(ICD10或ICD9)编码不能为空!");
                    }
                    //存在先上传后编码的情况导致ICD为空
                    else if(b.ClinicDiag.ID == "-")
                    {
                        throw new Exception("没有门（急）诊诊断(ICD10或ICD9)编码");
                    }
                    else
                    {
                        s[41] = b.ClinicDiag.ID;//门（急）诊诊断(ICD10或ICD9)编码
                    }

                    if (b.ClinicDiag.Name.Length > 50)//门（急）诊诊断(ICD10或ICD9)对应疾病名
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name.Substring(0, 50).ToString());
                    }
                    else
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name);
                    }
                    s[43] = this.ConverDoc(b.ClinicDoc.ID).ID;//门、急诊医生编号，对应tdoctor 中的ftygh
                    s[44] = b.ClinicDoc.Name;//门、急诊医生
                    //病理诊断
                    //if (b.PathologicalDiagCode == null)
                    //{
                    //    s[45] = b.PathologicalDiagName;
                    //}
                    //else
                    //{
                    //    s[45] = b.PathologicalDiagCode;
                    //}

                    if (b.PathologicalDiagName.Trim() == "-"
                    || b.PathologicalDiagName.Trim() == "－"
                    || b.PathologicalDiagName.Trim() == "--"
                    || b.PathologicalDiagName.Trim() == "——"
                    || b.PathologicalDiagName.Trim() == "—"
                    || b.PathologicalDiagName == "未发现"
                    || b.PathologicalDiagName == "/"
                    || b.PathologicalDiagName.Trim() == "无")
                    // {1E7E4820-75FB-4048-99AD-AD40EB205D46}
                    //病理诊断名称为2个字符的时候传不到
                    //|| b.PathologicalDiagName.Trim().Length < 3)
                    {
                        s[45] = "";
                    }
                    else
                    {
                        s[45] = b.PathologicalDiagName;
                    }
                    

                    //过敏药物
                    string anaphyPh = b.FirstAnaphyPharmacy.ID;
                    if (anaphyPh.Length > 100)
                    {
                        s[46] = this.ChangeCharacter(anaphyPh.Substring(0, 100));
                    }
                    else
                    {
                        s[46] = this.ChangeCharacter(anaphyPh);//药物过敏  
                    }
                    //门诊与出院诊断符合情况编号
                    if (b.CePi == null || b.CePi == "")
                    {
                        s[47] = "1";
                        s[48] = "符合";
                    }
                    else
                    {
                        s[47] = b.CePi;
                        s[48] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.CePi).Name;
                    }
                    //临床与病理诊断符合情况
                    if (b.ClPa == null || b.ClPa == "")
                    {
                        s[49] = "1";
                        s[50] = "符合";
                    }
                    else
                    {
                        s[49] = b.ClPa;
                        s[50] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.ClPa).Name;
                    }
                    s[51] = b.SalvTimes.ToString();//抢救次数
                    s[52] = b.SuccTimes.ToString();//成功次数

                    s[53] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID).ID;//科主任编号，对应tdoctor 中的ftygh
                    s[54] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
                    s[55] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID).ID;//主（副主）任医生编号，对应tdoctor 中的ftygh
                    s[56] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
                    s[57] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID).ID;//主治医生编号，对应tdoctor 中的ftygh
                    s[58] = b.PatientInfo.PVisit.AttendingDoctor.Name;//主治医师姓名
                    s[59] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID).ID;//住院医生编号，对应tdoctor 中的ftygh
                    s[60] = b.PatientInfo.PVisit.AdmittingDoctor.Name;//住院医师姓名
                    s[61] = this.ConverDoc(b.RefresherDoc.ID).ID;//进修医师编号，对应tdoctor 中的ftygh
                    s[62] = b.RefresherDoc.Name;//进修医生
                    s[63] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID).ID;//实习医生编号，对应tdoctor 中的ftygh
                    s[64] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                    s[65] = this.ConverDoc(b.CodingOper.ID).ID;//编码员编号
                    s[66] = b.CodingOper.Name;//编码员名称
                    s[67] = this.ConverDoc(b.OperInfo.ID).ID;//病案整理者编号
                    s[68] = b.OperInfo.Name;//操作员名称（病案整理者）
                    s[69] = b.MrQuality;//病案质量 
                    s[70] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                    s[71] = this.ConverDoc(b.QcDoc.ID).ID;//质控医师名称
                    s[72] = b.QcDoc.Name;//质控医师
                    s[73] = this.ConverDoc(b.QcNurse.ID).ID;
                    s[74] = b.QcNurse.Name;//质控护士名称
                    //质控日期
                    if (b.CheckDate < new DateTime(1900, 1, 1))
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else if (b.CheckDate <= b.PatientInfo.PVisit.OutTime)//质控日期不可能小于出院日期
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else
                    {
                        s[75] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
                    }
                    #region 费用 总费用s[76] 西药费s[77] 中药费s[78] 中成药费s[79] 中草药费s[80] 其他费s[81]
                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[76] = "0.00";//总费用
                        s[77] = "0.00";//西药费
                        s[78] = "0.00";//中药费
                        s[79] = "0.00";//中成药费
                        s[80] = "0.00";//中草药费
                        s[81] = "0.00";//其他费
                    }
                    else
                    {
                        s[76] = Feeds.Tables[0].Rows[0][0].ToString();//总费用
                        s[77] = Feeds.Tables[0].Rows[0][17].ToString();//西药费
                        s[78] = "0.00";//中药费
                        s[79] = Feeds.Tables[0].Rows[0][19].ToString();//中成药费
                        s[80] = Feeds.Tables[0].Rows[0][20].ToString();//中草药费
                        s[81] = Feeds.Tables[0].Rows[0][29].ToString();//其他费
                    }
                    #endregion
                    //是否尸检编号1：是 2：否
                    if (b.CadaverCheck == "1")
                    {
                        s[82] = "1";
                        s[83] = "是";
                    }
                    else if (b.CadaverCheck == "2")
                    {
                        s[82] = "2";
                        s[83] = "否";
                    }
                    else
                    {
                        s[82] = "";
                        s[83] = "-";
                    }
                    //s[83] = this.constMana.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
                    #region s[84]血型编号 s[85]血型
                    //if (b.PatientInfo.BloodType.ID.ToString() == "A")
                    //{
                    //    s[84] = "1";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "B")
                    //{
                    //    s[84] = "2";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "AB")
                    //{
                    //    s[84] = "4";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                    //{
                    //    s[84] = "3";
                    //    s[85] = b.PatientInfo.BloodType.ID.ToString();
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                    //{
                    //    s[84] = "6";
                    //    s[85] = "未查";
                    //}
                    //else if (b.PatientInfo.BloodType.ID.ToString() == "6")
                    //{
                    //    s[84] = "6";
                    //    s[85] = "未查";
                    //}
                    //else
                    //{
                    //    s[84] = "5";
                    //    s[85] = "不详";
                    //} 

                    //CHANGE  BY ZHY    2013-08-01                 
                    if (b.PatientInfo.BloodType.ID.ToString() == "1")
                    {
                        s[84] = "1";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "2")
                    {
                        s[84] = "2";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "3")
                    {
                        s[84] = "3";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "4")
                    {
                        s[84] = "4";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "5")
                    {
                        s[84] = "5";
                        s[85] = b.PatientInfo.BloodType.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "6")
                    {
                        s[84] = "6";
                        // s[85] = b.PatientInfo.BloodType.ToString();
                        s[85] = "未查";
                    }
                    else
                    {
                        s[84] = "5";
                        // s[85] = b.PatientInfo.BloodType.ToString();
                        s[85] = "不详";
                    }
                    #endregion
                    //s[86] = b.RhBlood;//RH编号
                    //RH
                    if (b.RhBlood == "1")
                    {
                        s[86] = "1";
                        s[87] = "阴";
                    }
                    else if (b.RhBlood == "2")
                    {
                        s[86] = "2";
                        s[87] = "阳";
                    }
                    else if (b.RhBlood == "3")
                    {
                        s[86] = "3";
                        s[87] = "不详";
                    }
                    else
                    {
                        s[86] = "4";
                        s[87] = "未查";
                    }
                    //婴儿数
                    int babyNum = 0;
                    try
                    {
                        babyNum = Neusoft.FrameWork.Function.NConvert.ToInt32(b.PatientInfo.User03);
                        s[88] = babyNum.ToString();
                    }
                    catch
                    {
                        s[88] = "0";
                    }
                    s[89] = "0";//是否部分病种，1是 0否
                    #region  s[90]首次转科统一科号，HIS接收时存储HIS科号 s[91]首次转科科别  s[92]首次转科日期  s[93]首次转科时间
                    if (alChangeDepe != null && alChangeDepe.Count > 0)
                    {
                        Neusoft.HISFC.Models.RADT.Location changeDept = alChangeDepe[0] as Neusoft.HISFC.Models.RADT.Location;
                        if (changeDept.Dept.ID != null && changeDept.Dept.ID != "" && changeDept.Dept.ID != b.InDept.ID)
                        {
                            try
                            {
                                s[90] = this.ConverDept(changeDept.Dept.ID).ID;//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = this.ConverDept(changeDept.Dept.ID).Name;//首次转科科别
                                s[92] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).ToShortDateString();//首次转科日期
                                s[93] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).ToShortTimeString();//Hour.ToString().PadLeft(2, '0');//首次转科时间
                            }
                            catch
                            {
                                s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = "";//首次转科科别
                                s[92] = "";//首次转科日期
                                s[93] = "";//首次转科时间
                            }
                        }
                        else
                        {
                            s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                            s[91] = "";//首次转科科别
                            s[92] = "";//首次转科日期
                            s[93] = "";//首次转科时间
                        }
                    }
                    else
                    {
                        s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                        s[91] = "";//首次转科科别
                        s[92] = "";//首次转科日期
                        s[93] = "";//首次转科时间
                    }
                    #endregion
                    s[94] = this.ConverDoc(b.OperInfo.ID).ID;//输入员编号
                    // {18212A5B-6EB5-40cf-BF42-516E71A11D2E}
                    //s[95] = SaveOperation.ID;//输入员
                    //s[96] = System.DateTime.Now.ToShortDateString();//输入日期Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.FrameWork.Models.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');               
                    if (!string.IsNullOrEmpty(b.SaveOperation.ID))
                    {
                        Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
                        employee = personMgr.GetPersonByID(b.SaveOperation.ID);
                        s[95] = employee.Name;//输入员
                        s[96] = b.LastSaveTime.ToShortDateString();
                    }
                    else
                    {
                        s[95] = b.OperInfo.Name;//输入员
                        s[96] = System.DateTime.Now.ToShortDateString();//输入日期Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.FrameWork.Models.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');          
                    }
                    
                    Neusoft.FrameWork.Models.NeuObject CaseExaplTypeObj = this.constMana.GetConstant("CASEEXAMPLETYPE", b.ExampleType);
                    if (CaseExaplTypeObj != null && CaseExaplTypeObj.ID != "")
                    {
                        if (CaseExaplTypeObj.Memo != "" && CaseExaplTypeObj.Memo.ToUpper() != "TRUE")
                        {
                            s[97] = CaseExaplTypeObj.Memo;//疾病分型编号
                            s[98] = b.ExampleType;//疾病分型
                        }
                        else
                        {
                            switch (b.ExampleType)//疾病分型编号
                            {
                                case "A":
                                    s[97] = "1";
                                    break;
                                case "B":
                                    s[97] = "2";
                                    break;
                                case "C":
                                    s[97] = "3";
                                    break;
                                case "D":
                                    s[97] = "4";
                                    break;
                            }
                            s[98] = b.ExampleType;//疾病分型
                        }
                    }
                    else
                    {
                        switch (b.ExampleType)//疾病分型编号
                        {
                            case "A":
                                s[97] = "1";
                                break;
                            case "B":
                                s[97] = "2";
                                break;
                            case "C":
                                s[97] = "3";
                                break;
                            case "D":
                                s[97] = "4";
                                break;
                        }
                        s[98] = b.ExampleType;//疾病分型
                    }

                    s[99] = "";//复合归档编号
                    s[100] = "";//复合归档
                    s[101] = b.PatientInfo.PVisit.InSource.ID;//病人来源编号
                    s[102] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;//病人来源
                    s[103] = b.PatientInfo.User02;//是否手术 chengym
                    if (b.PatientInfo.User03 == "0")
                    {
                        s[104] = "0";//是否输入妇婴卡
                    }
                    else
                    {
                        s[104] = "1";//是否输入妇婴卡
                    }
                    //院感次数，来源院感报卡 12-8-28
                    try
                    {
                        int infNum = this.baseDml.QueryInfCount(b.PatientInfo.ID);
                        if (infNum == -1)
                        {
                            s[105] = "0";//医院感染次数，不允许为空，否则影响报表统计结果 chengym
                        }
                        else
                        {
                            s[105] = infNum.ToString();
                        }
                    }
                    catch
                    {
                        s[105] = "0";
                    }
                    s[106] = "";//扩展1 
                    s[107] = "";
                    s[108] = "";
                    s[109] = "";
                    s[110] = "";
                    s[111] = "";
                    s[112] = "";
                    s[113] = "";//扩展8档案号
                    s[114] = "";//扩展9档案次数
                    s[115] = "";
                    s[116] = "";
                    s[117] = "";
                    s[118] = "";
                    s[119] = "";
                    s[120] = "";//扩展15
                    s[121] = b.PatientInfo.DIST;//籍贯
                    string currenadd = string.Empty;
                    try
                    {
                        string[] tt = b.CurrentAddr.Split('@');
                        string temp = string.Empty;
                        if (tt.Length > 1)
                        {
                            temp = tt[0].ToString();
                        }
                        string code = this.QueryCurrentAdrrCodeByName(temp + "@");
                        currenadd = code + b.CurrentAddr;
                        if (currenadd.Length > 50)//超过50个字符，省厅病案不认
                        {
                            currenadd = code + temp + "@";
                        }
                    }
                    catch
                    {
                    }
                    if (currenadd != string.Empty)
                    {
                        s[122] = currenadd;
                    }
                    else
                    {
                        s[122] = b.CurrentAddr;//现住址
                    }
                    s[123] = b.CurrentPhone;//现电话
                    if (b.CurrentZip != null && b.CurrentZip.Length == 6)
                    {
                        s[124] = b.CurrentZip;//现邮编
                    }
                    else
                    {
                        s[124] = "";
                    }
                    s[125] = b.PatientInfo.Profession.ID;//职业编号
                    //可能存在医院填写非数字内容
                    try
                    {
                        int bweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyBirthWeight);
                        b.BabyBirthWeight = bweight.ToString();
                    }
                    catch
                    {
                        b.BabyBirthWeight = "0";
                    }
                    s[126] = b.BabyBirthWeight;//新生儿出生体重
                    //可能存在医院填写非数字内容
                    try
                    {
                        int biweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyInWeight);
                        b.BabyInWeight = biweight.ToString();
                    }
                    catch
                    {
                        b.BabyInWeight = "0";
                    }
                    s[127] = b.BabyInWeight;//新生儿入院体重
                    s[128] = b.InPath;//入院途径编号
                    s[129] = this.constMana.GetConstant("CASEINAVENUE", b.InPath).Name;//入院途径
                    s[130] = b.ClinicPath;//临床路径病例编号
                    if (b.ClinicPath == "1")
                    {
                        s[131] = "是";//临床路径病例
                    }
                    else
                    {
                        s[131] = "否";//临床路径病例
                    }
                    if (b.PathologicalDiagName.Trim() == "-"
                   || b.PathologicalDiagName.Trim() == "－"
                   || b.PathologicalDiagName.Trim() == "--"
                   || b.PathologicalDiagName.Trim() == "——"
                   || b.PathologicalDiagName.Trim() == "—"
                   || b.PathologicalDiagName == "未发现"
                   || b.PathologicalDiagName == "/"
                   || b.PathologicalDiagName.Trim() == "无"
                   //|| b.PathologicalDiagName.Trim().Length < 3
                        )
                    {
                        s[49] = "";
                        s[50] = "";
                        s[132] = "";//病理疾病编码
                        s[133] = "";//病理号
                    }
                    else
                    {
                        s[132] = b.PathologicalDiagCode;//病理疾病编码
                        s[133] = b.PathNum;//病理号
                    }
                    s[134] = b.AnaphyFlag;//是否药物过敏编号
                    if (b.AnaphyFlag == "1")
                    {
                        s[135] = "无";//是否药物过敏
                    }
                    else
                    {
                        s[135] = "有";//是否药物过敏
                    }
                    s[136] = this.ConverDoc(b.DutyNurse.ID).ID;//责任护士编号
                    s[137] = b.DutyNurse.Name;//责任护士
                    //s[138] = b.Out_Type;//离院方式编号
                    if (b.Out_Type == "1")//离院方式
                    {
                        s[138] = "1";
                        s[139] = "医嘱离院";
                    }
                    else if (b.Out_Type == "2")//离院方式
                    {
                        s[138] = "2";
                        s[139] = "医嘱转院";
                    }
                    else if (b.Out_Type == "3")//离院方式
                    {
                        s[138] = "3";
                        s[139] = "医嘱转社区乡镇卫生院";
                    }
                    else if (b.Out_Type == "4")//离院方式
                    {
                        s[138] = "4";
                        s[139] = "非医嘱离院";
                    }
                    else if (b.Out_Type == "5")//离院方式
                    {
                        s[138] = "5";
                        s[139] = "死亡";
                    }
                    else
                    {
                        s[138] = "9";
                        s[139] = "其他";
                    }
                    s[140] = b.HighReceiveHopital;//离院方式为医嘱转院，拟接收医疗机构名称
                    s[141] = b.LowerReceiveHopital;//离院方式为转社区卫生服务器机构/乡镇卫生院，拟接收医疗机构名称
                    s[142] = b.ComeBackInMonth;//是否有出院31天内再住院计划编号
                    s[143] = "";//是否有出院31天内再住院计划
                    s[144] = b.ComeBackPurpose;//再住院目的
                    s[145] = b.OutComeDay.ToString();//颅脑损伤患者昏迷时间：入院前 天
                    s[146] = b.OutComeHour.ToString();//颅脑损伤患者昏迷时间：入院前 小时
                    s[147] = b.OutComeMin.ToString();//颅脑损伤患者昏迷时间：入院前 分钟
                    s[148] = (b.OutComeDay * 24 * 60 + b.OutComeHour * 60 + b.OutComeMin).ToString();//入院前昏迷总分钟(天、小时换算成分钟)
                    s[149] = b.InComeDay.ToString();//颅脑损伤患者昏迷时间：入院后 天
                    s[150] = b.InComeHour.ToString();//颅脑损伤患者昏迷时间：入院后 小时
                    s[151] = b.InComeMin.ToString();//颅脑损伤患者昏迷时间：入院后 分钟
                    s[152] = (b.InComeDay * 24 * 60 + b.InComeHour * 60 + b.InComeMin).ToString();//入院后昏迷总分钟
                    if (b.PatientInfo.Pact.ID == "9" || string.IsNullOrEmpty(b.PatientInfo.Pact.ID))
                    {
                        s[153] = "99";//付款方式编号
                    }
                    else
                    {
                        s[153] = b.PatientInfo.Pact.ID.PadLeft(2, '0');//付款方式编号
                    }
                    //s[153] = b.PatientInfo.Pact.ID;//付款方式编号
                    s[154] = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID).Name;//付款方式

                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[155] = "0.00";//住院总费用：自费金额
                        s[156] = "0.00";//综合医疗服务类：（1）一般医疗服务费
                        s[157] = "0.00";//综合医疗服务类：（2）一般治疗操作费
                        s[158] = "0.00";//综合医疗服务类：（3）护理费
                        s[159] = "0.00";//综合医疗服务类：（4）其他费用
                        s[160] = "0.00";//诊断类：(5) 病理诊断费
                        s[161] = "0.00";//诊断类：(6) 实验室诊断费
                        s[162] = "0.00";//诊断类：(7) 影像学诊断费
                        s[163] = "0.00";//诊断类：(8) 临床诊断项目费
                        s[164] = "0.00";//治疗类：(9) 非手术治疗项目费
                        s[165] = "0.00";//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = "0.00";//治疗类：(10) 手术治疗费
                        s[167] = "0.00";//治疗类：手术治疗费 其中麻醉费
                        s[168] = "0.00";//治疗类：手术治疗费 其中手术费
                        s[169] = "0.00";//康复类：(11) 康复费
                        s[170] = "0.00";//中医类：中医治疗类
                        s[171] = "0.00";//西药类： 西药费 其中抗菌药物费用
                        s[172] = "0.00";//血液和血液制品类： 血费
                        s[173] = "0.00";//血液和血液制品类： 白蛋白类制品费
                        s[174] = "0.00";//血液和血液制品类： 球蛋白制品费
                        s[175] = "0.00";//血液和血液制品类：凝血因子类制品费
                        s[176] = "0.00";//血液和血液制品类： 细胞因子类费
                        s[177] = "0.00";//耗材类：检查用一次性医用材料费
                        s[178] = "0.00";//耗材类：治疗用一次性医用材料费
                        s[179] = "0.00";//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    else
                    {
                        s[155] = Feeds.Tables[0].Rows[0][1].ToString();//自负金额
                        s[156] = Feeds.Tables[0].Rows[0][2].ToString();//综合医疗服务类：（1）一般医疗服务费
                        s[157] = Feeds.Tables[0].Rows[0][3].ToString();//综合医疗服务类：（2）一般治疗操作费
                        s[158] = Feeds.Tables[0].Rows[0][4].ToString();//综合医疗服务类：（3）护理费
                        s[159] = Feeds.Tables[0].Rows[0][5].ToString();//综合医疗服务类：（4）其他费用
                        s[160] = Feeds.Tables[0].Rows[0][6].ToString();//诊断类：(5) 病理诊断费
                        s[161] = Feeds.Tables[0].Rows[0][7].ToString();//诊断类：(6) 实验室诊断费
                        s[162] = Feeds.Tables[0].Rows[0][8].ToString();//诊断类：(7) 影像学诊断费
                        s[163] = Feeds.Tables[0].Rows[0][9].ToString();//诊断类：(8) 临床诊断项目费
                        s[164] = Feeds.Tables[0].Rows[0][10].ToString();//治疗类：(9) 非手术治疗项目费
                        s[165] = Feeds.Tables[0].Rows[0][11].ToString();//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = Feeds.Tables[0].Rows[0][12].ToString();//治疗类：(10) 手术治疗费
                        s[167] = Feeds.Tables[0].Rows[0][13].ToString();//治疗类：手术治疗费 其中麻醉费
                        s[168] = Feeds.Tables[0].Rows[0][14].ToString();//治疗类：手术治疗费 其中手术费
                        s[169] = Feeds.Tables[0].Rows[0][15].ToString();//康复类：(11) 康复费
                        s[170] = Feeds.Tables[0].Rows[0][16].ToString();//中医类：中医治疗类
                        s[171] = Feeds.Tables[0].Rows[0][18].ToString();//西药类： 西药费 其中抗菌药物费用
                        s[172] = Feeds.Tables[0].Rows[0][21].ToString();//血液和血液制品类： 血费
                        s[173] = Feeds.Tables[0].Rows[0][22].ToString();//血液和血液制品类： 白蛋白类制品费
                        s[174] = Feeds.Tables[0].Rows[0][23].ToString();//血液和血液制品类： 球蛋白制品费
                        s[175] = Feeds.Tables[0].Rows[0][24].ToString();//血液和血液制品类：凝血因子类制品费
                        s[176] = Feeds.Tables[0].Rows[0][25].ToString();//血液和血液制品类： 细胞因子类费
                        s[177] = Feeds.Tables[0].Rows[0][26].ToString();//耗材类：检查用一次性医用材料费
                        s[178] = Feeds.Tables[0].Rows[0][27].ToString();//耗材类：治疗用一次性医用材料费
                        s[179] = Feeds.Tables[0].Rows[0][28].ToString();//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    return s;
                }
                catch (Exception ex)
                {
                    this.Err = ex.ToString();
                    return null;
                }
                #endregion
            }
            else //顺德妇幼传基本信息 2012-12-27
            {
                this.Err = "患者信息来自患者主表,门诊诊断编码为空!不能上传!联系信息科";
                return null;
                #region
                string[] s = new string[194];
                try
                {
                    s[0] = "0";//是否输入，0：否，1：是，默认接收为0
                    //s[1] = this.PatientNoChang(b.PatientInfo.PID.PatientNO.TrimStart(new char[] { '0' }));//病案号
                    string patientNO = b.PatientInfo.PID.PatientNO.PadLeft(10, '0');
                    s[1] = this.PatientNoChang(patientNO.Substring(this.PatientNoSubstr()));//病案号
                    s[2] = b.PatientInfo.InTimes.ToString();//住院次数
                    s[3] = "11";//ICD版本，9：ICD9字典库，10：ICD10字典库，待扩展11：国家ICD库，默认接收为11
                    s[4] = b.PatientInfo.ID;//住院流水号
                    #region //s[5] 年龄
                    if (b.PatientInfo.Age != "" && b.PatientInfo.Age != "0")
                    {
                        if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") < 0) //整岁
                        {
                            s[5] = "Y" + b.AgeUnit.Replace("岁", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//整月
                        {
                            s[5] = "M" + b.AgeUnit.Replace("月", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") < 0 && b.AgeUnit.IndexOf("天") > 0)//整天
                        {
                            s[5] = "D" + b.AgeUnit.Replace("天", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") < 0)//N岁N月
                        {
                            string[] PAge = b.AgeUnit.Split('岁');
                            s[5] = "Y" + PAge[0] + "M" + PAge[1].Replace("岁", "").Replace("月", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") < 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N月N天
                        {
                            string[] PAge = b.AgeUnit.Split('月');
                            s[5] = "M" + PAge[0] + "D" + PAge[1].Replace("月", "").Replace("天", "");
                        }
                        else if (b.AgeUnit.IndexOf("岁") > 0 && b.AgeUnit.IndexOf("月") > 0 && b.AgeUnit.IndexOf("天") > 0)//N岁N月N天
                        {
                            string[] PAge = b.AgeUnit.Split('岁');

                            string[] PAge1 = PAge[1].Split('月');
                            s[5] = "Y" + PAge[0] + "M" + PAge1[0] + "D" + PAge1[1].Replace("月", "").Replace("天", "");
                        }
                    }
                    else
                    {
                        int ts = b.PatientInfo.PVisit.InTime.Year - b.PatientInfo.Birthday.Year;

                        if (ts == 0)
                        {
                            ts = b.PatientInfo.PVisit.InTime.Month - b.PatientInfo.Birthday.Month;

                            if (ts == 0)
                            {
                                ts = b.PatientInfo.PVisit.InTime.Day - b.PatientInfo.Birthday.Day;
                                s[5] = "D" + ts.ToString();//年龄 
                            }
                            else
                            {
                                s[5] = "M" + ts.ToString();//年龄 
                            }
                        }
                        else
                        {
                            s[5] = "Y" + ts.ToString();//年龄 
                        }
                    }
                    #endregion

                    s[6] = b.PatientInfo.Name;//病人姓名
                    //性别编号
                    //性别
                    if (b.PatientInfo.Sex.ID.ToString() == "M" || b.PatientInfo.Sex.ID.ToString() == "1")
                    {
                        s[7] = "1";
                        s[8] = "男";
                    }
                    else
                    {
                        s[7] = "2";
                        s[8] = "女";
                    }
                    s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');//出生日期
                    s[10] = b.PatientInfo.AddressHome; //出生地
                    if (b.PatientInfo.IDCard == "" || b.PatientInfo.IDCard.Trim() == "-")
                    {
                        s[11] = "不详";
                    }
                    else
                    {
                        s[11] = b.PatientInfo.IDCard;//身份证号
                    }
                    #region s[12]国籍编号\ s[13]国籍  中国  需要转换其他
                    if (b.PatientInfo.Country.ID.ToString() == "1")
                    {
                        s[12] = "A156";
                        s[13] = "中国";
                    }
                    else
                    {
                        Neusoft.FrameWork.Models.NeuObject countryObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                        if (countryObj != null && countryObj.ID != "")
                        {
                            if (countryObj.Memo != "" && countryObj.Memo.ToUpper() != "TRUE")
                            {
                                s[12] = countryObj.Memo.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                            else
                            {
                                s[12] = countryObj.ID.ToString();
                                s[13] = countryObj.Name.ToString();
                            }
                        }
                        else
                        {
                            s[12] = b.PatientInfo.Country.ID.ToString();
                            s[13] = "";
                        }
                    }
                    #endregion
                    #region s[14]民族编号 s[15]民族
                    Neusoft.FrameWork.Models.NeuObject NationObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                    if (NationObj != null && NationObj.ID != "")
                    {
                        if (NationObj.Memo != "" && NationObj.Memo.ToUpper() != "TRUE")
                        {
                            s[14] = NationObj.Memo;
                            s[15] = NationObj.Name;
                        }
                        else
                        {
                            s[14] = NationObj.ID;
                            s[15] = NationObj.Name;
                        }
                    }
                    else
                    {
                        s[14] = b.PatientInfo.Nationality.ID;
                        s[15] = "";
                    }
                    #endregion
                    #region  s[16] 职业有中文传中文
                    //add by chengym 2011-6-15  字典表的名称字段varchar（100） 有些职业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                    Neusoft.FrameWork.Models.NeuObject JobObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.CASEPROFESSION, b.PatientInfo.Profession.ID.ToString());
                    if (JobObj != null && JobObj.ID != "")
                    {
                        if (JobObj.Memo != "" && JobObj.Memo.ToUpper() != "TRUE")
                        {
                            if (JobObj.Memo.Length <= 50)
                            {
                                s[16] = JobObj.Memo;
                            }
                            else
                            {
                                s[16] = JobObj.Memo.Substring(0, 50);
                            }
                        }
                        else
                        {
                            if (JobObj.Name.Length <= 50)
                            {
                                s[16] = JobObj.Name;
                            }
                            else
                            {
                                s[16] = JobObj.Name.Substring(0, 50);
                            }
                        }
                    }
                    else
                    {
                        s[16] = b.PatientInfo.Profession.ID.ToString(); //职业 没有传中文不知道是否可以
                    }
                    #endregion
                    #region s[17] 婚姻状况编号 s[18]婚姻状况
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "10")
                    {
                        s[17] = "10"; //婚姻状况编号
                        s[18] = "未婚"; //婚姻状况
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "20")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "3"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "30")
                    {
                        s[17] = "30";
                        s[18] = "丧偶";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4"
                        || b.PatientInfo.MaritalStatus.ID.ToString() == "40")
                    {
                        s[17] = "40";
                        s[18] = "离婚";
                    }
                    else if (b.PatientInfo.MaritalStatus.ID.ToString() == "R")
                    {
                        s[17] = "20";
                        s[18] = "已婚";
                    }
                    else
                    {
                        s[17] = "90";
                        s[18] = "未说明的婚姻状况";
                    }
                    #endregion
                    if (b.PatientInfo.HomeZip != null && b.PatientInfo.HomeZip != "")
                    {
                        b.PatientInfo.BusinessZip = b.PatientInfo.HomeZip;
                        b.CurrentZip = b.PatientInfo.HomeZip;
                    }
                    else if (b.PatientInfo.BusinessZip != null && b.PatientInfo.BusinessZip != "")
                    {
                        b.PatientInfo.HomeZip = b.PatientInfo.BusinessZip;
                        b.CurrentZip = b.PatientInfo.BusinessZip;
                    }
                    else if (b.CurrentZip != null && b.CurrentZip != "")
                    {
                        b.PatientInfo.HomeZip = b.CurrentZip;
                        b.PatientInfo.BusinessZip = b.CurrentZip;
                    }
                    if (b.PatientInfo.AddressHome != null && b.PatientInfo.AddressHome != "")
                    {
                        b.CurrentAddr = b.PatientInfo.AddressHome;
                    }
                    s[19] = b.PatientInfo.AddressBusiness;  //单位名称
                    s[20] = b.PatientInfo.CompanyName;//单位地址 
                    s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                    s[22] = b.PatientInfo.BusinessZip;//单位邮编      
                    s[23] = b.PatientInfo.AddressHome;//户口地址
                    s[24] = b.PatientInfo.HomeZip;//户口邮编
                    s[25] = b.PatientInfo.Kin.Name;//联系人
                    #region s[26] 与病人关系
                    Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.Relation.ID);
                    if (RelativeObj != null && RelativeObj.ID != "")
                    {
                        if (RelativeObj.Memo != "" && RelativeObj.Memo.ToUpper() != "TRUE")
                        {
                            if (RelativeObj.Memo.Length <= 10)
                            {
                                s[26] = RelativeObj.Memo;//与患者关系
                            }
                            else
                            {
                                s[26] = RelativeObj.Memo.Substring(0, 10);//与患者关系
                            }
                        }
                        else
                        {
                            if (RelativeObj.Name.Length <= 10)
                            {
                                s[26] = RelativeObj.Name;//与患者关系
                            }
                            else
                            {
                                s[26] = RelativeObj.Name.Substring(0, 10);//与患者关系
                            }
                        }
                    }
                    else
                    {
                        s[26] = b.PatientInfo.Kin.RelationLink;//与患者关系
                    }
                    #endregion
                    s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                    s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话
                    s[29] = b.PatientInfo.SSN; // 原3.0的医保卡号
                    s[30] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                    s[31] = b.PatientInfo.PVisit.InTime.Hour.ToString().PadLeft(2, '0'); //入院时间
                    Neusoft.HISFC.Models.RADT.Location indept = this.baseDml.GetDeptIn(b.PatientInfo.ID);
                    if (indept != null) //入院科室 
                    {
                        s[32] = this.ConverDept(indept.Dept.ID).ID;//入院科室代码
                        s[33] = this.ConverDept(indept.Dept.ID).Name;//入院科室名称
                    }
                    else
                    {
                        s[32] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//入院科室代码
                        s[33] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//入院科室名称
                    }
                    s[34] = b.InRoom;//入院病室    
                    s[35] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                    s[36] = b.PatientInfo.PVisit.OutTime.Hour.ToString().PadLeft(2, '0'); //出院时间
                    s[37] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//出院科室代码
                    s[38] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//出院科室名称2011-6-8
                    s[39] = b.OutRoom; //出院病室
                    System.TimeSpan tt = b.PatientInfo.PVisit.OutTime - b.PatientInfo.PVisit.InTime;
                    s[40] = tt.Days.ToString();//实际住院天数
                    s[41] = b.ClinicDiag.ID;//门（急）诊诊断(ICD10或ICD9)编码

                    if (b.ClinicDiag.Name.Length > 50)//门（急）诊诊断(ICD10或ICD9)对应疾病名
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name.Substring(0, 50).ToString());
                    }
                    else
                    {
                        s[42] = this.ChangeCharacter(b.ClinicDiag.Name);
                    }
                    s[43] = this.ConverDoc(b.ClinicDoc.ID).ID;//门、急诊医生编号，对应tdoctor 中的ftygh
                    s[44] = b.ClinicDoc.Name;//门、急诊医生
                    //病理诊断
                    if (b.PathologicalDiagCode == null)
                    {
                        s[45] = b.PathologicalDiagName;
                    }
                    else
                    {
                        s[45] = b.PathologicalDiagCode;
                    }
                    //过敏药物
                    string anaphyPh = b.FirstAnaphyPharmacy.ID;
                    if (anaphyPh.Length > 100)
                    {
                        s[46] = this.ChangeCharacter(anaphyPh.Substring(0, 100));
                    }
                    else
                    {
                        s[46] = this.ChangeCharacter(anaphyPh);//药物过敏  
                    }
                    //s[46] = "1";
                    //门诊与出院诊断符合情况编号
                    if (b.CePi == null || b.CePi == "")
                    {
                        s[47] = "1";
                        s[48] = "符合";
                    }
                    else
                    {
                        s[47] = b.CePi;
                        s[48] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.CePi).Name;
                    }
                    //临床与病理诊断符合情况
                    if (b.ClPa == null || b.ClPa == "")
                    {
                        s[49] = "1";
                        s[50] = "符合";
                    }
                    else
                    {
                        s[49] = b.ClPa;
                        s[50] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.ClPa).Name;
                    }
                    s[51] = b.SalvTimes.ToString();//抢救次数
                    s[52] = b.SuccTimes.ToString();//成功次数

                    s[53] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID).ID;//科主任编号，对应tdoctor 中的ftygh
                    s[54] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
                    s[55] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID).ID;//主（副主）任医生编号，对应tdoctor 中的ftygh
                    s[56] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
                    s[57] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID).ID;//主治医生编号，对应tdoctor 中的ftygh
                    s[58] = b.PatientInfo.PVisit.AttendingDoctor.Name;//主治医师姓名
                    s[59] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID).ID;//住院医生编号，对应tdoctor 中的ftygh
                    s[60] = b.PatientInfo.PVisit.AdmittingDoctor.Name;//住院医师姓名
                    s[61] = this.ConverDoc(b.RefresherDoc.ID).ID;//进修医师编号，对应tdoctor 中的ftygh
                    s[62] = b.RefresherDoc.Name;//进修医生
                    s[63] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID).ID;//实习医生编号，对应tdoctor 中的ftygh
                    s[64] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                    s[65] = this.ConverDoc(b.CodingOper.ID).ID;//编码员编号
                    s[66] = b.CodingOper.Name;//编码员名称
                    s[67] = this.ConverDoc(b.OperInfo.ID).ID;//病案整理者编号
                    s[68] = b.OperInfo.Name;//操作员名称（病案整理者）
                    if (b.MrQuality == null || b.MrQuality == "")
                    {
                        s[69] = "1";
                    }
                    else
                    {
                        s[69] = b.MrQuality;//病案质量 
                    }
                    s[70] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                    s[71] = this.ConverDoc(b.QcDoc.ID).ID;//质控医师名称
                    s[72] = b.QcDoc.Name;//质控医师
                    s[73] = this.ConverDoc(b.QcNurse.ID).ID;
                    s[74] = b.QcNurse.Name;//质控护士名称
                    //质控日期
                    if (b.CheckDate < new DateTime(1900, 1, 1))
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else if (b.CheckDate <= b.PatientInfo.PVisit.OutTime)//质控日期不可能小于出院日期
                    {
                        s[75] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else
                    {
                        s[75] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
                    }
                    #region 费用 总费用s[76] 西药费s[77] 中药费s[78] 中成药费s[79] 中草药费s[80] 其他费s[81]
                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[76] = "0.00";//总费用
                        s[77] = "0.00";//西药费
                        s[78] = "0.00";//中药费
                        s[79] = "0.00";//中成药费
                        s[80] = "0.00";//中草药费
                        s[81] = "0.00";//其他费
                    }
                    else
                    {
                        s[76] = Feeds.Tables[0].Rows[0][0].ToString();//总费用
                        s[77] = Feeds.Tables[0].Rows[0][17].ToString();//西药费
                        s[78] = "0.00";//中药费
                        s[79] = Feeds.Tables[0].Rows[0][19].ToString();//中成药费
                        s[80] = Feeds.Tables[0].Rows[0][20].ToString();//中草药费
                        s[81] = Feeds.Tables[0].Rows[0][29].ToString();//其他费
                    }
                    #endregion
                    //是否尸检编号1：是 2：否
                    if (b.CadaverCheck == "1")
                    {
                        s[82] = "1";
                        s[83] = "是";
                    }
                    else if (b.CadaverCheck == "2")
                    {
                        s[82] = "2";
                        s[83] = "否";
                    }
                    else
                    {
                        s[82] = "2";
                        s[83] = "否";
                    }
                    //s[83] = this.constMana.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
                    #region s[84]血型编号 s[85]血型
                    if (b.PatientInfo.BloodType.ID.ToString() == "A")
                    {
                        s[84] = "1";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "B")
                    {
                        s[84] = "2";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "AB")
                    {
                        s[84] = "4";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                    {
                        s[84] = "3";
                        s[85] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                    {
                        s[84] = "6";
                        s[85] = "未查";
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "6")
                    {
                        s[84] = "6";
                        s[85] = "未查";
                    }
                    else
                    {
                        s[84] = "6";
                        s[85] = "未查";
                    }
                    #endregion
                    //s[86] = b.RhBlood;//RH编号
                    //RH
                    //if (b.RhBlood == "1")
                    //{
                    //    s[86] = "1";
                    //    s[87] = "阴";
                    //}
                    //else if (b.RhBlood == "2")
                    //{
                    //    s[86] = "2";
                    //    s[87] = "阳";
                    //}
                    //else if (b.RhBlood == "3")
                    //{
                    //    s[86] = "3";
                    //    s[87] = "不详";
                    //}
                    //else
                    //{
                    //    //s[86] = "4";
                    //    //s[87] = "未查";
                    s[86] = "2";
                    s[87] = "阳";
                    //}
                    //婴儿数
                    int babyNum = 0;
                    try
                    {
                        babyNum = Neusoft.FrameWork.Function.NConvert.ToInt32(b.PatientInfo.User03);
                        s[88] = babyNum.ToString();
                    }
                    catch
                    {
                        s[88] = "0";
                    }
                    s[89] = "0";//是否部分病种，1是 0否
                    #region  s[90]首次转科统一科号，HIS接收时存储HIS科号 s[91]首次转科科别  s[92]首次转科日期  s[93]首次转科时间
                    if (alChangeDepe != null && alChangeDepe.Count > 0)
                    {
                        Neusoft.HISFC.Models.RADT.Location changeDept = alChangeDepe[0] as Neusoft.HISFC.Models.RADT.Location;
                        if (changeDept.Dept.ID != null && changeDept.Dept.ID != "")
                        {
                            try
                            {
                                s[90] = this.ConverDept(changeDept.Dept.ID).ID;//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = this.ConverDept(changeDept.Dept.ID).Name;//首次转科科别
                                s[92] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).ToShortDateString();//首次转科日期
                                s[93] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changeDept.Dept.Memo).Hour.ToString().PadLeft(2, '0');//首次转科时间
                            }
                            catch
                            {
                                s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                                s[91] = "";//首次转科科别
                                s[92] = "";//首次转科日期
                                s[93] = "";//首次转科时间
                            }
                        }
                        else
                        {
                            s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                            s[91] = "";//首次转科科别
                            s[92] = "";//首次转科日期
                            s[93] = "";//首次转科时间
                        }
                    }
                    else
                    {
                        s[90] = "";//首次转科统一科号，HIS接收时存储HIS科号
                        s[91] = "";//首次转科科别
                        s[92] = "";//首次转科日期
                        s[93] = "";//首次转科时间
                    }
                    #endregion
                    s[94] = this.ConverDoc(b.OperInfo.ID).ID;//输入员编号
                    s[95] = b.OperInfo.Name;//输入员
                    s[96] = System.DateTime.Now.ToShortDateString();//输入日期Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.FrameWork.Models.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');
                    //Neusoft.FrameWork.Models.NeuObject CaseExaplTypeObj = this.constMana.GetConstant("CASEEXAMPLETYPE", b.ExampleType);
                    //if (CaseExaplTypeObj != null && CaseExaplTypeObj.ID != "")
                    //{
                    //    if (CaseExaplTypeObj.Memo != "" && CaseExaplTypeObj.Memo.ToUpper() != "TRUE")
                    //    {
                    //        s[97] = CaseExaplTypeObj.Memo;//疾病分型编号
                    //        s[98] = b.ExampleType;//疾病分型
                    //    }
                    //}
                    //else
                    //{
                    //    switch (b.ExampleType)//疾病分型编号
                    //    {
                    //        case "A":
                    //            s[97] = "1";
                    //            break;
                    //        case "B":
                    //            s[97] = "2";
                    //            break;
                    //        case "C":
                    //            s[97] = "3";
                    //            break;
                    //        case "D":
                    //            s[97] = "4";
                    //            break;
                    //    }
                    //    s[98] = b.ExampleType;//疾病分型
                    //}
                    s[97] = "1";
                    s[98] = "一般";//疾病分型

                    s[99] = "";//复合归档编号
                    s[100] = "";//复合归档
                    s[101] = b.PatientInfo.PVisit.InSource.ID;//病人来源编号
                    s[102] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;//病人来源
                    s[103] = b.PatientInfo.User02;//是否手术 chengym
                    if (b.PatientInfo.User03 == "0")
                    {
                        s[104] = "0";//是否输入妇婴卡
                    }
                    else
                    {
                        s[104] = "1";//是否输入妇婴卡
                    }
                    //院感次数，来源院感报卡 12-8-28
                    try
                    {
                        int infNum = this.baseDml.QueryInfCount(b.PatientInfo.ID);
                        if (infNum == -1)
                        {
                            s[105] = "0";//医院感染次数，不允许为空，否则影响报表统计结果 chengym
                        }
                        else
                        {
                            s[105] = infNum.ToString();
                        }
                    }
                    catch
                    {
                        s[105] = "0";
                    }
                    s[106] = "";//扩展1 
                    s[107] = "";
                    s[108] = "";
                    s[109] = "";
                    s[110] = "";
                    s[111] = "";
                    s[112] = "";
                    s[113] = "";//扩展8档案号
                    s[114] = "";//扩展9档案次数
                    s[115] = "";
                    s[116] = "";
                    s[117] = "";
                    s[118] = "";
                    s[119] = "";
                    s[120] = "";//扩展15
                    s[121] = b.PatientInfo.DIST;//籍贯
                    s[122] = b.CurrentAddr;//现住址
                    s[123] = b.CurrentPhone;//现电话
                    s[124] = b.CurrentZip;//现邮编
                    s[125] = b.PatientInfo.Profession.ID;//职业编号
                    //可能存在医院填写非数字内容
                    try
                    {
                        int bweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyBirthWeight);
                        b.BabyBirthWeight = bweight.ToString();
                    }
                    catch
                    {
                        b.BabyBirthWeight = "0";
                    }
                    s[126] = b.BabyBirthWeight;//新生儿出生体重
                    //可能存在医院填写非数字内容
                    try
                    {
                        int biweight = Neusoft.FrameWork.Function.NConvert.ToInt32(b.BabyInWeight);
                        b.BabyInWeight = biweight.ToString();
                    }
                    catch
                    {
                        b.BabyInWeight = "0";
                    }
                    s[127] = b.BabyInWeight;//新生儿入院体重
                    s[128] = b.InPath;//入院途径编号
                    s[129] = this.constMana.GetConstant("CASEINAVENUE", b.InPath).Name;//入院途径
                    s[130] = b.ClinicPath;//临床路径病例编号
                    if (b.ClinicPath == "1")
                    {
                        s[131] = "是";//临床路径病例
                    }
                    else
                    {
                        s[131] = "否";//临床路径病例
                    }
                    s[132] = "";//病理疾病编码
                    s[133] = b.PathNum;//病理号
                    //s[134] = b.AnaphyFlag;//是否药物过敏编号
                    //if (b.AnaphyFlag == "1")
                    //{
                    //    s[135] = "无";//是否药物过敏
                    //}
                    //else
                    //{
                    //    s[135] = "有";//是否药物过敏
                    //}
                    s[134] = "1";
                    s[135] = "无";
                    s[136] = this.ConverDoc(b.DutyNurse.ID).ID;//责任护士编号
                    s[137] = b.DutyNurse.Name;//责任护士
                    //s[138] = b.Out_Type;//离院方式编号
                    //if (b.Out_Type == "1")//离院方式
                    //{
                    //    s[138] = "1";
                    //    s[139] = "医嘱离院";
                    //}
                    //else if (b.Out_Type == "2")//离院方式
                    //{
                    //    s[138] = "2";
                    //    s[139] = "医嘱转院";
                    //}
                    //else if (b.Out_Type == "3")//离院方式
                    //{
                    //    s[138] = "3";
                    //    s[139] = "医嘱转社区乡镇卫生院";
                    //}
                    //else if (b.Out_Type == "4")//离院方式
                    //{
                    //    s[138] = "4";
                    //    s[139] = "非医嘱离院";
                    //}
                    //else if (b.Out_Type == "5")//离院方式
                    //{
                    //    s[138] = "5";
                    //    s[139] = "死亡";
                    //}
                    //else
                    //{
                    //    s[138] = "1";
                    //    s[139] = "医嘱离院";
                    //}
                    s[138] = "1";
                    s[139] = "医嘱离院";
                    s[140] = b.HighReceiveHopital;//离院方式为医嘱转院，拟接收医疗机构名称
                    s[141] = b.LowerReceiveHopital;//离院方式为转社区卫生服务器机构/乡镇卫生院，拟接收医疗机构名称
                    //s[142] = b.ComeBackInMonth;//是否有出院31天内再住院计划编号
                    //s[143] = "";//是否有出院31天内再住院计划
                    s[142] = "1";//是否有出院31天内再住院计划编号
                    s[143] = "无";//是否有出院31天内再住院计划
                    s[144] = b.ComeBackPurpose;//再住院目的
                    s[145] = b.OutComeDay.ToString();//颅脑损伤患者昏迷时间：入院前 天
                    s[146] = b.OutComeHour.ToString();//颅脑损伤患者昏迷时间：入院前 小时
                    s[147] = b.OutComeMin.ToString();//颅脑损伤患者昏迷时间：入院前 分钟
                    s[148] = (b.OutComeDay * 24 * 60 + b.OutComeHour * 60 + b.OutComeMin).ToString();//入院前昏迷总分钟(天、小时换算成分钟)
                    s[149] = b.InComeDay.ToString();//颅脑损伤患者昏迷时间：入院后 天
                    s[150] = b.InComeHour.ToString();//颅脑损伤患者昏迷时间：入院后 小时
                    s[151] = b.InComeMin.ToString();//颅脑损伤患者昏迷时间：入院后 分钟
                    s[152] = (b.InComeDay * 24 * 60 + b.InComeHour * 60 + b.InComeMin).ToString();//入院后昏迷总分钟
                    Neusoft.FrameWork.Models.NeuObject pactObj = this.constMana.GetConstant("CASEPACTCHANGE", b.PatientInfo.Pact.ID);
                    if (pactObj != null)
                    {
                        if (pactObj.Memo != "" && pactObj.Memo.ToUpper() != "TRUE")
                        {
                            if (pactObj.Memo == "9")
                            {
                                s[153] = "99";//付款方式编号
                            }
                            else
                            {
                                s[153] = pactObj.Memo.PadLeft(2, '0');//付款方式编号
                            }
                            s[154] = this.constMana.GetConstant("CASEPACT", pactObj.Memo).Name;//付款方式
                        }
                        else
                        {
                            s[153] = b.PatientInfo.Pact.ID;
                            s[154] = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID).Name;//付款方式
                        }
                    }
                    else
                    {
                        s[153] = b.PatientInfo.Pact.ID;
                        s[154] = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID).Name;//付款方式
                    }

                    if (Feeds == null || Feeds.Tables.Count == 0 || Feeds.Tables[0].Rows.Count == 0)
                    {
                        s[155] = "0.00";//住院总费用：自费金额
                        s[156] = "0.00";//综合医疗服务类：（1）一般医疗服务费
                        s[157] = "0.00";//综合医疗服务类：（2）一般治疗操作费
                        s[158] = "0.00";//综合医疗服务类：（3）护理费
                        s[159] = "0.00";//综合医疗服务类：（4）其他费用
                        s[160] = "0.00";//诊断类：(5) 病理诊断费
                        s[161] = "0.00";//诊断类：(6) 实验室诊断费
                        s[162] = "0.00";//诊断类：(7) 影像学诊断费
                        s[163] = "0.00";//诊断类：(8) 临床诊断项目费
                        s[164] = "0.00";//治疗类：(9) 非手术治疗项目费
                        s[165] = "0.00";//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = "0.00";//治疗类：(10) 手术治疗费
                        s[167] = "0.00";//治疗类：手术治疗费 其中麻醉费
                        s[168] = "0.00";//治疗类：手术治疗费 其中手术费
                        s[169] = "0.00";//康复类：(11) 康复费
                        s[170] = "0.00";//中医类：中医治疗类
                        s[171] = "0.00";//西药类： 西药费 其中抗菌药物费用
                        s[172] = "0.00";//血液和血液制品类： 血费
                        s[173] = "0.00";//血液和血液制品类： 白蛋白类制品费
                        s[174] = "0.00";//血液和血液制品类： 球蛋白制品费
                        s[175] = "0.00";//血液和血液制品类：凝血因子类制品费
                        s[176] = "0.00";//血液和血液制品类： 细胞因子类费
                        s[177] = "0.00";//耗材类：检查用一次性医用材料费
                        s[178] = "0.00";//耗材类：治疗用一次性医用材料费
                        s[179] = "0.00";//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    else
                    {
                        s[155] = Feeds.Tables[0].Rows[0][1].ToString();//自负金额
                        s[156] = Feeds.Tables[0].Rows[0][2].ToString();//综合医疗服务类：（1）一般医疗服务费
                        s[157] = Feeds.Tables[0].Rows[0][3].ToString();//综合医疗服务类：（2）一般治疗操作费
                        s[158] = Feeds.Tables[0].Rows[0][4].ToString();//综合医疗服务类：（3）护理费
                        s[159] = Feeds.Tables[0].Rows[0][5].ToString();//综合医疗服务类：（4）其他费用
                        s[160] = Feeds.Tables[0].Rows[0][6].ToString();//诊断类：(5) 病理诊断费
                        s[161] = Feeds.Tables[0].Rows[0][7].ToString();//诊断类：(6) 实验室诊断费
                        s[162] = Feeds.Tables[0].Rows[0][8].ToString();//诊断类：(7) 影像学诊断费
                        s[163] = Feeds.Tables[0].Rows[0][9].ToString();//诊断类：(8) 临床诊断项目费
                        s[164] = Feeds.Tables[0].Rows[0][10].ToString();//治疗类：(9) 非手术治疗项目费
                        s[165] = Feeds.Tables[0].Rows[0][11].ToString();//治疗类：非手术治疗项目费 其中临床物理治疗费
                        s[166] = Feeds.Tables[0].Rows[0][12].ToString();//治疗类：(10) 手术治疗费
                        s[167] = Feeds.Tables[0].Rows[0][13].ToString();//治疗类：手术治疗费 其中麻醉费
                        s[168] = Feeds.Tables[0].Rows[0][14].ToString();//治疗类：手术治疗费 其中手术费
                        s[169] = Feeds.Tables[0].Rows[0][15].ToString();//康复类：(11) 康复费
                        s[170] = Feeds.Tables[0].Rows[0][16].ToString();//中医类：中医治疗类
                        s[171] = Feeds.Tables[0].Rows[0][18].ToString();//西药类： 西药费 其中抗菌药物费用
                        s[172] = Feeds.Tables[0].Rows[0][21].ToString();//血液和血液制品类： 血费
                        s[173] = Feeds.Tables[0].Rows[0][22].ToString();//血液和血液制品类： 白蛋白类制品费
                        s[174] = Feeds.Tables[0].Rows[0][23].ToString();//血液和血液制品类： 球蛋白制品费
                        s[175] = Feeds.Tables[0].Rows[0][24].ToString();//血液和血液制品类：凝血因子类制品费
                        s[176] = Feeds.Tables[0].Rows[0][25].ToString();//血液和血液制品类： 细胞因子类费
                        s[177] = Feeds.Tables[0].Rows[0][26].ToString();//耗材类：检查用一次性医用材料费
                        s[178] = Feeds.Tables[0].Rows[0][27].ToString();//耗材类：治疗用一次性医用材料费
                        s[179] = Feeds.Tables[0].Rows[0][28].ToString();//耗材类：手术用一次性医用材料费
                        s[180] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治费（中医）
                        s[181] = "0.00";//综合医疗服务类：一般医疗服务费 其中中医辨证论治会诊费（中医）
                        s[182] = "0.00";//中医类：诊断（中医）
                        s[183] = "0.00";//中医类：治疗（中医）
                        s[184] = "0.00";//中医类：治疗 其中外治（中医）
                        s[185] = "0.00";//中医类：治疗 其中骨伤（中医）
                        s[186] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        s[187] = "0.00";//中医类：治疗推拿治疗（中医）
                        s[188] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        s[189] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        s[190] = "0.00";//中医类：其他（中医）
                        s[191] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        s[192] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        s[193] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                        //s[194] = "0.00";//中医类：治疗 其中骨伤（中医）
                        //s[195] = "0.00";//中医类：治疗 其中针刺与灸法（中医）
                        //s[196] = "0.00";//中医类：治疗推拿治疗（中医）
                        //s[197] = "0.00";//中医类：治疗 其中肛肠治疗（中医）
                        //s[198] = "0.00";//中医类：治疗 其中特殊治疗（中医）
                        //s[199] = "0.00";//中医类：其他（中医）
                        //s[200] = "0.00";//中医类：其他 其中中药特殊调配加工（中医）
                        //s[201] = "0.00";//中医类：其他 其中辨证施膳（中医）
                        //s[202] = "0.00";//中药类：中成药费 其中医疗机构中药制剂费（中医）
                    }
                    return s;
                }
                catch (Exception ex)
                {
                    this.Err = ex.ToString();
                    return null;
                }
                #endregion
            }
        }

        /// <summary>
        /// HIS_BA1 --病人住院信息
        /// </summary>
        /// <returns></returns>
        public string GetInsertHISBA1SQL(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            if (b == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }
            this.fid = this.GetCasUpload();


            string strReturn = string.Empty;
            if (fid == -1 || fid == 0)
            {
                #region sql
                strReturn = @"INSERT INTO tPatientVisit
  (
   FPRN,
   FTIMES,
   FICDVersion,
   FZYID,
   FAGE,
   FNAME,
   FSEXBH,
   FSEX,
   FBIRTHDAY,
   FBIRTHPLACE, --10
   FIDCard,
   fcountrybh,
   fcountry,
   fnationalitybh,
   fnationality,
   FJOB,
   FSTATUSBH,
   FSTATUS,
   FDWNAME,
   FDWADDR,
   FDWTELE,
   FDWPOST,
   FHKADDR,
   FHKPOST,
   FLXNAME,
   FRELATE,
   FLXADDR,
   FLXTELE,
   FFBBH,
   FFB,--30
   FASCARD1,
   FASCARD2,
   FRYDATE,
   FRYTIME,
   FRYTYKH,
   FRYDEPT,
   FRYBS,
   FCYDATE,
   FCYTIME,
   FCYTYKH,
   FCYDEPT,
   FCYBS,
   FDAYS,
   FMZZDBH,
   FMZZD,
   FMZDOCTBH,
   FMZDOCT,
   FRYINFOBH,
   FRYINFO,
   FRYZDBH,
   FRYZD,
   FQZDATE,
   FPHZD,
   FGMYW,
   FHBSAGBH,
   FHBSAG,
   FHCVABBH,
   FHCVAB,
   FHIVABBH,
   FHIVAB,
   FMZCYACCOBH,--60
   FMZCYACCO,
   FRYCYACCOBH,
   FRYCYACCO,
   FLCBLACCOBH,
   FLCBLACCO,
   FFSBLACCOBH,
   FFSBLACCO,
   FOPACCOBH,
   FOPACCO,
   FQJTIMES,
   FQJSUCTIMES,
   FKZRBH,
   FKZR,
   FZRDOCTBH,
   FZRDOCTOR,
   FZZDOCTBH,
   FZZDOCT,
   FZYDOCTBH,
   FZYDOCT,
   FJXDOCTBH,--80
   FJXDOCT,
   FYJSSXDOCTBH,
   FYJSSXDOCT,
   FSXDOCTBH,
   FSXDOCT,
   FBMYBH,
   FBMY,
   FZLRBH,
   FZLR,
   FQUALITYBH,
   FQUALITY,
   FZKDOCTBH,
   FZKDOCT,
   FZKNURSEBH,
   FZKNURSE,
   FZKRQ,
   FMZDEADBH,
   FMZDEAD,
   FSUM1,
   FCWF,--100
   FHLF,
   FXYF,
   FZYF,
   FZCHYF,
   FZCYF,
   FFSF,
   FHYF,
   FSYF,
   FSXF,
   FZLF,
   FSSF,
   FJSF,
   FJCF,
   FMZF,
   FYEF,
   FPCF,
   FQTF,
   FBODYBH,
   FBODY,
   FISOPFIRSTBH,--120
   FISOPFIRST,
   FISZLFIRSTBH,
   FISZLFIRST,
   FISJCFIRSTBH,
   FISJCFIRST,
   FISZDFIRSTBH,
   FISZDFIRST,
   FISSZBH,
   FISSZ,
   FSZQX,
   FSAMPLEBH,
   FSAMPLE,
   FBLOODBH,
   FBLOOD,
   FRHBH,
   FRH,
   FSXFYBH,
   FSXFY,
   FSYFYBH,
   FSYFY,--140
   FREDCELL,
   FPLAQUE,
   FSEROUS,
   FALLBLOOD,
   FOTHERBLOOD,
   FHZYJ,
   FHZYC,
   FHLTJ,
   FHL1,
   FHL2,
   FHL3,
   FHLZZ,
   FHLTS,
   FBABYNUM,
   FTWILL,
   FQJBR,
   FQJSUC,
   FTHREQZ,
   FBACK,
   FIFZDSS,--160
   FIFDBZ,
   FZLFZY,
   FZKTYKH,
   FZKDEPT,
   FZKDATE,
   FZKTIME,
   FSRYBH,
   FSRY,
   FWORKRQ,
   FJBFXBH,
   FJBFX,
   FFHGDBH,
   FFHGD,
   FSOURCEBH,
   FSOURCE,
   FIFSS,
   FIFFYK,
   FBFZ,
   FYNGR,
   FEXTEND1,--180
   FEXTEND2,
   FEXTEND3,
   FEXTEND4,
   FEXTEND5,
   FEXTEND6,
   FEXTEND7,
   FEXTEND8,
   FEXTEND9,
   FEXTEND10,
   FEXTEND11,
   FEXTEND12,
   FEXTEND13,
   FEXTEND14,
   FEXTEND15)
  VALUES
  (
'{1}',
{2},
{3},
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}',
'{28}',
'{29}',
'{30}',
'{31}',
'{32}',
'{33}',
'{34}',
'{35}',
'{36}',
'{37}',
'{38}',
'{39}',
'{40}',
'{41}',
'{42}',
{43},
'{44}',
'{45}',
'{46}',
'{47}',
'{48}',
'{49}',
'{50}',
'{51}',
'{52}',
'{53}',
'{54}',
'{55}',
'{56}',
'{57}',
'{58}',
'{59}',
'{60}',
'{61}',
'{62}',
'{63}',
'{64}',
'{65}',
'{66}',
'{67}',
'{68}',
'{69}',
'{70}',
{71},
{72},
'{73}',
'{74}',
'{75}',
'{76}',
'{77}',
'{78}',
'{79}',
'{80}',
'{81}',
'{82}',
'{83}',
'{84}',
'{85}',
'{86}',
'{87}',
'{88}',
'{89}',
'{90}',
'{91}',
'{92}',
'{93}',
'{94}',
'{95}',
'{96}',
'{97}',
'{98}',
'{99}',
{100},
{101},
{102},
{103},
{104},
{105},
{106},
{107},
{108},
{109},
{110},
{111},
{112},
{113},
{114},
{115},
{116},
{117},
{118},
'{119}',
'{120}',
'{121}',
'{122}',
'{123}',
'{124}',
'{125}',
'{126}',
'{127}',
'{128}',
'{129}',
'{130}',
'{131}',
'{132}',
'{133}',
'{134}',
'{135}',
'{136}',
'{137}',
'{138}',
'{139}',
'{140}',
'{141}',
{142},
{143},
{144},
{145},
{146},
{147},
{148},
{149},
{150},
{151},
{152},
{153},
{154},
{155},
{156},
{157},
{158},
{159},
{160},
{161},
{162},
{163},
'{164}',
'{165}',
'{166}',
'{167}',
'{168}',
'{169}',
'{170}',
'{171}',
'{172}',
'{173}',
'{174}',
'{175}',
'{176}',
{177},
{178},
{179},
{180},
'{181}',
'{182}',
'{183}',
'{184}',
'{185}',
'{186}',
'{187}',
'{188}',
'{189}',
'{190}',
'{191}',
'{192}',
'{193}',
'{194}',
'{195}'
)";
            }
            else
            {
                strReturn = @"INSERT INTO tPatientVisit
  (
  -- FID,
   FPRN,
   FTIMES,
   FICDVersion,
   FZYID,
   FAGE,
   FNAME,
   FSEXBH,
   FSEX,
   FBIRTHDAY,
   FBIRTHPLACE, --10
   FIDCard,
   fcountrybh,
   fcountry,
   fnationalitybh,
   fnationality,
   FJOB,
   FSTATUSBH,
   FSTATUS,
   FDWNAME,
   FDWADDR,
   FDWTELE,
   FDWPOST,
   FHKADDR,
   FHKPOST,
   FLXNAME,
   FRELATE,
   FLXADDR,
   FLXTELE,
   FFBBH,
   FFB,--30
   FASCARD1,
   FASCARD2,
   FRYDATE,
   FRYTIME,
   FRYTYKH,
   FRYDEPT,
   FRYBS,
   FCYDATE,
   FCYTIME,
   FCYTYKH,
   FCYDEPT,
   FCYBS,
   FDAYS,
   FMZZDBH,
   FMZZD,
   FMZDOCTBH,
   FMZDOCT,
   FRYINFOBH,
   FRYINFO,
   FRYZDBH,
   FRYZD,
   FQZDATE,
   FPHZD,
   FGMYW,
   FHBSAGBH,
   FHBSAG,
   FHCVABBH,
   FHCVAB,
   FHIVABBH,
   FHIVAB,
   FMZCYACCOBH,--60
   FMZCYACCO,
   FRYCYACCOBH,
   FRYCYACCO,
   FLCBLACCOBH,
   FLCBLACCO,
   FFSBLACCOBH,
   FFSBLACCO,
   FOPACCOBH,
   FOPACCO,
   FQJTIMES,
   FQJSUCTIMES,
   FKZRBH,
   FKZR,
   FZRDOCTBH,
   FZRDOCTOR,
   FZZDOCTBH,
   FZZDOCT,
   FZYDOCTBH,
   FZYDOCT,
   FJXDOCTBH,--80
   FJXDOCT,
   FYJSSXDOCTBH,
   FYJSSXDOCT,
   FSXDOCTBH,
   FSXDOCT,
   FBMYBH,
   FBMY,
   FZLRBH,
   FZLR,
   FQUALITYBH,
   FQUALITY,
   FZKDOCTBH,
   FZKDOCT,
   FZKNURSEBH,
   FZKNURSE,
   FZKRQ,
   FMZDEADBH,
   FMZDEAD,
   FSUM1,
   FCWF,--100
   FHLF,
   FXYF,
   FZYF,
   FZCHYF,
   FZCYF,
   FFSF,
   FHYF,
   FSYF,
   FSXF,
   FZLF,
   FSSF,
   FJSF,
   FJCF,
   FMZF,
   FYEF,
   FPCF,
   FQTF,
   FBODYBH,
   FBODY,
   FISOPFIRSTBH,--120
   FISOPFIRST,
   FISZLFIRSTBH,
   FISZLFIRST,
   FISJCFIRSTBH,
   FISJCFIRST,
   FISZDFIRSTBH,
   FISZDFIRST,
   FISSZBH,
   FISSZ,
   FSZQX,
   FSAMPLEBH,
   FSAMPLE,
   FBLOODBH,
   FBLOOD,
   FRHBH,
   FRH,
   FSXFYBH,
   FSXFY,
   FSYFYBH,
   FSYFY,--140
   FREDCELL,
   FPLAQUE,
   FSEROUS,
   FALLBLOOD,
   FOTHERBLOOD,
   FHZYJ,
   FHZYC,
   FHLTJ,
   FHL1,
   FHL2,
   FHL3,
   FHLZZ,
   FHLTS,
   FBABYNUM,
   FTWILL,
   FQJBR,
   FQJSUC,
   FTHREQZ,
   FBACK,
   FIFZDSS,--160
   FIFDBZ,
   FZLFZY,
   FZKTYKH,
   FZKDEPT,
   FZKDATE,
   FZKTIME,
   FSRYBH,
   FSRY,
   FWORKRQ,
   FJBFXBH,
   FJBFX,
   FFHGDBH,
   FFHGD,
   FSOURCEBH,
   FSOURCE,
   FIFSS,
   FIFFYK,
   FBFZ,
   FYNGR,
   FEXTEND1,--180
   FEXTEND2,
   FEXTEND3,
   FEXTEND4,
   FEXTEND5,
   FEXTEND6,
   FEXTEND7,
   FEXTEND8,
   FEXTEND9,
   FEXTEND10,
   FEXTEND11,
   FEXTEND12,
   FEXTEND13,
   FEXTEND14,
   FEXTEND15)
  VALUES
  (
--'{197}',
'{1}',
{2},
{3},
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}',
'{28}',
'{29}',
'{30}',
'{31}',
'{32}',
'{33}',
'{34}',
'{35}',
'{36}',
'{37}',
'{38}',
'{39}',
'{40}',
'{41}',
'{42}',
{43},
'{44}',
'{45}',
'{46}',
'{47}',
'{48}',
'{49}',
'{50}',
'{51}',
'{52}',
'{53}',
'{54}',
'{55}',
'{56}',
'{57}',
'{58}',
'{59}',
'{60}',
'{61}',
'{62}',
'{63}',
'{64}',
'{65}',
'{66}',
'{67}',
'{68}',
'{69}',
'{70}',
{71},
{72},
'{73}',
'{74}',
'{75}',
'{76}',
'{77}',
'{78}',
'{79}',
'{80}',
'{81}',
'{82}',
'{83}',
'{84}',
'{85}',
'{86}',
'{87}',
'{88}',
'{89}',
'{90}',
'{91}',
'{92}',
'{93}',
'{94}',
'{95}',
'{96}',
'{97}',
'{98}',
'{99}',
{100},
{101},
{102},
{103},
{104},
{105},
{106},
{107},
{108},
{109},
{110},
{111},
{112},
{113},
{114},
{115},
{116},
{117},
{118},
'{119}',
'{120}',
'{121}',
'{122}',
'{123}',
'{124}',
'{125}',
'{126}',
'{127}',
'{128}',
'{129}',
'{130}',
'{131}',
'{132}',
'{133}',
'{134}',
'{135}',
'{136}',
'{137}',
'{138}',
'{139}',
'{140}',
'{141}',
{142},
{143},
{144},
{145},
{146},
{147},
{148},
{149},
{150},
{151},
{152},
{153},
{154},
{155},
{156},
{157},
{158},
{159},
{160},
{161},
{162},
{163},
'{164}',
'{165}',
'{166}',
'{167}',
'{168}',
'{169}',
'{170}',
'{171}',
'{172}',
'{173}',
'{174}',
'{175}',
'{176}',
{177},
{178},
{179},
{180},
'{181}',
'{182}',
'{183}',
'{184}',
'{185}',
'{186}',
'{187}',
'{188}',
'{189}',
'{190}',
'{191}',
'{192}',
'{193}',
'{194}',
'{195}'
)";
                #endregion
            }
            try
            {

                strReturn = string.Format(strReturn, this.GetBaseInfoBA1(b, alFee, alChangeDepe, alDose, isMetCasBase));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        /// <summary>
        /// 将病案首页基本信息实体 转变成字符串数组
        /// </summary>
        /// <param name="b">病案的实体类</param>
        /// <param name="alFee"></param>
        /// <param name="alChangeDepe"></param>
        /// <param name="alDose"></param>
        /// <returns>失败返回null</returns>
        public string[] GetBaseInfoBA1(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase)
        {
            if (!isMetCasBase)//首页程序限制了该字段必填 故根据判断为空则 从主表获取的，用回原来的
            {
                #region 屏蔽了吧 直接修改前面的数据获取方式 --病案首页已经全部上线了2011-3-24 ch
                //baseObj = new Neusoft.HISFC.Models.HealthRecord.Base();
                ArrayList alDiagnose = this.baseDml.QueryCaseDiagnoseByInpatientNo(b.PatientInfo.ID);
                if (this.fid == -1 || this.fid == 0)
                {
                    string[] s = new string[197];
                    try
                    {
                        s[0] = "0";
                        s[1] = b.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());//病案号
                        s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
                        s[3] = "10";
                        s[4] = b.PatientInfo.ID;
                        s[5] = b.PatientInfo.Age;//年龄
                        s[6] = b.PatientInfo.Name;//姓名
                        if (b.PatientInfo.Sex.ID.ToString() == "M")
                        {
                            s[7] = "1";
                            s[8] = "男";
                        }
                        else
                        {
                            s[7] = "2";
                            s[8] = "女";
                        }
                        s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');//出生日期
                        s[10] = b.PatientInfo.AreaCode; //出生地
                        s[11] = b.PatientInfo.IDCard;//身份证号
                        if (b.PatientInfo.Country.ID != null)
                        {
                            if (b.PatientInfo.Country.ID.ToString() == "1")//中国  需要转换其他
                            {
                                s[12] = "A156";
                                s[13] = "中国";
                            }
                            else
                            {
                                Neusoft.FrameWork.Models.NeuObject countryObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                                if (countryObj != null && countryObj.ID != "")
                                {
                                    if (countryObj.Memo != "")
                                    {
                                        s[12] = countryObj.Memo.ToString(); //国籍编号
                                        s[13] = countryObj.Name.ToString(); //国籍
                                    }
                                    else
                                    {
                                        s[12] = countryObj.ID.ToString(); //国籍编号
                                        s[13] = countryObj.Name.ToString(); //国籍
                                    }
                                }
                                else
                                {
                                    s[12] = b.PatientInfo.Country.ID.ToString(); //国籍编号
                                    s[13] = ""; //国籍
                                }
                            }
                        }
                        else
                        {
                            s[12] = "A156";
                            s[13] = "中国";
                        }
                        Neusoft.FrameWork.Models.NeuObject NationObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                        if (NationObj != null && NationObj.ID != "")
                        {
                            if (NationObj.Memo != "")
                            {
                                s[14] = NationObj.Memo; //民族编号
                                s[15] = NationObj.Name; //民族
                            }
                            else
                            {
                                s[14] = NationObj.ID; //民族编号
                                s[15] = NationObj.Name; //民族
                            }
                        }
                        else
                        {
                            s[14] = b.PatientInfo.Nationality.ID; //民族编号
                            s[15] = ""; //民族
                        }
                        //add by chengym 2011-6-15  字典表的名称字段varchar（50） 有些执业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                        Neusoft.FrameWork.Models.NeuObject JobObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.PROFESSION, b.PatientInfo.Profession.ID.ToString());
                        if (JobObj != null && JobObj.ID != "")
                        {
                            if (JobObj.Memo != "")
                            {
                                if (JobObj.Memo.Length <= 100)
                                {
                                    s[16] = JobObj.Memo;
                                }
                                else
                                {
                                    s[16] = JobObj.Memo.Substring(0, 100);
                                }
                            }
                            else
                            {
                                if (JobObj.Name.Length <= 100)
                                {
                                    s[16] = JobObj.Name;
                                }
                                else
                                {
                                    s[16] = JobObj.Name.Substring(0, 100);
                                }
                            }
                        }
                        else
                        {
                            s[16] = b.PatientInfo.Profession.ID; //职业 没有传中文不知道是否可以
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1")
                        {
                            s[17] = "1"; //婚姻状况编号
                            s[18] = "未婚"; //婚姻状况
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2")
                        {
                            s[17] = "2";
                            s[18] = "已婚";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "3")
                        {
                            s[17] = "3";
                            s[18] = "离婚";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4")
                        {
                            s[17] = "5";
                            s[18] = "其他";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "R" || b.PatientInfo.MaritalStatus.ID.ToString() == "5")
                        {
                            s[17] = "5";
                            s[18] = "其他";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                        {
                            s[17] = "5";
                            s[18] = "其他";
                        }

                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "6")
                        {
                            s[17] = "4";
                            s[18] = "丧偶";
                        }

                        s[19] = b.PatientInfo.AddressBusiness;  //工作单位及地址
                        s[20] = b.PatientInfo.CompanyName;//单位地址
                        s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                        s[22] = b.PatientInfo.BusinessZip;//单位邮编      
                        s[23] = b.PatientInfo.AddressHome;//家庭住址
                        s[24] = b.PatientInfo.HomeZip;//住址邮编
                        s[25] = b.PatientInfo.Kin.Name;//联系人
                        Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.Relation.ID);
                        if (RelativeObj != null && RelativeObj.ID != "")
                        {
                            if (RelativeObj.Memo != "")
                            {
                                if (RelativeObj.Memo.Length <= 20)
                                {
                                    s[26] = RelativeObj.Memo;//与患者关系
                                }
                                else
                                {
                                    s[26] = RelativeObj.Memo.Substring(0, 20);//与患者关系
                                }
                            }
                            else
                            {
                                if (RelativeObj.Name.Length <= 20)
                                {
                                    s[26] = RelativeObj.Name;//与患者关系
                                }
                                else
                                {
                                    s[26] = RelativeObj.Name.Substring(0, 20);//与患者关系
                                }
                            }
                        }
                        else
                        {
                            s[26] = b.PatientInfo.Kin.Name;//与患者关系
                        }
                        s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                        s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话

                        #region 医疗付款方式
                        if (b.PatientInfo.Pact.PayKind.ID == "DRGS")
                        {
                            Neusoft.FrameWork.Models.NeuObject pactObj = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID);
                            if (pactObj != null)
                            {
                                if (pactObj.Memo != "")
                                {
                                    s[29] = pactObj.Memo;
                                    s[30] = pactObj.Name;
                                    s[31] = b.PatientInfo.SSN;
                                }
                                else
                                {
                                    s[29] = b.PatientInfo.Pact.ID;
                                    s[30] = pactObj.Name;
                                    s[31] = b.PatientInfo.SSN;
                                }
                            }
                        }
                        else
                        {
                            if (b.PatientInfo.Pact.PayKind.ID == "01")
                            {
                                s[31] = b.PatientInfo.SSN;
                                s[29] = "3";
                                s[30] = "自费医疗";
                            }
                            else if (b.PatientInfo.Pact.PayKind.ID == "02")//医保
                            {
                                s[31] = b.PatientInfo.SSN;
                                s[29] = "1";
                                s[30] = "社会基本医疗保险";
                            }
                            else if (b.PatientInfo.Pact.PayKind.ID == "03")
                            {
                                s[29] = "4";
                                s[31] = b.PatientInfo.SSN;
                                s[30] = "公费医疗";
                            }
                            else
                            {
                                s[29] = "6";
                                s[31] = b.PatientInfo.SSN;
                                s[30] = "其他";
                            }
                        }
                        #endregion
                        s[32] = b.PatientInfo.SSN; //其他医疗保险卡号
                        s[33] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                        s[34] = b.PatientInfo.PVisit.InTime.Hour.ToString().PadLeft(2, '0'); //入院时间

                        Neusoft.HISFC.Models.RADT.Location indept = this.baseDml.GetDeptIn(b.PatientInfo.ID);
                        if (indept != null) //入院科室 
                        {
                            s[35] = this.ConverDept(indept.Dept.ID).ID;//入院科室代码
                            s[36] = this.ConverDept(indept.Dept.ID).Name;//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//出院科室代码
                            s[41] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//出院科室名称
                        }
                        else
                        {
                            s[35] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//入院科室代码
                            s[36] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//出院科室代码
                            s[41] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//出院科室名称
                        }
                        s[38] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                        s[39] = b.PatientInfo.PVisit.OutTime.Hour.ToString().PadLeft(2, '0');
                        System.TimeSpan tt = b.PatientInfo.PVisit.OutTime - b.PatientInfo.PVisit.InTime;
                        s[43] = tt.Days.ToString();//住院天数
                        s[44] = b.ClinicDiag.ID;
                        if (b.ClinicDiag.Name.Length > 50)
                        {
                            s[45] = this.ChangeCharacter(b.ClinicDiag.Name.Substring(0, 50).ToString());
                        }
                        else
                        {
                            s[45] = this.ChangeCharacter(b.ClinicDiag.Name);
                        }
                        s[46] = b.ClinicDoc.ID;
                        s[47] = b.ClinicDoc.Name;
                        Neusoft.FrameWork.Models.NeuObject inCircs = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCIRCS, b.PatientInfo.PVisit.Circs.ID);
                        if (inCircs != null && inCircs.ID != "")
                        {
                            if (inCircs.Memo != "")
                            {
                                s[48] = inCircs.Memo.Trim();
                                if (inCircs.Memo.Trim() == "1")
                                {
                                    s[49] = "危";
                                }
                                else if (inCircs.Memo.Trim() == "2")
                                {
                                    s[49] = "急";
                                }
                                else if (inCircs.Memo.Trim() == "3")
                                {
                                    s[49] = "一般";
                                }
                            }
                        }
                        else
                        {
                            s[48] = b.PatientInfo.PVisit.Circs.ID;//入院情况
                            s[49] = this.constMana.GetConstant("INCIRCS", b.PatientInfo.PVisit.Circs.ID).Name;
                        }
                        s[50] = b.InHospitalDiag.ID;

                        if (b.DiagDate < new DateTime(1900, 1, 1))
                        {
                            s[52] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');
                        }
                        else if (b.DiagDate >= b.PatientInfo.PVisit.OutTime)//确诊日期大于等于出院日期  按入院日期获取
                        {
                            s[52] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');
                        }
                        else
                        {
                            s[52] = b.DiagDate.ToShortDateString().Replace('-', '/');
                            //确诊日期
                        }


                        s[37] = "";//入院病室               
                        s[42] = ""; //出院病室


                        if (b.InHospitalDiag.Name.Length > 50)
                        {
                            s[51] = this.ChangeCharacter(b.InHospitalDiag.Name.Substring(0, 50).ToString());
                        }
                        else
                        {
                            s[51] = this.ChangeCharacter(b.InHospitalDiag.Name);
                        }

                        s[156] = "0";
                        if (alDose != null)
                        {
                            foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose obj in alDose)
                            {
                                if (obj.DiagInfo.DiagType.ID == "1")
                                {
                                    if (obj.DiagInfo.ICD10.ID.Contains("B15") || obj.DiagInfo.ICD10.ID.Contains("B16") || obj.DiagInfo.ICD10.ID.Contains("B17") || obj.DiagInfo.ICD10.ID.Contains("B18") || obj.DiagInfo.ICD10.ID.Contains("B19"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("A15.0") || obj.DiagInfo.ICD10.ID.Contains("A15.1") || obj.DiagInfo.ICD10.ID.Contains("A15.2") || obj.DiagInfo.ICD10.ID.Contains("A16.0") || obj.DiagInfo.ICD10.ID.Contains("A15.0") || obj.DiagInfo.ICD10.ID.Contains("A16.1") || obj.DiagInfo.ICD10.ID.Contains("A16.2"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I21"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I50.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("J13") || obj.DiagInfo.ICD10.ID.Contains("J14") || obj.DiagInfo.ICD10.ID.Contains("J15"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I27.9"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K92.208"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("N04"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("E05"))
                                    {
                                        s[156] = "1";

                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I61"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I63"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("D60") || obj.DiagInfo.ICD10.ID.Contains("D61"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C91.0") || obj.DiagInfo.ICD10.ID.Contains("C92.0") || obj.DiagInfo.ICD10.ID.Contains("C93.0") || obj.DiagInfo.ICD10.ID.Contains("C94.0") || obj.DiagInfo.ICD10.ID.Contains("C95.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("E04"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K35"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K81.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K40"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C16"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C34.1") || obj.DiagInfo.ICD10.ID.Contains("C34.2") || obj.DiagInfo.ICD10.ID.Contains("C34.3") || obj.DiagInfo.ICD10.ID.Contains("C34.4") || obj.DiagInfo.ICD10.ID.Contains("C34.5") || obj.DiagInfo.ICD10.ID.Contains("C34.6") || obj.DiagInfo.ICD10.ID.Contains("C34.7") || obj.DiagInfo.ICD10.ID.Contains("C34.8") || obj.DiagInfo.ICD10.ID.Contains("C34.901"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C15"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("36.1"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C67"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("N40"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("S06"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("M51.202"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("J18.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("A04.903"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("D25"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("74"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("H25"))
                                    {
                                        s[156] = "1";
                                    }
                                    break;
                                }
                            }
                        }
                        if (alDiagnose != null)
                        {
                            if (alDiagnose.Count > 0)
                            {
                                foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose diagnoseObj in alDiagnose)
                                {
                                    if (diagnoseObj.MainFlag.Equals("6"))
                                    {
                                        s[53] = this.ChangeCharacter(diagnoseObj.DiagInfo.ICD10.Name);//病理诊断
                                        break;
                                    }
                                }

                            }
                        }
                        s[54] = b.FirstAnaphyPharmacy.Name;//药物过敏 
                        if (b.Hbsag == null)
                        {
                            s[55] = "0";
                        }
                        else
                        {
                            s[55] = b.Hbsag;
                        }
                        s[56] = this.constMana.GetConstant("HbsAg", b.Hbsag).Name;
                        if (b.HcvAb == null)
                        {
                            s[57] = "0";
                        }
                        else
                        {
                            s[57] = b.HcvAb;
                        }
                        s[58] = this.constMana.GetConstant("HbsAg", b.HcvAb).Name;
                        if (b.HivAb == null)
                        {
                            s[59] = "0";
                        }
                        else
                        {
                            s[59] = b.HivAb;
                        }
                        s[60] = this.constMana.GetConstant("HbsAg", b.HivAb).Name;
                        if (b.CePi == null)
                        {
                            s[61] = "1";
                        }
                        else
                        {
                            s[61] = b.CePi;
                        }
                        s[62] = this.constMana.GetConstant("CASEYSEORNO", b.CePi).Name;
                        if (b.PiPo == null)
                        {
                            s[63] = "1";
                        }
                        else
                        {
                            s[63] = b.PiPo;
                        }
                        s[64] = this.constMana.GetConstant("CASEYSEORNO", b.PiPo).Name;
                        s[65] = "0";
                        s[66] = this.constMana.GetConstant("CASEYSEORNO", b.ClPa).Name;
                        s[67] = "0";
                        s[68] = this.constMana.GetConstant("CASEYSEORNO", b.FsBl).Name;
                        s[69] = "0";
                        s[70] = this.constMana.GetConstant("CASEYSEORNO", b.OpbOpa).Name;


                        s[71] = b.SalvTimes.ToString();//抢救次数
                        s[72] = b.SuccTimes.ToString();//成功次数
                        s[73] = b.PatientInfo.PVisit.ReferringDoctor.ID;
                        s[74] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
                        s[75] = b.PatientInfo.PVisit.ConsultingDoctor.ID;
                        s[76] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
                        s[77] = b.PatientInfo.PVisit.AttendingDoctor.ID;//主治医师姓名
                        s[78] = b.PatientInfo.PVisit.AttendingDoctor.Name;
                        s[79] = b.PatientInfo.PVisit.AdmittingDoctor.ID;//住院医师姓名
                        s[80] = b.PatientInfo.PVisit.AdmittingDoctor.Name;
                        s[81] = b.RefresherDoc.ID;//进修医生
                        s[82] = b.RefresherDoc.Name;
                        s[83] = b.GraduateDoc.ID;//研究生实习医师名称
                        s[84] = b.GraduateDoc.Name;
                        s[85] = b.PatientInfo.PVisit.TempDoctor.ID;
                        s[86] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                        s[87] = b.CodingOper.ID;//编码员名称
                        s[88] = b.CodingOper.Name;
                        s[89] = b.OperInfo.ID;
                        s[90] = b.OperInfo.Name;//操作员名称（病案整理者）
                        s[91] = "1";//1";//b.MrQuality;//病案质量 
                        s[92] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                        s[93] = b.QcDoc.ID;//质控医师名称
                        s[94] = b.QcDoc.Name;
                        s[95] = b.QcNurse.ID;
                        s[96] = b.QcNurse.Name;//质控护士名称

                        if (b.CheckDate < new DateTime(1900, 1, 1))
                        {
                            s[97] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                        }
                        else if (b.CheckDate <= b.PatientInfo.PVisit.OutTime)//质控日期不可能小于出院日期
                        {
                            s[97] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                        }
                        else
                        {
                            s[97] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
                        }

                        s[98] = "";//是否因麻醉死亡编号
                        s[99] = "";//是否因麻醉死亡


                        for (int j = 100; j <= 118; j++)
                        {
                            s[j] = "0.00";
                        }

                        decimal feeTot = 0.0M;
                        decimal feeOther = 0.0M;
                        foreach (Neusoft.HISFC.Models.RADT.Patient feeInfo in alFee)
                        {
                            decimal fee1 = 0.0M;
                            fee1 = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeInfo.IDCard), 2);

                            feeTot += fee1;

                            string fee = fee1.ToString();

                            #region  正常定义病案费用表达式的情况  南庄等几家医院没有定义使用了fp表达式 晕了
                            if (feeInfo.DIST.TrimStart('0') == "1")//床位费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[101]) + fee1;
                                s[101] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "2")//护理费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[102]) + fee1;
                                s[102] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "3")//西药费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[103]) + fee1;
                                s[103] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "4")//中成药费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + fee1;
                                s[105] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "5")//中草药费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]) + fee1;
                                s[106] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "6")//放射费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[107]) + fee1;
                                s[107] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "7")//化验费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[108]) + fee1;
                                s[108] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "8")//输氧费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[109]) + fee1;
                                s[109] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "9")//输血费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[110]) + fee1;
                                s[110] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "10")//诊疗费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[111]) + fee1;
                                s[111] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "11")//手术费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[112]) + fee1;
                                s[112] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "12")//接生费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[113]) + fee1;
                                s[113] = temp.ToString();//接生费
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "13")//检查费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[114]) + fee1;
                                s[114] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "14")//麻醉费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[115]) + fee1;
                                s[115] = temp.ToString();
                            }
                            else
                            {
                                feeOther += fee1;
                            }
                            #endregion
                        }
                        s[100] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeTot), 2).ToString();
                        decimal tempZYF = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]);
                        s[104] = tempZYF.ToString(); //中药费

                        s[116] = "0.00";//婴儿费
                        s[117] = "0.00";//陪床费
                        s[118] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeOther), 2).ToString();


                        s[119] = b.CadaverCheck;//尸检
                        s[120] = this.constMana.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
                        s[121] = b.YnFirst;
                        s[122] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        s[123] = b.YnFirst;
                        s[124] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        s[125] = b.YnFirst;
                        s[126] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        s[127] = b.YnFirst;
                        s[128] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        if (b.VisiStat == "0")
                        {
                            b.VisiStat = "2";
                        }
                        s[129] = b.VisiStat;
                        s[130] = this.constMana.GetConstant("CASEYSEORNO", b.VisiStat).Name;
                        //s[131] = b.VisiPeriodWeek;
                        if (b.VisiPeriodYear.ToString() != "0") //随诊期限
                        {
                            s[131] = "Y" + b.VisiPeriodYear.ToString();
                        }
                        else if (b.VisiPeriodMonth.ToString() != "0")
                        {
                            s[131] = "M" + b.VisiPeriodMonth.ToString();

                        }
                        else if (b.VisiPeriodWeek.ToString() != "0")
                        {
                            try
                            {
                                System.Convert.ToInt32(b.VisiPeriodWeek);
                                s[131] = "W" + b.VisiPeriodWeek.ToString();
                            }
                            catch
                            {
                                s[131] = b.VisiPeriodWeek.ToString();
                            }
                        }
                        else
                        {
                            s[131] = "";
                        }

                        if (b.TechSerc == "0")
                        {
                            b.TechSerc = "2";
                        }
                        s[132] = b.TechSerc;//示教科研
                        s[133] = this.constMana.GetConstant("CASEYSEORNO", b.TechSerc).Name;
                        if (b.PatientInfo.BloodType.ID.ToString() == "A")
                        {
                            s[134] = "1";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "B")
                        {
                            s[134] = "2";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "AB")
                        {
                            s[134] = "3";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                        {
                            s[134] = "4";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                        {
                            s[134] = "9";
                            s[135] = "未查";
                        }
                        else
                        {
                            s[134] = "5";
                            s[135] = "其他";
                        }
                        s[136] = b.RhBlood;
                        s[137] = this.constMana.GetConstant("RHSTATE", b.RhBlood).Name;
                        s[138] = b.ReactionBlood;
                        s[139] = this.constMana.GetConstant("CASEYSEORNO", b.ReactionBlood).Name;
                        try
                        {
                            s[142] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodRed).ToString();//红细胞数
                        }
                        catch
                        {
                        }

                        try
                        {
                            s[143] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodPlatelet).ToString();//血小板数

                        }
                        catch
                        {
                        }

                        try
                        {
                            s[144] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BodyAnotomize).ToString();//血浆数
                        }
                        catch
                        {
                        }

                        try
                        {
                            s[145] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodWhole).ToString();//全血数

                        }
                        catch
                        {
                        }

                        try
                        {
                            s[146] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodOther).ToString();//其他输血数

                        }
                        catch
                        {
                        }


                        s[140] = "";//输液反应编码
                        s[141] = "";//输液反应


                        s[147] = b.InconNum.ToString();//院际会诊次数 70 远程会诊次数
                        s[148] = b.OutconNum.ToString();//院际会诊次数 70 远程会诊次数
                        s[149] = b.SuperNus.ToString(); //特级护理时间(小时)                         
                        s[150] = b.INus.ToString(); //I级护理时间(日)                                     
                        s[151] = b.IINus.ToString(); //II级护理时间(日)                                    
                        s[152] = b.IIINus.ToString(); //III级护理时间(日)                                   
                        s[153] = b.StrictNuss.ToString(); //重症监护时间( 小时)                               
                        s[154] = b.SpecalNus.ToString();  // 特殊护理(日) 
                        if (b.SalvTimes > 0)
                        {
                            s[157] = "1";
                            s[158] = "1";
                        }
                        else
                        {
                            s[157] = "0";
                            s[158] = "0";
                        }
                        s[162] = "0";//是否单病种




                        s[155] = "0.00";//婴儿数
                        s[156] = "0";//是否部分病种


                        s[159] = "0";//是否三日确诊
                        s[160] = "0";//是否月内再次住院
                        s[161] = "0";//是否中度烧伤

                        s[163] = "0.00";//中医院治疗费(预留字段)

                        if (alChangeDepe.Count > 0)
                        {
                            Neusoft.HISFC.Models.RADT.Location dept = alChangeDepe[0] as Neusoft.HISFC.Models.RADT.Location;
                            s[164] = this.ConverDept(dept.Dept.ID).ID;
                            s[165] = dept.Dept.Name;
                            s[166] = Neusoft.FrameWork.Function.NConvert.ToDateTime(dept.Dept.Memo).ToShortDateString().Replace('-', '/');
                        }
                        else
                        {
                            s[164] = "";
                            s[165] = "";
                            s[166] = "";
                        }
                        try
                        {

                        }
                        catch
                        {
                        }
                        s[167] = "";
                        s[168] = "";
                        s[169] = "";
                        s[170] = this.GetDateTimeFromSysDateTime().ToShortDateString().Replace('-', '/');
                        s[171] = "";
                        s[172] = "";
                        s[173] = "";
                        s[174] = "";
                        //s[175] = b.PatientInfo.PVisit.InSource.ID;//入院来源
                        //s[176] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;
                        Neusoft.FrameWork.Models.NeuObject inSource = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INAVENUE, b.PatientInfo.PVisit.InSource.ID);
                        if (inSource != null && inSource.ID != "")
                        {
                            if (inSource.Memo != "")
                            {
                                s[175] = inSource.Memo.Trim();
                                if (inSource.Memo.Trim() == "1")
                                {
                                    s[176] = "医院所在区（县）";
                                }
                                else if (inSource.Memo.Trim() == "2")
                                {
                                    s[176] = "医院所在市的外区（县）";
                                }
                                else if (inSource.Memo.Trim() == "3")
                                {
                                    s[176] = "本省其他市";
                                }
                                else if (inSource.Memo.Trim() == "4")
                                {
                                    s[176] = "外省（直辖市）";
                                }
                            }
                        }
                        else
                        {
                            s[175] = b.PatientInfo.PVisit.InSource.ID;//入院来源
                            s[176] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;
                        }
                        if (b.FirstOperation.ID.Trim() == "")
                        {
                            s[177] = "0";
                        }
                        else
                        {
                            s[177] = "1";
                        }
                        s[178] = "0";
                        if (b.SyndromeFlag == null || b.SyndromeFlag == "")
                        {
                            s[179] = "0";
                        }
                        else
                        {
                            s[179] = b.SyndromeFlag;
                        }
                        s[180] = b.InfectionNum.ToString();
                        s[181] = "";
                        s[182] = "";
                        s[183] = "";
                        s[184] = "";
                        s[185] = "";
                        s[186] = "";
                        s[187] = "";
                        s[188] = "";
                        s[189] = "";
                        s[190] = "";
                        s[191] = "";
                        s[192] = "";
                        s[193] = "";
                        s[194] = "";
                        s[195] = "";
                        s[196] = "0";//是否输入
                        return s;

                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else
                {
                    string[] s = new string[198];
                    try
                    {
                        s[197] = this.fid.ToString();
                        s[0] = "0";
                        s[1] = b.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());//病案号
                        s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
                        s[3] = "10";
                        s[4] = b.PatientInfo.ID;
                        s[5] = b.PatientInfo.Age;//年龄
                        s[6] = b.PatientInfo.Name;//姓名
                        if (b.PatientInfo.Sex.ID.ToString() == "M")
                        {
                            s[7] = "1";
                            s[8] = "男";
                        }
                        else
                        {
                            s[7] = "2";
                            s[8] = "女";
                        }
                        s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');//出生日期
                        s[10] = b.PatientInfo.AreaCode; //出生地
                        s[11] = b.PatientInfo.IDCard;//身份证号
                        if (b.PatientInfo.Country.ID != null)
                        {
                            if (b.PatientInfo.Country.ID.ToString() == "1")//中国  需要转换其他
                            {
                                s[12] = "A156";
                                s[13] = "中国";
                            }
                            else
                            {
                                Neusoft.FrameWork.Models.NeuObject countryObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                                if (countryObj != null && countryObj.ID != "")
                                {
                                    if (countryObj.Memo != "")
                                    {
                                        s[12] = countryObj.Memo.ToString(); //国籍编号
                                        s[13] = countryObj.Name.ToString(); //国籍
                                    }
                                    else
                                    {
                                        s[12] = countryObj.ID.ToString(); //国籍编号
                                        s[13] = countryObj.Name.ToString(); //国籍
                                    }
                                }
                                else
                                {
                                    s[12] = b.PatientInfo.Country.ID.ToString(); //国籍编号
                                    s[13] = ""; //国籍
                                }
                            }
                        }
                        else
                        {
                            s[12] = "A156";
                            s[13] = "中国";
                        }
                        Neusoft.FrameWork.Models.NeuObject NationObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                        if (NationObj != null && NationObj.ID != "")
                        {
                            if (NationObj.Memo != "")
                            {
                                s[14] = NationObj.Memo; //民族编号
                                s[15] = NationObj.Name; //民族
                            }
                            else
                            {
                                s[14] = NationObj.ID; //民族编号
                                s[15] = NationObj.Name; //民族
                            }
                        }
                        else
                        {
                            s[14] = b.PatientInfo.Nationality.ID; //民族编号
                            s[15] = ""; //民族
                        }
                        //add by chengym 2011-6-15  字典表的名称字段varchar（50） 有些执业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                        Neusoft.FrameWork.Models.NeuObject JobObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.PROFESSION, b.PatientInfo.Profession.ID.ToString());
                        if (JobObj != null && JobObj.ID != "")
                        {
                            if (JobObj.Memo != "")
                            {
                                if (JobObj.Memo.Length <= 100)
                                {
                                    s[16] = JobObj.Memo;
                                }
                                else
                                {
                                    s[16] = JobObj.Memo.Substring(0, 100);
                                }
                            }
                            else
                            {
                                if (JobObj.Name.Length <= 100)
                                {
                                    s[16] = JobObj.Name;
                                }
                                else
                                {
                                    s[16] = JobObj.Name.Substring(0, 100);
                                }
                            }
                        }
                        else
                        {
                            s[16] = b.PatientInfo.Profession.ID; //职业 没有传中文不知道是否可以
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1")
                        {
                            s[17] = "1"; //婚姻状况编号
                            s[18] = "未婚"; //婚姻状况
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2")
                        {
                            s[17] = "2";
                            s[18] = "已婚";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "3")
                        {
                            s[17] = "3";
                            s[18] = "离婚";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4")
                        {
                            s[17] = "5";
                            s[18] = "其他";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "R" || b.PatientInfo.MaritalStatus.ID.ToString() == "5")
                        {
                            s[17] = "5";
                            s[18] = "其他";
                        }
                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                        {
                            s[17] = "5";
                            s[18] = "其他";
                        }

                        if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "6")
                        {
                            s[17] = "4";
                            s[18] = "丧偶";
                        }

                        s[19] = b.PatientInfo.AddressBusiness;  //工作单位及地址
                        s[20] = b.PatientInfo.CompanyName;//单位地址
                        s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                        s[22] = b.PatientInfo.BusinessZip;//单位邮编      
                        s[23] = b.PatientInfo.AddressHome;//家庭住址
                        s[24] = b.PatientInfo.HomeZip;//住址邮编
                        s[25] = b.PatientInfo.Kin.Name;//联系人
                        Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.Relation.ID);
                        if (RelativeObj != null && RelativeObj.ID != "")
                        {
                            if (RelativeObj.Memo != "")
                            {
                                if (RelativeObj.Memo.Length <= 20)
                                {
                                    s[26] = RelativeObj.Memo;//与患者关系
                                }
                                else
                                {
                                    s[26] = RelativeObj.Memo.Substring(0, 20);//与患者关系
                                }
                            }
                            else
                            {
                                if (RelativeObj.Name.Length <= 20)
                                {
                                    s[26] = RelativeObj.Name;//与患者关系
                                }
                                else
                                {
                                    s[26] = RelativeObj.Name.Substring(0, 20);//与患者关系
                                }
                            }
                        }
                        else
                        {
                            s[26] = b.PatientInfo.Kin.Name;//与患者关系
                        }
                        s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                        s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话

                        #region 医疗付款方式
                        if (b.PatientInfo.Pact.PayKind.ID == "DRGS")
                        {
                            Neusoft.FrameWork.Models.NeuObject pactObj = this.constMana.GetConstant("CASEPACT", b.PatientInfo.Pact.ID);
                            if (pactObj != null)
                            {
                                if (pactObj.Memo != "")
                                {
                                    s[29] = pactObj.Memo;
                                    s[30] = pactObj.Name;
                                    s[31] = b.PatientInfo.SSN;
                                }
                                else
                                {
                                    s[29] = b.PatientInfo.Pact.ID;
                                    s[30] = pactObj.Name;
                                    s[31] = b.PatientInfo.SSN;
                                }
                            }
                        }
                        else
                        {
                            if (b.PatientInfo.Pact.PayKind.ID == "01")
                            {
                                s[31] = b.PatientInfo.SSN;
                                s[29] = "3";
                                s[30] = "自费医疗";
                            }
                            else if (b.PatientInfo.Pact.PayKind.ID == "02")//医保
                            {
                                s[31] = b.PatientInfo.SSN;
                                s[29] = "1";
                                s[30] = "社会基本医疗保险";
                            }
                            else if (b.PatientInfo.Pact.PayKind.ID == "03")
                            {
                                s[29] = "4";
                                s[31] = b.PatientInfo.SSN;
                                s[30] = "公费医疗";
                            }
                            else
                            {
                                s[29] = "6";
                                s[31] = b.PatientInfo.SSN;
                                s[30] = "其他";
                            }
                        }
                        #endregion
                        s[32] = b.PatientInfo.SSN; //其他医疗保险卡号
                        s[33] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                        s[34] = b.PatientInfo.PVisit.InTime.Hour.ToString().PadLeft(2, '0'); //入院时间

                        Neusoft.HISFC.Models.RADT.Location indept = this.baseDml.GetDeptIn(b.PatientInfo.ID);
                        if (indept != null) //入院科室 
                        {
                            s[35] = this.ConverDept(indept.Dept.ID).ID;//入院科室代码
                            s[36] = this.ConverDept(indept.Dept.ID).Name;//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//出院科室代码
                            s[41] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//出院科室名称
                        }
                        else
                        {
                            s[35] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//入院科室代码
                            s[36] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).ID;//出院科室代码
                            s[41] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID).Name;//出院科室名称
                        }
                        s[38] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                        s[39] = b.PatientInfo.PVisit.OutTime.Hour.ToString().PadLeft(2, '0');
                        System.TimeSpan tt = b.PatientInfo.PVisit.OutTime - b.PatientInfo.PVisit.InTime;
                        s[43] = tt.Days.ToString();//住院天数
                        s[44] = b.ClinicDiag.ID;
                        if (b.ClinicDiag.Name.Length > 50)
                        {
                            s[45] = this.ChangeCharacter(b.ClinicDiag.Name.Substring(0, 50).ToString());
                        }
                        else
                        {
                            s[45] = this.ChangeCharacter(b.ClinicDiag.Name);
                        }
                        s[46] = b.ClinicDoc.ID;
                        s[47] = b.ClinicDoc.Name;
                        Neusoft.FrameWork.Models.NeuObject inCircs = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCIRCS, b.PatientInfo.PVisit.Circs.ID);
                        if (inCircs != null && inCircs.ID != "")
                        {
                            if (inCircs.Memo != "")
                            {
                                s[48] = inCircs.Memo.Trim();
                                if (inCircs.Memo.Trim() == "1")
                                {
                                    s[49] = "危";
                                }
                                else if (inCircs.Memo.Trim() == "2")
                                {
                                    s[49] = "急";
                                }
                                else if (inCircs.Memo.Trim() == "3")
                                {
                                    s[49] = "一般";
                                }
                            }
                        }
                        else
                        {
                            s[48] = b.PatientInfo.PVisit.Circs.ID;//入院情况
                            s[49] = this.constMana.GetConstant("INCIRCS", b.PatientInfo.PVisit.Circs.ID).Name;
                        }
                        s[50] = b.InHospitalDiag.ID;

                        if (b.DiagDate < new DateTime(1900, 1, 1))
                        {
                            s[52] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');
                        }
                        else if (b.DiagDate >= b.PatientInfo.PVisit.OutTime)//确诊日期大于等于出院日期  按入院日期获取
                        {
                            s[52] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');
                        }
                        else
                        {
                            s[52] = b.DiagDate.ToShortDateString().Replace('-', '/');
                            //确诊日期
                        }


                        s[37] = "";//入院病室               
                        s[42] = ""; //出院病室


                        if (b.InHospitalDiag.Name.Length > 50)
                        {
                            s[51] = this.ChangeCharacter(b.InHospitalDiag.Name.Substring(0, 50).ToString());
                        }
                        else
                        {
                            s[51] = this.ChangeCharacter(b.InHospitalDiag.Name);
                        }

                        s[156] = "0";
                        if (alDose != null)
                        {
                            foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose obj in alDose)
                            {
                                if (obj.DiagInfo.DiagType.ID == "1")
                                {
                                    if (obj.DiagInfo.ICD10.ID.Contains("B15") || obj.DiagInfo.ICD10.ID.Contains("B16") || obj.DiagInfo.ICD10.ID.Contains("B17") || obj.DiagInfo.ICD10.ID.Contains("B18") || obj.DiagInfo.ICD10.ID.Contains("B19"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("A15.0") || obj.DiagInfo.ICD10.ID.Contains("A15.1") || obj.DiagInfo.ICD10.ID.Contains("A15.2") || obj.DiagInfo.ICD10.ID.Contains("A16.0") || obj.DiagInfo.ICD10.ID.Contains("A15.0") || obj.DiagInfo.ICD10.ID.Contains("A16.1") || obj.DiagInfo.ICD10.ID.Contains("A16.2"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I21"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I50.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("J13") || obj.DiagInfo.ICD10.ID.Contains("J14") || obj.DiagInfo.ICD10.ID.Contains("J15"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I27.9"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K92.208"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("N04"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("E05"))
                                    {
                                        s[156] = "1";

                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I61"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("I63"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("D60") || obj.DiagInfo.ICD10.ID.Contains("D61"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C91.0") || obj.DiagInfo.ICD10.ID.Contains("C92.0") || obj.DiagInfo.ICD10.ID.Contains("C93.0") || obj.DiagInfo.ICD10.ID.Contains("C94.0") || obj.DiagInfo.ICD10.ID.Contains("C95.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("E04"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K35"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K81.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("K40"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C16"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C34.1") || obj.DiagInfo.ICD10.ID.Contains("C34.2") || obj.DiagInfo.ICD10.ID.Contains("C34.3") || obj.DiagInfo.ICD10.ID.Contains("C34.4") || obj.DiagInfo.ICD10.ID.Contains("C34.5") || obj.DiagInfo.ICD10.ID.Contains("C34.6") || obj.DiagInfo.ICD10.ID.Contains("C34.7") || obj.DiagInfo.ICD10.ID.Contains("C34.8") || obj.DiagInfo.ICD10.ID.Contains("C34.901"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C15"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("36.1"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("C67"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("N40"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("S06"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("M51.202"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("J18.0"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("A04.903"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("D25"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("74"))
                                    {
                                        s[156] = "1";
                                    }
                                    else if (obj.DiagInfo.ICD10.ID.Contains("H25"))
                                    {
                                        s[156] = "1";
                                    }
                                    break;
                                }
                            }
                        }
                        if (alDiagnose != null)
                        {
                            if (alDiagnose.Count > 0)
                            {
                                foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose diagnoseObj in alDiagnose)
                                {
                                    if (diagnoseObj.MainFlag.Equals("6"))
                                    {
                                        s[53] = this.ChangeCharacter(diagnoseObj.DiagInfo.ICD10.Name);//病理诊断
                                        break;
                                    }
                                }

                            }
                        }
                        s[54] = b.FirstAnaphyPharmacy.Name;//药物过敏 
                        if (b.Hbsag == null)
                        {
                            s[55] = "0";
                        }
                        else
                        {
                            s[55] = b.Hbsag;
                        }
                        s[56] = this.constMana.GetConstant("HbsAg", b.Hbsag).Name;
                        if (b.HcvAb == null)
                        {
                            s[57] = "0";
                        }
                        else
                        {
                            s[57] = b.HcvAb;
                        }
                        s[58] = this.constMana.GetConstant("HbsAg", b.HcvAb).Name;
                        if (b.HivAb == null)
                        {
                            s[59] = "0";
                        }
                        else
                        {
                            s[59] = b.HivAb;
                        }
                        s[60] = this.constMana.GetConstant("HbsAg", b.HivAb).Name;
                        if (b.CePi == null)
                        {
                            s[61] = "1";
                        }
                        else
                        {
                            s[61] = b.CePi;
                        }
                        s[62] = this.constMana.GetConstant("CASEYSEORNO", b.CePi).Name;
                        if (b.PiPo == null)
                        {
                            s[63] = "1";
                        }
                        else
                        {
                            s[63] = b.PiPo;
                        }
                        s[64] = this.constMana.GetConstant("CASEYSEORNO", b.PiPo).Name;
                        s[65] = "0";
                        s[66] = this.constMana.GetConstant("CASEYSEORNO", b.ClPa).Name;
                        s[67] = "0";
                        s[68] = this.constMana.GetConstant("CASEYSEORNO", b.FsBl).Name;
                        s[69] = "0";
                        s[70] = this.constMana.GetConstant("CASEYSEORNO", b.OpbOpa).Name;


                        s[71] = b.SalvTimes.ToString();//抢救次数
                        s[72] = b.SuccTimes.ToString();//成功次数
                        s[73] = b.PatientInfo.PVisit.ReferringDoctor.ID;
                        s[74] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
                        s[75] = b.PatientInfo.PVisit.ConsultingDoctor.ID;
                        s[76] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
                        s[77] = b.PatientInfo.PVisit.AttendingDoctor.ID;//主治医师姓名
                        s[78] = b.PatientInfo.PVisit.AttendingDoctor.Name;
                        s[79] = b.PatientInfo.PVisit.AdmittingDoctor.ID;//住院医师姓名
                        s[80] = b.PatientInfo.PVisit.AdmittingDoctor.Name;
                        s[81] = b.RefresherDoc.ID;//进修医生
                        s[82] = b.RefresherDoc.Name;
                        s[83] = b.GraduateDoc.ID;//研究生实习医师名称
                        s[84] = b.GraduateDoc.Name;
                        s[85] = b.PatientInfo.PVisit.TempDoctor.ID;
                        s[86] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                        s[87] = b.CodingOper.ID;//编码员名称
                        s[88] = b.CodingOper.Name;
                        s[89] = b.OperInfo.ID;
                        s[90] = b.OperInfo.Name;//操作员名称（病案整理者）
                        s[91] = "";//1";//b.MrQuality;//病案质量 
                        s[92] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                        s[93] = b.QcDoc.ID;//质控医师名称
                        s[94] = b.QcDoc.Name;
                        s[95] = b.QcNurse.ID;
                        s[96] = b.QcNurse.Name;//质控护士名称

                        if (b.CheckDate < new DateTime(1900, 1, 1))
                        {
                            s[97] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                        }
                        else if (b.CheckDate <= b.PatientInfo.PVisit.OutTime)//质控日期不可能小于出院日期
                        {
                            s[97] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                        }
                        else
                        {
                            s[97] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
                        }

                        s[98] = "";//是否因麻醉死亡编号
                        s[99] = "";//是否因麻醉死亡


                        for (int j = 100; j <= 118; j++)
                        {
                            s[j] = "0.00";
                        }

                        decimal feeTot = 0.0M;
                        decimal feeOther = 0.0M;
                        foreach (Neusoft.HISFC.Models.RADT.Patient feeInfo in alFee)
                        {
                            decimal fee1 = 0.0M;
                            fee1 = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeInfo.IDCard), 2);

                            feeTot += fee1;

                            string fee = fee1.ToString();

                            #region  正常定义病案费用表达式的情况  南庄等几家医院没有定义使用了fp表达式 晕了
                            if (feeInfo.DIST.TrimStart('0') == "1")//床位费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[101]) + fee1;
                                s[101] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "2")//护理费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[102]) + fee1;
                                s[102] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "3")//西药费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[103]) + fee1;
                                s[103] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "4")//中成药费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + fee1;
                                s[105] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "5")//中草药费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]) + fee1;
                                s[106] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "6")//放射费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[107]) + fee1;
                                s[107] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "7")//化验费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[108]) + fee1;
                                s[108] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "8")//输氧费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[109]) + fee1;
                                s[109] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "9")//输血费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[110]) + fee1;
                                s[110] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "10")//诊疗费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[111]) + fee1;
                                s[111] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "11")//手术费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[112]) + fee1;
                                s[112] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "12")//接生费
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[113]) + fee1;
                                s[113] = temp.ToString();//接生费
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "13")//检查费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[114]) + fee1;
                                s[114] = temp.ToString();
                            }
                            else if (feeInfo.DIST.TrimStart('0') == "14")//麻醉费*
                            {
                                decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[115]) + fee1;
                                s[115] = temp.ToString();
                            }
                            else
                            {
                                feeOther += fee1;
                            }
                            #endregion
                        }
                        s[100] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeTot), 2).ToString();
                        decimal tempZYF = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]);
                        s[104] = tempZYF.ToString(); //中药费

                        s[116] = "0.00";//婴儿费
                        s[117] = "0.00";//陪床费
                        s[118] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeOther), 2).ToString();


                        s[119] = b.CadaverCheck;//尸检
                        s[120] = this.constMana.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
                        s[121] = b.YnFirst;
                        s[122] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        s[123] = b.YnFirst;
                        s[124] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        s[125] = b.YnFirst;
                        s[126] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        s[127] = b.YnFirst;
                        s[128] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                        if (b.VisiStat == "0")
                        {
                            b.VisiStat = "2";
                        }
                        s[129] = b.VisiStat;
                        s[130] = this.constMana.GetConstant("CASEYSEORNO", b.VisiStat).Name;
                        //s[131] = b.VisiPeriodWeek;
                        if (b.VisiPeriodYear.ToString() != "0") //随诊期限
                        {
                            s[131] = "Y" + b.VisiPeriodYear.ToString();
                        }
                        else if (b.VisiPeriodMonth.ToString() != "0")
                        {
                            s[131] = "M" + b.VisiPeriodMonth.ToString();

                        }
                        else if (b.VisiPeriodWeek.ToString() != "0")
                        {
                            try
                            {
                                System.Convert.ToInt32(b.VisiPeriodWeek);
                                s[131] = "W" + b.VisiPeriodWeek.ToString();
                            }
                            catch
                            {
                                s[131] = b.VisiPeriodWeek.ToString();
                            }
                        }
                        else
                        {
                            s[131] = "";
                        }

                        if (b.TechSerc == "0")
                        {
                            b.TechSerc = "2";
                        }
                        s[132] = b.TechSerc;//示教科研
                        s[133] = this.constMana.GetConstant("CASEYSEORNO", b.TechSerc).Name;
                        if (b.PatientInfo.BloodType.ID.ToString() == "A")
                        {
                            s[134] = "1";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "B")
                        {
                            s[134] = "2";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "AB")
                        {
                            s[134] = "3";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                        {
                            s[134] = "4";
                            s[135] = b.PatientInfo.BloodType.ID.ToString();
                        }
                        else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                        {
                            s[134] = "9";
                            s[135] = "未查";
                        }
                        else
                        {
                            s[134] = "5";
                            s[135] = "其他";
                        }
                        s[136] = b.RhBlood;
                        s[137] = this.constMana.GetConstant("RHSTATE", b.RhBlood).Name;
                        s[138] = b.ReactionBlood;
                        s[139] = this.constMana.GetConstant("CASEYSEORNO", b.ReactionBlood).Name;
                        try
                        {
                            s[142] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodRed).ToString();//红细胞数
                        }
                        catch
                        {
                        }

                        try
                        {
                            s[143] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodPlatelet).ToString();//血小板数

                        }
                        catch
                        {
                        }

                        try
                        {
                            s[144] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BodyAnotomize).ToString();//血浆数
                        }
                        catch
                        {
                        }

                        try
                        {
                            s[145] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodWhole).ToString();//全血数

                        }
                        catch
                        {
                        }

                        try
                        {
                            s[146] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodOther).ToString();//其他输血数

                        }
                        catch
                        {
                        }


                        s[140] = "";//输液反应编码
                        s[141] = "";//输液反应


                        s[147] = b.InconNum.ToString();//院际会诊次数 70 远程会诊次数
                        s[148] = b.OutconNum.ToString();//院际会诊次数 70 远程会诊次数
                        s[149] = b.SuperNus.ToString(); //特级护理时间(小时)                         
                        s[150] = b.INus.ToString(); //I级护理时间(日)                                     
                        s[151] = b.IINus.ToString(); //II级护理时间(日)                                    
                        s[152] = b.IIINus.ToString(); //III级护理时间(日)                                   
                        s[153] = b.StrictNuss.ToString(); //重症监护时间( 小时)                               
                        s[154] = b.SpecalNus.ToString();  // 特殊护理(日) 
                        if (b.SalvTimes > 0)
                        {
                            s[157] = "1";
                            s[158] = "1";
                        }
                        else
                        {
                            s[157] = "0";
                            s[158] = "0";
                        }
                        s[162] = "0";//是否单病种




                        s[155] = "0.00";//婴儿数
                        s[156] = "0";//是否部分病种


                        s[159] = "0";//是否三日确诊
                        s[160] = "0";//是否月内再次住院
                        s[161] = "0";//是否中度烧伤

                        s[163] = "0.00";//中医院治疗费(预留字段)

                        if (alChangeDepe.Count > 0)
                        {
                            Neusoft.HISFC.Models.RADT.Location dept = alChangeDepe[0] as Neusoft.HISFC.Models.RADT.Location;
                            s[164] = this.ConverDept(dept.Dept.ID).ID;
                            s[165] = this.ConverDept(dept.Dept.ID).Name;
                            s[166] = Neusoft.FrameWork.Function.NConvert.ToDateTime(dept.Dept.Memo).ToShortDateString().Replace('-', '/');
                        }
                        else
                        {
                            s[164] = "";
                            s[165] = "";
                            s[166] = "";
                        }
                        try
                        {

                        }
                        catch
                        {
                        }
                        s[167] = "";
                        s[168] = "";
                        s[169] = "";
                        s[170] = this.GetDateTimeFromSysDateTime().ToShortDateString().Replace('-', '/');
                        s[171] = "";
                        s[172] = "";
                        s[173] = "";
                        s[174] = "";
                        //s[175] = b.PatientInfo.PVisit.InSource.ID;//入院来源
                        //s[176] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;
                        Neusoft.FrameWork.Models.NeuObject inSource = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INAVENUE, b.PatientInfo.PVisit.InSource.ID);
                        if (inSource != null && inSource.ID != "")
                        {
                            if (inSource.Memo != "")
                            {
                                s[175] = inSource.Memo.Trim();
                                if (inSource.Memo.Trim() == "1")
                                {
                                    s[176] = "医院所在区（县）";
                                }
                                else if (inSource.Memo.Trim() == "2")
                                {
                                    s[176] = "医院所在市的外区（县）";
                                }
                                else if (inSource.Memo.Trim() == "3")
                                {
                                    s[176] = "本省其他市";
                                }
                                else if (inSource.Memo.Trim() == "4")
                                {
                                    s[176] = "外省（直辖市）";
                                }
                            }
                        }
                        else
                        {
                            s[175] = b.PatientInfo.PVisit.InSource.ID;//入院来源
                            s[176] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;
                        }
                        if (b.FirstOperation.ID.Trim() == "")
                        {
                            s[177] = "0";
                        }
                        else
                        {
                            s[177] = "1";
                        }
                        s[178] = "0";
                        if (b.SyndromeFlag == null || b.SyndromeFlag == "")
                        {
                            s[179] = "0";
                        }
                        else
                        {
                            s[179] = b.SyndromeFlag;
                        }
                        s[180] = b.InfectionNum.ToString();
                        s[181] = "";
                        s[182] = "";
                        s[183] = "";
                        s[184] = "";
                        s[185] = "";
                        s[186] = "";
                        s[187] = "";
                        s[188] = "";
                        s[189] = "";
                        s[190] = "";
                        s[191] = "";
                        s[192] = "";
                        s[193] = "";
                        s[194] = "";
                        s[195] = "";
                        s[196] = "0";//是否输入
                        return s;

                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                #endregion
            }
            else
            {
                string[] s = new string[198];
                try
                {
                    s[197] = fid.ToString();
                    s[0] = "0";
                    s[1] = b.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());//病案号
                    s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
                    s[3] = "10";
                    s[4] = b.PatientInfo.ID;
                    if (b.PatientInfo.Age == "0")
                    {
                        if (b.AgeUnit == "不存")
                        {
                            s[5] = this.baseDml.GetAgeByFun(b.PatientInfo.Birthday.Date, b.PatientInfo.PVisit.InTime.Date);
                        }
                        else
                        {
                            s[5] = b.AgeUnit.ToString();//年龄
                        }
                    }
                    else
                    {
                        s[5] = b.PatientInfo.Age.ToString();//年龄
                    }
                    s[6] = b.PatientInfo.Name;//姓名
                    if (b.PatientInfo.Sex.ID.ToString() == "M")
                    {
                        s[7] = "1";
                        s[8] = "男";
                    }
                    else
                    {
                        s[7] = "2";
                        s[8] = "女";
                    }
                    s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');//出生日期
                    s[10] = b.PatientInfo.AreaCode; //出生地
                    s[11] = b.PatientInfo.IDCard;//身份证号 
                    if (b.PatientInfo.Country.ID.ToString() == "1")//中国  需要转换其他
                    {
                        s[12] = "A156";
                        s[13] = "中国";
                    }
                    {
                        Neusoft.FrameWork.Models.NeuObject countryObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY, b.PatientInfo.Country.ID.ToString());
                        if (countryObj != null && countryObj.ID != "")
                        {
                            if (countryObj.Memo != "")
                            {
                                s[12] = countryObj.Memo.ToString(); //国籍编号
                                s[13] = countryObj.Name.ToString(); //国籍
                            }
                            else
                            {
                                s[12] = countryObj.ID.ToString(); //国籍编号
                                s[13] = countryObj.Name.ToString(); //国籍
                            }
                        }
                        else
                        {
                            s[12] = b.PatientInfo.Country.ID.ToString(); //国籍编号
                            s[13] = ""; //国籍
                        }
                    }

                    Neusoft.FrameWork.Models.NeuObject NationObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION, b.PatientInfo.Nationality.ID.ToString());
                    if (NationObj != null && NationObj.ID != "")
                    {
                        if (NationObj.Memo != "")
                        {
                            s[14] = NationObj.Memo; //民族编号
                            s[15] = NationObj.Name; //民族
                        }
                        else
                        {
                            s[14] = NationObj.ID; //民族编号
                            s[15] = NationObj.Name; //民族
                        }
                    }
                    else
                    {
                        s[14] = b.PatientInfo.Nationality.ID; //民族编号
                        s[15] = ""; //民族
                    }
                    //add by chengym 2011-6-15  字典表的名称字段varchar（50） 有些执业描述超过25个中文字符，这时获取备注的完整名称，保证上传的数据没有问题； 
                    Neusoft.FrameWork.Models.NeuObject JobObj = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.PROFESSION, b.PatientInfo.Profession.ID.ToString());
                    if (JobObj != null && JobObj.ID != "")
                    {
                        if (JobObj.Memo != "")
                        {
                            if (JobObj.Memo.Length <= 100)
                            {
                                s[16] = JobObj.Memo;
                            }
                            else
                            {
                                s[16] = JobObj.Memo.Substring(0, 100);
                            }
                        }
                        else
                        {
                            if (JobObj.Name.Length <= 100)
                            {
                                s[16] = JobObj.Name;
                            }
                            else
                            {
                                s[16] = JobObj.Name.Substring(0, 100);
                            }
                        }
                    }
                    else
                    {
                        s[16] = b.PatientInfo.Profession.ID.ToString(); //职业 没有传中文不知道是否可以
                    }
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "S" || b.PatientInfo.MaritalStatus.ID.ToString() == "1")
                    {
                        s[17] = "1"; //婚姻状况编号
                        s[18] = "未婚"; //婚姻状况
                    }
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "M" || b.PatientInfo.MaritalStatus.ID.ToString() == "2")
                    {
                        s[17] = "2";
                        s[18] = "已婚";
                    }
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "3")
                    {
                        s[17] = "3";
                        s[18] = "离婚";
                    }
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "D" || b.PatientInfo.MaritalStatus.ID.ToString() == "4")
                    {
                        s[17] = "5";
                        s[18] = "其他";
                    }
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "R" || b.PatientInfo.MaritalStatus.ID.ToString() == "5")
                    {
                        s[17] = "5";
                        s[18] = "其他";
                    }
                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "A")
                    {
                        s[17] = "5";
                        s[18] = "其他";
                    }

                    if (b.PatientInfo.MaritalStatus.ID.ToString() == "W" || b.PatientInfo.MaritalStatus.ID.ToString() == "6")
                    {
                        s[17] = "4";
                        s[18] = "丧偶";
                    }

                    s[19] = b.PatientInfo.AddressBusiness.ToString();  //工作单位及地址
                    s[20] = b.PatientInfo.AddressBusiness.ToString();//单位地址
                    s[21] = b.PatientInfo.PhoneBusiness;//单位电话
                    s[22] = b.PatientInfo.BusinessZip;//单位邮编      
                    s[23] = b.PatientInfo.AddressHome;//家庭住址
                    s[24] = b.PatientInfo.HomeZip;//住址邮编
                    s[25] = b.PatientInfo.Kin.Name;//联系人
                    Neusoft.FrameWork.Models.NeuObject RelativeObj = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.RelationLink);
                    if (RelativeObj != null && RelativeObj.ID != "")
                    {
                        if (RelativeObj.Memo != "")
                        {
                            if (RelativeObj.Memo.Length <= 20)
                            {
                                s[26] = RelativeObj.Memo;//与患者关系
                            }
                            else
                            {
                                s[26] = RelativeObj.Memo.Substring(0, 20);//与患者关系
                            }
                        }
                        else
                        {
                            if (RelativeObj.Name.Length <= 20)
                            {
                                s[26] = RelativeObj.Name;//与患者关系
                            }
                            else
                            {
                                s[26] = RelativeObj.Name.Substring(0, 20);//与患者关系
                            }
                        }
                    }
                    else
                    {
                        s[26] = b.PatientInfo.Kin.RelationLink;//与患者关系
                    }
                    s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址
                    s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话

                    #region 医疗付款方式
                    if (b.PatientInfo.Pact.PayKind.ID == "01")
                    {
                        s[31] = b.PatientInfo.SSN;
                        s[29] = "3";
                        s[30] = "自费医疗";
                    }
                    else if (b.PatientInfo.Pact.PayKind.ID == "02")//医保
                    {
                        s[31] = b.PatientInfo.SSN;
                        s[29] = "1";
                        s[30] = "社会基本医疗保险";
                    }
                    else if (b.PatientInfo.Pact.PayKind.ID == "03")
                    {
                        s[29] = "4";
                        s[31] = b.PatientInfo.SSN;
                        s[30] = "公费医疗";
                    }
                    else
                    {
                        s[29] = "6";
                        s[31] = b.PatientInfo.SSN;
                        s[30] = "其他";
                    }
                    #endregion
                    s[32] = b.PatientInfo.SSN; //其他医疗保险卡号
                    s[33] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
                    s[34] = b.PatientInfo.PVisit.InTime.ToShortTimeString(); //入院时间
                    s[35] = this.ConverDept(b.InDept.ID).ID;//入院科室代码
                    s[36] = this.ConverDept(b.InDept.ID).Name;//出院科室名称2011-6-8
                    s[37] = b.InRoom;//入院病室    
                    s[38] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                    s[39] = b.PatientInfo.PVisit.OutTime.ToShortTimeString(); //出院时间
                    s[40] = this.ConverDept(b.OutDept.ID).ID;//出院科室代码
                    s[41] = this.ConverDept(b.OutDept.ID).Name;//出院科室名称2011-6-8
                    s[42] = b.OutRoom; //出院病室
                    s[43] = b.InHospitalDays.ToString();//住院天数
                    s[44] = b.ClinicDiag.ID;
                    if (b.ClinicDiag.Name.Length > 50)
                    {
                        s[45] = this.ChangeCharacter(b.ClinicDiag.Name.Substring(0, 50).ToString());
                    }
                    else
                    {
                        s[45] = this.ChangeCharacter(b.ClinicDiag.Name);
                    }
                    s[46] = b.ClinicDoc.ID;
                    s[47] = b.ClinicDoc.Name;
                    Neusoft.FrameWork.Models.NeuObject inCircs = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCIRCS, b.PatientInfo.PVisit.Circs.ID);
                    if (inCircs != null && inCircs.ID != "")
                    {
                        if (inCircs.Memo != "")
                        {
                            s[48] = inCircs.Memo.Trim();
                            if (inCircs.Memo.Trim() == "1")
                            {
                                s[49] = "危";
                            }
                            else if (inCircs.Memo.Trim() == "2")
                            {
                                s[49] = "急";
                            }
                            else if (inCircs.Memo.Trim() == "3")
                            {
                                s[49] = "一般";
                            }
                        }
                    }
                    else
                    {
                        s[48] = b.PatientInfo.PVisit.Circs.ID;//入院情况
                        s[49] = this.constMana.GetConstant("INCIRCS", b.PatientInfo.PVisit.Circs.ID).Name;
                    }
                    s[50] = b.InHospitalDiag.ID;
                    if (b.InHospitalDiag.Name.Length > 50)
                    {
                        s[51] = this.ChangeCharacter(b.InHospitalDiag.Name.Substring(0, 50).ToString());
                    }
                    else
                    {
                        s[51] = this.ChangeCharacter(b.InHospitalDiag.Name.ToString());
                    }
                    if (b.DiagDate < new DateTime(1900, 1, 1))
                    {
                        s[52] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');
                    }
                    else if (b.DiagDate >= b.PatientInfo.PVisit.OutTime)//确诊日期大于等于出院日期  按入院日期获取
                    {
                        s[52] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');
                    }
                    else
                    {
                        s[52] = b.DiagDate.ToShortDateString().Replace('-', '/');//确诊日期
                    }
                    //一般提到界面填写病理诊断 
                    if (b.PathologicalDiagName != null)
                    {
                        if (b.PathologicalDiagName.ToString().Length > 100)
                        {
                            s[53] = this.ChangeCharacter(b.PathologicalDiagName.Substring(0, 100).ToString());
                        }
                        else
                        {
                            s[53] = this.ChangeCharacter(b.PathologicalDiagName.ToString());
                        }
                    }
                    else
                    {
                        ArrayList alDiagnose = this.baseDml.QueryCaseDiagnoseByInpatientNo(b.PatientInfo.ID);

                        if (alDiagnose.Count > 0)
                        {
                            foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose diagnoseObj in alDiagnose)
                            {
                                if (diagnoseObj.DiagInfo.DiagType.ID.ToString() == "6")
                                {
                                    if (diagnoseObj.DiagInfo.ICD10.Name.Length > 100)
                                    {
                                        s[53] = this.ChangeCharacter(diagnoseObj.DiagInfo.ICD10.Name.Substring(0, 100).ToString());//病理诊断
                                    }
                                    else
                                    {
                                        s[53] = this.ChangeCharacter(diagnoseObj.DiagInfo.ICD10.Name);//病理诊断
                                    }
                                    break;
                                }
                            }

                        }
                    }

                    //string anaphyPh=b.AnaphyFlag.ToString() + b.FirstAnaphyPharmacy.ID + b.FirstAnaphyPharmacy.Name + b.SecondAnaphyPharmacy.ID;
                    string anaphyPh = b.FirstAnaphyPharmacy.ID;
                    if (anaphyPh.Length > 100)
                    {
                        s[54] = this.ChangeCharacter(anaphyPh.Substring(0, 100));
                    }
                    else
                    {
                        s[54] = this.ChangeCharacter(anaphyPh);//药物过敏  
                    }
                    s[55] = b.Hbsag;//HBsAg编号
                    s[56] = this.constMana.GetConstant("ASSAYTYPE", b.Hbsag).Name; //HBsAg
                    s[57] = b.HcvAb;//HCV-Ab编号
                    s[58] = this.constMana.GetConstant("ASSAYTYPE", b.HcvAb).Name;//HCV-Ab
                    s[59] = b.HivAb;//HIV-AB编号
                    s[60] = this.constMana.GetConstant("ASSAYTYPE", b.HivAb).Name;//HIV-AB

                    s[61] = b.CePi;//门诊与出院诊断符合情况编号
                    s[62] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.CePi).Name;//门诊与出院诊断符合情况
                    s[63] = b.PiPo;//入院与出院诊断符合情况编号
                    s[64] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.PiPo).Name;//入院与出院诊断符合情况
                    s[65] = b.ClPa;//临床与病理诊断符合情况编号
                    s[66] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.ClPa).Name;//临床与病理诊断符合情况
                    s[67] = b.FsBl;//放射与病理诊断符合情况编号
                    s[68] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.FsBl).Name;//放射与病理诊断符合情况
                    s[69] = b.OpbOpa;//手术符合编号
                    s[70] = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.ACCORDSTAT, b.OpbOpa).Name;//手术符合

                    s[71] = b.SalvTimes.ToString();//抢救次数
                    s[72] = b.SuccTimes.ToString();//成功次数

                    s[73] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID).ID;
                    s[74] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID).Name;//科主任名称PatientInfo.PVisit.ReferringDoctor.Name
                    s[75] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID).ID;
                    s[76] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID).Name;//主任医师姓名PatientInfo.PVisit.ConsultingDoctor.Name
                    s[77] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID).ID;//主治医师姓名PatientInfo.PVisit.AttendingDoctor.Name
                    s[78] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID).Name;
                    s[79] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID).ID;//住院医师姓名PatientInfo.PVisit.AdmittingDoctor.Name
                    s[80] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID).Name;
                    s[81] = this.ConverDoc(b.RefresherDoc.ID).ID;//进修医生
                    s[82] = this.ConverDoc(b.RefresherDoc.ID).Name;
                    s[83] = this.ConverDoc(b.GraduateDoc.ID).ID;//研究生实习医师名称
                    s[84] = this.ConverDoc(b.GraduateDoc.ID).Name;
                    s[85] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID).ID;
                    s[86] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID).Name;//实习医师名称
                    s[87] = this.ConverDoc(b.CodingOper.ID).ID;//编码员名称
                    s[88] = this.ConverDoc(b.CodingOper.ID).Name;
                    s[89] = this.ConverDoc(b.OperInfo.ID).ID;
                    s[90] = this.ConverDoc(b.OperInfo.ID).Name;//操作员名称（病案整理者）
                    s[91] = b.MrQuality.ToString();//1";//b.MrQuality;//病案质量 
                    s[92] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                    s[93] = this.ConverDoc(b.QcDoc.ID).ID;//质控医师名称
                    s[94] = this.ConverDoc(b.QcDoc.ID).Name;
                    s[95] = this.ConverDoc(b.QcNurse.ID).ID;
                    s[96] = this.ConverDoc(b.QcNurse.ID).Name;//质控护士名称

                    if (b.CheckDate < new DateTime(1900, 1, 1))
                    {
                        s[97] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else if (b.CheckDate <= b.PatientInfo.PVisit.OutTime)//质控日期不可能小于出院日期
                    {
                        s[97] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');
                    }
                    else
                    {
                        s[97] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
                    }

                    s[98] = "";//是否因麻醉死亡编号
                    s[99] = "";//是否因麻醉死亡


                    for (int j = 100; j <= 118; j++)
                    {
                        s[j] = "0.00";
                    }

                    decimal feeTot = 0.0M;
                    decimal feeOther = 0.0M;
                    foreach (Neusoft.HISFC.Models.RADT.Patient feeInfo in alFee)
                    {
                        decimal fee1 = 0.0M;
                        fee1 = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeInfo.IDCard), 2);

                        feeTot += fee1;

                        string fee = fee1.ToString();
                        #region  正常定义病案费用表达式的情况  南庄等几家医院没有定义使用了fp表达式 晕了
                        if (feeInfo.DIST.TrimStart('0') == "1")//床位费
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[101]) + fee1;
                            s[101] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "2")//护理费
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[102]) + fee1;
                            s[102] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "3")//西药费
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[103]) + fee1;
                            s[103] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "4")//中成药费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + fee1;
                            s[105] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "5")//中草药费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]) + fee1;
                            s[106] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "6")//放射费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[107]) + fee1;
                            s[107] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "7")//化验费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[108]) + fee1;
                            s[108] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "8")//输氧费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[109]) + fee1;
                            s[109] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "9")//输血费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[110]) + fee1;
                            s[110] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "10")//诊疗费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[111]) + fee1;
                            s[111] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "11")//手术费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[112]) + fee1;
                            s[112] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "12")//接生费
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[113]) + fee1;
                            s[113] = temp.ToString();//接生费
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "13")//检查费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[114]) + fee1;
                            s[114] = temp.ToString();
                        }
                        else if (feeInfo.DIST.TrimStart('0') == "14")//麻醉费*
                        {
                            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[115]) + fee1;
                            s[115] = temp.ToString();
                        }
                        else
                        {
                            feeOther += fee1;
                        }
                        #endregion
                    }
                    s[100] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeTot), 2).ToString();
                    decimal tempZYF = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]);
                    s[104] = tempZYF.ToString(); //中药费

                    s[116] = "0.00";//婴儿费
                    s[117] = "0.00";//陪床费
                    s[118] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeOther), 2).ToString();

                    if (b.CadaverCheck.ToString() == "0")
                    {
                        s[119] = "2";//尸检
                    }
                    else
                    {
                        s[119] = b.CadaverCheck;//尸检
                    }
                    s[120] = this.constMana.GetConstant("CASEYSEORNO", b.CadaverCheck).Name;
                    if (b.YnFirst.ToString() == "0")
                    {
                        s[121] = "2";
                    }
                    else
                    {
                        s[121] = b.YnFirst;
                    }
                    s[122] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                    if (b.YnFirst.ToString() == "0")
                    {
                        s[123] = "2";
                    }
                    else
                    {
                        s[123] = b.YnFirst;
                    }
                    s[124] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                    if (b.YnFirst.ToString() == "0")
                    {
                        s[125] = "2";
                    }
                    else
                    {
                        s[125] = b.YnFirst;
                    }
                    s[126] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                    if (b.YnFirst.ToString() == "0")
                    {
                        s[127] = "2";
                    }
                    else
                    {
                        s[127] = b.YnFirst;
                    }
                    s[128] = this.constMana.GetConstant("CASEYSEORNO", b.YnFirst).Name;
                    if (b.VisiStat == "0")
                    {
                        s[129] = "2";
                    }
                    else
                    {
                        s[129] = b.VisiStat;//是否随诊
                    }
                    s[130] = this.constMana.GetConstant("CASEYSEORNO", b.VisiStat).Name;
                    if (b.VisiPeriodYear.ToString() != "0") //随诊期限
                    {
                        s[131] = "Y" + b.VisiPeriodYear.ToString();
                    }
                    else if (b.VisiPeriodMonth.ToString() != "0")
                    {
                        s[131] = "M" + b.VisiPeriodMonth.ToString();

                    }
                    else if (b.VisiPeriodWeek.ToString() != "0")
                    {
                        try
                        {
                            System.Convert.ToInt32(b.VisiPeriodWeek);
                            s[131] = "W" + b.VisiPeriodWeek.ToString();
                        }
                        catch
                        {
                            s[131] = b.VisiPeriodWeek.ToString();
                        }
                    }
                    else
                    {
                        s[131] = "";
                    }

                    if (b.TechSerc == "0") //是否示教病案编号
                    {
                        s[132] = "2";

                    }
                    else
                    {
                        s[132] = b.TechSerc;//示教科研
                    }
                    s[133] = this.constMana.GetConstant("CASEYSEORNO", b.TechSerc).Name;
                    if (b.PatientInfo.BloodType.ID.ToString() == "A")
                    {
                        s[134] = "1";
                        s[135] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "B")
                    {
                        s[134] = "2";
                        s[135] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "AB")
                    {
                        s[134] = "3";
                        s[135] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "O")
                    {
                        s[134] = "4";
                        s[135] = b.PatientInfo.BloodType.ID.ToString();
                    }
                    else if (b.PatientInfo.BloodType.ID.ToString() == "9")
                    {
                        s[134] = "9";
                        s[135] = "未查";
                    }
                    else
                    {
                        s[134] = "5";
                        s[135] = "其他";
                    }

                    s[136] = b.RhBlood;
                    if (b.RhBlood.ToString() == "1")
                    {
                        s[137] = "阴";
                    }
                    else if (b.RhBlood.ToString() == "2")
                    {
                        s[137] = "阳";
                    }
                    else
                    {
                        s[137] = "未查";
                    }

                    s[138] = b.ReactionBlood;
                    if (b.ReactionBlood.ToString() == "1")
                    {
                        s[139] = "有";
                    }
                    else
                    {
                        s[139] = "无";
                    }

                    s[140] = b.ReactionLiquid;//输液反应编码
                    if (b.ReactionLiquid == "1")
                    {
                        s[141] = "有";//输液反应
                    }
                    else
                    {
                        s[141] = "无";//输液反应
                    }

                    try
                    {
                        s[142] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodRed).ToString();//红细胞数
                    }
                    catch
                    {
                    }

                    try
                    {
                        s[143] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodPlatelet).ToString();//血小板数

                    }
                    catch
                    {
                    }

                    try
                    {
                        s[144] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BodyAnotomize).ToString();//血浆数
                    }
                    catch
                    {
                    }

                    try
                    {
                        s[145] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodWhole).ToString();//全血数

                    }
                    catch
                    {
                    }

                    try
                    {
                        s[146] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodOther).ToString();//其他输血数

                    }
                    catch
                    {
                    }
                    s[147] = b.InconNum.ToString();//院际会诊次数 70 远程会诊次数
                    s[148] = b.OutconNum.ToString();//院际会诊次数 70 远程会诊次数
                    s[149] = b.SuperNus.ToString(); //特级护理时间(小时)                         
                    s[150] = b.INus.ToString(); //I级护理时间(日)                                     
                    s[151] = b.IINus.ToString(); //II级护理时间(日)                                    
                    s[152] = b.IIINus.ToString(); //III级护理时间(日)                                   
                    s[153] = b.StrictNuss.ToString(); //重症监护时间( 小时)                               
                    s[154] = b.SpecalNus.ToString();  // 特殊护理(日) 
                    s[155] = b.PatientInfo.User03;//婴儿数"0.00"
                    s[156] = "0";//是否部分病种
                    if (b.SalvTimes > 0)
                    {
                        s[157] = b.SalvTimes.ToString();
                    }
                    else
                    {
                        s[157] = "0";
                    }
                    if (b.SuccTimes > 0)
                    {
                        s[158] = b.SuccTimes.ToString();
                    }
                    else
                    {
                        s[158] = "0";
                    }

                    if (b.DiagDays < 4)
                    {
                        s[159] = "1";//是否三日确诊
                    }
                    else
                    {
                        s[159] = "0";//是否三日确诊
                    }
                    s[160] = "0";//是否月内再次住院
                    s[161] = "0";//是否中度烧伤
                    s[162] = "0";

                    s[163] = "0.00";//中医院治疗费(预留字段)

                    if (alChangeDepe.Count > 0)
                    {
                        Neusoft.HISFC.Models.RADT.Location dept = alChangeDepe[0] as Neusoft.HISFC.Models.RADT.Location;
                        s[164] = this.ConverDept(dept.Dept.ID).ID;
                        s[165] = this.ConverDept(dept.Dept.ID).Name;
                    }
                    else
                    {
                        s[164] = "";
                        s[165] = "";
                        //s[166] = "";
                    }
                    try//难道这样插进去的数据就不会变成1900-1-1
                    {
                        string FZKDATE = Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.HISFC.Models.RADT.Location)alChangeDepe[0]).Memo).ToShortDateString().Replace('-', '/');
                        if (Neusoft.FrameWork.Function.NConvert.ToDateTime(FZKDATE) == System.DateTime.MinValue)
                        {
                            s[166] = "";
                            s[167] = "";
                        }
                        else
                        {
                            s[166] = Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.HISFC.Models.RADT.Location)alChangeDepe[0]).Dept.Memo).ToShortDateString().Replace('-', '/');
                            s[167] = Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.HISFC.Models.RADT.Location)alChangeDepe[0]).Dept.Memo).ToShortTimeString();
                        }
                    }
                    catch
                    {
                    }

                    //s[167] = "";
                    s[168] = "";
                    s[169] = "";
                    s[170] = this.GetDateTimeFromSysDateTime().ToShortDateString().Replace('-', '/');
                    s[171] = "";
                    s[172] = "";
                    s[173] = "";
                    s[174] = "";
                    Neusoft.FrameWork.Models.NeuObject inSource = this.constMana.GetConstant(Neusoft.HISFC.Models.Base.EnumConstant.INAVENUE, b.PatientInfo.PVisit.InSource.ID);
                    if (inSource != null && inSource.ID != "")
                    {
                        if (inSource.Memo != "")
                        {
                            s[175] = inSource.Memo.Trim();
                            if (inSource.Memo.Trim() == "1")
                            {
                                s[176] = "医院所在区（县）";
                            }
                            else if (inSource.Memo.Trim() == "2")
                            {
                                s[176] = "医院所在市的外区（县）";
                            }
                            else if (inSource.Memo.Trim() == "3")
                            {
                                s[176] = "本省其他市";
                            }
                            else if (inSource.Memo.Trim() == "4")
                            {
                                s[176] = "外省（直辖市）";
                            }
                        }
                    }
                    else
                    {
                        s[175] = b.PatientInfo.PVisit.InSource.ID;//入院来源
                        s[176] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;
                    }
                    if (b.FirstOperation.ID.Trim() == "")
                    {
                        s[177] = "0";
                    }
                    else
                    {
                        s[177] = "1";
                    }
                    s[178] = "0";
                    if (b.SyndromeFlag == null || b.SyndromeFlag == "")
                    {
                        s[179] = "0";
                    }
                    else
                    {
                        s[179] = b.SyndromeFlag;
                    }
                    s[180] = b.InfectionNum.ToString();
                    s[181] = "";
                    s[182] = "";
                    s[183] = "";
                    s[184] = "";
                    s[185] = "";
                    s[186] = "";
                    s[187] = "";
                    s[188] = "";
                    s[189] = "";
                    s[190] = "";
                    s[191] = "";
                    s[192] = "";
                    s[193] = "";
                    s[194] = "";
                    s[195] = "";
                    s[196] = "0";//是否输入
                    return s;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }

        public int GetCasUpload()
        {
            string strSQL = "select SEQ_CAS_UPLOAP.Nextval from dual";
            //返回最大的发生序号
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strSQL));
        }

        #endregion
        /// <summary>
        /// 获取现地址编码
        /// </summary>
        /// <param name="AdrrName">现地址名称</param>
        /// <returns></returns>
        private string QueryCurrentAdrrCodeByName(string AdrrName)
        {
            string strSql = @" select d.code from com_dictionary d where d.type='BZDZK' and d.name='{0}'";

            string iReturn = string.Empty;
            try
            {
                strSql = string.Format(strSql, AdrrName);
                //查询
                if (this.ExecQuery(strSql) < 0)
                {
                    this.Err = "执行sql失败!";
                    return "";
                }
                this.Reader.Read();
                iReturn = Reader[0].ToString();
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            catch (Exception ex)
            {
                this.Err = "" + ex.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                return "";
            }
            return iReturn;
        }
        #region 字典对照

        /// <summary>
        /// 获取医生对照信息
        /// </summary>
        /// <param name="doctid"></param>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject GetDoctInfoByCode(string doctid)
        {
            if (this.server == null)
                return null;
            //SQLServerManager server = new SQLServerManager();
            DataSet ds = new DataSet();
            string sql = @"select y.ftygh,y.fname from tdoctor y where y.fgh='{0}'";
            sql = string.Format(sql, doctid);
            int result = server.Execute(sql, ref ds);
            if (result < 0)
            {
                this.Erro = "获取医生对照信息失败!";
                return null;
            }
            if (ds.Tables[0].Rows.Count <= 0)
            {
                return null;
            }
            Neusoft.FrameWork.Models.NeuObject odcobj = new Neusoft.FrameWork.Models.NeuObject();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                odcobj.ID = dr[0].ToString();
                odcobj.Name = dr[1].ToString();

            }
            return odcobj;
        }

        /// <summary>
        /// 获取医生对照信息
        /// </summary>
        /// <param name="doctid"></param>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject GetDoctInfoByName(string doctName)
        {
            if (this.server == null)
                return null;
            //SQLServerManager server = new SQLServerManager();
            DataSet ds = new DataSet();
            string sql = @"select y.ftygh,y.fname from tdoctor y where y.fname='{0}'";
            sql = string.Format(sql, doctName);
            int result = server.Execute(sql, ref ds);
            if (result < 0)
            {
                this.Erro = "获取医生对照信息失败!";
                return null;
            }
            if (ds.Tables[0].Rows.Count <= 0)
            {
                return null;
            }
            Neusoft.FrameWork.Models.NeuObject odcobj = new Neusoft.FrameWork.Models.NeuObject();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                odcobj.ID = dr[0].ToString();
                odcobj.Name = dr[1].ToString();

            }
            return odcobj;
        }

        /// <summary>
        /// 获取操作员对照信息(没有信息)
        /// </summary>
        /// <param name="doctid"></param>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject GetOperatorInfo()
        {
            //
            Neusoft.FrameWork.Models.NeuObject odcobj = new Neusoft.FrameWork.Models.NeuObject();
            odcobj.ID = "";
            odcobj.Name = "";
            return odcobj;
        }

        /// <summary>
        /// 取对照科室
        /// </summary>
        /// <param name="deptCode"></param>
        public Neusoft.HISFC.Models.Base.Const ConverDept(string deptCode)
        {
            //测试用,没有对照
            //Neusoft.HISFC.Models.Base.Const testcon = new Neusoft.HISFC.Models.Base.Const();
            //return testcon;

            if (deptdic == null || deptdic.Count==0)
            {
                ArrayList al = this.constMana.GetAllList("CASEDEPT");
                if (al == null)
                {
                    return null;
                }
                foreach (Neusoft.HISFC.Models.Base.Const objcon in al)
                {
                    if (string.IsNullOrEmpty(objcon.UserCode))
                    {
                        continue;
                    }
                    string[] inputcode = objcon.UserCode.Split(',');
                    for (int i = 0; i < inputcode.Length; i++)
                    {
                        if (!deptdic.ContainsKey(inputcode[i]))
                        {
                            deptdic.Add(inputcode[i], objcon);    
                        }
                        
                    }
                }
            }

            if (deptdic.Keys.Contains(deptCode))
            {
                return deptdic[deptCode];
            }
            else
            {
                throw new NullReferenceException("错误原因:科室编号:" + deptCode + ",没有对照!");    
                return null;
                //Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("CASEDEPT", deptCode);
                
                //if (obj == null)
                //{
                //    throw new Exception("科室代码"+deptCode+"没有维护!");
                //    return null;
                //}
                //else
                //{
                //    Neusoft.HISFC.Models.Base.Const con = obj as Neusoft.HISFC.Models.Base.Const;
                //    deptdic.Add(deptCode, con);
                //    return obj;
                //}
            }
            
        }

        /// <summary>
        /// 取手术级别
        /// </summary>
        /// <param name="deptCode"></param>
        public Neusoft.HISFC.Models.Base.Const ConverOperationL(string OperationID)
        {
            if (this.server == null)
                return null;
            //SQLServerManager server = new SQLServerManager();
            DataSet ds = new DataSet();
            string sql = @"
                        select 
                        p.fopjb,
                        CASE  p.fopjb 
                        when '一级' then '1'
                        when '二级' then '2'
                        when '三级' then '3'
                        when '四级' then '4'
                        else ''
                        end
                        from tOperate p 
                        where p.fopcode='{0}'
                        AND P.fopversion='2020'
                        order by p.fopversion
                        ";
            // {F4163568-BD1F-475e-BB0F-94DADC5C7402}   加入条件AND P.fopversion='2018'
            sql = string.Format(sql, OperationID);
            int result = server.Execute(sql, ref ds);
            if (result < 0)
            {
                //server.Rollback();
                this.Erro = "获取手术级别失败!";
                return null;
            }
            //server.Commit();
            Neusoft.HISFC.Models.Base.Const newcon = new Neusoft.HISFC.Models.Base.Const();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                newcon.ID = dr[1].ToString();
                newcon.Name = dr[0].ToString();
            }
            return newcon;
        }

        public string GetOperationLevelNameByCode(string OperationLevelCode)
        {
            string levelName = string.Empty;
            switch (OperationLevelCode)
            {
                case "1":
                    levelName = "一级";
                    break;
                case "2":
                    levelName = "二级";
                    break;
                case "3":
                    levelName = "三级";
                    break;
                case "4":
                    levelName = "四级";
                    break;
                default:
                    break;
            }
            return levelName;
        }


        /// <summary>
        /// 员工对照
        /// </summary>
        /// <param name="DocCode">员工号</param>
        public Neusoft.HISFC.Models.Base.Const ConverDoc(string DocCode)
        {
            //测试用,没有对照
            //Neusoft.HISFC.Models.Base.Const testcon = new Neusoft.HISFC.Models.Base.Const();
            //return testcon;

            if (doctordic == null || doctordic.Count == 0)
            {
                ArrayList al = this.constMana.GetAllList("CASEDOCTOR");
                if (al == null)
                {
                    return null;
                }
                foreach (Neusoft.HISFC.Models.Base.Const objcon in al)
                {
                    doctordic.Add(objcon.UserCode, objcon);
                }
            }

            if (doctordic.Keys.Contains(DocCode))
            {
                return doctordic[DocCode];
            }
            else
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\d{6}");
                System.Text.RegularExpressions.Match machobj = regex.Match(DocCode);
                if (machobj == null || string.IsNullOrEmpty(machobj.Value))
                {
                    Neusoft.HISFC.Models.Base.Const notExist = new Neusoft.HISFC.Models.Base.Const();
                    notExist.ID = "-";
                    notExist.Name = "-";
                    notExist.UserCode = DocCode;
                    notExist.Memo = "_";
                    doctordic.Add(DocCode, notExist);
                    return notExist;
                }

                Neusoft.FrameWork.Models.NeuObject obj = GetDoctInfoByCode(DocCode);//根据工号查询
                if (obj == null || string.IsNullOrEmpty(obj.ID))
                {
                    string name = docMana.GetPersonByID(DocCode).Name;//工号查不到,只能用姓名查了.....
                    obj = GetDoctInfoByName(name);
                    if (obj == null || string.IsNullOrEmpty(obj.ID))//用姓名查询都没有数据,只能用本地的了.(不要怪我,什么对照方法都没有跟我说,我也只能这么干了!!)
                    {
                        Neusoft.HISFC.Models.Base.Employee empl = docMana.GetPersonByID(DocCode);
                        Neusoft.HISFC.Models.Base.Const locldoc = new Neusoft.HISFC.Models.Base.Const();
                        locldoc.ID = "";//不上传本地工号
                        locldoc.Name = empl.Name;
                        locldoc.UserCode = empl.ID;
                        locldoc.Memo = empl.Name;
                        doctordic.Add(DocCode, locldoc);
                        return locldoc;
                    }
                }
                Neusoft.HISFC.Models.Base.Const con = new Neusoft.HISFC.Models.Base.Const();
                con.ID = obj.ID;
                con.Name = obj.Name;
                con.UserCode = DocCode;
                con.Memo = obj.Name;
                doctordic.Add(DocCode, con);
                return con;

                //Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("CASEDOCTOR", DocCode);
                //if (obj == null)
                //{
                //    obj = GetDoctInfo(DocCode);
                //    if (obj == null || string.IsNullOrEmpty(obj.ID))
                //    {
                //        throw new Exception("医生工号" + DocCode + "没有维护!");
                //        return null;
                //    }
                //    else
                //    {
                //        Neusoft.HISFC.Models.Base.Const con = new Neusoft.HISFC.Models.Base.Const();
                //        con.ID = DocCode;
                //        con.Name = obj.Name;
                //        con.UserCode = obj.ID;
                //        doctordic.Add(DocCode, con);
                //        return obj;
                //    }
                //}
                //else
                //{
                //    Neusoft.HISFC.Models.Base.Const con = obj as Neusoft.HISFC.Models.Base.Const;
                //    doctordic.Add(DocCode, con);
                //    return obj;
                //}
            }
        }

        #endregion


        #region 转换
        public string PatientNoChang(string patientNo)
        {
            string ret = string.Empty;
            //是否需要转换
            ArrayList al = this.constMana.GetList("CasePatientNoChang");
            if (al != null && al.Count > 0)//需要转换
            {
                ret = patientNo.Replace("v", "V");

                if (ret.IndexOf("V") >= 0)
                {
                    ret = ret.Replace("V", "0");
                    ret = "V" + ret.Substring(1);
                }
                else
                {
                    ret = patientNo;
                }
            }
            else
            {
                ret = patientNo;
            }
            return ret;
        }

        /// <summary>
        ///  上传病案号位数
        ///  没有设置常数：返回8位 否则按照实际返回
        /// </summary>
        /// <returns></returns>
        public int PatientNoSubstr()
        {
            int ret = 2;//8位 
            Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("CASEPNOSUBSTR", "1");
            //无维护情况上传8位
            if (obj == null)
            {
                ret = 2;
                return ret;
            }
            if (obj.Memo == "")
            {
                ret = 2;
                return ret;
            }
            else if (obj.Memo.ToUpper() == "TRUE")
            {
                ret = 2;
                return ret;
            }
            else
            {
                int uplaodNum = 0;
                try
                {
                    uplaodNum = Neusoft.FrameWork.Function.NConvert.ToInt32(obj.Memo);
                }
                catch
                {
                    uplaodNum = 0;
                }
                if (uplaodNum == 0)
                {
                    ret = 2;
                    return ret;
                }
                else
                {
                    ret = 10 - uplaodNum;
                }
            }
            return ret;
        }

        /// <summary>
        /// 将“ '” 转换成 “’”
        /// </summary>
        /// <param name="Character"></param>
        /// <returns></returns>
        public string ChangeCharacter(string Character)
        {
            Character = Character.Replace("'", "’");
            return Character;
        }

        /// <summary>
        /// 时间转换
        /// sqlserver 不认0001-01-01
        /// </summary>
        /// <param name="dtStr"></param>
        /// <returns></returns>
        public string ChangeDateTime(string dtStr)
        {
            string retStr = string.Empty;
            DateTime dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(dtStr);

            if (dt.Date.Year < 1990)
            {
                retStr = "NULL";
            }
            else
            {
                retStr = dtStr;
            }
            return retStr;
        }
        #endregion


    }
}
