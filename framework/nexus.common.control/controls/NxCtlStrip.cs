
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Data;

using System;
using System.Collections.Generic;
using System.Linq;

namespace nexus.common.control
{
    public partial class NxCtlStrip : NxControlBase
    {
        public enum StripTypes      { Tab, Check, Option }
        public enum PromptPositions { Top, Left, Below }
        
        public static readonly DependencyProperty PromptProperty         = DependencyProperty.Register(nameof(Prompt),          typeof(string),          typeof(NxCtlStrip), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty PromptPositionProperty = DependencyProperty.Register(nameof(PromptPosition),  typeof(PromptPositions), typeof(NxCtlStrip), new PropertyMetadata(PromptPositions.Top));
        public static readonly DependencyProperty PromptWidthProperty    = DependencyProperty.Register(nameof(PromptWidth),     typeof(double),          typeof(NxCtlStrip), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty StripTypeProperty      = DependencyProperty.Register(nameof(StripTypes),      typeof(string),          typeof(NxCtlStrip), new PropertyMetadata(StripTypes.Tab, OnListChanged));
        public static readonly DependencyProperty ListProperty           = DependencyProperty.Register(nameof(List),            typeof(string),          typeof(NxCtlStrip), new PropertyMetadata(string.Empty, OnListChanged));
        public static readonly DependencyProperty SelectedItemProperty   = DependencyProperty.Register(nameof(SelectedItem),    typeof(object),          typeof(NxCtlStrip), new PropertyMetadata(null, OnSelectedItemChanged));
        public static readonly DependencyProperty CtlWidthProperty       = DependencyProperty.Register(nameof(CtlWidth),        typeof(double),          typeof(NxCtlStrip), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty CtlHeightProperty      = DependencyProperty.Register(nameof(CtlHeight),       typeof(double),          typeof(NxCtlStrip), new PropertyMetadata(double.NaN));

        public string          Prompt         { get => (string)GetValue(PromptProperty);                  set => SetValue(PromptProperty, value); }
        public PromptPositions PromptPosition { get => (PromptPositions)GetValue(PromptPositionProperty); set => SetValue(PromptPositionProperty, value); }
        public double          PromptWidth    { get => (double)GetValue(PromptWidthProperty);             set => SetValue(PromptWidthProperty, value); }
        public string          StripType      { get => (string)GetValue(StripTypeProperty);               set => SetValue(StripTypeProperty, value); }
        public string          List           { get => (string)GetValue(ListProperty);                    set => SetValue(ListProperty, value); }
        public object          SelectedItem   { get => GetValue(SelectedItemProperty);                    set => SetValue(SelectedItemProperty, value); }
        public double          CtlWidth       { get => (double)GetValue(CtlWidthProperty);                set => SetValue(CtlWidthProperty, value); }
        public double          CtlHeight      { get => (double)GetValue(CtlHeightProperty);               set => SetValue(CtlHeightProperty, value); }

        private static void OnStripTypeChanged   (DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is NxCtlStrip strip && e.NewValue is StripTypes type) strip.BuildTabs(strip.List); }
        private static void OnListChanged        (DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is NxCtlStrip strip && e.NewValue is string     list) strip.BuildTabs(list); }
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is NxCtlStrip strip) strip.UpdateSelection(); }

        private StackPanel? _tabHost;

        public NxCtlStrip()
        {
            this.DefaultStyleKey = typeof(NxCtlStrip);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _tabHost = GetTemplateChild("TabHost") as StackPanel;
            BuildTabs(List);
        }

        private void BuildTabs(string list)
        {
            if (_tabHost == null)
                return;

            _tabHost.Children.Clear();

            var items = list.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

            // Step 1: Get the longest item
            var longest = items.OrderByDescending(s => s.Length).FirstOrDefault() ?? string.Empty;

            // Step 2: Estimate width � adjust per your font + style
            int charWidth         = 8;   // average width per character for Segoe UI 14px
            int horizontalPadding = 10; // extra spacing inside the button

            int uniformWidth = (longest.Length * charWidth) + horizontalPadding;
            int uniformHeight = 32; // reasonable default

            foreach (var item in items)
            {
                switch (StripType)
                {   case nameof(StripTypes.Tab):                        // Create a button
                        var btn = new NxButton
                        {
                            Prompt = item,
                            ButtonType = ButtonTypes.Tab,
                            BorderType = BorderTypes.Edit,
                            Margin     = new Thickness(1, 0, 1, 0),
                            GroupName  = "NxTabGroup",
                            Width      = double.IsNaN(CtlWidth)  ? uniformWidth  : CtlWidth,
                            Height     = double.IsNaN(CtlHeight) ? uniformHeight : CtlHeight    
                        };

                        btn.SetSelected(Equals(item, SelectedItem));
                        btn.OnClicked += Btn_OnClicked;

                        _tabHost.Children.Add(btn);

                        break;

                    case nameof(StripTypes.Check):                        // Create a check button
                        var chk = new NxCheck
                        {
                            Prompt     = item,
                            Margin     = new Thickness(4, 0, 4, 0),
                            GroupName  = "NxTabGroup",
                            Width      = double.IsNaN(CtlWidth) ? uniformWidth : CtlWidth,
                            Height     = double.IsNaN(CtlHeight) ? uniformHeight : CtlHeight    
                        };
                        chk.OnClicked += Btn_OnClicked;

                        if (Equals(item, SelectedItem)) chk.IsSelected = true;
                        _tabHost.Children.Add(chk);

                        break;

                    case nameof(StripTypes.Option):                        // Create an option button

                        var opt = new NxRadio
                        {
                            Prompt = item,
                            Margin = new Thickness(4, 0, 4, 0),
                            GroupName  = "NxTabGroup",
                            Width      = double.IsNaN(CtlWidth)  ? uniformWidth : CtlWidth,
                            Height     = double.IsNaN(CtlHeight) ? uniformHeight : CtlHeight    
                        };
                        opt.OnClicked += Btn_OnClicked;

                        if (Equals(item, SelectedItem)) opt.IsSelected = true;
                        _tabHost.Children.Add(opt);

                        break;
                }
            }
        }

        private void Btn_OnClicked(object? sender, ClickedEventArgs e)
        {
            if (sender is NxButton btn && btn.Prompt is string content)
            {
                SelectedItem  = content;
                if (RaiseOnChange(content)) RaiseOnChanged(content);
                RaiseOnClicked(true); 
                UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            if (_tabHost == null) return;

            foreach (var child in _tabHost.Children)
            {
                switch (StripType)
                {
                    case nameof(StripTypes.Tab):    if (child is NxButton btn) btn.SetSelected (Equals(btn.Prompt, SelectedItem)); break;
                    case nameof(StripTypes.Check):  if (child is NxCheck  chk) chk.IsSelected = Equals(chk.Prompt, SelectedItem); break;
                    case nameof(StripTypes.Option): if (child is NxRadio  rad) rad.IsSelected = Equals(rad.Prompt, SelectedItem); break;
                }
            }
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string str)
            {
                return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}



