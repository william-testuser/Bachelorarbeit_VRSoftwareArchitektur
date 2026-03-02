using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UMLConnectionBuilder : MonoBehaviour
{
    public static UMLConnectionBuilder Instance { get; private set; }
    public UMLAnchor firstAnchor;
    public GameObject linePrefab; // Ein Prefab mit dem UMLLineController
    public LineRenderer previewLine; // Eine visuelle Linie während des Ziehens
    
    public InputActionReference cancelActionLeft;
    public InputActionReference cancelActionRight; 
    private List<UMLLineController> AllConnections;

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

    public void SetGlobalVisibility(bool visible, BaseComponent focusObject)
    {
        // Finde alle BaseComponents in der Szene
        UMLLineController[] allConns = FindObjectsOfType<UMLLineController>(true);
        Debug.Log("SetGlobalVisibility ausgeführt, gefundene UMLConnectionController: " + allConns.Length);
        foreach (var comp in allConns)
        {
            // beide parents der anchor suchen wenn beide gleich und
            if (!visible && comp.anchorA.transform.parent != comp.anchorB.transform.parent 
                && !comp.anchorA.transform.parent == focusObject.transform)
            {
                comp.gameObject.SetActive(false);
            }
            else
            {
                comp.gameObject.SetActive(true);
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
        else if (firstAnchor == selectedAnchor)
        {
            // ABBRUCH: Zweimal den gleichen Anker gewählt
            CancelConnection();
        }
        else
        {
            // SCHRITT 2: Zweiten Anker wählen und verbinden
            FinalizeConnection(selectedAnchor);
        }
    }

    void FinalizeConnection(UMLAnchor secondAnchor)
    {
        GameObject newConnection = Instantiate(linePrefab);
        UMLLineController controller = newConnection.GetComponent<UMLLineController>();
        controller.AssignAnchors(firstAnchor.transform, secondAnchor.transform);
        
        // In Listen eintragen
        AllConnections.Add(controller);

        CancelConnection(); // Reset für die nächste Verbindung
    }

    void UpdatePreviewLine()
    {
        //irgendwas ein bisschen besser setzen im anchor, alle winkel sind mist.


        // Die Vorschau-Linie geht vom ersten Anker bis zur Hand (oder Ray-Ende)
        previewLine.SetPosition(0, firstAnchor.transform.position);
        //alternativ die position auf irendwas setzen was getracked wird z.B. hand etc. transform.position
        previewLine.SetPosition(1, transform.position); 
    }

    void CancelConnection()
    {
        firstAnchor = null;
        previewLine.enabled = false;
    }
    //wenn nicht anchor chanel auslosen. muss aber das andere ding prüfen. oder anders sum anchor setzten immer sich selbst auftrue und alles andere trifft automatisch auf niht true.
}