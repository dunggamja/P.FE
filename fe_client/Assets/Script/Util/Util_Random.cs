using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Util
{
    static MyRandom s_random = new MyRandom(1234);

    public static void  SetRandomSeed(uint _seed, uint _index = 0) => s_random.SetSeed(_seed, _index);
    public static uint  GetSeed()    => s_random.Seed;
    public static float Random01()   => s_random.NextFloat();  // 0.0 ~ 1.0
    public static int   Random100()  => s_random.Next(0, 100); // 0 ~ 99
    public static bool  Random100_Result(int _success_rate)  
    {
        return Random100() < _success_rate;
    }
    public static bool  Random01_Result(float _success_rate) 
    {
         var random = Random01();
         return (_success_rate < 1f) ? random < _success_rate : true;
    }

}