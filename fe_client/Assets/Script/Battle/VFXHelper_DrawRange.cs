// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Battle
// {
//   public class VFXHelper_DrawRange
//   {
//     public enum EnumType
//     {
//       None,

//       MoveRange,    // 이동 거리
//       WeaponRange,  // 무기 범위
//       ActionRange,  // 공격 범위
//     }

//     public EnumType    DrawType     { get; private set; } = EnumType.None;
//     public Int64       DrawEntityID { get; private set; } = 0;
//     public List<Int64> VFXList      { get; private set; } = new List<Int64>();

//     public void SetDraw(EnumType _type, Int64 _entityID)
//     {
//       DrawType     = _type;
//       DrawEntityID = _entityID;
//     }

//     public void Reset()
//     {
//       DrawType     = EnumType.None;
//       DrawEntityID = 0;
//     }


//     public void DrawVFX()
//     {

//     }

//     void ReleaseVFX()
//     {
//       foreach (var vfx in VFXList)
//       {
//         VFXManager.Instance.ReserveReleaseVFX(vfx);
//       }

//       VFXList.Clear();
//     }






//   }
// }