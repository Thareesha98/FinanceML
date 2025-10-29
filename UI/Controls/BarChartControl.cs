using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace FinanceML.UI.Controls
{
    public class BarChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public Color Color { get; set; }

        public BarChartData() { }

        public BarChartData(string label, decimal value, Color color)
        {
            Label = label;
            Value = value;
            Color = color;
        }
    }

    public class BarChartControl : UserControl
    {
        private List<BarChartData> _data = new();
        private string _title = string.Empty;
        private Font _titleFont = new("Segoe UI", 12F, FontStyle.Bold);
        private Font _labelFont = new("Segoe UI", 9F);
        private bool _showValues = true;
        private bool _showGrid = true;

        public List<BarChartData> Data
        {
            get => _data;
            set
            {
                _data = value ?? new List<BarChartData>();
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

        public bool ShowValues
        {
            get => _showValues;
            set
            {
                _showValues = value;
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

        public BarChartControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            BackColor = Color.Transparent;
            Size = new Size(400, 200);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                if (_data == null || !_data.Any() || _data.All(d => d.Value <= 0))
                {
                    DrawEmptyState(e.Graphics);
                    return;
                }

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = ClientRectangle;
            var titleHeight = !string.IsNullOrEmpty(_title) ? 30 : 0;
            var bottomMargin = 40; // Space for labels
            var leftMargin = 50; // Space for values
            var rightMargin = 20;
            var topMargin = titleHeight + 10;

            // Draw title
            if (!string.IsNullOrEmpty(_title))
            {
                DrawTitle(e.Graphics, rect);
            }

            // Calculate chart area
            var chartRect = new Rectangle(
                leftMargin,
                topMargin,
                rect.Width - leftMargin - rightMargin,
                rect.Height - topMargin - bottomMargin
            );

            // Draw grid if enabled
            if (_showGrid)
            {
                DrawGrid(e.Graphics, chartRect);
            }

            // Draw bars
            DrawBars(e.Graphics, chartRect);

            // Draw labels
            DrawLabels(e.Graphics, chartRect, bottomMargin);
            }
            catch (Exception ex)
            {
                // Handle any drawing errors gracefully
                DrawErrorState(e.Graphics, ex.Message);
            }
        }

        private void DrawEmptyState(Graphics g)
        {
            var rect = ClientRectangle;
            var message = "No data to display";
            var font = new Font("Segoe UI", 10F);
            var brush = new SolidBrush(Color.Gray);
            
            var textSize = g.MeasureString(message, font);
            var x = (rect.Width - textSize.Width) / 2;
            var y = (rect.Height - textSize.Height) / 2;
            
            g.DrawString(message, font, brush, x, y);
            
            font.Dispose();
            brush.Dispose();
        }

        private void DrawErrorState(Graphics g, string errorMessage)
        {
            var rect = ClientRectangle;
            var message = "Chart error occurred";
            var font = new Font("Segoe UI", 10F);
            var brush = new SolidBrush(Color.Red);
            
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

        private void DrawGrid(Graphics g, Rectangle chartRect)
        {
            var pen = new Pen(Color.FromArgb(230, 230, 230), 1);
            
            // Draw horizontal grid lines
            var gridLines = 5;
            for (int i = 0; i <= gridLines; i++)
            {
                var y = chartRect.Y + (chartRect.Height * i / gridLines);
                g.DrawLine(pen, chartRect.X, y, chartRect.Right, y);
            }

            pen.Dispose();
        }

        private void DrawAxes(Graphics g, Rectangle chartRect, decimal maxValue)
        {
            var axisPen = new Pen(Color.FromArgb(100, 100, 100), 2);
            var textBrush = new SolidBrush(Color.FromArgb(31, 41, 55));

            // Draw X-axis (bottom line)
            g.DrawLine(axisPen, chartRect.X, chartRect.Bottom, chartRect.Right, chartRect.Bottom);

            // Draw Y-axis (left line)
            g.DrawLine(axisPen, chartRect.X, chartRect.Y, chartRect.X, chartRect.Bottom);

            // Draw Y-axis labels (values)
            var ySteps = 5;
            for (int i = 0; i <= ySteps; i++)
            {
                var value = maxValue * i / ySteps;
                var y = chartRect.Bottom - (chartRect.Height * i / ySteps);
                var valueText = $"Rs {value:N0}";
                var textSize = g.MeasureString(valueText, _labelFont);
                
                // Draw tick mark
                g.DrawLine(axisPen, chartRect.X - 5, y, chartRect.X, y);
                
                // Draw label
                g.DrawString(valueText, _labelFont, textBrush, 
                    chartRect.X - textSize.Width - 10, y - textSize.Height / 2);
            }

            // Draw axis labels
            var yAxisLabel = "Amount (Rs)";
            var xAxisLabel = "Months";
            
            // Y-axis label (rotated)
            var yLabelSize = g.MeasureString(yAxisLabel, _labelFont);
            var matrix = g.Transform;
            g.TranslateTransform(15, chartRect.Y + chartRect.Height / 2 + yLabelSize.Width / 2);
            g.RotateTransform(-90);
            g.DrawString(yAxisLabel, _labelFont, textBrush, 0, 0);
            g.Transform = matrix;

            // X-axis label
            var xLabelSize = g.MeasureString(xAxisLabel, _labelFont);
            g.DrawString(xAxisLabel, _labelFont, textBrush, 
                chartRect.X + (chartRect.Width - xLabelSize.Width) / 2, 
                chartRect.Bottom + 25);

            axisPen.Dispose();
            textBrush.Dispose();
        }

        private void DrawBars(Graphics g, Rectangle chartRect)
        {
            if (!_data.Any()) return;

            var maxValue = _data.Max(d => d.Value);
            if (maxValue <= 0) return;

            // Draw axes
            DrawAxes(g, chartRect, maxValue);

            var barWidth = chartRect.Width / _data.Count * 0.8f;
            var barSpacing = chartRect.Width / _data.Count * 0.2f;

            for (int i = 0; i < _data.Count; i++)
            {
                var item = _data[i];
                if (item.Value <= 0) continue; // Skip zero or negative values
                
                var barHeight = (float)(item.Value / maxValue * chartRect.Height);
                if (barHeight < 0.1f) barHeight = 0.1f; // Minimum bar height for visibility
                
                var barX = chartRect.X + (i * chartRect.Width / _data.Count) + barSpacing / 2;
                var barY = chartRect.Bottom - barHeight;

                // Ensure valid dimensions
                if (barWidth <= 0 || barHeight <= 0) continue;

                var barRect = new RectangleF(barX, barY, barWidth, barHeight);

                // Draw bar with gradient (only if bar has sufficient height)
                if (barHeight > 1)
                {
                    using (var brush = new LinearGradientBrush(
                        new PointF(barX, barY),
                        new PointF(barX, barY + barHeight),
                        Color.FromArgb(200, item.Color),
                        item.Color))
                    {
                        g.FillRectangle(brush, barRect);
                    }
                }
                else
                {
                    // For very small bars, use solid color
                    using (var brush = new SolidBrush(item.Color))
                    {
                        g.FillRectangle(brush, barRect);
                    }
                }

                // Draw bar border
                using (var pen = new Pen(Color.FromArgb(100, item.Color), 1))
                {
                    g.DrawRectangle(pen, barRect.X, barRect.Y, barRect.Width, barRect.Height);
                }

                // Draw value on top of bar if enabled
                if (_showValues)
                {
                    var valueText = $"Rs {item.Value:N0}";
                    var textSize = g.MeasureString(valueText, _labelFont);
                    var textX = barX + (barWidth - textSize.Width) / 2;
                    var textY = barY - textSize.Height - 5;

                    using (var brush = new SolidBrush(Color.FromArgb(31, 41, 55)))
                    {
                        g.DrawString(valueText, _labelFont, brush, textX, textY);
                    }
                }
            }
        }

        private void DrawLabels(Graphics g, Rectangle chartRect, int bottomMargin)
        {
            if (!_data.Any()) return;

            var brush = new SolidBrush(Color.FromArgb(31, 41, 55));
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            for (int i = 0; i < _data.Count; i++)
            {
                var item = _data[i];
                var labelX = chartRect.X + (i * chartRect.Width / _data.Count) + (chartRect.Width / _data.Count / 2);
                var labelY = chartRect.Bottom + 10;
                var labelRect = new Rectangle((int)labelX - 50, (int)labelY, 100, bottomMargin - 10);

                g.DrawString(item.Label, _labelFont, brush, labelRect, format);
            }

            brush.Dispose();
            format.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _titleFont?.Dispose();
                _labelFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}