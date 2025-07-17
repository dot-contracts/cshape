
using System.Data;
using System.Linq;
using Microsoft.UI;
using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.control.Themes;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.gaming;
using Windows.UI.Core;

namespace nexus.plugins.promo
{
    public sealed partial class MainUI : NxMainUIBase, IMainUI
    {

        private Promos    Promos;
        private Game      Game;

        private delegate bool ProcessCommandCB(string Data);
        private delegate void CreateCB();
        private delegate void ShowPanelCB(int MemberID);

        public MainUI()
        {
            this.InitializeComponent();

            MainInfo = new MainInfo("Promos", "957A050D-8418-4341-B6D1-D647937F3C99");

            MaxLoadLoops = 20;

            this.Loaded       += MainUI_Loaded;
            base.LoadLoop     += MainUI_LoadLoop;
            base.LoopFinished += MainUI_LoopFinished;
        }

        private void MainUI_Loaded(object sender, RoutedEventArgs e) { TimerStart(20); }

        private async void MainUI_LoadLoop()
        {
            try
            {
                switch (LoadState)
                {
                    case 0:
                        //await Task.Run(async () => await InitStartupAsync());

                        await EnumCodeHelper.GetEnums();

                        msg.OnPluginEvent += _OnEvent;

                        LoadState++;
                        break;

                    case 1: // connect to SignalR instead
                            //tcpClient.Create("Main", "man","man123");
                        LoadState++;
                        break;

                    case 2:
                        Promos = new Promos(); Host.Children.Add(Promos); Promos.Visibility = Visibility.Visible;   Promos.OnEvent += _OnEvent; 
                        Game   = new Game();   Host.Children.Add(Game);   Game.Visibility   = Visibility.Collapsed; Game  .OnEvent += _OnEvent;

                        LoadState++;
                        break;

                    case 3: // load any default module based on look up
                        if (Promos!=null) 
                           Promos.Create();

                        LoadState++;
                        break;

                    case 4:
                        ApplyThemeDefaults();
                        NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();

                        LoadState++;
                        break;

                    default:
                        LoadState = MaxLoadLoops;
                        break;
                }

            }
            catch (Exception ex)
            {
                string xx = ex.Message;

            }
        }

        private void ApplyThemeDefaults()
        {
        }

        private void MainUI_LoopFinished()
        {
        }
        public async Task InitStartupAsync()
        {

            await EnumCodeHelper.GetEnums();

            //using (OptionHelper opt = new OptionHelper())
            //{
            //    await opt.GetOptions(OptionPath: "Startup.State");

            //    string Params = opt.GetValue("State", "");
            //    if (!string.IsNullOrEmpty(Params))
            //        msg.RaisePluginEvent(enums.EventTypes.command, "", "SetState", Params);
            //}
        }

        private async void _OnEvent(enums.EventTypes reason, string sender = "", string command = "", string Property = "")
        {
            switch (reason)
            {
                case enums.EventTypes.command:
                    if (command == "ShowPromos")
                    {
                        if (Promos != null)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Game.Visibility   = Visibility.Collapsed;
                                Promos.Visibility = Visibility.Visible;
                                //Game.Create(Property);
                            });

                        }
                    }
                    else if (command == "ShowGame")
                    {
                        if (Game != null)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Game.Visibility   = Visibility.Visible;
                                Promos.Visibility = Visibility.Collapsed;
                                Game.Create(Property);
                            });
                        }
                    }
                    break;

                default:
                    break;
            }   

        }

    }
}
