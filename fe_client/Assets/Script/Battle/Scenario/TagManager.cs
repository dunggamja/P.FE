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


      public bool IsExistTagOwner(TAG_INFO _tag_info, EnumTagAttributeType _attribute, Func<HashSet<TAG_DATA>, bool> _func_condition = null)
      {
         if (m_repository.TryGetValue(_tag_info, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(_attribute, out var repo_tag_data))
            {
               // 카운트 체크.
               if (repo_tag_data.Count == 0)
                  return false;

               if (_func_condition == null || _func_condition(repo_tag_data))
                  return true;
            }
         }

         return false;
      }

      public bool IsExistTagTarget(TAG_INFO _tag_info, EnumTagAttributeType _attribute, Func<HashSet<TAG_DATA>, bool> _func_condition = null)
      {
         if (m_repository_target.TryGetValue(_tag_info, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(_attribute, out var repo_tag_data))
            {
               // 카운트 체크.
               if (repo_tag_data.Count == 0)
                  return false;


               if (_func_condition == null || _func_condition(repo_tag_data))
                  return true;

            }
         }

         return false;
      }

      public bool IsExistTagRelation(TAG_INFO _tag_info, TAG_INFO _target_info, Func<HashSet<EnumTagAttributeType>, bool> _func_condition = null)
      {
         if (m_repository_relation.TryGetValue((_tag_info, _target_info), out var repo_attribute))
         {
            if (repo_attribute.Count == 0)
               return false;

            if (_func_condition == null || _func_condition(repo_attribute))
               return true;
         }
         
         return false;
      }







      public void CollectTagOwner(TAG_INFO _tag_info, EnumTagAttributeType _attribute, List<TAG_DATA> _result)
      {
         if (m_repository.TryGetValue(_tag_info, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(_attribute, out var repo_tag_data))
               _result.AddRange(repo_tag_data);
         }
      }

      public void CollectTagTarget(TAG_INFO _tag_info, EnumTagAttributeType _attribute, List<TAG_DATA> _result)
      {
         if (m_repository_target.TryGetValue(_tag_info, out var repo_attribute))
         {
            if (repo_attribute.TryGetValue(_attribute, out var repo_tag_data))
               _result.AddRange(repo_tag_data);
         }
      }      


      public void CollectTagRelation(TAG_INFO _tag_info, TAG_INFO _target_info, List<EnumTagAttributeType> _result)
      {        
         if (m_repository_relation.TryGetValue((_tag_info, _target_info), out var repo_attribute))
            _result.AddRange(repo_attribute);
      }

   }

}


