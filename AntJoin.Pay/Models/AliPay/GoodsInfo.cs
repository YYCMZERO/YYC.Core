namespace AntJoin.Pay.Models.AliPay
{
    internal class GoodsInfo
    {
        /// <summary>
        /// 必填，商品的编号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string goods_id { get; set; }

        /// <summary>
        /// 必填，物品名称
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string goods_name { get; set; }

        /// <summary>
        /// 必填，数量
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string quantity { get; set; }

        /// <summary>
        /// 必填，商品单价，单位为元
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string price { get; set; }

        /// <summary>
        /// 可选，商品类目
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string goods_category { get; set; }

        /// <summary>
        /// 可选，商品描述信息
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string body { get; set; }

        /// <summary>
        /// 可选，商品的展示地址
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string show_url { get; set; }

    }
}
