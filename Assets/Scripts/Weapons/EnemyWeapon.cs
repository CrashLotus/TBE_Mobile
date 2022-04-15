using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    protected override Vector3 Aim()
    {
        return Dir2Player(transform.position);
    }
}
