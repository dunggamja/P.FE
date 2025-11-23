using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent),
    typeof(GUI_Menu_ForwardEvent)
    )]
public class GUIPage_Unit_Command_Exchange : GUIPage, IEventReceiver
{


   public class PARAM : GUIOpenParam
   {
      public override EnumGUIType GUIType => EnumGUIType.Screen;

      public Int64                EntityID { get; private set; }
      public Int64                TargetID { get; private set; }

      private PARAM(Int64 _entity_id, Int64 _target_id) 
      : base(
          // id      
          GUIPage.GenerateID(),           

          // asset path
          "gui/page/unit_command_exchange", 

          // is input enabled
          true,

          // is multiple open
          false
      )
      {
         EntityID = _entity_id;
         TargetID = _target_id;
      }

      static public PARAM Create(Int64 _entity_id, Int64 _target_id)
      {
         return new PARAM(_entity_id, _target_id);
      }
   }

   struct MENU_ITEM_DATA
    {
        public int  Index      { get; private set; }
        public Item ItemObject { get; private set; }

        public MENU_ITEM_DATA(int _index, Item _item_object)
        {
            Index      = _index;
            ItemObject = _item_object;
        }
        
        static public MENU_ITEM_DATA Empty => new MENU_ITEM_DATA(0, null);
    }


   [SerializeField]
   private GUIElement_Grid_Item_MenuText m_grid_menu_item;

   [SerializeField]
   private GridLayoutGroup               m_grid_menu_root_actor;
   
   [SerializeField]
   private GridLayoutGroup               m_grid_menu_root_target;



   private Int64 m_entity_id = 0;
   private Int64 m_target_id = 0;

   private List<MENU_ITEM_DATA> m_menu_item_datas_actor       = new();
   private BehaviorSubject<int> m_cursor_index_subject_actor  = new(0);
   private BehaviorSubject<int> m_select_index_subject_actor  = new(0);


   private List<MENU_ITEM_DATA> m_menu_item_datas_target      = new();
   private BehaviorSubject<int> m_cursor_index_subject_target = new(0);
   private BehaviorSubject<int> m_select_index_subject_target = new(0);


   private int ActorItemCount  => m_menu_item_datas_actor.Count(e => e.ItemObject != null);
   private int TargetItemCount => m_menu_item_datas_target.Count(e => e.ItemObject != null);



   // TODO:
   // 교환중에는 리스트랑 UI 만 서로 바꾸고...
   // -변경된 것이 있으면 교환 상태 처리.
   // -교환 하고 나면 이동은 더이상 불가능하게 처리.
   // -Command_Exchange를 만들긴 해야겠군.

  


   public void OnReceiveEvent(IEventParam _event)
   {
      switch (_event)
      {
         case Battle_Scene_ChangeEvent:
                GUIManager.Instance.CloseUI(ID);
                break;

         case GUI_Menu_MoveEvent menu_move_event:
                OnReceiveEvent_GUI_Menu_MoveEvent(menu_move_event);
                break;

         case GUI_Menu_SelectEvent menu_select_event:
                OnReceiveEvent_GUI_Menu_SelectEvent(menu_select_event);
                break;
      }
   }



    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;
        m_target_id = param?.TargetID ?? 0;  

        var actor  = EntityManager.Instance.GetEntity(m_entity_id);
        var target = EntityManager.Instance.GetEntity(m_target_id);

        UpdateMenuItemData(actor, m_menu_item_datas_actor);
        UpdateMenuItemData(target, m_menu_item_datas_target);
        CreateMenuItems();
    }

    protected override void OnClose()
    {
    }

    protected override void OnPostProcess_Close()
    {
    }


    protected override void OnInputFocusChanged(bool _focused)
    {
        base.OnInputFocusChanged(_focused);

        if (_focused)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    void UpdateMenuItemData(Entity _entity, List<MENU_ITEM_DATA> _menu_item_datas)
    {
      if (_entity == null || _menu_item_datas == null)
         return;

      
      using var list_items = ListPool<Item>.AcquireWrapper();
      _entity.Inventory.CollectItem(list_items.Value);

      _menu_item_datas.Clear();
      for(int i = 0; i < list_items.Value.Count; i++)
      {
        var item = list_items.Value[i];
        if (item == null)
            continue;

        var index   = _menu_item_datas.Count;
        _menu_item_datas.Add(new MENU_ITEM_DATA(index, item));
      }
    }

    void CreateMenuItems(int _cursor_actor = 0, int _cursor_target = -1, int _select_actor = -1, int _select_target = -1)
    {
       if (m_grid_menu_item == null || m_grid_menu_root_actor == null || m_grid_menu_root_target == null)
          return;

      // 기존 메뉴 아이템 삭제.
      m_grid_menu_root_actor.transform.DestroyAllChildren();
      m_grid_menu_root_target.transform.DestroyAllChildren();


       for(int i = 0; i < Data_Const.UNIT_INVENTORY_MAX; i++)
       {
          var item_data    = (i < m_menu_item_datas_actor.Count) ? m_menu_item_datas_actor[i] : MENU_ITEM_DATA.Empty;
          var item_object  = item_data.ItemObject;

          var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root_actor.transform);
          var text_subject = (item_object != null) 
                           ? item_object.GetNameText(item_object, true)
                           : Observable.Return(string.Empty);

          clonedItem.Initialize(i, 
               m_cursor_index_subject_actor, 
               text_subject,
               m_select_index_subject_actor);
       }



       for(int i = 0; i <  Data_Const.UNIT_INVENTORY_MAX; i++)
       {
          var item_data   = (i < m_menu_item_datas_target.Count) ? m_menu_item_datas_target[i] : MENU_ITEM_DATA.Empty;
          var item_object = item_data.ItemObject;

          var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root_target.transform);
          var text_subject = (item_object != null) 
                           ? item_object.GetNameText(item_object, true)
                           : Observable.Return(string.Empty);

          clonedItem.Initialize(i, 
            m_cursor_index_subject_target, 
            text_subject,
            m_select_index_subject_target);
       }


       m_cursor_index_subject_actor.OnNext(_cursor_actor);
       m_cursor_index_subject_target.OnNext(_cursor_target);
       m_select_index_subject_actor.OnNext(_select_actor);
       m_select_index_subject_target.OnNext(_select_target);
    }



   private void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
   {
      // throw new NotImplementedException();
   }

   private void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
   {
      // throw new NotImplementedException();
      if (_event == null || _event.GUI_ID != ID)
         return;

      // 메뉴 이동 방향이 없으면 종료.
      if (_event.MoveDirection.y  == 0 && _event.MoveDirection.x == 0)
         return;

      // 메뉴 이동 방향에 따라 인덱스 추가.
      var move_x   = Math.Clamp(_event.MoveDirection.x, -1, 1);
      var move_y   = Math.Clamp(_event.MoveDirection.y, -1, 1);


      // 조작 전 커서 위치.
      var cursor_is_actor = (m_cursor_index_subject_actor.Value >= 0) ;
      var cursor_index    = (cursor_is_actor) 
                          ? m_cursor_index_subject_actor.Value 
                          : m_cursor_index_subject_target.Value;


      var new_cursor_is_actor = cursor_is_actor;
      var new_cursor_index    = cursor_index;
      
      // 커서 위치 좌우 변경. 왼쪽이 Actor, 오른쪽이 Target.
      if (move_x != 0) 
         new_cursor_is_actor = (move_x < 0) ? true : false;     

      if (move_y != 0)
         new_cursor_index = Math.Clamp(new_cursor_index + move_y, 0, Data_Const.UNIT_INVENTORY_MAX - 1);
            

      var actor_index  = (new_cursor_is_actor == true)  ? new_cursor_index : -1;
      var target_index = (new_cursor_is_actor == false) ? new_cursor_index : -1;

      m_cursor_index_subject_actor.OnNext(actor_index);
      m_cursor_index_subject_target.OnNext(target_index);      
   }
}
