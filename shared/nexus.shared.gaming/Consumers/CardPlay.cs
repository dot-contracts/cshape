using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using nexus.common;
using nexus.common.cache;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.gaming;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nexus.shared.gaming
{
    public class CardPlayHelper : IDisposable
    {

        private static readonly HttpClient _httpClient;

        static CardPlayHelper()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(setting.GamingSvr)
            };
        }

        public async Task<List<CardPlay>> Process(string ActionType = "0")
        {
            List<CardPlay> data = new();

            if (await TokenHelper.GetToken())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                var requestBody = new CardPlayRequest
                {
                    ActionType = helpers.ToInt(ActionType),
                    Vendor     = setting.Vendor,
                    Location   = setting.Location
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("gam/cardplay", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CardPlayResponse>(json);
                    if (result?.Status == "Valid")
                        data = result.Results.ToList();
                }
            }

            return data;
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
