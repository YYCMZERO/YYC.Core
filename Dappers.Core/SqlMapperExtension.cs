using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Dapper
{

    partial class SqlMapper
    {
        /*
        /// <summary>
        /// 增加Clob格式支持
        /// Action... CreateParamInfoGenerator(Identity identity)
        /// if (prop.PropertyType == typeof(DbString))....
        /// else if (prop.PropertyType == typeof(string))//added by crabo
        /// ....GetMethod("AddStringParameter")
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static void AddStringParameter(IDbCommand cmd, string name, object value)
        {
            if (value == null)
            {
                if (cmd.CommandText.IndexOf("select", StringComparison.InvariantCultureIgnoreCase) == 0)//name like :name => name is null
                {
                    cmd.CommandText = System.Text.RegularExpressions.Regex.Replace(cmd.CommandText, @"(\S*)\W[@:]" + name, "is null", System.Text.RegularExpressions.RegexOptions.Multiline);
                    return;
                }
                value = DBNull.Value;
            }

            string val = value as string;
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            
            PatchStringToClob(p, val);

            cmd.Parameters.Add(p);

        }

        
        static void PatchStringToClob(IDbDataParameter p ,string val)
        {
            p.DbType = DbType.AnsiString;//where中使用索引,nvarchar 则不行
            if (val != null)// && val.Length <= 4000)//其余情况会自动转为ntext等类型
            {
                if (val.Length > 2000)//ORA-01461 CLOB LONG/LONG  bug: 2000-4000之间的String使用Clob类型
                {
                    object clobType = GetClob(p);
                    if (!string.Empty.Equals(clobType))//Oracle?
                        GetOracleDbTypeInfo(p).SetValue(p, clobType, null);//设置DbType为 NClob
                }
                else
                    p.Size = val.Length;//CLOB 不需要设置长度
            }
        }*/


        /// <summary>
        /// 处理存储过程out参数, 以及oracle cursor类型
        /// </summary>
        /// GridReader QueryMultiple(.....
        /// if (commandType == CommandType.StoredProcedure)//added by crabo
        ///      ParepareSPParam(cmd);
        public static DynamicParameters ParepareSPParam(IDbConnection conn,IDictionary<string,object> param)
        {
            DynamicParameters outParams=new DynamicParameters();

            foreach (KeyValuePair<string,object> kp in param)
            {
                if (kp.Key.StartsWith("out_"))
                {
                    if ("CURSOR".Equals(kp.Value))
                    {
                        object refCursor = GetRefCursor(conn);

                        if (string.Empty.Equals(refCursor))//当前加载的不是Oracle驱动
                            continue;//移除并忽略此参数
                        
                        outParams.Add(kp.Key,kp.Value,(DbType)refCursor,ParameterDirection.Output,null);
                    }else
                        outParams.Add(kp.Key,kp.Value,null,ParameterDirection.Output,null);//copy参数
                }else
                {
                    outParams.Add(kp.Key,kp.Value,null,null,null);//copy参数
                }
            }
            return outParams;
        }

        static object frefCursor=null;
        static object GetRefCursor(IDbConnection p)
        {
            if (frefCursor == null)
                GetOracleDbTypeInfo(p);
            return frefCursor;
        }
        static object fClob = null;
        static object GetClob(IDbConnection p)
        {
            if (fClob == null)
                GetOracleDbTypeInfo(p);
            return fClob;
        }
        static System.Reflection.PropertyInfo foracleDbTypeInfo;
        static System.Reflection.PropertyInfo GetOracleDbTypeInfo(IDbConnection p)
        {
            if (foracleDbTypeInfo == null)
            {
                Type pType = p.GetType();
                foracleDbTypeInfo = pType.GetProperty("OracleType");
                if (foracleDbTypeInfo != null)//Microsoft
                {
                    frefCursor = Enum.Parse(foracleDbTypeInfo.PropertyType, "Cursor");//RefCursor
                    fClob = Enum.Parse(foracleDbTypeInfo.PropertyType, "NClob");
                }
                else
                {
                    foracleDbTypeInfo = pType.GetProperty("OracleDbType");
                    if (foracleDbTypeInfo != null)//ODP.net
                    {
                        frefCursor = Enum.Parse(foracleDbTypeInfo.PropertyType, "RefCursor");//RefCursor
                        fClob = Enum.Parse(foracleDbTypeInfo.PropertyType, "NClob");
                    }
                    else //初始化
                    { 
                        frefCursor = string.Empty;
                        fClob = string.Empty;
                    }
                }
            }
            return foracleDbTypeInfo;
        }
    }
}