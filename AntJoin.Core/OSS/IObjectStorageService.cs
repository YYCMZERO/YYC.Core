using System.IO;

namespace AntJoin.Core.OSS
{
    public interface IObjectStorageService
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件本地路径</param>
        /// <param name="filename">远程路径,如果以/开头，则加上_1等</param>
        /// <param name="isCover">是否覆盖</param>
        /// <returns></returns>
        string UploadObject(string filePath, string filename, bool isCover = false);


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="filename">远程路径,如果以/开头，则加上_1等</param>
        /// <param name="isCover">是否覆盖</param>
        /// <returns></returns>
        string UploadObject(Stream stream, string filename, bool isCover = false);


        /// <summary>
        /// 获取完整文件路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        string GetFileUrl(string filename);


        /// <summary>
        /// 获取文件相对路径，仅对oss的文件路径有效
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        string GetFileRelativePath(string url);


        /// <summary>
        /// 获取完整图片路径
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="watermark">水印内容</param>
        /// <param name="fontSize">水印文字大小</param>
        /// <returns></returns>
        string GetImageUrl(string filename, int? width = null, int? height=null, string watermark=null, int? fontSize=null);


        /// <summary>
        /// 获取完整视频路径
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="snapshot">简要</param>
        /// <param name="mode">模式</param>
        /// <returns></returns>
        string GetVideoUrl(string filename, int? width = null, int? height = null, int? snapshot = 0, string mode = "fast");


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool DeleteObject(string key);


        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool DoesObjectExist(string filename);



        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件的名称</param>
        /// <param name="fileToDownload">文件保存的本地路径</param>
        bool GetObject(string key, string fileToDownload);




        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件的名称</param>
        byte[] GetObjectBytes(string key);


        /// <summary>
        /// 获取临时上传凭据
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        PostPolicy GeneratePostPolicy(string dir = "");
    }
}
