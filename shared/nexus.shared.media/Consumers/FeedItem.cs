using System;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


using nexus.common;
using nexus.common.dal;
using nexus.shared.common;
using nexus.common.cache;

namespace nexus.shared.media
{
    public class FeedItemHelper : IDisposable
    {

        public async Task<FeedItemResponse> Process(string RoleId = "", string FeedId ="")
        {
            FeedItemRequest feedItemRequest = new FeedItemRequest
            {
                ActionType = 0,
                Vendor     = setting.Vendor,
                Location   = setting.Location,
                FeedItem   = new FeedItem
                {
                    FeedId = Convert.ToInt32(FeedId)
                }
            };

            return await Process(feedItemRequest);
        }

        public async Task<FeedItemResponse> Process(FeedItemRequest request)
        {

            FeedItemResponse Items = new FeedItemResponse();

            if (await TokenHelper.GetToken())
            {
                // Now that you have the token, you can use it to check for roles
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(setting.MediaSvr);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    // Send a POST request to check for feeditems
                    var response = await httpClient.PostAsync("med/FeedItem", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();

                        try
                        {
                            // Deserialize response into FeedResponse object
                            Items = JsonSerializer.Deserialize<FeedItemResponse>(jsonString, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                        }

                    }
                    else
                    {
                        helpers.WriteToLog($"FeedHelper check failed with status code: {response.StatusCode}");
                    }
                }
            }

            return Items;
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
