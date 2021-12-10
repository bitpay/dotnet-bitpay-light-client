using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using BitPayLight.Exceptions;
using Newtonsoft.Json.Linq;

namespace BitPayLight.Helpers
{
    public class RestHelper
    {
        private HttpClient _httpClient;
        private string _baseUrl;

        /// <summary>
        ///     Creates an instance of RestHelper
        /// </summary>
        /// <param name="environment">Env type for target environment</param>
        public RestHelper(string environment = Env.Prod)
        {
            _baseUrl = environment == Env.Prod ? Env.ProdUrl : Env.TestUrl;
            _httpClient = new HttpClient {BaseAddress = new Uri(_baseUrl)};
        }
        
        /// <summary>
        ///     Make a GET request
        /// </summary>
        /// <param name="uri">The URI to query</param>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        public async Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> parameters = null)
        {
            try
            {
                var fullUrl = _baseUrl + uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", Env.BitPayLightVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Env.BitpayPluginInfo);
                if (parameters != null)
                {
                    fullUrl += "?";
                    foreach (var entry in parameters) fullUrl += entry.Key + "=" + entry.Value + "&";

                    fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
                }

                var result = await _httpClient.GetAsync(fullUrl);
                return result;
            }
            catch (Exception ex)
            {
                throw new BitPayLightCommunicationException(ex);
            }
        }
        
        /// <summary>
        ///     Make a POST request
        /// </summary>
        /// <param name="uri">The URI to query</param>
        /// <param name="json">The request payload as Json string</param>
        /// <returns>The HttpResponseMessage of the request</returns>
        public async Task<HttpResponseMessage> Post(string uri, string json)
        {
            try
            {
                var bodyContent = new StringContent(UnicodeToAscii(json));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-accept-version", Env.BitPayLightVersion);
                _httpClient.DefaultRequestHeaders.Add("x-bitpay-plugin-info", Env.BitpayPluginInfo);
                bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await _httpClient.PostAsync(uri, bodyContent).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                throw new BitPayLightCommunicationException(ex);
            }
        }
        
        /// <summary>
        ///     Process the given response in order to find valid data NOR errors
        /// </summary>
        /// <param name="response">HttpResponseMessage from API</param>
        /// <returns>A readable Json string</returns>
        public async Task<string> ResponseToJsonString(HttpResponseMessage response)
        {
            if (response == null)
                throw new BitPayLightCommunicationException(new NullReferenceException("Response is null"));

            try
            {
                // Get the response as a dynamic object for detecting possible error(s) or data object.
                // An error(s) object raises an exception.
                // A data object has its content extracted (throw away the data wrapper object).
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                JObject jObj;
                if (!string.IsNullOrEmpty(responseString) && responseString != "[]")
                    try
                    {
                        jObj = JObject.Parse(responseString);
                    }
                    catch (Exception)
                    {
                        var jArray = JArray.Parse(responseString);
                        jObj = JObject.Parse(jArray[0].ToString());
                    }
                else
                    jObj = new JObject();

                JToken value;
                JToken code;

                if (jObj.TryGetValue("status", out value))
                {
                    if (value.ToString().Equals("error"))
                    {
                        jObj.TryGetValue("code", out code);
                        jObj.TryGetValue("message", out value);
                        throw new BitPayLightCommunicationException(code.ToString(), value.ToString());
                    }
                }

                // Check for error response.
                if (jObj.TryGetValue("error", out value)) throw new BitPayLightCommunicationException(value.ToString());

                if (jObj.TryGetValue("errors", out value))
                {
                    var errors = value.Children().ToList();
                    var message = "Multiple errors:";
                    foreach (var errorItem in errors)
                    {
                        var error = errorItem.ToObject<JProperty>();
                        message += "\n" + error.Name + ": " + error.Value;
                    }

                    throw new BitPayLightCommunicationException(message);
                }

                // Check for and exclude a "data" object from the response.
                if (jObj.TryGetValue("data", out value)) responseString = value.ToString();

                return Regex.Replace(responseString, @"\r\n", "");
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BitPayLightCommunicationException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Parse a Json string into ascii string set
        /// </summary>
        /// <param name="json">Json string</param>
        /// <returns>ascii char set as string</returns>
        private string UnicodeToAscii(string json)
        {
            var unicodeBytes = Encoding.Unicode.GetBytes(json);
            var asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, unicodeBytes);
            var asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            return new string(asciiChars);
        }
    }
}