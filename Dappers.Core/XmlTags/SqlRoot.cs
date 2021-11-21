using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Xml.Serialization;

namespace Dappers.XmlTags
{
    [XmlRoot("sqlRoot")]
    public class SqlRoot
    {
        //protected static Logger log = LogManager.GetCurrentClassLogger();
        
         [XmlAttribute("source")]
        public String source;

        [XmlElement("sqlGroup")]
        public SqlGroup[] sqlGroups;

        IDictionary<string,SqlItem> items=null;
        public IDictionary<string,SqlItem> GetItems()
        {
            if(items==null)
            {
                items=new Dictionary<string,SqlItem>();
                foreach(SqlGroup gp in this.sqlGroups)
                {
                    foreach(SqlItem item in gp.sqlItems)
                    {
                        item.source=this.source;//数据库配置
                        items[gp.name+"."+item.id]=item;//groupName.sqlName
                    }
                }
            }
            return items;
        }
    }
}