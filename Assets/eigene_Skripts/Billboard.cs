using UnityEngine;
using TMPro;

public class Billboard : MonoBehaviour
{
    public Transform targetCube; // Ziehe hier deinen Cube rein
    public float distance = 0.6f; // Abstand vom Mittelpunkt des Cubes
    private Transform camTransform;

    [SerializeField] private TextMeshProUGUI textContainer;
    [SerializeField] private Transform canvasTransform;

    void awake()
    {
        textContainer = GetComponentInChildren<TextMeshProUGUI>();
        InitialSettingOfCanvasDistance();
        if (textContainer == null)
        {
            Debug.LogError("Kein TextMeshPro im Canvas gefunden!");
        }
    }

    void Start()
    {
        // Sucht die Hauptkamera (dein VR-Headset)
        camTransform = Camera.main.transform;
    }

    private void InitialSettingOfCanvasDistance()
    {
        Vector3 worldScale = targetCube.lossyScale;
        float distanceTop = worldScale.y / 2f;
    
        // Offset hinzufügen, damit die Canvas nicht direkt auf dem Mesh klebt
        float offset = 0.1f; 
        
        // Positioniere die Canvas relativ zum Cube-Zentrum
        canvasTransform.position = targetCube.transform.position + new Vector3(0, distanceTop + offset, 0);
    }
    // LateUpdate wird nach allen anderen Bewegungen aufgerufen
    void LateUpdate()
    {
        if (targetCube == null) return;

        // 1. Berechne die Richtung vom Cube zur Kamera
        Vector3 directionToCam = (camTransform.position - targetCube.position).normalized;

        // 2. Setze die Position des Textes ein Stück vor den Cube in Richtung Kamera
        transform.position = targetCube.position + directionToCam * distance;

        // 3. Richte den Text zur Kamera aus
        transform.forward = camTransform.forward;
    }

    public void activateText()
    {
        if (textContainer != null)
        {
            // Der Canvas bleibt aktiv, nur der Inhalt verschwindet
            textContainer.gameObject.SetActive(true);
            
        }
    }

    public void deactivateText()
    {
        if (textContainer != null)
        {
            // Der Canvas bleibt aktiv, nur der Inhalt verschwindet
            textContainer.gameObject.SetActive(false);
            
        }
    }
}