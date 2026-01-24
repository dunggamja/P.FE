// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Battle
// {
//     public class BattleSystem_Navigation : BattleSystem
//     {
//         public BattleSystem_Navigation() : base(EnumSystem.BattleSystem_Navigation)
//         {

//         }

//         public override void Reset()
//         {
            
//         }

//         protected override void OnEnter(IBattleSystemParam _param)
//         {
            
//         }

//         protected override bool OnUpdate(IBattleSystemParam _param)
//         {
//             // TODO: 
//             BattleObjectManager.Instance.Loop((e) =>
//             {
//                 if (e == null)
//                     return;

//                 // TODO: ê¸¸ì°¾ê¸°ê?? ?•„?š”?•  ê²½ìš° ?—¬ê¸°ì„œ ?…‹?Œ…?•œ?‹¤.??

//                 if (e.PathNodeManager != null)
//                     e.PathNodeManager.Update();

//                 if (e.PathVehicle != null)
//                     e.PathVehicle.Update(e, Time.deltaTime);
//             });

//             // TODO: NavigationSystem ì¢…ë£Œ?¼?Š” ê°œë…?´ ?•„?š”?• ê¹????;;;
//             return false;
//         }
        
//         protected override void OnExit(IBattleSystemParam _param)
//         {
            
//         }

//     }
// }