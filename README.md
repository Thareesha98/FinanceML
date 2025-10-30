# 💰 Smart Personal Finance Manager

A comprehensive desktop application built with C# and Windows Forms that combines traditional financial management with AI-powered insights to help users track expenses, manage budgets, and make informed financial decisions.

## ✨ Features

### 🏦 Core Financial Management
- **Transaction Tracking**: Record income and expenses with detailed categorization
- **Budget Management**: Create and monitor budgets across different spending categories
- **User Authentication**: Secure login system with encrypted password storage
- **Data Export/Import**: Export financial data to PDF and other formats

### 🤖 AI-Powered Features
- **Smart Categorization**: Automatically categorize transactions using machine learning algorithms
- **Expense Forecasting**: Predict future spending patterns using regression analysis
- **Financial Insights**: Get personalized recommendations and spending analysis
- **Trend Analysis**: Identify spending patterns and seasonal variations

### 📊 Analytics & Reporting
- **Interactive Charts**: Visualize spending patterns with pie charts and graphs
- **Financial Health Score**: Get an overall assessment of your financial wellness
- **Spending Predictions**: AI-driven forecasts for future expenses
- **Budget Recommendations**: Intelligent suggestions for budget optimization

## 🛠️ Technology Stack

- **Framework**: .NET 8.0 (Windows Forms)
- **Database**: SQLite for local data storage
- **Security**: BCrypt.Net for password hashing
- **Logging**: Serilog for structured logging
- **PDF Generation**: iTextSharp for report exports
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

## 🚀 Getting Started

### Prerequisites
- Windows 10/11
- .NET 8.0 Runtime
- Visual Studio 2022 (for development)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/smart-personal-finance-manager.git
   cd smart-personal-finance-manager
   ```

2. **Build the solution**
   ```bash
   dotnet build "Smart Personal Finance Manager.sln"
   ```

3. **Run the application**
   ```bash
   cd FinanceML
   dotnet run
   ```

### First Time Setup
1. Launch the application
2. Create a new user account through the registration form
3. Start adding transactions to begin tracking your finances
4. Set up budgets for different spending categories
5. Explore AI insights as your transaction history grows

## 📁 Project Structure

```
FinanceML/
├── AI/                          # Machine Learning Services
│   ├── AIInsightsService.cs     # Financial insights and recommendations
│   ├── ExpenseForecastService.cs # Expense prediction algorithms
│   └── SmartCategorizationService.cs # Transaction categorization
├── Core/                        # Business Logic Layer
│   ├── Data/                    # Data Access Layer
│   ├── Models/                  # Entity Models
│   └── Services/                # Core Business Services
├── UI/                          # User Interface Layer
│   ├── Forms/                   # Application Forms
│   └── Controls/                # Custom UI Controls
└── Program.cs                   # Application Entry Point
```

## 🤖 AI Features Deep Dive

### Smart Categorization
- **Pattern Recognition**: Learns from transaction descriptions and amounts
- **Historical Analysis**: Uses past categorizations to improve accuracy
- **Multi-factor Classification**: Considers description, amount, and context
- **User Learning**: Adapts to user corrections and preferences

### Expense Forecasting
- **Linear Regression**: Analyzes spending trends over time
- **Seasonal Adjustments**: Accounts for monthly and seasonal variations
- **Category-wise Predictions**: Forecasts spending by category
- **Confidence Scoring**: Provides reliability metrics for predictions

### Financial Insights
- **Spending Analysis**: Identifies patterns and anomalies in expenses
- **Budget Optimization**: Suggests budget adjustments based on actual spending
- **Savings Goals**: Recommends achievable savings targets
- **Financial Health**: Calculates overall financial wellness score

## 📊 Key Algorithms

### Expense Prediction Model
```csharp
// Linear regression for trend analysis
var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);

// Seasonal factor calculation
seasonalFactor = monthAverage / overallAverage;

// Final prediction with confidence
predictedAmount = baseAmount + (trend * monthsAhead) * seasonalFactor;
```

### Categorization Algorithm
- **Exact Match**: Checks for identical transaction descriptions
- **Pattern Matching**: Uses predefined keyword patterns
- **Partial Matching**: Finds similar transactions in history
- **Amount-based Rules**: Applies heuristics based on transaction amounts

## 🔧 Configuration

### Database
The application uses SQLite for local data storage. The database file is automatically created on first run.

### Logging
Logs are written to both console and file using Serilog:
- **File Location**: `logs/financeml-.log`
- **Log Levels**: Information, Warning, Error
- **Structured Logging**: JSON format for better analysis

## 🧪 Usage Examples

### Adding a Transaction
```csharp
var transaction = new Transaction
{
    Description = "Grocery Shopping",
    Amount = -150.00m,
    Category = "Food & Dining",
    Date = DateTime.Now
};
```

### Getting AI Insights
```csharp
var insights = AIInsightsService.Instance.GetSpendingInsights();
var forecast = ExpenseForecastService.Instance.GenerateExpenseForecast(6);
```

### Smart Categorization
```csharp
var suggestedCategory = SmartCategorizationService.Instance
    .SuggestCategory("Amazon Purchase", 299.99m);
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow C# coding conventions
- Add unit tests for new features
- Update documentation for API changes
- Ensure AI algorithms are well-documented


## 📞 Support

For support and questions:
- Create an issue on GitHub
- Check the documentation in the `docs/` folder
- Review the project structure guide

## 🔮 Future Enhancements

- **Cloud Sync**: Multi-device synchronization
- **Mobile App**: Companion mobile application
- **Advanced ML**: Deep learning for better predictions
- **Investment Tracking**: Portfolio management features
- **Bill Reminders**: Automated payment notifications
- **Goal Tracking**: Visual progress indicators for financial goals

---
