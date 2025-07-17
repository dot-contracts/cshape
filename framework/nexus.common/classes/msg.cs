using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace nexus.common

{
    static public partial class msg
    {
        public delegate void OnStartupEventHandler (enums.EventTypes reason, string sender, string command, string Property); public static event OnPluginEventHandler OnStartup;
        public delegate void OnPluginEventHandler  (enums.EventTypes reason, string sender, string command, string Property); public static event OnPluginEventHandler OnPluginEvent;

        public static void RaisePluginEvent(enums.EventTypes reason, string sender, string command, string Property)
        {
            OnPluginEvent?.Invoke(reason, sender, command, Property);
        }

        public static void SetActionMenu(string moduleId, string ActionMenu)
        {
            //StackTrace stackTrace = new StackTrace();
            //OnPluginEvent?.Invoke(EventTypes.topmenu, stackTrace.GetFrame(1).GetMethod().Module + "." + stackTrace.GetFrame(1).GetMethod().Name, "Actions", ActionMenu);
            OnPluginEvent?.Invoke(enums.EventTypes.topmenu, moduleId, "Actions", ActionMenu);
        }

    }
}
