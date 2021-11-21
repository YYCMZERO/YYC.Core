namespace AntJoin.Pay.Models.AliPay
{
    internal class ExtendParams
    {
        /// <summary>
        /// 可选，系统商编号  该参数作为系统商返佣数据提取的依据，请填写系统商签约协议的PID
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string sys_service_provider_id { get; set; }

        /// <summary>
        /// 可选，行业数据回流信息, 详见：地铁支付接口参数补充说明
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string industry_reflux_info { get; set; }

        /// <summary>
        /// 可选，卡类型
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string card_type { get; set; }
    }
}
