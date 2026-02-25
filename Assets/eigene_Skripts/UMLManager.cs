using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class UMLManager : MonoBehaviour
{
    // Das ist das "Herzstück" des Singletons
    public static UMLManager Instance { get; private set; }
    // Die zentrale Liste aller UML-Elemente
    public List<BaseComponent> AlleKomponenten = new List<BaseComponent>();

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
        Debug.Log("SetGlobalVisibility ausgeführt, gefundene BaseComponenten: " + allComps.Length);
        foreach (var comp in allComps)
        {
            // Wenn wir im Arbeitsmodus sind, blende alles aus außer dem Fokus-Objekt
            if (!visible && comp != focusObject && !comp.transform.IsChildOf(focusObject.transform))
            {
                comp.gameObject.SetActive(false);
            }
            else
            {
                comp.gameObject.SetActive(true);
            }
        }
    }
}