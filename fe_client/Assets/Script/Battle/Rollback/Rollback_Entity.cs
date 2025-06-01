using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    // 포지션 변경.
    public class Rollback_Entity_Position : Rollback    
    {
        Int64          EntityID  = 0;
        (int x, int y) Position  = (0, 0);

        public Rollback_Entity_Position(
            int            _turn,
            Int64          _entity_id,
            (int x, int y) _position) 
            : base(_turn)
        {
            EntityID = _entity_id;
            Position = _position;
        }

        public override void Undo()
        {
            var entity = EntityManager.Instance.GetEntity(EntityID);
            if (entity != null)
            {
                entity.UpdateCellPosition(
                    Position.x,
                    Position.y,
                    EnumCellPositionEvent.Move,
                    true);
            }
        }
    }

    public class Rollback_Entity_HP : Rollback
    {
        Int64 EntityID = 0;
        Int32 HP       = 0;
        public Rollback_Entity_HP(
            Int32 _turn,
            Int64 _entity_id,
            Int32 _hp)
             : base(_turn)
        {
            EntityID = _entity_id;
            HP        = _hp;
        }

        public override void Undo()
        {
            var entity = EntityManager.Instance.GetEntity(EntityID);
            if (entity != null)
            {
                entity.StatusManager.Status.SetPoint(EnumUnitPoint.HP, HP);
            }
        }
    }

    public class Rollback_Entity_Death : Rollback
    {
        public Rollback_Entity_Death(
            Int32 _turn)
        : base(_turn)
        {
            // entity
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }


}
