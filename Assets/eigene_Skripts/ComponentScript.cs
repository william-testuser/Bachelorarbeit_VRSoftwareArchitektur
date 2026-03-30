using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Component : BaseComponent
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // checkt ab der Ray en Cube anvisiert hat über RayHover und gleichzeitig der 
    // Trigger gedrückt wird, wenn ja dann informiert die Methode Call DadToDoTheWork)() 
    // den UMLManager um die Metadaten upzudaten
    void Update()
    {
         if (GetRayHover())
        {
            //Debug.Log("updated");
            if(rightTriggerAction.action.triggered) CallDadToDoTheWork();
        }
        // Sanftes Anpassen der Größe (Interpolation)
        //transform.localScale = Vector3.Lerp(transform.localScale, _currentGoal, Time.deltaTime * speed);
    }
    
}
