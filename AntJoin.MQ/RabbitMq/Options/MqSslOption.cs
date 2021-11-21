namespace AntJoin.MQ.Options
{
    /// <summary>
    /// Mq连接加密配置
    /// </summary>
    public class MqSslOption
    {
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 证书路径
        /// </summary>
        public string CertPath { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPwd { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public MqSslOption()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public MqSslOption(string serverName, string certPath, string certPwd)
        {
            ServerName = serverName;
            CertPath = certPath;
            CertPwd = certPwd;
        }
    }
}