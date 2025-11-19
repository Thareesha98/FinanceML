using System.Drawing;

namespace FinanceML.UI.Controls.Charting.Themes
{
    public class BarChartTheme
    {
        public Color Background { get; set; } = Color.White;
        public Color Foreground { get; set; } = Color.Black;
        public Color AxisColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color GridColor { get; set; } = Color.FromArgb(220, 220, 220);
        public Color TooltipBackground { get; set; } = Color.FromArgb(40, 40, 40);
        public Color TooltipForeground { get; set; } = Color.White;
        public int CornerRadius { get; set; } = 4;
    }

    public static class BarChartThemes
    {
        public static BarChartTheme Light => new BarChartTheme
        {
            Background = Color.White,
            Foreground = Color.Black,
            GridColor = Color.FromArgb(230, 230, 230)
        };

        public static BarChartTheme Dark => new BarChartTheme
        {
            Background = Color.FromArgb(30, 30, 30),
            Foreground = Color.White,
            GridColor = Color.FromArgb(70, 70, 70),
            AxisColor = Color.White
        };

        public static BarChartTheme Ocean => new BarChartTheme
        {
            Background = Color.FromArgb(230, 248, 255),
            Foreground = Color.FromArgb(0, 55, 90),
            GridColor = Color.FromArgb(190, 220, 240)
        };

        public static BarChartTheme Sunset => new BarChartTheme
        {
            Background = Color.FromArgb(255, 245, 230),
            Foreground = Color.FromArgb(120, 40, 10),
            GridColor = Color.FromArgb(255, 220, 180)
        };
    }
}

