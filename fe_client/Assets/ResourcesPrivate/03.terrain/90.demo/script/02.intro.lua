-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_02"
SCRIPT_MODULE_FUNC = "Run"

demo_script_02 = {}

-- lua의 주석
function demo_script_02.Run()  

   local portrait_empty = dialogue.PORTRAIT("", "", "");
   local portrait_jade  = dialogue.PORTRAIT("Jade", "", "");
   local portrait_jax   = dialogue.PORTRAIT("Jax", "", "");
   local portrait_garan = dialogue.PORTRAIT("Garan", "", "");



   CutsceneBuilder.RootBegin("Intro");
      -- 전투 시작시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnBattleStart, 0, 0);
      -- 1번만 실행.
      CutsceneBuilder.Condition_PlayOneShot();
      
      -- 트랙 시작.
      CutsceneBuilder.TrackBegin();
         -- 대화 연출
         local dialogue_intro = dialogue.CENTER_SHOW(portrait_empty, "서장-demo test");
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE({dialogue_intro}));
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE_END());

         -- 시작 위치 표시 및 카메라 포커스 (index:0, 성 위치.)
         CutsceneBuilder.VFX_TileSelect_On(0, 3, 24);         
         CutsceneBuilder.Delay(1.0);

         -- 성 이라는 표시.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.CENTER_SHOW(portrait_empty, "여기가 왕국 성임.")
            }
         ));
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE_END());

         -- 떠나기 전에 대화.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.TOP_SHOW(portrait_jax, 
[[너는 도망가라]]),
               dialogue.TOP_SHOW(portrait_jax, 
[[항구에 배가 준비되있다
빨리가라]]),
               dialogue.BOTTOM_SHOW(portrait_jade, 
[[나는 도망갈거임]]),
               dialogue.BOTTOM_SHOW(portrait_jade, 
[[형만 혼자 남기고
나만 떠날 거임]]),
               dialogue.TOP_SHOW(portrait_jax, 
[[나도 따라갈거임
너 먼저 가서 기다리고 있으면 됨]]),


               dialogue.BOTTOM_SHOW(portrait_jade, 
[[안 가고 같이 싸울까 말까]]),
               dialogue.BOTTOM_SHOW(portrait_jade, 
[[음...]]),
               dialogue.CENTER_SHOW(portrait_garan, 
[[꾸물꾸물대지말고 빨리가자
배 떠나겠다.]]),
               
               dialogue.BOTTOM_HIDE(),
               dialogue.TOP_HIDE(),

               dialogue.CENTER_SHOW(portrait_garan, 
[[자 빨리 짐 싸고 가자
배타러 가자]]),
            }
         ));
         CutsceneBuilder.DialogueEnd();


         -- 타일 선택 해제. (index:0)
         CutsceneBuilder.VFX_TileSelect_Off(0);  


         -- 유닛들 이동 연출. 
         -- 유닛들 원래 좌표 받아옴.
         local pos_1_x, pos_1_y = EntityManager.GetPosition(1);
         local pos_2_x, pos_2_y = EntityManager.GetPosition(2);
         local pos_3_x, pos_3_y = EntityManager.GetPosition(3);
         local pos_4_x, pos_4_y = EntityManager.GetPosition(4);
         local pos_5_x, pos_5_y = EntityManager.GetPosition(5);
         
         CutsceneBuilder.Unit_Move(
            {cutscene.UNIT_MOVE_DATA(1, 3, 24, pos_1_x, pos_1_y),
             cutscene.UNIT_MOVE_DATA(2, 3, 24, pos_2_x, pos_2_y),
             cutscene.UNIT_MOVE_DATA(3, 3, 24, pos_3_x, pos_3_y),
             cutscene.UNIT_MOVE_DATA(4, 3, 24, pos_4_x, pos_4_y),
             cutscene.UNIT_MOVE_DATA(5, 3, 24, pos_5_x, pos_5_y)},
            false);

         -- 1초 대기.
         CutsceneBuilder.Delay(1.0);


         -- 탈출 위치 표시 및 카메라 포커스 (index:0, 성 위치.)
         CutsceneBuilder.VFX_TileSelect_On(0, 16, 2);         
         CutsceneBuilder.Delay(1.0);

         -- 여기가 탈출 위치임.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.CENTER_SHOW(portrait_empty, "여기가 탈출 위치임.")
            }
         ));
         CutsceneBuilder.DialogueEnd();

         -- 타일 선택 해제. (index:0)
         CutsceneBuilder.Delay(1.0);
         CutsceneBuilder.VFX_TileSelect_Off(0);  

         -- 시작 위치 표시 및 카메라 포커스 (index:0, 성 위치.)
         CutsceneBuilder.Grid_Cursor(3, 22);             


      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();  
end