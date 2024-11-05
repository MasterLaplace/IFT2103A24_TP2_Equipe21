using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnTime = 3f;
    [SerializeField] private int maxEnemies = 10;

    void Start()
    {
        InvokeRepeating(nameof(SpawnObject), spawnTime, spawnTime);
    }

    void SpawnObject()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
            Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
}
