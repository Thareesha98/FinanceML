using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace FinanceML.UI.Controls
{
    public class PieChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public Color Color { get; set; }

        public PieChartData() { }

        public PieChartData(string label, decimal value, Color color)
        {
            Label = label;
            Value = value;
            Color = color;
        }
    }

    public class PieChartControl : UserControl
    {
        private List<PieChartData> _data = new();
        private string _title = string.Empty;
        private Font _titleFont = new("Segoe UI", 12F, FontStyle.Bold);
        private Font _labelFont = new("Segoe UI", 9F);
        private bool _showLabels = true;
        private bool _showLegend = true;
        private bool _showPercentages = true;

        public List<PieChartData> Data
        {
            get => _data;
            set
            {
                _data = value ?? new List<PieChartData>();
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

        public bool ShowLabels
        {
            get => _showLabels;
            set
            {
                _showLabels = value;
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

        public bool ShowPercentages
        {
            get => _showPercentages;
            set
            {
                _showPercentages = value;
                Invalidate();
            }
        }

        public PieChartControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            BackColor = Color.Transparent;
            Size = new Size(300, 250);
        }

        protected override void OnPaint(PaintEventArgs e)
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
            var legendWidth = _showLegend ? 120 : 0;

            // Draw title
            if (!string.IsNullOrEmpty(_title))
            {
                DrawTitle(e.Graphics, rect);
            }

            // Calculate pie chart area
            var pieRect = new Rectangle(
                10,
                titleHeight + 10,
                rect.Width - legendWidth - 30,
                rect.Height - titleHeight - 20
            );

            // Make it square
            var size = Math.Min(pieRect.Width, pieRect.Height);
            pieRect = new Rectangle(
                pieRect.X + (pieRect.Width - size) / 2,
                pieRect.Y + (pieRect.Height - size) / 2,
                size,
                size
            );

            // Draw pie chart
            DrawPieChart(e.Graphics, pieRect);

            // Draw legend
            if (_showLegend)
            {
                var legendRect = new Rectangle(
                    rect.Width - legendWidth,
                    titleHeight + 10,
                    legendWidth - 10,
                    rect.Height - titleHeight - 20
                );
                DrawLegend(e.Graphics, legendRect);
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

        private void DrawPieChart(Graphics g, Rectangle rect)
        {
            var total = _data.Sum(d => d.Value);
            if (total <= 0) return;

            var startAngle = -90f; // Start from top

            foreach (var item in _data.Where(d => d.Value > 0))
            {
                var sweepAngle = (float)((item.Value / total) * 360);
                
                // Draw pie slice
                using (var brush = new SolidBrush(item.Color))
                {
                    g.FillPie(brush, rect, startAngle, sweepAngle);
                }

                // Draw border
                using (var pen = new Pen(Color.White, 2))
                {
                    g.DrawPie(pen, rect, startAngle, sweepAngle);
                }

                // Draw label if enabled
                if (_showLabels && sweepAngle > 15) // Only show label if slice is large enough
                {
                    DrawSliceLabel(g, rect, startAngle, sweepAngle, item, total);
                }

                startAngle += sweepAngle;
            }
        }

        private void DrawSliceLabel(Graphics g, Rectangle rect, float startAngle, float sweepAngle, 
                                   PieChartData item, decimal total)
        {
            var midAngle = startAngle + sweepAngle / 2;
            var radians = midAngle * Math.PI / 180;
            
            var centerX = rect.X + rect.Width / 2f;
            var centerY = rect.Y + rect.Height / 2f;
            var radius = Math.Min(rect.Width, rect.Height) / 2f * 0.7f;
            
            var labelX = centerX + (float)(Math.Cos(radians) * radius);
            var labelY = centerY + (float)(Math.Sin(radians) * radius);

            var percentage = (item.Value / total) * 100;
            var labelText = _showPercentages ? $"{percentage:F1}%" : item.Label;

            var textColor = GetContrastColor(item.Color);
            using (var brush = new SolidBrush(textColor))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(labelText, _labelFont, brush, labelX, labelY, format);
            }
        }

        private void DrawLegend(Graphics g, Rectangle rect)
        {
            var total = _data.Sum(d => d.Value);
            if (total <= 0) return;

            var y = rect.Y;
            var itemHeight = 24;
            var colorBoxSize = 12;

            foreach (var item in _data.Where(d => d.Value > 0))
            {
                if (y + itemHeight > rect.Bottom) break;

                // Draw color box
                var colorRect = new Rectangle(rect.X, y + 4, colorBoxSize, colorBoxSize);
                using (var brush = new SolidBrush(item.Color))
                {
                    g.FillRectangle(brush, colorRect);
                }
                using (var pen = new Pen(Color.Gray))
                {
                    g.DrawRectangle(pen, colorRect);
                }

                // Draw label and value
                var percentage = (item.Value / total) * 100;
                var text = $"{item.Label} ({percentage:F1}%)";
                var textRect = new Rectangle(rect.X + colorBoxSize + 5, y, rect.Width - colorBoxSize - 5, itemHeight);
                
                using (var brush = new SolidBrush(Color.FromArgb(31, 41, 55)))
                using (var format = new StringFormat { LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(text, _labelFont, brush, textRect, format);
                }

                y += itemHeight + 10;
            }
        }

        private Color GetContrastColor(Color backgroundColor)
        {
            // Calculate luminance
            var luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
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
