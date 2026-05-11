using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 冰箱层管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-10]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class IceBoxLayerManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">冰箱中层对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.IceBoxLayer layer)
        {           
            layer.Comment = "";
            string[] str = new string[]
						{
							layer.LayerId.ToString(), 
							layer.IceBox.IceBoxId.ToString(),
                            layer.IsOccupy.ToString(),
                            layer.LayerNum.ToString(),
                            layer.Height.ToString(),
                            layer.Row.ToString(),
                            layer.Col.ToString(),
                            layer.SpecID.ToString(),
                            layer.SaveType.ToString(),
                            layer.DiseaseType.DisTypeID.ToString(),
                            layer.SpecTypeID.ToString(),
                            layer.BloodOrOrgId,
                            layer.Capacity.ToString(),
                            layer.OccupyCount.ToString(),
                            layer.Comment
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
            sequence = this.GetSequence("Speciment.BizLogic.IceBoxLayerManage.GetNextSequence");
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

        #region 更新冰箱层操作
        /// <summary>
        /// 更新架子规格
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateIceBoxLayer(string sqlIndex, params string[] args)
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
        /// 读取标本组织类型信息
        /// </summary>
        /// <returns>组织类型实体</returns>
        private IceBoxLayer SetIceBoxLayer()
        {
            IceBoxLayer layer = new IceBoxLayer();
            try
            {
                layer.LayerId = Convert.ToInt32(this.Reader[0].ToString());
                layer.IceBox.IceBoxId = Convert.ToInt32(this.Reader[1].ToString());
                layer.IsOccupy = Convert.ToInt16(this.Reader[2].ToString());
                layer.LayerNum = Convert.ToInt16(this.Reader[3].ToString());
                layer.Height = Convert.ToInt32(this.Reader[4].ToString());
                layer.Row = Convert.ToInt32(this.Reader[5].ToString());
                layer.Col = Convert.ToInt32(this.Reader[6].ToString());
                layer.SpecID = Convert.ToInt32(this.Reader[7].ToString());
                layer.SaveType = Convert.ToChar(this.Reader[8].ToString());
                layer.DiseaseType.DisTypeID = Convert.ToInt32(this.Reader[9].ToString());
                if (!Reader.IsDBNull(10)) layer.SpecTypeID = Convert.ToInt32(this.Reader[10].ToString());
                if (!Reader.IsDBNull(11)) layer.BloodOrOrgId = Reader[11].ToString();
                if (!Reader.IsDBNull(12)) layer.Capacity = Convert.ToInt32(Reader[12].ToString());
                if (!Reader.IsDBNull(13)) layer.OccupyCount = Convert.ToInt32(Reader[13].ToString());
                if (!Reader.IsDBNull(14)) layer.Comment = Reader[14].ToString();

                // orgType.OrgName = this.Reader[1].ToString();
                //orgType.IsShowOnApp = Convert.ToInt16(this.Reader[2].ToString());
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return layer;
        }
        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 冰箱层插入
        /// </summary>
        /// <param name="specBox">即将插入的冰箱层</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertIceBoxLayer(Neusoft.HISFC.Models.Speciment.IceBoxLayer layer)
        {
            return this.UpdateIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.Insert", this.GetParam(layer));

        }

        /// <summary>
        /// 根据冰箱名和层得到IceBoxLayerID
        /// </summary>
        /// <param name="iceBoxID">冰箱的ID</param>
        /// <param name="layNum">层</param>
        /// <returns>LayerID</returns>
        public IceBoxLayer GetLayerIDByIceBox(string iceBoxID, string layNum)
        {
            string[] parm = new string[] { iceBoxID, layNum };
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.IceBoxLayerManage.SelectIDByIceBox", ref sql) == -1)
                return  null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            IceBoxLayer layer = new IceBoxLayer();
            while (this.Reader.Read())
            {
                layer = SetIceBoxLayer();
            }
            this.Reader.Close();
            return layer;
        }

        /// <summary>
        ///  病种ID,和标本盒规格ID
        /// </summary>
        /// <param name="disTypeId">病种Id</param>
        /// <param name="specId">标本盒规格Id</param>
        /// <param name="specClassId">标本种类ID</param>
        /// <param name="specTypeId">标本类型ID</param>
        /// <returns>冰箱层</returns>
        private IceBoxLayer GetLayer(string selectSQL, string disTypeId, string specId, string specClassId, string specTypeId)
        {
            string[] parm = new string[] { disTypeId, specId, specClassId, specTypeId };
            string sql = "";
            if (this.Sql.GetSql(selectSQL, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            IceBoxLayer layer = new IceBoxLayer();
            while (this.Reader.Read())
            {
                layer = SetIceBoxLayer();
                break;
            }
            this.Reader.Close();
            return layer;
        }

        /// <summary>
        /// 根据sql索引查找IceBoxLayer
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList GetIceBoxLayer(string sqlIndex, string[] parm)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            IceBoxLayer layer;
            ArrayList arrLayer = new ArrayList();
            while (this.Reader.Read())
            {
                layer = new IceBoxLayer();
                layer = SetIceBoxLayer();
                arrLayer.Add(layer);
            }
            this.Reader.Close();
            return arrLayer;
        }

        /// <summary>
        /// 根据sql索引查找IceBoxLayer
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList GetIceBoxLayerAndAtt(string sqlIndex, string[] parm, string whereSub)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            sql += whereSub;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            IceBoxLayer layer;
            ArrayList arrLayer = new ArrayList();
            while (this.Reader.Read())
            {
                layer = new IceBoxLayer();
                layer = SetIceBoxLayer();
                arrLayer.Add(layer);
            }
            this.Reader.Close();
            return arrLayer;
        }

        /// <summary>
        ///  病种ID,和标本盒规格ID, 如果标本存放在冰箱上执行此函数
        /// </summary>
        /// <param name="disTypeId">病种Id</param>
        /// <param name="specId">标本盒规格Id</param>
        /// <param name="specClassId">标本种类ID</param>
        /// <param name="specTypeId">标本类型ID</param>
        /// <returns>冰箱层</returns>
        public IceBoxLayer LayerGetLayerById(string disTypeId, string specId, string specClassId, string specTypeId)
        {
            return GetLayer("Speciment.BizLogic.IceBoxLayerManage.GetLayerById", disTypeId, specId, specClassId, specTypeId);
        }

        /// <summary>
        ///  病种ID,和标本盒规格ID, 如果标本存放在冻存架执行此函数
        /// </summary>
        /// <param name="disTypeId">病种Id</param>
        /// <param name="specId">冻存架规格Id</param>
        /// <param name="specClassId">标本种类ID</param>
        /// <param name="specTypeId">标本类型ID</param>
        /// <returns>冰箱层</returns>
        public IceBoxLayer ShelfGetLayerById(string disTypeId, string specId, string specClassId, string specTypeId)
        {
            return GetLayer("Speciment.BizLogic.IceBoxLayerManage.GetLayerByShelfId", disTypeId, specId, specClassId, specTypeId);
        }

        /// <summary>
        /// 更新冰箱层的占用情况
        /// </summary>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public int UpdateOccupy(string occupyFlag, string layerId)
        {
            string[] parms = new string[] { occupyFlag ,layerId };
            return this.UpdateIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.UpdateOccupy", parms);
        }

        /// <summary>
        /// 更新冰箱占用的位置数
        /// </summary>
        /// <param name="occupyCount">占用的位置数</param>
        /// <param name="layerId">冰箱层ID</param>
        /// <returns></returns>
        public int UpdateOccupyCount(string occupyCount, string layerId)
        {
            string[] parms = new string[] { occupyCount, layerId };
            return this.UpdateIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.UpdateOccupyCount", parms);
        }

        /// <summary>
        /// 获取一个冰箱中有几层
        /// </summary>
        /// <param name="iceBoxId"></param>
        /// <returns></returns>
        public ArrayList GetIceBoxLayers(string iceBoxId)
        {
            string[] parm = new string[] { iceBoxId };
            return GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.SelectByIceBoxName", parm);
        }

        /// <summary>
        /// 获取冰箱中每一层的规格
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public Dictionary<IceBoxLayer, List<int>> GetLayerSpec(ArrayList arr)
        {
            int i = 1;
            List<int> layerNum;
            List<KeyValuePair<IceBoxLayer, List<int>>> listLayerSpec = new List<KeyValuePair<IceBoxLayer, List<int>>>();
            Dictionary<IceBoxLayer, List<int>> dicLayerSpec = new Dictionary<IceBoxLayer, List<int>>();

            //遍历每一层，如果层中设置一样，将层号加入LayerNum中，如果层中设置不一样将每一层设置加入listLayerSpec
            foreach (IceBoxLayer layer in arr)
            {
                layerNum = new List<int>();
                if (i == 1)
                {
                    layerNum.Add(layer.LayerNum);
                    KeyValuePair<IceBoxLayer, List<int>> tmp = new KeyValuePair<IceBoxLayer, List<int>>(layer, layerNum);
                    listLayerSpec.Add(tmp);
                }
                //小bug，当冰箱中隔层设置一样，计算成不一样的规格，SourcePlanManage也一样
                for (int k = listLayerSpec.Count - 1; k >= -1; k--)
                {
                    if (k == -1)
                    {
                        layerNum.Add(layer.LayerNum);
                        KeyValuePair<IceBoxLayer, List<int>> tmp = new KeyValuePair<IceBoxLayer, List<int>>(layer, layerNum);
                        listLayerSpec.Add(tmp);
                        break;
                    }
                    KeyValuePair<IceBoxLayer, List<int>> item = listLayerSpec[k];
                    //if (!layer.CheckSameSpec(item.Key))
                    //{
                    //    layerNum.Add(layer.LayerNum);
                    //    KeyValuePair<IceBoxLayer, List<int>> tmp = new KeyValuePair<IceBoxLayer, List<int>>(layer, layerNum);
                    //    listLayerSpec.Add(tmp);
                    //    break;
                    //}
                    if (layer.CheckSameSpec(item.Key))
                    {
                        if (item.Value.Contains(layer.LayerNum))
                            break;
                        else
                        {
                            item.Value.Add(layer.LayerNum);
                            break;
                        }
                    }
                }
                i++;
            }
            foreach (KeyValuePair<IceBoxLayer, List<int>> kp in listLayerSpec)
            {
                dicLayerSpec.Add(kp.Key, kp.Value);
            }
            return dicLayerSpec;
        }

        /// <summary>
        /// 根据LayerID获取Layer
        /// </summary>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public IceBoxLayer GetLayerById(string layerId)
        {
            ArrayList arrLayer = GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.SelectIDByLayerID", new string[] { layerId });
            return arrLayer[0] as IceBoxLayer;
            //Neusoft.HISFC.BizLogic.Speciment.IceBoxLayer.SelectIDByLayerID
        }

        public int UpdateLayer(IceBoxLayer layer)
        {
            return this.UpdateIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.Update", this.GetParam(layer));
        }

        /// <summary>
        /// 获取一个冰箱里所有的冰箱层
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public ArrayList GetLayerInOneBox(string boxId)
        {
            return GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.SelectInOneIceBox", new string[] { boxId });
        } 

        /// <summary>
        /// 根据标本盒Id获取冰箱层
        /// </summary>
        /// <param name="boxId">标本盒Id</param>
        /// <returns></returns>
        public IceBoxLayer GetLayerBySpecBox(string boxId)
        {
            try
            {
                return this.GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.GetIceBoxLayerBySpecBoxId", new string[] { boxId })[0] as IceBoxLayer;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据架子获取所在的冰箱层
        /// </summary>
        /// <param name="shelfId">架子Id</param>
        /// <returns></returns>
        public IceBoxLayer GetLayerByShelf(string shelfId)
        {
            try
            {
                return this.GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.GetIceBoxLayerByShelfId", new string[] { shelfId })[0] as IceBoxLayer;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据病种类型和标本类型获取冰箱层 (OR操作)
        /// </summary>
        /// <param name="disId">病种类型</param>
        /// <param name="specTypeId">标本类型</param>
        /// <returns></returns>
        public ArrayList GetLayerByDisOrSpecType(string disId, string specTypeId)
        {
            try
            {
                return this.GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.GetIceBoxLayerByDisAndSpecType", new string[] { disId, specTypeId });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据病种类型和标本类型获取冰箱层 (And操作)
        /// </summary>
        /// <param name="disId">病种类型</param>
        /// <param name="specTypeId">标本类型</param>
        /// <returns></returns>
        public ArrayList GetIceBoxByDisAndSpecType(string disId, string specTypeId)
        {
            try
            {
                return this.GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.GetIceBoxLayerByDisAndSpecType1", new string[] { disId, specTypeId });
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
        public ArrayList GetIceBoxBySubStoreTimeA(string disId, string specTypeId,string start ,string end)
        {
            try
            {
                return this.GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.GetIceBoxLayerByDisAndSpecTypeAndStoreTime1", new string[] { disId, specTypeId, start, end });
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 根据存储时间+病种类型，标本类型查询 OR 操作
        /// </summary>
        /// <param name="disId"></param>
        /// <param name="specTypeId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetIceBoxBySubStoreTimeO(string disId, string specTypeId, string start, string end)
        {
            try
            {
                return this.GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.GetIceBoxLayerByDisAndSpecTypeAndStoreTime", new string[] { disId, specTypeId, start, end });
            }
            catch
            {
                return null;
            }
        } 

        /// <summary>
        /// 根据标本存储时间查询冰箱层
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetIceBoxByStoreTime(string start, string end)
        {
            try
            {
                return this.GetIceBoxLayer("Speciment.BizLogic.IceBoxLayerManage.GetIceBoxLayerByStoreTime", new string[] { start, end });
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
