using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretReload : IEInteractable
{
    public override void EndInteraction()
    {
    }

    public override void Interaction(string action = "")
    {
        if (action.Equals("R"))
        {
            Debug.Log("Hola");
            GameManager.Instance.placeAmmo(GetComponent<TurretsFather>());
        }
    }
}