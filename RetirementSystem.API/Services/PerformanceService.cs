using System;
using System.Diagnostics;
using RetirementSystem.API.Models;

namespace RetirementSystem.API.Services
{
    public interface IPerformanceService
    {
        PerformanceResponse GetMetrics(TimeSpan elapsed);
    }

    public class PerformanceService : IPerformanceService
    {
        public PerformanceResponse GetMetrics(TimeSpan elapsed)
        {
            var process = Process.GetCurrentProcess();
            return new PerformanceResponse
            {
                Time = elapsed.ToString(@"hh\:mm\:ss\.fff"),
                Memory = $"{(process.WorkingSet64 / 1024.0 / 1024.0):F2} MB",
                Threads = process.Threads.Count
            };
        }
    }
}
