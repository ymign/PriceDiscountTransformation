using System;
using System.Collections;
using System.Data;
using Neusoft.HISFC.Models.Fee.Item;
using Neusoft.FrameWork.Function;
using System.Collections.Generic;

namespace Neusoft.HISFC.BizLogic.Fee
{
    /// <summary>
    /// Item<br></br>
    /// [功能描述: 非药品信息业务类]<br></br>
    /// [创 建 者: 王宇]<br></br>
    /// [创建时间: 2006-09-25]<br></br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class Item : Neusoft.FrameWork.Management.Database
    {
        #region 私有函数

        /// <summary>
        /// 根据SqlIndex查询非药品列表
        /// </summary>
        /// <param name="whereSqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获得update或者insert非药品字典表的传入参数数组
        /// </summary>
        /// <param name="undrug">非药品实体</param>
        /// <returns>参数数组</returns>
        private string[] GetItemParams(Undrug undrug)
        {
            string[] args = 
			{	
				undrug.ID, 
				undrug.Name,
				undrug.SysClass.ID.ToString(),
				undrug.MinFee.ID.ToString(),
				undrug.UserCode, 
				undrug.SpellCode,
				undrug.WBCode,
				undrug.GBCode, 
				undrug.NationCode,
				undrug.Price.ToString(),
				undrug.PriceUnit,						
				undrug.FTRate.EMCRate.ToString(),
				NConvert.ToInt32(undrug.IsFamilyPlanning).ToString(),	
				"",  				        
				undrug.Grade,					
				//NConvert.ToInt32(undrug.IsNeedConfirm).ToString(),
                ((int)undrug.NeedConfirm).ToString(),
				Neusoft.FrameWork.Function.NConvert.ToInt32(undrug.ValidState).ToString(),		
				undrug.Specs,					
				undrug.ExecDept,					
				undrug.MachineNO,
				undrug.CheckBody,				
				undrug.OperationInfo.ID,                 
				undrug.OperationType.ID,			
				undrug.OperationScale.ID,
				NConvert.ToInt32(undrug.IsCompareToMaterial).ToString(),		
				undrug.Memo,					
				undrug.Oper.ID ,	
				undrug.ChildPrice.ToString(),            
				undrug.SpecialPrice.ToString(),         
				undrug.SpecialFlag,                   
				undrug.SpecialFlag1,                          
				undrug.SpecialFlag2,
				undrug.SpecialFlag3,                     
				undrug.SpecialFlag4,                  
				"0",     
                "0",
				undrug.DiseaseType.ID ,
				undrug.SpecialDept.ID,
				NConvert.ToInt32(undrug.IsConsent).ToString(),
				undrug.MedicalRecord,                        
				undrug.CheckRequest,                          
				undrug.Notice,					
				undrug.CheckApplyDept,
				NConvert.ToInt32(undrug.IsNeedBespeak).ToString(),
			    undrug.ItemArea,
			    undrug.ItemException,
                undrug.UnitFlag,/*[2007/01/19]后加的字段,单位标识46*/
                undrug.ApplicabilityArea,
               //{2A5608D8-26AD-47d7-82CC-81375722FF72}
                undrug.DeptList,
                //{55CFCB36-B084-4a56-95AD-2CDED962ADC4}
                undrug.ItemPriceType,
                undrug.IsOrderPrint
			};

            return args;
        }

        /// <summary>
        /// 通过项目数组获得数组脚标为0的元素,转换成非药品实体
        /// </summary>
        /// <param name="items">非药品项目数组</param>
        /// <returns>成功返回非药品实体,失败返回null</returns>
        private Undrug GetItemFromArrayList(ArrayList items)
        {
            //如果获得数组为空,说明sql或者其他原因产生错误
            if (items == null)
            {
                return null;
            }
            //如果获得的数组元素数大于0,说明查找到了项目,理论上只能有一个元素
            //所以取脚标为0的元素,转成Undrug实体
            if (items.Count > 0)
            {
                Undrug tempUndrug = items[0] as Undrug;

                return tempUndrug;
            }
            else//如果元素数等于0(不可能小于0),说明此编码的非药品项目不存在
            {
                return null;
            }
        }

        /// <summary>
        /// 通过项目数组获得数组脚标为0的元素,转换成非药品实体
        /// </summary>
        /// <param name="items">非药品项目数组</param>
        /// <returns>成功返回非药品实体,失败返回null</returns>
        private Undrug GetItemFromList(List<Undrug> items)
        {
            //如果获得数组为空,说明sql或者其他原因产生错误
            if (items == null)
            {
                return null;
            }
            //如果获得的数组元素数大于0,说明查找到了项目,理论上只能有一个元素
            //所以取脚标为0的元素,转成Undrug实体
            if (items.Count > 0)
            {
                return items[0];
            }
            else//如果元素数等于0(不可能小于0),说明此编码的非药品项目不存在
            {
                return null;
            }
        }

        #endregion

        #region 公有函数

        /// <summary>
        /// 按照查询条件获得非药品信息列表
        /// </summary>
        /// <returns>成功:返回非药品实体数组 失败:返回null</returns>
        public int QueryMaxCode()
        {
            string sql = string.Empty; //获得全部非药品信息的SELECT语句

            //取SELECT语句
            if (this.Sql.GetSql("Fee.Item.ZTMaxCode", ref sql) == -1)
            {
                this.Err = "没有找到Fee.Item.ZTMaxCode字段!";
                this.WriteErr();

                return -1;
            }
            //格式化SQL语句
            try
            {
                sql = string.Format(sql);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return -1;
            }
            //根据SQL语句取非药品类数组并返回数组
            // 2A5608D8-26AD-47d7-82CC-81375722FF72}
            //return this.GetItemsBySql(sql);
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql, "-1"));
        }


        /// <summary>
        /// 获得所有可能的项目信息
        /// </summary>
        /// <returns>成功 有效的可用项目信息, 失败 null</returns>
        public ArrayList QueryValidZTItems()
        {
            return this.QueryZTItems("all", "1");
        }

        /// <summary>
        /// 按照查询条件获得非药品信息列表
        /// </summary>
        /// <param name="undrugCode">如果为非药品编码为查询单一项目,为字符串"all"时为查询所有项目</param>
        /// <param name="validState">非药品状态: 再用(1) 停用(0) 废弃(2) 所有(all)</param>
        /// <returns>成功:返回非药品实体数组 失败:返回null</returns>
        public ArrayList QueryZTItems(string undrugCode, string validState)
        {
            string sql = string.Empty; //获得全部非药品信息的SELECT语句

            //取SELECT语句
            if (this.Sql.GetSql("Fee.Item.ZTInfo", ref sql) == -1)
            {
                this.Err = "没有找到Fee.Item.ZTInfo字段!";
                this.WriteErr();

                return null;
            }
            //格式化SQL语句
            try
            {
                sql = string.Format(sql, undrugCode, validState);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return null;
            }
            //根据SQL语句取非药品类数组并返回数组
            // 2A5608D8-26AD-47d7-82CC-81375722FF72}
            //return this.GetItemsBySql(sql);
            return this.GetItemsBySqlInfo(sql);
        }

        /// <summary>
        /// 取非药品基本信息数组  2A5608D8-26AD-47d7-82CC-81375722FF72}
        /// </summary>
        /// <param name="sql">当前Sql语句</param>
        /// <returns>成功返回非药品数组 失败返回null</returns>
        private ArrayList GetItemsBySqlInfo(string sql)
        {
            ArrayList items = new ArrayList(); //用于返回非药品信息的数组

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
                    item.IsNeedConfirm = NConvert.ToBoolean(this.Reader[15].ToString());//确认标志 1 需要确认 0 不需要确认
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
                    item.UnitFlag = this.Reader[45].ToString();// []单位标识
                    item.ApplicabilityArea = this.Reader[46].ToString();
                    item.DeptList = this.Reader[47].ToString(); //允许开立科室
                    item.ItemPriceType = this.Reader[48].ToString();  //物价费用类别
                    item.IsOrderPrint = this.Reader[49].ToString();   //医嘱单打印标示
                    item.Oper.ID = this.Reader[50].ToString();   //申请人
                    if (item.ValidState == "0")
                    {
                        item.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[51].ToString()); //停止时间
                    }
                    items.Add(item);
                }//循环结束

                //关闭Reader
                this.Reader.Close();

                return items;
            }
            catch (Exception e)
            {
                this.Err = "获得非药品基本信息出错！" + e.Message;
                this.WriteErr();

                //如果还没有关闭Reader 关闭之
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }

                items = null;

                return null;
            }
        }

        /// <summary>
        /// 判断该项目是否已经使用过
        /// </summary>
        /// <param name="undrugCode">非药品编码</param>
        /// <returns>true 已经使用 false 没有使用</returns>
        public bool IsUsed(string undrugCode)
        {
            string sql = null; //返回的SQL语句
            string returnRows = null; //其他表已经使用的当前非药品数目
            bool isUsed = false; //是否可以删除

            //获得当前非药品的使用次数SQL语句
            if (this.Sql.GetCommonSql("Fee.Item.CanDelete.Select", ref sql) == -1)
            {
                this.Err = "没有找到Fee.Item.CanDelete.Select字段";

                return false;
            }

            //格式化SQL语句
            try
            {
                sql = string.Format(sql, undrugCode);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return false;
            }

            //获得当前非药品的使用次数
            returnRows = this.ExecSqlReturnOne(sql);

            //如果返回条目大于0,该非药品已经使用
            if (NConvert.ToInt32(returnRows) > 0)
            {
                isUsed = true;
            }
            else//返回的条目小于等于0 说明该项目没有使用
            {
                isUsed = false;
            }

            return isUsed;
        }
        /// <summary>
        /// 获取是否开立四肢血管项目
        /// </summary>
        /// <param name="pactCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="minCode"></param>
        /// <returns></returns>
        public bool SetUltrasound(string itemname)
        {
            string strSql = "";
            string returnRows = null;
            if (this.Sql.GetCommonSql("Fee.PactUnitItemRate.setUltrasound", ref strSql) == -1) return false;
            strSql = string.Format(strSql, itemname);

            try
            {
                returnRows = this.ExecSqlReturnOne(strSql);
                if (NConvert.ToInt32(returnRows) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                return false;
            }

        }
        /// <summary>
        /// 获取是否为限制收费项目
        /// </summary>
        /// <param name="pactCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="minCode"></param>
        /// <returns></returns>
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
        //查询主项目是否存在儿童加收项目
        public  string GetAdditionallychargeCode(string itemcode)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string strSql = string.Empty;
            if (this.Sql.GetSql("Fee.PactUnitItemRate.AdditionallychargeCode", ref strSql) == -1)
            {
                return "";
            }
            strSql = string.Format(strSql, itemcode);
            string AdditionallychargeCode = outpatientManager.ExecSqlReturnOne(strSql);
            if (AdditionallychargeCode == "-1")
            {
                return "";
            }
            return AdditionallychargeCode;

        }
        /// <summary>
        /// 查询是否为限制项目及限制参数
        /// </summary>
        /// <param name="pactCode"></param>
        /// <param name="itemCode"></param>
        public string SetDiscountfee(string itemcode, ref decimal DISCOUNT_RATE, ref decimal TOPPRICE)
        {
            string  Discount_type = "1";
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
                    DISCOUNT_RATE = NConvert.ToDecimal(this.Reader[0]);//价格系数
                    TOPPRICE = NConvert.ToDecimal(this.Reader[1]); //单项最高价
                    Discount_type = this.Reader[2].ToString(); //折价类别
 
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
        /// 查询有多少未收的限制项目提示收回
        /// </summary>
        /// <param name="pactCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="minCode"></param>
        /// <returns></returns>
        public void SetLimitCharges(string CARD_NO, string itemcode, string InvoiceNO, ref string itemname, ref decimal paycot, ref decimal deratecot) 
        {
            int returnRows = 0;
            string strSql = string.Empty;
            if (this.Sql.GetSql("Fee.SetLimitCharges", ref strSql) == -1)
            {
            }
            if (this.ExecQuery(strSql, CARD_NO, itemcode, InvoiceNO) == -1)
            {
            }
            try
            {
                while (this.Reader.Read())
                {
                    itemname = this.Reader[0].ToString();
                    paycot = NConvert.ToDecimal(this.Reader[1]);
                    deratecot = NConvert.ToDecimal(this.Reader[2]);
                }
            }
            catch (Exception ex)
            {
                this.Reader.Close();
            }
            finally
            {
                this.Reader.Close();
            }
        }

        /// <summary>
        /// 获取四肢血管加收价格
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
                    this.Err = "Fee.InvoiceService.GetUndrugPrice";
                    this.ErrCode = "未检索到行!";
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
        /// <summary>
        /// 获取住院限制项目已收费数量
        /// </summary>
        public int Getpayqty(string ItemID, string feedate, string ItemCode, ref decimal payqty)
        {
            //{B9303CFE-755D-4585-B5EE-8C1901F79450} 整合时对照修改此函数
            string strSql = "";
            //获得患者合同单位
            try
            {

                if (this.Sql.GetCommonSql("Fee.InvoiceService.Getpayqty", ref strSql) == -1) return -1;
                strSql = string.Format(strSql, ItemID, feedate, ItemCode);
                if (this.ExecQuery(strSql) == -1) return -1;
                int count = 0;

                while (Reader.Read())
                {
                    payqty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[0].ToString());
                    count++;
                }
                Reader.Close();

            }
            catch (Exception e)
            {
                this.Err = "Fee.InvoiceService.Getpayqty";
                this.ErrCode = e.Message;
                if (Reader.IsClosed == false) Reader.Close();
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 按照查询条件获得非药品信息列表
        /// </summary>
        /// <param name="undrugCode">如果为非药品编码为查询单一项目,为字符串"all"时为查询所有项目</param>
        /// <param name="validState">非药品状态: 再用(1) 停用(0) 废弃(2) 所有(all)</param>
        /// <returns>成功:返回非药品实体数组 失败:返回null</returns>
        public ArrayList Query(string undrugCode, string validState)
        {
            List<Undrug> list = this.QueryUndrugItemsBase("Fee.Item.Query.ByCode", undrugCode, validState);
            if (list != null)
            {
                return new ArrayList(list);
            }
            return null;
        }

        /// <summary>
        /// 按照查询条件获得非药品信息列表
        /// </summary>
        /// <param name="undrugCode">如果为非药品编码为查询单一项目,为字符串"all"时为查询所有项目</param>
        /// <param name="validState">非药品状态: 再用(1) 停用(0) 废弃(2) 所有(all)</param>
        /// <returns>成功:返回非药品实体数组 失败:返回null</returns>
        public List<Undrug> QueryList(string undrugCode, string validState)
        {
            return this.QueryUndrugItemsBase("Fee.Item.Query.ByCode", undrugCode, validState);
        }

        /// <summary>
        /// 按照查询条件获得住院非药品信息列表
        /// </summary>
        /// <param name="undrugCode">如果为非药品编码为查询单一项目,为字符串"all"时为查询所有项目</param>
        /// <param name="validState">非药品状态: 再用(1) 停用(0) 废弃(2) 所有(all)</param>
        /// <returns>成功:返回非药品实体数组 失败:返回null</returns>
        public List<Undrug> QueryListWithIn(string undrugCode, string validState)
        {
            return this.QueryUndrugItemsBase("Fee.Item.Query.ByCode.In", undrugCode, validState);
        }

        // {4F4F0803-78F5-4c7f-9A0A-65474148B9E0} 护士站只收材料费(非药品U)
        /// <summary>
        /// 按照查询条件获得住院非药品信息列表
        /// </summary>
        /// <param name="sysCode">如果为非药品编码为查询单一项目类别,为字符串"all"时为查询所有项目</param>
        /// <param name="validState">非药品状态: 再用(1) 停用(0) 废弃(2) 所有(all)</param>
        /// <returns>成功:返回非药品实体数组 失败:返回null</returns>
        public List<Undrug> QueryListWithInMaterial(string sysCode, string validState)
        {
            return this.QueryUndrugItemsBase("Fee.Item.Query.ByCode.In2", sysCode, validState);
        }

        /// <summary>
        /// 查询科室收费项目
        /// </summary>
        /// <param name="dept">科室</param>
        /// <returns></returns>
        public List<Undrug> QueryList(string dept)
        {
            return this.QueryUndrugItemsBase("Fee.Item.Query.ByDept", dept);
        }

        /// <summary>
        /// 根据非药品编码获得该项目信息(该项目必须有效)
        /// </summary>
        /// <param name="undrugCode">非药品编码</param>
        /// <returns>成功:返回非药品实体 失败:返回null</returns>
        public Undrug GetValidItemByUndrugCode(string undrugCode)
        {
            List<Undrug> list = this.QueryUndrugItemsBase("Fee.Item.Query.ByCode", undrugCode,"1");
            if (list != null
                && list.Count > 0)
            {
                return list[0];
            }
            return null;
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

        /// <summary>
        /// 根据非药品编码 批量查询非药品基本信息
        /// </summary>
        /// <param name="undrugCodes"></param>
        /// <returns></returns>
        public List<Undrug> GetItemByCodeBatch(string undrugCodes)
        {
            return this.QueryUndrugItemsBase("Fee.ItemInfo.Query.ByItemID.Batch", undrugCodes);
        }

        /// <summary>
        /// 获得非药品信息
        /// </summary>
        /// <param name="undrugCode"></param>
        /// <returns>成功 非药品信息 失败 null</returns>
        public Undrug GetUndrugByCode(string undrugCode)
        {
            return GetItemByUndrugCode(undrugCode);
        }

        /// <summary>
        /// 根据自定义编码获得非药品信息
        /// </summary>
        /// <param name="userCode">项目自定义码</param>
        /// <returns>成功返回非药品项目实体 失败返回null</returns>
        public Undrug GetItemByUserCode(string userCode)
        {
            List<Undrug> list = this.QueryUndrugItemsBase("Fee.ItemInfo.Query.ByUserCode", userCode);
            if (list != null
                && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        /// <summary>
        /// 获取项目比例
        /// </summary>
        /// <param name="undrugCode"></param>
        /// <returns></returns>
        public decimal GetItemRate(string undrugCode)
        {
            string sql = null;//SQL语句

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.Item.Info.GetItemRate", ref sql) == -1)
            {
                sql = "select EMERG_SCALE from fin_com_undruginfo where item_code='{0}'";
            }

            decimal itemRate = 1.00M;
            //格式化SQL语句
            try
            {
                sql = string.Format(sql, undrugCode);

                if (this.ExecQuery(sql) <= 0)
                {
                    this.WriteErr();
                    return itemRate;
                }

                if (this.Reader != null)
                {
                    if (this.Reader.Read())
                    {
                        itemRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[0]);
                        if (itemRate <= 0)
                        {
                            itemRate = 1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();
                return itemRate;
            }
            finally
            {
                if (this.Reader != null && this.Reader.IsClosed == false)
                {
                    this.Reader.Close();
                }
            }

            return itemRate;
        }

        /// <summary>
        /// 获得有效的,项目类别为手术的项目数组
        /// </summary>
        /// <returns>成功:项目数组 失败返回null</returns>
        public ArrayList QueryOperationItems()
        {
            string strSql = string.Empty;
            string strSql1 = string.Empty;
            if(this.Sql.GetSql("Fee.Item.Main",ref strSql) == -1)
            {
               return null;
            }
            if (this.Sql.GetSql("Fee.ItemInfo.Query.GetOperationItemList", ref strSql1) == -1)
            {
                return null;
            }
            List<Undrug> list = this.QueryUndrugItemsBase(strSql + " " + strSql1);
            if (list != null)
            {
                return new ArrayList(list);
            }
            return null;
        }

        /// <summary>
        /// 获取ICD9手术项目
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryOperationICDItems()
        {
            ArrayList al = new ArrayList();
            string strSql = string.Empty;
            if (this.Sql.GetSql("Fee.ItemInfo.Query.GetOperationICDItemList", ref strSql) == -1)
            {
                return null;
            }
            if (this.ExecQuery(strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Fee.Item.Undrug operationItem = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                    operationItem.UserCode = this.Reader[0].ToString();
                    operationItem.Name = this.Reader[1].ToString();
                    operationItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[2]);
                    operationItem.SpellCode = this.Reader[3].ToString();
                    operationItem.WBCode = this.Reader[4].ToString();
                    operationItem.OperationInfo.ID = this.Reader[5].ToString();
                    operationItem.OperationType = null;
                    operationItem.OperationScale = null;
                    operationItem.ID = this.Reader[8].ToString();
                    al.Add(operationItem);
                }
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return al;

        }

        /// <summary>
        /// 日间手术推荐目录手术项目
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryDayOperationItems()
        {
            ArrayList al = new ArrayList();
            string strSql = string.Empty;
            if (this.Sql.GetSql("Fee.ItemInfo.Query.GetDayOperationItemList", ref strSql) == -1)
            {
                return null;
            }
            if (this.ExecQuery(strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Fee.Item.Undrug operationItem = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                    operationItem.UserCode = this.Reader[0].ToString();
                    operationItem.Name = this.Reader[1].ToString();
                    operationItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[2]);
                    operationItem.SpellCode = this.Reader[3].ToString();
                    operationItem.WBCode = this.Reader[4].ToString();
                    operationItem.OperationInfo.ID = this.Reader[5].ToString();
                    operationItem.OperationType = null;
                    operationItem.OperationScale = null;
                    operationItem.ID = this.Reader[8].ToString();
                    al.Add(operationItem);
                }
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return al;

        }

        /// <summary>
        /// 获得所有可能的项目信息
        /// </summary>
        /// <returns>成功 有效的可用项目信息, 失败 null</returns>
        public ArrayList QueryValidItems()
        {
            return this.Query("all", "1");
        }

        /// <summary>
        /// 根据科室获得所有可能的项目信息
        /// </summary>
        /// <returns>成功 有效的可用项目信息, 失败 null</returns>
        public ArrayList QueryItemsForOrder(string deptCode, bool isAll)
        {
            string sql = string.Empty; //获得全部非药品信息的SELECT语句

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.Item.Info.ForOrder", ref sql) == -1)
            {
                this.Err = "没有找到SQL，索引为【Fee.Item.Info.ForOrder】!";
                this.WriteErr();

                return null;
            }
            //格式化SQL语句
            try
            {
                if (isAll)
                {
                    sql = string.Format(sql, "all", "all", deptCode);
                }
                else
                {
                    sql = string.Format(sql, "all", "1", deptCode);
                }
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return null;
            }

            List<Undrug> list = this.QueryUndrugItemsBase(sql);
            if (list != null)
            {
                return new ArrayList(list);
            }
            return null;
        }

        /// <summary>
        /// 获得所有可能的项目信息
        /// </summary>
        /// <returns>成功 有效的可用项目信息, 失败 null</returns>
        public List<Undrug> QueryValidItemsList()
        {
            return this.QueryList("all", "1");
        }

        /// <summary>
        /// 获得所有可能的项目信息
        /// </summary>
        /// <returns>成功 有效的可用项目信息, 失败 null</returns>
        public List<Undrug> QueryValidItemsListWithIn()
        {
            return this.QueryListWithIn("all", "1");
        }

        // {4F4F0803-78F5-4c7f-9A0A-65474148B9E0} 护士站只收材料费(非药品U)
        /// <summary>
        /// 获得所有可能的项目信息
        /// </summary>
        /// <returns>成功 有效的可用项目信息, 失败 null</returns>
        public List<Undrug> QueryValidItemsListWithInMaterial()
        {
            return this.QueryListWithInMaterial("U", "1");
        }

        /// <summary>
        /// 获得科室所有可能的项目信息
        /// </summary>
        /// <returns>成功 有效的可用项目信息, 失败 null</returns>
        public List<Undrug> QueryValidItemsList(string dept)
        {
            return this.QueryList(dept);
        }

        /// <summary>
        /// 获得所有项目信息
        /// </summary>
        /// <returns>成功 所有项目信息, 失败 null</returns>
        public List<Undrug> QueryAllItemList()
        {
            return this.QueryList("all", "all");
        }

        /// <summary>
        /// 获得全部可用非药品信息和组合项目信息
        /// </summary>
        /// <returns>成功:全部可用非药品信息和组合项目信息 失败: null</returns>
        public ArrayList GetAvailableListWithGroup()
        {
            string sql = null; //获得全部非药品信息的SELECT语句
            ArrayList items = new ArrayList(); //用于返回非药品信息的数组

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.Item.Info.GetAvailableListWithGroup", ref sql) == -1)
            {
                this.Err = "没有找到索引为:Fee.Item.Undrug.Info.GetAvailableListWithGroup的Sql语句!";

                return null;
            }

            //如果执行查询SQL语句,那么返回null
            if (this.ExecQuery(sql) == -1)
            {
                return null;
            }

            try
            {
                //循环获得数据
                while (this.Reader.Read())
                {
                    Undrug item = new Undrug();//临时非药品信息

                    item.ID = this.Reader[0].ToString();
                    item.Name = this.Reader[1].ToString();
                    item.SysClass.ID = this.Reader[2].ToString();
                    item.UserCode = this.Reader[3].ToString();
                    item.SpellCode = this.Reader[4].ToString();
                    item.WBCode = this.Reader[5].ToString();
                    item.Price = NConvert.ToDecimal(this.Reader[6].ToString());
                    item.PriceUnit = this.Reader[7].ToString();
                    item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[8].ToString());
                    item.ExecDept = this.Reader[9].ToString();
                    item.MachineNO = this.Reader[10].ToString();
                    item.CheckBody = this.Reader[11].ToString();
                    item.Memo = this.Reader[12].ToString();
                    item.DiseaseType.ID = this.Reader[13].ToString();
                    item.SpecialDept.ID = this.Reader[14].ToString();
                    item.MedicalRecord = this.Reader[15].ToString();
                    item.CheckRequest = this.Reader[16].ToString();
                    item.Notice = this.Reader[17].ToString();
                    item.Grade = this.Reader[18].ToString();//-- 类别

                    items.Add(item);
                }//循环结束

                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                //如果Reader没有关闭,关闭之
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }

                items = null;

                return null;
            }

            return items;
        }

        /// <summary>
        /// 获得新的非药品编码
        /// </summary>
        /// <returns>新的非药品编码</returns>
        public string GetUndrugCode()
        {
            string tempUndrugCode = null;//临时非药品编码
            string sql = null;//SQL语句

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.Item.UndrugCode", ref sql) == -1)
            {
                this.Err = "获得非药品流水号查询字段Fee.Item.UndrugCode出错!";

                return null;
            }

            tempUndrugCode = this.ExecSqlReturnOne(sql);

            tempUndrugCode = "F" + tempUndrugCode.PadLeft(11, '0');

            return tempUndrugCode;
        }

        /// <summary>
        /// 向非药品字典表(fin_com_undruginfo)中插入一条记录
        /// </summary>
        /// <param name="item">非药品实体</param>
        /// <returns>成功 1 失败 -1</returns>
        public int InsertUndrugItem(Undrug item)
        {
            string sql = null; //插入fin_com_undruginfo的SQL语句

            if (this.Sql.GetCommonSql("Fee.Item.InsertItem", ref sql) == -1)
            {
                this.Err = "获得索引为:Fee.Item.InsertItem的SQL语句失败!";

                return -1;
            }
            //格式化SQL语句
            try
            {
                //取参数列表
                string[] parms = this.GetItemParams(item);
                //替换SQL语句中的参数。
                sql = string.Format(sql, parms);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 更新非药品信息，以非药品编码为主键
        /// </summary>
        /// <param name="item">非药品实体</param>
        /// <returns>成功 1 失败 -1 ,未更新到数据 0</returns>
        public int UpdateUndrugItem(Undrug item)
        {
            string sql = null; //更新fin_com_undruginfo的SQL语句

            if (this.Sql.GetCommonSql("Fee.Item.UpdateItem", ref sql) == -1)
            {
                this.Err = "获得索引为:Fee.Item.UpdateItem的SQL语句失败!";

                return -1;
            }
            //格式化SQL语句
            try
            {
                //取参数列表
                string[] parms = GetItemParams(item);
                //替换SQL语句中的参数。
                sql = string.Format(sql, parms);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 删除非药品信息
        /// </summary>
        /// <param name="undrugCode">非药品编码</param>
        /// <returns>成功 1 失败 -1 未删除到数据 0</returns>
        public int DeleteUndrugItemByCode(string undrugCode)
        {
            string sql = null; //根据非药品编码删除某一非药品信息的DELETE语句

            if (this.Sql.GetCommonSql("Fee.Item.DeleteItem", ref sql) == -1)
            {
                this.Err = "获得索引为:Fee.Item.DeleteItem的SQL语句失败!";

                return -1;
            }
            //格式化SQL语句
            try
            {
                sql = string.Format(sql, undrugCode);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 检索该项目的允许开立科室{2A5608D8-26AD-47d7-82CC-81375722FF72}
        /// </summary>
        /// <param name="undrugCode">非药品编码</param>
        /// <returns> 0</returns>
        public Neusoft.HISFC.Models.Fee.Item.Undrug SelectUndrugDeptListByCode(string undrugCode)
        {
            string sql = "";
            Neusoft.HISFC.Models.Fee.Item.Undrug returnValue = new Undrug();
            if (this.Sql.GetCommonSql("Fee.Item.Info.SelectDeptList", ref sql) == -1)
            {
                this.Err = "获得索引为:Fee.Item.Info.SelectDeptList的SQL语句失败!";

                return null;
            }
            //格式化SQL语句
            try
            {
                sql = string.Format(sql, undrugCode);
                this.ExecQuery(sql);
                while (this.Reader.Read())
                {
                    returnValue.DeptList = this.Reader[0].ToString();
                    returnValue.ItemPriceType = this.Reader[1].ToString();
                    returnValue.IsOrderPrint = this.Reader[2].ToString();
                }
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return null;
            }
            return returnValue;

        }

        /// <summary>
        /// 根据医嘱单打印标志检索项目
        /// {2AFC76CB-3353-4865-AEB4-AFBEE09DD1D7}
        /// </summary>
        /// <returns>返回项目数组</returns>
        public ArrayList SelectAllItemByOrderPrint(string tag)
        {
            string strSql = string.Empty;
            ArrayList list = new ArrayList();
            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = null;
            if (this.Sql.GetCommonSql("Fee.Item.Info.SelectAllItemByOrderPrint.Where", ref strSql) == -1)
            {
                this.Err = "获得索引为:Fee.Item.Info.SelectAllItemByOrderPrint.Where的SQL语句失败!";

                return null;
            }          
            try
            {
                strSql = string.Format(strSql, tag);
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    undrug = new Undrug();
                    undrug.ID = this.Reader[0].ToString();
                    list.Add(undrug);
                }
            }
            catch { }
            finally { Reader.Close(); }
            return list;
        }

        /// <summary>
        /// 非药品调价专用 ，时如果立即生效， 调用这个函数 他只更新非药品的 默认价 ，儿童价， 特诊价
        /// </summary>
        /// <param name="item">价格变化后的非药品实体</param>
        /// <returns>成功 1 失败 -1 未更新到数据 0</returns>
        public int AdjustPrice(Undrug item)
        {
            string sql = null; //调价SQL语句

            if (this.Sql.GetCommonSql("Fee.Item.ItemPriceSave", ref sql) == -1)
            {
                this.Err = "获得索引为:Fee.Item.ItemPriceSave的SQL语句失败!";

                return -1;
            }
            //格式化SQL语句
            try
            {
                //替换SQL语句中的参数。
                sql = string.Format(sql, item.ID, item.Price, item.ChildPrice, item.SpecialPrice);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return -1;
            }

            //{58010499-3CA3-4b9d-B537-BBF964F8EB8B}  根据本次调价项目更新包含了该明细项目的复合项目价格
            if (this.ExecNoQuery(sql) == -1)
            {
                return -1;
            }

            return this.AdjustZTPrice(item);
        }

        /// <summary>
        /// 非药品调价时 根据调价的非药品更新相关的复合项目价格
        /// 
        /// {58010499-3CA3-4b9d-B537-BBF964F8EB8B}  根据本次调价项目更新包含了该明细项目的复合项目价格
        /// </summary>
        /// <param name="adjustPriceItem">价格变化后的非药品实体</param>
        /// <returns>成功1 失败-1 </returns>
        public int AdjustZTPrice(Undrug adjustPriceItem)
        {
            if (adjustPriceItem.UnitFlag == "1")            //复合项目不需要进行后续处理
            {
                return 1;
            }

            List<Neusoft.FrameWork.Models.NeuObject> ztList = this.QueryZTListByDetailItem(adjustPriceItem);
            if (ztList == null)
            {
                return -1;
            }

            foreach (Neusoft.FrameWork.Models.NeuObject ztInfo in ztList)
            {
                if (this.UpdateZTPrice(ztInfo.ID) == -1)
                {
                    return -1;
                }
            }

            return 1;
        }

        /// <summary>
        /// 根据复合项目明细重新计算更新复合项目价格
        /// 
        /// {58010499-3CA3-4b9d-B537-BBF964F8EB8B}  根据本次调价项目更新包含了该明细项目的复合项目价格
        /// </summary>
        /// <param name="undrugZTCode">复合项目编码</param>
        /// <returns>成功返回1 失败返回-1</returns>
        protected int UpdateZTPrice(string undrugZTCode)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Fee.Item.UpdateZTPrice", ref sql) == -1)
            {
                this.Err = "没有找到Fee.Item.UpdateZTPrice字段!";
                this.WriteErr();
                return -1;
            }

            sql = string.Format(sql, undrugZTCode);

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 根据非药品明细项目获取包含了该明细项目的复合项目列表
        /// 
        /// {58010499-3CA3-4b9d-B537-BBF964F8EB8B}  根据本次调价项目更新包含了该明细项目的复合项目价格
        /// </summary>
        /// <param name="detailItem">非药品明细项目</param>
        /// <returns>成功返回1 失败返回-1</returns>
        protected List<Neusoft.FrameWork.Models.NeuObject> QueryZTListByDetailItem(Undrug detailItem)
        {
            string sql = string.Empty; //获得全部变更计划的SELECT语句

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.Item.QueryZTListByDetailItem", ref sql) == -1)
            {
                this.Err = "没有找到Fee.Item.QueryZTListByDetailItem字段!";
                this.WriteErr();

                return null;
            }

            try
            {
                sql = string.Format(sql, detailItem.ID);

                if (this.ExecQuery(sql) == -1)
                {
                    return null;
                }

                List<Neusoft.FrameWork.Models.NeuObject> ztList = new List<Neusoft.FrameWork.Models.NeuObject>();
                while (this.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject tempObj = new Neusoft.FrameWork.Models.NeuObject();

                    tempObj.ID = this.Reader[0].ToString();             //复合项目编码
                    tempObj.Name = this.Reader[1].ToString();           //复合项目名称

                    ztList.Add(tempObj);
                }

                return ztList;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (this.Reader != null && !this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

        #region 执行单管理 单项目维护
        //addby xuewj 2009-8-26 执行单管理 单项目维护 {0BB98097-E0BE-4e8c-A619-8B4BCA715001}

        /// <summary>
        /// 获取不在非药品项目执行单中的非药品项目
        /// </summary>
        /// <param name="nruseID">护士站编码</param>
        /// <param name="sysClass">医嘱类别</param>
        /// <param name="validState">非药品状态: 再用(1) 停用(0) 废弃(2) 所有(all)</param>
        /// <returns></returns>
        public int QueryItemOutExecBill(string nruseID, string sysClass, string validState, ref DataSet ds)
        {
            string sql = string.Empty; //获得全部非药品信息的SELECT语句

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.Item.Info.OutExecBill", ref sql) == -1)
            {
                this.Err = "没有找到Fee.Item.Info字段!";
                this.WriteErr();

                return -1;
            }
            //格式化SQL语句
            try
            {
                sql = string.Format(sql, nruseID, sysClass, validState);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return -1;
            }

            //根据SQL语句取非药品类数组并返回数组
            return this.ExecQuery(sql, ref ds);
        }

        #endregion

        #region zl 非药品科室维护{CA82280B-51B6-4462-B63E-43F4ECF456A3}

        /// <summary>
        /// 根据项目编码，获取执行科室(id='{0}' or id="ALL")
        /// </summary>
        /// <param name="id">项目编码</param>
        /// <returns></returns>
        public ArrayList GetDeptByItemCode(string id)
        {
            ArrayList list = new ArrayList();
            string sql = "";
            if (this.Sql.GetCommonSql("UnDrugDept.Compare.Query", ref sql) == -1)
            {
                this.Err = "没有找到UnDrugDept.Compare.Query";
                return null;
            }

            sql = string.Format(sql, id);

            if (this.ExecQuery(sql) == -1)
            {
                this.Err = "没有找到科室";
                return null;
            }

            try
            {
                Neusoft.HISFC.Models.Fee.Item.Undrug obj = null;
                while (this.Reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                    obj.ID = this.Reader[0].ToString();                //项目ID
                    obj.Name = this.Reader[1].ToString();          //项目名称
                    obj.ExecDept = this.Reader[2].ToString();     //科室ID
                    obj.User01 = this.Reader[3].ToString();        //科室名称                 
                    obj.UserCode = this.Reader[4].ToString();      //SOID
                    obj.Memo = this.Reader[5].ToString();         //默认标记
                    obj.ItemArea = this.Reader[6].ToString();     //适用范围
                    obj.Oper.Oper.ID = this.Reader[7].ToString(); //操作人
                    obj.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[8]);    //操作日期

                    obj.User02 = this.Reader[2].ToString();//原科室ID(用于更新和插入操作)
                    obj.User03 = this.Reader[0].ToString();//原项目ID(用于更新和插入操作)

                    list.Add(obj);
                }
            }
            catch (Exception e)
            {
                this.Err = "查询出错";
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 保存非药品执行科室数据－－先执行更新操作，如果没有找到可以更新的数据，则插入一条新记录
        /// </summary>
        /// <param name="company">非药品实体</param>
        /// <returns>1成功 -1失败</returns>
        public int SetUnDrugCompare(Neusoft.HISFC.Models.Fee.Item.Undrug item)
        {
            int parm;
            //执行更新操作
            parm = UpdateUnDrugCompare(item);

            //如果没有找到可以更新的数据，则插入一条新记录
            if (parm == 0)
            {
                parm = InsertUnDrugCompare(item);
            }
            return parm;
        }

        /// <summary>
        /// 更新非药品执行科室表
        /// </summary>
        /// <param name="company">非药品信息</param>
        /// <returns>0没有更新 1成功 -1失败</returns>
        public int UpdateUnDrugCompare(Neusoft.HISFC.Models.Fee.Item.Undrug item)
        {
            string strSQL = "";
            if (this.Sql.GetCommonSql("UnDrugDept.Compare.Update", ref strSQL) == -1)
            {
                this.Err = "没有找到UnDrugDept.Compare.Update字段!";
                return -1;
            }

            try
            {
                object[] strParm = myGetParmItem(item);  //取参数列表
                strSQL = string.Format(strSQL, strParm);       //替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }

            //执行sql语句
            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 向非药品执行科室表中插入一条记录
        /// </summary>
        /// <param name="company">非药品信息</param>
        /// <returns>0没有更新 1成功 -1失败</returns>
        public int InsertUnDrugCompare(Neusoft.HISFC.Models.Fee.Item.Undrug item)
        {
            string strSQL = "";
            if (this.Sql.GetCommonSql("UnDrugDept.Compare.Insert", ref strSQL) == -1)
            {
                this.Err = "没有找到UnDrugDept.Compare.Insert字段!";
                return -1;
            }

            try
            {
                object[] strParm = myGetParmItem(item);  //取参数列表
                strSQL = string.Format(strSQL, strParm);    //替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSQL);
        }

        public int DeleteUnDrugCompare(Neusoft.HISFC.Models.Fee.Item.Undrug item)
        {
            string strSQL = "";
            if (this.Sql.GetCommonSql("UnDrugDept.Compare.Delete", ref strSQL) == -1)
            {
                this.Err = "没有找到UnDrugDept.Compare.Delete字段!";
                return -1;
            }

            try
            {
                strSQL = string.Format(strSQL, item.User03, item.User02);    //替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                this.Err = "删除时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 获得update或者insert公司表的传入参数数组
        /// </summary>
        /// <param name="company">公司信息</param>
        /// <returns>参数数组</returns>
        private object[] myGetParmItem(Neusoft.HISFC.Models.Fee.Item.Undrug item)
        {

            object[] strParm ={   
								 item.ID,                //新项目ID
                    item.ExecDept ,   //新科室ID
                    item.User01,        //科室名称                 
                    Neusoft.FrameWork.Function.NConvert.ToDecimal( item.UserCode),      //SOID
                    item.Memo,        //默认标记
                    item.ItemArea,     //适用范围
                    item.Oper.Oper.ID, //操作人
                    item.Oper.OperTime.ToString(),    //操作日期
                    item.User02 ,//原科室ID,
                    item.User03 //原项目ID
							 };
            return strParm;
        }

        /// <summary>
        /// 根据门诊住院加载非药品执行科室
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArrayList GetDeptNeuobjByItemID(string id, string type)
        {
            ArrayList list = new ArrayList();
            string sql = "";
            if (this.Sql.GetCommonSql("UnDrugDeptNeuobj.Compare.Query", ref sql) == -1)
            {
                this.Err = "没有找到UnDrugDeptNeuobj.Compare.Query";
                return null;
            }

            sql = string.Format(sql, id, type);

            if (this.ExecQuery(sql) == -1)
            {
                this.Err = "没有找到科室";
                return null;
            }

            try
            {
                Neusoft.FrameWork.Models.NeuObject obj = null;
                while (this.Reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.Reader[0].ToString();                //科室ID
                    obj.Name = this.Reader[1].ToString();          //科室名称
                    obj.User01 = this.Reader[2].ToString();        //soid
                    obj.User02 = this.Reader[3].ToString();        //默认标记
                    obj.User03 = this.Reader[4].ToString();        //适用范围(0 全院 1门诊 2住院)
                    obj.Memo = this.Reader[5].ToString();         //非药品ID

                    list.Add(obj);
                }
            }
            catch (Exception e)
            {
                this.Err = "查询出错";
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return list;
        }      

        #endregion

        #region 非药品多科室组套信息{933F5263-3408-4ccd-A2A6-4C3693A9D10C}

        /// <summary>
        /// 所有有效的组套信息
        /// </summary>
        /// <param name="lstUndrugzt">返回的数据集</param>
        /// <returns>1,成功; -1,失败</returns>
        public int QueryAllValidItemztAllDepts(ref List<Neusoft.HISFC.Models.Fee.Item.Undrug> lstUndrugzt)
        {
            string strsql = "";
            if (this.Sql.GetCommonSql("Fee.Itemzt.Info.AllDepts", ref strsql) == -1)
            {
                return -1;
            }

            //执行当前Sql语句
            if (this.ExecQuery(strsql) == -1)
            {
                this.Err = this.Sql.Err;

                return -1;
            }

            try
            {
                //循环读取数据
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Fee.Item.Undrug item = new Neusoft.HISFC.Models.Fee.Item.Undrug();

                    item.ID = this.Reader[0].ToString();//非药品编码 
                    item.Name = this.Reader[1].ToString(); //非药品名称 
                    item.SysClass.ID = this.Reader[2].ToString(); //系统类别
                    item.MinFee.ID = this.Reader[3].ToString();  //最小费用代码 
                    item.UserCode = this.Reader[4].ToString(); //输入码
                    item.SpellCode = this.Reader[5].ToString(); //拼音码
                    item.WBCode = this.Reader[6].ToString();    //五笔码
                    item.GBCode = this.Reader[7].ToString();    //国家编码
                    item.NationCode = this.Reader[8].ToString();//国际编码
                    item.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[9].ToString()); //三甲价
                    item.PriceUnit = this.Reader[10].ToString();  //计价单位
                    item.FTRate.EMCRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[11].ToString()); // 急诊加成比例
                    item.IsFamilyPlanning = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[12].ToString()); // 计划生育标记 
                    item.User01 = this.Reader[13].ToString(); //特定诊疗项目
                    item.Grade = this.Reader[14].ToString();//甲乙类标志
                    item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[15].ToString());//确认标志 1 需要确认 0 不需要确认
                    item.ValidState = this.Reader[16].ToString(); //有效性标识 在用 1 停用 0 废弃 2   
                    item.Specs = this.Reader[17].ToString(); //规格
                    item.ExecDept = this.Reader[18].ToString();//执行科室
                    item.MachineNO = this.Reader[19].ToString(); //设备编号 用 | 区分 
                    item.CheckBody = this.Reader[20].ToString(); //默认检查部位或标本
                    item.OperationInfo.ID = this.Reader[21].ToString(); // 手术编码 
                    item.OperationType.ID = this.Reader[22].ToString(); // 手术分类
                    item.OperationScale.ID = this.Reader[23].ToString(); //手术规模 
                    item.IsCompareToMaterial = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[24].ToString());//是否有物资项目与之对照(1有，0没有) 
                    item.Memo = this.Reader[25].ToString(); //备注  
                    item.ChildPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString()); //儿童价
                    item.SpecialPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString()); //特诊价
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
                    item.IsConsent = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[40].ToString());
                    item.CheckApplyDept = this.Reader[41].ToString();//检查申请单名称
                    item.IsNeedBespeak = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[42].ToString());//是否需要预约
                    item.ItemArea = this.Reader[43].ToString();//项目范围
                    item.ItemException = this.Reader[44].ToString();//项目约束

                    //单位标识(0,明细; 1,组套)[2007/01/01  xuweizhe]
                    item.UnitFlag = this.Reader.IsDBNull(45) ? "" : this.Reader.GetString(45);

                    item.SpecialDept.ID = this.Reader.IsDBNull(46) ? "" : this.Reader[46].ToString();//非药品多科室维护的科室

                    lstUndrugzt.Add(item);
                }//循环结束

                //关闭Reader
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.Reader.Close();
                return -1;
            }
            return 1;
        }

        #endregion

        #endregion
    }
}
