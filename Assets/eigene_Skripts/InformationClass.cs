using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//braucht kein MonoBehaiour, da es ein reines Totes Datenobject ist
//speichert die Metasaten als Datenobjekt einer Komponente
public class InformationNode
{
    private string _responsibility; // Das eigentliche Feld

    public string responsibility 
    {
        get { return _responsibility; }
        set 
        { 
            _responsibility = value; 
            // Hier kannst du Code ausführen, sobald der Wert geändert wird!
            //UpdateUMLText(_responsibility); 
        }
    }
    public string discription { get; set; } 
    public string name { get; set; } 

    //Diese Methode liefert eine FormatioertenString den man in ein Textfeld einfach übernehmen kann 
    public string BuildText()
    {
        string returnText = string.Empty;

        return returnText;
    }
    
}
