using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadedDataRequester : MonoBehaviour
{
    static ThreadedDataRequester instance;
    private Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    private void Awake()
    {
        instance = FindObjectOfType<ThreadedDataRequester>();
    }

    public static void RequestData(Func<object> generateData, Action<object> callBack)
    {
        ThreadStart threadStart = delegate
        {
            instance.DataThread(generateData, callBack);
        };

        new Thread(threadStart).Start();
    }

    private void DataThread(Func<object> generateData, Action<object> callBack)
    {
        object data = generateData();
        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadInfo(callBack, data));
        }
    }

    private void Update()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callBack(threadInfo.parameter);
            }
        }
    }

    private struct ThreadInfo
    {
        public readonly Action<object> callBack;
        public readonly object parameter;

        public ThreadInfo(Action<object> callBack, object parameter)
        {
            this.callBack = callBack;
            this.parameter = parameter;
        }
    }
}
