item = {}



function item.ITEM_DATA(kind, value)
   return {
      Kind      = kind,
      Value     = value or 0 -- nil이면 0
   }
end
