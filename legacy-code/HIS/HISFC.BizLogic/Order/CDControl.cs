using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.Management;
using Neusoft.HISFC.Models.Order;
using System;

namespace Neusoft.HISFC.BizLogic.Order
{
	public class CDControl : Neusoft.FrameWork.Management.Database
	{
		public CDNoticeModel GetItemMZ(string inpatient_no)
		{
			CDNoticeModel cDNoticeModel = null;
			string str = "";
			string text = "";
			CDNoticeModel result;
			if (base.Sql.GetSql("Met.Ord.CDManager", ref str) == -1)
			{
				base.Err = "没有找到PMet.Ord.PCDRUGMZ字段!";
				result = null;
			}
			else if (base.Sql.GetSql("Met.Ord.CDManager.Where", ref text) == -1)
			{
				base.Err = "没有找到Met.Ord.CDManager.Where字段!";
				result = null;
			}
			else
			{
				try
				{
					text = string.Format(text, inpatient_no);
				}
				catch
				{
					result = null;
					return result;
				}
				try
				{
					base.ExecQuery(str + " " + text);
					while (base.Reader.Read())
					{
						cDNoticeModel = new CDNoticeModel();
						cDNoticeModel.Clinic_no = base.Reader[0].ToString();
						cDNoticeModel.P_patientno = base.Reader[1].ToString();
						cDNoticeModel.P_name = base.Reader[2].ToString();
						cDNoticeModel.Sex_code = base.Reader[3].ToString();
						cDNoticeModel.Idenno = base.Reader[4].ToString();
						cDNoticeModel.P_allergy = base.Reader[5].ToString();
						cDNoticeModel.Linkma_name = base.Reader[6].ToString();
						cDNoticeModel.Rela_name = base.Reader[7].ToString();
						cDNoticeModel.Linkman_tel = base.Reader[8].ToString();
						cDNoticeModel.Isck_01 = NConvert.ToBoolean(base.Reader[9].ToString());
						cDNoticeModel.Isck_02 = NConvert.ToBoolean(base.Reader[10].ToString());
						cDNoticeModel.Isck_03 = NConvert.ToBoolean(base.Reader[11].ToString());
						cDNoticeModel.Isck_04 = NConvert.ToBoolean(base.Reader[12].ToString());
						cDNoticeModel.Isck_05 = NConvert.ToBoolean(base.Reader[13].ToString());
						cDNoticeModel.Isck_06 = NConvert.ToBoolean(base.Reader[14].ToString());
						cDNoticeModel.Isck_07 = NConvert.ToBoolean(base.Reader[15].ToString());
						cDNoticeModel.Isck_08 = NConvert.ToBoolean(base.Reader[16].ToString());
						cDNoticeModel.Isck_09 = NConvert.ToBoolean(base.Reader[17].ToString());
						cDNoticeModel.Isck_10 = NConvert.ToBoolean(base.Reader[18].ToString());
						cDNoticeModel.Isck_11 = NConvert.ToBoolean(base.Reader[19].ToString());
						cDNoticeModel.Isck_12 = NConvert.ToBoolean(base.Reader[20].ToString());
						cDNoticeModel.Mo_oper = base.Reader[21].ToString();
						cDNoticeModel.Mo_date = NConvert.ToDateTime(base.Reader[22].ToString());
						cDNoticeModel.Oper_code = base.Reader[23].ToString();
						cDNoticeModel.Oper_date = NConvert.ToDateTime(base.Reader[24].ToString());
						cDNoticeModel.Mark = base.Reader[25].ToString();
						cDNoticeModel.Mark1 = base.Reader[26].ToString();
						cDNoticeModel.Mark2 = base.Reader[27].ToString();
						cDNoticeModel.Mark3 = base.Reader[28].ToString();
						cDNoticeModel.Mark4 = base.Reader[29].ToString();
					}
				}
				catch (Exception ex)
				{
					base.Err = "获取基本信息时，执行SQL语句出错！" + ex.Message;
					base.ErrCode = "-1";
					this.WriteErr();
					result = null;
					return result;
				}
				finally
				{
					base.Reader.Close();
				}
				result = cDNoticeModel;
			}
			return result;
		}

		public int UpdateCDNotice(CDNoticeModel info)
		{
			string text = @"  update MET_ORD_CHRONICDISEASE
set clinic_no = '{0}',
p_patientno  = '{1}',
p_name       = '{2}',
sex_code   = '{3}',
idenno  = '{4}',      
p_allergy    = '{5}',
linkma_name    = '{6}',
rela_name       = '{7}',
linkman_tel       = '{8}',   
isck_01       = '{9}',
isck_02       = '{10}',
isck_03       = '{11}',
isck_04       = '{12}',       
isck_05       = '{13}',
isck_06      = '{14}',
isck_07       = '{15}',
isck_08       = '{16}',      
isck_09       = '{17}',
isck_10      = '{18}',
isck_11       = '{19}',
isck_12      = '{20}',      
mo_oper      = '{21}',
mo_date      = to_date('{22}', 'yyyy-mm-dd HH24:mi:ss'),
oper_code    = '{23}',
oper_date    = to_date('{24}', 'yyyy-mm-dd HH24:mi:ss'),
mark         = '{25}',
mark1         = '{26}',
mark2         = '{27}',
mark3         = '{29}',
mark4         = '{29}'      
where clinic_no = '{0}'
";
			int num = 0;
			int result;
			try
			{
				text = string.Format(text, new string[]
				{
					info.Clinic_no,
					info.P_patientno,
					info.P_name,
					info.Sex_code,
					info.Idenno,
					info.P_allergy,
					info.Linkma_name,
					info.Rela_name,
					info.Linkman_tel,
					NConvert.ToInt32(info.Isck_01).ToString(),
					NConvert.ToInt32(info.Isck_02).ToString(),
					NConvert.ToInt32(info.Isck_03).ToString(),
					NConvert.ToInt32(info.Isck_04).ToString(),
					NConvert.ToInt32(info.Isck_05).ToString(),
					NConvert.ToInt32(info.Isck_06).ToString(),
					NConvert.ToInt32(info.Isck_07).ToString(),
					NConvert.ToInt32(info.Isck_08).ToString(),
					NConvert.ToInt32(info.Isck_09).ToString(),
					NConvert.ToInt32(info.Isck_10).ToString(),
					NConvert.ToInt32(info.Isck_11).ToString(),
					NConvert.ToInt32(info.Isck_12).ToString(),
					info.Mo_oper,
					info.Mo_date.ToString(),
					info.Oper_code,
					info.Oper_date.ToString(),
					info.Mark,
					info.Mark1,
					info.Mark2,
					info.Mark3,
					info.Mark4
				});
				num = base.ExecNoQuery(text);
			}
			catch (Exception ex)
			{
				base.Err = ex.Message;
				this.WriteErr();
				result = -1;
				return result;
			}
			result = num;
			return result;
		}

		public int InsertCDNotice(CDNoticeModel info)
		{
            string text = @"insert into MET_ORD_CHRONICDISEASE
  (clinic_no,
   p_patientno,
   p_name,
   sex_code,
   idenno,
   p_allergy,
   linkma_name,
   rela_name,
   linkman_tel,
   isck_01,
   isck_02,
   isck_03,
   isck_04,
   isck_05,
   isck_06,
   isck_07,
   isck_08,
   isck_09,
   isck_10,
   isck_11,
   isck_12,
   mo_oper,
   mo_date,
   oper_code,
   oper_date,
   mark,
   mark1,
   mark2,
   mark3,
   mark4)
values
  ('{0}',
   '{1}',
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
   to_date('{22}', 'yyyy-mm-dd HH24:mi:ss'),
   '{23}',
   to_date('{24}', 'yyyy-mm-dd HH24:mi:ss'),
   '{25}',
   '{26}',
   '{27}',
   '{28}',
   '{29}')
";
			int num = 0;
			int result;
			try
			{
				text = string.Format(text, new string[]
				{
					info.Clinic_no,
					info.P_patientno,
					info.P_name,
					info.Sex_code,
					info.Idenno,
					info.P_allergy,
					info.Linkma_name,
					info.Rela_name,
					info.Linkman_tel,
					NConvert.ToInt32(info.Isck_01).ToString(),
					NConvert.ToInt32(info.Isck_02).ToString(),
					NConvert.ToInt32(info.Isck_03).ToString(),
					NConvert.ToInt32(info.Isck_04).ToString(),
					NConvert.ToInt32(info.Isck_05).ToString(),
					NConvert.ToInt32(info.Isck_06).ToString(),
					NConvert.ToInt32(info.Isck_07).ToString(),
					NConvert.ToInt32(info.Isck_08).ToString(),
					NConvert.ToInt32(info.Isck_09).ToString(),
					NConvert.ToInt32(info.Isck_10).ToString(),
					NConvert.ToInt32(info.Isck_11).ToString(),
					NConvert.ToInt32(info.Isck_12).ToString(),
					info.Mo_oper,
					info.Mo_date.ToString(),
					info.Oper_code,
					info.Oper_date.ToString(),
					info.Mark,
					info.Mark1,
					info.Mark2,
					info.Mark3,
					info.Mark4
				});
				num = base.ExecNoQuery(text);
			}
			catch (Exception ex)
			{
				base.Err = ex.Message;
				this.WriteErr();
				result = -1;
				return result;
			}
			result = num;
			return result;
		}

		public bool CdIsExists(string inpatientNo)
		{
			bool result;
			try
			{
				string text = @"select count(*)
  FROM MET_ORD_CHRONICDISEASE a
 where a.P_PATIENTNO = '{0}'
   and a.mo_date > sysdate - 365";
				text = string.Format(text, inpatientNo);
				int num = int.Parse(base.ExecSqlReturnOne(text));
				if (num > 0)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				base.Err = ex.Message;
				result = false;
			}
			return result;
		}
	}
}
