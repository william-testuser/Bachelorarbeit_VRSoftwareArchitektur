using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UMLManager : MonoBehaviour
{
    // Das ist das "Herzstück" des Singletons
    public static UMLManager Instance { get; private set; }
    // Die zentrale Liste aller UML-Elemente
    public List<BaseComponent> AlleKomponenten = new List<BaseComponent>();
    private BaseComponent lastComponentTriggered;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject componentView;
    [SerializeField] private GameObject connectoinView;
    [SerializeField] private GameObject MetaView;

    public void ClearScene()
    {
         BaseComponent[] allComps = FindObjectsOfType<BaseComponent>(true);
        //Debug.Log("SetGlobalVisibility ausgeführt, gefundene BaseComponenten: " + allComps.Length);
        foreach (var comp in allComps)
        {
            lastComponentTriggered = comp;
            DeleteComponent();
        }
       
    }

    public void Search(string nameOfSearchObject)
    {
        BaseComponent searchComp = null;
        foreach (BaseComponent comp in AlleKomponenten)
        {
            if(comp.ReadInformationNode(1) == nameOfSearchObject){
                 searchComp = comp;
                 break;
            }
        }

        if(searchComp != null)SetGlobalVisibility(false, searchComp);
        else Debug.Log("Text nicht richtig oder Gameobjekt nicht gefunden");
        // irgendwas mit timer oder zweites mal drücken für zurücksetzten? dann mit text ändern
    }
    public void RegistriereKomponente(BaseComponent comp)
    {
        Debug.Log("registrierungsmethode ausgeführt, Anzahl vorher: " + AlleKomponenten.Count);
        if(!AlleKomponenten.Contains(comp))  AlleKomponenten.Add(comp);
        Debug.Log("Anzahl jetzt "+ AlleKomponenten.Count);
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

    public void SetGlobalVisibility(bool visible, BaseComponent focusObject)
    {
        bool topLayer = false;
        BaseComponent upwardLayerComponent;
        if(focusObject.transform.parent == null)
        {
            upwardLayerComponent = null;
            topLayer = true; 
            Debug.Log("true = topLayer");
        }
        //else upwardLayerComponent = focusObject.transform.GetComponentInParent<BaseComponent>();
      
        
        Debug.Log(focusObject);
        // Finde alle BaseComponents in der Szene
        BaseComponent[] allComps = FindObjectsOfType<BaseComponent>(true);
        //Debug.Log("SetGlobalVisibility ausgeführt, gefundene BaseComponenten: " + allComps.Length);
        foreach (var comp in allComps)
        {
            if(comp == null) continue;
            // Wenn wir im Arbeitsmodus sind, blende alles aus außer dem Fokus-Objekt
            if (!visible && comp != focusObject && !comp.transform.IsChildOf(focusObject.transform) 
                && !focusObject.transform.IsChildOf(comp.transform))
            {
                comp.gameObject.SetActive(false);
            }
            else if(visible  /*|| comp.transform.parent == focusObject.transform || comp.transform == focusObject.transform)*/)
            {

                if ((topLayer && comp.transform.parent == null ) || (!topLayer && (comp.transform.parent == focusObject.transform.parent || focusObject.transform.IsChildOf(comp.transform))))
                {
                    Debug.Log("toplayer oder das child...");
                    comp.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("else");
                    comp.gameObject.SetActive(false);
                }
                
            }
            else if(!visible)
            {
                comp.gameObject.SetActive(true);
            }
            else
            {
                //nur die kinder objekte die zuletzt auf true gesetzt wurden, sollten hier deaktiviert werden.
                Debug.Log("Darf nicht passieren");
            }
        }
    }
    /**
    die Ausgewählte komponente wird hier auch gespeichert. da wenn eine komponente ausgewählt wird auch immer der 
    sceen ein update bekommt zusammen ausführen und auch um die ausführreinfolge zu kontrollieren
    */
    public void UpdateInfoScreen(BaseComponent baseComponenet)
    {
        lastComponentTriggered = baseComponenet;
        VRTextEditor editor =canvas.GetComponentInChildren<VRTextEditor>(true);
        editor.UpdateInputFields(baseComponenet.ReadInformationNode(1), baseComponenet.ReadInformationNode(2), baseComponenet.ReadInformationNode(3));
        editor.ResetPosition();
        SetActivationInfoScreen(true);

    }
    public void DeleteComponent()
    {
        List<BaseComponent> allChilden = new List<BaseComponent>();
        foreach (BaseComponent com in AlleKomponenten)
        {
            if(com.transform.IsChildOf(lastComponentTriggered.transform)) allChilden.Add(com);
        }
        foreach (BaseComponent com in allChilden)
        {
            AlleKomponenten.Remove(com);
        }
        AlleKomponenten.Remove(lastComponentTriggered);
        Destroy(lastComponentTriggered.gameObject, 0.2f);
        lastComponentTriggered = null;
    }
    public void SetActivationInfoScreen(bool activity)
    {
        componentView.SetActive(true);
        connectoinView.SetActive(false);
        MetaView.SetActive(false);
        canvas.gameObject.SetActive(activity);
    }
    public void UpdateBaseComponent(string name, string responsability, string description)
    {
        if(lastComponentTriggered != null)lastComponentTriggered.UpdateInfoNode(name, responsability, description);
        else Debug.Log("no lastComponentTriggered");
    }
}