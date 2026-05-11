using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 计划取标本管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-21]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-19' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecSourcePlanManage : Neusoft.FrameWork.Management.Database
    {       
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specSource">计划标本对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecSourcePlan specPlan)
        {
            //string sequence = "";
            //GetNextSequence(ref sequence);
            string[] str = new string[]
						{
							specPlan.PlanID.ToString(), 
							specPlan.SpecID.ToString(),
                            specPlan.SpecType.SpecTypeID.ToString(),
                            specPlan.Count.ToString(),
                            specPlan.Capacity.ToString(),
                            specPlan.Unit,
                            specPlan.ForSelfUse.ToString(),
                            specPlan.TumorType.ToString(),
                            specPlan.LimitUse.ToString(),
                            specPlan.TumorPos,
                            specPlan.SideFrom,
                            specPlan.TumorPor,
                            specPlan.BaoMoEntire,
                            specPlan.BreakPiece.ToString(),
                            specPlan.StoreTime.ToString(),
                            specPlan.StoreCount.ToString(),
                            specPlan.Comment,
                            specPlan.TransPos,
                            specPlan.Ext1,
                            specPlan.Ext2,
                            specPlan.Ext3
						};
            return str;
        }

        #region 设置错误信息
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="errorCode">错误代码发生行数</param>
        /// <param name="errorText">错误信息</param>
        private void SetError(string errorCode, string errorText)
        {
            this.ErrCode = errorCode;
            this.Err = errorText + "[" + this.Err + "]"; // + "在TerminalConfirm.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新SpecSourcePlan操作

        private SpecSourcePlan SetSpecSource()
        {
            SpecSourcePlan specSourcePlan = new SpecSourcePlan();
            try
            {
                specSourcePlan.PlanID = Convert.ToInt32(this.Reader["SOTREID"].ToString());
                specSourcePlan.SpecID = Convert.ToInt32(this.Reader["SPECID"].ToString());
                if (!Reader.IsDBNull(2)) specSourcePlan.SpecType.SpecTypeID = Convert.ToInt32(this.Reader["SPECTYPEID"].ToString());//2
                else specSourcePlan.SpecType.SpecTypeID = 0;
                if (!Reader.IsDBNull(3)) specSourcePlan.Count = Convert.ToInt32(this.Reader["SUBCOUNT"].ToString());//3
                else specSourcePlan.Count = 0;
                if (!Reader.IsDBNull(4)) specSourcePlan.Capacity = Convert.ToDecimal(this.Reader["CAPACITY"].ToString());//4
                else specSourcePlan.Capacity = 0;
                if (!Reader.IsDBNull(5)) specSourcePlan.Unit = this.Reader["UNIT"].ToString();//5
                else specSourcePlan.Unit = "支";
                if (!Reader.IsDBNull(6)) specSourcePlan.ForSelfUse = Convert.ToInt32(this.Reader["FORSELFUSE"].ToString());//6
                else specSourcePlan.ForSelfUse = 0;
                if (!Reader.IsDBNull(7)) specSourcePlan.TumorType = this.Reader["TUMORTYPE"].ToString();//7
                else specSourcePlan.TumorType = "";
                if (!Reader.IsDBNull(8)) specSourcePlan.LimitUse = this.Reader["LIMITUSE"].ToString();//8
                else specSourcePlan.LimitUse = "";
                if (!Reader.IsDBNull(9)) specSourcePlan.TumorPos = this.Reader["TUMORPOS"].ToString();//9
                else specSourcePlan.TumorPos = "";
                if (!Reader.IsDBNull(10)) specSourcePlan.SideFrom = this.Reader["SIDEFROM"].ToString();//10
                else specSourcePlan.SideFrom = "";
                if (!Reader.IsDBNull(11)) specSourcePlan.TumorPor = this.Reader["TUMORPOR"].ToString();//11
                else specSourcePlan.TumorPor = "";
                if (!Reader.IsDBNull(12)) specSourcePlan.BaoMoEntire = this.Reader["BAOMOENTIRE"].ToString();//12
                else specSourcePlan.BaoMoEntire ="";
                if (!Reader.IsDBNull(13)) specSourcePlan.BreakPiece = this.Reader["BREAKPIECE"].ToString();//13
                else specSourcePlan.BreakPiece = "";
                if (!Reader.IsDBNull(14)) specSourcePlan.StoreTime = Convert.ToDateTime(this.Reader["STORETIME"].ToString());//14
                else specSourcePlan.StoreTime = DateTime.MinValue;
                if (!Reader.IsDBNull(15)) specSourcePlan.StoreCount = Convert.ToInt32(this.Reader["SOTRECOUNT"].ToString());//15
                else specSourcePlan.StoreCount = 0;
                if (!Reader.IsDBNull(16)) specSourcePlan.Comment = this.Reader["MARK"].ToString();//16
                else specSourcePlan.Comment = "";
                if (!Reader.IsDBNull(17)) specSourcePlan.TransPos = this.Reader["TRANSPOS"].ToString();//16
                else specSourcePlan.TransPos = "";
                if (!Reader.IsDBNull(18)) specSourcePlan.Ext1 = this.Reader["EXT1"].ToString();//16
                else specSourcePlan.Ext1 = "";
                if (!Reader.IsDBNull(19)) specSourcePlan.Ext2 = this.Reader["EXT2"].ToString();//16
                else specSourcePlan.Ext2 = "";
                if (!Reader.IsDBNull(20)) specSourcePlan.Ext3 = this.Reader["EXT3"].ToString();//16
                else specSourcePlan.Ext3 = "";

            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return specSourcePlan;

        }

        /// <summary>
        /// 更新SpecSourcePlan
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSpecSource(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;//Update语句

            //获得Where语句
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return -1;
            }

            return this.ExecNoQuery(sql, args);
        }

        /// <summary>
        /// 根据sql索引查找符合条件的原标本
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns>原标本列表</returns>
        private ArrayList GetSpecSourcePlan(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrSpecSourcePlan = new ArrayList();
            while (this.Reader.Read())
            {
                SpecSourcePlan specTmpPlan = SetSpecSource();
                arrSpecSourcePlan.Add(specTmpPlan);
            }
            Reader.Close();
            return arrSpecSourcePlan;
        }
        #endregion

        #endregion

        #region 公共方法
        
        /// <summary>
        /// SpecSourcePlan插入
        /// </summary>
        /// <param name="specBox">即将插入的SpecSourcePlan</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertSourcePlan(Neusoft.HISFC.Models.Speciment.SpecSourcePlan specSourcePlan)
        {
            return this.UpdateSpecSource("Speciment.BizLogic.SpecSourcePlanManage.Insert", this.GetParam(specSourcePlan));
        }

        public int UpdateSourcePlan(string sql)
        {
            return this.ExecNoQuery(sql);
        }

        public ArrayList GetSourcePlan(string sql)
        {
            if (this.ExecQuery(sql) == -1)
                return null;
            ArrayList arrSourcePlan = new ArrayList();
            while (this.Reader.Read())
            {
                SpecSourcePlan s = SetSpecSource();
                arrSourcePlan.Add(s);
            }
            this.Reader.Close();
            return arrSourcePlan;
        }

        /// <summary>
        /// 根据Storeid
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public SpecSourcePlan GetPlanById(string storeId, string subSpecId )
        {
            ArrayList arr = new ArrayList();
            if (storeId != "")
            {
                arr = this.GetSpecSourcePlan("Speciment.BizLogic.SpecSourcePlanManage.QueryStoreById", new string[] { storeId });
            }
            if (subSpecId != "")
            {
                arr = this.GetSpecSourcePlan("Speciment.BizLogic.SpecSourcePlanManage.QueryStoreBySpecid", new string[] { subSpecId });
            }
            if (arr != null && arr.Count > 0)
            {
                return arr[0] as SpecSourcePlan;
            }
            else
            {
                return null;
            }
        }

       /// <summary>
       /// 根据分装条码号获取
       /// </summary>
       /// <param name="storeId"></param>
       /// <param name="subSpecId"></param>
       /// <returns></returns>
        public SpecSourcePlan GetPlanBySubBarCode(string subBarCode)
        {
            ArrayList arr = new ArrayList();

            arr = this.GetSpecSourcePlan("Speciment.BizLogic.SpecSourcePlanManage.QueryStoreBySubSpecBarCode", new string[] { subBarCode });

            try
            {
                return arr[0] as SpecSourcePlan;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 合并ArrayList中具有相同的属性设置的标本
        /// </summary>
        /// <param name="arrSpecPlan"></param>
        /// <returns></returns>
        public Dictionary<SpecSourcePlan, List<StoreInfo>> ParseSourcePlan(ArrayList arrSpecPlan)
        {
            Dictionary<SpecSourcePlan, List<StoreInfo>> dicMergeResult = new Dictionary<SpecSourcePlan, List<StoreInfo>>();
            List<KeyValuePair<SpecSourcePlan, List<StoreInfo>>> listTmp = new List<KeyValuePair<SpecSourcePlan, List<StoreInfo>>>();
          
            int i = 0;
            foreach (SpecSourcePlan p in arrSpecPlan)
            {
                List<StoreInfo> tmpStore = new List<StoreInfo>();                
                StoreInfo store = new StoreInfo();
                store.Count = p.Count;
                store.LimitUse = p.LimitUse;
                store.SpecTypeId = p.SpecType.SpecTypeID;
                store.StoreId = p.PlanID;
                if (i == 0)
                {
                    tmpStore.Add(store);
                    KeyValuePair<SpecSourcePlan, List<StoreInfo>> tmpPair = new KeyValuePair<SpecSourcePlan, List<StoreInfo>>(p,tmpStore);
                    listTmp.Add(tmpPair);
                    i++;
                    continue;
                }
                for (int k = listTmp.Count - 1; k >= -1; k--)
                {
                    if (k == -1)
                    {
                        tmpStore.Add(store);
                        KeyValuePair<SpecSourcePlan, List<StoreInfo>> tmpPair = new KeyValuePair<SpecSourcePlan, List<StoreInfo>>(p, tmpStore);
                        listTmp.Add(tmpPair);
                        break;
                    }
                    KeyValuePair<SpecSourcePlan, List<StoreInfo>> item = listTmp[k];
                    if (p.ChkFromSamePro(item.Key))
                    {
                        if (item.Value.Contains(store))
                            break;
                        else
                        {
                            item.Value.Add(store);
                            break;
                        }
                    }
                    //if (!p.ChkFromSamePro(item.Key))
                    //{
                    //    tmpStore.Add(store);
                    //    KeyValuePair<SpecSourcePlan, List<StoreInfo>> tmpPair = new KeyValuePair<SpecSourcePlan, List<StoreInfo>>(p, tmpStore);
                    //    listTmp.Add(tmpPair);
                    //    break;
                    //}
                }
                i++;
            }
            foreach (KeyValuePair<SpecSourcePlan, List<StoreInfo>> tmp in listTmp)
            {
                dicMergeResult.Add(tmp.Key,tmp.Value);
            }
            return dicMergeResult; 
        }

        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        public string GetNextSequence()
        {
            //
            // 执行SQL语句
            //
            return this.GetSequence("Speciment.BizLogic.SpecSourcePlanManage.GetNextSequence");
        }

        public int UpdateSpecPlan(SpecSourcePlan sp)
        {
            return this.UpdateSpecSource("Speciment.BizLogic.SpecSourcePlanManage.Update", GetParam(sp));
        }

        #endregion
    }
    public class StoreInfo
    {
        private int specTypeId = 0;
        private string specTypeName = "";
        private int count = 0;
        private string limitUse = "";
        private int storeId = 0;

        public int SpecTypeId
        {
            get
            {
                return specTypeId;
            }
            set
            {
                specTypeId = value;
            }
        }

        public string SpecTypeName
        {
            get
            {
                return specTypeName;
            }
            set
            {
                specTypeName = value;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        public string LimitUse
        {
            get
            {
                return limitUse;
            }
            set
            {
                limitUse = value;
            }
        }

        public int StoreId
        {
            get
            {
                return storeId;
            }
            set
            {
                storeId = value;
            }
        }
    }
    
}
