//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWarp_Refill : Upgrade
{
    public override bool IsOwned()
    {
        return BulletTime.Get().IsReady();
    }

    public override void Buy()
    {
        BulletTime bt = BulletTime.Get();
        int value = bt.GetTimeBarValue();
        bt.AddPoints(BulletTime.s_timeWarpPoints - value);
    }
}
