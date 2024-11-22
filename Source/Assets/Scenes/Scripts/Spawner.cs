using UnityEngine;

public class Spawner : MonoBehaviour
{
    private GameObject objectPrefab;
    private float spawnTime;
    private int maxObjects;

    public void Create(GameObject objectPrefab, float spawnTime, int maxObjects)
    {
        this.objectPrefab = objectPrefab;
        this.spawnTime = spawnTime;
        this.maxObjects = maxObjects;
        InvokeRepeating(nameof(SpawnObject), spawnTime, spawnTime);
    }

    private void SpawnObject()
    {
        if (GameObject.FindGameObjectsWithTag(objectPrefab.tag).Length < maxObjects)
            Instantiate(objectPrefab, transform.position, transform.rotation);
    }
}
