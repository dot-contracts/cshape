using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

using Windows.Media.Core;
using Windows.Media.Playback;


namespace nexus.common.control.ViewModels
{
    public class NxNumPadViewModel : ObservableObject
    {
        private string _currentValue = string.Empty;
        public string CurrentValue
        {
            get => _currentValue;
            set
            {
                SetProperty(ref _currentValue, value);
                NumberChanged?.Invoke(this, value);
            }
        }

        public ObservableCollection<NxNumPadButton> Buttons { get; } = new();

        public event EventHandler<string>? NumberChanged;

        public NxNumPadViewModel()
        {
            for (int i = 1; i <= 9; i++)
                Add(i.ToString(), () => Append(i.ToString()));

            Add("Clear", Clear);
            Add("0", () => Append("0"));
            Add("←", Backspace); // Unicode arrow
        }

        private void Add(string label, Action action)
        {
            Buttons.Add(new NxNumPadButton { Label = label, Command = new RelayCommand(() => { action(); PlayClickSound(); TriggerHapticFeedback(); }) });
        }

        private void Append(string digit) => CurrentValue += digit;
        private void Clear() => CurrentValue = string.Empty;
        private void Backspace()
        {
            if (!string.IsNullOrEmpty(CurrentValue))
                CurrentValue = CurrentValue[..^1];
        }

        private void PlayClickSound()
        {
            try
            {
                //var player = new MediaPlayer();
                //var uri = new Uri("ms-appx:///Assets/Sounds/click.wav");
                //player.Source = MediaSource.CreateFromUri(uri);
                //player.Play();
            }
            catch { }
        }

        private void TriggerHapticFeedback()
        {
#if __ANDROID__
            try
            {
                var vibrator = Android.OS.VibrationEffect.CreateOneShot(30, Android.OS.VibrationEffect.DefaultAmplitude);
                var vibService = (Android.OS.Vibrator)Android.App.Application.Context.GetSystemService(Android.Content.Context.VibratorService);
                vibService?.Vibrate(vibrator);
            }
            catch { }
#elif __IOS__
            try
            {
                var generator = new UIKit.UIImpactFeedbackGenerator(UIKit.UIImpactFeedbackStyle.Light);
                generator.Prepare();
                generator.ImpactOccurred();
            }
            catch { }
#endif
        }
    }

    public class NxNumPadButton
    {
        public string Label { get; set; } = string.Empty;
        public RelayCommand Command { get; set; } = null!;
    }
}
