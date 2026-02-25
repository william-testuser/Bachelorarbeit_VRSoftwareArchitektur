using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//braucht kein MonoBehaiour, da es ein reines Totes Datenobject ist
public class InformationNode
{
    private string _responsibility; // Das eigentliche Feld

    public string Responsibility 
    {
        get { return _responsibility; }
        set 
        { 
            _responsibility = value; 
            // Hier kannst du Code ausführen, sobald der Wert geändert wird!
            //UpdateUMLText(_responsibility); 
        }
    }
    public string Discription { get; private set; } 

    //Diese Methode liefert eine FormatioertenString den man in ein Textfeld einfach übernehmen kann 
    public string BuildText()
    {
        string returnText = string.Empty;

        return returnText;
    }
    
}
