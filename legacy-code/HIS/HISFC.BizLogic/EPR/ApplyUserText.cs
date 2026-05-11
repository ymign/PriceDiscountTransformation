using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.EPR
{
    /// <summary>
    /// 电子申请单的用户文本接口
    /// </summary>
    public class ApplyUserText:Neusoft.FrameWork.Management.Database
    {
        public ApplyUserText()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		/// <summary>
		/// 插入
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int Insert(Neusoft.HISFC.Models.Base.UserText obj)
		{
            if (string.IsNullOrEmpty(obj.ID))
            {
                obj.ID = this.GetUserTextSequence();
            }
            if (string.IsNullOrEmpty(obj.ID))
            {
                return -1;
            }
		
            string sql =string.Empty;
            if (this.Sql.GetSql("Manager.UserText.Insert", ref sql) == -1) return -1;
            if (this.ReplaceSql(obj, ref sql) == -1) return -1;
            if (this.ExecNoQuery(sql) < 0) return -1;


            string sqlBlob=string.Empty;
            if (this.Sql.GetSql("Manager.UserText.UpdateBolbColumn", ref sqlBlob) == -1) return -1;
            sqlBlob = string.Format(sqlBlob, obj.ID);
            byte[] data = Neusoft.FrameWork.Public.CompressionHelper.CompressString(obj.RichText);
            if (this.InputBlob(sqlBlob, data) < 0) return -1;
            return 0;
		}

		protected int ReplaceSql(Neusoft.HISFC.Models.Base.UserText obj,ref string sql)
		{
            try
            {
                Neusoft.FrameWork.Public.String.FormatString(sql, out sql, obj.ID, obj.Code, obj.Name, obj.Group.ID,
                    obj.Group.Name, obj.Text, obj.Memo, obj.SpellCode, obj.Type.ToString());
            }
			catch{return -1;}
			return 0;
		}

        public string GetUserTextSequence()
        {
            return this.GetSequence("Manager.UserText.GetSeq");
        }

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int Update(Neusoft.HISFC.Models.Base.UserText obj)
		{
			string sql =string.Empty;
            if (this.Sql.GetSql("Manager.UserText.Update", ref sql) == -1) return -1;
            if (this.ReplaceSql(obj, ref sql) == -1) return -1;
            if (this.ExecNoQuery(sql) < 0) return -1;

            string sqlBlob =string.Empty;
            if (this.Sql.GetSql("Manager.UserText.UpdateBolbColumn", ref sqlBlob) == -1) return -1;
            sqlBlob = string.Format(sqlBlob, obj.ID);
            byte[] data = Neusoft.FrameWork.Public.CompressionHelper.CompressString(obj.RichText);
            if (this.InputBlob(sqlBlob, data) < 0) return -1;
            return 0;
		}
		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public int Delete(string ID)
		{
			string sql ="Manager.UserText.Delete";
			if(this.Sql.GetSql(sql,ref sql)==-1) return -1;
			try
			{
				sql = string.Format(sql,ID);
			}
			catch{return -1;}
			if(this.ExecNoQuery(sql)<=0) return -1;
			return 0;
		}
		/// <summary>
		/// 更新使用次数
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int UpdateFrequency(string id,string operId) 
		{
			string sql ="Manager.UserText.UpdateFrequency";
			if(this.Sql.GetSql(sql,ref sql)==-1) return -1;
			sql = System.String.Format(sql,id,operId);
			return this.ExecNoQuery(sql) ;

		}
		/// <summary>
		/// 获得
		/// </summary>
		/// <param name="Code"></param>
		/// <returns></returns>
		public ArrayList GetList(string Code,int Type)
		{
			return this.GetList("AAAA",Code,Type);
//			string sql ="Manager.UserText.Select";
//			if(this.Sql.GetSql(sql,ref sql)==-1) return null;
//			try
//			{
//				sql = string.Format(sql,Code,Type);
//			}
//			catch{return null;}
//			if(this.ExecQuery(sql)==-1) return null;
//			ArrayList al =new ArrayList();
//			while(this.Reader.Read())
//			{
//				Neusoft.HISFC.Models.Base.UserText obj =  new Neusoft.HISFC.Models.Base.UserText();
//				obj.ID = this.Reader[0].ToString();
//				obj.Code= this.Reader[1].ToString();
//				obj.Name= this.Reader[2].ToString();
//				obj.Group.ID= this.Reader[3].ToString();
//				obj.Group.Name= this.Reader[4].ToString();
//				obj.Text= this.Reader[5].ToString();
//				obj.RichText= this.Reader[6].ToString();
//				obj.Memo = this.Reader[7].ToString();
//				obj.SpellCode= this.Reader[8].ToString();
//				obj.Type= Neusoft.NFC.Function.NConvert.ToInt32(this.Reader[9].ToString());
//				al.Add(obj);
//			}
//			return al;
		}
		/// <summary>
		/// 根据组id，类别、编码获取用户文本信息
		/// </summary>
		/// <param name="GroupId"></param>
		/// <param name="Code"></param>
		/// <param name="Type"></param>
		/// <returns></returns>
		public ArrayList GetList(string GroupId,string Code,int Type)
		{
			string sql ="Manager.UserText.Select";
			if(this.Sql.GetSql(sql,ref sql)==-1) return null;
			try
			{
				sql = string.Format(sql,GroupId,Code,Type);
			}
			catch{return null;}
			if(this.ExecQuery(sql)==-1) return null;
			ArrayList al =new ArrayList();
			while(this.Reader.Read())
			{
				Neusoft.HISFC.Models.Base.UserText obj =  new Neusoft.HISFC.Models.Base.UserText();
				obj.ID = this.Reader[0].ToString();
				obj.Code= this.Reader[1].ToString();
				obj.Name= this.Reader[2].ToString();
				obj.Group.ID= this.Reader[3].ToString();
				obj.Group.Name= this.Reader[4].ToString();
				obj.Text= this.Reader[5].ToString();
                obj.RichText = Neusoft.FrameWork.Public.CompressionHelper.DecompressString((byte[])this.Reader[6]);
				obj.Memo = this.Reader[7].ToString();
				obj.SpellCode= this.Reader[8].ToString();
				obj.Type= this.Reader[9].ToString();
				al.Add(obj);
			}
			return al;
		}
        /// <summary>
        /// 查找组别
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ArrayList GetGroupList(string code,string Type)
        {
            string sql = "Manager.UserText.SelectGroup";
            if(this.Sql.GetSql(sql,ref sql)==-1) return null;
            try
            {
                sql = string.Format(sql,code,Type);
            }
            catch { return null; }
            if (this.ExecQuery(sql) == -1) return null;
            ArrayList al = new ArrayList();
            Neusoft.FrameWork.Models.NeuObject obj = null;
            while (this.Reader.Read())
            {

                if (this.Reader[0] != DBNull.Value)
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.Name = this.Reader[0].ToString();
                    al.Add(obj);
                }
                else
                {
                    continue;
                }
                
            }
            return al;
        }
	}

   
}
