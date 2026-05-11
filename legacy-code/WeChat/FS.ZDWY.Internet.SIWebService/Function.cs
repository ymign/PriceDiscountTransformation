using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace FS.ZDWY.Internet.SIWebService
{
    public class Function
    {
        /// <summary>
        /// 获取标签对的值
        /// </summary>
        /// <param name="xmlDoc">XML文档</param>
        /// <param name="xpath">xml表达式</param>
        /// <returns></returns>
        public static string GetNoteValue(XmlDocument xmlDoc, string xpath)
        {
            var node = xmlDoc.SelectSingleNode(xpath);
            if (node == null)
            {
                throw new Exception("没有找到xml节点。" + xpath);
            }
            return node.InnerText;
        }
    }
}