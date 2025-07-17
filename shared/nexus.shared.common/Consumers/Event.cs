using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

using nexus.common;
using nexus.shared.gaming;
using nexus.shared.common;
using nexus.common.dal;
using nexus.common.cache;

namespace nexus.shared.common
{
    public class EventHelper : IDisposable
    {
        private static readonly HttpClient _httpClient;

        static EventHelper()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(setting.CommonSvr)
            };
        }

        public async Task<List<Event>> Process(string OpenDate = "", string CloseDate = "", string MaxRows = "")
        {
            List<Event> data = new();

            if (await TokenHelper.GetToken())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                var requestBody = new EventRequest
                {
                    Vendor    = setting.Vendor,
                    Location  = setting.Location,
                    OpenDate  = OpenDate,
                    CloseDate = CloseDate,
                    MaxRows   = MaxRows
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("cmn/Event", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<EventResponse>(json);
                    if (result?.Status == "Valid")
                        data = result.Results.ToList();
                }
            }

            return data;
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}