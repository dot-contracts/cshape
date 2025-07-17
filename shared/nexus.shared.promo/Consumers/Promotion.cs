using nexus.common;
using nexus.common.cache;
using nexus.common.dal;
using nexus.shared.common;
using System;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using nexus.common;

namespace nexus.shared.promo
{
    public class PromotionHelper : IDisposable
    {


        public async Task<PromotionResponse> Process(PromotionRequest request)
        {

            PromotionResponse Items = new PromotionResponse();

            if (await TokenHelper.GetToken())
            {
                // Now that you have the token, you can use it to check for roles
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(setting.PromoSvr);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    // Send a POST request to check for Promotions
                    var response = await httpClient.PostAsync("/pro/Promotion", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();

                        try
                        {
                            // Deserialize response into FeedResponse object
                            Items = JsonSerializer.Deserialize<PromotionResponse>(jsonString, new JsonSerializerOptions
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

            return Items;
        }

        public async static Task<DataTable> CreateDataTableFromClass<T>() where T : class, new()
        {
            DataTable dataTable = new DataTable("Promotion");// typeof(T).Name);

            // Get all public properties of the class
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(prop => prop.CanRead && prop.PropertyType != typeof(Schedule)); // Ignore Schedule property

            foreach (var prop in properties)
            {
                // Handle nullable types
                Type columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, columnType);
            }

            // Get all public properties of the schedule class
            properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(prop => prop.CanRead && prop.PropertyType == typeof(Schedule)); // Ignore Schedule property
            foreach (var prop in properties)
            {
                // Handle nullable types
                Type columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (!dataTable.Columns.Contains(prop.Name))
                   dataTable.Columns.Add(prop.Name, columnType);
            }

            return dataTable;
        }

        public async static Task<DataTable> GetDataFromResponse( PromotionResponse response , DataTable PromoData)
        {
            PropertyInfo[] PromoProps = typeof(Promotion).GetProperties();
            PropertyInfo[] SchedProps = typeof(Schedule) .GetProperties();

            foreach (var Promo in response.Promotions)
            {
                DataRow row = PromoData.NewRow();

                foreach (PropertyInfo property in PromoProps)
                {
                    try
                    {
                        if (property.Name != "Schedule")
                        {
                            object? value = property.GetValue(Promo) ?? DBNull.Value;
                            row[property.Name] = value;
                        }
                    }
                    catch (Exception ex) { string m = ex.Message; }
                }

                //if (Promo.Schedule.ScheduleId>0)
                {
                    foreach (PropertyInfo property in SchedProps)
                    {
                        try
                        {
                            if (PromoData.Columns.Contains(property.Name))
                            {
                                object? value = property.GetValue(Promo.Schedule) ?? DBNull.Value;
                                row[property.Name] = value;
                            }
                        }
                        catch (Exception ex) { string m = ex.Message; }
                    }
                }

                PromoData.Rows.Add(row);
            }

            return PromoData;
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
