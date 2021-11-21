using System;
using System.Collections.Generic;
using System.Data;
using Dappers.Query;
using System.Linq;

namespace Dappers
{
    public interface IDao
    {
        ConnectionManager ConnectionManager { get; }
        IDbTransaction TxBegin();
        IDbTransaction TxBegin(IsolationLevel il);
        void TxCommit();
        void TxRollback();
        IDbConnection Open();
        /// <summary>
        /// close connection and commit transaction
        /// </summary>
        void Close();

        //------------------QUERY / EXECUTE----------------------------------
        /// <param name="sql"></param>
        /// <param name="param">IEnumerable for multiExec</param>
        int Execute(string sql, object param);
        int Execute(QueryInfo info);
        object ExecuteScalar(string sql, object param);

        int QueryCount(QueryInfo info);
        IEnumerable<T> Query<T>(string sql, object param);
        IEnumerable<object> Query(string sql, object param, Type type);
        IEnumerable<T> Query<T>(QueryInfo info);

        /// <summary>
        /// TotalCount==1 ʱ�����ظ�����ҳ������
        /// info.MappingType=null�������ֵ�
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        IEnumerable<object> Query(QueryInfo info);

        /// <summary>
        /// �˷����ɴ���SP output����,Oracle Cursor: info.AddParam("out_mycursor","CURSOR")
        /// ֵΪnull�Ĳ�����������,����out_RETURN Ϊ����ֵ
        /// </summary>
        GridReader QueryMultiple(string sql, object param);
        
        /// <summary>
        /// �˷����ɴ���SP output����,Oracle Cursor: info.AddParam("out_mycursor","CURSOR")
        /// ֵΪnull�Ĳ�����������,����out_RETURN Ϊ����ֵ
        /// </summary>
        GridReader QueryMultiple(QueryInfo info);
        DataTable QueryDataTable(string sql, object param);

        string BuildPagingSQL(QueryInfo info);

        /// <summary>
        /// ����TotalCount=1,���Զ�SQL�����ɷ�ҳ��ѯ������+��¼��
        /// info.MappingType ��ʱ��������
        /// </summary>
        QueryInfo QueryPaginate(QueryInfo info);

        /// <summary>
        /// TotalCount=1时，执行 分页查询+总数
        /// </summary>
        QueryInfo<T> QueryPaginate<T>(QueryInfo<T> info);
    }
}
