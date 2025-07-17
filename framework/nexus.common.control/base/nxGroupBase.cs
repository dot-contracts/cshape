using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;

using nexus.common.control.Themes;

namespace nexus.common.control
{
    public partial class NxGroupBase : NxPanelBase, IDisposable
    {
        public string                                GroupName     { get; set; } = string.Empty;

        public Dictionary<string, IControl>          ControlList   { get; private set; } = new();
        private  List<string>                        ControlKeys   => ControlList.Keys.ToList();

        public IControl?                             CurrentCtl    { get; private set; }
        public int                                   CurrentIndex  { get; private set; } = 0;
        public string                                KeyFocus      { get; private set; } = string.Empty;

        public double                                ValueDbl      => CurrentCtl?.ValueDbl ?? 0;
        public double                                TagValue      => double.TryParse((string?)CurrentCtl?.Tag, out var val) ? val : 0;
        public bool                                  IsReadonly    { get; private set; }

        #region Events

        public event Action<string>?                 OnFocusSet;
        //public event Action<string>?                 OnProcessKey;
        public event Action<string>?                 OnPreviewKey;
        public event EventHandler<ChangeEventArgs>?  OnChange;
        public event EventHandler<(string Tag, DateTime Value, bool Valid)>? OnDateChanged;
        public event EventHandler<(string Tag, bool Value, bool Valid)>?     OnCheckChanged;

        private Action<string>? _onProcessKey;

        public event Action<string> OnProcessKey
        {
            add    => _onProcessKey += value;
            remove => _onProcessKey -= value;
        }

        private void RaiseProcessKey(string key)
        {
            _onProcessKey?.Invoke(key);
        }

        #endregion

        public NxGroupBase() { }

        public void AddControl(object addobj)
        {
            if (addobj is not FrameworkElement fe || string.IsNullOrEmpty(fe.Name)) return;
            if (addobj is not IControl ctl) return;
            if (fe.Visibility == Visibility.Collapsed) return;

            ControlList[fe.Name] = ctl;

            ctl.PreviewMouseDown += PrevMouseDown;
            ctl.OnChange         += _OnChanged;
            ctl.OnProcessKey     += _OnProcessKey;

            int hh = ctl.GetHashCode();

            //ctl.OnProcessKey     += (_, key) => OnProcessKey?.Invoke(key);
            ctl.OnProcessKeyDown += (_, e)   => OnPreviewKey?.Invoke(e.Key.ToString());
        }

        private void _OnProcessKey(object? sender, string e)
        {
            RaiseProcessKey(e);
            //OnProcessKey?.Invoke(e);
        }

        public void Dispose()
        {
            foreach (var ctl in ControlList.Values)
            {
                ctl.PreviewMouseDown -= PrevMouseDown;
                ctl.OnChange         -= _OnChanged;
            }

            ControlList.Clear();
        }

        public void SetReadonly(bool isreadonly)
        {
            IsReadonly = isreadonly;

            foreach (var ctl in ControlList.Values)
                ctl.IsReadOnly = isreadonly;
        }

        public void SetFocus(string name)
        {
            if (!ControlList.TryGetValue(name, out var ctl)) return;

            KeyFocus     = name;
            CurrentCtl   = ctl;
            CurrentIndex = ControlKeys.IndexOf(name);

            ctl.Opacity  = 1;
            ctl.Focus();
            ctl.SetFocus();

            OnFocusSet?.Invoke(KeyFocus);
        }

        public string GetValue() =>
            CurrentCtl?.Value ?? string.Empty;

        private void ProcKey(string key, bool sendKey) =>
            CurrentCtl?.ProcessKey(key, sendKey);

        private void PrevMouseDown(object sender, object e)
        {
            if (sender is FrameworkElement fe && ControlList.ContainsKey(fe.Name))
                SetFocus(fe.Name);
        }

        public string GetNext()
        {
            for (int i = CurrentIndex + 1; i < ControlKeys.Count; i++)
            {
                var ctl = ControlList[ControlKeys[i]];
                if (ctl.Visibility == Visibility.Visible && !ctl.IsReadOnly)
                    return ControlKeys[i];
            }

            return string.Empty;
        }

        public void MoveNext()
        {
            while (++CurrentIndex < ControlKeys.Count)
            {
                var ctl = ControlList[ControlKeys[CurrentIndex]];
                if (ctl.Visibility == Visibility.Visible && !ctl.IsReadOnly)
                {
                    SetFocus(ControlKeys[CurrentIndex]);
                    return;
                }
            }

            CurrentIndex = ControlKeys.Count - 1;
            RaiseProcessKey("LastCtl");
            //OnProcessKey?.Invoke("LastCtl");
        }

        public void MovePrevious()
        {
            if (!string.IsNullOrEmpty(GetValue()))
            {
                ProcKey("Back", true);
                return;
            }

            while (--CurrentIndex >= 0)
            {
                var ctl = ControlList[ControlKeys[CurrentIndex]];
                if (ctl.Visibility == Visibility.Visible && !ctl.IsReadOnly)
                {
                    SetFocus(ControlKeys[CurrentIndex]);
                    return;
                }
            }

            CurrentIndex = 0;
        }

        private void _OnChanged(object? sender, ChangeEventArgs e)
        {
            OnChange?.Invoke(this, e);

            if (DateTime.TryParse(e.Value, out var dt))
                OnDateChanged?.Invoke(this, (e.Tag, dt, e.Valid));
            else if (bool.TryParse(e.Value, out var b))
                OnCheckChanged?.Invoke(this, (e.Tag, b, e.Valid));
        }

        public Dictionary<string, string> GetValues()
        {
            return ControlList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value);
        }

        public bool ValidateAll(bool autoFocusFirstInvalid = true)
        {
            bool isValid = true;
            bool focusSet = false;

            foreach (var ctl in ControlList.Values)
            {
                if (ctl.Visibility != Visibility.Visible)
                    continue;

                if (!ctl.Validate())
                {
                    isValid = false;

                    if (autoFocusFirstInvalid && !focusSet)
                    {
                        ctl.Focus();
                        ctl.SetFocus();
                        focusSet = true;
                    }
                }
            }

            return isValid;
        }

        public void ResetAll()
        {
            foreach (var ctl in ControlList.Values)
            {
                ctl.Value        = string.Empty;
                ctl.ValueDbl     = 0;
                ctl.IsValidated  = false;
            }

            CurrentIndex = 0;
            CurrentCtl   = null;
            KeyFocus     = string.Empty;
        }

        public bool TryGetControl(string key, out IControl? ctl) =>
            ControlList.TryGetValue(key, out ctl);

        public void ApplyThemeToAll(NxTheme? theme = null)
        {
            foreach (var ctl in ControlList.Values)
            {
                if (ctl is NxControlBase nxBase)
                    nxBase.Theme = theme ?? NxThemeManager.Current;
            }
        }

        public void SetVisibilityAll(VisiStates state)
        {
            foreach (var ctl in ControlList.Values)
            {
                if (ctl is NxControlBase nxBase)
                    nxBase.SetVisibility(state);
            }
        }

        public void SetAnimation(AnimationTypes animType)
        {
            foreach (var ctl in ControlList.Values)
            {
                if (ctl is NxControlBase nxBase)
                    nxBase.AnimationType = animType;
            }
        }

        public void PrintState()
        {
            Console.WriteLine($"[Group: {GroupName}] Current Focus = {KeyFocus} ({CurrentIndex}/{ControlList.Count})");
            foreach (var kvp in ControlList)
                Console.WriteLine($"  - {kvp.Key}: {kvp.Value.Value}");
        }
    }
}
