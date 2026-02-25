using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ray_Interactor_triggered : MonoBehaviour
{

    [SerializeField] private GameObject rayVisuals; // Das Objekt mit dem Line Renderer
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        // Initial den Strahl deaktivieren
        if(rayVisuals) rayVisuals.SetActive(false);
    }

    // Wir nutzen die Framework-Events
    public void OnSelectEntered(SelectEnterEventArgs args) => rayVisuals.SetActive(true);
    public void OnSelectExited(SelectExitEventArgs args) => rayVisuals.SetActive(false);
}
