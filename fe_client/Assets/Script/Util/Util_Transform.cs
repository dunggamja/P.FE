using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static partial class Util
{
   public static void DestroyAllChildren(this Transform _transform)
   {
      if (_transform == null)
         return;

      for(int i = _transform.childCount; i >= 0; --i)
      {
         var child = _transform.GetChild(i);
         if (child == null)
            continue;

         if (Application.isPlaying)
         {
            UnityEngine.Object.Destroy(child.gameObject);
         }
         else
         {
            UnityEngine.Object.DestroyImmediate(child.gameObject);
         }
      }
   }
    
}
