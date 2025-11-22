
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FinanceML.UI.Controls.Charting.Models;
using FinanceML.UI.Controls.Charting.Calculators;
using FinanceML.UI.Controls.Charting.Renderers;

namespace FinanceML.UI.Controls
{
    public class PieChartControl : UserControl
    {
        private readonly PieSliceRenderer _sliceRenderer;
        private readonly PieLegendRenderer _legendRenderer;
        private readonly PieSliceCalculator _calculator;

        public IList<PieChartData> Data { get; private set; } = new List<PieChartData>();

        public string Title { get; set; } = string.Empty;
        public bool ShowLegend { get; set; } = true;

        private readonly Font _titleFont = new("Segoe UI", 12, FontStyle.Bold);
        private readonly Font _labelFont = new("Segoe UI", 9);

        public PieChartControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer, true);

            _calculator = new PieSliceCalculator();
            _sliceRenderer = new PieSliceRenderer();
            _legendRenderer = new PieLegendRenderer(_labelFont);

            Size = new Size(320, 260);
        }

        public void SetData(IEnumerable<PieChartData> data)
        {
            Data = data?.ToList() ?? new List<PieChartData>();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!Data.Any(d => d.HasValue))
            {
                DrawEmpty(e.Graphics);
                return;
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            var titleHeight = string.IsNullOrWhiteSpace(Title) ? 0 : 30;
            var legendWidth = ShowLegend ? 120 : 0;

            DrawTitle(e.Graphics);

            var pieArea = new Rectangle(
                6,
                titleHeight + 10,
                rect.Width - legendWidth - 20,
                rect.Height - titleHeight - 20);

            int size = Math.Min(pieArea.Width, pieArea.Height);
            pieArea = new Rectangle(
                pieArea.X + (pieArea.Width - size) / 2,
                pieArea.Y + (pieArea.Height - size) / 2,
                size, size);

            DrawSlices(e.Graphics, pieArea);

            if (ShowLegend)
            {
                var legRect = new Rectangle(rect.Width - legendWidth, titleHeight + 10, legendWidth - 10, rect.Height - titleHeight - 10);
                _legendRenderer.Render(e.Graphics, legRect, Data.ToList());
            }
        }

        private void DrawSlices(Graphics g, Rectangle area)
        {
            float total = _calculator.TotalValue(Data);
            foreach (var (item, start, sweep) in _calculator.CalculateSlices(Data))
            {
                _sliceRenderer.RenderSlice(g, area, item, start, sweep, total, _labelFont);
            }
        }

        private void DrawTitle(Graphics g)
        {
            if (string.IsNullOrWhiteSpace(Title)) return;

            using var brush = new SolidBrush(Color.Black);
            using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            g.DrawString(Title, _titleFont, brush, new Rectangle(0, 5, Width, 25), fmt);
        }

        private void DrawEmpty(Graphics g)
        {
            using var b = new SolidBrush(Color.Gray);
            g.DrawString("No data available.", _labelFont, b, Width / 2 - 50, Height / 2);
        }
    }
}
