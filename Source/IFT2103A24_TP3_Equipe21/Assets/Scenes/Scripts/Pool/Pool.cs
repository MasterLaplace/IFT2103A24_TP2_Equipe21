using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Pool : Singleton<Pool>
{
    private readonly Stack<Poolable> pool = new();
    public GameObject objectPrefab;
    private Transform Cache => transform;
    private float lastTime = 0.0f;
    private const float timeBeforeDelete = 2.0f;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Cache.name = $"{objectPrefab.name} Pool";

        if (scene.name == "Simulation")
            Init(10);
        else
            Init(5);
    }

    public void FixedUpdate()
    {
        if (Time.time - lastTime > timeBeforeDelete)
        {
            lastTime = Time.time;

            // if (pool.Count >= 10)
            //     return;

            // Poolable instance = pool.Peek();

            // if (instance.gameObject.activeSelf)
            //     return;

            // Destroy(instance.gameObject);
            // pool.Pop();
        }
    }

    public void Init(uint quantity)
    {
        pool.Clear();

        for (uint i = 0; i < quantity; ++i)
        {
            Poolable instance = Instantiate(objectPrefab).GetComponent<Poolable>();
            instance.transform.parent = Cache;
            pool.Push(instance);
        }
    }

    public T Get<T>(Transform parent) where T : Poolable
    {
        if (pool.Count == 0)
        {
            T newInstance = Instantiate(objectPrefab).GetComponent<T>();
            newInstance.transform.parent = parent;
            newInstance.transform.position = parent.position;
            newInstance.OnUnpool();
            return newInstance;
        }

        T instance = pool.Pop() as T;
        instance.transform.parent = parent;
        instance.transform.position = parent.position;
        instance.OnUnpool();
        return instance;
    }

    public void Return(Poolable instance)
    {
        pool.Push(instance);
        instance.transform.parent = Cache;
        instance.OnPool();
    }
}
