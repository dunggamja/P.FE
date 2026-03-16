-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_04"
SCRIPT_MODULE_FUNC = "Run"

demo_script_04 = {}

-- lua의 주석
function demo_script_04.Run()     

   -- 5턴, 선박 진영 시작시 이벤트, 
   -- 유닛이동: (배 1개가 떠나서 사라짐)
   -- 대화:    (항구가 위험해... 빨리 좀 와달라고...)
   demo_script_04.Ship_Turn_5_Start();

end



   -- 5턴, 선박 진영 시작시 이벤트, 
   -- 유닛이동: (배 1개가 떠나서 사라짐)
   -- 대화:    (항구가 위험해... 빨리 좀 와달라고...)
function demo_script_04.Ship_Turn_5_Start()
end


-- 3턴, 마을 진영 시작시 이벤트, 
-- 커틀러스가 이 마을에 있다는 것을 알려주는 이벤트.
function demo_script_04.Villiage_1001_Turn_3_Start()
   local portrait_child   = dialogue.PORTRAIT("손녀", "", "");
   local portrait_grandpa = dialogue.PORTRAIT("노인", "", "");

   CutsceneBuilder.RootBegin("Villiage_1001_Turn_3_Start");
      -- 턴3_진영1 시작시 실행
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnTurnStart, 3, 1);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();

      -- 마을 위치로 카메라 포커싱
         CutsceneBuilder.VFX_TileSelect_On(0, tag.POSITION(4, 3));   
         CutsceneBuilder.Delay(1.0);
         CutsceneBuilder.VFX_TileSelect_Off(0);

      -- 대화.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.TOP_SHOW(portrait_child,
[[할아버지 그런 낡은 검을 들고 어딜 가는거야?]]),
               dialogue.BOTTOM_SHOW(portrait_grandpa,
[[이건 내가 젊었을 때 쓰던 
명검'커틀러스'다
이걸로 도적들을 혼내줄거다]]),               
               dialogue.TOP_SHOW(portrait_child,
[[그.. 그만둬
기분은 알겠지만 그런건 무리야!]]),
               dialogue.TOP_SHOW(portrait_child,
[[그러다가 할아버지에게 
무슨 일이 생기면 나는 어떡해!]]),
               dialogue.TOP_SHOW(portrait_child,
[[흑... 흑...]]),d
               dialogue.BOTTOM_SHOW(portrait_grandpa,
[[알았으니까... 그만 울거라...
나도 얌전히 마을에 있을 거니까...]]),
               dialogue.BOTTOM_SHOW(portrait_grandpa,
[[하지만 이 나라는 어떻게 되는걸까?]])
            }
         ));
         CutsceneBuilder.DialogueEnd();
      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

-- 마을 방문 및 커틀러스 획득
function demo_script_04.Villiage_1001_Visit()

   local portrait_grandpa = dialogue.PORTRAIT("노인", "", "");

   CutsceneBuilder.RootBegin("Villiage_1001_Visit");
      -- 턴3_진영1 시작시 실행
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnTurnStart, 3, 1);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();

      -- 대화.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.TOP_SHOW(portrait_grandpa,
[[산적들이 여기까지 쳐들어왔나.!
커틀러스로 해치워주마!]]),
               dialogue.TOP_SHOW(portrait_grandpa,
[[아니... 그대는 산적이 아니군...]]),               
               dialogue.TOP_SHOW(portrait_grandpa,
[[이봐 젊은이. 이것은 내가 젋을 때 애용하던
커틀러스다. 가벼워서 2번 공격할 수 있는 명검이지]]),
               dialogue.TOP_SHOW(portrait_grandpa,
[[너희들에게 이 검을 줄테니 나라를 부탁한다.
여기는 내가 태어나고 자란 조국이야. 
멸망하는 건 내가 허락치 않을거다]])               
         ));
         CutsceneBuilder.DialogueEnd();

         -- 커틀러스 획득 처리.


      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   


end



function demo_script_04.Villiage_1002_Visit()
end

function demo_script_04.Villiage_1003_Visit()
end

function demo_script_04.Villiage_1004_Visit()
end

function demo_script_04.Villiage_1005_Visit()
end

function demo_script_04.Villiage_1006_Visit()
end

function demo_script_04.Villiage_1007_Visit()
end

function demo_script_04.Villiage_1008_Visit()
end

function demo_script_04.Villiage_1009_Visit()
end

function demo_script_04.Villiage_1010_Visit()
end


function demo_script_04.Villiage_1011_Visit()
end

function demo_script_04.Villiage_1012_Visit()
end

function demo_script_04.Villiage_1013_Visit()
end

function demo_script_04.Villiage_1014_Visit()
end

function demo_script_04.Villiage_1015_Visit()
end

function demo_script_04.Villiage_1016_Visit()
end

function demo_script_04.Villiage_1017_Visit()
end

function demo_script_04.Villiage_1018_Visit()
end

function demo_script_04.Villiage_1019_Visit()
end

-- 꽃 방문. 약초 획득.
function demo_script_04.Villiage_1020_Visit()
end


-- 탈출.
function demo_script_04.Villiage_1021_Exit()
end
