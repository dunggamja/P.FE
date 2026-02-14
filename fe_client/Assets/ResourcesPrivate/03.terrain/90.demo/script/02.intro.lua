-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_02"
SCRIPT_MODULE_FUNC = "Run"

demo_script_02 = {}

-- lua의 주석
function demo_script_02.Run()  

   local portrait_empty = dialogue.PORTRAIT("", "", "");



   CutsceneBuilder.RootBegin("Intro");
      CutsceneBuilder.TrackBegin();
         CutsceneBuilder.VFX_TileSelect(0, true, 3, 24);
         
         dialogue.DATA(true, DIALOGUE_DATA_EnumPosition.Center, portrait_empty, "서장");

      -- CutsceneBuilder.AddCutscene_Dialogue(new DIALOGUE_SEQUENCE(1, true, {
      --    new DIALOGUE_DATA(true, "Top", "John Doe", "Hello, World!"),
      --    new DIALOGUE_DATA(true, "Center", "Jane Doe", "Hello, World!2"),
      --    new DIALOGUE_DATA(true, "Bottom", "Doey Doe", "Hello, World!3"),
      -- }));
      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();  
end