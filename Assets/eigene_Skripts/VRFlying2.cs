using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
// Wir nutzen den Namespace für die Interactor-Basisklassen
using UnityEngine.XR.Interaction.Toolkit.Interactors; 
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
public class VRFlySystem2 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InputActionReference rightJoystickAction;
    [SerializeField] private float flySpeed = 2.0f;

    private bool uiHover = false;
    private bool grabbedSomething = false;

    [Header("Interactors")]
    // Wir nutzen XRBaseInputInteractor, da dein NearFarInteractor davon erbt
    [SerializeField] private XRBaseInputInteractor leftNearFarInteractor;
    [SerializeField] private XRBaseInputInteractor rightNearFarInteractor;

    void Update()
    {
        // 1. Abbruch, wenn eine Hand etwas hält oder über UI schwebt
        if (IsInteracting()) 
        {
            
            //return;
        }
        // 2. Flug-Logik
        Vector2 input = rightJoystickAction.action.ReadValue<Vector2>();
        if (Mathf.Abs(input.y) > 0.1f && !uiHover && !grabbedSomething)
        {
            characterController.Move(Vector3.up * input.y * flySpeed * Time.deltaTime);
        }
    }

    private bool IsInteracting()
    {
        // Check für die rechte Hand
        if (CheckHand(rightNearFarInteractor)) return true;
        
        // Check für die linke Hand
        if (CheckHand(leftNearFarInteractor)) return true;

        return false;
    }

    private bool CheckHand(XRBaseInputInteractor interactor)
    {
        if (interactor == null) {
            Debug.Log("weil kein interactor");
            return false;

        }
        // 1. Greift der Interactor gerade ein Objekt?
        if (interactor.hasSelection) return true;

        // 2. UI-Check ohne das widerspenstige Interface
        // Wir versuchen den Interactor als XRRayInteractor zu behandeln
        XRRayInteractor rayInteractor = interactor as XRRayInteractor;
        if (rayInteractor != null)
        {
            if (rayInteractor.TryGetCurrentUIRaycastResult(out _)) 
            {
                return true;
            }
        }

        return false;
    }

    public void IsHoveringInteractableField()
    {
        uiHover = true;
    }

    public void NotHoveringInteractableField()
    {
        uiHover = false;
    }

    public void IsGrabbingSomething()
    {
        grabbedSomething = true;
    }
    public void NotGrabbingSomething()
    {
        grabbedSomething = false;
    }
}