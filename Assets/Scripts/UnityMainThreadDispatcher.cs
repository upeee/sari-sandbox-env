using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private static UnityMainThreadDispatcher _instance;

    public static UnityMainThreadDispatcher Instance()
    {
        if (!_instance)
        {
            _instance = FindFirstObjectByType<UnityMainThreadDispatcher>();
            if (!_instance)
            {
                var obj = new GameObject("UnityMainThreadDispatcher");
                _instance = obj.AddComponent<UnityMainThreadDispatcher>();
            }
        }
        return _instance;
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}