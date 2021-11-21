namespace AntJoin.Core.Domains
{
    public class LoginUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string FullName { get; set; }


        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }


        /// <summary>
        /// 客户端类型
        /// </summary>
        public string ClientType { get; set; }


        /// <summary>
        /// 项目类型
        /// </summary>
        public string ProjectType { get; set; }


        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }


        /// <summary>
        /// 国际移动设备识别码
        /// </summary>
        public string Imel { get; set; }


        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }


        /// <summary>
        /// 系统类型
        /// </summary>
        public string SystemType { get; set; }


        /// <summary>
        /// 商户ID/租户ID
        /// </summary>
        public string TenantID { get; set; }


        /// <summary>
        /// 是否绑定微信
        /// </summary>
        public bool IsBingWechat { get; set; }
    }
}
