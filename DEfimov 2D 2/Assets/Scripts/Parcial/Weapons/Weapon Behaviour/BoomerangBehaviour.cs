using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangBehaviour : ProjectileWeaponBehaviour
{
    private Vector2 startPosition;
    private bool returning = false;
    private float maxDistance = 8.0f; // Maximum distance before it returns
    private float rotationSpeed = 360.0f; // Rotation speed in degrees per second

    protected override void Start()
    {
        base.Start();
        startPosition = transform.position;
    }

    void Update()
    {
        float distanceTravelled = (transform.position - (Vector3)startPosition).magnitude;

        if (!returning && distanceTravelled >= maxDistance)
        {
            returning = true;
        }

        if (returning)
        {
            Vector2 returnDirection = ((Vector2)startPosition - (Vector2)transform.position).normalized;
            transform.position += (Vector3)returnDirection * currentSpeed * Time.deltaTime;
            
            //if (distanceTravelled <= 0.1f) // Close enough to the start position
            //{
            //    Destroy(gameObject); // Destroy the boomerang when it reaches the player
            //}
        }
        else
        {
            transform.position += direction * currentSpeed * Time.deltaTime;
        }

        RotateBoomerang();
    }

    private void RotateBoomerang()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
