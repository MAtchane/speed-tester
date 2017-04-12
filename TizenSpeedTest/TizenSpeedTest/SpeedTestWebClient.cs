using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml.Serialization;
using TizenSpeedTest;

namespace NSpeedTest
{
    public static class SpeedTestWebClient 
    {
        public static int ConnectionLimit { get; set; }

        public static Uri requestUri { get; set; }

        public static HttpClient client { get; set; }


        static SpeedTestWebClient()
        {
            ConnectionLimit = 10;
            client = new HttpClient();
            SetRequestConfiguration();


        }

        public static async System.Threading.Tasks.Task<T> GetConfigAsync<T>(string url)
        {
            var uri = new Uri(url);
            uri = AddTimeStamp(uri);
            var data = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult().Content.ReadAsStringAsync();
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(data))
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        public static void SetRequestConfiguration()
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html, application/xhtml+xml, */*");
            //            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache); connection limit

        }


        public static Uri AddTimeStamp(Uri address)
        {
            var uriBuilder = new UriBuilder(address);
            var query = HttpUtility.ParseQueryString(address);
            query["x"] = DateTime.Now.ToFileTime().ToString(CultureInfo.InvariantCulture);
            uriBuilder.Query = "x=" + query["x"];
            return uriBuilder.Uri;
        }
    }

}
