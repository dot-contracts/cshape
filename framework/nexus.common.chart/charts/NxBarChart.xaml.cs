
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Composition;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI;
using Microsoft.UI.Text;

namespace nexus.common.charts
{
    public sealed partial class NxBarChart : UserControl
    {
        private string _valueField = string.Empty;
        private string _idField = string.Empty;
        public string ChartTitle { get; set; } = "Bar Chart";

        public event Action<string>? OnEnterChart;
        public event Action<string>? OnLeaveChart;
        public event Action<string>? OnClickChart;

        public NxBarChart()
        {
            this.InitializeComponent();
        }

        public void DrawChart(DataTable data, string valueField, string idField, double height, double width)
        {
            if (data.Rows.Count == 0) return;

            PlotArea.Children.Clear();

            _valueField = valueField;
            _idField = idField;

            double minValue = data.AsEnumerable().Min(row => Convert.ToDouble(row[valueField]));
            double maxValue = data.AsEnumerable().Max(row => Convert.ToDouble(row[valueField]));

            if (maxValue == minValue)
                maxValue = minValue + 1;

            double yAxisWidth = 40;
            double labelAreaHeight = 40;
            double titleAreaHeight = 40;
            double plotHeight = height - labelAreaHeight - titleAreaHeight;
            double plotWidth = width - yAxisWidth;
            double plotInnerHeight = plotHeight - 8;

            double scaleY = plotInnerHeight / (maxValue - minValue);

            int count = data.Rows.Count;
            double barWidth = plotWidth / count * 0.6;
            double gap = (plotWidth - (barWidth * count)) / (count + 1);

            bool showEveryOtherLabel = (barWidth + gap) < 40;

            // Add chart title
            var titleBlock = new TextBlock
            {
                Text = ChartTitle,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                Width = width
            };
            Canvas.SetLeft(titleBlock, 0);
            Canvas.SetTop(titleBlock, 0);
            PlotArea.Children.Add(titleBlock);

            double offsetY = titleAreaHeight;

            // Y-axis ticks
            int ticks = 5;
            for (int i = 0; i <= ticks; i++)
            {
                double v = minValue + i * (maxValue - minValue) / ticks;
                double y = offsetY + plotInnerHeight - (v - minValue) * scaleY;

                var line = new Line
                {
                    X1 = yAxisWidth,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = new SolidColorBrush(Colors.LightGray),
                    StrokeThickness = 1
                };

                var label = new TextBlock
                {
                    Text = $"{v:N0}",
                    FontSize = 10
                };
                Canvas.SetLeft(label, 0);
                Canvas.SetTop(label, y - 8);

                PlotArea.Children.Add(line);
                PlotArea.Children.Add(label);
            }

            // Bars and X-axis labels
            for (int i = 0; i < count; i++)
            {
                double value = Convert.ToDouble(data.Rows[i][valueField]);
                string labelText = data.Rows[i][idField]?.ToString() ?? "";

                double heightVal = (value - minValue) * scaleY;
                if (heightVal <= 0) heightVal = 1;

                double x = yAxisWidth + gap + i * (barWidth + gap);
                double baseY = offsetY + plotInnerHeight;

                SolidColorBrush fill = value >= maxValue * 0.9
                    ? new SolidColorBrush(Colors.Red)
                    : value >= maxValue * 0.7
                        ? new SolidColorBrush(Colors.Orange)
                        : new SolidColorBrush(Colors.SteelBlue);

                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = 0,
                    Fill = fill,
                    Tag = labelText
                };

                ToolTipService.SetToolTip(rect, $"{labelText}: {value:N2}");

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, baseY);

                PlotArea.Children.Add(rect);

                // Animate manually
                var animationStart = DateTime.Now + TimeSpan.FromMilliseconds(i * 30);
                var animationDuration = TimeSpan.FromMilliseconds(200);
                double finalHeight = heightVal;
                bool done = false;

                void OnFrame(object? sender, object e)
                {
                    var now = DateTime.Now;
                    if (now < animationStart)
                        return;

                    var t = Math.Min(1.0, (now - animationStart).TotalMilliseconds / animationDuration.TotalMilliseconds);
                    t = 1 - Math.Pow(1 - t, 3); // ease out cubic

                    var currentHeight = finalHeight * t;
                    rect.Height = currentHeight;
                    Canvas.SetTop(rect, baseY - currentHeight);

                    if (t >= 1.0 && !done)
                    {
                        done = true;
                        Microsoft.UI.Xaml.Media.CompositionTarget.Rendering -= OnFrame;
                    }
                }

                Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += OnFrame;

                rect.PointerEntered += (s, e) =>
                {
                    foreach (var child in PlotArea.Children.OfType<Rectangle>())
                        child.Opacity = child == s ? 1.0 : 0.4;
                    OnEnterChart?.Invoke((string)((FrameworkElement)s).Tag);
                };

                rect.PointerExited += (s, e) =>
                {
                    foreach (var child in PlotArea.Children.OfType<Rectangle>())
                        child.Opacity = 1.0;
                    OnLeaveChart?.Invoke((string)((FrameworkElement)s).Tag);
                };

                rect.PointerPressed += (s, e) => OnClickChart?.Invoke((string)((FrameworkElement)s).Tag);

                if (!showEveryOtherLabel || i % 2 == 0)
                {
                    var text = new TextBlock
                    {
                        Text = labelText,
                        FontSize = 10,
                        TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                        TextWrapping = TextWrapping.NoWrap,
                        Width = barWidth + 4,
                        RenderTransform = new RotateTransform { Angle = -30 },
                        RenderTransformOrigin = new Point(0, 0.5)
                    };

                    Canvas.SetLeft(text, x - 2);
                    Canvas.SetTop(text, offsetY + plotInnerHeight + 5);
                    PlotArea.Children.Add(text);
                }
            }
        }
    }
}
