using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;
   
//TODO: 이것은 다시 Battle로 옮기는게 나을거 같은...

// namespace Battle
// {
   public struct TAG_DATA : IEquatable<TAG_DATA>
   {
      public TAG_INFO              TagInfo      { get; private set; }
      public EnumTagAttributeType  Attribute   { get; private set; }
      public TAG_INFO              TargetInfo   { get; private set; }

      public static TAG_DATA Create(TAG_INFO _tag_info, EnumTagAttributeType _attribute, TAG_INFO _target_info)
      {
         return new TAG_DATA { TagInfo = _tag_info, Attribute = _attribute, TargetInfo = _target_info };
      }

      public static bool operator ==(TAG_DATA _left, TAG_DATA _right)
      {
         return (_left.TagInfo, _left.Attribute, _left.TargetInfo) == (_right.TagInfo, _right.Attribute, _right.TargetInfo);
      }

      public static bool operator !=(TAG_DATA _left, TAG_DATA _right)
      {
         return (_left.TagInfo, _left.Attribute, _left.TargetInfo) != (_right.TagInfo, _right.Attribute, _right.TargetInfo);
      }

      public bool Equals(TAG_DATA other)
      {
         return this == other;
      }

      public override bool Equals(object obj)
      {
         if (obj is TAG_DATA tag_data)
            return Equals(tag_data);

         return false;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(TagInfo, Attribute, TargetInfo);
      }
    }


   

   public class TagManager : Singleton<TagManager>
   {
      
      // 계층이 N개 이상 누적될 일이 없다고 가정하고 설정.
      // TODO: SetTag 시점에 최대 깊이 체크가 되어야 할것 같음.
      const int MAX_RECURSIVE_DEPTH = 20;
      
      Dictionary<TAG_INFO, Dictionary<EnumTagAttributeType, HashSet<TAG_DATA>>> m_repository          = new(); // TAG_INFO -> Attribute -> TAG_DATA

      Dictionary<TAG_INFO, Dictionary<EnumTagAttributeType, HashSet<TAG_DATA>>> m_repository_target   = new(); // TAG_INFO <- Attribute <- TAG_DATA

      Dictionary<(TAG_INFO, TAG_INFO), HashSet<EnumTagAttributeType>>           m_repository_relation = new(); // TAG_INFO -> Attribute -> TAG_INFO


      public bool IsExistTag(TAG_DATA _tag_data)
      {
         if (m_repository.TryGetValue(_tag_data.TagInfo, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(_tag_data.Attribute, out var repo_tag_data))            
               return repo_tag_data.Contains(_tag_data);
         }

         return false;
      }

      public void SetTag(TAG_DATA _tag_data)
      {
         // 이미 있는 Tag일 경우.
         if (IsExistTag(_tag_data))
            return; 

         // Debug.Log($"SetTag: {_tag_data.TagInfo.TagType}.{_tag_data.TagInfo.TagValue}, {_tag_data.Attribute}, {_tag_data.TargetInfo.TagType}.{_tag_data.TargetInfo.TagValue}");

         // 소유자 레포에 추가.
         {
            if (m_repository.TryGetValue(_tag_data.TagInfo, out var repo_attribute) == false)
            {
               repo_attribute = new ();
               m_repository.Add(_tag_data.TagInfo, repo_attribute);
            }

            if (repo_attribute.TryGetValue(_tag_data.Attribute, out var repo_tag_data) == false)
            {
                repo_tag_data = new ();
                repo_attribute.Add(_tag_data.Attribute, repo_tag_data);
            }

            repo_tag_data.Add(_tag_data);
         }

         // 타겟 레포에 추가.
         {
            if (m_repository_target.TryGetValue(_tag_data.TargetInfo, out var repo_attribute) == false)
            {
               repo_attribute = new ();
               m_repository_target.Add(_tag_data.TargetInfo, repo_attribute);
            }

            if (repo_attribute.TryGetValue(_tag_data.Attribute, out var repo_tag_data) == false)
            {
                repo_tag_data = new ();
                repo_attribute.Add(_tag_data.Attribute, repo_tag_data);
            }

            repo_tag_data.Add(_tag_data);
         }

         // 관계 레포에 추가.
         {
            if (m_repository_relation.TryGetValue((_tag_data.TagInfo, _tag_data.TargetInfo), out var repo_attribute) == false)            
            {
               repo_attribute = new ();
               m_repository_relation.Add((_tag_data.TagInfo, _tag_data.TargetInfo), repo_attribute);
            }

            repo_attribute.Add(_tag_data.Attribute);
         }
      }

      public void RemoveTag(TAG_DATA _tag_data)
      {
         // Tag가 없으면.
         if (IsExistTag(_tag_data) == false)
            return;

         // 소유자 레포에서 제거.
         {            
            if (m_repository.TryGetValue(_tag_data.TagInfo, out var repo_attribute))
            {
               if (repo_attribute.TryGetValue(_tag_data.Attribute, out var repo_tag_data))
                   repo_tag_data.Remove(_tag_data);
            }            
         }

         // 타겟 레포에서 제거.
         {
            if (m_repository_target.TryGetValue(_tag_data.TargetInfo, out var repo_attribute))
            {
               if (repo_attribute.TryGetValue(_tag_data.Attribute, out var repo_tag_data))
                  repo_tag_data.Remove(_tag_data);
            }
         }

         // 관계 레포에서 제거.
         {           
            if (m_repository_relation.TryGetValue((_tag_data.TagInfo, _tag_data.TargetInfo), out var repo_attribute))
            {
               repo_attribute.Remove(_tag_data.Attribute);
            }
         }
      }

      // 일단 사용처가 애매한 함수들은 주석처리 해둔다.
      // public bool IsExistTagOwner(Entity _owner_entity, EnumTagAttributeType _attribute)
      // {
      //    if (_owner_entity == null)
      //       return false;
      //    using var list_result = ListPool<TAG_DATA>.AcquireWrapper();
      //    CollectTagOwner(TAG_INFO.Create(_owner_entity), _attribute, list_result.Value, true, 0);
      //    return list_result.Value.Count > 0;
      // }
      // 일단 사용처가 애매한 함수들은 주석처리 해둔다.
      // public bool IsExistTagTarget(Entity _target_entity, EnumTagAttributeType _attribute)
      // {
      //    if (_target_entity == null)
      //       return false;
      //    using var list_result = ListPool<TAG_DATA>.AcquireWrapper();
      //    CollectTagTarget(TAG_INFO.Create(_target_entity), _attribute, list_result.Value, true, 0);
      //    return list_result.Value.Count > 0;
      // }


      public bool IsExistTagRelation(
         TAG_INFO             _tag_info, 
         TAG_INFO             _target_info, 
         EnumTagAttributeType _attribute)
      {
         using var list_result = ListPool<EnumTagAttributeType>.AcquireWrapper();
         CollectTagRelation(_tag_info, _target_info, list_result.Value);

         return list_result.Value.Contains(_attribute);
      }

      public bool IsExistTagRelation(Entity _owner_entity, Entity _target_entity, EnumTagAttributeType _attribute)
      {
         if (_owner_entity == null || _target_entity == null)
            return false;

         return IsExistTagRelation(TAG_INFO.Create(_owner_entity), TAG_INFO.Create(_target_entity), _attribute);
      }


      public void CollectTagOwner(
         TAG_INFO                  _tag_info, 
         EnumTagAttributeType      _attribute, 
         List<TAG_DATA>            _result, 
         int                       _recursive_depth = MAX_RECURSIVE_DEPTH)
      {
         // 최대 재귀 횟수 체크.
         if (_recursive_depth <= 0)
         {
            Debug.LogError($"CollectTagOwner: MAX_RECURSIVE_DEPTH <= _recursive_depth, TagInfo: {_tag_info.TagType}.{_tag_info.TagValue}, Attribute: {_attribute}, RecursiveDepth: {_recursive_depth}");
            return;
         }

         // 소유자 데이터 
         if (m_repository.TryGetValue(_tag_info, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(_attribute, out var repo_tag_data))
                  _result.AddRange(repo_tag_data);
         }


         // 재귀적으로 조회할 경우.
         {
            // 계층 태그 데이터 컬렉트.
            using var list_tag = ListPool<TAG_INFO>.AcquireWrapper();
            CollectTag_Hierarchy(_tag_info, list_tag.Value, EnumTagRecursiveDirection.Downward, 1);

            // 재귀적으로 순회.
            foreach(var e in list_tag.Value)
            {
               CollectTagTarget(e, _attribute, _result, _recursive_depth - 1);
            }
         }
      }

      public void CollectTagTarget(
         TAG_INFO                  _tag_info, 
         EnumTagAttributeType      _attribute, 
         List<TAG_DATA>            _result, 
         int                       _recursive_depth     = MAX_RECURSIVE_DEPTH)
         // EnumTagRecursiveDirection _recursive_direction = EnumTagRecursiveDirection.Owner,
      {
         // 최대 재귀 횟수 체크.
         if (_recursive_depth <= 0)
         {
            Debug.LogError($"CollectTagTarget: MAX_RECURSIVE_DEPTH < _recursive_depth, TagInfo: {_tag_info.TagType}.{_tag_info.TagValue}, Attribute: {_attribute}, RecursiveDepth: {_recursive_depth}");
            return;
         }


         // 타겟 조회 후 없으면 종료 처리.
         if (m_repository_target.TryGetValue(_tag_info, out var repo_attribute))
         {
            // 타겟의 태그 데이터 컬렉트.
            if (repo_attribute.TryGetValue(_attribute, out var repo_tag_data))
                  _result.AddRange(repo_tag_data);
         }


         // 재귀적으로 조회할 경우.
         {
            // 계층 태그 데이터 컬렉트.
            using var list_tag = ListPool<TAG_INFO>.AcquireWrapper();
            CollectTag_Hierarchy(_tag_info, list_tag.Value, EnumTagRecursiveDirection.Downward, 1);

            // 재귀적으로 순회.
            foreach(var e in list_tag.Value)
            {
               CollectTagTarget(e, _attribute, _result, _recursive_depth - 1);
            }
         }
      }      


      public void CollectTagRelation(
         TAG_INFO                   _tag_info, 
         TAG_INFO                   _target_info, 
         List<EnumTagAttributeType> _result)
      {
         // if (_recursive_depth <= 0)
         // {
         //    Debug.LogError($"CollectTagRelation: MAX_RECURSIVE_DEPTH <= _recursive_depth, TagInfo: {_tag_info.TagType}.{_tag_info.TagValue}, TargetInfo: {_target_info.TagType}.{_target_info.TagValue}, RecursiveDepth: {_recursive_depth}");
         //    return;
         // }


         using var list_owner  = ListPool<TAG_INFO>.AcquireWrapper();
         using var list_target = ListPool<TAG_INFO>.AcquireWrapper();

         list_owner.Value.Add(_tag_info);
         list_target.Value.Add(_target_info);

         //  Owner/Target 의 상위 계층 수집. 
         {
            CollectTag_Hierarchy(_tag_info,    list_owner.Value,  EnumTagRecursiveDirection.Upward);
            CollectTag_Hierarchy(_target_info, list_target.Value, EnumTagRecursiveDirection.Upward);
         }

         // 관계 목록 수집
         foreach(var owner in list_owner.Value)
         {
            foreach(var target in list_target.Value)
            {
               if (m_repository_relation.TryGetValue((owner, target), out var repo_attribute))
               {
                  _result.AddRange(repo_attribute);
               }
            }
         }
      }

      //  현재는 Hierarchy를 ENTITY만 사용하는 중...;;
      private void CollectTag_Hierarchy(
         TAG_INFO                  _tag_info, 
         List<TAG_INFO>            _result, 
         EnumTagRecursiveDirection _recursive_direction,
         int                       _recursive_depth = MAX_RECURSIVE_DEPTH)
      {
         // 최대 깊이 체크.
         if (_recursive_depth <= 0)
         {
            Debug.LogError($"CollectTagHierarchy: MAX_RECURSIVE_DEPTH <= _recursive_depth, TagInfo: {_tag_info.TagType}.{_tag_info.TagValue}, RecursiveDepth: {_recursive_depth}");
            return;
         }

         // 계층 태그 정보들 컬렉트.
         using var list_hierarchy = ListPool<TAG_INFO>.AcquireWrapper();
         
         // 재귀 방향에 따라 레포지토리를 다르게 선택합시다.         
         var tag_repository = _recursive_direction == EnumTagRecursiveDirection.Upward ? m_repository_target : m_repository;
         if (tag_repository.TryGetValue(_tag_info, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(EnumTagAttributeType.HIERARCHY, out var repo_tag_data))
            {
                foreach(var tag_data in repo_tag_data)
                {
                    list_hierarchy.Value.Add(tag_data.TagInfo);
                }
            }
         }

         // 결과값 추가.
         _result.AddRange(list_hierarchy.Value);

         // 재귀 탐색 진행.
         if (1 < _recursive_depth)
         {
            foreach(var tag_parent in list_hierarchy.Value)
            {
               CollectTag_Hierarchy(tag_parent, _result, _recursive_direction, _recursive_depth - 1);
            }
         }
      }

   }

// }


