using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    // Called when the object is pooled
    // This is where you should reset the object's state
    public virtual void OnPool()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the object is taken out of the pool.
    /// </summary>
    public virtual void OnUnpool()
    {
        gameObject.SetActive(true);
    }
}
