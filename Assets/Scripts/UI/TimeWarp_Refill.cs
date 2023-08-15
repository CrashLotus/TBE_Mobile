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
