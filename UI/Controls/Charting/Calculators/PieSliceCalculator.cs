
namespace FinanceML.UI.Controls.Charting.Calculators
{
    public class PieSliceCalculator
    {
        public float TotalValue(IList<PieChartData> data)
            => (float)data.Where(d => d.HasValue).Sum(d => d.Value);

        public IEnumerable<(PieChartData item, float startAngle, float sweepAngle)> 
            CalculateSlices(IList<PieChartData> data)
        {
            float total = TotalValue(data);
            if (total <= 0) yield break;

            float current = -90f;

            foreach (var item in data.Where(d => d.HasValue))
            {
                float sweep = (float)(item.Value / total * 360f);
                yield return (item, current, sweep);
                current += sweep;
            }
        }
    }
}
