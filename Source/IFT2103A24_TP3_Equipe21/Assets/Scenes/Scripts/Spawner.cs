using UnityEngine;

public class Spawner : MonoBehaviour
{
    private GameObject objectPrefab;
    private float spawnTime;
    private int maxObjects;

    public void Setup(GameObject objectPrefab, float spawnTime, int maxObjects)
    {
        this.objectPrefab = objectPrefab;
        this.spawnTime = spawnTime;
        this.maxObjects = maxObjects;
    }

    public void Create(GameObject objectPrefab)
    {
        Instantiate(objectPrefab, transform.position, transform.rotation);
    }

    public GameObject CreateEmpty()
    {
        return Instantiate(new GameObject(), transform.position, transform.rotation);
    }

    public void Start()
    {
        InvokeRepeating(nameof(SpawnObject), spawnTime, spawnTime);
    }

    public void Stop()
    {
        CancelInvoke(nameof(SpawnObject));
    }

    private void SpawnObject()
    {
        if (GameObject.FindGameObjectsWithTag(objectPrefab.tag).Length < maxObjects)
            Instantiate(objectPrefab, transform.position, transform.rotation);
    }
}
