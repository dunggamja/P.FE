using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

namespace Battle
{

   public struct MapInteraction : IEquatable<MapInteraction>
   {
      public (int x, int y) Cell         { get; private set; }
      public string         CutsceneName { get; private set; }


      public static MapInteraction Create(int _x, int _y, string _cutscene_name)
      {
         var interaction          = new MapInteraction();
         interaction.Cell         = (_x, _y);
         interaction.CutsceneName = _cutscene_name;
         return interaction;
      }

      public static bool operator ==(MapInteraction _left, MapInteraction _right)
      {
         return _left.Equals(_right);
      }

      public static bool operator !=(MapInteraction _left, MapInteraction _right)
      {
         return !_left.Equals(_right);
      }

      public bool Equals(MapInteraction _other)
      {
         return Cell == _other.Cell && CutsceneName == _other.CutsceneName;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(Cell, CutsceneName);
      }

      public override bool Equals(object _other)
      {
         if (_other is MapInteraction interaction)
            return Equals(interaction);

         return false;
      }
   }

   public class MapInteractionManager : Singleton<MapInteractionManager>
   {
      private Dictionary<(int x, int y), HashSet<MapInteraction>> m_repository = new();

      public void Collect((int x, int y) _cell, List<MapInteraction> _result)
      {
         if (m_repository.TryGetValue(_cell, out var interactions) == false)
            return;

         foreach(var interaction in interactions)
         {
            _result.Add(interaction);
         }
      }

      public bool AddInteraction(MapInteraction _interaction)
      {
         if (m_repository.TryGetValue(_interaction.Cell, out var interactions) == false)
         {
            interactions = new HashSet<MapInteraction>();
            m_repository.Add(_interaction.Cell, interactions);
         }

         return interactions.Add(_interaction);
      }

      public bool RemoveInteraction((int x, int y) _cell, string _cutscene_name)
      {
         if (m_repository.TryGetValue(_cell, out var interactions) == false)
            return false;

         
         return 0 < interactions.RemoveWhere(e => e.CutsceneName == _cutscene_name);
      }


      public void Clear()
      {
         m_repository.Clear();
      }
   }


   public static class MapInteractionHelper
   {
      public static void CollectExecutableCutscene(Entity _entity, (int x, int y) _cell, List<string> _list_cutscene)
      {
         if (_entity == null || _cell.x < 0 || _cell.y < 0)
            return;

         using var list_collect = ListPool<MapInteraction>.AcquireWrapper();

         // 해당 좌표에 셋팅된 상호작용 목록을 가져옵니다.
         MapInteractionManager.Instance.Collect(_cell, list_collect.Value);  

         // 실행 가능한 컷씬 목록을 가져옵시다.
         foreach(var interaction in list_collect.Value)
         {
            if (CutsceneManager.Instance.VerifyPlayCutscene(interaction.CutsceneName))
               _list_cutscene.Add(interaction.CutsceneName);
         }
      }
   }
}

