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
   public enum EnumUIState
   {
      SelectTarget, // 교환할 타겟 선택
      ExchangeItem, // 교환할 아이템 선택
      
   }

   readonly (int MIN, int MAX) TARGET_INDEX_RANGE = (0, 1);


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

   class MENU_ITEM_DATA
    {
        public int  Index                          { get; private set; }
        public Item ItemObject                     { get; private set; }
        public BehaviorSubject<string> TextSubject { get; private set; }

        

        public MENU_ITEM_DATA(int _index)
        {
            Index       = _index;
            ItemObject  = null;
            TextSubject = new BehaviorSubject<string>(string.Empty);
        }

        public void SetItemObject(Item _item_object)
        {
            ItemObject  = _item_object;
        }
        
        static public MENU_ITEM_DATA Empty => new MENU_ITEM_DATA(0);


        public void UpdateText()
        {
            if (TextSubject == null)
               return;

            var text_subject = TextSubject;
            Item.GetNameText(ItemObject, true).Subscribe(text => text_subject.OnNext(text));
        }
    }


   [SerializeField]
   private GUIElement_Grid_Item_MenuText m_grid_menu_item;

   [SerializeField]
   private GridLayoutGroup               m_grid_menu_root_actor;
   
   [SerializeField]
   private GridLayoutGroup               m_grid_menu_root_target;



   private Int64                   m_entity_id    = 0;
   private Int64                   m_target_id    = 0;
   private EnumUIState             m_ui_state     = EnumUIState.SelectTarget;

   private (int target, int index) m_cursor_index = (-1, -1); // 0: actor, 1: target
   private (int target, int index) m_select_index = (-1, -1); // 0: actor, 1: target

   private List<MENU_ITEM_DATA>    m_menu_item_datas_actor       = new();
   private BehaviorSubject<int>    m_cursor_index_subject_actor  = new(0);
   private BehaviorSubject<int>    m_select_index_subject_actor  = new(0);
   
   
   private List<MENU_ITEM_DATA>    m_menu_item_datas_target      = new();
   private BehaviorSubject<int>    m_cursor_index_subject_target = new(0);
   private BehaviorSubject<int>    m_select_index_subject_target = new(0);



   private int CursorIndex_Actor 
   {
      get
      {
         if (m_ui_state == EnumUIState.ExchangeItem)
            return (m_cursor_index.target == 0) ? m_cursor_index.index : -1;

         return -1;
      }
   }
   private int CursorIndex_Target
   {
      get
      {
         if (m_ui_state == EnumUIState.ExchangeItem)
            return (m_cursor_index.target == 1) ? m_cursor_index.index : -1;

         return -1;
      }
   }
   private int SelectIndex_Actor
   {
      get
      {
         if (m_ui_state == EnumUIState.ExchangeItem)
            return (m_select_index.target == 0) ? m_select_index.index : -1;

         return -1;
      }
   }
   private int SelectIndex_Target
   {
      get
      {
         if (m_ui_state == EnumUIState.ExchangeItem)
            return (m_select_index.target == 1) ? m_select_index.index : -1;

         return -1;
      }
   }


   private int ActorItemCount  => m_menu_item_datas_actor.Count(e => e.ItemObject != null);
   private int TargetItemCount => m_menu_item_datas_target.Count(e => e.ItemObject != null);





   // TODO:
   // 교환중에는 리스트랑 UI 만 서로 바꾸고...
   // -변경된 것이 있으면 교환 상태 처리.
   // -교환 하고 나면 이동은 더이상 불가능하게 처리.
   // -Command_Exchange를 만들긴 해야겠군.

   // TODO:
   // 1. 처음에는 주변 아군 유닛 선택
   // 2. 선택 후에는 아이템 선택. 

  


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

         case GUI_Menu_CancelEvent menu_cancel_event:
                OnReceiveEvent_GUI_Menu_CancelEvent(menu_cancel_event);
                break;
      }
   }



    protected override void OnOpen(GUIOpenParam _param)
    {
      var param   = _param as PARAM;
      m_entity_id = param?.EntityID ?? 0;
      m_target_id = param?.TargetID ?? 0;  


      for (int i = 0; i < Data_Const.UNIT_INVENTORY_MAX; i++)
      {
         m_menu_item_datas_actor.Add(new MENU_ITEM_DATA(i));
         m_menu_item_datas_target.Add(new MENU_ITEM_DATA(i));
      }

      var actor  = EntityManager.Instance.GetEntity(m_entity_id);
      var target = EntityManager.Instance.GetEntity(m_target_id);

      // 교환 아이템 데이터 업데이트
      UpdateItemData(actor, m_menu_item_datas_actor);
      UpdateItemData(target, m_menu_item_datas_target);

      // 교환 아이템 UI 오브젝트 생성.
      CreateItemUIObject();

      // 커서, 선택 UI 업데이트.
      UpdateUI_CursorAndSelect();
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

    void UpdateItemData(Entity _entity, List<MENU_ITEM_DATA> _menu_item_datas)
    {
      if (_entity == null || _menu_item_datas == null)
         return;
      
      using var list_items = ListPool<Item>.AcquireWrapper();
      _entity.Inventory.CollectItem(list_items.Value);

      for(int i = 0; i < _menu_item_datas.Count; ++i)
      {
         if (_menu_item_datas[i] == null)
            continue;

         var item_object = (i < list_items.Value.Count) ? list_items.Value[i] : null;

         _menu_item_datas[i].SetItemObject(item_object);
         _menu_item_datas[i].UpdateText();
      }
    }

    void CreateItemUIObject()
    {
       if (m_grid_menu_item == null || m_grid_menu_root_actor == null || m_grid_menu_root_target == null)
          return;

      // 기존 메뉴 아이템 삭제.
      m_grid_menu_root_actor.transform.DestroyAllChildren();
      m_grid_menu_root_target.transform.DestroyAllChildren();


       // 내 아이템 그리기.
       for(int i = 0; i < Data_Const.UNIT_INVENTORY_MAX; i++)
       {
          var item_data    = (i < m_menu_item_datas_actor.Count) ? m_menu_item_datas_actor[i] : MENU_ITEM_DATA.Empty;
          var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root_actor.transform);          

          clonedItem.Initialize(i, 
               m_cursor_index_subject_actor, 
               item_data.TextSubject,
               m_select_index_subject_actor);

          // 텍스트 갱신.
          item_data.UpdateText();
       }


       // 타겟 아이템 그리기.
       for(int i = 0; i <  Data_Const.UNIT_INVENTORY_MAX; i++)
       {
          var item_data   = (i < m_menu_item_datas_target.Count) ? m_menu_item_datas_target[i] : MENU_ITEM_DATA.Empty;
          var clonedItem  = Instantiate(m_grid_menu_item, m_grid_menu_root_target.transform);
   
          clonedItem.Initialize(i, 
               m_cursor_index_subject_target, 
               item_data.TextSubject,
               m_select_index_subject_target);

          // 텍스트 갱신.
          item_data.UpdateText();
       }
    }


   void UpdateUI_CursorAndSelect()
   {
      m_cursor_index_subject_actor.OnNext(CursorIndex_Actor);
      m_cursor_index_subject_target.OnNext(CursorIndex_Target);
      m_select_index_subject_actor.OnNext(SelectIndex_Actor);
      m_select_index_subject_target.OnNext(SelectIndex_Target);
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

      switch (m_ui_state)
      {
         // 타겟 선택 메뉴.
         case EnumUIState.SelectTarget:
            OnReceiveEvent_GUI_Menu_MoveEvent_SelectTarget(_event);
            break;

         // 아이템 교환 메뉴
         case EnumUIState.ExchangeItem:
            OnReceiveEvent_GUI_Menu_MoveEvent_ExchangeItem(_event);
            break;
      } 
   }

   private void OnReceiveEvent_GUI_Menu_MoveEvent_SelectTarget(GUI_Menu_MoveEvent _event)
   {
      if (_event == null)
         return;
   }

   private void OnReceiveEvent_GUI_Menu_MoveEvent_ExchangeItem(GUI_Menu_MoveEvent _event)
   {
      if (_event == null)
         return;


      // 메뉴 이동 방향에 따라 인덱스 추가.
      var move_x   = Math.Clamp(_event.MoveDirection.x, -1, 1);
      var move_y   = Math.Clamp(_event.MoveDirection.y, -1, 1);


      // 조작 전 커서 위치.
      var new_cursor_target = m_cursor_index.target;
      var new_cursor_index  = m_cursor_index.index;
      
      // 커서 위치 좌우 변경. left: actor, right: target
      if (move_x != 0) 
      {
           new_cursor_target += move_x;    
           new_cursor_target = Math.Clamp(new_cursor_target, TARGET_INDEX_RANGE.MIN, TARGET_INDEX_RANGE.MAX);
      }

      if (move_y != 0)
         new_cursor_index = Math.Clamp(new_cursor_index + move_y, 0, Data_Const.UNIT_INVENTORY_MAX - 1);


      m_cursor_index = (new_cursor_target, new_cursor_index);
            
      m_cursor_index_subject_actor.OnNext(CursorIndex_Actor);
      m_cursor_index_subject_target.OnNext(CursorIndex_Target); 
   }



   void OnReceiveEvent_GUI_Menu_CancelEvent(GUI_Menu_CancelEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        switch (m_ui_state)
        {
           case EnumUIState.SelectTarget:
               GUIManager.Instance.CloseUI(ID);
               break;
           case EnumUIState.ExchangeItem:
           {
               if (0 <= SelectIndex_Actor || 0 <= SelectIndex_Target)
               {
                  // 선택한 인덱스 초기화.
                  m_select_index = (-1, -1);                  
               }
               else
               {
                  // 타겟 선택 상태로 이동.
                  m_ui_state = EnumUIState.SelectTarget;
               }
            
           }
           break;
        }

        
    }
}
