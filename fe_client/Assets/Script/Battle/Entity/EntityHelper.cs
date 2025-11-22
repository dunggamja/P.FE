using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    public static class EntityHelper
    {
        public static void CreateProcess(this Entity _entity)
        {
            if (_entity == null)
                return;

            _entity.CreateHUD();
            
            WorldObjectManager.Instance.CreateObject(_entity.ID).Forget();   
        }


        public static void DeleteProcess(this Entity _entity)
        {
            if (_entity == null)
                return;

            // 
            _entity.Reset();

            // ������Ʈ ����.
            WorldObjectManager.Instance.DeleteObject(_entity.ID);
            EntityManager.Instance.Remove(_entity.ID);
        }

        // public static (string table, string key) GetLocalizeName(this Entity _entity)
        // {
        //     if (_entity == null)
        //         return (string.Empty, string.Empty);
        //     DataManager.Instance.MapSettingSheet.GetLocalizeName(_entity.ID);
        // }
    }
}