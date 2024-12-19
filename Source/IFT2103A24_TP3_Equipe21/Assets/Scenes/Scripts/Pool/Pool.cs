using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : Poolable, new()
{
    private Stack<T> pool;
    public GameObject objectPrefab;
    private Transform Cache => transform;

    public void Init(uint quantity)
    {
        pool = new Stack<T>();

        for (uint i = 0; i < quantity; ++i)
        {
            T instance = new();
            instance.transform.parent = Cache;
            pool.Push(instance);
        }
    }

    public T Get(Transform parent, params object[] args)
    {
        if (pool.Count == 0)
        {
            T newInstance = new();
            newInstance.transform.parent = parent;
            newInstance.OnUnpool(args);
            return newInstance;
        }

        T instance = pool.Pop();
        instance.transform.parent = parent;
        instance.OnUnpool(args);
        return instance;
    }

    public void Return(T instance)
    {
        pool.Push(instance);
        instance.transform.parent = Cache;
        instance.OnPool();
    }
}
