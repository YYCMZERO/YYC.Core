using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Dappers.Cfg
{
    public class XmlChangeWatcher
    {
        static XmlChangeWatcher m_watcher;
        public static XmlChangeWatcher Instance{
            get{
                if(m_watcher==null){
                    lock(typeof(XmlChangeWatcher)){
                        if(m_watcher==null)
                            m_watcher=new XmlChangeWatcher();
                    }
                }
                return m_watcher;
            }
        }
        //仅指定路径名， 自动监控*.*文件
        //public void Watch(String path){
        //    this.lastChanged=DateTime.Now;
        //    Assembly.GetEntryAssembly();
        //    if (path.IndexOf(":")<0)
        //        path=Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,path);

        //    this.path=path;

        //    this.watch();
        //}

        FileSystemWatcher watcher;
        String path;
        DateTime lastChanged;


        private void watch()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = "*.*";

            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // NotifyFilters.LastAccess 
            watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.EnableRaisingEvents = true;
            
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //仅处理间隔5秒以上的持续写入
            if((DateTime.Now - this.lastChanged).TotalSeconds >5)
            {
                this.lastChanged=DateTime.Now;
                //ConfigManager.Clear("DB_ADAPTERS");
                Dappers.StatementParser.StatementCache.Clear();
            }
        }
    }
}
