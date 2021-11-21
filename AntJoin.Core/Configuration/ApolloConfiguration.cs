using System;
using System.Linq;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.Extensions.Configuration;

namespace AntJoin.Core.Configuration
{
    public class ApolloConfiguration : IConfiguration
    {

        private readonly IConfigurationRoot _config;
        private readonly IConfigurationRoot _appConfig = LoadAppSettingConfiguration.BuildConfiguration();

        private readonly string _defaultValue = null;

        public ApolloConfiguration()
        {
            var appId = GetAppConfig("AppId");
            var metaServer = GetAppConfig("MetaServer");
            var cluster = GetAppConfig("Cluster");
            var namespaces = GetAppConfig("NameSpaces")?.Split(',');
            _config = SetConfig(appId, metaServer, cluster, namespaces);
        }
        public ApolloConfiguration(string appId)
        {
            var metaServer = GetAppConfig("MetaServer");
            var cluster = GetAppConfig("Cluster");
            var namespaces = GetAppConfig("NameSpaces")?.Split(',');
            _config = SetConfig(appId, metaServer, cluster, namespaces);
        }
        public ApolloConfiguration(string appId, string metaServer, string cluster, string namespaces = null)
        {
            _config = SetConfig(appId, metaServer, cluster, namespaces);
        }

        public ApolloConfiguration(string appId, string metaServer, string cluster, string[] namespaces)
        {
            _config = SetConfig(appId, metaServer, cluster, namespaces);
        }
        private IConfigurationRoot SetConfig(string appId, string metaServer, string cluster, params string[] namespaces)
        {
            var builder = new ConfigurationBuilder();
            IApolloConfigurationBuilder configurationBuilder;
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(metaServer))
            {
                configurationBuilder = builder.AddApollo(_appConfig.GetSection("apollo"));
            }
            else
            {
                configurationBuilder = builder.AddApollo(new ApolloOptions
                {
                    AppId = appId,
                    MetaServer = metaServer,
                    Cluster = cluster
                });
            }
            namespaces = namespaces?.Where(a => !string.IsNullOrEmpty(a)).ToArray();
            if (namespaces != null && namespaces.Any())
            {
                configurationBuilder = namespaces.Aggregate(configurationBuilder, (current, ns) => configurationBuilder.AddNamespace(ns));
            }
            configurationBuilder = configurationBuilder
                .AddDefault(ConfigFileFormat.Xml)
                .AddDefault(ConfigFileFormat.Json)
                .AddDefault(ConfigFileFormat.Yml)
                .AddDefault(ConfigFileFormat.Yaml)
                .AddDefault();
            return configurationBuilder.Build();
        }
        /// <summary>
        /// Get the config from app config via key
        /// </summary>
        /// <returns> the value or null if not found </returns>
        private string GetAppConfig(string key)
        {
            var key1 = "Apollo." + key;
            var key2 = "Apollo:" + key;

            var value = _appConfig[key1];
            if (string.IsNullOrEmpty(value))
                value = _appConfig[key2];

            if (string.IsNullOrEmpty(value))
                value = Environment.GetEnvironmentVariable(key1);

            if (string.IsNullOrEmpty(value))
                value = Environment.GetEnvironmentVariable(key2);

            return string.IsNullOrEmpty(value) ? null : value;
        }


        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            return _config.GetValue(key, _defaultValue);
        }


        /// <summary>
        /// 获取配置项。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TValue GetValue<TValue>(string key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            return _config.GetValue<TValue>(key);
        }


        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public string GetDefaultValue(string key, string defaultValue)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            return _config.GetValue(key, defaultValue);
        }


        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public TValue GetDefaultValue<TValue>(string key, TValue defaultValue)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            return _config.GetValue(key, defaultValue);
        }
    }
}
