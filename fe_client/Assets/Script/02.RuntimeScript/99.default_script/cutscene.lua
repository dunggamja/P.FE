cutscene = {}

function cutscene.DIALOGUE_PORTRAIT(name, portrait_asset, portrait_sprite)
   return {
      Name           = name,
      PortraitAsset  = portrait_asset,
      PortraitSprite = portrait_sprite
   }
end

function cutscene.DIALOGUE_DATA(is_active, position, portrait, dialogue)
   return {
      IsActive = is_active,
      Position = position,
      Portrait = portrait,
      Dialogue = dialogue
   }
end

function cutscene.DIALOGUE_SEQUENCE(id, close_dialogue, dialogue_data)
   return {
      ID            = id,
      CloseDialogue = close_dialogue,
      DialogueData  = dialogue_data
   }
end