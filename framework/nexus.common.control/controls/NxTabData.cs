
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Windows.System;

using nexus.common;
using nexus.common.control;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public partial class NxTabData : NxControlBase
    {
        public event ChangedEventHandler?        OnDataRowChanged;    public delegate void ChangedEventHandler(int dataRowIndex, string Display, string Value);
        public event RowDoubleClickEventHandler? OnRowDoubleClicked;  public delegate void RowDoubleClickEventHandler(int dataRowIndex, string Display, string Value);

        private StackPanel? _dataPanel;
        private NxButton? _btnPrev;
        private NxButton? _btnNext;

        private List<object> _sourceData = new();
        private List<object> _pagedData = new();
        private List<FieldDefinition> _fieldDefs = new();
        public IReadOnlyList<object> SourceData => _sourceData;

        private bool _initialized;
        private int _currentPage = 0;
        private int _pageSize;
        private bool _preferAutoSize;
        private bool _isTouchMode;
        private bool _showHeader = true;
        private int _selectedIndex = -1;
        private DateTime lastClickTime = DateTime.MinValue;

        private readonly List<Border> _rowContainers = new();
        private readonly List<int> _multiSelectedIndices = new();
        public bool AllowMultipleSelection { get; set; }

        public string Value { get; set; } = "";
        public string ValuePath { get; set; } = "Id";
        public string Display { get; set; } = "";
        public string DisplayPath { get; set; } = "Description";

        public bool ShowHeader
        {
            get => _showHeader;
            set => _showHeader = value;
        }

        public bool EnablePaging { get; set; }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                EnablePaging = _pageSize > 0;
                ApplyPaging();
                UpdatePagingButtons();
            }
        }

        public bool PreferAutoSize
        {
            get => _preferAutoSize;
            set { _preferAutoSize = value; ApplyPaging(); }
        }

        public bool IsTouchMode
        {
            get => _isTouchMode;
            set { _isTouchMode = value; ApplyPaging(); }
        }


        private NxImage? _menuIcon;

        public static readonly DependencyProperty RowHeightProperty  = DependencyProperty.Register(nameof(RowHeight),   typeof(double), typeof(NxTabData), new PropertyMetadata(32.0));
        public static readonly DependencyProperty ShowMenuProperty   = DependencyProperty.Register(nameof(ShowMenu),    typeof(bool),   typeof(NxTabData), new PropertyMetadata(false));
        public static readonly DependencyProperty MenuItemsProperty  = DependencyProperty.Register(nameof(MenuItems),   typeof(string), typeof(NxTabData), new PropertyMetadata("Print;Export"));
        public static readonly DependencyProperty SelectModeProperty = DependencyProperty.Register(nameof(SelectModes), typeof(bool),   typeof(NxTabData), new PropertyMetadata(SelectModes.Single));

        public bool        ShowMenu      { get => (bool)       GetValue(ShowMenuProperty);   set => SetValue(ShowMenuProperty, value); }
        public string      MenuItems     { get => (string)     GetValue(MenuItemsProperty);  set => SetValue(MenuItemsProperty, value); }
        public SelectModes SelectionMode { get => (SelectModes)GetValue(SelectModeProperty); set => SetValue(SelectModeProperty, value); }
        public double      RowHeight     { get => (double)     GetValue(RowHeightProperty);  set => SetValue(RowHeightProperty, value); }


        public NxTabData()
        {
            this.DefaultStyleKey = typeof(NxTabData);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dataPanel = GetTemplateChild("PART_DataPanel") as StackPanel;
            _btnPrev = GetTemplateChild("PART_PrevPage") as NxButton;
            _btnNext = GetTemplateChild("PART_NextPage") as NxButton;

            if (_btnPrev != null)
                _btnPrev.OnClicked += (s, e) => { if (_currentPage > 0) _currentPage--; ApplyPaging(); };

            if (_btnNext != null)
                _btnNext.OnClicked += (s, e) => { _currentPage++; ApplyPaging(); };


            _menuIcon = GetTemplateChild("PART_MenuIcon") as NxImage;
            if (_menuIcon != null)
            {
                _menuIcon.Visibility = ShowMenu ? Visibility.Visible : Visibility.Collapsed;
                _menuIcon.Tapped += (s, e) => ShowMenuPopup();
            }

            this.KeyDown += NxTabData_KeyDown;
            this.IsTabStop = true;
            this.Focus(FocusState.Programmatic);

            _initialized = true;
            ApplyPaging();
        }

        public void SetItemsSource<T>(List<T> source, List<FieldDefinition> fieldDefinitions)
        {
            Reset();

            _sourceData = source.Cast<object>().ToList();
            _fieldDefs = fieldDefinitions;

            ApplyPaging();
        }

        public void SetItemsSource(DataTable table, List<FieldDefinition> fieldDefinitions)
        {
            Reset();

            _currentPage = 0;
            _fieldDefs = fieldDefinitions;
            _sourceData = table.AsEnumerable().ToList<object>();

            ApplyPaging();
        }

        private void ApplyPaging()
        {
            if (!_initialized || _dataPanel == null || _sourceData == null || _fieldDefs == null)
                return;

            _dataPanel.Children.Clear();
            _rowContainers.Clear();

            if (ShowHeader)
            {
                var headerGrid = new Grid { MinHeight = 32 };
                foreach (var field in _fieldDefs)
                {
                    headerGrid.ColumnDefinitions.Add(
                        !string.IsNullOrWhiteSpace(field.Width) && double.TryParse(field.Width, out var width)
                        ? new ColumnDefinition { Width = new GridLength(width) }
                        : new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                }

                for (int i = 0; i < _fieldDefs.Count; i++)
                {
                    var field = _fieldDefs[i];

                    var tb = new TextBlock
                    {
                        Text = field.Header,
                        FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                        Margin = new Thickness(4, 0, 4, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.NoWrap,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        TextAlignment = field.TextAlign?.ToLower() switch
                        {
                            "right" => Microsoft.UI.Xaml.TextAlignment.Right,
                            "center" => Microsoft.UI.Xaml.TextAlignment.Center,
                            _ => Microsoft.UI.Xaml.TextAlignment.Left
                        }
                    };

                    var border = new Border
                    {
                        Background = NxThemeManager.Current.GetThemeBrush("NxListBack", Colors.Black),
                        BorderBrush = new SolidColorBrush(Colors.LightGray),
                        BorderThickness = new Thickness(0, 0, 1, 0),
                        Padding = new Thickness(4, 2, 4, 2),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Child = tb
                    };


                    Grid.SetColumn(border, i);
                    headerGrid.Children.Add(border);
                }

                var headerContainer = new Border
                {
                    Background = NxThemeManager.Current.GetThemeBrush("NxListBack", Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    Child = headerGrid
                };

                _dataPanel.Children.Add(headerContainer);
            }

            int skip = EnablePaging ? _currentPage * PageSize : 0;
            _pagedData = EnablePaging ? _sourceData.Skip(skip).Take(PageSize).ToList() : _sourceData;

            for (int rowIndex = 0; rowIndex < _pagedData.Count; rowIndex++)
            {
                var item = _pagedData[rowIndex];
                var grid = new Grid { MinHeight = RowHeight };

                foreach (var field in _fieldDefs)
                {
                    grid.ColumnDefinitions.Add(
                        !string.IsNullOrWhiteSpace(field.Width) && double.TryParse(field.Width, out var width)
                        ? new ColumnDefinition { Width = new GridLength(width) }
                        : new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                }

                for (int i = 0; i < _fieldDefs.Count; i++)
                {
                    var field = _fieldDefs[i];
                    var tb = new TextBlock
                    {
                        Text = GetPropertyValue(item, field.Field)?.ToString(),
                        Margin = new Thickness(4, 0, 4, 0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.NoWrap,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };

                    if (field.TextAlign?.ToLower() == "right")
                        tb.TextAlignment = Microsoft.UI.Xaml.TextAlignment.Right;

                    var border = new Border
                    {
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(0),
                        Child = tb
                    };

                    Grid.SetColumn(border, i);
                    grid.Children.Add(border);
                }

                var isSelected = AllowMultipleSelection ? _multiSelectedIndices.Contains(rowIndex) : _selectedIndex == rowIndex;
                var bgColor = isSelected
                    ? NxThemeManager.Current.GetThemeBrush("NxListSelectedBack", Colors.Black)
                    : rowIndex % 2 == 0
                        ? NxThemeManager.Current.GetThemeBrush("NxListBack", Colors.Black)
                        : NxThemeManager.Current.GetThemeBrush("NxListAltBack", Colors.Black);

                var rowContainer = new Border
                {
                    Background = bgColor,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Transparent),
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    Child = grid
                };

                int capturedIndex = rowIndex;
                rowContainer.PointerEntered += (s, e) =>
                {
                    rowContainer.BorderBrush = NxThemeManager.Current.GetThemeBrush("NxListPointerOver", Colors.White);
                };
                rowContainer.PointerExited += (s, e) =>
                {
                    if (_selectedIndex != capturedIndex)
                        rowContainer.BorderBrush = new SolidColorBrush(Colors.Transparent);
                };


                rowContainer.Tapped += (s, e) =>
                {
                    if (AllowMultipleSelection)
                    {
                        if (_multiSelectedIndices.Contains(capturedIndex))
                            _multiSelectedIndices.Remove(capturedIndex);
                        else
                            _multiSelectedIndices.Add(capturedIndex);
                    }
                    else
                    {
                        if (_selectedIndex != capturedIndex && _selectedIndex >= 0 && _selectedIndex < _rowContainers.Count)
                        {
                            var previous = _rowContainers[_selectedIndex];
                            previous.Background = (_selectedIndex % 2 == 0)
                                ? NxThemeManager.Current.GetThemeBrush("NxListBack", Colors.Black)
                                : NxThemeManager.Current.GetThemeBrush("NxListAltBack", Colors.Black);
                        }
                        _selectedIndex = capturedIndex;
                    }

                    rowContainer.Background = NxThemeManager.Current.GetThemeBrush("NxListSelectedBack", Colors.Black);
                    RaiseSelectionChanged();
                };

                rowContainer.PointerPressed += (s, e) =>
                {
                    var currentTime = DateTime.Now;
                    if ((currentTime - lastClickTime).TotalMilliseconds < 400)
                    {
                        int absoluteIndex = EnablePaging ? (_currentPage * PageSize) + capturedIndex : capturedIndex;

                        if (absoluteIndex >= 0 && absoluteIndex < _sourceData.Count)
                        {
                            var srcItem = _sourceData[absoluteIndex];
                            var display = GetPropertyValue(srcItem, DisplayPath)?.ToString() ?? string.Empty;
                            var value = GetPropertyValue(srcItem, ValuePath)?.ToString() ?? string.Empty;

                            OnRowDoubleClicked?.Invoke(absoluteIndex, display, value);
                        }

                        lastClickTime = DateTime.MinValue;
                    }
                    else
                    {
                        lastClickTime = currentTime;
                    }
                };

                _rowContainers.Add(rowContainer);
                _dataPanel.Children.Add(rowContainer);
            }

            UpdatePagingButtons();
        }

        private void RaiseSelectionChanged()
        {
            if (_selectedIndex >= 0 && _selectedIndex < _pagedData.Count)
            {
                var item = _pagedData[_selectedIndex];
                OnDataRowChanged?.Invoke(_selectedIndex,
                    GetPropertyValue(item, DisplayPath)?.ToString() ?? string.Empty,
                    GetPropertyValue(item, ValuePath)?.ToString() ?? string.Empty);
            }
        }

        private void UpdatePagingButtons()
        {
            if (_sourceData == null) return;
            if (_btnPrev == null || _btnNext == null) return;

            _btnPrev.IsEnabled = _currentPage > 0;
            _btnNext.IsEnabled = (_currentPage + 1) * PageSize < _sourceData.Count;
        }

        private object? GetPropertyValue(object obj, string path)
        {
            if (string.IsNullOrEmpty(path) || obj == null)
                return null;

            if (obj is DataRow row)
                return row.Table.Columns.Contains(path) ? row[path] : null;

            var parts = path.Split('.');
            object? current = obj;

            foreach (var part in parts)
            {
                if (current == null)
                    return null;

                var prop = current.GetType().GetProperty(part);
                if (prop == null)
                    return null;

                current = prop.GetValue(current);
            }

            return current;
        }

        private void NxTabData_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_pagedData == null || _pagedData.Count == 0) return;

            switch (e.Key)
            {
                case VirtualKey.Up:
                    if (_selectedIndex > 0) _selectedIndex--;
                    break;
                case VirtualKey.Down:
                    if (_selectedIndex < _pagedData.Count - 1) _selectedIndex++;
                    break;
                case VirtualKey.Home:
                    _selectedIndex = 0;
                    break;
                case VirtualKey.End:
                    _selectedIndex = _pagedData.Count - 1;
                    break;
                case VirtualKey.PageUp:
                    _selectedIndex = Math.Max(0, _selectedIndex - PageSize);
                    break;
                case VirtualKey.PageDown:
                    _selectedIndex = Math.Min(_pagedData.Count - 1, _selectedIndex + PageSize);
                    break;
                case VirtualKey.Enter:
                    RaiseSelectionChanged();
                    break;
                default:
                    return;
            }

            ApplyPaging();
            RaiseSelectionChanged();
            e.Handled = true;
        }

        private void ShowMenuPopup()
        {
            var menu = new MenuFlyout();
            var items = (MenuItems ?? "Print;Export").Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                var menuItem = new MenuFlyoutItem { Text = item.Trim() };
                menuItem.Click += (s, e) =>
                {
                    switch (item.Trim().ToLower())
                    {
                        case "print":
                            HandlePrint();
                            break;
                        case "export":
                            HandleExport();
                            break;
                    }
                };
                menu.Items.Add(menuItem);
            }
            menu.ShowAt(_menuIcon);
        }

        private async void HandleExport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(string.Join(",", _fieldDefs.Select(f => $"\"{f.Header}\"")));

            foreach (var item in _pagedData)
            {
                var values = _fieldDefs.Select(f => $"\"{GetPropertyValue(item, f.Field)?.ToString()?.Replace("\"", "\"\"") ?? ""}\"");
                sb.AppendLine(string.Join(",", values));
            }

#if __WASM__
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
            var filename = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            Uno.Foundation.WebAssemblyRuntime.InvokeJS($@"
                var a = document.createElement('a');
                a.href = 'data:text/csv;base64,{base64}';
                a.download = '{filename}';
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
            ");
#else
            var filename = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, sb.ToString());

            var dialog = new ContentDialog
            {
                Title = "Export Successful",
                Content = $"File saved to local storage:\n{filename}",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
#endif
        }

        private async void HandlePrint()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("NxTabData Printout");
            foreach (var item in _pagedData)
            {
                var line = string.Join(" | ", _fieldDefs.Select(f => $"{GetPropertyValue(item, f.Field)}"));
                sb.AppendLine(line);
            }

            var dialog = new ContentDialog
            {
                Title = "Print Preview",
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = sb.ToString(),
                        TextWrapping = TextWrapping.Wrap
                    }
                },
                CloseButtonText = "Close",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public void Reset()
        {
            _currentPage   = 0;
            _selectedIndex = -1;
            _multiSelectedIndices.Clear();
        }
    }
}
