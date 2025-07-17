using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

using nexus.common;
using nexus.shared.gaming;
using nexus.shared.common;
using nexus.common.dal;
using nexus.common.cache;

namespace nexus.shared.gaming
{
    public class TodayHelper : IDisposable
    {
        private static readonly HttpClient _httpClient;

        static TodayHelper()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(setting.GamingSvr)
            };
        }

        public async Task<List<Today>> Process()
        {
            List<Today> data = new();

            if (await TokenHelper.GetToken())
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                var requestBody = new TodayRequest
                {
                    ActionType = 1,
                    Vendor     = setting.Vendor,
                    Location   = setting.Location,
                    EgmID      = 0
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("gam/today", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TodayResponse>(json);
                    if (result?.Status == "Valid")
                        data = result.Results.ToList();
                }
            }

            return data;
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}