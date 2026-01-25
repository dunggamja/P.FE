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
   public static CutsceneSequence Root  { get; private set; } = null;
   // public static CutsceneSequence BuildCutscene(string _cutscene_name)
   // {
   //    return new CutsceneSequence();
   // }

   public static void CreateRoot()
   {
      if (Root != null)
         Debug.LogError("CutsceneBuilder: Root already created.");

      Root = new CutsceneSequence();
   }


   public static void RegisterCutscene(string _cutscene_name)
   {
      if (string.IsNullOrEmpty(_cutscene_name))
         return;

      CutsceneManager.Instance.RegisterCutscene(_cutscene_name, Root);
      Root = null;
   }

}