using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    public class SpecOperatTissuManage : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// Speciment <br></br>
        /// [功能描述: 标本入库管理]<br></br>
        /// [创 建 者: 刘伊]<br></br>
        /// [创建时间: 2009-12-01]<br></br>
        /// <修改记录 
        ///		修改人='林国科' 
        ///		修改时间='2011-10-19' 
        ///		修改目的='版本转换4.5转5.0'
        ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
        ///  />
        /// </summary>
        public SpecOperatTissuManage()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
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
            sequence = this.GetSequence("Speciment.BizLogic.ShelfSpecManage.GetNextSequence");
            //
            // 如果返回NULL，则获取失败
            //
            if ( sequence == null )
            {
                this.Err="获取Sequence失败";
                return -1;
            }
            //
            // 成功返回
            //
            return 1;
        }

        #region 增加
        /// <summary>
        /// 插入一条手术标本信息
        /// </summary>
        /// <param name="ApplyTable"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Speciment.ApplyTable optisuinfo)
        {
            string sql = "";
            string sqdi = "";

            if (this.Sql.GetSql("Speciment.BizLogic.SpecOperatTissuManage.Insert", ref sql) == -1)
                return -1;

            int value = this.GetNextSequence(ref sqdi);
            if(value !=-1)
            {
                //this.optisuinfo.ID = sqdi;
            }
            else
            {
                return -1;
            }

            try
            {
                sql = string.Format(sql,
                    sqdi,
                    optisuinfo.ApplyId.ToString(),
                    optisuinfo.SubjectId +sqdi,
                    optisuinfo.ApplyUserId,  //标本序号
                    optisuinfo.ApplyUserName,//标本名称 
                    optisuinfo.ImpDocId,
                    optisuinfo.ImpName,
                    optisuinfo.FundName,
                    optisuinfo.FundId,//病人住院流水号 
                    optisuinfo.SubjectName,//病人病历号 
                    optisuinfo.DeptId,
                    optisuinfo.DeptName,
                    optisuinfo.OutPutTime.ToString(),
                    optisuinfo.OutPutOperDoc,
                    optisuinfo.OutPutResult,//操作人编码
                    optisuinfo.SepcAdmDate.ToString(),//打印时间
                    optisuinfo.SpecAdmComment,//打印人
                    optisuinfo.SpecCountInDpet,//打印人编码
                    optisuinfo.SpecIsCancer,
                    optisuinfo.Memo,
                    optisuinfo.User01,
                    optisuinfo.User02,
                    optisuinfo.User03
                    );
            }
            catch ( Exception e )
            {
                this.Err = "[Speciment.BizLogic.SpecOperatTissuManage.Insert]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据id删除一条手术标本信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int Delete(string applyNo)
        {
            string sql = "";

            if (this.Sql.GetSql("Speciment.BizLogic.SpecOperatTissuManage.Delete", ref sql) == -1)
                return -1;

            try
            {
                sql = string.Format(sql, applyNo);
            }
            catch ( Exception e )
            {
                this.Err = "[Speciment.BizLogic.SpecOperatTissuManage.Delete]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新手术标本信息
        /// </summary>
        /// <param name="ApplyTable"></param>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.Speciment.ApplyTable optisuinfo)
        {
            string sql = "";

            if (this.Sql.GetSql("Speciment.BizLogic.SpecOperatTissuManage.Update", ref sql) == -1)
                return -1;

            try
            {
                sql = string.Format(sql,
                    optisuinfo.ID,
                    optisuinfo.SubjectId + optisuinfo.ID,
                    optisuinfo.ApplyUserId,  //标本序号
                    optisuinfo.ApplyUserName,//标本名称 
                    optisuinfo.DeptId,
                    optisuinfo.DeptName,
                    optisuinfo.OutPutTime.ToString(),
                    optisuinfo.OutPutOperDoc,
                    optisuinfo.OutPutResult,//操作人编码
                    optisuinfo.SepcAdmDate.ToString(),//打印时间
                    optisuinfo.SpecAdmComment,//打印人
                    optisuinfo.SpecCountInDpet,//打印人编码
                    optisuinfo.SpecIsCancer,
                    optisuinfo.Memo,
                    optisuinfo.User01,
                    optisuinfo.User02,
                    optisuinfo.User03);
            }
            catch ( Exception e )
            {
                this.Err = "[Speciment.BizLogic.SpecOperatTissuManage.Update]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        #endregion

        #region 查询
        private ArrayList al;
        private Neusoft.HISFC.Models.Speciment.ApplyTable optisuinfo;

        /// <summary>
        /// 按sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList QueryOptisuinfo(string inpatientno,string applyno)
        {
            string sql = "";


            if (this.Sql.GetSql("Speciment.BizLogic.SpecOperatTissuManage.Query", ref sql) == -1)
                return null;

            try
            {
                sql = string.Format(sql, applyno, inpatientno);
            }
            catch ( Exception e )
            {
                this.Err = "[Speciment.BizLogic.SpecOperatTissuManage.Query]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
                

            this.al = new ArrayList();
            this.ExecQuery(sql);
            try
            {   
                
                while ( this.Reader.Read() )
                {
                    this.optisuinfo = new Neusoft.HISFC.Models.Speciment.ApplyTable();
                    optisuinfo.ID = this.Reader[0].ToString();
                    optisuinfo.ApplyId = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[1].ToString());
                    optisuinfo.SubjectId =this.Reader[2].ToString();
                    optisuinfo.ApplyUserId =this.Reader[3].ToString();  //标本序号
                    optisuinfo.ApplyUserName=this.Reader[4].ToString();//标本名称 
                    optisuinfo.ImpDocId=this.Reader[5].ToString();
                    optisuinfo.ImpName=this.Reader[6].ToString();
                    optisuinfo.FundName=this.Reader[7].ToString();
                    optisuinfo.FundId=this.Reader[8].ToString();//病人住院流水号 
                    optisuinfo.SubjectName=this.Reader[9].ToString();//病人病历号 
                    optisuinfo.DeptId=this.Reader[10].ToString();
                    optisuinfo.DeptName=this.Reader[11].ToString();
                    optisuinfo.OutPutTime =DateTime.Parse(this.Reader[12].ToString());
                    optisuinfo.OutPutOperDoc=this.Reader[13].ToString();
                    optisuinfo.OutPutResult=this.Reader[14].ToString();//操作人编码
                    optisuinfo.SepcAdmDate=DateTime.Parse(this.Reader[15].ToString());//打印时间
                    optisuinfo.SpecAdmComment=this.Reader[16].ToString();//打印人
                    optisuinfo.SpecCountInDpet=this.Reader[17].ToString();//打印人编码
                    optisuinfo.SpecIsCancer=this.Reader[18].ToString();
                    optisuinfo.Memo=this.Reader[19].ToString();
                    optisuinfo.User01=this.Reader[20].ToString();
                    optisuinfo.User02=this.Reader[21].ToString();
                    optisuinfo.User03=this.Reader[22].ToString();
                    

                    this.al.Add(this.optisuinfo);
                }

                this.Reader.Close();
            }
            catch ( Exception e )
            {
                this.Err = "查询手术标本信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return al;
        }

        
        #endregion

       



    }
}
