using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


//bekommt einen Float test und der wir immer zur bamera gedreht (mehrspeiler...)
//update oder bei spielerbewegung... und aufwand?
public abstract class BaseComponent : MonoBehaviour
{
    [Header("Basics")]
    private InformationNode infoNode;
    private List<BaseComponent> nachbarKomponenten;
    [SerializeField] public TextMeshProUGUI frontText;
    [SerializeField] protected InputActionReference rightTriggerAction;
    [SerializeField] protected List<BaseComponent> childComponents = new List<BaseComponent>();
    public UMLAnchor anchorFront;
    public UMLAnchor anchorBack;
    public UMLAnchor anchorRight;
    public UMLAnchor anchorLeft;
    public UMLAnchor anchorUp;
    public UMLAnchor anchorDown;
    private string id;
    private int count = 0;
    [SerializeField] public string playerTagName;
    private bool rayHover = false;

    [Header("Visuals")]
    public MeshRenderer shellRenderer;
    public Material solidMaterial;
    public Material glassMaterial;
    


    [Header("Heatmap Settings")]
    //Shellrenderer in Visuals
    public Color baseColor = Color.white;
    private MaterialPropertyBlock _propBlock;
    public int complexityThreshold = 10; // Ab wie vielen Gesamt-Kindern ist es maximal rot?
    //vergrößerung der Componenete Funktion, kleinen

    [Header("ScalerSettings")]
    [SerializeField] protected Vector3 targetScale; // 20% größer
    [SerializeField] protected float speed = 5f; // Wie schnell skaliert es?

    protected Vector3 _originalScale;
    protected Vector3 _currentGoal;

    [Header("trigger")]
    [SerializeField] private BoxCollider interactionCollider;
    //private int _playerInZoneCount = 0;

    /*
    *. Cleared die Liste der zu nutzenden collider und setzt nur den interactioncollider
    * für den Grab, damit der Collider für die Trigger Interaction nicht übernommen wird
    *
    */
    void Awake()
    {
        // Hole die Referenz zum Grab Interactable
        if (TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(out UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable))
        {
            // 1. Die Liste der Collider leeren
            interactable.colliders.Clear();

            // 2. Deinen spezifischen Collider hinzufügen
            if (interactionCollider != null)
            {
                interactable.colliders.Add(interactionCollider);
            }
            else
            {
                Debug.LogWarning("Kein interaction Collider für Awake zugewiesen!");
            }
        }
    }
    /**
    je nach menge der Komponenten als Kinder einer Komponente ändert dieser 
    die Farbe der eigenen Komponente ab einem schwellenert: complexityThreshold
    */
    public void UpdateVisualHeatmap()
    {
    //if (_propBlock == null) _propBlock = new MaterialPropertyBlock();
    
    int totalComplexity = childComponents.Count;
    float factor = (totalComplexity > 4) ? Mathf.Clamp01((float)totalComplexity / complexityThreshold) : 0f;
    Color heatColor = Color.Lerp(baseColor, Color.red, factor);

    if (shellRenderer != null)
    {
        // Holt aktuelle Werte, setzt die Farbe, schiebt sie zurück
        //shellRenderer.GetPropertyBlock(_propBlock);
        //_propBlock.SetColor("_Color", heatColor); // "_Color" ist der Standard-Name im Shader
        //shellRenderer.SetPropertyBlock(_propBlock);
        solidMaterial.color = heatColor;
    }
}

    // Diese Funktion fragt rekursiv alle Unterebenen ab
    public int GetTotalChildCount()
    {
        int count = childComponents.Count;

        foreach (var child in childComponents)
        {
            // Rekursiver Aufruf: Das Kind fragt seine eigenen Kinder ab
            count += child.GetTotalChildCount();
        }

        return count;
    }
    public void UpdateComponentCollider()
    {
        
    }
    /**
    In einer eigenen Initialisierung werden alle positionen, Größen und auch der UMLManager neu gesetzt
    um die exestenz aller Komponenten zu gewähleisten und die initialisierung auch zurküftig seperat ansteuern zu können
    */
    public void Initiate()
    { 
        Debug.Log("new Komponent initiated");

        Shader defaultShader = Shader.Find("Universal Render Pipeline/Lit");
        if (defaultShader == null) defaultShader = Shader.Find("Standard");

        solidMaterial = new Material(defaultShader);
        infoNode = new InformationNode();
        nachbarKomponenten = new List<BaseComponent>();
        childComponents = new List<BaseComponent>();
        UMLManager.Instance.RegistriereKomponente(this);
        id = System.Guid.NewGuid().ToString();
        //schrift positionieren
        float cubeSizeZ = transform.localScale.z;
        float zPos = -0.5f -(0.01f/cubeSizeZ); 
        frontText.transform.localPosition = new  Vector3(0,0,zPos);

        //zum dynamic zoom
        _originalScale = transform.localScale;
        targetScale = Vector3.Scale(transform.localScale ,targetScale);

        //nicht dauerhaft triggern im Cube
        if (interactionCollider == null) interactionCollider = GetComponent<BoxCollider>();

        //farbe und material setzen
        if (shellRenderer != null)
        {
            
            shellRenderer.material.color = baseColor;
        }


    }
    public void AddChild(BaseComponent child)
    {
        if (!childComponents.Contains(child))
        {
            childComponents.Add(child);
            // Optional: Das Kind-Objekt in der VR-Hierarchie unterordnen
            child.transform.SetParent(this.transform);
        }
        UpdateVisualHeatmap();
        if (transform.parent != null) 
        {
            var parentComp = transform.parent.GetComponent<BaseComponent>();
            if (parentComp != null) parentComp.UpdateVisualHeatmap();
        }
    }

    
    /*
    prüft ob das eingetretene objekt der Player ist über den Tag, setzt das Depthlevel in der 
    Toolbox durch einen aufruf und rüft Enter Workmode  
    */
    // FIXME:setzten des Depthlevel auf EnterWorkMode versetzten
    private void OnTriggerEnter(Collider other)
    {
        ToolboxLogik toolbox;
        
        if (other.CompareTag(playerTagName)) // Prüft ob der Kopf des Nutzers eintritt
        {
            _currentGoal = targetScale;

            interactionCollider.isTrigger = true;

            EnterWorkMode();
            toolbox = other.GetComponentInChildren<ToolboxLogik>();
            toolbox.SetParentBasecomponent(this);

            interactionCollider.isTrigger = false;

            if(toolbox != null) 
            {
                //Debug.Log("toolbox object: " + toolbox.gameObject.name);
                toolbox.addOneDepthLevel();
            }
            else
            {
                Debug.Log("toolbox object: " + toolbox.gameObject.name);
            } 
            
        }
    }
    /*
    analog zu OnTriggerEnter die Rückabwicklung
    */
    private void OnTriggerExit(Collider other)
    {
        ToolboxLogik toolbox;
        if (other.CompareTag(playerTagName))
        {
            _currentGoal = _originalScale;

            ExitWorkMode();
            toolbox = other.GetComponentInChildren<ToolboxLogik>();
            //in workmode vercshieben
            toolbox.subtractOneDepthLevel();
            Transform myParent = this.transform.parent;
            if(myParent != null) toolbox.SetParentBasecomponent(myParent.GetComponent<BaseComponent>());
            else toolbox.SetParentBasecomponent(null);
            
        }
    }
   
    /*
    Ruft alle Methoden auf die den Semantic Zoom umsetzen
    */
    private void EnterWorkMode()
    {
        // Blende alles aus, was nicht dieses Objekt oder ein Kind davon ist
        UMLManager.Instance.SetGlobalVisibility(false, this);
        UMLConnectionBuilder.Instance.SetGlobalVisibility(false, this);
        SetMaterial(glassMaterial);
        //SetScaleSoft();
        //Debug.Log("Arbeitsmodus aktiviert: Fokus auf " + gameObject.name + "(" + frontText + ")");
    }

    /*
    Analog zu EnterWorkMode die Rückabwicklung des Semantic Zooms
    zusätzlich speichern über den SaveManager auslösen
    */
    private void ExitWorkMode()
    { 
        UMLManager.Instance.SetGlobalVisibility(true, this);
        UMLConnectionBuilder.Instance.SetGlobalVisibility(true, this.transform.GetComponentInParent<BaseComponent>());
        UpdateVisualHeatmap();
        SetMaterial(solidMaterial);
        //SetScaleSoft();
        
        // Autosave Aufruf des Singleton
        if(SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveScene();
            Debug.Log("Auto-Save beim Verlassen der Komponente ausgeführt.");
        }
    }
  
  /*
  überträgt die Position der Anchor für die speicherung in eine Position als String ausgegeben
  */
    public string ReadAnchorPosition(Transform checkAnchor)
    {
        if (checkAnchor == anchorFront.transform) return "front";
        if (checkAnchor == anchorBack.transform)  return "back";
        if (checkAnchor == anchorUp.transform)    return "up";
        if (checkAnchor == anchorDown.transform)  return "down";
        if (checkAnchor == anchorRight.transform) return "right";
        if (checkAnchor == anchorLeft.transform)  return "left";
        Debug.Log("<color=blue>kein match </color>");
        return null;
    }

    /*
    liest aus der positinsbeschreibung den richtigen Anchor aus und gibt diesen zurück
    */
    public Transform GetAnchorByPosition(string position)
    {
        switch (position)
        {
            case "front":
                return anchorFront.transform;
            case "back":
                return anchorBack.transform;
            case "up":
                return anchorUp.transform;
            case "down":
                return anchorDown.transform;
            case "left":
                return anchorLeft.transform;
            case "right":
                return anchorRight.transform;
            
        }
        return null;
    }
    /*
    liest die InformationNode
    */
    public string ReadInformationNode(int infoNummer)
    {
        if(infoNode == null)
        {
            return "DIeses Objekt hat keine InfoNode";
        }

        switch (infoNummer)
        {
            case 1: return infoNode.name;
            case 2: return infoNode.discription;
            case 3: return infoNode.responsibility;

        }

        return infoNode.BuildText();
    }
    
    //updated die InfoNode
    public void UpdateInfoNode(string name, string responability, string description)
    {
        frontText.text = name;
        if(infoNode != null)
        {
            infoNode.name = name;
            infoNode.discription = description;
            infoNode.responsibility = responability;
        }
    }
    public string getId()
    {
        return id;
    }
    public void setId(string newID)
    {
        id = newID;
    }
    //sollte immer vonlocalposition ausgehen, keinn kein Parent da ist ist localPosition auch gleich position
    public void SetPosition(Vector3 vector)
    {
       if (transform.parent == null) 
       {
            transform.position = vector;
        }
        else
        {
            transform.localPosition = vector;
        }
    }
    public void SetRotation(Quaternion targetRotation)
    {
        transform.rotation = targetRotation;
    }

    private void SetMaterial(Material material)
    {
        shellRenderer.material = material;
    }
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
    private void SetScaleSoft()
    {
        if(transform.localScale == targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, Time.deltaTime * speed);
        }
        else if(transform.localScale == _originalScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
        }
        
    }
    /*
    Ruft den UMLManager um die Metadaten im Display darzustellen
    */
    public void CallDadToDoTheWork()
    {
        //Debug.Log("called Dad");
        UMLManager.Instance.UpdateInfoScreen(this);
    }
    public void RayHoveringTrue()
    {
        
        rayHover = true;
    }
    public void RayHoveringFalse()
    {
        
        rayHover= false;
    }
    public bool GetRayHover()
    {
        return rayHover;
    }

}
