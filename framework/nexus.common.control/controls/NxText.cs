//using Microsoft.Extensions.Options;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic;
using nexus.common;
using nexus.common.control.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Windows.System;
using Windows.UI.Core;

namespace nexus.common.control
{
	public sealed partial class NxText : NxControlBase 
	{

        private string                        _realText = string.Empty;

        public enum ChangeEventTypes  {ByKeyPress,OnLostFocus,OnEnterKey,OnValidEntry,None}

        private        bool isLoaded     = false;
        private        bool hasFocus     = false;
        private        bool suppressONC  = false; 
        private        bool isValid      = false; public bool IsValid      { get { return isValid; }      set { isValid = value; } }
        private        bool isValidated  = false; 
        private        bool isValidating = false; public bool IsValidating { get { return isValidating; } set { isValidating = value; } }
        private        bool isTouchMode  = false; public bool IsTouchMode  { get { return isTouchMode; }  set { isTouchMode = value; } }
        private static bool isChanged    = false; public bool IsChanged    { get { return isChanged; }    set { isChanged = value; } }

        #region Editor Dependency Property
        public static readonly     DependencyProperty IsReadOnlyProperty               = DependencyProperty.Register("IsReadOnly",               typeof(bool),                    typeof(NxText), new PropertyMetadata(false));
        public static readonly     DependencyProperty IsRequiredProperty               = DependencyProperty.Register("IsRequired",               typeof(bool),                    typeof(NxText), new PropertyMetadata(false));
        public static readonly     DependencyProperty SuppressNavigateProperty         = DependencyProperty.Register("SuppressNavigate",         typeof(bool),                    typeof(NxText), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyFontFamilyProperty          = DependencyProperty.Register("ReplyFontFamily",          typeof(FontFamily),              typeof(NxText), new PropertyMetadata(""));
        public static readonly     DependencyProperty ReplyFontSizeProperty            = DependencyProperty.Register("ReplyFontSize",            typeof(double),                  typeof(NxText), new PropertyMetadata(12));

        public static readonly     DependencyProperty ReplyInvalidPictureProperty      = DependencyProperty.Register("ReplyInvalidPicture",      typeof(Image),                   typeof(NxText), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyValidPictureProperty        = DependencyProperty.Register("ReplyValidPicture",        typeof(Image),                   typeof(NxText), new PropertyMetadata(null));

        public static readonly     DependencyProperty ReplyRequiredTextProperty        = DependencyProperty.Register("ReplyRequiredText",        typeof(string),                  typeof(NxText), new PropertyMetadata(String.Empty));
        public static readonly     DependencyProperty MaxTextLengthProperty            = DependencyProperty.Register("MaxTextLength",            typeof(int),                     typeof(NxText), new PropertyMetadata(0));
        public static readonly     DependencyProperty MaskTypeProperty                 = DependencyProperty.Register("MaskType",                 typeof(MaskTypes),               typeof(NxText), new PropertyMetadata(MaskTypes.None, OnMaskTypeChanged)); 
        public static readonly     DependencyProperty MaskProperty                     = DependencyProperty.Register("Mask",                     typeof(string),                  typeof(NxText), new PropertyMetadata(""));
        public static readonly     DependencyProperty ChangeEventTypeProperty          = DependencyProperty.Register("ChangeEventType",          typeof(NxText.ChangeEventTypes), typeof(NxText), new PropertyMetadata(NxText.ChangeEventTypes.OnEnterKey));
        public static readonly     DependencyProperty ReplyHorizontalAlignmentProperty = DependencyProperty.Register("ReplyHorizontalAlignment", typeof(HorizontalAlignment),     typeof(NxText), new PropertyMetadata(HorizontalAlignments.Left));
        public static readonly     DependencyProperty ReplyVerticalAlignmentProperty   = DependencyProperty.Register("ReplyVerticalAlignment",   typeof(VerticalAlignment),       typeof(NxText), new PropertyMetadata(VerticalAlignments.Top));
        public static readonly     DependencyProperty ReplyTextWrappingProperty        = DependencyProperty.Register("ReplyTextWrapping",        typeof(TextWrapping),            typeof(NxText), new PropertyMetadata(TextWrapping.Wrap));
        public static readonly     DependencyProperty ReplyTextAutofitProperty         = DependencyProperty.Register("ReplyTextAutofit",         typeof(bool),                    typeof(NxText), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyMaxFontSizeProperty         = DependencyProperty.Register("ReplyMaxFontSize",         typeof(double),                  typeof(NxText), new PropertyMetadata(0));


        public new  bool                    IsReadOnly               { get { return (bool)                    GetValue(IsReadOnlyProperty); }               set { SetValue(IsReadOnlyProperty,               value); } }
        public      bool                    IsRequired               { get { return (bool)                    GetValue(IsRequiredProperty); }               set { SetValue(IsRequiredProperty,               value); } }
        public      bool                    SuppressNavigate         { get { return (bool)                    GetValue(SuppressNavigateProperty); }         set { SetValue(SuppressNavigateProperty,         value); } }
        public      FontFamily              ReplyFontFamily          { get { return (FontFamily)              GetValue(ReplyFontFamilyProperty); }          set { SetValue(ReplyFontFamilyProperty,          value); } }
        public      double                  ReplyFontSize            { get { return (double)                  GetValue(ReplyFontSizeProperty); }            set { SetValue(ReplyFontSizeProperty,            value); } }

        public      Image                   ReplyInvalidPicture      { get { return (Image)                   GetValue(ReplyInvalidPictureProperty);}       set { SetValue(ReplyInvalidPictureProperty,      value); } }
        public      Image                   ReplyValidPicture        { get { return (Image)                   GetValue(ReplyValidPictureProperty);}         set { SetValue(ReplyValidPictureProperty,        value); } }

        public      string                  ReplyRequiredText        { get { return (string)                  GetValue(ReplyRequiredTextProperty); }        set { SetValue(ReplyRequiredTextProperty,        value); } }
        public int                          MaxTextLength            { get { return (int)                     GetValue(MaxTextLengthProperty); }            set { SetValue(MaxTextLengthProperty,            value); } }
        public      MaskTypes               MaskType                 { get { return (MaskTypes)               GetValue(MaskTypeProperty); }                 set { SetValue(MaskTypeProperty,                 value); } }
        public      string                  Mask                     { get { return (string)                  GetValue(MaskProperty); }                     set { SetValue(MaskProperty,                     value); } }
        public      NxText.ChangeEventTypes ChangeEventType          { get { return (NxText.ChangeEventTypes) GetValue(ChangeEventTypeProperty); }          set { SetValue(ChangeEventTypeProperty,          value); } }
        public      HorizontalAlignment     ReplyHorizontalAlignment { get { return (HorizontalAlignment)     GetValue(ReplyHorizontalAlignmentProperty); } set { SetValue(ReplyHorizontalAlignmentProperty, value); } }
        public      VerticalAlignment       ReplyVerticalAlignment   { get { return (VerticalAlignment)       GetValue(ReplyVerticalAlignmentProperty); }   set { SetValue(ReplyVerticalAlignmentProperty,   value); } }
        public      TextWrapping            ReplyTextWrapping        { get { return (TextWrapping)            GetValue(ReplyTextWrappingProperty); }        set { SetValue(ReplyTextWrappingProperty,        value); } }
        public      bool                    ReplyTextAutofit         { get { return (bool)                    GetValue(ReplyTextAutofitProperty); }         set { SetValue(ReplyTextAutofitProperty,         value); } }
        public      double                  ReplyMaxFontSize         { get { return (double)                  GetValue(ReplyMaxFontSizeProperty); }         set { SetValue(ReplyMaxFontSizeProperty,         value); } }
        
        public Brush ReplyBackColor        { get => ReplyBack;    set => ReplyBack  = value; }
        public Brush ReplyForeColor        { get => ReplyFore;    set => ReplyFore  = value; }
        public Brush ReplyInvalidBackColor { get => ReplyInValid; set => ReplyInValid = value; }
        public Brush ReplyValidBackColor   { get => ReplyValid;   set => ReplyValid = value; }
        
        #endregion

        #region Mask Property

        private static string NullMaskString = "<>";
        private static char   promptChar = '_';

        private bool mMaskIsNull = true;
        private MaskedTextProvider mMaskProvider;  public MaskedTextProvider MaskedProvider { get { if (!mMaskIsNull) { return mMaskProvider.Clone() as MaskedTextProvider; } else { return null; } } }

        #endregion Mask Property

        #region Property
        public    TextBox       Editor      { get { return base.GetTemplateChild("Editor")            as TextBox; } }
        #endregion

        #region Value

        static  string OriginalValue = string.Empty;
        public  int    ValueInt { get { return helpers.ToInt(Value); } }
        public  double ValueDbl { get { return helpers.ToDbl(Value); } }

        public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value),typeof(string),typeof(NxText),new PropertyMetadata(string.Empty, OnValueChanged));
        #endregion 

        #region Event

        public delegate void OnChangedControlEventHandler (NxText Ctl,string Tag, string Display, string Value, ref bool Valid);   public event OnChangedControlEventHandler OnChangedControl;

        public delegate void OnProcessKeyDownEventHandler (VirtualKey Key, ref bool Handled);                                     public event OnProcessKeyDownEventHandler OnProcessKeyDown;
        public delegate void OnProcessKeyEventHandler     (string Key);                                                           public event OnProcessKeyEventHandler     OnProcessKey;
        public delegate void OnInValidEventHandler        (string errorMsg);                                                      public event OnInValidEventHandler        OnInValid;
        public delegate void OnDispose();
        #endregion

        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NxText sender = obj as NxText;
            if (sender.Editor != null)
            {
                sender.Editor.Text = e.NewValue as string;
                sender.Validate();
            }
        }

        public static void OnMaskTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NxText sender = obj as NxText;
            sender.Validate();
        }

        #region NxText

        public NxText()
        {
            DefaultStyleKey = typeof(NxText);

            if (Tag == null) Tag = string.Empty;
            this.IsTabStop    = true;
            this.Loaded      += _Loaded;
            this.GotFocus    += _GotFocus;
            this.SizeChanged += _SizeChanged;

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyThemeDefaults();
        }
        private void ApplyThemeDefaults()
        {
            if (Theme != null)
            {
                ReplyForeColor = NxThemeManager.Current.GetThemeBrush("NxReplyFore", Colors.White);
                ReplyBackColor = NxThemeManager.Current.GetThemeBrush("NxReplyBack", Colors.White);
            }
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            if (Editor != null)
            {
                // Initialize the text
                Editor.KeyUp         += _KeyUp;
                Editor.KeyDown       += _KeyDown;
                Editor.LosingFocus   += _LosingFocus;
                Editor.LostFocus     += _LostFocus;
                Editor.GotFocus      += _GotFocus;
                Editor.TextChanged   += Editor_TextChanged;

                // Configure max length
                Editor.MaxLength = MaxTextLength > 0 ? MaxTextLength : 0;

                // Set foreground color (inherited from base, if desired)
                //Editor.Foreground = new SolidColorBrush(ForeColor);
            }

            isChanged   = false;
            mMaskIsNull = true;
            isLoaded    = true;

        }

        private void _Unloaded(object sender, RoutedEventArgs e)
        {
            if (Editor != null)
            {
                Editor.KeyDown     -= _KeyDown;
                Editor.LosingFocus -= _LosingFocus;
                Editor.LostFocus   -= _LostFocus;
                Editor.GotFocus    -= _GotFocus;
            }
        }

        public void SetFocus()
        {
            try
            {
                this.Focus(FocusState.Programmatic);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void _GotFocus(object sender, RoutedEventArgs e)
        {
            OriginalValue = Value;
            if (Editor != null)
            {
                Editor.Background = this.ReplyBackColor;
            }
        }


        private void _LostFocus(object sender, RoutedEventArgs e)
        {
            if (!OriginalValue.Equals(Value))
                if (RaiseOnChange(Value))
                    RaiseOnChanged(Value);
            if (!OriginalValue.Equals(Value)) { }
                //OnChanged?.Invoke(Tag, "", Value, ref isValid); // Notify that the value has changed    
            if( Editor != null)
            {
               //Editor.Background = this.ReplyLostBackColor;
            }
        }


        private void _SizeChanged(object sender, SizeChangedEventArgs e) { Resize(); }

        public void Resize()
        {
            try
            {
                if (Editor != null)      { Editor.Height = ActualHeight;      Editor.Width = ActualWidth; }
            }
            catch (Exception x)
            { }
        }
        public void SetFontSize()
        {
            if (Editor != null)
            {
                if (ReplyMaxFontSize > 0)
                {
                    //ReplyFontSize = helpers.MakeTextFit(Editor, ReplyMaxFontSize);
                    if (ReplyFontSize > 0)
                        Editor.FontSize = ReplyFontSize;
                }
            }
        }
        #endregion

        #region Private TextBox Methods 

        private void _KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (MaskType == MaskTypes.Password)
            {
                e.Handled = true;

                int caret = Editor.SelectionStart;

                switch (e.Key)
                {
                    case VirtualKey.Back:
                        if (caret > 0 && caret <= _realText.Length)
                        {
                            _realText = _realText.Remove(caret - 1, 1);
                            caret--;
                        }
                        break;

                    case VirtualKey.Delete:
                        if (caret < _realText.Length)
                        {
                            _realText = _realText.Remove(caret, 1);
                        }
                        break;

                    case VirtualKey.Enter:
                        OnProcessKey?.Invoke("Enter");
                        return;

                    default:
                        var inputChar = GetCharFromVirtualKey(e.Key);
                        if (!string.IsNullOrEmpty(inputChar))
                        {
                            if (string.IsNullOrEmpty(_realText)) _realText = inputChar;
                            else                                 _realText = _realText.Insert(caret-1, inputChar);
                            caret++;
                        }
                        break;
                }

                Value = _realText;
                Editor.Text = new string('●', _realText.Length);
                Editor.SelectionStart = caret;

                return;
            }

            // === ORIGINAL LOGIC BELOW ===

            Validate();

            OriginalValue = Value;
            bool valid = true;

            if (MaskType == MaskTypes.NumericOnly ||
                MaskType == MaskTypes.Numeric09 ||
                MaskType == MaskTypes.TwoDigitDecimal)
            {
                if (!IsNumericKey(e.Key))
                    e.Handled = true;
            }

            switch (e.Key)
            {
                case VirtualKey.Tab:
                    if (isChanged)
                    {
                        if (ChangeEventType.Equals(ChangeEventTypes.OnLostFocus))
                            if (RaiseOnChange(Value))
                                RaiseOnChanged(Value);
                    }
                    break;
            }
        }
        private string GetCharFromVirtualKey(VirtualKey key)
        {
            bool shift = false;
            try
            {
                //shift = (Window.Current?.CoreWindow?.GetKeyState(VirtualKey.Shift) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            }
            catch
            {
                shift = false;
            }

            if (key >= VirtualKey.A && key <= VirtualKey.Z)
            {
                string c = key.ToString();
                return shift ? c : c.ToLower();
            }

            if (key >= VirtualKey.Number0 && key <= VirtualKey.Number9)
            {
                char c = (char)('0' + (key - VirtualKey.Number0));

                if (shift)
                {
                    // Handle shifted number keys for symbols
                    switch (c)
                    {
                        case '1': return "!";
                        case '2': return "@";
                        case '3': return "#";
                        case '4': return "$";
                        case '5': return "%";
                        case '6': return "^";
                        case '7': return "&";
                        case '8': return "*";
                        case '9': return "(";
                        case '0': return ")";
                    }
                }

                return c.ToString();
            }

            if (key >= VirtualKey.NumberPad0 && key <= VirtualKey.NumberPad9)
            {
                return ((char)('0' + (key - VirtualKey.NumberPad0))).ToString();
            }

            switch (key)
            {
                case VirtualKey.Space: return " ";
                case VirtualKey.Decimal: return ".";
                case VirtualKey.Subtract: return "-";
                case VirtualKey.Add: return "+";
                case VirtualKey.Multiply: return "*";
                case VirtualKey.Divide: return "/";
                default: return string.Empty;
            }

        }
        private void _KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (MaskType == MaskTypes.Password)
            {
                Value = _realText;
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                    case VirtualKey.Tab:     if (ChangeEventType.Equals(ChangeEventTypes.OnEnterKey))
                                             OnProcessKey?.Invoke("Enter");                                  break;
                    case VirtualKey.Back:
                    case VirtualKey.Delete:  Editor.Text = new string('●', _realText.Length);
                                             Editor.SelectionStart = _realText.Length;                       break;

                    default:                 Editor.Text = new string('●', _realText.Length);
                                             Editor.SelectionStart = _realText.Length;                       break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                    case VirtualKey.Tab:
                        if (ChangeEventType.Equals(ChangeEventTypes.OnEnterKey))
                            OnProcessKey?.Invoke("Enter");
                        break;

                    default:
                        Value = Editor.Text;
                        if (ReplyTextAutofit) SetFontSize();

                        if (ChangeEventType.Equals(ChangeEventTypes.ByKeyPress))
                        {
                            bool valid = true;
                            RaiseOnChanged(Value);
                        }
                        break;
                }
            }
        }


        private void _LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() => Validate());

            //if (!OriginalValue.Equals(Editor.Text))
            //{
            //    isValid = true;
            //    if (RaiseOnChange(Value))
            //    {
            //        RaiseOnChanged(Value);

            //        Editor.Text = OriginalValue;
            //        isChanged = false;
            //        args.Handled = true;
            //    }
            //}
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MaskType == MaskTypes.Password)
            {
                if (Editor.Text.Length == 0) _realText = "";
            }
        }

        #endregion 

        #region Validation
        public void Validate()
        {

            if (Editor == null) return;
            if (MaskType == MaskTypes.Password)
            {
                Value = _realText;
                return;
            }

            Editor.Visibility = Visibility.Visible;
            string newText    = Editor.Text;
            var caret         = Editor.SelectionStart;

            switch (MaskType)
            {
                case MaskTypes.Numeric09:      newText = new string (newText.Where(char.IsDigit).ToArray());                                                  break;
                case MaskTypes.NumericOnly:    newText = FormatNumber(newText);                                                                               break;
                case MaskTypes.AlphaOnly:      newText = new string (newText.Where(char.IsLetter).ToArray());                                                 break;
                case MaskTypes.AlphaUpperCase: newText = newText.ToUpper();                                                                                   break;
                case MaskTypes.AlphaLowerCase: newText = newText.ToLower();                                                                                   break;
                case MaskTypes.AlphaTitleCase: newText = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newText);                       break;
                case MaskTypes.AlphaNameCase:  newText = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(newText.ToLower());                         break;
                case MaskTypes.AlphaNumeric:   newText = new string(newText.Where((char c) => c == '.' || c == '-' || char.IsLetterOrDigit(c)).ToArray());    break;
                case MaskTypes.PhoneWithArea:  newText = FormatPhoneNumber(newText, ref caret);                                                               break;
                case MaskTypes.IpAddress:      newText = FormatIPAddress(newText);                                                                            break;
                case MaskTypes.Date:           newText = FormatDate(newText, ref caret);                                                                      break;
                case MaskTypes.DateTime:       newText = FormatDateTime(newText, ref caret);                                                                  break;
                case MaskTypes.TwoDigitDecimal: if (!decimal.TryParse(newText, out _)) newText = string.Empty; else newText = FormatTwoDigitDecimal(newText); break;
            }

            Value       = newText;
            Editor.Text = newText;
            Editor.SelectionStart = caret < newText.Length ? caret : newText.Length;
        }


        #endregion 

        #region Internal Methods

        public bool ProcessKey(string Key, bool SendKey)
        {
            if (IsReadOnly)     return true;
            if (Key.Equals("")) return true;

            string lText = "";
            string rText = "";
            string kText = Key;

            bool Handled = false;
            bool ValidKey = true;
            bool IsNumeric = false;
            bool NumericOnly = MaskType.Equals(MaskTypes.NumericOnly) || MaskType.Equals(MaskTypes.Numeric09) || MaskType.Equals(MaskTypes.TwoDigitDecimal);

            switch (Key)
            {
                case "LastCtl": OnProcessKey?.Invoke(Key); break;

                case "OK":
                case "Enter":
                    Value = _realText;
                    OnProcessKey?.Invoke(Key);

                    kText = "";
                    Handled = true;
                    break;

                case "Delete":
                case "Shift":
                case "Back":
                    ValidKey = true;
                    kText = "";
                    break;
                case "Clear":
                    Editor.Text = "";
                    kText = "";
                    Value = "";
                    break;

                case "Space":
                    kText = " ";
                    break;

                case "LeftShift":
                    ValidKey = false;
                    kText = "";
                    break;

                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "F":
                case "G":
                case "H":
                case "I":
                case "J":
                case "K":
                case "L":
                case "M":
                case "N":
                case "O":
                case "P":
                case "Q":
                case "R":
                case "S":
                case "T":
                case "U":
                case "V":
                case "W":
                case "X":
                case "Y":
                case "Z":
                case "a":
                case "b":
                case "c":
                case "d":
                case "e":
                case "f":
                case "g":
                case "h":
                case "i":
                case "j":
                case "k":
                case "l":
                case "m":
                case "n":
                case "o":
                case "p":
                case "q":
                case "r":
                case "s":
                case "t":
                case "u":
                case "v":
                case "w":
                case "x":
                case "y":
                case "z":
                case "@":
                case "#":
                case "(":
                case ")":
                case "/":
                case "_":
                case "!":
                    if (NumericOnly) ValidKey = false;
                    break;

                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    switch (MaskType)
                    {
                        case MaskTypes.AlphaOnly:
                            ValidKey = false;
                            break;

                        case MaskTypes.TwoDigitDecimal:
                            if (Value.Contains("."))
                            {
                                string sval = Value.Substring(Value.IndexOf("."));
                                ValidKey = (sval.Length <= 2);
                            }
                            break;

                        default:
                            break;
                    }

                    IsNumeric = true;
                    break;

                case "~":
                case "$":
                case "%":
                case "^":
                case "&":
                case "*":
                case "+":
                case "|":
                case "`":
                case "{":
                case "=":
                case "}":
                case "[":
                case "]":
                case ":":
                case "'":
                case "<":
                case ">":
                case "?":
                    ValidKey = false;
                    IsNumeric = true;
                    break;

                case "-":
                    IsNumeric = true;
                    break;

                case ".":
                    switch (MaskType)
                    {
                        case MaskTypes.Numeric09:
                            IsNumeric = true;
                            ValidKey = false;
                            break;
                        case MaskTypes.NumericOnly:
                        case MaskTypes.TwoDigitDecimal:
                            IsNumeric = true;
                            if (Value.Contains(".")) ValidKey = false;
                            break;
                        default:
                            break;
                    }
                    break;
            }

            if (ValidKey)
            {
                if (SendKey)
                {
                    if (Editor.Text.Length > 0)
                    {
                        lText = Editor.Text.Substring(0, Editor.SelectionStart);
                        if ((Editor.SelectionStart + Editor.SelectionLength) < Editor.Text.Length)
                            rText = Editor.Text.Substring(Editor.SelectionStart + Editor.SelectionLength);

                        switch (Key)
                        {
                            case "Delete":
                                if (rText.Length > 0) rText = rText.Substring(1, lText.Length - 1);
                                break;
                            case "Back":
                                if (lText.Length > 0) lText = lText.Substring(0, lText.Length - 1);
                                break;
                            default:
                                break;
                        }
                    }
                    Editor.Text = lText + kText + rText;
                    Editor.SelectionStart = lText.Length + kText.Length;
                    Editor.SelectionLength = 0;
                    Value = Editor.Text;
                    if (ReplyTextAutofit) SetFontSize();
                }
                else
                {
                    switch (MaskType)
                    {
                        case MaskTypes.AlphaOnly:
                        case MaskTypes.AlphaLowerCase:
                        case MaskTypes.AlphaTitleCase:
                        case MaskTypes.AlphaUpperCase:
                        case MaskTypes.AlphaNumeric:
                        case MaskTypes.AlphaNameCase:
                            if (!IsNumeric & MaskType.Equals(MaskTypes.AlphaOnly))
                                //ResultChar = TestChar;
                                if (Editor.MaxLength > 0)
                                    if (Editor.Text.Length + 1 > Editor.MaxLength)
                                        ValidKey = false;


                            if (ValidKey & (MaskType.Equals(MaskTypes.AlphaLowerCase) | MaskType.Equals(MaskTypes.AlphaUpperCase)))
                            {
                                if (Editor.Text.Length > 0)
                                {
                                    lText = Editor.Text.Substring(0, Editor.SelectionStart);
                                    if ((Editor.SelectionStart + Editor.SelectionLength + 1) < Editor.Text.Length)
                                        rText = Editor.Text.Substring(Editor.SelectionStart + Editor.SelectionLength + 1);
                                }
                                //
                                if (!IsNumeric)
                                {
                                    switch (MaskType)
                                    {
                                        case MaskTypes.AlphaLowerCase:
                                            kText = kText.ToLower();
                                            Handled = true;
                                            break;
                                        case MaskTypes.AlphaUpperCase:
                                            kText = kText.ToUpper();
                                            Handled = true;
                                            break;
                                    }
                                }
                                if (Handled)
                                {
                                    Editor.Text = lText + kText + rText;
                                    Editor.SelectionStart = lText.Length + kText.Length;
                                    Editor.SelectionLength = 0;
                                    if (ReplyTextAutofit) SetFontSize();
                                }
                            }
                            break;
                    }
                }
            }

            if (!ValidKey) Handled = true;
            return Handled;
        }
        #endregion


        private bool IsNumericKey(VirtualKey key)
        {
            return (key >= VirtualKey.Number0 && key <= VirtualKey.Number9) ||
                   (key >= VirtualKey.NumberPad0 && key <= VirtualKey.NumberPad9) ||
                   key == VirtualKey.Decimal;
        }

        private string FormatPhoneNumber(string input, ref int currentSel)
        {
            if (input.Length > currentSel)
                input = input.Remove(currentSel, 1);

            if (input.Length > 17)
            {
                input = input.Substring(0, 17);
            }

            string strPhoneWithArea = "(+__) ___ ___ ___";

            int i = 0;

            for (i = 0; i < input.Length; i++)
                if (!(strPhoneWithArea[i] == '_' && char.IsDigit(input[i])) && !(strPhoneWithArea[i] == ' ' && input[i] == ' ') &&
                    !(strPhoneWithArea[i] == '(' && input[i] == '(') && !(strPhoneWithArea[i] == ')' && input[i] == ')') &&
                    !(strPhoneWithArea[i] == '+' && input[i] == '+'))
                    break;

            if (i < 17)
            {
                input = input.Substring(0, i) + strPhoneWithArea.Substring(i);
                currentSel = i;
            }

            if (input.Length > currentSel && input[currentSel] == '(')
                currentSel += 2;
            if (input.Length > currentSel && (input[currentSel] == ')' || input[currentSel] == ' '))
                currentSel++;

            return input;

        }

        private string FormatIPAddress(string input)
        {
            input = new string(input.Where((char c) => !char.IsLetter(c)).ToArray());
            if (input.Length > 0)
            {
                return string.Join(".", input.Split('.').Take(4).Select(octet => octet.Length == 0 || Int32.Parse(octet) < 255 ? octet : "254"));
            }
            else
            {
                return input;
            }
        }

        private string FormatTwoDigitDecimal(string input)
        {
            if (decimal.TryParse(input, out decimal result))
                return result.ToString("0.00");
            return input;
        }

        private string validateDate(string date)
        {
            string strDateOnly = "__/__/____";

            int[] days = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            string[] parts = date.Split('/');

            int level = 0;

            if (!parts[0].Contains('_'))
            {
                parts[0] = Math.Min(int.Parse(parts[0]), 12).ToString().PadLeft(2, '0');
                level++;

                if (!parts[1].Contains('_'))
                {
                    parts[1] = Math.Min(int.Parse(parts[1]), days[int.Parse(parts[0]) - 1]).ToString().PadLeft(2, '0');
                    level++;

                    if (!parts[2].Contains('_'))
                    {
                        parts[2] = Math.Max(int.Parse(parts[2]), 1000).ToString();
                    }
                }
            }

            if (level == 2)
                return string.Join('/', parts);
            else
                return string.Join("/", parts.Take(level + 1)) + strDateOnly.Substring((level + 1) * 2 + level);
        }

        private string validateTime(string time)
        {
            string strTimeOnly = "__:__";

            string[] parts = time.Split(':');

            int level = 0;

            if (!parts[0].Contains('_'))
            {
                parts[0] = Math.Min(int.Parse(parts[0]), 23).ToString().PadLeft(2, '0');
                level++;

                if (!parts[1].Contains('_'))
                {
                    parts[1] = Math.Min(int.Parse(parts[1]), 59).ToString().PadLeft(2, '0');
                }
            }

            if (level == 0)
                return parts[0] + strTimeOnly.Substring(2);
            else
                return string.Join(':', parts);
        }

        private string FormatDate(string input, ref int currentSel)
        {
            if (input.Length > currentSel)
                input = input.Remove(currentSel, 1);

            if (input.Length > 10)
            {
                input = input.Substring(0, 10);
            }

            string strDateOnly = "__/__/____";

            int i = 0;

            for (i = 0; i < input.Length; i++)
                if (!(strDateOnly[i] == '_' && char.IsDigit(input[i])) && !(strDateOnly[i] == '/' && input[i] == '/'))
                    break;

            if (i < 10)
            {
                input = input.Substring(0, i) + strDateOnly.Substring(i);
                currentSel = i;
            }

            if (input.Length > currentSel && input[currentSel] == '/')
                currentSel++;

            return validateDate(input);
        }

        private string FormatDateTime(string input, ref int currentSel)
        {
            if (input.Length > currentSel)
            {
                input = input.Remove(currentSel, 1);
            }

            if (input.Length > 16)
            {
                input = input.Substring(0, 16);
            }

            string strDateTime = "__/__/____ __:__";

            int i = 0;

            for (i = 0; i < input.Length; i++)
                if (!(strDateTime[i] == '_' && char.IsDigit(input[i])) && !(strDateTime[i] == '/' && input[i] == '/') &&
                     !(strDateTime[i] == ' ' && input[i] == ' ') && !(strDateTime[i] == ':' && input[i] == ':'))
                    break;

            if (i < 16)
            {
                input = input.Substring(0, i) + strDateTime.Substring(i);
                currentSel = i;
            }

            if (input.Length > currentSel && (input[currentSel] == '/' || input[currentSel] == ' ' || input[currentSel] == ':'))
                currentSel++;

            string[] parts = input.Split(' ');
            return validateDate(parts[0]) + ' ' + validateTime(parts[1]);
        }

        private string FormatNumber(string input)
        {
            bool hasDot = false;
            var result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '-')
                {
                    if (i == 0)
                        result.Append(c);
                    continue;
                }
                if (c == '.')
                {
                    if (!hasDot)
                    {
                        result.Append(c);
                        hasDot = true;
                    }
                    continue;
                }
                if (char.IsDigit(c))
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }


    }
}
