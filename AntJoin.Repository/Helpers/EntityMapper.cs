using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace AntJoin.Repository
{
    /// <summary>
    /// 实体映射器
    /// </summary>
    internal class EntityMapper
    {
        /// <summary>
        /// 实体映射集合
        /// </summary>
        private static IReadOnlyList<IEntityRegister> _maps = new List<IEntityRegister>();



        /// <summary>
        /// 实例化
        /// </summary>
        internal EntityMapper()
        {

        }

        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        internal void Map(ModelBuilder modelBuilder)
        {
            GetMaps().ForEach(s => s.RegistTo(modelBuilder));
        }


        private List<IEntityRegister> GetMaps()
        {
            if (!_maps.Any())
            {
                var maps = new List<IEntityRegister>();
                var assemblies = AssemblyLoadContext.Default.Assemblies.ToList();
                assemblies.ForEach(s =>
                {
                    var list = GetInstancesByInterface<IEntityRegister>(s);
                    maps.AddRange(list);
                });
                _maps = maps;
            }
            return _maps.ToList();
        }


        /// <summary>
        /// 获取实现了接口的所有实例
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="assembly">在该程序集中查找</param>
        private List<TInterface> GetInstancesByInterface<TInterface>(Assembly assembly)
        {
            var typeInterface = typeof(TInterface);
            return assembly.GetTypes()
                .Where(t => typeInterface.GetTypeInfo().IsAssignableFrom(t) &&
                            t != typeInterface &&
                            t.GetTypeInfo().IsAbstract == false)
                .Select(t => (TInterface)Activator.CreateInstance(t)).ToList();
        }
    }
}
