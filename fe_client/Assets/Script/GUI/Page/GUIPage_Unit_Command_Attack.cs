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
            EnumGUIType.Screen)             
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
        // throw new System.NotImplementedException();
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
        // throw new System.NotImplementedException();
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


      // // 메뉴 아이템 생성.
      // for (int i = 0; i < m_menu_item_datas.Count; i++)
      // {
      //     var (table, key) = m_menu_item_datas[i].GetLocalizeKey();
      //     var text_subject = LocalizationManager.Instance.GetTextObservable(table, key);

      //     // TODO: 오브젝트 풀링하는게 더 좋을까?
      //     var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root.transform);
          
      //     clonedItem.Initialize(i, m_selected_index_subject, text_subject);

      //     // clonedItem.SetText(m_menu_item_datas[i].GetMenuText());
      //     clonedItem.gameObject.SetActive(true);
      // }

    }

    void UpdateLayout()
    {
        // throw new System.NotImplementedException();



    }
}
