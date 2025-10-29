namespace FinanceML
{
    partial class MainForm
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

            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.sidebarPanel = new System.Windows.Forms.Panel();
            this.logoPanel = new System.Windows.Forms.Panel();
            this.logoLabel = new System.Windows.Forms.Label();
            this.navDashboardBtn = new System.Windows.Forms.Button();
            this.navTransactionsBtn = new System.Windows.Forms.Button();
            this.navBudgetsBtn = new System.Windows.Forms.Button();
            this.navGoalsBtn = new System.Windows.Forms.Button();

            this.navSettingsBtn = new System.Windows.Forms.Button();
            this.navLogoutBtn = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.dashboardPanel = new System.Windows.Forms.Panel();
            this.spendingInsightsCard = new System.Windows.Forms.Panel();
            this.spendingInsightsTitle = new System.Windows.Forms.Label();

            this.balanceCard = new System.Windows.Forms.Panel();
            this.balanceLabel = new System.Windows.Forms.Label();
            this.balanceAmountLabel = new System.Windows.Forms.Label();
            this.incomeCard = new System.Windows.Forms.Panel();
            this.incomeLabel = new System.Windows.Forms.Label();
            this.incomeAmountLabel = new System.Windows.Forms.Label();
            this.expenseCard = new System.Windows.Forms.Panel();
            this.expenseLabel = new System.Windows.Forms.Label();
            this.expenseAmountLabel = new System.Windows.Forms.Label();
            this.savingsCard = new System.Windows.Forms.Panel();
            this.savingsLabel = new System.Windows.Forms.Label();
            this.savingsAmountLabel = new System.Windows.Forms.Label();
            this.recentTransactionsCard = new System.Windows.Forms.Panel();
            this.recentTransactionsTitle = new System.Windows.Forms.Label();
            this.recentTransactionsList = new System.Windows.Forms.ListView();
            this.quickActionsCard = new System.Windows.Forms.Panel();
            this.quickActionsTitle = new System.Windows.Forms.Label();
            this.addTransactionBtn = new System.Windows.Forms.Button();
            this.createBudgetBtn = new System.Windows.Forms.Button();
            this.viewReportsBtn = new System.Windows.Forms.Button();
            this.aiInsightsBtn = new System.Windows.Forms.Button();
            this.expensesPieChartCard = new System.Windows.Forms.Panel();
            this.expensesPieChartTitle = new System.Windows.Forms.Label();
            this.expensesPieChart = new FinanceML.UI.Controls.PieChartControl();
            this.incomePieChartCard = new System.Windows.Forms.Panel();
            this.incomePieChartTitle = new System.Windows.Forms.Label();
            this.incomePieChart = new FinanceML.UI.Controls.PieChartControl();
            this.aiInsightsCard = new System.Windows.Forms.Panel();
            this.aiInsightsTitle = new System.Windows.Forms.Label();
            this.aiInsightsText = new System.Windows.Forms.RichTextBox();
            this.forecastChart = new FinanceML.UI.Controls.ForecastGraphControl();
            this.spendingBarChart = new FinanceML.UI.Controls.BarChartControl();
            this.sidebarPanel.SuspendLayout();
            this.logoPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.spendingInsightsCard.SuspendLayout();
            this.expensesPieChartCard.SuspendLayout();
            this.incomePieChartCard.SuspendLayout();
            this.aiInsightsCard.SuspendLayout();

            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.statusStrip.Location = new System.Drawing.Point(0, 746);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1366, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // sidebarPanel
            // 
            this.sidebarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.sidebarPanel.Controls.Add(this.logoPanel);
            this.sidebarPanel.Controls.Add(this.navDashboardBtn);
            this.sidebarPanel.Controls.Add(this.navTransactionsBtn);
            this.sidebarPanel.Controls.Add(this.navBudgetsBtn);
            this.sidebarPanel.Controls.Add(this.navGoalsBtn);

            this.sidebarPanel.Controls.Add(this.navSettingsBtn);
            this.sidebarPanel.Controls.Add(this.navLogoutBtn);
            this.sidebarPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidebarPanel.Location = new System.Drawing.Point(0, 0);
            this.sidebarPanel.Name = "sidebarPanel";
            this.sidebarPanel.Size = new System.Drawing.Size(250, 746);
            this.sidebarPanel.TabIndex = 0;
            this.sidebarPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnSidebarPaint);
            // 
            // logoPanel
            // 
            this.logoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.logoPanel.Controls.Add(this.logoLabel);
            this.logoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.logoPanel.Location = new System.Drawing.Point(0, 0);
            this.logoPanel.Name = "logoPanel";
            this.logoPanel.Size = new System.Drawing.Size(250, 80);
            this.logoPanel.TabIndex = 0;
            this.logoPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnLogoPanelPaint);
            // 
            // logoLabel
            // 
            this.logoLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.logoLabel.ForeColor = System.Drawing.Color.White;
            this.logoLabel.Location = new System.Drawing.Point(0, 0);
            this.logoLabel.Name = "logoLabel";
            this.logoLabel.Size = new System.Drawing.Size(250, 80);
            this.logoLabel.TabIndex = 0;
            this.logoLabel.Text = "FinanceML";
            this.logoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // navDashboardBtn
            // 
            this.navDashboardBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.navDashboardBtn.FlatAppearance.BorderSize = 0;
            this.navDashboardBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navDashboardBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.navDashboardBtn.ForeColor = System.Drawing.Color.Transparent;
            this.navDashboardBtn.Location = new System.Drawing.Point(0, 100);
            this.navDashboardBtn.Name = "navDashboardBtn";
            this.navDashboardBtn.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.navDashboardBtn.Size = new System.Drawing.Size(250, 60);
            this.navDashboardBtn.TabIndex = 1;
            this.navDashboardBtn.Text = "Dashboard";
            this.navDashboardBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.navDashboardBtn.UseVisualStyleBackColor = false;
            this.navDashboardBtn.Click += new System.EventHandler(this.OnNavDashboardClick);
            this.navDashboardBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnNavButtonPaint);
            this.navDashboardBtn.MouseEnter += new System.EventHandler(this.OnNavButtonMouseEnter);
            this.navDashboardBtn.MouseLeave += new System.EventHandler(this.OnNavButtonMouseLeave);
            // 
            // navTransactionsBtn
            // 
            this.navTransactionsBtn.BackColor = System.Drawing.Color.Transparent;
            this.navTransactionsBtn.FlatAppearance.BorderSize = 0;
            this.navTransactionsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navTransactionsBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.navTransactionsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.navTransactionsBtn.Location = new System.Drawing.Point(0, 160);
            this.navTransactionsBtn.Name = "navTransactionsBtn";
            this.navTransactionsBtn.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.navTransactionsBtn.Size = new System.Drawing.Size(250, 60);
            this.navTransactionsBtn.TabIndex = 2;
            this.navTransactionsBtn.Text = "Transactions";
            this.navTransactionsBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.navTransactionsBtn.UseVisualStyleBackColor = false;
            this.navTransactionsBtn.Click += new System.EventHandler(this.OnNavTransactionsClick);
            this.navTransactionsBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnNavButtonPaint);
            this.navTransactionsBtn.MouseEnter += new System.EventHandler(this.OnNavButtonMouseEnter);
            this.navTransactionsBtn.MouseLeave += new System.EventHandler(this.OnNavButtonMouseLeave);
            // 
            // navBudgetsBtn
            // 
            this.navBudgetsBtn.BackColor = System.Drawing.Color.Transparent;
            this.navBudgetsBtn.FlatAppearance.BorderSize = 0;
            this.navBudgetsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navBudgetsBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.navBudgetsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.navBudgetsBtn.Location = new System.Drawing.Point(0, 220);
            this.navBudgetsBtn.Name = "navBudgetsBtn";
            this.navBudgetsBtn.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.navBudgetsBtn.Size = new System.Drawing.Size(250, 60);
            this.navBudgetsBtn.TabIndex = 3;
            this.navBudgetsBtn.Text = "Budgets";
            this.navBudgetsBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.navBudgetsBtn.UseVisualStyleBackColor = false;
            this.navBudgetsBtn.Click += new System.EventHandler(this.OnNavBudgetsClick);
            this.navBudgetsBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnNavButtonPaint);
            this.navBudgetsBtn.MouseEnter += new System.EventHandler(this.OnNavButtonMouseEnter);
            this.navBudgetsBtn.MouseLeave += new System.EventHandler(this.OnNavButtonMouseLeave);
            // 
            // navGoalsBtn
            // 
            this.navGoalsBtn.BackColor = System.Drawing.Color.Transparent;
            this.navGoalsBtn.FlatAppearance.BorderSize = 0;
            this.navGoalsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navGoalsBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.navGoalsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.navGoalsBtn.Location = new System.Drawing.Point(0, 280);
            this.navGoalsBtn.Name = "navGoalsBtn";
            this.navGoalsBtn.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.navGoalsBtn.Size = new System.Drawing.Size(250, 60);
            this.navGoalsBtn.TabIndex = 4;
            this.navGoalsBtn.Text = "Financial Goals";
            this.navGoalsBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.navGoalsBtn.UseVisualStyleBackColor = false;
            this.navGoalsBtn.Click += new System.EventHandler(this.OnNavGoalsClick);
            this.navGoalsBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnNavButtonPaint);
            this.navGoalsBtn.MouseEnter += new System.EventHandler(this.OnNavButtonMouseEnter);
            this.navGoalsBtn.MouseLeave += new System.EventHandler(this.OnNavButtonMouseLeave);

            // 
            // navSettingsBtn
            // 
            this.navSettingsBtn.BackColor = System.Drawing.Color.Transparent;
            this.navSettingsBtn.FlatAppearance.BorderSize = 0;
            this.navSettingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navSettingsBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.navSettingsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.navSettingsBtn.Location = new System.Drawing.Point(0, 580);
            this.navSettingsBtn.Name = "navSettingsBtn";
            this.navSettingsBtn.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.navSettingsBtn.Size = new System.Drawing.Size(250, 60);
            this.navSettingsBtn.TabIndex = 6;
            this.navSettingsBtn.Text = "Settings";
            this.navSettingsBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.navSettingsBtn.UseVisualStyleBackColor = false;
            this.navSettingsBtn.Click += new System.EventHandler(this.OnNavSettingsClick);
            this.navSettingsBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnNavButtonPaint);
            // 
            // navLogoutBtn
            // 
            this.navLogoutBtn.BackColor = System.Drawing.Color.Transparent;
            this.navLogoutBtn.FlatAppearance.BorderSize = 0;
            this.navLogoutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navLogoutBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.navLogoutBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.navLogoutBtn.Location = new System.Drawing.Point(0, 640);
            this.navLogoutBtn.Name = "navLogoutBtn";
            this.navLogoutBtn.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.navLogoutBtn.Size = new System.Drawing.Size(250, 60);
            this.navLogoutBtn.TabIndex = 6;
            this.navLogoutBtn.Text = "Logout";
            this.navLogoutBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.navLogoutBtn.UseVisualStyleBackColor = false;
            this.navLogoutBtn.Click += new System.EventHandler(this.OnNavLogoutClick);
            this.navLogoutBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnNavButtonPaint);
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.contentPanel.Controls.Add(this.dashboardPanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(250, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(1116, 746);
            this.contentPanel.TabIndex = 1;
            // 
            // dashboardPanel
            // 
            this.dashboardPanel.AutoScroll = true;
            this.dashboardPanel.Controls.Add(this.spendingInsightsCard);
            this.dashboardPanel.Controls.Add(this.expensesPieChartCard);
            this.dashboardPanel.Controls.Add(this.incomePieChartCard);
            this.dashboardPanel.Controls.Add(this.balanceCard);
            this.dashboardPanel.Controls.Add(this.incomeCard);
            this.dashboardPanel.Controls.Add(this.expenseCard);
            this.dashboardPanel.Controls.Add(this.savingsCard);
            this.dashboardPanel.Controls.Add(this.recentTransactionsCard);
            this.dashboardPanel.Controls.Add(this.quickActionsCard);

            this.dashboardPanel.Controls.Add(this.aiInsightsCard);
            this.dashboardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardPanel.Location = new System.Drawing.Point(0, 0);
            this.dashboardPanel.Name = "dashboardPanel";
            this.dashboardPanel.Padding = new System.Windows.Forms.Padding(20);
            this.dashboardPanel.Size = new System.Drawing.Size(1116, 746);
            this.dashboardPanel.TabIndex = 0;
            // 
            // spendingInsightsCard
            // 
            this.spendingInsightsCard.BackColor = System.Drawing.Color.White;
            this.spendingInsightsCard.Controls.Add(this.spendingInsightsTitle);
            this.spendingInsightsCard.Controls.Add(this.spendingBarChart);
            this.spendingInsightsCard.Location = new System.Drawing.Point(30, 650);
            this.spendingInsightsCard.Name = "spendingInsightsCard";
            this.spendingInsightsCard.Size = new System.Drawing.Size(1050, 290);
            this.spendingInsightsCard.TabIndex = 6;
            this.spendingInsightsCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // spendingInsightsTitle
            // 
            this.spendingInsightsTitle.BackColor = System.Drawing.Color.White;
            this.spendingInsightsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.spendingInsightsTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.spendingInsightsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.spendingInsightsTitle.Location = new System.Drawing.Point(0, 0);
            this.spendingInsightsTitle.Name = "spendingInsightsTitle";
            this.spendingInsightsTitle.Padding = new System.Windows.Forms.Padding(25, 18, 25, 18);
            this.spendingInsightsTitle.Size = new System.Drawing.Size(1050, 70);
            this.spendingInsightsTitle.TabIndex = 0;
            this.spendingInsightsTitle.Text = "Spending Insights";

            // 
            // balanceCard
            // 
            this.balanceCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.balanceCard.Controls.Add(this.balanceLabel);
            this.balanceCard.Controls.Add(this.balanceAmountLabel);
            this.balanceCard.Location = new System.Drawing.Point(30, 30);
            this.balanceCard.Name = "balanceCard";
            this.balanceCard.Size = new System.Drawing.Size(250, 160);
            this.balanceCard.TabIndex = 0;
            this.balanceCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // balanceLabel
            // 
            this.balanceLabel.AutoSize = true;
            this.balanceLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.balanceLabel.ForeColor = System.Drawing.Color.White;
            this.balanceLabel.Location = new System.Drawing.Point(25, 25);
            this.balanceLabel.Name = "balanceLabel";
            this.balanceLabel.Size = new System.Drawing.Size(125, 25);
            this.balanceLabel.TabIndex = 0;
            this.balanceLabel.Text = "Total Balance";
            // 
            // balanceAmountLabel
            // 
            this.balanceAmountLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.balanceAmountLabel.ForeColor = System.Drawing.Color.White;
            this.balanceAmountLabel.Location = new System.Drawing.Point(25, 60);
            this.balanceAmountLabel.Name = "balanceAmountLabel";
            this.balanceAmountLabel.Size = new System.Drawing.Size(230, 60);
            this.balanceAmountLabel.TabIndex = 1;
            this.balanceAmountLabel.Text = "Rs 12,345.67";
            this.balanceAmountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // incomeCard
            // 
            this.incomeCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.incomeCard.Controls.Add(this.incomeLabel);
            this.incomeCard.Controls.Add(this.incomeAmountLabel);
            this.incomeCard.Location = new System.Drawing.Point(300, 30);
            this.incomeCard.Name = "incomeCard";
            this.incomeCard.Size = new System.Drawing.Size(250, 160);
            this.incomeCard.TabIndex = 1;
            this.incomeCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // incomeLabel
            // 
            this.incomeLabel.AutoSize = true;
            this.incomeLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.incomeLabel.ForeColor = System.Drawing.Color.White;
            this.incomeLabel.Location = new System.Drawing.Point(25, 25);
            this.incomeLabel.Name = "incomeLabel";
            this.incomeLabel.Size = new System.Drawing.Size(142, 25);
            this.incomeLabel.TabIndex = 0;
            this.incomeLabel.Text = "Monthly Income";
            // 
            // incomeAmountLabel
            // 
            this.incomeAmountLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.incomeAmountLabel.ForeColor = System.Drawing.Color.White;
            this.incomeAmountLabel.Location = new System.Drawing.Point(25, 60);
            this.incomeAmountLabel.Name = "incomeAmountLabel";
            this.incomeAmountLabel.Size = new System.Drawing.Size(230, 60);
            this.incomeAmountLabel.TabIndex = 1;
            this.incomeAmountLabel.Text = "Rs 5,200.00 ↗";
            this.incomeAmountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // expenseCard
            // 
            this.expenseCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.expenseCard.Controls.Add(this.expenseLabel);
            this.expenseCard.Controls.Add(this.expenseAmountLabel);
            this.expenseCard.Location = new System.Drawing.Point(570, 30);
            this.expenseCard.Name = "expenseCard";
            this.expenseCard.Size = new System.Drawing.Size(250, 160);
            this.expenseCard.TabIndex = 2;
            this.expenseCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // expenseLabel
            // 
            this.expenseLabel.AutoSize = true;
            this.expenseLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.expenseLabel.ForeColor = System.Drawing.Color.White;
            this.expenseLabel.Location = new System.Drawing.Point(25, 25);
            this.expenseLabel.Name = "expenseLabel";
            this.expenseLabel.Size = new System.Drawing.Size(150, 25);
            this.expenseLabel.TabIndex = 0;
            this.expenseLabel.Text = "Monthly Expense";
            // 
            // expenseAmountLabel
            // 
            this.expenseAmountLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.expenseAmountLabel.ForeColor = System.Drawing.Color.White;
            this.expenseAmountLabel.Location = new System.Drawing.Point(25, 60);
            this.expenseAmountLabel.Name = "expenseAmountLabel";
            this.expenseAmountLabel.Size = new System.Drawing.Size(230, 60);
            this.expenseAmountLabel.TabIndex = 1;
            this.expenseAmountLabel.Text = "Rs 3,450.00 ↘";
            this.expenseAmountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // savingsCard
            // 
            this.savingsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.savingsCard.Controls.Add(this.savingsLabel);
            this.savingsCard.Controls.Add(this.savingsAmountLabel);
            this.savingsCard.Location = new System.Drawing.Point(840, 30);
            this.savingsCard.Name = "savingsCard";
            this.savingsCard.Size = new System.Drawing.Size(250, 160);
            this.savingsCard.TabIndex = 3;
            this.savingsCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // savingsLabel
            // 
            this.savingsLabel.AutoSize = true;
            this.savingsLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.savingsLabel.ForeColor = System.Drawing.Color.White;
            this.savingsLabel.Location = new System.Drawing.Point(25, 25);
            this.savingsLabel.Name = "savingsLabel";
            this.savingsLabel.Size = new System.Drawing.Size(139, 25);
            this.savingsLabel.TabIndex = 0;
            this.savingsLabel.Text = "Monthly Savings";
            // 
            // savingsAmountLabel
            // 
            this.savingsAmountLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.savingsAmountLabel.ForeColor = System.Drawing.Color.White;
            this.savingsAmountLabel.Location = new System.Drawing.Point(25, 60);
            this.savingsAmountLabel.Name = "savingsAmountLabel";
            this.savingsAmountLabel.Size = new System.Drawing.Size(230, 60);
            this.savingsAmountLabel.TabIndex = 1;
            this.savingsAmountLabel.Text = "Rs 1,750.00 ↗";
            this.savingsAmountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // recentTransactionsCard
            // 
            this.recentTransactionsCard.BackColor = System.Drawing.Color.White;
            this.recentTransactionsCard.Controls.Add(this.recentTransactionsTitle);
            this.recentTransactionsCard.Controls.Add(this.recentTransactionsList);
            this.recentTransactionsCard.Location = new System.Drawing.Point(30, 220);
            this.recentTransactionsCard.Name = "recentTransactionsCard";
            this.recentTransactionsCard.Size = new System.Drawing.Size(750, 400);
            this.recentTransactionsCard.TabIndex = 4;
            this.recentTransactionsCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // recentTransactionsTitle
            // 
            this.recentTransactionsTitle.BackColor = System.Drawing.Color.White;
            this.recentTransactionsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.recentTransactionsTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.recentTransactionsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.recentTransactionsTitle.Location = new System.Drawing.Point(0, 0);
            this.recentTransactionsTitle.Name = "recentTransactionsTitle";
            this.recentTransactionsTitle.Padding = new System.Windows.Forms.Padding(25, 18, 25, 18);
            this.recentTransactionsTitle.Size = new System.Drawing.Size(750, 60);
            this.recentTransactionsTitle.TabIndex = 0;
            this.recentTransactionsTitle.Text = "Recent Transactions";
            // 
            // recentTransactionsList
            // 
            this.recentTransactionsList.BackColor = System.Drawing.Color.White;
            this.recentTransactionsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.recentTransactionsList.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.recentTransactionsList.FullRowSelect = true;
            this.recentTransactionsList.GridLines = true;
            this.recentTransactionsList.Location = new System.Drawing.Point(10, 70);
            this.recentTransactionsList.Name = "recentTransactionsList";
            this.recentTransactionsList.Size = new System.Drawing.Size(730, 320);
            this.recentTransactionsList.TabIndex = 1;
            this.recentTransactionsList.UseCompatibleStateImageBehavior = false;
            this.recentTransactionsList.View = System.Windows.Forms.View.Details;
            this.recentTransactionsList.Scrollable = true;
            // 
            // quickActionsCard
            // 
            this.quickActionsCard.BackColor = System.Drawing.Color.White;
            this.quickActionsCard.Controls.Add(this.quickActionsTitle);
            this.quickActionsCard.Controls.Add(this.addTransactionBtn);
            this.quickActionsCard.Controls.Add(this.createBudgetBtn);
            this.quickActionsCard.Controls.Add(this.viewReportsBtn);
            this.quickActionsCard.Controls.Add(this.aiInsightsBtn);
            this.quickActionsCard.Location = new System.Drawing.Point(800, 220);
            this.quickActionsCard.Name = "quickActionsCard";
            this.quickActionsCard.Size = new System.Drawing.Size(290, 400);
            this.quickActionsCard.TabIndex = 5;
            this.quickActionsCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // quickActionsTitle
            // 
            this.quickActionsTitle.BackColor = System.Drawing.Color.White;
            this.quickActionsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.quickActionsTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.quickActionsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.quickActionsTitle.Location = new System.Drawing.Point(0, 0);
            this.quickActionsTitle.Name = "quickActionsTitle";
            this.quickActionsTitle.Padding = new System.Windows.Forms.Padding(25, 18, 25, 18);
            this.quickActionsTitle.Size = new System.Drawing.Size(290, 60);
            this.quickActionsTitle.TabIndex = 0;
            this.quickActionsTitle.Text = "Quick Actions";
            // 
            // addTransactionBtn
            // 
            this.addTransactionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.addTransactionBtn.FlatAppearance.BorderSize = 0;
            this.addTransactionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addTransactionBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.addTransactionBtn.ForeColor = System.Drawing.Color.White;
            this.addTransactionBtn.Location = new System.Drawing.Point(70, 90);
            this.addTransactionBtn.Name = "addTransactionBtn";
            this.addTransactionBtn.Size = new System.Drawing.Size(150, 50);
            this.addTransactionBtn.TabIndex = 1;
            this.addTransactionBtn.Text = "Add Transaction";
            this.addTransactionBtn.UseVisualStyleBackColor = false;
            this.addTransactionBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnButtonPaint);
            // 
            // createBudgetBtn
            // 
            this.createBudgetBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.createBudgetBtn.FlatAppearance.BorderSize = 0;
            this.createBudgetBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.createBudgetBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.createBudgetBtn.ForeColor = System.Drawing.Color.White;
            this.createBudgetBtn.Location = new System.Drawing.Point(70, 160);
            this.createBudgetBtn.Name = "createBudgetBtn";
            this.createBudgetBtn.Size = new System.Drawing.Size(150, 50);
            this.createBudgetBtn.TabIndex = 2;
            this.createBudgetBtn.Text = "Create Budget";
            this.createBudgetBtn.UseVisualStyleBackColor = false;
            this.createBudgetBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnButtonPaint);
            // 
            // viewReportsBtn
            // 
            this.viewReportsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.viewReportsBtn.FlatAppearance.BorderSize = 0;
            this.viewReportsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.viewReportsBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.viewReportsBtn.ForeColor = System.Drawing.Color.White;
            this.viewReportsBtn.Location = new System.Drawing.Point(70, 230);
            this.viewReportsBtn.Name = "viewReportsBtn";
            this.viewReportsBtn.Size = new System.Drawing.Size(150, 50);
            this.viewReportsBtn.TabIndex = 3;
            this.viewReportsBtn.Text = "View Reports";
            this.viewReportsBtn.UseVisualStyleBackColor = false;
            this.viewReportsBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnButtonPaint);
            // 
            // aiInsightsBtn
            // 
            this.aiInsightsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(69)))), ((int)(((byte)(19)))));
            this.aiInsightsBtn.FlatAppearance.BorderSize = 0;
            this.aiInsightsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aiInsightsBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.aiInsightsBtn.ForeColor = System.Drawing.Color.White;
            this.aiInsightsBtn.Location = new System.Drawing.Point(70, 300);
            this.aiInsightsBtn.Name = "aiInsightsBtn";
            this.aiInsightsBtn.Size = new System.Drawing.Size(150, 50);
            this.aiInsightsBtn.TabIndex = 4;
            this.aiInsightsBtn.Text = "AI Insights";
            this.aiInsightsBtn.UseVisualStyleBackColor = false;
            this.aiInsightsBtn.Click += new System.EventHandler(this.OnAIInsightsClick);
            this.aiInsightsBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.OnButtonPaint);
            // 
            // expensesPieChartCard
            // 
            this.expensesPieChartCard.BackColor = System.Drawing.Color.White;
            this.expensesPieChartCard.Controls.Add(this.expensesPieChartTitle);
            this.expensesPieChartCard.Controls.Add(this.expensesPieChart);
            this.expensesPieChartCard.Location = new System.Drawing.Point(30, 960);
            this.expensesPieChartCard.Name = "expensesPieChartCard";
            this.expensesPieChartCard.Size = new System.Drawing.Size(520, 320);
            this.expensesPieChartCard.TabIndex = 5;
            this.expensesPieChartCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // expensesPieChartTitle
            // 
            this.expensesPieChartTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.expensesPieChartTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.expensesPieChartTitle.Location = new System.Drawing.Point(20, 20);
            this.expensesPieChartTitle.Name = "expensesPieChartTitle";
            this.expensesPieChartTitle.Size = new System.Drawing.Size(480, 30);
            this.expensesPieChartTitle.TabIndex = 0;
            this.expensesPieChartTitle.Text = "Expense Breakdown";
            this.expensesPieChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // expensesPieChart
            // 
            this.expensesPieChart.BackColor = System.Drawing.Color.White;
            this.expensesPieChart.Location = new System.Drawing.Point(20, 60);
            this.expensesPieChart.Name = "expensesPieChart";
            this.expensesPieChart.Size = new System.Drawing.Size(480, 260);
            this.expensesPieChart.TabIndex = 1;
            // 
            // incomePieChartCard
            // 
            this.incomePieChartCard.BackColor = System.Drawing.Color.White;
            this.incomePieChartCard.Controls.Add(this.incomePieChartTitle);
            this.incomePieChartCard.Controls.Add(this.incomePieChart);
            this.incomePieChartCard.Location = new System.Drawing.Point(570, 960);
            this.incomePieChartCard.Name = "incomePieChartCard";
            this.incomePieChartCard.Size = new System.Drawing.Size(520, 320);
            this.incomePieChartCard.TabIndex = 6;
            this.incomePieChartCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // incomePieChartTitle
            // 
            this.incomePieChartTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.incomePieChartTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.incomePieChartTitle.Location = new System.Drawing.Point(20, 20);
            this.incomePieChartTitle.Name = "incomePieChartTitle";
            this.incomePieChartTitle.Size = new System.Drawing.Size(480, 30);
            this.incomePieChartTitle.TabIndex = 0;
            this.incomePieChartTitle.Text = "Income Sources";
            this.incomePieChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // incomePieChart
            // 
            this.incomePieChart.BackColor = System.Drawing.Color.White;
            this.incomePieChart.Location = new System.Drawing.Point(20, 60);
            this.incomePieChart.Name = "incomePieChart";
            this.incomePieChart.Size = new System.Drawing.Size(480, 260);
            this.incomePieChart.TabIndex = 1;
            // 
            // aiInsightsCard
            // 
            this.aiInsightsCard.BackColor = System.Drawing.Color.White;
            this.aiInsightsCard.Controls.Add(this.aiInsightsTitle);
            this.aiInsightsCard.Controls.Add(this.aiInsightsText);
            this.aiInsightsCard.Controls.Add(this.forecastChart);
            this.aiInsightsCard.Location = new System.Drawing.Point(30, 1300);
            this.aiInsightsCard.Name = "aiInsightsCard";
            this.aiInsightsCard.Size = new System.Drawing.Size(1050, 400);
            this.aiInsightsCard.TabIndex = 7;
            this.aiInsightsCard.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCardPaint);
            // 
            // aiInsightsTitle
            // 
            this.aiInsightsTitle.BackColor = System.Drawing.Color.White;
            this.aiInsightsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.aiInsightsTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.aiInsightsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.aiInsightsTitle.Location = new System.Drawing.Point(0, 0);
            this.aiInsightsTitle.Name = "aiInsightsTitle";
            this.aiInsightsTitle.Padding = new System.Windows.Forms.Padding(25, 18, 25, 18);
            this.aiInsightsTitle.Size = new System.Drawing.Size(1050, 70);
            this.aiInsightsTitle.TabIndex = 0;
            this.aiInsightsTitle.Text = "🤖 AI Financial Insights";
            // 
            // aiInsightsText
            // 
            this.aiInsightsText.BackColor = System.Drawing.Color.White;
            this.aiInsightsText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.aiInsightsText.Dock = System.Windows.Forms.DockStyle.None;
            this.aiInsightsText.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.aiInsightsText.Location = new System.Drawing.Point(20, 70);
            this.aiInsightsText.Name = "aiInsightsText";
            this.aiInsightsText.Padding = new System.Windows.Forms.Padding(20);
            this.aiInsightsText.ReadOnly = true;
            this.aiInsightsText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.aiInsightsText.Size = new System.Drawing.Size(1010, 120);
            this.aiInsightsText.TabIndex = 1;
            this.aiInsightsText.Text = "";
            // 
            // forecastChart
            // 
            this.forecastChart.BackColor = System.Drawing.Color.White;
            this.forecastChart.Location = new System.Drawing.Point(20, 200);
            this.forecastChart.Name = "forecastChart";
            this.forecastChart.Size = new System.Drawing.Size(1010, 180);
            this.forecastChart.TabIndex = 1;
            this.forecastChart.Title = "Expense Forecast";
            // 
            // spendingBarChart
            // 
            this.spendingBarChart.BackColor = System.Drawing.Color.White;
            this.spendingBarChart.Location = new System.Drawing.Point(20, 70);
            this.spendingBarChart.Name = "spendingBarChart";
            this.spendingBarChart.Size = new System.Drawing.Size(1010, 200);
            this.spendingBarChart.TabIndex = 1;
            this.spendingBarChart.Title = "Monthly Spending Trends";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Smart Personal Finance Manager";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.sidebarPanel.ResumeLayout(false);
            this.logoPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.spendingInsightsCard.ResumeLayout(false);
            this.expensesPieChartCard.ResumeLayout(false);
            this.incomePieChartCard.ResumeLayout(false);
            this.aiInsightsCard.ResumeLayout(false);

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel sidebarPanel;
        private System.Windows.Forms.Panel logoPanel;
        private System.Windows.Forms.Label logoLabel;
        private System.Windows.Forms.Button navDashboardBtn;
        private System.Windows.Forms.Button navTransactionsBtn;
        private System.Windows.Forms.Button navBudgetsBtn;
        private System.Windows.Forms.Button navGoalsBtn;

        private System.Windows.Forms.Button navSettingsBtn;
        private System.Windows.Forms.Button navLogoutBtn;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel dashboardPanel;
        private System.Windows.Forms.Panel balanceCard;
        private System.Windows.Forms.Label balanceLabel;
        private System.Windows.Forms.Label balanceAmountLabel;
        private System.Windows.Forms.Panel incomeCard;
        private System.Windows.Forms.Label incomeLabel;
        private System.Windows.Forms.Label incomeAmountLabel;
        private System.Windows.Forms.Panel expenseCard;
        private System.Windows.Forms.Label expenseLabel;
        private System.Windows.Forms.Label expenseAmountLabel;
        private System.Windows.Forms.Panel savingsCard;
        private System.Windows.Forms.Label savingsLabel;
        private System.Windows.Forms.Label savingsAmountLabel;
        private System.Windows.Forms.Panel recentTransactionsCard;
        private System.Windows.Forms.Label recentTransactionsTitle;
        private System.Windows.Forms.ListView recentTransactionsList;
        private System.Windows.Forms.Panel quickActionsCard;
        private System.Windows.Forms.Label quickActionsTitle;
        private System.Windows.Forms.Button addTransactionBtn;
        private System.Windows.Forms.Button createBudgetBtn;
        private System.Windows.Forms.Button viewReportsBtn;
        private System.Windows.Forms.Button aiInsightsBtn;
        private System.Windows.Forms.Panel spendingInsightsCard;
        private System.Windows.Forms.Label spendingInsightsTitle;

        private System.Windows.Forms.Panel expensesPieChartCard;
        private System.Windows.Forms.Label expensesPieChartTitle;
        private FinanceML.UI.Controls.PieChartControl expensesPieChart;
        private System.Windows.Forms.Panel incomePieChartCard;
        private System.Windows.Forms.Label incomePieChartTitle;
        private FinanceML.UI.Controls.PieChartControl incomePieChart;
        private System.Windows.Forms.Panel aiInsightsCard;
        private System.Windows.Forms.Label aiInsightsTitle;
        private System.Windows.Forms.RichTextBox aiInsightsText;
        private FinanceML.UI.Controls.ForecastGraphControl forecastChart;
        private FinanceML.UI.Controls.BarChartControl spendingBarChart;
    }
}
