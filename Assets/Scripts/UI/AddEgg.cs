using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEgg : Upgrade
{
    public override bool IsOwned()
    {
        return false;
    }

    public override bool IsLocked()
    {
        SaveData data = SaveData.Get();
        int numEgg = data.GetPlayerHP();
        return numEgg >= Player.MaxEgg();
    }

    public override void Buy()
    {
        SaveData data = SaveData.Get();
        int numEgg = data.GetPlayerHP();
        if (numEgg < Player.MaxEgg())
            data.SetPlayerHP(numEgg + 1);
    }
}
