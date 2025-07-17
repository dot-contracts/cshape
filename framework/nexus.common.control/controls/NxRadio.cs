using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.System;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using nexus.common;
using nexus.common.control.Themes;
//using CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.BsonHelpers;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace nexus.common.control
{
    public sealed partial class NxRadio : NxControlBase
    {

        #region Prompt Dependency Property
        public static readonly      DependencyProperty PromptProperty                    = DependencyProperty.Register("Prompt",                    typeof(string),                typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptWidthProperty               = DependencyProperty.Register("PromptWidth",               typeof(double),                typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptFontFamilyProperty          = DependencyProperty.Register("PromptFontFamily",          typeof(FontFamily),            typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptFontSizeProperty            = DependencyProperty.Register("PromptFontSize",            typeof(double),                typeof(NxRadio), new PropertyMetadata(false));
        //public static readonly      DependencyProperty PromptFontWeightProperty          = DependencyProperty.Register("PromptFontWeight",          typeof(FontWeight),            typeof(NxRadio));
        //public static readonly      DependencyProperty PromptForeColorProperty           = DependencyProperty.Register("PromptForeColor",           typeof(Brush),                 typeof(NxRadio), new PropertyMetadata(false));
        //public static readonly      DependencyProperty PromptBackColorProperty           = DependencyProperty.Register("PromptBackColor",           typeof(Brush),                 typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptMarginProperty              = DependencyProperty.Register("PromptMargin",              typeof(Thickness),             typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty MaxPromptWidthProperty            = DependencyProperty.Register("MaxPromptWidth",            typeof(double),                typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptHorizontalAlignmentProperty = DependencyProperty.Register("PromptHorizontalAlignment", typeof(HorizontalAlignment),   typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptVerticalAlignmentProperty   = DependencyProperty.Register("PromptVerticalAlignment",   typeof(VerticalAlignment),     typeof(NxRadio), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptTextWrappingProperty        = DependencyProperty.Register("PromptTextWrapping",        typeof(TextWrapping),          typeof(NxRadio), new PropertyMetadata(false));

        public     string                Prompt                     { get { return (string)                 GetValue(PromptProperty); }                    set { SetValue(PromptProperty,                    value); } }
        public     double                PromptWidth                { get { return (double)                 GetValue(PromptWidthProperty); }               set { SetValue(PromptWidthProperty,               value); } }
        public     FontFamily            PromptFontFamily           { get { return (FontFamily)             GetValue(PromptFontFamilyProperty); }          set { SetValue(PromptFontFamilyProperty,          value); } }
        public     double                PromptFontSize             { get { return (double)                 GetValue(PromptFontSizeProperty); }            set { SetValue(PromptFontSizeProperty,            value); } }
        //public     FontWeight            PromptFontWeight           { get { return (FontWeight)             GetValue(PromptFontWeightProperty); }          set { SetValue(PromptFontWeightProperty,          value); } }
        //public     Brush                 PromptForeColor            { get { return (Brush)                  GetValue(PromptForeColorProperty); }           set { SetValue(PromptForeColorProperty,           value); } }
        //public     Brush                 PromptBackColor            { get { return (Brush)                  GetValue(PromptBackColorProperty); }           set { SetValue(PromptBackColorProperty,           value); } }
        public     Thickness             PromptMargin               { get { return (Thickness)              GetValue(PromptMarginProperty); }              set { SetValue(PromptMarginProperty,              value); } }
        public     double                MaxPromptWidth             { get { return (double)                 GetValue(MaxPromptWidthProperty); }            set { SetValue(MaxPromptWidthProperty,            value); } }
        public     HorizontalAlignment   PromptHorizontalAlignment  { get { return (HorizontalAlignment)    GetValue(PromptHorizontalAlignmentProperty); } set { SetValue(PromptHorizontalAlignmentProperty, value); } }
        public     VerticalAlignment     PromptVerticalAlignment    { get { return (VerticalAlignment)      GetValue(PromptVerticalAlignmentProperty); }   set { SetValue(PromptVerticalAlignmentProperty,   value); } }
        public     TextWrapping          PromptTextWrapping         { get { return (TextWrapping)           GetValue(PromptTextWrappingProperty); }        set { SetValue(PromptTextWrappingProperty,        value); } }

        public Brush PromptBackColor { get => PromptBack; set => PromptBack = value; }
        public Brush PromptForeColor { get => PromptFore; set => PromptFore = value; }

        #endregion


        #region Dependency
        public static readonly DependencyProperty IsReadonlyProperty            = DependencyProperty.Register("IsReadonly", typeof(bool), typeof(NxRadio), new PropertyMetadata(false));
        public static readonly DependencyProperty ValueProperty     =   DependencyProperty.Register("Value", typeof(bool), typeof(NxRadio), new PropertyMetadata(false));

        public bool     IsReadonly    { get { return (bool)  GetValue(IsReadonlyProperty); }   set { SetValue(IsReadonlyProperty, value); } }
        public bool     Value         { get { return (bool)  GetValue(ValueProperty); }        set { SetValue(ValueProperty, value); } }

        #endregion

        #region Constructor

        public NxRadio()
        {
            //if (Tag == null) Tag = string.Empty;
            this.DefaultStyleKey = typeof(NxCheck);
            this.IsTabStop = true;
            this.PointerPressed += OnClick;
            base.OnThemeChange += _OnThemeChange;
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetVisualState();
        }

        private void _OnThemeChange(NxTheme Theme)
        {
            if (Theme != null)
            {
                this.PromptBackColor = Theme.PromptBack.Brush;
                this.PromptForeColor = Theme.PromptFore.Brush;
            }
        }

        private void SetVisualState()
        {
            VisualStateManager.GoToState(this, Value ? "Checked" : "Unchecked", true);
        }

        #endregion

        #region Override

        private void OnClick(object sender, PointerRoutedEventArgs e)
        {
            if (!IsReadonly)
            {
                bool valid = true;
                //OnChanged?.Invoke(string.IsNullOrEmpty(Tag.ToString()) ? this.Name : Tag.ToString(), Value, ref valid);
                // base.OnChecked(e);
                ToggleChecked();
            }
        }

        private void ToggleChecked()
        {
            Value = !Value;
            SetVisualState();
        }

        #endregion

    }
}
