using UnityEngine;

public class DestroyProjectile : MonoBehaviour
{
    [SerializeField] private GameObject SkyBox;
    private Vector3 mapLimit;
    [SerializeField] public int damage { get; set; } = 50;

    public void Start()
    {
        mapLimit = SkyBox.transform.localScale / 2;
    }

    public void Update()
    {
        if (IsOutOfBounds())
            Destroy(gameObject);

        if (TouchingEnemy())
            Destroy(gameObject);
    }

    private bool IsOutOfBounds()
    {
        return transform.position.x > mapLimit.x || transform.position.x < -mapLimit.x ||
               transform.position.y > mapLimit.y || transform.position.y < -mapLimit.y ||
               transform.position.z > mapLimit.z || transform.position.z < -mapLimit.z;
    }

    private bool TouchingEnemy()
    {
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (target.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds))
            {
                target.GetComponent<Enemy>().TakeDamage(damage);
                return true;
            }
        }

        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (target.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds))
            {
                target.GetComponent<Player>().TakeDamage(damage);
                return true;
            }
        }

        return false;
    }
}
