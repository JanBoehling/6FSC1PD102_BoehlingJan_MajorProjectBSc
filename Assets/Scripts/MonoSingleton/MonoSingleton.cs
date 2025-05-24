using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object instanceLock = new();
    private static T instance = null;

    public static T Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance) return instance;

                // Find T in scene
                instance = GameObject.FindFirstObjectByType<T>();

                // If T exists in scene, return it
                if (instance) return instance;

                // Else, create new game object and attach T component
                var obj = new GameObject(typeof(T).ToString());
                instance = obj.AddComponent<T>();

                DontDestroyOnLoad(instance.gameObject);

                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        // If instance is not set, get the component of this game object
        if (!instance) instance = gameObject.GetComponent<T>();

        // Otherwise, check if this object differs from the object set in instance. If so, destroy it.
        //else if (instance.GetInstanceID() != GetInstanceID())
        //{
        //    Destroy(gameObject);
        //    Debug.LogError($"Instance of {this.GetType().FullName} already exists, removing {this}");
        //}

        DontDestroyOnLoad(instance.gameObject);
    }
}
