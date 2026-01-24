using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public static class CutsceneBuilder
{
   private static CutsceneSequence root = null;
   // public static CutsceneSequence BuildCutscene(string _cutscene_name)
   // {
   //    return new CutsceneSequence();
   // }

   public static void CreateRoot()
   {
      if (root != null)
         Debug.LogError("CutsceneBuilder: Root already created.");

      root = new CutsceneSequence();
   }
}