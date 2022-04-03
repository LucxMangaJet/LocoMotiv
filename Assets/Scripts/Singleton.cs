using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance == null)
            instance = (T)this;
        else
            Debug.LogError($"Singleton Instance already set for {typeof(T).Name}", this);
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
