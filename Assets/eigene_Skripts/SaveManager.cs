using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private ToolboxLogik saveToolbox ;//= new ToolboxLogik();
    // Singleton-Instanz
    public static SaveManager Instance { get; private set; }

    private string savePath;
    private string defaultFileName = "uml_save.json";
    private string saveFileName = "uml_save.json";
    private string loadFileName = "uml_save.json";
    [SerializeField] private TMP_InputField inputField;
     [SerializeField] private TMP_Dropdown dropDown;
     /*
     abfrage ob es als Singleton exestiert und wenn nicht zerstört es sich
     legt den Speicerord in der Aplikation fest beim erwachen
     */
    void Awake()
    {
        // Singleton-Logik
        if (Instance == null)
        {
            Instance = this;
            // Der Pfad wird einmalig beim Start festgelegt
            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            DontDestroyOnLoad(gameObject); // Optional: Behält den Manager beim Szenenwechsel


            // heir alle optionen einlesen aud der file und hinzufügen
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void changeSaveFileName()
    {
        saveFileName = inputField.text;
        // hier noch ganzen path ändern
    }

    public void changeLoadFile()
    {
        int index = dropDown.value;
        string selectedFilename = dropDown.options[index].text;
        if (selectedFilename == "default")
        {
            loadFileName = defaultFileName;
        }
        else
        {
            loadFileName = selectedFilename;
        }

        // hier noch den ganzen path ändern
    }

    /*
    Speichert alle Daten der Connections und Komponentnen in einer Liste, getrennt nach Komponenten und Connections
    mit JSONUtility wird alles in eine Stringdatei umgewandelt im JSON-Format
    */
    public void SaveScene()
    {
        UMLSaveData data = new UMLSaveData();

        // Hier noch die obkecte erscahffen und wichtig deren relationship speichern
        // Das ist performanter und weniger fehleranfällig.
        foreach (BaseComponent comp in UMLManager.Instance.AlleKomponenten)
        {
            string parent = "none";
            
            if(comp.transform.parent != null)
            {
                BaseComponent parentComp = comp.transform.parent.GetComponentInParent<BaseComponent>();
                parent = parentComp.getId();
                
            }
            UMLObjectData nodeData = new UMLObjectData
            {
                name = comp.ReadInformationNode(1),
                position = comp.transform.localPosition,
                rotation = comp.transform.rotation,
                guid = comp.getId(),              
                responsability = comp.ReadInformationNode(3),
                description = comp.ReadInformationNode(2),
                parentGuid = parent,
                // Hier könntest du noch comp.infoNode.Text etc. speichern
            };
            data.allObjects.Add(nodeData);
        }

        foreach (UMLLineController line in UMLConnectionBuilder.Instance.AllConnections)
        {
            BaseComponent AnchorAParent = line.anchorA.GetComponentInParent<BaseComponent>();
            BaseComponent AnchorBParent = line.anchorB.GetComponentInParent<BaseComponent>();
            string positionA = AnchorAParent.ReadAnchorPosition(line.anchorA);
            string positionB = AnchorBParent.ReadAnchorPosition(line.anchorB);
            UMLLineData lineData = new UMLLineData
            {
                connectionType = line.GetConnectionType(),
                parentA = AnchorAParent.getId(),
                anchorA = AnchorAParent.ReadAnchorPosition(line.anchorA),
                parentB = AnchorBParent.getId(),
                anchorB = AnchorBParent.ReadAnchorPosition(line.anchorB)
            };
            data.allLines.Add(lineData);
        }
        //Debug.Log("Alle iteriert");
        string json = JsonUtility.ToJson(data, true);
        //Debug.Log("toJson ausgeführt. \n" + json);
        File.WriteAllText(savePath, json);
        Debug.Log($"<color=green>Daten erfolgreich gesichert in: {savePath}</color>");
    }


    /*
    Prüft ob die Datei exestiert und fehlerfreifrei gelesen werden kann
    wenn ja, dann wird über die Toolbox alle elemente erstellt und zwischenzeitlich über eine Map gespeichert mit der UID als Key
    und anschließend die Hierarchie wieder hergestellt 
    */
    public void LoadScene()
    {
    if (!File.Exists(savePath))
    {
        Debug.LogWarning("Keine Speicherdatei gefunden.");
        return;
    }

    // 1. Bestehende Szene bereinigen
   

    // 2. Daten einlesen
    string json = File.ReadAllText(savePath);
    UMLSaveData data;
    try 
    {
        data = JsonUtility.FromJson<UMLSaveData>(json);
        if (data == null || data.allObjects == null)
        {
            Debug.LogError("Speicherdatei enthält keine validen Objektdaten.");
            return;
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError($"JSON-Struktur korrupt: {e.Message}");
        return; // Abbrechen, um Folgefehler zu vermeiden
    }


    UMLManager.Instance.ClearScene(); 
    UMLConnectionBuilder.Instance.ClearAllConnections();
    // 3. Komponenten instanziieren
    // Wir speichern die neuen Instanzen in einem Dictionary, 
    // um später schnell über die GUID auf sie zuzugreifen.
    Dictionary<string, BaseComponent> createdComponents = new Dictionary<string, BaseComponent>();
   
    foreach (UMLObjectData objData in data.allObjects)
    {
        if (string.IsNullOrEmpty(objData.guid)|| createdComponents.ContainsKey(objData.guid)) continue;
        // Nutzt deinen vorhandenen Mechanismus zum Spawnen
        BaseComponent newComp = saveToolbox.SpawnPrefab() ;
           
        
        if(newComp == null) Debug.Log("<color=red>ünull zurückbekommen</color>");
        // Daten zurückschreiben
        newComp.SetPosition(objData.position);
        newComp.SetRotation(objData.rotation);
                
        newComp.setId(objData.guid);
        newComp.UpdateInfoNode(objData.name, objData.responsability, objData.description);
                
        createdComponents.Add(objData.guid, newComp);  
    }


    // 4. Hierarchie wiederherstellen (Parenting)
    foreach (UMLObjectData objData in data.allObjects)
    {
        if (!createdComponents.ContainsKey(objData.guid)) continue;
        BaseComponent childComp = createdComponents[objData.guid];

        if (objData.parentGuid != "none" && createdComponents.TryGetValue(objData.parentGuid, out BaseComponent parentComp))
        {
            Debug.Log("<color=red>überhaupt ein child gefunden</color>");
            //BaseComponent parentComp = createdComponents[objData.parentGuid];
            
            parentComp.AddChild(createdComponents[objData.guid]);
            childComp.gameObject.transform.SetParent(parentComp.gameObject.transform, false);
            childComp.transform.localPosition = Vector3.zero;
            if(childComp.transform.IsChildOf(parentComp.transform)) Debug.Log("<color=red>is Child jetzt</color>");
            if(childComp.transform.parent == parentComp.transform) Debug.Log("<color=red>parent gesetzt</color>");
            childComp.SetPosition(objData.position);
            childComp.SetRotation(objData.rotation);
            childComp.SetScale(0.2f);


        }
    }

    // 5. Verbindungen (Lines) wiederherstellen
    foreach (UMLLineData lineData in data.allLines)
    {
        if (createdComponents.ContainsKey(lineData.parentA) && createdComponents.ContainsKey(lineData.parentB))
        {
            BaseComponent compA = createdComponents[lineData.parentA];
            BaseComponent compB = createdComponents[lineData.parentB];

            // Anker-Referenzen über die gespeicherte Position finden
            Transform anchorA = compA.GetAnchorByPosition(lineData.anchorA);
            Transform anchorB = compB.GetAnchorByPosition(lineData.anchorB);

            if (anchorA != null && anchorB != null)
            {
                // Nutzt deinen Builder, um die physische Linie zu erzeugen
                UMLConnectionBuilder.Instance.CreateConnection(anchorA, anchorB, lineData.connectionType);
                
            }

        }else
        {
            Debug.LogWarning($"Verbindung übersprungen: Eine der Komponenten (A: {lineData.parentA} oder B: {lineData.parentB}) fehlt.");
                
        }
            
    }

    Debug.Log($"<color=cyan>Szene erfolgreich aus {savePath} geladen.</color>");
}
}