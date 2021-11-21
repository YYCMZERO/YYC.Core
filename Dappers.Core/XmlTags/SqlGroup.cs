using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Xml.Serialization;

namespace Dappers.XmlTags
{
    [XmlRoot("sqlGroup")]
    public class SqlGroup
    {
        //protected static Logger log = LogManager.GetCurrentClassLogger();
        [XmlAttribute("name")]
        public String name;
        
        [XmlElement("sql")]
        public List<SqlItem> sqlItems;
    }
}