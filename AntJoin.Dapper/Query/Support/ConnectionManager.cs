using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Data;

using AntJoin.Dapper.Query.Support;
using NLog;
using System.Reflection;

namespace AntJoin.Dapper.Query
{
    public sealed class ConnectionManager : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly ILocalStorage _threadStorage = new CallContextStorage();

        private const string Connslot = "CON";
        private const string Translot = "Tx";
        public string Name { get; set; }
        private string ConnSlot => Connslot + GetHashCode();
        private string TransSlot => Translot + GetHashCode();
        
        private delegate IDbConnection CtorStringDelegate(string arg);

        //cached DbConnection constructor
        private CtorStringDelegate _connCtor = null;
        private static readonly object SyncObj = new object();
        
        

        private Type _connType;
        public Type ConnectionType
        {
            get
            {
                if (_connType == null)
                {
                    if (string.IsNullOrEmpty(ConnectionTypeName))
                    {
                        throw new ArgumentNullException(nameof(ConnectionTypeName), "At lease 'ConnectionType' or 'ConnectionTypeName' should be provided.");
                    }
                    _connType = Type.GetType(ConnectionTypeName, true);

                    if (_connType == null)
                    {
                        throw new ArgumentNullException(nameof(ConnectionType), "'ConnectionType' is null or invalid!");
                    }
                }
                return _connType;
            }
            set => _connType = value;
        }

        public string Prefix;
        public string ParamPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(Prefix))
                {
                    Prefix = ConnectionType.Name.IndexOf("Oracle", StringComparison.Ordinal) > -1 ? ":" : StatementParser.PREFIX;
                }
                return Prefix;
            }
            set => Prefix = value;
        }
        public string ConnectionTypeName { get; set; }
        public string ConnectionString { get; set; }
        public int? Timeout { get; set; }

        /// <summary>
        /// 获取保存的连接
        /// </summary>
        /// <returns></returns>
        private IDbConnection GetStoredConnection()
        {
            return _threadStorage.GetData(ConnSlot) as IDbConnection;
        }
        
        
        /// <summary>
        /// 获取事务
        /// </summary>
        /// <returns></returns>
        internal IDbTransaction GetTransaction()
        {
            return _threadStorage.GetData(TransSlot) as IDbTransaction;
        }

        /// <summary>
        /// 获取连接，如果连接为打开，会打开
        /// </summary>
        /// <returns></returns>
        internal IDbConnection GetConnection()
        {
            if (!(_threadStorage.GetData(ConnSlot) is IDbConnection conn))
            {
                conn = CreateConnection();
                _threadStorage.SetData(ConnSlot, conn);
            }
            if(conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            return conn;
        }

        /// <summary>
        /// 设置新连接
        /// </summary>
        /// <param name="connNew"></param>
        public void SetConnection(IDbConnection connNew)
        {
            if (connNew != null)
            {
                if (_threadStorage.GetData(ConnSlot) is IDbConnection conn && conn.State == ConnectionState.Open)
                {
                    conn.Close();//close old connection
                    conn.Dispose();
                }
                
                _threadStorage.SetData(ConnSlot, connNew);
                //刷新为新类型
                ConnectionType = connNew.GetType();
                Prefix = null;
            }
        }

        /// <summary>
        /// get opened connection or create a new one.
        /// </summary>
        /// <returns></returns>
        public IDbConnection Open()
        {
            var conn = GetConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"=> Open Connection [{Name} CN#{conn.GetHashCode()}]");
                }
            }
            return conn;
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal IDbConnection CreateConnection()
        {
            if (_connCtor == null)
            {
                lock (SyncObj)
                {
                    if (_connCtor == null)
                    {
                        if (string.IsNullOrEmpty(ConnectionString))
                        {
                            throw new System.ArgumentNullException(nameof(ConnectionString), "数据库连接字符串未被设置！");
                        }
                        var type = ConnectionType;
                        var dm = new DynamicMethod("NewConn", type, new Type[] { typeof(string) }, typeof(ConnectionManager), true);
                        var ilgen = dm.GetILGenerator();
                        ilgen.Emit(OpCodes.Nop);
                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Newobj, type.GetTypeInfo().GetConstructor(new Type[] { typeof(string) })!);
                        ilgen.Emit(OpCodes.Ret);

                        _connCtor = (CtorStringDelegate)dm.CreateDelegate(typeof(CtorStringDelegate));
                    }
                }
            }
            return _connCtor(ConnectionString);
        }
        
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            var conn = GetStoredConnection();
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"=> Close Connection [{Name} CN#{conn.GetHashCode()}]");
                }
            }

            FreeAllSlot();
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        internal IDbTransaction TxBegin(IsolationLevel il)
        {
            var tx = GetTransaction();
            if (tx != null)
            {
                tx.Dispose();
                _threadStorage.FreeNamedDataSlot(TransSlot);
            }
            tx = Open().BeginTransaction(il);
            _threadStorage.SetData(TransSlot, tx);
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"==> TxBegin [{Name} Tx#{tx.Connection.GetHashCode()}]");
            }
            return tx;
        }
        
        /// <summary>
        /// 提交事务
        /// </summary>
        internal void TxCommit()
        {
            var tx = GetTransaction();
            if (tx?.Connection != null)
            {
                if (Log.IsInfoEnabled)
                {
                    Log.Info($"==> TxCommit [{Name} Tx#{tx.Connection.GetHashCode()}]");
                }
                tx.Commit();
            }
        }
        
        /// <summary>
        ///Tx之中若调用SP，则SP内部禁用commit， 否则导致Rollback无效
        /// </summary>
        internal void TxRollback()
        {
            var tx = GetTransaction();
            if (tx?.Connection != null)
            {
                if (Log.IsWarnEnabled)
                {
                    Log.Warn($"==> TxRollback [{Name} Tx#{tx.Connection.GetHashCode()}]");
                }
                tx.Rollback();
            }
        }
        
        public void Dispose()
        {
            var conn = GetStoredConnection();
            if (conn != null)
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Dispose();
                
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"=> Dispose Connection [{Name} CN#{conn.GetHashCode()}]");
                }
            }
            FreeAllSlot();
        }
        
        private void FreeAllSlot()
        {
            _threadStorage.FreeNamedDataSlot(ConnSlot);
            _threadStorage.FreeNamedDataSlot(TransSlot);
        }
    }
}