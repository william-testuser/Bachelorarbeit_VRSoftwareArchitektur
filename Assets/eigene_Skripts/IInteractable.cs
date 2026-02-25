using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface IInteractable 
{
   public TextMeshProUGUI frontText {get;}
   public string InteractMassege {get;}
   public void Interact();
  // public void Initialize();
}
