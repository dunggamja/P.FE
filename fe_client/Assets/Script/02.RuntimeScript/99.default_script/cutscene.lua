cutscene = {}

function cutscene.DIALOGUE(dialogue_data)
   return {
      Type = EnumCutsceneType.Dialogue,
      DialogueData = dialogue_data
   }
end