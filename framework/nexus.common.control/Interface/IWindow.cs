using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nexus.common.control
{
    //public delegate void    OnSnapinEventHandler( string owner, SnapinEventArgs reason );
    //public class            SnapinEventArgs : EventArgs 
    //{
    //    public SnapinEventType  source;

    //    public SnapinEventArgs( SnapinEventType src )
    //    {
    //        source = src;
    //    }
    //}

    public interface IWindow
    {
        //event           OnSnapinEventHandler OnChange;

        #region Properties
        bool         IsInitialized   { get; }
        bool         IsLoaded        { get; }
        bool         IsChanged       { get; }
        bool         IsValid         { get; }
        bool         IsActive        { get; }

        //PropertyBag<NameObject> Context { get; set; }
        #endregion

        void            Reset();        // Clear all loaded data and re-initialize
        void            Clear();        // Clear current context data
        bool         Load();         // Load context specified data
        bool         Save();         // Save any/all changed data <IsChanged>
    }
}
