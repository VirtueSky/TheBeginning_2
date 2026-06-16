using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace VirtueSky.Tracking.Editor
{
    public class TrackingAnalyzerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string appsScriptUrl = "";
        private string googleSheetUrl = "";
        private bool allowEditUrls = false;
        private bool includeUnusedEvents = true;
        private bool includeUnusedParams = true;

        private TrackingAnalysisResult analysisResult;
        private bool isAnalyzing = false;

        private const string GoogleAppsScriptTemplate =
            @"/**
 * Google Apps Script for Tracking Analyzer Integration
 *
 * HOW TO USE:
 * 1. Open your Google Sheet
 * 2. Extensions → Apps Script
 * 3. Paste this code
 * 4. Click Deploy → New Deployment → Web App
 * 5. Settings:
 *    - Execute as: Me
 *    - Who has access: Anyone
 * 6. Copy the Web App URL
 * 7. Paste into Unity Tracking Analyzer tool
 */

// Configuration
const SHEET_NAME = ""Tracking Analysis""; // Tên sheet để ghi data
const CLEAR_BEFORE_UPDATE = true; // Xóa data cũ trước khi update

/**
 * Handle POST requests from Unity
 */
function doPost(e) {
  try {
    // Parse JSON data
    const data = JSON.parse(e.postData.contents);

    // Get or create sheet
    const sheet = getOrCreateSheet(SHEET_NAME);

    // Clear existing data if configured
    if (CLEAR_BEFORE_UPDATE) {
      sheet.clear();
    }

    // Write data
    writeTrackingData(sheet, data);

    // Format sheet
    formatSheet(sheet);

    return ContentService
      .createTextOutput(JSON.stringify({
        success: true,
        message: ""Data uploaded successfully"",
        timestamp: new Date().toISOString(),
        rowsWritten: data.events.length + 1 // +1 for header
      }))
      .setMimeType(ContentService.MimeType.JSON);

  } catch (error) {
    Logger.log(""Error: "" + error.toString());

    return ContentService
      .createTextOutput(JSON.stringify({
        success: false,
        error: error.toString()
      }))
      .setMimeType(ContentService.MimeType.JSON);
  }
}

/**
 * Get existing sheet or create new one
 */
function getOrCreateSheet(sheetName) {
  const spreadsheet = SpreadsheetApp.getActiveSpreadsheet();
  let sheet = spreadsheet.getSheetByName(sheetName);

  if (!sheet) {
    sheet = spreadsheet.insertSheet(sheetName);
  }

  return sheet;
}

/**
 * Write tracking data to sheet
 */
function writeTrackingData(sheet, data) {
  // Prepare rows
  const rows = [];

  // Header row
  rows.push([
    ""Event Name"",
    ""Category"",
    ""Status"",
    ""Usage Count"",
    ""File Paths"",
    ""Last Updated""
  ]);

  // Data rows
  data.events.forEach(event => {
    rows.push([
      event.name,
      event.category,
      event.status,
      event.usageCount,
      event.filePaths,
      data.timestamp
    ]);
  });

  // Write to sheet
  sheet.getRange(1, 1, rows.length, rows[0].length).setValues(rows);

  // Add summary at the top (as note or separate area)
  addSummarySection(sheet, data);
}

/**
 * Add summary statistics section
 */
function addSummarySection(sheet, data) {
  // Insert rows at top for summary
  sheet.insertRowsBefore(1, 5);

  // Write summary
  sheet.getRange(""A1"").setValue(""TRACKING ANALYSIS SUMMARY"");
  sheet.getRange(""A2"").setValue(""Generated:"");
  sheet.getRange(""B2"").setValue(data.timestamp);
  sheet.getRange(""A3"").setValue(""Total Events:"");
  sheet.getRange(""B3"").setValue(data.summary.totalEvents);
  sheet.getRange(""A4"").setValue(""Used Events:"");
  sheet.getRange(""B4"").setValue(data.summary.usedEvents);
  sheet.getRange(""A5"").setValue(""Unused Events:"");
  sheet.getRange(""B5"").setValue(data.summary.unusedEvents);

  // Style summary section
  sheet.getRange(""A1:B5"").setBackground(""#f3f3f3"");
  sheet.getRange(""A1:B1"").setFontWeight(""bold"").setFontSize(12);

  // Add empty row separator
  sheet.insertRowsAfter(5, 1);
}

/**
 * Format sheet for better readability
 */
function formatSheet(sheet) {
  // Header row (row 7 after summary)
  const headerRow = 7;
  const headerRange = sheet.getRange(headerRow, 1, 1, 6);

  headerRange.setBackground(""#4285f4"");
  headerRange.setFontColor(""#ffffff"");
  headerRange.setFontWeight(""bold"");
  headerRange.setHorizontalAlignment(""center"");

  // Freeze header rows
  sheet.setFrozenRows(headerRow);

  // Auto-resize columns
  for (let i = 1; i <= 6; i++) {
    sheet.autoResizeColumn(i);
  }

  // Set column widths
  sheet.setColumnWidth(1, 200); // Event Name
  sheet.setColumnWidth(2, 100); // Category
  sheet.setColumnWidth(3, 100); // Status
  sheet.setColumnWidth(4, 100); // Usage Count
  sheet.setColumnWidth(5, 400); // File Paths
  sheet.setColumnWidth(6, 150); // Last Updated

  // Add borders
  const dataRange = sheet.getDataRange();
  dataRange.setBorder(true, true, true, true, true, true);

  // Conditional formatting for status
  addConditionalFormatting(sheet, headerRow);
}

/**
 * Add conditional formatting for status column
 */
function addConditionalFormatting(sheet, headerRow) {
  const lastRow = sheet.getLastRow();
  const statusColumn = 3; // Column C

  if (lastRow <= headerRow) return;

  const statusRange = sheet.getRange(headerRow + 1, statusColumn, lastRow - headerRow, 1);

  // Rule for ""Used"" status - green background
  const usedRule = SpreadsheetApp.newConditionalFormatRule()
    .whenTextContains(""Used"")
    .setBackground(""#d9ead3"")
    .setRanges([statusRange])
    .build();

  // Rule for ""Unused"" status - red background
  const unusedRule = SpreadsheetApp.newConditionalFormatRule()
    .whenTextContains(""Unused"")
    .setBackground(""#f4cccc"")
    .setRanges([statusRange])
    .build();

  const rules = sheet.getConditionalFormatRules();
  rules.push(usedRule);
  rules.push(unusedRule);
  sheet.setConditionalFormatRules(rules);
}

/**
 * Test function - call this to verify script works
 */
function testWithSampleData() {
  const sampleData = {
    timestamp: new Date().toISOString(),
    summary: {
      totalEvents: 10,
      usedEvents: 7,
      unusedEvents: 3
    },
    events: [
      {
        name: ""song_start"",
        category: ""Song"",
        status: ""\u2713 Used"",
        usageCount: 5,
        filePaths: ""GameManager.cs:120\nLevelLoader.cs:45""
      },
      {
        name: ""song_complete"",
        category: ""Song"",
        status: ""\u2713 Used"",
        usageCount: 3,
        filePaths: ""GameManager.cs:250""
      },
      {
        name: ""song_fail"",
        category: ""Song"",
        status: ""\u2717 Unused"",
        usageCount: 0,
        filePaths: """"
      }
    ]
  };

  const sheet = getOrCreateSheet(SHEET_NAME);
  sheet.clear();
  writeTrackingData(sheet, sampleData);
  formatSheet(sheet);

  Logger.log(""Test completed successfully!"");
}";

        [MenuItem("TheBeginning_2/Tracking Analyzer", priority = 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<TrackingAnalyzerWindow>("Tracking Analyzer");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void OnEnable()
        {
            LoadPreferences();
        }

        private void OnDisable()
        {
            SavePreferences();
        }

        private void LoadPreferences()
        {
            appsScriptUrl = EditorPrefs.GetString("TrackingAnalyzer_AppsScriptUrl", "");
            googleSheetUrl = EditorPrefs.GetString("TrackingAnalyzer_GoogleSheetUrl", "");
            allowEditUrls = EditorPrefs.GetBool("TrackingAnalyzer_AllowEditUrls", false);
            includeUnusedEvents = EditorPrefs.GetBool("TrackingAnalyzer_IncludeUnused", true);
            includeUnusedParams = EditorPrefs.GetBool("TrackingAnalyzer_IncludeUnusedParams", true);
        }

        private void SavePreferences()
        {
            EditorPrefs.SetString("TrackingAnalyzer_AppsScriptUrl", appsScriptUrl);
            EditorPrefs.SetString("TrackingAnalyzer_GoogleSheetUrl", googleSheetUrl);
            EditorPrefs.SetBool("TrackingAnalyzer_AllowEditUrls", allowEditUrls);
            EditorPrefs.SetBool("TrackingAnalyzer_IncludeUnused", includeUnusedEvents);
            EditorPrefs.SetBool("TrackingAnalyzer_IncludeUnusedParams", includeUnusedParams);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Tracking Analyzer", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Tool này sẽ scan toàn bộ code để tìm tracking events đang được sử dụng.", MessageType.Info);

            GUILayout.Space(10);
            DrawSettingsSection();

            GUILayout.Space(10);
            DrawAnalysisSection();

            GUILayout.Space(10);
            DrawResultsSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawSettingsSection()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Google Apps Script Configuration", EditorStyles.miniBoldLabel);
            EditorGUILayout.HelpBox(
                "Setup:\n" +
                "1. Open your Google Sheet\n" +
                "2. Extensions → Apps Script\n" +
                "3. Click 'Copy Apps Script Template' below and paste the code\n" +
                "4. Deploy → New Deployment → Web App (Execute as: Me, Access: Anyone)\n" +
                "5. Copy Web App URL and paste below",
                MessageType.Info
            );

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("📋 Copy Apps Script Template", GUILayout.Width(220)))
            {
                GUIUtility.systemCopyBuffer = GoogleAppsScriptTemplate;
                EditorUtility.DisplayDialog("Copied!", "Google Apps Script template copied to clipboard.\nPaste it into Apps Script editor.", "OK");
            }

            EditorGUILayout.EndHorizontal();

            allowEditUrls = EditorGUILayout.Toggle("Edit URLs", allowEditUrls);

            GUI.enabled = allowEditUrls;
            appsScriptUrl = EditorGUILayout.TextField("Apps Script URL", appsScriptUrl);
            googleSheetUrl = EditorGUILayout.TextField("Google Sheet URL", googleSheetUrl);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = !string.IsNullOrWhiteSpace(googleSheetUrl);
            if (GUILayout.Button("Open Sheet", GUILayout.Width(120)))
            {
                OpenGoogleSheet();
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Analysis Options", EditorStyles.miniBoldLabel);
            includeUnusedEvents = EditorGUILayout.Toggle("Include Unused Events", includeUnusedEvents);
            includeUnusedParams = EditorGUILayout.Toggle("Include Unused Params", includeUnusedParams);

            EditorGUILayout.EndVertical();
        }

        private void DrawAnalysisSection()
        {
            EditorGUILayout.LabelField("Analysis", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = !isAnalyzing;
            if (GUILayout.Button("Analyze Code", GUILayout.Height(30)))
            {
                AnalyzeTrackingUsage();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Export Options:", EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = analysisResult != null && !isAnalyzing;
            if (GUILayout.Button("Export to CSV", GUILayout.Height(25)))
            {
                ExportToCSV();
            }

            if (GUILayout.Button("Export Detailed CSV", GUILayout.Height(25)))
            {
                ExportDetailedCSV();
            }

            if (GUILayout.Button("Export Summary Report", GUILayout.Height(25)))
            {
                ExportSummaryReport();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = analysisResult != null && !isAnalyzing && !string.IsNullOrEmpty(appsScriptUrl);
            if (GUILayout.Button("📤 Upload to Google Sheets", GUILayout.Height(30)))
            {
                UploadToGoogleSheets();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            if (isAnalyzing)
            {
                EditorGUILayout.HelpBox("Analyzing... Please wait.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawResultsSection()
        {
            if (analysisResult == null) return;

            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Total Events Found: {analysisResult.totalEvents}");
            EditorGUILayout.LabelField($"Used Events: {analysisResult.usedEvents.Count}");
            EditorGUILayout.LabelField($"Unused Events: {analysisResult.unusedEvents.Count}");
            EditorGUILayout.LabelField($"Total Params Found: {analysisResult.totalParams}");

            GUILayout.Space(10);

            if (analysisResult.usedEvents.Count > 0)
            {
                EditorGUILayout.LabelField("Used Events:", EditorStyles.boldLabel);
                foreach (var evt in analysisResult.usedEvents.OrderBy(e => e.Key))
                {
                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField($"✓ {evt.Key}", GUILayout.Width(300));
                    EditorGUILayout.LabelField($"Used {evt.Value.Count} times", GUILayout.Width(150));
                    if (GUILayout.Button("Show Usages", GUILayout.Width(100)))
                    {
                        ShowEventUsages(evt.Key, evt.Value);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (includeUnusedEvents && analysisResult.unusedEvents.Count > 0)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Unused Events:", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("Các events này đã được định nghĩa nhưng chưa được sử dụng trong code.", MessageType.Warning);

                foreach (var evt in analysisResult.unusedEvents.OrderBy(e => e))
                {
                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField($"✗ {evt}", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void AnalyzeTrackingUsage()
        {
            isAnalyzing = true;
            analysisResult = new TrackingAnalysisResult();

            try
            {
                // Get all defined events and params from enums
                var allEvents = GetAllTrackingEvents();
                var allParams = GetAllTrackingParams();
                var eventDefinitions = GetTrackingEventDefinitions();

                analysisResult.totalEvents = allEvents.Count;
                analysisResult.totalParams = allParams.Count;
                analysisResult.eventDefinitions = eventDefinitions;

                var eventMethodNames = eventDefinitions
                    .Where(kvp => !string.IsNullOrEmpty(kvp.Value.methodName))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.methodName);

                // Scan all C# files
                string[] scriptFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

                foreach (string file in scriptFiles)
                {
                    // Skip tracking definition files
                    if (file.Contains("Trackings.") || file.Contains("TrackEnum")) continue;

                    string content = File.ReadAllText(file);

                    // Find tracking method calls
                    foreach (string eventName in allEvents)
                    {
                        string resolvedMethodName = eventMethodNames.TryGetValue(eventName, out var definedMethodName)
                            ? definedMethodName
                            : ConvertToPascalCase(eventName);
                        string methodName = Regex.Escape(resolvedMethodName);
                        string pattern = $@"Trackings\.{methodName}\b";
                        var matches = Regex.Matches(content, pattern);

                        if (matches.Count > 0)
                        {
                            if (!analysisResult.usedEvents.ContainsKey(eventName))
                            {
                                analysisResult.usedEvents[eventName] = new List<TrackingUsage>();
                            }

                            foreach (Match match in matches)
                            {
                                analysisResult.usedEvents[eventName].Add(new TrackingUsage
                                {
                                    filePath = file.Replace(Application.dataPath, "Assets"),
                                    lineNumber = GetLineNumber(content, match.Index)
                                });
                            }
                        }
                    }
                }

                // Find unused events
                foreach (string eventName in allEvents)
                {
                    if (!analysisResult.usedEvents.ContainsKey(eventName))
                    {
                        analysisResult.unusedEvents.Add(eventName);
                    }
                }

                Debug.Log(
                    $"Analysis complete! Found {analysisResult.usedEvents.Count} used events and {analysisResult.unusedEvents.Count} unused events.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Analysis failed: {e.Message}");
            }
            finally
            {
                isAnalyzing = false;
            }
        }

        private void ExportToCSV()
        {
            if (analysisResult == null)
            {
                EditorUtility.DisplayDialog("Error", "Please run analysis first!", "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanel("Export Tracking Analysis", "", "tracking_analysis.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    // Header
                    writer.WriteLine("Event Name,Status,Usage Count,File Paths");

                    // Used events
                    foreach (var evt in analysisResult.usedEvents.OrderBy(e => e.Key))
                    {
                        string filePaths = GetEventDefinitionFilePath(evt.Key);
                        writer.WriteLine($"{evt.Key},Used,{evt.Value.Count},\"{filePaths}\"");
                    }

                    // Unused events
                    if (includeUnusedEvents)
                    {
                        foreach (var evt in analysisResult.unusedEvents.OrderBy(e => e))
                        {
                            string filePaths = GetEventDefinitionFilePath(evt);
                            writer.WriteLine($"{evt},Unused,0,\"{filePaths}\"");
                        }
                    }
                }

                EditorUtility.DisplayDialog("Success", $"Exported to {path}", "OK");
                System.Diagnostics.Process.Start(Path.GetDirectoryName(path));
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Export failed: {e.Message}", "OK");
            }
        }

        private void UploadToGoogleSheets()
        {
            if (analysisResult == null)
            {
                EditorUtility.DisplayDialog("Error", "Please run analysis first!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(appsScriptUrl))
            {
                EditorUtility.DisplayDialog("Error", "Please configure Apps Script URL in Settings!", "OK");
                return;
            }

            // Show progress bar (will be cleared by callback)
            EditorUtility.DisplayProgressBar("Uploading", "Uploading to Google Sheets...", 0.5f);

            var uploader = new GoogleSheetsUploader(appsScriptUrl);
            uploader.UploadAnalysisAsync(analysisResult, (success, message) =>
            {
                EditorUtility.ClearProgressBar();

                if (success)
                {
                    EditorUtility.DisplayDialog("Success",
                        "✅ Data uploaded to Google Sheets successfully!\n\nRefresh your Google Sheet to see the results.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", $"Upload failed:\n{message}", "OK");
                }
            });
        }

        private void OpenGoogleSheet()
        {
            if (string.IsNullOrWhiteSpace(googleSheetUrl))
            {
                EditorUtility.DisplayDialog("Error", "Please configure Google Sheet URL in Settings!", "OK");
                return;
            }

            Application.OpenURL(googleSheetUrl);
        }

        private void ExportDetailedCSV()
        {
            if (analysisResult == null)
            {
                EditorUtility.DisplayDialog("Error", "Please run analysis first!", "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanel("Export Detailed Analysis", "", "tracking_analysis_detailed.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                TrackingExporter.ExportDetailedCSV(analysisResult, path);
                EditorUtility.DisplayDialog("Success", $"Exported detailed CSV to:\n{path}", "OK");
                System.Diagnostics.Process.Start(Path.GetDirectoryName(path));
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Export failed: {e.Message}", "OK");
            }
        }

        private void ExportSummaryReport()
        {
            if (analysisResult == null)
            {
                EditorUtility.DisplayDialog("Error", "Please run analysis first!", "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanel("Export Summary Report", "", "tracking_summary.txt", "txt");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                TrackingExporter.ExportSummaryReport(analysisResult, path);
                EditorUtility.DisplayDialog("Success", $"Exported summary report to:\n{path}", "OK");
                System.Diagnostics.Process.Start(path);
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Export failed: {e.Message}", "OK");
            }
        }

        private void ShowEventUsages(string eventName, List<TrackingUsage> usages)
        {
            string message = $"Event: {eventName}\nUsed in {usages.Count} location(s):\n\n";

            foreach (var usage in usages.Take(10))
            {
                message += $"• {usage.filePath}:{usage.lineNumber}\n";
            }

            if (usages.Count > 10)
            {
                message += $"\n... and {usages.Count - 10} more locations";
            }

            EditorUtility.DisplayDialog("Event Usages", message, "OK");
        }

        private List<string> GetAllTrackingEvents()
        {
            var events = new List<string>();
            var trackNameType = typeof(VirtueSky.Tracking.TrackName);

            foreach (var name in System.Enum.GetNames(trackNameType))
            {
                events.Add(name);
            }

            return events;
        }

        private List<string> GetAllTrackingParams()
        {
            var parameters = new List<string>();
            var trackParamType = typeof(VirtueSky.Tracking.TrackParam);

            foreach (var name in System.Enum.GetNames(trackParamType))
            {
                parameters.Add(name);
            }

            return parameters;
        }

        private Dictionary<string, TrackingDefinition> GetTrackingEventDefinitions()
        {
            var definitions = new Dictionary<string, TrackingDefinition>();
            string[] trackingFiles = Directory.GetFiles(Application.dataPath, "Trackings.*.cs", SearchOption.AllDirectories);

            foreach (string file in trackingFiles)
            {
                string content = File.ReadAllText(file);
                string assetPath = file.Replace(Application.dataPath, "Assets");

                var matches = Regex.Matches(content, @"Track\s*\(\s*(?:nameof\s*\(\s*TrackName\.(\w+)\s*\)|TrackName\.(\w+)\.ToString\s*\(\s*\))");
                foreach (Match match in matches)
                {
                    string eventName = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                    if (definitions.ContainsKey(eventName))
                    {
                        continue;
                    }

                    definitions[eventName] = new TrackingDefinition
                    {
                        eventName = eventName,
                        filePath = assetPath,
                        lineNumber = GetLineNumber(content, match.Index),
                        className = GetEnclosingClassName(content, match.Index),
                        methodName = GetEnclosingMethodName(content, match.Index)
                    };
                }
            }

            return definitions;
        }

        private string GetEnclosingClassName(string content, int index)
        {
            string beforeMatch = content.Substring(0, index);
            var classMatches = Regex.Matches(beforeMatch, @"class\s+(\w+)");
            if (classMatches.Count == 0)
            {
                return string.Empty;
            }

            return classMatches[classMatches.Count - 1].Groups[1].Value;
        }

        private string GetEnclosingMethodName(string content, int index)
        {
            string beforeMatch = content.Substring(0, index);
            var methodMatches = Regex.Matches(beforeMatch,
                @"(?:public|private|protected|internal)\s+(?:static\s+)?(?:async\s+)?[\w<>\[\],\s]+\s+(\w+)\s*\(");
            if (methodMatches.Count == 0)
            {
                return string.Empty;
            }

            return methodMatches[methodMatches.Count - 1].Groups[1].Value;
        }

        private string GetEventDefinitionFilePath(string eventName)
        {
            if (analysisResult?.eventDefinitions != null &&
                analysisResult.eventDefinitions.TryGetValue(eventName, out var definition))
            {
                return definition.filePath;
            }

            return string.Empty;
        }

        private string ConvertToPascalCase(string snakeCase)
        {
            string[] parts = snakeCase.Split('_');
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
            }

            return string.Join("", parts);
        }

        private int GetLineNumber(string content, int index)
        {
            return content.Substring(0, index).Split('\n').Length;
        }
    }

    public class TrackingAnalysisResult
    {
        public int totalEvents;
        public int totalParams;
        public Dictionary<string, List<TrackingUsage>> usedEvents = new Dictionary<string, List<TrackingUsage>>();
        public List<string> unusedEvents = new List<string>();
        public Dictionary<string, List<TrackingUsage>> usedParams = new Dictionary<string, List<TrackingUsage>>();
        public List<string> unusedParams = new List<string>();
        public Dictionary<string, TrackingDefinition> eventDefinitions = new Dictionary<string, TrackingDefinition>();
    }

    public class TrackingUsage
    {
        public string filePath;
        public int lineNumber;
    }

    public class TrackingDefinition
    {
        public string eventName;
        public string filePath;
        public int lineNumber;
        public string className;
        public string methodName;
    }
}