
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using nexus.common;
using nexus.shared.common;
using nexus.common.dal;
using nexus.common.cache;

namespace nexus.shared.media
{
    public class FeedSourceHelper : IDisposable
    {

        public async Task<DataTable> Process(string RoleId = "")
        {

            DataTable FeedSources = new DataTable();

            if (await TokenHelper.GetToken())
            {
                // Now that you have the token, you can use it to check for roles
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(setting.MediaSvr);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                    // Create a request body 
                    var requestBody = new { actiontype = 0, setting.Vendor, setting.Location };

                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    // Send a POST request to check for tickets
                    var response = await httpClient.PostAsync("med/FeedSource", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();

                        // Parse JSON to JObject
                        var jsonObject = JObject.Parse(jsonString);

                        // Create DataTable
                        FeedSources = nextjson.ArrayToDataTable(jsonObject["FeedSources"].ToArray());
                    }
                    else
                    {
                        //MessageBox.Show($"Roles check failed with status code: {response.StatusCode}");
                    }
                }
            }

            return FeedSources;
        }


        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }
            }
            this.disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

}
