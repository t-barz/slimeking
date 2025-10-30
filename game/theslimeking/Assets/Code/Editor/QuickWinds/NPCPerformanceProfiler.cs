using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Performance profiler for NPCQuickConfig operations.
    /// Tracks timing and memory usage to ensure performance targets are met.
    /// </summary>
    public static class NPCPerformanceProfiler
    {
        private static Dictionary<string, Stopwatch> activeTimers = new Dictionary<string, Stopwatch>();
        private static Dictionary<string, List<double>> performanceMetrics = new Dictionary<string, List<double>>();

        /// <summary>
        /// Starts timing an operation.
        /// </summary>
        /// <param name="operationName">Name of the operation to time</param>
        public static void StartTiming(string operationName)
        {
            if (!activeTimers.ContainsKey(operationName))
            {
                activeTimers[operationName] = new Stopwatch();
            }

            activeTimers[operationName].Restart();
        }

        /// <summary>
        /// Stops timing an operation and records the result.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Elapsed time in milliseconds</returns>
        public static double StopTiming(string operationName)
        {
            if (!activeTimers.ContainsKey(operationName))
            {
                UnityEngine.Debug.LogWarning($"⚠️ No timer found for operation: {operationName}");
                return 0;
            }

            activeTimers[operationName].Stop();
            double elapsedMs = activeTimers[operationName].Elapsed.TotalMilliseconds;

            // Record metric
            if (!performanceMetrics.ContainsKey(operationName))
            {
                performanceMetrics[operationName] = new List<double>();
            }
            performanceMetrics[operationName].Add(elapsedMs);

            return elapsedMs;
        }

        /// <summary>
        /// Logs timing information for an operation.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <param name="elapsedMs">Elapsed time in milliseconds</param>
        /// <param name="targetMs">Target time in milliseconds (for comparison)</param>
        public static void LogTiming(string operationName, double elapsedMs, double targetMs = 0)
        {
            string message = $"⏱️ {operationName}: {elapsedMs:F2}ms";

            if (targetMs > 0)
            {
                if (elapsedMs <= targetMs)
                {
                    message += $" ✅ (Target: {targetMs}ms)";
                    UnityEngine.Debug.Log(message);
                }
                else
                {
                    message += $" ⚠️ (Target: {targetMs}ms, Exceeded by {(elapsedMs - targetMs):F2}ms)";
                    UnityEngine.Debug.LogWarning(message);
                }
            }
            else
            {
                UnityEngine.Debug.Log(message);
            }
        }

        /// <summary>
        /// Gets average performance for an operation.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Average time in milliseconds</returns>
        public static double GetAverageTime(string operationName)
        {
            if (!performanceMetrics.ContainsKey(operationName) || performanceMetrics[operationName].Count == 0)
            {
                return 0;
            }

            double sum = 0;
            foreach (double time in performanceMetrics[operationName])
            {
                sum += time;
            }

            return sum / performanceMetrics[operationName].Count;
        }

        /// <summary>
        /// Gets performance statistics for an operation.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Statistics string</returns>
        public static string GetStatistics(string operationName)
        {
            if (!performanceMetrics.ContainsKey(operationName) || performanceMetrics[operationName].Count == 0)
            {
                return $"No data for {operationName}";
            }

            List<double> times = performanceMetrics[operationName];
            times.Sort();

            double min = times[0];
            double max = times[times.Count - 1];
            double avg = GetAverageTime(operationName);
            double median = times[times.Count / 2];

            return $"{operationName} Statistics:\n" +
                   $"  Samples: {times.Count}\n" +
                   $"  Min: {min:F2}ms\n" +
                   $"  Max: {max:F2}ms\n" +
                   $"  Avg: {avg:F2}ms\n" +
                   $"  Median: {median:F2}ms";
        }

        /// <summary>
        /// Clears all performance metrics.
        /// </summary>
        public static void ClearMetrics()
        {
            performanceMetrics.Clear();
            activeTimers.Clear();
        }

        /// <summary>
        /// Logs all collected performance statistics.
        /// </summary>
        public static void LogAllStatistics()
        {
            if (performanceMetrics.Count == 0)
            {
                UnityEngine.Debug.Log("📊 No performance metrics collected");
                return;
            }

            UnityEngine.Debug.Log("📊 Performance Statistics:");
            foreach (string operationName in performanceMetrics.Keys)
            {
                UnityEngine.Debug.Log(GetStatistics(operationName));
            }
        }

        /// <summary>
        /// Measures memory allocation during an operation.
        /// </summary>
        /// <returns>Memory allocated in MB</returns>
        public static long GetCurrentMemoryUsage()
        {
            return System.GC.GetTotalMemory(false) / (1024 * 1024); // Convert to MB
        }

        /// <summary>
        /// Logs memory usage information.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <param name="memoryBefore">Memory before operation (MB)</param>
        /// <param name="memoryAfter">Memory after operation (MB)</param>
        /// <param name="targetMB">Target memory usage (MB)</param>
        public static void LogMemoryUsage(string operationName, long memoryBefore, long memoryAfter, long targetMB = 10)
        {
            long allocated = memoryAfter - memoryBefore;
            string message = $"💾 {operationName}: {allocated}MB allocated";

            if (allocated <= targetMB)
            {
                message += $" ✅ (Target: {targetMB}MB)";
                UnityEngine.Debug.Log(message);
            }
            else
            {
                message += $" ⚠️ (Target: {targetMB}MB, Exceeded by {(allocated - targetMB)}MB)";
                UnityEngine.Debug.LogWarning(message);
            }
        }
    }
}
