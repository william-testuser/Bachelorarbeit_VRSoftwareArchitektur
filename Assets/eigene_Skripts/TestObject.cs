using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TestObject : MonoBehaviour
{
    [SerializeField] private ToolboxLogik toolbox;
    
    private bool test1 = false;
   
    void Update()
    {

       /*
            // Zeit des letzten Frames in Millisekunden
            float frameTimeMs = Time.unscaledDeltaTime * 1000;

            //13.88 ist die zeit die das rendern pro frame bei 72 Frames dauern darf 
            //ohne doppelframe zu erzeugen
            if (frameTimeMs > 13.88f) {
                Debug.LogWarning($"Performance-Einbruch: {frameTimeMs:F2} ms (Budget überschritten!)");
            }
            
        */
    }

    public void StartTests()
    {
          Debug.Log("Test1: erstellung über 500 Objekte ");
            try
            {
                List<BaseComponent> currentList = CreateFiveChilden(null);
                currentList = CreateFiveChilden(currentList);
                currentList = CreateFiveChilden(currentList);
                //currentList = CreateFiveChilden(currentList);
                test1 = true;
            }
            catch (System.Exception)
            {
                Debug.Log("Fehlschlag der generierung.");
                throw;
            }
            
        //Debug.Log("Test2: );
    }
    //bekommt eine liste mit objecten in die es jeweis 5 kinder erzeugt. mit level übergabe fibt eine liste mit allen erzeigeten objekten zurück.
    private List<BaseComponent> CreateFiveChilden(List<BaseComponent> currentList)
    {
        List<BaseComponent> returnListodNewComponents = new List<BaseComponent>();
        if(currentList == null)
        {
            for(int i = 0; i < 5; i++)
            {
                
                returnListodNewComponents.Add(toolbox.SpawnPrefab());
            }
            return returnListodNewComponents;
        }
        else
        {
            foreach (BaseComponent comp in currentList)
            {
                
                for(int i = 0; i < 5; i++)
                {
                    returnListodNewComponents.Add(toolbox.SpawnPrefab(comp));
                }
            }
            return returnListodNewComponents;
        }  
    }
}

