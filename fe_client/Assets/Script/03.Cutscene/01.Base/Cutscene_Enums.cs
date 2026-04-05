using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;



// // TODO: 컷씬 타입이 뭔가 굳이 필요하지 않은 것 같기도?
// public enum EnumCutsceneType
// {
//     None      = 0,
//    //  Wait           = 1,   // 대기
//     Dialogue       = 10,  // 대화 연출
//     Unit_Move      = 20,  // 유닛 이동 연출. 
//     Unit_Active    = 21,  // 유닛 표시 On/Off.
//     VFX_TileSelect = 100,  // 타일 선택 커서 연출.
//     Trigger        = 1000, // 트리거 관련 동작.
// }

public enum EnumCutsceneLifeTime
{
    None    = 0,
    Chapter = 1,    
    Battle  = 2,
}


// 로컬 블랙보드는, Sequence 내에서만 사용되고 버려진다.
public enum EnumCutsceneLocalMemory
{

    // 컷씬에서 표시할 타일 선택 커서 VFX ID.
    VFX_Tile_Select_Begin = 100,
    VFX_Tile_Select_End   = 110, 

    // 트리거 범위
    Trigger_Begin = 100_000_000,
    Trigger_End   = 200_000_000,

}

// 글로벌 블랙보드는, Save/Load 대상에도 포함된다.
public enum EnumCutsceneGlobalMemory
{
   Trigger_Begin = 100_000_000,
   Trigger_End   = 200_000_000,
}




public enum EnumCutscenePlayEvent
{
    None = 0,


    // 챕터, 거점 등 관련 컷씬은 1~999번대로 지정.

    OnVerifyCondition  = 1, // 챕터 등장 조건 검사 시 실행할 스크립트. [kind]

    OnChapterStart     = 11, // 챕터 시작 시 실행할 스크립트. (시나리오 연출 등) []
    OnChapterEnd       = 12, // 챕터 종료 시 실행할 스크립트. (시나리오 연출 등) []



    // 전투 관련 컷씬은 1000번대로 지정.

    OnBattleMapSetting       = 1021, // 맵 설정 시 실행할 스크립트.  (승패조건, 기본 태그 셋팅) []

    OnBattleArrangementStart = 1031, // 배치 시작 시 실행할 스크립트. []
    OnBattleArrangementEnd   = 1032, // 배치 종료 시 실행할 스크립트. []

    OnBattleStart            = 1041, // 전투 시작 시 실행할 스크립트. (시나리오 연출 등) []
    OnBattleEnd              = 1042, // 전투 종료 시 실행할 스크립트. (시나리오 연출 등) []
      
    OnBattleVictory          = 1043, // 승리 확정 직후 연출용. CutsceneBuilder.PlayEvent 등록 []
    OnBattleDefeat           = 1044, // 패배 확정 직후 연출용. []
      
    OnTurnStart              = 1051, // 턴 시작 시 실행할 스크립트. [turn_number, faction_id]
    OnTurnEnd                = 1052, // 턴 종료 시 실행할 스크립트. [turn_number, faction_id]
    


    OnCombatStart            = 1061, // 전투 시작 시 실행할 스크립트. [attacker, defender]    
    OnCombatEnd              = 1062, // 전투 종료 시 실행할 스크립트. [attacker, defender]
    OnCombatDirectionStart   = 1063, // 전투 연출 시작 시 실행할 스크립트 [attacker, defender]
    OnCombatDirectionEnd     = 1064, // 전투 연출 종료 시 실행할 스크립트 [attacker, defender]


    OnTileSelect             = 1071, // 타일 선택 시 실행할 스크립트. [x, y]
      
    OnCommandStart           = 1081, // 명령 시작 시 실행할 스크립트. [id]
    OnCommandEnd             = 1082, // 명령 종료 시 실행할 스크립트. [id]
      
    OnEntityCreate           = 1091, // 엔티티 생성 시 실행할 스크립트. [entity_id]
    OnEntityDead             = 1092, // 엔티티 죽음 시 실행할 스크립트. [entity_id]

    OnMapObjectVisit         = 1201, // 맵 오브젝트 방문 시 실행할 스크립트. [map_object_id]
    OnMapObjectExit          = 1202, // 맵 오브젝트 이탈 시 실행할 스크립트. [map_object_id]            

    OnTalkCommand            = 1211, // 대화 명령 가능 시 실행할 스크립트. [entity_id]      


    OnCheckVictory           = 1301, // 승리 체크.
    OnCheckDefeat            = 1302, // 패배 체크.
    OnVictory                = 1303, // 승리 시 실행할 스크립트.
    OnDefeat                 = 1304, // 패배 시 실행할 스크립트.


    OnBattleUpdate           = 1999, // 매 프레임 트리거
}
