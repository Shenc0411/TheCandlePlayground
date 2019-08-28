using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{

    private static T _instance;
    public static T Instance { get => _instance; }

    protected virtual void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("Instance of " + typeof(T).FullName + " already exists. Destorying new instance under " + gameObject.name);
            Destroy(this);
        }
        else
        {
            Debug.Log("Setting instance of " + typeof(T).FullName);
            _instance = this as T;
        }
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}
