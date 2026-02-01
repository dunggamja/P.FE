using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

namespace Battle
{

   public class MapInteraction
   {
      public Int64          ID   { get; private set; } = 0;
      public (int x, int y) Cell { get; private set; } = (0, 0);     


      public static MapInteraction Create(Int64 _id, int _x, int _y)
      {
         var interaction  = new MapInteraction();
         interaction.ID   = _id;
         interaction.Cell = (_x, _y);
         return interaction;
      }

      public void Process_OnCreate()
      {
         
      }

      public void Process_OnDelete()
      {

      }


   }

   public class MapInteractionManager : Singleton<MapInteractionManager>
   {
      private Dictionary<Int64, MapInteraction> m_repository = new();

      public MapInteraction GetInteraction(Int64 _id)
      {
         if (m_repository.TryGetValue(_id, out var interaction))
            return interaction;

         return null;
      }

      public bool AddInteraction(MapInteraction _interaction)
      {
         if (m_repository.ContainsKey(_interaction.ID))
            return false;

         m_repository.Add(_interaction.ID, _interaction);
         return true;
      }

      public bool RemoveInteraction(Int64 _id)
      {
         return m_repository.Remove(_id);
      }

      public void Loop(Action<MapInteraction> _callback)
      {
         if (_callback == null)
            return;

         foreach(var interaction in m_repository.Values)
         {
            _callback(interaction);
         }
      }

      public void Collect(List<MapInteraction> _result, Func<MapInteraction, bool> _callback = null)
      {
         foreach(var interaction in m_repository.Values)
         {
            if (_callback == null || _callback(interaction))
               _result.Add(interaction);
         }
      }

      public MapInteraction Find(Func<MapInteraction, bool> _callback)
      {
         if (_callback == null)
            return null;

         foreach(var interaction in m_repository.Values)
         {
            if (_callback(interaction))
               return interaction;
      
         }

         return null;
      }

      public void Clear()
      {
         m_repository.Clear();
      }
   }


   public static class MapInteractionHelper
   {
      public static void CreateProcess(this MapInteraction _interaction)
      {
         if (MapInteractionManager.Instance.AddInteraction(_interaction) == false)
            return;

         _interaction.Process_OnCreate();
      }

      public static void DeleteProcess(this MapInteraction _interaction)
      {
         if (_interaction == null)
            return;

         _interaction.Process_OnDelete();

         MapInteractionManager.Instance.RemoveInteraction(_interaction.ID);
      }
   }
}

