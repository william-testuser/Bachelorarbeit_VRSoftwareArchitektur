using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

//bekommt einen Float test und der wir immer zur bamera gedreht (mehrspeiler...)
//update oder bei spielerbewegung... und aufwand?
public abstract class BaseComponent : MonoBehaviour
{
    private InformationNode infoNode;
    private List<BaseComponent> nachbarKomponenten;
    [SerializeField] public TextMeshProUGUI frontText;

     private bool isExplored = false;
    [SerializeField] protected List<BaseComponent> childComponents = new List<BaseComponent>();


    [Header("Visuals")]
    public MeshRenderer shellRenderer;
    public Material solidMaterial;
    public Material glassMaterial;
    private string id;
    private int count = 0;
    [SerializeField] public string playerTagName;


    [Header("Heatmap Settings")]
    //Shellrenderer in Visuals
    public Color baseColor = Color.white;
    public int complexityThreshold = 10; // Ab wie vielen Gesamt-Kindern ist es maximal rot?
    //vergrößerung der Componenete Funktion, kleinen

    public void UpdateVisualHeatmap()
    {
        // 1. Berechne die totale Anzahl aller Nachfahren (Kinder, Kindeskinder...)
        int totalComplexity = GetTotalChildCount();
        float factor = 0.0f;
        // 2. Berechne den Faktor (0.0 bis 1.0)
        if(totalComplexity>4) factor = Mathf.Clamp01((float)totalComplexity / complexityThreshold);

        // 3. Farbe mischen: Von Weiß/Blau stufenweise zu Rot
        // Color.Lerp mischt zwei Farben basierend auf dem Faktor
        Color heatColor = Color.Lerp(baseColor, Color.red, factor);

        // 4. Farbe zuweisen
        if (shellRenderer != null)
        {
            shellRenderer.material.color = heatColor;
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
        infoNode = new InformationNode();
        nachbarKomponenten = new List<BaseComponent>();
        childComponents = new List<BaseComponent>();
        UMLManager.Instance.RegistriereKomponente(this);
        id = System.Guid.NewGuid().ToString();
        //schrift positionieren
        float cubeSizeZ = transform.localScale.z;
        float zPos = -0.5f -(0.01f/cubeSizeZ); 
        frontText.transform.localPosition = new  Vector3(0,0,zPos);
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

    // --- TEIL 1: Fokus setzen (Hülle wird Glas, Kinder erscheinen) ---
    public void ToggleExpansion()
    {
        isExplored = !isExplored;
        
        // Hülle wechseln
        shellRenderer.material = isExplored ? glassMaterial : solidMaterial;

        // Kinder aktivieren/deaktivieren
        foreach (var child in childComponents)
        {
            child.gameObject.SetActive(isExplored);
        }
    }

    // --- TEIL 2: Betreten der Box (Außenwelt ausblenden) ---
    // Voraussetzung: Die Box hat einen BoxCollider mit "Is Trigger = true"
    private void OnTriggerEnter(Collider other)
    {
        ToolboxLogik toolbox;
        
        
        if (other.CompareTag(playerTagName)) // Prüft ob der Kopf des Nutzers eintritt
        {

            EnterWorkMode();
            toolbox = other.GetComponentInChildren<ToolboxLogik>();
            toolbox.SetParentBasecomponent(this);
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
        SetMaterial(glassMaterial);
        Debug.Log("Arbeitsmodus aktiviert: Fokus auf " + gameObject.name);
    }

    private void ExitWorkMode()
    {
        UMLManager.Instance.SetGlobalVisibility(true, null);
        SetMaterial(solidMaterial);
        
        // Tipp: Hier rufen wir das Singleton auf
        if(SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveScene();
            Debug.Log("Auto-Save beim Verlassen der Komponente ausgeführt.");
        }
    }
    

    // hier noch viel ändern
  
    public string ReadInformationNode(int infoNummer)
    {
        if(infoNode == null)
        {
            return "DIeses Objekt hat keine InfoNode";
        }

        switch (infoNummer)
        {
            case 1: return infoNode.Discription;
            case 2: return infoNode.Responsibility;

        }

        return infoNode.BuildText();
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
}
