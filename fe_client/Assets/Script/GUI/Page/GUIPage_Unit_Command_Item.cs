using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Battle.MoveRange;
using R3;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent),
    typeof(GUI_Menu_SelectEvent),
    typeof(GUI_Menu_CancelEvent),
    typeof(Battle_Scene_ChangeEvent)
    )]
public class GUIPage_Unit_Command_Item : GUIPage, IEventReceiver
{
   public class PARAM : GUIOpenParam
    {
        public Int64    EntityID { get; private set; }
      //   public EnumUnitCommandType  MenuType  { get; private set; }
        public override EnumGUIType GUIType => EnumGUIType.Screen;

        private PARAM(Int64 _entity_id) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/page/unit_command_item", 

            // is input enabled
            true,

            // is multiple open
            false
            )             
        { 
            EntityID = _entity_id;  
            // MenuType = _menu_type;
        }


        static public PARAM Create(Int64 _entity_id)
        {
            return new PARAM(_entity_id);
        }
    }

    struct MENU_ITEM_DATA
    {
        public int          Index    { get; private set; }
        public Int64        ItemID   { get; private set; }

        public MENU_ITEM_DATA(int _index, Int64 _item_id)
        {
            Index    = _index;
            ItemID   = _item_id;
        }
        
        static public MENU_ITEM_DATA Empty => new MENU_ITEM_DATA(0, 0);
    }

   [SerializeField]
    private RectTransform                 m_grid_menu_root_bg_rect;

    [SerializeField]
    private RectTransform                 m_grid_menu_root_rect;

    [SerializeField]
    private GridLayoutGroup               m_grid_menu_root;

    [SerializeField]
    private GUIElement_Grid_Item_MenuText m_grid_menu_item;


    private Int64                         m_entity_id              = 0;    
    private List<MENU_ITEM_DATA>          m_menu_item_datas        = new();
    private BehaviorSubject<int>          m_cursor_index_subject   = new(0);

    private MENU_ITEM_DATA CursorItemData
    {
        get
        {
            var cur_index = m_cursor_index_subject.Value;
            if (cur_index < 0 || cur_index >= m_menu_item_datas.Count)
                return MENU_ITEM_DATA.Empty;

            return m_menu_item_datas[cur_index];
        }
    }




    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;

        // 아이템 목록 추출.
        UpdateItems();

    }

    protected override void OnClose()
    {
      //   throw new NotImplementedException();
    }



    protected override void OnPostProcess_Close()
    {
      //   throw new NotImplementedException();
    }


    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case GUI_Menu_MoveEvent menu_move_event:
                OnReceiveEvent_GUI_Menu_MoveEvent(menu_move_event);
                break;

            case GUI_Menu_SelectEvent menu_select_event:
                OnReceiveEvent_GUI_Menu_SelectEvent(menu_select_event);
                break;

            case Battle_Scene_ChangeEvent:
                GUIManager.Instance.CloseUI(ID);
                break;

            case GUI_Menu_CancelEvent menu_cancel_event:
                OnReceiveEvent_GUI_Menu_CancelEvent(menu_cancel_event);
                break;
        }
      
    }

    private void OnReceiveEvent_GUI_Menu_CancelEvent(GUI_Menu_CancelEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        GUIManager.Instance.CloseUI(ID);
    }

    private void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;


         var item_index = CursorItemData.Index;
         var item_id    = CursorItemData.ItemID;

         var entity = EntityManager.Instance.GetEntity(m_entity_id);
         if (entity == null)
            return;

         // var item_object = entity.Inventory.GetItem(item_id);
         // if (item_object == null)
         //    return;


         using var list_action_type = ListPool<EnumItemActionType>.AcquireWrapper();
         ItemHelper.Verify_Item_Action(item_id, entity, list_action_type.Value);


         var entity_id = entity.ID;

         GUIManager.Instance.OpenUI(GUIPage_Common_Selection.PARAM.Create(list_action_type.Value, (action_type) => 
         {
            var entity = EntityManager.Instance.GetEntity(entity_id);
            if (entity == null)
               return;

            var item_object = entity.Inventory.GetItem(item_id);
            if (item_object == null)
                return;

            // 일단 자신을 대상으로 사용하는 아이템에 대해서만 
            var target_type = ItemHelper.GetItemTargetType(item_object.Kind);

            // TODO: 소모품 아이템은 일단 자신에게 쓴느 아이템만 구현된 상태.
            if (item_object.ItemType == EnumItemType.Consumable && target_type != EnumTargetType.Owner)
                return;


           
            //if (entity.Cell != entity.PathBasePosition)
            {
                BattleSystemManager.Instance.PushCommand(
                new Command_Move(
                    m_entity_id,
                    entity.Cell,
                    _execute_command: true,
                    _visual_immediate: true,
                    _is_plan: false));
            }

            // 아이템 사용 Command 
            BattleSystemManager.Instance.PushCommand(
            new Command_Item(
                _owner_id:         m_entity_id,
                _target_id:        m_entity_id,
                _item_id:          item_id,
                _item_action_type: action_type)
            );


         }));
        
    }

    private void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
         if (_event == null || _event.GUI_ID != ID)
            return;

         // 메뉴 이동 방향이 없으면 종료.
         if (_event.MoveDirection.y  == 0)
            return;
        
         // 데이터 없음.
         if (m_menu_item_datas.Count == 0)
            return;

         var cur_index = m_cursor_index_subject.Value;
         var new_index = cur_index + (_event.MoveDirection.y < 0 ? 1 : -1);

         // 선택 인덱스 설정.
         m_cursor_index_subject.OnNext(new_index);

         ClampCursorIndex();
    }

    void ClampCursorIndex()
    {
        var cur_index = m_cursor_index_subject.Value;
        var new_index = Math.Clamp(cur_index, 0, Math.Max(0, m_menu_item_datas.Count - 1));
        if (new_index != cur_index)
            m_cursor_index_subject.OnNext(new_index);
    }


    void UpdateItems()
    {
        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity == null)
          return;

        // 소지한 아이템 목록 추출.
        using var list_items = ListPool<Item>.AcquireWrapper();
        entity.Inventory.CollectItem(list_items.Value);

        // 메뉴 아이템 목록 초기화.
        m_menu_item_datas.Clear();

        // 메뉴 아이템 목록 추가.
        for(int i = 0; i < list_items.Value.Count; i++)
        {
          var item = list_items.Value[i];
          m_menu_item_datas.Add(new MENU_ITEM_DATA(i, item.ID));
        }
        
        // 메뉴 아이템 그리기.
        m_grid_menu_root.transform.DestroyAllChildren();
        for(int i = 0, item_index = 0; i < m_menu_item_datas.Count; i++)
        {
          var item         = m_menu_item_datas[i];
          var item_object  = entity.Inventory.GetItem(item.ItemID);
          if (item_object == null)
            continue;


          var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root.transform);
          clonedItem.Initialize(item_index++, m_cursor_index_subject, Item.GetNameText(item_object, true));
        }


        m_cursor_index_subject.OnNext(0);
       
    }
}
