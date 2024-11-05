using System.Collections;
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

    void Start()
    {
        mapLimit = SkyBox.transform.localScale / 2;

        transform.position = RandomPosition();
        ChooseRandomDirection();

        lastShotTime = Time.time;
    }

    void Update()
    {
        if (IsOutOfBounds()) {
            ResolvedPosition();
            ChooseRandomDirection();
        }

        if (IsObjectNear())
            ChooseRandomDirection();
        else if (IsPlayerNear())
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, speed * Time.deltaTime);
            if (Time.time - lastShotTime >= cooldown)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position + (2 * transform.forward), Quaternion.identity);
                projectile.GetComponent<Rigidbody>().velocity = (Camera.main.transform.position - transform.position).normalized * projectileVelocity;
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
        {
            Destroy(gameObject);
        }
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
            transform.position = new Vector3(-mapLimit.x, transform.position.y, transform.position.z);
        else if (transform.position.x < -mapLimit.x)
            transform.position = new Vector3(mapLimit.x, transform.position.y, transform.position.z);
        if (transform.position.y > mapLimit.y)
            transform.position = new Vector3(transform.position.x, -mapLimit.y, transform.position.z);
        else if (transform.position.y < -mapLimit.y)
            transform.position = new Vector3(transform.position.x, mapLimit.y, transform.position.z);
        if (transform.position.z > mapLimit.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, -mapLimit.z);
        else if (transform.position.z < -mapLimit.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, mapLimit.z);
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

    private bool IsPlayerNear()
    {
        return Vector3.Distance(transform.position, Camera.main.transform.position) < 20f;
    }

    private bool IsObjectNear()
    {
        return Vector3.Distance(transform.position, Camera.main.transform.position) < 5f;
    }
}
