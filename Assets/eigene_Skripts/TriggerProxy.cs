using UnityEngine;
public class TriggerProxy : MonoBehaviour
{
    private BaseComponent parentComponent;

    void Start() {
        // Sucht die BaseComponent im Parent
        parentComponent = GetComponentInParent<BaseComponent>();
    }

    private void OnTriggerEnter(Collider other) {
        if (parentComponent != null) {
            //parentComponent.HandleTrigger(other); // Ruf eine Methode im Parent auf
        }
    }
}