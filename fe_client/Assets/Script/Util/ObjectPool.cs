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


public static class ObjectPool<T> where T :  class, IPoolObject, new()
{
    public struct Wrapper : IDisposable
    {
        public T Value { get; private set; }

        internal Wrapper(T _value)
        {
            Value = _value;
        }

        public void Dispose()
        {
            ObjectPool<T>.Return(Value);
        }        
    }


    static ObjectPool()
    {
        if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
        {
            throw new InvalidOperationException("ObjectPool<T>�� MonoBehaviour(GameObject)�� ���� Ǯ�� ó���� ���� �ʽ��ϴ�.");
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

    public static Wrapper AcquireWrapper()
    {
        return new Wrapper(Acquire());
    }

    public static void Return(T _obj)
    {
        if (_obj == null)
            return;

        // // Ǯ���� �������� ������ ��ü�� �ƴ϶�� ��ȯ ó�� ���� �ʴ´�.
        // if (m_inuse.Contains(_obj) == false)
        //     return;

        _obj.Reset();

        
        // Ǯ�� �ִ� ������ �ʰ��ϸ� ��ȯ ó�� ���� �ʴ´�.
        if (m_pool.Count < m_max_count)  
        {
            m_pool.Push(_obj);
        }

        _obj = null;
    }
}

public static class ListPool<T>
{
    public struct Wrapper : IDisposable
    {
        public List<T> Value { get; private set; }

        internal Wrapper(List<T> _value)
        {
            Value = _value;
        }

        public void Dispose()
        {
            ListPool<T>.Return(Value);
        }        
    }


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

    public static void Return(List<T> list)
    {
        if (list == null)
            return;

        list.Clear();

        // Ǯ�� �ִ� ������ �ʰ��ϸ� ��ȯ ó�� ���� �ʴ´�.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(list);
        }

        list = null;
    }

    public static Wrapper AcquireWrapper()
    {
        return new Wrapper(Acquire());
    }
}

public static class HashSetPool<T>
{
    public struct Wrapper : IDisposable
    {
        public HashSet<T> Value { get; private set; }

        internal Wrapper(HashSet<T> _value)
        {
            Value = _value;
        }

        public void Dispose()
        {
            HashSetPool<T>.Return(Value);
        }  
    }
      

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

    public static void Return(HashSet<T> hashSet)
    {
        if (hashSet == null)
            return;

        hashSet.Clear();

        // Ǯ�� �ִ� ������ �ʰ��ϸ� ��ȯ ó�� ���� �ʴ´�.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(hashSet);
        }

        hashSet = null;
    }

    public static Wrapper AcquireWrapper()
    {
        return new Wrapper(Acquire());
    }
}

public static class DictionaryPool<TKey, TValue>
{
    public struct Wrapper : IDisposable
    {
        public Dictionary<TKey, TValue> Value { get; private set; }

        internal Wrapper(Dictionary<TKey, TValue> _value)
        {
            Value = _value;
        }

        public void Dispose()
        {
            DictionaryPool<TKey, TValue>.Return(Value);
        }
    }

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

    public static void Return(Dictionary<TKey, TValue> dict)
    {
        if (dict == null)
            return;
        
        dict.Clear();

        // Ǯ�� �ִ� ������ �ʰ��ϸ� ��ȯ ó�� ���� �ʴ´�.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(dict);
        }

        dict = null;
    }

    public static Wrapper AcquireWrapper()
    {
        return new Wrapper(Acquire());
    }
}



public static class QueuePool<T>
{
    public struct Wrapper : IDisposable
    {
        public Queue<T> Value { get; private set; }

        internal Wrapper(Queue<T> _value)
        {
            Value = _value;
        }

        public void Dispose()
        {
            QueuePool<T>.Return(Value);
        }
    }

    private static readonly Stack<Queue<T>> m_pool = new();
    private static int m_max_count = 5;


    public static Queue<T> Acquire()
    {
        var    queue = m_pool.Count > 0 ? m_pool.Pop() : new Queue<T>();
        return queue;
    }

    public static void Return(Queue<T> queue)
    {
        if (queue == null)
            return;

        queue.Clear();

        // Ǯ�� �ִ� ������ �ʰ��ϸ� ��ȯ ó�� ���� �ʴ´�.
        if (m_pool.Count < m_max_count)
        {
            m_pool.Push(queue);
        }

        queue = null;
    }

    public static Wrapper AcquireWrapper()
    {
        return new Wrapper(Acquire());
    }
}



