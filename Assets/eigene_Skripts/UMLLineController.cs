using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class UMLLineController : MonoBehaviour
{
    public Transform anchorA;
    public Transform anchorB;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Ein paar Standardwerte für die Linie
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        
        // Material sollte im Inspector zugewiesen werden (z.B. URP Unlit)
    }

    void Update()
    {
        if (anchorA != null && anchorB != null)
        {
            // Die Linie folgt den Ankern jeden Frame
            lineRenderer.SetPosition(0, anchorA.position);
            lineRenderer.SetPosition(1, anchorB.position);
        }
    }

    public void AssignAnchors(Transform a, Transform b)
    {
        anchorA = a;
        anchorB = b;
    }
}