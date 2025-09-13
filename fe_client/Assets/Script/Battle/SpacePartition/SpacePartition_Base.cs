using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    namespace SpacePartition
    {
        public delegate bool QueryFilter((int x, int y) _position, Int64 _id);
    }


    public struct AABB 
    {
      public Vector2 min, max;

      public void SetMinMax(Vector2 _min, Vector2 _max)
      {
        min = _min;
        max = _max;
      }

      public bool Intersects(AABB o) 
      {
        return !((max.x < o.min.x || o.max.x < min.x) ||
                (max.y < o.min.y || o.max.y < min.y));
      }

      public float Perimeter() 
      {
        float wx = max.x - min.x;
        float wy = max.y - min.y;

        return 2f * (wx + wy);
      }

      public bool Contains(AABB _box)
      {
        return min.x <= _box.min.x && _box.max.x <= max.x
            && min.y <= _box.min.y && _box.max.y <= max.y;
      }

      public static AABB Combine(AABB _a, AABB _b)
      {
        return new AABB 
        { 
          min = Vector2.Min(_a.min, _b.min),
          max = Vector2.Max(_a.max, _b.max) 
        };
      }

      public static AABB Fatten(AABB _box, float _margin) 
      {
        var v = new Vector2(_margin, _margin);
        
        return new AABB 
        { 
          min = _box.min - v, 
          max = _box.max + v 
        };
      }


      // public static AABB FromCenterRadius(Vector2 c, float r) {
      //   return new AABB { min = c - new Vector2(r, r), max = c + new Vector2(r, r) };
      // }
      // public static bool IntersectsCircle(AABB a, Vector2 c, float r) {
      //   float cx = Mathf.Clamp(c.x, a.min.x, a.max.x);
      //   float cy = Mathf.Clamp(c.y, a.min.y, a.max.y);
      //   float dx = c.x - cx, dy = c.y - cy;
      //   return dx*dx + dy*dy <= r*r;
      // }
    }

}