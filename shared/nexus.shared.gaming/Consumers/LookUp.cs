using System.Data;
using System.Text;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using nexus.common;
using nexus.shared.gaming;
using nexus.shared.common;
using nexus.common.dal;
using nexus.common.cache;

namespace nexus.shared.gaming
{
public class LookUpHelper : IDisposable
{
    private static readonly HttpClient _httpClient;

    static LookUpHelper()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(setting.GamingSvr)
        };
    }

    public async Task<List<LookUp>> Process(string Search = "HouseNo", string Value = "")
    {
        List<LookUp> LookUps = new List<LookUp>();

        if (await TokenHelper.GetToken())
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

            var requestBody = new LookUpRequest()
            {
                ActionType  = 0,
                Vendor      = setting.Vendor,
                Location    = setting.Location,
                Search      = Search,
                Value       = Value
            };

            var content = new StringContent( JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json" );

            var response = await _httpClient.PostAsync("gam/LookUp", content);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                var lookUpResponse = JsonConvert.DeserializeObject<LookUpResponse>(jsonString);

                if (lookUpResponse        != null    &&
                    lookUpResponse.Status == "Valid" &&
                    lookUpResponse.Results.Length > 0)
                {
                    LookUps = lookUpResponse.Results.ToList();
                }
            }
            else
            {
                // Optional: handle or log error
            }
        }

        return LookUps;
    }

    #region IDisposable Implementation
    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // Cleanup managed resources here if needed
            }
        }
        this.disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}

}
