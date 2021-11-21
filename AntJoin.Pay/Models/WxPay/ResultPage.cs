namespace AntJoin.Pay.Models.WxPay
{
    internal class ResultPage : BaseResult
    {
        public ResultPage(string xml) : base(xml)
        {
        }
        /// <summary>
        /// mweb_url为拉起微信支付收银台的中间页面，可通过访问该url来拉起微信客户端，完成支付,mweb_url的有效期为5分钟。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string mweb_url { get; set; }
    }
}
