using System;
using System.IO;
using System.Linq;

namespace AntJoin.Core.OSS
{
    public abstract class BaseObjectStorageService
    {
        protected readonly string[] VideoTypes = { ".avi", ".wmv", ".mpeg", ".mp4", ".mov", ".mkv", ".flv", ".f4v", ".m4v", ".rmvb", ".rm", ".3gp", ".dat", ".ts", ".mts", ".vob", ".mp3", ".m3u8" };


        /// <summary>
        /// 获取扩展名
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected string GetFileExtension(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            int startIndex = source.LastIndexOf(".", StringComparison.Ordinal);
            if (startIndex != -1)
                return source.Substring(startIndex);
            return null;
        }


        /// <summary>
        /// 是否视频文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected bool IsVideoFile(string filename)
        {
            return VideoTypes.Contains(GetFileExtension(filename)?.ToLower());
        }



        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public byte[] StreamToBytes(Stream stream)
        {
            using MemoryStream memoryStream = new MemoryStream();
            memoryStream.Position = 0;
            stream.CopyTo(memoryStream);
            byte[] bytes = new byte[memoryStream.Length];
            memoryStream.Position = 0;
            memoryStream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
