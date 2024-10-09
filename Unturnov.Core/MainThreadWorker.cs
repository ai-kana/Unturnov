using System.Collections.Concurrent;
using SDG.Unturned;
using UnityEngine;
using Action = System.Action;

namespace Unturnov.Core;

public class MainThreadWorker : MonoBehaviour
{
    private static ConcurrentQueue<WorkWrapper> WorkQueue = new();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (WorkQueue.TryDequeue(out WorkWrapper work))
        {
            work.Run();
        }
    }

    public static void Enqueue(Action work)
    {
        WorkQueue.Enqueue(new(work));
    }

    /// <summary>
    /// This is dogshit but fine since its really only used one shutdown when server stops anyway :P;
    /// Or at least should be...
    /// </summary>
    public static void EnqueueSync(Action work)
    {
        WorkWrapper wrapper = new(work);

        WorkQueue.Enqueue(wrapper);

        while (!wrapper.Finished);
    }

    private class WorkWrapper
    {
        public bool Finished {get; private set;}
        private Action _Work;

        public void Run()
        {
            _Work();
            Finished = true;
        }
        
        public WorkWrapper(Action work)
        {
            _Work = work;
            Finished = false;
        }
    }
}
