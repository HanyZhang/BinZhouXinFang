using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace XinFangAndroid.Common
{
    public static class WebRequest
    {
        private readonly static string ServerUrl = "http://192.144.173.160:8011";
        /// <summary>
        /// send the post request based on HttpClient
        /// </summary>
        /// <param name="requestUrl">the url you post</param>
        /// <param name="routeParameters">the parameters you post</param>
        /// <returns>return a response object</returns>
        public static async Task<object> SendPostRequestBasedOnHttpClient(string requestUrl, IDictionary<string, string> routeParameters)
        {
            object returnValue = new object();
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            Uri uri = new Uri(ServerUrl+ requestUrl);
            var content = new FormUrlEncodedContent(routeParameters);
            try
            {
                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var stringValue = await response.Content.ReadAsStringAsync();
                    returnValue = JsonObject.Parse(stringValue);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }
        
    }
    /*
    public static class AccessData
    {
        public static string GetRouteData(string url)
        {
            var client = new RestClient("http://example.com");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("resource/{id}", Method.POST);
            request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            request.AddHeader("header", "value");

            // add files to upload (works with compatible verbs)
            //request.AddFile(path);

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            IRestResponse<Person> response2 = client.Execute<Person>(request);
            var name = response2.Data.Name;

            // easy async support
            client.ExecuteAsync(request, response => {
                Console.WriteLine(response.Content);
            });

            // async with deserialization
            var asyncHandle = client.ExecuteAsync<Person>(request, response => {
                Console.WriteLine(response.Data.Name);
            });

            // abort the request on demand
            asyncHandle.Abort();
        }
    }

    */

    /*
    public static class AccessData
    {
        public static string GetRouteData(string url)
        {
            //构建请求  
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            request.ContentType = "text/json;chartset=UTF-8";
            //request.UserAgent = "";  
            request.Method = "Get";
            //接收响应  
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string retString = streamReader.ReadToEnd();
            return retString;
        }
    }
    */
}