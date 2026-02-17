cutscene = {}

function cutscene.UNIT_MOVE_DATA(unit_id, start_pos_x, start_pos_y, end_pos_x, end_pos_y)
   return {
      UnitID        = unit_id,
      StartPosX     = start_pos_x,
      StartPosY     = start_pos_y,
      EndPosX       = end_pos_x,
      EndPosY       = end_pos_y
   }
end

function cutscene.UNIT_MOVE(unit_move_data, update_cell_position)
   return {
      UnitMoveData       = unit_move_data,
      UpdateCellPosition = update_cell_position
   }
end

function cutscene.LIFE_TIME(life_type, value, is_repeatable)
   return {
      LifeType       = life_type,
      Value          = value,
      IsRepeatable   = is_repeatable
   }
end

function cutscene.LIFE_TIME_CHAPTER(chapter_number, stage_number, is_repeatable)
   return cutscene.LIFE_TIME(
      EnumCutsceneLifeTime.Chapter, 
      chapter_number * Constants.STAGE_NUMBER_MAX + stage_number, 
      is_repeatable);
end

function cutscene.LIFE_TIME_BATTLE(is_repeatable)
   return cutscene.LIFE_TIME(EnumCutsceneLifeTime.Battle, 0, is_repeatable);
end