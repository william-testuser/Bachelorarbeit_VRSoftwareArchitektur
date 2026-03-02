using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class VRFlySystem : MonoBehaviour
{
    public InputActionReference rightJoystickAction; // XRI RightHand Locomotion/Move
    public InputActionReference leftJoystickAction; // XRI RightHand Locomotion/Move
    public float flySpeed = 4.0f;
    public Transform xrOriginTransform; // Ziehe dein XR Origin hier rein
    [SerializeField] public CharacterController characterController;
    private int count;

    [Header("Interaktions-Referenzen")]
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor leftHandInteractor;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor rightHandInteractor;

    void OnEnable()
    {
        // Falls die Referenz existiert, erzwingen wir die Aktivierung
        if (rightJoystickAction != null)
        {
            rightJoystickAction.action.Enable();
            Debug.Log($"Action Status nach Force-Enable: {rightJoystickAction.action.enabled}");
        }
        else
        {
            Debug.LogError("Die InputActionReference fehlt im Inspector!");
        }
    }

    void OnDisable()
    {
        //rightJoystickAction.action.Disable();
    }
    void Update()
    {
        // Wir lesen den Vector2 vom rechten Stick
        Vector2 inputRight = rightJoystickAction.action.ReadValue<Vector2>();
        Vector2 inputLeft = leftJoystickAction.action.ReadValue<Vector2>();
        
        /* input.y ist oben/unten am Stick
        if (Mathf.Abs(inputRight.y) > 0.1f)
        {
            // Wir bewegen den gesamten XR Origin nur auf der Y-Achse
            characterController.Move(Vector3.up * inputRight.y * flySpeed * Time.deltaTime);
                 
        }*/

        // 1. Prüfen, ob ÜBERHAUPT etwas gegriffen ist (Globaler Check)
        if (IsSomethingSelected()) return;

        // 2. Deine Flug-Logik
        
        if (Mathf.Abs(inputRight.y) > 0.1f)
        {
            characterController.Move(Vector3.up * inputRight.y * flySpeed * Time.deltaTime);
        }

    }
    private bool IsSomethingSelected()
    {
        // Wir holen uns alle aktiven Interaktoren vom Manager
        // Und prüfen, ob einer davon ein Objekt "selektiert" hat
        /*var interactors = interactionManager.activeInteractors;
        foreach (var interactor in interactors)
        {
            if (interactor.hasSelection) return true;
            
            // UI-Check: Wenn der Interactor ein UI-Treffer hat
            if (interactor is IXRRayInteractor rayInteractor)
            {
                if (rayInteractor.TryGetCurrentUIRaycastResult(out _)) return true;
            }
        }*/
        return false;
    }
}