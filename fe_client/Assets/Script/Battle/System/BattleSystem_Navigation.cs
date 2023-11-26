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
//             // TODO: 일단 모든 유닛들의 이동 로직을 갱신한다....
//             BattleObjectManager.Instance.Loop((e) =>
//             {
//                 if (e == null)
//                     return;

//                 // TODO: 길찾기가 필요할 경우 여기서 셋팅한다.??

//                 if (e.PathNodeManager != null)
//                     e.PathNodeManager.Update();

//                 if (e.PathVehicle != null)
//                     e.PathVehicle.Update(e, Time.deltaTime);
//             });

//             // TODO: NavigationSystem 종료라는 개념이 필요할까???;;;
//             return false;
//         }
        
//         protected override void OnExit(IBattleSystemParam _param)
//         {
            
//         }

//     }
// }