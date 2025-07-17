using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using nexus.common.control.Themes;
using nexus.common.control;

namespace nexus.common.control
{
    public partial class NxMenuGroup : NxPanelBase
    {
        private readonly List<NxMenuItem> _menuItems = new();

        #region Property
        private NxMenuCaption _caption;
        private StackPanel    _stacker;
        private Border        _border;
        #endregion

        public string CaptionText;
        //{
        //    get => _caption?.Prompt ?? string.Empty;
        //    set { if (_caption != null) _caption.Prompt = value; }
        //}

        public event MenuGroupStatusEventHandler MenuGroupStatusEvent;
        public event OnMenuItemClickedEventHandler OnMenuItemClicked;

        public delegate void MenuGroupStatusEventHandler(object sender);
        public delegate void OnMenuItemClickedEventHandler(NxMenuItem item, string prompt, string assembly, string entry, string snapIn, int moduleId, int functionId);

        public NxMenuGroup(string caption)
        {
            DefaultStyleKey = typeof(NxMenuGroup);
            CaptionText     = caption;

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }
        private void ApplyThemeDefaults()
        {
            if (_border != null)
            {
                _border.Background = NxThemeManager.Current.GetThemeBrush("NxMenuCaptionBack", Colors.LightGray);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _caption = GetTemplateChild("Caption") as NxMenuCaption;
            _stacker = GetTemplateChild("Stacker") as StackPanel;
            _border  = GetTemplateChild("Border")  as Border;

            if (_caption != null)
            {
                _caption.Prompt                    = CaptionText;
                _caption.PromptHorizontalAlignment = HorizontalAlignment.Center;
                _caption.PromptVerticalAlignment   = VerticalAlignment.Center;
                _caption.PointerPressed           += Caption_PointerPressed;
            }
        }

        public void AddMenuItem(string prompt, string assemblyData, bool showImmediately)
        {
            if (string.IsNullOrWhiteSpace(assemblyData)) return;

            var mi = new NxMenuItem();
            int moduleId  =0;
            int functionId=0;

            string[] parts = assemblyData.Split(';');
            if (parts.Length == 5)
            {
                mi = new NxMenuItem
                {
                    Assembly = parts[0],
                    Entry = parts[1],
                    SnapIn = parts[2],
                    Prompt = prompt
                    //,
                    //HorizontalAlignment = HorizontalAlignment.Right,
                    //HorizontalAlignment = HorizontalAlignment.Stretch
                };

                _ = moduleId   = helpers.ToInt(parts[3]);
                _ = functionId = helpers.ToInt(parts[4]);
            }
            else
            {
                mi.Prompt = prompt;
            }
            mi.ModuleID = moduleId;
            mi.FunctionID = functionId;
            mi.OnMenuItemClicked += HandleMenuItemClicked;

            //mi.Height = 30; 

            if (showImmediately && _stacker != null)
            {
                _stacker.Children.Add(mi);
            }

            _menuItems.Add(mi);
        }

        public void RemoveMenuItem()
        {
            _stacker?.Children.Clear();
        }

        private void Caption_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_stacker == null) return;

            if (_stacker.Children.Count > 0)
            {
                RemoveMenuItem();
            }
            else
            {
                foreach (var item in _menuItems)
                {
                    _stacker.Children.Add(item);
                }
            }

            MenuGroupStatusEvent?.Invoke(this);
        }

        private void HandleMenuItemClicked(NxMenuItem item, string prompt, string assembly, string entry, string snapIn, int moduleId, int functionId)
        {
            OnMenuItemClicked?.Invoke(item, prompt, assembly, entry, snapIn, moduleId, functionId);
        }
    }
}
