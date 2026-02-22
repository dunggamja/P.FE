tag = {}

function tag.TAG_INFO(tag_type, tag_value)
   tag_type  = tag_type  or EnumTagType.None
   tag_value = tag_value or 0
   return {
      TagType  = tag_type,
      TagValue = tag_value
   }
end

function tag.TAG_DATA(tag_info, attribute, target_info)
   -- tag_info가 테이블이 아니면 TAG_INFO로 변환
   if type(tag_info) ~= "table" then
      tag_info = tag.TAG_INFO(EnumTagType.None, 0)
   end
   
   -- target_info가 테이블이 아니면 TAG_INFO로 변환
   if type(target_info) ~= "table" then
      target_info = tag.TAG_INFO(EnumTagType.None, 0)
   end
   
   attribute = attribute or EnumTagAttributeType.None
   
   return {
      TagInfo    = tag_info,
      Attribute  = attribute,
      TargetInfo = target_info
   }
end

function tag.POSITION(pos_x, pos_y)
   return tag.TAG_INFO(
      EnumTagType.Position,
      pos_x * Constants.MAX_MAP_SIZE + pos_y
   )
end