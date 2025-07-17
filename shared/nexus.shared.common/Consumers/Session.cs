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

//using Uno.Foundation;
//using Uno.Extensions;


namespace nexus.shared.common
{
    public static class Session
    {

        public static int ComputerId { get; set; }

        private static LoginResponse? _currentUser;

        public static LoginResponse? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnUserChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler? OnUserChanged;

        public static bool IsInRole(string roleName)
        {
            return _currentUser?.Role.ToString().Equals(roleName, StringComparison.OrdinalIgnoreCase) == true;
        }


        public static bool IsInRole(UserRoles role)
        {
            return _currentUser?.Role == role;
        }


        //public static string GetOrCreateDeviceId()
        //{
        //    var id = WebAssemblyRuntime.InvokeJS("localStorage.getItem('deviceId');");
        //    if (!string.IsNullOrEmpty(id))
        //        return id;

        //    var newId = Guid.NewGuid().ToString();
        //    WebAssemblyRuntime.InvokeJS($"localStorage.setItem('deviceId', '{newId}');");
        //    return newId;
        //}

    }
}


