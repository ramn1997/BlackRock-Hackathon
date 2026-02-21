using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RetirementSystem.API.Models;

namespace RetirementSystem.API.Services
{
    public interface IRetirementService
    {
        Transaction EnrichTransaction(Expense expense);
        List<Transaction> ApplyRules(List<Transaction> transactions, List<QPeriod> q, List<PPeriod> p);
        List<SavingsByDate> GroupByK(List<Transaction> transactions, List<KPeriod> k);
        double CalculateTax(double income);
        ReturnsResponse CalculateReturns(ReturnsRequest request, bool isNps);
    }

    public class RetirementService : IRetirementService
    {
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        public Transaction EnrichTransaction(Expense expense)
        {
            double ceiling = (Math.Floor(expense.Amount / 100.0) + 1) * 100;
            return new Transaction
            {
                Timestamp = expense.Timestamp,
                Amount = expense.Amount,
                Ceiling = ceiling,
                Remanent = ceiling - expense.Amount
            };
        }

        public List<Transaction> ApplyRules(List<Transaction> transactions, List<QPeriod> q, List<PPeriod> p)
        {
            var result = new List<Transaction>();
            foreach (var t in transactions)
            {
                DateTime dt = DateTime.ParseExact(t.Timestamp, DateFormat, CultureInfo.InvariantCulture);
                double remanent = t.Remanent;

                // Apply Q Rules (Override)
                var matchingQ = q.Where(rule => dt >= DateTime.ParseExact(rule.Start, DateFormat, CultureInfo.InvariantCulture) 
                                            && dt <= DateTime.ParseExact(rule.End, DateFormat, CultureInfo.InvariantCulture))
                                 .OrderByDescending(rule => DateTime.ParseExact(rule.Start, DateFormat, CultureInfo.InvariantCulture))
                                 .FirstOrDefault();
                
                if (matchingQ != null)
                {
                    remanent = matchingQ.Fixed;
                }

                // Apply P Rules (Add Extra)
                var matchingP = p.Where(rule => dt >= DateTime.ParseExact(rule.Start, DateFormat, CultureInfo.InvariantCulture) 
                                            && dt <= DateTime.ParseExact(rule.End, DateFormat, CultureInfo.InvariantCulture));
                
                foreach (var rule in matchingP)
                {
                    remanent += rule.Extra;
                }

                result.Add(new Transaction
                {
                    Timestamp = t.Timestamp,
                    Amount = t.Amount,
                    Ceiling = t.Ceiling,
                    Remanent = remanent
                });
            }
            return result;
        }

        public List<SavingsByDate> GroupByK(List<Transaction> transactions, List<KPeriod> k)
        {
            var result = new List<SavingsByDate>();
            foreach (var period in k)
            {
                DateTime start = DateTime.ParseExact(period.Start, DateFormat, CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(period.End, DateFormat, CultureInfo.InvariantCulture);

                double totalRemanent = transactions
                    .Where(t => {
                        DateTime dt = DateTime.ParseExact(t.Timestamp, DateFormat, CultureInfo.InvariantCulture);
                        return dt >= start && dt <= end;
                    })
                    .Sum(t => t.Remanent);

                result.Add(new SavingsByDate
                {
                    Start = period.Start,
                    End = period.End,
                    Amount = totalRemanent
                });
            }
            return result;
        }

        public double CalculateTax(double income)
        {
            double tax = 0;
            if (income <= 700000) return 0;

            // 7L to 10L: 10%
            if (income > 700000)
            {
                double taxable = Math.Min(income, 1000000) - 700000;
                tax += taxable * 0.10;
            }

            // 10L to 12L: 15%
            if (income > 1000000)
            {
                double taxable = Math.Min(income, 1200000) - 1000000;
                tax += taxable * 0.15;
            }

            // 12L to 15L: 20%
            if (income > 1200000)
            {
                double taxable = Math.Min(income, 1500000) - 1200000;
                tax += taxable * 0.20;
            }

            // Above 15L: 30%
            if (income > 1500000)
            {
                double taxable = income - 1500000;
                tax += taxable * 0.30;
            }

            return tax;
        }

        public ReturnsResponse CalculateReturns(ReturnsRequest request, bool isNps)
        {
            var processedTransactions = ApplyRules(request.Transactions, request.Q, request.P);
            var savingsByDates = GroupByK(processedTransactions, request.K);

            double annualIncome = request.Wage * 12;
            double inflationRate = request.Inflation / 100.0;
            double interestRate = isNps ? 0.0711 : 0.1449;
            int years = (60 - request.Age) > 0 ? (60 - request.Age) : 5;

            foreach (var saving in savingsByDates)
            {
                // Compound Interest: A = P * (1 + r)^t
                double finalAmount = saving.Amount * Math.Pow(1 + interestRate, years);
                
                // Real Return: Areal = A / (1 + inflation)^t
                double realValue = finalAmount / Math.Pow(1 + inflationRate, years);
                saving.Profits = realValue;

                if (isNps)
                {
                    double maxNpsDeduction = Math.Min(annualIncome * 0.10, 200000);
                    double npsDeduction = Math.Min(saving.Amount, maxNpsDeduction);
                    saving.TaxBenefit = CalculateTax(annualIncome) - CalculateTax(annualIncome - npsDeduction);
                }
                else
                {
                    saving.TaxBenefit = 0;
                }
            }

            return new ReturnsResponse
            {
                TransactionsTotalAmount = request.Transactions.Sum(t => t.Amount),
                TransactionsTotalCeiling = request.Transactions.Sum(t => t.Ceiling),
                SavingsByDates = savingsByDates
            };
        }
    }
}
