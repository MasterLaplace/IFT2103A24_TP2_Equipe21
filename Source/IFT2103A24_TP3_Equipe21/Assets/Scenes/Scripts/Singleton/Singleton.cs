using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            // Chercher une instance déjà existante dans la scène
            if ((_instance = FindObjectOfType<T>()) != null)
                return _instance;

            // Créer une nouvelle instance si aucune n'existe
            GameObject singletonObject = new(typeof(T).Name);
            _instance = singletonObject.AddComponent<T>();

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
