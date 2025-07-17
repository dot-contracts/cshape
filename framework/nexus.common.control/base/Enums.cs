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
    public enum PackageTypes { None, WinForm, WinControl, WPFControl, WPFForm, WebForm }
    public enum MainUITypes  { None, WinForm, WinControl, WPFControl, WPFForm, WebForm }
    public enum PluginTypes  { None, WinForm, WinControl, WPFControl, WPFForm, WebForm }
    public enum SnapinTypes  { None, WinForm, WinControl, WPFControl, WPFForm, WebForm }

    public enum DialogTypes          { standard, wizard }
    public enum ControlAlignments    { Left, Right }
    public enum HorizontalAlignments { Left, Center, Right, Justify }
    public enum VerticalAlignments   { Top, Center, Bottom }
    public enum BorderTypes          { Normal, OK, Cancel, New, Edit, Save, Delete }
    public enum BorderColors         { blue, green, grey, red, yellow, one }
    public enum GridTypes            { SelectGrid, DropList }
    public enum GridFixedRows        { none, bottom, top }
    public enum GridFixedColumns     { none, left, right }
    public enum ButtonTypes          { Normal, Tab }
    public enum MaskTypes            { None, NumericOnly, Numeric09, AlphaOnly, AlphaUpperCase, AlphaLowerCase, AlphaTitleCase, AlphaNameCase, AlphaNumeric, Date, DateTime, Password, PhoneWithArea, IpAddress, TwoDigitDecimal }
    public enum VisiStates           { Visible, Hidden, Collapsed}
    public enum AnimationTypes       { None, Border, Full }
    public enum SelectModes          { Single, Multiple }
}
