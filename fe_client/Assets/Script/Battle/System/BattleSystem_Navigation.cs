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

//                 // TODO: 길찾기�?? ?��?��?�� 경우 ?��기서 ?��?��?��?��.??

//                 if (e.PathNodeManager != null)
//                     e.PathNodeManager.Update();

//                 if (e.PathVehicle != null)
//                     e.PathVehicle.Update(e, Time.deltaTime);
//             });

//             // TODO: NavigationSystem 종료?��?�� 개념?�� ?��?��?���????;;;
//             return false;
//         }
        
//         protected override void OnExit(IBattleSystemParam _param)
//         {
            
//         }

//     }
// }