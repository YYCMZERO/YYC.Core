using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Dappers.Mapping;

namespace Dappers
{
    /// <summary>
    /// 属性-列名映射关系
    /// </summary>
    public class MappingInfo
    {
        public static string GetSqlStatementByType(Type type, String crudType, string extTableName = null)
        {
            String key = type.Name + "." + crudType;
            if (!StatementParser.StatementCache.ContainsKey(key))
            {
                string sql = StatementParser.GetMappedStaticSql(key);
                if (sql.Equals(key))
                {
                    switch (crudType)
                    {
                        case "Update":
                            sql = MappingInfo.GetMappingInfo(type, extTableName).Update;
                            break;
                        case "Insert":
                            sql = MappingInfo.GetMappingInfo(type, extTableName).Insert;
                            break;
                        case "Delete":
                            sql = MappingInfo.GetMappingInfo(type).Delete;
                            break;
                        default:
                            throw new Exception("GetSqlStatementByType found Unknow CRUD key: " + key);
                    }
                }
                StatementParser.StatementCache.Insert(key, sql);
            }
            return (string)StatementParser.StatementCache.Get(key);
        }
        public static MappingInfo GetMappingInfo<T>()
        {
            return GetMappingInfo(typeof(T));
        }


        static Dappers.Context.ICache cacheMappings = new Dappers.Context.Impl.LruCache(null);
        /// <summary>
        /// 映射当前对象所有属性， 以及父类所有声明Linq ColumnAttribute的属性。 除非Column(Expression="Ignore")
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static MappingInfo GetMappingInfo(Type objType, string extTableName = null)
        {
            if (objType.Namespace == "System")
                throw new ArgumentOutOfRangeException("Type", "Trying to GetMappingInfo with type:'" + objType.Name + "'.");
            //return BuildMappingInfo(objType);//for debug
            if (!cacheMappings.ContainsKey(objType.AssemblyQualifiedName))
            {
                cacheMappings.Insert(objType.AssemblyQualifiedName, MappingInfo.CreateMappingInfo(objType, extTableName));
            }
            return (MappingInfo)cacheMappings.Get(objType.AssemblyQualifiedName);
        }

        static readonly string DEFAULT_PRIMARY_ID = "ID";
        public static MappingInfo CreateMappingInfo(Type objType, string extTableName = null)
        {
            var props = objType.GetTypeInfo().GetCustomAttributes(typeof(TableAttribute), true).ToArray();
            TableAttribute tableName = null;
            if (props != null && props.Length > 0)
                tableName = props[0] as TableAttribute;

            IDictionary<string, string> ids = new Dictionary<string, string>(4);
            IDictionary<string, string> recordversion = new Dictionary<string, string>(1);
            IDictionary<string, string> otherFields = new Dictionary<string, string>();
            IDictionary<string, string> extProps = new Dictionary<string, string>(4);
            bool hasMapColumn = false;//存在列映射？不能使用select *
            var properties = objType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.GetProperty);
            foreach (System.Reflection.PropertyInfo p in properties)
            {
                if (p.PropertyType.Namespace.Equals("System") || p.PropertyType.IsEnum())//only primitive types
                {
                    string key = null;

                    ColumnAttribute col = null;
                    object[] attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false).ToArray();
                    if (attrs.Length > 0) col = attrs[0] as ColumnAttribute;
                    if (col != null)
                    {
                        key = col.Name ?? p.Name;//mapping<original,mapped>?
                        if (!hasMapColumn)
                            hasMapColumn = (p.Name != key);//存在列映射？不能使用select *

                        if (col.IsPrimaryKey)
                            ids.Add(p.Name, key);
                        else if (col.IsVersion)
                            recordversion.Add(p.Name, key);
                        else if (col.Expression != "Ignore")
                            otherFields.Add(p.Name, key);

                        if (col.UpdateCheck == UpdateCheck.Never)
                            extProps.Add(p.Name, "UpdateNever");//already in otherFields
                    }
                    else //if (p.DeclaringType == p.ReflectedType)//only current class allow no LinqAttribute
                        otherFields.Add(p.Name, p.Name);//no mapping
                }
            }

            if (ids.Count == 0 && otherFields.ContainsKey(DEFAULT_PRIMARY_ID))
            {
                ids.Add(DEFAULT_PRIMARY_ID, otherFields[DEFAULT_PRIMARY_ID]);
                otherFields.Remove(DEFAULT_PRIMARY_ID);
            }
            if (tableName != null && ids.Count == 0)//未映射的不检查
                throw new ArgumentException("未映射主键，请重新检查对象：" + objType.FullName);

            return new MappingInfo(objType.FullName, tableName == null ? objType.Name : (extTableName == null ? tableName.Name : tableName.Name + "_" + extTableName), ids, recordversion, otherFields, extProps, hasMapColumn);
        }

        string TypeFullName;
        string tableName;
        public string Table
        {
            get
            {
                if (tableName == null)
                    throw new ArgumentNullException(TypeFullName, "需对象->SQL映射的类必须描述表名: 如 [System.Data.Linq.Mapping.Table(Name = 'MY_TABLE')]");
                return tableName;
            }
            set
            {
                tableName = value;
            }
        }
        IDictionary<string, string> allProps;
        //<original,mapped>
        public IDictionary<string, string> AllProperties
        {
            get
            {
                if (allProps == null)
                {
                    allProps = Fields.Concat(Ids).Concat(RecordVersion).ToDictionary(m => m.Key, m => m.Value);
                }
                return allProps;
            }
        }
        IDictionary<string, string> Fields;//<originalProp,mappedCol>
        IDictionary<string, string> RecordVersion;//<originalProp,mappedCol>
        IDictionary<string, string> Ids;//<originalProp,mappedCol>
        IDictionary<string, string> Props;//<originalProp,mappedCol>
        bool HasMapColumn = false;//存在列映射？不能使用select *
        public MappingInfo(string typeFullName, string table, IDictionary<string, string> ids, IDictionary<string, string> recordversion, IDictionary<string, string> otherFields, IDictionary<string, string> props, bool hasMapColumn)
        {
            TypeFullName = typeFullName;
            Fields = otherFields;
            RecordVersion = recordversion;
            Ids = ids;
            Table = table;
            Props = props;
            HasMapColumn = hasMapColumn;
        }
        string m_select, m_update, m_delete, m_insert, m_selectById;
        string GetParamEqual(string join, bool hasfirstJoin, IEnumerable<KeyValuePair<string, string>> mapping)
        {
            return GetParamEqual(join, hasfirstJoin, mapping, "`{0}`=" + StatementParser.PREFIX + "{1}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="join"></param>
        /// <param name="hasfirstJoin"></param>
        /// <param name="mapping"></param>
        /// <param name="format">0=column,1=orginal prop</param>
        /// <returns></returns>
        string GetParamEqual(string join, bool hasfirstJoin, IEnumerable<KeyValuePair<string, string>> mapping, string format)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in mapping)//<originalProp,mappedCol>
            {
                sb.Append(join);
                sb.AppendFormat(format, pair.Value, pair.Key);//{0}=:{1}, {0} as {1} ;0=column,1=original prop
            }
            if (hasfirstJoin || sb.Length == 0)
                return sb.ToString();
            return sb.Remove(0, join.Length).ToString();
        }

        /// <summary>
        /// 判断是否存在映射， 以生成select col1 as prop1,col2,col3
        /// </summary>
        /// <param name="join"></param>
        /// <param name="hasfirstJoin"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        string GetColumnAlias(string join, bool hasfirstJoin, IEnumerable<KeyValuePair<string, string>> mapping)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in mapping)//<originalProp,mappedCol>
            {
                sb.Append(join);
                if (pair.Value == pair.Key)
                    sb.Append(string.Format("`{0}`", pair.Value));
                else
                    sb.AppendFormat("`{0}` as `{1}`", pair.Value, pair.Key);//{0}=:{1}, {0} as {1} ;0=column,1=original prop
            }
            if (hasfirstJoin || sb.Length == 0)
                return sb.ToString();
            return sb.Remove(0, join.Length).ToString();
        }

        public string Update
        {
            get
            {
                if (m_update == null)
                    m_update = string.Format("update {0} set {1}{4} where {2}{3}",
                        Table,
                        GetParamEqual(",", false, Fields.Where(m => !Props.ContainsKey(m.Key))),
                        GetParamEqual(" and ", false, Ids),
                        GetParamEqual(" and ", true, RecordVersion),
                        RecordVersion.Count == 0 ? null :
                            string.Format(",{0}=" + StatementParser.PREFIX + "{1}+1"
                            , RecordVersion.Values.First()
                            , RecordVersion.Keys.First())
                    );
                return m_update;
            }
        }
        public string Delete
        {
            get
            {
                if (m_delete == null)
                    m_delete = string.Format("delete from {0} where {1}{2}",
                    Table,
                    GetParamEqual(" and ", false, Ids),
                    null);//GetParamEqual(" and ", true, RecordVersion));
                return m_delete;
            }
        }
        public string Insert
        {
            get
            {
                if (m_insert == null)
                    m_insert = string.Format("insert into {0} ({1},{2}{3}) values (" + StatementParser.PREFIX + "{4}," + StatementParser.PREFIX + "{5}{6})",
                        Table
                    , string.Join(",", Ids.Values.ToArray())
                    , string.Format("`{0}`", string.Join("`,`", Fields.Values.ToArray()))
                    , GetParamEqual(",", true, RecordVersion, "{0}")

                    , string.Join("," + StatementParser.PREFIX, Ids.Keys.ToArray())
                    , string.Join("," + StatementParser.PREFIX, Fields.Keys.ToArray())
                    , GetParamEqual(",", true, RecordVersion, StatementParser.PREFIX + "{1}"));
                return m_insert;
            }
        }
        public string Select
        {
            get
            {
                if (m_select == null)
                {
                    if (this.HasMapColumn)//存在列映射?
                        m_select = string.Format("select {0}{1}{2} from {3} ",
                            GetColumnAlias(",", false, Ids)
                            , GetColumnAlias(",", true, Fields)
                            , GetColumnAlias(",", true, RecordVersion)
                            , Table);
                    else//无映射字段
                        m_select = string.Format("select * from {0} ", Table);
                }

                return m_select;
            }
        }
        public string SelectById
        {
            get
            {
                if (m_selectById == null)
                {
                    if (Ids.Count != 1)
                        throw new Exception("Auto generate SelectById sql is not support in '" + this.Table + "' for multi PrimaryKeys or non-keys!");

                    m_selectById = string.Format("{0} where {1}=" + StatementParser.PREFIX + Ids.Values.First(),
                        this.Select,
                        Ids.Values.First());
                }
                return m_selectById;
            }
        }

        public string IdColumnName
        {
            get
            {
                if (Ids.Count > 1)
                    throw new Exception("Get Id-ColumnName is not support in '" + this.Table + "' with Multi PrimaryKeys!");
                return Ids.Values.First();
            }
        }
    }


}
