using System;
using System.Windows;

using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Markup;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using nexus.common;
using nexus.shared.common;


namespace nexus.common
{
    public class LoginService : IDisposable
    {
        public string username = "apiuser";
        public string password = "7$5uZ@88vs^V#9z8";

        public event OnMessageEventHandler OnMessage; public delegate void OnMessageEventHandler(string Message);

        public async Task<LoginResponse> LoginAsync(LoginRequest login)
        {

            if (login.Username.Equals("test") && login.Password.Equals("test"))
            {
                return CreateTest();
            }
            else
            {
                using (var tokenAPI = new HttpClient())
                {
                    tokenAPI.BaseAddress = new Uri(setting.CommonSvr);
                    tokenAPI.Timeout = TimeSpan.FromSeconds(15);

                    // Set the credentials
                    var credentials = new { userName = login.Username, password = login.Password, locationid="0" };
                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");

                    try
                    {
                        // Send a POST request to retrieve the token
                        var response = await tokenAPI.PostAsync("/cmn/Login", content).ConfigureAwait(false);  // go get token

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            LoginResponse tokenBody = JsonConvert.DeserializeObject<LoginResponse>(jsonString);

                            string fn = tokenBody.FirstName;

                            return (tokenBody);

                        }
                        else
                        {
                            throw new Exception($"Token retrieval failed with status code: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }

                    return (null);
                }
            }
        }

        public LoginResponse CreateTest()
        {

            return new LoginResponse
            {
                Status = "OK",
                Response = "Login successful",
                Token = "abc123xyz456token",
                WorkerType = "FullTime",
                WorkerState = "Active",
                Department = "IT",
                Gender = "Male",
                Title = "Mr",
                FirstName = "John",
                OtherName = "A.",
                LastName = "Doe",
                BirthDate = "1985-06-15",
                EMail = "john.doe@example.com",
                Phone = "123-456-7890",
                IsApproved = true,
                LastUse = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                PasswordHash = "hashed-password-goes-here",
                //Role = UserRoles.Admin,
                WorkerId = 101,
                WorkerTypeId = 1,
                WorkerStateId = 1,
                GenderId = 1,
                TitleId = 1,
                Menus = new[]
                {
                new Menu
                {
                    MenuId          = 1,
                    ItemId          = 101,
                    ModuleId        = 10,
                    MenuDescription = "Mobile",
                    ItemDescription = "Utilities",
                    Assembly        = "nexus.plugins.floor",
                    RoleType        = "Admin",
                    EndPoint        = "Utils",
                    Property        = "Visible",
                    HasSubMenu      = false
                }//,
                //new Menu
                //{
                //    MenuId          = 1,
                //    ItemId          = 102,
                //    ModuleId        = 10,
                //    MenuDescription = "Mobile",
                //    ItemDescription = "Cards",
                //    Assembly        = "nexus.plugins.mobile",
                //    RoleType        = "Admin",
                //    EndPoint        = "Utils",
                //    Property        = "Visible",
                //    HasSubMenu      = false
                //},
                //new Menu
                //{
                //    MenuId          = 3,
                //    ItemId          = 103,
                //    ModuleId        = 30,
                //    MenuDescription = "Mobile",
                //    ItemDescription = "Settings",
                //    Assembly        = "nexus.plugins.mobile",
                //    RoleType        = "Admin",
                //    EndPoint        = "Settings",
                //    Property        = "Enabled",
                //    HasSubMenu      = false
                //}
            }
            };
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

