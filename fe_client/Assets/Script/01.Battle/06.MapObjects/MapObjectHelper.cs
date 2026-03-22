using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

namespace Battle
{
   public static class MapObjectHelper
   {

      //  static void Test_Collect_MapObject(Entity _entity, List<MapObject> _list_map_object)
      //  {
      //    if (_entity == null || _list_map_object == null)
      //     return;
      //  }


       
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
            // 그냥 컷씬 조건체크로 변경.
            // var verify_tag =
            //     TagManager.Instance.IsExistTagRelation(
            //         TAG_INFO.Create(_entity), 
            //         TAG_INFO.Create(map_object), 
            //         EnumTagAttributeType.POSITION_VISIT);
            // if (verify_tag)
            //     _list_map_object.Add(map_object);


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
            // 그냥 컷씬 조건체크로 변경.
            // var is_enable =
            //     TagManager.Instance.IsExistTagRelation(
            //         TAG_INFO.Create(_entity), 
            //         TAG_INFO.Create(map_object), 
            //         EnumTagAttributeType.POSITION_EXIT);

            if (CutsceneManager.Instance.VerifyPlayEvent(
                CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnMapObjectExit, map_object.ID)))
                _list_map_object.Add(map_object);
          }
       }

   }
}
