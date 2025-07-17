using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using nexus.common;
using nexus.common.control;
using nexus.shared.common;

namespace nexus.plugins.common
{
    public sealed partial class DlgSchedule : NxDialogBase
    {
        public Schedule? Schedule { get; private set; }
        public event Action<Schedule>? OnScheduleFinished;

        private readonly PropertyBag<NameValue> PropBag = new();
        private readonly NxDialog Dialog;

        public DlgSchedule(Schedule? existingSchedule = null)
        {
            this.InitializeComponent();

            Dialog = new NxDialog
            {
                Title               = "Schedule Wizard",
                DialogType          = DialogTypes.wizard,
                Padding             = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Stretch,
                Height              = 600, // Auto
                Width               = 300  // Auto
            };
            this.Content = Dialog;

            Dialog.OnFinished += Dialog_OnFinished;
            this.Loaded       += DlgSchedule_Loaded;

            if (existingSchedule == null)  Schedule = new Schedule();
            else                           Schedule = existingSchedule;

            PropBag.Add(new NameValue("StartsOn",   Schedule.StartsOn   ?? "AllDay"));
            PropBag.Add(new NameValue("StartsAt",   Schedule.StartsAt   ?? "AllDay"));
            PropBag.Add(new NameValue("FinishesOn", Schedule.FinishesOn ?? "Never"));
            PropBag.Add(new NameValue("FinishesAt", Schedule.FinishesAt ?? "Never"));
            PropBag.Add(new NameValue("Days",       Schedule.Days       ?? ""));
            PropBag.Add(new NameValue("Hours",      Schedule.Hours      ?? ""));

            Dialog.AddPanel("StartDate",  new WinStartDate (PropBag.GetValue("StartsOn")));
            Dialog.AddPanel("StartTime",  new WinStartTime (PropBag.GetValue("StartsAt")));
            Dialog.AddPanel("FinishDate", new WinFinishDate(PropBag.GetValue("FinishesOn")));
            Dialog.AddPanel("FinishTime", new WinFinishTime(PropBag.GetValue("FinishesAt")));
            Dialog.AddPanel("Days",       new WinDays      (PropBag.GetValue("Days")));
            Dialog.AddPanel("Hours",      new WinHours     (PropBag.GetValue("Hours")));

            Dialog.ShowFirst();
        }

        private void DlgSchedule_Loaded(object sender, RoutedEventArgs e)
        {
            Dialog.ShowFirst();
        }

        private bool Dialog_OnFinished()
        {
            for (int i = 0; i < Dialog.Panels.Count; i++)
            {
                var panel = Dialog.Panels[i];
                if (panel is NxPanelBase nxPanel)
                {
                    switch (i)
                    {
                        case 0: Schedule.StartsOn   = panel.PropBag.GetValue("StartsOn")?   .Value ?? "AllDay"; break;
                        case 1: Schedule.StartsAt   = panel.PropBag.GetValue("StartsAt")?   .Value ?? "AllDay"; break;
                        case 2: Schedule.FinishesOn = panel.PropBag.GetValue("FinishesOn")? .Value ?? "Never"; break;
                        case 3: Schedule.FinishesAt = panel.PropBag.GetValue("FinishesAt")? .Value ?? "Never"; break;
                        case 4: Schedule.Days       = panel.PropBag.GetValue("Days")?       .Value ?? ""; break;
                        case 5: Schedule.Hours      = panel.PropBag.GetValue("Hours")?      .Value ?? ""; break;
                    }
                }
            }

            OnScheduleFinished?.Invoke(Schedule);
            return true;
        }

        public async Task ShowAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            this.OnScheduleFinished += _ => tcs.TrySetResult(true);
            this.Show();
            await tcs.Task;
        }
    }
}
