using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nexus.common.control;


namespace nexus.common.control
{

    //public delegate void    OnMainUIEventHandler  ( NexusEnums.EventTypes reason, string sender, string command, string Property);
    //public class            MainUIEventArgs : EventArgs 
    //{
    //    private NexusEnums.EventTypes eventType; public MainUIEventArgs(NexusEnums.EventTypes Type) {eventType = Type; }
    //}

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] // multiuse attribute
    public class NexusMainUIAttribute : Attribute
    {
        public string      GUID { get; set; }
        public MainUITypes Type { get; set; }

        public NexusMainUIAttribute(string guid, MainUITypes Type) : base()
        {
            GUID = guid;
            this.Type = Type;
        }
    }

    public interface IMainUI
    {
        //event           OnPluginEventHandler OnChange;

        #region Properties
        bool            IsLoadComplete { get; set; }
        bool            IsActive       { get; set; }
        bool            IsBusy         { get; set; }
        #endregion

        Task            Create(string PluginName, string Property = "", string hubURL = "http://localhost:9010/pluginhub"); // Instantiate and pass back properties if needed
        void            Reset();                                                         // Clear all loaded data and re-initialize
        bool            Save();                                                          // Save any/all changed page specific control data
        bool            Execute(WinRect ScreenRect, enums.EventTypes Reason, string SourcePlugin, string Command, string Parameters = "", string DestinationPlugin = "");   // Execute a command
        void            Resize (WinSize PanelSize);                      // Resize this and sub panels
        bool            Process();                                                       // Process any threads

    }
}
