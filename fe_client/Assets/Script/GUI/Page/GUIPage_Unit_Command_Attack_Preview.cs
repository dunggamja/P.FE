using System;
using System.Collections.Generic;
using Battle;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent)
    )]
public class GUIPage_Unit_Command_Attack_Preview : GUIPage, IEventReceiver
{



    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }
        public Int64 TargetID { get; private set; }
        public Int64 WeaponID { get; private set; }

        private PARAM(Int64 _entity_id, Int64 _target_id, Int64 _weapon_id) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/page/unit_command_attack_preview", 

            // gui type
            EnumGUIType.Screen,

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







    private Int64 m_entity_id = 0;     
    private Int64 m_target_id = 0;
    private Int64 m_weapon_id = 0;

    public void OnReceiveEvent(IEventParam _event)
    {
        // throw new NotImplementedException();
    }

    protected override void OnOpen(GUIOpenParam _param)
    {
        // throw new NotImplementedException();

        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;
        m_target_id = param?.TargetID ?? 0;
        m_weapon_id = param?.WeaponID ?? 0;


        UpdatePreview(m_entity_id, m_target_id, m_weapon_id);

        // CombatSystemManager.Instance.Setup()

    }

    protected override void OnLoop()
    {
        base.OnLoop();

        UpdateDrawRange();
    }

    protected override void OnClose()
    {
        // throw new NotImplementedException();
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


    void UpdatePreview(Int64 _attacker_id, Int64 _target_id, Int64 _weapon_id)
    {
        var result = CombatHelper.Run_Plan(
            _attacker_id, 
            _target_id, 
            _weapon_id);


        if (result == null)
        {
            Debug.LogError("CombatHelper.Run_Plan() is null");
            return;
        }


        // 공격자 표시.
        m_preview_attacker.Initialize(
            result.Attacker.EntityID,
            result.Attacker.WeaponID,
            result.Attacker.Damage,
            result.Attacker.CriticalRate,
            result.Attacker.HitRate,
            result.Attacker.HP_Before,
            result.Attacker.HP_After);

        // 수비자 표시.
        m_preview_defender.Initialize(
            result.Defender.EntityID,
            result.Defender.WeaponID,
            result.Defender.Damage,
            result.Defender.CriticalRate,
            result.Defender.HitRate,
            result.Defender.HP_Before,
            result.Defender.HP_After);

        // 공격 시퀀스 표시.
        var temp = ListPool<Transform>.Acquire();
        for (int i = 0; i < m_grid_attack_root.transform.childCount; i++)
        {
            temp.Add(m_grid_attack_root.transform.GetChild(i));        
        }

        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] != null)
                GameObject.Destroy(temp[i].gameObject);
        }


        ListPool<Transform>.Return(temp);

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

        // 무기 범위를 그려줍니다.
        BattleSystemManager.Instance.DrawRange.DrawRange
        (
            (int)Battle.MoveRange.EnumDrawFlag.AttackRange,
            _entityID:          m_entity_id,
            _use_base_position: false,
            _use_weapon_id:     m_weapon_id
        );
    }

}