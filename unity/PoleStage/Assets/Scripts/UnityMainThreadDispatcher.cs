using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<System.Action> _executionQueue = new ConcurrentQueue<System.Action>();

    public static UnityMainThreadDispatcher Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void ExecuteOnMainThread(System.Action action)
    {
        if (action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        _executionQueue.Enqueue(action);
    }

    void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }
}
