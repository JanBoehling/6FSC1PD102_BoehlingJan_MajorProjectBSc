using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            lock (_instanceLock)
            {
                if (_instance) return _instance;

                // Find T in scene
                _instance = GameObject.FindFirstObjectByType<T>();

                // If T exists in scene, return it
                if (_instance) return _instance;

                // Else, create new game object and attach T component
                var obj = new GameObject(typeof(T).ToString());
                _instance = obj.AddComponent<T>();

                DontDestroyOnLoad(_instance.gameObject);

                return _instance;
            }
        }
    }

    private static readonly object _instanceLock = new();
    private static T _instance = null;

    protected virtual void Awake()
    {
        // If instance is not set, get the component of this game object
        if (!_instance) _instance = gameObject.GetComponent<T>();

        DontDestroyOnLoad(_instance.gameObject);
    }
}
