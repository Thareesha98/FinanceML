
namespace FinanceML.UI.Controls.Charting.Helpers
{
    public static class ColorUtils
    {
        public static Color GetContrast(Color baseColor)
        {
            var luminance =
                (0.299 * baseColor.R +
                 0.587 * baseColor.G +
                 0.114 * baseColor.B) / 255;

            return luminance > 0.5 ? Color.Black : Color.White;
        }
    }
}
