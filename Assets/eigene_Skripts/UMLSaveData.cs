using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class UMLSaveData
{
    // Eine Liste, die viele UMLObjectData-Steckbriefe enthalten kann
    public List<UMLObjectData> allObjects = new List<UMLObjectData>();
    public List<UMLLineData> allLines = new List<UMLLineData>();
}