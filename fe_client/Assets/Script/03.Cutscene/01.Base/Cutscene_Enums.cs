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

    OnVerifyCondition  = 1, // 챕터 등장 조건 검사 시 실행할 스크립트. [kind]

    OnChapterStart     = 11, // 챕터 시작 시 실행할 스크립트. (시나리오 연출 등) []
    OnChapterEnd       = 12, // 챕터 종료 시 실행할 스크립트. (시나리오 연출 등) []

    OnMapSetting       = 21, // 맵 설정 시 실행할 스크립트.  (승패조건, 기본 태그 셋팅) []

    OnArrangementStart = 31, // 배치 시작 시 실행할 스크립트. []
    OnArrangementEnd   = 32, // 배치 종료 시 실행할 스크립트. []

    OnBattleStart      = 41, // 전투 시작 시 실행할 스크립트. (시나리오 연출 등) []
    OnBattleEnd        = 42, // 전투 종료 시 실행할 스크립트. (시나리오 연출 등) []

    OnTurnStart        = 51, // 턴 시작 시 실행할 스크립트. [turn_number]
    OnTurnEnd          = 52, // 턴 종료 시 실행할 스크립트. [turn_number]

    OnCombatStart      = 61, // 전투 시작 시 실행할 스크립트. [attacker, defender]
    OnCombatEnd        = 62, // 전투 종료 시 실행할 스크립트. [attacker, defender]


    OnTileSelect       = 71, // 타일 선택 시 실행할 스크립트. [x, y]

    OnCommandStart     = 81, // 명령 시작 시 실행할 스크립트. [id]
    OnCommandEnd       = 82, // 명령 종료 시 실행할 스크립트. [id]
    

    OnEveryUpdate      = 999, // 매 프레임 트리거
}
