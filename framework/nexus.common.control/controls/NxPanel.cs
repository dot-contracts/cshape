using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using nexus.common.control.Themes;

namespace nexus.common.control
{
    public sealed partial class NxPanel : Panel
    {
        public ObservableCollection<RowDefinition> RowDefinitions { get; } = new();
        public ObservableCollection<ColumnDefinition> ColumnDefinitions { get; } = new();

        public NxPanel()
        {
            RowDefinitions.CollectionChanged    += (_, __) => InvalidateMeasure();
            ColumnDefinitions.CollectionChanged += (_, __) => InvalidateMeasure();

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();

        }
        private void ApplyThemeDefaults()
        {
            this.Background = NxThemeManager.Current.PanelBack;
        }

        protected override WinSize MeasureOverride(WinSize availableSize)
        {
            EnsureDefinitions();

            double[] columnWidths = GetColumnWidths(availableSize.Width);
            double[] rowHeights = GetRowHeights(availableSize.Height);

            foreach (UIElement child in Children)
            {
                if (child is FrameworkElement fe)
                {
                    int row = Grid.GetRow(fe);
                    int col = Grid.GetColumn(fe);
                    int rowSpan = Math.Max(Grid.GetRowSpan(fe), 1);
                    int colSpan = Math.Max(Grid.GetColumnSpan(fe), 1);

                    double width = Sum(columnWidths, col, colSpan);
                    double height = Sum(rowHeights, row, rowSpan);

                    child.Measure(new WinSize(width, height));
                }
            }

            return availableSize;
        }

        protected override WinSize ArrangeOverride(WinSize finalSize)
        {
            double[] columnWidths = GetColumnWidths(finalSize.Width);
            double[] columnOffsets = GetOffsets(columnWidths);
            double[] rowHeights = GetRowHeights(finalSize.Height);
            double[] rowOffsets = GetOffsets(rowHeights);

            foreach (UIElement child in Children)
            {
                if (child is FrameworkElement fe)
                {
                    int row = Grid.GetRow(fe);
                    int col = Grid.GetColumn(fe);
                    int rowSpan = Math.Max(Grid.GetRowSpan(fe), 1);
                    int colSpan = Math.Max(Grid.GetColumnSpan(fe), 1);

                    double x = columnOffsets[col];
                    double y = rowOffsets[row];
                    double width = Sum(columnWidths, col, colSpan);
                    double height = Sum(rowHeights, row, rowSpan);

                    child.Arrange(new WinRect(x, y, width, height));
                }
            }

            return finalSize;
        }

        private void EnsureDefinitions()
        {
            if (RowDefinitions.Count == 0)
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            if (ColumnDefinitions.Count == 0)
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        private double[] GetColumnWidths(double totalWidth)
        {
            double[] widths = new double[ColumnDefinitions.Count];
            double totalStar = 0;
            double usedFixed = 0;

            for (int i = 0; i < ColumnDefinitions.Count; i++)
            {
                var def = ColumnDefinitions[i];
                if (def.Width.IsAbsolute)
                {
                    widths[i] = def.Width.Value;
                    usedFixed += widths[i];
                }
                else if (def.Width.IsStar)
                {
                    totalStar += def.Width.Value;
                }
                else // Auto
                {
                    widths[i] = 0;
                }
            }

            double remaining = Math.Max(totalWidth - usedFixed, 0);
            for (int i = 0; i < ColumnDefinitions.Count; i++)
            {
                var def = ColumnDefinitions[i];
                if (def.Width.IsStar && totalStar > 0)
                {
                    widths[i] = (def.Width.Value / totalStar) * remaining;
                }
            }

            return widths;
        }

        private double[] GetRowHeights(double totalHeight)
        {
            double[] heights = new double[RowDefinitions.Count];
            double totalStar = 0;
            double usedFixed = 0;

            for (int i = 0; i < RowDefinitions.Count; i++)
            {
                var def = RowDefinitions[i];
                if (def.Height.IsAbsolute)
                {
                    heights[i] = def.Height.Value;
                    usedFixed += heights[i];
                }
                else if (def.Height.IsStar)
                {
                    totalStar += def.Height.Value;
                }
                else // Auto
                {
                    heights[i] = 0;
                }
            }

            double remaining = Math.Max(totalHeight - usedFixed, 0);
            for (int i = 0; i < RowDefinitions.Count; i++)
            {
                var def = RowDefinitions[i];
                if (def.Height.IsStar && totalStar > 0)
                {
                    heights[i] = (def.Height.Value / totalStar) * remaining;
                }
            }

            return heights;
        }

        private static double[] GetOffsets(double[] sizes)
        {
            double[] offsets = new double[sizes.Length];
            double acc = 0;
            for (int i = 0; i < sizes.Length; i++)
            {
                offsets[i] = acc;
                acc += sizes[i];
            }
            return offsets;
        }

        private static double Sum(double[] values, int start, int count)
        {
            double total = 0;
            for (int i = start; i < start + count && i < values.Length; i++)
                total += values[i];
            return total;
        }
    }
}