
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.System;

using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

using System.Collections.Generic;
using System.Threading.Channels;
using System;

namespace nexus.common.control
{

    public class Values
    {
        public string Description { get; set; } = string.Empty;
        public string Id          { get; set; } = string.Empty;
    }

    public class KeyDownEventArgs
    {
        public VirtualKey Key   { get; set; }
        public bool       Valid { get; set; } = true;

        public KeyDownEventArgs(VirtualKey Key)
        {
            this.Key        = Key;
        }
    }

    public class ChangeEventArgs
    {
        public string Tag        { get; set; }
        public string Display    { get; set; }
        public string Value      { get; set; }
        public bool   Valid      { get; set; } = true;

        public ChangeEventArgs(string Tag, string Display, string Value)
        {
            this.Tag        = Tag;
            this.Display    = Display;
            this.Value      = Value;
        }
    }

    public class ChangedEventArgs
    {
        public string  Tag        { get; set; }
        public string  Display    { get; set; }
        public string  Value      { get; set; }
        public WinRect ScreenRect { get; set; }

        public ChangedEventArgs(string Tag, string Display, string Value, WinRect ScreenRect)
        {
            this.Tag        = Tag;
            this.Display    = Display;
            this.Value      = Value;
            this.ScreenRect = ScreenRect;
        }
    }

    public class ClickedEventArgs
    {
        public string  Tag        { get; set; }
        public bool    Value      { get; set; }
        public WinRect ScreenRect { get; set; }

        public ClickedEventArgs(string Tag, bool Value, WinRect ScreenRect)
        {
            this.Tag        = Tag;
            this.Value      = Value;
            this.ScreenRect = ScreenRect;
        }
    }

    public class CloseEventArgs
    {
        public string  Value { get; set; } = string.Empty;
        public bool    Valid { get; set; } = true;

        public CloseEventArgs(string Value)
        {
            this.Value = Value;
        }
    }

}
