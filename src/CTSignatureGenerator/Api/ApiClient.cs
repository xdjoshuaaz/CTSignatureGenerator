using CTSignatureGenerator.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CTSignatureGenerator.Api
{
    public class ApiClient
    {
        internal static readonly TimeSpan ServerTimezoneOffset = TimeSpan.FromHours(1);
        private static readonly Uri BaseUri = new Uri("http://convoytrucking.net/api/");

        private HttpClient client;
        private JsonSerializer jsonSerializer;

        public ApiClient() {
            client = new HttpClient();
            jsonSerializer = new JsonSerializer();
        }

        public async Task<Player> GetPlayerAsync(string name) {
            HttpResponseMessage response = await client.GetAsync(Api("players?name=" + Uri.EscapeUriString(name)));
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            JArray results = json.Value<JArray>("results");

            if (results.Count <= 0) {
                return null;
            }
            return Player.BuildFromJson(results.First() as JObject);
        }

        private static Uri Api(string relativePath) {
            return Api(new Uri(relativePath, UriKind.Relative));
        }

        private static Uri Api(Uri path) {
            return new Uri(BaseUri, path);
        }
    }
}