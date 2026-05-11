using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views.OutPatient
{
    public class ComObject
    {
        /// <summary>
        /// 编号
        /// </summary>
        private string id;

        /// <summary>
        /// 名称
        /// </summary>
        private string name;

        /// <summary>
        /// 备注
        /// </summary>
        private string memo;

        /// <summary>
        /// 编号
        /// </summary>
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo
        {
            get
            {
                return this.memo;
            }
            set
            {
                this.memo = value;
            }
        }
    }
}
