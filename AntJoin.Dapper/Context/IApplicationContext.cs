using System;
using System.Collections.Generic;
using System.Text;

namespace AntJoin.Dapper.Context
{
    public interface IApplicationContext
    {
        /// <summary>
        /// 暴露原始容器接口
        /// </summary>
        //object Container { get; }
        string Name { get; set; }
        //object GetObject(Type type);
        object GetObject(string typename);
        T TryGetObject<T>();
        //bool IsRegisteredWithName<T>(string name);
        //T GetObject<T>(string name);
        T GetObject<T>();
        T GetRootObject<T>();
    }
}
