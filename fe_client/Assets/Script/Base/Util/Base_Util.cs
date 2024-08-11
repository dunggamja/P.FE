using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Util
{
    static long   s_last_generated_id = 0;
    public static long GenerateID() => ++s_last_generated_id;


    public static void SetRandomSeed(int _seed)            => UnityEngine.Random.InitState(_seed);
    public static int  Random100()                         => UnityEngine.Random.Range(0, 100);
    public static bool Random100_Result(int _success_rate) => Random100() < _success_rate;


    public static T TryAddComponent<T>(this GameObject _object) where T : Component
    {
        if (_object == null)
            return null;

        if (!_object.TryGetComponent<T>(out var result))
        {
            result = _object.AddComponent<T>();
        }

        return result;
    }
}
