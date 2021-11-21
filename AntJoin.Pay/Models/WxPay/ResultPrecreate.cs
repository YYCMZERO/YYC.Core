namespace AntJoin.Pay.Models.WxPay
{
    internal class ResultPrecreate : BaseResult
    {
        public ResultPrecreate(string xml) : base(xml)
        {
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_type { get; set; }

        /// <summary>
        /// 二维码链接
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string code_url { get; set; }
    }
}
