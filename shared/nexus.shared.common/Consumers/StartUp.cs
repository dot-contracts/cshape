
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

using nexus.common;
using nexus.common.dal;
using nexus.common.cache;


namespace nexus.shared.common
{

    public static class StartUpHelper
    {

        public static async Task GetEnums(string RoleId = "")
        {

            DataTable Enums = new DataTable();

            if (await TokenHelper.GetToken())
            {
                // Now that you have the token, you can use it to check for roles
                using (var httpClient = new HttpClient())
                {
                    //httpClient.BaseAddress = new Uri(nexus.common.dal.setting.CommonSvr);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                    // Create a request body 
                    var requestBody = new { actiontype = 0, setting.Vendor, setting.Location };

                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    // Send a POST request to check for tickets
                    var response = await httpClient.PostAsync("/cmn/StartUp", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();

                        // Parse JSON to JObject
                        var jsonObject = JObject.Parse(jsonString);

                        // Create DataTable
                        Enums = nextjson.ArrayToDataTable(jsonObject["StartUps"].ToArray());
                    }
                    else
                    {
                        //MessageBox.Show($"Roles check failed with status code: {response.StatusCode}");
                    }
                }
            }

            EnumCache.Instance.CacheTable  = Enums;
            EnumCache.Instance.CacheLoaded = true;
        }
    }
}
