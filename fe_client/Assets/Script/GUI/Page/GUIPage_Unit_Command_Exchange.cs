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
    typeof(GUI_Menu_CancelEvent),
    typeof(GUI_Menu_ForwardEvent)
    )]
public class GUIPage_Unit_Command_Exchange : GUIPage, IEventReceiver
{
   public enum EnumUIState
   {
      SelectTarget, // 교환할 타겟 선택
      ExchangeItem, // 교환할 아이템 선택
      
   }

   readonly (int ACTOR, int TARGET) TARGET_INDEX_RANGE = (0, 1); // 0 : actor, 1 : target


   public class PARAM : GUIOpenParam
   {
      public override EnumGUIType GUIType => EnumGUIType.Screen;

      public Int64                EntityID { get; private set; }
      // public Int64                TargetID { get; private set; }

      private PARAM(Int64 _entity_id) 
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
         // TargetID = _target_id;
      }

      static public PARAM Create(Int64 _entity_id)//, Int64 _target_id)
      {
         return new PARAM(_entity_id);//, _target_id);
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
        
      //   static public MENU_ITEM_DATA Empty => new MENU_ITEM_DATA(0);


        public void UpdateText()
        {
            if (TextSubject == null)
               return;

            if (ItemObject == null)
            {
               TextSubject.OnNext(string.Empty);
               return;
            }

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



   private Int64                   m_entity_id             = 0;
   private List<Int64>             m_exchange_target_list  = new();
   private int                     m_exchange_target_index = 0;
   private EnumUIState             m_ui_state              = EnumUIState.SelectTarget;

    private Int64                  m_vfx_exchange_target  = 0;


   private (int target, int index) m_cursor_index          = (-1, -1); // 0: actor, 1: target
   private (int target, int index) m_select_index          = (-1, -1); // 0: actor, 1: target


   private List<Int64>             m_previous_item_list_actor   = new();
   private List<Int64>             m_previous_item_list_target  = new();


   private List<MENU_ITEM_DATA>    m_menu_item_datas_actor      = new();
   private BehaviorSubject<int>    m_cursor_index_subject_actor = new(0);
   private BehaviorSubject<int>    m_select_index_subject_actor  = new(0);
   
   
   private List<MENU_ITEM_DATA>    m_menu_item_datas_target      = new();
   private BehaviorSubject<int>    m_cursor_index_subject_target = new(0);
   private BehaviorSubject<int>    m_select_index_subject_target = new(0);



   private Int64 ExchangeTargetID
   {
      get
      {
         if (m_exchange_target_index < 0 || m_exchange_target_index >= m_exchange_target_list.Count)
            return 0;

         return m_exchange_target_list[m_exchange_target_index];
      }
   }



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


   private int MaxCursorIndex_Actor  
   {
      get
      {
         var   max_index = m_menu_item_datas_actor.Count(e => e.ItemObject != null) - 1;


         // 아이템 선택중일때 예외처리.
         if (SelectIndex_Target >= 0)
             max_index += 1;


         return Math.Clamp(max_index, 0, Data_Const.UNIT_INVENTORY_MAX - 1);
      }
       
   }
   private int MaxCursorIndex_Target
   {
      get
      {
         var max_index = m_menu_item_datas_target.Count(e => e.ItemObject != null) - 1;

         // 아이템 선택중일때 예외처리.
         if (SelectIndex_Actor >= 0)
             max_index += 1;


         return Math.Clamp(max_index, 0, Data_Const.UNIT_INVENTORY_MAX - 1);
      }
   }

   private MENU_ITEM_DATA CursorItemData
   {
      get
      {
         if (CursorIndex_Actor >= 0)
            return m_menu_item_datas_actor[CursorIndex_Actor];

         if (CursorIndex_Target >= 0)
            return m_menu_item_datas_target[CursorIndex_Target];

         return null;
      }

      // set
      // {
      //    if (CursorIndex_Actor >= 0)
      //    {
      //       m_menu_item_datas_actor[CursorIndex_Actor] = value;
      //    }
      //    else if (CursorIndex_Target >= 0)
      //    {
      //       m_menu_item_datas_target[CursorIndex_Target] = value;
      //    }
      // }
   }

   private MENU_ITEM_DATA SelectItemData
   {
      get
      {
         if (SelectIndex_Actor >= 0)
            return m_menu_item_datas_actor[SelectIndex_Actor];

         if (SelectIndex_Target >= 0)
            return m_menu_item_datas_target[SelectIndex_Target];

         return null;
      }

      // set
      // {
      //    if (SelectIndex_Actor >= 0)
      //    {
      //       m_menu_item_datas_actor[SelectIndex_Actor] = value;
      //    }
      //    else if (SelectIndex_Target >= 0)
      //    {
      //       m_menu_item_datas_target[SelectIndex_Target] = value;
      //    }
      // }
   }





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
      // m_target_id = param?.TargetID ?? 0;  


      // 각자 인벤토리 아이템 8개까지 생성.
      for (int i = 0; i < Data_Const.UNIT_INVENTORY_MAX; i++)
      {
         m_menu_item_datas_actor.Add(new MENU_ITEM_DATA(i));
         m_menu_item_datas_target.Add(new MENU_ITEM_DATA(i));
      }

      // 교환 타겟 업데이트.
      CombatHelper.FindExchangeTargetList(m_entity_id, m_exchange_target_list);

      

      // 교환 아이템 UI 오브젝트 생성.
      CreateItemUIObject(m_menu_item_datas_actor, m_menu_item_datas_target);

      // 교환 아이템 데이터 셋팅.
      SetItemData();

      // 커서, 선택 UI 업데이트.
      UpdateUI_CursorAndSelect();
      UpdateUI_ItemData();

      // 교환 타겟 VFX 생성.
      CreateExchangeTargetVFX();
      UpdateExchangeTargetVFX();

      
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        UpdateDrawRange();
    }

    protected override void OnClose()
    {
        // 교환 타겟 VFX 해제.
        ReleaseExchangeTargetVFX();
    }

    protected override void OnPostProcess_Close()
    {
    }



    void UpdateDrawRange()
    {
        if (IsVisible == false)
            return;

        BattleSystemManager.Instance.DrawRange.DrawRange(
            _draw_flag:         (int)Battle.MoveRange.EnumDrawFlag.ExchangeRange,
            _entityID:          m_entity_id,
            _use_base_position: false);
    }

    void SetItemData()
    {
      var actor  = EntityManager.Instance.GetEntity(m_entity_id);
      var target = EntityManager.Instance.GetEntity(ExchangeTargetID);
      
      
      {
         using var list_items = ListPool<Item>.AcquireWrapper();

         if (actor != null)
             actor.Inventory.CollectItem(list_items.Value);

         for (int i = 0; i < m_menu_item_datas_actor.Count; ++i)
         {
            var item_object = i < list_items.Value.Count ? list_items.Value[i] : null;

            m_menu_item_datas_actor[i].SetItemObject(item_object);
         }

         m_previous_item_list_actor.Clear();
         foreach(var e in list_items.Value)
            m_previous_item_list_actor.Add(e.ID);
      }

      {
         using var list_items = ListPool<Item>.AcquireWrapper();

         if (target != null)
             target.Inventory.CollectItem(list_items.Value);

         for(int i = 0; i < m_menu_item_datas_target.Count; ++i)
         {
            var item_object = i < list_items.Value.Count ? list_items.Value[i] : null;

            m_menu_item_datas_target[i].SetItemObject(item_object);
         }

         m_previous_item_list_target.Clear();
         foreach(var e in list_items.Value)
            m_previous_item_list_target.Add(e.ID);
      }
    }

    void CompactItemDataList(List<MENU_ITEM_DATA> _menu_item_datas)
    {
      for (int i = 0; i < _menu_item_datas.Count; i++)
      {
         if (_menu_item_datas[i].ItemObject == null)
         {
               var next_index = _menu_item_datas.FindIndex(i + 1, e => e.ItemObject != null);
               if (next_index >= 0)
               {
                  _menu_item_datas[i].SetItemObject(_menu_item_datas[next_index].ItemObject);
                  _menu_item_datas[next_index].SetItemObject(null);
               }
               else
               {
               break;
               }
         }
      }
    }




    void CreateItemUIObject(List<MENU_ITEM_DATA> _menu_item_datas_actor, List<MENU_ITEM_DATA> _menu_item_datas_target)
    {
       if (m_grid_menu_item == null || m_grid_menu_root_actor == null || m_grid_menu_root_target == null)
          return;

      if (_menu_item_datas_actor == null || _menu_item_datas_target == null)
         return;

      // 기존 메뉴 아이템 삭제.
      m_grid_menu_root_actor.transform.DestroyAllChildren();
      m_grid_menu_root_target.transform.DestroyAllChildren();


       // 내 아이템 그리기.
       for(int i = 0; i < _menu_item_datas_actor.Count; i++)
       {
          var item_data    = _menu_item_datas_actor[i];
          var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root_actor.transform);          

          clonedItem.Initialize(i, 
               m_cursor_index_subject_actor, 
               item_data.TextSubject,
               m_select_index_subject_actor);
       }


       // 타겟 아이템 그리기.
       for(int i = 0; i < _menu_item_datas_target.Count; i++)
       {
          var item_data   = _menu_item_datas_target[i];
          var clonedItem  = Instantiate(m_grid_menu_item, m_grid_menu_root_target.transform);
   
          clonedItem.Initialize(i, 
               m_cursor_index_subject_target, 
               item_data.TextSubject,
               m_select_index_subject_target);
       }
    }


   void UpdateUI_CursorAndSelect()
   {
      m_cursor_index_subject_actor.OnNext(CursorIndex_Actor);
      m_cursor_index_subject_target.OnNext(CursorIndex_Target);
      m_select_index_subject_actor.OnNext(SelectIndex_Actor);
      m_select_index_subject_target.OnNext(SelectIndex_Target);
   }

   void UpdateUI_ItemData()
   {
      for(int i = 0; i < m_menu_item_datas_actor.Count; i++)
      {
         m_menu_item_datas_actor[i].UpdateText();
      }
      for(int i = 0; i < m_menu_item_datas_target.Count; i++)
      {
         m_menu_item_datas_target[i].UpdateText();
      }
   }

   void CreateExchangeTargetVFX()
   {
      var entity = EntityManager.Instance.GetEntity(m_entity_id);
      if (entity == null)
         return;

      // 커서 VFX 생성.
      var vfx_param = ObjectPool<VFXObject.Param>.Acquire()
         .SetVFXRoot_Default()
         .SetPosition(entity.Cell.CellToPosition())
         .SetVFXName(AssetName.TILE_SELECTION)
         .SetSnapToTerrain(true, Constants.BATTLE_VFX_SNAP_OFFSET_TILE);

      m_vfx_exchange_target = VFXManager.Instance.CreateVFXAsync(vfx_param);
   }

   void ReleaseExchangeTargetVFX()
   {
      VFXManager.Instance.ReserveReleaseVFX(m_vfx_exchange_target);
      m_vfx_exchange_target = 0;
   }

   void UpdateExchangeTargetVFX()
   {
      var entity = EntityManager.Instance.GetEntity(ExchangeTargetID);
      if (entity == null)
         return;

      EventDispatchManager.Instance.UpdateEvent(
         ObjectPool<VFX_TransformEvent>.Acquire()
         .SetID(m_vfx_exchange_target)
         .SetPosition(entity.Cell.CellToPosition())                
      ); 

      EventDispatchManager.Instance.UpdateEvent(
         ObjectPool<Battle_Cursor_PositionEvent>.Acquire()
         .Set(entity.Cell)
      ); 
   }

   private void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
   {
      if (_event == null || _event.GUI_ID != ID)
         return;

      switch (m_ui_state)
      {
         case EnumUIState.SelectTarget:
            OnReceiveEvent_GUI_Menu_SelectEvent_SelectTarget(_event);
            break;

         case EnumUIState.ExchangeItem:
            OnReceiveEvent_GUI_Menu_SelectEvent_ExchangeItem(_event);
            break;
      }
   }

   private void OnReceiveEvent_GUI_Menu_SelectEvent_SelectTarget(GUI_Menu_SelectEvent _event)
   {
      if (_event == null)
         return;

      m_ui_state     = EnumUIState.ExchangeItem;
      m_cursor_index = (TARGET_INDEX_RANGE.ACTOR, 0);
      m_select_index = (-1, -1);
      UpdateUI_CursorAndSelect();
   }

   private void OnReceiveEvent_GUI_Menu_SelectEvent_ExchangeItem(GUI_Menu_SelectEvent _event)
   {
      if (_event == null)
         return;


      var cursor_item_data = CursorItemData;
      var select_item_data = SelectItemData;


      var no_select = (select_item_data == null);
      if (no_select)
      {
         // 선택한 인덱스에 아이템이 있다면 선택 처리.
         if (cursor_item_data != null && cursor_item_data.ItemObject != null)
         {
            m_select_index = m_cursor_index;
            UpdateUI_CursorAndSelect();
         }
      }
      else
      {         
         if (m_select_index == m_cursor_index)
         {
            // 동일한 아이템을 선택했다면 선택 취소 처리.
            m_select_index = (-1, -1);
            UpdateUI_CursorAndSelect();
         }
         else
         {
            // 선택한 위치의 아이템과 교환.    
            var select_item_object = select_item_data.ItemObject;        
            var cursor_item_object = cursor_item_data.ItemObject;
            SelectItemData.SetItemObject(cursor_item_object);
            CursorItemData.SetItemObject(select_item_object);

            // 데이터 리스트 정렬.
            CompactItemDataList(m_menu_item_datas_actor);
            CompactItemDataList(m_menu_item_datas_target);

            // UI 업데이트.
            UpdateUI_ItemData();

            // 선택 종료 처리.
            m_select_index = (-1, -1);
            UpdateUI_CursorAndSelect();
         }

      }

      
      //UpdateUI_CursorAndSelect();
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

      if (m_exchange_target_list.Count == 0)
         return;


      var move_x       = Math.Clamp(_event.MoveDirection.x, -1, 1);
      var move_y       = Math.Clamp(_event.MoveDirection.y, -1, 1);

      // 타겟 이동 방향에 따라 인덱스 추가.
      int offset =
          (_event.MoveDirection.x != 0) 
        ? (_event.MoveDirection.x > 0 ? 1 : -1) 
        : (_event.MoveDirection.y != 0) 
        ? (_event.MoveDirection.y > 0 ? 1 : -1) 
        : 0;



      var prev_target_id       = ExchangeTargetID;

      m_exchange_target_index += offset;
      m_exchange_target_index += m_exchange_target_list.Count;
      m_exchange_target_index %= m_exchange_target_list.Count;

      var new_target_id        = ExchangeTargetID;

      // 타겟 변경시 처리.
      if (prev_target_id != new_target_id)
      {
         
         SetItemData();
         UpdateUI_ItemData();
         UpdateExchangeTargetVFX();
      }


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
           new_cursor_target = Math.Clamp(new_cursor_target, TARGET_INDEX_RANGE.ACTOR, TARGET_INDEX_RANGE.TARGET);
      }

      // 아이템 선택.
      new_cursor_index += (-move_y);


      // 커서 인덱스 클랩프.
      var max_cursor_index = (new_cursor_target == TARGET_INDEX_RANGE.ACTOR) ? MaxCursorIndex_Actor: MaxCursorIndex_Target;
      new_cursor_index     = Math.Clamp(new_cursor_index, 0, max_cursor_index);

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
                  // 선택한 인덱스가 있을 경우 초기화.
                  m_select_index = (-1, -1);                  
               }
               else
               {
                  // 교환 명령 플래그를 셋팅할 것인지 체크.
                  var is_flag_command_done = IsFlagCommandDone();
                  // 교환 명령 처리.
                  ProcessExchangeCommand(is_flag_command_done);

                  // 교환 명령 플래그에 따라 UI 처리.
                  if (is_flag_command_done)
                  {
                     // 교환 명령을 실행한 경우 UI 종료.                     
                     GUIManager.Instance.CloseUI(ID);
                  }
                  else
                  {
                     // 명령을 실행한 것이 아니면 타겟 선택 상태로 이동.
                     m_ui_state = EnumUIState.SelectTarget;
                     UpdateUI_CursorAndSelect();
                  }
               }            
           }
           break;
        }        
    }


    bool IsFlagCommandDone()
    {
        var entity_actor  = EntityManager.Instance.GetEntity(m_entity_id);
        var entity_target = EntityManager.Instance.GetEntity(ExchangeTargetID);
        if (entity_actor == null || entity_target == null)
           return false;


      using var list_actor_items_prev  = ListPool<Int64>.AcquireWrapper();
      using var list_target_items_prev = ListPool<Int64>.AcquireWrapper();

      // 기존 아이템 목록 추출.
      entity_actor.Inventory.ForEachItem(e => list_actor_items_prev.Value.Add(e.ID));
      entity_target.Inventory.ForEachItem(e => list_target_items_prev.Value.Add(e.ID));


      using var list_actor_items_after = ListPool<Int64>.AcquireWrapper();
      using var list_target_items_after = ListPool<Int64>.AcquireWrapper();

      // 변경된 아이템 목록 추출.
      m_menu_item_datas_actor.ForEach(e =>
      {
         if (e.ItemObject != null)
            list_actor_items_after.Value.Add(e.ItemObject.ID);
      });
      m_menu_item_datas_target.ForEach(e =>
      {
         if (e.ItemObject != null)
            list_target_items_after.Value.Add(e.ItemObject.ID);
      });

      // 비교를 위해서 정렬.
      list_actor_items_prev.Value.Sort();
      list_target_items_prev.Value.Sort();
      list_actor_items_after.Value.Sort();
      list_target_items_after.Value.Sort();

      // 변경 사항이 있는지 체크.
      var is_changed_actor  = list_actor_items_prev.Value.SequenceEqual(list_actor_items_after.Value) == false;
      var is_changed_target = list_target_items_prev.Value.SequenceEqual(list_target_items_after.Value) == false;

      return is_changed_actor || is_changed_target;

    }

    void ProcessExchangeCommand(bool _execute_command)
    {
       
      var entity = EntityManager.Instance.GetEntity(m_entity_id);
      if (entity == null)
         return;


      {
         using var list_actor_items  = ListPool<Item>.AcquireWrapper();
         using var list_target_items = ListPool<Item>.AcquireWrapper();

         foreach(var e in m_menu_item_datas_actor)
         {
            if (e.ItemObject != null)
               list_actor_items.Value.Add(e.ItemObject);
         }

         foreach(var e in m_menu_item_datas_target)
         {
            if (e.ItemObject != null)
               list_target_items.Value.Add(e.ItemObject);
         }

         // 이동 Command도 추가.
         
         {
            BattleSystemManager.Instance.PushCommand(
               new Command_Move(
                  m_entity_id,
                  entity.Cell,
                  _execute_command: _execute_command,
                  _visual_immediate: true,
                  _is_plan: _execute_command == false));
         }

         

         // 교환 Command 실행.
         BattleSystemManager.Instance.PushCommand(
               new Command_Exchange(
                  m_entity_id,
                  ExchangeTargetID,
                  list_actor_items.Value,
                  list_target_items.Value,
                  _execute_command
                  )
         );
      }      
    }


    

}
