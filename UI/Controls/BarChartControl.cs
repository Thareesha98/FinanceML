using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using FinanceML.UI.Controls.Charting.Models;
using FinanceML.UI.Controls.Charting.Themes;

namespace FinanceML.UI.Controls.Charting.Renderer
{
    public class BarChartRenderer : IChartRenderer
    {
        public bool ShowValues { get; set; } = true;
        public bool ShowGrid { get; set; } = true;
        public bool AnimateBars { get; set; } = true;
        public float AnimationProgress { get; set; } = 1f;

        public void Render(Graphics g, Rectangle bounds, IList<BarChartData> data, BarChartTheme theme)
        {
            if (data.Count == 0)
            {
                DrawEmpty(g, bounds, theme);
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawGrid(g, bounds, theme);
            DrawAxes(g, bounds, theme);
            DrawBars(g, bounds, data, theme);
        }

        private void DrawEmpty(Graphics g, Rectangle rect, BarChartTheme theme)
        {
            using var brush = new SolidBrush(theme.Foreground);
            var msg = "No data";
            var size = g.MeasureString(msg, SystemFonts.DefaultFont);
            g.DrawString(msg, SystemFonts.DefaultFont, brush,
                rect.X + (rect.Width - size.Width) / 2,
                rect.Y + (rect.Height - size.Height) / 2);
        }

        private void DrawGrid(Graphics g, Rectangle rect, BarChartTheme theme)
        {
            if (!ShowGrid) return;

            using var pen = new Pen(theme.GridColor, 1);

            int lines = 5;
            for (int i = 0; i <= lines; i++)
            {
                int y = rect.Y + rect.Height * i / lines;
                g.DrawLine(pen, rect.X, y, rect.Right, y);
            }
        }

        private void DrawAxes(Graphics g, Rectangle rect, BarChartTheme theme)
        {
            using var pen = new Pen(theme.AxisColor, 2);
            g.DrawLine(pen, rect.X, rect.Bottom, rect.Right, rect.Bottom); // X axis
            g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom);          // Y axis
        }

        private void DrawBars(Graphics g, Rectangle rect, IList<BarChartData> data, BarChartTheme theme)
        {
            var max = data.Max(d => d.Value);
            if (max <= 0) return;

            int count = data.Count;
            float sectionWidth = rect.Width / (float)count;

            for (int i = 0; i < count; i++)
            {
                var item = data[i];
                float scaled = (float)(item.Value / max * rect.Height) * AnimationProgress;

                var barHeight = scaled;
                var barX = rect.X + (i * sectionWidth) + (sectionWidth * 0.1f);
                var barY = rect.Bottom - barHeight;
                var barWidth = sectionWidth * 0.8f;

                var barRect = new RectangleF(barX, barY, barWidth, barHeight);

                using var brush = new LinearGradientBrush(
                    barRect,
                    Color.FromArgb(200, item.Color),
                    item.Color,
                    LinearGradientMode.Vertical);

                using var path = RoundedRect(barRect, theme.CornerRadius);

                g.FillPath(brush, path);
                g.DrawPath(Pens.Black, path);

                if (ShowValues)
                {
                    var valueText = item.Value.ToString("N0");
                    var size = g.MeasureString(valueText, SystemFonts.DefaultFont);
                    g.DrawString(
                        valueText,
                        SystemFonts.DefaultFont,
                        Brushes.Black,
                        barX + (barWidth - size.Width) / 2,
                        barY - size.Height - 2);
                }
            }
        }

        private GraphicsPath RoundedRect(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float r = radius * 2;

            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

