using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Component : BaseComponent
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
