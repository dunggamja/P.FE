using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class CutsceneCondition_Decorator : CutsceneCondition
{
   public CutsceneCondition DecoratedCondition { get; private set; } = null;

   public CutsceneCondition_Decorator()
   {
   }

   public void SetCondition(CutsceneCondition _condition)
   {
      DecoratedCondition = _condition;
   }
}


public class CutsceneCondition_Decorator_Not : CutsceneCondition_Decorator
{
   public CutsceneCondition_Decorator_Not() : base()
   {
   }

   public override bool Verify(CutsceneSequence _sequence)
   {
      return !DecoratedCondition.Verify(_sequence);
   }
}