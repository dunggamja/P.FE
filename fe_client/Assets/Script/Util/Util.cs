using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Util
{
    static long   s_last_generated_id = 0;
    public static long GenerateID() => ++s_last_generated_id;
    public static void InitializeGenerateID(Int64 _last_generated_id) => s_last_generated_id = _last_generated_id;


    static Dictionary<Type, Array> s_cached_enum_values = new();
    public static T[] CachedEnumValues<T>() where T : Enum
    {
        var type = typeof(T);
        if (!s_cached_enum_values.TryGetValue(type, out var values))
        {
            values = Enum.GetValues(type);
            s_cached_enum_values.Add(type, values);
        }
        
        return (T[])values;
    }



    //--------------------------------------
    // extension methods
    //--------------------------------------
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

    public static Vector3 CellToPosition(this (int x, int y) _cell) => new Vector3(_cell.x, 0f, _cell.y);
}
