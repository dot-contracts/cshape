using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Microsoft.UI;

namespace nexus.common.charts
{
    public partial class NxLineChart : UserControl
    {
        private string _valueField = string.Empty;
        private string _idField = string.Empty;

        private double _plotHeight;
        private double _plotWidth;
        private double _plotBorder;
        private double _scaleY;

        private double _maxValue;
        private double _minValue;

        private readonly Random _random = new Random();
        private double[] _randomMS;
        private double[] _yValues;
        private double[] _xValues;

        public event Action<string>? OnEnterChart;
        public event Action<string>? OnLeaveChart;
        public event Action<string>? OnClickChart;


        public NxLineChart()
        {
            this.InitializeComponent();
        }

        public void DrawChart(DataTable data, string valueField, string idField, double height, double width)
        {
            if (data.Rows.Count == 0) return;

            _valueField = valueField;
            _idField = idField;

            _minValue = data.AsEnumerable().Min(row => Convert.ToDouble(row[valueField]));
            _maxValue = data.AsEnumerable().Max(row => Convert.ToDouble(row[valueField]));

            _plotHeight = height;
            _plotWidth = width;
            _plotBorder = 4;
            double plotInnerHeight = _plotHeight - 2 * _plotBorder;

            _scaleY = plotInnerHeight / (_maxValue - _minValue);

            int count = data.Rows.Count;
            double bandWidth = (_plotWidth - _plotBorder * 2) / (count + 1);
            double barWidth = (_plotWidth - _plotBorder * 2) / count;
            double bandLeft = _plotBorder + (barWidth - bandWidth) / 2;

            _randomMS = new double[count];
            _yValues = new double[count];
            _xValues = new double[count];

            for (int i = 0; i < count; i++)
            {
                _randomMS[i] = _random.Next(300, 900);
                double value = Convert.ToDouble(data.Rows[i][valueField]);
                _yValues[i] = plotInnerHeight - ((value - _minValue) * _scaleY);
                if (_yValues[i] < 4) _yValues[i] = 4;
                if (_yValues[i] > plotInnerHeight) _yValues[i] = plotInnerHeight;
                _xValues[i] = i == 0 ? bandLeft : _xValues[i - 1] + barWidth;
            }

            PlotArea.Children.Clear();
            PlotArea.Resources.Clear();

            CreatePath(data);
            CreateLine(data);
        }

        private void CreateLine(DataTable data)
        {
            for (int i = 0; i < data.Rows.Count - 1; i++)
            {
                var line = new Line
                {
                    X1 = _xValues[i],
                    Y1 = _yValues[i],
                    X2 = _xValues[i + 1],
                    Y2 = _yValues[i + 1],
                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(Colors.Blue)
                };
                PlotArea.Children.Add(line);
            }
        }

        private void CreatePath(DataTable data)
        {
            var figure = new PathFigure
            {
                StartPoint = new Point(_xValues[0], _plotHeight - _plotBorder)
            };

            for (int i = 0; i < data.Rows.Count; i++)
            {
                figure.Segments.Add(new LineSegment { Point = new Point(_xValues[i], _yValues[i]) });
            }

            figure.Segments.Add(new LineSegment { Point = new Point(_xValues[^1], _plotHeight - _plotBorder) });
            figure.Segments.Add(new LineSegment { Point = new Point(_xValues[0], _plotHeight - _plotBorder) });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            var path = new Microsoft.UI.Xaml.Shapes.Path
            {
                Fill = new SolidColorBrush(Colors.LightBlue),
                Stroke = new SolidColorBrush(Colors.LightBlue),
                Data = geometry
            };

            PlotArea.Children.Add(path);
        }
    }
}
