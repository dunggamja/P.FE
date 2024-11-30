using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    // 포지션 변경.
    public class Rollback_Entity_Position : Rollback    
    {
        Int64          entity_id = 0;
        (int x, int y) position  = (0, 0);

        public Rollback_Entity_Position(
            Int64          _time_stamp,
            Int64          _entity_id,
            (int x, int y) _position) 
            : base(_time_stamp)
        {
            entity_id = _entity_id;
            position  = _position;
        }

        public override void Undo()
        {
            var entity = EntityManager.Instance.GetEntity(entity_id);
            if (entity != null)
            {
                entity.UpdateCellPosition(position.x, position.y, true);
            }
        }
    }

    public class Rollback_Entity_Damage : Rollback
    {
        public Rollback_Entity_Damage(long _time_stamp) : base(_time_stamp)
        {
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    public class Rollback_Entity_Death : Rollback
    {
        public Rollback_Entity_Death(long _time_stamp) : base(_time_stamp)
        {
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }


}
