using System.IO;
using AntJoin.Core.Configuration;
using AntJoin.Core.Security;

namespace AntJoin.Core.OSS
{
    /// <summary>
    /// OSS（对象存储服务）帮助类
    /// </summary>
    public class OSSHelper
    {
        private static readonly string Endpoint = AJSecurity.Decrypt(ConfigurationHelper.GetValue("endpoint"));
        private static readonly string BucketName = AJSecurity.Decrypt(ConfigurationHelper.GetValue("bucketName"));
        private static readonly string ImgDomain = ConfigurationHelper.GetValue("imgDomain");
        private static readonly string AccessKeyId = AJSecurity.Decrypt(ConfigurationHelper.GetValue("accessKeyId"));
        private static readonly string AccessKeySecret = AJSecurity.Decrypt(ConfigurationHelper.GetValue("accessKeySecret"));

        private static readonly IObjectStorageService Instance;
        private static readonly object Padlock = new object();


        static OSSHelper()
        {
            if (Instance == null)
            {
                lock (Padlock)
                {
                    if (Instance == null)
                    {
                        Instance = new AliYunOss(Endpoint, BucketName, ImgDomain, AccessKeyId, AccessKeySecret);
                    }
                }
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件本地路径</param>
        /// <param name="filename">远程路径,如果以/开头，则加上_1等</param>
        /// <param name="isCover">是否覆盖</param>
        /// <returns></returns>
        public static string UploadObject(string filePath, string filename, bool isCover = false)
        {
            return Instance.UploadObject(filePath, filename, isCover);
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="filename">远程路径,如果以/开头，则加上_1等</param>
        /// <param name="isCover">是否覆盖</param>
        /// <returns></returns>
        public static string UploadObject(Stream stream, string filename, bool isCover = false)
        {
            return Instance.UploadObject(stream, filename, isCover);
        }


        /// <summary>
        /// 获取完整文件路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFileUrl(string filename)
        {
            return Instance.GetFileUrl(filename);
        }

        /// <summary>
        /// 获取文件相对路径，仅对oss的文件路径有效
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetFileRelativePath(string url)
        {
            return Instance.GetFileRelativePath(url);
        }


        /// <summary>
        /// 获取完整图片路径
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="watermark">水印内容</param>
        /// <param name="fontSize">水印文字大小</param>
        /// <returns></returns>
        public static string GetImageUrl(string filename, int? width = null, int? height = null, string watermark = null, int? fontSize = null)
        {
            return Instance.GetImageUrl(filename, width, height, watermark, fontSize);
        }



        /// <summary>
        /// 获取完整视频路径
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="snapshot">简要</param>
        /// <param name="mode">模式</param>
        /// <returns></returns>
        public static string GetVideoUrl(string filename, int? width = null, int? height = null, int? snapshot = 0, string mode = "fast")
        {
            return Instance.GetVideoUrl(filename, width, height, snapshot, mode);
        }

        /// <summary>
        /// 删除指定的文件
        /// </summary>   
        /// <param name="key">待删除的文件名称</param>
        public static bool DeleteObject(string key)
        {
            return Instance.DeleteObject(key);
        }

        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件的名称</param>
        /// <param name="fileToDownload">文件保存的本地路径</param>
        public static bool GetObject(string key, string fileToDownload)
        {
            return Instance.GetObject(key, fileToDownload);
        }


        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件的名称</param>
        public static byte[] GetObjectBytes(string key)
        {
            return Instance.GetObjectBytes(key);
        }


        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool DoesObjectExist(string filename)
        {
            return Instance.DoesObjectExist(filename);
        }



        /// <summary>
        /// 获取临时上传凭据
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static PostPolicy GeneratePostPolicy(string dir = "")
        {
            return Instance.GeneratePostPolicy(dir);
        }
    }
}
