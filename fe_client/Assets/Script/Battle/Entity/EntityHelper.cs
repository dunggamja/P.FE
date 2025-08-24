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

            // ��ǥ ���� ����. 
            _entity.UpdateCellOccupied(false);

            // ������Ʈ ����.
            WorldObjectManager.Instance.DeleteObject(_entity.ID);
            EntityManager.Instance.Remove(_entity.ID);
        }
    }
}