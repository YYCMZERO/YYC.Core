namespace AntJoin.MQ.Options
{
    public class MqConstacts
    {
        /// <summary>
        /// 默认的连接名
        /// </summary>
        public const string DefaultConnectionName = "Default";
    }


    public class DeliveryModes
    {
        /// <summary>
        ///  不持久化
        /// </summary>
        public const int NonPersistent = 1;
        
        /// <summary>
        /// 持久化
        /// </summary>
        public const int Persistent = 2;
    }
}