using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AntJoin.Core.Configuration
{
    /// <summary>
    /// 取配置文件帮助类
    /// </summary>
    public class ConfigurationHelper
    {
        private static readonly string Appid = null;
        private static readonly string MetaServer = null;
        private static readonly string Cluster = null;

        private static readonly IConfiguration Instance;
        private static readonly object Padlock = new object();


        static ConfigurationHelper()
        {
            if (Instance == null)
            {
                lock (Padlock)
                {
                    if (Instance == null)
                    {
                        Instance = new ApolloConfiguration(Appid, MetaServer, Cluster, GetAppConfig("Apollo.NameSpaces")?.Split(','));
                    }
                }
            }
            Dictionary = new Dictionary<string, IConfiguration>();
        }

        #region 单项目
        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return Instance.GetValue(key);
        }


        /// <summary>
        /// 获取配置项。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static TValue GetValue<TValue>(string key)
        {
            return Instance.GetValue<TValue>(key);
        }


        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetDefaultValue(string key, string defaultValue)
        {
            return Instance.GetDefaultValue(key, defaultValue);
        }


        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static TValue GetDefaultValue<TValue>(string key, TValue defaultValue)
        {
            return Instance.GetDefaultValue(key, defaultValue);
        }
        #endregion


        #region 多项目
        private static readonly Dictionary<string, IConfiguration> Dictionary;
        private static readonly object Padlocks = new object();
        private static readonly IConfigurationRoot AppConfig = LoadAppSettingConfiguration.BuildConfiguration();

        private static IConfiguration GetConfiguration(string projectName)
        {
            var isSuc = Dictionary.TryGetValue(projectName, out var instance);
            if (!isSuc)
            {
                lock (Padlocks)
                {
                    isSuc = Dictionary.TryGetValue(projectName, out instance);
                    if (!isSuc)
                    {
                        var appid = GetAppConfig($"Apollo.{projectName}.AppId");
                        if (string.IsNullOrEmpty(appid))
                            throw new InvalidOperationException($"请配置{projectName}对应的Apollo.AppId");
                        var metaServer = GetAppConfig($"{projectName}.Apollo.MetaServer") ?? GetAppConfig("Apollo.MetaServer");
                        var cluster = GetAppConfig($"{projectName}.Apollo.Cluster");
                        var namespaces = GetAppConfig($"{projectName}.Apollo.NameSpaces")?.Split(",");
                        instance = new ApolloConfiguration(appid, metaServer, cluster, namespaces);
                        Dictionary[projectName] = instance;
                    }
                }
            }

            return instance;
        }
        /// <summary>
        /// Get the config from app config via key
        /// </summary>
        /// <returns> the value or null if not found </returns>
        private static string GetAppConfig(string key)
        {
            var key2 = key.Replace(".", ":");

            var value = AppConfig[key];
            if (string.IsNullOrEmpty(value))
                value = AppConfig[key2];

            if (string.IsNullOrEmpty(value))
                value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrEmpty(value))
                value = Environment.GetEnvironmentVariable(key2);

            return string.IsNullOrEmpty(value) ? null : value;
        }


        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static string GetValue(string key, string projectName)
        {
            return GetConfiguration(projectName).GetValue(key);
        }


        /// <summary>
        /// 获取配置项。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="projectName"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static TValue GetValue<TValue>(string key, string projectName)
        {
            return GetConfiguration(projectName).GetValue<TValue>(key);
        }


        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="projectName"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetDefaultValue(string key, string projectName, string defaultValue)
        {
            return GetConfiguration(projectName).GetDefaultValue(key, defaultValue);
        }


        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="projectName"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static TValue GetDefaultValue<TValue>(string key, string projectName, TValue defaultValue)
        {
            return GetConfiguration(projectName).GetDefaultValue(key, defaultValue);
        }


        #endregion
    }
}
