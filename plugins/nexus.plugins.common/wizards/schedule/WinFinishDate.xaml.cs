using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nexus.common;
using nexus.common.control;

namespace nexus.plugins.common
{
    public sealed partial class WinFinishDate : NxPanelBase
    {
        public WinFinishDate(NameValue Value)
        {
            this.InitializeComponent();

            PropBag.Add(Value);

            chkNoEnd.OnChanged += ChkNoEnd_OnChanged;
            dpFinish.OnChanged += DpFinish_OnChanged;
        }

        private void DpFinish_OnChanged(object? sender, ChangedEventArgs e)
        {
            Result.Prompt = e.Value.ToString();
        }

        public override bool Show()
        {
            if (!PropBag.ContainsKey("FinishesOn"))
                PropBag.Add(new NameValue("FinishesOn", "Never"));

            if (PropBag.ContainsKey("FinishesOn"))
            {
                string input = PropBag["FinishesOn"].Value;
                chkNoEnd.SetChecked (input.Equals("Never"));
                SetReadOnly         (input.Equals("Never"));

                if (input != "Never")
                {
                    if (helpers.IsDate(input))
                        dpFinish.Value = helpers.ToDate(input);
                }

                Result.Prompt = input;
            }
            return true;
        }

        public override bool Save()
        {
            if (!PropBag.ContainsKey("FinishesOn"))
                PropBag.Add(new NameValue("FinishesOn", "Never"));

            if (PropBag.ContainsKey("FinishesOn"))
            {
                if (chkNoEnd.IsSelected) PropBag["FinishesOn"].Value = "Never";
                else                     PropBag["FinishesOn"].Value = dpFinish.Value.ToString("MMM dd, yyyy");
            }
            return true;
        }

        private void ChkNoEnd_OnChanged(object? sender, ChangedEventArgs e)
        {
            SetReadOnly(chkNoEnd.IsSelected == true);
        }

        private void SetReadOnly(bool isReadOnly)
        {
            dpFinish.IsEnabled = !isReadOnly;
            dpFinish.Opacity   = isReadOnly ? 0.25 : 1;
        }

    }
}
