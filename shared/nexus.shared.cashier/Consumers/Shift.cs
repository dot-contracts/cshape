using System.Data;
using System.Text;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using nexus.common;
using nexus.common.dal;
using nexus.common.cache;

using nexus.shared.common;

namespace nexus.shared.cashier
{
    public class ShiftHelper : IDisposable
    {

        public async Task<DataTable> GetRoles(string RoleId="")
        {

            DataTable Roles = new DataTable();

            if (await TokenHelper.GetToken())
            {
                // Now that you have the token, you can use it to check for roles
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(setting.CommonSvr);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                    // Create a request body 
                    var requestBody = new { actiontype=0, setting.Vendor, setting.Location};

                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    // Send a POST request to check for tickets
                    var response = await httpClient.PostAsync("app/role", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();

                        // Parse JSON to JObject
                        var jsonObject = JObject.Parse(jsonString);

                        // Create DataTable
                        Roles = nextjson.ArrayToDataTable(jsonObject["roles"].ToArray());
                    }
                    else
                    {
                        //MessageBox.Show($"Roles check failed with status code: {response.StatusCode}");
                    }
                }
            }

            return Roles;
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
