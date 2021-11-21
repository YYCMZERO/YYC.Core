using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Aliyun.OSS;
using AntJoin.Core.Extensions;

namespace AntJoin.Core.OSS
{
    public class AliYunOss : BaseObjectStorageService, IObjectStorageService
    {
        private readonly string _endpoint;
        private readonly string _bucketName;
        private readonly string _imgDomain;
        private readonly string _accessKeyId;
        private readonly string _accessKeySecret;
        private readonly OssClient _client;

        public AliYunOss(string endpoint, string bucketName, string imgDomain, string accessKeyId, string accessKeySecret)
        {
            _endpoint = endpoint;
            _bucketName = bucketName;
            _imgDomain = imgDomain;
            _accessKeyId = accessKeyId;
            _accessKeySecret = accessKeySecret;
            _client = new OssClient(_endpoint, _accessKeyId, _accessKeySecret);
        }



        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filename"></param>
        /// <param name="isCover"></param>
        /// <returns></returns>
        public string UploadObject(string filePath, string filename, bool isCover = false)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return UploadObject(stream, filename, isCover);
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        /// <param name="isCover"></param>
        /// <returns></returns>
        public string UploadObject(Stream stream, string filename, bool isCover = false)
        {
            //不是覆盖且不是/开头，则加上_1等
            if (!isCover && !filename.StartsWith("/"))
            {
                filename = DateTime.Now.ToString("yyyyMM") + "/" + filename;
                filename = GetFileName(filename, 1);
            }
            else
            {
                filename = filename.TrimStart('/');
            }
            //ObjectMetadata metadata = client.GetObjectMetadata(_bucketName, key);
            _client.PutObject(_bucketName, filename, stream);
            return filename;
        }


        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string GetFileUrl(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string startPart = _imgDomain ?? "https://" + _bucketName + "." + _endpoint;
                if (!filename.StartsWith("/"))
                {
                    startPart += "/";
                }
                if (!filename.StartsWith("http"))
                {
                    filename = startPart + filename;
                }
                return filename;
            }

            return string.Empty;
        }


        /// <summary>
        /// 获取文件相对路径，仅对oss的文件路径有效
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetFileRelativePath(string url)
        {
            string startPart = _imgDomain ?? ("https://" + _bucketName + "." + _endpoint);
            if (url.IsNullOrEmpty() || url.IndexOf(startPart) != 0)
                return url;
            url = url.Replace(startPart, "").Trim('/');
            return url.Split("?")[0];
        }


        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="watermark"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public string GetImageUrl(string filename, int? width = null, int? height = null, string watermark = null, int? fontSize = null)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                var names = filename.Split("?");
                var parts = names[0].Split("/");
                parts[parts.Length - 1] = HttpUtility.UrlEncode(parts[parts.Length - 1]);
                filename = string.Join("/", parts);
                if (names.Length > 1)
                {
                    filename = filename + "?" + names[1];
                }

                string startPart = _imgDomain ?? ("https://" + _bucketName + "." + _endpoint);
                if (!filename.StartsWith("/"))
                {
                    startPart = startPart + "/";
                }
                if (!filename.StartsWith("http"))
                {
                    filename = startPart + filename;
                }
                try
                {
                    string extendName = GetFileExtension(filename);
                    List<string> ltParam = new List<string>();
                    string resize = string.Empty;
                    if (width.HasValue)
                    {
                        resize = "resize,w_" + width;
                    }
                    if (height.HasValue)
                    {
                        resize = "resize,h_" + height;
                    }
                    if (width.HasValue && height.HasValue)
                    {
                        resize = "resize,m_fill,h_" + height + ",w_" + width + ",color_FFFFFF";
                    }
                    if (!string.IsNullOrEmpty(resize))
                    {
                        ltParam.Add(resize);
                    }
                    if (!string.IsNullOrEmpty(watermark))
                    {
                        byte[] b = Encoding.Default.GetBytes(watermark);
                        //转成 Base64 形式的 System.String  
                        string strBase64 = Convert.ToBase64String(b);
                        ltParam.Add("watermark,type_ZmFuZ3poZW5na2FpdGk=,size_" + fontSize + ",text_" + strBase64);
                    }
                    if (extendName.ToLower().Equals(".jpg") || extendName.ToLower().Equals(".jpeg"))
                        ltParam.Add("interlace,1");//图片模糊到清晰
                    if (ltParam.Any() && !filename.Contains("?x-oss-process="))
                    {
                        filename += "?x-oss-process=image/" + string.Join("/", ltParam);
                    }
                    return filename;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 获取视频路径
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="snapshot"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public string GetVideoUrl(string filename, int? width = null, int? height = null, int? snapshot = 0, string mode = "fast")
        {
            if (!string.IsNullOrEmpty(filename))
            {
                var names = filename.Split("?");
                var parts = names[0].Split("/");
                parts[parts.Length - 1] = HttpUtility.UrlEncode(parts[parts.Length - 1]);
                filename = string.Join("/", parts);
                if (names.Length > 1)
                {
                    filename = filename + "?" + names[1];
                }

                string startPart = (_imgDomain ?? ("https://" + _bucketName + "." + _endpoint));
                if (!filename.StartsWith("/"))
                {
                    startPart = startPart + "/";
                }
                if (!filename.StartsWith("http"))
                {
                    filename = startPart + filename;
                }
                try
                {
                    if (!string.IsNullOrEmpty(mode))
                    {
                        mode = ",m_" + mode;
                    }
                    filename = filename + "?x-oss-process=video/snapshot,t_" + snapshot + ",ar_auto" + mode;
                    if (width.HasValue)
                    {
                        filename += ",w_" + width;
                    }
                    if (height.HasValue)
                    {
                        filename += ",h_" + height;
                    }
                    return filename;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 删除指定的文件
        /// </summary>   
        /// <param name="key">待删除的文件名称</param>
        public bool DeleteObject(string key)
        {
            var ossClient = new OssClient(_endpoint, _accessKeyId, _accessKeySecret);
            ossClient.DeleteObject(_bucketName, key);
            return true;
        }

        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件的名称</param>
        /// <param name="fileToDownload">文件保存的本地路径</param>
        public bool GetObject(string key, string fileToDownload)
        {
            OssObject ossObject = _client.GetObject(_bucketName, key);
            using (var requestStream = ossObject.Content)
            {
                byte[] buf = new byte[1024];
                var fs = File.Open(fileToDownload, FileMode.OpenOrCreate);
                int len;
                while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                {
                    fs.Write(buf, 0, len);
                }
                fs.Close();
            }
            return true;
        }


        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件的名称</param>
        public byte[] GetObjectBytes(string key)
        {
            key = key.TrimStart('/');
            OssObject ossObject = _client.GetObject(_bucketName, key);
            using var requestStream = ossObject.Content;
            return StreamToBytes(requestStream);
        }



        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private string GetFileName(string filename, int i)
        {
            string name = filename.Substring(0, filename.LastIndexOf(".", StringComparison.Ordinal));
            string ext = GetFileExtension(filename);
            string result;
            if (i != 1)
                result = name + "_" + i + ext;
            else
                result = filename;

            if (_client.DoesObjectExist(_bucketName, result))
            {
                i++;
                return GetFileName(filename, i);
            }

            return result;
        }


        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool DoesObjectExist(string filename)
        {
            if (filename.StartsWith("/"))
            {
                filename = filename.Substring(1);
            }
            if (_client.DoesObjectExist(_bucketName, filename))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取临时上传凭据
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public PostPolicy GeneratePostPolicy(string dir = "")
        {
            if (string.IsNullOrEmpty(dir))
            {
                dir = DateTime.Now.ToString("yyyyMM") + "/"; // 用户上传文件时指定的前缀。
            }
            int expireTime = 30;
            DateTime expiration = DateTime.Now.AddMinutes(expireTime);
            PolicyConditions policyConditions = new PolicyConditions();
            policyConditions.AddConditionItem(PolicyConditions.CondContentLengthRange, 0, 1048576000);
            policyConditions.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondKey, dir);

            string postPolicy = _client.GeneratePostPolicy(expiration, policyConditions);

            byte[] binaryData = Encoding.Default.GetBytes(postPolicy);
            string encodedPolicy = Convert.ToBase64String(binaryData);

            Encoding encode = Encoding.UTF8;
            byte[] byteData = encode.GetBytes(encodedPolicy);
            byte[] byteKey = encode.GetBytes(_accessKeySecret);
            HMACSHA1 hmac = new HMACSHA1(byteKey);
            CryptoStream cs = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write);
            cs.Write(byteData, 0, byteData.Length);
            cs.Close();

            var policy = new PostPolicy
            {
                AccessKeyId = _accessKeyId,
                Signature = Convert.ToBase64String(hmac.Hash),
                Policy = encodedPolicy,
                BucketName = _bucketName,
                Dir = dir,
                ImgDomain = _imgDomain
            };
            return policy;
        }
    }
}
