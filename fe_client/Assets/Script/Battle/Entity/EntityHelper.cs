using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public static class EntityHelper
    {
        public static void DeleteProcess(this Entity _entity)
        {
            if (_entity == null)
                return;

            // 좌표 점유 해제. 
            _entity.UpdateCellOccupied(false);

            // 오브젝트 삭제.
            WorldObjectManager.Instance.DeleteObject(_entity.ID);
            EntityManager.Instance.Remove(_entity.ID);
        }
    }
}