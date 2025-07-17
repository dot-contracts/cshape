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
    public class PlayerHelper : IDisposable
    {
        private static readonly HttpClient _httpClient;

        static PlayerHelper()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(setting.GamingSvr)
            };
        }

        public async Task<List<Player>> Process()
        {
            List<Player> data = new();

            if (await TokenHelper.GetToken())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                var requestBody = new PlayerRequest
                {
                    Vendor   = setting.Vendor,
                    Location = setting.Location
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("gam/Player", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PlayerResponse>(json);
                    if (result?.Status == "Valid")
                        data = result.Results.ToList();
                }
            }

            return data;
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}