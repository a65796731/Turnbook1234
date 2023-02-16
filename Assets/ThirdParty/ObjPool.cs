using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool<T> where T : new()
{
    int count;
    T obj;
    List<T> LifePool;
    Queue<T> DeathPool;
    public void CreatePool(int count)
    {
        this.count = count;
        DeathPool = new Queue<T>(count);
        LifePool = new List<T>();
        for (int i = 0; i < count; i++)
        {

            DeathPool.Enqueue(new T());
        }
    }
    public T New()
    {
        if (DeathPool.Count == 0)
        {
            for (int i = 0; i < count; i++)
            {

                DeathPool.Enqueue(new T());
            }
        }
        obj = DeathPool.Dequeue();
        LifePool.Add(obj);
        return obj;
    }
    public void ResetPool()
    {
        for (int i = 0; i < LifePool.Count; i++)
        {
            DeathPool.Enqueue(LifePool[i]);
        }
        LifePool.Clear();
    }
}
