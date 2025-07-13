using System;
using System.Collections.Generic;
using Battle;
using R3;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent)
    )]
public class GUIPage_Unit_Command_Attack : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }

        private PARAM(Int64 _entity_id) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/page/unit_command_attack", 

            // gui type
            EnumGUIType.Screen,

            // is input enabled
            true
            )             
        { 
            EntityID = _entity_id;  
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
    private BehaviorSubject<int>          m_selected_index_subject = new(0);





    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;

        UpdateMenuItems();
    }

    protected override void OnClose()
    {
        // throw new System.NotImplementedException();
    }



    protected override void OnPostProcess_Close()
    {
        // throw new System.NotImplementedException();
    }


    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case GUI_Menu_MoveEvent menu_move_event:
                OnReceiveEvent_GUI_Menu_MoveEvent(menu_move_event);
                break;
        }
    }

    void UpdateMenuItems()
    {
      var owner = EntityManager.Instance.GetEntity(m_entity_id);
      if (owner == null)
        return;

      // 소유중인 무기 목록
      {     
        var list_weapons = ListPool<Item>.Acquire();
        owner.Inventory.CollectItemByType(ref list_weapons, EnumItemType.Weapon);

        //
        m_menu_item_datas.Clear();
        for(int i = 0; i < list_weapons.Count; i++)
        {
          var item    = list_weapons[i];
          var item_id = item.ID;

          var menu_item_data = new MENU_ITEM_DATA(i, item_id);
          m_menu_item_datas.Add(menu_item_data);
        }

        ListPool<Item>.Return(list_weapons);      
      }


      // 메뉴 아이템 생성.      
      for (int i = 0, item_index = 0; i < m_menu_item_datas.Count; i++)
      {
          var item_id = m_menu_item_datas[i].ItemID;
          var item  = owner.Inventory.GetItem(item_id);
          if (item == null)
            continue;          

          var localize_key = item.GetLocalizeName();
          var text_subject = LocalizationManager.Instance.GetTextObservable(
            localize_key.Table, 
            localize_key.Key);

          // 
          //for (int k = 0; k < 10; k++)
          //{
            var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root.transform);
            clonedItem.Initialize(item_index++, m_selected_index_subject, text_subject);
          //}
       
        //   clonedItem.gameObject.SetActive(true);
      }

      // 초기 선택 인덱스 설정 (0번 인덱스 선택)
      m_selected_index_subject.OnNext(0);
    }


    // 메뉴 이동 이벤트 수신.
    void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // 이동 방향이 없으면 종료.
        if (_event.MoveDirection.y  == 0)
            return;

        
        // 이동 방향에 따라서 메뉴 아이템 선택.
        var add_index = _event.MoveDirection.y > 0 ? -1 : +1;
        var cur_index = m_selected_index_subject.Value;
        var new_index = cur_index + add_index;

        // 인덱스 범위 체크.
        new_index     = Math.Clamp(new_index, 0, m_menu_item_datas.Count - 1);
        
        // 인덱스 변경.
        m_selected_index_subject.OnNext(new_index);
    }
}
