using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace Dappers.Cfg
{
    public class ConfigManager
    {
        protected static Logger log = LogManager.GetCurrentClassLogger();
        static readonly string DEFAULT_CONFIG = "COMMON_CFG";
        
        /// <summary>
        /// 清除该配置下的全部key
        /// </summary>
        public static void Clear(string _ConfigName)
        {
            htCache.Remove(_ConfigName);
        }

        public static string GetRootConfig(string _key){
            return System.Configuration.ConfigurationManager.AppSettings[_key];
        }

        public static IDictionary<string, string> htCache = new Dictionary<string, string>();//ConfigFile_Key
        /// <summary>
        /// 从COMMON_CFG 对应文件中读取键值,null if not exists
        /// </summary>
        /// <param name="_Key"></param>
        /// <returns></returns>
        public static string GetConfig(string _Key)
        {
            return GetConfig(DEFAULT_CONFIG, _Key);
        }
        /// <summary>
        /// 从_ConfigFile中读取_Key的值,null if not exists
        /// </summary>
        /// <param name="_ConfigName">web.config中appSettings节配置的键</param>
        /// <param name="_Key">对应配置文件内需检索的键</param>
        /// <returns>对应配置文件内检索结果</returns>
        public static string GetConfig (string _ConfigName, string _Key)
		{
			string _CACHE_KEY = _ConfigName + _Key;

			//already cached the key ?
			if (htCache.ContainsKey (_CACHE_KEY))
				return htCache [_CACHE_KEY];

			//no key, but already cached the file?
			if (htCache.ContainsKey (_ConfigName))
				return null;

			string configValue = System.Configuration.ConfigurationManager.AppSettings [_ConfigName];
			if (configValue == null) {
				configValue = System.Configuration.ConfigurationManager
					.OpenExeConfiguration(ConfigurationUserLevel.None)
					.AppSettings.Settings[_ConfigName].Value;
			}
            //not cached, read files.
            ReadConfig(_ConfigName
                , configValue
                , htCache);


            htCache[_ConfigName] = "FILE_LOADED";//File Loaded

            //now, cached the key?
            if (htCache.ContainsKey(_CACHE_KEY))
                return htCache[_CACHE_KEY];
            else
                return null;
        }

        static void ReadConfig(string ConfigName,string physicalFiles, IDictionary<string, string> storage)
        {
            if (physicalFiles == null)
                physicalFiles = @"Config/App.config";

            ExeConfigurationFileMap _mappedFile = new ExeConfigurationFileMap();

            string[] sfiles = null;
            if (physicalFiles.IndexOf('*') > 0)
            {
                string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, physicalFiles);
                string file = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);
                sfiles = Directory.GetFiles(path, file, SearchOption.TopDirectoryOnly);
            }
            else
            {
                sfiles = physicalFiles.Split(',');
                for (int i = 0; i < sfiles.Length; i++)
                {
                    sfiles[i] = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, sfiles[i]);
                }
            }
            
            foreach (string file in sfiles)
            {
                _mappedFile.ExeConfigFilename = file;
                try
                {
                    Configuration _configs = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(_mappedFile, ConfigurationUserLevel.None);
                    if(_configs.AppSettings.Settings.Count==0)
                        throw new Exception("Empty config file is not allowed!");
                        
                    log.Debug("Loading config file '"+file+ "' with [ "+_configs.AppSettings.Settings.Count+" ] items.");
                    foreach (string key in _configs.AppSettings.Settings.AllKeys)
                    {
                        storage[ConfigName + key] = _configs.AppSettings.Settings[key].Value;
                    }
                }
                catch (System.Configuration.ConfigurationErrorsException ex)
                {
                    log.Error("Loading config file failed: "+file,ex);
                    if (ex.Message.IndexOf("<configuration>") < 0)//如果此文件非<configuration>根,忽略。 否则抛出
                        throw ex;
                }
            }
            
            if(log.IsDebugEnabled)
                printCachedItems(storage);
        }

        static void printCachedItems(IDictionary<string, string> storage)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("List all loaded items: ");
            foreach(string key in storage.Keys){
                sb.AppendLine(key);
            }
            log.Debug(sb);
        }
        
        /// <summary>
        /// 从_ConfigFile中保存_Key对应的值
        /// </summary>
        /// <param name="_ConfigName">web.config中appSettings节配置的键</param>
        /// <param name="_Key">对应配置文件内需检索的键</param>
        /// <param name="_Value">对应配置文件内需设置的值</param>
        public static void SetConfig(string _ConfigName, string _Key, string _Value)
        {
            ExeConfigurationFileMap _configFile = new ExeConfigurationFileMap();
            string[] sfiles = System.Configuration.ConfigurationManager.AppSettings[_ConfigName].Split(',');
            _configFile.ExeConfigFilename = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + sfiles[0];
            Configuration _config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(_configFile, ConfigurationUserLevel.None);

            _config.AppSettings.Settings.Remove(_Key);
            _config.AppSettings.Settings.Add(_Key, _Value);
            _config.Save();
        }
    }
}
