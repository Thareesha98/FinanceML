
namespace FinanceML.UI.Controls.Charting.Renderers
{
    public class PieSliceRenderer
    {
        public bool ShowLabels { get; set; } = true;
        public bool ShowPercentages { get; set; } = true;

        public void RenderSlice(
            Graphics g,
            Rectangle rect,
            PieChartData item,
            float startAngle,
            float sweepAngle,
            float total,
            Font labelFont)
        {
            using var brush = new SolidBrush(item.Color);
            using var pen = new Pen(Color.White, 2);

            g.FillPie(brush, rect, startAngle, sweepAngle);
            g.DrawPie(pen, rect, startAngle, sweepAngle);

            if (!ShowLabels || sweepAngle < 15) return;

            DrawLabel(g, rect, item, startAngle, sweepAngle, total, labelFont);
        }

        private void DrawLabel(
            Graphics g,
            Rectangle rect,
            PieChartData item,
            float start,
            float sweep,
            float total,
            Font font)
        {
            float angle = start + sweep / 2;
            var center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            float radius = rect.Width * 0.33f;

            var pos = GeometryHelper.GetPointOnCircle(center, radius, angle);

            string text = ShowPercentages
                ? $"{item.Value / total * 100:F1}%"
                : item.Label;

            using var brush = new SolidBrush(ColorUtils.GetContrast(item.Color));
            using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            g.DrawString(text, font, brush, pos, fmt);
        }
    }
}
