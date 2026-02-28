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



function demo_script_04.Ship_Turn_5_Start()
end