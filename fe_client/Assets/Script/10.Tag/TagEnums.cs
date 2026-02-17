// namespace Battle
// {
    public enum EnumTagType
    {
       None = 0, 

       Entity         = 1,    // 엔티티.
       Entity_Faction = 2,    // 진영.
       Entity_All     = 3,    // 구분 없이 모든 엔티티.   // All > Faction > Entity, 이것은 일일이 문서화하기 힘드므로 시스템으로 처리한다.

       Entity_Group   = 10,    // 따로 그룹 ID가 존재할 경우. // TODO: 맵셋팅. 시나리오를 통해 관리할것 같은데...

       Position       = 100, // 위치 - 점.
       Position_Rect  = 101, // 위치 - 사각형. 12자리 사용. (XXXYYYXXXYYY) (min,max)

       BattleMap      = 1000, // 배틀 맵.


      //  // AI 타입.
      //  AIType         = 200, // AI 타입.
      //  Trigger        = 10000, // 트리거.


    }

    public enum EnumTagAttributeType
    {
       None = 0,

       TARGET_FOCUS     = 1, // 타겟팅 집중.
       TARGET_IGNORE    = 2, // 타겟팅 제외.
       TARGET_CONTAIN   = 3, // 타겟팅 포함.


       POSITION_VISIT   = 11, // 위치 - 방문 가능       
       POSITION_EXIT    = 12, // 위치 - 이탈 

       TALK_COMMAND     = 21,  // 대화 명령 가능


       AI_TYPE          = 31, // AI 타입 설정

       ENTITY_HIERARCHY = 100, // 엔티티들 그룹핑을 위한 값.

       FromScenario     = 999,
    }
// }
