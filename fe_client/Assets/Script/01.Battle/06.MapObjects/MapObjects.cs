using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

namespace Battle
{

   public struct MapObject : IEquatable<MapObject>
   {
      public Int64          ID           { get; private set; }
      public (int x, int y) Cell         { get; private set; }
      // public string         CutsceneName { get; private set; }


      public static MapObject Create(Int64 _id, int _x, int _y)
      {
         var interaction          = new MapObject();
         interaction.ID           = _id;
         interaction.Cell         = (_x, _y);
         // interaction.CutsceneName = _cutscene_name;
         return interaction;
      }

      public static bool operator ==(MapObject _left, MapObject _right)
      {
         return _left.Equals(_right);
      }

      public static bool operator !=(MapObject _left, MapObject _right)
      {
         return !_left.Equals(_right);
      }

      public bool Equals(MapObject _other)
      {
         return ID == _other.ID && Cell == _other.Cell;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(ID, Cell);
      }

      public override bool Equals(object _other)
      {
         if (_other is MapObject map_object)
            return Equals(map_object);

         return false;
      }
   }

   public class MapObjectManager : Singleton<MapObjectManager>
   {
      private Dictionary<Int64, MapObject>                   m_repository         = new();
      private Dictionary<(int x, int y), HashSet<MapObject>> m_repository_by_cell = new();

      public void Collect((int x, int y) _cell, List<MapObject> _result)
      {
         if (m_repository_by_cell.TryGetValue(_cell, out var map_objects) == false)
            return;

         if (map_objects == null || map_objects.Count == 0)
            return;

         _result.AddRange(map_objects);
      }

      public bool AddMapObject(MapObject _map_object)
      {
         if (_map_object == null)
            return false;
         
         RemoveMapObject(_map_object.ID);


         if (m_repository_by_cell.TryGetValue(_map_object.Cell, out var interactions) == false)
         {
            interactions = new HashSet<MapObject>();
            m_repository_by_cell.Add(_map_object.Cell, interactions);
         }

         interactions.Add(_map_object);
         m_repository.Add(_map_object.ID, _map_object);
         return true;
      }

      public void RemoveMapObject(Int64 _id)
      {
         var map_object = SeekMapObject(_id);

         m_repository.Remove(_id);

         if (m_repository_by_cell.TryGetValue(map_object.Cell, out var interactions))
         {
            interactions.Remove(map_object);
         }
      }

      public MapObject SeekMapObject(Int64 _id)
      {
         return m_repository.TryGetValue(_id, out var map_object) ? map_object : default;
      }


      public void Clear()
      {
         m_repository_by_cell.Clear();
      }
   }



}

