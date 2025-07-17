using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Printing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Windows.Storage;

using nexus.common;
using nexus.common.control.Themes;
using Windows.Devices.Display.Core;
using System.Buffers;
using System.Collections;

namespace nexus.common.control
{
    public sealed partial class NxList : NxControlBase
    {
        #region Public Data

        public ObservableCollection<Dictionary<string, object>> Items   { get; } = new();
        public List<string>                                     Columns { get; private set; } = new();

        // Optional row/cell styling delegates
        public Func<Dictionary<string, object>, Brush> RowBackgroundEvaluator    { get; set; }
        public Func<string, object, Brush>             CellForegroundEvaluator   { get; set; }
        public Func<string, double, Brush>             SummaryHighlightEvaluator { get; set; }

        // Sorting/filtering/paging
        public string FilterText    { get; set; } = string.Empty;
        public string SortColumn    { get; set; } = string.Empty;
        public bool   SortAscending { get; set; } = true;
        public int    PageSize      { get; set; } = 25;
        public int    CurrentPage   { get; private set; } = 1;
        public int    SelectedIndex = 0;

        #endregion

        public enum GridTypes { SelectGrid, DropList }

        public  string    ValuePath   { get; set; } = string.Empty;
        public  string    DisplayPath { get; set; } = string.Empty;
        public  GridTypes GridType    { get; set; } = GridTypes.SelectGrid;

        public bool ShowPaging             { get; set; } = false;
        public bool ShowSummary            { get; set; } = false;
        public bool AllowGroupBy           { get; set; } = false;
        public bool AllowSort              { get; set; } = false;
        public bool AllowFilter            { get; set; } = false;
        public bool AllowResize            { get; set; } = true;
        public bool AllowMultiSelect       { get; set; } = true;
        public bool AutoSelect             { get; set; } = false;
        public bool RaiseMultiSelectEvents { get; set; } = false;

        private bool Loaded          = false;

        #region Value and Diisplay Properties
        private string _display = "";
        private string _value = "";

        public string Display
        {
            get { return _display; }
            set
            {
                _display = value;
                //FindValue(DisplayPath, value);
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                //FindValue(ValuePath, value);
            }
        }
        
        public bool FindValue(string searchPath, string searchValue)
        {
            try
            {
                searchPath = (string.IsNullOrEmpty(searchPath) ? ValuePath : searchPath);

                var record = _fullData.FirstOrDefault(r => r.ContainsKey(searchPath) && r[searchPath]?.ToString() == searchValue);
                if (record == null) return false;

                int index = _fullData.IndexOf(record);

                // Calculate the page that record appears on.
                CurrentPage = (index / PageSize) + 1;

                // Reapply filtering, sorting and paging to rebuild the Items collection.
                ApplyFilterSortAndPaging();

                // Determine the record's index within the current page.
                int indexInPage = index % PageSize;

                // Ensure the index is valid within the _rowsPanel.Children collection.
                if (indexInPage < _rowsPanel.Children.Count)
                {

                    // Retrieve the UI element corresponding to the record.
                    var rowElement = _rowsPanel.Children[indexInPage] as FrameworkElement;
                    if (rowElement != null)
                    {
                        // Scroll the element into view.
                        CustomBringIntoView(rowElement);

                        // Retrieve the underlying data from the DataContext.
                        if (rowElement.DataContext is Dictionary<string, object> rowData)
                        {
                            // Extract Display and Value using the paths provided.
                            Display = rowData.TryGetValue(DisplayPath, out var displayObj) ? displayObj?.ToString() ?? "" : "";
                            Value   = rowData.TryGetValue(ValuePath,   out var valueObj)   ? valueObj?  .ToString() ?? "" : "";
                        }
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
            }

            return false;
        }
        private T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T typedParent)
                {
                    return typedParent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return default;
        }
        private void CustomBringIntoView(UIElement element)
        {
            // Find the parent ScrollViewer.
            var scrollViewer = FindParent<ScrollViewer>(element);
            if (scrollViewer != null)
            {
                // Calculate the element's position relative to the ScrollViewer.
                var transform = element.TransformToVisual(scrollViewer);
                var point = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
                // Scroll the ScrollViewer to the element's location.
                scrollViewer.ChangeView(point.X, point.Y, null);
            }
        }
        #endregion 

        #region Template Parts

        private Grid       _headerGrid;      // PART_HeaderGrid
        private StackPanel _rowsPanel;       // PART_RowsPanel
        private StackPanel _summaryPanel;    // PART_SummaryPanel
        private StackPanel _pagerPanel;      // PART_PagerPanel

        #endregion

        #region Internal Fields

        private List<Dictionary<string, object>> _fullData  = new();
        private Dictionary     <string, double>  _summaries = new();

        #endregion

        #region Events

        public delegate void BeforeSelectionChangedEventHandler(string Display, string Value, ref bool Cancel);     public event BeforeSelectionChangedEventHandler BeforeSelectionChanged;
        public delegate void AfterSelectionChangedEventHandler (string Display, string Value);                      public event AfterSelectionChangedEventHandler  AfterSelectionChanged;
        public delegate void OnMultipleSelectionsEventHandler  (string Values);                                     public event OnMultipleSelectionsEventHandler   OnMultipleSelections;
        public delegate void GetFieldLayoutEventHandler        (string ColumnName, Int32 GridColumn);               public event GetFieldLayoutEventHandler         GetFieldLayout;
        public delegate void GetFieldLayoutEventHandlerEx      (NxList List, string ColumnName, Int32 GridColumn);  public event GetFieldLayoutEventHandlerEx       GetFieldLayoutEx;
        //public delegate void OnLoadingRowEventHandler          (System.Windows.Controls.DataGridRow Row);           public event OnLoadingRowEventHandler           OnLoadingRow;
        //public delegate void UnLoadingRowEventHandler          (System.Windows.Controls.DataGridRow Row);           public event UnLoadingRowEventHandler           UnLoadingRow;
        public delegate void OnInValidEventHandler             (string errorMsg);                                   public event OnInValidEventHandler              OnInValid;
        public delegate void OnDataRowChangedEventHandler      (int    DataRowIndex);                               public event OnDataRowChangedEventHandler       OnDataRowChanged;

        private Dictionary<string, object> _selectedRow = null;
        public  Dictionary<string, object> SelectedRow
        {
            get => _selectedRow;
            set
            {
                // Get values for display and key (if available)
                string newDisplay = (value != null && value.ContainsKey(DisplayPath)) ? value[DisplayPath]?.ToString() : "";
                string newValue = (value != null && value.ContainsKey(ValuePath)) ? value[ValuePath]?.ToString() : "";

                // Raise the "before" event: allow cancellation
                bool cancel = false;
                // For example, using the already defined BeforeSelectionChanged delegate
                BeforeSelectionChanged?.Invoke(newDisplay, newValue, ref cancel);
                if (cancel)
                {
                    // Abort changing the row if any handler cancels the change.
                    return;
                }

                // Update the selected row
                _selectedRow = value;

                // Now that the change has been made, raise the "after" event.
                AfterSelectionChanged?.Invoke(newDisplay, newValue);

                this.Value = newValue;
                this.Display = newDisplay;

                RaiseOnChanged(Value);
            }
        }




        #endregion

        #region Constructors

        public NxList()
        {
            // Use Themes/Generic.xaml for layout
            DefaultStyleKey = typeof(NxList);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Grab XAML template parts
            _headerGrid   = GetTemplateChild("PART_HeaderGrid") as Grid;
            _rowsPanel    = GetTemplateChild("PART_RowsPanel") as StackPanel;
            _summaryPanel = GetTemplateChild("PART_SummaryPanel") as StackPanel;
            _pagerPanel   = GetTemplateChild("PART_PagerPanel") as StackPanel;

            RowBackgroundEvaluator = (row) =>
            {
                if(SelectedRow == null)
                    return null;
    
                bool equal = SelectedRow.OrderBy(kv => kv.Key).SequenceEqual(row.OrderBy(kv => kv.Key));
                if (equal)
                    return NxThemeManager.Current.ButtonBack.Brush;
                else
                    return null;
            };

            // Rebuild everything
            //BuildHeader();
            PopulateRows();
            UpdateSummary();
            BuildPager();

            Loaded = true;

        }
        #endregion

        #region Data Binding

        public void SetItemsSource<T>(List<T> items)
        {
            if (items == null || items.Count == 0)
            {
                _fullData.Clear();
                Items.Clear();
                return;
            }

            Columns = typeof(T).GetProperties()
                               .Select(p => p.Name)
                               .ToList();

            _fullData.Clear();

            foreach (var item in items)
            {
                var dict = new Dictionary<string, object>();

                foreach (var prop in typeof(T).GetProperties())
                {
                    dict[prop.Name] = prop.GetValue(item) ?? "";
                }

                _fullData.Add(dict);
            }

            ApplyFilterSortAndPaging();
        }

        public void ClearDataTable()
        {
            Items.Clear();
        }
        public bool HasRows()
        {
            bool ret = false;
            if (_fullData != null) ret = _fullData.Count > 0;
            return ret;
        }

        public void SetListTable(string ListData) { SetDataTable(helpers.CreateDataTable(ListData)); }
        public void SetENumTable(DataTable Data, string DisplayPath = "ValueDesc", string ValuePath = "ValuePk", GridTypes GridType = GridTypes.DropList, string Value = "")
        { SetDataTable(Data, DisplayPath, ValuePath, GridType, Value); }
        public void SetDataTable(DataTable Data, string DisplayPath = "Description", string ValuePath= "Id", GridTypes GridType = GridTypes.DropList , string Value = "")
        {
            this.DisplayPath = DisplayPath;
            this.ValuePath   = ValuePath;
            this.GridType    = GridType;
            this.Value = Value;
            if (Data != null)
            {
                Columns = Data.Columns
                    .Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();

                _fullData.Clear();
                foreach (DataRow row in Data.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var col in Columns)
                        dict[col] = row[col];
                    _fullData.Add(dict);
                }

                ApplyFilterSortAndPaging();
            }
        }

        #endregion

        #region Filtering/Sorting/Paging

        private void ApplyFilterSortAndPaging()
        {
            var query = _fullData.AsEnumerable();

            // Filter
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                query = query.Where(row =>
                    row.Values.Any(v => v?.ToString()?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            // Sort
            if (!string.IsNullOrWhiteSpace(SortColumn) && Columns.Contains(SortColumn))
            {
                query = SortAscending
                    ? query.OrderBy(row => row[SortColumn])
                    : query.OrderByDescending(row => row[SortColumn]);
            }

            // Paginate
            var filtered = query.ToList();
            int totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, totalPages));

            var paged = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Refill Items
            Items.Clear();
            foreach (var item in paged)
            {
                if (!string.IsNullOrEmpty(ValuePath))
                {
                    if (item[ValuePath] == Value) SelectedRow = item;
                }
                Items.Add(item);
            }

            // If template is applied, rebuild UI
            //BuildHeader();
            PopulateRows();
            UpdateSummary();
            BuildPager();
        }

        #endregion

        #region Layout Builders

        private void BuildHeader()
        {
            if (_headerGrid == null) return;

            _headerGrid.Children.Clear();
            _headerGrid.ColumnDefinitions.Clear();

            foreach (var _ in Columns)
                _headerGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

            for (int i = 0; i < Columns.Count; i++)
            {
                var colName = Columns[i];
                var headerBtn = new NxButton
                {
                    Prompt = colName,
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    Margin = new Thickness(0),   //4, 2, 4, 2)
                    Height = 20
                };
                headerBtn.OnClicked += (s, e) =>
                {
                    if (SortColumn == colName)
                        SortAscending = !SortAscending;
                    else
                    {
                        SortColumn = colName;
                        SortAscending = true;
                    }
                    ApplyFilterSortAndPaging();
                };
                Grid.SetColumn(headerBtn, i);
                _headerGrid.Children.Add(headerBtn);
            }
        }

        private void PopulateRows()
        {
            if (_rowsPanel == null) return;
            _rowsPanel.Children.Clear();

            // Build one Grid per item in Items
            foreach (var row in Items)
            {
                var rowGrid = new Grid
                {
                    DataContext = row  // Store the underlying data so you can retrieve it later.
                };

                // Column definitions
                foreach (var _ in Columns)
                {
                    rowGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                }

                // Optional row background
                if (RowBackgroundEvaluator != null)
                {
                    var bg = RowBackgroundEvaluator(row);
                    if (bg != null) rowGrid.Background = bg;
                    else rowGrid.Background = null;
                }

                // Build cells
                for (int i = 0; i < Columns.Count; i++)
                {
                    var key = Columns[i];
                    var val = row[key]?.ToString() ?? "";
                    var textBlock = new TextBlock
                    {
                        Text = val,
                        Margin = new Thickness(4, 2, 4, 2),
                        Visibility = key == DisplayPath ? Visibility.Visible : Visibility.Collapsed
                    };

                    // Cell foreground
                    if (CellForegroundEvaluator != null)
                    {
                        var brush = CellForegroundEvaluator(key, row[key]);
                        if (brush != null) textBlock.Foreground = brush;
                    }

                    Grid.SetColumn(textBlock, i);
                    rowGrid.Children.Add(textBlock);
                }


                // Attach a tapped event to change the SelectedRow when this row is clicked.
                rowGrid.Tapped += (s, e) =>
                {
                    // The tapped row's display and value can be determined using your display and value paths.
                    // Setting the SelectedRow property will raise the events.
                    SelectedRow = row;
                };
                _rowsPanel.Children.Add(rowGrid);
            }
        }

        private void UpdateSummary()
        {
            if (_summaryPanel == null) return;
            _summaryPanel.Children.Clear();
            if (!ShowSummary)
            {
                _summaryPanel.Visibility = Visibility.Collapsed;
                return;
            }


            if (Items.Count == 0) return;

            _summaries.Clear();

            // Summaries for numeric columns
            foreach (var key in Columns)
            {
                // If entire column is numeric
                if (Items.All(r => double.TryParse(r[key]?.ToString(), out _)))
                {
                    double sum = Items.Sum(r => Convert.ToDouble(r[key]));
                    _summaries[key] = sum;

                    var summaryText = new TextBlock
                    {
                        Text = $"{key}: {sum:N2}",
                        Margin = new Thickness(10, 0, 10, 0),
                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
                    };

                    if (SummaryHighlightEvaluator != null)
                    {
                        var brush = SummaryHighlightEvaluator(key, sum);
                        if (brush != null)
                            summaryText.Foreground = brush;
                    }

                    _summaryPanel.Children.Add(summaryText);
                }
            }
        }

        private void BuildPager()
        {
            if (_pagerPanel == null) return;
            _pagerPanel.Children.Clear();
            if (!ShowPaging)
            {
                _pagerPanel.Visibility = Visibility.Collapsed;
                return;
            }

            var prevBtn = new Button
            {
                Content = "< Prev",
                Margin = new Thickness(4)
            };
            prevBtn.Click += (s, e) =>
            {
                if (CurrentPage > 1)
                {
                    CurrentPage--;
                    ApplyFilterSortAndPaging();
                }
            };
            _pagerPanel.Children.Add(prevBtn);

            var pageText = new TextBlock
            {
                Text = $" Page {CurrentPage} ",
                VerticalAlignment = VerticalAlignment.Center
            };
            _pagerPanel.Children.Add(pageText);

            var nextBtn = new Button
            {
                Content = "Next >",
                Margin = new Thickness(4)
            };
            nextBtn.Click += (s, e) =>
            {
                CurrentPage++;
                ApplyFilterSortAndPaging();
            };
            _pagerPanel.Children.Add(nextBtn);
        }

        #endregion

        #region Export/Print

        public async void ExportToCsv(string fileName = "NxGridLiteExport.csv")
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(desktop);
            var file = await folder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", Columns));
            foreach (var row in Items)
            {
                sb.AppendLine(string.Join(",", Columns.Select(c => row[c])));
            }
            await Windows.Storage.FileIO.WriteTextAsync(file, sb.ToString());
        }

        //#if WINDOWS
        //        public void PrintGrid()
        //        {
        //            var printDoc = new PrintDocument();
        //            printDoc.Paginate += (s, e) => { /* single page only */ };
        //            printDoc.GetPreviewPage += (s, e) => printDoc.SetPreviewPage(1, this);
        //            printDoc.AddPages += (s, e) =>
        //            {
        //                printDoc.AddPage(this);
        //                printDoc.AddPagesComplete();
        //            };
        //            Microsoft.UI.Xaml.Printing.PrintManager.ShowPrintUIAsync();
        //        }
        //#endif

        #endregion Export/Print
    }
}