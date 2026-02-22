-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_03"
SCRIPT_MODULE_FUNC = "Run"

demo_script_03 = {}

-- lua의 주석
function demo_script_03.Run()     
   -- 1턴, 적 진영 시작시 대화 이벤트, (건달들: '다 죽여버리자')
   demo_script_03.Enemy_Bully_Turn_1_Start();

   -- 건달 3021 전투중 죽음 이벤트.
   demo_script_03.Enemy_Bully_3021_Battle_End_Dead();

   -- 건달 3022 전투중 죽음 이벤트.
   demo_script_03.Enemy_Bully_3022_Battle_End_Dead();

   -- 2턴, 적 진영 시작시 도적의 대화 이벤트, (도적: '이 보석단검은 아무에게도 안 줘') (ENTITY.3011)
   demo_script_03.Enemy_Thief_Turn_2_Start();

   -- 해당 도적이 죽을때 대화 이벤트. (도적: '보석단검을 줄게 목숨만은 살려줘') (ENTITY.3011)
   demo_script_03.Enemy_Thief_Battle_End_Dead();

   -- 해적이 배를 때릴때 대사 연출. ('딱 좋은 사냥감이군') 
   demo_script_03.Enemy_Pirate_Battle_Start_Ship();

   -- 해적 (3031)은 적이 특정 거리 이내에 들어와야 공격을 시작한다. (ENTITY.3031)
   demo_script_03.Enemy_Pirate_Wait_Enemy();
end


-- 1턴, 적 진영 시작시 대화 이벤트, (건달들: '다 죽여버리자') (ENTITY.3021, ENTITY.3022) (Entity_Group: 1)
function demo_script_03.Enemy_Bully_Turn_1_Start()

   local portrait_3021 = dialogue.PORTRAIT("건달", "", "");
   local portrait_3022 = dialogue.PORTRAIT("건달", "", "");

   CutsceneBuilder.RootBegin("Enemy_Faction_Turn_1_Start");
      -- 턴1_진영2 시작시 실행
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnTurnStart, 1, 2);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();

      -- 건달들 위치로 카메라 포커싱.
         CutsceneBuilder.VFX_TileSelect_On(0, tag.TAG_INFO(EnumTagType.Entity_Group, 1));   
         CutsceneBuilder.Delay(1.0);
         CutsceneBuilder.VFX_TileSelect_Off(0);

      -- 건달들의 대화.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.TOP_SHOW(portrait_3021,
[[이봐 저 친구들은 뭐지?]]),
               dialogue.BOTTOM_SHOW(portrait_3022,
[[전쟁에 참패해서 도망치는 놈들이야]]),
               dialogue.BOTTOM_SHOW(portrait_3022,
[[지금까지 잘난체 했던 녀석들 꼴좋군.
원한을 갚을 때가 왔어.]]),
               dialogue.TOP_SHOW(portrait_3021,
[[이제보니 저 녀석들 성을 버리고
배타는 곳으로 가려는 것 같군]]),
               dialogue.TOP_SHOW(portrait_3021,
[[항구로 가는 길 근처에서
놈들을 잡을수 있을 거 같은데?]]),
               dialogue.BOTTOM_SHOW(portrait_3022,
[[후후 좋아. 건방진 놈들
다 죽여서 쓸어버리자고!]])
            }
         ));
         CutsceneBuilder.DialogueEnd();
      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();
end

-- 건달 3021 전투중 죽음 이벤트.
function demo_script_03.Enemy_Bully_3021_Battle_End_Dead()
end

-- 건달 3022 전투중 죽음 이벤트.
function demo_script_03.Enemy_Bully_3022_Battle_End_Dead()
end

-- 2턴, 적 진영 시작시 도적의 대화 이벤트, (도적: '이 보석단검은 아무에게도 안 줘') (ENTITY.3011)
function demo_script_03.Enemy_Thief_Turn_2_Start()
end

-- 해당 도적이 죽을때 대화 이벤트. (도적: '보석단검을 줄게 목숨만은 살려줘') (ENTITY.3011)
function demo_script_03.Enemy_Thief_Battle_End_Dead()
end

-- 해적이 배를 때릴때 대사 연출. ('딱 좋은 사냥감이군')
function demo_script_03.Enemy_Pirate_Battle_Start_Ship()
end

-- 해적 (3031)은 적이 특정 거리 이내에 들어와야 공격을 시작한다. (ENTITY.3031)
function demo_script_03.Enemy_Pirate_Wait_Enemy()
end


