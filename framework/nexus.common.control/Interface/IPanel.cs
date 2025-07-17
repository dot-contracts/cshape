using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nexus.common.control
{
    //public delegate void    OnPanelEventHandler( string owner, PanelEventArgs reason );
    //public class            PanelEventArgs : EventArgs 
    //{
    //    public  PanelEventType  source;

    //    public PanelEventArgs( PanelEventType src )
    //    {
    //        source = src;
    //    }
    //}

    public interface IPanel
    {
        //event           OnPanelEventHandler OnChange;

        #region Properties
        bool         IsInitialized   { get; }
        bool         IsLoaded        { get; }
        bool         IsChanged       { get; }
        bool         IsValid         { get; }
        bool         IsActive        { get; }

        //PropertyBag<NameObject> Context { get; set; }
        #endregion

        void            Create();       // Initialize
        void            Reset();        // Clear all loaded data and re-initialize
        void            Clear();        // Clear current context data
        bool         Load();         // Load context specified data
        bool         Save();         // Save any/all changed data <IsChanged>
    }
}
