using UnityEngine;
using TMPro;

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

    void HandleLog(string logString, string stackTrace, LogType type)
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
    }
}