using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using nexus.common;
using nexus.common.control;
namespace nexus.plugins.common
{
    public sealed partial class WinStartTime : NxPanelBase
    {

        public WinStartTime(NameValue Value)
        {
            PropBag.Add(Value);
            this.InitializeComponent();

            chkNone.OnChanged += ChkNone_OnChanged;
            tpStart.OnChanged += TpStart_OnChanged;
        }

        private void TpStart_OnChanged(object? sender, ChangedEventArgs e)
        {
            Result.Prompt = e.Value.ToString();
        }

        public override bool Show()
        {
            if (!PropBag.ContainsKey("StartsAt"))
                PropBag.Add(new NameValue("StartsAt", "AllDay"));

            if (PropBag.ContainsKey("StartsAt"))
            {
                string input = PropBag["StartsAt"].Value;
                chkNone.SetChecked (input.Equals("AllDay"));
                SetReadOnly        (input.Equals("AllDay"));

                if (input != "AllDay") tpStart.Value = input;

                Result.Prompt = input;
            }
            return true;
        }

        public override bool Save()
        {
            if (!PropBag.ContainsKey("StartsAt"))
                PropBag.Add(new NameValue("StartsAt", "AllDay"));

            if (PropBag.ContainsKey("StartsAt"))
            {
                if  (chkNone.IsSelected) PropBag["StartsAt"].Value = "AllDay";
                else                     PropBag["StartsAt"].Value = tpStart.Value;
            }
            return true;
        }


        private void ChkNone_OnChanged(object? sender, ChangedEventArgs e)
        {
            SetReadOnly(chkNone.IsSelected == true);
        }

        private void SetReadOnly(bool isReadOnly)
        {
            tpStart.IsEnabled = !isReadOnly;
            tpStart.Opacity   = isReadOnly ? 0.25 : 1;
        }

    }
}
