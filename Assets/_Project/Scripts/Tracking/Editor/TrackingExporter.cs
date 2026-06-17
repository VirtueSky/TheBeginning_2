using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VirtueSky.Tracking.Editor
{
    /// <summary>
    /// Helper class to export tracking analysis to various formats
    /// </summary>
    public static class TrackingExporter
    {
        /// <summary>
        /// Export to detailed CSV with categories
        /// </summary>
        public static void ExportDetailedCSV(TrackingAnalysisResult result, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                // Header
                writer.WriteLine("Category,Event Name,Status,Usage Count,Defined In CSV,File Paths");

                // Group events by category
                var eventsByCategory = new Dictionary<string, List<(string name, bool used)>>();

                foreach (var evt in result.usedEvents)
                {
                    string category = GetEventCategory(evt.Key);
                    if (!eventsByCategory.ContainsKey(category))
                        eventsByCategory[category] = new List<(string, bool)>();
                    eventsByCategory[category].Add((evt.Key, true));
                }

                foreach (var evt in result.unusedEvents)
                {
                    string category = GetEventCategory(evt);
                    if (!eventsByCategory.ContainsKey(category))
                        eventsByCategory[category] = new List<(string, bool)>();
                    eventsByCategory[category].Add((evt, false));
                }

                // Write data grouped by category
                foreach (var category in eventsByCategory.Keys.OrderBy(k => k))
                {
                    foreach (var evt in eventsByCategory[category].OrderBy(e => e.name))
                    {
                        if (evt.used)
                        {
                            var usages = result.usedEvents[evt.name];
                            string filePaths = GetEventDefinitionFilePath(result, evt.name);
                            writer.WriteLine($"{category},{evt.name},Used,{usages.Count},Yes,\"{filePaths}\"");
                        }
                        else
                        {
                            string filePaths = GetEventDefinitionFilePath(result, evt.name);
                            writer.WriteLine($"{category},{evt.name},Unused,0,Yes,\"{filePaths}\"");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Export summary report
        /// </summary>
        public static void ExportSummaryReport(TrackingAnalysisResult result, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("=== TRACKING ANALYSIS SUMMARY REPORT ===");
                writer.WriteLine($"Generated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();

                writer.WriteLine("## OVERVIEW");
                writer.WriteLine($"Total Events Defined: {result.totalEvents}");
                writer.WriteLine($"Used Events: {result.usedEvents.Count} ({GetPercentage(result.usedEvents.Count, result.totalEvents)}%)");
                writer.WriteLine($"Unused Events: {result.unusedEvents.Count} ({GetPercentage(result.unusedEvents.Count, result.totalEvents)}%)");
                writer.WriteLine();

                writer.WriteLine("## BREAKDOWN BY CATEGORY");
                var categoryStats = GetCategoryStatistics(result);
                foreach (var stat in categoryStats.OrderBy(s => s.Key))
                {
                    writer.WriteLine($"\n### {stat.Key}");
                    writer.WriteLine($"  Total: {stat.Value.total}");
                    writer.WriteLine($"  Used: {stat.Value.used} ({GetPercentage(stat.Value.used, stat.Value.total)}%)");
                    writer.WriteLine($"  Unused: {stat.Value.unused} ({GetPercentage(stat.Value.unused, stat.Value.total)}%)");
                }

                writer.WriteLine("\n## MOST USED EVENTS");
                var topEvents = result.usedEvents.OrderByDescending(e => e.Value.Count).Take(10);
                foreach (var evt in topEvents)
                {
                    writer.WriteLine($"  {evt.Key}: {evt.Value.Count} times");
                }

                writer.WriteLine("\n## UNUSED EVENTS");
                if (result.unusedEvents.Count > 0)
                {
                    writer.WriteLine("The following events are defined but not used in code:");
                    foreach (var evt in result.unusedEvents.OrderBy(e => e))
                    {
                        writer.WriteLine($"  - {evt} ({GetEventCategory(evt)})");
                    }
                }
                else
                {
                    writer.WriteLine("All events are being used!");
                }
            }
        }

        /// <summary>
        /// Export to JSON format
        /// </summary>
        public static void ExportJSON(TrackingAnalysisResult result, string path)
        {
            var data = new
            {
                generatedAt = System.DateTime.Now.ToString("o"),
                summary = new
                {
                    totalEvents = result.totalEvents,
                    usedEventsCount = result.usedEvents.Count,
                    unusedEventsCount = result.unusedEvents.Count
                },
                usedEvents = result.usedEvents.Select(e => new
                {
                    name = e.Key,
                    category = GetEventCategory(e.Key),
                    usageCount = e.Value.Count,
                    usages = e.Value.Select(u => new
                    {
                        file = u.filePath,
                        line = u.lineNumber
                    }).ToArray()
                }).ToArray(),
                unusedEvents = result.unusedEvents.Select(e => new
                {
                    name = e,
                    category = GetEventCategory(e)
                }).ToArray()
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Export parameter usage analysis
        /// </summary>
        public static void ExportParameterAnalysis(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Parameter Name,Type,Used In Events,Description");

                var paramType = typeof(VirtueSky.Tracking.TrackParam);
                foreach (var paramName in System.Enum.GetNames(paramType))
                {
                    string usedIn = GetParameterUsageInfo(paramName);
                    writer.WriteLine($"{paramName},Enum,{usedIn},");
                }
            }
        }

        private static string GetEventCategory(string eventName)
        {
            int underscoreIndex = eventName.IndexOf('_');
            return underscoreIndex > 0 ? eventName.Substring(0, underscoreIndex) : eventName;
        }

        private static Dictionary<string, (int total, int used, int unused)> GetCategoryStatistics(TrackingAnalysisResult result)
        {
            var stats = new Dictionary<string, (int total, int used, int unused)>();

            foreach (var evt in result.usedEvents)
            {
                string category = GetEventCategory(evt.Key);
                if (!stats.ContainsKey(category))
                    stats[category] = (0, 0, 0);

                var current = stats[category];
                stats[category] = (current.total + 1, current.used + 1, current.unused);
            }

            foreach (var evt in result.unusedEvents)
            {
                string category = GetEventCategory(evt);
                if (!stats.ContainsKey(category))
                    stats[category] = (0, 0, 0);

                var current = stats[category];
                stats[category] = (current.total + 1, current.used, current.unused + 1);
            }

            return stats;
        }

        private static int GetPercentage(int value, int total)
        {
            if (total == 0) return 0;
            return Mathf.RoundToInt((float)value / total * 100);
        }

        private static string GetParameterUsageInfo(string paramName)
        {
            // This would require scanning the Tracking method definitions
            // For now, return placeholder
            return "Multiple events";
        }

        private static string GetEventDefinitionFilePath(TrackingAnalysisResult result, string eventName)
        {
            if (result.eventDefinitions != null &&
                result.eventDefinitions.TryGetValue(eventName, out var definition))
            {
                return definition.filePath;
            }

            return string.Empty;
        }
    }
}