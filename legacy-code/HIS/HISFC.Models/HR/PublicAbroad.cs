using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>全类名</br>
    /// <br>[功能描述: 公派出国实体类]</br>
    /// <br>[创 建 者: 赵阳]</br>
    /// <br>[创建时间: 2008-10-16]</br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class PublicAbroad : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 出国人员
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject personOut = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 所属科室
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject dept = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 职务
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject post = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 批件号
        /// </summary>
        private string approveNo = string.Empty;

        /// <summary>
        /// 政审号
        /// </summary>
        private string polityAuditNo = string.Empty;

        /// <summary>
        /// 有效期
        /// </summary>
        private decimal effectDuring = 0;

        /// <summary>
        /// 出国任务
        /// </summary>
        private string outDuty = string.Empty;

        /// <summary>
        /// 出国起始日期
        /// </summary>
        private DateTime outStartDate;

        /// <summary>
        /// 出国结束日期
        /// </summary>
        private DateTime outEndDate;

        /// <summary>
        /// 出国天数
        /// </summary>
        private decimal outDays = 0;

        /// <summary>
        /// 出生日期
        /// </summary>
        private DateTime birthdayDate;

        /// <summary>
        /// 操作员      
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性
        /// <summary>
        /// 操作员
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get
            {
                return oper;
            }
            set
            {
                oper = value;
            }
        }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime BirthdayDate
        {
            get
            {
                return birthdayDate;
            }
            set
            {
                birthdayDate = value;
            }
        }


        /// <summary>
        /// 出国天数
        /// </summary>
        public decimal OutDays
        {
            get
            {
                return outDays;
            }
            set
            {
                outDays = value;
            }
        }

        /// <summary>
        /// 出国结束日期
        /// </summary>
        public DateTime OutEndDate
        {
            get
            {
                return outEndDate;
            }
            set
            {
                outEndDate = value;
            }
        }


        /// <summary>
        /// 出国起始日期
        /// </summary>
        public DateTime OutStartDate
        {
            get
            {
                return outStartDate;
            }
            set
            {
                outStartDate = value;
            }
        }


        /// <summary>
        /// 出国任务
        /// </summary>
        public string OutDuty
        {
            get
            {
                return outDuty;
            }
            set
            {
                outDuty = value;
            }
        }


        /// <summary>
        /// 有效期
        /// </summary>
        public decimal EffectDuring
        {
            get
            {
                return effectDuring;
            }
            set
            {
                effectDuring = value;
            }
        }


        /// <summary>
        /// 政审号
        /// </summary>
        public string PolityAuditNo
        {
            get
            {
                return polityAuditNo;
            }
            set
            {
                polityAuditNo = value;
            }
        }


        /// <summary>
        /// 批件号
        /// </summary>
        public string ApproveNo
        {
            get
            {
                return approveNo;
            }
            set
            {
                approveNo = value;
            }
        }


        /// <summary>
        /// 职务
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Post
        {
            get
            {
                return post;
            }
            set
            {
                post = value;
            }
        }


        /// <summary>
        /// 所属科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Dept
        {
            get
            {
                return dept;
            }
            set
            {
                dept = value;
            }
        }


        /// <summary>
        /// 出国人员
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject PersonOut
        {
            get
            {
                return personOut;
            }
            set
            {
                personOut = value;
            }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new PublicAbroad Clone()
        {
            PublicAbroad publicAbr = base.Clone() as PublicAbroad;

            publicAbr.PersonOut = this.PersonOut.Clone();//出国人员
            publicAbr.Dept = this.Dept.Clone();//科室
            publicAbr.Post = this.Post.Clone();//职务
            publicAbr.Oper = this.Oper.Clone();//操作员

            return publicAbr;
        }
        #endregion
    }
}
