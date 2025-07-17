using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;

using nexus.common;
using nexus.common.control;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public sealed partial class NxImage : NxControlBase
    {
        #region Enums

        public enum Pictures
        {
            adv_search, back, bank, calculator, calendar, camera, clock, cross, dollar, down,
            edit, favourite, favourite_add, favourite_delete, favourite_edit, file_add, file_delete,
            file_fav, file_graph, file_minus, file_new, file_properties, file_save, file_time,
            finished, folder, folder_add, folder_delete, folder_down, folder_edit, folder_fav,
            folder_graph, folder_music, folder_save, folder_serach, folder_text, folder_up,
            folder_view, folders, form, graph, info1, key, left, lightning, locked, mail_open,
            mail_unopened, member_add, member_delete, member_edit, member_male, member_search,
            members, meters_hard, moon, music, star, note, phone, pokie_edit, print, redo,
            redrun, right, search, search_icon, tick, time, tools, trash, undo, unlock, up,
            user, wizard
        }

        #endregion

        #region Attributes

        private Image          _imageCtl;
        private Border         _outerBorder;
        private ScaleTransform _imageScale;
        private BitmapImage    _bitmapImage;

        private Pictures _picture;

        public bool IsAnimate      { get; set; } = true;
        public bool AllowToggle    { get; set; } = false;
        public bool AllowAnimation { get; set; } = true;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(NxImage), new PropertyMetadata(false, OnIsCheckedChanged));
        public static readonly DependencyProperty IsReadonlyProperty = DependencyProperty.Register(nameof(IsReadonly), typeof(bool), typeof(NxImage), new PropertyMetadata(false));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public bool IsReadonly
        {
            get => (bool)GetValue(IsReadonlyProperty);
            set => SetValue(IsReadonlyProperty, value);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NxImage control)
            {
                control.UpdateBorderVisual();
            }
        }

        #endregion

        #region Constructor

        public NxImage()
        {
            DefaultStyleKey = typeof(NxImage);

            if (Tag == null) Tag = string.Empty;

            this.IsTabStop = false;
            this.PointerEntered  += OnPointerEntered;
            this.PointerExited   += OnPointerExited;
            this.PointerReleased += OnPointerReleased;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _outerBorder = GetTemplateChild("OuterBorder") as Border;
            _imageCtl    = GetTemplateChild("Picture") as Image;
            _imageScale  = (_imageCtl?.RenderTransform as ScaleTransform);

            LoadImageFromPictureEnum(_picture);
            UpdateBorderVisual();
            UpdateImageScale();

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }

        private void ApplyThemeDefaults()
        {
            //if (_outerBorder != null)
            //    _outerBorder.Background = new SolidColorBrush(NxThemeManager.Current.GetThemeColor("NxPanelBack", Colors.Black));
        }
        #endregion


        #region Public Properties

        public Pictures Picture
        {
            get => _picture;
            set
            {
                _picture = value;
                LoadImageFromPictureEnum(_picture);
            }
        }

        public BitmapImage BMI
        {
            get => _bitmapImage;
            set
            {
                _bitmapImage = value;
                if (_imageCtl != null)
                    _imageCtl.Source = _bitmapImage;
            }
        }

        public ImageSource Image
        {
            get => _imageCtl?.Source;
            set { if (_imageCtl != null) _imageCtl.Source = value; }
        }

        public double ImageWidth
        {
            get => Width;
            set => Width = value;
        }

        public double ImageHeight
        {
            get => Height;
            set => Height = value;
        }

        public Size ImageSize
        {
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
                UpdateImageScale();
            }
        }

        #endregion

        #region Events

        public delegate void OnClickedEventHandler(string tag);
        public event OnClickedEventHandler OnClicked;

        #endregion

        #region Pointer Event Handlers

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (AllowAnimation && IsAnimate && _imageCtl != null)
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (AllowAnimation && IsAnimate && _imageCtl != null)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (AllowToggle)
                IsChecked = !IsChecked;

            OnClicked?.Invoke(Tag?.ToString() ?? string.Empty);
        }

        #endregion

        #region Private Helpers

        private void UpdateBorderVisual()
        {
            if (_outerBorder != null)
            {
                _outerBorder.BorderBrush = IsChecked ? Theme.Accent.Brush : new SolidColorBrush(Colors.Transparent);
            }
        }

        private void LoadImageFromPictureEnum(Pictures picture)
        {
            if (_imageCtl == null) return;

            string imageName = picture.ToString();

            if (!string.IsNullOrWhiteSpace(imageName))
            {
                try
                {
                    _bitmapImage = new BitmapImage(new Uri($"ms-appx:///nexus.common.control/Images/{imageName}.png"));
                    _imageCtl.Source = _bitmapImage;
                }
                catch
                {
                    _bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Images/placeholder.png"));
                    _imageCtl.Source = _bitmapImage;
                }
            }
        }

        private void UpdateImageScale()
        {
            if (_imageScale == null || Width <= 0 || Height <= 0)
                return;

            _imageScale.ScaleX = 1;
            _imageScale.ScaleY = 1;
        }

        #endregion

        #region Public Methods

        public async Task<bool> LoadFromDialog()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                return await LoadFromFile(file);
            }
            return false;
        }

        public async Task<bool> LoadFromFile(StorageFile mediaFile)
        {
            _bitmapImage = new BitmapImage();
            try
            {
                using (IRandomAccessStream stream = await mediaFile.OpenAsync(FileAccessMode.Read))
                {
                    await _bitmapImage.SetSourceAsync(stream);
                    if (_imageCtl != null)
                        _imageCtl.Source = _bitmapImage;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void PictureFromString(string value)
        {
            if (Enum.TryParse(value, ignoreCase: true, out Pictures result))
            {
                Picture = result;
            }
            else
            {
                Picture = Pictures.back;
            }
        }

        #endregion
    }
}