namespace AntJoin.Core.OSS
{
    public class PostPolicy
    {
        /// <summary>
        /// key
        /// </summary>
        public string AccessKeyId { get; set; }


        /// <summary>
        /// Policy
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 存储空间
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// 上传文件夹
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// 图片域名
        /// </summary>
        public string ImgDomain { get; set; }
    }
}
