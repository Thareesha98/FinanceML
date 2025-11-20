using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using FinanceML.UI.Controls.Charting.Models;
using FinanceML.UI.Controls.Charting.Themes;

namespace FinanceML.UI.Controls.Charting.Renderer
{
    /// <summary>
    /// Renders a bar chart using the provided chart theme & dataset.
    /// Designed for high contribution-frequency & long-term maintainability.
    /// </summary>
    public class BarChartRenderer : IChartRenderer
    {
        public bool ShowValues { get; set; } = true;
        public bool ShowGrid { get; set; } = true;
        public bool AnimateBars { get; set; } = true;

        /// <summary>
        /// Range: 0.0 → 1.0  
        /// Used for smooth bar height animation.
        /// </summary>
        public float AnimationProgress { get; set; } = 1f;

        // ------------------------------------------------------------
        // Main Render Entry
        // ------------------------------------------------------------
        public void Render(Graphics g, Rectangle bounds, IList<BarChartData> data, BarChartTheme theme)
        {
            if (g == null || theme == null)
                return;

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            if (data == null || data.Count == 0)
            {
                DrawEmptyState(g, bounds, theme);
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawGrid(g, bounds, theme);
            DrawAxes(g, bounds, theme);
            DrawBars(g, bounds, data, theme);
        }

        // ------------------------------------------------------------
        // Empty State
        // ------------------------------------------------------------
        private void DrawEmptyState(Graphics g, Rectangle rect, BarChartTheme theme)
        {
            const string message = "No data available";

            using var brush = new SolidBrush(theme.Foreground ?? Color.Gray);
            var size = g.MeasureString(message, SystemFonts.DefaultFont);

            float x = rect.X + (rect.Width - size.Width) / 2f;
            float y = rect.Y + (rect.Height - size.Height) / 2f;

            g.DrawString(message, SystemFonts.DefaultFont, brush, x, y);
        }

        // ------------------------------------------------------------
        // Grid Lines
        // ------------------------------------------------------------
        private void DrawGrid(Graphics g, Rectangle rect, BarChartTheme theme)
        {
            if (!ShowGrid)
                return;

            int lineCount = 5;

            using var pen = new Pen(theme.GridColor, 1);

            for (int i = 0; i <= lineCount; i++)
            {
                int y = rect.Y + (rect.Height * i / lineCount);
                g.DrawLine(pen, rect.X, y, rect.Right, y);
            }
        }

        // ------------------------------------------------------------
        // Axes
        // ------------------------------------------------------------
        private void DrawAxes(Graphics g, Rectangle rect, BarChartTheme theme)
        {
            using var pen = new Pen(theme.AxisColor, 2);

            // X-axis
            g.DrawLine(pen, rect.X, rect.Bottom, rect.Right, rect.Bottom);

            // Y-axis
            g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom);
        }

        // ------------------------------------------------------------
        // Bars Rendering
        // ------------------------------------------------------------
        private void DrawBars(Graphics g, Rectangle rect, IList<BarChartData> data, BarChartTheme theme)
        {
            var maxValue = data.Max(d => d.Value);
            if (maxValue <= 0)
                return;

            int count = data.Count;
            float sectionWidth = rect.Width / (float)count;

            for (int i = 0; i < count; i++)
            {
                var item = data[i];
                float normalized = (float)(item.Value / maxValue);
                float height = normalized * rect.Height * AnimationProgress;

                if (height < 0.1f)
                    height = 0.1f;

                float barWidth = sectionWidth * 0.8f;
                float barX = rect.X + (sectionWidth * i) + (sectionWidth * 0.1f);
                float barY = rect.Bottom - height;

                var barRect = new RectangleF(barX, barY, barWidth, height);

                DrawBarFill(g, barRect, item.Color);
                DrawBarBorder(g, barRect, item.Color);

                if (ShowValues)
                    DrawValueLabel(g, item, barRect, theme);
            }
        }

        // ------------------------------------------------------------
        // Individual Bar Fill
        // ------------------------------------------------------------
        private void DrawBarFill(Graphics g, RectangleF barRect, Color color)
        {
            if (barRect.Height <= 1)
            {
                // Too small for gradient
                using var solid = new SolidBrush(color);
                g.FillRectangle(solid, barRect);
                return;
            }

            using var gradient = new LinearGradientBrush(
                barRect,
                Color.FromArgb(200, color),
                color,
                LinearGradientMode.Vertical
            );

            g.FillRectangle(gradient, barRect);
        }

        // ------------------------------------------------------------
        // Individual Bar Border
        // ------------------------------------------------------------
        private void DrawBarBorder(Graphics g, RectangleF barRect, Color color)
        {
            using var pen = new Pen(Color.FromArgb(120, color), 1);
            g.DrawRectangle(pen, barRect.X, barRect.Y, barRect.Width, barRect.Height);
        }

        // ------------------------------------------------------------
        // Value Label
        // ------------------------------------------------------------
        private void DrawValueLabel(Graphics g, BarChartData item, RectangleF barRect, BarChartTheme theme)
        {
            string valueText = item.Value.ToString("N0");

            using var brush = new SolidBrush(theme.Foreground ?? Color.Black);
            var size = g.MeasureString(valueText, SystemFonts.DefaultFont);

            float x = barRect.X + (barRect.Width - size.Width) / 2f;
            float y = barRect.Y - size.Height - 5;

            g.DrawString(valueText, SystemFonts.DefaultFont, brush, x, y);
        }
    }
}

