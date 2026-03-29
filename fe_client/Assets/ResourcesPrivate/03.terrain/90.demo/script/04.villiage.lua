-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_04"
SCRIPT_MODULE_FUNC = "Run"

demo_script_04 = {}

-- lua의 주석
function demo_script_04.Run()     

   -- 5턴, 선박 진영 시작시 이벤트, 
   -- 유닛이동: (배 1개가 떠나서 사라짐)
   -- 대화:    (항구가 위험해... 빨리 좀 와달라고...)
   demo_script_04.Ship_4001_Turn_5_Start();

   
   -- 탈출 이벤트 등록
   demo_script_04.Villiage_1021_Exit();

   -- 3턴, 마을 진영 시작시 이벤트,
   -- 커틀러스가 이 마을에 있다는 것을 알려주는 이벤트.
   demo_script_04.Villiage_1001_Turn_3_Start();


   -- 마을 방문 및 커틀러스 획득
   demo_script_04.Villiage_1001_Visit();

   -- 마을방문 이벤트 등록
   demo_script_04.Villiage_1001_Visit();
   demo_script_04.Villiage_1002_Visit();
   demo_script_04.Villiage_1003_Visit();
   demo_script_04.Villiage_1004_Visit();
   demo_script_04.Villiage_1005_Visit();
   demo_script_04.Villiage_1006_Visit();
   demo_script_04.Villiage_1007_Visit();
   demo_script_04.Villiage_1008_Visit();
   demo_script_04.Villiage_1009_Visit();
   demo_script_04.Villiage_1010_Visit();
   demo_script_04.Villiage_1011_Visit();
   demo_script_04.Villiage_1012_Visit();
   demo_script_04.Villiage_1013_Visit();
   demo_script_04.Villiage_1014_Visit();
   demo_script_04.Villiage_1015_Visit();
   demo_script_04.Villiage_1016_Visit();
   demo_script_04.Villiage_1017_Visit();
   demo_script_04.Villiage_1018_Visit();
   demo_script_04.Villiage_1019_Visit();
   demo_script_04.Villiage_1020_Visit();

   -- 대화이벤트 등록해보자.
   demo_script_04.Entity_1_2_Talk();

end



   -- 5턴, 선박 진영 시작시 이벤트, 
   -- 유닛이동: (배 1개가 떠나서 사라짐)
   -- 대화:    (항구가 위험해... 빨리 좀 와달라고...)
function demo_script_04.Ship_4001_Turn_5_Start()

   local portrait_4001 = dialogue.PORTRAIT("선박1", "", "");

   CutsceneBuilder.RootBegin("Ship_4001_Turn_5_Start");
      -- 턴5_진영0 시작시 실행
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnTurnStart, 5, 0);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();

      -- 선박 위치로 카메라 포커싱.
         CutsceneBuilder.VFX_TileSelect_On(0, tag.TAG_INFO(EnumTagType.Entity, 4001));   
         CutsceneBuilder.Delay(1.0);
         CutsceneBuilder.VFX_TileSelect_Off(0);

      -- 선박의 대화.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.TOP_SHOW(portrait_4001,
[[항구가 위험해... 빨리 좀 와달라고...]])
            }
         ));
         CutsceneBuilder.DialogueEnd();
      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();

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
[[흑... 흑...]]),
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
      -- 1001번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1001);

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
[[이봐 젊은이. 이것은 내가 젊을 때 애용하던
커틀러스다. 가벼워서 2번 공격할 수 있는 명검이지]]),
               dialogue.TOP_SHOW(portrait_grandpa,
[[너희들에게 이 검을 줄테니 나라를 부탁한다.
여기는 내가 태어나고 자란 조국이야. 
멸망하는 건 내가 허락치 않을거다]])  
            }             
         ));
         CutsceneBuilder.DialogueEnd();

         -- 커틀러스 획득 처리. 
         --  맵오브젝트 위치의 유닛에게. (EnumTagType.MapObject, 1001)
         --  커틀러스 지급 (item.ITEM_DATA(1005)
         CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1001), {item.ITEM_DATA(1005)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   


end



function demo_script_04.Villiage_1002_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1002_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1002);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();


      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1002), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1003_Visit()

   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1003_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1003);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();

      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1003), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1004_Visit()

   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1004_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1004);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1004), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1005_Visit()

   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1005_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1005);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1005), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();    
end

function demo_script_04.Villiage_1006_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1006_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1006);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1006), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1007_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1007_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1007);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1007), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1008_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1008_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1008);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();       
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1008), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1009_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1009_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1009);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1009), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1010_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1010_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1010);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1010), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end


function demo_script_04.Villiage_1011_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1011_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1011);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1011), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1012_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1012_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1012);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1012), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1013_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1013_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1013);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1013), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1014_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1014_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1014);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();    
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1014), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1015_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1015_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1015);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1015), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1016_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1016_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1016);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1016), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1017_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1017_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1017);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1017), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1018_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1018_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1018);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1018), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

function demo_script_04.Villiage_1019_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1019_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1019);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1019), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end

-- 꽃 방문. 약초 획득.
function demo_script_04.Villiage_1020_Visit()
   local portrait_villiage = dialogue.PORTRAIT("마을", "", "");

   CutsceneBuilder.RootBegin("Villiage_1020_Visit");
      -- 1002번 맵오브젝트 방문 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnMapObjectVisit, 1020);

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();    
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_villiage,
[[돈 받고 꺼져]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();

      -- 골드 지급 (1000골드)
      CutsceneBuilder.ItemAcquire(tag.TAG_INFO(EnumTagType.MapObject, 1020), {item.ITEM_DATA(1, 1000)});

      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();   
end


-- 탈출.
function demo_script_04.Villiage_1021_Exit()
end


-- 대화이벤트. (제이드_아톨)
function demo_script_04.Entity_1_2_Talk()

   local portrait_jade   = dialogue.PORTRAIT("제이드", "", "");
   local portrait_atol   = dialogue.PORTRAIT("아톨", "", "");

   CutsceneBuilder.RootBegin("Entity_1_2_Talk");
      -- 대화이벤트 등록. 
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnTalkCommand, 2);

      -- 명령중인 유닛 조건 추가.
      CutsceneBuilder.Condition_CommandEntity(tag.TAG_INFO(EnumTagType.Entity, 1));

      -- 전투내에서만 사용되는 컷씬
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      CutsceneBuilder.TrackBegin();
      -- 대화.
      CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
         {
            dialogue.TOP_SHOW(portrait_jade,[[넌 뭐하는 놈이니?]]),
            dialogue.BOTTOM_SHOW(portrait_atol,[[난 아톨이야. 너는 뭔데?]]),
            dialogue.TOP_SHOW(portrait_jade,[[난 제이드야.]])
         }             
      ));
      CutsceneBuilder.DialogueEnd();
   CutsceneBuilder.RootEnd();

end
