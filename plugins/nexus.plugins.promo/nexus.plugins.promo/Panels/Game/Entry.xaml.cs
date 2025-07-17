
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using nexus.shared.promo;

namespace nexus.plugins.promo
{
    public sealed partial class Ticket : UserControl
    {
        private readonly Transaction trans;
        public int? TransactionId { get; private set; }

        public Ticket(Transaction trans)
        {
            this.trans = trans;
            this.InitializeComponent();
        }

        public void Create()
        {
            this.TransactionId = trans.TransactionId;

            // Only works if your XAML has these x:Name elements
            lbMember.Prompt = trans.Member;
            lbHouse.Prompt = $"House {trans.Device}";
            lbEntries.Prompt = trans.TriggerCount.ToString();
        }
    }
}
