using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UMLConnectionBuilder : MonoBehaviour
{
    public static UMLConnectionBuilder Instance { get; private set; }
    public UMLAnchor firstAnchor;
    [SerializeField] private GameObject linePrefab; // Ein Prefab mit dem UMLLineController
    [SerializeField] private LineRenderer previewLine; // Eine visuelle Linie während des Ziehens
    [SerializeField] private GameObject previewConenction;
    
    public InputActionReference cancelActionLeft;
    public InputActionReference cancelActionRight; 
    private List<UMLLineController> AllConnections = new List<UMLLineController>();

     void Awake()
    {
        // Sicherstellen, dass es nur eine Instanz gibt
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
       
    }

    public void SetGlobalVisibility(bool visible, BaseComponent focusObject)
    {
        // Finde alle BaseComponents in der Szene
        UMLLineController[] allConns = FindObjectsOfType<UMLLineController>(true);
        Debug.Log("SetGlobalVisibility ausgeführt, gefundene UMLConnectionController: " + allConns.Length);
        if(allConns == null) return;
        foreach (var cons in allConns)
        {
            if(cons.transform == previewConenction.GetComponent<UMLLineController>().transform) continue;
            // beide parents der anchor suchen wenn beide gleich und
            if (!visible && !(cons.anchorA.IsChildOf(focusObject.transform) 
                && cons.anchorB.IsChildOf(focusObject.transform)))
            {
                cons.gameObject.SetActive(false);
            }
            else
            {
                cons.gameObject.SetActive(true);
            }
        }
    }
    public void RegistriereConnection(UMLLineController connection)
    {
        Debug.Log("registrierungsmethode ausgeführt, Anzahl vorher: " + AllConnections.Count);
        if(!AllConnections.Contains(connection))  AllConnections.Add(connection);
        Debug.Log("Anzahl jetzt "+ AllConnections.Count);
    }

    void Update()
    {
        if (firstAnchor != null)
        {
            UpdatePreviewLine();
            
            // Prüfen, ob die Abbruch-Taste (z.B. B-Button) gedrückt wurde
            if ((cancelActionLeft != null && cancelActionLeft.action.triggered) || 
                (cancelActionRight != null && cancelActionRight.action.triggered))
            {
                CancelConnection();
            }
        }
    }

    public void OnAnchorSelected(UMLAnchor selectedAnchor)
    {
        if (firstAnchor == null)
        {
            // SCHRITT 1: Ersten Anker wählen
            firstAnchor = selectedAnchor;
            previewLine.enabled = true;
            previewLine.SetPosition(0, firstAnchor.transform.position);
        }
        else if (firstAnchor == selectedAnchor || firstAnchor.transform.IsChildOf(selectedAnchor.transform.parent)
            || selectedAnchor.transform.IsChildOf(firstAnchor.transform.parent))
        {
            // ABBRUCH: Zweimal den gleichen Anker gewählt
            CancelConnection();
        }
        else
        {
            // SCHRITT 2: Zweiten Anker wählen und verbinden
            FinalizeConnection(selectedAnchor);
            
            //CancelConnection();
        }
    }

    void FinalizeConnection(UMLAnchor secondAnchor)
    {
        GameObject newConnection = Instantiate(linePrefab);
        UMLLineController controller = newConnection.GetComponent<UMLLineController>();
        controller.AssignAnchors(firstAnchor.transform, secondAnchor.transform);
        Debug.Log("FinalizeConnection abgeschlossen");

        // In Listen eintragen
        //AllConnections.Add(controller);
        Debug.Log(AllConnections.Count);

        CancelConnection(); // Reset für die nächste Verbindung
    }

    void UpdatePreviewLine()
    {
        //irgendwas ein bisschen besser setzen im anchor, alle winkel sind mist.


        // Die Vorschau-Linie geht vom ersten Anker bis zur Hand (oder Ray-Ende)
        previewLine.SetPosition(0, firstAnchor.transform.position);
        //alternativ die position auf irendwas setzen was getracked wird z.B. hand das ende vom ray
        previewLine.SetPosition(1, transform.position); 
    }

    void CancelConnection()
    {
        Debug.Log("cancel ausgeführt");
        firstAnchor = null;
        previewLine.enabled = false;
    }
    //wenn nicht anchor chanel auslosen. muss aber das andere ding prüfen. oder anders sum anchor setzten immer sich selbst auftrue und alles andere trifft automatisch auf niht true.
}