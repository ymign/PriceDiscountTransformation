using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Components.OutpatientFee.BizLogic
{
    /// <summary>
    /// 审批项目
    /// </summary>
    public class ApproveItemBizLogic : Neusoft.FrameWork.Management.Database
    {
        private string[] getParam(ApproveItemModel approveItem)
        {
            return new string[] { 
            approveItem.RecipeNO,
            approveItem.SequenceNO.ToString(),
            approveItem.ID,
            approveItem.Name,
            approveItem.RegNO,
            approveItem.HosNO,
            approveItem.ApplyType,
            approveItem.Item.ID,
            approveItem.Item.Name,
            approveItem.Center.ID,
            approveItem.Center.Name,
            approveItem.Price.ToString("F4"),
            approveItem.Qty.ToString("F2"),
            approveItem.UseCode,
            Neusoft.FrameWork.Function.NConvert.ToInt32(approveItem.ImportFlag).ToString(),
            Neusoft.FrameWork.Function.NConvert.ToInt32(approveItem.HaveLocallyMaterialFlag).ToString(),
            approveItem.LocallyPrice.ToString("F4"),
            approveItem.HosOpinion,
            approveItem.ApplyReason,
            approveItem.Memo,
            approveItem.Oper.ID
            };
        }

        public int Insert(ApproveItemModel approveItem)
        {
            string sql = @"INSERT INTO LOCAL_FINOPB_APPROVEITEM
	                                    (
	                                    RECIPE_NO,
	                                    SEQUENCE_NO,
	                                    CLINIC_CODE,
	                                    NAME,
	                                    REG_NO,
	                                    HOS_NO,
	                                    APPLY_TYPE,
	                                    ITEM_CODE,
	                                    ITEM_NAME,
	                                    CENTER_CODE,
	                                    CENTER_NAME,
	                                    PRICE,
	                                    QTY,
	                                    USE_CODE,
	                                    IMPORT_FLAG,
	                                    HAVELOCALLYMATERIAL_FLAG,
	                                    LOCALLY_PRICE,
	                                    HOSOPINION,
	                                    APPLYREASON,
	                                    MEMO,
	                                    OPER_CODE,
	                                    OPER_DATE
	                                    )
                                    VALUES 
	                                    (
	                                    '{0}',
	                                    '{1}',
	                                    '{2}',
	                                    '{3}',
	                                    '{4}',
	                                    '{5}',
	                                    '{6}',
	                                    '{7}',
	                                    '{8}',
	                                    '{9}',
	                                    '{10}',
	                                    {11},
	                                    {12},
	                                    '{13}',
	                                    '{14}',
	                                    '{15}',
	                                    {16},
	                                    '{17}',
	                                    '{18}',
	                                    '{19}',
	                                    '{20}',
	                                    sysdate
	                                    )
                                    ";
            return this.ExecNoQuery(sql, getParam(approveItem));
        }

        public int Update(ApproveItemModel approveItem)
        {
            string sql = @"UPDATE LOCAL_FINOPB_APPROVEITEM
                                        SET 
	                                        CLINIC_CODE = '{2}',
	                                        NAME = '{3}',
	                                        REG_NO ='{4}',
	                                        HOS_NO ='{5}',
	                                        APPLY_TYPE = '{6}',
	                                        ITEM_CODE ='{7}',
	                                        ITEM_NAME = '{8}',
	                                        CENTER_CODE ='{9}',
	                                        CENTER_NAME = '{10}',
	                                        PRICE = {11},
	                                        QTY = {12},
	                                        USE_CODE = '{13}',
	                                        IMPORT_FLAG = '{14}',
	                                        HAVELOCALLYMATERIAL_FLAG = '{15}',
	                                        LOCALLY_PRICE = {16},
	                                        HOSOPINION ='{17}',
	                                        APPLYREASON = '{18}',
	                                        MEMO = '{19}',
	                                        OPER_CODE ='{20}',
	                                        OPER_DATE = SYSDATE
                                        WHERE RECIPE_NO = '{0}'
                                         AND  SEQUENCE_NO = {1}
                                        ";

            return this.ExecNoQuery(sql, getParam(approveItem));
        }

        public int Delete(string recipeNO, int sequenceNO)
        {
            string sql = @"DELETE FROM LOCAL_FINOPB_APPROVEITEM
                                        WHERE RECIPE_NO = '{0}' AND SEQUENCE_NO = {1}";
            return this.ExecNoQuery(sql, recipeNO, sequenceNO.ToString());
        }

        public decimal GetMaxPrice(string inpatientNO, string execID, string packageCode)
        {
            string sql = @"select max(a.unit_price) from fin_ipb_itemlist a where a.inpatient_no='{0}' and a.mo_exec_sqn='{1}' and a.trans_type='1' and a.package_code='{2}'";

            return Neusoft.FrameWork.Function.NConvert.ToDecimal(this.ExecSqlReturnOne(string.Format(sql, inpatientNO, execID, packageCode)));
        }

        public int GetMaxExportCount(string inpatientNO, DateTime dt)
        {
            string sql = @"select count(distinct export_filename) from local_finopb_approveitem a where a.clinic_code='{0}' and a.export_flag='1' and trunc(a.export_date)=to_date('{1}','yyyy-mm-dd hh24:mi:ss')";
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(string.Format(sql, inpatientNO, dt.ToString("yyyy-MM-dd HH:mm:ss"))));
        }

        public List<ApproveItemModel> Query(string inpatientNO)
        {
            string sql = @"SELECT 
	                                        RECIPE_NO,
	                                        SEQUENCE_NO,
	                                        CLINIC_CODE,
	                                        NAME,
	                                        REG_NO,
	                                        HOS_NO,
	                                        APPLY_TYPE,
	                                        ITEM_CODE,
	                                        ITEM_NAME,
	                                        CENTER_CODE,
	                                        CENTER_NAME,
	                                        PRICE,
	                                        QTY,
	                                        USE_CODE,
	                                        IMPORT_FLAG,
	                                        HAVELOCALLYMATERIAL_FLAG,
	                                        LOCALLY_PRICE,
	                                        HOSOPINION,
	                                        APPLYREASON,
	                                        MEMO,
	                                        OPER_CODE,
	                                        OPER_DATE
                                        FROM LOCAL_FINOPB_APPROVEITEM
                                        WHERE CLINIC_CODE='{0}'
                                        AND EXPORT_FLAG='0'
                                        ";

            if (this.ExecQuery(sql, new string[] { inpatientNO }) < 0)
            {
                return null;
            }

            List<ApproveItemModel> listApproveItem = null;
            if (this.Reader != null)
            {
                listApproveItem = new List<ApproveItemModel>();
                while (this.Reader.Read())
                {
                    ApproveItemModel approveItem = new ApproveItemModel();
                    approveItem.RecipeNO = this.Reader[0].ToString();
                    approveItem.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[1]);
                    approveItem.ID = this.Reader[2].ToString();
                    approveItem.Name = this.Reader[3].ToString();
                    approveItem.RegNO = this.Reader[4].ToString();
                    approveItem.HosNO = this.Reader[5].ToString();
                    approveItem.ApplyType = this.Reader[6].ToString();
                    approveItem.Item.ID = this.Reader[7].ToString();
                    approveItem.Item.Name = this.Reader[8].ToString();
                    approveItem.Center.ID = this.Reader[9].ToString();
                    approveItem.Center.Name = this.Reader[10].ToString();
                    approveItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[11]);
                    approveItem.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12]);
                    approveItem.UseCode = this.Reader[13].ToString();
                    approveItem.ImportFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[14]);
                    approveItem.HaveLocallyMaterialFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[15]);
                    approveItem.LocallyPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16]);
                    approveItem.HosOpinion = this.Reader[17].ToString();
                    approveItem.ApplyReason = this.Reader[18].ToString();
                    approveItem.Memo = this.Reader[19].ToString();
                    approveItem.Oper.ID = this.Reader[20].ToString();
                    approveItem.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[21]);

                    listApproveItem.Add(approveItem);
                }
            }

            return listApproveItem;
        }

        public int UpdateExportFlag(string inpatientNO, string fileName, DateTime dt)
        {
            string sql = @"update local_finopb_approveitem a 
                                       set a.export_flag='1',
                                             a.export_filename='{1}',
                                             a.export_date=to_date('{2}','yyyy-mm-dd hh24:mi:ss')
                                        where a.inpatient_no='{0}' and a.export_flag='0'";
            return this.ExecNoQuery(sql, inpatientNO, fileName, dt.ToString("yyyy-MM-dd HH:mm:ss"));

        }

        public ApproveItemModel Get(string reciepNO, int sequenceNO)
        {
            string sql = @"SELECT 
	                                        RECIPE_NO,
	                                        SEQUENCE_NO,
	                                        CLINIC_CODE,
	                                        NAME,
	                                        REG_NO,
	                                        HOS_NO,
	                                        APPLY_TYPE,
	                                        ITEM_CODE,
	                                        ITEM_NAME,
	                                        CENTER_CODE,
	                                        CENTER_NAME,
	                                        PRICE,
	                                        QTY,
	                                        USE_CODE,
	                                        IMPORT_FLAG,
	                                        HAVELOCALLYMATERIAL_FLAG,
	                                        LOCALLY_PRICE,
	                                        HOSOPINION,
	                                        APPLYREASON,
	                                        MEMO,
	                                        OPER_CODE,
	                                        OPER_DATE
                                        FROM LOCAL_FINOPB_APPROVEITEM
                                        WHERE RECIPE_NO='{0}'
                                        AND SEQUENCE_NO={1}
                                        ";

            if (this.ExecQuery(sql, new string[] { reciepNO, sequenceNO.ToString() }) < 0)
            {
                return null;
            }

            ApproveItemModel approveItem = null;
            if (this.Reader != null)
            {
                if (this.Reader.Read())
                {
                    approveItem = new ApproveItemModel();
                    approveItem.RecipeNO = this.Reader[0].ToString();
                    approveItem.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[1]);
                    approveItem.ID = this.Reader[2].ToString();
                    approveItem.Name = this.Reader[3].ToString();
                    approveItem.RegNO = this.Reader[4].ToString();
                    approveItem.HosNO = this.Reader[5].ToString();
                    approveItem.ApplyType = this.Reader[6].ToString();
                    approveItem.Item.ID = this.Reader[7].ToString();
                    approveItem.Item.Name = this.Reader[8].ToString();
                    approveItem.Center.ID = this.Reader[9].ToString();
                    approveItem.Center.Name = this.Reader[10].ToString();
                    approveItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[11]);
                    approveItem.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12]);
                    approveItem.UseCode = this.Reader[13].ToString();
                    approveItem.ImportFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[14]);
                    approveItem.HaveLocallyMaterialFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[15]);
                    approveItem.LocallyPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16]);
                    approveItem.HosOpinion = this.Reader[17].ToString();
                    approveItem.ApplyReason = this.Reader[18].ToString();
                    approveItem.Memo = this.Reader[19].ToString();
                    approveItem.Oper.ID = this.Reader[20].ToString();
                    approveItem.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[21]);
                }
            }

            return approveItem;

        }
    }

    /// <summary>
    /// 审批项目实体
    /// </summary>
    public class ApproveItemModel : Neusoft.FrameWork.Models.NeuObject
    {
        private string recipeNO;
        private int sequenceNO;

        public int SequenceNO
        {
            get { return sequenceNO; }
            set { sequenceNO = value; }
        }
        private string regNO;

        public string RegNO
        {
            get { return regNO; }
            set { regNO = value; }
        }
        private string hosNO;

        public string HosNO
        {
            get { return hosNO; }
            set { hosNO = value; }
        }
        private string applyType;

        public string ApplyType
        {
            get { return applyType; }
            set { applyType = value; }
        }
        private Neusoft.FrameWork.Models.NeuObject item = new Neusoft.FrameWork.Models.NeuObject();

        public Neusoft.FrameWork.Models.NeuObject Item
        {
            get { return item; }
            set { item = value; }
        }
        private Neusoft.FrameWork.Models.NeuObject center = new Neusoft.FrameWork.Models.NeuObject();

        public Neusoft.FrameWork.Models.NeuObject Center
        {
            get { return center; }
            set { center = value; }
        }
        private decimal price;

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        private decimal qty;

        public decimal Qty
        {
            get { return qty; }
            set { qty = value; }
        }
        private string useCode;

        public string UseCode
        {
            get { return useCode; }
            set { useCode = value; }
        }
        private bool importFlag;

        public bool ImportFlag
        {
            get { return importFlag; }
            set { importFlag = value; }
        }
        private bool haveLocallyMaterialFlag;

        public bool HaveLocallyMaterialFlag
        {
            get { return haveLocallyMaterialFlag; }
            set { haveLocallyMaterialFlag = value; }
        }
        private decimal locallyPrice;

        public decimal LocallyPrice
        {
            get { return locallyPrice; }
            set { locallyPrice = value; }
        }
        private string hosOpinion;

        public string HosOpinion
        {
            get { return hosOpinion; }
            set { hosOpinion = value; }
        }
        private string applyReason;

        public string ApplyReason
        {
            get { return applyReason; }
            set { applyReason = value; }
        }

        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get { return oper; }
            set { oper = value; }
        }

        public string RecipeNO
        {
            get
            {
                return recipeNO;
            }
            set
            {
                recipeNO = value;
            }
        }

    }
}
