-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_02"
SCRIPT_MODULE_FUNC = "Run"

demo_script_02 = {}

-- lua의 주석
function demo_script_02.Run()  

   local portrait_empty = dialogue.PORTRAIT("", "", "");



   CutsceneBuilder.RootBegin("Intro");
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnBattleStart, 0, 0);
      CutsceneBuilder.Condition_PlayOneShot();

      CutsceneBuilder.TrackBegin();
         -- 타일 선택 
         CutsceneBuilder.VFX_TileSelect(0, true, 3, 24);         

         -- 대화 연출
         local dialogue_intro = dialogue.DATA(true, DIALOGUE_DATA_EnumPosition.Center, portrait_empty, "서장");
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE({dialogue_intro}));
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE_END());

         -- 타일 선택 해제.
         CutsceneBuilder.VFX_TileSelect(0, false, 0, 0);  


      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();  
end