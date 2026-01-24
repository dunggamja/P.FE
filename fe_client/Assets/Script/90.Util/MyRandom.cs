using System;
using Unity.Android.Types;
using UnityEngine;

public class MyRandom
{
    public int Seed => UnityEngine.Random.state.GetHashCode();
    public MyRandom(int seed)
    {
        SetSeed(seed);
    }

    public void SetSeed(int _seed)
    {
        UnityEngine.Random.InitState(_seed);
    }


    public int Next(int minInclusive, int maxExclusive)
    {
        return UnityEngine.Random.Range(minInclusive, maxExclusive);
    }

    public float NextFloat()
    {
        // 0.0 ~ 1.0 범위의 랜덤 부동 소수점 수 생성
        return UnityEngine.Random.Range(0f, 1f); 
    }

    public MyRandom_IO Save()
    {
        return new MyRandom_IO { Seed = UnityEngine.Random.state.GetHashCode() };
    }

    public void Load(MyRandom_IO _io)
    {
        SetSeed(_io.Seed);
    }
} 

public class MyRandom_IO 
{
    public int Seed  { get; set; }
}