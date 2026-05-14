using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Function;
using System.Data;
using System.Reflection;
using Neusoft.HISFC.Models.RealTableNameModels;
using System.Collections;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Fee.Item;

namespace ZDWY.SpecialRule.Price.DB
{
    /// <summary>
    /// CT/MR/限制收费历史规则的数据访问层。
    /// </summary>
    /// <remarks>
    /// 该类直接连接旧 HIS 数据库与 SQL 模板，负责读取项目目录、组套明细、
    /// 历史已收费次数、合同单位价格、限制收费字典等规则数据。
    /// 上层规则类大量依赖它返回的“宽松模型”，因此这里不仅是 Repository，
    /// 也是旧系统收费口径向内存对象转换的适配层。
    /// </remarks>
    public class CTMRFeeRuleDB : Neusoft.FrameWork.Management.Database
    {
        #region HIS底层公用模块(量大慎用 配合实体生成工具使用)
        /// <summary>
        /// 执行 SQL 并返回首条映射结果。
        /// </summary>
        /// <typeparam name="T">目标实体类型。</typeparam>
        /// <param name="sql">已经格式化完成的 SQL 语句。</param>
        /// <returns>首条记录对应的实体；无结果时返回 <typeparamref name="T"/> 的新实例。</returns>
        /// <remarks>
        /// 旧系统大量查询都默认“有就取第一条，没有就给空对象”，
        /// 因此这里刻意不返回 <c>null</c>，以兼容上层既有调用方式。
        /// </remarks>
        private T GetModelTForSql<T>(string sql) where T : new()
        {
            List<T> list = GetListTForSql<T>(sql);
            if (list == null || list.Count <= 0)
                return new T();
            return list[0];
        }

        /// <summary>
        /// 执行 SQL 并把结果集映射为实体列表。
        /// </summary>
        /// <typeparam name="T">目标实体类型。</typeparam>
        /// <param name="sql">已经格式化完成的 SQL 语句。</param>
        /// <returns>映射后的实体列表；查询失败时返回空集合或 <c>null</c>。</returns>
        /// <remarks>
        /// 这个工具方法主要服务于本文件中的若干轻量实体查询，避免每个查询都手写 Reader 解析。
        /// </remarks>
        private List<T> GetListTForSql<T>(string sql) where T : new()
        {
            try
            {
                DataSet ds = new DataSet();
                List<T> list = new List<T>();
                if (this.ExecQuery(sql, ref ds) == -1)
                    return null;
                var dt = ds.Tables[0];
                if (dt == null)
                {
                    return new List<T>();
                }
                if (dt.Rows.Count <= 0)
                {
                    return new List<T>();//default(List<T>);
                }
                return this.DataTableConvertToList<T>(dt);
            }
            catch (Exception ex)
            {
                this.Err = "查询数据失败：" + ex.Message;
                return new List<T>();//default(List<T>);
            }

        }

        /// <summary>
        /// 通过反射把 <see cref="DataTable"/> 按列名映射到目标实体列表。
        /// </summary>
        /// <typeparam name="T">目标实体类型。</typeparam>
        /// <param name="dt">待转换的数据表。</param>
        /// <returns>转换后的实体列表。</returns>
        /// <remarks>
        /// 这里的类型转换非常“宽松”：
        /// 只要列名能对上属性名，就尽量按照常见基础类型完成转换。
        /// 这是为了兼容旧 HIS 中字段类型并不完全规范、但查询又必须尽量成功的现实情况。
        /// </remarks>
        private List<T> DataTableConvertToList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            string tempName = string.Empty;
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    T t = new T();
                    var dr = dt.Rows[i];
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    foreach (var pi in propertys)
                    {
                        tempName = pi.Name;
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (dt.Columns[j].Caption.ToUpper() == tempName.ToUpper())
                            {
                                if (!pi.CanWrite) continue;
                                var value = dt.Rows[i][j];
                                if (value != DBNull.Value)
                                {
                                    //pi.SetValue(t, value, null);
                                    var Name = pi.PropertyType.Name;
                                    switch (Name.ToLower())
                                    {
                                        case "string":
                                            pi.SetValue(t, value, null);
                                            break;
                                        case "char":
                                            pi.SetValue(t, value, null);
                                            break;
                                        case "int":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "int32":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "unint":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "byte":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "sbyte":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "short":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "ushort":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "long":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "ulong":
                                            pi.SetValue(t, NConvert.ToInt32(value), null);
                                            break;
                                        case "float":
                                            pi.SetValue(t, NConvert.ToDecimal(value), null);
                                            break;
                                        case "decimal":
                                            pi.SetValue(t, NConvert.ToDecimal(value), null);
                                            break;
                                        case "double":
                                            pi.SetValue(t, Convert.ToDouble(value), null);
                                            break;
                                        case "bool":
                                            pi.SetValue(t, NConvert.ToBoolean(value), null);
                                            break;
                                        case "datetime":
                                            pi.SetValue(t, NConvert.ToDateTime(value), null);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    list.Add(t);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return new List<T>();//default(List<T>);

            }
            finally
            {
                dt.Clear();
                dt.Dispose();
            }

        }
        #endregion

        public ArrayList QueryUndrugZTBypackageCode(string packageCode)
        {
            ArrayList List = null;
            string strSql = "";
            if (this.Sql.GetSql("Fee.undrugzt.GetUndrugztinfo.1", ref strSql) == -1) return null;
            try
            {
                if (packageCode != "")
                {
                    List = new ArrayList();

                    strSql = string.Format(strSql, packageCode);
                    this.ExecQuery(strSql);
                    Neusoft.HISFC.Models.Fee.Item.UndrugComb info = null;
                    while (this.Reader.Read())
                    {
                        info = new Neusoft.HISFC.Models.Fee.Item.UndrugComb();

                        info.Package.ID = Reader[0].ToString(); //组套编码
                        info.Name = Reader[1].ToString();//非药品名称
                        info.ID = Reader[2].ToString();  //非药品编码
                        if (Reader[3] != DBNull.Value)
                        {
                            info.SortID = Convert.ToInt32(Reader[3]); //顺序号
                        }
                        else
                        {
                            info.SortID = 0;
                        }
                        info.SpellCode = Reader[4].ToString();  //取拼音码
                        info.WBCode = Reader[5].ToString();    //取五笔码
                        info.UserCode = Reader[6].ToString(); //输入码
                        info.User01 = Reader[7].ToString(); //标志
                        info.User02 = Reader[8].ToString(); // 是否特殊医疗项目 0 否 1 是
                        info.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[9].ToString()); //数量
                        List.Add(info);
                        info = null;
                    }
                    this.Reader.Close();
                }
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                List = null;
            }
            return List;
        }

        public List<COM_DICTIONARY> GetDictionaryListForType(string type)
        {
            string sql = @"select p.code,p.name,p.mark,p.sort_id,p.spell_code,p.wb_code,p.input_code,p.valid_state from com_dictionary p where p.type='{0}' ";
            sql = string.Format(sql, type);
            var dictList = this.GetListTForSql<COM_DICTIONARY>(sql);
            return dictList;
        }

        #region 按门诊号查询一条挂号信息
        /// <summary>
        /// 按门诊流水号查询挂号信息
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Register GetByClinic(string clinicNo)
        {
            ArrayList al = this.QueryRegListBase("Registration.Register.Query.4", clinicNo);

            if (al == null)
            {
                return null;
            }
            else if (al.Count == 0)
            {
                return new Neusoft.HISFC.Models.Registration.Register();
            }
            else
            {
                return (Neusoft.HISFC.Models.Registration.Register)al[0];
            }
        }
        private ArrayList QueryRegListBase(string whereSQLIndex, params string[] args)
        {
            string where = "";

            if (this.Sql.GetCommonSql(whereSQLIndex, ref where) == -1)
            {
                return null;
            }

            try
            {
                where = string.Format(where, args);
            }
            catch (Exception e)
            {
                this.Err = "[" + whereSQLIndex + "]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return QueryRegListBase(where);
        }
        public ArrayList QueryRegListBase(string whereSQL)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1)
            {
                return null;
            }

            sql = sql + "\r\n" + whereSQL;

            return this.QueryRegister(sql);
        }
        public ArrayList QueryRegister(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;

            ArrayList al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    var reg = new Neusoft.HISFC.Models.Registration.Register();
                    reg.ID = this.Reader[0].ToString();//序号
                    reg.PID.CardNO = this.Reader[1].ToString();//病历号
                    reg.DoctorInfo.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[2].ToString());//挂号日期
                    reg.DoctorInfo.Templet.Noon.ID = this.Reader[3].ToString();
                    reg.Name = this.Reader[4].ToString();
                    reg.IDCard = this.Reader[5].ToString();
                    reg.Sex.ID = this.Reader[6].ToString();

                    reg.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[7].ToString());//出生日期

                    reg.Pact.PayKind.ID = this.Reader[8].ToString();//结算类别
                    reg.Pact.PayKind.Name = this.Reader[9].ToString();

                    reg.Pact.ID = this.Reader[10].ToString();//合同单位
                    reg.Pact.Name = this.Reader[11].ToString();
                    reg.SSN = this.Reader[12].ToString();
                    reg.SIMainInfo.RegNo = reg.SSN;

                    reg.DoctorInfo.Templet.RegLevel.ID = this.Reader[13].ToString();//挂号级别
                    reg.DoctorInfo.Templet.RegLevel.Name = this.Reader[14].ToString();

                    reg.DoctorInfo.Templet.Dept.ID = this.Reader[15].ToString();//挂号科室
                    reg.DoctorInfo.Templet.Dept.Name = this.Reader[16].ToString();

                    reg.DoctorInfo.SeeNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[17].ToString());

                    reg.DoctorInfo.Templet.Doct.ID = this.Reader[18].ToString();//看诊医生
                    reg.DoctorInfo.Templet.Doct.Name = this.Reader[19].ToString();

                    reg.RegType = (Neusoft.HISFC.Models.Base.EnumRegType)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[20].ToString());
                    reg.IsFirst = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[21].ToString());

                    reg.RegLvlFee.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[22].ToString());
                    reg.RegLvlFee.ChkFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[23].ToString());
                    reg.RegLvlFee.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[24].ToString());
                    reg.RegLvlFee.OthFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[25].ToString());

                    reg.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString());
                    reg.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString());
                    reg.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[28].ToString());

                    reg.Status = (Neusoft.HISFC.Models.Base.EnumRegisterStatus)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29].ToString());

                    reg.InputOper.ID = this.Reader[30].ToString();
                    reg.IsSee = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[31].ToString());
                    reg.InputOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[32].ToString());
                    reg.TranType = (Neusoft.HISFC.Models.Base.TransTypes)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[33].ToString());
                    reg.BalanceOperStat.IsCheck = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[34]);//日结
                    reg.BalanceOperStat.CheckNO = this.Reader[35].ToString();
                    reg.BalanceOperStat.Oper.ID = this.Reader[36].ToString();

                    if (!this.Reader.IsDBNull(37))
                        reg.BalanceOperStat.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[37].ToString());

                    reg.PhoneHome = this.Reader[38].ToString();//联系电话
                    reg.AddressHome = this.Reader[39].ToString();//地址
                    reg.IsFee = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[40].ToString());
                    //作废人信息
                    reg.CancelOper.ID = this.Reader[41].ToString();
                    reg.CancelOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42].ToString());
                    reg.CardType.ID = this.Reader[43].ToString();//证件类型
                    reg.DoctorInfo.Templet.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[44].ToString());
                    reg.DoctorInfo.Templet.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[45].ToString());
                    //reg.InvoiceNo = this.Reader[50].ToString() ;
                    //reg.InvoiceNO = this.Reader[51].ToString() ; by niuxinyuan
                    reg.InvoiceNO = this.Reader[50].ToString();
                    reg.RecipeNO = this.Reader[51].ToString();

                    reg.DoctorInfo.Templet.IsAppend = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[52].ToString());
                    reg.OrderNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[53].ToString());
                    reg.DoctorInfo.Templet.ID = this.Reader[54].ToString();
                    reg.InSource.ID = this.Reader[55].ToString();
                    reg.PVisit.InState.ID = this.Reader[56].ToString();
                    reg.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[57].ToString());
                    reg.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[58].ToString());
                    reg.PVisit.ZG.ID = this.Reader[59].ToString();
                    reg.PVisit.PatientLocation.Bed.ID = this.Reader[60].ToString();

                    //{6FC43DF1-86E1-4720-BA3F-356C25C74F16}
                    //标识是否是账户流程挂号 1代表是
                    reg.IsAccount = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[61].ToString());

                    //{E26C3EE9-D480-421e-9FD3-7094D8E4E1D0}
                    reg.SeeDoct.Dept.ID = this.Reader[62].ToString(); //看诊科室
                    reg.SeeDoct.ID = this.Reader[63].ToString();//看诊医生
                    //{156C449B-60A9-4536-B4FB-D00BC6F476A1}
                    reg.DoctorInfo.Templet.RegLevel.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[64].ToString());
                    //{921FBFCA-3D0D-4bc6-8EEA-A9BBE152E69A}
                    reg.Mark1 = this.Reader[65].ToString();
                    // reg.PID.CaseNO =this.q;

                    // {531B6C65-1DF5-4f16-94EC-F7D87287966F}
                    reg.SeeDoct.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46].ToString());
                    //患者是否已经分诊
                    reg.IsTriage = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[47].ToString());
                    //{4AC12996-BC4B-4272-9FA4-E06DB8326330}
                    if (this.Reader.FieldCount >= 67)
                    {
                        reg.NormalName = this.Reader[66].ToString();

                    }
                    if (this.Reader.FieldCount > 67)
                    {
                        reg.Card.ID = this.Reader[67].ToString();
                        reg.Card.CardType.ID = this.Reader[68].ToString();
                        reg.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[69].ToString());
                    }
                    if (this.Reader.FieldCount > 70)
                    {
                        reg.Temperature = this.Reader[70].ToString();
                    }
                    if (Reader.FieldCount > 71)
                    {
                        reg.PatientType = Reader[71].ToString();
                    }
                    reg.RegExtend = new Neusoft.HISFC.Models.Registration.RegisterExtend();
                    if (Reader.FieldCount > 72)
                    {
                        reg.RegExtend.DiagFeeRegCode = Reader[72].ToString();
                    }
                    if (Reader.FieldCount > 73)
                    {
                        reg.RegExtend.DiagFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[73].ToString());
                    }
                    if (Reader.FieldCount > 74)
                    {
                        reg.RegExtend.DiagItemCode = Reader[74].ToString();
                    }
                    if (Reader.FieldCount > 75)
                    {
                        reg.Greenway = Reader[75].ToString();
                    }
                    al.Add(reg);
                }
                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "检索挂号信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return al;
        }
        #endregion

        /// <summary>
        /// 获得医嘱流水号
        /// </summary>
        /// <returns></returns>
        public string GetNewOrderID()
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Management.Order.GetNewOrderID", ref sql) == -1) return null;
            string strReturn = this.ExecSqlReturnOne(sql);
            if (strReturn == "-1" || strReturn == "") return null;
            return strReturn;
        }

        public int QueryItemList(string deptCode, Neusoft.HISFC.Models.Base.ItemKind itemKind, ref DataSet ds)
        {
            if (itemKind == ItemKind.All)
            {
                return this.ExecQuery("Fee.Item.GetOutPatientItemList.Select", ref ds, deptCode);
            }
            if (itemKind == ItemKind.Undrug)
            {
                return this.ExecQuery("Fee.Item.GetOutPatientItemList.Select.Undrug", ref ds, deptCode);
            }
            if (itemKind == ItemKind.Pharmacy)
            {
                return this.ExecQuery("Fee.Item.GetOutPatientItemList.Select.Pharmacy", ref ds, deptCode);
            }
            return 1;
        }

        /// <summary>
        /// 非药品组套明细表 
        /// </summary>
        /// <param name="packageCode"></param>
        /// <returns></returns>
        public ArrayList QueryUndrugPackagesBypackageCode(string packageCode)
        {
            ArrayList List = null;
            string strSql = "";
            if (this.Sql.GetCommonSql("Fee.undrugzt.GetUndrugztinfo", ref strSql) == -1) return null;
            try
            {
                if (packageCode != "")
                {
                    List = new ArrayList();

                    strSql = string.Format(strSql, packageCode);
                    this.ExecQuery(strSql);
                    Neusoft.HISFC.Models.Fee.Item.UndrugComb info = null;
                    while (this.Reader.Read())
                    {
                        info = new Neusoft.HISFC.Models.Fee.Item.UndrugComb();

                        info.Package.ID = Reader[0].ToString(); //组套编码
                        info.Name = Reader[1].ToString();//非药品名称
                        info.ID = Reader[2].ToString();  //非药品编码
                        if (Reader[3] != DBNull.Value)
                        {
                            info.SortID = Convert.ToInt32(Reader[3]); //顺序号
                        }
                        else
                        {
                            info.SortID = 0;
                        }
                        info.SpellCode = Reader[4].ToString();  //取拼音码
                        info.WBCode = Reader[5].ToString();    //取五笔码
                        info.UserCode = Reader[6].ToString(); //输入码
                        info.User01 = Reader[7].ToString(); //标志
                        info.User02 = Reader[8].ToString(); // 是否特殊医疗项目 0 否 1 是
                        info.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[9].ToString()); //数量
                        List.Add(info);
                        info = null;
                    }
                    this.Reader.Close();
                }
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                List = null;
            }
            return List;
        }

        /// <summary>
        /// 根据合同代码查询
        /// </summary>
        /// <param name="pactCode">合同单位代码</param>
        /// <returns>成功 合同单位实体 失败 Null</returns>
        public Neusoft.HISFC.Models.Base.PactInfo GetPactUnitInfoByPactCode(string pactCode)
        {
            string sql = string.Empty;

            if (this.Sql.GetCommonSql("Fee.PactUnitInfo.GetPactUnitInfoByPactCode", ref sql) == -1)
            {
                this.Err = "没有找到索引为:Fee.PactUnitInfo.GetPactUnitInfoByPactCode得SQL语句";

                return null;
            }

            ArrayList temp = this.QueryPactUnitBySql(sql, pactCode);

            if (temp == null || temp.Count == 0)
            {
                return null;
            }

            return temp[0] as Neusoft.HISFC.Models.Base.PactInfo;
        }

        /// <summary>
        /// 根据SQL语句查询合同单位信息
        /// </summary>
        /// <param name="sql">查询得SQL语句</param>
        /// <param name="args">参数</param>
        /// <returns>成功 合同单位信息集合 失败 null</returns>
        private ArrayList QueryPactUnitBySql(string sql, params string[] args)
        {
            if (this.ExecQuery(sql, args) == -1)
            {
                return null;
            }

            ArrayList pactUnitList = new ArrayList();//费用明细数组
            Neusoft.HISFC.Models.Base.PactInfo pactUnit = null;

            try
            {
                while (this.Reader.Read())
                {
                    pactUnit = new Neusoft.HISFC.Models.Base.PactInfo();

                    pactUnit.ID = this.Reader[0].ToString();//合同代码          
                    pactUnit.Name = this.Reader[1].ToString();//合同单位名称                    
                    pactUnit.PayKind.ID = this.Reader[2].ToString();//结算类别                    
                    pactUnit.Rate.PubRate = NConvert.ToDecimal(Reader[3].ToString().Trim());//公费比例                    
                    pactUnit.Rate.PayRate = NConvert.ToDecimal(Reader[4].ToString().Trim());//自付比例                   
                    pactUnit.Rate.OwnRate = NConvert.ToDecimal(Reader[5].ToString().Trim()); //自费比例                   
                    pactUnit.Rate.RebateRate = NConvert.ToDecimal(Reader[6].ToString().Trim()); //优惠比例                    
                    pactUnit.Rate.ArrearageRate = NConvert.ToDecimal(Reader[7].ToString().Trim());//欠费比例                    
                    pactUnit.Rate.IsBabyShared = NConvert.ToBoolean(Reader[8].ToString());//婴儿标志 0 无关 1 有关                                
                    pactUnit.IsNeedMCard = NConvert.ToBoolean(Reader[9].ToString().Trim()); //是否要求必须有医疗证号 0 否 1 是                      
                    pactUnit.IsInControl = NConvert.ToBoolean(Reader[10].ToString().Trim());//是否受监控 1受监控0不受监控                   
                    pactUnit.ItemType = Reader[11].ToString().Trim(); //标志  0 全部 1 药品 2 非药品   
                    pactUnit.DayQuota = NConvert.ToDecimal(Reader[12].ToString().Trim());//日限额                     
                    pactUnit.MonthQuota = NConvert.ToDecimal(Reader[13].ToString().Trim()); //月限额                    
                    pactUnit.YearQuota = NConvert.ToDecimal(Reader[14].ToString().Trim());//年限额
                    pactUnit.OnceQuota = NConvert.ToDecimal(Reader[15].ToString().Trim());//一次限
                    string PriceForm = Reader[16].ToString();
                    if (PriceForm == "0")
                    {
                        pactUnit.PriceForm = "默认价";
                    }
                    else if (PriceForm == "1")
                    {
                        pactUnit.PriceForm = "特诊价";
                    }
                    else if (PriceForm == "2")
                    {
                        pactUnit.PriceForm = "儿童价";
                    }
                    //{B9303CFE-755D-4585-B5EE-8C1901F79450}maokb增加购入价
                    else if (PriceForm == "3")
                    {
                        pactUnit.PriceForm = "购入价";
                    }
                    else if (PriceForm == "4")
                    {
                        pactUnit.PriceForm = "MDT价";
                    }
                    else if (PriceForm == "5")
                    {
                        pactUnit.PriceForm = "围产中心价";//20190821 增加{6E420B5F-A956-4caa-8B18-B8FBBCBD8002}
                    }
                    else
                    {
                        pactUnit.PriceForm = "默认价";
                    }

                    pactUnit.BedQuota = NConvert.ToDecimal(Reader[17].ToString());//床位限额
                    pactUnit.AirConditionQuota = NConvert.ToDecimal(Reader[18].ToString());//空调限额
                    pactUnit.SortID = NConvert.ToInt32(Reader[19]);//序号             
                    pactUnit.ShortName = Reader[20].ToString();//合同单位简称
                    pactUnit.PactDllName = Reader[21].ToString(); //待遇dll名称
                    pactUnit.PactDllDescription = Reader[22].ToString();//待遇dll说明
                    if (Reader.FieldCount > 24)
                        pactUnit.SpellCode = Reader[24].ToString();//拼音码
                    if (Reader.FieldCount > 25)
                        pactUnit.WBCode = Reader[25].ToString();//五笔码
                    if (Reader.FieldCount > 26)
                        pactUnit.PatientType.ID = Reader[26].ToString();//人员类型编码
                    if (Reader.FieldCount > 27)
                        pactUnit.PatientType.Name = Reader[27].ToString();//人员类型名称
                    if (Reader.FieldCount > 28)
                        pactUnit.IsUseInOutPatientFee = NConvert.ToBoolean(Reader[28].ToString().Trim());
                    if (Reader.FieldCount > 23)
                    {
                        pactUnit.PactSystemType = Reader[23].ToString().Trim();

                        switch (pactUnit.PactSystemType)
                        {
                            case "1":
                                pactUnit.PactSystemType = "门诊";
                                break;
                            case "2":
                                pactUnit.PactSystemType = "住院";
                                break;
                            case "3":
                                pactUnit.PactSystemType = "系统";
                                break;
                            default:
                                pactUnit.PactSystemType = "全院";
                                break;
                        }
                    }
                    if (Reader.FieldCount > 29)
                    {
                        pactUnit.HosCode = Reader[29].ToString().Trim();
                    }
                    pactUnitList.Add(pactUnit);

                }

                this.Reader.Close();

                return pactUnitList;
            }
            catch (Exception e)
            {
                this.Err = e.Message;

                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }

                return null;
            }
        }

        /// <summary>
        /// 获得常数所有列
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ArrayList GetAllList(string type)
        {
            string strSql = "";

            ArrayList al = new ArrayList();
            //接口说明
            //返回
            if (this.Sql.GetSql("Manager.Constant.1", ref strSql) == -1) return null;

            try
            {
                strSql = string.Format(strSql, type);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = "接口错误！" + ex.Message;
                return null;
            }


            if (this.ExecQuery(strSql) == -1) return null;
            Neusoft.HISFC.Models.Base.Const cons;
            while (this.Reader.Read())
            {
                cons = new Const();

                cons.ID = this.Reader[1].ToString();
                cons.Name = this.Reader[2].ToString();
                cons.Memo = this.Reader[3].ToString();
                cons.SpellCode = this.Reader[4].ToString();
                cons.WBCode = this.Reader[5].ToString();
                cons.UserCode = this.Reader[6].ToString();
                if (!Reader.IsDBNull(7))
                    cons.SortID = Convert.ToInt32(this.Reader[7]);
                cons.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[8].ToString());
                cons.OperEnvironment.ID = this.Reader[9].ToString();
                if (!Reader.IsDBNull(10))
                    cons.OperEnvironment.OperTime = Convert.ToDateTime(this.Reader[10].ToString());
                al.Add(cons);
            }
            this.Reader.Close();
            return al;
        }

        /// <summary>
        /// 根据员工编号获取相关员工信息
        /// </summary>
        /// <param name="emplCode"></param>
        /// <returns></returns>
        public COM_EMPLOYEE GetEmployeeInfoForEmplCode(string emplCode)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Person.SelectByID", ref sql) == -1)
            {
                this.Err = "未找到索引Person.SelectByID";
                return null;
            }
            sql = string.Format(sql, emplCode);
            return this.GetModelTForSql<COM_EMPLOYEE>(sql);

        }

        public Neusoft.HISFC.Models.Pharmacy.Item GetPharmacyInfoForCode(string itemCode)
        {
            string sql = string.Format(@"  SELECT drug_code,          --药品编码
             trade_name,         --商品名称
             spell_code,         --商品名拼音码
             wb_code,            --商品名五笔码
             custom_code,        --商品名自定义码              
             pack_qty,           --包装数              
             specs,              --规格
             class_code,         --系统类别
             fee_code,           --最小费用代码              
             regular_name,       --通用名              
             regular_spell,      --通用名拼音码
             regular_wb,         --通用名五笔码
             formal_name,        --学名
             formal_spell,       --学名拼音码  
             formal_wb,          --学名五笔码  
             other_name,         --别名
             english_name,       --英文名              
             english_regular,    --英文通用名  
             english_other,      --英文别名
             international_code, --国际编码
             gb_code,            --国家编码
             pack_unit,          --包装单位
             min_unit,           --最小单位              
             base_dose,          --基本剂量
             dose_unit,          --剂量单位
             dose_model_code,    --剂型编码
             drug_type,          --药品类别
             drug_quality,       --药品性质 1-麻药 , 2-保管药 , 3-普药 , 4-输液
             retail_price,       --参考零售价
             wholesale_price,    --参考批发价
             purchase_price,     --最新够入价
             top_retailprice,    --最高零售价
             phy_function1,      --一级药理作用  
             store_condition,    --储藏条件
             usage_code,         --用法编码
             once_dose,          --一次用量              
             frequency_code,     --频次编码
             producer_code,      --生产厂家
             approve_info,       --批文信息
             label,              --商标
             price_form,         --价格形式
             valid_state,        --有效性标志 0 在用 1 停用 2 废弃
             self_flag,          --自制标志 0-非自产，1-自产
             gmp_flag,           --GMP标志
             oct_flag,           --OCT标志 0非处方药 1处方药  
             new_flag,           --新药标记，用户维护1－新药，0－非新药
             lack_flag,          --缺药标志 0-否，1-是              
             show_flag,          --大屏幕显示标记0非屏幕显示 1为大屏幕显示
             append_flag,        --附材标志 0-否，1-是              
             caution,            --注意事项
             item_grade,         --项目等级，1甲类，2乙类，3丙类
             bar_code,           --条形码              
             producing_area,     --产地
             company_code,       --最新供药公司(在入库时更新，用于生成药品采购单)
             ingredient,         --有效成分
             execute_standard,   --执行标准
             brief_introduction, --药品简介            
              manual,             --说明书内容               
              phy_function2,     --二级药理作用
             phy_function3,     --三级药理作用
             tender_flag,        --招标标志 0非招标1招标
             tender_price,       --招标价              
             contract_code,      --招标采购合同编号
             tender_begindate,   --中标开始日期              
             tender_enddate,     --中标结束日期
             tender_company,     --中标公司
             mark,               --备注
             oper_code,          --操作员              
             oper_date,          --操作时间
             other_spell,        --别名拼音码  
             other_wb,           --别名五笔码              
             other_custom,       --别名自定义码
             regular_custom,     --通用名自定义码  
             formal_custom,      --学名自定义码
             test_flag,          --是否需要试敏
             special_flag,       --省限制  
             special_flag1,      --市限制  
             special_flag2,      --自费项目   
             special_flag3,      --特殊标记   
             special_flag4,      --特殊标记  
             OLD_DRUG_CODE,      --80旧系统药品编码
             SHIFT_TYPE,     --81变动类型(0特殊修改,1新药,2停用,3调价)  
             SHIFT_DATE,     --82变动时间 
             SHIFT_MARK,     --83变动原因 
             SPLIT_TYPE,       --84拆分类型:0可拆包装单位,1不能拆包装单位
             NOSTRUM_FLAG,    --85协定处方：0不是  1是  
             EXTEND1,   ---86
             EXTEND2, ------87
             CREATE_TIME, ---88
             retail_price2,--89
             ext_num1,--90
             ext_num2,--91
             extend3,--92
             extend4, --93
             cdsplit_type,--94
             lzsplit_type, --95
        SECOND_BASE_DOSE, --96
       SECOND_DOSE_UNIT, --97
       ONCE_DOSE_UNIT,    --98
       PRODUCTID,          --99
       BIGPACKUNIT, -- 100
       BIGPACKQTY ,-- 101
       CONTROLSDRUG ,--102
       CONTROLSLEVEL, --103
       '', --104
       (select CENTER_CODE from fin_com_compare where PACT_CODE＝'16' and HIS_CODE = drug_code ) as CENTER_CODE--105
             FROM pha_com_baseinfo   where pha_com_baseinfo.drug_code='{0}'
 ", itemCode);
            Neusoft.HISFC.Models.Pharmacy.Item Item = new Neusoft.HISFC.Models.Pharmacy.Item(); //返回数组中的药品信息类

            try
            {
                this.ExecQuery(sql);

                while (this.Reader.Read())
                {
                    Item = new Neusoft.HISFC.Models.Pharmacy.Item();
                    #region "接口说明"
                    //  0 药品编码        1 商品名称        2 拼音码          3 五笔码          4 自定义码 
                    //  5 包装数量        6 规格            7 系统类别编码    8 最小费用代码    9 药品通用名     
                    // 10 通用名拼音码   11 通用名五笔码   12 学名           13 学名拼音码     14 学名五笔码     
                    // 15 别名           16 英文商品名     17 英文通用名     18 英文别名       19 国际编码
                    // 20 国家编码       21 包装单位       22 最小单位       23 基本剂量       24 剂量单位       
                    // 25 剂型编码       26 药品类别编码   27 药品性质编码   28 零售价	        29 批发价         
                    // 30 购入价         31 最高零售价     32 药理作用(一级) 33 储藏条件       34 使用方法       
                    // 35 一次用量       36 频次		    37 生产厂家编码   38 批准文号       39 注册商标       
                    // 40 价格形式编码   41 是否停用       42 是否自制       43 是否GMP        44 是否OTC（处方药） 
                    // 45 是否新药       46 是否缺药       47 是否大屏幕显示 48 是否附材       49 注意事项       
                    // 50 药品等级       51 条形码         52 产地           53 最新供货公司   54 有效成份       
                    // 55 中药执行标准   56 药品简介       57 药品说明书内容 58 二级药理作用   59 三级药理作用   
                    // 60 是否是招标用药 61 中标价         62 采购合同编号   63 采购开始周期   64 采购结束周期   
                    // 65 采购单位编码   66 备注           67 操作员代码     68 操作时间       69 别名拼音码     
                    // 70 别名五笔码     71 别名自定义码   72 通用名自定义码 73 学名自定义码   74 是否需要试敏
                    // 75 省限制         76市限制          77自费项目        78特殊标记        79 特殊标记
                    // 80 旧系统药品编码 81数据变动类型	   82数据变动时间	 83数据变动原因    84 门诊拆分属性
                    // 85 协定处方标志   94住院长嘱拆分    95住院临嘱拆分
                    #endregion
                    Item.ID = this.Reader[0].ToString();
                    Item.Name = this.Reader[1].ToString();
                    Item.SpellCode = this.Reader[2].ToString();
                    Item.WBCode = this.Reader[3].ToString();
                    Item.UserCode = this.Reader[4].ToString();
                    Item.PackQty = NConvert.ToDecimal(this.Reader[5].ToString());
                    Item.Specs = this.Reader[6].ToString();
                    Item.SysClass.ID = this.Reader[7].ToString();
                    Item.MinFee.ID = this.Reader[8].ToString();
                    Item.NameCollection.RegularName = this.Reader[9].ToString();
                    Item.NameCollection.RegularSpell.SpellCode = this.Reader[10].ToString();
                    Item.NameCollection.RegularSpell.WBCode = this.Reader[11].ToString();
                    Item.NameCollection.FormalName = this.Reader[12].ToString();
                    Item.NameCollection.FormalSpell.SpellCode = this.Reader[13].ToString();
                    Item.NameCollection.FormalSpell.WBCode = this.Reader[14].ToString();
                    Item.NameCollection.OtherName = this.Reader[15].ToString();
                    Item.NameCollection.EnglishName = this.Reader[16].ToString();
                    Item.NameCollection.EnglishRegularName = this.Reader[17].ToString();
                    Item.NameCollection.EnglishOtherName = this.Reader[18].ToString();
                    Item.NameCollection.InternationalCode = this.Reader[19].ToString();
                    Item.NameCollection.GbCode = this.Reader[20].ToString();
                    Item.PackUnit = this.Reader[21].ToString();
                    Item.MinUnit = this.Reader[22].ToString();
                    Item.BaseDose = NConvert.ToDecimal(this.Reader[23].ToString());
                    Item.DoseUnit = this.Reader[24].ToString();
                    Item.DosageForm.ID = this.Reader[25].ToString();
                    Item.Type.ID = this.Reader[26].ToString();
                    Item.Quality.ID = this.Reader[27].ToString();
                    Item.PriceCollection.RetailPrice = NConvert.ToDecimal(this.Reader[28].ToString());

                    Item.Price = Item.PriceCollection.RetailPrice;

                    Item.PriceCollection.WholeSalePrice = NConvert.ToDecimal(this.Reader[29].ToString());
                    Item.PriceCollection.PurchasePrice = NConvert.ToDecimal(this.Reader[30].ToString());
                    Item.PriceCollection.TopRetailPrice = NConvert.ToDecimal(this.Reader[31].ToString());
                    Item.PhyFunction1.ID = this.Reader[32].ToString();
                    Item.Product.StoreCondition = this.Reader[33].ToString();
                    Item.Usage.ID = this.Reader[34].ToString();
                    Item.OnceDose = NConvert.ToDecimal(this.Reader[35].ToString());
                    Item.Frequency.ID = this.Reader[36].ToString();
                    Item.Product.Producer.ID = this.Reader[37].ToString();
                    Item.Product.ApprovalInfo = this.Reader[38].ToString();
                    Item.Product.Label = this.Reader[39].ToString();
                    Item.PriceCollection.PriceForm.ID = this.Reader[40].ToString();

                    //有效性 1 有效 0 无效 2 废弃
                    Item.ValidState = (Neusoft.HISFC.Models.Base.EnumValidState)(NConvert.ToInt32(this.Reader[41]));

                    Item.IsValid = !Item.IsStop;


                    Item.Product.IsSelfMade = NConvert.ToBoolean(this.Reader[42].ToString());
                    Item.IsGMP = NConvert.ToBoolean(this.Reader[43].ToString());
                    Item.IsOTC = NConvert.ToBoolean(this.Reader[44].ToString());
                    Item.IsNew = NConvert.ToBoolean(this.Reader[45].ToString());
                    Item.IsLack = NConvert.ToBoolean(this.Reader[46].ToString());
                    Item.IsShow = true;//modified by zlw 2006-6-5
                    Item.IsShow = NConvert.ToBoolean(this.Reader[47].ToString());
                    Item.IsSubtbl = NConvert.ToBoolean(this.Reader[48].ToString());
                    Item.Product.Caution = this.Reader[49].ToString();
                    Item.Grade = this.Reader[50].ToString();
                    Item.Product.BarCode = this.Reader[51].ToString();
                    Item.Product.ProducingArea = this.Reader[52].ToString();
                    Item.Product.Company.ID = this.Reader[53].ToString();
                    Item.Ingredient = this.Reader[54].ToString();
                    Item.ExecuteStandard = this.Reader[55].ToString();
                    Item.Product.BriefIntroduction = this.Reader[56].ToString();
                    Item.Product.Manual = this.Reader[57].ToString();
                    Item.PhyFunction2.ID = this.Reader[58].ToString();
                    Item.PhyFunction3.ID = this.Reader[59].ToString();
                    Item.TenderOffer.IsTenderOffer = NConvert.ToBoolean(this.Reader[60].ToString());
                    Item.TenderOffer.Price = NConvert.ToDecimal(this.Reader[61].ToString());
                    Item.TenderOffer.ContractNO = this.Reader[62].ToString();
                    Item.TenderOffer.BeginTime = NConvert.ToDateTime(this.Reader[63].ToString());
                    Item.TenderOffer.EndTime = NConvert.ToDateTime(this.Reader[64].ToString());
                    Item.TenderOffer.Company.ID = this.Reader[65].ToString();
                    Item.Memo = this.Reader[66].ToString();
                    Item.Oper.ID = this.Reader[67].ToString();
                    Item.Oper.OperTime = NConvert.ToDateTime(this.Reader[68].ToString());
                    Item.NameCollection.OtherSpell.SpellCode = this.Reader[69].ToString();
                    Item.NameCollection.OtherSpell.WBCode = this.Reader[70].ToString();
                    Item.NameCollection.OtherSpell.UserCode = this.Reader[71].ToString();
                    Item.NameCollection.RegularSpell.UserCode = this.Reader[72].ToString();
                    Item.NameCollection.FormalSpell.UserCode = this.Reader[73].ToString();
                    Item.IsAllergy = NConvert.ToBoolean(this.Reader[74].ToString());
                    Item.SpecialFlag = this.Reader[75].ToString();		//75省限标记
                    Item.SpecialFlag1 = this.Reader[76].ToString();		//76市限标记
                    Item.SpecialFlag2 = this.Reader[77].ToString();		//77自费项目
                    Item.SpecialFlag3 = this.Reader[78].ToString();		//78特殊标记
                    Item.SpecialFlag4 = this.Reader[79].ToString();		//79特殊标记
                    Item.OldDrugID = this.Reader[80].ToString();		//80旧系统药品编码
                    Item.ShiftType.ID = this.Reader[81].ToString();		//81数据变动类型
                    Item.ShiftTime = NConvert.ToDateTime(this.Reader[82].ToString());//82数据变动日期
                    Item.ShiftMark = this.Reader[83].ToString();		//83数据变动原因
                    Item.SplitType = this.Reader[84].ToString();     //84拆分类型 0 可拆分 1 不可拆分
                    Item.ShowState = this.Reader[47].ToString();     //显示 属性 0全院 1 住院处 2 门诊
                    Item.IsNostrum = NConvert.ToBoolean(this.Reader[85].ToString()); //85协定处方标志
                    if (this.Reader.FieldCount > 86)
                    {
                        //扩展信息-备用
                        Item.ExtendData1 = this.Reader[86].ToString();
                    }
                    if (this.Reader.FieldCount > 87)
                    {
                        Item.ExtendData2 = this.Reader[87].ToString();
                    }
                    if (this.Reader.FieldCount > 88)
                    {  //字典生成时间
                        Item.CreateTime = NConvert.ToDateTime(this.Reader[88]);
                    }
                    if (this.Reader.FieldCount > 89)
                    {  //第二零售价
                        Item.RetailPrice2 = NConvert.ToDecimal(this.Reader[89]);
                    }
                    if (this.Reader.FieldCount > 90)
                    {
                        Item.ExtNumber1 = NConvert.ToDecimal(this.Reader[90]);
                    }
                    if (this.Reader.FieldCount > 91)
                    {
                        Item.ExtNumber2 = NConvert.ToDecimal(this.Reader[91]);
                    }
                    if (this.Reader.FieldCount > 92)
                    {
                        Item.ExtendData3 = this.Reader[92].ToString();
                    }
                    if (this.Reader.FieldCount > 93)
                    {
                        Item.ExtendData4 = this.Reader[93].ToString();
                    }
                    if (this.Reader.FieldCount > 94)
                    {
                        Item.CDSplitType = this.Reader[94].ToString();
                    }
                    if (this.Reader.FieldCount > 95)
                    {
                        Item.LZSplitType = this.Reader[95].ToString();
                    }
                    if (this.Reader.FieldCount > 96)
                    {
                        Item.SecondBaseDose = NConvert.ToDecimal(this.Reader[96]);
                    }
                    if (this.Reader.FieldCount > 97)
                    {
                        Item.SecondDoseUnit = this.Reader[97].ToString();
                    }
                    if (this.Reader.FieldCount > 98)
                    {
                        Item.OnceDoseUnit = this.Reader[98].ToString();
                    }
                    if (this.Reader.FieldCount > 99)
                    {
                        Item.ProductID = this.Reader[99].ToString();
                    }
                    if (this.Reader.FieldCount > 100)
                    {
                        Item.BigPackUnit = this.Reader[100].ToString();
                    }
                    if (this.Reader.FieldCount > 101)
                    {
                        Item.BigPackQty = this.Reader[101].ToString();
                    }
                    if (this.Reader.FieldCount > 102)
                    {
                        Item.ExtendData5 = this.Reader[102].ToString();
                    }
                    // {58B47A30-BB9D-4ed8-983B-5D067B0242E0}
                    if (this.Reader.FieldCount > 103)
                    {
                        Item.ExtendData6 = this.Reader[103].ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                this.Err = "获得药品基本信息时，执行SQL语句出错！" + ex.Message;
                this.ErrCode = "-1";
                return Item;
            }
            finally
            {
                this.Reader.Close();
            }
            return Item;
        }

        public decimal GetPrice(string itemCode, Neusoft.HISFC.Models.Registration.Register register, int age, decimal UnitPrice, decimal ChildPrice, decimal SPPrice, decimal PurchasePrice, ref decimal orgPrice, decimal rate)
        {
            decimal price = this.GetPrice(itemCode, register, UnitPrice, ChildPrice, SPPrice, PurchasePrice, ref orgPrice);

            return price * rate;
        }

        /// <summary>
        /// 按患者、项目和院区价格口径计算最终收费单价。
        /// </summary>
        /// <param name="itemCode">项目编码。</param>
        /// <param name="register">患者挂号信息。</param>
        /// <param name="UnitPrice">标准单价。</param>
        /// <param name="ChildPrice">儿童价。</param>
        /// <param name="SPPrice">特诊价或倍率参数。</param>
        /// <param name="PurchasePrice">购入价，供部分特殊口径使用。</param>
        /// <param name="orgPrice">返回原始基准价，供上层继续按比例换算。</param>
        /// <returns>按当前院区与患者条件判定后的最终单价。</returns>
        /// <remarks>
        /// 这里浓缩了旧 HIS 多年叠加出来的价格分支：
        /// 校区门诊、材料费、医疗服务项目、药品零差价、中成药例外、儿童价等，
        /// 最终都在这一处收敛，因此是整个折价链路的关键底座。
        /// </remarks>
        public decimal GetPrice(string itemCode, Neusoft.HISFC.Models.Registration.Register register, decimal UnitPrice, decimal ChildPrice, decimal SPPrice, decimal PurchasePrice, ref decimal orgPrice)
        {
            decimal num;
            Undrug undrug;
            string mdtsql = @"select t.item_code,t.unit_price,t.unit_price1,t.unit_price2,t.MDT_PRICE from fin_com_undruginfo t where t.item_code='{0}'";
            orgPrice = UnitPrice;
            string gbCode = "";//国标码
            string ISChildPrice = "";//是否需要收取儿童价
            if (Neusoft.FrameWork.Management.Connection.Hospital.ID != "CORE_HIS50")//校区门诊
            {
                // ========== 第一阶段：校区门诊走特殊价格口径 ==========
                // 老系统里校区门诊对非药品与药品采用不同定价策略，
                // 还要额外判断材料费、医疗服务项目和学校价格字段。
                #region MyRegion
                System.Collections.Hashtable hsFeeCode = GetFeeCodeHs("11");

                if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")//非药品 
                {
                    //this.getPrice(itemCode, ref UnitPrice, ref ChildPrice, ref SPPrice);
                    //旧的价格获取方式
                    //if (SPPrice == 0m || SPPrice.ToString() == "")//表的字段是UNIT_PRICE2特诊价
                    //{
                    //    return UnitPrice;
                    //}
                    //else
                    //{
                    //    return UnitPrice * SPPrice;
                    //}

                    //新的获取非药品项目价格方式
                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                    if (GetItemFeeCodeAndSchoolPrice(itemCode, ref obj) < 0 || hsFeeCode.Count < 0)
                    {
                        // 元数据取不到时，回退到旧逻辑，优先使用标准价 / 特诊价倍率，
                        // 这样至少不会因为辅助字段缺失导致整个收费失败。
                        if (SPPrice == 0m || SPPrice.ToString() == "")//表的字段是UNIT_PRICE2特诊价
                        {
                            return UnitPrice;
                        }
                        else
                        {
                            return UnitPrice * SPPrice;
                        }
                    }
                    if (hsFeeCode.Contains(obj.ID))//材料费
                    {
                        // 材料费沿用儿童价字段承载的平台价格，这是历史数据结构的兼容做法。
                        return ChildPrice;//儿童价，表数据是unit_price1，HERP通过平台把价格传过来
                    }
                    else//医疗服务项目
                    {
                        // 医疗服务项目优先取院区价格；缺失时才回退到标准价 * 特诊倍率。
                        if (string.IsNullOrEmpty(obj.Memo))
                        {
                            return UnitPrice * SPPrice;
                        }
                        return Neusoft.FrameWork.Function.NConvert.ToDecimal(obj.Memo);//校区价格school_price
                    }

                }
                else//药品
                {
                    // ========== 第二阶段：药品分支处理零差价与例外类型 ==========
                    Neusoft.HISFC.Models.Pharmacy.Item item = this.GetPharmacyInfoForCode(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    //zzf 2020-09-09 疫苗走零差价了
                    if (item.Type.ID.ToString() == "C")// || item.ExtendData2.ToString() == "Y")
                    {
                        if (item.PriceCollection.RetailPrice != 0)
                        {
                            return item.PriceCollection.RetailPrice;
                        }
                        else
                        {
                            return UnitPrice;
                        }
                    }
                    else
                    {
                        if (PurchasePrice != 0)
                        {
                            return PurchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return UnitPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    }
                }
                #endregion
            }
            if (register != null && register.Pact != null && register.Pact.PriceForm == "购入价")
            {
                if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")//药品
                {
                    #region 药品
                    Neusoft.HISFC.Models.Pharmacy.Item item = this.GetPharmacyInfoForCode(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    //zzf 2020-09-09 疫苗走零差价了
                    if (item.Type.ID.ToString() == "C")// || item.ExtendData2.ToString() == "Y")
                    {
                        if (item.PriceCollection.RetailPrice != 0)
                        {
                            return item.PriceCollection.RetailPrice;
                        }
                        else
                        {
                            return UnitPrice;
                        }
                    }
                    else
                    {
                        if (PurchasePrice != 0)
                        {
                            return PurchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return UnitPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    }
                    #endregion
                }
                else if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")
                {
                    #region 非药品
                    //6岁及以下儿童临床诊疗类项目加收30%
                    //undrug = Neusoft.SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemCode);
                    getPrice(itemCode, ref UnitPrice, ref ChildPrice, ref SPPrice, ref gbCode, ref ISChildPrice);
                    if ((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date))//6岁及以下儿童
                    {
                        if ((!string.IsNullOrEmpty(gbCode) && (gbCode.Substring(0, 1).ToString() == "3")) || ISChildPrice == "1")//国标码为3开头
                        {
                            decimal spPrice = 0;
                            if (this.GetChildSpPrice(itemCode, ref spPrice) == -1)//获取儿童价，临床诊疗类加收30%的价格
                            {
                                spPrice = UnitPrice;
                            }
                            if (spPrice <= 0)
                            {
                                spPrice = UnitPrice;
                            }
                            return spPrice;//儿童价
                        }
                    }
                    //if ((((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date)) && !string.IsNullOrEmpty(gbCode)) && (gbCode.Substring(0, 1).ToString() == "3"))//6岁以下儿童，并且是临床诊疗类项目
                    //{
                    //    decimal spPrice = 0;
                    //    if (this.GetChildSpPrice(itemCode, ref spPrice) == -1)//获取儿童价，临床诊疗类加收30%的价格
                    //    {
                    //        spPrice = UnitPrice;
                    //    }
                    //    if (spPrice <= 0)
                    //    {
                    //        spPrice = UnitPrice;
                    //    }
                    //    return spPrice;//儿童价
                    //}
                    return UnitPrice;
                    #endregion
                }
                else
                {
                    return UnitPrice;
                }
            }
            else if (register != null && register.Pact != null && register.Pact.PriceForm == "MDT价")
            {
                #region 药品
                if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")
                {
                    Neusoft.HISFC.Models.Pharmacy.Item item = this.GetPharmacyInfoForCode(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    if (item.Type.ID.ToString() == "C")//|| item.ExtendData2.ToString() == "Y")
                    {
                        if (item.PriceCollection.RetailPrice != 0)
                        {
                            return item.PriceCollection.RetailPrice;
                        }
                        else
                        {
                            return UnitPrice;
                        }
                    }
                    else
                    {
                        if (PurchasePrice != 0)
                        {
                            return PurchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return UnitPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    }
                }
                #endregion

                #region  非药品
                else if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")
                {
                    decimal mdtpric = 0;
                    int i = this.ExecQuery(string.Format(mdtsql, itemCode));
                    if (i > 0)
                    {
                        if (this.Reader != null)
                        {
                            try
                            {
                                if (this.Reader.Read())
                                {
                                    mdtpric = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[4]);
                                    SPPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[3]);
                                }
                            }
                            finally
                            {
                                if (this.Reader.IsClosed == false)
                                {
                                    this.Reader.Close();
                                }
                                if (mdtpric > 0)
                                {
                                    UnitPrice = mdtpric;
                                    decimal ssss = 0.0m;
                                    //undrug = Neusoft.SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemCode);
                                    getPrice(itemCode, ref ssss, ref ChildPrice, ref SPPrice, ref gbCode, ref ISChildPrice);
                                    // 6岁及以下儿童临床诊疗类项目加收30%
                                    if ((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date))//6岁及以下儿童
                                    {
                                        if ((!string.IsNullOrEmpty(gbCode) && (gbCode.Substring(0, 1).ToString() == "3")) || ISChildPrice == "1")//国标码为3开头
                                        {
                                            UnitPrice *= 1.3M;
                                        }
                                    }


                                    //if ((((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date)) && !string.IsNullOrEmpty(undrug.GBCode)) && (undrug.GBCode.Substring(0, 1).ToString() == "3"))//6岁以下儿童，并且是临床诊疗类项目
                                    //{
                                    //    UnitPrice *= 1.3M;
                                    //}

                                }
                            }
                        }
                    }

                    return UnitPrice;


                }
                #endregion
                else
                {
                    return UnitPrice;
                }
            }
            else if (register != null && register.Pact != null && register.Pact.PriceForm == "围产中心价")//20190821  {D5525B74-0581-41fe-962D-76C2C7C800E2}
            {
                //if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")//药品
                //{
                //    #region 药品
                //    Neusoft.HISFC.Models.Pharmacy.Item item = this.GetPharmacyInfoForCode(itemCode);
                //    //中成药，中草药，二类疫苗 不走零差价
                //    if (item.Type.ID.ToString() == "C")//|| item.ExtendData2.ToString() == "Y")
                //    {
                //        if (item.PriceCollection.RetailPrice != 0)
                //        {
                //            return item.PriceCollection.RetailPrice;
                //        }
                //        else
                //        {
                //            return UnitPrice;
                //        }
                //    }
                //    else
                //    {
                //        if (PurchasePrice != 0)
                //        {
                //            return PurchasePrice;
                //        }
                //        else
                //        {
                //            decimal retailPrice2 = item.RetailPrice2;
                //            if (retailPrice2 == 0)
                //            {
                //                return UnitPrice;
                //            }
                //            else
                //            {
                //                return retailPrice2;
                //            }
                //        }
                //    }
                //    #endregion
                //}
                //else if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")//非药品
                //{
                //    #region 非药品
                //    var undrugItem = this.GetUndrug(itemCode);
                //    if (undrugItem == null || undrugItem.WEICHAN_PRICE == 0)
                //    {
                //        return UnitPrice;
                //    }

                //    UnitPrice = (decimal)undrugItem.WEICHAN_PRICE;
                //    SPPrice = (decimal)undrugItem.UNIT_PRICE2;
                //    //6岁及以下儿童临床诊疗类项目加收30%
                //    if ((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date))//6岁及以下儿童
                //    {
                //        if ((!string.IsNullOrEmpty(undrugItem.GB_CODE) && (undrugItem.GB_CODE.Substring(0, 1).ToString() == "3")) || undrugItem.MARK5 == "1")//国标码为3开头
                //        {
                //            UnitPrice *= 1.3M;
                //        }

                //    }

                //    //if ((((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date)) && !string.IsNullOrEmpty(undrugItem.NameCollection.GbCode)) && (undrugItem.NameCollection.GbCode.Substring(0, 1).ToString() == "3"))
                //    //{
                //    //    UnitPrice *= 1.3M;
                //    //}

                //    return UnitPrice;
                //    #endregion
                //}
                //else
                //{
                return UnitPrice;
                //}
            }
            else
            {
                return UnitPrice;
            }
        }

        public FIN_COM_UNDRUGINFO GetUndrug(string undrugCode)
        {
            string sql = string.Format(@" SELECT item_code, --编码
                       item_name, --名称
                       sys_class, --系统类别
                       fee_code, --最小费用代码 
                       input_code, -- 输入码
                       spell_code, --拼音码
                       wb_code, -- 五笔码
                       gb_code, -- 国家编码
                       international_code, --国际编码 
                       decode(unitflag, '1', fun_get_packageprice(item_code), unit_price) unit_price, --三甲价
                       stock_unit, --单位
                       emerg_scale, -- 急诊加成比例
                       family_plane, -- 计划生育标记 
                       special_item, --特定诊疗项目
                       item_grade, --甲乙类标志
                       confirm_flag, --确认标志 1 需要确认 0 不需要确认 
                       valid_state, --有效性标识 0 在用 1 停用 2 废弃    
                       specs, --规格
                       exedept_code, --执行科室
                       facility_no, --设备编号 用 | 区分 
                       default_sample, --默认检查部位或标本
                       operate_code, -- 手术编码 
                       operate_kind, -- 手术分类
                       operate_type, --手术规模 
                       collate_flag, --是否有物资项目与之对照(1有，0没有) 
                       mark, --备注     
                       decode(unitflag, '1', fun_get_packageprice1(item_code), UNIT_PRICE1) UNIT_PRICE1, --儿童价
                       decode(unitflag, '1', fun_get_packageprice2(item_code), UNIT_PRICE2) UNIT_PRICE2, --特诊价
                       SPECIAL_FLAG, --省限制
                       SPECIAL_FLAG1, --市限制 
                       SPECIAL_FLAG2, --自费项目
                       SPECIAL_FLAG3, --特殊标识
                       SPECIAL_FLAG4, --特殊标识
                       UNIT_PRICE3, --单价3  住院mdt价（凤凰国际病区）
                       UNIT_PRICE4, --单价2    
                       DISEASE_CLASS, -- 疾病分类  
                       SPECIAL_DEPT, -- 专科名称  
                       MARK1, --病史及检查
                       MARK2, --检查要求 
                       MARK3, --  注意事项      
                       CONSENT_FLAG,
                       MARK4, --  检查申请单名称  
                       needbespeak, -- 是否需要预约 
                       ITEM_AREA,
                       ITEM_NOAREA,
                       UNITFLAG, -- 单位标识(0,明细; 1,组套) 
                       APPLICABILITYAREA, --有效范围
                       DEPT_LIST, --允许开立科室
                       ITEM_PRICETYPE, --物价费用类别
                       ORDERPRINT_TAG, --医嘱打印标示
                       oper_code, --申请人
                       oper_date, --停用时间
                       OTHER_NAME,
                       other_spell,
                       other_wb,
                       other_custom,
                       english_name,
                       english_other,
                       english_regular,
                       register_code,
                       register_date,
                       producer_info,
                       MTTYPECODE,
                       MTTYPENAME,
                       EXEC_DEPT_OUT,--默认执行科室（门诊）
                       exec_dept_in, --默认执行科室（住院）
                       MDT_PRICE,     --mdt价格
                       unit_price3,-- 住院mdt价（凤凰国际病区）
                       SCHOOL_PRICE,--校区门诊价
                       WeiChan_Price,--围产中心价格
                       mark5,--是否需要加收儿童价
                       SixYearsOldCountryCode,
                       SpecialNeedsCountryCode 
                  FROM fin_com_undruginfo  where fin_com_undruginfo.item_code='{0}' ", undrugCode);

            return this.GetModelTForSql<FIN_COM_UNDRUGINFO>(sql);
        }


        private int getPrice(string itemCode, ref decimal UnitPrice, ref decimal ChildPrice, ref decimal SPPrice, ref string gbCode, ref string ISChildPrice)
        {
            string sql = @"select t.item_code,t.unit_price,t.unit_price1,t.unit_price3,t.GB_CODE,t.mark5 from fin_com_undruginfo t where t.item_code='{0}'";
            int i = this.ExecQuery(string.Format(sql, itemCode));
            if (this.ExecQuery(string.Format(sql, itemCode)) == -1)
            {
                return -1;
            }
            while (this.Reader.Read())
            {
                UnitPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[1]);
                ChildPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[2]);
                SPPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[3]);
                gbCode = this.Reader[4].ToString();
                ISChildPrice = this.Reader[5].ToString();
            };
            this.Reader.Close();
            return 1;
        }
        /// <summary>
        /// 判断项目是否配置了限制收费规则，并返回限制上限。
        /// </summary>
        /// <remarks>
        /// 结合当前仓库中的迁移资料，这批 <c>Restrictingfee</c> 常数主要承载“2 小时窗口内连续收费的数量上限”。
        /// 这里返回的 <paramref name="LimitNumber"/> 对应字典中的最大收费数量；具体时间窗过滤仍在历史 SQL 中完成。
        /// </remarks>
        /// <param name="itemcode">项目编码。</param>
        /// <param name="LimitNumber">返回配置中的限制次数或限制数量。</param>
        /// <returns>大于 0 表示项目存在限制收费配置；否则视为未配置。</returns>
        public int SetRestrictingfee(string itemcode, ref decimal LimitNumber)
        {
            int returnRows = 0;
            string strSql = string.Empty;
            if (this.Sql.GetSql("Fee.PactUnitItemRate.Restrictingfee", ref strSql) == -1)
            {
                return returnRows;
            }
            if (this.ExecQuery(strSql, itemcode) == -1)
            {
                return returnRows;
            }
            try
            {
                while (this.Reader.Read())
                {
                    returnRows = NConvert.ToInt32(this.Reader[0]);
                    LimitNumber = NConvert.ToDecimal(this.Reader[1]);
                }
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return returnRows;
            }
            finally
            {
                this.Reader.Close();
            }
            return returnRows;
        }
        /// <summary>
        /// 查询项目是否配置了折价收费规则及其参数。
        /// </summary>
        /// <param name="itemcode">项目编码。</param>
        /// <param name="DISCOUNT_RATE">返回折扣率。</param>
        /// <param name="TOPPRICE">返回封顶金额。</param>
        /// <returns>折价类型标识；默认 <c>"1"</c> 表示无折价，<c>"2"</c> 表示需要执行折价收费。</returns>
        public string SetDiscountfee(string itemcode, ref decimal DISCOUNT_RATE, ref int TOPPRICE)
        {
            string Discount_type = "1";
            string strSql = string.Empty;
            if (this.Sql.GetSql("Fee.PactUnitItemRate.Discountfee", ref strSql) == -1)
            {
                return Discount_type;
            }
            if (this.ExecQuery(strSql, itemcode) == -1)
            {
                return Discount_type;
            }
            try
            {
                while (this.Reader.Read())
                {
                    DISCOUNT_RATE = NConvert.ToDecimal(this.Reader[0]);
                    TOPPRICE = NConvert.ToInt32(this.Reader[1]);
                    Discount_type = this.Reader[2].ToString();

                }
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return Discount_type;
            }
            finally
            {
                this.Reader.Close();
            }
            return Discount_type;
        }
        /// <summary>
        /// 查询患者历史上同项目已收费次数，并返回对应的限制收费 F 码。
        /// </summary>
        /// <param name="CARD_NO">患者卡号。</param>
        /// <param name="itemCode">项目编码。</param>
        /// <param name="Feecode">返回限制收费口径对应的 F 码。</param>
        /// <returns>历史已收费次数。</returns>
        public int getRestrictingfee(string CARD_NO, string itemCode, ref string Feecode)
        {
            int feetype = 0;
            string sql = string.Empty;
            if (this.Sql.GetSql("Fee.PactUnitItemRate.RestrictingfeePay2", ref sql) == -1)
            {
                return feetype;
            }

            if (this.ExecQuery(string.Format(sql, CARD_NO, itemCode)) == -1)
            {
                return 0;
            }
            while (this.Reader.Read())
            {
                Feecode = this.Reader[0].ToString();
                feetype =  NConvert.ToInt32(this.Reader[1]);
            };
            this.Reader.Close();
            return feetype;
        }
        /// <summary>
        /// 按分组收费口径查询患者历史已收费次数。
        /// </summary>
        /// <param name="CARD_NO">患者卡号。</param>
        /// <param name="itemCode">项目编码。</param>
        /// <param name="GroupNumber">组套分组号。</param>
        /// <param name="Feecode">返回限制收费口径对应的 F 码。</param>
        /// <returns>该分组维度下历史已收费次数。</returns>
        public int getRestrictingfeeZT(string CARD_NO, string itemCode, string GroupNumber, ref string Feecode)
        {
            int feetype = 0;
            string sql = string.Empty;
            if (this.Sql.GetSql("Fee.PactUnitItemRate.RestrictingfeePayZT", ref sql) == -1)
            {
                return feetype;
            }

            if (this.ExecQuery(string.Format(sql, CARD_NO, itemCode, GroupNumber)) == -1)
            {
                return 0;
            }
            while (this.Reader.Read())
            {
                Feecode = this.Reader[0].ToString();
                feetype = NConvert.ToInt32(this.Reader[1]);
            };
            this.Reader.Close();
            return feetype;
        }
        /// <summary>
        /// 根据项目编码反查限制收费口径对应的 F 码。
        /// </summary>
        /// <param name="itemCode">项目编码或组套编码。</param>
        /// <param name="Feecode">返回的限制收费 F 码。</param>
        public void getRestrictingfeecode(string itemCode, ref string Feecode)
        {
            string sql = @" 
           select feecode  
           from  
           ( select  undinfo.PACKAGE_CODE AS code,dic.code as feecode from fin_com_undrugztinfo undinfo 
           inner join  ( select code from com_dictionary   where  TYPE='Restrictingfee')dic 
           on undinfo.item_code=dic.code and undinfo.VALID_STATE='1' 
           union all         
           select code,code as feecode from com_dictionary   where  TYPE='Restrictingfee')
           where code='{0}' and  rownum=1 ";
            if (this.ExecQuery(string.Format(sql, itemCode)) == -1)
            {
            }
            while (this.Reader.Read())
            {
                Feecode = this.Reader[0].ToString();
            };
            this.Reader.Close();
        }
        /// <summary>
        /// 根据常数字典类型读取全部在用配置。
        /// </summary>
        /// <param name="type">
        /// 字典类型，例如：
        /// Restrictingfee（数量上限）、
        /// RestrictingfeeZT（同组互斥）、
        /// RestrictingfeeCP（床旁限制）、
        /// RestrictingfeeTX1（胎心监护限制）、
        /// Astrictpackagefee（组套豁免项）、
        /// ItemZT（CT/MR/DR 组套拆分规则）。
        /// </param>
        /// <returns>对应类型的在用常数集合。</returns>
        public ArrayList GetList(string type)
        {
            string strSql = "";

            ArrayList al = new ArrayList();
            //接口说明
            //返回
            if (this.Sql.Sql.GetSql("Manager.Constant.2", ref strSql) == -1) return null;

            try
            {
                strSql = string.Format(strSql, type);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = "接口错误！" + ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(strSql) == -1) return null;
            //Neusoft.FrameWork.Models.NeuObject NeuObject;
            Neusoft.HISFC.Models.Base.Const cons;
            while (this.Reader.Read())
            {
                //NeuObject=new NeuObject();
                cons = new Const();
                //cons.Type = (Const.enuConstant)(Reader[0].ToString());
                cons.ID = this.Reader[1].ToString();
                cons.Name = this.Reader[2].ToString();
                cons.Memo = this.Reader[3].ToString();
                cons.SpellCode = this.Reader[4].ToString();
                cons.WBCode = this.Reader[5].ToString();
                cons.UserCode = this.Reader[6].ToString();
                if (!Reader.IsDBNull(7))
                    cons.SortID = Convert.ToInt32(this.Reader[7]);
                cons.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[8].ToString());
                cons.OperEnvironment.ID = this.Reader[9].ToString();
                if (!Reader.IsDBNull(10))
                    cons.OperEnvironment.OperTime = Convert.ToDateTime(this.Reader[10].ToString());


                al.Add(cons);
            }
            this.Reader.Close();
            return al;
        }
        /// <summary>
        /// 获取价格
        /// </summary>
        public int GetPricesz(string ItemID, ref decimal Price)
        {
            //{B9303CFE-755D-4585-B5EE-8C1901F79450} 整合时对照修改此函数
            string strSql = "";
            //获得患者合同单位
            try
            {
                #region 非药品取项目价格

                if (this.Sql.GetCommonSql("Fee.InvoiceService.GetUndrugPrice", ref strSql) == -1) return -1;
                strSql = string.Format(strSql, ItemID);
                if (this.ExecQuery(strSql) == -1) return -1;
                int count = 0;

                while (Reader.Read())
                {
                    Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[0].ToString());
                    count++;
                }
                Reader.Close();

                if (count == 0)
                {
                    return -1;
                }
                #endregion

            }
            catch (Exception e)
            {
                this.Err = "Fee.InvoiceService.GetUndrugPrice";
                this.ErrCode = e.Message;
                if (Reader.IsClosed == false) Reader.Close();
                return -1;
            }
            return 0;
        }
        private int GetChildSpPrice(string itemCode, ref decimal spPrice)
        {
            try
            {
                string sql = @"select t.item_code,t.unit_price,t.unit_price1,t.unit_price3,t.GB_CODE,t.mark5 from fin_com_undruginfo t where t.item_code='{0}'";
                if (base.ExecQuery(string.Format(sql, itemCode)) == -1)
                {
                    return -1;
                }
                while (base.Reader.Read())
                {
                    spPrice = NConvert.ToDecimal(base.Reader[2]);//unit_price1 字段
                }
            }
            catch (Exception exception)
            {
                base.Err = exception.Message;
                return -1;
            }
            finally
            {
                this.Reader.Close();
            }

            return 1;
        }

        private System.Collections.Hashtable GetFeeCodeHs(string fee_stat_cate)
        {
            System.Collections.Hashtable hsFeeCode = new System.Collections.Hashtable();
            string strSql = " select fee_stat_name,fee_code from fin_com_feecodestat where report_code = 'MZ01' and fee_stat_cate = '{0}'";
            if (this.ExecQuery(string.Format(strSql, fee_stat_cate)) == -1)
            {
                return hsFeeCode;
            }
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.Name = this.Reader[0].ToString();//项目分类名称
                obj.ID = this.Reader[1].ToString();//最小费用代码
                hsFeeCode.Add(obj.ID, obj);
            };
            this.Reader.Close();
            return hsFeeCode;
        }

        private int GetItemFeeCodeAndSchoolPrice(string itemCode, ref Neusoft.FrameWork.Models.NeuObject obj)
        {
            string strSql = " select fee_code,school_price,item_name from fin_com_undruginfo where item_code = '{0}' and valid_state = fun_get_valid";
            if (this.ExecQuery(string.Format(strSql, itemCode)) == -1)
            {
                return -1;
            }
            while (this.Reader.Read())
            {
                obj.ID = this.Reader[0].ToString();//最小费用代码
                obj.Memo = this.Reader[1].ToString();//校区门诊价格shcool_price
                obj.Name = this.Reader[2].ToString();//项目名称
            }
            this.Reader.Close();
            return 1;
        }

        public Neusoft.HISFC.Models.Base.PactItemRate GetOnepPactUnitItemRateByItem(string pact_code, string Item)
        {
            return GetOnePactUnitItemRate(pact_code, Item, 1);
        }
        private Neusoft.HISFC.Models.Base.PactItemRate GetOnePactUnitItemRate(string pact_code, string ItemOrMincode, int index)
        {
            string strSql = "";
            Neusoft.HISFC.Models.Base.PactItemRate info = null;
            if (index == 0)
            {
                if (this.Sql.GetCommonSql("Fee.PactUnitItemRate.GetOnePactUnitItemRate", ref strSql) == -1) return null;
                strSql = string.Format(strSql, pact_code, ItemOrMincode);
            }
            else
            {
                if (this.Sql.GetCommonSql("Fee.PactUnitItemRate.GetOnePactUnitItemRate2", ref strSql) == -1) return null;
                strSql = string.Format(strSql, pact_code, ItemOrMincode);
            }
            try
            {
                if (this.ExecQuery(strSql) == -1) return null;
                if (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.Base.PactItemRate();
                    //合同代码
                    info.ID = Reader[0].ToString();
                    //名称 (最小费用或项目名称)Item.Name
                    info.PactItem.Name = Reader[1].ToString();
                    //类别  int ItemType
                    info.ItemType = Reader[2].ToString();
                    //公费比例 float PubRate
                    info.Rate.PubRate = Convert.ToDecimal(Reader[3].ToString());
                    //自费比例 float OwnRate
                    info.Rate.OwnRate = Convert.ToDecimal(Reader[4].ToString());
                    //自付比例 float PayRate
                    info.Rate.PayRate = Convert.ToDecimal(Reader[5].ToString());
                    //优惠比例 float RebateRate   {1C0DA8D4-50FF-4097-B29F-C6CB21595A1B}
                    info.Rate.RebateRate = Convert.ToDecimal(Reader[6].ToString());
                    //欠费比例 float ArrearageRate
                    info.Rate.ArrearageRate = Convert.ToDecimal(Reader[7].ToString());
                    //编码（最小费用或项目代码）Item.id
                    info.PactItem.ID = Reader[8].ToString();
                    //限额
                    if (this.Reader.FieldCount >= 10)
                    {
                        info.Rate.Quota = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[9].ToString());
                    }
                }
                this.Reader.Close();
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                info = null;
            }
            return info;
        }

        public decimal GetItemRateForZT(string packageCode, string itemCode)
        {
            string sql = string.Format(@" select nvl(p.item_rate,1) from fin_com_undrugztinfo p where p.package_code='{0}' and p.item_code='{1}' ", packageCode, itemCode);
            return NConvert.ToDecimal(this.ExecSqlReturnOne(sql));
        }

        public bool IsZTUndrugInfo(string itemCode)
        {
            string sql = string.Format(@" select p.unitflag from fin_com_undruginfo p where p.item_code='{0}' ", itemCode);
            var result = this.ExecSqlReturnOne(sql, "0");
            return result == "1";
        }

        public string GetNeedConfirmForItemCode(string itemCode)
        {
            string sql = string.Format(@" select p.confirm_flag from fin_com_undruginfo p where p.item_code ='{0}' ", itemCode);
            var result = this.ExecSqlReturnOne(sql, "");
            return result;
        }
        public Undrug GetUndrugByCode(string undrugCode)
        {
            return GetItemByUndrugCode(undrugCode);
        }
        /// <summary>
        /// 根据非药品编码获得该项目信息(不管是否有效)
        /// </summary>
        /// <param name="undrugCode">非药品编码</param>
        /// <returns>成功:返回非药品实体 失败:返回null</returns>
        public Undrug GetItemByUndrugCode(string undrugCode)
        {
            List<Undrug> list = this.QueryUndrugItemsBase("Fee.Item.Query.ByCode", undrugCode, "all");
            if (list != null
                && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }
        private List<Undrug> QueryUndrugItemsBase(string whereSqlIndex, params string[] args)
        {
            string selectSQL = string.Empty; //获得全部非药品信息的SELECT语句

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.Item.Main", ref selectSQL) == -1)
            {
                this.Err = "没有找到SQL语句，ID为[Fee.Item.Main]!";
                this.WriteErr();

                return null;
            }

            string whereSQL = string.Empty;
            if (Sql.GetCommonSql(whereSqlIndex, ref whereSQL) == -1)
            {
                this.Err = "没有找到SQL语句，ID为[" + whereSqlIndex + "]!";
                this.WriteErr();

                return null;
            }

            whereSQL = string.Format(whereSQL, args);

            selectSQL = selectSQL + "\r\n" + whereSQL;

            return this.QueryUndrugItemsBase(selectSQL);
        }
        /// <summary>
        /// 取非药品基本信息
        /// </summary>
        /// <param name="sql">当前Sql语句,不是SQLID</param>
        /// <returns>成功返回非药品数组 失败返回null</returns>
        private List<Undrug> QueryUndrugItemsBase(string sql)
        {
            List<Undrug> items = new List<Undrug>(); //用于返回非药品信息的数组

            //执行当前Sql语句
            if (this.ExecQuery(sql) == -1)
            {
                this.Err = this.Sql.Err;

                return null;
            }

            try
            {
                //循环读取数据
                while (this.Reader.Read())
                {
                    Undrug item = new Undrug();

                    item.ID = this.Reader[0].ToString();//非药品编码 
                    item.Name = this.Reader[1].ToString(); //非药品名称 
                    item.SysClass.ID = this.Reader[2].ToString(); //系统类别
                    item.MinFee.ID = this.Reader[3].ToString();  //最小费用代码 
                    item.UserCode = this.Reader[4].ToString(); //输入码

                    item.SpellCode = this.Reader[5].ToString(); //拼音码
                    item.WBCode = this.Reader[6].ToString();    //五笔码
                    item.GBCode = this.Reader[7].ToString();    //国家编码
                    item.NationCode = this.Reader[8].ToString();//国际编码
                    item.Price = NConvert.ToDecimal(this.Reader[9].ToString()); //默认价
                    item.PriceUnit = this.Reader[10].ToString();  //计价单位
                    item.FTRate.EMCRate = NConvert.ToDecimal(this.Reader[11].ToString()); // 急诊加成比例
                    item.IsFamilyPlanning = NConvert.ToBoolean(this.Reader[12].ToString()); // 计划生育标记 
                    item.User01 = this.Reader[13].ToString(); //特定诊疗项目
                    item.Grade = this.Reader[14].ToString();//甲乙类标志
                    if (string.IsNullOrEmpty(this.Reader[15].ToString()))
                    {
                        item.IsNeedConfirm = false;
                        item.NeedConfirm = EnumNeedConfirm.None;
                    }
                    else
                    {
                        if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                            Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[15].ToString())))
                        {
                            item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), this.Reader[15].ToString());
                            if (item.NeedConfirm == EnumNeedConfirm.All || item.NeedConfirm == EnumNeedConfirm.Inpatient)
                            {
                                item.IsNeedConfirm = true;
                            }
                        }
                    }
                    item.ValidState = this.Reader[16].ToString(); //有效性标识 在用 1 停用 0 废弃 2   
                    item.Specs = this.Reader[17].ToString(); //规格
                    item.ExecDept = this.Reader[18].ToString();//执行科室
                    item.MachineNO = this.Reader[19].ToString(); //设备编号 用 | 区分 
                    item.CheckBody = this.Reader[20].ToString(); //默认检查部位或标本
                    item.OperationInfo.ID = this.Reader[21].ToString(); // 手术编码 
                    item.OperationType.ID = this.Reader[22].ToString(); // 手术分类
                    item.OperationScale.ID = this.Reader[23].ToString(); //手术规模 
                    item.IsCompareToMaterial = NConvert.ToBoolean(this.Reader[24].ToString());//是否有物资项目与之对照(1有，0没有) 
                    item.Memo = this.Reader[25].ToString(); //备注  
                    item.ChildPrice = NConvert.ToDecimal(this.Reader[26].ToString()); //儿童价
                    item.SpecialPrice = NConvert.ToDecimal(this.Reader[27].ToString()); //特诊价
                    item.SpecialFlag = this.Reader[28].ToString(); //省限制
                    item.SpecialFlag1 = this.Reader[29].ToString(); //市限制
                    item.SpecialFlag2 = this.Reader[30].ToString(); //自费项目
                    item.SpecialFlag3 = this.Reader[31].ToString();// 特殊检查
                    item.SpecialFlag4 = this.Reader[32].ToString();// 备用		
                    item.DiseaseType.ID = this.Reader[35].ToString(); //疾病分类
                    item.SpecialDept.ID = this.Reader[36].ToString();  //专科名称
                    item.MedicalRecord = this.Reader[37].ToString(); //  --病史及检查
                    item.CheckRequest = this.Reader[38].ToString();//--检查要求
                    item.Notice = this.Reader[39].ToString();//--  注意事项  
                    item.IsConsent = NConvert.ToBoolean(this.Reader[40].ToString());
                    item.CheckApplyDept = this.Reader[41].ToString();//检查申请单名称
                    item.IsNeedBespeak = NConvert.ToBoolean(this.Reader[42].ToString());//是否需要预约
                    item.ItemArea = this.Reader[43].ToString();//项目范围
                    item.ItemException = this.Reader[44].ToString();//项目约束
                    item.UnitFlag = this.Reader[45].ToString(); //[]单位标识 
                    item.ApplicabilityArea = this.Reader[46].ToString();
                    item.DeptList = this.Reader[47].ToString(); //允许开立科室
                    item.ItemPriceType = this.Reader[48].ToString();  //物价费用类别
                    item.IsOrderPrint = this.Reader[49].ToString();   //医嘱单打印标示
                    item.Oper.ID = this.Reader[50].ToString();   //申请人

                    item.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[51].ToString()); //停止时间
                    item.NameCollection.OtherName = Reader[52].ToString();//别名
                    item.NameCollection.OtherSpell.SpellCode = Reader[53].ToString();//别名拼音码
                    item.NameCollection.OtherSpell.WBCode = Reader[54].ToString();//别名五笔码
                    item.NameCollection.OtherSpell.UserCode = Reader[55].ToString();//别名自定义码


                    items.Add(item);
                }

                return items;
            }
            catch (Exception e)
            {
                this.Err = "获得非药品基本信息出错！" + e.Message;
                this.WriteErr();

                return null;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

    }
}
