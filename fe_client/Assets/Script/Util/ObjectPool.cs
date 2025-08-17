using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolObject
{
    void Reset();
}

// public class ObjectPoolWrapper<T> : IDisposable, IPoolObject where T : IPoolObject, new() 
// {
//     public T    Value      { get; private set; }
//     public bool IsDisposed { get; private set; }

//     public ObjectPoolWrapper()
//     {
//         IsDisposed = false;
//     }

//     public void Reset()
//     {
//         Value      = default;
//         IsDisposed = false;
//     }

//     public static ObjectPoolWrapper<T> Acquire()
//     {
//         var pool_object        = ObjectPool<ObjectPoolWrapper<T>>.Acquire();
//         pool_object.Value      = ObjectPool<T>.Acquire();
//         pool_object.IsDisposed = false;

//         return pool_object;
//     }


//     public void Dispose()
//     {
//         ObjectPool<T>.Return(Value);
//         IsDisposed = true;
//     }
// }


// TODO: MonoBehaviour(GameObject)에 대한 풀링 처리 필요...

// public static class GameObjectPool<T> where T : MonoBehaviour
// {
//     private static readonly Stack<T>   m_pool  = new();
//     private static readonly HashSet<T> m_inuse = new();
// }

public static class ObjectPool<T> where T :  class, IPoolObject, new()
{
    static ObjectPool()
    {
        if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
        {
            throw new InvalidOperationException("ObjectPool<T>는 MonoBehaviour(GameObject)에 대한 풀링 처리를 하지 않습니다.");
        }
    }
    
    private static readonly Stack<T>   m_pool  = new();

    // private static readonly HashSet<T> m_inuse = new();

    private static int m_max_count = 5;

    public static void SetMaxPoolCount(int _max_count)
    {
        m_max_count = _max_count;
    }

    // public static bool IsPoolingObject



    public static T Acquire()
    {
        var pool_object = m_pool.Count > 0 ? m_pool.Pop() : new T();
        
        // m_inuse.Add(pool_object);

        return pool_object;
    }

    public static void Return(ref T _obj)
    {
        if (_obj == null)
            return;

        // // 풀링을 목적으로 생성된 객체가 아니라면 반환 처리 하지 않는다.
        // if (m_inuse.Contains(_obj) == false)
        //     return;

        _obj.Reset();

        
        // 풀링 최대 개수를 초과하면 반환 처리 하지 않는다.
        if (m_pool.Count < m_max_count)  
        {
            m_pool.Push(_obj);
        }

        _obj = null;
    }
}

public static class ListPool<T>
{
    private static readonly Stack<List<T>> m_pool = new();

    private static int m_max_count = 5;

    public static void SetMaxPoolCount(int _max_count)
    {
        m_max_count = _max_count;
    }


    public static List<T> Acquire()
    {
        var list = m_pool.Count > 0 ? m_pool.Pop() : new List<T>();
        return list;
    }

    public static void Return(ref List<T> list)
    {
        if (list == null)
            return;

        list.Clear();

        // 풀링 최대 개수를 초과하면 반환 처리 하지 않는다.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(list);
        }

        list = null;
    }
}

public static class HashSetPool<T>
{
    private static readonly Stack<HashSet<T>> m_pool = new();

    private static int m_max_count = 5;

    public static void SetMaxPoolCount(int _max_count)
    {
        m_max_count = _max_count;
    }


    public static HashSet<T> Acquire()
    {
        var hashSet = m_pool.Count > 0 ? m_pool.Pop() : new HashSet<T>();
        return hashSet;
    }

    public static void Return(ref HashSet<T> hashSet)
    {
        if (hashSet == null)
            return;

        hashSet.Clear();

        // 풀링 최대 개수를 초과하면 반환 처리 하지 않는다.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(hashSet);
        }

        hashSet = null;
    }
}

public static class DictionaryPool<TKey, TValue>
{
    private static readonly Stack<Dictionary<TKey, TValue>> m_pool = new();

    private static int m_max_count = 5;

    public static void SetMaxPoolCount(int _max_count)
    {
        m_max_count = _max_count;
    }


    public static Dictionary<TKey, TValue> Acquire()
    {
        var    dict = m_pool.Count > 0 ? m_pool.Pop() : new Dictionary<TKey, TValue>();
        return dict;
    }

    public static void Return(ref Dictionary<TKey, TValue> dict)
    {
        if (dict == null)
            return;
        
        dict.Clear();

        // 풀링 최대 개수를 초과하면 반환 처리 하지 않는다.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(dict);
        }

        dict = null;
    }
}



public static class QueuePool<T>
{
    private static readonly Stack<Queue<T>> m_pool = new();
    private static int m_max_count = 5;


    public static Queue<T> Acquire()
    {
        var    queue = m_pool.Count > 0 ? m_pool.Pop() : new Queue<T>();
        return queue;
    }

    public static void Return(ref Queue<T> queue)
    {
        if (queue == null)
            return;

        queue.Clear();

        // 풀링 최대 개수를 초과하면 반환 처리 하지 않는다.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(queue);
        }

        queue = null;
    }
}

