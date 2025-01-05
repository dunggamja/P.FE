using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Util
{
    const  long   ID_DIGIT        = 1_000_000_000_000_000_000;
    const  long   ID_SEC_DIGIT    = 100_000;
    const  long   ID_DAY_DIGIT    = 1_000;
    const  long   ID_YEAR_DIGIT   = 100;
    const  long   ID_TIME_DIGIT   = ID_SEC_DIGIT * ID_DAY_DIGIT * ID_YEAR_DIGIT;
    const  long   ID_SERIAL_DIGIT = ID_DIGIT / ID_TIME_DIGIT;
    
    static long   s_last_generated_id = 0;
    public static long GenerateID() 
    {
        
        var   utc_now  = DateTime.UtcNow;
        Int64 utc_year = utc_now.Year % ID_YEAR_DIGIT;
        Int64 utc_day  = utc_now.DayOfYear;
        Int64 utc_sec  = (Int64)utc_now.TimeOfDay.TotalSeconds;

        Int64 generate_time =
        utc_year * (ID_SEC_DIGIT * ID_DAY_DIGIT) + 
        utc_day  * (ID_SEC_DIGIT) +
        utc_sec;

        // 시간이 바뀌었는지 체크해봅시다.
        if ((s_last_generated_id / ID_SERIAL_DIGIT) != generate_time)
        {
            s_last_generated_id = generate_time * ID_SERIAL_DIGIT;
        }

        return ++s_last_generated_id;
    }
    
    // public static void InitializeGenerateID(Int64 _last_generated_id) => s_last_generated_id = _last_generated_id;


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
