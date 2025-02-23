using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IPoolObject
{
    void Reset();
}

// TODO: MonoBehaviour(GameObject)에 대한 풀링 처리 필요...

// public static class GameObjectPool<T> where T : MonoBehaviour
// {
//     private static readonly Stack<T>   m_pool  = new();
//     private static readonly HashSet<T> m_inuse = new();
// }

public static class ObjectPool<T> where T : IPoolObject, new() 
{
    static ObjectPool()
    {
        if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
        {
            throw new InvalidOperationException("ObjectPool<T>는 MonoBehaviour(GameObject)에 대한 풀링 처리를 하지 않습니다.");
        }
    }
    
    private static readonly Stack<T>   m_pool  = new();

    private static readonly HashSet<T> m_inuse = new();

    // public static bool IsPoolingObject



    public static T Acquire()
    {
        var pool_object = m_pool.Count > 0 ? m_pool.Pop() : new T();
        
        m_inuse.Add(pool_object);

        return pool_object;
    }

    public static void Return(T _obj)
    {
        if (_obj == null)
            return;

        // 풀링을 목적으로 생성된 객체가 아니라면 반환 처리 하지 않는다.
        if (m_inuse.Contains(_obj) == false)
            return;
        
        
        _obj.Reset();

        m_inuse.Remove(_obj);
        m_pool.Push(_obj);
    }
}

public static class ListPool<T>
{
    private static readonly Stack<List<T>> m_pool = new();

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
        m_pool.Push(list);
    }
}

public static class HashSetPool<T>
{
    private static readonly Stack<HashSet<T>> m_pool = new();

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
        m_pool.Push(hashSet);
    }
}

public static class DictionaryPool<TKey, TValue>
{
    private static readonly Stack<Dictionary<TKey, TValue>> m_pool = new();

    public static Dictionary<TKey, TValue> Acquire()
    {
        var dict = m_pool.Count > 0 ? m_pool.Pop() : new Dictionary<TKey, TValue>();
        return dict;
    }

    public static void Return(Dictionary<TKey, TValue> dict)
    {
        if (dict == null)
            return;

        dict.Clear();
        m_pool.Push(dict);
    }
}

