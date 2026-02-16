dialogue = {}

function dialogue.PORTRAIT(name, portrait_asset, portrait_sprite)
   return {
      Name           = name,
      PortraitAsset  = portrait_asset,
      PortraitSprite = portrait_sprite
   }
end

function dialogue.DATA(is_active, position, portrait, dialogue)
   return {
      IsActive = is_active,
      Position = position,
      Portrait = portrait,
      Dialogue = dialogue
   }
end

function dialogue.SEQUENCE(dialogue_data)
   return {
      CloseDialogue = false,
      DialogueData  = dialogue_data
   }
end

function dialogue.SEQUENCE_END()
   return {
      CloseDialogue = true,
      DialogueData  = nil
   }
end