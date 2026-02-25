using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;



public class InteractoinController : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] TextMeshProUGUI interactionText;
    BaseComponent currentTargetInteractable;
    [SerializeField] InputActionProperty inputRight;
    [SerializeField] InputActionProperty inputLeft;
    //[SerializeField] UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rightHandRayInteractor;
    [SerializeField] private XRBaseInteractor nearFarInteractor;


    // Update is called once per frame
    public void Update()
    {
        //rightHandRayInteractor = GetComponentInChilden(RayInteractor);
       /* UpdateCurrentInteractable();
        UpdateInteractableText();
        CheckForInteractionInput();*/

    }

    void UpdateCurrentInteractable()
    {
        var rayInteractor = nearFarInteractor.GetComponent<XRRayInteractor>();
    
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Versuch, dein UML-Skript vom getroffenen Objekt zu holen
            var interactable = hit.collider.GetComponent<BaseComponent>(); 

            if (interactable != null)
            {
                currentTargetInteractable = interactable;
                Debug.Log("UML Objekt erkannt: " + hit.collider.name);
            }
            else
            {
                currentTargetInteractable = null; // Wir zielen ins Leere
            }
        }
        else
        {
            currentTargetInteractable = null; // Der Strahl trifft gar nichts
        }
    }

    void UpdateInteractableText()
    {
        if (currentTargetInteractable == null)
        {
            interactionText.text = string.Empty;
            return;
        }
        //Vector3 objectPos = (currentTargetInteractable as MonoBehaviour).gameObject.transform.position;
        //Vector3 offset = new Vector3(0, 0.5f, 0); // Schwebend über dem Objekt
        //interactionText.transform.position = objectPos + offset;
        //currentTargetInteractable.frontText.text = currentTargetInteractable.InteractMassege;
        // 3. "Billboard" Effekt: Text zur Kamera drehen
    // Damit man den Text immer lesen kann, egal von wo man schaut
    //interactionText.transform.LookAt(Camera.main.transform);
    //interactionText.transform.Rotate(0, 180, 0); // Korrektur, da Text sonst spiegelverkehrt ist

    }

    void CheckForInteractionInput()
    {
        if ((inputLeft.action.WasPressedThisFrame() || inputRight.action.WasPressedThisFrame())&& currentTargetInteractable != null)
        {
            
            currentTargetInteractable.Test();
        }
    }

    /*private IInteractable GetTargetFrom(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
    {
        if (interactor == null) return null;

        List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable> targets = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable>();
        interactor.GetValidTargets(targets);

        if (targets.Count > 0)
        {
            // Wir suchen am getroffenen GameObject nach deinem Interface
            return (targets[0] as MonoBehaviour)?.GetComponent<IInteractable>();
        }

        return null;
    }*/
}
