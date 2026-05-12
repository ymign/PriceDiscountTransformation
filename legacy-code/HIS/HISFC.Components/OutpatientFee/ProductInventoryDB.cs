using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Pharmacy;
using Neusoft.HISFC.Models.Pharmacy.Common;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Components.OutpatientFee
{
    public class ProductInventoryDB : Neusoft.FrameWork.Management.Database
    {
        public ProductInventory GetProductInventoryForApplyNumber(string applyNumber)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("ZDWY.YB.ProductInventory.1", ref sql) == -1)
            {
                this.Err = "未找到索引ZDWY.YB.ProductInventory.1";
                return null;
            }

            try
            {
                sql = string.Format(sql, applyNumber);
                this.ExecQuery(sql);
                var productInventory = new ProductInventory();
                var i = 0;
                while (Reader.Read())
                {
                    productInventory.Id = Guid.NewGuid().ToString();
                    productInventory.SerialNo = Reader[i].ToString(); i++;
                    productInventory.DataType = Reader[i].ToString(); i++;
                    productInventory.CardNo = Reader[i].ToString(); i++;
                    productInventory.Name = Reader[i].ToString(); i++;
                    productInventory.DrugCode = Reader[i].ToString(); i++;
                    productInventory.TradeName = Reader[i].ToString(); i++;
                    productInventory.ApplyNumber = Reader[i].ToString(); i++;
                    productInventory.MoOrder = Reader[i].ToString(); i++;
                    productInventory.MedListCode = Reader[i].ToString(); i++;
                    productInventory.FixMedInsHistId = Reader[i].ToString(); i++;
                    productInventory.FixMedInsHistName = Reader[i].ToString(); i++;
                    productInventory.FixMedInsBchNo = Reader[i].ToString(); i++;
                    productInventory.PrsCdRCertType = Reader[i].ToString(); i++;
                    productInventory.PrsCdRCertNo = Reader[i].ToString(); i++;
                    productInventory.PrsCdrName = Reader[i].ToString(); i++;
                    productInventory.PharCertType = Reader[i].ToString(); i++;
                    productInventory.PharCertNo = Reader[i].ToString(); i++;
                    productInventory.PharName = Reader[i].ToString(); i++;
                    productInventory.PharPracCertNo = Reader[i].ToString(); i++;
                    productInventory.HIFeeSetlType = Reader[i].ToString(); i++;
                    productInventory.SetlId = Reader[i].ToString(); i++;
                    productInventory.MdtrTsN = Reader[i].ToString(); i++;
                    productInventory.PsnNo = Reader[i].ToString(); i++;
                    productInventory.PsnCertType = Reader[i].ToString(); i++;
                    productInventory.PsnName = Reader[i].ToString(); i++;
                    productInventory.ManuLotNum = Reader[i].ToString(); i++;
                    productInventory.ManuDate = Reader[i].ToString(); i++;
                    productInventory.ExpyEnd = Reader[i].ToString(); i++;
                    productInventory.RxFlag = Reader[i].ToString(); i++;
                    productInventory.TrdnFlag = Reader[i].ToString(); i++;
                    productInventory.FinlTrnsPric = Reader[i].ToString(); i++;
                    productInventory.RxNo = Reader[i].ToString(); i++;
                    productInventory.RxCircFlag = Reader[i].ToString(); i++;
                    productInventory.RtalDocNo = Reader[i].ToString(); i++;
                    productInventory.StoOutNo = Reader[i].ToString(); i++;
                    productInventory.BchNo = Reader[i].ToString(); i++;
                    productInventory.DrugTracCodg = Reader[i].ToString(); i++;
                    productInventory.DrugProdBarc = Reader[i].ToString(); i++;
                    productInventory.ShelfPosi = Reader[i].ToString(); i++;
                    productInventory.SelRetnCnt = Reader[i].ToString(); i++;
                    productInventory.SelRetnTime = Reader[i].ToString(); i++;
                    productInventory.Memo = Reader[i].ToString(); i++;
                    productInventory.MdtrtSetlType = Reader[i].ToString(); i++;
                    productInventory.InsuplCadmDvs = Reader[i].ToString(); i++;
                    productInventory.MdtrtAreaAdmVs = Reader[i].ToString(); i++;
                    productInventory.OpterType = Reader[i].ToString(); i++;
                    productInventory.Opter = Reader[i].ToString(); i++;
                    productInventory.OpterName = Reader[i].ToString(); i++;
                    productInventory.FixMedInsCode = Reader[i].ToString(); i++;
                    productInventory.FixMedInsName = Reader[i].ToString(); i++;
                    productInventory.OpterCode = Reader[i].ToString(); i++;
                    productInventory.Opter_Name = Reader[i].ToString(); i++;
                    productInventory.SendType = Reader[i].ToString(); i++;
                    productInventory.SendFlag = Reader[i].ToString(); i++;
                    productInventory.CreatedCode = Reader[i].ToString(); i++;
                    productInventory.CreatedName = Reader[i].ToString(); i++;
                    productInventory.CertNo = Reader[i].ToString(); i++;
                    break;
                }

                return productInventory;
            }
            catch (Exception ex)
            {
                this.Err = "查询ProductInventory异常:" + ex.Message;
                return null;
            }
            finally
            {
                Reader.Close();
            }



        }

        public ProductInventory GetProductReturn(string clincCode, string recipeNo, string sequenceNo)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("ZDWY.YB.ProductReturn.1", ref sql) == -1)
            {
                this.Err = "未找到索引ZDWY.YB.ProductReturn.1";
                return null;
            }

            try
            {
                sql = string.Format(sql, clincCode,recipeNo,sequenceNo);
                this.ExecQuery(sql);
                var productInventory = new ProductInventory();
                var i = 0;
                while (Reader.Read())
                {
                    productInventory.Id = Guid.NewGuid().ToString();
                    productInventory.SerialNo = Reader[i].ToString(); i++;
                    productInventory.DataType = Reader[i].ToString(); i++;
                    productInventory.CardNo = Reader[i].ToString(); i++;
                    productInventory.Name = Reader[i].ToString(); i++;
                    productInventory.DrugCode = Reader[i].ToString(); i++;
                    productInventory.TradeName = Reader[i].ToString(); i++;
                    productInventory.ApplyNumber = Reader[i].ToString(); i++;
                    productInventory.MoOrder = Reader[i].ToString(); i++;
                    productInventory.MedListCode = Reader[i].ToString(); i++;
                    productInventory.FixMedInsHistId = Reader[i].ToString(); i++;
                    productInventory.FixMedInsHistName = Reader[i].ToString(); i++;
                    productInventory.FixMedInsBchNo = Reader[i].ToString(); i++;
                    productInventory.PrsCdRCertType = Reader[i].ToString(); i++;
                    productInventory.PrsCdRCertNo = Reader[i].ToString(); i++;
                    productInventory.PrsCdrName = Reader[i].ToString(); i++;
                    productInventory.PharCertType = Reader[i].ToString(); i++;
                    productInventory.PharCertNo = Reader[i].ToString(); i++;
                    productInventory.PharName = Reader[i].ToString(); i++;
                    productInventory.PharPracCertNo = Reader[i].ToString(); i++;
                    productInventory.HIFeeSetlType = Reader[i].ToString(); i++;
                    productInventory.SetlId = Reader[i].ToString(); i++;
                    productInventory.MdtrTsN = Reader[i].ToString(); i++;
                    productInventory.PsnNo = Reader[i].ToString(); i++;
                    productInventory.PsnCertType = Reader[i].ToString(); i++;
                    productInventory.PsnName = Reader[i].ToString(); i++;
                    productInventory.ManuLotNum = Reader[i].ToString(); i++;
                    productInventory.ManuDate = Reader[i].ToString(); i++;
                    productInventory.ExpyEnd = Reader[i].ToString(); i++;
                    productInventory.RxFlag = Reader[i].ToString(); i++;
                    productInventory.TrdnFlag = Reader[i].ToString(); i++;
                    productInventory.FinlTrnsPric = Reader[i].ToString(); i++;
                    productInventory.RxNo = Reader[i].ToString(); i++;
                    productInventory.RxCircFlag = Reader[i].ToString(); i++;
                    productInventory.RtalDocNo = Reader[i].ToString(); i++;
                    productInventory.StoOutNo = Reader[i].ToString(); i++;
                    productInventory.BchNo = Reader[i].ToString(); i++;
                    productInventory.DrugTracCodg = Reader[i].ToString(); i++;
                    productInventory.DrugProdBarc = Reader[i].ToString(); i++;
                    productInventory.ShelfPosi = Reader[i].ToString(); i++;
                    productInventory.SelRetnCnt = Reader[i].ToString(); i++;
                    productInventory.SelRetnTime = Reader[i].ToString(); i++;
                    productInventory.Memo = Reader[i].ToString(); i++;
                    productInventory.MdtrtSetlType = Reader[i].ToString(); i++;
                    productInventory.InsuplCadmDvs = Reader[i].ToString(); i++;
                    productInventory.MdtrtAreaAdmVs = Reader[i].ToString(); i++;
                    productInventory.OpterType = Reader[i].ToString(); i++;
                    productInventory.Opter = Reader[i].ToString(); i++;
                    productInventory.OpterName = Reader[i].ToString(); i++;
                    productInventory.FixMedInsCode = Reader[i].ToString(); i++;
                    productInventory.FixMedInsName = Reader[i].ToString(); i++;
                    productInventory.OpterCode = Reader[i].ToString(); i++;
                    productInventory.Opter_Name = Reader[i].ToString(); i++;
                    productInventory.SendType = Reader[i].ToString(); i++;
                    productInventory.SendFlag = Reader[i].ToString(); i++;
                    productInventory.CreatedCode = Reader[i].ToString(); i++;
                    productInventory.CreatedName = Reader[i].ToString(); i++;
                    productInventory.CertNo = Reader[i].ToString(); i++;
                    break;
                }

                return productInventory;
            }
            catch (Exception ex)
            {
                this.Err = "查询ProductInventory异常:" + ex.Message;
                return null;
            }
            finally
            {
                Reader.Close();
            }
        }

        public bool InsertProductInventory(ProductInventory p)
        {
            try
            {
                string strSql = string.Empty;

                strSql = @" insert into ZDWY_YB_PRODUCTINVENTORY p
(
p.id,
p.serialno,
p.data_type,
p.cardno,
p.name,
p.drug_code,
p.trade_name,
p.apply_number,
p.moorder,
p.medlistcodg,
p.fixmedinshilistid,
p.fixmedinshilistname,
p.fixmedinsbchno,
p.prscdrcerttype,
p.prscdrcertno,
p.prscdrname,
p.pharcerttype,
p.pharcertno,
p.pharname,
p.pharpraccertno,
p.hifeesetltype,
p.setlid,
p.mdtrtsn,
p.psnno,
p.psncerttype,
p.certno,
p.psnname,
p.manulotnum,
p.manudate,
p.expyend,
p.rxflag,
p.trdnflag,
p.finltrnspric,
p.rxno,
p.rxcircflag,
p.rtaldocno,
p.stooutno,
p.bchno,
p.drugtraccodg,
p.drugprodbarc,
p.shelfposi,
p.selretncnt,
p.selretntime,
p.selretnoptername,
p.memo,
p.mdtrtsetltype,
p.insuplcadmdvs,
p.mdtrtareaadmvs,
p.optertype,
p.opter,
p.optername,
p.fixmedinscode,
p.fixmedinsname,
p.send_type,
p.send_flag,
p.created_code,
p.created_name,
p.opter_code,
p.opter_name
)
values
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
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}',
'{28}',
'{29}',
'{30}',
'{31}',
'{32}',
'{33}',
'{34}',
'{35}',
'{36}',
'{37}',
'{38}',
'{39}',
'{40}',
'{41}',
'{42}',
'{43}',
'{44}',
'{45}',
'{46}',
'{47}',
'{48}',
'{49}',
'{50}',
'{51}',
'{52}',
'{53}',
'{54}',
'{55}',
'{56}',
'{57}',
'{58}'
) ";

                strSql = string.Format(strSql,
                    p.Id,
                    p.SerialNo,
                    p.DataType,
                    p.CardNo,
                    p.Name,
                    p.DrugCode,
                    p.TradeName,
                    p.ApplyNumber,
                    p.MoOrder,
                    p.MedListCode,
                    p.FixMedInsHistId,
                    p.FixMedInsHistName,
                    p.FixMedInsBchNo,
                    p.PrsCdRCertType,
                    p.PrsCdRCertNo,
                    p.PrsCdrName,
                    p.PharCertType,
                    p.PharCertNo,
                    p.PharName,
                    p.PharPracCertNo,
                    p.HIFeeSetlType,
                    p.SetlId,
                    p.MdtrTsN,
                    p.PsnNo,
                    p.PsnCertType,
                    p.CertNo,
                    p.PsnName,
                    p.ManuLotNum,
                    p.ManuDate,
                    p.ExpyEnd,
                    p.RxFlag,
                    p.TrdnFlag,
                    p.FinlTrnsPric,
                    p.RxNo,
                    p.RxCircFlag,
                    p.RtalDocNo,
                    p.StoOutNo,
                    p.BchNo,
                    p.DrugTracCodg,
                    p.DrugProdBarc,
                    p.ShelfPosi,
                    p.SelRetnCnt,
                    p.SelRetnTime,
                    p.SelRetnOperName,
                    p.Memo,
                    p.MdtrtSetlType,
                    p.InsuplCadmDvs,
                    p.MdtrtAreaAdmVs,
                    p.OpterType,
                    p.Opter,
                    p.OpterName,
                    p.FixMedInsCode,
                    p.FixMedInsName,
                    p.SendType,
                    p.SendFlag,
                    p.CreatedCode,
                    p.CreatedName,
                    p.OpterCode,
                    p.Opter_Name
                    );

                if (this.ExecNoQuery(strSql) < 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Err = "插入ProductInventory出现异常:" + ex.Message;
                return false;
            }
        }

        public List<DrugCodeMapping> GetDrugCodeMappingList()
        {
            string sql = @" select p.id, p.drug_code, p.identifier_code
  from pha_com_CodeMapping p
 where p.valid_flag = '1' ";

            try
            {
                this.ExecQuery(sql);
                var drugMappingList = new List<DrugCodeMapping>();
                DrugCodeMapping drugMapInfo;

                while (Reader.Read())
                {
                    var i = 0;
                    drugMapInfo = new DrugCodeMapping();
                    drugMapInfo.Id = Reader[i].ToString(); i++;
                    drugMapInfo.DrugCode = Reader[i].ToString(); i++;
                    drugMapInfo.IdentifierCode = Reader[i].ToString(); i++;
                    drugMappingList.Add(drugMapInfo);
                }

                return drugMappingList;
            }
            catch (Exception ex)
            {
                this.Err = "查询药品标识码对照关系出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

        }

        public List<Neusoft.FrameWork.Models.NeuObject> GetDrugList()
        {
            string sql = @" select p.drug_code,p.trade_name,p.custom_code from pha_com_baseinfo p where p.valid_state='1' ";

            try
            {
                this.ExecQuery(sql);
                var drugList = new List<NeuObject>();
                NeuObject drug;

                while (Reader.Read())
                {
                    var i = 0;
                    drug = new NeuObject();
                    drug.ID = Reader[i].ToString(); i++;
                    drug.Name = Reader[i].ToString(); i++;
                    drug.Memo = Reader[i].ToString(); i++;
                    drugList.Add(drug);
                }

                return drugList;
            }
            catch (Exception ex)
            {
                this.Err = "查询药品基本信息异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public bool InsertDrugCodeMapping(DrugCodeMapping d)
        {
            try
            {
                string strSql = string.Empty;

                strSql = @" insert into pha_com_codemapping p
(
p.id,
p.drug_code,
p.identifier_code,
p.valid_flag,
p.opter_time,
p.opter_code,
p.opter_name
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
sysdate,
'{4}',
'{5}'
) ";

                strSql = string.Format(strSql, d.Id, d.DrugCode, d.IdentifierCode, d.ValidFlag, d.OpterCode, d.OpterName);

                if (this.ExecNoQuery(strSql) < 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Err = "插入pha_com_codemapping出现异常:" + ex.Message;
                return false;
            }
        }

        public bool DeleteDrugCodeMappingInfo(string drugCode, string identifierCode)
        {
            try
            {
                var sql = " update pha_com_codemapping p set p.valid_flag='0' where p.drug_code='{0}' and p.identifier_code='{1}'  ";
                sql = string.Format(sql, drugCode, identifierCode);

                if (this.ExecNoQuery(sql) < 0)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                this.Err = "删除对照关系异常:" + ex.Message;
                return false;
            }
        }


    }
}
