using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  public enum EnumSpacePartitionType
  {
    None,

    Threat, // 이동거리 + 공격범위.
    
  }


  public class SpacePartitionManager : Singleton<SpacePartitionManager>
  {
    // public DynamicAABBTree AABBTree { get; private set; } = new();
  }
}