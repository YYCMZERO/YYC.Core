namespace AntJoin.Signatures
{
    public class RedisConnectionOption
    {
        public string RedisHost { get; set; }
        public int RedisPort { get; set; }
        public string RedisPassword { get; set; }
        public int RedisDbName { get; set; }
        public string RedisClientName { get; set; }
    }
}
