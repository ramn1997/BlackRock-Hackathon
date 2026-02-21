using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace RetirementSystem.API.Models
{
    public class Expense
    {
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public double Amount { get; set; }
    }

    public class Transaction : Expense
    {
        [JsonPropertyName("ceiling")]
        public double Ceiling { get; set; }

        [JsonPropertyName("remanent")]
        public double Remanent { get; set; }
    }

    public class InvalidTransaction : Transaction
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class QPeriod
    {
        [JsonPropertyName("fixed")]
        public double Fixed { get; set; }

        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
    }

    public class PPeriod
    {
        [JsonPropertyName("extra")]
        public double Extra { get; set; }

        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
    }

    public class KPeriod
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
    }

    public class TransactionParseRequest
    {
        [JsonPropertyName("expenses")]
        public List<Expense> Expenses { get; set; } = new();
    }

    public class TransactionValidatorRequest
    {
        [JsonPropertyName("wage")]
        public double Wage { get; set; }

        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; } = new();
    }

    public class TransactionValidatorResponse
    {
        [JsonPropertyName("valid")]
        public List<Transaction> Valid { get; set; } = new();

        [JsonPropertyName("invalid")]
        public List<InvalidTransaction> Invalid { get; set; } = new();
    }

    public class FilterRequest
    {
        [JsonPropertyName("q")]
        public List<QPeriod> Q { get; set; } = new();

        [JsonPropertyName("p")]
        public List<PPeriod> P { get; set; } = new();

        [JsonPropertyName("k")]
        public List<KPeriod> K { get; set; } = new();

        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; } = new();
    }

    public class FilterResponse
    {
        [JsonPropertyName("valid")]
        public List<Transaction> Valid { get; set; } = new();

        [JsonPropertyName("invalid")]
        public List<InvalidTransaction> Invalid { get; set; } = new();
    }

    public class ReturnsRequest
    {
        [JsonPropertyName("age")]
        public int Age { get; set; }

        [JsonPropertyName("wage")]
        public double Wage { get; set; }

        [JsonPropertyName("inflation")]
        public double Inflation { get; set; }

        [JsonPropertyName("q")]
        public List<QPeriod> Q { get; set; } = new();

        [JsonPropertyName("p")]
        public List<PPeriod> P { get; set; } = new();

        [JsonPropertyName("k")]
        public List<KPeriod> K { get; set; } = new();

        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; } = new();
    }

    public class SavingsByDate
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("profits")]
        public double Profits { get; set; }

        [JsonPropertyName("taxBenefit")]
        public double TaxBenefit { get; set; }
    }

    public class ReturnsResponse
    {
        [JsonPropertyName("transactionsTotalAmount")]
        public double TransactionsTotalAmount { get; set; }

        [JsonPropertyName("transactionsTotalCeiling")]
        public double TransactionsTotalCeiling { get; set; }

        [JsonPropertyName("savingsByDates")]
        public List<SavingsByDate> SavingsByDates { get; set; } = new();
    }

    public class PerformanceResponse
    {
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("memory")]
        public string Memory { get; set; } = string.Empty;

        [JsonPropertyName("threads")]
        public int Threads { get; set; }
    }
}
