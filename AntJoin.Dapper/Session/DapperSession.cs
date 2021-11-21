using AntJoin.Core.DataAnnotations;
using AntJoin.Dapper.Cache;
using AntJoin.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AntJoin.Dapper.Session
{
    public class DapperSession : ISqlSession
    {
        private readonly IRedisClient _redisClient;
        private readonly IDao _dao;

        public DapperSession(IDao dao)
        {
            _dao = dao;
            _redisClient = L2CacheProvider.Get();
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void TxBegin() => _dao.TxBegin();

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void TxRollback() => _dao.TxRollback();

        /// <summary>
        /// 提交事务
        /// </summary>
        public void TxCommit() => _dao.TxCommit();

        /// <summary>
        /// 关闭连接和事务
        /// </summary>
        public void Close() => _dao.Close();

        public void Dispose()
        {
            _dao.Dispose();
        }

        ~DapperSession()
        {
            Dispose();
        }


        #region 判断该实体是否加入缓存读写

        /// <summary>
        /// 判断该实体是否加入缓存读写
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TableAttribute IsCacheReadWrite(Type type)
        {
            var props = type.GetTypeInfo().GetCustomAttributes(typeof(TableAttribute), true).ToArray();
            TableAttribute table = null;
            if (props.Length > 0)
            {
                table = props[0] as TableAttribute;
            }
            if (table != null && table.IsCacheReadWrite)
            {
                return table;
            }
            return null;
        }

        #endregion

        #region 获取主键名称

        /// <summary>
        /// 获取主键名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetPrimaryKey(IReflect type)
        {
            var primaryKey = string.Empty;

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
            foreach (var p in properties)
            {
                if (p.PropertyType.Namespace != null && (p.PropertyType.Namespace.Equals("System") || p.PropertyType.IsEnum()))
                {
                    ColumnAttribute col = null;
                    var attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false).ToArray();
                    if (attrs.Length > 0)
                    {
                        col = attrs[0] as ColumnAttribute;
                    }
                    if (col != null)
                    {
                        if (col.IsPrimaryKey)
                        {
                            primaryKey = col.Name;
                            break;
                        }
                    }
                }
            }
            if (primaryKey == string.Empty)
            {
                throw new ArgumentException("该表格未设置主键！");
            }
            return primaryKey;
        }

        #endregion

        #region 从redis缓存中移除该数据

        /// <summary>
        /// 从redis缓存中移除该数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        private async Task DeleteCacheInfo(MemberInfo type, object id)
        {
            if (id != null)
            {
                var isDeleted = false;
                var key = GetPrimaryRedisKey(type, id);
                if (_redisClient != null && await _redisClient.KeyExists(key))
                {
                    while (!isDeleted)
                    {
                        if (await _redisClient.KeyExists(key))
                        {
                            isDeleted = await _redisClient.KeyDelete(key);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从redis缓存中移除该数据
        /// </summary>
        /// <param name="obj"></param>
        private async Task DeleteCacheInfo(object obj)
        {
            var type = obj.GetType();
            var id = string.Empty;
            var table = IsCacheReadWrite(type);
            if (table != null)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
                foreach (var p in properties)
                {
                    if (p.PropertyType.Namespace != null && (p.PropertyType.Namespace.Equals("System") || p.PropertyType.IsEnum()))
                    {
                        ColumnAttribute col = null;
                        var attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false).ToArray();
                        if (attrs.Length > 0) col = attrs[0] as ColumnAttribute;
                        if (col != null)
                        {
                            if (col.IsPrimaryKey)
                            {
                                id = p.GetValue(obj, null)?.ToString();
                                break;
                            }
                        }
                    }
                }
                await DeleteCacheInfo(type, id);
            }
        }

        #endregion

        #region 从reids缓存中添加数据
        private async Task SetCacheInfo(MemberInfo type, object id, object data, int expireTime)
        {
            if (id == null || data == null)
            {
                return;
            }
            var key = GetPrimaryRedisKey(type, id);
            if (_redisClient != null)
            {
                if (expireTime > 0)
                {
                    await _redisClient.StringSet(key, data, new TimeSpan(0, 0, 0, expireTime));
                }
                else
                {
                    await _redisClient.StringSet(key, data, new TimeSpan(1, 0, 0));
                }
            }
        }

        private async Task SetCacheInfo(object obj)
        {
            var type = obj.GetType();
            var table = IsCacheReadWrite(type);
            object id = null;
            if (table != null && table.IsCacheReadWrite)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
                foreach (var p in properties)
                {
                    if (p.PropertyType.Namespace != null && (p.PropertyType.Namespace.Equals("System") || p.PropertyType.IsEnum()))
                    {
                        ColumnAttribute col = null;
                        var attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false).ToArray();
                        if (attrs.Length > 0)
                        {
                            col = attrs[0] as ColumnAttribute;
                        }
                        if (col != null)
                        {
                            if (col.IsPrimaryKey)
                            {
                                id = p.GetValue(obj, null)?.ToString();
                                break;
                            }
                        }
                    }
                }
                await SetCacheInfo(type, id, obj, table.CacheExpireTime);
            }
        }

        private async Task SetCacheInfo(object id, object obj)
        {
            var type = obj.GetType();
            var table = IsCacheReadWrite(type);
            if (table != null && table.IsCacheReadWrite)
            {
                await SetCacheInfo(type, id, obj, table.CacheExpireTime);
            }
        }
        

        /// <summary>
        /// 获取主键的Redis的Key
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetPrimaryRedisKey(MemberInfo type, object id)
        {
            return (_dao.Database + ":" + type.Name + ":" + id).ToLower();
        }
        #endregion


        #region 删除数据

        /// <summary>
        /// 根据id删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(object id)
        {
            if (id == null)
            {
                return false;
            }

            var type = typeof(T);
            if (IsCacheReadWrite(type) != null)
            {
                await DeleteCacheInfo(type, id);
            }

            var info = MappingInfo.GetMappingInfo<T>();
            return await _dao.ExecuteAsync(
                    info.Delete,
                    new Dictionary<string, object> { { GetPrimaryKey(type), id } }
                ) == 1;
        }
        

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(Type type, IDictionary<string, object> param)
        {
            var sql = await GetDeleteSql(type, param);
            return await _dao.ExecuteAsync(sql, param);
        }

        private async Task<string> GetDeleteSql(Type type, IDictionary<string, object> param)
        {
            var hasId = false;
            var primaryKey = GetPrimaryKey(type);
            var sql = MappingInfo.GetSqlStatementByType(type, "Delete");
            sql = sql.Substring(0, sql.IndexOf(" where ", StringComparison.CurrentCultureIgnoreCase) + 7);

            var where = "";
            foreach (var key in param.Keys)
            {
                if (string.Compare(key, primaryKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hasId = true;
                    await DeleteCacheInfo(type, param[key]);
                }

                where += string.Format(" and {0}={1}{0}", key, StatementParser.PREFIX);
            }
            if (IsCacheReadWrite(type) != null && !hasId)
            {
                throw new ArgumentException("该表格有加入缓存读写，但当前查询信息未包括主键字段！");
            }
            return sql + where.Substring(5);
        }
        
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            await DeleteCacheInfo(obj);
            var sql = MappingInfo.GetSqlStatementByType(obj.GetType(), "Delete");
            return await _dao.ExecuteAsync(sql, obj) == 1;
        }
        #endregion


        #region 插入数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="extTableName">扩展表名称</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(object obj, string extTableName = null)
        {
            if (obj == null)
            {
                return false;
            }

            var sql = MappingInfo.GetSqlStatementByType(obj.GetType(), "Insert", extTableName);
            return await _dao.ExecuteAsync(sql, obj) == 1;
        }
        

        /// <summary>
        /// 插入数据，返回ID
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="extTableName"></param>
        /// <returns></returns>
        public async Task<T> InsertAsync<T>(object obj, string extTableName = null)
        {
            T d = default;
            if (obj == null)
            {
                return d;
            }
            var t = obj.GetType();
            var sql = MappingInfo.GetSqlStatementByType(t, "Insert", extTableName);
            if (_dao.ConnectionManager.ConnectionTypeName.StartsWith("MySql."))
            {
                sql += ";SELECT LAST_INSERT_ID();";
            }

            var lastInsertId = await _dao.QueryFirstOrDefaultAsync<T>(sql, obj);
            return lastInsertId;
        }
        

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="li"></param>
        /// <param name="extTableName"></param>
        /// <returns>受影响行数</returns>
        public async Task<int> InsertAsync<T>(IList<T> li, string extTableName = null)
        {
            var affectedRows = 0;
            if (li == null || li.Count == 0)
            {
                return -1;
            }
            var sql = MappingInfo.GetSqlStatementByType(typeof(T), "Insert", extTableName);
            var paramNames = new List<string>();
            foreach (Match m in RegexGetParam.Matches(sql))
            {
                paramNames.Add(m.Value.Substring(1));
            }

            //为所有参数加入#号
            sql = RegexGetParam.Replace(sql, "$1#");
            var sb = new StringBuilder(sql);
            var values = sql.Substring(sql.IndexOf(" values", StringComparison.CurrentCultureIgnoreCase) + 7);
            var param = new Dictionary<string, object>();

            for (var i = 0; i < li.Count; i++)
            {
                //限制sql字符数
                if (sb.ToString().Length > 100000)
                {
                    affectedRows += await _dao.ExecuteAsync(sb.ToString(), param);
                    sb = new StringBuilder(sql);
                    param = new Dictionary<string, object>();
                    sb.Replace("#", i.ToString());
                }
                else if (i == 0)
                {
                    sb.Replace("#", i.ToString());
                }
                else
                {
                    sb.AppendLine(",").Append(values.Replace("#", i.ToString()));
                }

                foreach (var propname in paramNames)
                {
                    param.Add(propname + i, Context.Reflection.GetData(li[i], propname));
                }
            }

            if (sb.ToString().Length > 0)
            {
                affectedRows += await _dao.ExecuteAsync(sb.ToString(), param);
            }

            return affectedRows;
        }
        #endregion


        #region 修改数据
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="extTableName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(object obj, string extTableName = null)
        {
            if (obj == null)
            {
                return false;
            }

            //移除缓存
            await DeleteCacheInfo(obj);
            var sql = MappingInfo.GetSqlStatementByType(obj.GetType(), "Update", extTableName);
            return await _dao.ExecuteAsync(sql, obj) == 1;
        }
        
        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="li"></param>
        /// <param name="extTableName"></param>
        /// <returns>受影响行数</returns>
        public async Task<int> UpdateAsync<T>(IList<T> li, string extTableName = null)
        {
            if (li == null || li.Count == 0)
            {
                return -1;
            }

            var sql = MappingInfo.GetSqlStatementByType(typeof(T), "Update", extTableName);
            var paramNames = new List<string>();

            foreach (Match m in RegexGetParam.Matches(sql))
            {
                paramNames.Add(m.Value.Substring(1));
            }

            sql = RegexGetParam.Replace(sql, "$1#");//为所有参数加入#号
            var sb = new StringBuilder();
            var param = new Dictionary<string, object>(li.Count * paramNames.Count);

            for (var i = 0; i < li.Count; i++)
            {
                //移除缓存
                await DeleteCacheInfo(li[i]);

                sb.Append(sql.Replace("#", i.ToString())).AppendLine(";");
                foreach (var propname in paramNames)
                {
                    param.Add(propname + i, Context.Reflection.GetData(li[i], propname));
                }
            }
            return await _dao.ExecuteAsync(sb.ToString(), param);
        }
        

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="di"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(System.Collections.IDictionary di, QueryInfo<T> info)
        {
            var hasId = false;
            var type = typeof(T);
            var primaryKey = GetPrimaryKey(type);

            foreach (var key in info.Parameters.Keys)
            {
                if (string.Compare(key, primaryKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hasId = true;
                    await DeleteCacheInfo(type, info.Parameters[key]);
                }
            }
            if (IsCacheReadWrite(type) != null && !hasId)
            {
                throw new ArgumentException("该表格有加入缓存读写，但当前查询信息未包括主键字段！");
            }

            var update = info.GetSQLPartialUpdate(di);
            CheckNoParams(update, info.Parameters);

            return await _dao.ExecuteAsync(update, info.Parameters);
        }
        #endregion


        #region 直接执行操作
        /// <summary>
        /// 执行sql（注：有加入缓存读写的表格不可以使用该方法）
        /// </summary>
        /// <param name="mapSql"></param>
        /// <param name="param">支持Dictionary类型</param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string mapSql, IDictionary<string, object> param)
        {
            if (mapSql.IndexOf(' ') > 0)
            {
                CheckSqlInject(mapSql);
            }

            var sql = StatementParser.ParseDynamicSql(mapSql, param);
            CheckNoParams(sql, param);
            return await _dao.ExecuteAsync(sql, param);
        }
        
        /// <summary>
        /// 执行sql（注：有加入缓存读写的表格不可以使用该方法）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(QueryInfo info)
        {
            if (IsCacheReadWrite(info.MappingType.GetType()) != null)
            {
                throw new ArgumentException("该表格有加入缓存读写，无法使用该方法！");
            }
            GetDynamicOrDefaultSql(info);
            return await _dao.ExecuteAsync(info);
        }
        #endregion


        #region 查询操作
        /// <summary>
        /// 单一属性快捷查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SelectAsync<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value)
        {
            var info = new QueryInfo<T>().AddParam(prop, value);
            GetDynamicOrDefaultSql(info);
            return await _dao.QueryAsync<T>(info);
        }
        
        /// <summary>
        /// mapSql将检测配置节DB_ADAPTERS对应的key，动态构造并返回新的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql">如SysUser.List，动态处理{criteria [condition]}</param>
        /// <param name="param">必须为IDictionary类型才进行动态处理。 若不存在key,则不加载语句!</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SelectAsync<T>(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            var sql = AddParamToSqlWhere(typeof(T), mapSql, ref param);
            return await _dao.QueryAsync<T>(sql, param);
        }
        
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IEnumerable<object>> SelectAsync(QueryInfo info)
        {
            GetDynamicOrDefaultSql(info);
            return await _dao.QueryAsync(info);
        }
        
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SelectAsync<T>(QueryInfo<T> info)
        {
            GetDynamicOrDefaultSql(info);
            return await _dao.QueryAsync<T>(info);
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> SelectByIdAsync<T>(object id)
        {
            var sql = MappingInfo.GetMappingInfo<T>().SelectById;
            var type = typeof(T);
            var table = IsCacheReadWrite(type);
            var  isExistsCache = true;
            if (table != null && table.IsCacheReadWrite)
            {
                var key = GetPrimaryRedisKey(type, id);
                if (_redisClient != null && await _redisClient.KeyExists(key))
                {
                    return await _redisClient.StringGet<T>(key);
                }
                else
                {
                    isExistsCache = false;
                }
            }
            var data = await _dao.QuerySingleOrDefaultAsync<T>(sql, new Dictionary<string, object> { { GetPrimaryKey(typeof(T)), id } });
            if (!isExistsCache && data != null)
            {
                await SetCacheInfo(data);
            }
            return data;
        }
        
        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object> SelectByIdAsync(Type type, object id)
        {
            var info = new QueryInfo(type)
            {
                CustomSQL = MappingInfo.GetMappingInfo(type).SelectById
            };
            info.AddParam(GetPrimaryKey(type), id);
            return (await _dao.QueryAsync(info.ToSQLString(), info.Parameters, type)).SingleOrDefault();
        }
        
        
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> SelectSingleAsync<T>(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            string sql;
            if (param is string)
            {
                param = new Dictionary<string, object> { { GetPrimaryKey(typeof(T)), param } };
                sql = StatementParser.ParseDynamicSql(mapSql, (Dictionary<string, object>) param);
            }
            else
            {
                sql = AddParamToSqlWhere(typeof(T), mapSql, ref param);
            }

            return await _dao.QuerySingleAsync<T>(sql, param);
        }

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> SelectSingleAsync<T>(QueryInfo<T> info)
        {
            GetDynamicOrDefaultSql(info);
            return await _dao.QuerySingleAsync<T>(info);
        }

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> SelectSingleOrDefaultAsync<T>(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            string sql;
            if (param is string)
            {
                param = new Dictionary<string, object> { { GetPrimaryKey(typeof(T)), param } };
                sql = StatementParser.ParseDynamicSql(mapSql, (Dictionary<string, object>) param);
            }
            else
            {
                sql = AddParamToSqlWhere(typeof(T), mapSql, ref param);
            }

            return await _dao.QuerySingleOrDefaultAsync<T>(sql, param);
        }
        

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> SelectSingleOrDefaultAsync<T>(QueryInfo<T> info)
        {
            GetDynamicOrDefaultSql(info);
            return await _dao.QuerySingleOrDefaultAsync<T>(info);
        }
        
        
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> SelectFirstAsync<T>(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            string sql;
            if (param is string)
            {
                param = new Dictionary<string, object> { { GetPrimaryKey(typeof(T)), param } };
                sql = StatementParser.ParseDynamicSql(mapSql, (Dictionary<string, object>) param);
            }
            else
            {
                sql = AddParamToSqlWhere(typeof(T), mapSql, ref param);
            }

            return await _dao.QueryFirstAsync<T>(sql, param);
        }
        
        
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> SelectFirstAsync<T>(QueryInfo<T> info)
        {
            GetDynamicOrDefaultSql(info);
            return await _dao.QueryFirstAsync<T>(info);
        }
        

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> SelectFirstOrDefaultAsync<T>(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            string sql;
            if (param is string)
            {
                param = new Dictionary<string, object> { { GetPrimaryKey(typeof(T)), param } };
                sql = StatementParser.ParseDynamicSql(mapSql, (Dictionary<string, object>) param);
            }
            else
            {
                sql = AddParamToSqlWhere(typeof(T), mapSql, ref param);
            }

            return await _dao.QueryFirstOrDefaultAsync<T>(sql, param);
        }
        

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> SelectFirstOrDefaultAsync<T>(QueryInfo<T> info)
        {
            GetDynamicOrDefaultSql(info);
            return await _dao.QueryFirstOrDefaultAsync<T>(info);
        }
        

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> SelectCountAsync(QueryInfo info)
        {
            GetDynamicOrDefaultSql(info);
            return await _dao.QueryCountAsync(info);
        }
        
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<int> SelectCountAsync<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value)
        {
            var info = new QueryInfo<T>().AddParam(prop, value);
            GetDynamicOrDefaultSql(info);
            return await _dao.QueryCountAsync(info);
        }

        
        /// <summary>
        /// 返回指定页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectListAsync<T>(QueryInfo<T> info)
        {
            if (info.PageSize > 0)
            {
                info.DoCount(-1);
            }
            return (await SelectAsync(info)).ToList();
        }
        

        /// <summary>
        /// 返回总数+分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<QueryInfo<T>> SelectInfoAsync<T>(QueryInfo<T> info)
        {
            info.DoCount(1);
            GetDynamicOrDefaultSql(info);
            await _dao.QueryPaginateAsync(info);
            return info;
        }
        

        /// <summary>
        /// 当TotalCount==1时 返回总数+分页数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<QueryInfo> SelectInfoAsync(QueryInfo info)
        {
            info.DoCount(1);
            GetDynamicOrDefaultSql(info);
            await _dao.QueryPaginateAsync(info);
            info.MappingType = null;
            return info;
        }
        

        /// <summary>
        /// 返回数据+是否有下一页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<QueryInfo<T>> SelectInfoNextPageAsync<T>(QueryInfo<T> info)
        {
            var pageSize = info.PageSize;
            info.PageSize++;
            info.DoCount(-1);
            GetDynamicOrDefaultSql(info);
            await _dao.QueryPaginateAsync(info);
            info.HasNextPage = info.List.Count > pageSize;

            if (info.HasNextPage == true)
            {
                info.List = info.List.Take(pageSize).ToList();
            }
            return info;
        }
        #endregion

        
        #region 私有方法
        /// <summary>
        /// 根据T创建select，结合param生成where语句
        /// </summary>
        private string AddParamToSqlWhere(Type type, string mapSql, ref object param)
        {
            if (string.IsNullOrEmpty(mapSql))//类型获取
            {
                return GetMappedOrDefaultSql(type);
            }

            if (param != null)
            {
                //添加当前参数，通过参数名可以控制语句类型：EQ，LK，GT等
                var info = new QueryInfo
                {
                    Parameters = (IDictionary<string, object>)param
                };

                //重新映射
                if (!string.IsNullOrEmpty(mapSql))
                {
                    info.CustomSQL = mapSql;
                }

                //配置语句 动态构造
                StatementParser.ParseDynamicSql(info);

                //参数已被修改
                param = info.Parameters;
                return info.ToSQLString() + info.ToOrderBy();
            }
            else
            {
                var sql = StatementParser.GetMappedStaticSql(mapSql);
                if (sql.Equals(mapSql) && sql.IndexOf(" ", StringComparison.Ordinal) < 0)
                {
                    throw new ArgumentOutOfRangeException("无效的Sql配置项，请检查Key是否正确：" + mapSql);
                }
                return sql;
            }
        }

        private void CheckSqlInject(string sql)
        {
            if (sql != null && sql.IndexOf(' ') > 0)
            {
                if (HasSqlInject(sql))
                {
                    throw new ArgumentException("无效的SQL语句!");
                }
            }
        }

        //仅允许select 禁止sql中使用dba_/user_/v$等系统表
        private static readonly Regex RegExSql = new Regex("create |alter |truncate |exec", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        /// <summary>
        /// 仅允许select操作，并禁止对oracle系统表select
        /// </summary>
        /// <param name="sArgs"></param>
        /// <returns></returns>
        private bool HasSqlInject(string sArgs)
        {
            return sArgs != null && RegExSql.IsMatch(sArgs);
        }

        private void GetDynamicOrDefaultSql(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                if (string.IsNullOrEmpty(info.CustomSQL))//从Type获取SQL
                {
                    info.CustomSQL = GetMappedOrDefaultSql(info.GetMappingType());
                }
                else//已经制定SQL，动态化！
                {
                    StatementParser.ParseDynamicSql(info);
                }
            }
        }

        ///获取xml中配置的 User.Select语句， 或者根据对象的属性映射生成语句
        private string GetMappedOrDefaultSql(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "QueryInfo未指定查询的对象类型!");
            }

            var key = type.Name + ".Select";
            var sql = StatementParser.GetMappedStaticSql(key);
            if (sql.Equals(key))
            {
                sql = MappingInfo.GetMappingInfo(type).Select + "t";
            }
            return sql;
        }


        private static readonly Regex RegexGetParam = new Regex("(" + StatementParser.PREFIX + "\\w*)", RegexOptions.Multiline);

        /// <summary>
        /// update、delete语句必须提供参数才允许执行！
        /// </summary>
        private void CheckNoParams(string sql, IDictionary<string, object> param)
        {
            if (sql.IndexOf("delete from", StringComparison.CurrentCultureIgnoreCase) > -1 || sql.IndexOf(" set ", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                if (param == null || param.Count == 0)
                {
                    throw new DappersException("删除、更新语句必须至少提供一个参数！");
                }
            }
        }
        #endregion

    }
}
