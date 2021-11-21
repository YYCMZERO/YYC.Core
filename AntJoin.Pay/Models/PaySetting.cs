namespace AntJoin.Pay.Models
{
    /// <summary>
    /// 配置
    /// </summary>
    public class PaySetting
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }


        /// <summary>
        /// 支付宝公钥\商户号
        /// </summary>
        public string PublicKey { get; set; }


        /// <summary>
        /// 开发者私钥
        /// </summary>
        public string PrivateKey { get; set; }


        /// <summary>
        /// 支付类别
        /// </summary>
        public PayEnum PayEnum { get; set; }


        /// <summary>
        /// 密匙是否来自文件
        /// </summary>
        public bool KeyFromFile { get; set; }


        /// <summary>
        /// 文件目录地址
        /// </summary>
        public string KeyFilePath { get; set; }


        /// <summary>
        /// 通知地址
        /// </summary>
        public string NotifyUrl { get; set; }
    }
}
