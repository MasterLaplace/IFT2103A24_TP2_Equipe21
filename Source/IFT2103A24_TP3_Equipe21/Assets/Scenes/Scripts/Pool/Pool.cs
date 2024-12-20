using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Pool : Singleton<Pool>
{
    private readonly Stack<Poolable> pool = new();
    public GameObject objectPrefab;
    private Transform Cache => transform;

    protected override void Awake()
    {
        base.Awake();
        Cache.name = $"{objectPrefab.name} Pool";
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Simulation")
            Init(10);
        else
            Init(5);
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
            newInstance.OnUnpool();
            return newInstance;
        }

        T instance = pool.Pop() as T;
        instance.transform.parent = parent;
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
