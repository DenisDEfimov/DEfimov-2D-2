using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : WeaponController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedBoomerang = Instantiate(weaponData.Prefab);
        spawnedBoomerang.transform.position = transform.position; // Assign the position to be the same as this object which is parented to the player
        spawnedBoomerang.GetComponent<BoomerangBehaviour>().DirectionChecker(pm.lastMovedVector); // Reference and set the direction
    }
}
