using System;

public class MyRandom
{
    // AI�� ¥�� ���� �ڵ�.
    private const int N           = 624;
    private const int M           = 397;
    private const uint MATRIX_A   = 0x9908B0DF; // constant vector a
    private const uint UPPER_MASK = 0x80000000; // most significant w-r bits
    private const uint LOWER_MASK = 0x7FFFFFFF; // least significant r bits

    private uint[]  mt    = new uint[N]; // ���� ����
    private uint    mti   = N + 1; // mti == N+1 means mt[N] is not initialized
    private uint    mseed = 0;

    public MyRandom(uint seed)
    {
        SetSeed(seed);
    }

    public uint Seed  { get { return mseed; } }
    public uint Index { get { return mti;   } }


    private void InitializeGenerator(uint seed)
    {
        mt[0] = seed & 0xffffffff; // �ʱ� �õ� ����
        for (mti = 1; mti < N; mti++)
        {
            mt[mti] = (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + (uint)mti);
            mt[mti] &= 0xffffffff; // 32��Ʈ�� ����
        }
    }

    private void IntializeVector()
    {
        // 624���� ���� ���͸� ����
        for (int k = 0; k < N - M; k++)
        {
            uint y = (mt[k] & UPPER_MASK) | (mt[k + 1] & LOWER_MASK);
            mt[k]  = mt[k + M] ^ (y >> 1) ^ (y % 2 == 0 ? 0 : MATRIX_A);
        }
        for (int k = N - M; k < N - 1; k++)
        {
            uint y = (mt[k] & UPPER_MASK) | (mt[k + 1] & LOWER_MASK);
            mt[k]  = mt[k + (M - N)] ^ (y >> 1) ^ (y % 2 == 0 ? 0 : MATRIX_A);
        }
        
        {
            uint y    = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
            mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ (y % 2 == 0 ? 0 : MATRIX_A);
        }

    }

    public void SetSeed(uint _seed, uint _index = 0)
    {
        mseed = _seed;
        mti   = _index;

        InitializeGenerator(_seed);
        IntializeVector();
    }

    public uint Next()
    {
        if (mti >= N) // ���� ���Ͱ� �ʱ�ȭ���� ���� ���
        {
            IntializeVector();
            mti = 0;
        }

        uint y = mt[mti++];
        // ��Ʈ ����ũ�� ����Ͽ� ���� ����
        y ^= (y >> 11);
        y ^= (y << 7) & 0x9d2c5680;
        y ^= (y << 15) & 0xefc60000;
        y ^= (y >> 18);
        return y & 0xffffffff; // 32��Ʈ�� ����
    }

    public int Next(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive)
        {
            throw new ArgumentException("minInclusive must be less than maxExclusive");
        }

        return (int)(Next() % (maxExclusive - minInclusive)) + minInclusive; // ������ ���� ������ ���� ���� ����
    }

    public float NextFloat()
    {
        return Next() / (float)uint.MaxValue; // 0.0 ~ 1.0 ������ ���� �ε� �Ҽ��� �� ����
    }

    public Int64 NextInt64()
    {
        Int64 high = (Int64)Next() << 32;
        Int64 low  = (Int64)Next() & 0xFFFFFFFF;
        return high | low;
    }

    public Int64 NextInt64(Int64 minInclusive, Int64 maxExclusive)
    {
        if (minInclusive >= maxExclusive)
        {
            throw new ArgumentException("minInclusive must be less than maxExclusive");
        }
        
        return ((Int64)NextInt64() % (maxExclusive - minInclusive)) + minInclusive;
    }

    public MyRandom_IO Save()
    {
        return new MyRandom_IO { Seed = mseed, Index = mti };
    }

    public void Load(MyRandom_IO _io)
    {
        SetSeed(_io.Seed, _io.Index);
    }
} 

public class MyRandom_IO 
{
    public uint Seed  { get; set; }
    public uint Index { get; set; }
}