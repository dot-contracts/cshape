
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using nexus.common.control;

namespace nexus.common.control
{
    public sealed partial class NxDialog : NxPanelBase
    {
        public static readonly DependencyProperty TitleProperty      = DependencyProperty.Register(nameof(Title),      typeof(string),      typeof(NxDialog), new PropertyMetadata("Nx Dialog" , OnTitlePropertyChanged));
        public static readonly DependencyProperty DialogTypeProperty = DependencyProperty.Register(nameof(DialogType), typeof(DialogTypes), typeof(NxDialog), new PropertyMetadata(DialogTypes.wizard));
        public string      Title      { get => (string)     GetValue(TitleProperty);      set => SetValue(TitleProperty, value); }
        public DialogTypes DialogType { get => (DialogTypes)GetValue(DialogTypeProperty); set => SetValue(DialogTypeProperty, value); }

        public Dictionary<int, NxPanelBase> Panels = new();
        private int _position = 0;
        private NxPanelBase _currentPanel;

        public bool IsChanged { get; set; }
        public bool IsValid { get; set; }
        public bool IsCanceled { get; private set; } = true;
        public bool IsActive { get; set; }

        public event Action<bool>? OnValid;
        public event Action<bool>? OnAllowFinish;
        public event Action<bool>? OnRequired;
        public event Action<bool>? OnChanged;
        public event Action<System.Drawing.Size>? OnSizeChange;
        public event Func<string, NxPanelBase, bool>? OnPanelChange;
        public event Func<bool>? OnFinished;
        public event Action<string, string>? OnClose;

        private void OnNextClicked    (object sender, ClickedEventArgs e) => ShowNext();
        private void OnPreviousClicked(object sender, ClickedEventArgs e) => ShowPrevious();
        private void OnFinishClicked  (object sender, ClickedEventArgs e) => Finish();
        private void OnCancelClicked  (object sender, ClickedEventArgs e) => CloseDialog();
        private void OnOKClicked      (object sender, ClickedEventArgs e) => CloseDialog();
        private void OnCloseClicked   (object sender, ClickedEventArgs e) => CloseDialog();

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NxDialog)d).SetTitle();
        }
        private void SetTitle()
        {
            if (TitleText != null)
                TitleText.Text = Title;
        }

        public NxDialog()
        {
            this.InitializeComponent();

            TitleText.Text = Title;

            btNext.OnClicked     += OnNextClicked;
            btPrevious.OnClicked += OnPreviousClicked;
            btFinish.OnClicked   += OnFinishClicked;
            btCancel.OnClicked   += OnCancelClicked;
            btOK.OnClicked       += OnOKClicked;
            btClose.OnClicked    += OnCloseClicked;

            WizButtons.Visibility = (DialogType == DialogTypes.wizard)   ? Visibility.Visible : Visibility.Collapsed;
            DlgButtons.Visibility = (DialogType == DialogTypes.standard) ? Visibility.Visible : Visibility.Collapsed;
        }

        public void AddPanel(string panelName, NxPanelBase panel)
        {
            panel.Name = panelName;
            panel.Visibility = Visibility.Collapsed;
            Container.Children.Add(panel);
            Panels[Panels.Count] = panel;
        }

        private async void ShowPanel(NxPanelBase newPanel)
        {
            if (_currentPanel != null)
            {
                //var fadeOut = (Storyboard)this.Resources["FadeOutStoryboard"];
                //Storyboard.SetTarget(fadeOut, _currentPanel);
                //fadeOut.Begin();
                //await Task.Delay(300);
                _currentPanel.Visibility = Visibility.Collapsed;
            }

            _currentPanel = newPanel;
            //_currentPanel.Opacity = 0;
            _currentPanel.Visibility = Visibility.Visible;
            _currentPanel.Show();

            //var fadeIn = (Storyboard)this.Resources["FadeInStoryboard"];
            //Storyboard.SetTarget(fadeIn, _currentPanel);
            //fadeIn.Begin();

            SetButtons();
        }

        public void ShowFirst()
        {
            _currentPanel?.Save();
            bool cancel = OnPanelChange?.Invoke(_currentPanel.Name, _currentPanel) ?? false;
            if (cancel) return;

            var keys = Panels.Keys.ToList();
            var panel = Panels[keys[0]];
            _position = keys[0];
            ShowPanel(panel);
        }

        public void ShowNext()
        {
            _currentPanel?.Save();
            bool cancel = OnPanelChange?.Invoke(_currentPanel.Name, _currentPanel) ?? false;
            if (cancel) return;

            var keys = Panels.Keys.ToList();
            for (int i = _position + 1; i < keys.Count; i++)
            {
                var panel = Panels[keys[i]];
                _position = keys[i];
                ShowPanel(panel);
                break;
            }
        }

        public void ShowPrevious()
        {
            _currentPanel?.Save();
            bool cancel = OnPanelChange?.Invoke(_currentPanel.Name, _currentPanel) ?? false;
            if (cancel) return;

            var keys = Panels.Keys.ToList();
            for (int i = _position - 1; i >= 0; i--)
            {
                var panel = Panels[keys[i]];
                _position = keys[i];
                ShowPanel(panel);
                break;
            }
        }

        private void Finish()
        {
            foreach (var kvp in Panels)
            {
                if (!kvp.Value.Save())
                {
                    ShowPanel(kvp.Value);
                    return;
                }
            }

            IsCanceled = false;
            if (OnFinished?.Invoke() == false)
                return;

            CloseDialog();
        }

        private void SetButtons()
        {
            if (_currentPanel == null) return;
            bool canFinish = _currentPanel.IsValid && (_currentPanel.AllowFinish || _position == Panels.Count - 1);
            btFinish.IsReadOnly = !canFinish;
            btNext.IsReadOnly = _position >= Panels.Count - 1;
            btPrevious.IsReadOnly = _position == 0;
        }

        private void CloseDialog()
        {
            OnClose?.Invoke("OK", "");
            this.Visibility = Visibility.Collapsed;
        }
    }
}
