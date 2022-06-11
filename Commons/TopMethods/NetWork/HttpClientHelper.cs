using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TopMethods.Logs;

namespace TopMethods.NetWork
{
    public class HttpClientHelper : IDisposable
    {
        #region 属性

        /// <summary>
        /// HTTP请求客户端
        /// </summary>
        private HttpClient HttpClientGlobal;

        #endregion

        #region 方法

        #endregion

        private void InitHttpClient()
        {
            var handler = new HttpClientHandler()
            {
                UseCookies = true,
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip,
                UseProxy = false,
                MaxConnectionsPerServer = 15,
            };
            HttpClientGlobal = new HttpClient(handler);
            HttpClientGlobal.DefaultRequestHeaders.Connection.Add("keep-alive");
            HttpClientGlobal.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
            HttpClientGlobal.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/json,application/xml;q=0.9,image/webp,*/*;q=0.8");
            HttpClientGlobal.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

            /*超时设置*/
            HttpClientGlobal.Timeout = new TimeSpan(0, 0, 5);

        }

        public HttpClientHelper()
        {
            InitHttpClient();
        }

        #region POST

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="statusCode">HTTP请求响应代码</param>
        /// <param name="isSuccess">HTTP是否响应成功</param>
        /// <returns></returns>
        public string HTTP_PostMessage(string url, string postData, out HttpStatusCode statusCode, out bool isSuccess)
        {
            string result = string.Empty;
            statusCode = HttpStatusCode.RequestTimeout;
            isSuccess = false;

            //设置Http的正文
            HttpContent httpContent = new StringContent(postData);

            //设置Http的内容标头
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //设置Http的内容标头的字符
            httpContent.Headers.ContentType.CharSet = "utf-8";

            try
            {
                //异步Post
                HttpResponseMessage response = HttpClientGlobal.PostAsync(url, httpContent).Result;

                //输出Http响应状态码
                statusCode = response.StatusCode;
                isSuccess = response.IsSuccessStatusCode;

                //确保Http响应成功
                if (isSuccess)
                {
                    //异步读取json
                    result = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    LogFactory.SystemLog(string.Format("http请求任务(连接：{0})，HTTP状态码：{1}({2})", url, response.StatusCode, (int)response.StatusCode), LogLevelType.Warn);
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog($"HTTP Url:{url}，PostData:{postData}", LogLevelType.Exception, ex);
            }
            return result;
        }

        #endregion

        #region GET 

        /// <summary>
        /// get请求，可以对请求头进行多项设置
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetResponseByGet(List<KeyValuePair<string, string>> paramArray, string url)
        {
            string result = "";

            url = url + "?" + BuildParam(paramArray);
            var response = HttpClientGlobal.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }

            return result;
        }

        public string GetResponseBySimpleGet(List<KeyValuePair<string, string>> paramArray, string url)
        {
            url = url + "?" + BuildParam(paramArray);
            var result = HttpClientGlobal.GetStringAsync(url).Result;
            return result;
        }

        public string HttpPostRequestAsync(string Url, List<KeyValuePair<string, string>> paramArray, string ContentType = "application/x-www-form-urlencoded")
        {
            string result = "";

            var postData = BuildParam(paramArray);

            var data = Encoding.ASCII.GetBytes(postData);

            try
            {
                HttpResponseMessage message = null;
                using (Stream dataStream = new MemoryStream(data ?? new byte[0]))
                {
                    using (HttpContent content = new StreamContent(dataStream))
                    {
                        content.Headers.Add("Content-Type", ContentType);
                        var task = HttpClientGlobal.PostAsync(Url, content);
                        message = task.Result;
                    }
                }
                if (message != null && message.StatusCode == HttpStatusCode.OK)
                {
                    using (message)
                    {
                        result = message.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        private string Encode(string content, Encoding encode = null)
        {
            if (encode == null)
            {
                return content;
            }

            return System.Web.HttpUtility.UrlEncode(content, Encoding.UTF8);

        }

        private string BuildParam(List<KeyValuePair<string, string>> paramArray, Encoding encode = null)
        {
            string url = "";

            if (encode == null)
            {
                encode = Encoding.UTF8;
            }

            if (paramArray != null && paramArray.Count > 0)
            {
                var parms = "";
                foreach (var item in paramArray)
                {
                    parms += string.Format("{0}={1}&", Encode(item.Key, encode), Encode(item.Value, encode));
                }
                if (parms != "")
                {
                    parms = parms.TrimEnd('&');
                }
                url += parms;

            }
            return url;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string GetResponse(string url, out string statusCode)
        {
            string result = string.Empty;
            HttpResponseMessage response = HttpClientGlobal.GetAsync(url).Result;
            statusCode = response.StatusCode.ToString();

            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync().Result;
            }

            return result;
        }

        #endregion

        #region Put

        // Put请求
        public string PutResponse(string url, string putData, out HttpStatusCode statusCode)
        {
            string result = string.Empty;
            statusCode = HttpStatusCode.NotFound;
            HttpContent httpContent = new StringContent(putData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            try
            {
                HttpResponseMessage response = HttpClientGlobal.PutAsync(url, httpContent).Result;
                statusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex) { LogFactory.SystemLog(ex.Message, LogLevelType.Exception, ex); }
            return result;
        }

        #endregion

        #region 上传文件

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url">网络资源Url地址</param>
        /// <param name="formContent">表单字典</param>
        /// <param name="filePath">文件字典</param>
        public string HttpWebMultiPartFormDataRequest(string url, Dictionary<string, string> formContent, string fileName, IEnumerable<string> filePath)
        {
            var result = string.Empty;
            try
            {
                using (var httpContent = new MultipartFormDataContent())
                {
                    CreateMultipartFormContent(httpContent, formContent);
                    CreateFileContent(httpContent, fileName, filePath);
                    var httpResponse = HttpClientGlobal.PostAsync(url, httpContent).Result;
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        result = httpResponse.StatusCode.ToString();
                    }
                }
            }
            catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }
            return result;
        }

        private void CreateFileContent(MultipartFormDataContent httpContent, string fileName, IEnumerable<string> filePath)
        {
            var contentType = "application/octet-stream";
            try
            {
                foreach (var path in filePath)
                {
                    if (File.Exists(path))
                    {
                        var file = new FileInfo(path);

                        var fs = file.Open(FileMode.Open);

                        var fileContent = new StreamContent(fs);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "\"" + fileName + "\"",
                            FileName = string.Format("\"{0}\"", file.Name),
                        };
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                        httpContent.Add(fileContent);
                    }
                }
            }
            catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }
        }

        /// <summary>
        /// 编码文件名传输(中文文件名)
        /// </summary>
        /// <param name="fileStream">上传文件流</param>
        /// <param name="fileName">上传文件名</param>
        /// <returns></returns>
        public void CreateMultipartFormContent(MultipartFormDataContent httpContent, Dictionary<string, string> dicParameters)
        {
            foreach (var keyValuePair in dicParameters)
            {
                if (!string.IsNullOrWhiteSpace(keyValuePair.Key))
                {
                    httpContent.Add(new StringContent(keyValuePair.Value ?? string.Empty), String.Format("\"{0}\"", keyValuePair.Key));
                }
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    HttpClientGlobal.CancelPendingRequests();

                    // TODO: 释放托管状态(托管对象)。
                    HttpClientGlobal.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~HTTPClientHelper() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
