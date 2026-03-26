using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class UMLLineController : MonoBehaviour
{
    [SerializeField] protected InputActionReference rightTriggerAction;
    [SerializeField] private TextMeshProUGUI typeDisplay;
    private Transform canvasTransform;
    private string connectionType = "default Dummy";
    public Transform anchorA;
    public Transform anchorB;
    private LineRenderer lineRenderer;
    private MeshCollider meshCollider;
    private bool rayHover;
    public bool destroyByAnchor = false;

    void Start()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        // Ein paar Standardwerte für die Linie
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        
        if(typeDisplay == null) typeDisplay = GetComponentInChildren<TextMeshProUGUI>();
        typeDisplay.text = connectionType;
        canvasTransform = typeDisplay.transform.parent;
        
    }
    public void CallMomToDoTheWork()
    {
        Debug.Log("called Mom");
        UMLConnectionBuilder.Instance.UpdateInfoScreen(this);
    }
    public string GetConnectionType()
    {
        return connectionType;
    }
    public void ChangeConnection(string con)
    {
        connectionType = con;
        typeDisplay.text = con;
    }
    void Update()
    {
        if (anchorA != null && anchorB != null)
        {
            // Die Linie folgt den Ankern jeden Frame
            lineRenderer.SetPosition(0, anchorA.position);
            lineRenderer.SetPosition(1, anchorB.position);
            
            Vector3 mitte = (anchorA.position + anchorB.position) / 2f;

        // 3. Das Canvas/Text-Objekt an diese Position schieben
            if (canvasTransform != null)
            {
                canvasTransform.position = mitte;
                
                // Optional: Den Text immer zur Kamera drehen (Billboarding)
                // canvasTransform.LookAt(canvasTransform.position + Camera.main.transform.forward);
            }
        }


        if (GetRayHover())
        {
            //Debug.Log("updated");
            if(rightTriggerAction.action.triggered) CallMomToDoTheWork();
        }
        
    }
    void LateUpdate()
    {
        Update3DCollider();
    }
    
    public void Update3DCollider()
    {
        if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true); // Erzeugt ein Mesh aus der aktuellen Linie
        meshCollider.sharedMesh = mesh;
    }

    public void AssignAnchors(Transform a, Transform b)
    {
        anchorA = a;
        anchorB = b;
    }
    void OnDestroy()
    {
        if(!destroyByAnchor){
            UMLAnchor anc = anchorA.GetComponent<UMLAnchor>();
            anc.RemoveConnection(this);
            anc = anchorB.GetComponent<UMLAnchor>();
            anc.RemoveConnection(this);
        }
    }
    public void RayHoveringTrue()
    {
        //Debug.Log("ray on");
        rayHover = true;
    }
    public void RayHoveringFalse()
    {
        //Debug.Log("ray off");
        rayHover= false;
    }
    public bool GetRayHover()
    {
        return rayHover;
    }


}