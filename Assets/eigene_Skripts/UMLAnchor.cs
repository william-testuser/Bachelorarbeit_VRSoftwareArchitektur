using UnityEngine;
using System.Collections.Generic;

public class UMLAnchor : MonoBehaviour
{
    public BaseComponent parentComponent;
    // Liste aller Verbindungen, falls man sie später zusammen löschen will
    public List<UMLLineController> connections = new List<UMLLineController>();
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
}