using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

        public PlayerController controller;
    
    public void UseRoyalJelly()
    {
        if (controller != null)
        {
             controller.UseHealItem(PlayerController.healthType.royalJelly);
        }
    }

    public void UseStimPack()
    {
        if (controller != null)
        {
            controller.UseHealItem(PlayerController.healthType.stimPack);
        }
    }

        public void UseBandage()
    {
        if (controller != null)
        {
            controller.UseHealItem(PlayerController.healthType.bandage);
        }
    }
}


