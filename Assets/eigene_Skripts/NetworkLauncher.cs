using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkLauncher : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField ipInputField;
    
    [Header("Settings")]
    public string workSceneName = "KompoentScene";
    private int count;

    public void StartAsHost()
    {
        // 1. SceneManager von Netcode sagen, welche Szene geladen werden soll
        // WICHTIG: Nutze NetworkManager.SceneManager, nicht den normalen SceneManager!
        
       // ipInputField.text = "button geclickt";
       // NetworkManager.Singleton.StartHost();
        
        // Nur der Server/Host darf den Szenenwechsel für alle einleiten
        //NetworkManager.Singleton.SceneManager.LoadScene(workSceneName, LoadSceneMode.Single);

       if (NetworkManager.Singleton.StartHost())
    {
        Debug.Log("Host gestartet - wechsle Szene synchronisiert.");
        // NetworkManager übernimmt
        SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene(workSceneName, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.Log($"Fehler beim Szenenwechsel: {status}");
        }
    }
    else
    {
        Debug.LogWarning("Netzwerk fehlgeschlagen! Lade Arbeitsraum im Offline-Modus.");
        // Klassischer SceneManager übernimmt
        SceneManager.LoadScene(workSceneName);
    }
        
        
       

    }

    public void StartAsClient()
    {
        // IP aus dem Feld auslesen
        string ip = ipInputField.text;
        if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

        // IP im Transport setzen
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, 7777);

        // Als Client starten (er wartet dann, bis der Server ihn in die neue Szene "zieht")
        NetworkManager.Singleton.StartClient();
    }
}