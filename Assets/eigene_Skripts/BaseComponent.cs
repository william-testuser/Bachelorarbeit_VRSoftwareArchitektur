using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

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
    [SerializeField] protected Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f); // 20% größer
    [SerializeField] protected float speed = 5f; // Wie schnell skaliert es?

    protected Vector3 _originalScale;
    protected Vector3 _currentGoal;

    [Header("trigger")]
    [SerializeField] private BoxCollider interactionCollider;
    private int _playerInZoneCount = 0;

    /**
    gür übergroße Kinderkomponenten hell dunel einstellung nutzen
    je nach menge der Komponenten als Kinder einer Komponente ändert dieser die Farbe ab einem schwellenert
    */
    public void UpdateVisualHeatmap()
    {
    //if (_propBlock == null) _propBlock = new MaterialPropertyBlock();
    
    int totalComplexity = childComponents.Count;
    Debug.Log(totalComplexity);
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
        _currentGoal = _originalScale;

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

    // --- TEIL 2: Betreten der Box (Außenwelt ausblenden) ---
    // Voraussetzung: Die Box hat einen BoxCollider mit "Is Trigger = true"
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
                Debug.Log("toolbox object: " + toolbox.gameObject.name);
                toolbox.addOneDepthLevel();
            }
            else
            {
                Debug.Log("toolbox object: " + toolbox.gameObject.name);
            } 
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ToolboxLogik toolbox;
        if (other.CompareTag(playerTagName))
        {
            _currentGoal = _originalScale;

            ExitWorkMode();
            toolbox = other.GetComponentInChildren<ToolboxLogik>();
            toolbox.subtractOneDepthLevel();
            BaseComponent parentCompoenet = this.transform.parent.GetComponent<BaseComponent>();
            if(parentCompoenet != null) toolbox.SetParentBasecomponent(parentCompoenet);
            
        }
    }
   
 
    private void EnterWorkMode()
    {
        // Blende alles aus, was nicht dieses Objekt oder ein Kind davon ist
        UMLManager.Instance.SetGlobalVisibility(false, this);
        UMLConnectionBuilder.Instance.SetGlobalVisibility(false, this);
        SetMaterial(glassMaterial);
        Debug.Log("Arbeitsmodus aktiviert: Fokus auf " + gameObject.name + "(" + frontText + ")");
    }

    private void ExitWorkMode()
    {
        UMLManager.Instance.SetGlobalVisibility(true, null);
        UMLConnectionBuilder.Instance.SetGlobalVisibility(true, null);
        UpdateVisualHeatmap();
        SetMaterial(solidMaterial);
        

        
        // Tipp: Hier rufen wir das Singleton auf
        if(SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveScene();
            Debug.Log("Auto-Save beim Verlassen der Komponente ausgeführt.");
        }
    }
  
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
    
    //updated auch den namen...
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

    public void Test()
    {
        count++;
        frontText.text = count.ToString();
    }

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
    private void SetMaterial(Material material)
    {
        shellRenderer.material = material;
    }
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
    public void CallDadToDoTheWork()
    {
        Debug.Log("called Dad");
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
