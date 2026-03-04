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
            else
            {
                comp.gameObject.SetActive(true);
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
        SetActivationInfoScreen(true);
        VRTextEditor editor =canvas.GetComponentInChildren<VRTextEditor>(true);
        editor.UpdateInputFields(baseComponenet.ReadInformationNode(1), baseComponenet.ReadInformationNode(2), baseComponenet.ReadInformationNode(3));

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
        Debug.Log(canvas.name);
        canvas.gameObject.SetActive(activity);
    }
    public void UpdateBaseComponent(string name, string responsability, string description)
    {
        if(lastComponentTriggered != null)lastComponentTriggered.UpdateInfoNode(name, responsability, description);
        else Debug.Log("no lastComponentTriggered");
    }
}