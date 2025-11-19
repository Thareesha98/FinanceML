using System.Drawing;

namespace FinanceML.UI.Controls.Charting.Models
{
    /// <summary>
    /// Represents a single bar in the bar chart.
    /// Extended version with value formatting, optional secondary value, and tooltip text.
    /// </summary>
    public class BarChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal? SecondaryValue { get; set; }
        public Color Color { get; set; } = Color.SteelBlue;
        public string? TooltipText { get; set; }

        public BarChartData() { }

        public BarChartData(string label, decimal value, Color color)
        {
            Label = label;
            Value = value;
            Color = color;
        }

        public override string ToString()
            => $"{Label}: {Value:N2}";
    }
}

