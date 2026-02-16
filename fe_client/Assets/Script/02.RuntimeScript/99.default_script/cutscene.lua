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