using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using nexus.common;
using nexus.common.control;

namespace nexus.plugins.common
{
    public sealed partial class WinDays : NxPanelBase
    {
        public WinDays(NameValue Value)
        {
            this.InitializeComponent();

            PropBag.Add(Value);

            chkMon.OnChanged += OnChanged;
            chkTue.OnChanged += OnChanged;
            chkWed.OnChanged += OnChanged;
            chkThu.OnChanged += OnChanged;
            chkFri.OnChanged += OnChanged;
            chkSat.OnChanged += OnChanged;
            chkSun.OnChanged += OnChanged;

            chkNone.OnChanged += ChkNone_OnChanged;

        }

        public override bool Show()
        {
            if (!PropBag.ContainsKey("Days"))
                PropBag.Add(new NameValue("Days", ""));

            if (PropBag.ContainsKey("Days"))
            {
                var days = PropBag["Days"].Value.Split(',').Select(d => d.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                chkNone.SetChecked(string.IsNullOrEmpty(PropBag["Days"].Value));
                SetReadOnly(string.IsNullOrEmpty(PropBag["Days"].Value));

                if (days.Count>0)
                {
                    chkSun.IsSelected = days.Contains("Sun");
                    chkMon.IsSelected = days.Contains("Mon");
                    chkTue.IsSelected = days.Contains("Tue");
                    chkWed.IsSelected = days.Contains("Wed");
                    chkThu.IsSelected = days.Contains("Thu");
                    chkFri.IsSelected = days.Contains("Fri");
                    chkSat.IsSelected = days.Contains("Sat");
                }

                Result.Prompt = PropBag["Days"].Value;

            }

            return true;
        }

        private void OnChanged(object? sender, ChangedEventArgs e) { Save(); }

        public override bool Save()
        {
            if (!PropBag.ContainsKey("Days"))
                PropBag.Add(new NameValue("Days", ""));

            var selected = new List<string>();
            if (chkSun.IsSelected == true) selected.Add("Sun");
            if (chkMon.IsSelected == true) selected.Add("Mon");
            if (chkTue.IsSelected == true) selected.Add("Tue");
            if (chkWed.IsSelected == true) selected.Add("Wed");
            if (chkThu.IsSelected == true) selected.Add("Thu");
            if (chkFri.IsSelected == true) selected.Add("Fri");
            if (chkSat.IsSelected == true) selected.Add("Sat");

            PropBag["Days"].Value = string.Join(",", selected);

            Result.Prompt = PropBag["Days"].Value;

            return true;
        }

        private void ChkNone_OnChanged(object? sender, ChangedEventArgs e)
        {
            SetReadOnly(chkNone.IsSelected);

            chkSun.IsSelected = false;
            chkMon.IsSelected = false;
            chkTue.IsSelected = false;
            chkWed.IsSelected = false;
            chkThu.IsSelected = false;
            chkFri.IsSelected = false;
            chkSat.IsSelected = false;

            PropBag["Days"].Value = "";
            Result.Prompt = PropBag["Days"].Value;
        }

        private void SetReadOnly(bool isReadOnly)
        {
            chkSun.Enabled = isReadOnly;
            chkMon.Enabled = isReadOnly;
            chkTue.Enabled = isReadOnly;
            chkWed.Enabled = isReadOnly;
            chkThu.Enabled = isReadOnly;
            chkFri.Enabled = isReadOnly;
            chkSat.Enabled = isReadOnly;
            Selection.Opacity = isReadOnly ? 0.25 : 1;
        }

    }
}
