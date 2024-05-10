using UnityEngine;

public class FastEnemy : Enemy
{
    // Adjusted variables for FastEnemy
    protected override void Start()
    {
        base.Start();
        speed = 5f; // Increase speed for FastEnemy
        maxHP = 2; // Decrease max HP for FastEnemy
    }

    // You can override other methods or add new ones as needed
}
