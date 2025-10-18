# Banking App

## Overview
The Banking App is a simple banking system application built using C# and the .NET framework. It provides functionalities for managing bank accounts, processing transactions, and handling currency conversions through a user-friendly graphical interface.

## Features
- Create and manage bank accounts for individuals and companies.
- Deposit and withdraw funds from accounts.
- Change account currency with applicable fees.
- View transaction history for each account.
- User-friendly interface built with WPF.

## Project Structure
- **Models**: Contains the data models for the application.
  - `Account.cs`: Defines the Account class.
  - `Transaction.cs`: Defines the Transaction class.
  - `Bank.cs`: Manages multiple accounts and transactions.
  
- **ViewModels**: Contains the ViewModel classes for the MVVM pattern.
  - `MainViewModel.cs`: Data context for the main view.
  - `AccountViewModel.cs`: Manages account-related operations.
  - `TransactionViewModel.cs`: Manages transaction-related operations.

- **Views**: Contains the XAML files for the user interface.
  - `MainWindow.xaml`: Main application window layout.
  - `AccountView.xaml`: Layout for managing account information.
  - `TransactionView.xaml`: Layout for managing transactions.

- **Services**: Contains service classes for business logic.
  - `BankService.cs`: Methods for managing accounts and transactions.
  - `CurrencyService.cs`: Methods for currency conversion.

- **Helpers**: Contains helper classes.
  - `RelayCommand.cs`: Implements ICommand for command binding.

- **App.xaml**: Application resources and startup settings.
- **App.xaml.cs**: Application startup logic.
- **BankingApp.csproj**: Project configuration file.

## Setup Instructions
1. Clone the repository or download the source code.
2. Open the project in your preferred IDE that supports .NET development.
3. Restore the project dependencies.
4. Build the project to ensure there are no errors.
5. Run the application to start using the Banking App.

## Usage Guidelines
- Upon launching the application, you will be presented with the main window.
- Use the navigation options to access account management and transaction features.
- Follow the prompts to create accounts, make deposits, withdraw funds, and change currencies.

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.