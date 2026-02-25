using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour, IInteractable
{
    [SerializeField] public TextMeshProUGUI _frontText; 
    public TextMeshProUGUI frontText => _frontText;
    public string InteractMassege{get;} = "zerro";
    [SerializeField] private GameObject newWhatever;
    private IInteractable interactableObj;
    // Start is called before the first frame update
    public void Interact()
    {
        SpawnPrefab("component");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnPrefab(string interactable)
    {
       // interactable.tosmallLetters
        switch (interactable)
        {
        
            case "component": 
                GameObject initObject = Instantiate(newWhatever);
                var scipt = initObject.GetComponent<ComponentObjectInteracable>();
                interactableObj = initObject.GetComponent<IInteractable>();
                break;
            default:
                break;
        }
        newWhatever?.SetActive(true);

    }
}
