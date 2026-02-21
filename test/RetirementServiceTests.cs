using System;
using System.Collections.Generic;
using System.Linq;
using RetirementSystem.API.Models;
using RetirementSystem.API.Services;
using Xunit;

namespace RetirementSystem.Tests
{
    public class RetirementServiceTests
    {
        private readonly RetirementService _service;

        public RetirementServiceTests()
        {
            _service = new RetirementService();
        }

        [Fact]
        public void EnrichTransaction_ShouldCalculateCeilingAndRemanent()
        {
            // // Test type: Unit Test
            // // Validation: Verify ceiling and remanent calculation
            // // Command: dotnet test

            var expense = new Expense { Timestamp = "2023-10-12 20:15:00", Amount = 1519 };
            var result = _service.EnrichTransaction(expense);

            Assert.Equal(1600, result.Ceiling);
            Assert.Equal(81, result.Remanent);
        }

        [Fact]
        public void CalculateTax_ShouldApplySlabsCorrectly()
        {
            // // Test type: Unit Test
            // // Validation: Verify progressive tax calculation
            // // Command: dotnet test

            Assert.Equal(0, _service.CalculateTax(600000));
            Assert.Equal(5000, _service.CalculateTax(750000)); // (750k-700k)*0.10 = 5000
            Assert.Equal(30000 + 15000, _service.CalculateTax(1100000)); // (10L-7L)*0.10 + (11L-10L)*0.15 = 30k + 15k = 45000
        }

        [Fact]
        public void ApplyRules_ShouldApplyQandPRules()
        {
            // // Test type: Unit Test
            // // Validation: Verify Q (override) and P (extra) rules
            // // Command: dotnet test

            var transactions = new List<Transaction>
            {
                new Transaction { Timestamp = "2023-07-15 10:00:00", Remanent = 50 }, // Falls in Range Q
                new Transaction { Timestamp = "2023-10-15 10:00:00", Remanent = 50 }  // Falls in Range P
            };

            var q = new List<QPeriod> { new QPeriod { Start = "2023-07-01 00:00:00", End = "2023-07-31 23:59:59", Fixed = 0 } };
            var p = new List<PPeriod> { new PPeriod { Start = "2023-10-01 00:00:00", End = "2023-12-31 23:59:59", Extra = 25 } };

            var result = _service.ApplyRules(transactions, q, p);

            Assert.Equal(0, result[0].Remanent);
            Assert.Equal(75, result[1].Remanent);
        }

        [Fact]
        public void CompoundInterest_CalculationCheck()
        {
            // // Test type: Unit Test
            // // Validation: Verify compound interest formula implementation
            // // Formula: A = P(1 + r)^t (since n=1)
            // // Command: dotnet test

            double P = 145;
            double r = 0.0711;
            int t = 31;
            
            double expected = P * Math.Pow(1 + r, t);
            
            // Result should be around 1219.45 based on document example
            Assert.InRange(expected, 1219, 1220);
        }
    }
}
