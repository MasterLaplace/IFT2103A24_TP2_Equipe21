using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyProjectile : MonoBehaviour
{
    [SerializeField] private GameObject SkyBox;
    private Vector3 mapLimit;
    [SerializeField] private int damage = 50;

    void Start()
    {
        mapLimit = SkyBox.transform.localScale / 2;
    }

    void Update()
    {
        if (IsOutOfBounds())
        {
            Debug.Log("Projectile destroyed at " + transform.position);
            Destroy(gameObject);
        }

        if (TouchingEnemy())
        {
            Debug.Log("Projectile destroyed at " + transform.position);

            Destroy(gameObject);
        }
    }

    private bool IsOutOfBounds()
    {
        return transform.position.x > mapLimit.x || transform.position.x < -mapLimit.x ||
               transform.position.y > mapLimit.y || transform.position.y < -mapLimit.y ||
               transform.position.z > mapLimit.z || transform.position.z < -mapLimit.z;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, SkyBox.transform.localScale);
    }

    private bool TouchingEnemy()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds))
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
                return true;
            }
        }
        return false;
    }
}
