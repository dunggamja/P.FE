using System;
using System.Collections.Generic;
using Battle;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent),
    typeof(GUI_Menu_ForwardEvent),
    typeof(Battle_Scene_ChangeEvent)
    )]
public class GUIPage_Unit_Command_Attack_Preview : GUIPage, IEventReceiver
{

    public class PARAM : GUIOpenParam
    {
        public override EnumGUIType GUIType => EnumGUIType.Screen;

        public Int64 EntityID { get; private set; }
        public Int64 TargetID { get; private set; }
        public Int64 WeaponID { get; private set; }

        private PARAM(Int64 _entity_id, Int64 _target_id, Int64 _weapon_id) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/page/unit_command_attack_preview", 

            // is input enabled
            true
            )               
        {
            EntityID = _entity_id;
            TargetID = _target_id;
            WeaponID = _weapon_id;
        }

        static public PARAM Create(Int64 _entity_id, Int64 _target_id, Int64 _weapon_id)
        {
            return new PARAM(_entity_id, _target_id, _weapon_id);
        }
    }
    

    [SerializeField]
    private GUIElement_Attack_Preview_Unit          m_preview_attacker;
    [SerializeField]         
    private GUIElement_Attack_Preview_Unit          m_preview_defender;
    [SerializeField]
    private GridLayoutGroup                         m_grid_attack_root;
    [SerializeField]
    private GUIElement_Attack_Preview_Sequence_Grid m_grid_attack_sequence;







    private Int64       m_entity_id   = 0;     
    private Int64       m_target_id   = 0;
    private Int64       m_weapon_id   = 0;
    private Int64       m_vfx_cursor  = 0;

    private List<Int64> m_target_list = new();

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

            case GUI_Menu_ForwardEvent menu_forward_event:
                OnReceiveEvent_GUI_Menu_ForwardEvent(menu_forward_event);
                break;

            case Battle_Scene_ChangeEvent:
                GUIManager.Instance.CloseUI(ID);
                break;
        }
    }

    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;
        m_target_id = param?.TargetID ?? 0;
        m_weapon_id = param?.WeaponID ?? 0;


        CreateCursorVFX();

        // Ÿ�� ����.
        m_target_id = FindTarget(m_weapon_id, ref m_target_list);

        // ������ UI ����
        UpdateUI_Preview();

        // CombatSystemManager.Instance.Setup()

    }

    protected override void OnLoop()
    {
        base.OnLoop();


        // ���ݹ��� ǥ��.
        UpdateDrawRange();
    }

    protected override void OnClose()
    {
        // throw new NotImplementedException();
        
        ReleaseCursorVFX();
    }

    protected override void OnPostProcess_Close()
    {
        // throw new NotImplementedException();
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



    Int64 FindTarget(Int64 _weapon_id, ref List<Int64> _target_list)
    {
        _target_list.Clear();

        // ������ üũ.
        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity == null)
            return 0;

        // ���� ���� Ž��
        var attack_range_visit = ObjectPool<Battle.MoveRange.AttackRangeVisitor>.Acquire();
        attack_range_visit.SetData(
            _draw_flag:         (int)Battle.MoveRange.EnumDrawFlag.AttackRange,
            _terrain:           TerrainMapManager.Instance.TerrainMap,
            _entity_object:     EntityManager.Instance.GetEntity(m_entity_id),
            _use_base_position: false,
            _use_weapon_id:     _weapon_id
        );

        PathAlgorithm.FloodFill(attack_range_visit);

        // ���� ������ Ÿ�� ���.
        foreach(var pos in attack_range_visit.List_Weapon)
        {
            var target_id = TerrainMapManager.Instance.TerrainMap.EntityManager.GetCellData(pos.x, pos.y);
            if (target_id > 0)
            {   
                // ���� �������� üũ.
                if (CombatHelper.IsAttackable(m_entity_id, target_id) == false)
                    continue;             

                _target_list.Add(target_id);
            }
        }

        ObjectPool<Battle.MoveRange.AttackRangeVisitor>.Return(ref attack_range_visit);

        // ���� Ÿ���� ���� �����ϸ� ����, ���� �Ұ����ϸ� �ٸ� Ÿ������ ����.
        Int64 new_target_id = m_target_id;        
        if (_target_list.Contains(m_target_id) == false)
        {
            new_target_id = _target_list.Count > 0 ? _target_list[0] : 0;
        }

        return new_target_id;
    }


    void UpdateUI_Preview()
    {
        var result = CombatHelper.Run_Plan(
            m_entity_id, 
            m_target_id, 
            m_weapon_id);


        if (result == null)
        {
            Debug.LogError("CombatHelper.Run_Plan() is null");
            return;
        }

        // Ÿ�� ��ġ�� Ŀ�� �ű�.
        var entity_target = EntityManager.Instance.GetEntity(m_target_id);
        if (entity_target != null)
        {
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<VFX_TransformEvent>.Acquire()
                .SetID(m_vfx_cursor)
                .SetPosition(entity_target.Cell.CellToPosition())                
            ); 
        }

        // ������ ǥ��.
        m_preview_attacker.Initialize(
            result.Attacker.EntityID,
            result.Attacker.WeaponID,
            result.Attacker.Damage,
            result.Attacker.CriticalRate,
            result.Attacker.HitRate,
            result.Attacker.HP_Before,
            result.Attacker.HP_After);

        // ������ ǥ��.
        m_preview_defender.Initialize(
            result.Defender.EntityID,
            result.Defender.WeaponID,
            result.Defender.Damage,
            result.Defender.CriticalRate,
            result.Defender.HitRate,
            result.Defender.HP_Before,
            result.Defender.HP_After);

        // �׸��� ������ ����.
        {           
            var list_delete = ListPool<Transform>.Acquire();
            for (int i = 0; i < m_grid_attack_root.transform.childCount; i++)
                list_delete.Add(m_grid_attack_root.transform.GetChild(i)); 
            list_delete.ForEach(e => { if (e != null) GameObject.Destroy(e.gameObject);});
            ListPool<Transform>.Return(ref list_delete);
        }

        // �׸��� ������ ����
        for (int i = 0; i < result.Actions.Count; i++)
        {
            var clonedItem = Instantiate(m_grid_attack_sequence, m_grid_attack_root.transform);
            clonedItem.Initialize(
                result.Actions[i].isAttacker, 
                result.Actions[i].Damage);
        }
    }

     void UpdateDrawRange()
    {
        if (IsInputFocused == false)
            return;

        // ���� ������ �׷��ݴϴ�.
        BattleSystemManager.Instance.DrawRange.DrawRange
        (
            (int)Battle.MoveRange.EnumDrawFlag.AttackRange,
            _entityID:          m_entity_id,
            _use_base_position: false,
            _use_weapon_id:     m_weapon_id
        );
    }




    void CreateCursorVFX()
    {
        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity == null)
            return;

        // ����Ʈ ����.
        var vfx_param = ObjectPool<VFXObject.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetPosition(entity.Cell.CellToPosition())
            .SetVFXName(AssetName.TILE_SELECTION);

        m_vfx_cursor = VFXManager.Instance.CreateVFXAsync(vfx_param);
    }

    void ReleaseCursorVFX()
    {
        VFXManager.Instance.ReserveReleaseVFX(m_vfx_cursor);
        m_vfx_cursor = 0;
    }



    void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // �̵� ������ ������ ����.
        if (_event.MoveDirection == Vector2Int.zero)
            return;

        if (m_target_list.Count == 0)
            return;

        // ����Ű�� ���� ���� Ÿ���� �ٲ߽ô�.
        int offset =
          (_event.MoveDirection.x != 0) 
        ? (_event.MoveDirection.x > 0 ? 1 : -1) 
        : (_event.MoveDirection.y != 0) 
        ? (_event.MoveDirection.y > 0 ? 1 : -1) 
        : 0;


        // Ÿ�� ����.
        var index    = m_target_list.IndexOf(m_target_id);
        index       += (offset + m_target_list.Count);
        index       %= m_target_list.Count;
        m_target_id  = m_target_list[index];

        UpdateUI_Preview();
    }

    void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;


        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity == null)
            return;


        // Command_Attack
        // ���� ����: ���� ���
         BattleSystemManager.Instance.PushCommand(
                    new Command_Attack
                    (
                        m_entity_id,
                        m_target_id,
                        m_weapon_id,
                        entity.Cell                        
                    ));


        // ���� UI�� ��� �ݾƾ���...
        //GUIManager.Instance.CloseUI(ID);
    }

    void OnReceiveEvent_GUI_Menu_ForwardEvent(GUI_Menu_ForwardEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // ���� ���� �̺�Ʈ.
        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity == null)
            return;



        // ���� ����.
        {
            var   list_weapon   = ListPool<Item>.Acquire();
            var   list_target   = ListPool<Int64>.Acquire();

            Int64 new_weapon_id = 0;
            Int64 new_target_id = 0;

            // ���� ���� ���� ��� ��ȸ.
            entity.Inventory.CollectItemByType(ref list_weapon, EnumItemType.Weapon);
            if (1 < list_weapon.Count)
            {            
                var index  = list_weapon.FindIndex(e => e.ID == m_weapon_id);
                for(int i = 1; i < list_weapon.Count; ++i)
                {
                    var weapon_index     = (index + i) % list_weapon.Count;
                    var weapon_id        = list_weapon[weapon_index].ID;

                    // ����� ���ݰ����� Ÿ���� �ִ��� ã�´�.
                    var target_id = FindTarget(weapon_id, ref list_target);
                    if (target_id > 0)
                    {
                        // ���� & Ÿ�� ����.
                        new_weapon_id = weapon_id;
                        new_target_id = target_id;
                        break;
                    }
                }
            }

            // ���� & Ÿ�� ����Ǿ����� ������ & UI ����.
            if (new_weapon_id > 0 && new_target_id > 0)
            {
                m_weapon_id    = new_weapon_id;
                m_target_id    = new_target_id;
                m_target_list.Clear();
                m_target_list.AddRange(list_target);

                // UI ����.
                UpdateUI_Preview();
            }


            ListPool<Item>.Return(ref list_weapon);
            ListPool<Int64>.Return(ref list_target);
        }


        
    }


}