using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Util
{
    const  long   ID_DIGIT        = 1_000_000_000_000_000_000; // 10 ^ 18
    const  long   ID_SEC_DIGIT    = 100_000; // 10 ^ 5
    const  long   ID_DAY_DIGIT    = 1_000;   // 10 ^ 3
    const  long   ID_YEAR_DIGIT   = 100;     // 10 ^ 2
    const  long   ID_TIME_DIGIT   = ID_SEC_DIGIT * ID_DAY_DIGIT * ID_YEAR_DIGIT; // 10 ^ 10
    const  long   ID_SERIAL_DIGIT = ID_DIGIT / ID_TIME_DIGIT; // 10 ^ 8
    
    static long   s_last_generated_time   = 0;
    static long   s_last_generated_serial = 0;
    static long   s_last_generated_id     = 0;
    public static long GenerateID() 
    {
        // 현재 시간 가져오기
        var   utc_now  = DateTime.UtcNow;
        Int64 utc_year = utc_now.Year % ID_YEAR_DIGIT;
        Int64 utc_day  = utc_now.DayOfYear;
        Int64 utc_sec  = (Int64)utc_now.TimeOfDay.TotalSeconds;

        // 현재 시간을 초 단위로 변환
        Int64 generate_time =
        utc_year * (ID_SEC_DIGIT * ID_DAY_DIGIT) + 
        utc_day  * (ID_SEC_DIGIT) +
        utc_sec;



        // 시간이 바뀌었다면 time, serial 값 초기화.
        if (s_last_generated_time != generate_time)
        {
            s_last_generated_time   = generate_time;
            s_last_generated_serial = 0;
        }

        // ID 생성
        long gen_id = (s_last_generated_time * ID_SERIAL_DIGIT) + (++s_last_generated_serial);

        // // 이전 아이디보다 작은 경우 증가
        // if  (gen_id < s_last_generated_id)
        // {
        //      gen_id = s_last_generated_id + 1;
        // }

        // 이전 아이디 업데이트
        s_last_generated_id = gen_id;

        return gen_id;
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

    public static Vector3        CellToPosition(this (int x, int y) _cell) => new Vector3(_cell.x, 0f, _cell.y);
    public static (int x, int y) PositionToCell(this Vector3 _position)    => ((int)_position.x, (int)_position.z);


    public static float PERCENT(int _percent, bool _clamp01 = true)
    {
        var value = _percent * 0.01f;

        if (_clamp01)
            value = Mathf.Clamp01(value);

        return value;
    }


    
}
