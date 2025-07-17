using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using nexus.common;
using nexus.common.control;
using System.Xml.Linq;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace nexus.plugins.common
{
    public sealed partial class WinHours : NxPanelBase
    {
        public WinHours(NameValue Value)
        {
            this.InitializeComponent();

            PropBag.Add(Value);

            for (int i = 1; i <= 24; i++) HookCheck("chk" + i.ToString("00"));

            chkNone.OnChanged += ChkNone_OnChanged;
        }

        private void HookCheck(string name)
        {
            if (GetTemplateChild(name) is NxCheck button)
            {
                button.OnClicked += ButtonClicked;
            }
        }

        public override bool Show()
        {
            if (!PropBag.ContainsKey("Hours")) PropBag.Add(new NameValue("Hours", ""));

            if (PropBag.ContainsKey("Hours"))
            {
                var hours = PropBag["Hours"].Value.Split(',').Select(d => d.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                chkNone.SetChecked(string.IsNullOrEmpty(PropBag["Hours"].Value));
                SetReadOnly(string.IsNullOrEmpty(PropBag["Hours"].Value));

                Result.Prompt = PropBag["Hours"].Value;

                if ((hours.Count>0))
                {
                    chk01.IsSelected = hours.Contains("1");
                    chk02.IsSelected = hours.Contains("2");
                    chk03.IsSelected = hours.Contains("3");
                    chk04.IsSelected = hours.Contains("4");
                    chk05.IsSelected = hours.Contains("5");
                    chk06.IsSelected = hours.Contains("6");
                    chk07.IsSelected = hours.Contains("7");
                    chk08.IsSelected = hours.Contains("8");
                    chk09.IsSelected = hours.Contains("9");
                    chk10.IsSelected = hours.Contains("10");
                    chk11.IsSelected = hours.Contains("11");
                    chk12.IsSelected = hours.Contains("12");
                    chk12.IsSelected = hours.Contains("13");
                    chk14.IsSelected = hours.Contains("14");
                    chk15.IsSelected = hours.Contains("15");
                    chk16.IsSelected = hours.Contains("16");
                    chk17.IsSelected = hours.Contains("17");
                    chk18.IsSelected = hours.Contains("18");
                    chk19.IsSelected = hours.Contains("19");
                    chk20.IsSelected = hours.Contains("20");
                    chk21.IsSelected = hours.Contains("21");
                    chk22.IsSelected = hours.Contains("22");
                    chk23.IsSelected = hours.Contains("23");
                    chk24.IsSelected = hours.Contains("24");
                }
            }

            return true;
        }

        private void ButtonClicked(object? sender, ClickedEventArgs e) { Save(); }
        public override bool Save()
        {
            if (!PropBag.ContainsKey("Hours"))
                PropBag.Add(new NameValue("Hours", ""));

            var selected = new List<string>();
            if (chk01.IsSelected == true) selected.Add("1");
            if (chk02.IsSelected == true) selected.Add("2");
            if (chk03.IsSelected == true) selected.Add("3");
            if (chk04.IsSelected == true) selected.Add("4");
            if (chk05.IsSelected == true) selected.Add("5");
            if (chk06.IsSelected == true) selected.Add("6");
            if (chk07.IsSelected == true) selected.Add("7");
            if (chk08.IsSelected == true) selected.Add("8");
            if (chk09.IsSelected == true) selected.Add("9");
            if (chk10.IsSelected == true) selected.Add("10");
            if (chk11.IsSelected == true) selected.Add("11");
            if (chk12.IsSelected == true) selected.Add("12");
            if (chk13.IsSelected == true) selected.Add("13");
            if (chk14.IsSelected == true) selected.Add("14");
            if (chk15.IsSelected == true) selected.Add("15");
            if (chk16.IsSelected == true) selected.Add("16");
            if (chk17.IsSelected == true) selected.Add("17");
            if (chk18.IsSelected == true) selected.Add("18");
            if (chk19.IsSelected == true) selected.Add("19");
            if (chk20.IsSelected == true) selected.Add("20");
            if (chk21.IsSelected == true) selected.Add("21");
            if (chk22.IsSelected == true) selected.Add("22");
            if (chk23.IsSelected == true) selected.Add("23");
            if (chk24.IsSelected == true) selected.Add("24");

            PropBag["Hours"].Value = string.Join(",", selected);

            Result.Prompt = PropBag["Hours"].Value;

            return true;
        }

        private void ChkNone_OnChanged(object? sender, ChangedEventArgs e)
        {
            SetReadOnly(chkNone.IsSelected);

            chk01.IsSelected = false;
            chk02.IsSelected = false;
            chk03.IsSelected = false;
            chk04.IsSelected = false;
            chk05.IsSelected = false;
            chk06.IsSelected = false;
            chk07.IsSelected = false;
            chk08.IsSelected = false;
            chk09.IsSelected = false;
            chk10.IsSelected = false;
            chk11.IsSelected = false;
            chk12.IsSelected = false;
            chk12.IsSelected = false;
            chk14.IsSelected = false;
            chk15.IsSelected = false;
            chk16.IsSelected = false;
            chk17.IsSelected = false;
            chk18.IsSelected = false;
            chk19.IsSelected = false;
            chk20.IsSelected = false;
            chk21.IsSelected = false;
            chk22.IsSelected = false;
            chk23.IsSelected = false;
            chk24.IsSelected = false;

            PropBag["Hours"].Value = "";
            Result.Prompt = PropBag["Hours"].Value;

        }

        private void SetReadOnly(bool isReadOnly)
        {
            for (int i = 1; i <= 24; i++)
            {
                string name = "chk" + i.ToString("00");
                if (GetTemplateChild(name) is NxButton button)
                    button.Enabled = isReadOnly;
            }
            Selection.Opacity = isReadOnly ? 0.25 : 1;
        }
    }
}
