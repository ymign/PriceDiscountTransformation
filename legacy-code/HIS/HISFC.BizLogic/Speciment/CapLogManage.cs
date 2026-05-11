using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    public class CapLogManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="capLog"></param>
        /// <returns></returns>
        private string[] GetParam(CapLog capLog)
        {
            string[] str = new string[]
                          {
                              capLog.OperId.ToString(),
                              capLog.CapId.ToString(),
                              capLog.BarCode,
                              capLog.OldDesCapID.ToString(),
                              capLog.OldDesCapRow.ToString(),
                              capLog.OldDesCapCol.ToString(),
                              capLog.OldDesCapHeight.ToString(),
                              capLog.OldInType.ToString(),
                              capLog.NewDesCapID.ToString(),
                              capLog.NewDesCapRow.ToString(),
                              capLog.OldDesCapCol.ToString(),
                              capLog.OldDesCapHeight.ToString(),
                              capLog.NewInType.ToString(),
                              capLog.OperName,
                              capLog.OperTime.ToString(),
                              capLog.OperDes,
                              capLog.Comment,
                              capLog.OldDisType.ToString(),
                              capLog.NewDisType.ToString(),
                              capLog.OldSpecType.ToString(),
                              capLog.NewSpecType.ToString()
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
            this.Err = errorText + "[" + this.Err + "]"; // + "在ShelfSpecManage.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新标本容器日志操作
        /// <summary>
        /// 更新标本容器日志操作
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateLog(string sqlIndex, params string[] args)
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
        /// 读取标本容器日志信息
        /// </summary>
        /// <returns></returns>
        private CapLog SetCapLog()
        {
            CapLog capLog = new CapLog();
            try
            {
                capLog.OperId = Convert.ToInt32(this.Reader["OPERID"].ToString());
                capLog.CapId = Convert.ToInt32(this.Reader["CAPID"].ToString());
                capLog.BarCode = this.Reader["BARCODE"].ToString();
                capLog.OldDesCapID = Convert.ToInt32(this.Reader["OLDDESCAPID"].ToString());
                capLog.OldDesCapRow = Convert.ToInt32(this.Reader["OLDDESCAPROW"].ToString());
                capLog.OldDesCapCol = Convert.ToInt32(this.Reader["OLDDESCAPCOL"].ToString());
                capLog.OldDesCapHeight = Convert.ToInt32(this.Reader["OLDDESCAPHEIGHT"].ToString());
                capLog.OldInType = Convert.ToChar(this.Reader["OLDINTYPE"].ToString());
                capLog.NewDesCapID = Convert.ToInt32(this.Reader["NEWDESCAPID"].ToString());
                capLog.NewDesCapRow = Convert.ToInt32(this.Reader["NEWDESCAPROW"].ToString());
                capLog.NewDesCapCol = Convert.ToInt32(this.Reader["NEWDESCAPCOL"].ToString());
                capLog.NewDesCapHeight = Convert.ToInt32(this.Reader["NEWDESCAPHEIGHT"].ToString());
                capLog.NewInType = Convert.ToChar(this.Reader["NEWINTYPE"].ToString());
                capLog.OperName = this.Reader["OPERNAME"].ToString();
                capLog.OperTime = Convert.ToDateTime(this.Reader["OPERDATE"].ToString());
                capLog.OperDes = this.Reader["OPERDES"].ToString();
                capLog.Comment = this.Reader["MARK"].ToString();
                capLog.OldDisType = Convert.ToInt32(this.Reader["OLDDISTYPE"].ToString());
                capLog.NewDisType = Convert.ToInt32(this.Reader["NEWDISTYPE"].ToString());
                capLog.OldSpecType = Convert.ToInt32(this.Reader["OLDSPECTYPE"].ToString());
                capLog.NewSpecType = Convert.ToInt32(this.Reader["NEWSPECTYPE"].ToString());
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return capLog;
        }
        #endregion

        #endregion

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
            sequence = this.GetSequence("Speciment.BizLogic.CapLogManage.GetNextSequence");
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
        /// 插入日志信息
        /// </summary>
        /// <param name="capLog"></param>
        /// <returns></returns>
        public int InsertCapLog(CapLog capLog)
        {
            return this.UpdateLog("Speciment.BizLogic.CapLogManage.Insert", GetParam(capLog));
        }

        /// <summary>
        /// 废弃标本架
        /// </summary>
        /// <param name="shelf"></param>
        /// <returns></returns>
        public int DisuseShelf(Shelf shelf,string operName,string operDes,string comment)
        {
            CapLog capLog = new CapLog();
            string sequence = "";
            GetNextSequence(ref sequence);
            capLog.OperId = Convert.ToInt32(sequence);
            capLog.BarCode = shelf.SpecBarCode;
            capLog.CapId = shelf.ShelfID;
            capLog.NewDesCapCol = 0;
            capLog.NewDesCapHeight = 0;
            capLog.NewDesCapID = 0;
            capLog.NewDesCapRow = 0;
            capLog.NewInType = 'E';
            capLog.OldDesCapCol = shelf.IceBoxLayer.Col;
            capLog.OldDesCapHeight = shelf.IceBoxLayer.Height;
            capLog.OldDesCapID = shelf.IceBoxLayer.LayerId;
            capLog.OldDesCapRow = shelf.IceBoxLayer.Row;
            capLog.OldInType = 'L';
            capLog.OperDes = operDes;
            capLog.OperName = operName;
            capLog.OperTime = DateTime.Now;
            capLog.Comment = comment;
            return this.InsertCapLog(capLog);
        }

        /// <summary>
        /// 废弃标本盒
        /// </summary>
        /// <param name="box"></param>
        /// <param name="operName"></param>
        /// <returns></returns>
        public int DisuseSpecBox(SpecBox box, string operName,string operDes)
        {
            CapLog capLog = new CapLog();
            string sequence = "";
            GetNextSequence(ref sequence);
            capLog.OperId = Convert.ToInt32(sequence);
            capLog.BarCode = box.BoxBarCode;
            capLog.CapId = box.BoxId;
            capLog.NewDesCapCol = 0;
            capLog.NewDesCapHeight = 0;
            capLog.NewDesCapID = 0;
            capLog.NewDesCapRow = 0;
            capLog.NewInType = 'E';
            capLog.OldDesCapCol = box.DesCapCol;
            capLog.OldDesCapHeight = box.DesCapSubLayer;
            capLog.OldDesCapID = box.DesCapID;
            capLog.OldDesCapRow = box.DesCapRow;
            capLog.OldInType = box.DesCapType;
            capLog.OperDes = operDes;
            capLog.OperName = box.Name;
            capLog.OperTime = DateTime.Now;
            capLog.Comment = "标本盒废弃";
            return InsertCapLog(capLog);
        }

        /// <summary>
        /// 标本盒的修改日志信息
        /// </summary>
        /// <param name="box"></param>
        /// <param name="operName"></param>
        /// <param name="operDes"></param>
        /// <returns></returns>
        public int ModifyBoxLog(SpecBox box, string operName, string operDes,SpecBox newBox,string comment)
        {
            CapLog capLog = new CapLog();
            string sequence = "";
            GetNextSequence(ref sequence);
            capLog.OperId = Convert.ToInt32(sequence);
            capLog.BarCode = newBox.BoxBarCode == "" ? box.BoxBarCode : newBox.BoxBarCode;
            capLog.CapId = box.BoxId;
            capLog.NewDesCapCol = newBox.DesCapCol;
            capLog.NewDesCapHeight = newBox.DesCapSubLayer;
            capLog.NewDesCapID = newBox.DesCapID;
            capLog.NewDesCapRow = newBox.DesCapRow;
            capLog.NewInType = newBox.DesCapType;
            capLog.OldDesCapCol = box.DesCapCol;
            capLog.OldDesCapHeight = box.DesCapSubLayer;
            capLog.OldDesCapID = box.DesCapID;
            capLog.OldDesCapRow = box.DesCapRow;
            capLog.OldInType = box.DesCapType;
            capLog.OperDes = operDes;
            capLog.OperName = operName;
            capLog.OperTime = DateTime.Now;
            capLog.Comment = comment;
            capLog.OldSpecType = box.SpecTypeID;
            capLog.OldDisType = box.DiseaseType.DisTypeID;
            capLog.NewSpecType = newBox.SpecTypeID;
            capLog.NewDisType = newBox.DiseaseType.DisTypeID;
            return InsertCapLog(capLog);
        }

        /// <summary>
        /// 架子的修改日志
        /// </summary>
        /// <param name="shelf"></param>
        /// <param name="operName"></param>
        /// <param name="operDes"></param>
        /// <param name="newShelf"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public int ModifyShelf(Shelf shelf, string operName, string operDes,Shelf newShelf, string comment)
        {
            CapLog capLog = new CapLog();
            string sequence = "";
            GetNextSequence(ref sequence);
            capLog.OperId = Convert.ToInt32(sequence);
            capLog.BarCode = shelf.SpecBarCode == "" ? shelf.SpecBarCode : newShelf.SpecBarCode;
            capLog.CapId = shelf.ShelfID;
            capLog.NewDesCapCol = shelf.IceBoxLayer.Col;
            capLog.NewDesCapHeight = shelf.IceBoxLayer.Height;
            capLog.NewDesCapID = shelf.IceBoxLayer.LayerId;
            capLog.NewDesCapRow = shelf.IceBoxLayer.Row;            
            capLog.OldDesCapCol = shelf.IceBoxLayer.Col;
            capLog.OldDesCapHeight = shelf.IceBoxLayer.Height;
            capLog.OldDesCapID = shelf.IceBoxLayer.LayerId;
            capLog.OldDesCapRow = shelf.IceBoxLayer.Row;
            capLog.OldInType = 'L';
            capLog.OperDes = operDes;
            capLog.OperName = operName;
            capLog.OperTime = DateTime.Now;
            capLog.Comment = comment;
            capLog.OldSpecType = shelf.SpecTypeId;
            capLog.OldDisType = shelf.DisTypeId;
            capLog.NewSpecType = newShelf.SpecTypeId;
            capLog.NewDisType = newShelf.DisTypeId;
            return this.InsertCapLog(capLog);
        }

        /// <summary>
        /// 冰箱的修改日志
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="operName"></param>
        /// <param name="operDes"></param>
        /// <param name="newLayer"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public int ModifyIceBoxLayer(IceBoxLayer layer, string operName, string operDes, IceBoxLayer newLayer, string comment)
        {
            CapLog capLog = new CapLog();
            string sequence = "";
            GetNextSequence(ref sequence);
            capLog.OperId = Convert.ToInt32(sequence);
            capLog.CapId = layer.LayerId;
            capLog.NewDesCapCol = 1;
            capLog.NewDesCapHeight = newLayer.LayerNum;
            capLog.NewDesCapID = newLayer.IceBox.IceBoxId;
            capLog.NewDesCapRow = 1;
            capLog.OldDesCapCol = 1;
            capLog.OldDesCapHeight = layer.LayerNum;
            capLog.OldDesCapID = layer.IceBox.IceBoxId;
            capLog.OldDesCapRow = 1;
            capLog.OldInType = 'B';
            capLog.OperDes = operDes;
            capLog.OperName = operName;
            capLog.OperTime = DateTime.Now;
            capLog.Comment = comment;
            capLog.OldSpecType = layer.SpecTypeID;
            capLog.OldDisType = layer.DiseaseType.DisTypeID;
            capLog.NewSpecType = newLayer.SpecTypeID;
            capLog.NewDisType = newLayer.DiseaseType.DisTypeID;
            return this.InsertCapLog(capLog);
        }
    }
}
