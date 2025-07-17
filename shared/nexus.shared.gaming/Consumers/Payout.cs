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
    public class PayoutHelper : IDisposable
    {
        private static readonly HttpClient _httpClient;

        static PayoutHelper()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(setting.GamingSvr)
            };
        }

        public async Task<List<Payout>> Process(string OpenDate = "", string CloseDate = "")
        {
            List<Payout> data = new();

            if (await TokenHelper.GetToken())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                if (string.IsNullOrEmpty(OpenDate))  OpenDate  = DateTime.Now.           ToString("MMM dd, yyyy 4:30:00");
                if (string.IsNullOrEmpty(CloseDate)) CloseDate = DateTime.Now.AddDays(1).ToString("MMM dd, yyyy 4:30:00");

                var requestBody = new PayoutRequest
                {
                    Vendor    = setting.Vendor,
                    Location  = setting.Location,
                    OpenDate  = OpenDate,
                    CloseDate = CloseDate
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("gam/Payout", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PayoutResponse>(json);
                    if (result?.Status == "Valid")
                        data = result.Results.ToList();
                }
            }

            return data;
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}