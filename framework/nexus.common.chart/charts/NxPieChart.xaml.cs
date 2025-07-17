using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media.Animation;

using Windows.UI;
using Windows.Foundation;

namespace nexus.common.charts
{
    public sealed partial class NxPieChart : UserControl
    {
        public NxPieChart()
        {
            this.InitializeComponent();
        }

        public void DrawChart(DataTable data, string valueField, string idField)
        {
            if (data.Rows.Count == 0) return;

            PlotArea.Children.Clear();

            double total = data.AsEnumerable().Sum(r => Convert.ToDouble(r[valueField]));
            double angleStart = 0;

            foreach (DataRow row in data.Rows)
            {
                double value = Convert.ToDouble(row[valueField]);
                string id = row[idField].ToString() ?? string.Empty;

                double sweepAngle = (value / total) * 360;
                var path = CreatePieSlice(angleStart, sweepAngle, 100, Colors.CornflowerBlue);

                path.Tag = id;
                path.PointerEntered += (s, e) => OnEnterChart?.Invoke((string)((FrameworkElement)s).Tag);
                path.PointerExited += (s, e) => OnLeaveChart?.Invoke((string)((FrameworkElement)s).Tag);
                path.PointerPressed += (s, e) => OnClickChart?.Invoke((string)((FrameworkElement)s).Tag);

                PlotArea.Children.Add(path);
                angleStart += sweepAngle;
            }
        }

        private Microsoft.UI.Xaml.Shapes.Path CreatePieSlice(double startAngle, double sweepAngle, double radius, Color fillColor)
        {
            double startRad = (Math.PI / 180) * startAngle;
            double endRad = (Math.PI / 180) * (startAngle + sweepAngle);

            Point start = new Point(100 + radius * Math.Cos(startRad), 100 + radius * Math.Sin(startRad));
            Point end   = new Point(100 + radius * Math.Cos(endRad),   100 + radius * Math.Sin(endRad));

            bool isLargeArc = sweepAngle > 180;

            var segment = new ArcSegment
            {
                Point = end,
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc
            };

            var figure = new PathFigure
            {
                StartPoint = new Point(100, 100),
                Segments = { new LineSegment { Point = start }, segment, new LineSegment { Point = new Point(100, 100) } },
                IsClosed = true
            };

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return new Microsoft.UI.Xaml.Shapes.Path
            {
                Fill = new SolidColorBrush(fillColor),
                Data = geometry
            };
        }

        public event Action<string>? OnEnterChart;
        public event Action<string>? OnLeaveChart;
        public event Action<string>? OnClickChart;
    }
}
