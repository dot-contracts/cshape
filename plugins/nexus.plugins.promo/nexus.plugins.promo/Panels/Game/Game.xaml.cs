using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using nexus.common;
using nexus.common.control;
using nexus.shared.promo;

namespace nexus.plugins.promo
{
    public partial class Game : NxPanelBase
    {
        private GameDraw gamedraw;
        private string PromotionId;

        private DispatcherTimer counterTimer;
        //private List<Entry> pendingEntries = new();
        private ImageBrush ticketBrush;
        private bool ticketImageLoaded = false;

        public Game()
        {
            InitializeComponent();
        }

        public async void Create(string PromotionId)
        {
            this.PromotionId = PromotionId;

            gamedraw = new GameDraw();
            gamedraw.Create();
            gamedraw.Visibility = Visibility.Collapsed;
            WrapWithForm.AddPanel(gamedraw);
            WrapWithForm.AddButtons("Return;Draw Member;Show Promos");
            WrapWithForm.OnMenuClicked += GridWithForm_OnMenuClicked;

            await LoadTicketBackground();  // Load image first
        }

        private async Task LoadTicketBackground()
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.ImageOpened += TicketImage_ImageOpened;
                bitmap.UriSource = new Uri("ms-appx:///Assets/ticket_bg.png");

                ticketBrush = new ImageBrush
                {
                    ImageSource = bitmap,
                    Stretch = Stretch.UniformToFill
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start image load: {ex.Message}");
            }
        }

        private async void TicketImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            ticketImageLoaded = true;
            Console.WriteLine("Ticket image loaded.");

            await ShowData(); // Only show data after image is loaded
        }

        private async Task ShowData()
        {
            if (!ticketImageLoaded)
                return; // Don't load entries until image is ready

            try
            {
                TransactionRequest request = new()
                {
                    ActionType = 0,
                    Vendor = setting.Vendor,
                    Location = setting.Location,
                    PromotionId = PromotionId
                };

                using var helper = new TransactionHelper();
                var data = await helper.Process(request);

                foreach (var t in data.Transactions)
                {
                    if (!WrapWithForm.WrapItemExists(c => c is Ticket e && e.Tag?.ToString() == t.TransactionId.ToString()))
                    {
                        var entry = new Ticket(t);
                        entry.Create();
                        
                        //entry.Background = new SolidColorBrush(Colors.Red);//  ticketBrush;

                        entry.Background = ticketBrush;

                        entry.Tag = t.TransactionId;
                        WrapWithForm.AddWrapItem(entry); // Add only now that the image is ready
                    }
                }

                // Restart timer only after entries are loaded
                if (counterTimer == null)
                {
                    counterTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };
                    counterTimer.Tick += async (s, e) => await ShowData();
                    counterTimer.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
            }
        }

        private void GridWithForm_OnMenuClicked(object? sender, ClickedEventArgs e)
        {
            HandleButtonClicked(e.Tag);
        }

        private async void HandleButtonClicked(string buttonName = "")
        {
            switch (buttonName)
            {
                case "Show Promos":
                    RaiseEvent(enums.EventTypes.command, "Promos", "ShowPromos");
                    break;

                case "Draw Member":
                    gamedraw.Visibility = Visibility.Visible;
                    await WrapWithForm.AnimateForm(true, 1);
                    WrapWithForm.AddButtons("Return;Cancel");
                    break;

                case "Cancel":
                    await WrapWithForm.AnimateForm(false, 1);
                    WrapWithForm.AddButtons("Return;Draw Member");
                    break;
            }
        }

        #region Override Methods
        public new bool Load(int LoadState, string Property) => false;
        public new void Reset() { }
        public new bool Save() => false;
        public new bool Process() => false;
        public new bool Execute(string Command, string Parameters) => false;
        #endregion
    }
}
