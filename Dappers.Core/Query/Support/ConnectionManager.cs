using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Data;

using Dappers.Query.Support;
using NLog;
using System.Reflection;

namespace Dappers.Query
{
    public class ConnectionManager : IDisposable
    {
        protected static Logger log = LogManager.GetCurrentClassLogger();
        private ILocalStorage threadStorage = new CallContextStorage();

        static readonly string CONNSLOT = "CON";
        static readonly string TRANSLOT = "Tx";
        public string Name { get; set; }
        string ConnSlot { get { return CONNSLOT + this.GetHashCode().ToString(); } }
        string TransSlot { get { return TRANSLOT + this.GetHashCode().ToString(); } }
        private IDbConnection GetStoredConnection()
        {
            return threadStorage.GetData(ConnSlot) as IDbConnection;
        }
        internal IDbTransaction GetTransaction()
        {
            return threadStorage.GetData(TransSlot) as IDbTransaction;
        }

        /// <summary>
        /// Load cached conn or create a new one, without open!
        /// </summary>
        /// <returns></returns>
        internal IDbConnection GetConnection()
        {
            IDbConnection conn = threadStorage.GetData(ConnSlot) as IDbConnection;
            if (conn == null)
            {
                conn = CreateConnection();
                threadStorage.SetData(ConnSlot, conn);
            }
            return conn;

            //var conn = CreateConnection();

            //return conn;
        }

        public void SetConnection(IDbConnection connNew)
        {
            if (connNew != null)
            {
                IDbConnection conn = threadStorage.GetData(ConnSlot) as IDbConnection;
                if (conn != null)
                {
                    conn.Close();//close old connection
                }

                threadStorage.SetData(ConnSlot, connNew);
                //刷新为新类型
                ConnectionType = connNew.GetType();
                prefix = null;
            }
        }

        /// <summary>
        /// get opened connection or create a new one.
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection Open()
        {
            IDbConnection conn = GetConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();

                if (log.IsDebugEnabled)
                    log.Debug(string.Format("=> Open Connection [{0} CN#{1}]", Name, conn.GetHashCode()));
            }

            return conn;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public virtual void Close()
        {
            IDbConnection conn = GetStoredConnection();
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
                if (log.IsDebugEnabled)
                    log.Debug(string.Format("=> Close Connection [{0} CN#{1}]", Name, conn.GetHashCode()));
            }

            FreeAllSlot();
        }

        internal virtual IDbTransaction TxBegin(IsolationLevel il)
        {
            IDbTransaction tx = GetTransaction();
            if (tx == null)
            {
                tx = Open().BeginTransaction(il);
                threadStorage.SetData(TransSlot, tx);
                if (log.IsDebugEnabled)
                    log.Debug(string.Format("==> TxBegin [{0} Tx#{1}]", Name, tx.Connection.GetHashCode()));
            }
            return tx;
        }
        internal virtual void TxCommit()
        {
            IDbTransaction tx = GetTransaction();
            if (tx != null && tx.Connection != null)
            {
                if (log.IsInfoEnabled)
                    log.Info(string.Format("==> TxCommit [{0} Tx#{1}]", Name, tx.Connection.GetHashCode()));
                tx.Commit();
            }
        }
        /// <summary>
        ///Tx之中若调用SP，则SP内部禁用commit， 否则导致Rollback无效
        /// </summary>
        internal virtual void TxRollback()
        {
            IDbTransaction tx = GetTransaction();
            if (tx != null && tx.Connection != null)
            {
                if (log.IsWarnEnabled)
                    log.Warn(string.Format("==> TxRollback [{0} Tx#{1}]", Name, tx.Connection.GetHashCode()));
                tx.Rollback();
            }
        }

        delegate IDbConnection CtorStringDelegate(string arg);
        CtorStringDelegate connCtor = null;//cached DbConnection constructor
        static object syncObj = new object();
        internal IDbConnection CreateConnection()
        {
            if (connCtor == null)
            {
                lock (syncObj)
                {
                    if (connCtor == null)
                    {
                        if (string.IsNullOrEmpty(ConnectionString))
                            throw new System.ArgumentNullException("ConnectionString", "数据库连接字符串未被设置！");
                        Type type = ConnectionType;
                        DynamicMethod dm = new DynamicMethod("NewConn", type, new Type[] { typeof(string) }, typeof(ConnectionManager), true);
                        ILGenerator ilgen = dm.GetILGenerator();
                        ilgen.Emit(OpCodes.Nop);
                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Newobj, type.GetTypeInfo().GetConstructor(new Type[] { typeof(string) }));
                        ilgen.Emit(OpCodes.Ret);

                        connCtor = (CtorStringDelegate)dm.CreateDelegate(typeof(CtorStringDelegate));
                    }
                }
            }
            return connCtor(ConnectionString);
        }

        Type connType;
        public Type ConnectionType
        {
            get
            {
                if (connType == null)
                {
                    if (string.IsNullOrEmpty(ConnectionTypeName))
                        throw new ArgumentNullException("ConnectionTypeName", "At lease 'ConnectionType' or 'ConnectionTypeName' should be provided.");
                    connType = Type.GetType(ConnectionTypeName, true);

                    if (connType == null)
                        throw new ArgumentNullException("ConnectionType", "'ConnectionType' is null or invalid!");
                }
                return connType;
            }
            set
            {
                connType = value;
            }
        }

        string prefix;
        public string ParamPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(prefix))
                {
                    if (ConnectionType.Name.IndexOf("Oracle") > -1)
                        prefix = ":";
                    else
                        prefix = StatementParser.PREFIX;
                }
                return prefix;
            }
            set { prefix = value; }
        }
        public string ConnectionTypeName { get; set; }
        public string ConnectionString { get; set; }
        public int? Timeout { get; set; }

        public void Dispose()
        {
            IDbConnection conn = GetStoredConnection();
            if (conn != null)
                conn.Dispose();
            FreeAllSlot();
        }
        private void FreeAllSlot()
        {
            threadStorage.FreeNamedDataSlot(ConnSlot);
            threadStorage.FreeNamedDataSlot(TransSlot);
        }
    }
}