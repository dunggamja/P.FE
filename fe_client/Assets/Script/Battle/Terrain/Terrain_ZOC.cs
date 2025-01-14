using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Battle;
using UnityEngine;


public class Terrain_ZOC 
{
    int                     m_width;
    int                     m_height;
    Dictionary<int, int[,]> m_faction_zoc = new();


    public Terrain_ZOC(int _width, int _height)    
    {
        m_width  = _width;
        m_height = _height;

        // 진영이 보통 2개는 있으니까 만들어 둡시다.
        m_faction_zoc.Add(1, new int[m_width, m_height]);
        m_faction_zoc.Add(2, new int[m_width, m_height]);
    }

    public void IncreaseZOC(int _faction, int _x, int _y)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;
        
        if (_faction == 0)
            return;

        if (!m_faction_zoc.TryGetValue(_faction, out var data))
        {
            data = new int[m_width, m_height];
            m_faction_zoc.Add(_faction, data);
        }

        ++data[_x, _y];
    }

    public void DecreaseZOC(int _faction, int _x, int _y)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;

        if (_faction == 0)
            return;

        if (m_faction_zoc.TryGetValue(_faction, out var data))
        {
            if (data[_x, _y] > 0)
                --data[_x, _y];
        }
    }

    public bool IsBlockedZOC(int _x, int _y, int _faction_ignore)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return false;


        foreach((var faction, var data) in m_faction_zoc)
        {
            // 같은 진영은 통과가능.
            if (_faction_ignore == faction)
            {
                continue;
            }

            // var is_same_faction = false;
            // if (_faction_ignore != null)
            // {
            //     for (int i = 0; i < _faction_ignore.Length; ++i)
            //     {
            //         if (_faction_ignore[i] == faction)
            //         {
            //             is_same_faction = true;
            //             break;
            //         }
            //     }
            // }

            // // 같은 진영은 통과가능.
            // if (is_same_faction)
            //     continue;

            if (0 < data[_x, _y])
                return true;
        }

        return false;
    }

    // public static bool IsBlocked_ZOC(int _owner_zoc_faction, int _x, int _y)
    // {
    //     // 0은 통과시킨다는 뜻.
    //     if (_owner_zoc_faction == 0 || _terrain_zoc_faction == 0)
    //         return false;

    //     // 다른 진영이면 통과불가능.
    //     if (_owner_zoc_faction != _terrain_zoc_faction)
    //         return true;
        
    //     return false;
    // }
}
