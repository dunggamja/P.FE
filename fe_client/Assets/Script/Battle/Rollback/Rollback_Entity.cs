using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Rollback_Entity_Position : IRollback
    {
        Int64          _entity_id = 0;
        (int x, int y) _position  = (0, 0);

        public override void Rollback()
        {
            var entity = EntityManager.Instance.GetEntity(_entity_id);
            if (entity != null)
            {
                entity.UpdateCellPosition(_position.x, _position.y, true);
            }
        }
    }

}
