using UnityEngine;
using TMPro;
  using System.Text.RegularExpressions; // WICHTIG für die Suche

public class VRTerminal : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public int maxLines = 15; // Damit das Terminal nicht unendlich lang wird
    private System.Collections.Generic.List<string> logLines = new System.Collections.Generic.List<string>();

    void OnEnable()
    {
        // Unity Events abonnieren: Immer wenn Debug.Log aufgerufen wird
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

   /* void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Farbe basierend auf Log-Typ wählen
        string color = "white";
        if (type == LogType.Error) color = "red";
        if (type == LogType.Warning) color = "yellow";

        string newLine = $"<color={color}>[{System.DateTime.Now:HH:mm:ss}] {logString}</color>";
        logLines.Add(newLine);

        // Alte Zeilen entfernen, wenn es zu voll wird
        if (logLines.Count > maxLines)
        {
            logLines.RemoveAt(0);
        }

        // Text im UI aktualisieren
        logText.text = string.Join("\n", logLines);
    }*/
  

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = "white";
        if (type == LogType.Error || type == LogType.Exception) color = "red";
        if (type == LogType.Warning) color = "yellow";

        string filePath = "Unknown Source";
        
        if (!string.IsNullOrEmpty(stackTrace))
        {
            // Regex sucht nach dem Muster: (at Assets/...:Zeile)
            // Das Pattern findet den Pfad zwischen "at " und ")"
            Match match = Regex.Match(stackTrace, @"\(at (Assets/.*?)\)");
            if (match.Success)
            {
                filePath = match.Groups[1].Value; // Der gefundene Pfad + Zeile
            }
            else
            {
                // Fallback: Falls Regex nicht matcht, nimm die erste Zeile
                string[] lines = stackTrace.Split('\n');
                if (lines.Length > 0) filePath = lines[0].Trim();
            }
        }

        // Formatierung für das UI
        string time = System.DateTime.Now.ToString("HH:mm:ss");
        string sourceInfo = $"\n<size=75%><color=#888888><i>Location: {filePath}</i></color></size>";
        
        string newLine = $"<color={color}>[{time}] {logString}</color>{sourceInfo}";
        
        logLines.Add(newLine);

        if (logLines.Count > maxLines)
        {
            logLines.RemoveAt(0);
        }

        logText.text = string.Join("\n", logLines);
    }
}