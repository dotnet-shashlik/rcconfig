#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Shashlik.RC.Config
{
    internal static class HttpHelper
    {
        private static IRestClient GetClient(string uri, IWebProxy? proxy = null, Encoding? encoding = null)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(uri));

            var client = new RestClient(new Uri(uri));
            if (encoding != null)
                client.Encoding = encoding;
            if (proxy != null)
                client.Proxy = proxy;
            client.RemoteCertificateValidationCallback = (a, b, c, d) => true;
            return client;
        }

        private static IRestRequest GetRequest(IDictionary<string, string>? headers = null,
            IDictionary<string, string>? cookies = null, int timeout = 30)
        {
            var request = new RestRequest {Timeout = timeout * 1000};

            if (headers != null && headers.Any())
                foreach (var (key, value) in headers!)
                    request.AddHeader(key, value);

            if (cookies != null && cookies.Any())
                foreach (var (key, value) in cookies!)
                    request.AddCookie(key, value);

            return request;
        }

        /// <summary>
        /// post form表单
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<string> PostForm(string url, object? formData,
            IDictionary<string, string>? headers = null, IDictionary<string, string>? cookies = null, int timeout = 30,
            IWebProxy? proxy = null, Encoding? encoding = null)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var client = GetClient(url, proxy, encoding);
            var request = GetRequest(headers, cookies, timeout);

            if (formData != null)
                request.AddObject(formData);
            var response = await client.ExecutePostAsync(request);
            if (response.IsSuccessful)
                return response.Content;
            throw new HttpRequestException(
                $"Http request error, url: {url}, method: post, http code: {response.StatusCode}, result: {response.Content}",
                response.ErrorException);
        }
    }
}