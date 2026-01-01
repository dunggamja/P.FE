using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   


namespace Battle
{
   public struct TAG_DATA
   {
      public TAG_INFO              TagInfo      { get; private set; }
      public EnumTagAttributeType  Attribute   { get; private set; }
      public TAG_INFO              TargetInfo   { get; private set; }

      public static TAG_DATA Create(TAG_INFO _tag_info, EnumTagAttributeType _attribute, TAG_INFO _target_info)
      {
         return new TAG_DATA { TagInfo = _tag_info, Attribute = _attribute, TargetInfo = _target_info };
      }
   }


   public class TagManager : Singleton<TagManager>
   {
      const int MAX_RECURSIVE_DEPTH = 10;
      
      Dictionary<TAG_INFO, Dictionary<EnumTagAttributeType, HashSet<TAG_DATA>>> m_repository          = new();

      Dictionary<TAG_INFO, Dictionary<EnumTagAttributeType, HashSet<TAG_DATA>>> m_repository_target   = new();

      Dictionary<(TAG_INFO, TAG_INFO), HashSet<EnumTagAttributeType>>           m_repository_relation = new();


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


      public bool IsExistTagOwner(Entity _owner_entity, EnumTagAttributeType _attribute)
      {
         if (_owner_entity == null)
            return false;

         using var list_result = ListPool<TAG_DATA>.AcquireWrapper();
         CollectTagOwner(TAG_INFO.Create(_owner_entity), _attribute, list_result.Value, true, 0);

         return list_result.Value.Count > 0;
      }

      public bool IsExistTagTarget(Entity _target_entity, EnumTagAttributeType _attribute)
      {
         if (_target_entity == null)
            return false;

         using var list_result = ListPool<TAG_DATA>.AcquireWrapper();
         CollectTagTarget(TAG_INFO.Create(_target_entity), _attribute, list_result.Value, true, 0);

         return list_result.Value.Count > 0;
      }


      public bool IsExistTagRelation(
         TAG_INFO             _tag_info, 
         TAG_INFO             _target_info, 
         EnumTagAttributeType _arg0,         
         bool                 _recursive_hierarchy = false)
      {
         using var list_result = ListPool<EnumTagAttributeType>.AcquireWrapper();
         CollectTagRelation(_tag_info, _target_info, list_result.Value, _recursive_hierarchy, 0);

         return list_result.Value.Contains(_arg0);
      }

      public bool IsExistTagRelation(Entity _owner_entity, Entity _target_entity, EnumTagAttributeType _arg0)
      {
         if (_owner_entity == null || _target_entity == null)
            return false;

         return IsExistTagRelation(TAG_INFO.Create(_owner_entity), TAG_INFO.Create(_target_entity), _arg0, true);
      }


      public void CollectTagOwner(
         TAG_INFO             _tag_info, 
         EnumTagAttributeType _attribute, 
         List<TAG_DATA>       _result, 
         bool                 _recursive_hierarchy = false,
         int                  _recursive_depth     = 0)
      {
         // 최대 재귀 횟수 체크.
         if (MAX_RECURSIVE_DEPTH <= _recursive_depth)
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


         // 상위 계층까지 조회할 경우.
         if (_recursive_hierarchy)
         {
            // 상위 계층 태그 데이터 컬렉트.
            using var list_hierarchy = ListPool<TAG_INFO>.AcquireWrapper();
            CollectTagHierarchy(_tag_info, list_hierarchy.Value, false);

            // 상위 계층도 재귀적으로 순회.
            foreach(var tag_parent in list_hierarchy.Value)
            {
               CollectTagOwner(tag_parent, _attribute, _result, true, _recursive_depth + 1);
            }
         }
      }

      public void CollectTagTarget(
         TAG_INFO             _tag_info, 
         EnumTagAttributeType _attribute, 
         List<TAG_DATA>       _result, 
         bool                 _recursive_hierarchy = false,
         int                  _recursive_depth     = 0)
      {
         // 최대 재귀 횟수 체크.
         if (MAX_RECURSIVE_DEPTH <= _recursive_depth)
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


         // 상위 계층까지 조회할 경우.
         if (_recursive_hierarchy)
         {
            // 상위 계층 태그 데이터 컬렉트.
            using var list_hierarchy = ListPool<TAG_INFO>.AcquireWrapper();
            CollectTagHierarchy(_tag_info, list_hierarchy.Value, false);

            // 상위 계층도 재귀적으로 순회.
            foreach(var tag_parent in list_hierarchy.Value)
            {
               CollectTagTarget(tag_parent, _attribute, _result, true, _recursive_depth + 1);
            }
         }
      }      


      public void CollectTagRelation(
         TAG_INFO                   _tag_info, 
         TAG_INFO                   _target_info, 
         List<EnumTagAttributeType> _result,
         bool                       _recursive_hierarchy = false,
         int                        _recursive_depth     = 0)
      {
         if (MAX_RECURSIVE_DEPTH <= _recursive_depth)
         {
            Debug.LogError($"CollectTagRelation: MAX_RECURSIVE_DEPTH <= _recursive_depth, TagInfo: {_tag_info.TagType}.{_tag_info.TagValue}, TargetInfo: {_target_info.TagType}.{_target_info.TagValue}, RecursiveDepth: {_recursive_depth}");
            return;
         }


         using var list_owner  = ListPool<TAG_INFO>.AcquireWrapper();
         using var list_target = ListPool<TAG_INFO>.AcquireWrapper();

         list_owner.Value.Add(_tag_info);
         list_target.Value.Add(_target_info);

         //  Owner/Target 의 상위 계층 수집. 
         if (_recursive_hierarchy)
         {
            CollectTagHierarchy(_tag_info,    list_owner.Value,  true, _recursive_depth + 1);
            CollectTagHierarchy(_target_info, list_target.Value, true, _recursive_depth + 1);
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

      private void CollectTagHierarchy(
         TAG_INFO             _tag_info, 
         List<TAG_INFO>       _result, 
         bool                 _recursive_hierarchy = false,
         int                  _recursive_depth     = 0)
      {
         if (MAX_RECURSIVE_DEPTH <= _recursive_depth)
         {
            Debug.LogError($"CollectTagHierarchy: MAX_RECURSIVE_DEPTH <= _recursive_depth, TagInfo: {_tag_info.TagType}.{_tag_info.TagValue}, RecursiveDepth: {_recursive_depth}");
            return;
         }


         // 해당 태그를 소유한 태그 목록을 조회합니다.
         using var list_hierarchy = ListPool<TAG_INFO>.AcquireWrapper();
         if (m_repository_target.TryGetValue(_tag_info, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(EnumTagAttributeType.ENTITY_HIERARCHY, out var repo_tag_data))
            {
                foreach(var tag_data in repo_tag_data)
                {
                    list_hierarchy.Value.Add(tag_data.TagInfo);
                }
            }
         }

         _result.AddRange(list_hierarchy.Value);


         // 상위 계층까지 조회할 경우.
         if (_recursive_hierarchy)
         {
            foreach(var tag_parent in list_hierarchy.Value)
            {
               CollectTagHierarchy(tag_parent, _result, true, _recursive_depth + 1);
            }
         }
      }

   }

}


