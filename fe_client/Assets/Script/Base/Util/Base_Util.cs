using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Util
{
    static long   s_last_generated_id = 0;
    public static long GenerateID() => ++s_last_generated_id;


    public static void SetRandomSeed(int _seed)  => UnityEngine.Random.InitState(_seed);
    public static int  GetRandomRate()           => UnityEngine.Random.Range(0, 100);
    public static bool Random(int _success_rate) => GetRandomRate() < _success_rate;
}
