using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// »ù´¡µ¥Àý
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseSingleTon<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T GetInstance()
    {

        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(T)) as T;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent<T>();
            }
        }
        return _instance;

    }

}

