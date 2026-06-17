using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace VirtueSky.Tracking.Editor
{
    /// <summary>
    /// Google Sheets Integration via Apps Script
    /// Uses HTTP POST to Apps Script Web App endpoint
    /// </summary>
    public class GoogleSheetsUploader
    {
        private string appsScriptUrl;
        private static bool isUploading = false;

        public GoogleSheetsUploader(string appsScriptUrl)
        {
            this.appsScriptUrl = appsScriptUrl;
        }

        /// <summary>
        /// Upload tracking analysis data to Google Sheets via Apps Script
        /// Non-blocking version using Editor callback
        /// </summary>
        public void UploadAnalysisAsync(TrackingAnalysisResult analysisResult, System.Action<bool, string> onComplete)
        {
            if (isUploading)
            {
                onComplete?.Invoke(false, "Already uploading...");
                return;
            }

            isUploading = true;

            Task.Run(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(appsScriptUrl))
                    {
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            isUploading = false;
                            onComplete?.Invoke(false, "Apps Script URL is not configured");
                        });
                        return;
                    }

                    // Prepare JSON data
                    string jsonData = PrepareJsonData(analysisResult);

                    // Send POST request
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = System.TimeSpan.FromSeconds(30);

                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(appsScriptUrl, content);

                        string responseText = await response.Content.ReadAsStringAsync();

                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            isUploading = false;

                            if (!response.IsSuccessStatusCode)
                            {
                                Debug.LogError($"HTTP Error {response.StatusCode}: {responseText}");
                                onComplete?.Invoke(false, $"HTTP {response.StatusCode}: {responseText}");
                            }
                            else
                            {
                                Debug.Log($"Upload successful! Response: {responseText}");
                                onComplete?.Invoke(true, "Upload successful!");
                            }
                        });
                    }
                }
                catch (System.Exception e)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        isUploading = false;
                        Debug.LogError($"Failed to upload to Google Sheets: {e.Message}");
                        onComplete?.Invoke(false, e.Message);
                    });
                }
            });
        }

        /// <summary>
        /// Prepare JSON data in format expected by Apps Script
        /// Manual JSON building to avoid JsonUtility limitations
        /// </summary>
        private string PrepareJsonData(TrackingAnalysisResult analysisResult)
        {
            var sb = new StringBuilder();
            sb.Append("{");

            // Timestamp
            sb.Append($"\"timestamp\":\"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}\",");

            // Summary
            sb.Append("\"summary\":{");
            sb.Append($"\"totalEvents\":{analysisResult.totalEvents},");
            sb.Append($"\"usedEvents\":{analysisResult.usedEvents.Count},");
            sb.Append($"\"unusedEvents\":{analysisResult.unusedEvents.Count}");
            sb.Append("},");

            // Events array
            sb.Append("\"events\":[");

            bool first = true;

            // Add used events
            foreach (var evt in analysisResult.usedEvents.OrderBy(e => e.Key))
            {
                if (!first) sb.Append(",");
                first = false;

                string filePaths = GetEventDefinitionFilePath(analysisResult, evt.Key);
                string category = GetEventCategory(evt.Key);

                sb.Append("{");
                sb.Append($"\"name\":\"{EscapeJson(evt.Key)}\",");
                sb.Append($"\"category\":\"{EscapeJson(category)}\",");
                sb.Append($"\"status\":\"✓ Used\",");
                sb.Append($"\"usageCount\":{evt.Value.Count},");
                sb.Append($"\"filePaths\":\"{EscapeJson(filePaths)}\"");
                sb.Append("}");
            }

            // Add unused events
            foreach (var evt in analysisResult.unusedEvents.OrderBy(e => e))
            {
                if (!first) sb.Append(",");
                first = false;

                string category = GetEventCategory(evt);
                string filePaths = GetEventDefinitionFilePath(analysisResult, evt);

                sb.Append("{");
                sb.Append($"\"name\":\"{EscapeJson(evt)}\",");
                sb.Append($"\"category\":\"{EscapeJson(category)}\",");
                sb.Append($"\"status\":\"✗ Unused\",");
                sb.Append($"\"usageCount\":0,");
                sb.Append($"\"filePaths\":\"{EscapeJson(filePaths)}\"");
                sb.Append("}");
            }

            sb.Append("]");
            sb.Append("}");

            string json = sb.ToString();
            Debug.Log($"Prepared JSON ({json.Length} chars):\n{json.Substring(0, System.Math.Min(500, json.Length))}...");
            return json;
        }

        /// <summary>
        /// Escape special characters for JSON
        /// </summary>
        private string EscapeJson(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        private string GetEventDefinitionFilePath(TrackingAnalysisResult analysisResult, string eventName)
        {
            if (analysisResult.eventDefinitions != null &&
                analysisResult.eventDefinitions.TryGetValue(eventName, out var definition))
            {
                return definition.filePath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get event category from event name
        /// </summary>
        private string GetEventCategory(string eventName)
        {
            int underscoreIndex = eventName.IndexOf('_');
            return underscoreIndex > 0 ? eventName.Substring(0, underscoreIndex).ToUpper() : eventName.ToUpper();
        }
    }

    /// <summary>
    /// Serializable data structures for JSON upload
    /// </summary>
    [System.Serializable]
    public class UploadData
    {
        public string timestamp;
        public SummaryData summary;
        public List<EventData> events;
    }

    [System.Serializable]
    public class SummaryData
    {
        public int totalEvents;
        public int usedEvents;
        public int unusedEvents;
    }

    [System.Serializable]
    public class EventData
    {
        public string name;
        public string category;
        public string status;
        public int usageCount;
        public string filePaths;
    }
}