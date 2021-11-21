using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AntJoin.Core.Utils
{
    public static class HttpHelper
    {
        /// <summary>
        /// 使用Get方法获取字符串结果（没有加入Cookie）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url)
        {
            using (var http = HttpClientFactory.Create())
            {
                var data = await http.GetByteArrayAsync(url);
                var ret = Encoding.UTF8.GetString(data);
                return ret;
            }
        }

        /// <summary>
        /// Http Get 同步方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            return HttpGetAsync(url).Result;
        }

        /// <summary>
        /// Http Get 同步方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Stream HttpGetStream(string url)
        {
            return HttpGetStreamAsync(url).Result;
        }

        /// <summary>
        /// Http Get 同步方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<Stream> HttpGetStreamAsync(string url)
        {
            using (var http = HttpClientFactory.Create())
            {
                return await http.GetStreamAsync(url);
            }
        }

        /// <summary>
        /// Http Put 同步方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpPut(string url)
        {
            return HttpPutAsync(url).Result;
        }


        /// <summary>
        /// HTTP PUT
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> HttpPutAsync(string url)
        {
            using (var http = HttpClientFactory.Create())
            {
                var ms = new MemoryStream();
                var hc = new StreamContent(ms);
                var t = await http.PutAsync(url, hc);
                var t2 = await t.Content.ReadAsByteArrayAsync();
                return Encoding.UTF8.GetString(t2);
            }
        }



        /// <summary>
        /// POST 异步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postStream"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, IDictionary<string, string> formData = null, int timeOut = 10000)
        {
            HttpClientHandler handler = new HttpClientHandler();
            using (var http = HttpClientFactory.Create(handler))
            {
                MemoryStream ms = new MemoryStream();
                formData.FillFormDataStream(ms);//填充formData
                HttpContent hc = new StreamContent(ms);

                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
                hc.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36");
                hc.Headers.Add("Timeout", timeOut.ToString());
                hc.Headers.Add("KeepAlive", "true");

                var r = await http.PostAsync(url, hc);
                byte[] tmp = await r.Content.ReadAsByteArrayAsync();

                return Encoding.UTF8.GetString(tmp);
            }
        }

        /// <summary>
        /// POST 同步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postStream"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static string HttpPost(string url, IDictionary<string, string> formData = null, int timeOut = 10000)
        {
            return HttpPostAsync(url, formData, timeOut).Result;
        }

        /// <summary>
        /// POST 异步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="encodeType"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostJsonAsync(string url, string postData, int timeOut = 10000)
        {
            HttpClientHandler handler = new HttpClientHandler();
            using (var client = HttpClientFactory.Create(handler))
            {
                byte[] data = Encoding.UTF8.GetBytes(postData);
                HttpContent hc = new ByteArrayContent(data);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
                hc.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36");
                hc.Headers.Add("Timeout", timeOut.ToString());
                hc.Headers.Add("KeepAlive", "true");

                var responseMessage = await client.PostAsync(url, hc);
                var bytes = await responseMessage.Content.ReadAsByteArrayAsync();
                return Encoding.UTF8.GetString(bytes);
            }
        }
        
        /// <summary>
        /// POST 同步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="encodeType"></param>
        /// <returns></returns>
        public static string HttpPostJson(string url, string postData, int timeOut = 10000)
        {
            return HttpPostJsonAsync(url, postData, timeOut).Result;
        }


        /// <summary>
        /// POST 同步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string postDataStr)
        {
            return HttpPostAsync(url, postDataStr).Result;
        }


        /// <summary>
        /// POST 异步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, string postDataStr)
        {
            HttpClientHandler handler = new HttpClientHandler();
            using (var client = HttpClientFactory.Create(handler))
            {
                var t = await client.PostAsync(url, new StringContent(postDataStr, Encoding.UTF8, "application/json"));
                if(t.IsSuccessStatusCode)
                {
                    var t2 = await t.Content.ReadAsByteArrayAsync();
                    return Encoding.UTF8.GetString(t2);
                }
                throw new Exception(t.ReasonPhrase);
            }
        }



        /// <summary>
        /// 组装QueryString的方法
        /// 参数之间用&amp;连接，首位没有符号，如：a=1&amp;b=2&amp;c=3
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static string GetQueryString(this IDictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 填充表单信息的Stream
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="stream"></param>
        public static void FillFormDataStream(this IDictionary<string, string> formData, Stream stream)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            stream.Write(formDataBytes, 0, formDataBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置
        }
    }
}
