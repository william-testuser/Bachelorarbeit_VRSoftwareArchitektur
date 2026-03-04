using UnityEngine;
using System.Collections.Generic;

public class UMLAnchor : MonoBehaviour
{
    public BaseComponent parentComponent;
    public List<UMLLineController> connectedConnections= new List<UMLLineController>();
    // Liste aller Verbindungen, falls man sie später zusammen löschen will
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material glowMaterial;

    private UMLAnchor thisAnchor;
    private UMLConnectionBuilder builder;

    void Awake()
    {
        thisAnchor = GetComponent<UMLAnchor>();
        // Sucht den Manager in der Szene
        builder = FindFirstObjectByType<UMLConnectionBuilder>(); 
        meshRenderer.enabled = false;
    }
    void Start()
    {
        parentComponent = this.transform.parent.GetComponentInParent<BaseComponent>();
        // Sicherstellen, dass der Layer auf "Ignore Raycast" steht, 
        // falls der Ray nur die Box treffen soll, oder "Interactable"
    }
    void onDestroy()
    {
        foreach (var cons in connectedConnections)
        {
            connectedConnections.Remove(cons);
            UMLAnchor anc = cons.anchorA.GetComponent<UMLAnchor>();
            if(anc.transform == this.transform) anc = cons.anchorB.GetComponent<UMLAnchor>();
            anc.connectedConnections.Remove(cons);
            UMLConnectionBuilder.Instance.RemoveConnection(cons);
            connectedConnections.Remove(cons); 
            Destroy(cons.gameObject);
        }
    }
    public void NotifyManager()
    {
        if (builder != null && thisAnchor != null)
        {
            builder.OnAnchorSelected(thisAnchor);
        }
    }
    public void Glow()
    {
        if(meshRenderer!=null)
        {
            meshRenderer.enabled = true;
            meshRenderer.material = glowMaterial;
        }
    }
    public void GlowOff()
    {
        if(meshRenderer!=null) meshRenderer.enabled = false;
    }

    public void AddConnection(UMLLineController con)
    {
        connectedConnections.Add(con);
    }
}