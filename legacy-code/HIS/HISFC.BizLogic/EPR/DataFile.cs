 using System;
using System.Collections;
namespace Neusoft.HISFC.BizLogic.EPR
{
	/// <summary>
	/// GetFile 的摘要说明。
	/// 获得文件
	/// </summary>
	public class DataFile:Neusoft.FrameWork.Management.Database
	{
		public DataFile(string type)
		{
			try
			{
				dtParam = this.ParamManager.Get(type) as Neusoft.HISFC.Models.File.DataFileParam;
				if(dtParam==null) return;
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;				
				return;
			}
		}
		public DataFile()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

        public Neusoft.HISFC.BizLogic.EPR.DataFileParam ParamManager = new Neusoft.HISFC.BizLogic.EPR.DataFileParam();
        public Neusoft.HISFC.BizLogic.EPR.DataFileInfo FileManager = new Neusoft.HISFC.BizLogic.EPR.DataFileInfo();
		private Neusoft.HISFC.Models.File.DataFileParam  dtParam= null;
		private Neusoft.HISFC.Models.File.DataFileInfo dtFile =null;
		
		#region 属性
		/// <summary>
		/// 当前文件列表
		/// </summary>
		public ArrayList alFiles;
		/// <summary>
		/// 当前模板列表
		/// </summary>
		public ArrayList alModuals;
	
		/// <summary>
		/// 先设置显示类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int SetType(string type)
		{
			try
			{
				dtParam = this.ParamManager.Get(type) as Neusoft.HISFC.Models.File.DataFileParam;
				if(dtParam==null) return -1;
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;				
				return -1;
			}
			return 0;
		}
		/// <summary>
		/// 获得摸板
		/// </summary>
		/// <returns></returns>
		public int GetModuals(bool isAll)
		{
			try
			{
			
				alModuals = FileManager.GetList(dtParam,1,isAll);
				return alModuals.Count;
			}
			catch{return -1;}
		}
		/// <summary>
		/// 获得数据
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public int GetFiles(params string[] param)
		{
			try
			{
				this.dtFile = new Neusoft.HISFC.Models.File.DataFileInfo();
				this.dtFile.Param =(Neusoft.HISFC.Models.File.DataFileParam)this.dtParam.Clone();
				this.dtFile.Param.ID = string.Format(this.dtParam.ID,param);
				this.dtFile.Param.Type = this.dtParam.Type;
				alFiles = FileManager.GetList(this.dtFile.Param);
				return alFiles.Count;
			}
			catch{return -1;}
		}
		/// <summary>
		/// 获得参数
		/// </summary>
		public Neusoft.HISFC.Models.File.DataFileParam DataFileParam
		{
			get
			{
				if(this.dtParam==null)this.dtParam=new Neusoft.HISFC.Models.File.DataFileParam();
				return this.dtParam;
			}
			set
			{
				this.dtParam = value;
			}
		}
		/// <summary>
		/// 获得文件信息
		/// </summary>
		public Neusoft.HISFC.Models.File.DataFileInfo DataFileInfo
		{
			get
			{
				if(this.dtFile==null)this.dtFile =new Neusoft.HISFC.Models.File.DataFileInfo();
				return this.dtFile;
			}
			set
			{
				this.dtFile = value;
			}
		}

		
		#endregion

		#region 节点操作
		/// <summary>
		/// 保存结点到数据库
		/// </summary>
		/// <param name="Table"></param>
		/// <param name="dt"></param>
		/// <param name="ParentText"></param>
		/// <param name="NodeText"></param>
		/// <param name="NodeValue"></param>
		/// <returns></returns>
		public int SaveNodeToDataStore(string Table,Neusoft.HISFC.Models.File.DataFileInfo dt,string ParentText,string NodeText,string NodeValue)
		{			
			string strSql ="";
			string sql="";

			if(this.Sql.GetSql("Management.DataFile.Select",ref strSql)==-1) return -1;
			try
			{
				Neusoft.FrameWork.Public.String.FormatString (strSql,out sql,Table,dt.ID,dt.Type,dt.DataType,dt.Name,dt.Index1,dt.Index2,ParentText,NodeText,NodeValue,this.Operator.ID);
			}
			catch(Exception ex)
			{
				this.Err ="Management.DataFile.Select付值时候出错！"+ex.Message;
				this.WriteErr();
				return -1;
			}
			if(this.ExecQuery(sql)==-1) return -1;
			if(this.Reader.Read())//有记录－执行更新
			{
				if(NodeValue == this.Reader[0].ToString())//相同不变
				{
					this.Reader.Close();
					return 0;
				}
				else
				{
					if(this.Sql.GetSql("Management.DataFile.UpdateToDataStore",ref strSql)==-1) return -1;
				}
			}
			else//无记录－执行插入
			{
				if(this.Sql.GetSql("Management.DataFile.InsertToDataStore",ref strSql)==-1) return -1;
			}
			try
			{
				Neusoft.FrameWork.Public.String.FormatString (strSql,out sql,Table,dt.ID,dt.Type,dt.DataType,dt.Name,dt.Index1,dt.Index2,ParentText,NodeText,NodeValue,this.Operator.ID);
			}
			catch(Exception ex){
				this.Err ="SaveNodeToDataStore付值时候出错！"+ex.Message;
				this.WriteErr();
				return -1;
			}
			try
			{
				this.Reader.Close();
			}
			catch{}
			return this.ExecNoQuery(sql);
		}
		/// <summary>
		/// 删除结点　
		/// </summary>
		/// <param name="Table"></param>
		/// <param name="dt"></param>
		/// <returns></returns>
		public int DeleteAllNodeFromDataStore(string Table,Neusoft.HISFC.Models.File.DataFileInfo dt)
		{
			string strSql ="",sql="";
			if(this.Sql.GetSql("Management.DataFile.DeleteAllNodeFromDataStore",ref strSql)==-1) return -1;
			try
			{
				sql = string.Format(strSql,Table,dt.ID);
			}
			catch(Exception ex)
			{
				this.Err ="DeleteNode付值时候出错！"+ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(sql);
		}
		
		/// <summary>
		/// 获得节点内容
		/// </summary>
		/// <param name="Table"></param>
		/// <param name="inpatientNo"></param>
		/// <param name="nodeName"></param>
		/// <returns></returns>
		public string GetNodeValueFormDataStore(string Table,string inpatientNo,string nodeName)
		{
			string strSql ="",sql="";
			if(this.Sql.GetSql("Management.DataFile.GetNodeValueFormDataStore",ref strSql)==-1) return "-1";
			try
			{
				sql = string.Format(strSql,Table,inpatientNo,nodeName);
			}
			catch(Exception ex)
			{
				this.Err ="GetNodeValueFormDataStore付值时候出错！"+ex.Message;
				this.WriteErr();
				return "-1";
			}
			return this.ExecSqlReturnOne(sql);
		}
		#endregion

        //#region 大字段操作

        ///// <summary>
        ///// 将文件导入到数据库中
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <param name="fileData">输入的文件数据</param>
        ///// <returns></returns>
        //public int ImportToDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt,byte[] fileData)
        //{
        //    string strSql ="",sql="";
        //    if(dt.ID == null||dt.ID =="") return -1;

        //    if(this.Sql.GetSql("Management.DataFile.ImportToDatabase",ref strSql)==-1) return -1;
        //    try
        //    {
        //        sql = string.Format(strSql,dt.ID);
        //    }
        //    catch(Exception ex)
        //    {
        //        this.Err ="ImportToDatabase付值时候出错！"+ex.Message;
        //        this.WriteErr();
        //        return -1;
        //    }
			
        //    return this.InputBlob(sql,fileData);
        //}

        ///// <summary>
        ///// 输出文件 
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <param name="fileData"></param>
        ///// <returns></returns>
        //public int ExportFromDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt,ref byte[] fileData)
        //{
        //    string strSql ="",sql="";
        //    if(dt.ID == null||dt.ID =="") return -1;

        //    if(this.Sql.GetSql("Management.DataFile.ExportFromDatabase",ref strSql)==-1) return -1;
        //    try
        //    {
        //        sql = string.Format(strSql,dt.ID);
        //    }
        //    catch(Exception ex)
        //    {
        //        this.Err ="ExportFromDatabase付值时候出错！"+ex.Message;
        //        this.WriteErr();
        //        return -1;
        //    }
			
        //    fileData = this.OutputBlob(sql);
        //    return 0;
        //}

        ///// <summary>
        ///// 将文件导入到数据库中
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <param name="fileData">输入的文件数据</param>
        ///// <returns></returns>
        //public int ImportToDatabaseModual(Neusoft.HISFC.Models.File.DataFileInfo dt, byte[] fileData)
        //{
        //    string strSql = "", sql = "";
        //    if (dt.ID == null || dt.ID == "") return -1;

        //    if (this.Sql.GetSql("Management.DataFile.ImportToDatabase", ref strSql) == -1) return -1;
        //    try
        //    {
        //        sql = string.Format(strSql, dt.ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Err = "ImportToDatabase付值时候出错！" + ex.Message;
        //        this.WriteErr();
        //        return -1;
        //    }

        //    return this.InputBlob(sql, fileData);
        //}

        ///// <summary>
        ///// 输出文件 
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <param name="fileData"></param>
        ///// <returns></returns>
        //public int ExportFromDatabaseModual(Neusoft.HISFC.Models.File.DataFileInfo dt, ref byte[] fileData)
        //{
        //    string strSql = "", sql = "";
        //    if (dt.ID == null || dt.ID == "") return -1;

        //    if (this.Sql.GetSql("Management.DataFile.ExportFromDatabase", ref strSql) == -1) return -1;
        //    try
        //    {
        //        sql = string.Format(strSql, dt.ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Err = "ExportFromDatabase付值时候出错！" + ex.Message;
        //        this.WriteErr();
        //        return -1;
        //    }

        //    fileData = this.OutputBlob(sql);
        //    return 0;
        //}

        ///// <summary>
        ///// 将文件导入到数据库中
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <param name="fileData">输入的文件数据</param>
        ///// <returns></returns>
        //public int ImportToDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt,string fileData)
        //{
        //    string strSql ="",sql ="";
        //    if(dt.ID == null||dt.ID =="") return -1;

        //    if(this.Sql.GetSql("Management.DataFile.ImportToDatabase",ref strSql)==-1) return -1;
			
        //    try
        //    {
        //        sql = string.Format(strSql,dt.ID);
        //    }
        //    catch(Exception ex)
        //    {
        //        this.Err ="ImportToDatabase付值时候出错！"+ex.Message;
        //        this.WriteErr();
        //        return -1;
        //    }
        //    return this.InputLong(sql,fileData);
        //}

        ///// <summary>
        ///// 输出文件 
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <param name="fileData"></param>
        ///// <returns></returns>
        //public int ExportFromDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt,ref string fileData)
        //{
        //    string strSql ="",sql="";
        //    if(dt.ID == null||dt.ID =="") return -1;

        //    if(this.Sql.GetSql("Management.DataFile.ExportFromDatabase",ref strSql)==-1) return -1;
        //    try
        //    {
        //        sql = string.Format(strSql,dt.ID);
        //    }
        //    catch(Exception ex)
        //    {
        //        this.Err ="ExportFromDatabase付值时候出错！"+ex.Message;
        //        this.WriteErr();
        //        return -1;
        //    }
			
        //    fileData = this.ExecSqlReturnOne(sql);
        //    if(fileData =="-1") return -1;
        //    return 0;
        //}

        //#endregion
        #region 大字段操作

        /// <summary>
        /// 将文件导入到数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileData">输入的文件数据</param>
        /// <returns></returns>
        public int ImportToDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt, byte[] fileData)
        {
            string strSql = "", sql = "";
            if (dt.ID == null || dt.ID == "") return -1;
            if (dt.Type.Trim() == "0")//数据
            {
                if (this.Sql.GetSql("Management.DataFile.ImportToDatabase.byte", ref strSql) == -1) return -1;
            }
            else if (dt.Type.Trim() == "1") //模板
            {
                if (this.Sql.GetSql("Management.DataFile.ImportToDatabase.Modual.byte", ref strSql) == -1) return -1;
            }
            else
            {
                this.Err = "未知文件类型";
                return -1;
            }

            try
            {
                sql = string.Format(strSql, dt.ID);
            }
            catch (Exception ex)
            {
                this.Err = "ImportToDatabase付值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }

            return this.InputBlob(sql, fileData);
        }

        /// <summary>
        /// 输出文件 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public int ExportFromDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt, ref byte[] fileData)
        {
            string strSql = "", sql = "";
            if (dt.ID == null || dt.ID == "") return -1;

            if (dt.Type.Trim() == "0")//数据
            {
                if (this.Sql.GetSql("Management.DataFile.ExportFromDatabase.byte", ref strSql) == -1) return -1;
            }
            else if (dt.Type.Trim() == "1") //模板
            {
                if (this.Sql.GetSql("Management.DataFile.ExportFromDatabase.Modual.byte", ref strSql) == -1) return -1;
            }
            else
            {
                this.Err = "未知文件类型";
                return -1;
            }

            try
            {
                sql = string.Format(strSql, dt.ID);
            }
            catch (Exception ex)
            {
                this.Err = "ExportFromDatabase付值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }

            fileData = this.OutputBlob(sql);
            return 0;
        }



        /// <summary>
        /// 将文件导入到数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileData">输入的文件数据</param>
        /// <returns></returns>
        public int ImportToDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt, string fileData)
        {

            string strSql = "", sql = "";
            if (dt.ID == null || dt.ID == "") return -1;

            if (dt.Type.Trim() == "0")//数据
            {
                if (this.Sql.GetSql("Management.DataFile.ImportToDatabase", ref strSql) == -1) return -1;
            }
            else if (dt.Type.Trim() == "1") //模板
            {
                if (this.Sql.GetSql("Management.DataFile.ImportToDatabase.Modual", ref strSql) == -1) return -1;
            }
            else
            {
                this.Err = "未知文件类型";
                return -1;
            }

            try
            {
                sql = string.Format(strSql, dt.ID);
            }
            catch (Exception ex)
            {
                this.Err = "ImportToDatabase付值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.InputLong(sql, fileData);

        }

        /// <summary>
        /// 输出文件 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public int ExportFromDatabase(Neusoft.HISFC.Models.File.DataFileInfo dt, ref string fileData)
        {
            string strSql = "", sql = "";
            if (dt.ID == null || dt.ID == "") return -1;
            if (dt.Type.Trim() == "0")//数据
            {
                if (this.Sql.GetSql("Management.DataFile.ExportFromDatabase", ref strSql) == -1) return -1;
            }
            else if (dt.Type.Trim() == "1") //模板
            {
                if (this.Sql.GetSql("Management.DataFile.ExportFromDatabase.Modual", ref strSql) == -1) return -1;
            }
            else
            {
                this.Err = "未知文件类型";
                return -1;
            }

            try
            {
                sql = string.Format(strSql, dt.ID);
            }
            catch (Exception ex)
            {
                this.Err = "ExportFromDatabase付值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }

            fileData = this.ExecSqlReturnOne(sql);
            if (fileData == "-1") return -1;
            return 0;
        }

        #endregion


        #region add by lijp 2011-11-16 电子申请单添加

        /// <summary>
        /// 输出文件 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public int ExportFromDatabasePacsApply(Neusoft.HISFC.Models.File.DataFileInfo dt, ref byte[] fileData)
        {
            string strSql = "", sql = "";
            if (dt.ID == null || dt.ID == "") return -1;

            if (dt.Type.Trim() == "0")//数据
            {
                if (this.Sql.GetSql("Pacs.Management.DataFile.ExportFromDatabase.byte", ref strSql) == -1) return -1;
            }
            else if (dt.Type.Trim() == "1") //模板
            {
                if (this.Sql.GetSql("Management.DataFile.ExportFromDatabase.Modual.byte", ref strSql) == -1) return -1;
            }
            else
            {
                this.Err = "未知文件类型";
                return -1;
            }

            try
            {
                sql = string.Format(strSql, dt.ID);
            }
            catch (Exception ex)
            {
                this.Err = "ExportFromDatabase付值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }

            fileData = this.OutputBlob(sql);
            return 0;
        }

        /// <summary>
        /// 将文件导入到数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileData">输入的文件数据</param>
        /// <returns></returns>
        public int ImportToDatabasePacsApply(Neusoft.HISFC.Models.File.DataFileInfo dt, byte[] fileData)
        {
            string strSql = "", sql = "";
            if (dt.ID == null || dt.ID == "") return -1;
            if (dt.Type.Trim() == "0")//数据
            {
                if (this.Sql.GetSql("Pacs.Management.DataFile.ImportToDatabase.byte", ref strSql) == -1) return -1;
            }
            else if (dt.Type.Trim() == "1") //模板
            {
                if (this.Sql.GetSql("Management.DataFile.ImportToDatabase.Modual.byte", ref strSql) == -1) return -1;
            }
            else
            {
                this.Err = "未知文件类型";
                return -1;
            }

            try
            {
                sql = string.Format(strSql, dt.ID);
            }
            catch (Exception ex)
            {
                this.Err = "ImportToDatabase付值时候出错！" + ex.Message;
                this.WriteErr();
                return -1;
            }

            return this.InputBlob(sql, fileData);
        }

        /// <summary>
        /// 保存申请单
        /// </summary>
        /// <param name="pacsComList"></param>
        /// <param name="pacsList"></param>
        /// <param name="pacsItems"></param>
        /// <returns></returns>
        public int SavePacsApply(ArrayList pacsComList, ArrayList pacsList, ArrayList pacsItems)
        {
            //申请单主表
            int ret = UpdatePacsApply(pacsComList, pacsList);
            if (ret == -1) return -1;
            //if (ret >0) return 0;
            if (ret == 0)
            {
                ret = InsertPacsApply(pacsComList, pacsList);
                if (ret == -1) return -1;
            }
            if (pacsItems.Count != 0)
            {
                //全部删除
                if (DeletPacsItem(pacsComList[0].ToString()) == -1) return -1;
                foreach (string a in pacsItems)
                {
                    if (a != "" && a != string.Empty)
                    {
                        if (InsertPacsItem(pacsComList[0].ToString(), a) == -1) return -1;
                    }
                }
                if (pacsComList[3].ToString() == "1")
                {
                    string sql = @" Update Met_Ipm_Order
                                  Set Apply_NO='{0}' Where 
                                COMB_NO=( Select COMB_NO From Met_Ipm_Order Where MO_ORDER='{1}' And INPATIENT_NO='{2}' )
                                ";
                    sql = String.Format(sql, pacsComList[0].ToString(), pacsItems[0].ToString().Split(';')[0], pacsComList[2].ToString());
                    if (this.ExecNoQuery(sql) <= 0)
                        return -1;
                }
            }
            return 0;
        }

        /// <summary>
        /// 添加申请单信息
        /// </summary>
        /// <param name="pacsApp"></param>
        /// <returns></returns>
        private int InsertPacsApply(ArrayList pacsComList, ArrayList pacsList)
        {
            string[] arr = new string[]
            {
            pacsComList[0].ToString(),//申请单信息表唯一标识
            pacsComList[1].ToString(),//申请单标题
            pacsComList[2].ToString(),//流水号
            pacsComList[3].ToString(),//流水号标识;0门诊，1住院，2体检
            pacsComList[4].ToString(),//设备类型ID，也是判断申请单类型的根据
            pacsList[0].ToString(),//申请单诊断,即检查诊断
            pacsList[1].ToString(),//申请单病历史，分别针对不同的申请所对应的病史
            pacsList[2].ToString(),//申请单费用
            pacsList[3].ToString(),//是否收费（0：否，1：是）
            pacsList[4].ToString(),//申请医生
            pacsList[5].ToString(),//操作（确认）医生
            pacsList[6].ToString(),//执行科室

            pacsList[7].ToString(),//检查消耗时间
            pacsList[8].ToString()==""?this.Sql.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss"):pacsList[8].ToString(),//申请时间
            pacsList[9].ToString(),//取报告人
            pacsList[10].ToString(),//取报告人与患者的关系
            pacsList[11].ToString()==""?this.Sql.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss"):pacsList[11].ToString(),//预约检查时间

            //pacsList[12].ToString(),//申请单状态
            pacsComList[5].ToString(),//模板号
            pacsList[12].ToString(),
            pacsList[13].ToString(),
            pacsList[14].ToString()

             };
            string sql = string.Empty;
            if (this.Sql.GetSql("Pacs.PacsApplication.Insert", ref sql) == -1) return -1;
            sql = string.Format(sql, arr);
            int ret = this.ExecNoQuery(sql);
            if (ret == -1) return -1;
            return ret;
        }

        /// <summary>
        /// 修改申请单信息
        /// </summary>
        /// <param name="pacsApp"></param>
        /// <returns></returns>
        private int UpdatePacsApply(ArrayList pacsComList, ArrayList pacsList)
        {

            string[] arr = new string[]
            {
            pacsComList[0].ToString(),//申请单信息表唯一标识
            pacsComList[1].ToString(),//申请单标题
            pacsComList[2].ToString(),//流水号
            pacsComList[3].ToString(),//流水号标识;0门诊，1住院，2体检
            pacsComList[4].ToString(),//设备类型ID，也是判断申请单类型的根据
            pacsList[0].ToString(),//申请单诊断,即检查诊断
            pacsList[1].ToString(),//申请单病历史，分别针对不同的申请所对应的病史
            pacsList[2].ToString(),//申请单费用
            pacsList[3].ToString(),//是否收费（0：否，1：是）
            pacsList[4].ToString(),//申请医生
            pacsList[5].ToString(),//操作（确认）医生
            pacsList[6].ToString(),//执行科室
            pacsList[7].ToString(),//检查消耗时间
            pacsList[8].ToString()==""?this.Sql.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss"):pacsList[8].ToString(),//申请时间
            pacsList[9].ToString(),//取报告人
            pacsList[10].ToString(),//取报告人与患者的关系
            pacsList[11].ToString()==""?this.Sql.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm:ss"):pacsList[11].ToString(),//预约检查时间

            //pacsList[12].ToString(),//申请单状态
            pacsComList[5].ToString(),//模板号
            pacsList[12].ToString(),
            pacsList[13].ToString(),
            pacsList[14].ToString()
             };
            string sql = string.Empty;
            if (this.Sql.GetSql("Pacs.PacsApplication.Update", ref sql) == -1) return -1;
            sql = string.Format(sql, arr);
            int ret = this.ExecNoQuery(sql);
            if (ret == -1) return -1;
            return ret;
        }

        /// <summary>
        /// 插入检查项目
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private int InsertPacsItem(string applicationId, string item)
        {
            string[] arr1 = item.Split(';');

            string[] arr = new string[]
            {
               applicationId,
               arr1[2],  //ItemID
               Neusoft.FrameWork.Management.Connection.Operator.ID,
               new Neusoft.FrameWork.Management.DataBaseManger().GetSysDateTime().ToString(),
               arr1[0], //OrderID
               arr1[1], 
               arr1[3],  //检查方法
               arr1[4]   //检查部位
            };

            string sql = string.Empty;
            if (this.Sql.GetSql("Pacs.Items.Insert", ref sql) == -1) return -1;
            sql = string.Format(sql, arr);
            int ret = this.ExecNoQuery(sql);
            if (ret == -1) return -1;
            return ret;
        }

        /// <summary>
        /// 删除检查项目
        /// </summary>
        /// <param name="ApplicationID"></param>
        /// <returns></returns>
        private int DeletPacsItem(string ApplicationID)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql("Pacs.Items.DeleteByApplicationID", ref sql) == -1) return -1;
            sql = string.Format(sql, ApplicationID);
            int ret = this.ExecNoQuery(sql);
            if (ret == -1) return -1;
            return ret;
        }

        #endregion
    }
}
