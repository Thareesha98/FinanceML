
namespace FinanceML.UI.Controls.Charting.Models
{
    public class PieChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public Color Color { get; set; }

        public bool HasValue => Value > 0;

        public PieChartData() { }

        public PieChartData(string label, decimal value, Color color)
        {
            Label = label;
            Value = value;
            Color = color;
        }
    }
}
