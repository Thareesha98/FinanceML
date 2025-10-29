namespace FinanceML
{
    partial class AIInsightsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.insightsPanel = new System.Windows.Forms.Panel();
            this.insightsLabel = new System.Windows.Forms.Label();
            this.insightsListBox = new System.Windows.Forms.ListBox();
            this.recommendationsPanel = new System.Windows.Forms.Panel();
            this.recommendationsLabel = new System.Windows.Forms.Label();
            this.recommendationsListBox = new System.Windows.Forms.ListBox();
            this.savingsPanel = new System.Windows.Forms.Panel();
            this.savingsLabel = new System.Windows.Forms.Label();
            this.savingsListBox = new System.Windows.Forms.ListBox();
            this.healthPanel = new System.Windows.Forms.Panel();
            this.healthLabel = new System.Windows.Forms.Label();
            this.healthScoreLabel = new System.Windows.Forms.Label();
            this.predictionsPanel = new System.Windows.Forms.Panel();
            this.predictionsLabel = new System.Windows.Forms.Label();
            this.predictionsListBox = new System.Windows.Forms.ListBox();
            this.forecastChartTitle = new System.Windows.Forms.Label();
            this.incomeVsExpenseChartTitle = new System.Windows.Forms.Label();
            this.forecastChart = new FinanceML.UI.Controls.ForecastGraphControl();
            this.incomeVsExpenseChart = new FinanceML.UI.Controls.ForecastGraphControl();

            this.headerPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.insightsPanel.SuspendLayout();
            this.recommendationsPanel.SuspendLayout();
            this.savingsPanel.SuspendLayout();
            this.healthPanel.SuspendLayout();
            this.predictionsPanel.SuspendLayout();

            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.refreshButton);
            this.headerPanel.Controls.Add(this.exportButton);
            this.headerPanel.Controls.Add(this.closeButton);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1000, 80);
            this.headerPanel.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(30, 15);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(400, 50);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "🤖 AI Financial Insights";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // refreshButton
            // 
            this.refreshButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.refreshButton.FlatAppearance.BorderSize = 0;
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.refreshButton.ForeColor = System.Drawing.Color.White;
            this.refreshButton.Location = new System.Drawing.Point(650, 25);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(100, 35);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "🔄 Refresh";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.OnRefreshClick);
            // 
            // exportButton
            // 
            this.exportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.exportButton.FlatAppearance.BorderSize = 0;
            this.exportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exportButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.exportButton.ForeColor = System.Drawing.Color.White;
            this.exportButton.Location = new System.Drawing.Point(760, 25);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(100, 35);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "📄 Export";
            this.exportButton.UseVisualStyleBackColor = false;
            this.exportButton.Click += new System.EventHandler(this.OnExportInsightsClick);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.closeButton.ForeColor = System.Drawing.Color.White;
            this.closeButton.Location = new System.Drawing.Point(870, 25);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 35);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "❌ Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.OnCloseClick);
            // 
            // mainPanel
            // 
            this.mainPanel.AutoScroll = true;
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.mainPanel.Controls.Add(this.insightsPanel);
            this.mainPanel.Controls.Add(this.recommendationsPanel);
            this.mainPanel.Controls.Add(this.savingsPanel);
            this.mainPanel.Controls.Add(this.healthPanel);
            this.mainPanel.Controls.Add(this.predictionsPanel);

            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 80);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(20);
            this.mainPanel.Size = new System.Drawing.Size(1000, 1100);
            this.mainPanel.TabIndex = 1;
            // 
            // insightsPanel
            // 
            this.insightsPanel.BackColor = System.Drawing.Color.White;
            this.insightsPanel.Controls.Add(this.insightsLabel);
            this.insightsPanel.Controls.Add(this.insightsListBox);
            this.insightsPanel.Location = new System.Drawing.Point(30, 30);
            this.insightsPanel.Name = "insightsPanel";
            this.insightsPanel.Size = new System.Drawing.Size(450, 280);
            this.insightsPanel.TabIndex = 0;
            // 
            // insightsLabel
            // 
            this.insightsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.insightsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.insightsLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.insightsLabel.ForeColor = System.Drawing.Color.White;
            this.insightsLabel.Location = new System.Drawing.Point(0, 0);
            this.insightsLabel.Name = "insightsLabel";
            this.insightsLabel.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.insightsLabel.Size = new System.Drawing.Size(450, 50);
            this.insightsLabel.TabIndex = 0;
            this.insightsLabel.Text = "💡 Spending Insights";
            // 
            // insightsListBox
            // 
            this.insightsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.insightsListBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.insightsListBox.ItemHeight = 19;
            this.insightsListBox.Location = new System.Drawing.Point(20, 60);
            this.insightsListBox.Name = "insightsListBox";
            this.insightsListBox.Size = new System.Drawing.Size(410, 200);
            this.insightsListBox.TabIndex = 1;
            // 
            // recommendationsPanel
            // 
            this.recommendationsPanel.BackColor = System.Drawing.Color.White;
            this.recommendationsPanel.Controls.Add(this.recommendationsLabel);
            this.recommendationsPanel.Controls.Add(this.recommendationsListBox);
            this.recommendationsPanel.Location = new System.Drawing.Point(500, 30);
            this.recommendationsPanel.Name = "recommendationsPanel";
            this.recommendationsPanel.Size = new System.Drawing.Size(450, 280);
            this.recommendationsPanel.TabIndex = 1;
            // 
            // recommendationsLabel
            // 
            this.recommendationsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.recommendationsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.recommendationsLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.recommendationsLabel.ForeColor = System.Drawing.Color.White;
            this.recommendationsLabel.Location = new System.Drawing.Point(0, 0);
            this.recommendationsLabel.Name = "recommendationsLabel";
            this.recommendationsLabel.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.recommendationsLabel.Size = new System.Drawing.Size(450, 50);
            this.recommendationsLabel.TabIndex = 0;
            this.recommendationsLabel.Text = "📊 Budget Recommendations";
            // 
            // recommendationsListBox
            // 
            this.recommendationsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.recommendationsListBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.recommendationsListBox.ItemHeight = 19;
            this.recommendationsListBox.Location = new System.Drawing.Point(20, 60);
            this.recommendationsListBox.Name = "recommendationsListBox";
            this.recommendationsListBox.Size = new System.Drawing.Size(410, 200);
            this.recommendationsListBox.TabIndex = 1;
            // 
            // savingsPanel
            // 
            this.savingsPanel.BackColor = System.Drawing.Color.White;
            this.savingsPanel.Controls.Add(this.savingsLabel);
            this.savingsPanel.Controls.Add(this.savingsListBox);
            this.savingsPanel.Location = new System.Drawing.Point(30, 330);
            this.savingsPanel.Name = "savingsPanel";
            this.savingsPanel.Size = new System.Drawing.Size(450, 280);
            this.savingsPanel.TabIndex = 2;
            // 
            // savingsLabel
            // 
            this.savingsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.savingsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.savingsLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.savingsLabel.ForeColor = System.Drawing.Color.White;
            this.savingsLabel.Location = new System.Drawing.Point(0, 0);
            this.savingsLabel.Name = "savingsLabel";
            this.savingsLabel.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.savingsLabel.Size = new System.Drawing.Size(450, 50);
            this.savingsLabel.TabIndex = 0;
            this.savingsLabel.Text = "🎯 Savings Goals";
            // 
            // savingsListBox
            // 
            this.savingsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.savingsListBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.savingsListBox.ItemHeight = 19;
            this.savingsListBox.Location = new System.Drawing.Point(20, 60);
            this.savingsListBox.Name = "savingsListBox";
            this.savingsListBox.Size = new System.Drawing.Size(410, 200);
            this.savingsListBox.TabIndex = 1;
            // 
            // healthPanel
            // 
            this.healthPanel.BackColor = System.Drawing.Color.White;
            this.healthPanel.Controls.Add(this.healthLabel);
            this.healthPanel.Controls.Add(this.healthScoreLabel);
            this.healthPanel.Location = new System.Drawing.Point(500, 330);
            this.healthPanel.Name = "healthPanel";
            this.healthPanel.Size = new System.Drawing.Size(450, 130);
            this.healthPanel.TabIndex = 3;
            // 
            // healthLabel
            // 
            this.healthLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(51)))), ((int)(((byte)(234)))));
            this.healthLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.healthLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.healthLabel.ForeColor = System.Drawing.Color.White;
            this.healthLabel.Location = new System.Drawing.Point(0, 0);
            this.healthLabel.Name = "healthLabel";
            this.healthLabel.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.healthLabel.Size = new System.Drawing.Size(450, 50);
            this.healthLabel.TabIndex = 0;
            this.healthLabel.Text = "❤️ Financial Health Score";
            // 
            // healthScoreLabel
            // 
            this.healthScoreLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.healthScoreLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.healthScoreLabel.Location = new System.Drawing.Point(20, 60);
            this.healthScoreLabel.Name = "healthScoreLabel";
            this.healthScoreLabel.Size = new System.Drawing.Size(410, 50);
            this.healthScoreLabel.TabIndex = 1;
            this.healthScoreLabel.Text = "Calculating...";
            this.healthScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // predictionsPanel
            // 
            this.predictionsPanel.BackColor = System.Drawing.Color.White;
            this.predictionsPanel.Controls.Add(this.predictionsLabel);
            this.predictionsPanel.Controls.Add(this.predictionsListBox);
            this.predictionsPanel.Controls.Add(this.forecastChartTitle);
            this.predictionsPanel.Controls.Add(this.incomeVsExpenseChartTitle);
            this.predictionsPanel.Controls.Add(this.forecastChart);
            this.predictionsPanel.Controls.Add(this.incomeVsExpenseChart);
            this.predictionsPanel.Location = new System.Drawing.Point(30, 630);
            this.predictionsPanel.Name = "predictionsPanel";
            this.predictionsPanel.Size = new System.Drawing.Size(920, 450);
            this.predictionsPanel.TabIndex = 4;
            // 
            // predictionsLabel
            // 
            this.predictionsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.predictionsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.predictionsLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.predictionsLabel.ForeColor = System.Drawing.Color.White;
            this.predictionsLabel.Location = new System.Drawing.Point(0, 0);
            this.predictionsLabel.Name = "predictionsLabel";
            this.predictionsLabel.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.predictionsLabel.Size = new System.Drawing.Size(920, 50);
            this.predictionsLabel.TabIndex = 0;
            this.predictionsLabel.Text = "🔮 Spending Predictions";
            // 
            // predictionsListBox
            // 
            this.predictionsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.predictionsListBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.predictionsListBox.ItemHeight = 19;
            this.predictionsListBox.Location = new System.Drawing.Point(20, 60);
            this.predictionsListBox.Name = "predictionsListBox";
            this.predictionsListBox.Size = new System.Drawing.Size(880, 30);
            this.predictionsListBox.TabIndex = 1;
            // 
            // forecastChartTitle
            // 
            this.forecastChartTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.forecastChartTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.forecastChartTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.forecastChartTitle.ForeColor = System.Drawing.Color.White;
            this.forecastChartTitle.Location = new System.Drawing.Point(20, 100);
            this.forecastChartTitle.Name = "forecastChartTitle";
            this.forecastChartTitle.Size = new System.Drawing.Size(430, 30);
            this.forecastChartTitle.TabIndex = 10;
            this.forecastChartTitle.Text = "📈 Expense Forecast - Next 6 Months";
            this.forecastChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // incomeVsExpenseChartTitle
            // 
            this.incomeVsExpenseChartTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.incomeVsExpenseChartTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.incomeVsExpenseChartTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.incomeVsExpenseChartTitle.ForeColor = System.Drawing.Color.White;
            this.incomeVsExpenseChartTitle.Location = new System.Drawing.Point(460, 100);
            this.incomeVsExpenseChartTitle.Name = "incomeVsExpenseChartTitle";
            this.incomeVsExpenseChartTitle.Size = new System.Drawing.Size(430, 30);
            this.incomeVsExpenseChartTitle.TabIndex = 11;
            this.incomeVsExpenseChartTitle.Text = "💰 Net Savings Trend (Income - Expenses)";
            this.incomeVsExpenseChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // forecastChart
            // 
            this.forecastChart.BackColor = System.Drawing.Color.White;
            this.forecastChart.Location = new System.Drawing.Point(20, 140);
            this.forecastChart.Name = "forecastChart";
            this.forecastChart.Size = new System.Drawing.Size(430, 230);
            this.forecastChart.TabIndex = 2;
            this.forecastChart.Title = "Expense Forecast - Next 6 Months";
            // 
            // incomeVsExpenseChart
            // 
            this.incomeVsExpenseChart.BackColor = System.Drawing.Color.White;
            this.incomeVsExpenseChart.Location = new System.Drawing.Point(460, 140);
            this.incomeVsExpenseChart.Name = "incomeVsExpenseChart";
            this.incomeVsExpenseChart.Size = new System.Drawing.Size(430, 230);
            this.incomeVsExpenseChart.TabIndex = 3;
            this.incomeVsExpenseChart.Title = "Income vs Expense Trend";


            // 
            // AIInsightsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 950);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AIInsightsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "🤖 AI Financial Insights - Smart Personal Finance Manager";
            this.headerPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.insightsPanel.ResumeLayout(false);
            this.recommendationsPanel.ResumeLayout(false);
            this.savingsPanel.ResumeLayout(false);
            this.healthPanel.ResumeLayout(false);
            this.predictionsPanel.ResumeLayout(false);

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel insightsPanel;
        private System.Windows.Forms.Label insightsLabel;
        private System.Windows.Forms.ListBox insightsListBox;
        private System.Windows.Forms.Panel recommendationsPanel;
        private System.Windows.Forms.Label recommendationsLabel;
        private System.Windows.Forms.ListBox recommendationsListBox;
        private System.Windows.Forms.Panel savingsPanel;
        private System.Windows.Forms.Label savingsLabel;
        private System.Windows.Forms.ListBox savingsListBox;
        private System.Windows.Forms.Panel healthPanel;
        private System.Windows.Forms.Label healthLabel;
        private System.Windows.Forms.Label healthScoreLabel;
        private System.Windows.Forms.Panel predictionsPanel;
        private System.Windows.Forms.Label predictionsLabel;
        private System.Windows.Forms.ListBox predictionsListBox;
        private System.Windows.Forms.Label forecastChartTitle;
        private System.Windows.Forms.Label incomeVsExpenseChartTitle;
        private FinanceML.UI.Controls.ForecastGraphControl forecastChart;
        private FinanceML.UI.Controls.ForecastGraphControl incomeVsExpenseChart;

    }
}
