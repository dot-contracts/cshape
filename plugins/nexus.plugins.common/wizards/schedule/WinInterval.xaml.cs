using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nexus.common.control;

namespace nexus.plugins.common
{
    public sealed partial class WinInterval : NxPanelBase
    {
        public string Value { get; private set; } = string.Empty;

        public WinInterval(string input)
        {
            this.InitializeComponent();
            if (int.TryParse(input, out var val))
            {
               // txtInterval.Value = val;
            }
        }

    }
}
