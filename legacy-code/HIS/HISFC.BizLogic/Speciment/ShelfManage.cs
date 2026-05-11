using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 冻存架管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-15]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-18' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class ShelfManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">冻存架对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.Shelf shelf)
        {
            string[] str = new string[]
						{
							shelf.ShelfID.ToString(),
                            shelf.IceBoxLayer.Row.ToString(),
                            shelf.IceBoxLayer.Col.ToString(),
                            shelf.IceBoxLayer.Height.ToString(),
                            shelf.IceBoxLayer.LayerId.ToString(),
                            shelf.SpecBarCode.ToString(),
                            shelf.ShelfSpec.ShelfSpecID.ToString(),
                            shelf.Capacity.ToString(),
                            shelf.OccupyCount.ToString(),
                            shelf.IsOccupy.ToString(),
                            shelf.DisTypeId.ToString(),
                            shelf.SpecTypeId.ToString(),
                            shelf.TumorType,
                            shelf.Comment
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
            sequence = this.GetSequence("Speciment.BizLogic.ShelfManage.GetNextSequence");
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

        #region 更新架子规格操作
        /// <summary>
        /// 更新架子库位信息
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateShelf(string sqlIndex, params string[] args)
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
        #endregion

        private Shelf SetShelf()
        {
            Shelf shelf = new Shelf();
            try
            {
                //在层的第几列
                shelf.ShelfID = Convert.ToInt32(this.Reader["SHELFID"].ToString());                
                //shelf.IceBoxLayer.Col = Convert.ToInt32(this.Reader[""].ToString());
                shelf.IceBoxLayer.LayerId = Convert.ToInt32(this.Reader["ICEBOXLAYERID"].ToString());
                shelf.SpecBarCode = this.Reader["SHELFBARCODE"].ToString();
                shelf.ShelfSpec.ShelfSpecID = Convert.ToInt32(this.Reader["SHELFSPECID"].ToString());
                if (Reader["CAPACITY"] != null)
                {
                    shelf.Capacity = Convert.ToInt32(this.Reader["CAPACITY"].ToString());
                }
                else
                {
                    shelf.Capacity = 0;
                }
                if (Reader["OCCUPYCOUNT"] != null)
                {
                    shelf.OccupyCount = Convert.ToInt32(this.Reader["OCCUPYCOUNT"].ToString());
                }
                else
                {
                    shelf.OccupyCount = 0;
                }
                if (Reader["ISOCCUPY"] != null)
                {
                    shelf.IsOccupy = Convert.ToChar(this.Reader["ISOCCUPY"].ToString());
                }
                else
                    shelf.IsOccupy = '0';
                shelf.Comment = this.Reader["MARK"].ToString();
                shelf.DisTypeId = Convert.ToInt32(this.Reader["DISTYPEID"].ToString());
                shelf.SpecTypeId = Convert.ToInt32(this.Reader["SPECTYPEID"].ToString());
                shelf.TumorType = this.Reader["TUMORTYPE"].ToString();
                shelf.IceBoxLayer.Col = Convert.ToInt32(this.Reader["SHELFCOL"].ToString());
                shelf.IceBoxLayer.Row = Convert.ToInt32(this.Reader["SHELFROW"].ToString()) ;
                shelf.IceBoxLayer.Height = Convert.ToInt32(this.Reader["SPECLAYER"].ToString());

            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return shelf;
        }

        /// <summary>
        /// 根据索引查找出符合条件的ShelfList
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList SelectShelf(string sqlIndex, string[] parm)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            Shelf shelf;
            ArrayList arrList = new ArrayList();
            while (this.Reader.Read())
            {
                shelf = new Shelf();
                shelf = SetShelf();
                arrList.Add(shelf);
            }
            this.Reader.Close();
            return arrList;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 冻存架插入
        /// </summary>
        /// <param name="specBox">即将插入的架</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertShelf(Neusoft.HISFC.Models.Speciment.Shelf shelf)
        {
            return this.UpdateShelf("Speciment.BizLogic.ShelfManage.Insert", this.GetParam(shelf));

        }

        /// <summary>
        ///  病种ID,和标本盒规格ID
        /// </summary>
        /// <param name="disTypeId">病种Id</param>
        /// <param name="specId">标本盒规格Id</param>
        /// <param name="specClassId">标本种类ID</param>
        /// <param name="specTypeId">标本类型ID</param>
        /// <param name="shelfId">标本架的ID！=shelfId</param>
        /// <returns>冷冻架</returns>
        
        public Shelf GetShelf(string disTypeId, string specId, string specTypeId, string shelfId)
        {
            string[] parm = new string[] { disTypeId, specId, specTypeId, shelfId };
            ArrayList arrShelf = this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfBySpecBox", parm);
            if (arrShelf.Count >= 1)
            {
                return arrShelf[0] as Shelf;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查找按照病种，规格，标本类型查找架子 并且架子所在层的Id不在layerId中，用于移库
        /// </summary>
        /// <param name="disTypeId"></param>
        /// <param name="specId"></param>
        /// <param name="specTypeId"></param>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public ArrayList GetShelfNotInOneLayer(string disTypeId, string specId, string specTypeId, string layerId)
        {
            string[] parm = new string[] { disTypeId, specId, specTypeId, layerId };
            return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfNotInLayer", parm);
                      
        }

        /// <summary>
        /// 查找按照病种，规格，标本类型查找架子 并且架子所在冰箱的Id不在boxId中，用于移库
        /// </summary>
        /// <param name="disTypeId"></param>
        /// <param name="specId"></param>
        /// <param name="specTypeId"></param>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public ArrayList GetShelfNotInOneIceBox(string disTypeId, string specId, string specTypeId, string boxId)
        {
            string[] parm = new string[] { disTypeId, specId, specTypeId, boxId };
            return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfNotInIceBox", parm);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disTypeId"></param>
        /// <param name="specId"></param>
        /// <param name="specTypeId"></param>
        /// <param name="shelfId"></param>
        /// <returns></returns>
        public ArrayList GetOtherShelf(string disTypeId, string specId, string specTypeId, string shelfId)
        {
            string[] parm = new string[] { disTypeId, specId, specTypeId, shelfId };
            return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfBySpecBox", parm);
        }

        /// <summary>
        /// 根据barCode或者架子Id获取架子信息
        /// </summary>
        /// <param name="shelfId"></param>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public Shelf GetShelfByShelfId(string shelfId, string barCode)
        {
            ArrayList arrShelf = new ArrayList();
            if (shelfId != "")
            {
                string[] parms = new string[] { shelfId };
                arrShelf = this.SelectShelf("Speciment.BizLogic.ShelfManage.SelectByShelfID", parms);              
            }
            if (barCode != "")
            {
                arrShelf = this.SelectShelf("Speciment.BizLogic.ShelfManage.SelectByShelfBarCode", new string[] { barCode });                
            }
            return arrShelf[0] as Shelf;
        }

        /// <summary>
        /// 更新占用的数量
        /// </summary>
        /// <param name="count"></param>
        /// <param name="shelfId"></param>
        /// <returns></returns>
        public int UpdateOccupyCount(string count, string shelfId)
        {
            string[] parms = new string[] { count,shelfId };
            return this.UpdateShelf("Speciment.BizLogic.ShelfManage.OccupyCount", parms);
        }

        /// <summary>
        /// 更新架子是否已满
        /// </summary>
        /// <param name="shelfId"></param>
        /// <returns></returns>
        public int UpdateIsFull(string occupyFlag,string shelfId)
        {
            string[] parms = new string[] { occupyFlag,shelfId };
            return this.UpdateShelf("Speciment.BizLogic.ShelfManage.IsOccupy", parms);
        }

        /// <summary>
        /// 获取冰箱层的架子
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public ArrayList GetShelfByLayerID(string layerID)
        {
            string[] parms = new string[] { layerID };
            ArrayList arrShelf = this.SelectShelf("Speciment.BizLogic.ShelfManage.SelectByLayerID", parms);
            return arrShelf; 
        }

        /// <summary>
        /// 更新架子实体
        /// </summary>
        /// <param name="shelf"></param>
        /// <returns></returns>
        public int UpdateShelf(Shelf shelf)
        {
            return this.UpdateShelf("Speciment.BizLogic.ShelfManage.Update", GetParam(shelf));
        }

        /// <summary>
        /// 扫描冰箱层，给架子定位
        /// </summary>
        /// <param name="arrShelf">某一层中的所有架子</param>
        /// <returns></returns>
        public Shelf ScanLayer(ArrayList arrShelf)
        {
            Shelf current = new Shelf();
            Shelf next = new Shelf();

            //如果没有架子
            if (arrShelf.Count == 0)
            {
                return new Shelf();
            }
            if (arrShelf.Count >= 1)
            {
                current = arrShelf[0] as Shelf;
                //如果第一列没有架子
                if (current.IceBoxLayer.Col > 1)
                {
                    return new Shelf();
                }
            }
            //如果只有一个架子，返回当前的
            if (arrShelf.Count == 1)
            {
                return current;
            }
            for (int i = 0; i < arrShelf.Count - 1; i++)
            {
                current = arrShelf[i] as Shelf;
                next = arrShelf[i + 1] as Shelf;
                
                //如果两个架子是挨着的
                if (next.IceBoxLayer.Col - current.IceBoxLayer.Col == 1)
                {
                    continue;
                }
                if (next.IceBoxLayer.Col - current.IceBoxLayer.Col >= 2)
                {
                    return current;
                }
            }
            //遍历完冰箱层,发现都是挨着的,返回最后一个
            return next; 
        }

        /// <summary>
        /// 获取标本盒所在的架子
        /// </summary>
        /// <param name="boxId">标本盒ID</param>
        /// <returns></returns>
        public Shelf GetShelfByBoxId(string boxId)
        {
            try
            {
                return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfBySpecBoxId", new string[] { boxId })[0] as Shelf;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据病种类型和标本类型获取架子 (OR操作)
        /// </summary>
        /// <param name="disId">病种类型</param>
        /// <param name="specTypeId">标本类型</param>
        /// <returns></returns>
        public ArrayList GetShelfByDisOrSpecType(string disId, string specTypeId)
        {
            try
            {
                return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfByDisAndSpecType", new string[] { disId, specTypeId });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据病种类型和标本类型获取架子 (And操作)
        /// </summary>
        /// <param name="disId">病种类型</param>
        /// <param name="specTypeId">标本类型</param>
        /// <returns></returns>
        public ArrayList GetShelfByDisAndSpecType(string disId, string specTypeId)
        {
            try
            {
                return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfByDisAndSpecType1", new string[] { disId, specTypeId });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据存储时间+病种类型，标本类型查询 And 操作
        /// </summary>
        /// <param name="disId"></param>
        /// <param name="specTypeId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetShelfBySubStoreTimeA(string disId, string specTypeId, string start, string end)
        {
            try
            {
                return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfByDisAndSpecTypeAndStoreTime1", new string[] { disId, specTypeId, start, end });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据存储时间+病种类型，标本类型查询 Or 操作
        /// </summary>
        /// <param name="disId"></param>
        /// <param name="specTypeId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetShelfBySubStoreTimeO(string disId, string specTypeId, string start, string end)
        {
            try
            {
                return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfByDisAndSpecTypeAndStoreTime", new string[] { disId, specTypeId, start, end });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据存储时间查询标本架子列表
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetShelfBySubStoreTime(string start, string end)
        {
            try
            {
                return this.SelectShelf("Speciment.BizLogic.ShelfManage.GetShelfByStoreTime", new string[] { start, end });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取架子当中最大的标本号
        /// </summary>
        /// <param name="shelfId"></param>
        /// <returns></returns>
        public string GetMaxOrMinSubInShelf(string shelfId,string minOrMax)
        {
            string sql = @"
                          select case length(oper(subbarcode)) when 9 then  substr(oper(subbarcode),2,6) else substr(oper(subbarcode),1,6) end
                          from SPEC_SUBSPEC
                          where boxid in
                          (
                          select boxid 
                          from SPEC_BOX
                          where DESCAPID = {0} and DESCAPTYPE = 'J'

                          )
                          and subbarcode <>''";
            sql = sql.Replace("oper", minOrMax);
            sql = string.Format(sql, shelfId);
            return this.ExecSqlReturnOne(sql);
        }
 

        #endregion
    }
}
