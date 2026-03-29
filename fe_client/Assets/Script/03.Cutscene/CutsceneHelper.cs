using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;


public static class CutsceneHelper
{
   // 현재 위치서 대화 가능한 유닛들 컬렉트.
   public static void Collect_Talk(Entity _entity, List<Entity> _list_entity)
   {
      if (_entity == null || _list_entity == null)
         return;


      var pivot_cell = _entity.Cell;

      // 거리가 1만큼 떨어진 유닛들을 모아봅시다.
      using var list_neighbor = ListPool<Entity>.AcquireWrapper();
      EntityManager.Instance.Collect(list_neighbor.Value,
      e => PathAlgorithm.Distance(e.Cell, pivot_cell) == 1);

      foreach(var e in list_neighbor.Value)
      {
         // 대화가능한 대상인지 체크합니다.
         if (CutsceneManager.Instance.VerifyPlayEvent(
             CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnTalkCommand, e.ID)))
             _list_entity.Add(e);
      }

   }


   // 현재 위치서 방문 가능한 맵 오브젝트를 컬렉트.
   public static void Collect_Visit(Entity _entity, List<MapObject> _list_map_object)
   {
      if (_entity == null)
      return;

      // 유닛이 위치한 좌표의 맵 오브젝트들을 가져옵니다.
      using var list_map_object = ListPool<MapObject>.AcquireWrapper();
      MapObjectManager.Instance.Collect_By_Cell(_entity.Cell, list_map_object.Value);


      foreach(var map_object in list_map_object.Value)
      {
      if (CutsceneManager.Instance.VerifyPlayEvent(
            CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnMapObjectVisit, map_object.ID)))
            _list_map_object.Add(map_object);
      }
   }

   // 현재 위치서 이탈 가능한 맵 오브젝트를 컬렉트.
   public static void Collect_Exit(Entity _entity, List<MapObject> _list_map_object)
   {
      if (_entity == null)
      return;
      
      using var list_map_object = ListPool<MapObject>.AcquireWrapper();
      MapObjectManager.Instance.Collect_By_Cell(_entity.Cell, list_map_object.Value);

      foreach(var map_object in list_map_object.Value)
      {
      if (CutsceneManager.Instance.VerifyPlayEvent(
            CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnMapObjectExit, map_object.ID)))
            _list_map_object.Add(map_object);
      }
   }
}