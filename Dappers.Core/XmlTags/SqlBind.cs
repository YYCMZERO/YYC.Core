using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Xml.Serialization;

namespace Dappers.XmlTags
{
    [XmlRoot("bind")]
    public class SqlBind
    {
        //protected static Logger log = LogManager.GetCurrentClassLogger();
        [XmlAttribute("name")]
        public String name;

        [XmlAttribute("value")]
        public String value;
        
        public Object evalValue(IDictionary<String,Object> param){
            throw new ApplicationException("未完成部分");
            //return ExpressionEvaluator.GetValue(null,this.value, param);
        }
    }
}