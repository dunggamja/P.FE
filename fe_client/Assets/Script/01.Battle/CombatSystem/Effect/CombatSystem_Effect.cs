// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Battle
// {
//     public partial class CombatSystem_Effect : CombatSystem
//     {
//         // public BHManager BHManager { get; private set; } = new();

//         private List<IBHEffect> m_effects = new();


//         public CombatSystem_Effect() : base(EnumSystem.CombatSystem_Effect)
//         {
//         }

//         public override void Init()
//         {
//             // base.Init();
//             // BHManager.Clear();
//         }

//         public override void Release()
//         {
//             // throw new NotImplementedException();
//             // BHManager.Clear();
//         }

//         protected override void OnEnter(ICombatSystemParam _param)
//         {
          
//         }

//         protected override bool OnUpdate(ICombatSystemParam _param)
//         {
//             return true;
//         }
        
//         protected override void OnExit(ICombatSystemParam _param)
//         {
//             // BHManager.Clear();
//             m_effects.Clear();
//         }


//         public void AddEffect(IBHEffect _effect)
//         {
//             m_effects.Add(_effect);
//         }

//     }
// }