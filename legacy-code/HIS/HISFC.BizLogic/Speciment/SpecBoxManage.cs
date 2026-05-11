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
    /// [功能描述: 标本盒管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-27]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-19' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecBoxManage : Neusoft.FrameWork.Management.Database
    {

        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">标本盒对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecBox specBox)
        {
            string[] str = new string[]
						{
							specBox.BoxId.ToString(), 
							specBox.BoxBarCode,
                            specBox.BoxSpec.BoxSpecID.ToString(),
                            specBox.DesCapType.ToString(),
                            specBox.DesCapID.ToString(),
                            specBox.DesCapCol.ToString(),
                            specBox.DesCapRow.ToString(),
                            specBox.DesCapSubLayer.ToString(),
                            specBox.DiseaseType.DisTypeID.ToString(),
                            specBox.OrgOrBlood.ToString(),
                            specBox.SpecTypeID.ToString(),
                            specBox.IsOccupy.ToString(),
                            specBox.Capacity.ToString(),
                            specBox.OccupyCount.ToString(),
                            specBox.InIceBox.ToString(),
                            specBox.Comment,
                            specBox.SpecialUse
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
            sequence = this.GetSequence("Speciment.BizLogic.SpecBoxManage.GetNextSequence");
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
            this.Err = errorText + "[" + this.Err + "]"; // + "在TerminalConfirm.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新标本盒操作

        private SpecBox SetSpecBox()
        {
            SpecBox specBox = new SpecBox();
            try
            {
                specBox.BoxId = Convert.ToInt32(this.Reader["BOXID"].ToString());
                specBox.BoxBarCode = this.Reader["BOXBARCODE"].ToString();
                specBox.BoxSpec.BoxSpecID = Convert.ToInt32(this.Reader["BOXSPECID"].ToString());
                specBox.DesCapType = Convert.ToChar(this.Reader["DESCAPTYPE"].ToString());
                specBox.DesCapID = Convert.ToInt32(this.Reader["DESCAPID"].ToString());
                specBox.DesCapCol = Convert.ToInt32(this.Reader["DESCAPCOL"].ToString());
                specBox.DesCapRow = Convert.ToInt32(this.Reader["DESCAPROW"].ToString());
                specBox.DesCapSubLayer = Convert.ToInt32(this.Reader["DESCAPSUBLAYER"].ToString());
                specBox.DiseaseType.DisTypeID = Convert.ToInt32(this.Reader["DISEASETYPEID"].ToString());
                specBox.OrgOrBlood = Convert.ToInt32(this.Reader["BLOODORORGID"].ToString());
                specBox.SpecTypeID = Convert.ToInt32(this.Reader["SPECTYPEID"].ToString());
                specBox.IsOccupy = Convert.ToChar(this.Reader["ISOCCUPY"].ToString());
                specBox.Capacity = Convert.ToInt32(this.Reader["CAPACITY"].ToString());
                specBox.OccupyCount = Convert.ToInt32(this.Reader["OCCUPYCOUNT"].ToString());
                specBox.InIceBox = Convert.ToChar(this.Reader["INBOX"].ToString());

                if (!Reader.IsDBNull(15)) specBox.SpecialUse = this.Reader["SPECUSE"].ToString();
                else specBox.SpecialUse = "";

                if (!Reader.IsDBNull(16)) specBox.Comment = this.Reader["MARK"].ToString();
                else specBox.Comment = "";

            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return specBox;
                 
        }

        /// <summary>
        /// 更新标本盒
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSpecBox(string sqlIndex, params string[] args)
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
        /// 获取符合条件的标本盒List　
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList GetSpecBox(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return null;
            }
            if (this.ExecQuery(sql, args)==-1)
                return null;
            ArrayList arrSpecBox = new ArrayList();
            while (this.Reader.Read())
            {
                SpecBox boxTmp = SetSpecBox();
                arrSpecBox.Add(boxTmp); 
            }
            this.Reader.Close();
            return arrSpecBox;
        }
        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 标本盒插入
        /// </summary>
        /// <param name="specBox">即将插入的标本盒</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertSpecBox(Neusoft.HISFC.Models.Speciment.SpecBox specBox)
        {
            return this.UpdateSpecBox("Speciment.BizLogic.SpecBoxManage.Insert", this.GetParam(specBox));
        }

        /// <summary>
        /// 在标本盒所放的容器中，查找编号最大的一个盒子
        /// </summary>
        /// <param name="capID">标本盒容器的ID</param>
        /// <returns></returns>
        public SpecBox GetLastCapBox(string sql,string capID)
        {
            string[] parms = new string[] { capID };
            ArrayList arrSpecBox = GetSpecBox(sql ,capID);
            SpecBox lastBox = new SpecBox();
            //如果arrSpecBox.Count<0 说明架子或层是空的，那么初始化第一个位置
            if (arrSpecBox.Count <= 0)
            {
                lastBox.DesCapCol = 0;
                lastBox.DesCapRow = 1;
                lastBox.DesCapSubLayer = 1;
                return lastBox;
 
            }           
            foreach (SpecBox s in arrSpecBox)
            {
                lastBox = s; 
            }
            return lastBox; 
        }

        /// <summary>
        /// 如果放在冰箱中
        /// </summary>
        /// <param name="capID"></param>
        /// <returns></returns>
        public SpecBox LayerGetLastCapBox(string capID)
        {
            return GetLastCapBox("Speciment.BizLogic.SpecBoxManage.BoxLocationLayer", capID);
        }

        /// <summary>
        /// 如果放在冻存架中
        /// </summary>
        /// <param name="capID"></param>
        /// <returns></returns>
        public SpecBox ShelfGetLastCapBox(string capID)
        {
            return GetLastCapBox("Speciment.BizLogic.SpecBoxManage.BoxLocation", capID);
        }

        /// <summary>
        /// 根据病种类型和标本盒的规格获取用过的，但没放标本的盒子
        /// </summary>
        /// <param name="disTypeId">病种类型</param>
        /// <param name="specId">规格ID</param>
        /// <param name="saveType">所在容器类型</param>
        /// <param name="specClassId">标本种类ID</param>
        /// <param name="specTypeId">标本类型ID</param>
        /// <returns></returns>
        public SpecBox GetUsedNoOccupyBox(string disTypeId, string specId, string saveType, string specClassId, string specTypeId, string specUse)
        {
            string[] parms = new string[] { disTypeId, specId,saveType, specClassId, specTypeId };
            ArrayList arrTmp = GetSpecBox("Speciment.BizLogic.SpecBoxManage.GetUsedNoOccupyBox", parms);
            SpecBox usedBox = new SpecBox();
            if (arrTmp.Count <= 0)
                return null;
            foreach (SpecBox s in arrTmp)
            {
                if (s.SpecialUse == specUse)
                {
                    usedBox = s;
                    break;
                }
                else
                {
                    continue;
                }
                //break;
            }
            return usedBox;
        }

        /// <summary>
        /// 在指定的架子或者冰箱中获取未使用的标本盒
        /// </summary>
        /// <param name="saveType"></param>
        /// <param name="capId"></param>
        /// <returns></returns>
        public SpecBox GetUsedNoOccupyBox(string saveType, string capId, string specUse)
        {
            string[] parms = new string[] { capId,saveType, specUse };
            ArrayList arrTmp = GetSpecBox("Speciment.BizLogic.SpecBoxManage.GetUsedNoOccupyBoxInCap", parms);
            if (arrTmp.Count > 0)
                return arrTmp[0] as SpecBox;
            else
                return null;
        }

        /// <summary>
        /// 根据架子号或者冰箱层号获取标本盒List
        /// </summary>
        /// <param name="capId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ArrayList GetBoxByCap(string capId, char type)
        {
            string[] parms = new string[] { capId };
            ArrayList arrBox = new ArrayList();
            switch (type)
            {
                case 'J':
                    arrBox = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.SelectByShelfID", parms);
                    break;
                case 'B':
                    arrBox = GetSpecBox("Speciment.BizLogic.SpecBoxManage.SelectByLayerID", parms);
                    break;
                default:
                    break;
            }
            return arrBox;
        }

        /// <summary>
        /// 查找目标容器中 某一行的盒子集合
        /// </summary>
        /// <param name="capId"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public ArrayList GetBoxByCapRow(string capId, string row)
        {
            string[] parms = new string[] { capId, row };
            ArrayList arrBox = new ArrayList();
            arrBox = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.LayerIDAndRow", parms);
            return arrBox;
        }

        /// <summary>
        /// 查找目标容器中某一列的盒子
        /// </summary>
        /// <param name="capId"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public ArrayList GetBoxByCapCol(string capId, string col)
        {
            string[] parms = new string[] { capId, col };
            ArrayList arrBox = new ArrayList();
            arrBox = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.LayerIDAndCol", parms);
            return arrBox;
        }

        /// <summary>
        /// 查找当前使用的标本盒
        /// </summary>
        /// <param name="disTypeId"></param>
        /// <param name="specTypeId"></param>
        /// <returns></returns>
        public ArrayList GetLastLocation(string disTypeId, string specTypeId)
        {
            string[] parms = new string[] { disTypeId, specTypeId };           
            ArrayList arrBox = new ArrayList();
            arrBox = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.SubSpecLocate", parms);
            return arrBox;
        }

        /// <summary>
        /// 更新标本盒的占用数量
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public int UpdateOccupyCount(string occupyCount,string boxId)
        {
            string[] parms = new string[] { occupyCount, boxId };
            return this.UpdateSpecBox("Speciment.BizLogic.SpecBoxManage.UpdateOccupyCount", parms);
        }

        /// <summary>
        /// 更新标本盒已满标志位
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public int UpdateOccupy(string boxId, string occupyFlag)
        {
            string[] parms = new string[] { occupyFlag,boxId };
            return this.UpdateSpecBox("Speciment.BizLogic.SpecBoxManage.UpdateOccupy", parms);
        }

        public int UpdateSotreFlag(string flag, string boxId)
        {
            string[] parms = new string[] { flag, boxId };
            return this.UpdateSpecBox("Speciment.BizLogic.SpecBoxManage.InBoxFlag", parms);
        }

        /// <summary>
        /// 标本盒入库时根据界面输入条件获取标本盒List
        /// </summary>
        /// <param name="boxSpecID">规格</param>
        /// <param name="orgTypeID">标本种类</param>
        /// <param name="specTypeID">标本类型</param>
        /// <param name="diseaseTypeID">病种</param>
        /// <param name="boxCode">条形码</param>
        /// <param name="isOccupy">是否已满</param>
        /// <returns>标本盒List</returns>
        public ArrayList GetBoxForInStore(string boxSpecID, string orgTypeID, string specTypeID, string diseaseTypeID, string boxCode, string isOccupy)
        {
            string[] parms = new string[] { boxSpecID, orgTypeID, specTypeID, diseaseTypeID, boxCode, isOccupy };
            ArrayList arrBox = new ArrayList();
            arrBox = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.GetBoxForInStore", parms);
            return arrBox;
        }

        /// <summary>
        /// 根据sql语句获取boxlist集合
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ds">返回的dataset集合</param>
        /// <returns>影响的行数</returns>
        public int GetBoxBySql(string sql, ref DataSet ds)
        {      
            return  this.ExecQuery(sql, ref ds);            
        }

        public ArrayList GetBoxBySql(string sql)
        {
            if (this.ExecQuery(sql) == -1)
                return null;
            ArrayList arrSpecBox = new ArrayList();
            while (this.Reader.Read())
            {
                SpecBox boxTmp = SetSpecBox();
                arrSpecBox.Add(boxTmp);
            }
            this.Reader.Close();
            return arrSpecBox;
        }

        /// <summary>
        /// 根据ＢｏｘＩｄ　查询标本盒
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public SpecBox GetBoxById(string boxId)
        {
            string sql = "select * from SPEC_BOX WHERE BOXID = " + boxId;
            if (this.ExecQuery(sql) == -1)
                return null;
            SpecBox specBox = new SpecBox();
            while (Reader.Read())
            {
                specBox = SetSpecBox();
            }
            Reader.Close();
            return specBox;
        }

        public int UpdateSpecBox(SpecBox specBox)
        {
            return this.UpdateSpecBox("Speciment.BizLogic.SpecBoxManage.Update", GetParam(specBox));
 
        }

        /// <summary>
        /// 根据条形码读取盒子的信息
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public SpecBox GetBoxByBarCode(string barCode)
        {
            string sql = "select * from SPEC_BOX WHERE BOXBARCODE = '" + barCode + "' and DESCAPID > 0";
            if (this.ExecQuery(sql) == -1)
                return null;
            SpecBox specBox = new SpecBox();
            while (Reader.Read())
            {
                specBox = SetSpecBox();
            }
            Reader.Close();
            return specBox;
        }

        /// <summary>
        /// 根据标本入库的时间段 查询标本盒列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ArrayList GetBoxByIn(string startTime, string endTime)
        {
            ArrayList arr = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.SpecInTime", new string[] { startTime, endTime });
            return arr;
        }

        /// <summary>
        /// 根据标本出库的时间段查询标本盒列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ArrayList GetBoxByOut(string startTime, string endTime)
        {
            ArrayList arr = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.SpecOutTime", new string[] { startTime, endTime });
            return arr;
        }

        /// <summary>
        /// 在指定的位置上，查看是否存在标本盒
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="height"></param>
        /// <param name="capId"></param>
        /// <param name="capType"></param>
        /// <returns></returns>
        public SpecBox GetByPoint(string col, string row, string height, string capId, string capType)
        {
            ArrayList arr = this.GetSpecBox("Speciment.BizLogic.SpecBoxManage.GetBoxInPointLocation", new string[] { col, row, height, capId, capType });
            try
            {
                return arr[0] as SpecBox;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
