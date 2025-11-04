using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading; // For Thread.Sleep
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Required for Chart

namespace GitContributionApp.Dashboard
{
    public partial class SalesDashboardForm : Form
    {
        // This class will hold our simulated data
        private class DashboardData
        {
            public DataTable SalesByRegion { get; set; }
            public DataTable RawSalesData { get; set; }
        }

        public SalesDashboardForm()
        {
            InitializeComponent();
        }

        private void SalesDashboardForm_Load(object sender, EventArgs e)
        {
            // Set initial state and start loading data
            SetLoadingState(true);
            bgwDataLoader.RunWorkerAsync(); // Start the background worker
        }

        /// <summary>
        /// Toggles the UI state between loading and idle.
        /// </summary>
        private void SetLoadingState(bool isLoading)
        {
            if (isLoading)
            {
                statusLabel.Text = "Loading data...";
                progressBar.Style = ProgressBarStyle.Marquee;
                btnRefresh.Enabled = false;
                tabControl.Visible = false;
            }
            else
            {
                statusLabel.Text = "Ready";
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = 0;
                btnRefresh.Enabled = true;
                tabControl.Visible = true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Re-load the data
            SetLoadingState(true);
            bgwDataLoader.RunWorkerAsync();
        }

        #region Background Worker (Data Loading)

        /// <summary>
        /// This happens on a separate, background thread.
        /// No UI access is allowed here.
        /// </summary>
        private void bgwDataLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            // Simulate a long database query
            // In a real app, this is where you'd connect to SQL
            var rand = new Random();
            var data = new DashboardData();
            data.SalesByRegion = new DataTable("RegionSales");
            data.RawSalesData = new DataTable("RawSales");

            // --- 1. Create Chart Data (Sales by Region) ---
            data.SalesByRegion.Columns.Add("Region", typeof(string));
            data.SalesByRegion.Columns.Add("TotalSales", typeof(int));

            string[] regions = { "North", "South", "East", "West" };
            foreach (string region in regions)
            {
                // Simulate 50% of the work
                Thread.Sleep(500); // 0.5 second delay per region
                data.SalesByRegion.Rows.Add(region, rand.Next(10000, 50000));
                bgwDataLoader.ReportProgress(25 * (Array.IndexOf(regions, region) + 1));
            }

            // --- 2. Create Grid Data (Raw Sales) ---
            data.RawSalesData.Columns.Add("OrderID", typeof(int));
            data.RawSalesData.Columns.Add("Product", typeof(string));
            data.RawSalesData.Columns.Add("Region", typeof(string));
            data.RawSalesData.Columns.Add("Amount", typeof(decimal));

            string[] products = { "Widget", "Gadget", "Doodad", "Contraption" };
            for (int i = 1; i <= 50; i++)
            {
                // Simulate the other 50% of work
                if (i % 10 == 0)
                {
                    Thread.Sleep(200);
                }
                data.RawSalesData.Rows.Add(
                    1000 + i,
                    products[rand.Next(products.Length)],
                    regions[rand.Next(regions.Length)],
                    rand.Next(100, 2000)
                );
            }
            
            bgwDataLoader.ReportProgress(100); // Signal 100%
            Thread.Sleep(500); // Final short delay

            // Pass the generated data to the RunWorkerCompleted event
            e.Result = data;
        }

        /// <summary>
        /// This happens on the UI thread when bgwDataLoader.ReportProgress is called.
        /// </summary>
        private void bgwDataLoader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update progress bar
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = e.ProgressPercentage;
            statusLabel.Text = $"Loading... {e.ProgressPercentage}%";
        }

        /// <summary>
        /// This happens on the UI thread when the DoWork method completes.
        /// </summary>
        private void bgwDataLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Handle error
                MessageBox.Show("Failed to load data: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error loading data.";
                progressBar.Value = 0;
            }
            else if (e.Cancelled)
            {
                // Handle cancellation
                statusLabel.Text = "Data loading cancelled.";
                progressBar.Value = 0;
            }
            else
            {
                // Success! Bind the data
                DashboardData data = e.Result as DashboardData;
                if (data != null)
                {
                    BindDataToUI(data);
                    statusLabel.Text = "Data loaded successfully.";
                }
            }

            SetLoadingState(false); // Reset UI to idle state
        }

        #endregion

        #region Data Binding

        /// <summary>
        /// Binds the loaded data to the chart and grid controls.
        /// This MUST be called from the UI thread.
        /// </summary>
        private void BindDataToUI(DashboardData data)
        {
            // --- 1. Bind to Chart ---
            try
            {
                Series salesSeries = chartSales.Series["Sales"];
                salesSeries.Points.Clear();
                
                // Set chart type (can be Pie, Column, Bar, etc.)
                salesSeries.ChartType = SeriesChartType.Pie;

                // Tell the chart where to get its data
                salesSeries.XValueMember = "Region";
                salesSeries.YValueMembers = "TotalSales";
                salesSeries.IsValueShownAsLabel = true; // Show values on the pie slices

                chartSales.DataSource = data.SalesByRegion;
                chartSales.DataBind();
                chartSales.Titles["Title1"].Text = "Sales by Region";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chart binding error: " + ex.Message, "Chart Error");
            }
            
            // --- 2. Bind to DataGridView ---
            try
            {
                dgvRawData.DataSource = data.RawSalesData;

                // Apply some formatting
                dgvRawData.Columns["OrderID"].Width = 80;
                dgvRawData.Columns["Product"].Width = 150;
                dgvRawData.Columns["Region"].Width = 100;
                dgvRawData.Columns["Amount"].DefaultCellStyle.Format = "C2"; // Currency
                dgvRawData.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            catch (Exception ex)
            {
                 MessageBox.Show("Grid binding error: " + ex.Message, "Grid Error");
            }
        }

        #endregion
    }
}
