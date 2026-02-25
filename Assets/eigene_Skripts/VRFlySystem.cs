using UnityEngine;
using UnityEngine.InputSystem;

public class VRFlySystem : MonoBehaviour
{
    public InputActionReference rightJoystickAction; // XRI RightHand Locomotion/Move
    public InputActionReference leftJoystickAction; // XRI RightHand Locomotion/Move
    public float flySpeed = 4.0f;
    public Transform xrOriginTransform; // Ziehe dein XR Origin hier rein
    [SerializeField] public CharacterController characterController;
    private int count;


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
        //Debug.Log(rightJoystickAction.name + " " + inputRight.y + " enabled: " + rightJoystickAction.action.enabled);
        //Debug.Log(leftJoystickAction.name + " " + inputLeft.y +" " + inputLeft.x);
        // input.y ist oben/unten am Stick
        if (Mathf.Abs(inputRight.y) > 0.1f)
        {
            // Wir bewegen den gesamten XR Origin nur auf der Y-Achse
            characterController.Move(Vector3.up * inputRight.y * flySpeed * Time.deltaTime);
            //xrOriginTransform.position += Vector3.up * inputRight.y * flySpeed * Time.deltaTime;
            // inputLeft ist ein Vector2 vom linken Stick
            Vector3 moveDirection = new Vector3(inputLeft.x, 0, inputLeft.y);

            // Anwendung auf die Position
            //xrOriginTransform.position += moveDirection * flySpeed * Time.deltaTime;       
        }

    }
}