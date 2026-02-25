using UnityEngine;
using TMPro;

public class VRTextEditor : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private TMP_InputField inputField;
    private TouchScreenKeyboard overlayKeyboard;

    public void OpenKeyboard()
    {
        // Öffnet das systemeigene VR-Keyboard (Quest/SteamVR)
        overlayKeyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
    }

    void Update()
    {
        if (overlayKeyboard != null && overlayKeyboard.active)
        {
            // Übertrage den getippten Text live in das InputField
            inputField.text = overlayKeyboard.text;
        }
    }

    public void SetInputField(TMP_InputField newInputField)
    {
        inputField = newInputField;
        //neue tastatur aktivieren?
    }
    public void UpdateInputFields(string name, string responsability, string description)
    {
        
    }

    public void CloseInfoScreen()
    {
        canvas.gameObject.SetActive(false);
    }
    //get die texte
    //updaten der texte
    

}