using System;

namespace AntJoin.Pay.Models
{
    public class PayResult
    {
        public PayResult():this(2, "未知错误！")
        {
        }

        public PayResult(int codeNum, string msg)
        {
            SetResult(codeNum, msg);
        }

        public PayResult(ResultEnum codeNum, string msg)
        {
            SetResult(codeNum, msg);
        }

        /// <summary>
        /// 编码，1成功，2失败，3未知，4交易已关闭，5其他
        /// </summary>
        public int Code { get; set; }


        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime ServerTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 返回数据
        /// </summary>
        public PaymentResult Data { set; get; }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="codeNum"></param>
        /// <param name="msg"></param>
        public void SetResult(int codeNum, string msg)
        {
            Code = codeNum;
            Msg = msg;
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="codeNum"></param>
        /// <param name="msg"></param>
        public void SetResult(ResultEnum codeNum, string msg)
        {
            Code = (int)codeNum;
            Msg = msg;
            ServerTime = DateTime.Now;
        }
    }
}
