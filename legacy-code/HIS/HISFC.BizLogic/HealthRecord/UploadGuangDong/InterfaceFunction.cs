using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDong
{
    /// <summary>
    /// InterfaceFunction<br></br>
    /// [功能描述: 病案接口SQL]<br></br>
    /// [创 建 者: 王 立]<br></br>
    /// [创建时间: 2008-04-08]<br></br>
    /// <修改记录
    ///		修改人='新病案系统接口'
    ///		修改时间='2009-5-30'
    ///		修改目的='新系统接口'
    ///		修改描述='何荣'
    ///  />
    /// </summary>
    public class InterfaceFunction : Neusoft.FrameWork.Management.Database
    {
        Neusoft.HISFC.BizLogic.Manager.Constant constMana = new Neusoft.HISFC.BizLogic.Manager.Constant();

        Neusoft.HISFC.BizLogic.Manager.Department deptMana = new Neusoft.HISFC.BizLogic.Manager.Department();

        Neusoft.HISFC.BizLogic.HealthRecord.Base baseMgr = new Neusoft.HISFC.BizLogic.HealthRecord.Base();
        int fid = 0;
        #region ba2

        /// <summary>
        /// 将病案首页基本信息实体 转变成字符串数组
        /// <param name="b"> 病案的实体类</param>
        /// <param name="alFee">患者费用</param>
        /// <param name="alChangeDepe">患者转科信息</param>
        /// <param name="alDose">患者诊断信息</param>
        /// <returns>失败返回null</returns>
        /// </summary>
        public string[] GetBaseInfo(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose)
        {
            //string[] s = new string[198];
            //try
            //{
            //    s[0] = "0";
            //    s[1] = b.PatientInfo.PID.PatientNO.Substring(4);//病案号
            //    s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
            //    s[3] = "10";
            //    s[4] = b.PatientInfo.ID;
            //    s[5] = b.PatientInfo.Age.ToString();//年龄
            //    s[6] = b.PatientInfo.Name;//姓名
            //    s[7] = b.PatientInfo.Sex.Name.Equals("男") ? "1" : "2";
            //    s[8] = b.PatientInfo.Sex.Name;
            //    s[9] = b.PatientInfo.Birthday.ToShortDateString().Replace('-', '/');

            //    s[10] = b.PatientInfo.AreaCode;//就是籍贯，存编码？
            //    s[11] = b.PatientInfo.IDCard;
            //    s[12] = this.constMana.GetConstant("COUNTRY", b.PatientInfo.Country.ID).Memo;
            //    s[13] = b.PatientInfo.Country.Name;
            //    s[14] = b.PatientInfo.Nationality.ID;
            //    s[15] = b.PatientInfo.Nationality.Name;
            //    s[16] = this.constMana.GetConstant("PROFESSION", b.PatientInfo.Profession.ID).Name;
            //    // s [17] = b.PatientInfo.MaritalStatus.ID.ToString ( ).TrimStart('0');
            //    s[17] = b.PatientInfo.MaritalStatus.ID.ToString();
            //    switch (s[17])
            //    {
            //        case "1":
            //            s[18] = "未婚";
            //            break;
            //        case "2":
            //            s[18] = "已婚";
            //            break;
            //        case "3":
            //            s[18] = "离婚";
            //            break;
            //        case "4":
            //            s[18] = "其它";
            //            break;
            //        case "5":
            //            s[18] = "其它";
            //            break;
            //        case "6":
            //            s[18] = "丧偶";
            //            break;
            //    }
            //    //string tempmarital = this.constMana.GetConstant ( "CaseMarital", b.PatientInfo.MaritalStatus.ID.ToString ( ) ).Name;
            //    //s [17] = this.constMana.GetConstant ( "CaseMarital", b.PatientInfo.MaritalStatus.ID.ToString ( ) ).Name;
            //    //s [18] = this.constMana.GetConstant ( "MaritalStatus", tempmarital ).Name;

            //    //s[5] = b.PatientInfo.Profession.ID;//职业

            //    //s[18] = this.constMana.GetConstant("CaseMarital", b.PatientInfo.MaritalStatus.ID.ToString()).Name;
            //    //b.PatientInfo.MaritalStatus.ID.ToString();//婚姻状况
            //    s[19] = b.PatientInfo.AddressBusiness;  //工作单位及地址
            //    s[20] = b.PatientInfo.AddressBusiness;//单位地址
            //    s[21] = b.PatientInfo.PhoneBusiness;//单位电话
            //    s[22] = b.PatientInfo.BusinessZip;//单位邮编

            //    s[23] = b.PatientInfo.AddressHome;//家庭住址
            //    s[24] = b.PatientInfo.HomeZip;//住址邮编
            //    s[25] = b.PatientInfo.Kin.Name;//联系人


            //    //s[14] = b.PatientInfo.Kin.RelationLink;//与患者关系

            //    s[26] = this.constMana.GetConstant("RELATIVE", b.PatientInfo.Kin.RelationLink).Name;//与患者关系
            //    s[27] = b.PatientInfo.Kin.RelationAddress;//联系人地址

            //    s[28] = b.PatientInfo.Kin.RelationPhone;//联系人电话
            //    if (b.PatientInfo.Pact.ID == "1")
            //    {
            //        b.PatientInfo.Pact.PayKind.ID = "01";
            //        s[31] = "";
            //        s[32] = "";
            //    }
            //    else if (b.PatientInfo.Pact.ID == "2" || b.PatientInfo.Pact.ID == "3")
            //    {
            //        b.PatientInfo.Pact.PayKind.ID = "02";
            //        s[31] = b.PatientInfo.SSN;
            //        s[32] = "";
            //    }
            //    else
            //    {
            //        b.PatientInfo.Pact.PayKind.ID = "03";
            //        s[31] = "";
            //        s[32] = b.PatientInfo.SSN;

            //    }
            //    string temppaykinds = this.constMana.GetConstant("CasePayKind", b.PatientInfo.Pact.PayKind.ID).Name;
            //    //[29] = b.PatientInfo.Pact.PayKind.ID;
            //    s[29] = this.constMana.GetConstant("CasePayKind", b.PatientInfo.Pact.PayKind.ID).Name;
            //    s[30] = this.constMana.GetConstant("UPLOADPACKIND", temppaykinds).Name;


            //    s[33] = b.PatientInfo.PVisit.InTime.ToShortDateString().Replace('-', '/');//入院日期
            //    s[34] = ""; //入院时间


            //    // s[19] = b.PatientInfo.PVisit.Circs.ID;//入院状态
            //    // s[20] = b.PatientInfo.PVisit.InSource.ID;//入院来源
            //    Neusoft.HISFC.Models.HealthRecord.Base baseObj = this.baseMgr.QueryInDept(b.PatientInfo.ID);
            //    s[35] = baseObj.InDept.ID;
            //    s[36] = baseObj.InDept.Name;
            //    //patientinfo.InDept.ID = pat.InDept.ID;
            //    //patientinfo.InDept.Name = pat.InDept.Name;

            //    //s[35] = this.ConverDept(b.InDept.ID);//入院科室代码
            //    //s[36] = b.InDept.Name;//入院科室名称
            //    s[37] = b.VisiPeriodMonth;//入院病室


            //    s[38] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
            //    s[39] = "";


            //    Neusoft.HISFC.Models.HealthRecord.Base baseObject = this.baseMgr.QueryOutDept(b.PatientInfo.ID);
            //    s[40] = baseObject.OutDept.ID;
            //    s[41] = baseObject.OutDept.Name;
            //    //patientinfo.OutDept.ID = pati.OutDept.ID;
            //    //patientinfo.OutDept.Name = pati.OutDept.Name;

            //    //s[40] = this.ConverDept ( b.OutDept.ID );//出院科室代码
            //    //s[41] = b.OutDept.Name;//出院科室名称
            //    s[42] = b.VisiPeriodYear; //出院病室

            //    System.TimeSpan tt = b.PatientInfo.PVisit.OutTime - b.PatientInfo.PVisit.InTime;
            //    s[43] = tt.Days.ToString();//住院天数
            //    s[44] = b.ClinicDiag.ID;
            //    s[45] = b.ClinicDiag.Name;
            //    s[46] = b.ClinicDoc.ID;
            //    s[47] = b.ClinicDoc.Name;
            //    s[48] = b.PatientInfo.PVisit.Circs.ID;
            //    s[49] = this.constMana.GetConstant("INCIRCS", b.PatientInfo.PVisit.Circs.ID).Name;
            //    s[50] = b.InHospitalDiag.ID;
            //    s[51] = b.InHospitalDiag.Name;
            //    if (b.DiagDate < new DateTime(1900, 1, 1))
            //    {
            //        s[52] = "";
            //    }
            //    else
            //    {
            //        s[52] = b.DiagDate.ToShortDateString().Replace('-', '/');
            //        //确诊日期
            //    }
            //    s[156] = "0";
            //    foreach (Neusoft.HISFC.Object.HealthRecord.Diagnose obj in alDose)
            //    {
            //        if (obj.DiagInfo.DiagType.ID == "1")
            //        {
            //            if (obj.DiagInfo.ICD10.ID.Contains("B15") || obj.DiagInfo.ICD10.ID.Contains("B16") || obj.DiagInfo.ICD10.ID.Contains("B17") || obj.DiagInfo.ICD10.ID.Contains("B18") || obj.DiagInfo.ICD10.ID.Contains("B19"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("A15.0") || obj.DiagInfo.ICD10.ID.Contains("A15.1") || obj.DiagInfo.ICD10.ID.Contains("A15.2") || obj.DiagInfo.ICD10.ID.Contains("A16.0") || obj.DiagInfo.ICD10.ID.Contains("A15.0") || obj.DiagInfo.ICD10.ID.Contains("A16.1") || obj.DiagInfo.ICD10.ID.Contains("A16.2"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("I21"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("I50.0"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("J13") || obj.DiagInfo.ICD10.ID.Contains("J14") || obj.DiagInfo.ICD10.ID.Contains("J15"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("I27.9"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("K92.208"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("N04"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("E05"))
            //            {
            //                s[156] = "1";

            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("I61"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("I63"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("D60") || obj.DiagInfo.ICD10.ID.Contains("D61"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("C91.0") || obj.DiagInfo.ICD10.ID.Contains("C92.0") || obj.DiagInfo.ICD10.ID.Contains("C93.0") || obj.DiagInfo.ICD10.ID.Contains("C94.0") || obj.DiagInfo.ICD10.ID.Contains("C95.0"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("E04"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("K35"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("K81.0"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("K40"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("C16"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("C34.1") || obj.DiagInfo.ICD10.ID.Contains("C34.2") || obj.DiagInfo.ICD10.ID.Contains("C34.3") || obj.DiagInfo.ICD10.ID.Contains("C34.4") || obj.DiagInfo.ICD10.ID.Contains("C34.5") || obj.DiagInfo.ICD10.ID.Contains("C34.6") || obj.DiagInfo.ICD10.ID.Contains("C34.7") || obj.DiagInfo.ICD10.ID.Contains("C34.8") || obj.DiagInfo.ICD10.ID.Contains("C34.901"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("C15"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("36.1"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("C67"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("N40"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("S06"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("M51.202"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("J18.0"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("A04.903"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("D25"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("74"))
            //            {
            //                s[156] = "1";
            //            }
            //            else if (obj.DiagInfo.ICD10.ID.Contains("H25"))
            //            {
            //                s[156] = "1";
            //            }
            //            break;
            //        }

            //        //ArrayList temp = new ArrayList ( );

            //        //temp = visitIntergrate.GetVisitConst ( "BUFENDIAGNOSE" );

            //        //for ( int j = 0; j < temp.Count; j++ )
            //        //{
            //        //    Neusoft.FrameWork.Object.NeuObject obj1 = temp [j] as Neusoft.FrameWork.Object.NeuObject;
            //        //    obj1.Name = obj1.Name.Trim ( );
            //        //    if ( obj1.Name == obj.DiagInfo.ICD10.Name )
            //        //    {
            //        //        s [156] = "1";//是否部分病种
            //        //        break;

            //        //    }
            //        //    else
            //        //    {
            //        //        s [156] = "0";//是否部分病种
            //        //    }
            //        //}
            //    }

            //    //foreach ( Neusoft.HISFC.Object.HealthRecord.Diagnose obj in alDose )
            //    //{

            //    //    ArrayList temp = new ArrayList ( );

            //    //    temp = visitIntergrate.GetVisitConst ( "SINGELDIAGNOSE" );

            //    //    for ( int j = 0; j < temp.Count; j++ )
            //    //    {
            //    //        Neusoft.FrameWork.Object.NeuObject obj1 = temp [j] as Neusoft.FrameWork.Object.NeuObject;
            //    //        obj1.Name = obj1.Name.Trim ( );
            //    //        if ( obj1.Name == obj.DiagInfo.ICD10.Name )
            //    //        {
            //    //            s [161] = "1";//是否单病种
            //    //            break;

            //    //        }
            //    //        else
            //    //        {
            //    //            s [161] = "0";//是否单病种
            //    //        }
            //    //    }
            //    //    if ( temp.Count == 0 )
            //    //    {
            //    //        s [161] = "0";
            //    //    }
            //    //}

            //    foreach (Neusoft.HISFC.Object.HealthRecord.Diagnose obj in alDose)
            //    {
            //        if (obj.DiagInfo.DiagType.ID == "6")
            //        {
            //            s[53] = obj.DiagInfo.ICD10.Name;
            //            break;
            //        }


            //    }

            //    s[54] = b.FirstAnaphyPharmacy.Name;//药物过敏
            //    s[55] = b.Hbsag;
            //    s[56] = this.constMana.GetConstant("HbsAg", b.Hbsag).Name;
            //    s[57] = b.HcvAb;
            //    s[58] = this.constMana.GetConstant("HbsAg", b.HcvAb).Name;
            //    s[59] = b.HivAb;
            //    s[60] = this.constMana.GetConstant("HbsAg", b.HivAb).Name;
            //    s[61] = b.CePi;
            //    s[62] = this.constMana.GetConstant("YesOrNo", b.CePi).Name;
            //    s[63] = b.PiPo;
            //    s[64] = this.constMana.GetConstant("YesOrNo", b.PiPo).Name;
            //    s[65] = b.ClPa;
            //    s[66] = this.constMana.GetConstant("YesOrNo", b.ClPa).Name;
            //    s[67] = b.FsBl;
            //    s[68] = this.constMana.GetConstant("YesOrNo", b.FsBl).Name;
            //    s[69] = b.OpbOpa;
            //    s[70] = this.constMana.GetConstant("YesOrNo", b.OpbOpa).Name;

            //    s[71] = b.SalvTimes.ToString();//抢救次数
            //    s[72] = b.SuccTimes.ToString();//成功次数
            //    s[73] = b.PatientInfo.PVisit.ReferringDoctor.ID;
            //    s[74] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称
            //    s[75] = b.PatientInfo.PVisit.ConsultingDoctor.ID;
            //    s[76] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名
            //    s[77] = b.PatientInfo.PVisit.AttendingDoctor.ID;//主治医师姓名
            //    s[78] = b.PatientInfo.PVisit.AttendingDoctor.Name;
            //    s[79] = b.PatientInfo.PVisit.AdmittingDoctor.ID;//住院医师姓名
            //    s[80] = b.PatientInfo.PVisit.AdmittingDoctor.Name;
            //    s[81] = b.RefresherDoc.ID;//进修医生
            //    s[82] = b.RefresherDoc.Name;
            //    s[83] = b.GraduateDoc.ID;//研究生实习医师名称
            //    s[84] = b.GraduateDoc.Name;
            //    s[85] = b.PatientInfo.PVisit.TempDoctor.ID;
            //    s[86] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
            //    s[87] = b.CodingOper.ID;//编码员名称
            //    s[88] = b.CodingOper.Name;
            //    s[89] = b.OperInfo.ID;
            //    s[90] = b.OperInfo.Name;//操作员名称（病案整理者）
            //    s[91] = "1";//b.MrQuality;//病案质量 
            //    s[92] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
            //    s[93] = b.QcDoc.ID;//质控医师名称
            //    s[94] = b.QcDoc.Name;

            //    //s[14] = b.PatientInfo.Kin.RelationLink;//与患者关系

            //    s[95] = b.QcNurse.ID;
            //    s[96] = b.QcNurse.Name;//质控护士名称
            //    if (b.CheckDate < new DateTime(1900, 1, 1))
            //    {
            //        s[97] = "1900/1/1";
            //    }
            //    else
            //    {
            //        s[97] = b.CheckDate.ToShortDateString().Replace('-', '/');//质控日期
            //    }
            //    s[98] = "";//是否因麻醉死亡编号
            //    s[99] = "";//是否因麻醉死亡




            //    for (int j = 100; j <= 118; j++)
            //    {
            //        s[j] = "0.00";
            //    }

            //    decimal feeTot = 0.0M;
            //    decimal feeOther = 0.0M;
            //    foreach (Neusoft.HISFC.Object.RADT.Patient feeInfo in alFee)
            //    {
            //        decimal fee1 = 0.0M;
            //        fee1 = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeInfo.IDCard), 2);

            //        feeTot += fee1;

            //        string fee = fee1.ToString();

            //        if (feeInfo.DIST == "1")//床位费
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[101]) + fee1;
            //            s[101] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "2")//护理费
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[102]) + fee1;
            //            s[102] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "3")//西药费
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[103]) + fee1;
            //            s[103] = temp.ToString();
            //        }

            //        else if (feeInfo.DIST == "5")//中草药费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]) + fee1;
            //            s[106] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "6")//放射费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[107]) + fee1;
            //            s[107] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "13")//检查费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[114]) + fee1;
            //            s[114] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "4")//中成药费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + fee1;
            //            s[105] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "10")//诊疗费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[111]) + fee1;
            //            s[111] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "11")//手术费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[112]) + fee1;
            //            s[112] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "7")//化验费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[108]) + fee1;
            //            s[108] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "9")//输血费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[110]) + fee1;
            //            s[110] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "8")//输氧费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[109]) + fee1;
            //            s[109] = temp.ToString();
            //        }
            //        else if (feeInfo.DIST == "14")//麻醉费*
            //        {
            //            decimal temp = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[115]) + fee1;
            //            s[115] = temp.ToString();
            //        }
            //        else
            //        {
            //            feeOther += fee1;
            //        }
            //    }
            //    s[100] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeTot), 2).ToString();
            //    decimal tempZYF = Neusoft.FrameWork.Function.NConvert.ToDecimal(s[105]) + Neusoft.FrameWork.Function.NConvert.ToDecimal(s[106]);
            //    s[104] = tempZYF.ToString(); //中药费
            //    s[113] = "0.00";//接生费
            //    s[116] = "0.00";//婴儿费
            //    s[117] = "0.00";//陪床费
            //    s[118] = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(feeOther), 2).ToString();
            //    s[119] = b.CadaverCheck;//尸检
            //    s[120] = this.constMana.GetConstant("YesOrNo", b.CadaverCheck).Name;
            //    s[121] = b.YnFirst;
            //    s[122] = this.constMana.GetConstant("YesOrNo", b.YnFirst).Name;
            //    s[123] = b.YnFirst;
            //    s[124] = this.constMana.GetConstant("YesOrNo", b.YnFirst).Name;
            //    s[125] = b.YnFirst;
            //    s[126] = this.constMana.GetConstant("YesOrNo", b.YnFirst).Name;
            //    s[127] = b.YnFirst;
            //    s[128] = this.constMana.GetConstant("YesOrNo", b.YnFirst).Name;
            //    if (b.VisiStat == "0")
            //    {
            //        b.VisiStat = "2";
            //    }
            //    s[129] = b.VisiStat;
            //    s[130] = this.constMana.GetConstant("YesOrNo", b.VisiStat).Name;
            //    s[131] = b.VisiPeriodWeek;

            //    if (b.TechSerc == "0")
            //    {
            //        b.TechSerc = "2";
            //    }
            //    s[132] = b.TechSerc;//示教科研
            //    s[133] = this.constMana.GetConstant("YesOrNo", b.TechSerc).Name;
            //    s[134] = b.PatientInfo.BloodType.ID.ToString();//血型编码,格式有错
            //    if (s[134] == "AB")
            //        s[134] = "3";
            //    s[135] = this.constMana.GetConstant("BLOODTYPE", b.PatientInfo.BloodType.ID.ToString()).Name;
            //    s[136] = b.RhBlood;
            //    s[137] = this.constMana.GetConstant("RHSTATE", b.RhBlood).Name;
            //    s[138] = b.ReactionBlood;
            //    s[139] = this.constMana.GetConstant("YesOrNo", b.ReactionBlood).Name;
            //    s[140] = "2";
            //    s[141] = "无";
            //    try
            //    {
            //        s[142] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodRed).ToString();//红细胞数
            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[143] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodPlatelet).ToString();//血小板数

            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[144] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BodyAnotomize).ToString();//血浆数
            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[145] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodWhole).ToString();//全血数

            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[146] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodOther).ToString();//其他输血数

            //    }
            //    catch
            //    {
            //    }



            //    s[147] = b.InconNum.ToString();//院际会诊次数 70 远程会诊次数
            //    s[148] = b.OutconNum.ToString();//院际会诊次数 70 远程会诊次数
            //    s[149] = b.SuperNus.ToString(); //特级护理时间(小时)                         
            //    s[150] = b.INus.ToString(); //I级护理时间(日)                                     
            //    s[151] = b.IINus.ToString(); //II级护理时间(日)                                    
            //    s[152] = b.IIINus.ToString(); //III级护理时间(日)                                   
            //    s[153] = b.StrictNuss.ToString(); //重症监护时间( 小时)                                 

            //    s[154] = b.SpecalNus.ToString();  // 特殊护理(日) 

            //    s[155] = "0.00";//婴儿数
            //    //s [156] = "0";//是否部分病种
            //    if (b.SalvTimes > 0)
            //    {
            //        s[157] = "1";
            //        s[158] = "1";
            //    }
            //    else
            //    {
            //        s[157] = "0";
            //        s[158] = "0";
            //    }

            //    s[159] = "0";//是否三日确诊
            //    s[160] = "0";//是否月内再次住院
            //    s[161] = "0";//是否中度烧伤
            //    //s [161] = "0";
            //    s[162] = "0";//是否单病种
            //    s[163] = "0.00";//中医院治疗费(预留字段)

            //    if (alChangeDepe.Count > 0)
            //    {
            //        Neusoft.HISFC.Object.RADT.Location dept = alChangeDepe[0] as Neusoft.HISFC.Object.RADT.Location;
            //        s[164] = this.ConverDept(dept.Dept.ID);
            //        s[165] = dept.Dept.Name;
            //    }
            //    else
            //    {
            //        s[164] = "";
            //        s[165] = "";
            //    }
            //    try
            //    {

            //        s[166] = Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.HISFC.Object.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');
            //    }
            //    catch
            //    {
            //    }
            //    s[167] = "";
            //    s[168] = "";
            //    s[169] = "";
            //    s[170] = this.GetDateTimeFromSysDateTime().ToShortDateString().Replace('-', '/');
            //    s[171] = "";
            //    s[172] = "";
            //    s[173] = "";
            //    s[174] = "";
            //    s[175] = b.PatientInfo.PVisit.InSource.ID;//入院来源
            //    s[176] = this.constMana.GetConstant("INAVENUE", b.PatientInfo.PVisit.InSource.ID).Name;
            //    if (b.FirstOperation.ID.Trim() == "")
            //    {
            //        s[177] = "0";
            //    }
            //    else
            //    {
            //        s[177] = "1";
            //    }
            //    s[178] = "0";
            //    if (b.SyndromeFlag == null || b.SyndromeFlag == "")
            //    {
            //        s[179] = "0";
            //    }
            //    else
            //    {
            //        s[179] = b.SyndromeFlag;
            //    }
            //    s[180] = b.InfectionNum.ToString();
            //    s[181] = "0";//状态
            //    s[182] = "";//数字验证
            //    s[183] = "";
            //    s[184] = "";
            //    s[185] = "";
            //    s[186] = "";
            //    s[187] = "";
            //    s[188] = "";
            //    s[189] = "";
            //    s[190] = "";
            //    s[191] = "";
            //    s[192] = "";
            //    s[193] = "";
            //    s[194] = "";
            //    s[195] = "";
            //    s[196] = "";
            //    s[197] = "";
            //    return s;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            return null;
        }

        /// <summary>
        /// HIS_BA1 --病人住院信息
        /// </summary>
        /// <returns></returns>
        public string GetInsertba2SQL(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose)
        {
//            if (b == null)
//            {
//                this.Err = "传入的实体不能为null";

//                return null;
//            }

//            string strReturn = string.Empty;

//            strReturn = @"INSERT INTO tPatientVisit
//  (
//   FPRN,
//   FTIMES,
//   FICDVersion,
//   FZYID,
//   FAGE,
//   FNAME,
//   FSEXBH,
//   FSEX,
//   FBIRTHDAY,
//   FBIRTHPLACE, --10
//   FIDCard,
//   fcountrybh,
//   fcountry,
//   fnationalitybh,
//   fnationality,
//   FJOB,
//   FSTATUSBH,
//   FSTATUS,
//   FDWNAME,
//   FDWADDR,
//   FDWTELE,
//   FDWPOST,
//   FHKADDR,
//   FHKPOST,
//   FLXNAME,
//   FRELATE,
//   FLXADDR,
//   FLXTELE,
//   FFBBH,
//   FFB,--30
//   FASCARD1,
//   FASCARD2,
//   FRYDATE,
//   FRYTIME,
//   FRYTYKH,
//   FRYDEPT,
//   FRYBS,
//   FCYDATE,
//   FCYTIME,
//   FCYTYKH,
//   FCYDEPT,
//   FCYBS,
//   FDAYS,
//   FMZZDBH,
//   FMZZD,
//   FMZDOCTBH,
//   FMZDOCT,
//   FRYINFOBH,
//   FRYINFO,
//   FRYZDBH,
//   FRYZD,
//   --FQZDATE,
//   FPHZD,
//   FGMYW,
//   FHBSAGBH,
//   FHBSAG,
//   FHCVABBH,
//   FHCVAB,
//   FHIVABBH,
//   FHIVAB,
//   FMZCYACCOBH,--60
//   FMZCYACCO,
//   FRYCYACCOBH,
//   FRYCYACCO,
//   FLCBLACCOBH,
//   FLCBLACCO,
//   FFSBLACCOBH,
//   FFSBLACCO,
//   FOPACCOBH,
//   FOPACCO,
//   /*FQJTIMES,
//   FQJSUCTIMES,*/
//   FKZRBH,
//   FKZR,
//   FZRDOCTBH,
//   FZRDOCTOR,
//   FZZDOCTBH,
//   FZZDOCT,
//   FZYDOCTBH,
//   FZYDOCT,
//   FJXDOCTBH,--80
//   FJXDOCT,
//   FYJSSXDOCTBH,
//   FYJSSXDOCT,
//   FSXDOCTBH,
//   FSXDOCT,
//   FBMYBH,
//   FBMY,
//   FZLRBH,
//   FZLR,
//   FQUALITYBH,
//   FQUALITY,
//   FZKDOCTBH,
//   FZKDOCT,
//   FZKNURSEBH,
//   FZKNURSE,
//   FZKRQ,
//   FMZDEADBH,
//   FMZDEAD,
//   FSUM1,
//   FCWF,--100
//   FHLF,
//   FXYF,
//   FZYF,
//   FZCHYF,
//   FZCYF,
//   FFSF,
//   FHYF,
//   FSYF,
//   FSXF,
//   FZLF,
//   FSSF,
//   FJSF,
//   FJCF,
//   FMZF,
//   FYEF,
//   FPCF,
//   FQTF,
//   FBODYBH,
//   FBODY,
//   FISOPFIRSTBH,--120
//   FISOPFIRST,
//   FISZLFIRSTBH,
//   FISZLFIRST,
//   FISJCFIRSTBH,
//   FISJCFIRST,
//   FISZDFIRSTBH,
//   FISZDFIRST,
//   FISSZBH,
//   FISSZ,
//   FSZQX,
//   FSAMPLEBH,
//   FSAMPLE,
//   FBLOODBH,
//   FBLOOD,
//   FRHBH,
//   FRH,
//   FSXFYBH,
//   FSXFY,
//   FSYFYBH,
//   FSYFY,--140
//   /*FREDCELL,
//   FPLAQUE,
//   FSEROUS,
//   FALLBLOOD,
//   FOTHERBLOOD,*/
//   FHZYJ,
//   FHZYC,
//   FHLTJ,
//   FHL1,
//   FHL2,
//   FHL3,
//   FHLZZ,
//   FHLTS,
//   FBABYNUM,
//   FTWILL,
//   FQJBR,
//   FQJSUC,
//   FTHREQZ,
//   FBACK,
//   FIFZDSS,--160
//   FIFDBZ,
//   FZLFZY,
//   FZKTYKH,
//   FZKDEPT,
//   FZKDATE,
//   FZKTIME,
//   FSRYBH,
//   FSRY,
//   FWORKRQ,
//   FJBFXBH,
//   FJBFX,
//   FFHGDBH,
//   FFHGD,
//   FSOURCEBH,
//   FSOURCE,
//   FIFSS,
//   FIFFYK,
//   FBFZ,
//   FYNGR,
//  FFlag,
//  FDATACHECK,
//   FEXTEND1,--180
//   FEXTEND2,
//   FEXTEND3,
//   FEXTEND4,
//   FEXTEND5,
//   FEXTEND6,
//   FEXTEND7,
//   FEXTEND8,
//   FEXTEND9,
//   FEXTEND10,
//   FEXTEND11,
//   FEXTEND12,
//   FEXTEND13,
//   FEXTEND14,
//   FEXTEND15)
//  VALUES
//  (
//
//'{1}',
//{2},
//{3},
//'{4}',
//'{5}',
//'{6}',
//'{7}',
//'{8}',
//'{9}',
//'{10}',
//'{11}',
//'{12}',
//'{13}',
//'{14}',
//'{15}',
//'{16}',
//'{17}',
//'{18}',
//'{19}',
//'{20}',
//'{21}',
//'{22}',
//'{23}',
//'{24}',
//'{25}',
//'{26}',
//'{27}',
//'{28}',
//'{29}',
//'{30}',
//'{31}',
//'{32}',
//'{33}',
//'{34}',
//'{35}',
//'{36}',
//'{37}',
//'{38}',
//'{39}',
//'{40}',
//'{41}',
//'{42}',
//{43},
//'{44}',
//'{45}',
//'{46}',
//'{47}',
//'{48}',
//'{49}',
//'{50}',
//'{51}',
//'{52}',
//'{53}',
//'{54}',
//'{55}',
//'{56}',
//'{57}',
//'{58}',
//'{59}',
//'{60}',
//'{61}',
//'{62}',
//'{63}',
//'{64}',
//'{65}',
//'{66}',
//'{67}',
//'{68}',
//'{69}',
//'{70}',
///*{71},
//{72},*/
//'{73}',
//'{74}',
//'{75}',
//'{76}',
//'{77}',
//'{78}',
//'{79}',
//'{80}',
//'{81}',
//'{82}',
//'{83}',
//'{84}',
//'{85}',
//'{86}',
//'{87}',
//'{88}',
//'{89}',
//'{90}',
//'{91}',
//'{92}',
//'{93}',
//'{94}',
//'{95}',
//'{96}',
//'{97}',
//'{98}',
//'{99}',
//{100},
//{101},
//{102},
//{103},
//{104},
//{105},
//{106},
//{107},
//{108},
//{109},
//{110},
//{111},
//{112},
//{113},
//{114},
//{115},
//{116},
//{117},
//{118},
//'{119}',
//'{120}',
//'{121}',
//'{122}',
//'{123}',
//'{124}',
//'{125}',
//'{126}',
//'{127}',
//'{128}',
//'{129}',
//'{130}',
//'{131}',
//'{132}',
//'{133}',
//'{134}',
//'{135}',
//'{136}',
//'{137}',
//'{138}',
//'{139}',
//'{140}',
//'{141}',
///*{142},
//{143},
//{144},
//{145},
//{146},
//{147},
//{148},
//{149},
//{150},
//{151},
//{152},
//{153},
//{154},
//{155},
//{156},
//{157},
//{158},
//{159},
//{160},
//{161},
//{162},
//{163},
//'{164}',
//'{165}',
//'{166}',
//'{167}',
//'{168}',
//'{169}',
//'{170}',
//'{171}',
//'{172}',
//'{173}',
//'{174}',
//'{175}',
//'{176}',
//{177},
//{178},
//{179},
//{180},
//'{181}',
//'{182}',
//'{183}',
//'{184}',
//'{185}',
//'{186}',
//'{187}',
//'{188}',
//'{189}',
//'{190}',
//'{191}',
//'{192}',
//'{193}',
//'{194}',
//'{195}',
//'{196}',
//'{197}'
//)";

//            try
//            {
//                string[] ss = this.GetBaseInfo(b, alFee, alChangeDepe, alDose);
//                for (int i = 0; i < ss.Length; i++)
//                {
//                    if (ss[i] == null)
//                    {
//                        ss[i] = "";
//                    }
//                }
//                strReturn = string.Format(strReturn, this.GetBaseInfo(b, alFee, alChangeDepe, alDose));
//            }
//            catch (Exception ex)
//            {
//                this.Err = "赋值时出错！" + ex.Message;

//                return null;
//            }

//            return strReturn;
            return null;
        }

        #endregion
        #region HIS_BA1
        public int GetCasUpload()
        {
            string strSQL = "select SEQ_CAS_UPLOAP.Nextval from dual";
            //返回最大的发生序号
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strSQL));
        }
        Neusoft.HISFC.Models.HealthRecord.Base baseObj;
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
                ArrayList alDiagnose = this.baseMgr.QueryCaseDiagnoseByInpatientNo(b.PatientInfo.ID);
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

                        Neusoft.HISFC.Models.RADT.Location indept = this.baseMgr.GetDeptIn(b.PatientInfo.ID);
                        if (indept != null) //入院科室 
                        {
                            s[35] = this.ConverDept(indept.Dept.ID);//入院科室代码
                            s[36] = this.ConverDeptName(indept.Dept.ID, indept.Dept.Name);//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//出院科室代码
                            s[41] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//出院科室名称
                        }
                        else
                        {
                            s[35] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//入院科室代码
                            s[36] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//出院科室代码
                            s[41] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//出院科室名称
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
                            s[164] = this.ConverDept(dept.Dept.ID);
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

                        Neusoft.HISFC.Models.RADT.Location indept = this.baseMgr.GetDeptIn(b.PatientInfo.ID);
                        if (indept != null) //入院科室 
                        {
                            s[35] = this.ConverDept(indept.Dept.ID);//入院科室代码
                            s[36] = this.ConverDeptName(indept.Dept.ID, indept.Dept.Name);//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//出院科室代码
                            s[41] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//出院科室名称
                        }
                        else
                        {
                            s[35] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//入院科室代码
                            s[36] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//出院科室名称
                            s[40] = this.ConverDept(b.PatientInfo.PVisit.PatientLocation.Dept.ID);//出院科室代码
                            s[41] = this.ConverDeptName(b.PatientInfo.PVisit.PatientLocation.Dept.ID, b.PatientInfo.PVisit.PatientLocation.Dept.Name);//出院科室名称
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
                            s[164] = this.ConverDept(dept.Dept.ID);
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
                            s[5] = this.baseMgr.GetAgeByFun(b.PatientInfo.Birthday.Date, b.PatientInfo.PVisit.InTime.Date);
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
                                s[26] = RelativeObj.Memo.Substring(0,20);//与患者关系
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
                    s[35] = this.ConverDept(b.InDept.ID);//入院科室代码
                    s[36] = this.ConverDeptName(b.InDept.ID, b.InDept.Name);//出院科室名称2011-6-8
                    s[37] = b.InRoom;//入院病室    
                    s[38] = b.PatientInfo.PVisit.OutTime.ToShortDateString().Replace('-', '/');//出院日期
                    s[39] = b.PatientInfo.PVisit.OutTime.ToShortTimeString(); //出院时间
                    s[40] = this.ConverDept(b.OutDept.ID);//出院科室代码
                    s[41] = this.ConverDeptName(b.OutDept.ID, b.OutDept.Name);//出院科室名称2011-6-8
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
                        ArrayList alDiagnose = this.baseMgr.QueryCaseDiagnoseByInpatientNo(b.PatientInfo.ID);

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

                    s[73] = this.ConverDoc(b.PatientInfo.PVisit.ReferringDoctor.ID);
                    s[74] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称PatientInfo.PVisit.ReferringDoctor.Name
                    s[75] = this.ConverDoc(b.PatientInfo.PVisit.ConsultingDoctor.ID);
                    s[76] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名PatientInfo.PVisit.ConsultingDoctor.Name
                    s[77] = this.ConverDoc(b.PatientInfo.PVisit.AttendingDoctor.ID);//主治医师姓名PatientInfo.PVisit.AttendingDoctor.Name
                    s[78] = b.PatientInfo.PVisit.AttendingDoctor.Name;
                    s[79] = this.ConverDoc(b.PatientInfo.PVisit.AdmittingDoctor.ID);//住院医师姓名PatientInfo.PVisit.AdmittingDoctor.Name
                    s[80] = b.PatientInfo.PVisit.AdmittingDoctor.Name;
                    s[81] = this.ConverDoc(b.RefresherDoc.ID);//进修医生
                    s[82] = b.RefresherDoc.Name;
                    s[83] = this.ConverDoc(b.GraduateDoc.ID);//研究生实习医师名称
                    s[84] = b.GraduateDoc.Name;
                    s[85] = this.ConverDoc(b.PatientInfo.PVisit.TempDoctor.ID);
                    s[86] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称
                    s[87] = this.ConverDoc(b.CodingOper.ID);//编码员名称
                    s[88] = b.CodingOper.Name;
                    s[89] = this.ConverDoc(b.OperInfo.ID);
                    s[90] = b.OperInfo.Name;//操作员名称（病案整理者）
                    s[91] = b.MrQuality.ToString();//1";//b.MrQuality;//病案质量 
                    s[92] = this.constMana.GetConstant("CASEQUALITY", b.MrQuality).Name;
                    s[93] = this.ConverDoc(b.QcDoc.ID);//质控医师名称
                    s[94] = b.QcDoc.Name;
                    s[95] = this.ConverDoc(b.QcNurse.ID);
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
                        s[164] = this.ConverDept(dept.Dept.ID);
                        s[165] = dept.Dept.Name;
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
                strReturn = @"INSERT INTO HIS_BA1
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
   FEXTEND15,
   Fifinput)
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
'{195}',
'{196}'
)";
            }
            else
            {
                strReturn = @"INSERT INTO HIS_BA1
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
   FEXTEND15,
   Fifinput)
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
'{195}',
'{196}'
)";
            }
            try
            {
                
                strReturn = string.Format(strReturn, this.GetBaseInfoBA1(b, alFee, alChangeDepe, alDose,isMetCasBase));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        #endregion
        #region ba6--新病案系统已经把这部分整合在HIS_BA1中，先把函数里面内容屏蔽

        /// <summary>
        /// 将病案首页基本信息实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案的实体类</param>
        /// <returns>失败返回null</returns>
        public string[] GetBaseInfoBa6(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose)
        {
            //string[] s = new string[41];
            //try
            //{
            //    s[0] = b.PatientInfo.PID.PatientNO.Substring(3);//病案号


            //    s[1] = b.PatientInfo.Name;//姓名
            //    s[2] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
            //    //s[3] = b.PatientInfo.Pact.PayKind.ID;

            //    s[3] = b.InDept.Name;
            //    s[4] = b.OutDept.Name;
            //    s[5] = b.ClinicDiag.ID;

            //    s[6] = b.InHospitalDiag.ID;
            //    s[7] = b.Hbsag;
            //    s[8] = b.HcvAb;
            //    s[9] = b.HivAb;
            //    s[10] = b.PatientInfo.PVisit.ReferringDoctor.Name;

            //    s[11] = b.RefresherDoc.Name;//进修医生
            //    s[12] = b.GraduateDoc.Name;//研究生实习医师名称

            //    s[13] = b.QcDoc.Name;//质控医师名称


            //    //s[14] = b.PatientInfo.Kin.RelationLink;//与患者关系


            //    s[14] = b.QcNurse.Name;//质控护士名称
            //    //s[15] = b.CheckDate.ToShortDateString().Replace('-','/');//.ToString();//检查时间

            //    s[15] = "";


            //    s[16] = "0.00";
            //    s[17] = "0.00";
            //    s[18] = "0.00";
            //    s[19] = "0.00";
            //    s[20] = b.RhBlood;

            //    try
            //    {

            //        s[21] = Neusoft.FrameWork.Function.NConvert.ToDateTime(((Neusoft.HISFC.Models.RADT.Location)alChangeDepe[0]).User01).ToShortDateString().Replace('-', '/');
            //    }
            //    catch
            //    {
            //    }
            //    s[22] = "";

            //    if (b.YnFirst == "0")
            //    {
            //        b.YnFirst = "2";
            //    }

            //    s[23] = b.YnFirst;
            //    s[24] = b.YnFirst;

            //    s[25] = b.YnFirst;

            //    s[26] = b.YnFirst;

            //    try
            //    {
            //        s[27] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodRed).ToString();//红细胞数
            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[28] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodPlatelet).ToString();//血小板数

            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[29] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BodyAnotomize).ToString();//血浆数
            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[30] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodWhole).ToString();//全血数

            //    }
            //    catch
            //    {
            //    }

            //    try
            //    {
            //        s[31] = Neusoft.FrameWork.Function.NConvert.ToDecimal(b.BloodOther).ToString();//其他输血数

            //    }
            //    catch
            //    {
            //    }

            //    s[32] = "";

            //    s[33] = b.InconNum.ToString();//院际会诊次数 70 远程会诊次数

            //    s[34] = b.OutconNum.ToString();//院际会诊次数 70 远程会诊次数

                
            //    s[35] = b.SuperNus.ToString(); //特级护理时间(小时)                         
            //    s[36] = b.INus.ToString(); //I级护理时间(日)                                     
            //    s[37] = b.IINus.ToString(); //II级护理时间(日)                                    
            //    s[38] = b.IIINus.ToString(); //III级护理时间(日)                                   
            //    s[39] = b.StrictNuss.ToString(); //重症监护时间( 小时)                                 

            //    s[40] = b.SpecalNus.ToString();  // 特殊护理(日) 
                
                
            //    return s;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            return null;

        }

        /// <summary>
        /// insert into ba2
        /// </summary>
        /// <returns></returns>
        public string GetInsertba6SQL(Neusoft.HISFC.Models.HealthRecord.Base b, System.Collections.ArrayList alFee,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose)
        {
//            if (b == null)
//            {
//                this.Err = "传入的实体不能为null";

//                return null;
//            }

//            string strReturn = string.Empty;

//            strReturn = @"INSERT INTO ba6
//  (PRN,
//   NAME,
//   TIMES,
//   RYBS,
//   CYBS,
//   MZZD10,
//   RYZD10,
//   HBsAg,
//   HCV_Ab,
//   HIV_Ab,
//   KZR,
//   JXDOCT,
//   YSXDOCT,
//   ZKDOCT,
//   ZKNURSE,
//   ZKRQ,
//   HLF,
//   MZF,
//   YEF,
//   PCF,
//   RH,
//   ZKDATE,
//   ZKTIME,
//   ISOPFIRST,
//   ISZLFIRST,
//   ISJCFIRST,
//   ISZDFIRST,
//   REDCELL,
//   PLAQUE,
//   SEROUS,
//   ALLBLOOD,
//   OTHERBLOOD,
//   INPUTMAN,
//   HZ_YJ,
//   HZ_YC,
//   HL_TJ,
//   HL_I,
//   HL_II,
//   HL_III,
//   HL_ZZ,
//   HL_TS)
//VALUES
//  (
//'{0}',
//'{1}',
//'{2}',
//'{3}',
//'{4}',
//'{5}',
//'{6}',
//'{7}',
//'{8}',
//'{9}',
//'{10}',
//'{11}',
//'{12}',
//'{13}',
//'{14}',
//'{15}',
//{16},
//{17},
//{18},
//{19},
//'{20}',
//'{21}',
//'{22}',
//'{23}',
//'{24}',
//'{25}',
//'{26}',
//'{27}',
//{28},
//{29},
//{30},
//{31},
//'{32}',
//{33},
//{34},
//{35},
//{36},
//{37},
//{38},
//{39},
//{40}
//  )";

//            try
//            {
//                strReturn = string.Format(strReturn, this.GetBaseInfoBa6(b, alFee, alChangeDepe, alDose));
//            }
//            catch (Exception ex)
//            {
//                this.Err = "赋值时出错！" + ex.Message;

//                return null;
//            }

//            return strReturn;
            return null;
        }

        #endregion

        #region HIS_BA2 --病人转科情况

        /// <summary>
        /// HIS_BA2 --病人转科情况
        /// </summary>
        /// <param name="changDeptInfo"></param>
        /// <returns></returns>
        public string GetInsertba7SQL(Neusoft.HISFC.Models.RADT.Location  changDeptInfo, Neusoft.HISFC.Models.HealthRecord.Base patientInfo)
        {
            if (changDeptInfo == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            if (this.Sql.GetSql("Neusoft.HISFC.Management.HealthRecord.NewGetInsertba7SQL", ref strReturn) == -1)
            {
                this.Err = "没有找到Neusoft.HISFC.Management.HealthRecord.NewGetInsertba7SQL";

                return null;
            }

            try
            {
                strReturn = string.Format(strReturn, this.GetChangDeptInfo(changDeptInfo, patientInfo));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        /// <summary>
        /// 取转科参数

        /// </summary>
        /// <param name="changDeptInfo"></param>
        /// <returns></returns>
        public string[] GetChangDeptInfo(Neusoft.HISFC.Models.RADT.Location changDeptInfo, Neusoft.HISFC.Models.HealthRecord.Base patientInfo)
        {
            string[] s = new string[6];

            try
            {
                s[0] = patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());
                //s[1] = patientInfo.PatientInfo.Name;
                s[1] = patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0');
                s[2] = this.ConverDept(changDeptInfo.Dept.ID);//直接用回HIS常数中的对照关系，暂时不在新病案系统中处理
                s[3] = changDeptInfo.Dept.Name;
                s[4] = Neusoft.FrameWork.Function.NConvert.ToDateTime(changDeptInfo.User01).ToShortDateString().Replace('-', '/');;
                s[5] = "";

                return s;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region HIS_BA8  没写完，究竟用不用？

        /// <summary>
        /// 将病案首页基本信息实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案的实体类</param>
        /// <returns>失败返回null</returns>
        public string[] GetBaseInfoba8(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string[] s = new string[41];
            try
            {
                s[0] = b.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());//病案号
                s[1] = b.PatientInfo.InTimes.ToString().PadLeft(2, '0');//住院次数
                s[2] = b.PatientInfo.SSN;

                return s;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// HIS_BA8 --中医病人附件信息
        /// </summary>
        /// <returns></returns>
        public string GetInsertba8SQL(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            if (b == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            strReturn = @"INSERT INTO tChAdd
  (fprn,
   ftimes,
   fxxh,
   fryresourcebh,
   fryresource,
   fwyzdbh,
   fwyzd,
   fcyfsbh,
   fcyfs,
   fzllbbh,
   fzllb,
   fzzzybh,
   fzzzy,
   fmzzdzbbh,
   fmzzdzb,
   fmzzdzzbh,
   fmzzdzz,
   fryzdzbbh,
   fryzdzb,
   fryzdzzbh,
   fryzdzz,
   fphbh,
   fmzcybh,
   fmzcy,
   frycybh,
   frycy,
   fqjffbh,
   fqjff,
   fifwzbh,
   fifwz,
   fifjzbh,
   fifjz,
   fifynbh,
   fifyn,
   fswyybh,
   fswyy,
   fswdate,
   fswhour,
   fswminute,
   fkyblbh,
   fkybl)
VALUES
  (
'{0}',
{1},
'{2}',
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
'{40}'
  )";

            try
            {
                strReturn = string.Format(strReturn, this.GetBaseInfoba8(b));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        #endregion

        #region HIS_BA7 --肿瘤化疗药物

        /// <summary>
        /// insert into ba10
        /// </summary>
        /// <returns></returns>
        public string GetInsertba10SQL(Neusoft.HISFC.Models.HealthRecord.TumourDetail info, Neusoft.HISFC.Models.HealthRecord.Base patientInfo)
        {
            if (info == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            if (this.Sql.GetSql("Neusoft.HISFC.Management.HealthRecord.NewGetInsertba10SQL", ref strReturn) == -1)
            {
                this.Err = "没有找到Neusoft.HISFC.Management.HealthRecord.NewGetInsertba10SQL";

                return null;
            }

            try
            {
                strReturn = string.Format(strReturn, this.GetTumoutDetailInfo(info, patientInfo));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        /// <summary>
        /// 取肿瘤明细参数

        /// </summary>
        /// <param name="changDeptInfo"></param>
        /// <returns></returns>
        public string[] GetTumoutDetailInfo(Neusoft.HISFC.Models.HealthRecord.TumourDetail info, Neusoft.HISFC.Models.HealthRecord.Base patientInfo)
        {
            string[] s = new string[8];

            try
            {
                s[0] = patientInfo.PatientInfo.PID.PatientNO.Substring(this.PatientNoSubstr());
                s[1] = patientInfo.PatientInfo.InTimes.ToString().PadLeft(2, '0');
                s[2] = info.CureDate.ToShortDateString();
                s [3] = info.OperInfo.OperTime.ToShortDateString ( );
                s[4] = info.DrugInfo.Name; //+ "" + info.Qty + info.Unit;
                s[5] = info.Period;//
                s[6] = info.Result;//
                s[7] = this.constMana.GetConstant ( "RADIATERESULT", info.Result ).Name;
                return s;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region HIS_MZLOG9 -- 专科门诊病人数

        /// <summary>
        /// 将病案Mspecial实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案的实体类</param>
        /// <returns>失败返回null</returns>
        public string[] GetBaseInfoMspecial(Neusoft.HISFC.Models.Base.Employee  empl)
        {
            string[] s = new string[7];
            try
            {
                s[0] = empl.SpellCode.Replace ( '-', '/' );//日期
                s[1] = empl.ID;
                s[2] = empl.Name;
                s[3] = empl.User02;//专科号
                s[4] = empl.Memo;
                s[5] = empl.User01;
                s[6] = DateTime.Now.ToShortDateString().Replace('-', '/');//输入日期

                return s;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// insert into Mspecial
        /// </summary>
        /// <returns></returns>
        public string GetInsertMspecialSQL(Neusoft.HISFC.Models.Base.Employee empl)
        {
            if (empl == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            strReturn = @"INSERT INTO tSpecialLog  (FRQ,FTykh,FKsName,FTyzkcode,FzkName,FZlrs,FWorkrq)
VALUES  ('{0}','{1}','{2}',{3},'{4}',{5},'{6}' )";

            try
            {
                strReturn = string.Format(strReturn, this.GetBaseInfoMspecial(empl));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        #endregion

        #region HIS_MZLOG1 -- 科室门诊工作日志

        /// <summary>
        /// 将病案Mworklog1实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案Mworklog1的实体类</param>
        /// <returns>失败返回null</returns>
        public string[] GetBaseInfoMworklog1(Neusoft.HISFC.Models.Base.Employee empl)
        {
            string[] s = new string[14];
            try
            {
                s[0] = empl.IDCard.Replace ( '-', '/' );
                s[1] = empl.ID;
                s[2] = empl.Name;
                s[3] = empl.Memo;
                s[4] = empl.User01;
                s[5] = empl.User02;
                s[6] = empl.User03;
                s[7] = empl.SpellCode;
                s[8] = empl.WBCode;
                s[9] = "0";
                s[10] = "0";
                s[11] = "0";
                s[12] = "0";
                s[13] = DateTime.Now.ToShortDateString().Replace('-', '/');

                return s;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// insert into Mworklog1
        /// </summary>
        /// <returns></returns>
        public string GetInsertMworklog1SQL(Neusoft.HISFC.Models.Base.Employee empl)
        {
            if (empl == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            strReturn = @"INSERT INTO Mworklog1  (FRq,
FTykh,
FKsName,
FYsrs,
FZfzrys,
FGs,
FZlrc,
FZkmz,
FRc,
FJkjc,
FSsls,
FJtbc,
FQtrc,
FWorkrq
)
VALUES  ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},'{13}'  )";

            try
            {
                strReturn = string.Format(strReturn, this.GetBaseInfoMworklog1(empl));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }

        #endregion

        #region tWorklog -- 医生门诊工作日志（HIS_MZLOG2不支持写入操作，暂时不能插入到这个表中）

        /// <summary>
        /// 将病案Mworklog2实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案Mworklog2的实体类</param>
        /// <returns>失败返回null</returns>
        public string[] GetBaseInfotWorkLog(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string[] s = new string[17];
            try
            {
                s[0] = b.PatientInfo.PVisit.InTime.ToString();
                s[1] = b.InDept.ID;//科室编号
                s[2] = b.InDept.Name;
                s[3] = b.PatientInfo.DoctorReceiver.ID;// 医生工号
                s[4] = b.PatientInfo.DoctorReceiver.Name;//医生名称
                s[5] = b.PatientInfo.DoctorReceiver.Memo;
                s[6] = b.OutDept.ID;
                s[7] = b.OutDept.Name;
                s[8] = b.PatientInfo.Memo;//工时 5
                s[9] = b.PatientInfo.User01;//诊疗人次
                s[10] = b.PatientInfo.User02;//其中：专科门诊
                s[11] = b.PatientInfo.User03;//其中：专家门诊
                s[12] = b.PatientInfo.UserCode;
                s[13] = b.PatientInfo.WBCode;
                s[14] = b.PatientInfo.PVisit.User01;
                s[15] = b.PatientInfo.PVisit.User02;
                s[16] = b.PatientInfo.PVisit.OutTime.ToString();

                return s;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetOldDeptCode(string newDeptCode,ref string s2)
        {
            string oldDeptCode = newDeptCode;
            try
            {
                oldDeptCode = this.constMana.GetConstant("DEPTUPLOADCLI", newDeptCode).Name;
                s2 = this.constMana.GetConstant("DEPTUPLOADCLI", newDeptCode).Memo;
            }
            catch
            {
            }
            if (oldDeptCode.Length > 6)
            {
                oldDeptCode = oldDeptCode.Substring(0, 4);
            }
            return oldDeptCode;
        }
       

        /// <summary>
        /// insert into tWorklog
        /// </summary>
        /// <returns></returns>
        public string GetInserttWorkLogSQL(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            if (b == null)
            {
                this.Err = "传入的实体不能为null";

                return null;
            }

            string strReturn = string.Empty;

            strReturn = @"INSERT INTO tWorklog (FRq,
FTykh,
FKsName,
FTygh,
FDoctorName,
FOccup,
FTyzkcode,
FzkName,
FGs,
FZlrc,
FZkmz,
FRc,
FJkjc,
FSsls,
FJtbc,
FQtrc,
FWorkrq
)
VALUES  ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},{10},{11},{12},{13},{14},{15},'{16}')";

            try
            {
                strReturn = string.Format(strReturn, this.GetBaseInfotWorkLog(b));
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;

                return null;
            }

            return strReturn;
        }


        #endregion

        /// <summary>
        /// 返回删除语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="inpatientNo">病历号</param>
        /// <param name="times">次数</param>
        /// <returns></returns>
        public string GetDeleteSQL(string tableName, string inpatientNo, string times)
        {
            string strSQL = string.Empty;

            strSQL = "delete from {0} where fprn = '{1}' and ftimes = '{2}'";
            try
            {
                strSQL = string.Format(strSQL, tableName, inpatientNo, times);
            }
            catch(Exception ex)
            {
                this.Err = ex.Message;

                return null;
            }

            return strSQL;
        }

        /// <summary>
        /// 取对照科室
        /// </summary>
        /// <param name="deptCode"></param>
        public string ConverDept(string deptCode)
        {
            Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("DEPTUPLOAD", deptCode);
            if (obj == null)
            {
                return deptCode;
            }
            string strReturn = obj.Memo;

            if (strReturn == "")
            {
                return deptCode;
            }
            else
            {
                return strReturn;
            }
        }

        /// <summary>
        /// 取对照科室名称
        /// 暂时先这样处理吧--获取正确的科室名称看看是否可以实现：上传的科室转换 2011-6-8 chengym
        /// </summary>
        /// <param name="deptCode"></param>
        /// <param name="deptName"></param>
        private string ConverDeptName(string deptCode,string deptName)
        {
            Neusoft.FrameWork.Models.NeuObject obj = this.constMana.GetConstant("DEPTUPLOAD1", deptCode);
            if (obj == null)
            {
                return deptName;
            }
            string strReturn = obj.Memo;

            if (strReturn == "")
            {
                return deptName;
            }
            else
            {
                return strReturn;
            }
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
        /// 取对照工号
        /// </summary>
        /// <param name="DocCode"></param>
        public string ConverDoc(string DocCode)
        {
            Neusoft.FrameWork.Models.NeuObject obj=this.constMana.GetConstant("DOCTORUPLOAD", DocCode);
            if (obj == null)
            {
                return DocCode;
            }
            string strReturn = obj.Memo;

            if (strReturn == "")
            {
                return DocCode;
            }
            else
            {
                return strReturn;
            }
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

        /// <summary>
        ///  没有设置常数 返回8位
        ///  否则按照实际返回
        /// </summary>
        /// <returns></returns>
        public int PatientNoSubstr()
        {
            int ret = 2;//8位 
             Neusoft.FrameWork.Models.NeuObject obj= this.constMana.GetConstant("CASEPNOSUBSTR","1");
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
    }
}
