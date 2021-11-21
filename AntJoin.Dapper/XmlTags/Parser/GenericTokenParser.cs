using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AntJoin.Dapper.XmlTags.Parser
{
  public class GenericTokenParser {

    //private string delimiter;
    private string openToken;
    private string closeToken;
    StringTokenizer parser;

    public GenericTokenParser(string openToken, string closeToken) {
      //this.delimiter = delimiter;
      this.openToken = openToken;
      this.closeToken = closeToken;

      parser = new StringTokenizer(null);
      //this.handler = handler;
    }

    public string parse(string delimiter,string text, Func<string,string> handler) {
      if(text==null || text.IndexOf(delimiter)<0) return text;

        StringBuilder builder = new StringBuilder();//语句及替换
        StringBuilder expression = new StringBuilder();//目标参数
        if (text.Length > 0) {

            //分析当前内容文本
            parser.Init(delimiter,text);
            var enumerator = parser.GetEnumerator();

            while (enumerator.MoveNext()) 
            {
                string token = (string)enumerator.Current;
                if(token.StartsWith(this.openToken)){//捕获到参数
                    int paramEnd = token.IndexOf(this.closeToken);
                    builder.Append(
                        handler(token.Substring(1,paramEnd-1))
                        );

                    builder.Append(token.Substring(paramEnd+1));
                }else
                    builder.Append(token);
            }
        }
        return builder.ToString();
    }
  }
}