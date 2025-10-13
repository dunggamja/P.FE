using UnityEngine;
using System;
using System.Collections.Generic;


namespace Battle
{

   [CreateAssetMenu(
      fileName = "FactionSerialize", 
      menuName = "ScriptableObjects/Battle/FactionSerialize")
   ]
   public class FactionSerialize : ScriptableObject
   {
      [Serializable]
      public struct FactionSetting
      {
         public string            Memo;
         public EnumCommanderType CommanderType;         
         public int               Faction;
      }


      [Serializable]
      public struct AllianceSetting
      {
         public string Memo;

         public int    Faction_1;
         public int    Faction_2;
      }

      [SerializeField]
      private List<FactionSetting>  m_faction_settings = new();

      [SerializeField]
      private List<AllianceSetting> m_alliance_settings = new();

      public EnumCommanderType GetCommanderType(int _faction)
      {
         foreach(var e in m_faction_settings)
         {
            if (e.Faction == _faction)
               return e.CommanderType;
         }

         return EnumCommanderType.None;
      }

      public bool IsAlliance(int _faction_1, int _faction_2)
      {
         foreach(var e in m_alliance_settings)
         {
            if ((e.Faction_1 == _faction_1 && e.Faction_2 == _faction_2) ||
                (e.Faction_1 == _faction_2 && e.Faction_2 == _faction_1))
               return true;
         }

         return false;
      }

      public void SetAlliance(int _faction_1, int _faction_2, bool _set_alliance)
      {
         var is_alliance = IsAlliance(_faction_1, _faction_2);
         if (is_alliance == _set_alliance)
            return;

         if (_set_alliance)
         {
            // 동맹 데이터 추가.
            m_alliance_settings.Add(new AllianceSetting() { Faction_1 = _faction_1, Faction_2 = _faction_2 });
         }
         else
         {
            // 동맹 데이터 제거.
            for (int i = m_alliance_settings.Count - 1; i >= 0; --i)
            {
               if ((m_alliance_settings[i].Faction_1 == _faction_1 && m_alliance_settings[i].Faction_2 == _faction_2) ||
                   (m_alliance_settings[i].Faction_1 == _faction_2 && m_alliance_settings[i].Faction_2 == _faction_1))
               {
                  m_alliance_settings.RemoveAt(i);                  
               }
            }
         }
         
      }
      
   }
}
