using System.Collections.Generic;

namespace FinanceML.UI.Controls.Chart
{
    public static class ChartThemeProvider
    {
        public static readonly ChartTheme Light = new ChartTheme();
        public static readonly ChartTheme Dark = new ChartTheme
        {
            BackgroundColor = Color.FromArgb(33, 37, 41),
            ForegroundColor = Color.White,
            GridColor = Color.FromArgb(70, 70, 70),
            AxisColor = Color.White,
            LabelColor = Color.White,
        };
    }
}

