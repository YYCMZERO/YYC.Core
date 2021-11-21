namespace AntJoin.Core.SMS
{
    public class SMSHeper
    {
        private static readonly IShortMessageService Instance;
        private static readonly object Padlock = new object();


        static SMSHeper()
        {
            if (Instance == null)
            {
                lock (Padlock)
                {
                    if (Instance == null)
                    {
                        Instance = new DaHanSMS();
                    }
                }
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <param name="content">内容</param>
        /// <param name="sign"></param>
        /// <param name="msg">返回信息</param>
        /// <returns></returns>
        public static bool SendMessage(string phone, string content, string sign, out string msg)
        {
            return Instance.SendMessage(phone, content, sign, out msg);
        }


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <param name="content">内容</param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static bool SendMessage(string phone, string content, string sign)
        {
            return Instance.SendMessage(phone, content, sign, out var _);
        }
    }
}
