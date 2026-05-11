using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy
{
    /// <summary>
    /// [功能描述: 系统药品性质管理类]<br></br>
    /// [创 建 者: cube]<br></br>
    /// [创建时间: 2011-02]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    ///  ID 申请序号
    /// </summary>
    [Serializable]
    public class DrugUsageEnumService : Neusoft.HISFC.Models.Base.EnumServiceBase
    {
        public DrugUsageEnumService()
        {
            this.Items[EnumDrugType.IAST] = "皮试";
            this.Items[EnumDrugType.IH] = "皮下注射";
            this.Items[EnumDrugType.IM] = "肌注";
            this.Items[EnumDrugType.IV] = "静注";
            this.Items[EnumDrugType.IVD] = "静滴";
            this.Items[EnumDrugType.IZ] = "肿瘤注射";
            this.Items[EnumDrugType.IO] = "其它注射";
            this.Items[EnumDrugType.PO] = "口服";
            this.Items[EnumDrugType.POF] = "口服磨粉";
            this.Items[EnumDrugType.PON] = "内服(含、冲、嚼、漱等)";
            this.Items[EnumDrugType.WY] = "外用(敷、涂、滴、擦、洗等)";
            this.Items[EnumDrugType.SGY] = "给药";
            this.Items[EnumDrugType.HJZ] = "草药水煎煮(口服、外用、坐浴等)";
            this.Items[EnumDrugType.HXJ] = "草药先煎";
            this.Items[EnumDrugType.HHX] = "草药后下";
            this.Items[EnumDrugType.HLJ] = "草药另煎";
            this.Items[EnumDrugType.HBJ] = "草药包煎";
            this.Items[EnumDrugType.HYH] = "草药烊化";
            this.Items[EnumDrugType.HCF] = "草药冲服";
            this.Items[EnumDrugType.HZZ] = "草药榨汁";
            this.Items[EnumDrugType.HS] = "草药其它特殊";
            this.Items[EnumDrugType.USS] = "手术";
            this.Items[EnumDrugType.UZL] = "治疗";
            this.Items[EnumDrugType.UHL] = "护理";
            this.Items[EnumDrugType.O1] = "一类其他";
            this.Items[EnumDrugType.O2] = "二类其他";
            this.Items[EnumDrugType.O3] = "三类其他";


            this.Items[EnumDrugType.IX] = "穴位注射";
            this.Items[EnumDrugType.UHY] = "化验";
            this.Items[EnumDrugType.UJC] = "检查";
            this.Items[EnumDrugType.NONE] = "非药品通用";

            this.Items[EnumDrugType.IMO] = "肌注其他";
            this.Items[EnumDrugType.IVO] = "静注其他";
            this.Items[EnumDrugType.IVDO] = "静滴其他";
        }
        #region 变量

        /// <summary>
        /// 类别
        /// </summary>
        EnumDrugType enumDrugType;

        /// <summary>
        /// 存储枚举定义
        /// </summary>
        protected static System.Collections.Hashtable items = new System.Collections.Hashtable();

        #endregion

        protected override System.Collections.Hashtable Items
        {
            get
            {
                return items;
            }
        }

        protected override Enum EnumItem
        {
            get
            {
                return this.enumDrugType;
            }
        }


        /// <summary>
        /// 返回中文  获得全部列表
        /// </summary>
        /// <returns>ArrayList(Quality)</returns>
        public static System.Collections.ArrayList List()
        {
            return (new System.Collections.ArrayList(GetObjectItems(items)));
        }
    }

    /// <summary>
    /// 系统定义药品性质
    /// </summary>
    public enum EnumDrugType
    {
        /// <summary>
        /// 皮试
        /// </summary>
        IAST = 0,
        /// <summary>
        /// 皮下注射
        /// </summary>
        IH = 1,
        /// <summary>
        /// 肌注
        /// </summary>
        IM = 2,
        /// <summary>
        /// 静注
        /// </summary>
        IV = 3,
        /// <summary>
        /// 静滴
        /// </summary>
        IVD = 4,
        /// <summary>
        /// 肿瘤注射
        /// </summary>
        IZ = 5,
        /// <summary>
        /// 其它注射
        /// </summary>
        IO = 6,
        /// <summary>
        /// 口服
        /// </summary>
        PO = 7,
        /// <summary>
        /// 磨粉后口服(打碎)
        /// </summary>
        POF = 8,
        /// <summary>
        /// 内服(含、冲、嚼、漱等)
        /// </summary>
        PON = 9,
        /// <summary>
        /// 外用(敷、涂、滴、擦、洗等)
        /// </summary>
        WY = 10,
        /// <summary>
        /// 给药
        /// </summary>
        SGY = 11,
        /// <summary>
        /// 草药水煎煮(口服、外用、坐浴等)
        /// </summary>
        HJZ = 12,
        /// <summary>
        /// 草药先煎
        /// </summary>
        HXJ = 13,
        /// <summary>
        /// 草药后下
        /// </summary>
        HHX = 14,
        /// <summary>
        /// 草药另煎
        /// </summary>
        HLJ = 15,
        /// <summary>
        /// 草药包煎
        /// </summary>
        HBJ = 16,
        /// <summary>
        /// 草药烊化
        /// </summary>
        HYH = 17,
        /// <summary>
        /// 草药冲服
        /// </summary>
        HCF = 18,
        /// <summary>
        /// 草药榨汁
        /// </summary>
        HZZ = 19,
        /// <summary>
        /// 草药其它特殊
        /// </summary>
        HS = 20,
        /// <summary>
        /// 手术
        /// </summary>
        USS = 21,
        /// <summary>
        /// 治疗
        /// </summary>
        UZL = 22,
        /// <summary>
        /// 护理
        /// </summary>
        UHL = 23,
        /// <summary>
        /// 一类其他
        /// </summary>
        O1 = 24,
        /// <summary>
        /// 二类其他
        /// </summary>
        O2 = 25,
        /// <summary>
        /// 三类其他
        /// </summary>
        O3 = 26,
        /// <summary>
        /// 穴位注射
        /// </summary>
        IX = 27,
        /// <summary>
        /// 化验
        /// </summary>
        UHY = 28,
        /// <summary>
        /// 检查
        /// </summary>
        UJC = 29,
        /// <summary>
        /// 非药品通用
        /// </summary>
        NONE = 30,
        /// <summary>
        /// 肌注 其他
        /// </summary>
        IMO = 31,
        /// <summary>
        /// 静注 其他
        /// </summary>
        IVO = 32,
        /// <summary>
        /// 静滴 其他
        /// </summary>
        IVDO = 33
    }
}
