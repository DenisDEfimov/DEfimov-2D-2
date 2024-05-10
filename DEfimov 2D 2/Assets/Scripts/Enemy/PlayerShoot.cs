using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform spawnPoint;
    private bool hasPressedButton = false;

    void Update()
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !hasPressedButton)
        {
            SpawnObject();
            hasPressedButton = true;
        }
        if (!Input.GetButton("Fire1") && !Input.GetKey(KeyCode.Space)) hasPressedButton = false;
    }

    void SpawnObject()
    {
        if (objectToSpawn != null && spawnPoint != null) Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
        else Debug.LogWarning("Object to spawn or spawn point is not assigned.");
    }
}
