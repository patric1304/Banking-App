# Banking App

## Overview
A modern banking system application built with **C# .NET 8** and **WPF** using the **MVVM (Model-View-ViewModel)** pattern. The app provides a clean, user-friendly interface for managing bank accounts, processing transactions, and handling multi-currency operations with automatic data persistence.

## Features
- ✅ **Account Management**: Create and manage bank accounts (Checking/Savings)
- 💰 **Transactions**: Deposit and withdraw funds with full transaction history
- 💱 **Multi-Currency Support**: RON, EUR, USD, GBP with real-time conversion
- 🏦 **Multi-Bank Support**: Manage accounts across different banks
- 📊 **Transaction History**: View detailed transaction logs for each account
- 💾 **Automatic Persistence**: All data saved to JSON automatically
- 🎨 **Modern UI**: Clean, responsive WPF interface with loading states

## Technologies
- **.NET 8.0** (Windows Desktop)
- **WPF** (Windows Presentation Foundation)
- **C# 12**
- **MVVM Pattern** (Model-View-ViewModel)
- **JSON** for data persistence
- **Async/Await** for non-blocking operations

## Project Structure

### **Models** (Data Layer)
Contains entity classes representing business objects:
- `Account.cs` - Account entity with properties (IBAN, Balance, Holder, Type, Currency) and business logic (Deposit, Withdraw, ChangeCurrency)
- `Bank.cs` - Bank entity managing multiple accounts with SWIFT code
- `Transaction.cs` - Transaction record (Amount, Type, Timestamp, Balance snapshot)
- `AccountType.cs` - Enum: Checking, Savings
- `AccountCurrency.cs` - Enum: RON, EUR, USD, GBP

### **Services** (Business Logic Layer)
Handles all business operations and data persistence:
- `BankService.cs` - Core service for:
  - Account CRUD operations (Create, Read, Update, Delete)
  - Transaction processing (Deposit, Withdraw, Currency change)
  - Data persistence (Load/Save from JSON)
  - Business logic (IBAN generation, SWIFT codes, validation)

### **ViewModels** (Presentation Layer)
Implements MVVM pattern for UI binding:
- `MainViewModel.cs` - Main application ViewModel:
  - Exposes properties for UI binding (Accounts, TotalAccounts, TotalBanks, IsLoading)
  - Implements commands for user actions (CreateAccount, Deposit, Withdraw, etc.)
  - Handles async operations with loading states
  - Connects Views to Services
- `BaseViewModel.cs` - Base class providing:
  - INotifyPropertyChanged implementation
  - OnPropertyChanged() method for UI notifications
  - SetProperty() helper method

### **Views** (UI Layer)
WPF XAML views with corresponding code-behind:

#### XAML Files (UI Definition):
- `MainWindow.xaml` - Main application window:
  - Dashboard with statistics (Total Accounts, Total Banks)
  - Account list view with actions
  - Welcome panel for new users
  - Loading overlay with spinner
- `AddAccountDialog.xaml` - Dialog for creating new accounts
- `AmountDialog.xaml` - Dialog for deposit/withdraw operations
- `TransactionHistoryDialog.xaml` - Dialog displaying transaction history
- `CurrencyDialog.xaml` - Dialog for currency conversion

#### Code-Behind Files (Minimal UI Logic):
- `MainWindow.xaml.cs` - Initializes UI and sets DataContext
- `AddAccountDialog.xaml.cs` - Handles account creation validation
- `AmountDialog.xaml.cs` - Handles amount input validation
- `TransactionHistoryDialog.xaml.cs` - Displays transaction data
- `CurrencyDialog.xaml.cs` - Handles currency selection

### **Helpers**
Utility classes supporting MVVM:
- `RelayCommand.cs` - ICommand implementation for button bindings
- `BoolToVisibilityConverter.cs` - Converts bool to Visibility for UI elements

### **Application Entry**
- `App.xaml` - Application resources and startup configuration
- `App.xaml.cs` - Application lifecycle management

### **Data Storage**
- `banks.json` - JSON file storing all banks and accounts (auto-generated)

## Architecture: MVVM Pattern

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Models    │◄────────│   Services   │◄────────│  ViewModels │
│   (Data)    │         │   (Logic)    │         │ (Presenter) │
└─────────────┘         └──────────────┘         └──────┬──────┘
                                                         │
                                                    Bindings
                                                         │
                                                         ▼
                                                  ┌─────────────┐
                                                  │    Views    │
                                                  │    (UI)     │
                                                  └─────────────┘
```

**Data Flow Example:**
```
User clicks "Deposit" → Command binding → MainViewModel.DepositAsync()
→ BankService.DepositAsync() → Account.Deposit() → Save to JSON
→ OnPropertyChanged() → UI updates automatically
```

## Setup Instructions

### Prerequisites
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (or Visual Studio Code with C# extension)
- **Windows OS** (WPF is Windows-only)

### Installation Steps
1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd BankingApp
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

   Or press **F5** in Visual Studio to run with debugging.

## Usage Guide

### Creating an Account
1. Click the **"➕ Create Account"** button
2. Enter bank name (e.g., "BCR", "BRD")
3. Enter account holder name
4. Select account type (Checking/Savings)
5. Select currency (RON/EUR/USD/GBP)
6. Click **"Create Account"**
7. Account appears in the list with auto-generated IBAN

### Depositing Funds
1. Select an account from the list
2. Click **"💰 Deposit"** button
3. Enter amount to deposit
4. Click **"Deposit"**
5. Balance updates automatically

### Withdrawing Funds
1. Select an account from the list
2. Click **"💸 Withdraw"** button
3. Enter amount to withdraw
4. Click **"Withdraw"**
5. If insufficient funds, error message appears

### Changing Currency
1. Select an account from the list
2. Click **"💱 Change Currency"** button
3. Select new currency
4. **Note**: 1% conversion fee applies
5. Balance converts to new currency

### Viewing Transaction History
1. Select an account from the list
2. Click **"📊 View Transactions"** button
3. Dialog shows all transactions with:
   - Transaction type (Deposit/Withdraw/Currency Change)
   - Amount
   - Timestamp
   - Balance after transaction

### Deleting an Account
1. Select an account from the list
2. Click **"🗑️ Delete Account"** button
3. Confirm deletion
4. Account removed from all banks

## Data Persistence
- All data automatically saved to `banks.json` after every operation
- File created in application directory on first account creation
- Data loads automatically on application startup
- JSON format for easy inspection and debugging

## Currency Conversion Rates
Current exchange rates (hardcoded in `Account.cs`):
```
RON → EUR: 0.20
RON → USD: 0.22
RON → GBP: 0.17
EUR → RON: 5.00
EUR → USD: 1.10
EUR → GBP: 0.85
USD → RON: 4.50
USD → EUR: 0.91
USD → GBP: 0.77
GBP → RON: 5.90
GBP → EUR: 1.18
GBP → USD: 1.30
```
**Note**: 1% fee applies to all conversions.

## Key Features Implementation

### Async Operations
All file operations and UI-blocking tasks use `async/await`:
```csharp
await _bankService.CreateAccountAsync(...);
await RefreshAccountsAsync();
```
This keeps the UI responsive during operations.

### Observable Collections
```csharp
public ObservableCollection<Account> Accounts { get; set; }
```
Automatically notifies UI when accounts are added/removed.

### Command Pattern
```csharp
public ICommand DepositCommand { get; }
```
Enables/disables buttons based on state (e.g., no account selected).

### Property Change Notifications
```csharp
public bool IsLoading
{
    get => _isLoading;
    set
    {
        _isLoading = value;
        OnPropertyChanged(); 
    }
}
```

## Error Handling
- ✅ Input validation in dialogs (empty fields)
- ✅ Insufficient funds check for withdrawals
- ✅ Automatic retry on file save failures
- ✅ User-friendly error messages via MessageBox

## Future Enhancements (Potential)
- 🔐 User authentication and login
- 📧 Email notifications for transactions
- 📈 Account analytics and charts
- 🌐 Live currency exchange rates (API integration)
- 💳 Credit card support
- 🏧 ATM withdrawal simulation
- 📱 Responsive design for different screen sizes
- 🗄️ Database integration (SQL Server/SQLite)
- 🔍 Search and filter accounts
- 📊 Export transaction history (PDF/Excel)

## License
This project is for educational purposes.

## Author
Pop Patric

