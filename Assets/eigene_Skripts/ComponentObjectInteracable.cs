using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComponentObjectInteracable : MonoBehaviour, IInteractable
{
    public string InteractMassege{get;} = "ich bin ein Komponent";
    [SerializeField] private TextMeshProUGUI _frontText;
    private GameObject someGameObject;
    public TextMeshProUGUI frontText => _frontText;
    private Renderer componentRenderer;
    private float x = 3f;

    void awake()
    {
        //frontText = GetComponentInChildren<TextMeshPro>();
        componentRenderer = GetComponent<MeshRenderer>();
        componentRenderer.material.color = Color.white;


    }
    // Start is called before the first frame update
    
    public void Initialize(string name)
    {
        frontText.text = "so";
        float cubeSizeZ = transform.localScale.z;
        float zPos = -0.5f -(0.01f/cubeSizeZ); //wegen der textdicke, unity versetzt das kind schon richtig
        frontText.transform.localPosition = new  Vector3(0,0,zPos);

    }
    public void Interact()
    {
        x = x+1;
       // transform.position = new Vector3(0,2,x);
        frontText.text = "interacted";
    }

    public void onGrab()
    {
        
    }
}
