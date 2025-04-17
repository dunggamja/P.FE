using System;
using UnityEngine;

public class GUIPage_Unit_Command : GUIPage
{
    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }

        private PARAM(Int64 _entity_id) 
        : base(
            Util.GenerateID(),
            "gui/page/unit_command",
            EnumGUIType.Screen)
        { 
            EntityID = _entity_id;  
        }


        static public PARAM Create(Int64 _entity_id)
        {
            return new PARAM(_entity_id);
        }
    }


    [SerializeField]
    private Transform                     m_grid_menu_root;

    [SerializeField]
    private GUIElement_Grid_Item_MenuText m_grid_menu_item;

}
