using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Neusoft.HISFC.Models.PacsApply
{
    /// <summary>
    /// 标记申请单实体属性，根据电子病历节点名，赋值属性 
    /// /// </summary>
    public class NodeMapAttribute:Attribute
    {

        
        private string nodeName;

        

        private string dataType;

        /// <summary>
        /// 对应的节点名
        /// </summary>
        public string NodeName 
        {
            get 
            {
                return this.nodeName;
            }
            set 
            {
                nodeName = value;
            }
        }

        /// <summary>
        /// 数据类型 string decimal int bool
        /// </summary>
        public string DataType 
        {
            get 
            {
                return this.dataType;
            }
            set 
            {
                this.dataType = value;
            }
        }


        public NodeMapAttribute(string node, string type)
        {
            
            this.nodeName = node;
            this.dataType = type;
        }

    }
}
