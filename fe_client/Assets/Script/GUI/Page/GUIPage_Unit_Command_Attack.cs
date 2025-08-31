using System;
using System.Collections.Generic;
using Battle;
using Battle.MoveRange;
using R3;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent),
    typeof(Battle_Scene_ChangeEvent)
    )]
public class GUIPage_Unit_Command_Attack : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }
        public override EnumGUIType GUIType => EnumGUIType.Screen;

        private PARAM(Int64 _entity_id) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/page/unit_command_attack", 

            // is input enabled
            true,

            // is multiple open
            false
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
    private BehaviorSubject<int>          m_selected_index_subject = new(0);

    private MENU_ITEM_DATA SelectedItemData
    {
        get
        {
            var cur_index = m_selected_index_subject.Value;
            if (cur_index < 0 || cur_index >= m_menu_item_datas.Count)
                return MENU_ITEM_DATA.Empty;

            return m_menu_item_datas[cur_index];
        }
    }





    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;

        UpdateMenuItems();
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        
        UpdateDrawRange();
    }

    protected override void OnClose()
    {
        // throw new System.NotImplementedException();
    }



    protected override void OnPostProcess_Close()
    {
        // throw new System.NotImplementedException();
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
        }
    }

    void UpdateMenuItems()
    {
      var owner = EntityManager.Instance.GetEntity(m_entity_id);
      if (owner == null)
        return;

      // �������� ���� ���
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

        ListPool<Item>.Return(ref list_weapons);      
      }


      // �޴� ������ ����.      
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

      // �ʱ� ���� �ε��� ���� (0�� �ε��� ����)
      m_selected_index_subject.OnNext(0);
    }


    // �޴� �̵� �̺�Ʈ ����.
    void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // �̵� ������ ������ ����.
        if (_event.MoveDirection.y  == 0)
            return;

        
        // �̵� ���⿡ ���� �޴� ������ ����.
        var add_index = _event.MoveDirection.y > 0 ? -1 : +1;
        var cur_index = m_selected_index_subject.Value;
        var new_index = cur_index + add_index;

        // �ε��� ���� üũ.
        new_index     = Math.Clamp(new_index, 0, m_menu_item_datas.Count - 1);
        
        // �ε��� ����.
        m_selected_index_subject.OnNext(new_index);
    }

    void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        var terrain_map = TerrainMapManager.Instance.TerrainMap;
        if (terrain_map == null)
            return;


        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity == null)
            return;


        var weapon_id = SelectedItemData.ItemID;


        // ���� ������ �ִ� Ÿ���� 1���� �����մϴ�.

        // ���� ���� ������ ����
        var attack_range_visit = ObjectPool<AttackRangeVisitor>.Acquire();
        attack_range_visit.SetData(
            _draw_flag:         (int)Battle.MoveRange.EnumDrawFlag.AttackRange,
            _terrain:           TerrainMapManager.Instance.TerrainMap,
            _entity_object:     EntityManager.Instance.GetEntity(m_entity_id),
            _use_base_position: false,
            _use_weapon_id:     weapon_id
        );

        // ���� ���� ��� ã��
        PathAlgorithm.FloodFill(attack_range_visit);

        // ó�� �ɸ��� ����� ����.
        Int64 target_entity_id = 0;
        foreach (var pos in attack_range_visit.List_Weapon)
        {
            var target_id = terrain_map.EntityManager.GetCellData(pos.x, pos.y);

            if (CombatHelper.IsAttackable(m_entity_id, target_id))
            {
                target_entity_id = target_id;
                break;
            }
        }

        ObjectPool<AttackRangeVisitor>.Return(ref attack_range_visit);


        // ���� ���� UI�� ���ϴ�.
        if (target_entity_id > 0)
        {      
            // Debug.Log($"target_entity_id: {target_entity_id}");

            // ���� ���� UI ����    
            GUIManager.Instance.OpenUI(
                GUIPage_Unit_Command_Attack_Preview
                .PARAM
                .Create(m_entity_id, target_entity_id, weapon_id)
                );
        }
    }

    void UpdateDrawRange()
    {
        if (IsInputFocused == false)
            return;

        var select_item_id = SelectedItemData.ItemID;

        // ���� ������ �׷��ݴϴ�.
        BattleSystemManager.Instance.DrawRange.DrawRange
        (
            (int)Battle.MoveRange.EnumDrawFlag.AttackRange,
            _entityID:          m_entity_id,
            _use_base_position: false,
            _use_weapon_id:     select_item_id
        );
    }
}
