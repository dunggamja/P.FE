using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    // public enum EnumAIPriority
    // {
    //     Begin   = 0,
    //     Primary = Begin,    // 우선해서 처리할 행동.
    //     Secondary,  // 우선순위가 낮은 행동.
    //     Others,     // 나머지.

    //     Max,

    // }


    public enum EnumAIType
    {
        None,

        // # 일반 : 근처에 있는 적을 향해 이동해, 사정거리에 잡으면 공격해 온다. 

        // # 타겟이 있을 경우 어떻게 처리할 것인가?
        // 타겟 공격이 가능할 경우 -> 공격
        // 타겟 공격이 불가능할 경우 
        //  * 다른 적은 최대한 무시하는 경우.
        //    - 타겟과의 장애물이 없을 경우. : 타겟과의 가까워지는 것을 최우선
        //    - 타겟과의 장애물이 있을 경우. : 장애물 파괴/해결 우선. 
        //  * 다른 적도 공격하는 경우.
        //    - 공격 가능한 적이 있을 경우 : 공격 실행
        //    - 공격 가능한 적이 없을 경우 : 타겟과의 가까워지는 것을 최우선        
        Attack,


        // 사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 그 자리에서 대기. 
        Intercept,

        // 사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 거점으로 이동.
        Intercept_Base,


        // 사정거리 내에 공격 가능한 적이 있으면 공격,없는 경우에는 적의 사정거리에 있으면 사정거리 밖으로 도망친다, 안에 없으면 그 자리에서 대기.
        Alert,

        // 사정거리 내에 공격 가능한 적이 있으면 공격,없는 경우는 적의 사정거리에 있으면 사정거리 밖으로 도망치고, 
        // 안 들어오지 않으면 적의 공격 범위 아슬아슬하게 바깥까지 다가간다.
        Alert_Aggressive,

        // 적의 사정거리에 들어가면 사정거리 밖으로 도망치고, 들어가지 않으면 그 자리에서 대기. 공격해 오지 않는다.
        Alert_Evasive,

        // 랜덤으로 이동.
        Wandering,
        // 사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 랜덤으로 이동.
        Wandering_Aggressive,
        // // 
        // Wandering_Evasive,

        // 사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 일정한 루트를 이동.
        Patrol,

        // 그 자리를 움직이지 않는다.그 자리에서 공격 가능하면 공격한다.인접 등 공격 가능하지 않으면 도발은 할 수 없다.
        Fixed,

        // 특정 목표를 향해 이동. 공격해 오지 않는다. , 길을 찾을수 없다면 공격 해야할듯.
        Move,

        // 이탈 포인트로 향한다. 공격해 오지 않는다. , 길을 찾을수 없다면 공격해야 할듯.
        Leave,

        // 이탈 포인트로 향한다.이동처에서 공격 가능한 상대가 있으면 공격해 온다.
        Leave_Aggressive,
    }


    public enum EnumAIMoveType
    {

      Advance,    // 진군 - 타겟을 향해 이동.
      Intercept,  // 요격 - 거점에서 대기.

      Fixed,      // 그 자리를 움직이지 않는다
      Route,      // 일정한 루트를 이동.
      Random,     // 랜덤으로 이동.   
      Leave,      // 이탈 포인트로 이동.
    }

    public enum EnumAIAggressiveType
    {
      Aggressive, // 적을 공격 - 공격 O. 회피 X. 최대한 접근
      Alert,      // 적을 경계 - 공격 O. 회피 O. 공격 먼저 체크. 안 되면 회피. 회피시 거리 유지.
      Evassive,   // 적을 피함 - 공격 X. 회피 O. 최대한 도망.

      // // 밑의 것은 일단... 없앨까..;;;
      // Aiming,     // 대상을 노림. - 대상 외 공격 X. 최대한 접근.
      // Guard,      // 대상을 지킴. - 공격 O. 대상과 거리를 유지하는 선에서 적 공격.
    }
    
    
    // public enum EnumAITargetType
    // {
    //     None,
    //     Target,
    //     Position,
    // }
}