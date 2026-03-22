item = {}



function item.ITEM_DATA(kind, value)
   return {
      Kind      = kind,
      Value     = value
   }
end

function item.ITEM_DATA(kind)
   return {
      Kind      = kind,
      Value     = 0
   }
end