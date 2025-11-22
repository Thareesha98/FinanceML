
namespace FinanceML.UI.Controls.Charting.Helpers
{
    public static class GeometryHelper
    {
        public static PointF GetPointOnCircle(PointF center, float radius, float angleDegrees)
        {
            double radians = angleDegrees * Math.PI / 180;
            return new PointF(
                center.X + (float)(Math.Cos(radians) * radius),
                center.Y + (float)(Math.Sin(radians) * radius)
            );
        }
    }
}
