using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using FinanceML.AI;

namespace FinanceML.UI.Controls
{
    public class ForecastGraphControl : UserControl
    {
        private List<ExpenseForecastService.ForecastData> _data = new();
        private string _title = "Expense Forecast";
        private Font _titleFont = new("Segoe UI", 12F, FontStyle.Bold);
        private Font _labelFont = new("Segoe UI", 8F);
        private Font _axisFont = new("Segoe UI", 9F);
        private Color _lineColor = Color.FromArgb(59, 130, 246);
        private Color _fillColor = Color.FromArgb(50, 59, 130, 246);
        private Color _gridColor = Color.FromArgb(229, 231, 235);
        private bool _showGrid = true;
        private bool _showConfidenceBand = true;
        private bool _showLegend = false;

        public List<ExpenseForecastService.ForecastData> Data
        {
            get => _data;
            set
            {
                _data = value ?? new List<ExpenseForecastService.ForecastData>();
                Invalidate();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value ?? string.Empty;
                Invalidate();
            }
        }

        public Color LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                Invalidate();
            }
        }

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                _showGrid = value;
                Invalidate();
            }
        }

        public bool ShowConfidenceBand
        {
            get => _showConfidenceBand;
            set
            {
                _showConfidenceBand = value;
                Invalidate();
            }
        }

        public bool ShowLegend
        {
            get => _showLegend;
            set
            {
                _showLegend = value;
                Invalidate();
            }
        }

        public ForecastGraphControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            BackColor = Color.White;
            Size = new Size(400, 300);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_data == null || !_data.Any())
            {
                DrawEmptyState(e.Graphics);
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = ClientRectangle;
            var titleHeight = !string.IsNullOrEmpty(_title) ? 30 : 0;
            var legendHeight = _showLegend ? 25 : 0;
            var margin = 40;

            // Draw title
            if (!string.IsNullOrEmpty(_title))
            {
                DrawTitle(e.Graphics, rect);
            }

            // Calculate chart area
            var chartRect = new Rectangle(
                margin,
                titleHeight + 10,
                rect.Width - margin * 2,
                rect.Height - titleHeight - margin - legendHeight - 10
            );

            // Draw chart
            DrawChart(e.Graphics, chartRect);

            // Draw legend
            if (_showLegend)
            {
                DrawLegend(e.Graphics, new Rectangle(0, rect.Height - legendHeight - 5, rect.Width, legendHeight));
            }
        }

        private void DrawEmptyState(Graphics g)
        {
            var rect = ClientRectangle;
            var message = "No forecast data available";
            var font = new Font("Segoe UI", 10F);
            var brush = new SolidBrush(Color.Gray);
            
            var textSize = g.MeasureString(message, font);
            var x = (rect.Width - textSize.Width) / 2;
            var y = (rect.Height - textSize.Height) / 2;
            
            g.DrawString(message, font, brush, x, y);
            
            font.Dispose();
            brush.Dispose();
        }

        private void DrawTitle(Graphics g, Rectangle rect)
        {
            var brush = new SolidBrush(Color.FromArgb(31, 41, 55));
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var titleRect = new Rectangle(0, 5, rect.Width, 25);
            g.DrawString(_title, _titleFont, brush, titleRect, format);

            brush.Dispose();
            format.Dispose();
        }

        private void DrawChart(Graphics g, Rectangle chartRect)
        {
            if (!_data.Any()) return;

            var sortedData = _data.OrderBy(d => d.Month).ToList();
            var minValue = 0m;
            var maxValue = sortedData.Max(d => d.PredictedAmount) * 1.1m; // Add 10% padding

            // Draw background
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, chartRect);
            }

            // Draw grid
            if (_showGrid)
            {
                DrawGrid(g, chartRect, minValue, maxValue, sortedData);
            }

            // Draw confidence band
            if (_showConfidenceBand)
            {
                DrawConfidenceBand(g, chartRect, sortedData, minValue, maxValue);
            }

            // Draw main line
            DrawForecastLine(g, chartRect, sortedData, minValue, maxValue);

            // Draw data points
            DrawDataPoints(g, chartRect, sortedData, minValue, maxValue);

            // Draw axes
            DrawAxes(g, chartRect, minValue, maxValue, sortedData);
        }

        private void DrawGrid(Graphics g, Rectangle chartRect, decimal minValue, decimal maxValue, 
                             List<ExpenseForecastService.ForecastData> sortedData)
        {
            using (var pen = new Pen(_gridColor, 1))
            {
                // Horizontal grid lines
                var steps = 5;
                for (int i = 0; i <= steps; i++)
                {
                    var value = minValue + (maxValue - minValue) * i / steps;
                    var y = chartRect.Bottom - (int)((value - minValue) / (maxValue - minValue) * chartRect.Height);
                    
                    g.DrawLine(pen, chartRect.Left, y, chartRect.Right, y);
                }

                // Vertical grid lines
                if (sortedData.Count > 1)
                {
                    for (int i = 0; i < sortedData.Count; i++)
                    {
                        var x = chartRect.Left + (int)((double)i / (sortedData.Count - 1) * chartRect.Width);
                        g.DrawLine(pen, x, chartRect.Top, x, chartRect.Bottom);
                    }
                }
            }
        }

        private void DrawConfidenceBand(Graphics g, Rectangle chartRect, 
                                       List<ExpenseForecastService.ForecastData> sortedData, 
                                       decimal minValue, decimal maxValue)
        {
            if (sortedData.Count < 2) return;

            var points = new List<PointF>();
            var lowerPoints = new List<PointF>();

            // Calculate confidence band points
            for (int i = 0; i < sortedData.Count; i++)
            {
                var data = sortedData[i];
                var x = chartRect.Left + (float)((double)i / (sortedData.Count - 1) * chartRect.Width);
                
                var confidenceRange = data.PredictedAmount * (1 - data.ConfidenceScore) * 0.5m;
                var upperValue = data.PredictedAmount + confidenceRange;
                var lowerValue = Math.Max(0, data.PredictedAmount - confidenceRange);
                
                var upperY = chartRect.Bottom - (float)((upperValue - minValue) / (maxValue - minValue) * chartRect.Height);
                var lowerY = chartRect.Bottom - (float)((lowerValue - minValue) / (maxValue - minValue) * chartRect.Height);
                
                points.Add(new PointF(x, upperY));
                lowerPoints.Add(new PointF(x, lowerY));
            }

            // Create confidence band path
            lowerPoints.Reverse();
            var allPoints = points.Concat(lowerPoints).ToArray();

            if (allPoints.Length > 2)
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, _lineColor)))
                {
                    g.FillPolygon(brush, allPoints);
                }
            }
        }

        private void DrawForecastLine(Graphics g, Rectangle chartRect, 
                                     List<ExpenseForecastService.ForecastData> sortedData, 
                                     decimal minValue, decimal maxValue)
        {
            if (sortedData.Count < 2) return;

            var points = new PointF[sortedData.Count];
            
            for (int i = 0; i < sortedData.Count; i++)
            {
                var data = sortedData[i];
                var x = chartRect.Left + (float)((double)i / (sortedData.Count - 1) * chartRect.Width);
                var y = chartRect.Bottom - (float)((data.PredictedAmount - minValue) / (maxValue - minValue) * chartRect.Height);
                
                points[i] = new PointF(x, y);
            }

            using (var pen = new Pen(_lineColor, 3))
            {
                g.DrawLines(pen, points);
            }
        }

        private void DrawDataPoints(Graphics g, Rectangle chartRect, 
                                   List<ExpenseForecastService.ForecastData> sortedData, 
                                   decimal minValue, decimal maxValue)
        {
            var pointSize = 6;
            
            for (int i = 0; i < sortedData.Count; i++)
            {
                var data = sortedData[i];
                var x = chartRect.Left + (float)((double)i / (sortedData.Count - 1) * chartRect.Width);
                var y = chartRect.Bottom - (float)((data.PredictedAmount - minValue) / (maxValue - minValue) * chartRect.Height);
                
                var pointRect = new RectangleF(x - pointSize / 2, y - pointSize / 2, pointSize, pointSize);
                
                // Point color based on confidence
                var alpha = (int)(data.ConfidenceScore * 255);
                var pointColor = Color.FromArgb(alpha, _lineColor);
                
                using (var brush = new SolidBrush(pointColor))
                using (var pen = new Pen(Color.White, 2))
                {
                    g.FillEllipse(brush, pointRect);
                    g.DrawEllipse(pen, pointRect);
                }
            }
        }

        private void DrawAxes(Graphics g, Rectangle chartRect, decimal minValue, decimal maxValue, 
                             List<ExpenseForecastService.ForecastData> sortedData)
        {
            using (var pen = new Pen(Color.FromArgb(107, 114, 128), 1))
            using (var brush = new SolidBrush(Color.FromArgb(107, 114, 128)))
            {
                // Draw axes
                g.DrawLine(pen, chartRect.Left, chartRect.Bottom, chartRect.Right, chartRect.Bottom); // X-axis
                g.DrawLine(pen, chartRect.Left, chartRect.Top, chartRect.Left, chartRect.Bottom); // Y-axis

                // Y-axis labels
                var steps = 5;
                for (int i = 0; i <= steps; i++)
                {
                    var value = minValue + (maxValue - minValue) * i / steps;
                    var y = chartRect.Bottom - (int)((value - minValue) / (maxValue - minValue) * chartRect.Height);
                    var label = $"Rs {value:N0}";
                    
                    var textSize = g.MeasureString(label, _axisFont);
                    g.DrawString(label, _axisFont, brush, chartRect.Left - textSize.Width - 5, y - textSize.Height / 2);
                }

                // X-axis labels
                if (sortedData.Count > 0)
                {
                    var labelCount = Math.Min(6, sortedData.Count);
                    for (int i = 0; i < labelCount; i++)
                    {
                        var dataIndex = (int)((double)i / (labelCount - 1) * (sortedData.Count - 1));
                        var data = sortedData[dataIndex];
                        var x = chartRect.Left + (int)((double)dataIndex / (sortedData.Count - 1) * chartRect.Width);
                        var label = data.Month.ToString("MMM yy");
                        
                        var textSize = g.MeasureString(label, _axisFont);
                        g.DrawString(label, _axisFont, brush, x - textSize.Width / 2, chartRect.Bottom + 5);
                    }
                }
            }
        }

        private void DrawLegend(Graphics g, Rectangle legendRect)
        {
            var legendItems = new List<(string Text, Color Color)>();
            
            // Add main forecast line
            legendItems.Add(("Predicted Amount", _lineColor));
            
            // Add confidence band if shown
            if (_showConfidenceBand)
            {
                legendItems.Add(("Confidence Band", Color.FromArgb(100, _lineColor)));
            }

            if (!legendItems.Any()) return;

            var itemWidth = legendRect.Width / legendItems.Count;
            var legendFont = new Font("Segoe UI", 8F);
            var textBrush = new SolidBrush(Color.FromArgb(107, 114, 128));

            for (int i = 0; i < legendItems.Count; i++)
            {
                var item = legendItems[i];
                var x = legendRect.Left + i * itemWidth + 10;
                var y = legendRect.Top + 5;

                // Draw color indicator
                var colorRect = new Rectangle(x, y + 2, 12, 8);
                using (var brush = new SolidBrush(item.Color))
                {
                    g.FillRectangle(brush, colorRect);
                }

                // Draw text
                g.DrawString(item.Text, legendFont, textBrush, x + 18, y);
            }

            legendFont.Dispose();
            textBrush.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _titleFont?.Dispose();
                _labelFont?.Dispose();
                _axisFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
