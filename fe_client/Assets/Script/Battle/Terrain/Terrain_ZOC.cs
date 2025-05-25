using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Battle;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;


public class Terrain_ZOC 
{
    int                        m_width;
    int                        m_height;
    Dictionary<int, List<int>> m_faction_zoc = new();


    public Terrain_ZOC(int _width, int _height)    
    {
        m_width  = _width;
        m_height = _height;

        //2개는 미리 만들어둠.
        CreateZOCArray(1);
        CreateZOCArray(2);
    }

    public void IncreaseZOC(int _faction, int _x, int _y)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;
        
        if (_faction == 0)
            return;


        if (!m_faction_zoc.ContainsKey(_faction))
            CreateZOCArray(_faction);

        var index = _x + _y * m_width;
        m_faction_zoc[_faction][index]++;

        Debug.Log($"IncreaseZOC: {_faction}, {_x}, {_y}, {m_faction_zoc[_faction][index]}");
    }

    public void DecreaseZOC(int _faction, int _x, int _y)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;

        if (_faction == 0)
            return;

        if (!m_faction_zoc.ContainsKey(_faction))
            return;

        var index = _x + _y * m_width;
        m_faction_zoc[_faction][index]--;

        Debug.Log($"DecreaseZOC: {_faction}, {_x}, {_y}, {m_faction_zoc[_faction][index]}");
    }

    public bool IsBlockedZOC(int _x, int _y, int _faction_ignore)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return false;


        foreach((var faction, var zoc_array) in m_faction_zoc)
        {
            // 같은 진영은 통과가능.
            if (_faction_ignore == faction)
                continue;

            // 셀 점유 확인.
            var index = _x + _y * m_width;
            if (0 < zoc_array[index])
                return true;
        }

        return false;
    }

    void CreateZOCArray(int _faction)
    {
        if (m_faction_zoc.ContainsKey(_faction))
            return;

        var zoc_array = new List<int>(m_width * m_height);
        for (int i = 0; i < m_width * m_height; ++i)
        {
            zoc_array.Add(0);
        }

        m_faction_zoc.Add(_faction, zoc_array);
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
