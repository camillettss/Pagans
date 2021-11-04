﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceTracker<T> : MonoBehaviour where T : MonoBehaviour
{
    public static List<T> Instances { get; private set; } = new List<T>();
    int instanceIndex = 0;

    protected virtual void OnEnable()
    {
        instanceIndex = Instances.Count;
        Instances.Add(this as T);
    }

    protected virtual void OnDisable()
    {
        if(instanceIndex < Instances.Count)
        {
            var end = Instances.Count - 1;
            Instances[instanceIndex] = Instances[end];
            Instances.RemoveAt(end);
        }
    }
}
