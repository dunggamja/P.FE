dialogue = {}

function dialogue.PORTRAIT(name, portrait_asset, portrait_sprite)
   return {
      Name           = name,
      PortraitAsset  = portrait_asset,
      PortraitSprite = portrait_sprite
   }
end

function dialogue.DATA(is_active, position, portrait, dialogue_text)
   return {
      IsActive = is_active,
      Position = position,
      Portrait = portrait,
      Dialogue = dialogue_text
   }
end

function dialogue.TOP_SHOW(portrait, dialogue_text)
   return dialogue.DATA(true, DIALOGUE_DATA_EnumPosition.Top, portrait, dialogue_text)
end

function dialogue.CENTER_SHOW(portrait, dialogue_text)
   return dialogue.DATA(true, DIALOGUE_DATA_EnumPosition.Center, portrait, dialogue_text)
end

function dialogue.BOTTOM_SHOW(portrait, dialogue_text)
   return dialogue.DATA(true, DIALOGUE_DATA_EnumPosition.Bottom, portrait, dialogue_text)
end

function dialogue.TOP_HIDE()
   return dialogue.DATA(false, DIALOGUE_DATA_EnumPosition.Top, nil, "")
end

function dialogue.CENTER_HIDE()
   return dialogue.DATA(false, DIALOGUE_DATA_EnumPosition.Center, nil, "")
end

function dialogue.BOTTOM_HIDE()
   return dialogue.DATA(false, DIALOGUE_DATA_EnumPosition.Bottom, nil, "")
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