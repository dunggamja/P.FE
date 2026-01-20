using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;


public enum EnumCutsceneAction
{
    None      = 0,

    Wait      = 1,   // 대기

    Dialogue  = 10,   // 대화 연출
    Unit_Move = 20,
}

public abstract class Cutscene_Action
{
   public abstract EnumCutsceneAction Action { get; }
   public abstract UniTask Execute();
}


public class Cutscene
{
   // List<Cutscene_Action> Actions
}