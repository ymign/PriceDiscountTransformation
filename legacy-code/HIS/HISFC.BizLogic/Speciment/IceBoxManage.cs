using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 冰箱管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-12]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class IceBoxManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">冰箱实体对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.IceBox iceBox, string oper)
        {
            string sequence = "";
            switch (oper)
            {
                case "Insert":
                    GetNextSequence(ref sequence);
                    break;
                case "Update":
                    sequence = iceBox.IceBoxId.ToString();
                    break;
                default:
                    break;
            }

            string[] str = new string[]
						{
							sequence, 							
                            iceBox.LayerNum.ToString(),
                            iceBox.IsOccupy.ToString(),
                            iceBox.IceBoxTypeId.ToString(),
                            iceBox.IceBoxName,
                            iceBox.SpecTypeId,
                            iceBox.OrgOrBlood,
                            iceBox.UseStaus,
                            iceBox.Comment
                        };
            return str;
        }

        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        private int GetNextSequence(ref string sequence)
        {
            //
            // 执行SQL语句
            //
            sequence = this.GetSequence("Speciment.BizLogic.IceBoxManage.GetNextSequence");
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

        /// <summary>
        /// 读取冰箱ID信息
        /// </summary>
        /// <returns>冰箱的ID</returns>
        private IceBox SetIcebox()
        {
            IceBox iceBox = new IceBox();
            try
            {
                //iceBox.IceBoxId = Convert.ToInt32(this.Reader[0].ToString());
                iceBox.IceBoxId = Convert.ToInt32(this.Reader[0].ToString());
                //iceBox.LayerNum = Convert.ToInt32(this.Reader[1].ToString());
                //iceBox.IsOccupy = Convert.ToInt16(this.Reader[2].ToString());
                //iceBox.IceBoxTypeId = this.Reader[3].ToString();
                iceBox.IceBoxName = this.Reader[4].ToString();

            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return iceBox;
        }

        /// <summary>
        /// 读取IceBox的完整实体信息
        /// </summary>
        /// <returns>冰箱实体</returns>
        private IceBox SetAllIceBox()
        {
            IceBox iceBox = new IceBox();
            try
            {
                iceBox.IceBoxId = Convert.ToInt32(this.Reader[0].ToString());
                iceBox.LayerNum = Convert.ToInt32(this.Reader[1].ToString());
                iceBox.IsOccupy = Convert.ToInt16(this.Reader[2].ToString());
                iceBox.IceBoxTypeId = this.Reader[3].ToString();
                iceBox.IceBoxName = this.Reader[4].ToString();
                iceBox.SpecTypeId = this.Reader[5].ToString();
                iceBox.OrgOrBlood = this.Reader[6].ToString();
                iceBox.UseStaus = this.Reader[7].ToString();
                if (!Reader.IsDBNull(8)) iceBox.Comment = this.Reader[8].ToString();
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return iceBox;
        }

        /// <summary>
        /// 根据sql索引取出Icebox
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList SelectIceBox(string sqlIndex, string[] parm)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            IceBox iceBox;
            ArrayList arrIceBox = new ArrayList();
            while (this.Reader.Read())
            {
                iceBox = SetAllIceBox();
                arrIceBox.Add(iceBox);
            }
            this.Reader.Close();
            return arrIceBox;
        }

        /// <summary>
        /// 察看指定的冰箱是否有标本
        /// </summary>
        /// <param name="iceBoxId"></param>
        /// <returns></returns>
        public int CheckIceBoxHaveSpecBox(string iceBoxId)
        {
            string[] parm = new string[] { iceBoxId };
            IceBox ib = new IceBox();
            ArrayList arr = new ArrayList();

            arr = SelectIceBox("Speciment.BizLogic.IceBoxManage.IceBoxHaveSpecBox1", parm);
            if (arr.Count > 0 && arr != null)
                return 1;

            arr = new ArrayList();
            arr = SelectIceBox("Speciment.BizLogic.IceBoxManage.IceBoxHaveSpecBox2", parm);
            if (arr.Count > 0 && arr != null)
                return 1;

            return -1;

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

        #region 更新冰箱操作
        /// <summary>
        /// 更新冰箱
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateIceBox(string sqlIndex, params string[] args)
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

        #endregion

        #region 公共方法
        /// <summary>
        /// 冰箱插入
        /// </summary>
        /// <param name="specBox">即将插入的冰箱</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertIceBox(Neusoft.HISFC.Models.Speciment.IceBox iceBox)
        {
            try
            {
                return this.UpdateIceBox("Speciment.BizLogic.IceBoxManage.Insert", this.GetParam(iceBox, "Insert"));
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新冰箱
        /// </summary>
        /// <param name="iceBox"></param>
        /// <returns>影响的行数；－1：失败</returns>
        public int UpdateIceBox(IceBox iceBox)
        {
            try
            {
                return this.UpdateIceBox("Speciment.BizLogic.IceBoxManage.Update", this.GetParam(iceBox, "Update"));
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                return -1;
            }

        }

        /// <summary>
        ///根据名字获取冰箱的ID号
        /// </summary>
        /// <param name="iceBoxName"></param>
        /// <returns></returns>
        public IceBox GetIceBoxByName(string iceBoxName)
        {
            string[] parm = new string[] { iceBoxName };
            ArrayList arr = SelectIceBox("Speciment.BizLogic.IceBoxManage.SelectByName", parm);
            if (arr.Count > 0)
                return arr[0] as IceBox;
            else
                return null;
        }
        /// <summary>
        /// 根据层的ID号查找所在的冰箱
        /// </summary>
        /// <param name="layerID">层ID</param>
        /// <returns></returns>
        public IceBox GetIceBoxByLayerID(string layerID)
        {
            string[] parm = new string[] { layerID };
            ArrayList arr = SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByLayer", parm);
            if (arr.Count > 0)
                return arr[0] as IceBox;
            return null;
        }

        /// <summary>
        /// 查询得到所有冰箱的信息
        /// </summary>
        /// <returns>冰箱List</returns>
        public ArrayList GetAllIceBox()
        {
            return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.SelectAllIceBox", new string[] { });
            //string sql = "";
            //if (this.Sql.GetSql("Speciment.BizLogic.IceBoxManage.SelectAllIceBox", ref sql) == -1)
            //    return null;

            //if (this.ExecQuery(sql) == -1)
            //    return null;
            //ArrayList arrIceBox = new ArrayList();
            //while (this.Reader.Read())
            //{
            //    arrIceBox.Add(SetAllIceBox());
            //}
            //this.Reader.Close();
            //return arrIceBox;
        }

        /// <summary>
        /// 根据冰箱号获取冰箱
        /// </summary>
        /// <param name="iceBoxId"></param>
        /// <returns></returns>
        public IceBox GetIceBoxById(string iceBoxId)
        {
            string[] parm = new string[] { iceBoxId };
            ArrayList arr = SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxId", parm);
            if (arr.Count > 0)
            {
                return arr[0] as IceBox;
            }
            return null;
        }

        /// <summary>
        /// 根据冰箱的种类获取
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public ArrayList GetIceBoxByType(string typeId)
        {
            string[] parm = new string[] { typeId };
            return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByType", new string[] { typeId });
        }

        /// <summary>
        /// 查找盒子所在的冰箱
        /// </summary>
        /// <param name="boxId">标本盒Id</param>
        /// <returns></returns>
        public IceBox GetIceBoxBySpecBoxId(string boxId)
        {
            try
            {
                return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxBySpecBoxId", new string[] { boxId })[0] as IceBox;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 查找盒子所在的架子
        /// </summary>
        /// <param name="shelfId">架子Id</param>
        /// <returns></returns>
        public IceBox GetIceBoxByShelf(string shelfId)
        {
            try
            {
                return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByShelfId", new string[] { shelfId })[0] as IceBox;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据病种类型和标本类型获取冰箱 (OR操作)
        /// </summary>
        /// <param name="disId">病种类型</param>
        /// <param name="specTypeId">标本类型</param>
        /// <returns></returns>
        public ArrayList GetIceBoxByDisOrSpecType(string disId, string specTypeId)
        {
            try
            {
                return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByDisAndSpecType", new string[] { disId, specTypeId });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据病种类型和标本类型获取冰箱 (And操作)
        /// </summary>
        /// <param name="disId">病种类型</param>
        /// <param name="specTypeId">标本类型</param>
        /// <returns></returns>
        public ArrayList GetIceBoxByDisAndSpecType(string disId, string specTypeId)
        {
            try
            {
                return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByDisAndSpecType1", new string[] { disId, specTypeId });
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
        public ArrayList GetIceBoxBySubStoreTimeA(string disId, string specTypeId, string start, string end)
        {
            try
            {
                return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByDisAndSpecTypeAndStoreTime1", new string[] { disId, specTypeId, start, end });
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
        public ArrayList GetIceBoxBySubStoreTimeO(string disId, string specTypeId, string start, string end)
        {
            try
            {
                return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByDisAndSpecTypeAndStoreTime", new string[] { disId, specTypeId, start, end });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据操作时间查询冰箱
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetIceBoxBySubStoreTime(string start, string end)
        {
            try
            {
                return this.SelectIceBox("Speciment.BizLogic.IceBoxManage.GetIceBoxByStoreTime1", new string[] { "", "", start, end });
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
