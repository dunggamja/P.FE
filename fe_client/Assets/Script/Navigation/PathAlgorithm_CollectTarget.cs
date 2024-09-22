using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static  partial class PathAlgorithm
{
   public static List<Int64> CollectTarget(TerrainMap _terrain_map, int _path_owner_attribute, int _x, int _y, int _move_range, int _weapon_range)
   {
        var list_target = new List<Int64>();

        if (_terrain_map != null)
        {
            var close_list = new HashSet<(int x, int y)>(10);




        }

        return list_target;

   }



}