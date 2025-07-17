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
    public class LiabilityHelper : IDisposable
    {
        private static readonly HttpClient _httpClient;

        static LiabilityHelper()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(setting.GamingSvr)
            };
        }

        public async Task<LiabilityResponse> Process(string OpenDate = "", string CloseDate = "")
        {
            LiabilityResponse data = new();

            if (await TokenHelper.GetToken())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                if (string.IsNullOrEmpty(OpenDate))  OpenDate  = DateTime.Now.AddDays(-1).ToString("MMM dd, yyyy 4:30:00");
                if (string.IsNullOrEmpty(CloseDate)) CloseDate = DateTime.Now.AddDays( 0).ToString("MMM dd, yyyy 4:30:00");

                var requestBody = new LiabilityRequest
                {
                    Vendor    = setting.Vendor,
                    Location  = setting.Location,
                    OpenDate  = OpenDate,
                    CloseDate = CloseDate
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("gam/Liability", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LiabilityResponse>(json);
                    if (result?.Status == "Valid")
                        data = result;
                }
            }

            return data;
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}