using UnityEngine;
using TMPro;

public class VRTextEditor : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_InputField inputFieldname;
    [SerializeField] private TMP_InputField inputFieldResponability;
    [SerializeField] private TMP_InputField inputFieldDescription;
    [SerializeField] private TextMeshProUGUI titel;
    [SerializeField] private Transform uiForPostioning;
    private Vector3 resetPosition;

    private TouchScreenKeyboard overlayKeyboard;

    void Start()
    {
        resetPosition = uiForPostioning.position;
    }
    public void OpenKeyboard()
    {
        // Öffnet das systemeigene VR-Keyboard (Quest/SteamVR)
        //overlayKeyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
    }

    void Update()
    {
        if (overlayKeyboard != null && overlayKeyboard.active)
        {
            // Übertrage den getippten Text live in das InputField
            inputFieldname.text = overlayKeyboard.text;
        }
    }

    public void UpdateTitel(string var)
    {
        titel.text = var + "-Connection";
    }
    public void UpdateInputFields(string name, string responsability, string description)
    {

        inputFieldname.text = name;
        inputFieldResponability.text = responsability;
        inputFieldDescription.text = description;
        titel.text = name;

        //restliche Felder
    }

    public void CloseInfoScreen()
    {
        //componentName.text = "Dummy";
        if(canvas == null) Debug.Log("canvas = Null");
        else //Debug.Log("canvas = "+ canvas.name);
        canvas.gameObject.SetActive(false);
    }
    
    public void ClickUpdateButton()
    {
        UMLManager.Instance.UpdateBaseComponent(inputFieldname.text, inputFieldResponability.text, inputFieldDescription.text);
        titel.text = inputFieldname.text;
    }
    
    public void ResetPosition()
    {
        uiForPostioning.transform.position = resetPosition;
    }

}