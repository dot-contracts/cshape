
using System.Data;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using nexus.common;
using nexus.common.dal;


namespace nexus.shared.common
{

    public class OptionHelper : IDisposable 
    {

        public string ParentId = string.Empty;
        public string ParentCode = string.Empty;
        public string OptionCode = string.Empty;
        private bool Computer;
        private bool ComputerRole;
        private bool Worker;
        private bool WorkerRole;

        public OptionRequest  requestBody  = new OptionRequest();
        public OptionResponse responseBody = new OptionResponse();

        public OptionHelper()
        {

        }

        public async Task GetOptions(string OptionPath = "", string Mask = "")
        {
            if (!string.IsNullOrEmpty(OptionPath) && OptionPath.Contains("."))
            {
                var parts = OptionPath.Split('.');
                ParentCode = parts[0];
                OptionCode = parts[1];
            }
            else
            {
                ParentCode = OptionPath;
            }

            Computer     = Mask.Length > 0 && Mask[0] == '1';
            ComputerRole = Mask.Length > 1 && Mask[1] == '1';
            Worker       = Mask.Length > 2 && Mask[2] == '1';
            WorkerRole   = Mask.Length > 3 && Mask[3] == '1';

            Option[] options = new Option[1];
            Option option = new Option
            {
                ParentCode = ParentCode,
                OptionCode = OptionCode//,
                                       //ComputerId = Session.ComputerId,
                                       //WorkerId   = Worker ? Session.CurrentUser.WorkerId : 0,
                                       //WorkerRoleId = RoleId
            };
            options[0] = option;

            requestBody = new OptionRequest
            {
                Vendor     = setting.Vendor,
                Location   = setting.Location,
                ActionType = 0, // Assuming ActionType is required; update if needed
                Options    = options,
            };

            // Call the actual API-backed GetOptions
            await Process(requestBody);
        }

        public async Task PutOptions()
        {
            requestBody.ActionType = 1;
            requestBody.Options = responseBody.Options.ToArray(); // Preserve all values

            // Call the actual API-backed GetOptions
            await Process(requestBody);
        }

        public async Task Process(OptionRequest requestBody)
        {
            if (await TokenHelper.GetToken())
            {
                // Now that you have the token, you can use it to check for roles
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(setting.CommonSvr);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHelper.token);

                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    // Send a POST request to check for tickets
                    var response = await httpClient.PostAsync("/cmn/Option", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        responseBody = JsonSerializer.Deserialize<OptionResponse>(jsonString, options);
                    }
                    else
                    {
                        //MessageBox.Show($"Roles check failed with status code: {response.StatusCode}");
                    }
                }
            }
        }

        public string GetValue(string optionCode = "", string varDefault = "", string parentCode = "", int? parentId = null)
        {
            // Try to find an existing option by OptionCode and ParentCode (case-insensitive)
            var found = responseBody.Options.FirstOrDefault(o => string.Equals(o.OptionCode, optionCode, StringComparison.OrdinalIgnoreCase));

            if (found != null)
            {
                // Use VarDefault if ValueText is empty
                return string.IsNullOrEmpty(found.ValueText) && !string.IsNullOrEmpty(found.VarDefault) ? found.VarDefault : found.ValueText;
            }

            // Create and add a new Option entry since it doesn't exist
            responseBody.Options.Add(new Option
            {
                ParentCode = parentCode,
                OptionCode = optionCode,
                VarDefault = varDefault,
                ParentId   = parentId ?? 0
            });

            // Return the default value
            return varDefault;
        }

        public void SetValue(string optionCode, string valueText, string parentCode = "", int? parentId = null)
        {
            bool found = false;

            foreach (var option in responseBody.Options)
            {
                if (string.Equals(option.OptionCode, optionCode, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(option.ValueText, valueText, StringComparison.Ordinal))
                    {
                        option.ValueText = valueText;
                    }
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var newOption = new Option
                {
                    OptionCode = optionCode,
                    ValueText = valueText,
                    VarDefault = valueText,
                    ParentCode = parentCode,
                    ParentId = parentId ?? 0
                    // Optionally mark as dirty or assign other fields
                };

                responseBody.Options.Add(newOption);
            }
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
                    // TODO: free managed resources when explicitly called
                }

                // TODO: free shared unmanaged resources
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
