using UnityEngine;
using TMPro;

public class VRTextEditor : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_InputField inputFieldname;
    [SerializeField] private TMP_InputField inputFieldResponability;
    [SerializeField] private TMP_InputField inputFieldDescription;
    [SerializeField] private TextMeshProUGUI componentName;
    private TouchScreenKeyboard overlayKeyboard;
    [SerializeField] private Canvas canvasObject;

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

    public void SetInputField(TMP_InputField newInputField)
    {
        inputFieldname = newInputField;
        //neue tastatur aktivieren?
    }
    public void UpdateInputFields(string name, string responsability, string description)
    {

        inputFieldname.text = name;
        inputFieldResponability.text = responsability;
        inputFieldDescription.text = description;
        componentName.text = name;

        //restliche Felder
    }

    public void CloseInfoScreen()
    {
        //componentName.text = "Dummy";
        if(canvas == null) Debug.Log("canvas = Null");
        else Debug.Log("canvas = "+ canvas.name);
        Debug.Log("close Screen canvasUI" + canvasObject.name + canvas.name);
        //canvas.gameObject.SetActive(false);
        canvasObject.gameObject.SetActive(false);
    }
    
    public void ClickUpdateButton()
    {
        UMLManager.Instance.UpdateBaseComponent(inputFieldname.text, inputFieldResponability.text, inputFieldDescription.text);
        componentName.text = inputFieldname.text;
    }
    
    

}