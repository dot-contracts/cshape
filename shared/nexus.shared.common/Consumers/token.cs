
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using nexus.common;
using nexus.common.dal;
using nexus.common.cache;

namespace nexus.shared.common
{
    public static class TokenHelper
    {
        public static string token = string.Empty;
        public static bool status;

        public static async Task<bool> GetToken(string username = "man", string password = "man123", string locationId="0")
        {
            if (token != string.Empty && status) return true;

            status = false;

            using (var tokenAPI = new HttpClient())
            {
                tokenAPI.BaseAddress = new Uri(setting.CommonSvr);
                tokenAPI.Timeout = TimeSpan.FromSeconds(350);

                //

                // Set the credentials
                var credentials = new { userName = username, password = password, locationId = locationId };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
                try
                {
                    // Send a POST request to retrieve the token
                    var response = await tokenAPI.PostAsync("/cmn/getToken", content).ConfigureAwait(false); // go get token
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
                        LoginResponse tokenBody = JsonConvert.DeserializeObject<LoginResponse>(jsonString);

                        token = tokenBody.Token;
                        status = true;

                        return status;
                    }
                }
                
                catch (Exception ex) { helpers.WriteToLog("GetToken: " + ex.Message); }
            }
            return status;
        }
    }
}
