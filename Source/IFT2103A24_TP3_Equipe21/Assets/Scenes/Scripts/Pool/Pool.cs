using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private Stack<Poolable> pool;
    public GameObject objectPrefab;
    private Transform Cache => transform;

    public void Init(uint quantity)
    {
        pool = new Stack<Poolable>();

        for (uint i = 0; i < quantity; ++i)
        {
            Poolable instance = Instantiate(objectPrefab).GetComponent<Poolable>();
            instance.transform.parent = Cache;
            pool.Push(instance);
        }
    }

    public Poolable Get(Transform parent)
    {
        if (pool.Count == 0)
        {
            Poolable newInstance = Instantiate(objectPrefab).GetComponent<Poolable>();
            newInstance.transform.parent = parent;
            newInstance.OnUnpool();
            return newInstance;
        }

        Poolable instance = pool.Pop();
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
