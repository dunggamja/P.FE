using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;


// public class CutsceneCondition_PlayOneShot : CutsceneCondition
// {
//     public override bool Verify(CutsceneSequence _sequence)
//     {
//         // 한번 실행된 컷씬은 다시 실행할 수 없다.
//         return _sequence.HasPlayed == false;
//     }
// }
