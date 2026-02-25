using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    // Singleton-Instanz
    public static SaveManager Instance { get; private set; }

    private string savePath;

    void Awake()
    {
        // Singleton-Logik
        if (Instance == null)
        {
            Instance = this;
            // Der Pfad wird einmalig beim Start festgelegt
            savePath = Path.Combine(Application.persistentDataPath, "uml_save.json");
            DontDestroyOnLoad(gameObject); // Optional: Behält den Manager beim Szenenwechsel
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveScene()
    {
        UMLSaveData data = new UMLSaveData();

        // Hier noch die obkecte erscahffen und wichtig deren relationship speichern
        // Das ist performanter und weniger fehleranfällig.
        foreach (BaseComponent comp in UMLManager.Instance.AlleKomponenten)
        {
            UMLObjectData nodeData = new UMLObjectData
            {
                name = comp.name,
                position = comp.transform.position,
                rotation = comp.transform.rotation,
                guid = comp.getId()                
                // Hier könntest du noch comp.infoNode.Text etc. speichern
            };
            data.allObjects.Add(nodeData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"<color=green>Daten erfolgreich gesichert in: {savePath}</color>");
    }

    public void LoadScene()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Kein Savegame gefunden!");
            return;
        }

        string json = File.ReadAllText(savePath);
        UMLSaveData data = JsonUtility.FromJson<UMLSaveData>(json);

        // Hier triggerst du den Neuaufbau der Szene
        foreach (var node in data.allObjects)
        {
            // hier die Realtionship auch setzten und .Setparent()
            // UMLManager.Instance.SpawnNodeFromSave(node);
        }
    }
}