
namespace FinanceML.UI.Controls.Charting.Renderers
{
    public class PieLegendRenderer
    {
        private readonly Font _font;

        public PieLegendRenderer(Font font)
        {
            _font = font;
        }

        public void Render(Graphics g, Rectangle rect, IList<PieChartData> data)
        {
            float total = (float)data.Sum(d => d.Value);
            if (total <= 0) return;

            int y = rect.Y;
            const int itemHeight = 22;
            const int colorBoxSize = 12;

            foreach (var item in data.Where(d => d.HasValue))
            {
                var colorRect = new Rectangle(rect.X, y + 4, colorBoxSize, colorBoxSize);
                using (var b = new SolidBrush(item.Color)) g.FillRectangle(b, colorRect);
                using (var p = new Pen(Color.DarkGray)) g.DrawRectangle(p, colorRect);

                string txt = $"{item.Label} ({item.Value / total * 100:F1}%)";
                var textRect = new Rectangle(
                    rect.X + colorBoxSize + 8, y, rect.Width - colorBoxSize - 8, itemHeight);

                using var tb = new SolidBrush(Color.Black);
                g.DrawString(txt, _font, tb, textRect);

                y += itemHeight + 6;
            }
        }
    }
}
