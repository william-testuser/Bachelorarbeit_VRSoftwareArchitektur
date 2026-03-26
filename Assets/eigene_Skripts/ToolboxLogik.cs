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

    //public last Component entered 
    
    public void Interact()
    {
      //je nach LAyer inder man sich befindet, einenandere komponenete spawnen.
       SpawnPrefab(depthLevel);
        
    }
    // Die Signatur muss zum Event-System des XRI passen
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

    public BaseComponent SpawnPrefab()
    {
        GameObject initObject = null;
        initObject = Instantiate(componenteLevel0);
        baseComponent = initObject.GetComponent<BaseComponent>();
        baseComponent.SetPosition(new Vector3(count, 2.5f , 0));
        baseComponent.Initiate();
        return baseComponent;
    }
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
