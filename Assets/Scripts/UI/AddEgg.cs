//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class AddEgg : Upgrade
{
    public override bool IsOwned()
    {
        return false;
    }

    public override bool IsLocked()
    {
        SaveData data = SaveData.Get();
        int numEgg = Player.NumEgg();
        int maxEgg = Player.MaxEgg();
        if (data.HasUpgrade("MEGALASER"))
            maxEgg *= 3;
        else if (data.HasUpgrade("MULTISHOT"))
            maxEgg *= 2;
        return numEgg >= maxEgg;
    }

    public override void Buy()
    {
        SaveData data = SaveData.Get();
        int numEgg = Player.NumEgg();
        int maxEgg = Player.MaxEgg();
        if (data.HasUpgrade("MEGALASER"))
            maxEgg *= 3;
        else if (data.HasUpgrade("MULTISHOT"))
            maxEgg *= 2;
        if (numEgg < maxEgg)
            data.SetPlayerHP(numEgg + 1);
    }
}
