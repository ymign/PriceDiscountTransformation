using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 分装标本管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-11-18]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-19' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SubSpecManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中

        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="subSpec">分装标本</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SubSpec subSpec)
        {
            //string sequence = "";
            
            //GetNextSequence(ref sequence);
            string[] str = new string[]
						{
							subSpec.SubSpecId.ToString(), 
                            subSpec.SubBarCode,
                            subSpec.SpecId.ToString(),//原标本ID
                            subSpec.StoreTime.ToString(),
                            subSpec.SpecCount.ToString(),//
                            subSpec.SpecCap.ToString(),
                            subSpec.IsCancer.ToString(),
                            subSpec.SpecTypeId.ToString(),
                            subSpec.LastReturnTime.ToString(),
                            subSpec.DateReturnTime.ToString(),
                            subSpec.Status,
                            subSpec.IsReturnable.ToString(),
                            subSpec.BoxId.ToString(),
                            subSpec.BoxStartRow.ToString(),
                            subSpec.BoxStartCol.ToString(),
                            subSpec.BoxEndRow.ToString(),
                            subSpec.BoxEndCol.ToString(),
                            subSpec.InStore,
                            subSpec.Comment,
                            subSpec.StoreID.ToString()
						};
            return str;
        }

        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        public int GetNextSequence(ref string sequence)
        {
            //
            // 执行SQL语句
            //
            sequence = this.GetSequence("Speciment.BizLogic.SubSpecManage.GetNextSequence");
            //
            // 如果返回NULL，则获取失败
            //
            if (sequence == null)
            {
                this.SetError("", "获取Sequence失败");
                return -1;
            }
            //
            // 成功返回
            //
            return 1;
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
            this.Err = errorText + "[" + this.Err + "]"; // + "在ShelfSpecManage.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新标本组织类型操作
        /// <summary>
        /// 更新分装标本信息
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSubSpec(string sqlIndex, params string[] args)
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
        /// 读取标本信息
        /// </summary>
        /// <returns>组织类型实体</returns>
        private SubSpec SetSubSpec()
        {
            SubSpec subSpec = new SubSpec();
            try
            {
                subSpec.SubSpecId = Convert.ToInt32(this.Reader["SUBSPECID"].ToString());
                subSpec.BoxId = Convert.ToInt32(this.Reader["BOXID"]);
                subSpec.SubBarCode = this.Reader["SUBBARCODE"].ToString();
                subSpec.SpecId = Convert.ToInt32(this.Reader["SPECID"].ToString());
                subSpec.StoreTime = Convert.ToDateTime(this.Reader["STORETIME"].ToString());
                subSpec.SpecCount = Convert.ToInt32(this.Reader["SPECCOUNT"].ToString());
                subSpec.SpecCap = Convert.ToDecimal(this.Reader["SPECCAP"].ToString());
                subSpec.IsCancer = Convert.ToChar(this.Reader["ISCANCER"].ToString());
                subSpec.SpecTypeId = Convert.ToInt32(this.Reader["SPECTYPEID"].ToString());
                subSpec.LastReturnTime = Convert.ToDateTime(this.Reader["LASTRETURNTIME"].ToString());
                subSpec.DateReturnTime = Convert.ToDateTime(this.Reader["DATERETURNTIME"].ToString());
                subSpec.Status = this.Reader["STATUS"].ToString();
                subSpec.IsReturnable = Convert.ToChar(this.Reader["ISRETURNABLE"].ToString());        
                subSpec.BoxStartRow =  Convert.ToInt32 (this.Reader["BOXSTARTROW"].ToString());
                subSpec.BoxStartCol =  Convert.ToInt32 (this.Reader["BOXSTARTCOL"].ToString());
                subSpec.BoxEndRow =  Convert.ToInt32 (this.Reader["BOXENDROW"].ToString());
                subSpec.BoxEndCol =  Convert.ToInt32 (this.Reader["BOXENDCOL"].ToString());
                subSpec.InStore = this.Reader["INSTORE"].ToString();
                subSpec.Comment = this.Reader["MARK"].ToString();
                subSpec.StoreID = Convert.ToInt32(this.Reader["STOREID"].ToString());
             }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return subSpec;
        }

        /// <summary>
        /// 根据索引获取符合条件的分装标本列表
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList GetSubSpec(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrSubSpec = new ArrayList();
            while (this.Reader.Read())
            {
                SubSpec subTmp = SetSubSpec();
                arrSubSpec.Add(subTmp);
            }
            this.Reader.Close();
            return arrSubSpec;
        }

        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 分装标本插入
        /// </summary>
        /// <param name="specBox">即将标本</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertSubSpec(Neusoft.HISFC.Models.Speciment.SubSpec subSpec)
        {
            return this.UpdateSubSpec("Speciment.BizLogic.SubSpecManage.Insert", this.GetParam(subSpec));

        }

        public int UpdateSubSpec(SubSpec subSpec)
        {
            return this.UpdateSubSpec("Speciment.BizLogic.SubSpecManage.Update", this.GetParam(subSpec));
        }

        /// <summary>
        /// 查找在指定盒子中最后的标本
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public SubSpec GetLastSpecInBox(string boxId)
        {
            string[] parms = new string[] { boxId };
            ArrayList arrSubSpec= new ArrayList();
            SubSpec subSpec = new SubSpec();
            arrSubSpec = this.GetSubSpec("Speciment.BizLogic.SubSpecManage.SubLocate", parms);
            //如果arrSubSpec.Count<0 说明盒子是空的，那么初始化第一个位置
            if (arrSubSpec.Count <= 0)
            {
                subSpec.BoxStartCol = 0;
                subSpec.BoxStartRow = 1;
                subSpec.BoxEndCol = 0;
                subSpec.BoxEndRow = 1;
            }
            foreach (SubSpec s in arrSubSpec)
            {
                if (s == null)
                {
                    continue;
                }
                subSpec = s;
                break;
            }
            return subSpec; 
        }

        /// <summary>
        /// 常温柜返回最后一个位置
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public SubSpec GetLastSpecForTmp(string boxId)
        {

            string[] parms = new string[] { boxId };
            ArrayList arrSubSpec = new ArrayList();
            SubSpec subSpec = new SubSpec();
            arrSubSpec = this.GetSubSpec("Speciment.BizLogic.SubSpecManage.SubLocateTmp", parms);
            //如果arrSubSpec.Count<0 说明盒子是空的，那么初始化第一个位置
            if (arrSubSpec.Count <= 0)
            {
                subSpec.BoxStartCol = 1;
                subSpec.BoxStartRow = 0;
                subSpec.BoxEndCol = 1;
                subSpec.BoxEndRow = 0;
            }
            foreach (SubSpec s in arrSubSpec)
            {
                subSpec = s;
                break;
            }
            return subSpec; 
        }

        /// <summary>
        /// 根据标本盒的ID,及规格查找空位
        /// </summary>
        /// <param name="boxId">标本盒ID</param>
        /// <param name="boxSpec">标本盒规格</param>
        /// <returns></returns>
        public SubSpec ScanSpecBox(string boxId, BoxSpec boxSpec)
        {
            ArrayList arrSubSpecInBox = new ArrayList();
            arrSubSpecInBox = this.GetSubSpec("Speciment.BizLogic.SubSpecManage.InOneBox", new string[] { boxId });
            SubSpec current = new SubSpec();
            SubSpec next = new SubSpec();            

            //如果标本盒 中无标本
            if (arrSubSpecInBox.Count == 0)
            {
                SubSpec firstSubSpec = new SubSpec();
                firstSubSpec.BoxStartRow = 1;
                firstSubSpec.BoxEndRow = 1;
                return firstSubSpec;
            }           
            //如果第一行第一列没有标本
            if (arrSubSpecInBox.Count >= 1)
            {
                current = arrSubSpecInBox[0] as SubSpec;
                if (current.BoxStartCol > 1 || (current.BoxStartCol == 1 && current.BoxStartRow > 1))
                {
                    SubSpec firstSubSpec = new SubSpec();
                    firstSubSpec.BoxStartRow = 1;
                    firstSubSpec.BoxEndRow = 1;
                    return firstSubSpec;
                }               
            }
            //如果只有1条记录
            if (arrSubSpecInBox.Count == 1)
            {
                return current;
            }
            for (int i = 0; i < arrSubSpecInBox.Count-1; i++)
            {
                current = arrSubSpecInBox[i] as SubSpec;
                next = arrSubSpecInBox[i + 1] as SubSpec;
                int curCol = current.BoxEndCol;
                int curRow = current.BoxEndRow;
                int nextCol = next.BoxEndCol;
                int nextRow = next.BoxEndRow;
                //如果２个标本放在同一行，看看这２个标本是否紧挨，如果非紧挨，返回当前的标本
                if (curRow == nextRow)
                {
                    if ((nextCol - curCol) == 1)
                    {
                        continue;
                    }
                    if ((nextCol - curCol >= 2))
                    {
                        return current;
                    }
                }
                else
                {
                    //如果２个标本的行是挨着的
                    if ((nextRow - curRow) == 1)
                    {
                        //如果ｃｕｒｒｅｎｔ是最后一列，ｎｅｘｔ是第一列，则认为2个标本时挨着的
                        if (curCol == boxSpec.Col && nextCol == 1)
                        {
                            continue;
                        }
                        else
                        {
                            return current;
                        }
                    }
                    else
                    {
                        return current;
                    }
                }
            }
            //遍历完盒子,发现都是挨着的,返回最后一个标本
            return next;
 
        }

        /// <summary>
        /// 根据标本盒的ID查找空位数
        /// </summary>
        /// <param name="boxId">标本盒ID</param>
        /// <param name="vpCount">返回空位数</param>
        /// <returns>1成功 -1失败</returns>
        public int ScanSpecBox(string boxId, out int vpCount)
        {
            vpCount = 0;
            string strSQL = "";
            //取SELECT语句
            try
            {
                if (this.Sql.GetSql("Speciment.BizLogic.SubSpecManage.VPCount", ref strSQL) == -1)
                {
                    this.Err = "没有找到Speciment.BizLogic.SubSpecManage.VPCount字段!";
                    return -1;
                }
                strSQL = string.Format(strSQL, boxId);
                //取空位总数量
                if (this.ExecQuery(strSQL) == -1)
                {
                    this.Err = "执行取标本盒空位总数量SQL语句时出错：" + this.Err;
                    return -1;
                }

                if (this.Reader.Read())
                {
                    try
                    {
                        vpCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[0].ToString());  //标本盒空位总数量
                    }
                    catch (Exception ex)
                    {
                        this.Err = "取标本盒空位总数量时出错！" + ex.Message;
                        return -1;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                this.Err = "执行Sql语句 获取标本盒空位总数量发生错误" + ex.Message;
                return -1;
            }
            finally
            {
                this.Reader.Close();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int UpdateSubSpec(string sql)
        {
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 根据标本号或者条形码获取标本信息
        /// </summary>
        /// <param name="subSpecId"></param>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public SubSpec GetSubSpecById(string subSpecId,string barCode)
        {
            SubSpec subSpec = new SubSpec();
            ArrayList arrSubSpec = new ArrayList();
            if (subSpecId != "")
            {
                arrSubSpec = this.GetSubSpec("Speciment.BizLogic.SubSpecManage.ById", new string[] { subSpecId });
                foreach (SubSpec sub in arrSubSpec)
                {
                    subSpec = sub;
                    break;
                } 
            }
            if (barCode != "")
            {
                arrSubSpec = this.GetSubSpec("Speciment.BizLogic.SubSpecManage.ByBarCode", new string[] { barCode });
                foreach (SubSpec sub in arrSubSpec)
                {
                    subSpec = sub;
                    break;
                } 
            }
            return subSpec;
        }

        /// <summary>
        /// 查找同一个盒子中所有的标本
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public ArrayList GetSubSpecInOneBox(string boxId)
        {
            return this.GetSubSpec("Speciment.BizLogic.SubSpecManage.InOneBox", new string[] { boxId });
        }

        /// <summary>
        /// 查看层中 或架子中是否存在标本
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="cap"></param>
        /// <returns></returns>
        public ArrayList GetSubSpecInLayerOrShelf(string capId, string cap)
        {
            //冰箱层中放标本盒
            if (cap == "B")
            {
                return this.GetSubSpec("Speciment.BizLogic.SubSpecManage.GetInLayer", new string[] { capId });
            }
            //冰箱层中放架子
            if (cap == "J")
            {
                return this.GetSubSpec("Speciment.BizLogic.SubSpecManage.GetInShelf", new string[] { capId });
            }
            //单独的架子
            if (cap == "IJ")
            {
                return this.GetSubSpec("Speciment.BizLogic.SubSpecManage.GetInShelf1", new string[] { capId });
            }
            return null;
        }

        /// <summary>
        /// 根据原标本Id查询分装标本列表
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        public ArrayList GetSubSpecBySpecId(string specId)
        {
            return this.GetSubSpec("Speciment.BizLogic.SubSpecManage.GetBySpecID", new string[] { specId });
        }

        /// <summary>
        /// 根据StoreId查询分装获取分装标本
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public ArrayList GetSubSpecByStoreId(string storeId)
        {
            return this.GetSubSpec("Speciment.BizLogic.SubSpecManage.GetByStoreID", new string[] { storeId });
        }

        /// <summary>
        /// 在指定的位置查询标本
        /// </summary>
        /// <param name="boxId">标本盒ID</param>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <returns></returns>
        public SubSpec GetSubSpecByLocate(string boxId, string row, string col)
        {
            ArrayList arr = this.GetSubSpec("Speciment.BizLogic.SubSpecManage.Locate", new string[] { boxId, row, col });
            if (arr == null || arr.Count <= 0)
            {
                return null;
            }
            return arr[0] as SubSpec;
        }

        /// <summary>
        /// 在指定的位置查询标本
        /// </summary>
        /// <param name="boxId">标本盒ID</param>
        /// <param name="endCol">止行</param>
        /// <param name="endRow">止列</param>
        /// <param name="startCol">起列</param>
        /// <param name="startRow">起行</param>
        /// <returns></returns>
        public SubSpec GetSubSpecByLocate(string boxId, string endCol, string endRow, string startCol, string startRow)
        {
            ArrayList arr = this.GetSubSpec("Speciment.BizLogic.SubSpecManage.LocateDetail", new string[] { boxId, endCol, endRow, startCol, startRow });
            if (arr == null || arr.Count <= 0)
            {
                return null;
            }
            return arr[0] as SubSpec;
        }

        /// <summary>
        /// 根据sql获取符合条件的分装标本列表
        /// </summary>
        /// <param name="sql">sql语句</param> 
        /// <returns></returns>
        public ArrayList GetSubSpec(string sql)
        {
            if (this.ExecQuery(sql, new string[] { }) == -1)
                return null;
            ArrayList arrSubSpec = new ArrayList();
            while (this.Reader.Read())
            {
                SubSpec subTmp = SetSubSpec();
                arrSubSpec.Add(subTmp);
            }
            this.Reader.Close();
            return arrSubSpec;
        }

        /// <summary>
        /// 获取标本的专用属性
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetUseAlonePro(string storeId)
        {
            string sql = @"select ss.SUBBARCODE,ss.USER from SPEC_SUBSPEC ss where ss.STOREID = {0}";
            sql = string.Format(sql, storeId);
            DataSet ds = new DataSet();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                this.ExecQuery(sql, ref ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr[0] != null && dr[0].ToString() != "")
                    {
                        if (dr[1] != null && dr[1].ToString() != "")
                        {
                            dic.Add(dr[0].ToString(), dr[1].ToString());
                        }
                        else
                        {
                            dic.Add(dr[0].ToString(), "");
                        }
                    }
                }
            }
            catch
            { }
            finally
            {
                //this.Reader.Close();
            }
            return dic;
        }

        /// <summary>
        /// 更新标本专用属性
        /// </summary>
        /// <param name="useAlonePro"></param>
        /// <returns></returns>
        public int UpdateUseAlonePro(Dictionary<string, string> useAlonePro)
        {
            string sql = @" update SPEC_SUBSPEC set USEALONE = '{0}',USER='{1}' where SUBBARCODE= '{2}'";

            try
            {
                foreach (KeyValuePair<string, string> vp in useAlonePro)
                {
                    if (vp.Value == "")
                    {
                        sql = string.Format(sql, "0", "", vp.Key);
                    }
                    else
                    {
                        sql = string.Format(sql, "1", vp.Value, vp.Key);
                    }

                    if (this.ExecNoQuery(sql) < 0)
                    {
                        return -1;
                    }
                }
            }
            catch
            {
                return -1;
            }
            return 1;

        }
        #endregion
    }
}
