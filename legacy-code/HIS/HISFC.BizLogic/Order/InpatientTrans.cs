using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Order;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Order
{
    public class InpatientTrans : Neusoft.FrameWork.Management.Database
    {

        public int Insert(InpatientTransInfo info)
        {
            string sqlIndex = "HISFC.Compoments.Order.Trans.Insert";
            string sql = string.Empty;
            if (this.Sql.GetCommonSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到sql:HISFC.Compoments.Order.Trans.Insert";
                return -1;
            }
            sql = string.Format(sql, info.Id,
info.Transtype,
info.Inpatientno,
info.Patientno,
info.Name,
info.Sex,
info.Birthday,
info.Age,
info.Tel,
info.Diagcode,
info.Diagname,
info.Deptcode,
info.Deptname,
info.Indept,
info.Addr,
info.Outdate,
info.Note,
info.Ext1,
info.Ext2,
info.Opercode
);

            return this.ExecNoQuery(sql);
        }

        public int Update(InpatientTransInfo info)
        {
            /*P.ID='{0}',--1键
P.TRANSTYPE='{1}',--2转诊类别（0:上转；1:下转）
P.INPATIENTNO='{2}',--3住院流水号
P.PATIENTNO='{3}',--4住院号
P.NAME='{4}',--5患者姓名
P.SEX='{5}',--6性别
P.BIRTHDAY='{6}',--7出生日期
P.AGE='{7}',--8年龄
P.TEL='{8}',--9联系电话
P.DIAGCODE='{9}',--10出院诊断icd编码
P.DIAGNAME='{10}',--11出院诊断
P.DEPTCODE='{11}',--12转出科室编码
P.DEPTNAME='{12}',--13转出科室名称
P.INDEPT='{13}',--14下转单位名称
P.ADDR='{14}',--15患者住址
P.OUTDATE='{15}',--16转出日期
P.NOTE='{16}',--17备注
P.EXT1='{17}',--18拓展1
P.EXT2='{18}',--19拓展2
P.OPERCODE='{19}',--20操作人
P.OPERDATE='{20}',--21操作时间
//*/
            //            string sql = @" update FIN_IPR_TRANS a
            //                         set  set a.indept='{0}',operdate=sysdate
            //                          where a.inpatientno='{1}'";
            //            sql = string.Format(sql, info.Indept, info.Inpatientno);

            try
            {
                string sqlIndex = "HISFC.Compoments.Order.Trans.Update";
                string sql = string.Empty;
                if (this.Sql.GetCommonSql(sqlIndex, ref sql) == -1)
                {
                    this.Err = "没有找到sql:HISFC.Compoments.Order.Trans.Update";
                    return -1;
                }
                sql = string.Format(sql, info.Id,
    info.Transtype,
    info.Inpatientno,
    info.Patientno,
    info.Name,
    info.Sex,
    info.Birthday,
    info.Age,
    info.Tel,
    info.Diagcode,
    info.Diagname,
    info.Deptcode,
    info.Deptname,
    info.Indept,
    info.Addr,
    info.Outdate,
    info.Note,
    info.Ext1,
    info.Ext2,
    info.Opercode);
                return this.ExecNoQuery(sql);

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }


        }

        public List<InpatientTransInfo> QueryTrans(string strWhere)
        {
            string sqlIndex = "HISFC.Compoments.Order.Trans.Query";
            string sql = string.Empty;
            if (this.Sql.GetCommonSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到sql:HISFC.Compoments.Order.Trans.Query";
                return null;
            }
            List<InpatientTransInfo> list = new List<InpatientTransInfo>();
            this.ExecQuery(sql + strWhere);
            while (this.Reader.Read())
            {
                InpatientTransInfo info = new InpatientTransInfo();
                info.Id = this.Reader[0].ToString(); /*[键] */
                info.Transtype = this.Reader[1].ToString(); /*[转诊类别（0:上转；1:下转）] */
                info.Inpatientno = this.Reader[2].ToString(); /*[住院流水号] */
                info.Patientno = this.Reader[3].ToString(); /*[住院号] */
                info.Name = this.Reader[4].ToString(); /*[患者姓名] */
                info.Sex = this.Reader[5].ToString(); /*[性别] */
                info.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString()); /*[出生日期] */
                info.Age = this.Reader[7].ToString(); /*[年龄] */
                info.Tel = this.Reader[8].ToString(); /*[联系电话] */
                info.Diagcode = this.Reader[9].ToString(); /*[出院诊断icd编码] */
                info.Diagname = this.Reader[10].ToString(); /*[出院诊断] */
                info.Deptcode = this.Reader[11].ToString(); /*[转出科室编码] */
                info.Deptname = this.Reader[12].ToString(); /*[转出科室名称] */
                info.Indept = this.Reader[13].ToString(); /*[下转单位名称] */
                info.Addr = this.Reader[14].ToString(); /*[患者住址] */
                info.Outdate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString()); /*[转出日期] */
                info.Note = this.Reader[16].ToString(); /*[备注] */
                info.Ext1 = this.Reader[17].ToString(); /*[拓展1] */
                info.Ext2 = this.Reader[18].ToString(); /*[拓展2] */
                info.Opercode = this.Reader[19].ToString(); /*[操作人] */
                info.Operdate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[20].ToString()); /*[操作时间] */

                list.Add(info);

            }
            return list;
        }

        public ArrayList QueryTransDept()
        {
            HISFC.BizLogic.Manager.Constant s = new Neusoft.HISFC.BizLogic.Manager.Constant();
            return s.GetAllList("TransDept");
        }

        public bool IsPatientExist(string id)
        {
            string sql = "select count(*) from FIN_IPR_TRANS a where a.inpatientno='{0}'";
            sql = string.Format(sql, id);
            int s = int.Parse(this.ExecSqlReturnOne(sql));
            if (s > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
