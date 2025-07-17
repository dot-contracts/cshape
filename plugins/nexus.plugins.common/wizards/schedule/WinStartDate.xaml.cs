using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using nexus.common;
using nexus.common.control;

namespace nexus.plugins.common
{
    public sealed partial class WinStartDate : NxPanelBase
    {
        public WinStartDate(NameValue Value)
        {
            this.InitializeComponent();
            
            PropBag.Add(Value);

            chkNone.OnChanged += chkNone_OnChanged;
            dpStart.OnChanged += DpStart_OnChanged;
        }

        private void DpStart_OnChanged(object? sender, ChangedEventArgs e)
        {
            Result.Prompt = e.Value.ToString();
        }

        public override bool Show()
        {
            if (!PropBag.ContainsKey("StartsOn"))
                PropBag.Add(new NameValue("StartsOn", "AllDay"));

            if (PropBag.ContainsKey("StartsOn"))
            {
                string input = PropBag["StartsOn"].Value;
                chkNone.SetChecked(input.Equals("AllDay"));
                SetReadonly       (input.Equals("AllDay"));

                if (input != "AllDay")
                {
                    if (helpers.IsDate(input))
                        dpStart.Value = helpers.ToDate(input);
                }

                Result.Prompt = input;
            }
            return true;
        }

        public override bool Save()
        {
            if (!PropBag.ContainsKey("StartsOn"))
                PropBag.Add(new NameValue("StartsOn", "AllDay"));

            if (PropBag.ContainsKey("StartsOn"))
            {
                if (chkNone.IsSelected) PropBag["StartsOn"].Value = "AllDay";
                else                    PropBag["StartsOn"].Value = dpStart.Value.ToString("MMM dd, yyyy");
            }
            return true;
        }

        private void chkNone_OnChanged(object? sender, ChangedEventArgs e)
        {
            SetReadonly(chkNone.IsSelected == true);
        }

        private void SetReadonly(bool isReadOnly)
        {
            dpStart.IsEnabled = !isReadOnly;
            dpStart.Opacity   = isReadOnly ? 0.25 : 1;
        }

    }
}
