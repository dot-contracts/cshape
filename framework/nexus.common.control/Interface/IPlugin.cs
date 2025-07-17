using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nexus.common.control;

namespace nexus.common.control
{

    //public delegate void OnPluginEventHandler(PluginEventType reason, string sender, string command, string Property);
    //public class PLuginEventArgs : EventArgs
    //{
    //    public PluginEventType source;
    //    public NexusEnums. PluginEventArgs(PLuginEventType src) { source = src; }
    //}

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] // multiuse attribute
    public class NexusPluginAttribute : Attribute
    {
        public string                  GUID { get; set; }
        public PluginTypes  Type { get; set; }

        public NexusPluginAttribute(string guid, PluginTypes type) : base()
        {
            GUID = guid;
            Type = type;
        }
    }


    //public delegate void    OnPluginEventHandler(PluginEventType reason, string sender, string command, string Property);
    //public class            PluginEventArgs : EventArgs 
    //{
    //    public PluginEventType  source;

    //    public PluginEventArgs( PluginEventType src )
    //    {
    //        source = src;
    //    }
    //}
   
    public interface IPlugin
    {

        #region Properties
        bool            IsLoadComplete { get; set; }
        bool            IsActive       { get; set; }
        bool            IsBusy         { get; set; }
        #endregion
     
        bool            Create(PropertyBag<NameValue> PropBag);       // Instantiate 
        bool            Load(int LoadState, string Property);         // Load page specific control data
        void            Reset();                                      // Clear all loaded data and re-initialize
        bool            Save();                                       // Save any/all changed page specific control data
        bool            Execute(WinRect ScreenRect, enums.EventTypes Reason, string SourcePlugin, string Command, string Parameters = "", string DestinationPlugin = "");   // Execute a command
        void            Resize (WinSize PanelSize);                      // Resize this and sub panels
        bool            Process();                                    // Process loop
    }
}
