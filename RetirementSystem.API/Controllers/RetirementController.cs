using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RetirementSystem.API.Models;
using RetirementSystem.API.Services;

namespace RetirementSystem.API.Controllers
{
    [ApiController]
    [Route("blackrock/challenge/v1")]
    public class RetirementController : ControllerBase
    {
        private readonly IRetirementService _retirementService;
        private readonly IPerformanceService _performanceService;
        private static Stopwatch _systemStopwatch = Stopwatch.StartNew();

        public RetirementController(IRetirementService retirementService, IPerformanceService performanceService)
        {
            _retirementService = retirementService;
            _performanceService = performanceService;
        }

        [HttpGet("transactions:parse")]
        public ActionResult<List<Transaction>> ParseTransactions([FromBody] TransactionParseRequest request)
        {
            if (request?.Expenses == null) return BadRequest("Missing expenses");
            var result = request.Expenses.Select(e => _retirementService.EnrichTransaction(e)).ToList();
            return Ok(result);
        }

        [HttpPost("transactions:validator")]
        public ActionResult<TransactionValidatorResponse> ValidateTransactions([FromBody] TransactionValidatorRequest request)
        {
            if (request == null) return BadRequest();
            var response = new TransactionValidatorResponse();
            var seen = new HashSet<string>();

            foreach (var t in request.Transactions)
            {
                if (seen.Contains(t.Timestamp))
                {
                    response.Invalid.Add(new InvalidTransaction 
                    { 
                        Timestamp = t.Timestamp, 
                        Amount = t.Amount, 
                        Ceiling = t.Ceiling, 
                        Remanent = t.Remanent, 
                        Message = "Duplicate transaction" 
                    });
                    continue;
                }
                seen.Add(t.Timestamp);

                if (t.Remanent > request.Wage)
                {
                    response.Invalid.Add(new InvalidTransaction 
                    { 
                        Timestamp = t.Timestamp, 
                        Amount = t.Amount, 
                        Ceiling = t.Ceiling, 
                        Remanent = t.Remanent, 
                        Message = "Remanent exceeds wage" 
                    });
                }
                else if (t.Amount < 0)
                {
                    response.Invalid.Add(new InvalidTransaction 
                    { 
                        Timestamp = t.Timestamp, 
                        Amount = t.Amount, 
                        Ceiling = t.Ceiling, 
                        Remanent = t.Remanent, 
                        Message = "Negative amount" 
                    });
                }
                else
                {
                    response.Valid.Add(t);
                }
            }
            return Ok(response);
        }

        [HttpPost("transactions:filter")]
        public ActionResult<FilterResponse> FilterTransactions([FromBody] FilterRequest request)
        {
            if (request == null) return BadRequest();
            var response = new FilterResponse();
            try
            {
                var processed = _retirementService.ApplyRules(request.Transactions, request.Q, request.P);
                response.Valid = processed;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(response);
        }

        [HttpPost("returns:nps")]
        public ActionResult<ReturnsResponse> CalculateNps([FromBody] ReturnsRequest request)
        {
            if (request == null) return BadRequest();
            return Ok(_retirementService.CalculateReturns(request, true));
        }

        [HttpPost("returns:index")]
        public ActionResult<ReturnsResponse> CalculateIndexFund([FromBody] ReturnsRequest request)
        {
            if (request == null) return BadRequest();
            return Ok(_retirementService.CalculateReturns(request, false));
        }

        [HttpGet("performance")]
        public ActionResult<PerformanceResponse> GetPerformance()
        {
            return Ok(_performanceService.GetMetrics(_systemStopwatch.Elapsed));
        }
    }
}
