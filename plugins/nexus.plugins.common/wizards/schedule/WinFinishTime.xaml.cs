using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using nexus.common;
using nexus.common.control;

namespace nexus.plugins.common
{
    public sealed partial class WinFinishTime : NxPanelBase
    {
        public WinFinishTime(NameValue Value)
        {
            this.InitializeComponent();

            PropBag.Add(Value);

            chkNoEnd.OnChanged += ChkNoEnd_OnChanged;
            tpFinish.OnChanged += TpFinish_OnChanged;
        }

        private void TpFinish_OnChanged(object? sender, ChangedEventArgs e)
        {
            Result.Prompt = e.Value.ToString();
        }

        public override bool Show()
        {
            if (!PropBag.ContainsKey("FinishesAt"))
                PropBag.Add(new NameValue("FinishesAt", "Never"));

            if (PropBag.ContainsKey("FinishesAt"))
            {
                string input = PropBag["FinishesAt"].Value;
                chkNoEnd.SetChecked (input.Equals("Never"));
                SetReadOnly         (input.Equals("Never"));

                if (input != "Never") tpFinish.Value = input;

                Result.Prompt = input;
            }
            return true;
        }

        public override bool Save()
        {
            if (!PropBag.ContainsKey("FinishesAt"))
                PropBag.Add(new NameValue("FinishesAt", "Never"));

            if (PropBag.ContainsKey("FinishesAt"))
            {
                if (chkNoEnd.IsSelected) PropBag["FinishesAt"].Value = "Never";
                else                     PropBag["FinishesAt"].Value = tpFinish.Value;
            }
            return true;
        }

        private void ChkNoEnd_OnChanged(object? sender, ChangedEventArgs e)
        {
            SetReadOnly(chkNoEnd.IsSelected == true);
        }

        private void SetReadOnly(bool isReadOnly)
        {
            tpFinish.IsEnabled = !isReadOnly;
            tpFinish.Opacity = isReadOnly ? 0.25 : 1;
        }

    }
}
