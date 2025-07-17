
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
    public class FeedHelper : IDisposable
    {

        public async Task<FeedResponse> Process(string RoleId = "")
        {

            // DataTable Feeds =  FeedHelper.CreateDataTable();

            FeedResponse Feeds = new FeedResponse();

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

                    // Send a POST request to get Feeds
                    var response = await httpClient.PostAsync("med/Feed", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();

                        try
                        {
                            // Deserialize response into FeedResponse object
                            Feeds = JsonSerializer.Deserialize<FeedResponse>(jsonString, new JsonSerializerOptions
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

            return Feeds;
        }


        public static DataTable CreateDataTable()
        {
            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("Description", typeof(string));
            dtNew.Columns.Add("FeedId",      typeof(int));
            dtNew.Columns.Add("FeedType",    typeof(string));
            dtNew.Columns.Add("FeedState",   typeof(string));
            dtNew.Columns.Add("FeedTypeId",  typeof(int));
            dtNew.Columns.Add("FeedStateId", typeof(int));

            dtNew = ScheduleHelpers.AddScheduleColumns(dtNew);

            return dtNew;
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
