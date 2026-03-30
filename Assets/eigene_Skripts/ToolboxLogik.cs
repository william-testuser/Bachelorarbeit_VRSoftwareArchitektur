using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;

public class ToolboxLogik : MonoBehaviour //,IInteractable
{
    public string InteractMassege => objectInteractMessage;
    [SerializeField] public TextMeshProUGUI _frontText; // Serialisierung für Framework-Ebene
    [SerializeField] string objectInteractMessage;
    [SerializeField] private int depthLevel;
    private bool enteredFakeLevel = false;
    private BaseComponent insertIntoThisBasecomponent;

     [SerializeField] private GameObject componenteLevel0;
     [SerializeField] private GameObject componenteLevel1;
     [SerializeField] private GameObject componenteLevel2;
     [SerializeField] private GameObject componenteLevelPerson;

    private IInteractable interactableObj;
    private BaseComponent baseComponent;
    // Start is called before the first frame update

    public TextMeshProUGUI frontText => _frontText;
    private int count = 0;

    /*
    eine Interact methode für verschieden Szenarien, wurde in dieser itereation nur fürs Spawnern benutzt
    spawnt das jeweilige Prefab der Komponente je Depthlevel
    */    
    public void Interact()
    {
      //je nach LAyer inder man sich befindet, einenandere komponenete spawnen.
       SpawnPrefab(depthLevel);
        
    }
    // speichert das aktuell im peronal UI anzusehende objekt/ das zu bearbeitende Objekt
    public void SetParentBasecomponent(BaseComponent newBase)
    {
        if(newBase != null) insertIntoThisBasecomponent = newBase;
    }
    public void addOneDepthLevel()
    {
        if(depthLevel<=3) depthLevel++;
        else enteredFakeLevel = true;
        Debug.Log($"Player ist jetzt in Tiefe: {depthLevel}");
    }

    public void subtractOneDepthLevel()
    {
        if(!enteredFakeLevel) depthLevel--;
        enteredFakeLevel = false;
        Debug.Log($"Player ist jetzt in Tiefe: {depthLevel}");
    }

/*
Spawnt die unterscheidlichen Komponenten je übergebenen integer Wert
setzt deren initiale positin immer gleich oder mittig im Parentobjekt und passt deren größe je nach tiefenlevel an.
initialisiert diese danach dann
*/
    public void SpawnPrefab(int componente)
    {
        //Debug.Log("spawned executed");
        GameObject initObject = null;
        if(componente>0 && insertIntoThisBasecomponent == null)
        {
            Debug.Log("Ebene höher als 0: " + componente + " aber kein Parent der Basekomponente ist");
            return;
        }
        switch (componente)
        {
        
            case 0: 
                Debug.Log("case 1 ausgeführt");
                
                initObject = Instantiate(componenteLevel0);
                baseComponent = initObject.GetComponent<BaseComponent>();
                baseComponent.SetPosition(new Vector3(count, 2.5f , 0));
                baseComponent.Initiate();
                break;
            case 1:
                Debug.Log("case 2 ausgeführt. Parent: " + insertIntoThisBasecomponent.gameObject.name + "componente: "+ componente + " echt: " + depthLevel);
                
                initObject = Instantiate(componenteLevel1, insertIntoThisBasecomponent.gameObject.transform);
                //initObject.transform.localPosition = Vector3.zero;
                baseComponent = initObject.GetComponent<BaseComponent>();
                baseComponent.SetPosition(new Vector3(0, 0 , 0));
                baseComponent.SetScale(0.2f);
                baseComponent.Initiate();
                insertIntoThisBasecomponent.AddChild(baseComponent);
                break;
            case 2: 
                Debug.Log("case 3 ausgeführt. Parent: " + insertIntoThisBasecomponent.gameObject.name + "componente: "+ componente + " echt: " + depthLevel);
                
                initObject = Instantiate(componenteLevel1, insertIntoThisBasecomponent.gameObject.transform);
                //initObject.transform.localPosition = Vector3.zero;
                baseComponent = initObject.GetComponent<BaseComponent>();
                baseComponent.SetPosition(new Vector3(0, 0 , 0));
                baseComponent.SetScale(0.2f);
                baseComponent.Initiate();
                insertIntoThisBasecomponent.AddChild(baseComponent);
                break;
            case 3: 
                Debug.Log("case 4 ausgeführt. Parent: " + insertIntoThisBasecomponent.gameObject.name + "componente: "+ componente + " echt: " + depthLevel);
                
                initObject = Instantiate(componenteLevel1, insertIntoThisBasecomponent.gameObject.transform);
                //initObject.transform.localPosition = Vector3.zero;
                baseComponent = initObject.GetComponent<BaseComponent>();
                baseComponent.SetPosition(new Vector3(0, 0 , 0));
                baseComponent.SetScale(0.2f);
                baseComponent.Initiate();
                insertIntoThisBasecomponent.AddChild(baseComponent);
                break;

            case 11: break;
            case 12: break;
            
            default:
                break;
        }

        initObject?.SetActive(true);

    }

    /*
    hilfmethode für das Laden aus der JSOnlatei. ertsellt das klassiche prefab
    */
    public BaseComponent SpawnPrefab()
    {
        GameObject initObject = null;
        initObject = Instantiate(componenteLevel0);
        baseComponent = initObject.GetComponent<BaseComponent>();
        baseComponent.SetPosition(new Vector3(count, 2.5f , 0));
        baseComponent.Initiate();
        return baseComponent;
    }

    //hilfmethode für dem Test, spannt ein kindoBjekt ins übergebene Objekt
    public BaseComponent SpawnPrefab(BaseComponent newParent)
    {
        if(newParent != null)
        {
            
        
            GameObject initObject = null;
            initObject = Instantiate(componenteLevel1, newParent.gameObject.transform);
            //initObject.transform.localPosition = Vector3.zero;
            baseComponent = initObject.GetComponent<BaseComponent>();
            baseComponent.SetPosition(new Vector3(0, 0 , 0));
            baseComponent.SetScale(0.2f);
            baseComponent.Initiate();
            newParent.AddChild(baseComponent);
        }
        else Debug.Log("<color=red>new Parent ist null im spawn</color>");
        return baseComponent;
    }
}
