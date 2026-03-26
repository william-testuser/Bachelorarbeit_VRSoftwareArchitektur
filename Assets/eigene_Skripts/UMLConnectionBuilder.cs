using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UMLConnectionBuilder : MonoBehaviour
{
    public static UMLConnectionBuilder Instance { get; private set; }
    public UMLAnchor firstAnchor;
    [SerializeField] private GameObject linePrefab; // Ein Prefab mit dem UMLLineController
    [SerializeField] private LineRenderer previewLine; // Eine visuelle Linie während des Ziehens
    [SerializeField] private GameObject previewConenction;
    [SerializeField] private UMLLineController lastConnectionTriggered;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject componentView;
    [SerializeField] private GameObject connectoinView;
    [SerializeField] private GameObject MetaView;
    
    public InputActionReference cancelActionLeft;
    public InputActionReference cancelActionRight; 
    [SerializeField] private TMP_Dropdown dropDown;
    public List<UMLLineController> AllConnections = new List<UMLLineController>();
    public void RemoveConnection(UMLLineController con)
    {
        AllConnections.Remove(con);
    }

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
        //Debug.Log("SetGlobalVisibility ausgeführt, gefundene UMLConnectionController: " + allConns.Length);
        if(allConns == null) return;
        foreach (var cons in allConns)
        {
            if(cons == null) continue;
            if(cons.transform == previewConenction.GetComponent<UMLLineController>().transform) continue;
            // beide parents der anchor suchen wenn beide gleich und
            if (!visible && !(cons.anchorA.IsChildOf(focusObject.transform) 
                && cons.anchorB.IsChildOf(focusObject.transform)))
            {
                cons.gameObject.SetActive(false);
            }
            else if(visible && (cons.anchorA.parent.parent == null 
                && cons.anchorB.parent.parent == null))
            {
                cons.gameObject.SetActive(true);
            } 
            else if(!visible)
            {
                cons.gameObject.SetActive(true);
            } 
            else
            {
                cons.gameObject.SetActive(false);
            }
        }
    }
    public void RegistriereConnection(UMLLineController connection)
    {
        //Debug.Log("registrierungsmethode ausgeführt, Anzahl vorher: " + AllConnections.Count);
        if(!AllConnections.Contains(connection))  AllConnections.Add(connection);
        //Debug.Log("Anzahl jetzt "+ AllConnections.Count);
    }

    public void ClearAllConnections()
    {
       
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
        firstAnchor.AddConnection(controller);
        secondAnchor.AddConnection(controller);
        //Debug.Log("FinalizeConnection abgeschlossen");

        // In Listen eintragen
        AllConnections.Add(controller);
        lastConnectionTriggered = controller;
        //Debug.Log(AllConnections.Count);
        UpdateInfoScreen(controller);
        CancelConnection(); // Reset für die nächste Verbindung
    }
    public void CreateConnection(Transform anchorA, Transform AnchorB, string connT)
    {
        firstAnchor = anchorA.GetComponent<UMLAnchor>();
        FinalizeConnection(AnchorB.GetComponent<UMLAnchor>());
        lastConnectionTriggered.ChangeConnection(connT);
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
        //Debug.Log("cancel ausgeführt");
        firstAnchor = null;
        previewLine.enabled = false;
    }
    //wenn nicht anchor chanel auslosen. muss aber das andere ding prüfen. oder anders sum anchor setzten immer sich selbst auftrue und alles andere trifft automatisch auf niht true.
    public bool IsPreviewLine(UMLLineController compareController)
    {
        return compareController.gameObject == previewLine.gameObject;
    }

    public void UpdateConnectionInfo()
    {
        if(lastConnectionTriggered == null) return;
        int index = dropDown.value;
        string newTitle = dropDown.options[index].text;
        lastConnectionTriggered.ChangeConnection(newTitle);
        VRTextEditor editor =canvas.GetComponentInChildren<VRTextEditor>(true);
        editor.UpdateTitel(newTitle);

        //über ein dropdown aufrufebn? und übergeben können? oder einzelne aber das kaka
    }
    public void UpdateInfoScreen(UMLLineController newController)
    {
        lastConnectionTriggered = newController;
        VRTextEditor editor =canvas.GetComponentInChildren<VRTextEditor>(true);
        editor.UpdateTitel(newController.GetConnectionType());
        editor.ResetPosition();
        SetActivationInfoScreen(true);
    }
    public void SetActivationInfoScreen(bool activity)
    {
        
        componentView.SetActive(false);
        MetaView.SetActive(false);
        connectoinView.SetActive(true);
        canvas.gameObject.SetActive(activity);
    }
  
    public void DeleteConnection()
    {
        if(lastConnectionTriggered == null) return;
        AllConnections.Remove(lastConnectionTriggered);
        Destroy(lastConnectionTriggered.gameObject, 0.2f);
        lastConnectionTriggered = null;
    }
}