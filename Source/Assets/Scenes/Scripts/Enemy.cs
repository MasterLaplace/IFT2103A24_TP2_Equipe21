using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject SkyBox;
    private Vector3 mapLimit;
    [SerializeField] private float speed = 20f;
    private Vector3 direction;
    private int health = 100;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileVelocity = 40;
    [SerializeField] private float cooldown = 2f;
    private float lastShotTime;
    private List<Player> players;

    void Start()
    {
        mapLimit = SkyBox.transform.localScale / 2;

        transform.position = RandomPosition();
        ChooseRandomDirection();

        lastShotTime = Time.time;
    }

    void Update()
    {
        players = new List<Player>(FindObjectsOfType<Player>());

        if (IsOutOfBounds()) {
            ResolvedPosition();
            ChooseRandomDirection();
        }

        Player nearestPlayer = FindNearestPlayer();

        if (nearestPlayer != null && IsPlayerNear(nearestPlayer.transform.position))
        {
            if (!IsPlayerNear(nearestPlayer.transform.position, 5f))
                transform.position = Vector3.MoveTowards(transform.position, nearestPlayer.transform.position, speed * Time.deltaTime);

            if (Time.time - lastShotTime >= cooldown)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position + (2 * transform.forward), Quaternion.identity);
                projectile.GetComponent<Rigidbody>().velocity = (nearestPlayer.transform.position - transform.position).normalized * projectileVelocity;
                lastShotTime = Time.time;
            }
        }
        else
            transform.position += speed * Time.deltaTime * direction;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Destroy(gameObject);
    }

    private bool IsOutOfBounds()
    {
        return transform.position.x > mapLimit.x || transform.position.x < -mapLimit.x ||
               transform.position.y > mapLimit.y || transform.position.y < -mapLimit.y ||
               transform.position.z > mapLimit.z || transform.position.z < -mapLimit.z;
    }

    private void ResolvedPosition()
    {
        if (transform.position.x > mapLimit.x)
            transform.position = new Vector3(mapLimit.x, transform.position.y, transform.position.z);
        else if (transform.position.x < -mapLimit.x)
            transform.position = new Vector3(-mapLimit.x, transform.position.y, transform.position.z);
        if (transform.position.y > mapLimit.y)
            transform.position = new Vector3(transform.position.x, mapLimit.y, transform.position.z);
        else if (transform.position.y < -mapLimit.y)
            transform.position = new Vector3(transform.position.x, -mapLimit.y, transform.position.z);
        if (transform.position.z > mapLimit.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, mapLimit.z);
        else if (transform.position.z < -mapLimit.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, -mapLimit.z);
    }

    private Vector3 RandomPosition()
    {
        float x = Random.Range(-mapLimit.x, mapLimit.x);
        float y = Random.Range(-mapLimit.y, mapLimit.y);
        float z = Random.Range(-mapLimit.z, mapLimit.z);

        return new Vector3(x, y, z);
    }

    private void ChooseRandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);

        direction = new(x, y, z);
    }

    private Player FindNearestPlayer()

    {
        Player nearestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (Player player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }

    private bool IsPlayerNear(Vector3 playerPosition, float distance = 20f)
    {
        return Vector3.Distance(transform.position, playerPosition) < distance;
    }
}
