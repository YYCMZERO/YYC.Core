using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace AntJoin.Dapper.Context
{
    public class Reflection
    {
        static Dictionary<Type, TypeHolder> LRUCaches;
        public static TypeHolder GetHolder(Type type)
        {
            if (LRUCaches == null)
                LRUCaches = new Dictionary<Type, TypeHolder>();
            else if (LRUCaches.Count > 100)
                LRUCaches.Clear();

            if (!LRUCaches.ContainsKey(type))
                LRUCaches.Add(type, new TypeHolder(type));
            return LRUCaches[type];
        }

        /// <summary>
        /// 将di属性转换为对象（第2次调用后性能 约等于手写）
        /// </summary>
        /// <returns></returns>
        public static T SetData<T>(Dictionary<string, object> di)
        {
            T obj = Activator.CreateInstance<T>();
            SetData(di, obj);
            return obj;
        }

        /// <summary>
        /// 将di属性写入对象（第2次调用后性能 约等于手写）
        /// </summary>
        /// <returns></returns>
        public static void SetData(Dictionary<string, object> di, object obj)
        {
            if(obj!=null)
            {
                var holder=GetHolder(obj.GetType());
                foreach(var kp in di)
                {
                    SetHandler setter =null;
                    holder.Setters.TryGetValue(kp.Key,out setter);
                    if (setter == null) continue;

                    var property = holder.PropInfos[kp.Key];
                    Type t = Nullable.GetUnderlyingType(property.PropertyType)
                                     ?? property.PropertyType;

                    try { 
                        setter(obj, (kp.Value == null) ? null
                                     : Convert.ChangeType(kp.Value, t));
                    }catch(Exception ex)
                    {
                        throw new ArgumentException(string.Format("属性'{0}' ({1})设置值'{2}' ({3})发生异常",
                            kp.Key,t.Name,kp.Value,kp.Value==null?"null":kp.Value.GetType().Name
                            ),kp.Key, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 从对象中获取属性值
        /// </summary>
        public static object GetData(object obj,string propName)
        {
            var getters = GetHolder(obj.GetType()).Getters;
            return getters[propName](obj);
        }
    }

    public class TypeHolder
    {
        public TypeHolder(Type type)
        {
            MyType = type;

            this.propInfos = new Dictionary<string, PropertyInfo>();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var p in props)
                this.propInfos.Add(p.Name, p);
        }
        public Type MyType;
        Dictionary<string, PropertyInfo> propInfos;
        public Dictionary<string, PropertyInfo> PropInfos
        {
            get
            {
                return propInfos;
            }
        }

        Dictionary<string, SetHandler> setter;
        public Dictionary<string, SetHandler> Setters
        {
            get
            {
                if (setter == null)
                {
                    setter = new Dictionary<string, SetHandler>(propInfos.Count);
                    foreach (var ps in propInfos.Values)
                    {
                        if (ps.CanWrite)
                            setter.Add(ps.Name,
                                DynamicMethodCompiler.CreateSetHandler(MyType, ps)
                            );
                    }
                }
                return setter;
            }
        }

        Dictionary<string, GetHandler> getter;
        public Dictionary<string, GetHandler> Getters
        {
            get
            {
                if (getter == null)
                {
                    getter = new Dictionary<string, GetHandler>(propInfos.Count);
                    foreach (var ps in propInfos.Values)
                    {
                        if (ps.CanRead)
                            getter.Add(ps.Name,
                                DynamicMethodCompiler.CreateGetHandler(MyType, ps)
                            );
                    }
                }
                return getter;
            }
        }
    }
}
