using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UMLObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string guid;// = System.Guid.NewGuid().ToString();
    public string parentGuid;     // ID des Vaters (damit wir wissen, wer der Glaskasten ist)
    public List<string> neighborGuids; // Liste der IDs aller Nachbarn
}


