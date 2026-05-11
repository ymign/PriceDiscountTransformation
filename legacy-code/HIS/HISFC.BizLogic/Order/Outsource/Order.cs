using System;
using System.Collections;
//{55BBD9DB-F5C9-4e0a-94E5-9F7FCB121350}
using System.Collections.Generic;
namespace Neusoft.HISFC.BizLogic.Order.Outsource
{
	/// <summary>
	/// Order 的摘要说明。
	/// 门诊医嘱
	/// </summary>
    public class Order : Neusoft.FrameWork.Management.Database
	{
		public Order()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		

		#region 基本操作，增删改

		/// <summary>
		/// 插入一条
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
        public int InsertOutsourceOrder(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.OutsourceOrder.Insert";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = this.myGetCommonSql(sql, order);
            if (sql == null) return -1;
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }

		
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
        public int UpdateOutsourceOrder(Neusoft.HISFC.Models.Order.OutPatient.Order order)
		{
            if (this.DeleteOutsourceOrder(order.Patient.ID, Neusoft.FrameWork.Function.NConvert.ToInt32(order.ID)) < 0)
            {
                return -1;//删除不成功
            }
            return this.InsertOutsourceOrder(order);
		}
		
		
		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="seeNo"></param>
		/// <param name="seqNo"></param>
		/// <returns></returns>
        public int DeleteOutsourceOrder(string clinicCode, int seqNo)
		{
            /*
             * DELETE 
             * FROM met_ord_recipedetail   --诊间处方明细表
                WHERE     see_no='{0}' and sequence_no = {1} 
                AND status = '0'
             * */

            string sql = "Order.OutPatient.OutsourceOrder.Delete";
			if(this.Sql.GetCommonSql(sql, ref sql) == -1)
			{
				this.Err = this.Sql.Err;
				return -1;
			}
			try
			{
                sql = string.Format(sql, clinicCode, seqNo);
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(sql);
		}

		#endregion

        #region 门诊医嘱变更表操作add by sunm

        public int InsertOutsourceOrderChangeInfo(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.OutsourceOrder.InsertChangeInfo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            sql = this.myGetCommonSql(sql, order);
            if (sql == null) return -1;
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }
        /// <summary>
        /// 更新医嘱变更纪录
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOutsourceOrderChangedInfo(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.OutsourceOrder.UpdateChangeInfo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            sql = System.String.Format(sql, order.DCOper.ID, order.SeeNO, order.SequenceNO);
            if (sql == null) return -1;
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }

        /// <summary>
        /// 作废医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOutsourceOrderBeCaceled(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.OutsourceOrder.CancelOrder";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = "Can't Find Sql:Order.OutPatient.OutsourceOrder.CancelOrder";
                return -1;
            }
            sql = System.String.Format(sql, order.ID);
            if (sql == null) return -1;
            return this.ExecNoQuery(sql);
        }

        #endregion

        #region 获得新的看诊序号
        /// <summary>
		/// 
		/// </summary>
		/// <param name="cardNo"></param>
		/// <returns></returns>
		public int GetNewSeeNo( string cardNo )
		{
			string sql = "Order.OutPatient.Order.GetNewSeeNo.1";
			if(this.Sql.GetCommonSql(sql,ref sql) == -1) 
			{
				this.Err = this.Sql.Err;
				return -1;
			}
			try
			{
				sql = string.Format(sql,cardNo);
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.WriteErr();
				return -1;
			}
			return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
		}
		#endregion

        /// <summary>
        /// 获得新医嘱组合序号
        /// </summary>
        /// <returns></returns>
        public string GetNewOrderComboID()
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Management.Order.GetComboID", ref sql) == -1) return null;
            string strReturn = this.ExecSqlReturnOne(sql);
            if (strReturn == "-1" || strReturn == "") return null;
            return strReturn;
        }


        #region  更新医嘱序号
        /// <summary>
        /// 更新医嘱序号
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="sortID"></param>
        /// <returns></returns>
        public int UpdateOrderSortID(string orderID, int sortID, string clinicCode)
        {
            string sql = "Order.OutPatient.Order.Update.UpdateOrderSortID.1";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                sql = string.Format(sql, orderID, sortID, clinicCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(sql);
        }

        #endregion


        #region 查询

        /// <summary>
		/// 查询执行医嘱--通过看诊序号查询
		/// </summary>
		/// <param name="seeNo"></param>
		/// <returns></returns>
        public ArrayList QueryOutsourceOrder(string seeNo)
		{
			string sql ="",sqlSelect = "",sqlWhere = "Order.OutPatient.Order.Query.Where.1";
			if(this.myGetSelectSql(ref sqlSelect) == -1)
			{
				this.Err = this.Sql.Err;
				return null;
			}
            if (this.Sql.GetCommonSql(sqlWhere, ref sqlWhere) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
			sql = sqlSelect + " " + sqlWhere;
			sql = string.Format (sql,seeNo);
            return this.myGetExecOrder(sql);			
		}

        /// <summary>
        /// 查询门诊处方
        /// </summary>
        /// <param name="clinicCode">门诊看诊流水号</param>
        /// <param name="seeNo">看诊序号</param>
        /// <returns></returns>
        public ArrayList QueryOutsourceOrderByClinicCode(string clinicCode)
        {
            return this.QueryOutsourceOrderBase("Order.OutPatient.OutsourceOrder.Query.ByClinicCode", clinicCode);
        }

        /// <summary>
        /// 根据whereIndex查询门诊处方
        /// </summary>
        /// <param name="SqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList QueryOutsourceOrderBase(string SqlIndex, params string[] args)
        {
            string sqlStr = "";
            if (this.myGetSelectSql(ref sqlStr) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(SqlIndex, ref SqlIndex) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            sqlStr = sqlStr + "\r\n" + SqlIndex;

            sqlStr = string.Format(sqlStr, args);

            return this.myGetExecOrder(sqlStr);
        }

        /// <summary>
        /// 根据处方号查询医嘱
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="recipeNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByRecipeNO(string clinicCode,string recipeNO)
        {
            return this.QueryOutsourceOrderBase("Order.OutPatient.Order.Query.Where.4", clinicCode, recipeNO);
        }

        /// <summary>
        /// 根据处方号查询医嘱
        /// </summary>
        /// <param name="recipeNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByRecipeNO(string recipeNO)
        {
            return this.QueryOutsourceOrderBase("Order.OutPatient.Order.Query.ByRecipeNO", recipeNO);
        }

		/// <summary>
		/// 查询一条医嘱
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.Order QueryOneOrder(string clinicCode, string sequenceNO)
        {
            ArrayList al = this.QueryOutsourceOrderBase("Order.OutPatient.Order.Query.Where.2", sequenceNO, clinicCode);
            if (al == null)
            {
                return null;
            }
            if (al.Count <= 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Order.OutPatient.Order;
        }
        		/// <summary>
		/// 查询一条医嘱
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.Order QueryOneOrder(string clinicCode, string sequenceNO,string recipeNO)
        {
            ArrayList al = this.QueryOutsourceOrderBase("Order.OutPatient.Order.Query.Where.8", sequenceNO, clinicCode,recipeNO);
            if (al == null)
            {
                return null;
            }
            if (al.Count <= 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Order.OutPatient.Order;
        }
        /// <summary>
        /// 批量查询门诊处方
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArrayList QueryBatchOrder(string clinicCode, string[] batchSeq)
        {
            string strBatchSeq = "''";
            for (int i = 0; i < batchSeq.Length; i++)
            {
                strBatchSeq += ",'" + batchSeq[i] + "'";
            }

            return this.QueryOutsourceOrderBase("Order.OutPatient.Order.BatchQuery.ByClinicAndSeq", clinicCode, strBatchSeq);
        }

        /// <summary>
        /// 根据医嘱序号查询一条医嘱
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.Order QueryOneOrder(string sequenceNO)
        {
            ArrayList al = this.QueryOutsourceOrderBase("Order.OutPatient.Order.Query.Where.7", sequenceNO);
            if (al == null)
            {
                return null;
            }
            if (al.Count <= 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Order.OutPatient.Order;
        }

		/// <summary>
		/// 获得看诊序号列表
		/// </summary>
		/// <param name="cardNo">门诊卡号</param>
		/// <returns></returns>
		public ArrayList QuerySeeNoListByCardNo(string cardNo)
		{
			string sql = "Order.OutPatient.Order.GetSeeNoList";
			if(this.Sql.GetCommonSql(sql,ref sql) == -1)
			{
				this.Err = this.Sql.Err;
				return null;
			}
			try
			{
				sql = string.Format(sql,cardNo);
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.WriteErr();
				return null;
			}
			if(this.ExecQuery(sql) == -1) return null;
			ArrayList al = new ArrayList();
			while(this.Reader.Read())
			{
				Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
				obj.ID = this.Reader[0].ToString();
				obj.Name = this.Reader[1].ToString();
				obj.Memo = this.Reader[2].ToString();
				try
				{
					obj.User01 = this.Reader[3].ToString();
					obj.User02  = this.Reader[4].ToString();
					obj.User03 = this.Reader[5].ToString();
				}
				catch{}
				al.Add(obj);
			}		
			this.Reader.Close();
			return al;
		}
		/// <summary>
		/// 获得看诊序号列表
		/// </summary>
		/// <param name="clinicNo"></param>
		/// <param name="cardNo"></param>
		/// <returns></returns>
		public ArrayList QuerySeeNoListByCardNo( string clinicNo, string cardNo )
		{
			string sql = "Order.OutPatient.Order.GetSeeNoList.2";
			if(this.Sql.GetCommonSql(sql,ref sql) == -1) return null;
			try
			{
				sql = string.Format(sql,clinicNo,cardNo);
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.WriteErr();
				return null;
			}
			if(this.ExecQuery(sql) == -1) return null;
			ArrayList al = new ArrayList();
			while(this.Reader.Read())
			{
				Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
				obj.ID = this.Reader[0].ToString();
				obj.Name = this.Reader[1].ToString();
				obj.Memo = this.Reader[2].ToString();
				try
				{
					obj.User01 = this.Reader[3].ToString();
                    obj.User02  = this.Reader[4].ToString();
                    if (Reader.FieldCount > 5)
                    {
                        obj.User03 = this.Reader[5].ToString();
                    }
				}
				catch{}
				al.Add(obj);
			}		
			this.Reader.Close();
			return al;
		}
		/// <summary>
		/// 查询看诊序号根据名子
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ArrayList QuerySeeNoListByName(string name)
		{
			string sql = "Order.OutPatient.Order.GetSeeNoList.Name";
			if(this.Sql.GetCommonSql(sql,ref sql) == -1)
			{
				this.Err = this.Sql.Err;
				return null;
			}
			try
			{
				sql = string.Format(sql,name);
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.WriteErr();
				return null;
			}
			if(this.ExecQuery(sql) == -1) return null;
			ArrayList al = new ArrayList();
			while(this.Reader.Read())
			{
				Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
				obj.ID = this.Reader[0].ToString();
				obj.Name = this.Reader[1].ToString();
				obj.Memo = this.Reader[2].ToString();
				try
				{
					obj.User01 = this.Reader[3].ToString();
					obj.User02  = this.Reader[4].ToString();
					obj.User03 = this.Reader[5].ToString();
				}
				catch{}
				al.Add(obj);
			}		
			this.Reader.Close();
			return al;
		}

        /// <summary>
        /// 取得药品处方号通过门诊号和看诊号
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <param name="seeNo"></param>
        /// <returns></returns>
        public ArrayList GetPhaRecipeNoByClinicNoAndSeeNo(string clinicNo, string seeNo)
        {
            string sql = "Order.Outsource.Order.GetPhaRecipeNoByClinicNoAndSeeNo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, clinicNo, seeNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            ArrayList alRecipe = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();
                
                alRecipe.Add(obj);
            }
            this.Reader.Close();
            return alRecipe;
        }

       /// <summary>
        /// 获取处方号通过门诊号和看诊号
       /// </summary>
       /// <param name="clinicNo"></param>
       /// <param name="seeNo"></param>
       /// <param name="flag">0：全部、1：药品、2非药品</param>
       /// <returns></returns>
        public IList<Neusoft.FrameWork.Models.NeuObject> GetRecipeNoByClinicNoAndSeeNo(string clinicNo, string seeNo,string flag)
        {
            string sql = "Order.OutPatient.Order.GetRecipeNoByClinicNoAndSeeNo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, clinicNo, seeNo, flag);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            IList<Neusoft.FrameWork.Models.NeuObject> iRecipe = new List<Neusoft.FrameWork.Models.NeuObject>();
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();

                iRecipe.Add(obj);
            }
            this.Reader.Close();
            return iRecipe;
        }



        /// <summary>
        /// 根据发票号获取处方信息
        /// </summary>
        /// <param name="invociceNo"></param>
        /// <returns></returns>
        public ArrayList QueryRecipeListByInvoiceNo(string invociceNo)
        {
            string sql = "Order.OutPatient.Order.QueryRecipeNOByInvoiceNo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, invociceNo);

                if (this.ExecQuery(sql) == -1)
                {
                    return null;
                }

                ArrayList alRecipe = new ArrayList();
                Neusoft.HISFC.Models.Base.Spell obj = null;
                while (this.Reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.Base.Spell();
                    //处方号
                    obj.ID = this.Reader[0].ToString();
                    //医生
                    obj.Name = this.Reader[1].ToString();
                    //操作时间
                    obj.Memo = this.Reader[2].ToString();
                    //卡号
                    obj.SpellCode = this.Reader[3].ToString();
                    //姓名
                    obj.WBCode = this.Reader[4].ToString();
                    //发票号
                    obj.UserCode = this.Reader[5].ToString();

                    alRecipe.Add(obj);
                }
                return alRecipe;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            finally
            {
                if (this.Reader != null && !this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

		#endregion

        #region 私有函数

        /// <summary>
		/// 获得sql，传入参数
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="order"></param>
		/// <returns></returns>
        protected string myGetCommonSql(string sql, Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            #region sql
            //   0--看诊序号 ,1 --项目流水号,2 --门诊号,3   --病历号 ,4    --挂号日期
            //   5 --挂号科室,6   --项目代码,7   --项目名称, 8  --规格, 9  --1药品，2非药品
            //   10   --系统类别,   --最小费用代码,   --单价,   --开立数量,   --付数
            //    --包装数量,   --计价单位,   --自费金额0,   --自负金额0,   --报销金额0
            //   --基本剂量,   --自制药,   --药品性质，普药、贵药,   --每次用量
            //     --每次用量单位,   --剂型代码,   --频次,   --频次名称,   --使用方法
            //     --用法名称,   --用法英文缩写,   --执行科室代码,   --执行科室名称
            //      --主药标志,   --组合号,   --1不需要皮试/2需要皮试，未做/3皮试阳/4皮试阴
            //     --院内注射次数,   --备注,   --开立医生,   --开立医生名称,   --医生科室
            //     --开立时间,   --处方状态,1开立，2收费，3确认，4作废,   --作废人,   --作废时间
            //        --加急标记0普通/1加急,   --样本类型,   --检体,   --申请单号
            //     --0不是附材/1是附材,   --是否需要确认，1需要，0不需要,   --确认人
            //        --确认科室,   --确认时间,   --0未收费/1收费,   --收费员
            //       --收费时间,   --处方号,    --处方内流水号,     --发药药房，    
            //      --开立单位是否是最小单位 1 是 0 不是，      --医嘱类型（目前没有）
            #endregion

            //if(order.Item.IsPharmacy)//药品
            if (order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
            {
                Neusoft.HISFC.Models.Pharmacy.Item pItem = order.Item as Neusoft.HISFC.Models.Pharmacy.Item;
                //{9BAE643C-57BF-4dc5-889E-6B5F6B3E1E38} 由于接入电子申请单，apply_no字段赋order.ApplyNo20100505 yangw
                System.Object[] s = {
                                        order.SeeNO ,                                        
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.ID),
                                        order.Patient.ID,                                        
                                        order.Patient.PID.CardNO,                                        
                                        order.RegTime,                                        
										order.InDept.ID,                                        
                                        pItem.ID,                                        
                                        pItem.Name,                                        
                                        pItem.Specs,                                        
                                        "1",                                        
										order.Item.SysClass.ID,                                        
                                        order.Item.MinFee.ID,                                        
                                        order.Item.Price,
                                        order.Qty,
                                        order.HerbalQty,                                        
										pItem.PackQty,
                                        pItem.PriceUnit,
                                        order.FT.OwnCost ,
                                        order.FT.PayCost,
                                        order.FT.PubCost,                                        
										pItem.BaseDose,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(pItem.Product.IsSelfMade),
                                        pItem.Quality.ID,
                                        order.DoseOnce,                                        
										order.DoseUnit,
                                        pItem.DosageForm.ID,
                                        order.Frequency.ID,
                                        order.Frequency.Name,
                                        order.Usage.ID,                                        
										order.Usage.Name,
                                        order.Usage.Memo,
                                        order.ExeDept.ID,
                                        order.ExeDept.Name,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.Combo.IsMainDrug),
                                        order.Combo.ID,
                                        ((Int32)order.HypoTest).ToString(),
										order.InjectCount,
                                        order.Memo,
                                        order.ReciptDoctor.ID,
                                        order.ReciptDoctor.Name,
                                        order.ReciptDept.ID,                                        
										order.MOTime,                                        
                                        order.Status,
                                        order.DCOper.ID,
                                        order.DCOper.OperTime,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsEmergency),
                                        order.Sample.Name,
                                        order.CheckPartRecord,
                                        order.ApplyNo,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsSubtbl),
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsNeedConfirm),
                                        order.ConfirmOper.ID,                                        
										order.ConfirmOper.Dept.ID,
                                        order.ConfirmOper.OperTime,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsHaveCharged),
                                        order.ChargeOper.ID,                                        
										order.ChargeOper.OperTime,
                                        order.ReciptNO,
                                        order.SequenceNO,                                        
                                        order.StockDept.ID,
                                        order.MinunitFlag,
                                        order.UseDays.ToString(),
                                        order.SubCombNO,
                                        order.ExtendFlag1,                                        
										order.ReciptSequence,
                                        order.NurseStation.Memo,
                                        order.SortID,
                                        order.DoseOnceDisplay,
                                        order.DoseUnitDisplay,
                                        order.FirstUseNum,
                                        order.Patient.Pact.ID,
                                        order.Patient.Pact.PayKind.ID
                                    };

                try
                {
                    string sReturn = string.Format(sql, s);
                    return sReturn;
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            else//非药品
            {
                Neusoft.HISFC.Models.Fee.Item.Undrug pItem = order.Item as Neusoft.HISFC.Models.Fee.Item.Undrug;
                //{9BAE643C-57BF-4dc5-889E-6B5F6B3E1E38} 由于接入电子申请单，apply_no字段赋order.ApplyNo 20100505 yangw
                System.Object[] s = {
                                        order.SeeNO,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.ID),
                                        order.Patient.ID,
                                        order.Patient.PID.CardNO,
                                        order.RegTime,                                        
										order.InDept.ID,
                                        pItem.ID,
                                        pItem.Name,
                                        pItem.Specs,
                                        "2",                                        
										order.Item.SysClass.ID,
                                        order.Item.MinFee.ID,
                                        order.Item.Price,
                                        order.Qty,
                                        order.HerbalQty,                                        
										pItem.PackQty,
                                        pItem.PriceUnit,
                                        order.FT.OwnCost ,
                                        order.FT.PayCost,
                                        order.FT.PubCost,                                        
										"0",
                                        0,
                                        "",
                                        order.DoseOnce,                                        
										order.DoseUnit,
                                        "",
                                        order.Frequency.ID,
                                        order.Frequency.Name,
                                        order.Usage.ID,                                        
										order.Usage.Name,
                                        order.Usage.Memo,
                                        order.ExeDept.ID,
                                        order.ExeDept.Name,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.Combo.IsMainDrug),
                                        order.Combo.ID,
                                        ((Int32)order.HypoTest).ToString(),                                        
										order.InjectCount,
                                        order.Memo,
                                        order.ReciptDoctor.ID,
                                        order.ReciptDoctor.Name,
                                        order.ReciptDept.ID,                                        
										order.MOTime,                                        
                                        order.Status,
                                        order.DCOper.ID,
                                        order.DCOper.OperTime,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsEmergency),
                                        order.Sample.Name,
                                        order.CheckPartRecord,
                                        order.ApplyNo,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsSubtbl),
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsNeedConfirm),
                                        order.ConfirmOper.ID,                                        
										order.ConfirmOper.Dept.ID,
                                        order.ConfirmOper.OperTime,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsHaveCharged),
                                        order.ChargeOper.ID,                                        
										order.ChargeOper.OperTime,
                                        order.ReciptNO,
                                        order.SequenceNO,                                        
                                        order.StockDept.ID,
                                        order.MinunitFlag,
                                        "",                                        
                                        order.SubCombNO,
                                        order.ExtendFlag1,                                        
										order.ReciptSequence,
                                        order.NurseStation.Memo,
                                        order.SortID,
                                        order.DoseOnceDisplay,
                                        order.DoseUnitDisplay,
                                        order.FirstUseNum,
                                        order.Patient.Pact.ID,
                                        order.Patient.Pact.PayKind.ID
                                    };
                try
                {
                    string sReturn = string.Format(sql, s);
                    return sReturn;
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
        }

		
		/// <summary>
		/// 获得查询sql语句
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		protected int myGetSelectSql(ref string sql)
		{
            return this.Sql.GetCommonSql("Order.OutPatient.OutsourceOrder.Query.Select", ref sql);
		}
		

		/// <summary>
		/// 获得执行医嘱信息
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
        protected ArrayList myGetExecOrder(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;
            ArrayList al = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Order.OutPatient.Order order = new Neusoft.HISFC.Models.Order.OutPatient.Order();
                try
                {
                    order.SeeNO = this.Reader[0].ToString();
                    order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[1].ToString());//项目流水好
                    order.ID = this.Reader[1].ToString();//项目流水好
                    order.Patient.ID = this.Reader[2].ToString();//门诊号
                    order.Patient.PID.CardNO = this.Reader[3].ToString();//病历卡号
                    order.RegTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4]);//挂号日期
                    order.ReciptDept.ID = this.Reader[5].ToString();//挂号科室 编码
                    if (this.Reader[9].ToString() == "1")//药品
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item item = new Neusoft.HISFC.Models.Pharmacy.Item();
                        item.ID = this.Reader[6].ToString();
                        item.Name = this.Reader[7].ToString();
                        item.Specs = this.Reader[8].ToString();
                        item.SysClass.ID = this.Reader[10].ToString();
                        item.MinFee.ID = this.Reader[11].ToString();
                        item.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12]);
                        item.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20]);
                        item.DoseUnit = this.Reader[24].ToString();
                        item.Product.IsSelfMade = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[21]);
                        item.Quality.ID = this.Reader[22].ToString();
                        item.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[15]);
                        item.DosageForm.ID = this.Reader[25].ToString();
                        item.PriceUnit = this.Reader[16].ToString();

                        //{6DBBDC62-2303-4d97-85EF-8BA2A622117A} 拆分属性 xuc
                        item.SplitType = this.Reader[61].ToString();

                        order.Item = item;

                    }
                    else if (this.Reader[9].ToString() == "2")//非药品
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        item.ID = this.Reader[6].ToString();
                        item.Name = this.Reader[7].ToString();
                        item.Specs = this.Reader[8].ToString();
                        item.SysClass.ID = this.Reader[10].ToString();
                        item.MinFee.ID = this.Reader[11].ToString();
                        item.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12]);
                        item.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[15]);
                        item.PriceUnit = this.Reader[16].ToString();
                        order.Item = item;

                    }
                    else
                    {
                        this.Err = "读取met_ord_recipedetail，区分药品非药品出错，drug_flag=" + this.Reader[9].ToString();
                        this.WriteErr();
                        return null;
                    }
                    order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13]);
                    order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[14]);
                    order.Unit = this.Reader[16].ToString();
                    order.FT.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[17]);
                    order.FT.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[18]);
                    order.FT.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[19]);

                    order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[23]);
                    order.DoseUnit = this.Reader[24].ToString();

                    order.Frequency.ID = this.Reader[26].ToString();
                    order.Frequency.Name = this.Reader[27].ToString();
                    order.Usage.ID = this.Reader[28].ToString();
                    order.Usage.Name = this.Reader[29].ToString();
                    order.Usage.Memo = this.Reader[30].ToString();
                    order.ExeDept.ID = this.Reader[31].ToString();
                    order.ExeDept.Name = this.Reader[32].ToString();
                    order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[33]);
                    order.Combo.ID = this.Reader[34].ToString();
                    order.HypoTest = (Neusoft.HISFC.Models.Order.EnumHypoTest)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[35]);
                    order.InjectCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[36]);
                    order.Memo = this.Reader[37].ToString();
                    order.ReciptDoctor.ID = this.Reader[38].ToString();
                    order.ReciptDoctor.Name = this.Reader[39].ToString();
                    order.ReciptDept.ID = this.Reader[40].ToString();
                    //order.ReciptDept.Name =this.Reader[41].ToString();
                    order.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[41]);
                    order.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[42]);
                    order.DCOper.ID = this.Reader[43].ToString();
                    order.DCOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[44]);
                    order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[45]);
                    order.Sample.Name = this.Reader[46].ToString();
                    order.CheckPartRecord = this.Reader[47].ToString();
                    order.ApplyNo = this.Reader[48].ToString();
                    order.IsSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[49]);
                    order.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[50]);
                    order.ConfirmOper.ID = this.Reader[51].ToString();
                    order.ConfirmOper.Dept.ID = this.Reader[52].ToString();
                    order.ConfirmOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[53]);
                    order.IsHaveCharged = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[54]);
                    order.ChargeOper.ID = this.Reader[55].ToString();
                    order.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[56]);
                    order.ReciptNO = this.Reader[57].ToString();
                    order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[58]);
                    order.StockDept.ID = this.Reader[59].ToString();
                    order.MinunitFlag = this.Reader[60].ToString();//最小单位标志
                    order.UseDays = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[62]);//{08024C29-12FE-4629-B982-C50AE9034B82}
                    order.SubCombNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[63].ToString());//附材组合号（检验）
                    order.ExtendFlag1 = this.Reader[64].ToString();//接瓶信息
                    order.ReciptSequence = this.Reader[65].ToString();//收费序列
                    order.NurseStation.Memo = this.Reader[66].ToString();
                    order.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[67]);
                    #region {C3DF9328-3458-4bb4-895E-5B122B6582BB}

                    if (this.Reader[9].ToString() == "1")
                    {
                        order.DoseOnceDisplay = this.Reader[68].ToString();
                        if (order.DoseOnceDisplay.Length <= 0)
                            order.DoseOnceDisplay = order.DoseOnce.ToString();
                    }

                    order.DoseUnitDisplay = this.Reader[69].ToString();
                    order.FirstUseNum = this.Reader[70].ToString();

                    if (this.Reader.FieldCount > 71)
                    {
                        order.Patient.Pact.ID = Reader[71].ToString();
                    } 
                    if (this.Reader.FieldCount > 72)
                    {
                        order.Patient.Pact.PayKind.ID = Reader[72].ToString();
                    }

                    #endregion
                    #region sql
                    //		0--看诊序号 1  --项目流水号, 2  --门诊号, 3  --病历号 4 -挂号日期  5,   --挂号科室
                    //       6,   --项目代码 7,   --项目名称 8,   --规格 9,   --1药品，2非药品 10,   --系统类别
                    //       11,   --最小费用代码 12,   --单价 13,   --开立数量 14 ,   --付数  15,   --包装数量
                    //       16,   --计价单位 17,   --自费金额 18  --自负金额 19,   --报销金额 20,   --基本剂量
                    //       21,   --自制药 2  --药品性质，普药、贵药 23,   --每次用量 24,   --每次用量单位 25,   --剂型代码
                    //       26,   --频次 27,   --频次名称 28 --使用方法 29,   --用法名称 30,   --用法英文缩写
                    //       31,   --执行科室代码 32,   --执行科室名称 33   --主药标志 34   --组合号 35   --1不需要皮试/2需要皮试，未做/3皮试阳/4皮试阴
                    //       36,   --院内注射次数 37   --备注  38,   --开立医生 39,   --开立医生名称 40,   --医生科室
                    //       41,   --开立时间   42,   --处方状态,1开立，2收费，3确认，4作废    43,   --作废人   44,   --作废时间    45,   --加急标记0普通/1加急
                    //       46,   --样本类型    47,   --检体 48,   --申请单号    49,   --0没有附材/1带附材/2是附材     50,   --是否需要确认，1需要，0不需要
                    //       51,   --确认人     52,   --确认科室    53,   --确认时间    54,   --0未收费/1收费      55,   --收费员
                    //       56,   --收费时间      57,   --处方号      58    --处方内流水号
                    //  FROM met_ord_recipedetail   --诊间处方明细表
                    #endregion
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
                finally
                {
                    if (!this.Reader.IsClosed)
                    {
                        this.Reader.Close();
                    }
                }
                al.Add(order);
            }
            this.Reader.Close();
            return al;
        }


		#endregion


        /// <summary>
        /// 翻译皮试信息
        /// </summary>
        /// <param name="i"></param>
        /// <returns>1 [免试] 2 [需皮试] 3 [+] 4 [-]</returns>
        public string TransHypotest(Neusoft.HISFC.Models.Order.EnumHypoTest HypotestCode)
        {
            //return Neusoft.FrameWork.Public.EnumHelper.Current.GetName(HypotestCode);

            switch ((int)HypotestCode)
            {
                case 0:
                    //return "不需要皮试";
                    return "";
                case 1:
                    return "[免试]";
                case 2:
                    return "[需皮试]";
                case 3:
                    return "[+]";
                case 4:
                    return "[-]";
                default:
                    return "[免试]";
            }
        }
    }
}
