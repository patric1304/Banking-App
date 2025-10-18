namespace BankingApp.Services;

using System;
using System.Collections.Generic;
using BankingApp.Models;

public class CurrencyService
{
    private readonly Dictionary<(AccountCurrency from, AccountCurrency to), decimal> exchangeRates;

    public CurrencyService()
    {
        exchangeRates = new Dictionary<(AccountCurrency, AccountCurrency), decimal>
        {
            {(AccountCurrency.RON, AccountCurrency.EUR), 0.21m},
            {(AccountCurrency.EUR, AccountCurrency.RON), 4.75m},
            {(AccountCurrency.RON, AccountCurrency.USD), 0.22m},
            {(AccountCurrency.USD, AccountCurrency.RON), 4.50m},
            {(AccountCurrency.EUR, AccountCurrency.USD), 1.05m},
            {(AccountCurrency.USD, AccountCurrency.EUR), 0.95m},
            {(AccountCurrency.GBP, AccountCurrency.EUR), 1.15m},
            {(AccountCurrency.EUR, AccountCurrency.GBP), 0.87m},
            // Add more exchange rates as needed
        };
    }

    public decimal GetExchangeRate(AccountCurrency from, AccountCurrency to)
    {
        if (from == to) return 1m;

        if (exchangeRates.TryGetValue((from, to), out var rate))
        {
            return rate;
        }

        throw new InvalidOperationException("Exchange rate not available.");
    }
}