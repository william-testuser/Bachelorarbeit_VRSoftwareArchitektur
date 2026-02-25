using UnityEngine;
using UnityEngine.InputSystem;

public class UMLConnectionBuilder : MonoBehaviour
{
    public UMLAnchor firstAnchor;
    public GameObject linePrefab; // Ein Prefab mit dem UMLLineController
    public LineRenderer previewLine; // Eine visuelle Linie während des Ziehens
    
    public InputActionReference cancelActionLeft;
    public InputActionReference cancelActionRight; 

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
        firstAnchor.connections.Add(controller);
        secondAnchor.connections.Add(controller);

        CancelConnection(); // Reset für die nächste Verbindung
    }

    void UpdatePreviewLine()
    {
        // Die Vorschau-Linie geht vom ersten Anker bis zur Hand (oder Ray-Ende)
        previewLine.SetPosition(0, firstAnchor.transform.position);
        previewLine.SetPosition(1, transform.position); 
    }

    void CancelConnection()
    {
        firstAnchor = null;
        previewLine.enabled = false;
    }
}