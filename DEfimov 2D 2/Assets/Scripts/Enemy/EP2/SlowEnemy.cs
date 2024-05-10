using UnityEngine;

public class SlowEnemy : Enemy
{
    // Adjusted variables for FastEnemy
    protected override void Start()
    {
        base.Start();
        speed = 2f;
        maxHP = 7;
    }

    // You can override other methods or add new ones as needed
}
