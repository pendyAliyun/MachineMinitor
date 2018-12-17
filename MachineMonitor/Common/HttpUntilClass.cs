using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Common
{
    public class HttpUntilClass
    {
        /// <summary>
        /// 返回Http请求结果result（带参post）
        /// </summary>
        /// <param name="Url">请求完整的URL</param>
        /// <param name="data">需要请求的数据</param>
        /// <returns></returns>
        public static object getHttpResult(String Url, String requestMethod, String requestContentType, object data, Dictionary<string, string> requestHeaders)
        {
            //10.1.0.56:8088    //121.12.147.82:1999
            var request = WebRequest.Create(Url) as HttpWebRequest;
            request.ContentType = requestContentType;
            request.KeepAlive = false;
            request.Method = requestMethod;

            if (requestHeaders != null)
            {
                foreach (var dic in requestHeaders)
                {
                    request.Headers.Add(dic.Key, dic.Value);
                }

            }

            if (data != null)
            {
                var dataStr = JsonConvert.SerializeObject(data);//系列化
                var dataBytes = Encoding.UTF8.GetBytes(dataStr);

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                var reader = new StreamReader(response.GetResponseStream());
                var responseText = reader.ReadToEnd();
                var result = JsonConvert.DeserializeObject(responseText);
                return result;
            }
        }
    }
}
